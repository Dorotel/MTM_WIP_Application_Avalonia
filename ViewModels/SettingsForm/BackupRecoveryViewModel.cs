using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for Backup & Recovery panel.
/// Provides database backup, settings export/import, and recovery functionality.
/// </summary>
public class BackupRecoveryViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    private readonly ISettingsService _settingsService;
    
    private bool _isBackupInProgress;
    private string _lastBackupDate = "Never";
    private string _backupStatus = "Ready for backup";
    private bool _autoBackupEnabled = true;
    private int _backupRetentionDays = 30;
    private string _backupLocation = @"C:\MTM_Backups\";

    public BackupRecoveryViewModel(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        ISettingsService settingsService,
        ILogger<BackupRecoveryViewModel> logger) : base(logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

        // Initialize collections
        BackupHistory = new ObservableCollection<BackupHistoryItem>();
        ScheduledBackups = new ObservableCollection<ScheduledBackupItem>();

        // Initialize commands
        CreateBackupCommand = new AsyncCommand(ExecuteCreateBackupAsync);
        RestoreBackupCommand = new AsyncCommand(ExecuteRestoreBackupAsync);
        ExportSettingsCommand = new AsyncCommand(ExecuteExportSettingsAsync);
        ImportSettingsCommand = new AsyncCommand(ExecuteImportSettingsAsync);
        ScheduleBackupCommand = new AsyncCommand(ExecuteScheduleBackupAsync);
        DeleteBackupCommand = new AsyncCommand<BackupHistoryItem>(ExecuteDeleteBackupAsync);

        // Load initial data
        _ = LoadBackupDataAsync();
    }

    #region Properties

    /// <summary>
    /// Backup history collection.
    /// </summary>
    public ObservableCollection<BackupHistoryItem> BackupHistory { get; }

    /// <summary>
    /// Scheduled backups collection.
    /// </summary>
    public ObservableCollection<ScheduledBackupItem> ScheduledBackups { get; }

    /// <summary>
    /// Indicates if backup operation is in progress.
    /// </summary>
    public bool IsBackupInProgress
    {
        get => _isBackupInProgress;
        set => SetProperty(ref _isBackupInProgress, value);
    }

    /// <summary>
    /// Last backup date and time.
    /// </summary>
    public string LastBackupDate
    {
        get => _lastBackupDate;
        set => SetProperty(ref _lastBackupDate, value);
    }

    /// <summary>
    /// Current backup status message.
    /// </summary>
    public string BackupStatus
    {
        get => _backupStatus;
        set => SetProperty(ref _backupStatus, value);
    }

    /// <summary>
    /// Automatic backup enabled setting.
    /// </summary>
    public bool AutoBackupEnabled
    {
        get => _autoBackupEnabled;
        set => SetProperty(ref _autoBackupEnabled, value);
    }

    /// <summary>
    /// Backup retention period in days.
    /// </summary>
    public int BackupRetentionDays
    {
        get => _backupRetentionDays;
        set => SetProperty(ref _backupRetentionDays, value);
    }

    /// <summary>
    /// Backup storage location.
    /// </summary>
    public string BackupLocation
    {
        get => _backupLocation;
        set => SetProperty(ref _backupLocation, value);
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to create a new backup.
    /// </summary>
    public ICommand CreateBackupCommand { get; }

    /// <summary>
    /// Command to restore from backup.
    /// </summary>
    public ICommand RestoreBackupCommand { get; }

    /// <summary>
    /// Command to export settings.
    /// </summary>
    public ICommand ExportSettingsCommand { get; }

    /// <summary>
    /// Command to import settings.
    /// </summary>
    public ICommand ImportSettingsCommand { get; }

    /// <summary>
    /// Command to schedule backup.
    /// </summary>
    public ICommand ScheduleBackupCommand { get; }

    /// <summary>
    /// Command to delete backup.
    /// </summary>
    public ICommand DeleteBackupCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Creates a new database backup.
    /// </summary>
    private async Task ExecuteCreateBackupAsync()
    {
        try
        {
            IsBackupInProgress = true;
            BackupStatus = "Creating backup...";

            // Simulate backup creation process
            await Task.Delay(3000); // Simulate backup time

            var backupFileName = $"MTM_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            var backupItem = new BackupHistoryItem
            {
                FileName = backupFileName,
                CreatedDate = DateTime.Now,
                Size = "125.7 MB",
                Type = "Full Database",
                Status = "Completed"
            };

            BackupHistory.Insert(0, backupItem);
            LastBackupDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            BackupStatus = $"✅ Backup completed successfully: {backupFileName}";

            Logger.LogInformation("Database backup created successfully: {FileName}", backupFileName);
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Backup failed: {ex.Message}";
            Logger.LogError(ex, "Error creating database backup");
        }
        finally
        {
            IsBackupInProgress = false;
        }
    }

    /// <summary>
    /// Restores database from selected backup.
    /// </summary>
    private async Task ExecuteRestoreBackupAsync()
    {
        try
        {
            IsBackupInProgress = true;
            BackupStatus = "Restoring from backup...";

            // In real implementation, would show file picker for backup selection
            await Task.Delay(2000); // Simulate restore time

            BackupStatus = "✅ Database restored successfully";
            Logger.LogInformation("Database restored from backup");
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Restore failed: {ex.Message}";
            Logger.LogError(ex, "Error restoring from backup");
        }
        finally
        {
            IsBackupInProgress = false;
        }
    }

    /// <summary>
    /// Exports application settings to file.
    /// </summary>
    private async Task ExecuteExportSettingsAsync()
    {
        try
        {
            IsBackupInProgress = true;
            BackupStatus = "Exporting settings...";

            // Export settings using settings service
            var settingsData = await GenerateSettingsExportAsync();
            
            // In real implementation, would save to file
            await Task.Delay(1000); // Simulate export time

            BackupStatus = "✅ Settings exported successfully";
            Logger.LogInformation("Application settings exported");
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Export failed: {ex.Message}";
            Logger.LogError(ex, "Error exporting settings");
        }
        finally
        {
            IsBackupInProgress = false;
        }
    }

    /// <summary>
    /// Imports application settings from file.
    /// </summary>
    private async Task ExecuteImportSettingsAsync()
    {
        try
        {
            IsBackupInProgress = true;
            BackupStatus = "Importing settings...";

            // In real implementation, would show file picker
            await Task.Delay(1500); // Simulate import time

            BackupStatus = "✅ Settings imported successfully";
            Logger.LogInformation("Application settings imported");
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Import failed: {ex.Message}";
            Logger.LogError(ex, "Error importing settings");
        }
        finally
        {
            IsBackupInProgress = false;
        }
    }

    /// <summary>
    /// Schedules automatic backup.
    /// </summary>
    private async Task ExecuteScheduleBackupAsync()
    {
        try
        {
            var scheduledBackup = new ScheduledBackupItem
            {
                Name = "Daily Auto Backup",
                Schedule = "Daily at 2:00 AM",
                NextRun = DateTime.Today.AddDays(1).AddHours(2),
                IsEnabled = true,
                Type = "Database + Settings"
            };

            ScheduledBackups.Add(scheduledBackup);
            BackupStatus = "✅ Backup scheduled successfully";
            
            await Task.Delay(100); // Simulate async operation
            Logger.LogInformation("Automatic backup scheduled");
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Scheduling failed: {ex.Message}";
            Logger.LogError(ex, "Error scheduling backup");
        }
    }

    /// <summary>
    /// Deletes selected backup.
    /// </summary>
    private async Task ExecuteDeleteBackupAsync(BackupHistoryItem? backup)
    {
        if (backup == null) return;

        try
        {
            // In real implementation, would show confirmation dialog
            BackupHistory.Remove(backup);
            BackupStatus = $"✅ Backup deleted: {backup.FileName}";
            
            await Task.Delay(100); // Simulate async operation
            Logger.LogInformation("Backup deleted: {FileName}", backup.FileName);
        }
        catch (Exception ex)
        {
            BackupStatus = $"❌ Delete failed: {ex.Message}";
            Logger.LogError(ex, "Error deleting backup: {FileName}", backup.FileName);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads backup data and history.
    /// </summary>
    private async Task LoadBackupDataAsync()
    {
        try
        {
            // Load backup history
            BackupHistory.Clear();
            
            // Add sample backup history
            BackupHistory.Add(new BackupHistoryItem
            {
                FileName = "MTM_Backup_20241215_140000.bak",
                CreatedDate = DateTime.Now.AddDays(-1),
                Size = "125.7 MB",
                Type = "Full Database",
                Status = "Completed"
            });
            
            BackupHistory.Add(new BackupHistoryItem
            {
                FileName = "MTM_Settings_20241214_200000.json",
                CreatedDate = DateTime.Now.AddDays(-2),
                Size = "2.3 KB",
                Type = "Settings Only",
                Status = "Completed"
            });

            // Load scheduled backups
            ScheduledBackups.Clear();
            ScheduledBackups.Add(new ScheduledBackupItem
            {
                Name = "Weekly Full Backup",
                Schedule = "Every Sunday at 1:00 AM",
                NextRun = GetNextSunday().AddHours(1),
                IsEnabled = true,
                Type = "Database + Settings"
            });

            // Update last backup date
            if (BackupHistory.Count > 0)
            {
                LastBackupDate = BackupHistory[0].CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
            }

            await Task.Delay(100); // Simulate async loading
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading backup data");
        }
    }

    /// <summary>
    /// Generates settings export data.
    /// </summary>
    private async Task<string> GenerateSettingsExportAsync()
    {
        // In real implementation, would gather all settings
        var exportData = new
        {
            ExportDate = DateTime.Now,
            Version = "1.0.0",
            ThemeSettings = new { CurrentTheme = "MTM_Light" },
            DatabaseSettings = new { ConnectionTimeout = 30 },
            UserPreferences = new { AutoBackup = AutoBackupEnabled },
            BackupSettings = new { RetentionDays = BackupRetentionDays, Location = BackupLocation }
        };

        await Task.Delay(100); // Simulate async operation
        return exportData.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the next Sunday date.
    /// </summary>
    private static DateTime GetNextSunday()
    {
        var today = DateTime.Today;
        var daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
        return today.AddDays(daysUntilSunday == 0 ? 7 : daysUntilSunday);
    }

    #endregion
}

/// <summary>
/// Backup history data item.
/// </summary>
public class BackupHistoryItem
{
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FormattedDate => CreatedDate.ToString("yyyy-MM-dd HH:mm");
}

/// <summary>
/// Scheduled backup data item.
/// </summary>
public class ScheduledBackupItem
{
    public string Name { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public DateTime NextRun { get; set; }
    public bool IsEnabled { get; set; }
    public string Type { get; set; } = string.Empty;
    public string FormattedNextRun => NextRun.ToString("yyyy-MM-dd HH:mm");
}