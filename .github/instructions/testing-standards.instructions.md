---
description: 'Comprehensive testing standards for MTM WIP Application following awesome-copilot patterns'
applies_to: '**/*'
---

# MTM Testing Standards Instructions

## 🎯 Overview

These instructions define the comprehensive testing standards for the MTM WIP Application, ensuring consistent test implementation across all components with cross-platform validation for every feature.

## 📋 Testing Architecture

### 5-Tier Testing Strategy

1. **Unit Tests** - Individual component validation
2. **Integration Tests** - Service interaction and database validation  
3. **UI Automation Tests** - User interface workflow testing
4. **Cross-Platform Feature Tests** - Each feature validated on Windows/macOS/Linux/Android
5. **End-to-End Tests** - Complete manufacturing operator workflows

### Cross-Platform Feature Requirements

**EVERY MTM feature MUST have cross-platform tests covering:**

| Feature Category | Platforms Required | Test Focus |
|------------------|-------------------|------------|
| Inventory Management | Windows, macOS, Linux, Android | Database operations, UI rendering, file I/O |
| QuickButtons System | Windows, macOS, Linux | Transaction shortcuts, database persistence |
| Remove Operations | Windows, macOS, Linux | Bulk operations, data validation |
| Master Data Management | Windows, macOS, Linux | CRUD operations, data integrity |
| Settings System | Windows, macOS, Linux, Android | Configuration persistence, theme switching |
| Navigation System | Windows, macOS, Linux | Tab routing, view lifecycle |
| Database Operations | Windows, macOS, Linux | All 45+ stored procedures |
| File Operations | Windows, macOS, Linux, Android | Cross-platform file handling |
| Print System | Windows, macOS | Platform-specific printing |
| Error Handling | Windows, macOS, Linux, Android | Error propagation, logging |
| Theme System | Windows, macOS, Linux | UI rendering consistency |
| User Sessions | Windows, macOS, Linux | Session persistence |

## 🧪 Testing Patterns

### Unit Test Pattern (MVVM Community Toolkit)

```csharp
[TestFixture]
[Category("Unit")]
public class InventoryTabViewModelTests
{
    private Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private Mock<IInventoryService> _mockService;
    private InventoryTabViewModel _viewModel;
    
    [SetUp]
    public void SetUp()
    {
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        _mockService = new Mock<IInventoryService>();
        _viewModel = new InventoryTabViewModel(_mockLogger.Object, _mockService.Object);
    }
    
    [Test]
    public async Task SaveCommand_ValidData_CallsService()
    {
        // Arrange - Set up MVVM Community Toolkit properties
        _viewModel.PartId = "TEST001";
        _viewModel.Operation = "100";
        _viewModel.Quantity = 10;
        
        _mockService.Setup(s => s.SaveInventoryAsync(It.IsAny<InventoryItem>()))
                   .ReturnsAsync(ServiceResult.Success());
        
        // Act - Execute RelayCommand
        await _viewModel.SaveCommand.ExecuteAsync(null);
        
        // Assert - Verify service interaction
        _mockService.Verify(s => s.SaveInventoryAsync(
            It.Is<InventoryItem>(i => i.PartId == "TEST001")), Times.Once);
    }
}
```

### Integration Test Pattern (Database + Services)

```csharp
[TestFixture]  
[Category("Integration")]
public class DatabaseIntegrationTests
{
    private string _connectionString;
    
    [SetUp]
    public void SetUp()
    {
        _connectionString = "Server=localhost;Database=mtm_test;Uid=test_user;Pwd=test_password;";
    }
    
    [Test]
    public async Task StoredProcedure_inv_inventory_Add_Item_ValidParameters_ReturnsSuccess()
    {
        // Arrange - Use actual stored procedure names from MTM database
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "TEST001"),
            new("p_OperationNumber", "100"),
            new("p_Quantity", 10),
            new("p_Location", "STATION_A"),
            new("p_User", "TestUser")
        };
        
        // Act - Use MTM Helper pattern
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, "inv_inventory_Add_Item", parameters);
            
        // Assert - Validate MTM status pattern
        Assert.That(result.Status, Is.EqualTo(1), "Status should be 1 for success");
        Assert.That(result.Data, Is.Not.Null, "DataTable should be returned");
    }
}
```

### Cross-Platform Feature Test Pattern

```csharp
[TestFixture]
[Category("CrossPlatform")]
public class InventoryManagementCrossPlatformTests
{
    [Test]
    [Platform("Win")]
    public async Task InventoryManagement_Windows_AllFeaturesWork()
    {
        var results = await ValidateInventoryFeatures();
        
        Assert.That(results.DatabaseOperations, Is.True, "Database operations should work on Windows");
        Assert.That(results.UIRendering, Is.True, "UI should render correctly on Windows");
        Assert.That(results.FileOperations, Is.True, "File operations should work on Windows");
        Assert.That(results.WindowsSpecific.PrintingSupport, Is.True, "Printing should work on Windows");
    }
    
    [Test]
    [Platform("Mac")]
    public async Task InventoryManagement_macOS_CoreFeaturesWork()
    {
        var results = await ValidateInventoryFeatures();
        
        Assert.That(results.DatabaseOperations, Is.True, "Database operations should work on macOS");
        Assert.That(results.UIRendering, Is.True, "UI should render correctly on macOS"); 
        Assert.That(results.MacOSSpecific.CocoaIntegration, Is.True, "Cocoa integration should work");
    }
    
    [Test]
    [Platform("Linux")]
    public async Task InventoryManagement_Linux_CoreFeaturesWork()
    {
        var results = await ValidateInventoryFeatures();
        
        Assert.That(results.DatabaseOperations, Is.True, "Database operations should work on Linux");
        Assert.That(results.UIRendering, Is.True, "UI should render correctly on Linux");
        Assert.That(results.LinuxSpecific.GTKIntegration, Is.True, "GTK integration should work");
    }
    
    [Test]
    [Platform("Android")]
    public async Task InventoryManagement_Android_MobileFeaturesWork()
    {
        var results = await ValidateMobileInventoryFeatures();
        
        Assert.That(results.DatabaseOperations, Is.True, "Database operations should work on Android");
        Assert.That(results.TouchInterface, Is.True, "Touch interface should work on Android");
        Assert.That(results.AndroidSpecific.StorageAccessFramework, Is.True, "SAF should work");
    }
}
```

### UI Automation Test Pattern (Avalonia)

```csharp
[TestFixture]
[Category("UIAutomation")]
public class InventoryTabViewUITests
{
    private Application _app;
    private MainWindow _mainWindow;
    
    [SetUp]
    public void SetUp()
    {
        _app = AvaloniaApp.BuildAvaloniaApp()
                         .UseHeadless()
                         .StartWithClassicDesktopLifetime(Array.Empty<string>());
                         
        _mainWindow = _app.GetMainWindow();
    }
    
    [Test]
    public async Task InventoryTab_SaveOperation_UpdatesUIAndDatabase()
    {
        // Navigate to Inventory tab
        var inventoryView = _mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Fill form fields
        var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
        await partTextBox.SetTextAsync("UI_TEST_001");
        
        // Execute save operation
        var saveButton = inventoryView.Get<Button>("SaveButton");
        saveButton.Click();
        
        // Verify success overlay appears
        var successOverlay = await _mainWindow.WaitForElementAsync<SuccessOverlay>();
        Assert.That(successOverlay, Is.Not.Null, "Success overlay should appear");
    }
}
```

## 🗄️ Database Testing Requirements

### Stored Procedure Coverage (45+ Procedures)

**Inventory Procedures:**
- `inv_inventory_Add_Item` - Add inventory with validation
- `inv_inventory_Remove_Item` - Remove inventory with constraints
- `inv_inventory_Get_ByPartID` - Retrieve inventory data
- `inv_inventory_Get_ByPartIDandOperation` - Specific operation data
- `inv_inventory_Update_Quantity` - Update inventory quantities

**Transaction Procedures:**
- `inv_transaction_Add` - Record transaction history
- `inv_transaction_Get_History` - Retrieve transaction logs
- `inv_transaction_Get_ByUser` - User-specific transactions

**Master Data Procedures:**
- `md_part_ids_Get_All` - Load all part IDs
- `md_locations_Get_All` - Load all locations
- `md_operation_numbers_Get_All` - Load all operations

**QuickButtons Procedures:**
- `qb_quickbuttons_Get_ByUser` - User QuickButtons
- `qb_quickbuttons_Save` - Save QuickButton
- `qb_quickbuttons_Remove` - Remove QuickButton
- `qb_quickbuttons_Clear_ByUser` - Clear user QuickButtons

**User Management Procedures:**
- `usr_users_Get_All` - Load all users
- `usr_users_Add` - Add new user
- `usr_users_Update` - Update user details
- `usr_ui_settings_GetJsonSetting` - Get user settings
- `usr_ui_settings_SetJsonSetting` - Save user settings

**Error Logging Procedures:**
- `log_error_Add_Error` - Log application errors
- `log_error_Get_All` - Retrieve error logs

### Database Test Pattern

```csharp
[TestFixture]
[Category("Database")]
public class StoredProcedureTests
{
    [TestCase("inv_inventory_Add_Item", new object[] { "TEST001", "100", 10, "STATION_A", "TestUser" })]
    [TestCase("md_part_ids_Get_All", new object[] { })]
    [TestCase("qb_quickbuttons_Get_ByUser", new object[] { "TestUser" })]
    public async Task StoredProcedure_ValidParameters_ReturnsExpectedResult(string procedureName, object[] parameters)
    {
        // Convert parameters to MySqlParameter array
        var mysqlParams = ConvertToMySqlParameters(procedureName, parameters);
        
        // Execute using MTM Helper pattern
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString, procedureName, mysqlParams);
            
        // Validate MTM status pattern (1 = success, 0 = warning, -1 = error)
        Assert.That(result.Status, Is.GreaterThanOrEqualTo(0), 
            $"Stored procedure {procedureName} should not return error status");
    }
}
```

## 🎨 UI Testing Requirements

### View Coverage (7+ Major Views)

**Primary Views:**
- `MainView.axaml` - Application shell and navigation
- `InventoryTabView.axaml` - Inventory management interface
- `QuickButtonsView.axaml` - Transaction shortcuts
- `RemoveTabView.axaml` - Inventory removal operations
- `AdvancedRemoveView.axaml` - Bulk removal operations
- `SettingsForm.axaml` - Configuration management
- `PrintView.axaml` - Report generation

**Secondary Views:**
- All SettingsForm sub-panels
- Transaction history views
- Master data management views
- About and help views

### UI Test Categories

1. **Layout Tests** - Verify UI elements render correctly
2. **Interaction Tests** - Validate user input and commands
3. **Data Binding Tests** - Ensure ViewModel-View synchronization
4. **Theme Tests** - Validate MTM design system compliance
5. **Responsive Tests** - Check various window sizes
6. **Accessibility Tests** - Keyboard navigation and screen reader support

## ⚡ Performance Testing Requirements

### Performance Targets

| Operation | Target Time | Test Scenario |
|-----------|-------------|---------------|
| Inventory Save | < 2 seconds | Single item with database write |
| QuickButtons Load | < 1 second | Load 10 recent transactions |
| Master Data Load | < 3 seconds | Load all part IDs, locations, operations |
| UI Navigation | < 500ms | Switch between tabs |
| Database Query | < 1 second | Complex inventory lookup |
| File Export | < 5 seconds | Export 100+ inventory items |

### Load Testing Scenarios

1. **High-Volume Transactions** - 100+ concurrent inventory operations
2. **Database Stress** - Multiple simultaneous stored procedure calls
3. **UI Responsiveness** - Heavy data loading while UI remains interactive
4. **Memory Usage** - Extended application usage without leaks
5. **Cross-Platform Performance** - Consistent performance across platforms

## 🔧 Test Organization

### Directory Structure

```
Tests/
├── UnitTests/
│   ├── ViewModels/           # All ViewModel unit tests
│   ├── Services/             # All Service unit tests
│   └── Models/               # Model validation tests
├── IntegrationTests/
│   ├── DatabaseTests/        # Stored procedure integration
│   ├── ServiceIntegrationTests/ # Cross-service communication
│   └── ConfigurationTests/   # Settings and configuration
├── UITests/
│   ├── ViewTests/           # Individual view testing
│   ├── WorkflowTests/       # Complete user workflows
│   └── ThemeTests/          # UI rendering and theme validation
├── CrossPlatformTests/
│   ├── FeatureTests/        # Each feature on each platform
│   ├── PlatformSpecificTests/ # Platform-unique functionality
│   └── CompatibilityTests/  # Cross-platform compatibility
├── PerformanceTests/
│   ├── LoadTests/           # High-volume scenarios
│   ├── StressTests/         # System limit testing
│   └── BenchmarkTests/      # Performance benchmarking
└── E2ETests/
    ├── ManufacturingWorkflows/ # Complete operator journeys
    ├── DataIntegrityTests/  # End-to-end data validation
    └── ErrorRecoveryTests/  # Application resilience
```

### Test Naming Conventions

**Unit Tests:**
- `{ComponentName}Tests.cs`
- `Should_{ExpectedBehavior}_When_{Condition}()`

**Integration Tests:**
- `{FeatureName}IntegrationTests.cs`  
- `{Feature}_Should_{Behavior}_With_{Context}()`

**Cross-Platform Tests:**
- `{FeatureName}CrossPlatformTests.cs`
- `{Feature}_{Platform}_Should_{Behavior}()`

**UI Tests:**
- `{ViewName}UITests.cs`
- `{View}_Should_{UIBehavior}_When_{UserAction}()`

## 📊 Coverage Requirements

### Minimum Coverage Targets

| Test Category | Coverage Target | Validation Method |
|---------------|----------------|-------------------|
| Unit Tests | 95%+ | Code coverage analysis |
| Integration Tests | 100% stored procedures | Database procedure inventory |
| UI Tests | 100% major workflows | User story validation |
| Cross-Platform Tests | 100% features | Feature matrix validation |
| E2E Tests | 100% operator journeys | Business process coverage |

### Quality Gates

1. **All tests must pass** on target platforms before merge
2. **No reduction in code coverage** from baseline
3. **Performance benchmarks** must meet targets
4. **Cross-platform compatibility** validated on CI/CD
5. **Database integrity** maintained across test runs

## 🚦 CI/CD Integration

### GitHub Actions Requirements

- **Multi-Platform Execution** - Windows, macOS, Linux runners
- **Database Services** - MySQL containers for integration tests
- **Artifact Collection** - Test results, coverage reports, performance metrics
- **Failure Reporting** - Detailed failure analysis and platform-specific issues
- **Performance Monitoring** - Regression detection and alerting

This comprehensive testing strategy ensures the MTM WIP Application maintains the highest quality standards across all platforms while providing manufacturing-grade reliability and performance.