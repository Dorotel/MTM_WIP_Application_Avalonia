# MTMInputField Custom Control Implementation Prompt

**GitHub Copilot Implementation Prompt for MTMInputField**  
**Priority**: Rank 1 - Highest Impact Control  
**Complexity**: Simple - Extends TextBox with validation  
**Integration**: MVVM Community Toolkit + MTM Theme System

---

## 🎯 Implementation Prompt for GitHub Copilot

**Paste this exact text into your GitHub Copilot Agent:**

```text
Create a comprehensive MTMInputField custom control for MTM WIP Application Avalonia that serves as a standardized input control with built-in validation for manufacturing data entry. This control should eliminate the 500+ lines of repetitive input field AXAML found across 25+ views.

REQUIREMENTS:

Architecture Integration:
- Inherit from Avalonia UserControl (11.3.4)
- Use MVVM Community Toolkit 8.3.2 patterns
- Integrate with existing MTM theme system (DynamicResource bindings)
- Support .NET 8 C# 12 nullable reference types
- Follow MTM manufacturing domain patterns

Core Features:
1. Manufacturing-Optimized Validation:
   - PartId validation: ^[A-Z0-9\-]{1,50}$ pattern
   - Operation validation: Must be "90", "100", "110", or "120"
   - Quantity validation: Positive integers 1-999999
   - Location validation: Alphanumeric with underscores
   - Custom validation mode support

2. Visual Feedback System:
   - Error state: Red border + error icon + descriptive message
   - Success state: Green border + checkmark icon
   - Loading state: Progress spinner during async validation
   - Focus state: Blue border (MTM_Shared_Logic.FocusBrush)

3. Manufacturing UX Features:
   - Touch-friendly sizing: MinHeight="44" for Android tablets
   - Keyboard shortcuts: F1-F4 for operation selection (90,100,110,120)
   - Auto-uppercase for part IDs
   - Recent values suggestions with dropdown
   - Required field indicator (*) 

4. Accessibility:
   - WCAG 2.1 AA compliance
   - Screen reader support with proper ARIA labels
   - Keyboard navigation (Tab, Enter, Escape)
   - High contrast theme support

5. Performance Optimization:
   - Async validation with debouncing (300ms delay)
   - Caching for validation results
   - Memory-efficient event handling with proper disposal

TECHNICAL IMPLEMENTATION:

Control Structure:
```csharp
namespace MTM_WIP_Application_Avalonia.Controls;

public partial class MTMInputField : UserControl
{
    // Styled Properties
    public static readonly StyledProperty<string> LabelProperty;
    public static readonly StyledProperty<object?> ValueProperty;
    public static readonly StyledProperty<ValidationMode> ValidationModeProperty;
    public static readonly StyledProperty<bool> IsRequiredProperty;
    public static readonly StyledProperty<bool> ShowValidationIconProperty;
    public static readonly StyledProperty<string> PlaceholderProperty;
    public static readonly StyledProperty<Key> KeyboardShortcutProperty;
    
    // Events
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;
    public event EventHandler<ValidationCompletedEventArgs>? ValidationCompleted;
    
    // Validation
    public ValidationResult ValidationResult { get; private set; }
    public bool IsValid => ValidationResult.IsValid;
}

public enum ValidationMode
{
    None,
    ManufacturingPartId,
    ManufacturingOperation, 
    ManufacturingQuantity,
    ManufacturingLocation,
    Custom
}
```

AXAML Template:
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMInputField">
  
  <UserControl.Styles>
    <!-- MTM Theme Integration -->
    <Style Selector="TextBox.mtm-input">
      <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
      <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderDarkBrush}" />
      <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}" />
      <!-- Error, Focus, Success states -->
    </Style>
  </UserControl.Styles>

  <Grid RowDefinitions="Auto,Auto,Auto" ColumnDefinitions="*,Auto">
    <!-- Label with required indicator -->
    <!-- Input field with validation states -->
    <!-- Error message display -->
    <!-- Loading/success icons -->
  </Grid>
</UserControl>
```

Usage Example:
```xml
<!-- In InventoryTabView.axaml -->
<controls:MTMInputField 
    Label="Part ID"
    Value="{Binding PartId}"
    ValidationMode="ManufacturingPartId"
    IsRequired="True"
    ShowValidationIcon="True"
    KeyboardShortcut="F1"
    Placeholder="Enter part number..." />
```

Integration Requirements:
- Use existing Services/MasterDataService for validation data
- Integrate with Services/ErrorHandling for error logging
- Support existing MTM theme switching (Blue/Green/Red/Dark)
- Work with existing MVVM Community Toolkit ViewModels
- Follow MTM code patterns and naming conventions

Cross-Platform Considerations:
- Windows: Native keyboard shortcuts and styling
- macOS: Cocoa-compatible focus behavior
- Linux: GTK backend compatibility
- Android: Touch-optimized with larger targets

Generate complete implementation with:
1. Full MTMInputField.axaml and MTMInputField.axaml.cs
2. Validation logic with manufacturing patterns
3. Theme integration with all MTM themes
4. Event handling and property change notifications
5. Unit tests demonstrating validation scenarios
6. Documentation with usage examples
7. Integration guide for existing views

Ensure all code follows MTM patterns: nullable reference types, proper disposal, MVVM Community Toolkit integration, and manufacturing domain compliance.
```

---

## 📋 Expected Deliverables

1. **MTMInputField.axaml** - Complete AXAML template with MTM theme integration
2. **MTMInputField.axaml.cs** - Full C# implementation with validation logic  
3. **MTMInputFieldTests.cs** - Comprehensive unit tests
4. **Integration Guide** - How to replace existing TextBox controls
5. **Theme Compatibility** - Works with all 4 MTM theme variations
6. **Performance Validation** - Benchmarks showing improvement over standard TextBox

## 🎯 Success Criteria

- **Replaces 25+ repetitive input field implementations**
- **Reduces form validation code by 300+ lines**
- **Provides consistent validation across all manufacturing forms**
- **Improves user experience with better error feedback**
- **Maintains full backward compatibility with existing ViewModels**
- **Supports all cross-platform deployment targets**

## 🔄 Next Steps After Implementation

1. **Test Integration**: Migrate InventoryTabView to use new control
2. **Gather Feedback**: Validate with manufacturing domain experts
3. **Performance Validation**: Benchmark against existing implementations
4. **Documentation Update**: Update MTM View Implementation Guide
5. **Rollout Planning**: Plan migration of remaining 24 views

---

**Implementation Priority**: Start immediately - highest ROI control  
**Estimated Development Time**: 2-3 days for full implementation and testing  
**Expected Impact**: 60% improvement in form validation consistency and 300+ lines code reduction