using System;
using System.Collections.Generic;

namespace MTM_WIP_Application_Avalonia.Models.CustomDataGrid
{
    /// <summary>
    /// Event arguments for selection change events in CustomDataGrid
    /// </summary>
    public class SelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the list of selected items
        /// </summary>
        public List<object> SelectedItems { get; set; } = new();

        /// <summary>
        /// Gets or sets the selection mode (Selected, Deselected, etc.)
        /// </summary>
        public string SelectionMode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the timestamp of the selection change
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the total count of selected items
        /// </summary>
        public int SelectedCount => SelectedItems?.Count ?? 0;

        /// <summary>
        /// Gets whether any items are selected
        /// </summary>
        public bool HasSelection => SelectedCount > 0;
    }
}