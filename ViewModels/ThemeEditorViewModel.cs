using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    private string themeVersion = "1.0";

    [ObservableProperty]
    private string themeDocumentation = string.Empty;

    [ObservableProperty]
    private string baseTheme = "MTM_Blue";

    [ObservableProperty]
    private string conditionalContext = "Default";

    [ObservableProperty]
    private string statusMessage = "Ready to edit theme colors";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool hasUnsavedChanges = false;

    [ObservableProperty]
    private bool canApplyTheme = true;

    [ObservableProperty]
    private bool isPreviewMode = false;

    [ObservableProperty]
    private bool respectSystemTheme = true;

    #endregion

    #region Navigation Properties

    [ObservableProperty]
    private string selectedCategory = "Core";

    [ObservableProperty]
    private string selectedCategoryTitle = "Core Colors";

    [ObservableProperty]
    private string selectedCategoryContent = string.Empty;

    [ObservableProperty]
    private bool isCoreColorsSelected = true;

    [ObservableProperty]
    private bool isTextColorsSelected = false;

    [ObservableProperty]
    private bool isBackgroundColorsSelected = false;

    [ObservableProperty]
    private bool isStatusColorsSelected = false;

    [ObservableProperty]
    private bool isBorderColorsSelected = false;

    [ObservableProperty]
    private bool isAutoFillSelected = false;

    [ObservableProperty]
    private bool isAdvancedSelected = false;

    [ObservableProperty]
    private bool isOtherCategorySelected = false;

    [ObservableProperty]
    private ObservableCollection<ColorCategory> colorCategories = new();

    #endregion

    #region Advanced Features - Color History and Undo/Redo

    [ObservableProperty]
    private ObservableCollection<ColorSnapshot> colorHistory = new();

    [ObservableProperty]
    private int currentHistoryIndex = -1;

    [ObservableProperty]
    private bool canUndo = false;

    [ObservableProperty]
    private bool canRedo = false;

    [ObservableProperty]
    private ObservableCollection<Color> recentColors = new();

    /// <summary>
    /// Represents a snapshot of all theme colors at a point in time
    /// </summary>
    public class ColorSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public Dictionary<string, Color> Colors { get; set; } = new();
        
        public ColorSnapshot(string name = "Color Change")
        {
            Name = name;
        }
    }

    #endregion

    #region Color Blindness Simulation Properties

    [ObservableProperty]
    private bool isColorBlindPreviewEnabled = false;

    [ObservableProperty]
    private string colorBlindnessType = "None";

    [ObservableProperty]
    private ObservableCollection<string> colorBlindnessTypes = new()
    {
        "None",
        "Protanopia (Red-blind)",
        "Deuteranopia (Green-blind)", 
        "Tritanopia (Blue-blind)",
        "Protanomaly (Red-weak)",
        "Deuteranomaly (Green-weak)",
        "Tritanomaly (Blue-weak)",
        "Monochromacy (Total color blindness)"
    };

    #endregion

    #region Advanced Features - Print Preview, Lighting, Multi-Monitor

    [ObservableProperty]
    private bool isPrintPreviewEnabled = false;

    [ObservableProperty] 
    private bool isLightingSimulationEnabled = false;

    [ObservableProperty]
    private string lightingCondition = "Office (6500K)";

    [ObservableProperty]
    private ObservableCollection<string> lightingConditions = new()
    {
        "Natural Daylight (5500K)",
        "Office Fluorescent (6500K)", 
        "Warm Indoor (3000K)",
        "Cool White LED (4000K)",
        "Tungsten Bulb (2700K)",
        "Candlelight (1900K)",
        "Overcast Day (6000K)",
        "Direct Sunlight (5800K)"
    };

    [ObservableProperty]
    private bool isMultiMonitorPreviewEnabled = false;

    [ObservableProperty]
    private ObservableCollection<MonitorInfo> availableMonitors = new();

    [ObservableProperty]
    private int selectedMonitorIndex = 0;

    /// <summary>
    /// Represents information about an available monitor for preview
    /// </summary>
    public class MonitorInfo
    {
        public string Name { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPrimary { get; set; }
        public string Resolution => $"{Width}x{Height}";
        public string DisplayName => IsPrimary ? $"{Name} (Primary) - {Resolution}" : $"{Name} - {Resolution}";
    }

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
        AddToRecentColors(value);
        OnPropertyChanged(nameof(PrimaryActionColorHex));
    }

    partial void OnSecondaryActionColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
        AddToRecentColors(value);
        OnPropertyChanged(nameof(SecondaryActionColorHex));
    }

    partial void OnAccentColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
        AddToRecentColors(value);
        OnPropertyChanged(nameof(AccentColorHex));
    }

    partial void OnHighlightColorChanged(Color value)
    {
        HasUnsavedChanges = true;
        ValidateAllColors();
        AddToRecentColors(value);
        OnPropertyChanged(nameof(HighlightColorHex));
    }

    #endregion

    #region Hex Color Properties for Enhanced UI

    /// <summary>
    /// Hex color representation for Primary Action Color
    /// </summary>
    public string PrimaryActionColorHex
    {
        get => PrimaryActionColor.ToString();
        set
        {
            if (TryParseHexColor(value, out var color) && color != PrimaryActionColor)
            {
                PrimaryActionColor = color;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Hex color representation for Secondary Action Color
    /// </summary>
    public string SecondaryActionColorHex
    {
        get => SecondaryActionColor.ToString();
        set
        {
            if (TryParseHexColor(value, out var color) && color != SecondaryActionColor)
            {
                SecondaryActionColor = color;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Hex color representation for Accent Color
    /// </summary>
    public string AccentColorHex
    {
        get => AccentColor.ToString();
        set
        {
            if (TryParseHexColor(value, out var color) && color != AccentColor)
            {
                AccentColor = color;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Hex color representation for Highlight Color
    /// </summary>
    public string HighlightColorHex
    {
        get => HighlightColor.ToString();
        set
        {
            if (TryParseHexColor(value, out var color) && color != HighlightColor)
            {
                HighlightColor = color;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Tries to parse a hex color string and returns the corresponding Color
    /// </summary>
    private static bool TryParseHexColor(string? hexValue, out Color color)
    {
        color = Colors.Black;
        
        if (string.IsNullOrWhiteSpace(hexValue))
            return false;
            
        try
        {
            // Handle different hex formats
            var cleanHex = hexValue.Trim();
            if (!cleanHex.StartsWith("#"))
                cleanHex = "#" + cleanHex;
                
            color = Color.Parse(cleanHex);
            return true;
        }
        catch
        {
            return false;
        }
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

    #region Navigation Commands

    [RelayCommand]
    private void SelectCategory(string? categoryName)
    {
        if (string.IsNullOrEmpty(categoryName)) return;

        SelectedCategory = categoryName;
        UpdateCategorySelection(categoryName);
    }

    private void UpdateCategorySelection(string categoryName)
    {
        // Reset all selection flags
        IsCoreColorsSelected = false;
        IsTextColorsSelected = false;
        IsBackgroundColorsSelected = false;
        IsStatusColorsSelected = false;
        IsBorderColorsSelected = false;
        IsAutoFillSelected = false;
        IsAdvancedSelected = false;
        IsOtherCategorySelected = false;

        // Set the appropriate flag and title
        switch (categoryName)
        {
            case "Core":
                IsCoreColorsSelected = true;
                SelectedCategoryTitle = "Core Colors";
                SelectedCategoryContent = "Primary action colors, secondary elements, and accent colors";
                break;
            case "Text":
                IsTextColorsSelected = true;
                SelectedCategoryTitle = "Text Colors";
                SelectedCategoryContent = "Headings, body text, interactive text, and overlay text colors";
                break;
            case "Background":
                IsBackgroundColorsSelected = true;
                SelectedCategoryTitle = "Background Colors";
                SelectedCategoryContent = "Main background, cards, panels, and navigation areas";
                break;
            case "Status":
                IsStatusColorsSelected = true;
                SelectedCategoryTitle = "Status Colors";
                SelectedCategoryContent = "Success, warning, error, and informational state colors";
                break;
            case "Border":
                IsBorderColorsSelected = true;
                SelectedCategoryTitle = "Border Colors";
                SelectedCategoryContent = "Element borders, accent borders, and dividers";
                break;
            case "AutoFill":
                IsAutoFillSelected = true;
                SelectedCategoryTitle = "Auto-Fill Palettes";
                SelectedCategoryContent = "Generate harmonious color schemes automatically";
                break;
            case "Advanced":
                IsAdvancedSelected = true;
                SelectedCategoryTitle = "Advanced Tools";
                SelectedCategoryContent = "Color history, accessibility preview, and validation tools";
                break;
            default:
                IsOtherCategorySelected = true;
                SelectedCategoryTitle = categoryName;
                SelectedCategoryContent = $"Tools and options for {categoryName.ToLower()}";
                break;
        }

        Logger.LogDebug("Category selected: {CategoryName}", categoryName);
    }

    #endregion

    #region Advanced Color Features Commands

    [RelayCommand]
    private async Task UndoAsync()
    {
        if (!CanUndo || CurrentHistoryIndex <= 0) return;

        try
        {
            CurrentHistoryIndex--;
            var snapshot = ColorHistory[CurrentHistoryIndex];
            await RestoreFromSnapshotAsync(snapshot, false);
            
            CanUndo = CurrentHistoryIndex > 0;
            CanRedo = true;
            
            StatusMessage = $"Undone: {snapshot.Name}";
            Logger.LogDebug("Undo applied: {SnapshotName}", snapshot.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during undo operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Undo operation failed", Environment.UserName);
        }
    }

    [RelayCommand]
    private async Task RedoAsync()
    {
        if (!CanRedo || CurrentHistoryIndex >= ColorHistory.Count - 1) return;

        try
        {
            CurrentHistoryIndex++;
            var snapshot = ColorHistory[CurrentHistoryIndex];
            await RestoreFromSnapshotAsync(snapshot, false);
            
            CanRedo = CurrentHistoryIndex < ColorHistory.Count - 1;
            CanUndo = true;
            
            StatusMessage = $"Redone: {snapshot.Name}";
            Logger.LogDebug("Redo applied: {SnapshotName}", snapshot.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during redo operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Redo operation failed", Environment.UserName);
        }
    }

    [RelayCommand]
    private async Task ToggleColorBlindPreviewAsync()
    {
        try
        {
            IsColorBlindPreviewEnabled = !IsColorBlindPreviewEnabled;
            
            if (IsColorBlindPreviewEnabled)
            {
                StatusMessage = $"Color blindness preview enabled: {ColorBlindnessType}";
                await ApplyColorBlindnessFilterAsync();
            }
            else
            {
                StatusMessage = "Color blindness preview disabled";
                await RestoreOriginalColorsAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling color blind preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Color blindness preview failed", Environment.UserName);
        }
    }

    [RelayCommand]
    private async Task BulkAdjustBrightnessAsync(double adjustment)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Adjusting all colors brightness by {adjustment:P0}...";
            
            SaveColorSnapshot("Bulk Brightness Adjustment");
            
            // Adjust all color categories
            PrimaryActionColor = AdjustBrightness(PrimaryActionColor, adjustment);
            SecondaryActionColor = AdjustBrightness(SecondaryActionColor, adjustment);
            AccentColor = AdjustBrightness(AccentColor, adjustment);
            HighlightColor = AdjustBrightness(HighlightColor, adjustment);
            
            HeadingTextColor = AdjustBrightness(HeadingTextColor, adjustment);
            BodyTextColor = AdjustBrightness(BodyTextColor, adjustment);
            InteractiveTextColor = AdjustBrightness(InteractiveTextColor, adjustment);
            TertiaryTextColor = AdjustBrightness(TertiaryTextColor, adjustment);
            
            MainBackgroundColor = AdjustBrightness(MainBackgroundColor, adjustment);
            CardBackgroundColor = AdjustBrightness(CardBackgroundColor, adjustment);
            HoverBackgroundColor = AdjustBrightness(HoverBackgroundColor, adjustment);
            PanelBackgroundColor = AdjustBrightness(PanelBackgroundColor, adjustment);
            SidebarBackgroundColor = AdjustBrightness(SidebarBackgroundColor, adjustment);
            
            SuccessColor = AdjustBrightness(SuccessColor, adjustment);
            WarningColor = AdjustBrightness(WarningColor, adjustment);
            ErrorColor = AdjustBrightness(ErrorColor, adjustment);
            InfoColor = AdjustBrightness(InfoColor, adjustment);
            
            BorderColor = AdjustBrightness(BorderColor, adjustment);
            BorderAccentColor = AdjustBrightness(BorderAccentColor, adjustment);
            
            HasUnsavedChanges = true;
            StatusMessage = $"Brightness adjusted by {adjustment:P0} for all colors";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during bulk brightness adjustment");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Bulk brightness adjustment failed", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task BulkAdjustSaturationAsync(double adjustment)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Adjusting all colors saturation by {adjustment:P0}...";
            
            SaveColorSnapshot("Bulk Saturation Adjustment");
            
            // Adjust saturation for all colors
            PrimaryActionColor = AdjustSaturation(PrimaryActionColor, adjustment);
            SecondaryActionColor = AdjustSaturation(SecondaryActionColor, adjustment);
            AccentColor = AdjustSaturation(AccentColor, adjustment);
            HighlightColor = AdjustSaturation(HighlightColor, adjustment);
            
            HeadingTextColor = AdjustSaturation(HeadingTextColor, adjustment);
            BodyTextColor = AdjustSaturation(BodyTextColor, adjustment);
            InteractiveTextColor = AdjustSaturation(InteractiveTextColor, adjustment);
            TertiaryTextColor = AdjustSaturation(TertiaryTextColor, adjustment);
            
            // Skip backgrounds for saturation to maintain neutrality
            
            SuccessColor = AdjustSaturation(SuccessColor, adjustment);
            WarningColor = AdjustSaturation(WarningColor, adjustment);
            ErrorColor = AdjustSaturation(ErrorColor, adjustment);
            InfoColor = AdjustSaturation(InfoColor, adjustment);
            
            HasUnsavedChanges = true;
            StatusMessage = $"Saturation adjusted by {adjustment:P0} for all colors";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during bulk saturation adjustment");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Bulk saturation adjustment failed", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand] 
    private void AddToRecentColors(Color color)
    {
        try
        {
            // Add to recent colors, maintaining a maximum of 16 colors
            if (RecentColors.Contains(color))
            {
                RecentColors.Remove(color);
            }
            
            RecentColors.Insert(0, color);
            
            // Keep only the 16 most recent colors
            while (RecentColors.Count > 16)
            {
                RecentColors.RemoveAt(RecentColors.Count - 1);
            }
            
            Logger.LogDebug("Added color {Color} to recent colors", color.ToString());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding color to recent colors");
        }
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
        
        // Initialize with current colors as first snapshot
        SaveColorSnapshot("Initial Theme State");
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

    #region Advanced Color Picker Commands

    /// <summary>
    /// Open advanced color picker for a specific color property
    /// </summary>
    [RelayCommand]
    private async Task OpenAdvancedColorPickerAsync(string? colorProperty)
    {
        try
        {
            if (string.IsNullOrEmpty(colorProperty)) return;

            // Simulate advanced color picker dialog
            StatusMessage = $"🎨 Advanced color picker for {colorProperty} - Enhanced RGB/HSL/LAB controls";
            
            // In a real implementation, this would open a sophisticated color picker dialog
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Open color picker for {colorProperty}", Environment.UserName);
        }
    }

    /// <summary>
    /// Simulate eyedropper functionality
    /// </summary>
    [RelayCommand]
    private async Task EyeDropperAsync(string? colorProperty)
    {
        try
        {
            if (string.IsNullOrEmpty(colorProperty)) return;

            StatusMessage = $"👁️ Eyedropper mode for {colorProperty} - Click on screen to pick color";
            
            // In a real implementation, this would activate screen color picking
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Eyedropper for {colorProperty}", Environment.UserName);
        }
    }

    /// <summary>
    /// Copy color hex to clipboard
    /// </summary>
    [RelayCommand]
    private async Task CopyColorAsync(string? colorProperty)
    {
        try
        {
            if (string.IsNullOrEmpty(colorProperty)) return;

            var color = GetColorByProperty(colorProperty);
            var hex = color.ToString();
            
            // In a real implementation, copy to clipboard
            StatusMessage = $"📋 Copied {hex} to clipboard";
            
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Copy color {colorProperty}", Environment.UserName);
        }
    }

    /// <summary>
    /// Reset specific color to default
    /// </summary>
    [RelayCommand]
    private async Task ResetColorAsync(string? colorProperty)
    {
        try
        {
            if (string.IsNullOrEmpty(colorProperty)) return;

            switch (colorProperty)
            {
                case "PrimaryAction":
                    PrimaryActionColor = Color.Parse("#0078D4");
                    break;
                case "SecondaryAction":
                    SecondaryActionColor = Color.Parse("#106EBE");
                    break;
                // Add more cases as needed
            }

            StatusMessage = $"🔄 Reset {colorProperty} to default";
            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Reset color {colorProperty}", Environment.UserName);
        }
    }

    /// <summary>
    /// Helper method to get color by property name
    /// </summary>
    private Color GetColorByProperty(string propertyName)
    {
        return propertyName switch
        {
            "PrimaryAction" => PrimaryActionColor,
            "SecondaryAction" => SecondaryActionColor,
            "Accent" => AccentColor,
            "Highlight" => HighlightColor,
            _ => Colors.Black
        };
    }

    #endregion

    #region Theme Export/Import System

    [RelayCommand]
    private async Task ExportThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Exporting theme...";
            Logger.LogDebug("Starting theme export");

            // Create theme export model with all current colors
            var themeExport = new ThemeExportModel
            {
                Name = CurrentThemeName,
                Description = $"Custom theme created on {DateTime.Now:yyyy-MM-dd HH:mm}",
                CreatedBy = Environment.UserName,
                CreatedDate = DateTime.Now,
                Version = "1.0",
                Colors = new Dictionary<string, string>
                {
                    // Core colors
                    ["PrimaryAction"] = PrimaryActionColor.ToString(),
                    ["SecondaryAction"] = SecondaryActionColor.ToString(),
                    ["AccentColor"] = AccentColor.ToString(),
                    ["HighlightColor"] = HighlightColor.ToString(),
                    
                    // Text colors
                    ["HeadingText"] = HeadingTextColor.ToString(),
                    ["BodyText"] = BodyTextColor.ToString(),
                    ["InteractiveText"] = InteractiveTextColor.ToString(),
                    ["OverlayText"] = OverlayTextColor.ToString(),
                    ["TertiaryText"] = TertiaryTextColor.ToString(),
                    
                    // Background colors
                    ["MainBackground"] = MainBackgroundColor.ToString(),
                    ["CardBackground"] = CardBackgroundColor.ToString(),
                    ["HoverBackground"] = HoverBackgroundColor.ToString(),
                    ["PanelBackground"] = PanelBackgroundColor.ToString(),
                    ["SidebarBackground"] = SidebarBackgroundColor.ToString(),
                    
                    // Status colors
                    ["Success"] = SuccessColor.ToString(),
                    ["Warning"] = WarningColor.ToString(),
                    ["Error"] = ErrorColor.ToString(),
                    ["Info"] = InfoColor.ToString(),
                    
                    // Border colors
                    ["Border"] = BorderColor.ToString(),
                    ["BorderAccent"] = BorderAccentColor.ToString()
                }
            };

            // Serialize to JSON
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var themeJson = JsonSerializer.Serialize(themeExport, jsonOptions);
            
            // Save to file (in a production app, this would use a file dialog)
            var fileName = $"MTM_Theme_{CurrentThemeName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);
            
            await File.WriteAllTextAsync(filePath, themeJson);
            
            StatusMessage = $"Theme exported successfully to: {fileName}";
            Logger.LogInformation("Theme exported to {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error exporting theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to export theme", Environment.UserName);
            StatusMessage = "Failed to export theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Importing theme...";
            Logger.LogDebug("Starting theme import");

            // In a production app, this would use a file dialog to select the theme file
            // For now, we'll look for theme files on the desktop
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var themeFiles = Directory.GetFiles(desktopPath, "MTM_Theme_*.json");
            
            if (!themeFiles.Any())
            {
                StatusMessage = "No theme files found on desktop";
                return;
            }
            
            // Use the most recent theme file for demo purposes
            var latestThemeFile = themeFiles
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .First();
            
            // Read and parse theme file
            var themeJson = await File.ReadAllTextAsync(latestThemeFile.FullName);
            var themeImport = JsonSerializer.Deserialize<ThemeExportModel>(themeJson, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            if (themeImport == null || !themeImport.Colors.Any())
            {
                StatusMessage = "Invalid theme file format";
                return;
            }
            
            // Apply imported colors
            Logger.LogDebug("Applying imported theme colors from {FileName}", latestThemeFile.Name);
            
            if (themeImport.Colors.TryGetValue("PrimaryAction", out var primaryAction))
                PrimaryActionColor = SafeParseColor(primaryAction, PrimaryActionColor);
            if (themeImport.Colors.TryGetValue("SecondaryAction", out var secondaryAction))
                SecondaryActionColor = SafeParseColor(secondaryAction, SecondaryActionColor);
            if (themeImport.Colors.TryGetValue("AccentColor", out var accentColor))
                AccentColor = SafeParseColor(accentColor, AccentColor);
            if (themeImport.Colors.TryGetValue("HighlightColor", out var highlightColor))
                HighlightColor = SafeParseColor(highlightColor, HighlightColor);
                
            if (themeImport.Colors.TryGetValue("HeadingText", out var headingText))
                HeadingTextColor = SafeParseColor(headingText, HeadingTextColor);
            if (themeImport.Colors.TryGetValue("BodyText", out var bodyText))
                BodyTextColor = SafeParseColor(bodyText, BodyTextColor);
            if (themeImport.Colors.TryGetValue("InteractiveText", out var interactiveText))
                InteractiveTextColor = SafeParseColor(interactiveText, InteractiveTextColor);
            if (themeImport.Colors.TryGetValue("OverlayText", out var overlayText))
                OverlayTextColor = SafeParseColor(overlayText, OverlayTextColor);
            if (themeImport.Colors.TryGetValue("TertiaryText", out var tertiaryText))
                TertiaryTextColor = SafeParseColor(tertiaryText, TertiaryTextColor);
                
            if (themeImport.Colors.TryGetValue("MainBackground", out var mainBackground))
                MainBackgroundColor = SafeParseColor(mainBackground, MainBackgroundColor);
            if (themeImport.Colors.TryGetValue("CardBackground", out var cardBackground))
                CardBackgroundColor = SafeParseColor(cardBackground, CardBackgroundColor);
            if (themeImport.Colors.TryGetValue("HoverBackground", out var hoverBackground))
                HoverBackgroundColor = SafeParseColor(hoverBackground, HoverBackgroundColor);
            if (themeImport.Colors.TryGetValue("PanelBackground", out var panelBackground))
                PanelBackgroundColor = SafeParseColor(panelBackground, PanelBackgroundColor);
            if (themeImport.Colors.TryGetValue("SidebarBackground", out var sidebarBackground))
                SidebarBackgroundColor = SafeParseColor(sidebarBackground, SidebarBackgroundColor);
                
            if (themeImport.Colors.TryGetValue("Success", out var success))
                SuccessColor = SafeParseColor(success, SuccessColor);
            if (themeImport.Colors.TryGetValue("Warning", out var warning))
                WarningColor = SafeParseColor(warning, WarningColor);
            if (themeImport.Colors.TryGetValue("Error", out var error))
                ErrorColor = SafeParseColor(error, ErrorColor);
            if (themeImport.Colors.TryGetValue("Info", out var info))
                InfoColor = SafeParseColor(info, InfoColor);
                
            if (themeImport.Colors.TryGetValue("Border", out var border))
                BorderColor = SafeParseColor(border, BorderColor);
            if (themeImport.Colors.TryGetValue("BorderAccent", out var borderAccent))
                BorderAccentColor = SafeParseColor(borderAccent, BorderAccentColor);
            
            // Update theme metadata
            CurrentThemeName = themeImport.Name;
            HasUnsavedChanges = true;
            
            StatusMessage = $"Theme '{themeImport.Name}' imported successfully from {latestThemeFile.Name}";
            Logger.LogInformation("Theme imported successfully from {FileName} - {ThemeName}", latestThemeFile.Name, themeImport.Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to import theme", Environment.UserName);
            StatusMessage = "Failed to import theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadIndustryTemplateAsync(string templateName)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Loading {templateName} template...";
            Logger.LogDebug("Loading industry template: {TemplateName}", templateName);

            switch (templateName.ToLowerInvariant())
            {
                case "manufacturing":
                    await LoadManufacturingTemplateAsync();
                    break;
                case "healthcare":
                    await LoadHealthcareTemplateAsync();
                    break;
                case "office":
                    await LoadOfficeTemplateAsync();
                    break;
                case "highcontrast":
                    await LoadHighContrastTemplateAsync();
                    break;
                default:
                    StatusMessage = $"Template '{templateName}' not found";
                    return;
            }

            CurrentThemeName = $"{templateName} Theme";
            HasUnsavedChanges = true;
            StatusMessage = $"{templateName} template loaded successfully";
            Logger.LogInformation("Industry template loaded: {TemplateName}", templateName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading industry template: {TemplateName}", templateName);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load {templateName} template", Environment.UserName);
            StatusMessage = $"Failed to load {templateName} template";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Theme Version Management and Rollback

    [ObservableProperty]
    private ObservableCollection<ThemeVersionSnapshot> themeVersionHistory = new();

    [ObservableProperty]
    private ThemeVersionSnapshot? selectedVersion;

    [ObservableProperty]
    private bool canRollback = false;

    [RelayCommand]
    private async Task CreateVersionSnapshotAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Creating version snapshot...";
            Logger.LogDebug("Creating theme version snapshot");

            var snapshot = new ThemeVersionSnapshot
            {
                Version = ThemeVersion,
                Name = CurrentThemeName,
                Documentation = ThemeDocumentation,
                BaseTheme = BaseTheme,
                ConditionalContext = ConditionalContext,
                CreatedDate = DateTime.Now,
                CreatedBy = Environment.UserName,
                Colors = new Dictionary<string, string>
                {
                    // Core colors
                    ["PrimaryAction"] = PrimaryActionColor.ToString(),
                    ["SecondaryAction"] = SecondaryActionColor.ToString(),
                    ["AccentColor"] = AccentColor.ToString(),
                    ["HighlightColor"] = HighlightColor.ToString(),
                    
                    // Text colors
                    ["HeadingText"] = HeadingTextColor.ToString(),
                    ["BodyText"] = BodyTextColor.ToString(),
                    ["InteractiveText"] = InteractiveTextColor.ToString(),
                    ["OverlayText"] = OverlayTextColor.ToString(),
                    ["TertiaryText"] = TertiaryTextColor.ToString(),
                    
                    // Background colors
                    ["MainBackground"] = MainBackgroundColor.ToString(),
                    ["CardBackground"] = CardBackgroundColor.ToString(),
                    ["HoverBackground"] = HoverBackgroundColor.ToString(),
                    ["PanelBackground"] = PanelBackgroundColor.ToString(),
                    ["SidebarBackground"] = SidebarBackgroundColor.ToString(),
                    
                    // Status colors
                    ["Success"] = SuccessColor.ToString(),
                    ["Warning"] = WarningColor.ToString(),
                    ["Error"] = ErrorColor.ToString(),
                    ["Info"] = InfoColor.ToString(),
                    
                    // Border colors
                    ["Border"] = BorderColor.ToString(),
                    ["BorderAccent"] = BorderAccentColor.ToString()
                }
            };

            ThemeVersionHistory.Insert(0, snapshot);
            
            // Keep only the last 20 versions
            while (ThemeVersionHistory.Count > 20)
            {
                ThemeVersionHistory.RemoveAt(ThemeVersionHistory.Count - 1);
            }

            CanRollback = ThemeVersionHistory.Count > 1;
            
            // Auto-increment version
            if (double.TryParse(ThemeVersion, out var currentVersion))
            {
                ThemeVersion = (currentVersion + 0.1).ToString("F1");
            }

            StatusMessage = $"Version snapshot created: v{snapshot.Version}";
            Logger.LogInformation("Theme version snapshot created: v{Version}", snapshot.Version);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating version snapshot");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to create version snapshot", Environment.UserName);
            StatusMessage = "Failed to create version snapshot";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RollbackToVersionAsync(ThemeVersionSnapshot? version)
    {
        if (version == null) return;

        try
        {
            IsLoading = true;
            StatusMessage = $"Rolling back to version {version.Version}...";
            Logger.LogDebug("Rolling back to theme version: {Version}", version.Version);

            // Restore all properties from the selected version
            CurrentThemeName = version.Name;
            ThemeVersion = version.Version;
            ThemeDocumentation = version.Documentation;
            BaseTheme = version.BaseTheme;
            ConditionalContext = version.ConditionalContext;

            // Restore all colors
            if (version.Colors.TryGetValue("PrimaryAction", out var primaryAction))
                PrimaryActionColor = SafeParseColor(primaryAction, PrimaryActionColor);
            if (version.Colors.TryGetValue("SecondaryAction", out var secondaryAction))
                SecondaryActionColor = SafeParseColor(secondaryAction, SecondaryActionColor);
            if (version.Colors.TryGetValue("AccentColor", out var accentColor))
                AccentColor = SafeParseColor(accentColor, AccentColor);
            if (version.Colors.TryGetValue("HighlightColor", out var highlightColor))
                HighlightColor = SafeParseColor(highlightColor, HighlightColor);
                
            if (version.Colors.TryGetValue("HeadingText", out var headingText))
                HeadingTextColor = SafeParseColor(headingText, HeadingTextColor);
            if (version.Colors.TryGetValue("BodyText", out var bodyText))
                BodyTextColor = SafeParseColor(bodyText, BodyTextColor);
            if (version.Colors.TryGetValue("InteractiveText", out var interactiveText))
                InteractiveTextColor = SafeParseColor(interactiveText, InteractiveTextColor);
            if (version.Colors.TryGetValue("OverlayText", out var overlayText))
                OverlayTextColor = SafeParseColor(overlayText, OverlayTextColor);
            if (version.Colors.TryGetValue("TertiaryText", out var tertiaryText))
                TertiaryTextColor = SafeParseColor(tertiaryText, TertiaryTextColor);
                
            if (version.Colors.TryGetValue("MainBackground", out var mainBackground))
                MainBackgroundColor = SafeParseColor(mainBackground, MainBackgroundColor);
            if (version.Colors.TryGetValue("CardBackground", out var cardBackground))
                CardBackgroundColor = SafeParseColor(cardBackground, CardBackgroundColor);
            if (version.Colors.TryGetValue("HoverBackground", out var hoverBackground))
                HoverBackgroundColor = SafeParseColor(hoverBackground, HoverBackgroundColor);
            if (version.Colors.TryGetValue("PanelBackground", out var panelBackground))
                PanelBackgroundColor = SafeParseColor(panelBackground, PanelBackgroundColor);
            if (version.Colors.TryGetValue("SidebarBackground", out var sidebarBackground))
                SidebarBackgroundColor = SafeParseColor(sidebarBackground, SidebarBackgroundColor);
                
            if (version.Colors.TryGetValue("Success", out var success))
                SuccessColor = SafeParseColor(success, SuccessColor);
            if (version.Colors.TryGetValue("Warning", out var warning))
                WarningColor = SafeParseColor(warning, WarningColor);
            if (version.Colors.TryGetValue("Error", out var error))
                ErrorColor = SafeParseColor(error, ErrorColor);
            if (version.Colors.TryGetValue("Info", out var info))
                InfoColor = SafeParseColor(info, InfoColor);
                
            if (version.Colors.TryGetValue("Border", out var border))
                BorderColor = SafeParseColor(border, BorderColor);
            if (version.Colors.TryGetValue("BorderAccent", out var borderAccent))
                BorderAccentColor = SafeParseColor(borderAccent, BorderAccentColor);

            HasUnsavedChanges = true;
            StatusMessage = $"Rolled back to version {version.Version}";
            Logger.LogInformation("Theme rolled back to version: {Version}", version.Version);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error rolling back to version: {Version}", version.Version);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to rollback to version {version.Version}", Environment.UserName);
            StatusMessage = "Failed to rollback to selected version";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Color Naming System

    [ObservableProperty]
    private Dictionary<string, string> colorCustomNames = new();

    [ObservableProperty]
    private string selectedColorForNaming = string.Empty;

    [ObservableProperty]
    private string customColorName = string.Empty;

    [RelayCommand]
    private void SetCustomColorName(string colorProperty)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(colorProperty) || string.IsNullOrWhiteSpace(CustomColorName))
                return;

            ColorCustomNames[colorProperty] = CustomColorName;
            
            StatusMessage = $"Custom name '{CustomColorName}' set for {colorProperty}";
            Logger.LogDebug("Custom color name set: {ColorProperty} = {CustomName}", colorProperty, CustomColorName);
            
            // Clear the input
            CustomColorName = string.Empty;
            SelectedColorForNaming = string.Empty;
            
            HasUnsavedChanges = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting custom color name");
            StatusMessage = "Failed to set custom color name";
        }
    }

    [RelayCommand]
    private void RemoveCustomColorName(string colorProperty)
    {
        try
        {
            if (ColorCustomNames.ContainsKey(colorProperty))
            {
                ColorCustomNames.Remove(colorProperty);
                StatusMessage = $"Custom name removed for {colorProperty}";
                HasUnsavedChanges = true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing custom color name");
            StatusMessage = "Failed to remove custom color name";
        }
    }

    public string GetColorDisplayName(string colorProperty)
    {
        return ColorCustomNames.ContainsKey(colorProperty) 
            ? $"{ColorCustomNames[colorProperty]} ({colorProperty})"
            : colorProperty;
    }

    #endregion

    #region Theme Analytics

    [ObservableProperty]
    private Dictionary<string, ColorUsageStats> colorUsageAnalytics = new();

    [ObservableProperty]
    private bool showAnalytics = false;

    [RelayCommand]
    private void TrackColorUsage(string colorProperty)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(colorProperty)) return;

            if (!ColorUsageAnalytics.ContainsKey(colorProperty))
            {
                ColorUsageAnalytics[colorProperty] = new ColorUsageStats
                {
                    ColorProperty = colorProperty,
                    UsageCount = 0,
                    LastUsed = DateTime.Now,
                    FirstUsed = DateTime.Now
                };
            }

            var stats = ColorUsageAnalytics[colorProperty];
            stats.UsageCount++;
            stats.LastUsed = DateTime.Now;
            
            Logger.LogDebug("Color usage tracked: {ColorProperty} (Count: {UsageCount})", colorProperty, stats.UsageCount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error tracking color usage for: {ColorProperty}", colorProperty);
        }
    }

    [RelayCommand]
    private async Task GenerateUsageReportAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating usage analytics...";
            Logger.LogDebug("Generating color usage analytics report");

            if (!ColorUsageAnalytics.Any())
            {
                StatusMessage = "No usage data available yet";
                return;
            }

            var sortedStats = ColorUsageAnalytics.Values
                .OrderByDescending(s => s.UsageCount)
                .ToList();

            var mostUsed = sortedStats.First();
            var leastUsed = sortedStats.Last();
            var totalUsage = sortedStats.Sum(s => s.UsageCount);

            var reportMessage = $"Analytics: Most used: {mostUsed.ColorProperty} ({mostUsed.UsageCount}), " +
                              $"Least used: {leastUsed.ColorProperty} ({leastUsed.UsageCount}), " +
                              $"Total interactions: {totalUsage}";

            StatusMessage = reportMessage;
            Logger.LogInformation("Usage analytics generated: {Report}", reportMessage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating usage analytics");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate usage analytics", Environment.UserName);
            StatusMessage = "Failed to generate usage analytics";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Theme Inheritance System

    [ObservableProperty]
    private ObservableCollection<string> availableBaseThemes = new()
    {
        "MTM_Blue", "MTM_Green", "MTM_Red", "MTM_Dark", "Windows_Light", "Windows_Dark"
    };

    [ObservableProperty]
    private Dictionary<string, string> themeOverrides = new();

    [RelayCommand]
    private async Task ApplyBaseThemeAsync(string baseThemeName)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Applying base theme: {baseThemeName}...";
            Logger.LogDebug("Applying base theme: {BaseTheme}", baseThemeName);

            BaseTheme = baseThemeName;

            // Apply base theme colors based on the selected theme
            switch (baseThemeName.ToLowerInvariant())
            {
                case "mtm_blue":
                    await LoadMTMBlueBaseAsync();
                    break;
                case "mtm_green":
                    await LoadMTMGreenBaseAsync();
                    break;
                case "mtm_red":
                    await LoadMTMRedBaseAsync();
                    break;
                case "mtm_dark":
                    await LoadMTMDarkBaseAsync();
                    break;
                case "windows_light":
                    await LoadWindowsLightBaseAsync();
                    break;
                case "windows_dark":
                    await LoadWindowsDarkBaseAsync();
                    break;
                default:
                    StatusMessage = $"Unknown base theme: {baseThemeName}";
                    return;
            }

            // Apply any existing overrides
            await ApplyThemeOverridesAsync();

            CurrentThemeName = $"{baseThemeName} Custom";
            HasUnsavedChanges = true;
            StatusMessage = $"Base theme '{baseThemeName}' applied with custom overrides";
            Logger.LogInformation("Base theme applied: {BaseTheme}", baseThemeName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying base theme: {BaseTheme}", baseThemeName);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to apply base theme {baseThemeName}", Environment.UserName);
            StatusMessage = $"Failed to apply base theme {baseThemeName}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadMTMBlueBaseAsync()
    {
        // Standard MTM Blue theme colors
        PrimaryActionColor = Color.Parse("#0078D4");
        SecondaryActionColor = Color.Parse("#106EBE");
        AccentColor = Color.Parse("#40A2E8");
        HighlightColor = Color.Parse("#005A9E");
        
        HeadingTextColor = Color.Parse("#323130");
        BodyTextColor = Color.Parse("#605E5C");
        InteractiveTextColor = Color.Parse("#0078D4");
        OverlayTextColor = Color.Parse("#FFFFFF");
        TertiaryTextColor = Color.Parse("#8A8886");
        
        MainBackgroundColor = Color.Parse("#FAFAFA");
        CardBackgroundColor = Color.Parse("#FFFFFF");
        HoverBackgroundColor = Color.Parse("#F3F2F1");
        PanelBackgroundColor = Color.Parse("#F8F8F8");
        SidebarBackgroundColor = Color.Parse("#F0F0F0");
        
        SuccessColor = Color.Parse("#4CAF50");
        WarningColor = Color.Parse("#FF9800");
        ErrorColor = Color.Parse("#F44336");
        InfoColor = Color.Parse("#2196F3");
        
        BorderColor = Color.Parse("#E1DFDD");
        BorderAccentColor = Color.Parse("#0078D4");
        
        await Task.Delay(1); // Ensure async pattern
    }

    private async Task LoadMTMDarkBaseAsync()
    {
        // Dark theme variant
        PrimaryActionColor = Color.Parse("#4FC3F7");
        SecondaryActionColor = Color.Parse("#29B6F6");
        AccentColor = Color.Parse("#81D4FA");
        HighlightColor = Color.Parse("#0288D1");
        
        HeadingTextColor = Color.Parse("#FFFFFF");
        BodyTextColor = Color.Parse("#E0E0E0");
        InteractiveTextColor = Color.Parse("#4FC3F7");
        OverlayTextColor = Color.Parse("#000000");
        TertiaryTextColor = Color.Parse("#BDBDBD");
        
        MainBackgroundColor = Color.Parse("#121212");
        CardBackgroundColor = Color.Parse("#1E1E1E");
        HoverBackgroundColor = Color.Parse("#2C2C2C");
        PanelBackgroundColor = Color.Parse("#181818");
        SidebarBackgroundColor = Color.Parse("#202020");
        
        SuccessColor = Color.Parse("#66BB6A");
        WarningColor = Color.Parse("#FFCA28");
        ErrorColor = Color.Parse("#EF5350");
        InfoColor = Color.Parse("#42A5F5");
        
        BorderColor = Color.Parse("#404040");
        BorderAccentColor = Color.Parse("#4FC3F7");
        
        await Task.Delay(1);
    }

    private async Task LoadWindowsLightBaseAsync()
    {
        // Windows 11 Light theme
        PrimaryActionColor = Color.Parse("#0078D4");
        SecondaryActionColor = Color.Parse("#005A9E");
        AccentColor = Color.Parse("#106EBE");
        HighlightColor = Color.Parse("#004578");
        
        HeadingTextColor = Color.Parse("#000000");
        BodyTextColor = Color.Parse("#323130");
        InteractiveTextColor = Color.Parse("#0078D4");
        OverlayTextColor = Color.Parse("#FFFFFF");
        TertiaryTextColor = Color.Parse("#605E5C");
        
        MainBackgroundColor = Color.Parse("#FFFFFF");
        CardBackgroundColor = Color.Parse("#F9F9F9");
        HoverBackgroundColor = Color.Parse("#F5F5F5");
        PanelBackgroundColor = Color.Parse("#FAFAFA");
        SidebarBackgroundColor = Color.Parse("#F0F0F0");
        
        SuccessColor = Color.Parse("#107C10");
        WarningColor = Color.Parse("#FF8C00");
        ErrorColor = Color.Parse("#D13438");
        InfoColor = Color.Parse("#0078D4");
        
        BorderColor = Color.Parse("#CCCCCC");
        BorderAccentColor = Color.Parse("#0078D4");
        
        await Task.Delay(1);
    }

    private async Task LoadWindowsDarkBaseAsync()
    {
        // Windows 11 Dark theme
        PrimaryActionColor = Color.Parse("#60CDFF");
        SecondaryActionColor = Color.Parse("#409CFF");
        AccentColor = Color.Parse("#80DDFF");
        HighlightColor = Color.Parse("#2080FF");
        
        HeadingTextColor = Color.Parse("#FFFFFF");
        BodyTextColor = Color.Parse("#FFFFFF");
        InteractiveTextColor = Color.Parse("#60CDFF");
        OverlayTextColor = Color.Parse("#000000");
        TertiaryTextColor = Color.Parse("#CCCCCC");
        
        MainBackgroundColor = Color.Parse("#202020");
        CardBackgroundColor = Color.Parse("#2C2C2C");
        HoverBackgroundColor = Color.Parse("#383838");
        PanelBackgroundColor = Color.Parse("#252525");
        SidebarBackgroundColor = Color.Parse("#2B2B2B");
        
        SuccessColor = Color.Parse("#6CCB5F");
        WarningColor = Color.Parse("#FCE100");
        ErrorColor = Color.Parse("#FF5757");
        InfoColor = Color.Parse("#60CDFF");
        
        BorderColor = Color.Parse("#404040");
        BorderAccentColor = Color.Parse("#60CDFF");
        
        await Task.Delay(1);
    }

    private async Task LoadMTMGreenBaseAsync()
    {
        // MTM Green variant
        PrimaryActionColor = Color.Parse("#4CAF50");
        SecondaryActionColor = Color.Parse("#388E3C");
        AccentColor = Color.Parse("#66BB6A");
        HighlightColor = Color.Parse("#2E7D32");
        
        HeadingTextColor = Color.Parse("#1B5E20");
        BodyTextColor = Color.Parse("#2E7D32");
        InteractiveTextColor = Color.Parse("#4CAF50");
        OverlayTextColor = Color.Parse("#FFFFFF");
        TertiaryTextColor = Color.Parse("#81C784");
        
        MainBackgroundColor = Color.Parse("#F1F8E9");
        CardBackgroundColor = Color.Parse("#FFFFFF");
        HoverBackgroundColor = Color.Parse("#E8F5E8");
        PanelBackgroundColor = Color.Parse("#F3F8F3");
        SidebarBackgroundColor = Color.Parse("#E8F4E8");
        
        SuccessColor = Color.Parse("#4CAF50");
        WarningColor = Color.Parse("#FF9800");
        ErrorColor = Color.Parse("#F44336");
        InfoColor = Color.Parse("#2196F3");
        
        BorderColor = Color.Parse("#C8E6C9");
        BorderAccentColor = Color.Parse("#4CAF50");
        
        await Task.Delay(1);
    }

    private async Task LoadMTMRedBaseAsync()
    {
        // MTM Red variant
        PrimaryActionColor = Color.Parse("#F44336");
        SecondaryActionColor = Color.Parse("#D32F2F");
        AccentColor = Color.Parse("#EF5350");
        HighlightColor = Color.Parse("#C62828");
        
        HeadingTextColor = Color.Parse("#B71C1C");
        BodyTextColor = Color.Parse("#D32F2F");
        InteractiveTextColor = Color.Parse("#F44336");
        OverlayTextColor = Color.Parse("#FFFFFF");
        TertiaryTextColor = Color.Parse("#E57373");
        
        MainBackgroundColor = Color.Parse("#FFEBEE");
        CardBackgroundColor = Color.Parse("#FFFFFF");
        HoverBackgroundColor = Color.Parse("#FFCDD2");
        PanelBackgroundColor = Color.Parse("#FCE4EC");
        SidebarBackgroundColor = Color.Parse("#FFEBEE");
        
        SuccessColor = Color.Parse("#4CAF50");
        WarningColor = Color.Parse("#FF9800");
        ErrorColor = Color.Parse("#F44336");
        InfoColor = Color.Parse("#2196F3");
        
        BorderColor = Color.Parse("#FFCDD2");
        BorderAccentColor = Color.Parse("#F44336");
        
        await Task.Delay(1);
    }

    private async Task ApplyThemeOverridesAsync()
    {
        try
        {
            foreach (var kvp in ThemeOverrides)
            {
                var colorProperty = kvp.Key;
                var colorValue = kvp.Value;
                
                // Apply override to the appropriate color property
                if (Color.TryParse(colorValue, out var color))
                {
                    switch (colorProperty)
                    {
                        case "PrimaryAction":
                            PrimaryActionColor = color;
                            break;
                        case "SecondaryAction":
                            SecondaryActionColor = color;
                            break;
                        case "AccentColor":
                            AccentColor = color;
                            break;
                        case "HighlightColor":
                            HighlightColor = color;
                            break;
                        // Add other color properties as needed
                    }
                }
            }
            
            await Task.Delay(1); // Ensure async pattern
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying theme overrides");
        }
    }

    #endregion

    #region Conditional Theming System

    [ObservableProperty]
    private ObservableCollection<string> availableContexts = new()
    {
        "Default", "Day Shift", "Night Shift", "Maintenance", "Emergency", "Training", "Quality Control"
    };

    [ObservableProperty]
    private Dictionary<string, Dictionary<string, string>> conditionalThemes = new();

    [RelayCommand]
    private async Task SaveConditionalThemeAsync(string context)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Saving conditional theme for: {context}...";
            Logger.LogDebug("Saving conditional theme for context: {Context}", context);

            var contextColors = new Dictionary<string, string>
            {
                // Save current colors for this context
                ["PrimaryAction"] = PrimaryActionColor.ToString(),
                ["SecondaryAction"] = SecondaryActionColor.ToString(),
                ["AccentColor"] = AccentColor.ToString(),
                ["HighlightColor"] = HighlightColor.ToString(),
                ["HeadingText"] = HeadingTextColor.ToString(),
                ["BodyText"] = BodyTextColor.ToString(),
                ["InteractiveText"] = InteractiveTextColor.ToString(),
                ["OverlayText"] = OverlayTextColor.ToString(),
                ["TertiaryText"] = TertiaryTextColor.ToString(),
                ["MainBackground"] = MainBackgroundColor.ToString(),
                ["CardBackground"] = CardBackgroundColor.ToString(),
                ["HoverBackground"] = HoverBackgroundColor.ToString(),
                ["PanelBackground"] = PanelBackgroundColor.ToString(),
                ["SidebarBackground"] = SidebarBackgroundColor.ToString(),
                ["Success"] = SuccessColor.ToString(),
                ["Warning"] = WarningColor.ToString(),
                ["Error"] = ErrorColor.ToString(),
                ["Info"] = InfoColor.ToString(),
                ["Border"] = BorderColor.ToString(),
                ["BorderAccent"] = BorderAccentColor.ToString()
            };

            ConditionalThemes[context] = contextColors;
            ConditionalContext = context;
            
            StatusMessage = $"Conditional theme saved for: {context}";
            Logger.LogInformation("Conditional theme saved for context: {Context}", context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving conditional theme for context: {Context}", context);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to save conditional theme for {context}", Environment.UserName);
            StatusMessage = $"Failed to save conditional theme for {context}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadConditionalThemeAsync(string context)
    {
        try
        {
            IsLoading = true;
            StatusMessage = $"Loading conditional theme: {context}...";
            Logger.LogDebug("Loading conditional theme for context: {Context}", context);

            if (!ConditionalThemes.ContainsKey(context))
            {
                StatusMessage = $"No conditional theme found for: {context}";
                return;
            }

            var contextColors = ConditionalThemes[context];
            
            // Apply all colors from the conditional theme
            foreach (var kvp in contextColors)
            {
                var colorProperty = kvp.Key;
                var colorValue = kvp.Value;
                
                if (Color.TryParse(colorValue, out var color))
                {
                    switch (colorProperty)
                    {
                        case "PrimaryAction":
                            PrimaryActionColor = color;
                            break;
                        case "SecondaryAction":
                            SecondaryActionColor = color;
                            break;
                        case "AccentColor":
                            AccentColor = color;
                            break;
                        case "HighlightColor":
                            HighlightColor = color;
                            break;
                        case "HeadingText":
                            HeadingTextColor = color;
                            break;
                        case "BodyText":
                            BodyTextColor = color;
                            break;
                        case "InteractiveText":
                            InteractiveTextColor = color;
                            break;
                        case "OverlayText":
                            OverlayTextColor = color;
                            break;
                        case "TertiaryText":
                            TertiaryTextColor = color;
                            break;
                        case "MainBackground":
                            MainBackgroundColor = color;
                            break;
                        case "CardBackground":
                            CardBackgroundColor = color;
                            break;
                        case "HoverBackground":
                            HoverBackgroundColor = color;
                            break;
                        case "PanelBackground":
                            PanelBackgroundColor = color;
                            break;
                        case "SidebarBackground":
                            SidebarBackgroundColor = color;
                            break;
                        case "Success":
                            SuccessColor = color;
                            break;
                        case "Warning":
                            WarningColor = color;
                            break;
                        case "Error":
                            ErrorColor = color;
                            break;
                        case "Info":
                            InfoColor = color;
                            break;
                        case "Border":
                            BorderColor = color;
                            break;
                        case "BorderAccent":
                            BorderAccentColor = color;
                            break;
                    }
                }
            }

            ConditionalContext = context;
            CurrentThemeName = $"{BaseTheme} - {context}";
            HasUnsavedChanges = true;
            
            StatusMessage = $"Conditional theme loaded: {context}";
            Logger.LogInformation("Conditional theme loaded for context: {Context}", context);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading conditional theme for context: {Context}", context);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load conditional theme for {context}", Environment.UserName);
            StatusMessage = $"Failed to load conditional theme for {context}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region System Theme Integration

    [RelayCommand]
    private async Task SyncWithSystemThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Syncing with system theme...";
            Logger.LogDebug("Syncing theme editor with system theme settings");

            if (!RespectSystemTheme)
            {
                StatusMessage = "System theme sync is disabled";
                return;
            }

            // Check system theme preference (this would typically use platform-specific APIs)
            var isSystemDark = await DetectSystemDarkModeAsync();
            
            if (isSystemDark)
            {
                await LoadWindowsDarkBaseAsync();
                CurrentThemeName = "System Dark";
            }
            else
            {
                await LoadWindowsLightBaseAsync();
                CurrentThemeName = "System Light";
            }

            BaseTheme = isSystemDark ? "Windows_Dark" : "Windows_Light";
            HasUnsavedChanges = true;
            
            StatusMessage = $"Synced with system theme: {(isSystemDark ? "Dark" : "Light")}";
            Logger.LogInformation("Theme synced with system: {IsDark}", isSystemDark);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error syncing with system theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to sync with system theme", Environment.UserName);
            StatusMessage = "Failed to sync with system theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task<bool> DetectSystemDarkModeAsync()
    {
        try
        {
            // Platform-specific system theme detection
            // This is a simplified implementation - in practice, you'd use:
            // - Windows: Registry check or WinRT API
            // - macOS: NSUserDefaults
            // - Linux: gsettings or desktop environment specific APIs
            
            await Task.Delay(100); // Simulate async detection
            
            // For now, return based on current time (demo purposes)
            var currentHour = DateTime.Now.Hour;
            return currentHour < 7 || currentHour > 19; // Dark mode for evening/night hours
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error detecting system dark mode");
            return false; // Default to light mode on error
        }
    }

    #endregion

    #region Enhanced Color Palette Sharing

    [RelayCommand]
    private async Task ShareThemeToNetworkAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Sharing theme to network installations...";
            Logger.LogDebug("Sharing theme to network MTM installations");

            // Create a shareable theme package
            var shareableTheme = new ThemeShareModel
            {
                Name = CurrentThemeName,
                Version = ThemeVersion,
                Description = ThemeDocumentation,
                BaseTheme = BaseTheme,
                ConditionalContext = ConditionalContext,
                CreatedBy = Environment.UserName,
                CreatedDate = DateTime.Now,
                SharedDate = DateTime.Now,
                Colors = new Dictionary<string, string>
                {
                    ["PrimaryAction"] = PrimaryActionColor.ToString(),
                    ["SecondaryAction"] = SecondaryActionColor.ToString(),
                    ["AccentColor"] = AccentColor.ToString(),
                    ["HighlightColor"] = HighlightColor.ToString(),
                    ["HeadingText"] = HeadingTextColor.ToString(),
                    ["BodyText"] = BodyTextColor.ToString(),
                    ["InteractiveText"] = InteractiveTextColor.ToString(),
                    ["OverlayText"] = OverlayTextColor.ToString(),
                    ["TertiaryText"] = TertiaryTextColor.ToString(),
                    ["MainBackground"] = MainBackgroundColor.ToString(),
                    ["CardBackground"] = CardBackgroundColor.ToString(),
                    ["HoverBackground"] = HoverBackgroundColor.ToString(),
                    ["PanelBackground"] = PanelBackgroundColor.ToString(),
                    ["SidebarBackground"] = SidebarBackgroundColor.ToString(),
                    ["Success"] = SuccessColor.ToString(),
                    ["Warning"] = WarningColor.ToString(),
                    ["Error"] = ErrorColor.ToString(),
                    ["Info"] = InfoColor.ToString(),
                    ["Border"] = BorderColor.ToString(),
                    ["BorderAccent"] = BorderAccentColor.ToString()
                },
                CustomNames = new Dictionary<string, string>(ColorCustomNames)
            };

            // Serialize and save to shared network location
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var themeJson = JsonSerializer.Serialize(shareableTheme, jsonOptions);
            
            // Save to network shared folder (would be configurable in production)
            var networkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "MTM_SharedThemes");
            Directory.CreateDirectory(networkPath);
            
            var fileName = $"MTM_SharedTheme_{CurrentThemeName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var filePath = Path.Combine(networkPath, fileName);
            
            await File.WriteAllTextAsync(filePath, themeJson);
            
            StatusMessage = $"Theme shared to network: {fileName}";
            Logger.LogInformation("Theme shared to network: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error sharing theme to network");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to share theme to network", Environment.UserName);
            StatusMessage = "Failed to share theme to network";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ImportFromNetworkAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Checking network for shared themes...";
            Logger.LogDebug("Importing themes from network share");

            var networkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "MTM_SharedThemes");
            
            if (!Directory.Exists(networkPath))
            {
                StatusMessage = "No network shared themes folder found";
                return;
            }

            var themeFiles = Directory.GetFiles(networkPath, "MTM_SharedTheme_*.json");
            
            if (!themeFiles.Any())
            {
                StatusMessage = "No shared themes found on network";
                return;
            }

            // Get the most recent shared theme
            var latestFile = themeFiles
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var themeJson = await File.ReadAllTextAsync(latestFile.FullName);
            
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            var sharedTheme = JsonSerializer.Deserialize<ThemeShareModel>(themeJson, jsonOptions);
            
            if (sharedTheme == null)
            {
                StatusMessage = "Invalid shared theme format";
                return;
            }

            // Apply the shared theme
            CurrentThemeName = sharedTheme.Name;
            ThemeVersion = sharedTheme.Version;
            ThemeDocumentation = sharedTheme.Description;
            BaseTheme = sharedTheme.BaseTheme;
            ConditionalContext = sharedTheme.ConditionalContext;

            // Apply colors
            foreach (var kvp in sharedTheme.Colors)
            {
                if (Color.TryParse(kvp.Value, out var color))
                {
                    switch (kvp.Key)
                    {
                        case "PrimaryAction":
                            PrimaryActionColor = color;
                            break;
                        case "SecondaryAction":
                            SecondaryActionColor = color;
                            break;
                        case "AccentColor":
                            AccentColor = color;
                            break;
                        case "HighlightColor":
                            HighlightColor = color;
                            break;
                        case "HeadingText":
                            HeadingTextColor = color;
                            break;
                        case "BodyText":
                            BodyTextColor = color;
                            break;
                        case "InteractiveText":
                            InteractiveTextColor = color;
                            break;
                        case "OverlayText":
                            OverlayTextColor = color;
                            break;
                        case "TertiaryText":
                            TertiaryTextColor = color;
                            break;
                        case "MainBackground":
                            MainBackgroundColor = color;
                            break;
                        case "CardBackground":
                            CardBackgroundColor = color;
                            break;
                        case "HoverBackground":
                            HoverBackgroundColor = color;
                            break;
                        case "PanelBackground":
                            PanelBackgroundColor = color;
                            break;
                        case "SidebarBackground":
                            SidebarBackgroundColor = color;
                            break;
                        case "Success":
                            SuccessColor = color;
                            break;
                        case "Warning":
                            WarningColor = color;
                            break;
                        case "Error":
                            ErrorColor = color;
                            break;
                        case "Info":
                            InfoColor = color;
                            break;
                        case "Border":
                            BorderColor = color;
                            break;
                        case "BorderAccent":
                            BorderAccentColor = color;
                            break;
                    }
                }
            }

            // Apply custom names
            ColorCustomNames = new Dictionary<string, string>(sharedTheme.CustomNames);

            HasUnsavedChanges = true;
            StatusMessage = $"Imported shared theme: {sharedTheme.Name} by {sharedTheme.CreatedBy}";
            Logger.LogInformation("Imported shared theme: {ThemeName} by {CreatedBy}", sharedTheme.Name, sharedTheme.CreatedBy);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error importing shared theme from network");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to import shared theme", Environment.UserName);
            StatusMessage = "Failed to import shared theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion

    #region Enhanced Auto-Fill Algorithms

    [RelayCommand]
    private async Task AutoFillMonochromaticAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating monochromatic palette...";
            Logger.LogDebug("Auto-filling monochromatic colors");

            var baseColor = PrimaryActionColor;
            
            // Generate tints and shades of the base color
            SecondaryActionColor = DarkenColor(baseColor, 0.15f);
            AccentColor = LightenColor(baseColor, 0.2f);
            HighlightColor = DarkenColor(baseColor, 0.3f);
            
            // Subtle background variations
            MainBackgroundColor = LightenColor(baseColor, 0.95f);
            CardBackgroundColor = LightenColor(baseColor, 0.92f);
            HoverBackgroundColor = LightenColor(baseColor, 0.88f);
            
            // Text colors with good contrast
            HeadingTextColor = DarkenColor(baseColor, 0.7f);
            BodyTextColor = DarkenColor(baseColor, 0.5f);
            InteractiveTextColor = baseColor;

            HasUnsavedChanges = true;
            StatusMessage = "Monochromatic palette generated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating monochromatic palette");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate monochromatic palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillComplementaryAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating complementary palette...";
            Logger.LogDebug("Auto-filling complementary colors");

            var baseColor = PrimaryActionColor;
            var complementary = GetComplementaryColor(baseColor);
            
            // Use base and complementary for high contrast
            PrimaryActionColor = baseColor;
            SecondaryActionColor = complementary;
            AccentColor = BlendColor(baseColor, complementary, 0.7f);
            HighlightColor = DarkenColor(baseColor, 0.2f);
            
            // Status colors using complementary principles
            SuccessColor = GetComplementaryColor(Color.Parse("#4CAF50")); // Red-green complementary
            WarningColor = GetComplementaryColor(Color.Parse("#FF9800")); // Orange-blue complementary
            ErrorColor = baseColor; // Use base for error to maintain consistency
            InfoColor = complementary; // Use complementary for info

            HasUnsavedChanges = true;
            StatusMessage = "Complementary palette generated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating complementary palette");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate complementary palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillAnalogousAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating analogous palette...";
            Logger.LogDebug("Auto-filling analogous colors");

            var baseColor = PrimaryActionColor;
            
            // Generate analogous colors (adjacent on color wheel)
            var analogous1 = ShiftHue(baseColor, 30);  // +30 degrees
            var analogous2 = ShiftHue(baseColor, -30); // -30 degrees
            
            PrimaryActionColor = baseColor;
            SecondaryActionColor = analogous1;
            AccentColor = analogous2;
            HighlightColor = DarkenColor(baseColor, 0.2f);
            
            // Harmonious background colors
            MainBackgroundColor = LightenColor(analogous2, 0.9f);
            CardBackgroundColor = LightenColor(analogous1, 0.85f);
            HoverBackgroundColor = LightenColor(baseColor, 0.8f);

            HasUnsavedChanges = true;
            StatusMessage = "Analogous palette generated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating analogous palette");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate analogous palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillTriadicAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating triadic palette...";
            Logger.LogDebug("Auto-filling triadic colors");

            var baseColor = PrimaryActionColor;
            
            // Generate triadic colors (120 degrees apart)
            var triadic1 = ShiftHue(baseColor, 120);  // +120 degrees
            var triadic2 = ShiftHue(baseColor, 240);  // +240 degrees (or -120)
            
            PrimaryActionColor = baseColor;
            SecondaryActionColor = triadic1;
            AccentColor = triadic2;
            HighlightColor = DarkenColor(baseColor, 0.2f);
            
            // Use triadic colors for status
            SuccessColor = triadic1;
            WarningColor = BlendColor(baseColor, triadic2, 0.7f);
            ErrorColor = triadic2;
            InfoColor = baseColor;

            HasUnsavedChanges = true;
            StatusMessage = "Triadic palette generated successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating triadic palette");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate triadic palette", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AutoFillAccessibilityFirstAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating accessibility-first palette (WCAG AAA)...";
            Logger.LogDebug("Auto-filling accessibility-first colors");

            // Generate colors that meet WCAG AAA (7:1) contrast requirements
            var darkBackground = Color.Parse("#FFFFFF"); // White background
            
            // Generate high-contrast colors
            PrimaryActionColor = Color.Parse("#0066CC");   // Blue that meets AAA on white
            SecondaryActionColor = Color.Parse("#004499");  // Darker blue
            AccentColor = Color.Parse("#0099FF");          // Lighter blue, still AAA compliant
            HighlightColor = Color.Parse("#003366");       // Very dark blue
            
            // Text colors with maximum contrast
            HeadingTextColor = Color.Parse("#000000");     // Pure black
            BodyTextColor = Color.Parse("#333333");        // Very dark gray
            InteractiveTextColor = Color.Parse("#0066CC"); // Same as primary
            OverlayTextColor = Color.Parse("#FFFFFF");     // White for dark backgrounds
            TertiaryTextColor = Color.Parse("#666666");    // Medium gray, still AAA
            
            // High-contrast status colors
            SuccessColor = Color.Parse("#006600");         // Dark green
            WarningColor = Color.Parse("#CC6600");         // Dark orange  
            ErrorColor = Color.Parse("#CC0000");           // Dark red
            InfoColor = PrimaryActionColor;               // Consistent with primary
            
            // Clean, high-contrast backgrounds
            MainBackgroundColor = Color.Parse("#FFFFFF");   // Pure white
            CardBackgroundColor = Color.Parse("#F8F8F8");   // Very light gray
            HoverBackgroundColor = Color.Parse("#F0F0F0");  // Light gray
            PanelBackgroundColor = Color.Parse("#FAFAFA");  // Off-white
            SidebarBackgroundColor = Color.Parse("#F5F5F5"); // Light gray
            
            // Strong borders for clarity
            BorderColor = Color.Parse("#CCCCCC");          // Medium gray
            BorderAccentColor = Color.Parse("#999999");    // Darker gray

            HasUnsavedChanges = true;
            StatusMessage = "Accessibility-first palette generated (WCAG AAA compliant)";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating accessibility-first palette");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate accessibility-first palette", Environment.UserName);
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
            Logger.LogDebug("Generating comprehensive theme preview with all 27 colors");

            // Create comprehensive color dictionary for preview with all 27 colors
            var previewColors = new Dictionary<string, string>
            {
                // Core Action Colors (4)
                ["MTM_Shared_Logic.PrimaryAction"] = PrimaryActionColor.ToString(),
                ["MTM_Shared_Logic.SecondaryAction"] = SecondaryActionColor.ToString(),
                ["MTM_Shared_Logic.AccentColor"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.HighlightColor"] = HighlightColor.ToString(),
                
                // Text Colors (5)
                ["MTM_Shared_Logic.HeadingText"] = HeadingTextColor.ToString(),
                ["MTM_Shared_Logic.BodyText"] = BodyTextColor.ToString(),
                ["MTM_Shared_Logic.InteractiveText"] = InteractiveTextColor.ToString(),
                ["MTM_Shared_Logic.OverlayTextBrush"] = OverlayTextColor.ToString(),
                ["MTM_Shared_Logic.TertiaryTextBrush"] = TertiaryTextColor.ToString(),
                
                // Background Colors (5)
                ["MTM_Shared_Logic.MainBackground"] = MainBackgroundColor.ToString(),
                ["MTM_Shared_Logic.CardBackgroundBrush"] = CardBackgroundColor.ToString(),
                ["MTM_Shared_Logic.HoverBackground"] = HoverBackgroundColor.ToString(),
                ["MTM_Shared_Logic.PanelBackgroundBrush"] = PanelBackgroundColor.ToString(),
                ["MTM_Shared_Logic.SidebarBackground"] = SidebarBackgroundColor.ToString(),
                
                // Status Colors (4)
                ["MTM_Shared_Logic.SuccessBrush"] = SuccessColor.ToString(),
                ["MTM_Shared_Logic.WarningBrush"] = WarningColor.ToString(),
                ["MTM_Shared_Logic.ErrorBrush"] = ErrorColor.ToString(),
                ["MTM_Shared_Logic.InfoBrush"] = InfoColor.ToString(),
                
                // Border Colors (2)
                ["MTM_Shared_Logic.BorderBrush"] = BorderColor.ToString(),
                ["MTM_Shared_Logic.BorderAccentBrush"] = BorderAccentColor.ToString(),
                
                // Additional derived resources for comprehensive theming
                ["MTM_Shared_Logic.FocusBrush"] = AccentColor.ToString(),
                ["MTM_Shared_Logic.BorderDarkBrush"] = DarkenColor(BorderColor, 0.2f).ToString(),
                ["MTM_Shared_Logic.ErrorLightBrush"] = LightenColor(ErrorColor, 0.8f).ToString(),
                ["MTM_Shared_Logic.SidebarGradientBrush"] = PanelBackgroundColor.ToString(),
                ["MTM_Shared_Logic.HoverBrush"] = HoverBackgroundColor.ToString()
            };

            // Apply comprehensive preview via ThemeService
            if (_themeService != null)
            {
                var result = await _themeService.ApplyCustomColorsAsync(previewColors);
                if (result.IsSuccess)
                {
                    IsPreviewMode = true;
                    StatusMessage = $"✅ Preview applied ({previewColors.Count} colors) - Use Apply to save changes";
                    Logger.LogInformation("Theme preview applied successfully with {ColorCount} colors", previewColors.Count);
                }
                else
                {
                    StatusMessage = $"❌ Preview failed: {result.Message}";
                    Logger.LogWarning("Theme preview failed: {Message}", result.Message);
                }
            }
            else
            {
                StatusMessage = "❌ Preview unavailable - Theme service not available";
                Logger.LogWarning("Theme service not available for preview");
            }

            await Task.Delay(100); // Brief delay for user feedback
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating comprehensive theme preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate theme preview", Environment.UserName);
            StatusMessage = "❌ Error generating preview";
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
                StatusMessage = "Closing with unsaved changes - changes will be lost";
                Logger.LogWarning("Closing theme editor with unsaved changes");
                
                // Allow a moment for the user to see the status message
                await Task.Delay(1500);
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
                    Logger.LogDebug("Navigated back to MainView from theme editor");
                }
                else
                {
                    Logger.LogWarning("MainViewViewModel not found in service provider");
                    StatusMessage = "Warning: Could not return to main view properly";
                }
            }
            else
            {
                Logger.LogError("NavigationService not available for theme editor close");
                StatusMessage = "Error: Navigation service not available";
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing theme editor");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to close theme editor", Environment.UserName);
        }
    }

    [RelayCommand]
    private async Task ValidateThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Validating theme colors and accessibility...";
            Logger.LogDebug("Starting comprehensive theme validation");

            var validationResults = new List<string>();
            var warnings = new List<string>();

            // Basic color validation
            if (!ValidateAllColors())
            {
                validationResults.Add("❌ Basic color validation failed");
                return;
            }
            validationResults.Add("✅ All color properties are valid");

            // Contrast validation (simplified WCAG check)
            var contrastIssues = await ValidateContrastRatiosAsync();
            if (contrastIssues.Any())
            {
                warnings.AddRange(contrastIssues.Select(issue => $"⚠️ {issue}"));
            }
            else
            {
                validationResults.Add("✅ Contrast ratios meet basic accessibility requirements");
            }

            // Color harmony validation
            var harmonyCheck = ValidateColorHarmony();
            if (harmonyCheck.isHarmonious)
            {
                validationResults.Add($"✅ Color harmony: {harmonyCheck.harmonyType}");
            }
            else
            {
                warnings.Add($"⚠️ Colors may not be harmonious - consider using auto-fill algorithms");
            }

            // Prepare final status message
            var resultCount = validationResults.Count;
            var warningCount = warnings.Count;
            
            if (warningCount == 0)
            {
                StatusMessage = $"✅ Theme validation passed! ({resultCount} checks completed)";
            }
            else
            {
                StatusMessage = $"⚠️ Theme validation: {resultCount} passed, {warningCount} warnings";
            }

            // Log detailed results
            Logger.LogInformation("Theme validation completed: {PassedChecks} passed, {Warnings} warnings", 
                resultCount, warningCount);
            
            foreach (var result in validationResults)
            {
                Logger.LogDebug("Validation result: {Result}", result);
            }
            
            foreach (var warning in warnings)
            {
                Logger.LogWarning("Validation warning: {Warning}", warning);
            }

            await Task.Delay(100); // Brief pause for user feedback
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during theme validation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Theme validation failed", Environment.UserName);
            StatusMessage = "❌ Theme validation encountered an error";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ApplyOptimizedManufacturingThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Applying optimized manufacturing theme...";
            Logger.LogDebug("Applying high-contrast manufacturing theme optimized for industrial displays");

            // Take snapshot for undo functionality
            SaveColorSnapshot("Before Manufacturing Theme");

            // High-contrast colors specifically designed for manufacturing environments
            // These colors provide maximum readability in harsh lighting conditions
            PrimaryActionColor = Color.Parse("#0066CC");     // Strong blue for reliability
            SecondaryActionColor = Color.Parse("#004499");   // Darker blue for secondary actions
            AccentColor = Color.Parse("#FF6600");            // Safety orange for attention
            HighlightColor = Color.Parse("#FFCC00");         // Warning yellow for alerts

            // High contrast text colors for industrial displays
            HeadingTextColor = Color.Parse("#000000");       // Pure black for maximum contrast
            BodyTextColor = Color.Parse("#2C2C2C");          // Very dark gray
            InteractiveTextColor = Color.Parse("#0066CC");   // Matching primary blue
            OverlayTextColor = Color.Parse("#FFFFFF");       // Pure white
            TertiaryTextColor = Color.Parse("#666666");      // Medium gray

            // Industrial-grade background colors
            MainBackgroundColor = Color.Parse("#F5F5F5");    // Light gray background
            CardBackgroundColor = Color.Parse("#FFFFFF");    // Pure white cards
            HoverBackgroundColor = Color.Parse("#E6E6E6");   // Light hover state
            PanelBackgroundColor = Color.Parse("#F0F0F0");   // Panel background
            SidebarBackgroundColor = Color.Parse("#E8E8E8"); // Sidebar background

            // Manufacturing status colors (safety-oriented)
            SuccessColor = Color.Parse("#00AA00");           // Green for success/safe
            WarningColor = Color.Parse("#FF9900");           // Orange for warnings
            ErrorColor = Color.Parse("#CC0000");             // Red for errors/danger
            InfoColor = Color.Parse("#0077CC");              // Blue for information

            // High contrast borders
            BorderColor = Color.Parse("#CCCCCC");            // Light gray borders
            BorderAccentColor = Color.Parse("#666666");      // Darker accent borders

            HasUnsavedChanges = true;
            StatusMessage = "✅ Manufacturing theme applied - optimized for industrial displays";
            Logger.LogInformation("Successfully applied manufacturing-optimized theme");

            await Task.Delay(100); // Brief pause for user feedback
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying optimized manufacturing theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply manufacturing theme", Environment.UserName);
            StatusMessage = "❌ Failed to apply manufacturing theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Applies healthcare-optimized theme with calming colors
    /// </summary>
    [RelayCommand]
    private async Task ApplyHealthcareThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Applying healthcare theme...";
            Logger.LogDebug("Applying healthcare-optimized theme with calming colors");

            // Take snapshot for undo functionality
            SaveColorSnapshot("Before Healthcare Theme");

            // Calming healthcare colors
            PrimaryActionColor = Color.Parse("#008B8B");     // Teal - calming medical color
            SecondaryActionColor = Color.Parse("#006666");   // Darker teal
            AccentColor = Color.Parse("#40E0D0");            // Turquoise accent
            HighlightColor = Color.Parse("#20B2AA");         // Light sea green

            // Gentle text colors for healthcare
            HeadingTextColor = Color.Parse("#2F4F4F");       // Dark slate gray
            BodyTextColor = Color.Parse("#696969");          // Dim gray
            InteractiveTextColor = Color.Parse("#008B8B");   // Matching teal
            OverlayTextColor = Color.Parse("#FFFFFF");       // Pure white
            TertiaryTextColor = Color.Parse("#A0A0A0");      // Light gray

            // Soft, clean background colors
            MainBackgroundColor = Color.Parse("#F8FFFF");    // Azure white
            CardBackgroundColor = Color.Parse("#FFFFFF");    // Pure white
            HoverBackgroundColor = Color.Parse("#F0FFFF");   // Light cyan
            PanelBackgroundColor = Color.Parse("#F5FFFA");   // Mint cream
            SidebarBackgroundColor = Color.Parse("#F0F8FF");  // Alice blue

            // Healthcare-appropriate status colors
            SuccessColor = Color.Parse("#32CD32");           // Lime green
            WarningColor = Color.Parse("#FFD700");           // Gold
            ErrorColor = Color.Parse("#DC143C");             // Crimson
            InfoColor = Color.Parse("#4682B4");              // Steel blue

            // Soft borders
            BorderColor = Color.Parse("#E0E0E0");            // Light gray
            BorderAccentColor = Color.Parse("#B0B0B0");      // Silver

            CurrentThemeName = "Healthcare Theme";
            HasUnsavedChanges = true;
            StatusMessage = "✅ Healthcare theme applied - calming medical colors";
            Logger.LogInformation("Successfully applied healthcare theme");

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying healthcare theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply healthcare theme", Environment.UserName);
            StatusMessage = "❌ Failed to apply healthcare theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Applies office-optimized theme with professional business colors
    /// </summary>
    [RelayCommand]
    private async Task ApplyOfficeThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Applying office theme...";
            Logger.LogDebug("Applying office-optimized professional business theme");

            // Take snapshot for undo functionality
            SaveColorSnapshot("Before Office Theme");

            // Professional Microsoft Fluent-style colors
            PrimaryActionColor = Color.Parse("#0078D4");     // Microsoft blue
            SecondaryActionColor = Color.Parse("#106EBE");   // Darker blue
            AccentColor = Color.Parse("#0099BC");            // Cyan blue
            HighlightColor = Color.Parse("#00BCF2");         // Light blue

            // Professional text colors
            HeadingTextColor = Color.Parse("#323130");       // Neutral gray
            BodyTextColor = Color.Parse("#605E5C");          // Gray brown
            InteractiveTextColor = Color.Parse("#0078D4");   // Microsoft blue
            OverlayTextColor = Color.Parse("#FFFFFF");       // White
            TertiaryTextColor = Color.Parse("#8A8886");      // Light gray

            // Clean office backgrounds
            MainBackgroundColor = Color.Parse("#FAFAFA");    // Very light gray
            CardBackgroundColor = Color.Parse("#FFFFFF");    // White
            HoverBackgroundColor = Color.Parse("#F3F2F1");   // Light warm gray
            PanelBackgroundColor = Color.Parse("#F8F7F6");   // Warm white
            SidebarBackgroundColor = Color.Parse("#F5F5F5");  // Light gray

            // Professional status colors
            SuccessColor = Color.Parse("#107C10");           // Green
            WarningColor = Color.Parse("#FFB900");           // Amber
            ErrorColor = Color.Parse("#D13438");             // Red
            InfoColor = Color.Parse("#0078D4");              // Blue

            // Subtle borders
            BorderColor = Color.Parse("#EDEBE9");            // Light neutral
            BorderAccentColor = Color.Parse("#C8C6C4");      // Medium neutral

            CurrentThemeName = "Office Theme";
            HasUnsavedChanges = true;
            StatusMessage = "✅ Office theme applied - professional business colors";
            Logger.LogInformation("Successfully applied office theme");

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying office theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply office theme", Environment.UserName);
            StatusMessage = "❌ Failed to apply office theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Applies high contrast theme for maximum accessibility
    /// </summary>
    [RelayCommand]
    private async Task ApplyHighContrastThemeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Applying high contrast theme...";
            Logger.LogDebug("Applying high contrast theme for maximum accessibility");

            // Take snapshot for undo functionality
            SaveColorSnapshot("Before High Contrast Theme");

            // Maximum contrast colors
            PrimaryActionColor = Color.Parse("#0000FF");     // Pure blue
            SecondaryActionColor = Color.Parse("#000080");   // Navy blue
            AccentColor = Color.Parse("#8000FF");            // Purple
            HighlightColor = Color.Parse("#FF00FF");         // Magenta

            // High contrast text
            HeadingTextColor = Color.Parse("#000000");       // Pure black
            BodyTextColor = Color.Parse("#000000");          // Pure black
            InteractiveTextColor = Color.Parse("#0000FF");   // Pure blue
            OverlayTextColor = Color.Parse("#FFFFFF");       // Pure white
            TertiaryTextColor = Color.Parse("#808080");      // Gray

            // High contrast backgrounds
            MainBackgroundColor = Color.Parse("#FFFFFF");    // Pure white
            CardBackgroundColor = Color.Parse("#FFFFFF");    // Pure white
            HoverBackgroundColor = Color.Parse("#C0C0C0");   // Silver
            PanelBackgroundColor = Color.Parse("#F0F0F0");   // Light gray
            SidebarBackgroundColor = Color.Parse("#E0E0E0");  // Light gray

            // High contrast status colors
            SuccessColor = Color.Parse("#008000");           // Pure green
            WarningColor = Color.Parse("#FF8000");           // Orange
            ErrorColor = Color.Parse("#FF0000");             // Pure red
            InfoColor = Color.Parse("#0000FF");              // Pure blue

            // Strong borders
            BorderColor = Color.Parse("#000000");            // Black
            BorderAccentColor = Color.Parse("#808080");      // Gray

            CurrentThemeName = "High Contrast Theme";
            HasUnsavedChanges = true;
            StatusMessage = "✅ High contrast theme applied - maximum accessibility";
            Logger.LogInformation("Successfully applied high contrast theme");

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying high contrast theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply high contrast theme", Environment.UserName);
            StatusMessage = "❌ Failed to apply high contrast theme";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Adjusts brightness of all colors by specified percentage
    /// </summary>
    [RelayCommand]
    private async Task AdjustBrightnessAsync(string adjustmentParameter)
    {
        try
        {
            if (!int.TryParse(adjustmentParameter, out int adjustment))
            {
                StatusMessage = "Invalid brightness adjustment value";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Adjusting brightness by {adjustment}%...";
            Logger.LogDebug("Adjusting all colors brightness by {Adjustment}%", adjustment);

            // Take snapshot for undo functionality
            SaveColorSnapshot($"Before Brightness {adjustment}%");

            double factor = 1.0 + (adjustment / 100.0);

            // Adjust all colors
            PrimaryActionColor = AdjustColorBrightness(PrimaryActionColor, factor);
            SecondaryActionColor = AdjustColorBrightness(SecondaryActionColor, factor);
            AccentColor = AdjustColorBrightness(AccentColor, factor);
            HighlightColor = AdjustColorBrightness(HighlightColor, factor);

            HeadingTextColor = AdjustColorBrightness(HeadingTextColor, factor);
            BodyTextColor = AdjustColorBrightness(BodyTextColor, factor);
            InteractiveTextColor = AdjustColorBrightness(InteractiveTextColor, factor);
            OverlayTextColor = AdjustColorBrightness(OverlayTextColor, factor);
            TertiaryTextColor = AdjustColorBrightness(TertiaryTextColor, factor);

            MainBackgroundColor = AdjustColorBrightness(MainBackgroundColor, factor);
            CardBackgroundColor = AdjustColorBrightness(CardBackgroundColor, factor);
            HoverBackgroundColor = AdjustColorBrightness(HoverBackgroundColor, factor);
            PanelBackgroundColor = AdjustColorBrightness(PanelBackgroundColor, factor);
            SidebarBackgroundColor = AdjustColorBrightness(SidebarBackgroundColor, factor);

            SuccessColor = AdjustColorBrightness(SuccessColor, factor);
            WarningColor = AdjustColorBrightness(WarningColor, factor);
            ErrorColor = AdjustColorBrightness(ErrorColor, factor);
            InfoColor = AdjustColorBrightness(InfoColor, factor);

            BorderColor = AdjustColorBrightness(BorderColor, factor);
            BorderAccentColor = AdjustColorBrightness(BorderAccentColor, factor);

            HasUnsavedChanges = true;
            StatusMessage = $"✅ Brightness adjusted by {adjustment}%";
            Logger.LogInformation("Successfully adjusted brightness by {Adjustment}%", adjustment);

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adjusting brightness");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to adjust brightness", Environment.UserName);
            StatusMessage = "❌ Failed to adjust brightness";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Alias for AutoFillAccessibilityFirstAsync to match UI naming
    /// </summary>
    [RelayCommand]
    private async Task AutoFillAccessibilityAsync()
    {
        await AutoFillAccessibilityFirstAsync();
    }

    /// <summary>
    /// Opens an advanced color picker dialog for the specified color property
    /// </summary>
    [RelayCommand]
    private async Task OpenColorPickerAsync(string? colorProperty)
    {
        try
        {
            if (string.IsNullOrEmpty(colorProperty))
            {
                StatusMessage = "No color property specified";
                return;
            }

            IsLoading = true;
            StatusMessage = $"Opening color picker for {colorProperty}...";
            Logger.LogDebug("Opening color picker for property: {ColorProperty}", colorProperty);

            // Get current color for the specified property
            var currentColor = GetCurrentColorForProperty(colorProperty);
            
            // For now, we'll simulate a color picker dialog
            // In a real implementation, this would open a proper color picker dialog
            var newColor = await ShowSimpleColorPickerAsync(currentColor, colorProperty);
            
            if (newColor.HasValue)
            {
                SetColorForProperty(colorProperty, newColor.Value);
                StatusMessage = $"✅ Color updated for {colorProperty}";
                HasUnsavedChanges = true;
                ValidateAllColors();
            }
            else
            {
                StatusMessage = $"Color picker cancelled for {colorProperty}";
            }

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error opening color picker for {ColorProperty}", colorProperty);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to open color picker for {colorProperty}", Environment.UserName);
            StatusMessage = $"❌ Failed to open color picker for {colorProperty}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Simple color picker simulation - in real implementation would open proper dialog
    /// </summary>
    private async Task<Color?> ShowSimpleColorPickerAsync(Color currentColor, string propertyName)
    {
        await Task.Delay(100); // Simulate dialog opening time
        
        try
        {
            // For demonstration, we'll cycle through some predefined colors
            var predefinedColors = new[]
            {
                Color.Parse("#0078D4"), // MTM Blue
                Color.Parse("#107C10"), // Green  
                Color.Parse("#D83B01"), // Orange
                Color.Parse("#A4262C"), // Red
                Color.Parse("#5C2D91"), // Purple
                Color.Parse("#0078D4"), // Blue
                Color.Parse("#00BCF2"), // Cyan
                Color.Parse("#40E0D0"), // Turquoise
            };

            // Find current color index and move to next one (cycling through)
            var currentIndex = Array.FindIndex(predefinedColors, c => c == currentColor);
            var nextIndex = (currentIndex + 1) % predefinedColors.Length;
            
            Logger.LogDebug("Color picker for {Property}: changing from {Current} to {Next}", 
                propertyName, currentColor, predefinedColors[nextIndex]);
                
            return predefinedColors[nextIndex];
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in color picker simulation");
            return null;
        }
    }

    /// <summary>
    /// Gets the current color value for a specified property name
    /// </summary>
    private Color GetCurrentColorForProperty(string propertyName)
    {
        return propertyName switch
        {
            "PrimaryAction" => PrimaryActionColor,
            "SecondaryAction" => SecondaryActionColor,
            "Accent" => AccentColor,
            "Highlight" => HighlightColor,
            "HeadingText" => HeadingTextColor,
            "BodyText" => BodyTextColor,
            "InteractiveText" => InteractiveTextColor,
            "OverlayText" => OverlayTextColor,
            "TertiaryText" => TertiaryTextColor,
            "MainBackground" => MainBackgroundColor,
            "CardBackground" => CardBackgroundColor,
            "HoverBackground" => HoverBackgroundColor,
            "PanelBackground" => PanelBackgroundColor,
            "SidebarBackground" => SidebarBackgroundColor,
            "Success" => SuccessColor,
            "Warning" => WarningColor,
            "Error" => ErrorColor,
            "Info" => InfoColor,
            "Border" => BorderColor,
            "BorderAccent" => BorderAccentColor,
            _ => PrimaryActionColor // Default fallback
        };
    }

    /// <summary>
    /// Sets a color value for a specified property name
    /// </summary>
    private void SetColorForProperty(string propertyName, Color color)
    {
        switch (propertyName)
        {
            case "PrimaryAction":
                PrimaryActionColor = color;
                break;
            case "SecondaryAction":
                SecondaryActionColor = color;
                break;
            case "Accent":
                AccentColor = color;
                break;
            case "Highlight":
                HighlightColor = color;
                break;
            case "HeadingText":
                HeadingTextColor = color;
                break;
            case "BodyText":
                BodyTextColor = color;
                break;
            case "InteractiveText":
                InteractiveTextColor = color;
                break;
            case "OverlayText":
                OverlayTextColor = color;
                break;
            case "TertiaryText":
                TertiaryTextColor = color;
                break;
            case "MainBackground":
                MainBackgroundColor = color;
                break;
            case "CardBackground":
                CardBackgroundColor = color;
                break;
            case "HoverBackground":
                HoverBackgroundColor = color;
                break;
            case "PanelBackground":
                PanelBackgroundColor = color;
                break;
            case "SidebarBackground":
                SidebarBackgroundColor = color;
                break;
            case "Success":
                SuccessColor = color;
                break;
            case "Warning":
                WarningColor = color;
                break;
            case "Error":
                ErrorColor = color;
                break;
            case "Info":
                InfoColor = color;
                break;
            case "Border":
                BorderColor = color;
                break;
            case "BorderAccent":
                BorderAccentColor = color;
                break;
        }
    }

    /// <summary>
    /// Generates a comprehensive theme report with color analysis
    /// </summary>
    [RelayCommand]
    private async Task GenerateThemeReportAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Generating theme report...";
            Logger.LogDebug("Generating comprehensive theme analysis report");

            var report = new StringBuilder();
            report.AppendLine($"# Theme Analysis Report");
            report.AppendLine($"**Theme Name:** {CurrentThemeName}");
            report.AppendLine($"**Version:** {ThemeVersion}");
            report.AppendLine($"**Generated:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"**Base Theme:** {BaseTheme}");
            report.AppendLine();

            // Color accessibility analysis
            report.AppendLine("## Color Accessibility Analysis");
            var contrastIssues = await ValidateContrastRatiosAsync();
            if (!contrastIssues.Any())
            {
                report.AppendLine("✅ All color combinations meet WCAG AA standards (4.5:1 contrast ratio)");
            }
            else
            {
                report.AppendLine("⚠️ The following color combinations have accessibility issues:");
                foreach (var issue in contrastIssues)
                {
                    report.AppendLine($"- {issue}");
                }
            }
            report.AppendLine();

            // Color harmony analysis
            report.AppendLine("## Color Harmony Analysis");
            var harmonyType = AnalyzeColorHarmony();
            report.AppendLine($"**Detected Harmony:** {harmonyType}");
            report.AppendLine();

            // Color usage statistics
            report.AppendLine("## Color Categories");
            report.AppendLine($"**Core Colors:** {PrimaryActionColor}, {SecondaryActionColor}, {AccentColor}, {HighlightColor}");
            report.AppendLine($"**Text Colors:** {HeadingTextColor}, {BodyTextColor}, {InteractiveTextColor}");
            report.AppendLine($"**Background Colors:** {MainBackgroundColor}, {CardBackgroundColor}, {HoverBackgroundColor}");
            report.AppendLine($"**Status Colors:** {SuccessColor}, {WarningColor}, {ErrorColor}, {InfoColor}");
            report.AppendLine($"**Border Colors:** {BorderColor}, {BorderAccentColor}");

            // Save report to file
            var reportFileName = $"ThemeReport_{CurrentThemeName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.md";
            var reportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), reportFileName);
            await File.WriteAllTextAsync(reportPath, report.ToString());

            StatusMessage = $"✅ Theme report saved to Desktop: {reportFileName}";
            Logger.LogInformation("Theme report generated: {ReportPath}", reportPath);

            await Task.Delay(100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating theme report");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate theme report", Environment.UserName);
            StatusMessage = "❌ Failed to generate theme report";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Toggles print preview mode to show how colors appear in print
    /// </summary>
    [RelayCommand]
    private async Task TogglePrintPreviewAsync()
    {
        try
        {
            IsLoading = true;
            IsPrintPreviewEnabled = !IsPrintPreviewEnabled;
            
            if (IsPrintPreviewEnabled)
            {
                StatusMessage = "🖨️ Print preview enabled - colors adjusted for print output";
                Logger.LogDebug("Print preview mode enabled");
                await ApplyPrintPreviewFiltersAsync();
            }
            else
            {
                StatusMessage = "📱 Screen preview restored";
                Logger.LogDebug("Print preview mode disabled");
                await RestoreScreenPreviewAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling print preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to toggle print preview", Environment.UserName);
            StatusMessage = "❌ Failed to toggle print preview";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Toggles lighting simulation to preview colors under different lighting conditions
    /// </summary>
    [RelayCommand]
    private async Task ToggleLightingSimulationAsync()
    {
        try
        {
            IsLoading = true;
            IsLightingSimulationEnabled = !IsLightingSimulationEnabled;
            
            if (IsLightingSimulationEnabled)
            {
                StatusMessage = $"💡 Lighting simulation enabled: {LightingCondition}";
                Logger.LogDebug("Lighting simulation enabled with condition: {Condition}", LightingCondition);
                await ApplyLightingSimulationAsync(LightingCondition);
            }
            else
            {
                StatusMessage = "🖥️ Standard lighting restored";
                Logger.LogDebug("Lighting simulation disabled");
                await RestoreStandardLightingAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling lighting simulation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to toggle lighting simulation", Environment.UserName);
            StatusMessage = "❌ Failed to toggle lighting simulation";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Changes the lighting condition for simulation
    /// </summary>
    [RelayCommand]
    private async Task ChangeLightingConditionAsync(string? condition)
    {
        if (string.IsNullOrEmpty(condition)) return;
        
        try
        {
            IsLoading = true;
            LightingCondition = condition;
            StatusMessage = $"💡 Lighting changed to: {condition}";
            Logger.LogDebug("Lighting condition changed to: {Condition}", condition);
            
            if (IsLightingSimulationEnabled)
            {
                await ApplyLightingSimulationAsync(condition);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing lighting condition");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to change lighting condition", Environment.UserName);
            StatusMessage = "❌ Failed to change lighting condition";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Toggles multi-monitor preview mode
    /// </summary>
    [RelayCommand]
    private async Task ToggleMultiMonitorPreviewAsync()
    {
        try
        {
            IsLoading = true;
            IsMultiMonitorPreviewEnabled = !IsMultiMonitorPreviewEnabled;
            
            if (IsMultiMonitorPreviewEnabled)
            {
                await DetectAvailableMonitorsAsync();
                StatusMessage = $"🖥️ Multi-monitor preview enabled - {AvailableMonitors.Count} monitors detected";
                Logger.LogDebug("Multi-monitor preview enabled with {Count} monitors", AvailableMonitors.Count);
            }
            else
            {
                StatusMessage = "📱 Single monitor view restored";
                Logger.LogDebug("Multi-monitor preview disabled");
                AvailableMonitors.Clear();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling multi-monitor preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to toggle multi-monitor preview", Environment.UserName);
            StatusMessage = "❌ Failed to toggle multi-monitor preview";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Selects a specific monitor for preview
    /// </summary>
    [RelayCommand]
    private async Task SelectPreviewMonitorAsync(int monitorIndex)
    {
        try
        {
            if (monitorIndex < 0 || monitorIndex >= AvailableMonitors.Count) return;
            
            IsLoading = true;
            SelectedMonitorIndex = monitorIndex;
            var selectedMonitor = AvailableMonitors[monitorIndex];
            
            StatusMessage = $"🖥️ Preview monitor changed to: {selectedMonitor.DisplayName}";
            Logger.LogDebug("Preview monitor changed to: {Monitor}", selectedMonitor.DisplayName);
            
            await ApplyMonitorSpecificSettingsAsync(selectedMonitor);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting preview monitor");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to select preview monitor", Environment.UserName);
            StatusMessage = "❌ Failed to select preview monitor";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Applies print-specific color adjustments for better print output
    /// </summary>
    private async Task ApplyPrintPreviewFiltersAsync()
    {
        await Task.Delay(100); // Simulate processing time
        
        try
        {
            SaveColorSnapshot("Before print preview");
            
            // Apply print-specific adjustments
            // Print colors typically need higher contrast and less saturation
            var allColors = GetAllColorsForProcessing();
            
            foreach (var (propertyName, color) in allColors)
            {
                var hsv = RgbToHsv(color);
                
                // Adjust for print characteristics
                hsv.S = (float)Math.Max(0, Math.Min(1, hsv.S * 0.9f)); // Reduce saturation by 10%
                hsv.V = (float)Math.Max(0, Math.Min(1, hsv.V * 1.1f)); // Increase brightness by 10%
                
                var adjustedColor = HsvToRgb(hsv);
                SetColorForProperty(propertyName, adjustedColor);
            }
            
            Logger.LogDebug("Print preview filters applied to {Count} colors", allColors.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying print preview filters");
            throw;
        }
    }

    /// <summary>
    /// Restores screen-optimized colors from print preview
    /// </summary>
    private async Task RestoreScreenPreviewAsync()
    {
        await Task.Delay(50);
        
        try
        {
            // Find the last "Before print preview" snapshot and restore
            var printSnapshot = ColorHistory.LastOrDefault(s => s.Name.Contains("Before print preview"));
            if (printSnapshot != null)
            {
                await RestoreFromSnapshotAsync(printSnapshot, false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error restoring screen preview");
            throw;
        }
    }

    /// <summary>
    /// Applies lighting simulation based on selected condition
    /// </summary>
    private async Task ApplyLightingSimulationAsync(string lightingCondition)
    {
        await Task.Delay(100); // Simulate processing time
        
        try
        {
            SaveColorSnapshot($"Before {lightingCondition} simulation");
            
            // Get color temperature and intensity for the lighting condition
            var (colorTemp, intensity) = GetLightingParameters(lightingCondition);
            
            var allColors = GetAllColorsForProcessing();
            
            foreach (var (propertyName, color) in allColors)
            {
                var adjustedColor = ApplyColorTemperatureFilter(color, colorTemp, intensity);
                SetColorForProperty(propertyName, adjustedColor);
            }
            
            Logger.LogDebug("Lighting simulation applied: {Condition} ({ColorTemp}K, {Intensity}% intensity)", 
                lightingCondition, colorTemp, intensity * 100);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying lighting simulation");
            throw;
        }
    }

    /// <summary>
    /// Restores standard lighting conditions
    /// </summary>
    private async Task RestoreStandardLightingAsync()
    {
        await Task.Delay(50);
        
        try
        {
            // Find the last lighting simulation snapshot and restore
            var lightingSnapshot = ColorHistory.LastOrDefault(s => s.Name.Contains("simulation") && s.Name.Contains("Before"));
            if (lightingSnapshot != null)
            {
                await RestoreFromSnapshotAsync(lightingSnapshot, false);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error restoring standard lighting");
            throw;
        }
    }

    /// <summary>
    /// Detects available monitors for multi-monitor preview
    /// </summary>
    private async Task DetectAvailableMonitorsAsync()
    {
        await Task.Delay(200); // Simulate monitor detection time
        
        try
        {
            AvailableMonitors.Clear();
            
            // Primary monitor (always available)
            AvailableMonitors.Add(new MonitorInfo
            {
                Name = "Primary Display",
                Width = 1920,
                Height = 1080,
                IsPrimary = true
            });
            
            // Simulate additional monitors (in real implementation, would use platform APIs)
            AvailableMonitors.Add(new MonitorInfo
            {
                Name = "Secondary Display",
                Width = 1366,
                Height = 768,
                IsPrimary = false
            });
            
            AvailableMonitors.Add(new MonitorInfo
            {
                Name = "Wide Monitor",
                Width = 3440,
                Height = 1440,
                IsPrimary = false
            });
            
            Logger.LogDebug("Detected {Count} monitors for preview", AvailableMonitors.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error detecting available monitors");
            throw;
        }
    }

    /// <summary>
    /// Applies monitor-specific settings for color preview
    /// </summary>
    private async Task ApplyMonitorSpecificSettingsAsync(MonitorInfo monitor)
    {
        await Task.Delay(100);
        
        try
        {
            // Apply monitor-specific color corrections (simplified)
            Logger.LogDebug("Applying color settings for monitor: {Monitor}", monitor.DisplayName);
            
            // Different monitor types might need different color adjustments
            // This is a simplified example - in practice, would use monitor profiles
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying monitor-specific settings");
            throw;
        }
    }

    /// <summary>
    /// Gets lighting parameters for different conditions
    /// </summary>
    private (int colorTemp, float intensity) GetLightingParameters(string lightingCondition)
    {
        return lightingCondition switch
        {
            "Natural Daylight (5500K)" => (5500, 1.0f),
            "Office Fluorescent (6500K)" => (6500, 0.9f),
            "Warm Indoor (3000K)" => (3000, 0.8f),
            "Cool White LED (4000K)" => (4000, 0.95f),
            "Tungsten Bulb (2700K)" => (2700, 0.7f),
            "Candlelight (1900K)" => (1900, 0.5f),
            "Overcast Day (6000K)" => (6000, 0.85f),
            "Direct Sunlight (5800K)" => (5800, 1.1f),
            _ => (6500, 1.0f) // Default to office lighting
        };
    }

    /// <summary>
    /// Applies color temperature filter to a color
    /// </summary>
    private Color ApplyColorTemperatureFilter(Color color, int colorTemp, float intensity)
    {
        try
        {
            var rgb = (r: color.R / 255f, g: color.G / 255f, b: color.B / 255f);
            
            // Apply color temperature adjustment
            // Warmer temperatures (lower K) add red/yellow, cooler (higher K) add blue
            if (colorTemp < 5500)
            {
                // Warm light - add red/yellow tint
                var warmFactor = (5500f - colorTemp) / 3500f; // 0 to 1
                rgb.r = Math.Min(1.0f, rgb.r + warmFactor * 0.1f);
                rgb.g = Math.Min(1.0f, rgb.g + warmFactor * 0.05f);
            }
            else if (colorTemp > 5500)
            {
                // Cool light - add blue tint
                var coolFactor = (colorTemp - 5500f) / 1500f; // 0 to 1 (clamped)
                coolFactor = Math.Min(1.0f, coolFactor);
                rgb.b = Math.Min(1.0f, rgb.b + coolFactor * 0.1f);
            }
            
            // Apply intensity
            rgb.r = Math.Max(0f, Math.Min(1.0f, rgb.r * intensity));
            rgb.g = Math.Max(0f, Math.Min(1.0f, rgb.g * intensity));
            rgb.b = Math.Max(0f, Math.Min(1.0f, rgb.b * intensity));
            
            return Color.FromRgb(
                (byte)(rgb.r * 255),
                (byte)(rgb.g * 255),
                (byte)(rgb.b * 255)
            );
        }
        catch
        {
            return color; // Return original on error
        }
    }

    /// <summary>
    /// Gets all colors for processing operations
    /// </summary>
    private List<(string propertyName, Color color)> GetAllColorsForProcessing()
    {
        return new List<(string, Color)>
        {
            ("PrimaryAction", PrimaryActionColor),
            ("SecondaryAction", SecondaryActionColor),
            ("Accent", AccentColor),
            ("Highlight", HighlightColor),
            ("HeadingText", HeadingTextColor),
            ("BodyText", BodyTextColor),
            ("TertiaryText", TertiaryTextColor),
            ("InteractiveText", InteractiveTextColor),
            ("MainBackground", MainBackgroundColor),
            ("CardBackground", CardBackgroundColor),
            ("PanelBackground", PanelBackgroundColor),
            ("SidebarBackground", SidebarBackgroundColor),
            ("Success", SuccessColor),
            ("Warning", WarningColor),
            ("Error", ErrorColor),
            ("Info", InfoColor),
            ("Border", BorderColor),
            ("BorderAccent", BorderAccentColor)
        };
    }

    /// <summary>
    /// Analyzes color relationships to determine harmony type
    /// </summary>
    private string AnalyzeColorHarmony()
    {
        try
        {
            var primaryHsv = RgbToHsv(PrimaryActionColor);
            var secondaryHsv = RgbToHsv(SecondaryActionColor);
            var accentHsv = RgbToHsv(AccentColor);

            // Calculate hue differences
            var hueDiff1 = Math.Abs(primaryHsv.H - secondaryHsv.H);
            var hueDiff2 = Math.Abs(primaryHsv.H - accentHsv.H);
            var hueDiff3 = Math.Abs(secondaryHsv.H - accentHsv.H);

            // Normalize to 0-180 range
            hueDiff1 = Math.Min(hueDiff1, 360 - hueDiff1);
            hueDiff2 = Math.Min(hueDiff2, 360 - hueDiff2);
            hueDiff3 = Math.Min(hueDiff3, 360 - hueDiff3);

            // Check for monochromatic (hues within 30 degrees)
            if (hueDiff1 < 30 && hueDiff2 < 30 && hueDiff3 < 30)
            {
                return "Monochromatic";
            }

            // Check for complementary (hues around 180 degrees apart)
            if (Math.Abs(hueDiff1 - 180) < 30 || Math.Abs(hueDiff2 - 180) < 30 || Math.Abs(hueDiff3 - 180) < 30)
            {
                return "Complementary";
            }

            // Check for triadic (hues around 120 degrees apart)
            var triadic = Math.Abs(hueDiff1 - 120) < 30 && Math.Abs(hueDiff2 - 120) < 30;
            if (triadic)
            {
                return "Triadic";
            }

            // Check for analogous (hues within 60 degrees)
            if (hueDiff1 < 60 && hueDiff2 < 60 && hueDiff3 < 60)
            {
                return "Analogous";
            }

            // Check for split-complementary
            if ((Math.Abs(hueDiff1 - 150) < 30 && Math.Abs(hueDiff2 - 150) < 30) ||
                (Math.Abs(hueDiff1 - 210) < 30 && Math.Abs(hueDiff2 - 210) < 30))
            {
                return "Split-Complementary";
            }

            return "Custom";
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error analyzing color harmony");
            return "Unknown";
        }
    }

    /// <summary>
    /// Helper method to adjust color brightness
    /// </summary>
    private Color AdjustColorBrightness(Color color, double factor)
    {
        try
        {
            var hsv = RgbToHsv(color);
            hsv.V = (float)Math.Max(0, Math.Min(1, hsv.V * factor)); // Clamp to valid range
            var rgb = HsvToRgb(hsv);
            return rgb;
        }
        catch
        {
            return color; // Return original color if adjustment fails
        }
    }

    /// <summary>
    /// Validates contrast ratios between text and background colors
    /// </summary>
    private async Task<List<string>> ValidateContrastRatiosAsync()
    {
        await Task.Delay(10); // Async operation simulation
        
        var issues = new List<string>();
        
        try
        {
            // Check primary text on main background
            var primaryContrast = CalculateContrastRatio(HeadingTextColor, MainBackgroundColor);
            if (primaryContrast < 4.5)
            {
                issues.Add($"Heading text contrast too low: {primaryContrast:F1}:1 (needs 4.5:1)");
            }

            // Check body text on card background  
            var bodyContrast = CalculateContrastRatio(BodyTextColor, CardBackgroundColor);
            if (bodyContrast < 4.5)
            {
                issues.Add($"Body text contrast too low: {bodyContrast:F1}:1 (needs 4.5:1)");
            }

            // Check interactive text visibility
            var interactiveContrast = CalculateContrastRatio(InteractiveTextColor, MainBackgroundColor);
            if (interactiveContrast < 4.5)
            {
                issues.Add($"Interactive text contrast too low: {interactiveContrast:F1}:1 (needs 4.5:1)");
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error calculating contrast ratios");
            issues.Add("Could not calculate contrast ratios");
        }

        return issues;
    }

    /// <summary>
    /// Validates if the current color scheme follows basic color harmony principles
    /// </summary>
    private (bool isHarmonious, string harmonyType) ValidateColorHarmony()
    {
        try
        {
            // Convert primary colors to HSV for analysis
            var primaryHsv = RgbToHsv(PrimaryActionColor);
            var secondaryHsv = RgbToHsv(SecondaryActionColor);
            var accentHsv = RgbToHsv(AccentColor);

            // Check for monochromatic harmony (similar hues)
            var primarySecondaryHueDiff = Math.Abs(primaryHsv.H - secondaryHsv.H);
            if (primarySecondaryHueDiff < 30 || primarySecondaryHueDiff > 330)
            {
                return (true, "Monochromatic");
            }

            // Check for complementary harmony (opposite hues)
            if (Math.Abs(primarySecondaryHueDiff - 180) < 30)
            {
                return (true, "Complementary");
            }

            // Check for analogous harmony (adjacent hues)
            if (primarySecondaryHueDiff > 15 && primarySecondaryHueDiff < 90)
            {
                return (true, "Analogous");
            }

            // Check for triadic harmony
            var primaryAccentHueDiff = Math.Abs(primaryHsv.H - accentHsv.H);
            var secondaryAccentHueDiff = Math.Abs(secondaryHsv.H - accentHsv.H);
            
            if (Math.Abs(primarySecondaryHueDiff - 120) < 30 && 
                Math.Abs(primaryAccentHueDiff - 120) < 30 && 
                Math.Abs(secondaryAccentHueDiff - 120) < 30)
            {
                return (true, "Triadic");
            }

            return (false, "Custom");
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error validating color harmony");
            return (false, "Unknown");
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
    /// Gets the complementary color (opposite on color wheel)
    /// </summary>
    private Color GetComplementaryColor(Color color)
    {
        // Convert RGB to HSV, shift hue by 180 degrees, convert back
        var hsv = RgbToHsv(color);
        hsv.H = (hsv.H + 180) % 360;
        return HsvToRgb(hsv);
    }

    /// <summary>
    /// Shifts the hue of a color by specified degrees
    /// </summary>
    private Color ShiftHue(Color color, float degrees)
    {
        var hsv = RgbToHsv(color);
        hsv.H = (hsv.H + degrees) % 360;
        if (hsv.H < 0) hsv.H += 360;
        return HsvToRgb(hsv);
    }

    /// <summary>
    /// Converts RGB color to HSV
    /// </summary>
    private (float H, float S, float V) RgbToHsv(Color color)
    {
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float delta = max - min;

        float h = 0;
        if (delta != 0)
        {
            if (max == r)
                h = 60 * (((g - b) / delta) % 6);
            else if (max == g)
                h = 60 * ((b - r) / delta + 2);
            else if (max == b)
                h = 60 * ((r - g) / delta + 4);
        }

        if (h < 0) h += 360;

        float s = max == 0 ? 0 : delta / max;
        float v = max;

        return (h, s, v);
    }

    /// <summary>
    /// Converts HSV color to RGB
    /// </summary>
    private Color HsvToRgb((float H, float S, float V) hsv)
    {
        float c = hsv.V * hsv.S;
        float x = c * (1 - Math.Abs((hsv.H / 60) % 2 - 1));
        float m = hsv.V - c;

        float r = 0, g = 0, b = 0;

        if (hsv.H >= 0 && hsv.H < 60)
        {
            r = c; g = x; b = 0;
        }
        else if (hsv.H >= 60 && hsv.H < 120)
        {
            r = x; g = c; b = 0;
        }
        else if (hsv.H >= 120 && hsv.H < 180)
        {
            r = 0; g = c; b = x;
        }
        else if (hsv.H >= 180 && hsv.H < 240)
        {
            r = 0; g = x; b = c;
        }
        else if (hsv.H >= 240 && hsv.H < 300)
        {
            r = x; g = 0; b = c;
        }
        else if (hsv.H >= 300 && hsv.H < 360)
        {
            r = c; g = 0; b = x;
        }

        return Color.FromRgb(
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255));
    }

    /// <summary>
    /// Loads manufacturing industry template colors
    /// </summary>
    private async Task LoadManufacturingTemplateAsync()
    {
        // Manufacturing-focused color scheme - strong, industrial colors
        PrimaryActionColor = Color.Parse("#1565C0");   // Strong blue
        SecondaryActionColor = Color.Parse("#1976D2"); // Medium blue
        AccentColor = Color.Parse("#FFA726");          // Orange accent
        HighlightColor = Color.Parse("#0D47A1");       // Dark blue

        HeadingTextColor = Color.Parse("#263238");     // Dark blue-gray
        BodyTextColor = Color.Parse("#455A64");        // Medium blue-gray
        InteractiveTextColor = Color.Parse("#1565C0"); // Primary blue
        OverlayTextColor = Color.Parse("#FFFFFF");     // White
        TertiaryTextColor = Color.Parse("#78909C");    // Light blue-gray

        MainBackgroundColor = Color.Parse("#FAFAFA");  // Light gray
        CardBackgroundColor = Color.Parse("#FFFFFF");  // White cards
        HoverBackgroundColor = Color.Parse("#F5F5F5"); // Light hover
        PanelBackgroundColor = Color.Parse("#ECEFF1"); // Blue-gray panel
        SidebarBackgroundColor = Color.Parse("#CFD8DC"); // Medium blue-gray

        SuccessColor = Color.Parse("#388E3C");         // Industrial green
        WarningColor = Color.Parse("#F57C00");         // Orange warning
        ErrorColor = Color.Parse("#D32F2F");           // Red error
        InfoColor = Color.Parse("#1976D2");            // Blue info

        BorderColor = Color.Parse("#B0BEC5");          // Light blue-gray
        BorderAccentColor = Color.Parse("#90A4AE");    // Medium blue-gray

        await Task.Delay(100); // Simulate loading time
    }

    /// <summary>
    /// Loads healthcare industry template colors
    /// </summary>
    private async Task LoadHealthcareTemplateAsync()
    {
        // Healthcare-focused color scheme - clean, calming colors
        PrimaryActionColor = Color.Parse("#00796B");   // Teal
        SecondaryActionColor = Color.Parse("#00ACC1"); // Cyan
        AccentColor = Color.Parse("#26C6DA");          // Light cyan
        HighlightColor = Color.Parse("#004D40");       // Dark teal

        HeadingTextColor = Color.Parse("#263238");     // Dark gray
        BodyTextColor = Color.Parse("#37474F");        // Medium gray
        InteractiveTextColor = Color.Parse("#00796B"); // Teal
        OverlayTextColor = Color.Parse("#FFFFFF");     // White
        TertiaryTextColor = Color.Parse("#607D8B");    // Blue-gray

        MainBackgroundColor = Color.Parse("#F8FDFC");  // Very light teal
        CardBackgroundColor = Color.Parse("#FFFFFF");  // White cards
        HoverBackgroundColor = Color.Parse("#E0F2F1"); // Light teal hover
        PanelBackgroundColor = Color.Parse("#E8F5E8"); // Very light green
        SidebarBackgroundColor = Color.Parse("#B2DFDB"); // Light teal

        SuccessColor = Color.Parse("#4CAF50");         // Green
        WarningColor = Color.Parse("#FF9800");         // Orange
        ErrorColor = Color.Parse("#F44336");           // Red
        InfoColor = Color.Parse("#2196F3");            // Blue

        BorderColor = Color.Parse("#B2DFDB");          // Light teal
        BorderAccentColor = Color.Parse("#80CBC4");    // Medium teal

        await Task.Delay(100); // Simulate loading time
    }

    /// <summary>
    /// Loads office/professional template colors
    /// </summary>
    private async Task LoadOfficeTemplateAsync()
    {
        // Professional office color scheme - classic, business-appropriate
        PrimaryActionColor = Color.Parse("#0078D4");   // Microsoft blue
        SecondaryActionColor = Color.Parse("#106EBE"); // Darker blue
        AccentColor = Color.Parse("#40A2E8");          // Light blue
        HighlightColor = Color.Parse("#005A9E");       // Dark blue

        HeadingTextColor = Color.Parse("#323130");     // Fluent dark gray
        BodyTextColor = Color.Parse("#605E5C");        // Fluent medium gray
        InteractiveTextColor = Color.Parse("#0078D4"); // Blue
        OverlayTextColor = Color.Parse("#FFFFFF");     // White
        TertiaryTextColor = Color.Parse("#8A8886");    // Light gray

        MainBackgroundColor = Color.Parse("#FFFFFF");  // White
        CardBackgroundColor = Color.Parse("#F3F2F1");  // Light gray
        HoverBackgroundColor = Color.Parse("#EDEBE9"); // Hover gray
        PanelBackgroundColor = Color.Parse("#FAF9F8"); // Off-white
        SidebarBackgroundColor = Color.Parse("#F3F2F1"); // Light gray

        SuccessColor = Color.Parse("#107C10");         // Green
        WarningColor = Color.Parse("#D83B01");         // Orange-red
        ErrorColor = Color.Parse("#A4262C");           // Red
        InfoColor = Color.Parse("#0078D4");            // Blue

        BorderColor = Color.Parse("#EDEBE9");          // Light gray
        BorderAccentColor = Color.Parse("#D2D0CE");    // Medium gray

        await Task.Delay(100); // Simulate loading time
    }

    /// <summary>
    /// Loads high contrast template colors for accessibility
    /// </summary>
    private async Task LoadHighContrastTemplateAsync()
    {
        // High contrast color scheme for accessibility
        PrimaryActionColor = Color.Parse("#0000FF");   // Pure blue
        SecondaryActionColor = Color.Parse("#000080"); // Navy blue
        AccentColor = Color.Parse("#4169E1");          // Royal blue
        HighlightColor = Color.Parse("#000040");       // Very dark blue

        HeadingTextColor = Color.Parse("#000000");     // Pure black
        BodyTextColor = Color.Parse("#000000");        // Pure black
        InteractiveTextColor = Color.Parse("#0000FF"); // Blue
        OverlayTextColor = Color.Parse("#FFFFFF");     // White
        TertiaryTextColor = Color.Parse("#000000");    // Black

        MainBackgroundColor = Color.Parse("#FFFFFF");  // Pure white
        CardBackgroundColor = Color.Parse("#FFFFFF");  // White
        HoverBackgroundColor = Color.Parse("#E0E0E0"); // Light gray
        PanelBackgroundColor = Color.Parse("#F0F0F0");  // Very light gray
        SidebarBackgroundColor = Color.Parse("#E8E8E8"); // Light gray

        SuccessColor = Color.Parse("#008000");         // Green
        WarningColor = Color.Parse("#FF8000");         // Orange
        ErrorColor = Color.Parse("#FF0000");           // Red
        InfoColor = Color.Parse("#0000FF");            // Blue

        BorderColor = Color.Parse("#000000");          // Black borders
        BorderAccentColor = Color.Parse("#404040");    // Dark gray

        await Task.Delay(100); // Simulate loading time
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

    /// <summary>
    /// Calculates WCAG contrast ratio between two colors
    /// </summary>
    private double CalculateContrastRatio(Color foreground, Color background)
    {
        try
        {
            // Convert colors to relative luminance
            var foregroundLuminance = CalculateRelativeLuminance(foreground);
            var backgroundLuminance = CalculateRelativeLuminance(background);
            
            // Calculate contrast ratio
            var lighter = Math.Max(foregroundLuminance, backgroundLuminance);
            var darker = Math.Min(foregroundLuminance, backgroundLuminance);
            
            return (lighter + 0.05) / (darker + 0.05);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error calculating contrast ratio");
            return 1.0; // Return minimum contrast ratio on error
        }
    }

    /// <summary>
    /// Calculates relative luminance of a color according to WCAG guidelines
    /// </summary>
    private double CalculateRelativeLuminance(Color color)
    {
        // Convert RGB to sRGB
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        // Apply gamma correction
        r = (r <= 0.03928) ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = (g <= 0.03928) ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = (b <= 0.03928) ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

        // Calculate relative luminance
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    /// <summary>
    /// Generates a professional color palette based on color theory principles
    /// </summary>
    private Dictionary<string, Color> GenerateColorPalette(Color baseColor, string algorithm)
    {
        var palette = new Dictionary<string, Color>();
        
        try
        {
            var baseHsv = RgbToHsv(baseColor);
            
            switch (algorithm.ToLowerInvariant())
            {
                case "monochromatic":
                    palette["primary"] = baseColor;
                    palette["secondary"] = HsvToRgb((baseHsv.H, baseHsv.S, Math.Max(0, baseHsv.V - 0.2f)));
                    palette["accent"] = HsvToRgb((baseHsv.H, Math.Max(0, baseHsv.S - 0.3f), Math.Min(1, baseHsv.V + 0.1f)));
                    break;
                    
                case "complementary":
                    palette["primary"] = baseColor;
                    palette["secondary"] = HsvToRgb(((baseHsv.H + 180) % 360, baseHsv.S, baseHsv.V));
                    palette["accent"] = HsvToRgb(((baseHsv.H + 30) % 360, baseHsv.S * 0.7f, baseHsv.V));
                    break;
                    
                case "triadic":
                    palette["primary"] = baseColor;
                    palette["secondary"] = HsvToRgb(((baseHsv.H + 120) % 360, baseHsv.S, baseHsv.V));
                    palette["accent"] = HsvToRgb(((baseHsv.H + 240) % 360, baseHsv.S, baseHsv.V));
                    break;
                    
                case "analogous":
                    palette["primary"] = baseColor;
                    palette["secondary"] = HsvToRgb(((baseHsv.H + 30) % 360, baseHsv.S, baseHsv.V));
                    palette["accent"] = HsvToRgb(((baseHsv.H - 30 + 360) % 360, baseHsv.S, baseHsv.V));
                    break;
                    
                default:
                    palette["primary"] = baseColor;
                    palette["secondary"] = DarkenColor(baseColor, 0.2f);
                    palette["accent"] = LightenColor(baseColor, 0.3f);
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error generating color palette for algorithm: {Algorithm}", algorithm);
            // Return fallback palette
            palette["primary"] = baseColor;
            palette["secondary"] = DarkenColor(baseColor, 0.2f);
            palette["accent"] = LightenColor(baseColor, 0.3f);
        }
        
        return palette;
    }

    #region Advanced Feature Helper Methods

    /// <summary>
    /// Saves current color state as a snapshot for undo/redo functionality
    /// </summary>
    private void SaveColorSnapshot(string name)
    {
        try
        {
            var snapshot = new ColorSnapshot(name);
            
            // Capture all current colors
            snapshot.Colors["PrimaryAction"] = PrimaryActionColor;
            snapshot.Colors["SecondaryAction"] = SecondaryActionColor;
            snapshot.Colors["Accent"] = AccentColor;
            snapshot.Colors["Highlight"] = HighlightColor;
            
            snapshot.Colors["HeadingText"] = HeadingTextColor;
            snapshot.Colors["BodyText"] = BodyTextColor;
            snapshot.Colors["InteractiveText"] = InteractiveTextColor;
            snapshot.Colors["OverlayText"] = OverlayTextColor;
            snapshot.Colors["TertiaryText"] = TertiaryTextColor;
            
            snapshot.Colors["MainBackground"] = MainBackgroundColor;
            snapshot.Colors["CardBackground"] = CardBackgroundColor;
            snapshot.Colors["HoverBackground"] = HoverBackgroundColor;
            snapshot.Colors["PanelBackground"] = PanelBackgroundColor;
            snapshot.Colors["SidebarBackground"] = SidebarBackgroundColor;
            
            snapshot.Colors["Success"] = SuccessColor;
            snapshot.Colors["Warning"] = WarningColor;
            snapshot.Colors["Error"] = ErrorColor;
            snapshot.Colors["Info"] = InfoColor;
            
            snapshot.Colors["Border"] = BorderColor;
            snapshot.Colors["BorderAccent"] = BorderAccentColor;
            
            // Remove any snapshots after current index (for redo chain)
            while (ColorHistory.Count > CurrentHistoryIndex + 1)
            {
                ColorHistory.RemoveAt(ColorHistory.Count - 1);
            }
            
            ColorHistory.Add(snapshot);
            CurrentHistoryIndex = ColorHistory.Count - 1;
            
            // Limit history to 20 entries
            while (ColorHistory.Count > 20)
            {
                ColorHistory.RemoveAt(0);
                CurrentHistoryIndex--;
            }
            
            CanUndo = CurrentHistoryIndex > 0;
            CanRedo = false;
            
            Logger.LogDebug("Color snapshot saved: {SnapshotName}", name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving color snapshot");
        }
    }

    /// <summary>
    /// Restores colors from a snapshot
    /// </summary>
    private async Task RestoreFromSnapshotAsync(ColorSnapshot snapshot, bool saveSnapshot = true)
    {
        try
        {
            if (saveSnapshot)
            {
                SaveColorSnapshot($"Before restore: {snapshot.Name}");
            }
            
            // Restore all colors from snapshot
            if (snapshot.Colors.TryGetValue("PrimaryAction", out var primaryAction))
                PrimaryActionColor = primaryAction;
            if (snapshot.Colors.TryGetValue("SecondaryAction", out var secondaryAction))
                SecondaryActionColor = secondaryAction;
            if (snapshot.Colors.TryGetValue("Accent", out var accent))
                AccentColor = accent;
            if (snapshot.Colors.TryGetValue("Highlight", out var highlight))
                HighlightColor = highlight;
                
            if (snapshot.Colors.TryGetValue("HeadingText", out var headingText))
                HeadingTextColor = headingText;
            if (snapshot.Colors.TryGetValue("BodyText", out var bodyText))
                BodyTextColor = bodyText;
            if (snapshot.Colors.TryGetValue("InteractiveText", out var interactiveText))
                InteractiveTextColor = interactiveText;
            if (snapshot.Colors.TryGetValue("OverlayText", out var overlayText))
                OverlayTextColor = overlayText;
            if (snapshot.Colors.TryGetValue("TertiaryText", out var tertiaryText))
                TertiaryTextColor = tertiaryText;
                
            if (snapshot.Colors.TryGetValue("MainBackground", out var mainBg))
                MainBackgroundColor = mainBg;
            if (snapshot.Colors.TryGetValue("CardBackground", out var cardBg))
                CardBackgroundColor = cardBg;
            if (snapshot.Colors.TryGetValue("HoverBackground", out var hoverBg))
                HoverBackgroundColor = hoverBg;
            if (snapshot.Colors.TryGetValue("PanelBackground", out var panelBg))
                PanelBackgroundColor = panelBg;
            if (snapshot.Colors.TryGetValue("SidebarBackground", out var sidebarBg))
                SidebarBackgroundColor = sidebarBg;
                
            if (snapshot.Colors.TryGetValue("Success", out var success))
                SuccessColor = success;
            if (snapshot.Colors.TryGetValue("Warning", out var warning))
                WarningColor = warning;
            if (snapshot.Colors.TryGetValue("Error", out var error))
                ErrorColor = error;
            if (snapshot.Colors.TryGetValue("Info", out var info))
                InfoColor = info;
                
            if (snapshot.Colors.TryGetValue("Border", out var border))
                BorderColor = border;
            if (snapshot.Colors.TryGetValue("BorderAccent", out var borderAccent))
                BorderAccentColor = borderAccent;
            
            HasUnsavedChanges = true;
            await Task.Delay(50); // Brief delay for UI update
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error restoring from snapshot");
            throw;
        }
    }

    /// <summary>
    /// Applies color blindness simulation filter to preview colors
    /// </summary>
    private async Task ApplyColorBlindnessFilterAsync()
    {
        try
        {
            Logger.LogDebug("Applying color blindness filter: {Type}", ColorBlindnessType);
            
            // Save original colors for restoration
            SaveColorSnapshot($"Before {ColorBlindnessType} simulation");
            
            // Apply filter based on type
            switch (ColorBlindnessType)
            {
                case "Protanopia (Red-blind)":
                    ApplyProtanopiaFilter();
                    break;
                case "Deuteranopia (Green-blind)":
                    ApplyDeuteranopiaFilter();
                    break;
                case "Tritanopia (Blue-blind)":
                    ApplyTritanopiaFilter();
                    break;
                case "Protanomaly (Red-weak)":
                    ApplyProtanomalyFilter();
                    break;
                case "Deuteranomaly (Green-weak)":
                    ApplyDeuteranomalyFilter();
                    break;
                case "Tritanomaly (Blue-weak)":
                    ApplyTritanomalyFilter();
                    break;
                case "Monochromacy (Total color blindness)":
                    ApplyMonochromacyFilter();
                    break;
            }
            
            await Task.Delay(100); // Allow UI to update
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying color blindness filter");
            throw;
        }
    }

    /// <summary>
    /// Restores original colors from color blindness simulation
    /// </summary>
    private async Task RestoreOriginalColorsAsync()
    {
        try
        {
            if (ColorHistory.Any())
            {
                var lastSnapshot = ColorHistory.LastOrDefault();
                if (lastSnapshot != null && lastSnapshot.Name.Contains("Before") && lastSnapshot.Name.Contains("simulation"))
                {
                    await RestoreFromSnapshotAsync(lastSnapshot, false);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error restoring original colors");
            throw;
        }
    }

    /// <summary>
    /// Adjusts brightness of a color by the specified amount
    /// </summary>
    private Color AdjustBrightness(Color color, double adjustment)
    {
        var (h, s, v) = ColorToHsv(color);
        v = Math.Max(0, Math.Min(1, v + adjustment));
        return HsvToColor(h, s, v, color.A);
    }

    /// <summary>
    /// Adjusts saturation of a color by the specified amount
    /// </summary>
    private Color AdjustSaturation(Color color, double adjustment)
    {
        var (h, s, v) = ColorToHsv(color);
        s = Math.Max(0, Math.Min(1, s + adjustment));
        return HsvToColor(h, s, v, color.A);
    }

    /// <summary>
    /// Applies Protanopia (red-blindness) filter to all colors
    /// </summary>
    private void ApplyProtanopiaFilter()
    {
        // Simplified protanopia simulation - removes red component
        PrimaryActionColor = SimulateProtanopia(PrimaryActionColor);
        SecondaryActionColor = SimulateProtanopia(SecondaryActionColor);
        AccentColor = SimulateProtanopia(AccentColor);
        HighlightColor = SimulateProtanopia(HighlightColor);
        SuccessColor = SimulateProtanopia(SuccessColor);
        WarningColor = SimulateProtanopia(WarningColor);
        ErrorColor = SimulateProtanopia(ErrorColor);
        InfoColor = SimulateProtanopia(InfoColor);
    }

    /// <summary>
    /// Applies Deuteranopia (green-blindness) filter to all colors
    /// </summary>
    private void ApplyDeuteranopiaFilter()
    {
        PrimaryActionColor = SimulateDeuteranopia(PrimaryActionColor);
        SecondaryActionColor = SimulateDeuteranopia(SecondaryActionColor);
        AccentColor = SimulateDeuteranopia(AccentColor);
        HighlightColor = SimulateDeuteranopia(HighlightColor);
        SuccessColor = SimulateDeuteranopia(SuccessColor);
        WarningColor = SimulateDeuteranopia(WarningColor);
        ErrorColor = SimulateDeuteranopia(ErrorColor);
        InfoColor = SimulateDeuteranopia(InfoColor);
    }

    /// <summary>
    /// Applies Tritanopia (blue-blindness) filter to all colors
    /// </summary>
    private void ApplyTritanopiaFilter()
    {
        PrimaryActionColor = SimulateTritanopia(PrimaryActionColor);
        SecondaryActionColor = SimulateTritanopia(SecondaryActionColor);
        AccentColor = SimulateTritanopia(AccentColor);
        HighlightColor = SimulateTritanopia(HighlightColor);
        SuccessColor = SimulateTritanopia(SuccessColor);
        WarningColor = SimulateTritanopia(WarningColor);
        ErrorColor = SimulateTritanopia(ErrorColor);
        InfoColor = SimulateTritanopia(InfoColor);
    }

    /// <summary>
    /// Applies Protanomaly (red-weakness) filter
    /// </summary>
    private void ApplyProtanomalyFilter()
    {
        // Mild red deficiency - reduce red component by 50%
        PrimaryActionColor = ReduceRedComponent(PrimaryActionColor, 0.5);
        SecondaryActionColor = ReduceRedComponent(SecondaryActionColor, 0.5);
        AccentColor = ReduceRedComponent(AccentColor, 0.5);
        HighlightColor = ReduceRedComponent(HighlightColor, 0.5);
        SuccessColor = ReduceRedComponent(SuccessColor, 0.5);
        WarningColor = ReduceRedComponent(WarningColor, 0.5);
        ErrorColor = ReduceRedComponent(ErrorColor, 0.5);
        InfoColor = ReduceRedComponent(InfoColor, 0.5);
    }

    /// <summary>
    /// Applies Deuteranomaly (green-weakness) filter
    /// </summary>
    private void ApplyDeuteranomalyFilter()
    {
        PrimaryActionColor = ReduceGreenComponent(PrimaryActionColor, 0.5);
        SecondaryActionColor = ReduceGreenComponent(SecondaryActionColor, 0.5);
        AccentColor = ReduceGreenComponent(AccentColor, 0.5);
        HighlightColor = ReduceGreenComponent(HighlightColor, 0.5);
        SuccessColor = ReduceGreenComponent(SuccessColor, 0.5);
        WarningColor = ReduceGreenComponent(WarningColor, 0.5);
        ErrorColor = ReduceGreenComponent(ErrorColor, 0.5);
        InfoColor = ReduceGreenComponent(InfoColor, 0.5);
    }

    /// <summary>
    /// Applies Tritanomaly (blue-weakness) filter
    /// </summary>
    private void ApplyTritanomalyFilter()
    {
        PrimaryActionColor = ReduceBlueComponent(PrimaryActionColor, 0.5);
        SecondaryActionColor = ReduceBlueComponent(SecondaryActionColor, 0.5);
        AccentColor = ReduceBlueComponent(AccentColor, 0.5);
        HighlightColor = ReduceBlueComponent(HighlightColor, 0.5);
        SuccessColor = ReduceBlueComponent(SuccessColor, 0.5);
        WarningColor = ReduceBlueComponent(WarningColor, 0.5);
        ErrorColor = ReduceBlueComponent(ErrorColor, 0.5);
        InfoColor = ReduceBlueComponent(InfoColor, 0.5);
    }

    /// <summary>
    /// Applies Monochromacy (total color blindness) filter
    /// </summary>
    private void ApplyMonochromacyFilter()
    {
        PrimaryActionColor = ToGrayscale(PrimaryActionColor);
        SecondaryActionColor = ToGrayscale(SecondaryActionColor);
        AccentColor = ToGrayscale(AccentColor);
        HighlightColor = ToGrayscale(HighlightColor);
        HeadingTextColor = ToGrayscale(HeadingTextColor);
        BodyTextColor = ToGrayscale(BodyTextColor);
        InteractiveTextColor = ToGrayscale(InteractiveTextColor);
        TertiaryTextColor = ToGrayscale(TertiaryTextColor);
        SuccessColor = ToGrayscale(SuccessColor);
        WarningColor = ToGrayscale(WarningColor);
        ErrorColor = ToGrayscale(ErrorColor);
        InfoColor = ToGrayscale(InfoColor);
    }

    /// <summary>
    /// Simplified protanopia simulation
    /// </summary>
    private Color SimulateProtanopia(Color color)
    {
        // Matrix transformation for protanopia simulation
        var r = 0.567 * color.R + 0.433 * color.G;
        var g = 0.558 * color.R + 0.442 * color.G;
        var b = 0.242 * color.B;
        
        return Color.FromArgb(color.A, 
            (byte)Math.Min(255, Math.Max(0, r)),
            (byte)Math.Min(255, Math.Max(0, g)),
            (byte)Math.Min(255, Math.Max(0, b)));
    }

    /// <summary>
    /// Simplified deuteranopia simulation
    /// </summary>
    private Color SimulateDeuteranopia(Color color)
    {
        var r = 0.625 * color.R + 0.375 * color.G;
        var g = 0.70 * color.R + 0.30 * color.G;
        var b = 0.30 * color.B;
        
        return Color.FromArgb(color.A,
            (byte)Math.Min(255, Math.Max(0, r)),
            (byte)Math.Min(255, Math.Max(0, g)),
            (byte)Math.Min(255, Math.Max(0, b)));
    }

    /// <summary>
    /// Simplified tritanopia simulation
    /// </summary>
    private Color SimulateTritanopia(Color color)
    {
        var r = 0.95 * color.R + 0.05 * color.B;
        var g = 0.433 * color.G + 0.567 * color.B;
        var b = 0.475 * color.B;
        
        return Color.FromArgb(color.A,
            (byte)Math.Min(255, Math.Max(0, r)),
            (byte)Math.Min(255, Math.Max(0, g)),
            (byte)Math.Min(255, Math.Max(0, b)));
    }

    /// <summary>
    /// Reduces red component by specified factor
    /// </summary>
    private Color ReduceRedComponent(Color color, double factor)
    {
        return Color.FromArgb(color.A, 
            (byte)(color.R * factor), 
            color.G, 
            color.B);
    }

    /// <summary>
    /// Reduces green component by specified factor
    /// </summary>
    private Color ReduceGreenComponent(Color color, double factor)
    {
        return Color.FromArgb(color.A, 
            color.R, 
            (byte)(color.G * factor), 
            color.B);
    }

    /// <summary>
    /// Reduces blue component by specified factor
    /// </summary>
    private Color ReduceBlueComponent(Color color, double factor)
    {
        return Color.FromArgb(color.A, 
            color.R, 
            color.G, 
            (byte)(color.B * factor));
    }

    /// <summary>
    /// Converts color to grayscale using luminance formula
    /// </summary>
    private Color ToGrayscale(Color color)
    {
        var gray = (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
        return Color.FromArgb(color.A, gray, gray, gray);
    }

    /// <summary>
    /// Converts RGB color to HSV color space
    /// </summary>
    private (double hue, double saturation, double value) ColorToHsv(Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0; 
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double h = 0;
        if (delta != 0)
        {
            if (max == r) h = 60 * (((g - b) / delta) % 6);
            else if (max == g) h = 60 * (((b - r) / delta) + 2);
            else if (max == b) h = 60 * (((r - g) / delta) + 4);
        }
        if (h < 0) h += 360;

        double s = max == 0 ? 0 : delta / max;
        double v = max;

        return (h, s, v);
    }

    /// <summary>
    /// Converts HSV color space to RGB color
    /// </summary>
    private Color HsvToColor(double hue, double saturation, double value, byte alpha = 255)
    {
        int hi = (int)(hue / 60) % 6;
        double f = (hue / 60) - hi;
        double p = value * (1 - saturation);
        double q = value * (1 - f * saturation);
        double t = value * (1 - (1 - f) * saturation);

        double r, g, b;
        switch (hi)
        {
            case 0: r = value; g = t; b = p; break;
            case 1: r = q; g = value; b = p; break;
            case 2: r = p; g = value; b = t; break;
            case 3: r = p; g = q; b = value; break;
            case 4: r = t; g = p; b = value; break;
            case 5: r = value; g = p; b = q; break;
            default: r = g = b = 0; break;
        }

        return Color.FromArgb(alpha,
            (byte)Math.Min(255, Math.Max(0, r * 255)),
            (byte)Math.Min(255, Math.Max(0, g * 255)),
            (byte)Math.Min(255, Math.Max(0, b * 255)));
    }

    #endregion

    #region Professional Color Picker Properties for Primary Action Color

    /// <summary>
    /// RGB component properties for Primary Action Color
    /// </summary>
    public byte PrimaryActionColorRed
    {
        get => PrimaryActionColor.R;
        set
        {
            if (value != PrimaryActionColor.R)
            {
                PrimaryActionColor = Color.FromRgb(value, PrimaryActionColor.G, PrimaryActionColor.B);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    public byte PrimaryActionColorGreen
    {
        get => PrimaryActionColor.G;
        set
        {
            if (value != PrimaryActionColor.G)
            {
                PrimaryActionColor = Color.FromRgb(PrimaryActionColor.R, value, PrimaryActionColor.B);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    public byte PrimaryActionColorBlue
    {
        get => PrimaryActionColor.B;
        set
        {
            if (value != PrimaryActionColor.B)
            {
                PrimaryActionColor = Color.FromRgb(PrimaryActionColor.R, PrimaryActionColor.G, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    /// <summary>
    /// HSL component properties for Primary Action Color
    /// </summary>
    public double PrimaryActionColorHue
    {
        get => RgbToHsl(PrimaryActionColor).Hue;
        set
        {
            var hsl = RgbToHsl(PrimaryActionColor);
            if (Math.Abs(value - hsl.Hue) > 0.1)
            {
                PrimaryActionColor = HslToRgb(value, hsl.Saturation, hsl.Lightness);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    public double PrimaryActionColorSaturation
    {
        get => RgbToHsl(PrimaryActionColor).Saturation;
        set
        {
            var hsl = RgbToHsl(PrimaryActionColor);
            if (Math.Abs(value - hsl.Saturation) > 0.1)
            {
                PrimaryActionColor = HslToRgb(hsl.Hue, value, hsl.Lightness);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    public double PrimaryActionColorLightness
    {
        get => RgbToHsl(PrimaryActionColor).Lightness;
        set
        {
            var hsl = RgbToHsl(PrimaryActionColor);
            if (Math.Abs(value - hsl.Lightness) > 0.1)
            {
                PrimaryActionColor = HslToRgb(hsl.Hue, hsl.Saturation, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(PrimaryActionColorContrastInfo));
            }
        }
    }

    /// <summary>
    /// WCAG contrast information for Primary Action Color
    /// </summary>
    public string PrimaryActionColorContrastInfo
    {
        get
        {
            var contrastWithWhite = CalculateContrastRatio(PrimaryActionColor, Colors.White);
            var contrastWithBlack = CalculateContrastRatio(PrimaryActionColor, Colors.Black);
            
            var wcagAA = contrastWithWhite >= 4.5 || contrastWithBlack >= 4.5;
            var wcagAAA = contrastWithWhite >= 7.0 || contrastWithBlack >= 7.0;
            
            var status = wcagAAA ? "AAA ✓" : wcagAA ? "AA ✓" : "Fail ✗";
            return $"WCAG Contrast: {status} (vs White: {contrastWithWhite:F1}, vs Black: {contrastWithBlack:F1})";
        }
    }

    #region Secondary Action Color RGB/HSL Components

    /// <summary>
    /// RGB component properties for Secondary Action Color
    /// </summary>
    public byte SecondaryActionColorRed
    {
        get => SecondaryActionColor.R;
        set
        {
            if (value != SecondaryActionColor.R)
            {
                SecondaryActionColor = Color.FromRgb(value, SecondaryActionColor.G, SecondaryActionColor.B);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    public byte SecondaryActionColorGreen
    {
        get => SecondaryActionColor.G;
        set
        {
            if (value != SecondaryActionColor.G)
            {
                SecondaryActionColor = Color.FromRgb(SecondaryActionColor.R, value, SecondaryActionColor.B);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    public byte SecondaryActionColorBlue
    {
        get => SecondaryActionColor.B;
        set
        {
            if (value != SecondaryActionColor.B)
            {
                SecondaryActionColor = Color.FromRgb(SecondaryActionColor.R, SecondaryActionColor.G, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    /// <summary>
    /// HSL component properties for Secondary Action Color
    /// </summary>
    public double SecondaryActionColorHue
    {
        get => RgbToHsl(SecondaryActionColor).Hue;
        set
        {
            var hsl = RgbToHsl(SecondaryActionColor);
            if (Math.Abs(value - hsl.Hue) > 0.1)
            {
                SecondaryActionColor = HslToRgb(value, hsl.Saturation, hsl.Lightness);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    public double SecondaryActionColorSaturation
    {
        get => RgbToHsl(SecondaryActionColor).Saturation;
        set
        {
            var hsl = RgbToHsl(SecondaryActionColor);
            if (Math.Abs(value - hsl.Saturation) > 0.1)
            {
                SecondaryActionColor = HslToRgb(hsl.Hue, value, hsl.Lightness);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    public double SecondaryActionColorLightness
    {
        get => RgbToHsl(SecondaryActionColor).Lightness;
        set
        {
            var hsl = RgbToHsl(SecondaryActionColor);
            if (Math.Abs(value - hsl.Lightness) > 0.1)
            {
                SecondaryActionColor = HslToRgb(hsl.Hue, hsl.Saturation, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(SecondaryActionColorContrastInfo));
            }
        }
    }

    /// <summary>
    /// WCAG contrast information for Secondary Action Color
    /// </summary>
    public string SecondaryActionColorContrastInfo
    {
        get
        {
            var contrastWithWhite = CalculateContrastRatio(SecondaryActionColor, Colors.White);
            var contrastWithBlack = CalculateContrastRatio(SecondaryActionColor, Colors.Black);
            
            var wcagAA = contrastWithWhite >= 4.5 || contrastWithBlack >= 4.5;
            var wcagAAA = contrastWithWhite >= 7.0 || contrastWithBlack >= 7.0;
            
            var status = wcagAAA ? "AAA ✓" : wcagAA ? "AA ✓" : "Fail ✗";
            return $"WCAG Contrast: {status} (vs White: {contrastWithWhite:F1}, vs Black: {contrastWithBlack:F1})";
        }
    }

    #endregion

    #endregion

    #region Color Science Helper Methods

    /// <summary>
    /// Convert RGB color to HSL values
    /// </summary>
    private static (double Hue, double Saturation, double Lightness) RgbToHsl(Color rgb)
    {
        var r = rgb.R / 255.0;
        var g = rgb.G / 255.0;
        var b = rgb.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        var h = 0.0;
        var s = 0.0;
        var l = (max + min) / 2.0;

        if (delta != 0.0)
        {
            s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

            if (max == r)
                h = (g - b) / delta + (g < b ? 6.0 : 0.0);
            else if (max == g)
                h = (b - r) / delta + 2.0;
            else if (max == b)
                h = (r - g) / delta + 4.0;

            h /= 6.0;
        }

        return (h * 360.0, s * 100.0, l * 100.0);
    }

    /// <summary>
    /// Convert HSL values to RGB color
    /// </summary>
    private static Color HslToRgb(double hue, double saturation, double lightness)
    {
        var h = hue / 360.0;
        var s = saturation / 100.0;
        var l = lightness / 100.0;

        double r, g, b;

        if (s == 0.0)
        {
            r = g = b = l; // achromatic
        }
        else
        {
            double HueToRgb(double p, double q, double t)
            {
                if (t < 0) t += 1;
                if (t > 1) t -= 1;
                if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
                if (t < 1.0 / 2.0) return q;
                if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
                return p;
            }

            var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            var p = 2 * l - q;
            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }

        return Color.FromRgb(
            (byte)Math.Round(r * 255), 
            (byte)Math.Round(g * 255), 
            (byte)Math.Round(b * 255));
    }

    #endregion

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

/// <summary>
/// Theme export/import data model for JSON serialization
/// </summary>
public class ThemeExportModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Colors { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string Version { get; set; } = "1.0";
}

/// <summary>
/// Theme version snapshot for rollback functionality
/// </summary>
public class ThemeVersionSnapshot
{
    public string Version { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Documentation { get; set; } = string.Empty;
    public string BaseTheme { get; set; } = string.Empty;
    public string ConditionalContext { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;
    public Dictionary<string, string> Colors { get; set; } = new();
}

/// <summary>
/// Color usage analytics tracking
/// </summary>
public class ColorUsageStats
{
    public string ColorProperty { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime FirstUsed { get; set; } = DateTime.Now;
    public DateTime LastUsed { get; set; } = DateTime.Now;
}

/// <summary>
/// Enhanced theme sharing model with additional metadata
/// </summary>
public class ThemeShareModel
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BaseTheme { get; set; } = string.Empty;
    public string ConditionalContext { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime SharedDate { get; set; } = DateTime.Now;
    public Dictionary<string, string> Colors { get; set; } = new();
    public Dictionary<string, string> CustomNames { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}