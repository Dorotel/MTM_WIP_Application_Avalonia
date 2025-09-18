# DesktopActionButtonPanel Control - Desktop Implementation Prompt

**GitHub Copilot Implementation Prompt**  
**Priority**: #4 (High ROI, Low Complexity)  
**Complexity**: Simple (1-2 weeks)  
**Focus**: Windows Desktop Manufacturing Workstations

---

## 🎯 Paste this exact prompt to GitHub Copilot:

```text
Create a DesktopActionButtonPanel custom control for MTM WIP Application optimized for Windows desktop manufacturing workstations with keyboard shortcuts, mnemonic support, and native Windows behavior.

DESKTOP MANUFACTURING REQUIREMENTS:

## Technical Specifications
- **Framework**: Avalonia UI 11.3.4 with .NET 8
- **Base Class**: UserControl with MVVM Community Toolkit integration
- **Pattern**: [ObservableProperty] and [RelayCommand] support
- **Platform**: Windows Desktop (Keyboard + Mouse first)
- **Usage**: Replace 30+ repetitive button panel implementations

## Desktop Keyboard Features (CRITICAL)
- **Keyboard Shortcuts**: Visual display and handling (Ctrl+S, Ctrl+R, Escape, etc.)
- **Mnemonic Support**: Alt+S, Alt+C for manufacturing speed
- **Default Button**: Enter key triggers primary action
- **Cancel Button**: Escape key triggers cancel/reset action
- **Tab Navigation**: Proper tab order for manufacturing workflows
- **Visual Indicators**: Show keyboard shortcuts on button text or tooltips

## Manufacturing Button Patterns
- **Primary Action**: Save, Add, Update, Confirm operations (Windows 11 Blue style)
- **Secondary Action**: Cancel, Reset, Clear operations (Gray style)
- **Destructive Action**: Delete, Remove operations (Red style with confirmation)
- **Context Action**: Print, Export, Copy operations (Secondary style)

## Desktop UX Requirements
- **Button Sizing**: Desktop-appropriate sizes (not oversized for touch)
- **Mouse Hover**: Rich hover states with button descriptions
- **Context Menus**: Right-click for alternative actions
- **Focus Indicators**: Clear keyboard focus visualization
- **Spacing**: Windows 11 appropriate button spacing (8px)

## AXAML Structure Template
```xml
<UserControl x:Class="MTM_WIP_Application_Avalonia.Controls.DesktopActionButtonPanel"
             xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <StackPanel Orientation="Horizontal" 
              HorizontalAlignment="Right" 
              Spacing="8">
    
    <!-- Primary Action Button with shortcut -->
    <Button Name="PrimaryButton"
            Classes="primary"
            Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
            Foreground="White"
            Padding="16,8"
            CornerRadius="4">
      <StackPanel Orientation="Horizontal" Spacing="8">
        <TextBlock Text="{Binding PrimaryText}" />
        <TextBlock Text="{Binding PrimaryShortcut}" FontSize="10" Opacity="0.8" />
      </StackPanel>
    </Button>
    
    <!-- Secondary Action Button -->
    <Button Name="SecondaryButton"
            Classes="secondary" />
            
    <!-- Additional action buttons as needed -->
  </StackPanel>
</UserControl>
```

## Code-Behind C# Implementation
```csharp
public partial class DesktopActionButtonPanel : UserControl
{
    // Primary action properties
    public static readonly StyledProperty<string> PrimaryTextProperty = ...;
    public static readonly StyledProperty<ICommand> PrimaryCommandProperty = ...;
    public static readonly StyledProperty<string> PrimaryShortcutProperty = ...;
    public static readonly StyledProperty<bool> IsPrimaryDefaultProperty = ...;
    
    // Secondary action properties  
    public static readonly StyledProperty<string> SecondaryTextProperty = ...;
    public static readonly StyledProperty<ICommand> SecondaryCommandProperty = ...;
    public static readonly StyledProperty<string> SecondaryShortcutProperty = ...;
    
    // Desktop keyboard handling
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e.Key)
            {
                case Key.S: ExecutePrimaryAction(); e.Handled = true; break;
                case Key.R: ExecuteSecondaryAction(); e.Handled = true; break;
            }
        }
        else if (e.Key == Key.Enter && IsPrimaryDefault)
        {
            ExecutePrimaryAction();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            ExecuteCancelAction();
            e.Handled = true;
        }
        
        base.OnKeyDown(e);
    }
}
```

## Manufacturing Usage Patterns
- **Inventory Operations**: Save (Ctrl+S), Reset (Ctrl+R), Cancel (Escape)
- **Removal Operations**: Remove (Ctrl+D), Clear Selection (Ctrl+A), Cancel (Escape)  
- **Transfer Operations**: Transfer (Ctrl+T), Reset (Ctrl+R), Cancel (Escape)
- **Settings Operations**: Apply (Ctrl+S), Defaults (Ctrl+D), Cancel (Escape)

## Desktop Context Menu Integration
```csharp
// Right-click context menu for additional actions
private void ShowContextMenu()
{
    var contextMenu = new ContextMenu();
    contextMenu.Items.Add(new MenuItem { Header = "Copy Action Result", Command = CopyResultCommand });
    contextMenu.Items.Add(new MenuItem { Header = "Print Operation", Command = PrintOperationCommand });
    contextMenu.Items.Add(new MenuItem { Header = "Export Data", Command = ExportDataCommand });
    contextMenu.Open(this);
}
```

## MTM Architecture Integration
- **Theme System**: Full DynamicResource integration for all MTM themes (Blue/Green/Red/Dark)
- **Command Binding**: Direct binding to ViewModel commands following MVVM patterns
- **Service Integration**: Optional integration with printing, export services
- **Error Handling**: Visual error states and integration with Services.ErrorHandling

## Manufacturing Accessibility  
- **Screen Reader**: Proper AutomationProperties for manufacturing accessibility
- **High Contrast**: Support for Windows high contrast themes
- **Keyboard Navigation**: 100% keyboard accessible for manufacturing operators
- **Focus Management**: Logical tab order and focus indicators

## Desktop Performance Optimization
- **Lazy Loading**: Load context menus and tooltips on demand
- **Theme Caching**: Cache theme resources for instant theme switching
- **Command Optimization**: Efficient command binding and execution
- **Memory Management**: Proper event subscription cleanup

## Usage Example in Manufacturing Views
```xml
<!-- InventoryTabView.axaml -->
<controls:DesktopActionButtonPanel 
    PrimaryText="Save Inventory"
    PrimaryCommand="{Binding SaveInventoryCommand}"
    PrimaryShortcut="Ctrl+S"
    IsPrimaryDefault="True"
    
    SecondaryText="Reset Form"
    SecondaryCommand="{Binding ResetFormCommand}"
    SecondaryShortcut="Ctrl+R"
    
    ShowContextMenu="True"
    Margin="0,16,0,0" />
```

## Desktop Integration Features
- **Windows 11 Design**: Native Windows 11 button styling and behavior
- **DPI Scaling**: Perfect scaling on high-DPI manufacturing monitors  
- **Multi-Monitor**: Proper context menu placement on multi-monitor setups
- **Performance**: Optimized for desktop hardware acceleration

## Testing Requirements
- Unit tests for keyboard shortcut handling
- Integration tests with MVVM Community Toolkit commands
- Theme switching tests for all MTM theme variations
- Accessibility tests for keyboard navigation and screen readers
- Performance tests for rapid button interactions

Create complete implementation that eliminates all repetitive button panel code across 30+ MTM views while providing superior desktop keyboard navigation and Windows integration.

Focus on manufacturing operator efficiency with keyboard-first workflows and professional Windows desktop appearance.

#github-pull-request_copilot-coding-agent
```

---

## 🖥️ Desktop Manufacturing Impact

### **Code Elimination**
Replaces repetitive AXAML code in 30+ views:
```xml
<!-- OLD: 15+ lines per view -->
<StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8">
    <Button Content="Save" Classes="primary" Command="{Binding SaveCommand}" />
    <Button Content="Cancel" Classes="secondary" Command="{Binding CancelCommand}" />
</StackPanel>

<!-- NEW: 3 lines per view -->
<controls:DesktopActionButtonPanel 
    PrimaryCommand="{Binding SaveCommand}" 
    SecondaryCommand="{Binding CancelCommand}" />
```

### **Manufacturing Operator Efficiency**
- **Keyboard Speed**: Ctrl+S, Ctrl+R, Escape shortcuts eliminate mouse usage
- **Visual Clarity**: Shortcut indicators help operators learn keyboard patterns
- **Consistency**: Identical button behavior across all manufacturing workflows

### **Development Benefits**
- **Time Savings**: 90% reduction in button panel implementation time  
- **Consistency**: Guaranteed consistent button styling and behavior
- **Maintenance**: Single control update affects all manufacturing views
- **Theme Integration**: Automatic theme compliance across all button panels