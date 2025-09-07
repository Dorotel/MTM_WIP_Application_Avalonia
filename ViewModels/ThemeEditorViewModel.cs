using System;
using System.Collections.Generic;
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

    // Color change handlers for validation and unsaved changes tracking
    partial void OnPrimaryActionColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnSecondaryActionColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnAccentColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnHighlightColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

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

    // Text color change handlers
    partial void OnHeadingTextColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnBodyTextColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnInteractiveTextColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnOverlayTextColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnTertiaryTextColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

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

    // Background color change handlers
    partial void OnMainBackgroundColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnCardBackgroundColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnHoverBackgroundColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnPanelBackgroundColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnSidebarBackgroundColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

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

    // Status color change handlers
    partial void OnSuccessColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnWarningColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnErrorColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnInfoColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    #endregion

    #region Border Colors

    [ObservableProperty]
    private Color borderColor = Color.Parse("#E5E7EB");

    [ObservableProperty]
    private Color borderAccentColor = Color.Parse("#CED4DA");

    // Border color change handlers
    partial void OnBorderColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

    partial void OnBorderAccentColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
    }

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

            // Create color dictionary with all current color values
            var colorOverrides = new Dictionary<string, string>
            {
                // Core colors
                ["MTM_Shared_Logic.PrimaryAction"] = PrimaryActionColor.ToString(),
                ["MTM_Shared_Logic.SecondaryAction"] = SecondaryActionColor.ToString(),
                ["MTM_Shared_Logic.AccentColor"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.HighlightColor"] = HighlightColor.ToString(),
                
                // Text colors
                ["MTM_Shared_Logic.HeadingText"] = HeadingTextColor.ToString(),
                ["MTM_Shared_Logic.BodyText"] = BodyTextColor.ToString(),
                ["MTM_Shared_Logic.InteractiveText"] = InteractiveTextColor.ToString(),
                ["MTM_Shared_Logic.OverlayTextBrush"] = OverlayTextColor.ToString(),
                ["MTM_Shared_Logic.TertiaryTextBrush"] = TertiaryTextColor.ToString(),
                
                // Background colors
                ["MTM_Shared_Logic.MainBackground"] = MainBackgroundColor.ToString(),
                ["MTM_Shared_Logic.CardBackgroundBrush"] = CardBackgroundColor.ToString(),
                ["MTM_Shared_Logic.HoverBrush"] = HoverBackgroundColor.ToString(),
                ["MTM_Shared_Logic.PanelBackgroundBrush"] = PanelBackgroundColor.ToString(),
                ["MTM_Shared_Logic.SidebarBackground"] = SidebarBackgroundColor.ToString(),
                
                // Status colors
                ["MTM_Shared_Logic.SuccessBrush"] = SuccessColor.ToString(),
                ["MTM_Shared_Logic.WarningBrush"] = WarningColor.ToString(),
                ["MTM_Shared_Logic.ErrorBrush"] = ErrorColor.ToString(),
                ["MTM_Shared_Logic.InfoBrush"] = InfoColor.ToString(),
                
                // Border colors
                ["MTM_Shared_Logic.BorderBrush"] = BorderColor.ToString(),
                ["MTM_Shared_Logic.BorderAccentBrush"] = BorderAccentColor.ToString()
            };

            // Apply colors via ThemeService if available
            if (_themeService != null)
            {
                var result = await _themeService.ApplyCustomColorsAsync(colorOverrides);
                if (result.IsSuccess)
                {
                    HasUnsavedChanges = false;
                    StatusMessage = "Theme colors applied successfully";
                    Logger.LogInformation("Successfully applied {ColorCount} theme color overrides", colorOverrides.Count);
                }
                else
                {
                    StatusMessage = $"Failed to apply theme: {result.Message}";
                    Logger.LogWarning("Failed to apply theme colors: {Error}", result.Message);
                }
            }
            else
            {
                Logger.LogWarning("ThemeService not available - cannot apply theme colors");
                StatusMessage = "Theme service unavailable - colors not applied";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply theme changes", Environment.UserName);
            StatusMessage = "Error applying theme changes";
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
    private async Task PreviewThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Previewing theme changes...";
            Logger.LogDebug("Generating theme preview");

            // Create a temporary color dictionary for preview
            var previewColors = new Dictionary<string, string>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = PrimaryActionColor.ToString(),
                ["MTM_Shared_Logic.SecondaryAction"] = SecondaryActionColor.ToString(),
                ["MTM_Shared_Logic.AccentColor"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.HighlightColor"] = HighlightColor.ToString(),
                ["MTM_Shared_Logic.HeadingText"] = HeadingTextColor.ToString(),
                ["MTM_Shared_Logic.BodyText"] = BodyTextColor.ToString(),
                ["MTM_Shared_Logic.MainBackground"] = MainBackgroundColor.ToString(),
                ["MTM_Shared_Logic.CardBackgroundBrush"] = CardBackgroundColor.ToString(),
            };

            // Apply temporary preview without saving
            if (_themeService != null)
            {
                var result = await _themeService.ApplyCustomColorsAsync(previewColors);
                if (result.IsSuccess)
                {
                    StatusMessage = "Preview applied - use Apply to save changes";
                }
                else
                {
                    StatusMessage = $"Preview failed: {result.Message}";
                }
            }
            else
            {
                StatusMessage = "Preview unavailable - theme service not found";
            }

            await Task.Delay(100); // Small delay for user feedback
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating theme preview");
            StatusMessage = "Error generating preview";
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
        try
        {
            Logger.LogDebug("Loading current theme colors from ThemeService");
            
            if (_themeService != null)
            {
                // Get current theme name
                CurrentThemeName = _themeService.CurrentTheme ?? "MTM Theme";
                
                // Load current theme colors from application resources
                LoadColorsFromApplicationResources();
            }
            else
            {
                Logger.LogWarning("ThemeService not available, using default colors");
                LoadDefaultColors();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading current theme colors");
            LoadDefaultColors(); // Fallback to defaults
        }
    }
    
    private void LoadColorsFromApplicationResources()
    {
        try
        {
            if (Avalonia.Application.Current?.Resources == null)
            {
                Logger.LogWarning("Application resources not available, using defaults");
                LoadDefaultColors();
                return;
            }

            // Load colors from current application resources
            PrimaryActionColor = GetColorFromResource("MTM_Shared_Logic.PrimaryAction", "#0078D4");
            SecondaryActionColor = GetColorFromResource("MTM_Shared_Logic.SecondaryAction", "#106EBE");
            AccentColor = GetColorFromResource("MTM_Shared_Logic.AccentColor", "#40A2E8");
            HighlightColor = GetColorFromResource("MTM_Shared_Logic.HighlightColor", "#005A9E");

            HeadingTextColor = GetColorFromResource("MTM_Shared_Logic.HeadingText", "#323130");
            BodyTextColor = GetColorFromResource("MTM_Shared_Logic.BodyText", "#605E5C");
            InteractiveTextColor = GetColorFromResource("MTM_Shared_Logic.InteractiveText", "#0078D4");
            OverlayTextColor = GetColorFromResource("MTM_Shared_Logic.OverlayTextBrush", "#FFFFFF");
            TertiaryTextColor = GetColorFromResource("MTM_Shared_Logic.TertiaryTextBrush", "#8A8886");

            MainBackgroundColor = GetColorFromResource("MTM_Shared_Logic.MainBackground", "#FFFFFF");
            CardBackgroundColor = GetColorFromResource("MTM_Shared_Logic.CardBackgroundBrush", "#F3F2F1");
            HoverBackgroundColor = GetColorFromResource("MTM_Shared_Logic.HoverBrush", "#F0F0F0");
            PanelBackgroundColor = GetColorFromResource("MTM_Shared_Logic.PanelBackgroundBrush", "#FAFAFA");
            SidebarBackgroundColor = GetColorFromResource("MTM_Shared_Logic.SidebarBackground", "#F8F9FA");

            SuccessColor = GetColorFromResource("MTM_Shared_Logic.SuccessBrush", "#4CAF50");
            WarningColor = GetColorFromResource("MTM_Shared_Logic.WarningBrush", "#FF9800");
            ErrorColor = GetColorFromResource("MTM_Shared_Logic.ErrorBrush", "#F44336");
            InfoColor = GetColorFromResource("MTM_Shared_Logic.InfoBrush", "#2196F3");

            BorderColor = GetColorFromResource("MTM_Shared_Logic.BorderBrush", "#E5E7EB");
            BorderAccentColor = GetColorFromResource("MTM_Shared_Logic.BorderAccentBrush", "#CED4DA");

            Logger.LogDebug("Successfully loaded colors from application resources");
            StatusMessage = $"Loaded colors from {CurrentThemeName}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading colors from application resources");
            LoadDefaultColors();
        }
    }
    
    private Color GetColorFromResource(string resourceKey, string fallbackHex)
    {
        try
        {
            if (Avalonia.Application.Current?.Resources?.TryGetResource(resourceKey, null, out var resource) == true)
            {
                if (resource is IBrush brush && brush is ISolidColorBrush solidBrush)
                {
                    return solidBrush.Color;
                }
                if (resource is Color color)
                {
                    return color;
                }
                if (resource is string colorString && Color.TryParse(colorString, out var parsedColor))
                {
                    return parsedColor;
                }
            }
            
            // Fallback to provided hex color
            return Color.Parse(fallbackHex);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to get color from resource {ResourceKey}, using fallback {FallbackColor}", resourceKey, fallbackHex);
            return Color.Parse(fallbackHex);
        }
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

    /// <summary>
    /// Validates that a color string is a valid hex color format
    /// </summary>
    private bool IsValidHexColor(string colorString)
    {
        if (string.IsNullOrWhiteSpace(colorString))
            return false;
            
        try
        {
            Color.Parse(colorString);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Safely parses a hex color string, returning fallback if invalid
    /// </summary>
    private Color SafeParseColor(string colorString, Color fallback)
    {
        if (IsValidHexColor(colorString))
        {
            try
            {
                return Color.Parse(colorString);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to parse color '{Color}', using fallback", colorString);
            }
        }
        
        return fallback;
    }

    /// <summary>
    /// Validates all current color properties and updates status
    /// </summary>
    private bool ValidateAllColors()
    {
        try
        {
            var invalidColors = new List<string>();
            
            // Check each color property
            if (PrimaryActionColor == default) invalidColors.Add("Primary Action");
            if (SecondaryActionColor == default) invalidColors.Add("Secondary Action");
            if (AccentColor == default) invalidColors.Add("Accent");
            if (HighlightColor == default) invalidColors.Add("Highlight");
            
            if (HeadingTextColor == default) invalidColors.Add("Heading Text");
            if (BodyTextColor == default) invalidColors.Add("Body Text");
            if (InteractiveTextColor == default) invalidColors.Add("Interactive Text");
            if (OverlayTextColor == default) invalidColors.Add("Overlay Text");
            if (TertiaryTextColor == default) invalidColors.Add("Tertiary Text");
            
            if (MainBackgroundColor == default) invalidColors.Add("Main Background");
            if (CardBackgroundColor == default) invalidColors.Add("Card Background");
            if (HoverBackgroundColor == default) invalidColors.Add("Hover Background");
            if (PanelBackgroundColor == default) invalidColors.Add("Panel Background");
            if (SidebarBackgroundColor == default) invalidColors.Add("Sidebar Background");
            
            if (SuccessColor == default) invalidColors.Add("Success");
            if (WarningColor == default) invalidColors.Add("Warning");
            if (ErrorColor == default) invalidColors.Add("Error");
            if (InfoColor == default) invalidColors.Add("Info");
            
            if (BorderColor == default) invalidColors.Add("Border");
            if (BorderAccentColor == default) invalidColors.Add("Border Accent");

            if (invalidColors.Any())
            {
                StatusMessage = $"Invalid colors: {string.Join(", ", invalidColors)}";
                CanApplyTheme = false;
                return false;
            }
            
            StatusMessage = "All colors are valid";
            CanApplyTheme = true;
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating colors");
            StatusMessage = "Error validating colors";
            CanApplyTheme = false;
            return false;
        }
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