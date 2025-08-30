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
        ILogger? logger = null;
        try
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM WIP Application starting...");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Command line args: {string.Join(" ", args)}");
            
            ConfigureServices();
            logger = GetOptionalService<ILoggerFactory>()?.CreateLogger("Program");
            logger?.LogInformation("MTM WIP Application Program.Main() started with args: {Args}", string.Join(" ", args));
            logger?.LogInformation("Service configuration completed successfully");
            
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            
            logger?.LogInformation("MTM WIP Application Program.Main() completed successfully");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Application startup failed: {ex.Message}";
            var innerMessage = ex.InnerException?.Message;
            
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {errorMessage}");
            if (!string.IsNullOrEmpty(innerMessage))
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Inner exception: {innerMessage}");
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Stack trace: {ex.StackTrace}");
            
            logger?.LogCritical(ex, "Critical application startup failure");
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
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ConfigureServices() started");
        
        var services = new ServiceCollection();

        // Configuration
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Building configuration...");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Configuration registered");

        // Comprehensive logging with detailed event handler and service tracking
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Configuring logging...");
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
            
            // Set comprehensive logging levels for startup and event handler debugging
            builder.SetMinimumLevel(LogLevel.Debug);
            
            // Enable debug level for all MTM components for comprehensive event handler tracking
            builder.AddFilter("MTM_WIP_Application_Avalonia", LogLevel.Debug);
            builder.AddFilter("MTM_WIP_Application_Avalonia.ViewModels", LogLevel.Debug);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services", LogLevel.Debug);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Views", LogLevel.Debug);
            builder.AddFilter("MTM_Shared_Logic", LogLevel.Debug);
            
            // Specific filters for critical components
            builder.AddFilter("MTM_WIP_Application_Avalonia.ViewModels.QuickButtonsViewModel", LogLevel.Trace);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services.QuickButtonsService", LogLevel.Trace);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services.Helper_Database_StoredProcedure", LogLevel.Trace);
            builder.AddFilter("MTM_WIP_Application_Avalonia.Services.NavigationService", LogLevel.Trace);
            builder.AddFilter("MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel", LogLevel.Trace);
            builder.AddFilter("MTM_WIP_Application_Avalonia.ViewModels.MainViewViewModel", LogLevel.Trace);
        });
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logging configured with comprehensive debug levels");

        // ✅ CRITICAL: Use comprehensive MTM service registration
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Registering MTM services...");
        services.AddMTMServices(configuration);
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM services registered");

        // Build service provider with validation
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Building service provider...");
        _serviceProvider = services.BuildServiceProvider();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Service provider built successfully");

        // Validate critical services in debug mode
        #if DEBUG
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Validating runtime services...");
        _serviceProvider.ValidateRuntimeServices();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Runtime services validation completed");
        #endif
        
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ConfigureServices() completed successfully");
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
