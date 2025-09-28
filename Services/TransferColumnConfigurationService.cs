using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Service for managing TransferTabView column configuration persistence
/// Integrates with MySQL usr_ui_settings table for user-specific preferences
/// </summary>
public interface IColumnConfigurationService
{
    Task<ServiceResult<ColumnConfiguration>> LoadColumnConfigAsync(string userId);
    Task<ServiceResult<bool>> SaveColumnConfigAsync(string userId, ColumnConfiguration config);
    Task<ServiceResult<bool>> ResetColumnConfigAsync(string userId);
    ColumnConfiguration GetDefaultConfiguration();
}

/// <summary>
/// Implementation of column configuration service for TransferTabView DataGrid
/// Uses MySQL usr_ui_settings table with JSON column storage for user preferences
/// </summary>
public class TransferColumnConfigurationService : IColumnConfigurationService
{
    private readonly ILogger<TransferColumnConfigurationService> _logger;
    private readonly IDatabaseService _databaseService;
    private readonly JsonSerializerOptions _jsonOptions;

    public TransferColumnConfigurationService(
        ILogger<TransferColumnConfigurationService> logger,
        IDatabaseService databaseService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(databaseService);

        _logger = logger;
        _databaseService = databaseService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Load user's column configuration preferences from MySQL usr_ui_settings table
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Column configuration or default configuration if none exists</returns>
    public async Task<ServiceResult<ColumnConfiguration>> LoadColumnConfigAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogDebug("Loading default column configuration for TransferTabView");
                return ServiceResult<ColumnConfiguration>.Success(GetDefaultConfiguration());
            }

            _logger.LogDebug("Loading column configuration for user: {UserId}", userId);

            // Query usr_ui_settings table for TransferTabView column configuration
            var connectionString = _databaseService.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogWarning("Database connection string is not available, using default configuration");
                return ServiceResult<ColumnConfiguration>.Success(GetDefaultConfiguration());
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_UserId", userId }
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "usr_ui_settings_Get_TransferColumns",
                    parameters
                );

                if (result.IsSuccess && result.Data.Rows.Count > 0)
                {
                    var columnConfigJson = result.Data.Rows[0]["ColumnConfig"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(columnConfigJson))
                    {
                        var columnConfig = JsonSerializer.Deserialize<ColumnConfiguration>(columnConfigJson, _jsonOptions);
                        if (columnConfig != null && columnConfig.IsValid())
                        {
                            _logger.LogDebug("Successfully loaded column configuration for user: {UserId}", userId);
                            return ServiceResult<ColumnConfiguration>.Success(columnConfig);
                        }
                    }
                }

                // No saved configuration found, return default
                _logger.LogDebug("No saved column configuration found for user: {UserId}, using defaults", userId);
                return ServiceResult<ColumnConfiguration>.Success(GetDefaultConfiguration());
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Database error loading column configuration for user: {UserId}", userId);
                await Services.ErrorHandling.HandleErrorAsync(dbEx, $"Failed to load column configuration for user: {userId}", userId);
                return ServiceResult<ColumnConfiguration>.Success(GetDefaultConfiguration());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading column configuration for user: {UserId}", userId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load column configuration for user: {userId}", userId);
            return ServiceResult<ColumnConfiguration>.Success(GetDefaultConfiguration());
        }
    }

    /// <summary>
    /// Save user's column configuration preferences to MySQL usr_ui_settings table
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="config">Column configuration to save</param>
    /// <returns>Success result indicating if save operation completed</returns>
    public async Task<ServiceResult<bool>> SaveColumnConfigAsync(string userId, ColumnConfiguration config)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("SaveColumnConfigAsync called with empty user ID");
                return ServiceResult<bool>.Failure("User ID is required");
            }

            if (config == null || !config.IsValid())
            {
                _logger.LogWarning("SaveColumnConfigAsync called with invalid configuration");
                return ServiceResult<bool>.Failure("Valid column configuration is required");
            }

            _logger.LogDebug("Saving column configuration for user: {UserId}", userId);

            // Update last modified timestamp
            config.UpdateLastModified();

            var configJson = JsonSerializer.Serialize(config, _jsonOptions);
            var connectionString = _databaseService.GetConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return ServiceResult<bool>.Failure("Database connection string is not available");
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "p_UserId", userId },
                    { "p_ColumnConfig", configJson }
                };

                var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                    connectionString,
                    "usr_ui_settings_Set_TransferColumns",
                    parameters
                );

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Successfully saved column configuration for user: {UserId}", userId);
                    return ServiceResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("Failed to save column configuration for user: {UserId}, Status: {Status}, Message: {Message}",
                        userId, result.Status, result.Message);
                    return ServiceResult<bool>.Failure($"Database operation failed: {result.Message}");
                }
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Database error saving column configuration for user: {UserId}", userId);
                await Services.ErrorHandling.HandleErrorAsync(dbEx, $"Failed to save column configuration for user: {userId}", userId);
                return ServiceResult<bool>.Failure("Database operation failed", dbEx);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error saving column configuration for user: {UserId}", userId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to save column configuration for user: {userId}", userId);
            return ServiceResult<bool>.Failure("Failed to save column configuration", ex);
        }
    }

    /// <summary>
    /// Reset user's column configuration to default settings
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <returns>Success result indicating if reset operation completed</returns>
    public async Task<ServiceResult<bool>> ResetColumnConfigAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<bool>.Failure("User ID is required");
            }

            _logger.LogDebug("Resetting column configuration to defaults for user: {UserId}", userId);

            // Save default configuration for the user
            var defaultConfig = GetDefaultConfiguration();
            var saveResult = await SaveColumnConfigAsync(userId, defaultConfig);

            if (saveResult.IsSuccess)
            {
                _logger.LogInformation("Successfully reset column configuration for user: {UserId}", userId);
                return ServiceResult<bool>.Success(true);
            }
            else
            {
                return ServiceResult<bool>.Failure($"Failed to reset configuration: {saveResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting column configuration for user: {UserId}", userId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to reset column configuration for user: {userId}", userId);
            return ServiceResult<bool>.Failure("Failed to reset column configuration", ex);
        }
    }

    /// <summary>
    /// Get default column configuration for TransferTabView DataGrid
    /// </summary>
    /// <returns>Default column configuration</returns>
    public ColumnConfiguration GetDefaultConfiguration()
    {
        return ColumnConfiguration.Default;
    }
}
