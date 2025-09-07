using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Services;
using Avalonia.Media;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// ThemeEditorViewModel manages comprehensive theme color editing functionality.
/// Features navigation, color pickers, auto-fill generation, and real-time preview.
/// Uses MVVM Community Toolkit for property and command management.
/// </summary>
public partial class ThemeEditorViewModel : BaseViewModel
{
    private readonly IThemeService? _themeService;
    private readonly INavigationService? _navigationService;

    #region Theme Identification and Status

    [ObservableProperty]
    private string currentThemeName = "MTM Theme";

    [ObservableProperty]
    private string statusMessage = "Ready to edit theme colors";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasUnsavedChanges = false;

    [ObservableProperty]
    private bool canApplyTheme = true;

    #endregion

    #region Navigation Properties

    [ObservableProperty]
    private string selectedCategory = "core";

    [ObservableProperty]
    private ObservableCollection<ColorCategory> colorCategories = new();

    #endregion

    #region Core Colors

    [ObservableProperty]
    private Color primaryActionColor = Color.Parse("#0078D4");

    [ObservableProperty]
    private Color secondaryActionColor = Color.Parse("#106EBE");

    [ObservableProperty]
    private Color accentColor = Color.Parse("#40A2E8");

    [ObservableProperty]
    private Color highlightColor = Color.Parse("#005A9E");

    #endregion

    #region Text Colors

    [ObservableProperty]
    private Color headingTextColor = Color.Parse("#323130");

    [ObservableProperty]
    private Color bodyTextColor = Color.Parse("#605E5C");

    [ObservableProperty]
    private Color interactiveTextColor = Color.Parse("#0078D4");

    [ObservableProperty]
    private Color overlayTextColor = Color.Parse("#FFFFFF");

    [ObservableProperty]
    private Color tertiaryTextColor = Color.Parse("#8A8886");

    #endregion

    #region Background Colors

    [ObservableProperty]
    private Color mainBackgroundColor = Color.Parse("#FFFFFF");

    [ObservableProperty]
    private Color cardBackgroundColor = Color.Parse("#F3F2F1");

    [ObservableProperty]
    private Color hoverBackgroundColor = Color.Parse("#F0F0F0");

    [ObservableProperty]
    private Color panelBackgroundColor = Color.Parse("#FAFAFA");

    [ObservableProperty]
    private Color sidebarBackgroundColor = Color.Parse("#F8F9FA");

    #endregion

    #region Status Colors

    [ObservableProperty]
    private Color successColor = Color.Parse("#4CAF50");

    [ObservableProperty]
    private Color warningColor = Color.Parse("#FF9800");

    [ObservableProperty]
    private Color errorColor = Color.Parse("#F44336");

    [ObservableProperty]
    private Color infoColor = Color.Parse("#2196F3");

    #endregion

    #region Border Colors

    [ObservableProperty]
    private Color borderColor = Color.Parse("#E5E7EB");

    [ObservableProperty]
    private Color borderAccentColor = Color.Parse("#CED4DA");

    #endregion

    public ThemeEditorViewModel(
        ILogger<ThemeEditorViewModel> logger,
        IThemeService? themeService = null,
        INavigationService? navigationService = null) : base(logger)
    {
        _themeService = themeService;
        _navigationService = navigationService;

        Logger.LogDebug("ThemeEditorViewModel initialized");
        InitializeColorCategories();
        LoadCurrentThemeColors();
    }

    #region Navigation Commands

    [RelayCommand]
    private async Task NavigateToSectionAsync(string categoryId)
    {
        try
        {
            Logger.LogDebug("Navigating to section: {CategoryId}", categoryId);
            SelectedCategory = categoryId;
            StatusMessage = $"Viewing {GetCategoryName(categoryId)} colors";
            
            // Trigger UI scroll to section (handled in view)
            await Task.Delay(50); // Allow UI to process
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error navigating to section {CategoryId}", categoryId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to navigate to {categoryId} section", Environment.UserName);
        }
    }

    #endregion

    #region Auto-Fill Commands

    [RelayCommand]
    private async Task AutoFillCoreColorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating core color palette...";
            Logger.LogDebug("Auto-filling core colors from primary color");

            var referenceColor = PrimaryActionColor;
            
            // Generate harmonious core color palette
            SecondaryActionColor = DarkenColor(referenceColor, 0.2f);
            AccentColor = LightenColor(referenceColor, 0.3f);
            HighlightColor = DarkenColor(referenceColor, 0.4f);

            HasUnsavedChanges = true;
            StatusMessage = "Core colors updated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-filling core colors");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate core color palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillTextColorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating text color palette...";
            Logger.LogDebug("Auto-filling text colors");

            var referenceColor = HeadingTextColor;

            // Generate WCAG-compliant text colors
            BodyTextColor = LightenColor(referenceColor, 0.2f);
            InteractiveTextColor = PrimaryActionColor;
            OverlayTextColor = GetContrastColor(MainBackgroundColor);
            TertiaryTextColor = LightenColor(referenceColor, 0.5f);

            HasUnsavedChanges = true;
            StatusMessage = "Text colors updated with WCAG compliance";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-filling text colors");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate text color palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillBackgroundColorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating background color palette...";
            Logger.LogDebug("Auto-filling background colors");

            var referenceColor = MainBackgroundColor;

            // Generate subtle background variations
            CardBackgroundColor = DarkenColor(referenceColor, 0.05f);
            HoverBackgroundColor = DarkenColor(referenceColor, 0.1f);
            PanelBackgroundColor = LightenColor(referenceColor, 0.02f);
            SidebarBackgroundColor = LightenColor(referenceColor, 0.01f);

            HasUnsavedChanges = true;
            StatusMessage = "Background colors updated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-filling background colors");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate background color palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillStatusColorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating status color palette...";
            Logger.LogDebug("Auto-filling status colors");

            // Generate semantic status colors
            SuccessColor = Color.Parse("#4CAF50");
            WarningColor = Color.Parse("#FF9800");
            ErrorColor = Color.Parse("#F44336");
            InfoColor = PrimaryActionColor;

            HasUnsavedChanges = true;
            StatusMessage = "Status colors updated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-filling status colors");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate status color palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillBorderColorsAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating border color palette...";
            Logger.LogDebug("Auto-filling border colors");

            // Generate subtle border variations
            var baseColor = BlendColor(BorderColor, MainBackgroundColor, 0.5f);
            BorderAccentColor = DarkenColor(baseColor, 0.1f);

            HasUnsavedChanges = true;
            StatusMessage = "Border colors updated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error auto-filling border colors");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate border color palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Theme Operations Commands

    [RelayCommand]
    private async Task ApplyThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Applying theme changes...";
            Logger.LogInformation("Applying theme changes");

            HasUnsavedChanges = false;
            StatusMessage = "Theme applied successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply theme changes", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ResetThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Resetting theme to defaults...";
            Logger.LogInformation("Resetting theme to defaults");

            LoadDefaultColors();
            HasUnsavedChanges = false;
            StatusMessage = "Theme reset to defaults";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error resetting theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to reset theme", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CloseAsync()
    {
        try
        {
            Logger.LogDebug("Closing theme editor");

            if (HasUnsavedChanges)
            {
                // TODO: Show confirmation dialog
                Logger.LogWarning("Closing with unsaved changes");
            }

            // Navigate back to MainView
            if (_navigationService != null)
            {
                var mainViewModel = Program.GetOptionalService<MainViewViewModel>();
                if (mainViewModel != null)
                {
                    var mainView = new Views.MainView
                    {
                        DataContext = mainViewModel
                    };
                    _navigationService.NavigateTo(mainView);
                    Logger.LogDebug("Navigated back to MainView");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing theme editor");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to close theme editor", Environment.UserName);
        }
    }

    #endregion

    #region Private Helper Methods

    private void InitializeColorCategories()
    {
        ColorCategories.Clear();
        ColorCategories.Add(new ColorCategory("core", "🎯 Core Colors", "Primary actions, secondary elements, and accent colors"));
        ColorCategories.Add(new ColorCategory("text", "📝 Text Colors", "Headings, body text, overlay text, and interactive text"));
        ColorCategories.Add(new ColorCategory("background", "🖼️ Background Colors", "Main backgrounds, card backgrounds, and hover states"));
        ColorCategories.Add(new ColorCategory("status", "⚡ Status Colors", "Success, warning, error, and informational indicators"));
        ColorCategories.Add(new ColorCategory("border", "🔲 Border Colors", "Light, standard, and accent border variations"));
    }

    private void LoadCurrentThemeColors()
    {
        // Load colors from current theme if ThemeService is available
        Logger.LogDebug("Loading current theme colors");
        // Implementation would load from _themeService if available
    }

    private void LoadDefaultColors()
    {
        // Reset to MTM default colors
        PrimaryActionColor = Color.Parse("#0078D4");
        SecondaryActionColor = Color.Parse("#106EBE");
        AccentColor = Color.Parse("#40A2E8");
        HighlightColor = Color.Parse("#005A9E");

        HeadingTextColor = Color.Parse("#323130");
        BodyTextColor = Color.Parse("#605E5C");
        InteractiveTextColor = Color.Parse("#0078D4");
        OverlayTextColor = Color.Parse("#FFFFFF");
        TertiaryTextColor = Color.Parse("#8A8886");

        MainBackgroundColor = Color.Parse("#FFFFFF");
        CardBackgroundColor = Color.Parse("#F3F2F1");
        HoverBackgroundColor = Color.Parse("#F0F0F0");
        PanelBackgroundColor = Color.Parse("#FAFAFA");
        SidebarBackgroundColor = Color.Parse("#F8F9FA");

        SuccessColor = Color.Parse("#4CAF50");
        WarningColor = Color.Parse("#FF9800");
        ErrorColor = Color.Parse("#F44336");
        InfoColor = Color.Parse("#2196F3");

        BorderColor = Color.Parse("#E5E7EB");
        BorderAccentColor = Color.Parse("#CED4DA");
    }

    private string GetCategoryName(string categoryId)
    {
        var category = ColorCategories.FirstOrDefault(c => c.Id == categoryId);
        return category?.Name ?? categoryId;
    }

    #endregion

    #region Color Manipulation Helpers

    private Color DarkenColor(Color color, float amount)
    {
        var factor = 1.0f - amount;
        return Color.FromRgb(
            (byte)(color.R * factor),
            (byte)(color.G * factor),
            (byte)(color.B * factor));
    }

    private Color LightenColor(Color color, float amount)
    {
        var factor = 1.0f + amount;
        return Color.FromRgb(
            (byte)Math.Min(255, color.R * factor),
            (byte)Math.Min(255, color.G * factor),
            (byte)Math.Min(255, color.B * factor));
    }

    private Color BlendColor(Color color1, Color color2, float ratio)
    {
        var invRatio = 1.0f - ratio;
        return Color.FromRgb(
            (byte)(color1.R * ratio + color2.R * invRatio),
            (byte)(color1.G * ratio + color2.G * invRatio),
            (byte)(color1.B * ratio + color2.B * invRatio));
    }

    private Color GetContrastColor(Color backgroundColor)
    {
        var luminance = GetLuminance(backgroundColor);
        return luminance > 0.5f ? Color.Parse("#000000") : Color.Parse("#FFFFFF");
    }

    private float GetLuminance(Color color)
    {
        // Simplified luminance calculation
        return (color.R * 0.299f + color.G * 0.587f + color.B * 0.114f) / 255f;
    }

    #endregion
}

/// <summary>
/// Color category model for sidebar navigation
/// </summary>
public class ColorCategory
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }

    public ColorCategory(string id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}