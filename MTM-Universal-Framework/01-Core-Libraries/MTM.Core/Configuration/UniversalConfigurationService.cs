using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MTM.UniversalFramework.Core.Configuration;

/// <summary>
/// Universal configuration service interface for cross-platform application configuration.
/// </summary>
public interface IUniversalConfigurationService
{
    /// <summary>
    /// Gets a configuration value by key.
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <returns>Configuration value or null if not found</returns>
    string? GetValue(string key);

    /// <summary>
    /// Gets a configuration value by key with a default value.
    /// </summary>
    /// <param name="key">Configuration key</param>
    /// <param name="defaultValue">Default value to return if key is not found</param>
    /// <returns>Configuration value or default value</returns>
    string GetValue(string key, string defaultValue);

    /// <summary>
    /// Gets a typed configuration value.
    /// </summary>
    /// <typeparam name="T">Type to convert the value to</typeparam>
    /// <param name="key">Configuration key</param>
    /// <param name="defaultValue">Default value to return if key is not found</param>
    /// <returns>Typed configuration value or default value</returns>
    T GetValue<T>(string key, T defaultValue);

    /// <summary>
    /// Gets a connection string by name.
    /// </summary>
    /// <param name="name">Connection string name</param>
    /// <returns>Connection string or null if not found</returns>
    string? GetConnectionString(string name);

    /// <summary>
    /// Binds a configuration section to an object.
    /// </summary>
    /// <typeparam name="T">Type of object to bind to</typeparam>
    /// <param name="sectionKey">Configuration section key</param>
    /// <returns>Bound object instance</returns>
    T GetSection<T>(string sectionKey) where T : class, new();

    /// <summary>
    /// Checks if a configuration key exists.
    /// </summary>
    /// <param name="key">Configuration key to check</param>
    /// <returns>True if key exists, false otherwise</returns>
    bool KeyExists(string key);

    /// <summary>
    /// Gets all configuration keys with a specific prefix.
    /// </summary>
    /// <param name="prefix">Key prefix to search for</param>
    /// <returns>Dictionary of matching keys and values</returns>
    Dictionary<string, string> GetKeysWithPrefix(string prefix);

    /// <summary>
    /// Reloads configuration from all sources.
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task ReloadConfigurationAsync();
}

/// <summary>
/// Universal configuration service implementation.
/// </summary>
public class UniversalConfigurationService : IUniversalConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<UniversalConfigurationService> _logger;

    /// <summary>
    /// Initializes a new instance of the UniversalConfigurationService.
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger instance</param>
    public UniversalConfigurationService(IConfiguration configuration, ILogger<UniversalConfigurationService> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public string? GetValue(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        
        try
        {
            return _configuration[key];
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving configuration value for key: {Key}", key);
            return null;
        }
    }

    /// <inheritdoc />
    public string GetValue(string key, string defaultValue)
    {
        return GetValue(key) ?? defaultValue;
    }

    /// <inheritdoc />
    public T GetValue<T>(string key, T defaultValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            var value = _configuration.GetValue(key, defaultValue);
            _logger.LogTrace("Retrieved configuration value for key {Key}: {Value}", key, value);
            return value ?? defaultValue;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving typed configuration value for key: {Key}, returning default: {Default}", key, defaultValue);
            return defaultValue;
        }
    }

    /// <inheritdoc />
    public string? GetConnectionString(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        try
        {
            var connectionString = _configuration.GetConnectionString(name);
            _logger.LogTrace("Retrieved connection string for: {Name}, found: {Found}", name, !string.IsNullOrEmpty(connectionString));
            return connectionString;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving connection string: {Name}", name);
            return null;
        }
    }

    /// <inheritdoc />
    public T GetSection<T>(string sectionKey) where T : class, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sectionKey);

        try
        {
            var section = new T();
            _configuration.GetSection(sectionKey).Bind(section);
            _logger.LogTrace("Bound configuration section {SectionKey} to type {Type}", sectionKey, typeof(T).Name);
            return section;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error binding configuration section {SectionKey} to type {Type}, returning default instance", sectionKey, typeof(T).Name);
            return new T();
        }
    }

    /// <inheritdoc />
    public bool KeyExists(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        try
        {
            var section = _configuration.GetSection(key);
            return section.Exists();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking if configuration key exists: {Key}", key);
            return false;
        }
    }

    /// <inheritdoc />
    public Dictionary<string, string> GetKeysWithPrefix(string prefix)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);

        var result = new Dictionary<string, string>();

        try
        {
            var section = _configuration.GetSection(prefix);
            foreach (var child in section.GetChildren())
            {
                result[child.Key] = child.Value ?? string.Empty;
            }

            _logger.LogTrace("Retrieved {Count} configuration keys with prefix: {Prefix}", result.Count, prefix);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving configuration keys with prefix: {Prefix}", prefix);
            return result;
        }
    }

    /// <inheritdoc />
    public async Task ReloadConfigurationAsync()
    {
        try
        {
            _logger.LogInformation("Reloading configuration from all sources");
            
            if (_configuration is IConfigurationRoot configRoot)
            {
                configRoot.Reload();
            }

            _logger.LogInformation("Configuration reloaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reloading configuration");
        }

        await Task.CompletedTask;
    }
}