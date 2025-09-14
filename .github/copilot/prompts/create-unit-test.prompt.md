---
description: 'Prompt template for creating unit tests for MTM WIP Application ViewModels and Services'
applies_to: '**/*'
---

# Create Unit Test Prompt Template

## 🎯 Objective

Generate comprehensive unit tests for MTM WIP Application components using NUnit, MVVM Community Toolkit patterns, and Moq for mocking. Focus on ViewModels, Services, and business logic components.

## 📋 Instructions

When creating unit tests, follow these specific requirements:

### Test Class Structure

1. **Use MTM Unit Test Base Class**
   ```csharp
   [TestFixture]
   [Category("Unit")]
   [Category("{ComponentCategory}")]  // e.g., ViewModel, Service, BusinessLogic
   public class {ComponentName}Tests : UnitTestBase
   {
       // Test implementation
   }
   ```

2. **Apply MVVM Community Toolkit Testing Patterns**
   - Test `[ObservableProperty]` properties for change notifications
   - Test `[RelayCommand]` commands for execution and CanExecute logic
   - Mock all external dependencies using Moq
   - Test error handling and validation scenarios

3. **Required Test Categories**
   - Property Tests: Verify `[ObservableProperty]` behavior
   - Command Tests: Test `[RelayCommand]` execution
   - Service Integration Tests: Mock service interactions
   - Error Handling Tests: Test exception scenarios
   - Validation Tests: Test input validation logic

### Component-Specific Requirements

#### ViewModel Tests
```csharp
// Test Observable Properties
[Test]
public void PartId_SetValue_ShouldNotifyPropertyChanged()
{
    // Arrange
    var viewModel = CreateViewModel();
    var propertyChangedRaised = false;
    viewModel.PropertyChanged += (s, e) => 
    {
        if (e.PropertyName == nameof(viewModel.PartId))
            propertyChangedRaised = true;
    };
    
    // Act
    viewModel.PartId = "NEW_PART_001";
    
    // Assert
    Assert.That(propertyChangedRaised, Is.True);
    Assert.That(viewModel.PartId, Is.EqualTo("NEW_PART_001"));
}

// Test Relay Commands
[Test]
public async Task SearchCommand_Execute_ShouldCallServiceAndUpdateResults()
{
    // Arrange
    var mockService = new Mock<IInventoryService>();
    var expectedResults = new List<InventoryItem> 
    { 
        new() { PartId = "TEST_001", Operation = "100", Quantity = 25 }
    };
    mockService.Setup(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
        .ReturnsAsync(expectedResults);
    
    var viewModel = CreateViewModel(mockService.Object);
    
    // Act
    await viewModel.SearchCommand.ExecuteAsync(null);
    
    // Assert
    mockService.Verify(s => s.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    Assert.That(viewModel.SearchResults, Is.EqualTo(expectedResults));
}
```

#### Service Tests
```csharp
// Test Service Methods
[Test]
public async Task GetInventoryAsync_ValidPartId_ShouldReturnInventoryItems()
{
    // Arrange
    var mockConfig = new Mock<IConfigurationService>();
    mockConfig.Setup(c => c.GetConnectionStringAsync())
        .ReturnsAsync("test_connection_string");
    
    var service = CreateService(mockConfig.Object);
    var partId = "SERVICE_TEST_001";
    var operation = "100";
    
    // Mock database result
    var mockResult = new DatabaseResult
    {
        Status = 1,
        Data = CreateMockDataTable(partId, operation, 25)
    };
    
    // Act
    var result = await service.GetInventoryAsync(partId, operation);
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Count, Is.GreaterThan(0));
    Assert.That(result.First().PartId, Is.EqualTo(partId));
}

// Test Error Scenarios
[Test]
public async Task GetInventoryAsync_DatabaseError_ShouldReturnEmptyList()
{
    // Arrange
    var service = CreateService();
    
    // Mock database error
    MockDatabaseError();
    
    // Act
    var result = await service.GetInventoryAsync("ERROR_TEST", "100");
    
    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Count, Is.EqualTo(0));
}
```

### Required Mock Setups

#### Database Service Mocking
```csharp
protected void SetupMockDatabase(string procedureName, DatabaseResult expectedResult)
{
    MockDatabaseHelper.Setup(h => h.ExecuteDataTableWithStatus(
        It.IsAny<string>(), 
        procedureName, 
        It.IsAny<MySqlParameter[]>()))
        .ReturnsAsync(expectedResult);
}

protected DataTable CreateMockDataTable(string partId, string operation, int quantity)
{
    var dataTable = new DataTable();
    dataTable.Columns.Add("PartID", typeof(string));
    dataTable.Columns.Add("OperationNumber", typeof(string));
    dataTable.Columns.Add("Quantity", typeof(int));
    dataTable.Columns.Add("Location", typeof(string));
    dataTable.Columns.Add("User", typeof(string));
    
    var row = dataTable.NewRow();
    row["PartID"] = partId;
    row["OperationNumber"] = operation;
    row["Quantity"] = quantity;
    row["Location"] = "TEST_STATION";
    row["User"] = "TestUser";
    dataTable.Rows.Add(row);
    
    return dataTable;
}
```

#### Service Dependency Mocking
```csharp
// Configuration Service Mock
var mockConfigService = new Mock<IConfigurationService>();
mockConfigService.Setup(c => c.GetConnectionStringAsync())
    .ReturnsAsync("Server=localhost;Database=test;");
mockConfigService.Setup(c => c.GetSettingAsync<string>(It.IsAny<string>()))
    .ReturnsAsync("TestValue");

// Logger Mock
var mockLogger = new Mock<ILogger<{ComponentType}>>();

// Error Handling Service Mock
var mockErrorHandler = new Mock<IErrorHandlingService>();
mockErrorHandler.Setup(e => e.HandleErrorAsync(It.IsAny<Exception>(), It.IsAny<string>()))
    .Returns(Task.CompletedTask);
```

### Test Data Patterns

#### Valid Test Data
```csharp
protected static readonly object[][] ValidInventoryTestData = 
{
    new object[] { "VALID_PART_001", "90", 25, "STATION_A" },
    new object[] { "VALID_PART_002", "100", 50, "STATION_B" },
    new object[] { "VALID_PART_003", "110", 15, "STATION_C" }
};

[TestCaseSource(nameof(ValidInventoryTestData))]
public async Task AddInventoryAsync_ValidData_ShouldSucceed(
    string partId, string operation, int quantity, string location)
{
    // Test implementation
}
```

#### Invalid Test Data
```csharp
protected static readonly object[][] InvalidInventoryTestData = 
{
    new object[] { "", "100", 10, "STATION_A", "Empty PartId" },
    new object[] { "VALID_PART", "", 10, "STATION_A", "Empty Operation" },
    new object[] { "VALID_PART", "100", 0, "STATION_A", "Zero Quantity" },
    new object[] { "VALID_PART", "100", -5, "STATION_A", "Negative Quantity" },
    new object[] { "VALID_PART", "100", 10, "", "Empty Location" }
};

[TestCaseSource(nameof(InvalidInventoryTestData))]
public async Task AddInventoryAsync_InvalidData_ShouldReturnError(
    string partId, string operation, int quantity, string location, string testDescription)
{
    // Test invalid data scenarios
}
```

### Error Testing Requirements

```csharp
[Test]
public async Task ServiceMethod_DatabaseConnectionFailure_ShouldHandleGracefully()
{
    // Arrange
    var service = CreateService();
    MockDatabaseConnectionFailure();
    
    // Act & Assert
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(
        () => service.MethodUnderTest());
    
    Assert.That(exception.Message, Does.Contain("database"));
}

[Test]
public async Task ServiceMethod_TimeoutException_ShouldRetryAndFail()
{
    // Arrange
    var service = CreateService();
    MockDatabaseTimeout();
    
    // Act
    var result = await service.MethodUnderTest();
    
    // Assert
    Assert.That(result, Is.Null.Or.Empty);
    VerifyErrorLogging(Times.AtLeastOnce());
}
```

### Cross-Platform Considerations

```csharp
[Test]
[Platform("Win")]
public void WindowsSpecific_Test() { /* Windows-only logic */ }

[Test]
[Platform("Unix")]
public void UnixSpecific_Test() { /* Unix-only logic */ }

[Test]
public async Task CrossPlatform_FileOperations_ShouldNormalizePaths()
{
    // Test path normalization across platforms
    var testPaths = new[] 
    {
        "Config\\appsettings.json",
        "Config/appsettings.json"
    };
    
    foreach (var path in testPaths)
    {
        var normalizedPath = NormalizePath(path);
        Assert.That(Path.IsPathRooted(normalizedPath) || Path.GetFileName(normalizedPath), 
            Is.Not.Null.And.Not.Empty);
    }
}
```

## ✅ Test Completeness Checklist

When creating unit tests, ensure:

- [ ] All public methods are tested
- [ ] All `[ObservableProperty]` properties have change notification tests
- [ ] All `[RelayCommand]` commands have execution and CanExecute tests
- [ ] All external dependencies are properly mocked
- [ ] Error scenarios and edge cases are covered
- [ ] Validation logic is thoroughly tested
- [ ] Cross-platform considerations are addressed
- [ ] Test data covers typical and boundary conditions
- [ ] Async operations are properly tested with await/Task patterns
- [ ] Performance-critical methods have performance assertions
- [ ] All test methods have clear Arrange-Act-Assert structure
- [ ] Test names clearly describe the scenario and expected outcome

## 🏷️ Test Categorization

Use these category attributes consistently:

```csharp
[Category("Unit")]           // All unit tests
[Category("ViewModel")]      // ViewModel tests
[Category("Service")]        // Service tests  
[Category("BusinessLogic")]  // Business logic tests
[Category("Validation")]     // Validation tests
[Category("ErrorHandling")]  // Error handling tests
[Category("Performance")]    // Performance tests
[Category("CrossPlatform")]  // Cross-platform tests
```

This template ensures comprehensive unit test coverage for all MTM WIP Application components while maintaining consistency with MVVM Community Toolkit patterns and awesome-copilot testing standards.