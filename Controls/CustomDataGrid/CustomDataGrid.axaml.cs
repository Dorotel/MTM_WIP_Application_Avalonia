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
    /// Gets or sets the row height for data rows.
    /// </summary>
    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<CustomDataGrid, double>(nameof(RowHeight), 28.0);

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

    /// <summary>
    /// Gets the collection of visible columns (filtered from Columns where IsVisible = true).
    /// </summary>
    public ObservableCollection<CustomDataGridColumn> VisibleColumns { get; private set; }

    #endregion

    #region Fields

    private ListBox? _dataListBox;
    private ScrollViewer? _dataScrollViewer;
    private ItemsControl? _headerItemsControl;
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
        
        _logger?.LogDebug("CustomDataGrid components initialized");
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