# Create UI Element from Markdown Instructions - Custom Prompt

## Instructions
Use this prompt when you need to generate Avalonia AXAML and ReactiveUI ViewModels from parsed markdown instruction files with component hierarchies.

## ⚠️ CRITICAL: AVLN2000 Error Prevention
**BEFORE using this prompt, ALWAYS consult [../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

## Persona
**UI Architect Copilot + ReactiveUI Specialist**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Create UI Element from [filename].md following MTM-specific UI generation guidelines and AVLN2000 prevention rules.  
Parse the markdown component structure and generate both AXAML view and ReactiveUI ViewModel.  
Include MTM data patterns (Part ID, Operation numbers), context menu integration, and space optimization.  
Follow the MTM purple color scheme and modern layout patterns. Leave business logic as TODO comments.
```

## Purpose
For generating Avalonia AXAML and ReactiveUI ViewModels from parsed markdown files with component hierarchies while preventing AVLN2000 errors.

## Usage Examples

### Example 1: Quick Buttons Component
```
Create UI Element from Control_QuickButtons.instructions.md following MTM-specific UI generation guidelines and AVLN2000 prevention rules.  
Parse the markdown component structure and generate both AXAML view and ReactiveUI ViewModel.  
Include MTM data patterns (Part ID, Operation numbers), context menu integration, and space optimization.  
Follow the MTM purple color scheme and modern layout patterns. Leave business logic as TODO comments.
```

### Example 2: Inventory Tab Component
```
Create UI Element from Control_InventoryTab.instructions.md following MTM-specific UI generation guidelines and AVLN2000 prevention rules.  
Parse the markdown component structure and generate both AXAML view and ReactiveUI ViewModel.  
Include MTM data patterns (Part ID, Operation numbers), context menu integration, and space optimization.  
Follow the MTM purple color scheme and modern layout patterns. Leave business logic as TODO comments.
```

## Guidelines

### AVLN2000 Prevention Requirements
1. **Use Avalonia AXAML syntax** - Never WPF XAML
2. **Grid Definitions**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
3. **No Grid Names**: Never use `Name` property on ColumnDefinition/RowDefinition
4. **Correct Namespaces**: Use `xmlns="https://github.com/avaloniaui"`
5. **Control Equivalents**: Use Avalonia controls (TextBlock instead of Label)

### Markdown Parsing Requirements
1. **Extract UI Element Name**: Use as base for file naming
2. **Parse Component Structure**: Convert hierarchy to Avalonia AXAML
3. **Identify Props/Inputs**: Create ViewModel properties
4. **Map Visual Representation**: Apply layout and styling
5. **Extract Related Controls**: Create integration points

### Generated File Structure
```
Views/{Name}View.axaml - Avalonia UI markup (AVLN2000-safe)
ViewModels/{Name}ViewModel.cs - ReactiveUI ViewModel
```

### Component Hierarchy Mapping Example
**Markdown Structure:**
```
Control_QuickButtons
├── quickButtons List<Button> (10 buttons maximum)
│   ├── Button[0] - Position 1: (Operation) - [PartID x Quantity]
│   └── Button[9] - Position 10: (Operation) - [PartID x Quantity]
└── Context Menu (Right-click)
    ├── Edit Button
    └── Remove Button
```

**Generated Avalonia Structure (AVLN2000-Safe):**
```xml
<!-- CORRECT: Avalonia AXAML syntax -->
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate DataType="vm:QuickButtonItemViewModel">
            <Button Classes="quick-button">
                <Button.ContextMenu>
                    <ContextMenu>
                        <MenuItem Header="Edit Button"/>
                        <MenuItem Header="Remove Button"/>
                    </ContextMenu>
                </Button.ContextMenu>
            </Button>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### MTM Data Pattern Integration
- **Part ID**: String (e.g., "PART001")
- **Operation**: String numbers (e.g., "90", "100", "110") - workflow step identifiers
- **Quantity**: Integer
- **Position**: 1-based indexing for UI display
- **TransactionType**: Determined by user intent, NOT operation number

### Business Logic Integration Points
```csharp
// Leave as TODO comments with stored procedure pattern
// TODO: Implement database loading via stored procedure
// var dataResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     Model_AppVariables.ConnectionString,
//     "sys_last_10_transactions_Get_ByUser",
//     new Dictionary<string, object> { ["User"] = currentUser }
// );
```

### Event-Driven Communication
```csharp
// Events for parent-child communication
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct method calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation, // Just a workflow step number
    Quantity = button.Quantity
});
```

### Context Menu Integration
```xml
<Button.ContextMenu>
    <ContextMenu>
        <MenuItem Header="Edit Button" Command="{Binding EditCommand}"/>
        <MenuItem Header="Remove Button" Command="{Binding RemoveCommand}"/>
        <Separator/>
        <MenuItem Header="Clear All" Command="{Binding ClearAllCommand}"/>
        <MenuItem Header="Refresh" Command="{Binding RefreshCommand}"/>
    </ContextMenu>
</Button.ContextMenu>
```

### Space Optimization Patterns
- Use `UniformGrid` for equal distribution
- Implement `VerticalAlignment="Stretch"` for full height usage
- Remove `ScrollViewer` when all items fit in available space
- Increase font sizes and padding when more space is available

### Progress Integration
```csharp
// TODO: Inject services
// private readonly IProgressService _progressService;

// Commands with progress support
LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
{
    // TODO: Show progress during operation
    await LoadLast10TransactionsAsync();
});
```

## Technical Requirements

### AXAML View Template (AVLN2000-Safe)
```xml
<!-- CORRECT: Avalonia AXAML with proper namespaces -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:YourApp.ViewModels"
             x:Class="YourApp.Views.{Name}View"
             x:DataType="vm:{Name}ViewModel"
             x:CompileBindings="True">
    
    <!-- CORRECT: Use attribute syntax for simple grids -->
    <Grid RowDefinitions="*,Auto" ColumnDefinitions="Auto,*">
        <!-- UI Elements here -->
    </Grid>
</UserControl>
```

### ViewModel Template
```csharp
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace YourApp.ViewModels;

public class {Name}ViewModel : ReactiveObject
{
    // Observable properties with RaiseAndSetIfChanged
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    // Commands with error handling
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    
    public {Name}ViewModel()
    {
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            // TODO: Implement via stored procedure
            await Task.CompletedTask;
        });

        // Centralized error handling
        LoadDataCommand.ThrownExceptions
            .Subscribe(ex =>
            {
                // TODO: Log and present user-friendly error
            });
    }
}
```

## Related Files
- **[../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md)** - **CRITICAL**: AVLN2000 error prevention
- `Documentation/Development/UI_Documentation/Controls/` - Component instruction files
- `.github/ui-generation.instruction.md` - UI generation guidelines
- `.github/copilot-instructions.md` - MTM business logic rules
- `.github/naming.conventions.instruction.md` - Naming standards

## Quality Checklist
- [ ] **AVLN2000 Prevention**: Avalonia AXAML syntax used (not WPF)
- [ ] **Grid Syntax**: No `Name` properties on Grid definitions
- [ ] **Namespaces**: Correct Avalonia namespace used
- [ ] Markdown structure parsed correctly
- [ ] Component hierarchy mapped to Avalonia
- [ ] MTM data patterns implemented
- [ ] Context menus added where appropriate
- [ ] Space optimization applied
- [ ] MTM color scheme used
- [ ] Business logic left as TODO comments
- [ ] ReactiveUI patterns implemented
- [ ] Compiled bindings enabled
- [ ] Event-driven communication patterns included
