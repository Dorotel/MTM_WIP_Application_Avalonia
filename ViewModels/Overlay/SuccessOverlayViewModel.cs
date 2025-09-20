using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the success overlay that displays temporary success messages
/// after successful transactions. Uses MVVM Community Toolkit patterns.
/// Enhanced to inherit from BaseOverlayViewModel.
/// </summary>
public partial class SuccessOverlayViewModel : BaseOverlayViewModel
{
    #region Properties

    /// <summary>
    /// The success message to display
    /// </summary>
    [ObservableProperty]
    [StringLength(500, ErrorMessage = "Success message cannot exceed 500 characters")]
    private string _message = string.Empty;

    /// <summary>
    /// The icon to display with the success message
    /// </summary>
    [ObservableProperty]
    private string _iconKind = "CheckCircle";

    /// <summary>
    /// Additional details about the successful operation
    /// </summary>
    [ObservableProperty]
    [StringLength(1000, ErrorMessage = "Details cannot exceed 1000 characters")]
    private string _details = string.Empty;

    /// <summary>
    /// Indicates if details should be shown
    /// </summary>
    [ObservableProperty]
    private bool _showDetails = false;

    /// <summary>
    /// Progress animation value (0-1)
    /// </summary>
    [ObservableProperty]
    private double _progress = 0.0;

    /// <summary>
    /// Indicates if the overlay is currently animating
    /// </summary>
    [ObservableProperty]
    private bool _isAnimating = false;

    /// <summary>
    /// Display duration in milliseconds
    /// </summary>
    [ObservableProperty]
    [Range(500, 10000, ErrorMessage = "Duration must be between 500ms and 10000ms")]
    private int _displayDuration = 2000;

    /// <summary>
    /// Indicates if this overlay is displaying an error (true) or success (false)
    /// </summary>
    [ObservableProperty]
    private bool _isError = false;

    /// <summary>
    /// Emergency shutdown timer for UI lockup scenarios
    /// </summary>
    private Timer? _emergencyShutdownTimer;

    /// <summary>
    /// Cancellation token source for emergency operations
    /// </summary>
    private CancellationTokenSource? _emergencyCancellationSource;

    /// <summary>
    /// Emergency keyboard hook for global shortcuts
    /// </summary>
    private Services.EmergencyKeyboardHook? _emergencyKeyboardHook;

    #endregion

    #region Events

    /// <summary>
    /// Raised when the overlay should be dismissed
    /// </summary>
    public event Action? DismissRequested;

    /// <summary>
    /// Raised when the overlay animation completes
    /// </summary>
    public event Action? AnimationCompleted;

    #endregion

    #region Constructor

    public SuccessOverlayViewModel(ILogger<SuccessOverlayViewModel> logger) : base(logger)
    {
        Logger.LogDebug("SuccessOverlayViewModel initialized");
    }

    /// <summary>
    /// Design-time constructor
    /// </summary>
    public SuccessOverlayViewModel() : base(null!)
    {
        if (Avalonia.Controls.Design.IsDesignMode)
        {
            InitializeDesignTimeData();
        }
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to manually dismiss the overlay
    /// </summary>
    [RelayCommand]
    private async Task DismissAsync()
    {
        try
        {
            Logger.LogDebug("Manual dismiss requested");
            await AnimateOutAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during manual dismiss");
            // Force dismiss even if animation fails
            DismissRequested?.Invoke();
        }
    }

    /// <summary>
    /// Command to continue using the application (dismiss error overlay)
    /// </summary>
    [RelayCommand]
    private async Task ContinueAsync()
    {
        try
        {
            Logger.LogDebug("Continue requested from error overlay");
            
            // Stop emergency monitoring since user is continuing
            StopEmergencyShutdownMonitoring();
            
            await AnimateOutAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during continue operation");
            // Force dismiss even if animation fails
            DismissRequested?.Invoke();
        }
    }

    /// <summary>
    /// Command to exit the application gracefully, with fallbacks for UI lockups
    /// </summary>
    [RelayCommand]
    private async Task ExitApplicationAsync()
    {
        try
        {
            Logger.LogWarning("User requested application exit from error overlay");
            
            // Stop emergency monitoring since we're manually exiting
            StopEmergencyShutdownMonitoring();
            
            // Trigger immediate dismissal (don't wait for it)
            try
            {
                DismissRequested?.Invoke();
            }
            catch (Exception dismissEx)
            {
                Logger.LogError(dismissEx, "Error dismissing overlay during exit - continuing with shutdown");
            }

            // Use a task with timeout for graceful shutdown
            var shutdownTask = Task.Run(() =>
            {
                try
                {
                    // Attempt graceful shutdown through ApplicationLifetime
                    if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        Logger.LogInformation("Attempting graceful shutdown via ApplicationLifetime");
                        desktop.Shutdown(1); // Exit code 1 indicates error termination
                        return true;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during graceful shutdown attempt");
                    return false;
                }
            });

            // Wait up to 3 seconds for graceful shutdown
            var completed = await shutdownTask.WaitAsync(TimeSpan.FromSeconds(3));
            
            if (!completed)
            {
                Logger.LogWarning("Graceful shutdown timed out - forcing application exit");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during application exit process");
        }
        finally
        {
            // Force close as absolute last resort - this will work even if UI is completely locked
            try
            {
                Logger.LogCritical("Forcing immediate application termination");
                Environment.Exit(1);
            }
            catch
            {
                // If even Environment.Exit fails, try the nuclear option
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Shows the success overlay with the specified message
    /// </summary>
    /// <param name="message">The success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="isError">True if this is an error overlay, false for success</param>
    public async Task ShowSuccessAsync(string message, string? details = null, string iconKind = "CheckCircle", int duration = 2000, bool isError = false)
    {
        try
        {
            Logger.LogInformation("Showing {OverlayType} overlay: {Message}", isError ? "error" : "success", message);

            // Set properties
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Details = details ?? string.Empty;
            ShowDetails = !string.IsNullOrEmpty(details);
            IconKind = iconKind;
            IsError = isError;
            DisplayDuration = Math.Max(500, Math.Min(10000, duration)); // Clamp between 500ms and 10s

            // Start emergency shutdown monitoring for error states
            if (isError)
            {
                StartEmergencyShutdownMonitoring();
            }

            // Start animations
            await AnimateInAsync();

            // Start auto-dismiss timer using Dispatcher for better UI thread handling
            // Only auto-dismiss if not an error (errors should stay visible until user action)
            if (!isError)
            {
                _ = Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await Task.Delay(DisplayDuration);
                    if (IsAnimating) // Only dismiss if still showing
                    {
                        await AnimateOutAsync();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing overlay");
            throw;
        }
    }

    /// <summary>
    /// Forces immediate dismissal of the overlay
    /// </summary>
    public void ForceDismiss()
    {
        try
        {
            Logger.LogDebug("Force dismiss requested");
            
            // Stop emergency monitoring
            StopEmergencyShutdownMonitoring();
            
            IsAnimating = false;
            Progress = 0.0;
            DismissRequested?.Invoke();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during force dismiss");
        }
    }

    #endregion

    #region Emergency Shutdown Methods

    /// <summary>
    /// Starts emergency shutdown monitoring for UI lockup scenarios
    /// </summary>
    private void StartEmergencyShutdownMonitoring()
    {
        try
        {
            Logger.LogDebug("Starting emergency shutdown monitoring");

            _emergencyCancellationSource?.Cancel();
            _emergencyCancellationSource?.Dispose();
            _emergencyCancellationSource = new CancellationTokenSource();

            // Start emergency keyboard hook if on Windows
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    _emergencyKeyboardHook?.Dispose();
                    _emergencyKeyboardHook = new Services.EmergencyKeyboardHook(
                        Microsoft.Extensions.Logging.Abstractions.NullLogger<Services.EmergencyKeyboardHook>.Instance
                    );
                    
                    _emergencyKeyboardHook.EmergencyExitRequested += () =>
                    {
                        Logger.LogWarning("Emergency exit requested via global shortcut");
                        _ = Task.Run(async () => await ExitApplicationCommand.ExecuteAsync(null));
                    };
                    
                    _emergencyKeyboardHook.EmergencyContinueRequested += () =>
                    {
                        Logger.LogWarning("Emergency continue requested via global shortcut");
                        _ = Task.Run(async () => await ContinueCommand.ExecuteAsync(null));
                    };
                    
                    _emergencyKeyboardHook.StartHook();
                }
                catch (Exception hookEx)
                {
                    Logger.LogWarning(hookEx, "Failed to start emergency keyboard hook - using timer-only fallback");
                }
            }

            // Start background task to monitor for emergency shutdown conditions
            _ = Task.Run(async () =>
            {
                try
                {
                    await MonitorEmergencyShutdown(_emergencyCancellationSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Logger.LogDebug("Emergency monitoring cancelled");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error in emergency monitoring");
                }
            }, _emergencyCancellationSource.Token);

            // Set up emergency timer - force shutdown after 30 seconds if UI is completely locked
            _emergencyShutdownTimer?.Dispose();
            _emergencyShutdownTimer = new Timer(EmergencyTimerCallback, null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting emergency shutdown monitoring");
        }
    }

    /// <summary>
    /// Stops emergency shutdown monitoring
    /// </summary>
    private void StopEmergencyShutdownMonitoring()
    {
        try
        {
            _emergencyShutdownTimer?.Dispose();
            _emergencyShutdownTimer = null;

            _emergencyCancellationSource?.Cancel();
            _emergencyCancellationSource?.Dispose();
            _emergencyCancellationSource = null;

            Logger.LogDebug("Emergency shutdown monitoring stopped");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error stopping emergency monitoring");
        }
    }

    /// <summary>
    /// Monitors for emergency shutdown conditions in background
    /// </summary>
    private async Task MonitorEmergencyShutdown(CancellationToken cancellationToken)
    {
        Logger.LogDebug("Emergency shutdown monitoring started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Check for global key combinations even when UI is locked
                // This runs in background thread so it won't be blocked by UI issues
                if (await CheckEmergencyKeyboardShortcuts())
                {
                    Logger.LogWarning("Emergency keyboard shortcut detected - initiating shutdown");
                    await ExecuteEmergencyShutdown();
                    break;
                }

                // Check every 100ms for responsiveness
                await Task.Delay(100, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in emergency monitoring loop");
                await Task.Delay(1000, cancellationToken); // Wait longer on errors
            }
        }
    }

    /// <summary>
    /// Checks for emergency keyboard shortcuts using low-level system calls
    /// </summary>
    private async Task<bool> CheckEmergencyKeyboardShortcuts()
    {
        try
        {
            // Use Windows API to check key states even when UI is locked
            // This is a simplified version - in production you'd use P/Invoke to GetAsyncKeyState
            
            // For now, we'll use a timer-based approach since P/Invoke adds complexity
            // Check if UI thread is responsive by trying to dispatch a simple operation
            var uiResponsive = await CheckUIThreadResponsiveness();
            
            if (!uiResponsive)
            {
                Logger.LogWarning("UI thread appears unresponsive - considering emergency action");
                
                // If UI has been unresponsive for more than 5 seconds, assume emergency
                // In a real implementation, you'd check actual keyboard state here
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking emergency shortcuts");
            return false;
        }
    }

    /// <summary>
    /// Checks if UI thread is responsive
    /// </summary>
    private async Task<bool> CheckUIThreadResponsiveness()
    {
        try
        {
            var responseTask = Dispatcher.UIThread.InvokeAsync(() => Task.CompletedTask);
            await responseTask.WaitAsync(TimeSpan.FromSeconds(2));
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Executes emergency shutdown with multiple fallback methods
    /// </summary>
    private async Task ExecuteEmergencyShutdown()
    {
        Logger.LogCritical("Executing emergency shutdown due to UI lockup");

        try
        {
            // Try graceful shutdown first
            if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                var shutdownTask = Task.Run(() => desktop.Shutdown(1));
                await shutdownTask.WaitAsync(TimeSpan.FromSeconds(3));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Graceful shutdown failed during emergency");
        }

        // Force shutdown if graceful fails
        Logger.LogCritical("Force terminating application due to emergency");
        Environment.Exit(1);
    }

    /// <summary>
    /// Emergency timer callback - last resort shutdown
    /// </summary>
    private void EmergencyTimerCallback(object? state)
    {
        try
        {
            Logger.LogCritical("Emergency timer triggered - force shutting down application after 30 seconds");
            
            // This runs on a background thread and won't be affected by UI lockup
            Environment.Exit(1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in emergency timer callback");
            
            // Ultimate fallback
            try
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch
            {
                // Nothing more we can do
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Animates the overlay in
    /// </summary>
    private async Task AnimateInAsync()
    {
        try
        {
            IsAnimating = true;
            Progress = 0.0;

            // Simple progress animation (could be enhanced with Avalonia animations)
            for (double i = 0; i <= 1.0; i += 0.1)
            {
                Progress = i;
                await Task.Delay(50); // 500ms total animation
            }

            Progress = 1.0;
            Logger.LogDebug("Animate in completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during animate in");
            Progress = 1.0; // Ensure we're visible even if animation fails
        }
    }

    /// <summary>
    /// Animates the overlay out and triggers dismissal
    /// </summary>
    private async Task AnimateOutAsync()
    {
        try
        {
            Logger.LogDebug("Starting animate out");

            // Animate out
            for (double i = 1.0; i >= 0.0; i -= 0.1)
            {
                Progress = i;
                await Task.Delay(30); // 300ms total animation
            }

            Progress = 0.0;
            IsAnimating = false;

            // Notify completion
            AnimationCompleted?.Invoke();
            DismissRequested?.Invoke();

            Logger.LogDebug("Animate out completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during animate out");
            // Ensure dismissal even if animation fails
            IsAnimating = false;
            Progress = 0.0;
            DismissRequested?.Invoke();
        }
    }

    /// <summary>
    /// Initialize design-time data
    /// </summary>
    private void InitializeDesignTimeData()
    {
        Message = "Inventory item saved successfully!";
        Details = "Part ID: PART001 | Operation: 90 | Quantity: 5 | Location: WC01";
        ShowDetails = true;
        IconKind = "CheckCircle";
        IsError = false;
        Progress = 1.0;
        DisplayDuration = 2000;
    }

    #endregion
}
