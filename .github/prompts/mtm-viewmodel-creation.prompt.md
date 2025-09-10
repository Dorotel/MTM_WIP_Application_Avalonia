---
description: 'Create complete ViewModels using MVVM Community Toolkit patterns with proper service integration and error handling'
tools: ['codebase', 'search', 'usages', 'editFiles']
---

# MTM ViewModel Creation Template

Generate complete ViewModels for the MTM WIP Application using MVVM Community Toolkit source generators, service integration, and established architectural patterns.

## ViewModel Architecture Framework

### 1. ViewModel Design Analysis
- **Business Functionality**: Define the core business operations the ViewModel supports
- **Data Requirements**: Identify data sources, display models, and user input needs
- **Service Dependencies**: Map required services for data access, business logic, and external integrations
- **User Interactions**: Plan commands, validation, and user feedback mechanisms

### 2. MVVM Community Toolkit Integration
- **Source Generators**: Use `[ObservableObject]`, `[ObservableProperty]`, and `[RelayCommand]` attributes
- **Property Validation**: Implement `[NotifyDataErrorInfo]` with validation attributes
- **Command Logic**: Design async commands with proper error handling and loading states
- **Property Dependencies**: Set up `[NotifyPropertyChangedFor]` relationships

## Complete ViewModel Template

### Base ViewModel Structure
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

/// <summary>
/// ViewModel for [ViewName] - [Brief description of functionality]
/// Handles [specific business operations] for the MTM manufacturing system
/// </summary>
[ObservableObject]
public partial class [ViewName]ViewModel : BaseViewModel, IRecipient<[MessageType]>
{
    #region Private Fields
    
    private readonly I[PrimaryService] _primaryService;
    private readonly IMasterDataService _masterDataService;
    private readonly IConfigurationService _configurationService;
    
    #endregion

    #region Observable Properties - Form Data
    
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Only uppercase letters, numbers, and hyphens allowed")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecutePrimaryAction))]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    private string partId = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "Operation is required")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecutePrimaryAction))]
    private string selectedOperation = string.Empty;

    [ObservableProperty]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecutePrimaryAction))]
    private int quantity = 1;

    [ObservableProperty]
    [Required(ErrorMessage = "Location is required")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecutePrimaryAction))]
    private string selectedLocation = string.Empty;

    #endregion

    #region Observable Properties - UI State
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecutePrimaryAction))]
    [NotifyPropertyChangedFor(nameof(CanExecuteSecondaryAction))]
    private bool isLoading;

    [ObservableProperty]
    private string? statusMessage;

    [ObservableProperty]
    private bool hasSuccess;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string? errorDetails;

    #endregion

    #region Observable Properties - Data Collections
    
    [ObservableProperty]
    private ObservableCollection<string> operations = new();

    [ObservableProperty]
    private ObservableCollection<string> locations = new();

    [ObservableProperty]
    private ObservableCollection<[DataModel]> dataItems = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedItem))]
    [NotifyPropertyChangedFor(nameof(CanDeleteSelected))]
    private [DataModel]? selectedItem;

    #endregion

    #region Computed Properties
    
    /// <summary>
    /// Determines if the primary action can be executed
    /// </summary>
    public bool CanExecutePrimaryAction => 
        !IsLoading && 
        !string.IsNullOrWhiteSpace(PartId) && 
        !string.IsNullOrWhiteSpace(SelectedOperation) && 
        Quantity > 0 && 
        !string.IsNullOrWhiteSpace(SelectedLocation) &&
        !HasErrors;

    /// <summary>
    /// Determines if the form is valid for submission
    /// </summary>
    public bool IsFormValid => 
        !string.IsNullOrWhiteSpace(PartId) && 
        !string.IsNullOrWhiteSpace(SelectedOperation) && 
        Quantity > 0 && 
        !string.IsNullOrWhiteSpace(SelectedLocation) &&
        !HasErrors;

    /// <summary>
    /// Indicates if an item is currently selected
    /// </summary>
    public bool HasSelectedItem => SelectedItem != null;

    /// <summary>
    /// Determines if the selected item can be deleted
    /// </summary>
    public bool CanDeleteSelected => HasSelectedItem && !IsLoading;

    /// <summary>
    /// Indicates if there are data items to display
    /// </summary>
    public bool HasDataItems => DataItems.Count > 0;

    #endregion

    #region Commands - Primary Actions
    
    [RelayCommand(CanExecute = nameof(CanExecutePrimaryAction))]
    private async Task ExecutePrimaryActionAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            ClearStatus();
            
            try
            {
                // Validate all properties
                ValidateAllProperties();
                if (HasErrors)
                {
                    SetError("Please correct validation errors before proceeding");
                    return;
                }

                // Create request object
                var request = new [RequestModel]
                {
                    PartId = PartId.Trim().ToUpperInvariant(),
                    Operation = SelectedOperation,
                    Quantity = Quantity,
                    Location = SelectedLocation,
                    UserId = CurrentUser?.UserId ?? Environment.UserName,
                    Timestamp = DateTime.Now
                };

                // Execute primary business operation
                var result = await _primaryService.ExecutePrimaryOperationAsync(request);

                if (result.IsSuccess)
                {
                    SetSuccess($"Operation completed successfully. {result.Message}");
                    await RefreshDataAsync();
                    ResetForm();
                    
                    // Send notification message
                    WeakReferenceMessenger.Default.Send(new [OperationCompletedMessage](request));
                }
                else
                {
                    SetError($"Operation failed: {result.ErrorMessage}");
                    Logger.LogWarning("Primary operation failed: {Error}", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Primary operation execution");
                SetError("An unexpected error occurred. Please try again.");
            }
        });
    }

    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                var data = await _primaryService.GetDataAsync();
                
                DataItems.Clear();
                foreach (var item in data)
                {
                    DataItems.Add(item);
                }
                
                SetSuccess($"Loaded {data.Count} items");
                Logger.LogInformation("Data refreshed successfully, {Count} items loaded", data.Count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to refresh data");
                await Services.ErrorHandling.HandleErrorAsync(ex, "Data refresh");
                SetError("Failed to load data. Please try again.");
            }
        });
    }

    [RelayCommand(CanExecute = nameof(CanDeleteSelected))]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedItem == null) return;

        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                var itemToDelete = SelectedItem;
                var result = await _primaryService.DeleteItemAsync(itemToDelete.Id);

                if (result.IsSuccess)
                {
                    DataItems.Remove(itemToDelete);
                    SelectedItem = null;
                    SetSuccess($"Item deleted successfully");
                    
                    // Send notification message
                    WeakReferenceMessenger.Default.Send(new [ItemDeletedMessage](itemToDelete));
                }
                else
                {
                    SetError($"Delete failed: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Delete item operation");
                SetError("Failed to delete item. Please try again.");
            }
        });
    }

    #endregion

    #region Commands - Secondary Actions
    
    [RelayCommand]
    private void ResetForm()
    {
        PartId = string.Empty;
        SelectedOperation = string.Empty;
        Quantity = 1;
        SelectedLocation = string.Empty;
        SelectedItem = null;
        ClearStatus();
        ClearErrors();
        
        Logger.LogDebug("Form reset completed");
    }

    [RelayCommand]
    private async Task ExportDataAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                var exportData = DataItems.ToList();
                var result = await _primaryService.ExportDataAsync(exportData);

                if (result.IsSuccess)
                {
                    SetSuccess($"Data exported successfully to {result.Data}");
                }
                else
                {
                    SetError($"Export failed: {result.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Data export");
                SetError("Export failed. Please try again.");
            }
        });
    }

    [RelayCommand]
    private void ClearFilters()
    {
        // Clear any filter properties
        ResetForm();
        SetSuccess("Filters cleared");
    }

    #endregion

    #region Constructor and Initialization
    
    public [ViewName]ViewModel(
        ILogger<[ViewName]ViewModel> logger,
        I[PrimaryService] primaryService,
        IMasterDataService masterDataService,
        IConfigurationService configurationService)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(primaryService);
        ArgumentNullException.ThrowIfNull(masterDataService);
        ArgumentNullException.ThrowIfNull(configurationService);
        
        _primaryService = primaryService;
        _masterDataService = masterDataService;
        _configurationService = configurationService;
        
        // Register for messages
        WeakReferenceMessenger.Default.Register<[MessageType]>(this);
        
        // Initialize data
        _ = InitializeAsync();
    }
    
    private async Task InitializeAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            await Task.WhenAll(
                LoadMasterDataAsync(),
                RefreshDataAsync()
            );
        });
    }

    private async Task LoadMasterDataAsync()
    {
        try
        {
            // Load operations
            var operations = await _masterDataService.GetOperationsAsync();
            Operations.Clear();
            foreach (var operation in operations)
            {
                Operations.Add(operation);
            }

            // Load locations
            var locations = await _masterDataService.GetLocationsAsync();
            Locations.Clear();
            foreach (var location in locations)
            {
                Locations.Add(location);
            }

            // Set default values from configuration
            var defaultOperation = await _configurationService.GetDefaultOperationAsync();
            if (!string.IsNullOrEmpty(defaultOperation) && Operations.Contains(defaultOperation))
            {
                SelectedOperation = defaultOperation;
            }

            var defaultLocation = await _configurationService.GetDefaultLocationAsync();
            if (!string.IsNullOrEmpty(defaultLocation) && Locations.Contains(defaultLocation))
            {
                SelectedLocation = defaultLocation;
            }

            Logger.LogInformation("Master data loaded: {OperationCount} operations, {LocationCount} locations", 
                Operations.Count, Locations.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load master data");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Loading master data");
            SetError("Failed to load configuration data");
        }
    }
    
    #endregion

    #region Property Change Handlers
    
    partial void OnPartIdChanged(string value)
    {
        // Auto-format part ID
        if (!string.IsNullOrWhiteSpace(value))
        {
            PartId = value.ToUpperInvariant().Trim();
        }

        // Clear status when user starts typing
        ClearStatus();
        
        // Update command states
        ExecutePrimaryActionCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedOperationChanged(string value)
    {
        ClearStatus();
        ExecutePrimaryActionCommand.NotifyCanExecuteChanged();
        
        Logger.LogDebug("Operation changed to: {Operation}", value);
    }

    partial void OnQuantityChanged(int value)
    {
        ClearStatus();
        ExecutePrimaryActionCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedLocationChanged(string value)
    {
        ClearStatus();
        ExecutePrimaryActionCommand.NotifyCanExecuteChanged();
        
        Logger.LogDebug("Location changed to: {Location}", value);
    }

    partial void OnSelectedItemChanged([DataModel]? value)
    {
        DeleteSelectedCommand.NotifyCanExecuteChanged();
        
        if (value != null)
        {
            Logger.LogDebug("Item selected: {ItemId}", value.Id);
        }
    }

    #endregion

    #region Status Management
    
    private void SetSuccess(string message)
    {
        StatusMessage = message;
        HasSuccess = true;
        HasError = false;
        ErrorDetails = null;
    }

    private void SetError(string message, string? details = null)
    {
        StatusMessage = message;
        HasError = true;
        HasSuccess = false;
        ErrorDetails = details;
    }

    private void ClearStatus()
    {
        StatusMessage = null;
        HasSuccess = false;
        HasError = false;
        ErrorDetails = null;
    }

    #endregion

    #region Message Handling
    
    public void Receive([MessageType] message)
    {
        // Handle incoming messages
        switch (message.Action)
        {
            case "[ActionType]":
                _ = RefreshDataAsync();
                break;
                
            default:
                Logger.LogWarning("Unhandled message action: {Action}", message.Action);
                break;
        }
    }

    #endregion

    #region Dispose Pattern
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unregister from messages
            WeakReferenceMessenger.Default.Unregister<[MessageType]>(this);
            
            // Dispose other resources
            DataItems.Clear();
            Operations.Clear();
            Locations.Clear();
        }
        
        base.Dispose(disposing);
    }
    
    #endregion
}
```

## Advanced ViewModel Patterns

### Validation Integration
```csharp
// Custom validation with business rules
partial void OnPartIdChanged(string value)
{
    ClearErrors(nameof(PartId));
    
    if (!string.IsNullOrWhiteSpace(value))
    {
        // Format validation
        if (!Regex.IsMatch(value, @"^[A-Z0-9\-]+$"))
        {
            AddError("Part ID can only contain uppercase letters, numbers, and hyphens", nameof(PartId));
        }
        
        // Length validation
        if (value.Length > 50)
        {
            AddError("Part ID cannot exceed 50 characters", nameof(PartId));
        }
        
        // Business rule validation
        if (value.StartsWith("TEST") && !_configurationService.IsTestModeEnabled())
        {
            AddError("Test part IDs are not allowed in production mode", nameof(PartId));
        }
    }
}
```

### Collection Management
```csharp
[RelayCommand]
private void AddNewItem()
{
    var newItem = new [DataModel]
    {
        Id = Guid.NewGuid(),
        Name = "New Item",
        CreatedDate = DateTime.Now,
        CreatedBy = CurrentUser?.UserId ?? "Unknown"
    };
    
    DataItems.Add(newItem);
    SelectedItem = newItem;
}

[RelayCommand]
private void MoveItemUp([DataModel] item)
{
    var currentIndex = DataItems.IndexOf(item);
    if (currentIndex > 0)
    {
        DataItems.Move(currentIndex, currentIndex - 1);
    }
}
```

### Async Initialization with Cancellation
```csharp
private CancellationTokenSource? _initializationCancellation;

private async Task InitializeAsync()
{
    _initializationCancellation?.Cancel();
    _initializationCancellation = new CancellationTokenSource();
    
    try
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            await LoadMasterDataAsync(_initializationCancellation.Token);
            await RefreshDataAsync(_initializationCancellation.Token);
        });
    }
    catch (OperationCanceledException)
    {
        Logger.LogDebug("Initialization cancelled");
    }
}

private async Task LoadMasterDataAsync(CancellationToken cancellationToken)
{
    var tasks = new[]
    {
        LoadOperationsAsync(cancellationToken),
        LoadLocationsAsync(cancellationToken),
        LoadUsersAsync(cancellationToken)
    };
    
    await Task.WhenAll(tasks);
}
```

## Integration Guidelines

### Service Registration
```csharp
// Extensions/ServiceCollectionExtensions.cs
services.TryAddTransient<[ViewName]ViewModel>();
services.TryAddScoped<I[PrimaryService], [PrimaryService]>();
```

### View Integration
```xml
<!-- View AXAML DataContext binding -->
<UserControl.DataContext>
    <Binding Path="{x:Static vm:[ViewName]ViewModel}" 
             Source="{x:Static Application.Current}" />
</UserControl.DataContext>
```

### Navigation Integration
```csharp
// Navigation service integration
public async Task NavigateToViewAsync()
{
    var viewModel = _serviceProvider.GetRequiredService<[ViewName]ViewModel>();
    await _navigationService.NavigateAsync(viewModel);
}
```

Use this template when creating ViewModels that require full MVVM Community Toolkit integration, comprehensive error handling, and seamless service layer communication.