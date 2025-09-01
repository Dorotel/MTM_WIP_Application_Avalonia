# Complete_Stored_Procedures_Implementation_Phase3 - Agent Instructions

## 🎯 **AGENT EXECUTION CONTEXT**
**Issue Type:** Phase
**Complexity:** Medium
**Estimated Time:** 1hr
**Dependencies:** Phase1 and Phase2 completion, all services updated

## 📋 **PRECISE OBJECTIVES**
### Primary Goal
Update all relevant documentation and instruction files with new stored procedure information, remove outdated content, and ensure comprehensive documentation of the complete database layer implementation.

### Acceptance Criteria
- [ ] Database patterns documentation updated with complete procedure catalog
- [ ] All instruction files reflect new stored procedure implementations
- [ ] Outdated or irrelevant database information removed
- [ ] Service documentation updated to reflect new capabilities
- [ ] README files updated with complete database operation examples

## 🔧 **IMPLEMENTATION DETAILS**

### Files to Modify/Create
```
.github/Development-Instructions/database-patterns.instruction.md - Complete procedure documentation
Documentation/Development/Database_Files/README.html - Updated with new procedures
.github/copilot-instructions.md - Database service examples updated
Documentation/Development/Examples/ - Add comprehensive database examples
.github/Custom-Prompts/CustomPrompt_Create_StoredProcedure.md - Update with new patterns
.github/Custom-Prompts/CustomPrompt_Update_StoredProcedure.md - Current procedure catalog
```

### Code Patterns Required
```markdown
### Complete MTM Stored Procedure Catalog

#### Inventory Management Procedures
- `inv_inventory_Add_Item` - Add inventory with automatic batch number generation
- `inv_inventory_Remove_Item` - Remove inventory with validation and audit trail
- `inv_inventory_Transfer_Part` - Transfer entire part to new location
- `inv_inventory_Transfer_Quantity` - Transfer partial quantity with splitting
- `inv_inventory_Get_ByPartID` - Retrieve inventory by part identifier
- `inv_inventory_Get_ByPartIDandOperation` - Retrieve by part and operation
- `inv_inventory_Get_ByUser` - Retrieve user's inventory transactions

#### Master Data Procedures
- `md_part_ids_*` - Complete part management (Add, Update, Delete, Get operations)
- `md_locations_*` - Location management with building support
- `md_operations_*` - Operation number management
- `md_item_types_*` - Item type classification management

#### User Management Procedures
- `usr_users_*` - Complete user lifecycle management
- `usr_ui_settings_*` - User interface settings and preferences
- `sys_user_roles_*` - Role-based access control

#### Quick Button Procedures
- `qb_quickbuttons_*` - Quick action button management
- `sys_last_10_transactions_*` - Recent transaction tracking

#### System Procedures
- `sys_roles_*` - System role management
- `log_*` - System logging and changelog
```

### Database Operations (If Applicable)
```markdown
All database operations now use standardized stored procedures with:
- Comprehensive input validation
- Proper error handling with status codes
- MySQL 5.7 compatibility
- Audit trail functionality
- Business rule enforcement
```

## ⚡ **EXECUTION SEQUENCE**
1. **Step 1:** Update database-patterns.instruction.md with complete procedure catalog
2. **Step 2:** Update README.html with new procedure documentation
3. **Step 3:** Update copilot-instructions.md database examples
4. **Step 4:** Create comprehensive database operation examples
5. **Step 5:** Update custom prompt templates with new procedure patterns
6. **Step 6:** Remove outdated database information from instruction files
7. **Step 7:** Validate all cross-references and links in documentation

## 🧪 **VALIDATION REQUIREMENTS**
### Automated Tests
- [ ] All documentation links functional and accurate
- [ ] Cross-references between files maintained properly

### Manual Verification
- [ ] Database patterns documentation is comprehensive and accurate
- [ ] Examples in documentation match actual procedure implementations
- [ ] No outdated information remains in instruction files
- [ ] Service documentation reflects actual capabilities

## 🔗 **CONTEXT REFERENCES**
### Related Files
- [.github/Development-Instructions/database-patterns.instruction.md](.github/Development-Instructions/database-patterns.instruction.md) - Primary database documentation
- [Documentation/Development/Database_Files/README.html](../../../../Documentation/Development/Database_Files/README.html) - Database file overview
- [.github/copilot-instructions.md](.github/copilot-instructions.md) - Main instruction file

### MTM-Specific Requirements
- **Transaction Type Logic:** Document user intent vs operation number distinction
- **Database Pattern:** Emphasize stored procedures only approach
- **UI Pattern:** Document service integration with ViewModels

## 🚨 **ERROR HANDLING**
### Expected Issues
- Broken documentation links after file updates
- Inconsistent information between different documentation files
- Missing examples for new procedure functionality

### Rollback Plan
- Backup all documentation files before modifications
- Maintain existing examples while adding new ones
- Preserve established documentation structure and formatting

---
*Agent-Optimized Instructions for GitHub Copilot*
