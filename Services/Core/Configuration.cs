using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services.Core;

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
        // Use Model_AppVariables connection string logic which handles
        // server, database naming, and uppercase username
        var connectionString = Models.Model_AppVariables.ConnectionString;
        
        // Log the connection string for debugging (without sensitive info)
        _logger.LogInformation("Using connection string: Server={Server}, Database={Database}, Uid={User}", 
            GetServerFromConnectionString(connectionString),
            GetDatabaseFromConnectionString(connectionString),
            GetUserFromConnectionString(connectionString));
            
        return connectionString;
    }
    
    private static string GetServerFromConnectionString(string connectionString)
    {
        var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Server=([^;]+)");
        return match.Success ? match.Groups[1].Value : "Unknown";
    }
    
    private static string GetDatabaseFromConnectionString(string connectionString)
    {
        var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Database=([^;]+)");
        return match.Success ? match.Groups[1].Value : "Unknown";
    }
    
    private static string GetUserFromConnectionString(string connectionString)
    {
        var match = System.Text.RegularExpressions.Regex.Match(connectionString, @"Uid=([^;]+)");
        return match.Success ? match.Groups[1].Value : "Unknown";
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
    private int _progressValue = 0;
    private string _statusText = "Ready";

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    public ApplicationStateService(ILogger<ApplicationStateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Set default user for development/testing
        _currentUser = Environment.UserName.ToUpper();
        _logger.LogInformation("ApplicationStateService initialized with default user: {CurrentUser}", _currentUser);
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

    public int ProgressValue
    {
        get => _progressValue;
        set => SetProperty(ref _progressValue, value, nameof(ProgressValue));
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value, nameof(StatusText));
    }

    public async Task SetProgressAsync(int value, string status)
    {
        ProgressValue = Math.Clamp(value, 0, 100);
        StatusText = status ?? "Processing...";
        
        _logger.LogDebug("Progress updated: {ProgressValue}% - {StatusText}", ProgressValue, StatusText);
        await Task.CompletedTask;
    }

    public async Task ClearProgressAsync()
    {
        ProgressValue = 0;
        StatusText = "Ready";
        
        _logger.LogDebug("Progress cleared");
        await Task.CompletedTask;
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
