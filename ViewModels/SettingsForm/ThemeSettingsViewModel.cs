using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Styling;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm
{
    /// <summary>
    /// ViewModel for Theme V2 settings management
    /// Demonstrates proper usage of ThemeServiceV2 with MVVM Community Toolkit patterns
    /// Follows MTM architectural patterns with centralized error handling
    /// </summary>
    public partial class ThemeSettingsViewModel : BaseViewModel
    {
        private readonly IThemeServiceV2 _themeService;

        [ObservableProperty]
        private string selectedTheme = "System";

        [ObservableProperty]
        private bool isSystemThemeEnabled = true;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string currentUserId = string.Empty;

        /// <summary>
        /// Available theme options for user selection
        /// </summary>
        public ObservableCollection<string> AvailableThemes { get; }

        /// <summary>
        /// Current effective theme name for display
        /// </summary>
        public string CurrentThemeDisplay => _themeService.CurrentThemeName;

        public ThemeSettingsViewModel(
            ILogger<ThemeSettingsViewModel> logger,
            IThemeServiceV2 themeService)
            : base(logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(themeService);

            _themeService = themeService;

            // Initialize available themes
            AvailableThemes = new ObservableCollection<string>(_themeService.AvailableThemes);

            // Subscribe to theme changes
            _themeService.ThemeChanged += OnThemeChanged;

            // Initialize current values
            SelectedTheme = _themeService.CurrentThemeName;
            IsSystemThemeEnabled = _themeService.IsSystemThemeEnabled;
        }

        /// <summary>
        /// Applies the selected theme
        /// </summary>
        [RelayCommand]
        private async Task ApplyThemeAsync()
        {
            IsLoading = true;
            try
            {
                Logger.LogInformation("Applying theme: {ThemeName}", SelectedTheme);

                await _themeService.ApplyThemeAsync(SelectedTheme);

                Logger.LogInformation("Theme applied successfully: {ThemeName}", SelectedTheme);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply theme", Environment.UserName);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Loads user's saved theme preference
        /// </summary>
        [RelayCommand]
        private async Task LoadUserThemeAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId))
            {
                Logger.LogWarning("Cannot load theme preference: User ID is not set");
                return;
            }

            IsLoading = true;
            try
            {
                Logger.LogInformation("Loading theme preference for user: {UserId}", CurrentUserId);

                await _themeService.LoadUserThemePreferenceAsync(CurrentUserId);

                // Update UI to reflect loaded preference
                SelectedTheme = _themeService.CurrentThemeName;
                IsSystemThemeEnabled = _themeService.IsSystemThemeEnabled;

                Logger.LogInformation("Theme preference loaded successfully for user: {UserId}", CurrentUserId);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load user theme preference", Environment.UserName);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Saves current theme preference for user
        /// </summary>
        [RelayCommand]
        private async Task SaveUserThemeAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentUserId))
            {
                Logger.LogWarning("Cannot save theme preference: User ID is not set");
                return;
            }

            IsLoading = true;
            try
            {
                Logger.LogInformation("Saving theme preference for user: {UserId}", CurrentUserId);

                await _themeService.SaveUserThemePreferenceAsync(CurrentUserId);

                Logger.LogInformation("Theme preference saved successfully for user: {UserId}", CurrentUserId);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to save user theme preference", Environment.UserName);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Toggles system theme following
        /// </summary>
        [RelayCommand]
        private async Task ToggleSystemThemeAsync()
        {
            IsLoading = true;
            try
            {
                Logger.LogInformation("Toggling system theme following: {Enabled}", !IsSystemThemeEnabled);

                await _themeService.SetSystemThemeFollowingAsync(!IsSystemThemeEnabled);

                // Update UI state
                IsSystemThemeEnabled = _themeService.IsSystemThemeEnabled;
                SelectedTheme = _themeService.CurrentThemeName;

                Logger.LogInformation("System theme following updated: {Enabled}", IsSystemThemeEnabled);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to toggle system theme following", Environment.UserName);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Refreshes theme from system settings
        /// </summary>
        [RelayCommand]
        private async Task RefreshSystemThemeAsync()
        {
            IsLoading = true;
            try
            {
                Logger.LogInformation("Refreshing system theme");

                await _themeService.RefreshSystemThemeAsync();

                // Update UI to reflect refreshed theme
                SelectedTheme = _themeService.CurrentThemeName;

                Logger.LogInformation("System theme refreshed successfully");
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to refresh system theme", Environment.UserName);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Sets the current user for theme preferences
        /// </summary>
        [RelayCommand]
        private void SetCurrentUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                Logger.LogWarning("Cannot set empty user ID");
                return;
            }

            CurrentUserId = userId;
            Logger.LogInformation("Current user set for theme preferences: {UserId}", userId);
        }

        /// <summary>
        /// Handles theme change notifications from service
        /// </summary>
        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            Logger.LogInformation("Theme changed: {NewTheme} (System triggered: {IsSystemTriggered})",
                e.ThemeName, e.IsSystemTriggered);

            // Update UI to reflect theme change
            SelectedTheme = e.ThemeName;
            IsSystemThemeEnabled = _themeService.IsSystemThemeEnabled;

            // Notify property changes for UI binding
            OnPropertyChanged(nameof(CurrentThemeDisplay));
        }

        /// <summary>
        /// Gets current theme variant information for display
        /// </summary>
        [RelayCommand]
        private void GetThemeInfo()
        {
            var currentTheme = _themeService.CurrentTheme;
            var resolvedTheme = _themeService.GetResolvedThemeVariant();
            var isSystemEnabled = _themeService.IsSystemThemeEnabled;

            Logger.LogInformation(
                "Theme Info - Current: {Current}, Resolved: {Resolved}, System Enabled: {SystemEnabled}",
                currentTheme, resolvedTheme, isSystemEnabled);
        }

        /// <summary>
        /// Cleanup subscriptions when ViewModel is disposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
            }

            base.Dispose(disposing);
        }
    }
}
