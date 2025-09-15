# Service Integration - MTM WIP Application Instructions

**Framework**: .NET 8 with Microsoft Extensions  
**Pattern**: Service-Oriented Architecture with Messaging  
**Created**: 2025-09-14  

---

## 🎯 Core Service Integration Patterns

### Service-to-Service Communication via Dependency Injection

```csharp
// Standard MTM service integration pattern
public class InventoryService : IInventoryService
{
    private readonly IDatabaseService _databaseService;
    private readonly ITransactionService _transactionService;
    private readonly IMasterDataService _masterDataService;
    private readonly IMessenger _messenger;
    private readonly ILogger<InventoryService> _logger;
    
    public InventoryService(
        IDatabaseService databaseService,
        ITransactionService transactionService,
        IMasterDataService masterDataService,
        IMessenger messenger,
        ILogger<InventoryService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        _masterDataService = masterDataService ?? throw new ArgumentNullException(nameof(masterDataService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ServiceResult<InventoryItem>> AddInventoryAsync(
        string partId, string operation, int quantity, string location, string userId)
    {
        try
        {
            // Step 1: Validate with MasterDataService
            var validationResult = await ValidateInventoryDataAsync(partId, operation, location);
            if (!validationResult.IsSuccess)
            {
                return ServiceResult<InventoryItem>.Failure(validationResult.ErrorMessage);
            }
            
            // Step 2: Perform inventory operation via DatabaseService
            var inventoryResult = await _databaseService.ExecuteStoredProcedureAsync(
                "inv_inventory_Add_Item",
                new { PartId = partId, Operation = operation, Quantity = quantity, Location = location, User = userId });
                
            if (!inventoryResult.IsSuccess)
            {
                return ServiceResult<InventoryItem>.Failure("Failed to add inventory: " + inventoryResult.ErrorMessage);
            }
            
            // Step 3: Record transaction via TransactionService
            var transactionResult = await _transactionService.RecordTransactionAsync(
                partId, operation, quantity, "IN", location, userId, "Inventory Addition");
                
            if (!transactionResult.IsSuccess)
            {
                _logger.LogWarning("Inventory added but transaction recording failed: {Error}", transactionResult.ErrorMessage);
            }
            
            // Step 4: Notify other services via messaging
            var inventoryItem = new InventoryItem 
            { 
                PartId = partId, 
                Operation = operation, 
                Quantity = quantity, 
                Location = location 
            };
            
            _messenger.Send(new InventoryAddedMessage(inventoryItem, userId));
            
            return ServiceResult<InventoryItem>.Success(inventoryItem, "Inventory added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding inventory for {PartId} at {Operation}", partId, operation);
            return ServiceResult<InventoryItem>.Failure($"Unexpected error: {ex.Message}");
        }
    }
    
    private async Task<ServiceResult> ValidateInventoryDataAsync(string partId, string operation, string location)
    {
        // Coordinate with MasterDataService for validation
        var partExists = await _masterDataService.ValidatePartIdAsync(partId);
        if (!partExists)
        {
            return ServiceResult.Failure($"Part ID '{partId}' not found in master data");
        }
        
        var operationExists = await _masterDataService.ValidateOperationAsync(operation);
        if (!operationExists)
        {
            return ServiceResult.Failure($"Operation '{operation}' not found in master data");
        }
        
        var locationExists = await _masterDataService.ValidateLocationAsync(location);
        if (!locationExists)
        {
            return ServiceResult.Failure($"Location '{location}' not found in master data");
        }
        
        return ServiceResult.Success();
    }
}
```

### MVVM Community Toolkit Messaging Integration

```csharp
// Message definitions for service integration
public record InventoryAddedMessage(InventoryItem Item, string UserId);
public record InventoryRemovedMessage(string PartId, string Operation, int Quantity, string UserId);
public record TransactionCompletedMessage(TransactionRecord Transaction);
public record MasterDataUpdatedMessage(string DataType, string UpdatedBy);

// Service that sends messages
public class QuickButtonsService : IQuickButtonsService
{
    private readonly IDatabaseService _databaseService;
    private readonly IMessenger _messenger;
    
    public async Task<ServiceResult> ExecuteQuickActionAsync(QuickActionModel action, string userId)
    {
        try
        {
            // Execute the quick action
            var result = await _databaseService.ExecuteStoredProcedureAsync(
                "inv_inventory_Add_Item",
                new 
                { 
                    PartId = action.PartId, 
                    Operation = action.Operation, 
                    Quantity = action.Quantity, 
                    Location = action.Location, 
                    User = userId 
                });
            
            if (result.IsSuccess)
            {
                // Notify other services of the inventory change
                var inventoryItem = new InventoryItem
                {
                    PartId = action.PartId,
                    Operation = action.Operation,
                    Quantity = action.Quantity,
                    Location = action.Location
                };
                
                _messenger.Send(new InventoryAddedMessage(inventoryItem, userId));
                return ServiceResult.Success("Quick action executed successfully");
            }
            
            return ServiceResult.Failure("Quick action execution failed");
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Execute quick action");
            return ServiceResult.Failure($"Error executing quick action: {ex.Message}");
        }
    }
}

// Service that receives messages
public class AuditService : IAuditService, IRecipient<InventoryAddedMessage>, IRecipient<TransactionCompletedMessage>
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<AuditService> _logger;
    
    public AuditService(IDatabaseService databaseService, ILogger<AuditService> logger, IMessenger messenger)
    {
        _databaseService = databaseService;
        _logger = logger;
        
        // Register for messages
        messenger.Register<InventoryAddedMessage>(this);
        messenger.Register<TransactionCompletedMessage>(this);
    }
    
    public void Receive(InventoryAddedMessage message)
    {
        // Async fire-and-forget audit logging
        _ = Task.Run(async () =>
        {
            try
            {
                await LogInventoryChangeAsync(message.Item, "ADDED", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log inventory audit for {PartId}", message.Item.PartId);
            }
        });
    }
    
    public void Receive(TransactionCompletedMessage message)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await LogTransactionAuditAsync(message.Transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log transaction audit for {TransactionId}", message.Transaction.Id);
            }
        });
    }
    
    private async Task LogInventoryChangeAsync(InventoryItem item, string changeType, string userId)
    {
        await _databaseService.ExecuteStoredProcedureAsync(
            "log_audit_Add_InventoryChange",
            new 
            { 
                PartId = item.PartId, 
                Operation = item.Operation, 
                ChangeType = changeType, 
                Quantity = item.Quantity, 
                Location = item.Location, 
                User = userId, 
                Timestamp = DateTime.Now 
            });
    }
}
```

---

## 🏭 Manufacturing-Specific Service Integration

### Manufacturing Workflow Service Coordination

```csharp
// Manufacturing workflow coordination across multiple services
public class ManufacturingWorkflowService : IManufacturingWorkflowService
{
    private readonly IInventoryService _inventoryService;
    private readonly ITransactionService _transactionService;
    private readonly IQualityService _qualityService;
    private readonly IProductionService _productionService;
    private readonly IMessenger _messenger;
    
    public async Task<ServiceResult> ProcessManufacturingOperationAsync(
        string partId, string fromOperation, string toOperation, int quantity, string userId)
    {
        using var scope = _logger.BeginScope("Manufacturing operation {PartId} from {From} to {To}", 
            partId, fromOperation, toOperation);
            
        try
        {
            // Step 1: Validate manufacturing workflow transition
            var workflowResult = await ValidateWorkflowTransitionAsync(fromOperation, toOperation);
            if (!workflowResult.IsSuccess)
            {
                return workflowResult;
            }
            
            // Step 2: Check quality requirements
            var qualityResult = await _qualityService.ValidateOperationRequirementsAsync(
                partId, toOperation, quantity);
            if (!qualityResult.IsSuccess)
            {
                return ServiceResult.Failure($"Quality check failed: {qualityResult.ErrorMessage}");
            }
            
            // Step 3: Remove from current operation
            var removeResult = await _inventoryService.RemoveInventoryAsync(
                partId, fromOperation, quantity, GetWorkstationForOperation(fromOperation), userId);
            if (!removeResult.IsSuccess)
            {
                return ServiceResult.Failure($"Failed to remove from {fromOperation}: {removeResult.ErrorMessage}");
            }
            
            // Step 4: Add to next operation
            var addResult = await _inventoryService.AddInventoryAsync(
                partId, toOperation, quantity, GetWorkstationForOperation(toOperation), userId);
            if (!addResult.IsSuccess)
            {
                // Compensating transaction - add back to original operation
                await _inventoryService.AddInventoryAsync(
                    partId, fromOperation, quantity, GetWorkstationForOperation(fromOperation), userId);
                return ServiceResult.Failure($"Failed to add to {toOperation}: {addResult.ErrorMessage}");
            }
            
            // Step 5: Update production tracking
            await _productionService.RecordOperationCompletionAsync(
                partId, fromOperation, toOperation, quantity, userId);
            
            // Step 6: Notify workflow completion
            _messenger.Send(new ManufacturingOperationCompletedMessage(
                partId, fromOperation, toOperation, quantity, userId, DateTime.Now));
            
            return ServiceResult.Success($"Manufacturing operation completed: {partId} moved from {fromOperation} to {toOperation}");
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Manufacturing workflow operation");
            return ServiceResult.Failure($"Manufacturing operation failed: {ex.Message}");
        }
    }
    
    private async Task<ServiceResult> ValidateWorkflowTransitionAsync(string fromOperation, string toOperation)
    {
        // Manufacturing operation sequence validation
        var validTransitions = new Dictionary<string, string[]>
        {
            ["90"] = new[] { "100" },           // Receiving → First Operation
            ["100"] = new[] { "110", "90" },    // First → Second or back to Receiving
            ["110"] = new[] { "120", "100" },   // Second → Third or back to First
            ["120"] = new[] { "130", "110" },   // Third → Final or back to Second
            ["130"] = new[] { "140" }           // Final → Shipping
        };
        
        if (!validTransitions.ContainsKey(fromOperation))
        {
            return ServiceResult.Failure($"Invalid source operation: {fromOperation}");
        }
        
        if (!validTransitions[fromOperation].Contains(toOperation))
        {
            return ServiceResult.Failure(
                $"Invalid workflow transition from {fromOperation} to {toOperation}. " +
                $"Valid transitions: {string.Join(", ", validTransitions[fromOperation])}");
        }
        
        return ServiceResult.Success();
    }
    
    private string GetWorkstationForOperation(string operation) => operation switch
    {
        "90" => "RECEIVING_DOCK",
        "100" => "STATION_A",
        "110" => "STATION_B", 
        "120" => "STATION_C",
        "130" => "FINAL_ASSEMBLY",
        "140" => "SHIPPING_DOCK",
        _ => "UNKNOWN_STATION"
    };
}
```

### Manufacturing Event-Driven Integration

```csharp
// Manufacturing-specific messages
public record ManufacturingOperationCompletedMessage(
    string PartId, string FromOperation, string ToOperation, 
    int Quantity, string UserId, DateTime Timestamp);
    
public record QualityIssueDetectedMessage(
    string PartId, string Operation, string IssueType, 
    string Description, string DetectedBy, DateTime Timestamp);
    
public record ProductionMetricsUpdatedMessage(
    string Operation, int PartsProcessed, TimeSpan CycleTime, 
    double EfficiencyPercent, DateTime UpdateTime);

// Manufacturing dashboard service that aggregates data from multiple sources
public class ManufacturingDashboardService : IManufacturingDashboardService,
    IRecipient<ManufacturingOperationCompletedMessage>,
    IRecipient<QualityIssueDetectedMessage>,
    IRecipient<ProductionMetricsUpdatedMessage>
{
    private readonly IDashboardDataService _dashboardDataService;
    private readonly ILogger<ManufacturingDashboardService> _logger;
    private readonly ConcurrentDictionary<string, ProductionMetrics> _operationMetrics;
    
    public ManufacturingDashboardService(
        IDashboardDataService dashboardDataService, 
        ILogger<ManufacturingDashboardService> logger,
        IMessenger messenger)
    {
        _dashboardDataService = dashboardDataService;
        _logger = logger;
        _operationMetrics = new ConcurrentDictionary<string, ProductionMetrics>();
        
        // Register for manufacturing events
        messenger.Register<ManufacturingOperationCompletedMessage>(this);
        messenger.Register<QualityIssueDetectedMessage>(this);
        messenger.Register<ProductionMetricsUpdatedMessage>(this);
    }
    
    public void Receive(ManufacturingOperationCompletedMessage message)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Update real-time production metrics
                await UpdateProductionMetricsAsync(message);
                
                // Update dashboard displays
                await _dashboardDataService.UpdateOperationStatusAsync(
                    message.FromOperation, message.ToOperation, message.Quantity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update dashboard for manufacturing operation completion");
            }
        });
    }
    
    public void Receive(QualityIssueDetectedMessage message)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Alert quality team
                await _dashboardDataService.AddQualityAlertAsync(message);
                
                // Update quality metrics
                await UpdateQualityMetricsAsync(message.Operation, message.IssueType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process quality issue alert");
            }
        });
    }
    
    private async Task UpdateProductionMetricsAsync(ManufacturingOperationCompletedMessage message)
    {
        _operationMetrics.AddOrUpdate(message.FromOperation, 
            new ProductionMetrics { PartsCompleted = message.Quantity, LastUpdate = DateTime.Now },
            (key, existing) => 
            {
                existing.PartsCompleted += message.Quantity;
                existing.LastUpdate = DateTime.Now;
                return existing;
            });
            
        await _dashboardDataService.UpdateMetricsAsync(_operationMetrics[message.FromOperation]);
    }
}
```

---

## 🔧 Error Handling and Resilience Patterns

### Cross-Service Error Propagation

```csharp
// Service result pattern with error context propagation
public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; }
    public Exception? Exception { get; }
    public string? ServiceContext { get; }
    public List<string> ErrorChain { get; }
    
    private ServiceResult(bool isSuccess, T? data, string errorMessage, 
        Exception? exception, string? serviceContext, List<string>? errorChain = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
        Exception = exception;
        ServiceContext = serviceContext;
        ErrorChain = errorChain ?? new List<string>();
    }
    
    public static ServiceResult<T> Success(T data, string? message = null) =>
        new(true, data, message ?? "Success", null, null);
        
    public static ServiceResult<T> Failure(string errorMessage, Exception? exception = null, string? serviceContext = null) =>
        new(false, default, errorMessage, exception, serviceContext);
        
    // Chain error from another service result
    public static ServiceResult<T> ChainError<TOther>(ServiceResult<TOther> previousResult, string currentServiceContext)
    {
        var errorChain = new List<string>(previousResult.ErrorChain);
        if (!string.IsNullOrEmpty(previousResult.ServiceContext))
        {
            errorChain.Add($"{previousResult.ServiceContext}: {previousResult.ErrorMessage}");
        }
        
        return new ServiceResult<T>(
            false, 
            default, 
            previousResult.ErrorMessage, 
            previousResult.Exception, 
            currentServiceContext, 
            errorChain);
    }
}

// Service with error context propagation
public class InventoryWorkflowService : IInventoryWorkflowService
{
    private readonly IInventoryService _inventoryService;
    private readonly ITransactionService _transactionService;
    
    public async Task<ServiceResult<WorkflowResult>> ProcessInventoryWorkflowAsync(
        WorkflowRequest request, string userId)
    {
        try
        {
            // Step 1: Validate inventory
            var inventoryResult = await _inventoryService.ValidateInventoryAsync(request.PartId, request.Operation);
            if (!inventoryResult.IsSuccess)
            {
                return ServiceResult<WorkflowResult>.ChainError(inventoryResult, "InventoryWorkflowService");
            }
            
            // Step 2: Process transaction
            var transactionResult = await _transactionService.ProcessTransactionAsync(
                request.PartId, request.Operation, request.Quantity, userId);
            if (!transactionResult.IsSuccess)
            {
                return ServiceResult<WorkflowResult>.ChainError(transactionResult, "InventoryWorkflowService");
            }
            
            return ServiceResult<WorkflowResult>.Success(
                new WorkflowResult { Success = true, Message = "Workflow completed successfully" });
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Inventory workflow processing");
            return ServiceResult<WorkflowResult>.Failure(
                "Workflow processing failed due to unexpected error", ex, "InventoryWorkflowService");
        }
    }
}
```

### Circuit Breaker Pattern for Service Dependencies

```csharp
// Circuit breaker for external service dependencies
public class CircuitBreakerService<T> : ICircuitBreakerService<T>
{
    private readonly string _serviceName;
    private readonly TimeSpan _timeout;
    private readonly int _failureThreshold;
    private readonly TimeSpan _recoveryTimeout;
    private readonly ILogger _logger;
    
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private CircuitState _state = CircuitState.Closed;
    private readonly object _lock = new object();
    
    public async Task<ServiceResult<T>> ExecuteAsync<TResult>(
        Func<Task<ServiceResult<TResult>>> operation, string operationName) where TResult : T
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.Now - _lastFailureTime > _recoveryTimeout)
                {
                    _state = CircuitState.HalfOpen;
                    _logger.LogInformation("Circuit breaker for {Service} entering half-open state", _serviceName);
                }
                else
                {
                    return ServiceResult<T>.Failure(
                        $"Circuit breaker is open for {_serviceName}. Service temporarily unavailable.",
                        serviceContext: $"CircuitBreaker-{_serviceName}");
                }
            }
        }
        
        try
        {
            using var cts = new CancellationTokenSource(_timeout);
            var result = await operation().ConfigureAwait(false);
            
            if (result.IsSuccess)
            {
                OnSuccess();
                return ServiceResult<T>.Success((T)(object)result.Data!, $"{operationName} completed successfully");
            }
            else
            {
                OnFailure();
                return ServiceResult<T>.ChainError(result, $"CircuitBreaker-{_serviceName}");
            }
        }
        catch (Exception ex)
        {
            OnFailure();
            _logger.LogError(ex, "Circuit breaker operation failed for {Service}.{Operation}", _serviceName, operationName);
            return ServiceResult<T>.Failure(
                $"Operation {operationName} failed in {_serviceName}", ex, $"CircuitBreaker-{_serviceName}");
        }
    }
    
    private void OnSuccess()
    {
        lock (_lock)
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
    }
    
    private void OnFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _lastFailureTime = DateTime.Now;
            
            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitState.Open;
                _logger.LogWarning("Circuit breaker opened for {Service} after {Failures} failures", 
                    _serviceName, _failureCount);
            }
        }
    }
}

public enum CircuitState
{
    Closed,    // Normal operation
    Open,      // Failing fast
    HalfOpen   // Testing recovery
}
```

---

## 🧪 Service Integration Testing

### Mock Service Setup for Integration Testing

```csharp
// Mock services for integration testing
public class MockDatabaseService : IDatabaseService
{
    private readonly Dictionary<string, object> _mockData = new();
    private readonly List<StoredProcedureCall> _procedureCalls = new();
    
    public Task<DatabaseResult> ExecuteStoredProcedureAsync(string procedureName, object parameters)
    {
        _procedureCalls.Add(new StoredProcedureCall(procedureName, parameters, DateTime.Now));
        
        // Return mock data based on procedure name
        return procedureName switch
        {
            "inv_inventory_Get_ByPartID" => Task.FromResult(CreateMockInventoryResult(parameters)),
            "inv_transaction_Add" => Task.FromResult(DatabaseResult.Success()),
            _ => Task.FromResult(DatabaseResult.Failure($"Unknown procedure: {procedureName}"))
        };
    }
    
    public List<StoredProcedureCall> GetProcedureCalls() => _procedureCalls.ToList();
    
    public void SetMockData(string key, object data) => _mockData[key] = data;
    
    private DatabaseResult CreateMockInventoryResult(object parameters)
    {
        // Create mock DataTable result
        var dataTable = new DataTable();
        dataTable.Columns.Add("PartID", typeof(string));
        dataTable.Columns.Add("Operation", typeof(string));
        dataTable.Columns.Add("Quantity", typeof(int));
        
        // Add mock data row
        var row = dataTable.NewRow();
        row["PartID"] = "MOCK_PART";
        row["Operation"] = "100";
        row["Quantity"] = 50;
        dataTable.Rows.Add(row);
        
        return DatabaseResult.Success(dataTable);
    }
}

public record StoredProcedureCall(string ProcedureName, object Parameters, DateTime CalledAt);

// Integration test example
[TestFixture]
[Category("Integration")]
public class ServiceIntegrationTests
{
    private MockDatabaseService _mockDatabaseService;
    private Mock<IMessenger> _mockMessenger;
    private InventoryService _inventoryService;
    private TransactionService _transactionService;
    
    [SetUp]
    public void SetUp()
    {
        _mockDatabaseService = new MockDatabaseService();
        _mockMessenger = new Mock<IMessenger>();
        
        var mockLogger = new Mock<ILogger<InventoryService>>();
        var mockTransactionLogger = new Mock<ILogger<TransactionService>>();
        
        _transactionService = new TransactionService(_mockDatabaseService, mockTransactionLogger.Object);
        _inventoryService = new InventoryService(
            _mockDatabaseService, 
            _transactionService, 
            Mock.Of<IMasterDataService>(), 
            _mockMessenger.Object, 
            mockLogger.Object);
    }
    
    [Test]
    public async Task InventoryService_AddInventory_IntegratesWithTransactionService()
    {
        // Act
        var result = await _inventoryService.AddInventoryAsync("TEST001", "100", 10, "STATION_A", "TestUser");
        
        // Assert - Verify service integration
        Assert.That(result.IsSuccess, Is.True);
        
        // Verify database calls were made
        var calls = _mockDatabaseService.GetProcedureCalls();
        Assert.That(calls.Count, Is.GreaterThanOrEqualTo(2)); // Inventory + Transaction
        Assert.That(calls.Any(c => c.ProcedureName == "inv_inventory_Add_Item"), Is.True);
        Assert.That(calls.Any(c => c.ProcedureName == "inv_transaction_Add"), Is.True);
        
        // Verify messaging integration
        _mockMessenger.Verify(m => m.Send(It.IsAny<InventoryAddedMessage>()), Times.Once);
    }
    
    [Test]
    public async Task ServiceIntegration_DatabaseFailure_PropagatesErrorCorrectly()
    {
        // Arrange - Force database failure
        _mockDatabaseService.SetMockData("forceFailure", true);
        
        // Act
        var result = await _inventoryService.AddInventoryAsync("FAIL001", "100", 10, "STATION_A", "TestUser");
        
        // Assert - Verify error handling
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("Failed to add inventory"));
        
        // Verify no message was sent on failure
        _mockMessenger.Verify(m => m.Send(It.IsAny<InventoryAddedMessage>()), Times.Never);
    }
}
```

### End-to-End Service Testing

```csharp
// End-to-end test for complete manufacturing workflow
[TestFixture]
[Category("Integration")]
[Category("EndToEnd")]
public class ManufacturingWorkflowIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private TestServiceScope _scope;
    
    [SetUp]
    public async Task SetUp()
    {
        var services = new ServiceCollection();
        
        // Register real services with test configuration
        services.AddSingleton<IConfiguration>(CreateTestConfiguration());
        services.AddMTMServices(CreateTestConfiguration());
        
        // Override database service with test implementation
        services.AddScoped<IDatabaseService, TestDatabaseService>();
        
        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
        
        // Initialize test data
        await SeedTestDataAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await CleanupTestDataAsync();
        _scope?.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
    }
    
    [Test]
    public async Task ManufacturingWorkflow_CompletePartProcessing_IntegratesAllServices()
    {
        // Arrange
        var workflowService = _scope.ServiceProvider.GetRequiredService<IManufacturingWorkflowService>();
        var partId = "E2E_TEST_001";
        var userId = "IntegrationTestUser";
        
        // Act - Process part through complete manufacturing workflow
        var step1 = await workflowService.ProcessManufacturingOperationAsync(
            partId, "90", "100", 50, userId); // Receiving to First Operation
            
        var step2 = await workflowService.ProcessManufacturingOperationAsync(
            partId, "100", "110", 50, userId); // First to Second Operation
            
        var step3 = await workflowService.ProcessManufacturingOperationAsync(
            partId, "110", "120", 50, userId); // Second to Third Operation
        
        // Assert - Verify all steps succeeded
        Assert.That(step1.IsSuccess, Is.True, $"Step 1 failed: {step1.ErrorMessage}");
        Assert.That(step2.IsSuccess, Is.True, $"Step 2 failed: {step2.ErrorMessage}");
        Assert.That(step3.IsSuccess, Is.True, $"Step 3 failed: {step3.ErrorMessage}");
        
        // Verify final state
        var inventoryService = _scope.ServiceProvider.GetRequiredService<IInventoryService>();
        var finalInventory = await inventoryService.GetInventoryAsync(partId, "120");
        
        Assert.That(finalInventory.IsSuccess, Is.True);
        Assert.That(finalInventory.Data?.Quantity, Is.EqualTo(50));
        
        // Verify transaction history
        var transactionService = _scope.ServiceProvider.GetRequiredService<ITransactionService>();
        var history = await transactionService.GetTransactionHistoryAsync(partId);
        
        Assert.That(history.IsSuccess, Is.True);
        Assert.That(history.Data?.Count(), Is.GreaterThanOrEqualTo(6)); // 2 transactions per step (OUT + IN)
    }
    
    private IConfiguration CreateTestConfiguration()
    {
        var configData = new Dictionary<string, string>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=mtm_integration_test;Uid=test;Pwd=test;",
            ["MTMSettings:DefaultOperation"] = "90",
            ["MTMSettings:EnableAutoSave"] = "true"
        };
        
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
    }
    
    private async Task SeedTestDataAsync()
    {
        var databaseService = _scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        
        // Seed master data required for testing
        await databaseService.ExecuteStoredProcedureAsync("md_part_ids_Add", 
            new { PartID = "E2E_TEST_001", Description = "Integration test part", User = "System" });
            
        await databaseService.ExecuteStoredProcedureAsync("inv_inventory_Add_Item",
            new { PartID = "E2E_TEST_001", OperationNumber = "90", Quantity = 50, Location = "RECEIVING", User = "System" });
    }
}
```

---

## 📚 Related Documentation

- **Database Integration**: [MySQL Database Patterns](./mysql-database-patterns.instructions.md)
- **MVVM Messaging**: [MVVM Community Toolkit](./mvvm-community-toolkit.instructions.md)
- **Service Architecture**: [Service Layer Patterns](./service-architecture.instructions.md)
- **Configuration Management**: [Application Configuration](./application-configuration.instructions.md)

---

**Document Status**: ✅ Complete Service Integration Reference  
**Framework Version**: .NET 8 with Microsoft Extensions  
**Last Updated**: 2025-09-14  
**Service Integration Owner**: MTM Development Team