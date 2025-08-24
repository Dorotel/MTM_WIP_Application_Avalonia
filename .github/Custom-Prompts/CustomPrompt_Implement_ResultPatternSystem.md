# Custom Prompt: Implement Result Pattern System

## ?? **Instructions**
Use this prompt when you need to create the Result<T> pattern infrastructure for consistent service responses, error handling, and success/failure state management throughout the MTM application.

## ?? **Persona**
**Architecture Copilot** - Specializes in implementing foundational architectural patterns, including Result patterns, error handling infrastructure, and service response standardization.

## ?? **Prompt Template**

```
Act as Architecture Copilot. Implement the Result<T> pattern system for [SCOPE] with the following requirements:

**Implementation Scope:** [Service layer | Data access | Business logic | Application-wide]
**Result Types:** [Result<T> for data | Result for operations | Custom result types]
**Error Integration:** [Exception handling | Validation errors | Business rule violations]
**Success Patterns:** [Data return | Operation confirmation | Status reporting]

**Pattern Requirements:**
- Create generic Result<T> class for data operations
- Implement Result class for operation confirmations
- Support success/failure state management
- Include comprehensive error information
- Provide fluent API for result creation and handling
- Support async operations and Task<Result<T>>

**Technical Implementation:**
- Generate result classes with proper error encapsulation
- Create factory methods for success and failure scenarios
- Implement implicit conversions for ease of use
- Include serialization support for API responses
- Support result chaining and composition
- Prepare for dependency injection integration

**MTM-Specific Integration:**
- Support MTM business rule validation results
- Include inventory transaction result patterns
- Handle database operation results with stored procedure context
- Support user operation tracking and audit trails
- Integrate with error logging and reporting systems

**Additional Context:** [Any specific result scenarios or integration requirements]
```

## ?? **Purpose**
This prompt generates a comprehensive Result<T> pattern implementation that standardizes service responses, provides consistent error handling, and maintains type safety throughout the MTM application architecture.

## ?? **Usage Examples**

### **Example 1: Service Layer Result Implementation**
```
Act as Architecture Copilot. Implement the Result<T> pattern system for ServiceLayer with the following requirements:

**Implementation Scope:** Service layer
**Result Types:** Result<T> for data operations, Result for confirmation operations
**Error Integration:** Exception handling and business rule violations
**Success Patterns:** Data return with success confirmation

**Pattern Requirements:**
- Create generic Result<T> class for data operations
- Implement Result class for operation confirmations
- Support success/failure state management
- Include comprehensive error information
- Provide fluent API for result creation and handling
- Support async operations and Task<Result<T>>

**Technical Implementation:**
- Generate result classes with proper error encapsulation
- Create factory methods for success and failure scenarios
- Implement implicit conversions for ease of use
- Include serialization support for API responses
- Support result chaining and composition
- Prepare for dependency injection integration

**MTM-Specific Integration:**
- Support MTM business rule validation results
- Include inventory transaction result patterns
- Handle database operation results with stored procedure context
- Support user operation tracking and audit trails
- Integrate with error logging and reporting systems

**Additional Context:** Services handle inventory operations and need consistent result patterns for success/failure scenarios with detailed error information
```

### **Example 2: Database Operation Results**
```
Act as Architecture Copilot. Implement the Result<T> pattern system for DatabaseOperations with the following requirements:

**Implementation Scope:** Data access layer
**Result Types:** Result<T> for query results, Result for command operations
**Error Integration:** Database exceptions and stored procedure error handling
**Success Patterns:** Data retrieval confirmation and operation status reporting

**Pattern Requirements:**
- Create generic Result<T> class for data operations
- Implement Result class for operation confirmations
- Support success/failure state management
- Include comprehensive error information
- Provide fluent API for result creation and handling
- Support async operations and Task<Result<T>>

**Technical Implementation:**
- Generate result classes with proper error encapsulation
- Create factory methods for success and failure scenarios
- Implement implicit conversions for ease of use
- Include serialization support for API responses
- Support result chaining and composition
- Prepare for dependency injection integration

**MTM-Specific Integration:**
- Support MTM business rule validation results
- Include inventory transaction result patterns
- Handle database operation results with stored procedure context
- Support user operation tracking and audit trails
- Integrate with error logging and reporting systems

**Additional Context:** Database operations through stored procedures need consistent result handling with error details and success confirmation
```

## ?? **Guidelines**

### **Result Pattern Structure**
```csharp
// Generic Result<T> for data operations
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Data { get; }
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
    public Exception Exception { get; }

    private Result(T data)
    {
        IsSuccess = true;
        Data = data;
    }

    private Result(string errorMessage, string errorCode = null, Exception exception = null)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Exception = exception;
    }

    public static Result<T> Success(T data) => new Result<T>(data);
    public static Result<T> Failure(string errorMessage, string errorCode = null, Exception exception = null) 
        => new Result<T>(errorMessage, errorCode, exception);
}

// Non-generic Result for operations
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
    public Exception Exception { get; }

    public static Result Success() => new Result { IsSuccess = true };
    public static Result Failure(string errorMessage, string errorCode = null, Exception exception = null) 
        => new Result { IsSuccess = false, ErrorMessage = errorMessage, ErrorCode = errorCode, Exception = exception };
}
```

### **Service Integration Pattern**
```csharp
public async Task<Result<List<InventoryItem>>> GetInventoryAsync(string partId)
{
    try
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "inv_inventory_Get_ByPartID",
            new Dictionary<string, object> { ["PartID"] = partId }
        );

        if (result.IsSuccess && result.Data != null)
        {
            var items = DataTableToInventoryItems(result.Data);
            return Result<List<InventoryItem>>.Success(items);
        }

        return Result<List<InventoryItem>>.Failure(result.ErrorMessage ?? "Unknown database error", "DB_ERROR");
    }
    catch (Exception ex)
    {
        await LogErrorAsync(ex, nameof(GetInventoryAsync));
        return Result<List<InventoryItem>>.Failure($"Database operation failed: {ex.Message}", "DB_EXCEPTION", ex);
    }
}
```

### **ViewModel Usage Pattern**
```csharp
private async Task LoadInventoryAsync()
{
    IsLoading = true;
    var result = await _inventoryService.GetInventoryAsync(SelectedPartId);
    
    if (result.IsSuccess)
    {
        Inventory.Clear();
        foreach (var item in result.Data)
        {
            Inventory.Add(new InventoryItemViewModel(item));
        }
        ErrorMessage = string.Empty;
    }
    else
    {
        ErrorMessage = result.ErrorMessage;
        await _errorHandler.LogErrorAsync(result.Exception, nameof(LoadInventoryAsync), GetCurrentUserId());
    }
    
    IsLoading = false;
}
```

### **Fluent Extensions**
```csharp
public static class ResultExtensions
{
    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> mapper)
    {
        var result = await resultTask;
        return result.IsSuccess 
            ? Result<TOut>.Success(mapper(result.Data))
            : Result<TOut>.Failure(result.ErrorMessage, result.ErrorCode, result.Exception);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        return result.IsSuccess 
            ? await binder(result.Data)
            : Result<TOut>.Failure(result.ErrorMessage, result.ErrorCode, result.Exception);
    }
}
```

## ?? **Related Files**
- [../Core-Instructions/codingconventions.instruction.md](../Core-Instructions/codingconventions.instruction.md) - Architecture patterns and conventions
- [../Development-Instructions/database-patterns.instruction.md](../Development-Instructions/database-patterns.instruction.md) - Database result handling patterns
- [../Development-Instructions/errorhandler.instruction.md](../Development-Instructions/errorhandler.instruction.md) - Error handling integration
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - Architecture Copilot persona details

## ? **Quality Checklist**

### **Pattern Implementation**
- [ ] Generic Result<T> class for data operations
- [ ] Non-generic Result class for operation confirmations
- [ ] Proper success/failure state management
- [ ] Comprehensive error information encapsulation
- [ ] Factory methods for result creation

### **API Design**
- [ ] Fluent API for result handling
- [ ] Implicit conversions where appropriate
- [ ] Extension methods for result composition
- [ ] Async operation support with Task<Result<T>>
- [ ] Serialization support for API responses

### **MTM Integration**
- [ ] Business rule validation result support
- [ ] Inventory transaction result patterns
- [ ] Database operation result handling
- [ ] Error logging and audit trail integration
- [ ] User operation tracking capabilities

### **Code Quality**
- [ ] Thread-safe implementation
- [ ] Memory-efficient design
- [ ] Proper exception handling
- [ ] Comprehensive unit test support
- [ ] Documentation and usage examples