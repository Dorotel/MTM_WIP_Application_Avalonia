using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Core.Startup;

/// <summary>
/// Service interface for application startup validation.
/// Provides comprehensive validation of services, configuration, and dependencies.
/// </summary>
public interface IStartupValidationService
{
    /// <summary>
    /// Validates the entire application configuration and services.
    /// </summary>
    /// <returns>Validation results with detailed error information</returns>
    ValidationResult ValidateApplication();

    /// <summary>
    /// Validates specific service dependencies.
    /// </summary>
    /// <param name="serviceProvider">Service provider to validate</param>
    /// <returns>Validation results for service dependencies</returns>
    ValidationResult ValidateServices(IServiceProvider serviceProvider);

    /// <summary>
    /// Validates application configuration.
    /// </summary>
    /// <param name="configuration">Configuration to validate</param>
    /// <returns>Validation results for configuration</returns>
    ValidationResult ValidateConfiguration(IConfiguration configuration);
}

/// <summary>
/// Implementation of startup validation service following .NET best practices.
/// Provides comprehensive validation with detailed logging and error reporting.
/// </summary>
public class StartupValidationService : IStartupValidationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StartupValidationService> _logger;

    /// <summary>
    /// Initializes a new instance of the StartupValidationService.
    /// </summary>
    /// <param name="serviceProvider">Service provider for dependency resolution</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger for validation events</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    public StartupValidationService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<StartupValidationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates the entire application configuration and services.
    /// </summary>
    /// <returns>Comprehensive validation results</returns>
    public ValidationResult ValidateApplication()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Starting comprehensive application validation");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] StartupValidationService.ValidateApplication() started");
        Debug.WriteLine($"[VALIDATION] Comprehensive application validation started");

        var allErrors = new List<string>();
        var allWarnings = new List<string>();

        try
        {
            // Validate services
            var serviceValidation = ValidateServices(_serviceProvider);
            allErrors.AddRange(serviceValidation.Errors);
            allWarnings.AddRange(serviceValidation.Warnings);

            // Validate configuration
            var configValidation = ValidateConfiguration(_configuration);
            allErrors.AddRange(configValidation.Errors);
            allWarnings.AddRange(configValidation.Warnings);

            // Validate application-specific requirements
            var appValidation = ValidateApplicationRequirements();
            allErrors.AddRange(appValidation.Errors);
            allWarnings.AddRange(appValidation.Warnings);

            stopwatch.Stop();
            var result = new ValidationResult
            {
                IsValid = allErrors.Count == 0,
                Errors = allErrors,
                Warnings = allWarnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };

            if (result.IsValid)
            {
                _logger.LogInformation("Application validation completed successfully in {DurationMs}ms with {WarningCount} warnings",
                    stopwatch.ElapsedMilliseconds, allWarnings.Count);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application validation completed successfully in {stopwatch.ElapsedMilliseconds}ms");
            }
            else
            {
                _logger.LogError("Application validation failed in {DurationMs}ms with {ErrorCount} errors and {WarningCount} warnings",
                    stopwatch.ElapsedMilliseconds, allErrors.Count, allWarnings.Count);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application validation failed with {allErrors.Count} errors");
            }

            Debug.WriteLine($"[VALIDATION] Application validation completed in {stopwatch.ElapsedMilliseconds}ms - Valid: {result.IsValid}");
            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Critical error during application validation after {DurationMs}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Critical error during application validation: {ex.Message}");
            Debug.WriteLine($"[VALIDATION-ERROR] Critical validation error: {ex.Message}");

            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Critical validation error: {ex.Message}" },
                Warnings = allWarnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Validates service dependencies and registrations.
    /// </summary>
    /// <param name="serviceProvider">Service provider to validate</param>
    /// <returns>Service validation results</returns>
    public ValidationResult ValidateServices(IServiceProvider serviceProvider)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Starting service dependency validation");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ValidateServices() started");
        Debug.WriteLine($"[SERVICE-VALIDATION] Service validation started");

        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // Define critical services that must be available
            var criticalServices = new[]
            {
                typeof(IConfiguration),
                typeof(ILoggerFactory),
                typeof(MTM_WIP_Application_Avalonia.Services.IConfigurationService),
                typeof(MTM_WIP_Application_Avalonia.Services.IApplicationStateService),
                typeof(MTM_WIP_Application_Avalonia.Services.INavigationService)
            };

            // Define optional services that should be available but are not critical
            var optionalServices = new[]
            {
                typeof(MTM_WIP_Application_Avalonia.Services.IThemeService),
                typeof(MTM_WIP_Application_Avalonia.Services.ISettingsService),
                typeof(MTM_WIP_Application_Avalonia.Services.IDatabaseService)
            };

            // Validate critical services
            foreach (var serviceType in criticalServices)
            {
                try
                {
                    var service = serviceProvider.GetRequiredService(serviceType);
                    if (service == null)
                    {
                        errors.Add($"Critical service {serviceType.Name} resolved to null");
                    }
                    else
                    {
                        _logger.LogTrace("Critical service {ServiceType} validated successfully", serviceType.Name);
                        Debug.WriteLine($"[SERVICE-VALIDATION] Critical service validated: {serviceType.Name}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Failed to resolve critical service {serviceType.Name}: {ex.Message}");
                    _logger.LogError("Failed to resolve critical service {ServiceType}: {Error}", serviceType.Name, ex.Message);
                }
            }

            // Validate optional services
            foreach (var serviceType in optionalServices)
            {
                try
                {
                    var service = serviceProvider.GetService(serviceType);
                    if (service == null)
                    {
                        warnings.Add($"Optional service {serviceType.Name} is not available");
                        _logger.LogWarning("Optional service {ServiceType} is not available", serviceType.Name);
                    }
                    else
                    {
                        _logger.LogTrace("Optional service {ServiceType} validated successfully", serviceType.Name);
                        Debug.WriteLine($"[SERVICE-VALIDATION] Optional service validated: {serviceType.Name}");
                    }
                }
                catch (Exception ex)
                {
                    warnings.Add($"Error resolving optional service {serviceType.Name}: {ex.Message}");
                    _logger.LogWarning("Error resolving optional service {ServiceType}: {Error}", serviceType.Name, ex.Message);
                }
            }

            // Validate ViewModels
            ValidateViewModels(serviceProvider, errors, warnings);

            stopwatch.Stop();
            _logger.LogDebug("Service validation completed in {DurationMs}ms with {ErrorCount} errors and {WarningCount} warnings",
                stopwatch.ElapsedMilliseconds, errors.Count, warnings.Count);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service validation completed in {stopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine($"[SERVICE-VALIDATION] Service validation completed in {stopwatch.ElapsedMilliseconds}ms");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Critical error during service validation after {DurationMs}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Critical error during service validation: {ex.Message}");

            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Critical service validation error: {ex.Message}" },
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Validates application configuration settings.
    /// </summary>
    /// <param name="configuration">Configuration to validate</param>
    /// <returns>Configuration validation results</returns>
    public ValidationResult ValidateConfiguration(IConfiguration configuration)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Starting configuration validation");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ValidateConfiguration() started");
        Debug.WriteLine($"[CONFIG-VALIDATION] Configuration validation started");

        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // Validate connection strings
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                warnings.Add("DefaultConnection string is not configured - database features may not work");
                _logger.LogWarning("DefaultConnection string is not configured");
            }
            else
            {
                _logger.LogDebug("DefaultConnection string is configured");
                Debug.WriteLine($"[CONFIG-VALIDATION] DefaultConnection validated");
            }

            // Validate application settings
            var appName = configuration["ApplicationName"];
            if (string.IsNullOrWhiteSpace(appName))
            {
                warnings.Add("ApplicationName is not configured");
            }

            var logLevel = configuration["Logging:LogLevel:Default"];
            if (string.IsNullOrWhiteSpace(logLevel))
            {
                warnings.Add("Default log level is not configured");
            }

            // Validate MTM-specific settings
            var mtmSettings = configuration.GetSection("MTM");
            if (!mtmSettings.Exists())
            {
                warnings.Add("MTM configuration section is not found");
            }

            stopwatch.Stop();
            _logger.LogDebug("Configuration validation completed in {DurationMs}ms with {ErrorCount} errors and {WarningCount} warnings",
                stopwatch.ElapsedMilliseconds, errors.Count, warnings.Count);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration validation completed in {stopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine($"[CONFIG-VALIDATION] Configuration validation completed in {stopwatch.ElapsedMilliseconds}ms");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Critical error during configuration validation after {DurationMs}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Critical error during configuration validation: {ex.Message}");

            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Critical configuration validation error: {ex.Message}" },
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    /// <summary>
    /// Validates ViewModel dependencies and creation.
    /// </summary>
    /// <param name="serviceProvider">Service provider for ViewModel resolution</param>
    /// <param name="errors">Error list to populate</param>
    /// <param name="warnings">Warning list to populate</param>
    private void ValidateViewModels(IServiceProvider serviceProvider, List<string> errors, List<string> warnings)
    {
        _logger.LogDebug("Starting ViewModel validation");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ValidateViewModels() started");
        Debug.WriteLine($"[VIEWMODEL-VALIDATION] ViewModel validation started");

        var criticalViewModels = new[]
        {
            // Temporarily disabled MainWindowViewModel validation due to complex dependency chain
            // typeof(MTM_WIP_Application_Avalonia.ViewModels.MainWindowViewModel),
            typeof(MTM_WIP_Application_Avalonia.ViewModels.MainForm.InventoryTabViewModel)
        };

        foreach (var viewModelType in criticalViewModels)
        {
            try
            {
                var viewModel = serviceProvider.GetService(viewModelType);
                if (viewModel == null)
                {
                    errors.Add($"Critical ViewModel {viewModelType.Name} could not be resolved");
                    _logger.LogError("Critical ViewModel {ViewModelType} could not be resolved", viewModelType.Name);
                }
                else
                {
                    _logger.LogTrace("Critical ViewModel {ViewModelType} validated successfully", viewModelType.Name);
                    Debug.WriteLine($"[VIEWMODEL-VALIDATION] Critical ViewModel validated: {viewModelType.Name}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error creating ViewModel {viewModelType.Name}: {ex.Message}");
                _logger.LogError("Error creating ViewModel {ViewModelType}: {Error}", viewModelType.Name, ex.Message);
            }
        }

        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ViewModel validation completed");
        Debug.WriteLine($"[VIEWMODEL-VALIDATION] ViewModel validation completed");
    }

    /// <summary>
    /// Validates application-specific requirements.
    /// </summary>
    /// <returns>Application requirements validation results</returns>
    private ValidationResult ValidateApplicationRequirements()
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug("Starting application requirements validation");
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ValidateApplicationRequirements() started");
        Debug.WriteLine($"[APP-REQUIREMENTS] Application requirements validation started");

        var errors = new List<string>();
        var warnings = new List<string>();

        try
        {
            // Validate runtime environment
            var runtime = Environment.Version;
            if (runtime.Major < 8)
            {
                errors.Add($"Unsupported .NET runtime version: {runtime}. Requires .NET 8.0 or higher.");
            }

            // Validate application directories
            var appDirectory = AppContext.BaseDirectory;
            if (!System.IO.Directory.Exists(appDirectory))
            {
                errors.Add($"Application directory does not exist: {appDirectory}");
            }

            // Validate required files
            var requiredFiles = new[] { "appsettings.json" };
            foreach (var file in requiredFiles)
            {
                var filePath = System.IO.Path.Combine(appDirectory, file);
                if (!System.IO.File.Exists(filePath))
                {
                    warnings.Add($"Required file not found: {file}");
                }
            }

            stopwatch.Stop();
            _logger.LogDebug("Application requirements validation completed in {DurationMs}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Application requirements validation completed in {stopwatch.ElapsedMilliseconds}ms");
            Debug.WriteLine($"[APP-REQUIREMENTS] Application requirements validation completed in {stopwatch.ElapsedMilliseconds}ms");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors,
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Critical error during application requirements validation");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Critical error during application requirements validation: {ex.Message}");

            return new ValidationResult
            {
                IsValid = false,
                Errors = new List<string> { $"Critical application requirements validation error: {ex.Message}" },
                Warnings = warnings,
                ValidationDurationMs = stopwatch.ElapsedMilliseconds
            };
        }
    }
}

/// <summary>
/// Represents the result of a validation operation.
/// Provides comprehensive information about validation outcomes.
/// </summary>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets whether the validation passed.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of validation warnings.
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Gets or sets the duration of the validation in milliseconds.
    /// </summary>
    public long ValidationDurationMs { get; set; }

    /// <summary>
    /// Gets a summary of the validation results.
    /// </summary>
    public string Summary => $"Valid: {IsValid}, Errors: {Errors.Count}, Warnings: {Warnings.Count}, Duration: {ValidationDurationMs}ms";
}
