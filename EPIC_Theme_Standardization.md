# EPIC: MTM Theme Standardization and WCAG 2.1 AA Compliance

## 📋 Epic Summary
Standardize all MTM theme files (`.axaml`) to match the complete property set from `MTMTheme.axaml`, ensure WCAG 2.1 AA compliance across all themes, remove deprecated preview sections, and validate all 64 view files for proper theme integration and accessibility compliance.

## 🎯 Epic Goals
1. **Consistency**: All theme files follow the same structure and property definitions
2. **Accessibility**: All themes and UI components meet WCAG 2.1 AA contrast requirements (minimum 4.5:1 ratio)
3. **Maintainability**: Remove deprecated `Design.PreviewWith` sections to reduce file size and complexity
4. **Performance**: Standardized themes improve theme switching performance
5. **Theme Integration**: All UI components use dynamic theme resources, no hardcoded colors
6. **UI Compliance**: Every view file validated for WCAG 2.1 AA compliance across all text/background combinations

## 📊 Current State Analysis

### Theme Files to Update (22 files)
```
✅ MTMTheme.axaml (Master template - already compliant)
🔄 MTM_Blue.axaml (In Progress)
⚠️  MTM_Amber.axaml
⚠️  MTM_Dark.axaml  
⚠️  MTM_Emerald.axaml
⚠️  MTM_Green.axaml
⚠️  MTM_Green_Dark.axaml
⚠️  MTM_HighContrast.axaml
⚠️  MTM_Indigo.axaml
⚠️  MTM_Indigo_Dark.axaml
⚠️  MTM_Light.axaml
⚠️  MTM_Light_Dark.axaml
⚠️  MTM_Orange.axaml
⚠️  MTM_Red.axaml
⚠️  MTM_Red_Dark.axaml
⚠️  MTM_Rose.axaml
⚠️  MTM_Rose_Dark.axaml
⚠️  MTM_Teal.axaml
⚠️  MTM_Teal_Dark.axaml
⚠️  MTM_Blue_Dark.axaml
```

### View Files to Validate (64 files)
**MainForm Views (9 files)**
```
⚠️  Views\MainForm\Panels\AdvancedInventoryView.axaml
⚠️  Views\MainForm\Panels\AdvancedRemoveView.axaml
⚠️  Views\MainForm\Panels\InventoryTabView.axaml (Proper theming detected)
⚠️  Views\MainForm\Panels\MainView.axaml
⚠️  Views\MainForm\Panels\QuickButtonsView.axaml
⚠️  Views\MainForm\Panels\RemoveTabView.axaml
⚠️  Views\MainForm\Panels\TransferTabView.axaml
⚠️  Views\MainForm\Overlays\SuggestionOverlayView.axaml
⚠️  Views\MainForm\Overlays\ThemeQuickSwitcher.axaml
```

**SettingsForm Views (23 files)**
```
⚠️  Views\SettingsForm\AboutView.axaml
⚠️  Views\SettingsForm\AddItemTypeView.axaml
⚠️  Views\SettingsForm\AddLocationView.axaml
⚠️  Views\SettingsForm\AddOperationView.axaml
⚠️  Views\SettingsForm\AddPartView.axaml
⚠️  Views\SettingsForm\AddUserView.axaml
⚠️  Views\SettingsForm\BackupRecoveryView.axaml
⚠️  Views\SettingsForm\DatabaseSettingsView.axaml
⚠️  Views\SettingsForm\EditItemTypeView.axaml
⚠️  Views\SettingsForm\EditLocationView.axaml
⚠️  Views\SettingsForm\EditOperationView.axaml
⚠️  Views\SettingsForm\EditPartView.axaml
⚠️  Views\SettingsForm\EditUserView.axaml
⚠️  Views\SettingsForm\RemoveItemTypeView.axaml
⚠️  Views\SettingsForm\RemoveLocationView.axaml
⚠️  Views\SettingsForm\RemoveOperationView.axaml
⚠️  Views\SettingsForm\RemovePartView.axaml
⚠️  Views\SettingsForm\RemoveUserView.axaml
⚠️  Views\SettingsForm\SecurityPermissionsView.axaml
⚠️  Views\SettingsForm\SettingsForm.axaml (Proper theming detected)
⚠️  Views\SettingsForm\ShortcutsView.axaml
⚠️  Views\SettingsForm\SystemHealthView.axaml
🔍 Views\SettingsForm\ThemeBuilderView.axaml (Hardcoded colors detected - Priority)
```

**TransactionsForm Views (32 files)**
```
⚠️  All TransactionsForm view files require validation
```

### Issues Identified
- ❌ **Inconsistent property sets**: Not all themes have the same brush definitions
- ❌ **WCAG non-compliance**: Some themes use colors that don't meet 4.5:1 contrast ratios
- ❌ **Large file sizes**: `Design.PreviewWith` sections add ~15KB per file (330KB total)
- ❌ **Maintenance overhead**: Preview sections require updating when UI changes
- 🔍 **Hardcoded colors detected**: ThemeBuilderView.axaml contains 4 hardcoded color values (#1E88E5, #FFA726, #4CAF50, #F44336)
- ❌ **UI validation needed**: All 64 view files require WCAG 2.1 AA validation
- ❌ **Theme integration gaps**: Some views may not be fully integrated with theming system
- ❌ **Accessibility audit required**: Text/background contrast ratios need systematic validation

## 🔧 Technical Requirements

### UI Integration Requirements (New)
All view files must comply with these standards:

#### Theming Integration
- **No hardcoded colors**: All color references must use `{DynamicResource MTM_Shared_Logic.*}` patterns
- **Consistent brush usage**: Use standardized brush names across all components
- **Theme responsiveness**: All UI elements must respond to theme changes without restart
- **Fallback handling**: Graceful degradation when theme resources are missing
- **Color property flexibility**: Changing between different MTM_Shared_Logic brush properties is permitted to achieve optimal UI quality (e.g., changing from `MTM_Shared_Logic.OverlayTextBrush` to `MTM_Shared_Logic.DarkNavigation` for better visual design)

#### WCAG 2.1 AA Compliance Per View
Each view file must pass these accessibility checks:

**Text Contrast Requirements:**
- Normal text: 4.5:1 minimum contrast ratio with background
- Large text (18pt+): 3:1 minimum contrast ratio
- UI component text: 4.5:1 minimum contrast ratio

**Interactive Element Requirements:**
- Button states: All hover, pressed, disabled states meet contrast requirements
- Focus indicators: Visible 3:1 minimum contrast ratio with adjacent colors
- Form controls: Input fields, borders, labels meet accessibility standards
- Link text: Distinguishable from body text with sufficient contrast

**Color Independence:**
- Information not conveyed by color alone
- Status indicators use icons/text in addition to color
- Error states include text descriptions, not just red coloring

#### View-Specific Validation Checklist
```xml
<!-- ✅ CORRECT: Dynamic theme resource -->
<TextBlock Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}"/>
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"/>

<!-- ✅ CORRECT: Switching between MTM_Shared_Logic properties for better UI quality -->
<!-- Example: Changing from OverlayTextBrush to DarkNavigation for optimal visual design -->
<Button Foreground="{DynamicResource MTM_Shared_Logic.DarkNavigation}"
        Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>

<!-- ❌ WRONG: Hardcoded color -->
<TextBlock Foreground="#1E88E5"/>
<Border Background="White"/>
```

### Required Property Set (From MTMTheme.axaml)
All themes must include these brush definitions:

#### Core Brand Colors (6)
- `MTM_Shared_Logic.PrimaryAction`
- `MTM_Shared_Logic.SecondaryAction` 
- `MTM_Shared_Logic.Warning`
- `MTM_Shared_Logic.Status`
- `MTM_Shared_Logic.Critical`
- `MTM_Shared_Logic.Highlight`

#### Extended Palette (3)
- `MTM_Shared_Logic.DarkNavigation`
- `MTM_Shared_Logic.CardBackground`
- `MTM_Shared_Logic.HoverBackground`

#### Interactive State Colors (9)
- `MTM_Shared_Logic.OverlayTextBrush`
- `MTM_Shared_Logic.PrimaryHoverBrush`
- `MTM_Shared_Logic.SecondaryHoverBrush`
- `MTM_Shared_Logic.MagentaHoverBrush`
- `MTM_Shared_Logic.PrimaryPressedBrush`
- `MTM_Shared_Logic.SecondaryPressedBrush`
- `MTM_Shared_Logic.MagentaPressedBrush`
- `MTM_Shared_Logic.PrimaryDisabledBrush`
- `MTM_Shared_Logic.SecondaryDisabledBrush`

#### Gradient Brushes (5)
- `MTM_Shared_Logic.PrimaryGradientBrush`
- `MTM_Shared_Logic.HeroGradientBrush`
- `MTM_Shared_Logic.SidebarGradientBrush`
- `MTM_Shared_Logic.CardHoverGradientBrush`
- `MTM_Shared_Logic.AccentRadialBrush`

#### UI Layout Colors (10)
- `MTM_Shared_Logic.MainBackground`
- `MTM_Shared_Logic.ContentAreas`
- `MTM_Shared_Logic.SidebarDark`
- `MTM_Shared_Logic.PageHeaders`
- `MTM_Shared_Logic.FooterBackgroundBrush`
- `MTM_Shared_Logic.StatusBarBackgroundBrush`
- `MTM_Shared_Logic.CardBackgroundBrush`
- `MTM_Shared_Logic.PanelBackgroundBrush`
- `MTM_Shared_Logic.BorderBrush`
- `MTM_Shared_Logic.BorderDarkBrush`

#### Border and Shadow Effects (5)
- `MTM_Shared_Logic.BorderLightBrush`
- `MTM_Shared_Logic.BorderAccentBrush`
- `MTM_Shared_Logic.ShadowBrush`
- `MTM_Shared_Logic.DropShadowBrush`
- `MTM_Shared_Logic.GlowBrush`

#### Text Color System (8)
- `MTM_Shared_Logic.HeadingText`
- `MTM_Shared_Logic.BodyText`
- `MTM_Shared_Logic.TertiaryTextBrush`
- `MTM_Shared_Logic.InteractiveText`
- `MTM_Shared_Logic.LinkTextBrush`
- `MTM_Shared_Logic.MutedTextBrush`
- `MTM_Shared_Logic.HighlightTextBrush`
- `MTM_Shared_Logic.PlaceholderTextBrush`

#### Semantic Colors (12)
- `MTM_Shared_Logic.SuccessBrush` + Light/Dark variants
- `MTM_Shared_Logic.WarningBrush` + Light/Dark variants
- `MTM_Shared_Logic.ErrorBrush` + Light/Dark variants
- `MTM_Shared_Logic.InfoBrush` + Light/Dark variants

#### Transaction Type Colors (6)
- `MTM_Shared_Logic.TransactionInBrush` + Light variant
- `MTM_Shared_Logic.TransactionOutBrush` + Light variant
- `MTM_Shared_Logic.TransactionTransferBrush` + Light variant

#### Specialized Theme Colors (5)
- `MTM_Shared_Logic.Specialized1` through `Specialized5`

#### State Management Colors (6)
- `MTM_Shared_Logic.FocusBrush`
- `MTM_Shared_Logic.SelectionBrush`
- `MTM_Shared_Logic.ActiveBrush`
- `MTM_Shared_Logic.InactiveBrush`
- `MTM_Shared_Logic.LoadingBrush`
- `MTM_Shared_Logic.ProcessingBrush`

**Total: 80+ brush definitions per theme**

### WCAG 2.1 AA Compliance Requirements

#### Contrast Ratios
- **Normal text**: Minimum 4.5:1 contrast ratio
- **Large text**: Minimum 3:1 contrast ratio  
- **UI components**: Minimum 3:1 contrast ratio for boundaries

#### Theme-Specific Guidelines
- **Light themes**: Dark text on light backgrounds
- **Dark themes**: Light text on dark backgrounds
- **High contrast theme**: Maximum possible contrast ratios
- **Color themes**: Maintain readability while preserving brand colors

## 🚀 Implementation Strategy

### Phase 1: Template Creation and Validation
- [x] **Task 1.1**: Validate MTMTheme.axaml as master template
- [x] **Task 1.2**: Create WCAG compliance validation checklist
- [x] **Task 1.3**: Develop color contrast testing procedure

### Phase 2: Light Theme Updates (8 themes)
- [ ] **Task 2.1**: Update MTM_Blue.axaml (50% complete)
- [ ] **Task 2.2**: Update MTM_Green.axaml
- [ ] **Task 2.3**: Update MTM_Red.axaml
- [ ] **Task 2.4**: Update MTM_Amber.axaml
- [ ] **Task 2.5**: Update MTM_Orange.axaml
- [ ] **Task 2.6**: Update MTM_Emerald.axaml
- [ ] **Task 2.7**: Update MTM_Teal.axaml
- [ ] **Task 2.8**: Update MTM_Rose.axaml
- [ ] **Task 2.9**: Update MTM_Indigo.axaml
- [ ] **Task 2.10**: Update MTM_Light.axaml

### Phase 3: Dark Theme Updates (8 themes)
- [ ] **Task 3.1**: Update MTM_Dark.axaml
- [ ] **Task 3.2**: Update MTM_Blue_Dark.axaml
- [ ] **Task 3.3**: Update MTM_Green_Dark.axaml
- [ ] **Task 3.4**: Update MTM_Red_Dark.axaml
- [ ] **Task 3.5**: Update MTM_Rose_Dark.axaml
- [ ] **Task 3.6**: Update MTM_Teal_Dark.axaml
- [ ] **Task 3.7**: Update MTM_Indigo_Dark.axaml
- [ ] **Task 3.8**: Update MTM_Light_Dark.axaml

### Phase 4: Specialized Theme Updates (1 theme)
- [ ] **Task 4.1**: Update MTM_HighContrast.axaml (special WCAG AAA requirements)

### Phase 5: UI Integration and WCAG Validation
- [x] **Task 5.1**: Automated hardcoded color detection across all view files
- [x] **Task 5.2**: Replace hardcoded colors with dynamic theme resources
- [ ] **Task 5.3**: WCAG 2.1 AA contrast validation for MainForm views (9 files)
- [ ] **Task 5.4**: WCAG 2.1 AA contrast validation for SettingsForm views (23 files)
- [ ] **Task 5.5**: WCAG 2.1 AA contrast validation for TransactionsForm views (32 files)
- [x] **Task 5.6**: Fix ThemeBuilderView.axaml hardcoded colors (Priority)
- [ ] **Task 5.7**: Validate all text/background combinations across themes
- [ ] **Task 5.8**: Ensure proper focus indicators meet WCAG guidelines
- [ ] **Task 5.9**: Validate button states (hover, pressed, disabled) for accessibility
- [ ] **Task 5.10**: Test theme switching across all views for consistency

### Phase 6: Automated Testing and Validation Tools
- [ ] **Task 6.1**: Create automated WCAG contrast testing pipeline
- [ ] **Task 6.2**: Develop hardcoded color detection script
- [ ] **Task 6.3**: Visual regression testing across all themes
- [ ] **Task 6.4**: Theme switching performance testing
- [ ] **Task 6.5**: File size verification (target: <5KB per theme)
- [ ] **Task 6.6**: Generate accessibility compliance reports
- [ ] **Task 6.7**: Create theme integration validation checklist

### Phase 7: Documentation Updates and Future-Proofing
- [ ] **Task 7.1**: Update `docs/` folder with theme standardization guidelines
- [ ] **Task 7.2**: Update `.github/` folder with new UI development standards
- [ ] **Task 7.3**: Create WCAG compliance documentation for developers
- [ ] **Task 7.4**: Update theme switching documentation and best practices
- [ ] **Task 7.5**: Document hardcoded color detection pipeline for future PRs
- [ ] **Task 7.6**: Add theme integration checklist to PR templates
- [ ] **Task 7.7**: Update Copilot instructions with new theming requirements
- [ ] **Task 7.8**: Document WCAG 2.1 AA validation procedures
- [ ] **Task 7.9**: Create accessibility testing guidelines for QA
- [ ] **Task 7.10**: Update development workflows with theme validation steps

## 📏 Definition of Done

### Per Theme File
- [ ] Contains all 80+ required brush definitions
- [ ] Passes WCAG 2.1 AA contrast validation
- [ ] `Design.PreviewWith` section completely removed
- [ ] File size reduced by 70%+ (from ~22KB to <5KB)
- [ ] Theme-appropriate colors while maintaining accessibility
- [ ] Consistent header comments with theme name and compliance note

### Per View File (64 files)
- [ ] No hardcoded color values (automatic detection passes)
- [ ] All colors use `{DynamicResource MTM_Shared_Logic.*}` patterns
- [ ] **Color property optimization**: MTM_Shared_Logic brush assignments optimized for UI quality and visual design
- [ ] Text contrast meets WCAG 2.1 AA standards across all themes
- [ ] Interactive elements (buttons, inputs) meet accessibility requirements
- [ ] Focus indicators visible and accessible
- [ ] Error states accessible (not color-dependent)
- [ ] Responsive to theme switching without application restart
- [ ] Manual accessibility review completed and documented

### Epic Completion
- [ ] All 22 theme files updated and validated
- [ ] All 64 view files validated for theme integration and accessibility
- [ ] Hardcoded color detection tool implemented and passing
- [ ] Automated WCAG compliance testing implemented
- [ ] Documentation updated with new theme structure and UI guidelines
- [ ] Performance improvements measured and documented
- [ ] WCAG compliance report generated for all themes and views
- [ ] No visual regressions in existing UI components
- [ ] Theme switching performance improved measurably
- [ ] **Documentation in `docs/` and `.github/` folders updated with new standards**
- [ ] **Future-proofing measures implemented to prevent regression**
- [ ] **Developer guidelines updated for theme and accessibility compliance**

## 🛠️ Automated Testing and Validation Tools

### Hardcoded Color Detection Script
```powershell
# PowerShell script to detect hardcoded colors in AXAML files
$hardcodedPatterns = @(
    'Color="#[0-9A-Fa-f]{6}"',
    'Color="#[0-9A-Fa-f]{8}"', 
    'Background="(White|Black|Red|Blue|Green|Yellow|Gray)"',
    'Foreground="(White|Black|Red|Blue|Green|Yellow|Gray)"'
)

Get-ChildItem -Path "Views\" -Filter "*.axaml" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    foreach ($pattern in $hardcodedPatterns) {
        if ($content -match $pattern) {
            Write-Warning "Hardcoded color found in: $($_.Name)"
        }
    }
}
```

### WCAG Contrast Validation Tool
```csharp
// C# utility class for contrast ratio calculation
public class ContrastValidator 
{
    public static double CalculateContrastRatio(Color foreground, Color background)
    {
        var lum1 = CalculateRelativeLuminance(foreground);
        var lum2 = CalculateRelativeLuminance(background);
        
        var lighter = Math.Max(lum1, lum2);
        var darker = Math.Min(lum1, lum2);
        
        return (lighter + 0.05) / (darker + 0.05);
    }
    
    public static bool MeetsWCAG_AA(Color foreground, Color background, bool isLargeText = false)
    {
        var ratio = CalculateContrastRatio(foreground, background);
        return isLargeText ? ratio >= 3.0 : ratio >= 4.5;
    }
}
```

### Theme Integration Checklist Template
```yaml
# Per-view validation template
ViewFile: "Views/[Category]/[ViewName].axaml"
ValidationChecks:
  HardcodedColors:
    Status: "❌ Pending" # ✅ Pass | ❌ Fail | 🔄 In Progress
    DetectedIssues: []
    
  ThemeIntegration:
    Status: "❌ Pending"
    DynamicResources: "❌ Not Validated"
    ThemeSwitching: "❌ Not Tested"
    
  WCAGCompliance:
    Status: "❌ Pending" 
    TextContrast: "❌ Not Validated"
    InteractiveElements: "❌ Not Validated"
    FocusIndicators: "❌ Not Validated"
    
  ManualReview:
    Reviewer: ""
    Date: ""
    Status: "❌ Pending"
    Notes: []
```

## 🎨 Theme Color Guidelines

### Blue Theme Example (WCAG Compliant)
```xml
<!-- Primary colors maintain blue brand while meeting 4.5:1 contrast -->
<SolidColorBrush x:Key="MTM_Shared_Logic.PrimaryAction" Color="#0056B3"/>      <!-- 4.5:1 on white -->
<SolidColorBrush x:Key="MTM_Shared_Logic.SecondaryAction" Color="#004085"/>    <!-- 7:1 on white -->
<SolidColorBrush x:Key="MTM_Shared_Logic.Warning" Color="#E67E00"/>           <!-- 4.5:1 on white -->
```

### Dark Theme Considerations
- Text colors must be light enough for readability on dark backgrounds
- UI component borders need sufficient contrast
- Semantic colors (success, warning, error) remain consistent across themes

### Additional Design References
- **Material Design Color Application**: https://m2.material.io/design/color/applying-color-to-ui.html#usage
  - Provides comprehensive guidelines for applying color to UI components
  - Covers color hierarchy, emphasis, and accessibility considerations
  - Includes best practices for color usage in different UI contexts

## 🔍 View File Remediation Examples

### ThemeBuilderView.axaml Issues (Priority Fix Required)
**Current Issues:**
```xml
<!-- ❌ WRONG: Hardcoded colors detected -->
<Border Background="#1E88E5"/>  <!-- Line 139 -->
<Border Background="#FFA726"/>  <!-- Line 160 -->
<Border Background="#4CAF50"/>  <!-- Line 182 -->
<Border Background="#F44336"/>  <!-- Line 204 -->
```

**Required Fix:**
```xml
<!-- ✅ CORRECT: Use dynamic theme resources -->
<Border Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>
<Border Background="{DynamicResource MTM_Shared_Logic.Warning}"/>
<Border Background="{DynamicResource MTM_Shared_Logic.SuccessBrush}"/>
<Border Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}"/>
```

### Common UI Patterns for WCAG Compliance
```xml
<!-- Text with proper contrast -->
<TextBlock Text="Heading Text" 
           Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}"
           Background="{DynamicResource MTM_Shared_Logic.MainBackground}"/>

<!-- Button with accessible states - optimized color assignments for UI quality -->
<Button Content="Action Button"
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

<!-- Example: Navigation element optimized for better visual hierarchy -->
<Border Background="{DynamicResource MTM_Shared_Logic.DarkNavigation}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}">
    <TextBlock Text="Navigation Item" 
               Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
</Border>

<!-- Form input with error states -->
<TextBox Text="{Binding InputValue}"
         Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
         Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"
         BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}">
    <TextBox.Styles>
        <Style Selector="TextBox.error">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}"/>
        </Style>
        <Style Selector="TextBox:focus">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.FocusBrush}"/>
        </Style>
    </TextBox.Styles>
</TextBox>
```

## 📊 Expected Benefits

### Performance
- **Theme file size reduction**: 330KB → 95KB (~71% reduction)
- **Faster theme switching**: Reduced memory footprint
- **Improved loading times**: Smaller resource dictionary parsing

### Maintainability  
- **Consistent structure**: All themes follow same pattern
- **Easier updates**: Changes propagate predictably across themes
- **Reduced complexity**: No preview sections to maintain
- **Centralized theming**: All colors managed through theme system

### Accessibility
- **WCAG 2.1 AA compliance**: All themes and views meet accessibility standards
- **Better readability**: Proper contrast ratios across all color combinations
- **Inclusive design**: Themes work for users with visual impairments
- **Universal usability**: No information conveyed by color alone

### User Experience
- **Visual consistency**: All themes provide same UI elements across 64+ views
- **Professional appearance**: WCAG-compliant colors look more polished
- **Better brand representation**: Colors maintain identity while being accessible
- **Seamless theme switching**: No hardcoded colors ensure consistent experience

### Development Quality
- **Automated validation**: Tools detect hardcoded colors and contrast issues
- **Comprehensive testing**: All UI combinations validated for accessibility
- **Documentation**: Clear guidelines for future UI development
- **Compliance assurance**: WCAG reports validate accessibility claims

## 🔍 Risk Assessment

### Low Risk
- File structure changes (well-defined pattern)
- Color updates (can be validated automatically)
- Preview removal (no functional impact)

### Medium Risk  
- WCAG compliance verification (requires testing tools)
- Theme switching testing (potential UI regressions)
- **UI validation scope**: 64+ view files require individual validation
- **Hardcoded color replacement**: Risk of breaking existing UI layouts

### High Risk
- **ThemeBuilderView hardcoded colors**: Critical component with 4 hardcoded colors
- **Cross-theme compatibility**: Ensuring all themes work with all 64 views
- **Accessibility regression**: Potential contrast issues when replacing hardcoded colors

### Mitigation Strategies
- Automated contrast ratio testing with configurable thresholds
- Visual regression test suite with screenshot comparison
- Phased rollout with validation at each step
- Backup of original files maintained
- **Dedicated hardcoded color detection pipeline**
- **Per-theme UI validation before release**
- **Manual accessibility review for critical components**
- **Theme switching integration testing**

## 📅 Timeline Estimate
- **Phase 1**: 1 day (template validation and tooling)
- **Phase 2**: 3 days (light themes - 10 files)
- **Phase 3**: 3 days (dark themes - 8 files) 
- **Phase 4**: 1 day (high contrast theme)
- **Phase 5**: 5 days (UI integration and WCAG validation - 64 view files)
- **Phase 6**: 3 days (automated testing and validation tools)
- **Phase 7**: 2 days (documentation updates and future-proofing)

**Total: 18 working days**

## ✅ Acceptance Criteria
1. All 22 theme files conform to MTMTheme.axaml structure
2. All themes pass WCAG 2.1 AA contrast validation
3. No `Design.PreviewWith` sections remain in any theme file
4. File sizes reduced by 70%+ across all themes
5. **No hardcoded colors in any of the 64 view files**
6. **All view files pass WCAG 2.1 AA contrast validation across all 22 themes**
7. **ThemeBuilderView.axaml hardcoded colors replaced with dynamic resources**
8. **Automated hardcoded color detection tool implemented and passing**
9. **Theme switching works seamlessly across all 64 views**
10. No visual regressions in existing UI components
11. Theme switching performance improved measurably
12. Documentation updated to reflect new theme structure and UI guidelines
13. **WCAG compliance report generated and validated for all themes and views**
14. **UI integration checklist created for future development**
15. **Documentation in `docs/` and `.github/` folders updated and validated**
16. **Future-proofing documentation created to prevent theme/accessibility regressions**

---

## 📋 Session Tracking and Continuation Instructions

### Session Progress Tracking
Each development session working on this EPIC must maintain:

1. **Session Start**: Reference this EPIC document (#file:EPIC_Theme_Standardization.md) and its current progress
2. **Session Work**: Document all completed tasks with specific file names and changes made
3. **Session End**: Provide comprehensive summary of:
   - **Completed Work**: Detailed list of all files modified, tasks completed, and tests passed
   - **Remaining Work**: Updated task list with current status and next priorities
   - **Known Issues**: Any blockers, errors, or dependencies that need attention
   - **Next Steps**: Specific actions for the next session to continue efficiently

### Copilot Continuation Protocol
When user comments `@copilot continue work`, the coding agent must:

1. **Load Context**: 
   - Read this EPIC document (#file:EPIC_Theme_Standardization.md) in full
   - Review the most recent session summary for current progress
   - Identify the next priority tasks from the implementation phases

2. **Validate Current State**:
   - Confirm which tasks are marked as complete vs pending
   - Verify no regressions have been introduced
   - Check that completed work still passes validation

3. **Continue Implementation**:
   - Pick up from the exact point where previous session ended
   - Follow the phase-by-phase approach outlined in this EPIC
   - Maintain all quality standards and validation requirements

4. **Session Documentation**:
   - Update task completion status in real-time
   - Document all changes made during the session
   - Prepare comprehensive handoff summary at session end

### Session Summary Template
**REQUIRED at the end of every session:**

```markdown
## 🏁 SESSION SUMMARY - [Date/Time]

### ✅ COMPLETED WORK
- [ ] Task X.Y: [Specific task] - [Files modified: file1.axaml, file2.cs]
- [ ] Task X.Z: [Specific task] - [Validation results: passed/failed]
- [Detailed list of all work completed]

### 🔄 IN PROGRESS WORK  
- [ ] Task A.B: [Task name] - [Current status and blockers]
- [Any partially completed tasks]

### ❌ REMAINING WORK
- [ ] Task C.D: [Next priority task]
- [ ] Task E.F: [Subsequent task]
- [Complete list of remaining tasks from EPIC phases]

### 🚨 ISSUES AND BLOCKERS
- [Any technical issues encountered]
- [Dependencies or requirements needed]
- [Validation failures or errors to address]

### ➡️ NEXT SESSION PRIORITIES
1. [Specific next task to start with]
2. [Dependencies to resolve first]
3. [Validation steps to complete]

### 📊 OVERALL PROGRESS
- **Theme Files**: [X/22] completed and validated
- **View Files**: [Y/64] validated for hardcoded colors and WCAG compliance  
- **Documentation**: [Z/10] tasks completed in docs/ and .github/ folders
- **Overall Epic Progress**: [XX]% complete

### 🎯 CONTINUATION REFERENCE
Next session must reference: #file:EPIC_Theme_Standardization.md
Continue from: Phase [X], Task [Y]
Priority: [Specific next action]
```

### Epic Completion Protocol
**When 100% of all EPIC tasks are complete**, the final session summary must include:

**🏆 ALL EPIC TASKS COMPLETED - FINAL SUMMARY**
- Complete validation of all 22 theme files ✅
- Complete validation of all 64 view files ✅  
- All documentation updated in docs/ and .github/ folders ✅
- All automated tools implemented and passing ✅
- All WCAG 2.1 AA compliance verified ✅
- All performance improvements documented ✅

**# THIS PULL REQUEST IS READY FOR TESTING**

**Epic Owner**: Development Team  
**Priority**: High (Accessibility and Compliance)  
**Estimated Effort**: 18 days  
**Dependencies**: None  
**Impact**: Critical (Accessibility, Performance, Maintainability, Legal Compliance)
