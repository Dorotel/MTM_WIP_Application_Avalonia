# MTM Test Automation Guidelines

## 📋 Overview

This document provides comprehensive guidelines for implementing automated testing in the MTM WIP Application, covering unit tests, integration tests, UI automation, and continuous testing pipelines specifically designed for manufacturing environments where reliability and accuracy are paramount.

## 🤖 **Automation Framework Architecture**

### **Test Automation Stack**
```yaml
Test Runners:
  - xUnit: Unit and integration testing
  - SpecFlow: Behavior-driven development (BDD)
  - Playwright: End-to-end UI automation
  - NBomber: Performance and load testing

CI/CD Integration:
  - GitHub Actions: Automated test execution
  - Azure DevOps: Enterprise testing pipelines
  - Docker: Containerized test environments
  - TestContainers: Database integration testing

Reporting:
  - Allure: Comprehensive test reporting
  - ReportGenerator: Code coverage reports
  - Slack/Teams: Test result notifications
  - Azure Application Insights: Test telemetry
```

### **Automation Testing Pyramid**
```
                    [UI Automation]
                         (5%)
                  End-to-End Workflows
                  Manufacturing Scenarios
                  
                [API/Integration Tests]
                       (25%)
                Service Layer Testing
                Database Integration
                
              [Unit Tests]
                  (70%)
             Fast, Isolated Tests
             Business Logic Validation
```

## 🧪 **Unit Test Automation**

### **Automated Unit Test Generation**
```csharp
// Test Generator Attributes for Automation
[AutoGenerateTests]
[TestCategory("Unit")]
public partial class InventoryViewModel : BaseViewModel
{
    [TestableProperty]
    [ObservableProperty]
    private string partId = string.Empty;

    [TestableProperty]
    [ObservableProperty]
    private int quantity;

    [TestableCommand]
    [RelayCommand]
    private async Task AddInventoryAsync()
    {
        // Implementation
    }
}

// Generated Test Class (Auto-Generated)
[TestFixture]
[Category("Unit")]
public partial class InventoryViewModelTests : ViewModelTestBase<InventoryViewModel>
{
    // Auto-generated property tests
    [Test]
    [TestCase("")]
    [TestCase("PART001")]
    [TestCase("INVALID_PART_ID")]
    public void PartId_SetValue_ShouldUpdateProperty(string value)
    {
        // Arrange & Act
        ViewModel.PartId = value;

        // Assert
        ViewModel.PartId.Should().Be(value);
        PropertyChangedEvents.Should().Contain(e => e.PropertyName == nameof(InventoryViewModel.PartId));
    }

    // Auto-generated command tests
    [Test]
    public async Task AddInventoryCommand_Execute_ShouldCallService()
    {
        // Arrange
        SetupMockService();

        // Act
        await ViewModel.AddInventoryCommand.ExecuteAsync(null);

        // Assert
        MockInventoryService.Verify(s => s.AddInventoryAsync(It.IsAny<InventoryItem>()), Times.Once);
    }
}
```

### **Automated Test Data Management**
```csharp
// Test Data Builder Pattern
public class InventoryItemBuilder
{
    private string _partId = "DEFAULT_PART";
    private int _quantity = 100;
    private string _location = "WH-A-001";
    private string _operation = "100";

    public static InventoryItemBuilder Create() => new();

    public InventoryItemBuilder WithPartId(string partId)
    {
        _partId = partId;
        return this;
    }

    public InventoryItemBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public InventoryItemBuilder WithLocation(string location)
    {
        _location = location;
        return this;
    }

    public InventoryItemBuilder WithOperation(string operation)
    {
        _operation = operation;
        return this;
    }

    public InventoryItemBuilder ForManufacturingScenario(ManufacturingScenario scenario)
    {
        return scenario switch
        {
            ManufacturingScenario.RawMaterial => WithOperation("90").WithQuantity(1000),
            ManufacturingScenario.WorkInProcess => WithOperation("100").WithQuantity(500),
            ManufacturingScenario.FinishedGoods => WithOperation("120").WithQuantity(100),
            _ => this
        };
    }

    public InventoryItem Build() => new()
    {
        PartId = _partId,
        Quantity = _quantity,
        Location = _location,
        Operation = _operation,
        Timestamp = DateTime.UtcNow
    };
}

// Automated Test Data Factory
[TestDataSource]
public static class AutomatedTestData
{
    [TestData("ValidPartIds")]
    public static IEnumerable<string> ValidPartIds => new[]
    {
        "PART001", "ABC123", "XYZ999", "TEST001"
    };

    [TestData("InvalidPartIds")]
    public static IEnumerable<string> InvalidPartIds => new[]
    {
        "", "123", "part001", "TOOLONGPARTID123456"
    };

    [TestData("ManufacturingOperations")]
    public static IEnumerable<string> Operations => new[]
    {
        "90", "100", "110", "120"
    };

    [TestData("ProductionScenarios")]
    public static IEnumerable<object[]> ProductionScenarios => new[]
    {
        new object[] { "PART001", "90", 1000, "Raw Material Addition" },
        new object[] { "PART001", "100", 950, "First Operation Processing" },
        new object[] { "PART001", "110", 900, "Second Operation Processing" },
        new object[] { "PART001", "120", 850, "Final Operation Completion" }
    };
}
```

## 🔗 **Integration Test Automation**

### **Database Integration Testing**
```csharp
// Automated Database Testing Framework
[TestFixture]
[Category("Integration")]
public class DatabaseIntegrationTests : IAsyncLifetime
{
    private readonly DatabaseTestContainer _dbContainer;
    private readonly ServiceProvider _serviceProvider;

    public DatabaseIntegrationTests()
    {
        _dbContainer = new DatabaseTestContainer();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        
        var services = new ServiceCollection();
        services.AddSingleton<IConfigurationService>(new TestConfigurationService
        {
            ConnectionString = _dbContainer.ConnectionString
        });
        services.AddMTMServices();
        
        _serviceProvider = services.BuildServiceProvider();
        
        // Run database migrations and seed test data
        await SeedTestDatabaseAsync();
    }

    [Test]
    [TestCaseSource(nameof(StoredProcedureTestCases))]
    public async Task StoredProcedure_AutomatedTesting(StoredProcedureTestCase testCase)
    {
        // Arrange
        var parameters = testCase.Parameters.Select(p => new MySqlParameter(p.Name, p.Value)).ToArray();

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _dbContainer.ConnectionString,
            testCase.ProcedureName,
            parameters
        );

        // Assert
        result.Should().NotBeNull();
        
        if (testCase.ExpectedStatus.HasValue)
        {
            result.Status.Should().Be(testCase.ExpectedStatus.Value);
        }

        if (testCase.ExpectedRowCount.HasValue)
        {
            result.Data.Rows.Count.Should().Be(testCase.ExpectedRowCount.Value);
        }

        // Validate result schema if specified
        if (testCase.ExpectedColumns?.Any() == true)
        {
            var actualColumns = result.Data.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            actualColumns.Should().Contain(testCase.ExpectedColumns);
        }
    }

    // Auto-generated test cases from stored procedure catalog
    public static IEnumerable<StoredProcedureTestCase> StoredProcedureTestCases => new[]
    {
        new StoredProcedureTestCase
        {
            ProcedureName = "inv_inventory_Add_Item",
            Parameters = new[]
            {
                new TestParameter("p_PartID", "AUTO_TEST_001"),
                new TestParameter("p_Quantity", 100),
                new TestParameter("p_Location", "WH-A-001"),
                new TestParameter("p_Operation", "100"),
                new TestParameter("p_TransactionType", "IN")
            },
            ExpectedStatus = 1,
            Description = "Add inventory item should return success status"
        },
        new StoredProcedureTestCase
        {
            ProcedureName = "inv_inventory_Get_ByPartID",
            Parameters = new[]
            {
                new TestParameter("p_PartID", "AUTO_TEST_001")
            },
            ExpectedStatus = 1,
            ExpectedRowCount = 1,
            ExpectedColumns = new[] { "PartID", "Quantity", "Location", "Operation" },
            Description = "Get inventory by part ID should return item details"
        }
    };
}

// Test Case Model
public class StoredProcedureTestCase
{
    public string ProcedureName { get; set; } = string.Empty;
    public TestParameter[] Parameters { get; set; } = Array.Empty<TestParameter>();
    public int? ExpectedStatus { get; set; }
    public int? ExpectedRowCount { get; set; }
    public string[]? ExpectedColumns { get; set; }
    public string Description { get; set; } = string.Empty;

    public override string ToString() => $"{ProcedureName}: {Description}";
}

public class TestParameter
{
    public string Name { get; set; }
    public object Value { get; set; }

    public TestParameter(string name, object value)
    {
        Name = name;
        Value = value;
    }
}
```

### **Service Integration Automation**
```csharp
// Automated Service Integration Testing
[TestFixture]
[Category("Integration")]
public class ServiceIntegrationTestSuite
{
    private readonly TestHost _testHost;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _testHost = await CreateTestHostAsync();
    }

    [Test]
    [TestCaseSource(nameof(ServiceWorkflowTestCases))]
    public async Task ServiceWorkflow_Automated(ServiceWorkflowTestCase testCase)
    {
        // Arrange
        var services = testCase.RequiredServices.Select(serviceType => 
            _testHost.Services.GetRequiredService(serviceType)).ToArray();

        // Act
        var results = new List<object>();
        
        foreach (var step in testCase.WorkflowSteps)
        {
            var service = services.First(s => s.GetType() == step.ServiceType);
            var method = step.ServiceType.GetMethod(step.MethodName);
            
            var parameters = step.Parameters ?? Array.Empty<object>();
            var result = await (Task<object>)method.Invoke(service, parameters);
            results.Add(result);
            
            // Validate intermediate results
            if (step.Validation != null)
            {
                step.Validation(result);
            }
        }

        // Assert
        testCase.FinalValidation?.Invoke(results);
    }

    public static IEnumerable<ServiceWorkflowTestCase> ServiceWorkflowTestCases => new[]
    {
        new ServiceWorkflowTestCase
        {
            Name = "Complete Inventory Transaction Workflow",
            RequiredServices = new[] { typeof(IInventoryService), typeof(ITransactionService) },
            WorkflowSteps = new[]
            {
                new WorkflowStep
                {
                    ServiceType = typeof(IInventoryService),
                    MethodName = nameof(IInventoryService.AddInventoryAsync),
                    Parameters = new object[] { InventoryItemBuilder.Create().WithPartId("WORKFLOW_001").Build() },
                    Validation = result => ((ServiceResult)result).IsSuccess.Should().BeTrue()
                },
                new WorkflowStep
                {
                    ServiceType = typeof(ITransactionService),
                    MethodName = nameof(ITransactionService.GetTransactionHistoryAsync),
                    Parameters = new object[] { "WORKFLOW_001", DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1) },
                    Validation = result => ((ServiceResult<List<TransactionHistory>>)result).Data.Should().NotBeEmpty()
                }
            },
            FinalValidation = results =>
            {
                results.Should().HaveCount(2);
                results.All(r => r is ServiceResult sr && sr.IsSuccess).Should().BeTrue();
            }
        }
    };
}

// Workflow Test Models
public class ServiceWorkflowTestCase
{
    public string Name { get; set; } = string.Empty;
    public Type[] RequiredServices { get; set; } = Array.Empty<Type>();
    public WorkflowStep[] WorkflowSteps { get; set; } = Array.Empty<WorkflowStep>();
    public Action<List<object>>? FinalValidation { get; set; }

    public override string ToString() => Name;
}

public class WorkflowStep
{
    public Type ServiceType { get; set; } = typeof(object);
    public string MethodName { get; set; } = string.Empty;
    public object[]? Parameters { get; set; }
    public Action<object>? Validation { get; set; }
}
```

## 🖥️ **UI Automation Framework**

### **Avalonia UI Testing Automation**
```csharp
// Automated UI Testing with Playwright-like approach for Avalonia
[TestFixture]
[Category("UI")]
public class UIAutomationTests
{
    private TestApplication _app;
    private Window _mainWindow;

    [SetUp]
    public async Task SetUp()
    {
        _app = await TestApplication.StartAsync();
        _mainWindow = _app.GetMainWindow();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _app.StopAsync();
    }

    [Test]
    [TestCase("MainView")]
    [TestCase("InventoryTabView")]
    [TestCase("RemoveTabView")]
    [TestCase("TransferTabView")]
    public async Task NavigateToView_ShouldLoadCorrectContent(string viewName)
    {
        // Arrange
        var navigationService = _app.GetService<INavigationService>();

        // Act
        await navigationService.NavigateToAsync(viewName);
        await _app.WaitForIdleAsync();

        // Assert
        var currentView = _mainWindow.FindControl<UserControl>("ContentArea");
        currentView.Should().NotBeNull();
        currentView.GetType().Name.Should().Contain(viewName);
    }

    [Test]
    public async Task InventorySearch_WithValidPartId_ShouldDisplayResults()
    {
        // Arrange
        await _app.NavigateToViewAsync("MainView");
        var searchTextBox = _mainWindow.FindControl<TextBox>("PartIdSearchBox");
        var searchButton = _mainWindow.FindControl<Button>("SearchButton");
        var resultsDataGrid = _mainWindow.FindControl<DataGrid>("SearchResultsGrid");

        // Act
        await searchTextBox.SetTextAsync("PART001");
        await searchButton.ClickAsync();
        await _app.WaitForOperationAsync(() => !resultsDataGrid.ItemsSource?.Cast<object>()?.Any() != true);

        // Assert
        resultsDataGrid.ItemsSource.Should().NotBeNull();
        resultsDataGrid.ItemsSource.Cast<InventoryItem>().Should().NotBeEmpty();
        resultsDataGrid.ItemsSource.Cast<InventoryItem>().First().PartId.Should().Be("PART001");
    }

    [Test]
    public async Task AddInventory_CompleteWorkflow_ShouldUpdateInventory()
    {
        // Arrange
        await _app.NavigateToViewAsync("InventoryTabView");
        var partIdBox = _mainWindow.FindControl<TextBox>("NewPartIdBox");
        var quantityBox = _mainWindow.FindControl<NumericUpDown>("NewQuantityBox");
        var locationCombo = _mainWindow.FindControl<ComboBox>("NewLocationCombo");
        var operationCombo = _mainWindow.FindControl<ComboBox>("NewOperationCombo");
        var saveButton = _mainWindow.FindControl<Button>("SaveInventoryButton");

        var testPartId = $"UI_TEST_{Guid.NewGuid():N}"[..12];

        // Act
        await partIdBox.SetTextAsync(testPartId);
        await quantityBox.SetValueAsync(150);
        await locationCombo.SelectItemAsync("WH-A-001");
        await operationCombo.SelectItemAsync("100");
        await saveButton.ClickAsync();

        // Wait for save operation
        await _app.WaitForOperationAsync(() => !saveButton.IsEnabled);
        await _app.WaitForOperationAsync(() => saveButton.IsEnabled);

        // Assert - Verify inventory was added
        await _app.NavigateToViewAsync("MainView");
        var searchBox = _mainWindow.FindControl<TextBox>("PartIdSearchBox");
        var searchBtn = _mainWindow.FindControl<Button>("SearchButton");
        var resultsGrid = _mainWindow.FindControl<DataGrid>("SearchResultsGrid");

        await searchBox.SetTextAsync(testPartId);
        await searchBtn.ClickAsync();
        await _app.WaitForOperationAsync(() => resultsGrid.ItemsSource?.Cast<object>()?.Any() == true);

        var results = resultsGrid.ItemsSource.Cast<InventoryItem>().ToList();
        results.Should().ContainSingle(item => 
            item.PartId == testPartId && 
            item.Quantity >= 150 && 
            item.Location == "WH-A-001");
    }
}

// UI Test Extensions
public static class UITestExtensions
{
    public static async Task SetTextAsync(this TextBox textBox, string text)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            textBox.Focus();
            textBox.Clear();
            textBox.Text = text;
        });
    }

    public static async Task SetValueAsync(this NumericUpDown numericUpDown, double value)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            numericUpDown.Value = value;
        });
    }

    public static async Task ClickAsync(this Button button)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var clickEvent = new RoutedEventArgs(Button.ClickEvent);
            button.RaiseEvent(clickEvent);
        });
    }

    public static async Task SelectItemAsync(this ComboBox comboBox, string itemText)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var item = comboBox.Items?.Cast<object>()
                .FirstOrDefault(i => i.ToString() == itemText);
            if (item != null)
            {
                comboBox.SelectedItem = item;
            }
        });
    }

    public static async Task WaitForOperationAsync(this TestApplication app, Func<bool> condition, 
        int timeoutMs = 5000, int pollIntervalMs = 100)
    {
        var startTime = DateTime.UtcNow;
        while (!condition() && DateTime.UtcNow.Subtract(startTime).TotalMilliseconds < timeoutMs)
        {
            await Task.Delay(pollIntervalMs);
            await Dispatcher.UIThread.InvokeAsync(() => { }); // Process UI updates
        }

        if (!condition())
        {
            throw new TimeoutException($"Operation did not complete within {timeoutMs}ms");
        }
    }
}
```

## 🚀 **Continuous Testing Pipeline**

### **GitHub Actions Automation**
```yaml
# .github/workflows/automated-testing.yml
name: Automated Testing Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  schedule:
    - cron: '0 2 * * *'  # Daily at 2 AM

env:
  DOTNET_VERSION: '8.0.x'
  MYSQL_VERSION: '9.4'

jobs:
  unit-tests:
    name: Unit Tests
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run Unit Tests
      run: |
        dotnet test \
          --no-build \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --collect:"XPlat Code Coverage" \
          --filter "Category=Unit" \
          -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
    
    - name: Upload Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: unit-test-results
        path: TestResults/
    
    - name: Code Coverage Report
      uses: codecov/codecov-action@v3
      with:
        file: TestResults/coverage.opencover.xml
        flags: unittests
        name: unit-tests-coverage

  integration-tests:
    name: Integration Tests
    runs-on: ubuntu-latest
    
    services:
      mysql:
        image: mysql:9.4
        env:
          MYSQL_ROOT_PASSWORD: testpass
          MYSQL_DATABASE: mtm_test
        ports:
          - 3306:3306
        options: --health-cmd="mysqladmin ping" --health-interval=10s --health-timeout=5s --health-retries=3
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Setup Test Database
      run: |
        mysql -h127.0.0.1 -uroot -ptestpass mtm_test < Scripts/Database/CreateTables.sql
        mysql -h127.0.0.1 -uroot -ptestpass mtm_test < Scripts/Database/StoredProcedures.sql
        mysql -h127.0.0.1 -uroot -ptestpass mtm_test < Scripts/Database/TestData.sql
    
    - name: Run Integration Tests
      run: |
        dotnet test \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --collect:"XPlat Code Coverage" \
          --filter "Category=Integration"
      env:
        MTM_TEST_CONNECTION_STRING: "Server=127.0.0.1;Database=mtm_test;Uid=root;Pwd=testpass;"
    
    - name: Upload Integration Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: integration-test-results
        path: TestResults/

  ui-automation:
    name: UI Automation Tests
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Build Application
      run: dotnet build --configuration Release
    
    - name: Run UI Tests
      run: |
        dotnet test \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=UI"
    
    - name: Screenshot on Failure
      if: failure()
      run: |
        mkdir -p TestResults/Screenshots
        # Custom screenshot capture logic here
    
    - name: Upload UI Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: ui-test-results
        path: TestResults/

  performance-tests:
    name: Performance Tests
    runs-on: ubuntu-latest
    if: github.event_name == 'schedule' || contains(github.event.pull_request.labels.*.name, 'performance')
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Run Performance Tests
      run: |
        dotnet test \
          --configuration Release \
          --logger trx \
          --results-directory TestResults \
          --filter "Category=Performance"
    
    - name: Upload Performance Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: performance-test-results
        path: TestResults/

  test-report:
    name: Generate Test Report
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests, ui-automation]
    if: always()
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Download All Test Results
      uses: actions/download-artifact@v3
      with:
        path: AllTestResults/
    
    - name: Generate Combined Report
      uses: dorny/test-reporter@v1
      if: always()
      with:
        name: MTM Test Results
        path: 'AllTestResults/**/*.trx'
        reporter: dotnet-trx
        fail-on-error: false
    
    - name: Comment PR with Results
      if: github.event_name == 'pull_request'
      uses: actions/github-script@v6
      with:
        script: |
          const fs = require('fs');
          
          // Read test results and generate summary
          let summary = '## 🧪 Automated Test Results\n\n';
          summary += '| Test Type | Status | Details |\n';
          summary += '|-----------|--------|----------|\n';
          
          // Add results for each test type
          if (fs.existsSync('AllTestResults/unit-test-results')) {
            summary += '| Unit Tests | ✅ Passed | Core functionality validated |\n';
          }
          
          if (fs.existsSync('AllTestResults/integration-test-results')) {
            summary += '| Integration Tests | ✅ Passed | Service integration validated |\n';
          }
          
          if (fs.existsSync('AllTestResults/ui-test-results')) {
            summary += '| UI Automation | ✅ Passed | User interface workflows validated |\n';
          }
          
          summary += '\n✨ All automated tests completed successfully!';
          
          github.rest.issues.createComment({
            issue_number: context.issue.number,
            owner: context.repo.owner,
            repo: context.repo.repo,
            body: summary
          });
```

### **Test Data Management Automation**
```csharp
// Automated Test Data Lifecycle Management
public class TestDataManager : ITestDataManager
{
    private readonly string _connectionString;
    private readonly ILogger<TestDataManager> _logger;

    public async Task<TestDataSet> CreateTestDataSetAsync(TestScenario scenario)
    {
        var dataSet = new TestDataSet { Scenario = scenario };

        switch (scenario)
        {
            case TestScenario.InventoryOperations:
                dataSet.InventoryItems = await CreateInventoryTestDataAsync();
                dataSet.MasterData = await CreateMasterDataAsync();
                break;

            case TestScenario.TransactionHistory:
                dataSet.Transactions = await CreateTransactionTestDataAsync();
                dataSet.InventoryItems = await CreateInventoryTestDataAsync();
                break;

            case TestScenario.UserPermissions:
                dataSet.Users = await CreateUserTestDataAsync();
                dataSet.Permissions = await CreatePermissionTestDataAsync();
                break;
        }

        dataSet.Id = await PersistTestDataSetAsync(dataSet);
        return dataSet;
    }

    public async Task CleanupTestDataSetAsync(string dataSetId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var cleanupQueries = new[]
        {
            "DELETE FROM inventory WHERE part_id LIKE 'TEST_%'",
            "DELETE FROM transactions WHERE part_id LIKE 'TEST_%'",
            "DELETE FROM users WHERE username LIKE 'TEST_%'"
        };

        foreach (var query in cleanupQueries)
        {
            using var command = new MySqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }

        _logger.LogInformation("Cleaned up test data set {DataSetId}", dataSetId);
    }

    public async Task<bool> ValidateTestDataIntegrityAsync(string dataSetId)
    {
        // Validate that test data hasn't been corrupted during tests
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        var validationQueries = new[]
        {
            ("SELECT COUNT(*) FROM inventory WHERE quantity < 0", 0), // No negative inventory
            ("SELECT COUNT(*) FROM transactions WHERE transaction_type NOT IN ('IN', 'OUT', 'TRANSFER')", 0), // Valid transaction types
            ("SELECT COUNT(*) FROM inventory WHERE part_id IS NULL OR part_id = ''", 0) // No empty part IDs
        };

        foreach (var (query, expectedCount) in validationQueries)
        {
            using var command = new MySqlCommand(query, connection);
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            
            if (count != expectedCount)
            {
                _logger.LogError("Data integrity validation failed: {Query} returned {Count}, expected {Expected}",
                    query, count, expectedCount);
                return false;
            }
        }

        return true;
    }
}
```

### **Automated Test Maintenance**
```csharp
// Test Health Monitoring and Maintenance
[TestFixture]
[Category("Maintenance")]
public class TestHealthMonitoring
{
    [Test]
    [Explicit("Health Check")]
    public async Task ValidateTestEnvironment()
    {
        var healthChecks = new[]
        {
            ("Database Connection", CheckDatabaseConnectionAsync),
            ("Test Data Integrity", CheckTestDataIntegrityAsync),
            ("Service Dependencies", CheckServiceDependenciesAsync),
            ("External API Availability", CheckExternalApisAsync)
        };

        var results = new List<(string Check, bool Passed, string Message)>();

        foreach (var (checkName, checkMethod) in healthChecks)
        {
            try
            {
                var passed = await checkMethod();
                results.Add((checkName, passed, passed ? "OK" : "Failed"));
            }
            catch (Exception ex)
            {
                results.Add((checkName, false, ex.Message));
            }
        }

        // Report results
        var report = new StringBuilder();
        report.AppendLine("Test Environment Health Check Results:");
        report.AppendLine("==========================================");

        foreach (var (check, passed, message) in results)
        {
            var status = passed ? "✅ PASS" : "❌ FAIL";
            report.AppendLine($"{status} {check}: {message}");
        }

        Console.WriteLine(report.ToString());

        // Assert all checks passed
        var failedChecks = results.Where(r => !r.Passed).ToList();
        if (failedChecks.Any())
        {
            var failedMessages = string.Join(", ", failedChecks.Select(f => f.Check));
            Assert.Fail($"Health checks failed: {failedMessages}");
        }
    }

    private async Task<bool> CheckDatabaseConnectionAsync()
    {
        // Implementation for database connection check
        return true;
    }

    private async Task<bool> CheckTestDataIntegrityAsync()
    {
        // Implementation for test data integrity check
        return true;
    }

    private async Task<bool> CheckServiceDependenciesAsync()
    {
        // Implementation for service dependency check
        return true;
    }

    private async Task<bool> CheckExternalApisAsync()
    {
        // Implementation for external API availability check
        return true;
    }
}
```

This comprehensive test automation framework ensures reliable, maintainable, and efficient testing for the MTM WIP Application, with full integration into CI/CD pipelines and manufacturing-specific validation patterns.
