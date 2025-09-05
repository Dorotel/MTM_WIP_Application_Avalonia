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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Threading;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// InventoryTabViewModel - Comprehensive inventory management interface using MVVM Community Toolkit
/// 
/// Provides comprehensive inventory management functionality including item entry, validation,
/// lookup data management, and integration with MTM business operations. Uses MVVM Community Toolkit
/// patterns for modern .NET development with source generators and optimized performance.
/// 
/// Key Features:
/// - MVVM Community Toolkit with [ObservableProperty] and [RelayCommand] source generators
/// - Centralized progress reporting via IApplicationStateService 
/// - MTM database integration with stored procedures
/// - Comprehensive input validation with user feedback
/// - Comprehensive input validation with user feedback
/// - Dependency injection with proper service lifetimes
/// </summary>
public partial class InventoryTabViewModel : BaseViewModel, IDisposable
{
    #region Private Fields & Services

    private readonly IApplicationStateService _applicationStateService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService? _configurationService;
    private readonly ISuggestionOverlayService _suggestionService;
    private readonly IMasterDataService _masterDataService;

    #endregion

    #region Observable Properties - Using MVVM Community Toolkit

    /// <summary>
    /// Currently selected part ID for inventory operations
    /// </summary>
    [ObservableProperty]
    private string _selectedPart = string.Empty;

    /// <summary>
    /// Currently selected operation number (e.g., "90", "100", "110")
    /// </summary>
    [ObservableProperty] 
    private string _selectedOperation = string.Empty;

    /// <summary>
    /// Currently selected location for inventory placement
    /// </summary>
    [ObservableProperty]
    private string _selectedLocation = string.Empty;

    /// <summary>
    /// Quantity of items for the inventory transaction
    /// </summary>
    [ObservableProperty]
    private int _quantity = 1;

    /// <summary>
    /// Optional notes for the inventory transaction
    /// </summary>
    [ObservableProperty]
    private string _notes = string.Empty;

    /// <summary>
    /// Application version text display
    /// </summary>
    [ObservableProperty]
    private string _versionText = "v1.0";

    /// <summary>
    /// Indicates if any loading operation is in progress
    /// </summary>
    [ObservableProperty]
    private bool _isLoading = false;

    /// <summary>
    /// Specific loading indicator for part IDs lookup
    /// </summary>
    [ObservableProperty]
    private bool _isLoadingParts = false;

    /// <summary>
    /// Specific loading indicator for operations lookup
    /// </summary>
    [ObservableProperty]
    private bool _isLoadingOperations = false;

    /// <summary>
    /// Specific loading indicator for locations lookup
    /// </summary>
    [ObservableProperty]
    private bool _isLoadingLocations = false;

    /// <summary>
    /// Current error message to display to user
    /// </summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Indicates if there is an active error condition
    /// </summary>
    [ObservableProperty]
    private bool _hasError = false;

    /// <summary>
    /// Text content for Part ID input field (two-way binding support)
    /// </summary>
    [ObservableProperty]
    private string _partText = string.Empty;

    /// <summary>
    /// Text content for Operation input field (two-way binding support)
    /// </summary>
    [ObservableProperty]
    private string _operationText = string.Empty;

    /// <summary>
    /// Text content for Location input field (two-way binding support)
    /// </summary>
    [ObservableProperty]
    private string _locationText = string.Empty;

    #endregion

    #region Property Change Notifications for Watermarks

    /// <summary>
    /// Called when SelectedPart property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedPartChanged(string value)
    {
        OnPropertyChanged(nameof(PartWatermark));
        OnPropertyChanged(nameof(IsPartValid));
    }

    /// <summary>
    /// Called when SelectedOperation property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedOperationChanged(string value)
    {
        OnPropertyChanged(nameof(OperationWatermark));
        OnPropertyChanged(nameof(IsOperationValid));
    }

    /// <summary>
    /// Called when SelectedLocation property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedLocationChanged(string value)
    {
        OnPropertyChanged(nameof(LocationWatermark));
        OnPropertyChanged(nameof(IsLocationValid));
    }

    /// <summary>
    /// Called when Quantity property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnQuantityChanged(int value)
    {
        OnPropertyChanged(nameof(QuantityWatermark));
        OnPropertyChanged(nameof(IsQuantityValid));
    }

    /// <summary>
    /// Called when Notes property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnNotesChanged(string value)
    {
        OnPropertyChanged(nameof(NotesWatermark));
        OnPropertyChanged(nameof(IsNotesValid));
    }

    #endregion

    #region Collections for Lookup Data - Using Centralized Master Data Service

    /// <summary>
    /// Available Part IDs from centralized master data service
    /// </summary>
    public ObservableCollection<string> PartIds => _masterDataService?.PartIds ?? new ObservableCollection<string>();

    /// <summary>
    /// Available Operations from centralized master data service
    /// </summary>
    public ObservableCollection<string> Operations => _masterDataService?.Operations ?? new ObservableCollection<string>();

    /// <summary>
    /// Available Locations from centralized master data service
    /// </summary>
    public ObservableCollection<string> Locations => _masterDataService?.Locations ?? new ObservableCollection<string>();

    #endregion

    #region Computed Properties

    /// <summary>
    /// Determines if the save operation can be executed based on current validation state
    /// </summary>
    public bool CanSave => !IsLoading && 
                          !string.IsNullOrWhiteSpace(SelectedPart) &&
                          !string.IsNullOrWhiteSpace(SelectedOperation) &&
                          !string.IsNullOrWhiteSpace(SelectedLocation) &&
                          Quantity > 0;

    /// <summary>
    /// Validation state for Part ID field
    /// </summary>
    public bool IsPartValid => !string.IsNullOrWhiteSpace(SelectedPart);

    /// <summary>
    /// Dynamic watermark for Part ID field - shows error or placeholder
    /// </summary>
    public string PartWatermark => IsPartValid ? "Enter or select a part number..." : "Part ID is required";

    /// <summary>
    /// Validation state for Operation field
    /// </summary>
    public bool IsOperationValid => !string.IsNullOrWhiteSpace(SelectedOperation);

    /// <summary>
    /// Dynamic watermark for Operation field - shows error or placeholder
    /// </summary>
    public string OperationWatermark => IsOperationValid ? "Enter or select an operation..." : "Operation is required";

    /// <summary>
    /// Validation state for Location field
    /// </summary>
    public bool IsLocationValid => !string.IsNullOrWhiteSpace(SelectedLocation);

    /// <summary>
    /// Dynamic watermark for Location field - shows error or placeholder
    /// </summary>
    public string LocationWatermark => IsLocationValid ? "Enter or select a location..." : "Location is required";

    /// <summary>
    /// Validation state for Quantity field
    /// </summary>
    public bool IsQuantityValid => Quantity > 0;

    /// <summary>
    /// Dynamic watermark for Quantity field - shows error or placeholder
    /// </summary>
    public string QuantityWatermark => IsQuantityValid ? "Enter quantity..." : "Quantity must be greater than 0";

    /// <summary>
    /// Validation state for Notes field
    /// </summary>
    public bool IsNotesValid => string.IsNullOrEmpty(Notes) || Notes.Length <= DatabaseConstraints.Notes_MaxLength;

    /// <summary>
    /// Dynamic watermark for Notes field - shows error or placeholder
    /// </summary>
    public string NotesWatermark => IsNotesValid ? "Optional notes or comments..." : $"Notes cannot exceed {DatabaseConstraints.Notes_MaxLength} characters";

    #endregion

    #region Events

    /// <summary>
    /// Event fired when inventory save operation completes successfully
    /// </summary>
    public event EventHandler<InventorySavedEventArgs>? SaveCompleted;

    #endregion

    #region Constructors

    /// <summary>
    /// Design-time constructor for XAML designer support
    /// </summary>
    public InventoryTabViewModel() : this(null!, null!, null!, null!, null!, null!)
    {
        // Design-time constructor - services will be null but handled gracefully
    }

    /// <summary>
    /// Primary constructor with dependency injection
    /// </summary>
    /// <param name="applicationStateService">Application state management service</param>
    /// <param name="navigationService">Navigation service for view management</param>
    /// <param name="databaseService">Database operations service</param>
    /// <param name="configurationService">Configuration management service</param>
    /// <param name="suggestionService">Suggestion overlay service</param>
    /// <param name="masterDataService">Master data service for shared reference data</param>
    public InventoryTabViewModel(
        IApplicationStateService applicationStateService,
        INavigationService navigationService, 
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ISuggestionOverlayService suggestionService,
        IMasterDataService masterDataService) : base()
    {
        // Validate required dependencies with descriptive error messages
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService), "Application state service is required for inventory management");
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService), "Navigation service is required for view management");
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService), "Database service is required for inventory operations");
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService), "Configuration service is required for database connectivity");
        _suggestionService = suggestionService ?? throw new ArgumentNullException(nameof(suggestionService), "Suggestion service is required for user experience");
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService), "Master data service is required for reference data");

        Logger.LogInformation("InventoryTabViewModel initialized with MVVM Community Toolkit patterns");
        Logger.LogInformation("Connection string configured: {HasConnectionString}", 
            !string.IsNullOrEmpty(_configurationService?.GetConnectionString()));
        
        // Initialize master data loading
        _ = Task.Run(async () => 
        {
            try
            {
                await _masterDataService.LoadAllMasterDataAsync();
                Logger.LogInformation("Master data loaded successfully for InventoryTabViewModel");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load master data in InventoryTabViewModel");
            }
        });
        
        // Database loading will be deferred until after UI is shown to prevent startup deadlocks
        Logger.LogInformation("InventoryTabViewModel constructor completed - database loading deferred");
    }

    #endregion

    #region MVVM Community Toolkit Commands

    /// <summary>
    /// Command to save the current inventory item to the database
    /// Includes comprehensive validation, progress reporting, and error handling
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            Logger.LogInformation("Saving inventory item: Part={PartId}, Operation={Operation}, Quantity={Quantity}", 
                SelectedPart, SelectedOperation, Quantity);

            // Update progress to MainView - following centralized progress pattern
            await _applicationStateService.SetProgressAsync(10, "Validating inventory data...");

            // Validate input using MTM database constraints
            if (!ValidateInput())
            {
                await _applicationStateService.SetProgressAsync(0, "Validation failed");
                return;
            }

            await _applicationStateService.SetProgressAsync(25, "Connecting to database...");

            // Prepare parameters for inv_inventory_Add_Item stored procedure
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = SelectedPart,
                ["p_Location"] = SelectedLocation,
                ["p_Operation"] = SelectedOperation,
                ["p_Quantity"] = Quantity,
                ["p_ItemType"] = "WIP", // Default item type for MTM
                ["p_User"] = _applicationStateService.CurrentUser,
                ["p_Notes"] = !string.IsNullOrWhiteSpace(Notes) ? Notes : DBNull.Value
            };

            await _applicationStateService.SetProgressAsync(50, "Saving inventory item...");

            // Execute stored procedure following MTM database patterns
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available"),
                "inv_inventory_Add_Item",
                parameters
            );

            if (result.IsSuccess)
            {
                await _applicationStateService.SetProgressAsync(75, "Processing transaction...");
                
                Logger.LogInformation("Inventory item saved successfully");
                
                // Generate batch number for session tracking
                var batchNumber = $"BATCH-{DateTime.Now:yyyyMMdd-HHmmss}";
                
                // Fire event to notify parent components
                SaveCompleted?.Invoke(this, new InventorySavedEventArgs
                {
                    PartId = SelectedPart,
                    Operation = SelectedOperation,
                    Quantity = Quantity,
                    Location = SelectedLocation,
                    Notes = Notes
                });
                
                await _applicationStateService.SetProgressAsync(100, "Inventory saved successfully");
                
                // Clear progress after user feedback delay
                await Task.Delay(1500);
                await _applicationStateService.ClearProgressAsync();
                
                // Reset form after successful save
                await ResetAsync();
                
                // Update application state with last used values
                _applicationStateService.CurrentOperation = SelectedOperation;
                _applicationStateService.CurrentLocation = SelectedLocation;
            }
            else
            {
                HasError = true;
                ErrorMessage = result.Message;
                Logger.LogWarning("Failed to save inventory item: {Error}", result.Message);
                
                await _applicationStateService.SetProgressAsync(0, $"Error: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = Services.ErrorHandling.GetUserFriendlyMessage(ex);
            await Services.ErrorHandling.HandleErrorAsync(ex, "SaveInventoryItem", _applicationStateService.CurrentUser,
                new Dictionary<string, object>
                {
                    ["PartId"] = SelectedPart,
                    ["Operation"] = SelectedOperation,
                    ["Quantity"] = Quantity,
                    ["Location"] = SelectedLocation
                });
                
            await _applicationStateService.SetProgressAsync(0, $"Database error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            // Notify that CanSave may have changed
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    /// <summary>
    /// Command to reset the form to initial state
    /// Preserves user experience by maintaining focus context
    /// </summary>
    [RelayCommand]
    private async Task ResetAsync()
    {
        try
        {
            SelectedPart = string.Empty;
            SelectedOperation = string.Empty;
            SelectedLocation = string.Empty;
            PartText = string.Empty;
            OperationText = string.Empty;
            LocationText = string.Empty;
            Quantity = 1;
            Notes = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;

            Logger.LogDebug("Form reset completed");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "ResetForm", _applicationStateService.CurrentUser);
        }
    }

    /// <summary>
    /// Command to open advanced entry dialog
    /// Future implementation for complex inventory operations
    /// </summary>
    [RelayCommand]
    private void AdvancedEntry()
    {
        try
        {
            Logger.LogInformation("Opening advanced entry dialog");
            // TODO: Implement advanced entry dialog navigation
        }
        catch (Exception ex)
        {
            _ = Services.ErrorHandling.HandleErrorAsync(ex, "AdvancedEntry", _applicationStateService.CurrentUser);
        }
    }

    /// <summary>
    /// Command to toggle panel visibility
    /// Future implementation for collapsible UI sections
    /// </summary>
    [RelayCommand]
    private void TogglePanel()
    {
        try
        {
            Logger.LogInformation("Toggling panel visibility");
            // TODO: Implement panel toggle functionality
        }
        catch (Exception ex)
        {
            _ = Services.ErrorHandling.HandleErrorAsync(ex, "TogglePanel", _applicationStateService.CurrentUser);
        }
    }

    /// <summary>
    /// Command to load lookup data from database
    /// Supports both initial load and refresh scenarios
    /// </summary>
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await LoadLookupDataAsync();
    }

    /// <summary>
    /// Command to refresh all lookup data
    /// Provides user-initiated data refresh capability
    /// </summary>
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            await LoadLookupDataAsync();
            Logger.LogInformation("Lookup data refreshed successfully");
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "RefreshLookupData", _applicationStateService.CurrentUser);
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
                _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available"),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (masterResult.IsSuccess && masterResult.Data.Rows.Count > 0)
            {
                Logger.LogInformation("Successfully retrieved {Count} rows from md_part_ids_Get_All", masterResult.Data.Rows.Count);
                
                // Log column names for debugging
                var columnNames = string.Join(", ", masterResult.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                Logger.LogInformation("Available columns: {Columns}", columnNames);
                
                Dispatcher.UIThread.Post(() =>
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
                    
                    Dispatcher.UIThread.Post(() =>
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
                Dispatcher.UIThread.Post(() =>
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
                Dispatcher.UIThread.Post(() =>
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

    private Task LoadFallbackPartIdsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            PartIds.Clear();
            var fallbackParts = new[] { "PART001", "PART002", "PART003", "ABC-123", "XYZ-789" };
            foreach (var part in fallbackParts)
            {
                PartIds.Add(part);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Part IDs", 5);
        return Task.CompletedTask;
    }

    private Task LoadFallbackOperationsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            Operations.Clear();
            var fallbackOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in fallbackOperations)
            {
                Operations.Add(operation);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Operations", 5);
        return Task.CompletedTask;
    }

    private Task LoadFallbackLocationsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            Locations.Clear();
            var fallbackLocations = new[] { "WC01", "WC02", "FLOOR", "QC", "SHIPPING" };
            foreach (var location in fallbackLocations)
            {
                Locations.Add(location);
            }
        });
        Logger.LogDebug("Loaded {Count} fallback Locations", 5);
        return Task.CompletedTask;
    }

    #endregion

    #region Validation

    /// <summary>
    /// MTM Database field constraints for validation
    /// </summary>
    private static class DatabaseConstraints
    {
        public const int PartID_MaxLength = 300;      // VARCHAR(300)
        public const int Location_MaxLength = 100;    // VARCHAR(100) 
        public const int Operation_MaxLength = 100;   // VARCHAR(100)
        public const int ItemType_MaxLength = 100;    // VARCHAR(100)
        public const int User_MaxLength = 100;        // VARCHAR(100)
        public const int Notes_MaxLength = 1000;      // VARCHAR(1000)
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(SelectedPart))
        {
            HasError = true;
            ErrorMessage = "Part ID is required";
            return false;
        }

        if (SelectedPart.Length > DatabaseConstraints.PartID_MaxLength)
        {
            HasError = true;
            ErrorMessage = $"Part ID cannot exceed {DatabaseConstraints.PartID_MaxLength} characters";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedOperation))
        {
            HasError = true;
            ErrorMessage = "Operation is required";
            return false;
        }

        if (SelectedOperation.Length > DatabaseConstraints.Operation_MaxLength)
        {
            HasError = true;
            ErrorMessage = $"Operation cannot exceed {DatabaseConstraints.Operation_MaxLength} characters";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLocation))
        {
            HasError = true;
            ErrorMessage = "Location is required";
            return false;
        }

        if (SelectedLocation.Length > DatabaseConstraints.Location_MaxLength)
        {
            HasError = true;
            ErrorMessage = $"Location cannot exceed {DatabaseConstraints.Location_MaxLength} characters";
            return false;
        }

        if (Quantity <= 0)
        {
            HasError = true;
            ErrorMessage = "Quantity must be greater than zero";
            return false;
        }

        if (!string.IsNullOrEmpty(Notes) && Notes.Length > DatabaseConstraints.Notes_MaxLength)
        {
            HasError = true;
            ErrorMessage = $"Notes cannot exceed {DatabaseConstraints.Notes_MaxLength} characters";
            return false;
        }

        // All validation passed - clear error state
        HasError = false;
        ErrorMessage = string.Empty;

        return true;
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Dispose of resources and cleanup subscriptions
    /// Following .NET disposal patterns for proper resource management
    /// </summary>
    /// <param name="disposing">True if disposing managed resources</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PartIds.Clear();
            Operations.Clear();
            Locations.Clear();
            Logger.LogDebug("InventoryTabViewModel disposed with MVVM Community Toolkit patterns");
        }
        base.Dispose(disposing);
    }

    #endregion
}
