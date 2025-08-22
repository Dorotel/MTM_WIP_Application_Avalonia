# Custom Prompt: Create Missing Navigation Service

## ⚠️ HIGH PRIORITY FIX #10

**Issue**: No navigation service exists, leading to direct view instantiation that violates MVVM patterns.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- Missing `Services/INavigationService.cs`
- ViewModels directly creating/showing views
- No navigation history or state management
- MainWindow handling navigation manually

**Priority**: ⚠️ **HIGH - MVVM COMPLIANCE**

---

## Custom Prompt

```
HIGH PRIORITY MVVM COMPLIANCE: Create comprehensive navigation service to enable proper MVVM navigation patterns, eliminate direct view instantiation, and provide centralized navigation logic.

REQUIREMENTS:
1. Create INavigationService interface with navigation methods
2. Implement NavigationService with view registration
3. Add navigation methods (NavigateTo, GoBack, etc.)
4. Register view types with their ViewModels
5. Update ViewModels to use navigation service
6. Implement navigation history and state management
7. Enable MainWindow to respond to navigation requests

NAVIGATION SERVICE INTERFACE:

**Services/INavigationService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Services;

public interface INavigationService
{
    // Navigation Methods
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo(Type viewModelType);
    void NavigateTo(string viewName);
    void NavigateToWithParameter<TViewModel, TParameter>(TParameter parameter) where TViewModel : class;
    
    // Navigation History
    void GoBack();
    void GoForward();
    bool CanGoBack { get; }
    bool CanGoForward { get; }
    void ClearHistory();
    
    // Navigation State
    Type? CurrentViewModelType { get; }
    string? CurrentViewName { get; }
    object? CurrentViewModel { get; }
    IReadOnlyList<string> NavigationHistory { get; }
    
    // View Registration
    void RegisterView<TViewModel, TView>() 
        where TViewModel : class 
        where TView : class;
    void RegisterView(Type viewModelType, Type viewType);
    void RegisterView(string viewName, Type viewModelType, Type viewType);
    
    // Events
    event EventHandler<NavigationRequestedEventArgs>? NavigationRequested;
    event EventHandler<NavigationCompletedEventArgs>? NavigationCompleted;
    event EventHandler<NavigationFailedEventArgs>? NavigationFailed;
}

public class NavigationRequestedEventArgs : EventArgs
{
    public Type ViewModelType { get; set; } = null!;
    public Type ViewType { get; set; } = null!;
    public string? ViewName { get; set; }
    public object? Parameter { get; set; }
    public object? ViewModel { get; set; }
    public bool Cancel { get; set; } = false;
}

public class NavigationCompletedEventArgs : EventArgs
{
    public Type ViewModelType { get; set; } = null!;
    public Type ViewType { get; set; } = null!;
    public string? ViewName { get; set; }
    public object ViewModel { get; set; } = null!;
    public object View { get; set; } = null!;
}

public class NavigationFailedEventArgs : EventArgs
{
    public Type? ViewModelType { get; set; }
    public string? ViewName { get; set; }
    public Exception Exception { get; set; } = null!;
    public string ErrorMessage { get; set; } = string.Empty;
}
```

NAVIGATION SERVICE IMPLEMENTATION:

**Services/NavigationService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;
    
    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
    private readonly Dictionary<string, (Type ViewModelType, Type ViewType)> _viewNameMap = new();
    private readonly List<NavigationHistoryEntry> _navigationHistory = new();
    private int _currentHistoryIndex = -1;

    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Navigation Properties
    public Type? CurrentViewModelType { get; private set; }
    public string? CurrentViewName { get; private set; }
    public object? CurrentViewModel { get; private set; }
    
    public bool CanGoBack => _currentHistoryIndex > 0;
    public bool CanGoForward => _currentHistoryIndex < _navigationHistory.Count - 1;
    
    public IReadOnlyList<string> NavigationHistory => 
        _navigationHistory.Select(h => h.ViewName ?? h.ViewModelType.Name).ToList();

    // Events
    public event EventHandler<NavigationRequestedEventArgs>? NavigationRequested;
    public event EventHandler<NavigationCompletedEventArgs>? NavigationCompleted;
    public event EventHandler<NavigationFailedEventArgs>? NavigationFailed;

    // View Registration
    public void RegisterView<TViewModel, TView>() 
        where TViewModel : class 
        where TView : class
    {
        RegisterView(typeof(TViewModel), typeof(TView));
    }

    public void RegisterView(Type viewModelType, Type viewType)
    {
        if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
        if (viewType == null) throw new ArgumentNullException(nameof(viewType));

        _viewModelToViewMap[viewModelType] = viewType;
        _logger.LogDebug("Registered view mapping: {ViewModelType} -> {ViewType}", 
            viewModelType.Name, viewType.Name);
    }

    public void RegisterView(string viewName, Type viewModelType, Type viewType)
    {
        if (string.IsNullOrWhiteSpace(viewName)) throw new ArgumentException("View name cannot be null or empty", nameof(viewName));
        if (viewModelType == null) throw new ArgumentNullException(nameof(viewModelType));
        if (viewType == null) throw new ArgumentNullException(nameof(viewType));

        _viewNameMap[viewName] = (viewModelType, viewType);
        RegisterView(viewModelType, viewType);
        
        _logger.LogDebug("Registered named view mapping: {ViewName} -> {ViewModelType} -> {ViewType}", 
            viewName, viewModelType.Name, viewType.Name);
    }

    // Navigation Methods
    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        NavigateTo(typeof(TViewModel));
    }

    public void NavigateTo(Type viewModelType)
    {
        NavigateToInternal(viewModelType, null, null);
    }

    public void NavigateTo(string viewName)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            throw new ArgumentException("View name cannot be null or empty", nameof(viewName));

        if (!_viewNameMap.TryGetValue(viewName, out var mapping))
        {
            var error = $"View '{viewName}' is not registered";
            _logger.LogError(error);
            OnNavigationFailed(null, viewName, new InvalidOperationException(error));
            return;
        }

        NavigateToInternal(mapping.ViewModelType, viewName, null);
    }

    public void NavigateToWithParameter<TViewModel, TParameter>(TParameter parameter) where TViewModel : class
    {
        NavigateToInternal(typeof(TViewModel), null, parameter);
    }

    private void NavigateToInternal(Type viewModelType, string? viewName, object? parameter)
    {
        try
        {
            _logger.LogInformation("Navigating to {ViewModelType} (View: {ViewName})", 
                viewModelType.Name, viewName ?? "default");

            // Get view type
            if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
            {
                throw new InvalidOperationException($"No view registered for ViewModel '{viewModelType.Name}'");
            }

            // Create ViewModel instance
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);

            // Set parameter if provided and supported
            if (parameter != null && viewModel is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }

            // Create navigation request
            var requestArgs = new NavigationRequestedEventArgs
            {
                ViewModelType = viewModelType,
                ViewType = viewType,
                ViewName = viewName,
                Parameter = parameter,
                ViewModel = viewModel
            };

            // Fire navigation requested event
            NavigationRequested?.Invoke(this, requestArgs);

            // Check if navigation was cancelled
            if (requestArgs.Cancel)
            {
                _logger.LogInformation("Navigation to {ViewModelType} was cancelled", viewModelType.Name);
                return;
            }

            // Update current navigation state
            CurrentViewModelType = viewModelType;
            CurrentViewName = viewName;
            CurrentViewModel = viewModel;

            // Add to navigation history
            AddToHistory(viewModelType, viewType, viewName, viewModel);

            // Fire navigation completed event
            NavigationCompleted?.Invoke(this, new NavigationCompletedEventArgs
            {
                ViewModelType = viewModelType,
                ViewType = viewType,
                ViewName = viewName,
                ViewModel = viewModel,
                View = requestArgs.ViewModel // The actual view will be created by the UI layer
            });

            _logger.LogInformation("Successfully navigated to {ViewModelType}", viewModelType.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Navigation failed for {ViewModelType}", viewModelType.Name);
            OnNavigationFailed(viewModelType, viewName, ex);
        }
    }

    // Navigation History
    public void GoBack()
    {
        if (!CanGoBack)
        {
            _logger.LogWarning("Cannot go back - no previous navigation");
            return;
        }

        _currentHistoryIndex--;
        var entry = _navigationHistory[_currentHistoryIndex];
        
        _logger.LogInformation("Navigating back to {ViewModelType}", entry.ViewModelType.Name);
        NavigateToHistoryEntry(entry);
    }

    public void GoForward()
    {
        if (!CanGoForward)
        {
            _logger.LogWarning("Cannot go forward - no forward navigation");
            return;
        }

        _currentHistoryIndex++;
        var entry = _navigationHistory[_currentHistoryIndex];
        
        _logger.LogInformation("Navigating forward to {ViewModelType}", entry.ViewModelType.Name);
        NavigateToHistoryEntry(entry);
    }

    public void ClearHistory()
    {
        _navigationHistory.Clear();
        _currentHistoryIndex = -1;
        _logger.LogInformation("Navigation history cleared");
    }

    private void NavigateToHistoryEntry(NavigationHistoryEntry entry)
    {
        try
        {
            // Create fresh ViewModel instance for history navigation
            var viewModel = _serviceProvider.GetRequiredService(entry.ViewModelType);

            CurrentViewModelType = entry.ViewModelType;
            CurrentViewName = entry.ViewName;
            CurrentViewModel = viewModel;

            // Fire navigation completed event
            NavigationCompleted?.Invoke(this, new NavigationCompletedEventArgs
            {
                ViewModelType = entry.ViewModelType,
                ViewType = entry.ViewType,
                ViewName = entry.ViewName,
                ViewModel = viewModel,
                View = viewModel
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "History navigation failed for {ViewModelType}", entry.ViewModelType.Name);
            OnNavigationFailed(entry.ViewModelType, entry.ViewName, ex);
        }
    }

    private void AddToHistory(Type viewModelType, Type viewType, string? viewName, object viewModel)
    {
        // Remove any forward history if we're navigating to a new view
        if (_currentHistoryIndex < _navigationHistory.Count - 1)
        {
            _navigationHistory.RemoveRange(_currentHistoryIndex + 1, 
                _navigationHistory.Count - _currentHistoryIndex - 1);
        }

        // Add new entry
        var entry = new NavigationHistoryEntry
        {
            ViewModelType = viewModelType,
            ViewType = viewType,
            ViewName = viewName,
            Timestamp = DateTime.Now
        };

        _navigationHistory.Add(entry);
        _currentHistoryIndex = _navigationHistory.Count - 1;

        // Limit history size (optional)
        const int maxHistorySize = 50;
        if (_navigationHistory.Count > maxHistorySize)
        {
            _navigationHistory.RemoveAt(0);
            _currentHistoryIndex--;
        }
    }

    private void OnNavigationFailed(Type? viewModelType, string? viewName, Exception exception)
    {
        NavigationFailed?.Invoke(this, new NavigationFailedEventArgs
        {
            ViewModelType = viewModelType,
            ViewName = viewName,
            Exception = exception,
            ErrorMessage = exception.Message
        });
    }

    private class NavigationHistoryEntry
    {
        public Type ViewModelType { get; set; } = null!;
        public Type ViewType { get; set; } = null!;
        public string? ViewName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

// Interface for ViewModels that need navigation parameters
public interface INavigationAware
{
    void OnNavigatedTo(object? parameter);
    void OnNavigatedFrom();
}
```

MAIN WINDOW INTEGRATION:

**MainWindow.axaml.cs** (Updated to handle navigation):
```csharp
using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia.Views;

public partial class MainWindow : Window
{
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow()
    {
        InitializeComponent();
        
        // Get services from DI container
        _navigationService = Program.GetService<INavigationService>();
        _serviceProvider = Program.GetService<IServiceProvider>();
        _logger = Program.GetService<ILogger<MainWindow>>();

        // Subscribe to navigation events
        _navigationService.NavigationRequested += OnNavigationRequested;
        _navigationService.NavigationCompleted += OnNavigationCompleted;
        _navigationService.NavigationFailed += OnNavigationFailed;

        // Register views
        RegisterViews();

        // Set up initial view
        _navigationService.NavigateTo<MainViewModel>();
    }

    private void RegisterViews()
    {
        // Register all view mappings
        _navigationService.RegisterView<MainViewModel, MainView>();
        _navigationService.RegisterView<InventoryViewModel, InventoryView>();
        _navigationService.RegisterView<AddItemViewModel, AddItemView>();
        _navigationService.RegisterView<RemoveItemViewModel, RemoveItemView>();
        _navigationService.RegisterView<TransferItemViewModel, TransferItemView>();
        _navigationService.RegisterView<TransactionHistoryViewModel, TransactionHistoryView>();
        _navigationService.RegisterView<UserManagementViewModel, UserManagementView>();

        // Register named views for menu navigation
        _navigationService.RegisterView("Home", typeof(MainViewModel), typeof(MainView));
        _navigationService.RegisterView("Inventory", typeof(InventoryViewModel), typeof(InventoryView));
        _navigationService.RegisterView("AddItem", typeof(AddItemViewModel), typeof(AddItemView));
        _navigationService.RegisterView("RemoveItem", typeof(RemoveItemViewModel), typeof(RemoveItemView));
        _navigationService.RegisterView("Transfer", typeof(TransferItemViewModel), typeof(TransferItemView));
        _navigationService.RegisterView("History", typeof(TransactionHistoryViewModel), typeof(TransactionHistoryView));
        _navigationService.RegisterView("Users", typeof(UserManagementViewModel), typeof(UserManagementView));

        _logger.LogInformation("All views registered with navigation service");
    }

    private void OnNavigationRequested(object? sender, NavigationRequestedEventArgs e)
    {
        try
        {
            // Create the view instance
            var view = _serviceProvider.GetRequiredService(e.ViewType) as Control;
            if (view == null)
            {
                throw new InvalidOperationException($"Could not create view of type {e.ViewType.Name}");
            }

            // Set the ViewModel as DataContext
            view.DataContext = e.ViewModel;

            // Update the main content area (assuming ContentArea is a ContentControl)
            if (ContentArea != null)
            {
                ContentArea.Content = view;
            }

            _logger.LogDebug("View {ViewType} created and displayed", e.ViewType.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create view {ViewType}", e.ViewType.Name);
            e.Cancel = true;
        }
    }

    private void OnNavigationCompleted(object? sender, NavigationCompletedEventArgs e)
    {
        _logger.LogInformation("Navigation completed to {ViewModelType}", e.ViewModelType.Name);
        
        // Update window title if needed
        if (!string.IsNullOrEmpty(e.ViewName))
        {
            Title = $"MTM WIP Application - {e.ViewName}";
        }
    }

    private void OnNavigationFailed(object? sender, NavigationFailedEventArgs e)
    {
        _logger.LogError(e.Exception, "Navigation failed: {ErrorMessage}", e.ErrorMessage);
        
        // Show error to user (implement error display)
        // ShowErrorMessage($"Navigation failed: {e.ErrorMessage}");
    }
}
```

VIEWMODEL BASE CLASS UPDATE:

**ViewModels/BaseViewModel.cs** (Updated with Navigation):
```csharp
using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public abstract class BaseViewModel : ReactiveObject, INavigationAware, IDisposable
{
    protected readonly ILogger Logger;
    protected readonly INavigationService NavigationService;
    private bool _isDisposed = false;

    protected BaseViewModel(ILogger logger, INavigationService navigationService)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    // Navigation methods for ViewModels
    protected void NavigateTo<TViewModel>() where TViewModel : class
    {
        NavigationService.NavigateTo<TViewModel>();
    }

    protected void NavigateTo(string viewName)
    {
        NavigationService.NavigateTo(viewName);
    }

    protected void NavigateToWithParameter<TViewModel, TParameter>(TParameter parameter) where TViewModel : class
    {
        NavigationService.NavigateToWithParameter<TViewModel, TParameter>(parameter);
    }

    protected void GoBack()
    {
        NavigationService.GoBack();
    }

    // Navigation awareness (override in derived classes as needed)
    public virtual void OnNavigatedTo(object? parameter)
    {
        Logger.LogDebug("{ViewModelType} navigated to with parameter: {Parameter}", 
            GetType().Name, parameter?.ToString() ?? "null");
    }

    public virtual void OnNavigatedFrom()
    {
        Logger.LogDebug("{ViewModelType} navigated from", GetType().Name);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                OnNavigatedFrom();
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
```

VIEWMODEL USAGE EXAMPLE:

**ViewModels/MainViewModel.cs** (Updated with Navigation):
```csharp
using System.Reactive;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewModel : BaseViewModel
{
    public MainViewModel(
        INavigationService navigationService,
        ILogger<MainViewModel> logger) : base(logger, navigationService)
    {
        Logger.LogInformation("MainViewModel initialized with navigation service");
        InitializeCommands();
    }

    // Navigation Commands
    public ReactiveCommand<Unit, Unit> NavigateToInventoryCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> NavigateToAddItemCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> NavigateToRemoveItemCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> NavigateToTransferCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> NavigateToHistoryCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; private set; } = null!;

    private void InitializeCommands()
    {
        // Navigation commands using the navigation service
        NavigateToInventoryCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating to Inventory view");
            NavigateTo("Inventory");
        });

        NavigateToAddItemCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating to Add Item view");
            NavigateTo<AddItemViewModel>();
        });

        NavigateToRemoveItemCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating to Remove Item view");
            NavigateTo<RemoveItemViewModel>();
        });

        NavigateToTransferCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating to Transfer view");
            NavigateTo<TransferItemViewModel>();
        });

        NavigateToHistoryCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating to History view");
            NavigateTo("History");
        });

        GoBackCommand = ReactiveCommand.Create(() => 
        {
            Logger.LogInformation("Navigating back");
            GoBack();
        }, this.WhenAnyValue(vm => NavigationService.CanGoBack));
    }

    public override void OnNavigatedTo(object? parameter)
    {
        base.OnNavigatedTo(parameter);
        Logger.LogInformation("Main view displayed");
    }
}
```

DEPENDENCY INJECTION REGISTRATION:

**Program.cs** (Add Navigation Service Registration):
```csharp
// In ConfigureServices method
services.AddSingleton<INavigationService, NavigationService>();
```

After implementing navigation service, create Development/Services/README_NavigationService.md documenting:
- Navigation service usage patterns
- View registration procedures
- Navigation history management
- MVVM compliance guidelines
- Parameter passing patterns
- Error handling strategies
```

---

## Expected Deliverables

1. **INavigationService interface** with comprehensive navigation methods
2. **NavigationService implementation** with view registration and history
3. **MainWindow integration** to handle navigation events
4. **BaseViewModel updates** with navigation support
5. **Example ViewModel usage** demonstrating navigation patterns
6. **View registration system** mapping ViewModels to Views
7. **Navigation history management** with back/forward support
8. **Parameter passing support** for navigation with data
9. **MVVM compliance** eliminating direct view instantiation
10. **Error handling** for navigation failures

---

## Validation Steps

1. Verify ViewModels can navigate without directly instantiating views
2. Test view registration system works correctly
3. Confirm navigation history (back/forward) functions properly
4. Validate parameter passing between views works
5. Test error handling for invalid navigation requests
6. Verify MVVM patterns are properly maintained
7. Confirm logging captures navigation activities

---

## Success Criteria

- [ ] Complete navigation service interface and implementation
- [ ] ViewModels navigate through service, not direct view creation
- [ ] View registration system maps ViewModels to Views
- [ ] Navigation history supports back/forward navigation
- [ ] Parameter passing works for data transfer between views
- [ ] Error handling covers navigation failure scenarios
- [ ] MainWindow responds to navigation events properly
- [ ] MVVM patterns maintained throughout navigation
- [ ] Comprehensive logging of navigation activities
- [ ] Ready for complex navigation scenarios

---

*Priority: HIGH - Essential for proper MVVM compliance and maintainable navigation patterns.*