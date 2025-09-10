using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Represents a filter operation that can be applied to a column.
/// Phase 5 feature for advanced filtering capabilities.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Value equals the filter criteria (case-insensitive for strings).
    /// </summary>
    Equals,
    
    /// <summary>
    /// Value contains the filter criteria (case-insensitive for strings).
    /// </summary>
    Contains,
    
    /// <summary>
    /// Value starts with the filter criteria (case-insensitive for strings).
    /// </summary>
    StartsWith,
    
    /// <summary>
    /// Value ends with the filter criteria (case-insensitive for strings).
    /// </summary>
    EndsWith,
    
    /// <summary>
    /// Value is greater than the filter criteria.
    /// </summary>
    GreaterThan,
    
    /// <summary>
    /// Value is greater than or equal to the filter criteria.
    /// </summary>
    GreaterThanOrEqual,
    
    /// <summary>
    /// Value is less than the filter criteria.
    /// </summary>
    LessThan,
    
    /// <summary>
    /// Value is less than or equal to the filter criteria.
    /// </summary>
    LessThanOrEqual,
    
    /// <summary>
    /// Value is not equal to the filter criteria.
    /// </summary>
    NotEquals,
    
    /// <summary>
    /// Value does not contain the filter criteria.
    /// </summary>
    NotContains,
    
    /// <summary>
    /// Value is null or empty (for nullable types and strings).
    /// </summary>
    IsEmpty,
    
    /// <summary>
    /// Value is not null or empty (for nullable types and strings).
    /// </summary>
    IsNotEmpty,
    
    /// <summary>
    /// Value is within a date range (for DateTime columns).
    /// </summary>
    DateRange,
    
    /// <summary>
    /// Value is within a numeric range (for numeric columns).
    /// </summary>
    NumericRange,
    
    /// <summary>
    /// Value is in a list of values (for dropdown filtering).
    /// </summary>
    InList,
    
    /// <summary>
    /// Value is not in a list of values.
    /// </summary>
    NotInList
}

/// <summary>
/// Represents a filter condition for a specific column.
/// Phase 5 feature for advanced column-level filtering.
/// </summary>
public partial class ColumnFilter : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the property name of the column this filter applies to.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the display name of the column for UI purposes.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the data type of the column for appropriate filter UI.
    /// </summary>
    public Type DataType { get; set; } = typeof(string);
    
    private bool _isActive;
    /// <summary>
    /// Gets or sets whether this filter is currently active.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(HasValidCriteria));
                OnPropertyChanged(nameof(FilterSummary));
            }
        }
    }
    
    private FilterOperator _filterOperator = FilterOperator.Contains;
    /// <summary>
    /// Gets or sets the filter operator to use.
    /// </summary>
    public FilterOperator FilterOperator
    {
        get => _filterOperator;
        set
        {
            if (_filterOperator != value)
            {
                _filterOperator = value;
                OnPropertyChanged(nameof(FilterOperator));
                OnPropertyChanged(nameof(IsTextFilter));
                OnPropertyChanged(nameof(IsNumericFilter));
                OnPropertyChanged(nameof(IsDateFilter));
                OnPropertyChanged(nameof(IsBooleanFilter));
                OnPropertyChanged(nameof(IsRangeFilter));
                OnPropertyChanged(nameof(IsListFilter));
                OnPropertyChanged(nameof(FilterSummary));
            }
        }
    }
    
    private object? _filterValue;
    /// <summary>
    /// Gets or sets the primary filter value.
    /// </summary>
    public object? FilterValue
    {
        get => _filterValue;
        set
        {
            if (!Equals(_filterValue, value))
            {
                _filterValue = value;
                OnPropertyChanged(nameof(FilterValue));
                OnPropertyChanged(nameof(FilterSummary));
                OnPropertyChanged(nameof(HasValidCriteria));
            }
        }
    }
    
    private object? _filterValueTo;
    /// <summary>
    /// Gets or sets the secondary filter value (used for range operations).
    /// </summary>
    public object? FilterValueTo
    {
        get => _filterValueTo;
        set
        {
            if (!Equals(_filterValueTo, value))
            {
                _filterValueTo = value;
                OnPropertyChanged(nameof(FilterValueTo));
            }
        }
    }
    
    private ObservableCollection<object> _filterValues = new();
    /// <summary>
    /// Gets or sets the list of values for InList/NotInList operations.
    /// </summary>
    public ObservableCollection<object> FilterValues
    {
        get => _filterValues;
        set
        {
            if (_filterValues != value)
            {
                _filterValues = value;
                OnPropertyChanged(nameof(FilterValues));
            }
        }
    }
    
    private bool _isCaseSensitive = false;
    /// <summary>
    /// Gets or sets whether the filter should be case-sensitive (for string operations).
    /// </summary>
    public bool IsCaseSensitive
    {
        get => _isCaseSensitive;
        set
        {
            if (_isCaseSensitive != value)
            {
                _isCaseSensitive = value;
                OnPropertyChanged(nameof(IsCaseSensitive));
            }
        }
    }

    /// <summary>
    /// Property changed event for INotifyPropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// Gets whether this filter has valid criteria.
    /// </summary>
    public bool HasValidCriteria => IsActive && (FilterValue != null || 
        FilterOperator == FilterOperator.IsEmpty || 
        FilterOperator == FilterOperator.IsNotEmpty ||
        (FilterOperator == FilterOperator.InList && FilterValues.Count > 0) ||
        (FilterOperator == FilterOperator.DateRange && FilterValue != null && FilterValueTo != null) ||
        (FilterOperator == FilterOperator.NumericRange && FilterValue != null && FilterValueTo != null));
    
    /// <summary>
    /// Gets available filter operators for the column's data type.
    /// </summary>
    public ObservableCollection<FilterOperator> AvailableOperators => GetAvailableOperators(DataType);
    
    /// <summary>
    /// Gets a summary description of the current filter.
    /// </summary>
    public string FilterSummary
    {
        get
        {
            if (!IsActive || !HasValidCriteria)
                return "No filter applied";
                
            return FilterOperator switch
            {
                FilterOperator.Equals => $"= '{FilterValue}'",
                FilterOperator.Contains => $"contains '{FilterValue}'",
                FilterOperator.StartsWith => $"starts with '{FilterValue}'",
                FilterOperator.EndsWith => $"ends with '{FilterValue}'",
                FilterOperator.GreaterThan => $"> {FilterValue}",
                FilterOperator.GreaterThanOrEqual => $">= {FilterValue}",
                FilterOperator.LessThan => $"< {FilterValue}",
                FilterOperator.LessThanOrEqual => $"<= {FilterValue}",
                FilterOperator.NotEquals => $"!= '{FilterValue}'",
                FilterOperator.NotContains => $"not contains '{FilterValue}'",
                FilterOperator.IsEmpty => "is empty",
                FilterOperator.IsNotEmpty => "is not empty",
                FilterOperator.DateRange => $"between {FilterValue} and {FilterValueTo}",
                FilterOperator.NumericRange => $"between {FilterValue} and {FilterValueTo}",
                FilterOperator.InList => $"in ({FilterValues.Count} values)",
                FilterOperator.NotInList => $"not in ({FilterValues.Count} values)",
                _ => "Unknown filter"
            };
        }
    }
    
    /// <summary>
    /// Gets whether this filter should display a text input control.
    /// </summary>
    public bool IsTextFilter => DataType == typeof(string) && 
        (FilterOperator == FilterOperator.Contains ||
         FilterOperator == FilterOperator.Equals ||
         FilterOperator == FilterOperator.StartsWith ||
         FilterOperator == FilterOperator.EndsWith ||
         FilterOperator == FilterOperator.NotEquals ||
         FilterOperator == FilterOperator.NotContains);
    
    /// <summary>
    /// Gets whether this filter should display a numeric input control.
    /// </summary>
    public bool IsNumericFilter => IsNumericType(DataType) && 
        (FilterOperator == FilterOperator.Equals ||
         FilterOperator == FilterOperator.GreaterThan ||
         FilterOperator == FilterOperator.GreaterThanOrEqual ||
         FilterOperator == FilterOperator.LessThan ||
         FilterOperator == FilterOperator.LessThanOrEqual ||
         FilterOperator == FilterOperator.NotEquals);
    
    /// <summary>
    /// Gets whether this filter should display a date picker control.
    /// </summary>
    public bool IsDateFilter => (DataType == typeof(DateTime) || DataType == typeof(DateTime?)) && 
        (FilterOperator == FilterOperator.Equals ||
         FilterOperator == FilterOperator.GreaterThan ||
         FilterOperator == FilterOperator.GreaterThanOrEqual ||
         FilterOperator == FilterOperator.LessThan ||
         FilterOperator == FilterOperator.LessThanOrEqual ||
         FilterOperator == FilterOperator.NotEquals);
    
    /// <summary>
    /// Gets whether this filter should display a boolean selector control.
    /// </summary>
    public bool IsBooleanFilter => (DataType == typeof(bool) || DataType == typeof(bool?)) && 
        (FilterOperator == FilterOperator.Equals ||
         FilterOperator == FilterOperator.NotEquals);
    
    /// <summary>
    /// Gets whether this filter should display range input controls.
    /// </summary>
    public bool IsRangeFilter => FilterOperator == FilterOperator.DateRange || 
                                FilterOperator == FilterOperator.NumericRange;
    
    /// <summary>
    /// Gets whether this filter should display list input controls.
    /// </summary>
    public bool IsListFilter => FilterOperator == FilterOperator.InList || 
                               FilterOperator == FilterOperator.NotInList;
    
    /// <summary>
    /// Initializes a new column filter.
    /// </summary>
    public ColumnFilter(string propertyName, string displayName, Type dataType)
    {
        PropertyName = propertyName;
        DisplayName = displayName;
        DataType = dataType;
        FilterOperator = GetDefaultOperator(dataType);
    }
    
    /// <summary>
    /// Clears the filter criteria and deactivates the filter.
    /// </summary>
    public void Clear()
    {
        IsActive = false;
        FilterValue = null;
        FilterValueTo = null;
        FilterValues.Clear();
    }
    
    /// <summary>
    /// Evaluates whether the given value matches this filter's criteria.
    /// </summary>
    public bool MatchesCriteria(object? value)
    {
        if (!IsActive || !HasValidCriteria)
            return true;
            
        return FilterOperator switch
        {
            FilterOperator.Equals => EvaluateEquals(value, FilterValue),
            FilterOperator.Contains => EvaluateContains(value, FilterValue),
            FilterOperator.StartsWith => EvaluateStartsWith(value, FilterValue),
            FilterOperator.EndsWith => EvaluateEndsWith(value, FilterValue),
            FilterOperator.GreaterThan => EvaluateGreaterThan(value, FilterValue),
            FilterOperator.GreaterThanOrEqual => EvaluateGreaterThanOrEqual(value, FilterValue),
            FilterOperator.LessThan => EvaluateLessThan(value, FilterValue),
            FilterOperator.LessThanOrEqual => EvaluateLessThanOrEqual(value, FilterValue),
            FilterOperator.NotEquals => !EvaluateEquals(value, FilterValue),
            FilterOperator.NotContains => !EvaluateContains(value, FilterValue),
            FilterOperator.IsEmpty => EvaluateIsEmpty(value),
            FilterOperator.IsNotEmpty => !EvaluateIsEmpty(value),
            FilterOperator.DateRange => EvaluateDateRange(value, FilterValue, FilterValueTo),
            FilterOperator.NumericRange => EvaluateNumericRange(value, FilterValue, FilterValueTo),
            FilterOperator.InList => EvaluateInList(value, FilterValues),
            FilterOperator.NotInList => !EvaluateInList(value, FilterValues),
            _ => true
        };
    }
    
    #region Private Methods
    
    private static ObservableCollection<FilterOperator> GetAvailableOperators(Type dataType)
    {
        if (dataType == typeof(string))
        {
            return new ObservableCollection<FilterOperator>
            {
                FilterOperator.Contains,
                FilterOperator.Equals,
                FilterOperator.StartsWith,
                FilterOperator.EndsWith,
                FilterOperator.NotEquals,
                FilterOperator.NotContains,
                FilterOperator.IsEmpty,
                FilterOperator.IsNotEmpty,
                FilterOperator.InList,
                FilterOperator.NotInList
            };
        }
        
        if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
        {
            return new ObservableCollection<FilterOperator>
            {
                FilterOperator.Equals,
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual,
                FilterOperator.NotEquals,
                FilterOperator.DateRange,
                FilterOperator.IsEmpty,
                FilterOperator.IsNotEmpty
            };
        }
        
        if (IsNumericType(dataType))
        {
            return new ObservableCollection<FilterOperator>
            {
                FilterOperator.Equals,
                FilterOperator.GreaterThan,
                FilterOperator.GreaterThanOrEqual,
                FilterOperator.LessThan,
                FilterOperator.LessThanOrEqual,
                FilterOperator.NotEquals,
                FilterOperator.NumericRange,
                FilterOperator.InList,
                FilterOperator.NotInList
            };
        }
        
        if (dataType == typeof(bool) || dataType == typeof(bool?))
        {
            return new ObservableCollection<FilterOperator>
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.IsEmpty,
                FilterOperator.IsNotEmpty
            };
        }
        
        // Default operators for other types
        return new ObservableCollection<FilterOperator>
        {
            FilterOperator.Equals,
            FilterOperator.NotEquals,
            FilterOperator.IsEmpty,
            FilterOperator.IsNotEmpty
        };
    }
    
    private static FilterOperator GetDefaultOperator(Type dataType)
    {
        if (dataType == typeof(string))
            return FilterOperator.Contains;
            
        return FilterOperator.Equals;
    }
    
    private static bool IsNumericType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        return underlyingType == typeof(int) || 
               underlyingType == typeof(long) || 
               underlyingType == typeof(decimal) || 
               underlyingType == typeof(double) || 
               underlyingType == typeof(float) || 
               underlyingType == typeof(short) || 
               underlyingType == typeof(byte);
    }
    
    #region Evaluation Methods
    
    private bool EvaluateEquals(object? value, object? filterValue)
    {
        if (value == null && filterValue == null) return true;
        if (value == null || filterValue == null) return false;
        
        if (value is string stringValue && filterValue is string filterStringValue)
        {
            return string.Equals(stringValue, filterStringValue, 
                IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }
        
        return value.Equals(filterValue);
    }
    
    private bool EvaluateContains(object? value, object? filterValue)
    {
        if (value is not string stringValue || filterValue is not string filterStringValue)
            return false;
            
        return stringValue.Contains(filterStringValue, 
            IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }
    
    private bool EvaluateStartsWith(object? value, object? filterValue)
    {
        if (value is not string stringValue || filterValue is not string filterStringValue)
            return false;
            
        return stringValue.StartsWith(filterStringValue, 
            IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }
    
    private bool EvaluateEndsWith(object? value, object? filterValue)
    {
        if (value is not string stringValue || filterValue is not string filterStringValue)
            return false;
            
        return stringValue.EndsWith(filterStringValue, 
            IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
    }
    
    private static bool EvaluateGreaterThan(object? value, object? filterValue)
    {
        if (value == null || filterValue == null) return false;
        
        if (value is IComparable comparableValue && filterValue.GetType() == value.GetType())
        {
            return comparableValue.CompareTo(filterValue) > 0;
        }
        
        return false;
    }
    
    private static bool EvaluateGreaterThanOrEqual(object? value, object? filterValue)
    {
        if (value == null || filterValue == null) return false;
        
        if (value is IComparable comparableValue && filterValue.GetType() == value.GetType())
        {
            return comparableValue.CompareTo(filterValue) >= 0;
        }
        
        return false;
    }
    
    private static bool EvaluateLessThan(object? value, object? filterValue)
    {
        if (value == null || filterValue == null) return false;
        
        if (value is IComparable comparableValue && filterValue.GetType() == value.GetType())
        {
            return comparableValue.CompareTo(filterValue) < 0;
        }
        
        return false;
    }
    
    private static bool EvaluateLessThanOrEqual(object? value, object? filterValue)
    {
        if (value == null || filterValue == null) return false;
        
        if (value is IComparable comparableValue && filterValue.GetType() == value.GetType())
        {
            return comparableValue.CompareTo(filterValue) <= 0;
        }
        
        return false;
    }
    
    private static bool EvaluateIsEmpty(object? value)
    {
        return value == null || (value is string str && string.IsNullOrWhiteSpace(str));
    }
    
    private static bool EvaluateDateRange(object? value, object? fromValue, object? toValue)
    {
        if (value is not DateTime dateValue || fromValue is not DateTime fromDate || toValue is not DateTime toDate)
            return false;
            
        return dateValue >= fromDate && dateValue <= toDate;
    }
    
    private static bool EvaluateNumericRange(object? value, object? fromValue, object? toValue)
    {
        if (value == null || fromValue == null || toValue == null) return false;
        
        if (value is IComparable comparableValue && 
            fromValue.GetType() == value.GetType() && 
            toValue.GetType() == value.GetType())
        {
            return comparableValue.CompareTo(fromValue) >= 0 && comparableValue.CompareTo(toValue) <= 0;
        }
        
        return false;
    }
    
    private bool EvaluateInList(object? value, ObservableCollection<object> values)
    {
        if (values.Count == 0) return false;
        
        return values.Any(filterValue => EvaluateEquals(value, filterValue));
    }
    
    #endregion
    
    #endregion
}

/// <summary>
/// Represents a complete filtering configuration for a CustomDataGrid.
/// Phase 5 feature for advanced filtering and search capabilities.
/// </summary>
public partial class FilterConfiguration : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the unique identifier for this filter configuration.
    /// </summary>
    public string ConfigurationId { get; set; } = Guid.NewGuid().ToString();
    
    private string _displayName = "New Filter";
    /// <summary>
    /// Gets or sets the display name for this filter configuration.
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
    /// Gets or sets the description of this filter configuration.
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
    
    private string _globalSearchText = string.Empty;
    /// <summary>
    /// Gets or sets the global search text that applies to all searchable columns.
    /// </summary>
    public string GlobalSearchText
    {
        get => _globalSearchText;
        set
        {
            if (_globalSearchText != value)
            {
                _globalSearchText = value;
                OnPropertyChanged(nameof(GlobalSearchText));
            }
        }
    }
    
    private bool _isGlobalSearchCaseSensitive = false;
    /// <summary>
    /// Gets or sets whether the global search should be case-sensitive.
    /// </summary>
    public bool IsGlobalSearchCaseSensitive
    {
        get => _isGlobalSearchCaseSensitive;
        set
        {
            if (_isGlobalSearchCaseSensitive != value)
            {
                _isGlobalSearchCaseSensitive = value;
                OnPropertyChanged(nameof(IsGlobalSearchCaseSensitive));
            }
        }
    }
    
    private ObservableCollection<ColumnFilter> _columnFilters = new();
    /// <summary>
    /// Gets the collection of column-specific filters.
    /// </summary>
    public ObservableCollection<ColumnFilter> ColumnFilters
    {
        get => _columnFilters;
        set
        {
            if (_columnFilters != value)
            {
                _columnFilters = value;
                OnPropertyChanged(nameof(ColumnFilters));
            }
        }
    }
    
    private bool _isActive;
    /// <summary>
    /// Gets or sets whether this filter configuration is currently active.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
            }
        }
    }
    
    /// <summary>
    /// Gets or sets the date this configuration was created.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    private DateTime _lastModified = DateTime.Now;
    /// <summary>
    /// Gets or sets the date this configuration was last modified.
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
    
    /// <summary>
    /// Gets or sets whether this is a built-in preset filter.
    /// </summary>
    public bool IsPreset { get; set; }

    /// <summary>
    /// Property changed event for INotifyPropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    /// <summary>
    /// Gets whether this filter configuration has any active filters.
    /// </summary>
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(GlobalSearchText) || 
                                   ColumnFilters.Any(f => f.IsActive && f.HasValidCriteria);
    
    /// <summary>
    /// Gets the number of active column filters.
    /// </summary>
    public int ActiveFilterCount => ColumnFilters.Count(f => f.IsActive && f.HasValidCriteria);
    
    /// <summary>
    /// Gets a summary of the active filters.
    /// </summary>
    public string FilterSummary
    {
        get
        {
            var summaryParts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(GlobalSearchText))
            {
                summaryParts.Add($"Global: '{GlobalSearchText}'");
            }
            
            var activeColumnFilters = ColumnFilters.Where(f => f.IsActive && f.HasValidCriteria).ToList();
            if (activeColumnFilters.Count > 0)
            {
                if (activeColumnFilters.Count == 1)
                {
                    summaryParts.Add($"{activeColumnFilters[0].DisplayName}: {activeColumnFilters[0].FilterSummary}");
                }
                else
                {
                    summaryParts.Add($"{activeColumnFilters.Count} column filters");
                }
            }
            
            return summaryParts.Count > 0 
                ? string.Join(", ", summaryParts)
                : "No active filters";
        }
    }
    
    /// <summary>
    /// Initializes a new filter configuration.
    /// </summary>
    public FilterConfiguration()
    {
        
    }
    
    /// <summary>
    /// Initializes a new filter configuration with the specified ID and name.
    /// </summary>
    public FilterConfiguration(string configurationId, string displayName)
    {
        ConfigurationId = configurationId;
        DisplayName = displayName;
    }
    
    /// <summary>
    /// Initializes column filters based on the provided columns.
    /// </summary>
    public void InitializeFromColumns(ObservableCollection<CustomDataGridColumn> columns)
    {
        ColumnFilters.Clear();
        
        foreach (var column in columns.Where(c => c.CanFilter))
        {
            ColumnFilters.Add(new ColumnFilter(column.PropertyName, column.DisplayName, column.DataType));
        }
        
        LastModified = DateTime.Now;
    }
    
    /// <summary>
    /// Clears all filters and deactivates them.
    /// </summary>
    public void ClearAllFilters()
    {
        GlobalSearchText = string.Empty;
        
        foreach (var filter in ColumnFilters)
        {
            filter.Clear();
        }
        
        LastModified = DateTime.Now;
    }
    
    /// <summary>
    /// Evaluates whether the given item matches all active filters.
    /// </summary>
    public bool MatchesFilters<T>(T item)
    {
        if (!IsActive || !HasActiveFilters)
            return true;
            
        // Check global search
        if (!string.IsNullOrWhiteSpace(GlobalSearchText))
        {
            if (!MatchesGlobalSearch(item))
                return false;
        }
        
        // Check column-specific filters
        foreach (var filter in ColumnFilters.Where(f => f.IsActive && f.HasValidCriteria))
        {
            var propertyInfo = typeof(T).GetProperty(filter.PropertyName);
            var value = propertyInfo?.GetValue(item);
            
            if (!filter.MatchesCriteria(value))
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// Updates the last modified timestamp.
    /// </summary>
    public void UpdateLastModified()
    {
        LastModified = DateTime.Now;
    }
    
    /// <summary>
    /// Gets the column filter for the specified property name.
    /// </summary>
    public ColumnFilter? GetColumnFilter(string propertyName)
    {
        return ColumnFilters.FirstOrDefault(f => f.PropertyName == propertyName);
    }
    
    #region Private Methods
    
    private bool MatchesGlobalSearch<T>(T item)
    {
        if (string.IsNullOrWhiteSpace(GlobalSearchText))
            return true;
            
        var searchText = IsGlobalSearchCaseSensitive ? GlobalSearchText : GlobalSearchText.ToLowerInvariant();
        
        // Search in all filterable columns
        var searchableColumns = ColumnFilters.Where(f => f.DataType == typeof(string));
        
        foreach (var filter in searchableColumns)
        {
            var propertyInfo = typeof(T).GetProperty(filter.PropertyName);
            var value = propertyInfo?.GetValue(item)?.ToString();
            
            if (!string.IsNullOrWhiteSpace(value))
            {
                var valueToSearch = IsGlobalSearchCaseSensitive ? value : value.ToLowerInvariant();
                if (valueToSearch.Contains(searchText))
                    return true;
            }
        }
        
        return false;
    }
    
    #endregion
}

/// <summary>
/// Represents filter statistics and information for display in the UI.
/// Phase 5 feature for filter result tracking and user feedback.
/// </summary>
public class FilterStatistics
{
    /// <summary>
    /// Gets or sets the total number of items before filtering.
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items after filtering.
    /// </summary>
    public int FilteredCount { get; set; }
    
    /// <summary>
    /// Gets or sets the number of items hidden by filters.
    /// </summary>
    public int HiddenCount => TotalCount - FilteredCount;
    
    /// <summary>
    /// Gets or sets whether any filters are currently applied.
    /// </summary>
    public bool HasActiveFilters { get; set; }
    
    /// <summary>
    /// Gets the percentage of items visible after filtering.
    /// </summary>
    public double VisibilityPercentage => TotalCount > 0 ? (double)FilteredCount / TotalCount * 100 : 100;
    
    /// <summary>
    /// Gets a summary string describing the filter results.
    /// </summary>
    public string FilterResultSummary => HasActiveFilters
        ? FilteredCount == 0 
            ? "No items match the current filters"
            : $"Showing {FilteredCount} of {TotalCount} items ({VisibilityPercentage:F0}% visible)"
        : $"Showing all {TotalCount} items";
}