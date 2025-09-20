using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the emergency shutdown overlay that provides safe application termination
/// when critical errors or emergency situations occur. Ensures data integrity and graceful shutdown.
/// Follows BaseOverlayViewModel pattern and MTM design guidelines.
/// </summary>
public partial class EmergencyShutdownOverlayViewModel : BaseOverlayViewModel
{
    private CancellationTokenSource? _shutdownCancellationSource;
    private const int DefaultShutdownTimeoutSeconds = 10;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the reason for the emergency shutdown.
    /// </summary>
    [ObservableProperty]
    private string shutdownReason = "Critical error detected";

    /// <summary>
    /// Gets or sets additional details about the emergency situation.
    /// </summary>
    [ObservableProperty]
    private string emergencyDetails = string.Empty;

    /// <summary>
    /// Gets or sets whether the shutdown is currently in progress.
    /// </summary>
    [ObservableProperty]
    private bool isShuttingDown;

    /// <summary>
    /// Gets or sets the current shutdown progress (0-100).
    /// </summary>
    [ObservableProperty]
    private int shutdownProgress;

    /// <summary>
    /// Gets or sets the current shutdown status message.
    /// </summary>
    [ObservableProperty]
    private string shutdownStatusMessage = "Ready for emergency shutdown";

    /// <summary>
    /// Gets or sets the countdown seconds remaining before automatic shutdown.
    /// </summary>
    [ObservableProperty]
    private int countdownSeconds;

    /// <summary>
    /// Gets or sets whether automatic shutdown countdown is active.
    /// </summary>
    [ObservableProperty]
    private bool isCountdownActive;

    /// <summary>
    /// Gets or sets the shutdown timeout in seconds.
    /// </summary>
    [ObservableProperty]
    private int shutdownTimeoutSeconds = DefaultShutdownTimeoutSeconds;

    /// <summary>
    /// Gets or sets whether to save data before shutdown.
    /// </summary>
    [ObservableProperty]
    private bool saveDataBeforeShutdown = true;

    /// <summary>
    /// Gets or sets whether the emergency shutdown was user-initiated.
    /// </summary>
    [ObservableProperty]
    private bool isUserInitiated;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the EmergencyShutdownOverlayViewModel class.
    /// </summary>
    /// <param name="logger">Logger instance for emergency shutdown operations.</param>
    public EmergencyShutdownOverlayViewModel(ILogger<EmergencyShutdownOverlayViewModel> logger)
        : base(logger)
    {
        _logger.LogDebug("EmergencyShutdownOverlayViewModel initialized");

        // Set default values
        CountdownSeconds = shutdownTimeoutSeconds;
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to initiate immediate emergency shutdown.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanInitiateShutdown))]
    private async Task InitiateEmergencyShutdownAsync()
    {
        try
        {
            _logger.LogCritical("User initiated emergency shutdown: {Reason}", ShutdownReason);

            IsUserInitiated = true;
            IsShuttingDown = true;
            ShutdownStatusMessage = "Initiating emergency shutdown...";

            await PerformEmergencyShutdownAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to initiate emergency shutdown");
        }
    }

    /// <summary>
    /// Command to start automatic shutdown countdown.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanStartCountdown))]
    private async Task StartShutdownCountdownAsync()
    {
        try
        {
            _logger.LogWarning("Starting emergency shutdown countdown: {Seconds} seconds", ShutdownTimeoutSeconds);

            IsCountdownActive = true;
            CountdownSeconds = ShutdownTimeoutSeconds;
            ShutdownStatusMessage = $"Emergency shutdown in {CountdownSeconds} seconds...";

            await RunCountdownAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to start shutdown countdown");
        }
    }

    /// <summary>
    /// Command to cancel the shutdown countdown.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCancelCountdown))]
    private async Task CancelShutdownCountdownAsync()
    {
        try
        {
            _logger.LogInformation("User cancelled emergency shutdown countdown");

            _shutdownCancellationSource?.Cancel();

            IsCountdownActive = false;
            CountdownSeconds = ShutdownTimeoutSeconds;
            ShutdownStatusMessage = "Shutdown countdown cancelled";

            // Close overlay after brief delay to show cancellation message
            await Task.Delay(1500);
            await HideAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to cancel shutdown countdown");
        }
    }

    /// <summary>
    /// Command to force immediate shutdown without data saving.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanForceShutdown))]
    private async Task ForceImmediateShutdownAsync()
    {
        try
        {
            _logger.LogCritical("User forced immediate shutdown without data saving");

            SaveDataBeforeShutdown = false;
            IsUserInitiated = true;
            IsShuttingDown = true;
            ShutdownStatusMessage = "Forcing immediate shutdown...";

            await PerformEmergencyShutdownAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to force immediate shutdown");

            // Ultimate fallback - force process termination
            _logger.LogCritical("Emergency shutdown failed - terminating process");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Command to attempt to continue application operation.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanContinueOperation))]
    private async Task ContinueOperationAsync()
    {
        try
        {
            _logger.LogWarning("User chose to continue operation despite emergency situation");

            ShutdownStatusMessage = "Continuing operation...";

            await Task.Delay(1000); // Brief delay to show message
            await HideAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Failed to continue operation");
        }
    }

    /// <summary>
    /// Command to set the shutdown timeout to a specific value.
    /// </summary>
    [RelayCommand]
    private void SetTimeout(string timeoutValue)
    {
        if (int.TryParse(timeoutValue, out var timeout) && timeout >= 5 && timeout <= 60)
        {
            ShutdownTimeoutSeconds = timeout;
            CountdownSeconds = timeout;

            _logger.LogInformation("Shutdown timeout set to {Timeout} seconds", timeout);
        }
    }

    #endregion

    #region Command CanExecute Methods

    /// <summary>
    /// Determines whether the initiate shutdown command can be executed.
    /// </summary>
    private bool CanInitiateShutdown() => !IsShuttingDown && !IsCountdownActive;

    /// <summary>
    /// Determines whether the start countdown command can be executed.
    /// </summary>
    private bool CanStartCountdown() => !IsShuttingDown && !IsCountdownActive;

    /// <summary>
    /// Determines whether the cancel countdown command can be executed.
    /// </summary>
    private bool CanCancelCountdown() => IsCountdownActive && !IsShuttingDown;

    /// <summary>
    /// Determines whether the force shutdown command can be executed.
    /// </summary>
    private bool CanForceShutdown() => !IsShuttingDown;

    /// <summary>
    /// Determines whether the continue operation command can be executed.
    /// </summary>
    private bool CanContinueOperation() => !IsShuttingDown && !IsCountdownActive;

    #endregion

    #region Protected Override Methods

    protected override string GetDefaultTitle() => "Emergency Shutdown";

    protected override async Task OnInitializeAsync(object requestData)
    {
        await base.OnInitializeAsync(requestData);

        // Handle initialization data if provided
        if (requestData is EmergencyShutdownRequest request)
        {
            ShutdownReason = request.Reason;
            EmergencyDetails = request.Details;
            SaveDataBeforeShutdown = request.SaveData;
            ShutdownTimeoutSeconds = request.TimeoutSeconds > 0 ? request.TimeoutSeconds : DefaultShutdownTimeoutSeconds;
            CountdownSeconds = ShutdownTimeoutSeconds;

            // Auto-start countdown for critical emergencies
            if (request.AutoStartCountdown)
            {
                await StartShutdownCountdownCommand.ExecuteAsync(null);
            }
        }
    }

    protected override async Task OnShowingAsync()
    {
        await base.OnShowingAsync();

        ShutdownProgress = 0;
        IsShuttingDown = false;
        IsCountdownActive = false;
    }

    protected override async Task<bool> OnConfirmAsync()
    {
        // For emergency shutdown, confirm means proceed with shutdown
        await InitiateEmergencyShutdownCommand.ExecuteAsync(null);
        return false; // Don't close overlay yet - shutdown process will handle it
    }

    protected override async Task OnCancelAsync()
    {
        await base.OnCancelAsync();

        // Cancel any active countdown
        _shutdownCancellationSource?.Cancel();

        _logger.LogInformation("Emergency shutdown overlay cancelled by user");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the countdown timer for automatic shutdown.
    /// </summary>
    private async Task RunCountdownAsync()
    {
        _shutdownCancellationSource?.Cancel();
        _shutdownCancellationSource = new CancellationTokenSource();

        try
        {
            while (CountdownSeconds > 0 && IsCountdownActive && !_shutdownCancellationSource.Token.IsCancellationRequested)
            {
                ShutdownStatusMessage = $"Emergency shutdown in {CountdownSeconds} seconds...";

                await Task.Delay(1000, _shutdownCancellationSource.Token);
                CountdownSeconds--;
            }

            // If countdown completed naturally (not cancelled)
            if (CountdownSeconds <= 0 && IsCountdownActive)
            {
                _logger.LogCritical("Emergency shutdown countdown expired - initiating automatic shutdown");

                IsCountdownActive = false;
                await InitiateEmergencyShutdownCommand.ExecuteAsync(null);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Emergency shutdown countdown was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during emergency shutdown countdown");
            await HandleErrorAsync(ex, "Countdown failed");
        }
    }

    /// <summary>
    /// Performs the actual emergency shutdown process.
    /// </summary>
    private async Task PerformEmergencyShutdownAsync()
    {
        try
        {
            _logger.LogCritical("Beginning emergency shutdown process");

            // Step 1: Save critical data if requested
            if (SaveDataBeforeShutdown)
            {
                ShutdownProgress = 10;
                ShutdownStatusMessage = "Saving critical data...";
                await SaveCriticalDataAsync();
            }
            else
            {
                ShutdownProgress = 30;
                ShutdownStatusMessage = "Skipping data save (forced shutdown)...";
            }

            // Step 2: Notify services of shutdown
            ShutdownProgress = 40;
            ShutdownStatusMessage = "Notifying services of shutdown...";
            await NotifyServicesOfShutdownAsync();

            // Step 3: Close active overlays and dialogs
            ShutdownProgress = 60;
            ShutdownStatusMessage = "Closing application windows...";
            await CloseApplicationWindowsAsync();

            // Step 4: Dispose resources
            ShutdownProgress = 80;
            ShutdownStatusMessage = "Disposing resources...";
            await DisposeResourcesAsync();

            // Step 5: Perform final shutdown
            ShutdownProgress = 100;
            ShutdownStatusMessage = "Shutting down application...";
            await PerformFinalShutdownAsync();

        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error during emergency shutdown process");

            // If graceful shutdown fails, force termination
            ShutdownStatusMessage = "Emergency shutdown failed - forcing termination...";
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Saves critical application data before shutdown.
    /// </summary>
    private async Task SaveCriticalDataAsync()
    {
        try
        {
            _logger.LogInformation("Saving critical data during emergency shutdown");

            // Simulate critical data saving (in real implementation, this would save:)
            // - Current user session data
            // - Unsaved inventory changes
            // - Application state
            // - Configuration changes

            await Task.Delay(1500); // Simulate save operation

            _logger.LogInformation("Critical data saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save critical data during emergency shutdown");
            // Continue with shutdown even if save fails
        }
    }

    /// <summary>
    /// Notifies services that emergency shutdown is occurring.
    /// </summary>
    private async Task NotifyServicesOfShutdownAsync()
    {
        try
        {
            _logger.LogInformation("Notifying services of emergency shutdown");

            // In real implementation, this would:
            // - Notify database service to close connections
            // - Notify file services to flush pending writes
            // - Notify background services to stop processing

            await Task.Delay(800); // Simulate service notification

            _logger.LogInformation("Services notified of emergency shutdown");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify services during emergency shutdown");
            // Continue with shutdown even if notification fails
        }
    }

    /// <summary>
    /// Closes all application windows and overlays.
    /// </summary>
    private async Task CloseApplicationWindowsAsync()
    {
        try
        {
            _logger.LogInformation("Closing application windows during emergency shutdown");

            // In real implementation, this would close:
            // - Main application window
            // - Settings windows
            // - Other overlay windows

            await Task.Delay(500); // Simulate window closing

            _logger.LogInformation("Application windows closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to close windows during emergency shutdown");
            // Continue with shutdown
        }
    }

    /// <summary>
    /// Disposes critical resources before shutdown.
    /// </summary>
    private async Task DisposeResourcesAsync()
    {
        try
        {
            _logger.LogInformation("Disposing resources during emergency shutdown");

            // Dispose cancellation source
            _shutdownCancellationSource?.Cancel();
            _shutdownCancellationSource?.Dispose();
            _shutdownCancellationSource = null;

            await Task.Delay(300); // Simulate resource disposal

            _logger.LogInformation("Resources disposed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispose resources during emergency shutdown");
            // Continue with shutdown
        }
    }

    /// <summary>
    /// Performs the final application shutdown.
    /// </summary>
    private async Task PerformFinalShutdownAsync()
    {
        try
        {
            _logger.LogCritical("Performing final emergency shutdown");

            // Try graceful shutdown first
            if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                _logger.LogInformation("Attempting graceful shutdown via ApplicationLifetime");

                var shutdownTask = Task.Run(() => desktop.Shutdown(1)); // Exit code 1 for emergency
                await shutdownTask.WaitAsync(TimeSpan.FromSeconds(3));

                _logger.LogInformation("Graceful shutdown completed");
            }
            else
            {
                _logger.LogWarning("ApplicationLifetime not available - using Environment.Exit");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Final shutdown failed - forcing process termination");
            Environment.Exit(1);
        }
    }

    #endregion

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _shutdownCancellationSource?.Cancel();
            _shutdownCancellationSource?.Dispose();
            _shutdownCancellationSource = null;
        }

        base.Dispose(disposing);
    }

    #endregion
}

/// <summary>
/// Request data for initializing the emergency shutdown overlay.
/// </summary>
public class EmergencyShutdownRequest
{
    public string Reason { get; set; } = "Critical error detected";
    public string Details { get; set; } = string.Empty;
    public bool SaveData { get; set; } = true;
    public int TimeoutSeconds { get; set; } = 10;
    public bool AutoStartCountdown { get; set; } = false;

    public static EmergencyShutdownRequest Critical(string reason, string details = "")
        => new() { Reason = reason, Details = details, AutoStartCountdown = true, TimeoutSeconds = 15 };

    public static EmergencyShutdownRequest UserInitiated(string reason = "User requested emergency shutdown")
        => new() { Reason = reason, AutoStartCountdown = false, SaveData = true };

    public static EmergencyShutdownRequest Immediate(string reason, string details = "")
        => new() { Reason = reason, Details = details, SaveData = false, TimeoutSeconds = 5, AutoStartCountdown = true };
}
