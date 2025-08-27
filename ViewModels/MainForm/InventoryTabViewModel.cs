using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Models;
using MTM_Shared_Logic.Services;
using MTM_Shared_Logic.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using ReactiveUI;
using static MTM_Shared_Logic.Services.DataType;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// Primary inventory management ViewModel for the MTM WIP Application.
/// Implements the complete business logic for adding new inventory items to the system,
/// including part selection, operation assignment, location specification, quantity entry, and notes.
/// Follows MTM patterns with string Part IDs, string operation numbers, and integer quantities.
/// </summary>
public class InventoryTabViewModel : BaseViewModel, INotifyPropertyChanged
{
    #region Private Fields

    private readonly MTM_Shared_Logic.Services.IInventoryService _inventoryService;
    private readonly MTM_Shared_Logic.Services.Interfaces.IUserService _userService;
    private readonly IApplicationStateService _applicationStateService;
    private readonly MTM_Shared_Logic.Services.IValidationService _validationService;
    private readonly MTM_Shared_Logic.Services.ILookupDataService _lookupDataService;

    // Observable collections for AutoCompleteBoxes
    public ObservableCollection<string> PartOptions { get; } = new();
    public ObservableCollection<string> OperationOptions { get; } = new();
    public ObservableCollection<string> LocationOptions { get; } = new();

    // Loading state properties
    private bool _isLoadingParts = false;
    private bool _isLoadingOperations = false;
    private bool _isLoadingLocations = false;

    // Form field backing properties
    private string? _selectedPart;
    private string? _selectedOperation;
    private string? _selectedLocation;
    private string _quantity = string.Empty;
    private string _notes = string.Empty;
    private string _versionText = "Version: 4.6.0.0";
    private bool _isLoading = false;
    private string? _errorMessage;
    private bool _hasError = false;

    // Validation state helpers
    private readonly ObservableAsPropertyHelper<bool> _canSave;
    private readonly ObservableAsPropertyHelper<bool> _isPartValid;
    private readonly ObservableAsPropertyHelper<bool> _isOperationValid;
    private readonly ObservableAsPropertyHelper<bool> _isLocationValid;
    private readonly ObservableAsPropertyHelper<bool> _isQuantityValid;

    #endregion

    #region Public Properties

    /// <summary>
    /// Selected Part ID for the inventory item.
    /// Must be selected from valid parts list (SelectedIndex > 0).
    /// </summary>
    public string? SelectedPart
    {
        get => _selectedPart;
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    /// <summary>
    /// Selected Operation for the inventory item.
    /// Operations are string numbers like "90", "100", "110" (workflow steps).
    /// Must be selected from valid operations list (SelectedIndex > 0).
    /// </summary>
    public string? SelectedOperation
    {
        get => _selectedOperation;
        set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
    }

    /// <summary>
    /// Selected Location for the inventory item.
    /// Must be selected from valid locations list (SelectedIndex > 0).
    /// </summary>
    public string? SelectedLocation
    {
        get => _selectedLocation;
        set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
    }

    /// <summary>
    /// Quantity for the inventory item.
    /// Must be a valid positive integer.
    /// </summary>
    public string Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    /// <summary>
    /// Optional notes for the inventory item.
    /// No validation required - optional field.
    /// </summary>
    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    /// <summary>
    /// Version display text for the control.
    /// </summary>
    public string VersionText
    {
        get => _versionText;
        set => this.RaiseAndSetIfChanged(ref _versionText, value);
    }

    /// <summary>
    /// Indicates if the control is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    /// <summary>
    /// Indicates if parts data is currently loading.
    /// </summary>
    public bool IsLoadingParts
    {
        get => _isLoadingParts;
        set => this.RaiseAndSetIfChanged(ref _isLoadingParts, value);
    }

    /// <summary>
    /// Indicates if operations data is currently loading.
    /// </summary>
    public bool IsLoadingOperations
    {
        get => _isLoadingOperations;
        set => this.RaiseAndSetIfChanged(ref _isLoadingOperations, value);
    }

    /// <summary>
    /// Indicates if locations data is currently loading.
    /// </summary>
    public bool IsLoadingLocations
    {
        get => _isLoadingLocations;
        set => this.RaiseAndSetIfChanged(ref _isLoadingLocations, value);
    }

    /// <summary>
    /// Current error message, if any.
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    /// <summary>
    /// Indicates if there is currently an error.
    /// </summary>
    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged(ref _hasError, value);
    }

    // Validation state properties
    public bool CanSave => _canSave.Value;
    public bool IsPartValid => _isPartValid.Value;
    public bool IsOperationValid => _isOperationValid.Value;
    public bool IsLocationValid => _isLocationValid.Value;
    public bool IsQuantityValid => _isQuantityValid.Value;

    #endregion

    #region Commands

    /// <summary>
    /// Command to save the inventory item.
    /// Enabled only when all required fields are valid.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }

    /// <summary>
    /// Command to reset the form.
    /// Supports soft reset (clear fields) and hard reset (reload data).
    /// </summary>
    public ReactiveCommand<bool, Unit> ResetCommand { get; private set; }

    /// <summary>
    /// Command to open advanced inventory features.
    /// Opens Control_AdvancedInventory.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AdvancedEntryCommand { get; private set; }

    /// <summary>
    /// Command to toggle the quick actions panel.
    /// </summary>
    public ReactiveCommand<Unit, Unit> TogglePanelCommand { get; private set; }

    /// <summary>
    /// Command to load AutoCompleteBox data from database.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

    /// <summary>
    /// Command to refresh all lookup data from database.
    /// </summary>
    public ReactiveCommand<Unit, Unit> RefreshDataCommand { get; private set; }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when an inventory item is successfully saved.
    /// Used for quick buttons integration.
    /// </summary>
    public event EventHandler<InventoryItemSavedEventArgs>? InventoryItemSaved;

    /// <summary>
    /// Event fired when panel toggle is requested.
    /// </summary>
    public event EventHandler? PanelToggleRequested;

    /// <summary>
    /// Event fired when advanced entry is requested.
    /// </summary>
    public event EventHandler? AdvancedEntryRequested;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the InventoryTabViewModel with dependency injection.
    /// </summary>
    public InventoryTabViewModel(
        MTM_Shared_Logic.Services.IInventoryService inventoryService,
        MTM_Shared_Logic.Services.Interfaces.IUserService userService,
        IApplicationStateService applicationStateService,
        MTM_Shared_Logic.Services.IValidationService validationService,
        MTM_Shared_Logic.Services.ILookupDataService lookupDataService,
        ILogger<InventoryTabViewModel> logger) : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _lookupDataService = lookupDataService ?? throw new ArgumentNullException(nameof(lookupDataService));

        Logger.LogInformation("Initializing InventoryTabViewModel with dependency injection");

        // Subscribe to data refresh events
        _lookupDataService.DataRefreshed += OnLookupDataRefreshed;

        // Initialize validation helpers
        _isPartValid = this.WhenAnyValue(vm => vm.SelectedPart)
            .Select(part => !string.IsNullOrWhiteSpace(part))
            .ToProperty(this, vm => vm.IsPartValid);

        _isOperationValid = this.WhenAnyValue(vm => vm.SelectedOperation)
            .Select(operation => !string.IsNullOrWhiteSpace(operation))
            .ToProperty(this, vm => vm.IsOperationValid);

        _isLocationValid = this.WhenAnyValue(vm => vm.SelectedLocation)
            .Select(location => !string.IsNullOrWhiteSpace(location))
            .ToProperty(this, vm => vm.IsLocationValid);

        _isQuantityValid = this.WhenAnyValue(vm => vm.Quantity)
            .Select(IsValidQuantity)
            .ToProperty(this, vm => vm.IsQuantityValid);

        // Overall validation - enable save only when all required fields are valid
        _canSave = this.WhenAnyValue(
                vm => vm.IsPartValid,
                vm => vm.IsOperationValid,
                vm => vm.IsLocationValid,
                vm => vm.IsQuantityValid,
                vm => vm.IsLoading,
                (partValid, operationValid, locationValid, quantityValid, loading) =>
                    partValid && operationValid && locationValid && quantityValid && !loading)
            .ToProperty(this, vm => vm.CanSave);

        // Initialize commands
        InitializeCommands();

        // Load initial data
        _ = Task.Run(async () => await LoadInitialDataAsync());

        Logger.LogInformation("InventoryTabViewModel initialized successfully");
    }

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        // Save command - enabled only when form is valid
        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await SaveInventoryItemAsync();
        }, this.WhenAnyValue(vm => vm.CanSave));

        // Reset command - takes boolean parameter for hard reset
        ResetCommand = ReactiveCommand.CreateFromTask<bool>(async (hardReset) =>
        {
            await ResetFormAsync(hardReset);
        });

        // Advanced entry command
        AdvancedEntryCommand = ReactiveCommand.Create(() =>
        {
            Logger.LogInformation("Advanced entry requested");
            AdvancedEntryRequested?.Invoke(this, EventArgs.Empty);
        });

        // Toggle panel command
        TogglePanelCommand = ReactiveCommand.Create(() =>
        {
            Logger.LogInformation("Panel toggle requested");
            PanelToggleRequested?.Invoke(this, EventArgs.Empty);
        });

        // Load data command - now refreshes lookup data
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadAutoCompleteDataAsync();
        });

        // Refresh data command - forces refresh of all lookup data
        RefreshDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await _lookupDataService.RefreshAllAsync();
            if (!result.IsSuccess)
            {
                SetError($"Failed to refresh data: {result.ErrorMessage}");
            }
        });

        // Error handling for all commands
        SaveCommand.ThrownExceptions.Subscribe(HandleException);
        ResetCommand.ThrownExceptions.Subscribe(HandleException);
        LoadDataCommand.ThrownExceptions.Subscribe(HandleException);
        RefreshDataCommand.ThrownExceptions.Subscribe(HandleException);

        Logger.LogDebug("Commands initialized successfully");
    }

    #endregion

    #region Business Logic

    /// <summary>
    /// Saves the inventory item to the database.
    /// Implements MTM business patterns with proper transaction logging.
    /// </summary>
    private async Task SaveInventoryItemAsync()
    {
        try
        {
            Logger.LogInformation("Starting inventory item save operation");
            
            ClearError();
            IsLoading = true;

            // Validate form data
            if (!ValidateFormData())
            {
                return;
            }

            // Get current user
            var currentUserResult = await _userService.GetUserByUsernameAsync(_applicationStateService.CurrentUser ?? "DefaultUser");
            if (!currentUserResult.IsSuccess || currentUserResult.Value == null)
            {
                SetError("Unable to determine current user. Please login again.");
                return;
            }

            var currentUser = currentUserResult.Value;

            // Parse quantity
            if (!int.TryParse(Quantity, out int quantity))
            {
                SetError("Invalid quantity value.");
                return;
            }

            // Create inventory item
            var inventoryItem = new InventoryItem
            {
                PartID = SelectedPart!,
                Operation = SelectedOperation,
                Location = SelectedLocation!,
                Quantity = quantity,
                Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim(),
                User = currentUser.User_Name,
                ItemType = Model_AppVariables.MTM.DefaultItemType, // "WIP"
                ReceiveDate = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            // Save to database
            Logger.LogInformation("Saving inventory item: Part={PartId}, Operation={Operation}, Location={Location}, Quantity={Quantity}", 
                inventoryItem.PartID, inventoryItem.Operation, inventoryItem.Location, inventoryItem.Quantity);

            var saveResult = await _inventoryService.AddInventoryItemAsync(inventoryItem);
            
            if (!saveResult.IsSuccess)
            {
                SetError($"Failed to save inventory item: {saveResult.ErrorMessage}");
                return;
            }

            Logger.LogInformation("Inventory item saved successfully");

            // Fire event for quick buttons integration
            InventoryItemSaved?.Invoke(this, new InventoryItemSavedEventArgs
            {
                PartId = inventoryItem.PartID,
                Operation = inventoryItem.Operation ?? string.Empty,
                Location = inventoryItem.Location,
                Quantity = inventoryItem.Quantity,
                Notes = inventoryItem.Notes ?? string.Empty
            });

            // Reset form after successful save (soft reset)
            await ResetFormAsync(hardReset: false);

            Logger.LogInformation("Save operation completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving inventory item");
            SetError("An unexpected error occurred while saving. Please try again.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Resets the form to initial state.
    /// Supports soft reset (clear fields) and hard reset (reload data).
    /// </summary>
    private async Task ResetFormAsync(bool hardReset = false)
    {
        try
        {
            Logger.LogInformation("Resetting form. Hard reset: {HardReset}", hardReset);
            
            IsLoading = true;
            ClearError();

            // Clear form fields
            SelectedPart = null;
            SelectedOperation = null;
            SelectedLocation = null;
            Quantity = string.Empty;
            Notes = string.Empty;

            if (hardReset)
            {
                // Hard reset - reload all AutoCompleteBox data from database
                Logger.LogInformation("Performing hard reset - reloading all data");
                
                // Clear cached data and reload
                var refreshResult = await _lookupDataService.RefreshAllAsync();
                if (!refreshResult.IsSuccess)
                {
                    Logger.LogWarning("Failed to refresh lookup data during hard reset: {Error}", refreshResult.ErrorMessage);
                    // Continue with existing data rather than failing
                }
            }

            Logger.LogInformation("Form reset completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting form");
            SetError("An error occurred while resetting the form.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads initial data for AutoCompleteBoxes.
    /// </summary>
    private async Task LoadInitialDataAsync()
    {
        try
        {
            Logger.LogInformation("Loading initial data for AutoCompleteBoxes");
            
            IsLoading = true;
            
            await LoadAutoCompleteDataAsync();
            
            Logger.LogInformation("Initial data loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading initial data");
            // Load sample data as fallback
            LoadSampleData();
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads AutoCompleteBox data from the lookup data service efficiently.
    /// Implements parallel loading with individual loading indicators.
    /// </summary>
    private async Task LoadAutoCompleteDataAsync()
    {
        try
        {
            Logger.LogDebug("Loading AutoCompleteBox data from lookup service");

            // Load data in parallel for better performance
            var loadTasks = new[]
            {
                LoadPartsDataAsync(),
                LoadOperationsDataAsync(),
                LoadLocationsDataAsync()
            };

            await Task.WhenAll(loadTasks);

            Logger.LogInformation("AutoCompleteBox data loaded successfully. Parts: {PartCount}, Operations: {OpCount}, Locations: {LocCount}",
                PartOptions.Count, OperationOptions.Count, LocationOptions.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading AutoCompleteBox data from lookup service");
            
            // Fallback to sample data
            LoadSampleData();
        }
    }

    /// <summary>
    /// Loads parts data with individual loading indicator.
    /// </summary>
    private async Task LoadPartsDataAsync()
    {
        try
        {
            IsLoadingParts = true;
            var result = await _lookupDataService.GetPartIdsAsync();
            
            if (result.IsSuccess && result.Value != null)
            {
                PartOptions.Clear();
                foreach (var part in result.Value)
                {
                    PartOptions.Add(part);
                }
                Logger.LogDebug("Loaded {Count} parts", PartOptions.Count);
            }
            else
            {
                Logger.LogWarning("Failed to load parts: {Error}", result.ErrorMessage);
                LoadSampleParts();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading parts data");
            LoadSampleParts();
        }
        finally
        {
            IsLoadingParts = false;
        }
    }

    /// <summary>
    /// Loads operations data with individual loading indicator.
    /// </summary>
    private async Task LoadOperationsDataAsync()
    {
        try
        {
            IsLoadingOperations = true;
            var result = await _lookupDataService.GetOperationsAsync();
            
            if (result.IsSuccess && result.Value != null)
            {
                OperationOptions.Clear();
                foreach (var operation in result.Value)
                {
                    OperationOptions.Add(operation);
                }
                Logger.LogDebug("Loaded {Count} operations", OperationOptions.Count);
            }
            else
            {
                Logger.LogWarning("Failed to load operations: {Error}", result.ErrorMessage);
                LoadSampleOperations();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading operations data");
            LoadSampleOperations();
        }
        finally
        {
            IsLoadingOperations = false;
        }
    }

    /// <summary>
    /// Loads locations data with individual loading indicator.
    /// </summary>
    private async Task LoadLocationsDataAsync()
    {
        try
        {
            IsLoadingLocations = true;
            var result = await _lookupDataService.GetLocationsAsync();
            
            if (result.IsSuccess && result.Value != null)
            {
                LocationOptions.Clear();
                foreach (var location in result.Value)
                {
                    LocationOptions.Add(location);
                }
                Logger.LogDebug("Loaded {Count} locations", LocationOptions.Count);
            }
            else
            {
                Logger.LogWarning("Failed to load locations: {Error}", result.ErrorMessage);
                LoadSampleLocations();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading locations data");
            LoadSampleLocations();
        }
        finally
        {
            IsLoadingLocations = false;
        }
    }

    /// <summary>
    /// Handles lookup data refresh events.
    /// </summary>
    private async void OnLookupDataRefreshed(object? sender, DataRefreshedEventArgs e)
    {
        try
        {
            Logger.LogInformation("Lookup data refreshed for type: {DataType}", e.DataType);

            // Reload specific data based on refresh type
            switch (e.DataType)
            {
                case DataType.Parts:
                    await LoadPartsDataAsync();
                    break;
                case DataType.Operations:
                    await LoadOperationsDataAsync();
                    break;
                case DataType.Locations:
                    await LoadLocationsDataAsync();
                    break;
                case DataType.All:
                    await LoadAutoCompleteDataAsync();
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling lookup data refresh for type: {DataType}", e.DataType);
        }
    }

    #endregion

    #region Validation

    /// <summary>
    /// Validates that the quantity is a valid positive integer.
    /// </summary>
    private bool IsValidQuantity(string? quantity)
    {
        if (string.IsNullOrWhiteSpace(quantity))
            return false;

        return int.TryParse(quantity, out int result) && result > 0;
    }

    /// <summary>
    /// Validates all form data before saving.
    /// </summary>
    private bool ValidateFormData()
    {
        if (string.IsNullOrWhiteSpace(SelectedPart))
        {
            SetError("Part ID is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedOperation))
        {
            SetError("Operation is required.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedLocation))
        {
            SetError("Location is required.");
            return false;
        }

        if (!IsValidQuantity(Quantity))
        {
            SetError("Quantity must be a valid positive number.");
            return false;
        }

        // Validate quantity against business rules
        if (int.TryParse(Quantity, out int qty) && qty > Model_AppVariables.MTM.MaxTransactionQuantity)
        {
            SetError($"Quantity cannot exceed {Model_AppVariables.MTM.MaxTransactionQuantity}.");
            return false;
        }

        return true;
    }

    #endregion

    #region Sample Data

    /// <summary>
    /// Loads sample data for demonstration and fallback purposes.
    /// </summary>
    private void LoadSampleData()
    {
        Logger.LogDebug("Loading sample data for ComboBoxes");
        
        LoadSampleParts();
        LoadSampleOperations();
        LoadSampleLocations();
    }

    private void LoadSampleParts()
    {
        PartOptions.Clear();
        var sampleParts = new[] { "PART001", "PART002", "PART003", "PART004", "PART005", "ABC-123", "XYZ-789" };
        foreach (var part in sampleParts)
        {
            PartOptions.Add(part);
        }
    }

    private void LoadSampleOperations()
    {
        OperationOptions.Clear();
        var sampleOperations = new[] { "90", "100", "110", "120", "130", "140", "150" };
        foreach (var operation in sampleOperations)
        {
            OperationOptions.Add(operation);
        }
    }

    private void LoadSampleLocations()
    {
        LocationOptions.Clear();
        var sampleLocations = new[] { "WC01", "WC02", "WC03", "WC04", "WC05", "FLOOR", "QC" };
        foreach (var location in sampleLocations)
        {
            LocationOptions.Add(location);
        }
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Sets an error message and updates error state.
    /// </summary>
    private void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        Logger.LogWarning("Error set: {ErrorMessage}", message);
    }

    /// <summary>
    /// Clears the current error state.
    /// </summary>
    private void ClearError()
    {
        ErrorMessage = null;
        HasError = false;
    }

    /// <summary>
    /// Handles exceptions from commands.
    /// </summary>
    private void HandleException(Exception ex)
    {
        Logger.LogError(ex, "Command execution error");
        SetError("An unexpected error occurred. Please try again.");
    }

    #endregion

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from lookup data service events
            if (_lookupDataService != null)
            {
                _lookupDataService.DataRefreshed -= OnLookupDataRefreshed;
            }

            _canSave?.Dispose();
            _isPartValid?.Dispose();
            _isOperationValid?.Dispose();
            _isLocationValid?.Dispose();
            _isQuantityValid?.Dispose();
        }
        base.Dispose(disposing);
    }

    #endregion
}

/// <summary>
/// Event arguments for inventory item saved event.
/// Used for quick buttons integration.
/// </summary>
public class InventoryItemSavedEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Notes { get; set; } = string.Empty;
}
