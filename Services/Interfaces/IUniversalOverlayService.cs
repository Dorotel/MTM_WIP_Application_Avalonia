using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTM_WIP_Application_Avalonia.Services.Interfaces
{
    /// <summary>
    /// Universal Overlay Service interface for managing all overlay operations in the MTM application.
    /// Provides consistent overlay management with proper lifecycle events and memory pooling.
    /// </summary>
    public interface IUniversalOverlayService
    {
        /// <summary>
        /// Shows an overlay with the specified request data.
        /// </summary>
        /// <typeparam name="T">Type of the overlay request/configuration</typeparam>
        /// <param name="request">Overlay configuration and data</param>
        /// <returns>Overlay result with user response data</returns>
        Task<OverlayResult<T>> ShowOverlayAsync<T>(T request) where T : class;

        /// <summary>
        /// Hides the currently displayed overlay by its ID.
        /// </summary>
        /// <param name="overlayId">Unique identifier of the overlay to hide</param>
        /// <returns>Task representing the hide operation</returns>
        Task<bool> HideOverlayAsync(string overlayId);

        /// <summary>
        /// Registers an overlay ViewModel and View pair for dependency injection.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type for the overlay</typeparam>
        /// <typeparam name="TView">View type for the overlay</typeparam>
        void RegisterOverlay<TViewModel, TView>()
            where TViewModel : class
            where TView : class;

        /// <summary>
        /// Gets all currently active overlay IDs.
        /// </summary>
        /// <returns>Collection of active overlay identifiers</returns>
        IReadOnlyList<string> GetActiveOverlayIds();

        /// <summary>
        /// Event fired when an overlay is about to be shown.
        /// </summary>
        event EventHandler<OverlayEventArgs> OverlayShowing;

        /// <summary>
        /// Event fired when an overlay has been shown.
        /// </summary>
        event EventHandler<OverlayEventArgs> OverlayShown;

        /// <summary>
        /// Event fired when an overlay is about to be hidden.
        /// </summary>
        event EventHandler<OverlayEventArgs> OverlayHiding;

        /// <summary>
        /// Event fired when an overlay has been hidden.
        /// </summary>
        event EventHandler<OverlayEventArgs> OverlayHidden;
    }

    /// <summary>
    /// Result type for overlay operations containing response data and status.
    /// </summary>
    /// <typeparam name="T">Type of the overlay result data</typeparam>
    public class OverlayResult<T>
    {
        /// <summary>
        /// Gets whether the overlay operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets whether the overlay was cancelled by the user.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets the result data from the overlay operation.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets the overlay ID that generated this result.
        /// </summary>
        public string OverlayId { get; set; } = string.Empty;

        /// <summary>
        /// Gets any error message if the operation failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Creates a successful overlay result.
        /// </summary>
        public static OverlayResult<T> Success(T data, string overlayId = "")
        {
            return new OverlayResult<T>
            {
                IsSuccess = true,
                IsCancelled = false,
                Data = data,
                OverlayId = overlayId
            };
        }

        /// <summary>
        /// Creates a cancelled overlay result.
        /// </summary>
        public static OverlayResult<T> Cancelled(string overlayId = "")
        {
            return new OverlayResult<T>
            {
                IsSuccess = false,
                IsCancelled = true,
                OverlayId = overlayId
            };
        }

        /// <summary>
        /// Creates a failed overlay result.
        /// </summary>
        public static OverlayResult<T> Failed(string errorMessage, string overlayId = "")
        {
            return new OverlayResult<T>
            {
                IsSuccess = false,
                IsCancelled = false,
                ErrorMessage = errorMessage,
                OverlayId = overlayId
            };
        }
    }

    /// <summary>
    /// Event arguments for overlay lifecycle events.
    /// </summary>
    public class OverlayEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the unique identifier of the overlay.
        /// </summary>
        public string OverlayId { get; }

        /// <summary>
        /// Gets the type of the overlay ViewModel.
        /// </summary>
        public Type OverlayType { get; }

        /// <summary>
        /// Gets whether the event can be cancelled.
        /// </summary>
        public bool CanCancel { get; set; }

        /// <summary>
        /// Gets or sets whether the event should be cancelled.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Initializes a new instance of OverlayEventArgs.
        /// </summary>
        public OverlayEventArgs(string overlayId, Type overlayType, bool canCancel = false)
        {
            OverlayId = overlayId;
            OverlayType = overlayType;
            CanCancel = canCancel;
        }
    }
}