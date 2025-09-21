using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Feature;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay;

/// <summary>
/// Base class for all overlay ViewModels in the MTM WIP Application.
/// Provides common overlay functionality with MVVM Community Toolkit patterns.
/// </summary>
public abstract partial class BaseOverlayViewModel : ObservableObject, IOverlayViewModel, IDisposable
{
    protected readonly ILogger Logger;
    private bool _disposed = false;

    #region Observable Properties

    /// <summary>
    /// Gets or sets whether the overlay is currently visible.
    /// </summary>
    [ObservableProperty]
    private bool isVisible;

    /// <summary>
    /// Gets or sets the title displayed in the overlay header.
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets whether the overlay can be closed by the user.
    /// </summary>
    [ObservableProperty]
    private bool canClose = true;

    /// <summary>
    /// Gets or sets whether the overlay is modal (blocks interaction with parent).
    /// </summary>
    [ObservableProperty]
    private bool isModal = true;

    /// <summary>
    /// Gets or sets whether the overlay is currently processing an operation.
    /// </summary>
    [ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Gets or sets the current status message displayed to the user.
    /// </summary>
    [ObservableProperty]
    private string statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets any error message to display to the user.
    /// </summary>
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>
    /// Gets or sets whether there is currently an error state.
    /// </summary>
    [ObservableProperty]
    private bool hasError;

    /// <summary>
    /// Gets or sets the unique identifier for this overlay instance.
    /// </summary>
    [ObservableProperty]
    private string overlayId = Guid.NewGuid().ToString();

    #endregion

    #region Construction

    protected BaseOverlayViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize overlay in hidden state
        IsVisible = false;
        Title = "Overlay"; // Use string literal to avoid virtual call in constructor
    }

    #endregion

    #region Commands

    /// <summary>
    /// Command to close the overlay with cancellation result.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanClose))]
    protected async Task CloseAsync()
    {
        try
        {
            await OnClosingAsync();
            await HideOverlayAsync();
            OnClosed(OverlayCloseReason.UserCancelled);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing overlay {OverlayId}", OverlayId);
            await HandleErrorAsync(ex, "Failed to close overlay");
        }
    }

    /// <summary>
    /// Command to confirm the overlay operation and close with success result.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    protected virtual async Task ConfirmAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            var result = await OnConfirmAsync();
            if (result)
            {
                await HideOverlayAsync();
                OnClosed(OverlayCloseReason.UserConfirmed);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error confirming overlay {OverlayId}", OverlayId);
            await HandleErrorAsync(ex, "Failed to confirm operation");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Command to cancel the overlay operation and close with cancellation result.
    /// </summary>
    [RelayCommand]
    protected virtual async Task CancelAsync()
    {
        try
        {
            await OnCancelAsync();
            await HideOverlayAsync();
            OnClosed(OverlayCloseReason.UserCancelled);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error cancelling overlay {OverlayId}", OverlayId);
            await HandleErrorAsync(ex, "Failed to cancel operation");
        }
    }

    #endregion

    #region Virtual Methods for Override

    /// <summary>
    /// Gets the default title for this overlay type.
    /// Override in derived classes to provide specific titles.
    /// </summary>
    protected virtual string GetDefaultTitle() => "Overlay";

    /// <summary>
    /// Called when the overlay is being initialized with request data.
    /// Override in derived classes to handle specific initialization logic.
    /// </summary>
    /// <param name="requestData">The request data for overlay initialization</param>
    protected virtual Task OnInitializeAsync(object requestData)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the overlay is being shown.
    /// Override in derived classes to perform setup logic.
    /// </summary>
    protected virtual Task OnShowingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the overlay has been shown and is visible.
    /// Override in derived classes to perform post-show logic.
    /// </summary>
    protected virtual Task OnShownAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the overlay is being closed.
    /// Override in derived classes to perform cleanup logic.
    /// </summary>
    protected virtual Task OnClosingAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the confirm action is executed.
    /// Override in derived classes to perform confirmation logic.
    /// </summary>
    /// <returns>True if confirmation succeeded and overlay should close, false otherwise</returns>
    protected virtual Task<bool> OnConfirmAsync()
    {
        return Task.FromResult(true);
    }

    /// <summary>
    /// Called when the cancel action is executed.
    /// Override in derived classes to perform cancellation logic.
    /// </summary>
    protected virtual Task OnCancelAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when the overlay has been closed.
    /// Override in derived classes to perform post-close logic.
    /// </summary>
    protected virtual void OnClosed(OverlayCloseReason reason)
    {
        Logger.LogInformation("Overlay {OverlayId} closed with reason: {Reason}", OverlayId, reason);
    }

    #endregion

    #region Command CanExecute Methods

    /// <summary>
    /// Determines whether the confirm command can be executed.
    /// Override in derived classes for specific business logic.
    /// </summary>
    protected virtual bool CanConfirm() => !IsLoading;

    #endregion

    #region Public Methods

    /// <summary>
    /// Shows the overlay (synchronous version).
    /// </summary>
    public void Show()
    {
        try
        {
            IsVisible = true;
            Logger.LogDebug("Overlay {OverlayId} shown (sync)", OverlayId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing overlay {OverlayId}", OverlayId);
        }
    }

    /// <summary>
    /// Shows buttons in the overlay.
    /// </summary>
    public void ShowButtons()
    {
        try
        {
            // Default implementation - derived classes can override
            CanClose = true;
            Logger.LogDebug("Buttons shown in overlay {OverlayId}", OverlayId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing buttons in overlay {OverlayId}", OverlayId);
        }
    }

    /// <summary>
    /// Shows the overlay asynchronously.
    /// </summary>
    public async Task ShowAsync()
    {
        try
        {
            await OnShowingAsync();
            IsVisible = true;
            await OnShownAsync();
            Logger.LogInformation("Overlay {OverlayId} shown", OverlayId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing overlay {OverlayId}", OverlayId);
            await HandleErrorAsync(ex, "Failed to show overlay");
        }
    }

    /// <summary>
    /// Hides the overlay asynchronously.
    /// </summary>
    public async Task HideAsync()
    {
        await HideOverlayAsync();
    }

    #endregion

    #region IOverlayViewModel Implementation

    public virtual async Task InitializeAsync(object requestData)
    {
        try
        {
            await OnInitializeAsync(requestData);
            Logger.LogInformation("Overlay {OverlayId} initialized with data type: {DataType}",
                OverlayId, requestData?.GetType().Name ?? "null");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing overlay {OverlayId}", OverlayId);
            await HandleErrorAsync(ex, "Failed to initialize overlay");
        }
    }

    #endregion

    #region Error Handling

    /// <summary>
    /// Handles errors that occur during overlay operations.
    /// </summary>
    protected async Task HandleErrorAsync(Exception ex, string context)
    {
        var errorMsg = $"{context}: {ex.Message}";
        ErrorMessage = errorMsg;
        HasError = true;

        try
        {
            // Use MTM centralized error handling
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, context);
        }
        catch (Exception loggingEx)
        {
            Logger.LogError(loggingEx, "Failed to log error for overlay {OverlayId}", OverlayId);
        }
    }

    /// <summary>
    /// Clears any current error state.
    /// </summary>
    protected void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    #endregion

    #region Private Methods

    private async Task HideOverlayAsync()
    {
        try
        {
            await OnClosingAsync();
            IsVisible = false;
            Logger.LogInformation("Overlay {OverlayId} hidden", OverlayId);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error hiding overlay {OverlayId}", OverlayId);
        }
    }

    #endregion

    #region IDisposable Implementation

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Dispose managed resources
            try
            {
                if (IsVisible)
                {
                    IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during overlay disposal for {OverlayId}", OverlayId);
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}

/// <summary>
/// Enum representing the reason an overlay was closed.
/// </summary>
public enum OverlayCloseReason
{
    /// <summary>
    /// Overlay was closed for an unknown reason.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// User confirmed the overlay operation.
    /// </summary>
    UserConfirmed = 1,

    /// <summary>
    /// User cancelled the overlay operation.
    /// </summary>
    UserCancelled = 2,

    /// <summary>
    /// Overlay was closed automatically (timeout, etc.).
    /// </summary>
    AutoClose = 3,

    /// <summary>
    /// Overlay was closed programmatically.
    /// </summary>
    ProgrammaticClose = 4,

    /// <summary>
    /// Overlay was closed due to an error.
    /// </summary>
    Error = 5
}
