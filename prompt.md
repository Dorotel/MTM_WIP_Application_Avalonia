@copilot Please continue work on the TransferTabView feature using the latest gap report:

- Implement TransferTabView.axaml with dual-panel layout and DataGrid-centric display.
- Integrate CollapsiblePanel for input area with auto-collapse/expand behavior.
- Add NumericUpDown control for quantity transfer with max/min limits and keyboard support.
- Replace all ComboBoxes with TextBoxes + SuggestionOverlay for Part ID, Operation, and To Location fields.
- Apply InventoryTabView styling and grid pattern for consistency.
- Use SuggestionOverlay and TextBoxFuzzyValidationBehavior for all input fields; add watermark text and real-time validation feedback.
- Enable single/multi-row selection, sortable columns, and row highlighting in DataGrid.
- Add "Nothing Found" indicator and loading state.
- Implement batch transfer, atomic transaction handling, and audit trail logic.
- Integrate QuickButtons, SuccessOverlay, MainView status bar, CollapsiblePanel, ErrorHandling, and MTM Theme system.
- Enforce validation, error handling, accessibility, and keyboard shortcuts (F5, Enter, Escape, Tab, Arrow keys).
- Add integration tests and optimize for large datasets and real-time validation.

Refer to `.github/issues/transfertabview-implementation-gap-report.yml` for the full list of requirements and blockers.
