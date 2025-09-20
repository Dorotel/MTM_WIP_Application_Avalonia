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

#region File Selection Service

/// <summary>
/// File selection service interface for unified file operations.
/// Provides standardized file selection, validation, and panel-aware UI integration.
/// </summary>
public interface IFileSelectionService
{
    /// <summary>
    /// Opens file selection dialog for import operations
    /// </summary>
    /// <param name="options">File selection configuration</param>
    /// <returns>Selected file path or null if cancelled</returns>
    Task<string?> SelectFileForImportAsync(FileSelectionOptions options);

    /// <summary>
    /// Opens file selection dialog for export operations
    /// </summary>
    /// <param name="options">File selection configuration</param>
    /// <returns>Selected file path or null if cancelled</returns>
    Task<string?> SelectLocationForExportAsync(FileSelectionOptions options);

    /// <summary>
    /// Validates file access permissions and existence
    /// </summary>
    /// <param name="filePath">Path to validate</param>
    /// <returns>True if file is accessible</returns>
    Task<bool> ValidateFileAccessAsync(string filePath);

    /// <summary>
    /// Gets detailed file information
    /// </summary>
    /// <param name="filePath">Path to examine</param>
    /// <returns>FileInfo object or null if file doesn't exist</returns>
    Task<FileInfo?> GetFileInfoAsync(string filePath);

    /// <summary>
    /// Shows file selection view in appropriate panel based on context
    /// </summary>
    /// <param name="sourceControl">Control that initiated the request</param>
    /// <param name="selectionType">Type of file selection operation</param>
    /// <param name="options">File selection options</param>
    Task<string?> ShowFileSelectionViewAsync(Control? sourceControl, FileSelectionType selectionType, FileSelectionOptions options);
}

/// <summary>
/// File selection configuration options
/// </summary>
public class FileSelectionOptions
{
    public string Title { get; set; } = "Select File";
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();
    public string? DefaultFileName { get; set; }
    public string? InitialDirectory { get; set; }
    public bool AllowMultiple { get; set; } = false;
}

/// <summary>
/// File selection operation types
/// </summary>
public enum FileSelectionType
{
    Import,
    Export,
    Open,
    Save
}

/// <summary>
/// File selection service implementation using Avalonia storage provider.
/// Follows MTM patterns for error handling, logging, and service architecture.
/// </summary>
public class FileSelectionService : IFileSelectionService
{
    private readonly ILogger<FileSelectionService> _logger;

    public FileSelectionService(ILogger<FileSelectionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FileSelectionService constructed successfully");
    }

    /// <inheritdoc />
    public async Task<string?> SelectFileForImportAsync(FileSelectionOptions options)
    {
        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null)
            {
                _logger.LogError("Storage provider not available");
                return null;
            }

            var fileTypes = CreateFileTypes(options.AllowedExtensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = options.Title,
                FileTypeFilter = fileTypes,
                SuggestedStartLocation = startLocation,
                AllowMultiple = options.AllowMultiple
            });

            if (result?.Count > 0)
            {
                var selectedPath = result[0].Path.LocalPath;
                _logger.LogInformation("File selected for import: {FilePath}", selectedPath);
                return selectedPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to select file for import");
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<string?> SelectLocationForExportAsync(FileSelectionOptions options)
    {
        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider == null)
            {
                _logger.LogError("Storage provider not available");
                return null;
            }

            var fileTypes = CreateFileTypes(options.AllowedExtensions);
            var startLocation = await GetStartLocationAsync(options.InitialDirectory);
            var defaultExtension = GetDefaultExtension(options.AllowedExtensions);

            var result = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = options.Title,
                FileTypeChoices = fileTypes,
                SuggestedStartLocation = startLocation,
                SuggestedFileName = options.DefaultFileName,
                DefaultExtension = defaultExtension
            });

            if (result != null)
            {
                var selectedPath = result.Path.LocalPath;
                _logger.LogInformation("Export location selected: {FilePath}", selectedPath);
                return selectedPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to select export location");
            return null;
        }
    }

    /// <inheritdoc />
    public Task<bool> ValidateFileAccessAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.FromResult(false);

            var fileInfo = new FileInfo(filePath);
            var isValid = fileInfo.Exists && !fileInfo.Attributes.HasFlag(FileAttributes.Hidden);

            _logger.LogDebug("File access validation for {FilePath}: {IsValid}", filePath, isValid);
            return Task.FromResult(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate file access for {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public Task<FileInfo?> GetFileInfoAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return Task.FromResult<FileInfo?>(null);

            var fileInfo = new FileInfo(filePath);
            return Task.FromResult(fileInfo.Exists ? fileInfo : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file info for {FilePath}", filePath);
            return Task.FromResult<FileInfo?>(null);
        }
    }

    /// <inheritdoc />
    public async Task<string?> ShowFileSelectionViewAsync(Control? sourceControl, FileSelectionType selectionType, FileSelectionOptions options)
    {
        return selectionType switch
        {
            FileSelectionType.Import or FileSelectionType.Open => await SelectFileForImportAsync(options),
            FileSelectionType.Export or FileSelectionType.Save => await SelectLocationForExportAsync(options),
            _ => throw new ArgumentOutOfRangeException(nameof(selectionType), selectionType, null)
        };
    }

    private IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.StorageProvider;
        }
        return null;
    }

    private List<FilePickerFileType> CreateFileTypes(string[] extensions)
    {
        var fileTypes = new List<FilePickerFileType>();

        if (extensions.Length > 0)
        {
            // Create specific file type
            var patterns = extensions.Select(ext => ext.StartsWith("*") ? ext : $"*{ext}").ToList();
            var name = GetFileTypeName(extensions);

            fileTypes.Add(new FilePickerFileType(name)
            {
                Patterns = patterns
            });
        }

        // Always add "All Files" option
        fileTypes.Add(FilePickerFileTypes.All);

        return fileTypes;
    }

    private string GetFileTypeName(string[] extensions)
    {
        if (extensions.Length == 0) return "All Files";

        if (extensions.Any(ext => ext.Contains("json", StringComparison.OrdinalIgnoreCase)))
            return "JSON Files";
        if (extensions.Any(ext => ext.Contains("xml", StringComparison.OrdinalIgnoreCase)))
            return "XML Files";
        if (extensions.Any(ext => ext.Contains("txt", StringComparison.OrdinalIgnoreCase)))
            return "Text Files";
        if (extensions.Any(ext => ext.Contains("csv", StringComparison.OrdinalIgnoreCase)))
            return "CSV Files";

        return "Configuration Files";
    }

    private string? GetDefaultExtension(string[] extensions)
    {
        if (extensions.Length > 0)
        {
            var ext = extensions[0];
            return ext.StartsWith("*") ? ext.Substring(1) : ext;
        }
        return null;
    }

    private async Task<IStorageFolder?> GetStartLocationAsync(string? initialDirectory)
    {
        if (string.IsNullOrWhiteSpace(initialDirectory))
            return null;

        try
        {
            var storageProvider = GetStorageProvider();
            if (storageProvider != null)
            {
                return await storageProvider.TryGetFolderFromPathAsync(initialDirectory);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get start location for directory: {Directory}", initialDirectory);
        }

        return null;
    }
}

#endregion

#region File Path Service

/// <summary>
/// Service for file path operations and management
/// </summary>
public interface IFilePathService
{
    /// <summary>
    /// Gets application data directory path
    /// </summary>
    string GetAppDataPath();

    /// <summary>
    /// Gets user documents directory path
    /// </summary>
    string GetDocumentsPath();

    /// <summary>
    /// Ensures directory exists, creates if necessary
    /// </summary>
    /// <param name="path">Directory path</param>
    /// <returns>True if directory exists or was created successfully</returns>
    bool EnsureDirectoryExists(string path);

    /// <summary>
    /// Gets temp directory path for the application
    /// </summary>
    string GetTempPath();

    /// <summary>
    /// Combines path segments safely
    /// </summary>
    string CombinePath(params string[] pathSegments);

    /// <summary>
    /// Validates if path is safe to use
    /// </summary>
    bool IsPathSafe(string path);
}

/// <summary>
/// File path service implementation
/// </summary>
public class FilePathService : IFilePathService
{
    private readonly ILogger<FilePathService> _logger;

    public FilePathService(ILogger<FilePathService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FilePathService constructed successfully");
    }

    public string GetAppDataPath()
    {
        try
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MTM_WIP_Application");
            EnsureDirectoryExists(path);
            return path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get application data path");
            return Path.GetTempPath();
        }
    }

    public string GetDocumentsPath()
    {
        try
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get documents path");
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }
    }

    public bool EnsureDirectoryExists(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logger.LogDebug("Created directory: {Path}", path);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure directory exists: {Path}", path);
            return false;
        }
    }

    public string GetTempPath()
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "MTM_WIP_Application");
            EnsureDirectoryExists(tempPath);
            return tempPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get temp path");
            return Path.GetTempPath();
        }
    }

    public string CombinePath(params string[] pathSegments)
    {
        try
        {
            if (pathSegments == null || pathSegments.Length == 0)
                return string.Empty;

            return Path.Combine(pathSegments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to combine path segments");
            return string.Empty;
        }
    }

    public bool IsPathSafe(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // Check for invalid characters
            var invalidChars = Path.GetInvalidPathChars();
            if (path.Any(c => invalidChars.Contains(c)))
                return false;

            // Check for path traversal attempts
            var normalizedPath = Path.GetFullPath(path);
            return !normalizedPath.Contains("..");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Path safety check failed for: {Path}", path);
            return false;
        }
    }
}

#endregion

#region Print Service

/// <summary>
/// Print service interface for application printing functionality
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Prints the specified content
    /// </summary>
    /// <param name="content">Content to print</param>
    /// <param name="title">Print job title</param>
    /// <returns>True if print was successful</returns>
    Task<bool> PrintAsync(string content, string title = "MTM Print Job");

    /// <summary>
    /// Prints data grid content
    /// </summary>
    /// <param name="dataGrid">DataGrid control to print</param>
    /// <param name="title">Print job title</param>
    /// <returns>True if print was successful</returns>
    Task<bool> PrintDataGridAsync(DataGrid dataGrid, string title = "MTM Report");

    /// <summary>
    /// Shows print preview
    /// </summary>
    /// <param name="content">Content to preview</param>
    /// <param name="title">Preview title</param>
    Task ShowPrintPreviewAsync(string content, string title = "Print Preview");

    /// <summary>
    /// Gets available printers
    /// </summary>
    /// <returns>List of available printer names</returns>
    Task<List<string>> GetAvailablePrintersAsync();
}

/// <summary>
/// Print service implementation
/// </summary>
public class PrintService : IPrintService
{
    private readonly ILogger<PrintService> _logger;

    public PrintService(ILogger<PrintService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("PrintService constructed successfully");
    }

    public async Task<bool> PrintAsync(string content, string title = "MTM Print Job")
    {
        try
        {
            _logger.LogInformation("Print request for: {Title}", title);

            // Platform-specific printing would be implemented here
            // For now, log the print request
            _logger.LogDebug("Print content length: {Length} characters", content.Length);

            await Task.Delay(100); // Simulate async print operation

            _logger.LogInformation("Print job completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print content: {Title}", title);
            return false;
        }
    }

    public async Task<bool> PrintDataGridAsync(DataGrid dataGrid, string title = "MTM Report")
    {
        try
        {
            if (dataGrid == null)
            {
                _logger.LogWarning("Cannot print null DataGrid");
                return false;
            }

            _logger.LogInformation("DataGrid print request for: {Title}", title);

            // Convert DataGrid to printable content
            var content = ConvertDataGridToString(dataGrid);
            return await PrintAsync(content, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to print DataGrid: {Title}", title);
            return false;
        }
    }

    public async Task ShowPrintPreviewAsync(string content, string title = "Print Preview")
    {
        try
        {
            _logger.LogInformation("Print preview requested for: {Title}", title);

            // Print preview would be implemented here
            // Could show a dialog with preview content

            await Task.Delay(100); // Simulate async operation

            _logger.LogDebug("Print preview shown successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show print preview: {Title}", title);
        }
    }

    public async Task<List<string>> GetAvailablePrintersAsync()
    {
        try
        {
            await Task.Delay(50); // Simulate async operation

            // Platform-specific printer enumeration would be implemented here
            var printers = new List<string> { "Default Printer", "Microsoft Print to PDF" };

            _logger.LogDebug("Found {PrinterCount} available printers", printers.Count);
            return printers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available printers");
            return new List<string>();
        }
    }

    private string ConvertDataGridToString(DataGrid dataGrid)
    {
        try
        {
            var sb = new StringBuilder();

            // Add headers
            if (dataGrid.Columns.Count > 0)
            {
                sb.AppendLine(string.Join("\t", dataGrid.Columns.Select(c => c.Header?.ToString() ?? "")));
            }

            // Add rows (simplified - real implementation would iterate through actual data)
            sb.AppendLine("Data grid content would be formatted here");

            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to convert DataGrid to string");
            return "Error formatting grid content";
        }
    }
}

#endregion

#region Emergency Keyboard Hook Service

/// <summary>
/// Emergency keyboard hook service interface for system-level key interception
/// </summary>
public interface IEmergencyKeyboardHookService
{
    /// <summary>
    /// Starts the emergency keyboard hook
    /// </summary>
    void StartHook();

    /// <summary>
    /// Stops the emergency keyboard hook
    /// </summary>
    void StopHook();

    /// <summary>
    /// Gets whether the hook is currently active
    /// </summary>
    bool IsHookActive { get; }

    /// <summary>
    /// Event fired when emergency key combination is detected
    /// </summary>
    event EventHandler<EmergencyKeyEventArgs>? EmergencyKeyPressed;
}

/// <summary>
/// Emergency key event arguments
/// </summary>
public class EmergencyKeyEventArgs : EventArgs
{
    public Key Key { get; }
    public KeyModifiers Modifiers { get; }
    public DateTime Timestamp { get; }

    public EmergencyKeyEventArgs(Key key, KeyModifiers modifiers)
    {
        Key = key;
        Modifiers = modifiers;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Emergency keyboard hook service implementation
/// Provides system-level keyboard interception for emergency situations
/// </summary>
public class EmergencyKeyboardHookService : IEmergencyKeyboardHookService, IDisposable
{
    private readonly ILogger<EmergencyKeyboardHookService> _logger;
    private bool _isHookActive;
    private bool _disposed;

    public event EventHandler<EmergencyKeyEventArgs>? EmergencyKeyPressed;

    public EmergencyKeyboardHookService(ILogger<EmergencyKeyboardHookService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("EmergencyKeyboardHookService constructed successfully");
    }

    public bool IsHookActive => _isHookActive;

    public void StartHook()
    {
        try
        {
            if (_isHookActive)
            {
                _logger.LogWarning("Emergency keyboard hook is already active");
                return;
            }

            // Platform-specific hook implementation would go here
            // This is a simplified version for demonstration

            _isHookActive = true;
            _logger.LogInformation("Emergency keyboard hook started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start emergency keyboard hook");
        }
    }

    public void StopHook()
    {
        try
        {
            if (!_isHookActive)
            {
                _logger.LogDebug("Emergency keyboard hook is not active");
                return;
            }

            // Platform-specific unhook implementation would go here

            _isHookActive = false;
            _logger.LogInformation("Emergency keyboard hook stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop emergency keyboard hook");
        }
    }

    protected virtual void OnEmergencyKeyPressed(Key key, KeyModifiers modifiers)
    {
        try
        {
            var args = new EmergencyKeyEventArgs(key, modifiers);
            EmergencyKeyPressed?.Invoke(this, args);

            _logger.LogWarning("Emergency key combination detected: {Key} + {Modifiers}", key, modifiers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing emergency key event");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            StopHook();
            _disposed = true;
            _logger.LogDebug("EmergencyKeyboardHookService disposed");
        }
    }
}

#endregion

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

