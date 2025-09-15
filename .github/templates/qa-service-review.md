---
name: Service Layer Code Review Checklist
description: 'Quality assurance checklist for Service layer code review in MTM manufacturing context'
applies_to: '**/*Service.cs'
manufacturing_context: true
review_type: 'code'
quality_gate: 'critical'
---

# Service Layer Code Review - Quality Assurance Checklist

## Context
- **Component Type**: Service Layer (.NET 8 with Dependency Injection)
- **Manufacturing Domain**: Inventory Services / Database Services / Configuration Services
- **Quality Gate**: Pre-merge (Critical)
- **Reviewer**: [Name]
- **Review Date**: [Date]

## Dependency Injection Compliance

### Service Registration
- [ ] **Service interface defined** with proper abstraction
- [ ] **Service registration** follows MTM patterns in ServiceCollectionExtensions
- [ ] **Lifetime management** appropriate (Singleton/Scoped/Transient)
- [ ] **Service dependencies** properly injected through constructor
- [ ] **No service locator** anti-patterns used

### Constructor Validation
- [ ] **Constructor parameters** validated with ArgumentNullException.ThrowIfNull()
- [ ] **All dependencies** properly stored in readonly fields
- [ ] **Logging service** included for troubleshooting
- [ ] **Configuration services** injected for settings access
- [ ] **No circular dependencies** between services

## Database Integration Patterns

### Stored Procedure Usage
- [ ] **Only stored procedures used** (no direct SQL queries)
- [ ] **Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()** used for all database calls
- [ ] **MySqlParameter arrays** properly constructed with parameter names
- [ ] **Database result status** properly checked (1 = success, 0/-1 = error)
- [ ] **Connection strings** accessed through configuration services

### Transaction Management
- [ ] **Database transactions** properly scoped
- [ ] **Error handling** includes rollback logic where appropriate
- [ ] **Connection management** follows using statement patterns
- [ ] **Async patterns** used consistently (await ConfigureAwait(false) in library code)
- [ ] **Database timeouts** configured appropriately for manufacturing operations

## Error Handling and Logging

### Exception Management
- [ ] **All public methods** wrapped in try-catch blocks
- [ ] **Service exceptions** properly logged with context
- [ ] **Business exceptions** vs system exceptions properly differentiated
- [ ] **Error propagation** appropriate for caller context
- [ ] **No swallowed exceptions** without logging

### Logging Standards
- [ ] **Structured logging** with Microsoft.Extensions.Logging
- [ ] **Log levels** appropriate (Information/Warning/Error)
- [ ] **Manufacturing context** included in log messages
- [ ] **Performance logging** for slow operations
- [ ] **Security sensitive data** not logged (connection strings, etc.)

## Manufacturing Domain Logic

### Business Rules
- [ ] **Manufacturing workflows** properly implemented
- [ ] **Inventory operations** follow MTM business rules
- [ ] **Transaction types** correctly determined by user intent
- [ ] **Part ID validation** enforces manufacturing standards
- [ ] **Operation sequences** validated against manufacturing workflow

### Data Validation
- [ ] **Input validation** comprehensive for manufacturing data
- [ ] **Business rule validation** enforced before database operations
- [ ] **Cross-service data consistency** maintained
- [ ] **Manufacturing constraints** properly enforced
- [ ] **Master data validation** against manufacturing standards

## Performance and Scalability

### Performance Patterns
- [ ] **Async/await** used consistently for I/O operations
- [ ] **ConfigureAwait(false)** used in library code
- [ ] **Connection pooling** properly configured
- [ ] **Batch operations** implemented for high-volume scenarios
- [ ] **Caching strategies** implemented where appropriate

### Memory Management
- [ ] **IDisposable** implemented where appropriate
- [ ] **Using statements** for disposable resources
- [ ] **Large object handling** optimized for manufacturing data volumes
- [ ] **Collection management** prevents memory leaks
- [ ] **Background operations** properly cancelled

## Service Integration

### Cross-Service Communication
- [ ] **Service boundaries** properly defined and respected
- [ ] **Data transfer objects** used for cross-service communication
- [ ] **Service coupling** minimized through proper abstraction
- [ ] **Event-driven patterns** used where appropriate
- [ ] **Messaging patterns** follow MTM standards

### Configuration Management
- [ ] **Configuration access** through IConfiguration or IOptions
- [ ] **Environment-specific settings** properly handled
- [ ] **Configuration validation** implemented
- [ ] **Hot-reload support** where appropriate
- [ ] **Manufacturing settings** properly typed and validated

## Testing Requirements

### Unit Test Coverage
- [ ] **Public methods** have unit tests
- [ ] **Error scenarios** tested with appropriate exceptions
- [ ] **Mock dependencies** properly configured
- [ ] **Database operations** tested with mocked database service
- [ ] **Manufacturing scenarios** covered with domain-specific tests

### Integration Testing
- [ ] **Database integration** tested with real stored procedures
- [ ] **Cross-service integration** tested
- [ ] **Configuration integration** tested
- [ ] **Error recovery scenarios** tested
- [ ] **Performance benchmarks** established for manufacturing operations

## Manufacturing Compliance

### Manufacturing Standards
- [ ] **Inventory accuracy** maintained through service operations
- [ ] **Audit trail** properly maintained for manufacturing operations
- [ ] **Transaction integrity** preserved across service calls
- [ ] **Master data consistency** enforced
- [ ] **Manufacturing workflow compliance** validated

### Operational Requirements
- [ ] **High availability** considerations for manufacturing operations
- [ ] **Data backup/recovery** considerations
- [ ] **Performance monitoring** capabilities
- [ ] **Scalability** for manufacturing volume requirements
- [ ] **Security** appropriate for manufacturing data

## Code Quality Standards

### Code Organization
- [ ] **Single responsibility principle** followed
- [ ] **Method complexity** kept reasonable
- [ ] **Service interfaces** properly abstracted
- [ ] **Code documentation** adequate for manufacturing domain
- [ ] **Naming conventions** clear for manufacturing concepts

### Architecture Patterns
- [ ] **Service layer patterns** consistently applied
- [ ] **Repository pattern** used where appropriate
- [ ] **Factory pattern** used for complex object creation
- [ ] **Strategy pattern** used for business rule variations
- [ ] **Command pattern** used for operation encapsulation

## Sign-off

- [ ] **Developer Self-Review**: _________________ - _________
- [ ] **Peer Code Review**: _________________ - _________  
- [ ] **Manufacturing Domain Review**: _________________ - _________
- [ ] **Architecture Review**: _________________ - _________
- [ ] **Quality Gate Approval**: _________________ - _________

## Review Notes

### Issues Identified
[Document any issues found during review]

### Architecture Recommendations
[Document architectural improvement suggestions]

### Performance Considerations
[Document performance-related feedback]

### Manufacturing Domain Feedback
[Document feedback specific to manufacturing workflows and requirements]

---

**Review Status**: [ ] Approved [ ] Approved with Comments [ ] Requires Changes  
**Architecture Compliance**: [ ] Validated [ ] Needs Review  
**Manufacturing Domain Validation**: [ ] Complete [ ] Pending