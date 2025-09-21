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
/// FIXED: Now properly handles procedures with and without OUT parameters.
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
    /// HashSet of stored procedures that have OUT parameters (p_Status, p_ErrorMsg).
    /// Only these procedures should have OUT parameters added automatically.
    /// Based on database schema analysis - 16 out of 51 procedures have OUT parameters.
    /// </summary>
    private static readonly HashSet<string> ProceduresWithOutParameters = new(StringComparer.OrdinalIgnoreCase)
    {
        // Inventory Operations
        "inv_inventory_Add_Item",
        "inv_inventory_Remove_Item",
        "inv_inventory_Update_Item",
        "inv_inventory_Update_Notes",

        // Quick Buttons
        "qb_quickbuttons_Clear_ByUser",
        "qb_quickbuttons_Get_ByUser",
        "qb_quickbuttons_Remove",
        "qb_quickbuttons_Save",
        "qb_quickbuttons_Save_Test",

        // System Operations
        "sys_last_10_transactions_Add_Transaction",
        "sys_user_Validate",

        // User Settings (standard pattern)
        "usr_ui_settings_Get",
        "usr_ui_settings_SetJsonSetting",
        "usr_ui_settings_SetShortcutsJson",
        "usr_ui_settings_SetThemeJson"

        // NOTE: usr_ui_settings_GetShortcutsJson has different OUT parameter pattern (p_ShortcutsJson)
        // This is handled separately in the special cases
    };

    /// <summary>
    /// Determines if a stored procedure has standard OUT parameters (p_Status, p_ErrorMsg).
    /// </summary>
    private static bool HasStandardOutParameters(string procedureName)
    {
        return ProceduresWithOutParameters.Contains(procedureName);
    }

    /// <summary>
    /// Determines if a stored procedure has special OUT parameters (non-standard pattern).
    /// </summary>
    private static bool HasSpecialOutParameters(string procedureName)
    {
        // Special case: usr_ui_settings_GetShortcutsJson has p_ShortcutsJson(OUT)
        return string.Equals(procedureName, "usr_ui_settings_GetShortcutsJson", StringComparison.OrdinalIgnoreCase);
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
    /// FIXED: Only adds OUT parameters for procedures that actually have them.
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

            MySqlParameter? statusParam = null;
            MySqlParameter? messageParam = null;

            // Only add OUT parameters for procedures that have them
            if (HasStandardOutParameters(procedureName))
            {
                statusParam = new MySqlParameter("@p_Status", MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                messageParam = new MySqlParameter("@p_ErrorMsg", MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(statusParam);
                command.Parameters.Add(messageParam);
            }
            else if (HasSpecialOutParameters(procedureName))
            {
                // Special case: usr_ui_settings_GetShortcutsJson
                var shortcutsParam = new MySqlParameter("@p_ShortcutsJson", MySqlDbType.Text)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(shortcutsParam);
            }

            await connection.OpenAsync();

            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();

            await Task.Run(() => adapter.Fill(dataTable));

            result.Data = dataTable;

            // Extract status from OUT parameters if available
            if (statusParam != null && messageParam != null)
            {
                result.Status = statusParam.Value as int? ?? -1;
                result.Message = messageParam.Value as string ?? "Unknown error";
            }
            else
            {
                // For procedures without OUT parameters, determine status from result
                result.Status = dataTable.Rows.Count > 0 ? 1 : 0;
                result.Message = dataTable.Rows.Count > 0 ? "Success" : "No data returned";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Status = -1;
            result.Message = ex.Message;
            _logger?.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
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
    /// FIXED: Only adds OUT parameters for procedures that actually have them.
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

            MySqlParameter? statusParam = null;
            MySqlParameter? messageParam = null;

            // Only add OUT parameters for procedures that have them
            if (HasStandardOutParameters(procedureName))
            {
                statusParam = new MySqlParameter("@p_Status", MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                messageParam = new MySqlParameter("@p_ErrorMsg", MySqlDbType.VarChar, 500)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(statusParam);
                command.Parameters.Add(messageParam);
            }
            // Note: ExecuteWithStatus is not typically used for special cases like GetShortcutsJson

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            // Extract status from OUT parameters if available
            if (statusParam != null && messageParam != null)
            {
                result.Status = statusParam.Value as int? ?? -1;
                result.Message = messageParam.Value as string ?? "Unknown error";
            }
            else
            {
                // For procedures without OUT parameters, use rows affected
                result.Status = rowsAffected > 0 ? 1 : 0;
                result.Message = rowsAffected > 0 ? "Success" : "No rows affected";
            }

            return result;
        }
        catch (Exception ex)
        {
            result.Status = -1;
            result.Message = ex.Message;
            _logger?.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
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
    /// FIXED: Procedures without OUT parameters work correctly.
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

            // ExecuteDataTableDirect doesn't add OUT parameters - just executes as-is
            // This method is for compatibility and direct data retrieval

            await connection.OpenAsync();

            using var adapter = new MySqlDataAdapter(command);
            var dataTable = new DataTable();

            await Task.Run(() => adapter.Fill(dataTable));

            return dataTable;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error executing stored procedure {ProcedureName}", procedureName);
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
