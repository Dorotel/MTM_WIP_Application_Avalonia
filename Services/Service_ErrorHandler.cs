using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reactive;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Central error handling service that provides comprehensive error logging and management
    /// across the application. Integrates with ReactiveUI patterns and MTM business operations.
    /// </summary>
    public static class Service_ErrorHandler
    {
        private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
        private static readonly object _lockObject = new();

        /// <summary>
        /// Handles an exception with specified operation context and user information.
        /// Provides structured logging with database fallback and MTM business context.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="operation">The business operation being performed</param>
        /// <param name="userId">The current user ID</param>
        /// <param name="context">Additional context information for business operations</param>
        /// <param name="callerMemberName">Auto-populated caller method name</param>
        /// <param name="callerFilePath">Auto-populated caller file path</param>
        /// <param name="callerLineNumber">Auto-populated caller line number</param>
        public static async Task HandleErrorAsync(
            Exception exception,
            string operation,
            string userId,
            Dictionary<string, object>? context = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                var errorCategory = DetermineErrorCategory(exception);
                var severity = DetermineSeverity(exception, errorCategory);
                var errorKey = GenerateErrorKey(exception, callerMemberName, callerFilePath, callerLineNumber);
                
                if (IsNewError(errorCategory, errorKey))
                {
                    await LogErrorAsync(exception, operation, userId, context ?? new Dictionary<string, object>());
                    AddToSessionCache(errorCategory, errorKey);
                }
            }
            catch (Exception handlerException)
            {
                // Fallback to file logging if structured logging fails
                await LogToFileAsync(handlerException, operation, userId, new Dictionary<string, object>
                {
                    ["OriginalError"] = exception.Message,
                    ["HandlerFailure"] = true
                });
            }
        }

        /// <summary>
        /// Legacy method for backwards compatibility. Use HandleErrorAsync for new implementations.
        /// </summary>
        public static void HandleException(
            Exception exception,
            ErrorSeverity severity,
            string? source = null,
            Dictionary<string, object>? additionalData = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            // Convert to new async pattern
            _ = Task.Run(async () =>
            {
                await HandleErrorAsync(exception, source ?? callerMemberName, GetCurrentUserId(), 
                    additionalData, callerMemberName, callerFilePath, callerLineNumber);
            });
        }

        /// <summary>
        /// Structured error logging with stored procedure integration and file fallback.
        /// </summary>
        public static async Task LogErrorAsync(Exception ex, string operation, string userId, Dictionary<string, object> context)
        {
            try
            {
                // Try database logging first using stored procedures
                var parameters = new Dictionary<string, object>
                {
                    ["ErrorMessage"] = ex.Message,
                    ["StackTrace"] = ex.StackTrace ?? "",
                    ["Operation"] = operation,
                    ["UserId"] = userId,
                    ["Context"] = JsonSerializer.Serialize(context),
                    ["Timestamp"] = DateTime.UtcNow,
                    ["MachineName"] = Environment.MachineName,
                    ["ExceptionType"] = ex.GetType().FullName ?? "",
                    ["Severity"] = DetermineSeverity(ex, DetermineErrorCategory(ex)).ToString()
                };

                // Use MTM database pattern with stored procedures
                if (ErrorHandlingConfiguration.EnableMySqlLogging)
                {
                    try
                    {
                        // TODO: Replace with actual Helper_Database_StoredProcedure call when available
                        // var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                        //     Model_AppVariables.ConnectionString,
                        //     "sys_error_Log_Insert",
                        //     parameters
                        // );
                        
                        // For now, use existing MySQL logging
                        var errorEntry = CreateErrorEntry(ex, operation, userId, context);
                        await LoggingUtility.LogToMySQL(errorEntry);
                    }
                    catch
                    {
                        // Database logging failed, continue to file fallback
                    }
                }
            }
            catch
            {
                // Continue to file fallback
            }

            // Fallback to file logging
            await LogToFileAsync(ex, operation, userId, context);
        }

        /// <summary>
        /// File-based logging fallback when database logging fails.
        /// </summary>
        public static async Task LogToFileAsync(Exception ex, string operation, string userId, Dictionary<string, object> context)
        {
            try
            {
                var errorEntry = CreateErrorEntry(ex, operation, userId, context);
                LoggingUtility.LogToFileServer(errorEntry);
            }
            catch (Exception fileEx)
            {
                // Last resort - console logging
                Console.WriteLine($"Critical: All logging failed. Error: {ex.Message}, Logging Error: {fileEx.Message}");
            }
        }

        /// <summary>
        /// Gets a user-friendly error message while hiding technical details.
        /// </summary>
        public static string GetUserFriendlyMessage(Exception ex)
        {
            var category = DetermineErrorCategory(ex);
            var severity = DetermineSeverity(ex, category);
            return ErrorMessageProvider.GetCompleteUserMessage(category, ex.GetType().FullName ?? "", severity);
        }

        /// <summary>
        /// Determines if an error should be displayed to the user based on context.
        /// </summary>
        public static bool ShouldShowToUser(Exception ex)
        {
            var category = DetermineErrorCategory(ex);
            var severity = DetermineSeverity(ex, category);
            return ErrorMessageProvider.ShouldShowToUser(severity, category);
        }

        /// <summary>
        /// Determines error category based on exception type and context.
        /// Enhanced to support MTM business operations.
        /// </summary>
        private static ErrorCategory DetermineErrorCategory(Exception exception)
        {
            return exception switch
            {
                _ when exception.GetType().FullName?.Contains("MySql") == true => ErrorCategory.MySQL,
                _ when exception.GetType().FullName?.Contains("Network") == true => ErrorCategory.Network,
                _ when exception.GetType().FullName?.Contains("Http") == true => ErrorCategory.Network,
                _ when exception.GetType().FullName?.Contains("Socket") == true => ErrorCategory.Network,
                _ when exception.GetType().FullName?.Contains("UI") == true => ErrorCategory.UI,
                _ when exception.GetType().FullName?.Contains("Avalonia") == true => ErrorCategory.UI,
                _ when exception.GetType().FullName?.Contains("ReactiveUI") == true => ErrorCategory.UI,
                ArgumentException => ErrorCategory.BusinessLogic,
                InvalidOperationException => ErrorCategory.BusinessLogic,
                NotSupportedException => ErrorCategory.BusinessLogic,
                UnauthorizedAccessException => ErrorCategory.BusinessLogic,
                TimeoutException => ErrorCategory.Network,
                _ => ErrorCategory.Other
            };
        }

        /// <summary>
        /// Determines severity based on exception type and category.
        /// </summary>
        private static ErrorSeverity DetermineSeverity(Exception exception, ErrorCategory category)
        {
            return exception switch
            {
                OutOfMemoryException => ErrorSeverity.Critical,
                StackOverflowException => ErrorSeverity.Critical,
                _ when exception.GetType().FullName?.Contains("Critical") == true => ErrorSeverity.Critical,
                UnauthorizedAccessException => ErrorSeverity.High,
                TimeoutException when category == ErrorCategory.MySQL => ErrorSeverity.High,
                InvalidOperationException when category == ErrorCategory.BusinessLogic => ErrorSeverity.High,
                ArgumentException => ErrorSeverity.Medium,
                _ when category == ErrorCategory.UI => ErrorSeverity.Medium,
                _ => ErrorSeverity.Low
            };
        }

        /// <summary>
        /// Creates a comprehensive error entry with MTM business context.
        /// </summary>
        private static ErrorEntry CreateErrorEntry(
            Exception exception,
            string operation,
            string userId,
            Dictionary<string, object> context)
        {
            return new ErrorEntry
            {
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                MachineName = Environment.MachineName,
                Category = DetermineErrorCategory(exception),
                Severity = DetermineSeverity(exception, DetermineErrorCategory(exception)),
                ErrorMessage = exception.Message,
                FileName = "", // Will be populated by caller info
                MethodName = operation,
                LineNumber = 0, // Will be populated by caller info
                StackTrace = exception.StackTrace ?? "",
                source = operation,
                AdditionalData = JsonSerializer.Serialize(context),
                ExceptionType = exception.GetType().FullName ?? "",
                BusinessContext = ExtractBusinessContext(context)
            };
        }

        /// <summary>
        /// Extracts MTM-specific business context from the context dictionary.
        /// </summary>
        private static string ExtractBusinessContext(Dictionary<string, object> context)
        {
            var businessItems = new List<string>();

            // Extract common MTM business context
            if (context.TryGetValue("PartId", out var partId))
                businessItems.Add($"PartId={partId}");
            if (context.TryGetValue("Operation", out var operation))
                businessItems.Add($"Operation={operation}");
            if (context.TryGetValue("Quantity", out var quantity))
                businessItems.Add($"Quantity={quantity}");
            if (context.TryGetValue("Location", out var location))
                businessItems.Add($"Location={location}");
            if (context.TryGetValue("TransactionType", out var transactionType))
                businessItems.Add($"TransactionType={transactionType}");
            if (context.TryGetValue("StoredProcedure", out var storedProcedure))
                businessItems.Add($"StoredProcedure={storedProcedure}");

            return string.Join("; ", businessItems);
        }

        private static string GenerateErrorKey(Exception exception, string memberName, string filePath, int lineNumber)
        {
            return $"{exception.GetType().Name}_{memberName}_{Path.GetFileName(filePath)}_{lineNumber}";
        }

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

        private static string GetCurrentUserId()
        {
            // TODO: Integrate with ApplicationStateService or similar to get actual user ID
            return Environment.UserName; // Temporary fallback
        }

        public static void ClearSessionCache()
        {
            lock (_lockObject)
            {
                _sessionErrorCache.Clear();
            }
        }

        public static int GetSessionErrorCount(ErrorCategory category)
        {
            lock (_lockObject)
            {
                var categoryKey = category.ToString();
                return _sessionErrorCache.ContainsKey(categoryKey) ? _sessionErrorCache[categoryKey].Count : 0;
            }
        }
    }

    /// <summary>
    /// Enhanced error entry with MTM business context support.
    /// </summary>
    public class ErrorEntry
    {
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public ErrorCategory Category { get; set; }
        public ErrorSeverity Severity { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public string StackTrace { get; set; } = string.Empty;
        public string? source { get; set; }
        public string AdditionalData { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        
        /// <summary>
        /// MTM-specific business context (PartId, Operation, TransactionType, etc.)
        /// </summary>
        public string BusinessContext { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Category}] {Severity}: {ErrorMessage} in {FileName}:{MethodName}:{LineNumber}";
        }
    }

    /// <summary>
    /// ReactiveUI integration patterns for centralized error handling.
    /// </summary>
    public static class ReactiveUIErrorExtensions
    {
        /// <summary>
        /// Subscribe to command exceptions with centralized error handling.
        /// </summary>
        public static IDisposable SubscribeToErrors<T>(
            this ReactiveUI.ReactiveCommand<Unit, T> command,
            string operation,
            string userId,
            Dictionary<string, object>? context = null)
        {
            return command.ThrownExceptions.Subscribe(async ex =>
            {
                await Service_ErrorHandler.HandleErrorAsync(ex, operation, userId, context);
            });
        }

        /// <summary>
        /// Subscribe to command exceptions with user notification.
        /// </summary>
        public static IDisposable SubscribeToErrorsWithNotification<T>(
            this ReactiveUI.ReactiveCommand<Unit, T> command,
            string operation,
            string userId,
            Action<string> showError,
            Dictionary<string, object>? context = null)
        {
            return command.ThrownExceptions.Subscribe(async ex =>
            {
                await Service_ErrorHandler.HandleErrorAsync(ex, operation, userId, context);
                
                if (Service_ErrorHandler.ShouldShowToUser(ex))
                {
                    var userMessage = Service_ErrorHandler.GetUserFriendlyMessage(ex);
                    showError(userMessage);
                }
            });
        }
    }

    public enum ErrorCategory
    {
        UI,
        BusinessLogic,
        MySQL,
        Network,
        Other
    }

    public enum ErrorSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public static class ErrorHandlingConfiguration
    {
        private static string _fileServerBasePath = @"\\FileServer\Logs";
        private static string _mySqlConnectionString = "";
        private static bool _enableFileServerLogging = true;
        private static bool _enableMySqlLogging = true;
        private static bool _enableConsoleLogging = false;

        public static string FileServerBasePath
        {
            get => _fileServerBasePath;
            set => _fileServerBasePath = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static string MySqlConnectionString
        {
            get => _mySqlConnectionString;
            set => _mySqlConnectionString = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static bool EnableFileServerLogging
        {
            get => _enableFileServerLogging;
            set => _enableFileServerLogging = value;
        }

        public static bool EnableMySqlLogging
        {
            get => _enableMySqlLogging;
            set => _enableMySqlLogging = value;
        }

        public static bool EnableConsoleLogging
        {
            get => _enableConsoleLogging;
            set => _enableConsoleLogging = value;
        }

        public static string FallbackLocalPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MTM_WIP_Application", "Logs");

        public static void LoadFromConfiguration()
        {
            try
            {
                Console.WriteLine("Loading error handling configuration...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load error handling configuration: {ex.Message}");
            }
        }

        public static bool ValidateConfiguration()
        {
            try
            {
                if (EnableFileServerLogging)
                {
                    if (string.IsNullOrWhiteSpace(FileServerBasePath))
                        return false;
                }

                if (EnableMySqlLogging)
                {
                    if (string.IsNullOrWhiteSpace(MySqlConnectionString))
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Configuration validation failed: {ex.Message}");
                return false;
            }
        }

        public static void ResetToDefaults()
        {
            _fileServerBasePath = @"\\FileServer\Logs";
            _mySqlConnectionString = "";
            _enableFileServerLogging = true;
            _enableMySqlLogging = true;
            _enableConsoleLogging = false;
        }

        public static string GetConfigurationSummary()
        {
            return $"Error Handling Configuration:\n" +
                   $"  File Server Logging: {EnableFileServerLogging} (Path: {FileServerBasePath})\n" +
                   $"  MySQL Logging: {EnableMySqlLogging}\n" +
                   $"  Console Logging: {EnableConsoleLogging}\n" +
                   $"  Fallback Path: {FallbackLocalPath}";
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

        public static string GetUserFriendlyMessage(ErrorCategory category)
        {
            return _categoryMessages.TryGetValue(category, out var message)
                ? message
                : "An unexpected error occurred. Please try again or contact support.";
        }

        public static string GetCompleteUserMessage(ErrorCategory category, string exceptionType, ErrorSeverity severity)
        {
            var baseMessage = GetUserFriendlyMessage(category);
            var actions = _categoryActions.TryGetValue(category, out var action) ? action : "Try restarting the application.";
            var severityIndicator = GetSeverityIndicator(severity);

            return $"{severityIndicator}{baseMessage}\n\nWhat you can do: {actions}";
        }

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

        public static bool ShouldShowToUser(ErrorSeverity severity, ErrorCategory category)
        {
            return severity switch
            {
                ErrorSeverity.Low => false,
                ErrorSeverity.Medium => category != ErrorCategory.Other,
                ErrorSeverity.High => true,
                ErrorSeverity.Critical => true,
                _ => true
            };
        }

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
}