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
    /// Observable collection of all Users from master data
    /// </summary>
    ObservableCollection<string> Users { get; }
    
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
    Task RefreshUsersAsync();
    
    /// <summary>
    /// Event raised when master data is loaded or refreshed
    /// </summary>
    event EventHandler? MasterDataLoaded;
}

/// <summary>
/// Centralized master data service for MTM WIP Application.
/// Provides shared access to Part IDs, Operations, Locations, and Users across all ViewModels.
/// Uses MTM stored procedure patterns. When database is unavailable, collections remain empty
/// and validation logic shows appropriate "no data available" messages to users.
/// </summary>
public class MasterDataService : IMasterDataService
{
    private readonly ILogger<MasterDataService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IDatabaseService _databaseService;
    private bool _isLoading = false;

    public MasterDataService(ILogger<MasterDataService> logger, IConfigurationService configurationService, IDatabaseService databaseService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        
        PartIds = new ObservableCollection<string>();
        Operations = new ObservableCollection<string>();
        Locations = new ObservableCollection<string>();
        Users = new ObservableCollection<string>();
        
        _logger.LogInformation("MasterDataService initialized");
    }

    #region Public Properties

    public ObservableCollection<string> PartIds { get; }
    public ObservableCollection<string> Operations { get; }
    public ObservableCollection<string> Locations { get; }
    public ObservableCollection<string> Users { get; }
    
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
                LoadLocationsFromDatabaseAsync(),
                LoadUsersFromDatabaseAsync()
            );
            
            _logger.LogInformation("Master data loaded successfully from database - Parts: {PartCount}, Operations: {OpCount}, Locations: {LocCount}, Users: {UserCount}", 
                PartIds.Count, Operations.Count, Locations.Count, Users.Count);
                
            MasterDataLoaded?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load master data from database. Collections will remain empty to indicate data unavailability.");
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate server connectivity issues
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

    /// <summary>
    /// Refresh Users from database
    /// </summary>
    public async Task RefreshUsersAsync()
    {
        await LoadUsersFromDatabaseAsync();
        _logger.LogInformation("Users refreshed - Count: {Count}", Users.Count);
    }

    #endregion

    #region Private Database Loading Methods

    /// <summary>
    /// Load Part IDs from database using DatabaseService
    /// </summary>
    private async Task LoadPartIdsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Part IDs from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllPartIDsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    PartIds.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns PartID column
                        var partId = row["PartID"]?.ToString();
                        if (!string.IsNullOrEmpty(partId))
                        {
                            PartIds.Add(partId);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Part IDs from database", PartIds.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Part IDs. No fallback data will be loaded - collections will remain empty.");
                // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Part IDs from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
        }
    }

    /// <summary>
    /// Load Operations from database using DatabaseService
    /// </summary>
    private async Task LoadOperationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Operations from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllOperationsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Operations.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns Operation column
                        var operation = row["Operation"]?.ToString();
                        if (!string.IsNullOrEmpty(operation))
                        {
                            Operations.Add(operation);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Operations from database", Operations.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Operations. No fallback data will be loaded - collections will remain empty.");
                // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Operations from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
        }
    }

    /// <summary>
    /// Load Locations from database using DatabaseService
    /// </summary>
    private async Task LoadLocationsFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Locations from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllLocationsAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Locations.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns Location column
                        var location = row["Location"]?.ToString();
                        if (!string.IsNullOrEmpty(location))
                        {
                            Locations.Add(location);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Locations from database", Locations.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Locations. No fallback data will be loaded - collections will remain empty.");
                // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Locations from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
        }
    }

    /// <summary>
    /// Load Users from database using DatabaseService
    /// </summary>
    private async Task LoadUsersFromDatabaseAsync()
    {
        try
        {
            _logger.LogDebug("Loading Users from database using DatabaseService");
            
            var dataTable = await _databaseService.GetAllUsersAsync();

            if (dataTable != null && dataTable.Rows.Count > 0)
            {
                // Update collection on UI thread
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Users.Clear();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // The stored procedure returns User column (not UserId)
                        var user = row["User"]?.ToString();
                        if (!string.IsNullOrEmpty(user))
                        {
                            Users.Add(user);
                        }
                    }
                });
                _logger.LogInformation("Successfully loaded {Count} Users from database", Users.Count);
            }
            else
            {
                _logger.LogWarning("Database service returned no Users. No fallback data will be loaded - collections will remain empty.");
                // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load Users from database - {ErrorMessage}. No fallback data will be loaded.", ex.Message);
            // MTM Pattern: Do not load fallback data - leave collections empty to indicate data unavailability
        }
    }

    #endregion

    #region Data Availability Status Methods

    /// <summary>
    /// Checks if all master data collections have been populated from the database.
    /// Returns false if any collection is empty, indicating potential database connectivity issues.
    /// </summary>
    public bool IsAllDataAvailable => 
        PartIds.Count > 0 && Operations.Count > 0 && Locations.Count > 0 && Users.Count > 0;

    /// <summary>
    /// Gets a status message describing data availability for user feedback.
    /// </summary>
    public string GetDataAvailabilityStatus()
    {
        var missingData = new List<string>();
        
        if (PartIds.Count == 0) missingData.Add("Part IDs");
        if (Operations.Count == 0) missingData.Add("Operations");
        if (Locations.Count == 0) missingData.Add("Locations");
        if (Users.Count == 0) missingData.Add("Users");

        if (missingData.Count == 0)
        {
            return "All master data loaded successfully.";
        }

        return $"No data available for: {string.Join(", ", missingData)}. Please check server connection.";
    }

    #endregion
}
