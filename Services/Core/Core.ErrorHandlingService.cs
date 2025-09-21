using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services.Core;

#region Error Handling Services

/// <summary>
/// Comprehensive error handling and logging service for MTM WIP Application.
/// Provides structured error logging, file-based fallback, and user-friendly error messages.
/// </summary>
public static class ErrorHandling
{
    private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
    private static readonly object _lockObject = new();
    private static string _fileServerBasePath = @"\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs";

    /// <summary>
    /// Handles an exception with specified operation context and user information.
    /// </summary>
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
            await LogToFileAsync(handlerException, operation, userId, new Dictionary<string, object>
            {
                ["OriginalException"] = exception.Message,
                ["HandlerError"] = true
            });
        }
    }

    /// <summary>
    /// Handles an exception with default user context.
    /// </summary>
    public static async Task HandleErrorAsync(Exception exception, string operation, Dictionary<string, object>? context = null)
    {
        await HandleErrorAsync(exception, operation, Environment.UserName, context);
    }

    private static string DetermineErrorCategory(Exception exception)
    {
        return exception switch
        {
            MySqlException => "Database",
            TimeoutException => "Timeout",
            ArgumentException => "Validation",
            UnauthorizedAccessException => "Security",
            FileNotFoundException or DirectoryNotFoundException => "FileSystem",
            InvalidOperationException => "Business",
            _ => "General"
        };
    }

    private static string DetermineSeverity(Exception exception, string category)
    {
        return exception switch
        {
            MySqlException mysql when mysql.Number == 1042 => "Critical", // Connection failed
            TimeoutException => "High",
            UnauthorizedAccessException => "High",
            ArgumentNullException => "Medium",
            FileNotFoundException => "Medium",
            _ => "Low"
        };
    }

    private static string GenerateErrorKey(Exception exception, string memberName, string filePath, int lineNumber)
    {
        var fileName = Path.GetFileName(filePath);
        return $"{exception.GetType().Name}_{fileName}_{memberName}_{lineNumber}";
    }

    private static bool IsNewError(string category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category, out var errors))
            {
                errors = new HashSet<string>();
                _sessionErrorCache[category] = errors;
            }

            return !errors.Contains(errorKey);
        }
    }

    private static void AddToSessionCache(string category, string errorKey)
    {
        lock (_lockObject)
        {
            if (!_sessionErrorCache.TryGetValue(category, out var errors))
            {
                errors = new HashSet<string>();
                _sessionErrorCache[category] = errors;
            }

            errors.Add(errorKey);

            // Keep cache size manageable
            if (errors.Count > 100)
            {
                var toRemove = errors.Take(20).ToList();
                foreach (var item in toRemove)
                    errors.Remove(item);
            }
        }
    }

    private static async Task LogErrorAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        try
        {
            // Try database logging first
            // This would use the actual database service when available
            await LogToDatabaseAsync(exception, operation, userId, context);
        }
        catch
        {
            // Fallback to file logging
            await LogToFileAsync(exception, operation, userId, context);
        }
    }

    private static async Task LogToDatabaseAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        // Placeholder for database logging
        // Would use Helper_Database_StoredProcedure when available
        await Task.CompletedTask;
    }

    private static async Task LogToFileAsync(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        try
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                User = userId,
                Operation = operation,
                Exception = exception.GetType().Name,
                Message = exception.Message,
                StackTrace = exception.StackTrace,
                Context = context
            };

            var jsonLog = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"MTM_Error_{DateTime.Now:yyyy-MM-dd}.log";

            // Try network path first, fall back to local
            var logPath = Path.Combine(_fileServerBasePath, fileName);
            try
            {
                await File.AppendAllTextAsync(logPath, jsonLog + Environment.NewLine);
            }
            catch
            {
                // Fallback to local logging
                var localPath = Path.Combine(Path.GetTempPath(), "MTM_Logs", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                await File.AppendAllTextAsync(localPath, jsonLog + Environment.NewLine);
            }
        }
        catch
        {
            // Last resort - do nothing rather than crash
        }
    }

    /// <summary>
    /// Gets a user-friendly error message for the given exception.
    /// </summary>
    public static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            MySqlException mysql => mysql.Number switch
            {
                1042 => "Unable to connect to the database. Please check your network connection and try again.",
                1045 => "Database authentication failed. Please contact your system administrator.",
                1146 => "A required database table was not found. Please contact your system administrator.",
                _ => "A database error occurred. Please try again or contact support."
            },
            TimeoutException => "The operation took too long to complete. Please try again.",
            UnauthorizedAccessException => "You don't have permission to perform this operation.",
            FileNotFoundException => "A required file was not found. Please ensure all files are available and try again.",
            ArgumentException => "Invalid input provided. Please check your data and try again.",
            InvalidOperationException => "The requested operation cannot be completed at this time.",
            _ => "An unexpected error occurred. Please contact support if the problem persists."
        };
    }

    /// <summary>
    /// Clears the session error cache.
    /// </summary>
    public static void ClearSessionCache()
    {
        lock (_lockObject)
        {
            _sessionErrorCache.Clear();
        }
    }
}

#endregion
