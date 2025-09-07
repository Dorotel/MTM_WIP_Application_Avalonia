using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for the success overlay that displays temporary success messages
/// after successful transactions. Uses MVVM Community Toolkit patterns.
/// </summary>

public partial class SuccessOverlayViewModel : BaseViewModel
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

    #endregion

    #region Public Methods

    /// <summary>
    /// Shows the success overlay with the specified message
    /// </summary>
    /// <param name="message">The success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    public async Task ShowSuccessAsync(string message, string? details = null, string iconKind = "CheckCircle", int duration = 2000)
    {
        try
        {
            Logger.LogInformation("Showing success overlay: {Message}", message);

            // Set properties
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Details = details ?? string.Empty;
            ShowDetails = !string.IsNullOrEmpty(details);
            IconKind = iconKind;
            DisplayDuration = Math.Max(500, Math.Min(10000, duration)); // Clamp between 500ms and 10s

            // Start animations
            await AnimateInAsync();

            // Start auto-dismiss timer using Dispatcher for better UI thread handling
            _ = Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(DisplayDuration);
                if (IsAnimating) // Only dismiss if still showing
                {
                    await AnimateOutAsync();
                }
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing success overlay");
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
        Progress = 1.0;
        DisplayDuration = 2000;
    }

    #endregion
}
