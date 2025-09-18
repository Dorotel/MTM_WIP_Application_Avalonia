using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Sort direction enumeration for column sorting
    /// </summary>
    public enum SortDirection
    {
        None = 0,
        Ascending = 1,
        Descending = 2
    }

    /// <summary>
    /// Represents sorting criteria for a single column
    /// Follows MTM MVVM Community Toolkit patterns
    /// </summary>
    public partial class SortCriteria : ObservableObject
    {
        /// <summary>
        /// Gets or sets the column identifier for sorting
        /// </summary>
        [ObservableProperty]
        private string columnId = string.Empty;

        /// <summary>
        /// Gets or sets the sort direction (None, Ascending, Descending)
        /// </summary>
        [ObservableProperty]
        private SortDirection direction = SortDirection.None;

        /// <summary>
        /// Gets or sets the sort precedence for multi-column sorting (0 = primary, 1 = secondary, etc.)
        /// </summary>
        [ObservableProperty]
        private int precedence;

        /// <summary>
        /// Gets or sets whether this sort criteria is currently active
        /// </summary>
        [ObservableProperty]
        private bool isActive;

        /// <summary>
        /// Gets or sets the data type for proper sorting logic
        /// </summary>
        [ObservableProperty]
        private Type? dataType;

        /// <summary>
        /// Creates a new SortCriteria instance
        /// </summary>
        public SortCriteria()
        {
        }

        /// <summary>
        /// Creates a new SortCriteria instance with specified values
        /// </summary>
        /// <param name="columnId">Column identifier</param>
        /// <param name="direction">Sort direction</param>
        /// <param name="precedence">Sort precedence</param>
        public SortCriteria(string columnId, SortDirection direction, int precedence = 0)
        {
            ColumnId = columnId;
            Direction = direction;
            Precedence = precedence;
            IsActive = direction != SortDirection.None;
        }

        /// <summary>
        /// Creates a deep copy of this sort criteria
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
                DataType = DataType
            };
        }

        /// <summary>
        /// Returns a string representation of this sort criteria
        /// </summary>
        public override string ToString()
        {
            return $"{ColumnId} ({Direction}) [Priority: {Precedence}]";
        }
    }

    /// <summary>
    /// Configuration class for managing column sorting in CustomDataGrid
    /// Supports single-column and multi-column sorting with precedence
    /// Phase 2 enhancement for MTM Custom Data Grid Control
    /// </summary>
    public partial class SortConfiguration : ObservableObject
    {
        #region Constants

        /// <summary>
        /// Maximum number of simultaneous sort columns supported
        /// </summary>
        public const int MaxSortColumns = 3;

        #endregion

        #region Observable Properties

        /// <summary>
        /// Gets or sets the collection of active sort criteria
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SortCriteria> sortCriteria = new();

        /// <summary>
        /// Gets or sets whether multi-column sorting is enabled
        /// </summary>
        [ObservableProperty]
        private bool isMultiColumnSortEnabled = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of SortConfiguration
        /// </summary>
        public SortConfiguration()
        {
            SortCriteria.CollectionChanged += OnSortCriteriaCollectionChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies single column sort, clearing any existing sorts
        /// </summary>
        /// <param name="columnId">Column to sort by</param>
        /// <param name="direction">Sort direction</param>
        public void ApplySingleColumnSort(string columnId, SortDirection direction)
        {
            ClearAllSorts();

            if (direction != SortDirection.None)
            {
                var criteria = new SortCriteria(columnId, direction, 0)
                {
                    IsActive = true  // Ensure the sort criteria is active
                };
                SortCriteria.Add(criteria);
            }
        }

        /// <summary>
        /// Applies multi-column sort, maintaining existing sorts and adding/updating the specified column
        /// </summary>
        /// <param name="columnId">Column to sort by</param>
        /// <param name="direction">Sort direction</param>
        public void ApplyMultiColumnSort(string columnId, SortDirection direction)
        {
            if (!IsMultiColumnSortEnabled)
            {
                ApplySingleColumnSort(columnId, direction);
                return;
            }

            // Find existing sort criteria for this column
            var existing = SortCriteria.FirstOrDefault(s => s.ColumnId == columnId);

            if (existing != null)
            {
                if (direction == SortDirection.None)
                {
                    // Remove this sort criteria
                    SortCriteria.Remove(existing);
                    ReorderPrecedence();
                }
                else
                {
                    // Update existing sort direction
                    existing.Direction = direction;
                    existing.IsActive = true;
                }
            }
            else if (direction != SortDirection.None)
            {
                // Add new sort criteria if we haven't reached the limit
                if (SortCriteria.Count < MaxSortColumns)
                {
                    var newCriteria = new SortCriteria(columnId, direction, SortCriteria.Count)
                    {
                        IsActive = true  // Ensure the sort criteria is active
                    };
                    SortCriteria.Add(newCriteria);
                }
                else
                {
                    // Replace the last sort criteria
                    var lastCriteria = SortCriteria.OrderByDescending(s => s.Precedence).First();
                    SortCriteria.Remove(lastCriteria);
                    
                    var newCriteria = new SortCriteria(columnId, direction, MaxSortColumns - 1)
                    {
                        IsActive = true  // Ensure the sort criteria is active
                    };
                    SortCriteria.Add(newCriteria);
                }
            }
        }

        /// <summary>
        /// Gets the sort direction for a specific column
        /// </summary>
        /// <param name="columnId">Column identifier</param>
        /// <returns>Sort direction for the column, or None if not sorted</returns>
        public SortDirection GetSortDirection(string columnId)
        {
            var criteria = SortCriteria.FirstOrDefault(s => s.ColumnId == columnId && s.IsActive);
            return criteria?.Direction ?? SortDirection.None;
        }

        /// <summary>
        /// Gets the sort precedence for a specific column
        /// </summary>
        /// <param name="columnId">Column identifier</param>
        /// <returns>Sort precedence (0-based), or -1 if not sorted</returns>
        public int GetSortPrecedence(string columnId)
        {
            var criteria = SortCriteria.FirstOrDefault(s => s.ColumnId == columnId && s.IsActive);
            return criteria?.Precedence ?? -1;
        }

        /// <summary>
        /// Checks if a specific column is currently being sorted
        /// </summary>
        /// <param name="columnId">Column identifier</param>
        /// <returns>True if the column is actively sorted, false otherwise</returns>
        public bool IsColumnSorted(string columnId)
        {
            return SortCriteria.Any(s => s.ColumnId == columnId && s.IsActive && s.Direction != SortDirection.None);
        }

        /// <summary>
        /// Gets all active sort criteria ordered by precedence
        /// </summary>
        /// <returns>Collection of active sort criteria</returns>
        public IEnumerable<SortCriteria> GetActiveSortCriteria()
        {
            return SortCriteria
                .Where(s => s.IsActive && s.Direction != SortDirection.None)
                .OrderBy(s => s.Precedence);
        }

        /// <summary>
        /// Determines whether any sort criteria are currently active
        /// </summary>
        /// <returns>True if there are active sorts, false otherwise</returns>
        public bool HasActiveSorts()
        {
            return SortCriteria.Any(s => s.IsActive && s.Direction != SortDirection.None);
        }

        /// <summary>
        /// Clears all sort criteria
        /// </summary>
        public void ClearAllSorts()
        {
            SortCriteria.Clear();
        }

        /// <summary>
        /// Creates a deep copy of this sort configuration
        /// </summary>
        /// <returns>New SortConfiguration instance with copied values</returns>
        public SortConfiguration Clone()
        {
            var clone = new SortConfiguration
            {
                IsMultiColumnSortEnabled = IsMultiColumnSortEnabled
            };

            foreach (var criteria in SortCriteria)
            {
                clone.SortCriteria.Add(criteria.Clone());
            }

            return clone;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reorders precedence values after removing a sort criteria
        /// </summary>
        private void ReorderPrecedence()
        {
            var activeCriteria = SortCriteria
                .Where(s => s.IsActive && s.Direction != SortDirection.None)
                .OrderBy(s => s.Precedence)
                .ToList();

            for (int i = 0; i < activeCriteria.Count; i++)
            {
                activeCriteria[i].Precedence = i;
            }
        }

        /// <summary>
        /// Handles changes to the sort criteria collection
        /// </summary>
        private void OnSortCriteriaCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Could add logging or additional validation here if needed
            OnPropertyChanged(nameof(SortCriteria));
        }

        #endregion

        #region IDisposable Implementation

        private bool _disposed = false;

        /// <summary>
        /// Disposes of resources used by this configuration
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (SortCriteria != null)
                {
                    SortCriteria.CollectionChanged -= OnSortCriteriaCollectionChanged;
                    SortCriteria.Clear();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes of this configuration
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Event arguments for sort requests from the UI
    /// </summary>
    public class SortRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the column identifier for the sort request
        /// </summary>
        public string ColumnId { get; init; } = string.Empty;

        /// <summary>
        /// Gets whether this is a multi-column sort request (Shift+Click)
        /// </summary>
        public bool IsMultiColumn { get; init; }

        /// <summary>
        /// Gets the requested sort direction
        /// </summary>
        public SortDirection RequestedDirection { get; init; } = SortDirection.None;
    }
}