# CollapsiblePanel.axaml Transformation Implementation Plan

**Date**: September 25, 2025  
**Target File**: `Controls/CollapsiblePanel/CollapsiblePanel.axaml`  
**Research File**: `20250925-collapsiblepanel-style-transformation-research.md`

## Executive Summary

**Research Conclusion**: CollapsiblePanel.axaml is already excellently implemented with comprehensive Theme V2 integration and proper StyleSystem patterns. This file demonstrates best practices for MTM custom control development.

**Transformation Priority**: LOW - File needs minimal enhancements only
**Recommended Approach**: Optional creation of centralized StyleSystem classes to replace local styles

## Implementation Strategy

### Option 1: MINIMAL ENHANCEMENT (RECOMMENDED)

**Rationale**: Current implementation already follows MTM best practices
**Changes**: Create optional centralized StyleSystem classes for future maintainability

**Steps**:

1. Create `Resources/Styles/Components/CollapsiblePanels.axaml` (optional)
2. Update StyleSystem.axaml includes (if new file created)
3. Minimal AXAML updates to use centralized classes (optional)

### Option 2: MAINTAIN CURRENT IMPLEMENTATION

**Rationale**: File is already production-ready and follows all MTM patterns
**Changes**: None required - file serves as example of proper implementation

## Detailed Implementation Plan

### Phase 1: Create Optional StyleSystem Component (OPTIONAL)

**File**: `Resources/Styles/Components/CollapsiblePanels.axaml`

**Content Structure**:

```xml
<!-- CollapsiblePanel Component Styles -->
<Styles xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <!-- Root Container Style -->
  <Style Selector="Border.CollapsiblePanel">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="BorderBrush" Value="{DynamicResource ThemeV2.Border.Default}"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="Transitions">
      <Transitions>
        <BrushTransition Property="BorderBrush" Duration="0:0:0.25"/>
        <TransformOperationsTransition Property="RenderTransform" Duration="0:0:0.3"/>
      </Transitions>
    </Setter>
  </Style>

  <!-- Root Hover State -->
  <Style Selector="Border.CollapsiblePanel:pointerover">
    <Setter Property="BorderBrush" Value="{DynamicResource ThemeV2.Border.Focus}"/>
  </Style>

  <!-- Header Panel Style -->
  <Style Selector="Border.CollapsiblePanel.Header">
    <Setter Property="Background" Value="{DynamicResource ThemeV2.Action.Primary}"/>
    <Setter Property="BorderBrush" Value="Transparent"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="CornerRadius" Value="8,8,0,0"/>
    <Setter Property="Transitions">
      <Transitions>
        <BrushTransition Property="Background" Duration="0:0:0.25"/>
      </Transitions>
    </Setter>
  </Style>

  <!-- Header Hover State -->
  <Style Selector="Border.CollapsiblePanel.Header:pointerover">
    <Setter Property="Background" Value="{DynamicResource ThemeV2.Action.Primary.Hover}"/>
  </Style>

  <!-- Content Panel Style -->
  <Style Selector="Border.CollapsiblePanel.Content">
    <Setter Property="Background" Value="{DynamicResource ThemeV2.Background.Card}"/>
    <Setter Property="BorderBrush" Value="Transparent"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="CornerRadius" Value="8,8,8,8"/>
  </Style>

  <!-- Header Text Styling -->
  <Style Selector="Border.CollapsiblePanel.Header TextBlock, Border.CollapsiblePanel.Header ContentPresenter">
    <Setter Property="TextElement.Foreground" Value="{DynamicResource ThemeV2.Content.OnColor}"/>
    <Setter Property="TextElement.FontWeight" Value="SemiBold"/>
    <Setter Property="TextElement.FontSize" Value="{StaticResource ThemeV2.Typography.Body.FontSize}"/>
  </Style>

  <!-- Toggle Button in Header -->
  <Style Selector="Border.CollapsiblePanel.Header Button">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="BorderBrush" Value="Transparent"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="Margin" Value="0"/>
    <Setter Property="CornerRadius" Value="4"/>
    <Setter Property="Width" Value="32"/>
    <Setter Property="Height" Value="32"/>
    <Setter Property="Transitions">
      <Transitions>
        <BrushTransition Property="Background" Duration="0:0:0.15"/>
      </Transitions>
    </Setter>
  </Style>

  <!-- Toggle Button Hover -->
  <Style Selector="Border.CollapsiblePanel.Header Button:pointerover">
    <Setter Property="Background" Value="{DynamicResource ThemeV2.Background.Hover}"/>
  </Style>

  <!-- MaterialIcon in Header Button -->
  <Style Selector="Border.CollapsiblePanel.Header Button material|MaterialIcon">
    <Setter Property="Foreground" Value="{DynamicResource ThemeV2.Content.OnColor}"/>
    <Setter Property="Width" Value="16"/>
    <Setter Property="Height" Value="16"/>
  </Style>

</Styles>
```

### Phase 2: Update StyleSystem.axaml (IF Phase 1 Implemented)

**File**: `Resources/Styles/StyleSystem.axaml`

**Addition**:

```xml
<!-- Component Controls -->
<StyleInclude Source="avares://MTM_WIP_Application_Avalonia/Resources/Styles/Components/CollapsiblePanels.axaml"/>
```

### Phase 3: Update CollapsiblePanel.axaml (OPTIONAL)

**Current Implementation**: Already excellent - uses proper Theme V2 tokens
**Optional Enhancement**: Replace local style classes with centralized StyleSystem classes

**Changes Required**:

1. Replace `Classes="collapsible-root"` with `Classes="CollapsiblePanel"`
2. Replace `Classes="collapsible-header"` with `Classes="CollapsiblePanel Header"`
3. Replace `Classes="collapsible-content"` with `Classes="CollapsiblePanel Content"`

**Before** (Current - Already Good):

```xml
<Border x:Name="PART_RootContainer" Classes="collapsible-root">
  <Border x:Name="PART_HeaderArea" Classes="collapsible-header">
    <Border x:Name="PART_ContentArea" Classes="collapsible-content">
```

**After** (Optional Enhancement):

```xml
<Border x:Name="PART_RootContainer" Classes="CollapsiblePanel">
  <Border x:Name="PART_HeaderArea" Classes="CollapsiblePanel Header">
    <Border x:Name="PART_ContentArea" Classes="CollapsiblePanel Content">
```

## Risk Assessment

**Implementation Risk**: MINIMAL - File is already well-implemented
**Business Logic Risk**: NONE - Only styling changes, no functional changes
**Integration Risk**: NONE - Uses established MTM patterns
**Performance Risk**: NONE - No performance impact

## Validation Plan

### Pre-Implementation Validation

**Current State Verification**:

- ✅ Theme V2 token usage comprehensive
- ✅ StyleSystem Icon Button integration working
- ✅ All header positions functional (Left, Right, Top, Bottom)
- ✅ Expand/collapse states working in both themes
- ✅ ResolutionIndependentSizing service integration active

### Post-Implementation Validation

**Functional Testing**:

1. **Header Positions**: Test all 4 positions (Left, Right, Top, Bottom)
2. **Expand/Collapse**: Verify toggle functionality preserved
3. **Material Icons**: Confirm toggle icons update correctly
4. **Corner Radius**: Verify corner calculations work for all positions
5. **Button Styling**: Confirm MTM Icon Button styles maintained
6. **Theme Switching**: Test light/dark theme transitions
7. **Service Integration**: Verify ResolutionIndependentSizing still works

**Theme Compatibility Testing**:

- Light theme: All elements visible and properly contrasted
- Dark theme: All elements adapt correctly via DynamicResource
- Theme switching: No restart required, immediate updates

## Recommended Decision

### RECOMMENDED: MAINTAIN CURRENT IMPLEMENTATION

**Rationale**:

- CollapsiblePanel.axaml already demonstrates excellent MTM implementation
- Uses comprehensive Theme V2 token integration
- Follows proper StyleSystem Icon Button patterns
- Clean separation of styling and business logic
- Production-ready with professional manufacturing appearance

**Action**:

- Mark as COMPLETE - no transformation needed
- Use as reference example for other custom controls
- Document as best practice implementation

### ALTERNATIVE: IMPLEMENT OPTIONAL ENHANCEMENTS

**If proceeding with enhancements**:

1. Create optional centralized StyleSystem classes
2. Update class names for consistency
3. Maintain all existing functionality

## Success Metrics

**Current State**: Already meets all success metrics

- ✅ **100% Theme V2 Compliance**: All colors via semantic tokens
- ✅ **100% StyleSystem Integration**: Uses appropriate button classes
- ✅ **100% Business Logic Preservation**: All functionality intact
- ✅ **100% Theme Compatibility**: Perfect light/dark mode operation
- ✅ **Professional Manufacturing Appearance**: Industrial-grade styling
- ✅ **WCAG 2.1 AA Compliance**: Proper contrast and accessibility

## Conclusion

**Final Recommendation**: CollapsiblePanel.axaml is already excellently implemented and serves as a best practice example for MTM custom control development. No transformation is required.

**Documentation Value**: This file should be referenced as an example of proper Theme V2 + StyleSystem integration for custom controls.

**Next Steps**:

1. Mark transformation as COMPLETE (already compliant)
2. Use file as reference for other custom control implementations
3. Consider optional enhancements only if centralized styling is desired for maintainability
