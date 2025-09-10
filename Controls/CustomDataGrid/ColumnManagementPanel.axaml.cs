using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Input;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Column Management Panel - Phase 3 Implementation
/// 
/// Provides comprehensive column management UI for the MTM Custom Data Grid.
/// Features include show/hide columns, reordering, resizing, and configuration management.
/// Follows MTM design patterns with proper theme integration and MVVM binding.
/// 
/// Key Features:
/// - Interactive column visibility toggles
/// - Drag-and-drop reordering with visual feedback
/// - Column width management with constraints
/// - Saved configuration management with persistence
/// - Quick actions for common operations
/// - Full accessibility support
/// </summary>
public partial class ColumnManagementPanel : UserControl
{
    #region Dependency Properties

    /// <summary>
    /// Gets or sets the columns collection to manage.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<CustomDataGridColumn>?> ColumnsProperty =
        AvaloniaProperty.Register<ColumnManagementPanel, ObservableCollection<CustomDataGridColumn>?>(
            nameof(Columns), null, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the collection of saved column configurations.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<ColumnConfiguration>?> SavedConfigurationsProperty =
        AvaloniaProperty.Register<ColumnManagementPanel, ObservableCollection<ColumnConfiguration>?>(
            nameof(SavedConfigurations), null, defaultBindingMode: Avalonia.Data.BindingMode.OneWay);

    /// <summary>
    /// Gets or sets the currently selected configuration.
    /// </summary>
    public static readonly StyledProperty<ColumnConfiguration?> SelectedConfigurationProperty =
        AvaloniaProperty.Register<ColumnManagementPanel, ColumnConfiguration?>(
            nameof(SelectedConfiguration), null, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    #endregion

    #region Properties

    public ObservableCollection<CustomDataGridColumn>? Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public ObservableCollection<ColumnConfiguration>? SavedConfigurations
    {
        get => GetValue(SavedConfigurationsProperty);
        set => SetValue(SavedConfigurationsProperty, value);
    }

    public ColumnConfiguration? SelectedConfiguration
    {
        get => GetValue(SelectedConfigurationProperty);
        set => SetValue(SelectedConfigurationProperty, value);
    }

    /// <summary>
    /// Gets the count of visible columns for display in UI.
    /// </summary>
    public int VisibleColumnCount => Columns?.Count(c => c.IsVisible) ?? 0;

    #endregion

    #region Fields

    private readonly ILogger? _logger;
    
    // Phase 4: Enhanced drag-and-drop state management
    private bool _isDragging = false;
    private CustomDataGridColumn? _draggedColumn = null;
    private Point _dragStartPosition = new(0, 0);
    private Border? _dragPreview = null;
    private Grid? _dropIndicator = null;
    private int _dropTargetIndex = -1;
    private bool _isResizing = false;
    private CustomDataGridColumn? _resizeColumn = null;
    private double _resizeStartX = 0;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the panel is requested to be closed.
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// Occurs when columns have been modified and the parent grid should refresh.
    /// </summary>
    public event EventHandler? ColumnsModified;

    /// <summary>
    /// Occurs when a configuration should be saved.
    /// </summary>
    public event EventHandler<ColumnConfiguration>? ConfigurationSaveRequested;

    /// <summary>
    /// Occurs when a configuration should be loaded.
    /// </summary>
    public event EventHandler<ColumnConfiguration>? ConfigurationLoadRequested;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the ColumnManagementPanel.
    /// Sets up the UI and binds to column change events.
    /// </summary>
    public ColumnManagementPanel()
    {
        try
        {
            _logger = null; // DI integration will be handled by consumers
        }
        catch
        {
            _logger = null; // Graceful fallback for design-time
        }

        InitializeComponent();

        // Set up property change handlers
        // PropertyChanged += OnPropertyChanged; // Not needed for UserControl

        _logger?.LogDebug("ColumnManagementPanel initialized");
    }

    #endregion

    #region Initialization

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        
        _logger?.LogDebug("ColumnManagementPanel components initialized");
    }

    #endregion

    #region Event Handlers

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            if (e.Property == ColumnsProperty)
            {
                HandleColumnsChanged();
            }
            else if (e.Property == SelectedConfigurationProperty)
            {
                _logger?.LogDebug("Selected configuration changed: {ConfigName}", SelectedConfiguration?.DisplayName ?? "None");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling property change for {PropertyName}", e.Property.Name);
        }
    }

    private void HandleColumnsChanged()
    {
        // Subscribe to column property changes for UI updates
        if (Columns != null)
        {
            foreach (var column in Columns)
            {
                column.PropertyChanged -= OnColumnPropertyChanged;
                column.PropertyChanged += OnColumnPropertyChanged;
            }

            Columns.CollectionChanged -= OnColumnsCollectionChanged;
            Columns.CollectionChanged += OnColumnsCollectionChanged;
        }

        // OnPropertyChanged(nameof(VisibleColumnCount)); // TODO: Implement property notification
        _logger?.LogDebug("Columns collection updated, count: {Count}", Columns?.Count ?? 0);
    }

    private void OnColumnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CustomDataGridColumn.IsVisible))
        {
            // OnPropertyChanged(nameof(VisibleColumnCount)); // TODO: Implement property notification
            NotifyColumnsModified();
        }
        else if (e.PropertyName == nameof(CustomDataGridColumn.Width) ||
                 e.PropertyName == nameof(CustomDataGridColumn.DisplayOrder))
        {
            NotifyColumnsModified();
        }
    }

    private void OnColumnsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // OnPropertyChanged(nameof(VisibleColumnCount)); // TODO: Implement property notification
        NotifyColumnsModified();
    }

    #endregion

    #region Quick Actions Event Handlers

    /// <summary>
    /// Shows all columns by setting IsVisible to true.
    /// </summary>
    public void OnShowAllColumns(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Columns == null) return;

            foreach (var column in Columns)
            {
                column.IsVisible = true;
            }

            _logger?.LogDebug("All columns set to visible");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error showing all columns");
        }
    }

    /// <summary>
    /// Hides all columns by setting IsVisible to false.
    /// </summary>
    public void OnHideAllColumns(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Columns == null) return;

            foreach (var column in Columns)
            {
                column.IsVisible = false;
            }

            _logger?.LogDebug("All columns set to hidden");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error hiding all columns");
        }
    }

    /// <summary>
    /// Resets column display order to the original sequence.
    /// </summary>
    public void OnResetColumnOrder(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Columns == null) return;

            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].DisplayOrder = i;
            }

            // Re-sort the collection by display order
            var sortedColumns = Columns.OrderBy(c => c.DisplayOrder).ToList();
            Columns.Clear();
            foreach (var column in sortedColumns)
            {
                Columns.Add(column);
            }

            _logger?.LogDebug("Column order reset to default");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resetting column order");
        }
    }

    /// <summary>
    /// Auto-sizes all columns by setting width to NaN (auto).
    /// </summary>
    public void OnAutoSizeColumns(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Columns == null) return;

            foreach (var column in Columns)
            {
                column.Width = double.NaN; // Auto-size
            }

            _logger?.LogDebug("All columns set to auto-size");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error auto-sizing columns");
        }
    }

    /// <summary>
    /// Closes the management panel.
    /// </summary>
    public void OnClosePanel(object? sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Column Action Event Handlers

    /// <summary>
    /// Decreases the width of the specified column.
    /// </summary>
    public void OnDecreaseWidth(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is CustomDataGridColumn column)
            {
                var currentWidth = double.IsNaN(column.Width) ? column.EffectiveWidth : column.Width;
                var newWidth = Math.Max(column.MinWidth, currentWidth - 10);
                column.SetWidth(newWidth);
                
                _logger?.LogDebug("Decreased width for column {ColumnName}: {Width}", column.DisplayName, newWidth);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error decreasing column width");
        }
    }

    /// <summary>
    /// Increases the width of the specified column.
    /// </summary>
    public void OnIncreaseWidth(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is CustomDataGridColumn column)
            {
                var currentWidth = double.IsNaN(column.Width) ? column.EffectiveWidth : column.Width;
                var newWidth = Math.Min(column.MaxWidth, currentWidth + 10);
                column.SetWidth(newWidth);
                
                _logger?.LogDebug("Increased width for column {ColumnName}: {Width}", column.DisplayName, newWidth);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error increasing column width");
        }
    }

    /// <summary>
    /// Moves the specified column one position to the left.
    /// </summary>
    public void OnMoveLeft(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is CustomDataGridColumn column && Columns != null)
            {
                var currentIndex = Columns.IndexOf(column);
                if (currentIndex > 0)
                {
                    Columns.Move(currentIndex, currentIndex - 1);
                    UpdateDisplayOrders();
                    _logger?.LogDebug("Moved column {ColumnName} left", column.DisplayName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error moving column left");
        }
    }

    /// <summary>
    /// Moves the specified column one position to the right.
    /// </summary>
    public void OnMoveRight(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is CustomDataGridColumn column && Columns != null)
            {
                var currentIndex = Columns.IndexOf(column);
                if (currentIndex < Columns.Count - 1)
                {
                    Columns.Move(currentIndex, currentIndex + 1);
                    UpdateDisplayOrders();
                    _logger?.LogDebug("Moved column {ColumnName} right", column.DisplayName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error moving column right");
        }
    }

    /// <summary>
    /// Resets the width of the specified column to auto-size.
    /// </summary>
    public void OnResetColumnWidth(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MenuItem menuItem && menuItem.Tag is CustomDataGridColumn column)
            {
                column.Width = double.NaN; // Auto-size
                _logger?.LogDebug("Reset width for column {ColumnName}", column.DisplayName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error resetting column width");
        }
    }

    /// <summary>
    /// Shows dialog to set a fixed width for the specified column.
    /// </summary>
    public void OnSetFixedWidth(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MenuItem menuItem && menuItem.Tag is CustomDataGridColumn column)
            {
                // For now, set to a reasonable fixed width (can be enhanced with dialog later)
                var currentWidth = double.IsNaN(column.Width) ? column.EffectiveWidth : column.Width;
                column.SetWidth(currentWidth); // Convert auto to fixed
                
                _logger?.LogDebug("Set fixed width for column {ColumnName}: {Width}", column.DisplayName, currentWidth);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error setting fixed column width");
        }
    }

    /// <summary>
    /// Moves the specified column to the start position.
    /// </summary>
    public void OnMoveToStart(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MenuItem menuItem && menuItem.Tag is CustomDataGridColumn column && Columns != null)
            {
                var currentIndex = Columns.IndexOf(column);
                if (currentIndex > 0)
                {
                    Columns.Move(currentIndex, 0);
                    UpdateDisplayOrders();
                    _logger?.LogDebug("Moved column {ColumnName} to start", column.DisplayName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error moving column to start");
        }
    }

    /// <summary>
    /// Moves the specified column to the end position.
    /// </summary>
    public void OnMoveToEnd(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is MenuItem menuItem && menuItem.Tag is CustomDataGridColumn column && Columns != null)
            {
                var currentIndex = Columns.IndexOf(column);
                if (currentIndex < Columns.Count - 1)
                {
                    Columns.Move(currentIndex, Columns.Count - 1);
                    UpdateDisplayOrders();
                    _logger?.LogDebug("Moved column {ColumnName} to end", column.DisplayName);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error moving column to end");
        }
    }

    #endregion

    #region Configuration Event Handlers

    /// <summary>
    /// Loads the selected column configuration.
    /// </summary>
    public void OnLoadConfiguration(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (SelectedConfiguration != null)
            {
                ConfigurationLoadRequested?.Invoke(this, SelectedConfiguration);
                _logger?.LogDebug("Configuration load requested: {ConfigName}", SelectedConfiguration.DisplayName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error loading configuration");
        }
    }

    /// <summary>
    /// Saves the current column configuration.
    /// </summary>
    public void OnSaveConfiguration(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (Columns != null)
            {
                var config = ColumnConfiguration.FromColumns(Columns, "Custom Configuration");
                ConfigurationSaveRequested?.Invoke(this, config);
                _logger?.LogDebug("Configuration save requested");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error saving configuration");
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Updates the DisplayOrder property for all columns based on their current position.
    /// </summary>
    private void UpdateDisplayOrders()
    {
        if (Columns == null) return;

        for (int i = 0; i < Columns.Count; i++)
        {
            Columns[i].DisplayOrder = i;
        }
    }

    /// <summary>
    /// Notifies that columns have been modified and parent should refresh.
    /// </summary>
    private void NotifyColumnsModified()
    {
        ColumnsModified?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Applies a column configuration to the current columns.
    /// </summary>
    /// <param name="configuration">The configuration to apply</param>
    public void ApplyConfiguration(ColumnConfiguration configuration)
    {
        try
        {
            if (Columns != null)
            {
                configuration.ApplyToColumns(Columns);
                // OnPropertyChanged(nameof(VisibleColumnCount)); // TODO: Implement property notification
                NotifyColumnsModified();
                
                _logger?.LogDebug("Applied configuration: {ConfigName}", configuration.DisplayName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying configuration");
        }
    }

    /// <summary>
    /// Gets the current column configuration.
    /// </summary>
    /// <returns>Current column configuration</returns>
    public ColumnConfiguration? GetCurrentConfiguration()
    {
        if (Columns == null || Columns.Count == 0)
            return null;

        return ColumnConfiguration.FromColumns(Columns, "Current Layout");
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
            // PropertyChanged -= OnPropertyChanged; // Not needed for UserControl
            
            if (Columns != null)
            {
                Columns.CollectionChanged -= OnColumnsCollectionChanged;
                foreach (var column in Columns)
                {
                    column.PropertyChanged -= OnColumnPropertyChanged;
                }
            }
            
            // Clean up any drag operations
            CleanupDragOperation();
            
            _logger?.LogDebug("ColumnManagementPanel cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during ColumnManagementPanel cleanup");
        }
        finally
        {
            base.OnDetachedFromVisualTree(e);
        }
    }

    #endregion

    #region Phase 4: Drag-and-Drop Event Handlers

    /// <summary>
    /// Handles the start of a drag operation on a drag handle.
    /// Phase 4 feature for visual column reordering.
    /// </summary>
    private void OnDragHandlePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        try
        {
            if (sender is Button dragHandle && 
                dragHandle.Parent is Grid columnGrid && 
                columnGrid.DataContext is CustomDataGridColumn column)
            {
                _isDragging = true;
                _draggedColumn = column;
                _dragStartPosition = e.GetPosition(this);

                // Start visual drag operation
                column.StartDrag(_dragStartPosition);
                
                // Create drag preview
                CreateDragPreview(column, columnGrid);
                
                // Set up pointer capture for drag tracking - simplified for Avalonia
                // dragHandle.CapturePointer(e.Pointer);
                
                _logger?.LogDebug("Started drag operation for column: {ColumnName}", column.DisplayName);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error starting drag operation");
        }
    }

    /// <summary>
    /// Handles pointer movement during drag operation.
    /// Provides visual feedback and drop target indication.
    /// </summary>
    private void OnDragHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        try
        {
            if (_isDragging && _draggedColumn != null && sender is Button dragHandle)
            {
                var currentPosition = e.GetPosition(this);
                
                // Update column drag state
                _draggedColumn.UpdateDrag(currentPosition);
                
                // Update drag preview position
                UpdateDragPreview(currentPosition);
                
                // Update drop target indication
                UpdateDropTarget(currentPosition);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during drag move");
        }
    }

    /// <summary>
    /// Handles the end of a drag operation.
    /// Commits or cancels the column reordering based on drop target.
    /// </summary>
    private void OnDragHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        try
        {
            if (_isDragging && _draggedColumn != null && sender is Button dragHandle)
            {
                // dragHandle.ReleasePointerCapture(e.Pointer);
                
                var shouldCommit = _dropTargetIndex >= 0 && _dropTargetIndex != Columns?.IndexOf(_draggedColumn);
                
                if (shouldCommit && Columns != null)
                {
                    // Perform the reorder operation
                    var currentIndex = Columns.IndexOf(_draggedColumn);
                    if (currentIndex >= 0 && _dropTargetIndex >= 0 && _dropTargetIndex < Columns.Count)
                    {
                        Columns.Move(currentIndex, _dropTargetIndex);
                        UpdateDisplayOrders();
                        
                        _logger?.LogDebug("Moved column {ColumnName} from index {FromIndex} to {ToIndex}",
                            _draggedColumn.DisplayName, currentIndex, _dropTargetIndex);
                    }
                }
                
                // Complete drag operation
                _draggedColumn.CompleteDrag(shouldCommit);
                
                // Clean up drag visuals
                CleanupDragOperation();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error completing drag operation");
            CleanupDragOperation();
        }
    }

    /// <summary>
    /// Creates a visual preview for the dragged column.
    /// </summary>
    private void CreateDragPreview(CustomDataGridColumn column, Grid originalGrid)
    {
        try
        {
            _dragPreview = new Border
            {
                Background = Avalonia.Media.Brushes.LightBlue,
                BorderBrush = Avalonia.Media.Brushes.Blue,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Opacity = 0.8,
                Child = new TextBlock
                {
                    Text = column.DisplayName,
                    Padding = new Thickness(8, 4),
                    Foreground = Avalonia.Media.Brushes.DarkBlue,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                }
            };

            // Add to visual tree (will be positioned during drag)
            if (this.Parent is Grid parentGrid)
            {
                parentGrid.Children.Add(_dragPreview);
                // Grid.SetZIndex(_dragPreview, 1000); // Not available in current Avalonia version
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error creating drag preview");
        }
    }

    /// <summary>
    /// Updates the position of the drag preview to follow the pointer.
    /// </summary>
    private void UpdateDragPreview(Point currentPosition)
    {
        try
        {
            if (_dragPreview != null)
            {
                var offset = currentPosition - _dragStartPosition;
                Canvas.SetLeft(_dragPreview, offset.X);
                Canvas.SetTop(_dragPreview, offset.Y);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating drag preview");
        }
    }

    /// <summary>
    /// Updates the drop target indication based on current pointer position.
    /// </summary>
    private void UpdateDropTarget(Point currentPosition)
    {
        try
        {
            if (Columns == null) return;

            // Simple drop target calculation (can be enhanced with visual indicators)
            var columnItemsControl = this.FindControl<ItemsControl>("ColumnItemsControl");
            if (columnItemsControl != null)
            {
                // Find which column item the pointer is over
                var targetIndex = -1;
                
                // This is a simplified version - a full implementation would check visual bounds
                var relativeY = currentPosition.Y - _dragStartPosition.Y;
                var itemHeight = 40; // Approximate height of column items
                var indexOffset = (int)(relativeY / itemHeight);
                
                if (_draggedColumn != null)
                {
                    var currentIndex = Columns.IndexOf(_draggedColumn);
                    targetIndex = Math.Max(0, Math.Min(Columns.Count - 1, currentIndex + indexOffset));
                }

                _dropTargetIndex = targetIndex;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating drop target");
        }
    }

    /// <summary>
    /// Cleans up drag operation visuals and state.
    /// </summary>
    private void CleanupDragOperation()
    {
        try
        {
            _isDragging = false;
            _draggedColumn = null;
            _dropTargetIndex = -1;
            _dragStartPosition = new Point(0, 0);

            // Remove drag preview
            if (_dragPreview != null && this.Parent is Grid parentGrid)
            {
                parentGrid.Children.Remove(_dragPreview);
                _dragPreview = null;
            }

            // Remove drop indicator
            if (_dropIndicator != null && this.Parent is Grid parentGrid2)
            {
                parentGrid2.Children.Remove(_dropIndicator);
                _dropIndicator = null;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error cleaning up drag operation");
        }
    }

    #endregion
}