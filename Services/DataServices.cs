using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_Shared_Logic.Core.Services;
using MTM_Shared_Logic.Services.Interfaces;
using MTM_WIP_Application_Avalonia.Models;
using MTM_Shared_Logic.Models;

namespace MTM_Shared_Logic.Services
{
    /// <summary>
    /// Data services for managing lookup data used in AutoCompleteBox controls.
    /// Implements efficient caching, lazy loading, and shared data management for large datasets.
    /// All data services are grouped in this file following MTM service organization patterns.
    /// </summary>

    /// <summary>
    /// Interface for managing lookup data collections.
    /// </summary>
    public interface ILookupDataService
    {
        Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetPartIdsAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetOperationsAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetLocationsAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetUsersAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result> RefreshAllAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result> RefreshPartIdsAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result> RefreshOperationsAsync(CancellationToken cancellationToken = default);
        Task<MTM_Shared_Logic.Models.Result> RefreshLocationsAsync(CancellationToken cancellationToken = default);
        void ClearCache();
        event EventHandler<DataRefreshedEventArgs>? DataRefreshed;
    }

    /// <summary>
    /// Centralized lookup data service for AutoCompleteBox controls.
    /// Manages large datasets efficiently with caching and lazy loading.
    /// Optimized for datasets with tens of thousands of rows.
    /// </summary>
    public class LookupDataService : ILookupDataService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<LookupDataService> _logger;

        // Cache keys for different data types
        private const string PARTS_CACHE_KEY = "lookup_data_parts";
        private const string OPERATIONS_CACHE_KEY = "lookup_data_operations";
        private const string LOCATIONS_CACHE_KEY = "lookup_data_locations";
        private const string USERS_CACHE_KEY = "lookup_data_users";

        // Cache expiration times
        private static readonly TimeSpan PARTS_CACHE_DURATION = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan OPERATIONS_CACHE_DURATION = TimeSpan.FromHours(2);
        private static readonly TimeSpan LOCATIONS_CACHE_DURATION = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan USERS_CACHE_DURATION = TimeSpan.FromMinutes(10);

        // Thread-safe collections for in-memory data
        private readonly ConcurrentDictionary<string, ObservableCollection<string>> _dataCache = new();
        private readonly ConcurrentDictionary<string, DateTime> _lastRefresh = new();
        private readonly SemaphoreSlim _refreshSemaphore = new(1, 1);

        public LookupDataService(
            IDatabaseService databaseService,
            ICacheService cacheService,
            ILogger<LookupDataService> logger)
        {
            _databaseService = databaseService;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Event fired when data is refreshed.
        /// </summary>
        public event EventHandler<DataRefreshedEventArgs>? DataRefreshed;

        /// <summary>
        /// Gets Part IDs with intelligent caching and lazy loading.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetPartIdsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedDataAsync(
                PARTS_CACHE_KEY,
                PARTS_CACHE_DURATION,
                LoadPartIdsFromDatabaseAsync,
                cancellationToken);
        }

        /// <summary>
        /// Gets Operations with intelligent caching and lazy loading.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetOperationsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedDataAsync(
                OPERATIONS_CACHE_KEY,
                OPERATIONS_CACHE_DURATION,
                LoadOperationsFromDatabaseAsync,
                cancellationToken);
        }

        /// <summary>
        /// Gets Locations with intelligent caching and lazy loading.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetLocationsAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedDataAsync(
                LOCATIONS_CACHE_KEY,
                LOCATIONS_CACHE_DURATION,
                LoadLocationsFromDatabaseAsync,
                cancellationToken);
        }

        /// <summary>
        /// Gets Users with intelligent caching and lazy loading.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            return await GetCachedDataAsync(
                USERS_CACHE_KEY,
                USERS_CACHE_DURATION,
                LoadUsersFromDatabaseAsync,
                cancellationToken);
        }

        /// <summary>
        /// Refreshes all cached data.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result> RefreshAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Refreshing all lookup data");

                await _refreshSemaphore.WaitAsync(cancellationToken);
                try
                {
                    // Clear all cached data
                    ClearCache();

                    // Preload commonly used data
                    var tasks = new[]
                    {
                        GetPartIdsAsync(cancellationToken),
                        GetOperationsAsync(cancellationToken),
                        GetLocationsAsync(cancellationToken)
                    };

                    await Task.WhenAll(tasks);

                    DataRefreshed?.Invoke(this, new DataRefreshedEventArgs(DataType.All));
                    _logger.LogInformation("All lookup data refreshed successfully");

                    return MTM_Shared_Logic.Models.Result.Success();
                }
                finally
                {
                    _refreshSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing all lookup data");
                return MTM_Shared_Logic.Models.Result.Failure($"Failed to refresh lookup data: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes Part IDs data specifically.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result> RefreshPartIdsAsync(CancellationToken cancellationToken = default)
        {
            return await RefreshSpecificDataAsync(PARTS_CACHE_KEY, DataType.Parts, cancellationToken);
        }

        /// <summary>
        /// Refreshes Operations data specifically.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result> RefreshOperationsAsync(CancellationToken cancellationToken = default)
        {
            return await RefreshSpecificDataAsync(OPERATIONS_CACHE_KEY, DataType.Operations, cancellationToken);
        }

        /// <summary>
        /// Refreshes Locations data specifically.
        /// </summary>
        public async Task<MTM_Shared_Logic.Models.Result> RefreshLocationsAsync(CancellationToken cancellationToken = default)
        {
            return await RefreshSpecificDataAsync(LOCATIONS_CACHE_KEY, DataType.Locations, cancellationToken);
        }

        /// <summary>
        /// Clears all cached data.
        /// </summary>
        public void ClearCache()
        {
            _dataCache.Clear();
            _lastRefresh.Clear();
            _logger.LogDebug("Lookup data cache cleared");
        }

        #region Private Helper Methods

        /// <summary>
        /// Generic method for getting cached data with intelligent refresh logic.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<ObservableCollection<string>>> GetCachedDataAsync(
            string cacheKey,
            TimeSpan cacheDuration,
            Func<CancellationToken, Task<MTM_Shared_Logic.Models.Result<List<string>>>> dataLoader,
            CancellationToken cancellationToken)
        {
            try
            {
                // Check if data is in memory cache and still valid
                if (_dataCache.TryGetValue(cacheKey, out var cachedData) && 
                    _lastRefresh.TryGetValue(cacheKey, out var lastRefresh) &&
                    DateTime.UtcNow - lastRefresh < cacheDuration)
                {
                    _logger.LogDebug("Returning cached data for key: {CacheKey}", cacheKey);
                    return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Success(cachedData);
                }

                // Check persistent cache
                var persistentCachedData = await _cacheService.GetAsync<List<string>>(cacheKey);
                if (persistentCachedData != null && persistentCachedData.Count > 0)
                {
                    var observableData = new ObservableCollection<string>(persistentCachedData);
                    _dataCache[cacheKey] = observableData;
                    _lastRefresh[cacheKey] = DateTime.UtcNow;
                    
                    _logger.LogDebug("Returning persistent cached data for key: {CacheKey}", cacheKey);
                    return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Success(observableData);
                }

                // Load from database
                await _refreshSemaphore.WaitAsync(cancellationToken);
                try
                {
                    // Double-check in case another thread loaded the data
                    if (_dataCache.TryGetValue(cacheKey, out var doubleCheckData))
                    {
                        return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Success(doubleCheckData);
                    }

                    _logger.LogInformation("Loading data from database for key: {CacheKey}", cacheKey);
                    var result = await dataLoader(cancellationToken);

                    if (!result.IsSuccess)
                    {
                        return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Failure(result.ErrorMessage ?? "Failed to load data");
                    }

                    var observableCollection = new ObservableCollection<string>(result.Value ?? new List<string>());
                    
                    // Cache in memory and persistent storage
                    _dataCache[cacheKey] = observableCollection;
                    _lastRefresh[cacheKey] = DateTime.UtcNow;
                    await _cacheService.SetAsync(cacheKey, result.Value, cacheDuration);

                    _logger.LogInformation("Loaded {Count} items for key: {CacheKey}", observableCollection.Count, cacheKey);
                    return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Success(observableCollection);
                }
                finally
                {
                    _refreshSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cached data for key: {CacheKey}", cacheKey);
                return MTM_Shared_Logic.Models.Result<ObservableCollection<string>>.Failure($"Failed to get data: {ex.Message}");
            }
        }

        /// <summary>
        /// Refreshes specific data type.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result> RefreshSpecificDataAsync(string cacheKey, DataType dataType, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Refreshing {DataType} data", dataType);

                // Remove from caches
                _dataCache.TryRemove(cacheKey, out _);
                _lastRefresh.TryRemove(cacheKey, out _);
                await _cacheService.RemoveAsync(cacheKey);

                // Reload data based on type
                var result = dataType switch
                {
                    DataType.Parts => await GetPartIdsAsync(cancellationToken),
                    DataType.Operations => await GetOperationsAsync(cancellationToken),
                    DataType.Locations => await GetLocationsAsync(cancellationToken),
                    DataType.Users => await GetUsersAsync(cancellationToken),
                    _ => throw new ArgumentException($"Unknown data type: {dataType}")
                };

                if (result.IsSuccess)
                {
                    DataRefreshed?.Invoke(this, new DataRefreshedEventArgs(dataType));
                    _logger.LogInformation("{DataType} data refreshed successfully", dataType);
                    return MTM_Shared_Logic.Models.Result.Success();
                }

                return MTM_Shared_Logic.Models.Result.Failure(result.ErrorMessage ?? $"Failed to refresh {dataType} data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing {DataType} data", dataType);
                return MTM_Shared_Logic.Models.Result.Failure($"Failed to refresh {dataType} data: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads Part IDs from the database using stored procedure.
        /// For now, returns sample data until Helper_Database_StoredProcedure is available.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<List<string>>> LoadPartIdsFromDatabaseAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Implement actual database access when Helper_Database_StoredProcedure is available
                // For now, return sample data
                _logger.LogDebug("Loading sample part IDs (database integration pending)");
                
                await Task.Delay(100, cancellationToken); // Simulate database call
                
                var partIds = new List<string>
                {
                    "PART001", "PART002", "PART003", "PART004", "PART005", 
                    "ABC-123", "XYZ-789", "DEF-456", "GHI-101", "JKL-202",
                    "MNO-303", "PQR-404", "STU-505", "VWX-606", "YZA-707"
                };

                // Sort for better user experience
                partIds.Sort();

                _logger.LogDebug("Loaded {Count} part IDs from sample data", partIds.Count);
                return MTM_Shared_Logic.Models.Result<List<string>>.Success(partIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading part IDs from database");
                return MTM_Shared_Logic.Models.Result<List<string>>.Failure($"Failed to load part IDs: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads Operations from the database using stored procedure.
        /// For now, returns sample data until Helper_Database_StoredProcedure is available.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<List<string>>> LoadOperationsFromDatabaseAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Implement actual database access when Helper_Database_StoredProcedure is available
                // For now, return sample data
                _logger.LogDebug("Loading sample operations (database integration pending)");
                
                await Task.Delay(50, cancellationToken); // Simulate database call
                
                var operations = new List<string>
                {
                    "90", "100", "110", "120", "130", "140", "150", "160", "170", "180"
                };

                // Sort numerically for operations
                operations.Sort((x, y) => 
                {
                    if (int.TryParse(x, out int numX) && int.TryParse(y, out int numY))
                        return numX.CompareTo(numY);
                    return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
                });

                _logger.LogDebug("Loaded {Count} operations from sample data", operations.Count);
                return MTM_Shared_Logic.Models.Result<List<string>>.Success(operations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading operations from database");
                return MTM_Shared_Logic.Models.Result<List<string>>.Failure($"Failed to load operations: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads Locations from the database using stored procedure.
        /// For now, returns sample data until Helper_Database_StoredProcedure is available.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<List<string>>> LoadLocationsFromDatabaseAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Implement actual database access when Helper_Database_StoredProcedure is available
                // For now, return sample data
                _logger.LogDebug("Loading sample locations (database integration pending)");
                
                await Task.Delay(75, cancellationToken); // Simulate database call
                
                var locations = new List<string>
                {
                    "WC01", "WC02", "WC03", "WC04", "WC05", 
                    "FLOOR", "QC", "SHIPPING", "RECEIVING", "ASSEMBLY",
                    "STORAGE", "INSPECTION", "REWORK", "FINISHED"
                };

                // Sort for better user experience
                locations.Sort();

                _logger.LogDebug("Loaded {Count} locations from sample data", locations.Count);
                return MTM_Shared_Logic.Models.Result<List<string>>.Success(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading locations from database");
                return MTM_Shared_Logic.Models.Result<List<string>>.Failure($"Failed to load locations: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads Users from the database using stored procedure.
        /// For now, returns sample data until Helper_Database_StoredProcedure is available.
        /// </summary>
        private async Task<MTM_Shared_Logic.Models.Result<List<string>>> LoadUsersFromDatabaseAsync(CancellationToken cancellationToken)
        {
            try
            {
                // TODO: Implement actual database access when Helper_Database_StoredProcedure is available
                // For now, return sample data
                _logger.LogDebug("Loading sample users (database integration pending)");
                
                await Task.Delay(25, cancellationToken); // Simulate database call
                
                var users = new List<string>
                {
                    "DefaultUser", "Admin", "Operator1", "Operator2", "Supervisor", 
                    "QCInspector", "Manager", "Technician1", "Technician2"
                };

                // Sort for better user experience
                users.Sort();

                _logger.LogDebug("Loaded {Count} users from sample data", users.Count);
                return MTM_Shared_Logic.Models.Result<List<string>>.Success(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users from database");
                return MTM_Shared_Logic.Models.Result<List<string>>.Failure($"Failed to load users: {ex.Message}");
            }
        }

        #endregion

        /// <summary>
        /// Disposable cleanup.
        /// </summary>
        public void Dispose()
        {
            _refreshSemaphore?.Dispose();
            ClearCache();
        }
    }

    /// <summary>
    /// Event arguments for data refresh notifications.
    /// </summary>
    public class DataRefreshedEventArgs : EventArgs
    {
        public DataType DataType { get; }
        public DateTime RefreshTime { get; }

        public DataRefreshedEventArgs(DataType dataType)
        {
            DataType = dataType;
            RefreshTime = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Types of lookup data.
    /// </summary>
    public enum DataType
    {
        Parts,
        Operations,
        Locations,
        Users,
        All
    }
}
