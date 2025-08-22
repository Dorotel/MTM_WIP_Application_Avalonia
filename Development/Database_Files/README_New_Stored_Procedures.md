# New Stored Procedures - Development

## Overview
This file contains **new stored procedures** being developed for the MTM WIP Application. These procedures are not yet in production and are being tested and refined in the development environment.

## ?? CRITICAL DATABASE RULE ??
**ALL database operations in application code MUST use stored procedures ONLY.**
- **PROHIBITED**: Direct SQL queries in C# code
- **REQUIRED**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` pattern

## File Information
- **Source File**: `New_Stored_Procedures.sql`
- **Database**: `mtm_wip_application_test` (development)
- **Status**: Under development and testing
- **Deployment**: Will be promoted to production after testing

## Development Process

### Adding New Procedures
1. **Design**: Define procedure purpose, parameters, and return values
2. **Implement**: Add procedure to `New_Stored_Procedures.sql`
3. **Document**: Add detailed documentation below
4. **Test**: Comprehensive testing in development environment
5. **Review**: Code review and approval process
6. **Deploy**: Move to production when ready

### Updating New Procedures
- Directly overwrite procedures in `New_Stored_Procedures.sql`
- Update documentation below
- Notify team of changes
- Re-run all tests

## Status: EMPTY

**Current Status**: No new stored procedures have been added yet.

**Next Steps When Adding New Procedures**:
1. Add the complete stored procedure definition to `New_Stored_Procedures.sql`
2. Document the procedure in the appropriate category below
3. Update corresponding application service layers
4. Add comprehensive error handling and transaction management
5. Include parameter validation and security measures

---

## Categories for New Procedures

### Inventory Management Extensions
*Place new inventory-related procedures here*

**Potential Areas for Enhancement**:
- Advanced inventory reporting procedures
- Bulk inventory operations with error handling
- Inventory optimization and reorder point calculations
- Cross-location inventory analysis procedures

### Transaction Analytics
*Place new transaction analysis procedures here*

**Potential Areas for Enhancement**:
- Transaction pattern analysis procedures
- Performance metrics calculation procedures
- Usage trend reporting procedures
- Transaction validation and audit procedures

### User Experience Enhancements
*Place new user experience procedures here*

**Potential Areas for Enhancement**:
- Enhanced quick button management procedures
- User preference synchronization procedures
- Activity dashboard data procedures
- Advanced personalization features

### System Administration
*Place new system administration procedures here*

**Potential Areas for Enhancement**:
- Database maintenance and cleanup procedures
- Performance monitoring procedures
- Data archival and purging procedures
- System health check procedures

### Security and Auditing
*Place new security procedures here*

**Potential Areas for Enhancement**:
- Enhanced access control procedures
- Audit trail analysis procedures
- Security event logging procedures
- Compliance reporting procedures

### Integration Support
*Place new integration procedures here*

**Potential Areas for Enhancement**:
- External system interface procedures
- Data import/export procedures
- Synchronization with legacy systems
- API support procedures

---

## Development Guidelines

### Procedure Naming Convention
Follow the established pattern: `{category}_{entity}_{action}_{specifics}`

**Examples**:
- `inv_inventory_Get_ByDateRange`
- `sys_analytics_Calculate_UsageMetrics`
- `usr_preferences_Sync_FromProfile`
- `maint_cleanup_Archive_OldTransactions`

### Parameter Conventions
- **Input Parameters**: Prefix with `p_` or `in_`
- **Output Parameters**: Prefix with `p_` and declare as `OUT`
- **Input/Output Parameters**: Prefix with `p_` and declare as `INOUT`

**Standard Output Parameters** (required for all procedures):
```sql
OUT p_Status INT,           -- 0=success, 1=warning, -1=error
OUT p_ErrorMsg VARCHAR(255) -- Success/error message
```

### Error Handling Template
All new procedures must include comprehensive error handling:

```sql
DELIMITER $$
CREATE PROCEDURE new_procedure_name(
    IN p_Parameter1 VARCHAR(100),
    IN p_Parameter2 INT,
    OUT p_Status INT,
    OUT p_ErrorMsg VARCHAR(255)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        GET DIAGNOSTICS CONDITION 1
            @sqlstate = RETURNED_SQLSTATE, 
            @errno = MYSQL_ERRNO, 
            @text = MESSAGE_TEXT;
        SET p_Status = -1;
        SET p_ErrorMsg = CONCAT('Database error in new_procedure_name: ', @text);
    END;

    -- Initialize output parameters
    SET p_Status = 0;
    SET p_ErrorMsg = '';

    -- Validate input parameters
    IF p_Parameter1 IS NULL OR p_Parameter1 = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Parameter1 is required';
    END IF;

    START TRANSACTION;

    -- Procedure logic here
    -- Use appropriate business logic
    
    COMMIT;
    SET p_Status = 0;
    SET p_ErrorMsg = 'Operation completed successfully';
END$$
DELIMITER ;
```

### Documentation Template
Each new procedure must be documented with:

```sql
-- =====================================================
-- Procedure: procedure_name
-- Purpose: Brief description of what the procedure does
-- Parameters:
--   IN p_Parameter1 (TYPE): Description
--   OUT p_Status (INT): Success (0), warning (1), or error (-1)
--   OUT p_ErrorMsg (VARCHAR(255)): Status message
-- Returns: Description of any result sets returned
-- Business Rules:
--   - Rule 1
--   - Rule 2
-- Related Tables: 
--   - table1 (READ/WRITE)
--   - table2 (READ)
-- Security: Special security considerations
-- Performance: Performance characteristics and notes
-- Author: Developer name
-- Created: YYYY-MM-DD
-- Modified: YYYY-MM-DD - Change description
-- =====================================================
```

### Application Integration Pattern
When creating new procedures, plan for application integration:

```csharp
// Service layer method
public async Task<Result<DataTable>> NewOperationAsync(string param1, int param2)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "new_procedure_name",
            new Dictionary<string, object>
            {
                ["p_Parameter1"] = param1,
                ["p_Parameter2"] = param2
            }
        );

        if (result.Success)
        {
            return Result<DataTable>.Success(result.Data);
        }
        else
        {
            return Result<DataTable>.Failure(result.ErrorMessage);
        }
    }
    catch (Exception ex)
    {
        await Service_ErrorHandler.LogErrorAsync(ex, "NewOperationAsync");
        return Result<DataTable>.Failure($"Operation failed: {ex.Message}");
    }
}
```

## Security Requirements

All new procedures must implement:

### Input Validation
```sql
-- Example parameter validation
IF p_PartID IS NULL OR p_PartID = '' THEN
    SET p_Status = -1;
    SET p_ErrorMsg = 'PartID parameter is required';
    LEAVE procedure_block;
END IF;

-- Foreign key validation
IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
    SET p_Status = 1;
    SET p_ErrorMsg = CONCAT('PartID not found: ', p_PartID);
    LEAVE procedure_block;
END IF;
```

### Access Control
```sql
-- User validation where appropriate
IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_User) THEN
    SET p_Status = -1;
    SET p_ErrorMsg = 'Invalid user';
    LEAVE procedure_block;
END IF;
```

### SQL Injection Prevention
- Use parameterized queries only
- Validate all input parameters
- Never construct dynamic SQL with user input
- Use prepared statements for complex queries

## Performance Standards

### Query Optimization
- Ensure all WHERE clauses use indexed columns
- Use appropriate JOINs instead of subqueries where possible
- Limit result sets with realistic bounds
- Use EXPLAIN to validate query execution plans

### Resource Management
```sql
-- Batch processing for large operations
DECLARE batch_size INT DEFAULT 1000;
DECLARE processed_count INT DEFAULT 0;

WHILE processed_count < total_records DO
    -- Process batch
    -- Update processed_count
    -- Check for termination conditions
END WHILE;
```

### Monitoring Integration
```sql
-- Add execution time monitoring for complex procedures
DECLARE start_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
-- Procedure logic
DECLARE end_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP;
-- Log execution time if needed
```

## Testing Requirements

### Unit Test Template
```csharp
[Test]
public async Task NewProcedure_ShouldReturnSuccess_WhenValidParameters()
{
    // Arrange
    var parameters = new Dictionary<string, object>
    {
        ["p_Parameter1"] = "valid_value",
        ["p_Parameter2"] = 123
    };

    // Act
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "new_procedure_name",
        parameters
    );

    // Assert
    Assert.IsTrue(result.Success);
    Assert.AreEqual(0, result.OutputParameters["p_Status"]);
    Assert.IsNotNull(result.Data);
}

[Test]
public async Task NewProcedure_ShouldReturnError_WhenInvalidParameters()
{
    // Arrange
    var parameters = new Dictionary<string, object>
    {
        ["p_Parameter1"] = "", // Invalid empty value
        ["p_Parameter2"] = 123
    };

    // Act
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "new_procedure_name",
        parameters
    );

    // Assert
    Assert.IsFalse(result.Success);
    Assert.AreEqual(-1, result.OutputParameters["p_Status"]);
    Assert.IsTrue(result.OutputParameters["p_ErrorMsg"].ToString().Contains("required"));
}
```

### Integration Testing
- Test with realistic data volumes
- Test concurrent access scenarios
- Test error recovery and rollback
- Test performance under load

## Migration to Production

### Pre-Migration Checklist
- [ ] All unit tests pass
- [ ] Integration tests pass
- [ ] Performance tests meet requirements
- [ ] Security review completed
- [ ] Code review approved
- [ ] Documentation complete and accurate
- [ ] Staging environment testing successful

### Promotion Process
1. **Final Testing**: Complete testing in development environment
2. **Code Review**: Peer review and approval
3. **Documentation**: Update all documentation
4. **Staging Deploy**: Test in staging environment
5. **Production Deploy**: Move to production stored procedures file
6. **Verification**: Verify successful deployment
7. **Cleanup**: Remove from development file or mark as deployed

## Quality Assurance

### Code Review Checklist
- [ ] Follows naming conventions
- [ ] Includes comprehensive error handling
- [ ] Has proper parameter validation
- [ ] Uses transactions appropriately
- [ ] Includes adequate documentation
- [ ] Meets performance requirements
- [ ] Follows security best practices
- [ ] Has complete test coverage

### Performance Standards
- Execution time under specified limits for expected data volumes
- Memory usage within acceptable bounds
- Scalable design for growth
- Efficient use of database resources

## Related Documentation

- [Updated Stored Procedures](README_Updated_Stored_Procedures.md) - Modified existing procedures
- [Development Database Schema](README_Development_Database_Schema.md) - Development database structure
- [Production Stored Procedures](../../Database_Files/README_Existing_Stored_Procedures.md) - Current production procedures
- [Main Development README](README.md) - Development workflow overview

## Development Notes

### Common Patterns to Follow
1. **Consistent Error Handling**: Use standard error handling pattern for all procedures
2. **Parameter Validation**: Validate all inputs before processing
3. **Transaction Management**: Use transactions for data consistency
4. **Documentation**: Document all business rules and usage patterns
5. **Testing**: Comprehensive testing before promotion

### Common Pitfalls to Avoid
1. **Missing Error Handling**: All procedures must handle errors gracefully
2. **Poor Parameter Validation**: Always validate input parameters
3. **Transaction Issues**: Use appropriate transaction boundaries
4. **Performance Problems**: Consider indexing and query optimization
5. **Security Gaps**: Validate user permissions and prevent SQL injection

This file serves as the development workspace for creating robust, well-tested stored procedures that will enhance the MTM WIP Application's database functionality while maintaining the highest standards of security, performance, and reliability.