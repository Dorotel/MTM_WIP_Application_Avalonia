using Avalonia;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// Theme V2 service implementation providing comprehensive theme management
    /// Integrates with Avalonia 11.3.4 ThemeVariant system and MTM database persistence
    /// Follows MTM architectural patterns with centralized error handling
    /// </summary>
    public class ThemeServiceV2 : IThemeServiceV2
    {
        private readonly ILogger<ThemeServiceV2> _logger;
        private readonly IConfigurationService _configurationService;

        private ThemeVariant _currentTheme = ThemeVariant.Default;
        private bool _isSystemThemeEnabled = true;
        private string _currentUserId = string.Empty;
        private bool _isInitialized = false;

        /// <summary>
        /// Available theme options for MTM application
        /// </summary>
        public string[] AvailableThemes { get; } = { "Light", "Dark", "System" };

        /// <summary>
        /// Event raised when theme changes occur
        /// </summary>
        public event EventHandler<MTM_WIP_Application_Avalonia.Services.Interfaces.ThemeChangedEventArgs>? ThemeChanged;

        /// <summary>
        /// Gets the current theme variant
        /// </summary>
        public ThemeVariant CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets the current theme name for UI display
        /// </summary>
        public string CurrentThemeName => GetThemeName(_currentTheme, _isSystemThemeEnabled);

        /// <summary>
        /// Gets whether system theme following is enabled
        /// </summary>
        public bool IsSystemThemeEnabled => _isSystemThemeEnabled;

        public ThemeServiceV2(
            ILogger<ThemeServiceV2> logger,
            IConfigurationService configurationService)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configurationService);

            _logger = logger;
            _configurationService = configurationService;
        }

        /// <summary>
        /// Initializes the theme service with system theme monitoring
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing Theme V2 service");

                // Set up system theme change monitoring
                if (Application.Current is not null)
                {
                    Application.Current.ActualThemeVariantChanged += OnSystemThemeChanged;
                }

                // Apply default theme (system following enabled by default)
                await ApplyThemeAsync(ThemeVariant.Default);

                _isInitialized = true;
                _logger.LogInformation("Theme V2 service initialized successfully");
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to initialize Theme V2 service", Environment.UserName);
                throw;
            }
        }

        /// <summary>
        /// Applies the specified theme variant
        /// </summary>
        public async Task ApplyThemeAsync(ThemeVariant themeVariant)
        {
            try
            {
                _logger.LogInformation("Applying theme variant: {ThemeVariant}", themeVariant);

                var previousTheme = _currentTheme;
                _currentTheme = themeVariant;

                // Determine system following state
                _isSystemThemeEnabled = themeVariant == ThemeVariant.Default;

                // Apply theme to Avalonia application
                if (Application.Current is not null)
                {
                    Application.Current.RequestedThemeVariant = themeVariant;
                    _logger.LogInformation("Applied theme variant to Avalonia application: {ThemeVariant}", themeVariant);
                }

                // Raise theme changed event
                var themeName = GetThemeName(themeVariant, _isSystemThemeEnabled);
                ThemeChanged?.Invoke(this, new MTM_WIP_Application_Avalonia.Services.Interfaces.ThemeChangedEventArgs(
                    themeVariant,
                    previousTheme,
                    themeName,
                    false));

                // Save to database if user is set
                if (!string.IsNullOrEmpty(_currentUserId))
                {
                    await SaveThemePreferenceToDatabase(_currentUserId, themeName);
                }

                _logger.LogInformation("Theme applied successfully: {ThemeName}", themeName);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to apply theme variant: {themeVariant}", Environment.UserName);
                throw;
            }
        }

        /// <summary>
        /// Applies theme by string name
        /// </summary>
        public async Task ApplyThemeAsync(string themeName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(themeName);

            var themeVariant = ParseThemeName(themeName);
            await ApplyThemeAsync(themeVariant);
        }

        /// <summary>
        /// Loads user's saved theme preference from database
        /// </summary>
        public async Task LoadUserThemePreferenceAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            try
            {
                _logger.LogInformation("Loading theme preference for user: {UserId}", userId);
                _currentUserId = userId;

                var savedThemeName = await LoadThemePreferenceFromDatabase(userId);

                if (!string.IsNullOrEmpty(savedThemeName))
                {
                    _logger.LogInformation("Found saved theme preference: {ThemeName}", savedThemeName);
                    await ApplyThemeAsync(savedThemeName);
                }
                else
                {
                    _logger.LogInformation("No saved theme preference found, using system default");
                    await ApplyThemeAsync(ThemeVariant.Default);
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load theme preference for user: {userId}", Environment.UserName);
                // Continue with default theme on error
                await ApplyThemeAsync(ThemeVariant.Default);
            }
        }

        /// <summary>
        /// Saves current theme preference to database
        /// </summary>
        public async Task SaveUserThemePreferenceAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            try
            {
                _currentUserId = userId;
                var themeName = GetThemeName(_currentTheme, _isSystemThemeEnabled);
                await SaveThemePreferenceToDatabase(userId, themeName);

                _logger.LogInformation("Saved theme preference for user {UserId}: {ThemeName}", userId, themeName);
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to save theme preference for user: {userId}", Environment.UserName);
                throw;
            }
        }

        /// <summary>
        /// Sets system theme following mode
        /// </summary>
        public async Task SetSystemThemeFollowingAsync(bool enabled)
        {
            try
            {
                _logger.LogInformation("Setting system theme following: {Enabled}", enabled);

                if (enabled)
                {
                    await ApplyThemeAsync(ThemeVariant.Default);
                }
                else if (_currentTheme == ThemeVariant.Default)
                {
                    // Switch to explicit light theme when disabling system following
                    await ApplyThemeAsync(ThemeVariant.Light);
                }

                _isSystemThemeEnabled = enabled;
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to set system theme following", Environment.UserName);
                throw;
            }
        }

        /// <summary>
        /// Gets the resolved theme variant (never Default)
        /// </summary>
        public ThemeVariant GetResolvedThemeVariant()
        {
            if (_currentTheme == ThemeVariant.Default)
            {
                // Resolve system theme to actual Light/Dark variant
                if (Application.Current?.ActualThemeVariant == ThemeVariant.Dark)
                {
                    return ThemeVariant.Dark;
                }
                return ThemeVariant.Light;
            }

            return _currentTheme;
        }

        /// <summary>
        /// Refreshes theme from system settings
        /// </summary>
        public async Task RefreshSystemThemeAsync()
        {
            try
            {
                if (_isSystemThemeEnabled && Application.Current is not null)
                {
                    _logger.LogInformation("Refreshing system theme");

                    // Get current system theme
                    var systemTheme = Application.Current.ActualThemeVariant;
                    _logger.LogInformation("System theme detected: {SystemTheme}", systemTheme);

                    // Apply system theme without changing system following state
                    var previousTheme = _currentTheme;
                    _currentTheme = ThemeVariant.Default;

                    // Raise event for system-triggered change
                    ThemeChanged?.Invoke(this, new MTM_WIP_Application_Avalonia.Services.Interfaces.ThemeChangedEventArgs(
                        ThemeVariant.Default,
                        previousTheme,
                        "System",
                        true));
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to refresh system theme", Environment.UserName);
            }
        }

        /// <summary>
        /// Handles system theme changes when system following is enabled
        /// </summary>
        private async void OnSystemThemeChanged(object? sender, EventArgs e)
        {
            if (_isSystemThemeEnabled)
            {
                _logger.LogInformation("System theme changed, updating application theme");
                await RefreshSystemThemeAsync();
            }
        }

        /// <summary>
        /// Converts theme variant to user-friendly name
        /// </summary>
        private static string GetThemeName(ThemeVariant themeVariant, bool isSystemEnabled)
        {
            if (isSystemEnabled || themeVariant == ThemeVariant.Default)
                return "System";

            if (themeVariant == ThemeVariant.Light)
                return "Light";
            else if (themeVariant == ThemeVariant.Dark)
                return "Dark";
            else
                return "System";
        }

        /// <summary>
        /// Parses theme name string to ThemeVariant
        /// </summary>
        private static ThemeVariant ParseThemeName(string themeName)
        {
            var lowerThemeName = themeName.ToLowerInvariant();
            if (lowerThemeName == "light")
                return ThemeVariant.Light;
            else if (lowerThemeName == "dark")
                return ThemeVariant.Dark;
            else if (lowerThemeName == "system")
                return ThemeVariant.Default;
            else
                throw new ArgumentException($"Unknown theme name: {themeName}", nameof(themeName));
        }

        /// <summary>
        /// Loads theme preference from MTM database using stored procedure
        /// </summary>
        private async Task<string?> LoadThemePreferenceFromDatabase(string userId)
        {
            try
            {
                var connectionString = _configurationService.GetConnectionString();
                var parameters = new Dictionary<string, object>
                {
                    ["p_UserId"] = userId
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "usr_theme_preferences_Get",  // Stored procedure to get theme preference
                    parameters
                );

                if (result.Status == 1 && result.Data.Rows.Count > 0)
                {
                    var row = result.Data.Rows[0];
                    var themeName = row["ThemeName"]?.ToString();

                    _logger.LogDebug("Loaded theme preference from database: {ThemeName}", themeName);
                    return themeName;
                }

                _logger.LogDebug("No theme preference found in database for user: {UserId}", userId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load theme preference from database for user: {UserId}", userId);
                return null;  // Return null to use default theme
            }
        }

        /// <summary>
        /// Saves theme preference to MTM database using stored procedure
        /// </summary>
        private async Task SaveThemePreferenceToDatabase(string userId, string themeName)
        {
            try
            {
                var connectionString = _configurationService.GetConnectionString();
                var parameters = new Dictionary<string, object>
                {
                    ["p_UserId"] = userId,
                    ["p_ThemeName"] = themeName
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "usr_theme_preferences_Set",  // Stored procedure to save theme preference
                    parameters
                );

                if (result.Status != 1)
                {
                    throw new InvalidOperationException($"Failed to save theme preference. Database status: {result.Status}");
                }

                _logger.LogDebug("Saved theme preference to database: User={UserId}, Theme={ThemeName}", userId, themeName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save theme preference to database: User={UserId}, Theme={ThemeName}", userId, themeName);
                throw;
            }
        }
    }
}
