using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Database service interface for MTM operations.
/// </summary>
public interface IDatabaseService
{
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<bool> TestConnectionAsync();
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
            await ErrorHandling.HandleErrorAsync(ex, "ExecuteQueryAsync", Environment.UserName, 
                new Dictionary<string, object> { ["Query"] = query, ["Parameters"] = parameters ?? new Dictionary<string, object>() });
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
            await ErrorHandling.HandleErrorAsync(ex, "ExecuteScalarAsync", Environment.UserName, 
                new Dictionary<string, object> { ["Query"] = query, ["Parameters"] = parameters ?? new Dictionary<string, object>() });
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
            await ErrorHandling.HandleErrorAsync(ex, "ExecuteNonQueryAsync", Environment.UserName, 
                new Dictionary<string, object> { ["Query"] = query, ["Parameters"] = parameters ?? new Dictionary<string, object>() });
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
            await ErrorHandling.HandleErrorAsync(ex, "TestConnectionAsync", Environment.UserName);
            return false;
        }
    }
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
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
        string connectionString, 
        string procedureName, 
        Dictionary<string, object> parameters)
    {
        try
        {
            _logger?.LogDebug("Executing stored procedure: {ProcedureName}", procedureName);

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add input parameters
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }

            // Add output parameters for status and message
            command.Parameters.Add("@status", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@message", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            // Execute and get data
            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);

            // Get output parameter values
            var status = command.Parameters["@status"].Value as int? ?? -1;
            var message = command.Parameters["@message"].Value as string ?? "Unknown error";

            var result = new StoredProcedureResult
            {
                Status = status,
                Message = message,
                Data = dataTable
            };

            _logger?.LogDebug("Stored procedure executed: {ProcedureName}, Status: {Status}, Rows: {RowCount}", 
                procedureName, status, dataTable.Rows.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
            
            await ErrorHandling.HandleErrorAsync(ex, "ExecuteDataTableWithStatus", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["ProcedureName"] = procedureName, 
                    ["Parameters"] = parameters 
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
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteWithStatus(
        string connectionString, 
        string procedureName, 
        Dictionary<string, object> parameters)
    {
        try
        {
            _logger?.LogDebug("Executing stored procedure (no data): {ProcedureName}", procedureName);

            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Add input parameters
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }

            // Add output parameters
            command.Parameters.Add("@status", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            command.Parameters.Add("@message", MySqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            await command.ExecuteNonQueryAsync();

            var status = command.Parameters["@status"].Value as int? ?? -1;
            var message = command.Parameters["@message"].Value as string ?? "Unknown error";

            var result = new StoredProcedureResult
            {
                Status = status,
                Message = message,
                Data = new DataTable()
            };

            _logger?.LogDebug("Stored procedure executed: {ProcedureName}, Status: {Status}", 
                procedureName, status);

            return result;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to execute stored procedure: {ProcedureName}", procedureName);
            
            await ErrorHandling.HandleErrorAsync(ex, "ExecuteWithStatus", Environment.UserName, 
                new Dictionary<string, object> 
                { 
                    ["ProcedureName"] = procedureName, 
                    ["Parameters"] = parameters 
                });

            return new StoredProcedureResult
            {
                Status = -1,
                Message = $"Error executing stored procedure: {ex.Message}",
                Data = new DataTable()
            };
        }
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

    public bool IsSuccess => Status == 0;
}
