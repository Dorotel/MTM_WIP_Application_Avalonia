using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for the inventory transfer interface (Control_TransferTab).
/// Provides comprehensive functionality for transferring inventory items between locations,
/// including search capabilities, quantity specification, batch transfer operations,
/// and complete transaction history tracking.
/// Uses MVVM Community Toolkit for modern .NET patterns.
/// </summary>
public partial class TransferItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;
    private readonly IMasterDataService _masterDataService;
    private readonly ILogger<TransferItemViewModel> _logger;
    private readonly ISuccessOverlayService? _successOverlayService;
    private readonly IPrintService? _printService;
    private readonly INavigationService? _navigationService;

    #region Observable Collections

    /// <summary>
    /// Available part options for filtering (DEPRECATED - Use PartIds)
    /// </summary>
    public ObservableCollection<string> PartOptions { get; } = new();

    /// <summary>
    /// Available operation options for refined filtering (DEPRECATED - Use Operations)
    /// </summary>
    public ObservableCollection<string> OperationOptions { get; } = new();

    /// <summary>
    /// Available part IDs from MasterDataService
    /// </summary>
    public ObservableCollection<string> PartIds => _masterDataService?.PartIds ?? new ObservableCollection<string>();

    /// <summary>
    /// Available operations from MasterDataService
    /// </summary>
    public ObservableCollection<string> Operations => _masterDataService?.Operations ?? new ObservableCollection<string>();

    /// <summary>
    /// Available locations from MasterDataService
    /// </summary>
    public ObservableCollection<string> Locations => _masterDataService?.Locations ?? new ObservableCollection<string>();

    /// <summary>
    /// Available locations for transfer destinations
    /// </summary>
    public ObservableCollection<string> LocationOptions { get; } = new();

    /// <summary>
    /// Current inventory items displayed in the DataGrid
    /// </summary>
    public ObservableCollection<TransferInventoryItem> InventoryItems { get; } = new();

    #endregion

    #region Search Criteria Properties

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Part selection cannot exceed 50 characters")]
    private string? _selectedPart;

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Operation selection cannot exceed 50 characters")]
    private string? _selectedOperation;

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Part text cannot exceed 50 characters")]
    private string _partText = string.Empty;

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Operation text cannot exceed 50 characters")]
    private string _operationText = string.Empty;

    #endregion

    #region Transfer Configuration Properties

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Destination location cannot exceed 50 characters")]
    private string? _selectedToLocation;

    [ObservableProperty]
    [StringLength(50, ErrorMessage = "Location text cannot exceed 50 characters")]
    private string _toLocationText = string.Empty;

    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Transfer quantity must be at least 1")]
    private int _transferQuantity = 1;

    [ObservableProperty]
    [StringLength(10, ErrorMessage = "Quantity text cannot exceed 10 characters")]
    private string _transferQuantityText = "1";

    /// <summary>
    /// Called when TransferQuantityText property changes - handles text validation and conversion
    /// </summary>
    partial void OnTransferQuantityTextChanged(string value)
    {
        // Convert text to integer, handling empty/invalid inputs
        if (string.IsNullOrWhiteSpace(value))
        {
            TransferQuantity = 0; // 0 represents empty/invalid state
        }
        else if (int.TryParse(value, out int quantity) && quantity > 0)
        {
            TransferQuantity = quantity;
        }
        else
        {
            TransferQuantity = 0; // Invalid input - set to 0 to trigger validation error
        }

        // Notify property changes for validation
        OnPropertyChanged(nameof(HasQuantityValidationError));
        OnPropertyChanged(nameof(IsTransferQuantityValid));
        OnPropertyChanged(nameof(CanTransfer));
    }

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Maximum transfer quantity must be 0 or greater")]
    private int _maxTransferQuantity = 0;

    #endregion

    #region Selection and State Properties

    [ObservableProperty]
    private TransferInventoryItem? _selectedInventoryItem;

    /// <summary>
    /// Status message for MainView status bar integration
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Collection of selected inventory items for batch transfer operations
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<TransferInventoryItem> _selectedInventoryItems = new();

    [ObservableProperty]
    private bool _isLoading;

    #endregion

    #region QuickButtons Integration Properties

    /// <summary>
    /// Controls whether the QuickActions panel is expanded
    /// </summary>
    [ObservableProperty]
    private bool _isQuickActionsPanelExpanded;

    /// <summary>
    /// Controls visibility of advanced panels
    /// </summary>
    [ObservableProperty]
    private bool _isAdvancedPanelVisible;

    /// <summary>
    /// QuickButtons ViewModel for transfer operations
    /// </summary>
    public QuickButtonsViewModel? QuickButtonsViewModel { get; set; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    public bool HasInventoryItems => InventoryItems.Count > 0;

    /// <summary>
    /// Indicates if the "Nothing Found" indicator should be shown
    /// </summary>
    public bool ShowNothingFoundIndicator => !IsLoading && !HasInventoryItems && _hasSearchBeenExecuted;

    /// <summary>
    /// Tracks if a search has been executed to determine when to show nothing found indicator
    /// </summary>
    private bool _hasSearchBeenExecuted = false;

    /// <summary>
    /// Indicates if there's a location validation error (same source and destination)
    /// </summary>
    public bool HasLocationValidationError => !string.IsNullOrWhiteSpace(SelectedToLocation) &&
                                              !ValidateTransferDestination();

    /// <summary>
    /// Indicates if there's a quantity validation error (exceeds available)
    /// </summary>
    public bool HasQuantityValidationError => TransferQuantity > MaxTransferQuantity || TransferQuantity <= 0;

    /// <summary>
    /// Validation state for transfer quantity - must be positive and within available limits
    /// </summary>
    public bool IsTransferQuantityValid => TransferQuantity > 0 && TransferQuantity <= MaxTransferQuantity;

    /// <summary>
    /// Indicates if transfer operation can be performed
    /// </summary>
    public bool CanTransfer => (SelectedInventoryItem != null || SelectedInventoryItems.Count > 0) &&
                              !string.IsNullOrWhiteSpace(SelectedToLocation) &&
                              TransferQuantity > 0 &&
                              !IsLoading &&
                              ValidateTransferDestination();

    /// <summary>
    /// Indicates if search operations can be performed
    /// </summary>
    public bool CanSearch => !IsLoading;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when items are successfully transferred
    /// </summary>
    public event EventHandler<ItemsTransferredEventArgs>? ItemsTransferred;

    /// <summary>
    /// Event fired when panel toggle is requested
    /// </summary>
    public event EventHandler? PanelToggleRequested;

    /// <summary>
    /// Event fired when panel expand is requested
    /// </summary>
    public event EventHandler? PanelExpandRequested;

    /// <summary>
    /// Event fired when SuccessOverlay should be shown
    /// </summary>
    public event EventHandler<SuccessOverlayEventArgs>? SuccessOverlayRequested;

    /// <summary>
    /// Event fired when progress status should be reported to MainView
    /// </summary>
    public event EventHandler<ProgressReportEventArgs>? ProgressReported;

    #endregion

    #region Constructor

    public TransferItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        IMasterDataService masterDataService,
        ILogger<TransferItemViewModel> logger,
        ISuccessOverlayService? successOverlayService = null,
        IPrintService? printService = null,
        INavigationService? navigationService = null) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _successOverlayService = successOverlayService; // Optional service
        _printService = printService;
        _navigationService = navigationService;

        _logger.LogInformation("TransferItemViewModel initialized with dependency injection");

        _ = LoadComboBoxDataAsync(); // Load real data from database
    }

    #endregion

    #region Command Implementations

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] PropertyChanged: {e.PropertyName}");
        _logger.LogDebug("[TRANSFER-DEBUG] PropertyChanged: {PropertyName}", e.PropertyName);

        switch (e.PropertyName)
        {
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] InventoryItems changed - Count: {InventoryItems.Count}, HasItems: {HasInventoryItems}, ShowNothing: {ShowNothingFoundIndicator}");
                _logger.LogInformation("[TRANSFER-DEBUG] InventoryItems changed - Count: {Count}, HasItems: {HasItems}",
                    InventoryItems.Count, HasInventoryItems);
                break;
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] IsLoading changed: {IsLoading} - CanSearch: {CanSearch}, CanTransfer: {CanTransfer}");
                _logger.LogInformation("[TRANSFER-DEBUG] IsLoading changed: {IsLoading}", IsLoading);
                break;
            case nameof(TransferQuantity):
            case nameof(SelectedInventoryItem):
            case nameof(SelectedInventoryItems):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasQuantityValidationError));
                OnPropertyChanged(nameof(HasLocationValidationError));
                UpdateMaxTransferQuantity();

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Selection/Quantity changed:");
                System.Diagnostics.Debug.WriteLine($"  - SelectedItem: {SelectedInventoryItem?.PartId ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"  - SelectedItems Count: {SelectedInventoryItems.Count}");
                System.Diagnostics.Debug.WriteLine($"  - TransferQuantity: {TransferQuantity}");
                System.Diagnostics.Debug.WriteLine($"  - MaxTransferQuantity: {MaxTransferQuantity}");
                System.Diagnostics.Debug.WriteLine($"  - CanTransfer: {CanTransfer}");
                break;
            case nameof(SelectedToLocation):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasLocationValidationError));
                UpdateMaxTransferQuantity();
                ToLocationText = SelectedToLocation ?? string.Empty;

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SelectedToLocation changed: '{SelectedToLocation}' - CanTransfer: {CanTransfer}");

                // Update ToLocation for all inventory items when destination is selected
                UpdateInventoryItemsToLocation();
                break;
            case nameof(ToLocationText):
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] ToLocationText changed: '{ToLocationText}'");
                _logger.LogDebug("[TRANSFER-DEBUG] ToLocationText changed: '{ToLocationText}'", ToLocationText);
                OnPropertyChanged(nameof(HasLocationValidationError));
                if (!string.IsNullOrEmpty(ToLocationText) && LocationOptions.Contains(ToLocationText))
                    SelectedToLocation = ToLocationText;
                break;
            case nameof(MaxTransferQuantity):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasQuantityValidationError));
                UpdateMaxTransferQuantity();

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] MaxTransferQuantity changed: {MaxTransferQuantity}");
                // Ensure transfer quantity doesn't exceed maximum
                if (TransferQuantity > MaxTransferQuantity)
                {
                    TransferQuantity = Math.Max(1, MaxTransferQuantity);
                    System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] TransferQuantity adjusted to: {TransferQuantity}");
                }
                break;
            case nameof(SelectedPart):
                PartText = SelectedPart ?? string.Empty;
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SelectedPart changed: '{SelectedPart}' -> PartText: '{PartText}'");
                _logger.LogDebug("[TRANSFER-DEBUG] SelectedPart changed: '{SelectedPart}' -> PartText: '{PartText}'", SelectedPart, PartText);
                break;
            case nameof(SelectedOperation):
                OperationText = SelectedOperation ?? string.Empty;
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SelectedOperation changed: '{SelectedOperation}' -> OperationText: '{OperationText}'");
                _logger.LogDebug("[TRANSFER-DEBUG] SelectedOperation changed: '{SelectedOperation}' -> OperationText: '{OperationText}'", SelectedOperation, OperationText);
                break;
            case nameof(PartText):
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] PartText changed: '{PartText}', PartOptions.Count: {PartOptions.Count}");
                _logger.LogDebug("[TRANSFER-DEBUG] PartText changed: '{PartText}', PartOptions.Count: {Count}", PartText, PartOptions.Count);
                if (!string.IsNullOrEmpty(PartText) && PartOptions.Contains(PartText))
                    SelectedPart = PartText;
                break;
            case nameof(OperationText):
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] OperationText changed: '{OperationText}', OperationOptions.Count: {OperationOptions.Count}");
                _logger.LogDebug("[TRANSFER-DEBUG] OperationText changed: '{OperationText}', OperationOptions.Count: {Count}", OperationText, OperationOptions.Count);
                if (!string.IsNullOrEmpty(OperationText) && OperationOptions.Contains(OperationText))
                    SelectedOperation = OperationText;
                break;
        }
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Executes inventory search based on selected criteria with progress tracking
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task ExecuteSearchAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] ExecuteSearchAsync - Starting search operation");
            IsLoading = true;
            ReportProgress("Searching inventory...", "search", 0);

            _logger.LogInformation("[TRANSFER-DEBUG] ExecuteSearchAsync started - PartText: '{PartText}', OperationText: '{OperationText}', SelectedPart: '{SelectedPart}', SelectedOperation: '{SelectedOperation}'",
                PartText, OperationText, SelectedPart, SelectedOperation);

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Search criteria - PartText: '{PartText}', OperationText: '{OperationText}'");
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Selected criteria - SelectedPart: '{SelectedPart}', SelectedOperation: '{SelectedOperation}'");

            InventoryItems.Clear();
            SelectedInventoryItem = null;
            SelectedInventoryItems.Clear();
            _hasSearchBeenExecuted = true;

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Collections cleared - InventoryItems count: {InventoryItems.Count}");

            _logger.LogInformation("Executing transfer search for Part: {PartId}, Operation: {Operation}",
                SelectedPart, SelectedOperation);

            ReportProgress("Querying database...", "search", 25);

            // Dynamic search based on selection criteria
            System.Data.DataTable result;

            if (!string.IsNullOrWhiteSpace(SelectedPart) && !string.IsNullOrWhiteSpace(SelectedOperation))
            {
                // Search by both part and operation
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Searching by BOTH Part AND Operation: '{SelectedPart}' + '{SelectedOperation}'");
                _logger.LogInformation("[TRANSFER-DEBUG] Searching by Part AND Operation: {Part} + {Operation}", SelectedPart, SelectedOperation);
                result = await _databaseService.GetInventoryByPartAndOperationAsync(SelectedPart, SelectedOperation).ConfigureAwait(false);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                // Search by part only
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Searching by Part ONLY: '{SelectedPart}'");
                _logger.LogInformation("[TRANSFER-DEBUG] Searching by Part only: {Part}", SelectedPart);
                result = await _databaseService.GetInventoryByPartIdAsync(SelectedPart).ConfigureAwait(false);
            }
            else
            {
                // No search criteria specified, don't load anything
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] NO SEARCH CRITERIA specified - returning empty results");
                _logger.LogWarning("[TRANSFER-DEBUG] No search criteria specified for transfer search");
                ReportProgress("No search criteria specified", "search", 0, false, true);
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Database query returned {result?.Rows.Count ?? 0} rows");
            _logger.LogInformation("[TRANSFER-DEBUG] Database query returned {RowCount} rows", result?.Rows.Count ?? 0);
            ReportProgress("Processing results...", "search", 75);

            // Convert DataTable to TransferInventoryItem list first (off UI thread)
            var transferItems = new List<TransferInventoryItem>();
            foreach (System.Data.DataRow row in result.Rows)
            {
                var inventoryItem = new MTM_Shared_Logic.Models.InventoryItem
                {
                    ID = Convert.ToInt32(row["ID"]),
                    PartID = row["PartID"]?.ToString() ?? string.Empty,
                    Location = row["Location"]?.ToString() ?? string.Empty,
                    Operation = row["Operation"]?.ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    ItemType = row["ItemType"]?.ToString() ?? "WIP",
                    ReceiveDate = Convert.ToDateTime(row["ReceiveDate"]),
                    LastUpdated = Convert.ToDateTime(row["LastUpdated"]),
                    User = row["User"]?.ToString() ?? string.Empty,
                    BatchNumber = row["BatchNumber"]?.ToString() ?? string.Empty,
                    Notes = row["Notes"]?.ToString() ?? string.Empty
                };

                // Convert to TransferInventoryItem using our enhanced converter
                var transferItem = ConvertToTransferItem(inventoryItem);
                transferItems.Add(transferItem);

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Converted item: {transferItem.PartId} - {transferItem.Operation} at {transferItem.Location} (Qty: {transferItem.Quantity})");
            }

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Created {transferItems.Count} transfer items, updating UI on UI thread");

            // Update ObservableCollection on UI thread to prevent InvalidOperationException
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] UI Thread - Adding items to InventoryItems collection");
                foreach (var item in transferItems)
                {
                    InventoryItems.Add(item);
                }
                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] UI Thread - InventoryItems.Count now: {InventoryItems.Count}");

                // Trigger property change notifications for debugging
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] UI Thread - HasInventoryItems: {HasInventoryItems}, ShowNothingFoundIndicator: {ShowNothingFoundIndicator}");
            });

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Search operation completed successfully with {transferItems.Count} items");
            _logger.LogInformation("Transfer search completed. Found {Count} inventory items", transferItems.Count);
            ReportProgress($"Found {transferItems.Count} transfer candidates", "search", 100, true);
        }
        finally
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] ExecuteSearchAsync - Setting IsLoading = false");
            IsLoading = false;
        }
    }

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    [RelayCommand]
    private async Task ResetSearchAsync()
    {
        try
        {
            IsLoading = true;
            ReportProgress("Resetting transfer form...", "reset", 0);

            // Clear search criteria
            SelectedPart = null;
            SelectedOperation = null;
            SelectedToLocation = null;
            PartText = string.Empty;
            OperationText = string.Empty;
            ToLocationText = string.Empty;
            TransferQuantity = 1;
            InventoryItems.Clear();
            SelectedInventoryItem = null;
            MaxTransferQuantity = 0;

            ReportProgress("Reloading master data...", "reset", 50);

            // Reload all ComboBox data
            await LoadComboBoxDataAsync().ConfigureAwait(false);

            _logger.LogInformation("Transfer search criteria reset and data refreshed");
            ReportProgress("Transfer form reset complete", "reset", 100, true);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Executes transfer operations for selected inventory items with comprehensive validation
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanTransfer))]
    private async Task ExecuteTransferAsync()
    {
        // Handle both single and batch transfers
        var itemsToTransfer = SelectedInventoryItems.Count > 0 ? SelectedInventoryItems.ToList() :
            (SelectedInventoryItem != null ? new List<TransferInventoryItem> { SelectedInventoryItem } : new List<TransferInventoryItem>());

        if (!itemsToTransfer.Any() || string.IsNullOrWhiteSpace(SelectedToLocation))
        {
            _logger.LogWarning("Transfer operation attempted with invalid selection");
            ReportProgress("Invalid transfer configuration", "transfer", 0, false, true);
            return;
        }

        try
        {
            IsLoading = true;
            ReportProgress($"Initiating batch transfer of {itemsToTransfer.Count()} item(s)...", "transfer", 0);

            var totalItems = itemsToTransfer.Count();
            var processedItems = 0;
            var successfulTransfers = 0;
            var failedTransfers = 0;

            foreach (var item in itemsToTransfer)
            {
                var fromLocation = item.Location;
                var partId = item.PartId;
                var operation = item.Operation ?? string.Empty;

                ReportProgress($"Processing {partId} ({processedItems + 1} of {totalItems})...", "transfer",
                    (int)((double)processedItems / totalItems * 80)); // Use 80% for processing, 20% for finalization

                // Validate destination location is different from source
                if (fromLocation.Equals(SelectedToLocation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Transfer skipped - same location: {PartId} at {Location}", partId, fromLocation);
                    failedTransfers++;
                    processedItems++;
                    continue;
                }

                // For batch transfers, use the quantity from each item (complete transfer)
                // or the specified TransferQuantity for single item transfers
                var transferQuantity = (totalItems == 1) ? TransferQuantity : item.Quantity;

                // Validate transfer quantity
                if (transferQuantity > item.Quantity)
                {
                    _logger.LogWarning("Transfer quantity {TransferQuantity} exceeds available {Available} for {PartId}",
                        transferQuantity, item.Quantity, partId);
                    failedTransfers++;
                    processedItems++;
                    continue;
                }

                // Execute transfer
                bool transferResult;

                if (transferQuantity < item.Quantity && totalItems == 1)
                {
                    // Partial quantity transfer (only for single item)
                    _logger.LogInformation("Executing partial transfer: {TransferQuantity} of {TotalQuantity} units for {PartId}",
                        transferQuantity, item.Quantity, partId);

                    transferResult = await _databaseService.TransferQuantityAsync(
                        item.BatchNumber ?? string.Empty,
                        partId,
                        operation,
                        transferQuantity,
                        item.Quantity,
                        SelectedToLocation,
                        _applicationState.CurrentUser
                    );
                }
                else
                {
                    // Complete item transfer
                    _logger.LogInformation("Executing complete transfer: {TransferQuantity} units for {PartId}", transferQuantity, partId);

                    transferResult = await _databaseService.TransferPartAsync(
                        item.BatchNumber ?? string.Empty,
                        partId,
                        operation,
                        SelectedToLocation
                    );
                }

                if (transferResult)
                {
                    successfulTransfers++;

                    // Update UI - remove or update item
                    if (transferQuantity >= item.Quantity)
                    {
                        // Complete transfer - remove item from list
                        InventoryItems.Remove(item);
                    }
                    else
                    {
                        // Partial transfer - update quantity (single item only)
                        item.Quantity -= transferQuantity;
                        item.LastUpdated = DateTime.Now;
                    }

                    // Fire event for each successful transfer
                    ItemsTransferred?.Invoke(this, new ItemsTransferredEventArgs
                    {
                        PartId = partId,
                        Operation = operation,
                        FromLocation = fromLocation,
                        ToLocation = SelectedToLocation,
                        TransferredQuantity = transferQuantity,
                        TransferTime = DateTime.Now
                    });

                    _logger.LogInformation("Successfully transferred {Quantity} units of {PartId} from {FromLocation} to {ToLocation}",
                        transferQuantity, partId, fromLocation, SelectedToLocation);
                }
                else
                {
                    _logger.LogError("Transfer operation failed for {PartId}", partId);
                    failedTransfers++;
                }

                processedItems++;
            }

            // Final status report
            var finalMessage = totalItems == 1 ?
                $"Transfer complete: {TransferQuantity} units" :
                $"Batch transfer complete: {successfulTransfers} successful, {failedTransfers} failed";

            ReportProgress(finalMessage, "transfer", 100, true, failedTransfers > 0);

            // Show success overlay with batch transfer details
            if (successfulTransfers > 0)
            {
                var firstItem = itemsToTransfer.First();
                if (totalItems == 1)
                {
                    ShowTransferSuccessOverlay(firstItem.PartId, firstItem.Operation ?? string.Empty,
                        firstItem.Location, SelectedToLocation, TransferQuantity);
                }
                else
                {
                    ShowBatchTransferSuccessOverlay(successfulTransfers, failedTransfers, SelectedToLocation ?? "Unknown");
                }
            }

            // Reset transfer form for next operation
            SelectedInventoryItem = null;
            SelectedInventoryItems.Clear();
            TransferQuantity = 1;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Tests print functionality with mock data for development and testing purposes
    /// </summary>
    [RelayCommand]
    private async Task ExecutePrintAsync()
    {
        try
        {
            if (_printService == null || _navigationService == null)
            {
                _logger.LogWarning("Print service or navigation service not available");
                return;
            }

            IsLoading = true;
            _logger.LogInformation("Initiating test print operation with mock data");

            // Create mock inventory data for testing print functionality
            var mockDataTable = CreateMockInventoryDataTable();

            // Get or create PrintViewModel
            var printViewModel = Program.GetOptionalService<PrintViewModel>();
            if (printViewModel == null)
            {
                _logger.LogError("PrintViewModel not available from DI container");
                return;
            }

            // Configure print data with mock data
            printViewModel.PrintData = mockDataTable;
            printViewModel.DataSourceType = MTM_WIP_Application_Avalonia.Models.PrintDataSourceType.Transfer;
            printViewModel.DocumentTitle = "Test Transfer Report - Mock Data";
            printViewModel.OriginalViewContext = this; // Store current context for navigation back

            // Create and navigate to PrintView
            var printView = new Views.PrintView
            {
                DataContext = printViewModel
            };

            // Initialize print view with data
            await printViewModel.InitializeAsync();

            // Navigate to print view using NavigationService
            _navigationService.NavigateTo(printView);

            _logger.LogInformation("Navigated to print view with mock test data for {Count} items", mockDataTable.Rows.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating test print operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to open print interface", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Command Aliases for AXAML Bindings
    /// <summary>
    /// Alias command for ExecuteSearchCommand to match AXAML bindings
    /// </summary>
    public IRelayCommand SearchCommand => ExecuteSearchCommand;

    /// <summary>
    /// Alias command for ExecuteTransferCommand to match AXAML bindings
    /// </summary>
    public IRelayCommand TransferCommand => ExecuteTransferCommand;

    /// <summary>
    /// Alias command for ResetSearchCommand to match AXAML bindings
    /// </summary>
    public IRelayCommand ResetCommand => ResetSearchCommand;

    /// <summary>
    /// Alias command for ExecutePrintCommand to match AXAML bindings
    /// </summary>
    public IRelayCommand PrintCommand => ExecutePrintCommand;

    /// <summary>
    /// Toggles the visibility of the search panel
    /// </summary>
    [RelayCommand]
    private void TogglePanel()
    {
        try
        {
            PanelToggleRequested?.Invoke(this, EventArgs.Empty);
            _logger.LogDebug("Panel toggle requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling panel");
        }
    }

    /// <summary>
    /// Expands the transfer configuration panel
    /// </summary>
    [RelayCommand]
    private void ExpandPanel()
    {
        try
        {
            // This will be handled by the code-behind to expand the CollapsiblePanel
            PanelExpandRequested?.Invoke(this, EventArgs.Empty);
            _logger.LogDebug("Panel expand requested");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error expanding panel");
        }
    }
    /// <summary>
    /// Shows success overlay with transfer confirmation details
    /// </summary>
    private void ShowTransferSuccessOverlay(string partId, string operation, string fromLocation, string toLocation, int quantity)
    {
        try
        {
            if (_successOverlayService != null)
            {
                var message = "Transfer Complete";
                var details = $"Part: {partId}\nOperation: {operation}\nFrom: {fromLocation} → To: {toLocation}\nQuantity: {quantity:N0} units";

                // Use SuccessOverlayRequested event to trigger overlay from the View
                SuccessOverlayRequested?.Invoke(this, new SuccessOverlayEventArgs
                {
                    Message = message,
                    Details = details,
                    IconKind = "ArrowRightBold",
                    Duration = 3000
                });

                _logger.LogDebug("SuccessOverlay requested for transfer: {PartId} from {FromLocation} to {ToLocation}",
                    partId, fromLocation, toLocation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing transfer success overlay");
        }
    }

    /// <summary>
    /// Shows success overlay with batch transfer summary
    /// </summary>
    private void ShowBatchTransferSuccessOverlay(int successfulTransfers, int failedTransfers, string toLocation)
    {
        try
        {
            if (_successOverlayService != null)
            {
                var message = "Batch Transfer Complete";
                var details = $"Successful: {successfulTransfers}\nFailed: {failedTransfers}\nDestination: {toLocation}";

                // Use different icon for batch transfers
                var iconKind = failedTransfers > 0 ? "AlertCircle" : "CheckAll";

                SuccessOverlayRequested?.Invoke(this, new SuccessOverlayEventArgs
                {
                    Message = message,
                    Details = details,
                    IconKind = iconKind,
                    Duration = 4000 // Longer duration for batch operations
                });

                _logger.LogDebug("Batch SuccessOverlay requested: {Successful} successful, {Failed} failed transfers to {ToLocation}",
                    successfulTransfers, failedTransfers, toLocation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing batch transfer success overlay");
        }
    }

    /// <summary>
    /// Reports progress to MainView status bar
    /// </summary>
    private void ReportProgress(string message, string operation, int? progressPercentage = null, bool isComplete = false, bool isError = false)
    {
        try
        {
            StatusMessage = message;

            ProgressReported?.Invoke(this, new ProgressReportEventArgs
            {
                Message = message,
                Operation = operation,
                ProgressPercentage = progressPercentage,
                IsComplete = isComplete,
                IsError = isError
            });

            _logger.LogDebug("Progress reported: {Message} for {Operation}", message, operation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting progress");
        }
    }

    #endregion

    #region Data Loading and Helper Methods

    /// <summary>
    /// Loads ComboBox data from database including parts, operations, and locations
    /// </summary>
    private async Task LoadComboBoxDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading transfer ComboBox data from database");

            // Load Parts using md_part_ids_Get_All stored procedure
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (partResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    PartOptions.Clear();
                    foreach (System.Data.DataRow row in partResult.Data.Rows)
                    {
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartOptions.Add(partId);
                        }
                    }
                });
                _logger.LogInformation("Loaded {Count} parts for transfer", PartOptions.Count);
            }

            // Load Operations using md_operation_numbers_Get_All stored procedure
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            if (operationResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    OperationOptions.Clear();
                    foreach (System.Data.DataRow row in operationResult.Data.Rows)
                    {
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            OperationOptions.Add(operation);
                        }
                    }
                });
                _logger.LogInformation("Loaded {Count} operations for transfer", OperationOptions.Count);
            }

            // Load Locations using md_locations_Get_All stored procedure
            var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            if (locationResult.IsSuccess)
            {
                // Update collection on UI thread
                Dispatcher.UIThread.Post(() =>
                {
                    LocationOptions.Clear();
                    foreach (System.Data.DataRow row in locationResult.Data.Rows)
                    {
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            LocationOptions.Add(location);
                        }
                    }
                });
                _logger.LogInformation("Loaded {Count} locations for transfer", LocationOptions.Count);
            }

            _logger.LogInformation("Transfer ComboBox data loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Transfer ComboBox data");
            throw;
        }
    }

    /// <summary>
    /// Loads sample data for demonstration purposes
    /// </summary>
    private Task LoadSampleDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            // Clear existing data
            PartOptions.Clear();
            OperationOptions.Clear();
            LocationOptions.Clear();

            // Sample parts
            var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005" };
            foreach (var part in sampleParts)
            {
                PartOptions.Add(part);
            }

            // Sample operations (MTM uses string numbers)
            var sampleOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in sampleOperations)
            {
                OperationOptions.Add(operation);
            }

            // Sample locations for transfer destinations
            var sampleLocations = new[] { "WC01", "WC02", "WC03", "SHIP", "QC", "STORE" };
            foreach (var location in sampleLocations)
            {
                LocationOptions.Add(location);
            }
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads sample inventory data for demonstration with proper filtering
    /// </summary>
    private Task LoadSampleInventoryDataAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var sampleItems = new[]
            {
                new InventoryItem
                {
                    ID = 1,
                    PartID = "PART001",
                    Operation = "100",
                    Location = "WC01",
                    Quantity = 25,
                    Notes = "Ready for transfer",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-2),
                    BatchNumber = "B001",
                    ItemType = "WIP"
                },
                new InventoryItem
                {
                    ID = 2,
                    PartID = "PART001",
                    Operation = "110",
                    Location = "WC02",
                    Quantity = 15,
                    Notes = "Quality check complete",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddHours(-1),
                    BatchNumber = "B002",
                    ItemType = "WIP"
                },
                new InventoryItem
                {
                    ID = 3,
                    PartID = "PART002",
                    Operation = "90",
                    Location = "WC01",
                    Quantity = 40,
                    Notes = "Incoming material",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddMinutes(-30),
                    BatchNumber = "B003",
                    ItemType = "WIP"
                },
                new InventoryItem
                {
                    ID = 4,
                    PartID = "PART003",
                    Operation = "120",
                    Location = "WC03",
                    Quantity = 8,
                    Notes = "Final operation",
                    User = "TestUser",
                    LastUpdated = DateTime.Now.AddMinutes(-15),
                    BatchNumber = "B004",
                    ItemType = "WIP"
                }
            };

            // Filter sample data based on search criteria
            var filteredItems = sampleItems.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                filteredItems = filteredItems.Where(item =>
                    item.PartID.Equals(SelectedPart, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SelectedOperation))
            {
                filteredItems = filteredItems.Where(item =>
                    item.Operation?.Equals(SelectedOperation, StringComparison.OrdinalIgnoreCase) == true);
            }

            foreach (var item in filteredItems)
            {
                InventoryItems.Add(ConvertToTransferItem(item));
            }
        });
        return Task.CompletedTask;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Converts an InventoryItem to a TransferInventoryItem
    /// </summary>
    private static TransferInventoryItem ConvertToTransferItem(InventoryItem item)
    {
        return new TransferInventoryItem
        {
            Id = item.ID,
            PartId = item.PartID,
            Location = item.Location,
            Operation = item.Operation,
            Quantity = item.Quantity,
            ItemType = item.ItemType,
            ReceiveDate = item.ReceiveDate,
            LastUpdated = item.LastUpdated,
            User = item.User,
            BatchNumber = item.BatchNumber,
            Notes = item.Notes,

            // Set Transfer-specific properties
            FromLocation = item.Location, // Source location from inventory
            ToLocation = string.Empty, // Will be set when user selects destination
            AvailableQuantity = item.Quantity, // Available quantity for transfer
            TransferQuantity = 1 // Default transfer quantity
        };
    }

    /// <summary>
    /// Updates maximum transfer quantity based on selected inventory item
    /// </summary>
    private void UpdateMaxTransferQuantity()
    {
        if (SelectedInventoryItem != null)
        {
            MaxTransferQuantity = SelectedInventoryItem.Quantity;

            // Ensure current transfer quantity is within bounds
            if (TransferQuantity > MaxTransferQuantity)
            {
                TransferQuantity = Math.Max(1, MaxTransferQuantity);
            }
        }
        else if (SelectedInventoryItems.Count == 1)
        {
            MaxTransferQuantity = SelectedInventoryItems[0].Quantity;

            // Ensure current transfer quantity is within bounds
            if (TransferQuantity > MaxTransferQuantity)
            {
                TransferQuantity = Math.Max(1, MaxTransferQuantity);
            }
        }
        else
        {
            MaxTransferQuantity = 0;
            TransferQuantity = 1;
        }
    }

    /// <summary>
    /// Validates that the transfer destination is different from source location(s)
    /// </summary>
    private bool ValidateTransferDestination()
    {
        if (string.IsNullOrWhiteSpace(SelectedToLocation))
            return false;

        // Check single item selection
        if (SelectedInventoryItem != null)
        {
            return !SelectedInventoryItem.Location.Equals(SelectedToLocation, StringComparison.OrdinalIgnoreCase);
        }

        // Check multi-item selection - all items must have different location than destination
        if (SelectedInventoryItems.Count > 0)
        {
            return SelectedInventoryItems.All(item =>
                !item.Location.Equals(SelectedToLocation, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    /// <summary>
    /// Updates the ToLocation property for all inventory items when user selects a destination
    /// </summary>
    private void UpdateInventoryItemsToLocation()
    {
        if (!InventoryItems.Any()) return;

        var selectedLocation = SelectedToLocation ?? string.Empty;

        foreach (var item in InventoryItems)
        {
            item.ToLocation = selectedLocation;
        }

        _logger.LogDebug("Updated ToLocation to '{ToLocation}' for {Count} inventory items",
            selectedLocation, InventoryItems.Count);
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions from command operations with comprehensive logging and user-friendly error presentation
    /// </summary>
    private void HandleException(Exception ex)
    {
        _logger.LogError(ex, "Error in TransferItemViewModel operation");

        // Present user-friendly error message via centralized error service
        _ = Services.ErrorHandling.HandleErrorAsync(ex, "Transfer Operation", _applicationState.CurrentUser);

        // Update UI state to reflect error
        // Note: StatusMessage property may need to be added to this ViewModel for UI feedback
        Logger.LogInformation("User-friendly error message: {Message}", GetUserFriendlyErrorMessage(ex));
    }

    /// <summary>
    /// Gets a user-friendly error message based on the exception type
    /// </summary>
    private string GetUserFriendlyErrorMessage(Exception ex) => ex switch
    {
        InvalidOperationException => "The transfer operation could not be completed. Please verify the part details and try again.",
        TimeoutException => "The transfer operation timed out. Please check your connection and try again.",
        UnauthorizedAccessException => "You do not have permission to perform this transfer operation.",
        ArgumentException => "Invalid transfer details provided. Please check your input and try again.",
        _ => "An unexpected error occurred during the transfer operation. Please contact support if this continues."
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Programmatically triggers a search operation
    /// </summary>
    public void TriggerSearch()
    {
        if (ExecuteSearchCommand.CanExecute(null))
        {
            ExecuteSearchCommand.Execute(null);
        }
    }

    /// <summary>
    /// Sets transfer configuration from external source (e.g., QuickButtons)
    /// </summary>
    public void SetTransferConfiguration(string partId, string operation, string toLocation, int quantity)
    {
        SelectedPart = partId;
        PartText = partId;
        SelectedOperation = operation;
        OperationText = operation;
        SelectedToLocation = toLocation;
        ToLocationText = toLocation;
        TransferQuantity = quantity;

        // Trigger search to populate inventory grid
        TriggerSearch();

        _logger.LogInformation("Transfer configuration set via external source: Part={PartId}, Operation={Operation}, ToLocation={ToLocation}, Quantity={Quantity}",
            partId, operation, toLocation, quantity);
    }

    /// <summary>
    /// Sets transfer configuration from QuickButtonData
    /// </summary>
    public void SetTransferConfiguration(QuickButtonData quickButton)
    {
        if (quickButton == null)
        {
            _logger.LogWarning("Cannot set transfer configuration: QuickButtonData is null");
            return;
        }

        // For transfer operations, we need to interpret QuickButton data:
        // - PartId and Operation are source selection criteria
        // - We don't set destination since that's what user needs to configure
        // - Quantity becomes default transfer quantity
        SelectedPart = quickButton.PartId;
        PartText = quickButton.PartId;
        SelectedOperation = quickButton.Operation;
        OperationText = quickButton.Operation;

        // Don't pre-set destination location for transfers - user must select
        // SelectedToLocation = null;
        // ToLocationText = string.Empty;

        TransferQuantity = Math.Max(1, quickButton.Quantity);
        TransferQuantityText = TransferQuantity.ToString();

        // Trigger search to populate inventory grid with source criteria
        TriggerSearch();

        _logger.LogInformation("Transfer configuration set from QuickButton: Part={PartId}, Operation={Operation}, Quantity={Quantity}",
            quickButton.PartId, quickButton.Operation, quickButton.Quantity);
    }

    /// <summary>
    /// Validates current transfer configuration
    /// </summary>
    public bool ValidateTransferOperation()
    {
        // Validate destination location is selected
        if (string.IsNullOrWhiteSpace(SelectedToLocation))
            return false;

        // Validate transfer quantity
        if (TransferQuantity <= 0)
            return false;

        // Validate selection exists
        if (SelectedInventoryItem == null)
            return false;

        // Validate sufficient quantity available
        if (TransferQuantity > SelectedInventoryItem.Quantity)
            return false;

        return true;
    }

    /// <summary>
    /// Generates a comprehensive transfer report.
    /// </summary>
    private string GenerateTransferReport()
    {
        var report = new StringBuilder();

        // Report header
        report.AppendLine("MTM WIP Application - Transfer Report");
        report.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.AppendLine($"Generated by: {Environment.UserName}");
        report.AppendLine(new string('=', 50));
        report.AppendLine();

        // Search criteria
        report.AppendLine("SEARCH CRITERIA");
        report.AppendLine(new string('-', 15));
        report.AppendLine($"Part ID: {SelectedPart ?? "All"}");
        report.AppendLine($"Operation: {SelectedOperation ?? "All"}");
        report.AppendLine($"Location Filter: {PartText ?? "All"}"); // Using PartText instead of SelectedLocation
        report.AppendLine();

        // Transfer configuration
        if (!string.IsNullOrEmpty(SelectedToLocation))
        {
            report.AppendLine("TRANSFER CONFIGURATION");
            report.AppendLine(new string('-', 22));
            report.AppendLine($"Destination Location: {SelectedToLocation}");
            report.AppendLine($"Transfer Quantity: {TransferQuantity}");

            if (SelectedInventoryItem != null)
            {
                report.AppendLine($"Selected Item: {SelectedInventoryItem.PartId} - {SelectedInventoryItem.Operation}");
                report.AppendLine($"Available Quantity: {SelectedInventoryItem.Quantity}");
            }

            report.AppendLine();
        }

        // Inventory data
        report.AppendLine("INVENTORY ITEMS");
        report.AppendLine(new string('-', 15));
        report.AppendLine("Part ID\t\t\tOperation\tLocation\tQuantity\tItem Type\tLast Updated");
        report.AppendLine(new string('-', 80));

        foreach (var item in InventoryItems.OrderBy(i => i.PartId).ThenBy(i => i.Operation))
        {
            report.AppendLine($"{item.PartId}\t\t{item.Operation}\t\t{item.Location}\t\t{item.Quantity}\t\t{item.ItemType}\t\t{item.LastUpdated:yyyy-MM-dd}");
        }

        // Summary statistics
        report.AppendLine();
        report.AppendLine("SUMMARY STATISTICS");
        report.AppendLine(new string('-', 18));
        report.AppendLine($"Total Items: {InventoryItems.Count}");
        report.AppendLine($"Total Quantity: {InventoryItems.Sum(i => i.Quantity):N0}");
        report.AppendLine($"Unique Parts: {InventoryItems.Select(i => i.PartId).Distinct().Count()}");
        report.AppendLine($"Unique Locations: {InventoryItems.Select(i => i.Location).Distinct().Count()}");

        report.AppendLine();
        report.AppendLine("End of Transfer Report");

        return report.ToString();
    }

    /// <summary>
    /// Converts inventory items to DataTable for print service
    /// </summary>
    private DataTable ConvertInventoryToDataTable(ObservableCollection<InventoryItem> inventoryItems)
    {
        var dataTable = new DataTable();

        // Define columns based on InventoryItem properties
        dataTable.Columns.Add("PartId", typeof(string));
        dataTable.Columns.Add("Operation", typeof(string));
        dataTable.Columns.Add("Location", typeof(string));
        dataTable.Columns.Add("Quantity", typeof(int));
        dataTable.Columns.Add("Notes", typeof(string));
        dataTable.Columns.Add("LastUpdated", typeof(DateTime));
        dataTable.Columns.Add("LastUpdatedBy", typeof(string));

        // Add rows
        foreach (var item in inventoryItems)
        {
            var row = dataTable.NewRow();
            row["PartId"] = item.PartID ?? string.Empty;
            row["Operation"] = item.Operation ?? string.Empty;
            row["Location"] = item.Location ?? string.Empty;
            row["Quantity"] = item.Quantity;
            row["Notes"] = item.Notes ?? string.Empty;
            row["LastUpdated"] = item.LastUpdated;
            row["LastUpdatedBy"] = item.User ?? string.Empty;
            dataTable.Rows.Add(row);
        }

        _logger.LogDebug("Converted {ItemCount} inventory items to DataTable with {ColumnCount} columns",
            inventoryItems.Count, dataTable.Columns.Count);

        return dataTable;
    }

    /// <summary>
    /// Creates mock inventory data for testing print functionality
    /// </summary>
    private DataTable CreateMockInventoryDataTable()
    {
        var dataTable = new DataTable();

        // Define columns based on InventoryItem properties
        dataTable.Columns.Add("PartId", typeof(string));
        dataTable.Columns.Add("Operation", typeof(string));
        dataTable.Columns.Add("Location", typeof(string));
        dataTable.Columns.Add("Quantity", typeof(int));
        dataTable.Columns.Add("Notes", typeof(string));
        dataTable.Columns.Add("LastUpdated", typeof(DateTime));
        dataTable.Columns.Add("LastUpdatedBy", typeof(string));

        // Add mock data rows for testing
        var mockData = new[]
        {
            new { PartId = "TEST-PART-001", Operation = "90", Location = "WC01", Quantity = 25, Notes = "Mock data for testing print functionality" },
            new { PartId = "TEST-PART-002", Operation = "100", Location = "WC02", Quantity = 50, Notes = "Sample inventory item for print preview" },
            new { PartId = "TEST-PART-003", Operation = "110", Location = "WC03", Quantity = 15, Notes = "Test data - transfer candidate" },
            new { PartId = "TEST-PART-004", Operation = "120", Location = "WC01", Quantity = 75, Notes = "Mock inventory for print system validation" },
            new { PartId = "TEST-PART-005", Operation = "90", Location = "QC01", Quantity = 30, Notes = "Quality control staging area" },
            new { PartId = "TEST-PART-006", Operation = "100", Location = "SHIP", Quantity = 100, Notes = "Ready for shipment - test data" },
            new { PartId = "TEST-PART-007", Operation = "110", Location = "WC04", Quantity = 20, Notes = "Manufacturing work center 4" },
            new { PartId = "TEST-PART-008", Operation = "120", Location = "FG01", Quantity = 60, Notes = "Finished goods inventory mock data" }
        };

        var currentUser = Environment.UserName;
        var baseTime = DateTime.Now.AddDays(-1); // Yesterday as base time

        for (int i = 0; i < mockData.Length; i++)
        {
            var item = mockData[i];
            var row = dataTable.NewRow();
            row["PartId"] = item.PartId;
            row["Operation"] = item.Operation;
            row["Location"] = item.Location;
            row["Quantity"] = item.Quantity;
            row["Notes"] = item.Notes;
            row["LastUpdated"] = baseTime.AddHours(i); // Stagger the update times
            row["LastUpdatedBy"] = $"{currentUser}_TEST";
            dataTable.Rows.Add(row);
        }

        _logger.LogDebug("Created mock DataTable with {RowCount} test inventory items for print testing", dataTable.Rows.Count);

        return dataTable;
    }

    #endregion

    #region Enhanced Validation Methods

    /// <summary>
    /// Enhanced MTM manufacturing validation for transfer operations
    /// Validates operation sequences, location routing, and business rules
    /// </summary>
    public bool ValidateManufacturingTransferOperation()
    {
        try
        {
            var validationErrors = new List<string>();

            // Validate manufacturing operations sequence
            if (!string.IsNullOrEmpty(SelectedOperation))
            {
                var operationNumber = int.TryParse(SelectedOperation, out int opNum) ? opNum : 0;
                var validOperations = new[] { 90, 100, 110, 120, 130 }; // MTM standard operations

                if (operationNumber > 0 && !validOperations.Contains(operationNumber))
                {
                    validationErrors.Add($"Operation {SelectedOperation} is not a valid MTM manufacturing operation (90, 100, 110, 120, 130)");
                }
            }

            // Validate location routing logic
            if (!string.IsNullOrEmpty(SelectedToLocation))
            {
                var validLocationPatterns = new[] { "WC", "FLOOR", "QC", "SHIP", "RECEIVING" };
                var locationValid = validLocationPatterns.Any(pattern =>
                    SelectedToLocation.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));

                if (!locationValid)
                {
                    validationErrors.Add($"Destination location '{SelectedToLocation}' does not follow MTM location naming standards");
                }

                // Validate not transferring to same location
                if (SelectedInventoryItem != null &&
                    string.Equals(SelectedInventoryItem.Location, SelectedToLocation, StringComparison.OrdinalIgnoreCase))
                {
                    validationErrors.Add("Cannot transfer to the same location");
                }
            }

            // Validate transfer quantity business rules
            if (TransferQuantity <= 0)
            {
                validationErrors.Add("Transfer quantity must be greater than zero");
            }

            if (SelectedInventoryItem != null && TransferQuantity > SelectedInventoryItem.Quantity)
            {
                validationErrors.Add($"Transfer quantity ({TransferQuantity}) exceeds available quantity ({SelectedInventoryItem.Quantity})");
            }

            // Log validation results
            if (validationErrors.Any())
            {
                _logger.LogWarning("MTM Transfer validation failed: {Errors}", string.Join("; ", validationErrors));
                StatusMessage = $"Validation errors: {validationErrors.Count} issues found";
                return false;
            }

            _logger.LogDebug("MTM Transfer validation passed for {PartId} to {Location}",
                SelectedInventoryItem?.PartId, SelectedToLocation);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MTM transfer validation");
            return false;
        }
    }

    #endregion
}

#region Event Args

/// <summary>
/// Event arguments for items transferred event
/// </summary>
public class ItemsTransferredEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public int TransferredQuantity { get; set; }
    public DateTime TransferTime { get; set; }
}

/// <summary>
/// Event arguments for SuccessOverlay requests
/// </summary>
public class SuccessOverlayEventArgs : EventArgs
{
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string IconKind { get; set; } = "CheckCircle";
    public int Duration { get; set; } = 3000;
}

/// <summary>
/// Event arguments for progress reporting to MainView status bar
/// </summary>
public class ProgressReportEventArgs : EventArgs
{
    public string Message { get; set; } = string.Empty;
    public int? ProgressPercentage { get; set; }
    public bool IsComplete { get; set; }
    public bool IsError { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

#endregion
