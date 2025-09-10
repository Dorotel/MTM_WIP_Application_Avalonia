using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            // Check for validation mode command line argument
            if (args.Length > 0 && args[0].Equals("--validate-procedures", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Running in stored procedure validation mode");
                await RunStoredProcedureValidationAsync();
                return;
            }

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
    /// Builds the Avalonia application with platform detection and trace logging.
    /// </summary>
    /// <returns>Configured AppBuilder instance</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Building Avalonia app...");
        Debug.WriteLine($"[PROGRAM] Building Avalonia application");

        try
        {
            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
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
            // Check if application is already initialized from startup tests
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

    /// <summary>
    /// Runs stored procedure validation in console mode
    /// </summary>
    private static async Task RunStoredProcedureValidationAsync()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Initializing services for validation...");
        
        try
        {
            // Configure services using ApplicationStartup
            var services = new ServiceCollection();
            _serviceProvider = ApplicationStartup.InitializeApplication(services, Environment.GetCommandLineArgs());

            if (_serviceProvider == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Failed to initialize service provider");
                Environment.Exit(1);
                return;
            }

            // Get validation service
            var validationService = _serviceProvider.GetRequiredService<Services.IStoredProcedureValidationService>();
            _logger = _serviceProvider.GetService<ILogger<object>>();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting stored procedure validation...");

            // Run validation
            var report = await validationService.ValidateAllStoredProceduresAsync();

            // Output results to console
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Validation completed!");
            Console.WriteLine("========================================");
            Console.WriteLine(report.ToSummaryReport());
            Console.WriteLine("========================================");

            // Save detailed JSON report
            var reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"validation_report_{DateTime.Now:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(reportPath, report.ToJsonReport());
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Detailed report saved to: {reportPath}");

            // Summary for immediate feedback
            if (report.MismatchedCalls > 0)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] VALIDATION FAILED: {report.MismatchedCalls} procedure calls have issues");
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] VALIDATION PASSED: All procedure calls are consistent");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Validation failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Environment.Exit(1);
        }
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
