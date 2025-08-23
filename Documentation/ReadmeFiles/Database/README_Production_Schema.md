# Production Database Files

This directory contains **production-ready** database files for the MTM WIP Application.

## ?? CRITICAL RULES

### **NO HARD-CODED MYSQL COMMANDS**
All database operations in the application **MUST** use stored procedures only. Zero hard-coded SQL commands are allowed in application code.

### **READ-ONLY PRODUCTION FILES**
- Files in this directory represent the **current production state**
- **DO NOT EDIT** these files directly
- For modifications, use the development workflow described below

## File Structure

### Core Production Files
- `Production_Database_Schema.sql` - Current production database structure
- `Existing_Stored_Procedures.sql` - **READ ONLY** - All current production stored procedures
- `README_*.md` files - Documentation for production database components

## Development Workflow

### Making Changes to Database
1. **New Stored Procedures**: Add to `Development/Database_Files/New_Stored_Procedures.sql`
2. **Updating Existing Procedures**: 
   - Copy from `Existing_Stored_Procedures.sql` to `Development/Database_Files/Updated_Stored_Procedures.sql`
   - Make modifications in the development file
3. **Schema Changes**: Update `Development/Database_Files/Development_Database_Schema.sql`

### File Organization Rules
```
Database_Files/ (Production - READ ONLY)
??? Production_Database_Schema.sql           # Current production schema
??? Existing_Stored_Procedures.sql          # READ ONLY - Current production procedures
??? README_*.md files                       # Production documentation

Development/Database_Files/ (Development - EDITABLE)
??? Development_Database_Schema.sql         # Development schema changes
??? New_Stored_Procedures.sql              # New procedures for development
??? Updated_Stored_Procedures.sql          # Modified versions of existing procedures
??? README_*.md files                      # Development documentation
```

### Deployment Process
1. **Development Phase**: Work in `Development/Database_Files/`
2. **Testing Phase**: Validate all procedures in development environment
3. **Production Deployment**: Copy validated files to `Database_Files/` (production)
4. **Update Production**: Replace production files with tested versions

## Database Access Pattern

### CORRECT - Using Stored Procedures
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "sp_inventory_Get_ByPartID",
    new Dictionary<string, object> 
    {
        ["PartID"] = partId,
        ["UserID"] = userId
    }
);
```

### PROHIBITED - Direct SQL
```csharp
// ? NEVER DO THIS - Direct SQL is forbidden
var query = "SELECT * FROM inventory WHERE part_id = @partId";
```

## Security Requirements

### Stored Procedure Standards
- **Parameter Validation**: All inputs must be validated within stored procedures
- **SQL Injection Prevention**: Use parameterized procedures only
- **Error Handling**: Comprehensive error handling in all procedures
- **Audit Logging**: Log all database modifications
- **Performance Monitoring**: Include performance tracking

### Access Control
- **Role-Based Access**: Procedures must implement appropriate access controls
- **User Authentication**: Verify user permissions before data access
- **Sensitive Data**: Sanitize error messages to prevent information leakage

## Performance Guidelines

### Query Optimization
- **Indexed Queries**: Ensure all frequent queries use appropriate indexes
- **Batch Operations**: Use batch processing for large data sets
- **Transaction Management**: Implement proper transaction boundaries
- **Connection Pooling**: Efficient connection management

### Monitoring
- **Execution Time**: Monitor and optimize slow queries
- **Resource Usage**: Track memory and CPU usage
- **Error Rates**: Monitor and alert on database errors

## Documentation Standards

Each SQL file should include:
- **Purpose**: What the procedure/schema accomplishes
- **Parameters**: Input parameter descriptions and validation rules
- **Returns**: Output format and error codes
- **Dependencies**: Related procedures and tables
- **Version History**: Change log with dates and authors
- **Performance Notes**: Optimization considerations

## Related Documentation
- [Development Database Files](../Development/Database_Files/README.md) - Development workflow details
- [Database Operations Prompts](../Docs/database-operations-prompts.md) - Copilot prompts for database work
- [Copilot Instructions](../.github/copilot-instructions.md) - Complete development guidelines

## Contact
For questions about database operations or to request changes, refer to the development team guidelines in the project documentation.