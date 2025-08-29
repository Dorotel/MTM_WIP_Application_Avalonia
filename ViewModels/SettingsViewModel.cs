using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// Settings management ViewModel following standard .NET patterns.
/// Provides unified interface for theme and application settings.
/// </summary>
public class SettingsViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    private readonly ISettingsService _settingsService;
    private bool _isLoading;
    private string _statusMessage = string.Empty;
    private ThemeInfo? _selectedTheme;

    public SettingsViewModel(
        IThemeService themeService,
        ISettingsService settingsService,
        ILogger<SettingsViewModel> logger) : base(logger)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        
        // Initialize commands
        ApplyThemeCommand = new AsyncCommand(ExecuteApplyThemeAsync, CanExecuteApplyTheme);
        ResetSettingsCommand = new AsyncCommand(ExecuteResetSettingsAsync);
        SaveSettingsCommand = new AsyncCommand(ExecuteSaveSettingsAsync);
        LoadSettingsCommand = new AsyncCommand(ExecuteLoadSettingsAsync);
        
        // Initialize collections
        AvailableThemes = new ObservableCollection<ThemeInfo>(_themeService.AvailableThemes);
        
        // Set current theme selection
        var currentTheme = AvailableThemes.FirstOrDefault(t => t.Id == _themeService.CurrentTheme);
        if (currentTheme != null)
        {
            SelectedTheme = currentTheme;
        }
        
        // Subscribe to service events
        _themeService.ThemeChanged += OnThemeServiceThemeChanged;
        _settingsService.SettingsChanged += OnSettingsServiceSettingsChanged;
        
        Logger.LogInformation("SettingsViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// Available themes for selection.
    /// </summary>
    public ObservableCollection<ThemeInfo> AvailableThemes { get; }

    /// <summary>
    /// Currently selected theme.
    /// </summary>
    public ThemeInfo? SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    /// <summary>
    /// Indicates if settings operations are in progress.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Status message for user feedback.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Auto-save settings option.
    /// </summary>
    public bool AutoSaveSettings
    {
        get => _settingsService.AutoSaveSettings;
        set
        {
            _settingsService.AutoSaveSettings = value;
            RaisePropertyChanged(nameof(AutoSaveSettings));
        }
    }

    /// <summary>
    /// Enable advanced features option.
    /// </summary>
    public bool EnableAdvancedFeatures
    {
        get => _settingsService.EnableAdvancedFeatures;
        set
        {
            _settingsService.EnableAdvancedFeatures = value;
            RaisePropertyChanged(nameof(EnableAdvancedFeatures));
        }
    }

    /// <summary>
    /// Default page size for data grids.
    /// </summary>
    public int DefaultPageSize
    {
        get => _settingsService.DefaultPageSize;
        set
        {
            _settingsService.DefaultPageSize = value;
            RaisePropertyChanged(nameof(DefaultPageSize));
        }
    }

    /// <summary>
    /// Enable real-time updates option.
    /// </summary>
    public bool EnableRealTimeUpdates
    {
        get => _settingsService.EnableRealTimeUpdates;
        set
        {
            _settingsService.EnableRealTimeUpdates = value;
            RaisePropertyChanged(nameof(EnableRealTimeUpdates));
        }
    }

    /// <summary>
    /// Remember window size option.
    /// </summary>
    public bool RememberWindowSize
    {
        get => _settingsService.RememberWindowSize;
        set
        {
            _settingsService.RememberWindowSize = value;
            RaisePropertyChanged(nameof(RememberWindowSize));
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to apply selected theme.
    /// </summary>
    public ICommand ApplyThemeCommand { get; }

    /// <summary>
    /// Command to reset all settings to defaults.
    /// </summary>
    public ICommand ResetSettingsCommand { get; }

    /// <summary>
    /// Command to save current settings.
    /// </summary>
    public ICommand SaveSettingsCommand { get; }

    /// <summary>
    /// Command to load settings from configuration.
    /// </summary>
    public ICommand LoadSettingsCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Applies the selected theme.
    /// </summary>
    private async Task ExecuteApplyThemeAsync()
    {
        if (SelectedTheme == null) return;

        try
        {
            IsLoading = true;
            StatusMessage = $"Applying theme: {SelectedTheme.DisplayName}...";

            var result = await _themeService.SetThemeAsync(SelectedTheme.Id);
            
            if (result.IsSuccess)
            {
                // Update settings service to match
                _settingsService.CurrentTheme = SelectedTheme.Id;
                
                StatusMessage = $"Theme applied: {SelectedTheme.DisplayName}";
                Logger.LogInformation("Theme applied successfully: {Theme}", SelectedTheme.DisplayName);
            }
            else
            {
                StatusMessage = $"Failed to apply theme: {result.Message}";
                Logger.LogWarning("Failed to apply theme {Theme}: {Message}", SelectedTheme.DisplayName, result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error applying theme: {ex.Message}";
            Logger.LogError(ex, "Error applying theme {Theme}", SelectedTheme?.DisplayName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Determines if theme can be applied.
    /// </summary>
    private bool CanExecuteApplyTheme()
    {
        return SelectedTheme != null && !IsLoading;
    }

    /// <summary>
    /// Resets all settings to default values.
    /// </summary>
    private async Task ExecuteResetSettingsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Resetting settings to defaults...";

            var result = await _settingsService.ResetToDefaultsAsync();
            
            if (result.IsSuccess)
            {
                // Reset theme selection to match settings
                var defaultTheme = AvailableThemes.FirstOrDefault(t => t.Id == _settingsService.CurrentTheme);
                if (defaultTheme != null)
                {
                    SelectedTheme = defaultTheme;
                }
                
                // Refresh all property bindings
                RefreshAllProperties();
                
                StatusMessage = "Settings reset to defaults";
                Logger.LogInformation("Settings reset to defaults successfully");
            }
            else
            {
                StatusMessage = $"Failed to reset settings: {result.Message}";
                Logger.LogWarning("Failed to reset settings: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error resetting settings: {ex.Message}";
            Logger.LogError(ex, "Error resetting settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Saves current settings.
    /// </summary>
    private async Task ExecuteSaveSettingsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Saving settings...";

            var result = await _settingsService.SaveSettingsAsync();
            
            if (result.IsSuccess)
            {
                StatusMessage = "Settings saved successfully";
                Logger.LogInformation("Settings saved successfully");
            }
            else
            {
                StatusMessage = $"Failed to save settings: {result.Message}";
                Logger.LogWarning("Failed to save settings: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error saving settings: {ex.Message}";
            Logger.LogError(ex, "Error saving settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads settings from configuration.
    /// </summary>
    private async Task ExecuteLoadSettingsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading settings...";

            var result = await _settingsService.LoadSettingsAsync();
            
            if (result.IsSuccess)
            {
                // Refresh theme selection to match loaded settings
                var loadedTheme = AvailableThemes.FirstOrDefault(t => t.Id == _settingsService.CurrentTheme);
                if (loadedTheme != null)
                {
                    SelectedTheme = loadedTheme;
                }
                
                // Refresh all property bindings
                RefreshAllProperties();
                
                StatusMessage = "Settings loaded successfully";
                Logger.LogInformation("Settings loaded successfully");
            }
            else
            {
                StatusMessage = $"Failed to load settings: {result.Message}";
                Logger.LogWarning("Failed to load settings: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading settings: {ex.Message}";
            Logger.LogError(ex, "Error loading settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles theme service theme changed events.
    /// </summary>
    private void OnThemeServiceThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        // Update selected theme to match service
        var newTheme = AvailableThemes.FirstOrDefault(t => t.Id == e.NewTheme.Id);
        if (newTheme != null && newTheme != SelectedTheme)
        {
            SelectedTheme = newTheme;
        }
        
        StatusMessage = $"Theme changed to: {e.NewTheme.DisplayName}";
    }

    /// <summary>
    /// Handles settings service settings changed events.
    /// </summary>
    private void OnSettingsServiceSettingsChanged(object? sender, SettingsChangedEventArgs e)
    {
        // Refresh property that changed
        RaisePropertyChanged(e.SettingName);
        
        Logger.LogDebug("Settings property changed: {PropertyName}", e.SettingName);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Refreshes all property change notifications.
    /// </summary>
    private void RefreshAllProperties()
    {
        RaisePropertyChanged(nameof(AutoSaveSettings));
        RaisePropertyChanged(nameof(EnableAdvancedFeatures));
        RaisePropertyChanged(nameof(DefaultPageSize));
        RaisePropertyChanged(nameof(EnableRealTimeUpdates));
        RaisePropertyChanged(nameof(RememberWindowSize));
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Dispose resources and unsubscribe from events.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _themeService.ThemeChanged -= OnThemeServiceThemeChanged;
            _settingsService.SettingsChanged -= OnSettingsServiceSettingsChanged;
        }
        
        base.Dispose(disposing);
    }

    #endregion
}