using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Universal progress overlay service for MTM WIP Application.
/// Manages progress overlay display and coordinates with UI components.
/// Designed to work alongside existing ProgressService in QuickButtons.cs.
/// </summary>
public interface IProgressOverlayService : IDisposable
{
    /// <summary>
    /// Gets the current progress overlay ViewModel if active
    /// </summary>
    ProgressOverlayViewModel? CurrentProgressOverlay { get; }

    /// <summary>
    /// Gets whether the overlay is currently visible
    /// </summary>
    bool IsOverlayVisible { get; }

    /// <summary>
    /// Event fired when overlay visibility changes
    /// </summary>
    event EventHandler<bool>? OverlayVisibilityChanged;

    /// <summary>
    /// Show progress overlay with specified configuration
    /// </summary>
    Task ShowProgressOverlayAsync(string title, string? statusMessage = null,
        bool isDeterminate = false, bool cancellable = true);

    /// <summary>
    /// Update the current progress overlay
    /// </summary>
    Task UpdateProgressOverlayAsync(double value, string? statusMessage = null,
        string? details = null);

    /// <summary>
    /// Complete the progress overlay successfully
    /// </summary>
    Task CompleteProgressOverlayAsync(string? completionMessage = null,
        int autoCloseDelayMs = 2000);

    /// <summary>
    /// Set progress overlay to error state
    /// </summary>
    Task SetProgressOverlayErrorAsync(string errorMessage, string? details = null);

    /// <summary>
    /// Hide the progress overlay
    /// </summary>
    Task HideProgressOverlayAsync();

    /// <summary>
    /// Get cancellation token for current operation
    /// </summary>
    CancellationToken GetCancellationToken();
}

/// <summary>
/// Implementation of universal progress overlay service
/// </summary>
public class ProgressOverlayService : IProgressOverlayService, INotifyPropertyChanged
{
    private const string SystemUserId = "System";
    private readonly ILogger<ProgressOverlayService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly object _lockObject = new();

    private ProgressOverlayViewModel? _currentProgressOverlay;
    private bool _disposed = false;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Events

    public event EventHandler<bool>? OverlayVisibilityChanged;

    #endregion

    #region Properties

    public ProgressOverlayViewModel? CurrentProgressOverlay
    {
        get
        {
            lock (_lockObject)
            {
                return _currentProgressOverlay;
            }
        }
        private set
        {
            bool visibilityChanged = false;
            lock (_lockObject)
            {
                var wasVisible = _currentProgressOverlay != null;
                var isVisible = value != null;
                visibilityChanged = wasVisible != isVisible;

                _currentProgressOverlay = value;
            }

            if (visibilityChanged)
            {
                // Ensure property change notifications are on UI thread
                if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
                {
                    OnPropertyChanged(nameof(CurrentProgressOverlay));
                    OnPropertyChanged(nameof(IsOverlayVisible));
                    OverlayVisibilityChanged?.Invoke(this, value != null);
                }
                else
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        OnPropertyChanged(nameof(CurrentProgressOverlay));
                        OnPropertyChanged(nameof(IsOverlayVisible));
                        OverlayVisibilityChanged?.Invoke(this, value != null);
                    });
                }
            }
        }
    }

    public bool IsOverlayVisible
    {
        get
        {
            lock (_lockObject)
            {
                return _currentProgressOverlay != null;
            }
        }
    }

    #endregion

    #region Constructor

    public ProgressOverlayService(ILogger<ProgressOverlayService> logger, ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _logger.LogDebug("ProgressOverlayService initialized successfully");
    }

    #endregion

    #region Public Methods

    public async Task ShowProgressOverlayAsync(string title, string? statusMessage = null,
        bool isDeterminate = false, bool cancellable = true)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressOverlayService));

        try
        {
            ProgressOverlayViewModel? newOverlay = null;

            lock (_lockObject)
            {
                // Dispose existing overlay if present
                if (_currentProgressOverlay != null)
                {
                    _logger.LogWarning("Replacing existing progress overlay with new one: {Title}", title);
                    _currentProgressOverlay.DismissRequested -= OnProgressDismissRequested;
                    _currentProgressOverlay.CancelRequested -= OnProgressCancelRequested;
                    _currentProgressOverlay.Dispose();
                }

                // Create new progress overlay ViewModel
                var overlayLogger = _loggerFactory.CreateLogger<ProgressOverlayViewModel>();
                newOverlay = new ProgressOverlayViewModel(overlayLogger);
                newOverlay.DismissRequested += OnProgressDismissRequested;
                newOverlay.CancelRequested += OnProgressCancelRequested;
            }

            // Set the overlay (this triggers property change notifications)
            CurrentProgressOverlay = newOverlay;

            // Start the progress operation
            CurrentProgressOverlay!.StartProgress(title, statusMessage, isDeterminate, cancellable);

            _logger.LogInformation("Progress overlay shown: {Title}", title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing progress overlay: {Title}", title);
            await ErrorHandling.HandleErrorAsync(ex, "Show progress overlay", SystemUserId);
            throw;
        }
    }

    public async Task UpdateProgressOverlayAsync(double value, string? statusMessage = null,
        string? details = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressOverlayService));

        try
        {
            ProgressOverlayViewModel? overlay;
            lock (_lockObject)
            {
                overlay = _currentProgressOverlay;
            }

            if (overlay == null)
            {
                _logger.LogWarning("Attempted to update progress overlay when none is active");
                return;
            }

            overlay.UpdateProgress(value, statusMessage, details);
            _logger.LogDebug("Progress overlay updated: {Value}", value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating progress overlay");
            await ErrorHandling.HandleErrorAsync(ex, "Update progress overlay", SystemUserId);
        }
    }

    public async Task CompleteProgressOverlayAsync(string? completionMessage = null,
        int autoCloseDelayMs = 2000)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressOverlayService));

        try
        {
            ProgressOverlayViewModel? overlay;
            lock (_lockObject)
            {
                overlay = _currentProgressOverlay;
            }

            if (overlay == null)
            {
                _logger.LogWarning("Attempted to complete progress overlay when none is active");
                return;
            }

            await overlay.CompleteAsync(completionMessage, autoCloseDelayMs);
            _logger.LogInformation("Progress overlay completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing progress overlay");
            await ErrorHandling.HandleErrorAsync(ex, "Complete progress overlay", SystemUserId);
        }
    }

    public async Task SetProgressOverlayErrorAsync(string errorMessage, string? details = null)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressOverlayService));

        try
        {
            ProgressOverlayViewModel? overlay;
            lock (_lockObject)
            {
                overlay = _currentProgressOverlay;
            }

            if (overlay == null)
            {
                _logger.LogWarning("Attempted to set error on progress overlay when none is active");
                return;
            }

            await overlay.SetErrorAsync(errorMessage, details);
            _logger.LogError("Progress overlay error set: {Error}", errorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting progress overlay error state");
            await ErrorHandling.HandleErrorAsync(ex, "Set progress overlay error", SystemUserId);
        }
    }

    public async Task HideProgressOverlayAsync()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressOverlayService));

        try
        {
            lock (_lockObject)
            {
                if (_currentProgressOverlay != null)
                {
                    _currentProgressOverlay.DismissRequested -= OnProgressDismissRequested;
                    _currentProgressOverlay.CancelRequested -= OnProgressCancelRequested;
                    _currentProgressOverlay.Dispose();
                    _currentProgressOverlay = null;
                }
            }

            // Force property change notification directly
            if (Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                OnPropertyChanged(nameof(CurrentProgressOverlay));
                OnPropertyChanged(nameof(IsOverlayVisible));
                OverlayVisibilityChanged?.Invoke(this, false);
            }
            else
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    OnPropertyChanged(nameof(CurrentProgressOverlay));
                    OnPropertyChanged(nameof(IsOverlayVisible));
                    OverlayVisibilityChanged?.Invoke(this, false);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hiding progress overlay");
            await ErrorHandling.HandleErrorAsync(ex, "Hide progress overlay", SystemUserId);
        }
    }

    public CancellationToken GetCancellationToken()
    {
        lock (_lockObject)
        {
            return _currentProgressOverlay?.GetCancellationToken() ?? CancellationToken.None;
        }
    }

    #endregion

    #region Event Handlers

    private async void OnProgressDismissRequested()
    {
        try
        {
            await HideProgressOverlayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling progress overlay dismiss request");
        }
    }

    private void OnProgressCancelRequested(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Progress overlay cancellation requested");
            // Cancellation logic is handled by the ViewModel
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling progress overlay cancel request");
        }
    }

    #endregion

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                lock (_lockObject)
                {
                    if (_currentProgressOverlay != null)
                    {
                        _currentProgressOverlay.DismissRequested -= OnProgressDismissRequested;
                        _currentProgressOverlay.CancelRequested -= OnProgressCancelRequested;
                        _currentProgressOverlay.Dispose();
                        _currentProgressOverlay = null;
                    }
                }

                _disposed = true;
                _logger.LogDebug("ProgressOverlayService disposed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during ProgressOverlayService disposal");
            }
        }
    }

    #endregion
}
