# Development Database Schema (mtm_wip_application_test)

## Overview
This file contains the complete schema definition for the MTM WIP Application **development** database. This is used for development, testing, and experimentation before changes are promoted to production.

## ?? CRITICAL DATABASE RULE ??
**ALL database operations in application code MUST use stored procedures ONLY.**
- **PROHIBITED**: Direct SQL queries in C# code
- **REQUIRED**: Use `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` pattern

## Database Information
- **Database Name**: `mtm_wip_application_test`
- **Engine**: MySQL 5.7.24+
- **Character Set**: utf8mb4
- **Purpose**: Development and testing environment for MTM WIP Application
- **Production Equivalent**: `mtm_wip_application`

## Related Files
- **Schema File**: `Development_Database_Schema.sql`
- **New Procedures**: `New_Stored_Procedures.sql` (new development procedures)
- **Updated Procedures**: `Updated_Stored_Procedures.sql` (modified existing procedures)
- **Production Schema**: `../../Database_Files/Production_Database_Schema.sql`

## Development vs Production Schema

The development schema is **identical** to the production schema in structure but allows for:
- **Testing new procedures** without affecting production
- **Schema modifications** during development
- **Data experimentation** with test data
- **Performance testing** with realistic data volumes

## Schema Structure

This development database contains the same tables as production:

### Core Inventory Tables
- `inv_inventory` - Current inventory items and quantities
- `inv_transaction` - Complete audit trail of inventory movements
- `inv_inventory_batch_seq` - Batch number sequencing

### Master Data Tables
- `md_part_ids` - Part master data with descriptions and operations
- `md_locations` - Valid storage locations
- `md_operation_numbers` - Valid manufacturing operations
- `md_item_types` - Valid inventory item types

### User Management Tables
- `usr_users` - User accounts and preferences
- `sys_roles` - System roles for access control
- `sys_user_roles` - User role assignments
- `usr_ui_settings` - User interface preferences

### System Tables
- `log_error` - Application error logging
- `log_changelog` - Application version tracking
- `sys_last_10_transactions` - User quick buttons
- `app_themes` - Application theme configurations

## Development Workflow

### 1. Schema Changes
When modifying the database schema during development:

```sql
-- Example: Adding a new column
ALTER TABLE inv_inventory ADD COLUMN new_field VARCHAR(100) NULL;

-- Update this file and the schema SQL file
-- Test thoroughly before promoting to production
```

### 2. Stored Procedure Development
All database operations use stored procedures:

```csharp
// ? CORRECT - Using stored procedure
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Get_ByPartID",
    new Dictionary<string, object> 
    {
        ["p_PartID"] = partId
    }
);

// ? PROHIBITED - Direct SQL
// var query = "SELECT * FROM inv_inventory WHERE PartID = @partId";
```

### 3. Testing Strategy
- **Unit Tests**: Test procedures individually
- **Integration Tests**: Test complete workflows
- **Performance Tests**: Validate with realistic data volumes
- **Data Integrity Tests**: Ensure referential integrity

## Development Environment Setup

### Database Connection
```csharp
// Development connection string
var connectionString = "Server=localhost;Database=mtm_wip_application_test;Uid=dev_user;Pwd=dev_password;";
```

### Test Data Management
- Use realistic test data that mirrors production patterns
- Maintain referential integrity across all tables
- Include edge cases and boundary conditions
- Regular cleanup of test data

## Common Development Operations

### Inventory Management
```csharp
// Add inventory via stored procedure
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    new Dictionary<string, object>
    {
        ["p_PartID"] = "TEST001",
        ["p_Location"] = "DEV-LOC-001",
        ["p_Operation"] = "100",
        ["p_Quantity"] = 50,
        ["p_ItemType"] = "WIP",
        ["p_User"] = "dev_user",
        ["p_Notes"] = "Development test data"
    }
);
```

### Error Logging
```csharp
// Log errors via stored procedure
await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "log_error_Add_Error",
    new Dictionary<string, object>
    {
        ["p_User"] = "dev_user",
        ["p_Severity"] = "Error",
        ["p_ErrorType"] = "Development Test",
        ["p_ErrorMessage"] = "Test error message",
        ["p_ModuleName"] = "TestModule",
        ["p_MethodName"] = "TestMethod",
        ["p_ErrorTime"] = DateTime.Now
    }
);
```

### Master Data Management
```csharp
// Add new part via stored procedure
await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "md_part_ids_Add_Part",
    new Dictionary<string, object>
    {
        ["p_ItemNumber"] = "DEV-PART-001",
        ["p_Customer"] = "Development Customer",
        ["p_Description"] = "Development test part",
        ["p_IssuedBy"] = "dev_user",
        ["p_ItemType"] = "WIP"
    }
);
```

## Data Integrity Rules

### Foreign Key Relationships
All relationships in development mirror production:
- `sys_user_roles.UserID` ? `usr_users.ID`
- `sys_user_roles.RoleID` ? `sys_roles.ID`
- `usr_ui_settings.UserId` ? `usr_users.User`

### Business Logic Constraints
- All inventory changes must create transaction records
- BatchNumber provides complete traceability
- User assignments must reference valid users
- Master data validates all references

## Development Stored Procedures

### New Procedures (in `New_Stored_Procedures.sql`)
New stored procedures being developed for future features.

### Updated Procedures (in `Updated_Stored_Procedures.sql`)
Modified versions of existing production procedures.

### Testing Procedures
Development-specific procedures for testing and data validation.

## Development-Specific Considerations

### Test Data Requirements

#### Sample Users
Development database should include test users representing different roles:
```sql
-- Example test users (DO NOT use in production)
INSERT INTO usr_users (User, `Full Name`, Shift) VALUES 
('dev_admin', 'Development Administrator', '1'),
('test_user1', 'Test User One', '1'),
('test_user2', 'Test User Two', '2'),
('readonly_user', 'Read Only Test User', '1');
```

#### Sample Parts
Include representative part numbers for testing:
```sql
-- Example test parts (DO NOT use in production)
INSERT INTO md_part_ids (PartID, Customer, Description, ItemType) VALUES
('TEST001', 'TestCustomer', 'Test Part Number One', 'WIP'),
('TEST002', 'TestCustomer', 'Test Part Number Two', 'WIP'),
('SAMPLE123', 'SampleCorp', 'Sample Widget Assembly', 'WIP');
```

#### Sample Locations
Include test locations for development work:
```sql
-- Example test locations (DO NOT use in production)
INSERT INTO md_locations (Location, Building, IssuedBy) VALUES
('DEV-001', 'Development Building', 'dev_admin'),
('TEST-AREA', 'Test Building', 'dev_admin'),
('STAGING', 'Staging Area', 'dev_admin');
```

### Testing Scenarios

#### Inventory Flow Testing
The development database should support testing of complete inventory workflows:
1. **Receipt Testing**: Parts entering inventory (IN transactions)
2. **Usage Testing**: Parts leaving inventory (OUT transactions)  
3. **Transfer Testing**: Parts moving between locations (TRANSFER transactions)
4. **Batch Tracking**: Complete traceability through batch numbers

#### User Interface Testing
- Theme testing with various theme configurations
- UI settings persistence and retrieval
- Quick button functionality
- Error message display and logging

#### Error Condition Testing
- Database connection failures
- Invalid data entry
- Constraint violations
- Transaction rollback scenarios

### Database Maintenance

#### Data Reset Procedures
```sql
-- WARNING: Development use only - NEVER run on production
-- Reset all inventory data
TRUNCATE TABLE inv_transaction;
TRUNCATE TABLE inv_inventory;
UPDATE inv_inventory_batch_seq SET last_batch_number = 0, current_match = 0;

-- Reset user-specific data
TRUNCATE TABLE sys_last_10_transactions;
DELETE FROM usr_ui_settings WHERE UserId LIKE 'test_%';

-- Clear error logs
TRUNCATE TABLE log_error;
```

#### Test Data Refresh
Development database should be refreshed with clean test data periodically:
1. Backup current schema structure
2. Clear all data tables
3. Insert fresh test data
4. Verify all functionality works with clean data

### Performance Testing

#### Volume Testing
Development database should include procedures for testing with larger datasets:
- Generate large numbers of inventory records
- Test query performance with substantial transaction history
- Validate index effectiveness with realistic data volumes

#### Concurrent User Testing
- Test multiple simultaneous user sessions
- Validate transaction locking and isolation
- Test batch number sequencing under concurrent load

## Performance Considerations

### Indexing Strategy
Same indexes as production plus development-specific indexes for testing:
- Performance testing indexes
- Development query optimization
- Test data access patterns

### Query Optimization
- All queries through stored procedures
- Parameter validation and sanitization
- Efficient data access patterns
- Minimal locking and blocking

## Migration to Production

### Pre-Migration Checklist
1. **All tests pass** - Unit, integration, and performance tests
2. **Code review complete** - Peer review of all changes
3. **Documentation updated** - README files and code comments
4. **Schema validated** - Structure matches production requirements
5. **Procedures tested** - All stored procedures work correctly
6. **Data integrity verified** - All constraints and relationships valid

### Migration Process
1. **Export procedures** from development files
2. **Update production files** in `../../Database_Files/`
3. **Deploy to staging** environment for final validation
4. **Deploy to production** after staging approval

## Development Guidelines

### Code Standards
- **No direct SQL** - Use stored procedures only
- **Parameter validation** - Validate all inputs in procedures
- **Error handling** - Comprehensive error management
- **Documentation** - Comment all procedures and complex logic

### Testing Requirements
- **Unit tests** for all procedures
- **Integration tests** for workflows
- **Performance tests** for scalability
- **Security tests** for data protection

### Version Control
- Track all schema changes in git
- Tag releases for production deployment
- Maintain change logs for all modifications
- Document breaking changes

## Security Considerations

### Access Control
- Development database isolated from production
- Limited user permissions
- No production data in development
- Secure connection strings

### Development Credentials
- Use separate credentials from production
- Limit network access to development team
- Rotate credentials regularly
- Monitor access logs

### Data Privacy
- Never use production data in development
- Anonymize any data that resembles real information
- Ensure test data doesn't contain sensitive information
- Comply with data protection regulations

## Database Integration

### Development Tools Integration
Development database serves as the testing ground for:
- Schema changes and migrations
- Stored procedure modifications
- Index optimization
- Data type changes

### Continuous Integration
Development database should integrate with CI/CD pipelines:
- Automated schema validation
- Test data population
- Integration test execution
- Performance regression testing

### Synchronization Procedures

#### Schema Sync from Production
When production schema changes:
1. Export production schema structure only (no data)
2. Compare with development schema
3. Apply necessary changes to development
4. Update this documentation
5. Notify development team of changes

#### Testing New Schema Changes
When developing schema changes:
1. Implement and test in development first
2. Document all changes and their impact
3. Create migration scripts
4. Test rollback procedures
5. Schedule production deployment

#### Version Control Integration
- Track all schema changes in version control
- Maintain migration scripts alongside code
- Document breaking changes
- Tag releases with corresponding schema versions

## Related Documentation

- [New Stored Procedures](README_New_Stored_Procedures.md) - Documentation for new procedures
- [Updated Stored Procedures](README_Updated_Stored_Procedures.md) - Documentation for updated procedures
- [Production Database Schema](../../Database_Files/README_Production_Database_Schema.md) - Production schema reference
- [Main Development README](README.md) - Development workflow overview

This development database provides a complete testing environment that mirrors production while allowing for safe experimentation and new feature development.