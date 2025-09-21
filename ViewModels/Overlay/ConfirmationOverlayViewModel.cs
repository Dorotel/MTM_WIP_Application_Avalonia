using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for confirmation and error message overlay.
/// Supports both confirmation dialogs and error message display.
/// Now inherits from BaseOverlayViewModel for consistent overlay behavior.
/// </summary>
public partial class ConfirmationOverlayViewModel : BaseOverlayViewModel
{
    #region Observable Properties

    /// <summary>
    /// Main message text
    /// </summary>
    [ObservableProperty]
    private string message = string.Empty;

    /// <summary>
    /// Detailed message or error details
    /// </summary>
    [ObservableProperty]
    private string? details;

    /// <summary>
    /// Type of overlay to determine styling and buttons
    /// </summary>
    [ObservableProperty]
    private OverlayType overlayType = OverlayType.Confirmation;

    /// <summary>
    /// Text for the primary action button
    /// </summary>
    [ObservableProperty]
    private string primaryButtonText = "Confirm";

    /// <summary>
    /// Text for the secondary action button
    /// </summary>
    [ObservableProperty]
    private string secondaryButtonText = "Cancel";

    /// <summary>
    /// Whether to show the secondary button
    /// </summary>
    [ObservableProperty]
    private bool showSecondaryButton = true;

    /// <summary>
    /// Icon kind for the overlay header
    /// </summary>
    [ObservableProperty]
    private string iconKind = "QuestionMark";

    #endregion

    #region Events

    /// <summary>
    /// Event fired when user confirms the action
    /// </summary>
    public event EventHandler? Confirmed;

    /// <summary>
    /// Event fired when user cancels the action
    /// </summary>
    public event EventHandler? Cancelled;

    #endregion

    #region Constructor

    public ConfirmationOverlayViewModel(ILogger<ConfirmationOverlayViewModel> logger)
        : base(logger)
    {
        // Override default title
        Title = "Confirm Action";
    }

    #endregion

    #region BaseOverlayViewModel Overrides

    protected override string GetDefaultTitle() => "Confirm Action";

    protected override async Task<bool> OnConfirmAsync()
    {
        try
        {
            Confirmed?.Invoke(this, EventArgs.Empty);
            return true; // Close overlay after confirmation
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Confirmation action failed");
            return false; // Don't close overlay on error
        }
    }

    protected override async Task OnCancelAsync()
    {
        try
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Cancellation action failed");
        }
    }

    protected override void OnClosed(OverlayCloseReason reason)
    {
        base.OnClosed(reason);
        
        // Fire appropriate events based on close reason
        switch (reason)
        {
            case OverlayCloseReason.UserConfirmed:
                // Already handled in OnConfirmAsync
                break;
            case OverlayCloseReason.UserCancelled:
                // Already handled in OnCancelAsync  
                break;
            default:
                // For other close reasons, treat as cancellation
                Cancelled?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Show confirmation dialog for deletion
    /// </summary>
    public void ShowDeleteConfirmation(string itemDescription)
    {
        Title = "Confirm Deletion";
        Message = $"Are you sure you want to delete {itemDescription}?";
        Details = "This action cannot be undone.";
        OverlayType = OverlayType.Warning;
        PrimaryButtonText = "Delete";
        SecondaryButtonText = "Cancel";
        ShowSecondaryButton = true;
        IconKind = "Delete";
        Show();

        Logger.LogInformation("Showing delete confirmation for: {ItemDescription}", itemDescription);
    }

    /// <summary>
    /// Show error message dialog
    /// </summary>
    public void ShowError(string title, string message, string? details = null)
    {
        Title = title;
        Message = message;
        Details = details;
        OverlayType = OverlayType.Error;
        PrimaryButtonText = "OK";
        SecondaryButtonText = string.Empty;
        ShowSecondaryButton = false;
        IconKind = "AlertCircle";
        Show();

        Logger.LogWarning("Showing error dialog: {Title} - {Message}", title, message);
    }

    /// <summary>
    /// Show success message dialog
    /// </summary>
    public void ShowSuccess(string title, string message, string? details = null)
    {
        Title = title;
        Message = message;
        Details = details;
        OverlayType = OverlayType.Success;
        PrimaryButtonText = "OK";
        SecondaryButtonText = string.Empty;
        ShowSecondaryButton = false;
        IconKind = "CheckCircle";
        Show();

        Logger.LogInformation("Showing success dialog: {Title} - {Message}", title, message);
    }

    /// <summary>
    /// Show warning message dialog
    /// </summary>
    public void ShowWarning(string title, string message, string? details = null)
    {
        Title = title;
        Message = message;
        Details = details;
        OverlayType = OverlayType.Warning;
        PrimaryButtonText = "OK";
        SecondaryButtonText = string.Empty;
        ShowSecondaryButton = false;
        IconKind = "Warning";
        Show();

        Logger.LogWarning("Showing warning dialog: {Title} - {Message}", title, message);
    }

    /// <summary>
    /// Show custom confirmation dialog
    /// </summary>
    public void ShowConfirmation(string title, string message, string primaryText = "Confirm", string secondaryText = "Cancel", string? details = null)
    {
        Title = title;
        Message = message;
        Details = details;
        OverlayType = OverlayType.Confirmation;
        PrimaryButtonText = primaryText;
        SecondaryButtonText = secondaryText;
        ShowSecondaryButton = true;
        IconKind = "QuestionMark";
        Show();

        Logger.LogInformation("Showing confirmation dialog: {Title} - {Message}", title, message);
    }

    #endregion
}

/// <summary>
/// Types of overlay dialogs
/// </summary>
public enum OverlayType
{
    Confirmation,
    Error,
    Warning,
    Success,
    Information
}
