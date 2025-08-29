# Create UI Element - Custom Prompt

## Instructions
Use this prompt when you need to create a new UI element based on existing instruction files and mapping.

## ⚠️ CRITICAL: AVLN2000 Error Prevention
**BEFORE using this prompt, ALWAYS consult [../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

## Persona
**UI Architect Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Create a new UI element for [source] using the mapped .instructions.md and screenshot.  
Follow AVLN2000 prevention guidelines and all naming and UI layout conventions. Only include navigation logic and event stubs as per our standards.
```

## Purpose
For generating Avalonia controls or views based on mapping and instructions while preventing AVLN2000 errors.

## Usage Examples

### Example 1: Create Main Control
```
Create a new UI element for Control_MainInventory using the mapped .instructions.md and screenshot.  
Follow AVLN2000 prevention guidelines and all naming and UI layout conventions. Only include navigation logic and event stubs as per our standards.
```

### Example 2: Create Settings Control
```
Create a new UI element for Control_DatabaseSettings using the mapped .instructions.md and screenshot.  
Follow AVLN2000 prevention guidelines and all naming and UI layout conventions. Only include navigation logic and event stubs as per our standards.
```

## Guidelines

### What This Prompt Creates
- Avalonia AXAML view files (AVLN2000-safe)
- Standard .NET ViewModel classes with INotifyPropertyChanged
- Proper MVVM binding structure
- Navigation event handlers only
- Empty stubs for business logic

### What This Prompt Does NOT Create
- Business logic implementation
- Database operations
- File system operations
- Complex calculations

### Technical Requirements
- **AVLN2000 Prevention**: Use Avalonia AXAML syntax, never WPF XAML
- Must use `.instructions.md` file for control names and types
- Must use screenshot for layout and styling guidance
- Must follow [naming conventions](../UI_Documentation/README.md)
- Must implement compiled bindings with `x:CompileBindings="True"`
- Must use standard .NET MVVM patterns (INotifyPropertyChanged, ICommand)
- **Grid Syntax**: Use `ColumnDefinitions="Auto,*"` attribute form when possible
- **No Grid Names**: Never use `Name` property on ColumnDefinition/RowDefinition

### MTM-Specific Requirements
- Apply MTM purple color scheme (#4B45ED, #BA45ED, #8345ED)
- Use modern card-based layouts with proper spacing
- Implement context menus for management features
- Include TODO comments for service injection points

## Related Files
- **[../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md)** - **CRITICAL**: AVLN2000 error prevention
- `.github/ui-generation.instruction.md` - UI generation guidelines
- `.github/naming.conventions.instruction.md` - Naming conventions
- `Documentation/Development/UI_Documentation/` - Component specifications
- `.github/personas.instruction.md` - Persona behavioral guidelines

## Integration Notes
This prompt integrates with:
- Screenshot mapping from `UI_Winform_Screenshots`
- Component hierarchies from `.instructions.md` files
- MTM design system and color schemes
- Standard .NET MVVM and Avalonia UI frameworks
- AVLN2000 error prevention guidelines

## Quality Checklist
- [ ] **AVLN2000 Prevention**: Avalonia AXAML syntax used (not WPF)
- [ ] **Grid Syntax**: No `Name` properties on Grid definitions
- [ ] **Namespaces**: Correct Avalonia namespace used
- [ ] Control names match `.instructions.md` specifications
- [ ] Layout matches screenshot reference
- [ ] Navigation logic implemented correctly
- [ ] Event stubs created with TODO comments
- [ ] Naming conventions followed consistently
- [ ] MTM design patterns applied
- [ ] Compiled bindings implemented
- [ ] Standard .NET MVVM patterns used correctly
