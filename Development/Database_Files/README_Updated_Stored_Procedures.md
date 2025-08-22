# Updated Stored Procedures - Development

## Overview
This file contains **modified versions** of existing stored procedures from production. When existing procedures need updates, they are copied here, modified, and tested before being promoted back to production.

## ?? CRITICAL DATABASE RULE ??
**ALL database operations in application code MUST use stored procedures ONLY.**
- **PROHIBITED**: Direct SQL queries in C# code
- **REQUIRED**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` pattern

## File Information
- **Source File**: `Updated_Stored_Procedures.sql`
- **Database**: `mtm_wip_application_test` (development)
- **Status**: Modified versions of production procedures under testing
- **Source**: Procedures copied from `../../Database_Files/Existing_Stored_Procedures.sql`

## ?? CRITICAL RULE ??
**NEVER edit `../../Database_Files/Existing_Stored_Procedures.sql` directly**
- That file is **READ-ONLY** production code
- Always copy procedures to this file for modifications
- Test thoroughly before promoting changes back to production

## Development Process

### Updating Existing Procedures
1. **Copy**: Copy the original procedure from `Existing_Stored_Procedures.sql`
2. **Modify**: Make necessary changes in this file
3. **Document**: Document all changes and reasons
4. **Test**: Comprehensive testing in development environment
5. **Review**: Code review and approval process
6. **Deploy**: Replace production procedure when ready

### Workflow Steps
```
Production Procedure (READ-ONLY)
         ? (copy)
Development Update (this file)
         ? (test & review)
Production Deployment
         ? (replace original)
Updated Production Procedure
```

## Status: EMPTY

**Current Status**: No existing procedures have been modified yet.

**Next Steps When Updating Procedures**:
1. Copy the complete procedure from `Existing_Stored_Procedures.sql`
2. Modify the procedure in `Updated_Stored_Procedures.sql`
3. Document the changes in the appropriate section below
4. Update corresponding application service layers if needed
5. Test thoroughly before promoting to production

---

## Change Tracking

### Change Documentation Template
When updating a procedure, document using this format:

```
Procedure: {procedure_name}
Original Version: Found in Existing_Stored_Procedures.sql
Modified Version: Updated in this file
Change Date: YYYY-MM-DD
Modified By: {developer_name}
Reason for Change: {brief description}
Changes Made:
  - Change 1
  - Change 2
  - Change 3
Testing Status: [Not Started/In Progress/Complete]
Review Status: [Pending/Approved/Needs Changes]
Deployment Status: [Development/Staging/Production]
```

## Current Updated Procedures

*This section will be populated as procedures are modified*

---

## Common Update Scenarios

### Performance Optimization
**Example**: Optimizing a slow query in an existing procedure

```sql
-- Original (slow version)
SELECT * FROM large_table WHERE some_condition;

-- Updated (optimized version)
SELECT specific_columns FROM large_table 
WHERE indexed_column = @parameter
LIMIT 1000;
```

**Documentation Required**:
- Performance metrics before and after
- Index usage analysis
- Query execution plan comparison

### Bug Fixes
**Example**: Fixing a logic error in existing procedure

```sql
-- Original (with bug)
IF quantity > 0 THEN
    -- Missing validation

-- Updated (bug fixed)
IF quantity > 0 AND part_exists = TRUE THEN
    -- Added proper validation
```

**Documentation Required**:
- Description of the bug
- Impact assessment
- Test cases that verify the fix

### Feature Enhancements
**Example**: Adding new functionality to existing procedure

```sql
-- Original procedure parameters
IN p_PartID VARCHAR(300),
OUT p_Status INT

-- Updated procedure parameters (added new parameter)
IN p_PartID VARCHAR(300),
IN p_IncludeHistory BOOLEAN DEFAULT FALSE,
OUT p_Status INT,
OUT p_HistoryCount INT
```

**Documentation Required**:
- New functionality description
- Backwards compatibility analysis
- Migration plan for existing calls

### Security Improvements
**Example**: Adding parameter validation

```sql
-- Original (minimal validation)
BEGIN
    INSERT INTO table VALUES (p_param1, p_param2);

-- Updated (enhanced validation)
BEGIN
    -- Validate input parameters
    IF p_param1 IS NULL OR p_param1 = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Parameter1 is required';
        LEAVE;
    END IF;
    
    -- Check user permissions
    IF NOT EXISTS (SELECT 1 FROM usr_users WHERE User = p_User) THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'Invalid user';
        LEAVE;
    END IF;
    
    INSERT INTO table VALUES (p_param1, p_param2);
```

**Documentation Required**:
- Security vulnerabilities addressed
- New validation rules
- Impact on existing functionality

## Development Standards

### Code Modification Guidelines

#### Maintain Backwards Compatibility
- Don't change existing parameter names or types
- Don't change existing return values unless absolutely necessary
- Add new parameters as optional with defaults
- Preserve existing behavior for current usage patterns

#### Improve Error Handling
```sql
-- Standard error handling pattern for updated procedures
DECLARE EXIT HANDLER FOR SQLEXCEPTION
BEGIN
    ROLLBACK;
    GET DIAGNOSTICS CONDITION 1
        @sqlstate = RETURNED_SQLSTATE, 
        @errno = MYSQL_ERRNO, 
        @text = MESSAGE_TEXT;
    SET p_Status = -1;
    SET p_ErrorMsg = CONCAT('Database error in ', procedure_name, ': ', @text);
END;
```

#### Add Comprehensive Logging
```sql
-- Add logging for significant operations
INSERT INTO log_operation (
    ProcedureName, 
    Parameters, 
    ExecutionTime, 
    User, 
    Result
) VALUES (
    'procedure_name',
    CONCAT('param1=', p_param1, '; param2=', p_param2),
    execution_time,
    p_User,
    operation_result
);
```

### Testing Updated Procedures

#### Regression Testing
- Verify all existing functionality still works
- Test with existing application code
- Validate performance hasn't degraded
- Ensure error handling still works correctly

#### New Functionality Testing
- Test all new features thoroughly
- Validate new parameters work correctly
- Test new error conditions
- Verify new output values

#### Integration Testing
```csharp
[Test]
public async Task UpdatedProcedure_ShouldMaintainBackwardsCompatibility()
{
    // Test with original parameter set
    var originalParams = new Dictionary<string, object>
    {
        ["p_PartID"] = "TEST001"
        // Original parameters only
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "updated_procedure_name",
        originalParams
    );

    Assert.IsTrue(result.Success);
    // Verify original behavior is preserved
}

[Test]
public async Task UpdatedProcedure_ShouldSupportNewFeatures()
{
    // Test with new parameters
    var newParams = new Dictionary<string, object>
    {
        ["p_PartID"] = "TEST001",
        ["p_NewParameter"] = "new_value"
    };

    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "updated_procedure_name",
        newParams
    );

    Assert.IsTrue(result.Success);
    // Verify new functionality works
}
```

## Quality Assurance

### Change Review Checklist
- [ ] Original procedure correctly copied from production
- [ ] Changes are minimal and focused
- [ ] Backwards compatibility maintained
- [ ] Error handling improved or maintained
- [ ] Performance impact analyzed
- [ ] Security implications reviewed
- [ ] Documentation complete and accurate
- [ ] All tests pass (regression + new)

### Performance Review
- [ ] Query execution plans analyzed
- [ ] Index usage verified
- [ ] Resource consumption measured
- [ ] Scalability impact assessed
- [ ] No performance regressions introduced

### Security Review
- [ ] Input validation enhanced
- [ ] SQL injection prevention verified
- [ ] Access control maintained
- [ ] Audit trail preserved
- [ ] Error message security checked

## Application Impact Analysis

### Service Layer Updates
When updating procedures, consider impact on service layer:

```csharp
// Existing service method (may need updates)
public async Task<Result<DataTable>> ExistingOperationAsync(string partId)
{
    // May need parameter additions or changes
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        connectionString,
        "updated_procedure_name",
        new Dictionary<string, object>
        {
            ["p_PartID"] = partId,
            // New parameters may be needed
            ["p_NewParameter"] = defaultValue
        }
    );
    
    return HandleResult(result);
}
```

### UI Impact
- Verify UI still works with updated procedures
- Test any new data returned
- Validate error messages display correctly
- Ensure performance is acceptable

### Integration Points
- Verify external system integrations still work
- Test API endpoints that use updated procedures
- Validate report generation still functions
- Check batch processing operations

## Migration Planning

### Staging Environment Testing
Before production deployment:
1. **Deploy to Staging**: Deploy updated procedure to staging environment
2. **Full Application Testing**: Test complete application with updated procedure
3. **Performance Testing**: Validate performance with production-like data
4. **User Acceptance Testing**: Get approval from stakeholders
5. **Rollback Testing**: Verify rollback plan works if needed

### Production Deployment Strategy
1. **Backup**: Backup production database
2. **Maintenance Window**: Schedule appropriate maintenance window
3. **Deploy**: Replace procedure in production
4. **Verify**: Verify deployment success
5. **Monitor**: Monitor for issues post-deployment
6. **Rollback Plan**: Be ready to rollback if issues occur

### Post-Deployment Verification
- [ ] Procedure executes without errors
- [ ] Application functionality works correctly
- [ ] Performance meets expectations
- [ ] No error log spikes
- [ ] User feedback is positive

## Common Update Patterns

### Adding Output Parameters
```sql
-- Original
OUT p_Status INT

-- Updated (backwards compatible)
OUT p_Status INT,
OUT p_AdditionalInfo VARCHAR(500) -- New output parameter
```

### Adding Optional Input Parameters
```sql
-- Original
IN p_PartID VARCHAR(300)

-- Updated (backwards compatible)
IN p_PartID VARCHAR(300),
IN p_OptionalFilter VARCHAR(100) DEFAULT NULL -- Optional parameter
```

### Improving Error Messages
```sql
-- Original
SET p_ErrorMsg = 'Error occurred';

-- Updated
SET p_ErrorMsg = CONCAT('Error in operation for PartID ', p_PartID, ': ', specific_error_details);
```

### Adding Validation
```sql
-- Original (minimal validation)
BEGIN
    -- Direct operation

-- Updated (enhanced validation)
BEGIN
    -- Input validation
    IF p_PartID IS NULL OR p_PartID = '' THEN
        SET p_Status = -1;
        SET p_ErrorMsg = 'PartID parameter is required';
        LEAVE;
    END IF;
    
    -- Business rule validation
    IF NOT EXISTS (SELECT 1 FROM md_part_ids WHERE PartID = p_PartID) THEN
        SET p_Status = 1;
        SET p_ErrorMsg = CONCAT('PartID not found: ', p_PartID);
        LEAVE;
    END IF;
    
    -- Proceed with operation
```

## Related Documentation

- [New Stored Procedures](README_New_Stored_Procedures.md) - New procedures being developed
- [Development Database Schema](README_Development_Database_Schema.md) - Development database structure
- [Production Stored Procedures](../../Database_Files/README_Existing_Stored_Procedures.md) - Current production procedures
- [Main Development README](README.md) - Development workflow overview

## Best Practices

### Change Management
1. **Small, Focused Changes**: Make minimal changes that address specific issues
2. **Thorough Testing**: Test all aspects of functionality
3. **Clear Documentation**: Document all changes and their rationale
4. **Backwards Compatibility**: Maintain compatibility with existing code
5. **Performance Awareness**: Consider performance impact of all changes

### Version Control
- Track all changes in git with clear commit messages
- Tag releases with version numbers
- Maintain detailed change logs
- Document breaking changes clearly

### Communication
- Notify team of procedure updates
- Coordinate with dependent teams
- Schedule updates to minimize impact
- Provide migration guidance for any breaking changes

This file serves as the development workspace for safely updating existing production stored procedures while maintaining system stability and backwards compatibility.