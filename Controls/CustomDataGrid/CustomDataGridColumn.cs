using System;
using System.ComponentModel;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Defines a column in the CustomDataGrid control.
/// Provides configuration for display, binding, and behavior of individual columns
/// following MTM design patterns and MVVM Community Toolkit integration.
/// </summary>
public class CustomDataGridColumn : INotifyPropertyChanged
{
    private string _propertyName = string.Empty;
    private string _displayName = string.Empty;
    private Type _dataType = typeof(string);
    private bool _isVisible = true;
    private double _width = double.NaN; // Auto width by default
    private bool _canSort = true;
    private bool _canFilter = true;
    private bool _canResize = true;
    private int _displayOrder = 0;
    private bool _isDragging = false;
    private bool _isResizing = false;
    private double _minWidth = 30;
    private double _maxWidth = double.MaxValue;
    
    // Phase 4: Enhanced drag-and-drop and resize state management
    private Avalonia.Point _dragStartPosition = new(0, 0);
    private double _dragOffset = 0;
    private double _resizeStartWidth = 0;
    private bool _isDragValid = false;
    private int _originalDisplayOrder = 0;
    private DataTemplate? _cellTemplate;
    private IValueConverter? _cellConverter;
    private string? _stringFormat;

    /// <summary>
    /// Gets or sets the property name to bind to in the data source.
    /// </summary>
    public string PropertyName
    {
        get => _propertyName;
        set
        {
            if (_propertyName != value)
            {
                _propertyName = value;
                OnPropertyChanged(nameof(PropertyName));
            }
        }
    }

    /// <summary>
    /// Gets or sets the display name shown in the column header.
    /// </summary>
    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (_displayName != value)
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    /// <summary>
    /// Gets or sets the expected data type for this column.
    /// Used for formatting, sorting, and filtering logic.
    /// </summary>
    public Type DataType
    {
        get => _dataType;
        set
        {
            if (_dataType != value)
            {
                _dataType = value;
                OnPropertyChanged(nameof(DataType));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column is visible in the grid.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    /// <summary>
    /// Gets or sets the width of the column.
    /// Use double.NaN for auto-sizing, specific values for fixed width.
    /// </summary>
    public double Width
    {
        get => _width;
        set
        {
            if (!_width.Equals(value))
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column can be sorted by clicking the header.
    /// </summary>
    public bool CanSort
    {
        get => _canSort;
        set
        {
            if (_canSort != value)
            {
                _canSort = value;
                OnPropertyChanged(nameof(CanSort));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column can have filters applied.
    /// </summary>
    public bool CanFilter
    {
        get => _canFilter;
        set
        {
            if (_canFilter != value)
            {
                _canFilter = value;
                OnPropertyChanged(nameof(CanFilter));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column can be resized by the user.
    /// </summary>
    public bool CanResize
    {
        get => _canResize;
        set
        {
            if (_canResize != value)
            {
                _canResize = value;
                OnPropertyChanged(nameof(CanResize));
            }
        }
    }

    /// <summary>
    /// Gets or sets the display order of this column in the grid.
    /// Lower values appear first (left-most). Used for column reordering.
    /// </summary>
    public int DisplayOrder
    {
        get => _displayOrder;
        set
        {
            if (_displayOrder != value)
            {
                _displayOrder = value;
                OnPropertyChanged(nameof(DisplayOrder));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column is currently being dragged.
    /// Used internally for drag-and-drop reordering UI feedback.
    /// </summary>
    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            if (_isDragging != value)
            {
                _isDragging = value;
                OnPropertyChanged(nameof(IsDragging));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether this column is currently being resized.
    /// Used internally for resize operation UI feedback.
    /// </summary>
    public bool IsResizing
    {
        get => _isResizing;
        set
        {
            if (_isResizing != value)
            {
                _isResizing = value;
                OnPropertyChanged(nameof(IsResizing));
            }
        }
    }

    /// <summary>
    /// Gets or sets the minimum width for this column in pixels.
    /// Prevents user from resizing column below this value.
    /// </summary>
    public double MinWidth
    {
        get => _minWidth;
        set
        {
            if (!_minWidth.Equals(value) && value >= 0)
            {
                _minWidth = Math.Max(0, value);
                OnPropertyChanged(nameof(MinWidth));
                
                // Ensure current width respects min width
                if (!double.IsNaN(Width) && Width < _minWidth)
                {
                    Width = _minWidth;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the maximum width for this column in pixels.
    /// Prevents user from resizing column above this value.
    /// </summary>
    public double MaxWidth
    {
        get => _maxWidth;
        set
        {
            if (!_maxWidth.Equals(value) && value > 0)
            {
                _maxWidth = Math.Max(_minWidth, value);
                OnPropertyChanged(nameof(MaxWidth));
                
                // Ensure current width respects max width
                if (!double.IsNaN(Width) && Width > _maxWidth)
                {
                    Width = _maxWidth;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a custom data template for rendering cells in this column.
    /// If null, uses default string representation.
    /// </summary>
    public DataTemplate? CellTemplate
    {
        get => _cellTemplate;
        set
        {
            if (_cellTemplate != value)
            {
                _cellTemplate = value;
                OnPropertyChanged(nameof(CellTemplate));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value converter for transforming data in this column.
    /// </summary>
    public IValueConverter? CellConverter
    {
        get => _cellConverter;
        set
        {
            if (_cellConverter != value)
            {
                _cellConverter = value;
                OnPropertyChanged(nameof(CellConverter));
            }
        }
    }

    /// <summary>
    /// Gets or sets the drag start position for drag-and-drop operations.
    /// Phase 4 feature for visual column reordering.
    /// </summary>
    public Avalonia.Point DragStartPosition
    {
        get => _dragStartPosition;
        set
        {
            if (!_dragStartPosition.Equals(value))
            {
                _dragStartPosition = value;
                OnPropertyChanged(nameof(DragStartPosition));
            }
        }
    }

    /// <summary>
    /// Gets or sets the current drag offset for visual feedback.
    /// Phase 4 feature for drag-and-drop positioning.
    /// </summary>
    public double DragOffset
    {
        get => _dragOffset;
        set
        {
            if (!_dragOffset.Equals(value))
            {
                _dragOffset = value;
                OnPropertyChanged(nameof(DragOffset));
            }
        }
    }

    /// <summary>
    /// Gets or sets the starting width when a resize operation begins.
    /// Phase 4 feature for interactive column resizing.
    /// </summary>
    public double ResizeStartWidth
    {
        get => _resizeStartWidth;
        set
        {
            if (!_resizeStartWidth.Equals(value))
            {
                _resizeStartWidth = value;
                OnPropertyChanged(nameof(ResizeStartWidth));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the current drag operation is valid.
    /// Phase 4 feature for drag-and-drop validation.
    /// </summary>
    public bool IsDragValid
    {
        get => _isDragValid;
        set
        {
            if (_isDragValid != value)
            {
                _isDragValid = value;
                OnPropertyChanged(nameof(IsDragValid));
            }
        }
    }

    /// <summary>
    /// Gets or sets a string format for displaying values in this column.
    /// Only applies when CellTemplate is null.
    /// </summary>
    public string? StringFormat
    {
        get => _stringFormat;
        set
        {
            if (_stringFormat != value)
            {
                _stringFormat = value;
                OnPropertyChanged(nameof(StringFormat));
            }
        }
    }

    /// <summary>
    /// Gets the effective display width for this column.
    /// Returns actual width or a default if auto-sized.
    /// Ensures the value is within MinWidth and MaxWidth constraints.
    /// </summary>
    public double EffectiveWidth 
    {
        get
        {
            var effectiveWidth = double.IsNaN(Width) ? 100 : Width;
            effectiveWidth = Math.Max(MinWidth, effectiveWidth);
            effectiveWidth = Math.Min(MaxWidth, effectiveWidth);
            return effectiveWidth;
        }
    }

    /// <summary>
    /// Gets whether this column can be reordered via drag-and-drop.
    /// Based on whether the column allows reordering (can be extended in future).
    /// </summary>
    public bool CanReorder => true; // For now, all columns can be reordered

    /// <summary>
    /// Gets whether this column width is automatically sized.
    /// </summary>
    public bool IsAutoWidth => double.IsNaN(Width);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Sets the width of this column with constraint validation.
    /// Ensures the new width is within MinWidth and MaxWidth bounds.
    /// </summary>
    /// <param name="newWidth">The desired width</param>
    /// <param name="respectConstraints">Whether to enforce min/max width constraints</param>
    public void SetWidth(double newWidth, bool respectConstraints = true)
    {
        if (respectConstraints)
        {
            newWidth = Math.Max(MinWidth, newWidth);
            newWidth = Math.Min(MaxWidth, newWidth);
        }
        
        Width = newWidth;
    }

    /// <summary>
    /// Moves this column to a new display order position.
    /// </summary>
    /// <param name="newOrder">The new display order</param>
    public void MoveTo(int newOrder)
    {
        DisplayOrder = Math.Max(0, newOrder);
    }

    /// <summary>
    /// Creates a copy of this column with the same settings.
    /// </summary>
    /// <returns>New CustomDataGridColumn instance with copied properties</returns>
    public CustomDataGridColumn Clone()
    {
        return new CustomDataGridColumn
        {
            PropertyName = PropertyName,
            DisplayName = DisplayName,
            DataType = DataType,
            IsVisible = IsVisible,
            Width = Width,
            CanSort = CanSort,
            CanFilter = CanFilter,
            CanResize = CanResize,
            DisplayOrder = DisplayOrder,
            MinWidth = MinWidth,
            MaxWidth = MaxWidth,
            CellTemplate = CellTemplate,
            CellConverter = CellConverter,
            StringFormat = StringFormat
        };
    }

    #region Phase 4: Drag-and-Drop Operations

    /// <summary>
    /// Starts a drag operation for this column.
    /// Phase 4 feature for visual column reordering.
    /// </summary>
    /// <param name="startPosition">The starting position of the drag</param>
    public void StartDrag(Avalonia.Point startPosition)
    {
        DragStartPosition = startPosition;
        _originalDisplayOrder = DisplayOrder;
        IsDragging = true;
        IsDragValid = true;
        DragOffset = 0;
        
        OnPropertyChanged(nameof(IsDragging));
    }

    /// <summary>
    /// Updates the drag position and validates the drag operation.
    /// Phase 4 feature for drag-and-drop feedback.
    /// </summary>
    /// <param name="currentPosition">Current drag position</param>
    /// <returns>True if the drag is valid and should continue</returns>
    public bool UpdateDrag(Avalonia.Point currentPosition)
    {
        if (!IsDragging) return false;

        var deltaX = currentPosition.X - DragStartPosition.X;
        var deltaY = currentPosition.Y - DragStartPosition.Y;
        
        DragOffset = deltaX;
        
        // Basic validation - can be enhanced with drop zone logic
        IsDragValid = Math.Abs(deltaY) < 50; // Allow some vertical tolerance
        
        OnPropertyChanged(nameof(DragOffset));
        OnPropertyChanged(nameof(IsDragValid));
        
        return IsDragValid;
    }

    /// <summary>
    /// Completes or cancels the drag operation.
    /// Phase 4 feature for drag-and-drop completion.
    /// </summary>
    /// <param name="commit">True to commit the drag, false to cancel</param>
    public void CompleteDrag(bool commit = true)
    {
        if (!commit)
        {
            // Restore original order if drag was cancelled
            DisplayOrder = _originalDisplayOrder;
        }
        
        IsDragging = false;
        IsDragValid = false;
        DragOffset = 0;
        DragStartPosition = new Avalonia.Point(0, 0);
        
        OnPropertyChanged(nameof(IsDragging));
        OnPropertyChanged(nameof(IsDragValid));
        OnPropertyChanged(nameof(DragOffset));
        OnPropertyChanged(nameof(DisplayOrder));
    }

    #endregion

    #region Phase 4: Interactive Resize Operations

    /// <summary>
    /// Starts a resize operation for this column.
    /// Phase 4 feature for interactive column resizing.
    /// </summary>
    /// <param name="currentWidth">The current width of the column</param>
    public void StartResize(double currentWidth)
    {
        ResizeStartWidth = double.IsNaN(currentWidth) ? EffectiveWidth : currentWidth;
        IsResizing = true;
        
        OnPropertyChanged(nameof(IsResizing));
        OnPropertyChanged(nameof(ResizeStartWidth));
    }

    /// <summary>
    /// Updates the column width during a resize operation.
    /// Phase 4 feature for interactive resizing with constraints.
    /// </summary>
    /// <param name="deltaWidth">The change in width from the start position</param>
    /// <returns>True if the resize was applied, false if constrained</returns>
    public bool UpdateResize(double deltaWidth)
    {
        if (!IsResizing) return false;
        
        var newWidth = ResizeStartWidth + deltaWidth;
        var constrainedWidth = Math.Max(MinWidth, Math.Min(MaxWidth, newWidth));
        
        if (Math.Abs(constrainedWidth - newWidth) > 0.1)
        {
            // Hit a constraint
            Width = constrainedWidth;
            return false;
        }
        
        Width = constrainedWidth;
        return true;
    }

    /// <summary>
    /// Completes the resize operation.
    /// Phase 4 feature for resize completion.
    /// </summary>
    public void CompleteResize()
    {
        IsResizing = false;
        OnPropertyChanged(nameof(IsResizing));
    }

    #endregion

    public override string ToString()
    {
        return $"{DisplayName} ({PropertyName}) - Order: {DisplayOrder}, Width: {(double.IsNaN(Width) ? "Auto" : Width.ToString("F0"))}, Visible: {IsVisible}";
    }
}