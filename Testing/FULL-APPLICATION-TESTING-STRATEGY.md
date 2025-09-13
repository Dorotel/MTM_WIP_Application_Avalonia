# Complete MTM Application Testing Strategy

## 🎯 Full Application Testing Overview

Testing the **entire MTM WIP Application** requires a comprehensive approach that covers all components, user workflows, and platform variations. This goes far beyond individual service testing to validate the complete manufacturing inventory management system.

## 🏗️ MTM Application Architecture

### **Application Components Analysis**
Based on code analysis, the MTM application includes:

#### **User Interface Layer (7 Major Views)**
- **MainView** - Application shell with tab navigation and status display
- **InventoryTabView** - Primary inventory management interface
- **QuickButtonsView** - Recent transaction shortcuts and rapid actions
- **RemoveTabView** - Inventory removal and transfer operations  
- **AdvancedRemoveView** - Bulk operations and complex removals
- **SettingsForm** - Application configuration and theme management
- **PrintView** - Report generation and printing functionality

#### **Service Layer (15+ Services)**
- **Database Service** - MySQL stored procedure execution
- **QuickButtons Service** - Recent transaction management
- **Configuration Service** - Application settings and user preferences
- **Theme Service** - UI theming and appearance management
- **Navigation Service** - View routing and state management
- **Error Handling Service** - Centralized error management
- **Success/Suggestion Overlay Services** - User feedback systems
- **File Selection Service** - Cross-platform file operations
- **Master Data Service** - Parts, operations, locations management
- **Print Service** - Report generation and output
- **Focus Management Service** - UI focus and keyboard navigation
- **Application State Service** - User session and preferences

#### **Data Layer (45+ Stored Procedures)**
- **Inventory Operations**: `inv_inventory_Add_Item`, `inv_inventory_Remove_Item`, etc.
- **Transaction Management**: `inv_transaction_Add`, `inv_transaction_Get_History`, etc.
- **Master Data**: `md_part_ids_Get_All`, `md_locations_Get_All`, etc.
- **User Management**: `usr_users_Get_All`, user preference procedures
- **Error Logging**: `log_error_Add_Error`, error tracking procedures

#### **Business Logic Layer**
- **Manufacturing Workflows** - Add/Remove/Transfer inventory operations
- **Transaction History** - Session and permanent transaction tracking
- **Quick Action Management** - Frequently used operation shortcuts
- **User Preference Management** - Settings persistence and application
- **Validation Logic** - Data integrity and business rule enforcement

## 🧪 Comprehensive Testing Strategy

### **1. Unit Testing (Individual Components)**

#### **ViewModel Unit Tests**
```csharp
// Tests/UnitTests/ViewModels/InventoryTabViewModelTests.cs
[TestFixture]
public class InventoryTabViewModelTests
{
    private Mock<IDatabase> _mockDatabase;
    private Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private InventoryTabViewModel _viewModel;
    
    [SetUp]
    public void SetUp()
    {
        _mockDatabase = new Mock<IDatabase>();
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        _viewModel = new InventoryTabViewModel(_mockLogger.Object, _mockDatabase.Object);
    }
    
    [Test]
    public async Task Should_Save_Inventory_Item_Successfully()
    {
        // Arrange
        _viewModel.SelectedPart = "PART001";
        _viewModel.SelectedOperation = "100";
        _viewModel.Quantity = 10;
        _viewModel.SelectedLocation = "STATION_A";
        
        _mockDatabase.Setup(db => db.ExecuteStoredProcedureWithStatus(
            It.IsAny<string>(),
            It.Is<string>(sp => sp == "inv_inventory_Add_Item"),
            It.IsAny<MySqlParameter[]>()
        )).ReturnsAsync(new DatabaseResult { Status = 1, Message = "Success" });
        
        // Act
        await _viewModel.SaveInventoryItemCommand.ExecuteAsync(null);
        
        // Assert
        Assert.IsFalse(_viewModel.HasValidationErrors);
        _mockDatabase.Verify(db => db.ExecuteStoredProcedureWithStatus(
            It.IsAny<string>(),
            "inv_inventory_Add_Item",
            It.IsAny<MySqlParameter[]>()
        ), Times.Once);
    }
    
    [Test]
    public void Should_Validate_Required_Fields()
    {
        // Arrange - Empty form
        
        // Act
        var isValid = _viewModel.ValidateForm();
        
        // Assert
        Assert.IsFalse(isValid);
        Assert.IsTrue(_viewModel.HasValidationErrors);
    }
    
    [Test]
    public void Should_Update_Properties_With_PropertyChanged_Events()
    {
        // Arrange
        bool propertyChangedFired = false;
        _viewModel.PropertyChanged += (s, e) => 
        {
            if (e.PropertyName == nameof(InventoryTabViewModel.SelectedPart))
                propertyChangedFired = true;
        };
        
        // Act
        _viewModel.SelectedPart = "PART001";
        
        // Assert
        Assert.IsTrue(propertyChangedFired);
        Assert.AreEqual("PART001", _viewModel.SelectedPart);
    }
}
```

#### **Service Unit Tests**
```csharp
// Tests/UnitTests/Services/QuickButtonsServiceTests.cs
[TestFixture]
public class QuickButtonsServiceTests
{
    private Mock<IDatabase> _mockDatabase;
    private Mock<ILogger<QuickButtonsService>> _mockLogger;
    private QuickButtonsService _service;
    
    [SetUp]
    public void SetUp()
    {
        _mockDatabase = new Mock<IDatabase>();
        _mockLogger = new Mock<ILogger<QuickButtonsService>>();
        _service = new QuickButtonsService(_mockLogger.Object, _mockDatabase.Object);
    }
    
    [Test]
    public async Task Should_Load_Recent_Transactions()
    {
        // Arrange
        var mockDataTable = CreateMockTransactionDataTable();
        _mockDatabase.Setup(db => db.ExecuteDataTableWithStatus(
            It.IsAny<string>(),
            "qb_quick_buttons_Get_Last10",
            It.IsAny<MySqlParameter[]>()
        )).ReturnsAsync(new DatabaseResult<DataTable> 
        { 
            Status = 1, 
            Data = mockDataTable 
        });
        
        // Act
        var result = await _service.LoadLast10TransactionsAsync();
        
        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count); // Mock data has 5 items
        Assert.AreEqual("PART001", result.First().PartId);
    }
    
    private DataTable CreateMockTransactionDataTable()
    {
        var table = new DataTable();
        table.Columns.Add("PartID", typeof(string));
        table.Columns.Add("OperationNumber", typeof(string));
        table.Columns.Add("Quantity", typeof(int));
        table.Columns.Add("Location", typeof(string));
        table.Columns.Add("TransactionDate", typeof(DateTime));
        
        table.Rows.Add("PART001", "100", 10, "STATION_A", DateTime.Now);
        table.Rows.Add("PART002", "110", 15, "STATION_B", DateTime.Now);
        table.Rows.Add("PART003", "90", 5, "STATION_C", DateTime.Now);
        table.Rows.Add("PART004", "100", 20, "STATION_A", DateTime.Now);
        table.Rows.Add("PART005", "120", 8, "STATION_D", DateTime.Now);
        
        return table;
    }
}
```

### **2. Integration Testing (Service Interactions)**

#### **Database Integration Tests**
```csharp
// Tests/IntegrationTests/DatabaseIntegrationTests.cs
[TestFixture]
[Category("Integration")]
public class DatabaseIntegrationTests
{
    private string _connectionString;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _connectionString = GetTestDatabaseConnectionString();
        SetupTestDatabase();
    }
    
    [OneTimeTearDown] 
    public void OneTimeTearDown()
    {
        CleanupTestDatabase();
    }
    
    [Test]
    public async Task Should_Execute_All_Inventory_Stored_Procedures()
    {
        var testCases = new[]
        {
            ("inv_inventory_Add_Item", new[] 
            { 
                new MySqlParameter("p_PartID", "TEST001"),
                new MySqlParameter("p_OperationNumber", "100"),
                new MySqlParameter("p_Quantity", 10),
                new MySqlParameter("p_Location", "TEST_LOC"),
                new MySqlParameter("p_User", "TestUser")
            }),
            ("inv_inventory_Get_ByPartID", new[]
            {
                new MySqlParameter("p_PartID", "TEST001")
            }),
            ("inv_inventory_Remove_Item", new[]
            {
                new MySqlParameter("p_PartID", "TEST001"),
                new MySqlParameter("p_OperationNumber", "100"),
                new MySqlParameter("p_Quantity", 5),
                new MySqlParameter("p_Location", "TEST_LOC"),
                new MySqlParameter("p_User", "TestUser")
            })
        };
        
        foreach (var (procedureName, parameters) in testCases)
        {
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString, procedureName, parameters);
                
            Assert.AreEqual(1, result.Status, 
                $"Stored procedure {procedureName} failed with status {result.Status}");
        }
    }
    
    [Test]
    [Platform("Win,Mac,Linux")]
    public async Task Should_Connect_To_Database_On_All_Platforms()
    {
        var platformConnectionString = GetPlatformSpecificConnectionString();
        
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            platformConnectionString,
            "md_part_ids_Get_All",
            Array.Empty<MySqlParameter>()
        );
        
        Assert.AreEqual(1, result.Status, "Database connection failed on current platform");
        Assert.IsNotNull(result.Data, "No data returned from database");
    }
    
    private string GetPlatformSpecificConnectionString()
    {
        var baseConnection = "server=localhost;database=mtm_test;";
        
        if (OperatingSystem.IsWindows())
            return baseConnection + "uid=mtm_win_user;pwd=test_password;";
        else if (OperatingSystem.IsMacOS())
            return baseConnection + "uid=mtm_mac_user;pwd=test_password;";
        else if (OperatingSystem.IsLinux())
            return baseConnection + "uid=mtm_linux_user;pwd=test_password;";
        else
            return baseConnection + "uid=mtm_default_user;pwd=test_password;";
    }
}
```

#### **Cross-Service Integration Tests**
```csharp
// Tests/IntegrationTests/ServiceIntegrationTests.cs
[TestFixture]
[Category("Integration")]
public class ServiceIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private IDatabase _database;
    private IQuickButtonsService _quickButtonsService;
    private IConfigurationService _configurationService;
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();
        ConfigureTestServices(services);
        _serviceProvider = services.BuildServiceProvider();
        
        _database = _serviceProvider.GetRequiredService<IDatabase>();
        _quickButtonsService = _serviceProvider.GetRequiredService<IQuickButtonsService>();
        _configurationService = _serviceProvider.GetRequiredService<IConfigurationService>();
    }
    
    [Test]
    public async Task Should_Update_QuickButtons_After_Inventory_Save()
    {
        // Arrange - Save an inventory item
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        
        await inventoryService.SaveInventoryItemAsync(new InventoryItem
        {
            PartId = "TEST_INTEGRATION_001",
            Operation = "100",
            Quantity = 15,
            Location = "TEST_STATION",
            User = "TestUser"
        });
        
        // Act - Load QuickButtons to see if it was updated
        var quickButtons = await _quickButtonsService.LoadLast10TransactionsAsync();
        
        // Assert
        Assert.IsTrue(quickButtons.Any(qb => qb.PartId == "TEST_INTEGRATION_001"),
            "QuickButtons should be updated after inventory save");
    }
    
    [Test]
    public async Task Should_Maintain_Configuration_Across_Service_Calls()
    {
        // Arrange
        var testSetting = "TestValue_" + Guid.NewGuid();
        
        // Act - Save configuration
        await _configurationService.SaveSettingAsync("TestKey", testSetting);
        
        // Reload configuration service
        var newConfigService = _serviceProvider.GetRequiredService<IConfigurationService>();
        var retrievedSetting = await newConfigService.GetSettingAsync<string>("TestKey");
        
        // Assert
        Assert.AreEqual(testSetting, retrievedSetting);
    }
}
```

### **3. UI Automation Testing (Full User Workflows)**

#### **Avalonia UI Test Framework**
```csharp
// Tests/UITests/InventoryWorkflowUITests.cs
[TestFixture]
[Category("UIAutomation")]
public class InventoryWorkflowUITests
{
    private MTMTestApplication _app;
    private TestMainWindow _mainWindow;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _app = new MTMTestApplication();
        await _app.InitializeAsync();
        _mainWindow = _app.GetMainWindow();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _app.ShutdownAsync();
    }
    
    [Test]
    public async Task Should_Complete_Full_Inventory_Add_Workflow()
    {
        // Step 1: Navigate to Inventory tab
        var inventoryTabButton = _mainWindow.GetTabButton("Inventory");
        inventoryTabButton.Click();
        
        var inventoryView = _mainWindow.Get<InventoryTabView>("InventoryTabView");
        Assert.IsNotNull(inventoryView);
        
        // Step 2: Fill inventory form
        var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
        await partTextBox.SetTextAsync("UITEST001");
        
        var operationTextBox = inventoryView.Get<TextBox>("OperationTextBox");
        await operationTextBox.SetTextAsync("100");
        
        var quantityTextBox = inventoryView.Get<TextBox>("QuantityTextBox");
        await quantityTextBox.SetTextAsync("25");
        
        var locationTextBox = inventoryView.Get<TextBox>("LocationTextBox");
        await locationTextBox.SetTextAsync("UI_STATION");
        
        // Step 3: Save the inventory item
        var saveButton = inventoryView.Get<Button>("SaveButton");
        Assert.IsTrue(saveButton.IsEnabled, "Save button should be enabled with valid data");
        
        saveButton.Click();
        
        // Step 4: Verify success overlay appears
        var successOverlay = await _mainWindow.WaitForElementAsync<SuccessOverlay>(timeout: TimeSpan.FromSeconds(5));
        Assert.IsNotNull(successOverlay, "Success overlay should appear after saving");
        
        // Step 5: Verify form is reset/cleared
        await Task.Delay(2000); // Allow overlay to disappear
        
        Assert.IsEmpty(partTextBox.Text, "Part field should be cleared after save");
        Assert.IsEmpty(operationTextBox.Text, "Operation field should be cleared after save");
        
        // Step 6: Verify QuickButtons is updated
        var quickButtonsView = _mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        var buttons = quickButtonsView.GetQuickButtons();
        
        Assert.IsTrue(buttons.Any(b => b.PartId == "UITEST001"), 
            "QuickButtons should contain the newly added item");
    }
    
    [Test]
    public async Task Should_Execute_QuickButton_And_Populate_Form()
    {
        // Prerequisite: Ensure there are QuickButtons available
        var quickButtonsView = _mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        var quickButtons = quickButtonsView.GetQuickButtons();
        
        Assume.That(quickButtons.Count, Is.GreaterThan(0), "QuickButtons should be available for testing");
        
        // Step 1: Click the first QuickButton
        var firstButton = quickButtons.First();
        var originalPartId = firstButton.PartId;
        
        firstButton.Click();
        
        // Step 2: Navigate to Inventory tab to see populated form
        var inventoryTabButton = _mainWindow.GetTabButton("Inventory");
        inventoryTabButton.Click();
        
        var inventoryView = _mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Step 3: Verify form is populated with QuickButton data
        var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
        Assert.AreEqual(originalPartId, partTextBox.Text, 
            "Part field should be populated with QuickButton data");
            
        var operationTextBox = inventoryView.Get<TextBox>("OperationTextBox");
        Assert.IsNotEmpty(operationTextBox.Text, 
            "Operation field should be populated");
            
        var quantityTextBox = inventoryView.Get<TextBox>("QuantityTextBox");
        Assert.IsNotEmpty(quantityTextBox.Text, 
            "Quantity field should be populated");
    }
    
    [Test]
    public async Task Should_Handle_Keyboard_Navigation()
    {
        // Navigate to Inventory tab
        var inventoryView = _mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Start with Part field focused
        var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
        partTextBox.Focus();
        
        // Enter part ID and press Tab to move to next field
        await partTextBox.SetTextAsync("KEYTEST001");
        await _app.SendKeysAsync("{TAB}");
        
        // Should now be on Operation field
        var operationTextBox = inventoryView.Get<TextBox>("OperationTextBox");
        Assert.IsTrue(operationTextBox.IsFocused, "Operation field should have focus after Tab");
        
        // Continue with Tab navigation
        await operationTextBox.SetTextAsync("100");
        await _app.SendKeysAsync("{TAB}");
        
        var quantityTextBox = inventoryView.Get<TextBox>("QuantityTextBox");
        Assert.IsTrue(quantityTextBox.IsFocused, "Quantity field should have focus after Tab");
        
        await quantityTextBox.SetTextAsync("10");
        await _app.SendKeysAsync("{TAB}");
        
        var locationTextBox = inventoryView.Get<TextBox>("LocationTextBox");
        Assert.IsTrue(locationTextBox.IsFocused, "Location field should have focus after Tab");
        
        await locationTextBox.SetTextAsync("KEY_STATION");
        
        // Press Enter to save (keyboard shortcut)
        await _app.SendKeysAsync("{ENTER}");
        
        // Verify save occurred
        var successOverlay = await _mainWindow.WaitForElementAsync<SuccessOverlay>(timeout: TimeSpan.FromSeconds(3));
        Assert.IsNotNull(successOverlay, "Save should work via Enter key");
    }
}
```

### **4. End-to-End User Journey Testing**

#### **Manufacturing Operator Complete Workflow**
```csharp
// Tests/E2ETests/ManufacturingOperatorJourneyTests.cs
[TestFixture]
[Category("E2E")]
public class ManufacturingOperatorJourneyTests
{
    private MTMTestApplication _app;
    private TestDatabaseFixture _testDatabase;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _testDatabase = new TestDatabaseFixture();
        await _testDatabase.SetupAsync();
        
        _app = new MTMTestApplication(_testDatabase.ConnectionString);
        await _app.InitializeAsync();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _app.ShutdownAsync();
        await _testDatabase.TearDownAsync();
    }
    
    [Test]
    [Order(1)]
    public async Task Journey_01_Application_Startup_And_Initialization()
    {
        // Verify application starts successfully
        Assert.IsTrue(_app.IsRunning, "Application should be running");
        
        // Verify database connection
        var connectionStatus = await _app.CheckDatabaseConnectionAsync();
        Assert.IsTrue(connectionStatus.IsConnected, "Database should be connected");
        
        // Verify all services are initialized
        var healthCheck = await _app.GetHealthStatusAsync();
        Assert.IsTrue(healthCheck.AllServicesHealthy, "All services should be healthy");
        
        // Verify main window is displayed
        var mainWindow = _app.GetMainWindow();
        Assert.IsNotNull(mainWindow, "Main window should be displayed");
        Assert.IsTrue(mainWindow.IsVisible, "Main window should be visible");
    }
    
    [Test]
    [Order(2)]
    public async Task Journey_02_Load_Master_Data()
    {
        var mainWindow = _app.GetMainWindow();
        var inventoryView = mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Navigate to Inventory tab to trigger master data loading
        var inventoryTabButton = mainWindow.GetTabButton("Inventory");
        inventoryTabButton.Click();
        
        // Wait for master data to load
        await _app.WaitForConditionAsync(
            () => inventoryView.IsMasterDataLoaded, 
            timeout: TimeSpan.FromSeconds(10));
        
        // Verify master data is available
        var partSuggestions = inventoryView.GetPartSuggestions();
        Assert.IsTrue(partSuggestions.Count > 0, "Part suggestions should be loaded");
        
        var operationSuggestions = inventoryView.GetOperationSuggestions();
        Assert.IsTrue(operationSuggestions.Count > 0, "Operation suggestions should be loaded");
        
        var locationSuggestions = inventoryView.GetLocationSuggestions();
        Assert.IsTrue(locationSuggestions.Count > 0, "Location suggestions should be loaded");
    }
    
    [Test]
    [Order(3)]
    public async Task Journey_03_Perform_Typical_Shift_Operations()
    {
        var mainWindow = _app.GetMainWindow();
        var inventoryView = mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Simulate a typical 8-hour shift with various operations
        var shiftOperations = new[]
        {
            new { PartId = "SHIFT_PART_001", Operation = "90", Quantity = 50, Location = "STATION_1" },
            new { PartId = "SHIFT_PART_002", Operation = "100", Quantity = 30, Location = "STATION_2" },
            new { PartId = "SHIFT_PART_003", Operation = "110", Quantity = 25, Location = "STATION_3" },
            new { PartId = "SHIFT_PART_001", Operation = "100", Quantity = 45, Location = "STATION_1" }, // Same part, different operation
            new { PartId = "SHIFT_PART_004", Operation = "90", Quantity = 60, Location = "STATION_4" },
        };
        
        foreach (var operation in shiftOperations)
        {
            // Perform inventory add operation
            await PerformInventoryOperation(inventoryView, operation.PartId, 
                operation.Operation, operation.Quantity, operation.Location);
                
            // Verify success
            var successOverlay = await mainWindow.WaitForElementAsync<SuccessOverlay>();
            Assert.IsNotNull(successOverlay);
            
            // Wait for operation to complete and UI to update
            await Task.Delay(1000);
        }
        
        // Verify all operations were recorded
        var quickButtonsView = mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        var recentTransactions = quickButtonsView.GetQuickButtons();
        
        Assert.IsTrue(recentTransactions.Count >= shiftOperations.Length, 
            "All shift operations should be reflected in QuickButtons");
    }
    
    [Test]
    [Order(4)]
    public async Task Journey_04_Use_QuickButtons_For_Repeated_Operations()
    {
        var mainWindow = _app.GetMainWindow();
        var quickButtonsView = mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        var inventoryView = mainWindow.Get<InventoryTabView>("InventoryTabView");
        
        // Get available QuickButtons
        var quickButtons = quickButtonsView.GetQuickButtons();
        Assert.IsTrue(quickButtons.Count > 0, "QuickButtons should be available from previous operations");
        
        // Use first QuickButton multiple times with different quantities
        var selectedButton = quickButtons.First();
        var quantities = new[] { 10, 15, 20, 12, 8 };
        
        foreach (var quantity in quantities)
        {
            // Click QuickButton to populate form
            selectedButton.Click();
            
            // Navigate to Inventory tab
            var inventoryTabButton = mainWindow.GetTabButton("Inventory");
            inventoryTabButton.Click();
            
            // Modify quantity and save
            var quantityTextBox = inventoryView.Get<TextBox>("QuantityTextBox");
            await quantityTextBox.SetTextAsync(quantity.ToString());
            
            var saveButton = inventoryView.Get<Button>("SaveButton");
            saveButton.Click();
            
            // Verify success
            await mainWindow.WaitForElementAsync<SuccessOverlay>();
            await Task.Delay(1000);
        }
        
        // Verify transaction history reflects the repeated operations
        var sessionHistory = quickButtonsView.GetSessionHistory();
        var repeatedPartOperations = sessionHistory.Count(h => 
            h.PartId == selectedButton.PartId && 
            h.Operation == selectedButton.Operation);
            
        Assert.AreEqual(quantities.Length, repeatedPartOperations,
            $"Session should show {quantities.Length} operations for the repeated part");
    }
    
    [Test]
    [Order(5)]
    public async Task Journey_05_Perform_Remove_Operations()
    {
        var mainWindow = _app.GetMainWindow();
        
        // Navigate to Remove tab
        var removeTabButton = mainWindow.GetTabButton("Remove");
        removeTabButton.Click();
        
        var removeView = mainWindow.Get<RemoveTabView>("RemoveTabView");
        
        // Perform several removal operations
        var removeOperations = new[]
        {
            new { PartId = "SHIFT_PART_001", Operation = "90", Quantity = 10, Location = "STATION_1" },
            new { PartId = "SHIFT_PART_002", Operation = "100", Quantity = 5, Location = "STATION_2" },
        };
        
        foreach (var operation in removeOperations)
        {
            await PerformRemoveOperation(removeView, operation.PartId, 
                operation.Operation, operation.Quantity, operation.Location);
                
            var successOverlay = await mainWindow.WaitForElementAsync<SuccessOverlay>();
            Assert.IsNotNull(successOverlay);
            await Task.Delay(1000);
        }
        
        // Verify removals are reflected in transaction history
        var quickButtonsView = mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        quickButtonsView.ShowHistory(); // Switch to history view
        
        var history = quickButtonsView.GetSessionHistory();
        var removeTransactions = history.Where(h => h.TransactionType == "OUT").ToList();
        
        Assert.AreEqual(removeOperations.Length, removeTransactions.Count,
            "Remove operations should be recorded in transaction history");
    }
    
    [Test]
    [Order(6)]
    public async Task Journey_06_View_Reports_And_Transaction_History()
    {
        var mainWindow = _app.GetMainWindow();
        
        // Test QuickButtons history view
        var quickButtonsView = mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        quickButtonsView.ShowHistory();
        
        var sessionHistory = quickButtonsView.GetSessionHistory();
        Assert.IsTrue(sessionHistory.Count > 0, "Session history should contain transactions");
        
        // Verify different transaction types are present
        var hasInTransactions = sessionHistory.Any(h => h.TransactionType == "IN");
        var hasOutTransactions = sessionHistory.Any(h => h.TransactionType == "OUT");
        
        Assert.IsTrue(hasInTransactions, "Session should contain IN transactions");
        Assert.IsTrue(hasOutTransactions, "Session should contain OUT transactions");
        
        // Test print functionality
        var printButton = quickButtonsView.Get<Button>("PrintButton");
        if (printButton.IsVisible)
        {
            printButton.Click();
            
            var printView = await mainWindow.WaitForElementAsync<PrintView>();
            Assert.IsNotNull(printView, "Print view should open for transaction history");
        }
    }
    
    [Test]
    [Order(7)]
    public async Task Journey_07_Validate_Data_Integrity()
    {
        // Validate that all operations performed during the journey are 
        // correctly stored in the database and accessible
        
        var totalTransactions = await _testDatabase.GetTransactionCountAsync();
        Assert.IsTrue(totalTransactions > 0, "Database should contain transaction records");
        
        var partCounts = await _testDatabase.GetPartInventoryCountsAsync();
        Assert.IsTrue(partCounts.Count > 0, "Database should contain inventory records");
        
        // Verify data consistency between UI and database
        var mainWindow = _app.GetMainWindow();
        var quickButtonsView = mainWindow.Get<QuickButtonsView>("QuickButtonsView");
        var uiTransactionCount = quickButtonsView.GetSessionHistory().Count;
        
        // UI session history should match database records for this session
        var sessionTransactions = await _testDatabase.GetSessionTransactionsAsync();
        Assert.AreEqual(sessionTransactions.Count, uiTransactionCount,
            "UI session history should match database session records");
    }
    
    [Test]
    [Order(8)]
    public async Task Journey_08_Graceful_Shutdown()
    {
        // Test application shutdown sequence
        var shutdownTask = _app.ShutdownGracefullyAsync();
        
        // Should complete within reasonable time
        var completed = await Task.WhenAny(shutdownTask, Task.Delay(TimeSpan.FromSeconds(30)));
        Assert.AreEqual(shutdownTask, completed, "Application should shutdown gracefully within 30 seconds");
        
        // Verify application is no longer running
        Assert.IsFalse(_app.IsRunning, "Application should not be running after shutdown");
        
        // Verify no critical errors during shutdown
        var shutdownLogs = await _testDatabase.GetApplicationLogsAsync(LogLevel.Error);
        var criticalShutdownErrors = shutdownLogs.Where(log => 
            log.Timestamp > DateTime.Now.AddMinutes(-1) && 
            log.Message.Contains("shutdown", StringComparison.OrdinalIgnoreCase));
            
        Assert.AreEqual(0, criticalShutdownErrors.Count(), 
            "No critical errors should occur during graceful shutdown");
    }
    
    // Helper methods
    private async Task PerformInventoryOperation(InventoryTabView inventoryView, 
        string partId, string operation, int quantity, string location)
    {
        var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
        await partTextBox.SetTextAsync(partId);
        
        var operationTextBox = inventoryView.Get<TextBox>("OperationTextBox");
        await operationTextBox.SetTextAsync(operation);
        
        var quantityTextBox = inventoryView.Get<TextBox>("QuantityTextBox");
        await quantityTextBox.SetTextAsync(quantity.ToString());
        
        var locationTextBox = inventoryView.Get<TextBox>("LocationTextBox");
        await locationTextBox.SetTextAsync(location);
        
        var saveButton = inventoryView.Get<Button>("SaveButton");
        saveButton.Click();
    }
    
    private async Task PerformRemoveOperation(RemoveTabView removeView, 
        string partId, string operation, int quantity, string location)
    {
        var partTextBox = removeView.Get<TextBox>("PartTextBox");
        await partTextBox.SetTextAsync(partId);
        
        var operationTextBox = removeView.Get<TextBox>("OperationTextBox");
        await operationTextBox.SetTextAsync(operation);
        
        var quantityTextBox = removeView.Get<TextBox>("QuantityTextBox");
        await quantityTextBox.SetTextAsync(quantity.ToString());
        
        var locationTextBox = removeView.Get<TextBox>("LocationTextBox");
        await locationTextBox.SetTextAsync(location);
        
        var removeButton = removeView.Get<Button>("RemoveButton");
        removeButton.Click();
    }
}
```

### **5. Performance and Load Testing**

#### **High-Volume Transaction Testing**
```csharp
// Tests/PerformanceTests/HighVolumeTransactionTests.cs
[TestFixture]
[Category("Performance")]
public class HighVolumeTransactionTests
{
    private MTMTestApplication _app;
    private string _connectionString;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _connectionString = GetPerformanceTestConnectionString();
        _app = new MTMTestApplication(_connectionString);
        await _app.InitializeAsync();
    }
    
    [Test]
    public async Task Should_Handle_100_Concurrent_Inventory_Operations()
    {
        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task<bool>>();
        var successCount = 0;
        var errorCount = 0;
        
        // Create 100 concurrent inventory operations
        for (int i = 0; i < 100; i++)
        {
            var partId = $"PERF_PART_{i:000}";
            var operation = (i % 3) switch
            {
                0 => "90",
                1 => "100", 
                _ => "110"
            };
            var quantity = 10 + (i % 50);
            var location = $"STATION_{(i % 5) + 1}";
            
            tasks.Add(PerformInventoryOperationAsync(partId, operation, quantity, location));
        }
        
        // Wait for all operations to complete
        var results = await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        
        // Analyze results
        successCount = results.Count(r => r);
        errorCount = results.Count(r => !r);
        
        // Performance assertions
        Assert.Less(stopwatch.ElapsedMilliseconds, 60000, // Should complete within 1 minute
            $"100 concurrent operations took {stopwatch.ElapsedMilliseconds}ms, expected < 60000ms");
        
        Assert.GreaterOrEqual(successCount, 95, // At least 95% success rate
            $"Success rate: {successCount}/100 operations, expected >= 95");
            
        Assert.LessOrEqual(errorCount, 5, // No more than 5% failures
            $"Error rate: {errorCount}/100 operations, expected <= 5");
            
        Console.WriteLine($"Performance Test Results:");
        Console.WriteLine($"  Total Time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"  Average per Operation: {stopwatch.ElapsedMilliseconds / 100.0:F2}ms");
        Console.WriteLine($"  Success Rate: {successCount}/100 ({successCount}%)");
        Console.WriteLine($"  Error Rate: {errorCount}/100 ({errorCount}%)");
    }
    
    [Test]
    public async Task Should_Maintain_UI_Responsiveness_During_Load()
    {
        var uiResponsivenessTasks = new List<Task>();
        var loadTestTask = SimulateHeavyDatabaseLoadAsync();
        
        // Continuously test UI responsiveness during load
        var uiTestTask = Task.Run(async () =>
        {
            for (int i = 0; i < 100; i++)
            {
                var responseTime = await MeasureUIResponseTimeAsync();
                Assert.Less(responseTime, 500, // UI should respond within 500ms
                    $"UI response time {responseTime}ms exceeded threshold during load test iteration {i}");
                    
                await Task.Delay(100); // Test every 100ms
            }
        });
        
        // Run both tasks concurrently
        await Task.WhenAll(loadTestTask, uiTestTask);
    }
    
    [Test]
    public async Task Should_Handle_Database_Connection_Recovery()
    {
        // Simulate database connection interruption and recovery
        var initialOperationResult = await PerformInventoryOperationAsync("CONN_TEST_001", "100", 10, "STATION_1");
        Assert.IsTrue(initialOperationResult, "Initial operation should succeed");
        
        // Simulate connection interruption (would need test database setup)
        await SimulateDatabaseConnectionIssue();
        
        // Attempt operation during connection issue
        var duringIssueResult = await PerformInventoryOperationAsync("CONN_TEST_002", "100", 10, "STATION_1");
        // This may fail, which is expected
        
        // Simulate connection recovery
        await SimulateDatabaseConnectionRecovery();
        
        // Wait for connection recovery
        await Task.Delay(5000);
        
        // Operation after recovery should succeed
        var afterRecoveryResult = await PerformInventoryOperationAsync("CONN_TEST_003", "100", 10, "STATION_1");
        Assert.IsTrue(afterRecoveryResult, "Operation should succeed after connection recovery");
    }
    
    private async Task<bool> PerformInventoryOperationAsync(string partId, string operation, int quantity, string location)
    {
        try
        {
            var parameters = new MySqlParameter[]
            {
                new("p_PartID", partId),
                new("p_OperationNumber", operation),
                new("p_Quantity", quantity),
                new("p_Location", location),
                new("p_User", "PerformanceTest")
            };
            
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                _connectionString,
                "inv_inventory_Add_Item",
                parameters
            );
            
            return result.Status == 1;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    private async Task<long> MeasureUIResponseTimeAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Simulate UI interaction
            var mainWindow = _app.GetMainWindow();
            var inventoryView = mainWindow.Get<InventoryTabView>("InventoryTabView");
            var partTextBox = inventoryView.Get<TextBox>("PartTextBox");
            
            // Measure response time for simple UI update
            await partTextBox.SetTextAsync($"UI_TEST_{DateTime.Now.Ticks}");
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
        catch
        {
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds; // Return the time even if failed
        }
    }
}
```

### **6. Cross-Platform Validation Testing**

#### **Platform-Specific Feature Testing**
```csharp
// Tests/CrossPlatformTests/PlatformSpecificFeatureTests.cs
[TestFixture]
[Category("CrossPlatform")]
public class PlatformSpecificFeatureTests
{
    [Test]
    [Platform("Win")]
    public async Task Windows_Should_Support_All_MTM_Features()
    {
        var features = await TestAllMTMFeaturesAsync();
        
        Assert.IsTrue(features.DatabaseOperations, "Database operations should work on Windows");
        Assert.IsTrue(features.FileOperations, "File operations should work on Windows");
        Assert.IsTrue(features.PrintingCapabilities, "Printing should work on Windows");
        Assert.IsTrue(features.UIRendering, "UI should render correctly on Windows");
        Assert.IsTrue(features.KeyboardNavigation, "Keyboard navigation should work on Windows");
        Assert.IsTrue(features.ThemeSupport, "Theme system should work on Windows");
    }
    
    [Test]
    [Platform("Mac")]
    public async Task MacOS_Should_Support_Core_MTM_Features()
    {
        var features = await TestAllMTMFeaturesAsync();
        
        Assert.IsTrue(features.DatabaseOperations, "Database operations should work on macOS");
        Assert.IsTrue(features.FileOperations, "File operations should work on macOS");
        Assert.IsTrue(features.UIRendering, "UI should render correctly on macOS");
        Assert.IsTrue(features.KeyboardNavigation, "Keyboard navigation should work on macOS");
        Assert.IsTrue(features.ThemeSupport, "Theme system should work on macOS");
        
        // Platform-specific validations
        Assert.IsTrue(features.MacOSSpecific.CocoaIntegration, "Cocoa integration should work");
        Assert.IsTrue(features.MacOSSpecific.FileSystemPermissions, "File system permissions should work");
    }
    
    [Test]
    [Platform("Linux")]
    public async Task Linux_Should_Support_Core_MTM_Features()
    {
        var features = await TestAllMTMFeaturesAsync();
        
        Assert.IsTrue(features.DatabaseOperations, "Database operations should work on Linux");
        Assert.IsTrue(features.FileOperations, "File operations should work on Linux");
        Assert.IsTrue(features.UIRendering, "UI should render correctly on Linux");
        Assert.IsTrue(features.ThemeSupport, "Theme system should work on Linux");
        
        // Linux-specific validations
        Assert.IsTrue(features.LinuxSpecific.GTKIntegration, "GTK integration should work");
        Assert.IsTrue(features.LinuxSpecific.FontRendering, "Font rendering should work");
    }
    
    [Test]
    [Platform("Android")]
    public async Task Android_Should_Support_Mobile_MTM_Features()
    {
        var features = await TestMobileMTMFeaturesAsync();
        
        Assert.IsTrue(features.DatabaseOperations, "Database operations should work on Android");
        Assert.IsTrue(features.FileOperations, "File operations should work on Android");
        Assert.IsTrue(features.TouchInterface, "Touch interface should work on Android");
        Assert.IsTrue(features.MobileUI, "Mobile UI should render correctly on Android");
        
        // Android-specific validations
        Assert.IsTrue(features.AndroidSpecific.StorageAccessFramework, "SAF should work");
        Assert.IsTrue(features.AndroidSpecific.RuntimePermissions, "Runtime permissions should work");
    }
    
    private async Task<MTMFeatureTestResults> TestAllMTMFeaturesAsync()
    {
        var results = new MTMFeatureTestResults();
        
        // Test database operations
        results.DatabaseOperations = await TestDatabaseOperations();
        
        // Test file operations
        results.FileOperations = await TestFileOperations();
        
        // Test UI rendering
        results.UIRendering = await TestUIRendering();
        
        // Test keyboard navigation
        results.KeyboardNavigation = await TestKeyboardNavigation();
        
        // Test theme support
        results.ThemeSupport = await TestThemeSupport();
        
        // Test printing (where supported)
        results.PrintingCapabilities = await TestPrintingCapabilities();
        
        // Platform-specific tests
        if (OperatingSystem.IsMacOS())
        {
            results.MacOSSpecific = await TestMacOSSpecificFeatures();
        }
        else if (OperatingSystem.IsLinux())
        {
            results.LinuxSpecific = await TestLinuxSpecificFeatures();
        }
        
        return results;
    }
    
    private async Task<bool> TestDatabaseOperations()
    {
        try
        {
            var connectionString = GetPlatformConnectionString();
            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                connectionString,
                "md_part_ids_Get_All",
                Array.Empty<MySqlParameter>()
            );
            
            return result.Status == 1;
        }
        catch
        {
            return false;
        }
    }
    
    private async Task<bool> TestFileOperations()
    {
        try
        {
            var fileService = Program.GetService<IFileSelectionService>();
            var options = new FileSelectionOptions
            {
                Title = "Platform Test",
                Extensions = new[] { "*.json" },
                Mode = FileSelectionMode.Export
            };
            
            // Test file selection service initialization
            var validationResult = await fileService.ValidateFileAccessAsync("./test-file.txt");
            
            return true; // Service should initialize without errors
        }
        catch
        {
            return false;
        }
    }
}
```

## 🔄 Testing Implementation Plan

### **Phase 1: Core Infrastructure (Week 1-2)**
1. **Set up test project structure** with proper categorization
2. **Create test database fixtures** for isolated testing
3. **Implement basic test utilities** and helpers
4. **Set up CI/CD pipeline** with multi-platform testing

### **Phase 2: Unit and Integration Tests (Week 3-4)**  
1. **Complete unit tests** for all ViewModels and Services
2. **Implement integration tests** for service interactions
3. **Database integration tests** with real stored procedures
4. **Cross-service communication testing**

### **Phase 3: UI Automation (Week 5-6)**
1. **Implement Avalonia UI test framework integration**
2. **Create UI automation tests** for major workflows
3. **Keyboard navigation and accessibility testing**
4. **Cross-platform UI rendering validation**

### **Phase 4: End-to-End Testing (Week 7-8)**
1. **Complete user journey tests** for all operator workflows
2. **Performance and load testing** implementation
3. **Error recovery and resilience testing**
4. **Cross-platform feature validation**

### **Phase 5: Continuous Testing (Ongoing)**
1. **Automated test execution** on all commits/PRs
2. **Performance monitoring** and regression detection
3. **Cross-platform compatibility** validation
4. **Test maintenance** and expansion as features grow

---

## 🎯 Expected Benefits

### **Quality Assurance**
- **99%+ Test Coverage** across all application components
- **Zero Regression** guarantee for existing functionality
- **Cross-Platform Compatibility** validated automatically
- **Performance Standards** maintained consistently

### **Development Velocity**
- **Faster Feature Development** with confidence in testing
- **Reduced Bug Reports** from production users
- **Easier Maintenance** with comprehensive test suite
- **Automated Validation** of all code changes

### **Manufacturing Reliability**
- **Production-Ready Quality** with extensive validation
- **Operator Confidence** in application reliability
- **Minimal Downtime** due to thorough testing
- **Consistent Performance** across all deployment platforms

This comprehensive testing strategy ensures your MTM WIP Application is thoroughly validated across all platforms, user workflows, and performance scenarios. The testing infrastructure supports the full manufacturing environment requirements while maintaining development agility.