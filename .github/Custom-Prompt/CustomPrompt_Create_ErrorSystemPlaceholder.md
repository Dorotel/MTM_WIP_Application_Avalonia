# Enhanced Error System Implementation - Custom Prompt

## Instructions
Use this prompt when you need to implement the enhanced error handling system with structured logging, ReactiveUI integration, and MTM business context support.

## Persona
**Enhanced Error Handling Copilot**  
*(See [../../.github/personas.instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Implement enhanced error handling following the robust .github error system patterns with:
- Structured logging with stored procedure integration
- ReactiveUI ThrownExceptions subscription patterns
- User-friendly error messages with hidden technical details
- MTM business context logging and audit trails
- Result pattern integration for better error flow
- Async error logging with comprehensive fallback mechanisms
```

## Purpose
For implementing the enhanced error handling system that provides enterprise-grade logging, user experience, and business context integration.

## Usage Examples

### Example 1: Service Layer Error Handling
```
Implement enhanced error handling for InventoryService with:
- Structured logging with stored procedure integration
- ReactiveUI ThrownExceptions subscription patterns
- User-friendly error messages with hidden technical details
- MTM business context logging and audit trails
- Result pattern integration for better error flow
- Async error logging with comprehensive fallback mechanisms

Service should handle inventory operations with comprehensive business context logging.
```

### Example 2: ViewModel Error Integration
```
Implement enhanced error handling for InventoryViewModel with:
- Structured logging with stored procedure integration
- ReactiveUI ThrownExceptions subscription patterns
- User-friendly error messages with hidden technical details
- MTM business context logging and audit trails
- Result pattern integration for better error flow
- Async error logging with comprehensive fallback mechanisms

ViewModel should use ReactiveUI extensions for automatic error handling and user notifications.
```

## Guidelines

### Enhanced Error Handler Structure
```csharp
public static class Service_ErrorHandler
{
    // Modern async error handling with business context
    public static async Task HandleErrorAsync(
        Exception exception,
        string operation,
        string userId,
        Dictionary<string, object>? context = null,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
    {
        // Structured logging with stored procedure integration
        // Business context extraction
        // Fallback mechanisms
    }

    // User-friendly messaging
    public static string GetUserFriendlyMessage(Exception ex)
    {
        // Hide technical details
        // Provide actionable guidance
        // Use MTM terminology
    }

    // ReactiveUI integration
    public static bool ShouldShowToUser(Exception ex)
    {
        // Context-appropriate display logic
    }
}
```

### Enhanced Error Categories
- **Validation Errors**: User input validation, business rule violations
- **System Errors**: Database connectivity, file system, network issues
- **Business Errors**: MTM-specific business logic violations  
- **Critical Errors**: Application stability threats, data corruption risks

### Structured Logging Pattern
```csharp
// Enhanced logging with stored procedure integration
public static async Task LogErrorAsync(Exception ex, string operation, string userId, Dictionary<string, object> context)
{
    try
    {
        // Primary: Database logging via stored procedures
        var parameters = new Dictionary<string, object>
        {
            ["ErrorMessage"] = ex.Message,
            ["StackTrace"] = ex.StackTrace,
            ["Operation"] = operation,
            ["UserId"] = userId,
            ["Context"] = JsonSerializer.Serialize(context),
            ["Timestamp"] = DateTime.UtcNow
        };

        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "sys_error_Log_Insert",
            parameters
        );
    }
    catch
    {
        // Fallback: File-based logging
        await LogToFileAsync(ex, operation, userId, context);
    }
}
```

### ReactiveUI Integration Pattern
```csharp
// Enhanced ReactiveUI extensions
public static class ReactiveUIErrorExtensions
{
    public static IDisposable SubscribeToErrorsWithNotification<T>(
        this ReactiveCommand<Unit, T> command,
        string operation,
        string userId,
        Action<string> showError,
        Dictionary<string, object>? context = null)
    {
        return command.ThrownExceptions.Subscribe(async ex =>
        {
            await Service_ErrorHandler.HandleErrorAsync(ex, operation, userId, context);
            
            if (Service_ErrorHandler.ShouldShowToUser(ex))
            {
                var userMessage = Service_ErrorHandler.GetUserFriendlyMessage(ex);
                showError(userMessage);
            }
        });
    }
}
```

### Result Pattern Integration
```csharp
// Enhanced Result<T> with error integration
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; }
    public string? ErrorCode { get; }
    public Exception? Exception { get; }

    public static Result<T> Success(T data) => new(true, data, string.Empty, null, null);
    public static Result<T> Failure(string errorMessage, string? errorCode = null, Exception? exception = null) 
        => new(false, default, errorMessage, errorCode, exception);
}

// MTM-specific extensions
public static class MTMResultExtensions
{
    public static Result<T> FromStoredProcedureResult<T>(object? status, string? message, T? data)
    {
        var statusCode = Convert.ToInt32(status ?? -1);
        return statusCode switch
        {
            0 => Result<T>.Success(data!),
            1 => Result<T>.Warning(data!, message ?? "Operation completed with warnings"),
            _ => Result<T>.Failure(message ?? "Operation failed", statusCode.ToString())
        };
    }
}
```

### Business Context Logging
```csharp
// MTM business context extraction
private static string ExtractBusinessContext(Dictionary<string, object> context)
{
    var businessItems = new List<string>();

    if (context.TryGetValue("PartId", out var partId))
        businessItems.Add($"PartId={partId}");
    if (context.TryGetValue("Operation", out var operation))
        businessItems.Add($"Operation={operation}");
    if (context.TryGetValue("TransactionType", out var transactionType))
        businessItems.Add($"TransactionType={transactionType}");
    if (context.TryGetValue("StoredProcedure", out var storedProcedure))
        businessItems.Add($"StoredProcedure={storedProcedure}");

    return string.Join("; ", businessItems);
}
```

### Enhanced Error Entry
```csharp
public class ErrorEntry
{
    // Standard properties
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public ErrorCategory Category { get; set; }
    public ErrorSeverity Severity { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string StackTrace { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    
    // Enhanced properties
    public string BusinessContext { get; set; } = string.Empty;
    public string AdditionalData { get; set; } = string.Empty;
}
```

### Performance and Audit Logging
```csharp
// Performance monitoring
public static async Task LogPerformanceMetricsAsync(
    string operation,
    TimeSpan duration,
    string userId,
    Dictionary<string, object>? additionalMetrics = null)
{
    // Log performance data for monitoring
}

// Audit trail logging
public static async Task LogAuditTrailAsync(
    string action,
    string userId,
    string entityType,
    string entityId,
    Dictionary<string, object>? beforeState = null,
    Dictionary<string, object>? afterState = null)
{
    // Log business operation audit trail
}
```

### User Experience Integration
```csharp
// User-friendly error messaging
public static class ErrorMessageProvider
{
    public static string GetUserFriendlyMessage(ErrorCategory category)
    {
        return category switch
        {
            ErrorCategory.UI => "A display issue occurred. The application will continue to work normally.",
            ErrorCategory.BusinessLogic => "Please check your input and try again.",
            ErrorCategory.MySQL => "A database issue occurred. Your changes may not have been saved.",
            ErrorCategory.Network => "A network issue occurred. Please check your connection.",
            _ => "An unexpected error occurred. Please try again or contact support."
        };
    }

    public static bool ShouldShowToUser(ErrorSeverity severity, ErrorCategory category)
    {
        return severity switch
        {
            ErrorSeverity.Low => false,
            ErrorSeverity.Medium => category != ErrorCategory.Other,
            ErrorSeverity.High => true,
            ErrorSeverity.Critical => true,
            _ => true
        };
    }
}
```

### Service Layer Pattern
```csharp
public class EnhancedInventoryService
{
    public async Task<Result<InventoryItem>> GetInventoryItemAsync(string partId)
    {
        try
        {
            var context = new Dictionary<string, object>
            {
                ["PartId"] = partId,
                ["Operation"] = "GetInventoryItem",
                ["StoredProcedure"] = "inv_inventory_Get_ByPartID"
            };

            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "inv_inventory_Get_ByPartID",
                parameters
            );

            return MTMResultExtensions.FromStoredProcedureResult(
                result.Status,
                result.Message,
                MapToInventoryItem(result.Data));
        }
        catch (Exception ex)
        {
            await Service_ErrorHandler.HandleErrorAsync(
                ex,
                "GetInventoryItem",
                GetCurrentUserId(),
                new Dictionary<string, object> { ["PartId"] = partId });

            return Result<InventoryItem>.Failure(
                "Unable to retrieve inventory item. Please try again.",
                "SERVICE_ERROR",
                ex);
        }
    }
}
```

### ViewModel Pattern
```csharp
public class EnhancedInventoryViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public EnhancedInventoryViewModel()
    {
        SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
        
        SaveCommand.SubscribeToErrorsWithNotification(
            "SaveInventoryOperation",
            GetCurrentUserId(),
            error => ErrorMessage = error,
            new Dictionary<string, object>
            {
                ["Component"] = "InventoryViewModel",
                ["Action"] = "Save"
            });
    }

    private async Task SaveAsync()
    {
        var result = await _inventoryService.SaveInventoryItemAsync(PartId, Quantity)
            .LogBusinessOperationAsync(
                "SaveInventoryItem",
                GetCurrentUserId(),
                new Dictionary<string, object>
                {
                    ["PartId"] = PartId,
                    ["Quantity"] = Quantity
                });

        result
            .OnSuccess(_ => StatusMessage = "Inventory saved successfully!")
            .OnFailure(error => ErrorMessage = error);
    }
}
```

## Related Files
- `Services/Service_ErrorHandler.cs` - Enhanced error handler implementation
- `Services/LoggingUtility.cs` - Enhanced logging infrastructure with business context
- `Models/ResultPattern.cs` - Result pattern implementation with MTM extensions
- `Documentation/Development/Examples/EnhancedErrorHandlingExample.cs` - Usage examples
- `.github/Development-Instructions/errorhandler.instruction.md` - Complete error handling guidelines

## Technical Requirements
- Must implement structured logging with stored procedure integration
- Must include comprehensive fallback mechanisms (database ? file ? console)
- Must support ReactiveUI ThrownExceptions subscription patterns
- Must provide user-friendly error messages hiding technical details
- Must include MTM business context logging (PartId, Operation, TransactionType, etc.)
- Must integrate with Result pattern for better error flow control
- Must include performance monitoring and audit trail capabilities
- Must follow MTM naming conventions and dependency injection patterns

## Quality Checklist
- [ ] Structured logging with stored procedure integration implemented
- [ ] Comprehensive fallback mechanisms (database ? file ? console) included
- [ ] ReactiveUI error subscription extensions created
- [ ] User-friendly error messaging with technical details hidden
- [ ] MTM business context extraction and logging implemented
- [ ] Result pattern integration with success/failure handling
- [ ] Performance monitoring and audit trail logging included
- [ ] Enhanced error entry with business context fields
- [ ] Service layer error handling patterns implemented
- [ ] ViewModel error handling patterns with ReactiveUI integration
- [ ] Async error logging throughout
- [ ] Thread-safe session cache management
- [ ] Comprehensive usage examples provided