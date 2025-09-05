using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models.Database;

/// <summary>
/// Parameter object for inventory add operations to reduce method complexity
/// </summary>
public class AddInventoryRequest
{
    [Required]
    public string PartId { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public string Operation { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    public string ItemType { get; set; } = string.Empty;
    
    [Required]
    public string User { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Parameter object for inventory removal operations to reduce method complexity
/// </summary>
public class RemoveInventoryRequest
{
    [Required]
    public string PartId { get; set; } = string.Empty;
    
    [Required]
    public string Location { get; set; } = string.Empty;
    
    [Required]
    public string Operation { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Required]
    public string ItemType { get; set; } = string.Empty;
    
    [Required]
    public string User { get; set; } = string.Empty;
    
    public string BatchNumber { get; set; } = string.Empty;
    
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// Parameter object for inventory transfer operations to reduce method complexity
/// </summary>
public class TransferQuantityRequest
{
    [Required]
    public string BatchNumber { get; set; } = string.Empty;
    
    [Required]
    public string PartId { get; set; } = string.Empty;
    
    [Required]
    public string Operation { get; set; } = string.Empty;
    
    [Range(1, int.MaxValue)]
    public int TransferQuantity { get; set; }
    
    [Range(1, int.MaxValue)]
    public int OriginalQuantity { get; set; }
    
    [Required]
    public string NewLocation { get; set; } = string.Empty;
    
    [Required]
    public string User { get; set; } = string.Empty;
}

/// <summary>
/// Parameter object for part update operations to reduce method complexity
/// </summary>
public class UpdatePartRequest
{
    public int Id { get; set; }
    
    [Required]
    public string PartId { get; set; } = string.Empty;
    
    [Required]
    public string Customer { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string IssuedBy { get; set; } = string.Empty;
    
    [Required]
    public string ItemType { get; set; } = string.Empty;
}

/// <summary>
/// Parameter object for user add operations to reduce method complexity
/// </summary>
public class AddUserRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = string.Empty;
    
    [Required]
    public string IssuedBy { get; set; } = string.Empty;
}

/// <summary>
/// Parameter object for user update operations to reduce method complexity
/// </summary>
public class UpdateUserRequest
{
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    public string LastName { get; set; } = string.Empty;
    
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    [Required]
    public string IssuedBy { get; set; } = string.Empty;
}