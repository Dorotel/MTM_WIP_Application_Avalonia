using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// ViewModel-specific models for UI binding compatibility.
/// These models provide simplified interfaces for UI binding while maintaining compatibility with the core models.
/// </summary>

/// <summary>
/// Comprehensive inventory item model matching all database columns.
/// Users can choose which columns to show/hide in the DataGrid.
/// </summary>
public class InventoryItem
{
    // Core identification
    public int Id { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    
    // Quantity and type information
    public int Quantity { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    
    // Date information - matching database schema
    public DateTime ReceiveDate { get; set; }    // Maps to database "ReceiveDate" column
    public DateTime LastUpdated { get; set; }    // Maps to database "LastUpdated" column  
    public DateTime DateAdded { get; set; }      // Alias for ReceiveDate (for UI compatibility)
    
    // User and tracking information
    public string User { get; set; } = string.Empty;
    public string LastUpdatedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    // Additional database columns that might be present
    public string Status { get; set; } = string.Empty;
    public string WorkOrder { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public decimal? Cost { get; set; }
    public string Vendor { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    // Display properties
    public string DisplayText => $"({Operation}) - [{PartId} x {Quantity}]";
    public string ShortDescription => $"{PartId} ({Quantity})";
    public string FullDescription => $"{PartId} - {Location} - Op: {Operation} - Qty: {Quantity}";
}

/// <summary>
/// Simplified transaction record model for UI binding.
/// Compatible with TransactionHistoryViewModel and other UI components.
/// </summary>
public class TransactionRecord
{
    public int TransactionId { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string User { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    public string DisplayText => $"{TransactionType}: {PartId} ({Quantity}) - {TransactionDate:MM/dd/yyyy HH:mm}";
}

/// <summary>
/// Simplified user info model for UI binding.
/// Compatible with UserManagementViewModel and other UI components.
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string DisplayName => !string.IsNullOrWhiteSpace(FullName) ? FullName : Username;
    public string StatusDisplay => IsActive ? "Active" : "Inactive";
}