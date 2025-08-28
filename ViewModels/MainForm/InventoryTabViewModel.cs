using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

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

    public event PropertyChangedEventHandler? PropertyChanged;

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
        InitializeLookupData();
        
        Logger.LogInformation("InventoryTabViewModel initialized");
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
        LoadDataCommand = new AsyncCommand(ExecuteLoadDataAsync);
        RefreshDataCommand = new AsyncCommand(ExecuteRefreshDataAsync);
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

            // Prepare parameters for stored procedure
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = SelectedPart,
                ["p_OperationID"] = SelectedOperation,
                ["p_LocationID"] = SelectedLocation,
                ["p_Quantity"] = Quantity,
                ["p_Notes"] = Notes,
                ["p_UserID"] = _applicationStateService.CurrentUser,
                ["p_TransactionType"] = "IN" // Always IN when adding inventory
            };

            // Execute stored procedure using connection string from configuration
            var connectionString = _configurationService.GetConnectionString();
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "inv_inventory_Add_Item",
                parameters
            );

            if (result.IsSuccess)
            {
                Logger.LogInformation("Inventory item saved successfully");
                await ExecuteResetAsync();
                
                // Update application state
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
                    ["Quantity"] = Quantity
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

    private async Task ExecuteLoadDataAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;

            await LoadPartIdsAsync();
            await LoadOperationsAsync();
            await LoadLocationsAsync();

            Logger.LogInformation("Data loading completed successfully");
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ErrorHandling.GetUserFriendlyMessage(ex);
            await ErrorHandling.HandleErrorAsync(ex, "LoadData", _applicationStateService.CurrentUser);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteRefreshDataAsync()
    {
        try
        {
            await ExecuteLoadDataAsync();
            Logger.LogInformation("Data refresh completed");
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "RefreshData", _applicationStateService.CurrentUser);
        }
    }

    #endregion

    #region Data Loading

    private void InitializeLookupData()
    {
        // Initialize with sample data until database integration is available
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        // Sample Part IDs
        PartIds.Clear();
        var sampleParts = new[] { "PART001", "PART002", "PART003", "ABC-123", "XYZ-789" };
        foreach (var part in sampleParts)
        {
            PartIds.Add(part);
        }

        // Sample Operations
        Operations.Clear();
        var sampleOperations = new[] { "90", "100", "110", "120", "130" };
        foreach (var operation in sampleOperations)
        {
            Operations.Add(operation);
        }

        // Sample Locations
        Locations.Clear();
        var sampleLocations = new[] { "WC01", "WC02", "FLOOR", "QC", "SHIPPING" };
        foreach (var location in sampleLocations)
        {
            Locations.Add(location);
        }
    }

    private async Task LoadPartIdsAsync()
    {
        try
        {
            IsLoadingParts = true;
            
            // TODO: Load from database using stored procedure
            // For now, use sample data
            await Task.Delay(100);
            LoadSampleData();
            
            Logger.LogDebug("Part IDs loaded: {Count} items", PartIds.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Part IDs");
            throw;
        }
        finally
        {
            IsLoadingParts = false;
        }
    }

    private async Task LoadOperationsAsync()
    {
        try
        {
            IsLoadingOperations = true;
            
            // TODO: Load from database using stored procedure
            await Task.Delay(50);
            
            Logger.LogDebug("Operations loaded: {Count} items", Operations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Operations");
            throw;
        }
        finally
        {
            IsLoadingOperations = false;
        }
    }

    private async Task LoadLocationsAsync()
    {
        try
        {
            IsLoadingLocations = true;
            
            // TODO: Load from database using stored procedure
            await Task.Delay(75);
            
            Logger.LogDebug("Locations loaded: {Count} items", Locations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Locations");
            throw;
        }
        finally
        {
            IsLoadingLocations = false;
        }
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
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
