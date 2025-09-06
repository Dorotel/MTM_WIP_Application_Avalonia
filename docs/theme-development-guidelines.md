# MTM Theme Development Guidelines - Complete Reference

## 📋 Overview

This document provides comprehensive guidelines for developing, maintaining, and validating MTM theme files in accordance with established architecture patterns and WCAG 2.1 AA accessibility standards.

**Framework**: Avalonia UI 11.3.4 with .NET 8  
**Theme System**: MTM_Shared_Logic resource-based theming  
**Accessibility Standard**: WCAG 2.1 AA compliance  
**Last Updated**: September 2025

---

## 🎨 MTM Theme Architecture

### Theme File Structure
All MTM theme files must follow this standardized structure:

```xml
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    
    <!-- Core Brand Colors (6 required) -->
    <SolidColorBrush x:Key="MTM_Shared_Logic.PrimaryAction" Color="#0078D4"/>
    <SolidColorBrush x:Key="MTM_Shared_Logic.SecondaryAction" Color="#106EBE"/>
    <!-- ... 73 additional required brushes ... -->
    
</Styles>
```

### Required Brush Definitions (75 Total)
Every theme file must contain all 75 MTM_Shared_Logic brush definitions:

#### Core Brand Colors (6)
- `MTM_Shared_Logic.PrimaryAction` - Primary button and accent color
- `MTM_Shared_Logic.SecondaryAction` - Secondary actions and highlights
- `MTM_Shared_Logic.Warning` - Warning messages and alerts
- `MTM_Shared_Logic.Status` - Status indicators and badges
- `MTM_Shared_Logic.Critical` - Critical errors and urgent alerts
- `MTM_Shared_Logic.Highlight` - Selection highlights and emphasis

#### UI Layout Colors (10)
- `MTM_Shared_Logic.MainBackground` - Primary application background
- `MTM_Shared_Logic.ContentAreas` - Main content area backgrounds
- `MTM_Shared_Logic.SidebarDark` - Navigation sidebar backgrounds
- `MTM_Shared_Logic.PageHeaders` - Page header section backgrounds
- `MTM_Shared_Logic.FooterBackgroundBrush` - Footer area backgrounds
- `MTM_Shared_Logic.StatusBarBackgroundBrush` - Status bar backgrounds
- `MTM_Shared_Logic.CardBackgroundBrush` - Card component backgrounds
- `MTM_Shared_Logic.PanelBackgroundBrush` - Panel and dialog backgrounds
- `MTM_Shared_Logic.BorderBrush` - Standard border colors
- `MTM_Shared_Logic.BorderDarkBrush` - Dark border variants

#### Text Color System (8)
- `MTM_Shared_Logic.HeadingText` - Primary heading text
- `MTM_Shared_Logic.BodyText` - Standard body text
- `MTM_Shared_Logic.TertiaryTextBrush` - Secondary/subtitle text
- `MTM_Shared_Logic.InteractiveText` - Clickable link text
- `MTM_Shared_Logic.LinkTextBrush` - Navigation links
- `MTM_Shared_Logic.MutedTextBrush` - Disabled/inactive text
- `MTM_Shared_Logic.HighlightTextBrush` - Emphasized text
- `MTM_Shared_Logic.PlaceholderTextBrush` - Input placeholder text

#### Interactive State Colors (9)
- `MTM_Shared_Logic.OverlayTextBrush` - Text over colored backgrounds
- `MTM_Shared_Logic.PrimaryHoverBrush` - Primary button hover states
- `MTM_Shared_Logic.SecondaryHoverBrush` - Secondary button hover states
- `MTM_Shared_Logic.MagentaHoverBrush` - Accent hover states
- `MTM_Shared_Logic.PrimaryPressedBrush` - Primary button pressed states
- `MTM_Shared_Logic.SecondaryPressedBrush` - Secondary button pressed states
- `MTM_Shared_Logic.MagentaPressedBrush` - Accent pressed states
- `MTM_Shared_Logic.PrimaryDisabledBrush` - Primary button disabled states
- `MTM_Shared_Logic.SecondaryDisabledBrush` - Secondary button disabled states

#### Semantic Colors (12)
- `MTM_Shared_Logic.SuccessBrush` + `SuccessLightBrush` + `SuccessDarkBrush`
- `MTM_Shared_Logic.WarningBrush` + `WarningLightBrush` + `WarningDarkBrush`
- `MTM_Shared_Logic.ErrorBrush` + `ErrorLightBrush` + `ErrorDarkBrush`
- `MTM_Shared_Logic.InfoBrush` + `InfoLightBrush` + `InfoDarkBrush`

#### Manufacturing Transaction Colors (6)
- `MTM_Shared_Logic.TransactionInBrush` - IN transactions (green, 4.5:1 contrast)
- `MTM_Shared_Logic.TransactionInLightBrush` - Light IN variant
- `MTM_Shared_Logic.TransactionOutBrush` - OUT transactions (red, 4.5:1 contrast)
- `MTM_Shared_Logic.TransactionOutLightBrush` - Light OUT variant
- `MTM_Shared_Logic.TransactionTransferBrush` - Transfer operations (orange, 4.5:1 contrast)
- `MTM_Shared_Logic.TransactionTransferLightBrush` - Light transfer variant

#### Additional System Colors (24)
- Extended palette, gradient brushes, shadow effects, specialized colors, and state management colors

---

## 🌈 WCAG 2.1 AA Compliance Requirements

### Mandatory Contrast Standards
All themes must meet these minimum contrast ratios:

- **Normal Text**: 4.5:1 contrast ratio with background
- **Large Text** (18pt+ or 14pt+ bold): 3:1 contrast ratio
- **UI Components**: 3:1 contrast ratio for interactive element boundaries
- **Focus Indicators**: 3:1 contrast ratio with adjacent colors

### Critical UI Context Validation
Every theme must pass these 14 essential accessibility tests:

1. **Page Headers** (`HeadingText` on `MainBackground`) - ≥4.5:1
2. **Content Text** (`BodyText` on `ContentAreas`) - ≥4.5:1
3. **Primary Buttons** (`OverlayTextBrush` on `PrimaryAction`) - ≥4.5:1
4. **Secondary Buttons** (`OverlayTextBrush` on `SecondaryAction`) - ≥4.5:1
5. **Critical Alerts** (`OverlayTextBrush` on `Critical`) - ≥4.5:1
6. **Warning Messages** (`OverlayTextBrush` on `Warning`) - ≥4.5:1
7. **Success Indicators** (`OverlayTextBrush` on `SuccessBrush`) - ≥4.5:1
8. **IN Transactions** (`OverlayTextBrush` on `TransactionInBrush`) - ≥4.5:1
9. **OUT Transactions** (`OverlayTextBrush` on `TransactionOutBrush`) - ≥4.5:1
10. **Transfer Operations** (`OverlayTextBrush` on `TransactionTransferBrush`) - ≥4.5:1
11. **Interactive Links** (`InteractiveText` on `ContentAreas`) - ≥4.5:1
12. **Secondary Text** (`TertiaryTextBrush` on `MainBackground`) - ≥4.5:1
13. **Card Headers** (`HeadingText` on `CardBackgroundBrush`) - ≥4.5:1
14. **Card Content** (`BodyText` on `CardBackgroundBrush`) - ≥4.5:1

---

## 🛠️ Development Tools and Automation

### Theme Structure Validation
```powershell
# Validate all themes have required 75 brushes
pwsh scripts/validate-theme-structure.ps1

# Check specific theme
pwsh scripts/validate-theme-structure.ps1 -TargetTheme "MTM_Blue"
```

### WCAG Compliance Testing
```powershell
# Full accessibility validation
pwsh scripts/validate-wcag-compliance.ps1

# Generate detailed compliance report
pwsh scripts/validate-wcag-compliance.ps1 -VerboseOutput

# Test single theme
pwsh scripts/validate-wcag-compliance.ps1 -TargetTheme "MTM_Dark"
```

### Automated WCAG Remediation
```powershell
# Preview contrast fixes without applying
pwsh scripts/remediate-wcag-failures.ps1 -DryRun

# Apply automatic contrast improvements
pwsh scripts/remediate-wcag-failures.ps1

# Target specific theme for remediation
pwsh scripts/remediate-wcag-failures.ps1 -TargetTheme "MTM_Blue_Dark"
```

### Hardcoded Color Detection
```powershell
# Scan all view files for hardcoded colors
pwsh scripts/detect-hardcoded-colors.ps1

# Generate detailed hardcoded color report
pwsh scripts/detect-hardcoded-colors.ps1 -VerboseOutput
```

### File Size Optimization
```powershell
# Remove Design.PreviewWith sections for size reduction
pwsh scripts/optimize-theme-file-sizes.ps1

# Preview optimization without changes
pwsh scripts/optimize-theme-file-sizes.ps1 -DryRun
```

---

## 📝 Theme Development Workflow

### 1. Create New Theme
```powershell
# Copy master template
Copy-Item "Resources/Themes/MTMTheme.axaml" "Resources/Themes/MTM_NewTheme.axaml"

# Update theme name and colors
# Ensure all 75 brushes are defined with appropriate colors for theme
```

### 2. Color Selection Guidelines

#### Light Themes
- **Backgrounds**: Light colors (#F0F0F0 to #FFFFFF range)
- **Text**: Dark colors ensuring 4.5:1+ contrast
- **Buttons**: Saturated colors dark enough for white text
- **Borders**: Medium tone colors for definition

#### Dark Themes  
- **Backgrounds**: Dark colors (#000000 to #3A3A3A range)
- **Text**: Light colors ensuring 4.5:1+ contrast
- **Buttons**: Colors bright enough but not glaring
- **Borders**: Light accent colors for visibility

#### High Contrast Themes
- **Extreme contrast**: Use pure black (#000000) and white (#FFFFFF)
- **Eliminate grays**: Convert to pure black or white
- **Maximum accessibility**: Target 7:1+ contrast ratios

### 3. Validation and Testing
```powershell
# Step 1: Structure validation
pwsh scripts/validate-theme-structure.ps1 -TargetTheme "MTM_NewTheme"

# Step 2: WCAG compliance testing  
pwsh scripts/validate-wcag-compliance.ps1 -TargetTheme "MTM_NewTheme"

# Step 3: Apply fixes if needed
pwsh scripts/remediate-wcag-failures.ps1 -TargetTheme "MTM_NewTheme"

# Step 4: Re-validate compliance
pwsh scripts/validate-wcag-compliance.ps1 -TargetTheme "MTM_NewTheme"
```

### 4. Integration Testing
- Test theme switching in actual application
- Verify all UI components display correctly  
- Check theme responsiveness across different views
- Validate accessibility with screen reader tools

---

## 🎨 Color Psychology and Brand Guidelines

### MTM Color Semantic Meanings
- **Blue** (`#0078D4`): Trust, reliability, primary actions
- **Green** (`#2E7D32`): Success, positive outcomes, IN transactions
- **Red** (`#DC3545`): Alerts, errors, OUT transactions  
- **Orange** (`#E67E00`): Warnings, transfers, intermediate states
- **Gray** (`#605E5C`): Neutral, secondary information
- **Purple** (`#5C2D91`): Advanced features, premium functions

### Brand Consistency Requirements
- **Primary brand color**: Must be present as `PrimaryAction`
- **Consistent semantics**: Green=success, Red=error, Orange=warning
- **Manufacturing context**: Transaction colors must be clearly distinguishable
- **Professional appearance**: All colors must maintain business software aesthetics

---

## 🧪 Testing and Quality Assurance

### Pre-Release Checklist
- [ ] All 75 brushes defined and valid
- [ ] WCAG 2.1 AA compliance achieved (90%+ passing)
- [ ] No hardcoded colors in view files
- [ ] File size optimized (< 10KB per theme)
- [ ] Theme switching tested in application
- [ ] Visual regression testing completed
- [ ] Accessibility testing with external tools
- [ ] Performance impact assessment

### Continuous Integration
Add theme validation to your CI/CD pipeline:

```yaml
# GitHub Actions Example
name: Theme Validation
on: [push, pull_request]

jobs:
  theme-validation:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v3
    - name: Validate Theme Structure
      run: pwsh scripts/validate-theme-structure.ps1
    - name: WCAG Compliance Check
      run: pwsh scripts/validate-wcag-compliance.ps1
    - name: Hardcoded Color Detection
      run: pwsh scripts/detect-hardcoded-colors.ps1
```

### Performance Benchmarks
Target metrics for theme performance:

- **File Size**: < 10KB per theme file (after optimization)
- **Loading Time**: < 100ms theme switching
- **Memory Usage**: < 5MB additional memory per theme
- **Parsing Time**: < 50ms XAML parsing per theme

---

## 📚 View File Integration Guidelines

### Dynamic Resource Usage
All view files must use dynamic theme resources exclusively:

```xml
<!-- ✅ CORRECT: Dynamic theme resource -->
<TextBlock Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}"
           Background="{DynamicResource MTM_Shared_Logic.MainBackground}"/>

<Button Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
        Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>

<!-- ❌ WRONG: Hardcoded color -->
<TextBlock Foreground="#1E88E5"/>
<Border Background="White"/>
```

### WCAG-Compliant View Patterns
```xml
<!-- Accessible button with proper states -->
<Button Content="Save Changes"
        Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
        Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderDarkBrush}">
    <Button.Styles>
        <Style Selector="Button:pointerover">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}"/>
        </Style>
        <Style Selector="Button:pressed">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryPressedBrush}"/>
        </Style>
        <Style Selector="Button:disabled">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryDisabledBrush}"/>
        </Style>
    </Button.Styles>
</Button>

<!-- Accessible form input -->
<TextBox Text="{Binding PartId}"
         Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
         Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"
         BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}">
    <TextBox.Styles>
        <Style Selector="TextBox:focus">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.FocusBrush}"/>
        </Style>
        <Style Selector="TextBox.error">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}"/>
        </Style>
    </TextBox.Styles>
</TextBox>
```

---

## 🔍 Troubleshooting Common Issues

### Issue: Theme validation fails with missing brushes
**Solution**: Run `validate-theme-structure.ps1` to identify missing brush definitions

### Issue: WCAG compliance failures
**Solution**: Use `remediate-wcag-failures.ps1` for automatic fixes, then manual adjustment for edge cases

### Issue: Colors look different after WCAG remediation  
**Solution**: Review color choices for brand consistency while maintaining accessibility

### Issue: Theme switching causes UI glitches
**Solution**: Ensure all color references use `{DynamicResource}` binding, not static colors

### Issue: File sizes too large after theme creation
**Solution**: Run `optimize-theme-file-sizes.ps1` to remove unnecessary preview sections

---

## 📈 Success Metrics and Monitoring

### Theme Quality Metrics
- **Structural Completeness**: 100% (all 75 brushes defined)
- **WCAG Compliance**: 90%+ average across all themes
- **File Size Efficiency**: < 10KB average per theme
- **Zero Hardcoded Colors**: 100% dynamic resource usage

### User Experience Metrics
- **Theme Switching Performance**: < 100ms average
- **Visual Consistency Score**: Subjective rating 4.0+/5.0
- **Accessibility User Satisfaction**: Survey score 4.2+/5.0
- **Support Tickets**: < 2% theme-related issues

---

## 🚀 Future Enhancements

### Planned Improvements
- **Dynamic contrast adjustment**: Real-time WCAG compliance
- **Custom theme builder**: User-friendly theme creation tool
- **Advanced accessibility**: Beyond WCAG AA compliance
- **Performance optimization**: Sub-50ms theme switching
- **Automated testing**: Expanded CI/CD validation coverage

### Version Roadmap
- **v1.0**: Complete WCAG 2.1 AA compliance (Current)
- **v1.1**: Enhanced performance and file size optimization
- **v1.2**: Advanced accessibility features (WCAG AAA)
- **v2.0**: User customization and dynamic theming capabilities

---

**Document Owner**: MTM Development Team  
**Theme System Version**: 2.0  
**Last Updated**: September 2025  
**Next Review**: December 2025