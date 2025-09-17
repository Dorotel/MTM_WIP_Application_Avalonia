using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.Core.Startup;

/// <summary>
/// Universal application startup management following .NET best practices.
/// Provides structured initialization, validation, and error handling for any business domain.
/// </summary>
public static class UniversalApplicationStartup
{
    private static readonly object _lockObject = new();
    private static bool _isInitialized = false;
    private static IServiceProvider? _serviceProvider;
    private static ILogger? _logger;

    /// <summary>
    /// Gets the current service provider if the application has been initialized.
    /// </summary>
    /// <returns>The current service provider, or null if not initialized</returns>
    public static IServiceProvider? GetServiceProvider()
    {
        lock (_lockObject)
        {
            return _serviceProvider;
        }
    }

    /// <summary>
    /// Initializes the application with comprehensive logging and validation.
    /// </summary>
    /// <param name="services">Service collection for dependency injection</param>
    /// <param name="args">Command line arguments</param>
    /// <returns>Configured service provider</returns>
    /// <exception cref="ArgumentNullException">Thrown when services parameter is null</exception>
    /// <exception cref="InvalidOperationException">Thrown when initialization fails</exception>
    public static IServiceProvider InitializeApplication(IServiceCollection services, string[] args)
    {
        ArgumentNullException.ThrowIfNull(services);

        lock (_lockObject)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException("Application is already initialized");
            }

            var stopwatch = Stopwatch.StartNew();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] UniversalApplicationStartup.InitializeApplication() started");
            Debug.WriteLine($"[STARTUP] Application initialization started with {args?.Length ?? 0} arguments");

            try
            {
                // Phase 1: Configuration
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Phase 1: Configuration setup");
                ConfigureConfiguration(services);

                // Phase 2: Logging
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Phase 2: Logging configuration");
                ConfigureLogging(services);

                // Phase 3: Core Services
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Phase 3: Core services registration");
                ConfigureCoreServices(services);

                // Phase 4: Build Service Provider
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Phase 4: Service provider creation");
                _serviceProvider = services.BuildServiceProvider();

                // Phase 5: Validation
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Phase 5: Service validation");
                ValidateServices(_serviceProvider);

                // Phase 6: Initialize Logger
                _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("UniversalApplicationStartup");

                stopwatch.Stop();
                _logger.LogInformation("Application initialized successfully in {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application initialization completed in {stopwatch.ElapsedMilliseconds}ms");

                _isInitialized = true;
                return _serviceProvider;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application initialization failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}");
                Debug.WriteLine($"[STARTUP] Application initialization failed: {ex}");
                throw new InvalidOperationException($"Application initialization failed: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Configures the application configuration hierarchy.
    /// </summary>
    /// <param name="services">Service collection for dependency injection</param>
    private static void ConfigureConfiguration(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(Environment.GetCommandLineArgs())
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
    }

    /// <summary>
    /// Configures comprehensive logging with structured output and performance tracking.
    /// </summary>
    /// <param name="services">Service collection for dependency injection</param>
    private static void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
#if DEBUG
            // Comprehensive debug logging for development
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddFilter("MTM.UniversalFramework", LogLevel.Trace);
            builder.AddFilter("UniversalFramework", LogLevel.Trace);
            builder.AddConsole();
            builder.AddDebug();
#else
            // Production logging levels
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddFilter("MTM.UniversalFramework", LogLevel.Information);
            builder.AddFilter("UniversalFramework", LogLevel.Information);
            builder.AddConsole();
#endif
        });
    }

    /// <summary>
    /// Configures core framework services.
    /// </summary>
    /// <param name="services">Service collection for dependency injection</param>
    private static void ConfigureCoreServices(IServiceCollection services)
    {
        // Add memory cache for performance
        services.AddMemoryCache();

        // Add HTTP client for external integrations
        services.AddHttpClient();

        // Framework marker service
        services.AddSingleton<IUniversalFrameworkMarker, UniversalFrameworkMarker>();
    }

    /// <summary>
    /// Validates that essential services are properly registered and can be resolved.
    /// </summary>
    /// <param name="serviceProvider">Service provider to validate</param>
    private static void ValidateServices(IServiceProvider serviceProvider)
    {
        var essentialServices = new[]
        {
            typeof(IConfiguration),
            typeof(ILoggerFactory),
            typeof(IUniversalFrameworkMarker)
        };

        foreach (var serviceType in essentialServices)
        {
            try
            {
                var service = serviceProvider.GetRequiredService(serviceType);
                if (service == null)
                {
                    throw new InvalidOperationException($"Essential service {serviceType.Name} resolved to null");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to resolve essential service {serviceType.Name}: {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Shuts down the application gracefully.
    /// </summary>
    public static async Task ShutdownAsync()
    {
        lock (_lockObject)
        {
            if (!_isInitialized) return;

            try
            {
                _logger?.LogInformation("Application shutdown initiated");
                
                if (_serviceProvider is IDisposable disposableServiceProvider)
                {
                    disposableServiceProvider.Dispose();
                }

                _serviceProvider = null;
                _logger = null;
                _isInitialized = false;
                
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application shutdown completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Error during shutdown: {ex.Message}");
            }
        }
        
        await Task.CompletedTask;
    }
}

/// <summary>
/// Marker interface for universal framework services.
/// </summary>
public interface IUniversalFrameworkMarker
{
    string FrameworkVersion { get; }
    DateTime InitializationTime { get; }
}

/// <summary>
/// Implementation of framework marker service.
/// </summary>
public class UniversalFrameworkMarker : IUniversalFrameworkMarker
{
    public string FrameworkVersion => "1.0.0";
    public DateTime InitializationTime { get; } = DateTime.UtcNow;
}