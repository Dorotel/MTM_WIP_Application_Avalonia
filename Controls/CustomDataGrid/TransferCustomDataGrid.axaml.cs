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
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

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
    public static readonly StyledProperty<ObservableCollection<TransferInventoryItem>> ItemsSourceProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ObservableCollection<TransferInventoryItem>>(
            nameof(ItemsSource),
            new ObservableCollection<TransferInventoryItem>());

    /// <summary>
    /// Gets or sets the selected item
    /// </summary>
    public static readonly StyledProperty<TransferInventoryItem?> SelectedItemProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, TransferInventoryItem?>(nameof(SelectedItem));

    /// <summary>
    /// Gets or sets the collection of selected items (strongly-typed for TransferInventoryItem)
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<TransferInventoryItem>> SelectedItemsCollectionProperty =
        AvaloniaProperty.Register<TransferCustomDataGrid, ObservableCollection<TransferInventoryItem>>(
            nameof(SelectedItemsCollection),
            new ObservableCollection<TransferInventoryItem>());

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

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the items source
    /// </summary>
    public ObservableCollection<TransferInventoryItem> ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected item
    /// </summary>
    public TransferInventoryItem? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected items collection
    /// </summary>
    public ObservableCollection<TransferInventoryItem> SelectedItemsCollection
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

        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferCustomDataGrid() constructor started");

        InitializeComponent();

        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferCustomDataGrid InitializeComponent() completed");

        // Set up event handlers after components are initialized
        Loaded += OnControlLoaded;

        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] TransferCustomDataGrid() constructor completed");
        _logger?.LogDebug("TransferCustomDataGrid initialized");
    }

    /// <summary>
    /// Handles the control loaded event to set up UI element event handlers
    /// </summary>
    private void OnControlLoaded(object? sender, RoutedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnControlLoaded started");

        // Set up event handlers
        if (DataListBox != null)
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] DataListBox found - setting up events");
            DataListBox.SelectionChanged += OnDataListBoxSelectionChanged;
            DataListBox.SelectionMode = SelectionMode.Multiple;
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] WARNING: DataListBox not found - selection events will not work");
        }

        // Initialize button states
        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] Updating initial action button states");
        UpdateActionButtonStates();

        System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnControlLoaded completed");
        _logger?.LogDebug("TransferCustomDataGrid control loaded and event handlers attached");
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
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] HandleItemsSourceChanged - Old: {oldValue?.GetType().Name ?? "null"}, New: {newValue?.GetType().Name ?? "null"}");

            // Subscribe to collection change notifications
            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsCollectionChanged;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] Unsubscribed from old collection change notifications");
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsCollectionChanged;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] Subscribed to new collection change notifications");
            }

            // Count items for debugging
            var itemCount = 0;
            if (newValue != null)
            {
                itemCount = newValue.Cast<object>().Count();
            }
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] New ItemsSource contains {itemCount} items");

            // Update display after data change
            UpdateSelectionInfo();
            UpdateActionButtonStates();

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] HandleItemsSourceChanged completed");
            _logger?.LogDebug("ItemsSource changed from {OldType} to {NewType}",
                oldValue?.GetType().Name ?? "null", newValue?.GetType().Name ?? "null");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] ERROR in HandleItemsSourceChanged: {ex.Message}");
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
    /// </summary>
    private void OnDataListBoxSelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnDataListBoxSelectionChanged started");

            if (DataListBox?.SelectedItems == null)
            {
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] DataListBox.SelectedItems is null - returning");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] DataListBox.SelectedItems.Count: {DataListBox.SelectedItems.Count}");

            // Update the SelectedItemsCollection with the current selection
            SelectedItemsCollection.Clear();
            foreach (var item in DataListBox.SelectedItems)
            {
                if (item is TransferInventoryItem transferItem)
                {
                    SelectedItemsCollection.Add(transferItem);
                    System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Added to selection: {transferItem.PartId} - {transferItem.Operation} at {transferItem.Location}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] WARNING: Item is not TransferInventoryItem: {item?.GetType().Name ?? "null"}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] SelectedItemsCollection.Count after update: {SelectedItemsCollection.Count}");

            // Update the single selected item
            if (SelectedItemsCollection.Count > 0)
            {
                if (SelectedItemsCollection.Count == 1)
                {
                    SelectedItem = SelectedItemsCollection[0];
                    System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Single item selected: {SelectedItem.PartId}");
                }
                else
                {
                    // Multiple items selected - clear single selection
                    SelectedItem = null;
                    System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] Multiple items selected ({SelectedItemsCollection.Count}) - cleared single selection");
                }

                UpdateSelectionInfo();
                UpdateActionButtonStates();
            }
            else
            {
                SelectedItem = null;
                System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] No items selected");
                UpdateSelectionInfo();
                UpdateActionButtonStates();
            }

            System.Diagnostics.Debug.WriteLine("[TRANSFER-DEBUG] OnDataListBoxSelectionChanged completed");
            _logger?.LogTrace("Selection changed: {Count} items selected", SelectedItemsCollection.Count);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TRANSFER-DEBUG] ERROR in OnDataListBoxSelectionChanged: {ex.Message}");
            _logger?.LogError(ex, "Error handling DataListBox selection change");
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
                totalQuantity += item.TransferQuantity;

                var key = $"{item.PartId}-{item.Operation}-{item.FromLocation}→{item.ToLocation}";
                transferItems[key] = transferItems.ContainsKey(key)
                    ? transferItems[key] + item.TransferQuantity
                    : item.TransferQuantity;
            }

            TotalTransferQuantityText.Text = $"Total Transfer Qty: {totalQuantity:N0}";

            // Set tooltip with breakdown
            if (transferItems.Count > 0)
            {
                var tooltip = string.Join("\n", transferItems.Select(kvp =>
                {
                    var parts = kvp.Key.Split('→');
                    if (parts.Length >= 2)
                    {
                        var leftParts = parts[0].Split('-');
                        return leftParts.Length >= 3
                            ? $"{leftParts[0]} Op:{leftParts[1]} from {leftParts[2]} → {parts[1]} ({kvp.Value:N0})"
                            : $"{parts[0]} → {parts[1]} ({kvp.Value:N0})";
                    }
                    return $"{kvp.Key} ({kvp.Value:N0})";
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
            var hasValidInventory = SelectedItemsCollection.Any(item => item.TransferQuantity > 0 && item.IsTransferValid);

            if (EditTransferButton != null)
            {
                EditTransferButton.IsEnabled = hasSelection;
            }

            if (ExecuteTransferButton != null)
            {
                ExecuteTransferButton.IsEnabled = hasSelection && hasValidInventory;
            }

            _logger?.LogTrace("Action buttons updated - HasSelection: {HasSelection}, HasValidInventory: {HasValidInventory}",
                hasSelection, hasValidInventory);
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

        // Unsubscribe from control events
        Loaded -= OnControlLoaded;

        base.OnDetachedFromVisualTree(e);
    }

    #endregion
}
