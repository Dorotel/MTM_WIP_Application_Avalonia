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

#region Helper_Database_StoredProcedure

/// <summary>
/// Static helper class for database stored procedure operations.
/// Provides consistent interface for executing stored procedures with status handling.
/// CRITICAL: This class was missing and causing compilation errors throughout the application.
/// </summary>
public static class Helper_Database_StoredProcedure
{
    private static ILogger? _logger;

    /// <summary>
    /// Sets the logger instance for database operations.
    /// </summary>
    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Result structure for stored procedure execution with status and data.
    /// </summary>
    public class StoredProcedureResult
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public DataTable Data { get; set; } = new();

        /// <summary>
        /// Indicates if the stored procedure executed successfully.
        /// MTM convention: Status >= 0 for success, -1 for error
        /// </summary>
        public bool IsSuccess => Status >= 0 || (Data != null && Data.Rows.Count > 0);
    }

    /// <summary>
    /// Result structure for stored procedure execution with status only.
    /// </summary>
    public class StoredProcedureStatusResult
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the stored procedure executed successfully.
        /// </summary>
        public bool IsSuccess => Status >= 0;
    }

    /// <summary>
    /// Executes a stored procedure and returns DataTable with status information.
    /// Status Pattern: 1=Success with data, 0=Success no data, -1=Error
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
        string connectionString,
        string procedureName,
        MySqlParameter[] parameters)
    {
        var result = new StoredProcedureResult();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            // Add output parameters for status
            var statusParam = new MySqlParameter("@p_Status", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            var messageParam = new MySqlParameter("@p_Message", MySqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();

            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();

            await Task.Run(() => adapter.Fill(dataTable));

            result.Data = dataTable;
            result.Status = statusParam.Value as int? ?? -1;
            result.Message = messageParam.Value as string ?? "Unknown error";

            return result;
        }
        catch (Exception ex)
        {
            result.Status = -1;
            result.Message = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Executes a stored procedure and returns DataTable with status (Dictionary parameter overload).
    /// </summary>
    public static async Task<StoredProcedureResult> ExecuteDataTableWithStatus(
        string connectionString,
        string procedureName,
        Dictionary<string, object> parameters)
    {
        var mysqlParams = parameters.Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
        return await ExecuteDataTableWithStatus(connectionString, procedureName, mysqlParams);
    }

    /// <summary>
    /// Executes a stored procedure and returns status information only (no data).
    /// Status Pattern: 1=Success with data, 0=Success no data, -1=Error
    /// </summary>
    public static async Task<StoredProcedureStatusResult> ExecuteWithStatus(
        string connectionString,
        string procedureName,
        MySqlParameter[] parameters)
    {
        var result = new StoredProcedureStatusResult();

        try
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            // Add output parameters for status
            var statusParam = new MySqlParameter("@p_Status", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            var messageParam = new MySqlParameter("@p_Message", MySqlDbType.VarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(statusParam);
            command.Parameters.Add(messageParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            result.Status = statusParam.Value as int? ?? -1;
            result.Message = messageParam.Value as string ?? "Unknown error";

            return result;
        }
        catch (Exception ex)
        {
            result.Status = -1;
            result.Message = ex.Message;
            return result;
        }
    }

    /// <summary>
    /// Executes a stored procedure and returns status (Dictionary parameter overload).
    /// </summary>
    public static async Task<StoredProcedureStatusResult> ExecuteWithStatus(
        string connectionString,
        string procedureName,
        Dictionary<string, object> parameters)
    {
        var mysqlParams = parameters.Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
        return await ExecuteWithStatus(connectionString, procedureName, mysqlParams);
    }

    /// <summary>
    /// Executes a stored procedure and returns DataTable directly (for compatibility).
    /// This method does not handle status - use ExecuteDataTableWithStatus for new code.
    /// </summary>
    public static async Task<DataTable> ExecuteDataTableDirect(
        string connectionString,
        string procedureName,
        MySqlParameter[] parameters)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            using var command = new MySqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();

            await Task.Run(() => adapter.Fill(dataTable));

            return dataTable;
        }
        catch (Exception)
        {
            return new DataTable(); // Return empty table on error
        }
    }

    /// <summary>
    /// Executes a stored procedure and returns DataTable directly (Dictionary parameter overload).
    /// </summary>
    public static async Task<DataTable> ExecuteDataTableDirect(
        string connectionString,
        string procedureName,
        Dictionary<string, object> parameters)
    {
        var mysqlParams = parameters.Select(p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray();
        return await ExecuteDataTableDirect(connectionString, procedureName, mysqlParams);
    }
}

#endregion
