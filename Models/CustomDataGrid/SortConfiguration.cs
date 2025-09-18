using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Models.CustomDataGrid
{
    /// <summary>
    /// Enumeration for sort directions
    /// </summary>
    public enum SortDirection
    {
        None,
        Ascending,
        Descending
    }

    /// <summary>
    /// Configuration for a single column sort criteria
    /// Follows MTM MVVM Community Toolkit patterns
    /// </summary>
    public partial class SortCriteria : ObservableObject
    {
        /// <summary>
        /// Gets or sets the column identifier being sorted
        /// </summary>
        [ObservableProperty]
        private string columnId = string.Empty;

        /// <summary>
        /// Gets or sets the sort direction
        /// </summary>
        [ObservableProperty]
        private SortDirection direction = SortDirection.None;

        /// <summary>
        /// Gets or sets the sort precedence (0 = primary, 1 = secondary, etc.)
        /// </summary>
        [ObservableProperty]
        private int precedence = 0;

        /// <summary>
        /// Gets or sets whether this sort criteria is currently active
        /// </summary>
        [ObservableProperty]
        private bool isActive = true;

        /// <summary>
        /// Gets or sets the data type of the column being sorted
        /// Used for proper sort comparisons
        /// </summary>
        [ObservableProperty]
        private string dataType = "String";

        /// <summary>
        /// Gets or sets the property path for complex object sorting
        /// </summary>
        [ObservableProperty]
        private string propertyPath = string.Empty;

        /// <summary>
        /// Constructor for SortCriteria
        /// </summary>
        public SortCriteria()
        {
        }

        /// <summary>
        /// Constructor for SortCriteria with parameters
        /// </summary>
        /// <param name="columnId">The column identifier</param>
        /// <param name="direction">The sort direction</param>
        /// <param name="precedence">The sort precedence</param>
        public SortCriteria(string columnId, SortDirection direction, int precedence = 0)
        {
            ColumnId = columnId;
            Direction = direction;
            Precedence = precedence;
            IsActive = true; // Default to active when created with parameters
        }

        /// <summary>
        /// Creates a copy of this sort criteria
        /// </summary>
        /// <returns>New SortCriteria instance with copied values</returns>
        public SortCriteria Clone()
        {
            return new SortCriteria
            {
                ColumnId = ColumnId,
                Direction = Direction,
                Precedence = Precedence,
                IsActive = IsActive,
                DataType = DataType,
                PropertyPath = PropertyPath
            };
        }

        /// <summary>
        /// Gets the next sort direction in the cycle (None → Ascending → Descending → None)
        /// </summary>
        /// <returns>The next sort direction</returns>
        public SortDirection GetNextDirection()
        {
            return Direction switch
            {
                SortDirection.None => SortDirection.Ascending,
                SortDirection.Ascending => SortDirection.Descending,
                SortDirection.Descending => SortDirection.None,
                _ => SortDirection.None
            };
        }

        /// <summary>
        /// Validates the sort criteria configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ColumnId) &&
                   Precedence >= 0 &&
                   !string.IsNullOrWhiteSpace(DataType);
        }

        /// <summary>
        /// Returns a string representation of the sort criteria
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            var directionText = Direction switch
            {
                SortDirection.Ascending => "↑",
                SortDirection.Descending => "↓",
                _ => "○"
            };

            return $"{ColumnId} {directionText} ({Precedence})";
        }
    }

    /// <summary>
    /// Collection of sort criteria for managing multi-column sorting
    /// </summary>
    public partial class SortConfiguration : ObservableObject
    {
        /// <summary>
        /// Maximum number of columns that can be sorted simultaneously
        /// </summary>
        public const int MaxSortColumns = 3;

        /// <summary>
        /// Gets or sets the collection of sort criteria
        /// </summary>
        [ObservableProperty]
        private List<SortCriteria> sortCriteria = new();

        /// <summary>
        /// Gets or sets whether sorting is currently enabled
        /// </summary>
        [ObservableProperty]
        private bool isSortingEnabled = true;

        /// <summary>
        /// Gets or sets whether multi-column sorting is enabled
        /// </summary>
        [ObservableProperty]
        private bool isMultiColumnSortEnabled = true;

        /// <summary>
        /// Adds a sort criteria or updates existing one
        /// </summary>
        /// <param name="criteria">The sort criteria to add or update</param>
        public void AddOrUpdateSort(SortCriteria criteria)
        {
            if (criteria == null || !criteria.IsValid())
                return;

            var existing = SortCriteria.FirstOrDefault(c => c.ColumnId == criteria.ColumnId);
            if (existing != null)
            {
                // Update existing criteria
                existing.Direction = criteria.Direction;
                existing.IsActive = criteria.Direction != SortDirection.None;
            }
            else
            {
                // Add new criteria
                criteria.Precedence = GetNextPrecedence();
                SortCriteria.Add(criteria);
            }

            // Clean up inactive sorts and maintain precedence order
            CleanupAndReorderSorts();
        }

        /// <summary>
        /// Removes a sort criteria for a specific column
        /// </summary>
        /// <param name="columnId">The column identifier to remove</param>
        public void RemoveSort(string columnId)
        {
            var toRemove = SortCriteria.Where(c => c.ColumnId == columnId).ToList();
            foreach (var criteria in toRemove)
            {
                SortCriteria.Remove(criteria);
            }

            ReorderPrecedence();
        }

        /// <summary>
        /// Clears all sort criteria
        /// </summary>
        public void ClearSort()
        {
            SortCriteria.Clear();
        }

        /// <summary>
        /// Gets the active sort criteria ordered by precedence
        /// </summary>
        /// <returns>List of active sort criteria</returns>
        public List<SortCriteria> GetActiveSorts()
        {
            return SortCriteria
                .Where(c => c.IsActive && c.Direction != SortDirection.None)
                .OrderBy(c => c.Precedence)
                .ToList();
        }

        /// <summary>
        /// Gets the sort criteria for a specific column
        /// </summary>
        /// <param name="columnId">The column identifier</param>
        /// <returns>Sort criteria or null if not found</returns>
        public SortCriteria? GetSortForColumn(string columnId)
        {
            return SortCriteria.FirstOrDefault(c => c.ColumnId == columnId);
        }

        /// <summary>
        /// Checks if a column is currently being sorted
        /// </summary>
        /// <param name="columnId">The column identifier</param>
        /// <returns>True if the column is being sorted</returns>
        public bool IsColumnSorted(string columnId)
        {
            var criteria = GetSortForColumn(columnId);
            return criteria != null && criteria.IsActive && criteria.Direction != SortDirection.None;
        }

        /// <summary>
        /// Gets the next available precedence value
        /// </summary>
        /// <returns>Next precedence value</returns>
        private int GetNextPrecedence()
        {
            var activeSorts = GetActiveSorts();
            return activeSorts.Count > 0 ? activeSorts.Max(c => c.Precedence) + 1 : 0;
        }

        /// <summary>
        /// Cleans up inactive sorts and reorders precedence
        /// </summary>
        private void CleanupAndReorderSorts()
        {
            // Remove inactive sorts
            var toRemove = SortCriteria.Where(c => !c.IsActive || c.Direction == SortDirection.None).ToList();
            foreach (var criteria in toRemove)
            {
                SortCriteria.Remove(criteria);
            }

            // Enforce maximum sort columns
            if (SortCriteria.Count > MaxSortColumns)
            {
                var excess = SortCriteria.OrderBy(c => c.Precedence).Skip(MaxSortColumns).ToList();
                foreach (var criteria in excess)
                {
                    SortCriteria.Remove(criteria);
                }
            }

            ReorderPrecedence();
        }

        /// <summary>
        /// Reorders the precedence values to be sequential starting from 0
        /// </summary>
        private void ReorderPrecedence()
        {
            var activeSorts = SortCriteria.OrderBy(c => c.Precedence).ToList();
            for (int i = 0; i < activeSorts.Count; i++)
            {
                activeSorts[i].Precedence = i;
            }
        }

        /// <summary>
        /// Creates a copy of this sort configuration
        /// </summary>
        /// <returns>New SortConfiguration instance with copied values</returns>
        public SortConfiguration Clone()
        {
            var clone = new SortConfiguration
            {
                IsSortingEnabled = IsSortingEnabled,
                IsMultiColumnSortEnabled = IsMultiColumnSortEnabled
            };

            foreach (var criteria in SortCriteria)
            {
                clone.SortCriteria.Add(criteria.Clone());
            }

            return clone;
        }

        /// <summary>
        /// Validates the sort configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return SortCriteria.All(c => c.IsValid()) &&
                   SortCriteria.Count <= MaxSortColumns &&
                   SortCriteria.Select(c => c.ColumnId).Distinct().Count() == SortCriteria.Count;
        }

        /// <summary>
        /// Gets a summary of the current sort state
        /// </summary>
        /// <returns>Sort state summary</returns>
        public string GetSortSummary()
        {
            var activeSorts = GetActiveSorts();
            if (activeSorts.Count == 0)
                return "No sorting";

            var descriptions = activeSorts.Select(c => c.ToString());
            return string.Join(", ", descriptions);
        }
    }
}