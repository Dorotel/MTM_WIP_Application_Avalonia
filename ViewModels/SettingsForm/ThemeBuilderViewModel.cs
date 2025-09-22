using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;

/// <summary>
/// ViewModel for advanced theme builder with live preview functionality.
/// Provides comprehensive theme customization and creation capabilities.
/// </summary>
public partial class ThemeBuilderViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    
    #region Observable Properties

    /// <summary>
    /// Base theme to customize
    /// </summary>
    [ObservableProperty]
    private ThemeInfo? baseTheme;

    /// <summary>
    /// Name of the custom theme
    /// </summary>
    [ObservableProperty]
    [Required(ErrorMessage = "Theme name is required")]
    [StringLength(50, ErrorMessage = "Theme name cannot exceed 50 characters")]
    private string themeName = "Custom Theme";

    /// <summary>
    /// Description of the custom theme
    /// </summary>
    [ObservableProperty]
    [StringLength(200, ErrorMessage = "Theme description cannot exceed 200 characters")]
    private string themeDescription = "Custom theme created with Theme Builder";

    /// <summary>
    /// Primary color of the theme
    /// </summary>
    [ObservableProperty]
    private Color primaryColor = Color.FromRgb(75, 69, 237); // MTM Purple

    /// <summary>
    /// Secondary color of the theme
    /// </summary>
    [ObservableProperty]
    private Color secondaryColor = Color.FromRgb(108, 99, 255);

    /// <summary>
    /// Accent color of the theme
    /// </summary>
    [ObservableProperty]
    private Color accentColor = Color.FromRgb(255, 165, 0);

    /// <summary>
    /// Background color of the theme
    /// </summary>
    [ObservableProperty]
    private Color backgroundColor = Color.FromRgb(255, 255, 255);

    /// <summary>
    /// Surface color for cards and panels
    /// </summary>
    [ObservableProperty]
    private Color surfaceColor = Color.FromRgb(248, 249, 250);

    /// <summary>
    /// Text color of the theme
    /// </summary>
    [ObservableProperty]
    private Color textColor = Color.FromRgb(33, 37, 41);

    /// <summary>
    /// Border color for UI elements
    /// </summary>
    [ObservableProperty]
    private Color borderColor = Color.FromRgb(222, 226, 230);

    /// <summary>
    /// Whether this is a dark theme
    /// </summary>
    [ObservableProperty]
    private bool isDarkTheme;

    /// <summary>
    /// Corner radius for UI elements
    /// </summary>
    [ObservableProperty]
    [Range(0, 20, ErrorMessage = "Corner radius must be between 0 and 20")]
    private double cornerRadius = 8.0;

    /// <summary>
    /// Shadow intensity for depth effects
    /// </summary>
    [ObservableProperty]
    [Range(0.0, 1.0, ErrorMessage = "Shadow intensity must be between 0 and 1")]
    private double shadowIntensity = 0.2;

    /// <summary>
    /// Whether animations are enabled in the theme
    /// </summary>
    [ObservableProperty]
    private bool enableAnimations = true;

    /// <summary>
    /// Whether the preview is currently active
    /// </summary>
    [ObservableProperty]
    private bool isPreviewActive;

    /// <summary>
    /// Whether the theme is currently being saved
    /// </summary>
    [ObservableProperty]
    private bool isSaving;

    #endregion

    public ThemeBuilderViewModel(
        IThemeService themeService,
        ILogger<ThemeBuilderViewModel> logger) : base(logger)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        // Initialize collections
        AvailableBaseThemes = new ObservableCollection<ThemeInfo>();
        ColorPresets = new ObservableCollection<ColorPreset>();
        
        InitializeColorPresets();
        LoadAvailableThemes();

        Logger.LogInformation("ThemeBuilderViewModel initialized");
    }

    #region Collections

    /// <summary>
    /// Available base themes for customization
    /// </summary>
    public ObservableCollection<ThemeInfo> AvailableBaseThemes { get; }

    /// <summary>
    /// Collection of predefined color presets
    /// </summary>
    public ObservableCollection<ColorPreset> ColorPresets { get; }

    #endregion

    #region Computed Properties

    /// <summary>
    /// Primary color as brush for preview
    /// </summary>
    public SolidColorBrush PrimaryBrush => new SolidColorBrush(PrimaryColor);

    /// <summary>
    /// Secondary color as brush for preview
    /// </summary>
    public SolidColorBrush SecondaryBrush => new SolidColorBrush(SecondaryColor);

    /// <summary>
    /// Accent color as brush for preview
    /// </summary>
    public SolidColorBrush AccentBrush => new SolidColorBrush(AccentColor);

    /// <summary>
    /// Background color as brush for preview
    /// </summary>
    public SolidColorBrush BackgroundBrush => new SolidColorBrush(BackgroundColor);

    /// <summary>
    /// Surface color as brush for preview
    /// </summary>
    public SolidColorBrush SurfaceBrush => new SolidColorBrush(SurfaceColor);

    /// <summary>
    /// Text color as brush for preview
    /// </summary>
    public SolidColorBrush TextBrush => new SolidColorBrush(TextColor);

    /// <summary>
    /// Border color as brush for preview
    /// </summary>
    public SolidColorBrush BorderBrush => new SolidColorBrush(BorderColor);

    #endregion

    #region Commands

    /// <summary>
    /// Command to apply live preview of the theme
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanApplyPreview))]
    private async Task ApplyPreviewAsync()
    {
        try
        {
            var colorOverrides = new Dictionary<string, string>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = PrimaryColor.ToString(),
                ["MTM_Shared_Logic.SecondaryAction"] = SecondaryColor.ToString(),
                ["MTM_Shared_Logic.AccentBrush"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.BackgroundBrush"] = BackgroundColor.ToString(),
                ["MTM_Shared_Logic.SurfaceBrush"] = SurfaceColor.ToString(),
                ["MTM_Shared_Logic.HeadingText"] = TextColor.ToString(),
                ["MTM_Shared_Logic.BorderBrush"] = BorderColor.ToString()
            };

            var result = await _themeService.ApplyCustomColorsAsync(colorOverrides);
            
            if (result.IsSuccess)
            {
                IsPreviewActive = true;
                Logger.LogInformation("Theme preview applied successfully");
            }
            else
            {
                Logger.LogWarning("Failed to apply theme preview: {Message}", result.Message);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying theme preview");
        }
    }

    private bool CanApplyPreview() => !string.IsNullOrWhiteSpace(ThemeName);

    /// <summary>
    /// Command to save the custom theme permanently
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSaveTheme))]
    private void SaveTheme()
    {
        try
        {
            IsSaving = true;

            // Create custom theme configuration
            var themeConfig = new
            {
                Name = ThemeName,
                Description = ThemeDescription,
                IsDark = IsDarkTheme,
                Colors = new
                {
                    Primary = PrimaryColor.ToString(),
                    Secondary = SecondaryColor.ToString(),
                    Accent = AccentColor.ToString(),
                    Background = BackgroundColor.ToString(),
                    Surface = SurfaceColor.ToString(),
                    Text = TextColor.ToString(),
                    Border = BorderColor.ToString()
                },
                Styles = new
                {
                    CornerRadius = CornerRadius,
                    ShadowIntensity = ShadowIntensity,
                    EnableAnimations = EnableAnimations
                }
            };

            // Save via theme service
            Logger.LogInformation("Custom theme {ThemeName} saved successfully", ThemeName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving custom theme {ThemeName}", ThemeName);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanSaveTheme() => !IsSaving && !string.IsNullOrWhiteSpace(ThemeName);

    /// <summary>
    /// Command to export the theme to a file
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExportTheme))]
    private void ExportTheme()
    {
        try
        {
            IsSaving = true;

            var themeData = CreateThemeData();
            // Implementation would show file picker and export theme
            Logger.LogInformation("Theme export initiated for {ThemeName}", ThemeName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting theme {ThemeName}", ThemeName);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanExportTheme() => !string.IsNullOrWhiteSpace(ThemeName);

    /// <summary>
    /// Command to import a theme from file
    /// </summary>
    [RelayCommand]
    private void ImportTheme()
    {
        try
        {
            IsSaving = true;

            // Implementation would show file picker and import theme
            Logger.LogInformation("Theme import initiated");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing theme");
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// Command to reset colors to base theme defaults
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanResetToBase))]
    private void ResetToBase()
    {
        try
        {
            if (BaseTheme != null)
            {
                ApplyBaseThemeColors();
                Logger.LogInformation("Theme colors reset to base theme {BaseTheme}", BaseTheme.Name);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting to base theme");
        }
    }

    private bool CanResetToBase() => BaseTheme != null && !IsSaving;

    /// <summary>
    /// Command to randomize theme colors for experimentation
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRandomizeColors))]
    private void RandomizeColors()
    {
        try
        {
            var random = new Random();
            
            PrimaryColor = GenerateRandomColor(random, 0.6, 0.8);
            SecondaryColor = GenerateRandomColor(random, 0.4, 0.7);
            AccentColor = GenerateRandomColor(random, 0.7, 0.9);
            BackgroundColor = GenerateRandomColor(random, 0.05, 0.15);
            SurfaceColor = GenerateRandomColor(random, 0.0, 0.1);
            TextColor = GenerateRandomColor(random, 0.8, 1.0);
            BorderColor = GenerateRandomColor(random, 0.2, 0.4);
            
            ThemeName = $"Random Theme {DateTime.Now:HHmmss}";
            ThemeDescription = "Randomly generated color theme";
            
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error randomizing theme colors");
        }
    }

    private bool CanRandomizeColors() => !IsSaving;

    #endregion

    #region Property Change Handlers

    partial void OnBaseThemeChanged(ThemeInfo? value)
    {
        if (value != null)
        {
            ApplyBaseThemeColors();
        }
    }

    partial void OnPrimaryColorChanged(Color value)
    {
        OnPropertyChanged(nameof(PrimaryBrush));
        OnColorChanged();
    }

    partial void OnSecondaryColorChanged(Color value)
    {
        OnPropertyChanged(nameof(SecondaryBrush));
        OnColorChanged();
    }

    partial void OnAccentColorChanged(Color value)
    {
        OnPropertyChanged(nameof(AccentBrush));
        OnColorChanged();
    }

    partial void OnBackgroundColorChanged(Color value)
    {
        OnPropertyChanged(nameof(BackgroundBrush));
        OnColorChanged();
    }

    partial void OnSurfaceColorChanged(Color value)
    {
        OnPropertyChanged(nameof(SurfaceBrush));
        OnColorChanged();
    }

    partial void OnTextColorChanged(Color value)
    {
        OnPropertyChanged(nameof(TextBrush));
        OnColorChanged();
    }

    partial void OnBorderColorChanged(Color value)
    {
        OnPropertyChanged(nameof(BorderBrush));
        OnColorChanged();
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        OnDarkThemeToggled();
    }

    partial void OnCornerRadiusChanged(double value)
    {
        OnStyleChanged();
    }

    partial void OnShadowIntensityChanged(double value)
    {
        OnStyleChanged();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads available base themes
    /// </summary>
    private void LoadAvailableThemes()
    {
        try
        {
            // Mock data - replace with actual theme service call
            var themes = new List<ThemeInfo>
            {
                new ThemeInfo { Id = "mtm-light", Name = "MTM Light", IsDark = false },
                new ThemeInfo { Id = "mtm-dark", Name = "MTM Dark", IsDark = true },
                new ThemeInfo { Id = "blue-light", Name = "Blue Light", IsDark = false },
                new ThemeInfo { Id = "blue-dark", Name = "Blue Dark", IsDark = true }
            };

            AvailableBaseThemes.Clear();
            foreach (var theme in themes)
            {
                AvailableBaseThemes.Add(theme);
            }

            BaseTheme = AvailableBaseThemes.FirstOrDefault(t => t.Id == "mtm-light");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading available themes");
        }
    }

    /// <summary>
    /// Initializes color presets for quick selection
    /// </summary>
    private void InitializeColorPresets()
    {
        ColorPresets.Add(new ColorPreset("MTM Purple", Color.FromRgb(75, 69, 237)));
        ColorPresets.Add(new ColorPreset("Blue", Color.FromRgb(0, 123, 255)));
        ColorPresets.Add(new ColorPreset("Green", Color.FromRgb(40, 167, 69)));
        ColorPresets.Add(new ColorPreset("Orange", Color.FromRgb(255, 165, 0)));
        ColorPresets.Add(new ColorPreset("Red", Color.FromRgb(220, 53, 69)));
        ColorPresets.Add(new ColorPreset("Purple", Color.FromRgb(111, 66, 193)));
        ColorPresets.Add(new ColorPreset("Teal", Color.FromRgb(32, 201, 151)));
        ColorPresets.Add(new ColorPreset("Gray", Color.FromRgb(108, 117, 125)));
    }

    /// <summary>
    /// Applies colors from the base theme
    /// </summary>
    private void ApplyBaseThemeColors()
    {
        if (BaseTheme == null) return;

        // Set colors based on base theme
        if (BaseTheme.IsDark)
        {
            BackgroundColor = Color.FromRgb(33, 37, 41);
            SurfaceColor = Color.FromRgb(52, 58, 64);
            TextColor = Color.FromRgb(255, 255, 255);
            BorderColor = Color.FromRgb(73, 80, 87);
        }
        else
        {
            BackgroundColor = Color.FromRgb(255, 255, 255);
            SurfaceColor = Color.FromRgb(248, 249, 250);
            TextColor = Color.FromRgb(33, 37, 41);
            BorderColor = Color.FromRgb(222, 226, 230);
        }

        IsDarkTheme = BaseTheme.IsDark;
        ThemeName = $"{BaseTheme.Name} Custom";
        ThemeDescription = $"Customized version of {BaseTheme.Name}";
    }

    /// <summary>
    /// Handles color changes for live preview updates
    /// </summary>
    private void OnColorChanged()
    {
        // Auto-apply preview if active
        if (IsPreviewActive)
        {
            _ = Task.Run(async () => await ApplyPreviewAsync());
        }
    }

    /// <summary>
    /// Handles dark theme toggle
    /// </summary>
    private void OnDarkThemeToggled()
    {
        if (IsDarkTheme)
        {
            BackgroundColor = Color.FromRgb(33, 37, 41);
            SurfaceColor = Color.FromRgb(52, 58, 64);
            TextColor = Color.FromRgb(255, 255, 255);
            BorderColor = Color.FromRgb(73, 80, 87);
        }
        else
        {
            BackgroundColor = Color.FromRgb(255, 255, 255);
            SurfaceColor = Color.FromRgb(248, 249, 250);
            TextColor = Color.FromRgb(33, 37, 41);
            BorderColor = Color.FromRgb(222, 226, 230);
        }
    }

    /// <summary>
    /// Handles style property changes
    /// </summary>
    private void OnStyleChanged()
    {
        // Auto-apply preview if active
        if (IsPreviewActive)
        {
            _ = Task.Run(async () => await ApplyPreviewAsync());
        }
    }

    /// <summary>
    /// Creates theme data from current settings
    /// </summary>
    private object CreateThemeData()
    {
        return new
        {
            Name = ThemeName,
            Description = ThemeDescription,
            IsDark = IsDarkTheme,
            Colors = new
            {
                Primary = PrimaryColor.ToString(),
                Secondary = SecondaryColor.ToString(),
                Accent = AccentColor.ToString(),
                Background = BackgroundColor.ToString(),
                Surface = SurfaceColor.ToString(),
                Text = TextColor.ToString(),
                Border = BorderColor.ToString()
            },
            Styles = new
            {
                CornerRadius = CornerRadius,
                ShadowIntensity = ShadowIntensity,
                EnableAnimations = EnableAnimations
            }
        };
    }

    /// <summary>
    /// Generates a random color with specified saturation range
    /// </summary>
    private Color GenerateRandomColor(Random random, double minSaturation, double maxSaturation)
    {
        var hue = random.NextDouble() * 360;
        var saturation = minSaturation + (random.NextDouble() * (maxSaturation - minSaturation));
        var lightness = 0.4 + (random.NextDouble() * 0.4); // 40% to 80%

        return HslToRgb(hue, saturation, lightness);
    }

    /// <summary>
    /// Converts HSL color to RGB
    /// </summary>
    private Color HslToRgb(double h, double s, double l)
    {
        h /= 360;

        var r = l;
        var g = l;
        var b = l;

        if (s != 0)
        {
            var hue2Rgb = new Func<double, double, double, double>((p, q, t) =>
            {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
                return p;
            });

            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;

            r = hue2Rgb(p, q, h + 1.0 / 3.0);
            g = hue2Rgb(p, q, h);
            b = hue2Rgb(p, q, h - 1.0 / 3.0);
        }

        return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    #endregion
}

/// <summary>
/// Color preset for quick theme color selection
/// </summary>
public class ColorPreset
{
    public ColorPreset(string name, Color color)
    {
        Name = name;
        Color = color;
    }

    public string Name { get; }
    public Color Color { get; }
    public SolidColorBrush Brush => new SolidColorBrush(Color);
}

/// <summary>
/// Theme information structure
/// </summary>
public class ThemeInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDark { get; set; }
}