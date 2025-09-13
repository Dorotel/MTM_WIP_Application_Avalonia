using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Custom file logging service that saves business logs and MySQL error logs to both network and local directories.
/// </summary>
public class FileLoggingService : IFileLoggingService
{
    private readonly string _networkBasePath;
    private readonly string _localBasePath;
    private readonly string _currentUser;
    private readonly bool _enableDualLocationLogging;
    private readonly Timer _flushTimer;
    private readonly ConcurrentQueue<LogEntry> _logQueue;
    private readonly object _lockObject = new();
    private bool _disposed = false;
    private readonly bool _suppressNetworkErrors;
    private readonly bool _networkLoggingOptional;
    private volatile bool _networkPathAvailable = true;
    private volatile bool _networkPathPreviouslyFailed = false;
    private readonly TimeSpan _networkRecheckInterval = TimeSpan.FromMinutes(5);
    private Timer? _networkRecheckTimer;
    private DateTime _lastNetworkFailureUtc = DateTime.MinValue; // DateTime is immutable struct; volatile not allowed

    public FileLoggingService(IConfiguration configuration, IFilePathService filePathService)
    {
        _currentUser = Environment.UserName.ToUpper();
        
        // Network path (primary)
        _networkBasePath = configuration["Logging:File:BasePath"] ??
                          configuration["ErrorHandling:FileServerPath"] ??
                          @"\\mtmanu-fs01\Expo Drive\MH_RESOURCE\Material_Handler\MTM WIP App\Logs";
        
        // Local path (secondary) - Use MyDocuments instead of AppData
        _localBasePath = filePathService.GetLogsPath();
        
    // Dual location logging setting
    _enableDualLocationLogging = configuration.GetValue<bool>("Logging:File:EnableDualLocationLogging", true);
    _suppressNetworkErrors = configuration.GetValue<bool>("Logging:File:SuppressNetworkErrors", false);
    _networkLoggingOptional = configuration.GetValue<bool>("Logging:File:NetworkLoggingOptional", false);
        
        _logQueue = new ConcurrentQueue<LogEntry>();
        
        // Get flush interval from configuration
        var flushInterval = configuration.GetValue<int>("Logging:File:FlushIntervalSeconds", 10);
        
        // Flush logs at configured interval
        _flushTimer = new Timer(FlushLogs, null, TimeSpan.FromSeconds(flushInterval), TimeSpan.FromSeconds(flushInterval));

        // Initial network path probe (non-blocking critical path)
        _networkPathAvailable = ProbeNetworkPath();
        if (!_networkPathAvailable)
        {
            _networkPathPreviouslyFailed = true;
            _lastNetworkFailureUtc = DateTime.UtcNow;
        }

        // Recheck timer only if network logging configured
        if (!string.IsNullOrWhiteSpace(_networkBasePath))
        {
            _networkRecheckTimer = new Timer(_ =>
            {
                try
                {
                    if (!_networkPathAvailable && (DateTime.UtcNow - _lastNetworkFailureUtc) >= _networkRecheckInterval)
                    {
                        if (Directory.Exists(_networkBasePath))
                        {
                            _networkPathAvailable = true;
                            _networkPathPreviouslyFailed = false;
                        }
                        else
                        {
                            _lastNetworkFailureUtc = DateTime.UtcNow;
                        }
                    }
                }
                catch
                {
                    // Suppress recheck exceptions
                }
            }, null, _networkRecheckInterval, _networkRecheckInterval);
        }
    }

    /// <summary>
    /// Logs a business operation message.
    /// </summary>
    public void LogBusiness(LogLevel level, string message, string category = "General", Dictionary<string, object>? context = null)
    {
        var logEntry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Category = "BusinessLog",
            Message = message,
            SubCategory = category,
            UserId = _currentUser,
            Context = context ?? new Dictionary<string, object>()
        };

        _logQueue.Enqueue(logEntry);
    }

    /// <summary>
    /// Logs a MySQL error or database operation.
    /// </summary>
    public void LogMySql(LogLevel level, string message, string operation = "Unknown", Exception? exception = null, Dictionary<string, object>? context = null)
    {
        var logEntry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Category = "MySQLLog",
            Message = message,
            SubCategory = operation,
            UserId = _currentUser,
            Exception = exception,
            Context = context ?? new Dictionary<string, object>()
        };

        _logQueue.Enqueue(logEntry);
    }

    /// <summary>
    /// Logs a general application message (routes to business log).
    /// </summary>
    public void LogGeneral(LogLevel level, string message, string source = "Application", Dictionary<string, object>? context = null)
    {
        LogBusiness(level, message, source, context);
    }

    /// <summary>
    /// Flushes all queued log entries to files.
    /// </summary>
    private void FlushLogs(object? state)
    {
        if (_logQueue.IsEmpty) return;

        var businessLogs = new List<LogEntry>();
        var mysqlLogs = new List<LogEntry>();

        // Dequeue all logs
        while (_logQueue.TryDequeue(out var logEntry))
        {
            if (logEntry.Category == "BusinessLog")
                businessLogs.Add(logEntry);
            else if (logEntry.Category == "MySQLLog")
                mysqlLogs.Add(logEntry);
        }

        // Write business logs
        if (businessLogs.Count > 0)
        {
            Task.Run(() => WriteLogsToFile(businessLogs, "BusinessLog.csv"));
        }

        // Write MySQL logs
        if (mysqlLogs.Count > 0)
        {
            Task.Run(() => WriteLogsToFile(mysqlLogs, "MySQLErrors.csv"));
        }
    }

    /// <summary>
    /// Writes log entries to both network and local file locations simultaneously.
    /// </summary>
    private void WriteLogsToFile(List<LogEntry> logs, string fileName)
    {
        if (!_enableDualLocationLogging)
        {
            // Fallback mode - try network first, then local
            try
            {
                if (_networkPathAvailable)
                {
                    WriteToLocation(logs, Path.Combine(_networkBasePath, _currentUser), fileName);
                }
                else if (!_networkLoggingOptional && !_suppressNetworkErrors && !_networkPathPreviouslyFailed)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Network log path unavailable: {_networkBasePath}");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    WriteToLocation(logs, Path.Combine(_localBasePath, _currentUser), fileName);
                    
                    // Log network failure
                    var networkFailureLog = new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        Level = LogLevel.Warning,
                        Category = "SystemLog",
                        Message = $"Network logging failed, using local fallback: {ex.Message}",
                        UserId = _currentUser,
                        Exception = ex
                    };
                    WriteToLocation(new List<LogEntry> { networkFailureLog }, Path.Combine(_localBasePath, _currentUser), "SystemLog.csv");
                }
                catch (Exception localEx)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL: Both log locations failed. Network: {ex.Message}, Local: {localEx.Message}");
                }
            }
            return;
        }

    // Dual location mode - write to both simultaneously
    var tasks = new List<Task>();

        // Write to network location
        tasks.Add(Task.Run(() =>
        {
            if (!_networkPathAvailable)
            {
                return; // Skip attempt until recheck succeeds
            }
            try
            {
                WriteToLocation(logs, Path.Combine(_networkBasePath, _currentUser), fileName);
            }
            catch (IOException ex)
            {
                _networkPathAvailable = false;
                _lastNetworkFailureUtc = DateTime.UtcNow;
                if (!_networkLoggingOptional && !_suppressNetworkErrors && !_networkPathPreviouslyFailed)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Network I/O failure: {ex.Message}");
                }
                _networkPathPreviouslyFailed = true;
                if (!_networkLoggingOptional)
                {
                    var networkFailureLog = new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        Level = LogLevel.Warning,
                        Category = "SystemLog",
                        Message = $"Network I/O failure - switched to local only: {ex.Message}",
                        UserId = _currentUser,
                        Exception = ex
                    };
                    try
                    {
                        WriteToLocation(new List<LogEntry> { networkFailureLog }, Path.Combine(_localBasePath, _currentUser), "SystemLog.csv");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                _networkPathAvailable = false;
                _lastNetworkFailureUtc = DateTime.UtcNow;
                if (!_networkLoggingOptional && !_suppressNetworkErrors && !_networkPathPreviouslyFailed)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Network logging failed: {ex.Message}");
                }
                _networkPathPreviouslyFailed = true;
            }
        }));

        // Write to local location
        tasks.Add(Task.Run(() =>
        {
            try
            {
                WriteToLocation(logs, Path.Combine(_localBasePath, _currentUser), fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL: Failed to write to local log location: {ex.Message}");
            }
        }));

        // Wait for both writes to complete (with timeout)
        try
        {
            Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(10));
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Some log writes failed: {string.Join(", ", ex.InnerExceptions.Select(e => e.Message))}");
        }
    }

    /// <summary>
    /// Writes logs to a specific location.
    /// </summary>
    private void WriteToLocation(List<LogEntry> logs, string directoryPath, string fileName)
    {
        lock (_lockObject)
        {
            Directory.CreateDirectory(directoryPath);
            var filePath = Path.Combine(directoryPath, fileName);
            var fileExists = File.Exists(filePath);

            using var writer = new StreamWriter(filePath, append: true);
            
            // Write header if file is new
            if (!fileExists)
            {
                writer.WriteLine("Timestamp,Level,Category,SubCategory,UserId,Message,Context,Exception");
            }

            // Write log entries
            foreach (var log in logs)
            {
                var contextJson = log.Context.Count > 0 ? 
                    System.Text.Json.JsonSerializer.Serialize(log.Context) : "";
                var exceptionInfo = log.Exception != null ? 
                    $"{log.Exception.GetType().Name}: {log.Exception.Message}" : "";

                var csvLine = $"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\"," +
                             $"\"{log.Level}\"," +
                             $"\"{log.Category}\"," +
                             $"\"{log.SubCategory}\"," +
                             $"\"{log.UserId}\"," +
                             $"\"{EscapeCsv(log.Message)}\"," +
                             $"\"{EscapeCsv(contextJson)}\"," +
                             $"\"{EscapeCsv(exceptionInfo)}\"";

                writer.WriteLine(csvLine);
            }
        }
    }

    /// <summary>
    /// Escapes CSV special characters.
    /// </summary>
    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("\"", "\"\"").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
    }

    /// <summary>
    /// Immediately flushes all queued logs (for shutdown scenarios).
    /// </summary>
    public void FlushImmediate()
    {
        FlushLogs(null);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _flushTimer?.Dispose();
            _networkRecheckTimer?.Dispose();
            FlushImmediate(); // Flush any remaining logs
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private bool ProbeNetworkPath()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_networkBasePath)) return false;
            if (!Directory.Exists(_networkBasePath)) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Represents a log entry for file logging.
/// </summary>
internal class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Category { get; set; } = "";
    public string SubCategory { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Message { get; set; } = "";
    public Dictionary<string, object> Context { get; set; } = new();
    public Exception? Exception { get; set; }
}

/// <summary>
/// Interface for file logging service that saves business logs and MySQL error logs.
/// </summary>
public interface IFileLoggingService : IDisposable
{
    /// <summary>
    /// Logs a business operation message.
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="message">Log message</param>
    /// <param name="category">Business category (optional)</param>
    /// <param name="context">Additional context data (optional)</param>
    void LogBusiness(LogLevel level, string message, string category = "General", Dictionary<string, object>? context = null);

    /// <summary>
    /// Logs a MySQL error or database operation.
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="message">Log message</param>
    /// <param name="operation">Database operation name (optional)</param>
    /// <param name="exception">Exception if any (optional)</param>
    /// <param name="context">Additional context data (optional)</param>
    void LogMySql(LogLevel level, string message, string operation = "Unknown", Exception? exception = null, Dictionary<string, object>? context = null);

    /// <summary>
    /// Logs a general application message (routes to business log).
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="message">Log message</param>
    /// <param name="source">Source component (optional)</param>
    /// <param name="context">Additional context data (optional)</param>
    void LogGeneral(LogLevel level, string message, string source = "Application", Dictionary<string, object>? context = null);

    /// <summary>
    /// Immediately flushes all queued logs (for shutdown scenarios).
    /// </summary>
    void FlushImmediate();
}
