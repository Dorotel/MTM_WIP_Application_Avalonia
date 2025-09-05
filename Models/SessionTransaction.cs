using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Represents a transaction in the current session for display in the history panel.
/// This model is used for real-time session tracking in the InventoryTab transaction history.
/// </summary>
public class SessionTransaction
{
    /// <summary>
    /// The time when the transaction was performed
    /// </summary>
    public DateTime TransactionTime { get; set; }
    
    /// <summary>
    /// The Part ID involved in the transaction
    /// </summary>
    public string PartId { get; set; } = string.Empty;
    
    /// <summary>
    /// The operation number (workflow step like "90", "100", "110")
    /// </summary>
    public string Operation { get; set; } = string.Empty;
    
    /// <summary>
    /// The location where the transaction occurred
    /// </summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// The quantity involved in the transaction
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// The item type (typically "WIP")
    /// </summary>
    public string ItemType { get; set; } = string.Empty;
    
    /// <summary>
    /// The batch number generated for this transaction
    /// </summary>
    public string BatchNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// The status of the transaction (Success, Failed, Error)
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional notes provided with the transaction
    /// </summary>
    public string Notes { get; set; } = string.Empty;
    
    /// <summary>
    /// The user who performed the transaction
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// The type of transaction (IN, OUT, TRANSFER)
    /// </summary>
    public string TransactionType { get; set; } = "IN";
}
