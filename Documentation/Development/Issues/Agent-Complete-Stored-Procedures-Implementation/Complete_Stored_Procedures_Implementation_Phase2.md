# Complete_Stored_Procedures_Implementation_Phase2 - Agent Instructions

## 🎯 **AGENT EXECUTION CONTEXT**
**Issue Type:** Phase
**Complexity:** Complex
**Estimated Time:** 4hr
**Dependencies:** Phase1 completion, Updated_Stored_Procedures.sql generated

## 📋 **PRECISE OBJECTIVES**
### Primary Goal
Update all service files that call stored procedures to ensure they send correct parameters and handle responses properly, maintaining compatibility with the newly generated stored procedures.

### Acceptance Criteria
- [ ] All Database.cs method calls updated to match new stored procedures
- [ ] QuickButtons.cs service fully compatible with updated procedures
- [ ] All Helper_Database_StoredProcedure calls validated and corrected
- [ ] Error handling properly implemented for all database operations
- [ ] Parameter validation added where missing

## 🔧 **IMPLEMENTATION DETAILS**

### Files to Modify/Create
```
Services/Database.cs - Update all stored procedure calls
Services/QuickButtons.cs - Validate QuickButton procedure integration
Services/ErrorHandling.cs - Enhance database error handling if needed
ViewModels/MainForm/InventoryViewModel.cs - Database operation validation
ViewModels/MainForm/AddItemViewModel.cs - Add item procedure integration
ViewModels/MainForm/RemoveItemViewModel.cs - Remove item procedure integration
ViewModels/SettingsForm/UserManagementViewModel.cs - User management procedures
```

### Code Patterns Required
```csharp
// MTM Database Service Pattern - Updated for comprehensive procedures
public async Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string notes)
{
    var parameters = new Dictionary<string, object>
    {
        ["p_PartID"] = partId,
        ["p_Location"] = location,
        ["p_Operation"] = operation,
        ["p_Quantity"] = quantity,
        ["p_ItemType"] = itemType,
        ["p_User"] = user,
        ["p_Notes"] = !string.IsNullOrWhiteSpace(notes) ? notes : DBNull.Value
    };

    return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        _connectionString,
        "inv_inventory_Add_Item",
        parameters
    );
}
```

### Database Operations (If Applicable)
```csharp
// Validate all procedure calls match new signatures
// Update parameter names to match stored procedure definitions
// Ensure proper handling of OUT parameters (p_Status, p_ErrorMsg)
// Add missing procedure calls for any unimplemented operations
```

## ⚡ **EXECUTION SEQUENCE**
1. **Step 1:** Review Database.cs service against new stored procedures in Updated_Stored_Procedures.sql
2. **Step 2:** Update all method signatures and parameter dictionaries to match procedures
3. **Step 3:** Validate QuickButtons.cs service calls against QuickButton procedures
4. **Step 4:** Update Helper_Database_StoredProcedure usage patterns if needed
5. **Step 5:** Add missing database operations identified in Phase1 analysis
6. **Step 6:** Enhance error handling for improved database operation reliability
7. **Step 7:** Test all service method calls for parameter compatibility

## 🧪 **VALIDATION REQUIREMENTS**
### Automated Tests
- [ ] All Database.cs methods compile successfully
- [ ] Parameter dictionaries match stored procedure signatures
- [ ] Helper_Database_StoredProcedure calls use correct procedure names

### Manual Verification
- [ ] All database operations return proper StatusResult objects
- [ ] Error handling gracefully manages database failures
- [ ] QuickButton operations work with updated procedures
- [ ] User management operations function correctly

## 🔗 **CONTEXT REFERENCES**
### Related Files
- [Services/Database.cs](../../../../Services/Database.cs) - Primary database service
- [Services/QuickButtons.cs](../../../../Services/QuickButtons.cs) - QuickButton database operations
- [Documentation/Development/Database_Files/Updated_Stored_Procedures.sql](../../../../Documentation/Development/Database_Files/Updated_Stored_Procedures.sql) - New procedures

### MTM-Specific Requirements
- **Transaction Type Logic:** Ensure user intent drives TransactionType, not operation numbers
- **Database Pattern:** Maintain stored procedures only approach
- **UI Pattern:** Support ViewModel data binding with proper error propagation

## 🚨 **ERROR HANDLING**
### Expected Issues
- Parameter name mismatches between service calls and procedures
- Missing output parameter handling for status/error messages
- Type conversion issues between C# and MySQL parameter types

### Rollback Plan
- Preserve current Database.cs implementation before modifications
- Maintain compatibility with existing ViewModel calls
- Ensure no breaking changes to public service interfaces

---
*Agent-Optimized Instructions for GitHub Copilot*
