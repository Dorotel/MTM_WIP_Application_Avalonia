using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Configuration model for persisting column settings and layouts.
/// Supports serialization for saving/loading column configurations across sessions.
/// Phase 3 enhancement for MTM Custom Data Grid Control.
/// </summary>
public partial class ColumnConfiguration : INotifyPropertyChanged
{
    #region Properties

    private string _configurationId = string.Empty;
    /// <summary>
    /// Unique identifier for this column configuration.
    /// Used for persistence and configuration management.
    /// </summary>
    public string ConfigurationId
    {
        get => _configurationId;
        set
        {
            if (_configurationId != value)
            {
                _configurationId = value;
                OnPropertyChanged(nameof(ConfigurationId));
            }
        }
    }

    private string _displayName = string.Empty;
    /// <summary>
    /// Display name for this configuration.
    /// Shown in configuration selection UI.
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

    private string _description = string.Empty;
    /// <summary>
    /// Description of this configuration.
    /// Optional field for user notes and configuration details.
    /// </summary>
    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    private List<ColumnSettings> _columnSettings = new();
    /// <summary>
    /// Collection of column settings for each defined column.
    /// Maintains order, visibility, width, and other column properties.
    /// </summary>
    public List<ColumnSettings> ColumnSettings
    {
        get => _columnSettings;
        set
        {
            if (_columnSettings != value)
            {
                _columnSettings = value;
                OnPropertyChanged(nameof(ColumnSettings));
            }
        }
    }

    private DateTime _createdAt = DateTime.Now;
    /// <summary>
    /// Timestamp when this configuration was created.
    /// </summary>
    public DateTime CreatedAt
    {
        get => _createdAt;
        set
        {
            if (_createdAt != value)
            {
                _createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }
    }

    private DateTime _lastModified = DateTime.Now;
    /// <summary>
    /// Timestamp when this configuration was last modified.
    /// </summary>
    public DateTime LastModified
    {
        get => _lastModified;
        set
        {
            if (_lastModified != value)
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
            }
        }
    }

    private string _createdBy = Environment.UserName;
    /// <summary>
    /// User ID who created this configuration.
    /// </summary>
    public string CreatedBy
    {
        get => _createdBy;
        set
        {
            if (_createdBy != value)
            {
                _createdBy = value;
                OnPropertyChanged(nameof(CreatedBy));
            }
        }
    }

    private bool _isDefault = false;
    /// <summary>
    /// Whether this is the default configuration for the grid.
    /// </summary>
    public bool IsDefault
    {
        get => _isDefault;
        set
        {
            if (_isDefault != value)
            {
                _isDefault = value;
                OnPropertyChanged(nameof(IsDefault));
            }
        }
    }

    private int _version = 1;
    /// <summary>
    /// Version number for configuration schema compatibility.
    /// </summary>
    public int Version
    {
        get => _version;
        set
        {
            if (_version != value)
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the ColumnConfiguration class.
    /// </summary>
    public ColumnConfiguration()
    {
        ConfigurationId = Guid.NewGuid().ToString();
        DisplayName = "Default Configuration";
        Description = "Auto-generated default column configuration";
    }

    /// <summary>
    /// Initializes a new instance with specific configuration ID and name.
    /// </summary>
    /// <param name="configurationId">Unique identifier for this configuration</param>
    /// <param name="displayName">Display name for this configuration</param>
    public ColumnConfiguration(string configurationId, string displayName)
    {
        ConfigurationId = configurationId;
        DisplayName = displayName;
        Description = $"Column configuration: {displayName}";
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a column configuration from a collection of CustomDataGridColumn objects.
    /// </summary>
    /// <param name="columns">The columns to create configuration from</param>
    /// <param name="configurationName">Name for the new configuration</param>
    /// <returns>New ColumnConfiguration instance</returns>
    public static ColumnConfiguration FromColumns(IEnumerable<CustomDataGridColumn> columns, string configurationName = "Custom Configuration")
    {
        var config = new ColumnConfiguration
        {
            DisplayName = configurationName,
            Description = $"Configuration created from current column layout"
        };

        var order = 0;
        foreach (var column in columns)
        {
            config.ColumnSettings.Add(new ColumnSettings
            {
                PropertyName = column.PropertyName,
                DisplayName = column.DisplayName,
                IsVisible = column.IsVisible,
                Width = column.Width,
                Order = order++,
                CanResize = column.CanResize,
                CanSort = column.CanSort,
                CanFilter = column.CanFilter,
                DataType = column.DataType.FullName ?? "System.String",
                StringFormat = column.StringFormat
            });
        }

        return config;
    }

    /// <summary>
    /// Applies this configuration to a collection of CustomDataGridColumn objects.
    /// </summary>
    /// <param name="columns">The columns to apply configuration to</param>
    public void ApplyToColumns(IList<CustomDataGridColumn> columns)
    {
        // Create a lookup for existing columns
        var columnLookup = columns.ToDictionary(c => c.PropertyName, c => c);

        // Apply settings from configuration
        foreach (var setting in ColumnSettings.OrderBy(s => s.Order))
        {
            if (columnLookup.TryGetValue(setting.PropertyName, out var column))
            {
                column.DisplayName = setting.DisplayName;
                column.IsVisible = setting.IsVisible;
                column.Width = setting.Width;
                column.CanResize = setting.CanResize;
                column.CanSort = setting.CanSort;
                column.CanFilter = setting.CanFilter;
                column.StringFormat = setting.StringFormat;

                // Apply data type if available
                if (!string.IsNullOrEmpty(setting.DataType))
                {
                    try
                    {
                        column.DataType = Type.GetType(setting.DataType) ?? typeof(string);
                    }
                    catch
                    {
                        column.DataType = typeof(string);
                    }
                }
            }
        }

        // Reorder columns based on configuration
        var orderedColumns = ColumnSettings
            .OrderBy(s => s.Order)
            .Select(s => columnLookup.TryGetValue(s.PropertyName, out var col) ? col : null)
            .Where(c => c != null)
            .ToList();

        columns.Clear();
        foreach (var column in orderedColumns)
        {
            if (column != null)
            {
                columns.Add(column);
            }
        }
    }

    /// <summary>
    /// Updates the last modified timestamp.
    /// </summary>
    public void UpdateLastModified()
    {
        LastModified = DateTime.Now;
    }

    /// <summary>
    /// Creates a deep copy of this configuration.
    /// </summary>
    /// <returns>New ColumnConfiguration instance with copied values</returns>
    public ColumnConfiguration Clone()
    {
        var clone = new ColumnConfiguration
        {
            ConfigurationId = Guid.NewGuid().ToString(), // New ID for clone
            DisplayName = $"{DisplayName} (Copy)",
            Description = Description,
            CreatedBy = Environment.UserName,
            IsDefault = false, // Clones are never default
            Version = Version
        };

        // Deep copy column settings
        foreach (var setting in ColumnSettings)
        {
            clone.ColumnSettings.Add(setting.Clone());
        }

        return clone;
    }

    /// <summary>
    /// Validates this configuration for completeness and consistency.
    /// </summary>
    /// <returns>True if configuration is valid, false otherwise</returns>
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(ConfigurationId) || 
            string.IsNullOrWhiteSpace(DisplayName) ||
            ColumnSettings.Count == 0)
        {
            return false;
        }

        // Check for duplicate property names
        var propertyNames = ColumnSettings.Select(s => s.PropertyName).ToList();
        if (propertyNames.Count != propertyNames.Distinct().Count())
        {
            return false;
        }

        // Check for valid order values
        var orders = ColumnSettings.Select(s => s.Order).ToList();
        if (orders.Any(o => o < 0))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}

/// <summary>
/// Settings for an individual column within a ColumnConfiguration.
/// Stores all properties needed to recreate column state.
/// </summary>
public partial class ColumnSettings : INotifyPropertyChanged
{
    private string _propertyName = string.Empty;
    /// <summary>
    /// Property name that this column binds to.
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

    private string _displayName = string.Empty;
    /// <summary>
    /// Display name shown in column header.
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

    private bool _isVisible = true;
    /// <summary>
    /// Whether this column is visible.
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

    private double _width = double.NaN;
    /// <summary>
    /// Column width. Use double.NaN for auto-sizing.
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

    private int _order = 0;
    /// <summary>
    /// Display order of this column (0-based).
    /// </summary>
    public int Order
    {
        get => _order;
        set
        {
            if (_order != value)
            {
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }
    }

    private bool _canResize = true;
    /// <summary>
    /// Whether this column can be resized by user.
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

    private bool _canSort = true;
    /// <summary>
    /// Whether this column can be sorted.
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

    private bool _canFilter = true;
    /// <summary>
    /// Whether this column can be filtered.
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

    private string _dataType = "System.String";
    /// <summary>
    /// Data type name for this column.
    /// </summary>
    public string DataType
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

    private string? _stringFormat;
    /// <summary>
    /// String format for displaying values.
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
    /// Creates a deep copy of this column setting.
    /// </summary>
    /// <returns>New ColumnSettings instance with copied values</returns>
    public ColumnSettings Clone()
    {
        return new ColumnSettings
        {
            PropertyName = PropertyName,
            DisplayName = DisplayName,
            IsVisible = IsVisible,
            Width = Width,
            Order = Order,
            CanResize = CanResize,
            CanSort = CanSort,
            CanFilter = CanFilter,
            DataType = DataType,
            StringFormat = StringFormat
        };
    }

    public override string ToString()
    {
        return $"{DisplayName} ({PropertyName}) - Order: {Order}, Visible: {IsVisible}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}