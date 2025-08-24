# Dependency Injection Setup Rules

## **CRITICAL: AddMTMServices Extension Method**

### **ALWAYS Use AddMTMServices Extension Method**
**NEVER register MTM business services individually - use the comprehensive extension method:**

```csharp
// ? CORRECT: Use comprehensive service registration
services.AddMTMServices(configuration);

// ? WRONG: Manual registration misses dependencies
services.AddScoped<MTM.Services.IInventoryService, MTM.Services.InventoryService>(); // Missing IValidationService dependency!
```

### **Required Service Registration Pattern in Program.cs**
```csharp
private static void ConfigureServices()
{
    var services = new ServiceCollection();

    // Configuration setup
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);

    // Logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    // ? CRITICAL: Use comprehensive MTM service registration
    services.AddMTMServices(configuration);

    // ? Override Avalonia-specific services AFTER AddMTMServices
    services.AddSingleton<MTM_WIP_Application_Avalonia.Services.IConfigurationService, 
                         MTM_WIP_Application_Avalonia.Services.ConfigurationService>();
    services.AddSingleton<MTM_WIP_Application_Avalonia.Services.IApplicationStateService, 
                         MTM_WIP_Application_Avalonia.Services.ApplicationStateService>();

    // Infrastructure Services (Avalonia-specific)
    services.AddSingleton<INavigationService, NavigationService>();

    // ViewModels (Transient - new instance each time)
    services.AddTransient<MainViewModel>();
    services.AddTransient<MainViewViewModel>();
    services.AddTransient<MainWindowViewModel>();
    services.AddTransient<InventoryViewModel>();
    services.AddTransient<AddItemViewModel>();
    services.AddTransient<RemoveItemViewModel>();
    services.AddTransient<TransferItemViewModel>();
    services.AddTransient<TransactionHistoryViewModel>();
    services.AddTransient<UserManagementViewModel>();

    // Build service provider
    _serviceProvider = services.BuildServiceProvider();
}
```

## **Why AddMTMServices is Required**

The `AddMTMServices` extension method (in `MTM.Extensions.ServiceCollectionExtensions`) registers ALL required dependencies:

**? Core Infrastructure Services**:
- `IDatabaseService` ? `DatabaseService`
- `IDbConnectionFactory` ? `MySqlConnectionFactory`
- `DatabaseTransactionService`

**? Business Services**:
- `IInventoryService` ? `InventoryService`
- `IUserService` ? `UserService`
- `ITransactionService` ? `TransactionService`

**? Supporting Services** (Critical - these are often missing):
- `IValidationService` ? `SimpleValidationService` ? **Required by InventoryService**
- `ICacheService` ? `SimpleCacheService` ? **Required by multiple services**
- `IConfigurationService` ? `ConfigurationService`
- `IApplicationStateService` ? `MTMApplicationStateService`

**? Validation and Options**:
- `IValidateOptions<MTMSettings>` ? `ConfigurationValidationService`
- `IMemoryCache` for caching support

## **Common DI Registration Errors to Avoid**

### ? **Error 1: Missing IValidationService**
```csharp
// This will fail at runtime:
services.AddScoped<MTM.Services.IInventoryService, MTM.Services.InventoryService>();
// Error: Unable to resolve service for type 'MTM.Core.Services.IValidationService'
```

### ? **Error 2: Missing ICacheService**
```csharp
// This will fail if services depend on caching:
services.AddScoped<MTM.Services.IUserService, MTM.Services.UserService>();
// Error: Unable to resolve service for type 'MTM.Core.Services.ICacheService'
```

### ? **Error 3: Missing ViewModel Registration**
```csharp
// This will fail when App.axaml.cs tries to resolve:
var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
// Error: No service for type 'MainWindowViewModel' has been registered.
```

## **Service Lifetime Guidelines**

**Singleton Services** (Created once, shared):
- Database services (`IDatabaseService`)
- Configuration services (`IConfigurationService`)
- Navigation services (`INavigationService`)
- Application state services (`IApplicationStateService`)
- Caching services (`ICacheService`)

**Scoped Services** (Created per logical operation):
- Business services (`IInventoryService`, `IUserService`, `ITransactionService`)
- Validation services (`IValidationService`)

**Transient Services** (Created each time requested):
- All ViewModels (UI components should be fresh instances)
- Short-lived utility services

## **Required Using Statements**
```csharp
using MTM.Extensions; // For AddMTMServices extension method
using MTM_WIP_Application_Avalonia.Services.Interfaces; // For Avalonia-specific interfaces
```

## **Service Resolution in ViewModels**

All ViewModels should use constructor injection:

```csharp
public class InventoryViewModel : BaseViewModel
{
    private readonly MTM.Services.IInventoryService _inventoryService;
    private readonly INavigationService _navigationService;

    public InventoryViewModel(
        MTM.Services.IInventoryService inventoryService,
        INavigationService navigationService,
        ILogger<InventoryViewModel> logger) : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        
        Logger.LogInformation("InventoryViewModel initialized with dependency injection");
    }
}
```

## **Debugging DI Issues**

1. **Check service registration order** - `AddMTMServices` must be called before service overrides
2. **Verify all ViewModels are registered** - Each ViewModel needs `services.AddTransient<ViewModelName>()`
3. **Check using statements** - Ensure `using MTM.Extensions;` is included
4. **Review error messages** - DI errors clearly indicate missing service types
5. **Use GetOptionalService for debugging** - Test if services can be resolved

## **Testing Service Registration**

Add this to validate all services can be resolved:

```csharp
private static void ValidateServiceRegistration()
{
    try
    {
        // Test critical services
        var dbService = Program.GetService<MTM.Core.Services.IDatabaseService>();
        var inventoryService = Program.GetService<MTM.Services.IInventoryService>();
        var validationService = Program.GetService<MTM.Core.Services.IValidationService>();
        var cacheService = Program.GetService<MTM.Core.Services.ICacheService>();
        
        // Test ViewModels
        var mainViewModel = Program.GetService<MainViewModel>();
        var inventoryViewModel = Program.GetService<InventoryViewModel>();
        
        Console.WriteLine("? All services resolved successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Service resolution failed: {ex.Message}");
        throw;
    }
}
```