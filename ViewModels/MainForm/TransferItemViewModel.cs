using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Commands;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for the inventory transfer interface (Control_TransferTab).
/// Provides comprehensive functionality for transferring inventory items between locations,
/// including search capabilities, quantity specification, batch transfer operations, 
/// and complete transaction history tracking.
/// </summary>
public class TransferItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;
    private readonly IDatabaseService _databaseService;

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

    private string? _selectedPart;
    /// <summary>
    /// Selected part ID for filtering inventory searches
    /// </summary>
    public string? SelectedPart
    {
        get => _selectedPart;
        set => SetProperty(ref _selectedPart, value);
    }

    private string? _selectedOperation;
    /// <summary>
    /// Selected operation for refined filtering (optional)
    /// </summary>
    public string? SelectedOperation
    {
        get => _selectedOperation;
        set => SetProperty(ref _selectedOperation, value);
    }

    // Text properties for AutoCompleteBox two-way binding
    private string _partText = string.Empty;
    /// <summary>
    /// Text content for Part AutoCompleteBox
    /// </summary>
    public string PartText
    {
        get => _partText;
        set => SetProperty(ref _partText, value ?? string.Empty);
    }

    private string _operationText = string.Empty;
    /// <summary>
    /// Text content for Operation AutoCompleteBox
    /// </summary>
    public string OperationText
    {
        get => _operationText;
        set => SetProperty(ref _operationText, value ?? string.Empty);
    }

    #endregion

    #region Transfer Configuration Properties

    private string? _selectedToLocation;
    /// <summary>
    /// Selected destination location for transfer operations
    /// </summary>
    public string? SelectedToLocation
    {
        get => _selectedToLocation;
        set => SetProperty(ref _selectedToLocation, value);
    }

    // Text property for destination location AutoCompleteBox
    private string _toLocationText = string.Empty;
    /// <summary>
    /// Text content for To Location AutoCompleteBox
    /// </summary>
    public string ToLocationText
    {
        get => _toLocationText;
        set => SetProperty(ref _toLocationText, value ?? string.Empty);
    }

    private int _transferQuantity = 1;
    /// <summary>
    /// Quantity to transfer (limited by available inventory)
    /// </summary>
    public int TransferQuantity
    {
        get => _transferQuantity;
        set => SetProperty(ref _transferQuantity, value);
    }

    private int _maxTransferQuantity = 0;
    /// <summary>
    /// Maximum quantity available for transfer from selected item
    /// </summary>
    public int MaxTransferQuantity
    {
        get => _maxTransferQuantity;
        set => SetProperty(ref _maxTransferQuantity, value);
    }

    #endregion

    #region Selection and State Properties

    private InventoryItem? _selectedInventoryItem;
    /// <summary>
    /// Currently selected inventory item for transfer operations
    /// </summary>
    public InventoryItem? SelectedInventoryItem
    {
        get => _selectedInventoryItem;
        set
        {
            SetProperty(ref _selectedInventoryItem, value);
            UpdateMaxTransferQuantity();
        }
    }

    private bool _isLoading;
    /// <summary>
    /// Indicates if a background operation is in progress
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    public bool HasInventoryItems => InventoryItems.Count > 0;

    /// <summary>
    /// Indicates if transfer operation can be performed
    /// </summary>
    public bool CanTransfer => SelectedInventoryItem != null && 
                              !string.IsNullOrWhiteSpace(SelectedToLocation) && 
                              TransferQuantity > 0 && 
                              TransferQuantity <= MaxTransferQuantity &&
                              !IsLoading;

    /// <summary>
    /// Indicates if search operations can be performed
    /// </summary>
    public bool CanSearch => !IsLoading;

    #endregion

    #region Commands

    /// <summary>
    /// Executes inventory search based on selected criteria with progress tracking
    /// </summary>
    public ICommand SearchCommand { get; private set; } = null!;

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    public ICommand ResetCommand { get; private set; } = null!;

    /// <summary>
    /// Executes transfer operations for selected inventory items
    /// </summary>
    public ICommand TransferCommand { get; private set; } = null!;

    /// <summary>
    /// Prints current inventory view with transfer details
    /// </summary>
    public ICommand PrintCommand { get; private set; } = null!;

    /// <summary>
    /// Toggles quick actions panel
    /// </summary>
    public ICommand TogglePanelCommand { get; private set; } = null!;

    /// <summary>
    /// Loads initial data including parts, operations, and locations
    /// </summary>
    public ICommand LoadDataCommand { get; private set; } = null!;

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

    #endregion

    #region Constructor

    public TransferItemViewModel(
        IApplicationStateService applicationState,
        IDatabaseService databaseService,
        ILogger<TransferItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));

        Logger.LogInformation("TransferItemViewModel initialized with dependency injection");

        InitializeCommands();
        _ = LoadComboBoxDataAsync(); // Load real data from database
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        switch (e.PropertyName)
        {
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
                break;
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(CanTransfer));
                break;
            case nameof(TransferQuantity):
            case nameof(SelectedInventoryItem):
                OnPropertyChanged(nameof(CanTransfer));
                UpdateMaxTransferQuantity();
                break;
            case nameof(SelectedToLocation):
                OnPropertyChanged(nameof(CanTransfer));
                UpdateMaxTransferQuantity();
                ToLocationText = SelectedToLocation ?? string.Empty;
                break;
            case nameof(MaxTransferQuantity):
                OnPropertyChanged(nameof(CanTransfer));
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
            case nameof(ToLocationText):
                if (!string.IsNullOrEmpty(ToLocationText) && LocationOptions.Contains(ToLocationText))
                    SelectedToLocation = ToLocationText;
                break;
        }
    }

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        // Search command with progress tracking
        SearchCommand = new AsyncCommand(ExecuteSearchAsync, () => CanSearch);

        // Reset command
        ResetCommand = new AsyncCommand(ResetSearchAsync);

        // Transfer command with validation
        TransferCommand = new AsyncCommand(ExecuteTransferAsync, () => CanTransfer);

        // Print command
        PrintCommand = new AsyncCommand(ExecutePrintAsync, () => HasInventoryItems);

        // Toggle panel command
        TogglePanelCommand = new RelayCommand(() =>
        {
            PanelToggleRequested?.Invoke(this, EventArgs.Empty);
        });

        // Load data command
        LoadDataCommand = new AsyncCommand(LoadComboBoxDataAsync);
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Executes inventory search based on selected criteria with progress tracking
    /// </summary>
    private async Task ExecuteSearchAsync()
    {
        try
        {
            IsLoading = true;
            InventoryItems.Clear();
            SelectedInventoryItem = null;

            Logger.LogInformation("Executing transfer search for Part: {PartId}, Operation: {Operation}", 
                _selectedPart, _selectedOperation);

            // Dynamic search based on selection criteria
            System.Data.DataTable result;
            
            if (!string.IsNullOrWhiteSpace(_selectedPart) && !string.IsNullOrWhiteSpace(_selectedOperation))
            {
                // Search by both part and operation
                result = await _databaseService.GetInventoryByPartAndOperationAsync(_selectedPart, _selectedOperation);
            }
            else if (!string.IsNullOrWhiteSpace(_selectedPart))
            {
                // Search by part only
                result = await _databaseService.GetInventoryByPartIdAsync(_selectedPart);
            }
            else
            {
                // No search criteria specified, don't load anything
                Logger.LogWarning("No search criteria specified for transfer search");
                return;
            }

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

            Logger.LogInformation("Transfer search completed. Found {Count} inventory items", InventoryItems.Count);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    private async Task ResetSearchAsync()
    {
        try
        {
            IsLoading = true;

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

            // Reload all ComboBox data
            await LoadComboBoxDataAsync();

            Logger.LogInformation("Transfer search criteria reset and data refreshed");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Executes transfer operations for selected inventory items with comprehensive validation
    /// </summary>
    private async Task ExecuteTransferAsync()
    {
        if (SelectedInventoryItem == null || string.IsNullOrWhiteSpace(SelectedToLocation))
        {
            Logger.LogWarning("Transfer operation attempted with invalid selection");
            return;
        }

        try
        {
            IsLoading = true;

            var fromLocation = SelectedInventoryItem.Location;
            var partId = SelectedInventoryItem.PartID;
            var operation = SelectedInventoryItem.Operation ?? string.Empty;

            // Critical: Validate destination location is different from source
            if (fromLocation.Equals(SelectedToLocation, StringComparison.OrdinalIgnoreCase))
            {
                Logger.LogWarning("Transfer attempted to same location: {Location}", fromLocation);
                // TODO: Show user-friendly error message
                return;
            }

            // Validate transfer quantity against available inventory
            if (TransferQuantity > SelectedInventoryItem.Quantity)
            {
                Logger.LogWarning("Transfer quantity {TransferQuantity} exceeds available {Available}", 
                    TransferQuantity, SelectedInventoryItem.Quantity);
                // TODO: Show user-friendly error message
                return;
            }

            // Implement actual database transfer operations
            bool transferResult;
            
            if (TransferQuantity < SelectedInventoryItem.Quantity)
            {
                // Partial quantity transfer
                Logger.LogInformation("Executing partial transfer: {TransferQuantity} of {TotalQuantity} units", 
                    TransferQuantity, SelectedInventoryItem.Quantity);
                
                transferResult = await _databaseService.TransferQuantityAsync(
                    SelectedInventoryItem.BatchNumber ?? string.Empty,
                    partId,
                    operation,
                    TransferQuantity,
                    SelectedInventoryItem.Quantity,
                    SelectedToLocation,
                    _applicationState.CurrentUser
                );
            }
            else
            {
                // Complete item transfer
                Logger.LogInformation("Executing complete transfer: {TransferQuantity} units", TransferQuantity);
                
                transferResult = await _databaseService.TransferPartAsync(
                    SelectedInventoryItem.BatchNumber ?? string.Empty,
                    partId,
                    operation,
                    SelectedToLocation
                );
            }

            if (!transferResult)
            {
                Logger.LogError("Transfer operation failed");
                // TODO: Show user-friendly error message
                return;
            }

            // Update UI - simulate successful transfer
            if (TransferQuantity >= SelectedInventoryItem.Quantity)
            {
                // Complete transfer - remove item from list
                InventoryItems.Remove(SelectedInventoryItem);
            }
            else
            {
                // Partial transfer - update quantity
                SelectedInventoryItem.Quantity -= TransferQuantity;
                SelectedInventoryItem.LastUpdated = DateTime.Now;
            }

            // Fire event for integration with other components
            ItemsTransferred?.Invoke(this, new ItemsTransferredEventArgs
            {
                PartId = partId,
                Operation = operation,
                FromLocation = fromLocation,
                ToLocation = SelectedToLocation,
                TransferredQuantity = TransferQuantity,
                TransferTime = DateTime.Now
            });

            Logger.LogInformation("Successfully transferred {Quantity} units of {PartId} from {FromLocation} to {ToLocation}", 
                TransferQuantity, partId, fromLocation, SelectedToLocation);

            // Reset transfer form for next operation
            SelectedInventoryItem = null;
            TransferQuantity = 1;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Prints current inventory view with transfer details and formatted output
    /// </summary>
    private async Task ExecutePrintAsync()
    {
        try
        {
            IsLoading = true;

            // TODO: Implement print functionality using Core_DgvPrinter equivalent
            // Generate printable transfer report
            // var printer = new AvaloniaDataGridPrinter();
            // await printer.PrintDataGridAsync(InventoryItems,
            //     title: "Inventory Transfer Report",
            //     searchCriteria: $"Part: {SelectedPart}, Operation: {SelectedOperation}",
            //     transferConfiguration: $"Destination: {SelectedToLocation}, Quantity: {TransferQuantity}");

            Logger.LogInformation("Print operation initiated for transfer inventory view with {Count} items", 
                InventoryItems.Count);

            await Task.Delay(1000); // Simulate print operation
        }
        finally
        {
            IsLoading = false;
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
            Logger.LogInformation("Loading transfer ComboBox data from database");

            // Load Parts using md_part_ids_Get_All stored procedure
            var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (partResult.IsSuccess)
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
                Logger.LogInformation("Loaded {Count} parts for transfer", PartOptions.Count);
            }
            
            // Load Operations using md_operation_numbers_Get_All stored procedure
            var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            if (operationResult.IsSuccess)
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
                Logger.LogInformation("Loaded {Count} operations for transfer", OperationOptions.Count);
            }

            // Load Locations using md_locations_Get_All stored procedure
            var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _databaseService.GetConnectionString(),
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            if (locationResult.IsSuccess)
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
                Logger.LogInformation("Loaded {Count} locations for transfer", LocationOptions.Count);
            }

            Logger.LogInformation("Transfer ComboBox data loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Transfer ComboBox data");
            throw;
        }
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
        else
        {
            MaxTransferQuantity = 0;
            TransferQuantity = 1;
        }
    }

    /// <summary>
    /// Loads sample data for demonstration purposes
    /// </summary>
    private void LoadSampleData()
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
    }

    /// <summary>
    /// Loads sample inventory data for demonstration with proper filtering
    /// </summary>
    private void LoadSampleInventoryData()
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
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions from command operations with comprehensive logging
    /// </summary>
    private void HandleException(Exception ex)
    {
        Logger.LogError(ex, "Error in TransferItemViewModel operation");
        
        // TODO: Present user-friendly error message via error service
        // await _errorService.LogErrorAsync(ex);
        // Show user notification with appropriate message based on exception type
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Programmatically triggers a search operation
    /// </summary>
    public void TriggerSearch()
    {
        if (SearchCommand != null)
        {
            SearchCommand.Execute(null);
        }
    }

    /// <summary>
    /// Sets transfer configuration from external source (e.g., QuickButtons)
    /// </summary>
    public void SetTransferConfiguration(string partId, string operation, string toLocation, int quantity)
    {
        SelectedPart = partId;
        SelectedOperation = operation;
        SelectedToLocation = toLocation;
        TransferQuantity = quantity;
        
        // Trigger search to populate inventory grid
        TriggerSearch();
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

#endregion
