using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Data;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// MTM Custom Data Grid Control - Phase 1 Implementation
/// 
/// High-performance data grid using ItemsRepeater for virtual scrolling.
/// Provides customizable columns, MTM theme integration, and basic selection.
/// Designed to replace standard DataGrid implementations across MTM inventory views.
/// 
/// Features:
/// - Virtual scrolling with ItemsRepeater for large datasets
/// - Configurable column definitions with binding support
/// - MTM design system integration with DynamicResource bindings
/// - Basic selection capabilities
/// - Theme-aware styling with proper contrast ratios
/// 
/// Future enhancements will include sorting, filtering, column management, 
/// and settings persistence following the established MTM patterns.
/// </summary>
public partial class CustomDataGrid : UserControl
{
    #region Dependency Properties

    /// <summary>
    /// Gets or sets the data source for the grid.
    /// Should be an ObservableCollection or IEnumerable for proper binding.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<CustomDataGrid, IEnumerable?>(nameof(ItemsSource), null, defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// Gets or sets the column definitions for the grid.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<CustomDataGridColumn>> ColumnsProperty =
        AvaloniaProperty.Register<CustomDataGrid, ObservableCollection<CustomDataGridColumn>>(
            nameof(Columns), 
            new ObservableCollection<CustomDataGridColumn>(),
            defaultBindingMode: BindingMode.OneWay);

    /// <summary>
    /// Gets or sets the currently selected item.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<CustomDataGrid, object?>(nameof(SelectedItem), null, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets whether multiple items can be selected.
    /// </summary>
    public static readonly StyledProperty<bool> IsMultiSelectEnabledProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsMultiSelectEnabled), false);

    /// <summary>
    /// Gets or sets the collection of selected items for multi-selection.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<object>?> SelectedItemsProperty =
        AvaloniaProperty.Register<CustomDataGrid, ObservableCollection<object>?>(nameof(SelectedItems), null, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the command to execute when deleting an item.
    /// </summary>
    public static readonly StyledProperty<System.Windows.Input.ICommand?> DeleteItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, System.Windows.Input.ICommand?>(nameof(DeleteItemCommand), null);

    /// <summary>
    /// Gets or sets the command to execute when editing an item.
    /// </summary>
    public static readonly StyledProperty<System.Windows.Input.ICommand?> EditItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, System.Windows.Input.ICommand?>(nameof(EditItemCommand), null);

    /// <summary>
    /// Gets or sets the command to execute when duplicating an item.
    /// </summary>
    public static readonly StyledProperty<System.Windows.Input.ICommand?> DuplicateItemCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, System.Windows.Input.ICommand?>(nameof(DuplicateItemCommand), null);

    /// <summary>
    /// Gets or sets the command to execute when viewing item details.
    /// </summary>
    public static readonly StyledProperty<System.Windows.Input.ICommand?> ViewDetailsCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, System.Windows.Input.ICommand?>(nameof(ViewDetailsCommand), null);

    /// <summary>
    /// Gets or sets the row height for data rows.
    /// </summary>
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<CustomDataGrid, double>(nameof(RowHeight), 28.0);

    /// <summary>
    /// Gets or sets whether column reordering is enabled.
    /// Phase 3 feature for drag-and-drop column reordering.
    /// </summary>
    public static readonly StyledProperty<bool> IsColumnReorderingEnabledProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsColumnReorderingEnabled), true);

    /// <summary>
    /// Gets or sets whether column resizing is enabled.
    /// Phase 3 feature for interactive column width adjustment.
    /// </summary>
    public static readonly StyledProperty<bool> IsColumnResizingEnabledProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsColumnResizingEnabled), true);

    /// <summary>
    /// Gets or sets whether the column management panel is visible.
    /// Phase 3 feature for advanced column management UI.
    /// </summary>
    public static readonly StyledProperty<bool> IsColumnManagementVisibleProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsColumnManagementVisible), false);

    /// <summary>
    /// Gets or sets the command to execute when toggling column visibility.
    /// Phase 3 feature for column show/hide operations.
    /// </summary>
    public static readonly StyledProperty<System.Windows.Input.ICommand?> ColumnVisibilityCommandProperty =
        AvaloniaProperty.Register<CustomDataGrid, System.Windows.Input.ICommand?>(nameof(ColumnVisibilityCommand), null);

    /// <summary>
    /// Gets or sets whether the filter panel is visible.
    /// Phase 5 feature for advanced filtering and search capabilities.
    /// </summary>
    public static readonly StyledProperty<bool> IsFilterPanelVisibleProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsFilterPanelVisible), false);

    /// <summary>
    /// Gets or sets whether filtering is enabled for this grid.
    /// Phase 5 feature for data filtering capabilities.
    /// </summary>
    public static readonly StyledProperty<bool> IsFilteringEnabledProperty =
        AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsFilteringEnabled), true);

    /// <summary>
    /// Gets or sets the current filter configuration.
    /// Phase 5 feature for managing active filters and search criteria.
    /// </summary>
    public static readonly StyledProperty<FilterConfiguration?> FilterConfigurationProperty =
        AvaloniaProperty.Register<CustomDataGrid, FilterConfiguration?>(nameof(FilterConfiguration), null, defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the filtered items source (items that match current filters).
    /// Phase 5 feature for displaying filtered results.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> FilteredItemsSourceProperty =
        AvaloniaProperty.Register<CustomDataGrid, IEnumerable?>(nameof(FilteredItemsSource), null, defaultBindingMode: BindingMode.OneWay);

    #endregion

    #region Properties

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ObservableCollection<CustomDataGridColumn> Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public ObservableCollection<object>? SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public bool IsMultiSelectEnabled
    {
        get => GetValue(IsMultiSelectEnabledProperty);
        set => SetValue(IsMultiSelectEnabledProperty, value);
    }

    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    public System.Windows.Input.ICommand? DeleteItemCommand
    {
        get => GetValue(DeleteItemCommandProperty);
        set => SetValue(DeleteItemCommandProperty, value);
    }

    public System.Windows.Input.ICommand? EditItemCommand
    {
        get => GetValue(EditItemCommandProperty);
        set => SetValue(EditItemCommandProperty, value);
    }

    public System.Windows.Input.ICommand? DuplicateItemCommand
    {
        get => GetValue(DuplicateItemCommandProperty);
        set => SetValue(DuplicateItemCommandProperty, value);
    }

    public System.Windows.Input.ICommand? ViewDetailsCommand
    {
        get => GetValue(ViewDetailsCommandProperty);
        set => SetValue(ViewDetailsCommandProperty, value);
    }

    public bool IsColumnReorderingEnabled
    {
        get => GetValue(IsColumnReorderingEnabledProperty);
        set => SetValue(IsColumnReorderingEnabledProperty, value);
    }

    public bool IsColumnResizingEnabled
    {
        get => GetValue(IsColumnResizingEnabledProperty);
        set => SetValue(IsColumnResizingEnabledProperty, value);
    }

    public bool IsColumnManagementVisible
    {
        get => GetValue(IsColumnManagementVisibleProperty);
        set => SetValue(IsColumnManagementVisibleProperty, value);
    }

    public System.Windows.Input.ICommand? ColumnVisibilityCommand
    {
        get => GetValue(ColumnVisibilityCommandProperty);
        set => SetValue(ColumnVisibilityCommandProperty, value);
    }

    public bool IsFilterPanelVisible
    {
        get => GetValue(IsFilterPanelVisibleProperty);
        set => SetValue(IsFilterPanelVisibleProperty, value);
    }

    public bool IsFilteringEnabled
    {
        get => GetValue(IsFilteringEnabledProperty);
        set => SetValue(IsFilteringEnabledProperty, value);
    }

    public FilterConfiguration? FilterConfiguration
    {
        get => GetValue(FilterConfigurationProperty);
        set => SetValue(FilterConfigurationProperty, value);
    }

    public IEnumerable? FilteredItemsSource
    {
        get => GetValue(FilteredItemsSourceProperty);
        private set => SetValue(FilteredItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets the collection of visible columns (filtered from Columns where IsVisible = true).
    /// </summary>
    public ObservableCollection<CustomDataGridColumn> VisibleColumns { get; private set; }

    #endregion

    #region Fields

    private ListBox? _dataListBox;
    private ScrollViewer? _dataScrollViewer;
    private ItemsControl? _headerItemsControl;
    private CheckBox? _selectAllCheckBox;
    private Grid? _columnManagementPanelHost;
    private Border? _columnManagementContainer;
    private ColumnManagementPanel? _columnManagementPanel;
    private Grid? _filterPanelHost;
    private Border? _filterPanelContainer;
    private FilterPanel? _filterPanel;
    private ViewModels.FilterPanelViewModel? _filterPanelViewModel;
    private readonly ILogger? _logger;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the CustomDataGrid control.
    /// Sets up the visual tree and ensures proper initialization following
    /// MTM patterns with minimal code-behind.
    /// </summary>
    public CustomDataGrid()
    {
        // Design-time safe logger creation
        try
        {
            // For now, use null logger - DI integration will be handled by consumers
            _logger = null;
        }
        catch
        {
            _logger = null; // Graceful fallback for design-time
        }

        // Initialize SelectedItems if null
        if (SelectedItems == null)
        {
            SelectedItems = new ObservableCollection<object>();
        }

        VisibleColumns = new ObservableCollection<CustomDataGridColumn>();
        
        InitializeComponent();
        
        // Set up property change handlers
        PropertyChanged += OnPropertyChanged;
        
        // Initialize columns collection change handler
        Columns.CollectionChanged += OnColumnsCollectionChanged;
        
        _logger?.LogDebug("CustomDataGrid initialized successfully");
    }

    #endregion

    #region Initialization

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        
        // Get references to named controls
        _dataListBox = this.FindControl<ListBox>("DataListBox");
        _dataScrollViewer = this.FindControl<ScrollViewer>("DataScrollViewer");
        _headerItemsControl = this.FindControl<ItemsControl>("HeaderItemsControl");
        _selectAllCheckBox = this.FindControl<CheckBox>("SelectAllCheckBox");
        _columnManagementPanelHost = this.FindControl<Grid>("ColumnManagementPanelHost");
        _columnManagementContainer = this.FindControl<Border>("ColumnManagementContainer");
        _filterPanelHost = this.FindControl<Grid>("FilterPanelHost");
        _filterPanelContainer = this.FindControl<Border>("FilterPanelContainer");
        
        // Set up ListBox selection mode based on IsMultiSelectEnabled
        UpdateSelectionMode();
        
        // Initialize column management panel
        InitializeColumnManagementPanel();
        
        // Initialize filter panel
        InitializeFilterPanel();
        
        _logger?.LogDebug("CustomDataGrid components initialized");
    }

    /// <summary>
    /// Initializes the column management panel for Phase 3 features.
    /// Sets up the panel UI and binds to column management events.
    /// </summary>
    private void InitializeColumnManagementPanel()
    {
        try
        {
            if (_columnManagementPanelHost != null)
            {
                // Create the column management panel
                _columnManagementPanel = new ColumnManagementPanel
                {
                    Columns = Columns,
                    SavedConfigurations = new ObservableCollection<ColumnConfiguration>(), // Will be populated by service
                    Margin = new Thickness(0)
                };

                // Subscribe to panel events
                _columnManagementPanel.CloseRequested += OnColumnManagementCloseRequested;
                _columnManagementPanel.ColumnsModified += OnColumnManagementColumnsModified;
                _columnManagementPanel.ConfigurationSaveRequested += OnColumnManagementConfigurationSaveRequested;
                _columnManagementPanel.ConfigurationLoadRequested += OnColumnManagementConfigurationLoadRequested;

                // Add panel to host grid
                _columnManagementPanelHost.Children.Add(_columnManagementPanel);

                _logger?.LogDebug("Column management panel initialized");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing column management panel");
        }
    }

    /// <summary>
    /// Initializes the filter panel for Phase 5 features.
    /// Sets up the panel UI and binds to filter events.
    /// </summary>
    private void InitializeFilterPanel()
    {
        try
        {
            if (_filterPanelHost != null)
            {
                // Create the filter panel ViewModel first (we'll need to use DI here in a real implementation)
                _filterPanelViewModel = new ViewModels.FilterPanelViewModel(
                    Microsoft.Extensions.Logging.Abstractions.NullLogger<ViewModels.FilterPanelViewModel>.Instance);

                // Create the filter panel
                _filterPanel = new FilterPanel
                {
                    DataContext = _filterPanelViewModel,
                    Margin = new Thickness(0)
                };

                // Subscribe to panel events
                _filterPanelViewModel.FiltersChanged += OnFiltersChanged;
                _filterPanelViewModel.CloseRequested += OnFilterPanelCloseRequested;

                // Add panel to host grid
                _filterPanelHost.Children.Add(_filterPanel);

                // Initialize filter configuration if not already set
                if (FilterConfiguration == null)
                {
                    FilterConfiguration = new FilterConfiguration();
                }

                _logger?.LogDebug("Filter panel initialized");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing filter panel");
        }
    }

    #endregion

    #region Event Handlers

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            if (e.Property == ColumnsProperty)
            {
                RefreshVisibleColumns();
                _logger?.LogDebug("Columns property changed, refreshed visible columns");
            }
            else if (e.Property == ItemsSourceProperty)
            {
                _logger?.LogDebug("ItemsSource changed, item count: {Count}", 
                    (ItemsSource as ICollection)?.Count ?? 0);
            }
            else if (e.Property == IsMultiSelectEnabledProperty)
            {
                UpdateSelectionMode();
                _logger?.LogDebug("Multi-select mode changed to: {IsEnabled}", IsMultiSelectEnabled);
            }
            else if (e.Property == SelectedItemsProperty)
            {
                UpdateSelectAllCheckBoxState();
                _logger?.LogDebug("SelectedItems changed, count: {Count}", SelectedItems?.Count ?? 0);
            }
            else if (e.Property == IsColumnManagementVisibleProperty)
            {
                UpdateColumnManagementVisibility();
                _logger?.LogDebug("Column management visibility changed: {IsVisible}", IsColumnManagementVisible);
            }
            else if (e.Property == IsFilterPanelVisibleProperty)
            {
                UpdateFilterPanelVisibility();
                _logger?.LogDebug("Filter panel visibility changed: {IsVisible}", IsFilterPanelVisible);
            }
            else if (e.Property == ItemsSourceProperty)
            {
                ApplyFilters();
                _logger?.LogDebug("ItemsSource changed, applying filters");
            }
            else if (e.Property == FilterConfigurationProperty)
            {
                ApplyFilters();
                _logger?.LogDebug("Filter configuration changed, applying filters");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling property change for {PropertyName}", e.Property.Name);
        }
    }

    private void OnColumnsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        try
        {
            RefreshVisibleColumns();
            
            // Subscribe to property changes on new columns
            if (e.NewItems != null)
            {
                foreach (CustomDataGridColumn column in e.NewItems)
                {
                    column.PropertyChanged += OnColumnPropertyChanged;
                }
            }

            // Unsubscribe from old columns
            if (e.OldItems != null)
            {
                foreach (CustomDataGridColumn column in e.OldItems)
                {
                    column.PropertyChanged -= OnColumnPropertyChanged;
                }
            }

            _logger?.LogDebug("Columns collection changed, visible columns updated");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling columns collection change");
        }
    }

    private void OnColumnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(CustomDataGridColumn.IsVisible))
            {
                RefreshVisibleColumns();
                _logger?.LogDebug("Column visibility changed, refreshed visible columns");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling column property change");
        }
    }

    /// <summary>
    /// Handles the Select All checkbox click event.
    /// </summary>
    public void OnSelectAllClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!IsMultiSelectEnabled || _selectAllCheckBox == null || SelectedItems == null)
                return;

            if (_selectAllCheckBox.IsChecked == true)
            {
                SelectAllItems();
            }
            else
            {
                ClearSelection();
            }

            _logger?.LogDebug("Select All clicked, new selection count: {Count}", SelectedItems.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Select All click");
        }
    }

    /// <summary>
    /// Handles the toggle column management button click event.
    /// Phase 3 feature for showing/hiding the column management panel.
    /// </summary>
    public void OnToggleColumnManagement(object? sender, RoutedEventArgs e)
    {
        try
        {
            IsColumnManagementVisible = !IsColumnManagementVisible;
            _logger?.LogDebug("Column management toggled: {IsVisible}", IsColumnManagementVisible);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error toggling column management");
        }
    }

    #endregion

    #region Phase 3 - Column Management Event Handlers

    /// <summary>
    /// Handles the close request from the column management panel.
    /// </summary>
    private void OnColumnManagementCloseRequested(object? sender, EventArgs e)
    {
        IsColumnManagementVisible = false;
    }

    /// <summary>
    /// Handles columns modified event from the management panel.
    /// </summary>
    private void OnColumnManagementColumnsModified(object? sender, EventArgs e)
    {
        try
        {
            RefreshGrid();
            _logger?.LogDebug("Grid refreshed due to column modifications");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling column modifications");
        }
    }

    /// <summary>
    /// Handles configuration save requests from the management panel.
    /// </summary>
    private void OnColumnManagementConfigurationSaveRequested(object? sender, ColumnConfiguration configuration)
    {
        try
        {
            // In a real implementation, this would use a service to save the configuration
            _logger?.LogInformation("Configuration save requested: {ConfigName}", configuration.DisplayName);
            
            // For now, just add it to the panel's saved configurations if it doesn't exist
            if (_columnManagementPanel?.SavedConfigurations != null)
            {
                var existing = _columnManagementPanel.SavedConfigurations
                    .FirstOrDefault(c => c.ConfigurationId == configuration.ConfigurationId);
                    
                if (existing == null)
                {
                    _columnManagementPanel.SavedConfigurations.Add(configuration);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling configuration save request");
        }
    }

    /// <summary>
    /// Handles configuration load requests from the management panel.
    /// </summary>
    private void OnColumnManagementConfigurationLoadRequested(object? sender, ColumnConfiguration configuration)
    {
        try
        {
            _logger?.LogInformation("Configuration load requested: {ConfigName}", configuration.DisplayName);
            
            // Apply the configuration to the current columns
            _columnManagementPanel?.ApplyConfiguration(configuration);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling configuration load request");
        }
    }

    /// <summary>
    /// Handles filter changes from the filter panel.
    /// </summary>
    private void OnFiltersChanged(object? sender, ViewModels.FilterChangedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Filters changed: {HasFilters}", e.HasActiveFilters);
            
            // Update the filter configuration
            if (FilterConfiguration != null)
            {
                FilterConfiguration.GlobalSearchText = e.GlobalSearchText;
                FilterConfiguration.IsGlobalSearchCaseSensitive = e.IsGlobalSearchCaseSensitive;
                FilterConfiguration.IsActive = e.HasActiveFilters;
                FilterConfiguration.UpdateLastModified();
            }
            
            // Apply the filters
            ApplyFilters();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling filter changes");
        }
    }

    /// <summary>
    /// Handles close requests from the filter panel.
    /// </summary>
    private void OnFilterPanelCloseRequested(object? sender, EventArgs e)
    {
        try
        {
            _logger?.LogDebug("Filter panel close requested");
            IsFilterPanelVisible = false;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling filter panel close request");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Refreshes the grid layout and data display.
    /// Call this method after making programmatic changes to data or columns.
    /// </summary>
    public void RefreshGrid()
    {
        try
        {
            RefreshVisibleColumns();
            InvalidateVisual();
            _logger?.LogDebug("Grid refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error refreshing grid");
        }
    }

    /// <summary>
    /// Scrolls to make the specified item visible.
    /// </summary>
    /// <param name="item">The item to scroll to</param>
    public void ScrollToItem(object item)
    {
        try
        {
            // Implementation for scrolling to specific item using ListBox
            _dataListBox?.ScrollIntoView(item);
            _logger?.LogDebug("Scroll to item requested");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error scrolling to item");
        }
    }

    /// <summary>
    /// Selects all items in the grid.
    /// Only works when IsMultiSelectEnabled is true.
    /// </summary>
    public void SelectAllItems()
    {
        try
        {
            if (!IsMultiSelectEnabled || SelectedItems == null || ItemsSource == null)
                return;

            SelectedItems.Clear();
            foreach (var item in ItemsSource.Cast<object>())
            {
                SelectedItems.Add(item);
            }

            UpdateSelectAllCheckBoxState();
            _logger?.LogDebug("All items selected, count: {Count}", SelectedItems.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error selecting all items");
        }
    }

    /// <summary>
    /// Clears the current selection.
    /// </summary>
    public void ClearSelection()
    {
        try
        {
            SelectedItem = null;
            SelectedItems?.Clear();
            UpdateSelectAllCheckBoxState();
            _logger?.LogDebug("Selection cleared");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error clearing selection");
        }
    }

    /// <summary>
    /// Gets the number of selected items.
    /// </summary>
    public int GetSelectedCount()
    {
        return IsMultiSelectEnabled ? (SelectedItems?.Count ?? 0) : (SelectedItem != null ? 1 : 0);
    }

    #endregion

    #region Private Methods

    private void RefreshVisibleColumns()
    {
        try
        {
            VisibleColumns.Clear();
            
            foreach (var column in Columns.Where(c => c.IsVisible))
            {
                VisibleColumns.Add(column);
            }

            _logger?.LogDebug("Visible columns refreshed, count: {Count}", VisibleColumns.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error refreshing visible columns");
        }
    }

    /// <summary>
    /// Updates the ListBox selection mode based on IsMultiSelectEnabled property.
    /// </summary>
    private void UpdateSelectionMode()
    {
        try
        {
            if (_dataListBox != null)
            {
                _dataListBox.SelectionMode = IsMultiSelectEnabled 
                    ? Avalonia.Controls.SelectionMode.Multiple 
                    : Avalonia.Controls.SelectionMode.Single;
                
                _logger?.LogDebug("Selection mode updated to: {Mode}", _dataListBox.SelectionMode);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating selection mode");
        }
    }

    /// <summary>
    /// Updates the visibility of the column management panel.
    /// Phase 3 feature for toggling the management UI.
    /// </summary>
    private void UpdateColumnManagementVisibility()
    {
        try
        {
            if (_columnManagementContainer != null)
            {
                _columnManagementContainer.IsVisible = IsColumnManagementVisible;
                
                // Update the columns binding in the management panel
                if (_columnManagementPanel != null && IsColumnManagementVisible)
                {
                    _columnManagementPanel.Columns = Columns;
                }
                
                _logger?.LogDebug("Column management visibility updated: {IsVisible}", IsColumnManagementVisible);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating column management visibility");
        }
    }

    /// <summary>
    /// Updates the visibility of the filter panel.
    /// Phase 5 feature for toggling the filtering UI.
    /// </summary>
    private void UpdateFilterPanelVisibility()
    {
        try
        {
            if (_filterPanelContainer != null)
            {
                _filterPanelContainer.IsVisible = IsFilterPanelVisible;
                
                // Initialize or update filter panel when shown
                if (_filterPanel != null && _filterPanelViewModel != null && IsFilterPanelVisible)
                {
                    _filterPanelViewModel.InitializeFromColumns(Columns);
                }
                
                _logger?.LogDebug("Filter panel visibility updated: {IsVisible}", IsFilterPanelVisible);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating filter panel visibility");
        }
    }

    /// <summary>
    /// Applies the current filter configuration to the data source.
    /// Phase 5 feature for filtering data based on search criteria and column filters.
    /// </summary>
    private void ApplyFilters()
    {
        try
        {
            if (!IsFilteringEnabled || ItemsSource == null)
            {
                FilteredItemsSource = ItemsSource;
                return;
            }

            var sourceItems = ItemsSource.Cast<object>().ToList();
            
            // If no filters are active, show all items
            if (FilterConfiguration == null || !HasActiveFilters())
            {
                FilteredItemsSource = sourceItems;
                UpdateFilterStatistics(sourceItems.Count, sourceItems.Count);
                return;
            }

            // Apply filters
            var filteredItems = sourceItems.Where(item => FilterConfiguration.MatchesFilters(item)).ToList();
            
            FilteredItemsSource = filteredItems;
            UpdateFilterStatistics(sourceItems.Count, filteredItems.Count);
            
            _logger?.LogDebug("Applied filters: {FilteredCount}/{TotalCount} items visible", 
                filteredItems.Count, sourceItems.Count);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying filters");
            // On error, show all items
            FilteredItemsSource = ItemsSource;
        }
    }

    /// <summary>
    /// Checks if there are any active filters.
    /// </summary>
    private bool HasActiveFilters()
    {
        return FilterConfiguration?.HasActiveFilters == true;
    }

    /// <summary>
    /// Updates filter statistics in the filter panel.
    /// </summary>
    private void UpdateFilterStatistics(int totalCount, int filteredCount)
    {
        try
        {
            if (_filterPanelViewModel != null)
            {
                _filterPanelViewModel.UpdateFilterStatistics(totalCount, filteredCount);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating filter statistics");
        }
    }

    /// <summary>
    /// Updates the state of the Select All checkbox based on current selection.
    /// </summary>
    private void UpdateSelectAllCheckBoxState()
    {
        try
        {
            if (_selectAllCheckBox == null || !IsMultiSelectEnabled || ItemsSource == null)
                return;

            var totalItems = ItemsSource.Cast<object>().Count();
            var selectedCount = SelectedItems?.Count ?? 0;

            if (selectedCount == 0)
            {
                _selectAllCheckBox.IsChecked = false;
            }
            else if (selectedCount == totalItems)
            {
                _selectAllCheckBox.IsChecked = true;
            }
            else
            {
                _selectAllCheckBox.IsChecked = null; // Indeterminate state
            }

            _logger?.LogDebug("Select All checkbox state updated: {State}", _selectAllCheckBox.IsChecked);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating Select All checkbox state");
        }
    }

    #endregion

    #region Cleanup

    /// <summary>
    /// Clean up resources when the control is detached from visual tree.
    /// Following MTM pattern for proper resource disposal.
    /// </summary>
    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        try
        {
            // Unsubscribe from events to prevent memory leaks
            PropertyChanged -= OnPropertyChanged;
            Columns.CollectionChanged -= OnColumnsCollectionChanged;
            
            foreach (var column in Columns)
            {
                column.PropertyChanged -= OnColumnPropertyChanged;
            }
            
            _logger?.LogDebug("CustomDataGrid cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during CustomDataGrid cleanup");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion
}