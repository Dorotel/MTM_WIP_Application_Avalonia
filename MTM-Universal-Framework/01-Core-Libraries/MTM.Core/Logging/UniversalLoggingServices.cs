using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.Core.Logging;

/// <summary>
/// Universal file logging service interface.
/// </summary>
public interface IUniversalFileLoggingService : IDisposable
{
    /// <summary>
    /// Logs a business operation message.
    /// </summary>
    /// <param name="level">Log level</param>
    /// <param name="message">Log message</param>
    /// <param name="category">Log category</param>
    /// <param name="context">Additional context</param>
    void LogBusiness(LogLevel level, string message, string category = "General", Dictionary<string, object>? context = null);

    /// <summary>
    /// Logs an error with full context.
    /// </summary>
    /// <param name="exception">Exception to log</param>
    /// <param name="operation">Operation context</param>
    /// <param name="userId">User identifier</param>
    /// <param name="context">Additional context</param>
    /// <returns>Task representing the async operation</returns>
    Task LogErrorAsync(Exception exception, string operation, string userId, Dictionary<string, object>? context = null);
}

/// <summary>
/// Universal settings service interface.
/// </summary>
public interface IUniversalSettingsService
{
    /// <summary>
    /// Gets a setting value by key.
    /// </summary>
    /// <param name="key">Setting key</param>
    /// <param name="defaultValue">Default value if key not found</param>
    /// <returns>Setting value or default</returns>
    string GetSetting(string key, string defaultValue = "");

    /// <summary>
    /// Sets a setting value by key.
    /// </summary>
    /// <param name="key">Setting key</param>
    /// <param name="value">Setting value</param>
    /// <returns>Task representing the async operation</returns>
    Task SetSettingAsync(string key, string value);

    /// <summary>
    /// Loads settings from storage.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task LoadSettingsAsync();

    /// <summary>
    /// Saves settings to storage.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task SaveSettingsAsync();
}

/// <summary>
/// Universal file logging service implementation.
/// </summary>
public class UniversalFileLoggingService : IUniversalFileLoggingService
{
    private readonly ILogger<UniversalFileLoggingService> _logger;
    private readonly string _logDirectory;
    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the UniversalFileLoggingService.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="options">Logging options</param>
    public UniversalFileLoggingService(ILogger<UniversalFileLoggingService> logger, UniversalLoggingOptions? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logDirectory = options?.BasePath ?? Path.Combine(AppContext.BaseDirectory, "Logs");
        
        Directory.CreateDirectory(_logDirectory);
    }

    /// <inheritdoc />
    public void LogBusiness(LogLevel level, string message, string category = "General", Dictionary<string, object>? context = null)
    {
        if (_disposed) return;

        try
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{category}] {message}";
            if (context != null && context.Count > 0)
            {
                logEntry += $" | Context: {string.Join(", ", context.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            }

            var logFile = Path.Combine(_logDirectory, $"business-{DateTime.Now:yyyy-MM-dd}.log");
            File.AppendAllText(logFile, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write business log");
        }
    }

    /// <inheritdoc />
    public async Task LogErrorAsync(Exception exception, string operation, string userId, Dictionary<string, object>? context = null)
    {
        if (_disposed) return;

        try
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [ERROR] Operation: {operation} | User: {userId} | Exception: {exception.GetType().Name} | Message: {exception.Message}";
            if (context != null && context.Count > 0)
            {
                logEntry += $" | Context: {string.Join(", ", context.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
            }
            logEntry += $" | StackTrace: {exception.StackTrace}";

            var logFile = Path.Combine(_logDirectory, $"errors-{DateTime.Now:yyyy-MM-dd}.log");
            await File.AppendAllTextAsync(logFile, logEntry + Environment.NewLine);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write error log");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }
}

/// <summary>
/// Universal settings service implementation.
/// </summary>
public class UniversalSettingsService : IUniversalSettingsService
{
    private readonly ILogger<UniversalSettingsService> _logger;
    private readonly Dictionary<string, string> _settings = new();
    private readonly string _settingsFile;

    /// <summary>
    /// Initializes a new instance of the UniversalSettingsService.
    /// </summary>
    /// <param name="logger">Logger instance</param>
    public UniversalSettingsService(ILogger<UniversalSettingsService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settingsFile = Path.Combine(AppContext.BaseDirectory, "settings.json");
    }

    /// <inheritdoc />
    public string GetSetting(string key, string defaultValue = "")
    {
        return _settings.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <inheritdoc />
    public async Task SetSettingAsync(string key, string value)
    {
        _settings[key] = value;
        await SaveSettingsAsync();
    }

    /// <inheritdoc />
    public async Task LoadSettingsAsync()
    {
        try
        {
            if (File.Exists(_settingsFile))
            {
                var json = await File.ReadAllTextAsync(_settingsFile);
                var settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                if (settings != null)
                {
                    _settings.Clear();
                    foreach (var kvp in settings)
                    {
                        _settings[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings from {SettingsFile}", _settingsFile);
        }
    }

    /// <inheritdoc />
    public async Task SaveSettingsAsync()
    {
        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_settingsFile, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to {SettingsFile}", _settingsFile);
        }
    }
}