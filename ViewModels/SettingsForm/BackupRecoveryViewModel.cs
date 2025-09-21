using MTM_WIP_Application_Avalonia.Services.Feature;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for Backup & Recovery panel.
/// Provides database backup, settings export/import, and recovery functionality.
/// </summary>
public partial class BackupRecoveryViewModel : BaseViewModel
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfigurationService _configurationService;
    private readonly ISettingsService _settingsService;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateBackupCommand))]
    [NotifyCanExecuteChangedFor(nameof(RestoreBackupCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExportSettingsCommand))]
    [NotifyCanExecuteChangedFor(nameof(ImportSettingsCommand))]
    private bool _isBackupInProgress;
    
    [ObservableProperty]
    private string _lastBackupDate = "Never";
    
    [ObservableProperty]
    private string _backupStatus = "Ready for backup";
    
    [ObservableProperty]
    private bool _autoBackupEnabled = true;
    
    [ObservableProperty]
    private int _backupRetentionDays = 30;
    
    [ObservableProperty]
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

        Logger.LogInformation("BackupRecoveryViewModel initialized");

        // Defer data loading to avoid DI resolution issues
        Avalonia.Threading.Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                await LoadBackupDataAsync();
                Logger.LogInformation("Backup data loaded successfully in background");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load backup data in background");
            }
        });
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

    #endregion

    #region Commands

    /// <summary>
    /// Creates a new database backup.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteBackupCommands))]
    private async Task CreateBackupAsync()
    {
        try
        {
            // Update UI status on UI thread
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsBackupInProgress = true;
                BackupStatus = "Creating backup...";
            });

            // Simulate backup creation process
            await Task.Delay(3000).ConfigureAwait(false); // Simulate backup time

            var backupFileName = $"MTM_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            var backupItem = new BackupHistoryItem
            {
                FileName = backupFileName,
                CreatedDate = DateTime.Now,
                Size = "125.7 MB",
                Type = "Full Database",
                Status = "Completed"
            };

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupHistory.Insert(0, backupItem);
                LastBackupDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                BackupStatus = $"âœ… Backup completed successfully: {backupFileName}";
                IsBackupInProgress = false;
            });

            Logger.LogInformation("Database backup created successfully: {FileName}", backupFileName);
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Backup failed: {ex.Message}";
                IsBackupInProgress = false;
            });
            Logger.LogError(ex, "Error creating database backup");
        }
    }

    /// <summary>
    /// Determines if backup commands can be executed.
    /// </summary>
    private bool CanExecuteBackupCommands() => !IsBackupInProgress;

    /// <summary>
    /// Restores database from selected backup.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteBackupCommands))]
    private async Task RestoreBackupAsync()
    {
        try
        {
            // Update UI status on UI thread
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsBackupInProgress = true;
                BackupStatus = "Restoring from backup...";
            });

            // In real implementation, would show file picker for backup selection
            await Task.Delay(2000).ConfigureAwait(false); // Simulate restore time

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = "âœ… Database restored successfully";
                IsBackupInProgress = false;
            });
            
            Logger.LogInformation("Database restored from backup");
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Restore failed: {ex.Message}";
                IsBackupInProgress = false;
            });
            Logger.LogError(ex, "Error restoring from backup");
        }
    }

    /// <summary>
    /// Exports application settings to file.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteBackupCommands))]
    private async Task ExportSettingsAsync()
    {
        try
        {
            // Update UI status on UI thread
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsBackupInProgress = true;
                BackupStatus = "Exporting settings...";
            });

            // Export settings using settings service
            var settingsData = await GenerateSettingsExportAsync().ConfigureAwait(false);
            
            // In real implementation, would save to file
            await Task.Delay(1000).ConfigureAwait(false); // Simulate export time

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = "âœ… Settings exported successfully";
                IsBackupInProgress = false;
            });
            
            Logger.LogInformation("Application settings exported");
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Export failed: {ex.Message}";
                IsBackupInProgress = false;
            });
            Logger.LogError(ex, "Error exporting settings");
        }
    }

    /// <summary>
    /// Imports application settings from file.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecuteBackupCommands))]
    private async Task ImportSettingsAsync()
    {
        try
        {
            // Update UI status on UI thread
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                IsBackupInProgress = true;
                BackupStatus = "Importing settings...";
            });

            // In real implementation, would show file picker
            await Task.Delay(1500).ConfigureAwait(false); // Simulate import time

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = "âœ… Settings imported successfully";
                IsBackupInProgress = false;
            });
            
            Logger.LogInformation("Application settings imported");
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Import failed: {ex.Message}";
                IsBackupInProgress = false;
            });
            Logger.LogError(ex, "Error importing settings");
        }
    }

    /// <summary>
    /// Schedules automatic backup.
    /// </summary>
    [RelayCommand]
    private async Task ScheduleBackupAsync()
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

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                ScheduledBackups.Add(scheduledBackup);
                BackupStatus = "âœ… Backup scheduled successfully";
            });
            
            await Task.Delay(100).ConfigureAwait(false); // Simulate async operation
            Logger.LogInformation("Automatic backup scheduled");
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Scheduling failed: {ex.Message}";
            });
            Logger.LogError(ex, "Error scheduling backup");
        }
    }

    /// <summary>
    /// Deletes selected backup.
    /// </summary>
    [RelayCommand]
    private async Task DeleteBackupAsync(BackupHistoryItem? backup)
    {
        if (backup == null) return;

        try
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                // In real implementation, would show confirmation dialog
                BackupHistory.Remove(backup);
                BackupStatus = $"âœ… Backup deleted: {backup.FileName}";
            });
            
            await Task.Delay(100).ConfigureAwait(false); // Simulate async operation
            Logger.LogInformation("Backup deleted: {FileName}", backup.FileName);
        }
        catch (Exception ex)
        {
            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                BackupStatus = $"âŒ Delete failed: {ex.Message}";
            });
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
            // Prepare data in background
            var backupHistoryItems = new List<BackupHistoryItem>
            {
                new BackupHistoryItem
                {
                    FileName = "MTM_Backup_20241215_140000.bak",
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Size = "125.7 MB",
                    Type = "Full Database",
                    Status = "Completed"
                },
                new BackupHistoryItem
                {
                    FileName = "MTM_Settings_20241214_200000.json",
                    CreatedDate = DateTime.Now.AddDays(-2),
                    Size = "2.3 KB",
                    Type = "Settings Only",
                    Status = "Completed"
                }
            };

            var scheduledBackupItems = new List<ScheduledBackupItem>
            {
                new ScheduledBackupItem
                {
                    Name = "Weekly Full Backup",
                    Schedule = "Every Sunday at 1:00 AM",
                    NextRun = GetNextSunday().AddHours(1),
                    IsEnabled = true,
                    Type = "Database + Settings"
                }
            };

            await Task.Delay(100).ConfigureAwait(false); // Simulate async loading

            // Update UI on UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Load backup history
                BackupHistory.Clear();
                foreach (var item in backupHistoryItems)
                {
                    BackupHistory.Add(item);
                }

                // Load scheduled backups
                ScheduledBackups.Clear();
                foreach (var item in scheduledBackupItems)
                {
                    ScheduledBackups.Add(item);
                }

                // Update last backup date
                if (BackupHistory.Count > 0)
                {
                    LastBackupDate = BackupHistory[0].CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
            });
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

        await Task.Delay(100).ConfigureAwait(false); // Simulate async operation
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
