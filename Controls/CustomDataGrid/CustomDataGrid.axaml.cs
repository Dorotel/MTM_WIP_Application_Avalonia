using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// CustomDataGrid control for MTM WIP Application
    /// Provides high-performance data grid with perfect header-data alignment
    /// Follows MTM design system and MVVM Community Toolkit patterns
    /// </summary>
    public partial class CustomDataGrid : UserControl
    {
        private readonly ILogger<CustomDataGrid> _logger;

        #region Dependency Properties

        /// <summary>
        /// Gets or sets the items source for the data grid
        /// </summary>
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<CustomDataGrid, IEnumerable?>(nameof(ItemsSource));

        /// <summary>
        /// Gets or sets whether multi-selection is enabled
        /// </summary>
        public static readonly StyledProperty<bool> IsMultiSelectEnabledProperty =
            AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsMultiSelectEnabled), true);

        /// <summary>
        /// Command for deleting an item
        /// </summary>
        public static readonly StyledProperty<ICommand?> DeleteItemCommandProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(DeleteItemCommand));

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
        /// Gets or sets whether multi-selection is enabled
        /// </summary>
        public bool IsMultiSelectEnabled
        {
            get => GetValue(IsMultiSelectEnabledProperty);
            set => SetValue(IsMultiSelectEnabledProperty, value);
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

            // Subscribe to ListBox selection changes
            if (DataListBox != null)
            {
                DataListBox.SelectionChanged += OnDataListBoxSelectionChanged;
            }

            // Initialize button states
            UpdateActionButtonStates();

            _logger.LogDebug("CustomDataGrid initialized");
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
            else if (change.Property == IsMultiSelectEnabledProperty)
            {
                HandleMultiSelectEnabledChanged((bool)change.NewValue!);
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

            // Update UI state
            UpdateSelectAllState();
            UpdateActionButtonStates();
            UpdateSelectionInfoText();

            // Log item count for debugging
            if (newValue != null)
            {
                var count = newValue.Cast<object>().Count();
                _logger.LogInformation("Loaded {ItemCount} items into CustomDataGrid", count);
            }
        }

        /// <summary>
        /// Handles changes to the IsMultiSelectEnabled property
        /// </summary>
        private void HandleMultiSelectEnabledChanged(bool isEnabled)
        {
            _logger.LogDebug("Multi-select enabled changed to: {IsEnabled}", isEnabled);

            if (DataListBox != null)
            {
                DataListBox.SelectionMode = isEnabled ? SelectionMode.Multiple : SelectionMode.Single;
            }

            // Update select all checkbox visibility
            if (SelectAllCheckBox != null)
            {
                SelectAllCheckBox.IsVisible = isEnabled;
            }
        }

        /// <summary>
        /// Handles collection change notifications from the items source
        /// </summary>
        private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _logger.LogTrace("Items collection changed: {Action}", e.Action);

            // Update select all state when collection changes
            UpdateSelectAllState();

            // Notify parent of collection change
            var itemCount = ItemsSource?.Cast<object>().Count() ?? 0;
            _logger.LogDebug("Collection changed, new item count: {ItemCount}", itemCount);
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

            // Raise selection changed event
            var selectedItems = DataListBox?.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
            {
                SelectedItems = selectedItems,
                SelectionMode = "ListBoxSelection"
            });
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

            if (DeleteButton != null && DeleteItemCommand != null)
            {
                // For delete, always pass the single selected item (not a collection)
                // The DeleteSingleItemCommand expects a single InventoryItem, not a collection
                DeleteButton.CommandParameter = selectedItem;
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

            _logger.LogTrace("Selection info updated: {SelectionText}", SelectionInfoText.Text);
        }

        /// <summary>
        /// Handles the Select All checkbox click
        /// </summary>
        private void OnSelectAllClick(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox selectAllCheckBox || ItemsSource == null)
                return;

            bool selectAll = selectAllCheckBox.IsChecked == true;
            var selectedItems = new List<object>();

            _logger.LogDebug("Select All clicked: {SelectAll}", selectAll);

            // Update selection state for all items that support it
            foreach (var item in ItemsSource)
            {
                if (item is INotifyPropertyChanged selectable)
                {
                    // Try to set IsSelected property via reflection for dynamic data
                    var isSelectedProperty = item.GetType().GetProperty("IsSelected");
                    if (isSelectedProperty != null && isSelectedProperty.CanWrite)
                    {
                        isSelectedProperty.SetValue(item, selectAll);

                        if (selectAll)
                        {
                            selectedItems.Add(item);
                        }
                    }
                }
            }

            // Notify parent of selection change
            var eventArgs = new SelectionChangedEventArgs
            {
                SelectedItems = selectedItems,
                SelectionMode = selectAll ? "SelectAll" : "DeselectAll"
            };

            SelectionChanged?.Invoke(this, eventArgs);

            _logger.LogInformation("Selection changed: {SelectedCount} items {Action}",
                selectedItems.Count, selectAll ? "selected" : "deselected");
        }

        /// <summary>
        /// Updates the state of the Select All checkbox based on current selection
        /// </summary>
        private void UpdateSelectAllState()
        {
            if (SelectAllCheckBox == null || ItemsSource == null)
                return;

            var items = ItemsSource.Cast<object>().ToList();
            if (items.Count == 0)
            {
                SelectAllCheckBox.IsChecked = false;
                return;
            }

            // Count selected items
            int selectedCount = 0;
            foreach (var item in items)
            {
                var isSelectedProperty = item.GetType().GetProperty("IsSelected");
                if (isSelectedProperty != null)
                {
                    var isSelected = isSelectedProperty.GetValue(item) as bool?;
                    if (isSelected == true)
                    {
                        selectedCount++;
                    }
                }
            }

            // Update checkbox state
            if (selectedCount == 0)
            {
                SelectAllCheckBox.IsChecked = false;
            }
            else if (selectedCount == items.Count)
            {
                SelectAllCheckBox.IsChecked = true;
            }
            else
            {
                SelectAllCheckBox.IsChecked = null; // Indeterminate state
            }

            _logger.LogTrace("Updated Select All state: {SelectedCount}/{TotalCount}", selectedCount, items.Count);
        }

        /// <summary>
        /// Gets the currently selected items
        /// </summary>
        public List<object> GetSelectedItems()
        {
            var selectedItems = new List<object>();

            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    var isSelectedProperty = item.GetType().GetProperty("IsSelected");
                    if (isSelectedProperty != null)
                    {
                        var isSelected = isSelectedProperty.GetValue(item) as bool?;
                        if (isSelected == true)
                        {
                            selectedItems.Add(item);
                        }
                    }
                }
            }

            return selectedItems;
        }

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public int GetSelectedItemCount()
        {
            return GetSelectedItems().Count;
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

        #region Validation and Error Handling

        /// <summary>
        /// Validates that a command parameter is of the expected type
        /// </summary>
        private static bool ValidateCommandParameter<T>(object? parameter, out T? validParameter) where T : class
        {
            validParameter = parameter as T;
            return validParameter != null;
        }

        /// <summary>
        /// Logs command execution for debugging
        /// </summary>
        private void LogCommandExecution(string commandName, object? parameter)
        {
            var parameterType = parameter?.GetType().Name ?? "null";
            _logger.LogDebug("Command executed: {CommandName} with parameter type: {ParameterType}",
                commandName, parameterType);
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
        /// Gets or sets the selection mode (SelectAll, DeselectAll, etc.)
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
