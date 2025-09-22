using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Services;
using Material.Icons;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ProgressOverlayViewModel manages universal progress indication overlay
/// following MVVM Community Toolkit patterns. Supports determinate and
/// indeterminate progress with proper MTM theming and user interaction.
/// </summary>
public partial class ProgressOverlayViewModel : BaseViewModel
{
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed = false;

    #region Observable Properties

    /// <summary>
    /// Gets or sets the main title displayed in the progress overlay
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CompactStatusText))]
    private string title = "Processing...";

    /// <summary>
    /// Gets or sets the current status message
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    [NotifyPropertyChangedFor(nameof(CompactStatusText))]
    private string statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets detailed progress information
    /// </summary>
    [ObservableProperty]
    private string progressDetails = string.Empty;

    /// <summary>
    /// Gets or sets the current progress value (0-100 for percentage)
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PercentageText))]
    private double progressValue = 0;

    /// <summary>
    /// Gets or sets the minimum progress value
    /// </summary>
    [ObservableProperty]
    private double progressMinimum = 0;

    /// <summary>
    /// Gets or sets the maximum progress value
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PercentageText))]
    private double progressMaximum = 100;

    /// <summary>
    /// Gets or sets whether the operation is currently in progress
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCompleted))]
    [NotifyPropertyChangedFor(nameof(ShowCancelButton))]
    [NotifyPropertyChangedFor(nameof(ShowCloseButton))]
    [NotifyPropertyChangedFor(nameof(CurrentIconKind))]
    [NotifyPropertyChangedFor(nameof(CompactStatusText))]
    private bool isInProgress = true;

    /// <summary>
    /// Gets or sets whether the operation has completed successfully
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsInProgress))]
    [NotifyPropertyChangedFor(nameof(ShowCancelButton))]
    [NotifyPropertyChangedFor(nameof(ShowCloseButton))]
    [NotifyPropertyChangedFor(nameof(CurrentIconKind))]
    [NotifyPropertyChangedFor(nameof(CompactStatusText))]
    private bool isCompleted = false;

    /// <summary>
    /// Gets or sets whether an error has occurred
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowCancelButton))]
    [NotifyPropertyChangedFor(nameof(ShowCloseButton))]
    [NotifyPropertyChangedFor(nameof(CurrentIconKind))]
    [NotifyPropertyChangedFor(nameof(CompactStatusText))]
    private bool hasError = false;

    /// <summary>
    /// Gets or sets whether to show determinate progress (with percentage)
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowIndeterminateProgress))]
    [NotifyPropertyChangedFor(nameof(ShowPercentage))]
    private bool showDeterminateProgress = false;

    /// <summary>
    /// Gets or sets whether the operation can be cancelled
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowCancelButton))]
    private bool isCancellable = true;

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets whether to show indeterminate progress bar
    /// </summary>
    public bool ShowIndeterminateProgress => IsInProgress && !ShowDeterminateProgress;

    /// <summary>
    /// Gets whether to show percentage text
    /// </summary>
    public bool ShowPercentage => ShowDeterminateProgress && IsInProgress;

    /// <summary>
    /// Gets the formatted percentage text
    /// </summary>
    public string PercentageText
    {
        get
        {
            if (ProgressMaximum <= ProgressMinimum) return "0%";
            var percentage = ((ProgressValue - ProgressMinimum) / (ProgressMaximum - ProgressMinimum)) * 100;
            return $"{percentage:F0}%";
        }
    }

    /// <summary>
    /// Gets whether status message should be visible
    /// </summary>
    public bool HasStatusMessage => !string.IsNullOrWhiteSpace(StatusMessage);

    /// <summary>
    /// Gets whether progress details should be visible
    /// </summary>
    public bool HasProgressDetails => !string.IsNullOrWhiteSpace(ProgressDetails);

    /// <summary>
    /// Gets compact status text for embedded progress display
    /// </summary>
    public string CompactStatusText
    {
        get
        {
            if (!IsInProgress && !HasError && IsCompleted)
                return "Complete";
            if (HasError)
                return "Error";
            if (!string.IsNullOrWhiteSpace(StatusMessage))
            {
                // Truncate status message for compact display
                var maxLength = 20;
                return StatusMessage.Length > maxLength
                    ? StatusMessage.Substring(0, maxLength - 3) + "..."
                    : StatusMessage;
            }
            return Title.Length > 15 ? Title.Substring(0, 12) + "..." : Title;
        }
    }

    /// <summary>
    /// Gets whether to show the cancel button
    /// </summary>
    public bool ShowCancelButton => IsInProgress && IsCancellable && !HasError;

    /// <summary>
    /// Gets whether to show the close button
    /// </summary>
    public bool ShowCloseButton => !IsInProgress || HasError;

    /// <summary>
    /// Gets the current icon kind based on state
    /// </summary>
    public MaterialIconKind CurrentIconKind
    {
        get
        {
            if (HasError) return MaterialIconKind.AlertCircle;
            if (IsCompleted) return MaterialIconKind.CheckCircle;
            return MaterialIconKind.Loading; // Animated spinning icon for in-progress
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Event fired when the overlay should be dismissed
    /// </summary>
    public event Action? DismissRequested;

    /// <summary>
    /// Event fired when the operation should be cancelled
    /// </summary>
    public event Action<CancellationToken>? CancelRequested;

    #endregion

    #region Constructor

    public ProgressOverlayViewModel(ILogger<ProgressOverlayViewModel> logger) : base(logger)
    {
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to cancel the current operation
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanCancel))]
    private async Task CancelAsync()
    {
        try
        {
            Logger.LogInformation("User requested operation cancellation");

            _cancellationTokenSource?.Cancel();

            StatusMessage = "Cancelling...";
            IsCancellable = false; // Prevent multiple cancel requests

            // Fire cancellation event
            CancelRequested?.Invoke(_cancellationTokenSource?.Token ?? CancellationToken.None);

            // Wait a moment for graceful cancellation
            await Task.Delay(500);

            // If still in progress after delay, force dismiss
            if (IsInProgress)
            {
                Title = "Operation Cancelled";
                StatusMessage = "The operation was cancelled by user request";
                IsInProgress = false;
                HasError = false;
                IsCompleted = false; // Not completed, but cancelled

                Logger.LogInformation("Operation cancellation completed");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during operation cancellation");
            await ErrorHandling.HandleErrorAsync(ex, "Cancel operation", "System");
        }
    }

    private bool CanCancel => IsInProgress && IsCancellable && !HasError;

    /// <summary>
    /// Command to close/dismiss the overlay
    /// </summary>
    [RelayCommand]
    private void Close()
    {
        try
        {
            DismissRequested?.Invoke();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing progress overlay");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start a new progress operation
    /// </summary>
    public void StartProgress(string title, string? statusMessage = null, bool isDeterminate = false, bool cancellable = true)
    {
        try
        {
            Logger.LogInformation("Starting progress operation: {Title}", title);

            // Reset state
            Title = title;
            StatusMessage = statusMessage ?? string.Empty;
            ProgressDetails = string.Empty;
            ProgressValue = 0;
            ProgressMinimum = 0;
            ProgressMaximum = 100;
            ShowDeterminateProgress = isDeterminate;
            IsCancellable = cancellable;
            IsInProgress = true;
            IsCompleted = false;
            HasError = false;

            // Create new cancellation token
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = cancellable ? new CancellationTokenSource() : null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error starting progress operation");
        }
    }

    /// <summary>
    /// Update progress value and status
    /// </summary>
    public void UpdateProgress(double value, string? statusMessage = null, string? details = null)
    {
        try
        {
            ProgressValue = Math.Max(ProgressMinimum, Math.Min(ProgressMaximum, value));

            if (!string.IsNullOrWhiteSpace(statusMessage))
            {
                StatusMessage = statusMessage;
            }

            if (!string.IsNullOrWhiteSpace(details))
            {
                ProgressDetails = details;
            }

            Logger.LogDebug("Progress updated: {Progress}/{Max} - {Status}",
                ProgressValue, ProgressMaximum, StatusMessage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating progress");
        }
    }

    /// <summary>
    /// Complete the progress operation successfully
    /// </summary>
    public async Task CompleteAsync(string? completionMessage = null, int autoCloseDelayMs = 2000)
    {
        try
        {
            Logger.LogInformation("Progress operation completed successfully");

            IsInProgress = false;
            IsCompleted = true;
            HasError = false;
            ProgressValue = ProgressMaximum;

            Title = completionMessage ?? "Operation Completed";
            StatusMessage = "The operation finished successfully";
            ProgressDetails = string.Empty;

            // Auto-close after delay if specified
            if (autoCloseDelayMs > 0)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(autoCloseDelayMs);

                    if (IsCompleted && !_disposed)
                    {
                        // Ensure we're on the UI thread for the event
                        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                        {
                            DismissRequested?.Invoke();
                        });
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error completing progress operation");
            await ErrorHandling.HandleErrorAsync(ex, "Complete operation", "System");
        }
    }

    /// <summary>
    /// Mark the operation as failed with error
    /// </summary>
    public async Task SetErrorAsync(string errorMessage, string? details = null)
    {
        try
        {
            Logger.LogError("Progress operation failed: {Error}", errorMessage);

            IsInProgress = false;
            IsCompleted = false;
            HasError = true;

            Title = "Operation Failed";
            StatusMessage = errorMessage;
            ProgressDetails = details ?? string.Empty;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting progress error state");
            await ErrorHandling.HandleErrorAsync(ex, "Set error state", "System");
        }
    }

    /// <summary>
    /// Get the cancellation token for the current operation
    /// </summary>
    public CancellationToken GetCancellationToken()
    {
        return _cancellationTokenSource?.Token ?? CancellationToken.None;
    }

    #endregion

    #region IDisposable Implementation

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _disposed = true;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during ProgressOverlayViewModel disposal");
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}
