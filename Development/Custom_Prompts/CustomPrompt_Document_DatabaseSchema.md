# Document Database Schema - Custom Prompt

## Instructions
Use this prompt when you need to create comprehensive documentation for database schemas, tables, or relationships.

## Prompt Template

```
Create comprehensive documentation for [DATABASE_ELEMENT] with the following requirements:

**Documentation Target**: [Database/Schema/Table/Relationship]

**Scope**: [Production/Development/Both]

**Documentation Type**: [Complete Schema/Table Details/Relationship Analysis/Migration Guide]

**Requirements**:
1. Follow MTM database documentation standards
2. Include all business rules and constraints
3. Document stored procedure relationships
4. Include security and access control information
5. Provide usage examples and patterns
6. Document data flow and relationships
7. Include troubleshooting information
8. Follow "NO direct SQL" rule - all examples use stored procedures

**Specific Documentation Needs**:

### For Complete Schema Documentation
- Overview of database purpose and scope
- Complete table listing with purposes
- Relationship diagrams and explanations
- Stored procedure catalog
- Security model documentation
- Performance considerations
- Backup and recovery procedures

### For Table Documentation
- Table purpose and business context
- Complete column definitions with types and constraints
- Business rules and validation requirements
- Relationships to other tables
- Stored procedures that access this table
- Indexes and performance considerations
- Data examples and usage patterns

### For Relationship Documentation
- Foreign key relationships
- Business relationship rules
- Data integrity constraints
- Cascade operations
- Stored procedures that manage relationships
- Common relationship patterns

### For Migration Documentation
- Schema differences between environments
- Migration procedures and scripts
- Data transformation requirements
- Rollback procedures
- Validation steps

**Format Requirements**:
- Use clear markdown formatting
- Include code examples using stored procedures only
- Provide visual diagrams where helpful
- Include troubleshooting sections
- Add cross-references to related documentation

**Target Audience**: [Developers/Database Administrators/Business Users/All]

Please provide:
1. Comprehensive documentation following the specified scope
2. Clear explanations suitable for the target audience
3. Practical examples using stored procedures
4. Cross-references to related components
5. Troubleshooting and common issues section
```

## Usage Examples

### Example 1: Complete Production Schema Documentation
```
Create comprehensive documentation for the MTM WIP Application Production Database Schema with the following requirements:

**Documentation Target**: Complete Production Database Schema

**Scope**: Production (mtm_wip_application)

**Documentation Type**: Complete Schema Documentation

**Specific Documentation Needs**:

### Overview Section
- Database purpose: MTM WIP Inventory Management System
- Target users: Manufacturing personnel, supervisors, administrators
- Key business processes supported
- Integration points with other systems

### Table Catalog
- Complete listing of all tables with purposes
- Categorization by functional area (Inventory, Users, Logging, etc.)
- Table size and growth patterns
- Critical vs. reference data classification

### Stored Procedure Catalog
- All production stored procedures grouped by function
- Parameter documentation for each procedure
- Usage examples for common operations
- Performance characteristics and recommendations

### Security Model
- User roles and permissions
- Access control patterns
- Data privacy considerations
- Audit trail requirements

### Business Rules Documentation
- Inventory management rules
- Transaction validation requirements
- User management policies
- Data retention requirements

### Performance and Scaling
- Index strategies
- Query optimization guidelines
- Capacity planning information
- Monitoring recommendations

### Backup and Recovery
- Backup procedures and schedules
- Recovery testing procedures
- Disaster recovery protocols
- Data integrity verification

**Target Audience**: Database Administrators and Senior Developers

[Continue with detailed requirements...]
```

### Example 2: Inventory Tables Deep Dive
```
Create comprehensive documentation for Inventory Management Tables with the following requirements:

**Documentation Target**: Inventory Tables (inv_inventory, inv_transaction, inv_inventory_batch_seq)

**Scope**: Both Production and Development

**Documentation Type**: Table Details with Relationship Analysis

**Specific Documentation Needs**:

### Table Purpose and Context
- Business context for each table
- Role in inventory management workflow
- Data lifecycle and timing
- Integration with other business processes

### Detailed Column Documentation
For each table and column:
- Business meaning and purpose
- Data type rationale and constraints
- Validation rules and business logic
- Default values and auto-generated fields
- Historical data considerations

### Relationship Analysis
- Foreign key relationships between tables
- Business relationship rules
- Data consistency requirements
- Cascade operations and data integrity

### Stored Procedure Integration
- All procedures that access these tables
- Common usage patterns and examples
- Performance considerations
- Error handling patterns

### Data Flow Documentation
- How data enters the system (IN transactions)
- How data flows between locations (TRANSFER)
- How data exits the system (OUT transactions)
- Batch processing and number assignment

### Common Operations
- Adding inventory items
- Removing inventory items
- Transferring between locations
- Batch number management
- Historical reporting

**Target Audience**: Developers and Business Analysts

[Continue with detailed requirements...]
```

### Example 3: Migration Documentation
```
Create comprehensive documentation for Database Migration Procedures with the following requirements:

**Documentation Target**: Migration from Development to Production

**Scope**: Both Development and Production environments

**Documentation Type**: Migration Guide with Procedures

**Specific Documentation Needs**:

### Environment Comparison
- Schema differences between development and production
- Data differences and considerations
- Configuration and connection differences
- Performance characteristics comparison

### Migration Procedures
- Schema migration steps and scripts
- Stored procedure deployment process
- Data migration requirements
- Configuration updates needed

### New Stored Procedures Deployment
- Procedures in Development/Database_Files/New_Stored_Procedures.sql
- Testing requirements before deployment
- Application code updates needed
- Rollback procedures if needed

### Updated Stored Procedures Deployment
- Procedures in Development/Database_Files/Updated_Stored_Procedures.sql
- Impact analysis on existing functionality
- Backwards compatibility verification
- Performance impact assessment

### Validation and Testing
- Post-migration validation steps
- Functionality testing procedures
- Performance verification
- Data integrity checks

### Rollback Planning
- Complete rollback procedures for each type of change
- Emergency rollback procedures
- Data recovery procedures
- Application rollback coordination

**Target Audience**: Database Administrators and DevOps Team

[Continue with detailed requirements...]
```

## Documentation Standards

### Markdown Structure
```markdown
# Database Element Name

## Overview
[Brief description of purpose and scope]

## Database Information
- **Database Name**: [name]
- **Environment**: [Production/Development]
- **Purpose**: [detailed purpose]
- **Owner**: [responsible team/person]

## ?? CRITICAL RULES ??
**ALL database operations must use stored procedures ONLY**
- Include this reminder in all documentation

## [Specific Sections Based on Documentation Type]

### Business Context
[How this fits into business processes]

### Technical Details
[Technical implementation details]

### Usage Examples
[Examples using stored procedures only]

### Related Components
[Cross-references to related elements]

### Troubleshooting
[Common issues and solutions]

## Related Documentation
[Links to related documentation files]
```

### Table Documentation Template
```markdown
### Table: table_name
**Purpose**: [What this table stores/represents]

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| column1 | type | constraints | description |
| column2 | type | constraints | description |

**Business Rules**:
- [Rule 1]
- [Rule 2]

**Related Tables**:
- [Table relationships]

**Stored Procedures**:
- [Procedures that access this table]

**Usage Examples**:
```csharp
// Example using stored procedure
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "procedure_name",
    parameters
);
```
```

### Stored Procedure Documentation Template
```markdown
### Procedure: procedure_name
**Purpose**: [What this procedure does]

**Parameters**:
- `IN p_param1` TYPE - Description
- `OUT p_Status` INT - Status code
- `OUT p_ErrorMsg` VARCHAR(255) - Status message

**Business Rules**:
- [Rule 1]
- [Rule 2]

**Usage Example**:
```csharp
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "procedure_name",
    new Dictionary<string, object>
    {
        ["p_param1"] = value1
    }
);
```

**Related Tables**: [Tables accessed]

**Performance Notes**: [Performance characteristics]
```

## Documentation Categories

### Schema-Level Documentation
- Complete database overview
- Table relationships and data flow
- Security model and access control
- Performance and scaling considerations
- Backup and recovery procedures

### Table-Level Documentation
- Individual table purposes and structures
- Column definitions and constraints
- Business rules and validation
- Relationships and dependencies
- Usage patterns and examples

### Procedure-Level Documentation
- Individual stored procedure functionality
- Parameter definitions and usage
- Business logic and rules
- Performance characteristics
- Integration examples

### Process-Level Documentation
- Business process workflows
- Data lifecycle management
- Integration procedures
- Migration and deployment processes
- Troubleshooting guides

## Quality Standards

### Completeness
- All elements documented thoroughly
- No missing critical information
- Cross-references complete and accurate
- Examples tested and verified

### Clarity
- Clear language appropriate for audience
- Technical terms defined
- Logical organization and flow
- Visual aids where helpful

### Accuracy
- Information verified against actual implementation
- Examples tested with real code
- Cross-references validated
- Version information current

### Maintainability
- Easy to update as system evolves
- Modular structure for partial updates
- Clear ownership and maintenance procedures
- Change tracking and versioning

## Related Files
- Production Schema: `Database_Files/README_Production_Database_Schema.md`
- Development Schema: `Development/Database_Files/README_Development_Database_Schema.md`
- Stored Procedures: Various README files in Database_Files directories
- Migration Guides: Process-specific documentation files