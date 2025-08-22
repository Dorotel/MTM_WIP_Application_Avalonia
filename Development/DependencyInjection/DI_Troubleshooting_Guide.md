# Dependency Injection Troubleshooting Guide

## ?? Critical Error Prevention

This guide covers the most common dependency injection errors in the MTM WIP Application and their solutions.

## **Error #1: Missing IValidationService**

### Error Message:
```
System.InvalidOperationException: Unable to resolve service for type 'MTM.Core.Services.IValidationService' 
while attempting to activate 'MTM.Services.InventoryService'.
```

### Root Cause:
`MTM.Services.InventoryService` requires `MTM.Core.Services.IValidationService` as a dependency, but it wasn't registered in the DI container.

### Solution:
**ALWAYS use `services.AddMTMServices(configuration);` instead of registering MTM services individually.**

#### ? Wrong Way:
```csharp
// This will fail - missing dependencies
services.AddScoped<MTM.Services.IInventoryService, MTM.Services.InventoryService>();
```

#### ? Correct Way:
```csharp
// This registers ALL required dependencies
services.AddMTMServices(configuration);
```

### Required Using Statement:
```csharp
using MTM.Extensions; // Required for AddMTMServices
```

---

## **Error #2: Missing ICacheService**

### Error Message:
```
System.InvalidOperationException: Unable to resolve service for type 'MTM.Core.Services.ICacheService'
while attempting to activate 'MTM.Services.UserService'.
```

### Root Cause:
Multiple MTM services require `ICacheService` for caching operations.

### Solution:
Use `services.AddMTMServices(configuration);` which automatically registers `ICacheService` ? `SimpleCacheService`.

---

## **Error #3: Missing ViewModel Registration**

### Error Message:
```
System.InvalidOperationException: No service for type 'MainWindowViewModel' has been registered.
```

### Root Cause:
`App.axaml.cs` tries to resolve a ViewModel that wasn't registered in the DI container.

### Solution:
Register ALL ViewModels that will be resolved through DI:

```csharp
// ViewModels (Transient - new instance each time)
services.AddTransient<MainViewModel>();
services.AddTransient<MainViewViewModel>();
services.AddTransient<MainWindowViewModel>(); // ? This was missing
services.AddTransient<InventoryViewModel>();
services.AddTransient<AddItemViewModel>();
services.AddTransient<RemoveItemViewModel>();
services.AddTransient<TransferItemViewModel>();
services.AddTransient<TransactionHistoryViewModel>();
services.AddTransient<UserManagementViewModel>();
```

### How to Identify Missing ViewModels:
Look at your `App.axaml.cs` file for lines like:
```csharp
var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
```
Every ViewModel referenced this way must be registered.

---

## **Error #4: Missing Using Statement**

### Error Message:
```
CS0103: The name 'AddMTMServices' does not exist in the current context
```

### Root Cause:
Missing the required using statement for the MTM extensions.

### Solution:
Add this using statement to the top of `Program.cs`:
```csharp
using MTM.Extensions; // Required for AddMTMServices extension method
```

---

## **Error #5: Interface Conflicts**

### Error Message:
```
System.InvalidOperationException: Unable to resolve service for type 'IConfigurationService'.
Multiple registrations exist.
```

### Root Cause:
Both MTM and Avalonia define `IConfigurationService`, causing conflicts.

### Solution:
Override Avalonia-specific services AFTER calling `AddMTMServices`:

```csharp
// ? Register MTM services first
services.AddMTMServices(configuration);

// ? Then override with Avalonia-specific implementations
services.AddSingleton<MTM_WIP_Application_Avalonia.Services.IConfigurationService, 
                     MTM_WIP_Application_Avalonia.Services.ConfigurationService>();
```

---

## **Debugging Checklist**

### 1. Service Registration Order
```csharp
private static void ConfigureServices()
{
    var services = new ServiceCollection();

    // 1. Configuration and Logging first
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging(...);

    // 2. MTM services (CRITICAL - call this first)
    services.AddMTMServices(configuration);

    // 3. Avalonia overrides (after MTM services)
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    services.AddSingleton<IApplicationStateService, ApplicationStateService>();

    // 4. Avalonia-specific services
    services.AddSingleton<INavigationService, NavigationService>();

    // 5. ViewModels last
    services.AddTransient<MainViewModel>();
    // ... other ViewModels
}
```

### 2. Required Using Statements
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
using MTM.Extensions; // ? CRITICAL for AddMTMServices
```

### 3. Service Validation Method
Add this to `Program.cs` for debugging:

```csharp
#if DEBUG
private static void ValidateServiceRegistration()
{
    try
    {
        Console.WriteLine("?? Validating service registration...");
        
        // Test critical MTM services
        var dbService = GetService<MTM.Core.Services.IDatabaseService>();
        Console.WriteLine("? IDatabaseService resolved");
        
        var validationService = GetService<MTM.Core.Services.IValidationService>();
        Console.WriteLine("? IValidationService resolved");
        
        var cacheService = GetService<MTM.Core.Services.ICacheService>();
        Console.WriteLine("? ICacheService resolved");
        
        var inventoryService = GetService<MTM.Services.IInventoryService>();
        Console.WriteLine("? IInventoryService resolved");
        
        // Test Avalonia services
        var navigationService = GetService<INavigationService>();
        Console.WriteLine("? INavigationService resolved");
        
        // Test ViewModels
        var mainViewModel = GetService<MainViewModel>();
        Console.WriteLine("? MainViewModel resolved");
        
        var mainWindowViewModel = GetService<MainWindowViewModel>();
        Console.WriteLine("? MainWindowViewModel resolved");
        
        Console.WriteLine("?? All services resolved successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Service resolution failed: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw;
    }
}
#endif
```

### 4. Runtime Validation
Call validation after building the service provider:

```csharp
// Build service provider
_serviceProvider = services.BuildServiceProvider();

#if DEBUG
ValidateServiceRegistration();
#endif
```

---

## **Complete Working Example**

Here's a complete, working `Program.cs` that avoids all common DI errors:

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
using MTM.Extensions; // CRITICAL for AddMTMServices

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

        // ? Override specific services for Avalonia application
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();

        // Infrastructure Services (Singleton - stateless utilities)
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

#if DEBUG
        ValidateServiceRegistration();
#endif
    }

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

#if DEBUG
    private static void ValidateServiceRegistration()
    {
        try
        {
            Console.WriteLine("?? Validating service registration...");
            
            // Test critical services
            var dbService = GetService<MTM.Core.Services.IDatabaseService>();
            var validationService = GetService<MTM.Core.Services.IValidationService>();
            var cacheService = GetService<MTM.Core.Services.ICacheService>();
            var inventoryService = GetService<MTM.Services.IInventoryService>();
            var navigationService = GetService<INavigationService>();
            
            // Test ViewModels
            var mainViewModel = GetService<MainViewModel>();
            var mainWindowViewModel = GetService<MainWindowViewModel>();
            
            Console.WriteLine("?? All services resolved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Service resolution failed: {ex.Message}");
            throw;
        }
    }
#endif
}
```

---

## **Quick Reference**

### Must-Have Checklist:
- [ ] `using MTM.Extensions;` at top of Program.cs
- [ ] `services.AddMTMServices(configuration);` called first
- [ ] All ViewModels registered with `services.AddTransient<ViewModel>()`
- [ ] Avalonia service overrides AFTER AddMTMServices
- [ ] Service validation in debug builds

### Services Automatically Registered by AddMTMServices:
- ? `IDatabaseService`
- ? `IValidationService` (critical dependency)
- ? `ICacheService` (critical dependency)
- ? `IInventoryService`
- ? `IUserService`
- ? `ITransactionService`
- ? `IDbConnectionFactory`
- ? `IMemoryCache`
- ? Configuration validation

### Services You Must Register Manually:
- ViewModels (all of them)
- Avalonia-specific services (`INavigationService`)
- Service overrides for Avalonia interfaces

This guide should prevent 99% of dependency injection errors in the MTM WIP Application.