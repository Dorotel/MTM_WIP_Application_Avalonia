# Services Compliance Reports

This directory contains compliance reports for service layer files.

## Service Layer Standards

Service files must comply with:
- **Architecture**: Proper separation of business logic from UI
- **Async Patterns**: Use async/await for database operations
- **Error Handling**: Integration with Service_ErrorHandler
- **MTM Data Patterns**: Part ID (string), Operation (string numbers)
- **Dependency Injection**: Interface/implementation separation
- **Naming Conventions**: {Name}Service.cs, I{Name}Service.cs

## Current Service Reports

*Reports will be generated here as services are audited*

## Expected Services

Based on MTM WIP Application requirements, these services should exist:
- `IInventoryService.cs` / `InventoryService.cs` - Inventory management
- `ITransactionService.cs` / `TransactionService.cs` - Transaction history
- `IUserService.cs` / `UserService.cs` - User management
- `IDatabaseService.cs` / `DatabaseService.cs` - Data access layer

---

*Generated compliance reports will appear in this directory following the naming convention: {ServiceName}-compliance-report-{YYYY-MM-DD}.md*