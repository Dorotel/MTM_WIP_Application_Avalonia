---
name: Database Operations Code Review Checklist
description: 'Quality assurance checklist for MySQL database operations in MTM manufacturing context'
applies_to: '**/*Service.cs,**/*Repository.cs'
manufacturing_context: true
review_type: 'code'
quality_gate: 'critical'
---

# Database Operations Code Review - Quality Assurance Checklist

## Context
- **Component Type**: Database Operations (MySQL 9.4.0 with Stored Procedures)
- **Manufacturing Domain**: Inventory Database / Transaction Recording / Master Data Management
- **Quality Gate**: Pre-merge (Critical)
- **Reviewer**: [Name]
- **Review Date**: [Date]

## Stored Procedure Compliance (MANDATORY)

### Database Access Patterns
- [ ] **ONLY stored procedures used** (no direct SQL queries allowed)
- [ ] **Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()** used exclusively
- [ ] **No string concatenation** for SQL construction
- [ ] **No MySqlCommand** used directly (bypass Helper prohibited)
- [ ] **No Entity Framework** or other ORM patterns (not used in MTM)

### Parameter Construction
- [ ] **MySqlParameter arrays** properly constructed
- [ ] **Parameter names** match stored procedure parameters exactly (p_PartID, p_OperationNumber, etc.)
- [ ] **Parameter data types** appropriate for values
- [ ] **Null handling** proper for optional parameters
- [ ] **Parameter validation** before database calls

### Stored Procedure Usage
- [ ] **Correct procedure names** used (inv_inventory_Add_Item, md_part_ids_Get_All, etc.)
- [ ] **Result status checking** implemented (1 = success, 0/-1 = error)
- [ ] **DataTable processing** proper for returned data
- [ ] **Error message handling** from stored procedure results
- [ ] **Return value validation** appropriate for operation type

## Manufacturing Database Integration

### MTM Business Logic
- [ ] **Manufacturing workflows** properly supported in database calls
- [ ] **Transaction types** correctly mapped (IN/OUT/TRANSFER based on user intent)
- [ ] **Operation numbers** used as workflow steps (90, 100, 110, 120)
- [ ] **Part ID validation** enforced before database operations
- [ ] **Inventory constraints** validated (no negative quantities)

### Database Transaction Management
- [ ] **Transaction scope** appropriate for business operations
- [ ] **Rollback logic** implemented for failed operations
- [ ] **Multi-step operations** properly coordinated
- [ ] **Data consistency** maintained across related tables
- [ ] **Concurrency handling** appropriate for manufacturing operations

## Connection Management

### Connection String Handling
- [ ] **Connection strings** accessed through configuration service
- [ ] **No hardcoded connections** in code
- [ ] **Environment-specific connections** properly configured
- [ ] **Connection string security** maintained
- [ ] **Backup connection** handling where applicable

### Performance Optimization
- [ ] **Connection pooling** properly configured
- [ ] **Connection lifetime** managed appropriately
- [ ] **Using statements** for disposable database resources
- [ ] **Async patterns** used consistently (await ConfigureAwait(false))
- [ ] **Query timeout** configured for manufacturing operations

## Error Handling and Resilience

### Database Error Management
- [ ] **MySqlException** handling implemented
- [ ] **Connection failure** recovery implemented
- [ ] **Timeout handling** appropriate for manufacturing operations
- [ ] **Deadlock detection** and retry logic implemented
- [ ] **Database constraint violations** properly handled

### Retry and Circuit Breaker Patterns
- [ ] **Transient error retry** implemented where appropriate
- [ ] **Exponential backoff** for retry operations
- [ ] **Circuit breaker** pattern for database availability
- [ ] **Fallback strategies** for critical manufacturing operations
- [ ] **Error logging** with sufficient context for troubleshooting

## Manufacturing Data Integrity

### Data Validation
- [ ] **Input validation** before database calls
- [ ] **Business rule validation** enforced
- [ ] **Cross-table consistency** maintained
- [ ] **Master data validation** against manufacturing standards
- [ ] **Audit trail** properly maintained

### Manufacturing Constraints
- [ ] **Inventory accuracy** preserved through all operations
- [ ] **Transaction atomicity** maintained for manufacturing operations
- [ ] **Manufacturing workflow compliance** enforced
- [ ] **Part traceability** maintained through database operations
- [ ] **Quality data** integrity preserved

## Performance and Scalability

### Query Performance
- [ ] **Stored procedure efficiency** validated
- [ ] **Parameter indexing** considered in stored procedure design
- [ ] **Large dataset handling** optimized
- [ ] **Batch operations** implemented for high-volume scenarios
- [ ] **Performance monitoring** capabilities included

### Manufacturing Load Considerations
- [ ] **High-frequency operations** optimized (inventory updates)
- [ ] **Concurrent user handling** appropriate for manufacturing shifts
- [ ] **Peak load scenarios** considered (shift changes)
- [ ] **Data archiving** strategy considered for historical data
- [ ] **Backup/restore** operations don't impact manufacturing

## Security and Compliance

### Database Security
- [ ] **SQL injection** prevention validated (stored procedures only)
- [ ] **Parameter validation** prevents malicious input
- [ ] **Connection security** appropriate for manufacturing environment
- [ ] **Audit logging** sufficient for manufacturing compliance
- [ ] **Data access patterns** follow least privilege principle

### Manufacturing Compliance
- [ ] **Audit trail** complete for manufacturing transactions
- [ ] **Data retention** policies followed
- [ ] **Traceability** maintained for manufacturing parts
- [ ] **Change tracking** implemented where required
- [ ] **Regulatory compliance** considerations addressed

## Testing Requirements

### Database Integration Testing
- [ ] **Stored procedure testing** with real database
- [ ] **Parameter validation** testing
- [ ] **Error scenario** testing (connection failures, constraint violations)
- [ ] **Performance testing** under manufacturing load
- [ ] **Concurrency testing** for multi-user scenarios

### Manufacturing Scenario Testing
- [ ] **Complete manufacturing workflows** tested end-to-end
- [ ] **Data consistency** verified across operations
- [ ] **Error recovery** tested with manufacturing data
- [ ] **Performance benchmarks** established for manufacturing operations
- [ ] **Cross-platform database compatibility** validated

## Code Quality

### Code Organization
- [ ] **Database operations** properly abstracted in service layer
- [ ] **Connection management** centralized and reusable
- [ ] **Error handling** consistent across all database operations
- [ ] **Logging** sufficient for manufacturing troubleshooting
- [ ] **Code documentation** adequate for manufacturing database operations

### Architecture Compliance
- [ ] **Repository pattern** used where appropriate
- [ ] **Unit of work** pattern implemented for complex operations
- [ ] **Service layer** properly abstracts database operations
- [ ] **Dependency injection** used for database service access
- [ ] **Configuration management** proper for database settings

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **Database Review**: _________________ - _________
- [ ] **Manufacturing Domain Review**: _________________ - _________
- [ ] **Performance Review**: _________________ - _________
- [ ] **Security Review**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Review Notes

### Database Performance
[Document performance considerations and optimizations]

### Manufacturing Integration
[Document manufacturing-specific database requirements and validation]

### Security Considerations
[Document security-related observations and recommendations]

### Compliance Notes
[Document regulatory and manufacturing compliance considerations]

---

**Review Status**: [ ] Approved [ ] Approved with Comments [ ] Requires Changes  
**Database Performance**: [ ] Validated [ ] Needs Optimization  
**Manufacturing Compliance**: [ ] Complete [ ] Pending  
**Security Validation**: [ ] Complete [ ] Needs Review