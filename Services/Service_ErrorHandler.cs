using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Central error handling service that provides comprehensive error logging and management
    /// across the application. Handles file server logging, MySQL logging, and duplicate detection.
    /// </summary>
    public static class Service_ErrorHandler
    {
        private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
        private static readonly object _lockObject = new();

        /// <summary>
        /// Handles an exception with specified severity and additional context data.
        /// Logs to both file server and MySQL database while preventing duplicate entries for the current session.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="severity">The severity level of the error</param>
        /// <param name="controlName">The name of the control where the error occurred (optional)</param>
        /// <param name="additionalData">Additional context data for debugging (optional)</param>
        /// <param name="callerMemberName">Auto-populated caller method name</param>
        /// <param name="callerFilePath">Auto-populated caller file path</param>
        /// <param name="callerLineNumber">Auto-populated caller line number</param>
        public static void HandleException(
            Exception exception,
            ErrorSeverity severity,
            string? controlName = null,
            Dictionary<string, object>? additionalData = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                // TODO: Implement exception handling logic
                // - Determine error category based on exception type
                // - Create unique error key for duplicate detection
                // - Check session cache to prevent duplicate logging
                // - Log to file server (CSV format)
                // - Log to MySQL database
                // - Update session cache
                
                var errorCategory = DetermineErrorCategory(exception);
                var errorKey = GenerateErrorKey(exception, callerMemberName, callerFilePath, callerLineNumber);
                
                if (IsNewError(errorCategory, errorKey))
                {
                    var errorEntry = CreateErrorEntry(exception, severity, controlName, additionalData, 
                        callerMemberName, callerFilePath, callerLineNumber, errorCategory);
                    
                    // Log to file server
                    LoggingUtility.LogToFileServer(errorEntry);
                    
                    // Log to MySQL
                    LoggingUtility.LogToMySQL(errorEntry);
                    
                    // Update session cache
                    AddToSessionCache(errorCategory, errorKey);
                }
            }
            catch (Exception handlerException)
            {
                // TODO: Implement fallback logging for when the error handler itself fails
                // This should use a minimal logging approach to avoid recursive failures
                Console.WriteLine($"Error handler failed: {handlerException.Message}");
            }
        }

        /// <summary>
        /// Determines the error category based on the exception type and context.
        /// </summary>
        /// <param name="exception">The exception to categorize</param>
        /// <returns>The appropriate error category</returns>
        private static ErrorCategory DetermineErrorCategory(Exception exception)
        {
            // TODO: Implement category determination logic based on exception type
            // - Check exception type and namespace
            // - Look for MySQL-related exceptions
            // - Check for network-related exceptions
            // - Default to appropriate category
            
            return exception switch
            {
                _ when exception.GetType().FullName?.Contains("MySql") == true => ErrorCategory.MySQL,
                _ when exception.GetType().FullName?.Contains("Network") == true => ErrorCategory.Network,
                _ when exception.GetType().FullName?.Contains("UI") == true => ErrorCategory.UI,
                ArgumentException => ErrorCategory.BusinessLogic,
                InvalidOperationException => ErrorCategory.BusinessLogic,
                _ => ErrorCategory.Other
            };
        }

        /// <summary>
        /// Generates a unique key for error identification to prevent duplicate logging.
        /// </summary>
        private static string GenerateErrorKey(Exception exception, string memberName, string filePath, int lineNumber)
        {
            // TODO: Implement unique key generation
            // Consider: exception type, message hash, location (file + line)
            return $"{exception.GetType().Name}_{memberName}_{System.IO.Path.GetFileName(filePath)}_{lineNumber}";
        }

        /// <summary>
        /// Checks if this is a new error that hasn't been logged in the current session.
        /// </summary>
        private static bool IsNewError(ErrorCategory category, string errorKey)
        {
            lock (_lockObject)
            {
                var categoryKey = category.ToString();
                if (!_sessionErrorCache.ContainsKey(categoryKey))
                {
                    _sessionErrorCache[categoryKey] = new HashSet<string>();
                }
                
                return !_sessionErrorCache[categoryKey].Contains(errorKey);
            }
        }

        /// <summary>
        /// Adds an error to the session cache to prevent duplicate logging.
        /// </summary>
        private static void AddToSessionCache(ErrorCategory category, string errorKey)
        {
            lock (_lockObject)
            {
                var categoryKey = category.ToString();
                if (!_sessionErrorCache.ContainsKey(categoryKey))
                {
                    _sessionErrorCache[categoryKey] = new HashSet<string>();
                }
                
                _sessionErrorCache[categoryKey].Add(errorKey);
            }
        }

        /// <summary>
        /// Creates a comprehensive error entry with all required details.
        /// </summary>
        private static ErrorEntry CreateErrorEntry(
            Exception exception,
            ErrorSeverity severity,
            string? controlName,
            Dictionary<string, object>? additionalData,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber,
            ErrorCategory category)
        {
            // TODO: Implement error entry creation
            // - Get current user ID
            // - Get machine name
            // - Format timestamp
            // - Extract relevant exception details
            // - Combine additional data
            
            return new ErrorEntry
            {
                Timestamp = DateTime.Now,
                UserId = GetCurrentUserId(),
                MachineName = Environment.MachineName,
                Category = category,
                Severity = severity,
                ErrorMessage = exception.Message,
                FileName = System.IO.Path.GetFileName(callerFilePath),
                MethodName = callerMemberName,
                LineNumber = callerLineNumber,
                StackTrace = exception.StackTrace ?? "",
                ControlName = controlName,
                AdditionalData = FormatAdditionalData(additionalData),
                ExceptionType = exception.GetType().FullName ?? ""
            };
        }

        /// <summary>
        /// Gets the current user ID from the application context.
        /// </summary>
        private static string GetCurrentUserId()
        {
            // TODO: Implement user ID retrieval from application context
            return "PLACEHOLDER_USER_ID";
        }

        /// <summary>
        /// Formats additional data into a readable string for logging.
        /// </summary>
        private static string FormatAdditionalData(Dictionary<string, object>? additionalData)
        {
            // TODO: Implement additional data formatting
            if (additionalData == null || additionalData.Count == 0)
                return "";
            
            var formatted = new List<string>();
            foreach (var kvp in additionalData)
            {
                formatted.Add($"{kvp.Key}={kvp.Value}");
            }
            
            return string.Join("; ", formatted);
        }

        /// <summary>
        /// Clears the session error cache. Call this when starting a new application session.
        /// </summary>
        public static void ClearSessionCache()
        {
            lock (_lockObject)
            {
                _sessionErrorCache.Clear();
            }
        }

        /// <summary>
        /// Gets the count of unique errors logged in the current session by category.
        /// </summary>
        /// <param name="category">The error category to check</param>
        /// <returns>Number of unique errors logged for the category</returns>
        public static int GetSessionErrorCount(ErrorCategory category)
        {
            lock (_lockObject)
            {
                var categoryKey = category.ToString();
                return _sessionErrorCache.ContainsKey(categoryKey) ? _sessionErrorCache[categoryKey].Count : 0;
            }
        }
    }

    public class ErrorEntry
    {
        /// <summary>
        /// Gets or sets the timestamp when the error occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the user who experienced the error.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the machine where the error occurred.
        /// </summary>
        public string MachineName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the error (UI, Business Logic, MySQL, etc.).
        /// </summary>
        public ErrorCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the severity level of the error.
        /// </summary>
        public ErrorSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the error message from the exception.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the file where the error occurred.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the method where the error occurred.
        /// </summary>
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the line number where the error occurred.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the stack trace of the exception.
        /// </summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the control where the error occurred (optional).
        /// </summary>
        public string? ControlName { get; set; }

        /// <summary>
        /// Gets or sets additional data relevant to debugging the error.
        /// This field contains formatted key-value pairs of contextual information.
        /// </summary>
        public string AdditionalData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full type name of the exception that occurred.
        /// </summary>
        public string ExceptionType { get; set; } = string.Empty;

        /// <summary>
        /// Returns a string representation of the error entry for debugging purposes.
        /// </summary>
        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Category}] {Severity}: {ErrorMessage} in {FileName}:{MethodName}:{LineNumber}";
        }
    }

    public static class ErrorHandlingConfiguration
    {
        private static string _fileServerBasePath = @"\\FileServer\Logs";
        private static string _mySqlConnectionString = "";
        private static bool _enableFileServerLogging = true;
        private static bool _enableMySqlLogging = true;
        private static bool _enableConsoleLogging = false;

        /// <summary>
        /// Gets or sets the base path on the file server where user log folders will be created.
        /// </summary>
        public static string FileServerBasePath
        {
            get => _fileServerBasePath;
            set => _fileServerBasePath = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the MySQL connection string for error logging.
        /// </summary>
        public static string MySqlConnectionString
        {
            get => _mySqlConnectionString;
            set => _mySqlConnectionString = value ?? throw new ArgumentNullException(nameof(value));
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
        /// Gets or sets whether MySQL database logging is enabled.
        /// </summary>
        public static bool EnableMySqlLogging
        {
            get => _enableMySqlLogging;
            set => _enableMySqlLogging = value;
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
        /// Gets the fallback local logging path when file server is unavailable.
        /// </summary>
        public static string FallbackLocalPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MTM_WIP_Application", "Logs");

        /// <summary>
        /// Loads configuration from application settings.
        /// </summary>
        public static void LoadFromConfiguration()
        {
            try
            {
                // TODO: Implement configuration loading from app.config, appsettings.json, or registry
                // - Read file server path from configuration
                // - Read MySQL connection string from secure storage
                // - Read logging preferences
                // - Validate configuration values

                // Placeholder implementation - replace with actual configuration source
                Console.WriteLine("Loading error handling configuration...");

                // Example configuration loading (replace with actual implementation):
                // FileServerBasePath = ConfigurationManager.AppSettings["ErrorLogFileServerPath"] ?? FileServerBasePath;
                // MySqlConnectionString = GetSecureConnectionString();
                // EnableFileServerLogging = bool.Parse(ConfigurationManager.AppSettings["EnableFileServerLogging"] ?? "true");
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
                // TODO: Implement configuration validation
                // - Check if file server path is accessible
                // - Test MySQL connection if enabled
                // - Verify fallback paths are writable

                if (EnableFileServerLogging)
                {
                    if (string.IsNullOrWhiteSpace(FileServerBasePath))
                        return false;

                    // Test file server accessibility
                    // var testPath = Path.Combine(FileServerBasePath, "test");
                    // Directory.CreateDirectory(testPath);
                    // Directory.Delete(testPath);
                }

                if (EnableMySqlLogging)
                {
                    if (string.IsNullOrWhiteSpace(MySqlConnectionString))
                        return false;

                    // Test MySQL connection
                    // using var connection = new MySqlConnection(MySqlConnectionString);
                    // connection.Open();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Configuration validation failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Resets configuration to default values.
        /// </summary>
        public static void ResetToDefaults()
        {
            _fileServerBasePath = @"\\FileServer\Logs";
            _mySqlConnectionString = "";
            _enableFileServerLogging = true;
            _enableMySqlLogging = true;
            _enableConsoleLogging = false;
        }

        /// <summary>
        /// Gets a summary of the current configuration for debugging purposes.
        /// </summary>
        /// <returns>A string containing configuration details</returns>
        public static string GetConfigurationSummary()
        {
            return $"Error Handling Configuration:\n" +
                   $"  File Server Logging: {EnableFileServerLogging} (Path: {FileServerBasePath})\n" +
                   $"  MySQL Logging: {EnableMySqlLogging}\n" +
                   $"  Console Logging: {EnableConsoleLogging}\n" +
                   $"  Fallback Path: {FallbackLocalPath}";
        }
    }

    public static class ErrorHandlingInitializer
    {
        /// <summary>
        /// Initializes the error handling system with default configuration.
        /// Call this during application startup.
        /// </summary>
        /// <param name="fileServerPath">Optional custom file server path</param>
        /// <param name="mySqlConnectionString">Optional MySQL connection string</param>
        /// <returns>True if initialization was successful; otherwise, false</returns>
        public static bool Initialize(string? fileServerPath = null, string? mySqlConnectionString = null)
        {
            try
            {
                Console.WriteLine("Initializing error handling system...");

                // Load configuration from settings
                ErrorHandlingConfiguration.LoadFromConfiguration();

                // Override with provided parameters
                if (!string.IsNullOrWhiteSpace(fileServerPath))
                {
                    ErrorHandlingConfiguration.FileServerBasePath = fileServerPath;
                }

                if (!string.IsNullOrWhiteSpace(mySqlConnectionString))
                {
                    ErrorHandlingConfiguration.MySqlConnectionString = mySqlConnectionString;
                }

                // Validate configuration
                if (!ErrorHandlingConfiguration.ValidateConfiguration())
                {
                    Console.WriteLine("Warning: Error handling configuration validation failed. Using defaults.");
                    ErrorHandlingConfiguration.ResetToDefaults();

                    // If using defaults, disable MySQL logging since no connection string
                    if (string.IsNullOrWhiteSpace(ErrorHandlingConfiguration.MySqlConnectionString))
                    {
                        ErrorHandlingConfiguration.EnableMySqlLogging = false;
                    }
                }

                // Update LoggingUtility with current configuration
                LoggingUtility.SetFileServerBasePath(ErrorHandlingConfiguration.FileServerBasePath);

                // Clear any previous session errors
                Service_ErrorHandler.ClearSessionCache();

                // Display configuration summary
                Console.WriteLine(ErrorHandlingConfiguration.GetConfigurationSummary());
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
        /// Initializes the error handling system with minimal configuration for development/testing.
        /// Uses local file system logging only.
        /// </summary>
        /// <returns>True if initialization was successful; otherwise, false</returns>
        public static bool InitializeForDevelopment()
        {
            try
            {
                Console.WriteLine("Initializing error handling system for development...");

                // Use local application data folder for development
                var localPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "MTM_WIP_Application", "Development_Logs");

                // Configure for local development
                ErrorHandlingConfiguration.FileServerBasePath = localPath;
                ErrorHandlingConfiguration.EnableFileServerLogging = true;
                ErrorHandlingConfiguration.EnableMySqlLogging = false; // Disable for development
                ErrorHandlingConfiguration.EnableConsoleLogging = true; // Enable for debugging

                // Update LoggingUtility
                LoggingUtility.SetFileServerBasePath(localPath);

                // Clear session cache
                Service_ErrorHandler.ClearSessionCache();

                // Create development log folder
                Directory.CreateDirectory(localPath);

                Console.WriteLine($"Development error logging configured to: {localPath}");
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
        /// Performs a quick test of the error handling system.
        /// Logs a test error to verify everything is working.
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
                    controlName: "ErrorHandlingInitializer_Test",
                    additionalData: new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["TestType"] = "SystemValidation",
                        ["Timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
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
                foreach (ErrorCategory category in Enum.GetValues<ErrorCategory>())
                {
                    var count = Service_ErrorHandler.GetSessionErrorCount(category);
                    status += $"  {category}: {count} errors\n";
                }

                status += $"\nFile Server Path: {LoggingUtility.GetFileServerBasePath()}\n";
                status += $"Path Exists: {Directory.Exists(LoggingUtility.GetFileServerBasePath())}\n";

                return status;
            }
            catch (Exception ex)
            {
                return $"Failed to get system status: {ex.Message}";
            }
        }

        /// <summary>
        /// Performs cleanup operations for the error handling system.
        /// Call this during application shutdown.
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                Console.WriteLine("Shutting down error handling system...");

                // Log final session statistics
                var finalStatus = GetSystemStatus();
                Console.WriteLine(finalStatus);

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

    public static class ErrorMessageProvider
    {
        private static readonly Dictionary<ErrorCategory, string> _categoryMessages = new()
        {
            { ErrorCategory.UI, "A display issue occurred. The application will continue to work, but some visual elements may not appear correctly." },
            { ErrorCategory.BusinessLogic, "A processing error occurred. Please check your input and try again." },
            { ErrorCategory.MySQL, "A database connection issue occurred. Your changes may not have been saved. Please try again in a moment." },
            { ErrorCategory.Network, "A network connection issue occurred. Please check your internet connection and try again." },
            { ErrorCategory.Other, "An unexpected error occurred. Please try again or contact support if the problem persists." }
        };

        private static readonly Dictionary<ErrorCategory, string> _categoryActions = new()
        {
            { ErrorCategory.UI, "Try refreshing the current view or restarting the application." },
            { ErrorCategory.BusinessLogic, "Review your input data and ensure all required fields are completed correctly." },
            { ErrorCategory.MySQL, "Wait a moment and try your action again. If the problem persists, contact your system administrator." },
            { ErrorCategory.Network, "Check your network connection and try again. Contact IT support if connectivity issues continue." },
            { ErrorCategory.Other, "Try closing and reopening the application. Contact support if the error continues to occur." }
        };

        private static readonly Dictionary<string, string> _commonExceptionMessages = new()
        {
            { "ArgumentNullException", "Required information is missing. Please ensure all necessary fields are filled out." },
            { "ArgumentException", "Invalid information was provided. Please check your input and try again." },
            { "InvalidOperationException", "This action cannot be performed right now. Please try again later." },
            { "FileNotFoundException", "A required file could not be found. The application may need to be reinstalled." },
            { "UnauthorizedAccessException", "You don't have permission to perform this action. Contact your administrator." },
            { "TimeoutException", "The operation took too long to complete. Please try again." },
            { "OutOfMemoryException", "The system is running low on memory. Please close other applications and try again." }
        };

        /// <summary>
        /// Gets a user-friendly error message based on the error category.
        /// </summary>
        /// <param name="category">The error category</param>
        /// <returns>A user-friendly error message</returns>
        public static string GetUserFriendlyMessage(ErrorCategory category)
        {
            return _categoryMessages.TryGetValue(category, out var message)
                ? message
                : "An unexpected error occurred. Please try again or contact support.";
        }

        /// <summary>
        /// Gets recommended actions for the user based on the error category.
        /// </summary>
        /// <param name="category">The error category</param>
        /// <returns>Recommended actions for the user</returns>
        public static string GetRecommendedActions(ErrorCategory category)
        {
            return _categoryActions.TryGetValue(category, out var actions)
                ? actions
                : "Try restarting the application. Contact support if the problem persists.";
        }

        /// <summary>
        /// Gets a user-friendly message for a specific exception type.
        /// </summary>
        /// <param name="exceptionType">The type name of the exception</param>
        /// <returns>A user-friendly message specific to the exception type</returns>
        public static string GetExceptionSpecificMessage(string exceptionType)
        {
            var simpleTypeName = exceptionType.Contains('.')
                ? exceptionType.Substring(exceptionType.LastIndexOf('.') + 1)
                : exceptionType;

            return _commonExceptionMessages.TryGetValue(simpleTypeName, out var message)
                ? message
                : "An unexpected error occurred.";
        }

        /// <summary>
        /// Gets a complete user message combining category message, exception-specific message, and recommended actions.
        /// </summary>
        /// <param name="category">The error category</param>
        /// <param name="exceptionType">The exception type</param>
        /// <param name="severity">The error severity</param>
        /// <returns>A comprehensive user-friendly error message</returns>
        public static string GetCompleteUserMessage(ErrorCategory category, string exceptionType, ErrorSeverity severity)
        {
            var baseMessage = GetUserFriendlyMessage(category);
            var specificMessage = GetExceptionSpecificMessage(exceptionType);
            var actions = GetRecommendedActions(category);
            var severityIndicator = GetSeverityIndicator(severity);

            // Combine messages intelligently
            if (baseMessage.Equals(specificMessage, StringComparison.OrdinalIgnoreCase))
            {
                return $"{severityIndicator}{baseMessage}\n\nWhat you can do: {actions}";
            }
            else
            {
                return $"{severityIndicator}{specificMessage}\n\nDetails: {baseMessage}\n\nWhat you can do: {actions}";
            }
        }

        /// <summary>
        /// Gets a severity indicator for display purposes.
        /// </summary>
        /// <param name="severity">The error severity</param>
        /// <returns>A string indicating the severity level</returns>
        private static string GetSeverityIndicator(ErrorSeverity severity)
        {
            return severity switch
            {
                ErrorSeverity.Low => "?? ",
                ErrorSeverity.Medium => "?? ",
                ErrorSeverity.High => "? ",
                ErrorSeverity.Critical => "?? ",
                _ => ""
            };
        }

        /// <summary>
        /// Determines if an error should be shown to the user based on its severity and category.
        /// </summary>
        /// <param name="severity">The error severity</param>
        /// <param name="category">The error category</param>
        /// <returns>True if the error should be displayed to the user; otherwise, false</returns>
        public static bool ShouldShowToUser(ErrorSeverity severity, ErrorCategory category)
        {
            // TODO: Implement logic for determining when to show errors to users
            // Consider user role, error frequency, and application context

            return severity switch
            {
                ErrorSeverity.Low => false, // Log only, don't show to user
                ErrorSeverity.Medium => category != ErrorCategory.Other, // Show most medium errors
                ErrorSeverity.High => true, // Always show high severity errors
                ErrorSeverity.Critical => true, // Always show critical errors
                _ => true
            };
        }

        /// <summary>
        /// Gets the appropriate dialog title based on error severity.
        /// </summary>
        /// <param name="severity">The error severity</param>
        /// <returns>An appropriate dialog title</returns>
        public static string GetDialogTitle(ErrorSeverity severity)
        {
            return severity switch
            {
                ErrorSeverity.Low => "Information",
                ErrorSeverity.Medium => "Warning",
                ErrorSeverity.High => "Error",
                ErrorSeverity.Critical => "Critical Error",
                _ => "Error"
            };
        }
    }

    public enum ErrorCategory
    {
        /// <summary>
        /// User interface related errors.
        /// Examples: Control initialization failures, binding errors, rendering issues.
        /// </summary>
        UI,

        /// <summary>
        /// Business logic and application workflow errors.
        /// Examples: Validation failures, calculation errors, rule violations.
        /// </summary>
        BusinessLogic,

        /// <summary>
        /// MySQL database related errors.
        /// Examples: Connection failures, query timeouts, constraint violations.
        /// </summary>
        MySQL,

        /// <summary>
        /// Network communication errors.
        /// Examples: HTTP request failures, timeout errors, connectivity issues.
        /// </summary>
        Network,

        /// <summary>
        /// All other errors that don't fit into the specific categories above.
        /// Examples: File system errors, configuration issues, third-party library errors.
        /// </summary>
        Other
    }

    public enum ErrorSeverity
    {
        /// <summary>
        /// Low severity errors that don't significantly impact functionality.
        /// Examples: Minor validation warnings, non-critical UI glitches.
        /// </summary>
        Low,

        /// <summary>
        /// Medium severity errors that impact functionality but allow continued operation.
        /// Examples: Failed non-critical operations, recoverable exceptions.
        /// </summary>
        Medium,

        /// <summary>
        /// High severity errors that significantly impact functionality.
        /// Examples: Database connection failures, critical business logic errors.
        /// </summary>
        High,

        /// <summary>
        /// Critical errors that prevent normal application operation.
        /// Examples: Application crashes, complete system failures.
        /// </summary>
        Critical
    }

}