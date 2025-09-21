using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models.UI;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Services.UI;


#region Column Configuration Service

/// <summary>
/// Service interface for column configuration persistence.
/// Phase 4 implementation for complete settings persistence across application sessions.
/// </summary>
public interface IColumnConfigurationService
{
    /// <summary>
    /// Saves a column configuration to persistent storage.
    /// </summary>
    Task<bool> SaveConfigurationAsync(string gridId, ColumnConfiguration configuration);

    /// <summary>
    /// Loads a column configuration from persistent storage.
    /// </summary>
    Task<ColumnConfiguration?> LoadConfigurationAsync(string gridId, string configurationId);

    /// <summary>
    /// Deletes a saved column configuration.
    /// </summary>
    Task<bool> DeleteConfigurationAsync(string gridId, string configurationId);

    /// <summary>
    /// Gets all saved column configurations for a specific grid.
    /// </summary>
    Task<List<ColumnConfiguration>> GetAllConfigurationsAsync(string gridId);

    /// <summary>
    /// Saves the current session configuration (auto-save on changes).
    /// </summary>
    Task<bool> SaveSessionConfigurationAsync(string gridId, ColumnConfiguration configuration);

    /// <summary>
    /// Restores the session configuration for the grid (auto-restore on startup).
    /// </summary>
    Task<ColumnConfiguration?> RestoreSessionConfigurationAsync(string gridId);

    /// <summary>
    /// Clears all session configurations.
    /// </summary>
    Task ClearSessionConfigurationsAsync();
}

/// <summary>
/// MTM Column Configuration Service - Phase 4 Implementation
///
/// Provides comprehensive column configuration persistence for CustomDataGrid controls.
/// Supports both user-saved configurations and automatic session management.
/// Follows established MTM service patterns with proper error handling and logging.
///
/// Phase 4 Features:
/// - Complete settings persistence to file system
/// - Automatic session save/restore for seamless user experience
/// - Configuration versioning and migration support
/// - Validation and error recovery for corrupted configurations
/// - Performance optimization with caching for frequently accessed configs
/// </summary>
public class ColumnConfigurationService : IColumnConfigurationService
{
    private readonly ILogger<ColumnConfigurationService> _logger;
    private readonly string _configurationsPath;
    private readonly string _sessionConfigurationsPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, ColumnConfiguration> _configurationCache;

    /// <summary>
    /// Initializes a new instance of the ColumnConfigurationService.
    /// Sets up file paths and JSON serialization options.
    /// </summary>
    public ColumnConfigurationService(ILogger<ColumnConfigurationService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;

        // Set up storage paths
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MTM_WIP_Application",
            "ColumnConfigurations"
        );

        _configurationsPath = Path.Combine(appDataPath, "Saved");
        _sessionConfigurationsPath = Path.Combine(appDataPath, "Sessions");

        // Ensure directories exist
        Directory.CreateDirectory(_configurationsPath);
        Directory.CreateDirectory(_sessionConfigurationsPath);

        // Configure JSON serialization
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        _configurationCache = new Dictionary<string, ColumnConfiguration>();

        _logger.LogDebug("ColumnConfigurationService initialized. Storage path: {Path}", appDataPath);
    }

    #region Saved Configurations

    /// <summary>
    /// Saves a column configuration to persistent storage.
    /// Creates a uniquely named file for the configuration with metadata.
    /// </summary>
    public async Task<bool> SaveConfigurationAsync(string gridId, ColumnConfiguration configuration)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentNullException.ThrowIfNull(configuration);

            if (!configuration.IsValid())
            {
                _logger.LogWarning("Cannot save invalid column configuration for grid: {GridId}", gridId);
                return false;
            }

            configuration.Touch();

            var fileName = GetConfigurationFileName(gridId, configuration.ConfigurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            var json = JsonSerializer.Serialize(configuration, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);

            // Update cache
            var cacheKey = $"{gridId}:{configuration.ConfigurationId}";
            _configurationCache[cacheKey] = configuration;

            _logger.LogInformation("Saved column configuration for grid {GridId}: {ConfigName} to {FilePath}",
                gridId, configuration.DisplayName, filePath);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving column configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Loads a column configuration from persistent storage.
    /// </summary>
    public async Task<ColumnConfiguration?> LoadConfigurationAsync(string gridId, string configurationId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentException.ThrowIfNullOrWhiteSpace(configurationId);

            // Check cache first
            var cacheKey = $"{gridId}:{configurationId}";
            if (_configurationCache.TryGetValue(cacheKey, out var cachedConfig))
            {
                _logger.LogDebug("Loaded column configuration from cache: {GridId}:{ConfigId}", gridId, configurationId);
                return cachedConfig;
            }

            var fileName = GetConfigurationFileName(gridId, configurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogDebug("Column configuration file not found: {FilePath}", filePath);
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

            if (configuration != null && configuration.IsValid())
            {
                // Update cache
                _configurationCache[cacheKey] = configuration;

                _logger.LogDebug("Loaded column configuration for grid {GridId}: {ConfigName}",
                    gridId, configuration.DisplayName);

                return configuration;
            }
            else
            {
                _logger.LogWarning("Invalid column configuration loaded from: {FilePath}", filePath);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading column configuration for grid: {GridId}", gridId);
            return null;
        }
    }

    /// <summary>
    /// Deletes a saved column configuration from persistent storage.
    /// </summary>
    public async Task<bool> DeleteConfigurationAsync(string gridId, string configurationId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentException.ThrowIfNullOrWhiteSpace(configurationId);

            var fileName = GetConfigurationFileName(gridId, configurationId);
            var filePath = Path.Combine(_configurationsPath, fileName);

            await Task.Run(() =>
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    // Remove from cache
                    var cacheKey = $"{gridId}:{configurationId}";
                    _configurationCache.Remove(cacheKey);

                    _logger.LogInformation("Deleted column configuration: {FilePath}", filePath);
                }
                else
                {
                    _logger.LogWarning("Column configuration file not found for deletion: {FilePath}", filePath);
                }
            });

            return !File.Exists(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting column configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Gets all saved column configurations for a specific grid.
    /// </summary>
    public async Task<List<ColumnConfiguration>> GetAllConfigurationsAsync(string gridId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);

            var configurations = new List<ColumnConfiguration>();
            var pattern = $"{gridId}_*.json";
            var files = Directory.GetFiles(_configurationsPath, pattern);

            foreach (var filePath in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(filePath);
                    var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

                    if (configuration != null && configuration.IsValid())
                    {
                        configurations.Add(configuration);
                    }
                    else
                    {
                        _logger.LogWarning("Skipping invalid configuration file: {FilePath}", filePath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error reading configuration file: {FilePath}", filePath);
                }
            }

            configurations.Sort((a, b) =>
                string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

            _logger.LogDebug("Retrieved {Count} configurations for grid: {GridId}", configurations.Count, gridId);
            return configurations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configurations for grid: {GridId}", gridId);
            return new List<ColumnConfiguration>();
        }
    }

    #endregion

    #region Session Management

    /// <summary>
    /// Saves the current session configuration for automatic restore.
    /// This is called automatically when columns are modified.
    /// </summary>
    public async Task<bool> SaveSessionConfigurationAsync(string gridId, ColumnConfiguration configuration)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);
            ArgumentNullException.ThrowIfNull(configuration);

            var fileName = GetSessionFileName(gridId);
            var filePath = Path.Combine(_sessionConfigurationsPath, fileName);

            // Create a session-specific configuration
            var sessionConfig = new ColumnConfiguration(configuration.ConfigurationId, "Session Layout")
            {
                Description = "Auto-saved session configuration",
                ColumnSettings = configuration.ColumnSettings.Select(s => s.Clone()).ToList(),
                IsDefault = false,
                CreatedBy = Environment.UserName
            };

            var json = JsonSerializer.Serialize(sessionConfig, _jsonOptions);
            await File.WriteAllTextAsync(filePath, json);

            _logger.LogDebug("Saved session configuration for grid: {GridId}", gridId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving session configuration for grid: {GridId}", gridId);
            return false;
        }
    }

    /// <summary>
    /// Restores the session configuration for automatic startup restore.
    /// </summary>
    public async Task<ColumnConfiguration?> RestoreSessionConfigurationAsync(string gridId)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(gridId);

            var fileName = GetSessionFileName(gridId);
            var filePath = Path.Combine(_sessionConfigurationsPath, fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogDebug("No session configuration found for grid: {GridId}", gridId);
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var configuration = JsonSerializer.Deserialize<ColumnConfiguration>(json, _jsonOptions);

            if (configuration != null && configuration.IsValid())
            {
                _logger.LogDebug("Restored session configuration for grid: {GridId}", gridId);
                return configuration;
            }
            else
            {
                _logger.LogWarning("Invalid session configuration for grid: {GridId}", gridId);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring session configuration for grid: {GridId}", gridId);
            return null;
        }
    }

    /// <summary>
    /// Clears all session configurations (called on clean application shutdown).
    /// </summary>
    public async Task ClearSessionConfigurationsAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                var files = Directory.GetFiles(_sessionConfigurationsPath, "*.json");

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                _logger.LogDebug("Cleared {Count} session configurations", files.Length);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing session configurations");
        }
    }

    #endregion

    #region Private Methods

    private static string GetConfigurationFileName(string gridId, string configurationId)
    {
        var safeGridId = GetSafeFileName(gridId);
        var safeConfigId = GetSafeFileName(configurationId);
        return $"{safeGridId}_{safeConfigId}.json";
    }

    private static string GetSessionFileName(string gridId)
    {
        var safeGridId = GetSafeFileName(gridId);
        return $"session_{safeGridId}.json";
    }

    private static string GetSafeFileName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "default";

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(input.Where(c => !invalidChars.Contains(c)).ToArray());

        return string.IsNullOrWhiteSpace(safeName) ? "default" : safeName;
    }

    #endregion
}

#endregion
