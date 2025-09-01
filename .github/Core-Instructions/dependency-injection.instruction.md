# Dependency Injection Setup Rules

<details>

<details>
<summary><strong>📑 Table of Contents</strong></summary>

- [CRITICAL: AddMTMServices Extension Method](#critical-addmtmservices-extension-method)
- [SERVICE ORGANIZATION RULE - CRITICAL](#service-organization-rule---critical)
- [DI Registration Pattern for Grouped Services](#di-registration-pattern-for-grouped-services)
- [IMPLEMENTED - Complete Service Registration Pattern in Program.cs](#implemented---complete-service-registration-pattern-in-programcs)
- [Common DI Registration Errors to Avoid](#common-di-registration-errors-to-avoid)
- [IMPLEMENTED - Service Lifetime Guidelines](#implemented---service-lifetime-guidelines)
- [IMPLEMENTED - Required Using Statements](#implemented---required-using-statements)
- [IMPLEMENTED - Service Resolution in ViewModels](#implemented---service-resolution-in-viewmodels)
- [IMPLEMENTED - Service Registration Validation](#implemented---service-registration-validation)
- [Debugging DI Issues](#debugging-di-issues)
- [Configuration Integration](#configuration-integration)
- [Service Organization Benefits](#service-organization-benefits)
- [Build Status](#build-status)

</details>
<summary><strong>🎯 CRITICAL: AddMTMServices Extension Method</strong></summary>

### **ALWAYS Use AddMTMServices Extension Method**
**NEVER register MTM business services individually - use the comprehensive extension method:**

```csharp
// ✅ CORRECT: Use comprehensive service registration
services.AddMTMServices(configuration);

// ❌ WRONG: Manual registration misses dependencies
services.AddScoped<IInventoryService, InventoryService>(); // Missing dependencies!
```

</details>

<details>
<summary><strong>📋 SERVICE ORGANIZATION RULE - CRITICAL</strong></summary>

### **Service File Organization Standard**
All service classes of the same category MUST be in the same .cs file.

**✅ CORRECT Service File Structure**:
```
Services/
├── ErrorHandling.cs          # ALL error handling functionality
├── Configuration.cs          # Configuration and app state
├── Navigation.cs             # Navigation service
└── Database.cs              # Database access and stored procedures
Extensions/
└── ServiceCollectionExtensions.cs  # Clean DI registration
```

**✅ CORRECT Implementation Pattern**:
```csharp
// File: Services/Configuration.cs
namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Configuration service interface
    /// </summary>
    public interface IConfigurationService
    {
        string GetConnectionString(string name = "DefaultConnection");
        T GetValue<T>(string key, T defaultValue = default!);
    }

    /// <summary>
    /// Configuration service implementation
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationService> _logger;
        
        public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        
        // Implementation
    }

    /// <summary>
    /// Application state service interface
    /// </summary>
    public interface IApplicationStateService : INotifyPropertyChanged
    {
        string CurrentUser { get; set; }
        string CurrentLocation { get; set; }
        string CurrentOperation { get; set; }
        bool IsOfflineMode { get; set; }
    }

    /// <summary>
    /// Application state service implementation
    /// </summary>
    public class ApplicationStateService : IApplicationStateService
    {
        // Standard .NET INotifyPropertyChanged implementation
    }
}
```

**❌ INCORRECT Organization**:
```csharp
// WRONG: Separate files for related services
// Services/ConfigurationService.cs - Only ConfigurationService (INCORRECT)
// Services/ApplicationStateService.cs - Only ApplicationStateService (INCORRECT)
```

### **Service Category Guidelines**:
- **ErrorHandling.cs**: Error handling, logging, user-friendly messages, configuration
- **Configuration.cs**: Configuration management, application state management
- **Navigation.cs**: Application navigation service
- **Database.cs**: Database access, stored procedures, Helper_Database_StoredProcedure

</details>

<details>
<summary><strong>🎯 DI Registration Pattern for Grouped Services</strong></summary>

### **Current Clean Implementation**
```csharp
// File: Extensions/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all MTM services to the service collection.
    /// Clean, simple registration of only the services that exist and work.
    /// </summary>
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure services
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        
        // Database services
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        
        // ViewModels - register only those that exist and compile
        services.TryAddTransient<InventoryTabViewModel>();
        services.TryAddTransient<AdvancedRemoveViewModel>();

        return services;
    }

    // Helper methods for TryAdd functionality
    public static IServiceCollection TryAddTransient<TService>(this IServiceCollection services)
        where TService : class
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddTransient<TService>();
        }
        return services;
    }

    public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddSingleton<TService, TImplementation>();
        }
        return services;
    }

    public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddScoped<TService, TImplementation>();
        }
        return services;
    }
}
```

</details>

<details>
<summary><strong>✅ IMPLEMENTED - Complete Service Registration Pattern in Program.cs</strong></summary>

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

    // ✅ CRITICAL: Use comprehensive MTM service registration
    services.AddMTMServices(configuration);

    // Infrastructure Services (Singleton - stateless utilities)
    services.AddSingleton<INavigationService, NavigationService>();

    // ViewModels (Transient - new instance each time)
    services.AddTransient<InventoryTabViewModel>();
    services.AddTransient<AdvancedRemoveViewModel>();
    // Add other ViewModels as they are converted to standard .NET patterns

    // Build service provider
    _serviceProvider = services.BuildServiceProvider();

    #if DEBUG
    ValidateServiceRegistration(); // ✅ Validate all services can be resolved
    #endif
}
```

</details>

<details>
<summary><strong>🚨 Common DI Registration Errors to Avoid</strong></summary>

### ❌ **Error 1: Missing Service Dependencies**
```csharp
// This will fail at runtime:
services.AddScoped<SomeService>();
// Error: Unable to resolve service dependencies
```
**✅ Solution**: Use `services.AddMTMServices(configuration);`

### ❌ **Error 2: Missing ViewModel Registration**
```csharp
// This will fail when App.axaml.cs tries to resolve:
var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
// Error: No service for type 'MainWindowViewModel' has been registered.
```
**✅ Solution**: Add `services.TryAddTransient<ViewModelName>();` for each ViewModel

### ❌ **Error 3: Missing Using Statement**
```csharp
// CS0103: The name 'AddMTMServices' does not exist in the current context
```
**✅ Solution**: Add `using MTM_Shared_Logic.Extensions;` at the top of Program.cs

### ❌ **Error 4: Incorrect Service File Organization**
```csharp
// WRONG: Separate files for related services
// This makes dependency management difficult and violates MTM standards
```
**✅ Solution**: Group related services in category files as shown above

</details>

<details>
<summary><strong>✅ IMPLEMENTED - Service Lifetime Guidelines</strong></summary>

**Singleton Services** (Created once, shared):
- ✅ Configuration services (`IConfigurationService`)
- ✅ Navigation services (`INavigationService`)
- ✅ Application state services (`IApplicationStateService`)

**Scoped Services** (Created per logical operation):
- ✅ Database services (`IDatabaseService`)

**Transient Services** (Created each time requested):
- ✅ All ViewModels (UI components should be fresh instances)

</details>

<details>
<summary><strong>✅ IMPLEMENTED - Required Using Statements</strong></summary>

```csharp
using MTM_Shared_Logic.Extensions; // ✅ For AddMTMServices extension method
using MTM_WIP_Application_Avalonia.Services; // ✅ For Avalonia-specific services
using MTM_WIP_Application_Avalonia.ViewModels.MainForm; // ✅ For ViewModel registration
```

</details>

<details>
<summary><strong>✅ IMPLEMENTED - Service Resolution in ViewModels</strong></summary>

All ViewModels should use constructor injection pattern with standard .NET:

```csharp
public class InventoryTabViewModel : BaseViewModel, INotifyPropertyChanged
{
    private readonly IApplicationStateService _applicationStateService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;

    public InventoryTabViewModel(
        IApplicationStateService applicationStateService,
        INavigationService navigationService,
        IDatabaseService databaseService,
        IConfigurationService configurationService) : base()
    {
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        
        Logger.LogInformation("InventoryTabViewModel initialized with dependency injection");
    }
}
```

</details>

<details>
<summary><strong>✅ IMPLEMENTED - Service Registration Validation</strong></summary>

The following validation method is included in Program.cs:

```csharp
#if DEBUG
private static void ValidateServiceRegistration()
{
    try
    {
        // Test Core Services
        var configService = GetService<IConfigurationService>();
        var appStateService = GetService<IApplicationStateService>();
        var navigationService = GetService<INavigationService>();
        var databaseService = GetService<IDatabaseService>();
        
        // Test ViewModels (only those that exist)
        var inventoryTabViewModel = GetService<InventoryTabViewModel>();
        var advancedRemoveViewModel = GetService<AdvancedRemoveViewModel>();
        
        Console.WriteLine("✅ All services resolved successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Service resolution failed: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        throw;
    }
}
#endif
```

</details>

<details>
<summary><strong>🔧 Debugging DI Issues</strong></summary>

1. **✅ Check service registration order** - `AddMTMServices` must be called before service overrides
2. **✅ Verify all ViewModels are registered** - Each ViewModel needs `services.TryAddTransient<ViewModelName>()`
3. **✅ Check using statements** - Ensure `using MTM_Shared_Logic.Extensions;` is included
4. **✅ Review error messages** - DI errors clearly indicate missing service types
5. **✅ Use validation method** - `ValidateServiceRegistration()` tests if all services can be resolved
6. **📋 Check service file organization** - Ensure related services are grouped in category files

</details>

<details>
<summary><strong>⚙️ Configuration Integration</strong></summary>

The DI system now properly integrates with configuration:

```csharp
// ✅ Configuration binding handled in services
services.AddSingleton<IConfiguration>(configuration);

// ✅ Services access configuration through constructor injection
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfigurationService configurationService)
    {
        _connectionString = configurationService.GetConnectionString();
    }
}
```

</details>

<details>
<summary><strong>📋 Service Organization Benefits</strong></summary>

### **✅ Advantages of Category-Based Service Files**:
1. **Logical Grouping**: Related functionality stays together
2. **Easier Maintenance**: Single location for category changes
3. **Reduced File Clutter**: Fewer files to navigate in Solution Explorer
4. **Better Dependency Management**: Clear visibility of related services
5. **Simplified Testing**: Category-based test organization
6. **Consistent Registration**: All related services registered together

### **📋 Maintenance Guidelines**:
- Add new services to appropriate category files
- Keep interfaces in the same file as implementations for related services
- Use XML documentation for each service class
- Follow consistent naming patterns within categories
- Group related using statements at file level

</details>

<details>
<summary><strong>🎯 Build Status</strong></summary>

✅ **VALIDATED**: All DI configuration compiles successfully  
✅ **TESTED**: Service resolution validation passes  
✅ **INTEGRATED**: Database and Helper_Database_StoredProcedure properly initialized  
✅ **DOCUMENTED**: Complete service registration patterns established  
📋 **ORGANIZED**: Service file organization standard implemented  
✅ **COMPLETED**: ViewModels converted to standard .NET patterns with INotifyPropertyChanged

**Current Phase**: Standard .NET MVVM implementation completed
</details>
