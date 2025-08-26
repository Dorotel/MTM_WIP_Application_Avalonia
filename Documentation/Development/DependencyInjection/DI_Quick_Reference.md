# DI Quick Reference Card

## ?? CRITICAL: Always Use AddMTMServices

**Copy this exact pattern to avoid runtime errors:**

```csharp
using MTM_Shared_Logic.Extensions; // REQUIRED

private static void ConfigureServices()
{
    var services = new ServiceCollection();
    
    // 1. Configuration & Logging
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging(builder => { builder.AddConsole(); });

    // 2. ? CRITICAL: MTM Services (must be first)
    services.AddMTMServices(configuration);

    // 3. Avalonia Service Overrides (after MTM)
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    services.AddSingleton<IApplicationStateService, ApplicationStateService>();
    services.AddSingleton<INavigationService, NavigationService>();

    // 4. ViewModels (register ALL that are resolved via DI)
    services.AddTransient<MainViewModel>();
    services.AddTransient<MainViewViewModel>();
    services.AddTransient<MainWindowViewModel>(); // Often forgotten!
    services.AddTransient<InventoryViewModel>();
    services.AddTransient<AddItemViewModel>();
    services.AddTransient<RemoveItemViewModel>();
    services.AddTransient<TransferItemViewModel>();
    services.AddTransient<TransactionHistoryViewModel>();
    services.AddTransient<UserManagementViewModel>();

    _serviceProvider = services.BuildServiceProvider();
}
```

## ? Common Fatal Errors

### Error: "Unable to resolve service for type 'IValidationService'"
**Cause**: Missing `services.AddMTMServices(configuration);`  
**Fix**: Always call AddMTMServices before registering other services

### Error: "No service for type 'MainWindowViewModel' has been registered"
**Cause**: Forgot to register ViewModel  
**Fix**: Add `services.AddTransient<MainWindowViewModel>();`

### Error: "The name 'AddMTMServices' does not exist"
**Cause**: Missing using statement  
**Fix**: Add `using MTM_Shared_Logic.Extensions;` at top of Program.cs

## ? Required Files to Update

When adding DI to a project, update these files:

1. **Program.cs**
   - Add `using MTM_Shared_Logic.Extensions;`
   - Call `services.AddMTMServices(configuration);`
   - Register all ViewModels

2. **App.axaml.cs**
   - Use `Program.GetService<ViewModel>()` for ViewModel resolution

3. **ViewModels**
   - Use constructor injection for all dependencies
   - Inherit from BaseViewModel for logging

## ?? Testing DI Setup

Add this validation method to Program.cs:

```csharp
#if DEBUG
private static void ValidateServiceRegistration()
{
    try
    {
        // Test critical services
        var dbService = GetService<MTM_Shared_Logic.Core.Services.IDatabaseService>();
        var validationService = GetService<MTM_Shared_Logic.Core.Services.IValidationService>();
        var inventoryService = GetService<MTM_Shared_Logic.Services.IInventoryService>();
        var mainWindowViewModel = GetService<MainWindowViewModel>();
        
        Console.WriteLine("? All services resolved successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? DI validation failed: {ex.Message}");
        throw;
    }
}
#endif
```

Call after building service provider:
```csharp
_serviceProvider = services.BuildServiceProvider();
#if DEBUG
ValidateServiceRegistration();
#endif
```

## ?? Full Documentation

- **Complete Setup**: [`Documentation/Development/DependencyInjection/README_DependencyInjection.md`](README_DependencyInjection.md)
- **Troubleshooting**: [`Documentation/Development/DependencyInjection/DI_Troubleshooting_Guide.md`](DI_Troubleshooting_Guide.md)
- **GitHub Instructions**: [`.github/copilot-instructions.md`](../../.github/copilot-instructions.md)