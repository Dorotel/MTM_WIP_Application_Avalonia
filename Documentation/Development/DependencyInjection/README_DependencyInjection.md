# Dependency Injection Setup for MTM WIP Application

## ?? CRITICAL NOTICE: Service Registration Requirements

**ALWAYS use `services.AddMTMServices(configuration)` - NEVER register MTM services individually!**

### ? Common Fatal Error Pattern
```csharp
// This WILL FAIL at runtime - missing dependencies!
services.AddScoped<MTM_Shared_Logic.Services.IInventoryService, MTM_Shared_Logic.Services.InventoryService>();
// Error: Unable to resolve service for type 'MTM_Shared_Logic.Core.Services.IValidationService'
```

### ? Required Pattern
```csharp
// This registers ALL required dependencies correctly
services.AddMTMServices(configuration);
```

## Overview

The MTM WIP Application now uses Microsoft.Extensions.DependencyInjection for comprehensive dependency injection throughout the application. This provides loose coupling, improved testability, and proper service lifetime management.

## **CRITICAL SETUP REQUIREMENTS**

### **1. Use AddMTMServices Extension Method**
The `AddMTMServices` extension method in `MTM_Shared_Logic.Extensions.ServiceCollectionExtensions` is **MANDATORY** for registering MTM business services. It automatically handles complex dependency chains that would be impossible to register manually.

**Required Using Statement:**
```csharp
using MTM_Shared_Logic.Extensions; // REQUIRED for AddMTMServices
```

**Services Automatically Registered by AddMTMServices:**
- `IDatabaseService` ? `DatabaseService`
- `IValidationService` ? `SimpleValidationService` ? **Critical dependency**
- `ICacheService` ? `SimpleCacheService` ? **Critical dependency**
- `IInventoryService` ? `InventoryService`
- `IUserService` ? `UserService`
- `ITransactionService` ? `TransactionService`
- `IDbConnectionFactory` ? `MySqlConnectionFactory`
- `DatabaseTransactionService`
- `IMemoryCache` support
- Strongly-typed configuration options with validation

### **2. Complete Program.cs Template**
```csharp
using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_Shared_Logic.Extensions; // CRITICAL: Required for AddMTMServices

namespace MTM_WIP_Application_Avalonia;

public static class Program
{
    private static IServiceProvider? _serviceProvider;

    public static void Main(string[] args) 
    {
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
    }

    // Service resolution methods...
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServices first.");

        return _serviceProvider.GetRequiredService<T>();
    }

    public static T? GetOptionalService<T>() where T : class
    {
        return _serviceProvider?.GetService<T>();
    }

    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");

        return _serviceProvider.CreateScope();
    }
}
```

### **3. Common Registration Errors and Solutions**

#### ? **Error: Missing IValidationService**
```
System.InvalidOperationException: Unable to resolve service for type 'MTM_Shared_Logic.Core.Services.IValidationService' 
while attempting to activate 'MTM_Shared_Logic.Services.InventoryService'.
```
**Solution:** Ensure `services.AddMTMServices(configuration);` is called.

#### ? **Error: Missing ICacheService**
```
System.InvalidOperationException: Unable to resolve service for type 'MTM_Shared_Logic.Core.Services.ICacheService'
while attempting to activate 'MTM_Shared_Logic.Services.UserService'.
```
**Solution:** Ensure `services.AddMTMServices(configuration);` is called.

#### ? **Error: Missing ViewModel**
```
System.InvalidOperationException: No service for type 'MainWindowViewModel' has been registered.
```
**Solution:** Add `services.AddTransient<MainWindowViewModel>();` to ConfigureServices.

#### ? **Error: Missing Using Statement**
```
CS0103: The name 'AddMTMServices' does not exist in the current context
```
**Solution:** Add `using MTM_Shared_Logic.Extensions;` at the top of Program.cs.

### **4. ViewModel Registration Checklist**
Every ViewModel that will be resolved through DI must be registered:

```csharp
// ? ALL of these are required if used in the application
services.AddTransient<MainViewModel>();
services.AddTransient<MainViewViewModel>();
services.AddTransient<MainWindowViewModel>();     // ? Often forgotten
services.AddTransient<InventoryViewModel>();
services.AddTransient<AddItemViewModel>();
services.AddTransient<RemoveItemViewModel>();
services.AddTransient<TransferItemViewModel>();
services.AddTransient<TransactionHistoryViewModel>();
services.AddTransient<UserManagementViewModel>();
```

### **5. Service Registration Validation**
Add this method to your Program.cs to validate all services can be resolved:

```csharp
#if DEBUG
private static void ValidateServiceRegistration()
{
    try
    {
        // Test MTM Core Services
        var dbService = GetService<MTM_Shared_Logic.Core.Services.IDatabaseService>();
        var validationService = GetService<MTM_Shared_Logic.Core.Services.IValidationService>();
        var cacheService = GetService<MTM_Shared_Logic.Core.Services.ICacheService>();
        
        // Test MTM Business Services
        var inventoryService = GetService<MTM_Shared_Logic.Services.IInventoryService>();
        var userService = GetService<MTM_Shared_Logic.Services.IUserService>();
        var transactionService = GetService<MTM_Shared_Logic.Services.ITransactionService>();
        
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

Call this method after `_serviceProvider = services.BuildServiceProvider();` in debug builds:
```csharp
// Build service provider
_serviceProvider = services.BuildServiceProvider();

#if DEBUG
ValidateServiceRegistration();
#endif