# Database Stored Procedures - OUT Parameters Classification

**CRITICAL BUG IDENTIFIED**: The Helper_Database_StoredProcedure.cs class is automatically adding `@p_Status` and `@p_Message` OUT parameters to ALL stored procedure calls, but only 16 out of 51 procedures actually have OUT parameters. This causes "Incorrect number of arguments" errors for the other 35 procedures.

## Procedures WITH OUT Parameters (16 total)

These procedures expect `p_Status (int, OUT)` and `p_ErrorMsg (varchar, OUT)` parameters:

### Inventory Operations

1. `inv_inventory_Add_Item` - Has: p_Status(OUT), p_ErrorMsg(OUT)
2. `inv_inventory_Remove_Item` - Has: p_Status(OUT), p_ErrorMsg(OUT)  
3. `inv_inventory_Update_Item` - Has: p_Status(OUT), p_ErrorMsg(OUT)
4. `inv_inventory_Update_Notes` - Has: p_Status(OUT), p_ErrorMsg(OUT)

### Quick Buttons

5. `qb_quickbuttons_Clear_ByUser` - Has: p_Status(OUT), p_ErrorMsg(OUT)
6. `qb_quickbuttons_Get_ByUser` - Has: p_Status(OUT), p_ErrorMsg(OUT)
7. `qb_quickbuttons_Remove` - Has: p_Status(OUT), p_ErrorMsg(OUT)
8. `qb_quickbuttons_Save` - Has: p_Status(OUT), p_ErrorMsg(OUT)
9. `qb_quickbuttons_Save_Test` - Has: p_Status(OUT), p_ErrorMsg(OUT)

### System Operations

10. `sys_last_10_transactions_Add_Transaction` - Has: p_Status(OUT), p_ErrorMsg(OUT)
11. `sys_user_Validate` - Has: p_Status(OUT), p_ErrorMsg(OUT)

### User Settings

12. `usr_ui_settings_Get` - Has: p_Status(OUT), p_ErrorMsg(OUT)
13. `usr_ui_settings_GetShortcutsJson` - Has: p_ShortcutsJson(OUT) **[DIFFERENT PATTERN!]**
14. `usr_ui_settings_SetJsonSetting` - Has: p_Status(OUT), p_ErrorMsg(OUT)
15. `usr_ui_settings_SetShortcutsJson` - Has: p_Status(OUT), p_ErrorMsg(OUT)
16. `usr_ui_settings_SetThemeJson` - Has: p_Status(OUT), p_ErrorMsg(OUT)

## Procedures WITHOUT OUT Parameters (35 total)

These procedures expect NO OUT parameters and will fail if Helper class adds them:

### Inventory GET Operations

1. `inv_inventory_Get_ByID`
2. `inv_inventory_Get_ByID_ForEdit`
3. `inv_inventory_Get_ByPartID`
4. `inv_inventory_Get_ByPartIDandOperation`
5. `inv_inventory_Get_ByUser`

### Inventory Transfer Operations

6. `inv_inventory_Transfer_Part`
7. `inv_inventory_Transfer_Quantity`
8. `inv_inventory_Validate_MasterData`

### Log Operations

9. `log_changelog_Get_Current`

### Master Data - Item Types

10. `md_item_types_Add_ItemType`
11. `md_item_types_Delete_ByID`
12. `md_item_types_Delete_ByType`
13. `md_item_types_Get_All`
14. `md_item_types_Update_ItemType`

### Master Data - Locations  

15. `md_locations_Add_Location`
16. `md_locations_Delete_ByLocation`
17. `md_locations_Get_All`
18. `md_locations_Update_Location`

### Master Data - Operation Numbers

19. `md_operation_numbers_Add_Operation`
20. `md_operation_numbers_Delete_ByOperation`
21. `md_operation_numbers_Get_All`
22. `md_operation_numbers_Update_Operation`

### Master Data - Part IDs

23. `md_part_ids_Add_Part`
24. `md_part_ids_Delete_ByItemNumber`
25. `md_part_ids_Get_All`
26. `md_part_ids_Get_ByItemNumber`
27. `md_part_ids_Update_Part`

### System Operations

28. `sys_last_10_transactions_Get_ByUser`
29. `sys_roles_Get_ById`
30. `sys_user_roles_Add`
31. `sys_user_roles_Delete`
32. `sys_user_roles_Update`

### User Management

33. `usr_users_Add_User`
34. `usr_users_Delete_User`
35. `usr_users_Exists`
36. `usr_users_Get_All`
37. `usr_users_Get_ByUser`
38. `usr_users_Update_User`

## Helper Class Fix Requirements

The Helper_Database_StoredProcedure.cs class needs to be modified to:

1. **Only add OUT parameters for the 16 procedures that have them**
2. **NOT add any OUT parameters for the 35 procedures without them**
3. **Handle the special case of `usr_ui_settings_GetShortcutsJson` which has `p_ShortcutsJson(OUT)` instead of the standard pattern**

## Status Code Patterns

- **Procedures with OUT parameters**: Return status via p_Status parameter (-1=error, 0/1=success)
- **Procedures without OUT parameters**: Return status via return value or result set structure
- **GET procedures**: Typically return data directly without status parameters
- **ADD/UPDATE/DELETE procedures**: More likely to have status parameters for error handling

## Impact Assessment

This bug affects **ALL database operations** in the application, causing failures on any procedure without OUT parameters (69% of all procedures). This explains why:

- Master data dropdowns are not loading (md_part_ids_Get_All, md_locations_Get_All, etc.)
- Inventory retrieval is failing (inv_inventory_Get_ByPartID, etc.)
- User management operations are broken (usr_users_Get_All, etc.)

**Priority**: CRITICAL - Must fix before any database functionality will work.
