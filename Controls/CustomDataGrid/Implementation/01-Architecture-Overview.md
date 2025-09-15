# CustomDataGrid - Architecture Overview

**Version**: 1.0  
**Created**: September 14, 2025  
**Framework**: .NET 8 Avalonia UI 11.3.4  

---

## 🏛️ Architecture Overview

The CustomDataGrid control is designed as a high-performance, professional data grid that matches DataGridView standards with perfect header-data alignment. It follows MTM architectural principles and integrates seamlessly with the MVVM Community Toolkit pattern.

### **Control Type**: Avalonia UserControl with code-behind
### **Data Pattern**: ListBox with ItemTemplate for virtual scrolling performance
### **Styling**: Style Selectors with MTM design system DynamicResource bindings
### **Layout**: Grid-based with proportional column definitions
### **Selection**: Multi-selection support with Select All checkbox
### **Actions**: Command binding for Read Note, Delete, Edit, Duplicate, View Details

## Key Components Structure
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

## Critical Design Principles

### 1. Perfect Header-Data Alignment
- **CRITICAL**: Headers and data rows must align exactly (no column misalignment)
- All cells must use identical proportional sizing between header and data grids
- Consistent height enforcement across all cell types

### 2. MTM Design System Integration
- All colors must use DynamicResource bindings for theme compatibility
- Professional appearance matching DataGridView standards
- Conditional styling based on data state (notes, selection, etc.)

### 3. Virtual Scrolling Performance
- ListBox with ItemTemplate for handling large datasets
- Memory-efficient rendering for manufacturing inventory data
- Responsive UI with minimal memory footprint

### 4. MVVM Community Toolkit Integration
- Command binding for all user actions
- ObservableCollection data binding
- Event-driven communication with parent ViewModels

## Supporting Classes

| Class | Purpose | Pattern |
|-------|---------|---------|
| `CustomDataGridColumn.cs` | Column definition and metadata | Configuration class |
| `ColumnConfiguration.cs` | Column management state | Settings persistence |
| `FilterConfiguration.cs` | Filter state management | Settings persistence |
| `SelectableItem.cs` | Selection wrapper for data items | MVVM wrapper pattern |
| `ColumnManagementPanel.axaml` | Column visibility/order management | UserControl (Phase 3) |
| `FilterPanel.axaml` | Advanced filtering UI | UserControl (Phase 5) |

## Integration with MTM Architecture

### Service Dependencies
- **Data Binding**: Parent ViewModel provides data through `ItemsSource`
- **Commands**: Parent ViewModel handles all business logic commands
- **Themes**: Automatic integration with MTM theme system
- **Overlay Integration**: Works with `ConfirmationOverlayView` and `SuccessOverlayView`

### Event Communication
- Selection changes communicated to parent ViewModel
- Action button clicks routed through command bindings
- Data context preserved for proper MVVM data flow

---

**Next Implementation Phase**: [02-Column-Layout-Specification.md](./02-Column-Layout-Specification.md)