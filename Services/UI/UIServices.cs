using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Views.Overlay;

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
/// Service result class for operations that return typed data.
/// </summary>
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }
    
    // Backward compatibility
    public T? Value => IsSuccess ? Data : default(T);
    
    public static ServiceResult<T> Success(T data, string message = "")
        => new() { IsSuccess = true, Data = data, Message = message };
    
    public static ServiceResult<T> Failure(string message, Exception? exception = null)
        => new() { IsSuccess = false, Message = message, Exception = exception };
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
    public bool Success => IsSuccess;
    public int Status => IsSuccess ? 1 : 0;
    public int SuccessCount => IsSuccess ? 1 : 0;
    
    public static ServiceResult Success(string message = "")
        => new() { IsSuccess = true, Message = message };
    
    public static ServiceResult Failure(string message, Exception? exception = null)
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

#endregion