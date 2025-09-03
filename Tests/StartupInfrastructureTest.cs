using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Core.Startup;

namespace MTM_WIP_Application_Avalonia.Tests;

/// <summary>
/// Test class for validating the startup infrastructure.
/// This class demonstrates the usage of the new startup system and validates all components.
/// </summary>
public static class StartupInfrastructureTest
{
    /// <summary>
    /// Runs a comprehensive test of the startup infrastructure.
    /// </summary>
    /// <returns>True if all tests pass, false otherwise</returns>
    public static async Task<bool> RunStartupInfrastructureTestAsync()
    {
        var testStopwatch = Stopwatch.StartNew();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting Startup Infrastructure Test");
        Debug.WriteLine($"[STARTUP-TEST] Starting comprehensive startup infrastructure test");

        try
        {
            // Test 1: Configuration Loading
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 1: Configuration Loading");
            var configuration = await TestConfigurationLoading();
            if (configuration == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] FAILED: Configuration loading test failed");
                return false;
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PASSED: Configuration loading test");

            // Test 2: Service Registration
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 2: Service Registration");
            var services = await TestServiceRegistration(configuration);
            if (services == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] FAILED: Service registration test failed");
                return false;
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PASSED: Service registration test");

            // Test 3: Application Startup
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 3: Application Startup");
            var serviceProvider = await TestApplicationStartup(services, configuration);
            if (serviceProvider == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] FAILED: Application startup test failed");
                return false;
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PASSED: Application startup test");

            // Test 4: Service Resolution
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 4: Service Resolution");
            var servicesWork = await TestServiceResolution(serviceProvider);
            if (!servicesWork)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] FAILED: Service resolution test failed");
                return false;
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PASSED: Service resolution test");

            // Test 5: Health Monitoring
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Test 5: Health Monitoring");
            var healthWorks = await TestHealthMonitoring(serviceProvider);
            if (!healthWorks)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] FAILED: Health monitoring test failed");
                return false;
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] PASSED: Health monitoring test");

            testStopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ALL TESTS PASSED in {testStopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine($"[STARTUP-TEST] All startup infrastructure tests completed successfully in {testStopwatch.ElapsedMilliseconds}ms");

            return true;
        }
        catch (Exception ex)
        {
            testStopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL ERROR in startup test: {ex.Message}");
            Debug.WriteLine($"[STARTUP-TEST] Critical error during startup test: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Tests configuration loading functionality.
    /// </summary>
    /// <returns>Configuration instance if successful, null otherwise</returns>
    private static async Task<IConfiguration?> TestConfigurationLoading()
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.WriteLine($"[TEST-CONFIG] Testing configuration loading");

        try
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // Verify configuration can be accessed
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var logLevel = configuration["Logging:LogLevel:Default"];

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration loaded - Connection: {(string.IsNullOrEmpty(connectionString) ? "Not configured" : "Configured")}, LogLevel: {logLevel ?? "Default"}");

            stopwatch.Stop();
            Debug.WriteLine($"[TEST-CONFIG] Configuration loading test completed in {stopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(1); // Yield control
            return configuration;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration loading failed: {ex.Message}");
            Debug.WriteLine($"[TEST-CONFIG] Configuration loading failed: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Tests service registration functionality.
    /// </summary>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service collection if successful, null otherwise</returns>
    private static async Task<IServiceCollection?> TestServiceRegistration(IConfiguration configuration)
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.WriteLine($"[TEST-SERVICES] Testing service registration");

        try
        {
            var services = new ServiceCollection();

            // Test direct service registration (simplified for testing)
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Basic services registered successfully - Count: {services.Count}");

            stopwatch.Stop();
            Debug.WriteLine($"[TEST-SERVICES] Service registration test completed in {stopwatch.ElapsedMilliseconds}ms with {services.Count} services");

            await Task.Delay(1); // Yield control
            return services;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service registration failed: {ex.Message}");
            Debug.WriteLine($"[TEST-SERVICES] Service registration failed: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Tests application startup functionality.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration instance</param>
    /// <returns>Service provider if successful, null otherwise</returns>
    private static async Task<IServiceProvider?> TestApplicationStartup(IServiceCollection services, IConfiguration configuration)
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.WriteLine($"[TEST-STARTUP] Testing application startup");

        try
        {
            var serviceProvider = ApplicationStartup.InitializeApplication(services, Environment.GetCommandLineArgs());

            if (serviceProvider == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service provider is null after startup");
                return null;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application startup completed successfully in {stopwatch.ElapsedMilliseconds}ms");

            stopwatch.Stop();
            Debug.WriteLine($"[TEST-STARTUP] Application startup test completed in {stopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(1); // Yield control
            return serviceProvider;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application startup failed: {ex.Message}");
            Debug.WriteLine($"[TEST-STARTUP] Application startup failed: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Tests service resolution functionality.
    /// </summary>
    /// <param name="serviceProvider">Service provider instance</param>
    /// <returns>True if all services resolve correctly, false otherwise</returns>
    private static async Task<bool> TestServiceResolution(IServiceProvider serviceProvider)
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.WriteLine($"[TEST-RESOLUTION] Testing service resolution");

        try
        {
            // Test logger resolution
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("StartupInfrastructureTest");
            if (logger == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Logger service resolution failed");
                return false;
            }

            // Test validation service resolution
            var validationService = serviceProvider.GetService<IStartupValidationService>();
            if (validationService == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Validation service resolution failed");
                return false;
            }

            // Test health service resolution
            var healthService = serviceProvider.GetService<IApplicationHealthService>();
            if (healthService == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health service resolution failed");
                return false;
            }

            // Test configuration resolution
            var configuration = serviceProvider.GetService<IConfiguration>();
            if (configuration == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration service resolution failed");
                return false;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] All core services resolved successfully");

            stopwatch.Stop();
            Debug.WriteLine($"[TEST-RESOLUTION] Service resolution test completed in {stopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(1); // Yield control
            return true;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service resolution failed: {ex.Message}");
            Debug.WriteLine($"[TEST-RESOLUTION] Service resolution failed: {ex}");
            return false;
        }
    }

    /// <summary>
    /// Tests health monitoring functionality.
    /// </summary>
    /// <param name="serviceProvider">Service provider instance</param>
    /// <returns>True if health monitoring works correctly, false otherwise</returns>
    private static async Task<bool> TestHealthMonitoring(IServiceProvider serviceProvider)
    {
        var stopwatch = Stopwatch.StartNew();
        Debug.WriteLine($"[TEST-HEALTH] Testing health monitoring");

        try
        {
            var healthService = serviceProvider.GetRequiredService<IApplicationHealthService>();

            // Test health status retrieval
            var healthStatus = await healthService.GetHealthStatusAsync();
            if (healthStatus == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health status retrieval failed");
                return false;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health Status - Healthy: {healthStatus.IsHealthy}, Uptime: {healthStatus.Uptime.TotalSeconds:F1}s, Memory: {healthStatus.WorkingSet / (1024 * 1024)} MB");

            // Test comprehensive health check
            var healthCheck = await healthService.PerformHealthCheckAsync();
            if (healthCheck == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health check failed");
                return false;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health Check - Status: {healthCheck.OverallHealth}, Duration: {healthCheck.CheckDurationMs}ms, Components: {healthCheck.ComponentResults.Count}");

            // Test performance metrics
            var performanceMetrics = healthService.GetPerformanceMetrics();
            if (performanceMetrics == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Performance metrics retrieval failed");
                return false;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Performance - Threads: {performanceMetrics.ThreadCount}, Handles: {performanceMetrics.HandleCount}, Memory: {performanceMetrics.WorkingSet / (1024 * 1024)} MB");

            // Test startup metrics
            var startupMetrics = healthService.GetStartupMetrics();
            if (startupMetrics == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Startup metrics retrieval failed");
                return false;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Startup - Duration: {startupMetrics.TotalStartupDuration.TotalMilliseconds:F1}ms, Framework: {startupMetrics.FrameworkVersion}, Processors: {startupMetrics.ProcessorCount}");

            stopwatch.Stop();
            Debug.WriteLine($"[TEST-HEALTH] Health monitoring test completed in {stopwatch.ElapsedMilliseconds}ms");

            await Task.Delay(1); // Yield control
            return true;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Health monitoring failed: {ex.Message}");
            Debug.WriteLine($"[TEST-HEALTH] Health monitoring failed: {ex}");
            return false;
        }
    }
}
