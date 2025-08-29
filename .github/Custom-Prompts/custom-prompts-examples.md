<!-- Copilot: Reading custom-prompts-examples.md — Example Custom Prompts Usage -->

# Custom Prompt Examples

> See [personas-instruction.md](personas-instruction.md) for full persona descriptions.

This file provides example usages for each custom Copilot prompt described in [custom-prompts.instruction.md](custom-prompts.instruction.md).  
Use these examples as templates when instructing Copilot for specific tasks.

---

## Examples

---

### 1. Create UI Element

**Prompt:**  
Create a new UI element for Control_AddLocation using the mapped .instructions.md and screenshot.  
Follow all naming and UI layout conventions. Only include navigation logic and event stubs as per our standards.

---

### 2. Create Error System Placeholder

**Prompt:**  
Scaffold a new error handler class or module according to our error_handler-instruction.md.  
Include methods and properties for logging errors, but do not implement business logic yet.

---

### 3. Create Logging Info Placeholder

**Prompt:**  
Generate a logging helper class or configuration section.  
Follow the conventions in error_handler-instruction.md for log file structure and method names, but keep logic as stubs.

---

### 4. Create Business Logic Handler

**Prompt:**  
Create a business logic handler class or method for inventory processing.  
Ensure no UI code is present, and follow naming and separation best practices.

---

### 5. Create Database Access Layer Placeholder

**Prompt:**  
Generate a C# class or repository interface for interacting with the PartNumbers table.  
Include method stubs for CRUD operations, following project conventions, but leave implementation empty.

---

### 6. Create Unit Test Skeleton

**Prompt:**  
Generate a unit test skeleton for the ErrorHandler class.  
Include setup/teardown stubs and name the test class/methods according to our conventions.

---

### 7. Create Configuration File Placeholder

**Prompt:**  
Create a placeholder for a configuration file (e.g., .json, .xml, .config) for application startup settings.  
Include the required structure but leave all fields empty or commented.

**Example:**  
Create a Config/appsettings.json placeholder with sections for Application, ErrorHandling, Logging, and Database. Leave all values empty or commented where applicable and include a Config/README.md explaining each setting. Example structure:

```
{
  "ErrorHandling": { "EnableFileServerLogging": true, "EnableMySqlLogging": false, "FileServerBasePath": "", "MySqlConnectionString": "" }
}
```

---

### 8. Refactor Code to Naming Convention

**Prompt:**  
Review and refactor the following code to match our naming conventions (see naming-conventions.instruction.md).  
Rename all classes, controls, and variables as needed.

---

### 9. Add Event Handler Stub

**Prompt:**  
Generate empty event handler stubs (with TODO comments) for MainView_TabControl and its specified events.  
Use correct naming and signature conventions.

---

### 10. Document Public API/Class

**Prompt:**  
Add XML documentation comments to all public members in InventoryService.cs according to C# and project standards.

---

### 11. Create Customized Prompt

**Prompt:**  
Draft a new custom Copilot prompt for generating a summary report for completed transactions.  
Ensure it is actionable, clear, and consistent with our existing instruction and naming conventions.  
Add a corresponding example in custom-prompts-examples.md.

---

### 12. Create UI Element for Error Messages

**Prompt:**  
Create an inline error UI using Controls/Control_ErrorMessage to present an InvalidOperationException that occurs in UserForm_Button_Save.  
Make it use severity High with a retry button stub. Follow ui-generation and error_handler-instruction.md.  
Do not add business logic; only wire event stubs.

---

### 13. Create Standard ViewModel

**Prompt:**  
Generate a Standard .NET ViewModel for user profile management following MTM patterns.  
Include INotifyPropertyChanged properties with SetProperty, ICommand implementations with proper error handling,  
validation patterns for user input, and centralized exception handling using try-catch blocks.  
Use ObservableCollection for data collections and prepare for dependency injection.

---

### 14. Create Avalonia Theme Resources

**Prompt:**  
Generate Avalonia theme resources using the MTM purple brand palette.  
Include SolidColorBrush resources for Primary (#4B45ED), Secondary (#8345ED), Magenta (#BA45ED),  
Blue (#4574ED), Pink (#ED45E7), and Light Purple (#B594ED) colors.  
Add gradient brushes for hero sections and hover states following the MTM design system.

---

### 15. Create MTM Data Model

**Prompt:**  
Generate a data model for InventoryTransaction following MTM-specific patterns.  
Include Part ID (string), Operation (string numbers like '90', '100'), Quantity (integer),  
Position (1-based indexing), and other relevant MTM inventory properties.  
Ensure compatibility with standard .NET observable patterns.

---

### 16. Create Modern Layout Pattern

**Prompt:**  
Generate a modern Avalonia layout for the main dashboard using MTM design patterns.  
Include sidebar navigation (240px width), card-based content with rounded corners and shadows,  
hero sections with MTM purple gradients, and proper spacing using Grid layouts.  
Apply the MTM color scheme and ensure responsive design principles.

---

### 17. Create Context Menu Integration

**Prompt:**  
Add context menu functionality to QuickButtonsPanel following MTM patterns.  
Include Edit, Remove, Clear All, and Refresh options with proper command bindings.  
Use separators for grouping and ensure commands are bound to ICommand properties.  
Follow MTM component management conventions.

---

### 18. Create UI Element from Markdown Instructions

**Prompt:**  
Create UI Element from Control_QuickButtons.instructions.md following MTM-specific UI generation guidelines.  
Parse the markdown component structure and generate both AXAML view and ReactiveUI ViewModel.  
Include MTM data patterns (Part ID, Operation numbers), context menu integration, and space optimization.  
Follow the MTM purple color scheme and modern layout patterns. Leave business logic as TODO comments.

---

### 19. Update Instruction File from Master

**Prompt:**  
Update ui-generation.instruction.md with any relevant information from copilot-instructions.md that is not there, only transfer data that is relevant to UI generation.  
Maintain the existing structure and organization of the target file while adding missing relevant content.  
Ensure cross-references to other instruction files remain intact.

---

### 20. Verify Code Compliance

**Prompt:**  
Review ViewModels/InventoryViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying any violations of naming conventions, UI generation standards, ReactiveUI patterns, MTM data patterns, color scheme requirements, architecture guidelines, error handling standards, and layout/design principles. Include specific code examples of violations and required fixes with priority levels.

**Advanced Example:**  
Review the entire Views/ and ViewModels/ directories for compliance with all MTM WIP Application instruction guidelines. Generate a comprehensive report in needsrepair.instruction.md identifying violations across naming conventions, ReactiveUI patterns, MTM data handling, Avalonia UI standards, color scheme usage, and architecture guidelines. Prioritize fixes by impact and provide specific implementation guidance with instruction file references.

**Specific File Example:**  
Review Views/MainView.axaml and ViewModels/MainViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations and required fixes.

**Bulk Assessment Example:**  
Conduct comprehensive quality audit of MTM_WIP_Application_Avalonia project. Review all C# and AXAML files for MTM compliance. Generate prioritized repair list in needsrepair.instruction.md with specific fix recommendations and instruction file references. Include compliance metrics and implementation roadmap.

**Domain-Specific Example:**  
Review all error handling implementations across the project for compliance with errorhandler.instruction.md guidelines. Generate focused compliance report in needsrepair.instruction.md identifying missing error patterns, logging inconsistencies, and user experience violations. Provide specific remediation steps for each finding.

---

## 🏗️ **Missing Core Systems Implementation Examples**

*Examples for implementing the 19 missing core systems identified in compliance analysis*

---

### 21. Implement Result Pattern System

**Prompt:**  
Create the Result<T> pattern infrastructure for MTM WIP Application following .NET 8 patterns.  
Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators.  
Include static factory methods for Success(T) and Failure(string), proper equality comparison,  
and integration patterns for async service methods. Follow MTM error handling conventions and  
ensure compatibility with ReactiveUI command error handling patterns.

**Usage Example:**  
```
Create the Result<T> pattern infrastructure for MTM WIP Application following .NET 8 patterns. Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators for service responses. Include Success<T>(T value) and Failure(string error) factory methods, IsSuccess and IsFailure properties, and proper integration with ReactiveUI error handling in ViewModels.
```

---

### 22. Create Data Models Foundation

**Prompt:**  
Create the complete Models namespace foundation for MTM WIP Application with all essential data entities.  
Generate Models/InventoryItem.cs, Models/InventoryTransaction.cs, Models/User.cs, and Models/QuickButtonItem.cs  
following MTM-specific patterns. Include Part ID (string), Operation (string numbers), Quantity (integer),  
Position (1-based indexing), timestamps, and user tracking. Ensure ReactiveUI compatibility with observable  
properties where needed. Add proper validation attributes and XML documentation.

**Usage Example:**  
```
Create the complete Models namespace foundation with Models/InventoryItem.cs (PartId string, Operation string, Quantity int, Location string, LastModified DateTime), Models/InventoryTransaction.cs (TransactionId, PartId, Operation, Quantity, UserId, Timestamp), Models/User.cs (UserId string, Username string, Role string), and Models/QuickButtonItem.cs (Position int, PartId string, Operation string, Quantity int) following MTM data patterns.
```

---

### 23. Setup Dependency Injection Container

**Prompt:**  
Configure Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs for MTM WIP Application.  
Setup service container with proper lifetime management (Singleton, Scoped, Transient).  
Create extension method for service registration and prepare registration for IInventoryService,  
IUserService, ITransactionService, IDatabaseService, INavigationService, IApplicationStateService,  
and IConfigurationService. Include development vs production service variants and  
integrate with existing LoggingUtility and Service_ErrorHandler.

**Usage Example:**  
```
Configure Microsoft.Extensions.DependencyInjection in Program.cs with service container setup. Add Microsoft.Extensions.DependencyInjection NuGet package, create Services/ServiceCollectionExtensions.cs with AddMTMServices() extension method, register all core service interfaces as Singleton/Scoped appropriately, and integrate with existing LoggingUtility and Service_ErrorHandler services.
```

---

### 24. Create Core Service Interfaces

**Prompt:**  
Generate all essential service interfaces for MTM WIP Application following clean architecture patterns.  
Create Services/IInventoryService.cs, Services/IUserService.cs, Services/ITransactionService.cs,  
Services/IDatabaseService.cs, Services/INavigationService.cs, Services/IApplicationStateService.cs,  
and Services/IConfigurationService.cs. Include async methods returning Result<T> pattern,  
MTM-specific operations (Part ID, Operation strings), proper cancellation token support,  
and comprehensive XML documentation. Prepare for dependency injection implementation.

**Usage Example:**  
```
Generate core service interfaces: Services/IInventoryService.cs with GetInventoryAsync(), AddInventoryItemAsync(), ProcessOperationAsync() methods; Services/IUserService.cs with GetCurrentUserAsync(), AuthenticateAsync(); Services/ITransactionService.cs with GetTransactionHistoryAsync(), LogTransactionAsync(); all using Result<T> return pattern and CancellationToken support.
```

---

### 25. Implement Service Layer

**Prompt:**  
Implement complete service layer for InventoryService following MTM patterns and clean architecture.  
Create Services/InventoryService.cs implementing IInventoryService interface with full error handling  
using Service_ErrorHandler, async/await patterns, and Result<T> return types. Include MTM data  
pattern support, integration with LoggingUtility for database operations, comprehensive logging,  
and proper validation. Add XML documentation and prepare for dependency injection with  
constructor parameter validation.

**Usage Example:**  
```
Implement Services/InventoryService.cs with constructor accepting LoggingUtility and Service_ErrorHandler. Include GetInventoryAsync() method with try/catch using Service_ErrorHandler.HandleException(), ProcessOperationAsync() for MTM operations ("90", "100", "110"), AddInventoryItemAsync() with validation, all returning Result<T> pattern and integrating with existing database utilities.
```

---

### 26. Create Database Service Layer

**Prompt:**  
Create centralized database service layer with proper connection management for MTM WIP Application.  
Implement Services/IDatabaseService.cs and Services/DatabaseService.cs with connection pooling,  
transaction management, and async operations. Include connection string management from configuration,  
retry policies for connection failures, proper disposal patterns, and integration with existing  
LoggingUtility. Add stored procedure execution methods, parameter handling, and MTM-specific  
data access patterns following clean architecture principles.

**Usage Example:**  
```
Create Services/DatabaseService.cs with connection management using Microsoft.Data.SqlClient, async ExecuteQueryAsync<T>() and ExecuteNonQueryAsync() methods, connection string from IConfiguration, retry policies with Polly, and integration with LoggingUtility for database operation logging. Include transaction scope support and proper disposal patterns.
```

---

### 27. Setup Application State Management

**Prompt:**  
Create global application state service for MTM WIP Application with proper encapsulation and reactivity.  
Implement Services/IApplicationStateService.cs and Services/ApplicationStateService.cs using ReactiveUI  
patterns for state change notifications. Include current user management, connection status tracking,  
application settings, and shared state between ViewModels. Ensure thread-safety, proper disposal,  
and integration with configuration service. Add state persistence and recovery mechanisms.

**Usage Example:**  
```
Create Services/ApplicationStateService.cs inheriting ReactiveObject with CurrentUser (User), ConnectionStatus (ReactiveProperty<ConnectionState>), ApplicationSettings (ReactiveProperty<AppSettings>), and UserPreferences (ReactiveProperty<UserPrefs>). Include thread-safe property updates with RaiseAndSetIfChanged and state change notifications for ViewModels to subscribe to.
```

---

### 28. Implement Configuration Service

**Prompt:**  
Create configuration service to read and manage appsettings.json for MTM WIP Application.  
Implement Services/IConfigurationService.cs and Services/ConfigurationService.cs using  
Microsoft.Extensions.Configuration. Include strongly-typed configuration classes for Application,  
ErrorHandling, Logging, and Database sections. Add configuration validation, environment-specific  
overrides, and real-time configuration reload support. Integrate with dependency injection  
and provide convenient access methods for all application components.

**Usage Example:**  
```
Create Services/ConfigurationService.cs with constructor accepting IConfiguration, strongly-typed properties like DatabaseConnectionString, ErrorHandlingSettings, LoggingConfiguration. Include GetSection<T>() generic method, configuration validation with IValidateOptions<T>, and environment-specific configuration override support for Development/Production environments.
```

---

### 29. Create Navigation Service

**Prompt:**  
Implement proper MVVM navigation service for MTM WIP Application with view-viewmodel mapping.  
Create Services/INavigationService.cs and Services/NavigationService.cs supporting parametrized  
navigation, navigation history, and modal dialog management. Include view registration,  
automatic ViewModel instantiation with dependency injection, navigation events, and proper  
cleanup of ViewModels. Ensure compatibility with Avalonia UI and ReactiveUI patterns while  
maintaining clean separation between Views and ViewModels.

**Usage Example:**  
```
Create Services/NavigationService.cs with NavigateToAsync<TViewModel>(object parameters), NavigateToAsync(string viewName, object parameters), ShowDialogAsync<TViewModel>(), and GoBackAsync() methods. Include view registration dictionary mapping ViewModels to Views, ViewModel instantiation through DI container, and navigation history stack management with proper disposal.
```

---

### 30. Implement Theme System

**Prompt:**  
Create comprehensive MTM purple brand theme system for Avalonia application with DynamicResource patterns.  
Generate Resources/Themes/MTMTheme.axaml with complete color palette: Primary (#4B45ED), Secondary (#8345ED),  
Magenta (#BA45ED), Blue (#4574ED), Pink (#ED45E7), Light Purple (#B594ED). Include gradient brushes,  
card styles, button styles, and modern UI element definitions. Add theme switching capability,  
dark/light mode variants, and proper integration with App.axaml. Ensure all hard-coded colors  
are replaced with DynamicResource references.

**Usage Example:**  
```
Create Resources/Themes/MTMTheme.axaml with SolidColorBrush resources: PrimaryBrush (#4B45ED), SecondaryBrush (#8345ED), MagentaAccentBrush (#BA45ED), BlueAccentBrush (#4574ED), PinkAccentBrush (#ED45E7), LightPurpleBrush (#B594ED). Include LinearGradientBrush for PrimaryGradientBrush and HeroGradientBrush, card styles with CornerRadius and BoxShadow, and integrate with App.axaml ResourceDictionary.
```

---

### 31. Setup Repository Pattern

**Prompt:**  
Implement data access abstraction layer with repository pattern for MTM WIP Application.  
Create Repositories/IInventoryRepository.cs, Repositories/IUserRepository.cs, and  
Repositories/ITransactionRepository.cs interfaces with corresponding implementations.  
Include generic repository base class, unit of work pattern, and proper async operations.  
Integrate with DatabaseService for connection management, add comprehensive error handling,  
and ensure compatibility with MTM data patterns. Include repository registration in DI container.

**Usage Example:**  
```
Create Repositories/IInventoryRepository.cs with GetAllAsync(), GetByIdAsync(string partId), AddAsync(InventoryItem), UpdateAsync(InventoryItem), DeleteAsync(string partId) methods. Implement Repositories/InventoryRepository.cs with constructor accepting DatabaseService, proper async implementations using repository pattern, and Result<T> return types with comprehensive error handling.
```

---

### 32. Create Validation System

**Prompt:**  
Implement business rule validation system for MTM WIP Application with comprehensive validation framework.  
Create Services/IValidationService.cs and validation attributes for MTM-specific patterns.  
Include Part ID format validation, Operation string number validation, quantity range validation,  
and business rule enforcement. Add FluentValidation integration, custom validation attributes,  
and ValidationResult patterns. Ensure integration with ReactiveUI ViewModels and proper  
error message localization support.

**Usage Example:**  
```
Create Services/ValidationService.cs with ValidatePartIdAsync(string partId), ValidateOperationAsync(string operation), ValidateQuantityAsync(int quantity) methods. Add custom validation attributes [PartIdFormat], [OperationNumber], [QuantityRange], and FluentValidation validators for InventoryItem, User, and InventoryTransaction models with proper error messages.
```

---

### 33. Create Unit Testing Infrastructure

**Prompt:**  
Setup comprehensive unit testing infrastructure for MTM WIP Application with .NET 8 testing framework.  
Create test project with xUnit, Moq, and FluentAssertions. Generate mock implementations for all  
service interfaces, test data builders for MTM entities, and testing utilities. Include repository  
pattern testing, ReactiveUI ViewModel testing with time scheduling, and service layer testing  
with proper async patterns. Add test fixtures, in-memory database testing, and CI/CD integration  
configuration.

**Usage Example:**  
```
Create MTM_WIP_Application_Avalonia.Tests project with xUnit, Moq, FluentAssertions NuGet packages. Generate Tests/Services/InventoryServiceTests.cs with Mock<ILogggingUtility>, Tests/ViewModels/InventoryTabViewModelTests.cs with ReactiveUI TestScheduler, Tests/Builders/InventoryItemBuilder.cs for test data, and Tests/Fixtures/DatabaseFixture.cs for integration testing.
```

---

### 34. Implement Structured Logging

**Prompt:**  
Add centralized structured logging throughout MTM WIP Application with Microsoft.Extensions.Logging.  
Enhance existing LoggingUtility with structured logging patterns, log levels, and proper categorization.  
Include performance logging, user action tracking, error correlation IDs, and log enrichment.  
Add Serilog integration for advanced logging capabilities, log file rotation, and structured  
JSON logging. Ensure integration with Service_ErrorHandler and proper log level configuration  
from appsettings.json.

**Usage Example:**  
```
Enhance Services/LoggingUtility.cs with Microsoft.Extensions.Logging.ILogger injection, structured logging with LogInformation("User {UserId} performed {Operation} on {PartId}", userId, operation, partId), Serilog integration with file and console sinks, log correlation IDs, and configuration-driven log levels from appsettings.json LogLevel section.
```

---

### 35. Create Caching Layer

**Prompt:**  
Implement performance-oriented caching layer for MTM WIP Application with Microsoft.Extensions.Caching.  
Create Services/ICacheService.cs and Services/CacheService.cs with memory and distributed caching support.  
Include ComboBox data caching, user preference caching, and frequently accessed data optimization.  
Add cache invalidation strategies, expiration policies, and cache warming. Ensure thread-safety,  
proper disposal, and integration with configuration service for cache settings. Include cache  
metrics and monitoring capabilities.

**Usage Example:**  
```
Create Services/CacheService.cs with GetAsync<T>(string key), SetAsync<T>(string key, T value, TimeSpan expiration), RemoveAsync(string key), and InvalidatePatternAsync(string pattern) methods using Microsoft.Extensions.Caching.Memory.IMemoryCache. Include cache warming for ComboBox data, user preferences, and frequently accessed inventory items with configurable expiration policies.
```

---

### 36. Setup Security Infrastructure

**Prompt:**  
Implement authentication, authorization, and secure connection management for MTM WIP Application.  
Create Services/IAuthenticationService.cs, Services/IAuthorizationService.cs, and security middleware.  
Include user authentication with secure credential storage, role-based authorization, and  
connection string encryption. Add security headers, input sanitization, and audit logging.  
Ensure integration with existing user management and proper security event tracking.  
Include development vs production security configurations.

**Usage Example:**  
```
Create Services/AuthenticationService.cs with AuthenticateAsync(string username, string password), Services/AuthorizationService.cs with HasPermissionAsync(string userId, string permission), secure credential storage using Data Protection API, connection string encryption in configuration, and security audit logging integration with existing logging infrastructure.
```

---

## Advanced Context-Aware Instruction File Update Prompts

For sophisticated updates with detailed context awareness of all project conventions:

### Coding Conventions (Advanced)
```
I have the main copilot-instructions.md file loaded with all MTM WIP Application project conventions. Update [codingconventions.instruction.md](#codingconventions.instruction.md-context) with any missing coding standards, ReactiveUI patterns, MVVM guidelines, file naming conventions, project structure information, and .NET 8 specific patterns from the main file. Only add content relevant to coding conventions and maintain existing cross-references.
```

### Custom Prompts (Advanced)
```
I have the main copilot-instructions.md file loaded with comprehensive project guidelines and UI generation patterns. Update [customprompts.instruction.md](#customprompts.instruction.md-context) with any missing prompt templates, persona mappings, MTM-specific workflow patterns, and instruction file synchronization methods from the main file. Focus only on custom prompts and persona behavioral guidelines.
```

### Error Handler (Advanced)
```
I have the main copilot-instructions.md file loaded with error handling patterns and ReactiveUI command structures. Update [errorhandler.instruction.md](#errorhandler.instruction.md-context) with any missing ReactiveUI error handling patterns, command error patterns, MTM-specific error scenarios, Avalonia UI error display patterns, and theme-aware error styling from the main file. Only transfer content relevant to error handling and logging.
```

### GitHub Workflow (Advanced)
```
I have the main copilot-instructions.md file loaded with project structure and build requirements. Update [githubworkflow.instruction.md](#githubworkflow.instruction.md-context) with any missing .NET 8 build configurations, Avalonia-specific CI/CD requirements, ReactiveUI testing patterns, and MTM project deployment guidelines from the main file. Focus only on GitHub workflow and CI/CD processes.
```

### Naming Conventions (Advanced)
```
I have the main copilot-instructions.md file loaded with comprehensive naming standards and project structure. Update [naming.conventions.instruction.md](#naming.conventions.instruction.md-context) with any missing file naming patterns, ViewModel naming, service naming, MTM data patterns, Avalonia-specific naming conventions, and ReactiveUI property naming standards from the main file. Maintain focus on naming standards only.
```

### Needs Repair (Advanced)
```
I have the main copilot-instructions.md file loaded with comprehensive MTM quality standards and compliance requirements. Update [needsrepair.instruction.md](#needsrepair.instruction.md-context) with any missing quality assurance patterns, code compliance verification methods, violation categorization systems, and fix recommendation frameworks from the main file. Only add content relevant to quality assurance and code compliance tracking.
```

### Personas (Advanced)
```
I have the main copilot-instructions.md file loaded with detailed project roles and responsibilities. Update [personas.instruction.md](#personas.instruction.md-context) with any missing Copilot personas, behavioral guidelines for MTM-specific tasks, Avalonia UI architect roles, ReactiveUI specialist behaviors, and documentation publisher patterns from the main file. Focus only on Copilot personas and their behavioral guidelines.
```

### UI Generation (Advanced)
```
I have the main copilot-instructions.md file loaded with comprehensive Avalonia UI guidelines and MTM-specific patterns. Update [ui-generation.instruction.md](#ui-generation.instruction.md-context) with any missing AXAML templates, layout patterns, control mappings, MTM-specific UI guidelines, theme system information, ReactiveUI binding patterns, and modern Avalonia design principles from the main file. Focus only on UI generation aspects and preserve existing structure.
```

### UI Mapping (Advanced)
```
I have the main copilot-instructions.md file loaded with control mapping standards and UI relationships. Update [ui-mapping.instruction.md](#ui-mapping.instruction.md-context) with any missing WinForms to Avalonia control mappings, screenshot-to-instruction relationships, MTM-specific control hierarchies, and modern UI pattern mappings from the main file. Only add content relevant to UI mapping and screenshot relationships while maintaining existing cross-references.
```

---

## Quality Assurance Examples

### **Single File Review**
```
Review Services/InventoryService.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations of naming conventions, async patterns, error handling standards, and MTM business logic guidelines. Include specific code examples and priority-based fix recommendations.
```

### **Component Pair Review**
```
Review Views/InventoryView.axaml and ViewModels/InventoryViewModel.cs as a paired component for compliance with all MTM instruction guidelines. Generate report in needsrepair.instruction.md identifying MVVM violations, binding issues, ReactiveUI pattern compliance, and MTM data handling. Provide specific fixes for maintaining proper View/ViewModel relationship.
```

### **Architecture-Focused Review**
```
Review the entire project architecture for compliance with MVVM separation guidelines. Generate focused report in needsrepair.instruction.md identifying business logic in UI components, missing dependency injection patterns, and improper service layer usage. Prioritize architectural violations that impact maintainability.
```

### **Theme and Design Review**
```
Review all AXAML files for compliance with MTM color scheme and modern UI design guidelines. Generate report in needsrepair.instruction.md identifying hard-coded colors, missing DynamicResource usage, non-MTM palette colors, and layout pattern violations. Include specific fixes for achieving design consistency.
```

### **ReactiveUI Pattern Review**
```
Review all ViewModels for compliance with ReactiveUI patterns and best practices. Generate report in needsrepair.instruction.md identifying missing RaiseAndSetIfChanged usage, improper command implementations, missing error handling patterns, and non-reactive property implementations. Provide specific ReactiveUI pattern fixes.
```

### **Missing Systems Assessment**
```
Conduct missing core systems analysis of MTM_WIP_Application_Avalonia project following needsrepair.instruction.md guidelines. Identify all missing service layers, data models, dependency injection setup, navigation services, theme systems, and other critical infrastructure. Generate comprehensive analysis report with implementation priority order and specific custom prompts for each missing system.
```

---

## Implementation Workflow Examples

### **Phase 1: Foundation Implementation**
```
Execute Phase 1 foundation implementation for MTM WIP Application. Use prompts 21-24 to implement Result pattern system, create data models foundation, setup dependency injection container, and create core service interfaces. Follow the implementation priority order and ensure each system integrates properly with existing LoggingUtility and Service_ErrorHandler infrastructure.
```

### **Phase 2: Service Layer Implementation**
```
Execute Phase 2 service layer implementation for MTM WIP Application. Use prompts 25-28 to implement service layer, create database service layer, setup application state management, and implement configuration service. Ensure proper integration with Phase 1 foundation systems and maintain MTM data patterns throughout implementation.
```

### **Phase 3: Infrastructure Implementation**
```
Execute Phase 3 infrastructure implementation for MTM WIP Application. Use prompts 29-32 to create navigation service, implement theme system, setup repository pattern, and create validation system. Ensure seamless integration with existing ViewModels and maintain proper MVVM separation patterns.
```

### **Phase 4: Quality Assurance Implementation**
```
Execute Phase 4 quality assurance implementation for MTM WIP Application. Use prompts 33-36 to create unit testing infrastructure, implement structured logging, create caching layer, and setup security infrastructure. Ensure comprehensive coverage of all implemented systems and proper CI/CD integration preparation.