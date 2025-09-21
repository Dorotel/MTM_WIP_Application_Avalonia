using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for simple loading overlay with spinner animation.
/// Used for quick operations that don't need progress tracking.
/// </summary>
public partial class LoadingOverlayViewModel : BaseOverlayViewModel
{
    #region Observable Properties

    /// <summary>
    /// Loading message to display
    /// </summary>
    [ObservableProperty]
    private string loadingMessage = "Loading...";

    /// <summary>
    /// Whether to show a spinner animation
    /// </summary>
    [ObservableProperty]
    private bool showSpinner = true;

    /// <summary>
    /// Size of the spinner (Small, Medium, Large)
    /// </summary>
    [ObservableProperty]
    private string spinnerSize = "Medium";

    /// <summary>
    /// Whether to show a semi-transparent background
    /// </summary>
    [ObservableProperty]
    private bool showBackground = true;

    /// <summary>
    /// Timeout in seconds (0 = no timeout)
    /// </summary>
    [ObservableProperty]
    private int timeoutSeconds = 0;

    #endregion

    #region Private Fields

    private DateTime _startTime;
    private System.Timers.Timer? _timeoutTimer;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when loading times out
    /// </summary>
    public event EventHandler? LoadingTimeout;

    #endregion

    #region Constructor

    public LoadingOverlayViewModel(ILogger<LoadingOverlayViewModel> logger)
        : base(logger)
    {
        Title = "Loading";
        // Loading overlays typically don't have confirm/cancel buttons
        CanClose = false;
    }

    #endregion

    #region BaseOverlayViewModel Overrides

    protected override string GetDefaultTitle() => "Loading";

    protected override async Task<bool> OnConfirmAsync()
    {
        // Loading overlay doesn't support confirmation
        return false;
    }

    protected override async Task OnCancelAsync()
    {
        // Loading overlay doesn't support cancellation by default
        await Task.CompletedTask;
    }

    protected override void OnClosed(OverlayCloseReason reason)
    {
        base.OnClosed(reason);
        StopTimeout();
    }

    public async Task ShowLoadingAsync()
    {
        _startTime = DateTime.Now;
        StartTimeoutIfNeeded();
        await ShowAsync();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show loading overlay with default settings
    /// </summary>
    public void ShowLoading(string message = "Loading...")
    {
        LoadingMessage = message;
        ShowSpinner = true;
        ShowBackground = true;
        TimeoutSeconds = 0; // No timeout

        Show();
        Logger.LogInformation("Loading started: {Message}", message);
    }

    /// <summary>
    /// Show loading overlay with timeout
    /// </summary>
    public void ShowLoadingWithTimeout(string message, int timeoutSeconds)
    {
        LoadingMessage = message;
        ShowSpinner = true;
        ShowBackground = true;
        TimeoutSeconds = timeoutSeconds;

        Show();
        Logger.LogInformation("Loading started with timeout: {Message} ({Timeout}s)", message, timeoutSeconds);
    }

    /// <summary>
    /// Show minimal loading overlay (no background, small spinner)
    /// </summary>
    public void ShowMinimalLoading(string message = "Loading...")
    {
        LoadingMessage = message;
        ShowSpinner = true;
        ShowBackground = false;
        SpinnerSize = "Small";
        TimeoutSeconds = 0;

        Show();
        Logger.LogInformation("Minimal loading started: {Message}", message);
    }

    /// <summary>
    /// Update the loading message
    /// </summary>
    public void UpdateMessage(string message)
    {
        LoadingMessage = message;
        Logger.LogDebug("Loading message updated: {Message}", message);
    }

    /// <summary>
    /// Complete the loading operation
    /// </summary>
    public async Task CompleteAsync()
    {
        try
        {
            var elapsed = DateTime.Now - _startTime;
            Logger.LogInformation("Loading completed in {ElapsedMs}ms", elapsed.TotalMilliseconds);

            StopTimeout();
            await CloseAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Loading completion failed");
        }
    }

    /// <summary>
    /// Fail the loading operation with error message
    /// </summary>
    public async Task FailAsync(string errorMessage)
    {
        try
        {
            var elapsed = DateTime.Now - _startTime;
            Logger.LogWarning("Loading failed after {ElapsedMs}ms: {Error}", elapsed.TotalMilliseconds, errorMessage);

            StopTimeout();
            LoadingMessage = $"Failed: {errorMessage}";
            ShowSpinner = false;

            // Brief delay to show error, then close
            await Task.Delay(1500);
            await CloseAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Loading failure handling failed");
        }
    }

    #endregion

    #region Private Methods

    private void StartTimeoutIfNeeded()
    {
        if (TimeoutSeconds > 0)
        {
            _timeoutTimer = new System.Timers.Timer(TimeoutSeconds * 1000);
            _timeoutTimer.Elapsed += OnTimeoutElapsed;
            _timeoutTimer.AutoReset = false;
            _timeoutTimer.Start();
        }
    }

    private void StopTimeout()
    {
        if (_timeoutTimer != null)
        {
            _timeoutTimer.Stop();
            _timeoutTimer.Dispose();
            _timeoutTimer = null;
        }
    }

    private async void OnTimeoutElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {
            Logger.LogWarning("Loading timeout after {Timeout} seconds", TimeoutSeconds);

            StopTimeout();
            LoadingTimeout?.Invoke(this, EventArgs.Empty);

            await FailAsync("Operation timed out");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling loading timeout");
        }
    }

    #endregion

    #region IDisposable

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopTimeout();
        }
        base.Dispose(disposing);
    }

    #endregion
}
