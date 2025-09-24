using Avalonia.Styling;
using System;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Services.Interfaces
{
    /// <summary>
    /// Interface for Theme V2 service providing comprehensive theme management
    /// Supports Avalonia 11.3.4 ThemeVariant system with Light/Dark/System themes
    /// Integrates with MTM database persistence and user preferences
    /// </summary>
    public interface IThemeServiceV2
    {
        /// <summary>
        /// Event raised when theme changes occur
        /// </summary>
        event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        /// <summary>
        /// Gets the current theme variant (Light, Dark, or Default for system)
        /// </summary>
        ThemeVariant CurrentTheme { get; }

        /// <summary>
        /// Gets the current theme name as string
        /// Used for UI display and database persistence
        /// </summary>
        string CurrentThemeName { get; }

        /// <summary>
        /// Gets whether system theme detection is enabled
        /// When true, follows OS theme preference
        /// </summary>
        bool IsSystemThemeEnabled { get; }

        /// <summary>
        /// Gets available theme variants
        /// Returns standard Light, Dark, and System options
        /// </summary>
        string[] AvailableThemes { get; }

        /// <summary>
        /// Applies the specified theme variant
        /// Updates Avalonia Application.RequestedThemeVariant and persists to database
        /// </summary>
        /// <param name="themeVariant">Theme to apply (Light, Dark, or Default for system)</param>
        /// <returns>Task representing the async operation</returns>
        Task ApplyThemeAsync(ThemeVariant themeVariant);

        /// <summary>
        /// Applies theme by string name
        /// Convenience method for UI binding and configuration
        /// </summary>
        /// <param name="themeName">Theme name ("Light", "Dark", or "System")</param>
        /// <returns>Task representing the async operation</returns>
        Task ApplyThemeAsync(string themeName);

        /// <summary>
        /// Loads user's saved theme preference from database
        /// Called during application startup
        /// </summary>
        /// <param name="userId">User identifier for preference lookup</param>
        /// <returns>Task representing the async operation</returns>
        Task LoadUserThemePreferenceAsync(string userId);

        /// <summary>
        /// Saves current theme preference to database
        /// Persists user's theme choice for next session
        /// </summary>
        /// <param name="userId">User identifier for preference storage</param>
        /// <returns>Task representing the async operation</returns>
        Task SaveUserThemePreferenceAsync(string userId);

        /// <summary>
        /// Enables or disables automatic system theme following
        /// When enabled, theme changes with OS preference
        /// </summary>
        /// <param name="enabled">True to follow system theme, false for manual control</param>
        /// <returns>Task representing the async operation</returns>
        Task SetSystemThemeFollowingAsync(bool enabled);

        /// <summary>
        /// Gets the effective theme variant that would be applied
        /// Resolves system theme preference to actual Light/Dark variant
        /// </summary>
        /// <returns>Resolved ThemeVariant (Light or Dark, never Default)</returns>
        ThemeVariant GetResolvedThemeVariant();

        /// <summary>
        /// Initializes the theme service
        /// Sets up system theme monitoring and applies initial theme
        /// </summary>
        /// <returns>Task representing the async initialization</returns>
        Task InitializeAsync();

        /// <summary>
        /// Refreshes theme from current system settings
        /// Updates theme if system theme following is enabled
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        Task RefreshSystemThemeAsync();

        /// <summary>
        /// Initializes the theme system with comprehensive setup
        /// Legacy method for compatibility with old theme system references
        /// Delegates to InitializeAsync()
        /// </summary>
        /// <returns>Task representing the async initialization</returns>
        Task InitializeThemeSystemAsync();

        /// <summary>
        /// Applies custom colors to the current theme
        /// Legacy method for compatibility with old theme system references
        /// No-op in V2 system as custom colors are not supported
        /// </summary>
        /// <param name="colors">Color dictionary (ignored in V2 system)</param>
        /// <returns>Completed task</returns>
        Task ApplyCustomColorsAsync(System.Collections.Generic.Dictionary<string, object> colors);
    }

    /// <summary>
    /// Event arguments for theme change notifications
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new theme variant that was applied
        /// </summary>
        public ThemeVariant NewTheme { get; }

        /// <summary>
        /// The previous theme variant
        /// </summary>
        public ThemeVariant PreviousTheme { get; }

        /// <summary>
        /// The theme name as displayed to users
        /// </summary>
        public string ThemeName { get; }

        /// <summary>
        /// Whether the theme change was triggered by system theme change
        /// </summary>
        public bool IsSystemTriggered { get; }

        public ThemeChangedEventArgs(ThemeVariant newTheme, ThemeVariant previousTheme, string themeName, bool isSystemTriggered = false)
        {
            NewTheme = newTheme;
            PreviousTheme = previousTheme;
            ThemeName = themeName;
            IsSystemTriggered = isSystemTriggered;
        }
    }
}
