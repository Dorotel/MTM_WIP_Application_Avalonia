# Custom Prompt: Create Missing Data Models and DTOs

## 🚨 CRITICAL PRIORITY FIX #5

**Issue**: The entire Models namespace is missing, with no strongly-typed data contracts or DTOs for database operations.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- Missing `Models/InventoryItem.cs`
- Missing `Models/InventoryTransaction.cs`
- Missing `Models/User.cs`
- Missing `Models/Result.cs<T>` pattern
- Missing `Models/DTOs/` folder entirely

**Priority**: 🚨 **CRITICAL - TYPE SAFETY FOUNDATION**

---

## Custom Prompt

```
CRITICAL FOUNDATION IMPLEMENTATION: Create comprehensive data models and DTOs to provide strongly-typed data contracts and enforce business rules throughout the application.

REQUIREMENTS:
1. Create core business entity models with proper properties
2. Implement Result<T> pattern for service responses
3. Add validation attributes to enforce business rules
4. Create DTOs for request/response objects
5. Map database table structures to strongly-typed models
6. Follow MTM business logic rules for transaction types and data patterns

CORE BUSINESS ENTITIES TO CREATE:

1. **Models/InventoryItem.cs**:
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class InventoryItem
{
    [Required]
    [StringLength(50)]
    public string PartId { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Operation { get; set; } = string.Empty; // Workflow step number like "90", "100", "110"

    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }

    [Required]
    [StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Description { get; set; }

    public DateTime LastModified { get; set; } = DateTime.Now;

    [Required]
    [StringLength(50)]
    public string LastModifiedBy { get; set; } = string.Empty;

    public DateTime? CreatedDate { get; set; }

    [StringLength(50)]
    public string? CreatedBy { get; set; }

    // Calculated properties
    public bool IsLowStock => Quantity < MinimumStockLevel;
    public int MinimumStockLevel { get; set; } = 10;

    // Navigation properties
    public virtual PartMasterData? PartInfo { get; set; }
    public virtual LocationMasterData? LocationInfo { get; set; }
    public virtual OperationMasterData? OperationInfo { get; set; }

    public override string ToString()
    {
        return $"{PartId} - Op:{Operation} - Qty:{Quantity} @ {Location}";
    }
}
```

2. **Models/InventoryTransaction.cs**:
```csharp
using System;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class InventoryTransaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string PartId { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Operation { get; set; } = string.Empty; // Workflow step identifier, NOT transaction type

    [Required]
    public TransactionType TransactionType { get; set; } // Determined by USER INTENT

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be positive")]
    public int Quantity { get; set; }

    [Required]
    [StringLength(50)]
    public string Location { get; set; } = string.Empty;

    [StringLength(50)]
    public string? ToLocation { get; set; } // For transfers

    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.Now;

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(100)]
    public string? Reference { get; set; } // Work order, PO number, etc.

    // Calculated properties
    public bool IsTransfer => TransactionType == TransactionType.TRANSFER;
    public string TransactionDescription => GetTransactionDescription();

    private string GetTransactionDescription()
    {
        return TransactionType switch
        {
            TransactionType.IN => $"Added {Quantity} units to {Location}",
            TransactionType.OUT => $"Removed {Quantity} units from {Location}",
            TransactionType.TRANSFER => $"Transferred {Quantity} units from {Location} to {ToLocation}",
            _ => $"Unknown transaction type"
        };
    }

    public override string ToString()
    {
        return $"{TransactionType}: {PartId} - {Quantity} units - {Timestamp:yyyy-MM-dd HH:mm}";
    }
}
```

3. **Models/TransactionType.cs**:
```csharp
namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// CRITICAL: TransactionType is determined by USER INTENT, NOT by Operation number.
/// Operations are workflow step identifiers and do not indicate transaction type.
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// User is adding stock to inventory (regardless of operation number)
    /// </summary>
    IN = 1,

    /// <summary>
    /// User is removing stock from inventory (regardless of operation number)
    /// </summary>
    OUT = 2,

    /// <summary>
    /// User is moving stock from one location to another (regardless of operation number)
    /// </summary>
    TRANSFER = 3,

    /// <summary>
    /// Other transaction types (adjustments, corrections, etc.)
    /// </summary>
    OTHER = 4
}
```

4. **Models/User.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class User
{
    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Department { get; set; }

    [StringLength(100)]
    public string? Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLogin { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Calculated properties
    public string FullName => $"{FirstName} {LastName}";
    public string DisplayName => $"{FullName} ({UserId})";

    // Permissions
    public List<string> Permissions { get; set; } = new();

    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
```

5. **Models/Result.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Generic result pattern for service operations with success/failure handling
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T Data { get; private set; } = default!;
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();
    public Exception? Exception { get; private set; }

    private Result(bool isSuccess, T data, string errorMessage, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Exception = exception;
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Errors.Add(errorMessage);
        }
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(true, data, string.Empty);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(false, default!, errorMessage);
    }

    public static Result<T> Failure(string errorMessage, Exception exception)
    {
        return new Result<T>(false, default!, errorMessage, exception);
    }

    public static Result<T> Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        var result = new Result<T>(false, default!, errorList.FirstOrDefault() ?? "Unknown error");
        result.Errors = errorList;
        return result;
    }

    // Implicit conversion from T to Result<T>
    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }

    // Method to transform the result
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure)
            return Result<TNew>.Failure(ErrorMessage);

        try
        {
            var newData = mapper(Data);
            return Result<TNew>.Success(newData);
        }
        catch (Exception ex)
        {
            return Result<TNew>.Failure("Mapping failed", ex);
        }
    }

    public override string ToString()
    {
        if (IsSuccess)
            return $"Success: {Data}";
        
        return $"Failure: {ErrorMessage}";
    }
}

/// <summary>
/// Non-generic result for operations that don't return data
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; private set; } = string.Empty;
    public List<string> Errors { get; private set; } = new();
    public Exception? Exception { get; private set; }

    private Result(bool isSuccess, string errorMessage, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
        
        if (!string.IsNullOrEmpty(errorMessage))
        {
            Errors.Add(errorMessage);
        }
    }

    public static Result Success()
    {
        return new Result(true, string.Empty);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result Failure(string errorMessage, Exception exception)
    {
        return new Result(false, errorMessage, exception);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        var result = new Result(false, errorList.FirstOrDefault() ?? "Unknown error");
        result.Errors = errorList;
        return result;
    }
}
```

MASTER DATA MODELS:

6. **Models/PartMasterData.cs**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class PartMasterData
{
    [Required]
    [StringLength(50)]
    public string PartId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Category { get; set; }

    [StringLength(20)]
    public string? UnitOfMeasure { get; set; }

    public decimal? StandardCost { get; set; }

    public int MinimumStockLevel { get; set; } = 0;

    public int MaximumStockLevel { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public override string ToString()
    {
        return $"{PartId} - {Description}";
    }
}
```

7. **Models/LocationMasterData.cs**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class LocationMasterData
{
    [Required]
    [StringLength(50)]
    public string LocationId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? LocationType { get; set; }

    [StringLength(50)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;

    public override string ToString()
    {
        return $"{LocationId} - {Description}";
    }
}
```

8. **Models/OperationMasterData.cs**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models;

public class OperationMasterData
{
    [Required]
    [StringLength(10)]
    public string OperationId { get; set; } = string.Empty; // "90", "100", "110", etc.

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string? WorkCenter { get; set; }

    public int? SequenceNumber { get; set; }

    public bool IsActive { get; set; } = true;

    // CRITICAL: Operation is a workflow step identifier, NOT a transaction type indicator
    public override string ToString()
    {
        return $"Op {OperationId} - {Description}";
    }
}
```

DTOS FOLDER STRUCTURE:

9. **Models/DTOs/InventoryRequestDto.cs**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models.DTOs;

public class InventoryRequestDto
{
    [Required]
    public string PartId { get; set; } = string.Empty;

    [Required]
    public string Operation { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string? Notes { get; set; }
    public string? Reference { get; set; }
}
```

10. **Models/DTOs/TransferRequestDto.cs**:
```csharp
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.Models.DTOs;

public class TransferRequestDto : InventoryRequestDto
{
    [Required]
    public string ToLocation { get; set; } = string.Empty;
}
```

11. **Models/DTOs/InventoryResponseDto.cs**:
```csharp
using System;

namespace MTM_WIP_Application_Avalonia.Models.DTOs;

public class InventoryResponseDto
{
    public string PartId { get; set; } = string.Empty;
    public string PartDescription { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string OperationDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string LocationDescription { get; set; } = string.Empty;
    public DateTime LastModified { get; set; }
    public string LastModifiedBy { get; set; } = string.Empty;
    public bool IsLowStock { get; set; }
}
```

VALIDATION ATTRIBUTES:

12. **Models/Validation/ValidOperationAttribute.cs**:
```csharp
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.Models.Validation;

public class ValidOperationAttribute : ValidationAttribute
{
    private static readonly string[] ValidOperations = { "90", "100", "110", "120", "130" };

    public override bool IsValid(object? value)
    {
        if (value is string operation)
        {
            return ValidOperations.Contains(operation);
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid operation number: {string.Join(", ", ValidOperations)}";
    }
}
```

13. **Models/Validation/ValidPartIdAttribute.cs**:
```csharp
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MTM_WIP_Application_Avalonia.Models.Validation;

public class ValidPartIdAttribute : ValidationAttribute
{
    private static readonly Regex PartIdPattern = new(@"^[A-Z0-9_-]{1,50}$", RegexOptions.Compiled);

    public override bool IsValid(object? value)
    {
        if (value is string partId)
        {
            return PartIdPattern.IsMatch(partId);
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid part ID (alphanumeric, underscore, hyphen only, max 50 characters)";
    }
}
```

BUSINESS RULE ENFORCEMENT:
- Use validation attributes to enforce data integrity
- Implement custom validation for MTM-specific rules
- Ensure TransactionType is never determined by Operation
- Validate all required fields have appropriate constraints
- Implement ToString() methods for debugging and display

FOLDER STRUCTURE:
```
Models/
├── InventoryItem.cs
├── InventoryTransaction.cs
├── TransactionType.cs
├── User.cs
├── Result.cs
├── PartMasterData.cs
├── LocationMasterData.cs
├── OperationMasterData.cs
├── DTOs/
│   ├── InventoryRequestDto.cs
│   ├── TransferRequestDto.cs
│   └── InventoryResponseDto.cs
└── Validation/
    ├── ValidOperationAttribute.cs
    └── ValidPartIdAttribute.cs
```

After creating all models, create Models/README_DataModels.md documenting:
- Model purposes and relationships
- Validation rules and business logic
- DTO usage patterns
- Result pattern implementation
- MTM-specific data rules
```

---

## Expected Deliverables

1. **Models/InventoryItem.cs** - Core inventory entity with validation
2. **Models/InventoryTransaction.cs** - Transaction logging with correct TransactionType logic
3. **Models/TransactionType.cs** - Enum with clear documentation about user intent
4. **Models/User.cs** - User entity with permissions
5. **Models/Result.cs** - Generic result pattern for service responses
6. **Models/PartMasterData.cs** - Master data for parts
7. **Models/LocationMasterData.cs** - Master data for locations
8. **Models/OperationMasterData.cs** - Master data for operations
9. **Models/DTOs/** folder with request/response objects
10. **Models/Validation/** folder with custom validation attributes
11. **Models/README_DataModels.md** - Comprehensive documentation

---

## Validation Steps

1. Verify all models compile without errors
2. Test validation attributes with valid and invalid data
3. Confirm Result<T> pattern works correctly
4. Validate TransactionType logic follows MTM business rules
5. Test ToString() methods provide useful output
6. Verify all required properties are properly annotated
7. Confirm DTOs map correctly to business entities

---

## Success Criteria

- [ ] All core business entities created with proper validation
- [ ] Result<T> pattern implemented for consistent error handling
- [ ] TransactionType logic follows MTM rules (user intent, not operation)
- [ ] Validation attributes enforce business rules
- [ ] DTOs provide clean request/response contracts
- [ ] Master data models support lookup operations
- [ ] Custom validation attributes work correctly
- [ ] All models have useful ToString() implementations
- [ ] Documentation explains model relationships and rules
- [ ] Ready for service layer integration

---

*Priority: CRITICAL - Foundation for type safety and business rule enforcement throughout application.*