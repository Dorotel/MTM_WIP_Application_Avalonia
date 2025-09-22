# External System Integration - MTM WIP Application Instructions

**Framework**: .NET 8 with HttpClient  
**Pattern**: External API Integration with Circuit Breaker  
**Created**: 2025-09-14  

---

## 🎯 Core External System Integration Patterns

### REST API Integration for Manufacturing Systems

```csharp
// HTTP client service for external manufacturing system integration
public class ExternalManufacturingApiService : IExternalManufacturingApiService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ExternalManufacturingApiService> _logger;
    private readonly ICircuitBreakerService _circuitBreaker;
    private readonly SemaphoreSlim _rateLimitSemaphore;
    
    public ExternalManufacturingApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ExternalManufacturingApiService> logger,
        ICircuitBreakerService circuitBreaker)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _circuitBreaker = circuitBreaker;
        
        // Rate limiting for external API calls (10 concurrent requests max)
        _rateLimitSemaphore = new SemaphoreSlim(10, 10);
        
        ConfigureHttpClient();
    }
    
    private void ConfigureHttpClient()
    {
        var baseUrl = _configuration["ExternalSystems:ManufacturingAPI:BaseUrl"];
        var apiKey = _configuration["ExternalSystems:ManufacturingAPI:ApiKey"];
        var timeout = _configuration.GetValue<int>("ExternalSystems:ManufacturingAPI:TimeoutSeconds", 30);
        
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(timeout);
        
        // Standard headers for manufacturing API
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        _httpClient.DefaultRequestHeaders.Add("X-Client-Name", "MTM-WIP-Application");
        _httpClient.DefaultRequestHeaders.Add("X-Client-Version", "1.0.0");
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }
    
    // Sync inventory data with external ERP system
    public async Task<ServiceResult<InventorySyncResult>> SyncInventoryWithERPAsync(
        IEnumerable<InventoryItem> inventoryItems, 
        CancellationToken cancellationToken = default)
    {
        var itemList = inventoryItems.ToList();
        
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            
            try
            {
                _logger.LogInformation("Starting inventory sync with ERP for {ItemCount} items", itemList.Count);
                
                var syncRequest = new ErpInventorySyncRequest
                {
                    Items = itemList.Select(item => new ErpInventoryItem
                    {
                        PartNumber = item.PartId,
                        Operation = item.Operation,
                        Quantity = item.Quantity,
                        Location = item.Location,
                        LastUpdated = item.LastUpdated,
                        UpdatedBy = item.LastUpdatedBy
                    }).ToList(),
                    SyncTimestamp = DateTime.UtcNow,
                    SourceSystem = "MTM-WIP"
                };
                
                var json = JsonSerializer.Serialize(syncRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                using var response = await _httpClient.PostAsync("/api/v1/inventory/sync", content, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                    var syncResult = JsonSerializer.Deserialize<ErpInventorySyncResponse>(responseJson, 
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    
                    _logger.LogInformation("ERP inventory sync completed: {Successful}/{Total} items synced", 
                        syncResult?.SuccessfulItems ?? 0, itemList.Count);
                    
                    return ServiceResult<InventorySyncResult>.Success(new InventorySyncResult
                    {
                        TotalItems = itemList.Count,
                        SuccessfulItems = syncResult?.SuccessfulItems ?? 0,
                        FailedItems = syncResult?.FailedItems ?? 0,
                        SyncTimestamp = DateTime.UtcNow,
                        ErrorMessages = syncResult?.Errors ?? new List<string>()
                    });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("ERP inventory sync failed: {StatusCode} - {Content}", 
                        response.StatusCode, errorContent);
                    
                    return ServiceResult<InventorySyncResult>.Failure(
                        $"ERP sync failed: HTTP {response.StatusCode} - {errorContent}");
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }, "SyncInventoryWithERP");
    }
    
    // Get part master data from external PLM system
    public async Task<ServiceResult<List<PartMasterData>>> GetPartMasterDataAsync(
        IEnumerable<string> partIds,
        CancellationToken cancellationToken = default)
    {
        var partIdList = partIds.Distinct().ToList();
        
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            
            try
            {
                var queryString = string.Join("&", partIdList.Select(id => $"partIds={Uri.EscapeDataString(id)}"));
                var requestUri = $"/api/v1/parts/masterdata?{queryString}";
                
                using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    var masterData = JsonSerializer.Deserialize<List<PlmPartMasterData>>(json,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    
                    var result = masterData?.Select(plm => new PartMasterData
                    {
                        PartId = plm.PartNumber,
                        Description = plm.Description,
                        Category = plm.Category,
                        UnitOfMeasure = plm.UnitOfMeasure,
                        StandardCost = plm.StandardCost,
                        IsActive = plm.Status == "Active",
                        LastUpdated = plm.LastModified
                    }).ToList() ?? new List<PartMasterData>();
                    
                    _logger.LogInformation("Retrieved master data for {Found}/{Requested} parts from PLM", 
                        result.Count, partIdList.Count);
                    
                    return ServiceResult<List<PartMasterData>>.Success(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    return ServiceResult<List<PartMasterData>>.Failure(
                        $"PLM master data retrieval failed: HTTP {response.StatusCode} - {errorContent}");
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }, "GetPartMasterData");
    }
    
    public void Dispose()
    {
        _rateLimitSemaphore?.Dispose();
        _httpClient?.Dispose();
    }
}

// Data transfer objects for external API integration
public class ErpInventorySyncRequest
{
    public List<ErpInventoryItem> Items { get; set; } = new();
    public DateTime SyncTimestamp { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
}

public class ErpInventoryItem
{
    public string PartNumber { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}

public class InventorySyncResult
{
    public int TotalItems { get; set; }
    public int SuccessfulItems { get; set; }
    public int FailedItems { get; set; }
    public DateTime SyncTimestamp { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
}
```

### Authentication and Security Patterns

```csharp
// Secure authentication service for external manufacturing systems
public class ManufacturingSystemAuthService : IManufacturingSystemAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _tokenCache;
    private readonly ILogger<ManufacturingSystemAuthService> _logger;
    private readonly SemaphoreSlim _tokenRefreshSemaphore;
    
    public ManufacturingSystemAuthService(
        HttpClient httpClient,
        IConfiguration configuration,
        IMemoryCache tokenCache,
        ILogger<ManufacturingSystemAuthService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _tokenCache = tokenCache;
        _logger = logger;
        _tokenRefreshSemaphore = new SemaphoreSlim(1, 1);
    }
    
    public async Task<string?> GetAuthTokenAsync(string systemName, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"auth_token_{systemName}";
        
        // Check cache first
        if (_tokenCache.TryGetValue(cacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            return cachedToken;
        }
        
        // Prevent multiple simultaneous token refresh attempts
        await _tokenRefreshSemaphore.WaitAsync(cancellationToken);
        
        try
        {
            // Double-check cache after acquiring lock
            if (_tokenCache.TryGetValue(cacheKey, out cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }
            
            // Refresh token
            var newToken = await RefreshAuthTokenAsync(systemName, cancellationToken);
            
            if (!string.IsNullOrEmpty(newToken))
            {
                // Cache token with expiration
                var expirationMinutes = _configuration.GetValue<int>($"ExternalSystems:{systemName}:TokenExpirationMinutes", 55);
                _tokenCache.Set(cacheKey, newToken, TimeSpan.FromMinutes(expirationMinutes));
                
                _logger.LogInformation("Authentication token refreshed for system: {SystemName}", systemName);
            }
            
            return newToken;
        }
        finally
        {
            _tokenRefreshSemaphore.Release();
        }
    }
    
    private async Task<string?> RefreshAuthTokenAsync(string systemName, CancellationToken cancellationToken)
    {
        try
        {
            var authUrl = _configuration[$"ExternalSystems:{systemName}:AuthUrl"];
            var clientId = _configuration[$"ExternalSystems:{systemName}:ClientId"];
            var clientSecret = _configuration[$"ExternalSystems:{systemName}:ClientSecret"];
            
            var authRequest = new
            {
                grant_type = "client_credentials",
                client_id = clientId,
                client_secret = clientSecret,
                scope = "manufacturing.inventory manufacturing.parts"
            };
            
            var json = JsonSerializer.Serialize(authRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using var response = await _httpClient.PostAsync(authUrl, content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<AuthTokenResponse>(responseJson,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                
                return tokenResponse?.AccessToken;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Authentication failed for system {SystemName}: {StatusCode} - {Content}", 
                    systemName, response.StatusCode, errorContent);
                
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing authentication token for system: {SystemName}", systemName);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Authentication - {systemName}");
            return null;
        }
    }
}

public class AuthTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string Scope { get; set; } = string.Empty;
}
```

---

## 🏭 Manufacturing-Specific External Integration

### Barcode Scanner Integration

```csharp
// Barcode scanner integration for manufacturing operations
public class BarcodeIntegrationService : IBarcodeIntegrationService
{
    private readonly ILogger<BarcodeIntegrationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly SerialPort? _serialPort;
    private readonly Timer _connectionMonitor;
    
    public event EventHandler<BarcodeScannedEventArgs>? BarcodeScanned;
    
    public BarcodeIntegrationService(ILogger<BarcodeIntegrationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        // Initialize serial port for barcode scanner
        var portName = _configuration["BarcodeScanner:PortName"];
        var baudRate = _configuration.GetValue<int>("BarcodeScanner:BaudRate", 9600);
        
        if (!string.IsNullOrEmpty(portName))
        {
            try
            {
                _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };
                
                _serialPort.DataReceived += OnDataReceived;
                
                // Monitor connection health
                _connectionMonitor = new Timer(CheckConnection, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
                
                OpenConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize barcode scanner on port {PortName}", portName);
            }
        }
    }
    
    private void OpenConnection()
    {
        try
        {
            if (_serialPort != null && !_serialPort.IsOpen)
            {
                _serialPort.Open();
                _logger.LogInformation("Barcode scanner connection opened on {PortName}", _serialPort.PortName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open barcode scanner connection");
        }
    }
    
    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (_serialPort?.IsOpen == true)
            {
                var data = _serialPort.ReadLine().Trim();
                
                if (!string.IsNullOrEmpty(data))
                {
                    var scannedData = ParseBarcodeData(data);
                    
                    _logger.LogInformation("Barcode scanned: {Data}", data);
                    
                    BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
                    {
                        RawData = data,
                        PartId = scannedData.PartId,
                        SerialNumber = scannedData.SerialNumber,
                        ScanTimestamp = DateTime.Now
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing barcode scan data");
        }
    }
    
    private BarcodeData ParseBarcodeData(string rawData)
    {
        // Parse manufacturing barcode format: PART|SERIAL|OPERATION
        var parts = rawData.Split('|', StringSplitOptions.RemoveEmptyEntries);
        
        return new BarcodeData
        {
            PartId = parts.Length > 0 ? parts[0] : rawData,
            SerialNumber = parts.Length > 1 ? parts[1] : string.Empty,
            Operation = parts.Length > 2 ? parts[2] : string.Empty
        };
    }
    
    private void CheckConnection(object? state)
    {
        if (_serialPort != null && !_serialPort.IsOpen)
        {
            _logger.LogWarning("Barcode scanner connection lost, attempting to reconnect");
            OpenConnection();
        }
    }
    
    public void Dispose()
    {
        _connectionMonitor?.Dispose();
        
        if (_serialPort?.IsOpen == true)
        {
            _serialPort.Close();
        }
        
        _serialPort?.Dispose();
    }
}

public class BarcodeScannedEventArgs : EventArgs
{
    public string RawData { get; set; } = string.Empty;
    public string PartId { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public DateTime ScanTimestamp { get; set; }
}

public class BarcodeData
{
    public string PartId { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
}
```

### MES (Manufacturing Execution System) Integration

```csharp
// Integration with Manufacturing Execution System
public class MESIntegrationService : IMESIntegrationService
{
    private readonly IExternalManufacturingApiService _apiService;
    private readonly IMessenger _messenger;
    private readonly ILogger<MESIntegrationService> _logger;
    private readonly Timer _syncTimer;
    
    public MESIntegrationService(
        IExternalManufacturingApiService apiService,
        IMessenger messenger,
        ILogger<MESIntegrationService> logger)
    {
        _apiService = apiService;
        _messenger = messenger;
        _logger = logger;
        
        // Periodic sync with MES every 5 minutes
        _syncTimer = new Timer(PeriodicSync, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
        
        // Register for inventory change notifications
        _messenger.Register<InventoryAddedMessage>(this, OnInventoryChanged);
        _messenger.Register<InventoryRemovedMessage>(this, OnInventoryChanged);
    }
    
    private void OnInventoryChanged(object message)
    {
        // Queue inventory change for next sync cycle
        _ = Task.Run(async () =>
        {
            try
            {
                if (message is InventoryAddedMessage added)
                {
                    await NotifyMESOfInventoryChangeAsync(added.Item, "ADDED");
                }
                else if (message is InventoryRemovedMessage removed)
                {
                    await NotifyMESOfInventoryChangeAsync(
                        new InventoryItem { PartId = removed.PartId, Operation = removed.Operation, Quantity = removed.Quantity },
                        "REMOVED");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify MES of inventory change");
            }
        });
    }
    
    private async Task NotifyMESOfInventoryChangeAsync(InventoryItem item, string changeType)
    {
        try
        {
            var notification = new MESInventoryNotification
            {
                PartNumber = item.PartId,
                Operation = item.Operation,
                Quantity = item.Quantity,
                Location = item.Location,
                ChangeType = changeType,
                Timestamp = DateTime.UtcNow,
                SourceSystem = "MTM-WIP"
            };
            
            await _apiService.PostMESNotificationAsync("/api/v1/inventory/notification", notification);
            
            _logger.LogInformation("Notified MES of inventory change: {PartId} {ChangeType}", item.PartId, changeType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to notify MES of inventory change for part {PartId}", item.PartId);
        }
    }
    
    private async void PeriodicSync(object? state)
    {
        try
        {
            await SyncWorkOrdersWithMESAsync();
            await SyncProductionScheduleAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Periodic MES sync failed");
        }
    }
    
    private async Task SyncWorkOrdersWithMESAsync()
    {
        try
        {
            var workOrders = await _apiService.GetActiveWorkOrdersAsync();
            
            if (workOrders.IsSuccess && workOrders.Data?.Any() == true)
            {
                _logger.LogInformation("Retrieved {Count} active work orders from MES", workOrders.Data.Count);
                
                // Notify other services of work order updates
                _messenger.Send(new WorkOrdersUpdatedMessage(workOrders.Data));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync work orders with MES");
        }
    }
    
    private async Task SyncProductionScheduleAsync()
    {
        try
        {
            var schedule = await _apiService.GetProductionScheduleAsync();
            
            if (schedule.IsSuccess && schedule.Data != null)
            {
                _logger.LogInformation("Updated production schedule from MES");
                
                _messenger.Send(new ProductionScheduleUpdatedMessage(schedule.Data));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync production schedule with MES");
        }
    }
    
    public void Dispose()
    {
        _syncTimer?.Dispose();
        _messenger.UnregisterAll(this);
    }
}

public class MESInventoryNotification
{
    public string PartNumber { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string ChangeType { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string SourceSystem { get; set; } = string.Empty;
}

public record WorkOrdersUpdatedMessage(List<WorkOrder> WorkOrders);
public record ProductionScheduleUpdatedMessage(ProductionSchedule Schedule);
```

---

## 🔧 Error Handling and Resilience Patterns

### Circuit Breaker Implementation

```csharp
// Circuit breaker pattern for external system resilience
public class ExternalSystemCircuitBreaker : ICircuitBreakerService
{
    private readonly Dictionary<string, CircuitBreakerState> _circuitStates;
    private readonly ILogger<ExternalSystemCircuitBreaker> _logger;
    private readonly IConfiguration _configuration;
    private readonly object _lock = new object();
    
    public ExternalSystemCircuitBreaker(ILogger<ExternalSystemCircuitBreaker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _circuitStates = new Dictionary<string, CircuitBreakerState>();
    }
    
    public async Task<ServiceResult<T>> ExecuteAsync<T>(
        Func<Task<ServiceResult<T>>> operation,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        var state = GetOrCreateCircuitState(operationName);
        
        lock (_lock)
        {
            if (state.State == CircuitState.Open)
            {
                if (DateTime.UtcNow - state.LastFailureTime < state.OpenTimeout)
                {
                    return ServiceResult<T>.Failure(
                        $"Circuit breaker is open for {operationName}. Service temporarily unavailable.");
                }
                
                // Transition to half-open
                state.State = CircuitState.HalfOpen;
                _logger.LogInformation("Circuit breaker transitioning to half-open for {Operation}", operationName);
            }
        }
        
        try
        {
            var result = await operation();
            
            if (result.IsSuccess)
            {
                OnSuccess(state, operationName);
            }
            else
            {
                OnFailure(state, operationName, new Exception(result.ErrorMessage));
            }
            
            return result;
        }
        catch (Exception ex)
        {
            OnFailure(state, operationName, ex);
            return ServiceResult<T>.Failure($"Operation {operationName} failed: {ex.Message}", ex);
        }
    }
    
    private CircuitBreakerState GetOrCreateCircuitState(string operationName)
    {
        lock (_lock)
        {
            if (!_circuitStates.ContainsKey(operationName))
            {
                var failureThreshold = _configuration.GetValue<int>($"CircuitBreaker:{operationName}:FailureThreshold", 5);
                var openTimeout = _configuration.GetValue<TimeSpan>($"CircuitBreaker:{operationName}:OpenTimeout", TimeSpan.FromMinutes(1));
                
                _circuitStates[operationName] = new CircuitBreakerState
                {
                    FailureThreshold = failureThreshold,
                    OpenTimeout = openTimeout,
                    State = CircuitState.Closed,
                    FailureCount = 0,
                    LastFailureTime = DateTime.MinValue
                };
            }
            
            return _circuitStates[operationName];
        }
    }
    
    private void OnSuccess(CircuitBreakerState state, string operationName)
    {
        lock (_lock)
        {
            state.FailureCount = 0;
            if (state.State == CircuitState.HalfOpen)
            {
                state.State = CircuitState.Closed;
                _logger.LogInformation("Circuit breaker closed for {Operation}", operationName);
            }
        }
    }
    
    private void OnFailure(CircuitBreakerState state, string operationName, Exception exception)
    {
        lock (_lock)
        {
            state.FailureCount++;
            state.LastFailureTime = DateTime.UtcNow;
            
            if (state.FailureCount >= state.FailureThreshold)
            {
                state.State = CircuitState.Open;
                _logger.LogWarning("Circuit breaker opened for {Operation} after {Failures} failures. Last error: {Error}",
                    operationName, state.FailureCount, exception.Message);
            }
        }
    }
}

public class CircuitBreakerState
{
    public CircuitState State { get; set; }
    public int FailureCount { get; set; }
    public int FailureThreshold { get; set; }
    public DateTime LastFailureTime { get; set; }
    public TimeSpan OpenTimeout { get; set; }
}

public enum CircuitState
{
    Closed,   // Normal operation
    Open,     // Failing fast
    HalfOpen  // Testing recovery
}
```

---

## 📚 Related Documentation

- **Service Integration**: [Cross-Service Communication](./service-integration.instructions.md)
- **Configuration Management**: [Application Configuration](./application-configuration.instructions.md)
- **Error Handling**: [Error Management Patterns](./error-handling.instructions.md)
- **Authentication**: [Security Patterns](./security-patterns.instructions.md)

---

**Document Status**: ✅ Complete External System Integration Reference  
**Framework Version**: .NET 8 with HttpClient  
**Last Updated**: 2025-09-14  
**External Integration Owner**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**
- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**
- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**
- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**
- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**
- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**
- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**
- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
