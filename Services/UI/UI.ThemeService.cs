using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Core;

namespace MTM_WIP_Application_Avalonia.Services.UI;

/// <summary>
/// Theme information model for MTM application.
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
/// Base service result class for theme operations.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }

    public int Status => IsSuccess ? 1 : 0;
    public int SuccessCount => IsSuccess ? 1 : 0;

    public static ServiceResult Success(string message = "")
        => new() { IsSuccess = true, Message = message };

    public static ServiceResult Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

/// <summary>
/// Service result class for operations that return typed data.
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public T? Data { get; set; }

    public int Status => IsSuccess ? 1 : 0;
    public int SuccessCount => IsSuccess ? 1 : 0;
    public T? Value => IsSuccess ? Data : default(T);

    public static ServiceResult<T> Success(T data, string message = "")
        => new() { IsSuccess = true, Data = data, Message = message };

    public static ServiceResult<T> Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

/// <summary>
/// Theme changed event arguments.
/// </summary>
public class ThemeChangedEventArgs : EventArgs
{
    public string PreviousTheme { get; }
    public string NewTheme { get; }

    public ThemeChangedEventArgs(string previousTheme, string newTheme)
    {
        PreviousTheme = previousTheme;
        NewTheme = newTheme;
    }
}

/// <summary>
/// Theme management service interface for MTM application.
/// Uses MTMTheme static approach to eliminate UI thread issues.
/// </summary>
public interface IThemeService : INotifyPropertyChanged
{
    string CurrentTheme { get; }
    bool IsDarkTheme { get; }
    IReadOnlyList<ThemeInfo> AvailableThemes { get; }

    Task<ServiceResult> SetThemeAsync(string themeId);
    Task<ServiceResult> InitializeThemeSystemAsync();
    Task<ServiceResult<string>> GetUserPreferredThemeAsync();
    Task<ServiceResult> ToggleVariantAsync();
    Task<ServiceResult> SaveUserPreferredThemeAsync(string themeId);
    Task<ServiceResult> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides);

    event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
}

/// <summary>
/// MTM theme service implementation using static ThemeVariant approach.
/// Eliminates UI thread issues and white background problems by using Avalonia's built-in theme system.
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ILogger<ThemeService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IApplicationStateService _applicationStateService;
    private string _currentTheme = "MTM_Blue";
    private readonly List<ThemeInfo> _availableThemes;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public ThemeService(
        ILogger<ThemeService> logger,
        IConfigurationService configurationService,
        IApplicationStateService applicationStateService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _applicationStateService = applicationStateService ?? throw new ArgumentNullException(nameof(applicationStateService));

        // Initialize available themes
        _availableThemes = new List<ThemeInfo>
        {
            new() { Id = "MTM_Blue", DisplayName = "MTM Blue", Description = "Primary MTM theme with Windows 11 blue", IsDark = false, PreviewColor = "#0078D4" },
            new() { Id = "MTM_Green", DisplayName = "MTM Green", Description = "Alternative green theme", IsDark = false, PreviewColor = "#107C10" },
            new() { Id = "MTM_Red", DisplayName = "MTM Red", Description = "High visibility red theme", IsDark = false, PreviewColor = "#D13438" },
            new() { Id = "MTM_Dark", DisplayName = "MTM Dark", Description = "Dark theme for low light environments", IsDark = true, PreviewColor = "#1F1F1F" },
            new() { Id = "MTM_Purple", DisplayName = "MTM Purple", Description = "Alternative professional theme", IsDark = false, PreviewColor = "#8B5CF6" }
        };

        _logger.LogDebug("ThemeService initialized with static MTMTheme approach");
    }

    public string CurrentTheme => _currentTheme;

    public bool IsDarkTheme => _currentTheme.Contains("Dark", StringComparison.OrdinalIgnoreCase);

    public IReadOnlyList<ThemeInfo> AvailableThemes => _availableThemes.AsReadOnly();

    public async Task<ServiceResult> SetThemeAsync(string themeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(themeId))
            {
                _logger.LogWarning("Theme ID cannot be empty");
                return ServiceResult.Failure("Theme ID cannot be empty");
            }

            var previousTheme = _currentTheme;

            if (_currentTheme == themeId)
            {
                _logger.LogDebug("Theme {ThemeId} is already active", themeId);
                return ServiceResult.Success($"Theme {themeId} is already active");
            }

            _currentTheme = themeId;

            // Apply theme using MTMTheme static approach - eliminates white background issues
            if (Application.Current != null)
            {
                _logger.LogInformation("Applying theme {ThemeId} using MTMTheme static variants", themeId);
                ApplyMTMTheme(themeId);
            }
            else
            {
                _logger.LogWarning("Application.Current is null - cannot apply theme");
            }

            OnPropertyChanged(nameof(CurrentTheme));
            OnPropertyChanged(nameof(IsDarkTheme));

            var args = new ThemeChangedEventArgs(previousTheme, _currentTheme);
            ThemeChanged?.Invoke(this, args);

            _logger.LogInformation("Theme changed from {PreviousTheme} to {CurrentTheme}", previousTheme, _currentTheme);
            return ServiceResult.Success($"Theme changed to {themeId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set theme to {ThemeId}", themeId);
            return ServiceResult.Failure($"Failed to set theme: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult<string>> GetUserPreferredThemeAsync()
    {
        try
        {
            // For now, return MTM_Blue as default - can be enhanced with database lookup
            await Task.Delay(1);
            return ServiceResult<string>.Success("MTM_Blue", "Using default theme");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user preferred theme");
            return ServiceResult<string>.Success("MTM_Blue", "Using fallback theme due to error");
        }
    }

    public async Task<ServiceResult> InitializeThemeSystemAsync()
    {
        try
        {
            _logger.LogInformation("Initializing MTM theme system using static ThemeVariants");

            // Get user's preferred theme
            var preferredThemeResult = await GetUserPreferredThemeAsync();
            var preferredTheme = preferredThemeResult.Data ?? "MTM_Blue";

            // Apply the theme
            var result = await SetThemeAsync(preferredTheme);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Theme system initialized successfully with theme: {ThemeId}", _currentTheme);
                return ServiceResult.Success($"Theme system initialized with theme: {_currentTheme}");
            }
            else
            {
                _logger.LogError("Failed to initialize theme system: {Message}", result.Message);
                return ServiceResult.Failure($"Failed to initialize theme system: {result.Message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during theme system initialization");
            return ServiceResult.Failure($"Theme system initialization failed: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult> ToggleVariantAsync()
    {
        try
        {
            var currentThemeInfo = _availableThemes.FirstOrDefault(t => t.Id == _currentTheme);
            if (currentThemeInfo == null)
                return ServiceResult.Failure("Current theme not found");

            // Simple toggle between light and dark variants
            var targetTheme = currentThemeInfo.IsDark ? "MTM_Blue" : "MTM_Dark";
            var result = await SetThemeAsync(targetTheme);

            return result.IsSuccess
                ? ServiceResult.Success($"Toggled theme to {targetTheme}")
                : ServiceResult.Failure($"Failed to toggle theme to {targetTheme}: {result.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to toggle theme variant");
            return ServiceResult.Failure($"Toggle failed: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult> SaveUserPreferredThemeAsync(string themeId)
    {
        try
        {
            // This would integrate with database/settings in a full implementation
            await Task.Delay(1); // Placeholder for async operation
            _logger.LogDebug("User preference saved for theme: {ThemeId}", themeId);
            return ServiceResult.Success("Theme preference saved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save user preferred theme {ThemeId}", themeId);
            return ServiceResult.Failure($"Failed to save preference: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult> ApplyCustomColorsAsync(Dictionary<string, string> colorOverrides)
    {
        try
        {
            // This would apply custom color overrides to the current theme
            await Task.Delay(1); // Placeholder for async operation
            _logger.LogDebug("Applied {OverrideCount} color overrides", colorOverrides.Count);
            return ServiceResult.Success("Custom colors applied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply custom colors");
            return ServiceResult.Failure($"Failed to apply colors: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Apply MTM theme using static ThemeVariant approach.
    /// This eliminates UI thread issues and white background problems.
    /// </summary>
    private void ApplyMTMTheme(string themeId)
    {
        try
        {
            if (Application.Current == null)
            {
                _logger.LogError("Cannot apply theme: Application.Current is null");
                return;
            }

            _logger.LogInformation("Applying MTM theme: {ThemeId}", themeId);

            // Use MTMTheme static class to get the appropriate ThemeVariant
            var themeVariant = MTMTheme.GetThemeVariant(themeId);
            Application.Current.RequestedThemeVariant = themeVariant;

            // Force style replacement by loading theme resources through StyleInclude
            var styles = Application.Current.Styles;

            // Remove any existing MTM theme styles (except MTMComponents)
            for (int i = styles.Count - 1; i >= 0; i--)
            {
                if (styles[i] is StyleInclude styleInclude &&
                    styleInclude.Source?.ToString().Contains("MTM_") == true &&
                    styleInclude.Source?.ToString().Contains("MTMComponents") != true)
                {
                    styles.RemoveAt(i);
                }
            }

            // Add the new theme style
            var themeUri = new Uri($"avares://MTM_WIP_Application_Avalonia/Resources/Themes/{themeId}.axaml");
            var themeStyleInclude = new StyleInclude(themeUri)
            {
                Source = themeUri
            };

            styles.Add(themeStyleInclude);

            _logger.LogInformation("Theme applied successfully: {ThemeId}", themeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to apply theme: {ThemeId}", themeId);

            // Fallback to default MTM Blue theme
            if (Application.Current != null)
            {
                Application.Current.RequestedThemeVariant = MTMTheme.Blue;
                _logger.LogInformation("Applied fallback MTM Blue theme");
            }
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
