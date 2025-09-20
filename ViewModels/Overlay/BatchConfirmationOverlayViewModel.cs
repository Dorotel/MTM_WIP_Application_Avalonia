using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.Models.Overlay;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for batch confirmation overlays handling multiple item operations.
    /// </summary>
    [ObservableObject]
    public partial class BatchConfirmationOverlayViewModel : BaseViewModel
    {
        private TaskCompletionSource<BatchConfirmationResponse>? _completionSource;

        #region Observable Properties

        /// <summary>
        /// Gets or sets the title of the batch confirmation dialog.
        /// </summary>
        [ObservableProperty]
        private string title = "Batch Operation Confirmation";

        /// <summary>
        /// Gets or sets the main message describing the batch operation.
        /// </summary>
        [ObservableProperty]
        private string message = string.Empty;

        /// <summary>
        /// Gets or sets the type of batch operation.
        /// </summary>
        [ObservableProperty]
        private string operationType = string.Empty;

        /// <summary>
        /// Gets or sets the list of items that will be affected.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> affectedItems = new();

        /// <summary>
        /// Gets or sets the text for the confirm button.
        /// </summary>
        [ObservableProperty]
        private string confirmText = "Proceed";

        /// <summary>
        /// Gets or sets the text for the cancel button.
        /// </summary>
        [ObservableProperty]
        private string cancelText = "Cancel";

        /// <summary>
        /// Gets or sets the severity level of the batch operation.
        /// </summary>
        [ObservableProperty]
        private ConfirmationSeverity severity = ConfirmationSeverity.Warning;

        /// <summary>
        /// Gets or sets whether to show individual item details.
        /// </summary>
        [ObservableProperty]
        private bool showItemDetails = true;

        /// <summary>
        /// Gets or sets the maximum number of items to display.
        /// </summary>
        [ObservableProperty]
        private int maxDisplayItems = 10;

        /// <summary>
        /// Gets or sets whether the overlay is currently visible.
        /// </summary>
        [ObservableProperty]
        private bool isVisible;

        /// <summary>
        /// Gets or sets the current progress for batch operations.
        /// </summary>
        [ObservableProperty]
        private double progressPercentage;

        /// <summary>
        /// Gets or sets whether the batch operation is currently in progress.
        /// </summary>
        [ObservableProperty]
        private bool isOperationInProgress;

        /// <summary>
        /// Gets or sets the current processing item.
        /// </summary>
        [ObservableProperty]
        private string currentProcessingItem = string.Empty;

        /// <summary>
        /// Gets the total count of items to be processed.
        /// </summary>
        public int ItemCount => AffectedItems.Count;

        /// <summary>
        /// Gets the display items (limited by MaxDisplayItems).
        /// </summary>
        public ObservableCollection<string> DisplayItems => new(
            AffectedItems.Take(MaxDisplayItems));

        /// <summary>
        /// Gets whether there are additional items not shown.
        /// </summary>
        public bool HasAdditionalItems => AffectedItems.Count > MaxDisplayItems;

        /// <summary>
        /// Gets the count of additional items.
        /// </summary>
        public int AdditionalItemsCount => Math.Max(0, AffectedItems.Count - MaxDisplayItems);

        /// <summary>
        /// Gets the summary text for the batch operation.
        /// </summary>
        public string OperationSummary => 
            $"{OperationType} {ItemCount} item{(ItemCount != 1 ? "s" : "")}";

        #endregion

        #region Commands

        /// <summary>
        /// Command to proceed with the batch operation.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanProceedWithOperation))]
        private async Task ProceedAsync()
        {
            try
            {
                Logger?.LogInformation("Batch confirmation: User confirmed {OperationType} operation for {ItemCount} items", 
                    OperationType, ItemCount);

                var response = BatchConfirmationResponse.Confirmed();
                await CloseOverlayAsync(response);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error handling batch confirmation proceed action");
                var response = BatchConfirmationResponse.Confirmed();
                await CloseOverlayAsync(response);
            }
        }

        /// <summary>
        /// Command to proceed with individual confirmation for each item.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanProceedWithOperation))]
        private async Task ProceedWithIndividualConfirmationAsync()
        {
            try
            {
                Logger?.LogInformation("Batch confirmation: User requested individual confirmation for {ItemCount} items", ItemCount);

                var response = BatchConfirmationResponse.Confirmed(useIndividualConfirmation: true);
                await CloseOverlayAsync(response);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error handling batch individual confirmation action");
                var response = BatchConfirmationResponse.Confirmed(useIndividualConfirmation: true);
                await CloseOverlayAsync(response);
            }
        }

        /// <summary>
        /// Command to cancel the batch operation.
        /// </summary>
        [RelayCommand]
        private async Task CancelAsync()
        {
            try
            {
                Logger?.LogInformation("Batch confirmation: User cancelled {OperationType} operation for {ItemCount} items", 
                    OperationType, ItemCount);

                var response = BatchConfirmationResponse.Cancelled();
                await CloseOverlayAsync(response);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error handling batch confirmation cancel action");
                var response = BatchConfirmationResponse.Cancelled();
                await CloseOverlayAsync(response);
            }
        }

        /// <summary>
        /// Determines whether the operation can proceed.
        /// </summary>
        private bool CanProceedWithOperation => !IsOperationInProgress && AffectedItems.Count > 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BatchConfirmationOverlayViewModel class.
        /// </summary>
        /// <param name="logger">Logger instance for logging batch confirmation operations.</param>
        public BatchConfirmationOverlayViewModel(ILogger<BatchConfirmationOverlayViewModel> logger)
            : base(logger)
        {
            Logger?.LogDebug("BatchConfirmationOverlayViewModel initialized");

            // Subscribe to collection changes to update computed properties
            AffectedItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(ItemCount));
                OnPropertyChanged(nameof(DisplayItems));
                OnPropertyChanged(nameof(HasAdditionalItems));
                OnPropertyChanged(nameof(AdditionalItemsCount));
                OnPropertyChanged(nameof(OperationSummary));
                
                // Update command states
                ProceedCommand.NotifyCanExecuteChanged();
                ProceedWithIndividualConfirmationCommand.NotifyCanExecuteChanged();
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the batch confirmation overlay with the specified request parameters.
        /// </summary>
        /// <param name="request">The batch confirmation request with display parameters.</param>
        /// <returns>A task that completes with the user's batch confirmation response.</returns>
        public Task<BatchConfirmationResponse> ShowAsync(BatchConfirmationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            Logger?.LogDebug("Showing batch confirmation overlay: Operation='{OperationType}', ItemCount={ItemCount}", 
                request.OperationType, request.ItemCount);

            // Set properties from request
            Title = request.Title;
            Message = request.Message;
            OperationType = request.OperationType;
            ConfirmText = request.ConfirmText;
            CancelText = request.CancelText;
            Severity = request.Severity;
            ShowItemDetails = request.ShowItemDetails;
            MaxDisplayItems = request.MaxDisplayItems;

            // Update affected items collection
            AffectedItems.Clear();
            foreach (var item in request.AffectedItems)
            {
                AffectedItems.Add(item);
            }

            // Create completion source for async operation
            _completionSource = new TaskCompletionSource<BatchConfirmationResponse>();

            // Show the overlay
            IsVisible = true;

            return _completionSource.Task;
        }

        /// <summary>
        /// Updates the progress during batch operation execution.
        /// </summary>
        /// <param name="percentage">Progress percentage (0-100).</param>
        /// <param name="currentItem">Currently processing item.</param>
        public void UpdateProgress(double percentage, string currentItem = "")
        {
            ProgressPercentage = Math.Max(0, Math.Min(100, percentage));
            CurrentProcessingItem = currentItem;
            IsOperationInProgress = percentage < 100;

            Logger?.LogDebug("Batch operation progress updated: {Percentage}%, Current item: {CurrentItem}", 
                percentage, currentItem);
        }

        /// <summary>
        /// Closes the overlay with the specified response.
        /// </summary>
        /// <param name="response">The batch confirmation response.</param>
        private async Task CloseOverlayAsync(BatchConfirmationResponse response)
        {
            try
            {
                IsVisible = false;
                IsOperationInProgress = false;
                _completionSource?.SetResult(response);

                Logger?.LogDebug("Batch confirmation overlay closed: IsConfirmed={IsConfirmed}, UseIndividualConfirmation={UseIndividualConfirmation}", 
                    response.IsConfirmed, response.UseIndividualConfirmation);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error closing batch confirmation overlay");
                _completionSource?.SetException(ex);
            }
            
            await Task.CompletedTask;
        }

        #endregion

        #region Property Change Handlers

        /// <summary>
        /// Handles changes to operation type to update summary display.
        /// </summary>
        partial void OnOperationTypeChanged(string value)
        {
            OnPropertyChanged(nameof(OperationSummary));
        }

        /// <summary>
        /// Handles changes to operation progress state.
        /// </summary>
        partial void OnIsOperationInProgressChanged(bool value)
        {
            ProceedCommand.NotifyCanExecuteChanged();
            ProceedWithIndividualConfirmationCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}