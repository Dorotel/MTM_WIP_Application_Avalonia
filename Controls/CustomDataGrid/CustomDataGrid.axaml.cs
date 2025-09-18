using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;
using Avalonia.Threading;
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
        private readonly SortManager _sortManager;
        private readonly SortConfiguration _sortConfiguration;
        
        // Column Management Components (Phase 3a)
        private ColumnManagementPanel? _columnManagementPanel;
        private Border? _columnManagementContainer;
        private readonly List<ColumnItem> _columnItems = new();
        private bool _isColumnManagementVisible = false;

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

        /// <summary>
        /// Gets or sets whether sorting is enabled
        /// </summary>
        public static readonly StyledProperty<bool> IsSortingEnabledProperty =
            AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsSortingEnabled), true);

        /// <summary>
        /// Gets or sets whether multi-column sorting is enabled
        /// </summary>
        public static readonly StyledProperty<bool> IsMultiColumnSortEnabledProperty =
            AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsMultiColumnSortEnabled), true);

        /// <summary>
        /// Command executed when sorting is requested
        /// </summary>
        public static readonly StyledProperty<ICommand?> SortCommandProperty =
            AvaloniaProperty.Register<CustomDataGrid, ICommand?>(nameof(SortCommand));

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

        /// <summary>
        /// Gets or sets whether sorting is enabled
        /// </summary>
        public bool IsSortingEnabled
        {
            get => GetValue(IsSortingEnabledProperty);
            set => SetValue(IsSortingEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets whether multi-column sorting is enabled
        /// </summary>
        public bool IsMultiColumnSortEnabled
        {
            get => GetValue(IsMultiColumnSortEnabledProperty);
            set => SetValue(IsMultiColumnSortEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets the sort command
        /// </summary>
        public ICommand? SortCommand
        {
            get => GetValue(SortCommandProperty);
            set => SetValue(SortCommandProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event raised when selection changes
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        /// <summary>
        /// Event raised when sorting is requested
        /// </summary>
        public event EventHandler<SortRequestEventArgs>? SortRequested;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the CustomDataGrid
        /// </summary>
        public CustomDataGrid()
        {
            // Use null logger for now - can be injected later if needed
            _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<CustomDataGrid>.Instance;

            // Initialize sorting components
            _sortConfiguration = new SortConfiguration();
            _sortManager = new SortManager();

            InitializeComponent();

            // Initialize column management (Phase 3a)
            InitializeColumnManagement();

            // Subscribe to ListBox selection changes
            if (DataListBox != null)
            {
                DataListBox.SelectionChanged += OnDataListBoxSelectionChanged;
            }

            // Initialize button states
            UpdateActionButtonStates();

            _logger.LogDebug("CustomDataGrid initialized with sorting and column management support");
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
            else if (change.Property == IsMultiColumnSortEnabledProperty)
            {
                HandleMultiColumnSortEnabledChanged((bool)change.NewValue!);
            }
            else if (change.Property == IsSortingEnabledProperty)
            {
                HandleSortingEnabledChanged((bool)change.NewValue!);
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
        /// Handles changes to the IsMultiColumnSortEnabled property
        /// </summary>
        private void HandleMultiColumnSortEnabledChanged(bool isEnabled)
        {
            _logger.LogDebug("Multi-column sort enabled changed to: {IsEnabled}", isEnabled);

            if (_sortConfiguration != null)
            {
                _sortConfiguration.IsMultiColumnSortEnabled = isEnabled;
            }
        }

        /// <summary>
        /// Handles changes to the IsSortingEnabled property
        /// </summary>
        private void HandleSortingEnabledChanged(bool isEnabled)
        {
            _logger.LogDebug("Sorting enabled changed to: {IsEnabled}", isEnabled);

            if (!isEnabled && _sortConfiguration != null)
            {
                // Clear existing sorts when sorting is disabled
                ClearSort();
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

        #region Sorting Event Handlers (Phase 2)

        /// <summary>
        /// Handles header click events for sorting
        /// </summary>
        private void OnHeaderClick(object? sender, PointerPressedEventArgs e)
        {
            if (!IsSortingEnabled) return;

            if (sender is not Border headerBorder) return;
            
            var columnId = headerBorder.Tag?.ToString();
            if (string.IsNullOrEmpty(columnId)) return;

            _logger.LogDebug("Header clicked for column: {ColumnId}", columnId);

            // Check if this is a multi-column sort request (Shift key held)
            bool isMultiColumnSort = e.KeyModifiers.HasFlag(KeyModifiers.Shift) && IsMultiColumnSortEnabled;

            // Determine the next sort direction
            var currentDirection = _sortConfiguration.GetSortDirection(columnId);
            var newDirection = GetNextSortDirection(currentDirection);

            _logger.LogDebug("Sort direction changing from {Current} to {New} for column {ColumnId}", 
                currentDirection, newDirection, columnId);

            try
            {
                // Apply the sort
                if (isMultiColumnSort)
                {
                    _sortConfiguration.ApplyMultiColumnSort(columnId, newDirection);
                }
                else
                {
                    _sortConfiguration.ApplySingleColumnSort(columnId, newDirection);
                }

                // Apply sorting to the data source
                ApplyCurrentSort();

                // Update visual indicators
                UpdateSortIndicators();

                // Raise the SortRequested event
                var sortEventArgs = new SortRequestEventArgs
                {
                    ColumnId = columnId,
                    IsMultiColumn = isMultiColumnSort,
                    RequestedDirection = newDirection
                };

                SortRequested?.Invoke(this, sortEventArgs);

                // Execute sort command if provided
                if (SortCommand?.CanExecute(sortEventArgs) == true)
                {
                    SortCommand.Execute(sortEventArgs);
                }

                _logger.LogInformation("Sort applied successfully for column: {ColumnId} ({Direction})", 
                    columnId, newDirection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying sort for column: {ColumnId}", columnId);
            }
        }

        /// <summary>
        /// Gets the next sort direction in the cycle: None -> Ascending -> Descending -> None
        /// </summary>
        private static SortDirection GetNextSortDirection(SortDirection currentDirection)
        {
            return currentDirection switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.Ascending
            };
        }

        /// <summary>
        /// Applies the current sort configuration to the data source
        /// </summary>
        private void ApplyCurrentSort()
        {
            if (ItemsSource == null) return;

            try
            {
                var sortedItems = _sortManager.GetSortedView(ItemsSource, _sortConfiguration);
                
                if (DataListBox != null)
                {
                    // Preserve selection during sort
                    var selectedItems = DataListBox.SelectedItems?.Cast<object>().ToList() ?? new List<object>();
                    
                    DataListBox.ItemsSource = sortedItems;
                    
                    // Restore selection after sort
                    var sortedList = sortedItems.Cast<object>().ToList();
                    foreach (var item in selectedItems)
                    {
                        if (sortedList.Contains(item))
                        {
                            DataListBox.SelectedItems?.Add(item);
                        }
                    }
                }

                _logger.LogDebug("Sort applied to data source successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying sort to data source");
            }
        }

        /// <summary>
        /// Updates the visual sort indicators in column headers
        /// </summary>
        private void UpdateSortIndicators()
        {
            try
            {
                // Update each sortable column's indicator
                UpdateSortIndicator("PartIdSortIndicator", "PartId");
                UpdateSortIndicator("OperationSortIndicator", "Operation");
                UpdateSortIndicator("LocationSortIndicator", "Location");
                UpdateSortIndicator("QuantitySortIndicator", "Quantity");

                _logger.LogTrace("Sort indicators updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sort indicators");
            }
        }

        /// <summary>
        /// Updates a single sort indicator
        /// </summary>
        private void UpdateSortIndicator(string indicatorName, string columnId)
        {
            var indicator = this.FindControl<TextBlock>(indicatorName);
            if (indicator == null) return;

            var direction = _sortConfiguration.GetSortDirection(columnId);
            var precedence = _sortConfiguration.GetSortPrecedence(columnId);

            indicator.IsVisible = direction != SortDirection.None;
            
            if (direction != SortDirection.None)
            {
                // Set the sort arrow
                indicator.Text = direction == SortDirection.Ascending ? "↑" : "↓";
                
                // Add precedence number for multi-column sorts
                if (precedence > 0)
                {
                    indicator.Text += precedence + 1; // Display as 1-based
                    indicator.Classes.Add("secondary");
                }
                else
                {
                    indicator.Classes.Remove("secondary");
                }
            }
        }

        /// <summary>
        /// Clears all sorting
        /// </summary>
        public void ClearSort()
        {
            try
            {
                _sortConfiguration.ClearAllSorts();
                
                if (ItemsSource != null && DataListBox != null)
                {
                    var unsortedItems = _sortManager.ClearSort(ItemsSource);
                    DataListBox.ItemsSource = unsortedItems;
                }

                UpdateSortIndicators();
                
                _logger.LogDebug("All sorting cleared");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing sort");
            }
        }

        /// <summary>
        /// Gets the current sort configuration
        /// </summary>
        public SortConfiguration GetCurrentSortConfiguration()
        {
            return _sortConfiguration.Clone();
        }

        #endregion

        #region Event Handlers (Future Features)

        /// <summary>
        /// Handles the column management toggle (Phase 3a - Active)
        /// </summary>
        private void OnToggleColumnManagement(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogDebug("Column management toggle clicked");
                
                _isColumnManagementVisible = !_isColumnManagementVisible;
                
                if (_isColumnManagementVisible)
                {
                    ShowColumnManagementPanel();
                }
                else
                {
                    HideColumnManagementPanel();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling column management panel");
            }
        }

        #endregion

        #region Column Management Methods (Phase 3a)

        /// <summary>
        /// Initializes column management components
        /// </summary>
        private void InitializeColumnManagement()
        {
            try
            {
                // Find the column management controls
                _columnManagementContainer = this.Find<Border>("ColumnManagementContainer");
                _columnManagementPanel = this.Find<ColumnManagementPanel>("ColumnManagementPanel");

                // Initialize default column configuration
                InitializeColumnItems();

                // Setup event handlers if controls are found
                if (_columnManagementPanel != null)
                {
                    _columnManagementPanel.ColumnVisibilityChanged += OnColumnVisibilityChanged;
                }

                _logger.LogDebug("Column management initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing column management");
            }
        }

        /// <summary>
        /// Initializes the default column items
        /// </summary>
        private void InitializeColumnItems()
        {
            _columnItems.Clear();
            
            // Define the default columns based on the grid structure
            _columnItems.AddRange(new[]
            {
                new ColumnItem { Id = "Selection", DisplayName = "Selection", IsVisible = true, CanHide = false, Order = 0, Width = 40 },
                new ColumnItem { Id = "PartId", DisplayName = "Part ID", IsVisible = true, CanHide = false, Order = 1, Width = 150 }, // Critical column
                new ColumnItem { Id = "Operation", DisplayName = "Operation", IsVisible = true, CanHide = true, Order = 2, Width = 100 },
                new ColumnItem { Id = "Location", DisplayName = "Location", IsVisible = true, CanHide = true, Order = 3, Width = 120 },
                new ColumnItem { Id = "Quantity", DisplayName = "Quantity", IsVisible = true, CanHide = true, Order = 4, Width = 100 },
                new ColumnItem { Id = "LastUpdated", DisplayName = "Last Updated", IsVisible = true, CanHide = true, Order = 5, Width = 180 },
                new ColumnItem { Id = "Notes", DisplayName = "Notes", IsVisible = true, CanHide = true, Order = 6, Width = 80 },
                new ColumnItem { Id = "Actions", DisplayName = "Actions", IsVisible = true, CanHide = true, Order = 7, Width = 100 },
                new ColumnItem { Id = "Management", DisplayName = "Manage", IsVisible = true, CanHide = true, Order = 8, Width = 40 }
            });

            // Pass column items to the management panel
            if (_columnManagementPanel != null)
            {
                _columnManagementPanel.SetColumnItems(_columnItems);
            }
        }

        /// <summary>
        /// Shows the column management panel with animation
        /// </summary>
        private void ShowColumnManagementPanel()
        {
            if (_columnManagementContainer == null) return;

            try
            {
                _columnManagementContainer.IsVisible = true;
                
                // Animate panel width from 0 to 300
                var animation = new Animation
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    Children =
                    {
                        new KeyFrame
                        {
                            Cue = new Cue(0.0),
                            Setters = { new Setter(Border.WidthProperty, 0.0) }
                        },
                        new KeyFrame
                        {
                            Cue = new Cue(1.0),
                            Setters = { new Setter(Border.WidthProperty, 300.0) }
                        }
                    }
                };

                animation.RunAsync(_columnManagementContainer);

                _logger.LogDebug("Column management panel shown");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing column management panel");
                // Fallback: Just show without animation
                _columnManagementContainer.IsVisible = true;
                _columnManagementContainer.Width = 300;
            }
        }

        /// <summary>
        /// Hides the column management panel with animation
        /// </summary>
        private void HideColumnManagementPanel()
        {
            if (_columnManagementContainer == null) return;

            try
            {
                // Animate panel width from 300 to 0
                var animation = new Animation
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    Children =
                    {
                        new KeyFrame
                        {
                            Cue = new Cue(0.0),
                            Setters = { new Setter(Border.WidthProperty, 300.0) }
                        },
                        new KeyFrame
                        {
                            Cue = new Cue(1.0),
                            Setters = { new Setter(Border.WidthProperty, 0.0) }
                        }
                    }
                };

                animation.RunAsync(_columnManagementContainer).ContinueWith(_ =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _columnManagementContainer.IsVisible = false;
                    });
                });

                _logger.LogDebug("Column management panel hidden");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hiding column management panel");
                // Fallback: Just hide without animation
                _columnManagementContainer.IsVisible = false;
                _columnManagementContainer.Width = 0;
            }
        }

        /// <summary>
        /// Handles column visibility changes from the management panel
        /// </summary>
        private void OnColumnVisibilityChanged(object? sender, ColumnVisibilityChangedEventArgs e)
        {
            try
            {
                var columnItem = _columnItems.FirstOrDefault(c => c.Id == e.ColumnId);
                if (columnItem != null)
                {
                    columnItem.IsVisible = e.IsVisible;
                    ApplyColumnVisibility(e.ColumnId, e.IsVisible);
                    
                    _logger.LogDebug("Column visibility changed: {ColumnId} = {IsVisible}", e.ColumnId, e.IsVisible);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling column visibility change for {ColumnId}", e.ColumnId);
            }
        }

        /// <summary>
        /// Applies column visibility changes to the grid
        /// </summary>
        private void ApplyColumnVisibility(string columnId, bool isVisible)
        {
            try
            {
                var headerGrid = this.Find<Grid>("DynamicHeaderGrid");
                var dataListBox = this.Find<ListBox>("DataListBox");

                if (headerGrid == null || dataListBox == null) return;

                // Map column IDs to grid column indices
                var columnIndex = GetColumnIndex(columnId);
                if (columnIndex < 0 || columnIndex >= headerGrid.ColumnDefinitions.Count) return;

                // Update header visibility
                var headerBorder = headerGrid.Children.OfType<Border>().ElementAtOrDefault(columnIndex);
                if (headerBorder != null)
                {
                    headerBorder.IsVisible = isVisible;
                }

                // Update column definition width
                var columnDefinition = headerGrid.ColumnDefinitions[columnIndex];
                if (isVisible)
                {
                    // Restore original width (this is simplified - could be enhanced with stored widths)
                    var columnItem = _columnItems.FirstOrDefault(c => c.Id == columnId);
                    if (columnItem != null)
                    {
                        columnDefinition.Width = new GridLength(columnItem.Width, GridUnitType.Pixel);
                    }
                }
                else
                {
                    columnDefinition.Width = new GridLength(0, GridUnitType.Pixel);
                }

                // Update data template visibility (this would require more complex data template changes)
                // For now, we'll just refresh the ItemsSource
                RefreshGridDisplay();

                _logger.LogDebug("Applied column visibility: {ColumnId} = {IsVisible}", columnId, isVisible);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying column visibility for {ColumnId}", columnId);
            }
        }

        /// <summary>
        /// Maps column ID to grid column index
        /// </summary>
        private int GetColumnIndex(string columnId)
        {
            return columnId switch
            {
                "Selection" => 0,
                "PartId" => 1,
                "Operation" => 2,
                "Location" => 3,
                "Quantity" => 4,
                "LastUpdated" => 5,
                "Notes" => 6,
                "Actions" => 7,
                "Management" => 8,
                _ => -1
            };
        }

        /// <summary>
        /// Refreshes the grid display after column changes
        /// </summary>
        private void RefreshGridDisplay()
        {
            try
            {
                var dataListBox = this.Find<ListBox>("DataListBox");
                if (dataListBox != null && dataListBox.ItemsSource != null)
                {
                    var currentSource = dataListBox.ItemsSource;
                    dataListBox.ItemsSource = null;
                    dataListBox.ItemsSource = currentSource;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing grid display");
            }
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
