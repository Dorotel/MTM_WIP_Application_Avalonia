# CustomDataGrid - MTM Design System Integration

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🎨 MTM Design System Integration

The CustomDataGrid must seamlessly integrate with the MTM design system using DynamicResource bindings for automatic theme switching and consistent visual appearance.

## Required Dynamic Resource Bindings

### Background Colors
```xml
<!-- Card and Panel Backgrounds -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}" />

<!-- Interactive State Backgrounds -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryLightBrush}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryHoverBrush}" />
```

### Border Colors
```xml
<!-- Border Styling -->
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}" />
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
```

### Text Colors
```xml
<!-- Text and Content Colors -->
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.HeadingText}" />
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}" />
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.InteractiveText}" />
```

### Action Colors
```xml
<!-- Primary Actions -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}" />

<!-- Warning/Error Actions -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.Critical}" />

<!-- Success Indicators -->
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.SuccessBrush}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SuccessLightBrush}" />
```

## Complete Style Implementation

### Header Cell Styling
```xml
<Style Selector="Border.header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Left" />
</Style>

<Style Selector="Border.header-cell TextBlock">
  <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.HeadingText}" />
  <Setter Property="FontWeight" Value="SemiBold" />
  <Setter Property="FontSize" Value="12" />
</Style>
```

### Data Cell Styling
```xml
<Style Selector="Border.data-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Left" />
</Style>

<Style Selector="Border.data-cell TextBlock">
  <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}" />
  <Setter Property="FontSize" Value="12" />
</Style>
```

### Interactive States
```xml
<!-- Hover States -->
<Style Selector="Border.data-cell:pointerover">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}" />
</Style>

<!-- Selection States -->
<Style Selector="Border.data-cell:selected">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryLightBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="BorderThickness" Value="2" />
</Style>

<!-- Focus States -->
<Style Selector="Border.data-cell:focus">
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="BorderThickness" Value="2" />
</Style>
```

### Checkbox Styling
```xml
<Style Selector="Border.checkbox-cell CheckBox, Border.checkbox-header-cell CheckBox">
  <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.InteractiveText}" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}" />
</Style>

<Style Selector="Border.checkbox-cell CheckBox:checked, Border.checkbox-header-cell CheckBox:checked">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="Foreground" Value="White" />
</Style>
```

### Action Button Styling
```xml
<!-- Primary Action Buttons (Read Note, Edit, etc.) -->
<Style Selector="Border.action-cell Button.primary-action">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="Foreground" Value="White" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="CornerRadius" Value="3" />
  <Setter Property="Padding" Value="4,2" />
  <Setter Property="FontSize" Value="10" />
</Style>

<Style Selector="Border.action-cell Button.primary-action:pointerover">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}" />
</Style>

<!-- Warning Action Buttons (Delete) -->
<Style Selector="Border.action-cell Button.warning-action">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
  <Setter Property="Foreground" Value="White" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
  <Setter Property="CornerRadius" Value="3" />
  <Setter Property="Padding" Value="4,2" />
  <Setter Property="FontSize" Value="10" />
</Style>

<Style Selector="Border.action-cell Button.warning-action:pointerover">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.Critical}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.Critical}" />
</Style>
```

## Theme Compatibility Matrix

### MTM_Blue Theme
- **Primary Action**: #0078D4 (Windows 11 Blue)
- **Card Background**: #F3F2F1 (Light gray)
- **Border Light**: #E1DFDD (Subtle border)
- **Heading Text**: #323130 (Dark gray)
- **Body Text**: #605E5C (Medium gray)

### MTM_Green Theme
- **Primary Action**: #107C10 (Success green)
- **Card Background**: #F3F2F1 (Consistent with blue)
- **Border Light**: #E1DFDD (Consistent with blue)
- **Text colors**: Same as blue theme for consistency

### MTM_Red Theme
- **Primary Action**: #D13438 (Alert red)
- **Card Background**: #F3F2F1 (Consistent)
- **Border Light**: #E1DFDD (Consistent)
- **Text colors**: Same as other themes

### MTM_Dark Theme
- **Primary Action**: #4FC3F7 (Light blue for dark)
- **Card Background**: #2D2D30 (Dark gray)
- **Border Light**: #3C3C3C (Dark border)
- **Heading Text**: #FFFFFF (White text)
- **Body Text**: #CCCCCC (Light gray text)

## Conditional Styling

### Notes Indicator
```xml
<!-- Show checkmark for items with notes -->
<TextBlock Text="✓"
           Foreground="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
           FontSize="14"
           FontWeight="Bold"
           HorizontalAlignment="Center"
           VerticalAlignment="Center"
           IsVisible="{Binding HasNotes}" />
```

### Row Alternation
```xml
<!-- Alternate row backgrounds for better readability -->
<Style Selector="ListBoxItem:nth-child(2n) Border.data-cell">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.AlternateRowBrush}" />
</Style>
```

### Loading State
```xml
<!-- Loading indicator styling -->
<Style Selector="Border.loading-overlay">
  <Setter Property="Background" Value="#80000000" />
  <Setter Property="CornerRadius" Value="8" />
</Style>

<Style Selector="Border.loading-overlay TextBlock">
  <Setter Property="Foreground" Value="White" />
  <Setter Property="FontSize" Value="14" />
  <Setter Property="FontWeight" Value="SemiBold" />
</Style>
```

## Accessibility Support

### High Contrast Mode
```xml
<!-- Ensure visibility in high contrast mode -->
<Style Selector="Border.data-cell:highcontrast">
  <Setter Property="BorderThickness" Value="2" />
</Style>

<Style Selector="Border.header-cell:highcontrast">
  <Setter Property="BorderThickness" Value="2" />
  <Setter Property="FontWeight" Value="Bold" />
</Style>
```

### Focus Indicators
```xml
<!-- Clear focus indicators for keyboard navigation -->
<Style Selector="Border.data-cell:focus-visible">
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <Setter Property="BorderThickness" Value="3" />
  <Setter Property="BoxShadow" Value="0 0 0 2px {DynamicResource MTM_Shared_Logic.PrimaryAction}" />
</Style>
```

---

**Next Implementation Phase**: [05-Data-Binding-Requirements.md](./05-Data-Binding-Requirements.md)