<!-- markdownlint-disable-file -->
# Task Research Notes: QuickButtonsView.axaml Theme V2 and StyleSystem Migration

## Research Executed

### File Analysis
- `Views/MainForm/Panels/QuickButtonsView.axaml`
  - Contains extensive manual styling with hardcoded colors and effects
  - Uses legacy color references like `{DynamicResource ThemeV2.Color.Gray.600}`
  - Complex button styles defined locally instead of using StyleSystem classes
  - Manufacturing-specific styling for quick action buttons

### Code Search Results
- `ThemeV2\.|MTM_Shared_Logic\.|StyleSystem`
  - Found CollapsiblePanel.axaml using proper ThemeV2 semantic tokens
  - Multiple views using Classes="Primary|Secondary|Icon" pattern successfully
  - StyleSystem.axaml properly structured with component includes

### External Research
- Theme V2 Light theme analysis
  - Complete semantic token system with Background, Content, Action, Border, Input categories
  - Manufacturing-specific tokens: `ThemeV2.MTM.QuickButton.Background`, `ThemeV2.MTM.Inventory.In/Out/Transfer`
  - Proper DynamicResource usage for theme switching support

### Project Conventions
- Standards referenced: Theme V2 semantic tokens, StyleSystem component classes
- Instructions followed: style-system-implementation.instructions.md, theme-v2-implementation.instructions.md

## Key Discoveries

### Project Structure
QuickButtonsView.axaml represents a complex manufacturing interface with:
- Header panel with toggle buttons (Quick Actions vs History)
- Main content area with 10-row UniformGrid for quick action buttons
- Footer panel with quality-of-life action buttons
- Transaction history display using custom controls

### Implementation Patterns
Current implementation uses extensive local styles instead of leveraging:
1. **StyleSystem Classes**: Available `QuickAction`, `HeaderToggle`, `FooterToggle` classes in Manufacturing/ActionButtons.axaml
2. **Theme V2 Tokens**: Manual color definitions instead of semantic tokens
3. **Component Hierarchy**: Could use Card/Form layout classes for better structure

### Complete Examples
```xml
<!-- EXISTING: Manual styling -->
<Style Selector="Button.quick-button">
  <Setter Property="Background" Value="{DynamicResource ThemeV2.Background.Card}"/>
  <Setter Property="BorderBrush" Value="{DynamicResource ThemeV2.Border.Default}"/>
  <Setter Property="Foreground" Value="{DynamicResource ThemeV2.Content.Primary}"/>
  <!-- 20+ manual property setters -->
</Style>

<!-- RECOMMENDED: StyleSystem approach -->
<Button Classes="QuickAction"
        Command="{Binding ExecuteQuickActionCommand}"
        Background="{DynamicResource ThemeV2.MTM.QuickButton.Background}"
        Foreground="{DynamicResource ThemeV2.MTM.QuickButton.Content}"/>
```

### API and Schema Documentation
StyleSystem Manufacturing/ActionButtons.axaml provides:
- `QuickAction` - Standard manufacturing quick buttons
- `HeaderToggle` - Header toggle buttons with active states
- `FooterToggle` - Compact footer action buttons
- Proper hover/pressed states with Theme V2 integration

### Configuration Examples
```xml
<!-- Header Toggle Pattern -->
<Button Classes="HeaderToggle"
        Classes.Active="{Binding !IsShowingHistory}"
        Background="{DynamicResource ThemeV2.Action.Primary}"
        Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>

<!-- Manufacturing Action Pattern -->
<Button Classes="QuickAction"
        Background="{DynamicResource ThemeV2.MTM.QuickButton.Background}"
        Foreground="{DynamicResource ThemeV2.MTM.QuickButton.Content}"/>
```

### Technical Requirements
Migration requires:
1. Replace 200+ lines of local styles with StyleSystem classes
2. Update all color references to proper semantic tokens
3. Maintain complex grid layout and interaction patterns
4. Preserve manufacturing touch-target requirements
5. Ensure theme switching compatibility

## Recommended Approach

**Comprehensive Theme V2 + StyleSystem Migration Strategy**

### Phase 1: Style Replacement
- Replace `Button.quick-button` styles with `Classes="QuickAction"`
- Replace `Button.header-toggle-button` with `Classes="HeaderToggle"`
- Replace `Button.toggle-button` with `Classes="FooterToggle"`
- Update all border/background/content colors to semantic tokens

### Phase 2: Layout Modernization
- Apply `Classes="Card"` to main container borders
- Use proper typography classes for text elements
- Implement Form layout classes for structured sections

### Phase 3: Manufacturing Integration
- Leverage `ThemeV2.MTM.QuickButton.*` tokens for manufacturing context
- Apply appropriate MTM manufacturing semantic tokens
- Ensure proper contrast and accessibility

## Implementation Guidance
- **Objectives**: Migrate to Theme V2 semantic tokens and StyleSystem classes while preserving complex manufacturing UI functionality
- **Key Tasks**: 
  1. Replace local button styles with StyleSystem classes
  2. Update color references to semantic tokens
  3. Apply layout component classes
  4. Validate manufacturing touch targets
- **Dependencies**: StyleSystem.axaml, Theme V2 resources, Manufacturing/ActionButtons.axaml
- **Success Criteria**: 
  1. No local color definitions
  2. All buttons use StyleSystem classes
  3. Theme switching works correctly
  4. Manufacturing workflow preserved
