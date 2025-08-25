using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Memory;
using MTM.Services;
using MTM.Core.Services;
using MTM.Models;
using IDatabaseService = MTM.Core.Services.IDatabaseService;

namespace MTM.Extensions
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

            // Add caching services
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, SimpleCacheService>();

            // Add validation services
            services.AddScoped<IValidationService, SimpleValidationService>();

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
    /// </summary>
    internal class SimpleCacheService : ICacheService
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

        public Task<Result> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) 
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = expiration.Value;
            
            _memoryCache.Set(key, value, options);
            return Task.FromResult(Result.Success());
        }

        public Task<Result> RemoveAsync(string key, CancellationToken cancellationToken = default) 
        {
            _memoryCache.Remove(key);
            return Task.FromResult(Result.Success());
        }

        public Task<Result> RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement pattern-based removal
            return Task.FromResult(Result.Success());
        }

        public Task<Result> ClearAllAsync(CancellationToken cancellationToken = default) 
        {
            // TODO: Implement cache clearing
            return Task.FromResult(Result.Success());
        }
    }

    /// <summary>
    /// Simple validation service implementation.
    /// </summary>
    internal class SimpleValidationService : IValidationService
    {
        public Task<Result<ValidationResult>> ValidateAsync<T>(T entity, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement using FluentValidation or similar
            var validationResult = new ValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            };
            return Task.FromResult(Result<ValidationResult>.Success(validationResult));
        }

        public Task<Result<bool>> ValidateRuleAsync(string ruleName, object context, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement rule validation
            return Task.FromResult(Result<bool>.Success(true));
        }
    }
}
