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

## **?? SERVICE ORGANIZATION RULE - CRITICAL**

### **Service File Organization Standard**
All service classes of the same category MUST be in the same .cs file. Interfaces remain in the `Services/Interfaces/` folder.

**? CORRECT Service File Structure**:
```
Services/
??? Interfaces/              # All service interfaces (separate files)
?   ??? IUserService.cs      # Single interface per file
?   ??? IUserValidationService.cs
?   ??? IUserAuditService.cs
?   ??? IInventoryService.cs
?   ??? ITransactionService.cs
??? UserServices.cs          # ALL user-related implementations
??? InventoryServices.cs     # ALL inventory-related implementations
??? TransactionServices.cs   # ALL transaction-related implementations
??? LocationServices.cs      # ALL location-related implementations
```

**? CORRECT Implementation Pattern**:
```csharp
// File: Services/UserServices.cs
namespace MTM.Services
{
    /// <summary>
    /// Primary user management service
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IValidationService _validationService;
        
        public UserService(IDatabaseService databaseService, IValidationService validationService)
        {
            _databaseService = databaseService;
            _validationService = validationService;
        }
        
        // User service implementation
    }

    /// <summary>
    /// User validation service
    /// </summary>
    public class UserValidationService : IUserValidationService
    {
        private readonly IDatabaseService _databaseService;
        
        public UserValidationService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        
        // User validation implementation
    }

    /// <summary>
    /// User audit and activity tracking service
    /// </summary>
    public class UserAuditService : IUserAuditService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<UserAuditService> _logger;
        
        public UserAuditService(IDatabaseService databaseService, ILogger<UserAuditService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }
        
        // User audit implementation
    }
}
```

**? INCORRECT Organization**:
```csharp
// WRONG: Separate files for related services
// File: Services/UserService.cs - Only UserService
// File: Services/UserValidationService.cs - Only UserValidationService 
// File: Services/UserAuditService.cs - Only UserAuditService
```

### **Service Category Guidelines**:
- **UserServices.cs**: User management, authentication, preferences, audit, validation
- **InventoryServices.cs**: Inventory CRUD, validation, reporting, analysis
- **TransactionServices.cs**: Transaction processing, history, validation, reporting
- **LocationServices.cs**: Location management, validation, hierarchy
- **SystemServices.cs**: Configuration, caching, logging, error handling

### **DI Registration Pattern for Grouped Services**:
```csharp
// File: Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all services from each category file
        
        // User Services (from UserServices.cs)
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserValidationService, UserValidationService>();
        services.AddScoped<IUserAuditService, UserAuditService>();
        
        // Inventory Services (from InventoryServices.cs)
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IInventoryValidationService, InventoryValidationService>();
        services.AddScoped<IInventoryReportService, InventoryReportService>();
        
        // Transaction Services (from TransactionServices.cs)
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();
        services.AddScoped<ITransactionValidationService, TransactionValidationService>();
        
        return services;
    }
}
```

### **? IMPLEMENTED - Complete Service Registration Pattern in Program.cs**
```csharp
private static void ConfigureServices()
{
    var services = new ServiceCollection();

    // Configuration setup
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
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
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    services.AddSingleton<IApplicationStateService, ApplicationStateService>();

    // Infrastructure Services (Singleton - stateless utilities)
    services.AddSingleton<INavigationService, NavigationService>();

    // ViewModels (Transient - new instance each time)
    services.AddTransient<MainViewModel>();
    services.AddTransient<MainViewViewModel>();
    services.AddTransient<MainWindowViewModel>(); // ? CRITICAL: Must register ALL ViewModels
    services.AddTransient<InventoryViewModel>();
    services.AddTransient<AddItemViewModel>();
    services.AddTransient<RemoveItemViewModel>();
    services.AddTransient<TransferItemViewModel>();
    services.AddTransient<TransactionHistoryViewModel>();
    services.AddTransient<UserManagementViewModel>();

    // Build service provider
    _serviceProvider = services.BuildServiceProvider();

    #if DEBUG
    ValidateServiceRegistration(); // ? Validate all services can be resolved
    #endif
}
```

## **? IMPLEMENTED - MTM Service Registration Infrastructure**

### **AddMTMServices Extension Method Details**
Located in `Extensions/ServiceCollectionExtensions.cs`, this method registers:

**? Core Infrastructure Services**:
- `IDatabaseService` ? `DatabaseService` (Scoped)
- `IDbConnectionFactory` ? `MySqlConnectionFactory` (Singleton)
- `DatabaseTransactionService` (Transient)

**? Business Services** (organized by category):
- **User Services**: `IUserService` ? `UserService`, `IUserValidationService` ? `UserValidationService`, `IUserAuditService` ? `UserAuditService` (Scoped)
- **Inventory Services**: `IInventoryService` ? `InventoryService`, `IInventoryValidationService` ? `InventoryValidationService` (Scoped) - **COMPLETE IMPLEMENTATION**
- **Transaction Services**: `ITransactionService` ? `TransactionService`, `ITransactionHistoryService` ? `TransactionHistoryService` (Scoped)

**? Supporting Services** (Critical dependencies):
- `IValidationService` ? `SimpleValidationService` (Scoped) - **Required by multiple services**
- `ICacheService` ? `SimpleCacheService` (Singleton) - **Required by multiple services**
- `IConfigurationService` ? `ConfigurationService` (Singleton)
- `IApplicationStateService` ? `MTMApplicationStateService` (Singleton)

**? Validation and Configuration**:
- `IValidateOptions<MTMSettings>` ? `ConfigurationValidationService`
- `IMemoryCache` for caching support
- Strongly-typed configuration binding for MTM, Database, ErrorHandling, and Logging sections

### **? Helper_Database_StoredProcedure Integration**
The database access layer is now properly initialized:

```csharp
// In App.axaml.cs - OnFrameworkInitializationCompleted()
var configuration = Program.GetService<IConfiguration>();
Model_AppVariables.Initialize(configuration);

var loggerFactory = Program.GetService<ILoggerFactory>();
var generalLogger = loggerFactory.CreateLogger("Helper_Database_StoredProcedure");
Helper_Database_StoredProcedure.SetLogger(generalLogger);
```

## **Common DI Registration Errors to Avoid**

### ? **Error 1: Missing IValidationService**
```csharp
// This will fail at runtime:
services.AddScoped<MTM.Services.IInventoryService, MTM.Services.InventoryService>();
// Error: Unable to resolve service for type 'MTM.Core.Services.IValidationService'
```
**? Solution**: Use `services.AddMTMServices(configuration);`

### ? **Error 2: Missing ICacheService**
```csharp
// This will fail if services depend on caching:
services.AddScoped<MTM.Services.IUserService, MTM.Services.UserService>();
// Error: Unable to resolve service for type 'MTM.Core.Services.ICacheService'
```
**? Solution**: Use `services.AddMTMServices(configuration);`

### ? **Error 3: Missing ViewModel Registration**
```csharp
// This will fail when App.axaml.cs tries to resolve:
var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
// Error: No service for type 'MainWindowViewModel' has been registered.
```
**? Solution**: Add `services.AddTransient<ViewModelName>();` for each ViewModel

### ? **Error 4: Missing Using Statement**
```csharp
// CS0103: The name 'AddMTMServices' does not exist in the current context
```
**? Solution**: Add `using MTM.Extensions;` at the top of Program.cs

### ? **Error 5: Incorrect Service File Organization**
```csharp
// WRONG: Separate files for related services
// This makes dependency management difficult and violates MTM standards
```
**? Solution**: Group related services in category files as shown above

## **? IMPLEMENTED - Service Lifetime Guidelines**

**Singleton Services** (Created once, shared):
- ? Database connection factory (`IDbConnectionFactory`)
- ? Configuration services (`IConfigurationService`) 
- ? Navigation services (`INavigationService`)
- ? Application state services (`IApplicationStateService`)
- ? Caching services (`ICacheService`)

**Scoped Services** (Created per logical operation):
- ? Database services (`IDatabaseService`)
- ? Business services (`IInventoryService`, `IUserService`, `ITransactionService`, etc.)
- ? Validation services (`IValidationService`)

**Transient Services** (Created each time requested):
- ? All ViewModels (UI components should be fresh instances)
- ? Database transaction services (`DatabaseTransactionService`)

## **? IMPLEMENTED - Required Using Statements**
```csharp
using MTM.Extensions; // ? For AddMTMServices extension method
using MTM_WIP_Application_Avalonia.Services; // ? For Avalonia-specific services
using MTM_WIP_Application_Avalonia.ViewModels; // ? For ViewModel registration
using MTM_WIP_Application_Avalonia.ViewModels.MainForm; // ? For specific ViewModels
```

## **? IMPLEMENTED - Service Resolution in ViewModels**

All ViewModels should use constructor injection pattern:

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

## **? IMPLEMENTED - Service Registration Validation**

The following validation method is included in Program.cs:

```csharp
#if DEBUG
private static void ValidateServiceRegistration()
{
    try
    {
        // Test MTM Core Services
        var dbService = GetService<MTM.Core.Services.IDatabaseService>();
        var validationService = GetService<MTM.Core.Services.IValidationService>();
        var cacheService = GetService<MTM.Core.Services.ICacheService>();
        
        // Test MTM Business Services (from category files)
        var inventoryService = GetService<MTM.Services.IInventoryService>();
        var userService = GetService<MTM.Services.IUserService>();
        var transactionService = GetService<MTM.Services.ITransactionService>();
        
        // Test Avalonia Services
        var navigationService = GetService<INavigationService>();
        var configService = GetService<IConfigurationService>();
        var appStateService = GetService<IApplicationStateService>();
        
        // Test ViewModels
        var mainViewModel = GetService<MainViewModel>();
        var mainViewViewModel = GetService<MainViewViewModel>();
        var mainWindowViewModel = GetService<MainWindowViewModel>();
        var inventoryViewModel = GetService<InventoryViewModel>();
        
        Console.WriteLine("? All services resolved successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Service resolution failed: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        throw;
    }
}
#endif
```

## **Debugging DI Issues**

1. **? Check service registration order** - `AddMTMServices` must be called before service overrides
2. **? Verify all ViewModels are registered** - Each ViewModel needs `services.AddTransient<ViewModelName>()`
3. **? Check using statements** - Ensure `using MTM.Extensions;` is included
4. **? Review error messages** - DI errors clearly indicate missing service types
5. **? Use validation method** - `ValidateServiceRegistration()` tests if all services can be resolved
6. **?? Check service file organization** - Ensure related services are grouped in category files

## **Configuration Integration**

The DI system now properly integrates with configuration:

```csharp
// ? Strongly-typed configuration binding
services.Configure<MTMSettings>(configuration.GetSection("MTM"));
services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
services.Configure<ErrorHandlingSettings>(configuration.GetSection("ErrorHandling"));
services.Configure<LoggingSettings>(configuration.GetSection("Logging"));

// ? Configuration validation
services.AddSingleton<IValidateOptions<MTMSettings>, ConfigurationValidationService>();
```

## **Service Organization Benefits**

### **?? Advantages of Category-Based Service Files**:
1. **Logical Grouping**: Related functionality stays together
2. **Easier Maintenance**: Single location for category changes
3. **Reduced File Clutter**: Fewer files to navigate in Solution Explorer
4. **Better Dependency Management**: Clear visibility of related services
5. **Simplified Testing**: Category-based test organization
6. **Consistent Registration**: All related services registered together

### **?? Maintenance Guidelines**:
- Add new services to appropriate category files
- Keep interfaces separate for contract clarity
- Use XML documentation for each service class
- Follow consistent naming patterns within categories
- Group related using statements at file level

## **Build Status**

? **VALIDATED**: All DI configuration compiles successfully  
? **TESTED**: Service resolution validation passes  
? **INTEGRATED**: Model_AppVariables and Helper_Database_StoredProcedure properly initialized  
? **DOCUMENTED**: Complete service registration patterns established  
?? **ORGANIZED**: Service file organization standard implemented

**Next Phase**: Ready to implement Phase 2 services using the established DI infrastructure and category-based organization.