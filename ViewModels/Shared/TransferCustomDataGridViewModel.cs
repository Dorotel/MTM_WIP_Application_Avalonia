using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_Shared_Logic.Models;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid.UI;

namespace MTM_WIP_Application_Avalonia.ViewModels.Shared;

/// <summary>
/// ViewModel for TransferCustomDataGrid control using MVVM Community Toolkit patterns.
/// Provides specialized functionality for inventory transfer operations including
/// location-to-location transfers, transfer quantity validation, and integration
/// with TransferItemViewModel business logic.
/// Follows established MTM patterns with proper error handling and logging.
/// </summary>
public partial class TransferCustomDataGridViewModel : BaseViewModel
{
    #region Private Fields

    private readonly IApplicationStateService _applicationStateService;
    private readonly ISuccessOverlayService? _successOverlayService;

    #endregion

    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<TransferInventoryItem> _itemsSource = new();

    [ObservableProperty]
    private ObservableCollection<CustomDataGridColumn> _columns = new();

    [ObservableProperty]
    private TransferInventoryItem? _selectedItem;

    [ObservableProperty]
    private ObservableCollection<TransferInventoryItem> _selectedItems = new();

    [ObservableProperty]
    private bool _isMultiSelectEnabled = true;

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _showTransferArrows = true;

    [ObservableProperty]
    private double _rowHeight = 36.0;

    #endregion

    #region Computed Properties

    /// <summary>
    /// Gets whether there is a current selection
    /// </summary>
    public bool HasSelection => SelectedItem != null;

    /// <summary>
    /// Gets whether there are multiple items selected
    /// </summary>
    public bool HasMultipleSelection => SelectedItems.Count > 1;

    /// <summary>
    /// Gets the total number of items in the data source
    /// </summary>
    public int TotalItemCount => ItemsSource.Count;

    /// <summary>
    /// Gets the number of selected items
    /// </summary>
    public int SelectedItemCount => SelectedItems.Count;

    /// <summary>
    /// Gets the total transfer quantity for selected items
    /// </summary>
    public int TotalTransferQuantity => SelectedItems.Sum(item => item.TransferQuantity);

    /// <summary>
    /// Gets whether transfers can be executed (has valid selections)
    /// </summary>
    public bool CanExecuteTransfers => HasSelection && SelectedItems.All(item => item.IsTransferValid);

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the TransferCustomDataGridViewModel
    /// </summary>
    public TransferCustomDataGridViewModel(
        ILogger<TransferCustomDataGridViewModel> logger,
        IApplicationStateService applicationStateService,
        ISuccessOverlayService? successOverlayService = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(applicationStateService);

        _applicationStateService = applicationStateService;
        _successOverlayService = successOverlayService;

        Logger.LogDebug("TransferCustomDataGridViewModel initialized");

        // Initialize event handlers
        SelectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;

        // Set up default columns for transfer operations
        SetupTransferColumns();
    }

    #endregion

    #region Commands

    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync()
    {
        IsLoading = true;
        try
        {
            Logger.LogInformation("Refreshing transfer grid data");

            await RefreshTransferDataAsync();

            Logger.LogDebug("Transfer grid data refreshed successfully, item count: {Count}", ItemsSource.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error refreshing transfer grid data");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Refresh transfer grid data", _applicationStateService.CurrentUser);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private async Task EditTransferAsync()
    {
        if (SelectedItem == null) return;

        try
        {
            Logger.LogInformation("Editing transfer for item: {PartId} from {FromLocation} to {ToLocation}",
                SelectedItem.PartId, SelectedItem.FromLocation, SelectedItem.ToLocation);

            await EditTransferItemAsync(SelectedItem);

            Logger.LogDebug("Transfer item edited successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error editing transfer item");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Edit transfer item", _applicationStateService.CurrentUser);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecuteTransfers))]
    private async Task ExecuteTransferAsync()
    {
        if (SelectedItems.Count == 0) return;

        try
        {
            Logger.LogInformation("Executing transfers for {Count} items", SelectedItems.Count);

            if (SelectedItems.Count == 1)
            {
                await ExecuteSingleTransferAsync(SelectedItems[0]);
            }
            else
            {
                await ExecuteMultipleTransfersAsync(SelectedItems.ToList());
            }

            Logger.LogDebug("Transfer execution completed successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing transfers");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Execute transfers", _applicationStateService.CurrentUser);
        }
    }

    [RelayCommand]
    private void ClearSelection()
    {
        try
        {
            SelectedItem = null;
            SelectedItems.Clear();
            Logger.LogDebug("Transfer selection cleared");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing transfer selection");
        }
    }

    [RelayCommand]
    private void SelectAll()
    {
        try
        {
            if (IsMultiSelectEnabled)
            {
                SelectedItems.Clear();
                foreach (var item in ItemsSource)
                {
                    SelectedItems.Add(item);
                }
                Logger.LogDebug("All transfer items selected, count: {Count}", SelectedItems.Count);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error selecting all transfer items");
        }
    }

    [RelayCommand]
    private async Task ValidateTransfersAsync()
    {
        try
        {
            Logger.LogInformation("Validating transfers for {Count} items", SelectedItems.Count);

            foreach (var item in SelectedItems)
            {
                await ValidateTransferItemAsync(item);
            }

            OnPropertyChanged(nameof(CanExecuteTransfers));
            Logger.LogDebug("Transfer validation completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating transfers");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Validate transfers", _applicationStateService.CurrentUser);
        }
    }

    #endregion

    #region Command Can Execute Methods

    private bool CanRefresh => !IsLoading;

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the transfer data source for the grid
    /// </summary>
    public void SetTransferItems(ObservableCollection<TransferInventoryItem> items)
    {
        try
        {
            ItemsSource.Clear();
            foreach (var item in items)
            {
                ItemsSource.Add(item);
            }

            Logger.LogDebug("Transfer ItemsSource set with {Count} items", items.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting transfer ItemsSource");
        }
    }

    /// <summary>
    /// Adds a transfer item to the grid
    /// </summary>
    public void AddTransferItem(TransferInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        try
        {
            ItemsSource.Add(item);
            Logger.LogDebug("Transfer item added: {PartId} from {FromLocation} to {ToLocation}",
                item.PartId, item.FromLocation, item.ToLocation);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding transfer item");
        }
    }

    /// <summary>
    /// Removes a transfer item from the grid
    /// </summary>
    public bool RemoveTransferItem(TransferInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        try
        {
            var removed = ItemsSource.Remove(item);
            if (removed)
            {
                // Also remove from selections
                SelectedItems.Remove(item);
                if (SelectedItem == item)
                {
                    SelectedItem = null;
                }

                Logger.LogDebug("Transfer item removed: {PartId}", item.PartId);
            }
            return removed;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing transfer item");
            return false;
        }
    }

    /// <summary>
    /// Updates transfer quantities for selected items
    /// </summary>
    public void UpdateTransferQuantities(Dictionary<string, int> quantityUpdates)
    {
        ArgumentNullException.ThrowIfNull(quantityUpdates);

        try
        {
            foreach (var item in SelectedItems)
            {
                var key = $"{item.PartId}_{item.Operation}_{item.FromLocation}";
                if (quantityUpdates.TryGetValue(key, out var newQuantity))
                {
                    item.TransferQuantity = Math.Min(newQuantity, item.AvailableQuantity);
                    Logger.LogDebug("Updated transfer quantity for {PartId}: {Quantity}",
                        item.PartId, item.TransferQuantity);
                }
            }

            OnPropertyChanged(nameof(TotalTransferQuantity));
            OnPropertyChanged(nameof(CanExecuteTransfers));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating transfer quantities");
        }
    }

    #endregion

    #region Protected Virtual Methods

    /// <summary>
    /// Override this method in derived classes to implement specific transfer data refresh logic
    /// </summary>
    protected virtual async Task RefreshTransferDataAsync()
    {
        // Default implementation - derived classes should override
        await Task.Delay(100); // Simulate async operation
        Logger.LogDebug("Default refresh transfer data implementation completed");
    }

    /// <summary>
    /// Override this method in derived classes to implement specific transfer editing logic
    /// </summary>
    protected virtual async Task EditTransferItemAsync(TransferInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Default implementation - placeholder
        await Task.Delay(100); // Simulate async operation
        Logger.LogDebug("Default edit transfer item implementation completed for {PartId}", item.PartId);
    }

    /// <summary>
    /// Execute single transfer operation
    /// </summary>
    protected virtual async Task ExecuteSingleTransferAsync(TransferInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        try
        {
            Logger.LogInformation("Executing single transfer: {PartId} from {FromLocation} to {ToLocation}, Qty: {Quantity}",
                item.PartId, item.FromLocation, item.ToLocation, item.TransferQuantity);

            // Validate transfer before execution
            await ValidateTransferItemAsync(item);

            if (!item.IsTransferValid)
            {
                Logger.LogWarning("Transfer validation failed for {PartId}", item.PartId);
                return;
            }

            // Execute transfer via database service
            // Using placeholder result until actual stored procedure is available
            Logger.LogInformation("Executing transfer for {PartId}: {Quantity} from {FromLocation} to {ToLocation}",
                item.PartId, item.TransferQuantity, item.FromLocation, item.ToLocation);

            var result = new { IsSuccess = true, Message = "Transfer completed successfully" };

            // Simulate database call
            await Task.Delay(100);

            if (result.IsSuccess)
            {
                // Remove from grid after successful transfer
                ItemsSource.Remove(item);
                SelectedItems.Remove(item);
                if (SelectedItem == item)
                {
                    SelectedItem = null;
                }

                // Show success notification
                if (_successOverlayService != null)
                {
                    Logger.LogInformation("Transfer successful for {PartId}: {Quantity} from {FromLocation} to {ToLocation}",
                        item.PartId, item.TransferQuantity, item.FromLocation, item.ToLocation);
                }

                Logger.LogInformation("Transfer completed successfully: {PartId}", item.PartId);
            }
            else
            {
                Logger.LogError("Transfer failed: {Message}", result.Message);
                await Services.Core.ErrorHandling.HandleErrorAsync(
                    new InvalidOperationException(result.Message),
                    $"Transfer failed for {item.PartId}",
                    _applicationStateService.CurrentUser
                );
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing single transfer for {PartId}", item.PartId);
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, $"Execute single transfer for {item.PartId}", _applicationStateService.CurrentUser);
            throw;
        }
    }

    /// <summary>
    /// Execute multiple transfer operations
    /// </summary>
    protected virtual async Task ExecuteMultipleTransfersAsync(List<TransferInventoryItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        try
        {
            Logger.LogInformation("Executing multiple transfers for {Count} items", items.Count);

            var successfulTransfers = new List<TransferInventoryItem>();
            var failedTransfers = new List<(TransferInventoryItem Item, string Error)>();

            foreach (var item in items)
            {
                try
                {
                    await ExecuteSingleTransferAsync(item);
                    successfulTransfers.Add(item);
                }
                catch (Exception ex)
                {
                    failedTransfers.Add((item, ex.Message));
                    Logger.LogError(ex, "Transfer failed for item {PartId}", item.PartId);
                }
            }

            // Show batch completion summary
            if (_successOverlayService != null)
            {
                Logger.LogInformation("Batch Transfer Complete - Successful: {Success}, Failed: {Failed}",
                    successfulTransfers.Count, failedTransfers.Count);
            }

            Logger.LogInformation("Multiple transfers completed - Success: {Success}, Failed: {Failed}",
                successfulTransfers.Count, failedTransfers.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error executing multiple transfers");
            await Services.Core.ErrorHandling.HandleErrorAsync(ex, "Execute multiple transfers", _applicationStateService.CurrentUser);
            throw;
        }
    }

    /// <summary>
    /// Validate transfer item for execution
    /// </summary>
    protected virtual async Task ValidateTransferItemAsync(TransferInventoryItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        try
        {
            await Task.Delay(1); // Simulate async validation

            // Basic validation rules for transfers
            item.IsTransferValid = true;
            item.ValidationErrors.Clear();

            // Check transfer quantity
            if (item.TransferQuantity <= 0)
            {
                item.IsTransferValid = false;
                item.ValidationErrors.Add("Transfer quantity must be greater than 0");
            }

            if (item.TransferQuantity > item.AvailableQuantity)
            {
                item.IsTransferValid = false;
                item.ValidationErrors.Add($"Transfer quantity cannot exceed available quantity ({item.AvailableQuantity})");
            }

            // Check locations
            if (string.IsNullOrWhiteSpace(item.FromLocation))
            {
                item.IsTransferValid = false;
                item.ValidationErrors.Add("From location is required");
            }

            if (string.IsNullOrWhiteSpace(item.ToLocation))
            {
                item.IsTransferValid = false;
                item.ValidationErrors.Add("To location is required");
            }

            if (item.FromLocation == item.ToLocation)
            {
                item.IsTransferValid = false;
                item.ValidationErrors.Add("From and To locations cannot be the same");
            }

            Logger.LogDebug("Transfer validation completed for {PartId}: Valid={IsValid}, Errors={ErrorCount}",
                item.PartId, item.IsTransferValid, item.ValidationErrors.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating transfer item {PartId}", item.PartId);
            item.IsTransferValid = false;
            item.ValidationErrors.Add("Validation error occurred");
        }
    }

    #endregion

    #region Private Methods

    private void SetupTransferColumns()
    {
        try
        {
            // Transfer-specific columns
            Columns.Clear();

            Columns.Add(new CustomDataGridColumn("PartId", "Part ID", typeof(string), 120));
            Columns.Add(new CustomDataGridColumn("Operation", "Operation", typeof(string), 80));
            Columns.Add(new CustomDataGridColumn("FromLocation", "From Location", typeof(string), 100));
            Columns.Add(new CustomDataGridColumn("TransferArrow", "➜", typeof(string), 40));
            Columns.Add(new CustomDataGridColumn("ToLocation", "To Location", typeof(string), 100));
            Columns.Add(new CustomDataGridColumn("AvailableQuantity", "Available", typeof(int), 80));
            Columns.Add(new CustomDataGridColumn("TransferQuantity", "Transfer", typeof(int), 80));
            Columns.Add(new CustomDataGridColumn("Notes", "Notes", typeof(string), 80));

            Logger.LogDebug("Transfer columns configured: {Count} columns", Columns.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting up transfer columns");
        }
    }

    private void OnSelectedItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        try
        {
            OnPropertyChanged(nameof(SelectedItemCount));
            OnPropertyChanged(nameof(HasMultipleSelection));
            OnPropertyChanged(nameof(TotalTransferQuantity));
            OnPropertyChanged(nameof(CanExecuteTransfers));

            // Update SelectedItem to the first selected item if we have selections
            if (SelectedItems.Count > 0 && SelectedItem != SelectedItems[0])
            {
                SelectedItem = SelectedItems[0];
            }
            else if (SelectedItems.Count == 0)
            {
                SelectedItem = null;
            }

            Logger.LogDebug("Selected transfer items collection changed, new count: {Count}", SelectedItems.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error handling selected transfer items collection change");
        }
    }

    #endregion

    #region Cleanup

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                SelectedItems.CollectionChanged -= OnSelectedItemsCollectionChanged;
                ItemsSource.Clear();
                SelectedItems.Clear();
                Columns.Clear();
                Logger.LogDebug("TransferCustomDataGridViewModel disposed");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error disposing TransferCustomDataGridViewModel");
            }
        }

        base.Dispose(disposing);
    }

    #endregion
}

