using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// ViewModel for confirmation and error message overlay.
/// Supports both confirmation dialogs and error message display.
/// </summary>
public partial class ConfirmationOverlayViewModel : BaseViewModel
{
    #region Observable Properties

    /// <summary>
    /// Title text for the overlay
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

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

    /// <summary>
    /// Whether the overlay is currently visible
    /// </summary>
    [ObservableProperty]
    private bool isVisible;

    /// <summary>
    /// Whether the overlay is in loading state
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

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

    /// <summary>
    /// Event fired when overlay is closed
    /// </summary>
    public event EventHandler? Closed;

    #endregion

    #region Commands

    /// <summary>
    /// Command to confirm the action
    /// </summary>
    [RelayCommand]
    private async Task ConfirmAsync()
    {
        try
        {
            IsLoading = true;
            Confirmed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during confirmation action");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Confirmation failed", Environment.UserName);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to cancel the action
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        try
        {
            IsVisible = false;
            Cancelled?.Invoke(this, EventArgs.Empty);
            Closed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during cancel action");
        }
    }

    /// <summary>
    /// Command to close the overlay
    /// </summary>
    [RelayCommand]
    private void Close()
    {
        try
        {
            IsVisible = false;
            Closed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during close action");
        }
    }

    #endregion

    #region Constructor

    public ConfirmationOverlayViewModel(ILogger<ConfirmationOverlayViewModel> logger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
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
        IsVisible = true;

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
        IsVisible = true;

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
        IsVisible = true;

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
        IsVisible = true;

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
        IsVisible = true;

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