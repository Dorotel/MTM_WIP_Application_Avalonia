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
    private readonly ILogger<TransferItemViewModel> _logger;
    private readonly ISuccessOverlayService? _successOverlayService;
    private readonly IPrintService? _printService;
    private readonly INavigationService? _navigationService;

    #region Observable Collections
    
    /// <summary>
    /// Available part options for filtering
    /// </summary>
    public ObservableCollection<string> PartOptions { get; } = new();
    
    /// <summary>
    /// Available operation options for refined filtering
    /// </summary>
    public ObservableCollection<string> OperationOptions { get; } = new();
    
    /// <summary>
    /// Available locations for transfer destinations
    /// </summary>
    public ObservableCollection<string> LocationOptions { get; } = new();
    
    /// <summary>
    /// Current inventory items displayed in the DataGrid
    /// </summary>
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();

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
    [Range(0, int.MaxValue, ErrorMessage = "Maximum transfer quantity must be 0 or greater")]
    private int _maxTransferQuantity = 0;

    #endregion

    #region Selection and State Properties

    [ObservableProperty]
    private InventoryItem? _selectedInventoryItem;

    /// <summary>
    /// Status message for MainView status bar integration
    /// </summary>
    [ObservableProperty]
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Collection of selected inventory items for batch transfer operations
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<InventoryItem> _selectedInventoryItems = new();

    [ObservableProperty]
    private bool _isLoading;

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
        ILogger<TransferItemViewModel> logger,
        ISuccessOverlayService? successOverlayService = null,
        IPrintService? printService = null,
        INavigationService? navigationService = null) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
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
        switch (e.PropertyName)
        {
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));
                break;
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));
                break;
            case nameof(TransferQuantity):
            case nameof(SelectedInventoryItem):
            case nameof(SelectedInventoryItems):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasQuantityValidationError));
                OnPropertyChanged(nameof(HasLocationValidationError));
                UpdateMaxTransferQuantity();
                break;
            case nameof(SelectedToLocation):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasLocationValidationError));
                UpdateMaxTransferQuantity();
                ToLocationText = SelectedToLocation ?? string.Empty;
                break;
            case nameof(ToLocationText):
                OnPropertyChanged(nameof(HasLocationValidationError));
                if (!string.IsNullOrEmpty(ToLocationText) && LocationOptions.Contains(ToLocationText))
                    SelectedToLocation = ToLocationText;
                break;
            case nameof(MaxTransferQuantity):
                OnPropertyChanged(nameof(CanTransfer));
                OnPropertyChanged(nameof(HasQuantityValidationError));
                UpdateMaxTransferQuantity();
                // Ensure transfer quantity doesn't exceed maximum
                if (TransferQuantity > MaxTransferQuantity)
                {
                    TransferQuantity = Math.Max(1, MaxTransferQuantity);
                }
                break;
            case nameof(SelectedPart):
                PartText = SelectedPart ?? string.Empty;
                break;
            case nameof(SelectedOperation):
                OperationText = SelectedOperation ?? string.Empty;
                break;
            case nameof(PartText):
                if (!string.IsNullOrEmpty(PartText) && PartOptions.Contains(PartText))
                    SelectedPart = PartText;
                break;
            case nameof(OperationText):
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
            IsLoading = true;
            ReportProgress("Searching inventory...", "search", 0);
            
            InventoryItems.Clear();
            SelectedInventoryItem = null;
            SelectedInventoryItems.Clear();
            _hasSearchBeenExecuted = true;

            _logger.LogInformation("Executing transfer search for Part: {PartId}, Operation: {Operation}", 
                SelectedPart, SelectedOperation);

            ReportProgress("Querying database...", "search", 25);

            // Dynamic search based on selection criteria
            System.Data.DataTable result;
            
            if (!string.IsNullOrWhiteSpace(SelectedPart) && !string.IsNullOrWhiteSpace(SelectedOperation))
            {
                // Search by both part and operation
                result = await _databaseService.GetInventoryByPartAndOperationAsync(SelectedPart, SelectedOperation).ConfigureAwait(false);
            }
            else if (!string.IsNullOrWhiteSpace(SelectedPart))
            {
                // Search by part only
                result = await _databaseService.GetInventoryByPartIdAsync(SelectedPart).ConfigureAwait(false);
            }
            else
            {
                // No search criteria specified, don't load anything
                _logger.LogWarning("No search criteria specified for transfer search");
                ReportProgress("No search criteria specified", "search", 0, false, true);
                return;
            }

            ReportProgress("Processing results...", "search", 75);

            // Convert DataTable to InventoryItem objects
            foreach (System.Data.DataRow row in result.Rows)
            {
                var inventoryItem = new InventoryItem
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
                
                InventoryItems.Add(inventoryItem);
            }

            _logger.LogInformation("Transfer search completed. Found {Count} inventory items", InventoryItems.Count);
            ReportProgress($"Found {InventoryItems.Count} transfer candidates", "search", 100, true);
        }
        finally
        {
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
            (SelectedInventoryItem != null ? new List<InventoryItem> { SelectedInventoryItem } : new List<InventoryItem>());

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
                var partId = item.PartID;
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
                    ShowTransferSuccessOverlay(firstItem.PartID, firstItem.Operation ?? string.Empty, 
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
    /// Prints current inventory view with transfer details using Print Service
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
            _logger.LogInformation("Initiating print operation for {Count} transfer items", InventoryItems.Count);

            // Convert inventory items to DataTable for printing
            var dataTable = ConvertInventoryToDataTable(InventoryItems);

            // Get or create PrintViewModel
            var printViewModel = Program.GetOptionalService<PrintViewModel>();
            if (printViewModel == null)
            {
                _logger.LogError("PrintViewModel not available from DI container");
                return;
            }

            // Configure print data
            printViewModel.PrintData = dataTable;
            printViewModel.DataSourceType = MTM_WIP_Application_Avalonia.Models.PrintDataSourceType.Transfer;
            printViewModel.DocumentTitle = "Inventory Transfer Report";
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

            _logger.LogInformation("Navigated to print view with {Count} transfer items", InventoryItems.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating print operation");
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
                InventoryItems.Add(item);
            }
        });
        return Task.CompletedTask;
    }

    #endregion

    #region Helper Methods

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
                report.AppendLine($"Selected Item: {SelectedInventoryItem.PartID} - {SelectedInventoryItem.Operation}");
                report.AppendLine($"Available Quantity: {SelectedInventoryItem.Quantity}");
            }
            
            report.AppendLine();
        }
        
        // Inventory data
        report.AppendLine("INVENTORY ITEMS");
        report.AppendLine(new string('-', 15));
        report.AppendLine("Part ID\t\t\tOperation\tLocation\tQuantity\tItem Type\tLast Updated");
        report.AppendLine(new string('-', 80));
        
        foreach (var item in InventoryItems.OrderBy(i => i.PartID).ThenBy(i => i.Operation))
        {
            report.AppendLine($"{item.PartID}\t\t{item.Operation}\t\t{item.Location}\t\t{item.Quantity}\t\t{item.ItemType}\t\t{item.LastUpdated:yyyy-MM-dd}");
        }
        
        // Summary statistics
        report.AppendLine();
        report.AppendLine("SUMMARY STATISTICS");
        report.AppendLine(new string('-', 18));
        report.AppendLine($"Total Items: {InventoryItems.Count}");
        report.AppendLine($"Total Quantity: {InventoryItems.Sum(i => i.Quantity):N0}");
        report.AppendLine($"Unique Parts: {InventoryItems.Select(i => i.PartID).Distinct().Count()}");
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
