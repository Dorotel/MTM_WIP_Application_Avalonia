using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Dapper;
using MTM_Shared_Logic.Models;

namespace MTM_WIP_Application_Avalonia.Services
{
    /// <summary>
    /// CRITICAL DATABASE ACCESS LAYER - Helper class for stored procedure execution.
    /// This class provides the essential database access layer that was missing.
    /// ALL database operations in the MTM application MUST use this class.
    /// 
    /// SECURITY ENFORCED: Only stored procedures are allowed - NO direct SQL execution.
    /// This matches the existing DatabaseService pattern but provides the Helper_Database_StoredProcedure 
    /// that existing code expects to use.
    /// </summary>
    public static class Helper_Database_StoredProcedure
    {
        private static ILogger? _logger;
        private static readonly int DefaultCommandTimeout = 30;

        /// <summary>
        /// Sets the logger for this helper class. Should be called during application startup.
        /// </summary>
        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes a stored procedure and returns a DataTable with status information.
        /// This is the primary method used throughout the MTM application.
        /// 
        /// CRITICAL: Only stored procedure names allowed - validates against SQL injection.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="procedureName">Name of the stored procedure to execute</param>
        /// <param name="parameters">Parameters for the stored procedure</param>
        /// <returns>DataTableWithStatus containing results and execution status</returns>
        public static async Task<DataTableWithStatus> ExecuteDataTableWithStatus(
            string connectionString,
            string procedureName,
            Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return DataTableWithStatus.Failure("Stored procedure name cannot be empty");
            }

            // SECURITY: Validate that this is a procedure name, not SQL injection
            if (IsSqlQuery(procedureName))
            {
                var errorMsg = "SECURITY VIOLATION: Direct SQL execution is prohibited. Only stored procedure names are allowed.";
                _logger?.LogError("Attempted SQL injection or policy violation with procedure name: {ProcedureName}", procedureName);
                return DataTableWithStatus.Failure(errorMsg);
            }

            try
            {
                _logger?.LogDebug("Executing stored procedure: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                var dynParams = new DynamicParameters();
                
                // Add input parameters
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        dynParams.Add(param.Key, param.Value);
                    }
                }

                // Add standard output parameters that MTM stored procedures use
                dynParams.Add("p_Status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                dynParams.Add("p_ErrorMsg", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

                // Execute the stored procedure and get results
                var reader = await connection.ExecuteReaderAsync(
                    procedureName,
                    dynParams,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: DefaultCommandTimeout);

                // Convert to DataTable
                var dataTable = new DataTable();
                dataTable.Load(reader);

                // Get output parameters
                var status = dynParams.Get<int?>("p_Status") ?? 0;
                var errorMessage = dynParams.Get<string>("p_ErrorMsg");

                // Create result with status information
                var result = new DataTableWithStatus
                {
                    Data = dataTable,
                    Status = status,
                    ErrorMessage = errorMessage,
                    IsSuccess = status == 0, // MTM convention: 0 = success
                    RowsAffected = dataTable.Rows.Count
                };

                // Capture all output parameters
                foreach (var paramName in dynParams.ParameterNames)
                {
                    if (paramName.StartsWith("p_") || paramName.StartsWith("@"))
                    {
                        result.OutputParameters[paramName] = dynParams.Get<object>(paramName);
                    }
                }

                _logger?.LogDebug("Stored procedure executed successfully. Status: {Status}, Rows: {RowCount}", 
                    status, dataTable.Rows.Count);

                return result;
            }
            catch (MySqlException mysqlEx)
            {
                var errorMsg = $"MySQL error executing stored procedure '{procedureName}': {mysqlEx.Message}";
                _logger?.LogError(mysqlEx, errorMsg);
                return DataTableWithStatus.Failure(errorMsg, mysqlEx);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error executing stored procedure '{procedureName}': {ex.Message}";
                _logger?.LogError(ex, errorMsg);
                return DataTableWithStatus.Failure(errorMsg, ex);
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns the number of affected rows.
        /// Used for INSERT, UPDATE, DELETE operations.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="procedureName">Name of the stored procedure to execute</param>
        /// <param name="parameters">Parameters for the stored procedure</param>
        /// <returns>Number of affected rows, or -1 on error</returns>
        public static async Task<int> ExecuteNonQuery(
            string connectionString,
            string procedureName,
            Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(procedureName))
            {
                _logger?.LogError("Stored procedure name cannot be empty");
                return -1;
            }

            // SECURITY: Validate that this is a procedure name, not SQL injection
            if (IsSqlQuery(procedureName))
            {
                _logger?.LogError("SECURITY VIOLATION: Direct SQL execution prohibited for: {ProcedureName}", procedureName);
                return -1;
            }

            try
            {
                _logger?.LogDebug("Executing non-query stored procedure: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                var affectedRows = await connection.ExecuteAsync(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: DefaultCommandTimeout);

                _logger?.LogDebug("Non-query stored procedure executed. Affected rows: {AffectedRows}", affectedRows);
                return affectedRows;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error executing non-query stored procedure: {ProcedureName}", procedureName);
                return -1;
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns a single scalar value.
        /// </summary>
        /// <typeparam name="T">Type of the scalar value to return</typeparam>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="procedureName">Name of the stored procedure to execute</param>
        /// <param name="parameters">Parameters for the stored procedure</param>
        /// <returns>Scalar value or default(T) on error</returns>
        public static async Task<T?> ExecuteScalar<T>(
            string connectionString,
            string procedureName,
            Dictionary<string, object>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty", nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(procedureName))
            {
                _logger?.LogError("Stored procedure name cannot be empty");
                return default(T);
            }

            // SECURITY: Validate that this is a procedure name, not SQL injection
            if (IsSqlQuery(procedureName))
            {
                _logger?.LogError("SECURITY VIOLATION: Direct SQL execution prohibited for: {ProcedureName}", procedureName);
                return default(T);
            }

            try
            {
                _logger?.LogDebug("Executing scalar stored procedure: {ProcedureName}", procedureName);

                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                var result = await connection.QuerySingleOrDefaultAsync<T>(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: DefaultCommandTimeout);

                _logger?.LogDebug("Scalar stored procedure executed. Result: {Result}", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error executing scalar stored procedure: {ProcedureName}", procedureName);
                return default(T);
            }
        }

        /// <summary>
        /// Validates that the input is a procedure name and not SQL code.
        /// Prevents SQL injection and enforces stored procedure usage.
        /// </summary>
        private static bool IsSqlQuery(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var lowerInput = input.Trim().ToLowerInvariant();
            
            // Check for SQL keywords that indicate direct SQL rather than procedure names
            var sqlKeywords = new[]
            {
                "select", "insert", "update", "delete", "drop", "create", "alter", 
                "truncate", "grant", "revoke", "union", "join", "where", "from", 
                "into", "values", "set", "exec", "execute", "sp_executesql",
                "information_schema", "mysql", "performance_schema"
            };

            return Array.Exists(sqlKeywords, keyword => 
                lowerInput.Contains(keyword + " ") || 
                lowerInput.StartsWith(keyword + " ") ||
                lowerInput.Equals(keyword));
        }
    }

    /// <summary>
    /// Represents the result of a stored procedure execution with status information.
    /// This class provides the expected return type for Helper_Database_StoredProcedure.ExecuteDataTableWithStatus.
    /// </summary>
    public class DataTableWithStatus
    {
        public DataTable Data { get; set; } = new DataTable();
        public int Status { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public int RowsAffected { get; set; }
        public Dictionary<string, object> OutputParameters { get; set; } = new Dictionary<string, object>();
        public Exception? Exception { get; set; }

        /// <summary>
        /// Creates a successful result with data.
        /// </summary>
        public static DataTableWithStatus Success(DataTable data, int rowsAffected = 0)
        {
            return new DataTableWithStatus
            {
                Data = data,
                Status = 0, // MTM convention: 0 = success
                IsSuccess = true,
                RowsAffected = rowsAffected > 0 ? rowsAffected : data.Rows.Count
            };
        }

        /// <summary>
        /// Creates a failed result with error message.
        /// </summary>
        public static DataTableWithStatus Failure(string errorMessage, Exception? exception = null)
        {
            return new DataTableWithStatus
            {
                Data = new DataTable(),
                Status = -1, // MTM convention: non-zero = error
                ErrorMessage = errorMessage,
                IsSuccess = false,
                RowsAffected = 0,
                Exception = exception
            };
        }

        /// <summary>
        /// Gets a typed value from the output parameters.
        /// </summary>
        public T? GetOutputParameter<T>(string parameterName)
        {
            if (OutputParameters.TryGetValue(parameterName, out var value))
            {
                if (value is T typedValue)
                    return typedValue;
                
                // Try to convert
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return default(T);
                }
            }
            
            return default(T);
        }
    }
}
