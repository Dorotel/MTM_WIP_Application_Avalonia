using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Core.Startup;
using Avalonia.Controls.ApplicationLifetimes;
using ZstdSharp.Unsafe;

namespace MTM_WIP_Application_Avalonia;

/// <summary>
/// Main program entry point with comprehensive startup infrastructure.
/// Follows .NET best practices for dependency injection, logging, and error handling.
/// </summary>
public static class Program
{
    private static IServiceProvider? _serviceProvider;
    private static ILogger<object>? _logger;

    /// <summary>
    /// Main application entry point with enhanced error handling and logging.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static async Task Main(string[] args)
    {
        var mainStopwatch = Stopwatch.StartNew();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MTM WIP Application Program.Main() starting...");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Command line args: [{string.Join(", ", args)}]");
        Debug.WriteLine($"[PROGRAM] Application main entry point started with {args.Length} arguments");

        try
        {
            // Phase 2: Configure services using ApplicationStartup infrastructure
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring services using ApplicationStartup...");
            var configureResult = await ConfigureServicesAsync();
            if (!configureResult)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL: Service configuration failed");
                Environment.Exit(1);
                return;
            }

            // Phase 3: Start Avalonia application (MainView initialization will happen after Avalonia starts)
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting Avalonia application...");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Platform: {Environment.OSVersion}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Runtime: {Environment.Version}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Is Interactive: {Environment.UserInteractive}");
            _logger?.LogInformation("Starting Avalonia application with {ArgCount} arguments", args.Length);

            var appStopwatch = Stopwatch.StartNew();

            // For now, all platforms use classic desktop lifetime
            // Mobile support can be added later when creating dedicated mobile projects
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting with classic desktop lifetime (cross-platform compatible)");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

            appStopwatch.Stop();

            mainStopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application completed successfully in {mainStopwatch.ElapsedMilliseconds}ms (Avalonia: {appStopwatch.ElapsedMilliseconds}ms)");
            _logger?.LogInformation("MTM WIP Application completed successfully - Total: {TotalMs}ms, Avalonia: {AvaloniaMs}ms",
                mainStopwatch.ElapsedMilliseconds, appStopwatch.ElapsedMilliseconds);

        }
        catch (Exception ex)
        {
            mainStopwatch.Stop();
            await HandleCriticalApplicationErrorAsync(ex, mainStopwatch.ElapsedMilliseconds);
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Builds the Avalonia application with cross-platform support and platform detection.
    /// Supports desktop (Windows, macOS, Linux) and mobile (Android, iOS) platforms.
    /// </summary>
    /// <returns>Configured AppBuilder instance</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Building Avalonia app for platform: {Environment.OSVersion.Platform}");
        Debug.WriteLine($"[PROGRAM] Building cross-platform Avalonia application");

        try
        {
            var builder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

            // Platform-specific configurations
            if (OperatingSystem.IsAndroid())
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring for Android platform");
                // Android-specific configuration would go here if needed
            }
            else if (OperatingSystem.IsIOS())
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring for iOS platform");
                // iOS-specific configuration would go here if needed
            }
            else if (OperatingSystem.IsMacOS())
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring for macOS platform");
                // macOS-specific configuration would go here if needed
            }
            else if (OperatingSystem.IsWindows())
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring for Windows platform");
                // Windows-specific configuration would go here if needed
            }
            else if (OperatingSystem.IsLinux())
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuring for Linux platform");
                // Linux-specific configuration would go here if needed
            }

            return builder;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Error building Avalonia app: {ex.Message}");
            // Fallback configuration without optional features
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }

    /// <summary>
    /// Configures services using the ApplicationStartup infrastructure.
    /// </summary>
    /// <returns>True if configuration succeeded, false otherwise</returns>
    private static async Task<bool> ConfigureServicesAsync()
    {
        var configureStopwatch = Stopwatch.StartNew();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ConfigureServicesAsync() started");
        Debug.WriteLine($"[PROGRAM] Starting service configuration using ApplicationStartup");

        try
        {
            // Check if application is already initialized from ApplicationStartup
            if (ApplicationStartup.IsInitialized)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application already initialized, using existing service provider");
                _serviceProvider = ApplicationStartup.GetServiceProvider();

                if (_serviceProvider == null)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service provider is null despite initialization flag");
                    Debug.WriteLine($"[PROGRAM] Service provider is null despite initialization flag");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application not yet initialized, starting fresh initialization");
                var services = new ServiceCollection();

                // Use ApplicationStartup to initialize the application
                _serviceProvider = ApplicationStartup.InitializeApplication(services, Environment.GetCommandLineArgs());

                if (_serviceProvider == null)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service provider is null after startup");
                    Debug.WriteLine($"[PROGRAM] Service provider is null after ApplicationStartup");
                    return false;
                }
            }

            // Get logger after service provider is available
            _logger = _serviceProvider.GetService<ILogger<object>>();

            // Initialize Theme V2 service for automatic theme management
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Initializing Theme V2 service...");
            try
            {
                var themeServiceV2 = _serviceProvider.GetService<IThemeServiceV2>();
                if (themeServiceV2 != null)
                {
                    await themeServiceV2.InitializeAsync();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Theme V2 service initialized successfully");
                    _logger?.LogInformation("Theme V2 service initialized successfully");
                }
                else
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Warning: Theme V2 service not available");
                    _logger?.LogWarning("Theme V2 service not available in service provider");
                }
            }
            catch (Exception themeEx)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Warning: Theme V2 initialization failed: {themeEx.Message}");
                _logger?.LogError(themeEx, "Theme V2 service initialization failed");
                // Continue startup - theme service failure shouldn't block the application
            }

            configureStopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service configuration completed in {configureStopwatch.ElapsedMilliseconds}ms");
            _logger?.LogInformation("Service configuration completed successfully in {ConfigureMs}ms",
                configureStopwatch.ElapsedMilliseconds);

            Debug.WriteLine($"[PROGRAM] Service configuration completed successfully in {configureStopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(1); // Yield control for async method
            return true;
        }
        catch (Exception ex)
        {
            configureStopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service configuration failed after {configureStopwatch.ElapsedMilliseconds}ms: {ex.Message}");
            Debug.WriteLine($"[PROGRAM] Service configuration failed: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Handles critical application errors with comprehensive logging and reporting.
    /// </summary>
    /// <param name="ex">The exception that occurred</param>
    /// <param name="elapsedMs">Elapsed time before the error</param>
    private static async Task HandleCriticalApplicationErrorAsync(Exception ex, long elapsedMs)
    {
        var errorMessage = $"CRITICAL APPLICATION FAILURE after {elapsedMs}ms: {ex.Message}";
        var innerMessage = ex.InnerException?.Message;

        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {errorMessage}");
        if (!string.IsNullOrEmpty(innerMessage))
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Inner exception: {innerMessage}");
        }
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Stack trace: {ex.StackTrace}");

        Debug.WriteLine($"[PROGRAM] CRITICAL FAILURE: {ex}");

        // Log through service if available
        _logger?.LogCritical(ex, "Critical application startup failure after {ElapsedMs}ms", elapsedMs);

        // Try to get health information if health service is available
        try
        {
            var healthService = _serviceProvider?.GetService<IApplicationHealthService>();
            if (healthService != null)
            {
                var healthStatus = await healthService.GetHealthStatusAsync();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Final health status - Healthy: {healthStatus.IsHealthy}, Memory: {healthStatus.WorkingSet / (1024 * 1024)}MB, Threads: {healthStatus.ThreadCount}");
            }
        }
        catch (Exception healthEx)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Unable to retrieve health status: {healthEx.Message}");
        }

        await Task.Delay(100); // Allow logging to complete
    }

    // Service resolution methods with null checking
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServicesAsync first.");
        return _serviceProvider.GetRequiredService<T>();
    }

    public static T? GetOptionalService<T>() where T : class =>
        _serviceProvider?.GetService<T>();

    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServicesAsync first.");
        return _serviceProvider.CreateScope();
    }
}
