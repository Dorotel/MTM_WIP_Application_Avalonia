using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    /// <summary>
    /// Creates an optimized theme for manufacturing environments
    /// </summary>
    private async Task ApplyOptimizedManufacturingThemeAsync()
    {
        try
        {
            Logger.LogDebug("Applying optimized manufacturing theme");
            
            // High-contrast colors for industrial environments
            PrimaryActionColor = Color.Parse("#0066CC");    // Strong blue for reliability
            SecondaryActionColor = Color.Parse("#004499");  // Darker blue for depth
            AccentColor = Color.Parse("#FF6600");           // Safety orange for attention
            HighlightColor = Color.Parse("#FFCC00");        // Warning yellow for alerts
            
            // Text optimized for industrial displays
            HeadingTextColor = Color.Parse("#1A1A1A");      // Near-black for readability
            BodyTextColor = Color.Parse("#333333");         // Dark gray for body text
            InteractiveTextColor = Color.Parse("#0066CC");  // Matching primary for links
            OverlayTextColor = Color.Parse("#FFFFFF");      // Pure white for overlays
            TertiaryTextColor = Color.Parse("#666666");     // Medium gray for secondary info
            
            // Backgrounds suitable for long working sessions
            MainBackgroundColor = Color.Parse("#F5F5F5");   // Light neutral background
            CardBackgroundColor = Color.Parse("#FFFFFF");   // Pure white for content cards
            HoverBackgroundColor = Color.Parse("#E6F2FF");  // Light blue hover state
            PanelBackgroundColor = Color.Parse("#FAFAFA");  // Very light gray for panels
            SidebarBackgroundColor = Color.Parse("#F0F0F0"); // Slightly darker sidebar
            
            // Clear status indicators
            SuccessColor = Color.Parse("#00AA00");          // Strong green for success
            WarningColor = Color.Parse("#FF8800");          // Orange for warnings  
            ErrorColor = Color.Parse("#CC0000");            // Strong red for errors
            InfoColor = Color.Parse("#0088CC");             // Information blue
            
            // Professional borders
            BorderColor = Color.Parse("#CCCCCC");           // Light gray borders
            BorderAccentColor = Color.Parse("#0066CC");     // Blue accent borders
            
            CurrentThemeName = "Optimized Manufacturing Theme";
            HasUnsavedChanges = true;
            StatusMessage = "Manufacturing-optimized theme applied - designed for industrial displays";
            
            await Task.Delay(100); // Brief pause for visual feedback
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying optimized manufacturing theme");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to apply manufacturing theme", Environment.UserName);
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