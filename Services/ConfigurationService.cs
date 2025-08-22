using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTM.Models;
using MTM.Core.Services;

namespace MTM.Services
{
    /// <summary>
    /// Configuration service implementation for reading and managing appsettings.json.
    /// Provides strongly-typed configuration access with validation and environment-specific overrides.
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ConfigurationService> _logger;
        private readonly IOptionsMonitor<MTMSettings> _mtmSettings;

        public ConfigurationService(
            IConfiguration configuration, 
            ILogger<ConfigurationService> logger,
            IOptionsMonitor<MTMSettings> mtmSettings)
        {
            _configuration = configuration;
            _logger = logger;
            _mtmSettings = mtmSettings;
        }

        /// <summary>
        /// Gets a configuration value by key.
        /// </summary>
        public string? GetValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            try
            {
                var value = _configuration[key];
                _logger.LogDebug("Retrieved configuration value for key {Key}: {HasValue}", key, value != null);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to retrieve configuration value for key {Key}: {Error}", key, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a strongly-typed configuration value.
        /// </summary>
        public T? GetValue<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default;
            }

            try
            {
                var value = _configuration.GetValue<T>(key);
                _logger.LogDebug("Retrieved typed configuration value for key {Key} as {Type}: {HasValue}", 
                    key, typeof(T).Name, value != null);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to retrieve typed configuration value for key {Key} as {Type}: {Error}", 
                    key, typeof(T).Name, ex.Message);
                return default;
            }
        }

        /// <summary>
        /// Gets a connection string by name.
        /// </summary>
        public string? GetConnectionString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            try
            {
                var connectionString = _configuration.GetConnectionString(name);
                _logger.LogDebug("Retrieved connection string for {Name}: {HasValue}", name, connectionString != null);
                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to retrieve connection string for {Name}: {Error}", name, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets a configuration section as a strongly-typed object.
        /// </summary>
        public T? GetSection<T>(string sectionName) where T : class, new()
        {
            if (string.IsNullOrWhiteSpace(sectionName))
            {
                return null;
            }

            try
            {
                var section = _configuration.GetSection(sectionName);
                if (!section.Exists())
                {
                    _logger.LogWarning("Configuration section {SectionName} does not exist", sectionName);
                    return null;
                }

                var value = section.Get<T>();
                _logger.LogDebug("Retrieved configuration section {SectionName} as {Type}: {HasValue}", 
                    sectionName, typeof(T).Name, value != null);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to retrieve configuration section {SectionName} as {Type}: {Error}", 
                    sectionName, typeof(T).Name, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Validates the current configuration.
        /// </summary>
        public Result ValidateConfiguration()
        {
            try
            {
                _logger.LogInformation("Validating application configuration");

                var errors = new List<string>();

                // Validate database connection string
                var connectionString = GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    errors.Add("DefaultConnection string is missing or empty");
                }

                // Validate MTM settings
                var mtmSettings = _mtmSettings.CurrentValue;
                var validationContext = new ValidationContext(mtmSettings);
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                
                if (!Validator.TryValidateObject(mtmSettings, validationContext, validationResults, true))
                {
                    foreach (var validationResult in validationResults)
                    {
                        errors.Add($"MTM Settings validation error: {validationResult.ErrorMessage}");
                    }
                }

                // Validate logging configuration
                var loggingSection = _configuration.GetSection("Logging");
                if (!loggingSection.Exists())
                {
                    errors.Add("Logging configuration section is missing");
                }

                // Validate database configuration
                var databaseSection = _configuration.GetSection("Database");
                if (!databaseSection.Exists())
                {
                    errors.Add("Database configuration section is missing");
                }

                if (errors.Count > 0)
                {
                    var errorMessage = $"Configuration validation failed: {string.Join("; ", errors)}";
                    _logger.LogError("Configuration validation failed with {ErrorCount} errors", errors.Count);
                    return Result.Failure(errorMessage);
                }

                _logger.LogInformation("Configuration validation completed successfully");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Configuration validation failed with exception");
                return Result.Failure($"Configuration validation failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Reloads configuration from sources.
        /// </summary>
        public async Task<Result> ReloadConfigurationAsync()
        {
            try
            {
                _logger.LogInformation("Reloading configuration");

                // Force configuration reload
                if (_configuration is IConfigurationRoot configRoot)
                {
                    configRoot.Reload();
                }

                // Re-validate after reload
                var validationResult = ValidateConfiguration();
                if (!validationResult.IsSuccess)
                {
                    return Result.Failure($"Configuration reload failed validation: {validationResult.ErrorMessage}");
                }

                _logger.LogInformation("Configuration reloaded successfully");
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Configuration reload failed");
                return Result.Failure($"Configuration reload failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets database configuration settings.
        /// </summary>
        public DatabaseSettings GetDatabaseSettings()
        {
            return GetSection<DatabaseSettings>("Database") ?? new DatabaseSettings();
        }

        /// <summary>
        /// Gets error handling configuration settings.
        /// </summary>
        public ErrorHandlingSettings GetErrorHandlingSettings()
        {
            return GetSection<ErrorHandlingSettings>("ErrorHandling") ?? new ErrorHandlingSettings();
        }

        /// <summary>
        /// Gets logging configuration settings.
        /// </summary>
        public LoggingSettings GetLoggingSettings()
        {
            return GetSection<LoggingSettings>("Logging") ?? new LoggingSettings();
        }
    }

    /// <summary>
    /// MTM-specific configuration settings.
    /// </summary>
    public class MTMSettings
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ApplicationName { get; set; } = "MTM WIP Application";

        [Required]
        [StringLength(10, MinimumLength = 1)]
        public string Version { get; set; } = "1.0.0";

        [Range(1, 3600)]
        public int SessionTimeoutMinutes { get; set; } = 60;

        [Range(5, 100)]
        public int MaxQuickButtons { get; set; } = 10;

        public bool EnableDebugMode { get; set; } = false;

        public bool AutoSaveUserPreferences { get; set; } = true;

        [Range(1, 60)]
        public int AutoSaveIntervalMinutes { get; set; } = 5;

        public List<string> ValidOperations { get; set; } = new() { "90", "100", "110" };

        public List<string> DefaultLocations { get; set; } = new() { "FLOOR", "RECEIVING", "SHIPPING" };
    }

    /// <summary>
    /// Database configuration settings.
    /// </summary>
    public class DatabaseSettings
    {
        [Range(5, 300)]
        public int CommandTimeoutSeconds { get; set; } = 30;

        [Range(1, 10)]
        public int MaxRetryAttempts { get; set; } = 3;

        [Range(1, 60)]
        public int RetryDelaySeconds { get; set; } = 5;

        public bool EnableConnectionPooling { get; set; } = true;

        [Range(1, 100)]
        public int MinPoolSize { get; set; } = 5;

        [Range(10, 1000)]
        public int MaxPoolSize { get; set; } = 100;
    }

    /// <summary>
    /// Error handling configuration settings.
    /// </summary>
    public class ErrorHandlingSettings
    {
        public bool EnableFileServerLogging { get; set; } = true;

        public bool EnableDatabaseLogging { get; set; } = false;

        public bool EnableConsoleLogging { get; set; } = false;

        [StringLength(500)]
        public string FileServerBasePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Application_Logs");

        [Range(1, 365)]
        public int LogRetentionDays { get; set; } = 30;

        [Range(1024, 1073741824)] // 1KB to 1GB
        public long MaxLogFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10MB

        public bool EnableDetailedStackTrace { get; set; } = true;
    }

    /// <summary>
    /// Logging configuration settings.
    /// </summary>
    public class LoggingSettings
    {
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;

        public bool EnableFileLogging { get; set; } = true;

        public bool EnableConsoleLogging { get; set; } = true;

        [StringLength(500)]
        public string LogFilePath { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "Application_Logs", "application.log");

        [Range(1, 365)]
        public int LogRetentionDays { get; set; } = 30;

        public bool EnableStructuredLogging { get; set; } = true;

        public bool IncludeScopes { get; set; } = true;

        public Dictionary<string, LogLevel> CategoryLevels { get; set; } = new()
        {
            ["MTM.Services"] = LogLevel.Information,
            ["MTM.ViewModels"] = LogLevel.Warning, 
            ["Microsoft"] = LogLevel.Warning,
            ["System"] = LogLevel.Error
        };
    }

    /// <summary>
    /// Configuration validation service for dependency injection.
    /// </summary>
    public class ConfigurationValidationService : IValidateOptions<MTMSettings>
    {
        public ValidateOptionsResult Validate(string? name, MTMSettings options)
        {
            var errors = new List<string>();

            // Validate operations
            if (options.ValidOperations == null || options.ValidOperations.Count == 0)
            {
                errors.Add("ValidOperations cannot be empty");
            }
            else
            {
                foreach (var operation in options.ValidOperations)
                {
                    if (string.IsNullOrWhiteSpace(operation) || !IsNumericString(operation))
                    {
                        errors.Add($"Invalid operation '{operation}' - must be a numeric string");
                    }
                }
            }

            // Validate locations
            if (options.DefaultLocations == null || options.DefaultLocations.Count == 0)
            {
                errors.Add("DefaultLocations cannot be empty");
            }

            if (errors.Count > 0)
            {
                return ValidateOptionsResult.Fail(errors);
            }

            return ValidateOptionsResult.Success;
        }

        private static bool IsNumericString(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.All(char.IsDigit);
        }
    }
}
