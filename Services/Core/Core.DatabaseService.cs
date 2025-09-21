using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services.Core;

#region Database Services

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

    // Inventory Operations - using Services.Core.Helper_Database_StoredProcedure pattern
    Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<DataTable> GetInventoryByPartIdAsync(string partId);
    Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation);
    Task<DataTable> GetInventoryByUserAsync(string user);
    Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes);
    Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation);
    Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user);

    // Note editing operations
    Task<StoredProcedureResult> UpdateInventoryNotesAsync(int inventoryId, string partId, string batchNumber, string notes, string user);
    Task<DataTable> GetInventoryByIdAsync(int inventoryId);

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

    // Master Data Operations - Users
    Task<DataTable> GetAllUsersAsync();
    Task<DataTable> GetUserDetailsAsync(string username);
    Task<StoredProcedureResult> AddUserAsync(string username, string fullName, string role, string issuedBy);
    Task<StoredProcedureResult> UpdateUserAsync(string username, string fullName, string role, string issuedBy);
    Task<bool> DeleteUserAsync(string username);

    // Master Data Operations - General
    Task<DataTable> GetAllPartsAsync();
    Task<DataTable> GetAllOperationsAsync();
    Task<DataTable> GetAllLocationsAsync();

    // Additional methods for backward compatibility
    Task<DataTable> GetAllPartIDsAsync();
    Task<DataTable> GetAllItemTypesAsync();
}

/// <summary>
/// Database service implementation.
/// </summary>
public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfigurationService configurationService, ILogger<DatabaseService> logger)
    {
        _connectionString = configurationService?.GetConnectionString() ?? throw new ArgumentNullException(nameof(configurationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string GetConnectionString() => _connectionString;

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection.State == ConnectionState.Open;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return false;
        }
    }

    public async Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        using var adapter = new MySqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        return dataTable;
    }

    public async Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        return await command.ExecuteScalarAsync();
    }

    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand(query, connection);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }
        }

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync();
    }

    // Inventory Operations
    public async Task<StoredProcedureResult> AddInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        try
        {
            // Use stored procedure inv_inventory_Add_Item which generates its own batch number
            var parameters = new Dictionary<string, object>
            {
                { "p_PartID", partId },
                { "p_Location", location },
                { "p_Operation", operation },
                { "p_Quantity", quantity },
                { "p_ItemType", itemType },
                { "p_User", user },
                { "p_Notes", notes ?? string.Empty }
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Add_Item",
                parameters
            );

            // Convert Helper result to DatabaseService result format
            return new StoredProcedureResult
            {
                Success = result.Status == 1,
                Message = result.Message,
                Data = result.Data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute inv_inventory_Add_Item stored procedure");
            return new StoredProcedureResult
            {
                Success = false,
                Message = $"Database error: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<DataTable> GetInventoryByPartIdAsync(string partId)
    {
        var query = "SELECT * FROM inventory WHERE PartId = @partId";
        var parameters = new Dictionary<string, object> { { "@partId", partId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetInventoryByPartAndOperationAsync(string partId, string operation)
    {
        var query = "SELECT * FROM inventory WHERE PartId = @partId AND Operation = @operation";
        var parameters = new Dictionary<string, object>
        {
            { "@partId", partId },
            { "@operation", operation }
        };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetInventoryByUserAsync(string user)
    {
        var query = "SELECT * FROM inventory WHERE User = @user ORDER BY Timestamp DESC LIMIT 100";
        var parameters = new Dictionary<string, object> { { "@user", user } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10)
    {
        var user = userId ?? Environment.UserName;
        var query = "SELECT * FROM transactions WHERE User = @user ORDER BY Timestamp DESC LIMIT @limit";
        var parameters = new Dictionary<string, object>
        {
            { "@user", user },
            { "@limit", limit }
        };
        return await ExecuteQueryAsync(query, parameters);
    }

    // Additional methods would be implemented similarly...
    // Placeholder implementations for interface compliance

    public async Task<StoredProcedureResult> RemoveInventoryItemAsync(string partId, string location, string operation, int quantity, string itemType, string user, string batchNumber, string notes)
    {
        try
        {
            // Use stored procedure inv_inventory_Remove_Item
            var parameters = new Dictionary<string, object>
            {
                { "p_PartID", partId },
                { "p_Location", location },
                { "p_Operation", operation },
                { "p_Quantity", quantity },
                { "p_ItemType", itemType },
                { "p_User", user },
                { "p_BatchNumber", batchNumber ?? string.Empty },
                { "p_Notes", notes ?? string.Empty }
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Remove_Item",
                parameters
            );

            // Convert Helper result to DatabaseService result format
            return new StoredProcedureResult
            {
                Success = result.Status == 1,
                Message = result.Message,
                Data = result.Data
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute inv_inventory_Remove_Item stored procedure");
            return new StoredProcedureResult
            {
                Success = false,
                Message = $"Database error: {ex.Message}",
                Data = null
            };
        }
    }

    public async Task<bool> TransferPartAsync(string batchNumber, string partId, string operation, string newLocation)
    {
        _logger.LogInformation("Transferring part: {PartId} to {Location}", partId, newLocation);
        return await Task.FromResult(true);
    }

    public async Task<bool> TransferQuantityAsync(string batchNumber, string partId, string operation, int transferQuantity, int originalQuantity, string newLocation, string user)
    {
        _logger.LogInformation("Transferring quantity: {Quantity} of {PartId} to {Location}", transferQuantity, partId, newLocation);
        return await Task.FromResult(true);
    }

    public async Task<StoredProcedureResult> UpdateInventoryNotesAsync(int inventoryId, string partId, string batchNumber, string notes, string user)
    {
        _logger.LogInformation("Updating notes for inventory ID: {InventoryId}", inventoryId);
        return new StoredProcedureResult { Success = true, Message = "Notes updated successfully" };
    }

    public async Task<DataTable> GetInventoryByIdAsync(int inventoryId)
    {
        var query = "SELECT * FROM inventory WHERE Id = @id";
        var parameters = new Dictionary<string, object> { { "@id", inventoryId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    // Master Data Operations - placeholder implementations
    public async Task<StoredProcedureResult> AddPartAsync(string partId, string customer, string description, string issuedBy, string itemType)
    {
        return new StoredProcedureResult { Success = true, Message = "Part added successfully" };
    }

    public async Task<StoredProcedureResult> UpdatePartAsync(int id, string partId, string customer, string description, string issuedBy, string itemType)
    {
        return new StoredProcedureResult { Success = true, Message = "Part updated successfully" };
    }

    public async Task<bool> DeletePartAsync(string partId)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetPartByIdAsync(string partId)
    {
        var query = "SELECT * FROM parts WHERE PartId = @partId";
        var parameters = new Dictionary<string, object> { { "@partId", partId } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<StoredProcedureResult> AddOperationAsync(string operation, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "Operation added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateOperationAsync(string operation, string newOperation, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "Operation updated successfully" };
    }

    public async Task<bool> DeleteOperationAsync(string operation)
    {
        return await Task.FromResult(true);
    }

    public async Task<StoredProcedureResult> AddLocationAsync(string location, string issuedBy, string building)
    {
        return new StoredProcedureResult { Success = true, Message = "Location added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateLocationAsync(string oldLocation, string location, string issuedBy, string building)
    {
        return new StoredProcedureResult { Success = true, Message = "Location updated successfully" };
    }

    public async Task<bool> DeleteLocationAsync(string location)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetAllUsersAsync()
    {
        try
        {
            // Use stored procedure instead of direct SQL query
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "usr_users_Get_All",
                new Dictionary<string, object>()
            );

            return result.Status == 1 ? result.Data : new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute usr_users_Get_All stored procedure");
            return new DataTable();
        }
    }

    public async Task<DataTable> GetUserDetailsAsync(string username)
    {
        var query = "SELECT * FROM users WHERE Username = @username";
        var parameters = new Dictionary<string, object> { { "@username", username } };
        return await ExecuteQueryAsync(query, parameters);
    }

    public async Task<StoredProcedureResult> AddUserAsync(string username, string fullName, string role, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "User added successfully" };
    }

    public async Task<StoredProcedureResult> UpdateUserAsync(string username, string fullName, string role, string issuedBy)
    {
        return new StoredProcedureResult { Success = true, Message = "User updated successfully" };
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        return await Task.FromResult(true);
    }

    public async Task<DataTable> GetAllPartsAsync()
    {
        try
        {
            // Use stored procedure instead of direct SQL query
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            return result.Status == 1 ? result.Data : new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute md_part_ids_Get_All stored procedure");
            return new DataTable();
        }
    }

    public async Task<DataTable> GetAllOperationsAsync()
    {
        try
        {
            // Use stored procedure instead of direct SQL query
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            return result.Status == 1 ? result.Data : new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute md_operation_numbers_Get_All stored procedure");
            return new DataTable();
        }
    }

    public async Task<DataTable> GetAllLocationsAsync()
    {
        try
        {
            // Use stored procedure instead of direct SQL query
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            return result.Status == 1 ? result.Data : new DataTable();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute md_locations_Get_All stored procedure");
            return new DataTable();
        }
    }

    public async Task<DataTable> GetAllPartIDsAsync()
    {
        // Alias method for compatibility
        return await GetAllPartsAsync();
    }

    public async Task<DataTable> GetAllItemTypesAsync()
    {
        var query = "SELECT DISTINCT ItemType FROM parts ORDER BY ItemType";
        return await ExecuteQueryAsync(query);
    }
}

/// <summary>
/// Result class for stored procedure operations.
/// </summary>
public class StoredProcedureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    // Backward compatibility properties
    public bool IsSuccess => Success;
    public int Status => Success ? 1 : 0;
}

#endregion
