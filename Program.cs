using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_Shared_Logic.Extensions;

namespace MTM_WIP_Application_Avalonia;

public static class Program
{
    private static IServiceProvider? _serviceProvider;

    public static void Main(string[] args) 
    {
        try
        {
            ConfigureServices();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application startup failed: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            throw;
        }
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
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging with console output optimized for Avalonia
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            
            // Configure console logging for better debugging
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                options.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
            });
            
            builder.SetMinimumLevel(LogLevel.Information);
            
            // Enable debug level for QuickButtons specifically
            builder.AddFilter("MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel", LogLevel.Debug);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services.QuickButtonsService", LogLevel.Debug);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services.Helper_Database_StoredProcedure", LogLevel.Debug);
        });

        // ✅ CRITICAL: Use comprehensive MTM service registration
        services.AddMTMServices(configuration);

        // Build service provider with validation
        _serviceProvider = services.BuildServiceProvider();

        // Validate critical services in debug mode
        #if DEBUG
        _serviceProvider.ValidateRuntimeServices();
        #endif
    }

    // Service resolution methods
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");
        return _serviceProvider.GetRequiredService<T>();
    }

    public static T? GetOptionalService<T>() where T : class =>
        _serviceProvider?.GetService<T>();

    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");
        return _serviceProvider.CreateScope();
    }
}
