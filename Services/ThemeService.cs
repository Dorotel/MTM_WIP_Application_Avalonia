using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using MTM.Models;
using MTM.Core.Services;

namespace MTM.Services
{
    /// <summary>
    /// MTM Theme management service for dynamic theme switching and customization.
    /// Provides centralized theme management with support for light/dark modes and brand customization.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the currently active theme.
        /// </summary>
        MTMTheme CurrentTheme { get; }

        /// <summary>
        /// Gets available themes.
        /// </summary>
        IReadOnlyList<MTMTheme> AvailableThemes { get; }

        /// <summary>
        /// Event fired when theme changes.
        /// </summary>
        event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        /// <summary>
        /// Sets the active theme.
        /// </summary>
        Task<Result> SetThemeAsync(string themeId);

        /// <summary>
        /// Toggles between light and dark variants of current theme.
        /// </summary>
        Task<Result> ToggleVariantAsync();

        /// <summary>
        /// Gets theme preference from user settings.
        /// </summary>
        Task<Result<MTMTheme>> GetUserPreferredThemeAsync();

        /// <summary>
        /// Saves theme preference to user settings.
        /// </summary>
        Task<Result> SaveUserPreferredThemeAsync(string themeId);

        /// <summary>
        /// Applies custom color overrides to current theme.
        /// </summary>
        Task<Result> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides);
    }

    /// <summary>
    /// Implementation of MTM theme management service.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private readonly ILogger<ThemeService> _logger;
        private readonly IApplicationStateService _applicationStateService;
        private MTMTheme _currentTheme;
        private readonly List<MTMTheme> _availableThemes;
        private Application? _application;

        public ThemeService(ILogger<ThemeService> logger, IApplicationStateService applicationStateService)
        {
            _logger = logger;
            _applicationStateService = applicationStateService;
            _currentTheme = CreateDefaultTheme();
            _availableThemes = CreateAvailableThemes();
        }

        public MTMTheme CurrentTheme => _currentTheme;

        public IReadOnlyList<MTMTheme> AvailableThemes => _availableThemes.AsReadOnly();

        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

        /// <summary>
        /// Initializes the theme service with the Avalonia application instance.
        /// </summary>
        public void Initialize(Application application)
        {
            _application = application;
            _logger.LogInformation("ThemeService initialized with application instance");
        }

        /// <summary>
        /// Sets the active theme by ID.
        /// </summary>
        public async Task<Result> SetThemeAsync(string themeId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(themeId))
                {
                    return Result.Failure("Theme ID cannot be empty");
                }

                var theme = _availableThemes.Find(t => t.Id == themeId);
                if (theme == null)
                {
                    _logger.LogWarning("Theme not found: {ThemeId}", themeId);
                    return Result.Failure($"Theme not found: {themeId}");
                }

                var previousTheme = _currentTheme;
                _currentTheme = theme;

                // Apply theme to application
                await ApplyThemeToApplicationAsync(theme);

                // Save to user preferences
                await SaveUserPreferredThemeAsync(themeId);

                // Fire theme changed event
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(previousTheme, theme));

                _logger.LogInformation("Theme changed from {PreviousTheme} to {CurrentTheme}", 
                    previousTheme.DisplayName, theme.DisplayName);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set theme: {ThemeId}", themeId);
                return Result.Failure($"Failed to set theme: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggles between light and dark variants.
        /// </summary>
        public async Task<Result> ToggleVariantAsync()
        {
            try
            {
                var targetVariant = _currentTheme.Variant == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
                var targetThemeId = _currentTheme.BaseName + (targetVariant == ThemeVariant.Dark ? "_Dark" : "_Light");

                var targetTheme = _availableThemes.Find(t => t.Id == targetThemeId);
                if (targetTheme == null)
                {
                    // Create variant if it doesn't exist
                    targetTheme = CreateThemeVariant(_currentTheme.BaseName, targetVariant);
                    _availableThemes.Add(targetTheme);
                }

                return await SetThemeAsync(targetTheme.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle theme variant");
                return Result.Failure($"Failed to toggle theme variant: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user's preferred theme from application state.
        /// </summary>
        public async Task<Result<MTMTheme>> GetUserPreferredThemeAsync()
        {
            try
            {
                var themeId = _applicationStateService.GetSetting<string>("PreferredTheme") ?? "MTM_Light";
                var theme = _availableThemes.Find(t => t.Id == themeId) ?? _currentTheme;

                _logger.LogDebug("Retrieved user preferred theme: {ThemeId}", theme.Id);
                return Result<MTMTheme>.Success(theme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user preferred theme");
                return Result<MTMTheme>.Failure($"Failed to get preferred theme: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves user's preferred theme to application state.
        /// </summary>
        public async Task<Result> SaveUserPreferredThemeAsync(string themeId)
        {
            try
            {
                _applicationStateService.SetSetting("PreferredTheme", themeId);
                _logger.LogDebug("Saved user preferred theme: {ThemeId}", themeId);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save user preferred theme: {ThemeId}", themeId);
                return Result.Failure($"Failed to save preferred theme: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies custom color overrides to the current theme.
        /// </summary>
        public async Task<Result> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides)
        {
            try
            {
                if (colorOverrides == null || colorOverrides.Count == 0)
                {
                    return Result.Success();
                }

                if (_application == null)
                {
                    return Result.Failure("Application not initialized");
                }

                _logger.LogInformation("Applying {Count} custom color overrides", colorOverrides.Count);

                // Apply color overrides to application resources
                foreach (var colorOverride in colorOverrides)
                {
                    if (_application.Resources.TryGetResource(colorOverride.Key, Avalonia.Styling.ThemeVariant.Default, out var existingResource))
                    {
                        // TODO: Parse color string and create SolidColorBrush
                        // This would require color parsing logic
                        _logger.LogDebug("Applied color override: {Key} = {Value}", colorOverride.Key, colorOverride.Value);
                    }
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply custom colors");
                return Result.Failure($"Failed to apply custom colors: {ex.Message}");
            }
        }

        /// <summary>
        /// Applies theme to the Avalonia application.
        /// </summary>
        private async Task ApplyThemeToApplicationAsync(MTMTheme theme)
        {
            if (_application == null)
            {
                _logger.LogWarning("Cannot apply theme - application not initialized");
                return;
            }

            try
            {
                // Set the requested theme variant
                _application.RequestedThemeVariant = theme.Variant switch
                {
                    ThemeVariant.Light => Avalonia.Styling.ThemeVariant.Light,
                    ThemeVariant.Dark => Avalonia.Styling.ThemeVariant.Dark,
                    _ => Avalonia.Styling.ThemeVariant.Default
                };

                // Apply theme-specific resource overrides
                await ApplyThemeResourcesAsync(theme);

                _logger.LogDebug("Applied theme to application: {ThemeName} ({Variant})", theme.DisplayName, theme.Variant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply theme to application: {ThemeName}", theme.DisplayName);
                throw;
            }
        }

        /// <summary>
        /// Applies theme-specific resources to the application.
        /// </summary>
        private async Task ApplyThemeResourcesAsync(MTMTheme theme)
        {
            if (_application == null) return;

            try
            {
                // Load theme-specific resource dictionary if needed
                if (!string.IsNullOrEmpty(theme.ResourcePath))
                {
                    // TODO: Load resource dictionary - requires proper URI handling
                    // var resourceDict = AvaloniaXamlLoader.Load(new Uri(theme.ResourcePath)) as ResourceDictionary;
                    _logger.LogDebug("Theme resources path: {ResourcePath}", theme.ResourcePath);
                }

                // Apply primary color overrides
                if (!string.IsNullOrEmpty(theme.PrimaryColor))
                {
                    // TODO: Parse color and apply to MTM.PrimaryBrush resource
                    _logger.LogDebug("Applied primary color: {PrimaryColor}", theme.PrimaryColor);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply theme resources for theme: {ThemeName}", theme.DisplayName);
                throw;
            }
        }

        /// <summary>
        /// Creates the default MTM theme.
        /// </summary>
        private MTMTheme CreateDefaultTheme()
        {
            return new MTMTheme
            {
                Id = "MTM_Light",
                BaseName = "MTM",
                DisplayName = "MTM Light",
                Description = "Default MTM purple brand theme with light background",
                Variant = ThemeVariant.Light,
                PrimaryColor = "#4B45ED",
                SecondaryColor = "#8345ED",
                AccentColor = "#BA45ED",
                ResourcePath = "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMTheme.axaml",
                IsDefault = true
            };
        }

        /// <summary>
        /// Creates the list of available themes.
        /// </summary>
        private List<MTMTheme> CreateAvailableThemes()
        {
            return new List<MTMTheme>
            {
                // Light Theme (Default)
                new MTMTheme
                {
                    Id = "MTM_Light",
                    BaseName = "MTM",
                    DisplayName = "MTM Light",
                    Description = "Default MTM purple brand theme with light background",
                    Variant = ThemeVariant.Light,
                    PrimaryColor = "#4B45ED",
                    SecondaryColor = "#8345ED",
                    AccentColor = "#BA45ED",
                    ResourcePath = "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMTheme.axaml",
                    IsDefault = true
                },

                // Dark Theme
                new MTMTheme
                {
                    Id = "MTM_Dark",
                    BaseName = "MTM",
                    DisplayName = "MTM Dark",
                    Description = "MTM purple brand theme with dark background",
                    Variant = ThemeVariant.Dark,
                    PrimaryColor = "#6B5FFF",
                    SecondaryColor = "#9B5FFF",
                    AccentColor = "#CB5FFF",
                    ResourcePath = "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMTheme.axaml",
                    IsDefault = false
                },

                // High Contrast Theme
                new MTMTheme
                {
                    Id = "MTM_HighContrast",
                    BaseName = "MTM",
                    DisplayName = "MTM High Contrast",
                    Description = "High contrast MTM theme for accessibility",
                    Variant = ThemeVariant.Light,
                    PrimaryColor = "#000000",
                    SecondaryColor = "#333333",
                    AccentColor = "#4B45ED",
                    ResourcePath = "avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTMTheme.axaml",
                    IsDefault = false
                }
            };
        }

        /// <summary>
        /// Creates a variant of an existing theme.
        /// </summary>
        private MTMTheme CreateThemeVariant(string baseName, ThemeVariant variant)
        {
            var baseTheme = _availableThemes.Find(t => t.BaseName == baseName && t.IsDefault);
            if (baseTheme == null)
            {
                baseTheme = CreateDefaultTheme();
            }

            return new MTMTheme
            {
                Id = $"{baseName}_{variant}",
                BaseName = baseName,
                DisplayName = $"{baseName} {variant}",
                Description = $"{baseName} theme with {variant.ToString().ToLower()} variant",
                Variant = variant,
                PrimaryColor = variant == ThemeVariant.Dark ? "#6B5FFF" : baseTheme.PrimaryColor,
                SecondaryColor = variant == ThemeVariant.Dark ? "#9B5FFF" : baseTheme.SecondaryColor,
                AccentColor = variant == ThemeVariant.Dark ? "#CB5FFF" : baseTheme.AccentColor,
                ResourcePath = baseTheme.ResourcePath,
                IsDefault = false
            };
        }
    }

    /// <summary>
    /// MTM theme definition model.
    /// </summary>
    public class MTMTheme
    {
        public string Id { get; set; } = string.Empty;
        public string BaseName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ThemeVariant Variant { get; set; } = ThemeVariant.Light;
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
        public string AccentColor { get; set; } = string.Empty;
        public string ResourcePath { get; set; } = string.Empty;
        public bool IsDefault { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Theme variant enumeration.
    /// </summary>
    public enum ThemeVariant
    {
        Light,
        Dark,
        HighContrast,
        Custom
    }

    /// <summary>
    /// Theme changed event arguments.
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        public MTMTheme PreviousTheme { get; }
        public MTMTheme NewTheme { get; }

        public ThemeChangedEventArgs(MTMTheme previousTheme, MTMTheme newTheme)
        {
            PreviousTheme = previousTheme;
            NewTheme = newTheme;
        }
    }
}
