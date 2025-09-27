# RemoveTabView.axaml Refactor Requirements

Transform `RemoveTabView.axaml` to replace CustomDataGrid with the Avalonia standard DataGrid (no custom or third-party variants permitted), ensuring all AXAML syntax strictly follows MTM AXAML patterns to prevent AVLN2000 errors, and including customizable columns and EditInventoryView integration.

**Requirements:**

A. **DataGrid Column Customization**:

- Implement customizable columns via a dropdown menu using only the standard Avalonia ComboBox control, styled exclusively with MTM Theme V2 as defined in avalonia-ui-guidelines.instructions.md; do not introduce any custom or third-party dropdowns under any circumstances.
- Replace CustomDataGrid with the standard Avalonia DataGrid, ensuring the implementation strictly follows MTM AXAML patterns as defined in avalonia-ui-guidelines.instructions.md and does not introduce any non-standard controls or properties.
- When returning columns from the database, return only columns defined in the actual MySQL database schema, and strictly enforce property-to-column mapping as documented in data-models.instructions.md to prevent runtime mapping errors.
- Implement customizable columns via a dropdown menu using standard Avalonia controls styled with MTM Theme V2 (no custom or third-party dropdowns)
- Return only columns defined in the actual MySQL database schema when searching, and ensure all column/property mapping strictly follows the documented property-to-column mapping as specified in data-models.instructions.md.
- Set default columns: PartID, OperationNumber, Location, Quantity, Notes

B. **Panel Switching System**:

- Integrate EditInventoryView.axaml into RemoveTabView.axaml by directly referencing the EditInventoryView UserControl in AXAML, ensuring DataContext is inherited from the parent or set via binding, and strictly prohibiting any code-behind instantiation or direct ViewModel assignment, in full compliance with avalonia-ui-guidelines.instructions.md and mvvm-community-toolkit.instructions.md.
- Maintain smooth transitions between views using only standard Avalonia visual state or visibility binding patterns (no custom animation frameworks or code-behind logic).
- Maintain smooth transitions between views

C. **Button Integration**:

- Place all action buttons in the same panel.
- All button styling must use only DynamicResource bindings for colors and styles as defined by the StyleSystem and ThemeService (MTM Theme V2), with no hardcoded values anywhere, strictly following the patterns in mtm-theme-style-refactor-prompt-template.md.

D. **StyleSystem Implementation**:

- Follow mtm-theme-style-refactor-prompt-template.md patterns
- Use Theme V2 + StyleSystem exclusively
- Remove all hardcoded styling: All colors, margins, and font sizes must be set exclusively via DynamicResource or StyleSystem references, strictly following the patterns in mtm-theme-style-refactor-prompt-template.md. No hardcoded values are permitted anywhere in AXAML or code-behind; all styling must use only DynamicResource or StyleSystem references as defined in the template.

**Additional Requirements**: Remove CustomDataGrid dependency completely, integrate EditInventoryView seamlessly, implement column customization dropdown
