# Create Error System Placeholder - Custom Prompt

## Instructions
Use this prompt when you need to scaffold a new error handling class or module with standard conventions.

## Persona
**Error Handling Specialist Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Scaffold a new error handler class or module according to our error_handler-instruction.md.  
Include methods and properties for logging errors, but do not implement business logic yet.
```

## Purpose
For scaffolding a new error handling class or module with standard conventions.

## Usage Examples

### Example 1: New Error Category Handler
```
Scaffold a new error handler class for NetworkErrorHandler according to our error_handler-instruction.md.  
Include methods and properties for logging errors, but do not implement business logic yet.
```

### Example 2: Custom Error Logger
```
Scaffold a new error handler class for UIValidationErrorHandler according to our error_handler-instruction.md.  
Include methods and properties for logging errors, but do not implement business logic yet.
```

## Guidelines

### Error Handler Structure
```csharp
public class CustomErrorHandler
{
    // TODO: Implement error logging to file server
    public async Task LogToFileServerAsync(ErrorEntry error)
    {
        // TODO: Log to CSV file per user/category
    }

    // TODO: Implement error logging to MySQL
    public async Task LogToMySqlAsync(ErrorEntry error)
    {
        // TODO: Log to database table
    }

    // TODO: Implement session-level duplicate detection
    public bool IsDuplicateError(ErrorEntry error)
    {
        // TODO: Check if error already logged this session
        return false;
    }
}
```

### Standard Error Categories
- **UI**: User interface errors and validation issues
- **Business Logic**: Domain logic and processing errors
- **MySQL**: Database connection and query errors
- **Network**: Connectivity and communication errors
- **Other**: Miscellaneous system errors

### Error Entry Properties
```csharp
public class ErrorEntry
{
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string MachineName { get; set; } = string.Empty;
    public ErrorCategory Category { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public int LineNumber { get; set; }
    public string StackTrace { get; set; } = string.Empty;
    public ErrorSeverity Severity { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}
```

### Error Severity Levels
```csharp
public enum ErrorSeverity
{
    Low,        // Information, non-critical issues
    Medium,     // Warning, needs attention
    High,       // Error, impacts functionality
    Critical    // System failure, immediate action required
}
```

### File Server Logging Pattern
```csharp
// TODO: User-specific subfolder creation
private async Task EnsureUserFolderExistsAsync(string userId)
{
    // TODO: Create user subfolder if it doesn't exist
}

// TODO: CSV file per error category
private string GetCsvFileName(string userId, ErrorCategory category)
{
    return category switch
    {
        ErrorCategory.UI => $"ui_errors.csv",
        ErrorCategory.BusinessLogic => $"business_logic_errors.csv",
        ErrorCategory.MySQL => $"mysql_errors.csv",
        ErrorCategory.Network => $"network_errors.csv",
        ErrorCategory.Other => $"other_errors.csv",
        _ => $"unknown_errors.csv"
    };
}
```

### MySQL Logging Pattern
```csharp
// TODO: Table per error category with same structure
private string GetTableName(ErrorCategory category)
{
    return category switch
    {
        ErrorCategory.UI => "ui_errors",
        ErrorCategory.BusinessLogic => "business_logic_errors", 
        ErrorCategory.MySQL => "mysql_errors",
        ErrorCategory.Network => "network_errors",
        ErrorCategory.Other => "other_errors",
        _ => "unknown_errors"
    };
}
```

### Duplicate Detection Pattern
```csharp
// TODO: Session-level tracking
private readonly HashSet<string> _sessionErrors = new();

private string GenerateErrorHash(ErrorEntry error)
{
    // TODO: Create unique hash for duplicate detection
    return $"{error.UserId}_{error.Category}_{error.ErrorMessage}_{error.MethodName}";
}
```

### Configuration Integration
```csharp
// TODO: Load from ErrorHandlingConfiguration
public void Initialize()
{
    // TODO: Read from Config/appsettings.json ErrorHandling section
    // - EnableFileServerLogging
    // - EnableMySqlLogging
    // - EnableConsoleLogging
    // - FileServerBasePath
    // - MySqlConnectionString
}
```

### ReactiveUI Integration Pattern
```csharp
// TODO: Subscribe to command exceptions
public void AttachToCommand(ReactiveCommand<Unit, Unit> command, string source)
{
    command.ThrownExceptions
        .Subscribe(ex =>
        {
            // TODO: Log exception with context
            var errorEntry = new ErrorEntry
            {
                // TODO: Populate error details
            };
        });
}
```

## Related Files
- `.github/errorhandler.instruction.md` - Error handling requirements
- `Services/Service_ErrorHandler.cs` - Main error handler implementation
- `Services/LoggingUtility.cs` - Logging infrastructure
- `Config/appsettings.json` - Configuration settings

## Technical Requirements
- Must implement both file server and MySQL logging
- Must include session-level duplicate detection
- Must support all MTM error categories
- Must integrate with existing error handling infrastructure
- Must follow MTM naming conventions
- Must include comprehensive TODO comments for implementation

## Quality Checklist
- [ ] Error categories properly defined
- [ ] File server logging structure included
- [ ] MySQL logging structure included
- [ ] Duplicate detection mechanism included
- [ ] Configuration loading prepared
- [ ] ReactiveUI integration patterns included
- [ ] Comprehensive TODO comments added
- [ ] Error severity levels implemented
- [ ] User-specific folder logic included
- [ ] CSV file naming conventions followed