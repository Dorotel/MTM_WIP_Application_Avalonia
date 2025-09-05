# GitHub Issue: Implement Batch Entry Functionality

**Title:** [FEATURE] Implement batch inventory entry for multiple items  
**Assignees:** @copilot  
**Labels:** feature, enhancement  
**Projects:** MTM-Development  
**Priority:** Medium - Could Have  
**Category:** Views & UI (AXAML)  
**Parent Epic:** MTM Inventory Management Epic - Inventory Transaction Management  

---

## Feature Title
Implement batch inventory entry for multiple items

## Parent Epic
MTM Inventory Management Epic - Inventory Transaction Management

## Feature Priority
Medium - Could Have

## Feature Category
Views & UI (AXAML)

## User Story
**As a** production operator  
**I want** to enter multiple inventory items at once  
**So that** I can efficiently process large batches of parts without repetitive form entry

**Example:**
As a receiving clerk processing a shipment of 50 different parts, I want to enter all parts in a single batch operation so that I can complete the receiving process in minutes instead of hours of individual entries.

## Functional Requirements

**Primary Functions:**
1. The system SHALL provide a "Batch Entry" mode accessible from Advanced button
2. The system SHALL allow entry of multiple rows with Part ID, Operation, Location, Quantity, User
3. The system SHALL validate each row before allowing batch save
4. The system SHALL provide progress indication during batch processing
5. The system SHALL rollback entire batch if any individual item fails

**Secondary Functions:**
- Import from clipboard (Excel copy/paste support)
- Duplicate row functionality for similar entries
- Bulk edit capabilities (apply same operation/location to multiple parts)
- Row-level validation with visual indicators

## Technical Implementation Approach

**Components Involved:**
- Views: New BatchEntryView.axaml and BatchEntryViewModel.cs
- Navigation: Modal dialog or dedicated view accessible from InventoryTabView
- Services: Enhanced database transaction handling for batch operations
- Models: BatchEntryRow model for individual line items

**Key Patterns:**
- DataGrid with inline editing and validation
- MVVM Community Toolkit with ObservableCollection<BatchEntryRowViewModel>
- TextBox + SuggestionOverlay pattern for each editable cell
- Transaction-wrapped multiple calls to `inv_inventory_Add_Item`

**Data Flow:**
Advanced Button → BatchEntryView → DataGrid Editing → Validation → Batch Save → Progress Feedback → Session History Update

## UI/UX Requirements

**Visual Design:**
- Modal dialog or full-screen overlay following MTM Amber theme
- DataGrid with editable cells using TextBox + SuggestionOverlay
- Progress bar showing batch processing status (X of Y items processed)
- Action buttons: Save Batch, Cancel, Import from Clipboard

**Interaction Patterns:**
- Tab navigation between cells in DataGrid
- Enter key to add new row
- Delete key to remove selected rows
- Context menu for row operations (duplicate, delete)
- Validation errors highlighted per-cell with tooltips

**Responsive Behavior:**
- Scrollable DataGrid for large batch sizes
- Resizable columns for different screen sizes
- Keyboard shortcuts for efficient data entry

## Acceptance Criteria

**User Interface:**
- [ ] Batch entry interface accessible from Advanced button in InventoryTabView
- [ ] DataGrid allows inline editing of Part ID, Operation, Location, Quantity, User
- [ ] Each cell supports TextBox + SuggestionOverlay pattern for data validation
- [ ] Add/Remove row functionality with keyboard and button controls
- [ ] Visual validation indicators for each cell

**Functionality:**
- [ ] All fields validate against master data using existing patterns
- [ ] Batch save operation processes all valid rows in database transaction
- [ ] Progress bar shows current processing status during batch save
- [ ] Transaction rollback occurs if any individual item fails
- [ ] Batch results added to session history with batch identifier

**Technical:**
- [ ] Uses DataGrid with custom cell templates for TextBox + SuggestionOverlay
- [ ] Transaction-wrapped database operations ensure atomicity
- [ ] Memory efficient for handling 100+ row batches
- [ ] Proper disposal of resources and view cleanup

## Components Affected

**New Components:**
- Views/MainForm/BatchEntryView.axaml (new modal/dialog)
- ViewModels/MainForm/BatchEntryViewModel.cs (new ViewModel)
- Models/BatchEntryRow.cs (data model for grid rows)
- Services: Enhanced transaction handling in DatabaseService

**Modified Components:**
- Views/MainForm/Panels/InventoryTabView.axaml (Advanced button navigation)
- ViewModels/MainForm/InventoryTabViewModel.cs (navigation to batch entry)
- Services/DatabaseService.cs (batch transaction support)

**Dependencies:**
- DataGrid control with custom cell templates
- Existing TextBox + SuggestionOverlay infrastructure
- Transaction management for database operations
- MVVM Community Toolkit for collection binding

## Testing Strategy

**Unit Tests:**
- BatchEntryViewModel validation logic
- Batch transaction rollback scenarios
- Row-level validation and error handling
- DataGrid binding and property change notifications

**Integration Tests:**
- End-to-end batch entry workflow
- Database transaction rollback testing
- Large batch performance testing (100+ rows)
- Memory usage and disposal verification

**Manual Tests:**
- User experience with batch data entry
- Validation feedback and error recovery
- Progress indication during long batch operations
- Integration with existing session history

## Estimated Effort
M (1-2 weeks)

## Dependencies & Blockers

**Prerequisites:**
- Advanced button functionality in InventoryTabView
- Understanding of DataGrid custom cell templates
- Transaction management patterns established

**Blockers:**
- DataGrid cell template complexity for TextBox + SuggestionOverlay integration
- Performance optimization requirements for large batches

## Feature Readiness Checklist

- [x] Requirements are clear and testable
- [x] Technical approach is feasible (with complexity noted)
- [x] UI/UX design follows MTM patterns
- [x] Dependencies are identified
- [x] Testing strategy is defined

## Additional Notes

This feature addresses the PRD Phase 3 requirement for "Batch entry functionality for multiple items." The implementation should leverage the existing TextBox + SuggestionOverlay pattern within DataGrid cells to maintain consistency with the single-item entry experience.

Key technical challenge will be integrating the SuggestionOverlay service with DataGrid cell templates while maintaining performance for large datasets.

**Performance Considerations:**
- Virtualization for large DataGrids (100+ rows)
- Lazy loading of master data for suggestions
- Background validation to avoid UI blocking
- Memory management for large batch operations

**Related Documentation:**
- PRD: `docs/ways-of-work/plan/mtm-inventory-management/inventory-transaction-management/prd.md` (Phase 3)
- AdvancedInventoryView patterns for bulk operations reference
- Existing DataGrid implementations in the application
