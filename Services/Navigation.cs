using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

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
            // This would typically resolve T from DI container
            _logger.LogInformation("Navigating to view type: {ViewType}", typeof(T).Name);
            
            // For now, just store the type name
            NavigateToInternal(typeof(T).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to view type: {ViewType}", typeof(T).Name);
        }
    }

    public void NavigateTo(object viewModel)
    {
        try
        {
            if (viewModel == null)
            {
                _logger.LogWarning("Attempted to navigate to null view model");
                return;
            }

            _logger.LogInformation("Navigating to view model: {ViewModelType}", viewModel.GetType().Name);
            NavigateToInternal(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to view model: {ViewModelType}", 
                viewModel?.GetType().Name ?? "null");
        }
    }

    public void NavigateTo(string viewKey)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(viewKey))
            {
                _logger.LogWarning("Attempted to navigate to empty view key");
                return;
            }

            _logger.LogInformation("Navigating to view key: {ViewKey}", viewKey);
            NavigateToInternal(viewKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to navigate to view key: {ViewKey}", viewKey);
        }
    }

    public void GoBack()
    {
        try
        {
            if (!CanGoBack)
            {
                _logger.LogDebug("Cannot go back - no items in back stack");
                return;
            }

            if (_currentView != null)
            {
                _forwardStack.Push(_currentView);
            }

            var previousView = _backStack.Pop();
            CurrentView = previousView;

            _logger.LogInformation("Navigated back to: {ViewType}", 
                previousView?.GetType().Name ?? "null");

            Navigated?.Invoke(this, new NavigationEventArgs(previousView, NavigationDirection.Back));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to go back");
        }
    }

    public void GoForward()
    {
        try
        {
            if (!CanGoForward)
            {
                _logger.LogDebug("Cannot go forward - no items in forward stack");
                return;
            }

            if (_currentView != null)
            {
                _backStack.Push(_currentView);
            }

            var forwardView = _forwardStack.Pop();
            CurrentView = forwardView;

            _logger.LogInformation("Navigated forward to: {ViewType}", 
                forwardView?.GetType().Name ?? "null");

            Navigated?.Invoke(this, new NavigationEventArgs(forwardView, NavigationDirection.Forward));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to go forward");
        }
    }

    public void ClearHistory()
    {
        try
        {
            _backStack.Clear();
            _forwardStack.Clear();
            
            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
            
            _logger.LogInformation("Navigation history cleared");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear navigation history");
        }
    }

    private void NavigateToInternal(object newView)
    {
        if (_currentView != null)
        {
            _backStack.Push(_currentView);
        }

        // Clear forward stack when navigating to new view
        _forwardStack.Clear();

        CurrentView = newView;

        Navigated?.Invoke(this, new NavigationEventArgs(newView, NavigationDirection.New));
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
    public object? Target { get; }
    public NavigationDirection Direction { get; }

    public NavigationEventArgs(object? target, NavigationDirection direction)
    {
        Target = target;
        Direction = direction;
    }
}

/// <summary>
/// Navigation direction enumeration.
/// </summary>
public enum NavigationDirection
{
    New,
    Back,
    Forward
}
