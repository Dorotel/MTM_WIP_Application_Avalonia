using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Database service interface for MTM operations.
/// </summary>
public interface IDatabaseService
{
    // Basic database operations
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<bool> TestConnectionAsync();
    Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10);
    string GetConnectionString();
    
    // Inventory Operations - using Helper_Database_StoredProcedure pattern
    Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<DataTable> GetInventoryByPartIdAsync(string partId);
    Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation);
    Task<DataTable> GetInventoryByUserAsync(string user);
    Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation);
    Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user);
    
    // Master Data Operations - Parts
    Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType);
    Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType);
    Task<bool> DeletePartAsync(string partId);
    Task<DataTable> GetPartByIdAsync(string partId);
    
    // Master Data Operations - Operations
    Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy);
    Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy);
    Task<bool> DeleteOperationAsync(string operation);
    
    // Master Data Operations - Locations
    Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building);
    Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building);
    Task<bool> DeleteLocationAsync(string location);
    
    // Master Data Operations - Item Types
    Task<DataTable> GetAllItemTypesAsync();
    Task<StoredProcedureResult> AddItemTypeAsync(string itemType, string issuedBy);
    Task<StoredProcedureResult> UpdateItemTypeAsync(int id, string itemType, string issuedBy);
    Task<bool> DeleteItemTypeAsync(string itemType);
    
    // Additional Master Data Operations
    Task<DataTable> GetAllLocationsAsync();
    Task<DataTable> GetAllOperationsAsync();
    Task<DataTable> GetAllPartIDsAsync();
    Task<DataTable> GetAllRolesAsync();
    
    // User Management
    Task<DataTable> GetAllUsersAsync();
    Task<DataTable> GetUserAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task<StoredProcedureResult> AddUserAsync(MTM_Shared_Logic.Models.User userInfo);
    Task<StoredProcedureResult> UpdateUserAsync(MTM_Shared_Logic.Models.User userInfo);
    Task<bool> DeleteUserAsync(string username);
    
    // Additional User Management overloads for ViewModels
    Task<StoredProcedureResult> AddUserAsync(string username, string firstName, string lastName, string email, string role, string issuedBy);
    Task<StoredProcedureResult> UpdateUserAsync(int id, string username, string firstName, string lastName, string email, string role, bool isActive, string issuedBy);
    Task<bool> DeleteUserAsync(int id);
    
    // System Configuration
    Task<string> GetUserSettingsAsync(string userId);
    Task<bool> SaveUserSettingsAsync(string userId, string settingsJson);
    Task<bool> SaveThemeSettingsAsync(string userId, string themeJson);
    Task<string> GetUserShortcutsAsync(string userId);
    Task<bool> SaveUserShortcutsAsync(string userId, string shortcutsJson);
}

/// <summary>
/// Comprehensive database service for MTM WIP Application.
/// Provides centralized database access with error handling and logging.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly string _connectionString;

    public DatabaseService(ILogger<DatabaseService> logger, IConfigurationService configurationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionString = configurationService?.GetConnectionString() ?? throw new ArgumentNullException(nameof(configurationService));
        
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured");
        }

        _logger.LogInformation("DatabaseService initialized with connection string");
    }

    /// <summary>
    /// Executes a query and returns results as DataTable.
    /// </summary>
    public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        try
        {
            _logger.LogDebug("Executing query: {Query}", query);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }
            }

            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            _logger.LogDebug("Query executed successfully, returned {RowCount} rows", dataTable.Rows.Count);
            return dataTable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute query: {Query}", query);
            
            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteQueryAsync", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["Query"] = query, 
                    ["Parameters"] = parameters ?? new Dictionary<string, object>(),
                    ["Operation"] = "DatabaseQuery",
                    ["Service"] = "DatabaseService"
                });
            throw;
        }
    }

    /// <summary>
    /// Executes a query and returns a single value.
    /// </summary>
    public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
    {
        try
        {
            _logger.LogDebug("Executing scalar query: {Query}", query);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
    
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }
            }

            var result = await command.ExecuteScalarAsync();
            
            _logger.LogDebug("Scalar query executed successfully, result: {Result}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute scalar query: {Query}", query);
            
            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteScalarAsync", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["Query"] = query, 
                    ["Parameters"] = parameters ?? new Dictionary<string, object>(),
                    ["Operation"] = "DatabaseScalarQuery",
                    ["Service"] = "DatabaseService"
                });
            throw;
        }
    }

    /// <summary>
    /// Executes a non-query command and returns affected rows.
    /// </summary>
    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        try
        {
            _logger.LogDebug("Executing non-query: {Query}", query);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(query, connection);
            
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                }
            }

            var affectedRows = await command.ExecuteNonQueryAsync();
            
            _logger.LogDebug("Non-query executed successfully, affected rows: {AffectedRows}", affectedRows);
            return affectedRows;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute non-query: {Query}", query);
            
            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteNonQueryAsync", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["Query"] = query, 
                    ["Parameters"] = parameters ?? new Dictionary<string, object>(),
                    ["Operation"] = "DatabaseNonQuery",
                    ["Service"] = "DatabaseService"
                });
            throw;
        }
    }

    /// <summary>
    /// Tests database connectivity.
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            _logger.LogDebug("Testing database connection");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            
            _logger.LogInformation("Database connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            
            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "TestConnectionAsync", Environment.UserName,
                new Dictionary<string, object>
                {
                    ["Operation"] = "DatabaseConnectionTest",
                    ["Service"] = "DatabaseService",
                    ["ConnectionString"] = !string.IsNullOrEmpty(_connectionString) ? "Configured" : "Missing"
                });
            return false;
        }
    }

    /// <summary>
    /// Gets the connection string for database operations.
    /// </summary>
    public string GetConnectionString()
    {
        return _connectionString;
    }

    /// <summary>
    /// Gets the last 10 transactions for a user.
    /// This stored procedure doesn't follow MTM status pattern, so we use direct execution.
    /// </summary>
    public async Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10)
    {
        // Use current user if none specified, and ensure it's uppercase
        var currentUser = !string.IsNullOrEmpty(userId) ? userId.ToUpper() : Models.Model_AppVariables.CurrentUser;
        
        _logger.LogDebug("Getting last {Limit} transactions for user: {UserId}", limit, currentUser);

        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = currentUser,
                ["p_Limit"] = limit
            };

            return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                _connectionString,
                "sys_last_10_transactions_Get_ByUser",
                parameters
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get transactions for user: {UserId}", currentUser);
            
            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "GetLastTransactionsForUserAsync", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["UserId"] = userId ?? "", 
                    ["Limit"] = limit,
                    ["Operation"] = "GetLastTransactions",
                    ["Service"] = "DatabaseService",
                    ["StoredProcedure"] = "sys_last_10_transactions_Get_ByUser"
                });
            
            // Return empty DataTable rather than throwing, so the UI doesn't crash
            _logger.LogWarning("Returning empty DataTable due to stored procedure failure");
            return new DataTable();
        }
    }

    #region Inventory Operations

    /// <summary>
    /// Adds inventory item using inv_inventory_Add_Item stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        // Validate required parameters before building dictionary
        if (string.IsNullOrWhiteSpace(partId))
        {
            var errorResult = new StoredProcedureResult 
            { 
                Status = -1,
                Message = "PartID cannot be null or empty",
                Data = new DataTable()
            };
            _logger?.LogError("AddInventoryItem failed: PartID is null or empty");
            return errorResult;
        }

        if (string.IsNullOrWhiteSpace(user))
        {
            var errorResult = new StoredProcedureResult 
            { 
                Status = -1,
                Message = "User cannot be null or empty",
                Data = new DataTable()
            };
            _logger?.LogError("AddInventoryItem failed: User is null or empty for PartID: {PartId}", partId);
            return errorResult;
        }

        if (quantity <= 0)
        {
            var errorResult = new StoredProcedureResult 
            { 
                Status = -1,
                Message = "Quantity must be greater than 0",
                Data = new DataTable()
            };
            _logger?.LogError("AddInventoryItem failed: Invalid quantity {Quantity} for PartID: {PartId}", quantity, partId);
            return errorResult;
        }

        _logger?.LogDebug("Adding inventory item: PartID {PartId}, Location {Location}, Operation {Operation}, Quantity {Quantity}, User {User}, BatchNumber {BatchNumber}", 
            partId, location, operation, quantity, user, batchNumber);

        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = partId,
            ["p_Location"] = location ?? string.Empty,
            ["p_Operation"] = operation ?? string.Empty,
            ["p_Quantity"] = quantity,
            ["p_ItemType"] = itemType ?? "Standard",
            ["p_User"] = user,
            ["p_Notes"] = !string.IsNullOrWhiteSpace(notes) ? notes : DBNull.Value
        };

        _logger?.LogDebug("Calling inv_inventory_Add_Item with parameters: {Parameters}", 
            string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));
        _logger?.LogInformation("Adding inventory item with BatchNumber generated by stored procedure: PartID={PartId}, User={User}", partId, user);

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Add_Item",
            parameters
        );

        _logger?.LogDebug("inv_inventory_Add_Item returned: Status={Status}, Message='{Message}'", 
            result.Status, result.Message);

        // REMOVED: QuickButton logic moved to dedicated QuickButtons service
        // The QuickButtons service will handle position management with proper shifting logic
        // This prevents duplicate/conflicting QuickButton saves

        return result;
    }

    /// <summary>
    /// Gets the next available position for a user's QuickButtons (1-10, cycling back to 1)
    /// </summary>
    private async Task<int> GetNextQuickButtonPosition(string user)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = user  // Fixed: Use p_User to match stored procedure
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "qb_quickbuttons_Get_ByUser",
                parameters
            );

            if (result.Status == 0 && result.Data != null && result.Data.Rows.Count > 0)
            {
                // Find the highest position currently used
                int maxPosition = 0;
                foreach (DataRow row in result.Data.Rows)
                {
                    if (row["Position"] != DBNull.Value && int.TryParse(row["Position"].ToString(), out int position))
                    {
                        maxPosition = Math.Max(maxPosition, position);
                    }
                }
                
                // Return next position (1-10, cycling back to 1)
                return maxPosition >= 10 ? 1 : maxPosition + 1;
            }
            
            // No existing buttons, start at position 1
            return 1;
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error getting next QuickButton position for user: {User}, defaulting to position 1", user);
            return 1; // Safe fallback
        }
    }

    /// <summary>
    /// Gets inventory by part ID using inv_inventory_Get_ByPartID stored procedure.
    /// </summary>
    public async Task<DataTable> GetInventoryByPartIdAsync(string partId)
    {
        try
        {
            _logger.LogDebug("GetInventoryByPartIdAsync called with PartId={PartId}", partId);
            
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId ?? string.Empty
            };

            _logger.LogDebug("Calling stored procedure inv_inventory_Get_ByPartID with parameters: {Parameters}", 
                string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Get_ByPartID",
                parameters
            );

            _logger.LogDebug("Stored procedure result: IsSuccess={IsSuccess}, RowCount={RowCount}, Message={Message}", 
                result.IsSuccess, result.Data?.Rows.Count ?? 0, result.Message);

            if (!result.IsSuccess)
            {
                _logger.LogError("Stored procedure inv_inventory_Get_ByPartID failed: Status={Status}, Message={Message}", 
                    result.Status, result.Message);
            }

            return result.Data ?? new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetInventoryByPartIdAsync for PartId={PartId}", partId);
            throw;
        }
    }

    /// <summary>
    /// Gets inventory by part ID and operation using inv_inventory_Get_ByPartIDandOperation stored procedure.
    /// </summary>
    public async Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation)
    {
        try
        {
            _logger.LogDebug("GetInventoryByPartAndOperationAsync called with PartId={PartId}, Operation={Operation}", partId, operation);
            
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = partId ?? string.Empty,
                ["p_Operation"] = operation ?? string.Empty
            };

            _logger.LogDebug("Calling stored procedure inv_inventory_Get_ByPartIDandOperation with parameters: {Parameters}", 
                string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}")));

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Get_ByPartIDandOperation",
                parameters
            );

            _logger.LogDebug("Stored procedure result: IsSuccess={IsSuccess}, RowCount={RowCount}, Message={Message}", 
                result.IsSuccess, result.Data?.Rows.Count ?? 0, result.Message);

            if (!result.IsSuccess)
            {
                _logger.LogError("Stored procedure inv_inventory_Get_ByPartIDandOperation failed: Status={Status}, Message={Message}", 
                    result.Status, result.Message);
            }

            return result.Data ?? new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception in GetInventoryByPartAndOperationAsync for PartId={PartId}, Operation={Operation}", partId, operation);
            throw;
        }
    }

    /// <summary>
    /// Gets inventory by user using inv_inventory_Get_ByUser stored procedure.
    /// </summary>
    public async Task<DataTable> GetInventoryByUserAsync(string user)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = user
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Get_ByUser",
            parameters
        );

        return result.Data;
    }

    /// <summary>
    /// Removes inventory item using inv_inventory_Remove_Item stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = partId,
            ["p_Location"] = location,
            ["p_Operation"] = operation,
            ["p_Quantity"] = quantity,
            ["p_ItemType"] = itemType,
            ["p_User"] = user,
            ["p_BatchNumber"] = !string.IsNullOrWhiteSpace(batchNumber) ? batchNumber : DBNull.Value,
            ["p_Notes"] = !string.IsNullOrWhiteSpace(notes) ? notes : DBNull.Value
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Remove_Item",
            parameters
        );
    }

    /// <summary>
    /// Transfers entire part to new location using inv_inventory_Transfer_Part stored procedure.
    /// </summary>
    public async Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_BatchNumber"] = batchNumber,
            ["p_PartID"] = partId,
            ["p_Operation"] = operation,
            ["p_NewLocation"] = newLocation
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Transfer_Part",
            parameters
        );

        return result.IsSuccess;
    }

    /// <summary>
    /// Transfers partial quantity to new location using inv_inventory_Transfer_Quantity stored procedure.
    /// </summary>
    public async Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_BatchNumber"] = batchNumber,
            ["p_PartID"] = partId,
            ["p_Operation"] = operation,
            ["p_TransferQuantity"] = transferQuantity,
            ["p_OriginalQuantity"] = originalQuantity,
            ["p_NewLocation"] = newLocation,
            ["p_User"] = user
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Transfer_Quantity",
            parameters
        );

        return result.IsSuccess;
    }

    #endregion

    #region Master Data Operations - Parts

    /// <summary>
    /// Adds a new part using md_part_ids_Add_Part stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_PartID"] = partId,
            ["p_Customer"] = customer,
            ["p_Description"] = description,
            ["p_IssuedBy"] = issuedBy,
            ["p_ItemType"] = itemType
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_part_ids_Add_Part",
            parameters
        );
    }

    /// <summary>
    /// Updates a part using md_part_ids_Update_Part stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ID"] = id,
            ["p_PartID"] = partId,
            ["p_Customer"] = customer,
            ["p_Description"] = description,
            ["p_IssuedBy"] = issuedBy,
            ["p_ItemType"] = itemType
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_part_ids_Update_Part",
            parameters
        );
    }

    /// <summary>
    /// Deletes a part using md_part_ids_Delete_ByItemNumber stored procedure.
    /// </summary>
    public async Task<bool> DeletePartAsync(string partId)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ItemNumber"] = partId
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_part_ids_Delete_ByItemNumber",
            parameters
        );

        return result.IsSuccess;
    }

    /// <summary>
    /// Gets a specific part using md_part_ids_Get_ByItemNumber stored procedure.
    /// </summary>
    public async Task<DataTable> GetPartByIdAsync(string partId)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ItemNumber"] = partId
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_part_ids_Get_ByItemNumber",
            parameters
        );

        return result.Data;
    }

    #endregion

    #region Master Data Operations - Operations

    /// <summary>
    /// Adds a new operation using md_operation_numbers_Add_Operation stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Operation"] = operation,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_operation_numbers_Add_Operation",
            parameters
        );
    }

    /// <summary>
    /// Updates an operation using md_operation_numbers_Update_Operation stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Operation"] = operation,
            ["p_NewOperation"] = newOperation,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_operation_numbers_Update_Operation",
            parameters
        );
    }

    /// <summary>
    /// Deletes an operation using md_operation_numbers_Delete_ByOperation stored procedure.
    /// </summary>
    public async Task<bool> DeleteOperationAsync(string operation)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Operation"] = operation
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_operation_numbers_Delete_ByOperation",
            parameters
        );

        return result.IsSuccess;
    }

    #endregion

    #region Master Data Operations - Locations

    /// <summary>
    /// Adds a new location using md_locations_Add_Location stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Location"] = location,
            ["p_IssuedBy"] = issuedBy,
            ["p_Building"] = building
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_locations_Add_Location",
            parameters
        );
    }

    /// <summary>
    /// Updates a location using md_locations_Update_Location stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_OldLocation"] = oldLocation,
            ["p_Location"] = location,
            ["p_IssuedBy"] = issuedBy,
            ["p_Building"] = building
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_locations_Update_Location",
            parameters
        );
    }

    /// <summary>
    /// Deletes a location using md_locations_Delete_ByLocation stored procedure.
    /// </summary>
    public async Task<bool> DeleteLocationAsync(string location)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Location"] = location
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_locations_Delete_ByLocation",
            parameters
        );

        return result.IsSuccess;
    }

    #endregion

    #region Master Data Operations - Item Types

    /// <summary>
    /// Gets all item types using md_item_types_Get_All stored procedure.
    /// </summary>
    public async Task<DataTable> GetAllItemTypesAsync()
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_item_types_Get_All",
            new Dictionary<string, object>()
        );

        return result.Data;
    }

    /// <summary>
    /// Adds a new item type using md_item_types_Add_ItemType stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddItemTypeAsync(string itemType, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ItemType"] = itemType,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_item_types_Add_ItemType",
            parameters
        );
    }

    /// <summary>
    /// Updates an item type using md_item_types_Update_ItemType stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> UpdateItemTypeAsync(int id, string itemType, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ID"] = id,
            ["p_ItemType"] = itemType,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_item_types_Update_ItemType",
            parameters
        );
    }

    /// <summary>
    /// Deletes an item type using md_item_types_Delete_ByType stored procedure.
    /// </summary>
    public async Task<bool> DeleteItemTypeAsync(string itemType)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ItemType"] = itemType
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "md_item_types_Delete_ByType",
            parameters
        );

        return result.IsSuccess;
    }

    /// <summary>
    /// Gets all locations using md_locations_Get_All stored procedure.
    /// This procedure doesn't follow MTM status pattern, so use direct execution.
    /// </summary>
    public async Task<DataTable> GetAllLocationsAsync()
    {
        return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
            _connectionString,
            "md_locations_Get_All",
            new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Gets all operations using md_operation_numbers_Get_All stored procedure.
    /// This procedure doesn't follow MTM status pattern, so use direct execution.
    /// </summary>
    public async Task<DataTable> GetAllOperationsAsync()
    {
        return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
            _connectionString,
            "md_operation_numbers_Get_All",
            new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Gets all Part IDs using md_part_ids_Get_All stored procedure.
    /// This procedure doesn't follow MTM status pattern, so use direct execution.
    /// </summary>
    public async Task<DataTable> GetAllPartIDsAsync()
    {
        return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
            _connectionString,
            "md_part_ids_Get_All",
            new Dictionary<string, object>()
        );
    }

    /// <summary>
    /// Gets all roles using sys_roles_Get_All stored procedure.
    /// </summary>
    public async Task<DataTable> GetAllRolesAsync()
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "sys_roles_Get_All",
            new Dictionary<string, object>()
        );

        return result.Data;
    }

    #endregion

    #region User Management

    /// <summary>
    /// Gets all users using usr_users_Get_All stored procedure.
    /// </summary>
    public async Task<DataTable> GetAllUsersAsync()
    {
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Get_All",
            new Dictionary<string, object>()
        );

        return result.Data;
    }

    /// <summary>
    /// Gets a specific user using usr_users_Get_ByUser stored procedure.
    /// </summary>
    public async Task<DataTable> GetUserAsync(string username)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Username"] = username
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Get_ByUser",
            parameters
        );

        return result.Data;
    }

    /// <summary>
    /// Checks if user exists using usr_users_Exists stored procedure.
    /// </summary>
    public async Task<bool> UserExistsAsync(string username)
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

    /// <summary>
    /// Adds a new user using usr_users_Add_User stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> AddUserAsync(MTM_Shared_Logic.Models.User userInfo)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = userInfo.User_Name,
            ["p_FullName"] = (object?)userInfo.FullName ?? DBNull.Value,
            ["p_Shift"] = userInfo.Shift,
            ["p_VitsUser"] = userInfo.VitsUser,
            ["p_Pin"] = (object?)userInfo.Pin ?? DBNull.Value,
            ["p_LastShownVersion"] = userInfo.LastShownVersion,
            ["p_HideChangeLog"] = userInfo.HideChangeLog,
            ["p_ThemeName"] = userInfo.Theme_Name,
            ["p_ThemeFontSize"] = userInfo.Theme_FontSize,
            ["p_VisualUserName"] = userInfo.VisualUserName,
            ["p_VisualPassword"] = userInfo.VisualPassword,
            ["p_WipServerAddress"] = userInfo.WipServerAddress,
            ["p_WipServerPort"] = userInfo.WipServerPort,
            ["p_WipDatabase"] = userInfo.WIPDatabase
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Add_User",
            parameters
        );
    }

    /// <summary>
    /// Updates a user using usr_users_Update_User stored procedure.
    /// </summary>
    public async Task<StoredProcedureResult> UpdateUserAsync(MTM_Shared_Logic.Models.User userInfo)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = userInfo.User_Name,
            ["p_FullName"] = (object?)userInfo.FullName ?? DBNull.Value,
            ["p_Shift"] = userInfo.Shift,
            ["p_VitsUser"] = userInfo.VitsUser,
            ["p_Pin"] = (object?)userInfo.Pin ?? DBNull.Value,
            ["p_LastShownVersion"] = userInfo.LastShownVersion,
            ["p_HideChangeLog"] = userInfo.HideChangeLog,
            ["p_ThemeName"] = userInfo.Theme_Name,
            ["p_ThemeFontSize"] = userInfo.Theme_FontSize,
            ["p_VisualUserName"] = userInfo.VisualUserName,
            ["p_VisualPassword"] = userInfo.VisualPassword,
            ["p_WipServerAddress"] = userInfo.WipServerAddress,
            ["p_WipServerPort"] = userInfo.WipServerPort,
            ["p_WipDatabase"] = userInfo.WIPDatabase
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Update_User",
            parameters
        );
    }

    /// <summary>
    /// Deletes a user using usr_users_Delete_User stored procedure.
    /// </summary>
    public async Task<bool> DeleteUserAsync(string username)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Username"] = username
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Delete_User",
            parameters
        );

        return result.IsSuccess;
    }

    /// <summary>
    /// Adds a new user using usr_users_Add stored procedure.
    /// Simplified overload for ViewModels.
    /// </summary>
    public async Task<StoredProcedureResult> AddUserAsync(string username, string firstName, string lastName, string email, string role, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_Username"] = username,
            ["p_FirstName"] = firstName,
            ["p_LastName"] = lastName,
            ["p_Email"] = email,
            ["p_Role"] = role,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Add",
            parameters
        );
    }

    /// <summary>
    /// Updates an existing user using usr_users_Update stored procedure.
    /// Simplified overload for ViewModels.
    /// </summary>
    public async Task<StoredProcedureResult> UpdateUserAsync(int id, string username, string firstName, string lastName, string email, string role, bool isActive, string issuedBy)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ID"] = id,
            ["p_Username"] = username,
            ["p_FirstName"] = firstName,
            ["p_LastName"] = lastName,
            ["p_Email"] = email,
            ["p_Role"] = role,
            ["p_IsActive"] = isActive,
            ["p_IssuedBy"] = issuedBy
        };

        return await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Update",
            parameters
        );
    }

    /// <summary>
    /// Deletes a user by ID using usr_users_Delete_ByID stored procedure.
    /// Simplified overload for ViewModels.
    /// </summary>
    public async Task<bool> DeleteUserAsync(int id)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_ID"] = id
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_users_Delete_ByID",
            parameters
        );

        return result.IsSuccess;
    }

    #endregion

    #region System Configuration

    /// <summary>
    /// Gets user UI settings from usr_ui_settings table by UserID.
    /// </summary>
    public async Task<string> GetUserSettingsAsync(string userId)
    {
        try
        {
            _logger.LogDebug("Getting user settings for user: {UserId}", userId);

            // Query the usr_ui_settings table directly for the user
            var query = "SELECT SettingsJson FROM usr_ui_settings WHERE UserID = @userId";
            var parameters = new Dictionary<string, object>
            {
                ["userId"] = userId
            };

            var result = await ExecuteScalarAsync(query, parameters);
            
            var settingsJson = result?.ToString() ?? string.Empty;
            
            if (string.IsNullOrEmpty(settingsJson))
            {
                _logger.LogDebug("No settings found for user {UserId}", userId);
            }
            else
            {
                _logger.LogDebug("Retrieved settings for user {UserId}: {SettingsJson}", userId, settingsJson);
            }

            return settingsJson;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings for user {UserId}", userId);
            return string.Empty;
        }
    }

    /// <summary>
    /// Saves user UI settings using usr_ui_settings_SetJsonSetting stored procedure.
    /// </summary>
    public async Task<bool> SaveUserSettingsAsync(string userId, string settingsJson)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = userId,
            ["p_SettingsJson"] = settingsJson
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_ui_settings_SetJsonSetting",
            parameters
        );

        return result.IsSuccess;
    }

    /// <summary>
    /// Saves user theme settings using usr_ui_settings_SetThemeJson stored procedure.
    /// </summary>
    public async Task<bool> SaveThemeSettingsAsync(string userId, string themeJson)
    {
        try
        {
            var parameters = new Dictionary<string, object>
            {
                ["p_User"] = userId,
                ["p_ThemeJson"] = themeJson
            };

            _logger.LogDebug("Calling usr_ui_settings_SetThemeJson for user {UserId} with theme JSON: {ThemeJson}", 
                userId, themeJson);

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "usr_ui_settings_SetThemeJson",
                parameters
            );

            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully saved theme settings for user {UserId}", userId);
                return true;
            }
            else
            {
                _logger.LogWarning("Failed to save theme settings for user {UserId}: Status={Status}, Message={Message}", 
                    userId, result.Status, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving theme settings for user {UserId}: {ThemeJson}", userId, themeJson);
            return false;
        }
    }

    /// <summary>
    /// Gets user keyboard shortcuts using usr_ui_settings_GetShortcutsJson stored procedure.
    /// </summary>
    public async Task<string> GetUserShortcutsAsync(string userId)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = userId
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_ui_settings_GetShortcutsJson",
            parameters
        );

        if (result.IsSuccess && result.Data.Rows.Count > 0)
        {
            return result.Data.Rows[0]["ShortcutsJson"]?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    /// <summary>
    /// Saves user keyboard shortcuts using usr_ui_settings_SetShortcutsJson stored procedure.
    /// </summary>
    public async Task<bool> SaveUserShortcutsAsync(string userId, string shortcutsJson)
    {
        var parameters = new Dictionary<string, object>
        {
            ["p_User"] = userId,
            ["p_ShortcutsJson"] = shortcutsJson
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "usr_ui_settings_SetShortcutsJson",
            parameters
        );

        return result.IsSuccess;
    }

    #endregion
}

/// <summary>
/// Helper class for stored procedure execution with status return.
/// Maintains compatibility with existing Helper_Database_StoredProcedure usage.
/// </summary>
public static class Helper_Database_StoredProcedure
{
    private static ILogger? _logger;

    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Executes a stored procedure with status return - MTM standard pattern.
    /// Use this for stored procedures that follow the MTM standard (have @p_Status and @p_ErrorMsg output parameters).
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
        string connectionString, 
        string procedureName, 
        Dictionary<string, object> parameters)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var isQuickButtonProcedure = IsQuickButtonProcedure(procedureName);
        
        try
        {
            _logger?.LogDebug("Executing stored procedure: {ProcedureName}", procedureName);
            
            // Enhanced debugging for QuickButton procedures
            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Starting execution of {ProcedureName}", procedureName);
                LogQuickButtonParameters(procedureName, parameters);
            }

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add input parameters
            foreach (var param in parameters)
            {
                var parameterValue = param.Value ?? DBNull.Value;
                command.Parameters.AddWithValue($"@{param.Key}", parameterValue);
                
                if (isQuickButtonProcedure)
                {
                    _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Added parameter @{ParameterName} = {ParameterValue} (Type: {ParameterType})", 
                        param.Key, parameterValue, parameterValue?.GetType().Name ?? "NULL");
                }
            }

            // Add output parameters for status and message (MTM standard naming)
            command.Parameters.Add("@p_Status", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@p_ErrorMsg", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Executing stored procedure with {ParameterCount} input parameters", parameters.Count);
            }

            // Execute and get data
            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            // Get output parameter values
            var status = command.Parameters["@p_Status"].Value as int? ?? -1;
            var message = command.Parameters["@p_ErrorMsg"].Value as string ?? "Unknown error";

            var result = new StoredProcedureResult
            {
                Status = status,
                Message = message,
                Data = dataTable
            };

            stopwatch.Stop();

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Procedure {ProcedureName} completed - Status: {Status}, Message: '{Message}', Rows: {RowCount}, Duration: {Duration}ms", 
                    procedureName, status, message, dataTable.Rows.Count, stopwatch.ElapsedMilliseconds);
                
                LogQuickButtonResult(procedureName, result, parameters);
                
                // Log column information for data procedures
                if (dataTable.Rows.Count > 0 && dataTable.Columns.Count > 0)
                {
                    var columnNames = string.Join(", ", dataTable.Columns.OfType<DataColumn>().Select(c => $"{c.ColumnName}({c.DataType.Name})"));
                    _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Result columns: {Columns}", columnNames);
                    
                    // Log first row data for debugging
                    if (dataTable.Rows.Count > 0)
                    {
                        var firstRow = dataTable.Rows[0];
                        var rowData = string.Join(", ", dataTable.Columns.OfType<DataColumn>()
                            .Select(c => $"{c.ColumnName}={firstRow[c.ColumnName] ?? "NULL"}"));
                        _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: First row data: {RowData}", rowData);
                    }
                }
            }

            _logger?.LogDebug("Stored procedure executed: {ProcedureName}, Status: {Status}, Rows: {RowCount}", 
                procedureName, status, dataTable.Rows.Count);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            if (isQuickButtonProcedure)
            {
                _logger?.LogError("🔍 QUICKBUTTON DEBUG: FAILED execution of {ProcedureName} after {Duration}ms - Error: {ErrorMessage}", 
                    procedureName, stopwatch.ElapsedMilliseconds, ex.Message);
                LogQuickButtonError(procedureName, parameters, ex);
            }

            _logger?.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
            
            // Use fully qualified namespace for ErrorHandling service with enhanced business context
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteDataTableWithStatus", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["ProcedureName"] = procedureName, 
                    ["Parameters"] = parameters,
                    ["Operation"] = "StoredProcedureExecution",
                    ["Service"] = "Helper_Database_StoredProcedure",
                    ["IsQuickButtonProcedure"] = isQuickButtonProcedure,
                    ["ExecutionTimeMs"] = stopwatch.ElapsedMilliseconds,
                    // Extract MTM business context from parameters if available
                    ["PartId"] = parameters.GetValueOrDefault("p_PartID", parameters.GetValueOrDefault("PartID", "")),
                    ["Quantity"] = parameters.GetValueOrDefault("p_Quantity", parameters.GetValueOrDefault("Quantity", "")),
                    ["TransactionType"] = parameters.GetValueOrDefault("p_TransactionType", parameters.GetValueOrDefault("TransactionType", "")),
                    ["Location"] = parameters.GetValueOrDefault("p_Location", parameters.GetValueOrDefault("Location", ""))
                });

            return new StoredProcedureResult
            {
                Status = -1,
                Message = $"Error executing stored procedure: {ex.Message}",
                Data = new DataTable()
            };
        }
    }

    /// <summary>
    /// Executes a stored procedure without expecting a data result.
    /// Use this for stored procedures that follow the MTM standard (have @p_Status and @p_ErrorMsg output parameters).
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteWithStatus(
        string connectionString, 
        string procedureName, 
        Dictionary<string, object> parameters)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var isQuickButtonProcedure = IsQuickButtonProcedure(procedureName);
        
        try
        {
            _logger?.LogDebug("Executing stored procedure (no data): {ProcedureName}", procedureName);
            
            // Enhanced debugging for QuickButton procedures
            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Starting non-query execution of {ProcedureName}", procedureName);
                LogQuickButtonParameters(procedureName, parameters);
            }

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add input parameters
            foreach (var param in parameters)
            {
                var parameterValue = param.Value ?? DBNull.Value;
                command.Parameters.AddWithValue($"@{param.Key}", parameterValue);
                
                if (isQuickButtonProcedure)
                {
                    _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Added parameter @{ParameterName} = {ParameterValue} (Type: {ParameterType})", 
                        param.Key, parameterValue, parameterValue?.GetType().Name ?? "NULL");
                }
            }

            // Add output parameters (MTM standard naming)
            command.Parameters.Add("@p_Status", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@p_ErrorMsg", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Executing non-query with {ParameterCount} input parameters", parameters.Count);
            }

            await command.ExecuteNonQueryAsync();

            var status = command.Parameters["@p_Status"].Value as int? ?? -1;
            var message = command.Parameters["@p_ErrorMsg"].Value as string ?? "Unknown error";

            var result = new StoredProcedureResult
            {
                Status = status,
                Message = message,
                Data = new DataTable()
            };

            stopwatch.Stop();

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Non-query {ProcedureName} completed - Status: {Status}, Message: '{Message}', Duration: {Duration}ms", 
                    procedureName, status, message, stopwatch.ElapsedMilliseconds);
                
                LogQuickButtonResult(procedureName, result, parameters);
            }

            _logger?.LogDebug("Stored procedure executed: {ProcedureName}, Status: {Status}", 
                procedureName, status);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            if (isQuickButtonProcedure)
            {
                _logger?.LogError("🔍 QUICKBUTTON DEBUG: FAILED non-query execution of {ProcedureName} after {Duration}ms - Error: {ErrorMessage}", 
                    procedureName, stopwatch.ElapsedMilliseconds, ex.Message);
                LogQuickButtonError(procedureName, parameters, ex);
            }

            _logger?.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
            
            // Use fully qualified namespace for ErrorHandling service with enhanced business context
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteWithStatus", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["ProcedureName"] = procedureName, 
                    ["Parameters"] = parameters,
                    ["Operation"] = "StoredProcedureExecution",
                    ["Service"] = "Helper_Database_StoredProcedure",
                    ["IsQuickButtonProcedure"] = isQuickButtonProcedure,
                    ["ExecutionTimeMs"] = stopwatch.ElapsedMilliseconds,
                    // Extract MTM business context from parameters if available
                    ["PartId"] = parameters.GetValueOrDefault("p_PartID", parameters.GetValueOrDefault("PartID", "")),
                    ["Quantity"] = parameters.GetValueOrDefault("p_Quantity", parameters.GetValueOrDefault("Quantity", "")),
                    ["TransactionType"] = parameters.GetValueOrDefault("p_TransactionType", parameters.GetValueOrDefault("TransactionType", "")),
                    ["Location"] = parameters.GetValueOrDefault("p_Location", parameters.GetValueOrDefault("Location", ""))
                });

            return new StoredProcedureResult
            {
                Status = -1,
                Message = $"Error executing stored procedure: {ex.Message}",
                Data = new DataTable()
            };
        }
    }

    /// <summary>
    /// Executes a stored procedure that doesn't follow MTM standard pattern (no status/message outputs).
    /// Use this for legacy or system stored procedures that don't have @status and @message output parameters.
    /// </summary>
    public static async Task<DataTable> ExecuteDataTableDirect(
        string connectionString,
        string procedureName,
        Dictionary<string, object> parameters)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var isQuickButtonProcedure = IsQuickButtonProcedure(procedureName);
        
        try
        {
            _logger?.LogDebug("Executing direct stored procedure: {ProcedureName}", procedureName);
            
            // Enhanced debugging for QuickButton procedures
            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Starting direct execution of {ProcedureName}", procedureName);
                LogQuickButtonParameters(procedureName, parameters);
            }

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add input parameters only
            foreach (var param in parameters)
            {
                var parameterValue = param.Value ?? DBNull.Value;
                command.Parameters.AddWithValue($"@{param.Key}", parameterValue);
                
                if (isQuickButtonProcedure)
                {
                    _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Added parameter @{ParameterName} = {ParameterValue} (Type: {ParameterType})", 
                        param.Key, parameterValue, parameterValue?.GetType().Name ?? "NULL");
                }
            }

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Executing direct procedure with {ParameterCount} input parameters", parameters.Count);
            }

            // Execute and get data without expecting status/message outputs
            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            stopwatch.Stop();

            if (isQuickButtonProcedure)
            {
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Direct procedure {ProcedureName} completed - Rows: {RowCount}, Duration: {Duration}ms", 
                    procedureName, dataTable.Rows.Count, stopwatch.ElapsedMilliseconds);
                
                // Log column information for data procedures
                if (dataTable.Rows.Count > 0 && dataTable.Columns.Count > 0)
                {
                    var columnNames = string.Join(", ", dataTable.Columns.OfType<DataColumn>().Select(c => $"{c.ColumnName}({c.DataType.Name})"));
                    _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Direct result columns: {Columns}", columnNames);
                    
                    // Log first row data for debugging
                    if (dataTable.Rows.Count > 0)
                    {
                        var firstRow = dataTable.Rows[0];
                        var rowData = string.Join(", ", dataTable.Columns.OfType<DataColumn>()
                            .Select(c => $"{c.ColumnName}={firstRow[c.ColumnName] ?? "NULL"}"));
                        _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: Direct first row: {RowData}", rowData);
                    }
                }
            }

            _logger?.LogDebug("Direct stored procedure executed: {ProcedureName}, Rows: {RowCount}",
                procedureName, dataTable.Rows.Count);

            return dataTable;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            if (isQuickButtonProcedure)
            {
                _logger?.LogError("🔍 QUICKBUTTON DEBUG: FAILED direct execution of {ProcedureName} after {Duration}ms - Error: {ErrorMessage}", 
                    procedureName, stopwatch.ElapsedMilliseconds, ex.Message);
                LogQuickButtonError(procedureName, parameters, ex);
            }

            _logger?.LogError(ex, "Failed to execute direct stored procedure: {ProcedureName}", procedureName);

            // Use fully qualified namespace for ErrorHandling service
            await Services.ErrorHandling.HandleErrorAsync(ex, "ExecuteDataTableDirect", Environment.UserName,
                new Dictionary<string, object>
                {
                    ["ProcedureName"] = procedureName,
                    ["Parameters"] = parameters,
                    ["Operation"] = "DirectStoredProcedureExecution",
                    ["Service"] = "Helper_Database_StoredProcedure",
                    ["IsQuickButtonProcedure"] = isQuickButtonProcedure,
                    ["ExecutionTimeMs"] = stopwatch.ElapsedMilliseconds
                });

            throw;
        }
    }

    /// <summary>
    /// Determines if a stored procedure is related to QuickButton functionality
    /// </summary>
    private static bool IsQuickButtonProcedure(string procedureName)
    {
        var quickButtonProcedures = new[]
        {
            "qb_quickbuttons_Save",
            "qb_quickbuttons_Remove", 
            "qb_quickbuttons_Clear_ByUser",
            "qb_quickbuttons_Get_ByUser",
            "sys_last_10_transactions_Get_ByUser",
            "sys_last_10_transactions_Add_Transaction"
        };
        
        return quickButtonProcedures.Any(proc => procedureName.Equals(proc, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Logs QuickButton procedure parameters with business context
    /// </summary>
    private static void LogQuickButtonParameters(string procedureName, Dictionary<string, object> parameters)
    {
        _logger?.LogDebug("🔍 QUICKBUTTON DEBUG: {ProcedureName} input parameters:", procedureName);
        
        foreach (var param in parameters)
        {
            var value = param.Value ?? "NULL";
            var type = param.Value?.GetType().Name ?? "NULL";
            _logger?.LogDebug("🔍 QUICKBUTTON DEBUG:   @{ParameterName} = '{ParameterValue}' ({ParameterType})", 
                param.Key, value, type);
        }

        // Log business context based on procedure type
        switch (procedureName.ToLowerInvariant())
        {
            case "qb_quickbuttons_save":
                LogQuickButtonSaveContext(parameters);
                break;
            case "qb_quickbuttons_remove":
                LogQuickButtonRemoveContext(parameters);
                break;
            case "qb_quickbuttons_clear_byuser":
                LogQuickButtonClearContext(parameters);
                break;
            case "qb_quickbuttons_get_byuser":
                LogQuickButtonGetContext(parameters);
                break;
            case "sys_last_10_transactions_get_byuser":
                LogTransactionGetContext(parameters);
                break;
            case "sys_last_10_transactions_add_transaction":
                LogTransactionAddContext(parameters);
                break;
        }
    }

    /// <summary>
    /// Logs the result of QuickButton procedure execution
    /// </summary>
    private static void LogQuickButtonResult(string procedureName, StoredProcedureResult result, Dictionary<string, object> parameters)
    {
        var statusText = result.Status switch
        {
            0 => "SUCCESS",
            1 => "WARNING", 
            _ => "ERROR"
        };

        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: {ProcedureName} result - Status: {Status} ({StatusText}), Message: '{Message}'", 
            procedureName, result.Status, statusText, result.Message);

        // Log business-specific result context
        switch (procedureName.ToLowerInvariant())
        {
            case "qb_quickbuttons_save":
                var position = parameters.GetValueOrDefault("p_Position", "unknown");
                var partId = parameters.GetValueOrDefault("p_PartID", "unknown");
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Save operation for Part '{PartId}' at position {Position} - {StatusText}", 
                    partId, position, statusText);
                break;
                
            case "qb_quickbuttons_remove":
                var buttonId = parameters.GetValueOrDefault("p_ButtonID", "unknown");
                var userId = parameters.GetValueOrDefault("p_User", "unknown");
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Remove operation for ButtonID {ButtonId} by user '{UserId}' - {StatusText}", 
                    buttonId, userId, statusText);
                break;
                
            case "qb_quickbuttons_get_byuser":
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Get operation returned {RowCount} quick buttons - {StatusText}", 
                    result.Data.Rows.Count, statusText);
                break;
                
            case "sys_last_10_transactions_get_byuser":
                _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: Transaction get operation returned {RowCount} transactions - {StatusText}", 
                    result.Data.Rows.Count, statusText);
                break;
        }
    }

    /// <summary>
    /// Logs detailed error information for QuickButton procedures
    /// </summary>
    private static void LogQuickButtonError(string procedureName, Dictionary<string, object> parameters, Exception ex)
    {
        _logger?.LogError("🔍 QUICKBUTTON DEBUG: {ProcedureName} ERROR DETAILS:", procedureName);
        _logger?.LogError("🔍 QUICKBUTTON DEBUG:   Error Type: {ErrorType}", ex.GetType().Name);
        _logger?.LogError("🔍 QUICKBUTTON DEBUG:   Error Message: {ErrorMessage}", ex.Message);
        
        if (ex.InnerException != null)
        {
            _logger?.LogError("🔍 QUICKBUTTON DEBUG:   Inner Exception: {InnerException}", ex.InnerException.Message);
        }

        // Log parameter context that might be causing the error
        var criticalParams = new[] { "p_User", "p_PartID", "p_Position", "p_ButtonID", "p_Operation", "p_Quantity" };
        foreach (var paramName in criticalParams)
        {
            if (parameters.ContainsKey(paramName))
            {
                _logger?.LogError("🔍 QUICKBUTTON DEBUG:   Critical Parameter {ParamName}: {ParamValue}", 
                    paramName, parameters[paramName] ?? "NULL");
            }
        }

        // Log MySQL-specific error details if available
        if (ex is MySqlException mysqlEx)
        {
            _logger?.LogError("🔍 QUICKBUTTON DEBUG:   MySQL Error Number: {ErrorNumber}", mysqlEx.Number);
            _logger?.LogError("🔍 QUICKBUTTON DEBUG:   MySQL Error Code: {ErrorCode}", mysqlEx.ErrorCode);
        }
    }

    // Business context logging methods
    private static void LogQuickButtonSaveContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        var position = parameters.GetValueOrDefault("p_Position", "");
        var partId = parameters.GetValueOrDefault("p_PartID", "");
        var operation = parameters.GetValueOrDefault("p_Operation", "");
        var quantity = parameters.GetValueOrDefault("p_Quantity", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: SAVE CONTEXT - User '{UserId}' saving Part '{PartId}' with Operation '{Operation}' (Qty: {Quantity}) at position {Position}", 
            userId, partId, operation, quantity, position);
    }

    private static void LogQuickButtonRemoveContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        var buttonId = parameters.GetValueOrDefault("p_ButtonID", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: REMOVE CONTEXT - User '{UserId}' removing button at position {ButtonId}", 
            userId, buttonId);
    }

    private static void LogQuickButtonClearContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: CLEAR CONTEXT - User '{UserId}' clearing all quick buttons", userId);
    }

    private static void LogQuickButtonGetContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: GET CONTEXT - Retrieving quick buttons for user '{UserId}'", userId);
    }

    private static void LogTransactionGetContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        var limit = parameters.GetValueOrDefault("p_Limit", "10");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: TRANSACTION GET CONTEXT - Retrieving last {Limit} transactions for user '{UserId}'", 
            limit, userId);
    }

    private static void LogTransactionAddContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_User", "");
        var partId = parameters.GetValueOrDefault("p_PartID", "");
        var operation = parameters.GetValueOrDefault("p_Operation", "");
        var quantity = parameters.GetValueOrDefault("p_Quantity", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: TRANSACTION ADD CONTEXT - User '{UserId}' adding transaction: Part '{PartId}', Operation '{Operation}', Quantity {Quantity}", 
            userId, partId, operation, quantity);
    }
}

/// <summary>
/// Standard result structure for stored procedure execution.
/// </summary>
public class StoredProcedureResult
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DataTable Data { get; set; } = new DataTable();

    /// <summary>
    /// Indicates if the stored procedure executed successfully.
    /// IMPORTANT: MTM stored procedures use different status conventions:
    /// - Status = -1 for SUCCESS with data (confirmed by logs showing Status: -1 with 2908+ rows)
    /// - Status = 0 for SUCCESS (some procedures)
    /// - Status = 1 for SUCCESS (some procedures)
    /// - Status > 1 for various error codes
    /// For data retrieval procedures, we also check if data was returned regardless of status
    /// </summary>
    public bool IsSuccess => Status <= 1 || (Data != null && Data.Rows.Count > 0);
}
