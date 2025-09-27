---
title: "MTM AXAML StyleSystem Transformation Research"
description: "Comprehensive analysis and research for RemoveTabView.axaml transformation"
date: "20250927"
target_file: "RemoveTabView.axaml"
phase: "Research"
---

## MTM AXAML StyleSystem Transformation Research

**Target File**: `RemoveTabView.axaml`
**Date**: 20250927
**Transformation Type**: StyleSystem + Theme V2 Integration
**Additional Requirements**: Replace CustomDataGrid with standard Avalonia DataGrid, implement customizable columns, integrate EditInventoryView seamlessly, remove all hardcoded styling

## Executive Summary

### Transformation Objective

Transform RemoveTabView.axaml to replace CustomDataGrid with standard Avalonia DataGrid while adding column customization dropdown, integrating EditInventoryView panel switching, and converting all hardcoded styling to Theme V2 + StyleSystem implementation.

### Complexity Assessment

- **Difficulty Level**: High
- **Estimated Duration**: 2-3 hours
- **Risk Factors**: CustomDataGrid dependency removal, maintaining EditInventoryView integration, ensuring proper column mapping to MySQL schema

### Success Criteria

- [ ] All hardcoded styling replaced with StyleSystem classes
- [ ] Theme V2 semantic tokens implemented
- [ ] Business logic preservation verified
- [ ] AVLN2000 compliance achieved
- [ ] CustomDataGrid completely replaced with standard DataGrid
- [ ] Column customization dropdown implemented
- [ ] EditInventoryView integration maintained

## Current File Analysis

### File Structure

```xml
Current RemoveTabView.axaml structure:
- Root Grid (RootContainer) with overlay positioning
- MainContainer Grid with RowDefinitions="*,Auto" 
- Data Display Section (Border with Card class)
  - SearchPanel (CollapsiblePanel)
  - DataContainer with CustomDataGrid
- Action Buttons Panel (Border with Card.Actions class)
- Multiple overlay sections (NoteEditor, EditInventory, Confirmation)
```

### Business Requirements

The RemoveTabView.axaml provides inventory removal functionality with:

- Search filtering by PartID and Operation
- CustomDataGrid display of inventory items with selection
- Multiple action buttons (Search, Delete, Reset, Undo, Print, Advanced)
- EditInventoryView integration via overlay
- Confirmation dialogs for deletions
- Note editor overlay for item notes

### Current Styling Approach

**Hardcoded Styling Identified**:

- Grid margins: `Margin="8"`, `Margin="0,0,0,8"`, `Margin="0,0,0,16"`
- Card padding and spacing
- Button width: `Width="100"`
- Container sizing: `MinWidth="600"`, `MinHeight="400"`, `MaxHeight="400"`
- Form spacing: `RowSpacing="12"`, `Spacing="6"`, `ColumnSpacing="12"`

### Dependencies Identified

**Controls and Components**:

- `customDataGrid:CustomDataGrid` (TO BE REPLACED)
- `controls:CollapsiblePanel`
- `materialIcons:MaterialIcon`
- `views:NoteEditorView`
- `overlayViews:EditInventoryView`
- `views:ConfirmationOverlayView`

**Converters**: `conv:NullToBoolConverter`

**ViewModels**: `apivm:RemoveItemViewModel`

## StyleSystem Requirements

### Required StyleSystem Classes

**Layout Components**:

- `ManufacturingTabView` (already used)
- `Card` (already used)
- `Card.Actions` (already used)
- `Form` (partially used)
- `Form.Container` (needed)
- `Form.Input` (needed)
- `Form.Label` (needed)

**Button Components**:

- `Button.Primary` (needed)
- `Button.Secondary` (needed)
- `Button.Label` (needed)
- `Icon.Button` (needed)
- `Icon.Form` (needed)

**Status Components**:

- `LoadingOverlay` (already used)
- `Loading` (already used)

### Missing StyleSystem Components

**DataGrid Components** (NEW - needed for standard DataGrid):

- `DataGrid.Standard` - Base DataGrid styling
- `DataGrid.Header` - Column header styling
- `DataGrid.Row` - Row styling
- `DataGrid.Cell` - Cell styling
- `DataGrid.Selection` - Selection styling

**Column Customization Components** (NEW):

- `ColumnCustomization.Dropdown` - Dropdown menu styling
- `ColumnCustomization.Item` - Dropdown item styling
- `ColumnCustomization.Panel` - Panel container styling

### Theme V2 Token Requirements

**Colors**:

- `{DynamicResource MTM_Shared_Logic.PrimaryBrush}`
- `{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}`
- `{DynamicResource MTM_Shared_Logic.BorderLightBrush}`
- `{DynamicResource MTM_Shared_Logic.TextBrush}`

**Spacing**:

- `{DynamicResource MTM_Shared_Logic.StandardMargin}`
- `{DynamicResource MTM_Shared_Logic.CardPadding}`
- `{DynamicResource MTM_Shared_Logic.FormSpacing}`

### Missing Theme V2 Tokens

**DataGrid Tokens** (NEW):

- `MTM_Shared_Logic.DataGridBackgroundBrush`
- `MTM_Shared_Logic.DataGridHeaderBrush`
- `MTM_Shared_Logic.DataGridSelectionBrush`
- `MTM_Shared_Logic.DataGridBorderBrush`

**Column Customization Tokens** (NEW):

- `MTM_Shared_Logic.DropdownBackgroundBrush`
- `MTM_Shared_Logic.DropdownBorderBrush`
- `MTM_Shared_Logic.DropdownHoverBrush`

## Technical Analysis

### ScrollViewer Compliance

**Current Implementation**: No ScrollViewer wrapper around MainContainer - CustomDataGrid handles its own scrolling
**Compliance Status**: ✅ COMPLIANT - No prohibited ScrollViewer usage
**Action Required**: Maintain this pattern with standard DataGrid

### AVLN2000 Risk Assessment

**Current Risks**:

- Grid definitions using proper `x:Name` syntax ✅
- Avalonia namespace correctly used ✅
- Control equivalents properly used ✅

**New Risks from Transformation**:

- Standard DataGrid column definitions must use proper AXAML syntax
- ComboBox for column customization must follow Avalonia patterns
- Property binding to MySQL columns must be validated

### Grid Structure Analysis

**Current Structure**:

```xml
Grid RootContainer
├── Grid MainContainer (RowDefinitions="*,Auto")
    ├── Border (Card) - Row 0
    │   └── Grid DataContainer (RowDefinitions="Auto,Auto,*")
    │       ├── CollapsiblePanel (SearchPanel) - Row 1
    │       └── Grid (Row 2, RowDefinitions="*,Auto")
    │           └── CustomDataGrid - Row 0
    └── Border (Card.Actions) - Row 1
        └── Grid ActionButtonsGrid (ColumnDefinitions="Auto,*,Auto")
```

**Needed Changes**:

- Replace CustomDataGrid with standard DataGrid
- Add column customization panel
- Maintain overlay structure

### Data Binding Analysis

**Critical Bindings to Preserve**:

- `ItemsSource="{Binding InventoryItems}"`
- `SelectedItem="{Binding SelectedItem, Mode=TwoWay}"`
- `SelectedItemsCollection="{Binding SelectedItems}"`
- All command bindings: SearchCommand, DeleteCommand, etc.
- Visibility bindings: IsLoading, HasInventoryItems

## Implementation Planning

### Transformation Strategy

1. **Pre-Implementation**: Create missing StyleSystem components and Theme V2 tokens
2. **DataGrid Replacement**: Replace CustomDataGrid with standard DataGrid
3. **Column Customization**: Add ComboBox dropdown for column selection
4. **StyleSystem Migration**: Replace all hardcoded styling
5. **Integration Testing**: Verify EditInventoryView and overlays still work

### Phase Dependencies

**Must Create First**:

- DataGrid StyleSystem components
- Column customization StyleSystem components
- Required Theme V2 tokens

**Then Transform**:

- Replace CustomDataGrid markup
- Add column customization UI
- Apply StyleSystem classes throughout

### Validation Approach

1. Build validation for syntax errors
2. Theme testing (light/dark)
3. Functional testing of all buttons and commands
4. DataGrid functionality verification
5. Column customization testing
6. EditInventoryView integration testing

## Risk Assessment

### High-Risk Areas

1. **CustomDataGrid Replacement**: Standard DataGrid may not have all CustomDataGrid features
2. **Column Customization**: New functionality must integrate seamlessly
3. **Property-Column Mapping**: MySQL column names must match exactly (PartID, OperationNumber, Location, Quantity, Notes)
4. **EditInventoryView Integration**: Overlay system must remain functional

### Mitigation Strategies

1. **DataGrid Features**: Research standard DataGrid capabilities, implement missing features via styling/templates
2. **Column Mapping**: Validate against actual database schema, use documented property mappings
3. **Integration Testing**: Test all overlay interactions thoroughly
4. **Incremental Changes**: Transform section by section with continuous testing

### Rollback Plan

- `RemoveTabView.axaml.backup` will contain original file
- Can restore if transformation fails
- All new StyleSystem components are additive (won't break existing files)

## Research Completion

### Research Status

- [x] File analysis complete
- [x] Business requirements documented
- [x] StyleSystem requirements identified
- [x] Theme V2 requirements identified
- [x] Risk assessment complete
- [x] Implementation strategy defined

### Next Phase: Planning

**Ready for Planning Phase**: Yes
**Blockers**: None identified - all dependencies can be created as part of implementation

---

**Research Completed**: 20250927
**Researcher**: GitHub Copilot
**Review Status**: Ready for Planning Phase
