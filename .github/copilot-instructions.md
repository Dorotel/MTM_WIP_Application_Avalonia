# GitHub Copilot Instructions for MTM WIP Application Avalonia

You are an expert Avalonia UI developer working on the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System. This is a .NET 8 application using Avalonia with **standard .NET patterns** following MVVM architecture.

<details>
<summary><strong>🎯 Your Role and Expertise</strong></summary>

- **Primary Focus**: Generate Avalonia UI components, standard .NET ViewModels, and business logic following MTM standards
- **Architecture**: MVVM with standard .NET patterns, dependency injection, and service-oriented design
- **Data Patterns**: MTM-specific patterns where Part ID = string, Operation = string numbers, Quantity = integer
- **Database Access**: Use stored procedures only via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` - NEVER direct SQL
- **UI Framework**: Avalonia (not WPF or WinForms) with standard data binding patterns

</details>

<details>
<summary><strong>🚨 CRITICAL: AVLN2000 Error Prevention</strong></summary>

**BEFORE generating ANY AXAML code, consult [avalonia-xaml-syntax.instruction.md](.github/UI-Instructions/avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

### Key Rules to Prevent AVLN2000:
1. **NEVER use `Name` property on Grid definitions** - Use `x:Name` only
2. **Use Avalonia namespace**: `xmlns="https://github.com/avaloniaui"` (NOT WPF namespace)
3. **Grid syntax**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
4. **Control equivalents**: Use `TextBlock` instead of `Label`, `Flyout` instead of `Popup`
5. **Use standard bindings**: `{Binding PropertyName}` with INotifyPropertyChanged

**Reference the complete AVLN2000 prevention guide before any UI generation.**

</details>

<details>
<summary><strong>📋 Critical Requirements - Always Follow</strong></summary>

### Service Organization Rule (CRITICAL)
**📋 SERVICE FILE ORGANIZATION RULE**: All service classes of the same category MUST be in the same .cs file. Interfaces remain in the `Services/Interfaces/` folder.

```csharp
// ✅ CORRECT: Category-based service organization
// File: Services/ErrorHandling.cs
namespace MTM_WIP_Application_Avalonia.Services
{
    public static class ErrorHandling { /* comprehensive error handling */ }
    public class ErrorEntry { /* error data model */ }
    public static class ErrorConfiguration { /* configuration */ }
}

// File: Services/Configuration.cs  
namespace MTM_WIP_Application_Avalonia.Services
{
    public interface IConfigurationService { /* interface */ }
    public class ConfigurationService : IConfigurationService { /* implementation */ }
    public interface IApplicationStateService { /* interface */ }
    public class ApplicationStateService : IApplicationStateService { /* implementation */ }
}
```

**Current Service Structure**:
- **ErrorHandling.cs**: Error handling, logging, user-friendly messages
- **Configuration.cs**: Configuration management, application state
- **Navigation.cs**: Application navigation service
- **Database.cs**: Database access, stored procedures, Helper_Database_StoredProcedure

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

// Current clean registration in ServiceCollectionExtensions.cs:
services.TryAddSingleton<IConfigurationService, ConfigurationService>();
services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
services.TryAddSingleton<INavigationService, NavigationService>();
services.TryAddScoped<IDatabaseService, DatabaseService>();
```

### Database Access Pattern (CRITICAL)
```csharp
// CORRECT: Use stored procedures only via Database service
var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "inv_inventory_Add_Item",
    parameters
);

// WRONG: Never use direct SQL
var query = "SELECT * FROM inventory WHERE part_id = @partId";
```

### Standard .NET ViewModel Pattern (CRITICAL)
```csharp
public class InventoryViewModel : BaseViewModel, INotifyPropertyChanged
{
    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => SetProperty(ref _partId, value);
    }

    public ICommand SearchCommand { get; private set; }

    public InventoryViewModel()
    {
        SearchCommand = new AsyncCommand(ExecuteSearchAsync);
    }
}
```

### Avalonia AXAML Patterns (CRITICAL)
```xml
<!-- CORRECT: Standard data bindings -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryView">
    
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
3. **Use standard bindings** - `{Binding PropertyName}` with INotifyPropertyChanged
4. **Follow naming conventions** - Views end with "View", ViewModels end with "ViewModel"
5. **Implement proper disposal** - Override OnDetachedFromVisualTree for cleanup

### When generating ViewModels:
1. **Inherit from BaseViewModel** - Use SetProperty for property changes
2. **Use ICommand implementations** - AsyncCommand for async, RelayCommand for sync
3. **Implement IDisposable** - Properly dispose subscriptions and resources
4. **Apply validation** - Use standard .NET validation patterns
5. **Prepare for DI** - Design constructors for service injection

### When generating business logic:
1. **Use established services** - ErrorHandling, Configuration, Navigation, Database
2. **Apply async/await** - For all I/O operations and database calls
3. **Implement logging** - Use ILogger<T> dependency injection
4. **Add error handling** - Use ErrorHandling.HandleErrorAsync for comprehensive error handling
5. **Follow separation** - No UI dependencies in business logic

### When generating services:
1. **📋 Group by category** - Multiple related services in one file (ErrorHandling.cs, Configuration.cs, etc.)
2. **📋 Follow established patterns** - Use existing service structure as template
3. **Follow DI patterns** - Use constructor injection and proper lifetimes
4. **Use stored procedures** - Via Database service and Helper_Database_StoredProcedure
5. **Implement proper error handling** - Use ErrorHandling service for consistent error management

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

public static class Program
{
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
```

### Current Command Infrastructure
```csharp
// Standard ICommand implementations available
 public class AsyncCommand : ICommand
 {
     private readonly Func<Task> _execute;
     private readonly Func<bool>? _canExecute;
     // Implementation in InventoryTabViewModel.cs
 }
 
 public class RelayCommand : ICommand
 {
     private readonly Action _execute;
     private readonly Func<bool>? _canExecute;
     // Implementation in InventoryTabViewModel.cs
 }
```

</details>

<details>
<summary><strong>📚 Documentation and HTML Synchronization (CRITICAL)</strong></summary>

### Instruction File Formatting Rule (CRITICAL)
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
1. **Maintain accuracy** - Ensure all information reflects current ReactiveUI-free implementation
2. **Update cross-references** - Update all related links and references to match new patterns
3. **Follow naming conventions** - Use established file naming patterns
4. **Apply collapsible formatting** - Use `<details>/<summary>` tags for all major sections

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

</details>

<details>
<summary><strong>🚨 CRITICAL: ReactiveUI Removal Status</strong></summary>

### ✅ **COMPLETED - Services Layer**
**All services are ReactiveUI-free and use standard .NET patterns:**
- ✅ **ErrorHandling.cs**: Comprehensive error handling without ReactiveUI
- ✅ **Configuration.cs**: Configuration and application state with INotifyPropertyChanged
- ✅ **Navigation.cs**: Simple navigation service with standard patterns
- ✅ **Database.cs**: Complete database access with Helper_Database_StoredProcedure

### ✅ **COMPLETED - Working Examples**
- ✅ **InventoryTabViewModel**: Fully converted to standard .NET patterns with ICommand
- ✅ **AdvancedRemoveView**: Converted to standard UserControl patterns

### ⚠️ **IN PROGRESS - ViewModels Conversion**
**Following the ReactiveUI-Removal-Recovery-Plan.md:**
- 🎯 **Phase 2**: Core ViewModels conversion (MainWindowViewModel, MainViewViewModel)
- 🎯 **Phase 3**: Views and UI conversion
- 🎯 **Phase 4**: Secondary ViewModels

### 🎯 **Standard .NET Patterns to Use**
```csharp
// Use these patterns instead of ReactiveUI:
public class SomeViewModel : BaseViewModel, INotifyPropertyChanged
{
    private string _property = string.Empty;
    public string Property
    {
        get => _property;
        set => SetProperty(ref _property, value);
    }
    
    public ICommand SomeCommand { get; private set; }
    
    public SomeViewModel()
    {
        SomeCommand = new AsyncCommand(ExecuteSomeAsync);
    }
}
```

**NO LONGER USE:**
- ❌ ReactiveObject
- ❌ ReactiveCommand<Unit, Unit>
- ❌ this.RaiseAndSetIfChanged()
- ❌ this.WhenActivated()
- ❌ ReactiveUserControl<T>

</details>
