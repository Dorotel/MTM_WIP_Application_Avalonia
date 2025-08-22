# Custom Prompt: Setup Missing Dependency Injection Container

## 🚨 CRITICAL PRIORITY FIX #6

**Issue**: No dependency injection container is configured, preventing proper service injection and loose coupling.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- `Program.cs` - No DI container setup
- `App.axaml.cs` - No service registration
- ViewModels - No service injection
- Services - No interface implementations registered

**Priority**: 🚨 **CRITICAL - ARCHITECTURE FOUNDATION**

---

## Custom Prompt

```
CRITICAL ARCHITECTURE SETUP: Configure Microsoft.Extensions.DependencyInjection container to enable proper service injection, loose coupling, and testability throughout the MTM WIP Application.

REQUIREMENTS:
1. Install Microsoft.Extensions.DependencyInjection and related packages
2. Configure service container in Program.cs with proper lifetime management
3. Register all service interfaces and implementations
4. Setup logging and configuration services
5. Enable ViewModel dependency injection
6. Configure service lifetimes appropriately (Singleton, Scoped, Transient)
7. Prepare for future service additions

NUGET PACKAGES TO INSTALL:
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options.ConfigurationExtensions" Version="8.0.0" />
```

UPDATED Program.cs:
```csharp
using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels;

namespace MTM_WIP_Application_Avalonia;

public static class Program
{
    private static IServiceProvider? _serviceProvider;

    public static void Main(string[] args) 
    {
        // Setup dependency injection before starting the application
        ConfigureServices();
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Core Services (Singleton - shared across application)
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        // Business Services (Scoped - per operation/request scope)
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITransactionService, TransactionService>();

        // Infrastructure Services (Singleton - stateless utilities)
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();

        // ViewModels (Transient - new instance each time)
        services.AddTransient<MainViewModel>();
        services.AddTransient<InventoryViewModel>();
        services.AddTransient<AddItemViewModel>();
        services.AddTransient<RemoveItemViewModel>();
        services.AddTransient<TransferItemViewModel>();
        services.AddTransient<TransactionHistoryViewModel>();
        services.AddTransient<UserManagementViewModel>();

        // Build service provider
        _serviceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Get service instance from DI container
    /// </summary>
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServices first.");

        return _serviceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Get service instance with optional fallback
    /// </summary>
    public static T? GetOptionalService<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }

    /// <summary>
    /// Create scope for scoped services (useful for operations that need isolated service instances)
    /// </summary>
    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");

        return _serviceProvider.CreateScope();
    }
}
```

UPDATED App.axaml.cs:
```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Views;

namespace MTM_WIP_Application_Avalonia;

public partial class App : Application
{
    private ILogger<App>? _logger;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            // Initialize logging
            _logger = Program.GetService<ILogger<App>>();
            _logger?.LogInformation("MTM WIP Application starting...");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Create MainWindow with injected ViewModel
                var mainViewModel = Program.GetService<MainViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };

                _logger?.LogInformation("Main window created with dependency injection");
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during application initialization");
            throw;
        }
    }
}
```

SERVICE INTERFACE DEFINITIONS:

**Services/IConfigurationService.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Services;

public interface IConfigurationService
{
    string GetConnectionString(string name = "DefaultConnection");
    T GetValue<T>(string key, T defaultValue = default!);
    string GetValue(string key, string defaultValue = "");
    bool GetBoolValue(string key, bool defaultValue = false);
    int GetIntValue(string key, int defaultValue = 0);
}
```

**Services/ConfigurationService.cs**:
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString(string name = "DefaultConnection")
    {
        var connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string '{Name}' not found, falling back to Model_AppVariables", name);
            return Model_AppVariables.ConnectionString;
        }
        return connectionString;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            if (value == null)
            {
                _logger.LogDebug("Configuration key '{Key}' not found, using default value", key);
                return defaultValue;
            }

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading configuration key '{Key}', using default value", key);
            return defaultValue;
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return GetValue<string>(key, defaultValue);
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        return GetValue<bool>(key, defaultValue);
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        return GetValue<int>(key, defaultValue);
    }
}
```

**Services/INavigationService.cs**:
```csharp
using System;
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Services;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo(Type viewModelType);
    void NavigateTo(string viewName);
    void GoBack();
    bool CanGoBack { get; }
    event EventHandler<NavigationEventArgs>? NavigationRequested;
}

public class NavigationEventArgs : EventArgs
{
    public Type ViewModelType { get; set; } = null!;
    public string? ViewName { get; set; }
}
```

**Services/NavigationService.cs**:
```csharp
using System;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool CanGoBack { get; private set; }

    public event EventHandler<NavigationEventArgs>? NavigationRequested;

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        NavigateTo(typeof(TViewModel));
    }

    public void NavigateTo(Type viewModelType)
    {
        _logger.LogInformation("Navigating to {ViewModelType}", viewModelType.Name);
        
        NavigationRequested?.Invoke(this, new NavigationEventArgs
        {
            ViewModelType = viewModelType
        });
    }

    public void NavigateTo(string viewName)
    {
        _logger.LogInformation("Navigating to view {ViewName}", viewName);
        
        NavigationRequested?.Invoke(this, new NavigationEventArgs
        {
            ViewName = viewName
        });
    }

    public void GoBack()
    {
        if (CanGoBack)
        {
            _logger.LogInformation("Navigating back");
            // TODO: Implement back navigation
        }
    }
}
```

**Services/IApplicationStateService.cs**:
```csharp
using System;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IApplicationStateService
{
    string CurrentUser { get; set; }
    string CurrentLocation { get; set; }
    string CurrentOperation { get; set; }
    bool IsOfflineMode { get; set; }
    event EventHandler<StateChangedEventArgs>? StateChanged;
}

public class StateChangedEventArgs : EventArgs
{
    public string PropertyName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
```

**Services/ApplicationStateService.cs**:
```csharp
using System;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public class ApplicationStateService : IApplicationStateService
{
    private readonly ILogger<ApplicationStateService> _logger;
    private string _currentUser = string.Empty;
    private string _currentLocation = string.Empty;
    private string _currentOperation = string.Empty;
    private bool _isOfflineMode = false;

    public ApplicationStateService(ILogger<ApplicationStateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value, nameof(CurrentUser));
    }

    public string CurrentLocation
    {
        get => _currentLocation;
        set => SetProperty(ref _currentLocation, value, nameof(CurrentLocation));
    }

    public string CurrentOperation
    {
        get => _currentOperation;
        set => SetProperty(ref _currentOperation, value, nameof(CurrentOperation));
    }

    public bool IsOfflineMode
    {
        get => _isOfflineMode;
        set => SetProperty(ref _isOfflineMode, value, nameof(IsOfflineMode));
    }

    public event EventHandler<StateChangedEventArgs>? StateChanged;

    private void SetProperty<T>(ref T field, T value, string propertyName)
    {
        if (!Equals(field, value))
        {
            var oldValue = field;
            field = value;
            
            _logger.LogDebug("Application state changed: {PropertyName} = {NewValue} (was {OldValue})", 
                propertyName, value, oldValue);
            
            StateChanged?.Invoke(this, new StateChangedEventArgs
            {
                PropertyName = propertyName,
                OldValue = oldValue,
                NewValue = value
            });
        }
    }
}
```

VIEWMODEL BASE CLASS WITH DI SUPPORT:

**ViewModels/BaseViewModel.cs**:
```csharp
using System;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public abstract class BaseViewModel : ReactiveObject, IDisposable
{
    protected readonly ILogger Logger;
    private bool _isDisposed = false;

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources
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

UPDATED MainViewModel WITH DI:

**ViewModels/MainViewModel.cs**:
```csharp
using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly IInventoryService _inventoryService;

    public MainViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        IInventoryService inventoryService,
        ILogger<MainViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));

        Logger.LogInformation("MainViewModel initialized with dependency injection");

        // Initialize commands and setup
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}
```

SERVICE LIFETIME GUIDELINES:

**Singleton Services** (Created once, shared across application):
- IDatabaseService - Database connection management
- IConfigurationService - Application settings
- INavigationService - Navigation state
- IApplicationStateService - Global application state

**Scoped Services** (Created per logical operation):
- IInventoryService - Business operations with state
- IUserService - User operations with context
- ITransactionService - Transaction operations with logging

**Transient Services** (Created each time requested):
- ViewModels - UI components should be fresh instances
- Short-lived utility services

ERROR HANDLING AND VALIDATION:
```csharp
// In service registration, validate critical services
services.AddSingleton<IDatabaseService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<DatabaseService>>();
    var config = provider.GetRequiredService<IConfigurationService>();
    
    // Validate database connectivity on startup
    var service = new DatabaseService(logger);
    // TODO: Add connectivity test
    
    return service;
});
```

TESTING SUPPORT:
Add extension method for test service registration:
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        // Replace with mock implementations for testing
        services.AddScoped<IDatabaseService, MockDatabaseService>();
        return services;
    }
}
```

After implementing DI container, create Development/DependencyInjection/README_DependencyInjection.md documenting:
- Service registration patterns
- Lifetime management guidelines
- How to add new services
- Testing with DI
- ViewmModel injection patterns
```

---

## Expected Deliverables

1. **Updated Program.cs** with complete DI container configuration
2. **Updated App.axaml.cs** with service-injected ViewModels
3. **IConfigurationService and implementation** for settings management
4. **INavigationService and implementation** for MVVM navigation
5. **IApplicationStateService and implementation** for global state
6. **BaseViewModel class** with logging and DI support
7. **Updated MainViewModel** demonstrating service injection
8. **Service lifetime configuration** with appropriate scopes
9. **NuGet package additions** for Microsoft.Extensions.DependencyInjection
10. **Documentation** for DI patterns and service registration

---

## Validation Steps

1. Verify application starts successfully with DI container
2. Test service injection in ViewModels works correctly
3. Confirm configuration service reads from appsettings.json
4. Validate logging works throughout the application
5. Test navigation service integration
6. Verify service lifetimes work as expected
7. Confirm all services can be resolved without circular dependencies

---

## Success Criteria

- [ ] Microsoft.Extensions.DependencyInjection properly configured
- [ ] All service interfaces and implementations registered
- [ ] ViewModels receive services via constructor injection
- [ ] Configuration service provides access to appsettings.json
- [ ] Logging works throughout the application
- [ ] Navigation service enables MVVM-compliant navigation
- [ ] Application state service manages global state
- [ ] Service lifetimes configured appropriately
- [ ] No circular dependencies in service registration
- [ ] Ready for business logic implementation with injected services

---

*Priority: CRITICAL - Foundation for loose coupling, testability, and proper MVVM architecture.*