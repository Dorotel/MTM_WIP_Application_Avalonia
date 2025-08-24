# MTM Copilot Quick Commands for Visual Studio 2022

## Copy these ready-to-use prompts for GitHub Copilot Chat:

### ?? UI Generation Commands

#### @ui:create - Create UI Element
```
Act as UI Architect Copilot. Create a new Avalonia UI element following MTM patterns.

Requirements:
- Generate AXAML with compiled bindings
- Apply MTM purple theme and design system  
- Implement modern layout patterns with proper spacing
- Use appropriate Avalonia controls and ReactiveUI integration
- Follow MTM naming conventions
- Include proper event handler stubs
- Ensure responsive design and accessibility

Component: [SPECIFY COMPONENT NAME]
Purpose: [SPECIFY PURPOSE]
```

#### @ui:viewmodel - Create ReactiveUI ViewModel
```
Act as ReactiveUI Specialist Copilot. Create a ReactiveUI ViewModel following MTM patterns.

Requirements:
- Implement ReactiveUI base class inheritance
- Use RaiseAndSetIfChanged for property notifications
- Create ReactiveCommand instances for user actions
- Implement WhenAnyValue for derived properties
- Apply proper validation with ReactiveUI patterns
- Include error handling with Result<T> integration
- Follow MTM naming conventions
- Prepare for dependency injection

ViewModel: [SPECIFY VIEWMODEL NAME]
Functionality: [SPECIFY FUNCTIONALITY]
```

### ?? Business Logic Commands

#### @biz:handler - Create Business Logic Handler
```
Act as Application Logic Copilot. Create a business logic handler class following MTM patterns.

Requirements:
- Implement comprehensive business rule validation
- Use Result<T> pattern for operation responses
- Integrate with MTM database patterns via stored procedures
- Apply proper error handling with logging
- Ensure no UI dependencies (pure business logic)
- Follow MTM naming conventions
- Include proper dependency injection preparation
- Implement async/await patterns where appropriate

Functionality: [SPECIFY FUNCTIONALITY]
Business Rules: [SPECIFY RULES]
```

#### @biz:model - Create MTM Data Model
```
Act as MTM Business Logic Specialist Copilot. Create data models following MTM-specific patterns.

Requirements:
- Implement MTM data patterns (PartID as string, Operation as string numbers, Quantity as integer)
- Apply proper validation attributes
- Include INotifyPropertyChanged for UI binding
- Follow MTM naming conventions
- Implement proper equality comparers
- Include XML documentation
- Apply business rule validation

Model: [SPECIFY MODEL NAME]
Data: [SPECIFY DATA REQUIREMENTS]
```

### ??? Database Commands

#### @db:procedure - Create Stored Procedure
```
Act as Data Access Copilot. Create a stored procedure following MTM database patterns.

Requirements:
- Implement standardized input validation
- Use consistent output parameters (@RowsAffected, @ErrorMessage, @StatusCode)
- Apply MTM business logic (TransactionType based on user intent)
- Include comprehensive error handling with try-catch blocks
- Implement proper transaction management
- Follow MTM naming conventions for procedures and parameters
- Include helpful comments and documentation
- Validate all inputs and provide meaningful error messages

Operation: [SPECIFY OPERATION]
Tables Involved: [SPECIFY TABLES]
```

#### @db:service - Create Database Service Layer
```
Act as Data Access Copilot. Create a database service layer following MTM patterns.

Requirements:
- Implement centralized database access using Helper_Database_StoredProcedure
- Use Result<T> pattern for all operations
- Apply comprehensive error handling and logging
- Include connection management and disposal
- Follow MTM naming conventions
- Implement async/await patterns
- Include proper transaction management
- Integrate with dependency injection

Service: [SPECIFY SERVICE NAME]
Operations: [SPECIFY OPERATIONS]
```

### ? Quality Assurance Commands

#### @qa:verify - Verify Code Compliance  
```
Act as Quality Assurance Auditor Copilot. Verify code compliance against MTM WIP Application instruction guidelines.

Focus on:
- MTM naming conventions compliance
- ReactiveUI pattern implementation
- MVVM separation validation
- Error handling integration
- UI generation standards
- Database access patterns

Generate a structured compliance report with priority levels and actionable remediation steps for:
[SPECIFY FILES TO CHECK]
```

#### @qa:refactor - Refactor Code to Naming Convention
```
Act as Code Style Advisor Copilot. Refactor code to match MTM naming conventions.

Requirements:
- Apply MTM file naming patterns
- Implement proper class and method naming
- Follow property and field naming standards
- Apply namespace organization rules
- Ensure consistent code formatting
- Maintain functionality while improving code quality
- Document any breaking changes

Files to refactor: [SPECIFY FILES]
Focus areas: [SPECIFY FOCUS]
```

### ??? System Commands

#### @sys:di - Setup Dependency Injection Container
```
Act as System Infrastructure Copilot. Setup dependency injection container following MTM patterns.

Requirements:
- Configure Microsoft.Extensions.DependencyInjection
- Use services.AddMTMServices(configuration) pattern
- Register all required service interfaces and implementations
- Apply proper service lifetimes (Singleton, Scoped, Transient)
- Include configuration binding
- Implement proper service resolution
- Follow MTM service registration patterns

Services to register: [SPECIFY SERVICES]
Configuration: [SPECIFY CONFIG NEEDS]
```

#### @sys:services - Create Core Service Interfaces
```
Act as System Infrastructure Copilot. Create core service interfaces following MTM patterns.

Requirements:
- Define clean service contracts
- Apply proper method signatures with Result<T> returns
- Include comprehensive XML documentation
- Follow MTM naming conventions
- Consider async/await patterns
- Plan for dependency injection
- Define proper separation of concerns

Service interfaces: [SPECIFY INTERFACES]
Functionality: [SPECIFY FUNCTIONALITY]
```

## ?? How to Use:

1. **Copy the desired command** from above
2. **Replace placeholders** in brackets with your specific needs
3. **Paste into GitHub Copilot Chat** in Visual Studio 2022
4. **Press Enter** to execute

## ?? Example Usage:

For your current file (AdvancedRemoveView.axaml), copy this:

```
Act as Quality Assurance Auditor Copilot. Verify code compliance against MTM WIP Application instruction guidelines.

Focus on:
- MTM naming conventions compliance
- ReactiveUI pattern implementation
- MVVM separation validation
- Error handling integration
- UI generation standards
- Database access patterns

Generate a structured compliance report with priority levels and actionable remediation steps for:
AdvancedRemoveView.axaml and AdvancedRemoveView.axaml.cs
```