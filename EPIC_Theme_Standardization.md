# EPIC: MTM Theme Standardization and WCAG 2.1 AA Compliance

## 📋 Epic Summary
Standardize all MTM theme files (`.axaml`) to match the complete property set from `MTMTheme.axaml`, ensure WCAG 2.1 AA compliance across all themes, and remove deprecated preview sections that are no longer used in the application.

## 🎯 Epic Goals
1. **Consistency**: All theme files follow the same structure and property definitions
2. **Accessibility**: All themes meet WCAG 2.1 AA contrast requirements (minimum 4.5:1 ratio)
3. **Maintainability**: Remove deprecated `Design.PreviewWith` sections to reduce file size and complexity
4. **Performance**: Standardized themes improve theme switching performance

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

### Issues Identified
- ❌ **Inconsistent property sets**: Not all themes have the same brush definitions
- ❌ **WCAG non-compliance**: Some themes use colors that don't meet 4.5:1 contrast ratios
- ❌ **Large file sizes**: `Design.PreviewWith` sections add ~15KB per file (330KB total)
- ❌ **Maintenance overhead**: Preview sections require updating when UI changes

## 🔧 Technical Requirements

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
- [ ] **Task 1.2**: Create WCAG compliance validation checklist
- [ ] **Task 1.3**: Develop color contrast testing procedure

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

### Phase 5: Validation and Testing
- [ ] **Task 5.1**: Automated WCAG contrast testing
- [ ] **Task 5.2**: Visual regression testing
- [ ] **Task 5.3**: Theme switching performance testing
- [ ] **Task 5.4**: File size verification (target: <5KB per theme)

## 📏 Definition of Done

### Per Theme File
- [ ] Contains all 80+ required brush definitions
- [ ] Passes WCAG 2.1 AA contrast validation
- [ ] `Design.PreviewWith` section completely removed
- [ ] File size reduced by 70%+ (from ~22KB to <5KB)
- [ ] Theme-appropriate colors while maintaining accessibility
- [ ] Consistent header comments with theme name and compliance note

### Epic Completion
- [ ] All 22 theme files updated and validated
- [ ] Documentation updated with new theme structure
- [ ] Performance improvements measured and documented
- [ ] WCAG compliance report generated
- [ ] No visual regressions in existing UI components

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

## 📊 Expected Benefits

### Performance
- **Theme file size reduction**: 330KB → 95KB (~71% reduction)
- **Faster theme switching**: Reduced memory footprint
- **Improved loading times**: Smaller resource dictionary parsing

### Maintainability  
- **Consistent structure**: All themes follow same pattern
- **Easier updates**: Changes propagate predictably across themes
- **Reduced complexity**: No preview sections to maintain

### Accessibility
- **WCAG 2.1 AA compliance**: All themes meet accessibility standards
- **Better readability**: Proper contrast ratios across all color combinations
- **Inclusive design**: Themes work for users with visual impairments

### User Experience
- **Visual consistency**: All themes provide same UI elements
- **Professional appearance**: WCAG-compliant colors look more polished
- **Better brand representation**: Colors maintain identity while being accessible

## 🔍 Risk Assessment

### Low Risk
- File structure changes (well-defined pattern)
- Color updates (can be validated automatically)
- Preview removal (no functional impact)

### Medium Risk  
- WCAG compliance verification (requires testing tools)
- Theme switching testing (potential UI regressions)

### Mitigation Strategies
- Automated contrast ratio testing
- Visual regression test suite
- Phased rollout with validation at each step
- Backup of original files maintained

## 📅 Timeline Estimate
- **Phase 1**: 1 day (template validation and tooling)
- **Phase 2**: 3 days (light themes - 10 files)
- **Phase 3**: 3 days (dark themes - 8 files) 
- **Phase 4**: 1 day (high contrast theme)
- **Phase 5**: 2 days (testing and validation)

**Total: 10 working days**

## ✅ Acceptance Criteria
1. All 22 theme files conform to MTMTheme.axaml structure
2. All themes pass WCAG 2.1 AA contrast validation
3. No `Design.PreviewWith` sections remain in any theme file
4. File sizes reduced by 70%+ across all themes
5. No visual regressions in existing UI components
6. Theme switching performance improved measurably
7. Documentation updated to reflect new theme structure
8. WCAG compliance report generated and validated

---

**Epic Owner**: Development Team  
**Priority**: Medium  
**Estimated Effort**: 10 days  
**Dependencies**: None  
**Impact**: High (Accessibility, Performance, Maintainability)
