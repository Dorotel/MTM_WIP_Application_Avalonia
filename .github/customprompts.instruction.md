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

---

## Custom Prompt Templates

---

### 1. Create UI Element  
**Persona:** UI Architect Copilot  
*(See [personas-instruction.md](personas-instruction.md) for role details)*

**Prompt:**  
"Create a new UI element for [ControlName] using the mapped .instructions.md and screenshot.  
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
"Generate empty event handler stubs (with TODO comments) for [ControlName] and its specified events.  
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