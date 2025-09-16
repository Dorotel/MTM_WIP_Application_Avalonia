# MTM Universal Testing Framework

A comprehensive testing framework providing cross-platform testing capabilities for applications built with the MTM Universal Framework.

## 🧪 Testing Architecture

The framework provides multi-tier testing strategy:

1. **Unit Tests** - Component-level validation with 95%+ coverage
2. **Integration Tests** - Service and database interaction validation 
3. **UI Tests** - Cross-platform UI workflow testing
4. **Cross-Platform Tests** - Platform-specific behavior validation
5. **Performance Tests** - Load testing and performance benchmarks

## 📋 Testing Categories

### Unit Testing Patterns
- MVVM ViewModel testing with MVVM Community Toolkit
- Service layer testing with dependency injection
- Business logic validation with domain rules
- Model validation and serialization testing

### Integration Testing Patterns
- Database operation testing with multiple providers
- Service communication testing with messaging
- Configuration and settings integration testing
- External API integration testing with circuit breakers

### UI Testing Patterns
- Avalonia UI automation with Headless testing
- Cross-platform UI consistency validation
- Touch and mouse interaction testing
- Theme and styling validation across platforms

### Cross-Platform Testing Patterns
- Windows desktop functionality testing
- Android mobile functionality testing
- macOS and Linux compatibility testing
- Platform-specific feature validation

## 🚀 Usage Examples

### Unit Test Template
```csharp
[TestFixture]
[Category("Unit")]
public class BusinessViewModelTests : TestBase
{
    private BusinessViewModel _viewModel;
    private Mock<IBusinessService> _mockService;
    
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IBusinessService>();
        _viewModel = new BusinessViewModel(_mockService.Object, Logger);
    }
    
    [Test]
    public async Task SaveCommand_ValidData_CallsService()
    {
        // Arrange
        _viewModel.Name = "Test Item";
        _mockService.Setup(s => s.SaveAsync(It.IsAny<BusinessItem>()))
                   .ReturnsAsync(true);
        
        // Act
        await _viewModel.SaveCommand.ExecuteAsync();
        
        // Assert
        _mockService.Verify(s => s.SaveAsync(It.IsAny<BusinessItem>()), Times.Once);
        Assert.That(_viewModel.StatusMessage, Contains.Substring("saved"));
    }
}
```

### Integration Test Template
```csharp
[TestFixture]
[Category("Integration")]
public class DatabaseIntegrationTests : DatabaseTestBase
{
    [Test]
    public async Task BusinessService_SaveItem_PersistsToDatabase()
    {
        // Arrange
        var service = ServiceProvider.GetRequiredService<IBusinessService>();
        var item = new BusinessItem { Name = "Integration Test" };
        
        // Act
        var result = await service.SaveAsync(item);
        
        // Assert
        Assert.That(result, Is.True);
        
        // Verify persistence
        var retrieved = await service.GetByNameAsync("Integration Test");
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Name, Is.EqualTo("Integration Test"));
    }
}
```

### Cross-Platform Test Template
```csharp
[TestFixture]
[Category("CrossPlatform")]
public class CrossPlatformFeatureTests
{
    [Test]
    [Platform("Win")]
    public void BusinessFeature_Windows_WorksCorrectly()
    {
        // Windows-specific testing
        var result = BusinessFeature.ExecuteOnWindows();
        Assert.That(result.IsSuccess, Is.True);
    }
    
    [Test]
    [Platform("Android")]
    public void BusinessFeature_Android_WorksCorrectly()
    {
        // Android-specific testing
        var result = BusinessFeature.ExecuteOnAndroid();
        Assert.That(result.IsSuccess, Is.True);
    }
}
```

## 📊 Coverage Requirements

| Test Category | Coverage Target | Key Focus Areas |
|---------------|----------------|-----------------|
| Unit Tests | 95%+ | ViewModels, Services, Business Logic |
| Integration Tests | 90%+ | Database Operations, Service Communication |
| UI Tests | 85%+ | User Workflows, Navigation, Validation |
| Cross-Platform Tests | 100% | Platform-Specific Features |

## 🛠️ Test Infrastructure

The framework provides:
- Base test classes for common scenarios
- Mock factories for service dependencies
- Database test fixtures with cleanup
- UI test helpers for Avalonia applications
- Performance testing utilities
- Cross-platform test execution helpers

## 🔧 Configuration

Configure testing via `appsettings.test.json`:

```json
{
  "Testing": {
    "DatabaseProvider": "InMemory",
    "EnablePerformanceTests": true,
    "CrossPlatformTestsEnabled": true,
    "UITestsEnabled": true,
    "ParallelExecution": true,
    "TestResultsPath": "./TestResults"
  }
}
```

This testing framework ensures manufacturing-grade quality standards across all platforms and deployment scenarios.