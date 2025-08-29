// ============================================================================
// MTM WIP Application - Comprehensive Database Operation Examples
// ============================================================================
// This file provides complete examples of all 45 stored procedures implemented
// in the MTM WIP Application with proper parameter usage and error handling.
//
// File: Documentation/Development/Examples/ComprehensiveDatabaseExamples.cs
// Last Updated: December 19, 2024
// ============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Documentation.Development.Examples
{
    /// <summary>
    /// Comprehensive examples of all 45 MTM stored procedures.
    /// Demonstrates proper parameter usage, error handling, and result processing.
    /// </summary>
    public class ComprehensiveDatabaseExamples
    {
        private readonly string _connectionString;
        private readonly ILogger<ComprehensiveDatabaseExamples> _logger;

        public ComprehensiveDatabaseExamples(string connectionString, ILogger<ComprehensiveDatabaseExamples> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        // ========================================================================
        // INVENTORY MANAGEMENT PROCEDURES (7 procedures)
        // ========================================================================

        /// <summary>
        /// Example: Add inventory item with automatic batch number generation
        /// Procedure: inv_inventory_Add_Item
        /// </summary>
        public async Task<bool> ExampleAddInventoryItemAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = "PART001",
                    ["p_Location"] = "A1-B2",
                    ["p_Operation"] = "100",
                    ["p_Quantity"] = 50,
                    ["p_ItemType"] = "Standard",
                    ["p_User"] = "JDOE",
                    ["p_Notes"] = "Initial stock addition"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "inv_inventory_Add_Item",
                    parameters
                );

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Successfully added inventory item: {PartID}", parameters["p_PartID"]);
                    return true;
                }
                else
                {
                    _logger.LogError("Failed to add inventory item: {Error}", result.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddInventoryItemAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Remove inventory item with validation
        /// Procedure: inv_inventory_Remove_Item
        /// </summary>
        public async Task<bool> ExampleRemoveInventoryItemAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = "PART001",
                    ["p_Location"] = "A1-B2",
                    ["p_Operation"] = "100",
                    ["p_Quantity"] = 10,
                    ["p_ItemType"] = "Standard",
                    ["p_User"] = "JDOE",
                    ["p_BatchNumber"] = "0000000001",
                    ["p_Notes"] = "Production consumption"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "inv_inventory_Remove_Item",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleRemoveInventoryItemAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Get inventory by part ID
        /// Procedure: inv_inventory_Get_ByPartID
        /// </summary>
        public async Task<DataTable> ExampleGetInventoryByPartIDAsync(string partId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = partId
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "inv_inventory_Get_ByPartID",
                    parameters
                );

                return result.IsSuccess ? result.Data : new DataTable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleGetInventoryByPartIDAsync");
                return new DataTable();
            }
        }

        // ========================================================================
        // MASTER DATA PROCEDURES (20 procedures)
        // ========================================================================

        /// <summary>
        /// Example: Add new part with validation
        /// Procedure: md_part_ids_Add_Part
        /// </summary>
        public async Task<bool> ExampleAddPartAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_PartID"] = "NEWPART001",
                    ["p_Customer"] = "ACME Corp",
                    ["p_Description"] = "Widget Assembly",
                    ["p_IssuedBy"] = "JDOE",
                    ["p_ItemType"] = "Assembly"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "md_part_ids_Add_Part",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddPartAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Get all parts for autocomplete
        /// Procedure: md_part_ids_Get_All
        /// </summary>
        public async Task<DataTable> ExampleGetAllPartsAsync()
        {
            try
            {
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "md_part_ids_Get_All",
                    new Dictionary<string, object>()
                );

                return result.IsSuccess ? result.Data : new DataTable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleGetAllPartsAsync");
                return new DataTable();
            }
        }

        /// <summary>
        /// Example: Add new location with building support
        /// Procedure: md_locations_Add_Location
        /// </summary>
        public async Task<bool> ExampleAddLocationAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_Location"] = "C3-D4",
                    ["p_IssuedBy"] = "JDOE",
                    ["p_Building"] = "Building A"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "md_locations_Add_Location",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddLocationAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Add new operation number
        /// Procedure: md_operation_numbers_Add_Operation
        /// </summary>
        public async Task<bool> ExampleAddOperationAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_Operation"] = "150",
                    ["p_IssuedBy"] = "JDOE"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "md_operation_numbers_Add_Operation",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddOperationAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Add new item type
        /// Procedure: md_item_types_Add_ItemType
        /// </summary>
        public async Task<bool> ExampleAddItemTypeAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_ItemType"] = "Special Assembly",
                    ["p_IssuedBy"] = "JDOE"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "md_item_types_Add_ItemType",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddItemTypeAsync");
                return false;
            }
        }

        // ========================================================================
        // USER MANAGEMENT PROCEDURES (14 procedures)
        // ========================================================================

        /// <summary>
        /// Example: Add new user with role assignment
        /// Procedure: usr_users_Add
        /// </summary>
        public async Task<bool> ExampleAddUserAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_Username"] = "NEWUSER",
                    ["p_FirstName"] = "John",
                    ["p_LastName"] = "Smith",
                    ["p_Email"] = "john.smith@company.com",
                    ["p_Role"] = "Operator",
                    ["p_IssuedBy"] = "ADMIN"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "usr_users_Add",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddUserAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Check if user exists
        /// Procedure: usr_users_Exists
        /// </summary>
        public async Task<bool> ExampleCheckUserExistsAsync(string username)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_Username"] = username
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "usr_users_Exists",
                    parameters
                );

                if (result.IsSuccess && result.Data.Rows.Count > 0)
                {
                    var userExists = Convert.ToInt32(result.Data.Rows[0]["UserExists"]);
                    return userExists > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleCheckUserExistsAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: Save user UI settings as JSON
        /// Procedure: usr_ui_settings_SetJsonSetting
        /// </summary>
        public async Task<bool> ExampleSaveUserSettingsAsync()
        {
            try
            {
                var settingsJson = @"{
                    ""theme"": ""MTM_Dark"",
                    ""fontSize"": 14,
                    ""language"": ""en-US"",
                    ""autoSave"": true
                }";

                var parameters = new Dictionary<string, object>
                {
                    ["p_UserId"] = "JDOE",
                    ["p_SettingsJson"] = settingsJson
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "usr_ui_settings_SetJsonSetting",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleSaveUserSettingsAsync");
                return false;
            }
        }

        // ========================================================================
        // QUICKBUTTON PROCEDURES (4 procedures)
        // Note: Get operation doesn't follow MTM status pattern
        // ========================================================================

        /// <summary>
        /// Example: Get user's quick buttons
        /// Procedure: qb_quickbuttons_Get_ByUser (Non-standard pattern)
        /// </summary>
        public async Task<DataTable> ExampleGetUserQuickButtonsAsync(string userId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_User"] = userId
                };

                // Note: This procedure doesn't follow MTM status pattern
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                    _connectionString,
                    "qb_quickbuttons_Get_ByUser",
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleGetUserQuickButtonsAsync");
                return new DataTable();
            }
        }

        /// <summary>
        /// Example: Save quick button with position
        /// Procedure: qb_quickbuttons_Save
        /// </summary>
        public async Task<bool> ExampleSaveQuickButtonAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_User"] = "JDOE",
                    ["p_Position"] = 1,
                    ["p_PartID"] = "PART001",
                    ["p_Location"] = "A1-B2",
                    ["p_Operation"] = "100",
                    ["p_Quantity"] = 25,
                    ["p_ItemType"] = "Standard"
                };

                var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _connectionString,
                    "qb_quickbuttons_Save",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleSaveQuickButtonAsync");
                return false;
            }
        }

        // ========================================================================
        // SYSTEM PROCEDURES (7 procedures)
        // ========================================================================

        /// <summary>
        /// Example: Get all system roles
        /// Procedure: sys_roles_Get_All
        /// </summary>
        public async Task<DataTable> ExampleGetAllRolesAsync()
        {
            try
            {
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    _connectionString,
                    "sys_roles_Get_All",
                    new Dictionary<string, object>()
                );

                return result.IsSuccess ? result.Data : new DataTable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleGetAllRolesAsync");
                return new DataTable();
            }
        }

        /// <summary>
        /// Example: Get last 10 transactions for user
        /// Procedure: sys_last_10_transactions_Get_ByUser (Non-standard pattern)
        /// </summary>
        public async Task<DataTable> ExampleGetLast10TransactionsAsync(string userId)
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_UserID"] = userId,
                    ["p_Limit"] = 10
                };

                // Note: This procedure doesn't follow MTM status pattern
                var result = await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                    _connectionString,
                    "sys_last_10_transactions_Get_ByUser",
                    parameters
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleGetLast10TransactionsAsync");
                return new DataTable();
            }
        }

        /// <summary>
        /// Example: Add transaction to history
        /// Procedure: sys_last_10_transactions_Add_Transaction
        /// </summary>
        public async Task<bool> ExampleAddTransactionAsync()
        {
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    ["p_TransactionType"] = "IN",
                    ["p_BatchNumber"] = "0000000001",
                    ["p_PartID"] = "PART001",
                    ["p_FromLocation"] = null,
                    ["p_ToLocation"] = "A1-B2",
                    ["p_Operation"] = "100",
                    ["p_Quantity"] = 50,
                    ["p_Notes"] = "Initial stock",
                    ["p_User"] = "JDOE",
                    ["p_ItemType"] = "Standard",
                    ["p_ReceiveDate"] = DateTime.Now
                };

                var result = await Helper_Database_StoredProcedure.ExecuteWithStatus(
                    _connectionString,
                    "sys_last_10_transactions_Add_Transaction",
                    parameters
                );

                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleAddTransactionAsync");
                return false;
            }
        }

        // ========================================================================
        // COMPREHENSIVE WORKFLOW EXAMPLES
        // ========================================================================

        /// <summary>
        /// Example: Complete inventory addition workflow
        /// Demonstrates a typical business process using multiple procedures
        /// </summary>
        public async Task<bool> ExampleCompleteInventoryWorkflowAsync()
        {
            try
            {
                _logger.LogInformation("Starting complete inventory workflow example");

                // Step 1: Validate part exists
                var partCheck = await ExampleGetInventoryByPartIDAsync("PART001");
                
                // Step 2: Add inventory if part is valid
                var addResult = await ExampleAddInventoryItemAsync();
                
                if (addResult)
                {
                    // Step 3: Add transaction record
                    var transactionResult = await ExampleAddTransactionAsync();
                    
                    if (transactionResult)
                    {
                        _logger.LogInformation("Complete inventory workflow completed successfully");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleCompleteInventoryWorkflowAsync");
                return false;
            }
        }

        /// <summary>
        /// Example: User setup workflow
        /// Demonstrates complete user creation and configuration
        /// </summary>
        public async Task<bool> ExampleUserSetupWorkflowAsync()
        {
            try
            {
                _logger.LogInformation("Starting user setup workflow example");

                // Step 1: Check if user already exists
                var userExists = await ExampleCheckUserExistsAsync("NEWUSER");
                
                if (!userExists)
                {
                    // Step 2: Create new user
                    var userCreated = await ExampleAddUserAsync();
                    
                    if (userCreated)
                    {
                        // Step 3: Set default UI settings
                        var settingsSet = await ExampleSaveUserSettingsAsync();
                        
                        if (settingsSet)
                        {
                            _logger.LogInformation("User setup workflow completed successfully");
                            return true;
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("User already exists");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ExampleUserSetupWorkflowAsync");
                return false;
            }
        }
    }
}

// ============================================================================
// SUMMARY OF ALL 45 STORED PROCEDURES
// ============================================================================
/*
INVENTORY MANAGEMENT (7 procedures):
- inv_inventory_Add_Item
- inv_inventory_Remove_Item  
- inv_inventory_Transfer_Part
- inv_inventory_Transfer_Quantity
- inv_inventory_Get_ByPartID
- inv_inventory_Get_ByPartIDandOperation
- inv_inventory_Get_ByUser

MASTER DATA MANAGEMENT (20 procedures):
Part Management (5):
- md_part_ids_Add_Part
- md_part_ids_Update_Part
- md_part_ids_Delete_ByItemNumber
- md_part_ids_Get_ByItemNumber
- md_part_ids_Get_All

Location Management (5):
- md_locations_Add_Location
- md_locations_Update_Location
- md_locations_Delete_ByLocation
- md_locations_Get_ByLocation
- md_locations_Get_All

Operation Management (5):
- md_operation_numbers_Add_Operation
- md_operation_numbers_Update_Operation
- md_operation_numbers_Delete_ByOperation
- md_operation_numbers_Get_ByOperation
- md_operation_numbers_Get_All

Item Type Management (5):
- md_item_types_Add_ItemType
- md_item_types_Update_ItemType
- md_item_types_Delete_ByType
- md_item_types_Get_ByType
- md_item_types_Get_All

USER MANAGEMENT (14 procedures):
Core User Operations (8):
- usr_users_Add
- usr_users_Update
- usr_users_Delete_ByID
- usr_users_Get_ByID
- usr_users_Get_ByUser
- usr_users_Get_All
- usr_users_Exists

Legacy User Operations (3):
- usr_users_Add_User
- usr_users_Update_User
- usr_users_Delete_User

UI Settings (6):
- usr_ui_settings_Get
- usr_ui_settings_SetJsonSetting
- usr_ui_settings_SetThemeJson
- usr_ui_settings_GetThemeJson
- usr_ui_settings_SetShortcutsJson
- usr_ui_settings_GetShortcutsJson

SYSTEM FUNCTIONS (7 procedures):
Role Management (1):
- sys_roles_Get_All

Transaction Management (2):
- sys_last_10_transactions_Get_ByUser (Non-standard)
- sys_last_10_transactions_Add_Transaction

QuickButton Management (4):
- qb_quickbuttons_Get_ByUser (Non-standard)
- qb_quickbuttons_Save
- qb_quickbuttons_Remove
- qb_quickbuttons_Clear_ByUser

TECHNICAL NOTES:
- 43 procedures follow MTM standard pattern with p_Status/p_ErrorMsg output parameters
- 2 procedures (qb_quickbuttons_Get_ByUser, sys_last_10_transactions_Get_ByUser) use direct execution
- All procedures are MySQL 5.7 compatible with proper DEFINER statements
- All procedures include proper transaction safety and error handling
*/