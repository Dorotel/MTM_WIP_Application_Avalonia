# Implement Result Pattern System - Custom Prompt

## Instructions
Use this prompt when you need to create the Result<T> pattern infrastructure for consistent service responses.

## Persona
**Data Modeling Copilot + Application Logic Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Create the Result<T> pattern infrastructure for MTM WIP Application following .NET 8 patterns.  
Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators.  
Include static factory methods for Success(T) and Failure(string), proper equality comparison,  
and integration patterns for async service methods. Follow MTM error handling conventions and  
ensure compatibility with ReactiveUI command error handling patterns.
```

## Purpose
For creating the Result<T> pattern infrastructure for consistent service responses.

## Usage Examples

### Example 1: Basic Result Pattern Implementation
```
Create the Result<T> pattern infrastructure for MTM WIP Application following .NET 8 patterns.  
Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators.  
Include static factory methods for Success(T) and Failure(string), proper equality comparison,  
and integration patterns for async service methods. Follow MTM error handling conventions and  
ensure compatibility with ReactiveUI command error handling patterns.
```

### Example 2: Extended Result Pattern with Validation
```
Create the Result<T> pattern infrastructure for MTM WIP Application with validation support.  
Implement Models/Result.cs and Models/ValidationResult.cs with Success/Failure states, error collections,  
and implicit operators. Include static factory methods and integration with FluentValidation.  
Follow MTM error handling conventions and ensure ReactiveUI compatibility.
```

## Guidelines

### Core Result<T> Implementation
```csharp
namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Represents the result of an operation that can either succeed or fail
/// </summary>
/// <typeparam name="T">The type of the success value</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string ErrorMessage { get; }
    public Exception? Exception { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorMessage = string.Empty;
        Exception = null;
    }

    private Result(string errorMessage, Exception? exception = null)
    {
        IsSuccess = false;
        Value = default;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result with the given value
    /// </summary>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Creates a failed result with the given error message
    /// </summary>
    public static Result<T> Failure(string errorMessage) => new(errorMessage);

    /// <summary>
    /// Creates a failed result with the given error message and exception
    /// </summary>
    public static Result<T> Failure(string errorMessage, Exception exception) => new(errorMessage, exception);

    /// <summary>
    /// Implicit conversion from T to Result<T>
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicit conversion from string to Result<T> (failure)
    /// </summary>
    public static implicit operator Result<T>(string errorMessage) => Failure(errorMessage);
}
```

### Non-Generic Result for Void Operations
```csharp
/// <summary>
/// Represents the result of an operation that doesn't return a value
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public Exception? Exception { get; }

    private Result(bool isSuccess, string errorMessage = "", Exception? exception = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result Success() => new(true);

    /// <summary>
    /// Creates a failed result with the given error message
    /// </summary>
    public static Result Failure(string errorMessage) => new(false, errorMessage);

    /// <summary>
    /// Creates a failed result with the given error message and exception
    /// </summary>
    public static Result Failure(string errorMessage, Exception exception) => new(false, errorMessage, exception);

    /// <summary>
    /// Implicit conversion from bool to Result
    /// </summary>
    public static implicit operator Result(bool success) => success ? Success() : Failure("Operation failed");

    /// <summary>
    /// Implicit conversion from string to Result (failure)
    /// </summary>
    public static implicit operator Result(string errorMessage) => Failure(errorMessage);
}
```

### Extension Methods for Result
```csharp
/// <summary>
/// Extension methods for Result types
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps a successful result to a new type
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value!))
            : Result<TOut>.Failure(result.ErrorMessage, result.Exception);
    }

    /// <summary>
    /// Binds a result to another operation that returns a result
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value!)
            : Result<TOut>.Failure(result.ErrorMessage, result.Exception);
    }

    /// <summary>
    /// Executes an action if the result is successful
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess && result.Value is not null)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Executes an action if the result is a failure
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
    {
        if (result.IsFailure)
        {
            action(result.ErrorMessage);
        }
        return result;
    }
}
```

### Service Integration Pattern
```csharp
// Example service method using Result pattern
public interface IInventoryService
{
    Task<Result<IEnumerable<InventoryItem>>> GetInventoryAsync(string partId, CancellationToken cancellationToken = default);
    Task<Result<InventoryItem>> CreateInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task<Result> UpdateInventoryAsync(string partId, int quantity, CancellationToken cancellationToken = default);
    Task<Result> DeleteInventoryAsync(string partId, CancellationToken cancellationToken = default);
}

// Example implementation
public class InventoryService : IInventoryService
{
    public async Task<Result<IEnumerable<InventoryItem>>> GetInventoryAsync(string partId, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement via stored procedure
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "sp_Inventory_Get_ByPartId",
                new Dictionary<string, object> { ["PartId"] = partId }
            );

            if (result.Success)
            {
                // TODO: Map DataTable to InventoryItem collection
                var items = new List<InventoryItem>();
                return Result<IEnumerable<InventoryItem>>.Success(items);
            }
            else
            {
                return Result<IEnumerable<InventoryItem>>.Failure(result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<InventoryItem>>.Failure("Failed to retrieve inventory", ex);
        }
    }
}
```

### ReactiveUI Integration Pattern
```csharp
public class InventoryViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    
    public ReactiveCommand<Unit, Unit> LoadInventoryCommand { get; }
    
    public InventoryViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
        
        LoadInventoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await _inventoryService.GetInventoryAsync(SelectedPartId);
            
            if (result.IsSuccess)
            {
                InventoryItems.Clear();
                foreach (var item in result.Value!)
                {
                    InventoryItems.Add(item);
                }
                StatusMessage = "Inventory loaded successfully";
            }
            else
            {
                StatusMessage = $"Failed to load inventory: {result.ErrorMessage}";
                
                // Log error via Service_ErrorHandler
                if (result.Exception is not null)
                {
                    Service_ErrorHandler.HandleException(
                        result.Exception,
                        ErrorSeverity.Medium,
                        "InventoryViewModel_LoadInventory",
                        new Dictionary<string, object> { ["PartId"] = SelectedPartId }
                    );
                }
            }
        });

        // Error handling for unexpected exceptions
        LoadInventoryCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                StatusMessage = "An unexpected error occurred while loading inventory";
                Service_ErrorHandler.HandleException(
                    ex,
                    ErrorSeverity.High,
                    "InventoryViewModel_LoadInventory_UnexpectedException"
                );
            });
    }
}
```

### Validation Result Pattern
```csharp
/// <summary>
/// Specialized result for validation operations
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }

    private ValidationResult(bool isValid, IEnumerable<string> errors)
    {
        IsValid = isValid;
        Errors = errors.ToList().AsReadOnly();
    }

    public static ValidationResult Success() => new(true, Array.Empty<string>());
    
    public static ValidationResult Failure(params string[] errors) => new(false, errors);
    
    public static ValidationResult Failure(IEnumerable<string> errors) => new(false, errors);

    /// <summary>
    /// Combines multiple validation results
    /// </summary>
    public static ValidationResult Combine(params ValidationResult[] results)
    {
        var allErrors = results.SelectMany(r => r.Errors).ToList();
        return allErrors.Any() ? Failure(allErrors) : Success();
    }
}
```

### MTM-Specific Result Extensions
```csharp
/// <summary>
/// MTM-specific extensions for Result pattern
/// </summary>
public static class MTMResultExtensions
{
    /// <summary>
    /// Validates MTM Part ID format and returns Result
    /// </summary>
    public static Result<string> ValidatePartId(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
            return Result<string>.Failure("Part ID cannot be empty");
            
        if (partId.Length > 50)
            return Result<string>.Failure("Part ID cannot exceed 50 characters");
            
        // TODO: Add MTM-specific Part ID validation rules
        
        return Result<string>.Success(partId);
    }

    /// <summary>
    /// Validates MTM Operation number format and returns Result
    /// </summary>
    public static Result<string> ValidateOperation(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
            return Result<string>.Failure("Operation cannot be empty");
            
        if (!operation.All(char.IsDigit))
            return Result<string>.Failure("Operation must be numeric");
            
        if (!int.TryParse(operation, out int operationNumber) || operationNumber <= 0)
            return Result<string>.Failure("Operation must be a positive number");
            
        return Result<string>.Success(operation);
    }

    /// <summary>
    /// Validates MTM Quantity and returns Result
    /// </summary>
    public static Result<int> ValidateQuantity(int quantity)
    {
        if (quantity < 0)
            return Result<int>.Failure("Quantity cannot be negative");
            
        if (quantity > 999999)
            return Result<int>.Failure("Quantity cannot exceed 999,999");
            
        return Result<int>.Success(quantity);
    }
}
```

## Integration Requirements

### Error Handling Integration
- Must integrate with existing `Service_ErrorHandler`
- Must support MTM error categorization
- Must provide user-friendly error messages
- Must include exception details for logging

### ReactiveUI Integration
- Must work with `ReactiveCommand` error handling
- Must support async operations with cancellation
- Must integrate with progress reporting
- Must support validation scenarios

### MTM Business Logic Integration
- Must support MTM data validation patterns
- Must handle MTM-specific error scenarios
- Must integrate with stored procedure results
- Must support transaction patterns

## Related Files
- `Models/` - Data models namespace
- `Services/` - Service layer implementations
- `Services/Service_ErrorHandler.cs` - Error handling integration
- `.github/copilot-instructions.md` - MTM business logic rules

## Quality Checklist
- [ ] Generic Result<T> implemented correctly
- [ ] Non-generic Result implemented for void operations
- [ ] Static factory methods included
- [ ] Implicit operators implemented
- [ ] Extension methods for functional composition
- [ ] ReactiveUI integration patterns included
- [ ] MTM-specific validation extensions
- [ ] Error handling integration
- [ ] Comprehensive XML documentation
- [ ] Unit test considerations included