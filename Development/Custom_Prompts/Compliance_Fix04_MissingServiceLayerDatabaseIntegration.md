# Custom Prompt: Create Missing Service Layer Database Integration

## 🚨 CRITICAL PRIORITY FIX #4

**Issue**: No service layer exists to abstract stored procedure calls and provide standardized database result processing.

**When you complete this task**
1. Update all relevant instruction.md files to reflect changes
1. Update all relevant Readme.md files to reflect changes
2. Update all relevant HTML documentation to reflect changes

**Files Affected**:
- Missing `Services/IInventoryService.cs`
- Missing `Services/IUserService.cs`
- Missing `Services/ITransactionService.cs`
- No centralized database result processing

**Priority**: 🚨 **CRITICAL - ARCHITECTURE FOUNDATION**

---

## Custom Prompt

```
CRITICAL ARCHITECTURE IMPLEMENTATION: Create comprehensive service layer with database integration to abstract stored procedure calls and provide standardized result processing.

REQUIREMENTS:
1. Create all missing service interfaces and implementations
2. Implement services using Helper_Database_StoredProcedure pattern
3. Add Result<T> pattern for consistent response handling
4. Implement comprehensive error logging in services
5. Register services in dependency injection container
6. Follow MTM business logic rules for transaction types
7. Provide standardized database result processing

SERVICE INTERFACES TO CREATE:

1. **IInventoryService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IInventoryService
{
    // CRITICAL: TransactionType determined by USER INTENT, not Operation number
    Task<Result<InventoryItem>> AddStockAsync(string partId, string operation, int quantity, string location, string userId);
    Task<Result<InventoryItem>> RemoveStockAsync(string partId, string operation, int quantity, string location, string userId);
    Task<Result<InventoryTransaction>> TransferStockAsync(string partId, string operation, int quantity, string fromLocation, string toLocation, string userId);
    
    Task<Result<IEnumerable<InventoryItem>>> GetInventoryByPartIdAsync(string partId);
    Task<Result<IEnumerable<InventoryItem>>> GetInventoryByLocationAsync(string location);
    Task<Result<IEnumerable<InventoryItem>>> GetInventoryByOperationAsync(string operation);
    Task<Result<IEnumerable<InventoryItem>>> GetAllInventoryAsync();
    
    Task<Result<bool>> ValidateStockAvailableAsync(string partId, string location, int requiredQuantity);
    Task<Result<int>> GetCurrentStockLevelAsync(string partId, string location);
}
```

2. **IUserService.cs**:
```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IUserService
{
    Task<Result<User>> GetUserByIdAsync(string userId);
    Task<Result<IEnumerable<User>>> GetAllUsersAsync();
    Task<Result<bool>> ValidateUserExistsAsync(string userId);
    Task<Result<IEnumerable<string>>> GetUserPermissionsAsync(string userId);
}
```

3. **ITransactionService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

public interface ITransactionService
{
    Task<Result<InventoryTransaction>> LogTransactionAsync(InventoryTransaction transaction);
    Task<Result<IEnumerable<InventoryTransaction>>> GetTransactionHistoryAsync(string partId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Result<IEnumerable<InventoryTransaction>>> GetUserTransactionHistoryAsync(string userId, int count = 10);
    Task<Result<IEnumerable<InventoryTransaction>>> GetLocationTransactionHistoryAsync(string location, DateTime? fromDate = null, DateTime? toDate = null);
}
```

4. **IDatabaseService.cs**:
```csharp
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IDatabaseService
{
    Task<Result<DataTable>> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters);
    Task<Result<T>> ExecuteStoredProcedureAsync<T>(string procedureName, Dictionary<string, object> parameters, Func<DataTable, T> mapper);
    Task<Result<IEnumerable<T>>> ExecuteStoredProcedureListAsync<T>(string procedureName, Dictionary<string, object> parameters, Func<DataRow, T> mapper);
}
```

SERVICE IMPLEMENTATIONS:

**InventoryService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Database_Engine;

namespace MTM_WIP_Application_Avalonia.Services;

public class InventoryService : IInventoryService
{
    private readonly IDatabaseService _databaseService;
    private readonly ITransactionService _transactionService;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(
        IDatabaseService databaseService,
        ITransactionService transactionService,
        ILogger<InventoryService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // CRITICAL: TransactionType based on USER INTENT, NOT Operation
    public async Task<Result<InventoryItem>> AddStockAsync(string partId, string operation, int quantity, string location, string userId)
    {
        try
        {
            _logger.LogInformation("Adding stock: PartId={PartId}, Operation={Operation}, Quantity={Quantity}, Location={Location}, User={UserId}", 
                partId, operation, quantity, location, userId);

            // Validate inputs
            if (string.IsNullOrWhiteSpace(partId))
                return Result<InventoryItem>.Failure("PartId is required");
            
            if (quantity <= 0)
                return Result<InventoryItem>.Failure("Quantity must be positive");

            // Execute stored procedure with enhanced error handling
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_Operation"] = operation, // Just a workflow step identifier
                ["p_Quantity"] = quantity,
                ["p_Location"] = location,
                ["p_User"] = userId
            };

            var result = await _databaseService.ExecuteStoredProcedureAsync("inv_inventory_Add_Item_Enhanced", parameters);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to add stock: {Error}", result.ErrorMessage);
                return Result<InventoryItem>.Failure($"Failed to add inventory: {result.ErrorMessage}");
            }

            // Log transaction - TransactionType.IN because user is ADDING stock
            var transaction = new InventoryTransaction
            {
                PartId = partId,
                Operation = operation, // Workflow step, NOT transaction type indicator
                TransactionType = TransactionType.IN, // USER INTENT: Adding stock
                Quantity = quantity,
                Location = location,
                UserId = userId,
                Timestamp = DateTime.Now
            };

            await _transactionService.LogTransactionAsync(transaction);

            // Create result object
            var inventoryItem = new InventoryItem
            {
                PartId = partId,
                Operation = operation,
                Quantity = quantity,
                Location = location,
                LastModified = DateTime.Now,
                LastModifiedBy = userId
            };

            _logger.LogInformation("Successfully added stock for PartId={PartId}", partId);
            return Result<InventoryItem>.Success(inventoryItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding stock for PartId={PartId}", partId);
            return Result<InventoryItem>.Failure($"Unexpected error adding stock: {ex.Message}");
        }
    }

    public async Task<Result<InventoryItem>> RemoveStockAsync(string partId, string operation, int quantity, string location, string userId)
    {
        try
        {
            _logger.LogInformation("Removing stock: PartId={PartId}, Operation={Operation}, Quantity={Quantity}, Location={Location}, User={UserId}", 
                partId, operation, quantity, location, userId);

            // Validate sufficient stock exists
            var stockCheck = await ValidateStockAvailableAsync(partId, location, quantity);
            if (!stockCheck.IsSuccess)
                return Result<InventoryItem>.Failure($"Insufficient stock: {stockCheck.ErrorMessage}");

            if (!stockCheck.Data)
                return Result<InventoryItem>.Failure($"Insufficient stock available. Required: {quantity}");

            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_Operation"] = operation,
                ["p_Quantity"] = quantity,
                ["p_Location"] = location,
                ["p_User"] = userId
            };

            var result = await _databaseService.ExecuteStoredProcedureAsync("inv_inventory_Remove_Item_Enhanced", parameters);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to remove stock: {Error}", result.ErrorMessage);
                return Result<InventoryItem>.Failure($"Failed to remove inventory: {result.ErrorMessage}");
            }

            // Log transaction - TransactionType.OUT because user is REMOVING stock
            var transaction = new InventoryTransaction
            {
                PartId = partId,
                Operation = operation, // Workflow step, NOT transaction type indicator
                TransactionType = TransactionType.OUT, // USER INTENT: Removing stock
                Quantity = quantity,
                Location = location,
                UserId = userId,
                Timestamp = DateTime.Now
            };

            await _transactionService.LogTransactionAsync(transaction);

            var inventoryItem = new InventoryItem
            {
                PartId = partId,
                Operation = operation,
                Quantity = -quantity, // Negative to indicate removal
                Location = location,
                LastModified = DateTime.Now,
                LastModifiedBy = userId
            };

            _logger.LogInformation("Successfully removed stock for PartId={PartId}", partId);
            return Result<InventoryItem>.Success(inventoryItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing stock for PartId={PartId}", partId);
            return Result<InventoryItem>.Failure($"Unexpected error removing stock: {ex.Message}");
        }
    }

    public async Task<Result<InventoryTransaction>> TransferStockAsync(string partId, string operation, int quantity, string fromLocation, string toLocation, string userId)
    {
        try
        {
            _logger.LogInformation("Transferring stock: PartId={PartId}, Operation={Operation}, Quantity={Quantity}, From={FromLocation}, To={ToLocation}, User={UserId}", 
                partId, operation, quantity, fromLocation, toLocation, userId);

            // Validate sufficient stock at source location
            var stockCheck = await ValidateStockAvailableAsync(partId, fromLocation, quantity);
            if (!stockCheck.IsSuccess || !stockCheck.Data)
                return Result<InventoryTransaction>.Failure("Insufficient stock at source location");

            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId,
                ["p_Operation"] = operation,
                ["p_Quantity"] = quantity,
                ["p_FromLocation"] = fromLocation,
                ["p_ToLocation"] = toLocation,
                ["p_User"] = userId
            };

            var result = await _databaseService.ExecuteStoredProcedureAsync("inv_inventory_Transfer_Item_New", parameters);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Failed to transfer stock: {Error}", result.ErrorMessage);
                return Result<InventoryTransaction>.Failure($"Failed to transfer inventory: {result.ErrorMessage}");
            }

            // Log transaction - TransactionType.TRANSFER because user is MOVING stock
            var transaction = new InventoryTransaction
            {
                PartId = partId,
                Operation = operation, // Workflow step, NOT transaction type indicator
                TransactionType = TransactionType.TRANSFER, // USER INTENT: Moving stock
                Quantity = quantity,
                Location = fromLocation,
                ToLocation = toLocation,
                UserId = userId,
                Timestamp = DateTime.Now
            };

            await _transactionService.LogTransactionAsync(transaction);

            _logger.LogInformation("Successfully transferred stock for PartId={PartId}", partId);
            return Result<InventoryTransaction>.Success(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring stock for PartId={PartId}", partId);
            return Result<InventoryTransaction>.Failure($"Unexpected error transferring stock: {ex.Message}");
        }
    }

    // Additional methods...
}
```

**DatabaseService.cs**:
```csharp
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Database_Engine;

namespace MTM_WIP_Application_Avalonia.Services;

public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<DataTable>> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> parameters)
    {
        try
        {
            _logger.LogDebug("Executing stored procedure: {ProcedureName}", procedureName);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                procedureName,
                parameters ?? new Dictionary<string, object>()
            );

            if (result.Status != 0)
            {
                _logger.LogWarning("Stored procedure {ProcedureName} returned status {Status}: {Message}", 
                    procedureName, result.Status, result.ErrorMessage);
                return Result<DataTable>.Failure(result.ErrorMessage ?? "Unknown database error");
            }

            _logger.LogDebug("Successfully executed stored procedure: {ProcedureName}", procedureName);
            return Result<DataTable>.Success(result.DataTable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {ProcedureName}", procedureName);
            return Result<DataTable>.Failure($"Database error: {ex.Message}");
        }
    }

    public async Task<Result<T>> ExecuteStoredProcedureAsync<T>(string procedureName, Dictionary<string, object> parameters, Func<DataTable, T> mapper)
    {
        var result = await ExecuteStoredProcedureAsync(procedureName, parameters);
        
        if (!result.IsSuccess)
            return Result<T>.Failure(result.ErrorMessage);

        try
        {
            var mappedResult = mapper(result.Data);
            return Result<T>.Success(mappedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping result from stored procedure: {ProcedureName}", procedureName);
            return Result<T>.Failure($"Error mapping database result: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<T>>> ExecuteStoredProcedureListAsync<T>(string procedureName, Dictionary<string, object> parameters, Func<DataRow, T> mapper)
    {
        var result = await ExecuteStoredProcedureAsync(procedureName, parameters);
        
        if (!result.IsSuccess)
            return Result<IEnumerable<T>>.Failure(result.ErrorMessage);

        try
        {
            var mappedResults = result.Data.AsEnumerable().Select(mapper).ToList();
            return Result<IEnumerable<T>>.Success(mappedResults);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping results from stored procedure: {ProcedureName}", procedureName);
            return Result<IEnumerable<T>>.Failure($"Error mapping database results: {ex.Message}");
        }
    }
}
```

DEPENDENCY INJECTION REGISTRATION:
In Program.cs or App.axaml.cs:
```csharp
services.AddScoped<IDatabaseService, DatabaseService>();
services.AddScoped<IInventoryService, InventoryService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ITransactionService, TransactionService>();
```

MTM BUSINESS LOGIC COMPLIANCE:
1. **TransactionType Determination**: ALWAYS based on user intent, NEVER on operation number
2. **Operation Numbers**: Treated as workflow step identifiers only ("90", "100", "110")
3. **Error Handling**: Comprehensive logging and user-friendly error messages
4. **Transaction Logging**: All inventory changes must be logged with correct TransactionType
5. **Validation**: All inputs validated before database operations

RESULT PATTERN INTEGRATION:
- Use Result<T> for all service method returns
- Provide detailed error messages for debugging
- Log all operations for audit trail
- Handle database errors gracefully
- Return user-friendly error messages

After implementing service layer, create Development/Services/README_ServiceLayer.md documenting:
- Service responsibilities and interfaces
- Database integration patterns
- Error handling strategies
- Transaction type business rules
- Dependency injection setup
```

---

## Expected Deliverables

1. **Services/IInventoryService.cs** - Complete interface for inventory operations
2. **Services/InventoryService.cs** - Full implementation with MTM business rules
3. **Services/IUserService.cs** - Interface for user management
4. **Services/UserService.cs** - Implementation with validation
5. **Services/ITransactionService.cs** - Interface for transaction logging
6. **Services/TransactionService.cs** - Implementation with history tracking
7. **Services/IDatabaseService.cs** - Interface for database operations
8. **Services/DatabaseService.cs** - Implementation using Helper_Database_StoredProcedure
9. **Dependency injection setup** in Program.cs or App.axaml.cs
10. **Comprehensive error handling** and logging throughout

---

## Validation Steps

1. Verify all service interfaces compile and follow naming conventions
2. Test service implementations with mock database calls
3. Confirm Result<T> pattern is used consistently
4. Validate MTM business rules are properly implemented
5. Test error handling scenarios and logging
6. Verify dependency injection registration works
7. Confirm TransactionType logic follows user intent rules

---

## Success Criteria

- [ ] All service interfaces and implementations created
- [ ] Database integration using Helper_Database_StoredProcedure pattern
- [ ] Result<T> pattern implemented consistently
- [ ] MTM business rules followed (TransactionType by intent)
- [ ] Comprehensive error handling and logging
- [ ] Dependency injection properly configured
- [ ] Ready for ViewModel integration
- [ ] All database operations abstracted through services
- [ ] Transaction logging implemented correctly

---

*Priority: CRITICAL - Foundation for all business logic and database operations.*