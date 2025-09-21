using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.Services.Infrastructure;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Business;
using MTM_WIP_Application_Avalonia.Services.Feature;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models.Events;
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
    private readonly ISuccessOverlayService? _successOverlayService;
    private readonly IUniversalOverlayService _universalOverlayService;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when the success overlay should be shown
    /// </summary>
    public event EventHandler<SuccessEventArgs>? ShowSuccessOverlay;

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
    private int _quantity = 0;  // Changed from 1 to 0 (blank/empty equivalent)

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

    /// <summary>
    /// Text content for Quantity input field (two-way binding support)
    /// </summary>
    [ObservableProperty]
    private string _quantityText = string.Empty;

    #endregion

    #region Property Change Notifications for Watermarks

    /// <summary>
    /// Called when SelectedPart property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedPartChanged(string value)
    {
        OnPropertyChanged(nameof(PartWatermark));
        OnPropertyChanged(nameof(IsPartValid));
        OnPropertyChanged(nameof(IsPartValidInDatabase));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when SelectedOperation property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedOperationChanged(string value)
    {
        OnPropertyChanged(nameof(OperationWatermark));
        OnPropertyChanged(nameof(IsOperationValid));
        OnPropertyChanged(nameof(IsOperationValidInDatabase));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when SelectedLocation property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnSelectedLocationChanged(string value)
    {
        OnPropertyChanged(nameof(LocationWatermark));
        OnPropertyChanged(nameof(IsLocationValid));
        OnPropertyChanged(nameof(IsLocationValidInDatabase));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when Quantity property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnQuantityChanged(int value)
    {
        OnPropertyChanged(nameof(QuantityWatermark));
        OnPropertyChanged(nameof(IsQuantityValid));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when Notes property changes - triggers watermark and validation updates
    /// </summary>
    partial void OnNotesChanged(string value)
    {
        OnPropertyChanged(nameof(NotesWatermark));
        OnPropertyChanged(nameof(IsNotesValid));
    }

    /// <summary>
    /// Called when IsLoading property changes - triggers CanSave validation updates
    /// </summary>
    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Called when QuantityText property changes - handles text validation and conversion
    /// </summary>
    partial void OnQuantityTextChanged(string value)
    {
        // Convert text to integer, handling empty/invalid inputs
        if (string.IsNullOrWhiteSpace(value))
        {
            Quantity = 0; // 0 represents empty/invalid state
        }
        else if (int.TryParse(value, out int quantity) && quantity > 0)
        {
            Quantity = quantity;
        }
        else
        {
            Quantity = 0; // Invalid input - set to 0 to trigger validation error
        }
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
    /// Requires all fields to have valid values from the database and quantity to be positive
    /// </summary>
    public bool CanSave => !IsLoading &&
                          IsPartValidInDatabase &&
                          IsOperationValidInDatabase &&
                          IsLocationValidInDatabase &&
                          IsQuantityValid;

    /// <summary>
    /// Validation state for Part ID field - checks if not empty AND exists in database
    /// </summary>
    public bool IsPartValid => !string.IsNullOrWhiteSpace(SelectedPart);

    /// <summary>
    /// Validation state for Part ID field - checks if value exists in database
    /// </summary>
    public bool IsPartValidInDatabase => IsPartValid &&
                                        _masterDataService?.PartIds?.Contains(SelectedPart) == true;

    /// <summary>
    /// Dynamic watermark for Part ID field - shows error or placeholder
    /// </summary>
    public string PartWatermark => !IsPartValid ? "Part ID is required" :
                                  !IsPartValidInDatabase ? "Part ID not found in database" :
                                  "Enter or select a part number...";

    /// <summary>
    /// Validation state for Operation field - checks if not empty and valid manufacturing operation number
    /// Valid operations are numeric strings like "90", "100", "110", "120", etc.
    /// </summary>
    public bool IsOperationValid => !string.IsNullOrWhiteSpace(SelectedOperation) &&
                                   IsValidManufacturingOperation(SelectedOperation);

    /// <summary>
    /// Validates if an operation number follows MTM manufacturing standards
    /// </summary>
    private bool IsValidManufacturingOperation(string operation)
    {
        // Must be numeric and greater than 0
        if (int.TryParse(operation, out int operationNumber))
        {
            return operationNumber > 0;
        }
        return false;
    }

    /// <summary>
    /// Validation state for Operation field - checks if value exists in database
    /// </summary>
    public bool IsOperationValidInDatabase => IsOperationValid &&
                                             _masterDataService?.Operations?.Contains(SelectedOperation) == true;

    /// <summary>
    /// Dynamic watermark for Operation field - shows error or placeholder
    /// </summary>
    public string OperationWatermark => !IsOperationValid ? "Operation is required" :
                                       !IsOperationValidInDatabase ? "Operation not found in database" :
                                       "Enter or select an operation...";

    /// <summary>
    /// Validation state for Location field - checks if not empty
    /// </summary>
    public bool IsLocationValid => !string.IsNullOrWhiteSpace(SelectedLocation);

    /// <summary>
    /// Validation state for Location field - checks if value exists in database
    /// </summary>
    public bool IsLocationValidInDatabase => IsLocationValid &&
                                            _masterDataService?.Locations?.Contains(SelectedLocation) == true;

    /// <summary>
    /// Dynamic watermark for Location field - shows error or placeholder
    /// </summary>
    public string LocationWatermark => !IsLocationValid ? "Location is required" :
                                      !IsLocationValidInDatabase ? "Location not found in database" :
                                      "Enter or select a location...";

    /// <summary>
    /// Validation state for Quantity field
    /// </summary>
    public bool IsQuantityValid => Quantity > 0;

    /// <summary>
    /// Dynamic watermark for Quantity field - shows error or placeholder
    /// </summary>
    public string QuantityWatermark => IsQuantityValid ? "Enter quantity..." : "Quantity is required and must be greater than 0";

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

    /// <summary>
    /// Event fired when advanced entry is requested
    /// </summary>
    public event EventHandler? AdvancedEntryRequested;

    /// <summary>
    /// Event fired when LostFocus should be triggered on form fields for validation
    /// </summary>
    public event EventHandler? TriggerValidationLostFocus;

    #endregion

    #region Constructors

    /// <summary>
    /// Design-time constructor for XAML designer support
    /// </summary>
    public InventoryTabViewModel() : this(null!, null!, null!, null!, null!, null!, null!, null!)
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
    /// <param name="successOverlayService">Success overlay service for user feedback</param>
    /// <param name="universalOverlayService">Universal overlay service for loading and error overlays</param>
    public InventoryTabViewModel(
        IApplicationStateService applicationStateService,
        INavigationService navigationService,
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ISuggestionOverlayService suggestionService,
        IMasterDataService masterDataService,
        ISuccessOverlayService? successOverlayService,
        IUniversalOverlayService universalOverlayService) : base()
    {
        // Validate required dependencies with descriptive error messages
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService), "Application state service is required for inventory management");
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService), "Navigation service is required for view management");
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService), "Database service is required for inventory operations");
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService), "Configuration service is required for database connectivity");
        _suggestionService = suggestionService ?? throw new ArgumentNullException(nameof(suggestionService), "Suggestion service is required for user experience");
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService), "Master data service is required for reference data");
        _successOverlayService = successOverlayService; // Optional service - can be null
        _universalOverlayService = universalOverlayService ?? throw new ArgumentNullException(nameof(universalOverlayService), "Universal overlay service is required for modern UX");

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

                // Update database validation states after master data is loaded
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    OnPropertyChanged(nameof(IsPartValidInDatabase));
                    OnPropertyChanged(nameof(IsOperationValidInDatabase));
                    OnPropertyChanged(nameof(IsLocationValidInDatabase));
                    OnPropertyChanged(nameof(PartWatermark));
                    OnPropertyChanged(nameof(OperationWatermark));
                    OnPropertyChanged(nameof(LocationWatermark));
                    OnPropertyChanged(nameof(CanSave));
                    SaveCommand.NotifyCanExecuteChanged();
                });
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
    /// Enhanced with Universal Overlay Service for better UX
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            // Enhanced overlay integration for better UX
            IsLoading = true;

            Logger.LogInformation("Saving inventory item: Part={PartId}, Operation={Operation}, Quantity={Quantity}",
                SelectedPart, SelectedOperation, Quantity);

            // Update progress to MainView - following centralized progress pattern
            await _applicationStateService.SetProgressAsync(10, "Validating inventory data...");

            // Validate input using MTM database constraints
            if (!ValidateInput())
            {
                await _applicationStateService.SetProgressAsync(0, "Validation failed");
                Logger.LogWarning("Form validation failed - Part: '{Part}', Operation: '{Operation}', Location: '{Location}', Quantity: {Quantity}",
                    SelectedPart, SelectedOperation, SelectedLocation, Quantity);

                // Show validation error overlay
                if (_successOverlayService != null)
                {
                    await _successOverlayService.ShowErrorOverlayInMainViewAsync(
                        null,
                        "Validation Failed",
                        "Please check all required fields and ensure data is valid",
                        "AlertCircle",
                        3000
                    );
                }
                return;
            }

            await _applicationStateService.SetProgressAsync(25, "Connecting to database...");

            // Get current user - ensure it's not null or empty
            var currentUser = _applicationStateService.CurrentUser;
            if (string.IsNullOrWhiteSpace(currentUser))
            {
                currentUser = Environment.UserName.ToUpper();
                Logger.LogWarning("CurrentUser was empty, using Environment.UserName: {User}", currentUser);
            }

            await _applicationStateService.SetProgressAsync(50, "Saving inventory item...");

            // Generate unique batch number for inventory tracking (following WinForms pattern)
            var batchNumber = await GenerateUniqueBatchNumberAsync();

            Logger.LogInformation("Attempting to save with values - Part: '{PartId}', Operation: '{Operation}', Location: '{Location}', Quantity: {Quantity}, User: '{User}', Batch: '{BatchNumber}'",
                SelectedPart, SelectedOperation ?? "NULL", SelectedLocation ?? "NULL", Quantity, currentUser, batchNumber);

            // Use DatabaseService method for proper parameter validation and error handling
            var result = await _databaseService.AddInventoryItemAsync(
                partId: SelectedPart,
                location: SelectedLocation ?? string.Empty,
                operation: SelectedOperation ?? string.Empty,
                quantity: Quantity,
                itemType: "WIP", // Default item type for MTM
                user: currentUser,
                batchNumber: batchNumber, // Include batch number for tracking
                notes: Notes ?? string.Empty
            );

            if (result.IsSuccess)
            {
                await _applicationStateService.SetProgressAsync(75, "Processing transaction...");

                Logger.LogInformation("Inventory item saved successfully with batch number: {BatchNumber}", batchNumber);

                // Show success overlay with enhanced details
                var successDetails = $"Part: {SelectedPart} | Operation: {SelectedOperation ?? "N/A"} | Quantity: {Quantity} | Location: {SelectedLocation ?? "N/A"}";

                // Use Success Overlay Service if available for better UX
                if (_successOverlayService != null)
                {
                    await _successOverlayService.ShowSuccessOverlayInMainViewAsync(
                        null,
                        "Inventory Saved Successfully",
                        successDetails,
                        "CheckCircle",
                        4000
                    );
                }
                else
                {
                    // Fallback to event-based approach
                    var successArgs = new SuccessEventArgs
                    {
                        Message = "Inventory item saved successfully!",
                        Details = successDetails,
                        IconKind = "CheckCircle",
                        Duration = 4000, // 4 seconds: form resets immediately, overlay continues for additional time
                        SuccessTime = DateTime.Now
                    };

                    Logger.LogInformation("[SUCCESS EVENT] About to fire ShowSuccessOverlay event. Event null? {EventNull}, ViewModel HashCode: {ViewModelHash}, Subscribers: {SubscriberCount}",
                        ShowSuccessOverlay == null,
                        GetHashCode(),
                        ShowSuccessOverlay?.GetInvocationList()?.Length ?? 0);

                    ShowSuccessOverlay?.Invoke(this, successArgs);
                }

                Logger.LogInformation("[SUCCESS EVENT] ShowSuccessOverlay event fired successfully");

                // Fire event to notify parent components (following WinForms pattern)
                SaveCompleted?.Invoke(this, new InventorySavedEventArgs
                {
                    PartId = SelectedPart,
                    Operation = SelectedOperation ?? string.Empty,
                    Quantity = Quantity,
                    Location = SelectedLocation ?? string.Empty,
                    Notes = Notes ?? string.Empty
                });

                await _applicationStateService.SetProgressAsync(100, "Inventory saved successfully");

                // Clear progress immediately
                await _applicationStateService.ClearProgressAsync();

                // Reset form immediately after overlay starts - overlay will continue running independently
                await ResetAsync();

                // Update application state with last used values
                _applicationStateService.CurrentOperation = SelectedOperation ?? string.Empty;
                _applicationStateService.CurrentLocation = SelectedLocation ?? string.Empty;
            }
            else
            {
                Logger.LogError("Failed to save inventory item: Status={Status}, Message='{Message}', Parameters: Part='{Part}', Operation='{Operation}', Location='{Location}', Quantity={Quantity}, User='{User}'",
                    result.Status, result.Message, SelectedPart, SelectedOperation, SelectedLocation, Quantity, currentUser);

                // Show error overlay with enhanced UX
                if (_successOverlayService != null)
                {
                    await _successOverlayService.ShowErrorOverlayInMainViewAsync(
                        null,
                        "Save Failed",
                        result.Message ?? "Database operation failed. Please check your data and try again.",
                        "AlertCircle",
                        5000 // Longer display for errors
                    );
                }

                // Use centralized error handling instead of local error properties
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.Message ?? "Database operation failed"),
                    "Failed to save inventory item",
                    _applicationStateService.CurrentUser
                );

                await _applicationStateService.SetProgressAsync(0, $"Error: {result.Message ?? "Database operation failed"}");
            }
        }
        catch (Exception ex)
        {
            // Show error overlay for exceptions
            if (_successOverlayService != null)
            {
                await _successOverlayService.ShowErrorOverlayInMainViewAsync(
                    null,
                    "Unexpected Error",
                    $"An unexpected error occurred: {ex.Message}",
                    "AlertCircle",
                    5000
                );
            }

            // Use centralized error handling for exceptions
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Error saving inventory item", _applicationStateService.CurrentUser);
            Logger.LogError(ex, "Exception during inventory save operation");
            await _applicationStateService.SetProgressAsync(0, "Error occurred during save");
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
            Quantity = 0;  // Changed from 1 to 0 for blank state
            QuantityText = string.Empty;  // Clear the text representation
            Notes = string.Empty;
            // Note: Error/Success messages now handled by SuccessOverlayView

            // Form reset completed
            Logger.LogInformation("Inventory form reset completed successfully");

            // Trigger LostFocus events to restore error highlighting on cleared fields
            TriggerValidationLostFocus?.Invoke(this, EventArgs.Empty);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "ResetForm", _applicationStateService.CurrentUser);
        }
    }

    /// <summary>
    /// Command to open advanced entry dialog
    /// Fires event to communicate with parent MainViewViewModel
    /// </summary>
    [RelayCommand]
    private void AdvancedEntry()
    {
        try
        {
            Logger.LogInformation("Advanced entry requested");
            AdvancedEntryRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _ = Services.Core.ErrorHandling.HandleErrorAsync(ex, "AdvancedEntry", _applicationStateService.CurrentUser);
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
            _ = Services.Core.ErrorHandling.HandleErrorAsync(ex, "TogglePanel", _applicationStateService.CurrentUser);
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
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "RefreshLookupData", _applicationStateService.CurrentUser);
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

            // Load all lookup data in parallel
            var partTask = LoadPartIdsAsync();
            var operationTask = LoadOperationsAsync();
            var locationTask = LoadLocationsAsync();

            await Task.WhenAll(partTask, operationTask, locationTask);

            Logger.LogInformation("All lookup data loaded successfully");
        }
        catch (Exception ex)
        {
            // Use centralized error handling instead of local properties
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
            var masterResult = await Services.Core.Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _configurationService?.GetConnectionString() ?? throw new InvalidOperationException("Configuration service not available"),
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            if (masterResult.Status == 1 && masterResult.Data.Rows.Count > 0)
            {
                Logger.LogInformation("Successfully retrieved {Count} rows from md_part_ids_Get_All", masterResult.Data.Rows.Count);

                Dispatcher.UIThread.Post(() =>
                {
                    PartIds.Clear();
                    foreach (DataRow row in masterResult.Data.Rows)
                    {
                        // Use correct column name from md_part_ids table schema: "PartID"
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartIds.Add(partId);
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

                    // Log inventory column names for reference
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
        Logger.LogInformation("Loaded {Count} fallback Part IDs", PartIds.Count);
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
        Logger.LogInformation("Loaded {Count} fallback Operations", Operations.Count);
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
        Logger.LogInformation("Loaded {Count} fallback Locations", Locations.Count);
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
            Logger.LogWarning("Validation failed: Part ID is required");
            return false;
        }

        if (SelectedPart.Length > DatabaseConstraints.PartID_MaxLength)
        {
            Logger.LogWarning("Validation failed: Part ID cannot exceed {MaxLength} characters", DatabaseConstraints.PartID_MaxLength);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedOperation))
        {
            Logger.LogWarning("Validation failed: Operation is required");
            return false;
        }

        if (SelectedOperation.Length > DatabaseConstraints.Operation_MaxLength)
        {
            Logger.LogWarning("Validation failed: Operation cannot exceed {MaxLength} characters", DatabaseConstraints.Operation_MaxLength);
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLocation))
        {
            Logger.LogWarning("Validation failed: Location is required");
            return false;
        }

        if (SelectedLocation.Length > DatabaseConstraints.Location_MaxLength)
        {
            Logger.LogWarning("Validation failed: Location cannot exceed {MaxLength} characters", DatabaseConstraints.Location_MaxLength);
            return false;
        }

        if (Quantity <= 0)
        {
            Logger.LogWarning("Validation failed: Quantity must be greater than zero");
            return false;
        }

        if (!string.IsNullOrEmpty(Notes) && Notes.Length > DatabaseConstraints.Notes_MaxLength)
        {
            Logger.LogWarning("Validation failed: Notes cannot exceed {MaxLength} characters", DatabaseConstraints.Notes_MaxLength);
            return false;
        }

        // All validation passed
        return true;
    }

    #endregion

    #region Batch Number Management

    /// <summary>
    /// Generates a unique batch number for inventory tracking
    /// Following WinForms pattern for batch number uniqueness validation
    /// </summary>
    private async Task<string> GenerateUniqueBatchNumberAsync()
    {
        const int maxAttempts = 100;
        var attempt = 0;

        while (attempt < maxAttempts)
        {
            // Generate batch number with timestamp and random component
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var randomComponent = (Environment.TickCount % 10000) + attempt;
            var batchNumber = $"BATCH-{timestamp}-{randomComponent:D4}";

            try
            {
                // Check if batch number already exists in the database
                // This would require a stored procedure to check batch number uniqueness
                // For now, we'll use the timestamp + random approach which should be unique
                // Generated batch number (attempt {Attempt})
                return batchNumber;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to validate batch number uniqueness on attempt {Attempt}", attempt + 1);
                attempt++;

                // Add small delay to ensure timestamp changes
                await Task.Delay(10);
            }
        }

        // Fallback - this should never happen but provides safety
        var fallbackBatch = $"BATCH-FALLBACK-{DateTime.Now.Ticks}";
        Logger.LogWarning("Using fallback batch number after {MaxAttempts} attempts: {BatchNumber}", maxAttempts, fallbackBatch);
        return fallbackBatch;
    }

    #endregion

    #region Validation Management

    /// <summary>
    /// Refreshes all validation properties and notifications after programmatic changes.
    /// This method is called when values are set programmatically (e.g., via QuickButtons)
    /// to ensure that validation state is properly updated and error styling is cleared.
    /// </summary>
    public void RefreshValidationState()
    {
        try
        {
            // Refresh validation state for all form fields

            // Trigger property change notifications for all validation properties
            OnPropertyChanged(nameof(IsPartValid));
            OnPropertyChanged(nameof(IsPartValidInDatabase));
            OnPropertyChanged(nameof(IsOperationValid));
            OnPropertyChanged(nameof(IsOperationValidInDatabase));
            OnPropertyChanged(nameof(IsLocationValid));
            OnPropertyChanged(nameof(IsLocationValidInDatabase));
            OnPropertyChanged(nameof(IsQuantityValid));
            OnPropertyChanged(nameof(IsNotesValid));

            // Trigger property change notifications for watermarks
            OnPropertyChanged(nameof(PartWatermark));
            OnPropertyChanged(nameof(OperationWatermark));
            OnPropertyChanged(nameof(LocationWatermark));
            OnPropertyChanged(nameof(QuantityWatermark));
            OnPropertyChanged(nameof(NotesWatermark));

            // Trigger property change notification for overall form state
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();

            Logger.LogInformation("Validation state refreshed - CanSave: {CanSave}", CanSave);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing validation state");
        }
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
            Logger.LogInformation("InventoryTabViewModel disposed");
        }
        base.Dispose(disposing);
    }

    #endregion
}
