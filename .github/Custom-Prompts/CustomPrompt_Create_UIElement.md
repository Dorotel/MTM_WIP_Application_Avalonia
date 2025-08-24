# Custom Prompt: Create UI Element

## ?? **Instructions**
Use this prompt when you need to generate Avalonia UI controls or views based on component mapping and instruction files. This prompt creates basic UI elements with proper AXAML structure, data binding, and MTM styling patterns.

## ?? **Persona**
**UI Generation Copilot** - Specializes in creating Avalonia AXAML controls and views with proper data binding, styling, and MTM design patterns.

## ?? **Prompt Template**

```
Act as UI Generation Copilot. Create an Avalonia UI element for [ELEMENT_NAME] with the following requirements:

**Element Type:** [UserControl/Window/Custom Control]
**Purpose:** [Describe what this element does]
**Layout:** [Grid/StackPanel/DockPanel - describe layout structure]
**Controls:** [List specific controls needed - Button, TextBox, DataGrid, etc.]
**Styling:** Apply MTM purple theme (#4B45ED primary, #BA45ED accents)
**Bindings:** [Specify data binding requirements]

**Requirements:**
- Use Avalonia AXAML with compiled bindings (x:CompileBindings="True")
- Follow MTM design patterns with cards, proper spacing, and modern layout
- Apply purple brand colors consistently
- Include proper margins (8px containers, 4px controls) and card padding (24px)
- Use DynamicResource for theme support
- Generate clean, readable AXAML with proper indentation

**Additional Context:** [Any specific requirements or constraints]
```

## ?? **Purpose**
This prompt generates basic Avalonia UI elements that follow MTM design standards, proper AXAML structure, and modern layout principles. It's designed for creating individual UI components rather than complete views or complex layouts.

## ?? **Usage Examples**

### **Example 1: Creating a Simple Card Component**
```
Act as UI Generation Copilot. Create an Avalonia UI element for InventoryCard with the following requirements:

**Element Type:** UserControl
**Purpose:** Display inventory item information in a card format
**Layout:** Grid with header row and content area
**Controls:** TextBlock for title, TextBlock for quantity, Button for actions
**Styling:** Apply MTM purple theme (#4B45ED primary, #BA45ED accents)
**Bindings:** Title="{Binding PartId}", Content="{Binding Quantity}", Command="{Binding ViewDetailsCommand}"

**Requirements:**
- Use Avalonia AXAML with compiled bindings (x:CompileBindings="True")
- Follow MTM design patterns with cards, proper spacing, and modern layout
- Apply purple brand colors consistently
- Include proper margins (8px containers, 4px controls) and card padding (24px)
- Use DynamicResource for theme support
- Generate clean, readable AXAML with proper indentation

**Additional Context:** Card should have subtle shadow and rounded corners for modern appearance
```

### **Example 2: Creating a Data Entry Form**
```
Act as UI Generation Copilot. Create an Avalonia UI element for PartEntryForm with the following requirements:

**Element Type:** UserControl
**Purpose:** Allow user to enter part information for inventory
**Layout:** Grid with label/input pairs in rows
**Controls:** TextBox for PartId, TextBox for Operation, NumericUpDown for Quantity, ComboBox for Location
**Styling:** Apply MTM purple theme (#4B45ED primary, #BA45ED accents)
**Bindings:** Two-way binding to ViewModel properties

**Requirements:**
- Use Avalonia AXAML with compiled bindings (x:CompileBindings="True")
- Follow MTM design patterns with cards, proper spacing, and modern layout
- Apply purple brand colors consistently
- Include proper margins (8px containers, 4px controls) and card padding (24px)
- Use DynamicResource for theme support
- Generate clean, readable AXAML with proper indentation

**Additional Context:** Form should be contained in a card with clear visual separation between input fields
```

## ?? **Guidelines**

### **Technical Requirements**
- Always use `x:CompileBindings="True"` and `x:DataType` for compiled bindings
- Include proper xmlns declarations for Avalonia
- Use Grid layouts for better performance over StackPanel when possible
- Apply MTM color scheme using DynamicResource references
- Include proper spacing and margins following MTM standards

### **MTM-Specific Patterns**
- **Colors**: Use #4B45ED for primary, #BA45ED for accents, #8345ED for secondary
- **Spacing**: 8px for container margins, 4px for control margins, 24px for card padding
- **Layout**: Prefer Grid over StackPanel, use cards for content grouping
- **Typography**: Use appropriate font sizes (20-28px for headers, 14-16px for content)
- **Shadows**: Apply subtle box shadows for depth `BoxShadow="0 2 8 0 #11000000"`

### **Data Binding Standards**
- Use compiled bindings with proper DataType specification
- Apply two-way binding for input controls: `{Binding Property, Mode=TwoWay}`
- Use one-way binding for display: `{Binding Property}`
- Bind commands to ReactiveCommand properties: `{Binding CommandName}`

## ?? **Related Files**
- [../UI-Instructions/ui-generation.instruction.md](../UI-Instructions/ui-generation.instruction.md) - Complete UI generation guidelines
- [../Core-Instructions/naming.conventions.instruction.md](../Core-Instructions/naming.conventions.instruction.md) - Naming standards for controls and properties
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - UI Generation Copilot persona details

## ? **Quality Checklist**

### **AXAML Structure**
- [ ] Proper xmlns declarations included
- [ ] x:CompileBindings="True" and x:DataType specified
- [ ] Clean, readable indentation and formatting
- [ ] Proper control hierarchy and layout structure

### **MTM Design Compliance**
- [ ] MTM purple color scheme applied using DynamicResource
- [ ] Proper spacing and margins (8px containers, 4px controls)
- [ ] Card-based layout with 24px padding where appropriate
- [ ] Modern layout patterns (Grid preferred over StackPanel)

### **Data Binding**
- [ ] All bindings use compiled binding syntax
- [ ] Two-way binding applied to input controls
- [ ] Commands bound to ReactiveCommand properties
- [ ] DataType matches ViewModel interface

### **Code Quality**
- [ ] No hard-coded colors (use DynamicResource)
- [ ] Proper control naming if x:Name is used
- [ ] Consistent with MTM naming conventions
- [ ] No business logic in code-behind (UI only)