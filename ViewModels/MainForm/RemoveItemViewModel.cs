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
/// ViewModel for the inventory removal interface (Control_RemoveTab).
/// Provides comprehensive functionality for removing inventory items from the system,
/// including search capabilities, batch deletion operations, undo functionality, 
/// and transaction history tracking.
/// </summary>
public class RemoveItemViewModel : BaseViewModel
{
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
    /// Current inventory items displayed in the DataGrid
    /// </summary>
    public ObservableCollection<InventoryItem> InventoryItems { get; } = new();
    
    /// <summary>
    /// Currently selected items in the DataGrid for batch operations
    /// </summary>
    public ObservableCollection<InventoryItem> SelectedItems { get; } = new();

    private InventoryItem? _selectedItem;
    /// <summary>
    /// Currently selected inventory item in the DataGrid
    /// </summary>
    public InventoryItem? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    #endregion

    #region Search Criteria Properties

    private string? _selectedPart;
    /// <summary>
    /// Selected part ID for filtering inventory
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

    #region State Properties

    private bool _isLoading;
    /// <summary>
    /// Indicates if a background operation is in progress
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _hasUndoItems;
    /// <summary>
    /// Indicates if there are items available for undo
    /// </summary>
    public bool HasUndoItems
    {
        get => _hasUndoItems;
        set => SetProperty(ref _hasUndoItems, value);
    }

    /// <summary>
    /// Indicates if there are inventory items to display
    /// </summary>
    public bool HasInventoryItems => InventoryItems.Count > 0;

    /// <summary>
    /// Indicates if delete operation can be performed (items selected)
    /// </summary>
    public bool CanDelete => SelectedItems.Count > 0 && !IsLoading;

    /// <summary>
    /// Indicates if undo operation is available
    /// </summary>
    public bool CanUndo => HasUndoItems && !IsLoading;

    #endregion

    #region Undo Functionality

    /// <summary>
    /// Stores items from the last removal operation for undo capability
    /// </summary>
    private readonly List<InventoryItem> _lastRemovedItems = new();

    #endregion

    #region Commands

    /// <summary>
    /// Executes inventory search based on selected criteria
    /// </summary>
    public ICommand SearchCommand { get; private set; } = null!;

    /// <summary>
    /// Resets search criteria and refreshes all data
    /// </summary>
    public ICommand ResetCommand { get; private set; } = null!;

    /// <summary>
    /// Removes selected inventory items with transaction logging
    /// </summary>
    public ICommand DeleteCommand { get; private set; } = null!;

    /// <summary>
    /// Restores last removed items
    /// </summary>
    public ICommand UndoCommand { get; private set; } = null!;

    /// <summary>
    /// Opens advanced removal features
    /// </summary>
    public ICommand AdvancedRemovalCommand { get; private set; } = null!;

    /// <summary>
    /// Prints current inventory view
    /// </summary>
    public ICommand PrintCommand { get; private set; } = null!;

    /// <summary>
    /// Toggles quick actions panel
    /// </summary>
    public ICommand TogglePanelCommand { get; private set; } = null!;

    /// <summary>
    /// Loads initial data including part and operation options
    /// </summary>
    public ICommand LoadDataCommand { get; private set; } = null!;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when items are successfully removed
    /// </summary>
    public event EventHandler<ItemsRemovedEventArgs>? ItemsRemoved;

    /// <summary>
    /// Event fired when panel toggle is requested
    /// </summary>
    public event EventHandler? PanelToggleRequested;

    /// <summary>
    /// Event fired when advanced removal is requested
    /// </summary>
    public event EventHandler? AdvancedRemovalRequested;

    #endregion

    #region Constructor

    public RemoveItemViewModel(
        IApplicationStateService applicationState,
        ILogger<RemoveItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("RemoveItemViewModel initialized with dependency injection");

        InitializeCommands();
        LoadSampleData(); // Load sample data for demonstration
        
        // Setup property change notifications for computed properties
        PropertyChanged += OnPropertyChanged;
    }
    
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Update computed properties when dependencies change
        switch (e.PropertyName)
        {
            case nameof(SelectedItems):
            case nameof(IsLoading):
                OnPropertyChanged(nameof(CanDelete));
                break;
            case nameof(HasUndoItems):
                OnPropertyChanged(nameof(CanUndo));
                break;
            case nameof(InventoryItems):
                OnPropertyChanged(nameof(HasInventoryItems));
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

    #region Command Initialization

    private void InitializeCommands()
    {
        // Search command with progress tracking
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);

        // Reset command
        ResetCommand = new AsyncCommand(ResetSearchAsync);

        // Delete command with validation
        DeleteCommand = new AsyncCommand(ExecuteDeleteAsync, () => CanDelete);

        // Undo command
        UndoCommand = new AsyncCommand(ExecuteUndoAsync, () => CanUndo);

        // Advanced removal command
        AdvancedRemovalCommand = new RelayCommand(() =>
        {
            AdvancedRemovalRequested?.Invoke(this, EventArgs.Empty);
        });

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

            // TODO: Implement database search operations
            // Dynamic search based on selection criteria
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

            Logger.LogInformation("Search executed for Part: {PartId}, Operation: {Operation}", 
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
            PartText = string.Empty;
            OperationText = string.Empty;
            InventoryItems.Clear();
            SelectedItem = null;

            // Reload all ComboBox data
            await LoadComboBoxDataAsync();

            Logger.LogInformation("Search criteria reset and data refreshed");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Batch deletes selected items with transaction logging
    /// </summary>
    private async Task ExecuteDeleteAsync()
    {
        if (SelectedItem == null)
        {
            Logger.LogWarning("Delete operation attempted with no item selected");
            return;
        }

        try
        {
            IsLoading = true;
            var itemsToRemove = new List<InventoryItem> { SelectedItem };

            // TODO: Implement database removal operations
            // var removeResult = await Dao_Inventory.RemoveInventoryItemsFromDataGridViewAsync(dgv, true);

            // TODO: Log transaction history for audit trail
            // foreach (var item in itemsToRemove)
            // {
            //     var transaction = new InventoryTransaction
            //     {
            //         TransactionType = TransactionType.OUT,
            //         PartId = item.PartId,
            //         Operation = item.Operation,
            //         Location = item.Location,
            //         Quantity = item.Quantity,
            //         User = Model_AppVariables.User,
            //         TransactionDateTime = DateTime.Now
            //     };
            //     await Dao_History.AddTransactionHistoryAsync(transaction);
            // }

            // Store for undo capability
            _lastRemovedItems.Clear();
            _lastRemovedItems.AddRange(itemsToRemove);
            HasUndoItems = _lastRemovedItems.Count > 0;

            // Remove from UI collections
            InventoryItems.Remove(SelectedItem);
            SelectedItem = null;

            // Fire event for integration
            ItemsRemoved?.Invoke(this, new ItemsRemovedEventArgs
            {
                RemovedItems = itemsToRemove,
                RemovalTime = DateTime.Now
            });

            Logger.LogInformation("Successfully removed {Count} inventory item", itemsToRemove.Count);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Restores last deleted items using undo functionality
    /// </summary>
    private async Task ExecuteUndoAsync()
    {
        if (_lastRemovedItems.Count == 0)
        {
            Logger.LogWarning("Undo operation attempted with no items to restore");
            return;
        }

        try
        {
            IsLoading = true;

            // TODO: Implement database restoration
            // foreach (var item in _lastRemovedItems)
            // {
            //     await Dao_Inventory.AddInventoryItemAsync(
            //         user: Model_AppVariables.User,
            //         partId: item.PartId,
            //         operation: item.Operation,
            //         location: item.Location,
            //         quantity: item.Quantity,
            //         notes: $"Restored via Undo: {item.Notes}"
            //     );
            // }

            // Restore to UI collections
            foreach (var item in _lastRemovedItems)
            {
                InventoryItems.Add(item);
            }

            Logger.LogInformation("Successfully restored {Count} inventory items via undo", 
                _lastRemovedItems.Count);

            _lastRemovedItems.Clear();
            HasUndoItems = _lastRemovedItems.Count > 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Prints current inventory view with formatted output
    /// </summary>
    private async Task ExecutePrintAsync()
    {
        try
        {
            IsLoading = true;

            // TODO: Implement print functionality using Core_DgvPrinter equivalent
            // var printer = new AvaloniaDataGridPrinter();
            // await printer.PrintDataGridAsync(InventoryItems, 
            //     title: "Inventory Removal Report",
            //     searchCriteria: $"Part: {SelectedPart}, Operation: {SelectedOperation}");

            Logger.LogInformation("Print operation initiated for {Count} inventory items", 
                InventoryItems.Count);

            await Task.Delay(1000); // Simulate print operation
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Loads ComboBox data from database
    /// </summary>
    private async Task LoadComboBoxDataAsync()
    {
        try
        {
            // TODO: Implement database loading
            // var partResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            //     Model_AppVariables.ConnectionString,
            //     "sys_parts_Get_All",
            //     new Dictionary<string, object>()
            // );
            
            // var operationResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            //     Model_AppVariables.ConnectionString,
            //     "sys_operations_Get_All", 
            //     new Dictionary<string, object>()
            // );

            await Task.Delay(200); // Simulate database operation
            LoadSampleData();

            Logger.LogInformation("ComboBox data loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load ComboBox data");
            throw;
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
    }

    /// <summary>
    /// Loads sample inventory data for demonstration
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
                Notes = "Ready for next operation",
                User = "TestUser",
                LastUpdated = DateTime.Now.AddHours(-2)
            },
            new InventoryItem
            {
                ID = 2,
                PartID = "PART001", 
                Operation = "110",
                Location = "WC02",
                Quantity = 15,
                Notes = "Quality check required",
                User = "TestUser",
                LastUpdated = DateTime.Now.AddHours(-1)
            },
            new InventoryItem
            {
                ID = 3,
                PartID = "PART002",
                Operation = "90",
                Location = "WC01",
                Quantity = 40,
                Notes = "Incoming from supplier",
                User = "TestUser",
                LastUpdated = DateTime.Now.AddMinutes(-30)
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
    /// Handles exceptions from command operations
    /// </summary>
    private void HandleException(Exception ex)
    {
        Logger.LogError(ex, "Error in RemoveItemViewModel operation");
        
        // TODO: Present user-friendly error message
        // await _errorService.LogErrorAsync(ex);
        // Show user notification with appropriate message
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
    /// Selects all visible inventory items
    /// </summary>
    public void SelectAllItems()
    {
        SelectedItems.Clear();
        foreach (var item in InventoryItems)
        {
            SelectedItems.Add(item);
        }
    }

    /// <summary>
    /// Clears all selected items
    /// </summary>
    public void ClearSelection()
    {
        SelectedItem = null;
        SelectedItems.Clear();
    }

    #endregion
}

#region Event Args

/// <summary>
/// Event arguments for items removed event
/// </summary>
public class ItemsRemovedEventArgs : EventArgs
{
    public List<InventoryItem> RemovedItems { get; set; } = new();
    public DateTime RemovalTime { get; set; }
    public int TotalQuantityRemoved => RemovedItems.Sum(item => item.Quantity);
}
#endregion
