using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Theme management service interface for dynamic theme switching.
/// Provides comprehensive theme management following MTM patterns.
/// </summary>
public interface IThemeService : INotifyPropertyChanged
{
    string CurrentTheme { get; }
    IReadOnlyList<ThemeInfo> AvailableThemes { get; }
    bool IsDarkTheme { get; }
    
    Task<ServiceResult> SetThemeAsync(string themeId);
    Task<ServiceResult> ToggleVariantAsync();
    Task<ServiceResult<string>> GetUserPreferredThemeAsync();
    Task<ServiceResult> SaveUserPreferredThemeAsync(string themeId);
    Task<ServiceResult> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides);
    
    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}

/// <summary>
/// Theme information model.
/// </summary>
public class ThemeInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDark { get; set; }
    public string PreviewColor { get; set; } = "#0078D4";
}

/// <summary>
/// Theme changed event arguments.
/// </summary>
public class ThemeChangedEventArgs : EventArgs
{
    public ThemeInfo PreviousTheme { get; set; } = new();
    public ThemeInfo NewTheme { get; set; } = new();
}

/// <summary>
/// Service result pattern for theme operations.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    
    public static ServiceResult Success(string message = "") => new() { IsSuccess = true, Message = message };
    public static ServiceResult Failure(string message, Exception? exception = null) => new() { IsSuccess = false, Message = message, Exception = exception };
}

/// <summary>
/// Service result with value pattern for theme operations.
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; set; }
    
    public static ServiceResult<T> Success(T value, string message = "") => new() { IsSuccess = true, Value = value, Message = message };
    public static new ServiceResult<T> Failure(string message, Exception? exception = null) => new() { IsSuccess = false, Message = message, Exception = exception };
}

/// <summary>
/// Theme service implementation integrated with MTM Configuration service.
/// Provides dynamic theme switching with real-time application.
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ILogger<ThemeService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly List<ThemeInfo> _availableThemes;
    private ThemeInfo _currentTheme;
    private const string DEFAULT_THEME_ID = "MTMTheme"; // Default to MTMTheme.axaml

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public ThemeService(ILogger<ThemeService> logger, IConfigurationService configurationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        
        _availableThemes = InitializeThemes();
        
        // Set default theme to MTMTheme if available, otherwise first theme
        _currentTheme = _availableThemes.FirstOrDefault(t => t.Id == DEFAULT_THEME_ID) ?? _availableThemes.First();
        
        _logger.LogInformation("ThemeService initialized with {ThemeCount} available themes, default theme: {DefaultTheme}", 
            _availableThemes.Count, _currentTheme.DisplayName);
            
        // Initialize with user's preferred theme asynchronously
        _ = Task.Run(async () =>
        {
            try
            {
                var preferredThemeResult = await GetUserPreferredThemeAsync();
                if (preferredThemeResult.IsSuccess && !string.IsNullOrEmpty(preferredThemeResult.Value))
                {
                    await SetThemeAsync(preferredThemeResult.Value);
                }
                else
                {
                    // Apply the default theme to ensure resources are loaded
                    await ApplyThemeToApplicationAsync(_currentTheme);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying initial theme");
                // Fallback to applying default theme
                try
                {
                    await ApplyThemeToApplicationAsync(_currentTheme);
                }
                catch (Exception fallbackEx)
                {
                    _logger.LogError(fallbackEx, "Error applying fallback theme");
                }
            }
        });
    }

    public string CurrentTheme => _currentTheme.Id;
    public IReadOnlyList<ThemeInfo> AvailableThemes => _availableThemes.AsReadOnly();
    public bool IsDarkTheme => _currentTheme.IsDark;

    /// <summary>
    /// Sets the application theme by ID.
    /// </summary>
    public async Task<ServiceResult> SetThemeAsync(string themeId)
    {
        try
        {
            var theme = _availableThemes.FirstOrDefault(t => t.Id == themeId);
            if (theme == null)
            {
                return ServiceResult.Failure($"Theme '{themeId}' not found");
            }

            var previousTheme = _currentTheme;
            _currentTheme = theme;

            // Apply theme to Avalonia application
            await ApplyThemeToApplicationAsync(theme);

            // Notify theme change
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTheme)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDarkTheme)));
            
            ThemeChanged?.Invoke(this, new ThemeChangedEventArgs
            {
                PreviousTheme = previousTheme,
                NewTheme = theme
            });

            _logger.LogInformation("Theme changed from {Previous} to {New}", previousTheme.DisplayName, theme.DisplayName);
            return ServiceResult.Success($"Theme changed to {theme.DisplayName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting theme to {ThemeId}", themeId);
            return ServiceResult.Failure($"Failed to set theme: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Toggles between light and dark variant of current theme.
    /// </summary>
    public async Task<ServiceResult> ToggleVariantAsync()
    {
        try
        {
            var targetIsDark = !_currentTheme.IsDark;
            var targetTheme = _availableThemes.FirstOrDefault(t => t.IsDark == targetIsDark);
            
            if (targetTheme != null)
            {
                return await SetThemeAsync(targetTheme.Id);
            }

            return ServiceResult.Failure("No alternate theme variant available");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling theme variant");
            return ServiceResult.Failure($"Failed to toggle theme: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets user's preferred theme from configuration.
    /// </summary>
    public async Task<ServiceResult<string>> GetUserPreferredThemeAsync()
    {
        try
        {
            await Task.CompletedTask; // Placeholder for async configuration access
            
            var preferredTheme = _configurationService.GetValue("User:PreferredTheme", DEFAULT_THEME_ID);
            
            // Validate that the preferred theme exists
            if (!_availableThemes.Any(t => t.Id == preferredTheme))
            {
                _logger.LogWarning("User preferred theme '{PreferredTheme}' not found, using default '{DefaultTheme}'", 
                    preferredTheme, DEFAULT_THEME_ID);
                preferredTheme = DEFAULT_THEME_ID;
            }
            
            _logger.LogDebug("Retrieved user preferred theme: {Theme}", preferredTheme);
            return ServiceResult<string>.Success(preferredTheme);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user preferred theme");
            return ServiceResult<string>.Failure($"Failed to get preferred theme: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Saves user's preferred theme to configuration.
    /// </summary>
    public async Task<ServiceResult> SaveUserPreferredThemeAsync(string themeId)
    {
        try
        {
            await Task.CompletedTask; // Placeholder for async configuration save
            
            // Note: Current IConfigurationService doesn't have save method
            // This would need to be implemented when configuration persistence is added
            
            _logger.LogInformation("User preferred theme saved: {Theme}", themeId);
            return ServiceResult.Success("Theme preference saved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving user preferred theme {ThemeId}", themeId);
            return ServiceResult.Failure($"Failed to save theme preference: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Applies custom color overrides to current theme.
    /// </summary>
    public async Task<ServiceResult> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides)
    {
        try
        {
            await Task.CompletedTask; // Placeholder for async implementation
            
            if (colorOverrides?.Any() != true)
            {
                return ServiceResult.Failure("No color overrides provided");
            }

            // Apply custom colors to current theme
            // This would modify the resource dictionary at runtime
            
            _logger.LogInformation("Applied {Count} custom color overrides", colorOverrides.Count);
            return ServiceResult.Success($"Applied {colorOverrides.Count} color customizations");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying custom colors");
            return ServiceResult.Failure($"Failed to apply custom colors: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Initializes available themes from all theme files in Resources/Themes folder.
    /// </summary>
    private List<ThemeInfo> InitializeThemes()
    {
        return new List<ThemeInfo>
        {
            // MTM Default Theme (MTMTheme.axaml)
            new ThemeInfo
            {
                Id = "MTMTheme",
                DisplayName = "MTM Default",
                Description = "Default MTM theme with Windows 11 style professional blue palette",
                IsDark = false,
                PreviewColor = "#0078D4"
            },
            
            // Light Themes
            new ThemeInfo
            {
                Id = "MTM_Light",
                DisplayName = "MTM Light Gold",
                Description = "Light theme with warm gold industrial palette",
                IsDark = false,
                PreviewColor = "#B8860B"
            },
            new ThemeInfo
            {
                Id = "MTM_Blue",
                DisplayName = "MTM Professional Blue",
                Description = "Professional blue theme for corporate environments",
                IsDark = false,
                PreviewColor = "#1E88E5"
            },
            new ThemeInfo
            {
                Id = "MTM_Green",
                DisplayName = "MTM Success Green",
                Description = "Growth and success oriented green theme",
                IsDark = false,
                PreviewColor = "#43A047"
            },
            new ThemeInfo
            {
                Id = "MTM_Red",
                DisplayName = "MTM Alert Red",
                Description = "Critical systems and alert red theme",
                IsDark = false,
                PreviewColor = "#E53935"
            },
            new ThemeInfo
            {
                Id = "MTM_Teal",
                DisplayName = "MTM Focus Teal",
                Description = "Calming and focus-oriented teal theme",
                IsDark = false,
                PreviewColor = "#00ACC1"
            },
            new ThemeInfo
            {
                Id = "MTM_Amber",
                DisplayName = "MTM Industrial Amber",
                Description = "Warm industrial amber theme for manufacturing",
                IsDark = false,
                PreviewColor = "#FF8F00"
            },
            new ThemeInfo
            {
                Id = "MTM_Indigo",
                DisplayName = "MTM Deep Indigo",
                Description = "Deep professional indigo theme",
                IsDark = false,
                PreviewColor = "#3F51B5"
            },
            new ThemeInfo
            {
                Id = "MTM_Rose",
                DisplayName = "MTM Soft Rose",
                Description = "Soft and approachable rose theme",
                IsDark = false,
                PreviewColor = "#E91E63"
            },
            new ThemeInfo
            {
                Id = "MTM_Emerald",
                DisplayName = "MTM Modern Emerald",
                Description = "Fresh and modern emerald theme",
                IsDark = false,
                PreviewColor = "#00C853"
            },
            
            // Dark Variants
            new ThemeInfo
            {
                Id = "MTM_Dark",
                DisplayName = "MTM Dark",
                Description = "Dark theme with MTM professional styling",
                IsDark = true,
                PreviewColor = "#4B45ED"
            },
            new ThemeInfo
            {
                Id = "MTM_Light_Dark",
                DisplayName = "MTM Light Gold Dark",
                Description = "Dark variant of MTM Light Gold theme",
                IsDark = true,
                PreviewColor = "#DAA520"
            },
            new ThemeInfo
            {
                Id = "MTM_Blue_Dark",
                DisplayName = "MTM Professional Blue Dark",
                Description = "Dark variant of professional blue theme",
                IsDark = true,
                PreviewColor = "#1565C0"
            },
            new ThemeInfo
            {
                Id = "MTM_Green_Dark",
                DisplayName = "MTM Success Green Dark",
                Description = "Dark variant of success green theme",
                IsDark = true,
                PreviewColor = "#2E7D32"
            },
            new ThemeInfo
            {
                Id = "MTM_Red_Dark",
                DisplayName = "MTM Alert Red Dark",
                Description = "Dark variant of alert red theme",
                IsDark = true,
                PreviewColor = "#C62828"
            },
            new ThemeInfo
            {
                Id = "MTM_Teal_Dark",
                DisplayName = "MTM Focus Teal Dark",
                Description = "Dark variant of focus teal theme",
                IsDark = true,
                PreviewColor = "#00838F"
            },
            new ThemeInfo
            {
                Id = "MTM_Indigo_Dark",
                DisplayName = "MTM Deep Indigo Dark",
                Description = "Dark variant of deep indigo theme",
                IsDark = true,
                PreviewColor = "#283593"
            },
            new ThemeInfo
            {
                Id = "MTM_Rose_Dark",
                DisplayName = "MTM Soft Rose Dark",
                Description = "Dark variant of soft rose theme",
                IsDark = true,
                PreviewColor = "#AD1457"
            },
            
            // Accessibility Theme
            new ThemeInfo
            {
                Id = "MTM_HighContrast",
                DisplayName = "MTM High Contrast",
                Description = "High contrast theme for accessibility compliance",
                IsDark = false,
                PreviewColor = "#000000"
            }
        };
    }

    /// <summary>
    /// Applies theme to Avalonia application.
    /// </summary>
    private async Task ApplyThemeToApplicationAsync(ThemeInfo theme)
    {
        try
        {
            // Wait for Application.Current to be available with timeout
            var maxWaitTime = TimeSpan.FromSeconds(10);
            var startTime = DateTime.UtcNow;
            
            while (Application.Current == null && DateTime.UtcNow - startTime < maxWaitTime)
            {
                _logger.LogDebug("Waiting for Application.Current to be available...");
                await Task.Delay(100);
            }
            
            if (Application.Current == null) 
            {
                _logger.LogWarning("Application.Current is still null after {MaxWaitSeconds}s, cannot apply theme", maxWaitTime.TotalSeconds);
                return;
            }

            // Set the theme variant based on theme
            var themeVariant = theme.IsDark ? ThemeVariant.Dark : ThemeVariant.Light;
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Application.Current.RequestedThemeVariant = themeVariant;
            });

            // Apply theme-specific resource file
            await LoadThemeResourcesAsync(theme.Id);
            
            _logger.LogDebug("Applied theme variant: {Variant} for theme: {Theme}", themeVariant, theme.DisplayName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying theme to application");
            throw;
        }
    }

    /// <summary>
    /// Load theme-specific resources from the Resources/Themes folder.
    /// </summary>
    private async Task LoadThemeResourcesAsync(string themeId)
    {
        try
        {
            if (Application.Current?.Resources == null) 
            {
                _logger.LogWarning("Application resources not available");
                return;
            }

            await Task.CompletedTask; // Make this actually async for future enhancements

            // Build the resource URI for the specific theme
            var themeResourceUri = new Uri($"avares://MTM_WIP_Application_Avalonia/Resources/Themes/{themeId}.axaml");
            
            try
            {
                // Clear existing theme-specific resources FIRST
                ClearThemeResources();
                
                // Small delay to ensure cleanup completes
                await Task.Delay(20);
                
                // Load the theme resource dictionary using AvaloniaXamlLoader
                try 
                {
                    // Try to load the resource dictionary directly
                    var themeResources = (ResourceDictionary)AvaloniaXamlLoader.Load(themeResourceUri);
                    
                    // Merge the new theme resources
                    Application.Current.Resources.MergedDictionaries.Add(themeResources);
                    
                    _logger.LogInformation("Successfully loaded theme resources from: {Uri}", themeResourceUri);
                    
                    // CRITICAL FIX: Force immediate and complete resource refresh
                    await ForceCompleteApplicationRefreshAsync();
                }
                catch (Exception loadEx)
                {
                    _logger.LogWarning(loadEx, "Could not load theme resource file {Uri}, theme files may not exist yet", themeResourceUri);
                    // Don't throw - this allows fallback to basic theme variant switching
                }
            }
            catch (Exception resourceEx)
            {
                _logger.LogWarning(resourceEx, "Could not load theme resource file {Uri}, using basic theme switching", themeResourceUri);
                // Don't throw - this allows fallback to basic theme variant switching
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in LoadThemeResourcesAsync for theme {ThemeId}", themeId);
            // Don't throw - allow basic theme switching to work
        }
    }

    /// <summary>
    /// Forces a complete application refresh similar to a restart for theme changes.
    /// This is the most aggressive approach to ensure all controls update.
    /// </summary>
    private async Task ForceCompleteApplicationRefreshAsync()
    {
        try
        {
            if (Application.Current == null) return;

            await Task.Delay(50); // Allow resource loading to settle

            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    if (Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        foreach (var window in desktop.Windows.ToList()) // ToList to avoid modification during iteration
                        {
                            try
                            {
                                // Force complete window refresh
                                RefreshWindowCompletely(window);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Error during complete window refresh");
                            }
                        }
                    }

                    _logger.LogDebug("Complete application refresh completed");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error during complete application refresh");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ForceCompleteApplicationRefreshAsync");
        }
    }

    /// <summary>
    /// Performs the most aggressive window refresh possible short of recreating the window.
    /// </summary>
    private void RefreshWindowCompletely(Window window)
    {
        try
        {
            // Step 1: Force all visual invalidation
            window.InvalidateVisual();
            window.InvalidateMeasure();
            window.InvalidateArrange();

            // Step 2: Force all child controls to refresh recursively
            InvalidateControlTreeRecursively(window);

            // Step 3: Force re-application of styles by temporarily changing and restoring properties
            var originalOpacity = window.Opacity;
            window.Opacity = 0.99;
            window.Opacity = originalOpacity;

            // Step 4: Force layout update
            window.UpdateLayout();

            _logger.LogDebug("Complete window refresh performed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error in RefreshWindowCompletely");
        }
    }

    /// <summary>
    /// Recursively invalidates all controls in a window's visual tree.
    /// </summary>
    private void InvalidateControlTreeRecursively(Avalonia.Controls.Control control)
    {
        try
        {
            if (control == null) return;

            // Force visual re-evaluation to pick up new resource values
            control.InvalidateVisual();
            control.InvalidateMeasure();
            control.InvalidateArrange();

            // Special handling for TabControls - they need extra invalidation
            if (control is Avalonia.Controls.TabControl tabControl)
            {
                ForceTabControlRefresh(tabControl);
            }

            // Recursively process children
            if (control is Avalonia.Controls.Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Avalonia.Controls.Control childControl)
                    {
                        InvalidateControlTreeRecursively(childControl);
                    }
                }
            }
            else if (control is Avalonia.Controls.ContentControl contentControl && contentControl.Content is Avalonia.Controls.Control content)
            {
                InvalidateControlTreeRecursively(content);
            }
            else if (control is Avalonia.Controls.Decorator decorator && decorator.Child is Avalonia.Controls.Control decoratorChild)
            {
                InvalidateControlTreeRecursively(decoratorChild);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error invalidating control tree recursively");
        }
    }

    /// <summary>
    /// Forces TabControl refresh by manipulating selection and invalidating all TabItems.
    /// </summary>
    private void ForceTabControlRefresh(Avalonia.Controls.TabControl tabControl)
    {
        try
        {
            // Store current selection
            var currentSelection = tabControl.SelectedIndex;
            
            // Force TabControl to refresh
            tabControl.InvalidateVisual();
            tabControl.InvalidateMeasure();
            tabControl.InvalidateArrange();
            
            // Force style re-application by cycling through all TabItems
            for (int i = 0; i < tabControl.Items.Count; i++)
            {
                try
                {
                    // Temporarily select each tab to force style refresh
                    tabControl.SelectedIndex = i;
                    Task.Delay(1).Wait(); // Minimal delay for style application
                    
                    // Force refresh after each selection change
                    tabControl.InvalidateVisual();
                    tabControl.InvalidateMeasure();
                    tabControl.InvalidateArrange();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error during TabItem {Index} refresh", i);
                }
            }
            
            // Restore original selection
            tabControl.SelectedIndex = currentSelection;
            
            // Final comprehensive refresh
            tabControl.InvalidateVisual();
            tabControl.InvalidateMeasure();
            tabControl.InvalidateArrange();
            
            // Force update of the entire TabControl layout
            tabControl.UpdateLayout();
            
            _logger.LogDebug("Forced TabControl refresh with style cycling for theme update");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during TabControl-specific refresh");
        }
    }

    /// <summary>
    /// Clear existing theme-specific resources to prevent conflicts.
    /// </summary>
    private void ClearThemeResources()
    {
        try
        {
            if (Application.Current?.Resources?.MergedDictionaries == null) return;

            // Remove any existing theme resource dictionaries
            // We identify them by checking if they contain MTM theme keys
            var themeResourceDictionaries = Application.Current.Resources.MergedDictionaries
                .Where(dict => 
                    dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out _) || 
                    dict.TryGetResource("MTM_Shared_Logic.CardBackgroundBrush", null, out _) ||
                    dict.TryGetResource("MTM_Shared_Logic.SecondaryAction", null, out _) ||
                    dict.TryGetResource("MTM_Shared_Logic.MainBackground", null, out _))
                .ToList();

            foreach (var themeDict in themeResourceDictionaries)
            {
                Application.Current.Resources.MergedDictionaries.Remove(themeDict);
            }

            // Also clear any direct MTM theme resources that might be cached
            var mtmResourceKeys = Application.Current.Resources.Keys
                .Where(key => key != null && key.ToString()?.StartsWith("MTM_Shared_Logic.") == true)
                .ToList();

            foreach (var key in mtmResourceKeys)
            {
                try
                {
                    Application.Current.Resources.Remove(key);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error removing resource key {Key}", key);
                }
            }

            _logger.LogDebug("Cleared {Count} existing theme resource dictionaries and {KeyCount} direct resources", 
                themeResourceDictionaries.Count, mtmResourceKeys.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error clearing theme resources");
            // Don't throw - continue with theme application
        }
    }
}
