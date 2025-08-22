using System;
using System.Collections.Generic;
using System.IO;

namespace MTM.Core.Services
{
    /// <summary>
    /// Configuration settings for the error handling system.
    /// Framework-agnostic implementation suitable for any .NET application.
    /// </summary>
    public static class ErrorHandlingConfiguration
    {
        private static string _fileServerBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Application_Logs");
        private static bool _enableFileServerLogging = true;
        private static bool _enableDatabaseLogging = false;
        private static bool _enableConsoleLogging = false;

        /// <summary>
        /// Gets or sets the base path where user log folders will be created.
        /// </summary>
        public static string FileServerBasePath
        {
            get => _fileServerBasePath;
            set => _fileServerBasePath = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets whether file server logging is enabled.
        /// </summary>
        public static bool EnableFileServerLogging
        {
            get => _enableFileServerLogging;
            set => _enableFileServerLogging = value;
        }

        /// <summary>
        /// Gets or sets whether database logging is enabled.
        /// </summary>
        public static bool EnableDatabaseLogging
        {
            get => _enableDatabaseLogging;
            set => _enableDatabaseLogging = value;
        }

        /// <summary>
        /// Gets or sets whether console logging is enabled for debugging purposes.
        /// </summary>
        public static bool EnableConsoleLogging
        {
            get => _enableConsoleLogging;
            set => _enableConsoleLogging = value;
        }

        /// <summary>
        /// Gets the fallback local logging path when primary logging is unavailable.
        /// </summary>
        public static string FallbackLocalPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Application_Logs", "Fallback");

        /// <summary>
        /// Loads configuration from the provided configuration provider.
        /// </summary>
        /// <param name="configProvider">Configuration provider (optional)</param>
        public static void LoadFromConfiguration(IConfigurationProvider? configProvider = null)
        {
            try
            {
                if (configProvider == null)
                {
                    // Use default configuration
                    return;
                }

                // Load settings from configuration provider
                FileServerBasePath = configProvider.GetValue("ErrorHandling:FileServerBasePath") ?? FileServerBasePath;
                EnableFileServerLogging = configProvider.GetBoolValue("ErrorHandling:EnableFileServerLogging") ?? EnableFileServerLogging;
                EnableDatabaseLogging = configProvider.GetBoolValue("ErrorHandling:EnableDatabaseLogging") ?? EnableDatabaseLogging;
                EnableConsoleLogging = configProvider.GetBoolValue("ErrorHandling:EnableConsoleLogging") ?? EnableConsoleLogging;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load error handling configuration: {ex.Message}");
                // Use default values if configuration loading fails
            }
        }

        /// <summary>
        /// Validates the current configuration settings.
        /// </summary>
        /// <returns>True if configuration is valid; otherwise, false.</returns>
        public static bool ValidateConfiguration()
        {
            try
            {
                if (EnableFileServerLogging)
                {
                    if (string.IsNullOrWhiteSpace(FileServerBasePath))
                        return false;

                    // Test file server accessibility
                    var testPath = Path.Combine(FileServerBasePath, "test_" + Guid.NewGuid().ToString("N")[..8]);
                    Directory.CreateDirectory(testPath);
                    Directory.Delete(testPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                if (EnableConsoleLogging)
                {
                    Console.WriteLine($"Configuration validation failed: {ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// Resets configuration to default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            _fileServerBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Application_Logs");
            _enableFileServerLogging = true;
            _enableDatabaseLogging = false;
            _enableConsoleLogging = false;
        }

        /// <summary>
        /// Gets a summary of the current configuration for debugging purposes.
        /// </summary>
        /// <returns>A string containing configuration details</returns>
        public static string GetConfigurationSummary()
        {
            return $"Error Handling Configuration:\n" +
                   $"  File Logging: {EnableFileServerLogging} (Path: {FileServerBasePath})\n" +
                   $"  Database Logging: {EnableDatabaseLogging}\n" +
                   $"  Console Logging: {EnableConsoleLogging}\n" +
                   $"  Fallback Path: {FallbackLocalPath}";
        }

        /// <summary>
        /// Applies configuration for development environment.
        /// </summary>
        public static void ConfigureForDevelopment()
        {
            var localPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Application_Logs", "Development");

            FileServerBasePath = localPath;
            EnableFileServerLogging = true;
            EnableDatabaseLogging = false;
            EnableConsoleLogging = true;
        }

        /// <summary>
        /// Applies configuration for production environment.
        /// </summary>
        public static void ConfigureForProduction()
        {
            EnableFileServerLogging = true;
            EnableDatabaseLogging = true;
            EnableConsoleLogging = false;
        }
    }

    /// <summary>
    /// Interface for configuration providers to integrate with various configuration systems.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets a string configuration value.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value or null if not found</returns>
        string? GetValue(string key);

        /// <summary>
        /// Gets a boolean configuration value.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value or null if not found</returns>
        bool? GetBoolValue(string key);

        /// <summary>
        /// Gets an integer configuration value.
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value or null if not found</returns>
        int? GetIntValue(string key);
    }

    /// <summary>
    /// Error handling system initializer.
    /// </summary>
    public static class ErrorHandlingInitializer
    {
        /// <summary>
        /// Initializes the error handling system with configuration.
        /// </summary>
        /// <param name="configProvider">Optional configuration provider</param>
        /// <param name="connectionFactory">Optional database connection factory</param>
        /// <returns>True if initialization was successful; otherwise, false</returns>
        public static bool Initialize(IConfigurationProvider? configProvider = null, IDbConnectionFactory? connectionFactory = null)
        {
            try
            {
                Console.WriteLine("Initializing error handling system...");

                // Load configuration
                ErrorHandlingConfiguration.LoadFromConfiguration(configProvider);

                // Set database connection factory if provided
                if (connectionFactory != null)
                {
                    LoggingUtility.SetConnectionFactory(connectionFactory);
                }

                // Validate configuration
                if (!ErrorHandlingConfiguration.ValidateConfiguration())
                {
                    Console.WriteLine("Warning: Error handling configuration validation failed. Using defaults.");
                    ErrorHandlingConfiguration.ResetToDefaults();
                }

                // Update LoggingUtility with current configuration
                LoggingUtility.SetFileServerBasePath(ErrorHandlingConfiguration.FileServerBasePath);

                // Clear any previous session errors
                Service_ErrorHandler.ClearSessionCache();

                // Display configuration summary
                if (ErrorHandlingConfiguration.EnableConsoleLogging)
                {
                    Console.WriteLine(ErrorHandlingConfiguration.GetConfigurationSummary());
                }

                Console.WriteLine("Error handling system initialized successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize error handling system: {ex.Message}");
                LoggingUtility.LogApplicationError(ex);
                return false;
            }
        }

        /// <summary>
        /// Initializes the error handling system for development environment.
        /// </summary>
        /// <returns>True if initialization was successful; otherwise, false</returns>
        public static bool InitializeForDevelopment()
        {
            try
            {
                Console.WriteLine("Initializing error handling system for development...");

                // Configure for development
                ErrorHandlingConfiguration.ConfigureForDevelopment();

                // Update LoggingUtility
                LoggingUtility.SetFileServerBasePath(ErrorHandlingConfiguration.FileServerBasePath);

                // Clear session cache
                Service_ErrorHandler.ClearSessionCache();

                // Create development log folder
                Directory.CreateDirectory(ErrorHandlingConfiguration.FileServerBasePath);

                Console.WriteLine($"Development error logging configured to: {ErrorHandlingConfiguration.FileServerBasePath}");
                Console.WriteLine("Error handling system initialized for development.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize error handling for development: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Initializes the error handling system for production environment.
        /// </summary>
        /// <param name="configProvider">Configuration provider</param>
        /// <param name="connectionFactory">Database connection factory</param>
        /// <returns>True if initialization was successful; otherwise, false</returns>
        public static bool InitializeForProduction(IConfigurationProvider configProvider, IDbConnectionFactory? connectionFactory = null)
        {
            try
            {
                Console.WriteLine("Initializing error handling system for production...");

                // Configure for production
                ErrorHandlingConfiguration.ConfigureForProduction();

                // Load additional configuration
                ErrorHandlingConfiguration.LoadFromConfiguration(configProvider);

                // Set database connection factory
                if (connectionFactory != null)
                {
                    LoggingUtility.SetConnectionFactory(connectionFactory);
                }

                // Validate configuration
                if (!ErrorHandlingConfiguration.ValidateConfiguration())
                {
                    throw new InvalidOperationException("Error handling configuration validation failed in production environment");
                }

                // Update LoggingUtility
                LoggingUtility.SetFileServerBasePath(ErrorHandlingConfiguration.FileServerBasePath);

                // Clear session cache
                Service_ErrorHandler.ClearSessionCache();

                Console.WriteLine("Error handling system initialized for production.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize error handling for production: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Performs a quick test of the error handling system.
        /// </summary>
        /// <returns>True if test was successful; otherwise, false</returns>
        public static bool RunSystemTest()
        {
            try
            {
                Console.WriteLine("Running error handling system test...");

                // Create a test exception
                var testException = new InvalidOperationException("This is a test error for system validation");

                // Log the test error
                Service_ErrorHandler.HandleException(testException, ErrorSeverity.Low,
                    source: "ErrorHandlingInitializer_Test",
                    additionalData: new Dictionary<string, object>
                    {
                        ["TestType"] = "SystemValidation",
                        ["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                        ["Purpose"] = "Verify error handling system is working correctly"
                    });

                // Check if error was logged
                var errorCount = Service_ErrorHandler.GetSessionErrorCount(ErrorCategory.Other);

                Console.WriteLine($"Test completed. Error count: {errorCount}");
                Console.WriteLine("Error handling system test passed.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling system test failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets a status report of the error handling system.
        /// </summary>
        /// <returns>A formatted string containing system status information</returns>
        public static string GetSystemStatus()
        {
            try
            {
                var status = "Error Handling System Status:\n";
                status += "================================\n";
                status += ErrorHandlingConfiguration.GetConfigurationSummary() + "\n\n";

                status += "Session Error Counts:\n";
                var stats = Service_ErrorHandler.GetSessionErrorStatistics();
                foreach (var kvp in stats)
                {
                    status += $"  {kvp.Key}: {kvp.Value} errors\n";
                }

                var loggingStats = LoggingUtility.GetStatistics();
                status += $"\n{loggingStats}\n";

                return status;
            }
            catch (Exception ex)
            {
                return $"Failed to get system status: {ex.Message}";
            }
        }

        /// <summary>
        /// Performs cleanup operations for the error handling system.
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                Console.WriteLine("Shutting down error handling system...");

                // Log final session statistics
                if (ErrorHandlingConfiguration.EnableConsoleLogging)
                {
                    var finalStatus = GetSystemStatus();
                    Console.WriteLine(finalStatus);
                }

                // Clear session cache
                Service_ErrorHandler.ClearSessionCache();

                Console.WriteLine("Error handling system shutdown complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during error handling system shutdown: {ex.Message}");
            }
        }
    }
}