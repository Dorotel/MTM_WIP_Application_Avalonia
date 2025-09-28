using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;
using MTM_WIP_Application_Avalonia.Models; // For ItemsTransferredEventArgs
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_Shared_Logic;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// Represents a column configuration item for dynamic DataGrid column management
/// </summary>
public class ColumnItem
{
    /// <summary>
    /// Internal column name from database (ID, PartID, Location, etc.)
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// User-friendly display name shown in the UI
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Whether this column is currently visible in the DataGrid
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Column width (optional)
    /// </summary>
    public double Width { get; set; } = double.NaN;

    /// <summary>
    /// Column display order
    /// </summary>
    public int Order { get; set; }
}

/// <summary>
/// Represents the result of a transfer operation for detailed tracking and reporting
/// </summary>
internal class TransferResult
{
    public string PartId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public int TransferredQuantity { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
}

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
            // Apply auto-capping logic
            int cappedQuantity = ApplyQuantityAutoCapping(quantity);
            TransferQuantity = cappedQuantity;

            // Update text if quantity was auto-capped
            if (cappedQuantity != quantity)
            {
                TransferQuantityText = cappedQuantity.ToString();

                // Show warning about auto-capping
                _logger?.LogInformation("Transfer quantity auto-capped from {Original} to {Capped} for part {PartId}",
                    quantity, cappedQuantity, SelectedInventoryItem?.PartId ?? "UNKNOWN");
            }
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

    /// <summary>
    /// Called when SelectedInventoryItem changes - updates MaxTransferQuantity and applies auto-capping
    /// </summary>
    partial void OnSelectedInventoryItemChanged(TransferInventoryItem? value)
    {
        try
        {
            if (value != null)
            {
                // Update maximum transfer quantity based on selected item
                MaxTransferQuantity = Math.Max(0, value.AvailableQuantity);

                // Apply auto-capping to current transfer quantity
                if (TransferQuantity > MaxTransferQuantity)
                {
                    int originalQuantity = TransferQuantity;
                    TransferQuantity = MaxTransferQuantity;
                    TransferQuantityText = MaxTransferQuantity.ToString();

                    _logger?.LogInformation("Transfer quantity auto-capped from {Original} to {Max} for selected item {PartId}",
                        originalQuantity, MaxTransferQuantity, value.PartId);
                }

                _logger?.LogDebug("Selected inventory item changed: {PartId}, Available: {Available}, MaxTransfer: {MaxTransfer}",
                    value.PartId, value.AvailableQuantity, MaxTransferQuantity);
            }
            else
            {
                MaxTransferQuantity = 0;
                TransferQuantity = 1;
                TransferQuantityText = "1";
                _logger?.LogDebug("Selected inventory item cleared - reset transfer quantity");
            }

            // Notify property changes
            OnPropertyChanged(nameof(HasQuantityValidationError));
            OnPropertyChanged(nameof(IsTransferQuantityValid));
            OnPropertyChanged(nameof(CanTransfer));
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling selected inventory item change");
        }
    }

    /// <summary>
    /// Apply quantity auto-capping logic based on selected inventory item
    /// </summary>
    /// <param name="requestedQuantity">Requested transfer quantity</param>
    /// <returns>Auto-capped quantity (never exceeds available quantity)</returns>
    private int ApplyQuantityAutoCapping(int requestedQuantity)
    {
        try
        {
            if (SelectedInventoryItem == null)
            {
                return Math.Max(1, requestedQuantity); // Default to at least 1
            }

            int availableQuantity = Math.Max(0, SelectedInventoryItem.AvailableQuantity);

            // Auto-cap to available quantity
            if (requestedQuantity > availableQuantity)
            {
                return availableQuantity;
            }

            // Ensure minimum of 1 for valid transfers
            return Math.Max(1, requestedQuantity);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying quantity auto-capping");
            return 1; // Safe fallback
        }
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

    #region EditInventoryView Integration Properties

    /// <summary>
    /// Controls visibility of EditInventoryView panel
    /// </summary>
    [ObservableProperty]
    private bool _showEditInventoryView;

    /// <summary>
    /// EditInventoryViewModel for item editing
    /// </summary>
    [ObservableProperty]
    private MTM_WIP_Application_Avalonia.ViewModels.Overlay.EditInventoryViewModel? _editInventoryViewModel;

    /// <summary>
    /// Controls visibility of column customization panel
    /// </summary>
    [ObservableProperty]
    private bool _showColumnCustomization;

    /// <summary>
    /// Available columns for customization dropdown (based on search results)
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ColumnItem> _availableColumns = new();

    /// <summary>
    /// Selected column action from dropdown
    /// </summary>
    [ObservableProperty]
    private ColumnItem? _selectedColumnAction;

    /// <summary>
    /// Dictionary to track column visibility (populated dynamically from search results)
    /// </summary>
    private readonly Dictionary<string, bool> _columnVisibility = new();

    /// <summary>
    /// Static mapping of database column names to user-friendly display names
    /// </summary>
    private static readonly Dictionary<string, string> ColumnDisplayNames = new()
    {
        { "ID", "Item ID" },
        { "PartID", "Part Number" },
        { "Location", "Current Location" },
        { "Operation", "Operation" },
        { "Quantity", "Quantity" },
        { "ItemType", "Item Type" },
        { "ReceiveDate", "Inventory Date" },
        { "LastUpdated", "Last Updated" },
        { "User", "Updated By" },
        { "BatchNumber", "Batch Number" },
        { "Notes", "Notes" }
    };

    /// <summary>
    /// Static mapping of database column names to suggested column widths
    /// </summary>
    private static readonly Dictionary<string, double> ColumnWidths = new()
    {
        { "ID", 80 },
        { "PartID", 120 },
        { "Location", 100 },
        { "Operation", 80 },
        { "Quantity", 80 },
        { "ItemType", 80 },
        { "ReceiveDate", 100 },
        { "LastUpdated", 130 },
        { "User", 100 },
        { "BatchNumber", 100 },
        { "Notes", 200 }
    };

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
    /// Indicates if there's a quantity validation error (exceeds available or invalid)
    /// </summary>
    public bool HasQuantityValidationError =>
        SelectedInventoryItem != null &&
        (TransferQuantity > MaxTransferQuantity || TransferQuantity <= 0);

    /// <summary>
    /// Validation state for transfer quantity - must be positive and within available limits
    /// </summary>
    public bool IsTransferQuantityValid =>
        SelectedInventoryItem != null &&
        TransferQuantity > 0 &&
        TransferQuantity <= MaxTransferQuantity;

    /// <summary>
    /// Enhanced validation for transfer operation - checks all required conditions
    /// Row Selected + Location entered + Quantity <= available quantity
    /// </summary>
    public bool CanTransfer =>
        // Must have at least one item selected
        (SelectedInventoryItem != null || SelectedInventoryItems.Count > 0) &&
        // Must have destination location entered
        !string.IsNullOrWhiteSpace(SelectedToLocation) &&
        // Location must be different from source location(s)
        ValidateTransferDestination() &&
        // For single item transfers, quantity must be valid
        (SelectedInventoryItem == null || (TransferQuantity > 0 && TransferQuantity <= MaxTransferQuantity)) &&
        // Must not be currently loading
        !IsLoading;

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
    /// Event fired when panel collapse is requested (after successful search)
    /// </summary>
    public event EventHandler? PanelCollapseRequested;

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
        _ = LoadColumnPreferencesAsync(); // Load user column preferences
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

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Database query returned {result?.Rows?.Count ?? 0} rows");
            _logger.LogInformation("[TRANSFER-DEBUG] Database query returned {RowCount} rows", result?.Rows?.Count ?? 0);
            ReportProgress("Processing results...", "search", 75);

            // Check if result is valid before processing
            if (result?.Rows == null)
            {
                _logger.LogWarning("[TRANSFER-DEBUG] Database query returned null result");
                ReportProgress("No results found", "search", 100, true);
                return;
            }

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

                // Populate available columns dynamically from the search results
                PopulateAvailableColumnsFromResults(result);

                // Trigger property change notifications for debugging
                OnPropertyChanged(nameof(HasInventoryItems));
                OnPropertyChanged(nameof(ShowNothingFoundIndicator));

                System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] UI Thread - HasInventoryItems: {HasInventoryItems}, ShowNothingFoundIndicator: {ShowNothingFoundIndicator}");
            });

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Search operation completed successfully with {transferItems.Count} items");
            _logger.LogInformation("Transfer search completed. Found {Count} inventory items", transferItems.Count);
            ReportProgress($"Found {transferItems.Count} transfer candidates", "search", 100, true);

            // Auto-collapse the search panel if results were found
            if (transferItems.Count > 0)
            {
                PanelCollapseRequested?.Invoke(this, EventArgs.Empty);
                _logger.LogDebug("Auto-collapse requested after successful search with {Count} results", transferItems.Count);
            }
        }
        finally
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] ExecuteSearchAsync - Setting IsLoading = false");
            IsLoading = false;
        }
    }

    /// <summary>
    /// Enhanced reset functionality with comprehensive form state management
    /// Resets search criteria and refreshes all data - only called by Reset button or tab switching
    /// </summary>
    [RelayCommand]
    private async Task ResetSearchAsync()
    {
        try
        {
            IsLoading = true;
            ReportProgress("Resetting transfer form...", "reset", 0);

            _logger.LogInformation("User initiated form reset - clearing all transfer form state");

            // Clear search criteria
            SelectedPart = null;
            SelectedOperation = null;
            SelectedToLocation = null;
            PartText = string.Empty;
            OperationText = string.Empty;
            ToLocationText = string.Empty; // Reset destination location on explicit reset

            // Reset transfer configuration
            TransferQuantity = 1;
            MaxTransferQuantity = 0;

            // Clear all selections and data
            InventoryItems.Clear();
            SelectedInventoryItem = null;
            SelectedInventoryItems.Clear();

            // Reset search state
            _hasSearchBeenExecuted = false;

            ReportProgress("Reloading master data...", "reset", 50);

            // Reload all ComboBox data to ensure fresh data
            await LoadComboBoxDataAsync().ConfigureAwait(false);

            _logger.LogInformation("Transfer form completely reset - all fields cleared and master data refreshed");
            ReportProgress("Transfer form reset complete", "reset", 100, true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during form reset operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Reset operation failed", _applicationState.CurrentUser ?? "SYSTEM");
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
        // Enhanced transfer logic with improved validation and partial transfer support
        var itemsToTransfer = SelectedInventoryItems.Count > 0 ? SelectedInventoryItems.ToList() :
            (SelectedInventoryItem != null ? new List<TransferInventoryItem> { SelectedInventoryItem } : new List<TransferInventoryItem>());

        if (!itemsToTransfer.Any())
        {
            _logger.LogWarning("Transfer operation attempted with no items selected");
            await Services.ErrorHandling.HandleErrorAsync(
                new InvalidOperationException("No items selected for transfer"),
                "Transfer operation failed",
                _applicationState.CurrentUser ?? "SYSTEM");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedToLocation))
        {
            _logger.LogWarning("Transfer operation attempted without destination location");
            await Services.ErrorHandling.HandleErrorAsync(
                new InvalidOperationException("Destination location is required"),
                "Transfer operation failed",
                _applicationState.CurrentUser ?? "SYSTEM");
            return;
        }

        try
        {
            IsLoading = true;
            ReportProgress($"Initiating transfer of {itemsToTransfer.Count} item(s)...", "transfer", 0);

            var totalItems = itemsToTransfer.Count;
            var processedItems = 0;
            var successfulTransfers = 0;
            var failedTransfers = 0;
            var transferResults = new List<TransferResult>();

            foreach (var item in itemsToTransfer)
            {
                var fromLocation = item.Location;
                var partId = item.PartId;
                var operation = item.Operation ?? string.Empty;

                ReportProgress($"Processing {partId} ({processedItems + 1} of {totalItems})...", "transfer",
                    (int)((double)processedItems / totalItems * 80)); // Use 80% for processing, 20% for finalization

                // Enhanced validation - ensure destination is different from source
                if (fromLocation.Equals(SelectedToLocation, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Transfer skipped - same location: {PartId} at {Location}", partId, fromLocation);
                    transferResults.Add(new TransferResult
                    {
                        PartId = partId,
                        Success = false,
                        ErrorMessage = "Source and destination locations are the same"
                    });
                    failedTransfers++;
                    processedItems++;
                    continue;
                }

                // Determine transfer quantity
                // For single item selection, use specified TransferQuantity (supports partial transfers)
                // For multiple item selection, transfer complete quantities
                var transferQuantity = (totalItems == 1) ? TransferQuantity : item.Quantity;

                // Enhanced quantity validation
                if (transferQuantity > item.Quantity)
                {
                    _logger.LogWarning("Transfer quantity {TransferQuantity} exceeds available {Available} for {PartId}",
                        transferQuantity, item.Quantity, partId);
                    transferResults.Add(new TransferResult
                    {
                        PartId = partId,
                        Success = false,
                        ErrorMessage = $"Requested quantity ({transferQuantity}) exceeds available ({item.Quantity})"
                    });
                    failedTransfers++;
                    processedItems++;
                    continue;
                }

                if (transferQuantity <= 0)
                {
                    _logger.LogWarning("Invalid transfer quantity {TransferQuantity} for {PartId}", transferQuantity, partId);
                    transferResults.Add(new TransferResult
                    {
                        PartId = partId,
                        Success = false,
                        ErrorMessage = "Transfer quantity must be greater than zero"
                    });
                    failedTransfers++;
                    processedItems++;
                    continue;
                }

                // Execute transfer with enhanced logic
                bool transferResult;
                var isPartialTransfer = transferQuantity < item.Quantity && totalItems == 1;

                if (isPartialTransfer)
                {
                    // Partial quantity transfer (only for single item)
                    _logger.LogInformation("Executing partial transfer: {TransferQuantity} of {TotalQuantity} units for {PartId} from {FromLocation} to {ToLocation}",
                        transferQuantity, item.Quantity, partId, fromLocation, SelectedToLocation);

                    transferResult = await _databaseService.TransferQuantityAsync(
                        item.BatchNumber ?? string.Empty,
                        partId,
                        operation,
                        transferQuantity,
                        item.Quantity,
                        SelectedToLocation,
                        _applicationState.CurrentUser ?? "SYSTEM"
                    );
                }
                else
                {
                    // Complete item transfer
                    _logger.LogInformation("Executing complete transfer: {TransferQuantity} units for {PartId} from {FromLocation} to {ToLocation}",
                        transferQuantity, partId, fromLocation, SelectedToLocation);

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
                    transferResults.Add(new TransferResult
                    {
                        PartId = partId,
                        Success = true,
                        TransferredQuantity = transferQuantity,
                        FromLocation = fromLocation,
                        ToLocation = SelectedToLocation
                    });

                    // Update UI - remove or update item
                    if (transferQuantity >= item.Quantity)
                    {
                        // Complete transfer - remove item from list
                        InventoryItems.Remove(item);
                    }
                    else
                    {
                        // Partial transfer - update remaining quantity (single item only)
                        item.Quantity -= transferQuantity;
                        item.LastUpdated = DateTime.Now;

                        // Update MaxTransferQuantity for remaining partial item
                        OnPropertyChanged(nameof(MaxTransferQuantity));
                    }

                    // Fire event for each successful transfer with "TRANSFER" transaction type
                    ItemsTransferred?.Invoke(this, new ItemsTransferredEventArgs
                    {
                        PartId = partId,
                        Operation = operation,
                        FromLocation = fromLocation,
                        ToLocation = SelectedToLocation,
                        TransferredQuantity = transferQuantity,
                        TransferTime = DateTime.Now,
                        TransactionType = "TRANSFER", // MTM-specific transaction type
                        IsPartialTransfer = isPartialTransfer
                    });

                    _logger.LogInformation("Successfully transferred {Quantity} units of {PartId} from {FromLocation} to {ToLocation} (Transaction Type: TRANSFER)",
                        transferQuantity, partId, fromLocation, SelectedToLocation);
                }
                else
                {
                    _logger.LogError("Transfer operation failed for {PartId}", partId);
                    transferResults.Add(new TransferResult
                    {
                        PartId = partId,
                        Success = false,
                        ErrorMessage = "Database transfer operation failed"
                    });
                    failedTransfers++;
                }

                processedItems++;
            }

            // Enhanced final status report
            var finalMessage = GenerateTransferSummary(successfulTransfers, failedTransfers, totalItems, TransferQuantity);
            ReportProgress(finalMessage, "transfer", 100, true, failedTransfers > 0);

            // Show enhanced success overlay with transfer details
            if (successfulTransfers > 0)
            {
                ShowTransferSuccessDetailsAsync(transferResults.Where(r => r.Success).ToList(), failedTransfers);
            }

            // Enhanced form reset - only reset fields when appropriate (not on partial transfer)
            if (successfulTransfers > 0)
            {
                var hasPartialTransfer = transferResults.Any(r => r.Success && totalItems == 1 && r.TransferredQuantity < MaxTransferQuantity);
                ResetAfterSuccessfulTransferAsync(isPartialTransfer: hasPartialTransfer);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during transfer operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Transfer operation failed", _applicationState.CurrentUser ?? "SYSTEM");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Generates a comprehensive transfer summary message
    /// </summary>
    private string GenerateTransferSummary(int successful, int failed, int total, int transferQuantity)
    {
        if (total == 1)
        {
            return successful > 0
                ? $"Transfer complete: {transferQuantity} units transferred successfully"
                : "Transfer failed: Unable to complete operation";
        }

        return $"Batch transfer complete: {successful} successful, {failed} failed out of {total} items";
    }

    /// <summary>
    /// Shows detailed transfer success information
    /// </summary>
    private void ShowTransferSuccessDetailsAsync(List<TransferResult> successfulTransfers, int failedCount)
    {
        if (!successfulTransfers.Any()) return;

        try
        {
            if (successfulTransfers.Count == 1)
            {
                var transfer = successfulTransfers.First();
                ShowTransferSuccessOverlay(transfer.PartId, "", transfer.FromLocation, transfer.ToLocation, transfer.TransferredQuantity);
            }
            else
            {
                ShowBatchTransferSuccessOverlay(successfulTransfers.Count, failedCount, successfulTransfers.First().ToLocation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing transfer success details");
        }
    }

    /// <summary>
    /// Enhanced form reset after successful transfer with intelligent state management
    /// Preserves user context while preparing for next operation
    /// </summary>
    private void ResetAfterSuccessfulTransferAsync(bool isPartialTransfer)
    {
        try
        {
            _logger.LogInformation("Resetting form after successful transfer - isPartialTransfer: {IsPartialTransfer}", isPartialTransfer);

            // For partial transfers, preserve the context to allow additional transfers
            if (isPartialTransfer)
            {
                // Keep the partially transferred item selected
                // Keep destination location for potential additional transfers
                // Only reset quantity to 1 for next transfer
                TransferQuantity = 1;

                _logger.LogInformation("Partial transfer completed - preserving selection and location for additional transfers");
            }
            else
            {
                // Complete transfer - clear selections but preserve search results
                SelectedInventoryItem = null;
                SelectedInventoryItems.Clear();
                TransferQuantity = 1;

                // Preserve destination location - user might want to transfer more items to same location
                // Preserve search results - user might want to transfer additional items
                // These will be reset only via explicit Reset button or tab switching

                _logger.LogInformation("Complete transfer finished - cleared selections, preserved search results and destination");
            }

            // Update computed properties
            OnPropertyChanged(nameof(MaxTransferQuantity));
            OnPropertyChanged(nameof(CanTransfer));
            OnPropertyChanged(nameof(HasQuantityValidationError));
            OnPropertyChanged(nameof(HasLocationValidationError));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting form after transfer");
        }
    }

    /// <summary>
    /// Handles tab switching by resetting form state appropriately
    /// Called when user navigates away from the Transfer tab
    /// </summary>
    public async Task HandleTabSwitchAsync()
    {
        try
        {
            _logger.LogInformation("Transfer tab deactivated - performing soft reset for tab switching");

            // Don't interrupt ongoing operations
            if (IsLoading)
            {
                _logger.LogWarning("Tab switching while transfer operation in progress - skipping reset");
                return;
            }

            // Soft reset - preserve search results but clear transfer configuration
            SelectedInventoryItem = null;
            SelectedInventoryItems.Clear();
            ToLocationText = string.Empty; // Clear destination for fresh start on return
            TransferQuantity = 1;
            MaxTransferQuantity = 0;

            // Keep search criteria and results for when user returns
            // This provides better UX as user doesn't lose their search context

            _logger.LogInformation("Transfer tab soft reset completed - preserved search context");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during tab switch handling");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Tab switching error", _applicationState.CurrentUser ?? "SYSTEM");
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
    /// Command for editing selected inventory items (opens edit dialog)
    /// Matches CustomDataGrid functionality for consistent user experience
    /// </summary>
    public IRelayCommand EditTransferCommand => ExecuteEditTransferCommand;

    /// <summary>
    /// Opens the EditInventory dialog for the selected item (via double-click)
    /// </summary>
    public IRelayCommand OpenEditInventoryCommand => ExecuteEditTransferCommand;

    /// <summary>
    /// Executes inventory edit operation for selected transfer items
    /// Uses inventory editing service to load, edit, and save changes
    /// </summary>
    [RelayCommand]
    private async Task ExecuteEditTransferAsync()
    {
        try
        {
            if (SelectedInventoryItem == null)
            {
                _logger.LogWarning("Edit operation attempted with no item selected");
                return;
            }

            _logger.LogInformation("Starting edit operation for transfer item: {PartId} at {Location}",
                SelectedInventoryItem.PartId, SelectedInventoryItem.Location);

            // Use the same edit service as the main CustomDataGrid for consistency
            var editService = Program.GetOptionalService<IInventoryEditingService>();
            if (editService == null)
            {
                _logger.LogError("InventoryEditingService not available from DI container");
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException("Edit service not available"),
                    "Edit operation failed",
                    _applicationState.CurrentUser ?? "SYSTEM");
                return;
            }

            // Load the current inventory item for editing
            var editModel = await editService.LoadInventoryItemForEditAsync(SelectedInventoryItem.Id);
            if (editModel == null)
            {
                _logger.LogWarning("Could not load inventory item {Id} for editing", SelectedInventoryItem.Id);
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException($"Inventory item {SelectedInventoryItem.Id} not found"),
                    "Edit operation failed",
                    _applicationState.CurrentUser ?? "SYSTEM");
                return;
            }

            // For now, let's just try to update notes as a simple edit operation
            // In the future, this could open a full edit dialog
            var currentNotes = editModel.Notes ?? string.Empty;
            var newNotes = currentNotes + $" [Transfer Edit: {DateTime.Now:HH:mm}]";

            // Update notes using the service
            var result = await editService.UpdateNotesOnlyAsync(
                SelectedInventoryItem.Id,
                SelectedInventoryItem.PartId,
                SelectedInventoryItem.BatchNumber ?? string.Empty,
                newNotes,
                _applicationState.CurrentUser ?? "SYSTEM"
            );

            if (result.Success && result.ChangedFields.Any())
            {
                _logger.LogInformation("Edit successful for {PartId} - notes updated", SelectedInventoryItem.PartId);

                // Update the item in the collection with new notes
                SelectedInventoryItem.Notes = newNotes;
                SelectedInventoryItem.LastUpdated = DateTime.Now;

                // Show success feedback
                ReportProgress($"Successfully updated notes for {SelectedInventoryItem.PartId}", "edit", 100, true);
            }
            else if (!result.Success)
            {
                _logger.LogWarning("Edit operation failed for {PartId}: {Error}",
                    SelectedInventoryItem.PartId, result.ErrorMessage);
                await Services.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.ErrorMessage ?? "Edit operation failed"),
                    "Edit operation failed",
                    _applicationState.CurrentUser ?? "SYSTEM");
            }
            else
            {
                _logger.LogInformation("Edit completed without changes for {PartId}", SelectedInventoryItem.PartId);
                ReportProgress($"No changes made to {SelectedInventoryItem.PartId}", "edit", 100, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during edit transfer operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Edit operation failed", _applicationState.CurrentUser ?? "SYSTEM");
        }
    }

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
    /// Opens EditInventoryView for selected item
    /// </summary>
    [RelayCommand]
    private async Task EditItemAsync(object? parameter)
    {
        try
        {
            var item = parameter as TransferInventoryItem ?? SelectedInventoryItem;
            if (item == null)
            {
                _logger.LogWarning("Edit item command executed with no item selected");
                return;
            }

            _logger.LogInformation("Opening EditInventoryView for item: {PartId}", item.PartId);

            // Create EditInventoryViewModel if not exists
            if (EditInventoryViewModel == null)
            {
                var editingService = Program.GetOptionalService<IInventoryEditingService>();
                var masterDataService = Program.GetOptionalService<IMasterDataService>();

                if (editingService == null || masterDataService == null)
                {
                    _logger.LogError("Required services not available for EditInventoryView");
                    return;
                }

                EditInventoryViewModel = new MTM_WIP_Application_Avalonia.ViewModels.Overlay.EditInventoryViewModel(
                    Program.GetService<ILogger<MTM_WIP_Application_Avalonia.ViewModels.Overlay.EditInventoryViewModel>>(),
                    editingService,
                    masterDataService);
            }

            // Initialize with the selected item
            await EditInventoryViewModel.InitializeAsync(new MTM_WIP_Application_Avalonia.Models.InventoryItem
            {
                Id = item.Id,
                PartId = item.PartId,
                Operation = item.Operation ?? string.Empty,
                Location = item.Location,
                Quantity = item.Quantity,
                ItemType = item.ItemType ?? string.Empty,
                BatchNumber = item.BatchNumber ?? string.Empty,
                Notes = item.Notes ?? string.Empty,
                User = item.User ?? string.Empty,
                ReceiveDate = item.ReceiveDate,
                LastUpdated = item.LastUpdated
            });

            ShowEditInventoryView = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening EditInventoryView");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to open edit dialog", _applicationState.CurrentUser ?? "SYSTEM");
        }
    }

    /// <summary>
    /// Closes EditInventoryView panel
    /// </summary>
    [RelayCommand]
    private void CloseEditInventory()
    {
        try
        {
            ShowEditInventoryView = false;
            EditInventoryViewModel?.Cleanup();
            _logger.LogDebug("EditInventoryView closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing EditInventoryView");
        }
    }

    /// <summary>
    /// Shows column customization panel
    /// </summary>
    [RelayCommand]
    private void ShowColumnCustomizationPanel()
    {
        try
        {
            ShowColumnCustomization = true;
            _logger.LogDebug("Column customization panel opened");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing column customization");
        }
    }

    /// <summary>
    /// Saves column preferences to usr_ui_settings table
    /// </summary>
    [RelayCommand]
    private async Task SaveColumnPreferencesAsync()
    {
        try
        {
            var columnSettings = new
            {
                GridId = "TransferTabView_DataGrid",
                Columns = new[]
                {
                    new { Name = "Select", Visible = true, Order = 0, Width = 60 },
                    new { Name = "PartId", Visible = true, Order = 1, Width = 150 },
                    new { Name = "Operation", Visible = true, Order = 2, Width = 100 },
                    new { Name = "Location", Visible = true, Order = 3, Width = 120 },
                    new { Name = "Quantity", Visible = true, Order = 4, Width = 100 },
                    new { Name = "WipLocation", Visible = true, Order = 5, Width = 150 },
                    new { Name = "Actions", Visible = true, Order = 6, Width = 120 }
                }
            };

            var jsonSettings = System.Text.Json.JsonSerializer.Serialize(columnSettings);

            // Use usr_ui_settings_SetJsonSetting to save column preferences
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = _applicationState.CurrentUser ?? Environment.UserName,
                ["p_SettingKey"] = "TransferTabView_ColumnSettings",
                ["p_JsonValue"] = jsonSettings
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "usr_ui_settings_SetJsonSetting",
                parameters
            );

            if (result.Status == 1)
            {
                _logger.LogInformation("Column preferences saved successfully for user: {User}", _applicationState.CurrentUser);
            }
            else
            {
                _logger.LogWarning("Failed to save column preferences: Status {Status}", result.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving column preferences");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to save column preferences", _applicationState.CurrentUser ?? "SYSTEM");
        }
    }

    /// <summary>
    /// Loads column preferences from usr_ui_settings table
    /// </summary>
    private async Task LoadColumnPreferencesAsync()
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = _applicationState.CurrentUser ?? Environment.UserName,
                ["p_SettingKey"] = "TransferTabView_ColumnSettings"
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "usr_ui_settings_GetJsonSetting",
                parameters
            );

            if (result.Status == 1 && result.Data.Rows.Count > 0)
            {
                var jsonValue = result.Data.Rows[0]["JsonValue"]?.ToString();
                if (!string.IsNullOrEmpty(jsonValue))
                {
                    // Parse and apply column settings
                    var columnSettings = System.Text.Json.JsonSerializer.Deserialize<dynamic>(jsonValue);
                    _logger.LogInformation("Column preferences loaded successfully for user: {User}", _applicationState.CurrentUser);
                    // TODO: Apply column settings to DataGrid
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading column preferences");
            // Don't show error to user for failed preference loading - use defaults
        }
    }

    /// <summary>
    /// Transfers single item directly from DataGrid action
    /// </summary>
    [RelayCommand]
    private async Task TransferItemAsync(object? parameter)
    {
        try
        {
            var item = parameter as TransferInventoryItem;
            if (item == null)
            {
                _logger.LogWarning("Transfer item command executed with no item parameter");
                return;
            }

            // Set as selected item and execute standard transfer
            SelectedInventoryItem = item;
            await ExecuteTransferAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring item directly from DataGrid");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Transfer operation failed", _applicationState.CurrentUser ?? "SYSTEM");
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
                new MTM_Shared_Logic.Models.InventoryItem
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
                new MTM_Shared_Logic.Models.InventoryItem
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
                new MTM_Shared_Logic.Models.InventoryItem
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
                new MTM_Shared_Logic.Models.InventoryItem
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
    private static TransferInventoryItem ConvertToTransferItem(MTM_Shared_Logic.Models.InventoryItem item)
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
    private DataTable ConvertInventoryToDataTable(ObservableCollection<MTM_Shared_Logic.Models.InventoryItem> inventoryItems)
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
    /// Populates the AvailableColumns collection based on the columns returned from stored procedure
    /// </summary>
    private void PopulateAvailableColumnsFromResults(System.Data.DataTable result)
    {
        try
        {
            AvailableColumns.Clear();
            _columnVisibility.Clear();

            if (result?.Columns == null)
            {
                _logger.LogWarning("No columns available from search results to populate AvailableColumns");
                return;
            }

            var order = 0;
            foreach (System.Data.DataColumn column in result.Columns)
            {
                var columnName = column.ColumnName;
                var displayName = GetUserFriendlyColumnName(columnName);
                var suggestedWidth = GetSuggestedColumnWidth(columnName);

                var columnItem = new ColumnItem
                {
                    ColumnName = columnName,
                    DisplayName = displayName,
                    IsVisible = true, // Default to visible
                    Width = suggestedWidth,
                    Order = order++
                };

                AvailableColumns.Add(columnItem);
                _columnVisibility[columnName] = true;

                _logger.LogDebug("Added column: {ColumnName} -> '{DisplayName}' (Width: {Width})",
                    columnName, displayName, suggestedWidth);
            }

            // Initialize column visibility backing properties based on search results
            IsPartIdColumnVisible = _columnVisibility.GetValueOrDefault("PartID", true);
            IsOperationColumnVisible = _columnVisibility.GetValueOrDefault("Operation", true);
            IsFromLocationColumnVisible = _columnVisibility.GetValueOrDefault("Location", true);
            IsAvailableQuantityColumnVisible = _columnVisibility.GetValueOrDefault("Quantity", true);
            IsTransferQuantityColumnVisible = true; // Always visible - UI only
            IsNotesColumnVisible = _columnVisibility.GetValueOrDefault("Notes", true);

            _logger.LogInformation("Populated {Count} columns from search results with user-friendly aliases", AvailableColumns.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating available columns from search results");
        }
    }

    /// <summary>
    /// Gets user-friendly display name for database column
    /// </summary>
    private static string GetUserFriendlyColumnName(string columnName)
    {
        return ColumnDisplayNames.TryGetValue(columnName, out var displayName)
            ? displayName
            : columnName; // Fallback to original name if no mapping exists
    }

    /// <summary>
    /// Gets suggested column width for database column
    /// </summary>
    private static double GetSuggestedColumnWidth(string columnName)
    {
        return ColumnWidths.TryGetValue(columnName, out var width)
            ? width
            : 100; // Default width if no mapping exists
    }

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

    /// <summary>
    /// Gets the visibility state for a specific column
    /// </summary>
    public bool GetColumnVisibility(string columnName)
    {
        return _columnVisibility.TryGetValue(columnName, out var isVisible) && isVisible;
    }

    /// <summary>
    /// Sets the visibility state for a specific column
    /// </summary>
    public void SetColumnVisibility(string columnName, bool isVisible)
    {
        _columnVisibility[columnName] = isVisible;

        // Update the corresponding ColumnItem if it exists
        var columnItem = AvailableColumns.FirstOrDefault(c => c.ColumnName == columnName);
        if (columnItem != null)
        {
            columnItem.IsVisible = isVisible;
        }

        // Update the specific backing property based on column name
        switch (columnName)
        {
            case "PartID":
                IsPartIdColumnVisible = isVisible;
                break;
            case "Operation":
                IsOperationColumnVisible = isVisible;
                break;
            case "Location":
                IsFromLocationColumnVisible = isVisible;
                break;
            case "Quantity":
                IsAvailableQuantityColumnVisible = isVisible;
                break;
            case "Notes":
                IsNotesColumnVisible = isVisible;
                break;
        }

        _logger.LogDebug("Column visibility updated: {ColumnName} = {IsVisible}", columnName, isVisible);
    }



    #region Column Visibility Properties for AXAML Binding

    [ObservableProperty]
    private bool _isPartIdColumnVisible = true;

    [ObservableProperty]
    private bool _isOperationColumnVisible = true;

    [ObservableProperty]
    private bool _isFromLocationColumnVisible = true;

    [ObservableProperty]
    private bool _isAvailableQuantityColumnVisible = true;

    [ObservableProperty]
    private bool _isTransferQuantityColumnVisible = true;

    [ObservableProperty]
    private bool _isNotesColumnVisible = true;

    #endregion

    #endregion
}

#region Event Args

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
}

#endregion

/// <summary>
/// Transfer-specific inventory item class for transfer operations
/// Extends the base inventory item with transfer-specific properties and methods
/// </summary>
public class TransferInventoryItem : INotifyPropertyChanged
{
    #region Base Inventory Properties (from MTM_Shared_Logic.Models.InventoryItem)

    /// <summary>
    /// Gets or sets the unique inventory item ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Part ID
    /// </summary>
    public string PartId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current location of the inventory item
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the operation number (as string: "90", "100", "110", etc.)
    /// </summary>
    public string? Operation { get; set; }

    /// <summary>
    /// Gets or sets the current quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the item type (WIP, RM, FG, etc.)
    /// </summary>
    public string ItemType { get; set; } = "WIP";

    /// <summary>
    /// Gets or sets the date when the item was received
    /// </summary>
    public DateTime ReceiveDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the last updated timestamp
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the user who last updated the item
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the batch number for tracking
    /// </summary>
    public string? BatchNumber { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Legacy ID property for compatibility (same as Id)
    /// </summary>
    public int ID => Id;

    #endregion

    #region Transfer-Specific Properties

    private string _fromLocation = string.Empty;
    private string _toLocation = string.Empty;
    private int _availableQuantity;
    private int _transferQuantity = 1;

    /// <summary>
    /// Gets or sets the source location for transfer (usually same as Location)
    /// </summary>
    public string FromLocation
    {
        get => _fromLocation;
        set
        {
            if (_fromLocation != value)
            {
                _fromLocation = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the destination location for transfer
    /// </summary>
    public string ToLocation
    {
        get => _toLocation;
        set
        {
            if (_toLocation != value)
            {
                _toLocation = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the available quantity for transfer
    /// </summary>
    public int AvailableQuantity
    {
        get => _availableQuantity;
        set
        {
            if (_availableQuantity != value)
            {
                _availableQuantity = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the quantity to transfer (for UI binding)
    /// </summary>
    public int TransferQuantity
    {
        get => _transferQuantity;
        set
        {
            if (_transferQuantity != value)
            {
                _transferQuantity = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    #region Transfer-Specific Computed Properties

    /// <summary>
    /// Indicates if this item can be transferred (has positive quantity and different to/from locations)
    /// </summary>
    public bool CanTransfer =>
        Quantity > 0 &&
        !string.IsNullOrEmpty(ToLocation) &&
        !FromLocation.Equals(ToLocation, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indicates if this is a partial transfer (transfer quantity less than total quantity)
    /// </summary>
    public bool IsPartialTransfer => TransferQuantity > 0 && TransferQuantity < Quantity;

    /// <summary>
    /// Gets a display string for transfer direction (From → To)
    /// </summary>
    public string TransferDirection => string.IsNullOrEmpty(ToLocation)
        ? "Select destination"
        : $"{FromLocation} → {ToLocation}";

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Transfer Methods

    /// <summary>
    /// Validates the transfer configuration for this item
    /// </summary>
    /// <returns>True if transfer is valid, false otherwise</returns>
    public bool ValidateTransfer()
    {
        // Check basic requirements
        if (string.IsNullOrEmpty(ToLocation))
            return false;

        if (string.IsNullOrEmpty(FromLocation))
            return false;

        // Check locations are different
        if (FromLocation.Equals(ToLocation, StringComparison.OrdinalIgnoreCase))
            return false;

        // Check quantity constraints
        if (TransferQuantity <= 0 || TransferQuantity > Quantity)
            return false;

        return true;
    }

    /// <summary>
    /// Prepares this item for transfer by setting transfer-specific properties
    /// </summary>
    /// <param name="destinationLocation">Where to transfer to</param>
    /// <param name="transferQuantity">How much to transfer (0 for full quantity)</param>
    public void PrepareForTransfer(string destinationLocation, int transferQuantity = 0)
    {
        FromLocation = Location; // Current location is source
        ToLocation = destinationLocation;
        AvailableQuantity = Quantity;
        TransferQuantity = transferQuantity > 0 ? Math.Min(transferQuantity, Quantity) : Quantity;
    }

    /// <summary>
    /// Updates the item after a successful transfer
    /// </summary>
    /// <param name="transferredQuantity">How much was actually transferred</param>
    /// <param name="wasComplete">True if complete item was transferred</param>
    public void UpdateAfterTransfer(int transferredQuantity, bool wasComplete)
    {
        if (wasComplete)
        {
            // Complete transfer - item moved to new location
            Location = ToLocation;
            FromLocation = ToLocation;
        }
        else
        {
            // Partial transfer - reduce quantity
            Quantity -= transferredQuantity;
            AvailableQuantity = Quantity;
        }

        LastUpdated = DateTime.Now;
        TransferQuantity = 1; // Reset for next transfer
    }

    #endregion

    #region ToString Override

    /// <summary>
    /// Returns a string representation of this transfer item
    /// </summary>
    public override string ToString()
    {
        return $"{PartId} ({Operation}) - {Quantity} at {Location}";
    }

    #endregion
}
