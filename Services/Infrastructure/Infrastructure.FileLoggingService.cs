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

#region File Logging Service

/// <summary>
/// File logging service interface for persistent logging to files
/// </summary>
public interface IFileLoggingService
{
    /// <summary>
    /// Writes log entry to file
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="message">Log message</param>
    /// <param name="exception">Optional exception</param>
    Task WriteLogAsync(LogLevel level, string message, Exception? exception = null);

    /// <summary>
    /// Gets log file path for the current date
    /// </summary>
    /// <returns>Log file path</returns>
    string GetLogFilePath();

    /// <summary>
    /// Cleans up old log files beyond retention period
    /// </summary>
    /// <param name="retentionDays">Number of days to retain logs</param>
    /// <returns>Number of files cleaned up</returns>
    Task<int> CleanupOldLogsAsync(int retentionDays = 30);

    /// <summary>
    /// Gets available log files
    /// </summary>
    /// <returns>List of log file paths</returns>
    Task<List<string>> GetAvailableLogFilesAsync();
}

/// <summary>
/// File logging service implementation
/// </summary>
public class FileLoggingService : IFileLoggingService
{
    private readonly ILogger<FileLoggingService> _logger;
    private readonly IFilePathService _filePathService;
    private readonly string _logDirectory;

    public FileLoggingService(ILogger<FileLoggingService> logger, IFilePathService filePathService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _filePathService = filePathService ?? throw new ArgumentNullException(nameof(filePathService));

        _logDirectory = _filePathService.CombinePath(_filePathService.GetAppDataPath(), "Logs");
        _filePathService.EnsureDirectoryExists(_logDirectory);

        _logger.LogDebug("FileLoggingService constructed with log directory: {LogDirectory}", _logDirectory);
    }

    public async Task WriteLogAsync(LogLevel level, string message, Exception? exception = null)
    {
        try
        {
            var logFilePath = GetLogFilePath();
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var logLine = $"[{timestamp}] [{level}] {message}";

            if (exception != null)
            {
                logLine += $"\nException: {exception}";
            }

            await File.AppendAllTextAsync(logFilePath, logLine + Environment.NewLine);
        }
        catch (Exception ex)
        {
            // Don't use _logger here to avoid potential recursion
            Debug.WriteLine($"Failed to write to log file: {ex.Message}");
        }
    }

    public string GetLogFilePath()
    {
        var fileName = $"MTM_WIP_Application_{DateTime.Now:yyyyMMdd}.log";
        return _filePathService.CombinePath(_logDirectory, fileName);
    }

    public async Task<int> CleanupOldLogsAsync(int retentionDays = 30)
    {
        try
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            var logFiles = Directory.GetFiles(_logDirectory, "*.log");
            var deletedCount = 0;

            foreach (var logFile in logFiles)
            {
                var fileInfo = new FileInfo(logFile);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    await Task.Run(() => File.Delete(logFile));
                    deletedCount++;
                }
            }

            _logger.LogInformation("Cleaned up {DeletedCount} old log files (older than {RetentionDays} days)",
                deletedCount, retentionDays);

            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old log files");
            return 0;
        }
    }

    public async Task<List<string>> GetAvailableLogFilesAsync()
    {
        try
        {
            await Task.Delay(1); // Make it async

            if (!Directory.Exists(_logDirectory))
                return new List<string>();

            var logFiles = Directory.GetFiles(_logDirectory, "*.log")
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .ToList();

            _logger.LogDebug("Found {LogFileCount} log files", logFiles.Count);
            return logFiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available log files");
            return new List<string>();
        }
    }
}

#endregion
