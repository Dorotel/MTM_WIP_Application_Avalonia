# TransferTabView.axaml Refactor Requirements

Transform `TransferTabView.axaml` to replace TransferCustomDataGrid with the Avalonia standard DataGrid (no custom or third-party variants permitted), ensuring all AXAML syntax strictly follows MTM AXAML patterns to prevent AVLN2000 errors, and including customizable columns and EditInventoryView integration.

**Requirements:**

A. **DataGrid Column Customization**:

- Implement customizable columns via a dropdown menu using only the standard Avalonia ComboBox control, styled exclusively with MTM Theme V2 as defined in avalonia-ui-guidelines.instructions.md and mtm-theme-style-refactor-prompt-template.md for full style compliance; do not introduce any custom or third-party dropdowns under any circumstances.
- Replace TransferCustomDataGrid with the standard Avalonia DataGrid, ensuring the implementation strictly follows MTM AXAML patterns as defined in avalonia-ui-guidelines.instructions.md and does not introduce any non-standard controls or properties.
- When returning columns from the database, return only columns defined in the actual MySQL database schema, and strictly enforce property-to-column mapping as documented in data-models.instructions.md to prevent runtime mapping errors.
- Implement customizable columns via a dropdown menu using standard Avalonia controls styled with MTM Theme V2 (no custom or third-party dropdowns)
- Return only columns defined in the actual MySQL database schema when searching, and ensure all column/property mapping strictly follows the documented property-to-column mapping as specified in data-models.instructions.md.
- Set default columns for transfer operations, explicitly specifying property-to-column mapping as per data-models.instructions.md:
  - PartID (property) → "PartID" (column)
  - Operation (property) → "Operation" (column)
  - FromLocation (property) → "FromLocation" (column)
- Integrate EditInventoryView.axaml into TransferTabView.axaml by directly referencing the EditInventoryView UserControl in AXAML, ensuring you import the correct namespace (e.g., `xmlns:views="using:MTM_WIP_Application_Avalonia.Views"`) and use `<views:EditInventoryView />` for inclusion; DataContext must be inherited from the parent or set via binding, and any code-behind instantiation or direct ViewModel assignment is strictly prohibited, in full compliance with avalonia-ui-guidelines.instructions.md and mvvm-community-toolkit.instructions.md to avoid AVLN2000 errors.
  - AvailableQuantity (property) → "AvailableQuantity" (column)
  - TransferQuantity (property) → "TransferQuantity" (column)
  - Notes (property) → "Notes" (column)

B. **Panel Switching System**:

- Integrate EditInventoryView.axaml into TransferTabView.axaml by directly referencing the EditInventoryView UserControl in AXAML, ensuring DataContext is inherited from the parent or set via binding, and strictly prohibiting any code-behind instantiation or direct ViewModel assignment, in full compliance with avalonia-ui-guidelines.instructions.md and mvvm-community-toolkit.instructions.md.
- Maintain smooth transitions between views using only standard Avalonia visual state or visibility binding patterns (no custom animation frameworks or code-behind logic).
- Maintain smooth transitions between views

C. **Button Integration**:

- Place all action buttons in the same panel.
- All button styling must use only DynamicResource bindings for all colors, backgrounds, borders, and font properties, referencing StyleSystem and ThemeService resources exclusively (as defined in MTM Theme V2), with absolutely no inline or hardcoded values permitted anywhere; this is strictly enforced per the patterns in mtm-theme-style-refactor-prompt-template.md.

D. **StyleSystem Implementation**:

- Follow mtm-theme-style-refactor-prompt-template.md patterns
- Use Theme V2 + StyleSystem exclusively
- Remove all hardcoded styling: This requirement applies to both AXAML and code-behind. All colors, margins, and font sizes must be set exclusively via DynamicResource or StyleSystem references, strictly following the enforcement patterns in mtm-theme-style-refactor-prompt-template.md. No hardcoded values are permitted anywhere in AXAML or code-behind; for developers unfamiliar with the template, refer to mtm-theme-style-refactor-prompt-template.md for detailed enforcement patterns and examples.

**Additional Requirements**: Remove TransferCustomDataGrid dependency completely, integrate EditInventoryView seamlessly, implement column customization dropdown for transfer-specific columns
