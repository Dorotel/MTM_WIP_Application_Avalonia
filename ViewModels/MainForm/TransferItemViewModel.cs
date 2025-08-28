using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for the inventory transfer interface (Control_TransferTab).
/// Provides comprehensive functionality for transferring inventory items between locations,
/// including search capabilities, quantity specification, batch transfer operations, 
/// and complete transaction history tracking.
/// </summary>
public class TransferItemViewModel : BaseViewModel
{
    private readonly MTM_Shared_Logic.Services.IInventoryService _inventoryService;
    private readonly IApplicationStateService _applicationState;

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
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    private string? _selectedOperation;
    /// <summary>
    /// Selected operation for refined filtering (optional)
    /// </summary>
    public string? SelectedOperation
    {
        get => _selectedOperation;
        set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
    }

    // Text properties for AutoCompleteBox two-way binding
    private string _partText = string.Empty;
    /// <summary>
    /// Text content for Part AutoCompleteBox
    /// </summary>
    public string PartText
    {
        get => _partText;
        set => this.RaiseAndSetIfChanged(ref _partText, value ?? string.Empty);
    }

    private string _operationText = string.Empty;
    /// <summary>
    /// Text content for Operation AutoCompleteBox
    /// </summary>
    public string OperationText
    {
        get => _operationText;
        set => this.RaiseAndSetIfChanged(ref _operationText, value ?? string.Empty);
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
        set => this.RaiseAndSetIfChanged(ref _selectedToLocation, value);
    }

    // Text property for destination location AutoCompleteBox
    private string _toLocationText = string.Empty;
    /// <summary>
    /// Text content for To Location AutoCompleteBox
    /// </summary>
    public string ToLocationText
    {
        get => _toLocationText;
        set => this.RaiseAndSetIfChanged(ref _toLocationText, value ?? string.Empty);
    }

    private int _transferQuantity = 1;
    /// <summary>
    /// Quantity to transfer (limited by available inventory)
    /// </summary>
    public int TransferQuantity
    {
        get => _transferQuantity;
        set => this.RaiseAndSetIfChanged(ref _transferQuantity, value);
    }

    private int _maxTransferQuantity = 0;
    /// <summary>
    /// Maximum quantity available for transfer from selected item
    /// </summary>
    public int MaxTransferQuantity
    {
        get => _maxTransferQuantity;
        set => this.RaiseAndSetIfChanged(ref _maxTransferQuantity, value);
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
            this.RaiseAndSetIfChanged(ref _selectedInventoryItem, value);
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
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _hasInventoryItems;
    public bool HasInventoryItems => _hasInventoryItems.Value;

    /// <summary>
    /// Indicates if transfer operation can be performed
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _canTransfer;
    public bool CanTransfer => _canTransfer.Value;

    /// <summary>
    /// Indicates if search operations can be performed
    /// </summary>
    private readonly ObservableAsPropertyHelper<bool> _canSearch;
    public bool CanSearch => _canSearch.Value;

    #endregion

    #region Commands

    /// <summary>
    /// Executes inventory search based on selected criteria with progress tracking
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; private set; } = null!;

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    public ReactiveCommand<Unit, Unit> ResetCommand { get; private set; } = null!;

    /// <summary>
    /// Executes transfer operations for selected inventory items
    /// </summary>
    public ReactiveCommand<Unit, Unit> TransferCommand { get; private set; } = null!;

    /// <summary>
    /// Prints current inventory view with transfer details
    /// </summary>
    public ReactiveCommand<Unit, Unit> PrintCommand { get; private set; } = null!;

    /// <summary>
    /// Toggles quick actions panel
    /// </summary>
    public ReactiveCommand<Unit, Unit> TogglePanelCommand { get; private set; } = null!;

    /// <summary>
    /// Loads initial data including parts, operations, and locations
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; } = null!;

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
        MTM_Shared_Logic.Services.IInventoryService inventoryService,
        IApplicationStateService applicationState,
        ILogger<TransferItemViewModel> logger) : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("TransferItemViewModel initialized with dependency injection");

        // Initialize computed properties
        _hasInventoryItems = this.WhenAnyValue(vm => vm.InventoryItems.Count)
            .Select(count => count > 0)
            .ToProperty(this, vm => vm.HasInventoryItems);

        _canSearch = this.WhenAnyValue(vm => vm.IsLoading)
            .Select(loading => !loading)
            .ToProperty(this, vm => vm.CanSearch);

        _canTransfer = this.WhenAnyValue(
                vm => vm.SelectedToLocation,
                vm => vm.TransferQuantity,
                vm => vm.MaxTransferQuantity,
                vm => vm.SelectedInventoryItem,
                vm => vm.IsLoading)
            .Select((tuple) =>
            {
                var (toLocation, quantity, maxQuantity, selectedItem, loading) = tuple;
                return !string.IsNullOrWhiteSpace(toLocation) &&
                       selectedItem != null &&
                       quantity > 0 &&
                       quantity <= maxQuantity &&
                       maxQuantity > 0 &&
                       !loading;
            })
            .ToProperty(this, vm => vm.CanTransfer);

        InitializeCommands();
        LoadSampleData(); // Load sample data for demonstration

        // Sync text properties with selected items
        this.WhenAnyValue(x => x.SelectedPart)
            .Subscribe(selected => PartText = selected ?? string.Empty);
        
        this.WhenAnyValue(x => x.SelectedOperation)
            .Subscribe(selected => OperationText = selected ?? string.Empty);
        
        this.WhenAnyValue(x => x.SelectedToLocation)
            .Subscribe(selected => ToLocationText = selected ?? string.Empty);

        // Sync selected items when text matches exactly
        this.WhenAnyValue(x => x.PartText)
            .Where(text => !string.IsNullOrEmpty(text) && PartOptions.Contains(text))
            .Subscribe(text => SelectedPart = text);
        
        this.WhenAnyValue(x => x.OperationText)
            .Where(text => !string.IsNullOrEmpty(text) && OperationOptions.Contains(text))
            .Subscribe(text => SelectedOperation = text);
        
        this.WhenAnyValue(x => x.ToLocationText)
            .Where(text => !string.IsNullOrEmpty(text) && LocationOptions.Contains(text))
            .Subscribe(text => SelectedToLocation = text);
    }

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        // Search command with progress tracking
        SearchCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ExecuteSearchAsync();
        }, this.WhenAnyValue(vm => vm.CanSearch));

        // Reset command
        ResetCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ResetSearchAsync();
        });

        // Transfer command with validation
        TransferCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ExecuteTransferAsync();
        }, this.WhenAnyValue(vm => vm.CanTransfer));

        // Print command
        PrintCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await ExecutePrintAsync();
        }, this.WhenAnyValue(vm => vm.HasInventoryItems));

        // Toggle panel command
        TogglePanelCommand = ReactiveCommand.Create(() =>
        {
            PanelToggleRequested?.Invoke(this, EventArgs.Empty);
        });

        // Load data command
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadComboBoxDataAsync();
        });

        // Reactive property updates for transfer quantity management
        this.WhenAnyValue(vm => vm.SelectedInventoryItem)
            .Subscribe(_ => UpdateMaxTransferQuantity());

        this.WhenAnyValue(vm => vm.MaxTransferQuantity)
            .Subscribe(maxQuantity =>
            {
                // Ensure transfer quantity doesn't exceed maximum
                if (TransferQuantity > maxQuantity)
                {
                    TransferQuantity = Math.Max(1, maxQuantity);
                }
            });

        // Centralized error handling
        Observable.Merge(
                SearchCommand.ThrownExceptions,
                ResetCommand.ThrownExceptions,
                TransferCommand.ThrownExceptions,
                PrintCommand.ThrownExceptions,
                LoadDataCommand.ThrownExceptions)
            .Subscribe(HandleException);
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

            // TODO: Implement database search operations
            // Dynamic search logic: Searches by part only or part+operation based on criteria
            // if (!string.IsNullOrWhiteSpace(_selectedOperation))
            // {
            //     // Search by both part and operation
            //     var result = await Dao_Inventory.GetInventoryByPartIdAndOperationAsync(
            //         _selectedPart, _selectedOperation, true);
            // }
            // else
            // {
            //     // Search by part only
            //     var result = await Dao_Inventory.GetInventoryByPartIdAsync(_selectedPart, true);
            // }

            // For demonstration, load sample filtered data
            await Task.Delay(500); // Simulate database operation
            LoadSampleInventoryData();

            Logger.LogInformation("Transfer search executed for Part: {PartId}, Operation: {Operation}", 
                _selectedPart, _selectedOperation);
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

            // TODO: Implement actual database transfer operations
            // Determine transfer type based on quantity
            // if (TransferQuantity < SelectedInventoryItem.Quantity)
            // {
            //     // Partial quantity transfer
            //     await Dao_Inventory.TransferInventoryQuantityAsync(
            //         SelectedInventoryItem.BatchNumber,
            //         partId,
            //         operation,
            //         TransferQuantity,
            //         SelectedInventoryItem.Quantity,
            //         SelectedToLocation,
            //         _applicationState.CurrentUser.UserName
            //     );
            // }
            // else
            // {
            //     // Complete item transfer
            //     await Dao_Inventory.TransferPartSimpleAsync(
            //         SelectedInventoryItem.BatchNumber,
            //         partId,
            //         operation,
            //         TransferQuantity.ToString(),
            //         SelectedToLocation
            //     );
            // }

            // TODO: Log transaction history for audit trail
            // Critical: ALL transfer operations create TRANSFER transactions regardless of operation number
            // var transaction = new InventoryTransaction
            // {
            //     TransactionType = TransactionType.TRANSFER, // Always TRANSFER when moving between locations
            //     PartId = partId,
            //     FromLocation = fromLocation,
            //     ToLocation = SelectedToLocation,
            //     Operation = operation, // Just a workflow step number
            //     Quantity = TransferQuantity,
            //     Notes = $"Transferred via Transfer Tab",
            //     User = _applicationState.CurrentUser.UserName,
            //     ItemType = SelectedInventoryItem.ItemType,
            //     BatchNumber = SelectedInventoryItem.BatchNumber,
            //     DateTime = DateTime.Now
            // };
            // await Dao_History.AddTransactionHistoryAsync(transaction);

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
            // TODO: Implement database loading
            // Load parts data
            // var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            //     Model_AppVariables.ConnectionString,
            //     "sys_parts_Get_All",
            //     new Dictionary<string, object>()
            // );
            
            // Load operations data
            // var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            //     Model_AppVariables.ConnectionString,
            //     "sys_operations_Get_All",
            //     new Dictionary<string, object>()
            // );

            // Load locations data
            // var locationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            //     Model_AppVariables.ConnectionString,
            //     "sys_locations_Get_All",
            //     new Dictionary<string, object>()
            // );

            await Task.Delay(200); // Simulate database operation
            LoadSampleData();

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
    public async Task TriggerSearchAsync()
    {
        if (SearchCommand != null)
        {
            await SearchCommand.Execute();
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
        _ = TriggerSearchAsync();
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
