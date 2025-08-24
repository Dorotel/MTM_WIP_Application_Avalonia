# Custom Prompt: Create Error System Placeholder

## ?? **Instructions**
Use this prompt when you need to scaffold error handling classes with standard conventions, including centralized error logging, user-friendly error display, and integration with ReactiveUI error handling patterns.

## ?? **Persona**
**Error Handling Copilot** - Specializes in creating comprehensive error handling systems with centralized logging, user-friendly error display, and proper integration with ReactiveUI and MTM patterns.

## ?? **Prompt Template**

```
Act as Error Handling Copilot. Create error handling system scaffolding for [COMPONENT_NAME] with the following requirements:

**Component Type:** [Service | ViewModel | View | System-wide]
**Error Scope:** [Database operations | UI interactions | Business logic | All operations]
**Logging Level:** [Information | Warning | Error | Critical]
**User Display:** [Silent logging | Toast notifications | Modal dialogs | Inline messages]

**Error Handling Requirements:**
- Create centralized error logging with database and file logging support
- Implement user-friendly error messages with technical details hidden
- Integrate with ReactiveUI ThrownExceptions subscription patterns
- Support error categorization (validation, system, business, critical)
- Include error recovery mechanisms where appropriate
- Provide audit trail capabilities for error tracking

**Technical Implementation:**
- Generate error handling classes with proper naming conventions
- Create error message templates with MTM branding
- Implement async error logging with stored procedure integration
- Include error code classification system
- Support structured logging with contextual information
- Prepare for dependency injection integration

**MTM-Specific Requirements:**
- Log all database operation errors with stored procedure details
- Track inventory transaction errors with business context
- Include user ID and operation context in error logs
- Support MTM business rule validation error handling
- Integrate with application state management for error recovery

**Additional Context:** [Any specific error scenarios or integration requirements]
```

## ?? **Purpose**
This prompt generates comprehensive error handling scaffolding that provides centralized logging, user-friendly error display, and proper integration with MTM business operations and ReactiveUI patterns.

## ?? **Usage Examples**

### **Example 1: Service Layer Error Handling**
```
Act as Error Handling Copilot. Create error handling system scaffolding for InventoryService with the following requirements:

**Component Type:** Service
**Error Scope:** Database operations and business logic
**Logging Level:** Error and Critical
**User Display:** Toast notifications for user errors, silent logging for system errors

**Error Handling Requirements:**
- Create centralized error logging with database and file logging support
- Implement user-friendly error messages with technical details hidden
- Integrate with ReactiveUI ThrownExceptions subscription patterns
- Support error categorization (validation, system, business, critical)
- Include error recovery mechanisms where appropriate
- Provide audit trail capabilities for error tracking

**Technical Implementation:**
- Generate error handling classes with proper naming conventions
- Create error message templates with MTM branding
- Implement async error logging with stored procedure integration
- Include error code classification system
- Support structured logging with contextual information
- Prepare for dependency injection integration

**MTM-Specific Requirements:**
- Log all database operation errors with stored procedure details
- Track inventory transaction errors with business context
- Include user ID and operation context in error logs
- Support MTM business rule validation error handling
- Integrate with application state management for error recovery

**Additional Context:** Service handles inventory transactions and should provide detailed business context in error logs while showing simple messages to users
```

### **Example 2: System-wide Error Infrastructure**
```
Act as Error Handling Copilot. Create error handling system scaffolding for GlobalErrorHandler with the following requirements:

**Component Type:** System-wide
**Error Scope:** All operations
**Logging Level:** All levels (Information through Critical)
**User Display:** Context-appropriate display (silent, toast, modal based on severity)

**Error Handling Requirements:**
- Create centralized error logging with database and file logging support
- Implement user-friendly error messages with technical details hidden
- Integrate with ReactiveUI ThrownExceptions subscription patterns
- Support error categorization (validation, system, business, critical)
- Include error recovery mechanisms where appropriate
- Provide audit trail capabilities for error tracking

**Technical Implementation:**
- Generate error handling classes with proper naming conventions
- Create error message templates with MTM branding
- Implement async error logging with stored procedure integration
- Include error code classification system
- Support structured logging with contextual information
- Prepare for dependency injection integration

**MTM-Specific Requirements:**
- Log all database operation errors with stored procedure details
- Track inventory transaction errors with business context
- Include user ID and operation context in error logs
- Support MTM business rule validation error handling
- Integrate with application state management for error recovery

**Additional Context:** Global handler should catch unhandled exceptions, provide fallback error display, and ensure application stability
```

## ?? **Guidelines**

### **Error Classification System**
- **Validation Errors**: User input validation, business rule violations
- **System Errors**: Database connectivity, file system, network issues
- **Business Errors**: MTM-specific business logic violations
- **Critical Errors**: Application stability threats, data corruption risks

### **Logging Patterns**
```csharp
// Structured logging with context
public async Task LogErrorAsync(Exception ex, string operation, string userId, Dictionary<string, object> context)
{
    try
    {
        await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            Model_AppVariables.ConnectionString,
            "sys_error_Log_Insert",
            new Dictionary<string, object>
            {
                ["ErrorMessage"] = ex.Message,
                ["StackTrace"] = ex.StackTrace,
                ["Operation"] = operation,
                ["UserId"] = userId,
                ["Context"] = JsonSerializer.Serialize(context),
                ["Timestamp"] = DateTime.UtcNow
            }
        );
    }
    catch
    {
        // Fallback to file logging if database fails
        await LogToFileAsync(ex, operation, userId, context);
    }
}
```

### **ReactiveUI Integration**
```csharp
// Centralized error handling for ViewModels
LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
LoadDataCommand.ThrownExceptions.Subscribe(async ex =>
{
    await _errorHandler.HandleErrorAsync(ex, nameof(LoadDataAsync), GetCurrentUserId());
    ErrorMessage = _errorHandler.GetUserFriendlyMessage(ex);
});
```

### **User-Friendly Error Messages**
- Hide technical details from end users
- Provide actionable guidance when possible
- Use MTM branding and terminology
- Support multiple languages/localization
- Include error reference numbers for support

## ?? **Related Files**
- [../Development-Instructions/errorhandler.instruction.md](../Development-Instructions/errorhandler.instruction.md) - Complete error handling guidelines
- [../Core-Instructions/codingconventions.instruction.md](../Core-Instructions/codingconventions.instruction.md) - Error handling patterns and conventions
- [../Development-Instructions/database-patterns.instruction.md](../Development-Instructions/database-patterns.instruction.md) - Database error handling and logging
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - Error Handling Copilot persona details

## ? **Quality Checklist**

### **Error Handling Infrastructure**
- [ ] Centralized error logging implementation
- [ ] Database and file logging fallback mechanisms
- [ ] Structured logging with contextual information
- [ ] Error categorization and classification system
- [ ] User-friendly error message generation

### **Integration Compliance**
- [ ] ReactiveUI ThrownExceptions integration
- [ ] Dependency injection preparation
- [ ] MTM business context logging
- [ ] Stored procedure error logging patterns
- [ ] Application state management integration

### **User Experience**
- [ ] Context-appropriate error display
- [ ] Technical details hidden from end users
- [ ] Actionable guidance provided where possible
- [ ] MTM branding and terminology consistency
- [ ] Error recovery mechanisms implemented

### **Technical Quality**
- [ ] Async error logging implementation
- [ ] Proper exception handling and propagation
- [ ] Performance considerations for logging
- [ ] Memory and resource management
- [ ] Thread safety for concurrent operations