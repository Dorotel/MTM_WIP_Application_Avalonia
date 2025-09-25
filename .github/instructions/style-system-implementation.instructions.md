---
description: 'Comprehensive guide for using StyleSystem.axaml component styles and organizing AXAML styling architecture'
applyTo: '**/*.axaml'
---

# StyleSystem Implementation Guide

**The StyleSystem.axaml provides centralized, component-specific styles built on Theme V2 semantic tokens for consistent UI implementation.**

## StyleSystem Architecture

### Component Organization

```text
StyleSystem.axaml (master include)
├── PrimaryButtons.axaml          <!-- Main action buttons -->
├── SecondaryButtons.axaml        <!-- Secondary/cancel buttons -->
├── IconButtons.axaml             <!-- Icon-only buttons -->
├── Manufacturing/
│   └── ActionButtons.axaml       <!-- MTM-specific buttons -->
├── TextInputs.axaml              <!-- TextBox styling -->
├── ComboBoxes.axaml              <!-- ComboBox styling -->
├── Cards.axaml                   <!-- Card/panel layouts -->
├── Forms.axaml                   <!-- Form-specific styles -->
└── Typography.axaml              <!-- Text and heading styles -->
```

### Usage in AXAML Files

#### **Automatic Style Application**

```xml
<!-- NO manual style references needed -->
<!-- Styles apply automatically via component classes -->

<Button Classes="Primary" Content="Save Changes"/>
<Button Classes="Secondary" Content="Cancel"/>
<Button Classes="Icon">
    <materialIcons:MaterialIcon Kind="Settings"/>
</Button>
```

#### **Standard Component Patterns**

```xml
<!-- Primary Action Buttons -->
<Button Classes="Primary" 
        Content="Submit"
        Command="{Binding SubmitCommand}"/>

<!-- Secondary Action Buttons -->
<Button Classes="Secondary" 
        Content="Cancel"
        Command="{Binding CancelCommand}"/>

<!-- Icon Buttons -->
<Button Classes="Icon" 
        ToolTip.Tip="Settings"
        Command="{Binding OpenSettingsCommand}">
    <materialIcons:MaterialIcon Kind="Settings"/>
</Button>

<!-- Manufacturing Action Buttons -->
<Button Classes="MTM.QuickAction"
        Content="Add Inventory"
        Command="{Binding AddInventoryCommand}"/>
```

## Component Style Categories

### **Button Styles**

#### **Primary Buttons** (PrimaryButtons.axaml)

```xml
<!-- Usage Examples -->
<Button Classes="Primary" Content="Save"/>
<Button Classes="Primary.Large" Content="Main Action"/>
<Button Classes="Primary.Small" Content="Quick"/>
```

**Available Classes:**

- `Primary` - Standard primary button
- `Primary.Large` - Larger primary button for main actions
- `Primary.Small` - Compact primary button
- `Primary.Icon` - Primary button with icon support

#### **Secondary Buttons** (SecondaryButtons.axaml)

```xml
<!-- Usage Examples -->
<Button Classes="Secondary" Content="Cancel"/>
<Button Classes="Secondary.Outline" Content="Back"/>
<Button Classes="Secondary.Ghost" Content="Skip"/>
```

**Available Classes:**

- `Secondary` - Standard secondary button
- `Secondary.Outline` - Outlined secondary button
- `Secondary.Ghost` - Minimal secondary button

#### **Icon Buttons** (IconButtons.axaml)

```xml
<!-- Usage Examples -->
<Button Classes="Icon">
    <materialIcons:MaterialIcon Kind="Close"/>
</Button>

<Button Classes="Icon.Small">
    <materialIcons:MaterialIcon Kind="Edit"/>
</Button>
```

**Available Classes:**

- `Icon` - Standard icon button (40x40px)
- `Icon.Small` - Small icon button (32x32px)
- `Icon.Large` - Large icon button (48x48px)

#### **Manufacturing Action Buttons** (Manufacturing/ActionButtons.axaml)

```xml
<!-- MTM-specific buttons -->
<Button Classes="MTM.QuickAction" Content="IN"/>
<Button Classes="MTM.QuickAction.Out" Content="OUT"/>
<Button Classes="MTM.Transaction" Content="Transfer"/>
```

**Available Classes:**

- `MTM.QuickAction` - Standard MTM quick button
- `MTM.QuickAction.In` - Green styling for IN transactions
- `MTM.QuickAction.Out` - Red styling for OUT transactions
- `MTM.Transaction` - Blue styling for transfers

### **Input Controls**

#### **Text Inputs** (TextInputs.axaml)

```xml
<!-- Usage Examples -->
<TextBox Classes="Standard" 
         Watermark="Enter part ID..."/>

<TextBox Classes="Large" 
         Text="{Binding Description}"/>

<TextBox Classes="Error" 
         Text="{Binding InvalidField}"/>
```

**Available Classes:**

- `Standard` - Default text input styling
- `Large` - Larger text input for main fields
- `Error` - Error state styling
- `Success` - Success state styling

#### **ComboBoxes** (ComboBoxes.axaml)

```xml
<!-- Usage Examples -->
<ComboBox Classes="Standard"
          ItemsSource="{Binding Locations}"
          SelectedItem="{Binding SelectedLocation}"/>

<ComboBox Classes="Searchable"
          ItemsSource="{Binding Parts}"
          IsEditable="True"/>
```

**Available Classes:**

- `Standard` - Default ComboBox styling
- `Searchable` - Editable ComboBox with search functionality
- `Compact` - Smaller ComboBox for dense layouts

### **Layout Components**

#### **Cards** (Cards.axaml)

```xml
<!-- Usage Examples -->
<Border Classes="Card">
    <StackPanel>
        <TextBlock Classes="Card.Title" Text="Information"/>
        <TextBlock Classes="Card.Content" Text="Details here"/>
    </StackPanel>
</Border>

<Border Classes="Card.Elevated">
    <!-- Elevated card content -->
</Border>
```

**Available Classes:**

- `Card` - Standard card layout
- `Card.Elevated` - Card with shadow/elevation
- `Card.Title` - Card title text styling
- `Card.Content` - Card content text styling

#### **Forms** (Forms.axaml)

```xml
<!-- Usage Examples -->
<Grid Classes="Form">
    <TextBlock Classes="Form.Label" Text="Part ID:"/>
    <TextBox Classes="Form.Input" Text="{Binding PartId}"/>
</Grid>

<StackPanel Classes="Form.Actions">
    <Button Classes="Primary" Content="Save"/>
    <Button Classes="Secondary" Content="Cancel"/>
</StackPanel>
```

**Available Classes:**

- `Form` - Form container styling
- `Form.Label` - Form field labels
- `Form.Input` - Form input styling
- `Form.Actions` - Action button container

### **Typography** (Typography.axaml)

```xml
<!-- Usage Examples -->
<TextBlock Classes="Heading1" Text="Main Title"/>
<TextBlock Classes="Heading2" Text="Section Title"/>
<TextBlock Classes="Body" Text="Content text"/>
<TextBlock Classes="Caption" Text="Small text"/>

<!-- Status Text -->
<TextBlock Classes="Success" Text="Operation successful"/>
<TextBlock Classes="Error" Text="Error occurred"/>
<TextBlock Classes="Warning" Text="Warning message"/>
```

**Available Classes:**

- `Heading1` - Main page titles (32px)
- `Heading2` - Section headers (24px)
- `Heading3` - Subsection headers (20px)
- `Body` - Standard body text (16px)
- `Caption` - Small text (12px)
- `Success` - Success message styling
- `Error` - Error message styling
- `Warning` - Warning message styling
- `Info` - Information message styling

## Implementation Rules

### **Class Application Priority**

1. **Component Class First**: Apply the primary component class
2. **Modifier Classes**: Add size/state modifiers
3. **Context Classes**: Add manufacturing/domain-specific classes

```xml
<!-- CORRECT: Primary class + modifier -->
<Button Classes="Primary Large" Content="Main Action"/>

<!-- CORRECT: Component + context -->
<Button Classes="Icon MTM.QuickAction" Content="IN"/>
```

### **Style Inheritance**

```xml
<!-- Child elements inherit context -->
<Border Classes="Card">
    <!-- These automatically use Card.* styles -->
    <TextBlock Classes="Title" Text="Card Title"/>
    <TextBlock Classes="Content" Text="Card body text"/>
</Border>
```

### **Theme Integration**

All StyleSystem classes use Theme V2 semantic tokens automatically:

```xml
<!-- Styles internally reference semantic tokens -->
<!-- NO manual theme token references needed -->
<Button Classes="Primary" Content="Action"/>
<!-- Automatically gets ThemeV2.Action.Primary background -->
```

## Common Layout Patterns

### **Form Layout with Cards**

```xml
<Border Classes="Card">
    <Grid Classes="Form" RowDefinitions="Auto,*,Auto">
        
        <!-- Header -->
        <TextBlock Grid.Row="0" 
                   Classes="Heading2" 
                   Text="Edit Inventory"/>
        
        <!-- Content -->
        <StackPanel Grid.Row="1" Spacing="16">
            <TextBox Classes="Standard" 
                     Watermark="Part ID"/>
            <ComboBox Classes="Standard"
                      ItemsSource="{Binding Operations}"/>
        </StackPanel>
        
        <!-- Actions -->
        <StackPanel Grid.Row="2" 
                    Classes="Form.Actions"
                    Orientation="Horizontal" 
                    Spacing="8">
            <Button Classes="Primary" Content="Save"/>
            <Button Classes="Secondary" Content="Cancel"/>
        </StackPanel>
    </Grid>
</Border>
```

### **Icon Button Toolbar**

```xml
<StackPanel Classes="Toolbar" 
            Orientation="Horizontal" 
            Spacing="4">
    
    <Button Classes="Icon" ToolTip.Tip="Add">
        <materialIcons:MaterialIcon Kind="Add"/>
    </Button>
    
    <Button Classes="Icon" ToolTip.Tip="Edit">
        <materialIcons:MaterialIcon Kind="Edit"/>
    </Button>
    
    <Button Classes="Icon" ToolTip.Tip="Delete">
        <materialIcons:MaterialIcon Kind="Delete"/>
    </Button>
</StackPanel>
```

### **Manufacturing Transaction Panel**

```xml
<Border Classes="Card">
    <StackPanel Spacing="12">
        
        <TextBlock Classes="Heading3" Text="Quick Actions"/>
        
        <UniformGrid Columns="2" Classes="MTM.QuickButtons">
            <Button Classes="MTM.QuickAction.In" Content="IN"/>
            <Button Classes="MTM.QuickAction.Out" Content="OUT"/>
            <Button Classes="MTM.Transaction" Content="TRANSFER"/>
            <Button Classes="MTM.QuickAction" Content="CYCLE"/>
        </UniformGrid>
        
    </StackPanel>
</Border>
```

## Validation & Best Practices

### **Style Application Checklist**

- [ ] Use component classes instead of manual styling
- [ ] Apply size/state modifiers appropriately
- [ ] Use manufacturing context classes for MTM components
- [ ] Verify responsive behavior across different screen sizes
- [ ] Test with both light and dark themes

### **Performance Guidelines**

- [ ] Prefer built-in classes over local styles
- [ ] Avoid overriding StyleSystem classes locally
- [ ] Use semantic token references in local styles only when needed
- [ ] Keep local style definitions minimal

### **Consistency Rules**

- [ ] All buttons use Classes instead of individual styling
- [ ] Form layouts use Form.* classes for consistency
- [ ] Typography follows heading hierarchy (H1, H2, H3, Body, Caption)
- [ ] Status messages use appropriate semantic classes (Success, Error, Warning, Info)

## Custom Styles Integration

### **When to Create Local Styles**

Only create local styles for:

- Component-specific variations not covered by StyleSystem
- Temporary prototyping before adding to StyleSystem
- View-specific layout adjustments

### **Local Style Pattern**

```xml
<UserControl.Styles>
    <!-- Build on StyleSystem classes, don't replace them -->
    <Style Selector="Button.CustomVariant" BasedOn="{StaticResource {x:Type Button}}">
        <Setter Property="Classes" Value="Primary"/>
        <Setter Property="Margin" Value="8"/>
    </Style>
</UserControl.Styles>
```

### **Extending StyleSystem Classes**

```xml
<UserControl.Styles>
    <!-- Extend existing classes with additional properties -->
    <Style Selector="Button.Primary.CustomSpacing">
        <Setter Property="Margin" Value="16,8"/>
    </Style>
</UserControl.Styles>
```

## Migration from Manual Styles

### **Replace Manual Button Styling**

```xml
<!-- OLD: Manual styling -->
<Button Background="#0078D4" 
        Foreground="White"
        Padding="12,8"
        CornerRadius="4"
        Content="Action"/>

<!-- NEW: StyleSystem class -->
<Button Classes="Primary" Content="Action"/>
```

### **Replace Manual Typography**

```xml
<!-- OLD: Manual text styling -->
<TextBlock FontSize="24" 
           FontWeight="Bold"
           Foreground="#2D3748"
           Text="Section Title"/>

<!-- NEW: Typography class -->
<TextBlock Classes="Heading2" Text="Section Title"/>
```

### **Replace Manual Form Layouts**

```xml
<!-- OLD: Manual form styling -->
<StackPanel Margin="16" Spacing="12">
    <TextBlock FontWeight="Bold" Text="Label:"/>
    <TextBox Background="White" BorderBrush="#CBD5E0"/>
</StackPanel>

<!-- NEW: Form classes -->
<StackPanel Classes="Form" Spacing="12">
    <TextBlock Classes="Form.Label" Text="Label:"/>
    <TextBox Classes="Form.Input"/>
</StackPanel>
```

## Troubleshooting

### **Classes Not Applying**

1. Verify StyleSystem.axaml is included in App.axaml
2. Check class name spelling and casing
3. Ensure no conflicting local styles override StyleSystem
4. Verify Theme V2 is properly loaded

### **Inconsistent Appearance**

1. Use only StyleSystem classes, avoid manual styling
2. Check for competing style definitions
3. Verify proper class hierarchy (component → modifier → context)
4. Test in both light and dark themes

### **Missing Manufacturing Styles**

1. Verify Manufacturing/ActionButtons.axaml is included
2. Use MTM.* prefix for manufacturing-specific classes
3. Check that manufacturing semantic tokens are defined

## StyleSystem Maintenance

### **Adding New Component Styles**

1. Create new .axaml file in appropriate StyleSystem subfolder
2. Use Theme V2 semantic tokens exclusively
3. Follow existing naming conventions
4. Add to StyleSystem.axaml includes
5. Document available classes in this guide

### **Style File Organization**

```text
Resources/Styles/
├── Components/           <!-- StyleSystem component files -->
├── BaseStyles.axaml      <!-- Control defaults -->
└── StyleSystem.axaml     <!-- Master include file -->
```

All individual component style files should:

- Use semantic tokens from Theme V2
- Provide clear class naming
- Include size and state variants
- Support both light and dark themes
- Follow accessibility guidelines (WCAG 2.1 AA)
