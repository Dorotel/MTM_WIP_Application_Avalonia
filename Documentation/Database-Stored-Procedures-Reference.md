# MTM WIP Application - Database Stored Procedures Reference

**Generated:** September 21, 2025  
**Database:** mtm_wip_application_test  
**Purpose:** Complete reference for all stored procedures and their parameters used by the MTM WIP Application

## Overview

This document provides a comprehensive reference for all stored procedures currently implemented in the MTM WIP Application database. Each procedure is documented with its parameters, data types, and modes (IN/OUT).

## Connection Information

- **Development Database:** `mtm_wip_application_test`
- **Production Database:** `mtm_wip_application`
- **Server:** localhost (MAMP)
- **Credentials:** root/root

## Inventory Management Procedures

### inv_inventory_Add_Item

**Purpose:** Adds a new inventory item to the system

**Parameters:**

- `p_PartID` (varchar, IN) - Part identifier
- `p_Location` (varchar, IN) - Storage location
- `p_Operation` (varchar, IN) - Operation number
- `p_Quantity` (int, IN) - Quantity to add
- `p_ItemType` (varchar, IN) - Type of item
- `p_User` (varchar, IN) - User performing the operation
- `p_Notes` (varchar, IN) - Optional notes
- `p_Status` (int, OUT) - Operation status (1=success, 0=failure)
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### inv_inventory_Get_ByID

**Purpose:** Retrieves inventory item by ID

**Parameters:**

- `p_ID` (int, IN) - Inventory item ID

### inv_inventory_Get_ByID_ForEdit

**Purpose:** Retrieves inventory item by ID for editing purposes

**Parameters:**

- `p_ID` (int, IN) - Inventory item ID

### inv_inventory_Get_ByPartID

**Purpose:** Retrieves all inventory items for a specific part ID

**Parameters:**

- `p_PartID` (varchar, IN) - Part identifier

### inv_inventory_Get_ByPartIDandOperation

**Purpose:** Retrieves inventory items by part ID and operation

**Parameters:**

- `p_PartID` (varchar, IN) - Part identifier
- `p_Operation` (varchar, IN) - Operation number

### inv_inventory_Get_ByUser

**Purpose:** Retrieves inventory items associated with a specific user

**Parameters:**

- `p_User` (varchar, IN) - Username

### inv_inventory_Remove_Item

**Purpose:** Removes inventory items from the system

**Parameters:**

- `p_PartID` (varchar, IN) - Part identifier
- `p_Location` (varchar, IN) - Storage location
- `p_Operation` (varchar, IN) - Operation number
- `p_Quantity` (int, IN) - Quantity to remove
- `p_ItemType` (varchar, IN) - Type of item
- `p_User` (varchar, IN) - User performing the operation
- `p_BatchNumber` (varchar, IN) - Batch number
- `p_Notes` (varchar, IN) - Optional notes
- `p_Status` (int, OUT) - Operation status (1=success, 0=failure)
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### inv_inventory_Transfer_Part

**Purpose:** Transfers entire part to new location

**Parameters:**

- `in_BatchNumber` (varchar, IN) - Batch number
- `in_PartID` (varchar, IN) - Part identifier
- `in_Operation` (varchar, IN) - Operation number
- `in_NewLocation` (varchar, IN) - Destination location

### inv_inventory_Transfer_Quantity

**Purpose:** Transfers specific quantity to new location

**Parameters:**

- `in_BatchNumber` (varchar, IN) - Batch number
- `in_PartID` (varchar, IN) - Part identifier
- `in_Operation` (varchar, IN) - Operation number
- `in_TransferQuantity` (int, IN) - Quantity to transfer
- `in_OriginalQuantity` (int, IN) - Original quantity
- `in_NewLocation` (varchar, IN) - Destination location
- `in_User` (varchar, IN) - User performing transfer

### inv_inventory_Update_Item

**Purpose:** Updates existing inventory item

**Parameters:**

- `p_ID` (int, IN) - Inventory item ID
- `p_PartID` (varchar, IN) - Part identifier
- `p_Location` (varchar, IN) - Storage location
- `p_Operation` (varchar, IN) - Operation number
- `p_Quantity` (int, IN) - Updated quantity
- `p_ItemType` (varchar, IN) - Type of item
- `p_BatchNumber` (varchar, IN) - Batch number
- `p_Notes` (varchar, IN) - Optional notes
- `p_User` (varchar, IN) - User performing update
- `p_Original_PartID` (varchar, IN) - Original part ID
- `p_Original_BatchNumber` (varchar, IN) - Original batch number
- `p_Status` (int, OUT) - Operation status (1=success, 0=failure)
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### inv_inventory_Update_Notes

**Purpose:** Updates notes for existing inventory item

**Parameters:**

- `p_ID` (int, IN) - Inventory item ID
- `p_PartID` (varchar, IN) - Part identifier
- `p_BatchNumber` (varchar, IN) - Batch number
- `p_Notes` (varchar, IN) - Updated notes
- `p_User` (varchar, IN) - User performing update
- `p_Status` (int, OUT) - Operation status (1=success, 0=failure)
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### inv_inventory_Validate_MasterData

**Purpose:** Validates master data for inventory operations

**Parameters:**

- `p_PartID` (varchar, IN) - Part identifier
- `p_Operation` (varchar, IN) - Operation number
- `p_Location` (varchar, IN) - Storage location

## Master Data Procedures

### md_item_types_Add_ItemType

**Purpose:** Adds a new item type to master data

**Parameters:**

- `p_ItemType` (varchar, IN) - Item type name
- `p_IssuedBy` (varchar, IN) - User creating the item type

### md_item_types_Delete_ByID

**Purpose:** Deletes item type by ID

**Parameters:**

- `p_ID` (int, IN) - Item type ID

### md_item_types_Delete_ByType

**Purpose:** Deletes item type by type name

**Parameters:**

- `p_ItemType` (varchar, IN) - Item type name

### md_item_types_Get_All

**Purpose:** Retrieves all item types

**Parameters:** None

### md_item_types_Update_ItemType

**Purpose:** Updates existing item type

**Parameters:**

- `p_ID` (int, IN) - Item type ID
- `p_ItemType` (varchar, IN) - Updated item type name
- `p_IssuedBy` (varchar, IN) - User performing update

### md_locations_Add_Location

**Purpose:** Adds a new location to master data

**Parameters:**

- `p_Location` (varchar, IN) - Location name
- `p_IssuedBy` (varchar, IN) - User creating the location
- `p_Building` (varchar, IN) - Building identifier

### md_locations_Delete_ByLocation

**Purpose:** Deletes location by name

**Parameters:**

- `p_Location` (varchar, IN) - Location name

### md_locations_Get_All

**Purpose:** Retrieves all locations

**Parameters:** None

### md_locations_Update_Location

**Purpose:** Updates existing location

**Parameters:**

- `p_OldLocation` (varchar, IN) - Current location name
- `p_Location` (varchar, IN) - New location name
- `p_IssuedBy` (varchar, IN) - User performing update
- `p_Building` (varchar, IN) - Building identifier

### md_operation_numbers_Add_Operation

**Purpose:** Adds a new operation number to master data

**Parameters:**

- `p_Operation` (varchar, IN) - Operation number
- `p_IssuedBy` (varchar, IN) - User creating the operation

### md_operation_numbers_Delete_ByOperation

**Purpose:** Deletes operation number by value

**Parameters:**

- `p_Operation` (varchar, IN) - Operation number

### md_operation_numbers_Get_All

**Purpose:** Retrieves all operation numbers

**Parameters:** None

### md_operation_numbers_Update_Operation

**Purpose:** Updates existing operation number

**Parameters:**

- `p_Operation` (varchar, IN) - Current operation number
- `p_NewOperation` (varchar, IN) - New operation number
- `p_IssuedBy` (varchar, IN) - User performing update

### md_part_ids_Add_Part

**Purpose:** Adds a new part ID to master data

**Parameters:**

- `p_ItemNumber` (varchar, IN) - Part number
- `p_Customer` (varchar, IN) - Customer name
- `p_Description` (varchar, IN) - Part description
- `p_IssuedBy` (varchar, IN) - User creating the part
- `p_ItemType` (varchar, IN) - Type of item

### md_part_ids_Delete_ByItemNumber

**Purpose:** Deletes part ID by item number

**Parameters:**

- `p_ItemNumber` (varchar, IN) - Part number

### md_part_ids_Get_All

**Purpose:** Retrieves all part IDs

**Parameters:** None

### md_part_ids_Get_ByItemNumber

**Purpose:** Retrieves specific part ID by item number

**Parameters:**

- `p_ItemNumber` (varchar, IN) - Part number

### md_part_ids_Update_Part

**Purpose:** Updates existing part ID

**Parameters:**

- `p_ID` (int, IN) - Part ID
- `p_ItemNumber` (varchar, IN) - Part number
- `p_Customer` (varchar, IN) - Customer name
- `p_Description` (varchar, IN) - Part description
- `p_IssuedBy` (varchar, IN) - User performing update
- `p_ItemType` (varchar, IN) - Type of item

## Quick Buttons Procedures

### qb_quickbuttons_Clear_ByUser

**Purpose:** Clears all quick buttons for a user

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### qb_quickbuttons_Get_ByUser

**Purpose:** Retrieves quick buttons for a specific user

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### qb_quickbuttons_Remove

**Purpose:** Removes a specific quick button

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Position` (int, IN) - Button position
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### qb_quickbuttons_Save

**Purpose:** Saves a quick button configuration

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Position` (int, IN) - Button position
- `p_PartID` (varchar, IN) - Part identifier
- `p_Operation` (varchar, IN) - Operation number
- `p_Quantity` (int, IN) - Quantity
- `p_Location` (varchar, IN) - Location
- `p_ItemType` (varchar, IN) - Item type
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### qb_quickbuttons_Save_Test

**Purpose:** Test version of quick button save operation

**Parameters:** Same as qb_quickbuttons_Save

## System Procedures

### sys_last_10_transactions_Add_Transaction

**Purpose:** Adds a transaction to the last 10 transactions log

**Parameters:**

- `p_TransactionType` (enum, IN) - Type of transaction
- `p_BatchNumber` (varchar, IN) - Batch number
- `p_PartID` (varchar, IN) - Part identifier
- `p_FromLocation` (varchar, IN) - Source location
- `p_ToLocation` (varchar, IN) - Destination location
- `p_Operation` (varchar, IN) - Operation number
- `p_Quantity` (int, IN) - Transaction quantity
- `p_Notes` (varchar, IN) - Optional notes
- `p_User` (varchar, IN) - User performing transaction
- `p_ItemType` (varchar, IN) - Item type
- `p_ReceiveDate` (datetime, IN) - Transaction date/time
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### sys_last_10_transactions_Get_ByUser

**Purpose:** Retrieves last transactions for a user

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Limit` (int, IN) - Maximum number of transactions to return

### sys_roles_Get_ById

**Purpose:** Retrieves role information by ID

**Parameters:**

- `p_ID` (int, IN) - Role ID

### sys_user_roles_Add

**Purpose:** Adds a role to a user

**Parameters:**

- `p_User` (int, IN) - User ID
- `p_RoleID` (int, IN) - Role ID
- `p_AssignedBy` (varchar, IN) - User assigning the role

### sys_user_roles_Delete

**Purpose:** Removes a role from a user

**Parameters:**

- `p_User` (int, IN) - User ID
- `p_RoleID` (int, IN) - Role ID

### sys_user_roles_Update

**Purpose:** Updates a user's role

**Parameters:**

- `p_User` (int, IN) - User ID
- `p_NewRoleID` (int, IN) - New role ID
- `p_AssignedBy` (varchar, IN) - User performing update

### sys_user_Validate

**Purpose:** Validates a user

**Parameters:**

- `p_User` (varchar, IN) - Username to validate
- `p_ValidatingUserID` (varchar, IN) - User performing validation
- `p_Status` (int, OUT) - Validation status
- `p_ErrorMsg` (varchar, OUT) - Error message if validation fails

## User Management & Settings Procedures

### usr_ui_settings_Get

**Purpose:** Retrieves UI settings for a user

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### usr_ui_settings_GetShortcutsJson

**Purpose:** Retrieves user shortcuts as JSON

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_ShortcutsJson` (json, OUT) - Shortcuts configuration JSON

### usr_ui_settings_SetJsonSetting

**Purpose:** Sets a JSON setting for a user

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_DgvName` (varchar, IN) - DataGridView name
- `p_SettingJson` (json, IN) - Setting configuration JSON
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### usr_ui_settings_SetShortcutsJson

**Purpose:** Sets user shortcuts as JSON

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_ShortcutsJson` (json, IN) - Shortcuts configuration JSON
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### usr_ui_settings_SetThemeJson

**Purpose:** Sets user theme configuration as JSON

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_ThemeJson` (json, IN) - Theme configuration JSON
- `p_Status` (int, OUT) - Operation status
- `p_ErrorMsg` (varchar, OUT) - Error message if operation fails

### usr_users_Add_User

**Purpose:** Adds a new user to the system

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_FullName` (varchar, IN) - Full name
- `p_Shift` (varchar, IN) - Work shift
- `p_VitsUser` (tinyint, IN) - VITS user flag
- `p_Pin` (varchar, IN) - User PIN
- `p_LastShownVersion` (varchar, IN) - Last shown version
- `p_HideChangeLog` (varchar, IN) - Hide change log flag
- `p_Theme_Name` (varchar, IN) - Theme name
- `p_Theme_FontSize` (int, IN) - Font size
- `p_VisualUserName` (varchar, IN) - Visual username
- `p_VisualPassword` (varchar, IN) - Visual password
- `p_WipServerAddress` (varchar, IN) - WIP server address
- `p_WipServerPort` (varchar, IN) - WIP server port
- `p_WipDatabase` (varchar, IN) - WIP database name

### usr_users_Delete_User

**Purpose:** Deletes a user from the system

**Parameters:**

- `p_User` (varchar, IN) - Username

### usr_users_Exists

**Purpose:** Checks if a user exists

**Parameters:**

- `p_User` (varchar, IN) - Username

### usr_users_Get_All

**Purpose:** Retrieves all users

**Parameters:** None

### usr_users_Get_ByUser

**Purpose:** Retrieves specific user information

**Parameters:**

- `p_User` (varchar, IN) - Username

### usr_users_Update_User

**Purpose:** Updates existing user information

**Parameters:**

- `p_User` (varchar, IN) - Username
- `p_FullName` (varchar, IN) - Full name
- `p_Shift` (varchar, IN) - Work shift
- `p_Pin` (varchar, IN) - User PIN
- `p_VisualUserName` (varchar, IN) - Visual username
- `p_VisualPassword` (varchar, IN) - Visual password

## Changelog Management

### log_changelog_Get_Current

**Purpose:** Retrieves current changelog information

**Parameters:** None

## Application Usage Pattern

The MTM WIP Application uses these stored procedures through the `Helper_Database_StoredProcedure.ExecuteDataTableWithStatus()` method. All database operations follow this pattern:

```csharp
var parameters = new MySqlParameter[]
{
    new("parameter_name", value),
    // ... additional parameters
};

var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
    connectionString,
    "stored_procedure_name",
    parameters
);
```

## Notes

1. **OUT Parameters:** Procedures with OUT parameters return status codes and error messages
2. **Parameter Naming:** All parameters use lowercase with underscores (snake_case)
3. **Status Codes:** Generally 1 = success, 0 = failure
4. **User Context:** Most procedures require a user parameter for audit trails
5. **Transaction Safety:** All modification procedures should be called within transactions

## Database Schema Validation

To verify parameter compatibility, use this SQL query:

```sql
SELECT SPECIFIC_NAME, PARAMETER_NAME, DATA_TYPE, PARAMETER_MODE, ORDINAL_POSITION 
FROM information_schema.PARAMETERS 
WHERE SPECIFIC_SCHEMA='mtm_wip_application_test' 
ORDER BY SPECIFIC_NAME, ORDINAL_POSITION;
```

---

**Last Updated:** September 21, 2025  
**Application Version:** 5.0.0  
**Documentation Generated By:** MTM Development Team
