@copilot Please continue work on the RemoveTabView feature using the latest gap report:

- Implement DataGrid-centric layout with search/filter inputs above DataGrid in RemoveTabView.axaml.
- Integrate CollapsiblePanel with auto-collapse/expand behavior for input area.
- Replace all ComboBoxes with TextBoxes + SuggestionOverlay.
- Apply InventoryTabView styling and grid pattern for consistency.
- Use SuggestionOverlay for Part ID, Operation, Location, and User fields.
- Apply TextBoxFuzzyValidationBehavior and watermark text; add real-time validation feedback.
- Enable multi-row selection, sortable columns, and row highlighting in DataGrid.
- Add "Nothing Found" indicator and loading state.
- Implement batch delete button, confirmation dialog, and progress indication.
- Integrate QuickButtons, SuccessOverlay, MainView status bar, CollapsiblePanel, ErrorHandling, and MTM Theme system.
- Enforce validation, error handling, accessibility, and keyboard shortcuts (F5, Delete, Ctrl+Z, Escape, Enter, Tab).
- Add integration tests and optimize for large datasets and batch operations.

Refer to `.github/issues/removetabview-implementation-gap-report.yml` for the full list of requirements and blockers.
