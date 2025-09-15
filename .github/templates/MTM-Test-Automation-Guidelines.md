---
description: 'Test automation guidelines for MTM WIP Application including CI/CD integration and automated testing strategies'
context_type: 'test_automation'
applies_to: 'all_components'
priority: 'high'
---

# MTM Test Automation Guidelines Template

## Overview

This template provides comprehensive test automation guidelines for MTM WIP Application, focusing on automated testing strategies, CI/CD integration, and manufacturing-grade continuous validation.

## Automation Strategy Overview

- **Automation Scope**: [Component/Feature/System Level]
- **Automation Framework**: [xUnit/NUnit/MSTest for .NET, Avalonia.Headless for UI]
- **CI/CD Platform**: [GitHub Actions/Azure DevOps/Other]
- **Test Execution Environment**: [Windows/macOS/Linux/Android]
- **Automation Coverage Target**: [Percentage and scope]

## Test Automation Architecture

### Test Automation Pyramid

```
                    ┌─────────────────┐
                    │   Manual Tests  │  ←  10% - Exploratory, Usability
                    │      (10%)      │
                ┌───┴─────────────────┴───┐
                │    End-to-End Tests     │  ←  20% - Critical User Journeys
                │        (20%)           │
            ┌───┴─────────────────────────┴───┐
            │     Integration Tests           │  ←  25% - Service Integration
            │         (25%)                 │
        ┌───┴─────────────────────────────────┴───┐
        │          Unit Tests                     │  ←  45% - Component Logic
        │            (45%)                       │
        └─────────────────────────────────────────┘
```

### Manufacturing Test Automation Layers

#### Layer 1: Unit Test Automation (45%)
- **MVVM Community Toolkit Tests**: Automated property and command testing
- **Service Logic Tests**: Business logic validation with mocking
- **Data Model Tests**: Manufacturing data validation and transformation
- **Utility Function Tests**: Helper methods and manufacturing calculations
- **Database Service Tests**: Stored procedure integration testing

#### Layer 2: Integration Test Automation (25%)
- **Service Integration Tests**: Cross-service communication validation
- **Database Integration Tests**: End-to-end database operation testing
- **Configuration Integration Tests**: Configuration loading and validation
- **External System Integration Tests**: Third-party service integration
- **Manufacturing Workflow Tests**: Complete manufacturing process validation

#### Layer 3: End-to-End Test Automation (20%)
- **Manufacturing User Journey Tests**: Complete operator workflows
- **Cross-Platform Validation Tests**: Multi-platform feature validation
- **Performance Regression Tests**: Automated performance validation
- **Manufacturing Scenario Tests**: Real manufacturing use case validation
- **Data Integrity Tests**: End-to-end data consistency validation

#### Layer 4: Manual Testing (10%)
- **Manufacturing Domain Validation**: Subject matter expert validation
- **Usability Testing**: Manufacturing operator experience validation
- **Exploratory Testing**: Ad-hoc testing of manufacturing scenarios
- **Security Testing**: Manufacturing security requirement validation
- **Accessibility Testing**: Manufacturing accessibility requirement validation

## Unit Test Automation Framework

### xUnit Test Framework Configuration
```csharp
// Global test configuration for MTM application
public class TestBase : IDisposable
{
    protected IServiceProvider ServiceProvider { get; private set; }
    protected IConfiguration Configuration { get; private set; }
    protected ITestOutputHelper Output { get; }
    
    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        SetupTestEnvironment();
    }
    
    private void SetupTestEnvironment()
    {
        // Setup test configuration
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json")
            .AddInMemoryCollection(GetTestConfiguration())
            .Build();
        
        // Setup test service provider
        var services = new ServiceCollection();
        ConfigureTestServices(services);
        ServiceProvider = services.BuildServiceProvider();
    }
    
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        // Register test-specific services
        services.AddMTMTestServices(Configuration);
        services.AddSingleton<ITestOutputHelper>(Output);
    }
    
    protected Dictionary<string, string> GetTestConfiguration()
    {
        return new Dictionary<string, string>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=mtm_test;Uid=test;Pwd=test;",
            ["MTMSettings:DefaultOperation"] = "90",
            ["MTMSettings:EnableAutoSave"] = "false"
        };
    }
    
    public void Dispose()
    {
        ServiceProvider?.Dispose();
        GC.SuppressFinalize(this);
    }
}
```

### MVVM Community Toolkit Test Automation
```csharp
// Automated testing for MVVM Community Toolkit patterns
[TestFixture]
[Category("Automation")]
[Category("Unit")]
public class InventoryViewModelAutomationTests : TestBase
{
    private InventoryTabViewModel _viewModel;
    private Mock<IInventoryService> _mockInventoryService;
    private Mock<IMasterDataService> _mockMasterDataService;
    
    public InventoryViewModelAutomationTests(ITestOutputHelper output) : base(output) { }
    
    [SetUp]
    public void SetUp()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        _viewModel = new InventoryTabViewModel(
            Mock.Of<ILogger<InventoryTabViewModel>>(),
            _mockInventoryService.Object,
            _mockMasterDataService.Object
        );
    }
    
    [Test]
    [TestCase("PART001", true)]
    [TestCase("", false)]
    [TestCase(null, false)]
    public void PartId_PropertyValidation_AutomatedValidation(string partId, bool expectedValid)
    {
        // Automated property validation testing
        _viewModel.PartId = partId;
        
        Assert.That(_viewModel.IsFormValid, Is.EqualTo(expectedValid));
        Assert.That(string.IsNullOrEmpty(_viewModel.PartId) == !expectedValid);
    }
    
    [Test]
    public async Task SaveCommand_AutomatedExecutionFlow_ShouldFollowExpectedPattern()
    {
        // Arrange - Setup automated test data
        _viewModel.PartId = "AUTO_TEST_001";
        _viewModel.Operation = "100";
        _viewModel.Quantity = 10;
        _viewModel.Location = "STATION_A";
        
        _mockInventoryService
            .Setup(s => s.AddInventoryAsync(It.IsAny<InventoryItem>()))
            .ReturnsAsync(true);
        
        // Act - Execute command
        await _viewModel.SaveCommand.ExecuteAsync(null);
        
        // Assert - Validate automated execution
        _mockInventoryService.Verify(s => s.AddInventoryAsync(It.Is<InventoryItem>(
            item => item.PartId == "AUTO_TEST_001" && 
                   item.Operation == "100" &&
                   item.Quantity == 10 &&
                   item.Location == "STATION_A")), Times.Once);
        
        Assert.That(_viewModel.IsLoading, Is.False);
        Output.WriteLine($"Automated test completed: {_viewModel.StatusMessage}");
    }
}
```

## Integration Test Automation

### Database Integration Test Automation
```csharp
// Automated database integration testing
[TestFixture]
[Category("Automation")]
[Category("Integration")]
[Category("Database")]
public class DatabaseIntegrationAutomationTests : TestBase
{
    private string _testConnectionString;
    private DatabaseTestFixture _dbFixture;
    
    public DatabaseIntegrationAutomationTests(ITestOutputHelper output) : base(output) { }
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _dbFixture = new DatabaseTestFixture(Output);
        await _dbFixture.SetupAsync();
        _testConnectionString = _dbFixture.ConnectionString;
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _dbFixture?.TearDownAsync();
    }
    
    [Test]
    [TestCaseSource(nameof(GetStoredProcedureTestCases))]
    public async Task StoredProcedure_AutomatedValidation_ShouldExecuteSuccessfully(
        StoredProcedureTestCase testCase)
    {
        // Automated stored procedure testing
        Output.WriteLine($"Testing stored procedure: {testCase.ProcedureName}");
        
        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _testConnectionString, testCase.ProcedureName, testCase.Parameters);
        
        // Assert
        Assert.That(result.Status, Is.EqualTo(testCase.ExpectedStatus), 
            $"Procedure {testCase.ProcedureName} returned unexpected status");
        
        if (testCase.ExpectedRowCount.HasValue)
        {
            Assert.That(result.Data.Rows.Count, Is.EqualTo(testCase.ExpectedRowCount.Value),
                $"Procedure {testCase.ProcedureName} returned unexpected row count");
        }
        
        Output.WriteLine($"✅ {testCase.ProcedureName} executed successfully");
    }
    
    private static IEnumerable<StoredProcedureTestCase> GetStoredProcedureTestCases()
    {
        // Automated test case generation for all stored procedures
        yield return new StoredProcedureTestCase
        {
            ProcedureName = "inv_inventory_Add_Item",
            Parameters = new MySqlParameter[]
            {
                new("p_PartID", "AUTO_TEST_001"),
                new("p_OperationNumber", "100"),
                new("p_Quantity", 10),
                new("p_Location", "STATION_A"),
                new("p_User", "AutomatedTest")
            },
            ExpectedStatus = 1
        };
        
        yield return new StoredProcedureTestCase
        {
            ProcedureName = "md_part_ids_Get_All",
            Parameters = new MySqlParameter[0],
            ExpectedStatus = 1,
            ExpectedRowCount = null // Variable row count
        };
        
        // Add more automated test cases...
    }
}

public class StoredProcedureTestCase
{
    public string ProcedureName { get; set; }
    public MySqlParameter[] Parameters { get; set; }
    public int ExpectedStatus { get; set; }
    public int? ExpectedRowCount { get; set; }
}
```

## UI Test Automation with Avalonia.Headless

### Avalonia UI Test Automation Framework
```csharp
// Automated UI testing with Avalonia.Headless
[TestFixture]
[Category("Automation")]
[Category("UI")]
public class InventoryTabUIAutomationTests
{
    private Application _app;
    private Window _mainWindow;
    private ITestOutputHelper _output;
    
    public InventoryTabUIAutomationTests(ITestOutputHelper output)
    {
        _output = output;
    }
    
    [SetUp]
    public async Task SetUp()
    {
        // Setup headless Avalonia application for automated testing
        _app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions
            {
                UseHeadlessDrawing = true,
                FrameBufferFormat = PixelFormat.Rgba8888
            })
            .StartWithClassicDesktopLifetime(Array.Empty<string>());
            
        _mainWindow = new MainWindow();
        _mainWindow.Show();
        
        // Wait for UI to initialize
        await Task.Delay(500);
        _output.WriteLine("UI test environment initialized");
    }
    
    [TearDown]
    public void TearDown()
    {
        _mainWindow?.Close();
        _app?.Dispose();
        _output.WriteLine("UI test environment cleaned up");
    }
    
    [Test]
    public async Task InventoryTab_AutomatedFormValidation_ShouldEnforceRequiredFields()
    {
        // Automated UI form validation testing
        var inventoryTab = _mainWindow.FindControl<InventoryTabView>("InventoryTab");
        Assert.That(inventoryTab, Is.Not.Null, "InventoryTab should be found");
        
        var partIdTextBox = inventoryTab.FindControl<TextBox>("PartIdTextBox");
        var saveButton = inventoryTab.FindControl<Button>("SaveButton");
        
        // Test initial state
        Assert.That(saveButton.IsEnabled, Is.False, "Save button should be disabled initially");
        
        // Test form filling automation
        await AutomateFormFilling(partIdTextBox, "AUTO_UI_TEST_001");
        await Task.Delay(100); // Allow for property change propagation
        
        // Validate form state after automation
        Assert.That(saveButton.IsEnabled, Is.True, "Save button should be enabled after form completion");
        _output.WriteLine("✅ Automated form validation completed successfully");
    }
    
    private async Task AutomateFormFilling(TextBox textBox, string value)
    {
        // Simulate automated user input
        textBox.Focus();
        await Task.Delay(50);
        
        textBox.Text = value;
        textBox.RaiseEvent(new TextChangedEventArgs
        {
            RoutedEvent = TextBox.TextChangedEvent
        });
        
        await Task.Delay(100);
        _output.WriteLine($"Automated text input: {value}");
    }
}
```

## CI/CD Integration Automation

### GitHub Actions Workflow Configuration
```yaml
# .github/workflows/automated-testing.yml
name: MTM Automated Testing

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  automated-unit-tests:
    name: Automated Unit Tests
    runs-on: ubuntu-latest
    strategy:
      matrix:
        dotnet-version: ['8.0.x']
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: ${{ matrix.dotnet-version }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Run Automated Unit Tests
      run: |
        dotnet test --no-build --configuration Release \
          --filter Category=Unit \
          --logger trx --results-directory TestResults/Unit \
          --collect:"XPlat Code Coverage"
    
    - name: Upload Unit Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: unit-test-results
        path: TestResults/Unit/
    
  automated-integration-tests:
    name: Automated Integration Tests
    runs-on: ubuntu-latest
    needs: automated-unit-tests
    
    services:
      mysql:
        image: mysql:8.0
        env:
          MYSQL_ROOT_PASSWORD: root
          MYSQL_DATABASE: mtm_test
        ports:
          - 3306:3306
        options: >-
          --health-cmd="mysqladmin ping"
          --health-interval=10s
          --health-timeout=5s
          --health-retries=3
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Setup Test Database
      run: |
        mysql -h 127.0.0.1 -u root -proot mtm_test < database/test-schema.sql
    
    - name: Run Automated Integration Tests
      run: |
        dotnet test --configuration Release \
          --filter Category=Integration \
          --logger trx --results-directory TestResults/Integration
      env:
        ConnectionStrings__DefaultConnection: "Server=127.0.0.1;Database=mtm_test;Uid=root;Pwd=root;"
    
    - name: Upload Integration Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: integration-test-results
        path: TestResults/Integration/

  automated-ui-tests:
    name: Automated UI Tests
    runs-on: windows-latest
    needs: automated-unit-tests
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Run Automated UI Tests
      run: |
        dotnet test --configuration Release \
          --filter Category=UI \
          --logger trx --results-directory TestResults/UI
    
    - name: Upload UI Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: ui-test-results
        path: TestResults/UI/

  automated-cross-platform-tests:
    name: Cross-Platform Automated Tests
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest, macos-latest]
    runs-on: ${{ matrix.os }}
    needs: [automated-unit-tests, automated-integration-tests]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Run Cross-Platform Tests
      run: |
        dotnet test --configuration Release \
          --filter Category=CrossPlatform \
          --logger trx --results-directory TestResults/CrossPlatform-${{ matrix.os }}
    
    - name: Upload Cross-Platform Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: crossplatform-test-results-${{ matrix.os }}
        path: TestResults/CrossPlatform-${{ matrix.os }}/

  automated-performance-tests:
    name: Automated Performance Tests
    runs-on: ubuntu-latest
    needs: [automated-integration-tests]
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Run Automated Performance Tests
      run: |
        dotnet test --configuration Release \
          --filter Category=Performance \
          --logger trx --results-directory TestResults/Performance \
          -- --timeout 600000
    
    - name: Upload Performance Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: performance-test-results
        path: TestResults/Performance/
```

## Test Data Management Automation

### Automated Test Data Generation
```csharp
// Automated test data management for manufacturing scenarios
public static class AutomatedTestDataGenerator
{
    private static readonly Random Random = new Random(42); // Seeded for consistency
    
    public static List<InventoryItem> GenerateManufacturingInventoryDataset(int count = 1000)
    {
        var partIds = GeneratePartIds(count / 10);
        var operations = new[] { "90", "100", "110", "120", "130" };
        var locations = new[] { "STATION_A", "STATION_B", "STATION_C", "WIP_001", "WAREHOUSE" };
        
        var dataset = new List<InventoryItem>();
        
        for (int i = 0; i < count; i++)
        {
            dataset.Add(new InventoryItem
            {
                PartId = partIds[Random.Next(partIds.Count)],
                Operation = operations[Random.Next(operations.Length)],
                Quantity = Random.Next(1, 1000),
                Location = locations[Random.Next(locations.Length)],
                TransactionType = Random.NextDouble() > 0.5 ? "IN" : "OUT",
                UserId = $"AutoUser{Random.Next(1, 10):00}",
                LastUpdated = DateTime.Now.AddDays(-Random.Next(0, 365))
            });
        }
        
        return dataset;
    }
    
    private static List<string> GeneratePartIds(int count)
    {
        var partIds = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            var prefix = new[] { "MTR", "HSG", "BLT", "GSK", "SPR" }[Random.Next(5)];
            var number = Random.Next(1000, 9999);
            var suffix = new[] { "A", "B", "C", "" }[Random.Next(4)];
            
            partIds.Add($"{prefix}-{number}{suffix}");
        }
        
        return partIds;
    }
    
    public static async Task SeedAutomatedTestDataAsync(string connectionString, int datasetSize = 1000)
    {
        var dataset = GenerateManufacturingInventoryDataset(datasetSize);
        
        foreach (var item in dataset)
        {
            var parameters = new MySqlParameter[]
            {
                new("p_PartID", item.PartId),
                new("p_OperationNumber", item.Operation),
                new("p_Quantity", item.Quantity),
                new("p_Location", item.Location),
                new("p_User", item.UserId)
            };
            
            await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString, "inv_inventory_Add_Item", parameters);
        }
    }
}
```

## Test Result Automation and Reporting

### Automated Test Result Analysis
```csharp
// Automated test result analysis and reporting
public class AutomatedTestResultAnalyzer
{
    private readonly ITestOutputHelper _output;
    
    public AutomatedTestResultAnalyzer(ITestOutputHelper output)
    {
        _output = output;
    }
    
    public async Task<TestAnalysisReport> AnalyzeTestResultsAsync(string testResultsPath)
    {
        var testResults = await LoadTestResultsAsync(testResultsPath);
        var report = new TestAnalysisReport();
        
        // Analyze test coverage
        report.UnitTestCoverage = CalculateUnitTestCoverage(testResults);
        report.IntegrationTestCoverage = CalculateIntegrationTestCoverage(testResults);
        report.UITestCoverage = CalculateUITestCoverage(testResults);
        
        // Analyze test performance
        report.AverageTestExecutionTime = CalculateAverageExecutionTime(testResults);
        report.SlowestTests = GetSlowestTests(testResults, 10);
        report.FastestTests = GetFastestTests(testResults, 10);
        
        // Analyze test reliability
        report.FlakyTests = IdentifyFlakyTests(testResults);
        report.FailureRate = CalculateFailureRate(testResults);
        report.TestTrends = AnalyzeTestTrends(testResults);
        
        // Generate automated recommendations
        report.Recommendations = GenerateAutomatedRecommendations(report);
        
        return report;
    }
    
    private List<string> GenerateAutomatedRecommendations(TestAnalysisReport report)
    {
        var recommendations = new List<string>();
        
        if (report.UnitTestCoverage < 0.95)
        {
            recommendations.Add($"Increase unit test coverage from {report.UnitTestCoverage:P1} to 95%+");
        }
        
        if (report.AverageTestExecutionTime > TimeSpan.FromMinutes(5))
        {
            recommendations.Add($"Optimize test execution time (current: {report.AverageTestExecutionTime:mm\\:ss})");
        }
        
        if (report.FlakyTests.Count > 0)
        {
            recommendations.Add($"Fix {report.FlakyTests.Count} flaky tests affecting reliability");
        }
        
        return recommendations;
    }
}
```

## Continuous Test Automation Improvement

### Automated Test Maintenance
- [ ] **Test Code Quality Monitoring**: Automated code quality checks for test code
- [ ] **Test Performance Monitoring**: Automated test execution time monitoring
- [ ] **Test Coverage Monitoring**: Continuous test coverage analysis and reporting
- [ ] **Test Reliability Monitoring**: Automated flaky test detection and reporting
- [ ] **Test Data Refresh**: Automated test data refresh and cleanup

### Manufacturing Test Automation Evolution
- [ ] **Manufacturing Scenario Expansion**: Continuous addition of new manufacturing test scenarios
- [ ] **Cross-Platform Test Expansion**: Expanding automated tests to new platforms
- [ ] **Performance Test Evolution**: Evolving performance tests with manufacturing requirements
- [ ] **Integration Test Evolution**: Adding new integration test scenarios
- [ ] **UI Test Evolution**: Expanding UI test coverage for new features

---

**Document Status**: ✅ Complete Test Automation Template  
**Framework Versions**: .NET 8, Avalonia UI 11.3.4, xUnit, Avalonia.Headless  
**Last Updated**: 2025-09-15  
**Test Automation Owner**: MTM Development Team