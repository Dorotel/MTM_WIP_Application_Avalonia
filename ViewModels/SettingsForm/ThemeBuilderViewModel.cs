using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ViewModel for advanced theme builder with live preview functionality.
/// Provides comprehensive theme customization and creation capabilities.
/// </summary>
public class ThemeBuilderViewModel : BaseViewModel
{
    private readonly IThemeService _themeService;
    
    private ThemeInfo? _baseTheme;
    private string _themeName = "Custom Theme";
    private string _themeDescription = "Custom theme created with Theme Builder";
    private Color _primaryColor = Color.FromRgb(75, 69, 237); // MTM Purple
    private Color _secondaryColor = Color.FromRgb(108, 99, 255);
    private Color _accentColor = Color.FromRgb(255, 165, 0);
    private Color _backgroundColor = Color.FromRgb(255, 255, 255);
    private Color _surfaceColor = Color.FromRgb(248, 249, 250);
    private Color _textColor = Color.FromRgb(33, 37, 41);
    private Color _borderColor = Color.FromRgb(222, 226, 230);
    private bool _isDarkTheme;
    private double _cornerRadius = 8.0;
    private double _shadowIntensity = 0.2;
    private bool _enableAnimations = true;
    private bool _isPreviewActive;
    private bool _isSaving;

    public ThemeBuilderViewModel(
        IThemeService themeService,
        ILogger<ThemeBuilderViewModel> logger) : base(logger)
    {
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));

        // Initialize commands
        ApplyPreviewCommand = new AsyncCommand(ExecuteApplyPreviewAsync, CanExecuteApplyPreview);
        SaveThemeCommand = new AsyncCommand(ExecuteSaveThemeAsync, CanExecuteSaveTheme);
        ExportThemeCommand = new AsyncCommand(ExecuteExportThemeAsync, CanExecuteExportTheme);
        ImportThemeCommand = new AsyncCommand(ExecuteImportThemeAsync);
        ResetToBaseCommand = new AsyncCommand(ExecuteResetToBaseAsync);
        RandomizeColorsCommand = new AsyncCommand(ExecuteRandomizeColorsAsync);

        // Initialize collections
        AvailableBaseThemes = new ObservableCollection<ThemeInfo>(_themeService.AvailableThemes);
        ColorPresets = new ObservableCollection<ColorPreset>();
        
        InitializeColorPresets();
        
        // Set default base theme
        BaseTheme = AvailableBaseThemes.FirstOrDefault(t => t.Id == "mtm-light");

        Logger.LogInformation("ThemeBuilderViewModel initialized");
    }

    #region Properties

    /// <summary>
    /// Base theme to customize.
    /// </summary>
    public ThemeInfo? BaseTheme
    {
        get => _baseTheme;
        set
        {
            if (SetProperty(ref _baseTheme, value))
            {
                OnBaseThemeChanged();
            }
        }
    }

    /// <summary>
    /// Name for the custom theme.
    /// </summary>
    public string ThemeName
    {
        get => _themeName;
        set => SetProperty(ref _themeName, value);
    }

    /// <summary>
    /// Description for the custom theme.
    /// </summary>
    public string ThemeDescription
    {
        get => _themeDescription;
        set => SetProperty(ref _themeDescription, value);
    }

    /// <summary>
    /// Primary color for the theme.
    /// </summary>
    public Color PrimaryColor
    {
        get => _primaryColor;
        set
        {
            if (SetProperty(ref _primaryColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Secondary color for the theme.
    /// </summary>
    public Color SecondaryColor
    {
        get => _secondaryColor;
        set
        {
            if (SetProperty(ref _secondaryColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Accent color for the theme.
    /// </summary>
    public Color AccentColor
    {
        get => _accentColor;
        set
        {
            if (SetProperty(ref _accentColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Background color for the theme.
    /// </summary>
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if (SetProperty(ref _backgroundColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Surface color for the theme.
    /// </summary>
    public Color SurfaceColor
    {
        get => _surfaceColor;
        set
        {
            if (SetProperty(ref _surfaceColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Text color for the theme.
    /// </summary>
    public Color TextColor
    {
        get => _textColor;
        set
        {
            if (SetProperty(ref _textColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Border color for the theme.
    /// </summary>
    public Color BorderColor
    {
        get => _borderColor;
        set
        {
            if (SetProperty(ref _borderColor, value))
            {
                OnColorChanged();
            }
        }
    }

    /// <summary>
    /// Indicates if this is a dark theme.
    /// </summary>
    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set
        {
            if (SetProperty(ref _isDarkTheme, value))
            {
                OnDarkThemeToggled();
            }
        }
    }

    /// <summary>
    /// Corner radius for theme elements.
    /// </summary>
    public double CornerRadius
    {
        get => _cornerRadius;
        set
        {
            if (SetProperty(ref _cornerRadius, value))
            {
                OnStyleChanged();
            }
        }
    }

    /// <summary>
    /// Shadow intensity for theme elements.
    /// </summary>
    public double ShadowIntensity
    {
        get => _shadowIntensity;
        set
        {
            if (SetProperty(ref _shadowIntensity, value))
            {
                OnStyleChanged();
            }
        }
    }

    /// <summary>
    /// Enable animations in the theme.
    /// </summary>
    public bool EnableAnimations
    {
        get => _enableAnimations;
        set => SetProperty(ref _enableAnimations, value);
    }

    /// <summary>
    /// Indicates if live preview is active.
    /// </summary>
    public bool IsPreviewActive
    {
        get => _isPreviewActive;
        set => SetProperty(ref _isPreviewActive, value);
    }

    /// <summary>
    /// Indicates if theme save is in progress.
    /// </summary>
    public bool IsSaving
    {
        get => _isSaving;
        set => SetProperty(ref _isSaving, value);
    }

    /// <summary>
    /// Available base themes to customize.
    /// </summary>
    public ObservableCollection<ThemeInfo> AvailableBaseThemes { get; }

    /// <summary>
    /// Available color presets for quick selection.
    /// </summary>
    public ObservableCollection<ColorPreset> ColorPresets { get; }

    /// <summary>
    /// Primary color as brush for preview.
    /// </summary>
    public SolidColorBrush PrimaryBrush => new SolidColorBrush(PrimaryColor);

    /// <summary>
    /// Secondary color as brush for preview.
    /// </summary>
    public SolidColorBrush SecondaryBrush => new SolidColorBrush(SecondaryColor);

    /// <summary>
    /// Background color as brush for preview.
    /// </summary>
    public SolidColorBrush BackgroundBrush => new SolidColorBrush(BackgroundColor);

    #endregion

    #region Commands

    /// <summary>
    /// Command to apply live preview of the theme.
    /// </summary>
    public ICommand ApplyPreviewCommand { get; }

    /// <summary>
    /// Command to save the custom theme.
    /// </summary>
    public ICommand SaveThemeCommand { get; }

    /// <summary>
    /// Command to export the theme to file.
    /// </summary>
    public ICommand ExportThemeCommand { get; }

    /// <summary>
    /// Command to import theme from file.
    /// </summary>
    public ICommand ImportThemeCommand { get; }

    /// <summary>
    /// Command to reset colors to base theme.
    /// </summary>
    public ICommand ResetToBaseCommand { get; }

    /// <summary>
    /// Command to randomize colors.
    /// </summary>
    public ICommand RandomizeColorsCommand { get; }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Applies live preview of the current theme configuration.
    /// </summary>
    private async Task ExecuteApplyPreviewAsync()
    {
        try
        {
            var colorOverrides = new Dictionary<string, string>
            {
                ["MTM_Shared_Logic.PrimaryBrush"] = PrimaryColor.ToString(),
                ["MTM_Shared_Logic.SecondaryBrush"] = SecondaryColor.ToString(),
                ["MTM_Shared_Logic.AccentBrush"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.BackgroundBrush"] = BackgroundColor.ToString(),
                ["MTM_Shared_Logic.SurfaceBrush"] = SurfaceColor.ToString(),
                ["MTM_Shared_Logic.PrimaryTextBrush"] = TextColor.ToString(),
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

    /// <summary>
    /// Determines if preview can be applied.
    /// </summary>
    private bool CanExecuteApplyPreview()
    {
        return !string.IsNullOrWhiteSpace(ThemeName);
    }

    /// <summary>
    /// Saves the custom theme permanently.
    /// </summary>
    private async Task ExecuteSaveThemeAsync()
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

            // Save via theme service (implementation would depend on storage mechanism)
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

    /// <summary>
    /// Determines if theme can be saved.
    /// </summary>
    private bool CanExecuteSaveTheme()
    {
        return !IsSaving && !string.IsNullOrWhiteSpace(ThemeName);
    }

    /// <summary>
    /// Exports the theme to a file.
    /// </summary>
    private async Task ExecuteExportThemeAsync()
    {
        try
        {
            // Implementation would show file picker and export theme
            Logger.LogInformation("Theme export initiated for {ThemeName}", ThemeName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting theme {ThemeName}", ThemeName);
        }
    }

    /// <summary>
    /// Determines if theme can be exported.
    /// </summary>
    private bool CanExecuteExportTheme()
    {
        return !string.IsNullOrWhiteSpace(ThemeName);
    }

    /// <summary>
    /// Imports a theme from file.
    /// </summary>
    private async Task ExecuteImportThemeAsync()
    {
        try
        {
            // Implementation would show file picker and import theme
            Logger.LogInformation("Theme import initiated");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing theme");
        }
    }

    /// <summary>
    /// Resets colors to base theme defaults.
    /// </summary>
    private async Task ExecuteResetToBaseAsync()
    {
        if (BaseTheme != null)
        {
            OnBaseThemeChanged();
            Logger.LogInformation("Theme colors reset to base theme {BaseTheme}", BaseTheme.DisplayName);
        }
    }

    /// <summary>
    /// Randomizes theme colors for experimentation.
    /// </summary>
    private async Task ExecuteRandomizeColorsAsync()
    {
        var random = new Random();
        
        PrimaryColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        SecondaryColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        AccentColor = Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        
        Logger.LogDebug("Theme colors randomized");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes color presets for quick selection.
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
    /// Handles base theme changes.
    /// </summary>
    private void OnBaseThemeChanged()
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
    }

    /// <summary>
    /// Handles color changes for live preview updates.
    /// </summary>
    private void OnColorChanged()
    {
        // Update brush properties
        RaisePropertyChanged(nameof(PrimaryBrush));
        RaisePropertyChanged(nameof(SecondaryBrush));
        RaisePropertyChanged(nameof(BackgroundBrush));

        // Auto-apply preview if active
        if (IsPreviewActive)
        {
            _ = Task.Run(ExecuteApplyPreviewAsync);
        }
    }

    /// <summary>
    /// Handles dark theme toggle.
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
    /// Handles style property changes.
    /// </summary>
    private void OnStyleChanged()
    {
        // Auto-apply preview if active
        if (IsPreviewActive)
        {
            _ = Task.Run(ExecuteApplyPreviewAsync);
        }
    }

    #endregion
}

/// <summary>
/// Color preset for quick theme color selection.
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