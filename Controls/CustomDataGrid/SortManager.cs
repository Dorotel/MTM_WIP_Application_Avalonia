using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;
using MTM_WIP_Application_Avalonia.Models.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Interface for sort management operations
    /// Simplified for Avalonia UI compatibility
    /// </summary>
    public interface ISortManager
    {
        /// <summary>
        /// Applies single column sort to a data source
        /// </summary>
        IEnumerable<object> ApplySingleColumnSort(IEnumerable source, string columnId, SortDirection direction);

        /// <summary>
        /// Applies multi-column sort to a data source
        /// </summary>
        IEnumerable<object> ApplyMultiColumnSort(IEnumerable source, IEnumerable<SortCriteria> sortCriteria);

        /// <summary>
        /// Clears all sorting from a data source
        /// </summary>
        IEnumerable ClearSort(IEnumerable source);

        /// <summary>
        /// Gets a sorted collection from the provided data source
        /// </summary>
        IEnumerable GetSortedView(IEnumerable source, SortConfiguration sortConfiguration);
    }

    /// <summary>
    /// Sort manager service for CustomDataGrid sorting operations
    /// Simplified implementation for Avalonia UI
    /// </summary>
    public partial class SortManager : ISortManager
    {
        private readonly ILogger<SortManager>? _logger;

        /// <summary>
        /// Initializes a new instance of the SortManager
        /// </summary>
        public SortManager(ILogger<SortManager>? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Applies single column sort to a data source
        /// </summary>
        public IEnumerable<object> ApplySingleColumnSort(IEnumerable source, string columnId, SortDirection direction)
        {
            var items = source?.Cast<object>() ?? Enumerable.Empty<object>();

            if (direction == SortDirection.None)
            {
                return items.ToList();
            }

            try
            {
                return direction == SortDirection.Ascending
                    ? items.OrderBy(item => GetPropertyValue(item, columnId)).ToList()
                    : items.OrderByDescending(item => GetPropertyValue(item, columnId)).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error applying single column sort for column: {ColumnId}", columnId);
                return items.ToList();
            }
        }

        /// <summary>
        /// Applies multi-column sort to a data source
        /// </summary>
        public IEnumerable<object> ApplyMultiColumnSort(IEnumerable source, IEnumerable<SortCriteria> sortCriteria)
        {
            var items = source?.Cast<object>() ?? Enumerable.Empty<object>();
            var criteriaList = sortCriteria?.ToList() ?? new List<SortCriteria>();

            if (!criteriaList.Any())
            {
                return items.ToList();
            }

            try
            {
                IOrderedEnumerable<object>? orderedItems = null;

                for (int i = 0; i < criteriaList.Count; i++)
                {
                    var criteria = criteriaList[i];

                    if (i == 0)
                    {
                        orderedItems = criteria.Direction == SortDirection.Ascending
                            ? items.OrderBy(item => GetPropertyValue(item, criteria.ColumnId))
                            : items.OrderByDescending(item => GetPropertyValue(item, criteria.ColumnId));
                    }
                    else
                    {
                        orderedItems = criteria.Direction == SortDirection.Ascending
                            ? orderedItems!.ThenBy(item => GetPropertyValue(item, criteria.ColumnId))
                            : orderedItems!.ThenByDescending(item => GetPropertyValue(item, criteria.ColumnId));
                    }
                }

                return orderedItems?.ToList() ?? items.ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error applying multi-column sort");
                return items.ToList();
            }
        }

        /// <summary>
        /// Gets a sorted view of the data using the provided sort configuration
        /// </summary>
        public IEnumerable GetSortedView(IEnumerable source, SortConfiguration sortConfiguration)
        {
            if (source == null || sortConfiguration == null)
            {
                return source ?? Enumerable.Empty<object>();
            }

            try
            {
                var activeSorts = sortConfiguration.GetActiveSortCriteria().ToList();
                if (!activeSorts.Any())
                {
                    return source;
                }

                var sortedItems = ApplyMultiColumnSort(source, activeSorts);
                return sortedItems;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error getting sorted view");
                return source;
            }
        }

        /// <summary>
        /// Returns the original unsorted data
        /// </summary>
        public IEnumerable ClearSort(IEnumerable source)
        {
            return source ?? Enumerable.Empty<object>();
        }

        /// <summary>
        /// Gets the value of a property from an object using reflection
        /// </summary>
        private static object? GetPropertyValue(object obj, string propertyPath)
        {
            if (obj == null || string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            try
            {
                var propertyInfo = obj.GetType().GetProperty(propertyPath, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return obj;
                }
                
                var value = propertyInfo.GetValue(obj);
                
                // Special handling for numeric strings (like Operation numbers)
                // This ensures proper sorting of values like "90", "100", "110"
                if (value is string strValue && !string.IsNullOrEmpty(strValue))
                {
                    // Try to parse as numeric for proper sorting
                    if (double.TryParse(strValue, out double numValue))
                    {
                        return numValue;
                    }
                }
                
                return value;
            }
            catch
            {
                // If property doesn't exist or can't be accessed, return the object itself
                return obj;
            }
        }
    }
}