using System;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// ViewModel-specific models for UI binding compatibility.
/// These models provide simplified interfaces for UI binding while maintaining compatibility with the core models.
/// </summary>

/// <summary>
/// Simplified inventory item model for UI binding.
/// Compatible with InventoryViewModel and other UI components.
/// </summary>
public class InventoryItem
{
    public int Id { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; }
    public string User { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    
    public string DisplayText => $"({Operation}) - [{PartId} x {Quantity}]";
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