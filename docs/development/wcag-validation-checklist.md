# WCAG Validation Checklist Template

*WCAG 2.1 AA Compliance validation template for MTM Theme Standardization EPIC*

## Overview
This checklist provides standardized validation procedures for ensuring all MTM theme files and view files meet WCAG 2.1 AA accessibility compliance requirements.

**Target**: WCAG 2.1 AA compliance across all 22 theme files and 64 view files  
**Minimum Requirements**: 4.5:1 contrast ratio for normal text, 3:1 for large text  

## Theme File Validation Template

### Basic Information
```yaml
ThemeFile: "Resources/Themes/[ThemeName].axaml"
ThemeVariant: "[Light/Dark/HighContrast]"
ValidationDate: "[YYYY-MM-DD]"
Reviewer: "[Agent/Developer Name]"
```

### Required Brush Definitions (80+ brushes)
```yaml
CoreBrandColors: # 6 brushes
  - MTM_Shared_Logic.PrimaryAction: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.SecondaryAction: "✅/❌ Present and WCAG compliant" 
  - MTM_Shared_Logic.Warning: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.Status: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.Critical: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.Highlight: "✅/❌ Present and WCAG compliant"

ExtendedPalette: # 3 brushes
  - MTM_Shared_Logic.DarkNavigation: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.CardBackground: "✅/❌ Present and WCAG compliant" 
  - MTM_Shared_Logic.HoverBackground: "✅/❌ Present and WCAG compliant"

InteractiveStateColors: # 9 brushes
  - MTM_Shared_Logic.OverlayTextBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.PrimaryHoverBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.SecondaryHoverBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.MagentaHoverBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.PrimaryPressedBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.SecondaryPressedBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.MagentaPressedBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.PrimaryDisabledBrush: "✅/❌ Present and WCAG compliant"
  - MTM_Shared_Logic.SecondaryDisabledBrush: "✅/❌ Present and WCAG compliant"
```

### WCAG Contrast Validation
```yaml
ContrastValidation:
  NormalText: "4.5:1 minimum ratio required"
  LargeText: "3:1 minimum ratio required" 
  UIComponents: "3:1 minimum for boundaries"
  
PrimaryColorCombinations:
  - Foreground: "MTM_Shared_Logic.HeadingText"
    Background: "MTM_Shared_Logic.MainBackground"
    ContrastRatio: "[Calculated ratio]"
    Status: "✅ Pass / ❌ Fail"
    
  - Foreground: "MTM_Shared_Logic.BodyText"
    Background: "MTM_Shared_Logic.CardBackgroundBrush"  
    ContrastRatio: "[Calculated ratio]"
    Status: "✅ Pass / ❌ Fail"
    
  - Foreground: "MTM_Shared_Logic.OverlayTextBrush"
    Background: "MTM_Shared_Logic.PrimaryAction"
    ContrastRatio: "[Calculated ratio]"
    Status: "✅ Pass / ❌ Fail"
```

### File Structure Validation
```yaml
FileStructure:
  FileSize: "[Size in KB] - Target: <5KB"
  PreviewSectionsRemoved: "✅ All Design.PreviewWith sections removed"
  HeaderComments: "✅ Proper theme name and compliance notes"
  BrushCount: "[Number] - Target: 80+ brushes"
```

## View File Validation Template

### Basic Information
```yaml
ViewFile: "Views/[Category]/[ViewName].axaml"
Category: "[MainForm/SettingsForm/TransactionsForm]"
ValidationDate: "[YYYY-MM-DD]" 
Reviewer: "[Agent/Developer Name]"
```

### Hardcoded Color Detection
```yaml
HardcodedColors:
  Status: "✅ Pass | ❌ Fail | 🔄 In Progress"
  DetectedIssues: []
  # Example issue format:
  # - Line: 139
  #   Content: '<Border Background="#1E88E5"/>'
  #   RequiredFix: '<Border Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>'
  #   Priority: "HIGH/MEDIUM/LOW"
```

### Theme Integration
```yaml
ThemeIntegration:
  Status: "✅ Pass | ❌ Fail | 🔄 In Progress"
  DynamicResources: "✅/❌ All colors use {DynamicResource MTM_Shared_Logic.*}"
  ColorPropertyFlexibility: "✅ Permitted - switching between MTM_Shared_Logic properties for UI quality"
  ThemeSwitching: "✅/❌ Responsive to theme changes without restart"
  FallbackHandling: "✅/❌ Graceful degradation when resources missing"
```

### WCAG Compliance Per View
```yaml
WCAGCompliance:
  Status: "✅ Pass | ❌ Fail | 🔄 In Progress"
  TextContrast: "✅/❌ All text meets 4.5:1 minimum contrast"
  InteractiveElements: "✅/❌ Buttons, inputs meet accessibility standards"
  FocusIndicators: "✅/❌ Visible focus indicators with 3:1 contrast"
  ColorIndependence: "✅/❌ Information not conveyed by color alone"
```

### Cross-Theme Validation
```yaml
CrossThemeValidation:
  TestedThemes: # Test against all 22 themes
    - MTM_Blue: "✅/❌ Pass"
    - MTM_Green: "✅/❌ Pass"
    - MTM_Red: "✅/❌ Pass" 
    - MTM_Dark: "✅/❌ Pass"
    - MTM_HighContrast: "✅/❌ Pass"
    # ... continue for all themes
  VisualConsistency: "✅/❌ Maintains intended design across themes"
  NoRegressions: "✅/❌ No visual or functional regressions"
```

### Manual Review
```yaml
ManualReview:
  Reviewer: ""
  Date: ""
  Status: "✅ Complete | ❌ Pending"
  Notes: []
  AccessibilityAudit: "✅/❌ Manual accessibility review completed"
```

## Contrast Ratio Calculation Reference

### WCAG 2.1 AA Requirements
- **Normal text**: Minimum 4.5:1 contrast ratio
- **Large text (18pt+ or 14pt+ bold)**: Minimum 3:1 contrast ratio  
- **UI component boundaries**: Minimum 3:1 contrast ratio
- **Focus indicators**: Minimum 3:1 contrast ratio with adjacent colors

### High Priority Color Combinations
```yaml
CriticalCombinations:
  ButtonStates:
    - Normal: "PrimaryAction background + OverlayTextBrush foreground"
    - Hover: "PrimaryHoverBrush background + OverlayTextBrush foreground"
    - Pressed: "PrimaryPressedBrush background + OverlayTextBrush foreground" 
    - Disabled: "PrimaryDisabledBrush background + appropriate text color"
    
  TextCombinations:
    - Headers: "HeadingText + MainBackground"
    - Body: "BodyText + CardBackgroundBrush"
    - Links: "InteractiveText + MainBackground"
    - Placeholders: "PlaceholderTextBrush + CardBackgroundBrush"
    
  SemanticColors:
    - Success: "SuccessBrush + MainBackground"  
    - Warning: "WarningBrush + MainBackground"
    - Error: "ErrorBrush + MainBackground"
    - Info: "InfoBrush + MainBackground"
```

## Validation Tools Integration

### PowerShell Hardcoded Color Detection
```powershell
# Usage: .\detect-hardcoded-colors.ps1 -OutputPath "validation-report.json"
# Results integration with this checklist
```

### C# Contrast Validation 
```csharp
// ContrastValidator.CalculateContrastRatio(foreground, background)
// Integration with validation workflow
```

### Automated Validation Pipeline
```yaml
AutomatedChecks:
  HardcodedColorDetection: "✅/❌ Pass"
  ContrastRatioValidation: "✅/❌ Pass"
  ThemeIntegrationTest: "✅/❌ Pass"
  FileStructureValidation: "✅/❌ Pass"
```

## Success Criteria

### Per Theme File Completion
- [ ] All 80+ required brushes defined with theme-appropriate colors
- [ ] WCAG 2.1 AA contrast validation passes for all color combinations
- [ ] File size reduced by 70%+ (target <5KB)
- [ ] No `Design.PreviewWith` sections remain
- [ ] Consistent header comments with theme name and compliance notes

### Per View File Completion  
- [ ] No hardcoded color values (automatic detection passes)
- [ ] All colors use `{DynamicResource MTM_Shared_Logic.*}` patterns
- [ ] Color property optimization completed for UI quality
- [ ] Text contrast meets WCAG 2.1 AA across all 22 themes
- [ ] Interactive elements meet accessibility requirements
- [ ] Focus indicators visible and accessible
- [ ] Responsive to theme switching without restart
- [ ] Manual accessibility review completed

### Epic Completion Criteria
- [ ] All 22 theme files validated and compliant
- [ ] All 64 view files validated for integration and accessibility  
- [ ] Automated validation tools implemented and passing
- [ ] Documentation updated with compliance procedures
- [ ] Performance improvements measured and documented
- [ ] No visual regressions in existing components
- [ ] Future-proofing measures implemented

---

**Document Status**: ✅ Complete WCAG Validation Framework  
**Created**: [Current Date]
**Last Updated**: [Current Date]  
**Validation Framework Owner**: MTM Development Team