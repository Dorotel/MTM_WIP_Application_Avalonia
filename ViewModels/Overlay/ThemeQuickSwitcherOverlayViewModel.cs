using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.UI;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the theme quick switcher overlay that provides rapid theme changes
/// with live preview functionality. Integrates with the existing IThemeService to offer
/// comprehensive theme management and user preference persistence.
/// Follows BaseOverlayViewModel pattern and MTM design guidelines.
/// </summary>
public partial class ThemeQuickSwitcherOverlayViewModel : BaseOverlayViewModel
{
    private readonly IThemeService _themeService;
    private ThemeInfo? _originalTheme;
    private bool _isPreviewMode;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the collection of available themes for switching.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ThemeInfo> availableThemes = new();

    /// <summary>
    /// Gets or sets the currently selected theme for preview or application.
    /// </summary>
    [ObservableProperty]
    private ThemeInfo? selectedTheme;

    /// <summary>
    /// Gets or sets whether theme preview mode is active.
    /// </summary>
    [ObservableProperty]
    private bool isPreviewMode;

    /// <summary>
    /// Gets or sets the current preview theme being displayed.
    /// </summary>
    [ObservableProperty]
    private ThemeInfo? previewTheme;

    /// <summary>
    /// Gets or sets whether the theme is currently being applied.
    /// </summary>
    [ObservableProperty]
    private bool isApplyingTheme;

    /// <summary>
    /// Gets or sets the current status message for theme operations.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = "Select a theme to preview or apply";

    /// <summary>
    /// Gets or sets whether to show only light themes.
    /// </summary>
    [ObservableProperty]
    private bool showLightThemesOnly;

    /// <summary>
    /// Gets or sets whether to show only dark themes.
    /// </summary>
    [ObservableProperty]
    private bool showDarkThemesOnly;

    /// <summary>
    /// Gets or sets whether to show all themes (default).
    /// </summary>
    [ObservableProperty]
    private bool showAllThemes = true;

    /// <summary>
    /// Gets or sets the search text for filtering themes.
    /// </summary>
    [ObservableProperty]
    private string themeSearchText = string.Empty;

    /// <summary>
    /// Gets or sets whether live preview is enabled.
    /// </summary>
    [ObservableProperty]
    private bool isLivePreviewEnabled = true;

    /// <summary>
    /// Gets or sets whether to save theme preference automatically.
    /// </summary>
    [ObservableProperty]
    private bool autoSavePreference = true;

    /// <summary>
    /// Gets the filtered themes collection based on current filters.
    /// </summary>
    public ObservableCollection<ThemeInfo> FilteredThemes { get; } = new();

    /// <summary>
    /// Gets the current theme's display name.
    /// </summary>
    public string CurrentThemeName => _themeService?.CurrentTheme ?? "Unknown";

    /// <summary>
    /// Gets whether the current theme is dark.
    /// </summary>
    public bool IsCurrentThemeDark => _themeService?.IsDarkTheme ?? false;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the ThemeQuickSwitcherOverlayViewModel class.
    /// </summary>
    /// <param name="logger">Logger instance for theme switcher operations.</param>
    /// <param name="themeService">Theme service for theme management operations.</param>
    public ThemeQuickSwitcherOverlayViewModel(
        ILogger<ThemeQuickSwitcherOverlayViewModel> logger,
        IThemeService themeService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(themeService);

        _themeService = themeService;
        Logger.LogDebug("ThemeQuickSwitcherOverlayViewModel initialized");

        // Subscribe to theme service changes
        _themeService.PropertyChanged += OnThemeServicePropertyChanged;
        _themeService.ThemeChanged += OnThemeServiceThemeChanged;
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to preview a specific theme without applying it permanently.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPreviewTheme))]
    private async Task PreviewThemeAsync(ThemeInfo? theme)
    {
        if (theme == null) return;

        try
        {
            Logger.LogInformation("Previewing theme: {ThemeName}", theme.DisplayName);

            IsApplyingTheme = true;
            StatusMessage = $"Previewing {theme.DisplayName}...";

            // Store original theme if this is the first preview
            if (!_isPreviewMode && _originalTheme == null)
            {
                _originalTheme = _themeService.AvailableThemes.FirstOrDefault(t => t.Id == _themeService.CurrentTheme);
            }

            _isPreviewMode = true;
            IsPreviewMode = true;
            PreviewTheme = theme;
            SelectedTheme = theme;

            // Apply theme temporarily
            var result = await _themeService.SetThemeAsync(theme.Id);
            if (result.IsSuccess)
            {
                StatusMessage = $"Previewing: {theme.DisplayName}";
                Logger.LogInformation("Theme preview applied successfully: {ThemeName}", theme.DisplayName);
            }
            else
            {
                StatusMessage = $"Failed to preview theme: {result.Message}";
                Logger.LogWarning("Failed to preview theme {ThemeName}: {Error}", theme.DisplayName, result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Preview failed - see logs for details";
            await HandleErrorAsync(ex, "Failed to preview theme");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    /// <summary>
    /// Command to apply the currently selected theme permanently.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanApplyTheme))]
    private async Task ApplyThemeAsync()
    {
        if (SelectedTheme == null) return;

        try
        {
            Logger.LogInformation("Applying theme permanently: {ThemeName}", SelectedTheme.DisplayName);

            IsApplyingTheme = true;
            StatusMessage = $"Applying {SelectedTheme.DisplayName}...";

            // Apply the theme
            var result = await _themeService.SetThemeAsync(SelectedTheme.Id);
            if (!result.IsSuccess)
            {
                StatusMessage = $"Failed to apply theme: {result.Message}";
                Logger.LogError("Failed to apply theme {ThemeName}: {Error}", SelectedTheme.DisplayName, result.Message);
                return;
            }

            // Save theme preference if enabled
            if (AutoSavePreference)
            {
                var saveResult = await _themeService.SaveUserPreferredThemeAsync(SelectedTheme.Id);
                if (saveResult.IsSuccess)
                {
                    Logger.LogInformation("Theme preference saved: {ThemeName}", SelectedTheme.DisplayName);
                }
                else
                {
                    Logger.LogWarning("Failed to save theme preference: {Error}", saveResult.Message);
                }
            }

            // Clear preview mode
            _isPreviewMode = false;
            IsPreviewMode = false;
            _originalTheme = null;
            PreviewTheme = null;

            StatusMessage = $"Applied: {SelectedTheme.DisplayName}";

            // Auto-close overlay after successful application
            await Task.Delay(1500);
            await HideAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = "Apply failed - see logs for details";
            await HandleErrorAsync(ex, "Failed to apply theme");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    /// <summary>
    /// Command to cancel preview mode and restore the original theme.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCancelPreview))]
    private async Task CancelPreviewAsync()
    {
        try
        {
            Logger.LogInformation("Cancelling theme preview and restoring original theme");

            if (_originalTheme != null)
            {
                IsApplyingTheme = true;
                StatusMessage = $"Restoring {_originalTheme.DisplayName}...";

                var result = await _themeService.SetThemeAsync(_originalTheme.Id);
                if (result.IsSuccess)
                {
                    StatusMessage = $"Restored: {_originalTheme.DisplayName}";
                    Logger.LogInformation("Original theme restored: {ThemeName}", _originalTheme.DisplayName);
                }
                else
                {
                    StatusMessage = "Failed to restore original theme";
                    Logger.LogWarning("Failed to restore original theme: {Error}", result.Message);
                }
            }

            // Clear preview state
            _isPreviewMode = false;
            IsPreviewMode = false;
            _originalTheme = null;
            PreviewTheme = null;
            SelectedTheme = _themeService.AvailableThemes.FirstOrDefault(t => t.Id == _themeService.CurrentTheme);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to cancel theme preview");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    /// <summary>
    /// Command to toggle between light and dark theme variants.
    /// </summary>
    [RelayCommand]
    private async Task ToggleThemeVariantAsync()
    {
        try
        {
            Logger.LogInformation("Toggling theme variant (light/dark)");

            IsApplyingTheme = true;
            StatusMessage = "Switching theme variant...";

            var result = await _themeService.ToggleVariantAsync();
            if (result.IsSuccess)
            {
                StatusMessage = result.Message;
                await RefreshThemeCollections();
                Logger.LogInformation("Theme variant toggled successfully");
            }
            else
            {
                StatusMessage = $"Toggle failed: {result.Message}";
                Logger.LogWarning("Failed to toggle theme variant: {Error}", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Variant toggle failed - see logs for details";
            await HandleErrorAsync(ex, "Failed to toggle theme variant");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    /// <summary>
    /// Command to reset to the default MTM theme.
    /// </summary>
    [RelayCommand]
    private async Task ResetToDefaultThemeAsync()
    {
        try
        {
            Logger.LogInformation("Resetting to default MTM theme");

            IsApplyingTheme = true;
            StatusMessage = "Resetting to default theme...";

            const string defaultThemeId = "MTMTheme";
            var result = await _themeService.SetThemeAsync(defaultThemeId);

            if (result.IsSuccess)
            {
                // Save as preference if enabled
                if (AutoSavePreference)
                {
                    await _themeService.SaveUserPreferredThemeAsync(defaultThemeId);
                }

                StatusMessage = "Reset to MTM Default theme";
                await RefreshThemeCollections();

                Logger.LogInformation("Successfully reset to default theme");
            }
            else
            {
                StatusMessage = $"Reset failed: {result.Message}";
                Logger.LogWarning("Failed to reset to default theme: {Error}", result.Message);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Reset failed - see logs for details";
            await HandleErrorAsync(ex, "Failed to reset to default theme");
        }
        finally
        {
            IsApplyingTheme = false;
        }
    }

    /// <summary>
    /// Command to filter themes by type (all, light, dark).
    /// </summary>
    [RelayCommand]
    private void FilterThemes(string filterType)
    {
        try
        {
            Logger.LogDebug("Filtering themes by type: {FilterType}", filterType);

            ShowAllThemes = filterType == "all";
            ShowLightThemesOnly = filterType == "light";
            ShowDarkThemesOnly = filterType == "dark";

            ApplyThemeFilters();

            StatusMessage = $"Showing {FilteredThemes.Count} {filterType} themes";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error filtering themes by type: {FilterType}", filterType);
        }
    }

    /// <summary>
    /// Command to clear the theme search text.
    /// </summary>
    [RelayCommand]
    private void ClearSearch()
    {
        ThemeSearchText = string.Empty;
        ApplyThemeFilters();
        StatusMessage = $"Showing all {FilteredThemes.Count} themes";
    }

    #endregion

    #region Command CanExecute Methods

    /// <summary>
    /// Determines whether a theme can be previewed.
    /// </summary>
    private bool CanPreviewTheme(ThemeInfo? theme) =>
        theme != null && !IsApplyingTheme && IsLivePreviewEnabled;

    /// <summary>
    /// Determines whether the current theme can be applied.
    /// </summary>
    private bool CanApplyTheme() =>
        SelectedTheme != null && !IsApplyingTheme;

    /// <summary>
    /// Determines whether preview mode can be cancelled.
    /// </summary>
    private bool CanCancelPreview() =>
        IsPreviewMode && _originalTheme != null && !IsApplyingTheme;

    #endregion

    #region Protected Override Methods

    protected override string GetDefaultTitle() => "Quick Theme Switcher";

    protected override async Task OnInitializeAsync(object requestData)
    {
        await base.OnInitializeAsync(requestData);

        try
        {
            await LoadThemeCollections();

            // Set current theme as selected
            SelectedTheme = _themeService.AvailableThemes.FirstOrDefault(t => t.Id == _themeService.CurrentTheme);

            StatusMessage = "Ready to switch themes";
        }
        catch (Exception ex)
        {
            StatusMessage = "Failed to load themes - see logs for details";
            await HandleErrorAsync(ex, "Failed to initialize theme collections");
        }
    }

    protected override async Task OnShowingAsync()
    {
        await base.OnShowingAsync();

        // Refresh theme collections in case themes have been added
        await RefreshThemeCollections();

        // Reset preview state
        _isPreviewMode = false;
        IsPreviewMode = false;
        _originalTheme = null;
        PreviewTheme = null;
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        // Confirm means apply the selected theme
        if (SelectedTheme != null)
        {
            await ApplyThemeCommand.ExecuteAsync(null);
            return false; // Don't close overlay yet - let ApplyTheme handle it
        }

        return true; // Close overlay if no theme selected
    }

    protected override async Task OnCancelAsync()
    {
        // Cancel any active preview
        if (IsPreviewMode && _originalTheme != null)
        {
            await CancelPreviewCommand.ExecuteAsync(null);
        }

        await base.OnCancelAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads all available themes from the theme service.
    /// </summary>
    private async Task LoadThemeCollections()
    {
        try
        {
            Logger.LogDebug("Loading theme collections from theme service");

            AvailableThemes.Clear();
            FilteredThemes.Clear();

            var themes = _themeService.AvailableThemes;
            foreach (var theme in themes)
            {
                AvailableThemes.Add(theme);
            }

            ApplyThemeFilters();

            Logger.LogInformation("Loaded {ThemeCount} themes from theme service", themes.Count);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading theme collections");
            throw;
        }
    }

    /// <summary>
    /// Refreshes theme collections and updates current selection.
    /// </summary>
    private async Task RefreshThemeCollections()
    {
        try
        {
            await LoadThemeCollections();

            // Update selected theme to match current theme service state
            SelectedTheme = _themeService.AvailableThemes.FirstOrDefault(t => t.Id == _themeService.CurrentTheme);

            // Update computed properties
            OnPropertyChanged(nameof(CurrentThemeName));
            OnPropertyChanged(nameof(IsCurrentThemeDark));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing theme collections");
        }
    }

    /// <summary>
    /// Applies current filter settings to the theme collection.
    /// </summary>
    private void ApplyThemeFilters()
    {
        try
        {
            FilteredThemes.Clear();

            var filteredThemes = AvailableThemes.AsEnumerable();

            // Apply type filter
            if (ShowLightThemesOnly)
            {
                filteredThemes = filteredThemes.Where(t => !t.IsDark);
            }
            else if (ShowDarkThemesOnly)
            {
                filteredThemes = filteredThemes.Where(t => t.IsDark);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(ThemeSearchText))
            {
                var searchLower = ThemeSearchText.ToLowerInvariant();
                filteredThemes = filteredThemes.Where(t =>
                    t.DisplayName.ToLowerInvariant().Contains(searchLower) ||
                    t.Description.ToLowerInvariant().Contains(searchLower));
            }

            foreach (var theme in filteredThemes)
            {
                FilteredThemes.Add(theme);
            }

            Logger.LogDebug("Applied filters - showing {FilteredCount}/{TotalCount} themes",
                FilteredThemes.Count, AvailableThemes.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying theme filters");
        }
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles changes to theme search text.
    /// </summary>
    partial void OnThemeSearchTextChanged(string value)
    {
        ApplyThemeFilters();
    }

    /// <summary>
    /// Handles changes to live preview enabled setting.
    /// </summary>
    partial void OnIsLivePreviewEnabledChanged(bool value)
    {
        if (!value && IsPreviewMode)
        {
            // Cancel any active preview if live preview is disabled
            _ = CancelPreviewCommand.ExecuteAsync(null);
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles theme service property changes.
    /// </summary>
    private async void OnThemeServicePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(IThemeService.CurrentTheme) ||
                e.PropertyName == nameof(IThemeService.IsDarkTheme))
            {
                await RefreshThemeCollections();
                StatusMessage = $"Current: {CurrentThemeName}";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling theme service property change: {PropertyName}", e.PropertyName);
        }
    }

    /// <summary>
    /// Handles theme service theme changed events.
    /// </summary>
    private void OnThemeServiceThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        try
        {
            Logger.LogDebug("Theme changed from {Previous} to {New}",
                e.PreviousTheme, e.NewTheme);

            StatusMessage = $"Changed to: {e.NewTheme}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling theme changed event");
        }
    }

    #endregion

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from theme service events
            if (_themeService != null)
            {
                _themeService.PropertyChanged -= OnThemeServicePropertyChanged;
                _themeService.ThemeChanged -= OnThemeServiceThemeChanged;
            }

            // Cancel any active preview before disposing
            if (IsPreviewMode && _originalTheme != null)
            {
                try
                {
                    _ = Task.Run(async () => await _themeService.SetThemeAsync(_originalTheme.Id));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error restoring original theme during disposal");
                }
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}
