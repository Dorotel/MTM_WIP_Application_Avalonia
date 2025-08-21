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

### 13. Create ReactiveUI ViewModel

**Prompt:**  
Generate a ReactiveUI ViewModel for user profile management following MTM patterns.  
Include observable properties with RaiseAndSetIfChanged, ReactiveCommands with proper error handling,  
WhenAnyValue patterns for derived properties, and centralized exception handling.  
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
Ensure compatibility with ReactiveUI observable patterns.

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
Use separators for grouping and ensure commands are bound to ReactiveCommand properties.  
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