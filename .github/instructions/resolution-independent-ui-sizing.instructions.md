# Resolution-Independent UI Sizing for Material Design MTM Application

## Overview

This document defines the resolution-independent UI sizing strategy for the MTM WIP Application Material Design migration. The goal is to ensure consistent UI control sizes across all resolutions, UI scales, and device form factors while optimizing for manufacturing environments.

## Core Sizing Principles

### 1. Manufacturing-First Design

- **Touch Targets**: Minimum 44px for manufacturing tablet interfaces
- **Readability**: Optimal font sizes for industrial lighting conditions
- **Efficiency**: Consistent spacing for rapid operator interactions
- **Accessibility**: Meets WCAG guidelines for visual accessibility

### 2. Resolution Independence

- **Absolute Sizing**: UI controls maintain consistent physical sizes regardless of screen resolution
- **Scale Awareness**: Components adapt appropriately to OS UI scaling settings
- **Cross-Platform Consistency**: Identical visual presentation on Windows, macOS, Linux
- **Device Optimization**: Responsive design for tablets, desktops, and manufacturing terminals

## MTM Material Design Sizing Constants

```csharp
public static class MTMaterialSizing
{
    // Manufacturing Touch Targets
    public const double ManufacturingTouchTarget = 44.0;    // Primary manufacturing actions
    public const double SecondaryTouchTarget = 36.0;       // Secondary actions
    public const double CompactTouchTarget = 32.0;         // Compact controls
    public const double MinimalTouchTarget = 28.0;         // Minimal viable target
    
    // Input Controls
    public const double StandardInputHeight = 32.0;        // TextFields, ComboBoxes
    public const double CompactInputHeight = 28.0;         // Compact forms
    public const double LargeInputHeight = 40.0;          // Primary data entry
    public const double MultilineInputMinHeight = 64.0;   // TextArea controls
    public const double MultilineInputMaxHeight = 120.0;  // TextArea maximum
    
    // Button Controls  
    public const double StandardButtonHeight = 36.0;       // Material raised/outlined buttons
    public const double CompactButtonHeight = 32.0;        // Secondary buttons
    public const double LargeButtonHeight = 44.0;         // Primary action buttons
    public const double IconButtonSize = 40.0;            // Square icon buttons
    
    // Typography Scale
    public const double DisplayFontSize = 20.0;           // Page titles
    public const double HeadingFontSize = 18.0;           // Section headings  
    public const double SubheadingFontSize = 16.0;        // Subsection headings
    public const double BodyFontSize = 14.0;              // Standard body text
    public const double CaptionFontSize = 12.0;           // Helper/caption text
    public const double OverlineFontSize = 10.0;          // Labels/overlines
    
    // Icon Sizes
    public const double StandardIconSize = 24.0;          // Standard Material icons
    public const double LargeIconSize = 32.0;             // Prominent icons
    public const double CompactIconSize = 20.0;           // Compact contexts
    public const double SmallIconSize = 16.0;             // Inline icons
    
    // Spacing System (Material Design 8dp grid)
    public static Thickness ExtraLargePadding => new(32, 24);   // Major sections
    public static Thickness LargePadding => new(24, 16);        // Card/panel padding
    public static Thickness StandardPadding => new(16, 12);     // Standard spacing
    public static Thickness MediumPadding => new(12, 8);        // Medium density
    public static Thickness CompactPadding => new(8, 6);        // Compact contexts
    public static Thickness MinimalPadding => new(4, 3);        // Minimal spacing
    
    // Corner Radius
    public const double StandardCornerRadius = 8.0;       // Cards, panels
    public const double CompactCornerRadius = 4.0;        // Buttons, inputs
    public const double LargeCornerRadius = 12.0;         // Prominent containers
    
    // Elevation/Shadow
    public const double CardElevation = 2.0;              // Standard cards
    public const double RaisedElevation = 4.0;            // Raised components
    public const double FloatingElevation = 8.0;          // FABs, dialogs
    
    // Border Thickness
    public static Thickness StandardBorder => new(1);      // Standard borders
    public static Thickness ThickBorder => new(2);         // Emphasized borders
    public static Thickness ThinBorder => new(0.5);        // Subtle borders
}
```

## Sizing Implementation Patterns

### XAML Resource Dictionary Approach

```xml
<!-- MTMaterialSizing.axaml -->
<ResourceDictionary xmlns="https://github.com/avaloniaui">
    
    <!-- Touch Targets -->
    <x:Double x:Key="ManufacturingTouchTarget">44</x:Double>
    <x:Double x:Key="SecondaryTouchTarget">36</x:Double>
    <x:Double x:Key="CompactTouchTarget">32</x:Double>
    
    <!-- Input Heights -->
    <x:Double x:Key="StandardInputHeight">32</x:Double>
    <x:Double x:Key="LargeInputHeight">40</x:Double>
    
    <!-- Button Heights -->
    <x:Double x:Key="StandardButtonHeight">36</x:Double>
    <x:Double x:Key="LargeButtonHeight">44</x:Double>
    
    <!-- Typography -->
    <x:Double x:Key="DisplayFontSize">20</x:Double>
    <x:Double x:Key="HeadingFontSize">18</x:Double>
    <x:Double x:Key="BodyFontSize">14</x:Double>
    <x:Double x:Key="CaptionFontSize">12</x:Double>
    
    <!-- Spacing -->
    <Thickness x:Key="StandardPadding">16,12</Thickness>
    <Thickness x:Key="CompactPadding">8,6</Thickness>
    <Thickness x:Key="LargePadding">24,16</Thickness>
    
    <!-- Corner Radius -->
    <x:Double x:Key="StandardCornerRadius">8</x:Double>
    <x:Double x:Key="CompactCornerRadius">4</x:Double>
    
</ResourceDictionary>
```

### Material Component Sizing Application

```xml
<!-- Standard Material TextField -->
<material:TextField Text="{Binding PartID}"
                   Hint="Part ID"
                   Height="{StaticResource StandardInputHeight}"
                   Margin="{StaticResource CompactPadding}"
                   FontSize="{StaticResource BodyFontSize}" />

<!-- Manufacturing-Optimized Button -->
<material:Button Content="Save Part"
                Command="{Binding SaveCommand}"
                Height="{StaticResource ManufacturingTouchTarget}"
                MinWidth="{StaticResource ManufacturingTouchTarget}"
                Margin="{StaticResource StandardPadding}"
                FontSize="{StaticResource BodyFontSize}"
                CornerRadius="{StaticResource StandardCornerRadius}" />

<!-- Material Card with Consistent Sizing -->
<material:Card CornerRadius="{StaticResource StandardCornerRadius}"
              Elevation="2"
              Margin="{StaticResource StandardPadding}"
              Padding="{StaticResource LargePadding}">
    <!-- Card content -->
</material:Card>

<!-- Quick Action Button (Touch-Optimized) -->
<material:Button Style="{StaticResource MaterialDesignOutlinedButton}"
                Content="{Binding QuickActionText}"
                Command="{Binding QuickActionCommand}"
                Height="{StaticResource ManufacturingTouchTarget}"
                MinWidth="{StaticResource ManufacturingTouchTarget}"
                Margin="{StaticResource CompactPadding}"
                FontSize="{StaticResource BodyFontSize}" />
```

## View-Specific Sizing Guidelines

### MainWindow.axaml Sizing

```xml
<Window MinWidth="1200" MinHeight="800">
    <!-- Consistent window sizing across all platforms -->
    <material:DialogHost>
        <!-- Content maintains consistent sizing -->
    </material:DialogHost>
</Window>

```

### MainView.axaml Header Sizing

```xml
<!-- Material AppBar with consistent height -->
<Border Height="64" 
        Padding="{StaticResource LargePadding}">
    <Menu FontSize="{StaticResource BodyFontSize}">
        <!-- Menu items -->
    </Menu>

</Border>
```

### InventoryTabView.axaml Form Sizing

```xml
<!-- Manufacturing-optimized form layout -->
<material:Card Padding="{StaticResource LargePadding}"
              Margin="{StaticResource StandardPadding}">
    <StackPanel Spacing="16">
        
        <!-- Part ID Field -->
        <material:TextField Text="{Binding PartID}"
                           Hint="Part ID"
                           Height="{StaticResource StandardInputHeight}"
                           FontSize="{StaticResource BodyFontSize}" />
        
        <!-- Operation ComboBox -->
        <material:ComboBox SelectedItem="{Binding SelectedOperation}"
                          ItemsSource="{Binding Operations}"
                          Height="{StaticResource StandardInputHeight}"
                          FontSize="{StaticResource BodyFontSize}" />
        
        <!-- Primary Action Button -->
        <material:Button Content="Save Transaction"
                        Command="{Binding SaveCommand}"
                        Height="{StaticResource ManufacturingTouchTarget}"
                        FontSize="{StaticResource BodyFontSize}"
                        Style="{StaticResource MaterialDesignRaisedButton}" />

    </StackPanel>
</material:Card>
```

### QuickButtonsView.axaml Touch Optimization

```xml
<!-- Manufacturing quick action buttons -->
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <material:Button Content="{Binding DisplayText}"
                           Command="{Binding Command}"
                           Height="{StaticResource ManufacturingTouchTarget}"
                           MinWidth="{StaticResource ManufacturingTouchTarget}"
                           Margin="{StaticResource CompactPadding}"
                           FontSize="{StaticResource BodyFontSize}"
                           Style="{StaticResource MaterialDesignOutlinedButton}" />

        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Overlay Views Sizing

```xml
<!-- Success Overlay with consistent sizing -->
<material:DialogHost.DialogContent>
    <material:Card MinWidth="400" 
                  MaxWidth="600"
                  Padding="{StaticResource LargePadding}"
                  CornerRadius="{StaticResource StandardCornerRadius}">
        
        <StackPanel Spacing="16">
            <!-- Success Icon -->
            <material:Icon Kind="CheckCircle"
                          Width="48" Height="48"
                          HorizontalAlignment="Center" />
            
            <!-- Success Message -->
            <TextBlock Text="{Binding SuccessMessage}"
                      FontSize="{StaticResource HeadingFontSize}"
                      HorizontalAlignment="Center" />
            
            <!-- Action Button -->
            <material:Button Content="Continue"
                           Command="{Binding ContinueCommand}"
                           Height="{StaticResource StandardButtonHeight}"
                           FontSize="{StaticResource BodyFontSize}"
                           HorizontalAlignment="Center" />
        </StackPanel>

    </material:Card>
</material:DialogHost.DialogContent>
```

## Responsive Sizing Considerations

### Screen Size Adaptations

- **Desktop (1200px+)**: Standard sizing with comfortable spacing
- **Tablet (768-1199px)**: Enhanced touch targets, slightly larger spacing  
- **Manufacturing Terminal**: Optimized for industrial environments

### UI Scale Handling

- **100% Scale**: Base sizing values
- **125% Scale**: Proportional scaling maintained
- **150% Scale**: Components remain usable and accessible
- **200% Scale**: High DPI optimization

## Testing and Validation

### Size Validation Checklist

- [ ] Touch targets minimum 44px on manufacturing tablets
- [ ] Text remains readable at all supported UI scales
- [ ] Components don't overlap at different resolutions

- [ ] Consistent spacing maintained across views
- [ ] Material Design elevation and shadows render correctly

### Cross-Platform Size Testing

- [ ] Windows 10/11 at various UI scales
- [ ] macOS with Retina display scaling
- [ ] Linux with different DPI settings
- [ ] Manufacturing tablet form factors

### Manufacturing Environment Testing

- [ ] Readability under industrial lighting
- [ ] Touch accuracy with safety gloves
- [ ] Performance with high-volume transactions
- [ ] Operator efficiency compared to current design

This sizing strategy ensures the MTM WIP Application maintains consistent, accessible, and manufacturing-optimized UI sizing across all platforms and usage scenarios while leveraging Material Design visual principles.