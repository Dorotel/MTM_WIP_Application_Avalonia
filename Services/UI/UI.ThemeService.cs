using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Services.UI;


#region Theme Service

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
    Task<ServiceResult> InitializeThemeSystemAsync();

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
/// Base service result class.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }

    // Backward compatibility properties
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
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    // Backward compatibility
    public T? Value => IsSuccess ? Data : default(T);

    public static ServiceResult<T> Success(T data, string message = "")
        => new() { IsSuccess = true, Data = data, Message = message };

    public static new ServiceResult<T> Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
}

/// <summary>
/// MTM theme service implementation.
/// Provides comprehensive theme management with support for MTM color schemes.
/// </summary>
public class ThemeService : IThemeService
{
    private readonly ILogger<ThemeService> _logger;
    private string _currentTheme = "MTM_Blue";
    private readonly List<ThemeInfo> _availableThemes;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public ThemeService(ILogger<ThemeService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _availableThemes = new List<ThemeInfo>
        {
            new() { Id = "MTM_Blue", DisplayName = "MTM Blue", Description = "Primary MTM theme with Windows 11 blue", IsDark = false, PreviewColor = "#0078D4" },
            new() { Id = "MTM_Green", DisplayName = "MTM Green", Description = "Alternative green theme", IsDark = false, PreviewColor = "#107C10" },
            new() { Id = "MTM_Red", DisplayName = "MTM Red", Description = "High visibility red theme", IsDark = false, PreviewColor = "#D13438" },
            new() { Id = "MTM_Dark", DisplayName = "MTM Dark", Description = "Dark theme for low light environments", IsDark = true, PreviewColor = "#1F1F1F" }
        };

        _logger.LogDebug("ThemeService initialized with {ThemeCount} themes", _availableThemes.Count);
    }

    public string CurrentTheme => _currentTheme;
    public IReadOnlyList<ThemeInfo> AvailableThemes => _availableThemes.AsReadOnly();
    public bool IsDarkTheme => _availableThemes.FirstOrDefault(t => t.Id == _currentTheme)?.IsDark ?? false;

    public async Task<ServiceResult> SetThemeAsync(string themeId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(themeId))
                return ServiceResult.Failure("Theme ID cannot be empty");

            if (!_availableThemes.Any(t => t.Id == themeId))
                return ServiceResult.Failure($"Theme '{themeId}' not found");

            if (_currentTheme == themeId)
                return ServiceResult.Success("Theme already active");

            var previousTheme = _currentTheme;
            _currentTheme = themeId;

            // Apply theme through Avalonia's styling system
            if (Application.Current != null)
            {
                await ApplyAvaloniaThemeAsync(themeId);
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

    public async Task<ServiceResult> ToggleVariantAsync()
    {
        var currentThemeInfo = _availableThemes.FirstOrDefault(t => t.Id == _currentTheme);
        if (currentThemeInfo == null)
            return ServiceResult.Failure("Current theme not found");

        // Simple toggle between light and dark variants
        var targetTheme = currentThemeInfo.IsDark ? "MTM_Blue" : "MTM_Dark";
        return await SetThemeAsync(targetTheme);
    }

    public async Task<ServiceResult<string>> GetUserPreferredThemeAsync()
    {
        try
        {
            // This would integrate with settings service in a full implementation
            await Task.Delay(1); // Placeholder for async operation
            return ServiceResult<string>.Success(_currentTheme, "Retrieved user preference");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user preferred theme");
            return ServiceResult<string>.Failure($"Failed to get preference: {ex.Message}", ex);
        }
    }

    public async Task<ServiceResult> SaveUserPreferredThemeAsync(string themeId)
    {
        try
        {
            // This would integrate with settings service in a full implementation
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

    public async Task<ServiceResult> InitializeThemeSystemAsync()
    {
        try
        {
            // Load user preferences and apply default theme
            var userPreferredTheme = await GetUserPreferredThemeAsync();
            if (userPreferredTheme.IsSuccess && !string.IsNullOrEmpty(userPreferredTheme.Data))
            {
                await SetThemeAsync(userPreferredTheme.Data);
            }

            _logger.LogInformation("Theme system initialized successfully");
            return ServiceResult.Success("Theme system initialized");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize theme system");
            return ServiceResult.Failure($"Initialization failed: {ex.Message}", ex);
        }
    }

    private async Task ApplyAvaloniaThemeAsync(string themeId)
    {
        // This would apply the theme through Avalonia's resource system
        await Task.Delay(1); // Placeholder
        _logger.LogDebug("Applied Avalonia theme: {ThemeId}", themeId);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
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

#endregion
