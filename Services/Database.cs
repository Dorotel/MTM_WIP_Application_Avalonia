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
    Task<DataTable> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<object?> ExecuteScalarAsync(string query, Dictionary<string, object>? parameters = null);
    Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null);
    Task<bool> TestConnectionAsync();
    Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10);
    string GetConnectionString();
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
    /// This stored procedure may have variable parameter requirements, so we try different approaches.
    /// </summary>
    public async Task<DataTable> GetLastTransactionsForUserAsync(string? userId = null, int limit = 10)
    {
        // Use current user if none specified, and ensure it's uppercase
        var currentUser = !string.IsNullOrEmpty(userId) ? userId.ToUpper() : Models.Model_AppVariables.CurrentUser;
        
        _logger.LogDebug("Getting last {Limit} transactions for user: {UserId}", limit, currentUser);

        try
        {
            // Try approach 1: Call with all possible parameters that the SP might expect
            var parameters = new Dictionary<string, object>
            {
                ["p_UserID"] = currentUser,
                ["p_Limit"] = limit,
                ["p_Status"] = "ALL" // Add the missing parameter with a default value
            };

            return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                _connectionString,
                "sys_last_10_transactions_Get_ByUser",
                parameters
            );
        }
        catch (Exception ex1)
        {
            _logger.LogWarning("First attempt failed, trying without p_Status parameter: {Error}", ex1.Message);
            
            try
            {
                // Try approach 2: Call with just the basic parameters
                var basicParameters = new Dictionary<string, object>
                {
                    ["p_UserID"] = currentUser,
                    ["p_Limit"] = limit
                };

                return await Helper_Database_StoredProcedure.ExecuteDataTableDirect(
                    _connectionString,
                    "sys_last_10_transactions_Get_ByUser",
                    basicParameters
                );
            }
            catch (Exception ex2)
            {
                _logger.LogWarning("Second attempt failed, trying direct SQL approach: {Error}", ex2.Message);
                
                try
                {
                    // Try approach 3: Use CALL statement directly to bypass parameter validation
                    var directQuery = "CALL sys_last_10_transactions_Get_ByUser(@p_UserID, @p_Limit)";
                    var directParameters = new Dictionary<string, object>
                    {
                        ["p_UserID"] = currentUser,
                        ["p_Limit"] = limit
                    };

                    var result = await ExecuteQueryAsync(directQuery, directParameters);
                    _logger.LogDebug("Retrieved {RowCount} transactions for user {UserId} using direct CALL", 
                        result.Rows.Count, currentUser);
                    return result;
                }
                catch (Exception ex3)
                {
                    _logger.LogError("All approaches failed for getting transactions");
                    
                    // Use fully qualified namespace for ErrorHandling service
                    await Services.ErrorHandling.HandleErrorAsync(ex3, "GetLastTransactionsForUserAsync", Environment.UserName, 
                        new Dictionary<string, object> 
                        { 
                            ["UserId"] = userId ?? "", 
                            ["Limit"] = limit,
                            ["Operation"] = "GetLastTransactions",
                            ["Service"] = "DatabaseService",
                            ["StoredProcedure"] = "sys_last_10_transactions_Get_ByUser",
                            ["AttemptedApproaches"] = "WithStatus,BasicParams,DirectCall"
                        });
                    
                    // Return empty DataTable rather than throwing, so the UI doesn't crash
                    _logger.LogWarning("Returning empty DataTable due to stored procedure failures");
                    return new DataTable();
                }
            }
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
                var userId = parameters.GetValueOrDefault("p_UserID", "unknown");
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
        var criticalParams = new[] { "p_UserID", "p_PartID", "p_Position", "p_ButtonID", "p_Operation", "p_Quantity" };
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
        var userId = parameters.GetValueOrDefault("p_UserID", "");
        var position = parameters.GetValueOrDefault("p_Position", "");
        var partId = parameters.GetValueOrDefault("p_PartID", "");
        var operation = parameters.GetValueOrDefault("p_Operation", "");
        var quantity = parameters.GetValueOrDefault("p_Quantity", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: SAVE CONTEXT - User '{UserId}' saving Part '{PartId}' with Operation '{Operation}' (Qty: {Quantity}) at position {Position}", 
            userId, partId, operation, quantity, position);
    }

    private static void LogQuickButtonRemoveContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_UserID", "");
        var buttonId = parameters.GetValueOrDefault("p_ButtonID", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: REMOVE CONTEXT - User '{UserId}' removing button at position {ButtonId}", 
            userId, buttonId);
    }

    private static void LogQuickButtonClearContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_UserID", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: CLEAR CONTEXT - User '{UserId}' clearing all quick buttons", userId);
    }

    private static void LogQuickButtonGetContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_UserID", "");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: GET CONTEXT - Retrieving quick buttons for user '{UserId}'", userId);
    }

    private static void LogTransactionGetContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_UserID", "");
        var limit = parameters.GetValueOrDefault("p_Limit", "10");
        
        _logger?.LogInformation("🔍 QUICKBUTTON DEBUG: TRANSACTION GET CONTEXT - Retrieving last {Limit} transactions for user '{UserId}'", 
            limit, userId);
    }

    private static void LogTransactionAddContext(Dictionary<string, object> parameters)
    {
        var userId = parameters.GetValueOrDefault("p_UserID", "");
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

    public bool IsSuccess => Status == 0;
}
