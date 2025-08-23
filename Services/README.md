# Services Directory

This directory contains all service classes that provide business logic, data access, and utility functionality for the MTM WIP Application Avalonia.

## ??? Service Architecture

### Service Categories

#### Core Services (`ICoreServices.cs`)
Essential services required for basic application functionality:
- **DatabaseService**: MySQL database connectivity and management
- **ConfigurationService**: Application configuration and settings management
- **LoggingUtility**: Comprehensive logging system for debugging and audit trails

#### Business Services (`IBusinessServices.cs`)
Domain-specific services for inventory and business operations:
- **InventoryService**: Core inventory management operations
- **UserAndTransactionServices**: User management and transaction processing
- **ApplicationStateService**: Application-wide state management

#### Utility Services
Support services for enhanced functionality:
- **Service_ErrorHandler**: Centralized error handling and user notification
- **Helper classes**: Various utility functions and database helpers

## ?? Service Files

### Core Infrastructure Services

#### `DatabaseService.cs`
Primary database connectivity and operations service.
```csharp
public class DatabaseService : IDatabaseService
{
    // MySQL connection management
    // Stored procedure execution
    // Transaction handling
    // Connection pooling
}
```

**Key Features:**
- Connection string management
- Stored procedure execution with parameters
- Transaction management and rollback
- Connection health monitoring
- Performance optimization

#### `ConfigurationService.cs`
Application configuration and settings management.
```csharp
public class ConfigurationService : IConfigurationService
{
    // Application settings
    // User preferences
    // Database configuration
    // UI theme settings
}
```

**Key Features:**
- Configuration file management
- Environment-specific settings
- User preference persistence
- Runtime configuration updates

#### `LoggingUtility.cs`
Comprehensive logging system for the application.
```csharp
public class LoggingUtility
{
    // Error logging
    // Debug tracing
    // User action logging
    // Performance monitoring
}
```

**Key Features:**
- Multi-level logging (Debug, Info, Warning, Error)
- File-based and database logging
- User action audit trails
- Performance metrics collection

### Business Logic Services

#### `InventoryService.cs`
Core inventory management operations and business logic.
```csharp
public class InventoryService : IInventoryService
{
    // Inventory CRUD operations
    // Transaction processing
    // Business rule validation
    // Inventory analytics
}
```

**Key Features:**
- Add, update, delete inventory items
- Transfer operations between locations
- Quantity management and validation
- Search and filtering capabilities
- Business rule enforcement

#### `UserService.cs`
User management and authentication service.
```csharp
public class UserService : IUserService
{
    // User authentication
    // Permission management
    // User profile management
    // User activity tracking
}
```

**Key Features:**
- User authentication and authorization
- Role-based access control
- User profile management
- User preference persistence

#### `TransactionService.cs`
Transaction processing and audit service.
```csharp
public class TransactionService : ITransactionService
{
    // Transaction processing
    // Transaction history
    // Audit logging
    // Transaction validation
}
```

**Key Features:**
- Transaction audit logging
- Transaction history management
- Business rule validation
- Transaction reporting

#### `ApplicationStateService.cs`
Application-wide state management service.
```csharp
public class ApplicationStateService : IApplicationStateService
{
    // Global application state
    // User session management
    // Cache management
    // Event coordination
}
```

**Key Features:**
- Global state management
- User session tracking
- Inter-service communication
- Cache coordination

### Support and Infrastructure Services

#### `CacheService.cs` (SimpleCacheService)
Application-wide caching service using IMemoryCache.
```csharp
public class SimpleCacheService : ICacheService
{
    // Memory cache management
    // Cache expiration handling
    // Pattern-based cache operations
    // Cache performance optimization
}
```

**Key Features:**
- In-memory caching with expiration
- Async cache operations
- Cache key pattern operations
- Performance optimization

#### `ValidationService.cs` (SimpleValidationService)
Data validation and business rule validation service.
```csharp
public class SimpleValidationService : IValidationService
{
    // Data validation
    // Business rule validation
    // Input sanitization
    // Validation error handling
}
```

**Key Features:**
- Comprehensive data validation
- Business rule enforcement
- Input parameter validation
- Validation error reporting

#### `DbConnectionFactory.cs` (MySqlConnectionFactory)
Database connection factory for MySQL operations.
```csharp
public class MySqlConnectionFactory : IDbConnectionFactory
{
    // MySQL connection creation
    // Connection string management
    // Connection pooling
    // Connection health monitoring
}
```

**Key Features:**
- MySQL connection management
- Connection pooling optimization
- Connection string validation
- Database connectivity monitoring

### Utility and Support Services

#### `Service_ErrorHandler.cs`
Centralized error handling and user notification system.
```csharp
public class Service_ErrorHandler
{
    // Exception handling
    // User-friendly error messages
    // Error logging and reporting
    // Error dialog management
}
```

**Key Features:**
- Centralized exception handling
- User-friendly error message translation
- Error logging to database and file
- Custom error dialog presentations

## ?? Service Integration

### Dependency Injection Setup
Services are registered using the comprehensive `AddMTMServices` extension method in `Extensions/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
{
    // Configure strongly-typed settings
    services.Configure<MTMSettings>(configuration.GetSection("MTM"));
    services.Configure<DatabaseSettings>(configuration.GetSection("Database"));
    services.Configure<ErrorHandlingSettings>(configuration.GetSection("ErrorHandling"));
    services.Configure<LoggingSettings>(configuration.GetSection("Logging"));

    // Add settings validation
    services.AddSingleton<IValidateOptions<MTMSettings>, ConfigurationValidationService>();

    // Add core infrastructure services
    services.AddSingleton<IConfigurationService, ConfigurationService>();
    services.AddSingleton<IApplicationStateService, MTMApplicationStateService>();

    // Add database services
    services.AddScoped<IDatabaseService, DatabaseService>();
    services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
    services.AddTransient<DatabaseTransactionService>();

    // Add business services
    services.AddScoped<IInventoryService, InventoryService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<ITransactionService, TransactionService>();

    // Add caching services
    services.AddMemoryCache();
    services.AddSingleton<ICacheService, SimpleCacheService>();

    // Add validation services
    services.AddScoped<IValidationService, SimpleValidationService>();

    return services;
}
```

### Service Lifetimes
- **Singleton**: Services that maintain state across the application
  - `IConfigurationService` - Application configuration and settings
  - `IApplicationStateService` - Global application state management
  - `IDbConnectionFactory` - Database connection factory
  - `ICacheService` - Application-wide caching
- **Scoped**: Services that are created per operation scope
  - `IDatabaseService` - Database operations and connectivity
  - `IInventoryService` - Inventory management operations
  - `IUserService` - User management and authentication
  - `ITransactionService` - Transaction processing
  - `IValidationService` - Data validation services
- **Transient**: Services that are created each time they're requested
  - `DatabaseTransactionService` - Database transaction management

## ?? Database Integration

### Stored Procedure Pattern
All services follow the stored procedure-only pattern:

```csharp
// ? CORRECT: Using stored procedures
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "sp_ProcedureName",
    new Dictionary<string, object> 
    {
        ["Parameter1"] = value1,
        ["Parameter2"] = value2
    }
);

// ? PROHIBITED: Direct SQL
// var query = "SELECT * FROM table WHERE column = @value"; // NEVER DO THIS
```

### Transaction Type Logic
Services implement correct transaction type determination:

```csharp
// ? CORRECT: TransactionType based on user intent
public async Task<Result> AddStockAsync(string partId, string operation, int quantity)
{
    var transaction = new InventoryTransaction
    {
        TransactionType = TransactionType.IN, // User is adding stock
        Operation = operation, // Just a workflow step number
        // ... other properties
    };
}
```

## ?? Error Handling

### Comprehensive Error Management
All services implement consistent error handling:

```csharp
public async Task<Result<T>> ServiceOperationAsync<T>(params object[] parameters)
{
    try
    {
        // Service operation logic
        var result = await PerformOperationAsync(parameters);
        return Result<T>.Success(result);
    }
    catch (Exception ex)
    {
        // Log error with full context
        await LoggingUtility.LogErrorAsync(ex, "ServiceOperation", parameters);
        
        // Return user-friendly error
        return Result<T>.Failure($"Operation failed: {ex.Message}");
    }
}
```

### Error Categories
- **Validation Errors**: Input parameter validation failures
- **Business Rule Violations**: Business logic constraint violations
- **Database Errors**: Database connectivity or constraint issues
- **System Errors**: Unexpected system-level errors

## ?? Security Considerations

### Data Access Security
- **Parameter Validation**: All input parameters validated before use
- **SQL Injection Prevention**: Stored procedures only, no dynamic SQL
- **Permission Checks**: User authorization validated for all operations
- **Audit Logging**: All operations logged for security audit trails

### Best Practices
- **Principle of Least Privilege**: Services only access required resources
- **Input Sanitization**: All user input sanitized and validated
- **Error Information Limits**: Error messages don't expose sensitive information
- **Secure Logging**: Sensitive data excluded from logs

## ?? Performance Optimization

### Service Performance
- **Async Operations**: All I/O operations are asynchronous
- **Connection Pooling**: Database connections managed efficiently
- **Caching Strategies**: Frequently accessed data cached appropriately
- **Lazy Loading**: Resources loaded only when needed

### Monitoring and Metrics
- **Performance Logging**: Operation timing and performance metrics
- **Resource Usage Tracking**: Memory and database connection monitoring
- **Error Rate Monitoring**: Service error rates and patterns
- **User Activity Analytics**: Service usage patterns and optimization opportunities

## ?? Testing Guidelines

### Service Testing Patterns
- **Unit Tests**: Individual service method testing
- **Integration Tests**: Service interaction testing
- **Mock Dependencies**: External dependency mocking for isolation
- **Performance Tests**: Service performance validation

### Test Organization
```
Tests/
??? UnitTests/
?   ??? Services/
?   ?   ??? DatabaseServiceTests.cs
?   ?   ??? InventoryServiceTests.cs
?   ?   ??? ConfigurationServiceTests.cs
??? IntegrationTests/
?   ??? ServiceIntegrationTests.cs
?   ??? DatabaseIntegrationTests.cs
??? PerformanceTests/
    ??? ServicePerformanceTests.cs
```

## ?? Development Guidelines

### Adding New Services
1. **Define Interface**: Create interface in `Interfaces/` directory
2. **Implement Service**: Create service class with comprehensive error handling
3. **Register Service**: Add to `ServiceCollectionExtensions.cs`
4. **Add Documentation**: Update this README and add XML comments
5. **Write Tests**: Create unit and integration tests

### Service Conventions
- **Naming**: Services end with `Service` (e.g., `InventoryService`)
- **Interfaces**: Interfaces start with `I` (e.g., `IInventoryService`)
- **Async Methods**: All I/O operations should be async with `Async` suffix
- **Result Pattern**: Use `Result<T>` for operation results with error handling
- **Logging**: Comprehensive logging for all operations and errors

## ?? Related Documentation

- **Interface Definitions**: See `Services/Interfaces/` for service contracts
- **Database Schema**: `Development/Database_Files/README_Development_Database_Schema.md`
- **Error Handling**: `Development/Custom_Prompts/Database_Error_Handling.md`
- **Code Examples**: `Development/Examples/` for service usage patterns

---

*This directory contains the core business logic and data access layer of the MTM WIP Application, following clean architecture principles and modern .NET practices.*