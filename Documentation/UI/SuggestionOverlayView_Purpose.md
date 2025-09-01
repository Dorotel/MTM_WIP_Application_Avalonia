# SuggestionOverlayView.axaml Purpose

## Overview
`SuggestionOverlayView.axaml` is an Avalonia UserControl designed to present a modal overlay or dialog to the user when an exact match for a part or item is not found during inventory operations. It displays a list of suggested similar items and allows the user to select one or cancel the operation.

## Key Features
- **Displays Suggestions:** Shows a list of similar items when no exact match is found.
- **User Selection:** Allows the user to select a suggestion or cancel the overlay.
- **Commands:** Binds to `SelectCommand` and `CancelCommand` in its ViewModel for user actions.
- **Modern UI:** Uses MTM theming and modern card-based design for a consistent look.

## Intended Usage
- The overlay is meant to be shown as a modal or floating entity, not as a child of any specific tab view.
- It should be managed at the same level as other main views (e.g., `InventoryTabView`, `RemoveTabView`, `TransferTabView`) within the main application shell (`MainView.axaml`).
- The overlay is typically triggered by business logic when a user search yields no exact match, prompting the user to select from alternatives.

## Why Remove It Now?
- The current architecture requires `SuggestionOverlayView` to be managed as a sibling to other main views, not as a nested child.
- This change will allow for more flexible overlay management and a cleaner separation of concerns in the UI.

---
**Next Steps:**
- Remove the file and all references to it.
- Refactor the application to treat suggestion overlays as top-level entities, not children of tab views.
