using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.Core.ErrorHandling;

/// <summary>
/// Universal error handling service interface for cross-platform error management.
/// </summary>
public interface IUniversalErrorHandlingService
{
    /// <summary>
    /// Handles an exception with operation context and user information.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="operation">Operation context where error occurred</param>
    /// <param name="userId">User identifier (optional)</param>
    /// <param name="context">Additional context information</param>
    /// <param name="callerMemberName">Caller member name (auto-populated)</param>
    /// <param name="callerFilePath">Caller file path (auto-populated)</param>
    /// <param name="callerLineNumber">Caller line number (auto-populated)</param>
    /// <returns>Task representing the async operation</returns>
    Task HandleErrorAsync(
        Exception exception,
        string operation,
        string? userId = null,
        Dictionary<string, object>? context = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0);

    /// <summary>
    /// Gets user-friendly error message for an exception.
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns>User-friendly error message</returns>
    string GetUserFriendlyMessage(Exception exception);

    /// <summary>
    /// Clears the session error cache.
    /// </summary>
    void ClearSessionCache();
}

/// <summary>
/// Universal error handling service implementation.
/// Provides structured error logging and user-friendly error messages for any business domain.
/// </summary>
public class UniversalErrorHandlingService : IUniversalErrorHandlingService
{
    private readonly ILogger<UniversalErrorHandlingService> _logger;
    private readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
    private readonly object _lockObject = new();

    /// <summary>
    /// Initializes a new instance of the UniversalErrorHandlingService.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public UniversalErrorHandlingService(ILogger<UniversalErrorHandlingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task HandleErrorAsync(
        Exception exception,
        string operation,
        string? userId = null,
        Dictionary<string, object>? context = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        try
        {
            var errorCategory = DetermineErrorCategory(exception);
            var severity = DetermineSeverity(exception, errorCategory);
            var errorKey = GenerateErrorKey(exception, callerMemberName, callerFilePath, callerLineNumber);
            var effectiveUserId = userId ?? Environment.UserName;

            if (IsNewError(errorCategory, errorKey))
            {
                await LogErrorAsync(exception, operation, effectiveUserId, context ?? new Dictionary<string, object>());
                AddToSessionCache(errorCategory, errorKey);
            }
        }
        catch (Exception handlerException)
        {
            // Fallback to basic console logging if all else fails
            await LogToConsoleAsync(handlerException, operation, userId, exception);
        }
    }

    /// <inheritdoc />
    public string GetUserFriendlyMessage(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception switch
        {
            UnauthorizedAccessException => "You don't have permission to perform this operation.",
            TimeoutException => "The operation took too long to complete. Please try again.",
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            DirectoryNotFoundException => "The specified directory could not be found.",
            FileNotFoundException => "The specified file could not be found.",
            InvalidOperationException => "The operation cannot be completed at this time. Please try again.",
            NotSupportedException => "This operation is not supported on the current platform.",
            _ when exception.GetType().Name.Contains("Network") => "Network connection error. Please check your internet connection.",
            _ when exception.GetType().Name.Contains("Database") => "Database operation failed. Please try again or contact support.",
            _ when exception.GetType().Name.Contains("Http") => "Service communication error. Please try again later.",
            _ => "An unexpected error occurred. Please contact support if the problem persists."
        };
    }

    /// <inheritdoc />
    public void ClearSessionCache()
    {
        lock (_lockObject)
        {
            _sessionErrorCache.Clear();
        }
    }

    private async Task LogErrorAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        var errorEntry = CreateErrorEntry(exception, operation, userId, context);
        
        // Log using structured logging
        _logger.LogError(exception, 
            "Error in {Operation} for user {UserId}: {ErrorMessage} | Category: {Category} | Severity: {Severity} | Context: {@Context}",
            operation, userId, exception.Message, errorEntry.Category, errorEntry.Severity, context);

        await Task.CompletedTask;
    }

    private async Task LogToConsoleAsync(Exception handlerException, string operation, string? userId, Exception originalException)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var message = $"[{timestamp}] CRITICAL ERROR HANDLER FAILURE: {handlerException.Message} | Original Error: {originalException.Message} | Operation: {operation} | User: {userId ?? "Unknown"}";
        
        Console.WriteLine(message);
        await Task.CompletedTask;
    }

    private UniversalErrorEntry CreateErrorEntry(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        return new UniversalErrorEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            MachineName = Environment.MachineName,
            Category = DetermineErrorCategory(exception),
            Severity = DetermineSeverity(exception, DetermineErrorCategory(exception)),
            ErrorMessage = exception.Message,
            MethodName = operation,
            StackTrace = exception.StackTrace ?? "",
            AdditionalData = JsonSerializer.Serialize(context),
            ExceptionType = exception.GetType().FullName ?? "",
            BusinessContext = ExtractBusinessContext(context)
        };
    }

    private UniversalErrorCategory DetermineErrorCategory(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => UniversalErrorCategory.Security,
            TimeoutException => UniversalErrorCategory.Network,
            ArgumentException => UniversalErrorCategory.BusinessLogic,
            InvalidOperationException => UniversalErrorCategory.BusinessLogic,
            DirectoryNotFoundException => UniversalErrorCategory.FileSystem,
            FileNotFoundException => UniversalErrorCategory.FileSystem,
            NotSupportedException => UniversalErrorCategory.Platform,
            _ when exception.GetType().Name.Contains("Network") => UniversalErrorCategory.Network,
            _ when exception.GetType().Name.Contains("Database") => UniversalErrorCategory.Database,
            _ when exception.GetType().Name.Contains("Http") => UniversalErrorCategory.Network,
            _ when exception.GetType().Name.Contains("UI") => UniversalErrorCategory.UI,
            _ => UniversalErrorCategory.Other
        };
    }

    private UniversalErrorSeverity DetermineSeverity(Exception exception, UniversalErrorCategory category)
    {
        return exception switch
        {
            OutOfMemoryException => UniversalErrorSeverity.Critical,
            StackOverflowException => UniversalErrorSeverity.Critical,
            UnauthorizedAccessException => UniversalErrorSeverity.High,
            TimeoutException when category == UniversalErrorCategory.Database => UniversalErrorSeverity.High,
            InvalidOperationException when category == UniversalErrorCategory.BusinessLogic => UniversalErrorSeverity.High,
            ArgumentException => UniversalErrorSeverity.Medium,
            _ when category == UniversalErrorCategory.UI => UniversalErrorSeverity.Medium,
            _ => UniversalErrorSeverity.Low
        };
    }

    private string GenerateErrorKey(Exception exception, string callerMemberName, string callerFilePath, int callerLineNumber)
    {
        var fileName = Path.GetFileName(callerFilePath);
        return $"{exception.GetType().Name}_{fileName}_{callerMemberName}_{callerLineNumber}";
    }

    private bool IsNewError(UniversalErrorCategory category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category.ToString(), out var categoryErrors))
            {
                return true;
            }

            return !categoryErrors.Contains(errorKey);
        }
    }

    private void AddToSessionCache(UniversalErrorCategory category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category.ToString(), out var categoryErrors))
            {
                categoryErrors = new HashSet<string>();
                _sessionErrorCache[category.ToString()] = categoryErrors;
            }

            categoryErrors.Add(errorKey);
        }
    }

    private string ExtractBusinessContext(Dictionary<string, object> context)
    {
        var businessKeys = new[] { "PartId", "Operation", "Quantity", "Location", "TransactionType", "EntityId", "EntityType" };
        var businessContext = new Dictionary<string, object>();

        foreach (var key in businessKeys)
        {
            if (context.TryGetValue(key, out var value))
            {
                businessContext[key] = value;
            }
        }

        return businessContext.Count > 0 ? JsonSerializer.Serialize(businessContext) : "";
    }
}

/// <summary>
/// Universal error entry for structured error logging.
/// </summary>
public class UniversalErrorEntry
{
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = "";
    public string MachineName { get; set; } = "";
    public UniversalErrorCategory Category { get; set; }
    public UniversalErrorSeverity Severity { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string MethodName { get; set; } = "";
    public string StackTrace { get; set; } = "";
    public string AdditionalData { get; set; } = "";
    public string ExceptionType { get; set; } = "";
    public string BusinessContext { get; set; } = "";
}

/// <summary>
/// Universal error categories for structured error classification.
/// </summary>
public enum UniversalErrorCategory
{
    BusinessLogic,
    Database,
    Network,
    UI,
    FileSystem,
    Security,
    Platform,
    Configuration,
    Other
}

/// <summary>
/// Universal error severity levels.
/// </summary>
public enum UniversalErrorSeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Static helper class for global error handling (maintains compatibility with original MTM patterns).
/// </summary>
public static class UniversalErrorHandling
{
    private static IUniversalErrorHandlingService? _errorHandlingService;

    /// <summary>
    /// Initializes the static error handler with a service instance.
    /// </summary>
    /// <param name="errorHandlingService">Error handling service instance</param>
    public static void Initialize(IUniversalErrorHandlingService errorHandlingService)
    {
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
    }

    /// <summary>
    /// Handles an error using the static service instance.
    /// </summary>
    /// <param name="exception">The exception to handle</param>
    /// <param name="operation">Operation context</param>
    /// <param name="userId">User identifier (optional)</param>
    /// <param name="context">Additional context (optional)</param>
    /// <returns>Task representing the async operation</returns>
    public static async Task HandleErrorAsync(
        Exception exception,
        string operation,
        string? userId = null,
        Dictionary<string, object>? context = null)
    {
        if (_errorHandlingService != null)
        {
            await _errorHandlingService.HandleErrorAsync(exception, operation, userId, context);
        }
        else
        {
            // Fallback when service is not initialized
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {operation}: {exception.Message}");
        }
    }
}