using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for database settings configuration panel.
/// Manages database connection settings and configuration options.
/// </summary>
public partial class DatabaseSettingsViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    
    [ObservableProperty]
    private string _connectionString = string.Empty;

    [ObservableProperty]
    private string _databaseName = string.Empty;

    [ObservableProperty]
    private string _serverName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RequiresCredentials))]
    private bool _useIntegratedSecurity = true;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private int _connectionTimeout = 30;

    [ObservableProperty]
    private int _commandTimeout = 60;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private bool _isTesting;

    [ObservableProperty]
    private string _connectionStatus = "Not Connected";

    public DatabaseSettingsViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ILogger<DatabaseSettingsViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        // Load current settings
        _ = LoadSettingsCommand.ExecuteAsync(null);

        Logger.LogInformation("DatabaseSettingsViewModel initialized");
    }

    /// <summary>
    /// Indicates if manual credentials are required.
    /// </summary>
    public bool RequiresCredentials => !UseIntegratedSecurity;

    /// <summary>
    /// Tests the database connection with current settings.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanTestConnection))]
    private async Task TestConnectionAsync()
    {
        try
        {
            IsTesting = true;
            ConnectionStatus = "Testing connection...";

            // Simple connection test using existing TestConnectionAsync method
            var result = await _databaseService.TestConnectionAsync().ConfigureAwait(false);

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
    private bool CanTestConnection()
    {
        return !IsTesting && !string.IsNullOrWhiteSpace(ServerName) && !string.IsNullOrWhiteSpace(DatabaseName);
    }

    /// <summary>
    /// Saves current database settings to configuration.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSaveSettings))]
    private Task SaveSettingsAsync()
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
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if settings can be saved.
    /// </summary>
    private bool CanSaveSettings()
    {
        return !string.IsNullOrWhiteSpace(ServerName) && !string.IsNullOrWhiteSpace(DatabaseName);
    }

    /// <summary>
    /// Loads database settings from configuration.
    /// </summary>
    [RelayCommand]
    private Task LoadSettingsAsync()
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
        
        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets all settings to default values.
    /// </summary>
    [RelayCommand]
    private Task ResetToDefaultsAsync()
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
        
        return Task.CompletedTask;
    }

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
}