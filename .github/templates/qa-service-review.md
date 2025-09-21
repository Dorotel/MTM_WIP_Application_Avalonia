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
- [ ] **Service registration** follows MTM patterns in ServiceCollectionExtensions.cs
- [ ] **Lifetime management** appropriate:
  - **Singleton**: ConfigurationService, ThemeService, NavigationService, SettingsService
  - **Scoped**: DatabaseService, MasterDataService, InventoryEditingService
  - **Transient**: StartupDialogService, ViewModels
- [ ] **Service dependencies** properly injected through constructor
- [ ] **No service locator** anti-patterns used (no `Program.GetService<T>()` in services)

### Constructor Validation

- [ ] **Constructor parameters** validated with `ArgumentNullException.ThrowIfNull(param)`
- [ ] **All dependencies** properly stored in readonly fields
- [ ] **ILogger injection** included for troubleshooting and monitoring (`ILogger<ServiceName>`)
- [ ] **IConfigurationService** injected for settings/connection string access
- [ ] **No circular dependencies** between services
- [ ] **Constructor injection** preferred over property injection

## Current MTM Service Architecture

### Core Service Categories

- [ ] **Configuration Services**: ConfigurationService, ApplicationStateService
- [ ] **Data Services**: DatabaseService, MasterDataService, InventoryEditingService, RemoveService
- [ ] **UI Services**: ThemeService, NavigationService, SuggestionOverlayService, VirtualPanelManager
- [ ] **State Services**: SettingsService, SettingsPanelStateManager, StartupDialogService
- [ ] **Utility Services**: QuickButtonsService, FocusManagementService, FilePathService, FileSelectionService
- [ ] **Error Services**: ErrorHandling (static class), FileLoggingService

### Service Implementation Patterns

- [ ] **Interface/Implementation pairs** for all services
- [ ] **ServiceResult pattern** used for operation results
- [ ] **Async/await** patterns properly implemented
- [ ] **Memory caching** implemented for master data (5-minute expiration)
- [ ] **Event-driven patterns** where appropriate (PropertyChanged, custom events)

## Database Integration Patterns

### Stored Procedure Usage

- [ ] **Only stored procedures used** (no direct SQL queries)
- [ ] **IDatabaseService dependency** injected for all database operations
- [ ] **DatabaseService.ExecuteStoredProcedureAsync()** used for all database calls
- [ ] **MySqlParameter arrays** properly constructed with correct parameter names
- [ ] **ServiceResult pattern** used for return values (`ServiceResult<DataTable>`)
- [ ] **Database result status** properly checked (result.Status == 1 for success)
- [ ] **Connection strings** accessed through IConfigurationService (never hardcoded)

### Transaction Management

- [ ] **Database operations** use service layer abstraction (no direct MySqlConnection)
- [ ] **Error handling** includes proper service result patterns
- [ ] **Async patterns** used consistently (`await` with `ConfigureAwait(false)` in services)
- [ ] **Operation timeouts** handled gracefully
- [ ] **Connection lifecycle** managed by DatabaseService
- [ ] **Resource disposal** handled by service layer

## Error Handling and Logging

### Exception Management

- [ ] **All public methods** wrapped in try-catch blocks
- [ ] **Centralized error handling** via `Services.ErrorHandling.HandleErrorAsync()`
- [ ] **Service exceptions** properly logged with manufacturing context
- [ ] **Business exceptions** vs system exceptions properly differentiated
- [ ] **ServiceResult pattern** used for operation results instead of exceptions
- [ ] **No swallowed exceptions** without proper logging
- [ ] **Database errors** handled gracefully with user-friendly messages

### Logging Standards

- [ ] **Microsoft.Extensions.Logging** used throughout (`ILogger<TService>`)
- [ ] **Log levels** appropriate:
  - **Debug**: Method entry/exit, detailed flow
  - **Information**: Successful operations, business events
  - **Warning**: Recoverable issues, business rule violations
  - **Error**: Exceptions, failed operations
- [ ] **Manufacturing context** included in log messages (PartID, Operation, User)
- [ ] **Performance logging** for operations > 1 second
- [ ] **Security sensitive data** not logged (connection strings, passwords)
- [ ] **Structured logging** with meaningful parameters

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
