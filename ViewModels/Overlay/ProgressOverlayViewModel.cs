using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for progress overlay displaying long-running operations.
/// Supports both determinate and indeterminate progress indicators.
/// </summary>
public partial class ProgressOverlayViewModel : BaseOverlayViewModel
{
    #region Observable Properties

    /// <summary>
    /// Progress percentage (0-100)
    /// </summary>
    [ObservableProperty]
    private double progressPercentage;

    /// <summary>
    /// Whether progress is indeterminate (unknown completion time)
    /// </summary>
    [ObservableProperty]
    private bool isIndeterminate = true;

    /// <summary>
    /// Status text for the current operation
    /// </summary>
    [ObservableProperty]
    private string statusText = "Processing...";

    /// <summary>
    /// Detailed status information
    /// </summary>
    [ObservableProperty]
    private string detailText = string.Empty;

    /// <summary>
    /// Whether the cancel button is visible
    /// </summary>
    [ObservableProperty]
    private bool canCancel = false;

    /// <summary>
    /// Whether the operation has completed
    /// </summary>
    [ObservableProperty]
    private bool isCompleted = false;

    /// <summary>
    /// Animation visibility for indeterminate progress
    /// </summary>
    [ObservableProperty]
    private bool showProgressAnimation = true;

    #endregion

    #region Events

    /// <summary>
    /// Event fired when user requests cancellation
    /// </summary>
    public event EventHandler? CancellationRequested;

    /// <summary>
    /// Event fired when progress is updated
    /// </summary>
    public event EventHandler<ProgressUpdatedEventArgs>? ProgressUpdated;

    #endregion

    #region Constructor

    public ProgressOverlayViewModel(ILogger<ProgressOverlayViewModel> logger)
        : base(logger)
    {
        Title = "Operation in Progress";
    }

    #endregion

    #region BaseOverlayViewModel Overrides

    protected override string GetDefaultTitle() => "Operation in Progress";

    protected override async Task<bool> OnConfirmAsync()
    {
        // For progress overlay, confirm means operation completed
        return true;
    }

    protected override async Task OnCancelAsync()
    {
        try
        {
            CancellationRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Progress cancellation failed");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start progress overlay for indeterminate operation
    /// </summary>
    public void StartIndeterminateProgress(string statusText, string title = "Processing", bool canCancel = false)
    {
        Title = title;
        StatusText = statusText;
        IsIndeterminate = true;
        CanCancel = canCancel;
        IsCompleted = false;
        ProgressPercentage = 0;
        ShowProgressAnimation = true;
        
        Show();
        Logger.LogInformation("Started indeterminate progress: {StatusText}", statusText);
    }

    /// <summary>
    /// Start progress overlay for determinate operation
    /// </summary>
    public void StartDeterminateProgress(string statusText, string title = "Processing", bool canCancel = false)
    {
        Title = title;
        StatusText = statusText;
        IsIndeterminate = false;
        CanCancel = canCancel;
        IsCompleted = false;
        ProgressPercentage = 0;
        ShowProgressAnimation = false;
        
        Show();
        Logger.LogInformation("Started determinate progress: {StatusText}", statusText);
    }

    /// <summary>
    /// Update progress with percentage and status
    /// </summary>
    public void UpdateProgress(double percentage, string? statusText = null, string? detailText = null)
    {
        try
        {
            ProgressPercentage = Math.Max(0, Math.Min(100, percentage));
            
            if (statusText != null)
                StatusText = statusText;
                
            if (detailText != null)
                DetailText = detailText;

            IsIndeterminate = false;
            ShowProgressAnimation = false;

            ProgressUpdated?.Invoke(this, new ProgressUpdatedEventArgs(ProgressPercentage, StatusText, DetailText));
            
            Logger.LogDebug("Progress updated: {Percentage}% - {StatusText}", ProgressPercentage, StatusText);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating progress");
        }
    }

    /// <summary>
    /// Complete the progress operation
    /// </summary>
    public async Task CompleteAsync(string? completionMessage = null)
    {
        try
        {
            IsCompleted = true;
            ProgressPercentage = 100;
            ShowProgressAnimation = false;
            
            if (completionMessage != null)
            {
                StatusText = completionMessage;
            }

            Logger.LogInformation("Progress completed: {StatusText}", StatusText);
            
            // Brief delay to show completion, then close
            await Task.Delay(1000);
            await CloseAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Progress completion failed");
        }
    }

    /// <summary>
    /// Fail the progress operation
    /// </summary>
    public async Task FailAsync(string errorMessage)
    {
        try
        {
            IsCompleted = true;
            StatusText = $"Failed: {errorMessage}";
            ShowProgressAnimation = false;

            Logger.LogWarning("Progress failed: {ErrorMessage}", errorMessage);
            
            // Brief delay to show error, then close
            await Task.Delay(2000);
            await CloseAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Progress failure handling failed");
        }
    }

    #endregion
}

/// <summary>
/// Event args for progress updates
/// </summary>
public class ProgressUpdatedEventArgs : EventArgs
{
    public double Percentage { get; }
    public string StatusText { get; }
    public string DetailText { get; }

    public ProgressUpdatedEventArgs(double percentage, string statusText, string detailText)
    {
        Percentage = percentage;
        StatusText = statusText;
        DetailText = detailText;
    }
}