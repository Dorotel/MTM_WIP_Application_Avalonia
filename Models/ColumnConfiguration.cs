using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Column configuration model for TransferTabView DataGrid customization.
    /// Represents user preferences for column visibility, order, and sizing.
    /// Integrates with MySQL usr_ui_settings table for persistence.
    /// </summary>
    public class ColumnConfiguration
    {
        /// <summary>
        /// List of visible column names in the DataGrid
        /// </summary>
        public List<string> VisibleColumns { get; set; } = new();

        /// <summary>
        /// Dictionary mapping column names to their display order (0-based index)
        /// </summary>
        public Dictionary<string, int> ColumnOrder { get; set; } = new();

        /// <summary>
        /// Dictionary mapping column names to their pixel widths
        /// </summary>
        public Dictionary<string, int> ColumnWidths { get; set; } = new();

        /// <summary>
        /// Timestamp of last modification
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.Now;

        /// <summary>
        /// Default column configuration for TransferTabView
        /// </summary>
        public static ColumnConfiguration Default => new()
        {
            VisibleColumns = new List<string>
            {
                "PartID", "Operation", "FromLocation", "AvailableQuantity", "TransferQuantity", "Notes"
            },
            ColumnOrder = new Dictionary<string, int>
            {
                { "PartID", 0 }, { "Operation", 1 }, { "FromLocation", 2 },
                { "AvailableQuantity", 3 }, { "TransferQuantity", 4 }, { "Notes", 5 }
            },
            ColumnWidths = new Dictionary<string, int>
            {
                { "PartID", 120 }, { "Operation", 80 }, { "FromLocation", 100 },
                { "AvailableQuantity", 120 }, { "TransferQuantity", 120 }, { "Notes", 200 }
            },
            LastModified = DateTime.Now
        };

        /// <summary>
        /// Validates the column configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            return VisibleColumns != null &&
                   VisibleColumns.Count > 0 &&
                   ColumnOrder != null &&
                   ColumnWidths != null;
        }

        /// <summary>
        /// Updates the LastModified timestamp
        /// </summary>
        public void UpdateLastModified()
        {
            LastModified = DateTime.Now;
        }

        /// <summary>
        /// Creates a deep copy of the configuration
        /// </summary>
        /// <returns>New ColumnConfiguration instance</returns>
        public ColumnConfiguration Clone()
        {
            return new ColumnConfiguration
            {
                VisibleColumns = new List<string>(VisibleColumns),
                ColumnOrder = new Dictionary<string, int>(ColumnOrder),
                ColumnWidths = new Dictionary<string, int>(ColumnWidths),
                LastModified = LastModified
            };
        }

        /// <summary>
        /// Merges another configuration into this one
        /// </summary>
        /// <param name="other">Configuration to merge</param>
        public void MergeWith(ColumnConfiguration other)
        {
            if (other == null) return;

            if (other.VisibleColumns?.Count > 0)
                VisibleColumns = new List<string>(other.VisibleColumns);

            if (other.ColumnOrder?.Count > 0)
                ColumnOrder = new Dictionary<string, int>(other.ColumnOrder);

            if (other.ColumnWidths?.Count > 0)
                ColumnWidths = new Dictionary<string, int>(other.ColumnWidths);

            UpdateLastModified();
        }

        /// <summary>
        /// Gets column width for a specific column, returning default if not found
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="defaultWidth">Default width to return if not found</param>
        /// <returns>Column width in pixels</returns>
        public int GetColumnWidth(string columnName, int defaultWidth = 100)
        {
            return ColumnWidths.TryGetValue(columnName, out var width) ? width : defaultWidth;
        }

        /// <summary>
        /// Gets column order for a specific column, returning default if not found
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="defaultOrder">Default order to return if not found</param>
        /// <returns>Column order index</returns>
        public int GetColumnOrder(string columnName, int defaultOrder = 999)
        {
            return ColumnOrder.TryGetValue(columnName, out var order) ? order : defaultOrder;
        }

        /// <summary>
        /// Checks if a column is visible
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <returns>True if column should be visible</returns>
        public bool IsColumnVisible(string columnName)
        {
            return VisibleColumns.Contains(columnName);
        }
    }
}
