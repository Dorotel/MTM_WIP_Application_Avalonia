using System;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Simple inventory item model for demonstration purposes.
/// Contains the basic properties expected by the CustomDataGrid demo.
/// </summary>
public class DemoInventoryItem
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Gets the display text for the inventory item in lists.
    /// Format: (Operation) - [PartId x Quantity]
    /// </summary>
    public string DisplayText => $"({Operation}) - [{PartId} x {Quantity}]";
}