using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// InventoryTabViewModel - Inventory Management Interface
/// 
/// Provides comprehensive inventory management functionality including item entry, validation,
/// lookup data management, and integration with MTM business operations. Uses standard .NET
/// patterns without ReactiveUI dependencies.
/// </summary>
public class InventoryTabViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IApplicationStateService _applicationStateService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    // Observable properties with backing fields
    private string _selectedPart = string.Empty;
    private string _selectedOperation = string.Empty;
    private string _selectedLocation = string.Empty;
    private int _quantity = 1;
    private string _notes = string.Empty;
    private string _versionText = "v1.0";
    private bool _isLoading = false;
    private bool _isLoadingParts = false;
    private bool _isLoadingOperations = false;
    private bool _isLoadingLocations = false;
    private string _errorMessage = string.Empty;
    private bool _hasError = false;

    // Collections for lookup data
    public ObservableCollection<string> PartIds { get; } = new();
    public ObservableCollection<string> Operations { get; } = new();
    public ObservableCollection<string> Locations { get; } = new();

    // Commands using standard ICommand interface
    public ICommand SaveCommand { get; private set; }
    public ICommand ResetCommand { get; private set; }
    public ICommand AdvancedEntryCommand { get; private set; }
    public ICommand TogglePanelCommand { get; private set; }
    public ICommand LoadDataCommand { get; private set; }
    public ICommand RefreshDataCommand { get; private set; }

    // Events for integration with other components
    public event EventHandler<InventorySavedEventArgs>? SaveCompleted;

    public InventoryTabViewModel() : this(null!, null!, null!, null!)
    {
        // Design-time constructor
    }

    public InventoryTabViewModel(
        IApplicationStateService applicationStateService,
        INavigationService navigationService, 
        IDatabaseService databaseService,
        IConfigurationService configurationService) : base()
    {
        _applicationStateService = applicationStateService;
        _navigationService = navigationService;
        _databaseService = databaseService;
        _configurationService = configurationService;

        InitializeCommands();
        
        Logger.LogInformation("InventoryTabViewModel initialized");
        Logger.LogInformation("Connection string configured: {HasConnectionString}", 
            !string.IsNullOrEmpty(_configurationService?.GetConnectionString()));
        
        // Load lookup data asynchronously on UI thread to avoid threading issues
        _ = Dispatcher.UIThread.InvokeAsync(async () => 
        {
            Logger.LogInformation("Starting lookup data initialization...");
            await InitializeLookupDataAsync();
            Logger.LogInformation("Lookup data initialization completed");
        });
    }

    #region Properties

    public string SelectedPart
    {
        get => _selectedPart;
        set => SetProperty(ref _selectedPart, value);
    }

    public string SelectedOperation
    {
        get => _selectedOperation;
        set => SetProperty(ref _selectedOperation, value);
    }

    public string SelectedLocation
    {
        get => _selectedLocation;
        set => SetProperty(ref _selectedLocation, value);
    }

    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    public string Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public string VersionText
    {
        get => _versionText;
        set => SetProperty(ref _versionText, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool IsLoadingParts
    {
        get => _isLoadingParts;
        set => SetProperty(ref _isLoadingParts, value);
    }

    public bool IsLoadingOperations
    {
        get => _isLoadingOperations;
        set => SetProperty(ref _isLoadingOperations, value);
    }

    public bool IsLoadingLocations
    {
        get => _isLoadingLocations;
        set => SetProperty(ref _isLoadingLocations, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    // Computed properties
    public bool CanSave => !IsLoading && 
                          !string.IsNullOrWhiteSpace(SelectedPart) &&
                          !string.IsNullOrWhiteSpace(SelectedOperation) &&
                          !string.IsNullOrWhiteSpace(SelectedLocation) &&
                          Quantity > 0;

    public bool IsPartValid => !string.IsNullOrWhiteSpace(SelectedPart);
    public bool IsOperationValid => !string.IsNullOrWhiteSpace(SelectedOperation);
    public bool IsLocationValid => !string.IsNullOrWhiteSpace(SelectedLocation);
    public bool IsQuantityValid => Quantity > 0;

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        SaveCommand = new AsyncCommand(ExecuteSaveAsync, () => CanSave);
        ResetCommand = new AsyncCommand(ExecuteResetAsync);
        AdvancedEntryCommand = new RelayCommand(ExecuteAdvancedEntry);
        TogglePanelCommand = new RelayCommand(ExecuteTogglePanel);
        LoadDataCommand = new AsyncCommand(LoadLookupDataAsync);
        RefreshDataCommand = new AsyncCommand(RefreshLookupDataAsync);
    }

    #endregion

    #region Command Implementations

    private async Task ExecuteSaveAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Logger.LogInformation("Saving inventory item: Part={PartId}, Operation={Operation}, Quantity={Quantity}", 
                SelectedPart, SelectedOperation, Quantity);

            // Validate input
            if (!ValidateInput())
            {
                return;
            }

            // Prepare parameters for inv_inventory_Add_Item stored procedure
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = SelectedPart,
                ["p_Location"] = SelectedLocation,
                ["p_Operation"] = SelectedOperation,
                ["p_Quantity"] = Quantity,
                ["p_ItemType"] = "WIP", // Default item type
                ["p_User"] = _applicationStateService.CurrentUser,
                ["p_Notes"] = !string.IsNullOrWhiteSpace(Notes) ? Notes : DBNull.Value
            };

            // Execute stored procedure for adding inventory
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _configurationService.GetConnectionString(),
                "inv_inventory_Add_Item",
                parameters
            );

            if (result.IsSuccess)
            {
                Logger.LogInformation("Inventory item saved successfully");
                
                // Fire event to notify parent that save was successful
                SaveCompleted?.Invoke(this, new InventorySavedEventArgs
                {
                    PartId = SelectedPart,
                    Operation = SelectedOperation,
                    Quantity = Quantity,
                    Location = SelectedLocation,
                    Notes = Notes
                });
                
                // Reset form after successful save
                await ExecuteResetAsync();
                
                // Update application state with last used values
                _applicationStateService.CurrentOperation = SelectedOperation;
                _applicationStateService.CurrentLocation = SelectedLocation;
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Message;
                Logger.LogWarning("Failed to save inventory item: {Error}", result.Message);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ErrorHandling.GetUserFriendlyMessage(ex);
            await ErrorHandling.HandleErrorAsync(ex, "SaveInventoryItem", _applicationStateService.CurrentUser,
                new Dictionary<string, object>
                {
                    ["PartId"] = SelectedPart,
                    ["Operation"] = SelectedOperation,
                    ["Quantity"] = Quantity,
                    ["Location"] = SelectedLocation
                });
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(CanSave));
        }
    }

    private async Task ExecuteResetAsync()
    {
        try
        {
            SelectedPart = string.Empty;
            SelectedOperation = string.Empty;
            SelectedLocation = string.Empty;
            Quantity = 1;
            Notes = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;

            Logger.LogDebug("Form reset completed");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "ResetForm", _applicationStateService.CurrentUser);
        }
    }

    private void ExecuteAdvancedEntry()
    {
        try
        {
            Logger.LogInformation("Opening advanced entry dialog");
            // TODO: Implement advanced entry dialog
        }
        catch (Exception ex)
        {
            _ = ErrorHandling.HandleErrorAsync(ex, "AdvancedEntry", _applicationStateService.CurrentUser);
        }
    }

    private void ExecuteTogglePanel()
    {
        try
        {
            Logger.LogInformation("Toggling panel visibility");
            // TODO: Implement panel toggle
        }
        catch (Exception ex)
        {
            _ = ErrorHandling.HandleErrorAsync(ex, "TogglePanel", _applicationStateService.CurrentUser);
        }
    }

    private async Task RefreshLookupDataAsync()
    {
        try
        {
            await LoadLookupDataAsync();
            Logger.LogInformation("Lookup data refreshed successfully");
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "RefreshLookupData", _applicationStateService.CurrentUser);
        }
    }

    #endregion

    #region Data Loading

    /// <summary>
    /// Initialize lookup data on startup
    /// </summary>
    private async Task InitializeLookupDataAsync()
    {
        try
        {
            // Test database connection first
            Logger.LogInformation("Testing database connection...");
            var isConnected = await _databaseService.TestConnectionAsync();
            
            if (!isConnected)
            {
                Logger.LogWarning("Database connection test failed, using fallback data");
                await LoadFallbackDataAsync();
                return;
            }
            
            Logger.LogInformation("Database connection successful, loading real data");
            await LoadLookupDataAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to initialize lookup data, using fallback data");
            await LoadFallbackDataAsync();
        }
    }

    /// <summary>
    /// Load all lookup data from database
    /// </summary>
    public async Task LoadLookupDataAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;

            // Load all lookup data in parallel
            var partTask = LoadPartIdsAsync();
            var operationTask = LoadOperationsAsync();
            var locationTask = LoadLocationsAsync();

            await Task.WhenAll(partTask, operationTask, locationTask);

            Logger.LogInformation("All lookup data loaded successfully");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ErrorHandling.GetUserFriendlyMessage(ex);
            await ErrorHandling.HandleErrorAsync(ex, "LoadLookupData", _applicationStateService.CurrentUser);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Load Part IDs from md_part_ids table
    /// </summary>
    private async Task LoadPartIdsAsync()
    {
        try
        {
            IsLoadingParts = true;
            
            Logger.LogInformation("Loading Part IDs from database...");
            
            // First try to get parts directly from master data table using the correct stored procedure
            var masterResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _configurationService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (masterResult.IsSuccess && masterResult.Data.Rows.Count > 0)
            {
                Logger.LogInformation("Successfully retrieved {Count} rows from md_part_ids_Get_All", masterResult.Data.Rows.Count);
                
                // Log column names for debugging
                var columnNames = string.Join(", ", masterResult.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                Logger.LogInformation("Available columns: {Columns}", columnNames);
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PartIds.Clear();
                    foreach (DataRow row in masterResult.Data.Rows)
                    {
                        // Try different possible column names based on the table structure
                        string? partId = null;
                        
                        if (masterResult.Data.Columns.Contains("PartID"))
                        {
                            partId = row["PartID"]?.ToString();
                        }
                        else if (masterResult.Data.Columns.Contains("partid"))
                        {
                            partId = row["partid"]?.ToString();
                        }
                        else if (masterResult.Data.Columns.Contains("part_id"))
                        {
                            partId = row["part_id"]?.ToString();
                        }
                        else if (masterResult.Data.Columns.Contains("ItemNumber"))
                        {
                            partId = row["ItemNumber"]?.ToString();
                        }
                        else if (masterResult.Data.Columns.Contains("item_number"))
                        {
                            partId = row["item_number"]?.ToString();
                        }
                        else
                        {
                            // If we can't find expected columns, use the first non-ID column
                            var firstDataColumn = masterResult.Data.Columns.Cast<DataColumn>()
                                .FirstOrDefault(c => !c.ColumnName.Equals("ID", StringComparison.OrdinalIgnoreCase));
                            if (firstDataColumn != null)
                            {
                                partId = row[firstDataColumn.ColumnName]?.ToString();
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartIds.Add(partId);
                            Logger.LogDebug("Added Part ID: {PartId}", partId);
                        }
                    }
                });
                
                Logger.LogInformation("Loaded {Count} Part IDs from master data table", PartIds.Count);
                
                // Log first few part IDs for verification
                if (PartIds.Count > 0)
                {
                    var firstFew = string.Join(", ", PartIds.Take(5));
                    Logger.LogInformation("First few Part IDs: {PartIds}", firstFew);
                }
            }
            else
            {
                Logger.LogWarning("md_part_ids_Get_All returned no data. Status: {Status}, Message: {Message}", 
                    masterResult.Status, masterResult.Message);
                
                // Try getting parts from inventory table as fallback
                Logger.LogInformation("Trying to get unique parts from inventory table...");
                
                var inventoryResult = await _databaseService.GetAllPartIDsAsync();
                
                if (inventoryResult != null && inventoryResult.Rows.Count > 0)
                {
                    Logger.LogInformation("Retrieved {Count} rows from inventory", inventoryResult.Rows.Count);
                    
                    // Log inventory column names for debugging
                    var invColumnNames = string.Join(", ", inventoryResult.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    Logger.LogInformation("Inventory columns: {Columns}", invColumnNames);
                    
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        PartIds.Clear();
                        var uniqueParts = new HashSet<string>();
                        
                        foreach (DataRow row in inventoryResult.Rows)
                        {
                            // Try different possible column names for part ID in inventory
                            string? partId = null;
                            
                            if (inventoryResult.Columns.Contains("PartID"))
                            {
                                partId = row["PartID"]?.ToString();
                            }
                            else if (inventoryResult.Columns.Contains("partid"))
                            {
                                partId = row["partid"]?.ToString();
                            }
                            else if (inventoryResult.Columns.Contains("part_id"))
                            {
                                partId = row["part_id"]?.ToString();
                            }
                            
                            if (!string.IsNullOrEmpty(partId) && uniqueParts.Add(partId))
                            {
                                PartIds.Add(partId);
                            }
                        }
                    });
                    
                    Logger.LogInformation("Loaded {Count} unique Part IDs from inventory table", PartIds.Count);
                }
                else
                {
                    Logger.LogWarning("No data found in inventory table either, loading fallback data");
                    await LoadFallbackPartIdsAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Part IDs from database");
            await LoadFallbackPartIdsAsync();
        }
        finally
        {
            IsLoadingParts = false;
        }
    }

    /// <summary>
    /// Load Operations from md_operation_numbers table
    /// </summary>
    private async Task LoadOperationsAsync()
    {
        try
        {
            IsLoadingOperations = true;
            
            Logger.LogInformation("Loading Operations from database...");
            
            // Load Operations using DatabaseService
            var result = await _databaseService.GetAllOperationsAsync();

            if (result != null && result.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Operations.Clear();
                    foreach (DataRow row in result.Rows)
                    {
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            Operations.Add(operation);
                        }
                    }
                });
                
                Logger.LogInformation("Loaded {Count} Operations from database", Operations.Count);
            }
            else
            {
                Logger.LogWarning("No operations found in database, loading fallback data");
                await LoadFallbackOperationsAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Operations from database");
            await LoadFallbackOperationsAsync();
        }
        finally
        {
            IsLoadingOperations = false;
        }
    }

    /// <summary>
    /// Load Locations from md_locations table
    /// </summary>
    private async Task LoadLocationsAsync()
    {
        try
        {
            IsLoadingLocations = true;
            
            Logger.LogInformation("Loading Locations from database...");
            
            // Load Locations using DatabaseService
            var result = await _databaseService.GetAllLocationsAsync();

            if (result != null && result.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Locations.Clear();
                    foreach (DataRow row in result.Rows)
                    {
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            Locations.Add(location);
                        }
                    }
                });
                
                Logger.LogInformation("Loaded {Count} Locations from database", Locations.Count);
            }
            else
            {
                Logger.LogWarning("No locations found in database, loading fallback data");
                await LoadFallbackLocationsAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Locations from database");
            await LoadFallbackLocationsAsync();
        }
        finally
        {
            IsLoadingLocations = false;
        }
    }

    #endregion

    #region Fallback Data Methods

    /// <summary>
    /// Load all fallback data when database is unavailable
    /// </summary>
    private async Task LoadFallbackDataAsync()
    {
        await Task.WhenAll(
            LoadFallbackPartIdsAsync(),
            LoadFallbackOperationsAsync(),
            LoadFallbackLocationsAsync()
        );
        Logger.LogInformation("Fallback data loaded for all AutoComplete boxes");
    }

    private async Task LoadFallbackPartIdsAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            PartIds.Clear();
            var fallbackParts = new[] { "PART001", "PART002", "PART003", "ABC-123", "XYZ-789" };
            foreach (var part in fallbackParts)
            {
                PartIds.Add(part);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Part IDs", PartIds.Count);
    }

    private async Task LoadFallbackOperationsAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Operations.Clear();
            var fallbackOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in fallbackOperations)
            {
                Operations.Add(operation);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Operations", Operations.Count);
    }

    private async Task LoadFallbackLocationsAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            Locations.Clear();
            var fallbackLocations = new[] { "WC01", "WC02", "FLOOR", "QC", "SHIPPING" };
            foreach (var location in fallbackLocations)
            {
                Locations.Add(location);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Locations", Locations.Count);
    }

    #endregion

    #region Validation

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(SelectedPart))
        {
            HasError = true;
            ErrorMessage = "Part ID is required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedOperation))
        {
            HasError = true;
            ErrorMessage = "Operation is required";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLocation))
        {
            HasError = true;
            ErrorMessage = "Location is required";
            return false;
        }

        if (Quantity <= 0)
        {
            HasError = true;
            ErrorMessage = "Quantity must be greater than zero";
            return false;
        }

        return true;
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        
        // Update computed properties
        if (propertyName is nameof(SelectedPart) or nameof(SelectedOperation) or 
            nameof(SelectedLocation) or nameof(Quantity) or nameof(IsLoading))
        {
            OnPropertyChanged(nameof(CanSave));
        }
        
        if (propertyName == nameof(SelectedPart))
        {
            OnPropertyChanged(nameof(IsPartValid));
        }
        
        if (propertyName == nameof(SelectedOperation))
        {
            OnPropertyChanged(nameof(IsOperationValid));
        }
        
        if (propertyName == nameof(SelectedLocation))
        {
            OnPropertyChanged(nameof(IsLocationValid));
        }
        
        if (propertyName == nameof(Quantity))
        {
            OnPropertyChanged(nameof(IsQuantityValid));
        }
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// Debug method to test Part ID loading directly
    /// Call this method to diagnose Part ID loading issues
    /// </summary>
    public async Task DebugPartIdLoadingAsync()
    {
        try
        {
            Logger.LogInformation("=== DEBUG: Testing Part ID Loading ===");
            
            // Test 1: Direct database connection
            Logger.LogInformation("Test 1: Testing database connection...");
            var isConnected = await _databaseService.TestConnectionAsync();
            Logger.LogInformation("Database connection test result: {IsConnected}", isConnected);
            
            if (!isConnected)
            {
                Logger.LogError("Database connection failed - this is why Part IDs are not loading!");
                return;
            }

            // Test 2: Direct stored procedure call
            Logger.LogInformation("Test 2: Testing md_part_ids_Get_All stored procedure...");
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _configurationService.GetConnectionString(),
                "md_part_ids_Get_All",
                new Dictionary<string, object>
                {
                }
            );

            Logger.LogInformation("Stored procedure result - Status: {Status}, Message: {Message}, Rows: {RowCount}", 
                result.Status, result.Message, result.Data?.Rows.Count ?? 0);

            if (!result.IsSuccess)
            {
                Logger.LogError("Stored procedure failed: {Message}", result.Message);
                return;
            }

            if (result.Data == null || result.Data.Rows.Count == 0)
            {
                Logger.LogWarning("Stored procedure returned no data - the md_part_ids table might be empty!");
                return;
            }

            // Test 3: Examine the data structure
            Logger.LogInformation("Test 3: Examining returned data structure...");
            var columnNames = string.Join(", ", result.Data.Columns.Cast<DataColumn>().Select(c => $"{c.ColumnName}({c.DataType.Name})"));
            Logger.LogInformation("Columns: {Columns}", columnNames);

            // Log first few rows
            Logger.LogInformation("First few rows of data:");
            for (int i = 0; i < Math.Min(5, result.Data.Rows.Count); i++)
            {
                var row = result.Data.Rows[i];
                var rowData = string.Join(", ", result.Data.Columns.Cast<DataColumn>()
                    .Select(c => $"{c.ColumnName}={row[c.ColumnName] ?? "NULL"}"));
                Logger.LogInformation("Row {Index}: {RowData}", i + 1, rowData);
            }

            // Test 4: Try to extract Part IDs using our logic
            Logger.LogInformation("Test 4: Testing Part ID extraction logic...");
            var extractedParts = new List<string>();
            
            foreach (DataRow row in result.Data.Rows)
            {
                string? partId = null;
                
                if (result.Data.Columns.Contains("PartID"))
                {
                    partId = row["PartID"]?.ToString();
                    Logger.LogDebug("Found PartID column, value: {Value}", partId ?? "NULL");
                }
                else if (result.Data.Columns.Contains("partid"))
                {
                    partId = row["partid"]?.ToString();
                    Logger.LogDebug("Found partid column, value: {Value}", partId ?? "NULL");
                }
                else
                {
                    // Try first non-ID column
                    var firstDataColumn = result.Data.Columns.Cast<DataColumn>()
                        .FirstOrDefault(c => !c.ColumnName.Equals("ID", StringComparison.OrdinalIgnoreCase));
                    if (firstDataColumn != null)
                    {
                        partId = row[firstDataColumn.ColumnName]?.ToString();
                        Logger.LogDebug("Using first non-ID column {Column}, value: {Value}", firstDataColumn.ColumnName, partId ?? "NULL");
                    }
                }
                
                if (!string.IsNullOrEmpty(partId))
                {
                    extractedParts.Add(partId);
                }
            }

            Logger.LogInformation("Successfully extracted {Count} Part IDs: {PartIds}", 
                extractedParts.Count, string.Join(", ", extractedParts.Take(10)));

            // Test 5: Update the actual collection
            Logger.LogInformation("Test 5: Updating PartIds collection...");
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                PartIds.Clear();
                foreach (var part in extractedParts)
                {
                    PartIds.Add(part);
                }
            });

            Logger.LogInformation("=== DEBUG COMPLETE: Part IDs collection now has {Count} items ===", PartIds.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "DEBUG: Exception during Part ID loading test");
        }
    }

    #endregion
}

#region Command Implementations

/// <summary>
/// Simple relay command implementation for synchronous operations
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Async command implementation for asynchronous operations
/// </summary>
public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

#endregion
