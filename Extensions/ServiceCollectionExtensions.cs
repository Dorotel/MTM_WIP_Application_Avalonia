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
using MTMSettings = MTM_Shared_Logic.Services.MTMSettings;
using DatabaseSettings = MTM_Shared_Logic.Services.DatabaseSettings;
using ErrorHandlingSettings = MTM_Shared_Logic.Services.ErrorHandlingSettings;
using LoggingSettings = MTM_Shared_Logic.Services.LoggingSettings;

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

            // Add settings validation (note: MTM_Shared_Logic.Services already has ConfigurationValidationService)
            // services.AddSingleton<IValidateOptions<MTMSettings>, ConfigurationValidationService>();

            // Add core infrastructure services - use explicit types to avoid ambiguity
            services.TryAddSingleton<MTM_Shared_Logic.Core.Services.IConfigurationService, MTM_Shared_Logic.Services.ConfigurationService>();
            services.TryAddSingleton<MTM_Shared_Logic.Core.Services.IApplicationStateService, MTM_Shared_Logic.Services.MTMApplicationStateService>();

            // Add database services
            services.TryAddScoped<IDatabaseService, DatabaseService>();
            services.TryAddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
            services.TryAddTransient<DatabaseTransactionService>();

            // Add business services - ensure all are registered
            services.TryAddScoped<IInventoryService, InventoryService>();
            services.TryAddScoped<MTM_Shared_Logic.Services.Interfaces.IUserService, UserService>();
            services.TryAddScoped<ITransactionService, TransactionService>();

            // Add data services for AutoCompleteBox support
            services.TryAddScoped<ILookupDataService, LookupDataService>();

            // Add UI and theme services
            services.TryAddSingleton<IThemeService, ThemeService>();

            // Add caching services - use the specific MTM_Shared_Logic.Services version
            services.AddMemoryCache();
            services.TryAddSingleton<MTM_Shared_Logic.Services.ICacheService, MTM_Shared_Logic.Services.CacheService>();

            // Add validation services - use the specific MTM_Shared_Logic.Services version
            services.TryAddScoped<MTM_Shared_Logic.Services.IValidationService, MTM_Shared_Logic.Services.ValidationService>();

            // Add notification services
            services.TryAddScoped<INotificationService, NotificationService>();

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
        /// Adds MTM services for development environment with enhanced validation.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMServicesForDevelopment(this IServiceCollection services, IConfiguration configuration)
        {
            // Add core services
            services.AddMTMServices(configuration);

            // Add development-specific logging configuration
            services.Configure<LoggerFilterOptions>(options =>
            {
                options.MinLevel = LogLevel.Debug;
            });

            // Validate services in development mode
            services.ValidateMTMServices();

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

            // Production-specific logging configuration
            services.Configure<LoggerFilterOptions>(options =>
            {
                options.MinLevel = LogLevel.Warning;
            });

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
        /// Validates that all critical MTM services are properly registered.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection ValidateMTMServices(this IServiceCollection services)
        {
            var requiredServices = new[]
            {
                // Core infrastructure
                typeof(IConfiguration),
                typeof(ILoggerFactory),
                
                // MTM Core Services
                typeof(MTM_Shared_Logic.Core.Services.IConfigurationService),
                typeof(MTM_Shared_Logic.Core.Services.IApplicationStateService),
                
                // Database Services
                typeof(IDatabaseService),
                typeof(IDbConnectionFactory),
                
                // Business Services
                typeof(IInventoryService),
                typeof(MTM_Shared_Logic.Services.Interfaces.IUserService),
                typeof(ITransactionService),
                
                // UI Services
                typeof(IThemeService),
                typeof(MTM_Shared_Logic.Services.ICacheService),
                typeof(MTM_Shared_Logic.Services.IValidationService),
                
                // Avalonia Services
                typeof(IApplicationStateService),
                typeof(INavigationService),
                typeof(IConfigurationService),
                
                // Critical ViewModels
                typeof(MainViewViewModel),
                typeof(MainWindowViewModel),
                typeof(QuickButtonsViewModel)
            };

            var missingServices = new List<string>();

            foreach (var serviceType in requiredServices)
            {
                if (!services.Any(x => x.ServiceType == serviceType))
                {
                    missingServices.Add(serviceType.Name);
                }
            }

            if (missingServices.Count > 0)
            {
                var errorMessage = $"Missing required services: {string.Join(", ", missingServices)}";
                throw new InvalidOperationException(errorMessage);
            }

            return services;
        }

        /// <summary>
        /// Tests service resolution at runtime to identify dependency issues.
        /// </summary>
        /// <param name="serviceProvider">The built service provider</param>
        public static void ValidateRuntimeServices(this IServiceProvider serviceProvider)
        {
            var criticalServices = new[]
            {
                typeof(ILogger<MainViewViewModel>),
                typeof(INavigationService),
                typeof(IApplicationStateService),
                typeof(IInventoryService),
                typeof(MainViewViewModel),
                typeof(MainWindowViewModel),
                typeof(QuickButtonsViewModel)
            };

            var failedServices = new List<string>();

            foreach (var serviceType in criticalServices)
            {
                try
                {
                    var service = serviceProvider.GetRequiredService(serviceType);
                    // Service resolved successfully
                }
                catch (Exception ex)
                {
                    failedServices.Add($"{serviceType.Name}: {ex.Message}");
                }
            }

            if (failedServices.Count > 0)
            {
                var errorMessage = $"Failed to resolve critical services:\n{string.Join("\n", failedServices)}";
                throw new InvalidOperationException(errorMessage);
            }
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
