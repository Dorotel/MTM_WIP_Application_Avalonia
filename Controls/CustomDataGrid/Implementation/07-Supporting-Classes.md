# CustomDataGrid - Supporting Classes Implementation

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🏗️ Supporting Classes Implementation

The CustomDataGrid requires several supporting classes to manage column configuration, filtering, selection, and overall functionality.

## CustomDataGridColumn.cs

### Column Definition and Metadata
```csharp
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Represents a column definition for the CustomDataGrid control
    /// </summary>
    public class CustomDataGridColumn : INotifyPropertyChanged
    {
        private string _header = string.Empty;
        private string _propertyName = string.Empty;
        private double _width = 100.0;
        private bool _isVisible = true;
        private bool _isSortable = true;
        private bool _isResizable = true;
        private string _stringFormat = string.Empty;
        private HorizontalAlignment _textAlignment = HorizontalAlignment.Left;
        private int _displayIndex = 0;

        /// <summary>
        /// Gets or sets the display text for the column header
        /// </summary>
        [Required(ErrorMessage = "Header is required")]
        public string Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the property name to bind to in the data source
        /// </summary>
        [Required(ErrorMessage = "PropertyName is required")]
        public string PropertyName
        {
            get => _propertyName;
            set
            {
                if (_propertyName != value)
                {
                    _propertyName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the column (supports star sizing like 1.5* or fixed pixel values)
        /// </summary>
        public double Width
        {
            get => _width;
            set
            {
                if (Math.Abs(_width - value) > 0.01)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the column is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the column can be sorted
        /// </summary>
        public bool IsSortable
        {
            get => _isSortable;
            set
            {
                if (_isSortable != value)
                {
                    _isSortable = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the column can be resized by the user
        /// </summary>
        public bool IsResizable
        {
            get => _isResizable;
            set
            {
                if (_isResizable != value)
                {
                    _isResizable = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the string format for displaying values (e.g., "N0" for numbers, "MM/dd/yy" for dates)
        /// </summary>
        public string StringFormat
        {
            get => _stringFormat;
            set
            {
                if (_stringFormat != value)
                {
                    _stringFormat = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment for the column content
        /// </summary>
        public HorizontalAlignment TextAlignment
        {
            get => _textAlignment;
            set
            {
                if (_textAlignment != value)
                {
                    _textAlignment = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the display order index of the column
        /// </summary>
        public int DisplayIndex
        {
            get => _displayIndex;
            set
            {
                if (_displayIndex != value)
                {
                    _displayIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column type for specialized rendering
        /// </summary>
        public CustomDataGridColumnType ColumnType { get; set; } = CustomDataGridColumnType.Text;

        /// <summary>
        /// Gets or sets whether this is a fixed-width column (pixels) or proportional (star sizing)
        /// </summary>
        public bool IsFixedWidth { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Creates a default column configuration for inventory data
        /// </summary>
        public static CustomDataGridColumn CreateDefault(string header, string propertyName, double width = 1.0)
        {
            return new CustomDataGridColumn
            {
                Header = header,
                PropertyName = propertyName,
                Width = width,
                IsVisible = true,
                IsSortable = true,
                IsResizable = true
            };
        }
    }

    /// <summary>
    /// Specifies the type of column for specialized rendering
    /// </summary>
    public enum CustomDataGridColumnType
    {
        Text,
        Number,
        DateTime,
        Boolean,
        Checkbox,
        Actions,
        Notes
    }

    /// <summary>
    /// Specifies horizontal alignment options
    /// </summary>
    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }
}
```

## ColumnConfiguration.cs

### Column Management and Persistence
```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Manages column configuration state for the CustomDataGrid
    /// Supports saving/loading column visibility, order, and sizing
    /// </summary>
    public class ColumnConfiguration : INotifyPropertyChanged
    {
        private ObservableCollection<CustomDataGridColumn> _columns = new();
        private string _configurationName = string.Empty;
        private DateTime _lastSaved = DateTime.MinValue;
        private string _savedBy = string.Empty;

        /// <summary>
        /// Gets or sets the collection of column definitions
        /// </summary>
        public ObservableCollection<CustomDataGridColumn> Columns
        {
            get => _columns;
            set
            {
                if (_columns != value)
                {
                    if (_columns != null)
                    {
                        _columns.CollectionChanged -= OnColumnsCollectionChanged;
                    }

                    _columns = value;

                    if (_columns != null)
                    {
                        _columns.CollectionChanged += OnColumnsCollectionChanged;
                    }

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this column configuration (for saving/loading)
        /// </summary>
        public string ConfigurationName
        {
            get => _configurationName;
            set
            {
                if (_configurationName != value)
                {
                    _configurationName = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets when this configuration was last saved
        /// </summary>
        public DateTime LastSaved
        {
            get => _lastSaved;
            set
            {
                if (_lastSaved != value)
                {
                    _lastSaved = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets who saved this configuration
        /// </summary>
        public string SavedBy
        {
            get => _savedBy;
            set
            {
                if (_savedBy != value)
                {
                    _savedBy = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether this configuration has any unsaved changes
        /// </summary>
        [JsonIgnore]
        public bool HasUnsavedChanges { get; private set; }

        public ColumnConfiguration()
        {
            InitializeDefaultColumns();
        }

        /// <summary>
        /// Initializes the default column configuration for inventory data
        /// </summary>
        private void InitializeDefaultColumns()
        {
            Columns = new ObservableCollection<CustomDataGridColumn>
            {
                new() { Header = "Select", PropertyName = "IsSelected", Width = 40, IsFixedWidth = true, ColumnType = CustomDataGridColumnType.Checkbox, DisplayIndex = 0 },
                new() { Header = "Part ID", PropertyName = "PartId", Width = 1.5, TextAlignment = HorizontalAlignment.Left, DisplayIndex = 1 },
                new() { Header = "Operation", PropertyName = "Operation", Width = 1.0, TextAlignment = HorizontalAlignment.Center, DisplayIndex = 2 },
                new() { Header = "Location", PropertyName = "Location", Width = 1.2, TextAlignment = HorizontalAlignment.Left, DisplayIndex = 3 },
                new() { Header = "Quantity", PropertyName = "Quantity", Width = 1.0, TextAlignment = HorizontalAlignment.Right, StringFormat = "N0", ColumnType = CustomDataGridColumnType.Number, DisplayIndex = 4 },
                new() { Header = "Last Updated", PropertyName = "LastUpdated", Width = 1.8, StringFormat = "MM/dd/yy HH:mm", ColumnType = CustomDataGridColumnType.DateTime, DisplayIndex = 5 },
                new() { Header = "Notes", PropertyName = "HasNotes", Width = 80, IsFixedWidth = true, TextAlignment = HorizontalAlignment.Center, ColumnType = CustomDataGridColumnType.Notes, DisplayIndex = 6 },
                new() { Header = "Actions", PropertyName = "", Width = 100, IsFixedWidth = true, IsSortable = false, ColumnType = CustomDataGridColumnType.Actions, DisplayIndex = 7 },
                new() { Header = "", PropertyName = "", Width = 40, IsFixedWidth = true, IsSortable = false, IsResizable = false, DisplayIndex = 8 }
            };

            ConfigurationName = "Default";
            LastSaved = DateTime.Now;
            SavedBy = Environment.UserName;
        }

        /// <summary>
        /// Gets the visible columns in display order
        /// </summary>
        public IEnumerable<CustomDataGridColumn> GetVisibleColumns()
        {
            return Columns.Where(c => c.IsVisible).OrderBy(c => c.DisplayIndex);
        }

        /// <summary>
        /// Moves a column to a new position
        /// </summary>
        public void MoveColumn(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= Columns.Count || newIndex < 0 || newIndex >= Columns.Count)
                return;

            var column = Columns[oldIndex];
            Columns.RemoveAt(oldIndex);
            Columns.Insert(newIndex, column);

            // Update display indices
            for (int i = 0; i < Columns.Count; i++)
            {
                Columns[i].DisplayIndex = i;
            }

            MarkAsChanged();
        }

        /// <summary>
        /// Resets column widths to their default values
        /// </summary>
        public void ResetColumnWidths()
        {
            var defaultConfig = new ColumnConfiguration();
            
            foreach (var column in Columns)
            {
                var defaultColumn = defaultConfig.Columns.FirstOrDefault(c => c.PropertyName == column.PropertyName);
                if (defaultColumn != null)
                {
                    column.Width = defaultColumn.Width;
                }
            }

            MarkAsChanged();
        }

        /// <summary>
        /// Resets column visibility to default settings
        /// </summary>
        public void ResetColumnVisibility()
        {
            foreach (var column in Columns)
            {
                column.IsVisible = true; // All columns visible by default
            }

            MarkAsChanged();
        }

        /// <summary>
        /// Serializes the configuration to JSON string
        /// </summary>
        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return JsonSerializer.Serialize(this, options);
        }

        /// <summary>
        /// Deserializes configuration from JSON string
        /// </summary>
        public static ColumnConfiguration FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new ColumnConfiguration();

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                return JsonSerializer.Deserialize<ColumnConfiguration>(json, options) ?? new ColumnConfiguration();
            }
            catch
            {
                // Return default configuration if deserialization fails
                return new ColumnConfiguration();
            }
        }

        /// <summary>
        /// Marks the configuration as having unsaved changes
        /// </summary>
        private void MarkAsChanged()
        {
            HasUnsavedChanges = true;
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Marks the configuration as saved
        /// </summary>
        public void MarkAsSaved()
        {
            HasUnsavedChanges = false;
            LastSaved = DateTime.Now;
            SavedBy = Environment.UserName;
            OnPropertyChanged(nameof(HasUnsavedChanges));
            OnPropertyChanged(nameof(LastSaved));
            OnPropertyChanged(nameof(SavedBy));
        }

        private void OnColumnsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MarkAsChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

## FilterConfiguration.cs

### Filter State Management
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Manages filter configuration and state for the CustomDataGrid
    /// Phase 5 implementation - currently disabled
    /// </summary>
    public class FilterConfiguration : INotifyPropertyChanged
    {
        private string _searchText = string.Empty;
        private List<ColumnFilter> _columnFilters = new();
        private bool _isEnabled = false;
        private FilterMode _mode = FilterMode.And;

        /// <summary>
        /// Gets or sets the global search text filter
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value ?? string.Empty;
                    OnPropertyChanged();
                    OnFilterChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the column-specific filters
        /// </summary>
        public List<ColumnFilter> ColumnFilters
        {
            get => _columnFilters;
            set
            {
                if (_columnFilters != value)
                {
                    _columnFilters = value ?? new List<ColumnFilter>();
                    OnPropertyChanged();
                    OnFilterChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether filtering is enabled (Phase 5 feature)
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets how multiple filters are combined (And/Or)
        /// </summary>
        public FilterMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    OnPropertyChanged();
                    OnFilterChanged();
                }
            }
        }

        /// <summary>
        /// Gets whether any filters are currently active
        /// </summary>
        public bool HasActiveFilters =>
            !string.IsNullOrEmpty(SearchText) ||
            ColumnFilters.Any(f => f.IsActive);

        /// <summary>
        /// Adds or updates a column filter
        /// </summary>
        public void SetColumnFilter(string propertyName, string filterValue, FilterOperator filterOperator = FilterOperator.Contains)
        {
            var existingFilter = ColumnFilters.FirstOrDefault(f => f.PropertyName == propertyName);
            
            if (existingFilter != null)
            {
                existingFilter.Value = filterValue;
                existingFilter.Operator = filterOperator;
                existingFilter.IsActive = !string.IsNullOrEmpty(filterValue);
            }
            else
            {
                ColumnFilters.Add(new ColumnFilter
                {
                    PropertyName = propertyName,
                    Value = filterValue,
                    Operator = filterOperator,
                    IsActive = !string.IsNullOrEmpty(filterValue)
                });
            }

            OnFilterChanged();
        }

        /// <summary>
        /// Removes a column filter
        /// </summary>
        public void RemoveColumnFilter(string propertyName)
        {
            var filter = ColumnFilters.FirstOrDefault(f => f.PropertyName == propertyName);
            if (filter != null)
            {
                ColumnFilters.Remove(filter);
                OnFilterChanged();
            }
        }

        /// <summary>
        /// Clears all filters
        /// </summary>
        public void ClearAllFilters()
        {
            SearchText = string.Empty;
            ColumnFilters.Clear();
            OnFilterChanged();
        }

        /// <summary>
        /// Event raised when filter configuration changes
        /// </summary>
        public event EventHandler? FilterChanged;

        protected virtual void OnFilterChanged()
        {
            FilterChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Represents a filter for a specific column
    /// </summary>
    public class ColumnFilter : INotifyPropertyChanged
    {
        private string _propertyName = string.Empty;
        private string _value = string.Empty;
        private FilterOperator _operator = FilterOperator.Contains;
        private bool _isActive;

        [Required(ErrorMessage = "PropertyName is required")]
        public string PropertyName
        {
            get => _propertyName;
            set
            {
                if (_propertyName != value)
                {
                    _propertyName = value ?? string.Empty;
                    OnPropertyChanged();
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value ?? string.Empty;
                    IsActive = !string.IsNullOrEmpty(_value);
                    OnPropertyChanged();
                }
            }
        }

        public FilterOperator Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Filter operators for column filtering
    /// </summary>
    public enum FilterOperator
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }

    /// <summary>
    /// How multiple filters are combined
    /// </summary>
    public enum FilterMode
    {
        And,
        Or
    }
}
```

## SelectableItem.cs

### Selection Wrapper for Data Items
```csharp
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Wrapper class to add selection functionality to any data item
    /// Implements MVVM-friendly selection pattern
    /// </summary>
    /// <typeparam name="T">The type of data item to wrap</typeparam>
    public class SelectableItem<T> : INotifyPropertyChanged where T : class
    {
        private bool _isSelected;
        private T _item;

        /// <summary>
        /// Gets or sets whether this item is selected
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                    SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(this, value));
                }
            }
        }

        /// <summary>
        /// Gets or sets the wrapped data item
        /// </summary>
        public T Item
        {
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value ?? throw new ArgumentNullException(nameof(value));
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event raised when selection state changes
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        public SelectableItem(T item)
        {
            _item = item ?? throw new ArgumentNullException(nameof(item));
        }

        /// <summary>
        /// Toggles the selection state
        /// </summary>
        public void ToggleSelection()
        {
            IsSelected = !IsSelected;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"SelectableItem<{typeof(T).Name}> [Selected: {IsSelected}]";
        }
    }

    /// <summary>
    /// Event arguments for selection change events
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        public object SelectableItem { get; }
        public bool IsSelected { get; }

        public SelectionChangedEventArgs(object selectableItem, bool isSelected)
        {
            SelectableItem = selectableItem;
            IsSelected = isSelected;
        }
    }

    /// <summary>
    /// Extension methods for working with SelectableItem collections
    /// </summary>
    public static class SelectableItemExtensions
    {
        /// <summary>
        /// Wraps a collection of items in SelectableItem wrappers
        /// </summary>
        public static IEnumerable<SelectableItem<T>> ToSelectableItems<T>(this IEnumerable<T> items) where T : class
        {
            return items.Select(item => new SelectableItem<T>(item));
        }

        /// <summary>
        /// Unwraps SelectableItem collection back to original items
        /// </summary>
        public static IEnumerable<T> UnwrapItems<T>(this IEnumerable<SelectableItem<T>> selectableItems) where T : class
        {
            return selectableItems.Select(si => si.Item);
        }

        /// <summary>
        /// Gets only the selected items from a SelectableItem collection
        /// </summary>
        public static IEnumerable<T> GetSelectedItems<T>(this IEnumerable<SelectableItem<T>> selectableItems) where T : class
        {
            return selectableItems.Where(si => si.IsSelected).Select(si => si.Item);
        }

        /// <summary>
        /// Selects all items in the collection
        /// </summary>
        public static void SelectAll<T>(this IEnumerable<SelectableItem<T>> selectableItems) where T : class
        {
            foreach (var item in selectableItems)
            {
                item.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all items in the collection
        /// </summary>
        public static void DeselectAll<T>(this IEnumerable<SelectableItem<T>> selectableItems) where T : class
        {
            foreach (var item in selectableItems)
            {
                item.IsSelected = false;
            }
        }

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        public static int GetSelectedCount<T>(this IEnumerable<SelectableItem<T>> selectableItems) where T : class
        {
            return selectableItems.Count(si => si.IsSelected);
        }
    }
}
```

---

**Next Implementation Phase**: [08-Overlay-Integration.md](./08-Overlay-Integration.md)