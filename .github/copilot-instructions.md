# GitHub Copilot Instructions for MTM WIP Application Avalonia

You are an expert Avalonia UI developer working on the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System. This is a .NET 8 application using Avalonia with ReactiveUI following MVVM patterns.

<details>
<summary><strong>🎯 Your Role and Expertise</strong></summary>

- **Primary Focus**: Generate Avalonia UI components, ReactiveUI ViewModels, and business logic following MTM standards
- **Architecture**: MVVM with ReactiveUI, dependency injection, and service-oriented design
- **Data Patterns**: MTM-specific patterns where Part ID = string, Operation = string numbers, Quantity = integer
- **Database Access**: Use stored procedures only via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` - NEVER direct SQL
- **UI Framework**: Avalonia (not WPF or WinForms) with compiled bindings and DynamicResource patterns

</details>

<details>
<summary><strong>🚨 CRITICAL: AVLN2000 Error Prevention</strong></summary>

**BEFORE generating ANY AXAML code, consult [avalonia-xaml-syntax.instruction.md](.github/UI-Instructions/avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

### Key Rules to Prevent AVLN2000:
1. **NEVER use `Name` property on Grid definitions** - Use `x:Name` only
2. **Use Avalonia namespace**: `xmlns="https://github.com/avaloniaui"` (NOT WPF namespace)
3. **Grid syntax**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
4. **Control equivalents**: Use `TextBlock` instead of `Label`, `Flyout` instead of `Popup`
5. **Always include compiled bindings**: `x:CompileBindings="True"` and `x:DataType="vm:ViewModelName"`

**Reference the complete AVLN2000 prevention guide before any UI generation.**

</details>

<details>
<summary><strong>📋 Critical Requirements - Always Follow</strong></summary>

### Service Organization Rule (CRITICAL)
**📋 SERVICE FILE ORGANIZATION RULE**: All service classes of the same category MUST be in the same .cs file. Interfaces remain in the `Services/Interfaces/` folder.

```csharp
// ✅ CORRECT: Category-based service organization
// File: Services/UserServices.cs
namespace MTM_Shared_Logic.Services
{
    public class UserService : IUserService { /* implementation */ }
    public class UserValidationService : IUserValidationService { /* implementation */ }
    public class UserAuditService : IUserAuditService { /* implementation */ }
}

// File: Services/Interfaces/IUserService.cs
namespace MTM_Shared_Logic.Services.Interfaces
{
    public interface IUserService { /* interface definition */ }
}

// ❌ WRONG: One service per file
// Services/UserService.cs - Only UserService (INCORRECT)
// Services/UserValidationService.cs - Only UserValidationService (INCORRECT)
```

**Service Categories**:
- **UserServices.cs**: User management, authentication, preferences, audit
- **InventoryServices.cs**: Inventory CRUD, validation, reporting
- **TransactionServices.cs**: Transaction processing, history, validation
- **LocationServices.cs**: Location management, validation, hierarchy

### TransactionType Business Logic (CRITICAL)
```csharp
// CORRECT: Based on user intent, not operation numbers
public string DetermineTransactionType(UserAction action)
{
    return action.Intent switch
    {
        UserIntent.AddingStock => "IN",      // User adding inventory
        UserIntent.RemovingStock => "OUT",   // User removing inventory  
        UserIntent.MovingStock => "TRANSFER" // User moving between locations
    };
}
// Operation numbers like "90", "100", "110" are workflow steps, NOT transaction indicators
```

### Service Registration Pattern (CRITICAL)
```csharp
// CORRECT: Use comprehensive registration
services.AddMTMServices(configuration);

// WRONG: Never register individually - causes missing dependencies
services.AddScoped<IInventoryService, InventoryService>();
services.AddScoped<ILocationService, LocationService>();
```

### Database Access Pattern (CRITICAL)
```csharp
// CORRECT: Use stored procedures only
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    Model_AppVariables.ConnectionString,
    "inv_inventory_Add_Item",
    parameters
);

// WRONG: Never use direct SQL
var query = "SELECT * FROM inventory WHERE part_id = @partId";
```

### ReactiveUI ViewModel Pattern (CRITICAL)
```csharp
public class InventoryViewModel : ReactiveObject
{
    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => this.RaiseAndSetIfChanged(ref _partId, value);
    }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    public InventoryViewModel()
    {
        SearchCommand = ReactiveCommand.CreateFromTask(ExecuteSearchAsync);
    }
}
```

### Avalonia AXAML Patterns (CRITICAL)
```xml
<!-- CORRECT: Compiled bindings with proper namespaces -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryView"
             x:CompileBindings="True"
             x:DataType="vm:InventoryViewModel">
    
    <TextBox Text="{Binding PartId}" />
    <Button Content="Search" Command="{Binding SearchCommand}" />
</UserControl>
```

</details>

<details>
<summary><strong>🔧 Code Generation Rules</strong></summary>

### When generating UI components:
1. **Always use Avalonia controls** - Not WPF or WinForms equivalents
2. **Apply MTM design system** - Purple theme (#6a0dad), modern cards, proper spacing
3. **Use compiled bindings** - Include x:CompileBindings="True" and x:DataType
4. **Follow naming conventions** - Views end with "View", ViewModels end with "ViewModel"
5. **Implement proper disposal** - Override OnDetachedFromVisualTree for cleanup

### When generating ViewModels:
1. **Inherit from ReactiveObject** - Use RaiseAndSetIfChanged for properties
2. **Use ReactiveCommand** - For all user actions and async operations
3. **Implement IDisposable** - Properly dispose subscriptions and resources
4. **Apply validation** - Use ReactiveUI validation patterns
5. **Prepare for DI** - Design constructors for service injection

### When generating business logic:
1. **Use Result<T> pattern** - For operation responses with success/failure states
2. **Apply async/await** - For all I/O operations and database calls
3. **Implement logging** - Use ILogger<T> dependency injection
4. **Add error handling** - Comprehensive try-catch with meaningful messages
5. **Follow separation** - No UI dependencies in business logic

### When generating services:
1. **📋 Group by category** - Multiple related services in one file
2. **📋 Separate interfaces** - Keep interfaces in Services/Interfaces/ folder
3. **Follow DI patterns** - Use constructor injection and proper lifetimes
4. **Use stored procedures** - Never direct SQL queries
5. **Implement proper error handling** - Use Result<T> pattern

</details>

<details>
<summary><strong>🔢 MTM-Specific Data Patterns</strong></summary>

### Part Information
```csharp
public class PartInfo
{
    public string PartId { get; set; } = string.Empty;        // "PART001", "ABC-123"
    public string Operation { get; set; } = string.Empty;     // "90", "100", "110" (workflow steps)
    public int Quantity { get; set; }                         // Integer count only
    public string Location { get; set; } = string.Empty;      // Location identifier
}
```

### Operation Numbers Usage
```csharp
// CORRECT: Operations are workflow steps
var operations = new[] { "90", "100", "110", "120" }; // String numbers representing workflow

// WRONG: Don't use operations to determine transaction type
if (operation == "90") transactionType = "IN"; // This is incorrect logic
```

</details>

<details>
<summary><strong>⚙️ Required Project Setup</strong></summary>

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
            .UseReactiveUI(); // REQUIRED: Enable ReactiveUI integration
}
```

</details>

<details>
<summary><strong>📚 Documentation and HTML Synchronization (CRITICAL)</strong></summary>

### Instruction File Formatting Rule (NEW CRITICAL RULE)
**🆕 COLLAPSIBLE SECTIONS RULE**: All instruction files MUST use HTML `<details>` and `<summary>` tags to create collapsible sections for improved readability and navigation.

```markdown
<details>
<summary><strong>📋 Section Title</strong></summary>

Section content goes here...

</details>
```

**Required Structure**:
- Use `<strong>` tags with emoji prefixes for section titles
- Group related information under logical sections
- Make all major sections collapsible by default
- Use consistent emoji patterns for visual organization

### When modifying any .md files:
1. **Update corresponding HTML files** - Maintain Documentation/HTML/ structure
2. **Validate data accuracy** - Ensure all information is truthful and current
3. **Maintain cross-references** - Update all related links and references
4. **Follow naming conventions** - Use established file naming patterns
5. **Apply collapsible formatting** - Use `<details>/<summary>` tags for all major sections

### When creating questionnaires or clarification:
1. **Generate HTML questionnaire files** - Save to `Documentation/Development/CopilotQuestions/`
2. **Use interactive forms** - Include progress tracking and validation
3. **Apply MTM styling** - Purple theme with responsive design
4. **Never ask questions in chat** - When complex configuration is needed

</details>

<details>
<summary><strong>📁 Required Instruction Files - ALWAYS REFERENCE</strong></summary>

**🎯 CORE INFRASTRUCTURE (REQUIRED):**
- [dependency-injection.instruction.md](.github/Core-Instructions/dependency-injection.instruction.md)
- [codingconventions.instruction.md](.github/Core-Instructions/codingconventions.instruction.md) 
- [project-structure.instruction.md](.github/Core-Instructions/project-structure.instruction.md)
- [naming.conventions.instruction.md](.github/Core-Instructions/naming.conventions.instruction.md)

**🚨 UI INSTRUCTIONS (CRITICAL):**
- [avalonia-xaml-syntax.instruction.md](.github/UI-Instructions/avalonia-xaml-syntax.instruction.md)
- [ui-generation.instruction.md](.github/UI-Instructions/ui-generation.instruction.md)
- [ui-mapping.instruction.md](.github/UI-Instructions/ui-mapping.instruction.md)
- [ui-styling.instruction.md](.github/UI-Instructions/ui-styling.instruction.md)

**⚙️ DEVELOPMENT PATTERNS (REQUIRED):**
- [database-patterns.instruction.md](.github/Development-Instructions/database-patterns.instruction.md)
- [errorhandler.instruction.md](.github/Development-Instructions/errorhandler.instruction.md)
- [templates-documentation.instruction.md](.github/Development-Instructions/templates-documentation.instruction.md)
- [githubworkflow.instruction.md](.github/Development-Instructions/githubworkflow.instruction.md)

**🎯 QUALITY & AUTOMATION (REQUIRED):**
- [needsrepair.instruction.md](.github/Quality-Instructions/needsrepair.instruction.md)
- [customprompts.instruction.md](.github/Automation-Instructions/customprompts.instruction.md)
- [personas.instruction.md](.github/Automation-Instructions/personas.instruction.md)
- [issue-pr-creation.instruction.md](.github/Automation-Instructions/issue-pr-creation.instruction.md)

**🔧 ESSENTIAL CUSTOM PROMPTS:**
- [CustomPrompt_Create_ReactiveUIViewModel.md](.github/Custom-Prompts/CustomPrompt_Create_ReactiveUIViewModel.md)
- [CustomPrompt_Verify_CodeCompliance.md](.github/Custom-Prompts/CustomPrompt_Verify_CodeCompliance.md)
- [CustomPrompt_Create_UIElement.md](.github/Custom-Prompts/CustomPrompt_Create_UIElement.md)
- [CustomPrompt_Create_UIElementFromMarkdown.md](.github/Custom-Prompts/CustomPrompt_Create_UIElementFromMarkdown.md)
- [CustomPrompt_Create_ModernLayoutPattern.md](.github/Custom-Prompts/CustomPrompt_Create_ModernLayoutPattern.md)
- [CustomPrompt_Create_StoredProcedure.md](.github/Custom-Prompts/CustomPrompt_Create_StoredProcedure.md)
- [CustomPrompt_Database_ErrorHandling.md](.github/Custom-Prompts/CustomPrompt_Database_ErrorHandling.md)
- [CustomPrompt_Create_CRUDOperations.md](.github/Custom-Prompts/CustomPrompt_Create_CRUDOperations.md)
- [CustomPrompt_Implement_ResultPatternSystem.md](.github/Custom-Prompts/CustomPrompt_Implement_ResultPatternSystem.md)

**📋 SUPPORTING DOCUMENTATION:**
- [missing-components.instruction.md](.github/missing-components.instruction.md)
- [custom-prompts-examples.md](.github/Custom-Prompts/custom-prompts-examples.md)
- [MTM_Hotkey_Reference.md](.github/Custom-Prompts/MTM_Hotkey_Reference.md)
- [all-copilot-files-list.instructions.md](.github/all-copilot-files-list.instructions.md)

</details>

<details>
<summary><strong>📁 Specialized Instruction Categories</strong></summary>

Reference these organized instruction files for detailed guidance:

### **Core Infrastructure Instructions**
- **[dependency-injection.instruction.md](.github/Core-Instructions/dependency-injection.instruction.md)** - Service registration patterns, AddMTMServices() usage
- **[codingconventions.instruction.md](.github/Core-Instructions/codingconventions.instruction.md)** - .NET 8 coding standards and ReactiveUI patterns
- **[project-structure.instruction.md](.github/Core-Instructions/project-structure.instruction.md)** - Project organization and file structure
- **[naming.conventions.instruction.md](.github/Core-Instructions/naming.conventions.instruction.md)** - MTM naming conventions and standards
- **[database-patterns.instruction.md](.github/Development-Instructions/database-patterns.instruction.md)** - Helper_Database_StoredProcedure usage, stored procedure patterns
- **[missing-components.instruction.md](.github/missing-components.instruction.md)** - Current implementation status, Phase 1 completion tracking

### **UI Generation Instructions**
- **[avalonia-xaml-syntax.instruction.md](.github/UI-Instructions/avalonia-xaml-syntax.instruction.md)** - **CRITICAL**: AVLN2000 error prevention, WPF vs Avalonia AXAML differences
- **[ui-generation.instruction.md](.github/UI-Instructions/ui-generation.instruction.md)** - Avalonia AXAML generation patterns
- **[ui-mapping.instruction.md](.github/UI-Instructions/ui-mapping.instruction.md)** - UI component mapping and conversion
- **[ui-styling.instruction.md](.github/UI-Instructions/ui-styling.instruction.md)** - MTM design system and styling patterns

### **Development Instructions**
- **[errorhandler.instruction.md](.github/Development-Instructions/errorhandler.instruction.md)** - Error handling patterns and logging
- **[templates-documentation.instruction.md](.github/Development-Instructions/templates-documentation.instruction.md)** - Documentation template patterns
- **[githubworkflow.instruction.md](.github/Development-Instructions/githubworkflow.instruction.md)** - GitHub workflow and development processes

### **Quality Assurance Instructions**
- **[needsrepair.instruction.md](.github/Quality-Instructions/needsrepair.instruction.md)** - Quality assurance and repair guidelines

### **Automation Instructions**
- **[customprompts.instruction.md](.github/Automation-Instructions/customprompts.instruction.md)** - Available custom prompts and workflows
- **[personas.instruction.md](.github/Automation-Instructions/personas.instruction.md)** - GitHub Copilot personas and role definitions
- **[issue-pr-creation.instruction.md](.github/Automation-Instructions/issue-pr-creation.instruction.md)** - Automated issue and PR creation

### **Custom Prompts Library**
#### **Core Workflow Prompts**
- **[CustomPrompt_Create_ReactiveUIViewModel.md](.github/Custom-Prompts/CustomPrompt_Create_ReactiveUIViewModel.md)** - ReactiveUI ViewModel generation
- **[CustomPrompt_Verify_CodeCompliance.md](.github/Custom-Prompts/CustomPrompt_Verify_CodeCompliance.md)** - Code compliance verification
- **[CustomPrompt_Create_Issue.md](.github/Custom-Prompts/CustomPrompt_Create_Issue.md)** - GitHub issue creation

#### **UI Development Prompts**
- **[CustomPrompt_Create_UIElement.md](.github/Custom-Prompts/CustomPrompt_Create_UIElement.md)** - UI element generation
- **[CustomPrompt_Create_UIElementFromMarkdown.md](.github/Custom-Prompts/CustomPrompt_Create_UIElementFromMarkdown.md)** - UI from markdown generation
- **[CustomPrompt_Create_ModernLayoutPattern.md](.github/Custom-Prompts/CustomPrompt_Create_ModernLayoutPattern.md)** - Modern layout patterns

#### **Database and System Prompts**
- **[CustomPrompt_Create_StoredProcedure.md](.github/Custom-Prompts/CustomPrompt_Create_StoredProcedure.md)** - Stored procedure generation
- **[CustomPrompt_Database_ErrorHandling.md](.github/Custom-Prompts/CustomPrompt_Database_ErrorHandling.md)** - Database error handling
- **[CustomPrompt_Create_CRUDOperations.md](.github/Custom-Prompts/CustomPrompt_Create_CRUDOperations.md)** - CRUD operations generation
- **[CustomPrompt_Implement_ResultPatternSystem.md](.github/Custom-Prompts/CustomPrompt_Implement_ResultPatternSystem.md)** - Result pattern implementation

#### **Development Enhancement Prompts**
- **[CustomPrompt_Create_HotkeySystem.md](.github/Custom-Prompts/CustomPrompt_Create_HotkeySystem.md)** - Hotkey system implementation
- **[CustomPrompt_Create_ErrorSystemPlaceholder.md](.github/Custom-Prompts/CustomPrompt_Create_ErrorSystemPlaceholder.md)** - Error system placeholders
- **[CustomPrompt_Document_DatabaseSchema.md](.github/Custom-Prompts/CustomPrompt_Document_DatabaseSchema.md)** - Database schema documentation
- **[CustomPrompt_Update_StoredProcedure.md](.github/Custom-Prompts/CustomPrompt_Update_StoredProcedure.md)** - Stored procedure updates

#### **Supporting Resources**
- **[custom-prompts-examples.md](.github/Custom-Prompts/custom-prompts-examples.md)** - Examples and usage patterns
- **[MTM_Hotkey_Reference.md](.github/Custom-Prompts/MTM_Hotkey_Reference.md)** - MTM hotkey reference guide

### **Directory Documentation**
- **[Core-Instructions README](.github/Core-Instructions/README.md)** - Core instructions overview
- **[UI-Instructions README](.github/UI-Instructions/README.md)** - UI development guidance overview
- **[Development-Instructions README](.github/Development-Instructions/README.md)** - Development processes overview
- **[Quality-Instructions README](.github/Quality-Instructions/README.md)** - Quality standards overview
- **[Automation-Instructions README](.github/Automation-Instructions/README.md)** - Automation workflows overview
- **[Custom-Prompts README](.github/Custom-Prompts/README.md)** - Custom prompts collection overview

### **Visual Studio Integration**
- **[copilot-vs2022-config.md](copilot-vs2022-config.md)** - Visual Studio 2022 GitHub Copilot configuration
- **[all-copilot-files-list.instructions.md](.github/all-copilot-files-list.instructions.md)** - Complete file inventory

</details>
