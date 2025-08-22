using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using MTM.Core.Services;
using MTM.Models;
using ConfigurationProvider = MTM.Core.Services.IConfigurationProvider;

namespace MTM.Core.Extensions
{
    /// <summary>
    /// Extension methods for registering MTM Core Services with dependency injection.
    /// Framework-agnostic implementation compatible with any .NET application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all MTM Core Services to the service collection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add core infrastructure services
            services.AddSingleton<IConfigurationService>(provider => 
                new ConfigurationService(configuration, provider.GetRequiredService<ILogger<ConfigurationService>>()));

            // Add application state management (singleton for global state)
            services.AddSingleton<IApplicationStateService, ApplicationStateService>();

            // Add caching services
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, CacheService>();

            // Add validation services
            services.AddScoped<IValidationService, ValidationService>();

            // TODO: Add service implementations as they are created
            // Uncomment and implement these as you create the service implementations:
            
            // services.AddScoped<IUserService, UserService>();
            // services.AddScoped<IDatabaseService, DatabaseService>();

            // Configure error handling
            ConfigureErrorHandling(services, configuration);

            // Configure logging
            ConfigureLogging(services, configuration);

            return services;
        }

        /// <summary>
        /// Adds MTM Core Services with custom database connection factory.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="connectionFactory">The database connection factory</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMCoreServices(this IServiceCollection services, 
            IConfiguration configuration, 
            IDbConnectionFactory connectionFactory)
        {
            // Add core services
            services.AddMTMCoreServices(configuration);

            // Add database connection factory
            services.AddSingleton(connectionFactory);

            // Configure error handling with database support
            LoggingUtility.SetConnectionFactory(connectionFactory);

            return services;
        }

        /// <summary>
        /// Adds MTM Core Services for development environment.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMCoreServicesForDevelopment(this IServiceCollection services, IConfiguration configuration)
        {
            // Add core services
            services.AddMTMCoreServices(configuration);

            // Configure for development
            ErrorHandlingConfiguration.ConfigureForDevelopment();

            return services;
        }

        /// <summary>
        /// Adds MTM Core Services for production environment.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="connectionFactory">The database connection factory</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMTMCoreServicesForProduction(this IServiceCollection services, 
            IConfiguration configuration, 
            IDbConnectionFactory connectionFactory)
        {
            // Add core services with database support
            services.AddMTMCoreServices(configuration, connectionFactory);

            // Configure for production
            ErrorHandlingConfiguration.ConfigureForProduction();

            return services;
        }

        /// <summary>
        /// Configures error handling system.
        /// </summary>
        private static void ConfigureErrorHandling(IServiceCollection services, IConfiguration configuration)
        {
            // Create configuration provider for error handling
            var configProvider = new ServiceConfigurationProvider(configuration);
            
            // Configure error handling
            services.AddSingleton<ConfigurationProvider>(configProvider);
            
            // Add post-configure action to initialize error handling
            services.AddHostedService<ErrorHandlingInitializationService>();
        }

        /// <summary>
        /// Configures logging system.
        /// </summary>
        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
                
                // Add file logging if configured
                var fileLoggingEnabled = configuration.GetValue<bool>("Logging:File:Enable");
                if (fileLoggingEnabled)
                {
                    // TODO: Add file logging provider
                    // builder.AddFile(configuration.GetSection("Logging:File"));
                }
            });
        }
    }

    /// <summary>
    /// Configuration provider implementation for integrating with Microsoft.Extensions.Configuration.
    /// </summary>
    internal class ServiceConfigurationProvider : ConfigurationProvider
    {
        private readonly IConfiguration _configuration;

        public ServiceConfigurationProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string? GetValue(string key)
        {
            return _configuration[key];
        }

        public bool? GetBoolValue(string key)
        {
            var value = _configuration[key];
            return bool.TryParse(value, out var result) ? result : null;
        }

        public int? GetIntValue(string key)
        {
            var value = _configuration[key];
            return int.TryParse(value, out var result) ? result : null;
        }
    }

    /// <summary>
    /// Hosted service for initializing error handling on application startup.
    /// </summary>
    internal class ErrorHandlingInitializationService : IHostedService
    {
        private readonly ConfigurationProvider _configProvider;
        private readonly IDbConnectionFactory? _connectionFactory;
        private readonly ILogger<ErrorHandlingInitializationService> _logger;

        public ErrorHandlingInitializationService(
            ConfigurationProvider configProvider, 
            ILogger<ErrorHandlingInitializationService> logger,
            IDbConnectionFactory? connectionFactory = null)
        {
            _configProvider = configProvider;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initializing MTM error handling system...");

            var success = ErrorHandlingInitializer.Initialize(_configProvider, _connectionFactory);
            
            if (success)
            {
                _logger.LogInformation("MTM error handling system initialized successfully");
            }
            else
            {
                _logger.LogWarning("MTM error handling system initialization completed with warnings");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down MTM error handling system...");
            ErrorHandlingInitializer.Shutdown();
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// TODO: Placeholder service implementations.
    /// Implement these services using the custom prompts:
    /// 
    /// - ConfigurationService (Prompt 28)
    /// - ApplicationStateService (Prompt 27)
    /// - CacheService (Prompt 35)
    /// - ValidationService (Prompt 32)
    /// - UserService (Prompt 25)
    /// - DatabaseService (Prompt 26)
    /// 
    /// See exportable-customprompt.instruction.md for detailed implementation prompts.
    /// </summary>
    
    // Placeholder implementations - replace with actual implementations
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string? GetValue(string key) => _configuration[key];
        public T? GetValue<T>(string key) => _configuration.GetValue<T>(key);
        public string? GetConnectionString(string name) => _configuration.GetConnectionString(name);
        public T? GetSection<T>(string sectionName) where T : class, new() => _configuration.GetSection(sectionName).Get<T>();
        public Result ValidateConfiguration() => Result.Success();
        public Task<Result> ReloadConfigurationAsync() => Task.FromResult(Result.Success());
    }

    internal class ApplicationStateService : IApplicationStateService
    {
        public User? CurrentUser { get; private set; }
        public bool IsUserLoggedIn => CurrentUser != null;
        public ConnectionStatus ConnectionStatus { get; private set; } = ConnectionStatus.Disconnected;
        public Dictionary<string, object> Settings { get; } = new();

        public event EventHandler<UserChangedEventArgs>? CurrentUserChanged;
        public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;
        public event EventHandler<SettingChangedEventArgs>? SettingChanged;

        public void SetCurrentUser(User? user)
        {
            var previous = CurrentUser;
            CurrentUser = user;
            CurrentUserChanged?.Invoke(this, new UserChangedEventArgs(previous, user));
        }

        public void SetConnectionStatus(ConnectionStatus status)
        {
            var previous = ConnectionStatus;
            ConnectionStatus = status;
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs(previous, status));
        }

        public T? GetSetting<T>(string key) => Settings.TryGetValue(key, out var value) ? (T?)value : default;

        public void SetSetting(string key, object value)
        {
            var previous = Settings.TryGetValue(key, out var prev) ? prev : null;
            Settings[key] = value;
            SettingChanged?.Invoke(this, new SettingChangedEventArgs(key, previous, value));
        }

        public void Clear()
        {
            SetCurrentUser(null);
            SetConnectionStatus(ConnectionStatus.Disconnected);
            Settings.Clear();
        }
    }

    internal class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
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

    internal class ValidationService : IValidationService
    {
        public Task<Result<ValidationResult>> ValidateAsync<T>(T entity, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement using FluentValidation or similar
            return Task.FromResult(Result<ValidationResult>.Success(ValidationResult.Success()));
        }

        public Task<Result<bool>> ValidateRuleAsync(string ruleName, object context, CancellationToken cancellationToken = default) 
        {
            // TODO: Implement rule validation
            return Task.FromResult(Result<bool>.Success(true));
        }
    }
}