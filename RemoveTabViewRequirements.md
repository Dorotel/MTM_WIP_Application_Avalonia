# RemoveTabView.axaml Refactor Requirements

Transform `RemoveTabView.axaml` to implement customizable DataGrid columns and EditInventoryView integration, ensuring all AXAML syntax strictly follows MTM AXAML patterns to prevent AVLN2000 errors.

**Requirements:**

A. **DataGrid Column Customization**:

- Implement customizable columns via a dropdown menu using only the standard Avalonia ComboBox control, styled exclusively with MTM Theme V2 as defined in avalonia-ui-guidelines.instructions.md and mtm-theme-style-refactor-prompt-template.md for full style compliance; do not introduce any custom or third-party dropdowns under any circumstances.
- When returning columns from the database, return only columns defined in the actual MySQL database schema, and strictly enforce property-to-column mapping as documented in data-models.instructions.md to prevent runtime mapping errors.
- Set default columns for remove operations, explicitly specifying property-to-column mapping as per data-models.instructions.md:
  - PartID (property) → "PartID" (column)
  - Operation (property) → "Operation" (column)
  - Location (property) → "Location" (column)
  - AvailableQuantity (property) → "AvailableQuantity" (column)
  - RemoveQuantity (property) → "RemoveQuantity" (column)
  - Notes (property) → "Notes" (column)

B. **Panel Switching System**:

- Integrate EditInventoryView.axaml into RemoveTabView.axaml by directly referencing the EditInventoryView UserControl in AXAML, ensuring you import the correct namespace (e.g., `xmlns:views="using:MTM_WIP_Application_Avalonia.Views"`) and use `<views:EditInventoryView />` for inclusion; DataContext must be inherited from the parent or set via binding, and any code-behind instantiation or direct ViewModel assignment is strictly prohibited, in full compliance with avalonia-ui-guidelines.instructions.md and mvvm-community-toolkit.instructions.md to avoid AVLN2000 errors.
- Maintain smooth transitions between views using only standard Avalonia visual state or visibility binding patterns (no custom animation frameworks or code-behind logic).

C. **StyleSystem Migration**:

- Replace all legacy theme references (`MTM_Shared_Logic.*`) with Theme V2 semantic tokens as defined in theme-v2-implementation.instructions.md:
  - `MTM_Shared_Logic.DataGridBackgroundBrush` → `ThemeV2.Background.Surface`
  - `MTM_Shared_Logic.BorderLightBrush` → `ThemeV2.Border.Default`
  - `MTM_Shared_Logic.CardBackgroundBrush` → `ThemeV2.Background.Card`
- All button styling must use only DynamicResource bindings for all colors, backgrounds, borders, and font properties, referencing StyleSystem and ThemeService resources exclusively (as defined in MTM Theme V2), with absolutely no inline or hardcoded values permitted anywhere; this is strictly enforced per the patterns in mtm-theme-style-refactor-prompt-template.md.

D. **StyleSystem Implementation**:

- Follow mtm-theme-style-refactor-prompt-template.md patterns
- Use Theme V2 + StyleSystem exclusively  
- Remove all hardcoded styling: This requirement applies to both AXAML and code-behind. All colors, margins, and font sizes must be set exclusively via DynamicResource or StyleSystem references, strictly following the enforcement patterns in mtm-theme-style-refactor-prompt-template.md. No hardcoded values are permitted anywhere in AXAML or code-behind; for developers unfamiliar with the template, refer to mtm-theme-style-refactor-prompt-template.md for detailed enforcement patterns and examples.

**Additional Requirements**: Implement column customization dropdown for remove-specific columns, integrate EditInventoryView seamlessly, complete Theme V2 migration from legacy MTM_Shared_Logic references
