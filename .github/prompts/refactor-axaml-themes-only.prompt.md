# AXAML Theme V2 Refactoring Prompt - Themes Only

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## Chat Mode Configuration

**Mode**: Theme V2 Token Migration Only  
**Focus**: Replace hardcoded colors and legacy theme references with Theme V2 semantic tokens  
**Scope**: Color and theming properties only - DO NOT modify component classes or layout structure  

## Core Instructions

You are refactoring AXAML files to implement Theme V2 semantic tokens while preserving existing component classes and layout structure. Focus EXCLUSIVELY on color and theming properties.

### PRIMARY TASK: Theme Token Migration

**✅ REPLACE THESE PATTERNS:**

```xml
<!-- Hardcoded colors -->
Background="Blue" → Background="{DynamicResource ThemeV2.Action.Primary}"
Foreground="White" → Foreground="{DynamicResource ThemeV2.Content.OnColor}"
BorderBrush="#E0E0E0" → BorderBrush="{DynamicResource ThemeV2.Border.Default}"

<!-- Legacy theme references -->
Background="{DynamicResource MTM_Shared_Logic.ButtonBackgroundBrush}"
→ Background="{DynamicResource ThemeV2.Action.Primary}"

Foreground="{DynamicResource MTM_Shared_Logic.ContentBrush}"
→ Foreground="{DynamicResource ThemeV2.Content.Primary}"
```

**❌ DO NOT MODIFY:**

- Existing Classes attributes (`Classes="Primary"`, `Classes="Secondary"`)
- Layout properties (Margin, Padding, Grid definitions)
- Control structure or hierarchy
- Event handlers or bindings
- Size-related properties (Width, Height, FontSize)

### Token Selection Guide

#### Background Properties

- **Main surfaces**: `ThemeV2.Background.Canvas`
- **Cards/panels**: `ThemeV2.Background.Card`
- **Interactive surfaces**: `ThemeV2.Background.Surface`
- **Primary buttons**: `ThemeV2.Action.Primary`
- **Secondary buttons**: `ThemeV2.Action.Secondary`

#### Text/Content Properties

- **Primary text**: `ThemeV2.Content.Primary`
- **Secondary text**: `ThemeV2.Content.Secondary`
- **Text on colored backgrounds**: `ThemeV2.Content.OnColor`
- **Disabled text**: `ThemeV2.Content.Disabled`
- **Placeholder text**: `ThemeV2.Content.Placeholder`

#### Border Properties

- **Standard borders**: `ThemeV2.Border.Default`
- **Subtle borders**: `ThemeV2.Border.Subtle`
- **Strong borders**: `ThemeV2.Border.Strong`
- **Focus indicators**: `ThemeV2.Border.Focus`

#### Input Fields

- **Input backgrounds**: `ThemeV2.Input.Background`
- **Input borders**: `ThemeV2.Input.Border`
- **Input text**: `ThemeV2.Input.Content`

#### Status States

- **Success**: `ThemeV2.Status.Success`
- **Warning**: `ThemeV2.Status.Warning`
- **Error**: `ThemeV2.Status.Error`
- **Info**: `ThemeV2.Status.Info`

#### Manufacturing Specific

- **IN transactions**: `ThemeV2.MTM.Inventory.In`
- **OUT transactions**: `ThemeV2.MTM.Inventory.Out`
- **TRANSFER**: `ThemeV2.MTM.Inventory.Transfer`
- **Quick buttons**: `ThemeV2.MTM.QuickButton.Background`

### Critical Rules

1. **DynamicResource Only**: All theme tokens MUST use `{DynamicResource}` for theme switching
2. **No StaticResource**: Never use `{StaticResource}` for color tokens
3. **Preserve Context**: Maintain existing component behavior and functionality
4. **Material Icons**: Update icon foreground colors to use semantic tokens

### Material Icons Pattern

```xml
<!-- OLD -->
<materialIcons:MaterialIcon Kind="Save" Foreground="White"/>

<!-- NEW -->
<materialIcons:MaterialIcon Kind="Save" 
                            Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
```

### Manufacturing Component Focus

Pay special attention to:

- Quick action buttons (IN/OUT/TRANSFER styling)
- CollapsiblePanel components
- SessionHistoryPanel components
- CustomDataGrid theming
- Manufacturing-specific indicators

### Validation Checklist

After refactoring, verify:

- [ ] No hardcoded color values remain (`#FFFFFF`, `Blue`, etc.)
- [ ] No legacy theme references (`MTM_Shared_Logic.*`)
- [ ] All color properties use `{DynamicResource ThemeV2.*}`
- [ ] Material icons use appropriate semantic tokens
- [ ] Component Classes attributes are unchanged
- [ ] Layout structure is preserved

## Example Transformation

**BEFORE:**

```xml
<Button Background="Blue" 
        Foreground="White"
        BorderBrush="#D0D0D0"
        Classes="Primary"
        Content="Save">
    <materialIcons:MaterialIcon Kind="Save" Foreground="White"/>
</Button>
```

**AFTER:**

```xml
<Button Background="{DynamicResource ThemeV2.Action.Primary}" 
        Foreground="{DynamicResource ThemeV2.Content.OnColor}"
        BorderBrush="{DynamicResource ThemeV2.Border.Default}"
        Classes="Primary"
        Content="Save">
    <materialIcons:MaterialIcon Kind="Save" 
                                Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
</Button>
```

## Execution Instructions

1. **Analyze** the provided AXAML file for theme-related properties
2. **Identify** hardcoded colors and legacy theme references
3. **Map** each color usage to appropriate Theme V2 semantic token
4. **Replace** color properties with `{DynamicResource ThemeV2.*}` references
5. **Preserve** all non-theming attributes and structure
6. **Validate** that no hardcoded colors or legacy references remain

**Focus exclusively on theming migration - do not modify component classes, layout, or functionality.**
