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
        _logger.LogDebug("NavigateToInternal started - Current view: {CurrentViewType}, New view: {NewViewType}", 
            _currentView?.GetType().Name ?? "null", newView?.GetType().Name ?? "null");
            
        try
        {
            if (_currentView != null)
            {
                _backStack.Push(_currentView);
                _logger.LogDebug("Current view added to back stack. Back stack count: {BackStackCount}", _backStack.Count);
            }

            // Clear forward stack when navigating to new view
            _forwardStack.Clear();
            _logger.LogDebug("Forward stack cleared. Forward stack count: {ForwardStackCount}", _forwardStack.Count);

            CurrentView = newView;
            _logger.LogInformation("Navigation completed successfully to: {NewViewType}", newView?.GetType().Name ?? "null");

            _logger.LogDebug("Raising Navigated event with {SubscriberCount} subscribers", 
                Navigated?.GetInvocationList()?.Length ?? 0);
            
            var eventArgs = new NavigationEventArgs(newView, NavigationDirection.New);
            Navigated?.Invoke(this, eventArgs);
            
            _logger.LogDebug("Navigated event raised successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in NavigateToInternal for view: {NewViewType}", newView?.GetType().Name ?? "null");
            throw;
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        try
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            _logger.LogTrace("PropertyChanged event raised for: {PropertyName} in NavigationService", propertyName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error raising PropertyChanged event for: {PropertyName} in NavigationService", propertyName);
            throw;
        }
    }
}

/// <summary>
/// Navigation event arguments.
/// </summary>
public class NavigationEventArgs : EventArgs
{
    public object? Target { get; }
    public NavigationDirection Direction { get; }
    public object? Source { get; }

    public NavigationEventArgs(object? target, NavigationDirection direction, object? source = null)
    {
        Target = target;
        Direction = direction;
        Source = source;
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
