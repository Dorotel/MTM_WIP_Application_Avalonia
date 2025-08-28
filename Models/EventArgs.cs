using System;
using System.Collections.Generic;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Event arguments for quick action execution events.
/// </summary>
public class QuickActionExecutedEventArgs : EventArgs
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
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
