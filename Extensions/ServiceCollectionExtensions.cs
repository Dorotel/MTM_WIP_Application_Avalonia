using System;
using System.Collections.Generic;
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

            // Add core infrastructure services
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IApplicationStateService, MTMApplicationStateService>();

            // Add database services
            services.AddScoped<IDatabaseService, DatabaseService>();
            services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
            services.AddTransient<DatabaseTransactionService>();

            // Add business services
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITransactionService, TransactionService>();

            // Add UI and theme services
            services.AddSingleton<IThemeService, ThemeService>();

            // Add caching services - use the specific MTM_Shared_Logic.Services version
            services.AddMemoryCache();
            services.AddSingleton<MTM_Shared_Logic.Services.ICacheService, MTM_Shared_Logic.Services.CacheService>();

            // Add validation services - use the specific MTM_Shared_Logic.Services version
            services.AddScoped<MTM_Shared_Logic.Services.IValidationService, MTM_Shared_Logic.Services.ValidationService>();

            // Add notification services
            services.AddScoped<INotificationService, NotificationService>();

            // Add ViewModels that exist
            services.AddTransient<InventoryTabViewModel>();
            services.AddTransient<InventoryViewModel>();
            services.AddTransient<AddItemViewModel>();
            services.AddTransient<MainViewViewModel>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<QuickButtonsViewModel>();
            services.AddTransient<RemoveItemViewModel>();
            services.AddTransient<TransferItemViewModel>();
            services.AddTransient<TransactionHistoryViewModel>();
            services.AddTransient<UserManagementViewModel>();

            // TODO: Add these ViewModels when they are created
            // services.AddTransient<AdvancedInventoryViewModel>();
            // services.AddTransient<AdvancedRemoveViewModel>();

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
