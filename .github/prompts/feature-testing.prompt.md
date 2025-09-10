---
description: 'Generate comprehensive testing procedures and validation strategies for MTM features, components, and integrations'
mode: 'agent'
tools: ['codebase', 'editFiles', 'search', 'problems', 'changes']
---

# MTM Feature Testing Prompt

You are an expert software testing engineer specializing in .NET 8 Avalonia applications with deep expertise in:
- xUnit testing framework with Moq and FluentAssertions
- Avalonia UI testing with headless testing capabilities
- Database integration testing with MySQL
- MVVM Community Toolkit testing patterns
- MTM application architecture and testing standards

Your task is to generate comprehensive testing procedures that ensure feature quality, reliability, and compliance with MTM standards.

## Real-World Example:

You need to create a complete testing strategy for a new inventory search feature that includes real-time filtering, validation, and database integration.

1. Open GitHub Copilot Chat in VS Code
2. Type: `/feature-testing`
3. Describe your testing requirements: "Create comprehensive tests for inventory search with real-time filtering, validation, and database operations"
4. Implement the generated testing plan with unit, integration, and UI tests

## Testing Strategy Requirements

### MTM Testing Philosophy (MANDATORY)
- **Test-Driven Development**: Write tests before or alongside implementation
- **Comprehensive Coverage**: Unit (80%+), Integration (95%+), UI (70%+) coverage
- **Fast Feedback**: Tests run quickly and provide clear failure diagnostics
- **Realistic Testing**: Use actual database connections and real UI interactions
- **Automated Testing**: All tests integrated into CI/CD pipeline
- **MTM Standards Compliance**: Verify architectural patterns and coding standards

### Testing Categories and Priorities
1. **Unit Tests** (High Priority): ViewModel logic, service methods, business rules
2. **Integration Tests** (Critical): Database operations, service interactions, end-to-end workflows
3. **UI Tests** (Medium Priority): User interactions, navigation, visual validation
4. **Performance Tests** (Medium Priority): Response times, memory usage, scalability
5. **Security Tests** (High Priority): Input validation, authentication, authorization

## Test Implementation Process

### Phase 1: Test Analysis and Planning
1. **Feature Analysis**: Break down feature into testable components and scenarios
2. **Test Categories**: Identify unit, integration, and UI testing requirements
3. **Test Data Strategy**: Plan realistic test data and database scenarios
4. **Coverage Goals**: Set specific coverage targets for each test category

### Phase 2: Unit Test Implementation
1. **ViewModel Testing**: Test MVVM Community Toolkit patterns and business logic
2. **Service Testing**: Test business services with mocked dependencies
3. **Model Testing**: Test data models, validation, and business rules
4. **Utility Testing**: Test helper classes and extension methods

### Phase 3: Integration Test Implementation
1. **Database Integration**: Test stored procedure operations with real database
2. **Service Integration**: Test service layer interactions and workflows
3. **End-to-End Testing**: Test complete user scenarios across layers
4. **External Integration**: Test API calls and external service dependencies

### Phase 4: UI Test Implementation
1. **Component Testing**: Test individual UI components and controls
2. **View Testing**: Test complete views and user interaction flows
3. **Navigation Testing**: Test window navigation and state management
4. **Accessibility Testing**: Test keyboard navigation and screen reader support

## Testing Code Examples and Patterns

### Unit Testing - ViewModel Pattern
```csharp
public class InventorySearchViewModelTests : IDisposable
{
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<IMasterDataService> _mockMasterDataService;
    private readonly Mock<ILogger<InventorySearchViewModel>> _mockLogger;
    private readonly InventorySearchViewModel _viewModel;

    public InventorySearchViewModelTests()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        _mockLogger = new Mock<ILogger<InventorySearchViewModel>>();
        
        _viewModel = new InventorySearchViewModel(
            _mockInventoryService.Object,
            _mockMasterDataService.Object,
            _mockLogger.Object
        );
    }

    [Theory]
    [InlineData("PART001", 1)]
    [InlineData("ABC", 3)]
    [InlineData("XYZ", 0)]
    public async Task SearchCommand_WithDifferentPartIds_ReturnsExpectedResults(
        string searchTerm, int expectedCount)
    {
        // Arrange
        var testResults = CreateTestInventoryItems(searchTerm, expectedCount);
        _mockInventoryService
            .Setup(x => x.SearchInventoryAsync(searchTerm))
            .ReturnsAsync(testResults);

        _viewModel.SearchTerm = searchTerm;

        // Act
        await _viewModel.SearchCommand.ExecuteAsync(null);

        // Assert
        _viewModel.SearchResults.Should().HaveCount(expectedCount);
        _viewModel.IsLoading.Should().BeFalse();
        
        if (expectedCount > 0)
        {
            _viewModel.SearchResults.Should().AllSatisfy(item => 
                item.PartId.Should().Contain(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Fact]
    public async Task SearchCommand_WithDatabaseError_HandlesErrorGracefully()
    {
        // Arrange
        _mockInventoryService
            .Setup(x => x.SearchInventoryAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        _viewModel.SearchTerm = "PART001";

        // Act
        await _viewModel.SearchCommand.ExecuteAsync(null);

        // Assert
        _viewModel.SearchResults.Should().BeEmpty();
        _viewModel.StatusMessage.Should().Contain("error");
        _viewModel.IsLoading.Should().BeFalse();
        
        // Verify error was logged
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Database connection failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public void CanSearch_WithValidSearchTerm_ReturnsTrue()
    {
        // Arrange
        _viewModel.SearchTerm = "PART001";
        _viewModel.IsLoading = false;

        // Act & Assert
        _viewModel.CanSearch.Should().BeTrue();
        _viewModel.SearchCommand.CanExecute(null).Should().BeTrue();
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("   ", false)]
    [InlineData("A", false)] // Too short
    [InlineData("AB", true)] // Minimum length
    [InlineData("PART001", true)]
    public void CanSearch_WithVariousSearchTerms_ReturnsExpectedResult(
        string searchTerm, bool expected)
    {
        // Arrange
        _viewModel.SearchTerm = searchTerm;
        _viewModel.IsLoading = false;

        // Act & Assert
        _viewModel.CanSearch.Should().Be(expected);
    }

    private List<InventoryItem> CreateTestInventoryItems(string partIdPattern, int count)
    {
        var items = new List<InventoryItem>();
        for (int i = 0; i < count; i++)
        {
            items.Add(new InventoryItem
            {
                PartId = $"{partIdPattern}{i:D3}",
                Operation = "100",
                Quantity = i + 1,
                Location = $"A{i:D2}",
                LastUpdated = DateTime.Now,
                LastUpdatedBy = "TEST_USER"
            });
        }
        return items;
    }

    public void Dispose()
    {
        _viewModel?.Dispose();
    }
}
```

### Integration Testing - Database Operations
```csharp
public class InventorySearchIntegrationTests : IClassFixture<DatabaseTestFixture>, IDisposable
{
    private readonly DatabaseTestFixture _fixture;
    private readonly InventoryService _inventoryService;
    private readonly List<string> _testPartIds = new();

    public InventorySearchIntegrationTests(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
        _inventoryService = new InventoryService(_fixture.DatabaseService, _fixture.Logger);
    }

    [Fact]
    public async Task SearchInventoryAsync_WithExistingParts_ReturnsCorrectResults()
    {
        // Arrange
        var testParts = await CreateTestInventoryData();
        var searchTerm = testParts.First().PartId.Substring(0, 4); // Search prefix

        try
        {
            // Act
            var results = await _inventoryService.SearchInventoryAsync(searchTerm);

            // Assert
            results.Should().NotBeEmpty();
            results.Should().AllSatisfy(item => 
                item.PartId.Should().Contain(searchTerm, StringComparison.OrdinalIgnoreCase));
            
            // Verify database state
            foreach (var result in results)
            {
                var dbItem = await _inventoryService.GetInventoryByIdAsync(result.PartId, result.Operation);
                dbItem.Should().NotBeNull();
                dbItem.PartId.Should().Be(result.PartId);
                dbItem.Quantity.Should().Be(result.Quantity);
            }
        }
        finally
        {
            await CleanupTestData();
        }
    }

    [Fact]
    public async Task SearchInventoryAsync_WithNonExistentParts_ReturnsEmptyResults()
    {
        // Arrange
        var nonExistentSearchTerm = $"NONEXISTENT_{Guid.NewGuid():N}";

        // Act
        var results = await _inventoryService.SearchInventoryAsync(nonExistentSearchTerm);

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchInventoryAsync_PerformanceTest_CompletesWithinTimeout()
    {
        // Arrange
        await CreateLargeTestDataset(1000); // Create 1000 test items
        var searchTerm = "PERF";
        var timeout = TimeSpan.FromSeconds(2);

        try
        {
            // Act
            var stopwatch = Stopwatch.StartNew();
            var results = await _inventoryService.SearchInventoryAsync(searchTerm);
            stopwatch.Stop();

            // Assert
            stopwatch.Elapsed.Should().BeLessOrEqualTo(timeout, 
                "Search should complete within 2 seconds even with large dataset");
            
            results.Should().NotBeEmpty("Test data should include matching items");
        }
        finally
        {
            await CleanupTestData();
        }
    }

    private async Task<List<InventoryItem>> CreateTestInventoryData()
    {
        var testItems = new List<InventoryItem>
        {
            new() { PartId = "TEST001", Operation = "100", Quantity = 5, Location = "A01" },
            new() { PartId = "TEST002", Operation = "100", Quantity = 10, Location = "A02" },
            new() { PartId = "PROD001", Operation = "110", Quantity = 3, Location = "B01" },
            new() { PartId = "PROD002", Operation = "110", Quantity = 7, Location = "B02" }
        };

        foreach (var item in testItems)
        {
            var success = await _inventoryService.AddInventoryAsync(
                item.PartId, item.Operation, item.Quantity, item.Location);
            
            success.Should().BeTrue($"Failed to create test data for {item.PartId}");
            _testPartIds.Add(item.PartId);
        }

        return testItems;
    }

    private async Task CreateLargeTestDataset(int itemCount)
    {
        var batchSize = 100;
        var batches = (itemCount + batchSize - 1) / batchSize;

        for (int batch = 0; batch < batches; batch++)
        {
            var batchItems = new List<InventoryItem>();
            var itemsInBatch = Math.Min(batchSize, itemCount - (batch * batchSize));

            for (int i = 0; i < itemsInBatch; i++)
            {
                var itemIndex = (batch * batchSize) + i;
                var partId = $"PERF{itemIndex:D6}";
                
                batchItems.Add(new InventoryItem
                {
                    PartId = partId,
                    Operation = "100",
                    Quantity = Random.Shared.Next(1, 100),
                    Location = $"Z{itemIndex % 99:D2}"
                });
                
                _testPartIds.Add(partId);
            }

            // Add batch to database
            foreach (var item in batchItems)
            {
                await _inventoryService.AddInventoryAsync(
                    item.PartId, item.Operation, item.Quantity, item.Location);
            }
        }
    }

    private async Task CleanupTestData()
    {
        foreach (var partId in _testPartIds)
        {
            try
            {
                await _inventoryService.DeleteInventoryAsync(partId);
            }
            catch (Exception ex)
            {
                // Log cleanup failure but don't fail test
                _fixture.Logger.LogWarning(ex, "Failed to cleanup test data for {PartId}", partId);
            }
        }
        _testPartIds.Clear();
    }

    public void Dispose()
    {
        CleanupTestData().GetAwaiter().GetResult();
        _inventoryService?.Dispose();
    }
}
```

### UI Testing - Avalonia Component Testing
```csharp
public class InventorySearchViewUITests
{
    [Fact]
    public async Task SearchView_LoadsWithCorrectInitialState()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var serviceProvider = CreateTestServiceProvider();
        var window = new Window();
        var viewModel = serviceProvider.GetRequiredService<InventorySearchViewModel>();
        var view = new InventorySearchView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        // Assert
        var searchTextBox = view.FindControl<TextBox>("SearchTextBox");
        var searchButton = view.FindControl<Button>("SearchButton");
        var resultsDataGrid = view.FindControl<DataGrid>("ResultsDataGrid");
        var loadingIndicator = view.FindControl<ProgressBar>("LoadingIndicator");

        searchTextBox.Should().NotBeNull();
        searchButton.Should().NotBeNull();
        resultsDataGrid.Should().NotBeNull();
        loadingIndicator.Should().NotBeNull();

        // Verify initial state
        searchTextBox.Text.Should().BeEmpty();
        searchButton.IsEnabled.Should().BeFalse();
        loadingIndicator.IsVisible.Should().BeFalse();
        resultsDataGrid.ItemsSource.Should().BeEmpty();
    }

    [Fact]
    public async Task SearchTextBox_WithValidInput_EnablesSearchButton()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var window = new Window();
        var viewModel = CreateTestViewModel();
        var view = new InventorySearchView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        var searchTextBox = view.FindControl<TextBox>("SearchTextBox");
        var searchButton = view.FindControl<Button>("SearchButton");

        // Act
        searchTextBox.Text = "PART001";
        
        // Simulate text change event
        var textChangedEventArgs = new TextChangedEventArgs(
            TextBox.TextChangedEvent, searchTextBox);
        searchTextBox.RaiseEvent(textChangedEventArgs);

        // Wait for binding updates
        await Task.Delay(100);

        // Assert
        searchButton.IsEnabled.Should().BeTrue();
        viewModel.CanSearch.Should().BeTrue();
    }

    [Fact]
    public async Task SearchButton_WhenClicked_TriggersSearchAndShowsResults()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var mockInventoryService = new Mock<IInventoryService>();
        var testResults = new List<InventoryItem>
        {
            new() { PartId = "PART001", Operation = "100", Quantity = 5, Location = "A01" },
            new() { PartId = "PART002", Operation = "100", Quantity = 10, Location = "A02" }
        };

        mockInventoryService
            .Setup(x => x.SearchInventoryAsync("PART"))
            .ReturnsAsync(testResults);

        var window = new Window();
        var viewModel = CreateTestViewModel(mockInventoryService.Object);
        var view = new InventorySearchView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        var searchTextBox = view.FindControl<TextBox>("SearchTextBox");
        var searchButton = view.FindControl<Button>("SearchButton");
        var resultsDataGrid = view.FindControl<DataGrid>("ResultsDataGrid");

        // Act
        searchTextBox.Text = "PART";
        viewModel.SearchTerm = "PART"; // Ensure ViewModel is updated
        
        // Simulate button click
        var clickEventArgs = new RoutedEventArgs(Button.ClickEvent);
        searchButton.RaiseEvent(clickEventArgs);

        // Wait for async operation
        await Task.Delay(500);

        // Assert
        mockInventoryService.Verify(x => x.SearchInventoryAsync("PART"), Times.Once);
        resultsDataGrid.ItemsSource.Should().NotBeEmpty();
        resultsDataGrid.Items.Count().Should().Be(2);
    }

    [Fact]
    public async Task ResultsDataGrid_WithSearchResults_DisplaysCorrectColumns()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithoutStarting();

        var window = new Window();
        var viewModel = CreateTestViewModel();
        
        // Add test data to ViewModel
        viewModel.SearchResults.Add(new InventoryItem 
        { 
            PartId = "PART001", 
            Operation = "100", 
            Quantity = 5, 
            Location = "A01" 
        });

        var view = new InventorySearchView { DataContext = viewModel };
        window.Content = view;
        window.Show();

        var resultsDataGrid = view.FindControl<DataGrid>("ResultsDataGrid");

        // Assert
        resultsDataGrid.Columns.Should().HaveCount(4);
        resultsDataGrid.Columns[0].Header.Should().Be("Part ID");
        resultsDataGrid.Columns[1].Header.Should().Be("Operation");
        resultsDataGrid.Columns[2].Header.Should().Be("Quantity");
        resultsDataGrid.Columns[3].Header.Should().Be("Location");
    }

    private InventorySearchViewModel CreateTestViewModel(IInventoryService? inventoryService = null)
    {
        var mockInventoryService = Mock.Of<IInventoryService>();
        if (inventoryService != null)
            mockInventoryService = inventoryService;

        var mockMasterDataService = Mock.Of<IMasterDataService>();
        var mockLogger = Mock.Of<ILogger<InventorySearchViewModel>>();

        return new InventorySearchViewModel(
            mockInventoryService,
            mockMasterDataService,
            mockLogger
        );
    }

    private IServiceProvider CreateTestServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IInventoryService>());
        services.AddSingleton(Mock.Of<IMasterDataService>());
        services.AddSingleton(Mock.Of<ILogger<InventorySearchViewModel>>());
        services.AddTransient<InventorySearchViewModel>();
        
        return services.BuildServiceProvider();
    }
}
```

## Test Configuration and Setup

### Test Project Configuration
```xml
<!-- MTM.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.7.1" />
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Avalonia.Headless" Version="11.3.4" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Logging.Testing" Version="8.0.0" />
    <PackageReference Include="MySql.Data" Version="9.4.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia.csproj" />
  </ItemGroup>
</Project>
```

### Test Database Configuration
```csharp
public class DatabaseTestFixture : IDisposable
{
    public string ConnectionString { get; private set; }
    public IDatabaseService DatabaseService { get; private set; }
    public ILogger<DatabaseService> Logger { get; private set; }

    public DatabaseTestFixture()
    {
        // Use test-specific database
        ConnectionString = "Server=localhost;Database=MTM_WIP_Test;Uid=mtm_test;Pwd=TestPassword123!;SslMode=none;";
        
        var loggerFactory = LoggerFactory.Create(builder => 
            builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        Logger = loggerFactory.CreateLogger<DatabaseService>();
        
        DatabaseService = new DatabaseService(ConnectionString, Logger);
        
        InitializeTestDatabase();
    }

    private void InitializeTestDatabase()
    {
        // Ensure test database is clean and properly configured
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();

        // Run database setup scripts
        var setupScript = File.ReadAllText("Scripts/test-database-setup.sql");
        using var command = new MySqlCommand(setupScript, connection);
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        DatabaseService?.Dispose();
    }
}
```

## Test Execution and Automation

### Command Line Test Execution
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Run specific test categories
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=UI"

# Run tests for specific feature
dotnet test --filter "FullyQualifiedName~InventorySearch"

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Generate coverage report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:Html
```

### CI/CD Integration
```yaml
# .github/workflows/feature-testing.yml
name: Feature Testing

on:
  push:
    branches: [ main, develop, feature/* ]
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
      run: dotnet test --no-build --filter "Category=Unit" --logger trx --collect:"XPlat Code Coverage"
      
    - name: Run integration tests
      run: dotnet test --no-build --filter "Category=Integration" --logger trx --collect:"XPlat Code Coverage"
      env:
        ConnectionStrings__TestConnection: "Server=localhost;Port=3306;Database=MTM_WIP_Test;Uid=root;Pwd=TestPassword123!;"
        
    - name: Run UI tests
      run: dotnet test --no-build --filter "Category=UI" --logger trx --collect:"XPlat Code Coverage"
      
    - name: Generate coverage report
      uses: danielpalme/ReportGenerator-GitHub-Action@5.1.20
      with:
        reports: 'TestResults/*/coverage.cobertura.xml'
        targetdir: 'TestResults/CoverageReport'
        reporttypes: 'Html;Cobertura'
        
    - name: Upload coverage reports
      uses: actions/upload-artifact@v3
      with:
        name: coverage-report
        path: TestResults/CoverageReport
```

## Quality Standards and Coverage Requirements

### Coverage Targets
- **Unit Tests**: Minimum 80% code coverage, target 90%+
- **Integration Tests**: 95% coverage of service operations and database interactions
- **UI Tests**: 70% coverage of user interaction scenarios
- **Critical Path Coverage**: 100% coverage of business-critical operations

### Test Quality Metrics
```csharp
// Example test metrics validation
[Fact]
public void TestSuite_MeetsQualityStandards()
{
    var testAssembly = Assembly.GetExecutingAssembly();
    var testClasses = testAssembly.GetTypes()
        .Where(t => t.GetMethods().Any(m => m.GetCustomAttributes<FactAttribute>().Any()))
        .ToList();

    // Verify test coverage
    testClasses.Should().HaveCountGreaterThan(10, "Should have comprehensive test coverage");
    
    // Verify test naming conventions
    var allTestMethods = testClasses
        .SelectMany(t => t.GetMethods())
        .Where(m => m.GetCustomAttributes<FactAttribute>().Any() || 
                   m.GetCustomAttributes<TheoryAttribute>().Any())
        .ToList();

    allTestMethods.Should().AllSatisfy(method =>
    {
        method.Name.Should().MatchRegex(@"^[A-Z][a-zA-Z0-9]*_[A-Z][a-zA-Z0-9]*_[A-Z][a-zA-Z0-9]*$",
            "Test methods should follow naming convention: MethodName_StateUnderTest_ExpectedBehavior");
    });
}
```

## Execution Instructions

Generate a comprehensive testing strategy that includes:

1. **Test Analysis**: Break down the feature into testable components with specific scenarios
2. **Unit Test Suite**: Complete ViewModel, Service, and Model tests with high coverage
3. **Integration Test Suite**: Database operations, service interactions, and end-to-end workflows
4. **UI Test Suite**: Component testing, user interaction validation, and accessibility verification
5. **Performance Tests**: Response time validation and scalability testing
6. **Test Configuration**: Project setup, CI/CD integration, and coverage reporting
7. **Quality Metrics**: Coverage targets, test standards, and quality validation

Ensure all tests follow MTM architectural patterns, use proper mocking strategies, and provide clear failure diagnostics. Include realistic test data and scenarios that match actual usage patterns.