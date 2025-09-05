# GitHub Issue: Add User Field with TextBox + SuggestionOverlay Integration

**Title:** [FEATURE] Add User field with TextBox + SuggestionOverlay integration  
**Assignees:** @copilot  
**Labels:** feature, enhancement  
**Projects:** MTM-Development  
**Priority:** High - Should Have  
**Category:** Views & UI (AXAML)  
**Parent Epic:** MTM Inventory Management Epic - Inventory Transaction Management  

---

## Feature Title
Add User field with TextBox + SuggestionOverlay integration to Inventory Transaction Management

## Parent Epic
MTM Inventory Management Epic - Inventory Transaction Management

## Feature Priority
High - Should Have

## Feature Category
Views & UI (AXAML)

## User Story
**As a** manufacturing operator  
**I want** to specify or select the user performing the inventory transaction  
**So that** accurate audit trails and accountability are maintained in the system

**Example:**
As a production supervisor covering multiple stations, I want to specify which operator actually performed the inventory transaction so that the audit trail reflects the correct person responsible for the work.

## Functional Requirements

**Primary Functions:**
1. The system SHALL add a User field to the inventory entry form between Location and Quantity fields
2. The system SHALL populate user suggestions from the database via `md_users_Get_All` stored procedure
3. The system SHALL provide SuggestionOverlay functionality when user enters partial text
4. The system SHALL default to current Windows user but allow override
5. The system SHALL validate user exists in master data before allowing save

**Secondary Functions:**
- Auto-complete with fuzzy matching for partial user names
- Visual feedback during user validation
- Integration with existing error handling patterns
- Support for both username and display name formats

## Technical Implementation Approach

**Components Involved:**
- Views: InventoryTabView.axaml (add User field)
- ViewModels: InventoryTabViewModel.cs (add User properties and validation)
- Services: MasterDataService.cs (add Users collection and loading)
- Database: Add `md_users_Get_All` stored procedure call
- Models: Update validation patterns for User field

**Key Patterns:**
- TextBox with TextBoxFuzzyValidationBehavior for consistent UX
- SuggestionOverlay service integration following existing Part/Operation/Location pattern
- MVVM Community Toolkit with [ObservableProperty] for User fields
- Centralized validation using existing IsValid pattern

**Data Flow:**
User Input → TextBox → ViewModel Property → Validation → SuggestionOverlay (if needed) → Database Save

## UI/UX Requirements

**Visual Design:**
- Material.Icons User or Account icon (consistent with existing field icons)
- Position between Location and Quantity fields in form layout
- Same styling as other input fields (MTM Amber theme compliance)
- Error states with red border and validation message

**Interaction Patterns:**
- Default value: Current Windows user (`Environment.UserName`)
- SuggestionOverlay triggers on partial text entry (same behavior as Part/Operation/Location)
- Tab order integration (Location → User → Quantity)
- Keyboard navigation support

**Responsive Behavior:**
- Consistent field sizing with other input controls
- Proper spacing using 8px/16px/24px system
- Icon alignment and field label formatting

## Acceptance Criteria

**User Interface:**
- [ ] User TextBox added between Location and Quantity fields
- [ ] Material.Icons User icon displayed with proper styling
- [ ] Field label "User:" with consistent typography
- [ ] Error validation message displays when invalid user entered
- [ ] Default value populated from current Windows user

**Functionality:**
- [ ] SuggestionOverlay shows valid users from `md_users_Get_All`
- [ ] Fuzzy matching works for partial user name entry
- [ ] Form validation includes User field in CanSave logic
- [ ] Save operation passes User to `inv_inventory_Add_Item` procedure
- [ ] Session history includes User information in transaction records

**Technical:**
- [ ] Follows existing TextBox + SuggestionOverlay pattern
- [ ] Uses MVVM Community Toolkit [ObservableProperty] pattern
- [ ] Integrates with existing validation system
- [ ] Maintains consistency with current error handling
- [ ] Database schema supports user field in transactions

## Components Affected

**New Components:**
- Database: `md_users_Get_All` stored procedure
- MasterDataService: Users collection and loading methods
- InventoryTabViewModel: User-related properties and validation

**Modified Components:**
- Views/MainForm/Panels/InventoryTabView.axaml (add User field)
- ViewModels/MainForm/InventoryTabViewModel.cs (add User properties)
- Services/MasterDataService.cs (add Users support)
- Database: Update `inv_inventory_Add_Item` if needed for user parameter

**Dependencies:**
- TextBoxFuzzyValidationBehavior (existing)
- SuggestionOverlay service (existing)
- MasterDataService pattern (existing)
- MVVM Community Toolkit (existing)

## Testing Strategy

**Unit Tests:**
- ViewModel User property validation
- MasterDataService Users loading
- User field integration in CanSave logic
- SuggestionOverlay integration for Users

**Integration Tests:**
- Database `md_users_Get_All` procedure execution
- End-to-end user selection and save operation
- Session history User field population
- Error handling for invalid users

**Manual Tests:**
- User field UI layout and styling verification
- SuggestionOverlay behavior with user data
- Default user population from Windows authentication
- Form validation with User field scenarios

## Estimated Effort
S (3-5 days)

## Dependencies & Blockers

**Prerequisites:**
- Database `md_users_Get_All` stored procedure created
- User master data populated in database
- Existing TextBox + SuggestionOverlay pattern understood

**Blockers:**
- None identified - follows established patterns

## Feature Readiness Checklist

- [x] Requirements are clear and testable
- [x] Technical approach is feasible
- [x] UI/UX design follows MTM patterns
- [x] Dependencies are identified
- [x] Testing strategy is defined

## Additional Notes

This enhancement addresses a critical gap in the current implementation where the User field is hardcoded in the ViewModel but not exposed to operators. The PRD specification requires user accountability for manufacturing compliance (21 CFR Part 11, ISO 9001).

Implementation should follow the exact same pattern as the existing Part ID, Operation, and Location fields to maintain consistency and leverage the proven TextBox + SuggestionOverlay architecture.

**Related Documentation:**
- PRD: `docs/ways-of-work/plan/mtm-inventory-management/inventory-transaction-management/prd.md`
- Existing SuggestionOverlay implementation in InventoryTabView.axaml.cs
- MasterDataService patterns for reference data loading
