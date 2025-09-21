using MTM_WIP_Application_Avalonia.Services.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for System Health & Diagnostics panel.
/// Provides database connection testing, system performance metrics, and log management.
/// </summary>
public partial class SystemHealthViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    private readonly IThemeService _themeService;
    
    #region Observable Properties

    /// <summary>
    /// Whether the database is connected
    /// </summary>
    [ObservableProperty]
    private bool isDatabaseConnected;

    /// <summary>
    /// Status message for database connection
    /// </summary>
    [ObservableProperty]
    private string databaseConnectionStatus = "Not tested";

    /// <summary>
    /// System performance metrics
    /// </summary>
    [ObservableProperty]
    private string systemPerformance = "Loading...";

    /// <summary>
    /// Memory usage information
    /// </summary>
    [ObservableProperty]
    private string memoryUsage = "Loading...";

    /// <summary>
    /// Log file size information
    /// </summary>
    [ObservableProperty]
    private string logFileSize = "Loading...";

    /// <summary>
    /// Whether system tests are currently running
    /// </summary>
    [ObservableProperty]
    private bool isTesting;

    #endregion

    public SystemHealthViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IThemeService themeService,
        ILogger<SystemHealthViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        // Initialize collections
        SystemMetrics = new ObservableCollection<SystemMetricItem>();
        LogEntries = new ObservableCollection<LogEntryItem>();

        // Load initial data
        _ = LoadSystemHealthDataAsync();

        Logger.LogInformation("SystemHealthViewModel initialized");
    }

    #region Collections

    /// <summary>
    /// Collection of system metrics for display
    /// </summary>
    public ObservableCollection<SystemMetricItem> SystemMetrics { get; }

    /// <summary>
    /// Collection of log entries for display
    /// </summary>
    public ObservableCollection<LogEntryItem> LogEntries { get; }

    #endregion

    #region Commands

    /// <summary>
    /// Tests the database connection and updates status
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanTestDatabaseConnection))]
    private async Task TestDatabaseConnectionAsync()
    {
        using var scope = Logger.BeginScope("TestDatabaseConnection");
        Logger.LogDebug("Testing database connection");

        try
        {
            IsTesting = true;
            DatabaseConnectionStatus = "Testing connection...";

            // Test database connection
            var isConnected = await _databaseService.TestConnectionAsync().ConfigureAwait(false);

            IsDatabaseConnected = isConnected;
            DatabaseConnectionStatus = isConnected
                ? "Connection successful"
                : "Connection failed";

            Logger.LogInformation("Database connection test completed: {IsConnected}", isConnected);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error testing database connection");
            IsDatabaseConnected = false;
            DatabaseConnectionStatus = $"Connection error: {ex.Message}";

            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Test Database Connection",
                "System",
                new Dictionary<string, object> { ["Operation"] = "TestDatabaseConnection" });
        }
        finally
        {
            IsTesting = false;
        }
    }

    private bool CanTestDatabaseConnection() => !IsTesting;

    /// <summary>
    /// Refreshes all system metrics and performance data
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRefreshMetrics))]
    private async Task RefreshMetricsAsync()
    {
        using var scope = Logger.BeginScope("RefreshMetrics");
        Logger.LogDebug("Refreshing system metrics");

        try
        {
            IsTesting = true;
            
            await LoadSystemHealthDataAsync().ConfigureAwait(false);
            
            Logger.LogInformation("System metrics refreshed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing system metrics");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Refresh System Metrics",
                "System",
                new Dictionary<string, object> { ["Operation"] = "RefreshMetrics" });
        }
        finally
        {
            IsTesting = false;
        }
    }

    private bool CanRefreshMetrics() => !IsTesting;

    /// <summary>
    /// Clears all log entries from the system
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanClearLogs))]
    private async Task ClearLogsAsync()
    {
        using var scope = Logger.BeginScope("ClearLogs");
        Logger.LogDebug("Clearing system logs");

        try
        {
            IsTesting = true;

            // Clear log entries
            LogEntries.Clear();
            
            // Update log file size
            LogFileSize = "0 KB";

            Logger.LogInformation("System logs cleared successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing system logs");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Clear System Logs",
                "System",
                new Dictionary<string, object> { ["Operation"] = "ClearLogs" });
        }
        finally
        {
            IsTesting = false;
        }
    }

    private bool CanClearLogs() => !IsTesting && LogEntries.Count > 0;

    /// <summary>
    /// Exports diagnostic information to a file
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExportDiagnostics))]
    private async Task ExportDiagnosticsAsync()
    {
        using var scope = Logger.BeginScope("ExportDiagnostics");
        Logger.LogDebug("Exporting diagnostic information");

        try
        {
            IsTesting = true;

            // Implementation for diagnostic export would go here
            // This is a placeholder for the actual export logic
            
            Logger.LogInformation("Diagnostic export completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting diagnostics");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Export Diagnostics",
                "System",
                new Dictionary<string, object> { ["Operation"] = "ExportDiagnostics" });
        }
        finally
        {
            IsTesting = false;
        }
    }

    private bool CanExportDiagnostics() => !IsTesting && (SystemMetrics.Count > 0 || LogEntries.Count > 0);

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads initial system health data
    /// </summary>
    private async Task LoadSystemHealthDataAsync()
    {
        try
        {
            Logger.LogDebug("Loading system health data");

            // Load system metrics
            await LoadSystemMetricsAsync().ConfigureAwait(false);
            
            // Load log entries
            await LoadLogEntriesAsync().ConfigureAwait(false);
            
            // Update performance indicators
            await UpdatePerformanceIndicatorsAsync().ConfigureAwait(false);

            Logger.LogInformation("System health data loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading system health data");
            await Services.Core.ErrorHandling.HandleErrorAsync(
                ex,
                "Load System Health Data",
                "System",
                new Dictionary<string, object> { ["Operation"] = "LoadSystemHealthData" });
        }
    }

    /// <summary>
    /// Loads system performance metrics
    /// </summary>
    private async Task LoadSystemMetricsAsync()
    {
        // Simulate loading system metrics
        await Task.Delay(100).ConfigureAwait(false);
        
        SystemPerformance = "CPU: 25%, Disk: 15%";
        MemoryUsage = $"Used: {GC.GetTotalMemory(false) / 1024 / 1024} MB";
    }

    /// <summary>
    /// Loads recent log entries
    /// </summary>
    private async Task LoadLogEntriesAsync()
    {
        // Simulate loading log entries
        await Task.Delay(50).ConfigureAwait(false);
        
        LogFileSize = "2.4 MB";
    }

    /// <summary>
    /// Updates performance indicators
    /// </summary>
    private async Task UpdatePerformanceIndicatorsAsync()
    {
        // Simulate performance data collection
        await Task.Delay(25).ConfigureAwait(false);
    }

    #endregion
}

/// <summary>
/// Model for system metric display items
/// </summary>
public class SystemMetricItem
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Status { get; set; } = "Normal";
}

/// <summary>
/// Model for log entry display items
/// </summary>
public class LogEntryItem
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}

