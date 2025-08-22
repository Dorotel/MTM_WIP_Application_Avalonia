using System;
using System.Collections.Generic;

namespace MTM.Models
{
    /// <summary>
    /// Represents a user in the MTM system.
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents an inventory item in the MTM system.
    /// </summary>
    public class InventoryItem
    {
        public string PartId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // String number like "90", "100", "110"
        public DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; } = string.Empty;
        public decimal UnitCost { get; set; }
        public string UnitOfMeasure { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents an inventory transaction in the MTM system.
    /// </summary>
    public class InventoryTransaction
    {
        public int TransactionId { get; set; }
        public string PartId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty; // String number
        public string Location { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // "IN", "OUT", "TRANSFER"
        public DateTime TransactionDateTime { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }

    /// <summary>
    /// Connection status enumeration for application state.
    /// </summary>
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Failed,
        Error
    }

    /// <summary>
    /// Validation result for business rule validation.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public Dictionary<string, List<string>> PropertyErrors { get; set; } = new();

        public static ValidationResult Success() => new() { IsValid = true };
        
        public static ValidationResult Failure(string errorMessage) => new() 
        { 
            IsValid = false, 
            ErrorMessages = { errorMessage } 
        };

        public static ValidationResult Failure(IEnumerable<string> errorMessages) => new() 
        { 
            IsValid = false, 
            ErrorMessages = new List<string>(errorMessages) 
        };
    }
}