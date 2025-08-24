using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM.Extensions;

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

        // Core Services (Singleton - shared across application)
        // Use the comprehensive MTM service registration extension
        services.AddMTMServices(configuration);

        // Override specific services for Avalonia application
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        // Infrastructure Services (Singleton - stateless utilities)
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IApplicationStateService, ApplicationStateService>();

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
