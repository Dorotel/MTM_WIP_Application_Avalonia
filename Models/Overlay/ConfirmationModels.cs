using System.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Models.Overlay
{
    /// <summary>
    /// Request model for confirmation overlays with configurable severity and messages.
    /// </summary>
    public class ConfirmationRequest
    {
        /// <summary>
        /// Gets or sets the title of the confirmation dialog.
        /// </summary>
        public string Title { get; set; } = "Confirmation";

        /// <summary>
        /// Gets or sets the main message to display.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the text for the confirm button.
        /// </summary>
        public string ConfirmText { get; set; } = "Confirm";

        /// <summary>
        /// Gets or sets the text for the cancel button.
        /// </summary>
        public string CancelText { get; set; } = "Cancel";

        /// <summary>
        /// Gets or sets the severity level of the confirmation.
        /// </summary>
        public ConfirmationSeverity Severity { get; set; } = ConfirmationSeverity.Info;

        /// <summary>
        /// Gets or sets whether the confirmation can be cancelled.
        /// </summary>
        public bool CanCancel { get; set; } = true;

        /// <summary>
        /// Gets or sets the default button (Confirm or Cancel).
        /// </summary>
        public ConfirmationButton DefaultButton { get; set; } = ConfirmationButton.Cancel;
    }

    /// <summary>
    /// Response model for confirmation overlays.
    /// </summary>
    public class ConfirmationResponse
    {
        /// <summary>
        /// Gets or sets whether the user confirmed the action.
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Gets or sets whether the user cancelled the action.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets the button that was clicked.
        /// </summary>
        public ConfirmationButton ClickedButton { get; set; }

        /// <summary>
        /// Creates a confirmed response.
        /// </summary>
        public static ConfirmationResponse Confirmed()
        {
            return new ConfirmationResponse
            {
                IsConfirmed = true,
                IsCancelled = false,
                ClickedButton = ConfirmationButton.Confirm
            };
        }

        /// <summary>
        /// Creates a cancelled response.
        /// </summary>
        public static ConfirmationResponse Cancelled()
        {
            return new ConfirmationResponse
            {
                IsConfirmed = false,
                IsCancelled = true,
                ClickedButton = ConfirmationButton.Cancel
            };
        }
    }

    /// <summary>
    /// Severity levels for confirmation overlays.
    /// </summary>
    public enum ConfirmationSeverity
    {
        /// <summary>
        /// Informational confirmation - blue color scheme.
        /// </summary>
        [Description("Information")]
        Info = 0,

        /// <summary>
        /// Warning confirmation - yellow/amber color scheme.
        /// </summary>
        [Description("Warning")]
        Warning = 1,

        /// <summary>
        /// Error confirmation - red color scheme.
        /// </summary>
        [Description("Error")]
        Error = 2,

        /// <summary>
        /// Critical confirmation - dark red color scheme with strong emphasis.
        /// </summary>
        [Description("Critical")]
        Critical = 3
    }

    /// <summary>
    /// Button types for confirmation overlays.
    /// </summary>
    public enum ConfirmationButton
    {
        /// <summary>
        /// Confirm button was clicked.
        /// </summary>
        Confirm = 0,

        /// <summary>
        /// Cancel button was clicked.
        /// </summary>
        Cancel = 1
    }
}