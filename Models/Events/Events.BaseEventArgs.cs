using System;
using System.Collections.Generic;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.Models.Events;

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
/// Event arguments for triggering LostFocus events on specific TextBoxes.
/// </summary>
public class TriggerLostFocusEventArgs : EventArgs
{
    /// <summary>
    /// List of field names that should have their LostFocus events triggered.
    /// </summary>
    public List<string> FieldNames { get; set; } = new();

    /// <summary>
    /// Tab index where the fields are located (0=Inventory, 1=Remove, 2=Transfer).
    /// </summary>
    public int TabIndex { get; set; }

    /// <summary>
    /// Delay in milliseconds between triggering each field's LostFocus event.
    /// </summary>
    public int DelayBetweenFields { get; set; } = 100;

    /// <summary>
    /// If true, only focus the fields without triggering LostFocus events.
    /// If false, trigger LostFocus events as normal.
    /// </summary>
    public bool FocusOnly { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of TriggerLostFocusEventArgs.
    /// </summary>
    public TriggerLostFocusEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance with specific field names and tab index.
    /// </summary>
    /// <param name="fieldNames">Names of the fields to trigger LostFocus on</param>
    /// <param name="tabIndex">Tab containing the fields</param>
    /// <param name="delay">Delay between field events</param>
    /// <param name="focusOnly">If true, only focus without triggering LostFocus</param>
    public TriggerLostFocusEventArgs(List<string> fieldNames, int tabIndex, int delay = 100, bool focusOnly = false)
    {
        FieldNames = fieldNames ?? new List<string>();
        TabIndex = tabIndex;
        DelayBetweenFields = delay;
        FocusOnly = focusOnly;
    }

    /// <summary>
    /// Convenience constructor for a single field.
    /// </summary>
    /// <param name="fieldName">Name of the field to trigger LostFocus on</param>
    /// <param name="tabIndex">Tab containing the field</param>
    /// <param name="focusOnly">If true, only focus without triggering LostFocus</param>
    public TriggerLostFocusEventArgs(string fieldName, int tabIndex, bool focusOnly = false) : this(new List<string> { fieldName }, tabIndex, 100, focusOnly)
    {
    }
}

/// <summary>
/// Event arguments for items removed events.
/// </summary>
public class ItemsRemovedEventArgs : EventArgs
{
    public List<InventoryItem> RemovedItems { get; set; } = new();
    public int TotalQuantityRemoved { get; set; }
    public string Location { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime RemovalTime { get; set; } = DateTime.Now;
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

/// <summary>
/// Event arguments for success overlay display events.
/// </summary>
public class SuccessEventArgs : EventArgs
{
    /// <summary>
    /// The success message to display.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional details about the success.
    /// </summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>
    /// Material icon kind to display.
    /// </summary>
    public string IconKind { get; set; } = "CheckCircle";

    /// <summary>
    /// Display duration in milliseconds.
    /// </summary>
    public int Duration { get; set; } = 2000;

    /// <summary>
    /// Time when the success occurred.
    /// </summary>
    public DateTime SuccessTime { get; set; } = DateTime.Now;
}
