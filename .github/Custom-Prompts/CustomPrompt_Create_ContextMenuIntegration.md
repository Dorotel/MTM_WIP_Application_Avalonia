# Custom Prompt: Create Context Menu Integration

## ?? **Instructions**
Use this prompt when you need to add context menus to components with management features following MTM patterns. This prompt creates right-click functionality for component management, editing, and organization.

## ?? **Persona**
**UI Enhancement Copilot** - Specializes in adding context menus, interactive features, and component management functionality to Avalonia UI elements with proper MTM design integration.

## ?? **Prompt Template**

```
Act as UI Enhancement Copilot. Add context menu integration to [COMPONENT_NAME] with the following requirements:

**Component Type:** [Button collection | Data grid | List view | Card layout]
**Menu Actions:** [Edit | Remove | Copy | Move | Configure | Custom actions]
**Management Features:** [Add new items | Bulk operations | Quick actions | Settings]
**User Experience:** [Right-click activation | Keyboard shortcuts | Touch-friendly options]

**Context Menu Requirements:**
- Create context menu with appropriate management actions
- Implement proper command binding to ReactiveUI commands
- Support conditional menu item visibility based on context
- Include keyboard shortcuts for accessibility
- Apply MTM design styling and branding
- Support both mouse and touch interaction patterns

**Technical Implementation:**
- Generate AXAML context menu with proper structure
- Create corresponding ViewModel commands for all actions
- Implement proper data context passing for menu actions
- Include visual separators and menu organization
- Support async operations with loading states
- Prepare for dependency injection of required services

**MTM-Specific Features:**
- Support quick button management (Edit, Remove, Clear All)
- Include inventory item context actions
- Support operation workflow context menus
- Apply purple brand styling to menu items
- Include confirmation dialogs for destructive actions

**Additional Context:** [Any specific menu requirements or business logic]
```

## ?? **Purpose**
This prompt generates context menu functionality that enhances component usability by providing right-click management features, quick actions, and administrative capabilities while maintaining MTM design consistency.

## ?? **Usage Examples**

### **Example 1: Quick Button Context Menu**
```
Act as UI Enhancement Copilot. Add context menu integration to QuickButtonCollection with the following requirements:

**Component Type:** Button collection
**Menu Actions:** Edit Button, Remove Button, Clear All, Refresh
**Management Features:** Add new quick buttons, bulk clear operations, configuration access
**User Experience:** Right-click activation with keyboard shortcuts

**Context Menu Requirements:**
- Create context menu with appropriate management actions
- Implement proper command binding to ReactiveUI commands
- Support conditional menu item visibility based on context
- Include keyboard shortcuts for accessibility
- Apply MTM design styling and branding
- Support both mouse and touch interaction patterns

**Technical Implementation:**
- Generate AXAML context menu with proper structure
- Create corresponding ViewModel commands for all actions
- Implement proper data context passing for menu actions
- Include visual separators and menu organization
- Support async operations with loading states
- Prepare for dependency injection of required services

**MTM-Specific Features:**
- Support quick button management (Edit, Remove, Clear All)
- Include inventory item context actions
- Support operation workflow context menus
- Apply purple brand styling to menu items
- Include confirmation dialogs for destructive actions

**Additional Context:** Quick buttons should allow individual management and bulk operations, with confirmation for destructive actions
```

### **Example 2: Inventory Data Grid Context Menu**
```
Act as UI Enhancement Copilot. Add context menu integration to InventoryDataGrid with the following requirements:

**Component Type:** Data grid
**Menu Actions:** View Details, Edit Item, Remove Item, Copy to Clipboard, Export Row
**Management Features:** Bulk selection operations, filtering shortcuts, column management
**User Experience:** Right-click activation with selection-aware actions

**Context Menu Requirements:**
- Create context menu with appropriate management actions
- Implement proper command binding to ReactiveUI commands
- Support conditional menu item visibility based on context
- Include keyboard shortcuts for accessibility
- Apply MTM design styling and branding
- Support both mouse and touch interaction patterns

**Technical Implementation:**
- Generate AXAML context menu with proper structure
- Create corresponding ViewModel commands for all actions
- Implement proper data context passing for menu actions
- Include visual separators and menu organization
- Support async operations with loading states
- Prepare for dependency injection of required services

**MTM-Specific Features:**
- Support quick button management (Edit, Remove, Clear All)
- Include inventory item context actions
- Support operation workflow context menus
- Apply purple brand styling to menu items
- Include confirmation dialogs for destructive actions

**Additional Context:** Data grid context menu should be selection-aware and provide different options for single vs. multiple selection
```

## ?? **Guidelines**

### **Context Menu Structure**
```xml
<Button.ContextMenu>
    <ContextMenu>
        <!-- Primary Actions -->
        <MenuItem Header="Edit Item" 
                  Command="{Binding EditCommand}"
                  CommandParameter="{Binding .}"
                  InputGesture="F2">
            <MenuItem.Icon>
                <PathIcon Data="{StaticResource EditIcon}" />
            </MenuItem.Icon>
        </MenuItem>
        
        <MenuItem Header="Remove Item" 
                  Command="{Binding RemoveCommand}"
                  CommandParameter="{Binding .}"
                  InputGesture="Delete">
            <MenuItem.Icon>
                <PathIcon Data="{StaticResource DeleteIcon}" />
            </MenuItem.Icon>
        </MenuItem>
        
        <Separator/>
        
        <!-- Secondary Actions -->
        <MenuItem Header="Copy" 
                  Command="{Binding CopyCommand}"
                  CommandParameter="{Binding .}"
                  InputGesture="Ctrl+C" />
        
        <Separator/>
        
        <!-- Bulk Actions -->
        <MenuItem Header="Clear All" 
                  Command="{Binding ClearAllCommand}" />
        
        <MenuItem Header="Refresh" 
                  Command="{Binding RefreshCommand}"
                  InputGesture="F5" />
    </ContextMenu>
</Button.ContextMenu>
```

### **ViewModel Command Implementation**
```csharp
public ReactiveCommand<object, Unit> EditCommand { get; }
public ReactiveCommand<object, Unit> RemoveCommand { get; }
public ReactiveCommand<object, Unit> CopyCommand { get; }
public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }
public ReactiveCommand<Unit, Unit> RefreshCommand { get; }

public ConstructorName()
{
    // Edit command with parameter
    EditCommand = ReactiveCommand.Create<object>(item =>
    {
        if (item is TargetItemType targetItem)
        {
            // TODO: Implement edit functionality
            // Navigate to edit view or show edit dialog
        }
    });

    // Remove command with confirmation
    RemoveCommand = ReactiveCommand.CreateFromTask<object>(async item =>
    {
        if (item is TargetItemType targetItem)
        {
            // TODO: Show confirmation dialog
            // await _dialogService.ShowConfirmationAsync("Remove item?");
            // Remove item from collection
        }
    });

    // Bulk operations
    ClearAllCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        // TODO: Show confirmation for bulk operation
        // Clear all items with user confirmation
    });
}
```

### **Conditional Menu Visibility**
```xml
<MenuItem Header="Advanced Options" 
          IsVisible="{Binding IsAdvancedMode}"
          Command="{Binding AdvancedCommand}" />

<MenuItem Header="Admin Functions" 
          IsVisible="{Binding CurrentUser.IsAdmin}"
          Command="{Binding AdminCommand}" />
```

### **MTM Styling Integration**
```xml
<ContextMenu.Resources>
    <Style TargetType="MenuItem">
        <Setter Property="Foreground" Value="{DynamicResource ForegroundBrush}"/>
        <Setter Property="Background" Value="{DynamicResource BackgroundBrush}"/>
    </Style>
    <Style TargetType="MenuItem" x:Key="DangerMenuItem">
        <Setter Property="Foreground" Value="{DynamicResource PinkAccentBrush}"/>
    </Style>
</ContextMenu.Resources>

<!-- Destructive action with warning styling -->
<MenuItem Header="Delete All" 
          Style="{StaticResource DangerMenuItem}"
          Command="{Binding DeleteAllCommand}" />
```

## ?? **Related Files**
- [../UI-Instructions/ui-generation.instruction.md](../UI-Instructions/ui-generation.instruction.md) - UI component generation guidelines
- [../Core-Instructions/naming.conventions.instruction.md](../Core-Instructions/naming.conventions.instruction.md) - Command and menu naming standards
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - UI Enhancement Copilot persona details

## ? **Quality Checklist**

### **Context Menu Implementation**
- [ ] Proper AXAML context menu structure
- [ ] Command binding to ReactiveUI commands
- [ ] Conditional menu item visibility
- [ ] Keyboard shortcuts and accessibility support
- [ ] Visual separators and organization

### **User Experience**
- [ ] Right-click activation functionality
- [ ] Touch-friendly interaction support
- [ ] Selection-aware menu options
- [ ] Confirmation dialogs for destructive actions
- [ ] Loading states for async operations

### **MTM Design Integration**
- [ ] Purple brand styling applied
- [ ] Consistent icon usage
- [ ] Proper typography and spacing
- [ ] Warning styling for destructive actions
- [ ] Theme resource integration

### **Technical Quality**
- [ ] Proper data context passing
- [ ] Async operation support
- [ ] Error handling for menu actions
- [ ] Performance optimization for large collections
- [ ] Memory management for menu instances