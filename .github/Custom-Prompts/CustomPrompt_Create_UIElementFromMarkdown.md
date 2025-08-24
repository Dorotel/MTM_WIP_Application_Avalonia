# Custom Prompt: Create UI Element from Markdown Instructions

## ?? **Instructions**
Use this prompt when you need to generate Avalonia AXAML and ReactiveUI ViewModels from markdown instruction files that describe UI component hierarchies. This prompt parses markdown structure and converts it to proper Avalonia UI components with MTM design patterns.

## ?? **Persona**
**Markdown UI Parser Copilot** - Specializes in parsing markdown component specifications and generating corresponding Avalonia AXAML views and ReactiveUI ViewModels with proper data binding and MTM patterns.

## ?? **Prompt Template**

```
Act as Markdown UI Parser Copilot. Create Avalonia UI components from the markdown instruction file [FILENAME].md with the following requirements:

**Source File:** [Path to markdown file with component specification]
**Component Type:** [Extract from markdown - UserControl/Window/Custom Control]
**Target Files:** 
- Views/[NAME]View.axaml
- ViewModels/[NAME]ViewModel.cs

**Parse Requirements:**
- Extract component hierarchy from markdown tree structure
- Convert component descriptions to appropriate Avalonia controls
- Map data properties to observable ViewModel properties
- Generate commands for all user interactions mentioned
- Apply MTM design patterns and styling

**Component Structure Parsing:**
- Parse tree structure (??? ???) to determine control hierarchy
- Map control types (Button, TextBox, DataGrid, etc.) to Avalonia equivalents
- Extract data binding requirements from property descriptions
- Identify event handlers and convert to ReactiveUI commands
- Determine layout patterns from structural relationships

**MTM Design Integration:**
- Apply purple brand colors (#4B45ED primary, #BA45ED accents)
- Use card-based layouts with proper spacing (24px padding)
- Implement context menus for management features
- Follow space optimization patterns (UniformGrid, proper alignment)
- Apply responsive design principles

**Code Generation:**
- Create AXAML with compiled bindings (x:CompileBindings="True")
- Generate ReactiveUI ViewModel with proper observable patterns
- Leave business logic as TODO comments with stored procedure patterns
- Include event-driven communication for parent-child relationships
- Apply MTM data patterns (Part ID strings, Operation numbers, etc.)

**Additional Context:** [Any specific parsing requirements or modifications needed]
```

## ?? **Purpose**
This prompt generates complete Avalonia UI components (View + ViewModel) from markdown specifications, ensuring proper component hierarchy mapping, MTM design consistency, and ReactiveUI patterns while maintaining the intent described in the markdown documentation.

## ?? **Usage Examples**

### **Example 1: Parsing a Quick Button Component**
```
Act as Markdown UI Parser Copilot. Create Avalonia UI components from the markdown instruction file Control_QuickButtons.md with the following requirements:

**Source File:** Development/UI_Documentation/Control_QuickButtons.md
**Component Type:** UserControl
**Target Files:** 
- Views/QuickButtonsView.axaml
- ViewModels/QuickButtonsViewModel.cs

**Parse Requirements:**
- Extract component hierarchy from markdown tree structure
- Convert component descriptions to appropriate Avalonia controls
- Map data properties to observable ViewModel properties
- Generate commands for all user interactions mentioned
- Apply MTM design patterns and styling

**Component Structure Parsing:**
The markdown shows:
```
Control_QuickButtons
??? quickButtons List<Button> (10 buttons maximum)
?   ??? Button[0] - Position 1: (Operation) - [PartID x Quantity]
?   ??? Button[9] - Position 10: (Operation) - [PartID x Quantity]
??? Context Menu (Right-click)
    ??? Edit Button
    ??? Remove Button
```

**MTM Design Integration:**
- Apply purple brand colors (#4B45ED primary, #BA45ED accents)
- Use card-based layouts with proper spacing (24px padding)
- Implement context menus for management features
- Follow space optimization patterns (UniformGrid, proper alignment)
- Apply responsive design principles

**Code Generation:**
- Create AXAML with compiled bindings (x:CompileBindings="True")
- Generate ReactiveUI ViewModel with proper observable patterns
- Leave business logic as TODO comments with stored procedure patterns
- Include event-driven communication for parent-child relationships
- Apply MTM data patterns (Part ID strings, Operation numbers, etc.)

**Additional Context:** Quick buttons should populate other controls when clicked, not switch tabs. Use UniformGrid for equal distribution.
```

### **Example 2: Parsing a Data Grid Component**
```
Act as Markdown UI Parser Copilot. Create Avalonia UI components from the markdown instruction file Control_TransactionHistory.md with the following requirements:

**Source File:** Development/UI_Documentation/Control_TransactionHistory.md
**Component Type:** UserControl
**Target Files:** 
- Views/TransactionHistoryView.axaml
- ViewModels/TransactionHistoryViewModel.cs

**Parse Requirements:**
- Extract component hierarchy from markdown tree structure
- Convert component descriptions to appropriate Avalonia controls
- Map data properties to observable ViewModel properties
- Generate commands for all user interactions mentioned
- Apply MTM design patterns and styling

**Component Structure Parsing:**
- Parse tree structure (??? ???) to determine control hierarchy
- Map control types (Button, TextBox, DataGrid, etc.) to Avalonia equivalents
- Extract data binding requirements from property descriptions
- Identify event handlers and convert to ReactiveUI commands
- Determine layout patterns from structural relationships

**MTM Design Integration:**
- Apply purple brand colors (#4B45ED primary, #BA45ED accents)
- Use card-based layouts with proper spacing (24px padding)
- Implement context menus for management features
- Follow space optimization patterns (UniformGrid, proper alignment)
- Apply responsive design principles

**Code Generation:**
- Create AXAML with compiled bindings (x:CompileBindings="True")
- Generate ReactiveUI ViewModel with proper observable patterns
- Leave business logic as TODO comments with stored procedure patterns
- Include event-driven communication for parent-child relationships
- Apply MTM data patterns (Part ID strings, Operation numbers, etc.)

**Additional Context:** Transaction history should support filtering, sorting, and export functionality with progress tracking integration.
```

## ?? **Guidelines**

### **Markdown Parsing Patterns**
- **Tree Structure**: Parse `???` and `???` symbols to determine component hierarchy
- **Control Types**: Map control names (Button, TextBox, DataGrid) to Avalonia equivalents
- **Data Properties**: Extract property descriptions and convert to observable properties
- **Events**: Convert event descriptions to ReactiveUI commands
- **Layout Hints**: Use structural relationships to determine Grid/StackPanel/DockPanel usage

### **Component Hierarchy Mapping**
```
Markdown Tree ? Avalonia Structure

Control_Name                    ? UserControl
??? container List<Type>        ? ItemsControl with ItemsPanel
?   ??? Item[0] - properties   ? DataTemplate with bindings
?   ??? Item[n] - properties   ? DataTemplate continuation
??? Context Menu               ? ContextMenu with MenuItems
    ??? Menu Item 1           ? MenuItem with Command binding
    ??? Menu Item 2           ? MenuItem with Command binding
```

### **MTM Data Pattern Integration**
- **Part ID**: String properties (e.g., "PART001")
- **Operation**: String number properties (e.g., "90", "100", "110")
- **Quantity**: Integer properties with validation
- **Position**: 1-based indexing for UI display
- **TransactionType**: Determined by user intent, not operation numbers

### **Event-Driven Communication**
```csharp
// Parent-child communication pattern
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct method calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation,
    Quantity = button.Quantity
});
```

### **Space Optimization Patterns**
- Use `UniformGrid` for equal distribution when components are removed
- Implement `VerticalAlignment="Stretch"` for full height usage
- Remove `ScrollViewer` when all items fit in available space
- Increase font sizes and padding when more space is available

## ?? **Related Files**
- [../UI-Instructions/ui-generation.instruction.md](../UI-Instructions/ui-generation.instruction.md) - Complete UI generation guidelines
- [../Development-Instructions/templates-documentation.instruction.md](../Development-Instructions/templates-documentation.instruction.md) - Markdown parsing and documentation standards
- [../Core-Instructions/naming.conventions.instruction.md](../Core-Instructions/naming.conventions.instruction.md) - Naming standards for generated components
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - Markdown UI Parser Copilot persona details

## ? **Quality Checklist**

### **Markdown Parsing Accuracy**
- [ ] Component hierarchy correctly extracted from tree structure
- [ ] All control types properly mapped to Avalonia equivalents
- [ ] Data properties converted to observable ViewModel properties
- [ ] Events converted to ReactiveUI commands
- [ ] Layout structure reflects markdown organization

### **Avalonia Implementation**
- [ ] AXAML uses compiled bindings with x:DataType specification
- [ ] Proper xmlns declarations and control hierarchy
- [ ] MTM design patterns applied (cards, spacing, colors)
- [ ] Context menus implemented where specified
- [ ] Responsive design principles followed

### **ReactiveUI ViewModel**
- [ ] Inherits from ReactiveObject
- [ ] Observable properties use RaiseAndSetIfChanged
- [ ] Commands are ReactiveCommand instances
- [ ] Event communication implemented for parent-child relationships
- [ ] Business logic left as TODO comments

### **MTM Compliance**
- [ ] Part IDs treated as strings
- [ ] Operations treated as string numbers (workflow steps)
- [ ] Purple brand colors applied consistently
- [ ] Space optimization patterns implemented
- [ ] 1-based indexing used for UI positions