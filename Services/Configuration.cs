using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

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
            return "";
        }
        return connectionString;
    }

    public T GetValue<T>(string key, T defaultValue = default!)
    {
        try
        {
            var value = _configuration[key];
            if (value == null)
            {
                _logger.LogDebug("Configuration key '{Key}' not found, using default value", key);
                return defaultValue;
            }

            if (typeof(T) == typeof(string))
                return (T)(object)value;

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading configuration key '{Key}', using default value", key);
            return defaultValue;
        }
    }

    public string GetValue(string key, string defaultValue = "")
    {
        return GetValue<string>(key, defaultValue);
    }

    public bool GetBoolValue(string key, bool defaultValue = false)
    {
        return GetValue<bool>(key, defaultValue);
    }

    public int GetIntValue(string key, int defaultValue = 0)
    {
        return GetValue<int>(key, defaultValue);
    }
}

/// <summary>
/// Application state service implementation.
/// </summary>
public class ApplicationStateService : IApplicationStateService
{
    private readonly ILogger<ApplicationStateService> _logger;
    private string _currentUser = string.Empty;
    private string _currentLocation = string.Empty;
    private string _currentOperation = string.Empty;
    private bool _isOfflineMode = false;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public ApplicationStateService(ILogger<ApplicationStateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value, nameof(CurrentUser));
    }

    public string CurrentLocation
    {
        get => _currentLocation;
        set => SetProperty(ref _currentLocation, value, nameof(CurrentLocation));
    }

    public string CurrentOperation
    {
        get => _currentOperation;
        set => SetProperty(ref _currentOperation, value, nameof(CurrentOperation));
    }

    public bool IsOfflineMode
    {
        get => _isOfflineMode;
        set => SetProperty(ref _isOfflineMode, value, nameof(IsOfflineMode));
    }

    private void SetProperty<T>(ref T field, T value, string propertyName)
    {
        if (!Equals(field, value))
        {
            var oldValue = field;
            field = value;
            
            _logger.LogDebug("Application state changed: {PropertyName} = {NewValue} (was {OldValue})", 
                propertyName, value, oldValue);
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            
            StateChanged?.Invoke(this, new StateChangedEventArgs
            {
                PropertyName = propertyName,
                OldValue = oldValue,
                NewValue = value
            });
        }
    }
}

/// <summary>
/// State change event arguments.
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    public string PropertyName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
}
