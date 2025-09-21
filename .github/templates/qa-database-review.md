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
- [ ] **DatabaseService dependency injection** used properly via IDatabaseService

### Parameter Construction

- [ ] **MySqlParameter arrays** properly constructed
- [ ] **Parameter names** match stored procedure parameters exactly (p_PartID, p_OperationNumber, etc.)
- [ ] **Parameter data types** appropriate for values (`MySqlDbType` used correctly)
- [ ] **Null handling** proper for optional parameters (`DBNull.Value` when needed)
- [ ] **Parameter validation** before database calls
- [ ] **DateTime parameters** use proper format and time zone handling

### Current Stored Procedure Usage

- [ ] **Inventory procedures**: `inv_inventory_Add_Item`, `inv_inventory_Remove_Item`, `inv_inventory_Get_ByPartID`
- [ ] **Transaction procedures**: `inv_transaction_Add`, `inv_transaction_Get_History`
- [ ] **Master Data procedures**: `md_part_ids_Get_All`, `md_locations_Get_All`, `md_operation_numbers_Get_All`
- [ ] **QuickButtons procedures**: `qb_quickbuttons_Get_ByUser`, `qb_quickbuttons_Save`
- [ ] **Error Logging procedures**: `log_error_Add_Error`, `log_error_Get_All`
- [ ] **System procedures**: `sys_health_check`
- [ ] **Result status checking** implemented (Status = 1 success with data returned, Status = 0 success with no data returned, Status = -1 error)
- [ ] **DataTable processing** proper for returned data
- [ ] **Error message handling** from stored procedure results

## Service Architecture Patterns

### DatabaseService Integration

- [ ] **IDatabaseService injection** used in service constructors
- [ ] **ServiceResult pattern** used for database operation returns (`ServiceResult<DataTable>`)
- [ ] **Connection string** accessed via IConfigurationService (not hardcoded)
- [ ] **Async/await patterns** properly implemented throughout
- [ ] **ConfigureAwait(false)** used in library/service code
- [ ] **Transaction support** considered for multi-step operations

### Error Handling Integration

- [ ] **Centralized error handling** via Services.ErrorHandling.HandleErrorAsync()
- [ ] **Database exceptions** properly caught and handled
- [ ] **Error logging** to database via log_error_Add_Error procedure
- [ ] **User-friendly error messages** returned to ViewModels
- [ ] **Connection failures** handled gracefully
- [ ] **Timeout handling** implemented for long operations

### Caching Strategy (Master Data)

- [ ] **IMemoryCache injection** for master data services
- [ ] **Cache keys** consistent and well-defined
- [ ] **Cache expiration** appropriate (5 minutes for master data)
- [ ] **Cache invalidation** on data modifications
- [ ] **Cache miss handling** proper fallback to database

## Manufacturing Database Integration

### MTM Business Logic

- [ ] **Manufacturing workflows** properly supported in database calls
- [ ] **Transaction types** correctly mapped (IN/OUT/TRANSFER based on user intent)
- [ ] **Part tracking** through all manufacturing operations
- [ ] **Location management** integrated with warehouse operations
- [ ] **Operation numbers** linked to manufacturing process steps
- [ ] **User tracking** for all manufacturing transactions
- [ ] **Timestamp handling** for manufacturing audit trail

### Manufacturing Data Validation

- [ ] **Part ID validation** against master data
- [ ] **Operation number validation** against valid operations
- [ ] **Location validation** against active locations
- [ ] **Quantity validation** (positive numbers, decimal handling)
- [ ] **Business rule enforcement** at database level
- [ ] **Referential integrity** maintained across manufacturing data

## Current Service Implementation Patterns

### DatabaseService Implementation

```csharp
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfigurationService configurationService, ILogger<DatabaseService> logger)
    {
        _connectionString = configurationService.GetConnectionString();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ServiceResult<DataTable>> ExecuteStoredProcedureAsync(
        string procedureName, MySqlParameter[] parameters)
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, procedureName, parameters);
            
        return result.Status == 1 
            ? ServiceResult<DataTable>.Success(result.Data)
            : ServiceResult<DataTable>.Failure(result.Message);
    }
}
```

### Master Data Service Pattern

```csharp
public class MasterDataService : IMasterDataService
{
    private readonly IDatabaseService _databaseService;
    private readonly IMemoryCache _cache;
    
    public async Task<List<string>> GetPartIdsAsync()
    {
        const string cacheKey = "master_data_part_ids";
        
        if (_cache.TryGetValue(cacheKey, out List<string> cachedPartIds))
            return cachedPartIds;

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "md_part_ids_Get_All", Array.Empty<MySqlParameter>());

        if (result.IsSuccess)
        {
            var partIds = ExtractPartIds(result.Data);
            _cache.Set(cacheKey, partIds, TimeSpan.FromMinutes(5));
            return partIds;
        }
        
        return new List<string>();
    }
}
```

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
