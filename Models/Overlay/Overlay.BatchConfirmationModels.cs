using System.Collections.ObjectModel;

namespace MTM_WIP_Application_Avalonia.Models.Overlay
{
    /// <summary>
    /// Request model for batch confirmation overlays.
    /// </summary>
    public class BatchConfirmationRequest
    {
        /// <summary>
        /// Gets or sets the title of the batch confirmation dialog.
        /// </summary>
        public string Title { get; set; } = "Batch Operation Confirmation";

        /// <summary>
        /// Gets or sets the main message describing the batch operation.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of batch operation (e.g., "Remove", "Update", "Transfer").
        /// </summary>
        public string OperationType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of items that will be affected by the batch operation.
        /// </summary>
        public ObservableCollection<string> AffectedItems { get; set; } = new();

        /// <summary>
        /// Gets or sets the total count of items to be processed.
        /// </summary>
        public int ItemCount => AffectedItems.Count;

        /// <summary>
        /// Gets or sets the text for the confirm button.
        /// </summary>
        public string ConfirmText { get; set; } = "Proceed";

        /// <summary>
        /// Gets or sets the text for the cancel button.
        /// </summary>
        public string CancelText { get; set; } = "Cancel";

        /// <summary>
        /// Gets or sets the severity level of the batch operation.
        /// </summary>
        public ConfirmationSeverity Severity { get; set; } = ConfirmationSeverity.Warning;

        /// <summary>
        /// Gets or sets whether to show individual item details.
        /// </summary>
        public bool ShowItemDetails { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum number of items to display (others will be summarized).
        /// </summary>
        public int MaxDisplayItems { get; set; } = 10;
    }

    /// <summary>
    /// Response model for batch confirmation overlays.
    /// </summary>
    public class BatchConfirmationResponse
    {
        /// <summary>
        /// Gets or sets whether the user confirmed the batch operation.
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Gets or sets whether the user cancelled the batch operation.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets whether the user wants to proceed with individual confirmation for each item.
        /// </summary>
        public bool UseIndividualConfirmation { get; set; }

        /// <summary>
        /// Creates a confirmed batch response.
        /// </summary>
        public static BatchConfirmationResponse Confirmed(bool useIndividualConfirmation = false)
        {
            return new BatchConfirmationResponse
            {
                IsConfirmed = true,
                IsCancelled = false,
                UseIndividualConfirmation = useIndividualConfirmation
            };
        }

        /// <summary>
        /// Creates a cancelled batch response.
        /// </summary>
        public static BatchConfirmationResponse Cancelled()
        {
            return new BatchConfirmationResponse
            {
                IsConfirmed = false,
                IsCancelled = true,
                UseIndividualConfirmation = false
            };
        }
    }
}