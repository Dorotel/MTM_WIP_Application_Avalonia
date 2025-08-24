# Models Directory

This directory contains all data models, DTOs (Data Transfer Objects), and business entities for the MTM WIP Application Avalonia (.NET 8).

## ??? Model Architecture (.NET 8)

### Model Categories (.NET 8 Enhanced)

#### Core Models (`CoreModels.cs`)
Essential data structures leveraging .NET 8 modern features:
- **Result Pattern**: Standardized result handling with generic constraints and nullable references
- **Inventory Models**: Core inventory and transaction data with record types
- **User Models**: User authentication with strong typing and null safety
- **Application State**: Global state management with immutable patterns

#### Business Entities (.NET 8)
Domain-specific models using modern C# patterns:
- **Inventory Items**: Part-based inventory with location tracking and nullable references
- **Transactions**: Comprehensive audit trail with record types for immutability
- **Users**: User accounts with role-based permissions and strong typing
- **Locations**: Physical storage with hierarchical organization

#### Utility Models (.NET 8)
Support models with enhanced functionality:
- **Configuration**: Application settings with options pattern validation
- **Error Handling**: Structured error information with source generators
- **UI State**: View-specific state with reactive patterns

## ?? Model Files (.NET 8)

### Core Models (`CoreModels.cs`)

#### Result Pattern (.NET 8 Enhanced)
Standardized result handling with modern C# features:

```csharp
namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
public record Result
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result Failure(IEnumerable<string> errors) => new() 
    { 
        IsSuccess = false, 
        Errors = errors.ToList().AsReadOnly() 
    };
}

/// <summary>
/// Generic result type that can return data on success
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public record Result<T> : Result
{
    public T? Data { get; init; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static new Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static new Result<T> Failure(IEnumerable<string> errors) => new() 
    { 
        IsSuccess = false, 
        Errors = errors.ToList().AsReadOnly() 
    };
}
```

#### Inventory Models (.NET 8 with Record Types)
Core inventory management entities using modern patterns:

```csharp
/// <summary>
/// Represents an inventory item in the system
/// </summary>
public record InventoryItem
{
    public int Id { get; init; }
    public required string PartId { get; init; }
    public required string Operation { get; init; } // Workflow step identifier
    public required string Location { get; init; }
    public int Quantity { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedDate { get; init; } = DateTime.UtcNow;
    public required string CreatedBy { get; init; }
    public required string ModifiedBy { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public decimal UnitCost { get; init; }
    
    // Factory methods for common scenarios
    public static InventoryItem Create(string partId, string operation, string location, 
        int quantity, string user) => new()
    {
        PartId = partId,
        Operation = operation,
        Location = location,
        Quantity = quantity,
        CreatedBy = user,
        ModifiedBy = user
    };
}

/// <summary>
/// Represents a transaction record for audit trail
/// </summary>
public record InventoryTransaction
{
    public int Id { get; init; }
    public required string PartId { get; init; }
    public required string Operation { get; init; } // Workflow step number only
    public TransactionType TransactionType { get; init; } // Based on user intent
    public string FromLocation { get; init; } = string.Empty;
    public string ToLocation { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public string ItemType { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; } = DateTime.UtcNow;
    public required string User { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public string ReferenceNumber { get; init; } = string.Empty;
    
    // Helper property for UI display
    public string LocationDisplay => TransactionType switch
    {
        TransactionType.IN => ToLocation,
        TransactionType.OUT => FromLocation,
        TransactionType.TRANSFER => $"{FromLocation} ? {ToLocation}",
        _ => "Unknown"
    };
}

/// <summary>
/// Transaction types based on user intent (NOT operation numbers)
/// </summary>
public enum TransactionType
{
    IN,       // User is adding stock to inventory
    OUT,      // User is removing stock from inventory
    TRANSFER  // User is moving stock between locations
}
```

#### User Models (.NET 8 with Strong Typing)
User management with enhanced type safety:

```csharp
/// <summary>
/// Represents a user in the system
/// </summary>
public record User
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public required string FullName { get; init; }
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; } = UserRole.ReadOnly;
    public bool IsActive { get; init; } = true;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; init; }
    public UserPreferences Preferences { get; init; } = new();
    
    // Computed properties
    public bool CanEditInventory => Role >= UserRole.Standard;
    public bool CanManageUsers => Role >= UserRole.Supervisor;
    public bool IsAdministrator => Role == UserRole.Administrator;
}

/// <summary>
/// User roles with different permission levels
/// </summary>
public enum UserRole
{
    ReadOnly = 0,     // Can view data only
    Standard = 1,     // Can add/edit inventory
    Supervisor = 2,   // Can manage users and advanced operations
    Administrator = 3 // Full system access
}

/// <summary>
/// User-specific preferences and settings
/// </summary>
public record UserPreferences
{
    public string Theme { get; init; } = "Default";
    public string DefaultLocation { get; init; } = string.Empty;
    public string DefaultOperation { get; init; } = string.Empty;
    public bool EnableQuickButtons { get; init; } = true;
    public int QuickButtonCount { get; init; } = 10;
    public IReadOnlyDictionary<string, object> CustomSettings { get; init; } = 
        new Dictionary<string, object>().AsReadOnly();
}
```

### Business Entity Models (.NET 8)

#### Location Models with Hierarchy
```csharp
/// <summary>
/// Represents a storage location with hierarchical structure
/// </summary>
public record StorageLocation
{
    public int Id { get; init; }
    public required string LocationCode { get; init; }
    public required string LocationName { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public string Zone { get; init; } = string.Empty;
    public string Building { get; init; } = string.Empty;
    public int Capacity { get; init; } = 0;
    public LocationType Type { get; init; } = LocationType.Warehouse;
    public int? ParentLocationId { get; init; }
    
    // Computed properties
    public string FullPath => !string.IsNullOrEmpty(Building) && !string.IsNullOrEmpty(Zone)
        ? $"{Building}/{Zone}/{LocationName}"
        : LocationName;
}

public enum LocationType
{
    Warehouse,
    ProductionFloor,
    QualityControl,
    Shipping,
    Receiving,
    Quarantine
}
```

#### Part Models (.NET 8 Enhanced)
```csharp
/// <summary>
/// Represents a part definition with specifications
/// </summary>
public record Part
{
    public int Id { get; init; }
    public required string PartId { get; init; }
    public required string PartName { get; init; }
    public string Description { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public required string CreatedBy { get; init; }
    public PartSpecifications Specifications { get; init; } = new();
    
    // Factory method for common creation
    public static Part Create(string partId, string partName, string createdBy) => new()
    {
        PartId = partId,
        PartName = partName,
        CreatedBy = createdBy
    };
}

/// <summary>
/// Part specifications and technical details
/// </summary>
public record PartSpecifications
{
    public string Material { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public string Dimensions { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public string Supplier { get; init; } = string.Empty;
    public decimal StandardCost { get; init; }
    public IReadOnlyDictionary<string, object> CustomProperties { get; init; } = 
        new Dictionary<string, object>().AsReadOnly();
}
```

#### Operation Models (.NET 8)
```csharp
/// <summary>
/// Represents a workflow operation with modern patterns
/// </summary>
public record Operation
{
    public int Id { get; init; }
    public required string OperationCode { get; init; } // e.g., "90", "100", "110"
    public required string OperationName { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Sequence { get; init; } = 0;
    public bool IsActive { get; init; } = true;
    public WorkflowType WorkflowType { get; init; } = WorkflowType.Manufacturing;
    public OperationSettings Settings { get; init; } = new();
    
    // Helper for validation
    public static bool IsValidOperationCode(string? code) =>
        !string.IsNullOrWhiteSpace(code) && 
        code.All(char.IsDigit) && 
        code.Length is > 0 and <= 10;
}

public enum WorkflowType
{
    Manufacturing,
    Assembly,
    QualityControl,
    Packaging,
    Shipping,
    Custom
}

/// <summary>
/// Operation-specific settings
/// </summary>
public record OperationSettings
{
    public bool RequiresQualityCheck { get; init; }
    public bool AllowsPartialQuantities { get; init; } = true;
    public int StandardProcessingTime { get; init; } // minutes
    public string DefaultLocation { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, object> CustomSettings { get; init; } = 
        new Dictionary<string, object>().AsReadOnly();
}
```

### Configuration and Error Models (.NET 8)

#### Application Configuration
```csharp
/// <summary>
/// Application configuration using .NET 8 options pattern
/// </summary>
public record AppConfiguration
{
    public required string ConnectionString { get; init; }
    public DatabaseSettings Database { get; init; } = new();
    public UISettings UI { get; init; } = new();
    public LoggingSettings Logging { get; init; } = new();
    public SecuritySettings Security { get; init; } = new();
}

public record DatabaseSettings
{
    public int CommandTimeout { get; init; } = 30;
    public int ConnectionPoolSize { get; init; } = 100;
    public bool EnableRetry { get; init; } = true;
    public int MaxRetryAttempts { get; init; } = 3;
}

public record UISettings
{
    public string DefaultTheme { get; init; } = "MTM_Purple";
    public bool EnableAnimations { get; init; } = true;
    public int AutoSaveInterval { get; init; } = 300; // seconds
    public bool ShowDebugInfo { get; init; } = false;
}
```

#### Error Models (.NET 8 Enhanced)
```csharp
/// <summary>
/// Represents an error with structured information
/// </summary>
public record ErrorInfo
{
    public int Id { get; init; }
    public required string ErrorCode { get; init; }
    public required string ErrorMessage { get; init; }
    public string StackTrace { get; init; } = string.Empty;
    public ErrorSeverity Severity { get; init; } = ErrorSeverity.Error;
    public required string Source { get; init; }
    public string User { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public IReadOnlyDictionary<string, object> Context { get; init; } = 
        new Dictionary<string, object>().AsReadOnly();
    
    // Factory methods for common scenarios
    public static ErrorInfo CreateValidationError(string message, string source) => new()
    {
        ErrorCode = "VALIDATION_ERROR",
        ErrorMessage = message,
        Source = source,
        Severity = ErrorSeverity.Warning
    };
    
    public static ErrorInfo CreateDatabaseError(Exception ex, string source) => new()
    {
        ErrorCode = "DATABASE_ERROR",
        ErrorMessage = ex.Message,
        StackTrace = ex.StackTrace ?? string.Empty,
        Source = source,
        Severity = ErrorSeverity.Error
    };
}

public enum ErrorSeverity
{
    Information,
    Warning,
    Error,
    Critical
}
```

## ?? Model Validation (.NET 8)

### Data Annotations with Enhanced Attributes
```csharp
public record ValidatedInventoryItem
{
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(300, ErrorMessage = "Part ID cannot exceed 300 characters")]
    [RegularExpression(@"^[A-Z0-9\-_]+$", ErrorMessage = "Part ID can only contain uppercase letters, numbers, hyphens, and underscores")]
    public required string PartId { get; init; }

    [Required(ErrorMessage = "Operation is required")]
    [StringLength(100, ErrorMessage = "Operation cannot exceed 100 characters")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Operation must be numeric")]
    public required string Operation { get; init; }

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public required string Location { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public int Quantity { get; init; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string Notes { get; init; } = string.Empty;
}
```

### Business Rule Validation (.NET 8)
```csharp
/// <summary>
/// Validates business rules for inventory operations using modern patterns
/// </summary>
public static class InventoryValidator
{
    public static ValidationResult ValidateInventoryItem(InventoryItem item)
    {
        var errors = new List<string>();

        // Use pattern matching for validation
        var validationChecks = new[]
        {
            (string.IsNullOrWhiteSpace(item.PartId), "Part ID is required"),
            (string.IsNullOrWhiteSpace(item.Operation), "Operation is required"),
            (string.IsNullOrWhiteSpace(item.Location), "Location is required"),
            (item.Quantity <= 0, "Quantity must be greater than zero"),
            (!Operation.IsValidOperationCode(item.Operation), "Operation must be numeric")
        };

        errors.AddRange(validationChecks
            .Where(check => check.Item1)
            .Select(check => check.Item2));

        return errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errors);
    }

    public static ValidationResult ValidateTransferOperation(string fromLocation, string toLocation, int quantity)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(fromLocation))
            errors.Add("Source location is required");

        if (string.IsNullOrWhiteSpace(toLocation))
            errors.Add("Destination location is required");

        if (fromLocation.Equals(toLocation, StringComparison.OrdinalIgnoreCase))
            errors.Add("Source and destination locations must be different");

        if (quantity <= 0)
            errors.Add("Transfer quantity must be greater than zero");

        return errors.Count == 0 
            ? ValidationResult.Success() 
            : ValidationResult.Failure(errors);
    }
}

public record ValidationResult
{
    public bool IsValid { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    
    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Failure(IEnumerable<string> errors) => new() 
    { 
        IsValid = false, 
        Errors = errors.ToList().AsReadOnly() 
    };
}
```

## ?? Business Logic Integration (.NET 8)

### Transaction Type Helper (.NET 8)
```csharp
/// <summary>
/// Helper for transaction type determination using modern C# patterns
/// </summary>
public static class TransactionTypeHelper
{
    /// <summary>
    /// Determines transaction type based on user action, NOT operation number
    /// </summary>
    public static TransactionType GetTransactionTypeForAction(UserAction action) => action switch
    {
        UserAction.AddInventory => TransactionType.IN,      // User adding stock
        UserAction.RemoveInventory => TransactionType.OUT,  // User removing stock
        UserAction.TransferInventory => TransactionType.TRANSFER, // User moving stock
        _ => throw new ArgumentException($"Unknown user action: {action}")
    };

    /// <summary>
    /// Creates a transaction record with proper business logic
    /// </summary>
    public static InventoryTransaction CreateTransaction(
        string partId, 
        string operation, 
        UserAction action, 
        int quantity, 
        string user,
        string? fromLocation = null,
        string? toLocation = null) => new()
    {
        PartId = partId,
        Operation = operation,
        TransactionType = GetTransactionTypeForAction(action),
        FromLocation = fromLocation ?? string.Empty,
        ToLocation = toLocation ?? string.Empty,
        Quantity = quantity,
        User = user
    };
}

public enum UserAction
{
    AddInventory,
    RemoveInventory,
    TransferInventory,
    ViewInventory,
    EditInventory
}
```

### Audit Trail Models (.NET 8)
```csharp
/// <summary>
/// Comprehensive audit log entry using modern patterns
/// </summary>
public record AuditLogEntry
{
    public int Id { get; init; }
    public required string TableName { get; init; }
    public required string Operation { get; init; } // INSERT, UPDATE, DELETE
    public required string PrimaryKeyValue { get; init; }
    public string? OldValues { get; init; } // JSON
    public string? NewValues { get; init; } // JSON
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public required string User { get; init; }
    public string Source { get; init; } = "Application";
    
    // Factory method for creation
    public static AuditLogEntry Create(string tableName, string operation, 
        string primaryKey, string user) => new()
    {
        TableName = tableName,
        Operation = operation,
        PrimaryKeyValue = primaryKey,
        User = user
    };
}

/// <summary>
/// User activity tracking with performance metrics
/// </summary>
public record UserActivity
{
    public int Id { get; init; }
    public required string User { get; init; }
    public required string Action { get; init; }
    public required string Entity { get; init; }
    public string EntityId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public TimeSpan Duration { get; init; }
    public string Details { get; init; } = string.Empty;
    public bool Success { get; init; } = true;
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = 
        new Dictionary<string, object>().AsReadOnly();
}
```

## ?? Query Models (.NET 8)

### Search Criteria with Modern Patterns
```csharp
/// <summary>
/// Advanced inventory search with builder pattern
/// </summary>
public record InventorySearchCriteria
{
    public string? PartId { get; init; }
    public string? PartNameContains { get; init; }
    public string? Operation { get; init; }
    public string? Location { get; init; }
    public string? ItemType { get; init; }
    public int? MinQuantity { get; init; }
    public int? MaxQuantity { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
    public string? CreatedBy { get; init; }
    public string? NotesContains { get; init; }
    public bool? IsActive { get; init; }
    
    // Sorting and pagination
    public string SortBy { get; init; } = "CreatedDate";
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
    public int PageSize { get; init; } = 50;
    public int PageNumber { get; init; } = 1;
    
    // Builder pattern for fluent interface
    public static InventorySearchCriteriaBuilder Builder() => new();
}

public class InventorySearchCriteriaBuilder
{
    private InventorySearchCriteria _criteria = new();
    
    public InventorySearchCriteriaBuilder WithPartId(string partId) => 
        this with { _criteria = _criteria with { PartId = partId } };
    
    public InventorySearchCriteriaBuilder WithLocation(string location) => 
        this with { _criteria = _criteria with { Location = location } };
    
    public InventorySearchCriteriaBuilder WithQuantityRange(int min, int max) => 
        this with { _criteria = _criteria with { MinQuantity = min, MaxQuantity = max } };
    
    public InventorySearchCriteria Build() => _criteria;
}

public enum SortDirection
{
    Ascending,
    Descending
}
```

## ?? Testing Support (.NET 8)

### Test Data Builders
```csharp
/// <summary>
/// Test data builder using modern C# patterns
/// </summary>
public class InventoryItemBuilder
{
    private InventoryItem _item = InventoryItem.Create("TEST-PART", "100", "TEST-LOCATION", 1, "TestUser");

    public InventoryItemBuilder WithPartId(string partId) => 
        this with { _item = _item with { PartId = partId } };

    public InventoryItemBuilder WithOperation(string operation) => 
        this with { _item = _item with { Operation = operation } };

    public InventoryItemBuilder WithLocation(string location) => 
        this with { _item = _item with { Location = location } };

    public InventoryItemBuilder WithQuantity(int quantity) => 
        this with { _item = _item with { Quantity = quantity } };

    public InventoryItem Build() => _item;

    public static InventoryItemBuilder Create() => new();
    
    // Common test scenarios
    public static InventoryItem ValidItem() => Create().Build();
    public static InventoryItem ItemWithLargeQuantity() => Create().WithQuantity(1000).Build();
    public static InventoryItem ItemForTransfer() => Create().WithLocation("WAREHOUSE-A").Build();
}
```

## ?? Development Guidelines (.NET 8)

### Adding New Models
1. **Use record types**: For immutable data transfer objects and value objects
2. **Required properties**: Use `required` keyword for essential properties
3. **Nullable references**: Properly handle nullable reference types
4. **Factory methods**: Provide static factory methods for common creation scenarios
5. **Validation**: Include appropriate data annotations and business rule validation
6. **Documentation**: Add comprehensive XML comments

### Model Conventions (.NET 8)
- **Naming**: Use descriptive names with proper C# conventions
- **Immutability**: Prefer record types for immutable data structures
- **Null safety**: Use nullable reference types appropriately
- **Performance**: Consider readonly collections for better performance
- **Validation**: Implement both attribute-based and business rule validation

## ?? Related Documentation

- **Services**: See `Services/README.md` for service integration patterns with .NET 8
- **Database Schema**: `Documentation/Development/Database_Files/README_NewProcedures.md`
- **GitHub Instructions**: `.github/copilot-instructions.md` for comprehensive development guidelines
- **ViewModels**: `ViewModels/README.md` for ReactiveUI integration patterns

---

*This directory contains the data layer using .NET 8 modern patterns, defining all business entities, DTOs, and data structures with enhanced type safety, immutability, and performance optimizations for the MTM WIP Application.*