using System;
using System.Collections.Generic;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Event arguments for quick action execution events.
/// </summary>
public class QuickActionExecutedEventArgs : EventArgs
{
    /// <summary>
    /// Part ID from the quick action.
    /// </summary>
    public string PartId { get; set; } = string.Empty;

    /// <summary>
    /// Operation from the quick action.
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Quantity from the quick action.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Location (optional, for some quick actions).
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Notes (optional, for some quick actions).
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Position of the quick button that was executed.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Time when the quick action was executed.
    /// </summary>
    public DateTime ExecutionTime { get; set; } = DateTime.Now;

    /// <summary>
    /// User who executed the quick action.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// Display text of the quick button.
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the QuickActionExecutedEventArgs class.
    /// </summary>
    public QuickActionExecutedEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the QuickActionExecutedEventArgs class with specific values.
    /// </summary>
    /// <param name="partId">The Part ID</param>
    /// <param name="operation">The Operation number</param>
    /// <param name="quantity">The Quantity</param>
    /// <param name="position">The button position</param>
    /// <param name="user">The user executing the action</param>
    public QuickActionExecutedEventArgs(string partId, string operation, int quantity, int position = 0, string user = "")
    {
        PartId = partId;
        Operation = operation;
        Quantity = quantity;
        Position = position;
        User = user;
        ExecutionTime = DateTime.Now;
        DisplayText = $"{partId} - {operation} ({quantity})";
    }
}

/// <summary>
/// Event arguments for items removed events.
/// </summary>
public class ItemsRemovedEventArgs : EventArgs
{
    public List<RemovedItem> RemovedItems { get; set; } = new();
    public int TotalQuantityRemoved { get; set; }
    public string Location { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime RemovedDateTime { get; set; } = DateTime.Now;
}

/// <summary>
/// Event arguments for items transferred events.
/// </summary>
public class ItemsTransferredEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int TransferredQuantity { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime TransferredDateTime { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Represents a removed inventory item.
/// </summary>
public class RemovedItem
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime RemovedDate { get; set; } = DateTime.Now;
    public string Notes { get; set; } = string.Empty;
}
