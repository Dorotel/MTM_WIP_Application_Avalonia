---
description: 'Complete testing implementation template for MTM WIP Application components'
applies_to: '**/*.cs'
---

# MTM Testing Implementation Template

## 🧪 Testing Development Instructions

Comprehensive testing patterns for MTM WIP Application components following manufacturing-grade quality standards.

## Unit Testing Pattern (MVVM Community Toolkit)

### ViewModel Testing Template
```csharp
[TestFixture]
[Category("Unit")]
[Category("ViewModel")]
public class [ComponentName]ViewModelTests
{
    private Mock<ILogger<[ComponentName]ViewModel>> _mockLogger;
    private Mock<I[Service]Service> _mockService;
    private [ComponentName]ViewModel _viewModel;
    
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<[ComponentName]ViewModel>>();
        _mockService = new Mock<I[Service]Service>();
        
        _viewModel = new [ComponentName]ViewModel(
            _mockLogger.Object,
            _mockService.Object);
    }
    
    [TearDown]
    public void TearDown()
    {
        _viewModel?.Dispose();
    }
    
    [Test]
    public void Constructor_ValidParameters_InitializesCorrectly()
    {
        // Assert - Constructor should initialize without exceptions
        Assert.That(_viewModel, Is.Not.Null);
        Assert.That(_viewModel.IsLoading, Is.False);
        Assert.That(_viewModel.ErrorMessage, Is.Empty);
    }
    
    [Test]
    public void [Property]_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        var propertyChangedEvents = new List<string>();
        _viewModel.PropertyChanged += (s, e) => propertyChangedEvents.Add(e.PropertyName);
        
        // Act
        _viewModel.[Property] = "[TestValue]";
        
        // Assert
        Assert.That(propertyChangedEvents, Contains.Item(nameof(_viewModel.[Property])));
    }
    
    [Test]
    public async Task [Command]Command_ValidData_ShouldCallService()
    {
        // Arrange
        var testData = new [DataType] { /* test data */ };
        _mockService.Setup(s => s.[Method]Async(It.IsAny<[DataType]>()))
                   .ReturnsAsync(ServiceResult.Success());
        
        // Act
        await _viewModel.[Command]Command.ExecuteAsync(null);
        
        // Assert
        _mockService.Verify(s => s.[Method]Async(It.IsAny<[DataType]>()), Times.Once);
    }
    
    [Test]
    public void [Command]Command_InvalidData_ShouldNotExecute()
    {
        // Arrange - Set invalid state
        _viewModel.[Property] = string.Empty; // Invalid value
        
        // Act & Assert
        Assert.That(_viewModel.[Command]Command.CanExecute(null), Is.False);
    }
    
    [TestCase("", false)]
    [TestCase("ValidValue", true)]
    [TestCase(null, false)]
    public void [Command]Command_CanExecute_ValidatesData(string value, bool expectedCanExecute)
    {
        // Arrange
        _viewModel.[Property] = value;
        
        // Act & Assert
        Assert.That(_viewModel.[Command]Command.CanExecute(null), Is.EqualTo(expectedCanExecute));
    }
}
```

## Service Testing Pattern (Database Integration)

### Service Testing Template
```csharp
[TestFixture]
[Category("Unit")]
[Category("Service")]
public class [Service]ServiceTests
{
    private Mock<IDatabaseService> _mockDatabaseService;
    private Mock<ILogger<[Service]Service>> _mockLogger;
    private [Service]Service _service;
    
    [SetUp]
    public void SetUp()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<[Service]Service>>();
        
        _service = new [Service]Service(_mockDatabaseService.Object, _mockLogger.Object);
    }
    
    [Test]
    public async Task [Method]Async_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var inputData = new [DataType] { /* test data */ };
        var mockResult = new DatabaseResult
        {
            Status = 1,
            Data = new DataTable(),
            Message = "Success"
        };
        
        _mockDatabaseService.Setup(db => db.ExecuteStoredProcedureAsync(
            It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
            .ReturnsAsync(mockResult);
        
        // Act
        var result = await _service.[Method]Async(inputData);
        
        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.ErrorMessage, Is.Empty);
        
        // Verify database call
        _mockDatabaseService.Verify(db => db.ExecuteStoredProcedureAsync(
            "[stored_procedure_name]", 
            It.Is<MySqlParameter[]>(p => ValidateParameters(p, inputData))), 
            Times.Once);
    }
    
    [Test]
    public async Task [Method]Async_DatabaseError_ReturnsError()
    {
        // Arrange
        var inputData = new [DataType] { /* test data */ };
        var mockResult = new DatabaseResult
        {
            Status = -1,
            Data = null,
            Message = "Database connection failed"
        };
        
        _mockDatabaseService.Setup(db => db.ExecuteStoredProcedureAsync(
            It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
            .ReturnsAsync(mockResult);
        
        // Act
        var result = await _service.[Method]Async(inputData);
        
        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Database connection failed"));
    }
    
    [TestCase(null)]
    [TestCase("")]
    public async Task [Method]Async_InvalidInput_ReturnsValidationError(string invalidValue)
    {
        // Arrange
        var inputData = new [DataType] { [Property] = invalidValue };
        
        // Act
        var result = await _service.[Method]Async(inputData);
        
        // Assert
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Does.Contain("validation").IgnoreCase);
    }
    
    private bool ValidateParameters(MySqlParameter[] parameters, [DataType] expectedData)
    {
        // Validate that parameters match expected data
        return parameters.Any(p => p.ParameterName == "p_[Property]" && 
                                  p.Value.ToString() == expectedData.[Property]);
    }
}
```

## Integration Testing Pattern (Cross-Service)

### Integration Testing Template
```csharp
[TestFixture]
[Category("Integration")]
[Category("CrossService")]
public class [Feature]IntegrationTests
{
    private IServiceProvider _serviceProvider;
    private DatabaseTestFixture _databaseFixture;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _databaseFixture = new DatabaseTestFixture();
        await _databaseFixture.SetupAsync();
        
        var services = new ServiceCollection();
        services.AddMTMServices(_databaseFixture.Configuration);
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _serviceProvider?.Dispose();
        await _databaseFixture?.TearDownAsync();
    }
    
    [Test]
    public async Task [Feature]_EndToEndWorkflow_ShouldCompleteSuccessfully()
    {
        // Arrange
        var [service1] = _serviceProvider.GetRequiredService<I[Service1]Service>();
        var [service2] = _serviceProvider.GetRequiredService<I[Service2]Service>();
        
        var testData = new [DataType] { /* integration test data */ };
        
        // Act - Execute complete workflow
        var step1Result = await [service1].[Method1]Async(testData);
        Assert.That(step1Result.IsSuccess, Is.True, "Step 1 should complete successfully");
        
        var step2Result = await [service2].[Method2]Async(step1Result.Data);
        Assert.That(step2Result.IsSuccess, Is.True, "Step 2 should complete successfully");
        
        // Assert - Verify end-to-end state
        var finalState = await [service1].[VerifyMethod]Async(testData.Id);
        Assert.That(finalState.Data.[Property], Is.EqualTo(expectedFinalValue));
    }
}
```

## UI Testing Pattern (Avalonia)

### UI Testing Template
```csharp
[TestFixture]
[Category("UI")]
[Category("Integration")]
public class [Component]UITests
{
    private TestAppBuilder _appBuilder;
    private Window _testWindow;
    
    [SetUp]
    public void SetUp()
    {
        _appBuilder = AppBuilder.Configure<App>()
                               .UseHeadless()
                               .SetupWithoutStarting();
    }
    
    [Test]
    public async Task [Component]_LoadsCorrectly_DisplaysExpectedElements()
    {
        // Arrange
        using var app = _appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        var window = new Window();
        var component = new [Component]Control();
        window.Content = component;
        
        // Act
        window.Show();
        await Task.Delay(100); // Allow rendering
        
        // Assert
        Assert.That(component.IsVisible, Is.True);
        Assert.That(component.FindControl<TextBlock>("[ElementName]"), Is.Not.Null);
    }
    
    [Test]
    public async Task [Component]_UserInteraction_TriggersExpectedBehavior()
    {
        // Arrange
        using var app = _appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        var component = new [Component]Control();
        var viewModel = new Mock<[Component]ViewModel>();
        component.DataContext = viewModel.Object;
        
        // Act
        var button = component.FindControl<Button>("[ButtonName]");
        button?.Command?.Execute(null);
        
        // Assert
        viewModel.Verify(vm => vm.[ExpectedMethod](), Times.Once);
    }
}
```

## Performance Testing Pattern

### Performance Testing Template
```csharp
[TestFixture]
[Category("Performance")]
public class [Component]PerformanceTests
{
    [Test]
    public async Task [Method]_HighVolume_CompletesWithinTimeLimit()
    {
        // Arrange
        var service = new [Service]Service(/* dependencies */);
        var testData = GenerateTestData(1000); // High volume
        var stopwatch = Stopwatch.StartNew();
        
        // Act
        var tasks = testData.Select(data => service.[Method]Async(data));
        var results = await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        
        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(5000), 
            "1000 operations should complete within 5 seconds");
        Assert.That(results.Count(r => r.IsSuccess), Is.GreaterThan(950),
            "At least 95% of operations should succeed");
    }
    
    [Test]
    public async Task [Method]_MemoryUsage_StaysWithinLimits()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        var service = new [Service]Service(/* dependencies */);
        
        // Act
        for (int i = 0; i < 1000; i++)
        {
            await service.[Method]Async(/* test data */);
            
            if (i % 100 == 0)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        
        var finalMemory = GC.GetTotalMemory(true);
        var memoryIncrease = finalMemory - initialMemory;
        
        // Assert
        Assert.That(memoryIncrease, Is.LessThan(10_000_000), 
            "Memory increase should be less than 10MB for 1000 operations");
    }
}
```

## Manufacturing Domain Testing

### Manufacturing Workflow Testing
```csharp
[TestFixture]
[Category("Manufacturing")]
public class ManufacturingWorkflowTests
{
    [Test]
    public async Task InventoryTransaction_ValidWorkflow_MaintainsDataIntegrity()
    {
        // Arrange - Manufacturing scenario
        var partId = "MFGTEST001";
        var operation = "100"; // Manufacturing operation step
        var quantity = 50;
        
        var inventoryService = new InventoryService(/* dependencies */);
        var transactionService = new TransactionService(/* dependencies */);
        
        // Act - Complete manufacturing workflow
        var addResult = await inventoryService.AddInventoryAsync(partId, operation, quantity, "STATION_A");
        Assert.That(addResult.IsSuccess, Is.True, "Add inventory should succeed");
        
        var removeResult = await inventoryService.RemoveInventoryAsync(partId, operation, 20, "STATION_A");
        Assert.That(removeResult.IsSuccess, Is.True, "Remove inventory should succeed");
        
        // Assert - Verify manufacturing data integrity
        var currentInventory = await inventoryService.GetInventoryAsync(partId, operation);
        Assert.That(currentInventory.Quantity, Is.EqualTo(30), "Inventory should reflect correct quantity");
        
        var transactionHistory = await transactionService.GetTransactionHistoryAsync(partId, operation);
        Assert.That(transactionHistory.Count, Is.EqualTo(2), "Should have both add and remove transactions");
        Assert.That(transactionHistory[0].TransactionType, Is.EqualTo("IN"), "First transaction should be IN");
        Assert.That(transactionHistory[1].TransactionType, Is.EqualTo("OUT"), "Second transaction should be OUT");
    }
}
```

## Test Coverage Requirements

### Coverage Targets
- **Unit Tests**: 95%+ code coverage
- **Integration Tests**: 90%+ service interaction coverage
- **UI Tests**: 100% critical user workflows
- **Performance Tests**: All high-volume scenarios
- **Manufacturing Tests**: 100% business workflow coverage

### Critical Test Scenarios
1. **Constructor Validation** - All dependency injection
2. **Property Change Notifications** - All [ObservableProperty]
3. **Command Execution** - All [RelayCommand] with success/error paths
4. **Service Integration** - All external service calls
5. **Database Operations** - All stored procedure calls
6. **Error Handling** - All exception scenarios
7. **Manufacturing Workflows** - Complete business processes

This comprehensive testing template ensures manufacturing-grade quality and reliability for all MTM WIP Application components.