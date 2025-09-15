# CustomDataGrid - Column Layout Specification

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🏗️ Column Layout Specification

**CRITICAL**: All columns must use identical proportional sizing between header grid and data grid to ensure perfect alignment.

## Column Definitions

| Column | Width | Content | Header Cell Class | Data Cell Class | Purpose |
|--------|-------|---------|-------------------|-----------------|---------|
| Selection | `40` (fixed) | CheckBox | `checkbox-header-cell` | `checkbox-cell` | Multi-select functionality |
| Part ID | `1.5*` (proportional) | PartId binding | `header-cell` | `data-cell` | Primary identifier |
| Operation | `1*` (proportional) | Operation binding | `header-cell` | `data-cell` | Manufacturing operation |
| Location | `1.2*` (proportional) | Location binding | `header-cell` | `data-cell` | Inventory location |
| Quantity | `1*` (proportional) | Quantity numeric format | `header-cell` | `data-cell` | Stock quantity |
| Last Updated | `1.8*` (proportional) | DateTime format | `header-cell` | `data-cell` | Timestamp display |
| Notes | `80` (fixed) | Conditional checkmark | `header-cell` | `data-cell` | Note indicator |
| Actions | `100` (fixed) | Button stack panel | `action-header-cell` | `action-cell` | User actions |
| Management | `40` (fixed) | Empty/disabled | `action-header-cell` | `data-cell` | Future expansion |

## Header Grid Definition

```xml
<Grid x:Name="DynamicHeaderGrid">
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="40" />      <!-- Selection -->
    <ColumnDefinition Width="1.5*" />    <!-- Part ID -->
    <ColumnDefinition Width="1*" />      <!-- Operation -->
    <ColumnDefinition Width="1.2*" />    <!-- Location -->
    <ColumnDefinition Width="1*" />      <!-- Quantity -->
    <ColumnDefinition Width="1.8*" />    <!-- Last Updated -->
    <ColumnDefinition Width="80" />      <!-- Notes -->
    <ColumnDefinition Width="100" />     <!-- Actions -->
    <ColumnDefinition Width="40" />      <!-- Management -->
  </Grid.ColumnDefinitions>
  
  <!-- Header cells with proper styling classes -->
</Grid>
```

## Data Grid ItemTemplate

```xml
<DataTemplate>
  <Grid>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="40" />      <!-- MUST match header exactly -->
      <ColumnDefinition Width="1.5*" />    <!-- MUST match header exactly -->
      <ColumnDefinition Width="1*" />      <!-- MUST match header exactly -->
      <ColumnDefinition Width="1.2*" />    <!-- MUST match header exactly -->
      <ColumnDefinition Width="1*" />      <!-- MUST match header exactly -->
      <ColumnDefinition Width="1.8*" />    <!-- MUST match header exactly -->
      <ColumnDefinition Width="80" />      <!-- MUST match header exactly -->
      <ColumnDefinition Width="100" />     <!-- MUST match header exactly -->
      <ColumnDefinition Width="40" />      <!-- MUST match header exactly -->
    </Grid.ColumnDefinitions>
    
    <!-- Data cells with proper styling classes -->
  </Grid>
</DataTemplate>
```

## Cell Content Specifications

### Selection Column (Column 0)
- **Header**: Select All checkbox with `Click="OnSelectAllClick"`
- **Data**: Individual selection checkbox with `IsChecked="{Binding IsSelected}"`
- **Styling**: Both use `checkbox-header-cell` and `checkbox-cell` classes
- **Padding**: 8px all sides

### Part ID Column (Column 1)
- **Header**: Static text "Part ID"
- **Data**: `Text="{Binding PartId}"` with TextBlock
- **Styling**: Standard `header-cell` and `data-cell` classes
- **Padding**: 8px horizontal, 6px vertical

### Operation Column (Column 2)
- **Header**: Static text "Operation"
- **Data**: `Text="{Binding Operation}"` with TextBlock
- **Styling**: Standard `header-cell` and `data-cell` classes

### Location Column (Column 3)
- **Header**: Static text "Location"
- **Data**: `Text="{Binding Location}"` with TextBlock
- **Styling**: Standard `header-cell` and `data-cell` classes

### Quantity Column (Column 4)
- **Header**: Static text "Quantity"
- **Data**: `Text="{Binding Quantity, StringFormat=N0}"` with TextBlock
- **Alignment**: Right-aligned for numeric display
- **Styling**: Standard `header-cell` and `data-cell` classes

### Last Updated Column (Column 5)
- **Header**: Static text "Last Updated"
- **Data**: `Text="{Binding LastUpdated, StringFormat='MM/dd/yy HH:mm'}"`
- **Tooltip**: `ToolTip.Tip="{Binding LastUpdated, StringFormat='MM/dd/yyyy HH:mm:ss'}"`
- **Styling**: Standard `header-cell` and `data-cell` classes

### Notes Column (Column 6)
- **Header**: Static text "Notes"
- **Data**: Conditional checkmark `IsVisible="{Binding HasNotes}"`
- **Content**: "✓" character for items with notes
- **Styling**: Standard `header-cell` and `data-cell` classes

### Actions Column (Column 7)
- **Header**: Static text "Actions"
- **Data**: StackPanel with action buttons (Read Note, Delete, etc.)
- **Commands**: All buttons use `RelativeSource={RelativeSource AncestorType=UserControl}` binding
- **Styling**: `action-header-cell` and `action-cell` classes

### Management Column (Column 8)
- **Header**: Column management toggle button (disabled for Phase 1)
- **Data**: Empty content (reserved for future features)
- **Styling**: `action-header-cell` and `data-cell` classes

## Critical Alignment Requirements

### 1. Identical Column Definitions
Both header grid and data template MUST use exactly the same column definitions to prevent alignment issues.

### 2. Consistent Cell Height
All cells must be exactly 36px height with proper padding to ensure row alignment.

### 3. Border and Spacing Consistency
All cells must use consistent border thickness and margin settings.

### 4. ScrollViewer Coordination
Header grid must remain fixed while data grid scrolls, maintaining alignment across all scroll positions.

---

**Next Implementation Phase**: [03-Cell-Height-Consistency.md](./03-Cell-Height-Consistency.md)