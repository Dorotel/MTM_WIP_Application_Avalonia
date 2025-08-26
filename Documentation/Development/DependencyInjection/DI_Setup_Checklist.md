# Dependency Injection Setup Checklist

Use this checklist when setting up DI or adding new services to prevent runtime errors.

## ? Pre-Development Checklist

Before starting any development that involves DI:

### 1. Program.cs Validation
- [ ] `using MTM_Shared_Logic.Extensions;` is present at the top
- [ ] `services.AddMTMServices(configuration);` is called before other service registrations
- [ ] All ViewModels that will be resolved are registered with `services.AddTransient<ViewModel>()`
- [ ] Avalonia service overrides come AFTER `AddMTMServices`
- [ ] Service validation method is included for debug builds

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
using MTM_Shared_Logic.Extensions; // ? CRITICAL
```

### 3. Service Registration Order
```csharp
// ? Correct order
var services = new ServiceCollection();

// 1. Basic services
services.AddSingleton<IConfiguration>(configuration);
services.AddLogging(...);

// 2. MTM services (FIRST!)
services.AddMTMServices(configuration);

// 3. Avalonia overrides
services.AddSingleton<IConfigurationService, ConfigurationService>();
services.AddSingleton<IApplicationStateService, ApplicationStateService>();

// 4. Avalonia-specific services
services.AddSingleton<INavigationService, NavigationService>();

// 5. ViewModels
services.AddTransient<MainViewModel>();
// ... all other ViewModels
```

## ? Development Phase Checklist

### When Adding New ViewModels
- [ ] ViewModel is registered in Program.cs with `services.AddTransient<NewViewModel>()`
- [ ] ViewModel constructor uses dependency injection
- [ ] ViewModel inherits from BaseViewModel
- [ ] ViewModel is added to validation method (debug builds)

### When Adding New Services
- [ ] Service interface and implementation are defined
- [ ] Service is registered with appropriate lifetime (Singleton/Scoped/Transient)
- [ ] Service dependencies are properly injected
- [ ] Service is added to validation method if critical

### When Modifying Existing Services
- [ ] Interface contracts are maintained
- [ ] Dependencies haven't changed (or are properly updated)
- [ ] Service lifetime is still appropriate
- [ ] Tests still pass

## ? Testing Checklist

### Before Committing Code
- [ ] Application starts without DI exceptions
- [ ] All ViewModels can be resolved
- [ ] All critical services can be resolved
- [ ] Debug validation passes
- [ ] No circular dependencies

### Build Validation
- [ ] Clean build succeeds
- [ ] No DI-related warnings
- [ ] Runtime DI validation passes (debug mode)

## ? Common Error Prevention

### Must-Have Patterns
```csharp
// ? Required in Program.cs
using MTM_Shared_Logic.Extensions;

// ? Required service registration
services.AddMTMServices(configuration);

// ? Required ViewModel registration
services.AddTransient<MainWindowViewModel>();

// ? Required validation (debug)
#if DEBUG
ValidateServiceRegistration();
#endif
```

### Never Do This
```csharp
// ? NEVER register MTM services individually
services.AddScoped<MTM_Shared_Logic.Services.IInventoryService, MTM_Shared_Logic.Services.InventoryService>();

// ? NEVER forget to register ViewModels
// Missing: services.AddTransient<MainWindowViewModel>();

// ? NEVER register Avalonia services before MTM services
services.AddSingleton<INavigationService, NavigationService>();
services.AddMTMServices(configuration); // Too late!
```

## ? Error Recovery

### If You Get DI Errors
1. **Check Program.cs** - Ensure AddMTMServices is called first
2. **Check Using Statements** - Ensure `using MTM_Shared_Logic.Extensions;` is present
3. **Check ViewModel Registration** - Ensure all used ViewModels are registered
4. **Check Service Order** - MTM services must be registered first
5. **Run Validation** - Use the validation method to identify missing services

### Common Error Messages and Fixes
| Error | Fix |
|-------|-----|
| "Unable to resolve service for type 'IValidationService'" | Add `services.AddMTMServices(configuration);` |
| "No service for type 'MainWindowViewModel'" | Add `services.AddTransient<MainWindowViewModel>();` |
| "The name 'AddMTMServices' does not exist" | Add `using MTM_Shared_Logic.Extensions;` |
| "Multiple registrations exist" | Register Avalonia services AFTER AddMTMServices |

## ? Documentation Updates

When changing DI setup:
- [ ] Update `Documentation/Development/DependencyInjection/README_DependencyInjection.md`
- [ ] Update this checklist if new patterns emerge
- [ ] Update `.github/copilot-instructions.md` if needed
- [ ] Update troubleshooting guide with new error patterns

## ? Code Review Checklist

When reviewing DI-related code:
- [ ] AddMTMServices is used instead of individual registrations
- [ ] All ViewModels are registered
- [ ] Service registration order is correct
- [ ] Using statements include MTM_Shared_Logic.Extensions
- [ ] No circular dependencies introduced
- [ ] Service lifetimes are appropriate
- [ ] Error handling is present

## ?? Quick Reference

- **Setup Guide**: [`README_DependencyInjection.md`](README_DependencyInjection.md)
- **Troubleshooting**: [`DI_Troubleshooting_Guide.md`](DI_Troubleshooting_Guide.md)
- **Quick Reference**: [`DI_Quick_Reference.md`](DI_Quick_Reference.md)
- **GitHub Instructions**: [`../.github/copilot-instructions.md`](../../.github/copilot-instructions.md)

---

**Remember**: DI errors usually occur at startup and can completely prevent the application from running. Following this checklist prevents 99% of common DI issues in the MTM WIP Application.