using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models.Core;

namespace MTM_WIP_Application_Avalonia.Views.CustomControls
{
    /// <summary>
    /// CustomDataGrid control for MTM WIP Application
    /// Provides high-performance data grid with WinForms-style multi-selection
    /// Supports Ctrl+Click (toggle), Shift+Click (range), and drag selection
    /// Follows MTM design system and MVVM Community Toolkit patterns
    /// </summary>
    public partial class CustomDataGrid : UserControl
    {
        private readonly ILogger<CustomDataGrid> _logger;
        private int _lastSelectedIndex = -1;
        private bool _isDragging = false;
        private int _dragStartIndex = -1;

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the items source for the data grid
        /// </summary>
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<CustomDataGrid, IEnumerable?>(nameof(ItemsSource));

        /// <summary>
        /// Command for deleting an item
        /// </summary>
        public static readonly StyledProperty<ICommand?> DeleteItemCommandProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(DeleteItemCommand));

        /// <summary>
        /// Command for deleting multiple selected items with confirmation
        /// </summary>
        public static readonly StyledProperty<ICommand?> MultiRowDeleteCommandProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(MultiRowDeleteCommand));

        /// <summary>
        /// Command for editing an item
        /// </summary>
        public static readonly StyledProperty<ICommand?> EditItemCommandProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(EditItemCommand));

        /// <summary>
        /// Gets or sets the selected item
        /// </summary>
        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<CustomDataGrid, object?>(nameof(SelectedItem));

        /// <summary>
        /// Gets or sets the selected items collection (for multi-selection binding)
        /// </summary>
        public static readonly StyledProperty<ICollection<object>?> SelectedItemsCollectionProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICollection<object>?>(nameof(SelectedItemsCollection));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the items source for the data grid
        /// </summary>
        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the command for deleting an item
        /// </summary>
        public ICommand? DeleteItemCommand
        {
            get => GetValue(DeleteItemCommandProperty);
            set => SetValue(DeleteItemCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command for deleting multiple selected items with confirmation
        /// </summary>
        public ICommand? MultiRowDeleteCommand
        {
            get => GetValue(MultiRowDeleteCommandProperty);
            set => SetValue(MultiRowDeleteCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command for editing an item
        /// </summary>
        public ICommand? EditItemCommand
        {
            get => GetValue(EditItemCommandProperty);
            set => SetValue(EditItemCommandProperty, value);
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
        /// Gets or sets the selected items collection (for multi-selection binding)
        /// </summary>
        public ICollection<object>? SelectedItemsCollection
        {
            get => GetValue(SelectedItemsCollectionProperty);
            set => SetValue(SelectedItemsCollectionProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when selection changes
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomDataGrid
        /// </summary>
        public CustomDataGrid()
        {
            // Use null logger for now - can be injected later if needed
            _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<CustomDataGrid>.Instance;

            InitializeComponent();

            // Subscribe to ListBox selection changes and mouse events
            if (DataListBox != null)
            {
                DataListBox.SelectionChanged += OnDataListBoxSelectionChanged;
                DataListBox.PointerPressed += OnDataListBoxPointerPressed;
                DataListBox.PointerMoved += OnDataListBoxPointerMoved;
                DataListBox.PointerReleased += OnDataListBoxPointerReleased;

                // Enable multi-selection by default
                DataListBox.SelectionMode = SelectionMode.Multiple;
            }

            // Initialize button states
            UpdateActionButtonStates();

            _logger.LogDebug("CustomDataGrid initialized with WinForms-style multi-selection");
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
            _logger.LogDebug("ItemsSource changed from {OldType} to {NewType}",
                oldValue?.GetType().Name ?? "null",
                newValue?.GetType().Name ?? "null");

            // Unsubscribe from old collection change notifications
            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= OnItemsCollectionChanged;
            }

            // Subscribe to new collection change notifications
            if (newValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += OnItemsCollectionChanged;
            }

            // Reset selection tracking
            _lastSelectedIndex = -1;
            _isDragging = false;
            _dragStartIndex = -1;

            // Update UI state
            UpdateActionButtonStates();
            UpdateSelectionInfoText();
            UpdateTotalQuantityDisplay(); // Update totals when data changes

            // Log item count for debugging
            if (newValue != null)
            {
                var count = newValue.Cast<object>().Count();
                _logger.LogInformation("Loaded {ItemCount} items into CustomDataGrid", count);
            }
        }

        /// <summary>
        /// Handles collection change notifications from the items source
        /// </summary>
        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _logger.LogTrace("Items collection changed: {Action}", e.Action);

            // Update total quantities when collection changes
            UpdateTotalQuantityDisplay();

            // Notify parent of collection change
            var itemCount = ItemsSource?.Cast<object>().Count() ?? 0;
            _logger.LogDebug("Collection changed, new item count: {ItemCount}", itemCount);
        }

        #endregion

        #region WinForms-Style Multi-Selection

        /// <summary>
        /// Handles pointer pressed events for WinForms-style selection
        /// </summary>
        private void OnDataListBoxPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataListBox == null) return;

            var point = e.GetPosition(DataListBox);
            var hitTestResult = DataListBox.InputHitTest(point);
            var listBoxItem = FindParent<ListBoxItem>(hitTestResult as Control);

            if (listBoxItem == null) return;

            var clickedIndex = DataListBox.IndexFromContainer(listBoxItem);
            if (clickedIndex < 0) return;

            var keyModifiers = e.KeyModifiers;
            var isCtrlPressed = keyModifiers.HasFlag(KeyModifiers.Control);
            var isShiftPressed = keyModifiers.HasFlag(KeyModifiers.Shift);

            _logger.LogDebug("Item clicked at index {Index}, Ctrl: {Ctrl}, Shift: {Shift}",
                clickedIndex, isCtrlPressed, isShiftPressed);

            if (isCtrlPressed)
            {
                // Ctrl+Click: Toggle selection
                HandleCtrlClick(clickedIndex);
            }
            else if (isShiftPressed && _lastSelectedIndex >= 0)
            {
                // Shift+Click: Range selection
                HandleShiftClick(clickedIndex);
            }
            else
            {
                // Normal click: Single selection
                HandleNormalClick(clickedIndex);

                // Start potential drag operation
                _isDragging = true;
                _dragStartIndex = clickedIndex;
            }

            _lastSelectedIndex = clickedIndex;
            e.Handled = true;
        }

        /// <summary>
        /// Handles pointer moved events for drag selection
        /// </summary>
        private void OnDataListBoxPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDragging || DataListBox == null || _dragStartIndex < 0) return;

            var point = e.GetPosition(DataListBox);
            var hitTestResult = DataListBox.InputHitTest(point);
            var listBoxItem = FindParent<ListBoxItem>(hitTestResult as Control);

            if (listBoxItem == null) return;

            var currentIndex = DataListBox.IndexFromContainer(listBoxItem);
            if (currentIndex < 0) return;

            // Perform drag selection from drag start to current position
            HandleDragSelection(_dragStartIndex, currentIndex);
        }

        /// <summary>
        /// Handles pointer released events to end drag selection
        /// </summary>
        private void OnDataListBoxPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            _isDragging = false;
            _dragStartIndex = -1;
        }

        /// <summary>
        /// Handles normal click (single selection)
        /// </summary>
        private void HandleNormalClick(int index)
        {
            if (DataListBox == null) return;

            DataListBox.SelectedItems?.Clear();
            if (index >= 0 && index < (ItemsSource?.Cast<object>().Count() ?? 0))
            {
                var items = ItemsSource?.Cast<object>().ToList();
                if (items != null && index < items.Count)
                {
                    DataListBox.SelectedItems?.Add(items[index]);
                }
            }

            _logger.LogDebug("Normal click selection: index {Index}", index);
        }

        /// <summary>
        /// Handles Ctrl+Click (toggle selection)
        /// </summary>
        private void HandleCtrlClick(int index)
        {
            if (DataListBox == null || ItemsSource == null) return;

            var items = ItemsSource.Cast<object>().ToList();
            if (index < 0 || index >= items.Count) return;

            var item = items[index];
            var isSelected = DataListBox.SelectedItems?.Contains(item) == true;

            if (isSelected)
            {
                DataListBox.SelectedItems?.Remove(item);
            }
            else
            {
                DataListBox.SelectedItems?.Add(item);
            }

            _logger.LogDebug("Ctrl+Click toggle: index {Index}, now {Selected}",
                index, !isSelected ? "selected" : "deselected");
        }

        /// <summary>
        /// Handles Shift+Click (range selection)
        /// </summary>
        private void HandleShiftClick(int currentIndex)
        {
            if (DataListBox == null || ItemsSource == null || _lastSelectedIndex < 0) return;

            var items = ItemsSource.Cast<object>().ToList();
            var startIndex = Math.Min(_lastSelectedIndex, currentIndex);
            var endIndex = Math.Max(_lastSelectedIndex, currentIndex);

            // Clear current selection and select range
            DataListBox.SelectedItems?.Clear();
            for (int i = startIndex; i <= endIndex && i < items.Count; i++)
            {
                DataListBox.SelectedItems?.Add(items[i]);
            }

            _logger.LogDebug("Shift+Click range selection: from {Start} to {End}", startIndex, endIndex);
        }

        /// <summary>
        /// Handles drag selection
        /// </summary>
        private void HandleDragSelection(int startIndex, int currentIndex)
        {
            if (DataListBox == null || ItemsSource == null) return;

            var items = ItemsSource.Cast<object>().ToList();
            var rangeStart = Math.Min(startIndex, currentIndex);
            var rangeEnd = Math.Max(startIndex, currentIndex);

            // Clear and select the drag range
            DataListBox.SelectedItems?.Clear();
            for (int i = rangeStart; i <= rangeEnd && i < items.Count; i++)
            {
                DataListBox.SelectedItems?.Add(items[i]);
            }

            _logger.LogTrace("Drag selection: from {Start} to {End}", rangeStart, rangeEnd);
        }

        /// <summary>
        /// Finds parent control of specified type
        /// </summary>
        private static T? FindParent<T>(Control? child) where T : Control
        {
            var parent = child?.Parent as Control;
            while (parent != null)
            {
                if (parent is T targetParent)
                    return targetParent;
                parent = parent.Parent as Control;
            }
            return null;
        }

        #endregion

        #region Selection Management

        /// <summary>
        /// Handles ListBox selection changes for action button state management
        /// </summary>
        private void OnDataListBoxSelectionChanged(object? sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            _logger.LogDebug("DataListBox selection changed: {SelectedCount} items selected",
                DataListBox?.SelectedItems?.Count ?? 0);

            // Update action button states based on selection
            UpdateActionButtonStates();

            // Update selection info text
            UpdateSelectionInfoText();

            // Update SelectedItem property
            SelectedItem = DataListBox?.SelectedItem;

            // CRITICAL FIX: Handle type conversion for SelectedItemsCollection binding
            // This addresses the binding error: cannot convert ObservableCollection<InventoryItem> to ICollection<object>
            SyncSelectedItemsCollection();

            // Raise selection changed event
            var selectedItemsForEvent = DataListBox?.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
            {
                SelectedItems = selectedItemsForEvent,
                SelectionMode = "WinFormsSelection"
            });
        }

        /// <summary>
        /// Synchronizes the SelectedItemsCollection property with DataListBox selection
        /// Handles type conversion between strongly typed collections and object collections
        /// Compatible with Universal Overlay System integration planned in refactor
        /// </summary>
        private void SyncSelectedItemsCollection()
        {
            try
            {
                if (DataListBox?.SelectedItems == null)
                {
                    if (SelectedItemsCollection != null)
                    {
                        SelectedItemsCollection.Clear();
                    }
                    return;
                }

                var selectedItems = DataListBox.SelectedItems.Cast<object>().ToList();

                // Create new collection to properly trigger binding notifications
                // This works with both ICollection<object> and ObservableCollection<object> bindings
                var newCollection = new System.Collections.ObjectModel.ObservableCollection<object>(selectedItems);

                // Set the property to trigger two-way binding
                SelectedItemsCollection = newCollection;

                _logger.LogDebug("Synced SelectedItemsCollection: {Count} items", selectedItems.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing SelectedItemsCollection");

                // Fallback: ensure we have a valid empty collection
                SelectedItemsCollection = new System.Collections.ObjectModel.ObservableCollection<object>();
            }
        }

        /// <summary>
        /// Updates the state of action buttons based on current selection
        /// </summary>
        private void UpdateActionButtonStates()
        {
            var selectedItems = DataListBox?.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
            var selectedCount = selectedItems.Count;
            var hasSelection = selectedCount > 0;
            var singleSelection = selectedCount == 1;

            _logger.LogTrace("Updating action button states: {SelectedCount} items selected", selectedCount);

            // Enable/disable buttons based on selection
            if (EditButton != null)
                EditButton.IsEnabled = singleSelection; // Edit only works on single selection

            if (DeleteButton != null)
                DeleteButton.IsEnabled = hasSelection; // Delete works on any selection

            // Update command parameters with selected items
            UpdateCommandParameters();
        }

        /// <summary>
        /// Updates the command parameters to use selected items from ListBox
        /// </summary>
        private void UpdateCommandParameters()
        {
            var selectedItems = DataListBox?.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
            var selectedItem = selectedItems.FirstOrDefault();

            _logger.LogTrace("Updating command parameters for {SelectedCount} selected items", selectedItems.Count);

            // Update action button command parameters to use selected item(s)
            if (EditButton != null && EditItemCommand != null && selectedItem != null)
            {
                EditButton.CommandParameter = selectedItem;
            }

            if (DeleteButton != null)
            {
                // Smart delete button logic: choose between single and multi-row commands
                if (selectedItems.Count > 1 && MultiRowDeleteCommand != null)
                {
                    // Multiple items selected - use multi-row delete command (no parameter needed)
                    DeleteButton.Command = MultiRowDeleteCommand;
                    DeleteButton.CommandParameter = null;
                    _logger.LogTrace("Delete button configured for multi-row deletion ({Count} items)", selectedItems.Count);
                }
                else if (selectedItems.Count == 1 && DeleteItemCommand != null)
                {
                    // Single item selected - use single item delete command
                    DeleteButton.Command = DeleteItemCommand;
                    DeleteButton.CommandParameter = selectedItem;
                    _logger.LogTrace("Delete button configured for single item deletion");
                }
                else if (selectedItems.Count == 0)
                {
                    // No items selected - disable delete button
                    DeleteButton.Command = null;
                    DeleteButton.CommandParameter = null;
                    _logger.LogTrace("Delete button disabled - no items selected");
                }
                else
                {
                    // Fallback: use single delete command if multi-row not available
                    DeleteButton.Command = DeleteItemCommand;
                    DeleteButton.CommandParameter = selectedItem;
                    _logger.LogTrace("Delete button configured for single item deletion (fallback)");
                }
            }
        }

        /// <summary>
        /// Updates the selection info text display
        /// </summary>
        private void UpdateSelectionInfoText()
        {
            if (SelectionInfoText == null)
                return;

            var selectedCount = DataListBox?.SelectedItems?.Count ?? 0;
            var totalCount = ItemsSource?.Cast<object>().Count() ?? 0;

            SelectionInfoText.Text = selectedCount switch
            {
                0 => "No items selected",
                1 => "1 item selected",
                _ => $"{selectedCount} items selected"
            };

            if (totalCount > 0)
            {
                SelectionInfoText.Text += $" of {totalCount}";
            }

            // Update total quantity display
            UpdateTotalQuantityDisplay();

            _logger.LogTrace("Selection info updated: {SelectionText}", SelectionInfoText.Text);
        }

        /// <summary>
        /// Updates the total quantity display and tooltip
        /// </summary>
        private void UpdateTotalQuantityDisplay()
        {
            if (TotalQuantityText == null || ItemsSource == null)
                return;

            try
            {
                // Calculate total quantity and group by PartId and Operation
                var totalQuantity = 0;
                var groupedTotals = new Dictionary<(string PartId, string Operation), int>();

                foreach (var item in ItemsSource)
                {
                    // Handle both InventoryItem and EditInventoryModel types
                    string partId = GetPartId(item);
                    string operation = GetOperation(item);
                    int quantity = GetQuantity(item);

                    if (!string.IsNullOrEmpty(partId) && !string.IsNullOrEmpty(operation) && quantity > 0)
                    {
                        totalQuantity += quantity;

                        var key = (partId, operation);
                        if (groupedTotals.ContainsKey(key))
                            groupedTotals[key] += quantity;
                        else
                            groupedTotals[key] = quantity;
                    }
                }

                // Update the display text
                TotalQuantityText.Text = $"Total Quantity: {totalQuantity:N0}";

                // Generate tooltip with breakdown
                var tooltipLines = new List<string>();
                foreach (var kvp in groupedTotals.OrderBy(x => x.Key.PartId).ThenBy(x => x.Key.Operation))
                {
                    tooltipLines.Add($"{kvp.Key.PartId} ({kvp.Key.Operation}) Total: {kvp.Value:N0}");
                }

                if (tooltipLines.Count > 0)
                {
                    ToolTip.SetTip(TotalQuantityText, string.Join("\n", tooltipLines));
                }
                else
                {
                    ToolTip.SetTip(TotalQuantityText, "No inventory items available");
                }

                _logger.LogTrace("Total quantity updated: {Total}, Groups: {Groups}", totalQuantity, groupedTotals.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating total quantity display");
                TotalQuantityText.Text = "Total Quantity: Error";
                ToolTip.SetTip(TotalQuantityText, "Error calculating totals");
            }
        }

        /// <summary>
        /// Gets the PartId from an inventory item, handling different types
        /// </summary>
        private string GetPartId(object item)
        {
            return item switch
            {
                MTM_Shared_Logic.Models.InventoryItem inventoryItem => inventoryItem.PartId,
                EditInventoryModel editModel => editModel.PartId,
                _ => GetPropertyValue(item, "PartId") ?? GetPropertyValue(item, "PartID") ?? string.Empty
            };
        }

        /// <summary>
        /// Gets the Operation from an inventory item, handling different types
        /// </summary>
        private string GetOperation(object item)
        {
            return item switch
            {
                MTM_Shared_Logic.Models.InventoryItem inventoryItem => inventoryItem.Operation ?? string.Empty,
                EditInventoryModel editModel => editModel.Operation,
                _ => GetPropertyValue(item, "Operation") ?? string.Empty
            };
        }

        /// <summary>
        /// Gets the Quantity from an inventory item, handling different types
        /// </summary>
        private int GetQuantity(object item)
        {
            return item switch
            {
                MTM_Shared_Logic.Models.InventoryItem inventoryItem => inventoryItem.Quantity,
                EditInventoryModel editModel => editModel.Quantity,
                _ => int.TryParse(GetPropertyValue(item, "Quantity"), out int qty) ? qty : 0
            };
        }

        /// <summary>
        /// Gets a property value by name using reflection (fallback for unknown types)
        /// </summary>
        private string? GetPropertyValue(object item, string propertyName)
        {
            try
            {
                var property = item.GetType().GetProperty(propertyName);
                return property?.GetValue(item)?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogTrace(ex, "Failed to get property {PropertyName} from {Type}", propertyName, item.GetType().Name);
                return null;
            }
        }

        /// <summary>
        /// Gets the currently selected items
        /// </summary>
        public List<object> GetSelectedItems()
        {
            return DataListBox?.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
        }

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public int GetSelectedItemCount()
        {
            return DataListBox?.SelectedItems?.Count ?? 0;
        }

        /// <summary>
        /// Clears all selections
        /// </summary>
        public void ClearSelection()
        {
            DataListBox?.SelectedItems?.Clear();
            _lastSelectedIndex = -1;
            _logger.LogDebug("Selection cleared");
        }

        /// <summary>
        /// Selects all items
        /// </summary>
        public void SelectAll()
        {
            if (DataListBox == null || ItemsSource == null) return;

            DataListBox.SelectedItems?.Clear();
            foreach (var item in ItemsSource)
            {
                DataListBox.SelectedItems?.Add(item);
            }

            _logger.LogDebug("All items selected");
        }

        #endregion

        #region Event Handlers (Future Features)

        /// <summary>
        /// Handles the column management toggle (Phase 3 - Currently disabled)
        /// </summary>
        private void OnToggleColumnManagement(object sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Column management toggle clicked (Phase 3 - Not implemented)");

            // Phase 3 implementation:
            // - Toggle ColumnManagementContainer visibility
            // - Animate panel slide in/out
            // - Load column management UI
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Performs cleanup when the control is detached from the visual tree
        /// </summary>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _logger.LogDebug("CustomDataGrid detached from visual tree, performing cleanup");

            // Unsubscribe from collection change notifications
            if (ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnItemsCollectionChanged;
            }

            // Unsubscribe from ListBox events
            if (DataListBox != null)
            {
                DataListBox.SelectionChanged -= OnDataListBoxSelectionChanged;
                DataListBox.PointerPressed -= OnDataListBoxPointerPressed;
                DataListBox.PointerMoved -= OnDataListBoxPointerMoved;
                DataListBox.PointerReleased -= OnDataListBoxPointerReleased;
            }

            base.OnDetachedFromVisualTree(e);
        }

        #endregion
    }

    #region Event Arguments

    /// <summary>
    /// Event arguments for selection changed events
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the list of selected items
        /// </summary>
        public List<object> SelectedItems { get; set; } = new();

        /// <summary>
        /// Gets or sets the selection mode (WinFormsSelection, etc.)
        /// </summary>
        public string SelectionMode { get; set; } = string.Empty;

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public int SelectedCount => SelectedItems.Count;

        /// <summary>
        /// Gets or sets the timestamp of the selection change
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    #endregion
}
