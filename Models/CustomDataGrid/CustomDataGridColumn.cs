using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Data;

namespace MTM_WIP_Application_Avalonia.Models.CustomDataGrid
{
    /// <summary>
    /// Represents a column in the CustomDataGrid control
    /// Provides column definition, data binding, and display configuration
    /// Follows MTM architecture patterns with proper validation and event handling
    /// </summary>
    public class CustomDataGridColumn : INotifyPropertyChanged
    {
        #region Private Fields

        private string _propertyName = string.Empty;
        private string _displayName = string.Empty;
        private Type _dataType = typeof(string);
        private double _width = double.NaN;
        private bool _isVisible = true;
        private bool _canSort = true;
        private bool _canResize = true;
        private bool _canFilter = true;
        private string? _stringFormat;
        private ColumnAlignment _alignment = ColumnAlignment.Left;
        private object? _defaultValue;
        private string? _converterParameter;
        private ICommand? _cellCommand;
        private bool _isReadOnly = false;
        private double _minWidth = 20;
        private double _maxWidth = double.PositiveInfinity;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the property name for data binding
        /// </summary>
        public string PropertyName
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the display name shown in the column header
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value ?? string.Empty);
        }

        /// <summary>
        /// Gets or sets the data type for this column
        /// </summary>
        public Type DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value ?? typeof(string));
        }

        /// <summary>
        /// Gets or sets the column width (NaN for auto-sizing)
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /// <summary>
        /// Gets or sets whether this column is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        /// <summary>
        /// Gets or sets whether this column can be sorted
        /// </summary>
        public bool CanSort
        {
            get => _canSort;
            set => SetProperty(ref _canSort, value);
        }

        /// <summary>
        /// Gets or sets whether this column can be resized
        /// </summary>
        public bool CanResize
        {
            get => _canResize;
            set => SetProperty(ref _canResize, value);
        }

        /// <summary>
        /// Gets or sets whether this column can be filtered
        /// </summary>
        public bool CanFilter
        {
            get => _canFilter;
            set => SetProperty(ref _canFilter, value);
        }

        /// <summary>
        /// Gets or sets the string format for displaying values
        /// </summary>
        public string? StringFormat
        {
            get => _stringFormat;
            set => SetProperty(ref _stringFormat, value);
        }

        /// <summary>
        /// Gets or sets the column content alignment
        /// </summary>
        public ColumnAlignment Alignment
        {
            get => _alignment;
            set => SetProperty(ref _alignment, value);
        }

        /// <summary>
        /// Gets or sets the default value for new items in this column
        /// </summary>
        public object? DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }

        /// <summary>
        /// Gets or sets the converter parameter
        /// </summary>
        public string? ConverterParameter
        {
            get => _converterParameter;
            set => SetProperty(ref _converterParameter, value);
        }

        /// <summary>
        /// Gets or sets the command to execute when a cell is clicked
        /// </summary>
        public ICommand? CellCommand
        {
            get => _cellCommand;
            set => SetProperty(ref _cellCommand, value);
        }

        /// <summary>
        /// Gets or sets whether this column is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        /// <summary>
        /// Gets or sets the minimum width for this column
        /// </summary>
        public double MinWidth
        {
            get => _minWidth;
            set => SetProperty(ref _minWidth, Math.Max(0, value));
        }

        /// <summary>
        /// Gets or sets the maximum width for this column
        /// </summary>
        public double MaxWidth
        {
            get => _maxWidth;
            set => SetProperty(ref _maxWidth, Math.Max(0, value));
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of CustomDataGridColumn
        /// </summary>
        public CustomDataGridColumn()
        {
        }

        /// <summary>
        /// Initializes a new instance with basic column information
        /// </summary>
        /// <param name="propertyName">The property name for data binding</param>
        /// <param name="displayName">The display name for the column header</param>
        public CustomDataGridColumn(string propertyName, string displayName)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        /// <summary>
        /// Initializes a new instance with full column configuration
        /// </summary>
        /// <param name="propertyName">The property name for data binding</param>
        /// <param name="displayName">The display name for the column header</param>
        /// <param name="dataType">The data type for the column</param>
        /// <param name="width">The column width</param>
        public CustomDataGridColumn(string propertyName, string displayName, Type dataType, double width)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
            DataType = dataType;
            Width = width;
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a standard MTM column configuration
        /// </summary>
        /// <param name="columnType">The type of MTM column to create</param>
        /// <returns>Configured CustomDataGridColumn instance</returns>
        public static CustomDataGridColumn CreateMTMColumn(MTMColumnType columnType)
        {
            return columnType switch
            {
                MTMColumnType.Selection => new CustomDataGridColumn("IsSelected", "")
                {
                    DataType = typeof(bool),
                    Width = 40,
                    CanSort = false,
                    CanResize = false,
                    CanFilter = false,
                    Alignment = ColumnAlignment.Center,
                    IsReadOnly = false
                },

                MTMColumnType.PartID => new CustomDataGridColumn("PartId", "Part ID")
                {
                    DataType = typeof(string),
                    Width = 150,
                    Alignment = ColumnAlignment.Left,
                    CanSort = true,
                    CanFilter = true
                },

                MTMColumnType.Operation => new CustomDataGridColumn("Operation", "Operation")
                {
                    DataType = typeof(string),
                    Width = 100,
                    Alignment = ColumnAlignment.Center,
                    CanSort = true,
                    CanFilter = true
                },

                MTMColumnType.Location => new CustomDataGridColumn("Location", "Location")
                {
                    DataType = typeof(string),
                    Width = 120,
                    Alignment = ColumnAlignment.Left,
                    CanSort = true,
                    CanFilter = true
                },

                MTMColumnType.Quantity => new CustomDataGridColumn("Quantity", "Quantity")
                {
                    DataType = typeof(int),
                    Width = 100,
                    Alignment = ColumnAlignment.Right,
                    StringFormat = "N0",
                    CanSort = true,
                    CanFilter = true
                },

                MTMColumnType.LastUpdated => new CustomDataGridColumn("LastUpdated", "Last Updated")
                {
                    DataType = typeof(DateTime),
                    Width = 180,
                    Alignment = ColumnAlignment.Center,
                    StringFormat = "MM/dd/yyyy HH:mm",
                    CanSort = true,
                    CanFilter = true,
                    IsReadOnly = true
                },

                MTMColumnType.Notes => new CustomDataGridColumn("HasNotes", "Notes")
                {
                    DataType = typeof(bool),
                    Width = 80,
                    Alignment = ColumnAlignment.Center,
                    CanSort = false,
                    CanFilter = false,
                    IsReadOnly = true
                },

                MTMColumnType.Actions => new CustomDataGridColumn("", "Actions")
                {
                    DataType = typeof(object),
                    Width = 100,
                    CanSort = false,
                    CanResize = false,
                    CanFilter = false,
                    Alignment = ColumnAlignment.Center,
                    IsReadOnly = true
                },

                MTMColumnType.Management => new CustomDataGridColumn("", "")
                {
                    DataType = typeof(object),
                    Width = 40,
                    CanSort = false,
                    CanResize = false,
                    CanFilter = false,
                    Alignment = ColumnAlignment.Center,
                    IsReadOnly = true
                },

                _ => new CustomDataGridColumn("Unknown", "Unknown")
                {
                    DataType = typeof(string),
                    Width = 100
                }
            };
        }

        /// <summary>
        /// Creates a text column with standard settings
        /// </summary>
        /// <param name="propertyName">Property name for binding</param>
        /// <param name="displayName">Display name for header</param>
        /// <param name="width">Column width</param>
        /// <param name="alignment">Text alignment</param>
        /// <returns>Configured text column</returns>
        public static CustomDataGridColumn CreateTextColumn(string propertyName, string displayName, 
            double width = 100, ColumnAlignment alignment = ColumnAlignment.Left)
        {
            return new CustomDataGridColumn(propertyName, displayName, typeof(string), width)
            {
                Alignment = alignment
            };
        }

        /// <summary>
        /// Creates a numeric column with standard settings
        /// </summary>
        /// <param name="propertyName">Property name for binding</param>
        /// <param name="displayName">Display name for header</param>
        /// <param name="width">Column width</param>
        /// <param name="format">Number format string</param>
        /// <returns>Configured numeric column</returns>
        public static CustomDataGridColumn CreateNumericColumn(string propertyName, string displayName, 
            double width = 100, string format = "N0")
        {
            return new CustomDataGridColumn(propertyName, displayName, typeof(decimal), width)
            {
                Alignment = ColumnAlignment.Right,
                StringFormat = format
            };
        }

        /// <summary>
        /// Creates a date column with standard settings
        /// </summary>
        /// <param name="propertyName">Property name for binding</param>
        /// <param name="displayName">Display name for header</param>
        /// <param name="width">Column width</param>
        /// <param name="format">Date format string</param>
        /// <returns>Configured date column</returns>
        public static CustomDataGridColumn CreateDateColumn(string propertyName, string displayName, 
            double width = 120, string format = "MM/dd/yyyy")
        {
            return new CustomDataGridColumn(propertyName, displayName, typeof(DateTime), width)
            {
                Alignment = ColumnAlignment.Center,
                StringFormat = format
            };
        }

        /// <summary>
        /// Creates a boolean column with checkbox display
        /// </summary>
        /// <param name="propertyName">Property name for binding</param>
        /// <param name="displayName">Display name for header</param>
        /// <param name="width">Column width</param>
        /// <returns>Configured boolean column</returns>
        public static CustomDataGridColumn CreateBooleanColumn(string propertyName, string displayName, 
            double width = 80)
        {
            return new CustomDataGridColumn(propertyName, displayName, typeof(bool), width)
            {
                Alignment = ColumnAlignment.Center,
                CanFilter = false
            };
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates the column configuration
        /// </summary>
        /// <returns>True if the column is valid, false otherwise</returns>
        public bool IsValid()
        {
            // Property name is required for data binding columns
            if (string.IsNullOrWhiteSpace(PropertyName) && CanSort)
                return false;

            // Display name should be provided (can be empty for special columns)
            if (DisplayName == null)
                return false;

            // Width validation
            if (Width <= 0 && !double.IsNaN(Width))
                return false;

            // Min/Max width validation
            if (MinWidth > MaxWidth)
                return false;

            if (!double.IsNaN(Width) && (Width < MinWidth || Width > MaxWidth))
                return false;

            return true;
        }

        /// <summary>
        /// Gets validation errors for the current configuration
        /// </summary>
        /// <returns>Array of validation error messages</returns>
        public string[] GetValidationErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(PropertyName) && CanSort)
                errors.Add("PropertyName is required for sortable columns");

            if (DisplayName == null)
                errors.Add("DisplayName cannot be null");

            if (Width <= 0 && !double.IsNaN(Width))
                errors.Add("Width must be positive or NaN for auto-sizing");

            if (MinWidth > MaxWidth)
                errors.Add("MinWidth cannot be greater than MaxWidth");

            if (!double.IsNaN(Width) && Width < MinWidth)
                errors.Add($"Width ({Width}) cannot be less than MinWidth ({MinWidth})");

            if (!double.IsNaN(Width) && Width > MaxWidth)
                errors.Add($"Width ({Width}) cannot be greater than MaxWidth ({MaxWidth})");

            return errors.ToArray();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a deep copy of this column
        /// </summary>
        /// <returns>New CustomDataGridColumn with copied values</returns>
        public CustomDataGridColumn Clone()
        {
            return new CustomDataGridColumn
            {
                PropertyName = PropertyName,
                DisplayName = DisplayName,
                DataType = DataType,
                Width = Width,
                IsVisible = IsVisible,
                CanSort = CanSort,
                CanResize = CanResize,
                CanFilter = CanFilter,
                StringFormat = StringFormat,
                Alignment = Alignment,
                DefaultValue = DefaultValue,
                ConverterParameter = ConverterParameter,
                CellCommand = CellCommand,
                IsReadOnly = IsReadOnly,
                MinWidth = MinWidth,
                MaxWidth = MaxWidth
            };
        }

        /// <summary>
        /// Gets the effective width for this column
        /// </summary>
        /// <param name="availableWidth">Available width for auto-sizing</param>
        /// <returns>The effective width to use</returns>
        public double GetEffectiveWidth(double availableWidth = double.NaN)
        {
            if (double.IsNaN(Width))
            {
                // Auto-sizing logic
                if (!double.IsNaN(availableWidth))
                {
                    var autoWidth = Math.Max(MinWidth, Math.Min(MaxWidth, availableWidth));
                    return autoWidth;
                }
                return MinWidth;
            }

            // Fixed width - ensure it's within bounds
            return Math.Max(MinWidth, Math.Min(MaxWidth, Width));
        }

        /// <summary>
        /// Returns whether this column represents a special UI column (actions, selection, etc.)
        /// </summary>
        public bool IsSpecialColumn()
        {
            return string.IsNullOrEmpty(PropertyName) || 
                   PropertyName.Equals("IsSelected", StringComparison.OrdinalIgnoreCase) ||
                   PropertyName.Equals("HasNotes", StringComparison.OrdinalIgnoreCase) ||
                   DisplayName.Equals("Actions", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Sets the width of this column
        /// </summary>
        /// <param name="width">The width to set</param>
        public void SetWidth(double width)
        {
            Width = width;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingStore, T value, 
            [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this column
        /// </summary>
        public override string ToString()
        {
            return $"{DisplayName} ({PropertyName}) - Width: {Width}, Type: {DataType.Name}";
        }

        /// <summary>
        /// Determines equality based on PropertyName
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is CustomDataGridColumn other)
            {
                return string.Equals(PropertyName, other.PropertyName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Gets hash code based on PropertyName
        /// </summary>
        public override int GetHashCode()
        {
            return PropertyName?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
        }

        #endregion
    }

    /// <summary>
    /// Enumeration of column alignment options
    /// </summary>
    public enum ColumnAlignment
    {
        Left,
        Center,
        Right,
        Stretch
    }

    /// <summary>
    /// Enumeration of standard MTM column types
    /// </summary>
    public enum MTMColumnType
    {
        Selection,
        PartID,
        Operation,
        Location,
        Quantity,
        LastUpdated,
        Notes,
        Actions,
        Management
    }
}