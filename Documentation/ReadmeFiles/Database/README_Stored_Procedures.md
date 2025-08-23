# Existing Stored Procedures (Production)

**Status**: Production Ready  
**Environment**: Production  
**Last Updated**: [To be updated during deployment]

## ?? CRITICAL - READ-ONLY FILE

**This file contains the current production stored procedures and is READ-ONLY.**

### For Modifications:
- **New Procedures**: Add to `Development/Database_Files/New_Stored_Procedures.sql`
- **Update Existing**: Copy to `Development/Database_Files/Updated_Stored_Procedures.sql` and modify

## Overview
This file contains all stored procedures currently deployed in the production MTM WIP Application database. All application database operations must use these procedures.

## ?? NO HARD-CODED SQL RULE
**ALL database operations in the application code MUST use stored procedures from this file.**

Prohibited:
```csharp
// ? NEVER DO THIS
var query = "SELECT * FROM inventory WHERE part_id = @partId";
```

Required:
```csharp
// ? ALWAYS DO THIS
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_inventory_Get_ByPartID",
    new Dictionary<string, object> { ["PartID"] = partId }
);
```

## Stored Procedure Categories

### Inventory Management
- `sp_inventory_Get_ByPartID` - Retrieve inventory by part ID
- `sp_inventory_Get_ByLocation` - Retrieve inventory by location
- `sp_inventory_Add_Transaction` - Add new inventory transaction
- `sp_inventory_Update_Quantity` - Update inventory quantities
- `sp_inventory_Transfer_Location` - Transfer inventory between locations

### Transaction Processing
- `sp_transaction_Create_IN` - Process IN transactions
- `sp_transaction_Create_OUT` - Process OUT transactions  
- `sp_transaction_Create_TRANSFER` - Process TRANSFER transactions
- `sp_transaction_Get_History` - Retrieve transaction history
- `sp_transaction_Get_ByUser` - Get transactions by user

### User Management
- `sp_user_Authenticate` - User authentication
- `sp_user_Get_Permissions` - Retrieve user permissions
- `sp_user_Log_Activity` - Log user activities
- `sp_user_Get_Profile` - Get user profile information

### Part Management
- `sp_part_Get_ByID` - Retrieve part information
- `sp_part_Search` - Search parts by criteria
- `sp_part_Get_Operations` - Get valid operations for part
- `sp_part_Validate_Operation` - Validate part-operation combination

### Location Management
- `sp_location_Get_All` - Get all locations
- `sp_location_Get_ByType` - Get locations by type
- `sp_location_Validate` - Validate location exists
- `sp_location_Get_Capacity` - Get location capacity information

### Error Handling & Logging
- `sp_error_Log_Exception` - Log application errors
- `sp_audit_Log_Transaction` - Log audit trail
- `sp_system_Get_Configuration` - Get system configuration
- `sp_performance_Log_Metrics` - Log performance metrics

## Security Standards

### Parameter Validation
All stored procedures include:
- Input parameter validation
- Data type checking
- Range validation
- SQL injection prevention

### Access Control
- User permission verification
- Role-based access control
- Audit logging for all operations
- Error message sanitization

### Error Handling
```sql
-- Standard error handling pattern in all procedures
BEGIN TRY
    -- Procedure logic here
    
    SET @Success = 1
    SET @Message = 'Operation completed successfully'
    
END TRY
BEGIN CATCH
    SET @Success = 0
    SET @Message = 'Operation failed: ' + ERROR_MESSAGE()
    
    -- Log error details
    INSERT INTO error_log (error_message, procedure_name, user_id, created_date)
    VALUES (ERROR_MESSAGE(), OBJECT_NAME(@@PROCID), @UserID, GETDATE())
    
END CATCH
```

## Performance Standards

### Optimization Features
- Proper indexing utilization
- Query plan optimization
- Parameter sniffing prevention
- Connection efficiency

### Monitoring
- Execution time tracking
- Resource usage monitoring
- Deadlock detection
- Query performance analysis

## Usage Guidelines

### Calling Convention
```csharp
// Standard pattern for all stored procedure calls
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_ProcedureName",
    new Dictionary<string, object> 
    {
        ["Parameter1"] = value1,
        ["Parameter2"] = value2,
        ["UserID"] = currentUserId  // Always include UserID for audit
    }
);

// Check result status
if (result.Success)
{
    // Process successful result
    var data = result.Data;
}
else
{
    // Handle error
    var errorMessage = result.Message;
}
```

### Error Handling Pattern
```csharp
try
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString, procedureName, parameters);
        
    if (!result.Success)
    {
        // Log and handle database error
        await _errorHandler.LogErrorAsync($"Database operation failed: {result.Message}");
        return Result.Failure(result.Message);
    }
    
    return Result.Success(result.Data);
}
catch (Exception ex)
{
    // Log and handle connection/system errors
    await _errorHandler.LogErrorAsync(ex);
    return Result.Failure("Database operation failed");
}
```

## Testing Requirements

### Validation Process
Before deployment, all procedures must pass:
- Unit testing with various parameter combinations
- Performance testing under load
- Security testing for SQL injection
- Integration testing with application code

### Test Coverage
- Valid input scenarios
- Invalid input handling
- Edge cases and boundary conditions
- Error condition testing
- Performance benchmarks

## Deployment History

### Version Control
- All changes tracked in version control
- Deployment scripts for each release
- Rollback procedures documented
- Change impact analysis

### Release Notes
[To be updated with each deployment]
- New procedures added
- Existing procedures modified
- Performance improvements
- Security enhancements

## Development Workflow

### Making Changes
1. **Identify Need**: Document requirement for new/modified procedure
2. **Development**: Create/modify in `Development/Database_Files/`
3. **Testing**: Thorough testing in development environment
4. **Review**: Code review and security analysis
5. **Staging**: Deploy to staging for integration testing
6. **Production**: Deploy through change management process

### Quality Gates
- Code review approval
- Security scan completion
- Performance validation
- Integration test success
- Documentation updates

## Related Documentation
- [New Stored Procedures](../Development/Database_Files/README_New_Stored_Procedures.md)
- [Updated Stored Procedures](../Development/Database_Files/README_Updated_Stored_Procedures.md)  
- [Database Operations Guide](../Docs/database-operations-prompts.md)
- [Production Schema](README_Production_Database_Schema.md)

---
**For new procedures, see**: `Development/Database_Files/New_Stored_Procedures.sql`  
**For procedure updates, see**: `Development/Database_Files/Updated_Stored_Procedures.sql`