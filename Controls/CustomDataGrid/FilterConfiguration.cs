using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Configuration class for filtering data in the CustomDataGrid
    /// Supports multiple filter types and conditions for flexible data filtering
    /// Follows MTM MVVM Community Toolkit patterns for consistency
    /// </summary>
    public partial class FilterConfiguration : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the column property name to filter on
        /// </summary>
        [ObservableProperty]
        private string _propertyName = string.Empty;

        /// <summary>
        /// Gets or sets the display name for this filter
        /// </summary>
        [ObservableProperty]
        private string _displayName = string.Empty;

        /// <summary>
        /// Gets or sets the filter operator
        /// </summary>
        [ObservableProperty]
        private FilterOperator _operator = FilterOperator.Contains;

        /// <summary>
        /// Gets or sets the filter value
        /// </summary>
        [ObservableProperty]
        private object? _value;

        /// <summary>
        /// Gets or sets the secondary value for range filters
        /// </summary>
        [ObservableProperty]
        private object? _value2;

        /// <summary>
        /// Gets or sets whether this filter is enabled
        /// </summary>
        [ObservableProperty]
        private bool _isEnabled = true;

        /// <summary>
        /// Gets or sets whether this filter is active
        /// </summary>
        [ObservableProperty]
        private bool _isActive;

        /// <summary>
        /// Gets or sets the data type for this filter
        /// </summary>
        [ObservableProperty]
        private Type _dataType = typeof(string);

        /// <summary>
        /// Gets or sets whether this filter is case sensitive (for string filters)
        /// </summary>
        [ObservableProperty]
        private bool _isCaseSensitive;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of FilterConfiguration
        /// </summary>
        public FilterConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance with basic settings
        /// </summary>
        /// <param name="propertyName">The property name to filter</param>
        /// <param name="displayName">The display name for this filter</param>
        /// <param name="dataType">The data type of the property</param>
        public FilterConfiguration(string propertyName, string displayName, Type dataType)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
            DataType = dataType;
            Operator = GetDefaultOperatorForType(dataType);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the default filter operator for a given data type
        /// </summary>
        /// <param name="dataType">The data type</param>
        /// <returns>Default filter operator</returns>
        public static FilterOperator GetDefaultOperatorForType(Type dataType)
        {
            if (dataType == typeof(string))
                return FilterOperator.Contains;
            
            if (dataType == typeof(bool))
                return FilterOperator.Equals;
            
            if (dataType.IsNumeric() || dataType == typeof(DateTime))
                return FilterOperator.Equals;
            
            return FilterOperator.Contains;
        }

        /// <summary>
        /// Gets available operators for the current data type
        /// </summary>
        /// <returns>List of applicable filter operators</returns>
        public List<FilterOperator> GetAvailableOperators()
        {
            var operators = new List<FilterOperator>();

            if (DataType == typeof(string))
            {
                operators.AddRange(new[]
                {
                    FilterOperator.Contains,
                    FilterOperator.StartsWith,
                    FilterOperator.EndsWith,
                    FilterOperator.Equals,
                    FilterOperator.NotEquals,
                    FilterOperator.IsEmpty,
                    FilterOperator.IsNotEmpty
                });
            }
            else if (DataType == typeof(bool))
            {
                operators.AddRange(new[]
                {
                    FilterOperator.Equals,
                    FilterOperator.NotEquals
                });
            }
            else if (DataType.IsNumeric() || DataType == typeof(DateTime))
            {
                operators.AddRange(new[]
                {
                    FilterOperator.Equals,
                    FilterOperator.NotEquals,
                    FilterOperator.GreaterThan,
                    FilterOperator.GreaterThanOrEqual,
                    FilterOperator.LessThan,
                    FilterOperator.LessThanOrEqual,
                    FilterOperator.Between,
                    FilterOperator.IsEmpty,
                    FilterOperator.IsNotEmpty
                });
            }
            else
            {
                // Default operators for other types
                operators.AddRange(new[]
                {
                    FilterOperator.Equals,
                    FilterOperator.NotEquals,
                    FilterOperator.IsEmpty,
                    FilterOperator.IsNotEmpty
                });
            }

            return operators;
        }

        /// <summary>
        /// Validates whether the current filter configuration is valid
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Property name is required
            if (string.IsNullOrWhiteSpace(PropertyName))
                return false;

            // For most operators, a value is required
            if (Operator != FilterOperator.IsEmpty && Operator != FilterOperator.IsNotEmpty)
            {
                if (Value == null)
                    return false;

                // For Between operator, both values are required
                if (Operator == FilterOperator.Between && Value2 == null)
                    return false;
            }

            // Validate data type compatibility
            if (Value != null && !IsValueCompatibleWithDataType(Value, DataType))
                return false;

            if (Value2 != null && !IsValueCompatibleWithDataType(Value2, DataType))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a value is compatible with a data type
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <param name="dataType">The target data type</param>
        /// <returns>True if compatible, false otherwise</returns>
        private static bool IsValueCompatibleWithDataType(object value, Type dataType)
        {
            if (value == null)
                return true;

            var valueType = value.GetType();
            
            // Direct type match
            if (valueType == dataType || dataType.IsAssignableFrom(valueType))
                return true;

            // String can be converted to most types
            if (valueType == typeof(string))
                return true;

            // Numeric compatibility
            if (valueType.IsNumeric() && dataType.IsNumeric())
                return true;

            return false;
        }

        /// <summary>
        /// Applies this filter to a collection of items
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <returns>Filtered collection</returns>
        public IEnumerable<object> Apply(IEnumerable<object> items)
        {
            if (!IsActive || !IsValid())
                return items;

            return items.Where(item => EvaluateItem(item));
        }

        /// <summary>
        /// Evaluates whether an item matches this filter
        /// </summary>
        /// <param name="item">The item to evaluate</param>
        /// <returns>True if the item matches, false otherwise</returns>
        public bool EvaluateItem(object item)
        {
            if (!IsActive || !IsValid() || item == null)
                return true;

            // Get the property value from the item
            var propertyValue = GetPropertyValue(item, PropertyName);

            return EvaluateValue(propertyValue);
        }

        /// <summary>
        /// Evaluates whether a value matches the filter criteria
        /// </summary>
        /// <param name="propertyValue">The value to evaluate</param>
        /// <returns>True if the value matches, false otherwise</returns>
        private bool EvaluateValue(object? propertyValue)
        {
            return Operator switch
            {
                FilterOperator.Equals => AreValuesEqual(propertyValue, Value),
                FilterOperator.NotEquals => !AreValuesEqual(propertyValue, Value),
                FilterOperator.Contains => ContainsValue(propertyValue, Value),
                FilterOperator.StartsWith => StartsWithValue(propertyValue, Value),
                FilterOperator.EndsWith => EndsWithValue(propertyValue, Value),
                FilterOperator.GreaterThan => CompareValues(propertyValue, Value) > 0,
                FilterOperator.GreaterThanOrEqual => CompareValues(propertyValue, Value) >= 0,
                FilterOperator.LessThan => CompareValues(propertyValue, Value) < 0,
                FilterOperator.LessThanOrEqual => CompareValues(propertyValue, Value) <= 0,
                FilterOperator.Between => IsBetweenValues(propertyValue, Value, Value2),
                FilterOperator.IsEmpty => IsEmptyValue(propertyValue),
                FilterOperator.IsNotEmpty => !IsEmptyValue(propertyValue),
                _ => true
            };
        }

        /// <summary>
        /// Gets a property value from an object using reflection
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>The property value</returns>
        private static object? GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return null;

            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                return null;

            try
            {
                return property.GetValue(obj);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Compares two values for equality
        /// </summary>
        private bool AreValuesEqual(object? value1, object? value2)
        {
            if (value1 == null && value2 == null)
                return true;
            
            if (value1 == null || value2 == null)
                return false;

            // String comparison with case sensitivity option
            if (value1 is string str1 && value2 is string str2)
            {
                var comparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return string.Equals(str1, str2, comparison);
            }

            return object.Equals(value1, value2);
        }

        /// <summary>
        /// Checks if value1 contains value2 (for string types)
        /// </summary>
        private bool ContainsValue(object? value1, object? value2)
        {
            if (value1?.ToString() is string str1 && value2?.ToString() is string str2)
            {
                var comparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return str1.Contains(str2, comparison);
            }
            return false;
        }

        /// <summary>
        /// Checks if value1 starts with value2 (for string types)
        /// </summary>
        private bool StartsWithValue(object? value1, object? value2)
        {
            if (value1?.ToString() is string str1 && value2?.ToString() is string str2)
            {
                var comparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return str1.StartsWith(str2, comparison);
            }
            return false;
        }

        /// <summary>
        /// Checks if value1 ends with value2 (for string types)
        /// </summary>
        private bool EndsWithValue(object? value1, object? value2)
        {
            if (value1?.ToString() is string str1 && value2?.ToString() is string str2)
            {
                var comparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return str1.EndsWith(str2, comparison);
            }
            return false;
        }

        /// <summary>
        /// Compares two values numerically
        /// </summary>
        private static int CompareValues(object? value1, object? value2)
        {
            if (value1 == null && value2 == null) return 0;
            if (value1 == null) return -1;
            if (value2 == null) return 1;

            if (value1 is IComparable comparable1 && value2 is IComparable)
            {
                try
                {
                    return comparable1.CompareTo(value2);
                }
                catch
                {
                    return 0;
                }
            }

            return string.Compare(value1.ToString(), value2?.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if a value is between two other values
        /// </summary>
        private static bool IsBetweenValues(object? value, object? min, object? max)
        {
            if (value == null || min == null || max == null)
                return false;

            return CompareValues(value, min) >= 0 && CompareValues(value, max) <= 0;
        }

        /// <summary>
        /// Checks if a value is considered empty
        /// </summary>
        private static bool IsEmptyValue(object? value)
        {
            if (value == null)
                return true;

            if (value is string str)
                return string.IsNullOrWhiteSpace(str);

            if (value is IEnumerable<object> enumerable)
                return !enumerable.Any();

            return false;
        }

        /// <summary>
        /// Clears the filter values
        /// </summary>
        public void Clear()
        {
            Value = null;
            Value2 = null;
            IsActive = false;
        }

        /// <summary>
        /// Creates a copy of this filter configuration
        /// </summary>
        /// <returns>New FilterConfiguration instance</returns>
        public FilterConfiguration Clone()
        {
            return new FilterConfiguration
            {
                PropertyName = PropertyName,
                DisplayName = DisplayName,
                Operator = Operator,
                Value = Value,
                Value2 = Value2,
                IsEnabled = IsEnabled,
                IsActive = IsActive,
                DataType = DataType,
                IsCaseSensitive = IsCaseSensitive
            };
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this filter
        /// </summary>
        public override string ToString()
        {
            var valueText = Value?.ToString() ?? "null";
            if (Operator == FilterOperator.Between && Value2 != null)
            {
                valueText = $"{valueText} - {Value2}";
            }
            
            return $"{DisplayName} {Operator} {valueText} (Active: {IsActive})";
        }

        #endregion
    }

    /// <summary>
    /// Enumeration of available filter operators
    /// </summary>
    public enum FilterOperator
    {
        /// <summary>Equal to</summary>
        Equals,
        
        /// <summary>Not equal to</summary>
        NotEquals,
        
        /// <summary>Contains (for strings)</summary>
        Contains,
        
        /// <summary>Starts with (for strings)</summary>
        StartsWith,
        
        /// <summary>Ends with (for strings)</summary>
        EndsWith,
        
        /// <summary>Greater than</summary>
        GreaterThan,
        
        /// <summary>Greater than or equal to</summary>
        GreaterThanOrEqual,
        
        /// <summary>Less than</summary>
        LessThan,
        
        /// <summary>Less than or equal to</summary>
        LessThanOrEqual,
        
        /// <summary>Between two values</summary>
        Between,
        
        /// <summary>Is empty or null</summary>
        IsEmpty,
        
        /// <summary>Is not empty or null</summary>
        IsNotEmpty
    }

    /// <summary>
    /// Collection of filter configurations for managing multiple filters
    /// </summary>
    public partial class FilterCollection : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the collection of filter configurations
        /// </summary>
        [ObservableProperty]
        private List<FilterConfiguration> _filters = new();

        /// <summary>
        /// Gets or sets the logical operator to combine filters (AND/OR)
        /// </summary>
        [ObservableProperty]
        private FilterLogicalOperator _logicalOperator = FilterLogicalOperator.And;

        #endregion

        #region Methods

        /// <summary>
        /// Adds a new filter to the collection
        /// </summary>
        /// <param name="filter">The filter to add</param>
        public void Add(FilterConfiguration filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            Filters.Add(filter);
        }

        /// <summary>
        /// Removes a filter from the collection
        /// </summary>
        /// <param name="filter">The filter to remove</param>
        /// <returns>True if removed, false if not found</returns>
        public bool Remove(FilterConfiguration filter)
        {
            return Filters.Remove(filter);
        }

        /// <summary>
        /// Clears all filters
        /// </summary>
        public void Clear()
        {
            Filters.Clear();
        }

        /// <summary>
        /// Gets all active filters
        /// </summary>
        /// <returns>Collection of active filters</returns>
        public IEnumerable<FilterConfiguration> GetActiveFilters()
        {
            return Filters.Where(f => f.IsActive && f.IsValid());
        }

        /// <summary>
        /// Applies all active filters to a collection of items
        /// </summary>
        /// <param name="items">The items to filter</param>
        /// <returns>Filtered collection</returns>
        public IEnumerable<object> Apply(IEnumerable<object> items)
        {
            var activeFilters = GetActiveFilters().ToList();
            if (activeFilters.Count == 0)
                return items;

            return items.Where(item =>
            {
                var results = activeFilters.Select(filter => filter.EvaluateItem(item));
                
                return LogicalOperator == FilterLogicalOperator.And 
                    ? results.All(result => result) 
                    : results.Any(result => result);
            });
        }

        /// <summary>
        /// Gets the count of active filters
        /// </summary>
        /// <returns>Number of active filters</returns>
        public int GetActiveFilterCount()
        {
            return GetActiveFilters().Count();
        }

        /// <summary>
        /// Checks if any filters are active
        /// </summary>
        /// <returns>True if any filters are active, false otherwise</returns>
        public bool HasActiveFilters()
        {
            return GetActiveFilters().Any();
        }

        #endregion
    }

    /// <summary>
    /// Logical operators for combining multiple filters
    /// </summary>
    public enum FilterLogicalOperator
    {
        /// <summary>All filters must match (AND)</summary>
        And,
        
        /// <summary>Any filter can match (OR)</summary>
        Or
    }

    /// <summary>
    /// Extension methods for Type to help with filter operations
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if a type is numeric
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if numeric, false otherwise</returns>
        public static bool IsNumeric(this Type type)
        {
            return type == typeof(int) || type == typeof(long) || type == typeof(short) ||
                   type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort) ||
                   type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(float) || type == typeof(double) || type == typeof(decimal) ||
                   type == typeof(int?) || type == typeof(long?) || type == typeof(short?) ||
                   type == typeof(uint?) || type == typeof(ulong?) || type == typeof(ushort?) ||
                   type == typeof(byte?) || type == typeof(sbyte?) ||
                   type == typeof(float?) || type == typeof(double?) || type == typeof(decimal?);
        }
    }
}