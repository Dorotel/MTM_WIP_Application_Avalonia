---
description: 'Comprehensive testing procedures for unit, integration, and UI testing in the MTM WIP Application'
mode: 'instruction'
tools: ['codebase', 'editFiles', 'search', 'problems']
---

# Testing Procedures Instructions

Complete guide for implementing and running comprehensive tests in the MTM WIP Application, including unit tests, integration tests, and UI tests.

## When to Use This Guide

Use this guide when:
- Writing tests for new features or components
- Setting up testing infrastructure for the project
- Running comprehensive test suites before releases
- Debugging failing tests or test infrastructure
- Establishing testing standards for the team

## Testing Architecture Overview

### MTM Testing Philosophy
- **Test-Driven Development**: Write tests before implementation when possible
- **Comprehensive Coverage**: Unit, integration, and UI tests for complete validation
- **Fast Feedback**: Tests should run quickly and provide clear failure messages
- **Realistic Testing**: Use real database connections and actual UI interactions
- **Automated Testing**: All tests run in CI/CD pipeline automatically

## Unit Testing Setup and Standards

### Testing Framework Configuration
```xml
<!-- Test project dependencies in MTM.Tests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.7.1" />
<PackageReference Include="xUnit" Version="2.4.2" />
<PackageReference Include="xUnit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Testing" Version="8.0.0" />
```

### ViewModel Testing Patterns
```csharp
// Example: InventoryTabViewModel unit tests
public class InventoryTabViewModelTests : IDisposable
{
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<IMasterDataService> _mockMasterDataService;
    private readonly Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private readonly InventoryTabViewModel _viewModel;

    public InventoryTabViewModelTests()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        
        _viewModel = new InventoryTabViewModel(
            _mockInventoryService.Object,
            _mockMasterDataService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task SaveCommand_WithValidData_CallsInventoryService()
    {
        // Arrange
        _viewModel.PartId = "PART001";
        _viewModel.SelectedOperation = "100";
        _viewModel.Quantity = 5;
        _viewModel.Location = "A01";

        _mockInventoryService
            .Setup(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _mockInventoryService.Verify(x => x.AddInventoryAsync("PART001", "100", 5, "A01"), Times.Once);
        _viewModel.StatusMessage.Should().Contain("successfully");
    }

    [Theory]
    [InlineData("", "100", 1, "A01", false)] // Empty PartId
    [InlineData("PART001", "", 1, "A01", false)] // Empty Operation
    [InlineData("PART001", "100", 0, "A01", false)] // Zero Quantity
    [InlineData("PART001", "100", 1, "", false)] // Empty Location
    [InlineData("PART001", "100", 1, "A01", true)] // Valid data
    public void IsFormValid_WithVariousInputs_ReturnsExpectedResult(
        string partId, string operation, int quantity, string location, bool expected)
    {
        // Arrange
        _viewModel.PartId = partId;
        _viewModel.SelectedOperation = operation;
        _viewModel.Quantity = quantity;
        _viewModel.Location = location;

        // Act & Assert
        _viewModel.IsFormValid.Should().Be(expected);
    }

    public void Dispose()
    {
        _viewModel?.Dispose();
    }
}
```

### Service Testing Patterns
```csharp
// Example: InventoryService unit tests
public class InventoryServiceTests : IDisposable
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<InventoryService>> _mockLogger;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<InventoryService>>();
        _service = new InventoryService(_mockDatabaseService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddInventoryAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var expectedResult = new DatabaseResult { Status = 1, Message = "Success" };
        _mockDatabaseService
            .Setup(x => x.ExecuteStoredProcedureAsync("inv_inventory_Add_Item", It.IsAny<MySqlParameter[]>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.AddInventoryAsync("PART001", "100", 5, "A01");

        // Assert
        result.Should().BeTrue();
        _mockDatabaseService.Verify(x => x.ExecuteStoredProcedureAsync(
            "inv_inventory_Add_Item", 
            It.Is<MySqlParameter[]>(p => 
                p.Any(param => param.ParameterName == "p_PartID" && param.Value.ToString() == "PART001")
            )), Times.Once);
    }

    [Fact]
    public async Task AddInventoryAsync_WithDatabaseError_ReturnsFalse()
    {
        // Arrange
        var errorResult = new DatabaseResult { Status = 0, Message = "Database error" };
        _mockDatabaseService
            .Setup(x => x.ExecuteStoredProcedureAsync(It.IsAny<string>(), It.IsAny<MySqlParameter[]>()))
            .ReturnsAsync(errorResult);

        // Act
        var result = await _service.AddInventoryAsync("PART001", "100", 5, "A01");

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _service?.Dispose();
    }
}
```

## Integration Testing Setup

### Database Integration Testing
```csharp
// Integration test with real database
public class DatabaseIntegrationTests : IClassFixture<DatabaseTestFixture>, IDisposable
{
    private readonly DatabaseTestFixture _fixture;
    private readonly DatabaseService _databaseService;

    public DatabaseIntegrationTests(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
        _databaseService = new DatabaseService(_fixture.ConnectionString, _fixture.Logger);
    }

    [Fact]
    public async Task InventoryService_AddAndRetrieve_WorksEndToEnd()
    {
        // Arrange
        var partId = $"TEST_{Guid.NewGuid():N}";
        var operation = "100";
        var quantity = 5;
        var location = "A01";

        var inventoryService = new InventoryService(_databaseService, _fixture.Logger);

        try
        {
            // Act - Add inventory
            var addResult = await inventoryService.AddInventoryAsync(partId, operation, quantity, location);
            addResult.Should().BeTrue();

            // Act - Retrieve inventory
            var retrieveResult = await inventoryService.GetInventoryAsync(partId, operation);

            // Assert
            retrieveResult.Should().NotBeEmpty();
            var item = retrieveResult.First();
            item.PartId.Should().Be(partId);
            item.Operation.Should().Be(operation);
            item.Quantity.Should().Be(quantity);
            item.Location.Should().Be(location);
        }
        finally
        {
            // Cleanup
            await CleanupTestData(partId);
        }
    }

    private async Task CleanupTestData(string partId)
    {
        var parameters = new MySqlParameter[] { new("p_PartID", partId) };
        await _databaseService.ExecuteStoredProcedureAsync("test_cleanup_inventory", parameters);
    }

    public void Dispose()
    {
        _databaseService?.Dispose();
    }
}

// Test fixture for database integration tests
public class DatabaseTestFixture : IDisposable
{
    public string ConnectionString { get; }
    public ILogger<DatabaseService> Logger { get; }

    public DatabaseTestFixture()
    {
        // Use test database
        ConnectionString = "Server=localhost;Database=MTM_WIP_Test;Uid=mtm_test;Pwd=TestPassword123!;SslMode=none;";
        
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        Logger = loggerFactory.CreateLogger<DatabaseService>();

        InitializeTestDatabase();
    }

    private void InitializeTestDatabase()
    {
        // Ensure test database exists and is properly configured
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        
        // Run any necessary setup scripts
        var setupScript = File.ReadAllText("Scripts/test-database-setup.sql");
        using var command = new MySqlCommand(setupScript, connection);
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        // Cleanup test database if needed
    }
}
```

## UI Testing with Avalonia

### Avalonia UI Testing Setup
```csharp
// UI integration tests using Avalonia.Headless
public class InventoryTabViewUITests
{
    [Fact]
    public async Task InventoryTabView_LoadsWithCorrectInitialState()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var serviceCollection = new ServiceCollection();
        ConfigureTestServices(serviceCollection);

        // Act
        var window = new Window();
        var viewModel = serviceCollection.BuildServiceProvider().GetRequiredService<InventoryTabViewModel>();
        var view = new InventoryTabView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        // Assert
        Assert.NotNull(view.FindControl<TextBox>("PartIdTextBox"));
        Assert.NotNull(view.FindControl<ComboBox>("OperationComboBox"));
        Assert.NotNull(view.FindControl<NumericUpDown>("QuantityNumericUpDown"));
        Assert.NotNull(view.FindControl<Button>("SaveButton"));
    }

    [Fact]
    public async Task SaveButton_WhenFormValid_EnabledCorrectly()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var window = new Window();
        var viewModel = CreateTestViewModel();
        var view = new InventoryTabView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        var saveButton = view.FindControl<Button>("SaveButton");
        var partIdTextBox = view.FindControl<TextBox>("PartIdTextBox");

        // Act - Enter valid data
        partIdTextBox.Text = "PART001";
        viewModel.SelectedOperation = "100";
        viewModel.Quantity = 5;
        viewModel.Location = "A01";

        // Assert
        Assert.True(saveButton.IsEnabled);
    }

    private InventoryTabViewModel CreateTestViewModel()
    {
        var mockInventoryService = new Mock<IInventoryService>();
        var mockMasterDataService = new Mock<IMasterDataService>();
        var mockLogger = new Mock<ILogger<InventoryTabViewModel>>();

        return new InventoryTabViewModel(
            mockInventoryService.Object,
            mockMasterDataService.Object,
            mockLogger.Object
        );
    }

    private void ConfigureTestServices(IServiceCollection services)
    {
        // Configure test services with mocks
        services.AddSingleton(Mock.Of<IInventoryService>());
        services.AddSingleton(Mock.Of<IMasterDataService>());
        services.AddTransient<InventoryTabViewModel>();
    }
}
```

## Test Organization and Structure

### Test Project Structure
```
MTM.Tests/
├── Unit/
│   ├── ViewModels/
│   │   ├── InventoryTabViewModelTests.cs
│   │   ├── RemoveTabViewModelTests.cs
│   │   └── TransferTabViewModelTests.cs
│   ├── Services/
│   │   ├── InventoryServiceTests.cs
│   │   ├── MasterDataServiceTests.cs
│   │   └── DatabaseServiceTests.cs
│   └── Models/
│       ├── InventoryItemTests.cs
│       └── ValidationTests.cs
├── Integration/
│   ├── Database/
│   │   ├── InventoryIntegrationTests.cs
│   │   └── TransactionIntegrationTests.cs
│   ├── Services/
│   │   └── EndToEndServiceTests.cs
│   └── Fixtures/
│       ├── DatabaseTestFixture.cs
│       └── ServiceTestFixture.cs
├── UI/
│   ├── Views/
│   │   ├── InventoryTabViewTests.cs
│   │   └── MainWindowTests.cs
│   └── Controls/
│       └── CustomControlTests.cs
├── Helpers/
│   ├── TestDataBuilders.cs
│   ├── MockHelpers.cs
│   └── AssertionHelpers.cs
└── Scripts/
    ├── test-database-setup.sql
    └── test-data-cleanup.sql
```

## Running Tests

### Command Line Test Execution
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
dotnet test --filter Category=UI

# Run tests for specific class
dotnet test --filter FullyQualifiedName~InventoryTabViewModelTests

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests in parallel
dotnet test --parallel
```

### Visual Studio Test Integration
```json
// .vscode/settings.json - Test configuration
{
    "dotnet-test-explorer.testProjectPath": "MTM.Tests",
    "dotnet-test-explorer.useTreeView": true,
    "dotnet-test-explorer.showCodeLens": true,
    "dotnet-test-explorer.codeLensFailed": "❌",
    "dotnet-test-explorer.codeLensPassed": "✅",
    "dotnet-test-explorer.codeLensSkipped": "⚠️"
}
```

## Test Data Management

### Test Data Builders
```csharp
// Test data builder for consistent test objects
public class InventoryItemBuilder
{
    private string _partId = "TEST001";
    private string _operation = "100";
    private int _quantity = 1;
    private string _location = "A01";

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

    public InventoryItemBuilder WithLocation(string location)
    {
        _location = location;
        return this;
    }

    public InventoryItem Build()
    {
        return new InventoryItem
        {
            PartId = _partId,
            Operation = _operation,
            Quantity = _quantity,
            Location = _location,
            LastUpdated = DateTime.Now,
            LastUpdatedBy = "TEST_USER"
        };
    }
}

// Usage in tests
var testItem = new InventoryItemBuilder()
    .WithPartId("PART001")
    .WithQuantity(5)
    .Build();
```

## Continuous Integration Testing

### GitHub Actions Test Configuration
```yaml
# .github/workflows/tests.yml
name: Run Tests

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      mysql:
        image: mysql:8.0
        env:
          MYSQL_ROOT_PASSWORD: TestPassword123!
          MYSQL_DATABASE: MTM_WIP_Test
        ports:
          - 3306:3306
        options: --health-cmd="mysqladmin ping" --health-interval=10s --health-timeout=5s --health-retries=3

    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Run unit tests
      run: dotnet test --no-build --filter Category=Unit --logger trx --results-directory TestResults
      
    - name: Run integration tests
      run: dotnet test --no-build --filter Category=Integration --logger trx --results-directory TestResults
      env:
        ConnectionStrings__DefaultConnection: "Server=localhost;Port=3306;Database=MTM_WIP_Test;Uid=root;Pwd=TestPassword123!;"
        
    - name: Upload test results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-results
        path: TestResults
```

## Test Quality Standards

### Code Coverage Requirements
- **Minimum Coverage**: 80% overall code coverage
- **Critical Components**: 95% coverage for ViewModels and Services
- **UI Components**: 70% coverage for Views and Controls
- **Database Layer**: 90% coverage for database operations

### Test Naming Conventions
```csharp
// Pattern: MethodName_StateUnderTest_ExpectedBehavior
[Fact]
public void SaveCommand_WithValidData_CallsInventoryService() { }

[Fact]
public void SaveCommand_WithInvalidData_ShowsErrorMessage() { }

[Fact]
public void IsFormValid_WithEmptyPartId_ReturnsFalse() { }
```

### Assertion Standards
```csharp
// Use FluentAssertions for readable assertions
result.Should().BeTrue("because the operation should succeed with valid data");
items.Should().HaveCount(5, "because we added 5 items to the collection");
exception.Should().BeOfType<ArgumentException>("because invalid input should throw ArgumentException");

// Avoid basic Assert methods
Assert.True(result); // Too basic
Assert.Equal(5, items.Count); // Less readable
```

## Test Maintenance and Best Practices

### Regular Test Maintenance
- **Weekly**: Review and update test data builders
- **Monthly**: Clean up obsolete tests and test data
- **Per Release**: Validate test coverage meets requirements
- **Continuous**: Update tests when requirements change

### Best Practices Checklist
- [ ] Tests are independent and can run in any order
- [ ] Test data is isolated and doesn't affect other tests
- [ ] Tests use descriptive names that explain the scenario
- [ ] Each test validates one specific behavior
- [ ] Tests include both positive and negative scenarios
- [ ] Integration tests use realistic data and scenarios
- [ ] UI tests validate user workflows, not implementation details
- [ ] Tests are fast and provide quick feedback
- [ ] Test failures provide clear, actionable error messages

## Related Documentation
- [Setup Environment](setup-environment.instructions.md) - Development environment configuration
- [Component Development](component-development.instructions.md) - UI component testing
- [Service Architecture](service-architecture.instructions.md) - Service testing patterns
- [MVVM Community Toolkit](mvvm-community-toolkit.instructions.md) - ViewModel testing patterns