# Database Error Handling - Custom Prompt

## Instructions
Use this prompt when you need to implement comprehensive error handling for stored procedures or database operations.

## Prompt Template

```
Implement comprehensive error handling for [DATABASE_OPERATION/STORED_PROCEDURE] with the following requirements:

**Target**: [Specific stored procedure or database operation]

**Error Handling Scope**: [New Procedure/Existing Procedure Update/Service Layer/Complete System]

**Requirements**:
1. Follow MTM database standards for error handling
2. Use stored procedures only - NO direct SQL in application code
3. Implement transaction management with proper rollback
4. Provide meaningful error messages for users and developers
5. Include comprehensive logging for troubleshooting
6. Handle all error scenarios (database errors, business rule violations, validation failures)
7. Return consistent error status codes and messages
8. Integrate with existing error logging system

**Error Categories to Handle**:

### Database/System Errors
- Connection failures
- Timeout issues
- Constraint violations
- Deadlocks and locking issues
- SQL syntax errors
- Insufficient permissions

### Business Rule Violations
- Invalid data relationships
- Business logic constraint failures
- Workflow validation errors
- Data consistency violations

### Input Validation Errors
- Missing required parameters
- Invalid data types or formats
- Out-of-range values
- Invalid references to other entities

### Application Integration Errors
- Service layer error handling
- Error message translation
- User notification patterns
- Logging and monitoring integration

**Specific Requirements**:

### Stored Procedure Error Handling
- Comprehensive SQL exception handling
- Transaction management (START/COMMIT/ROLLBACK)
- Consistent output parameter patterns
- Error logging integration
- Meaningful error messages

### Service Layer Error Handling
- Database result processing
- Error translation for UI
- Logging integration
- User-friendly error messages
- Retry logic where appropriate

### System-Wide Error Patterns
- Consistent error code standards
- Error message templates
- Logging standards and formats
- Monitoring and alerting integration

Please provide:
1. Complete error handling implementation
2. Error code standards and documentation
3. Integration with existing error logging system
4. Service layer error handling patterns
5. User-friendly error message examples
6. Testing scenarios for all error conditions
```

## Usage Examples

### Example 1: Comprehensive Stored Procedure Error Handling
```
Implement comprehensive error handling for inventory management stored procedures with the following requirements:

**Target**: All inventory stored procedures (inv_inventory_* and inv_transaction_*)

**Error Handling Scope**: Complete error handling framework for inventory operations

**Specific Requirements**:

### Database Error Handling
- SQL exception handling with detailed error capture
- Transaction rollback on any failure
- Deadlock detection and retry logic
- Constraint violation specific error messages
- Performance monitoring for slow queries

### Business Rule Error Handling
- Part ID validation against md_part_ids table
- Location validation against md_locations table
- Quantity validation (positive numbers, realistic ranges)
- Batch number validation and consistency checks
- User permission validation

### Input Validation
- Required parameter checking
- Data type and format validation
- Range checking for quantities and dates
- Foreign key reference validation
- Special character and injection prevention

### Error Logging Integration
- Log all errors to log_error table using log_error_Add_Error procedure
- Include procedure name, parameters, error details
- Capture user context and timing information
- Severity classification (Information, Warning, Error, Critical)

### Output Standards
- Consistent p_Status codes: 0=success, 1=warning, -1=error
- Detailed p_ErrorMsg with specific failure information
- Additional output parameters for error context

**Error Scenarios to Handle**:
- Invalid part IDs
- Non-existent locations
- Insufficient inventory quantities
- Batch number conflicts
- User permission failures
- Database connectivity issues
- Transaction deadlocks

[Continue with detailed requirements...]
```

### Example 2: Service Layer Error Integration
```
Implement comprehensive error handling for the InventoryService class with the following requirements:

**Target**: InventoryService.cs and related service classes

**Error Handling Scope**: Service layer error handling with database integration

**Specific Requirements**:

### Database Result Processing
- Parse stored procedure output parameters
- Handle database connection failures
- Process timeout scenarios
- Interpret error status codes
- Extract meaningful error messages

### Error Translation
- Convert database error codes to user-friendly messages
- Localization support for error messages
- Context-specific error information
- Technical vs. user error message separation

### Logging Integration
- Use Service_ErrorHandler for all error logging
- Include service context and method information
- Capture parameter values (sanitized)
- Performance timing information
- User and session context

### User Notification
- Return Result<T> patterns with success/failure states
- Provide actionable error messages
- Include suggested resolution steps
- Error categorization for UI display

### Retry Logic
- Automatic retry for transient failures
- Exponential backoff for database timeouts
- Circuit breaker pattern for persistent failures
- User notification for retry attempts

**Integration Points**:
- Helper_Database_StoredProcedure class
- Service_ErrorHandler logging
- LoggingUtility for debugging
- UI error display components

[Continue with detailed requirements...]
```

### Example 3: System-Wide Error Standards
```
Implement comprehensive error handling standards for the entire MTM WIP Application with the following requirements:

**Target**: Complete application error handling framework

**Error Handling Scope**: System-wide standards and implementation

**Specific Requirements**:

### Error Code Standardization
- Consistent error code numbering system
- Category-based error code ranges
- Severity level classifications
- Error code documentation

### Error Message Templates
- Standardized message formats
- Parameter substitution patterns
- Multi-language support preparation
- Technical vs. user message separation

### Logging Framework
- Centralized error logging via stored procedures
- Performance logging and monitoring
- Security event logging
- Audit trail requirements

### Monitoring and Alerting
- Critical error alerting
- Performance threshold monitoring
- Error rate trend analysis
- System health dashboard integration

### Error Recovery Patterns
- Automatic retry strategies
- Graceful degradation patterns
- Circuit breaker implementations
- User notification and guidance

**Implementation Areas**:
- Stored procedure error handling
- Service layer error management
- UI error display and user experience
- API error responses
- Background job error handling

[Continue with detailed requirements...]
```

## Error Handling Patterns

### Stored Procedure Error Handler Template
```sql
DELIMITER $$
CREATE PROCEDURE example_procedure(
    IN p_Parameter1 VARCHAR(100),
    IN p_User VARCHAR(100),
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    -- Declare variables for error handling
    DECLARE v_ErrorCode INT DEFAULT 0;
    DECLARE v_ErrorMessage TEXT DEFAULT '';
    DECLARE v_RowCount INT DEFAULT 0;
    
    -- Exit handler for SQL exceptions
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        -- Rollback any transaction
        ROLLBACK;
        
        -- Capture error details
        GET DIAGNOSTICS CONDITION 1
            v_ErrorCode = MYSQL_ERRNO,
            v_ErrorMessage = MESSAGE_TEXT;
        
        -- Log the error
        CALL log_error_Add_Error(
            p_User,                           -- User
            'Error',                          -- Severity
            'DATABASE',                       -- ErrorType
            CONCAT('Error in example_procedure: ', v_ErrorMessage), -- ErrorMessage
            NULL,                            -- StackTrace
            'DATABASE',                      -- ModuleName
            'example_procedure',             -- MethodName
            CONCAT('Parameters: p_Parameter1=', IFNULL(p_Parameter1, 'NULL')), -- AdditionalInfo
            @@hostname,                      -- MachineName
            @@version,                       -- OSVersion
            '1.0.0',                         -- AppVersion
            NOW(),                           -- ErrorTime
            @log_status,                     -- Output status
            @log_error                       -- Output error
        );
        
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error [', v_ErrorCode, ']: Operation failed');
    END;
    
    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = '';
    
    -- Input validation
    IF p_Parameter1 IS NULL OR p_Parameter1 = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Parameter1 is required and cannot be empty';
        LEAVE procedure_block;
    END IF;
    
    -- User validation
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_User) THEN
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Invalid user: ', p_User);
        LEAVE procedure_block;
    END IF;
    
    -- Business rule validation
    IF EXISTS (SELECT 1 FROM some_table WHERE field = p_Parameter1) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('Parameter1 value already exists: ', p_Parameter1);
        LEAVE procedure_block;
    END IF;
    
    -- Main transaction
    START TRANSACTION;
    
    -- Perform operations
    INSERT INTO some_table (field1, created_by, created_at)
    VALUES (p_Parameter1, p_User, NOW());
    
    SET v_RowCount = ROW_COUNT();
    
    IF v_RowCount = 0 THEN
        ROLLBACK;
        SET p_Status = 1;
        SET p_ErrorMsg = 'No records were inserted';
    ELSE
        COMMIT;
        SET p_Status = 0;
        SET p_ErrorMsg = CONCAT('Operation completed successfully. Rows affected: ', v_RowCount);
    END IF;
    
END$$
DELIMITER ;
```

### Service Layer Error Handling
```csharp
public class InventoryService
{
    private readonly DatabaseService _databaseService;
    private readonly ILogger<InventoryService> _logger;

    public async Task<Result<bool>> AddInventoryItemAsync(InventoryItemModel item, string user)
    {
        try
        {
            _logger.LogInformation("Adding inventory item {PartID} for user {User}", 
                item.PartID, user);

            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = item.PartID ?? string.Empty,
                ["p_Location"] = item.Location ?? string.Empty,
                ["p_Operation"] = item.Operation ?? string.Empty,
                ["p_Quantity"] = item.Quantity,
                ["p_ItemType"] = item.ItemType ?? "WIP",
                ["p_User"] = user ?? string.Empty,
                ["p_Notes"] = item.Notes ?? string.Empty
            };

            var result = await _databaseService.ExecuteStoredProcedureAsync(
                "inv_inventory_Add_Item", parameters);

            if (result.Success)
            {
                var status = Convert.ToInt32(result.OutputParameters["p_Status"]);
                var message = result.OutputParameters["p_ErrorMsg"]?.ToString() ?? "";

                switch (status)
                {
                    case 0: // Success
                        _logger.LogInformation("Successfully added inventory item {PartID}", item.PartID);
                        return Result<bool>.Success(true, message);
                    
                    case 1: // Warning
                        _logger.LogWarning("Warning adding inventory item {PartID}: {Message}", 
                            item.PartID, message);
                        return Result<bool>.Success(true, message);
                    
                    default: // Error
                        _logger.LogError("Error adding inventory item {PartID}: {Message}", 
                            item.PartID, message);
                        return Result<bool>.Failure(message);
                }
            }
            else
            {
                _logger.LogError("Database operation failed for inventory item {PartID}: {Error}", 
                    item.PartID, result.ErrorMessage);
                    
                await Service_ErrorHandler.LogErrorAsync(
                    new Exception(result.ErrorMessage),
                    "AddInventoryItemAsync",
                    user,
                    new { PartID = item.PartID, Location = item.Location }
                );

                return Result<bool>.Failure("Failed to add inventory item. Please try again.");
            }
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Timeout adding inventory item {PartID}", item.PartID);
            
            await Service_ErrorHandler.LogErrorAsync(ex, "AddInventoryItemAsync", user);
            
            return Result<bool>.Failure("Operation timed out. Please try again.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding inventory item {PartID}", item.PartID);
            
            await Service_ErrorHandler.LogErrorAsync(ex, "AddInventoryItemAsync", user, 
                new { PartID = item.PartID, Location = item.Location });
            
            return Result<bool>.Failure("An unexpected error occurred. Please contact support.");
        }
    }
}
```

### Error Code Standards
```csharp
public static class DatabaseErrorCodes
{
    // Success codes
    public const int Success = 0;
    public const int SuccessWithWarning = 1;
    
    // General error codes
    public const int GeneralError = -1;
    public const int ValidationError = -2;
    public const int BusinessRuleViolation = -3;
    public const int PermissionDenied = -4;
    
    // Inventory specific error codes
    public const int PartNotFound = -101;
    public const int LocationNotFound = -102;
    public const int InsufficientQuantity = -103;
    public const int InvalidBatchNumber = -104;
    
    // User management error codes
    public const int UserNotFound = -201;
    public const int UserAlreadyExists = -202;
    public const int InvalidRole = -203;
    
    // System error codes
    public const int DatabaseConnectionError = -901;
    public const int DatabaseTimeout = -902;
    public const int DeadlockDetected = -903;
}

public static class ErrorMessages
{
    public static string GetUserFriendlyMessage(int errorCode)
    {
        return errorCode switch
        {
            DatabaseErrorCodes.PartNotFound => "The specified part number was not found.",
            DatabaseErrorCodes.LocationNotFound => "The specified location is not valid.",
            DatabaseErrorCodes.InsufficientQuantity => "There is not enough inventory for this operation.",
            DatabaseErrorCodes.InvalidBatchNumber => "The batch number is not valid for this part.",
            DatabaseErrorCodes.UserNotFound => "User account not found.",
            DatabaseErrorCodes.DatabaseTimeout => "The operation took too long. Please try again.",
            _ => "An error occurred. Please contact support if the problem persists."
        };
    }
}
```

## Error Testing Patterns

### Unit Test for Error Conditions
```csharp
[TestClass]
public class ErrorHandlingTests
{
    [Test]
    public async Task AddInventoryItem_ShouldReturnValidationError_WhenPartIDIsEmpty()
    {
        // Arrange
        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = "", // Invalid empty part ID
            ["p_Location"] = "TEST-LOC",
            ["p_Operation"] = "100",
            ["p_Quantity"] = 10,
            ["p_ItemType"] = "WIP",
            ["p_User"] = "test_user",
            ["p_Notes"] = ""
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            connectionString, "inv_inventory_Add_Item", parameters);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(-1, result.OutputParameters["p_Status"]);
        Assert.IsTrue(result.OutputParameters["p_ErrorMsg"].ToString().Contains("required"));
    }

    [Test]
    public async Task AddInventoryItem_ShouldHandleDatabaseTimeout()
    {
        // Test timeout scenarios
    }

    [Test]
    public async Task AddInventoryItem_ShouldHandleConstraintViolation()
    {
        // Test constraint violation scenarios
    }
}
```

## Related Files
- Error Logging: `Database_Files/Existing_Stored_Procedures.sql` (log_error_* procedures)
- Service Error Handling: `Services/Service_ErrorHandler.cs`
- Database Service: `Services/DatabaseService.cs`
- Result Pattern: `Models/Result.cs`
- Logging Utility: `Services/LoggingUtility.cs`