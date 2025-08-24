# GitHub Copilot Instructions for MTM WIP Application Avalonia

## Project Overview
This is an Avalonia UI application for MTM (Manitowoc Tool and Manufacturing) WIP Inventory System using MVVM pattern with Avalonia.ReactiveUI.

Required packages:
- Avalonia
- Avalonia.Desktop
- Avalonia.Themes.Fluent
- Avalonia.Diagnostics (dev only)
- Avalonia.ReactiveUI

Program setup (ensure ReactiveUI is enabled):
```csharp
using Avalonia;
using Avalonia.ReactiveUI;

public static class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI(); // Enable ReactiveUI integration
}
```

## **📚 Organized Instruction Files Reference**

**This main instruction file provides project overview and coordination, but for specialized contexts, refer to the organized instruction categories:**

### **🔧 Core Instructions** (`Core-Instructions/`)
- **Primary Usage**: All development work requiring coding patterns, naming standards, or project structure guidance
- **Key Files**: 
  - `codingconventions.instruction.md` - ReactiveUI, MVVM, and .NET 8 patterns
  - `naming.conventions.instruction.md` - File, class, and service naming standards
  - `dependency-injection.instruction.md` - DI container setup and AddMTMServices usage
  - `project-structure.instruction.md` - Repository organization and file placement
- **When to Use**: Before starting any development task, creating new components, or setting up services

### **🎨 UI Instructions** (`UI-Instructions/`)
- **Primary Usage**: Creating Views, ViewModels, or any UI-related components
- **Key Files**: 
  - `ui-generation.instruction.md` (823 lines) - Avalonia AXAML generation patterns
  - `ui-mapping.instruction.md` - WinForms to Avalonia control mapping
- **When to Use**: Generating Avalonia AXAML, converting WinForms patterns, or implementing MTM design system

### **⚙️ Development Instructions** (`Development-Instructions/`)
- **Primary Usage**: Workflow setup, error handling implementation, or database integration
- **Key Files**: 
  - `errorhandler.instruction.md` - Error handling patterns and implementation
  - `githubworkflow.instruction.md` - GitHub Actions and CI/CD configuration
  - `database-patterns.instruction.md` - Database access patterns and MTM business logic
  - `templates-documentation.instruction.md` - HTML templates and documentation migration
- **When to Use**: Implementing error handling, setting up CI/CD, working with database patterns, or migrating documentation

### **✅ Quality Instructions** (`Quality-Instructions/`)
- **Primary Usage**: Quality assurance, compliance verification, or system repair
- **Key Files**: `needsrepair.instruction.md` (630 lines, comprehensive quality standards)
- **When to Use**: Code reviews, quality assessments, or identifying missing system components

### **🤖 Automation Instructions** (`Automation-Instructions/`)
- **Primary Usage**: Creating custom prompts, defining Copilot personas, or automation workflows
- **Key Files**: 
  - `personas.instruction.md` (640 lines) - Specialized Copilot behaviors
  - `customprompts.instruction.md` - Custom prompt templates and automation
  - `issue-pr-creation.instruction.md` - Automated issue and PR documentation generation
- **When to Use**: Specialized Copilot behaviors, prompt engineering, or workflow automation

### **📂 Related Documentation**
- **Development/UI_Documentation/**: Component-specific instruction files (43 additional files)
- **Archive/**: Historical files and migration documentation
- **Each Category README.md**: Detailed descriptions of category contents and usage guidelines

---

## **Quick Reference for Common Tasks**

### **Starting New Development Work**
1. **Review**: [Core Instructions](Core-Instructions/) for coding patterns and naming
2. **Setup**: Use [dependency-injection.instruction.md](Core-Instructions/dependency-injection.instruction.md) for service registration
3. **Structure**: Follow [project-structure.instruction.md](Core-Instructions/project-structure.instruction.md) for file placement

### **Creating UI Components**
1. **Generate**: Use [UI Instructions](UI-Instructions/) for Avalonia AXAML and ViewModels
2. **Convert**: Reference [ui-mapping.instruction.md](UI-Instructions/ui-mapping.instruction.md) for WinForms patterns
3. **Style**: Apply MTM design system from [ui-generation.instruction.md](UI-Instructions/ui-generation.instruction.md)

### **Database Operations**
1. **Patterns**: Follow [database-patterns.instruction.md](Development-Instructions/database-patterns.instruction.md)
2. **Business Logic**: Apply MTM TransactionType rules (user intent, not operation numbers)
3. **Access**: Use stored procedures only, never direct SQL

### **Error Handling**
1. **Implementation**: Use [errorhandler.instruction.md](Development-Instructions/errorhandler.instruction.md)
2. **Patterns**: Apply ReactiveUI error handling with stored procedure logging
3. **UI**: Integrate with error display components

### **Documentation and Templates**
1. **HTML Migration**: Use [templates-documentation.instruction.md](Development-Instructions/templates-documentation.instruction.md)
2. **FileDefinitions**: Apply PlainEnglish or Technical templates as appropriate
3. **Cross-References**: Maintain links between related documentation

### **Automation and Issues**
1. **Issue Creation**: Use [issue-pr-creation.instruction.md](Automation-Instructions/issue-pr-creation.instruction.md)
2. **Custom Prompts**: Apply [customprompts.instruction.md](Automation-Instructions/customprompts.instruction.md)
3. **Specialized Behavior**: Reference [personas.instruction.md](Automation-Instructions/personas.instruction.md)

---

## **Critical MTM Business Rules Summary**

### **Documentation Integrity and Synchronization** (CRITICAL)
- **MANDATORY**: When ANY .md file is altered in any way, TWO actions are REQUIRED:
  1. **HTML Synchronization**: Update ALL corresponding HTML files that use the modified .md file as a source
  2. **Data Validation**: Validate that ALL data being added/removed from the .md file is 100% accurate and truthful
- **Scope**: This applies to ALL markdown files including instruction files, documentation, FileDefinitions, issue tracking, and project documentation
- **Verification**: Before completing any markdown modification task, confirm both HTML updates and data accuracy validation have been performed
- **Quality Gate**: No markdown changes should be considered complete without validated HTML synchronization and verified data accuracy

### **Question Generation Rule** (CRITICAL)
- **MANDATORY**: When a user asks me to "ask questions" or requests clarification through questions, I MUST generate an interactive HTML questionnaire file instead of asking questions directly in chat
- **Location**: All questionnaire files must be saved in `Documentation/Development/CopilotQuestions/` directory
- **Naming Convention**: `{Topic}_{Purpose}_Questions.html` (e.g., `File_Reference_Validation_Script_Questions.html`)
- **Format Requirements**: 
  - Modern responsive HTML with MTM purple theme styling
  - Interactive form with progress tracking
  - Question categorization with visual badges
  - Real-time validation and progress updates
  - Summary generation upon completion
- **Never**: Ask clarification questions directly in chat when an HTML questionnaire is more appropriate
- **Always**: Generate questionnaire files for complex configuration, feature requirements, or multi-part decision processes

### **TransactionType Logic** (from [database-patterns.instruction.md](Development-Instructions/database-patterns.instruction.md))
- **IN**: User adding stock (regardless of operation number)
- **OUT**: User removing stock (regardless of operation number)  
- **TRANSFER**: User moving stock between locations (regardless of operation number)
- **Operation numbers are workflow steps, NOT transaction type indicators**

### **Database Access** (from [database-patterns.instruction.md](Development-Instructions/database-patterns.instruction.md))
- **Required**: Use stored procedures only via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()`
- **Prohibited**: Direct SQL queries in code
- **Development**: Use `Development/Database_Files/` for new procedures

### **Service Registration** (from [dependency-injection.instruction.md](Core-Instructions/dependency-injection.instruction.md))
- **Required**: Use `services.AddMTMServices(configuration)` - never register services individually
- **Pattern**: Comprehensive registration prevents missing dependency errors
- **Lifetimes**: Singletons for infrastructure, Scoped for business logic, Transient for ViewModels

---

## Remember
- This is an Avalonia app, not WPF or WinForms
- Use Avalonia-specific syntax and controls
- Follow MVVM pattern strictly with ReactiveUI
- Keep views and ViewModels paired and consistently named
- Generate clean, readable code with proper spacing
- Add XML comments only where helpful for understanding UI purpose
- Follow modern UI patterns with cards, sidebars, and clean layouts
- Use ReactiveUI's reactive programming paradigms (WhenAnyValue, OAPH, etc.)
- Apply the MTM brand gradient and color scheme consistently throughout the application
- **NEVER use direct SQL - always use stored procedures**
- All development files are now in the `Development/` folder
- **TransactionType is determined by USER INTENT, not Operation numbers**
- **Use specialized instruction files for detailed guidance - this main file provides coordination and overview**