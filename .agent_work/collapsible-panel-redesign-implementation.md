# CollapsiblePanel HeaderedContentControl Redesign - Implementation Guide

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## Executive Summary

Transform the MTM CollapsiblePanel from UserControl to HeaderedContentControl-based implementation to eliminate TabItem styling conflicts and corner overlap issues while maintaining public API compatibility and professional Theme V2 styling.

## Phase 1: HeaderedContentControl Foundation

### Objective

Redesign CollapsiblePanel.axaml using HeaderedContentControl base class with ControlTemplate approach to eliminate TabItem inheritance issues.

### Technical Specifications

**Base Class Change:**

```xml
<!-- FROM: UserControl -->
<UserControl xmlns="https://github.com/avaloniaui" ... >

<!-- TO: HeaderedContentControl -->
<ResourceDictionary xmlns="https://github.com/avaloniaui" ... >
  <Style Selector="local|CollapsiblePanel" TargetType="local:CollapsiblePanel">
    <Setter Property="Template">
      <ControlTemplate>
        <!-- Control template content -->
      </ControlTemplate>
    </Setter>
  </Style>
</ResourceDictionary>
```

**Code-Behind Update:**

```csharp
// FROM: UserControl inheritance
public partial class CollapsiblePanel : UserControl

// TO: HeaderedContentControl inheritance  
public partial class CollapsiblePanel : HeaderedContentControl
```

**Required Properties (Maintain API Compatibility):**

- `IsExpanded` (bool) - Current expand/collapse state
- `Header` (object) - Panel header content
- `Content` (object) - Panel body content
- `HeaderBackground` (IBrush) - Header styling
- `ExpanderBackground` (IBrush) - Content area styling

### Implementation Steps

1. **Create New CollapsiblePanel.cs:**
   - Inherit from HeaderedContentControl
   - Add AvaloniaProperty definitions for IsExpanded, HeaderBackground, ExpanderBackground
   - Implement PropertyChanged handlers for expand/collapse animation
   - Add TemplateApplied override for control initialization

2. **Redesign CollapsiblePanel.axaml:**
   - Convert to ResourceDictionary with Style definition
   - Create ControlTemplate with professional MTM styling
   - Use Theme V2 semantic tokens (`--mtm-surface-background`, `--mtm-border-color`)
   - Implement expand/collapse animations using DoubleAnimation
   - Include Material Icons for expand/collapse indicators

3. **Professional Styling Requirements:**
   - Gradient header backgrounds with MTM brand colors
   - Subtle drop shadows (`BoxShadow="0 2 8 0 #1A000000"`)
   - Rounded corners (`CornerRadius="8"`)
   - Smooth expand/collapse transitions (200ms duration)
   - Consistent spacing and typography

### Acceptance Criteria

- [ ] No TabItem styling inheritance
- [ ] Maintains existing public API
- [ ] Professional Theme V2 integration
- [ ] Smooth expand/collapse animations
- [ ] Material Icons integration
- [ ] Drop shadow and gradient styling

## Phase 2: Corner Overlap Resolution

### Objective

Fix corner overlap between MainView.axaml TabControl and QuickButtonsView.axaml CollapsiblePanel without changing internal Border content.

### Technical Analysis

**Current Overlap Issue:**

- MainView.axaml: Two-column Grid with TabControl (left) and QuickButtonsView (right)
- QuickButtonsView.axaml: Contains CollapsiblePanel with drop shadow
- Overlap: Drop shadows extend beyond panel boundaries causing visual conflicts

### Implementation Steps

1. **MainView.axaml Layout Adjustments:**

   ```xml
   <!-- Adjust Grid column margins to prevent overlap -->
   <Grid ColumnDefinitions="*,Auto" Margin="8">
     <TabControl Grid.Column="0" Margin="0,0,8,0" />
     <views:QuickButtonsView Grid.Column="1" Margin="8,0,0,0" />
   </Grid>
   ```

2. **QuickButtonsView.axaml Container Updates:**

   ```xml
   <!-- Add proper spacing around CollapsiblePanel -->
   <Border Padding="8" ClipToBounds="true">
     <controls:CollapsiblePanel ... />
   </Border>
   ```

3. **Z-Order Management:**
   - Ensure TabControl renders behind QuickButtonsView
   - Use `ZIndex` properties if needed
   - Verify drop shadow rendering doesn't interfere

### Acceptance Criteria

- [ ] No visual overlap between panels
- [ ] Proper spacing and margins
- [ ] Drop shadows render correctly
- [ ] No changes to internal Border content
- [ ] Professional layout appearance

## Phase 3: Style System Updates

### Objective

Update BaseStyles.axaml to prevent TabItem style conflicts with HeaderedContentControl implementation.

### Technical Requirements

**Style Selector Specificity:**

```xml
<!-- Current problematic global selector -->
<Style Selector="TabItem">
  <!-- Causes blue line on CollapsiblePanel -->
</Style>

<!-- Updated specific selector -->
<Style Selector="TabControl > TabItem">
  <!-- Only applies to actual TabControl children -->
</Style>
```

### Implementation Steps

1. **BaseStyles.axaml Updates:**
   - Change `TabItem` selectors to `TabControl > TabItem`
   - Add CollapsiblePanel-specific styles if needed
   - Preserve all existing TabControl functionality
   - Verify Theme V2 token integration

2. **CollapsiblePanel Style Integration:**

   ```xml
   <!-- Add dedicated CollapsiblePanel styles -->
   <Style Selector="local|CollapsiblePanel">
     <Setter Property="Background" Value="{DynamicResource --mtm-surface-background}" />
     <Setter Property="BorderBrush" Value="{DynamicResource --mtm-border-color}" />
     <!-- Additional professional styling -->
   </Style>
   ```

### Acceptance Criteria

- [ ] No TabItem styling on CollapsiblePanel
- [ ] All TabControl functionality preserved
- [ ] Professional CollapsiblePanel styling
- [ ] Theme V2 integration maintained

## Phase 4: View Integration Testing

### Objective

Identify and test all views using CollapsiblePanel control for compatibility with HeaderedContentControl implementation.

### Discovery Steps

1. **Find All CollapsiblePanel Usages:**

   ```powershell
   # Search for CollapsiblePanel references
   grep -r "CollapsiblePanel" Views/
   grep -r "controls:CollapsiblePanel" *.axaml
   ```

2. **Verify Usage Patterns:**
   - Check property bindings (IsExpanded, Header, Content)
   - Validate event handlers
   - Test expand/collapse functionality
   - Verify Theme V2 styling

3. **Test Integration:**
   - Build and run application
   - Test each view with CollapsiblePanel
   - Verify corner overlap resolution
   - Validate professional styling

### Acceptance Criteria

- [ ] All CollapsiblePanel usages identified
- [ ] No breaking changes to existing views
- [ ] Professional styling across all instances
- [ ] Smooth expand/collapse animations
- [ ] No corner overlap issues

## Implementation Sequence

### Step 1: Backup and Preparation

```powershell
# Create backup branch
git checkout -b feature/collapsible-panel-backup
git checkout copilot/fix-54850bc7-e6dd-49d5-b25c-b8b8bf4efc2e

# Verify current build state
dotnet build
```

### Step 2: Phase 1 Implementation

1. Update CollapsiblePanel.cs (HeaderedContentControl inheritance)
2. Redesign CollapsiblePanel.axaml (ControlTemplate approach)
3. Test basic functionality
4. Commit Phase 1 changes

### Step 3: Phase 2 Implementation

1. Update MainView.axaml layout
2. Adjust QuickButtonsView.axaml spacing
3. Test corner overlap resolution
4. Commit Phase 2 changes

### Step 4: Phase 3 Implementation

1. Update BaseStyles.axaml selectors
2. Add CollapsiblePanel-specific styles
3. Test TabControl functionality
4. Commit Phase 3 changes

### Step 5: Phase 4 Implementation

1. Discover all CollapsiblePanel usages
2. Test each view integration
3. Apply fixes as needed
4. Final testing and validation

## Risk Mitigation

### Potential Issues

1. **API Breaking Changes:** Maintain exact public property signatures
2. **Animation Conflicts:** Test expand/collapse transitions thoroughly
3. **Theme Integration:** Verify Theme V2 semantic token compatibility
4. **Performance Impact:** Monitor rendering and animation performance

### Rollback Plan

- Maintain backup branch with original implementation
- Implement gradual rollout for testing
- Monitor application stability post-deployment

## Success Metrics

### Technical Validation

- [ ] No blue line artifacts on collapsed panels
- [ ] No corner overlap between MainView and QuickButtonsView
- [ ] Professional Theme V2 styling throughout
- [ ] Smooth expand/collapse animations
- [ ] All existing functionality preserved

### Quality Assurance

- [ ] All unit tests pass
- [ ] Manual testing on all CollapsiblePanel instances
- [ ] Cross-platform compatibility verified
- [ ] Performance benchmarks maintained
- [ ] Code review approval

## Conclusion

This phased approach ensures systematic transformation of CollapsiblePanel from UserControl to HeaderedContentControl while maintaining API compatibility, resolving styling conflicts, and delivering professional Theme V2 integration. Each phase builds upon the previous, allowing for incremental testing and validation.

The HeaderedContentControl foundation eliminates TabItem inheritance issues at the source, while corner overlap resolution and style system updates ensure professional visual integration throughout the MTM application.
