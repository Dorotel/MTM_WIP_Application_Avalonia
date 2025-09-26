# MTM Style System Documentation

## Overview

The MTM Style System provides a comprehensive, centralized collection of UI styles for the MTM WIP Application. All styles are Theme V2 compatible, WCAG 2.1 AA compliant, and follow the established MTM design system patterns.

## Quick Start

### Including Styles

To include all MTM styles in your view, add the master StyleSystem.axaml file:

```xml
<UserControl.Styles>
    <StyleInclude Source="/Resources/Styles/StyleSystem.axaml"/>
</UserControl.Styles>
```

For individual categories, include specific files:

```xml
<UserControl.Styles>
    <StyleInclude Source="/Resources/Styles/Buttons/PrimaryButtons.axaml"/>
    <StyleInclude Source="/Resources/Styles/Typography/TextStyles.axaml"/>
</UserControl.Styles>
```

## Style Categories

### 1. Button Styles

#### Primary Buttons (`/Resources/Styles/Buttons/PrimaryButtons.axaml`)

Main action buttons with strong visual prominence:

```xml
<!-- Standard primary button -->
<Button Classes="Primary" Content="Save Changes"/>

<!-- Small variant -->
<Button Classes="Primary Small" Content="Quick Save"/>

<!-- With icon support -->
<Button Classes="Primary WithIcon" Content="📊 Generate Report"/>
```

**Available Classes:**

- `Primary` - Standard primary button
- `Primary Small` - Compact primary button
- `Primary WithIcon` - Primary button optimized for icons

#### Secondary Buttons (`/Resources/Styles/Buttons/SecondaryButtons.axaml`)

Supporting action buttons with subtle styling:

```xml
<!-- Standard secondary button -->
<Button Classes="Secondary" Content="Cancel"/>

<!-- Small variant -->
<Button Classes="Secondary Small" Content="Reset"/>

<!-- Outline style -->
<Button Classes="Secondary Outline" Content="More Options"/>
```

**Available Classes:**

- `Secondary` - Standard secondary button
- `Secondary Small` - Compact secondary button  
- `Secondary Outline` - Outline-only secondary button

#### Icon Buttons (`/Resources/Styles/Buttons/IconButtons.axaml`)

Compact, icon-focused buttons for toolbar and inline actions:

```xml
<!-- Standard icon button -->
<Button Classes="Icon" Content="⚙️" ToolTip.Tip="Settings"/>

<!-- Round variant -->
<Button Classes="Icon Round" Content="+" ToolTip.Tip="Add Item"/>

<!-- Primary icon button -->
<Button Classes="Icon Primary" Content="📋" ToolTip.Tip="Copy"/>

<!-- Danger icon button -->
<Button Classes="Icon Danger" Content="🗑️" ToolTip.Tip="Delete"/>
```

**Available Classes:**

- `Icon` - Standard icon button
- `Icon Round` - Circular icon button
- `Icon Primary` - Primary-colored icon button
- `Icon Danger` - Danger-colored icon button

### 2. Manufacturing Action Buttons

#### Manufacturing Actions (`/Resources/Styles/Manufacturing/ActionButtons.axaml`)

Specialized buttons for MTM manufacturing operations:

```xml
<!-- Quick action buttons -->
<Button Classes="QuickAction" Content="Quick Add"/>

<!-- Transfer operations -->
<Button Classes="Transfer" Content="Transfer"/>

<!-- Delete operations -->
<Button Classes="Delete" Content="Remove"/>

<!-- Header toggles -->
<Button Classes="HeaderToggle" Content="▼ Expand"/>
```

**Available Classes:**

- `QuickAction` - Manufacturing quick actions
- `Transfer` - Transfer operation buttons  
- `Delete` - Delete/remove operation buttons
- `HeaderToggle` - Collapsible header toggles

### 3. Input Controls

#### Text Inputs (`/Resources/Styles/Inputs/TextInputs.axaml`)

Text input fields with validation states and variants:

```xml
<!-- Standard text input -->
<TextBox Classes="InputField" Watermark="Enter part number"/>

<!-- Search input -->
<TextBox Classes="InputField Search" Watermark="Search inventory..."/>

<!-- Error state -->
<TextBox Classes="InputField Error" Text="Invalid input" Watermark="Enter valid data"/>

<!-- Success state -->
<TextBox Classes="InputField Success" Text="Valid data" Watermark="Data validated"/>

<!-- Multiline input -->
<TextBox Classes="InputField Multiline" AcceptsReturn="True" TextWrapping="Wrap" Height="80"/>

<!-- AutoComplete field -->
<TextBox Classes="InputField AutoComplete" Watermark="Type to search..."/>
```

**Available Classes:**

- `InputField` - Standard text input
- `InputField Search` - Search-optimized input
- `InputField Error` - Error state input
- `InputField Success` - Success state input
- `InputField Multiline` - Multi-line text input
- `InputField AutoComplete` - AutoComplete text input

#### ComboBox Controls (`/Resources/Styles/Inputs/ComboBoxes.axaml`)

Dropdown selection controls with consistent styling:

```xml
<!-- Standard dropdown -->
<ComboBox Classes="InputField" PlaceholderText="Select option">
    <ComboBoxItem Content="Option 1"/>
    <ComboBoxItem Content="Option 2"/>
</ComboBox>
```

**Available Classes:**

- `InputField` - Standard ComboBox styling (matches TextBox styling)

### 4. Layout Components

#### Cards (`/Resources/Styles/Layout/Cards.axaml`)

Container components with elevation and status variants:

```xml
<!-- Standard card -->
<Border Classes="Card">
    <StackPanel Spacing="8">
        <TextBlock Classes="Heading4" Text="Card Title"/>
        <TextBlock Classes="Body" Text="Card content goes here."/>
    </StackPanel>
</Border>

<!-- Header panel card -->
<Border Classes="Card HeaderPanel">
    <!-- Content with prominent header styling -->
</Border>

<!-- Footer panel card -->
<Border Classes="Card FooterPanel">
    <!-- Content with action footer area -->
</Border>

<!-- Status cards -->
<Border Classes="Card Success">
    <!-- Success-themed card -->
</Border>

<Border Classes="Card Warning">
    <!-- Warning-themed card -->
</Border>

<Border Classes="Card Error">
    <!-- Error-themed card -->
</Border>
```

**Available Classes:**

- `Card` - Standard card container
- `Card HeaderPanel` - Card with prominent header
- `Card FooterPanel` - Card with action footer
- `Card Success` - Success-themed card
- `Card Warning` - Warning-themed card
- `Card Error` - Error-themed card

#### Forms (`/Resources/Styles/Layout/Forms.axaml`)

Form field layouts and structured input patterns:

```xml
<!-- Form field layout -->
<Grid Classes="FormField" ColumnDefinitions="Auto,*">
    <TextBlock Grid.Column="0" Classes="Label" Text="Part ID:" VerticalAlignment="Center"/>
    <TextBox Grid.Column="1" Classes="InputField" Watermark="Enter part number"/>
</Grid>

<!-- Form actions area -->
<Grid Classes="FormActions">
    <StackPanel Orientation="Horizontal" Spacing="8" HorizontalAlignment="Right">
        <Button Classes="Secondary" Content="Reset"/>
        <Button Classes="Primary" Content="Submit"/>
    </StackPanel>
</Grid>
```

**Available Classes:**

- `FormField` - Structured form field layout
- `FormActions` - Form action button container

### 5. Navigation Controls

#### Tabs, Menus, and Navigation (`/Resources/Styles/Navigation/Tabs.axaml`)

Navigation components with consistent MTM styling:

```xml
<!-- Tab control -->
<TabControl Classes="MTMTabs">
    <TabItem Header="📦 Inventory">
        <!-- Tab content -->
    </TabItem>
    <TabItem Header="🔄 Transfer">
        <!-- Tab content -->
    </TabItem>
</TabControl>

<!-- Menu -->
<Menu Classes="MTMMenu">
    <MenuItem Header="📁 File">
        <MenuItem Header="📄 New"/>
        <MenuItem Header="📂 Open"/>
    </MenuItem>
</Menu>

<!-- Breadcrumb navigation -->
<StackPanel Classes="Breadcrumb" Orientation="Horizontal" Spacing="8">
    <Button Classes="BreadcrumbItem" Content="Home"/>
    <TextBlock Classes="BreadcrumbSeparator" Text="›" VerticalAlignment="Center"/>
    <Button Classes="BreadcrumbItem" Content="Inventory"/>
    <TextBlock Classes="BreadcrumbSeparator" Text="›" VerticalAlignment="Center"/>
    <TextBlock Classes="BreadcrumbCurrent" Text="Parts" VerticalAlignment="Center"/>
</StackPanel>
```

**Available Classes:**

- `MTMTabs` - Enhanced TabControl styling
- `MTMMenu` - Enhanced Menu styling  
- `Breadcrumb` - Breadcrumb container
- `BreadcrumbItem` - Breadcrumb navigation button
- `BreadcrumbSeparator` - Breadcrumb separator text
- `BreadcrumbCurrent` - Current breadcrumb location text

### 6. Typography System

#### Text Styles (`/Resources/Styles/Typography/TextStyles.axaml`)

Comprehensive typography hierarchy with semantic meaning:

##### Headings

```xml
<TextBlock Classes="Heading1" Text="Main Page Title"/>
<TextBlock Classes="Heading2" Text="Section Header"/>
<TextBlock Classes="Heading3" Text="Subsection Header"/>
<TextBlock Classes="Heading4" Text="Card Title"/>
<TextBlock Classes="Heading5" Text="Form Group Label"/>
<TextBlock Classes="Heading6" Text="Field Label"/>
```

##### Body Text

```xml
<TextBlock Classes="BodyLarge" Text="Important content and descriptions"/>
<TextBlock Classes="Body" Text="Standard content and interface text"/>
<TextBlock Classes="BodySmall" Text="Secondary information and captions"/>
<TextBlock Classes="Caption" Text="Metadata, timestamps, and fine print"/>
<TextBlock Classes="Label" Text="Form field and control labels"/>
```

##### Content Hierarchy

```xml
<TextBlock Classes="Body Primary" Text="Main text and important information"/>
<TextBlock Classes="Body Secondary" Text="Supporting text and descriptions"/>
<TextBlock Classes="Body Tertiary" Text="Subtle information and hints"/>
<TextBlock Classes="Body Disabled" Text="Inactive or unavailable text"/>
```

##### Status Text

```xml
<TextBlock Classes="Body Success" Text="Operation completed successfully"/>
<TextBlock Classes="Body Warning" Text="Attention required"/>
<TextBlock Classes="Body Error" Text="Operation failed or validation error"/>
<TextBlock Classes="Body Info" Text="General notification or helpful tip"/>
```

##### Special Purpose Text

```xml
<TextBlock Classes="Link" Text="Interactive navigation and actions"/>
<TextBlock Classes="Code" Text="Part IDs, system codes"/>
<TextBlock Classes="Emphasized" Text="Important highlights"/>
<TextBlock Classes="Subtle" Text="Less important information"/>
```

##### Manufacturing-Specific Text

```xml
<TextBlock Classes="PartID" Text="PART001"/>
<TextBlock Classes="OperationNumber" Text="100"/>
<TextBlock Classes="Quantity" Text="1,250"/>
```

**Available Typography Classes:**

- **Headings**: `Heading1`, `Heading2`, `Heading3`, `Heading4`, `Heading5`, `Heading6`
- **Body Text**: `Body`, `BodyLarge`, `BodySmall`, `Caption`, `Label`
- **Content Hierarchy**: `Primary`, `Secondary`, `Tertiary`, `Disabled`
- **Status**: `Success`, `Warning`, `Error`, `Info`
- **Special**: `Link`, `Code`, `Emphasized`, `Subtle`
- **Manufacturing**: `PartID`, `OperationNumber`, `Quantity`

## Theme Integration

All styles integrate with MTM Theme V2 semantic tokens:

### Color Tokens Used

- `ThemeV2.Action.Primary` - Primary action colors
- `ThemeV2.Action.Secondary` - Secondary action colors
- `ThemeV2.Status.Success` - Success state colors
- `ThemeV2.Status.Warning` - Warning state colors
- `ThemeV2.Status.Error` - Error state colors
- `ThemeV2.Content.Primary` - Primary text colors
- `ThemeV2.Content.Secondary` - Secondary text colors
- `ThemeV2.Background.Default` - Background colors
- `ThemeV2.Border.Default` - Border colors

### Typography Tokens Used

- `ThemeV2.Typography.*` - Font sizes, weights, and line heights
- `ThemeV2.FontSize.*` - Semantic font size scale
- `ThemeV2.FontWeight.*` - Font weight values

## Accessibility

All styles meet WCAG 2.1 AA standards:

- **Contrast Ratios**: Minimum 4.5:1 for normal text, 3:1 for large text
- **Focus Indicators**: Visible focus states on all interactive elements
- **Color Independence**: Information not conveyed by color alone
- **Touch Targets**: Minimum 44px touch target size for interactive elements

## Usage Examples

### Complete Form Example

```xml
<Border Classes="Card">
    <StackPanel Spacing="16">
        <TextBlock Classes="Heading3" Text="Add Inventory Item"/>
        
        <Grid Classes="FormField" ColumnDefinitions="Auto,*,Auto,*">
            <TextBlock Grid.Column="0" Classes="Label" Text="Part ID:" VerticalAlignment="Center"/>
            <TextBox Grid.Column="1" Classes="InputField" Watermark="Enter part number"/>
            
            <TextBlock Grid.Column="2" Classes="Label" Text="Quantity:" VerticalAlignment="Center"/>
            <TextBox Grid.Column="3" Classes="InputField" Watermark="Enter quantity"/>
        </Grid>
        
        <Grid Classes="FormActions">
            <StackPanel Orientation="Horizontal" Spacing="8" HorizontalAlignment="Right">
                <Button Classes="Secondary" Content="Reset"/>
                <Button Classes="Primary" Content="Add Item"/>
            </StackPanel>
        </Grid>
    </StackPanel>
</Border>
```

### Status Card Example

```xml
<Border Classes="Card Success">
    <StackPanel Spacing="8">
        <TextBlock Classes="Heading4" Text="Transaction Complete"/>
        <TextBlock Classes="Body Success" Text="✓ 25 items added to inventory"/>
        <TextBlock Classes="PartID" Text="PART001"/>
        <TextBlock Classes="OperationNumber" Text="Operation: 100"/>
    </StackPanel>
</Border>
```

## Demo Window

Access the complete style demonstration via the main application:
**File → Style Demo**

The demo window showcases all available styles with live examples, organized by category with interactive elements to test functionality.

## File Structure

```
Resources/Styles/
├── StyleSystem.axaml           # Master include file
├── Buttons/
│   ├── PrimaryButtons.axaml    # Primary action buttons
│   ├── SecondaryButtons.axaml  # Secondary action buttons
│   └── IconButtons.axaml       # Icon-focused buttons
├── Manufacturing/
│   └── ActionButtons.axaml     # Manufacturing-specific buttons
├── Inputs/
│   ├── TextInputs.axaml        # Text input controls
│   └── ComboBoxes.axaml        # Dropdown controls
├── Layout/
│   ├── Cards.axaml             # Card containers
│   └── Forms.axaml             # Form layouts
├── Navigation/
│   └── Tabs.axaml              # Navigation controls
└── Typography/
    └── TextStyles.axaml        # Typography system
```

## Best Practices

1. **Use Semantic Classes**: Choose classes that describe purpose, not appearance
2. **Include StyleSystem.axaml**: Use the master include file for complete coverage
3. **Follow Hierarchy**: Use heading levels appropriately (H1 for page titles, H2 for sections, etc.)
4. **Status Colors**: Use status classes (Success, Warning, Error, Info) consistently
5. **Form Patterns**: Use FormField and FormActions grids for consistent form layouts
6. **Accessibility**: Always include ToolTip.Tip for icon buttons
7. **Theme Compatibility**: Styles automatically adapt to theme changes

## Migration from Hardcoded Styles

When replacing hardcoded styles:

1. **Identify Purpose**: Determine the semantic purpose of the element
2. **Choose Appropriate Class**: Select the class that matches the purpose
3. **Remove Inline Styling**: Replace hardcoded properties with class references
4. **Test Theme Switching**: Verify appearance across all MTM themes
5. **Validate Accessibility**: Ensure proper contrast and focus states

## Support

For questions about the style system or to report issues:

1. Check the Style Demo window for visual examples
2. Review this documentation for usage patterns
3. Examine existing implementations in MTM views
4. Follow established MTM patterns for consistency
