@copilot Please continue work on the RemoveTabView feature using the latest gap report:

PR #60 introduces DataGrid implementation, CollapsiblePanel integration, InventoryTabView styling, and replaces ComboBoxes with TextBoxes + SuggestionOverlay. Search/Reset button functionality, QuickButtons, SuccessOverlay, and progress reporting are present but require further validation and refinement.

Immediate next steps:
- Refine DataGrid-centric layout and overlays for full InventoryTabView pattern consistency.
- Validate and complete CollapsiblePanel auto-collapse/expand behavior for input area.
- Update CollapsiblePanel.axaml so the header is positioned on top, not on the left side, for RemoveTabView and all InventoryTabView-based layouts.
- Confirm SuggestionOverlay and TextBoxFuzzyValidationBehavior for all input fields (Part ID, Operation, Location, User).
- Refine batch delete button, confirmation dialog, atomic transaction handling, and operation results reporting.
- Validate multi-row selection, sortable columns, row highlighting, "Nothing Found" indicator, and loading state in DataGrid.
- Complete QuickButtons, SuccessOverlay, MainView status bar, CollapsiblePanel, ErrorHandling, and MTM Theme system integration.
- Refine validation, error handling, accessibility, and keyboard shortcuts (F5, Delete, Ctrl+Z, Escape, Enter, Tab).
- Add/complete integration tests and optimize for large datasets and batch operations.

Refer to `.github/issues/removetabview-implementation-gap-report.yml` for the full list of requirements and blockers.
