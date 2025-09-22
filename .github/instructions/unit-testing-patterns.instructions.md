---
description: 'Unit testing patterns for MTM ViewModels and Services using MVVM Community Toolkit'
applies_to: '**/*.cs'
---

# MTM Unit Testing Patterns Instructions

## 🎯 Overview

Comprehensive unit testing patterns for MTM WIP Application components, focusing on MVVM Community Toolkit ViewModels, dependency-injected Services, and manufacturing domain logic.

## 🧪 ViewModel Testing Patterns

### MVVM Community Toolkit ViewModel Tests

**Template Pattern for [ObservableObject] ViewModels:**

```csharp
[TestFixture]
[Category("Unit")]
[Category("ViewModel")]
public class InventoryTabViewModelTests
{
    private Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private Mock<IInventoryService> _mockInventoryService; 
    private Mock<IMasterDataService> _mockMasterDataService;
    private InventoryTabViewModel _viewModel;
    
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        
        _viewModel = new InventoryTabViewModel(
            _mockLogger.Object,
            _mockInventoryService.Object,
            _mockMasterDataService.Object);
    }
    
    [TearDown]
    public void TearDown()
    {
        _viewModel?.Dispose();
    }
}
```

### [ObservableProperty] Testing

```csharp
[Test]
public void PartId_WhenSet_ShouldRaisePropertyChanged()
{
    // Arrange
    var propertyChangedEvents = new List<string>();
    _viewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);
    
    // Act
    _viewModel.PartId = "TEST001";
    
    // Assert
    Assert.That(_viewModel.PartId, Is.EqualTo("TEST001"));
    Assert.That(propertyChangedEvents, Contains.Item(nameof(InventoryTabViewModel.PartId)));
}

[Test]
public void IsLoading_WhenToggled_ShouldUpdateCanExecuteStates()
{
    // Arrange - Get initial CanExecute states
    var initialSaveCanExecute = _viewModel.SaveCommand.CanExecute(null);
    var initialLoadCanExecute = _viewModel.LoadPartIdsCommand.CanExecute(null);
    
    // Act - Toggle loading state
    _viewModel.IsLoading = true;
    
    // Assert - Commands should be disabled during loading
    Assert.That(_viewModel.IsLoading, Is.True);
    Assert.That(_viewModel.SaveCommand.CanExecute(null), Is.False);
    Assert.That(_viewModel.LoadPartIdsCommand.CanExecute(null), Is.False);
    
    // Act - Reset loading state  
    _viewModel.IsLoading = false;
    
    // Assert - Commands should be re-enabled
    Assert.That(_viewModel.SaveCommand.CanExecute(null), Is.EqualTo(initialSaveCanExecute));
    Assert.That(_viewModel.LoadPartIdsCommand.CanExecute(null), Is.EqualTo(initialLoadCanExecute));
}
```

### [RelayCommand] Testing

```csharp
[Test]
public async Task SaveCommand_ValidData_ShouldCallService()
{
    // Arrange
    _viewModel.PartId = "TEST001";
    _viewModel.Operation = "100";
    _viewModel.Quantity = 10;
    _viewModel.Location = "STATION_A";
    
    _mockInventoryService
        .Setup(s => s.SaveInventoryAsync(It.IsAny<InventoryItem>()))
        .ReturnsAsync(ServiceResult.Success("Item saved successfully"));
    
    // Act
    await _viewModel.SaveCommand.ExecuteAsync(null);
    
    // Assert
    _mockInventoryService.Verify(s => s.SaveInventoryAsync(
        It.Is<InventoryItem>(item => 
            item.PartId == "TEST001" &&
            item.Operation == "100" &&
            item.Quantity == 10 &&
            item.Location == "STATION_A")), Times.Once);
}

[Test]
public async Task SaveCommand_ServiceFailure_ShouldHandleError()
{
    // Arrange
    _viewModel.PartId = "TEST001";
    var serviceError = ServiceResult.Error("Database connection failed");
    
    _mockInventoryService
        .Setup(s => s.SaveInventoryAsync(It.IsAny<InventoryItem>()))
        .ReturnsAsync(serviceError);
    
    // Act
    await _viewModel.SaveCommand.ExecuteAsync(null);
    
    // Assert - Verify error handling
    Assert.That(_viewModel.IsLoading, Is.False, "Loading should be reset after error");
    Assert.That(_viewModel.ErrorMessage, Is.Not.Empty, "Error message should be set");
    
    // Verify error logging
    _mockLogger.Verify(
        l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                   It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}

[Test]
public async Task SaveCommand_Exception_ShouldHandleGracefully()
{
    // Arrange
    _viewModel.PartId = "TEST001";
    var exception = new InvalidOperationException("Service unavailable");
    
    _mockInventoryService
        .Setup(s => s.SaveInventoryAsync(It.IsAny<InventoryItem>()))
        .ThrowsAsync(exception);
    
    // Act & Assert - Should not throw
    Assert.DoesNotThrowAsync(async () => await _viewModel.SaveCommand.ExecuteAsync(null));
    
    // Assert - Error handling
    Assert.That(_viewModel.IsLoading, Is.False);
    Assert.That(_viewModel.ErrorMessage, Is.Not.Empty);
    
    // Verify exception logging
    _mockLogger.Verify(
        l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                   It.Is<Exception>(ex => ex == exception),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### CanExecute Logic Testing

```csharp
[Test]
public void SaveCommand_EmptyPartId_ShouldNotExecute()
{
    // Arrange
    _viewModel.PartId = string.Empty;
    _viewModel.Operation = "100";
    _viewModel.Quantity = 10;
    
    // Act & Assert
    Assert.That(_viewModel.SaveCommand.CanExecute(null), Is.False);
}

[TestCase("", "100", 10, false)]
[TestCase("TEST001", "", 10, false)]
[TestCase("TEST001", "100", 0, false)]
[TestCase("TEST001", "100", -5, false)]
[TestCase("TEST001", "100", 10, true)]
public void SaveCommand_CanExecute_ValidatesRequiredFields(
    string partId, string operation, int quantity, bool expectedCanExecute)
{
    // Arrange
    _viewModel.PartId = partId;
    _viewModel.Operation = operation;
    _viewModel.Quantity = quantity;
    
    // Act & Assert
    Assert.That(_viewModel.SaveCommand.CanExecute(null), Is.EqualTo(expectedCanExecute));
}
```

### Collection Property Testing

```csharp
[Test]
public async Task LoadPartIdsCommand_Success_ShouldPopulateCollection()
{
    // Arrange
    var partIds = new List<string> { "PART001", "PART002", "PART003" };
    _mockMasterDataService
        .Setup(s => s.GetPartIdsAsync())
        .ReturnsAsync(ServiceResult.Success(partIds));
    
    // Act
    await _viewModel.LoadPartIdsCommand.ExecuteAsync(null);
    
    // Assert
    Assert.That(_viewModel.PartIds, Is.Not.Null);
    Assert.That(_viewModel.PartIds.Count, Is.EqualTo(3));
    CollectionAssert.AreEqual(partIds, _viewModel.PartIds);
}

[Test]
public async Task LoadPartIdsCommand_EmptyResult_ShouldClearCollection()
{
    // Arrange
    _viewModel.PartIds.Add("EXISTING001");
    _mockMasterDataService
        .Setup(s => s.GetPartIdsAsync())
        .ReturnsAsync(ServiceResult.Success(new List<string>()));
    
    // Act
    await _viewModel.LoadPartIdsCommand.ExecuteAsync(null);
    
    // Assert
    Assert.That(_viewModel.PartIds, Is.Not.Null);
    Assert.That(_viewModel.PartIds.Count, Is.EqualTo(0));
}
```

## 🛠️ Service Testing Patterns

### Service Constructor Testing

```csharp
[TestFixture]
[Category("Unit")]
[Category("Service")]
public class InventoryServiceTests
{
    private Mock<ILogger<InventoryService>> _mockLogger;
    private Mock<IConfigurationService> _mockConfigService;
    private InventoryService _service;
    private string _connectionString;
    
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<InventoryService>>();
        _mockConfigService = new Mock<IConfigurationService>();
        _connectionString = "Server=localhost;Database=test;";
        
        _mockConfigService
            .Setup(c => c.GetConnectionString())
            .Returns(_connectionString);
            
        _service = new InventoryService(_mockLogger.Object, _mockConfigService.Object);
    }
    
    [Test]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            new InventoryService(null, _mockConfigService.Object));
    }
    
    [Test]  
    public void Constructor_NullConfigService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new InventoryService(_mockLogger.Object, null));
    }
}
```

### Database Service Method Testing

```csharp
[Test]
public async Task SaveInventoryAsync_ValidItem_ReturnsSuccess()
{
    // Arrange
    var inventoryItem = new InventoryItem
    {
        PartId = "TEST001",
        Operation = "100", 
        Quantity = 10,
        Location = "STATION_A",
        User = "TestUser"
    };
    
    // Mock successful database response
    var mockResult = new DatabaseResult
    {
        Status = 1,
        Data = new DataTable(),
        Message = "Success"
    };
    
    // Use TestHelper to mock stored procedure call
    DatabaseTestHelper.MockStoredProcedureCall(
        "inv_inventory_Add_Item", 
        mockResult);
    
    // Act
    var result = await _service.SaveInventoryAsync(inventoryItem);
    
    // Assert
    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Message, Is.EqualTo("Inventory item saved successfully"));
    
    // Verify stored procedure was called with correct parameters
    DatabaseTestHelper.VerifyStoredProcedureCall(
        "inv_inventory_Add_Item",
        parameters =>
            parameters["p_PartID"].Value.ToString() == "TEST001" &&
            parameters["p_OperationNumber"].Value.ToString() == "100" &&
            parameters["p_Quantity"].Value.ToString() == "10");
}

[Test]
public async Task SaveInventoryAsync_DatabaseError_ReturnsError()
{
    // Arrange
    var inventoryItem = new InventoryItem { PartId = "TEST001" };
    
    var mockResult = new DatabaseResult
    {
        Status = -1,
        Data = null,
        Message = "Database connection failed"
    };
    
    DatabaseTestHelper.MockStoredProcedureCall("inv_inventory_Add_Item", mockResult);
    
    // Act
    var result = await _service.SaveInventoryAsync(inventoryItem);
    
    // Assert
    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.ErrorMessage, Is.EqualTo("Database connection failed"));
    
    // Verify error logging
    _mockLogger.Verify(
        l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
                   It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()),
        Times.Once);
}
```

### Service Validation Testing

```csharp
[TestCase(null)]
[TestCase("")]
[TestCase("   ")]
public async Task SaveInventoryAsync_InvalidPartId_ReturnsValidationError(string invalidPartId)
{
    // Arrange
    var inventoryItem = new InventoryItem { PartId = invalidPartId };
    
    // Act
    var result = await _service.SaveInventoryAsync(inventoryItem);
    
    // Assert
    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.ErrorMessage, Contains.Substring("Part ID is required"));
}

[TestCase(-1)]
[TestCase(0)]
public async Task SaveInventoryAsync_InvalidQuantity_ReturnsValidationError(int invalidQuantity)
{
    // Arrange
    var inventoryItem = new InventoryItem 
    { 
        PartId = "TEST001",
        Quantity = invalidQuantity 
    };
    
    // Act
    var result = await _service.SaveInventoryAsync(inventoryItem);
    
    // Assert
    Assert.That(result.IsSuccess, Is.False);
    Assert.That(result.ErrorMessage, Contains.Substring("Quantity must be greater than zero"));
}
```

## 🏭 Manufacturing Domain Logic Testing

### Transaction Type Logic Testing

```csharp
[TestFixture]
[Category("Unit")]
[Category("BusinessLogic")]
public class TransactionLogicTests
{
    [TestCase(UserIntent.AddingStock, "IN")]
    [TestCase(UserIntent.RemovingStock, "OUT")]
    [TestCase(UserIntent.MovingStock, "TRANSFER")]
    public void DetermineTransactionType_ValidIntent_ReturnsCorrectType(
        UserIntent intent, string expectedType)
    {
        // Act
        var result = TransactionLogic.DetermineTransactionType(intent);
        
        // Assert
        Assert.That(result, Is.EqualTo(expectedType));
    }
    
    [Test]
    public void ValidatePartId_MTMPartFormat_ReturnsTrue()
    {
        // Arrange
        var validPartIds = new[] { "ABC-123", "PART001", "TEST-999" };
        
        foreach (var partId in validPartIds)
        {
            // Act
            var result = PartIdValidator.IsValid(partId);
            
            // Assert
            Assert.That(result, Is.True, $"Part ID '{partId}' should be valid");
        }
    }
    
    [Test]
    public void ValidateOperationNumber_WorkflowSteps_ReturnsTrue()
    {
        // Arrange - Standard MTM workflow operation numbers
        var validOperations = new[] { "90", "100", "110", "120", "130" };
        
        foreach (var operation in validOperations)
        {
            // Act
            var result = OperationValidator.IsValid(operation);
            
            // Assert
            Assert.That(result, Is.True, $"Operation '{operation}' should be valid");
        }
    }
}
```

### QuickButtons Logic Testing

```csharp
[TestFixture]
[Category("Unit")]
[Category("QuickButtons")]
public class QuickButtonLogicTests
{
    [Test]
    public void GenerateQuickButtonData_RecentTransaction_CreatesValidButton()
    {
        // Arrange
        var transaction = new SessionTransaction
        {
            PartId = "ABC-123",
            Operation = "100",
            Quantity = 5,
            Location = "STATION_A",
            TransactionType = "IN",
            Timestamp = DateTime.Now.AddMinutes(-5)
        };
        
        // Act
        var quickButton = QuickButtonLogic.GenerateFromTransaction(transaction);
        
        // Assert
        Assert.That(quickButton.PartId, Is.EqualTo("ABC-123"));
        Assert.That(quickButton.Operation, Is.EqualTo("100"));
        Assert.That(quickButton.Quantity, Is.EqualTo(5));
        Assert.That(quickButton.Location, Is.EqualTo("STATION_A"));
        Assert.That(quickButton.DisplayText, Is.EqualTo("ABC-123 @ 100 (5)"));
    }
}
```

## 🧹 Test Utilities and Helpers

### Base Test Classes

```csharp
public abstract class BaseViewModelTest<T> where T : BaseViewModel
{
    protected Mock<ILogger<T>> MockLogger { get; private set; }
    protected T ViewModel { get; set; }
    
    [SetUp]
    public virtual void BaseSetUp()
    {
        MockLogger = new Mock<ILogger<T>>();
        TestScheduler = new TestScheduler();
    }
    
    [TearDown]
    public virtual void BaseTearDown()
    {
        ViewModel?.Dispose();
    }
    
    protected void VerifyPropertyChanged(string propertyName)
    {
        var propertyChangedEvents = new List<string>();
        ViewModel.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args.PropertyName);
        
        Assert.That(propertyChangedEvents, Contains.Item(propertyName));
    }
}

public abstract class BaseServiceTest<T> where T : class
{
    protected Mock<ILogger<T>> MockLogger { get; private set; }
    protected Mock<IConfigurationService> MockConfigService { get; private set; }
    protected string TestConnectionString { get; private set; }
    
    [SetUp]
    public virtual void BaseSetUp()
    {
        MockLogger = new Mock<ILogger<T>>();
        MockConfigService = new Mock<IConfigurationService>();
        TestConnectionString = "Server=localhost;Database=mtm_test;";
        
        MockConfigService
            .Setup(c => c.GetConnectionString())
            .Returns(TestConnectionString);
    }
}
```

### Database Test Helpers

```csharp
public static class DatabaseTestHelper
{
    private static readonly Dictionary<string, DatabaseResult> MockedProcedures = new();
    
    public static void MockStoredProcedureCall(string procedureName, DatabaseResult result)
    {
        MockedProcedures[procedureName] = result;
    }
    
    public static void VerifyStoredProcedureCall(string procedureName, 
        Func<Dictionary<string, MySqlParameter>, bool> parameterValidator)
    {
        Assert.That(MockedProcedures.ContainsKey(procedureName), Is.True,
            $"Expected stored procedure '{procedureName}' to be called");
    }
    
    public static void ResetMocks()
    {
        MockedProcedures.Clear();
    }
}
```

### Test Data Builders

```csharp
public class InventoryItemBuilder
{
    private string _partId = "TEST001";
    private string _operation = "100";
    private int _quantity = 1;
    private string _location = "STATION_A";
    private string _user = "TestUser";
    
    public InventoryItemBuilder WithPartId(string partId)
    {
        _partId = partId;
        return this;
    }
    
    public InventoryItemBuilder WithOperation(string operation)
    {
        _operation = operation;
        return this;
    }
    
    public InventoryItemBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }
    
    public InventoryItem Build() => new()
    {
        PartId = _partId,
        Operation = _operation,
        Quantity = _quantity,
        Location = _location,
        User = _user
    };
}
```

## 📋 Coverage Requirements

### Unit Test Coverage Targets

| Component Type | Coverage Target | Key Areas |
|----------------|----------------|-----------|
| ViewModels | 95%+ | Properties, Commands, Validation, Error Handling |
| Services | 90%+ | Public methods, Error handling, Database calls |
| Business Logic | 95%+ | Domain rules, Calculations, Validations |
| Models | 80%+ | Property setters, Validation, Serialization |
| Utilities | 90%+ | Helper methods, Extensions, Converters |

### Critical Test Scenarios

1. **Constructor validation** - All dependency injection constructors
2. **Property change notifications** - All [ObservableProperty] properties
3. **Command execution** - All [RelayCommand] methods with success/error paths
4. **Service method calls** - All external service interactions
5. **Validation logic** - All business rule enforcement
6. **Error handling** - All try/catch blocks and error responses
7. **Async operations** - All Task-based methods with proper awaiting

This comprehensive unit testing strategy ensures individual component reliability while maintaining the manufacturing-grade quality standards required for the MTM WIP Application.

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
