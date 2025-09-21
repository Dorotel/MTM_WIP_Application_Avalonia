using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid.UI;

namespace MTM_WIP_Application_Avalonia.Views.CustomControls;

/// <summary>
/// TransferCustomDataGrid - Custom data grid control optimized for inventory transfer operations.
/// Provides enhanced functionality for location-to-location transfers with visual source→destination indicators,
/// transfer quantity validation, and transfer-specific action buttons.
/// Follows MTM design system and integrates with TransferItemViewModel.
/// Based on WinForms-style multi-selection patterns with Ctrl+Click and Shift+Click support.
/// </summary>
public partial class TransferCustomDataGrid : UserControl
{
    #region Fields

    private readonly ILogger<TransferCustomDataGrid>? _logger;

    #endregion

    #region Dependency Properties

    /// <summary>
    /// Gets or sets the items source for the transfer data grid
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<object>> ItemsSourceProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ObservableCollection<object>>(nameof(ItemsSource), new ObservableCollection<object>());

    /// <summary>
    /// Gets or sets the selected item
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, object?>(nameof(SelectedItem));

    /// <summary>
    /// Gets or sets the collection of selected items
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<object>> SelectedItemsCollectionProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ObservableCollection<object>>(nameof(SelectedItemsCollection), new ObservableCollection<object>());

    /// <summary>
    /// Command for editing a transfer
    /// </summary>
    public static readonly StyledProperty<ICommand?> EditTransferCommandProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ICommand?>(nameof(EditTransferCommand));

    /// <summary>
    /// Command for executing a transfer
    /// </summary>
    public static readonly StyledProperty<ICommand?> ExecuteTransferCommandProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ICommand?>(nameof(ExecuteTransferCommand));

    /// <summary>
    /// Gets or sets the items source
    /// </summary>
    public ObservableCollection<object> ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected item
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected items collection
    /// </summary>
    public ObservableCollection<object> SelectedItemsCollection
    {
        get => GetValue(SelectedItemsCollectionProperty);
        set => SetValue(SelectedItemsCollectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the edit transfer command
    /// </summary>
    public ICommand? EditTransferCommand
    {
        get => GetValue(EditTransferCommandProperty);
        set => SetValue(EditTransferCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the execute transfer command
    /// </summary>
    public ICommand? ExecuteTransferCommand
    {
        get => GetValue(ExecuteTransferCommandProperty);
        set => SetValue(ExecuteTransferCommandProperty, value);
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the TransferCustomDataGrid
    /// </summary>
    public TransferCustomDataGrid()
    {
        // Use null logger pattern from original CustomDataGrid
        _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<TransferCustomDataGrid>.Instance;

        InitializeComponent();

        // Set up event handlers
        if (DataListBox != null)
        {
            DataListBox.SelectionChanged += OnDataListBoxSelectionChanged;
            DataListBox.SelectionMode = SelectionMode.Multiple;
        }

        // Initialize button states
        UpdateActionButtonStates();

        _logger?.LogDebug("TransferCustomDataGrid initialized");
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handles property changes
    /// </summary>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            HandleItemsSourceChanged(change.OldValue as IEnumerable, change.NewValue as IEnumerable);
        }
    }

    /// <summary>
    /// Handles changes to the ItemsSource property
    /// </summary>
    private void HandleItemsSourceChanged(IEnumerable? oldValue, IEnumerable? newValue)
    {
        try
        {
            // Subscribe to collection change notifications
            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsCollectionChanged;
            }

            // Update display after data change
            UpdateSelectionInfo();
            UpdateActionButtonStates();

            _logger?.LogDebug("ItemsSource changed from {OldType} to {NewType}",
                oldValue?.GetType().Name ?? "null", newValue?.GetType().Name ?? "null");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling ItemsSource change");
        }
    }

    /// <summary>
    /// Handles collection change notifications
    /// </summary>
    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        try
        {
            UpdateSelectionInfo();
            UpdateActionButtonStates();
            _logger?.LogTrace("Items collection changed: {Action}", e.Action);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling collection change");
        }
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles selection changes in the DataListBox
    /// Includes type conversion to handle binding compatibility issues
    /// </summary>
    private void OnDataListBoxSelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        try
        {
            if (DataListBox == null) return;

            // CRITICAL FIX: Handle type conversion for SelectedItemsCollection binding
            // This addresses binding errors with ObservableCollection<InventoryItem> to ObservableCollection<object> conversion
            SyncSelectedItemsCollection();

            // Update the single selected item
            if (SelectedItemsCollection.Count > 0)
            {
                if (SelectedItemsCollection.Count == 1)
                {
                    SelectedItem = SelectedItemsCollection[0];
                }
                else
                {
                    // Multiple items selected - clear single selection
                    SelectedItem = null;
                }

                UpdateSelectionInfo();
                UpdateActionButtonStates();
            }
            else
            {
                SelectedItem = null;
                UpdateSelectionInfo();
                UpdateActionButtonStates();
            }

            _logger?.LogTrace("Selection changed: {Count} items selected", SelectedItemsCollection.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling DataListBox selection change");
        }
    }

    /// <summary>
    /// Synchronizes the SelectedItemsCollection property with DataListBox selection
    /// Handles type conversion for Avalonia binding compatibility
    /// Compatible with Universal Overlay System integration planned in refactor
    /// </summary>
    private void SyncSelectedItemsCollection()
    {
        try
        {
            if (DataListBox?.SelectedItems == null)
            {
                SelectedItemsCollection.Clear();
                return;
            }

            // Convert selected items to object collection for binding compatibility
            var selectedItems = DataListBox.SelectedItems.Cast<object>().ToList();

            // Update the collection efficiently - clear and add new items
            SelectedItemsCollection.Clear();
            foreach (var item in selectedItems)
            {
                SelectedItemsCollection.Add(item);
            }

            _logger?.LogTrace("Synced SelectedItemsCollection: {Count} items", selectedItems.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error syncing SelectedItemsCollection");

            // Ensure we have a valid empty collection on error
            SelectedItemsCollection.Clear();
        }
    }

    #endregion

    #region Selection Management

    /// <summary>
    /// Updates the selection information display
    /// </summary>
    private void UpdateSelectionInfo()
    {
        try
        {
            if (SelectionInfoText == null) return;

            var selectedCount = SelectedItemsCollection.Count;
            var totalCount = ItemsSource.Count;

            SelectionInfoText.Text = selectedCount switch
            {
                0 => $"No items selected ({totalCount} total)",
                1 => $"1 item selected ({totalCount} total)",
                _ => $"{selectedCount} items selected ({totalCount} total)"
            };

            // Update total transfer quantity
            UpdateTotalTransferQuantity();

            _logger?.LogTrace("Selection info updated: {Selected}/{Total}", selectedCount, totalCount);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating selection info");
            if (SelectionInfoText != null)
            {
                SelectionInfoText.Text = "Selection info error";
            }
        }
    }

    /// <summary>
    /// Updates the total transfer quantity display
    /// </summary>
    private void UpdateTotalTransferQuantity()
    {
        try
        {
            if (TotalTransferQuantityText == null) return;

            var totalQuantity = 0;
            var transferItems = new Dictionary<string, int>();

            foreach (var item in SelectedItemsCollection)
            {
                if (item is TransferInventoryItem transferItem)
                {
                    totalQuantity += transferItem.TransferQuantity;

                    var key = $"{transferItem.PartId}-{transferItem.FromLocation}-{transferItem.ToLocation}";
                    transferItems[key] = transferItems.ContainsKey(key)
                        ? transferItems[key] + transferItem.TransferQuantity
                        : transferItem.TransferQuantity;
                }
            }

            TotalTransferQuantityText.Text = $"Total Transfer Qty: {totalQuantity:N0}";

            // Set tooltip with breakdown
            if (transferItems.Count > 0)
            {
                var tooltip = string.Join("\n", transferItems.Select(kvp =>
                {
                    var parts = kvp.Key.Split('-');
                    return $"{parts[0]}: {parts[1]} → {parts[2]} ({kvp.Value:N0})";
                }));

                ToolTip.SetTip(TotalTransferQuantityText, tooltip);
            }
            else
            {
                ToolTip.SetTip(TotalTransferQuantityText, "No transfer items selected");
            }

            _logger?.LogTrace("Total transfer quantity updated: {Total}", totalQuantity);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating total transfer quantity");
            if (TotalTransferQuantityText != null)
            {
                TotalTransferQuantityText.Text = "Total Transfer Qty: Error";
            }
        }
    }

    /// <summary>
    /// Updates the state of action buttons based on selection
    /// </summary>
    private void UpdateActionButtonStates()
    {
        try
        {
            var hasSelection = SelectedItemsCollection.Count > 0;
            var hasValidTransfers = SelectedItemsCollection.OfType<TransferInventoryItem>().Any(t => t.TransferQuantity > 0);

            if (EditTransferButton != null)
            {
                EditTransferButton.IsEnabled = hasSelection;
            }

            if (ExecuteTransferButton != null)
            {
                ExecuteTransferButton.IsEnabled = hasSelection && hasValidTransfers;
            }

            _logger?.LogTrace("Action buttons updated - HasSelection: {HasSelection}, HasValidTransfers: {HasValidTransfers}",
                hasSelection, hasValidTransfers);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating action button states");
        }
    }

    #endregion

    #region Button Click Handlers

    /// <summary>
    /// Handles edit transfer button click
    /// </summary>
    private void OnEditTransferClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (EditTransferCommand?.CanExecute(SelectedItemsCollection) == true)
            {
                EditTransferCommand.Execute(SelectedItemsCollection);
                _logger?.LogDebug("Edit transfer command executed for {Count} items", SelectedItemsCollection.Count);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing edit transfer command");
        }
    }

    /// <summary>
    /// Handles execute transfer button click
    /// </summary>
    private void OnExecuteTransferClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (ExecuteTransferCommand?.CanExecute(SelectedItemsCollection) == true)
            {
                ExecuteTransferCommand.Execute(SelectedItemsCollection);
                _logger?.LogDebug("Execute transfer command executed for {Count} items", SelectedItemsCollection.Count);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing transfer command");
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Performs cleanup when the control is detached from the visual tree
    /// </summary>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _logger?.LogDebug("TransferCustomDataGrid detached from visual tree, performing cleanup");

        // Unsubscribe from collection change notifications
        if (ItemsSource is INotifyCollectionChanged collection)
        {
            collection.CollectionChanged -= OnItemsCollectionChanged;
        }

        // Unsubscribe from ListBox events
        if (DataListBox != null)
        {
            DataListBox.SelectionChanged -= OnDataListBoxSelectionChanged;
        }

        base.OnDetachedFromVisualTree(e);
    }

    #endregion
}
