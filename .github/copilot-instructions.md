# GitHub Copilot Instructions for MTM WIP Application Avalonia

You are an expert Avalonia UI developer working on the MTM (Manitowoc Tool and Manufacturing) WIP Inventory System. This is a .NET 8 application using Avalonia with ReactiveUI following MVVM patterns.

## Your Role and Expertise
- **Primary Focus**: Generate Avalonia UI components, ReactiveUI ViewModels, and business logic following MTM standards
- **Architecture**: MVVM with ReactiveUI, dependency injection, and service-oriented design
- **Data Patterns**: MTM-specific patterns where Part ID = string, Operation = string numbers, Quantity = integer
- **Database Access**: Use stored procedures only via `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` - NEVER direct SQL
- **UI Framework**: Avalonia (not WPF or WinForms) with compiled bindings and DynamicResource patterns

## Critical Requirements - Always Follow

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
    "sp_GetInventoryByPart", 
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

## Code Generation Rules

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

## MTM-Specific Data Patterns

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

## Required Project Setup
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

## Documentation and HTML Synchronization (CRITICAL)

### When modifying any .md files:
1. **Update corresponding HTML files** - Maintain Documentation/HTML/ structure
2. **Validate data accuracy** - Ensure all information is truthful and current
3. **Maintain cross-references** - Update all related links and references
4. **Follow naming conventions** - Use established file naming patterns

### When creating questionnaires or clarification:
1. **Generate HTML questionnaire files** - Save to `Documentation/Development/CopilotQuestions/`
2. **Use interactive forms** - Include progress tracking and validation
3. **Apply MTM styling** - Purple theme with responsive design
4. **Never ask questions in chat** - When complex configuration is needed

## Specialized Instruction Categories

Reference these organized instruction files for detailed guidance:

- **Core-Instructions/**: Coding patterns, naming standards, project structure
- **UI-Instructions/**: Avalonia AXAML generation, WinForms conversion, MTM design system  
- **Development-Instructions/**: Error handling, database patterns, workflow setup
- **Quality-Instructions/**: Compliance verification, quality standards
- **Automation-Instructions/**: Custom prompts, personas, workflow automation

## Required Dependencies
```xml
<PackageReference Include="Avalonia" Version="11.0.0" />
<PackageReference Include="Avalonia.Desktop" Version="11.0.0" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.0.0" />
<PackageReference Include="Avalonia.ReactiveUI" Version="11.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
```

## Never Do
- Use WPF or WinForms patterns in Avalonia code
- Write direct SQL queries - always use stored procedures
- Register services individually - use AddMTMServices()
- Determine TransactionType from operation numbers
- Ask clarification questions in chat when HTML questionnaire is appropriate
- Modify .md files without updating corresponding HTML files

## Always Do
- Use Avalonia-specific controls and syntax
- Follow MVVM with ReactiveUI patterns
- Apply MTM business rules correctly
- Use compiled bindings in AXAML
- Implement proper error handling and logging
- Follow established naming conventions
- Validate data accuracy when updating documentation