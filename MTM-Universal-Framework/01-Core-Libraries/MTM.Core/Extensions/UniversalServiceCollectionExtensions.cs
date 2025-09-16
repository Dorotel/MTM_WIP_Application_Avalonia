using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MTM.UniversalFramework.Core.Configuration;
using MTM.UniversalFramework.Core.ErrorHandling;
using MTM.UniversalFramework.Core.Logging;
using MTM.UniversalFramework.Core.Startup;

namespace MTM.UniversalFramework.Core.Extensions;

/// <summary>
/// Extension methods for registering Universal Framework services with dependency injection.
/// Provides a clean, domain-agnostic service registration pattern.
/// </summary>
public static class UniversalServiceCollectionExtensions
{
    /// <summary>
    /// Adds all Universal Framework core services to the service collection.
    /// This provides the foundation services needed by any application built with the framework.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddUniversalFrameworkCore(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Core infrastructure services
        services.TryAddSingleton<IUniversalConfigurationService, UniversalConfigurationService>();
        services.TryAddSingleton<IUniversalApplicationStateService, UniversalApplicationStateService>();
        
        // Logging services
        services.TryAddSingleton<IUniversalFileLoggingService, UniversalFileLoggingService>();
        
        // Settings and state management
        services.TryAddSingleton<IUniversalSettingsService, UniversalSettingsService>();
        
        // Framework marker service (already registered in startup but can be reinforced here)
        services.TryAddSingleton<IUniversalFrameworkMarker, UniversalFrameworkMarker>();

        return services;
    }

    /// <summary>
    /// Adds Universal Framework configuration services.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddUniversalConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.TryAddSingleton<IUniversalConfigurationService>(provider => 
            new UniversalConfigurationService(configuration, provider.GetRequiredService<ILogger<UniversalConfigurationService>>()));

        return services;
    }

    /// <summary>
    /// Adds Universal Framework logging services with customizable configuration.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configureOptions">Optional action to configure logging options</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddUniversalLogging(this IServiceCollection services, Action<UniversalLoggingOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new UniversalLoggingOptions();
        configureOptions?.Invoke(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<IUniversalFileLoggingService, UniversalFileLoggingService>();

        return services;
    }

    /// <summary>
    /// Adds Universal Framework error handling services.
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddUniversalErrorHandling(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IUniversalErrorHandlingService, UniversalErrorHandlingService>();

        return services;
    }
}

/// <summary>
/// Configuration options for Universal Framework logging.
/// </summary>
public class UniversalLoggingOptions
{
    /// <summary>
    /// Base path for log files. Defaults to application's Logs directory.
    /// </summary>
    public string BasePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Logs");

    /// <summary>
    /// Whether to enable dual location logging (network + local).
    /// </summary>
    public bool EnableDualLocationLogging { get; set; } = false;

    /// <summary>
    /// Network path for logs (when dual location logging is enabled).
    /// </summary>
    public string? NetworkBasePath { get; set; }

    /// <summary>
    /// Whether to suppress network errors when dual location logging fails.
    /// </summary>
    public bool SuppressNetworkErrors { get; set; } = true;

    /// <summary>
    /// Maximum log file size in MB before rotation.
    /// </summary>
    public int MaxLogFileSizeMB { get; set; } = 10;

    /// <summary>
    /// Maximum number of log files to keep.
    /// </summary>
    public int MaxLogFiles { get; set; } = 10;
}