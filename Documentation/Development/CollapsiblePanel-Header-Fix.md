# CollapsiblePanel Header Property Fix

## Issue Summary
The application was failing to build with error:
```
AVLN2000: Unable to resolve suitable regular or attached property Header on type MTM_WIP_Application_Avalonia:MTM_WIP_Application_Avalonia.Controls.CollapsiblePanel
```

This occurred because the `InventoryTabView.axaml` was trying to use `<controls:CollapsiblePanel.Header>` but the CollapsiblePanel control didn't have a Header property defined.

## Changes Made

### 1. Added Header Property to CollapsiblePanel.axaml.cs
```csharp
public static readonly StyledProperty<object?> HeaderProperty =
    AvaloniaProperty.Register<CollapsiblePanel, object?>(nameof(Header));

public object? Header
{
    get => GetValue(HeaderProperty);
    set => SetValue(HeaderProperty, value);
}
```

### 2. Updated CollapsiblePanel.axaml Template
- Added a `ContentPresenter` for the Header content
- Updated the HeaderContentGrid layout to accommodate both toggle button and header content
- Added proper spacing and alignment for the header content

```xml
<!-- Header Content Presenter -->
<ContentPresenter x:Name="HeaderPresenter"
                Grid.Column="1"
                Content="{TemplateBinding Header}"
                VerticalAlignment="Center"
                Margin="8,0,0,0"/>
```

### 3. Fixed Method Hiding Warning
- Added `new` keyword to `UpdateLayout()` method to properly hide the inherited member
- This resolves the CS0108 warning about hiding inherited members

## Result
✅ **Build Error Fixed**: The AVLN2000 error is resolved
✅ **Header Property Working**: CollapsiblePanel now supports custom header content
✅ **Template Updated**: Header content is properly displayed alongside the toggle button
✅ **Method Warning Fixed**: No more CS0108 warnings about method hiding

## Usage Example
The CollapsiblePanel can now be used with custom header content:
```xml
<controls:CollapsiblePanel HeaderPosition="Top" IsExpanded="True">
  <controls:CollapsiblePanel.Header>
    <Grid>
      <materialIcons:MaterialIcon Kind="History" />
      <TextBlock Text="Session History" Margin="8,0,0,0" />
    </Grid>
  </controls:CollapsiblePanel.Header>
  
  <!-- Panel content goes here -->
</controls:CollapsiblePanel>
```

The application should now build successfully and the startup infrastructure can be tested properly.
