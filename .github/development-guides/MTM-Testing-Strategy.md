# MTM Testing Strategy - Comprehensive Test Framework Documentation

## 📋 Overview

This document outlines the complete testing strategy for the MTM WIP Application, covering unit testing, integration testing, UI testing, and manufacturing workflow validation.

## 🎯 **Testing Philosophy**

### **Manufacturing-First Testing Approach**
- **Data Integrity**: Ensure inventory operations maintain accurate counts
- **Audit Compliance**: Verify all transactions are properly logged
- **User Safety**: Prevent data loss through comprehensive validation
- **Performance**: Validate response times for manufacturing floor operations
- **Error Recovery**: Ensure graceful handling of all failure scenarios

### **Testing Pyramid Strategy**
```
    🔺 E2E Tests (10%)
   ────────────────────
  🔺🔺 Integration Tests (20%)
 ────────────────────────────
🔺🔺🔺 Unit Tests (70%)
```

## 🧪 **Unit Testing Framework**

### **Technology Stack**
- **Test Framework**: MSTest v3.0+ (Microsoft official)
- **Mocking**: Moq 4.20+ for service dependencies
- **Assertions**: FluentAssertions for readable test assertions
- **Coverage**: Coverlet for code coverage analysis
- **Test Data**: AutoFixture for test data generation

### **Project Structure**
```
MTM_WIP_Application_Avalonia.Tests/
├── Unit/
│   ├── ViewModels/
│   │   ├── MainForm/
│   │   │   ├── InventoryTabViewModelTests.cs
│   │   │   ├── RemoveTabViewModelTests.cs
│   │   │   └── TransferTabViewModelTests.cs
│   │   ├── SettingsForm/
│   │   └── Shared/
│   ├── Services/
│   │   ├── ConfigurationServiceTests.cs
│   │   ├── MasterDataServiceTests.cs
│   │   └── ErrorHandlingTests.cs
│   ├── Converters/
│   └── Behaviors/
├── Integration/
├── TestHelpers/
└── TestData/
```

### **ViewModel Unit Testing Pattern**
```csharp
[TestClass]
public class InventoryTabViewModelTests
{
    private Mock<IInventoryService> _mockInventoryService = null!;
    private Mock<IMasterDataService> _mockMasterDataService = null!;
    private Mock<ILogger<InventoryTabViewModel>> _mockLogger = null!;
    private InventoryTabViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        
        _viewModel = new InventoryTabViewModel(
            _mockLogger.Object,
            _mockInventoryService.Object,
            _mockMasterDataService.Object
        );
    }

    [TestMethod]
    public async Task AddInventoryCommand_WithValidData_AddsInventorySuccessfully()
    {
        // Arrange
        var partInfo = new PartInfo
        {
            PartId = "TEST001",
            Operation = "90",
            Quantity = 10,
            Location = "A001"
        };

        _mockInventoryService
            .Setup(s => s.AddInventoryAsync(
                It.Is<string>(p => p == "TEST001"),
                It.Is<string>(o => o == "90"),
                It.Is<int>(q => q == 10),
                It.Is<string>(l => l == "A001")))
            .ReturnsAsync(Result<int>.Success(12345));

        _viewModel.PartId = "TEST001";
        _viewModel.Operation = "90";
        _viewModel.Quantity = 10;
        _viewModel.Location = "A001";

        // Act
        await _viewModel.AddInventoryCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsLoading.Should().BeFalse();
        _viewModel.ErrorMessage.Should().BeEmpty();
        _viewModel.PartId.Should().BeEmpty(); // Form cleared on success
        
        _mockInventoryService.Verify(
            s => s.AddInventoryAsync("TEST001", "90", 10, "A001"), 
            Times.Once);
    }

    [TestMethod]
    public async Task AddInventoryCommand_WithServiceFailure_DisplaysErrorMessage()
    {
        // Arrange
        _mockInventoryService
            .Setup(s => s.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), 
                                          It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(Result<int>.Failure("Database connection failed"));

        _viewModel.PartId = "TEST001";
        _viewModel.Operation = "90";
        _viewModel.Quantity = 10;
        _viewModel.Location = "A001";

        // Act
        await _viewModel.AddInventoryCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ErrorMessage.Should().NotBeEmpty();
        _viewModel.IsLoading.Should().BeFalse();
        _viewModel.HasError.Should().BeTrue();
    }

    [TestMethod]
    public void Validation_InvalidPartId_PreventsCammandExecution()
    {
        // Arrange
        _viewModel.PartId = ""; // Invalid empty part ID
        _viewModel.Operation = "90";
        _viewModel.Quantity = 10;
        _viewModel.Location = "A001";

        // Assert
        _viewModel.AddInventoryCommand.CanExecute(null).Should().BeFalse();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _viewModel?.Dispose();
    }
}
```

### **Service Layer Testing Pattern**
```csharp
[TestClass]
public class InventoryServiceTests
{
    private Mock<ILogger<InventoryService>> _mockLogger = null!;
    private Mock<IConfigurationService> _mockConfigurationService = null!;
    private InventoryService _service = null!;
    private string _testConnectionString = "Server=localhost;Database=test_mtm;";

    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<InventoryService>>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        
        _mockConfigurationService
            .Setup(c => c.GetConnectionStringAsync())
            .ReturnsAsync(_testConnectionString);

        _service = new InventoryService(_mockLogger.Object, _mockConfigurationService.Object);
    }

    [TestMethod]
    public async Task AddInventoryAsync_WithValidParameters_CallsStoredProcedure()
    {
        // This test would use a test database or mock the stored procedure helper
        // Arrange
        var partId = "TEST001";
        var operation = "90";
        var quantity = 10;
        var location = "A001";

        // Act
        var result = await _service.AddInventoryAsync(partId, operation, quantity, location);

        // Assert
        result.Should().NotBeNull();
        // Additional assertions based on expected behavior
    }
}
```

## 🔗 **Integration Testing Framework**

### **Database Integration Tests**
```csharp
[TestClass]
public class DatabaseIntegrationTests
{
    private const string TestConnectionString = "Server=localhost;Database=mtm_test;";
    
    [TestInitialize]
    public async Task Setup()
    {
        // Setup test database with clean state
        await DatabaseTestHelper.ResetTestDatabaseAsync();
        await DatabaseTestHelper.SeedTestDataAsync();
    }

    [TestMethod]
    public async Task StoredProcedure_inv_inventory_Add_Item_AddsInventoryCorrectly()
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "TEST001"),
            new("p_Operation", "90"),
            new("p_Quantity", 10),
            new("p_Location", "A001"),
            new("p_UserID", "testuser")
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            TestConnectionString,
            "inv_inventory_Add_Item",
            parameters
        );

        // Assert
        result.Status.Should().Be(1);
        result.Data.Should().NotBeNull();
        result.Data.Rows.Count.Should().Be(1);
        
        // Verify inventory was actually added
        var verifyParams = new MySqlParameter[]
        {
            new("p_PartID", "TEST001"),
            new("p_Operation", "90")
        };
        
        var verifyResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            TestConnectionString,
            "inv_inventory_Get_ByPartIDandOperation",
            verifyParams
        );
        
        verifyResult.Status.Should().Be(1);
        verifyResult.Data.Rows[0]["Quantity"].Should().Be(10);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await DatabaseTestHelper.CleanupTestDataAsync();
    }
}
```

### **Service Integration Tests**
```csharp
[TestClass]
public class ServiceIntegrationTests
{
    private IServiceProvider _serviceProvider = null!;
    private IConfigurationService _configurationService = null!;
    private IInventoryService _inventoryService = null!;

    [TestInitialize]
    public void Setup()
    {
        // Setup dependency injection container with test configuration
        var services = new ServiceCollection();
        services.AddMTMServices(CreateTestConfiguration());
        services.AddLogging(builder => builder.AddConsole());
        
        _serviceProvider = services.BuildServiceProvider();
        _configurationService = _serviceProvider.GetRequiredService<IConfigurationService>();
        _inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
    }

    [TestMethod]
    public async Task InventoryWorkflow_AddAndRemove_MaintainsDataIntegrity()
    {
        // Arrange
        const string partId = "INTTEST001";
        const string operation = "90";
        const string location = "TEST_LOC";
        
        // Act - Add inventory
        var addResult = await _inventoryService.AddInventoryAsync(partId, operation, 10, location);
        addResult.Should().BeSuccessful();

        // Verify addition
        var inventoryResult = await _inventoryService.GetInventoryByPartIdAndOperationAsync(partId, operation);
        inventoryResult.Should().BeSuccessful();
        inventoryResult.Data.Should().HaveCount(1);
        inventoryResult.Data[0].Quantity.Should().Be(10);

        // Act - Remove inventory
        var removeResult = await _inventoryService.RemoveInventoryAsync(partId, operation, 5, location);
        removeResult.Should().BeSuccessful();

        // Verify removal
        var updatedInventoryResult = await _inventoryService.GetInventoryByPartIdAndOperationAsync(partId, operation);
        updatedInventoryResult.Data[0].Quantity.Should().Be(5);
    }
}
```

## 🖥️ **UI Testing Framework**

### **Avalonia UI Testing**
```csharp
[TestClass]
public class InventoryTabViewUITests
{
    private TestAppBuilder _app = null!;
    private Window _window = null!;

    [TestInitialize]
    public void Setup()
    {
        _app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
        
        _window = new Window();
        _window.Content = new InventoryTabView
        {
            DataContext = CreateTestViewModel()
        };
    }

    [TestMethod]
    public void InventoryTab_InitialState_DisplaysCorrectly()
    {
        // Arrange & Act
        var inventoryTab = _window.GetControl<InventoryTabView>();
        var partIdTextBox = inventoryTab.GetControl<TextBox>("PartIdTextBox");
        var addButton = inventoryTab.GetControl<Button>("AddInventoryButton");

        // Assert
        partIdTextBox.Should().NotBeNull();
        partIdTextBox.Text.Should().BeEmpty();
        addButton.Should().NotBeNull();
        addButton.IsEnabled.Should().BeFalse(); // Should be disabled with empty form
    }

    [TestMethod]
    public async Task AddInventoryButton_WithValidInput_TriggersCommand()
    {
        // Arrange
        var inventoryTab = _window.GetControl<InventoryTabView>();
        var viewModel = (InventoryTabViewModel)inventoryTab.DataContext;
        var partIdTextBox = inventoryTab.GetControl<TextBox>("PartIdTextBox");
        var operationComboBox = inventoryTab.GetControl<ComboBox>("OperationComboBox");
        var quantityNumericUpDown = inventoryTab.GetControl<NumericUpDown>("QuantityNumericUpDown");
        var addButton = inventoryTab.GetControl<Button>("AddInventoryButton");

        // Act
        partIdTextBox.Text = "TEST001";
        operationComboBox.SelectedItem = "90";
        quantityNumericUpDown.Value = 10;

        await addButton.Command.ExecuteAsync(null);

        // Assert
        viewModel.IsLoading.Should().BeFalse();
        // Additional assertions based on expected behavior
    }
}
```

## 🏭 **Manufacturing Workflow Tests**

### **End-to-End Workflow Testing**
```csharp
[TestClass]
public class ManufacturingWorkflowTests
{
    private TestContext _testContext = null!;
    private IServiceProvider _serviceProvider = null!;

    [TestInitialize]
    public void Setup()
    {
        _serviceProvider = TestServiceProviderFactory.CreateWithTestDatabase();
    }

    [TestMethod]
    public async Task CompleteManufacturingWorkflow_ProcessPartThroughAllOperations()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var transactionService = _serviceProvider.GetRequiredService<ITransactionService>();
        
        const string partId = "WORKFLOW_TEST_001";
        const string startLocation = "RECEIVING";
        const string workLocation = "PRODUCTION";
        const string finishLocation = "SHIPPING";
        
        // Act & Assert - Step 1: Receive parts at operation 90
        var receiveResult = await inventoryService.AddInventoryAsync(partId, "90", 100, startLocation);
        receiveResult.Should().BeSuccessful();
        
        // Verify transaction was logged
        var receiveTransaction = await transactionService.GetTransactionHistoryAsync(partId, DateTime.Today, DateTime.Now);
        receiveTransaction.Data.Should().HaveCount(1);
        receiveTransaction.Data[0].TransactionType.Should().Be("IN");

        // Act & Assert - Step 2: Transfer to production for operation 100
        var transferResult = await inventoryService.TransferInventoryAsync(
            partId, "90", startLocation, workLocation, 50);
        transferResult.Should().BeSuccessful();
        
        // Act & Assert - Step 3: Process through operation 100
        var processResult = await inventoryService.ProcessOperationAsync(partId, "90", "100", workLocation, 50);
        processResult.Should().BeSuccessful();
        
        // Verify final state
        var finalInventory = await inventoryService.GetInventoryByPartIdAsync(partId);
        finalInventory.Data.Should().HaveCount(2); // Remaining at op 90 + processed at op 100
        
        var op90Inventory = finalInventory.Data.FirstOrDefault(i => i.Operation == "90");
        var op100Inventory = finalInventory.Data.FirstOrDefault(i => i.Operation == "100");
        
        op90Inventory.Should().NotBeNull();
        op90Inventory!.Quantity.Should().Be(50); // 100 - 50 transferred
        
        op100Inventory.Should().NotBeNull();
        op100Inventory!.Quantity.Should().Be(50); // 50 processed through
    }

    [TestMethod]
    public async Task ErrorScenario_InsufficientInventory_HandlesGracefully()
    {
        // Arrange
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        const string partId = "ERROR_TEST_001";
        
        // Add only 5 units
        await inventoryService.AddInventoryAsync(partId, "90", 5, "TEST_LOC");
        
        // Act - Try to remove 10 units (more than available)
        var removeResult = await inventoryService.RemoveInventoryAsync(partId, "90", 10, "TEST_LOC");
        
        // Assert
        removeResult.Should().BeFailure();
        removeResult.ErrorMessage.Should().Contain("insufficient inventory");
        
        // Verify inventory unchanged
        var inventory = await inventoryService.GetInventoryByPartIdAndOperationAsync(partId, "90");
        inventory.Data[0].Quantity.Should().Be(5); // Unchanged
    }
}
```

## ⚡ **Performance Testing**

### **Load Testing Framework**
```csharp
[TestClass]
public class PerformanceTests
{
    private IInventoryService _inventoryService = null!;
    private Stopwatch _stopwatch = null!;

    [TestInitialize]
    public void Setup()
    {
        _inventoryService = TestServiceProviderFactory.Create().GetRequiredService<IInventoryService>();
        _stopwatch = new Stopwatch();
    }

    [TestMethod]
    public async Task InventorySearch_Under100ms_MeetsPerformanceRequirements()
    {
        // Arrange - Add test data
        await SeedTestInventoryAsync(1000); // 1000 inventory records
        
        // Act
        _stopwatch.Start();
        var result = await _inventoryService.GetInventoryByPartIdAsync("PERF_TEST_500");
        _stopwatch.Stop();
        
        // Assert
        _stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
        result.Should().BeSuccessful();
    }

    [TestMethod]
    public async Task ConcurrentInventoryOperations_HandlesMultipleUsers()
    {
        // Arrange
        const int concurrentUsers = 10;
        const int operationsPerUser = 50;
        
        var tasks = new List<Task>();
        
        // Act - Simulate concurrent users
        for (int user = 0; user < concurrentUsers; user++)
        {
            var userTasks = Enumerable.Range(0, operationsPerUser)
                .Select(async operation => 
                {
                    var partId = $"CONCURRENT_TEST_{user}_{operation}";
                    return await _inventoryService.AddInventoryAsync(partId, "90", 1, "TEST_LOC");
                });
            
            tasks.AddRange(userTasks);
        }
        
        // Wait for all operations
        var results = await Task.WhenAll(tasks);
        
        // Assert
        results.Should().AllSatisfy(result => result.Should().BeSuccessful());
        
        // Verify data integrity
        var totalInventory = await _inventoryService.GetAllInventoryAsync();
        totalInventory.Data.Should().HaveCountGreaterOrEqualTo(concurrentUsers * operationsPerUser);
    }
}
```

## 📊 **Test Coverage Requirements**

### **Coverage Targets**
- **Unit Tests**: 90% line coverage minimum
- **Integration Tests**: 80% critical path coverage
- **UI Tests**: 70% user interaction coverage
- **E2E Tests**: 100% core workflow coverage

### **Coverage Analysis Tools**
```xml
<!-- In test project file -->
<PackageReference Include="coverlet.collector" Version="6.0.0" />
<PackageReference Include="coverlet.msbuild" Version="6.0.0" />
<PackageReference Include="ReportGenerator" Version="5.1.20" />
```

### **Coverage Report Generation**
```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:Html
```

## 🚀 **Test Execution Strategy**

### **Test Categories**
```csharp
[TestCategory("Unit")]
[TestCategory("Fast")]
public class FastUnitTests { }

[TestCategory("Integration")]
[TestCategory("Database")]
public class DatabaseIntegrationTests { }

[TestCategory("UI")]
[TestCategory("Slow")]
public class UITests { }

[TestCategory("E2E")]
[TestCategory("Manufacturing")]
public class ManufacturingWorkflowTests { }
```

### **CI/CD Pipeline Integration**
```yaml
# In GitHub Actions workflow
- name: Run Fast Tests
  run: dotnet test --filter TestCategory=Unit --logger trx

- name: Run Integration Tests
  run: dotnet test --filter TestCategory=Integration --logger trx
  
- name: Run UI Tests
  run: dotnet test --filter TestCategory=UI --logger trx

- name: Generate Coverage Report
  run: reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage"
```

### **Local Development Testing**
```bash
# Quick feedback loop - unit tests only
dotnet test --filter TestCategory=Unit

# Full test suite before commit
dotnet test --collect:"XPlat Code Coverage"

# Manufacturing workflow validation
dotnet test --filter TestCategory=Manufacturing
```

## 🔧 **Test Data Management**

### **Test Database Setup**
```sql
-- Create test database schema
CREATE DATABASE mtm_test;

-- Use minimal test data set
INSERT INTO part_ids VALUES ('TEST001', 'Test Part 1', 1);
INSERT INTO locations VALUES ('TEST_LOC', 'Test Location', 1);
INSERT INTO operation_numbers VALUES ('90', 'Test Operation', 1);
```

### **Test Data Factories**
```csharp
public static class TestDataFactory
{
    public static PartInfo CreatePartInfo(
        string partId = "TEST001",
        string operation = "90",
        int quantity = 10,
        string location = "TEST_LOC")
    {
        return new PartInfo
        {
            PartId = partId,
            Operation = operation,
            Quantity = quantity,
            Location = location
        };
    }

    public static List<PartInfo> CreatePartInfoList(int count = 5)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreatePartInfo($"TEST{i:000}"))
            .ToList();
    }
}
```

This comprehensive testing strategy ensures the MTM WIP Application maintains high quality, reliability, and performance standards suitable for manufacturing environments while providing fast feedback loops for developers.
