using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services.Infrastructure;

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
