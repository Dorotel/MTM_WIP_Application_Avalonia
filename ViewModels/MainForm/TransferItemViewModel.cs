using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels;

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

    [ObservableProperty]
    private bool _isLoading;

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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
            InventoryItems.Clear();
            SelectedInventoryItem = null;

            _logger.LogInformation("Executing transfer search for Part: {PartId}, Operation: {Operation}", 
                SelectedPart, SelectedOperation);

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

            _logger.LogInformation("Transfer search completed. Found {Count} inventory items", InventoryItems.Count);
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
            await LoadComboBoxDataAsync().ConfigureAwait(false);

            _logger.LogInformation("Transfer search criteria reset and data refreshed");
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
        if (SelectedInventoryItem == null || string.IsNullOrWhiteSpace(SelectedToLocation))
        {
            _logger.LogWarning("Transfer operation attempted with invalid selection");
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
                _logger.LogWarning("Transfer attempted to same location: {Location}", fromLocation);
                // TODO: Show user-friendly error message
                return;
            }

            // Validate transfer quantity against available inventory
            if (TransferQuantity > SelectedInventoryItem.Quantity)
            {
                _logger.LogWarning("Transfer quantity {TransferQuantity} exceeds available {Available}", 
                    TransferQuantity, SelectedInventoryItem.Quantity);
                // TODO: Show user-friendly error message
                return;
            }

            // Implement actual database transfer operations
            bool transferResult;
            
            if (TransferQuantity < SelectedInventoryItem.Quantity)
            {
                // Partial quantity transfer
                _logger.LogInformation("Executing partial transfer: {TransferQuantity} of {TotalQuantity} units", 
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
                _logger.LogInformation("Executing complete transfer: {TransferQuantity} units", TransferQuantity);
                
                transferResult = await _databaseService.TransferPartAsync(
                    SelectedInventoryItem.BatchNumber ?? string.Empty,
                    partId,
                    operation,
                    SelectedToLocation
                );
            }

            if (!transferResult)
            {
                _logger.LogError("Transfer operation failed");
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

            _logger.LogInformation("Successfully transferred {Quantity} units of {PartId} from {FromLocation} to {ToLocation}", 
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
    [RelayCommand]
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

            _logger.LogInformation("Print operation initiated for transfer inventory view with {Count} items", 
                InventoryItems.Count);

            await Task.Delay(1000).ConfigureAwait(false); // Simulate print operation
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
        else
        {
            MaxTransferQuantity = 0;
            TransferQuantity = 1;
        }
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles exceptions from command operations with comprehensive logging
    /// </summary>
    private void HandleException(Exception ex)
    {
        _logger.LogError(ex, "Error in TransferItemViewModel operation");
        
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
