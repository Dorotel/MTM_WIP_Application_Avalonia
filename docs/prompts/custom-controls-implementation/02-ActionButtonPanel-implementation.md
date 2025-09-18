# ActionButtonPanel Custom Control Implementation Prompt

**GitHub Copilot Implementation Prompt for ActionButtonPanel**  
**Priority**: Rank 2 - Very High Impact Control  
**Complexity**: Simple - Enhanced StackPanel with intelligent button management  
**Integration**: MVVM Community Toolkit + MTM Theme System

---

## 🎯 Implementation Prompt for GitHub Copilot

**Paste this exact text into your GitHub Copilot Agent:**

```text
Create a comprehensive ActionButtonPanel custom control for MTM WIP Application Avalonia that provides intelligent action button management with consistent ordering, loading states, and manufacturing-optimized behavior. This control should eliminate the 200+ lines of repetitive button panel AXAML found across 35+ views.

REQUIREMENTS:

Architecture Integration:
- Inherit from Avalonia UserControl (11.3.4)  
- Use MVVM Community Toolkit 8.3.2 patterns
- Integrate with existing MTM theme system (DynamicResource bindings)
- Support .NET 8 C# 12 nullable reference types
- Follow MTM manufacturing domain patterns

Core Features:
1. Intelligent Button Management:
   - Standardized button ordering: Save → Cancel → Reset → Advanced → Custom
   - Automatic button sizing for touch (MinHeight="44" for Android)
   - Built-in loading states with progress indication
   - Confirmation dialogs for destructive operations (Reset, Delete)
   - Keyboard shortcuts (Ctrl+S for Save, Escape for Cancel)

2. Manufacturing-Optimized UX:
   - Primary action emphasis (Save button highlighted)
   - Disabled state management during operations
   - Progress feedback during async operations
   - Error state indication with red borders
   - Success feedback with green checkmarks

3. Button Types Support:
   - Save: Primary action with confirmation
   - Cancel: Secondary action, no confirmation
   - Reset: Requires confirmation dialog
   - Delete: Destructive action with strong confirmation
   - Custom: User-defined actions with configurable behavior

4. Visual States:
   - Normal: Standard MTM theme colors
   - Loading: Progress spinner + disabled state
   - Error: Red border + error icon
   - Success: Green border + checkmark (brief animation)
   - Disabled: Grayed out appearance

5. Touch and Accessibility:
   - Large touch targets (44px minimum height)
   - Screen reader support with action descriptions
   - Keyboard navigation (Tab, Enter, Escape)
   - High contrast mode support

TECHNICAL IMPLEMENTATION:

Control Structure:
```csharp
namespace MTM_WIP_Application_Avalonia.Controls;

public partial class ActionButtonPanel : UserControl
{
    // Styled Properties
    public static readonly StyledProperty<ObservableCollection<ActionButton>> ButtonsProperty;
    public static readonly StyledProperty<ButtonPanelLayout> LayoutProperty;
    public static readonly StyledProperty<bool> ShowConfirmationsProperty;
    public static readonly StyledProperty<bool> IsLoadingProperty;
    public static readonly StyledProperty<string> LoadingMessageProperty;
    
    // Events  
    public event EventHandler<ActionExecutedEventArgs>? ActionExecuted;
    public event EventHandler<ActionExecutingEventArgs>? ActionExecuting;
    
    // Methods
    public void AddAction(ActionButtonType type, ICommand command, string? content = null);
    public void SetLoadingState(bool isLoading, string? message = null);
    public void ShowSuccessState(string? message = null, TimeSpan? duration = null);
}

public enum ActionButtonType
{
    Save,
    Cancel, 
    Reset,
    Delete,
    Custom
}

public class ActionButton
{
    public ActionButtonType Type { get; set; }
    public ICommand Command { get; set; }
    public string Content { get; set; }
    public bool RequiresConfirmation { get; set; }
    public string? ConfirmationMessage { get; set; }
    public KeyGesture? KeyboardShortcut { get; set; }
}
```

AXAML Template:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.ActionButtonPanel">
  
  <UserControl.Styles>
    <!-- Primary action button (Save) -->
    <Style Selector="Button.primary-action">
      <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
      <Setter Property="Foreground" Value="White" />
      <Setter Property="Padding" Value="20,12" />
      <Setter Property="MinHeight" Value="44" />
      <Setter Property="FontWeight" Value="SemiBold" />
    </Style>
    
    <!-- Secondary action button (Cancel) -->
    <Style Selector="Button.secondary-action">
      <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryAction}" />
      <Setter Property="Foreground" Value="White" />
      <Setter Property="Padding" Value="16,10" />
      <Setter Property="MinHeight" Value="44" />
    </Style>
    
    <!-- Destructive action button (Reset, Delete) -->
    <Style Selector="Button.destructive-action">
      <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
      <Setter Property="Foreground" Value="White" />
      <Setter Property="Padding" Value="16,10" />
      <Setter Property="MinHeight" Value="44" />
    </Style>
    
    <!-- Loading state -->
    <Style Selector="Button.loading">
      <Setter Property="IsEnabled" Value="False" />
      <Setter Property="Opacity" Value="0.7" />
    </Style>
  </UserControl.Styles>

  <Grid RowDefinitions="*,Auto">
    <!-- Progress overlay for loading state -->
    <Border Grid.Row="0" IsVisible="{Binding IsLoading}">
      <!-- Loading progress indicator -->
    </Border>
    
    <!-- Button panel -->
    <StackPanel Grid.Row="1" 
                Orientation="Horizontal" 
                HorizontalAlignment="Right" 
                Spacing="12" 
                Margin="0,16,0,0">
      <!-- Buttons populated dynamically -->
    </StackPanel>
  </Grid>
</UserControl>
```

Usage Examples:
```xml
<!-- Standard Save/Cancel/Reset pattern -->
<controls:ActionButtonPanel>
  <controls:ActionButton Type="Save" Command="{Binding SaveCommand}" />
  <controls:ActionButton Type="Cancel" Command="{Binding CancelCommand}" />
  <controls:ActionButton Type="Reset" Command="{Binding ResetCommand}" RequiresConfirmation="True" />
</controls:ActionButtonPanel>

<!-- With custom actions -->
<controls:ActionButtonPanel IsLoading="{Binding IsProcessing}" LoadingMessage="{Binding ProcessingMessage}">
  <controls:ActionButton Type="Save" Command="{Binding SaveCommand}" />
  <controls:ActionButton Type="Custom" Command="{Binding ExportCommand}" Content="Export Data" />
  <controls:ActionButton Type="Cancel" Command="{Binding CancelCommand}" />
</controls:ActionButtonPanel>

<!-- Manufacturing workflow actions -->
<controls:ActionButtonPanel ShowConfirmations="True">
  <controls:ActionButton Type="Save" Command="{Binding CompleteOperationCommand}" Content="Complete Operation" />
  <controls:ActionButton Type="Custom" Command="{Binding MoveToNextStationCommand}" Content="Move to Next" />
  <controls:ActionButton Type="Reset" Command="{Binding ScrapPartsCommand}" Content="Scrap Parts" RequiresConfirmation="True" ConfirmationMessage="This will mark parts as scrapped. Continue?" />
</controls:ActionButtonPanel>
```

Manufacturing Features:
1. Confirmation Dialogs:
   - "Are you sure you want to reset this form? All changes will be lost."
   - "This will scrap the selected parts. This action cannot be undone. Continue?"
   - "Complete this operation and move parts to next station?"

2. Keyboard Shortcuts:
   - Ctrl+S: Execute Save command
   - Escape: Execute Cancel command  
   - Ctrl+R: Execute Reset command (with confirmation)
   - F5: Execute Refresh/Reload command

3. Manufacturing Workflow Integration:
   - Integrate with existing Services/ErrorHandling for action logging
   - Support batch operations with progress indication
   - Audit trail for all destructive actions
   - Integration with manufacturing business rules

Integration Requirements:
- Work seamlessly with existing MVVM Community Toolkit ViewModels
- Support all MTM theme variations (Blue/Green/Red/Dark)
- Integrate with Services/ErrorHandling for action logging
- Follow MTM code patterns and disposal requirements
- Maintain backward compatibility with existing button implementations

Cross-Platform Considerations:
- Windows: Native dialog styling and keyboard shortcuts
- macOS: Platform-appropriate button ordering and styling
- Linux: GTK-compatible appearance and behavior  
- Android: Touch-optimized with haptic feedback support

Generate complete implementation with:
1. Full ActionButtonPanel.axaml and ActionButtonPanel.axaml.cs
2. Action button management logic with confirmation dialogs
3. Loading and progress state management
4. Theme integration with all MTM themes
5. Event handling and command execution
6. Unit tests for all action scenarios
7. Documentation with manufacturing workflow examples
8. Migration guide for existing button panels

Ensure all code follows MTM patterns: nullable reference types, proper disposal, MVVM Community Toolkit integration, and manufacturing workflow compliance.
```

---

## 📋 Expected Deliverables

1. **ActionButtonPanel.axaml** - Complete AXAML with MTM theme integration
2. **ActionButtonPanel.axaml.cs** - Full C# implementation with action management
3. **ActionButtonPanelTests.cs** - Unit tests for all button scenarios
4. **ConfirmationDialog.axaml/.cs** - Reusable confirmation dialog
5. **Integration Examples** - Manufacturing workflow usage patterns
6. **Migration Guide** - Converting existing button StackPanels

## 🎯 Success Criteria

- **Replaces 35+ repetitive button panel implementations**
- **Provides consistent action button ordering across all views**
- **Reduces button panel AXAML by 200+ lines**
- **Improves touch experience for Android manufacturing tablets**
- **Standardizes confirmation dialogs for destructive actions**
- **Maintains full MVVM Community Toolkit integration**

## 🔄 Next Steps After Implementation

1. **Integration Testing**: Replace button panels in 5 high-traffic views
2. **User Experience Validation**: Test with manufacturing operators
3. **Accessibility Testing**: Validate screen reader and keyboard navigation
4. **Performance Testing**: Benchmark loading states and animations
5. **Cross-Platform Validation**: Test on all target platforms

---

**Implementation Priority**: Phase 1 - Foundation control  
**Estimated Development Time**: 2-3 days for full implementation and testing  
**Expected Impact**: Consistent action management across all 35+ views with improved UX