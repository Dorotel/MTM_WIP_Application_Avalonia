<!-- Copilot: Reading custom-prompts.instruction.md — Custom Prompts and Copilot Personas -->

# Custom Copilot Prompts

Below is a bulleted list of custom prompt types, each with a plain-English description and suggested Copilot persona.  
Use these as templates for interacting with Copilot, ensuring responses are relevant and context-aware.

> **See [personas-instruction.md](personas-instruction.md) for complete persona definitions and behavioral guidelines.**

---

## Prompt Types

- **Create UI Element**  
  For generating Avalonia controls or views based on mapping and instructions.
- **Create UI Element from Markdown Instructions**  
  For generating Avalonia AXAML and ReactiveUI ViewModels from parsed markdown files with component hierarchies.
- **Create Error System Placeholder**  
  For scaffolding a new error handling class or module with standard conventions.
- **Create Logging Info Placeholder**  
  For generating a logging helper or logging configuration according to project standards.
- **Create Business Logic Handler**  
  For scaffolding a business logic class or method with correct naming and separation from UI.
- **Create Database Access Layer Placeholder**  
  For generating a stub or scaffolding for database interaction classes or repositories.
- **Create UI Element for Error Messages**  
  For generating or integrating `Control_ErrorMessage` or `ErrorDialog_Enhanced` based on the new error system. Follow ui-generation and error_handler instructions.
- **Create Unit Test Skeleton**  
  For generating a basic, empty unit test class or method in the appropriate structure.
- **Create Configuration File Placeholder**  
  For generating an empty config/settings file with standard project structure.
- **Create ReactiveUI ViewModel**  
  For generating ViewModels with ReactiveUI patterns, commands, and observable properties.
- **Create Avalonia Theme Resources**  
  For generating MTM-specific color schemes and theme resources using the purple brand palette.
- **Create MTM Data Model**  
  For generating models that follow MTM-specific data patterns (Part ID, Operation numbers, etc.).
- **Refactor Code to Naming Convention**  
  For requesting Copilot to rename code elements to fit project naming rules.
- **Add Event Handler Stub**  
  For quickly generating empty event handler methods (with TODOs) for specific controls.
- **Document Public API/Class**  
  For generating XML documentation for public members.
- **Create Customized Prompt**  
  For drafting new actionable custom prompts and assigning appropriate personas.
- **Update Instruction File from Master**  
  For synchronizing any instruction file with relevant content from the main copilot-instructions.md file.
- **Create Modern Layout Pattern**  
  For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns.
- **Create Context Menu Integration**  
  For adding context menus to components with management features following MTM patterns.
- **Verify Code Compliance**  
  For conducting comprehensive quality assurance reviews of code against all MTM instruction guidelines and generating detailed compliance reports.

## Missing Core Systems Implementation Prompts

### **Phase 1: Foundation Systems**
- **Implement Result Pattern System**  
  For creating the Result<T> pattern infrastructure for consistent service responses.
- **Create Data Models Foundation**  
  For generating the complete Models namespace with MTM-specific data entities.
- **Setup Dependency Injection Container**  
  For configuring Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs.
- **Create Core Service Interfaces**  
  For generating all essential service interfaces following MTM patterns.

### **Phase 2: Service Layer Implementation**
- **Implement Service Layer**  
  For creating complete service implementations with MTM patterns and error handling.
- **Create Database Service Layer**  
  For implementing centralized database access with proper connection management.
- **Setup Application State Management**  
  For creating global application state service with proper encapsulation.
- **Implement Configuration Service**  
  For creating service to read and manage appsettings.json configuration.

### **Phase 3: Infrastructure Systems**
- **Create Navigation Service**  
  For implementing proper MVVM navigation patterns with view-viewmodel mapping.
- **Implement Theme System**  
  For creating MTM purple brand theme resources and DynamicResource patterns.
- **Setup Repository Pattern**  
  For implementing data access abstraction layer with repository interfaces.
- **Create Validation System**  
  For implementing business rule validation with MTM-specific patterns.

### **Phase 4: Quality Assurance Systems**
- **Create Unit Testing Infrastructure**  
  For setting up comprehensive testing framework with mocks and fixtures.
- **Implement Structured Logging**  
  For adding centralized logging throughout the application with proper levels.
- **Create Caching Layer**  
  For implementing performance-oriented caching for ComboBox data and user preferences.
- **Setup Security Infrastructure**  
  For implementing authentication, authorization, and secure connection management.

---

## Custom Prompt Templates

---

### 1. Create UI Element  
**Persona:** UI Architect Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create a new UI element for [source] using the mapped .instructions.md and screenshot.  
Follow all naming and UI layout conventions. Only include navigation logic and event stubs as per our standards."

---

### 2. Create UI Element from Markdown Instructions  
**Persona:** UI Architect Copilot + ReactiveUI Specialist  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create UI Element from [filename].md following MTM-specific UI generation guidelines.  
Parse the markdown component structure and generate both AXAML view and ReactiveUI ViewModel.  
Include MTM data patterns (Part ID, Operation numbers), context menu integration, and space optimization.  
Follow the MTM purple color scheme and modern layout patterns. Leave business logic as TODO comments."

---

### 3. Create Error System Placeholder  
**Persona:** Error Handling Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Scaffold a new error handler class or module according to our error_handler-instruction.md.  
Include methods and properties for logging errors, but do not implement business logic yet."

---

### 4. Create Logging Info Placeholder  
**Persona:** Logging Engineer Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate a logging helper class or configuration section.  
Follow the conventions in error_handler-instruction.md for log file structure and method names, but keep logic as stubs."

---

### 5. Create Business Logic Handler  
**Persona:** Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create a business logic handler class or method for [Purpose].  
Ensure no UI code is present, and follow naming and separation best practices."

---

### 6. Create Database Access Layer Placeholder  
**Persona:** Data Access Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate a C# class or repository interface for interacting with [Database/Entity].  
Include method stubs for CRUD operations, following project conventions, but leave implementation empty."

---

### 7. Create UI Element for Error Messages  
**Persona:** UI Architect Copilot + Error Handling Specialist  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create or integrate an error message UI using Control_ErrorMessage (inline) or ErrorDialog_Enhanced (modal).  
Use ui-generation and error_handler-instruction for conventions. Include only navigation/event stubs, no business logic."

---

### 8. Create Unit Test Skeleton  
**Persona:** Test Automation Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate a unit test skeleton for [ClassOrMethodName].  
Include setup/teardown stubs and name the test class/methods according to our conventions."

---

### 9. Create Configuration File Placeholder  
**Persona:** Configuration Wizard Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create a placeholder configuration under Config/ (e.g., appsettings.json) with sections for Application, ErrorHandling, Logging, Database.  
Leave values empty or commented where appropriate and add a README describing fields.  
See custom-prompts-examples.md for an example."

---

### 10. Create ReactiveUI ViewModel  
**Persona:** ReactiveUI Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate a ReactiveUI ViewModel for [Purpose] following MTM patterns.  
Include observable properties with RaiseAndSetIfChanged, ReactiveCommands with proper error handling,  
WhenAnyValue patterns for derived properties, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection."

---

### 11. Create Avalonia Theme Resources  
**Persona:** UI Theme Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate Avalonia theme resources using the MTM purple brand palette.  
Include SolidColorBrush resources for Primary (#4B45ED), Secondary (#8345ED), Magenta (#BA45ED),  
Blue (#4574ED), Pink (#ED45E7), and Light Purple (#B594ED) colors.  
Add gradient brushes for hero sections and hover states following the MTM design system."

---

### 12. Create MTM Data Model  
**Persona:** Data Modeling Copilot  
*(See [personas-instruction.md](personas.instruction.md) for role details)*

**Prompt:**  
"Generate a data model for [Entity] following MTM-specific patterns.  
Include Part ID (string), Operation (string numbers like '90', '100'), Quantity (integer),  
Position (1-based indexing), and other relevant MTM inventory properties.  
Ensure compatibility with ReactiveUI observable patterns."

---

### 13. Refactor Code to Naming Convention  
**Persona:** Code Style Advisor Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Review and refactor the following code to match our naming conventions (see naming-conventions.instruction.md).  
Rename all classes, controls, and variables as needed."

---

### 14. Add Event Handler Stub  
**Persona:** Event Wiring Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate empty event handler stubs (with TODO comments) for [source] and its specified events.  
Use correct naming and signature conventions."

---

### 15. Document Public API/Class  
**Persona:** Documentation Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Add XML documentation comments to all public members in [ClassOrFileName] according to C# and project standards."

---

### 16. Create Customized Prompt  
**Persona:** Prompt Engineering Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Draft a new custom Copilot prompt for [Purpose or Scenario].  
Ensure it is actionable, clear, and consistent with our existing instruction and naming conventions.  
Add a corresponding example in custom-prompts-examples.md."

---

### 17. Update Instruction File from Master  
**Persona:** Documentation Web Publisher Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Update [target-instruction-file.md] with any relevant information from copilot-instructions.md that is not there, only transfer data that is relevant to the target file's specific purpose and scope.  
Maintain the existing structure and organization of the target file while adding missing relevant content.  
Ensure cross-references to other instruction files remain intact."

**Usage Example:**  
`update [ui-generation.instruction.md](#ui-generation.instruction.md-context) with any information from copilot-instructions.md that is not there, only transfer data that is relevant to UI generation`

**Alternative Usage:**  
`update [naming-conventions.instruction.md](#naming-conventions.instruction.md-context) with any information from copilot-instructions.md that is not there, only transfer data that is relevant to naming conventions`

---

### 18. Create Modern Layout Pattern  
**Persona:** UI Architect Copilot + Design System Specialist  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate a modern Avalonia layout using MTM design patterns.  
Include sidebar navigation (240px width), card-based content with rounded corners and shadows,  
hero sections with MTM purple gradients, and proper spacing using Grid layouts.  
Apply the MTM color scheme and ensure responsive design principles."

---

### 19. Create Context Menu Integration  
**Persona:** UI Interaction Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Add context menu functionality to [ComponentName] following MTM patterns.  
Include Edit, Remove, Clear All, and Refresh options with proper command bindings.  
Use separators for grouping and ensure commands are bound to ReactiveCommand properties.  
Follow MTM component management conventions."

---

### 20. Verify Code Compliance  
**Persona:** Quality Assurance Auditor Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Review [filename] for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying any violations of naming conventions, UI generation standards, ReactiveUI patterns, MTM data patterns, color scheme requirements, architecture guidelines, error handling standards, and layout/design principles. Include specific code examples of violations and required fixes with priority levels."

**Usage Example:**  
`Review Views/MainView.axaml and ViewModels/MainViewModel.cs for compliance with all MTM WIP Application instruction guidelines. Generate a detailed report in needsrepair.instruction.md identifying violations and required fixes.`

**Advanced Usage:**  
`Conduct comprehensive quality audit of [ProjectName] codebase. Review all Views and ViewModels for MTM compliance. Generate prioritized repair list in needsrepair.instruction.md with specific fix recommendations and instruction file references.`

---

## 🏗️ **Missing Core Systems Implementation Prompts**

*Based on analysis from needsrepair.instruction.md identifying 19 missing core systems*

### **21. Implement Result Pattern System**  
**Persona:** Data Modeling Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create the Result<T> pattern infrastructure for MTM WIP Application following .NET 8 patterns.  
Implement Models/Result.cs with Success/Failure states, error messages, and implicit operators.  
Include static factory methods for Success(T) and Failure(string), proper equality comparison,  
and integration patterns for async service methods. Follow MTM error handling conventions and  
ensure compatibility with ReactiveUI command error handling patterns."

---

### **22. Create Data Models Foundation**  
**Persona:** Data Modeling Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create the complete Models namespace foundation for MTM WIP Application with all essential data entities.  
Generate Models/InventoryItem.cs, Models/InventoryTransaction.cs, Models/User.cs, and Models/QuickButtonItem.cs  
following MTM-specific patterns. Include Part ID (string), Operation (string numbers), Quantity (integer),  
Position (1-based indexing), timestamps, and user tracking. Ensure ReactiveUI compatibility with observable  
properties where needed. Add proper validation attributes and XML documentation."

---

### **23. Setup Dependency Injection Container**  
**Persona:** Application Logic Copilot + Configuration Wizard Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Configure Microsoft.Extensions.DependencyInjection in Program.cs and App.axaml.cs for MTM WIP Application.  
Setup service container with proper lifetime management (Singleton, Scoped, Transient).  
Create extension method for service registration and prepare registration for IInventoryService,  
IUserService, ITransactionService, IDatabaseService, INavigationService, IApplicationStateService,  
and IConfigurationService. Include development vs production service variants and  
integrate with existing LoggingUtility and Service_ErrorHandler."

---

### **24. Create Core Service Interfaces**  
**Persona:** Application Logic Copilot + Data Access Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Generate all essential service interfaces for MTM WIP Application following clean architecture patterns.  
Create Services/IInventoryService.cs, Services/IUserService.cs, Services/ITransactionService.cs,  
Services/IDatabaseService.cs, Services/INavigationService.cs, Services/IApplicationStateService.cs,  
and Services/IConfigurationService.cs. Include async methods returning Result<T> pattern,  
MTM-specific operations (Part ID, Operation strings), proper cancellation token support,  
and comprehensive XML documentation. Prepare for dependency injection implementation."

---

### **25. Implement Service Layer**  
**Persona:** Application Logic Copilot + Error Handling Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement complete service layer for [ServiceName] following MTM patterns and clean architecture.  
Create Services/[ServiceName].cs implementing I[ServiceName] interface with full error handling  
using Service_ErrorHandler, async/await patterns, and Result<T> return types. Include MTM data  
pattern support, integration with LoggingUtility for database operations, comprehensive logging,  
and proper validation. Add XML documentation and prepare for dependency injection with  
constructor parameter validation."

---

### **26. Create Database Service Layer**  
**Persona:** Data Access Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create centralized database service layer with proper connection management for MTM WIP Application.  
Implement Services/IDatabaseService.cs and Services/DatabaseService.cs with connection pooling,  
transaction management, and async operations. Include connection string management from configuration,  
retry policies for connection failures, proper disposal patterns, and integration with existing  
LoggingUtility. Add stored procedure execution methods, parameter handling, and MTM-specific  
data access patterns following clean architecture principles."

---

### **27. Setup Application State Management**  
**Persona:** Application Logic Copilot + ReactiveUI Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create global application state service for MTM WIP Application with proper encapsulation and reactivity.  
Implement Services/IApplicationStateService.cs and Services/ApplicationStateService.cs using ReactiveUI  
patterns for state change notifications. Include current user management, connection status tracking,  
application settings, and shared state between ViewModels. Ensure thread-safety, proper disposal,  
and integration with configuration service. Add state persistence and recovery mechanisms."

---

### **28. Implement Configuration Service**  
**Persona:** Configuration Wizard Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create configuration service to read and manage appsettings.json for MTM WIP Application.  
Implement Services/IConfigurationService.cs and Services/ConfigurationService.cs using  
Microsoft.Extensions.Configuration. Include strongly-typed configuration classes for Application,  
ErrorHandling, Logging, and Database sections. Add configuration validation, environment-specific  
overrides, and real-time configuration reload support. Integrate with dependency injection  
and provide convenient access methods for all application components."

---

### **29. Create Navigation Service**  
**Persona:** UI Architect Copilot + ReactiveUI Specialist Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement proper MVVM navigation service for MTM WIP Application with view-viewmodel mapping.  
Create Services/INavigationService.cs and Services/NavigationService.cs supporting parametrized  
navigation, navigation history, and modal dialog management. Include view registration,  
automatic ViewModel instantiation with dependency injection, navigation events, and proper  
cleanup of ViewModels. Ensure compatibility with Avalonia UI and ReactiveUI patterns while  
maintaining clean separation between Views and ViewModels."

---

### **30. Implement Theme System**  
**Persona:** UI Theme Specialist Copilot + UI Architect Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create comprehensive MTM purple brand theme system for Avalonia application with DynamicResource patterns.  
Generate Resources/Themes/MTMTheme.axaml with complete color palette: Primary (#4B45ED), Secondary (#8345ED),  
Magenta (#BA45ED), Blue (#4574ED), Pink (#ED45E7), Light Purple (#B594ED). Include gradient brushes,  
card styles, button styles, and modern UI element definitions. Add theme switching capability,  
dark/light mode variants, and proper integration with App.axaml. Ensure all hard-coded colors  
are replaced with DynamicResource references."

---

### **31. Setup Repository Pattern**  
**Persona:** Data Access Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement data access abstraction layer with repository pattern for MTM WIP Application.  
Create Repositories/IInventoryRepository.cs, Repositories/IUserRepository.cs, and  
Repositories/ITransactionRepository.cs interfaces with corresponding implementations.  
Include generic repository base class, unit of work pattern, and proper async operations.  
Integrate with DatabaseService for connection management, add comprehensive error handling,  
and ensure compatibility with MTM data patterns. Include repository registration in DI container."

---

### **32. Create Validation System**  
**Persona:** Data Modeling Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement business rule validation system for MTM WIP Application with comprehensive validation framework.  
Create Services/IValidationService.cs and validation attributes for MTM-specific patterns.  
Include Part ID format validation, Operation string number validation, quantity range validation,  
and business rule enforcement. Add FluentValidation integration, custom validation attributes,  
and ValidationResult patterns. Ensure integration with ReactiveUI ViewModels and proper  
error message localization support."

---

### **33. Create Unit Testing Infrastructure**  
**Persona:** Test Automation Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Setup comprehensive unit testing infrastructure for MTM WIP Application with .NET 8 testing framework.  
Create test project with xUnit, Moq, and FluentAssertions. Generate mock implementations for all  
service interfaces, test data builders for MTM entities, and testing utilities. Include repository  
pattern testing, ReactiveUI ViewModel testing with time scheduling, and service layer testing  
with proper async patterns. Add test fixtures, in-memory database testing, and CI/CD integration  
configuration."

---

### **34. Implement Structured Logging**  
**Persona:** Logging Engineer Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Add centralized structured logging throughout MTM WIP Application with Microsoft.Extensions.Logging.  
Enhance existing LoggingUtility with structured logging patterns, log levels, and proper categorization.  
Include performance logging, user action tracking, error correlation IDs, and log enrichment.  
Add Serilog integration for advanced logging capabilities, log file rotation, and structured  
JSON logging. Ensure integration with Service_ErrorHandler and proper log level configuration  
from appsettings.json."

---

### **35. Create Caching Layer**  
**Persona:** Application Logic Copilot + Data Access Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement performance-oriented caching layer for MTM WIP Application with Microsoft.Extensions.Caching.  
Create Services/ICacheService.cs and Services/CacheService.cs with memory and distributed caching support.  
Include ComboBox data caching, user preference caching, and frequently accessed data optimization.  
Add cache invalidation strategies, expiration policies, and cache warming. Ensure thread-safety,  
proper disposal, and integration with configuration service for cache settings. Include cache  
metrics and monitoring capabilities."

---

### **36. Setup Security Infrastructure**  
**Persona:** Security Engineer Copilot + Application Logic Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Implement authentication, authorization, and secure connection management for MTM WIP Application.  
Create Services/IAuthenticationService.cs, Services/IAuthorizationService.cs, and security middleware.  
Include user authentication with secure credential storage, role-based authorization, and  
connection string encryption. Add security headers, input sanitization, and audit logging.  
Ensure integration with existing user management and proper security event tracking.  
Include development vs production security configurations."

---

## MTM-Specific Workflow Patterns

### UI Generation from Markdown Files
When working with markdown instruction files that describe UI components:

1. **Parse Component Hierarchy** - Extract the tree structure and convert to Avalonia AXAML
2. **Map MTM Data Types** - Use Part ID (string), Operation (string numbers), Quantity (integer)
3. **Apply Context Menu Patterns** - Add management features via right-click menus
4. **Implement Space Optimization** - Use UniformGrid and proper alignment for component removal
5. **Follow MTM Color Scheme** - Apply purple brand colors consistently
6. **Create ReactiveUI Bindings** - Generate ViewModels with proper observable patterns

### Quality Assurance Workflow
For comprehensive code compliance verification:

1. **Initial Assessment** - Review code files against all instruction guidelines
2. **Violation Identification** - Document specific non-compliance issues with examples
3. **Priority Classification** - Categorize issues as High/Medium/Low priority
4. **Fix Recommendations** - Provide specific guidance for bringing code into compliance
5. **Report Generation** - Update needsrepair.instruction.md with structured findings
6. **Re-verification** - Follow up reviews to confirm compliance improvements

### Missing Systems Implementation Workflow
For implementing missing core systems identified in compliance analysis:

1. **Foundation Phase** - Implement data models, Result pattern, and DI container
2. **Service Layer Phase** - Create all service interfaces and implementations
3. **Infrastructure Phase** - Add navigation, themes, repositories, and validation
4. **Quality Assurance Phase** - Implement testing, logging, caching, and security

### Instruction File Synchronization
For keeping instruction files synchronized with the main copilot-instructions.md:

**Pattern:** `I have the main copilot-instructions.md file loaded with [context description]. Update [target-file.md] with any missing [specific content type] from the main file. Focus only on [scope] and [behavioral guidelines].`

**Available Synchronization Targets:**
- `codingconventions.instruction.md` - Coding standards, ReactiveUI patterns, MVVM guidelines
- `ui-generation.instruction.md` - AXAML templates, layout patterns, MTM design guidelines  
- `error_handler-instruction.md` - ReactiveUI error patterns, Avalonia error display
- `naming-conventions.instruction.md` - File naming, ViewModel naming, service naming
- `personas-instruction.md` - Copilot personas, behavioral guidelines
- `ui-mapping.instruction.md` - Control mappings, screenshot relationships
- `needsrepair.instruction.md` - Quality assurance patterns, compliance tracking

### Advanced Context-Aware Updates
Use these patterns for comprehensive instruction file updates:

```
I have the main copilot-instructions.md file loaded with [comprehensive context]. Update [target-instruction.md] with any missing [specific domain knowledge] from the main file. Only add content relevant to [target domain] and maintain existing cross-references.
```

---

## Rule for Adding Custom Prompts

- **Whenever a new custom prompt is added to this file, an example usage for that prompt must also be added to [custom-prompts-examples.md](custom-prompts-examples.md).**  
  This ensures all prompts are documented with practical examples for reference and consistency.

- **For persona descriptions, see [personas-instruction.md](personas-instruction.md).**

## Persona Behavioral Guidelines

### MTM-Specific Behaviors
All personas should follow these MTM-specific behavioral patterns:

- **Operations are Numbers**: Always treat operations as string numbers ("90", "100", "110"), not action descriptions
- **Part ID Format**: Use string format for Part IDs (e.g., "PART001")
- **1-Based Indexing**: UI positions use 1-based indexing for display
- **Purple Brand Consistency**: Apply MTM purple color scheme (#4B45ED primary, #BA45ED accents)
- **ReactiveUI Patterns**: Use ReactiveObject, RaiseAndSetIfChanged, ReactiveCommand, WhenAnyValue
- **Modern Layout Principles**: Prefer Grid over StackPanel, use cards and sidebars
- **Context Menu Management**: Add right-click functionality for component management
- **Business Logic Separation**: Leave implementation as TODO comments, focus on structure

### Avalonia-Specific Behaviors
- **AXAML Generation**: Use proper xmlns declarations and compiled bindings
- **Control Mapping**: Convert WinForms controls to Avalonia equivalents
- **Theme Resources**: Use DynamicResource for colors to support theming
- **Modern UI Elements**: Implement cards, shadows, gradients, and proper spacing

### Quality Assurance Behaviors
- **Comprehensive Review**: Check all aspects of code against instruction guidelines
- **Structured Reporting**: Use standardized format for violation documentation
- **Priority Assignment**: Classify issues based on impact and compliance severity
- **Solution-Oriented**: Provide specific fix recommendations with instruction references
- **Progress Tracking**: Update compliance status and maintain audit trail

### Missing Systems Implementation Behaviors
- **Foundation First**: Always implement foundational systems before dependent systems
- **Clean Architecture**: Follow dependency inversion and separation of concerns
- **Error Handling**: Integrate Service_ErrorHandler in all new implementations
- **Testing Support**: Include testability considerations in all implementations
- **Documentation**: Provide comprehensive XML documentation for all public APIs
- **DI Preparation**: Ensure all services are prepared for dependency injection