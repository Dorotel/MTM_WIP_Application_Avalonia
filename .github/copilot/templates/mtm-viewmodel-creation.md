# MTM ViewModel Creation Template

## 🏗️ ViewModel Development Instructions

For creating ViewModels in the MTM WIP Application using MVVM Community Toolkit:

### **Base ViewModel Structure**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

/// <summary>
/// ViewModel for [Feature/Component] functionality
/// </summary>
[ObservableObject]
public partial class [Name]ViewModel : BaseViewModel
{
    #region Private Fields
    
    private readonly I[Service] _service;
    
    #endregion

    #region Observable Properties
    
    [ObservableProperty]
    private string inputValue = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string? errorMessage;
    
    [ObservableProperty]
    private ObservableCollection<ItemModel> items = new();
    
    #endregion

    #region Commands
    
    [RelayCommand]
    private async Task ExecuteAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Command implementation
            var result = await _service.ProcessAsync(InputValue);
            
            if (result.IsSuccess)
            {
                // Handle success
                Items.Clear();
                foreach (var item in result.Data)
                {
                    Items.Add(item);
                }
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
            }
        });
    }
    
    [RelayCommand(CanExecute = nameof(CanExecuteAction))]
    private async Task ExecuteActionAsync()
    {
        // Action with validation
    }
    
    private bool CanExecuteAction => !string.IsNullOrWhiteSpace(InputValue) && !IsLoading;
    
    #endregion

    #region Constructor
    
    /// <summary>
    /// Initializes a new instance of [Name]ViewModel
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="service">Service dependency</param>
    public [Name]ViewModel(
        ILogger<[Name]ViewModel> logger, 
        I[Service] service) 
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        
        _service = service;
        
        // Initialize data
        InitializeAsync();
    }
    
    #endregion

    #region Private Methods
    
    /// <summary>
    /// Initialize ViewModel data
    /// </summary>
    private async void InitializeAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            // Load initial data
            await LoadDataAsync();
        });
    }
    
    /// <summary>
    /// Load data from service
    /// </summary>
    private async Task LoadDataAsync()
    {
        try
        {
            var result = await _service.GetDataAsync();
            
            if (result.IsSuccess)
            {
                Items.Clear();
                foreach (var item in result.Data)
                {
                    Items.Add(item);
                }
            }
            else
            {
                Logger.LogWarning("Failed to load data: {Error}", result.ErrorMessage);
                ErrorMessage = "Failed to load data";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading data");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Loading data");
        }
    }
    
    #endregion

    #region Property Change Handlers
    
    partial void OnInputValueChanged(string value)
    {
        // Clear error when user starts typing
        ErrorMessage = null;
        
        // Notify commands that depend on this property
        ExecuteActionCommand.NotifyCanExecuteChanged();
    }
    
    #endregion

    #region IDisposable Implementation
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose resources
            Items.Clear();
        }
        
        base.Dispose(disposing);
    }
    
    #endregion
}
```

### **Property Validation Pattern**
```csharp
[ObservableProperty]
[NotifyDataErrorInfo]
[Required(ErrorMessage = "This field is required")]
[StringLength(50, ErrorMessage = "Maximum 50 characters")]
private string validatedProperty = string.Empty;
```

### **Command with Parameters**
```csharp
[RelayCommand]
private async Task ExecuteWithParameterAsync(object parameter)
{
    if (parameter is SpecificType typedParam)
    {
        // Handle typed parameter
    }
}
```

### **Collection Management**
```csharp
[ObservableProperty]
private ObservableCollection<ItemViewModel> items = new();

[RelayCommand]
private void AddItem()
{
    Items.Add(new ItemViewModel());
}

[RelayCommand]
private void RemoveItem(ItemViewModel item)
{
    Items.Remove(item);
}
```