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

#region Navigation Service

/// <summary>
/// Navigation service interface for application navigation.
/// </summary>
public interface INavigationService : INotifyPropertyChanged
{
    object? CurrentView { get; }
    bool CanGoBack { get; }
    bool CanGoForward { get; }

    void NavigateTo<T>() where T : class;
    void NavigateTo(object viewModel);
    void NavigateTo(string viewKey);
    void GoBack();
    void GoForward();
    void ClearHistory();

    event EventHandler<NavigationEventArgs>? Navigated;
}

/// <summary>
/// Simple navigation service implementation.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<object> _backStack = new();
    private readonly Stack<object> _forwardStack = new();
    private object? _currentView;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<NavigationEventArgs>? Navigated;

    public NavigationService(ILogger<NavigationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("NavigationService constructed successfully");
    }

    public object? CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoBack));
                OnPropertyChanged(nameof(CanGoForward));
            }
        }
    }

    public bool CanGoBack => _backStack.Count > 0;
    public bool CanGoForward => _forwardStack.Count > 0;

    public void NavigateTo<T>() where T : class
    {
        try
        {
            var viewInstance = Activator.CreateInstance<T>();
            NavigateTo(viewInstance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to view type {ViewType}", typeof(T).Name);
        }
    }

    public void NavigateTo(object viewModel)
    {
        if (viewModel == null) return;

        try
        {
            if (CurrentView != null)
            {
                _backStack.Push(CurrentView);
                _forwardStack.Clear();
            }

            CurrentView = viewModel;

            var args = new NavigationEventArgs(viewModel, NavigationType.Forward);
            Navigated?.Invoke(this, args);

            _logger.LogDebug("Navigated to {ViewModelType}", viewModel.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to {ViewModelType}", viewModel?.GetType().Name);
        }
    }

    public void NavigateTo(string viewKey)
    {
        _logger.LogWarning("String-based navigation not implemented for key: {ViewKey}", viewKey);
    }

    public void GoBack()
    {
        if (!CanGoBack) return;

        try
        {
            if (CurrentView != null)
            {
                _forwardStack.Push(CurrentView);
            }

            CurrentView = _backStack.Pop();

            var args = new NavigationEventArgs(CurrentView!, NavigationType.Back);
            Navigated?.Invoke(this, args);

            _logger.LogDebug("Navigated back to {ViewModelType}", CurrentView?.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate back");
        }
    }

    public void GoForward()
    {
        if (!CanGoForward) return;

        try
        {
            if (CurrentView != null)
            {
                _backStack.Push(CurrentView);
            }

            CurrentView = _forwardStack.Pop();

            var args = new NavigationEventArgs(CurrentView!, NavigationType.Forward);
            Navigated?.Invoke(this, args);

            _logger.LogDebug("Navigated forward to {ViewModelType}", CurrentView?.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate forward");
        }
    }

    public void ClearHistory()
    {
        _backStack.Clear();
        _forwardStack.Clear();
        _logger.LogDebug("Navigation history cleared");
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Navigation event arguments.
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public object Target { get; }
    public NavigationType Type { get; }

    public NavigationEventArgs(object target, NavigationType type)
    {
        Target = target;
        Type = type;
    }
}

/// <summary>
/// Navigation type enumeration.
/// </summary>
public enum NavigationType
{
    Forward,
    Back
}

#endregion

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

#region Focus Management Service

/// <summary>
/// Service for managing focus across views in the MTM WIP Application.
/// Provides automatic focus management during application startup and view switching
/// while preserving SuggestionOverlay functionality.
/// </summary>
public interface IFocusManagementService
{
    /// <summary>
    /// Sets focus to the first focusable control with TabIndex=1 in the specified view.
    /// </summary>
    /// <param name="view">The view to set focus in</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 100ms)</param>
    Task SetInitialFocusAsync(Control view, int delayMs = 100);

    /// <summary>
    /// Sets focus to the first focusable control with TabIndex=1 after application startup completes.
    /// </summary>
    /// <param name="mainView">The main view containing the tab system</param>
    Task SetStartupFocusAsync(Control mainView);

    /// <summary>
    /// Sets focus when switching between tabs in the main view.
    /// </summary>
    /// <param name="mainView">The main view containing the tab system</param>
    /// <param name="tabIndex">The tab index being switched to</param>
    Task SetTabSwitchFocusAsync(Control mainView, int tabIndex);

    /// <summary>
    /// Sets focus to the next TabIndex control after SuggestionOverlay closes with a selection.
    /// </summary>
    /// <param name="currentControl">The control that had focus before the overlay</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 100ms)</param>
    Task SetNextTabIndexFocusAsync(Control currentControl, int delayMs = 100);

    /// <summary>
    /// Sets focus back to the current TabIndex control after SuggestionOverlay closes without selection.
    /// </summary>
    /// <param name="currentControl">The control to refocus</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 50ms)</param>
    Task RestoreFocusAsync(Control currentControl, int delayMs = 50);
}

/// <summary>
/// Focus management service implementation for MTM WIP Application.
/// </summary>
public class FocusManagementService : IFocusManagementService
{
    private readonly ILogger<FocusManagementService> _logger;

    public FocusManagementService(ILogger<FocusManagementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FocusManagementService constructed successfully");
    }

    public async Task SetInitialFocusAsync(Control view, int delayMs = 100)
    {
        try
        {
            await Task.Delay(delayMs);

            var focusableControl = FindFirstFocusableControlWithTabIndex(view, 1);
            if (focusableControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    focusableControl.Focus();
                    _logger.LogDebug("Set initial focus to {ControlType}", focusableControl.GetType().Name);
                });
            }
            else
            {
                _logger.LogWarning("No focusable control with TabIndex=1 found in view {ViewType}", view.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set initial focus in view {ViewType}", view.GetType().Name);
        }
    }

    public async Task SetStartupFocusAsync(Control mainView)
    {
        try
        {
            // Allow application to fully load before setting focus
            await Task.Delay(500);

            await SetInitialFocusAsync(mainView, 0);
            _logger.LogDebug("Set startup focus in main view");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set startup focus");
        }
    }

    public async Task SetTabSwitchFocusAsync(Control mainView, int tabIndex)
    {
        try
        {
            await Task.Delay(100); // Allow tab switching to complete

            var focusableControl = FindFirstFocusableControlWithTabIndex(mainView, 1);
            if (focusableControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    focusableControl.Focus();
                    _logger.LogDebug("Set tab switch focus for tab {TabIndex}", tabIndex);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set tab switch focus for tab {TabIndex}", tabIndex);
        }
    }

    public async Task SetNextTabIndexFocusAsync(Control currentControl, int delayMs = 100)
    {
        try
        {
            await Task.Delay(delayMs);

            var nextControl = FindNextTabIndexControl(currentControl);
            if (nextControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    nextControl.Focus();
                    _logger.LogDebug("Set focus to next TabIndex control");
                });
            }
            else
            {
                // Fall back to restoring focus to current control
                await RestoreFocusAsync(currentControl, 0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set next TabIndex focus");
            await RestoreFocusAsync(currentControl, 0);
        }
    }

    public async Task RestoreFocusAsync(Control currentControl, int delayMs = 50)
    {
        try
        {
            await Task.Delay(delayMs);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                currentControl.Focus();
                _logger.LogDebug("Restored focus to {ControlType}", currentControl.GetType().Name);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore focus to {ControlType}", currentControl.GetType().Name);
        }
    }

    private Control? FindFirstFocusableControlWithTabIndex(Control container, int targetTabIndex)
    {
        if (container is InputElement input && input.Focusable && input.TabIndex == targetTabIndex)
            return container;

        foreach (var child in container.GetVisualChildren().OfType<Control>())
        {
            var result = FindFirstFocusableControlWithTabIndex(child, targetTabIndex);
            if (result != null)
                return result;
        }

        return null;
    }

    private Control? FindNextTabIndexControl(Control currentControl)
    {
        var parent = currentControl.GetVisualParent<Control>();
        if (parent == null) return null;

        var currentTabIndex = (currentControl as InputElement)?.TabIndex ?? 0;
        var nextTabIndex = currentTabIndex + 1;

        return FindFirstFocusableControlWithTabIndex(parent, nextTabIndex);
    }
}

#endregion

#region Success Overlay Service

/// <summary>
/// Interface for the success overlay service
/// </summary>
public interface ISuccessOverlayService
{
    /// <summary>
    /// Shows a success overlay on the specified target control
    /// </summary>
    /// <param name="targetControl">The control to overlay</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details</param>
    /// <param name="iconKind">Material icon to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="onDismissed">Callback when overlay is dismissed</param>
    /// <param name="isError">True if this is an error overlay, false for success</param>
    Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null,
        bool isError = false);

    /// <summary>
    /// Shows success overlay using MainView integration (preferred method)
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="isError">True if this is an error overlay, false for success</param>
    Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        bool isError = false);

    /// <summary>
    /// Shows error overlay using MainView integration
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Error message to display</param>
    /// <param name="details">Optional details about the error</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    Task ShowErrorOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "AlertCircle",
        int duration = 3000);
}

/// <summary>
/// Success overlay service implementation for MTM WIP Application.
/// </summary>
public class SuccessOverlayService : ISuccessOverlayService
{
    private readonly ILogger<SuccessOverlayService> _logger;

    public SuccessOverlayService(ILogger<SuccessOverlayService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("SuccessOverlayService constructed successfully");
    }

    public async Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null,
        bool isError = false)
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var overlayViewModel = new SuccessOverlayViewModel
                {
                    Message = message,
                    Details = details,
                    IconKind = iconKind,
                    IsError = isError
                };

                var overlayView = new SuccessOverlay
                {
                    DataContext = overlayViewModel
                };

                // This would show the overlay over the target control
                // Implementation would depend on specific overlay system

                _logger.LogDebug("Showing {OverlayType} overlay: {Message}", isError ? "error" : "success", message);

                // Auto-dismiss after duration
                await Task.Delay(duration);
                onDismissed?.Invoke();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show success overlay");
        }
    }

    public async Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        bool isError = false)
    {
        try
        {
            // Find MainView from source control or application
            var mainView = FindMainView(sourceControl);
            if (mainView != null)
            {
                await ShowSuccessOverlayAsync(mainView, message, details, iconKind, duration, null, isError);
            }
            else
            {
                _logger.LogWarning("Could not find MainView for overlay display");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show success overlay in MainView");
        }
    }

    public async Task ShowErrorOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "AlertCircle",
        int duration = 3000)
    {
        await ShowSuccessOverlayInMainViewAsync(sourceControl, message, details, iconKind, duration, true);
    }

    private Control? FindMainView(Control? sourceControl)
    {
        // This would traverse the visual tree to find the MainView
        // Implementation would depend on the application structure

        if (sourceControl == null)
        {
            // Try to get from current application
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }

        // Traverse up the visual tree to find MainView
        var current = sourceControl;
        while (current != null)
        {
            if (current.Name == "MainView" || current.GetType().Name.Contains("MainView"))
            {
                return current;
            }
            current = current.GetVisualParent<Control>();
        }

        return null;
    }
}

#endregion

#region Universal Overlay Service

/// <summary>
/// Universal Overlay Service implementation providing centralized overlay management
/// with memory pooling, z-index management, and theme integration for MTM application.
/// </summary>
public class UniversalOverlayService : Services.Interfaces.IUniversalOverlayService, IDisposable
{
    private readonly ILogger<UniversalOverlayService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Type, Type> _overlayRegistrations = new();
    private readonly Dictionary<string, OverlayState> _activeOverlays = new();
    private readonly Stack<object> _viewModelPool = new();
    private int _nextZIndex = 1000;
    private bool _disposed = false;

    public UniversalOverlayService(ILogger<UniversalOverlayService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public event EventHandler<Services.Interfaces.OverlayEventArgs>? OverlayShowing;
    public event EventHandler<Services.Interfaces.OverlayEventArgs>? OverlayShown;
    public event EventHandler<Services.Interfaces.OverlayEventArgs>? OverlayHiding;
    public event EventHandler<Services.Interfaces.OverlayEventArgs>? OverlayHidden;

    public async Task<Services.Interfaces.OverlayResult<T>> ShowOverlayAsync<T>(T request) where T : class
    {
        if (_disposed) throw new ObjectDisposedException(nameof(UniversalOverlayService));

        try
        {
            var overlayId = Guid.NewGuid().ToString();
            var requestType = typeof(T);

            // Check if overlay type is registered
            if (!_overlayRegistrations.ContainsKey(requestType))
            {
                _logger.LogError("Overlay type {OverlayType} is not registered", requestType.Name);
                return Services.Interfaces.OverlayResult<T>.Failed($"Overlay type {requestType.Name} not registered", overlayId);
            }

            var viewModelType = _overlayRegistrations[requestType];
            var viewType = GetViewTypeForViewModel(viewModelType);

            if (viewType == null)
            {
                _logger.LogError("No view type found for ViewModel {ViewModelType}", viewModelType.Name);
                return Services.Interfaces.OverlayResult<T>.Failed($"No view found for {viewModelType.Name}", overlayId);
            }

            // Fire overlay showing event
            var showingArgs = new Services.Interfaces.OverlayEventArgs(overlayId, viewModelType, true);
            OverlayShowing?.Invoke(this, showingArgs);

            if (showingArgs.Cancel)
            {
                _logger.LogInformation("Overlay {OverlayId} showing was cancelled", overlayId);
                return Services.Interfaces.OverlayResult<T>.Cancelled(overlayId);
            }

            // Get or create ViewModel instance
            var viewModel = GetOrCreateViewModel(viewModelType);
            var view = CreateView(viewType, viewModel);

            if (view == null)
            {
                _logger.LogError("Failed to create view for overlay {OverlayId}", overlayId);
                return Services.Interfaces.OverlayResult<T>.Failed("Failed to create view", overlayId);
            }

            // Configure overlay
            var overlayState = new OverlayState
            {
                Id = overlayId,
                ViewModel = viewModel,
                View = view,
                ZIndex = ++_nextZIndex,
                ShowTime = DateTime.Now,
                RequestType = requestType
            };

            _activeOverlays[overlayId] = overlayState;

            // Initialize ViewModel with request data if possible
            if (viewModel is IOverlayViewModel overlayViewModel)
            {
                await overlayViewModel.InitializeAsync(request);
            }

            // Show the overlay in the UI
            await ShowOverlayInUIAsync(overlayState);

            // Fire overlay shown event
            OverlayShown?.Invoke(this, new Services.Interfaces.OverlayEventArgs(overlayId, viewModelType));

            // Wait for overlay result (this would be handled by the overlay ViewModel)
            var result = await WaitForOverlayResultAsync<T>(overlayId);

            _logger.LogInformation("Overlay {OverlayId} completed with result: {IsSuccess}", overlayId, result.IsSuccess);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing overlay for type {RequestType}", typeof(T).Name);
            return Services.Interfaces.OverlayResult<T>.Failed($"Error showing overlay: {ex.Message}");
        }
    }

    public async Task<bool> HideOverlayAsync(string overlayId)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(UniversalOverlayService));

        if (!_activeOverlays.TryGetValue(overlayId, out var overlayState))
        {
            _logger.LogWarning("Attempted to hide non-existent overlay {OverlayId}", overlayId);
            return false;
        }

        try
        {
            // Fire overlay hiding event
            var hidingArgs = new Services.Interfaces.OverlayEventArgs(overlayId, overlayState.ViewModel?.GetType() ?? typeof(object), true);
            OverlayHiding?.Invoke(this, hidingArgs);

            if (hidingArgs.Cancel)
            {
                _logger.LogInformation("Overlay {OverlayId} hiding was cancelled", overlayId);
                return false;
            }

            // Hide overlay from UI
            await HideOverlayFromUIAsync(overlayState);

            // Clean up
            if (overlayState.ViewModel is IDisposable disposableViewModel)
            {
                disposableViewModel.Dispose();
            }

            _activeOverlays.Remove(overlayId);

            // Fire overlay hidden event
            OverlayHidden?.Invoke(this, new Services.Interfaces.OverlayEventArgs(overlayId, overlayState.ViewModel?.GetType() ?? typeof(object)));

            _logger.LogInformation("Overlay {OverlayId} hidden successfully", overlayId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hiding overlay {OverlayId}", overlayId);
            return false;
        }
    }

    public void RegisterOverlay<TViewModel, TView>() where TViewModel : class where TView : class
    {
        if (_disposed) throw new ObjectDisposedException(nameof(UniversalOverlayService));

        var viewModelType = typeof(TViewModel);
        var viewType = typeof(TView);

        _overlayRegistrations[viewModelType] = viewModelType;
        _logger.LogInformation("Registered overlay: {ViewModel} -> {View}", viewModelType.Name, viewType.Name);
    }

    public IReadOnlyList<string> GetActiveOverlayIds()
    {
        return _activeOverlays.Keys.ToList();
    }

    #region Private Helper Methods

    private object? GetOrCreateViewModel(Type viewModelType)
    {
        try
        {
            // Try to get from pool first (simple pooling strategy)
            if (_viewModelPool.Count > 0 && _viewModelPool.Peek().GetType() == viewModelType)
            {
                return _viewModelPool.Pop();
            }

            // Create new instance using DI container
            return _serviceProvider.GetService(viewModelType) ?? Activator.CreateInstance(viewModelType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ViewModel of type {ViewModelType}", viewModelType.Name);
            return null;
        }
    }

    private Control? CreateView(Type viewType, object? viewModel)
    {
        try
        {
            var view = _serviceProvider.GetService(viewType) as Control ?? Activator.CreateInstance(viewType) as Control;

            if (view != null && viewModel != null)
            {
                view.DataContext = viewModel;
            }

            return view;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating view of type {ViewType}", viewType.Name);
            return null;
        }
    }

    private Type? GetViewTypeForViewModel(Type viewModelType)
    {
        // Simple convention: ViewModel -> View (remove "ViewModel" suffix, add "View")
        var viewModelName = viewModelType.Name;
        if (viewModelName.EndsWith("ViewModel"))
        {
            var viewName = viewModelName.Replace("ViewModel", "View");
            var viewTypeName = viewModelType.Namespace?.Replace("ViewModels", "Views") + "." + viewName;

            return viewModelType.Assembly.GetType(viewTypeName);
        }

        return null;
    }

    private async Task ShowOverlayInUIAsync(OverlayState overlayState)
    {
        await Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // This would integrate with the main window overlay system
                // For now, we'll simulate the overlay being shown
                _logger.LogInformation("Showing overlay {OverlayId} in UI with Z-Index {ZIndex}",
                    overlayState.Id, overlayState.ZIndex);
            });
        });
    }

    private async Task HideOverlayFromUIAsync(OverlayState overlayState)
    {
        await Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // This would integrate with the main window overlay system
                // For now, we'll simulate the overlay being hidden
                _logger.LogInformation("Hiding overlay {OverlayId} from UI", overlayState.Id);
            });
        });
    }

    private async Task<Services.Interfaces.OverlayResult<T>> WaitForOverlayResultAsync<T>(string overlayId) where T : class
    {
        // This would typically wait for the overlay ViewModel to signal completion
        // For now, we'll return a simple success result
        await Task.Delay(100); // Simulate async operation

        return Services.Interfaces.OverlayResult<T>.Success(default(T)!, overlayId);
    }

    #endregion

    #region IDisposable Implementation

    public void Dispose()
    {
        if (!_disposed)
        {
            // Hide all active overlays
            foreach (var overlayId in _activeOverlays.Keys.ToList())
            {
                try
                {
                    HideOverlayAsync(overlayId).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error hiding overlay {OverlayId} during disposal", overlayId);
                }
            }

            // Clear pools and registrations
            _viewModelPool.Clear();
            _overlayRegistrations.Clear();
            _activeOverlays.Clear();

            _disposed = true;
        }
    }

    #endregion

    #region Helper Classes

    private class OverlayState
    {
        public string Id { get; set; } = string.Empty;
        public object? ViewModel { get; set; }
        public Control? View { get; set; }
        public int ZIndex { get; set; }
        public DateTime ShowTime { get; set; }
        public Type? RequestType { get; set; }
    }

    #endregion
}

/// <summary>
/// Interface for overlay ViewModels that need initialization with request data
/// </summary>
public interface IOverlayViewModel
{
    Task InitializeAsync(object requestData);
}

#endregion

#region Column Configuration Service

/// <summary>
/// Service interface for column configuration persistence.
/// Phase 4 implementation for complete settings persistence across application sessions.
/// </summary>
public interface IColumnConfigurationService
{
    /// <summary>
    /// Saves a column configuration to persistent storage.
    /// </summary>
    Task<bool> SaveConfigurationAsync(string gridId, ColumnConfiguration configuration);

    /// <summary>
    /// Loads a column configuration from persistent storage.
    /// </summary>
    Task<ColumnConfiguration?> LoadConfigurationAsync(string gridId, string configurationId);

    /// <summary>
    /// Deletes a saved column configuration.
    /// </summary>
    Task<bool> DeleteConfigurationAsync(string gridId, string configurationId);

    /// <summary>
    /// Gets all saved column configurations for a specific grid.
    /// </summary>
    Task<List<ColumnConfiguration>> GetAllConfigurationsAsync(string gridId);

    /// <summary>
    /// Saves the current session configuration (auto-save on changes).
    /// </summary>
    Task<bool> SaveSessionConfigurationAsync(string gridId, ColumnConfiguration configuration);

    /// <summary>
    /// Restores the session configuration for the grid (auto-restore on startup).
    /// </summary>
    Task<ColumnConfiguration?> RestoreSessionConfigurationAsync(string gridId);

    /// <summary>
    /// Clears all session configurations.
    /// </summary>
    Task ClearSessionConfigurationsAsync();
}

/// <summary>
/// MTM Column Configuration Service - Phase 4 Implementation
///
/// Provides comprehensive column configuration persistence for CustomDataGrid controls.
/// Supports both user-saved configurations and automatic session management.
/// Follows established MTM service patterns with proper error handling and logging.
///
/// Phase 4 Features:
/// - Complete settings persistence to file system
/// - Automatic session save/restore for seamless user experience
/// - Configuration versioning and migration support
/// - Validation and error recovery for corrupted configurations
/// - Performance optimization with caching for frequently accessed configs
/// </summary>
public class ColumnConfigurationService : IColumnConfigurationService
{
    private readonly ILogger<ColumnConfigurationService> _logger;
    private readonly string _configurationsPath;
    private readonly string _sessionConfigurationsPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, ColumnConfiguration> _configurationCache;

    /// <summary>
    /// Initializes a new instance of the ColumnConfigurationService.
    /// Sets up file paths and JSON serialization options.
    /// </summary>
    public ColumnConfigurationService(ILogger<ColumnConfigurationService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        // Set up storage paths
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MTM_WIP_Application",
            "ColumnConfigurations"
        );

        _configurationsPath = Path.Combine(appDataPath, "Saved");
        _sessionConfigurationsPath = Path.Combine(appDataPath, "Sessions");

        // Ensure directories exist
        Directory.CreateDirectory(_configurationsPath);
        Directory.CreateDirectory(_sessionConfigurationsPath);

        // Configure JSON serialization
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        _configurationCache = new Dictionary<string, ColumnConfiguration>();

        _logger.LogDebug("ColumnConfigurationService initialized. Storage path: {Path}", appDataPath);
    }

    #region Saved Configurations

    /// <summary>
    /// Saves a column configuration to persistent storage.
    /// Creates a uniquely named file for the configuration with metadata.
    /// </summary>
    public async Task<bool> SaveConfigurationAsync(string gridId, ColumnConfiguration configuration)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentNullException.ThrowIfNull(configuration);

            if (!configuration.IsValid())
            {
                _logger.LogWarning("Cannot save invalid column configuration for grid: {GridId}", gridId);
                return false;
            }

            configuration.UpdateLastModified();

            var fileName = GetConfigurationFileName(gridId, configuration.ConfigurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            var json = JsonSerializer.Serialize(configuration, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);

            // Update cache
            var cacheKey = $"{gridId}:{configuration.ConfigurationId}";
            _configurationCache[cacheKey] = configuration;

            _logger.LogInformation("Saved column configuration for grid {GridId}: {ConfigName} to {FilePath}",
                gridId, configuration.DisplayName, filePath);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving column configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Loads a column configuration from persistent storage.
    /// </summary>
    public async Task<ColumnConfiguration?> LoadConfigurationAsync(string gridId, string configurationId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentException.ThrowIfNullOrWhiteSpace(configurationId);

            // Check cache first
            var cacheKey = $"{gridId}:{configurationId}";
            if (_configurationCache.TryGetValue(cacheKey, out var cachedConfig))
            {
                _logger.LogDebug("Loaded column configuration from cache: {GridId}:{ConfigId}", gridId, configurationId);
                return cachedConfig;
            }

            var fileName = GetConfigurationFileName(gridId, configurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogDebug("Column configuration file not found: {FilePath}", filePath);
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

            if (configuration != null && configuration.IsValid())
            {
                // Update cache
                _configurationCache[cacheKey] = configuration;

                _logger.LogDebug("Loaded column configuration for grid {GridId}: {ConfigName}",
                    gridId, configuration.DisplayName);

                return configuration;
            }
            else
            {
                _logger.LogWarning("Invalid column configuration loaded from: {FilePath}", filePath);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading column configuration for grid: {GridId}", gridId);
            return null;
        }
    }

    /// <summary>
    /// Deletes a saved column configuration from persistent storage.
    /// </summary>
    public async Task<bool> DeleteConfigurationAsync(string gridId, string configurationId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentException.ThrowIfNullOrWhiteSpace(configurationId);

            var fileName = GetConfigurationFileName(gridId, configurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            await Task.Run(() =>
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    // Remove from cache
                    var cacheKey = $"{gridId}:{configurationId}";
                    _configurationCache.Remove(cacheKey);

                    _logger.LogInformation("Deleted column configuration: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogWarning("Column configuration file not found for deletion: {FilePath}", filePath);
                }
            });

            return File.Exists(filePath) == false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting column configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Gets all saved column configurations for a specific grid.
    /// </summary>
    public async Task<List<ColumnConfiguration>> GetAllConfigurationsAsync(string gridId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);

            var configurations = new List<ColumnConfiguration>();
            var pattern = $"{gridId}_*.json";
            var files = Directory.GetFiles(_configurationsPath, pattern);

            foreach (var filePath in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

                    if (configuration != null && configuration.IsValid())
                    {
                        configurations.Add(configuration);
                    }
                    else
                    {
                        _logger.LogWarning("Skipping invalid configuration file: {FilePath}", filePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error reading configuration file: {FilePath}", filePath);
                }
            }

            configurations.Sort((a, b) =>
                string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            _logger.LogDebug("Retrieved {Count} configurations for grid: {GridId}", configurations.Count, gridId);
            return configurations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configurations for grid: {GridId}", gridId);
            return new List<ColumnConfiguration>();
        }
    }

    #endregion

    #region Session Management

    /// <summary>
    /// Saves the current session configuration for automatic restore.
    /// This is called automatically when columns are modified.
    /// </summary>
    public async Task<bool> SaveSessionConfigurationAsync(string gridId, ColumnConfiguration configuration)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentNullException.ThrowIfNull(configuration);

            var fileName = GetSessionFileName(gridId);
            var filePath = Path.Combine(_sessionConfigurationsPath, fileName);

            // Create a session-specific configuration
            var sessionConfig = new ColumnConfiguration(configuration.ConfigurationId, "Session Layout")
            {
                Description = "Auto-saved session configuration",
                ColumnSettings = configuration.ColumnSettings.Select(s => s.Clone()).ToList(),
                IsDefault = false,
                CreatedBy = Environment.UserName
            };

            var json = JsonSerializer.Serialize(sessionConfig, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);

            _logger.LogDebug("Saved session configuration for grid: {GridId}", gridId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving session configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Restores the session configuration for automatic startup restore.
    /// </summary>
    public async Task<ColumnConfiguration?> RestoreSessionConfigurationAsync(string gridId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);

            var fileName = GetSessionFileName(gridId);
            var filePath = Path.Combine(_sessionConfigurationsPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogDebug("No session configuration found for grid: {GridId}", gridId);
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

            if (configuration != null && configuration.IsValid())
            {
                _logger.LogDebug("Restored session configuration for grid: {GridId}", gridId);
                return configuration;
            }
            else
            {
                _logger.LogWarning("Invalid session configuration for grid: {GridId}", gridId);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring session configuration for grid: {GridId}", gridId);
            return null;
        }
    }

    /// <summary>
    /// Clears all session configurations (called on clean application shutdown).
    /// </summary>
    public async Task ClearSessionConfigurationsAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(_sessionConfigurationsPath, "*.json");

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                _logger.LogDebug("Cleared {Count} session configurations", files.Length);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing session configurations");
        }
    }

    #endregion

    #region Private Methods

    private static string GetConfigurationFileName(string gridId, string configurationId)
    {
        var safeGridId = GetSafeFileName(gridId);
        var safeConfigId = GetSafeFileName(configurationId);
        return $"{safeGridId}_{safeConfigId}.json";
    }

    private static string GetSessionFileName(string gridId)
    {
        var safeGridId = GetSafeFileName(gridId);
        return $"session_{safeGridId}.json";
    }

    private static string GetSafeFileName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "default";

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(input.Where(c => !invalidChars.Contains(c)).ToArray());

        return string.IsNullOrWhiteSpace(safeName) ? "default" : safeName;
    }

    #endregion
}

#endregion

#region Suggestion Overlay Service

/// <summary>
/// Interface for the suggestion overlay service that provides autocomplete functionality.
/// </summary>
public interface ISuggestionOverlayService
{
    /// <summary>
    /// Shows a suggestion overlay with filtered options based on user input.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="suggestions">The list of available suggestions</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    Task<string?> ShowSuggestionsAsync(Control targetControl, IEnumerable<string> suggestions, string userInput);
}

/// <summary>
/// Service that provides suggestion overlay functionality for autocomplete scenarios.
/// Implements a lightweight popup overlay system for suggesting values to users.
/// Uses standard .NET patterns without ReactiveUI dependencies.
/// Enhanced with focus management for better user experience.
/// </summary>
public class SuggestionOverlayService : ISuggestionOverlayService
{
    private readonly ILogger<SuggestionOverlayService> _logger;
    private readonly IFocusManagementService? _focusManagementService;

    /// <summary>
    /// Initializes a new instance of the SuggestionOverlayService.
    /// </summary>
    /// <param name="logger">Logger for debugging and diagnostics</param>
    /// <param name="focusManagementService">Optional focus management service for enhanced UX</param>
    public SuggestionOverlayService(
        ILogger<SuggestionOverlayService> logger,
        IFocusManagementService? focusManagementService = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _focusManagementService = focusManagementService;
        _logger.LogDebug("SuggestionOverlayService created successfully with focus management: {HasFocusManagement}",
            _focusManagementService != null);
    }

    /// <summary>
    /// Shows a suggestion overlay with filtered options based on user input.
    /// Includes focus management: moves to next tab index on selection, stays on current tab index on cancellation.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="suggestions">The list of available suggestions</param>
    /// <param name="userInput">The current user input to filter suggestions</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    public async Task<string?> ShowSuggestionsAsync(Control targetControl, IEnumerable<string> suggestions, string userInput)
    {
        try
        {
            // CRITICAL: Check if a tab switch is in progress - if so, don't show overlay
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch in progress - skipping suggestion overlay for input: '{UserInput}'", userInput);
                return null;
            }

            if (targetControl == null)
            {
                _logger.LogWarning("Target control is null, cannot show suggestions");
                return null;
            }

            var suggestionList = suggestions?.ToList() ?? new List<string>();

            if (!suggestionList.Any())
            {
                _logger.LogDebug("No suggestions provided, returning null");
                return null;
            }

            // Double-check tab switch flag before filtering (extra safety)
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch detected during suggestion processing - aborting overlay");
                return null;
            }

            // Filter suggestions based on user input
            var filteredSuggestions = FilterSuggestions(suggestionList, userInput);

            if (!filteredSuggestions.Any())
            {
                _logger.LogDebug("No matching suggestions found for input: '{UserInput}'", userInput);
                return null;
            }

            // Final check before showing overlay
            if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
            {
                _logger.LogDebug("Tab switch detected before showing overlay - aborting for input: '{UserInput}'", userInput);
                return null;
            }

            _logger.LogDebug("Showing {Count} suggestions for input: '{UserInput}'", filteredSuggestions.Count, userInput);

            // Create and show the actual overlay
            return await ShowOverlayAsync(targetControl, filteredSuggestions, userInput);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing suggestions overlay");
            return null;
        }
    }

    /// <summary>
    /// Shows the actual suggestion overlay in the MainView panel and waits for user interaction.
    /// </summary>
    /// <param name="targetControl">The control to find the MainView from</param>
    /// <param name="filteredSuggestions">The filtered list of suggestions to display</param>
    /// <param name="userInput">The original user input</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    private async Task<string?> ShowOverlayAsync(Control targetControl, List<string> filteredSuggestions, string userInput)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                // Final safety check - if tab switch is in progress, don't show overlay
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected in ShowOverlayAsync - aborting overlay creation");
                    return null;
                }

                _logger.LogDebug("Creating suggestion overlay with {Count} suggestions", filteredSuggestions.Count);

                // Find the MainView instance in the visual tree
                var mainView = MTM_WIP_Application_Avalonia.Views.MainView.FindMainView(targetControl);
                if (mainView == null)
                {
                    _logger.LogWarning("Could not find MainView instance, falling back to popup");
                    return await ShowPopupOverlayAsync(targetControl, filteredSuggestions, userInput);
                }

                // One more check before creating ViewModel (tab switch could have started)
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected before ViewModel creation - aborting overlay");
                    return null;
                }

                // Create the ViewModel with suggestions and logger
                var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuggestionOverlayViewModel>();
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions, vmLogger);

                // Debug: Log the suggestions that were passed to the ViewModel
                _logger.LogDebug("ViewModel created with {Count} suggestions: {Suggestions}",
                    viewModel.Suggestions.Count,
                    string.Join(", ", viewModel.Suggestions.Take(3)) + (viewModel.Suggestions.Count > 3 ? "..." : ""));

                // Create the overlay view
                var overlayView = new SuggestionOverlayView
                {
                    DataContext = viewModel,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                };

                // Set up completion source to wait for user interaction
                var completionSource = new TaskCompletionSource<string?>();

                // Handle suggestion selection
                viewModel.SuggestionSelected += (selectedSuggestion) =>
                {
                    _logger.LogDebug("User selected suggestion: '{Suggestion}'", selectedSuggestion);
                    mainView.HideSuggestionOverlay();

                    // Handle focus management - move to next tab index after selection
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetNextTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus moved to next tab index after suggestion selection");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to next tab index after suggestion selection");
                            }
                        });
                    }

                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    mainView.HideSuggestionOverlay();

                    // Handle focus management - stay on current tab index after cancellation
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.RestoreFocusAsync(targetControl);
                                _logger.LogDebug("Focus maintained on current tab index after overlay cancellation");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to current tab index after overlay cancellation");
                            }
                        });
                    }

                    completionSource.TrySetResult(null);
                };

                // Show the overlay in the MainView panel
                _logger.LogDebug("Showing suggestion overlay in MainView panel");
                mainView.ShowSuggestionOverlay(overlayView);

                // Wait for user interaction
                var result = await completionSource.Task;
                _logger.LogDebug("Suggestion overlay completed with result: '{Result}'", result ?? "null");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowOverlayAsync");
                return null;
            }
        });
    }

    /// <summary>
    /// Fallback method that shows a popup overlay when MainView is not accessible.
    /// </summary>
    /// <param name="targetControl">The control to position the overlay relative to</param>
    /// <param name="filteredSuggestions">The filtered list of suggestions to display</param>
    /// <param name="userInput">The original user input</param>
    /// <returns>The selected suggestion or null if cancelled</returns>
    private async Task<string?> ShowPopupOverlayAsync(Control targetControl, List<string> filteredSuggestions, string userInput)
    {
        return await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                // Check if tab switch is in progress before showing popup
                if (MTM_WIP_Application_Avalonia.Views.MainView.IsTabSwitchInProgress)
                {
                    _logger.LogDebug("Tab switch detected in ShowPopupOverlayAsync - aborting popup overlay");
                    return null;
                }

                _logger.LogDebug("Using fallback popup overlay");

                // Create the ViewModel with suggestions and logger
                var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuggestionOverlayViewModel>();
                var viewModel = new SuggestionOverlayViewModel(filteredSuggestions, vmLogger);

                // Create the overlay view
                var overlayView = new SuggestionOverlayView
                {
                    DataContext = viewModel,
                    Width = 600,
                    Height = 500,
                    MinWidth = 500,
                    MinHeight = 400
                };

                // Find the main window or top-level control for better positioning
                var window = TopLevel.GetTopLevel(targetControl);
                var placementTarget = window ?? targetControl;

                // Create a popup to host the overlay
                var popup = new Popup
                {
                    Child = overlayView,
                    PlacementTarget = placementTarget,
                    Placement = PlacementMode.Center,
                    IsLightDismissEnabled = true,
                    Width = 600,
                    Height = 500,
                    HorizontalOffset = 0,
                    VerticalOffset = 0
                };

                // Set up completion source to wait for user interaction
                var completionSource = new TaskCompletionSource<string?>();

                // Handle suggestion selection
                viewModel.SuggestionSelected += (selectedSuggestion) =>
                {
                    _logger.LogDebug("User selected suggestion: '{Suggestion}'", selectedSuggestion);
                    popup.IsOpen = false;

                    // Handle focus management - move to next tab index after selection
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.SetNextTabIndexFocusAsync(targetControl);
                                _logger.LogDebug("Focus moved to next tab index after suggestion selection (popup)");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to next tab index after suggestion selection (popup)");
                            }
                        });
                    }

                    completionSource.TrySetResult(selectedSuggestion);
                };

                // Handle cancellation
                viewModel.Cancelled += () =>
                {
                    _logger.LogDebug("User cancelled suggestion overlay");
                    popup.IsOpen = false;

                    // Handle focus management - stay on current tab index after cancellation
                    if (_focusManagementService != null)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await _focusManagementService.RestoreFocusAsync(targetControl);
                                _logger.LogDebug("Focus maintained on current tab index after overlay cancellation (popup)");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error setting focus to current tab index after overlay cancellation (popup)");
                            }
                        });
                    }

                    completionSource.TrySetResult(null);
                };

                // Handle popup closed (light dismiss)
                popup.Closed += (sender, e) =>
                {
                    _logger.LogDebug("Suggestion overlay popup closed");
                    if (!completionSource.Task.IsCompleted)
                    {
                        // Handle focus management for light dismiss - stay on current tab index
                        if (_focusManagementService != null)
                        {
                            Task.Run(async () =>
                            {
                                try
                                {
                                    await _focusManagementService.RestoreFocusAsync(targetControl);
                                    _logger.LogDebug("Focus maintained on current tab index after light dismiss (popup)");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error setting focus to current tab index after light dismiss (popup)");
                                }
                            });
                        }

                        completionSource.TrySetResult(null);
                    }
                };

                // Show the popup
                _logger.LogDebug("Opening suggestion overlay popup");
                popup.IsOpen = true;

                // Wait for user interaction
                var result = await completionSource.Task;
                _logger.LogDebug("Suggestion overlay completed with result: '{Result}'", result ?? "null");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ShowPopupOverlayAsync");
                return null;
            }
        });
    }

    /// <summary>
    /// Filters suggestions based on user input using case-insensitive substring matching.
    /// Supports wildcard matching using % symbol (e.g., "R-%-0%" matches "R-ABC-01", "R-XYZ-02").
    /// </summary>
    /// <param name="suggestions">All available suggestions</param>
    /// <param name="userInput">The user's current input (may contain % wildcards)</param>
    /// <returns>Filtered list of suggestions that match the input</returns>
    private List<string> FilterSuggestions(List<string> suggestions, string userInput)
    {
        if (string.IsNullOrEmpty(userInput))
        {
            return suggestions.Take(100).ToList(); // Allow up to 100 suggestions when no input
        }

        // Check if the input contains wildcards (% symbols)
        bool hasWildcards = userInput.Contains('%');

        List<string> filtered;

        if (hasWildcards)
        {
            // Convert wildcard pattern to regex for matching
            var regexPattern = ConvertWildcardToRegex(userInput);
            _logger.LogDebug("Wildcard pattern '{Input}' converted to regex: '{Regex}'", userInput, regexPattern);

            try
            {
                var regex = new System.Text.RegularExpressions.Regex(regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                filtered = suggestions
                    .Where(s => !string.IsNullOrEmpty(s) && regex.IsMatch(s))
                    .OrderBy(s => s.Length) // Prefer shorter matches
                    .ThenBy(s => s) // Alphabetical order as tie-breaker
                    .Take(50) // Allow up to 50 filtered suggestions with scrolling support
                    .ToList();

                _logger.LogDebug("Wildcard search '{Input}' found {Count} matches", userInput, filtered.Count);
                if (filtered.Count <= 5) // Log first few matches for debugging
                {
                    _logger.LogDebug("First matches: {Matches}", string.Join(", ", filtered));
                }
            }
            catch (ArgumentException ex)
            {
                // If regex fails, fall back to simple contains matching
                _logger.LogWarning("Invalid wildcard pattern '{Pattern}': {Error}. Using simple matching.", userInput, ex.Message);

                filtered = suggestions
                    .Where(s => !string.IsNullOrEmpty(s) &&
                               s.Contains(userInput.Replace("%", ""), StringComparison.OrdinalIgnoreCase))
                    .Take(50)
                    .ToList();
            }
        }
        else
        {
            // Standard substring matching (no wildcards)
            filtered = suggestions
                .Where(s => !string.IsNullOrEmpty(s) &&
                           s.Contains(userInput, StringComparison.OrdinalIgnoreCase))
                .OrderBy(s => s.StartsWith(userInput, StringComparison.OrdinalIgnoreCase) ? 0 : 1) // Prefer starts-with matches
                .ThenBy(s => s.Length) // Prefer shorter matches
                .ThenBy(s => s) // Alphabetical order as final tie-breaker
                .Take(50) // Allow up to 50 filtered suggestions with scrolling support
                .ToList();
        }

        return filtered;
    }

    /// <summary>
    /// Converts a wildcard pattern (using % symbols) to a regex pattern.
    /// Examples: "R-%-0%" becomes "^R\-.*\-0.*$"
    /// </summary>
    /// <param name="wildcardPattern">Pattern with % wildcards</param>
    /// <returns>Equivalent regex pattern</returns>
    private string ConvertWildcardToRegex(string wildcardPattern)
    {
        if (string.IsNullOrEmpty(wildcardPattern))
            return string.Empty;

        _logger.LogDebug("Converting wildcard pattern: '{Pattern}'", wildcardPattern);

        // First replace % with a placeholder that won't be escaped
        var withPlaceholder = wildcardPattern.Replace("%", "<!WILDCARD!>");
        _logger.LogDebug("With placeholder: '{Pattern}'", withPlaceholder);

        // Escape special regex characters (% is now safe as placeholder)
        var escaped = System.Text.RegularExpressions.Regex.Escape(withPlaceholder);
        _logger.LogDebug("After escaping: '{Escaped}'", escaped);

        // Replace placeholder with .* (match any characters)
        var regexPattern = escaped.Replace("<!WILDCARD!>", ".*");
        _logger.LogDebug("After replacing wildcards: '{Pattern}'", regexPattern);

        // Anchor the pattern to match the entire string
        regexPattern = "^" + regexPattern + "$";
        _logger.LogDebug("Final regex pattern: '{FinalPattern}'", regexPattern);

        return regexPattern;
    }
}

/// <summary>
/// Simple data model for suggestion items.
/// </summary>
public class SuggestionItem
{
    public string Value { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public bool IsExactMatch { get; set; }

    public SuggestionItem(string value, string? displayText = null)
    {
        Value = value ?? string.Empty;
        DisplayText = displayText ?? value ?? string.Empty;
    }
}

#endregion

#region Virtual Panel Manager Service

/// <summary>
/// Interface for virtual panel management with adaptive performance-based panel creation.
/// </summary>
public interface IVirtualPanelManager
{
    /// <summary>
    /// Creates a virtual panel for the specified category with adaptive performance.
    /// </summary>
    Task<SettingsPanelViewModel?> CreateVirtualPanelAsync(SettingsCategoryViewModel category);

    /// <summary>
    /// Determines if a panel should be disposed based on performance and usage.
    /// </summary>
    bool ShouldDisposePanel(SettingsPanelViewModel panel);

    /// <summary>
    /// Optimizes panel collection based on current performance level.
    /// </summary>
    Task OptimizePanelCollectionAsync(IList<SettingsPanelViewModel> panels);
}

/// <summary>
/// Virtual panel manager for adaptive performance-based panel creation.
/// Manages dynamic loading and disposal of settings panels based on system performance.
/// Enhanced with comprehensive error handling and performance monitoring.
/// </summary>
public class VirtualPanelManager : IVirtualPanelManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IThemeService _themeService;
    private readonly ILogger<VirtualPanelManager> _logger;
    private readonly Dictionary<string, Type> _panelViewTypes;

    public VirtualPanelManager(
        IServiceProvider serviceProvider,
        IThemeService themeService,
        ILogger<VirtualPanelManager> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _panelViewTypes = new Dictionary<string, Type>();
        RegisterPanelViewTypes();

        _logger.LogDebug("VirtualPanelManager initialized with {PanelTypeCount} registered panel types", _panelViewTypes.Count);
    }

    #region Public Methods

    /// <summary>
    /// Creates a virtual panel for the specified category with adaptive performance.
    /// </summary>
    public async Task<SettingsPanelViewModel?> CreateVirtualPanelAsync(SettingsCategoryViewModel category)
    {
        if (category?.PanelType == null)
        {
            _logger.LogWarning("Cannot create panel for category {CategoryId} - no panel type specified", category?.Id);
            return null;
        }

        try
        {
            _logger.LogDebug("Creating virtual panel for category {CategoryId}", category.Id);

            // Determine performance level and create appropriate panel
            var performanceLevel = GetCurrentPerformanceLevel();

            var viewModel = await CreateViewModelAsync(category.PanelType, performanceLevel);
            var view = await CreateViewAsync(category.Id, viewModel, performanceLevel);

            var panel = new SettingsPanelViewModel(
                category.Id,
                category.DisplayName,
                view,
                viewModel);

            _logger.LogInformation("Virtual panel created for category {CategoryId} with {PerformanceLevel} performance level",
                category.Id, performanceLevel);

            return panel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating virtual panel for category {CategoryId}", category.Id);
            return null;
        }
    }

    /// <summary>
    /// Determines if a panel should be disposed based on performance and usage.
    /// </summary>
    public bool ShouldDisposePanel(SettingsPanelViewModel panel)
    {
        var performanceLevel = GetCurrentPerformanceLevel();

        var threshold = performanceLevel switch
        {
            PerformanceLevel.High => TimeSpan.FromMinutes(30),
            PerformanceLevel.Medium => TimeSpan.FromMinutes(15),
            PerformanceLevel.Low => TimeSpan.FromMinutes(5),
            _ => TimeSpan.FromMinutes(15)
        };

        return panel.IsEligibleForDisposal(threshold);
    }

    /// <summary>
    /// Optimizes panel collection based on current performance level.
    /// </summary>
    public Task OptimizePanelCollectionAsync(IList<SettingsPanelViewModel> panels)
    {
        var performanceLevel = GetCurrentPerformanceLevel();
        var maxPanels = GetMaxPanelsForPerformanceLevel(performanceLevel);

        if (panels.Count <= maxPanels) return Task.CompletedTask;

        // Find panels eligible for disposal
        var eligiblePanels = new List<SettingsPanelViewModel>();

        foreach (var panel in panels)
        {
            if (ShouldDisposePanel(panel))
            {
                eligiblePanels.Add(panel);
            }
        }

        // Sort by last accessed (oldest first)
        eligiblePanels.Sort((a, b) => a.LastAccessed.CompareTo(b.LastAccessed));

        // Dispose excess panels
        var panelsToDispose = Math.Min(eligiblePanels.Count, panels.Count - maxPanels);

        for (int i = 0; i < panelsToDispose; i++)
        {
            var panel = eligiblePanels[i];
            panels.Remove(panel);
            panel.Dispose();

            _logger.LogDebug("Disposed virtual panel {CategoryId} for performance optimization", panel.CategoryId);
        }

        if (panelsToDispose > 0)
        {
            _logger.LogInformation("Optimized panel collection: disposed {Count} panels for {PerformanceLevel} performance",
                panelsToDispose, performanceLevel);
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Registers view types for panel categories.
    /// </summary>
    private void RegisterPanelViewTypes()
    {
        // Database Settings
        _panelViewTypes["database"] = typeof(Views.SettingsForm.DatabaseSettingsView);

        // User Management
        _panelViewTypes["add-user"] = typeof(Views.SettingsForm.AddUserView);
        _panelViewTypes["edit-user"] = typeof(Views.SettingsForm.EditUserView);
        _panelViewTypes["delete-user"] = typeof(Views.SettingsForm.RemoveUserView);

        // Part Numbers
        _panelViewTypes["add-part"] = typeof(Views.SettingsForm.AddPartView);
        _panelViewTypes["edit-part"] = typeof(Views.SettingsForm.EditPartView);
        _panelViewTypes["remove-part"] = typeof(Views.SettingsForm.RemovePartView);

        // Operations
        _panelViewTypes["add-operation"] = typeof(Views.SettingsForm.AddOperationView);
        _panelViewTypes["edit-operation"] = typeof(Views.SettingsForm.EditOperationView);
        _panelViewTypes["remove-operation"] = typeof(Views.SettingsForm.RemoveOperationView);

        // Locations
        _panelViewTypes["add-location"] = typeof(Views.SettingsForm.AddLocationView);
        _panelViewTypes["edit-location"] = typeof(Views.SettingsForm.EditLocationView);
        _panelViewTypes["remove-location"] = typeof(Views.SettingsForm.RemoveLocationView);

        // Item Types
        _panelViewTypes["add-itemtype"] = typeof(Views.SettingsForm.AddItemTypeView);
        _panelViewTypes["edit-itemtype"] = typeof(Views.SettingsForm.EditItemTypeView);
        _panelViewTypes["remove-itemtype"] = typeof(Views.SettingsForm.RemoveItemTypeView);

        // Advanced Features
        _panelViewTypes["theme-builder"] = typeof(Views.SettingsForm.ThemeBuilderView);
        _panelViewTypes["shortcuts"] = typeof(Views.SettingsForm.ShortcutsView);
        _panelViewTypes["about"] = typeof(Views.SettingsForm.AboutView);

        // Additional Administrative Features
        _panelViewTypes["system-health"] = typeof(Views.SettingsForm.SystemHealthView);
        _panelViewTypes["backup-recovery"] = typeof(Views.SettingsForm.BackupRecoveryView);
        _panelViewTypes["security-permissions"] = typeof(Views.SettingsForm.SecurityPermissionsView);
    }

    /// <summary>
    /// Creates ViewModel instance with dependency injection.
    /// </summary>
    private async Task<BaseViewModel> CreateViewModelAsync(Type viewModelType, PerformanceLevel performanceLevel)
    {
        try
        {
            var viewModel = _serviceProvider.GetRequiredService(viewModelType) as BaseViewModel;

            if (viewModel == null)
            {
                throw new InvalidOperationException($"Could not create ViewModel of type {viewModelType.Name}");
            }

            // Initialize ViewModel based on performance level
            if (viewModel is IPerformanceAware performanceAware)
            {
                await performanceAware.InitializeAsync(performanceLevel);
            }

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ViewModel of type {ViewModelType}", viewModelType.Name);
            throw;
        }
    }

    /// <summary>
    /// Creates View instance with ViewModel binding.
    /// </summary>
    private async Task<UserControl> CreateViewAsync(string categoryId, BaseViewModel viewModel, PerformanceLevel performanceLevel)
    {
        if (!_panelViewTypes.TryGetValue(categoryId, out var viewType))
        {
            throw new InvalidOperationException($"No view type registered for category {categoryId}");
        }

        try
        {
            var view = Activator.CreateInstance(viewType) as UserControl;

            if (view == null)
            {
                throw new InvalidOperationException($"Could not create View of type {viewType.Name}");
            }

            // Set DataContext
            view.DataContext = viewModel;

            // Initialize View based on performance level
            if (view is IPerformanceAware performanceAware)
            {
                await performanceAware.InitializeAsync(performanceLevel);
            }

            return view;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating View of type {ViewType}", viewType.Name);
            throw;
        }
    }

    /// <summary>
    /// Determines current system performance level.
    /// </summary>
    private PerformanceLevel GetCurrentPerformanceLevel()
    {
        // Simple performance determination based on available memory and CPU
        // In a real implementation, this could check system metrics
        var availableMemory = GC.GetTotalMemory(false);

        if (availableMemory > 500_000_000) // > 500MB
        {
            return PerformanceLevel.High;
        }
        else if (availableMemory > 200_000_000) // > 200MB
        {
            return PerformanceLevel.Medium;
        }
        else
        {
            return PerformanceLevel.Low;
        }
    }

    /// <summary>
    /// Gets maximum panels allowed for performance level.
    /// </summary>
    private int GetMaxPanelsForPerformanceLevel(PerformanceLevel level)
    {
        return level switch
        {
            PerformanceLevel.High => 10,
            PerformanceLevel.Medium => 6,
            PerformanceLevel.Low => 3,
            _ => 6
        };
    }

    #endregion
}

/// <summary>
/// Performance levels for adaptive panel management.
/// </summary>
public enum PerformanceLevel
{
    Low,
    Medium,
    High
}

/// <summary>
/// Interface for performance-aware components.
/// </summary>
public interface IPerformanceAware
{
    Task InitializeAsync(PerformanceLevel performanceLevel);
}

#endregion

#region Settings Panel State Manager Service

/// <summary>
/// Interface for settings panel state management with change tracking.
/// </summary>
public interface ISettingsPanelStateManager
{
    /// <summary>
    /// Indicates if any panels have unsaved changes.
    /// </summary>
    bool HasAnyUnsavedChanges { get; }

    /// <summary>
    /// Gets count of panels with unsaved changes.
    /// </summary>
    int UnsavedChangesCount { get; }

    /// <summary>
    /// Gets all panel IDs with unsaved changes.
    /// </summary>
    IEnumerable<string> PanelsWithUnsavedChanges { get; }

    /// <summary>
    /// Creates a state snapshot for the specified panel.
    /// </summary>
    void CreateSnapshot(string panelId, BaseViewModel viewModel);

    /// <summary>
    /// Updates the current state for a panel and checks for changes.
    /// </summary>
    void UpdateCurrentState(string panelId, BaseViewModel viewModel);

    /// <summary>
    /// Checks if the specified panel has unsaved changes.
    /// </summary>
    bool HasUnsavedChanges(string panelId);

    /// <summary>
    /// Saves changes for the specified panel.
    /// </summary>
    Task<ServiceResult> SaveChangesAsync(string panelId);

    /// <summary>
    /// Saves changes for all panels with unsaved changes.
    /// </summary>
    Task<ServiceResult> SaveAllChangesAsync();

    /// <summary>
    /// Reverts changes for the specified panel to original state.
    /// </summary>
    Task<ServiceResult> RevertChangesAsync(string panelId, BaseViewModel viewModel);

    /// <summary>
    /// Reverts changes for all panels with unsaved changes.
    /// </summary>
    Task<ServiceResult> RevertAllChangesAsync();

    /// <summary>
    /// Removes state tracking for the specified panel.
    /// </summary>
    void RemoveSnapshot(string panelId);

    /// <summary>
    /// Gets state information for the specified panel.
    /// </summary>
    PanelStateSnapshot? GetSnapshot(string panelId);

    /// <summary>
    /// Event raised when state changes occur.
    /// </summary>
    event EventHandler<PanelStateChangedEventArgs>? StateChanged;
}

/// <summary>
/// Settings panel state manager for tracking changes and managing snapshots.
/// Provides per-panel change tracking with state snapshots and rollback capabilities.
/// Enhanced with comprehensive error handling and performance optimization.
/// </summary>
public class SettingsPanelStateManager : ISettingsPanelStateManager
{
    private readonly ILogger<SettingsPanelStateManager> _logger;
    private readonly Dictionary<string, PanelStateSnapshot> _stateSnapshots;

    public SettingsPanelStateManager(ILogger<SettingsPanelStateManager> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stateSnapshots = new Dictionary<string, PanelStateSnapshot>();
        _logger.LogDebug("SettingsPanelStateManager initialized successfully");
    }

    #region Events

    /// <summary>
    /// Event raised when state changes occur.
    /// </summary>
    public event EventHandler<PanelStateChangedEventArgs>? StateChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates if any panels have unsaved changes.
    /// </summary>
    public bool HasAnyUnsavedChanges => _stateSnapshots.Values.Any(s => s.HasChanges);

    /// <summary>
    /// Gets count of panels with unsaved changes.
    /// </summary>
    public int UnsavedChangesCount => _stateSnapshots.Values.Count(s => s.HasChanges);

    /// <summary>
    /// Gets all panel IDs with unsaved changes.
    /// </summary>
    public IEnumerable<string> PanelsWithUnsavedChanges =>
        _stateSnapshots.Where(kvp => kvp.Value.HasChanges).Select(kvp => kvp.Key);

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a state snapshot for the specified panel.
    /// </summary>
    public void CreateSnapshot(string panelId, BaseViewModel viewModel)
    {
        if (string.IsNullOrEmpty(panelId))
        {
            throw new ArgumentException("Panel ID cannot be null or empty", nameof(panelId));
        }

        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        try
        {
            var snapshot = new PanelStateSnapshot
            {
                PanelId = panelId,
                Timestamp = DateTime.UtcNow,
                OriginalValues = ExtractViewModelState(viewModel),
                CurrentValues = ExtractViewModelState(viewModel)
            };

            _stateSnapshots[panelId] = snapshot;

            _logger.LogDebug("Created state snapshot for panel {PanelId}", panelId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating state snapshot for panel {PanelId}", panelId);
            throw;
        }
    }

    /// <summary>
    /// Updates the current state for a panel and checks for changes.
    /// </summary>
    public void UpdateCurrentState(string panelId, BaseViewModel viewModel)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            _logger.LogWarning("No snapshot found for panel {PanelId}, creating new snapshot", panelId);
            CreateSnapshot(panelId, viewModel);
            return;
        }

        try
        {
            var previousHasChanges = snapshot.HasChanges;
            snapshot.CurrentValues = ExtractViewModelState(viewModel);
            snapshot.LastModified = DateTime.UtcNow;

            var currentHasChanges = snapshot.HasChanges;

            // Raise event if change status changed
            if (previousHasChanges != currentHasChanges)
            {
                OnStateChanged(panelId, currentHasChanges);
            }

            _logger.LogDebug("Updated current state for panel {PanelId}, HasChanges: {HasChanges}",
                panelId, currentHasChanges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating current state for panel {PanelId}", panelId);
        }
    }

    /// <summary>
    /// Checks if the specified panel has unsaved changes.
    /// </summary>
    public bool HasUnsavedChanges(string panelId)
    {
        return _stateSnapshots.TryGetValue(panelId, out var snapshot) && snapshot.HasChanges;
    }

    /// <summary>
    /// Saves changes for the specified panel.
    /// </summary>
    public Task<ServiceResult> SaveChangesAsync(string panelId)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            return Task.FromResult(ServiceResult.Failure($"No snapshot found for panel {panelId}"));
        }

        try
        {
            _logger.LogInformation("Saving changes for panel {PanelId}", panelId);

            // Update original values to match current (simulate save)
            snapshot.OriginalValues = new Dictionary<string, object?>(snapshot.CurrentValues);
            snapshot.LastSaved = DateTime.UtcNow;

            OnStateChanged(panelId, false);

            _logger.LogInformation("Successfully saved changes for panel {PanelId}", panelId);
            return Task.FromResult(ServiceResult.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes for panel {PanelId}", panelId);
            return Task.FromResult(ServiceResult.Failure($"Error saving changes: {ex.Message}"));
        }
    }

    /// <summary>
    /// Saves changes for all panels with unsaved changes.
    /// </summary>
    public async Task<ServiceResult> SaveAllChangesAsync()
    {
        var panelsWithChanges = PanelsWithUnsavedChanges.ToList();

        if (!panelsWithChanges.Any())
        {
            return ServiceResult.Success("No changes to save");
        }

        try
        {
            _logger.LogInformation("Saving changes for {Count} panels", panelsWithChanges.Count);

            var tasks = panelsWithChanges.Select(SaveChangesAsync);
            var results = await Task.WhenAll(tasks);

            var failedResults = results.Where(r => !r.IsSuccess).ToList();

            if (failedResults.Any())
            {
                var errorMessages = string.Join("; ", failedResults.Select(r => r.Message));
                return ServiceResult.Failure($"Some saves failed: {errorMessages}");
            }

            _logger.LogInformation("Successfully saved all changes for {Count} panels", panelsWithChanges.Count);
            return ServiceResult.Success($"Saved changes for {panelsWithChanges.Count} panels");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving all changes");
            return ServiceResult.Failure($"Error saving all changes: {ex.Message}");
        }
    }

    /// <summary>
    /// Reverts changes for the specified panel to original state.
    /// </summary>
    public Task<ServiceResult> RevertChangesAsync(string panelId, BaseViewModel viewModel)
    {
        if (!_stateSnapshots.TryGetValue(panelId, out var snapshot))
        {
            return Task.FromResult(ServiceResult.Failure($"No snapshot found for panel {panelId}"));
        }

        try
        {
            _logger.LogInformation("Reverting changes for panel {PanelId}", panelId);

            // Restore original values to ViewModel
            RestoreViewModelState(viewModel, snapshot.OriginalValues);

            // Update current values to match original
            snapshot.CurrentValues = new Dictionary<string, object?>(snapshot.OriginalValues);

            OnStateChanged(panelId, false);

            _logger.LogInformation("Successfully reverted changes for panel {PanelId}", panelId);
            return Task.FromResult(ServiceResult.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting changes for panel {PanelId}", panelId);
            return Task.FromResult(ServiceResult.Failure($"Error reverting changes: {ex.Message}"));
        }
    }

    /// <summary>
    /// Reverts changes for all panels with unsaved changes.
    /// </summary>
    public Task<ServiceResult> RevertAllChangesAsync()
    {
        var panelsWithChanges = PanelsWithUnsavedChanges.ToList();

        if (!panelsWithChanges.Any())
        {
            return Task.FromResult(ServiceResult.Success("No changes to revert"));
        }

        try
        {
            _logger.LogInformation("Reverting changes for {Count} panels", panelsWithChanges.Count);

            // Note: For full implementation, we would need references to ViewModels
            // For now, we'll just reset the snapshots
            foreach (var panelId in panelsWithChanges)
            {
                if (_stateSnapshots.TryGetValue(panelId, out var snapshot))
                {
                    snapshot.CurrentValues = new Dictionary<string, object?>(snapshot.OriginalValues);
                    OnStateChanged(panelId, false);
                }
            }

            _logger.LogInformation("Successfully reverted all changes for {Count} panels", panelsWithChanges.Count);
            return Task.FromResult(ServiceResult.Success($"Reverted changes for {panelsWithChanges.Count} panels"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverting all changes");
            return Task.FromResult(ServiceResult.Failure($"Error reverting all changes: {ex.Message}"));
        }
    }

    /// <summary>
    /// Removes state tracking for the specified panel.
    /// </summary>
    public void RemoveSnapshot(string panelId)
    {
        if (_stateSnapshots.Remove(panelId))
        {
            _logger.LogDebug("Removed state snapshot for panel {PanelId}", panelId);
        }
    }

    /// <summary>
    /// Gets state information for the specified panel.
    /// </summary>
    public PanelStateSnapshot? GetSnapshot(string panelId)
    {
        _stateSnapshots.TryGetValue(panelId, out var snapshot);
        return snapshot;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Extracts current state from ViewModel properties.
    /// </summary>
    private Dictionary<string, object?> ExtractViewModelState(BaseViewModel viewModel)
    {
        var state = new Dictionary<string, object?>();

        // Use reflection to get all public properties
        var properties = viewModel.GetType().GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanRead && !property.PropertyType.IsSubclassOf(typeof(System.Windows.Input.ICommand)))
            {
                try
                {
                    var value = property.GetValue(viewModel);
                    state[property.Name] = value;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not extract property {PropertyName} from {ViewModelType}",
                        property.Name, viewModel.GetType().Name);
                }
            }
        }

        return state;
    }

    /// <summary>
    /// Restores ViewModel state from saved values.
    /// </summary>
    private void RestoreViewModelState(BaseViewModel viewModel, Dictionary<string, object?> state)
    {
        var properties = viewModel.GetType().GetProperties(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanWrite && state.TryGetValue(property.Name, out var value))
            {
                try
                {
                    property.SetValue(viewModel, value);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not restore property {PropertyName} on {ViewModelType}",
                        property.Name, viewModel.GetType().Name);
                }
            }
        }
    }

    /// <summary>
    /// Raises state changed event.
    /// </summary>
    private void OnStateChanged(string panelId, bool hasChanges)
    {
        StateChanged?.Invoke(this, new PanelStateChangedEventArgs
        {
            PanelId = panelId,
            HasChanges = hasChanges,
            Timestamp = DateTime.UtcNow
        });
    }

    #endregion
}

/// <summary>
/// Panel state snapshot for change tracking.
/// </summary>
public class PanelStateSnapshot
{
    public string PanelId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime? LastModified { get; set; }
    public DateTime? LastSaved { get; set; }
    public Dictionary<string, object?> OriginalValues { get; set; } = new();
    public Dictionary<string, object?> CurrentValues { get; set; } = new();

    /// <summary>
    /// Indicates if current values differ from original values.
    /// </summary>
    public bool HasChanges
    {
        get
        {
            if (OriginalValues.Count != CurrentValues.Count)
                return true;

            foreach (var kvp in OriginalValues)
            {
                if (!CurrentValues.TryGetValue(kvp.Key, out var currentValue))
                    return true;

                if (!Equals(kvp.Value, currentValue))
                    return true;
            }

            return false;
        }
    }
}

/// <summary>
/// Event arguments for panel state changes.
/// </summary>
public class PanelStateChangedEventArgs : EventArgs
{
    public string PanelId { get; set; } = string.Empty;
    public bool HasChanges { get; set; }
    public DateTime Timestamp { get; set; }
}

#endregion
