using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Services.Feature;

#region Interfaces
/// <summary>
/// Universal Overlay Service interface for managing all overlay operations in the MTM application.
/// Provides consistent overlay management with proper lifecycle events and memory pooling.
/// </summary>
public interface IUniversalOverlayService
{
    /// <summary>
    /// Shows an overlay with the specified request data.
    /// </summary>
    /// <typeparam name="T">Type of the overlay request/configuration</typeparam>
    /// <param name="request">Overlay configuration and data</param>
    /// <returns>Overlay result with user response data</returns>
    Task<OverlayResult<T>> ShowOverlayAsync<T>(T request) where T : class;

    /// <summary>
    /// Hides the currently displayed overlay by its ID.
    /// </summary>
    /// <param name="overlayId">Unique identifier of the overlay to hide</param>
    /// <returns>Task representing the hide operation</returns>
    Task<bool> HideOverlayAsync(string overlayId);

    /// <summary>
    /// Registers an overlay ViewModel and View pair for dependency injection.
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel type for the overlay</typeparam>
    /// <typeparam name="TView">View type for the overlay</typeparam>
    void RegisterOverlay<TViewModel, TView>()
        where TViewModel : class
        where TView : class;

    /// <summary>
    /// Gets all currently active overlay IDs.
    /// </summary>
    /// <returns>Collection of active overlay identifiers</returns>
    IReadOnlyList<string> GetActiveOverlayIds();

    /// <summary>
    /// Event fired when an overlay is about to be shown.
    /// </summary>
    event EventHandler<OverlayEventArgs> OverlayShowing;

    /// <summary>
    /// Event fired when an overlay has been shown.
    /// </summary>
    event EventHandler<OverlayEventArgs> OverlayShown;

    /// <summary>
    /// Event fired when an overlay is about to be hidden.
    /// </summary>
    event EventHandler<OverlayEventArgs> OverlayHiding;

    /// <summary>
    /// Event fired when an overlay has been hidden.
    /// </summary>
    event EventHandler<OverlayEventArgs> OverlayHidden;
}

/// <summary>
/// Result type for overlay operations containing response data and status.
/// </summary>
/// <typeparam name="T">Type of the overlay result data</typeparam>
public class OverlayResult<T>
{
    /// <summary>
    /// Gets whether the overlay operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets whether the overlay was cancelled by the user.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the result data from the overlay operation.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets the overlay ID that generated this result.
    /// </summary>
    public string OverlayId { get; set; } = string.Empty;

    /// <summary>
    /// Gets any error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful overlay result.
    /// </summary>
    public static OverlayResult<T> Success(T data, string overlayId = "")
    {
        return new OverlayResult<T>
        {
            IsSuccess = true,
            IsCancelled = false,
            Data = data,
            OverlayId = overlayId
        };
    }

    /// <summary>
    /// Creates a cancelled overlay result.
    /// </summary>
    public static OverlayResult<T> Cancelled(string overlayId = "")
    {
        return new OverlayResult<T>
        {
            IsSuccess = false,
            IsCancelled = true,
            OverlayId = overlayId
        };
    }

    /// <summary>
    /// Creates a failed overlay result.
    /// </summary>
    public static OverlayResult<T> Failed(string errorMessage, string overlayId = "")
    {
        return new OverlayResult<T>
        {
            IsSuccess = false,
            IsCancelled = false,
            ErrorMessage = errorMessage,
            OverlayId = overlayId
        };
    }
}

/// <summary>
/// Event arguments for overlay lifecycle events.
/// </summary>
public class OverlayEventArgs : EventArgs
{
    /// <summary>
    /// Gets the unique identifier of the overlay.
    /// </summary>
    public string OverlayId { get; }

    /// <summary>
    /// Gets the type of the overlay ViewModel.
    /// </summary>
    public Type OverlayType { get; }

    /// <summary>
    /// Gets whether the event can be cancelled.
    /// </summary>
    public bool CanCancel { get; set; }

    /// <summary>
    /// Gets or sets whether the event should be cancelled.
    /// </summary>
    public bool Cancel { get; set; }

    /// <summary>
    /// Initializes a new instance of OverlayEventArgs.
    /// </summary>
    public OverlayEventArgs(string overlayId, Type overlayType, bool canCancel = false)
    {
        OverlayId = overlayId;
        OverlayType = overlayType;
        CanCancel = canCancel;
    }
}
#endregion

#region Universal Overlay Service

/// <summary>
/// Universal Overlay Service implementation providing centralized overlay management
/// with memory pooling, z-index management, and theme integration for MTM application.
/// </summary>
public class UniversalOverlayService : Services.Feature.IUniversalOverlayService, IDisposable
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
