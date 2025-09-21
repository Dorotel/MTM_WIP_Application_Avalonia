using System.Collections.ObjectModel;

namespace MTM_WIP_Application_Avalonia.Models.Overlay
{
    /// <summary>
    /// Request model for field validation overlays.
    /// </summary>
    public class ValidationRequest
    {
        /// <summary>
        /// Gets or sets the title of the validation dialog.
        /// </summary>
        public string Title { get; set; } = "Validation Errors";

        /// <summary>
        /// Gets or sets the main message describing the validation issues.
        /// </summary>
        public string Message { get; set; } = "Please correct the following errors:";

        /// <summary>
        /// Gets or sets the collection of validation errors.
        /// </summary>
        public ObservableCollection<ValidationError> ValidationErrors { get; set; } = new();

        /// <summary>
        /// Gets or sets whether to automatically hide the overlay after successful validation.
        /// </summary>
        public bool AutoHideOnSuccess { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to enable focus management to invalid fields.
        /// </summary>
        public bool EnableFocusManagement { get; set; } = true;

        /// <summary>
        /// Gets or sets the text for the OK button.
        /// </summary>
        public string OkText { get; set; } = "OK";

        /// <summary>
        /// Gets or sets the text for the cancel button (if cancellation is allowed).
        /// </summary>
        public string CancelText { get; set; } = "Cancel";

        /// <summary>
        /// Gets or sets whether the validation dialog can be cancelled.
        /// </summary>
        public bool CanCancel { get; set; } = true;
    }

    /// <summary>
    /// Response model for validation overlays.
    /// </summary>
    public class ValidationResponse
    {
        /// <summary>
        /// Gets or sets whether the user acknowledged the validation errors.
        /// </summary>
        public bool IsAcknowledged { get; set; }

        /// <summary>
        /// Gets or sets whether the user cancelled the validation dialog.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets whether the user requested to focus on the first error.
        /// </summary>
        public bool RequestFocusOnFirstError { get; set; }

        /// <summary>
        /// Creates an acknowledged validation response.
        /// </summary>
        public static ValidationResponse Acknowledged(bool requestFocus = false)
        {
            return new ValidationResponse
            {
                IsAcknowledged = true,
                IsCancelled = false,
                RequestFocusOnFirstError = requestFocus
            };
        }

        /// <summary>
        /// Creates a cancelled validation response.
        /// </summary>
        public static ValidationResponse Cancelled()
        {
            return new ValidationResponse
            {
                IsAcknowledged = false,
                IsCancelled = true,
                RequestFocusOnFirstError = false
            };
        }
    }

    /// <summary>
    /// Validation error details for field validation.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Gets or sets the field name that has the validation error.
        /// </summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the field (user-friendly name).
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current field value that caused the error.
        /// </summary>
        public object? CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets the severity of the validation error.
        /// </summary>
        public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;

        /// <summary>
        /// Gets or sets the error code (for programmatic handling).
        /// </summary>
        public string? ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets whether this error prevents form submission.
        /// </summary>
        public bool IsBlockingError { get; set; } = true;

        /// <summary>
        /// Gets or sets suggested fix or help text for the error.
        /// </summary>
        public string? SuggestedFix { get; set; }
    }

    /// <summary>
    /// Severity levels for validation errors.
    /// </summary>
    public enum ValidationSeverity
    {
        /// <summary>
        /// Informational message - does not prevent form submission.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Warning message - may prevent form submission depending on configuration.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error message - prevents form submission.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Critical error - prevents form submission and may require immediate attention.
        /// </summary>
        Critical = 3
    }
}