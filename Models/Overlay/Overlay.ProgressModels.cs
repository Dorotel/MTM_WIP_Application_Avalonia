using System;
using System.Threading;

namespace MTM_WIP_Application_Avalonia.Models.Overlay
{
    /// <summary>
    /// Request model for progress overlays.
    /// </summary>
    public class ProgressRequest
    {
        /// <summary>
        /// Gets or sets the title of the progress dialog.
        /// </summary>
        public string Title { get; set; } = "Operation in Progress";

        /// <summary>
        /// Gets or sets the main message describing the operation.
        /// </summary>
        public string Message { get; set; } = "Please wait...";

        /// <summary>
        /// Gets or sets the current task description.
        /// </summary>
        public string CurrentTask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the progress percentage (0-100). Set to -1 for indeterminate progress.
        /// </summary>
        public double ProgressPercentage { get; set; } = -1;

        /// <summary>
        /// Gets or sets whether the progress is indeterminate (unknown duration).
        /// </summary>
        public bool IsIndeterminate => ProgressPercentage < 0;

        /// <summary>
        /// Gets or sets whether the operation can be cancelled.
        /// </summary>
        public bool CanCancel { get; set; } = false;

        /// <summary>
        /// Gets or sets the cancellation token for the operation.
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }

        /// <summary>
        /// Gets or sets whether to show estimated time remaining.
        /// </summary>
        public bool ShowTimeEstimate { get; set; } = false;

        /// <summary>
        /// Gets or sets the estimated time remaining.
        /// </summary>
        public TimeSpan? EstimatedTimeRemaining { get; set; }

        /// <summary>
        /// Gets or sets the operation start time.
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets the elapsed time since the operation started.
        /// </summary>
        public TimeSpan ElapsedTime => DateTime.Now - StartTime;
    }

    /// <summary>
    /// Response model for progress overlays.
    /// </summary>
    public class ProgressResponse
    {
        /// <summary>
        /// Gets or sets whether the operation was completed successfully.
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets whether the operation was cancelled by the user.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets whether the operation failed with an error.
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// Gets or sets the error message if the operation failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the final progress percentage when completed.
        /// </summary>
        public double FinalProgress { get; set; } = 100;

        /// <summary>
        /// Gets or sets the total duration of the operation.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Creates a completed progress response.
        /// </summary>
        public static ProgressResponse Completed(TimeSpan duration)
        {
            return new ProgressResponse
            {
                IsCompleted = true,
                IsCancelled = false,
                HasError = false,
                FinalProgress = 100,
                Duration = duration
            };
        }

        /// <summary>
        /// Creates a cancelled progress response.
        /// </summary>
        public static ProgressResponse Cancelled(TimeSpan duration)
        {
            return new ProgressResponse
            {
                IsCompleted = false,
                IsCancelled = true,
                HasError = false,
                Duration = duration
            };
        }

        /// <summary>
        /// Creates an error progress response.
        /// </summary>
        public static ProgressResponse Error(string errorMessage, TimeSpan duration)
        {
            return new ProgressResponse
            {
                IsCompleted = false,
                IsCancelled = false,
                HasError = true,
                ErrorMessage = errorMessage,
                Duration = duration
            };
        }
    }

    /// <summary>
    /// Event args for progress update notifications.
    /// </summary>
    public class ProgressUpdateEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current progress percentage (0-100).
        /// </summary>
        public double ProgressPercentage { get; set; }

        /// <summary>
        /// Gets or sets the current task description.
        /// </summary>
        public string CurrentTask { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the estimated time remaining.
        /// </summary>
        public TimeSpan? EstimatedTimeRemaining { get; set; }

        /// <summary>
        /// Gets or sets whether the operation is complete.
        /// </summary>
        public bool IsComplete { get; set; }
    }
}