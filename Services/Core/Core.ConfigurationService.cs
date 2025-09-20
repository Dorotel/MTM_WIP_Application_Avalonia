using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services.Core;

#region Configuration Services

/// <summary>
/// Unified configuration and application state management service.
/// Provides centralized access to configuration settings and application state.
/// </summary>
public interface IConfigurationService
{
    string GetConnectionString(string name = "DefaultConnection");
    T GetValue<T>(string key, T defaultValue = default!);
    string GetValue(string key, string defaultValue = "");
    bool GetBoolValue(string key, bool defaultValue = false);
    int GetIntValue(string key, int defaultValue = 0);
}

/// <summary>
/// Application state management interface.
/// </summary>
public interface IApplicationStateService : INotifyPropertyChanged
{
    string CurrentUser { get; set; }
    string CurrentLocation { get; set; }
    string CurrentOperation { get; set; }
    bool IsOfflineMode { get; set; }

    // Progress communication for MainView integration
    int ProgressValue { get; set; }
    string StatusText { get; set; }

    // Async progress communication methods
    Task SetProgressAsync(int value, string status);
    Task ClearProgressAsync();

    event EventHandler<StateChangedEventArgs>? StateChanged;
}

/// <summary>
/// Configuration service implementation.
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(IConfiguration configuration, ILogger<ConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString(string name = "DefaultConnection")
    {
        var connectionString = _configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogWarning("Connection string '{Name}' not found", name);
            throw new InvalidOperationException($"Connection string '{name}' not configured");
        }
        return connectionString;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            if (value == null) return defaultValue;

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get configuration value for key '{Key}', using default", key);
            return defaultValue;
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return _configuration[key] ?? defaultValue;
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        var value = _configuration[key];
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        var value = _configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
}

/// <summary>
/// Application state service implementation.
/// </summary>
public class ApplicationStateService : IApplicationStateService
{
    private string _currentUser = Environment.UserName;
    private string _currentLocation = string.Empty;
    private string _currentOperation = string.Empty;
    private bool _isOfflineMode = false;
    private int _progressValue = 0;
    private string _statusText = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public string CurrentUser
    {
        get => _currentUser;
        set
        {
            if (_currentUser != value)
            {
                var oldValue = _currentUser;
                _currentUser = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentUser), oldValue, value));
            }
        }
    }

    public string CurrentLocation
    {
        get => _currentLocation;
        set
        {
            if (_currentLocation != value)
            {
                var oldValue = _currentLocation;
                _currentLocation = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentLocation), oldValue, value));
            }
        }
    }

    public string CurrentOperation
    {
        get => _currentOperation;
        set
        {
            if (_currentOperation != value)
            {
                var oldValue = _currentOperation;
                _currentOperation = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(CurrentOperation), oldValue, value));
            }
        }
    }

    public bool IsOfflineMode
    {
        get => _isOfflineMode;
        set
        {
            if (_isOfflineMode != value)
            {
                _isOfflineMode = value;
                OnPropertyChanged();
                StateChanged?.Invoke(this, new StateChangedEventArgs(nameof(IsOfflineMode), !value, value));
            }
        }
    }

    public int ProgressValue
    {
        get => _progressValue;
        set
        {
            if (_progressValue != value)
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }
    }

    public string StatusText
    {
        get => _statusText;
        set
        {
            if (_statusText != value)
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }
    }

    public async Task SetProgressAsync(int value, string status)
    {
        ProgressValue = value;
        StatusText = status;
        await Task.CompletedTask; // Allow for future async operations
    }

    public async Task ClearProgressAsync()
    {
        ProgressValue = 0;
        StatusText = string.Empty;
        await Task.CompletedTask;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Event args for state change events.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    public string PropertyName { get; }
    public object? OldValue { get; }
    public object? NewValue { get; }

    public StateChangedEventArgs(string propertyName, object? oldValue, object? newValue)
    {
        PropertyName = propertyName;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

#endregion
