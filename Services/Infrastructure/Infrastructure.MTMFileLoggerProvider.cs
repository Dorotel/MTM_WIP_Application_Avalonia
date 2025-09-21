using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Core;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services.Infrastructure;

#region MTM File Logger Provider Service

/// <summary>
/// Interface for MTM file logger provider service
/// </summary>
public interface IMTMFileLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// Gets the file logging service instance
    /// </summary>
    IFileLoggingService FileLoggingService { get; }
}

/// <summary>
/// Custom logger provider that routes logs to IFileLoggingService based on content and source.
/// Enhanced with MySQL operation detection and business category classification.
/// </summary>
public class MTMFileLoggerProvider : IMTMFileLoggerProvider
{
    private readonly IFileLoggingService _fileLoggingService;
    private readonly ConcurrentDictionary<string, MTMFileLogger> _loggers = new();
    private bool _disposed = false;

    public MTMFileLoggerProvider(IFileLoggingService fileLoggingService)
    {
        _fileLoggingService = fileLoggingService ?? throw new ArgumentNullException(nameof(fileLoggingService));
    }

    public IFileLoggingService FileLoggingService => _fileLoggingService;

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new MTMFileLogger(name, _fileLoggingService));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _loggers.Clear();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Custom logger implementation that categorizes and routes log messages.
/// Routes MySQL-related logs and business logs to the file logging service with enhanced categorization.
/// </summary>
internal class MTMFileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IFileLoggingService _fileLoggingService;

    public MTMFileLogger(string categoryName, IFileLoggingService fileLoggingService)
    {
        _categoryName = categoryName;
        _fileLoggingService = fileLoggingService;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= LogLevel.Information;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message)) return;

        // Enhanced message with category and operation context
        var enhancedMessage = FormatLogMessage(message, _categoryName, eventId);

        // Determine if this is a MySQL-related log and route accordingly
        if (IsMySqlRelated(message, exception, _categoryName))
        {
            var operation = GetMySqlOperation(message);
            var formattedMessage = $"[MySQL-{operation}] {enhancedMessage}";
            _ = _fileLoggingService.WriteLogAsync(logLevel, formattedMessage, exception);
        }
        else
        {
            // Route to business log with category
            var category = DetermineBusinessCategory(_categoryName, message);
            var formattedMessage = $"[{category}] {enhancedMessage}";
            _ = _fileLoggingService.WriteLogAsync(logLevel, formattedMessage, exception);
        }
    }

    /// <summary>
    /// Formats log message with additional context information.
    /// </summary>
    private string FormatLogMessage(string message, string categoryName, EventId eventId)
    {
        var categoryShort = GetShortCategoryName(categoryName);
        return eventId.Id != 0
            ? $"[{categoryShort}] [{eventId.Id}] {message}"
            : $"[{categoryShort}] {message}";
    }

    /// <summary>
    /// Gets short category name for cleaner log formatting.
    /// </summary>
    private string GetShortCategoryName(string fullCategoryName)
    {
        if (string.IsNullOrEmpty(fullCategoryName)) return "Unknown";

        // Extract last part of namespace
        var parts = fullCategoryName.Split('.');
        var lastPart = parts[^1];

        // Remove common suffixes for cleaner display
        return lastPart
            .Replace("ViewModel", "VM")
            .Replace("Service", "Svc")
            .Replace("Controller", "Ctrl");
    }

    /// <summary>
    /// Determines if a log entry is MySQL/database related.
    /// </summary>
    private static bool IsMySqlRelated(string message, Exception? exception, string categoryName)
    {
        // Check exception type
        if (exception is MySqlException ||
            (exception is InvalidOperationException && exception.Message.Contains("MySQL")))
            return true;

        // Check category name
        if (categoryName.Contains("Database", StringComparison.OrdinalIgnoreCase) ||
            categoryName.Contains("MySQL", StringComparison.OrdinalIgnoreCase) ||
            categoryName.Contains("Helper_Database", StringComparison.OrdinalIgnoreCase))
            return true;

        // Check message content
        var mysqlKeywords = new[]
        {
            "mysql", "database", "connection", "stored procedure", "executedata",
            "mysqldatareader", "mysqlcommand", "mysqlconnection", "sql error",
            "table", "column", "constraint", "foreign key", "primary key",
            "transaction", "rollback", "commit", "deadlock"
        };

        var messageLower = message.ToLowerInvariant();
        return mysqlKeywords.Any(keyword => messageLower.Contains(keyword));
    }

    /// <summary>
    /// Extracts MySQL operation from log message.
    /// </summary>
    private static string GetMySqlOperation(string message)
    {
        // Extract stored procedure names
        if (message.Contains("stored procedure:", StringComparison.OrdinalIgnoreCase))
        {
            var start = message.IndexOf("stored procedure:", StringComparison.OrdinalIgnoreCase) + 17;
            var end = message.IndexOf(' ', start);
            if (end == -1) end = message.Length;
            return message.Substring(start, end - start).Trim();
        }

        // Extract operation from common patterns
        if (message.Contains("Executing", StringComparison.OrdinalIgnoreCase))
            return "Execute";
        if (message.Contains("Connection", StringComparison.OrdinalIgnoreCase))
            return "Connection";
        if (message.Contains("Transaction", StringComparison.OrdinalIgnoreCase))
            return "Transaction";
        if (message.Contains("Query", StringComparison.OrdinalIgnoreCase))
            return "Query";

        return "General";
    }

    /// <summary>
    /// Determines the business category for non-MySQL logs.
    /// </summary>
    private static string DetermineBusinessCategory(string categoryName, string message)
    {
        // Extract from category name
        if (categoryName.Contains("ViewModel", StringComparison.OrdinalIgnoreCase))
            return "ViewModel";
        if (categoryName.Contains("Service", StringComparison.OrdinalIgnoreCase))
            return "Service";
        if (categoryName.Contains("View", StringComparison.OrdinalIgnoreCase))
            return "View";
        if (categoryName.Contains("Startup", StringComparison.OrdinalIgnoreCase))
            return "Startup";
        if (categoryName.Contains("Theme", StringComparison.OrdinalIgnoreCase))
            return "Theme";
        if (categoryName.Contains("Navigation", StringComparison.OrdinalIgnoreCase))
            return "Navigation";
        if (categoryName.Contains("Configuration", StringComparison.OrdinalIgnoreCase))
            return "Configuration";

        // Extract from message content
        if (message.Contains("user", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("login", StringComparison.OrdinalIgnoreCase))
            return "User";
        if (message.Contains("inventory", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("part", StringComparison.OrdinalIgnoreCase))
            return "Inventory";
        if (message.Contains("transaction", StringComparison.OrdinalIgnoreCase))
            return "Transaction";
        if (message.Contains("export", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("import", StringComparison.OrdinalIgnoreCase))
            return "ImportExport";
        if (message.Contains("validation", StringComparison.OrdinalIgnoreCase))
            return "Validation";

        return "General";
    }
}

#endregion
