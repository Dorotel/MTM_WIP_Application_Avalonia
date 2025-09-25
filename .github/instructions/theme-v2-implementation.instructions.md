---
description: 'Complete guide for implementing Theme V2 semantic token system in AXAML files for consistent, adaptive theming'
applyTo: '**/*.axaml'
---

# Theme V2 Implementation Guide

**The MTM Theme V2 system provides a comprehensive semantic token architecture for consistent, adaptive theming across light and dark modes.**

## Core Architecture

### Theme Layer Hierarchy

```text
App.axaml
├── FluentTheme (Avalonia base)
├── BaseStyles.axaml (control defaults)
├── Theme.Light.axaml & Theme.Dark.axaml (semantic tokens)
├── Tokens.axaml (primitive color/typography tokens)
└── StyleSystem.axaml (component-specific styles)
```

### Token Categories

#### **1. Background Tokens**

```xml
<!-- Surface backgrounds -->
{DynamicResource ThemeV2.Background.Canvas}      <!-- Main app background -->
{DynamicResource ThemeV2.Background.Surface}     <!-- Secondary surfaces -->
{DynamicResource ThemeV2.Background.Card}        <!-- Card/panel backgrounds -->

<!-- Interactive states -->
{DynamicResource ThemeV2.Background.Hover}       <!-- Hover states -->
{DynamicResource ThemeV2.Background.Pressed}     <!-- Pressed states -->
{DynamicResource ThemeV2.Background.Selected}    <!-- Selected states -->
```

#### **2. Content/Text Tokens**

```xml
<!-- Text hierarchy -->
{DynamicResource ThemeV2.Content.Primary}        <!-- Main content text -->
{DynamicResource ThemeV2.Content.Secondary}      <!-- Supporting text -->
{DynamicResource ThemeV2.Content.Tertiary}       <!-- Subtle text -->

<!-- Special contexts -->
{DynamicResource ThemeV2.Content.OnColor}        <!-- Text on colored backgrounds -->
{DynamicResource ThemeV2.Content.OnDark}         <!-- Text on dark surfaces -->
{DynamicResource ThemeV2.Content.Disabled}       <!-- Disabled text -->
{DynamicResource ThemeV2.Content.Placeholder}    <!-- Placeholder/watermark text -->
```

#### **3. Action/Interactive Tokens**

```xml
<!-- Primary actions -->
{DynamicResource ThemeV2.Action.Primary}         <!-- Primary buttons/actions -->
{DynamicResource ThemeV2.Action.Primary.Hover}   <!-- Primary hover state -->
{DynamicResource ThemeV2.Action.Primary.Pressed} <!-- Primary pressed state -->

<!-- Secondary actions -->
{DynamicResource ThemeV2.Action.Secondary}       <!-- Secondary buttons -->
{DynamicResource ThemeV2.Action.Secondary.Hover} <!-- Secondary hover -->
```

#### **4. Border/Outline Tokens**

```xml
{DynamicResource ThemeV2.Border.Default}         <!-- Standard borders -->
{DynamicResource ThemeV2.Border.Subtle}          <!-- Light borders -->
{DynamicResource ThemeV2.Border.Strong}          <!-- Emphasized borders -->
{DynamicResource ThemeV2.Border.Focus}           <!-- Focus indicators -->
```

#### **5. Input Field Tokens**

```xml
{DynamicResource ThemeV2.Input.Background}       <!-- Input backgrounds -->
{DynamicResource ThemeV2.Input.Border}           <!-- Input borders -->
{DynamicResource ThemeV2.Input.Content}          <!-- Input text -->
{DynamicResource ThemeV2.Input.Placeholder}      <!-- Input watermarks -->
```

#### **6. Status State Tokens**

```xml
{DynamicResource ThemeV2.Status.Success}         <!-- Success indicators -->
{DynamicResource ThemeV2.Status.Warning}         <!-- Warning indicators -->
{DynamicResource ThemeV2.Status.Error}           <!-- Error indicators -->
{DynamicResource ThemeV2.Status.Info}            <!-- Info indicators -->
```

## Implementation Rules

### **CRITICAL: Token Selection Rules**

#### **✅ Adaptive Token Usage**

- **Background elements**: Use `ThemeV2.Background.*` tokens
- **Primary text**: Use `ThemeV2.Content.Primary`
- **Secondary text**: Use `ThemeV2.Content.Secondary`
- **Text on colored backgrounds**: Use `ThemeV2.Content.OnColor`
- **Interactive elements**: Use `ThemeV2.Action.*` tokens

#### **❌ Never Use These Patterns**

```xml
<!-- WRONG: Hardcoded colors -->
<Button Background="Blue" Foreground="White"/>

<!-- WRONG: Direct color references -->
<Button Background="{StaticResource ThemeV2.Color.Blue.600}"/>

<!-- WRONG: Static resources for dynamic colors -->
<Button Background="{StaticResource ThemeV2.Action.Primary}"/>
```

#### **✅ Correct Implementation**

```xml
<!-- CORRECT: Semantic tokens with DynamicResource -->
<Button Background="{DynamicResource ThemeV2.Action.Primary}"
        Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>

<!-- CORRECT: Interactive states -->
<Button Classes="Primary">
    <materialIcons:MaterialIcon Kind="Save" 
                                Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
</Button>
```

### **Typography & Spacing Tokens**

#### **Font Sizes**

```xml
{StaticResource ThemeV2.Typography.Body.FontSize}      <!-- 16px -->
{StaticResource ThemeV2.Typography.Heading1.FontSize}  <!-- 32px -->
{StaticResource ThemeV2.Typography.Caption.FontSize}   <!-- 12px -->
```

#### **Spacing**

```xml
{StaticResource ThemeV2.Spacing.Small}    <!-- 8px Thickness -->
{StaticResource ThemeV2.Spacing.Medium}   <!-- 16px Thickness -->
{StaticResource ThemeV2.Spacing.Large}    <!-- 24px Thickness -->
```

#### **Corner Radius**

```xml
{StaticResource ThemeV2.CornerRadius.Small}   <!-- 4px -->
{StaticResource ThemeV2.CornerRadius.Medium}  <!-- 8px -->
{StaticResource ThemeV2.CornerRadius.Large}   <!-- 12px -->
```

## File Structure Requirements

### **AXAML File Header (MANDATORY)**

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:materialIcons="using:Material.Icons.Avalonia">
```

### **No Manual Theme Includes**

- **DO NOT** include theme files manually in individual AXAML files
- App.axaml handles all theme system integration
- Individual files only reference tokens via `{DynamicResource}`

### **Local Styles (When Needed)**

```xml
<UserControl.Styles>
    <!-- Component-specific styles using semantic tokens -->
    <Style Selector="Button.CustomAction">
        <Setter Property="Background" Value="{DynamicResource ThemeV2.Action.Primary}"/>
        <Setter Property="Foreground" Value="{DynamicResource ThemeV2.Content.OnColor}"/>
    </Style>
</UserControl.Styles>
```

## Common Implementation Patterns

### **Card/Panel Layout**

```xml
<Border Background="{DynamicResource ThemeV2.Background.Card}"
        BorderBrush="{DynamicResource ThemeV2.Border.Default}"
        BorderThickness="1"
        CornerRadius="{StaticResource ThemeV2.CornerRadius.Medium}"
        Padding="{StaticResource ThemeV2.Spacing.Medium}">
    
    <TextBlock Text="Card Content"
               Foreground="{DynamicResource ThemeV2.Content.Primary}"/>
</Border>
```

### **Form Inputs**

```xml
<TextBox Background="{DynamicResource ThemeV2.Input.Background}"
         BorderBrush="{DynamicResource ThemeV2.Input.Border}"
         Foreground="{DynamicResource ThemeV2.Input.Content}"
         Watermark="Enter text..."
         BorderThickness="1"
         CornerRadius="{StaticResource ThemeV2.CornerRadius.Small}"/>
```

### **Icon Buttons**

```xml
<Button Classes="Primary">
    <materialIcons:MaterialIcon Kind="Save"
                                Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
</Button>
```

### **Status Indicators**

```xml
<TextBlock Text="Success!"
           Foreground="{DynamicResource ThemeV2.Status.Success}"
           Classes="Body"/>
```

## Validation Checklist

### **Before Implementation**

- [ ] No hardcoded color values (`#FFFFFF`, `Blue`, etc.)
- [ ] No direct primitive token references (`ThemeV2.Color.Blue.600`)
- [ ] All color tokens use `{DynamicResource}`
- [ ] Typography tokens use `{StaticResource}`

### **After Implementation**

- [ ] Test in both Light and Dark themes
- [ ] Verify WCAG 2.1 AA contrast compliance
- [ ] Ensure all interactive states work properly
- [ ] Check theme switching works without restart

## Manufacturing-Specific Tokens

### **MTM Component Colors**

```xml
{DynamicResource ThemeV2.MTM.QuickButton.Background}
{DynamicResource ThemeV2.MTM.Inventory.In}       <!-- Green for IN transactions -->
{DynamicResource ThemeV2.MTM.Inventory.Out}      <!-- Red for OUT transactions -->
{DynamicResource ThemeV2.MTM.Inventory.Transfer} <!-- Blue for TRANSFER -->
```

### **CollapsiblePanel & Session History**

```xml
{DynamicResource ThemeV2.CollapsiblePanel.Header}
{DynamicResource ThemeV2.SessionHistory.Background}
{DynamicResource ThemeV2.SuccessOverlay.Background}
```

## Migration from Legacy Themes

### **Replace These Patterns**

```xml
<!-- OLD: Legacy theme references -->
<Button Background="{DynamicResource MTM_Shared_Logic.ButtonBackgroundBrush}"/>

<!-- NEW: Theme V2 semantic tokens -->
<Button Background="{DynamicResource ThemeV2.Action.Primary}"/>
```

### **Update Material Icons**

```xml
<!-- OLD: Hardcoded colors -->
<materialIcons:MaterialIcon Foreground="White"/>

<!-- NEW: Adaptive colors -->
<materialIcons:MaterialIcon Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
```

## Troubleshooting

### **Colors Not Updating on Theme Switch**

- Ensure using `{DynamicResource}` not `{StaticResource}`
- Check token exists in both Theme.Light.axaml and Theme.Dark.axaml

### **AVLN2000 Compilation Errors**

- Verify correct Avalonia namespace: `xmlns="https://github.com/avaloniaui"`
- Use `x:Name` not `Name` on Grid elements
- Check all referenced tokens exist

### **Contrast Issues**

- Use proper content tokens: `Primary` for main text, `Secondary` for supporting
- Use `OnColor` for text on colored backgrounds
- Test with actual content, not placeholder text

## Examples by Component Type

### **Buttons**

```xml
<!-- Primary Action -->
<Button Classes="Primary" Content="Save"/>

<!-- Secondary Action -->
<Button Classes="Secondary" Content="Cancel"/>

<!-- Icon Button -->
<Button Classes="Icon">
    <materialIcons:MaterialIcon Kind="Settings" 
                                Foreground="{DynamicResource ThemeV2.Content.OnColor}"/>
</Button>
```

### **Text Elements**

```xml
<!-- Heading -->
<TextBlock Classes="Heading2" Text="Section Title"/>

<!-- Body Text -->
<TextBlock Classes="Body" Text="Content text"/>

<!-- Status Text -->
<TextBlock Classes="Success" Text="Operation completed"/>
```

### **Form Layouts**

```xml
<Grid RowDefinitions="Auto,*,Auto"
      Background="{DynamicResource ThemeV2.Background.Canvas}">
    
    <!-- Header -->
    <Border Grid.Row="0" 
            Background="{DynamicResource ThemeV2.Background.Surface}"
            Padding="{StaticResource ThemeV2.Spacing.Medium}">
        <TextBlock Classes="Heading3" Text="Form Title"/>
    </Border>
    
    <!-- Content -->
    <ScrollViewer Grid.Row="1">
        <!-- Form fields -->
    </ScrollViewer>
    
    <!-- Actions -->
    <Border Grid.Row="2"
            Background="{DynamicResource ThemeV2.Background.Surface}"
            Padding="{StaticResource ThemeV2.Spacing.Small}">
        <Button Classes="Primary" Content="Submit"/>
    </Border>
</Grid>
```
