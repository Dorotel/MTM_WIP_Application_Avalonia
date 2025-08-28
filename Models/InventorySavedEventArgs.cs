using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Event arguments for when an inventory item is successfully saved.
/// Contains the data needed to add the item to QuickButtons.
/// </summary>
public class InventorySavedEventArgs : EventArgs
{
    /// <summary>
    /// The Part ID that was saved.
    /// </summary>
    public string PartId { get; set; } = string.Empty;

    /// <summary>
    /// The Operation number that was saved.
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// The Quantity that was saved.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The Location where the inventory was saved.
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Any notes associated with the save operation.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the InventorySavedEventArgs class.
    /// </summary>
    public InventorySavedEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the InventorySavedEventArgs class with specific values.
    /// </summary>
    /// <param name="partId">The Part ID</param>
    /// <param name="operation">The Operation number</param>
    /// <param name="quantity">The Quantity</param>
    /// <param name="location">The Location</param>
    /// <param name="notes">The Notes</param>
    public InventorySavedEventArgs(string partId, string operation, int quantity, string location, string notes = "")
    {
        PartId = partId;
        Operation = operation;
        Quantity = quantity;
        Location = location;
        Notes = notes;
    }
}
