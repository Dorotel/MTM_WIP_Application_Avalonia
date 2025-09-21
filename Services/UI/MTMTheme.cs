using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services.UI;

/// <summary>
/// MTM Theme System using Avalonia's built-in ThemeVariant approach
/// Inspired by Classic.Avalonia's elegant implementation
/// </summary>
public static class MTMTheme
{
    /// <summary>
    /// MTM Blue theme (default) - Windows 11 style
    /// </summary>
    public static ThemeVariant Blue { get; } = new("MTM_Blue", ThemeVariant.Light);

    /// <summary>
    /// MTM Green theme - Manufacturing focus
    /// </summary>
    public static ThemeVariant Green { get; } = new("MTM_Green", ThemeVariant.Light);

    /// <summary>
    /// MTM Red theme - Alert/warning style
    /// </summary>
    public static ThemeVariant Red { get; } = new("MTM_Red", ThemeVariant.Light);

    /// <summary>
    /// MTM Dark theme - Dark mode support
    /// </summary>
    public static ThemeVariant Dark { get; } = new("MTM_Dark", ThemeVariant.Dark);

    /// <summary>
    /// MTM Purple theme - Alternative professional look
    /// </summary>
    public static ThemeVariant Purple { get; } = new("MTM_Purple", ThemeVariant.Light);

    /// <summary>
    /// All available MTM theme variants
    /// </summary>
    public static IReadOnlyList<ThemeVariant> AllVariants { get; } =
        [Blue, Green, Red, Dark, Purple];

    /// <summary>
    /// Map theme ID strings to ThemeVariant objects
    /// </summary>
    public static readonly Dictionary<string, ThemeVariant> ThemeMap = new()
    {
        { "MTM_Blue", Blue },
        { "MTM_Green", Green },
        { "MTM_Red", Red },
        { "MTM_Dark", Dark },
        { "MTM_Purple", Purple },
        { "MTMTheme", Blue }, // Fallback for database values
        { "Default", Blue }   // Fallback for database values
    };

    /// <summary>
    /// Apply MTM theme using Avalonia's built-in system
    /// </summary>
    /// <param name="themeId">Theme identifier</param>
    /// <param name="logger">Optional logger for diagnostics</param>
    public static void ApplyTheme(string themeId, ILogger? logger = null)
    {
        try
        {
            if (Application.Current == null)
            {
                logger?.LogError("Cannot apply theme: Application.Current is null");
                return;
            }

            // Get the theme variant, defaulting to Blue
            var themeVariant = GetThemeVariant(themeId);

            logger?.LogInformation("Applying MTM theme: {ThemeId} -> {ThemeVariant}", themeId, themeVariant);

            // Use Avalonia's built-in theme system - NO manual resource dictionary loading!
            Application.Current.RequestedThemeVariant = themeVariant;

            logger?.LogInformation("MTM theme applied successfully: {ThemeVariant}", themeVariant);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to apply MTM theme: {ThemeId}", themeId);

            // Fallback to default theme
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = Blue;
                logger?.LogInformation("Applied fallback MTM Blue theme");
            }
        }
    }

    /// <summary>
    /// Get ThemeVariant for a theme ID, with fallback to Blue
    /// </summary>
    /// <param name="themeId">Theme identifier</param>
    /// <returns>Corresponding ThemeVariant</returns>
    public static ThemeVariant GetThemeVariant(string themeId)
    {
        if (string.IsNullOrEmpty(themeId))
            return Blue;

        return ThemeMap.TryGetValue(themeId, out var variant) ? variant : Blue;
    }

    /// <summary>
    /// Get theme ID from ThemeVariant
    /// </summary>
    /// <param name="themeVariant">ThemeVariant to look up</param>
    /// <returns>Theme ID string</returns>
    public static string GetThemeId(ThemeVariant? themeVariant)
    {
        if (themeVariant == null)
            return "MTM_Blue";

        return ThemeMap.FirstOrDefault(kvp => kvp.Value == themeVariant).Key ?? "MTM_Blue";
    }

    /// <summary>
    /// Get current theme ID from application
    /// </summary>
    /// <returns>Current theme ID</returns>
    public static string GetCurrentThemeId()
    {
        if (Application.Current?.RequestedThemeVariant == null)
            return "MTM_Blue";

        return GetThemeId(Application.Current.RequestedThemeVariant);
    }
}
