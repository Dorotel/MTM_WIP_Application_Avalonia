# AXAML StyleSystem Refactoring Prompt - Styles Only

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

## Chat Mode Configuration

**Mode**: StyleSystem Component Class Migration Only  
**Focus**: Replace manual styling with StyleSystem component classes  
**Scope**: Component classes and structure only - DO NOT modify color/theme properties  

## Core Instructions

You are refactoring AXAML files to implement StyleSystem component classes while preserving existing color/theme properties. Focus EXCLUSIVELY on component styling and structure.

### PRIMARY TASK: StyleSystem Class Migration

**✅ REPLACE THESE PATTERNS:**

```xml
<!-- Manual button styling -->
<Button Background="SomeColor" 
        Padding="12,8" 
        CornerRadius="4"
        Content="Action"/>
→ <Button Classes="Primary" 
          Background="SameColor"
          Content="Action"/>

<!-- Manual typography -->
<TextBlock FontSize="24" 
           FontWeight="Bold"
           Foreground="SomeColor"
           Text="Title"/>
→ <TextBlock Classes="Heading2" 
             Foreground="SameColor"
             Text="Title"/>
```

**❌ DO NOT MODIFY:**

- Color properties (Background, Foreground, BorderBrush)
- Theme token references (`{DynamicResource}`, `{StaticResource}`)
- Layout properties are secondary (focus on component classes first)
- Event handlers or bindings
- Content or data binding

### StyleSystem Class Reference

#### Button Classes

- `Primary` - Main action buttons
- `Secondary` - Secondary/cancel buttons  
- `Icon` - Icon-only buttons (40x40px)
- `Icon.Small` - Small icon buttons (32x32px)
- `Icon.Large` - Large icon buttons (48x48px)
- `MTM.QuickAction` - MTM quick buttons
- `MTM.QuickAction.In` - IN transaction buttons
- `MTM.QuickAction.Out` - OUT transaction buttons
- `MTM.Transaction` - Transfer buttons

#### Typography Classes

- `Heading1` - Main titles (32px)
- `Heading2` - Section headers (24px)
- `Heading3` - Subsection headers (20px)
- `Body` - Standard text (16px)
- `Caption` - Small text (12px)
- `Success` - Success messages
- `Error` - Error messages
- `Warning` - Warning messages
- `Info` - Information messages

#### Input Classes

- `Standard` - Default input styling
- `Large` - Larger inputs for main fields
- `Error` - Error state styling
- `Success` - Success state styling
- `Searchable` - Editable ComboBox
- `Compact` - Smaller inputs for dense layouts

#### Layout Classes

- `Card` - Standard card containers
- `Card.Elevated` - Cards with elevation
- `Card.Title` - Card title text
- `Card.Content` - Card body text
- `Form` - Form containers
- `Form.Label` - Form field labels
- `Form.Input` - Form input styling
- `Form.Actions` - Action button containers

### Component Migration Patterns

#### Buttons

```xml
<!-- BEFORE: Manual styling -->
<Button Padding="12,8" 
        CornerRadius="4"
        FontWeight="Bold"
        Content="Save Changes"/>

<!-- AFTER: StyleSystem class -->
<Button Classes="Primary" Content="Save Changes"/>
```

#### Typography

```xml
<!-- BEFORE: Manual typography -->
<TextBlock FontSize="24" 
           FontWeight="Bold"
           Margin="0,0,0,16"
           Text="Section Title"/>

<!-- AFTER: Typography class -->
<TextBlock Classes="Heading2" Text="Section Title"/>
```

#### Form Inputs

```xml
<!-- BEFORE: Manual input styling -->
<TextBox Padding="8" 
         BorderThickness="1"
         CornerRadius="4"
         Watermark="Enter text..."/>

<!-- AFTER: Input class -->
<TextBox Classes="Standard" Watermark="Enter text..."/>
```

#### Form Layouts

```xml
<!-- BEFORE: Manual form layout -->
<StackPanel Margin="16" Spacing="12">
    <TextBlock FontWeight="Bold" Text="Label:"/>
    <TextBox Padding="8"/>
    <StackPanel Orientation="Horizontal" Spacing="8">
        <Button Content="Save"/>
        <Button Content="Cancel"/>
    </StackPanel>
</StackPanel>

<!-- AFTER: Form classes -->
<StackPanel Classes="Form" Spacing="12">
    <TextBlock Classes="Form.Label" Text="Label:"/>
    <TextBox Classes="Form.Input"/>
    <StackPanel Classes="Form.Actions" Orientation="Horizontal" Spacing="8">
        <Button Classes="Primary" Content="Save"/>
        <Button Classes="Secondary" Content="Cancel"/>
    </StackPanel>
</StackPanel>
```

### Manufacturing-Specific Components

#### Quick Action Buttons

```xml
<!-- BEFORE -->
<Button Background="Green" Content="IN"/>
<Button Background="Red" Content="OUT"/>

<!-- AFTER -->
<Button Classes="MTM.QuickAction.In" Background="Green" Content="IN"/>
<Button Classes="MTM.QuickAction.Out" Background="Red" Content="OUT"/>
```

#### Icon Buttons

```xml
<!-- BEFORE -->
<Button Width="40" Height="40" Padding="8">
    <materialIcons:MaterialIcon Kind="Settings"/>
</Button>

<!-- AFTER -->
<Button Classes="Icon">
    <materialIcons:MaterialIcon Kind="Settings"/>
</Button>
```

### Critical Rules

1. **Preserve Colors**: DO NOT modify Background, Foreground, BorderBrush properties
2. **Add Classes**: Add appropriate Classes attribute to components
3. **Remove Manual Styling**: Remove redundant sizing, padding, font properties covered by classes
4. **Maintain Functionality**: Preserve all bindings, commands, and event handlers
5. **Structure Focus**: Prioritize component class application over layout changes

### Component Priority Order

1. **Buttons**: Apply Primary/Secondary/Icon classes first
2. **Typography**: Replace manual font styling with Heading/Body/Caption classes
3. **Inputs**: Apply Standard/Large/Error classes to form fields
4. **Containers**: Add Card/Form classes to layout containers
5. **Manufacturing**: Apply MTM.* classes to manufacturing components

### Validation Checklist

After refactoring, verify:

- [ ] All buttons have appropriate Classes (Primary, Secondary, Icon, MTM.*)
- [ ] Typography uses semantic classes (Heading1-3, Body, Caption)  
- [ ] Form inputs have Standard/Large/Error classes
- [ ] Form layouts use Form.* classes
- [ ] Manufacturing components use MTM.* classes
- [ ] Color properties are preserved unchanged
- [ ] Component functionality remains intact

## Example Transformation

**BEFORE:**

```xml
<StackPanel Margin="16" Spacing="12">
    <TextBlock FontSize="20" FontWeight="Bold" Text="Edit Inventory"/>
    <TextBox Padding="8" BorderThickness="1" Watermark="Part ID"/>
    <Button Padding="12,8" FontWeight="Bold" Content="Save Changes" Command="{Binding SaveCommand}"/>
</StackPanel>
```

**AFTER:**

```xml
<StackPanel Classes="Form" Spacing="12">
    <TextBlock Classes="Heading3" Text="Edit Inventory"/>
    <TextBox Classes="Standard" Watermark="Part ID"/>
    <Button Classes="Primary" Content="Save Changes" Command="{Binding SaveCommand}"/>
</StackPanel>
```

## Execution Instructions

1. **Analyze** the provided AXAML file for manual styling patterns
2. **Identify** components that can use StyleSystem classes
3. **Map** each component to appropriate StyleSystem class
4. **Apply** Classes attributes to components
5. **Remove** redundant manual styling properties (padding, font sizes, etc.)
6. **Preserve** all color properties and theme references
7. **Validate** that component functionality is maintained

**Focus exclusively on StyleSystem class migration - do not modify colors or theme properties.**
