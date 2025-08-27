using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using MTM_Shared_Logic.Services;
using MTM_Shared_Logic.Core.Services;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using ApplicationStateService = MTM_WIP_Application_Avalonia.Services.ApplicationStateService;
using NavigationService = MTM_WIP_Application_Avalonia.Services.NavigationService;
using INavigationService = MTM_WIP_Application_Avalonia.Services.Interfaces.INavigationService;
using IApplicationStateService = MTM_WIP_Application_Avalonia.Services.IApplicationStateService;
using IConfigurationService = MTM_WIP_Application_Avalonia.Services.IConfigurationService;
using ConfigurationService = MTM_WIP_Application_Avalonia.Services.ConfigurationService;
using IDatabaseService = MTM_Shared_Logic.Core.Services.IDatabaseService;

namespace MTM_Shared_Logic.Extensions
{
    /// <summary>
    /// Extension methods for registering MTM services with dependency injection.
    /// Implementation for the main MTM WIP Application following .NET 8 patterns.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all MTM services to the service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure strongly-typed settings
            services.Configure<MTMSettings>(configuration.GetSection("MTM"));
            services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
            services.Configure<ErrorHandlingSettings>(configuration.GetSection("ErrorHandling"));
            services.Configure<LoggingSettings>(configuration.GetSection("Logging"));

            // Add settings validation
            services.AddSingleton<IValidateOptions<MTMSettings>, ConfigurationValidationService>();

            // Add core infrastructure services - use explicit types to avoid ambiguity
            services.AddSingleton<MTM_Shared_Logic.Core.Services.IConfigurationService, MTM_Shared_Logic.Services.ConfigurationService>();
            services.AddSingleton<MTM_Shared_Logic.Core.Services.IApplicationStateService, MTM_Shared_Logic.Services.MTMApplicationStateService>();

            // Add database services
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
            services.AddTransient<DatabaseTransactionService>();

            // Add business services
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<MTM_Shared_Logic.Services.Interfaces.IUserService, UserService>();
            services.AddScoped<ITransactionService, TransactionService>();

            // Add data services for AutoCompleteBox support
            services.AddScoped<ILookupDataService, LookupDataService>();

            // Add UI and theme services
            services.AddSingleton<IThemeService, ThemeService>();

            // Add caching services - use the specific MTM_Shared_Logic.Services version
            services.AddMemoryCache();
            services.AddSingleton<MTM_Shared_Logic.Services.ICacheService, MTM_Shared_Logic.Services.CacheService>();

            // Add validation services - use the specific MTM_Shared_Logic.Services version
            services.AddScoped<MTM_Shared_Logic.Services.IValidationService, MTM_Shared_Logic.Services.ValidationService>();

            // Add notification services
            services.AddScoped<INotificationService, NotificationService>();

            // Add Avalonia-specific application services if not already registered
            services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
            services.TryAddSingleton<INavigationService, NavigationService>();
            services.TryAddSingleton<IConfigurationService, ConfigurationService>();

            // Add ViewModels that exist - Only register if not already registered
            services.TryAddTransient<InventoryTabViewModel>();
            services.TryAddTransient<InventoryViewModel>();
            services.TryAddTransient<AddItemViewModel>();
            services.TryAddTransient<MainViewViewModel>();
            services.TryAddTransient<MainWindowViewModel>();
            services.TryAddTransient<QuickButtonsViewModel>();
            services.TryAddTransient<RemoveItemViewModel>();
            services.TryAddTransient<TransferItemViewModel>();
            services.TryAddTransient<TransactionHistoryViewModel>();
            services.TryAddTransient<UserManagementViewModel>();
            services.TryAddTransient<AdvancedInventoryViewModel>();
            services.TryAddTransient<AdvancedRemoveViewModel>();

            return services;
        }

        /// <summary>
        /// Adds MTM services for development environment.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMServicesForDevelopment(this IServiceCollection services, IConfiguration configuration)
        {
            // Add core services
            services.AddMTMServices(configuration);

            // TODO: Configure for development environment
            // ErrorHandlingConfiguration.ConfigureForDevelopment();

            return services;
        }

        /// <summary>
        /// Adds MTM services for production environment.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="connectionFactory">Optional connection factory override</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMServicesForProduction(this IServiceCollection services, 
            IConfiguration configuration, 
            IDbConnectionFactory? connectionFactory = null)
        {
            // Add core services
            services.AddMTMServices(configuration);

            // Override connection factory if provided
            if (connectionFactory != null)
            {
                services.AddSingleton(connectionFactory);
                // TODO: Set connection factory in logging utility
                // LoggingUtility.SetConnectionFactory(connectionFactory);
            }

            // TODO: Configure for production environment
            // ErrorHandlingConfiguration.ConfigureForProduction();

            return services;
        }

        /// <summary>
        /// Extension methods for TryAdd functionality
        /// </summary>
        public static IServiceCollection TryAddTransient<TService>(this IServiceCollection services)
            where TService : class
        {
            return TryAddTransient<TService, TService>(services);
        }

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

        public static IServiceCollection TryAddSingleton<TService>(this IServiceCollection services)
            where TService : class
        {
            return TryAddSingleton<TService, TService>(services);
        }

        public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                services.AddSingleton<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection TryAddScoped<TService>(this IServiceCollection services)
            where TService : class
        {
            return TryAddScoped<TService, TService>(services);
        }

        public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
        {
            if (!services.Any(x => x.ServiceType == typeof(TService)))
            {
                services.AddScoped<TService, TImplementation>();
            }
            return services;
        }
    }

    /// <summary>
    /// Configuration validation service for MTM settings.
    /// Validates configuration options at startup to ensure proper application setup.
    /// </summary>
    public class ConfigurationValidationService : IValidateOptions<MTMSettings>
    {
        public ValidateOptionsResult Validate(string? name, MTMSettings options)
        {
            if (options == null)
            {
                return ValidateOptionsResult.Fail("MTMSettings cannot be null");
            }

            var errors = new List<string>();

            // Validate required settings
            if (options.MaxTransactionQuantity <= 0)
            {
                errors.Add("MaxTransactionQuantity must be greater than 0");
            }

            if (options.MaxTransactionQuantity > 999999)
            {
                errors.Add("MaxTransactionQuantity cannot exceed 999999");
            }

            if (string.IsNullOrWhiteSpace(options.ApplicationName))
            {
                errors.Add("ApplicationName is required");
            }

            if (string.IsNullOrWhiteSpace(options.Version))
            {
                errors.Add("Version is required");
            }

            // Validate theme settings if present
            if (!string.IsNullOrWhiteSpace(options.DefaultTheme))
            {
                var validThemes = new[] { "Light", "Dark", "Auto" };
                if (!validThemes.Contains(options.DefaultTheme))
                {
                    errors.Add($"DefaultTheme must be one of: {string.Join(", ", validThemes)}");
                }
            }

            // Validate database timeout settings
            if (options.DatabaseTimeoutSeconds < 5 || options.DatabaseTimeoutSeconds > 300)
            {
                errors.Add("DatabaseTimeoutSeconds must be between 5 and 300 seconds");
            }

            return errors.Count > 0 
                ? ValidateOptionsResult.Fail(errors) 
                : ValidateOptionsResult.Success;
        }
    }

    /// <summary>
    /// Simple cache service implementation using IMemoryCache.
    /// This is a fallback implementation for the core services interface.
    /// </summary>
    internal class SimpleCacheService : MTM_Shared_Logic.Core.Services.ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public SimpleCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) 
        {
            _memoryCache.TryGetValue(key, out var value);
            return Task.FromResult((T?)value);
        }

        public Task<MTM_Shared_Logic.Models.Result> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) 
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            
            _memoryCache.Set(key, value, options);
            return Task.FromResult(MTM_Shared_Logic.Models.Result.Success());
        }

        public Task<MTM_Shared_Logic.Models.Result> RemoveAsync(string key, CancellationToken cancellationToken = default) 
        {
            _memoryCache.Remove(key);
            return Task.FromResult(MTM_Shared_Logic.Models.Result.Success());
        }

        public Task<MTM_Shared_Logic.Models.Result> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement pattern-based removal
            return Task.FromResult(MTM_Shared_Logic.Models.Result.Success());
        }

        public Task<MTM_Shared_Logic.Models.Result> ClearAllAsync(CancellationToken cancellationToken = default) 
        {
            // TODO: Implement cache clearing for IMemoryCache
            return Task.FromResult(MTM_Shared_Logic.Models.Result.Success());
        }
    }

    /// <summary>
    /// Simple validation service implementation.
    /// This is a fallback implementation for the core services interface.
    /// </summary>
    internal class SimpleValidationService : MTM_Shared_Logic.Core.Services.IValidationService
    {
        public Task<MTM_Shared_Logic.Models.Result<ValidationResult>> ValidateAsync<T>(T entity, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement using FluentValidation or similar
            var validationResult = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };
            return Task.FromResult(MTM_Shared_Logic.Models.Result<ValidationResult>.Success(validationResult));
        }

        public Task<MTM_Shared_Logic.Models.Result<bool>> ValidateRuleAsync(string ruleName, object context, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement rule validation
            return Task.FromResult(MTM_Shared_Logic.Models.Result<bool>.Success(true));
        }
    }
}

namespace MTM_Shared_Logic.Models
{
    /// <summary>
    /// MTM-specific configuration settings.
    /// </summary>
    public class MTMSettings
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ApplicationName { get; set; } = "MTM WIP Application Avalonia";

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Version { get; set; } = "1.0.0";

        [Range(1, 999999)]
        public int MaxTransactionQuantity { get; set; } = 1000;

        [Range(1, 3600)]
        public int SessionTimeoutMinutes { get; set; } = 30;

        [Range(5, 100)]
        public int MaxQuickButtons { get; set; } = 10;

        public bool EnableDebugMode { get; set; } = false;

        public bool AutoSaveUserPreferences { get; set; } = true;

        [Range(1, 60)]
        public int AutoSaveIntervalMinutes { get; set; } = 5;

        [Range(5, 300)]
        public int DatabaseTimeoutSeconds { get; set; } = 30;

        public string DefaultTheme { get; set; } = "Light";

        public List<string> ValidOperations { get; set; } = new() { "90", "100", "110", "120" };

        public List<string> DefaultLocations { get; set; } = new() { "EXPO", "WAREHOUSE", "SHIPPING" };
    }

    /// <summary>
    /// Database configuration settings.
    /// </summary>
    public class DatabaseSettings
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Range(5, 300)]
        public int CommandTimeout { get; set; } = 30;

        [Range(1, 10)]
        public int MaxRetryAttempts { get; set; } = 3;

        public bool EnableConnectionPooling { get; set; } = true;

        [Range(1, 100)]
        public int MaxPoolSize { get; set; } = 10;

        public bool EnableDetailedErrors { get; set; } = false;
    }

    /// <summary>
    /// Error handling configuration settings.
    /// </summary>
    public class ErrorHandlingSettings
    {
        public bool ShowDetailedErrors { get; set; } = false;

        public bool LogToDatabase { get; set; } = true;

        public bool LogToFile { get; set; } = true;

        public string LogFilePath { get; set; } = "logs/";

        [Range(1, 30)]
        public int LogRetentionDays { get; set; } = 7;

        public bool NotifyOnCriticalErrors { get; set; } = true;
    }

    /// <summary>
    /// Logging configuration settings.
    /// </summary>
    public class LoggingSettings
    {
        public string MinimumLevel { get; set; } = "Information";

        public bool EnableConsoleLogging { get; set; } = true;

        public bool EnableFileLogging { get; set; } = true;

        public bool EnableDatabaseLogging { get; set; } = false;

        public string LogFilePath { get; set; } = "logs/";

        [Range(1, 100)]
        public int MaxLogFileSizeMB { get; set; } = 10;

        [Range(1, 30)]
        public int LogRetentionDays { get; set; } = 7;
    }
}
