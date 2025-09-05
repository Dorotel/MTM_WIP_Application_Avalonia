# GitHub Issue: Export Session History to Excel/CSV

**Title:** [FEATURE] Add export functionality for session transaction history  
**Assignees:** @copilot  
**Labels:** feature, enhancement  
**Projects:** MTM-Development  
**Priority:** Low - Won't Have This Time  
**Category:** Views & UI (AXAML)  
**Parent Epic:** MTM Inventory Management Epic - Inventory Transaction Management  

---

## Feature Title
Add export functionality for session transaction history

## Parent Epic
MTM Inventory Management Epic - Inventory Transaction Management

## Feature Priority
Low - Won't Have This Time

## Feature Category
Views & UI (AXAML)

## User Story
**As a** production supervisor  
**I want** to export session transaction history to Excel or CSV  
**So that** I can analyze productivity and maintain records outside the application

**Example:**
As a shift supervisor reviewing daily productivity, I want to export the session transaction history for all operators to Excel so that I can create reports for management and track performance trends over time.

## Functional Requirements

**Primary Functions:**
1. The system SHALL provide export button in transaction history panel
2. The system SHALL export to both Excel (.xlsx) and CSV formats
3. The system SHALL include all transaction details (time, part, operation, location, quantity, user, status)
4. The system SHALL auto-generate filename with timestamp
5. The system SHALL handle large transaction histories efficiently

**Secondary Functions:**
- Export filtering options (date range, status, user)
- Custom column selection for exports
- Progress indication for large exports
- Email integration for sharing exported files

## Technical Implementation Approach

**Components Involved:**
- Services: New ExportService.cs for file generation
- ViewModels: Enhanced InventoryTabViewModel with export commands
- Views: Export button integration in CollapsiblePanel header
- Libraries: EPPlus for Excel generation, built-in CSV writer

**Key Patterns:**
- Export service with format abstraction (IExportFormat interface)
- Background export operations with progress reporting
- File save dialog integration following Windows conventions
- Memory-efficient streaming for large datasets

**Data Flow:**
Export Button → Format Selection → File Dialog → Background Export → Progress Updates → Completion Notification → File Open Option

## UI/UX Requirements

**Visual Design:**
- Export dropdown button in transaction history panel header (next to transaction count)
- Format selection menu: "Export to Excel (.xlsx)" and "Export to CSV (.csv)"
- Progress dialog during export with progress bar and cancel option
- Success notification with option to open exported file

**Interaction Patterns:**
- Export button integrates with existing CollapsiblePanel header
- File save dialog with suggested filename (e.g., "Session_History_2025-09-04_14-30.xlsx")
- Progress dialog shows export status and allows cancellation
- Toast notification or dialog for export completion

**Responsive Behavior:**
- Export functionality works regardless of panel expansion state
- Progress dialog centers on main window
- File save dialog respects system theme and preferences

## Acceptance Criteria

**User Interface:**
- [ ] Export dropdown button added to transaction history panel header
- [ ] Format selection menu with Excel and CSV options
- [ ] File save dialog with auto-generated timestamp filename
- [ ] Progress dialog shows export status with cancel option
- [ ] Success notification with file open option

**Functionality:**
- [ ] Excel export includes all transaction columns with proper formatting
- [ ] CSV export uses proper comma separation and quoted fields
- [ ] Exported files include session date/time and user information
- [ ] Large session histories (100+ transactions) export efficiently
- [ ] Export can be cancelled during processing

**Technical:**
- [ ] Uses EPPlus library for Excel generation with proper formatting
- [ ] CSV export handles special characters and commas in data
- [ ] Memory-efficient export for large datasets (streaming)
- [ ] Proper error handling for file permission and disk space issues
- [ ] Export service is unit testable and mockable

## Components Affected

**New Components:**
- Services/ExportService.cs (file generation service)
- Models/ExportFormat.cs (export format definitions)
- Views/Dialogs/ExportProgressDialog.axaml (progress dialog)

**Modified Components:**
- Views/MainForm/Panels/InventoryTabView.axaml (export button in history panel)
- ViewModels/MainForm/InventoryTabViewModel.cs (export commands)
- Controls/CollapsiblePanel.axaml (header button integration)

**Dependencies:**
- EPPlus library for Excel generation
- System.IO for CSV generation and file operations
- File save dialog functionality
- Background task management for export operations

## Testing Strategy

**Unit Tests:**
- ExportService Excel and CSV generation
- Export data formatting and column mapping
- File naming and timestamp generation
- Error handling for file operations

**Integration Tests:**
- End-to-end export workflow from UI to file
- Large dataset export performance testing
- File save dialog integration testing
- Export cancellation and cleanup testing

**Manual Tests:**
- User experience validation for export workflow
- Exported file verification in Excel and text editors
- Export button placement and accessibility
- Progress dialog behavior and cancellation

## Estimated Effort
M (1-2 weeks)

## Dependencies & Blockers

**Prerequisites:**
- EPPlus library evaluation and approval
- File save dialog implementation patterns
- Background task management infrastructure

**Blockers:**
- EPPlus licensing considerations (GPL vs commercial)
- File system permissions in deployment environments
- Large dataset memory usage optimization

## Feature Readiness Checklist

- [x] Requirements are clear and testable
- [x] Technical approach is feasible
- [x] UI/UX design follows MTM patterns
- [x] Dependencies are identified
- [x] Testing strategy is defined

## Additional Notes

This feature addresses the PRD Phase 3 requirement for "Export transaction history to Excel/CSV." While marked as low priority, it provides significant value for supervisory and management roles who need to analyze productivity data outside the application.

**Export Format Specifications:**

**Excel Export (.xlsx):**
- Header row with bold formatting
- Date/time columns with proper formatting
- Auto-fit column widths
- Freeze panes on header row
- Cell borders and alternating row colors

**CSV Export (.csv):**
- Standard comma-separated format
- Quoted fields containing commas or quotes
- UTF-8 encoding for international characters
- Standard line endings (CRLF for Windows)

**Filename Convention:**
- `Session_History_YYYY-MM-DD_HH-mm.{extension}`
- Example: `Session_History_2025-09-04_14-30.xlsx`

**Performance Considerations:**
- Stream-based export for large datasets (>1000 rows)
- Progress reporting every 100 rows processed
- Memory usage optimization for Excel generation
- Background thread execution to avoid UI blocking

**Related Documentation:**
- PRD: `docs/ways-of-work/plan/mtm-inventory-management/inventory-transaction-management/prd.md` (Phase 3)
- AdvancedInventoryView Excel integration patterns for reference
- CollapsiblePanel enhancement patterns
