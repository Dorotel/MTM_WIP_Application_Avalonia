using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using System.Linq;

namespace MTM.Core.Services
{
    /// <summary>
    /// Central error handling service that provides comprehensive error logging and management
    /// across any .NET application. Framework-agnostic implementation suitable for web, desktop, or console applications.
    /// </summary>
    public static class Service_ErrorHandler
    {
        private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
        private static readonly object _lockObject = new();

        /// <summary>
        /// Handles an exception with specified severity and additional context data.
        /// Logs to configured destinations while preventing duplicate entries for the current session.
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="severity">The severity level of the error</param>
        /// <param name="source">The source component/service where the error occurred (optional)</param>
        /// <param name="additionalData">Additional context data for debugging (optional)</param>
        /// <param name="callerMemberName">Auto-populated caller method name</param>
        /// <param name="callerFilePath">Auto-populated caller file path</param>
        /// <param name="callerLineNumber">Auto-populated caller line number</param>
        public static void HandleException(
            Exception exception,
            ErrorSeverity severity,
            string? source = null,
            Dictionary<string, object>? additionalData = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            try
            {
                var errorCategory = DetermineErrorCategory(exception);
                var errorKey = GenerateErrorKey(exception, callerMemberName, callerFilePath, callerLineNumber);
                
                if (IsNewError(errorCategory, errorKey))
                {
                    var errorEntry = CreateErrorEntry(exception, severity, source, additionalData, 
                        callerMemberName, callerFilePath, callerLineNumber, errorCategory);
                    
                    // Log to configured destinations
                    LoggingUtility.LogToFileServer(errorEntry);
                    LoggingUtility.LogToDatabase(errorEntry);
                    
                    // Update session cache
                    AddToSessionCache(errorCategory, errorKey);
                }
            }
            catch (Exception handlerException)
            {
                // Fallback logging when error handler fails
                LoggingUtility.LogApplicationError(handlerException);
            }
        }

        /// <summary>
        /// Determines the error category based on the exception type and context.
        /// </summary>
        private static ErrorCategory DetermineErrorCategory(Exception exception)
        {
            return exception switch
            {
                _ when exception.GetType().FullName?.Contains("MySql") == true => ErrorCategory.Database,
                _ when exception.GetType().FullName?.Contains("Sql") == true => ErrorCategory.Database,
                _ when exception.GetType().FullName?.Contains("Network") == true => ErrorCategory.Network,
                _ when exception.GetType().FullName?.Contains("Http") == true => ErrorCategory.Network,
                ArgumentException => ErrorCategory.BusinessLogic,
                InvalidOperationException => ErrorCategory.BusinessLogic,
                UnauthorizedAccessException => ErrorCategory.Security,
                TimeoutException => ErrorCategory.Network,
                _ => ErrorCategory.Other
            };
        }

        /// <summary>
        /// Generates a unique key for error identification to prevent duplicate logging.
        /// </summary>
        private static string GenerateErrorKey(Exception exception, string memberName, string filePath, int lineNumber)
        {
            var fileName = Path.GetFileName(filePath);
            var exceptionTypeHash = exception.GetType().Name.GetHashCode();
            var messageHash = (exception.Message ?? "").GetHashCode();
            
            return $"{exceptionTypeHash}_{messageHash}_{memberName}_{fileName}_{lineNumber}";
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
            string? source,
            Dictionary<string, object>? additionalData,
            string callerMemberName,
            string callerFilePath,
            int callerLineNumber,
            ErrorCategory category)
        {
            return new ErrorEntry
            {
                Timestamp = DateTime.UtcNow,
                UserId = GetCurrentUserId(),
                MachineName = Environment.MachineName,
                ApplicationName = GetApplicationName(),
                Category = category,
                Severity = severity,
                ErrorMessage = exception.Message,
                FileName = Path.GetFileName(callerFilePath),
                MethodName = callerMemberName,
                LineNumber = callerLineNumber,
                StackTrace = exception.StackTrace ?? "",
                Source = source,
                AdditionalData = FormatAdditionalData(additionalData),
                ExceptionType = exception.GetType().FullName ?? "",
                InnerException = exception.InnerException?.ToString()
            };
        }

        /// <summary>
        /// Gets the current user ID from the application context.
        /// Override this method to integrate with your authentication system.
        /// </summary>
        private static string GetCurrentUserId()
        {
            // TODO: Implement user ID retrieval from your authentication system
            // Examples:
            // - ClaimsPrincipal.Current?.Identity?.Name
            // - HttpContext.Current?.User?.Identity?.Name
            // - Your custom authentication service
            return Environment.UserName ?? "SYSTEM";
        }

        /// <summary>
        /// Gets the current application name.
        /// </summary>
        private static string GetApplicationName()
        {
            return System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
        }

        /// <summary>
        /// Formats additional data into a readable string for logging.
        /// </summary>
        private static string FormatAdditionalData(Dictionary<string, object>? additionalData)
        {
            if (additionalData == null || additionalData.Count == 0)
                return "";
            
            var formatted = new List<string>();
            foreach (var kvp in additionalData)
            {
                var value = kvp.Value?.ToString() ?? "null";
                formatted.Add($"{kvp.Key}={value}");
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

        /// <summary>
        /// Gets total error count across all categories for the current session.
        /// </summary>
        public static int GetTotalSessionErrorCount()
        {
            lock (_lockObject)
            {
                return _sessionErrorCache.Values.Sum(set => set.Count);
            }
        }

        /// <summary>
        /// Gets error statistics for the current session.
        /// </summary>
        public static Dictionary<ErrorCategory, int> GetSessionErrorStatistics()
        {
            lock (_lockObject)
            {
                var stats = new Dictionary<ErrorCategory, int>();
                foreach (ErrorCategory category in Enum.GetValues<ErrorCategory>())
                {
                    stats[category] = GetSessionErrorCount(category);
                }
                return stats;
            }
        }
    }

    /// <summary>
    /// Represents a comprehensive error entry for logging purposes.
    /// </summary>
    public class ErrorEntry
    {
        /// <summary>
        /// Gets or sets the timestamp when the error occurred (UTC).
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
        /// Gets or sets the name of the application where the error occurred.
        /// </summary>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the category of the error.
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
        /// Gets or sets the source component/service where the error occurred.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets additional data relevant to debugging the error.
        /// </summary>
        public string AdditionalData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full type name of the exception that occurred.
        /// </summary>
        public string ExceptionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the inner exception information if available.
        /// </summary>
        public string? InnerException { get; set; }

        /// <summary>
        /// Returns a string representation of the error entry for debugging purposes.
        /// </summary>
        public override string ToString()
        {
            return $"{Timestamp:yyyy-MM-dd HH:mm:ss} UTC [{Category}] {Severity}: {ErrorMessage} in {FileName}:{MethodName}:{LineNumber}";
        }
    }

    /// <summary>
    /// Defines the categories of errors that can occur in the application.
    /// </summary>
    public enum ErrorCategory
    {
        /// <summary>
        /// Business logic and application workflow errors.
        /// Examples: Validation failures, calculation errors, rule violations.
        /// </summary>
        BusinessLogic,

        /// <summary>
        /// Database related errors.
        /// Examples: Connection failures, query timeouts, constraint violations.
        /// </summary>
        Database,

        /// <summary>
        /// Network communication errors.
        /// Examples: HTTP request failures, timeout errors, connectivity issues.
        /// </summary>
        Network,

        /// <summary>
        /// Security related errors.
        /// Examples: Authentication failures, authorization denials, security violations.
        /// </summary>
        Security,

        /// <summary>
        /// User interface related errors (if applicable).
        /// Examples: Control initialization failures, binding errors, rendering issues.
        /// </summary>
        UI,

        /// <summary>
        /// All other errors that don't fit into the specific categories above.
        /// Examples: File system errors, configuration issues, third-party library errors.
        /// </summary>
        Other
    }

    /// <summary>
    /// Defines the severity levels for errors.
    /// </summary>
    public enum ErrorSeverity
    {
        /// <summary>
        /// Low severity errors that don't significantly impact functionality.
        /// Examples: Minor validation warnings, non-critical operation failures.
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