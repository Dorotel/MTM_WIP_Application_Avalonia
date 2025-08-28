# ServiceCollectionExtensions.cs - Complete Documentation and Troubleshooting Guide

## Overview

The `ServiceCollectionExtensions.cs` file provides comprehensive dependency injection setup for the MTM WIP Application Avalonia. This file is critical for proper application startup and service resolution.

## Current Architecture

### Service Registration Flow

```csharp
services.AddMTMServices(configuration) // Main entry point
├── Configure strongly-typed settings (MTMSettings, DatabaseSettings, etc.)
├── Core infrastructure services (Configuration, ApplicationState)
├── Database services (DatabaseService, ConnectionFactory, Transactions)
├── Business services (Inventory, User, Transaction services)
├── Data services (LookupData for AutoCompleteBox)
├── UI services (Theme service)
├── Caching services (MTM_Shared_Logic.Services.CacheService)
├── Validation services (MTM_Shared_Logic.Services.ValidationService)
├── Notification services
├── Avalonia-specific services (ApplicationState, Navigation, Configuration)
└── ViewModels (All main ViewModels using TryAdd pattern)
```

## Service Categories and Lifetimes

### Configuration Services (Singleton)
- `MTMSettings` - Application-wide settings
- `DatabaseSettings` - Database configuration
- `ErrorHandlingSettings` - Error handling configuration  
- `LoggingSettings` - Logging configuration

### Core Infrastructure (Singleton)
- `MTM_Shared_Logic.Core.Services.IConfigurationService` → `MTM_Shared_Logic.Services.ConfigurationService`
- `MTM_Shared_Logic.Core.Services.IApplicationStateService` → `MTM_Shared_Logic.Services.MTMApplicationStateService`

### Database Services
- `IDatabaseService` → `DatabaseService` (Scoped)
- `IDbConnectionFactory` → `MySqlConnectionFactory` (Singleton)
- `DatabaseTransactionService` (Transient)

### Business Services (Scoped)
- `IInventoryService` → `InventoryService`
- `MTM_Shared_Logic.Services.Interfaces.IUserService` → `UserService`
- `ITransactionService` → `TransactionService`
- `ILookupDataService` → `LookupDataService`

### UI and Application Services
- `IThemeService` → `ThemeService` (Singleton)
- `MTM_Shared_Logic.Services.ICacheService` → `MTM_Shared_Logic.Services.CacheService` (Singleton)
- `MTM_Shared_Logic.Services.IValidationService` → `MTM_Shared_Logic.Services.ValidationService` (Scoped)
- `INotificationService` → `NotificationService` (Scoped)

### Avalonia-Specific Services (with TryAdd)
- `MTM_WIP_Application_Avalonia.Services.IApplicationStateService` → `ApplicationStateService` (Singleton)
- `INavigationService` → `NavigationService` (Singleton)
- `MTM_WIP_Application_Avalonia.Services.IConfigurationService` → `ConfigurationService` (Singleton)

### ViewModels (Transient - with TryAdd)
All ViewModels are registered as Transient with TryAdd to prevent duplicate registrations:
- `InventoryTabViewModel`
- `InventoryViewModel`
- `AddItemViewModel`
- `MainViewViewModel`
- `MainWindowViewModel`
- `QuickButtonsViewModel`
- `RemoveItemViewModel`
- `TransferItemViewModel`
- `TransactionHistoryViewModel`
- `UserManagementViewModel`
- `AdvancedInventoryViewModel`
- `AdvancedRemoveViewModel`

## Current Issues and Symptoms

### Binding Error Symptoms
```
[Binding]An error occurred binding 'Width' to 'QuickActionsPanelWidth': 'Could not convert FallbackValue '300' to 'Avalonia.Controls.GridLength'.'
```

### Exception Symptoms
```
Exception thrown: 'System.InvalidOperationException' in Avalonia.Base.dll
Exception thrown: 'System.InvalidOperationException' in System.Private.CoreLib.dll
```

## Root Cause Analysis

### 1. Service Resolution Issues
The `InvalidOperationException`s suggest that some services are not being properly resolved during dependency injection. This can happen when:
- A service dependency is missing from the container
- Circular dependencies exist
- Interface/implementation mismatches

### 2. Binding Resolution Issues
The `QuickActionsPanelWidth` binding error indicates:
- The property exists and returns `GridLength` correctly
- The binding is falling back to the fallback value `'300'` (string)
- The fallback value cannot be converted to `GridLength`
- This suggests the DataContext (MainViewViewModel) is not being properly resolved

## Troubleshooting Steps

### Step 1: Verify Service Registration Order
The current order should be:
1. Configuration settings
2. Core infrastructure
3. Database services
4. Business services
5. UI services
6. Avalonia-specific services (with TryAdd)
7. ViewModels (with TryAdd)

### Step 2: Check for Missing Services
Potential missing services that might be causing issues:

```csharp
// These services might need explicit registration:
services.TryAddSingleton<ILogger<MainViewViewModel>>();
services.TryAddSingleton<ILoggerFactory>();

// Verify these are available:
services.TryAddScoped<IInventoryService, InventoryService>();
services.TryAddScoped<MTM_Shared_Logic.Services.Interfaces.IUserService, UserService>();
```

### Step 3: Validate TryAdd Implementation
The current TryAdd implementation:

```csharp
public static IServiceCollection TryAddTransient<TService, TImplementation>(this IServiceCollection services)
    where TService : class
    where TImplementation : class, TService
{
    if (!services.Any(x => x.ServiceType == typeof(TService)))
    {
        services.AddTransient<TService, TImplementation>();
    }
    return services;
}
```

This should prevent duplicate registrations but ensure all required services are registered.

## Recommended Fixes

### Fix 1: Add Missing Service Registrations

```csharp
// Add to AddMTMServices method after existing registrations:

// Ensure logging services are properly registered
services.TryAddSingleton<ILoggerFactory, LoggerFactory>();

// Verify all ViewModel dependencies are registered
services.TryAddScoped<IInventoryService, InventoryService>();
services.TryAddScoped<ITransactionService, TransactionService>();
services.TryAddScoped<ILookupDataService, LookupDataService>();
```

### Fix 2: Validate Configuration Service Registration

```csharp
// Ensure both configuration service interfaces are properly registered
services.AddSingleton<MTM_Shared_Logic.Core.Services.IConfigurationService, MTM_Shared_Logic.Services.ConfigurationService>();
services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.IConfigurationService, MTM_WIP_Application_Avalonia.Services.ConfigurationService>();
```

### Fix 3: Add Service Validation
Add a service validation method to verify all required services are registered:

```csharp
public static IServiceCollection ValidateMTMServices(this IServiceCollection services)
{
    var requiredServices = new[]
    {
        typeof(ILogger<MainViewViewModel>),
        typeof(INavigationService),
        typeof(IApplicationStateService),
        typeof(IInventoryService),
        typeof(IThemeService)
    };

    foreach (var serviceType in requiredServices)
    {
        if (!services.Any(x => x.ServiceType == serviceType))
        {
            throw new InvalidOperationException($"Required service {serviceType.Name} is not registered");
        }
    }

    return services;
}
```

### Fix 4: Improve Error Handling in Program.cs

```csharp
// In Program.cs ConfigureServices method:
try
{
    services.AddMTMServices(configuration);
    services.ValidateMTMServices(); // Add validation
    _serviceProvider = services.BuildServiceProvider();
    
    // Validate critical services can be resolved
    var logger = _serviceProvider.GetService<ILogger<Program>>();
    var mainViewModel = _serviceProvider.GetService<MainWindowViewModel>();
    
    logger?.LogInformation("Service provider configured successfully");
}
catch (Exception ex)
{
    // Log detailed service registration error
    Console.WriteLine($"Service registration failed: {ex.Message}");
    throw;
}
```

## Testing Service Resolution

### Manual Service Resolution Test

```csharp
// Add to Program.cs after service configuration:
private static void TestServiceResolution(IServiceProvider serviceProvider)
{
    try
    {
        // Test critical services
        var logger = serviceProvider.GetRequiredService<ILogger<MainViewViewModel>>();
        var navigation = serviceProvider.GetRequiredService<INavigationService>();
        var appState = serviceProvider.GetRequiredService<IApplicationStateService>();
        var inventory = serviceProvider.GetRequiredService<IInventoryService>();
        var mainViewModel = serviceProvider.GetRequiredService<MainViewViewModel>();
        
        Console.WriteLine("All critical services resolved successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Service resolution test failed: {ex.Message}");
        throw;
    }
}
```

## Environment-Specific Configurations

### Development Environment Enhancements

```csharp
public static IServiceCollection AddMTMServicesForDevelopment(this IServiceCollection services, IConfiguration configuration)
{
    services.AddMTMServices(configuration);
    
    // Add development-specific services
    services.AddSingleton<IHostEnvironment>(new DevelopmentHostEnvironment());
    
    // Add enhanced logging for development
    services.Configure<LoggerFilterOptions>(options =>
    {
        options.MinLevel = LogLevel.Debug;
    });
    
    // Validate services in development
    services.ValidateMTMServices();
    
    return services;
}
```

### Production Environment Optimizations

```csharp
public static IServiceCollection AddMTMServicesForProduction(this IServiceCollection services, 
    IConfiguration configuration, 
    IDbConnectionFactory? connectionFactory = null)
{
    services.AddMTMServices(configuration);
    
    // Production-specific optimizations
    services.Configure<LoggerFilterOptions>(options =>
    {
        options.MinLevel = LogLevel.Warning;
    });
    
    // Override connection factory if provided
    if (connectionFactory != null)
    {
        services.AddSingleton(connectionFactory);
    }
    
    return services;
}
```

## Best Practices for Service Registration

### 1. Service Lifetime Guidelines
- **Singleton**: Configuration, Caching, Application State, Theme
- **Scoped**: Business services, Validation, Database operations
- **Transient**: ViewModels, Short-lived operations

### 2. Registration Order
1. Configuration and Settings
2. Infrastructure Services
3. Data Access Services
4. Business Logic Services
5. UI Services
6. Application-Specific Services
7. ViewModels

### 3. Dependency Validation
- Always validate that dependencies exist before registration
- Use TryAdd for services that might be registered elsewhere
- Implement service resolution testing

### 4. Error Handling
- Wrap service registration in try-catch blocks
- Provide meaningful error messages
- Log service registration issues

## Monitoring and Diagnostics

### Service Registration Logging

```csharp
public static IServiceCollection AddMTMServicesWithLogging(this IServiceCollection services, IConfiguration configuration)
{
    var logger = new ConsoleLogger("ServiceRegistration");
    
    logger.LogInformation("Starting MTM service registration");
    
    services.AddMTMServices(configuration);
    
    logger.LogInformation($"Registered {services.Count} services");
    
    return services;
}
```

### Runtime Service Validation

```csharp
public static void ValidateRuntimeServices(IServiceProvider serviceProvider)
{
    var criticalServices = new[]
    {
        typeof(MainViewViewModel),
        typeof(IInventoryService),
        typeof(INavigationService)
    };
    
    foreach (var serviceType in criticalServices)
    {
        try
        {
            var service = serviceProvider.GetRequiredService(serviceType);
            Console.WriteLine($"✓ {serviceType.Name} resolved successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ {serviceType.Name} failed to resolve: {ex.Message}");
        }
    }
}
```

## Summary

The `ServiceCollectionExtensions.cs` file is working correctly for basic service registration, but there may be missing service dependencies or configuration issues causing the runtime exceptions. The binding error for `QuickActionsPanelWidth` suggests that the `MainViewViewModel` is not being properly resolved, which could be due to missing dependencies in its constructor chain.

The recommended approach is to:
1. Add service validation
2. Enhance error handling in service registration
3. Test service resolution explicitly
4. Add missing service registrations as needed
5. Implement proper logging for service registration diagnostics

This documentation provides a comprehensive understanding of the service registration architecture and troubleshooting strategies for resolving the current issues.
