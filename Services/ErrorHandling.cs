using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Comprehensive error handling and logging service for MTM WIP Application.
/// Provides structured error logging, file-based fallback, and user-friendly error messages.
/// </summary>
public static class ErrorHandling
{
    private static readonly Dictionary<string, HashSet<string>> _sessionErrorCache = new();
    private static readonly object _lockObject = new();
    private static string _fileServerBasePath = @"\\FileServer\Logs";

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
                ["OriginalError"] = exception.Message,
                ["HandlerFailure"] = true
            });
        }
    }

    /// <summary>
    /// Gets a user-friendly error message while hiding technical details.
    /// </summary>
    public static string GetUserFriendlyMessage(Exception ex)
    {
        var category = DetermineErrorCategory(ex);
        return category switch
        {
            ErrorCategory.UI => "A display issue occurred. The application will continue to work.",
            ErrorCategory.BusinessLogic => "A processing error occurred. Please check your input and try again.",
            ErrorCategory.MySQL => "A database connection issue occurred. Please try again in a moment.",
            ErrorCategory.Network => "A network connection issue occurred. Please check your connection.",
            _ => "An unexpected error occurred. Please try again or contact support."
        };
    }

    /// <summary>
    /// Logs an error with structured data and fallback mechanisms.
    /// </summary>
    public static async Task LogErrorAsync(Exception ex, string operation, string userId, Dictionary<string, object> context)
    {
        try
        {
            var errorEntry = CreateErrorEntry(ex, operation, userId, context);
            
            // Try MySQL logging first
            if (ErrorConfiguration.EnableMySqlLogging)
            {
                await LogToMySQL(errorEntry);
            }
        }
        catch
        {
            // Continue to file fallback
        }

        // Always log to file as fallback
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
            LogToFileServer(errorEntry);
        }
        catch (Exception fileEx)
        {
            Console.WriteLine($"Critical: All logging failed. Error: {ex.Message}, Logging Error: {fileEx.Message}");
        }
    }

    /// <summary>
    /// Logs to file server with CSV format.
    /// </summary>
    public static void LogToFileServer(ErrorEntry errorEntry)
    {
        try
        {
            if (!ErrorConfiguration.EnableFileServerLogging) return;

            var userLogFolder = Path.Combine(_fileServerBasePath, errorEntry.UserId);
            var csvFileName = GetCsvFileName(errorEntry.Category);
            var csvFilePath = Path.Combine(userLogFolder, csvFileName);
            
            Directory.CreateDirectory(userLogFolder);
            
            lock (_lockObject)
            {
                var fileExists = File.Exists(csvFilePath);
                using var writer = new StreamWriter(csvFilePath, append: true);
                
                if (!fileExists)
                {
                    writer.WriteLine(GetCsvHeader());
                }
                
                writer.WriteLine(FormatErrorEntryAsCsv(errorEntry));
            }
        }
        catch (Exception ex)
        {
            LogToFallbackLocation(errorEntry, ex);
        }
    }

    /// <summary>
    /// MySQL logging with business context support.
    /// </summary>
    public static async Task LogToMySQL(ErrorEntry errorEntry)
    {
        try
        {
            if (!ErrorConfiguration.EnableMySqlLogging || 
                string.IsNullOrWhiteSpace(ErrorConfiguration.MySqlConnectionString))
                return;

            var tableName = GetMySqlTableName(errorEntry.Category);
            
            using var connection = new MySqlConnection(ErrorConfiguration.MySqlConnectionString);
            await connection.OpenAsync();
            
            await EnsureTableExists(connection, tableName);
            
            var insertSql = $@"
                INSERT INTO {tableName} 
                (Timestamp, UserId, MachineName, Category, Severity, ErrorMessage, 
                 FileName, MethodName, LineNumber, StackTrace, AdditionalData, ExceptionType, BusinessContext)
                VALUES 
                (@Timestamp, @UserId, @MachineName, @Category, @Severity, @ErrorMessage,
                 @FileName, @MethodName, @LineNumber, @StackTrace, @AdditionalData, @ExceptionType, @BusinessContext)";

            using var command = new MySqlCommand(insertSql, connection);
            AddMySqlParameters(command, errorEntry);
            
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MySQL logging failed: {ex.Message}");
        }
    }

    private static ErrorCategory DetermineErrorCategory(Exception exception)
    {
        return exception switch
        {
            _ when exception.GetType().FullName?.Contains("MySql") == true => ErrorCategory.MySQL,
            _ when exception.GetType().FullName?.Contains("Network") == true => ErrorCategory.Network,
            _ when exception.GetType().FullName?.Contains("UI") == true => ErrorCategory.UI,
            _ when exception.GetType().FullName?.Contains("Avalonia") == true => ErrorCategory.UI,
            ArgumentException => ErrorCategory.BusinessLogic,
            InvalidOperationException => ErrorCategory.BusinessLogic,
            TimeoutException => ErrorCategory.Network,
            _ => ErrorCategory.Other
        };
    }

    private static ErrorSeverity DetermineSeverity(Exception exception, ErrorCategory category)
    {
        return exception switch
        {
            OutOfMemoryException => ErrorSeverity.Critical,
            StackOverflowException => ErrorSeverity.Critical,
            UnauthorizedAccessException => ErrorSeverity.High,
            TimeoutException when category == ErrorCategory.MySQL => ErrorSeverity.High,
            InvalidOperationException when category == ErrorCategory.BusinessLogic => ErrorSeverity.High,
            ArgumentException => ErrorSeverity.Medium,
            _ when category == ErrorCategory.UI => ErrorSeverity.Medium,
            _ => ErrorSeverity.Low
        };
    }

    private static ErrorEntry CreateErrorEntry(Exception exception, string operation, string userId, Dictionary<string, object> context)
    {
        return new ErrorEntry
        {
            Timestamp = DateTime.UtcNow,
            UserId = userId,
            MachineName = Environment.MachineName,
            Category = DetermineErrorCategory(exception),
            Severity = DetermineSeverity(exception, DetermineErrorCategory(exception)),
            ErrorMessage = exception.Message,
            FileName = "",
            MethodName = operation,
            LineNumber = 0,
            StackTrace = exception.StackTrace ?? "",
            AdditionalData = JsonSerializer.Serialize(context),
            ExceptionType = exception.GetType().FullName ?? "",
            BusinessContext = ExtractBusinessContext(context)
        };
    }

    private static string ExtractBusinessContext(Dictionary<string, object> context)
    {
        var businessItems = new List<string>();

        if (context.TryGetValue("PartId", out var partId))
            businessItems.Add($"PartId={partId}");
        if (context.TryGetValue("Operation", out var operation))
            businessItems.Add($"Operation={operation}");
        if (context.TryGetValue("Quantity", out var quantity))
            businessItems.Add($"Quantity={quantity}");
        if (context.TryGetValue("Location", out var location))
            businessItems.Add($"Location={location}");

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
                _sessionErrorCache[categoryKey] = new HashSet<string>();
            
            return !_sessionErrorCache[categoryKey].Contains(errorKey);
        }
    }

    private static void AddToSessionCache(ErrorCategory category, string errorKey)
    {
        lock (_lockObject)
        {
            var categoryKey = category.ToString();
            if (!_sessionErrorCache.ContainsKey(categoryKey))
                _sessionErrorCache[categoryKey] = new HashSet<string>();
            
            _sessionErrorCache[categoryKey].Add(errorKey);
        }
    }

    private static async Task EnsureTableExists(MySqlConnection connection, string tableName)
    {
        var createTableSql = $@"
            CREATE TABLE IF NOT EXISTS {tableName} (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Timestamp DATETIME NOT NULL,
                UserId VARCHAR(255) NOT NULL,
                MachineName VARCHAR(255) NOT NULL,
                Category VARCHAR(50) NOT NULL,
                Severity VARCHAR(20) NOT NULL,
                ErrorMessage TEXT,
                FileName VARCHAR(500),
                MethodName VARCHAR(255),
                LineNumber INT,
                StackTrace TEXT,
                AdditionalData TEXT,
                ExceptionType VARCHAR(500),
                BusinessContext TEXT,
                INDEX idx_timestamp (Timestamp),
                INDEX idx_userid (UserId)
            )";

        using var command = new MySqlCommand(createTableSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static void AddMySqlParameters(MySqlCommand command, ErrorEntry errorEntry)
    {
        command.Parameters.AddWithValue("@Timestamp", errorEntry.Timestamp);
        command.Parameters.AddWithValue("@UserId", errorEntry.UserId);
        command.Parameters.AddWithValue("@MachineName", errorEntry.MachineName);
        command.Parameters.AddWithValue("@Category", errorEntry.Category.ToString());
        command.Parameters.AddWithValue("@Severity", errorEntry.Severity.ToString());
        command.Parameters.AddWithValue("@ErrorMessage", errorEntry.ErrorMessage);
        command.Parameters.AddWithValue("@FileName", errorEntry.FileName);
        command.Parameters.AddWithValue("@MethodName", errorEntry.MethodName);
        command.Parameters.AddWithValue("@LineNumber", errorEntry.LineNumber);
        command.Parameters.AddWithValue("@StackTrace", errorEntry.StackTrace);
        command.Parameters.AddWithValue("@AdditionalData", errorEntry.AdditionalData);
        command.Parameters.AddWithValue("@ExceptionType", errorEntry.ExceptionType);
        command.Parameters.AddWithValue("@BusinessContext", errorEntry.BusinessContext);
    }

    private static string GetCsvHeader()
    {
        return "Timestamp,UserId,MachineName,Category,Severity,ErrorMessage,FileName,MethodName,LineNumber,StackTrace,AdditionalData,ExceptionType,BusinessContext";
    }

    private static string FormatErrorEntryAsCsv(ErrorEntry errorEntry)
    {
        return string.Join(",",
            EscapeCsvField(errorEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")),
            EscapeCsvField(errorEntry.UserId),
            EscapeCsvField(errorEntry.MachineName),
            EscapeCsvField(errorEntry.Category.ToString()),
            EscapeCsvField(errorEntry.Severity.ToString()),
            EscapeCsvField(errorEntry.ErrorMessage),
            EscapeCsvField(errorEntry.FileName),
            EscapeCsvField(errorEntry.MethodName),
            EscapeCsvField(errorEntry.LineNumber.ToString()),
            EscapeCsvField(errorEntry.StackTrace),
            EscapeCsvField(errorEntry.AdditionalData),
            EscapeCsvField(errorEntry.ExceptionType),
            EscapeCsvField(errorEntry.BusinessContext)
        );
    }

    private static string GetCsvFileName(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.UI => "ui_errors.csv",
            ErrorCategory.BusinessLogic => "business_logic_errors.csv",
            ErrorCategory.MySQL => "mysql_errors.csv",
            ErrorCategory.Network => "network_errors.csv",
            _ => "other_errors.csv"
        };
    }

    private static string GetMySqlTableName(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.UI => "ui_errors",
            ErrorCategory.BusinessLogic => "business_logic_errors",
            ErrorCategory.MySQL => "mysql_errors",
            ErrorCategory.Network => "network_errors",
            _ => "other_errors"
        };
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field)) return "\"\"";
        
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }
        
        return field;
    }

    private static void LogToFallbackLocation(ErrorEntry errorEntry, Exception originalException)
    {
        try
        {
            var fallbackPath = Path.Combine(ErrorConfiguration.FallbackLocalPath, 
                errorEntry.UserId, GetCsvFileName(errorEntry.Category));
            
            Directory.CreateDirectory(Path.GetDirectoryName(fallbackPath)!);
            
            var fileExists = File.Exists(fallbackPath);
            using var writer = new StreamWriter(fallbackPath, append: true);
            
            if (!fileExists)
            {
                writer.WriteLine(GetCsvHeader());
            }
            
            writer.WriteLine(FormatErrorEntryAsCsv(errorEntry));
        }
        catch (Exception fallbackEx)
        {
            Console.WriteLine($"Critical: Both primary and fallback logging failed. Original: {originalException.Message}, Fallback: {fallbackEx.Message}");
        }
    }

    public static void ClearSessionCache()
    {
        lock (_lockObject)
        {
            _sessionErrorCache.Clear();
        }
    }
}

/// <summary>
/// Error entry with MTM business context support.
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
    public string AdditionalData { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public string BusinessContext { get; set; } = string.Empty;
}

/// <summary>
/// Error handling configuration.
/// </summary>
public static class ErrorConfiguration
{
    public static string FileServerBasePath { get; set; } = @"\\FileServer\Logs";
    public static string MySqlConnectionString { get; set; } = "";
    public static bool EnableFileServerLogging { get; set; } = true;
    public static bool EnableMySqlLogging { get; set; } = true;
    public static string FallbackLocalPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTM_WIP_Application", "Logs");
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
