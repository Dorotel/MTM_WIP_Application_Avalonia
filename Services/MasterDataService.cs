using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Avalonia.Threading;
using MySql.Data.MySqlClient;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for master data service that provides centralized access to reference data
/// </summary>
public interface IMasterDataService
{
    /// <summary>
    /// Observable collection of all Part IDs from master data
    /// </summary>
    ObservableCollection<string> PartIds { get; }
    
    /// <summary>
    /// Observable collection of all Operations from master data
    /// </summary>
    ObservableCollection<string> Operations { get; }
    
    /// <summary>
    /// Observable collection of all Locations from master data
    /// </summary>
    ObservableCollection<string> Locations { get; }
    
    /// <summary>
    /// Indicates if master data is currently being loaded
    /// </summary>
    bool IsLoading { get; }
    
    /// <summary>
    /// Loads all master data from database using stored procedures
    /// </summary>
    Task LoadAllMasterDataAsync();
    
    /// <summary>
    /// Refreshes specific master data category
    /// </summary>
    Task RefreshPartIdsAsync();
    Task RefreshOperationsAsync();
    Task RefreshLocationsAsync();
    
    /// <summary>
    /// Event raised when master data is loaded or refreshed
    /// </summary>
    event EventHandler? MasterDataLoaded;
}

/// <summary>
/// Centralized master data service for MTM WIP Application.
/// Provides shared access to Part IDs, Operations, and Locations across all ViewModels.
/// Uses MTM stored procedure patterns with fallback data when database is unavailable.
/// </summary>
public class MasterDataService : IMasterDataService
{
    private readonly ILogger<MasterDataService> _logger;
    private readonly IConfigurationService _configurationService;
    private bool _isLoading = false;

    public MasterDataService(ILogger<MasterDataService> logger, IConfigurationService configurationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        
        PartIds = new ObservableCollection<string>();
        Operations = new ObservableCollection<string>();
        Locations = new ObservableCollection<string>();
        
        _logger.LogInformation("MasterDataService initialized");
    }

    #region Public Properties

    public ObservableCollection<string> PartIds { get; }
    public ObservableCollection<string> Operations { get; }
    public ObservableCollection<string> Locations { get; }
    
    public bool IsLoading 
    { 
        get => _isLoading;
        private set => _isLoading = value;
    }

    public event EventHandler? MasterDataLoaded;

    #endregion

    #region Master Data Loading - MTM Database Pattern

    /// <summary>
    /// Load all master data from database using stored procedures (MTM Pattern)
    /// Falls back to hardcoded data if database is unavailable
    /// </summary>
    public async Task LoadAllMasterDataAsync()
    {
        if (IsLoading)
        {
            _logger.LogDebug("Master data loading already in progress, skipping");
            return;
        }

        try
        {
            IsLoading = true;
            _logger.LogInformation("Loading master data using MTM stored procedure patterns");
            
            // Test database connection first
            var connectionString = _configurationService.GetConnectionString();
            _logger.LogInformation("Testing database connection...");
            
            // Test basic connectivity with timeout
            try 
            {
                using var testConnection = new MySqlConnection(connectionString);
                // Set connection timeout
                var cts = new System.Threading.CancellationTokenSource(TimeSpan.FromSeconds(5));
                
                await testConnection.OpenAsync(cts.Token);
                _logger.LogInformation("✅ Database connection test successful - Server: {Server}, Database: {Database}", 
                    testConnection.DataSource, testConnection.Database);
                testConnection.Close();
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                _logger.LogError("❌ Database connection timed out after 5 seconds");
                throw new TimeoutException("Database connection timed out");
            }
            catch (Exception connEx)
            {
                _logger.LogError(connEx, "❌ Database connection test failed - {ErrorMessage}", connEx.Message);
                throw new InvalidOperationException("Database connection failed", connEx);
            }
            
            await Task.WhenAll(
                LoadPartIdsFromDatabaseAsync(),
                LoadOperationsFromDatabaseAsync(),
                LoadLocationsFromDatabaseAsync()
            );
            
            _logger.LogInformation("Master data loaded successfully from database - Parts: {PartCount}, Operations: {OpCount}, Locations: {LocCount}", 
                PartIds.Count, Operations.Count, Locations.Count);
                
            MasterDataLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load master data from database, falling back to hardcoded data");
            await LoadFallbackDataAsync();
            MasterDataLoaded?.Invoke(this, EventArgs.Empty);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Refresh Part IDs from database
    /// </summary>
    public async Task RefreshPartIdsAsync()
    {
        await LoadPartIdsFromDatabaseAsync();
        _logger.LogInformation("Part IDs refreshed - Count: {Count}", PartIds.Count);
    }

    /// <summary>
    /// Refresh Operations from database
    /// </summary>
    public async Task RefreshOperationsAsync()
    {
        await LoadOperationsFromDatabaseAsync();
        _logger.LogInformation("Operations refreshed - Count: {Count}", Operations.Count);
    }

    /// <summary>
    /// Refresh Locations from database
    /// </summary>
    public async Task RefreshLocationsAsync()
    {
        await LoadLocationsFromDatabaseAsync();
        _logger.LogInformation("Locations refreshed - Count: {Count}", Locations.Count);
    }

    #endregion

    #region Private Database Loading Methods

    /// <summary>
    /// Load Part IDs from database using stored procedure md_part_ids_Get_All
    /// </summary>
    private async Task LoadPartIdsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Attempting to load Part IDs from database using stored procedure md_part_ids_Get_All");
            
            var connectionString = _configurationService.GetConnectionString();
            _logger.LogDebug("Using connection string: {ConnectionString}", 
                connectionString.Replace("Pwd=password", "Pwd=***")); // Hide password in logs
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_part_ids_Get_All",
                new Dictionary<string, object>()
            );

            _logger.LogDebug("Database call completed - Status: {Status}, Message: '{Message}', Rows: {RowCount}", 
                result.Status, result.Message, result.Data?.Rows.Count ?? 0);

            if (result.IsSuccess && result.Data != null && result.Data.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PartIds.Clear();
                    foreach (DataRow row in result.Data.Rows)
                    {
                        // The stored procedure returns PartID column
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartIds.Add(partId);
                            _logger.LogDebug("Added Part ID: {PartId}", partId);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Part IDs from database", PartIds.Count);
            }
            else
            {
                _logger.LogWarning("Database query returned no data or failed - Status: {Status}, Message: '{Message}'. Loading fallback data.", 
                    result.Status, result.Message);
                await LoadFallbackPartIdsAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Part IDs from database - {ErrorMessage}. Loading fallback data.", ex.Message);
            await LoadFallbackPartIdsAsync();
        }
    }

    /// <summary>
    /// Load Operations from database using stored procedure md_operation_numbers_Get_All
    /// </summary>
    private async Task LoadOperationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Attempting to load Operations from database using stored procedure md_operation_numbers_Get_All");
            
            var connectionString = _configurationService.GetConnectionString();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_operation_numbers_Get_All",
                new Dictionary<string, object>()
            );

            _logger.LogDebug("Database call completed - Status: {Status}, Message: '{Message}', Rows: {RowCount}", 
                result.Status, result.Message, result.Data?.Rows.Count ?? 0);

            if (result.IsSuccess && result.Data != null && result.Data.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Operations.Clear();
                    foreach (DataRow row in result.Data.Rows)
                    {
                        // The stored procedure returns Operation column
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            Operations.Add(operation);
                            _logger.LogDebug("Added Operation: {Operation}", operation);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Operations from database", Operations.Count);
            }
            else
            {
                _logger.LogWarning("Database query returned no data or failed - Status: {Status}, Message: '{Message}'. Loading fallback data.", 
                    result.Status, result.Message);
                await LoadFallbackOperationsAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Operations from database - {ErrorMessage}. Loading fallback data.", ex.Message);
            await LoadFallbackOperationsAsync();
        }
    }

    /// <summary>
    /// Load Locations from database using stored procedure md_locations_Get_All
    /// </summary>
    private async Task LoadLocationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Attempting to load Locations from database using stored procedure md_locations_Get_All");
            
            var connectionString = _configurationService.GetConnectionString();
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_locations_Get_All",
                new Dictionary<string, object>()
            );

            _logger.LogDebug("Database call completed - Status: {Status}, Message: '{Message}', Rows: {RowCount}", 
                result.Status, result.Message, result.Data?.Rows.Count ?? 0);

            if (result.IsSuccess && result.Data != null && result.Data.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Locations.Clear();
                    foreach (DataRow row in result.Data.Rows)
                    {
                        // The stored procedure returns Location column
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            Locations.Add(location);
                            _logger.LogDebug("Added Location: {Location}", location);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Locations from database", Locations.Count);
            }
            else
            {
                _logger.LogWarning("Database query returned no data or failed - Status: {Status}, Message: '{Message}'. Loading fallback data.", 
                    result.Status, result.Message);
                await LoadFallbackLocationsAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Locations from database - {ErrorMessage}. Loading fallback data.", ex.Message);
            await LoadFallbackLocationsAsync();
        }
    }

    #endregion

    #region Fallback Data Methods

    /// <summary>
    /// Load all fallback data when database is unavailable
    /// </summary>
    private async Task LoadFallbackDataAsync()
    {
        await Task.WhenAll(
            LoadFallbackPartIdsAsync(),
            LoadFallbackOperationsAsync(),
            LoadFallbackLocationsAsync()
        );
        _logger.LogInformation("Fallback data loaded for all master data collections");
    }

    private Task LoadFallbackPartIdsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            PartIds.Clear();
            var fallbackParts = new[] { "PART001", "PART002", "PART003", "ABC-123", "XYZ-789", "MTM-001", "MTM-002" };
            foreach (var part in fallbackParts)
            {
                PartIds.Add(part);
            }
        });
        _logger.LogDebug("Loaded {Count} fallback Part IDs", 7);
        return Task.CompletedTask;
    }

    private Task LoadFallbackOperationsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            Operations.Clear();
            var fallbackOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in fallbackOperations)
            {
                Operations.Add(operation);
            }
        });
        _logger.LogDebug("Loaded {Count} fallback Operations", 5);
        return Task.CompletedTask;
    }

    private Task LoadFallbackLocationsAsync()
    {
        Dispatcher.UIThread.Post(() =>
        {
            Locations.Clear();
            var fallbackLocations = new[] { "WC01", "WC02", "FLOOR", "QC", "SHIPPING", "RECEIVING" };
            foreach (var location in fallbackLocations)
            {
                Locations.Add(location);
            }
        });
        _logger.LogDebug("Loaded {Count} fallback Locations", 6);
        return Task.CompletedTask;
    }

    #endregion
}
