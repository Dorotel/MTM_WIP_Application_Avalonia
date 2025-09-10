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
    /// </summary>
    public double EffectiveWidth => double.IsNaN(Width) ? 100 : Width;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"{DisplayName} ({PropertyName})";
    }
}