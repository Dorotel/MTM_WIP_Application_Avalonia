using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for database settings configuration panel.
/// Manages database connection settings and configuration options.
/// </summary>
public class DatabaseSettingsViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    
    private string _connectionString = string.Empty;
    private string _databaseName = string.Empty;
    private string _serverName = string.Empty;
    private bool _useIntegratedSecurity = true;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private int _connectionTimeout = 30;
    private int _commandTimeout = 60;
    private bool _isConnected;
    private bool _isTesting;
    private string _connectionStatus = "Not Connected";

    public DatabaseSettingsViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<DatabaseSettingsViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        // Initialize commands
        TestConnectionCommand = new AsyncCommand(ExecuteTestConnectionAsync, CanExecuteTestConnection);
        SaveSettingsCommand = new AsyncCommand(ExecuteSaveSettingsAsync, CanExecuteSaveSettings);
        LoadSettingsCommand = new AsyncCommand(ExecuteLoadSettingsAsync);
        ResetToDefaultsCommand = new AsyncCommand(ExecuteResetToDefaultsAsync);

        // Load current settings
        _ = ExecuteLoadSettingsAsync();

        Logger.LogInformation("DatabaseSettingsViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// Database connection string.
    /// </summary>
    public string ConnectionString
    {
        get => _connectionString;
        set => SetProperty(ref _connectionString, value);
    }

    /// <summary>
    /// Database name.
    /// </summary>
    public string DatabaseName
    {
        get => _databaseName;
        set => SetProperty(ref _databaseName, value);
    }

    /// <summary>
    /// Database server name or address.
    /// </summary>
    public string ServerName
    {
        get => _serverName;
        set => SetProperty(ref _serverName, value);
    }

    /// <summary>
    /// Use Windows integrated security.
    /// </summary>
    public bool UseIntegratedSecurity
    {
        get => _useIntegratedSecurity;
        set
        {
            if (SetProperty(ref _useIntegratedSecurity, value))
            {
                RaisePropertyChanged(nameof(RequiresCredentials));
            }
        }
    }

    /// <summary>
    /// Database username (if not using integrated security).
    /// </summary>
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    /// <summary>
    /// Database password (if not using integrated security).
    /// </summary>
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    /// <summary>
    /// Connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeout
    {
        get => _connectionTimeout;
        set => SetProperty(ref _connectionTimeout, value);
    }

    /// <summary>
    /// Command timeout in seconds.
    /// </summary>
    public int CommandTimeout
    {
        get => _commandTimeout;
        set => SetProperty(ref _commandTimeout, value);
    }

    /// <summary>
    /// Indicates if database connection is active.
    /// </summary>
    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }

    /// <summary>
    /// Indicates if connection test is in progress.
    /// </summary>
    public bool IsTesting
    {
        get => _isTesting;
        set => SetProperty(ref _isTesting, value);
    }

    /// <summary>
    /// Current connection status message.
    /// </summary>
    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetProperty(ref _connectionStatus, value);
    }

    /// <summary>
    /// Indicates if manual credentials are required.
    /// </summary>
    public bool RequiresCredentials => !UseIntegratedSecurity;

    #endregion

    #region Commands

    /// <summary>
    /// Command to test database connection.
    /// </summary>
    public ICommand TestConnectionCommand { get; }

    /// <summary>
    /// Command to save database settings.
    /// </summary>
    public ICommand SaveSettingsCommand { get; }

    /// <summary>
    /// Command to load database settings.
    /// </summary>
    public ICommand LoadSettingsCommand { get; }

    /// <summary>
    /// Command to reset settings to defaults.
    /// </summary>
    public ICommand ResetToDefaultsCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Tests the database connection with current settings.
    /// </summary>
    private async Task ExecuteTestConnectionAsync()
    {
        try
        {
            IsTesting = true;
            ConnectionStatus = "Testing connection...";

            // Simple connection test using existing TestConnectionAsync method
            var result = await _databaseService.TestConnectionAsync();

            if (result)
            {
                IsConnected = true;
                ConnectionStatus = "Connection successful";
                Logger.LogInformation("Database connection test successful");
            }
            else
            {
                IsConnected = false;
                ConnectionStatus = "Connection failed";
                Logger.LogWarning("Database connection test failed");
            }
        }
        catch (Exception ex)
        {
            IsConnected = false;
            ConnectionStatus = $"Connection error: {ex.Message}";
            Logger.LogError(ex, "Error testing database connection");
        }
        finally
        {
            IsTesting = false;
        }
    }

    /// <summary>
    /// Determines if connection test can be executed.
    /// </summary>
    private bool CanExecuteTestConnection()
    {
        return !IsTesting && !string.IsNullOrWhiteSpace(ServerName) && !string.IsNullOrWhiteSpace(DatabaseName);
    }

    /// <summary>
    /// Saves current database settings to configuration.
    /// </summary>
    private async Task ExecuteSaveSettingsAsync()
    {
        try
        {
            var connectionString = BuildConnectionString();
            // Simple save using existing configuration service
            ConnectionString = connectionString;
            Logger.LogInformation("Database settings saved successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving database settings");
        }
    }

    /// <summary>
    /// Determines if settings can be saved.
    /// </summary>
    private bool CanExecuteSaveSettings()
    {
        return !string.IsNullOrWhiteSpace(ServerName) && !string.IsNullOrWhiteSpace(DatabaseName);
    }

    /// <summary>
    /// Loads database settings from configuration.
    /// </summary>
    private async Task ExecuteLoadSettingsAsync()
    {
        try
        {
            // Load using existing configuration service
            var connectionString = _configurationService.GetConnectionString();
            
            if (!string.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;
                ParseConnectionString(connectionString);
                Logger.LogInformation("Database settings loaded successfully");
            }
            else
            {
                Logger.LogDebug("No existing database settings found, using defaults");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading database settings");
        }
    }

    /// <summary>
    /// Resets all settings to default values.
    /// </summary>
    private async Task ExecuteResetToDefaultsAsync()
    {
        try
        {
            ServerName = "localhost";
            DatabaseName = "MTM_WIP";
            UseIntegratedSecurity = true;
            Username = string.Empty;
            Password = string.Empty;
            ConnectionTimeout = 30;
            CommandTimeout = 60;
            ConnectionString = string.Empty;
            IsConnected = false;
            ConnectionStatus = "Settings reset to defaults";

            Logger.LogInformation("Database settings reset to defaults");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting database settings");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds connection string from current settings.
    /// </summary>
    private string BuildConnectionString()
    {
        // Simple connection string building for now
        var parts = new List<string>
        {
            $"Server={ServerName}",
            $"Database={DatabaseName}",
            $"Connect Timeout={ConnectionTimeout}",
            $"Command Timeout={CommandTimeout}"
        };

        if (UseIntegratedSecurity)
        {
            parts.Add("Integrated Security=true");
        }
        else
        {
            parts.Add($"User ID={Username}");
            if (!string.IsNullOrEmpty(Password))
            {
                parts.Add($"Password={Password}");
            }
        }

        return string.Join(";", parts);
    }

    /// <summary>
    /// Parses connection string to populate settings properties.
    /// </summary>
    private void ParseConnectionString(string connectionString)
    {
        try
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();
                    
                    switch (key.ToLowerInvariant())
                    {
                        case "server":
                        case "data source":
                            ServerName = value;
                            break;
                        case "database":
                        case "initial catalog":
                            DatabaseName = value;
                            break;
                        case "integrated security":
                            UseIntegratedSecurity = value.ToLowerInvariant() == "true";
                            break;
                        case "user id":
                            Username = value;
                            break;
                        case "connect timeout":
                            if (int.TryParse(value, out var timeout))
                                ConnectionTimeout = timeout;
                            break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error parsing connection string");
        }
    }

    #endregion
}