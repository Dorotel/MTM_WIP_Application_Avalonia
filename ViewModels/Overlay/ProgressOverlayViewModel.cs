using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for the Progress overlay providing operation progress tracking
    /// with progress bars, status messages, and cancellation support.
    /// </summary>
    public partial class ProgressOverlayViewModel : BaseOverlayViewModel
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly List<IProgress<ProgressInfo>> _progressReporters = new();

        [ObservableProperty]
        private string _operationName = string.Empty;

        [ObservableProperty]
        private string _operationDescription = string.Empty;

        [ObservableProperty]
        private double _progressPercentage;

        [ObservableProperty]
        private string _currentStatusMessage = "Initializing...";

        [ObservableProperty]
        private string _detailedStatusMessage = string.Empty;

        [ObservableProperty]
        private bool _isIndeterminate;

        [ObservableProperty]
        private bool _canCancel = true;

        [ObservableProperty]
        private bool _isCancellationRequested;

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private TimeSpan _elapsedTime;

        [ObservableProperty]
        private TimeSpan _estimatedTimeRemaining;

        [ObservableProperty]
        private string _currentStep = string.Empty;

        [ObservableProperty]
        private int _currentStepNumber;

        [ObservableProperty]
        private int _totalSteps;

        [ObservableProperty]
        private long _processedItems;

        [ObservableProperty]
        private long _totalItems;

        [ObservableProperty]
        private double _itemsPerSecond;

        [ObservableProperty]
        private bool _showDetailedProgress = true;

        [ObservableProperty]
        private bool _showStepProgress = true;

        [ObservableProperty]
        private bool _autoCloseOnCompletion;

        public ObservableCollection<ProgressStep> ProgressSteps { get; } = new();
        public ObservableCollection<string> StatusHistory { get; } = new();

        private DateTime _operationStartTime;
        private System.Timers.Timer? _elapsedTimeTimer;

        public ProgressOverlayViewModel(ILogger<ProgressOverlayViewModel> logger) : base(logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Title = "Operation Progress";
            IsModal = true;
            CanClose = false; // Prevent manual closing during operations

            InitializeTimers();
        }

        /// <summary>
        /// Initializes a new progress tracking operation
        /// </summary>
        public async Task InitializeProgressAsync(
            string operationName,
            string operationDescription,
            bool canCancel = true,
            bool isIndeterminate = false,
            int totalSteps = 0,
            long totalItems = 0)
        {
            try
            {
                OperationName = operationName;
                OperationDescription = operationDescription;
                CanCancel = canCancel;
                IsIndeterminate = isIndeterminate;
                TotalSteps = totalSteps;
                TotalItems = totalItems;

                // Reset state
                ProgressPercentage = 0;
                CurrentStatusMessage = "Initializing...";
                DetailedStatusMessage = string.Empty;
                IsCancellationRequested = false;
                IsCompleted = false;
                HasError = false;
                ErrorMessage = string.Empty;
                CurrentStepNumber = 0;
                ProcessedItems = 0;
                ItemsPerSecond = 0;

                // Clear collections
                ProgressSteps.Clear();
                StatusHistory.Clear();

                // Setup cancellation
                _cancellationTokenSource = new CancellationTokenSource();

                // Start timing
                _operationStartTime = DateTime.Now;
                _elapsedTimeTimer?.Start();

                // Initialize progress steps if provided
                if (totalSteps > 0)
                {
                    for (int i = 1; i <= totalSteps; i++)
                    {
                        ProgressSteps.Add(new ProgressStep
                        {
                            StepNumber = i,
                            StepName = $"Step {i}",
                            Status = ProgressStepStatus.Pending
                        });
                    }
                }

                AddStatusHistory($"Started: {operationName}");

                _logger.LogInformation("Progress tracking initialized for operation: {OperationName}", operationName);
                await ShowAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to initialize progress tracking");
            }
        }

        /// <summary>
        /// Updates the overall progress percentage
        /// </summary>
        public void UpdateProgress(double percentage, string statusMessage = "")
        {
            try
            {
                ProgressPercentage = Math.Max(0, Math.Min(100, percentage));

                if (!string.IsNullOrEmpty(statusMessage))
                {
                    CurrentStatusMessage = statusMessage;
                    AddStatusHistory(statusMessage);
                }

                // Calculate estimated time remaining
                if (ProgressPercentage > 0)
                {
                    var elapsed = DateTime.Now - _operationStartTime;
                    var totalEstimated = TimeSpan.FromMilliseconds(elapsed.TotalMilliseconds * (100.0 / ProgressPercentage));
                    EstimatedTimeRemaining = totalEstimated - elapsed;
                }

                _logger.LogDebug("Progress updated to {Percentage}%: {StatusMessage}", percentage, statusMessage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update progress");
            }
        }

        /// <summary>
        /// Updates the current step being processed
        /// </summary>
        public void UpdateCurrentStep(int stepNumber, string stepName = "", string statusMessage = "")
        {
            try
            {
                CurrentStepNumber = stepNumber;
                CurrentStep = !string.IsNullOrEmpty(stepName) ? stepName : $"Step {stepNumber}";

                // Update progress steps collection
                var step = ProgressSteps.FirstOrDefault(s => s.StepNumber == stepNumber);
                if (step != null)
                {
                    step.StepName = CurrentStep;
                    step.Status = ProgressStepStatus.InProgress;
                    step.StartTime = DateTime.Now;
                }

                // Mark previous steps as completed
                foreach (var prevStep in ProgressSteps.Where(s => s.StepNumber < stepNumber))
                {
                    if (prevStep.Status == ProgressStepStatus.InProgress)
                    {
                        prevStep.Status = ProgressStepStatus.Completed;
                        prevStep.EndTime = DateTime.Now;
                    }
                }

                if (!string.IsNullOrEmpty(statusMessage))
                {
                    CurrentStatusMessage = statusMessage;
                    AddStatusHistory($"Step {stepNumber}: {statusMessage}");
                }

                _logger.LogDebug("Current step updated to {StepNumber}: {StepName}", stepNumber, CurrentStep);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update current step");
            }
        }

        /// <summary>
        /// Updates item processing progress
        /// </summary>
        public void UpdateItemProgress(long processedItems, string statusMessage = "")
        {
            try
            {
                ProcessedItems = processedItems;

                // Calculate items per second
                var elapsed = DateTime.Now - _operationStartTime;
                if (elapsed.TotalSeconds > 0)
                {
                    ItemsPerSecond = ProcessedItems / elapsed.TotalSeconds;
                }

                // Update percentage if total items is known
                if (TotalItems > 0)
                {
                    var percentage = (double)ProcessedItems / TotalItems * 100;
                    UpdateProgress(percentage, statusMessage);
                }
                else if (!string.IsNullOrEmpty(statusMessage))
                {
                    CurrentStatusMessage = statusMessage;
                    AddStatusHistory(statusMessage);
                }

                _logger.LogDebug("Item progress updated: {ProcessedItems}/{TotalItems}", processedItems, TotalItems);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update item progress");
            }
        }

        /// <summary>
        /// Sets detailed status information
        /// </summary>
        public void SetDetailedStatus(string detailedMessage)
        {
            DetailedStatusMessage = detailedMessage;
            _logger.LogDebug("Detailed status updated: {DetailedMessage}", detailedMessage);
        }

        /// <summary>
        /// Marks the operation as completed successfully
        /// </summary>
        public async Task CompleteAsync(string completionMessage = "Operation completed successfully")
        {
            try
            {
                IsCompleted = true;
                CurrentStatusMessage = completionMessage;
                ProgressPercentage = 100;
                CanClose = true;

                // Mark all remaining steps as completed
                foreach (var step in ProgressSteps.Where(s => s.Status != ProgressStepStatus.Completed))
                {
                    step.Status = ProgressStepStatus.Completed;
                    step.EndTime = DateTime.Now;
                }

                AddStatusHistory($"Completed: {completionMessage}");

                _elapsedTimeTimer?.Stop();

                _logger.LogInformation("Operation completed: {CompletionMessage}", completionMessage);

                if (AutoCloseOnCompletion)
                {
                    await Task.Delay(2000); // Brief delay to show completion
                    await HideAsync();
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to complete operation");
            }
        }

        /// <summary>
        /// Marks the operation as failed with an error
        /// </summary>
        public async Task FailAsync(string errorMessage, Exception? exception = null)
        {
            try
            {
                HasError = true;
                ErrorMessage = errorMessage;
                CurrentStatusMessage = $"Error: {errorMessage}";
                CanClose = true;

                // Mark current step as failed
                var currentStep = ProgressSteps.FirstOrDefault(s => s.StepNumber == CurrentStepNumber);
                if (currentStep != null)
                {
                    currentStep.Status = ProgressStepStatus.Failed;
                    currentStep.EndTime = DateTime.Now;
                    currentStep.ErrorMessage = errorMessage;
                }

                AddStatusHistory($"Failed: {errorMessage}");

                _elapsedTimeTimer?.Stop();

                if (exception != null)
                {
                    await HandleErrorAsync(exception, "Operation failed");
                }
                else
                {
                    _logger.LogError("Operation failed: {ErrorMessage}", errorMessage);
                }
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to handle operation failure");
            }
        }

        /// <summary>
        /// Requests cancellation of the current operation
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCancelOperation))]
        public async Task RequestCancellationAsync()
        {
            try
            {
                if (!CanCancel || IsCancellationRequested) return;

                IsCancellationRequested = true;
                _cancellationTokenSource?.Cancel();

                CurrentStatusMessage = "Cancellation requested...";
                AddStatusHistory("Cancellation requested by user");

                _logger.LogInformation("Operation cancellation requested: {OperationName}", OperationName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to request cancellation");
            }
        }

        /// <summary>
        /// Handles operation cancellation completion
        /// </summary>
        public async Task HandleCancellationAsync(string cancellationMessage = "Operation was cancelled")
        {
            try
            {
                CurrentStatusMessage = cancellationMessage;
                CanClose = true;

                // Mark current step as cancelled
                var currentStep = ProgressSteps.FirstOrDefault(s => s.StepNumber == CurrentStepNumber);
                if (currentStep != null)
                {
                    currentStep.Status = ProgressStepStatus.Cancelled;
                    currentStep.EndTime = DateTime.Now;
                }

                AddStatusHistory($"Cancelled: {cancellationMessage}");

                _elapsedTimeTimer?.Stop();

                _logger.LogInformation("Operation cancelled: {CancellationMessage}", cancellationMessage);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to handle cancellation");
            }
        }

        /// <summary>
        /// Closes the progress overlay
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCloseOverlay))]
        public async Task CloseProgressAsync()
        {
            try
            {
                await HideAsync();
                _logger.LogInformation("Progress overlay closed for operation: {OperationName}", OperationName);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to close progress overlay");
            }
        }

        /// <summary>
        /// Toggles the detailed progress view
        /// </summary>
        [RelayCommand]
        public void ToggleDetailedView()
        {
            ShowDetailedProgress = !ShowDetailedProgress;
            _logger.LogDebug("Detailed progress view toggled to: {ShowDetailed}", ShowDetailedProgress);
        }

        /// <summary>
        /// Toggles the step progress view
        /// </summary>
        [RelayCommand]
        public void ToggleStepView()
        {
            ShowStepProgress = !ShowStepProgress;
            _logger.LogDebug("Step progress view toggled to: {ShowSteps}", ShowStepProgress);
        }

        /// <summary>
        /// Gets the cancellation token for the current operation
        /// </summary>
        public CancellationToken GetCancellationToken()
        {
            return _cancellationTokenSource?.Token ?? CancellationToken.None;
        }

        /// <summary>
        /// Creates a progress reporter for the current operation
        /// </summary>
        public IProgress<ProgressInfo> CreateProgressReporter()
        {
            var progress = new Progress<ProgressInfo>(info =>
            {
                if (info.Percentage.HasValue)
                {
                    UpdateProgress(info.Percentage.Value, info.StatusMessage);
                }

                if (info.CurrentStep.HasValue)
                {
                    UpdateCurrentStep(info.CurrentStep.Value, info.StepName, info.StatusMessage);
                }

                if (info.ProcessedItems.HasValue)
                {
                    UpdateItemProgress(info.ProcessedItems.Value, info.StatusMessage);
                }

                if (!string.IsNullOrEmpty(info.DetailedMessage))
                {
                    SetDetailedStatus(info.DetailedMessage);
                }
            });

            _progressReporters.Add(progress);
            return progress;
        }

        #region Command CanExecute Methods

        private bool CanCancelOperation() => CanCancel && !IsCancellationRequested && !IsCompleted;

        private bool CanCloseOverlay() => CanClose;

        #endregion

        #region Private Methods

        private void InitializeTimers()
        {
            _elapsedTimeTimer = new System.Timers.Timer(1000); // Update every second
            _elapsedTimeTimer.Elapsed += (_, _) =>
            {
                ElapsedTime = DateTime.Now - _operationStartTime;
            };
        }

        private void AddStatusHistory(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var historyEntry = $"[{timestamp}] {message}";

            StatusHistory.Insert(0, historyEntry);

            // Keep only the last 50 entries
            while (StatusHistory.Count > 50)
            {
                StatusHistory.RemoveAt(StatusHistory.Count - 1);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _elapsedTimeTimer?.Stop();
                _elapsedTimeTimer?.Dispose();
                _cancellationTokenSource?.Dispose();
                _progressReporters.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion
    }

    #region Supporting Models

    public partial class ProgressStep : ObservableObject
    {
        [ObservableProperty]
        private int stepNumber;

        [ObservableProperty]
        private string stepName = string.Empty;

        [ObservableProperty]
        private ProgressStepStatus status = ProgressStepStatus.Pending;

        [ObservableProperty]
        private DateTime? startTime;

        [ObservableProperty]
        private DateTime? endTime;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public TimeSpan? Duration => StartTime.HasValue && EndTime.HasValue
            ? EndTime.Value - StartTime.Value
            : null;
    }

    public class ProgressInfo
    {
        public double? Percentage { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
        public string DetailedMessage { get; set; } = string.Empty;
        public int? CurrentStep { get; set; }
        public string StepName { get; set; } = string.Empty;
        public long? ProcessedItems { get; set; }
    }

    public enum ProgressStepStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }

    #endregion
}
