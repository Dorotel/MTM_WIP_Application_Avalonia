# AXAML Comprehensive Refactoring Prompt - Themes + StyleSystem

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## Chat Mode Configuration

**Mode**: Complete AXAML Modernization  
**Focus**: Migrate to Theme V2 semantic tokens AND StyleSystem component classes  
**Scope**: Full refactoring to modern MTM AXAML patterns  

## Core Instructions

You are performing a comprehensive refactoring of AXAML files to implement both Theme V2 semantic tokens and StyleSystem component classes. This creates fully modernized, maintainable AXAML code following MTM standards.

### PRIMARY OBJECTIVES

1. **Theme V2 Migration**: Replace all hardcoded colors and legacy themes with semantic tokens
2. **StyleSystem Integration**: Apply appropriate component classes for consistent styling
3. **Architecture Modernization**: Ensure code follows current MTM AXAML patterns
4. **Maintainability**: Create clean, readable, and maintainable AXAML structure

### PHASE 1: Theme V2 Token Migration

#### Replace Hardcoded Colors

```xml
<!-- OLD: Hardcoded values -->
Background="Blue" → Background="{DynamicResource ThemeV2.Action.Primary}"
Foreground="White" → Foreground="{DynamicResource ThemeV2.Content.OnColor}"
BorderBrush="#E0E0E0" → BorderBrush="{DynamicResource ThemeV2.Border.Default}"
```

#### Replace Legacy Theme References

```xml
<!-- OLD: Legacy references -->
Background="{DynamicResource MTM_Shared_Logic.ButtonBackgroundBrush}"
→ Background="{DynamicResource ThemeV2.Action.Primary}"

Foreground="{DynamicResource MTM_Shared_Logic.ContentBrush}"
→ Foreground="{DynamicResource ThemeV2.Content.Primary}"
```

### PHASE 2: StyleSystem Class Application

#### Apply Component Classes

```xml
<!-- OLD: Manual styling -->
<Button Background="Blue" 
        Padding="12,8" 
        CornerRadius="4"
        Content="Save"/>

<!-- NEW: StyleSystem + Theme V2 -->
<Button Classes="Primary" 
        Background="{DynamicResource ThemeV2.Action.Primary}"
        Content="Save"/>
```

#### Typography Modernization

```xml
<!-- OLD: Manual typography -->
<TextBlock FontSize="24" 
           FontWeight="Bold"
           Foreground="Black"
           Text="Title"/>

<!-- NEW: Typography class + semantic token -->
<TextBlock Classes="Heading2" 
           Foreground="{DynamicResource ThemeV2.Content.Primary}"
           Text="Title"/>
```

### Comprehensive Token Reference

#### Background Tokens

- `ThemeV2.Background.Canvas` - Main app background
- `ThemeV2.Background.Surface` - Secondary surfaces  
- `ThemeV2.Background.Card` - Card/panel backgrounds
- `ThemeV2.Action.Primary` - Primary button backgrounds
- `ThemeV2.Action.Secondary` - Secondary button backgrounds

#### Content/Text Tokens

- `ThemeV2.Content.Primary` - Main content text
- `ThemeV2.Content.Secondary` - Supporting text
- `ThemeV2.Content.OnColor` - Text on colored backgrounds
- `ThemeV2.Content.Disabled` - Disabled text
- `ThemeV2.Content.Placeholder` - Placeholder text

#### Border Tokens

- `ThemeV2.Border.Default` - Standard borders
- `ThemeV2.Border.Subtle` - Light borders
- `ThemeV2.Border.Strong` - Emphasized borders
- `ThemeV2.Border.Focus` - Focus indicators

#### Input Tokens

- `ThemeV2.Input.Background` - Input field backgrounds
- `ThemeV2.Input.Border` - Input field borders
- `ThemeV2.Input.Content` - Input text color

#### Status Tokens

- `ThemeV2.Status.Success` - Success states
- `ThemeV2.Status.Warning` - Warning states
- `ThemeV2.Status.Error` - Error states
- `ThemeV2.Status.Info` - Information states

#### Manufacturing Tokens

- `ThemeV2.MTM.Inventory.In` - IN transaction styling
- `ThemeV2.MTM.Inventory.Out` - OUT transaction styling
- `ThemeV2.MTM.Inventory.Transfer` - Transfer styling
- `ThemeV2.MTM.QuickButton.Background` - Quick button backgrounds

### StyleSystem Classes Reference

#### Button Classes

- `Primary` - Main action buttons
- `Secondary` - Secondary/cancel buttons
- `Icon` - Icon-only buttons
- `Icon.Small` - Small icon buttons (32px)
- `Icon.Large` - Large icon buttons (48px)
- `MTM.QuickAction` - Standard MTM buttons
- `MTM.QuickAction.In` - IN transaction buttons
- `MTM.QuickAction.Out` - OUT transaction buttons
- `MTM.Transaction` - Transfer buttons

#### Typography Classes

- `Heading1` - Main page titles (32px)
- `Heading2` - Section headers (24px)
- `Heading3` - Subsection headers (20px)
- `Body` - Standard body text (16px)
- `Caption` - Small text (12px)
- `Success` - Success message styling
- `Error` - Error message styling
- `Warning` - Warning message styling
- `Info` - Information message styling

#### Input Classes

- `Standard` - Default input styling
- `Large` - Larger inputs for main fields
- `Error` - Error state inputs
- `Success` - Success state inputs
- `Searchable` - Editable ComboBox styling
- `Compact` - Dense layout inputs

#### Layout Classes

- `Card` - Standard card containers
- `Card.Elevated` - Elevated card styling
- `Card.Title` - Card title text
- `Card.Content` - Card body text
- `Form` - Form container styling
- `Form.Label` - Form field labels
- `Form.Input` - Form input styling
- `Form.Actions` - Action button containers

### Complete Transformation Examples

#### Button Transformation

**BEFORE:**

```xml
<Button Background="Blue" 
        Foreground="White"
        Padding="12,8"
        CornerRadius="4"
        FontWeight="Bold"
        Content="Save Changes"
        Command="{Binding SaveCommand}"/>
```

**AFTER:**

```xml
<Button Classes="Primary"
        Background="{DynamicResource ThemeV2.Action.Primary}"
        Foreground="{DynamicResource ThemeV2.Content.OnColor}"
        Content="Save Changes"
        Command="{Binding SaveCommand}"/>
```

#### Form Layout Transformation

**BEFORE:**

```xml
<StackPanel Margin="16" Spacing="12" Background="White">
    <TextBlock FontSize="20" FontWeight="Bold" Foreground="Black" Text="Edit Item"/>
    <TextBlock FontWeight="Bold" Foreground="Gray" Text="Part ID:"/>
    <TextBox Background="White" BorderBrush="Gray" Padding="8" Watermark="Enter part ID"/>
    <StackPanel Orientation="Horizontal" Spacing="8" Margin="0,16,0,0">
        <Button Background="Blue" Foreground="White" Padding="12,8" Content="Save"/>
        <Button Background="Gray" Foreground="Black" Padding="12,8" Content="Cancel"/>
    </StackPanel>
</StackPanel>
```

**AFTER:**

```xml
<Border Classes="Card" 
        Background="{DynamicResource ThemeV2.Background.Card}"
        Padding="16">
    <StackPanel Classes="Form" Spacing="12">
        <TextBlock Classes="Heading3" 
                   Foreground="{DynamicResource ThemeV2.Content.Primary}"
                   Text="Edit Item"/>
        <TextBlock Classes="Form.Label" 
                   Foreground="{DynamicResource ThemeV2.Content.Secondary}"
                   Text="Part ID:"/>
        <TextBox Classes="Standard"
                 Background="{DynamicResource ThemeV2.Input.Background}"
                 BorderBrush="{DynamicResource ThemeV2.Input.Border}"
                 Foreground="{DynamicResource ThemeV2.Input.Content}"
                 Watermark="Enter part ID"/>
        <StackPanel Classes="Form.Actions" Orientation="Horizontal" Spacing="8">
            <Button Classes="Primary"
                    Background="{DynamicResource ThemeV2.Action.Primary}"
                    Foreground="{DynamicResource ThemeV2.Content.OnColor}"
                    Content="Save"/>
            <Button Classes="Secondary"
                    Background="{DynamicResource ThemeV2.Action.Secondary}"
                    Foreground="{DynamicResource ThemeV2.Content.Primary}"
                    Content="Cancel"/>
        </StackPanel>
    </StackPanel>
</Border>
```

#### Manufacturing Component Transformation

**BEFORE:**

```xml
<UniformGrid Columns="2" Margin="8">
    <Button Background="Green" Foreground="White" Content="IN" Padding="16,12"/>
    <Button Background="Red" Foreground="White" Content="OUT" Padding="16,12"/>
    <Button Background="Blue" Foreground="White" Content="TRANSFER" Padding="16,12"/>
    <Button Background="Orange" Foreground="White" Content="CYCLE" Padding="16,12"/>
</UniformGrid>
```

**AFTER:**

```xml
<UniformGrid Classes="MTM.QuickButtons" Columns="2">
    <Button Classes="MTM.QuickAction.In"
            Background="{DynamicResource ThemeV2.MTM.Inventory.In}"
            Foreground="{DynamicResource ThemeV2.Content.OnColor}"
            Content="IN"/>
    <Button Classes="MTM.QuickAction.Out"
            Background="{DynamicResource ThemeV2.MTM.Inventory.Out}"
            Foreground="{DynamicResource ThemeV2.Content.OnColor}"
            Content="OUT"/>
    <Button Classes="MTM.Transaction"
            Background="{DynamicResource ThemeV2.MTM.Inventory.Transfer}"
            Foreground="{DynamicResource ThemeV2.Content.OnColor}"
            Content="TRANSFER"/>
    <Button Classes="MTM.QuickAction"
            Background="{DynamicResource ThemeV2.Action.Primary}"
            Foreground="{DynamicResource ThemeV2.Content.OnColor}"
            Content="CYCLE"/>
</UniformGrid>
```

### Refactoring Process

#### Step 1: Analysis

1. Identify all hardcoded colors and legacy theme references
2. Catalog manual styling patterns that can use StyleSystem classes
3. Note manufacturing-specific components requiring MTM classes
4. Plan token and class mapping strategy

#### Step 2: Theme Token Migration

1. Replace hardcoded colors with appropriate semantic tokens
2. Update legacy theme references to Theme V2 tokens
3. Ensure all color properties use `{DynamicResource}`
4. Update Material Icon foreground colors

#### Step 3: StyleSystem Integration

1. Apply appropriate component classes (Primary, Secondary, etc.)
2. Add typography classes to text elements
3. Implement form and card layout classes
4. Apply manufacturing-specific classes where appropriate

#### Step 4: Cleanup and Optimization

1. Remove redundant manual styling properties
2. Consolidate similar elements using consistent classes
3. Ensure proper AXAML structure and hierarchy
4. Validate all bindings and functionality remain intact

### Validation Checklist

#### Theme V2 Compliance

- [ ] No hardcoded color values (`#FFFFFF`, `Blue`, etc.)
- [ ] No legacy theme references (`MTM_Shared_Logic.*`)
- [ ] All color tokens use `{DynamicResource}`
- [ ] Material icons use semantic foreground colors

#### StyleSystem Integration

- [ ] Buttons use appropriate Classes (Primary, Secondary, Icon, MTM.*)
- [ ] Typography uses semantic classes (Heading1-3, Body, Caption)
- [ ] Form inputs have proper input classes
- [ ] Layout containers use Card/Form classes
- [ ] Manufacturing components use MTM.* classes

#### Code Quality

- [ ] AXAML structure is clean and readable
- [ ] Component hierarchy is logical
- [ ] All bindings and commands preserved
- [ ] No redundant styling properties
- [ ] Consistent spacing and organization

### Critical Rules

1. **DynamicResource**: All theme tokens MUST use `{DynamicResource}` for theme switching
2. **Preserve Functionality**: Maintain all bindings, commands, and event handlers
3. **Component Classes**: Prioritize StyleSystem classes over manual styling
4. **Manufacturing Context**: Apply appropriate MTM.* classes for manufacturing components
5. **Clean Structure**: Remove redundant properties covered by classes

## Execution Instructions

1. **Analyze** the provided AXAML file comprehensively
2. **Plan** the migration strategy for both themes and styles
3. **Execute** Theme V2 token migration first
4. **Apply** StyleSystem component classes
5. **Clean up** redundant manual styling
6. **Validate** that all functionality is preserved
7. **Test** mentally for both light/dark theme compatibility

**Create modern, maintainable AXAML that fully leverages Theme V2 and StyleSystem architecture.**
