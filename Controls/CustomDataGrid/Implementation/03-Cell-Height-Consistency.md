# CustomDataGrid - Cell Height Consistency Requirements

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🏗️ Cell Height Consistency Requirements

**CRITICAL**: All cells must be exactly 36px height to ensure perfect header-data alignment and professional DataGridView appearance.

## Cell Height Standards

### Universal Height Requirement
- **All cells**: `MinHeight="36" MaxHeight="36"`
- **No exceptions**: Every cell type must enforce this height
- **Content alignment**: All content must be vertically centered within the 36px cell

## Cell Styling Classes

### Standard Data and Header Cells
```xml
<!-- Header cells: 36px with consistent padding -->
<Style Selector="Border.header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Left" />
</Style>

<!-- Data cells: 36px with consistent padding -->
<Style Selector="Border.data-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Left" />
</Style>
```

### Checkbox Cells (Selection Column)
```xml
<!-- Checkbox cells: 36px with center alignment -->
<Style Selector="Border.checkbox-cell, Border.checkbox-header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Center" />
</Style>
```

### Action Cells (Buttons Column)
```xml
<!-- Action cells: 36px with reduced padding for buttons -->
<Style Selector="Border.action-cell, Border.action-header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="4,6" />
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
  <Setter Property="BorderThickness" Value="0,0,1,1" />
  <Setter Property="VerticalContentAlignment" Value="Center" />
  <Setter Property="HorizontalContentAlignment" Value="Center" />
</Style>
```

## Content Height Management

### TextBlock Content
```xml
<!-- Ensure TextBlocks don't exceed cell height -->
<TextBlock Text="{Binding PartId}"
           FontSize="12"
           VerticalAlignment="Center"
           HorizontalAlignment="Left"
           TextTrimming="CharacterEllipsis"
           ToolTip.Tip="{Binding PartId}" />
```

### CheckBox Content
```xml
<!-- CheckBoxes must fit within 36px cell -->
<CheckBox IsChecked="{Binding IsSelected}"
          VerticalAlignment="Center"
          HorizontalAlignment="Center"
          Margin="0"
          Padding="0" />
```

### Button Content in Actions Column
```xml
<!-- Buttons sized to fit within 36px with padding -->
<StackPanel Orientation="Horizontal"
            Spacing="2"
            VerticalAlignment="Center"
            HorizontalAlignment="Center">
  
  <Button Content="📝"
          Width="24"
          Height="24"
          Padding="2"
          FontSize="10"
          CornerRadius="2"
          Background="{DynamicResource MTM_Shared_Logic.PrimaryLightBrush}"
          Command="{Binding ReadNoteCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
          CommandParameter="{Binding}"
          ToolTip.Tip="Read Note" />
          
  <Button Content="🗑️"
          Width="24"
          Height="24"
          Padding="2"
          FontSize="10"
          CornerRadius="2"
          Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
          Foreground="White"
          Command="{Binding DeleteItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
          CommandParameter="{Binding}"
          ToolTip.Tip="Delete Item" />
</StackPanel>
```

## Height Enforcement Rules

### 1. No Variable Heights
- **Never** use `Auto` height for cells
- **Never** allow content to determine cell height
- **Always** enforce fixed 36px height across all cell types

### 2. Content Overflow Handling
- Use `TextTrimming="CharacterEllipsis"` for text that might overflow
- Provide tooltips for truncated content
- Scale button content to fit within cell boundaries

### 3. Padding Calculations
- **Standard cells**: 8px horizontal, 6px vertical = 12px total vertical used for padding
- **Content area**: 36px - 12px = 24px available for actual content
- **Checkbox cells**: 8px all sides = 16px total vertical padding, 20px content area
- **Action cells**: 4px horizontal, 6px vertical = 12px total vertical padding, 24px content area

### 4. Border Consistency
- All cells use `BorderThickness="0,0,1,1"` for right and bottom borders
- Last column and last row should adjust borders to avoid double-thickness

## Interactive State Heights

### Hover States
```xml
<Style Selector="Border.data-cell:pointerover">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}" />
  <!-- Height remains 36px - never change during interactions -->
</Style>
```

### Selection States
```xml
<Style Selector="Border.data-cell:selected">
  <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryLightBrush}" />
  <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
  <!-- Height remains 36px - never change during selection -->
</Style>
```

## Validation and Testing

### Height Verification
1. **Visual Inspection**: Use Avalonia DevTools to measure actual cell heights
2. **Alignment Testing**: Verify headers align with data across all columns
3. **Scrolling Test**: Ensure alignment maintains during vertical scrolling
4. **Content Testing**: Verify all content types fit properly within 36px constraint

### Common Height Issues to Avoid
- ❌ Allowing buttons to be larger than 24px (won't fit in 36px cell with padding)
- ❌ Using TextBlocks without height constraints
- ❌ Inconsistent padding between cell types
- ❌ Variable margin settings that affect effective height
- ❌ Border thickness variations that break alignment

---

**Next Implementation Phase**: [04-MTM-Design-System-Integration.md](./04-MTM-Design-System-Integration.md)