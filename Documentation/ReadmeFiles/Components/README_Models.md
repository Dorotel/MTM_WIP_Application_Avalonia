# Models Directory

This directory contains all data models, DTOs (Data Transfer Objects), and business entities for the MTM WIP Application Avalonia.

## ??? Model Architecture

### Model Categories

#### Core Models (`CoreModels.cs`)
Essential data structures for the application's core functionality:
- **Result Pattern**: Standardized result handling with success/failure states
- **Inventory Models**: Core inventory and transaction data structures
- **User Models**: User authentication and authorization entities
- **Application State**: Global application state management

#### Business Entities
Domain-specific models representing business concepts:
- **Inventory Items**: Part-based inventory with location and quantity tracking
- **Transactions**: Audit trail for all inventory operations
- **Users**: User accounts with role-based permissions
- **Locations**: Physical storage locations and their properties

#### Utility Models
Support models for enhanced functionality:
- **Configuration**: Application settings and user preferences
- **Error Handling**: Structured error information and logging
- **UI State**: View-specific state management and persistence

## ?? Model Files

### Core Models (`CoreModels.cs`)

#### Result Pattern
Standardized result handling for all operations:

```csharp
/// <summary>
/// Represents the result of an operation that can succeed or fail
/// </summary>
public class Result
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();
    
    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
    public static Result Failure(List<string> errors) => new() { IsSuccess = false, Errors = errors };
}

/// <summary>
/// Generic result type that can return data on success
/// </summary>
public class Result<T> : Result
{
    public T? Data { get; set; }
    
    public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static new Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}
```

#### Inventory Models
Core inventory management entities:

```csharp
/// <summary>
/// Represents an inventory item in the system
/// </summary>
public class InventoryItem
{
    public int Id { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // Workflow step identifier
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
}

/// <summary>
/// Represents a transaction record for audit trail
/// </summary>
public class InventoryTransaction
{
    public int Id { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // Just a workflow step number
    public TransactionType TransactionType { get; set; } // Based on user intent
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string User { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
}

/// <summary>
/// Transaction types based on user intent
/// </summary>
public enum TransactionType
{
    IN,       // User is adding stock to inventory
    OUT,      // User is removing stock from inventory
    TRANSFER  // User is moving stock between locations
}
```

#### User Models
User management and authentication:

```csharp
/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public UserPreferences Preferences { get; set; } = new();
}

/// <summary>
/// User roles with different permission levels
/// </summary>
public enum UserRole
{
    ReadOnly,     // Can view data only
    Standard,     // Can add/edit inventory
    Supervisor,   // Can manage users and advanced operations
    Administrator // Full system access
}

/// <summary>
/// User-specific preferences and settings
/// </summary>
public class UserPreferences
{
    public string Theme { get; set; } = "Default";
    public string DefaultLocation { get; set; } = string.Empty;
    public string DefaultOperation { get; set; } = string.Empty;
    public bool EnableQuickButtons { get; set; } = true;
    public int QuickButtonCount { get; set; } = 10;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}
```

### Business Entity Models

#### Location Models
Physical storage location management:

```csharp
/// <summary>
/// Represents a storage location
/// </summary>
public class StorageLocation
{
    public int Id { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Zone { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public LocationType Type { get; set; }
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

#### Part Models
Part management and specifications:

```csharp
/// <summary>
/// Represents a part definition
/// </summary>
public class Part
{
    public int Id { get; set; }
    public string PartId { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public PartSpecifications Specifications { get; set; } = new();
}

/// <summary>
/// Part specifications and technical details
/// </summary>
public class PartSpecifications
{
    public string Material { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public string Dimensions { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public decimal StandardCost { get; set; }
    public Dictionary<string, object> CustomProperties { get; set; } = new();
}
```

#### Operation Models
Workflow and operation management:

```csharp
/// <summary>
/// Represents a workflow operation
/// </summary>
public class Operation
{
    public int Id { get; set; }
    public string OperationCode { get; set; } = string.Empty; // e.g., "90", "100", "110"
    public string OperationName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public bool IsActive { get; set; }
    public WorkflowType WorkflowType { get; set; }
    public OperationSettings Settings { get; set; } = new();
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
/// Operation-specific settings and configuration
/// </summary>
public class OperationSettings
{
    public bool RequiresQualityCheck { get; set; }
    public bool AllowsPartialQuantities { get; set; }
    public int StandardProcessingTime { get; set; } // minutes
    public string DefaultLocation { get; set; } = string.Empty;
    public Dictionary<string, object> CustomSettings { get; set; } = new();
}
```

### Utility and Support Models

#### Configuration Models
Application configuration and settings:

```csharp
/// <summary>
/// Application configuration settings
/// </summary>
public class AppConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public DatabaseSettings Database { get; set; } = new();
    public UISettings UI { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
}

public class DatabaseSettings
{
    public int CommandTimeout { get; set; } = 30;
    public int ConnectionPoolSize { get; set; } = 100;
    public bool EnableRetry { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
}

public class UISettings
{
    public string DefaultTheme { get; set; } = "MTM_Purple";
    public bool EnableAnimations { get; set; } = true;
    public int AutoSaveInterval { get; set; } = 300; // seconds
    public bool ShowDebugInfo { get; set; } = false;
}
```

#### Error Models
Structured error handling and logging:

```csharp
/// <summary>
/// Represents an error that occurred in the system
/// </summary>
public class ErrorInfo
{
    public int Id { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public ErrorSeverity Severity { get; set; }
    public string Source { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public enum ErrorSeverity
{
    Information,
    Warning,
    Error,
    Critical
}

/// <summary>
/// Validation error details
/// </summary>
public class ValidationError
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public object? AttemptedValue { get; set; }
    public ErrorCode ErrorCode { get; set; }
}

public enum ErrorCode
{
    Required,
    InvalidFormat,
    OutOfRange,
    Duplicate,
    NotFound,
    BusinessRuleViolation
}
```

### DTO (Data Transfer Objects)

#### API DTOs
Data transfer objects for API communication:

```csharp
/// <summary>
/// DTO for inventory search requests
/// </summary>
public class InventorySearchRequest
{
    public string? PartId { get; set; }
    public string? Operation { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}

/// <summary>
/// DTO for inventory search results
/// </summary>
public class InventorySearchResponse
{
    public List<InventoryItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => PageNumber * PageSize < TotalCount;
}

/// <summary>
/// DTO for creating new inventory items
/// </summary>
public class CreateInventoryItemRequest
{
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // Workflow step identifier
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public string User { get; set; } = string.Empty;
    // TransactionType determined by service based on user intent (always IN for creation)
}
```

## ?? Model Validation

### Data Annotations
Models use data annotations for validation:

```csharp
public class InventoryItem
{
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(300, ErrorMessage = "Part ID cannot exceed 300 characters")]
    public string PartId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Operation is required")]
    [StringLength(100, ErrorMessage = "Operation cannot exceed 100 characters")]
    public string Operation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string Location { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
    public int Quantity { get; set; }

    [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
    public string Notes { get; set; } = string.Empty;
}
```

### Custom Validation
Business rule validation:

```csharp
/// <summary>
/// Validates business rules for inventory operations
/// </summary>
public static class InventoryValidator
{
    public static Result ValidateInventoryItem(InventoryItem item)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(item.PartId))
            errors.Add("Part ID is required");

        if (string.IsNullOrWhiteSpace(item.Operation))
            errors.Add("Operation is required");

        if (string.IsNullOrWhiteSpace(item.Location))
            errors.Add("Location is required");

        if (item.Quantity <= 0)
            errors.Add("Quantity must be greater than zero");

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }

    public static Result ValidateTransferOperation(string fromLocation, string toLocation, int quantity)
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

        return errors.Any() ? Result.Failure(errors) : Result.Success();
    }
}
```

## ?? Business Logic Integration

### Transaction Type Logic
Models implement correct transaction type handling:

```csharp
/// <summary>
/// Helper class for transaction type determination
/// </summary>
public static class TransactionTypeHelper
{
    /// <summary>
    /// Determines transaction type based on user action, NOT operation number
    /// </summary>
    public static TransactionType GetTransactionTypeForAction(UserAction action)
    {
        return action switch
        {
            UserAction.AddInventory => TransactionType.IN,      // User adding stock
            UserAction.RemoveInventory => TransactionType.OUT,  // User removing stock
            UserAction.TransferInventory => TransactionType.TRANSFER, // User moving stock
            _ => throw new ArgumentException($"Unknown user action: {action}")
        };
    }

    /// <summary>
    /// Validates that operation is used correctly (as workflow identifier, not transaction type)
    /// </summary>
    public static bool IsValidOperation(string operation)
    {
        // Operations are workflow step identifiers like "90", "100", "110"
        return !string.IsNullOrWhiteSpace(operation) && 
               operation.All(char.IsDigit) && 
               operation.Length <= 10;
    }
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

### Audit Trail Models
Complete audit trail for all operations:

```csharp
/// <summary>
/// Comprehensive audit log entry
/// </summary>
public class AuditLogEntry
{
    public int Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty; // INSERT, UPDATE, DELETE
    public string PrimaryKeyValue { get; set; } = string.Empty;
    public string OldValues { get; set; } = string.Empty; // JSON
    public string NewValues { get; set; } = string.Empty; // JSON
    public DateTime Timestamp { get; set; }
    public string User { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty; // Application, API, etc.
}

/// <summary>
/// User activity tracking
/// </summary>
public class UserActivity
{
    public int Id { get; set; }
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public TimeSpan Duration { get; set; }
    public string Details { get; set; } = string.Empty;
    public bool Success { get; set; }
}
```

## ?? Query Models

### Search and Filter Models
Comprehensive search and filtering capabilities:

```csharp
/// <summary>
/// Advanced inventory search criteria
/// </summary>
public class InventorySearchCriteria
{
    public string? PartId { get; set; }
    public string? PartNameContains { get; set; }
    public string? Operation { get; set; }
    public string? Location { get; set; }
    public string? ItemType { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public string? CreatedBy { get; set; }
    public string? NotesContains { get; set; }
    public bool? IsActive { get; set; }
    
    // Sorting and pagination
    public string SortBy { get; set; } = "CreatedDate";
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
    public int PageSize { get; set; } = 50;
    public int PageNumber { get; set; } = 1;
}

public enum SortDirection
{
    Ascending,
    Descending
}

/// <summary>
/// Transaction history search criteria
/// </summary>
public class TransactionSearchCriteria
{
    public string? PartId { get; set; }
    public string? Operation { get; set; }
    public TransactionType? TransactionType { get; set; }
    public string? User { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Location { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    
    // Sorting and pagination
    public string SortBy { get; set; } = "TransactionDate";
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;
    public int PageSize { get; set; } = 100;
    public int PageNumber { get; set; } = 1;
}
```

## ?? Performance Models

### Caching Models
Efficient data caching and performance optimization:

```csharp
/// <summary>
/// Cache entry for frequently accessed data
/// </summary>
public class CacheEntry<T>
{
    public T Data { get; set; } = default!;
    public DateTime CachedAt { get; set; }
    public TimeSpan ExpirationTime { get; set; }
    public bool IsExpired => DateTime.Now > CachedAt.Add(ExpirationTime);
    public string Key { get; set; } = string.Empty;
}

/// <summary>
/// Performance metrics for operations
/// </summary>
public class PerformanceMetrics
{
    public string OperationName { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime Timestamp { get; set; }
    public string User { get; set; } = string.Empty;
    public int RecordsProcessed { get; set; }
    public long MemoryUsed { get; set; }
    public Dictionary<string, object> AdditionalMetrics { get; set; } = new();
}
```

## ?? Testing Models

### Test Data Models
Models for unit and integration testing:

```csharp
/// <summary>
/// Test data builder for inventory items
/// </summary>
public class InventoryItemBuilder
{
    private InventoryItem _item = new();

    public InventoryItemBuilder WithPartId(string partId)
    {
        _item.PartId = partId;
        return this;
    }

    public InventoryItemBuilder WithOperation(string operation)
    {
        _item.Operation = operation;
        return this;
    }

    public InventoryItemBuilder WithLocation(string location)
    {
        _item.Location = location;
        return this;
    }

    public InventoryItemBuilder WithQuantity(int quantity)
    {
        _item.Quantity = quantity;
        return this;
    }

    public InventoryItem Build() => _item;

    public static InventoryItemBuilder Create() => new();
}
```

## ?? Development Guidelines

### Adding New Models
1. **Define Purpose**: Clearly define what the model represents
2. **Add Validation**: Include appropriate data annotations and custom validation
3. **Include Documentation**: Add comprehensive XML comments
4. **Consider Relationships**: Define relationships to other models
5. **Add Unit Tests**: Create tests for validation and business logic

### Model Conventions
- **Naming**: Use descriptive names that clearly indicate purpose
- **Properties**: Use proper C# property conventions with get/set
- **Validation**: Include appropriate validation attributes
- **Documentation**: Add XML comments for all public members
- **Immutability**: Consider immutable models for data that shouldn't change

## ?? Related Documentation

- **Services**: See `Services/README.md` for service integration patterns
- **Database Schema**: `Development/Database_Files/README_Development_Database_Schema.md`
- **Validation Rules**: Business rule documentation in `Development/Custom_Prompts/`
- **API Documentation**: Generated from XML comments for external integration

---

*This directory contains the data layer of the MTM WIP Application, defining all business entities, DTOs, and data structures used throughout the application.*