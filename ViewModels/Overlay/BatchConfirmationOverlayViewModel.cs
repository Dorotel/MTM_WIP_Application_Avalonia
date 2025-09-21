using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.ViewModels.Overlay
{
    /// <summary>
    /// ViewModel for the Batch Confirmation overlay providing confirmation UI
    /// for batch operations with item lists, action summaries, and confirmation controls.
    /// </summary>
    public partial class BatchConfirmationOverlayViewModel : BaseOverlayViewModel
    {
        [ObservableProperty]
        private string _operationName = string.Empty;

        [ObservableProperty]
        private string _operationDescription = string.Empty;

        [ObservableProperty]
        private BatchOperationType _operationType = BatchOperationType.Unknown;

        [ObservableProperty]
        private string _confirmationMessage = string.Empty;

        [ObservableProperty]
        private string _warningMessage = string.Empty;

        [ObservableProperty]
        private bool _hasWarning;

        [ObservableProperty]
        private bool _requiresConfirmation = true;

        [ObservableProperty]
        private bool _canProceed = true;

        [ObservableProperty]
        private bool _showItemDetails = true;

        [ObservableProperty]
        private bool _showSummary = true;

        [ObservableProperty]
        private int _totalItemCount;

        [ObservableProperty]
        private int _selectedItemCount;

        [ObservableProperty]
        private string _batchId = string.Empty;

        [ObservableProperty]
        private DateTime _scheduledTime = DateTime.Now;

        [ObservableProperty]
        private bool _isScheduled;

        [ObservableProperty]
        private bool _backupBeforeOperation = true;

        [ObservableProperty]
        private bool _continueOnError;

        [ObservableProperty]
        private string _userConfirmationText = string.Empty;

        [ObservableProperty]
        private string _requiredConfirmationText = string.Empty;

        [ObservableProperty]
        private bool _isUserConfirmed;

        public ObservableCollection<BatchOperationItem> OperationItems { get; } = new();
        public ObservableCollection<BatchOperationWarning> Warnings { get; } = new();
        public ObservableCollection<BatchOperationSummary> OperationSummaries { get; } = new();

        public BatchConfirmationOverlayViewModel(ILogger<BatchConfirmationOverlayViewModel> logger) : base(logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            Title = "Batch Operation Confirmation";
            IsModal = true;

            // Watch for user confirmation text changes
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserConfirmationText))
                {
                    UpdateConfirmationState();
                }
            };
        }

        /// <summary>
        /// Initializes the batch confirmation overlay with operation details
        /// </summary>
        public async Task InitializeBatchConfirmationAsync(
            string operationName,
            string operationDescription,
            BatchOperationType operationType,
            IEnumerable<BatchOperationItem> items,
            string confirmationMessage = "",
            string warningMessage = "",
            string requiredConfirmationText = "")
        {
            try
            {
                OperationName = operationName;
                OperationDescription = operationDescription;
                OperationType = operationType;
                ConfirmationMessage = string.IsNullOrEmpty(confirmationMessage)
                    ? $"Are you sure you want to perform this {operationName.ToLower()} operation?"
                    : confirmationMessage;
                WarningMessage = warningMessage;
                HasWarning = !string.IsNullOrEmpty(warningMessage);
                RequiredConfirmationText = requiredConfirmationText;

                // Clear and populate items
                OperationItems.Clear();
                foreach (var item in items)
                {
                    OperationItems.Add(item);
                }

                TotalItemCount = OperationItems.Count;
                SelectedItemCount = OperationItems.Count(i => i.IsSelected);

                // Generate batch ID
                BatchId = $"BATCH_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

                // Analyze operation and generate warnings
                await AnalyzeOperationAsync();

                // Generate operation summaries
                GenerateOperationSummaries();

                // Update confirmation state
                UpdateConfirmationState();

                _logger.LogInformation("Batch confirmation initialized for operation: {OperationName} with {ItemCount} items",
                    operationName, TotalItemCount);

                await ShowAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to initialize batch confirmation");
            }
        }

        /// <summary>
        /// Confirms the batch operation and proceeds
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanConfirmOperation))]
        public async Task ConfirmOperationAsync()
        {
            try
            {
                if (!IsUserConfirmed)
                {
                    _logger.LogWarning("User attempted to confirm operation without proper confirmation");
                    return;
                }

                var confirmedItems = OperationItems.Where(i => i.IsSelected).ToList();

                var result = new BatchConfirmationResult
                {
                    IsConfirmed = true,
                    OperationName = OperationName,
                    OperationType = OperationType,
                    BatchId = BatchId,
                    ConfirmedItems = confirmedItems,
                    ScheduledTime = IsScheduled ? ScheduledTime : null,
                    BackupBeforeOperation = BackupBeforeOperation,
                    ContinueOnError = ContinueOnError,
                    OperationSummaries = OperationSummaries.ToList()
                };

                await HideAsync();

                _logger.LogInformation("Batch operation confirmed: {OperationName}, BatchId: {BatchId}, Items: {ItemCount}",
                    OperationName, BatchId, confirmedItems.Count);

                // Raise confirmation result event or return result
                OnBatchConfirmed?.Invoke(result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to confirm batch operation");
            }
        }

        /// <summary>
        /// Cancels the batch operation
        /// </summary>
        [RelayCommand]
        public async Task CancelOperationAsync()
        {
            try
            {
                var result = new BatchConfirmationResult
                {
                    IsConfirmed = false,
                    OperationName = OperationName,
                    OperationType = OperationType,
                    BatchId = BatchId
                };

                await HideAsync();

                _logger.LogInformation("Batch operation cancelled: {OperationName}, BatchId: {BatchId}", OperationName, BatchId);

                OnBatchCancelled?.Invoke(result);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, "Failed to cancel batch operation");
            }
        }

        /// <summary>
        /// Selects all items for the batch operation
        /// </summary>
        [RelayCommand]
        public void SelectAllItems()
        {
            try
            {
                foreach (var item in OperationItems)
                {
                    item.IsSelected = true;
                }

                SelectedItemCount = TotalItemCount;
                UpdateConfirmationState();

                _logger.LogDebug("Selected all {ItemCount} items for batch operation", TotalItemCount);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to select all items");
            }
        }

        /// <summary>
        /// Deselects all items for the batch operation
        /// </summary>
        [RelayCommand]
        public void SelectNoneItems()
        {
            try
            {
                foreach (var item in OperationItems)
                {
                    item.IsSelected = false;
                }

                SelectedItemCount = 0;
                UpdateConfirmationState();

                _logger.LogDebug("Deselected all items for batch operation");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deselect all items");
            }
        }

        /// <summary>
        /// Toggles the selection of an individual item
        /// </summary>
        [RelayCommand]
        public void ToggleItemSelection(BatchOperationItem item)
        {
            try
            {
                if (item == null) return;

                item.IsSelected = !item.IsSelected;
                SelectedItemCount = OperationItems.Count(i => i.IsSelected);
                UpdateConfirmationState();

                _logger.LogDebug("Toggled selection for item: {ItemId}", item.ItemId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to toggle item selection for item: {ItemId}", item.ItemId);
            }
        }

        /// <summary>
        /// Shows detailed information about an item
        /// </summary>
        [RelayCommand]
        public async Task ShowItemDetailsAsync(BatchOperationItem item)
        {
            try
            {
                if (item == null) return;

                // This could show a detailed view of the item
                // For now, just log the details
                _logger.LogInformation("Item details requested for: {ItemId} - {ItemName}", item.ItemId, item.ItemName);

                // You could implement a detailed item view overlay here
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, $"Failed to show details for item: {item.ItemId}");
            }
        }

        /// <summary>
        /// Toggles the item details view
        /// </summary>
        [RelayCommand]
        public void ToggleItemDetails()
        {
            ShowItemDetails = !ShowItemDetails;
            _logger.LogDebug("Item details view toggled to: {ShowDetails}", ShowItemDetails);
        }

        /// <summary>
        /// Toggles the operation summary view
        /// </summary>
        [RelayCommand]
        public void ToggleSummary()
        {
            ShowSummary = !ShowSummary;
            _logger.LogDebug("Operation summary view toggled to: {ShowSummary}", ShowSummary);
        }

        #region Command CanExecute Methods

        private bool CanConfirmOperation() => CanProceed && IsUserConfirmed && SelectedItemCount > 0;

        #endregion

        #region Private Methods

        private async Task AnalyzeOperationAsync()
        {
            await Task.Run(() =>
            {
                Warnings.Clear();

                // Check for potential issues based on operation type
                switch (OperationType)
                {
                    case BatchOperationType.Delete:
                        AnalyzeDeleteOperation();
                        break;
                    case BatchOperationType.Update:
                        AnalyzeUpdateOperation();
                        break;
                    case BatchOperationType.Transfer:
                        AnalyzeTransferOperation();
                        break;
                    case BatchOperationType.Process:
                        AnalyzeProcessOperation();
                        break;
                }

                // Check for general warnings
                if (TotalItemCount > 100)
                {
                    Warnings.Add(new BatchOperationWarning
                    {
                        Severity = WarningLevel.Warning,
                        Message = $"Large batch operation detected ({TotalItemCount} items). This may take considerable time to complete.",
                        Category = "Performance"
                    });
                }

                if (OperationItems.Any(i => i.HasIssues))
                {
                    var problemItems = OperationItems.Count(i => i.HasIssues);
                    Warnings.Add(new BatchOperationWarning
                    {
                        Severity = WarningLevel.Error,
                        Message = $"{problemItems} items have issues that may prevent successful operation completion.",
                        Category = "Data Quality"
                    });
                }

                HasWarning = Warnings.Any();
            });
        }

        private void AnalyzeDeleteOperation()
        {
            Warnings.Add(new BatchOperationWarning
            {
                Severity = WarningLevel.Critical,
                Message = "Delete operations cannot be undone. Ensure you have proper backups before proceeding.",
                Category = "Data Safety"
            });

            RequiredConfirmationText = "DELETE";
        }

        private void AnalyzeUpdateOperation()
        {
            if (TotalItemCount > 50)
            {
                Warnings.Add(new BatchOperationWarning
                {
                    Severity = WarningLevel.Warning,
                    Message = "Large update operations should be performed during low-usage periods to minimize system impact.",
                    Category = "System Performance"
                });
            }
        }

        private void AnalyzeTransferOperation()
        {
            var locationsInvolved = OperationItems.Select(i => i.CurrentLocation).Distinct().Count();
            if (locationsInvolved > 10)
            {
                Warnings.Add(new BatchOperationWarning
                {
                    Severity = WarningLevel.Warning,
                    Message = $"Transfer operation involves {locationsInvolved} different locations. Consider organizing transfers by location for efficiency.",
                    Category = "Operations"
                });
            }
        }

        private void AnalyzeProcessOperation()
        {
            var estimatedTime = OperationItems.Count * 2; // Assume 2 seconds per item
            if (estimatedTime > 300) // More than 5 minutes
            {
                Warnings.Add(new BatchOperationWarning
                {
                    Severity = WarningLevel.Information,
                    Message = $"Estimated processing time: {estimatedTime / 60:F1} minutes. Consider scheduling during off-peak hours.",
                    Category = "Scheduling"
                });
            }
        }

        private void GenerateOperationSummaries()
        {
            OperationSummaries.Clear();

            // Overall summary
            OperationSummaries.Add(new BatchOperationSummary
            {
                Category = "Overall",
                Description = "Total Items",
                Value = TotalItemCount.ToString("N0"),
                IsImportant = true
            });

            OperationSummaries.Add(new BatchOperationSummary
            {
                Category = "Overall",
                Description = "Selected Items",
                Value = SelectedItemCount.ToString("N0"),
                IsImportant = true
            });

            // Operation-specific summaries
            switch (OperationType)
            {
                case BatchOperationType.Delete:
                    GenerateDeleteSummary();
                    break;
                case BatchOperationType.Update:
                    GenerateUpdateSummary();
                    break;
                case BatchOperationType.Transfer:
                    GenerateTransferSummary();
                    break;
                case BatchOperationType.Process:
                    GenerateProcessSummary();
                    break;
            }

            // Location summary
            var locationGroups = OperationItems.GroupBy(i => i.CurrentLocation).ToList();
            if (locationGroups.Count > 1)
            {
                foreach (var group in locationGroups.Take(5)) // Show top 5 locations
                {
                    OperationSummaries.Add(new BatchOperationSummary
                    {
                        Category = "Locations",
                        Description = group.Key,
                        Value = group.Count().ToString("N0")
                    });
                }
            }
        }

        private void GenerateDeleteSummary()
        {
            var totalValue = OperationItems.Where(i => i.IsSelected).Sum(i => i.EstimatedValue);
            if (totalValue > 0)
            {
                OperationSummaries.Add(new BatchOperationSummary
                {
                    Category = "Financial Impact",
                    Description = "Estimated Total Value",
                    Value = totalValue.ToString("C"),
                    IsImportant = true
                });
            }
        }

        private void GenerateUpdateSummary()
        {
            var fieldsToUpdate = OperationItems.SelectMany(i => i.FieldsToUpdate ?? new List<string>()).Distinct().ToList();
            if (fieldsToUpdate.Any())
            {
                OperationSummaries.Add(new BatchOperationSummary
                {
                    Category = "Updates",
                    Description = "Fields to Update",
                    Value = string.Join(", ", fieldsToUpdate.Take(3)) + (fieldsToUpdate.Count > 3 ? "..." : "")
                });
            }
        }

        private void GenerateTransferSummary()
        {
            var destinationLocations = OperationItems.Where(i => i.IsSelected && !string.IsNullOrEmpty(i.TargetLocation))
                .GroupBy(i => i.TargetLocation).ToList();

            foreach (var group in destinationLocations.Take(3))
            {
                OperationSummaries.Add(new BatchOperationSummary
                {
                    Category = "Destinations",
                    Description = $"To {group.Key}",
                    Value = group.Count().ToString("N0")
                });
            }
        }

        private void GenerateProcessSummary()
        {
            var estimatedTime = OperationItems.Count(i => i.IsSelected) * 2; // 2 seconds per item
            OperationSummaries.Add(new BatchOperationSummary
            {
                Category = "Timing",
                Description = "Estimated Duration",
                Value = TimeSpan.FromSeconds(estimatedTime).ToString(@"mm\:ss")
            });
        }

        private void UpdateConfirmationState()
        {
            if (!string.IsNullOrEmpty(RequiredConfirmationText))
            {
                IsUserConfirmed = string.Equals(UserConfirmationText, RequiredConfirmationText, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                IsUserConfirmed = !RequiresConfirmation || SelectedItemCount > 0;
            }

            CanProceed = !HasWarning || Warnings.All(w => w.Severity != WarningLevel.Critical) || IsUserConfirmed;
        }

        #endregion

        #region Events

        public event Action<BatchConfirmationResult>? OnBatchConfirmed;
        public event Action<BatchConfirmationResult>? OnBatchCancelled;

        #endregion
    }

    #region Supporting Models

    public partial class BatchOperationItem : ObservableObject
    {
        [ObservableProperty]
        private string itemId = string.Empty;

        [ObservableProperty]
        private string itemName = string.Empty;

        [ObservableProperty]
        private string itemDescription = string.Empty;

        [ObservableProperty]
        private string currentLocation = string.Empty;

        [ObservableProperty]
        private string targetLocation = string.Empty;

        [ObservableProperty]
        private bool isSelected = true;

        [ObservableProperty]
        private bool hasIssues;

        [ObservableProperty]
        private string issueDescription = string.Empty;

        [ObservableProperty]
        private decimal estimatedValue;

        [ObservableProperty]
        private int quantity;

        [ObservableProperty]
        private DateTime lastModified = DateTime.Now;

        [ObservableProperty]
        private BatchOperationItemStatus status = BatchOperationItemStatus.Ready;

        public List<string>? FieldsToUpdate { get; set; }
        public Dictionary<string, object>? AdditionalData { get; set; }
    }

    public class BatchOperationWarning
    {
        public WarningLevel Severity { get; set; } = WarningLevel.Information;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class BatchOperationSummary
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsImportant { get; set; }
    }

    public class BatchConfirmationResult
    {
        public bool IsConfirmed { get; set; }
        public string OperationName { get; set; } = string.Empty;
        public BatchOperationType OperationType { get; set; }
        public string BatchId { get; set; } = string.Empty;
        public List<BatchOperationItem> ConfirmedItems { get; set; } = new();
        public DateTime? ScheduledTime { get; set; }
        public bool BackupBeforeOperation { get; set; }
        public bool ContinueOnError { get; set; }
        public List<BatchOperationSummary> OperationSummaries { get; set; } = new();
    }

    public enum BatchOperationType
    {
        Unknown,
        Delete,
        Update,
        Transfer,
        Process,
        Import,
        Export,
        Archive
    }

    public enum BatchOperationItemStatus
    {
        Ready,
        Processing,
        Completed,
        Failed,
        Skipped
    }

    public enum WarningLevel
    {
        Information,
        Warning,
        Error,
        Critical
    }

    #endregion
}
