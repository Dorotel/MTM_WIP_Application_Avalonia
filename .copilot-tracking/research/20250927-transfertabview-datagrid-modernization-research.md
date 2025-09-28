# Task Research Notes: TransferTabView DataGrid Modernization

## Research Executed

### File Analysis

- `Views/MainForm/Panels/TransferTabView.axaml`
  - Currently uses TransferCustomDataGrid (needs replacement with standard Avalonia DataGrid)
  - Has complex layout with SearchConfiguration, DataGrid area, and Transfer controls
  - Uses MTM Theme V2 styling but has some hardcoded values that need StyleSystem migration
  - Contains EditInventoryView integration requirements but no current implementation
  
- `Views/MainForm/Panels/RemoveTabView.axaml`  
  - Successfully implemented standard Avalonia DataGrid with full MTM Theme V2 compliance
  - Uses ManufacturingField, ManufacturingForm, and ManufacturingActions style classes
  - Demonstrates proper DataGrid implementation pattern for MTM application
  - Shows correct styling patterns: Classes="Card", DynamicResource bindings, no hardcoded values

- `specs/001-transform-transfertabview-axaml/spec.md`
  - Defines 12 functional requirements for DataGrid replacement and column customization
  - Requires EditInventoryView integration within TransferTabView
  - Mandates MTM Theme V2 compliance with StyleSystem exclusive usage
  - Specifies transfer-specific columns: PartID, Operation, FromLocation, AvailableQuantity, TransferQuantity, Notes

### Code Search Results

- `TransferCustomDataGrid`
  - Found in TransferTabView.axaml at lines 173-273 (needs complete replacement)
  - Custom control that must be replaced with standard Avalonia DataGrid
  
- `DataGrid` usage patterns
  - RemoveTabView shows successful standard DataGrid implementation
  - Uses AutoGenerateColumns="True", IsReadOnly="True", proper styling
  - Demonstrates grid configuration: CanUserReorderColumns, CanUserResizeColumns, CanUserSortColumns

- `EditInventoryView` integration patterns
  - Found usage in overlay contexts in RemoveTabView (overlayViews:EditInventoryView)
  - Need to research direct inline integration approach for TransferTabView

### External Research

- #githubRepo:"AvaloniaUI/Avalonia DataGrid column customization dropdown"
  - Standard Avalonia DataGrid supports column visibility manipulation via DataGridColumn.IsVisible
  - ComboBox with MultiSelect or CheckBox list patterns for column selection
  - Column configuration can be persisted via user settings or ViewModel properties

- #fetch:<https://docs.avaloniaui.net/docs/reference/controls/datagrid>
  - DataGrid supports custom column templates and dynamic column visibility
  - AutoGenerateColumns can be disabled for manual column control
  - DataGridTemplateColumn allows custom content in columns

### Project Conventions

- Standards referenced: MTM Theme V2 semantic tokens, StyleSystem component classes
- Instructions followed: avalonia-ui-guidelines.instructions.md, theme-v2-implementation.instructions.md

## Key Discoveries

### Project Structure

RemoveTabView.axaml provides the definitive pattern for MTM DataGrid implementation:

```xml
<DataGrid x:Name="InventoryDataGrid"
          ItemsSource="{Binding InventoryItems}"
          IsVisible="{Binding HasInventoryItems}"
          AutoGenerateColumns="True"
          IsReadOnly="True"
          CanUserReorderColumns="True"
          CanUserResizeColumns="True"
          CanUserSortColumns="True"
          GridLinesVisibility="All"
          HeadersVisibility="All"
          Background="{DynamicResource MTM_Shared_Logic.DataGridBackgroundBrush}"
          BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
          BorderThickness="1"
          CornerRadius="4" />
```

### Implementation Patterns

TransferTabView requires these specific transformations:

1. **Replace TransferCustomDataGrid section (lines 173-273)** with standard Avalonia DataGrid
2. **Add column customization dropdown** above DataGrid using standard ComboBox with checkable items
3. **Integrate EditInventoryView directly in AXAML** rather than as overlay
4. **Migrate all hardcoded styling** to DynamicResource bindings with StyleSystem classes

### Complete Examples

**Standard MTM DataGrid Pattern:**

```xml
<Border Grid.Row="1" Classes="Card" Padding="16" Margin="0,4,0,4">
  <Grid RowDefinitions="Auto,*">
    <!-- Column Customization Dropdown -->
    <StackPanel Grid.Row="0" Orientation="Horizontal" Spacing="8" Margin="0,0,0,8">
      <TextBlock Classes="ManufacturingFieldLabel" Text="Visible Columns:" VerticalAlignment="Center"/>
      <ComboBox x:Name="ColumnSelector" 
                Classes="Standard"
                ItemsSource="{Binding AvailableColumns}"
                SelectedItems="{Binding SelectedColumns}"
                IsMultiSelect="True"
                Watermark="Select columns to display"/>
    </StackPanel>
    
    <!-- Standard DataGrid -->
    <DataGrid Grid.Row="1"
              x:Name="TransferDataGrid"
              ItemsSource="{Binding TransferItems}"
              IsVisible="{Binding HasTransferItems}"
              AutoGenerateColumns="False"
              IsReadOnly="True"
              CanUserReorderColumns="True"
              CanUserResizeColumns="True"
              CanUserSortColumns="True"
              GridLinesVisibility="All"
              HeadersVisibility="All"
              Background="{DynamicResource ThemeV2.Background.Surface}"
              BorderBrush="{DynamicResource ThemeV2.Border.Default}"
              BorderThickness="1"
              CornerRadius="{StaticResource ThemeV2.CornerRadius.Small}"
              SelectedItem="{Binding SelectedTransferItem}">
      
      <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID" Binding="{Binding PartID}" 
                            IsVisible="{Binding DataContext.ShowPartID, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        <DataGridTextColumn Header="Operation" Binding="{Binding Operation}"
                            IsVisible="{Binding DataContext.ShowOperation, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        <DataGridTextColumn Header="From Location" Binding="{Binding FromLocation}"
                            IsVisible="{Binding DataContext.ShowFromLocation, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        <DataGridTextColumn Header="Available Qty" Binding="{Binding AvailableQuantity}"
                            IsVisible="{Binding DataContext.ShowAvailableQuantity, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        <DataGridTextColumn Header="Transfer Qty" Binding="{Binding TransferQuantity}"
                            IsVisible="{Binding DataContext.ShowTransferQuantity, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
        <DataGridTextColumn Header="Notes" Binding="{Binding Notes}"
                            IsVisible="{Binding DataContext.ShowNotes, RelativeSource={RelativeSource AncestorType=UserControl}}"/>
      </DataGrid.Columns>
    </DataGrid>
  </Grid>
</Border>
```

**EditInventoryView Integration Pattern:**

```xml
<!-- Replace existing transfer controls section with integrated EditInventoryView -->
<Border Grid.Row="1" Classes="Card" Margin="0,0,0,8"
        IsVisible="{Binding SelectedTransferItem, Converter={StaticResource NullToBoolConverter}}">
  <views:EditInventoryView DataContext="{Binding EditInventoryViewModel}" />
</Border>
```

### API and Schema Documentation

Column configuration requires ViewModel properties for each column visibility:

- ShowPartID (bool) - Default: true
- ShowOperation (bool) - Default: true  
- ShowFromLocation (bool) - Default: true
- ShowAvailableQuantity (bool) - Default: false
- ShowTransferQuantity (bool) - Default: false
- ShowNotes (bool) - Default: false

### Configuration Examples

**Theme V2 Migration Examples:**

```xml
<!-- BEFORE: Hardcoded styling -->
<Button Background="#0078D4" Foreground="White" BorderThickness="1"/>

<!-- AFTER: StyleSystem + Theme V2 -->
<Button Classes="Primary"/>

<!-- BEFORE: Manual border styling -->
<Border Background="White" BorderBrush="#E0E0E0" BorderThickness="1" CornerRadius="4"/>

<!-- AFTER: Theme V2 semantic tokens -->
<Border Background="{DynamicResource ThemeV2.Background.Card}"
        BorderBrush="{DynamicResource ThemeV2.Border.Default}"
        BorderThickness="1"
        CornerRadius="{StaticResource ThemeV2.CornerRadius.Small}"/>
```

### Technical Requirements

1. **Remove TransferCustomDataGrid dependency completely** - Replace with standard Avalonia DataGrid
2. **Implement column visibility controls** - ComboBox with multi-select for 6 columns
3. **Direct EditInventoryView integration** - Inline UserControl reference, not overlay
4. **Complete Theme V2 migration** - All DynamicResource bindings, no hardcoded values
5. **Preserve existing functionality** - Search, transfer actions, validation, Quick buttons panel

## Recommended Approach

Based on successful RemoveTabView implementation and specification requirements, implement standard Avalonia DataGrid with:

1. **Standard DataGrid replacement** following RemoveTabView pattern exactly
2. **Column customization dropdown** above DataGrid using standard ComboBox with multi-select
3. **Direct EditInventoryView integration** replacing current transfer controls section
4. **Complete StyleSystem migration** removing all hardcoded values for Theme V2 compliance
5. **Maintain existing layout structure** preserving search configuration and action panels

## Implementation Guidance

- **Objectives**: Replace custom DataGrid with standard Avalonia DataGrid, add column customization, integrate EditInventoryView, ensure MTM Theme V2 compliance
- **Key Tasks**:
  1. Replace TransferCustomDataGrid section (lines 173-273) with standard DataGrid
  2. Add column customization ComboBox above DataGrid
  3. Replace transfer controls with EditInventoryView integration
  4. Migrate all hardcoded styling to DynamicResource + StyleSystem classes
- **Dependencies**: Standard Avalonia DataGrid, EditInventoryView UserControl, MTM Theme V2 tokens, StyleSystem classes
- **Success Criteria**: DataGrid displays transfer items with customizable columns, EditInventoryView integrates seamlessly, all styling uses Theme V2 semantic tokens, no hardcoded values remain
