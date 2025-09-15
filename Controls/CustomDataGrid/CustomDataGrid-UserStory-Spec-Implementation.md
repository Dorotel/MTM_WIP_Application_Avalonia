# CustomDataGrid Control - User Story, Specification & Implementation

## User Story
**As an MTM inventory management user**, I need a high-performance, professional data grid that matches DataGridView standards with **perfect header-data alignment**, so that I can efficiently view, select, and manage inventory items with a consistent and intuitive interface.

**Acceptance Criteria:**
- Headers and data rows must align exactly (no column misalignment)
- All cells must have consistent height (36px for both headers and data cells)
- Professional appearance matching DataGridView standards
- Support for multi-selection with visual feedback
- Action buttons (Read Note, Delete, Context Menu) with proper theming
- Conditional checkmarks for items with notes
- MTM design system integration with dynamic theme resources
- Virtual scrolling for performance with large datasets
- Clean MVVM binding patterns following MTM Community Toolkit standards

## Technical Specification

### Architecture Overview
- **Control Type**: Avalonia UserControl with code-behind
- **Data Pattern**: ListBox with ItemTemplate for virtual scrolling performance
- **Styling**: Style Selectors with MTM design system DynamicResource bindings
- **Layout**: Grid-based with proportional column definitions
- **Selection**: Multi-selection support with Select All checkbox
- **Actions**: Command binding for Read Note, Delete, Edit, Duplicate, View Details

### Key Components Structure
```
CustomDataGrid.axaml
├── UserControl.Styles (Cell styling definitions)
├── Grid MainGrid (RowDefinitions="Auto,*", ColumnDefinitions="*,Auto,Auto")
│   ├── Border HeaderSection (Grid.Row="0")
│   │   └── Grid DynamicHeaderGrid (8 columns with proportional sizing)
│   ├── ScrollViewer DataScrollViewer (Grid.Row="1") 
│   │   └── ListBox DataListBox (Virtual scrolling with ItemTemplate)
│   ├── Border ColumnManagementContainer (Phase 3 - Disabled)
│   └── Border FilterPanelContainer (Phase 5 - Disabled)
```

### Column Layout Specification
**CRITICAL: All columns must use identical proportional sizing**

| Column | Width | Content | Header Cell Class | Data Cell Class |
|--------|-------|---------|-------------------|-----------------|
| Selection | `40` (fixed) | CheckBox | `checkbox-header-cell` | `checkbox-cell` |
| Part ID | `1.5*` (proportional) | PartId binding | `header-cell` | `data-cell` |
| Operation | `1*` (proportional) | Operation binding | `header-cell` | `data-cell` |
| Location | `1.2*` (proportional) | Location binding | `header-cell` | `data-cell` |
| Quantity | `1*` (proportional) | Quantity numeric format | `header-cell` | `data-cell` |
| Last Updated | `1.8*` (proportional) | DateTime format | `header-cell` | `data-cell` |
| Notes | `80` (fixed) | Conditional checkmark | `header-cell` | `data-cell` |
| Actions | `100` (fixed) | Button stack panel | `action-header-cell` | `action-cell` |
| Management | `40` (fixed) | Empty/disabled | `action-header-cell` | `data-cell` |

### Cell Height Consistency Requirements
**CRITICAL: All cells must be exactly 36px height**

```xml
<!-- Header cells: 36px with 8px horizontal padding, 6px vertical padding -->
<Style Selector="Border.header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
</Style>

<!-- Data cells: 36px with 8px horizontal padding, 6px vertical padding -->
<Style Selector="Border.data-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8,6" />
</Style>

<!-- Checkbox cells: 36px with 8px padding all sides -->
<Style Selector="Border.checkbox-cell, Border.checkbox-header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="8" />
</Style>

<!-- Action cells: 36px with 4px horizontal padding, 6px vertical padding -->
<Style Selector="Border.action-cell, Border.action-header-cell">
  <Setter Property="MinHeight" Value="36" />
  <Setter Property="MaxHeight" Value="36" />
  <Setter Property="Padding" Value="4,6" />
</Style>
```

### MTM Design System Integration
**Required Dynamic Resource Bindings:**

```xml
<!-- Backgrounds -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}" />

<!-- Borders -->
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" />
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}" />

<!-- Text Colors -->
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.HeadingText}" />
<Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}" />

<!-- Interactive States -->
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}" />
<Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryLightBrush}" />
<Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
```

## Data Binding Requirements

### Required Command Bindings
```xml
<!-- Selection -->
Click="OnSelectAllClick" (Select All checkbox)

<!-- Action Commands -->
Command="{Binding ReadNoteCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
Command="{Binding DeleteItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
Command="{Binding EditItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
Command="{Binding DuplicateItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
Command="{Binding ViewDetailsCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"

<!-- Management -->
Click="OnToggleColumnManagement" (Column management toggle - disabled)
```

### Required Data Context Bindings
```xml
<!-- Data Source -->
ItemsSource="{Binding ItemsSource, RelativeSource={RelativeSource AncestorType=UserControl}}"

<!-- Selection State -->
IsVisible="{Binding IsMultiSelectEnabled, RelativeSource={RelativeSource AncestorType=UserControl}}"
IsChecked="{Binding IsSelected}" (Individual item checkboxes)

<!-- Item Properties -->
Text="{Binding PartId}"
Text="{Binding Operation}"
Text="{Binding Location}"
Text="{Binding Quantity, StringFormat=N0}"
Text="{Binding LastUpdated, StringFormat='MM/dd/yy HH:mm'}"
ToolTip.Tip="{Binding LastUpdated, StringFormat='MM/dd/yyyy HH:mm:ss'}"

<!-- Conditional Display -->
IsVisible="{Binding HasNotes}" (Notes checkmark)
```

## Implementation Critical Points

### 1. AXAML Structure Integrity
- **Namespace**: Must be `xmlns="https://github.com/avaloniaui"` (Avalonia, NOT WPF)
- **Grid Names**: Use `x:Name` (never `Name` property to avoid AVLN2000 errors)
- **Column Definitions**: Consistent between header and data grids
- **Border Classes**: Exact match for style selectors

### 2. Cell Height Consistency
- **All cells must be exactly 36px**: MinHeight="36" MaxHeight="36"
- **Consistent Padding**: 8px horizontal for data, 6px vertical
- **No Height Conflicts**: Ensure no conflicting height settings in child controls

### 3. Action Button Layout
```xml
<StackPanel Orientation="Horizontal" 
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Spacing="2">
  <!-- Read Note Button: MaterialIcon NoteText, PrimaryAction color -->
  <!-- Delete Button: MaterialIcon Delete, ErrorBrush color -->
  <!-- Context Menu: MaterialIcon DotsVertical, BodyText color -->
</StackPanel>
```

### 4. Conditional Notes Checkmark
```xml
<materialIcons:MaterialIcon Kind="Check"
                            Width="16" Height="16"
                            Foreground="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
                            IsVisible="{Binding HasNotes}"
                            ToolTip.Tip="Item has notes - click Read Note button to view/edit" />
```

### 5. Virtual Scrolling Performance
- **ListBox with ItemsPanel**: StackPanel for proper virtual scrolling
- **ScrollViewer Settings**: HorizontalScrollBarVisibility="Disabled", VerticalScrollBarVisibility="Auto"
- **ItemTemplate**: Efficient binding without complex nested grids

## Phase Implementation Status

### Phase 1: ACTIVE (Current Implementation)
- [x] Basic grid layout and styling
- [x] Virtual scrolling with ListBox
- [x] Multi-selection support
- [x] Action command bindings
- [x] MTM design system integration
- [x] Conditional notes display

### Phase 2: Future Enhancement
- [ ] Sorting by clicking column headers
- [ ] Sort indicators (arrows) in headers
- [ ] Data binding for sort operations

### Phase 3: DISABLED (Future Feature)
- [ ] Column management panel
- [ ] Column visibility toggles
- [ ] Column reordering UI
- [ ] Column width persistence

### Phase 4: Future Enhancement
- [ ] Interactive column resizing
- [ ] Drag-and-drop column reordering
- [ ] Enhanced resize visual feedback

### Phase 5: DISABLED (Future Feature)
- [ ] Filter panel implementation
- [ ] Global search functionality
- [ ] Column-specific filters
- [ ] Filter persistence and presets

## Error Prevention Guidelines

### XML Corruption Prevention
- **Never edit large sections**: Make small, targeted changes
- **Validate after edits**: Ensure XML structure integrity
- **Use consistent indentation**: Maintain readability
- **Match opening/closing tags**: Prevent structure corruption

### AVLN2000 Prevention
- **Grid naming**: Always use `x:Name="SomeName"` not `Name="SomeName"`
- **Namespace consistency**: Use Avalonia namespace throughout
- **Property syntax**: Use proper Avalonia property syntax

### Alignment Issues Prevention
- **Consistent column definitions**: Header and data MUST use identical widths
- **Cell height uniformity**: All cells exactly 36px with consistent padding
- **Border thickness**: Consistent BorderThickness for seamless appearance

## Testing Requirements

### Visual Testing
- [ ] Header-data alignment verification (no gaps or overlaps)
- [ ] Cell height consistency across all rows
- [ ] Theme switching compatibility (all DynamicResource bindings work)
- [ ] Action button responsiveness and styling

### Functional Testing
- [ ] Multi-selection with Select All functionality
- [ ] Action command execution (Read Note, Delete, etc.)
- [ ] Conditional notes checkmark display
- [ ] Virtual scrolling performance with large datasets

### Cross-Platform Testing
- [ ] Windows 11 appearance and functionality
- [ ] macOS layout consistency
- [ ] Linux theme integration

This document serves as the definitive reference for rebuilding and maintaining the CustomDataGrid.axaml file while ensuring alignment issues are permanently resolved and professional DataGridView standards are maintained.