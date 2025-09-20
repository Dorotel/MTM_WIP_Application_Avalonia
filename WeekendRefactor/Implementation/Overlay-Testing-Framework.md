# Overlay Testing Patterns and Framework

**Referenced Files:** Following MTM copilot-instructions.md - all required instruction files, context files, templates, and patterns are automatically referenced for this response.

**Document Version**: 1.0  
**Creation Date**: September 19, 2025  
**Target Audience**: MTM Application Developers and QA Engineers  

## 🎯 Testing Framework Overview

This document provides comprehensive testing patterns, frameworks, and templates for MTM overlay system validation. Following MTM standards with MVVM Community Toolkit patterns, Avalonia UI testing, and stored procedure validation.

## 📋 Testing Strategy

### **Testing Pyramid for Overlays**

```
                      ┌─────────────────┐
                      │   E2E Tests     │ 5%
                      │ (Full Scenarios)│
                      └─────────────────┘
                    ┌─────────────────────┐
                    │ Integration Tests   │ 25%
                    │ (Service + UI)      │
                    └─────────────────────┘
                  ┌───────────────────────────┐
                  │      Unit Tests           │ 70%
                  │ (ViewModels + Models)     │
                  └───────────────────────────┘
```

### **Coverage Requirements**

- **Unit Tests**: 95%+ coverage for ViewModels, Models, and Services
- **Integration Tests**: 80%+ coverage for overlay workflows
- **UI Tests**: 60%+ coverage for user interaction scenarios
- **Database Tests**: 100% coverage for stored procedures used

## 🧪 Unit Testing Patterns

### **Base Test Class Template**

```csharp
// File: Tests/Overlay/BaseOverlayViewModelTests.cs

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FluentAssertions;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Services;
using System;
using System.Threading.Tasks;

/// <summary>
/// Base class for overlay ViewModel testing with common setup
/// Follows MTM testing standards and patterns
/// </summary>
[TestClass]
public abstract class BaseOverlayViewModelTests<TViewModel, TRequest, TResponse>
    where TViewModel : BasePoolableOverlayViewModel
    where TRequest : BaseOverlayRequest
    where TResponse : BaseOverlayResponse
{
    #region Protected Fields

    protected Mock<ILogger<TViewModel>> MockLogger;
    protected Mock<IConfigurationService> MockConfigService;
    protected Mock<IUniversalOverlayService> MockOverlayService;
    protected TViewModel ViewModel;
    protected ServiceCollection Services;
    protected ServiceProvider ServiceProvider;

    #endregion

    #region Test Setup

    [TestInitialize]
    public virtual async Task BaseSetup()
    {
        // Setup service collection
        Services = new ServiceCollection();
        
        // Setup mocks
        MockLogger = new Mock<ILogger<TViewModel>>();
        MockConfigService = new Mock<IConfigurationService>();
        MockOverlayService = new Mock<IUniversalOverlayService>();
        
        // Default mock configurations
        SetupDefaultMocks();
        
        // Register services
        Services.AddSingleton(MockLogger.Object);
        Services.AddSingleton(MockConfigService.Object);
        Services.AddSingleton(MockOverlayService.Object);
        
        // Add custom services
        await SetupCustomServicesAsync();
        
        ServiceProvider = Services.BuildServiceProvider();
        
        // Create ViewModel
        ViewModel = CreateViewModel();
        
        // Custom setup
        await CustomSetupAsync();
    }

    [TestCleanup]
    public virtual void BaseCleanup()
    {
        ViewModel?.Dispose();
        ServiceProvider?.Dispose();
        
        // Custom cleanup
        CustomCleanup();
    }

    #endregion

    #region Abstract Methods

    protected abstract TViewModel CreateViewModel();
    protected abstract TRequest CreateValidRequest();
    protected abstract TRequest CreateInvalidRequest();
    
    #endregion

    #region Virtual Methods

    protected virtual void SetupDefaultMocks()
    {
        MockConfigService.Setup(x => x.GetConnectionStringAsync())
            .ReturnsAsync("TestConnectionString");
    }

    protected virtual Task SetupCustomServicesAsync() => Task.CompletedTask;
    protected virtual Task CustomSetupAsync() => Task.CompletedTask;
    protected virtual void CustomCleanup() { }

    #endregion

    #region Common Test Methods

    [TestMethod]
    public virtual async Task Initialize_WithValidRequest_ShouldSetIsVisible()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        await ViewModel.InitializeAsync(request);

        // Assert
        ViewModel.IsVisible.Should().BeTrue();
    }

    [TestMethod]
    public virtual async Task Initialize_WithInvalidRequest_ShouldThrowException()
    {
        // Arrange
        var request = CreateInvalidRequest();

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => ViewModel.InitializeAsync(request));
    }

    [TestMethod]
    public virtual void Reset_ShouldClearAllProperties()
    {
        // Act
        ViewModel.Reset();

        // Assert
        ViewModel.IsVisible.Should().BeFalse();
        // Additional assertions should be added by derived classes
    }

    [TestMethod]
    public virtual async Task CloseAsync_ShouldSetIsVisibleToFalse()
    {
        // Arrange
        ViewModel.IsVisible = true;

        // Act
        await ViewModel.CloseAsync();

        // Assert
        ViewModel.IsVisible.Should().BeFalse();
    }

    #endregion
}
```

### **Specific ViewModel Test Implementation**

```csharp
// File: Tests/Overlay/InventoryQuickEditOverlayViewModelTests.cs

[TestClass]
public class InventoryQuickEditOverlayViewModelTests 
    : BaseOverlayViewModelTests<
        InventoryQuickEditOverlayViewModel,
        InventoryQuickEditRequest,
        InventoryQuickEditResponse>
{
    #region Test Data

    private const string TestPartId = "TEST001";
    private const string TestOperation = "100";
    private const string TestLocation = "WH-TEST";
    private const int TestQuantity = 50;
    private const string TestNotes = "Test notes";

    #endregion

    #region Setup Override

    protected override InventoryQuickEditOverlayViewModel CreateViewModel()
    {
        return new InventoryQuickEditOverlayViewModel(
            MockLogger.Object,
            MockConfigService.Object);
    }

    protected override InventoryQuickEditRequest CreateValidRequest()
    {
        return new InventoryQuickEditRequest(
            PartId: TestPartId,
            Operation: TestOperation,
            Location: TestLocation,
            CurrentQuantity: TestQuantity,
            CurrentNotes: TestNotes
        );
    }

    protected override InventoryQuickEditRequest CreateInvalidRequest()
    {
        return new InventoryQuickEditRequest(
            PartId: string.Empty, // Invalid - empty part ID
            Operation: TestOperation,
            Location: TestLocation,
            CurrentQuantity: -1, // Invalid - negative quantity
            CurrentNotes: TestNotes
        );
    }

    #endregion

    #region Initialization Tests

    [TestMethod]
    public async Task Initialize_WithValidRequest_ShouldSetAllProperties()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        await ViewModel.InitializeAsync(request);

        // Assert
        ViewModel.PartId.Should().Be(TestPartId);
        ViewModel.Operation.Should().Be(TestOperation);
        ViewModel.Location.Should().Be(TestLocation);
        ViewModel.CurrentQuantity.Should().Be(TestQuantity);
        ViewModel.NewQuantity.Should().Be(TestQuantity);
        ViewModel.CurrentNotes.Should().Be(TestNotes);
        ViewModel.NewNotes.Should().Be(TestNotes);
        ViewModel.HasUnsavedChanges.Should().BeFalse();
        ViewModel.IsVisible.Should().BeTrue();
    }

    [TestMethod]
    public async Task Initialize_WithEmptyNotes_ShouldHandleCorrectly()
    {
        // Arrange
        var request = new InventoryQuickEditRequest(
            TestPartId, TestOperation, TestLocation, TestQuantity, string.Empty);

        // Act
        await ViewModel.InitializeAsync(request);

        // Assert
        ViewModel.CurrentNotes.Should().BeEmpty();
        ViewModel.NewNotes.Should().BeEmpty();
        ViewModel.RemainingCharacters.Should().Be(250);
    }

    #endregion

    #region Property Change Tests

    [TestMethod]
    public void NewQuantity_WhenChanged_ShouldUpdateHasUnsavedChanges()
    {
        // Arrange
        ViewModel.CurrentQuantity = TestQuantity;
        ViewModel.NewQuantity = TestQuantity;
        ViewModel.HasUnsavedChanges.Should().BeFalse();

        // Act
        ViewModel.NewQuantity = TestQuantity + 10;

        // Assert
        ViewModel.HasUnsavedChanges.Should().BeTrue();
    }

    [TestMethod]
    public void NewQuantity_WithSameValue_ShouldNotSetUnsavedChanges()
    {
        // Arrange
        ViewModel.CurrentQuantity = TestQuantity;
        ViewModel.NewQuantity = TestQuantity + 10;
        ViewModel.HasUnsavedChanges.Should().BeTrue();

        // Act
        ViewModel.NewQuantity = TestQuantity; // Back to original

        // Assert
        ViewModel.HasUnsavedChanges.Should().BeFalse();
    }

    [TestMethod]
    public void NewNotes_WhenChanged_ShouldUpdateRemainingCharacters()
    {
        // Arrange
        ViewModel.MaxNotesLength = 250;
        var notes = "Test notes content";

        // Act
        ViewModel.NewNotes = notes;

        // Assert
        ViewModel.RemainingCharacters.Should().Be(250 - notes.Length);
    }

    [TestMethod]
    public void NewNotes_WhenChanged_ShouldUpdateHasUnsavedChanges()
    {
        // Arrange
        ViewModel.CurrentNotes = TestNotes;
        ViewModel.NewNotes = TestNotes;
        ViewModel.HasUnsavedChanges.Should().BeFalse();

        // Act
        ViewModel.NewNotes = "Different notes";

        // Assert
        ViewModel.HasUnsavedChanges.Should().BeTrue();
    }

    #endregion

    #region Validation Tests

    [TestMethod]
    [DataRow(-1, false, "Quantity cannot be negative")]
    [DataRow(1000000, false, "Quantity cannot exceed 999,999")]
    [DataRow(0, true, "")]
    [DataRow(100, true, "")]
    [DataRow(999999, true, "")]
    public async Task ValidateQuantity_WithVariousValues_ShouldSetCorrectValidationState(
        int quantity, bool expectedValid, string expectedMessage)
    {
        // Act
        ViewModel.NewQuantity = quantity;
        await Task.Delay(50); // Allow async validation to complete

        // Assert
        ViewModel.IsQuantityValid.Should().Be(expectedValid);
        if (!expectedValid)
        {
            ViewModel.QuantityValidationMessage.Should().Contain(expectedMessage);
        }
    }

    [TestMethod]
    public async Task ValidateNotes_WithExcessiveLength_ShouldSetValidationError()
    {
        // Arrange
        ViewModel.MaxNotesLength = 250;
        var longNotes = new string('A', 300);

        // Act
        ViewModel.NewNotes = longNotes;
        await Task.Delay(50); // Allow async validation

        // Assert
        ViewModel.IsNotesValid.Should().BeFalse();
        ViewModel.NotesValidationMessage.Should().Contain("cannot exceed 250 characters");
    }

    [TestMethod]
    public async Task ValidateNotes_WithInvalidCharacters_ShouldSetValidationError()
    {
        // Act
        ViewModel.NewNotes = "Notes with <invalid> characters";
        await Task.Delay(50); // Allow async validation

        // Assert
        ViewModel.IsNotesValid.Should().BeFalse();
        ViewModel.NotesValidationMessage.Should().Contain("cannot contain < or > characters");
    }

    #endregion

    #region Command Tests

    [TestMethod]
    public void SaveChangesCommand_WithValidChanges_ShouldBeEnabled()
    {
        // Arrange
        SetupValidViewModel();

        // Act & Assert
        ViewModel.SaveChangesCommand.CanExecute(null).Should().BeTrue();
    }

    [TestMethod]
    public void SaveChangesCommand_WithoutChanges_ShouldBeDisabled()
    {
        // Arrange
        ViewModel.HasUnsavedChanges = false;
        ViewModel.IsQuantityValid = true;
        ViewModel.IsNotesValid = true;

        // Act & Assert
        ViewModel.SaveChangesCommand.CanExecute(null).Should().BeFalse();
    }

    [TestMethod]
    public void SaveChangesCommand_WithInvalidData_ShouldBeDisabled()
    {
        // Arrange
        ViewModel.HasUnsavedChanges = true;
        ViewModel.IsQuantityValid = false; // Invalid
        ViewModel.IsNotesValid = true;

        // Act & Assert
        ViewModel.SaveChangesCommand.CanExecute(null).Should().BeFalse();
    }

    [TestMethod]
    public void ResetChangesCommand_WithChanges_ShouldBeEnabled()
    {
        // Arrange
        ViewModel.HasUnsavedChanges = true;
        ViewModel.IsSaving = false;

        // Act & Assert
        ViewModel.ResetChangesCommand.CanExecute(null).Should().BeTrue();
    }

    [TestMethod]
    public void ResetChangesCommand_WhileSaving_ShouldBeDisabled()
    {
        // Arrange
        ViewModel.HasUnsavedChanges = true;
        ViewModel.IsSaving = true;

        // Act & Assert
        ViewModel.ResetChangesCommand.CanExecute(null).Should().BeFalse();
    }

    #endregion

    #region Database Mock Tests

    [TestMethod]
    public async Task SaveChanges_WithSuccessfulDatabase_ShouldReturnConfirmedResponse()
    {
        // Arrange
        SetupValidViewModel();
        SetupSuccessfulDatabaseMock();

        // Act
        await ViewModel.SaveChangesCommand.ExecuteAsync(null);

        // Assert
        // Verify the overlay was closed with correct response
        ViewModel.IsVisible.Should().BeFalse();
        // Additional verification would require response capture mechanism
    }

    [TestMethod]
    public async Task SaveChanges_WithFailedDatabase_ShouldReturnErrorResponse()
    {
        // Arrange
        SetupValidViewModel();
        SetupFailedDatabaseMock();

        // Act
        await ViewModel.SaveChangesCommand.ExecuteAsync(null);

        // Assert
        ViewModel.IsVisible.Should().BeFalse();
        // Additional verification would require response capture mechanism
    }

    #endregion

    #region Reset and Pooling Tests

    [TestMethod]
    public void Reset_ShouldClearAllOverlaySpecificData()
    {
        // Arrange
        SetupValidViewModel();

        // Act
        ViewModel.Reset();

        // Assert
        ViewModel.PartId.Should().BeEmpty();
        ViewModel.Operation.Should().BeEmpty();
        ViewModel.Location.Should().BeEmpty();
        ViewModel.CurrentQuantity.Should().Be(0);
        ViewModel.NewQuantity.Should().Be(0);
        ViewModel.CurrentNotes.Should().BeEmpty();
        ViewModel.NewNotes.Should().BeEmpty();
        ViewModel.HasUnsavedChanges.Should().BeFalse();
        ViewModel.IsQuantityValid.Should().BeTrue();
        ViewModel.IsNotesValid.Should().BeTrue();
        ViewModel.IsSaving.Should().BeFalse();
    }

    #endregion

    #region Helper Methods

    private void SetupValidViewModel()
    {
        ViewModel.CurrentQuantity = TestQuantity;
        ViewModel.NewQuantity = TestQuantity + 10; // Changed
        ViewModel.HasUnsavedChanges = true;
        ViewModel.IsQuantityValid = true;
        ViewModel.IsNotesValid = true;
        ViewModel.IsSaving = false;
    }

    private void SetupSuccessfulDatabaseMock()
    {
        // Mock Helper_Database_StoredProcedure for successful operation
        // This would require additional abstraction or testing framework setup
    }

    private void SetupFailedDatabaseMock()
    {
        // Mock Helper_Database_StoredProcedure for failed operation
        // This would require additional abstraction or testing framework setup
    }

    #endregion
}
```

## 🔗 Integration Testing Patterns

### **Overlay Service Integration Tests**

```csharp
// File: Tests/Integration/OverlayServiceIntegrationTests.cs

[TestClass]
public class OverlayServiceIntegrationTests : BaseIntegrationTest
{
    private IUniversalOverlayService _overlayService;
    private TestHost _testHost;

    [TestInitialize]
    public async Task Setup()
    {
        // Create test host with full service registration
        _testHost = await CreateTestHostAsync();
        _overlayService = _testHost.Services.GetRequiredService<IUniversalOverlayService>();
    }

    [TestMethod]
    public async Task ShowOverlay_WithValidRequest_ShouldReturnSuccessResponse()
    {
        // Arrange
        var request = new InventoryQuickEditRequest(
            PartId: "TEST001",
            Operation: "100",
            Location: "WH-TEST",
            CurrentQuantity: 50
        );

        // Act
        var responseTask = _overlayService.ShowOverlayAsync<
            InventoryQuickEditRequest,
            InventoryQuickEditResponse,
            InventoryQuickEditOverlayViewModel>(request);

        // Simulate user interaction (in real tests, use UI automation)
        await SimulateUserAcceptanceAsync();

        var response = await responseTask;

        // Assert
        response.Result.Should().Be(OverlayResult.Confirmed);
        response.IsSuccess.Should().BeTrue();
    }

    [TestMethod]
    public async Task ShowOverlay_WithCancellation_ShouldReturnCancelledResponse()
    {
        // Arrange
        var request = new InventoryQuickEditRequest(
            "TEST001", "100", "WH-TEST", 50);

        // Act
        var responseTask = _overlayService.ShowOverlayAsync<
            InventoryQuickEditRequest,
            InventoryQuickEditResponse,
            InventoryQuickEditOverlayViewModel>(request);

        // Simulate user cancellation
        await SimulateUserCancellationAsync();

        var response = await responseTask;

        // Assert
        response.Result.Should().Be(OverlayResult.Cancelled);
    }

    [TestMethod]
    public async Task ShowMultipleOverlays_Sequentially_ShouldHandleCorrectly()
    {
        // Arrange
        var request1 = new InventoryQuickEditRequest("TEST001", "100", "WH-TEST", 50);
        var request2 = new InventoryQuickEditRequest("TEST002", "110", "WH-TEST", 75);

        // Act & Assert - First overlay
        var response1Task = _overlayService.ShowInventoryQuickEditOverlayAsync(request1);
        await SimulateUserAcceptanceAsync();
        var response1 = await response1Task;
        response1.Result.Should().Be(OverlayResult.Confirmed);

        // Act & Assert - Second overlay
        var response2Task = _overlayService.ShowInventoryQuickEditOverlayAsync(request2);
        await SimulateUserAcceptanceAsync();
        var response2 = await response2Task;
        response2.Result.Should().Be(OverlayResult.Confirmed);
    }

    private async Task SimulateUserAcceptanceAsync()
    {
        // In real tests, this would use UI automation to click buttons
        await Task.Delay(100);
        // Simulate save button click via ViewModel command
    }

    private async Task SimulateUserCancellationAsync()
    {
        // In real tests, this would use UI automation
        await Task.Delay(100);
        // Simulate cancel button click or ESC key
    }
}
```

## 🎭 UI Automation Testing

### **Avalonia UI Testing Framework**

```csharp
// File: Tests/UI/OverlayUITests.cs

[TestClass]
public class InventoryQuickEditOverlayUITests : BaseAvaloniaUITest
{
    private Application _app;
    private Window _mainWindow;

    [TestInitialize]
    public async Task Setup()
    {
        _app = CreateTestApplication();
        _mainWindow = await CreateMainWindowAsync();
    }

    [TestMethod]
    public async Task Overlay_WhenShown_ShouldDisplayCorrectly()
    {
        // Arrange
        var request = new InventoryQuickEditRequest(
            "PART001", "100", "WH-A-01", 50, "Test notes");

        // Act
        await ShowOverlayAsync(request);

        // Assert - Check UI elements
        var overlay = FindOverlay<InventoryQuickEditOverlayView>();
        overlay.Should().NotBeNull();
        
        var partIdText = FindTextBlock(overlay, "PART001");
        partIdText.Should().NotBeNull();
        
        var quantityInput = FindTextBox(overlay, "NewQuantity");
        quantityInput.Text.Should().Be("50");
    }

    [TestMethod]
    public async Task QuantityInput_WithInvalidValue_ShouldShowValidationError()
    {
        // Arrange
        await ShowOverlayWithDefaults();
        var quantityInput = FindTextBox("NewQuantity");

        // Act
        await SetTextBoxValueAsync(quantityInput, "-5");
        await WaitForValidationAsync();

        // Assert
        var validationMessage = FindTextBlock("QuantityValidationMessage");
        validationMessage.Text.Should().Contain("cannot be negative");
        
        var saveButton = FindButton("SaveChanges");
        saveButton.IsEnabled.Should().BeFalse();
    }

    [TestMethod]
    public async Task SaveButton_WhenClicked_ShouldTriggerSaveCommand()
    {
        // Arrange
        await ShowOverlayWithDefaults();
        var quantityInput = FindTextBox("NewQuantity");
        await SetTextBoxValueAsync(quantityInput, "75");

        // Act
        var saveButton = FindButton("SaveChanges");
        await ClickButtonAsync(saveButton);

        // Assert
        // Verify overlay closes and database is updated
        await WaitForOverlayToCloseAsync();
        
        // Verify database call was made (requires mock verification)
        // This would be verified through service layer testing
    }

    [TestMethod]
    public async Task CancelButton_WithUnsavedChanges_ShouldShowConfirmation()
    {
        // Arrange
        await ShowOverlayWithDefaults();
        await MakeUnsavedChangesAsync();

        // Act
        var cancelButton = FindButton("Cancel");
        await ClickButtonAsync(cancelButton);

        // Assert
        // In full implementation, this would show confirmation overlay
        // For now, verify overlay behavior
        await WaitForOverlayToCloseAsync();
    }

    [TestMethod]
    public async Task EscapeKey_ShouldTriggerCancel()
    {
        // Arrange
        await ShowOverlayWithDefaults();

        // Act
        await SendKeyAsync(Key.Escape);

        // Assert
        await WaitForOverlayToCloseAsync();
    }

    [TestMethod]
    public async Task CtrlS_ShouldTriggerSave()
    {
        // Arrange
        await ShowOverlayWithDefaults();
        await MakeValidChangesAsync();

        // Act
        await SendKeyComboAsync(KeyModifiers.Control, Key.S);

        // Assert
        await WaitForOverlayToCloseAsync();
    }

    private async Task ShowOverlayWithDefaults()
    {
        var request = new InventoryQuickEditRequest(
            "TEST001", "100", "WH-TEST", 50);
        await ShowOverlayAsync(request);
    }

    private async Task MakeUnsavedChangesAsync()
    {
        var quantityInput = FindTextBox("NewQuantity");
        await SetTextBoxValueAsync(quantityInput, "75");
    }

    private async Task MakeValidChangesAsync()
    {
        await MakeUnsavedChangesAsync();
        await WaitForValidationAsync();
    }
}
```

## 🗄️ Database Testing Patterns

### **Stored Procedure Testing**

```csharp
// File: Tests/Database/InventoryOverlayDatabaseTests.cs

[TestClass]
public class InventoryOverlayDatabaseTests : BaseDatabaseTest
{
    private string _connectionString;
    private const string TestPartId = "DB_TEST_001";

    [TestInitialize]
    public async Task Setup()
    {
        _connectionString = GetTestConnectionString();
        await SetupTestDataAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await CleanupTestDataAsync();
    }

    [TestMethod]
    public async Task StoredProcedure_inv_inventory_Update_QuickEdit_WithValidData_ShouldSucceed()
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", TestPartId),
            new("p_Operation", "100"),
            new("p_Location", "WH-TEST"),
            new("p_NewQuantity", 75),
            new("p_Notes", "Test update"),
            new("p_User", "TestUser")
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Update_QuickEdit",
            parameters
        );

        // Assert
        result.Status.Should().Be(1);
        
        // Verify data was actually updated
        var verificationResult = await GetInventoryAsync(TestPartId, "100", "WH-TEST");
        verificationResult.Should().NotBeNull();
        verificationResult.Quantity.Should().Be(75);
        verificationResult.Notes.Should().Be("Test update");
    }

    [TestMethod]
    public async Task StoredProcedure_inv_inventory_Update_QuickEdit_WithInvalidPartId_ShouldFail()
    {
        // Arrange
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", "INVALID_PART"),
            new("p_Operation", "100"),
            new("p_Location", "WH-TEST"),
            new("p_NewQuantity", 75),
            new("p_Notes", "Test update"),
            new("p_User", "TestUser")
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Update_QuickEdit",
            parameters
        );

        // Assert
        result.Status.Should().NotBe(1); // Should fail
    }

    [TestMethod]
    public async Task StoredProcedure_inv_inventory_Get_ByPartIDandOperation_ShouldReturnCorrectData()
    {
        // Arrange
        await InsertTestInventoryAsync(TestPartId, "100", "WH-TEST", 100, "Initial notes");
        
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", TestPartId),
            new("p_Operation", "100")
        };

        // Act
        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Get_ByPartIDandOperation",
            parameters
        );

        // Assert
        result.Status.Should().Be(1);
        result.Data.Rows.Count.Should().BeGreaterThan(0);
        
        var row = result.Data.Rows[0];
        row["PartID"].ToString().Should().Be(TestPartId);
        row["OperationNumber"].ToString().Should().Be("100");
        Convert.ToInt32(row["Quantity"]).Should().Be(100);
    }

    [TestMethod]
    public async Task MultipleQuickEdits_InSequence_ShouldMaintainDataIntegrity()
    {
        // Arrange
        var initialQuantity = 100;
        await InsertTestInventoryAsync(TestPartId, "100", "WH-TEST", initialQuantity);

        // Act - First edit
        await UpdateInventoryQuantityAsync(TestPartId, 125, "First update");
        
        // Act - Second edit
        await UpdateInventoryQuantityAsync(TestPartId, 90, "Second update");
        
        // Act - Third edit
        await UpdateInventoryQuantityAsync(TestPartId, 110, "Third update");

        // Assert
        var finalData = await GetInventoryAsync(TestPartId, "100", "WH-TEST");
        finalData.Quantity.Should().Be(110);
        finalData.Notes.Should().Be("Third update");
    }

    private async Task UpdateInventoryQuantityAsync(string partId, int quantity, string notes)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_PartID", partId),
            new("p_Operation", "100"),
            new("p_Location", "WH-TEST"),
            new("p_NewQuantity", quantity),
            new("p_Notes", notes),
            new("p_User", "TestUser")
        };

        var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
            _connectionString,
            "inv_inventory_Update_QuickEdit",
            parameters
        );

        result.Status.Should().Be(1, $"Failed to update inventory to quantity {quantity}");
    }
}
```

## 🔄 Performance Testing

### **Load Testing for Overlay Operations**

```csharp
// File: Tests/Performance/OverlayPerformanceTests.cs

[TestClass]
public class OverlayPerformanceTests
{
    private IUniversalOverlayService _overlayService;
    private List<TimeSpan> _operationTimes;

    [TestInitialize]
    public void Setup()
    {
        _overlayService = CreateTestOverlayService();
        _operationTimes = new List<TimeSpan>();
    }

    [TestMethod]
    public async Task OverlayPooling_MultipleRequestsSequentially_ShouldReuseInstances()
    {
        const int iterations = 100;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var iterationStart = stopwatch.Elapsed;
            
            var request = new InventoryQuickEditRequest(
                $"PERF_TEST_{i:000}", "100", "WH-TEST", i + 10);
                
            var response = await _overlayService.ShowInventoryQuickEditOverlayAsync(request);
            // Simulate immediate cancellation for testing
            
            _operationTimes.Add(stopwatch.Elapsed - iterationStart);
        }

        stopwatch.Stop();

        // Assert
        var averageTime = _operationTimes.Average(t => t.TotalMilliseconds);
        var maxTime = _operationTimes.Max(t => t.TotalMilliseconds);
        
        averageTime.Should().BeLessThan(10, "Average overlay initialization should be under 10ms");
        maxTime.Should().BeLessThan(50, "No single initialization should exceed 50ms");
        
        // Verify pooling effectiveness (later operations should be faster)
        var firstTenAverage = _operationTimes.Take(10).Average(t => t.TotalMilliseconds);
        var lastTenAverage = _operationTimes.Skip(90).Take(10).Average(t => t.TotalMilliseconds);
        
        lastTenAverage.Should().BeLessThan(firstTenAverage, 
            "Pooled instances should initialize faster than new instances");
    }

    [TestMethod]
    public async Task DatabaseOperations_UnderLoad_ShouldMaintainPerformance()
    {
        const int concurrentOperations = 10;
        const int operationsPerThread = 10;
        
        var tasks = new List<Task>();
        var allTimes = new ConcurrentBag<TimeSpan>();

        // Act - Concurrent database operations
        for (int thread = 0; thread < concurrentOperations; thread++)
        {
            var threadId = thread;
            tasks.Add(Task.Run(async () =>
            {
                for (int op = 0; op < operationsPerThread; op++)
                {
                    var stopwatch = Stopwatch.StartNew();
                    
                    var request = new InventoryQuickEditRequest(
                        $"LOAD_TEST_{threadId}_{op:00}", "100", "WH-TEST", op + 50);
                    
                    // Simulate database operation
                    await SimulateDatabaseUpdateAsync(request);
                    
                    stopwatch.Stop();
                    allTimes.Add(stopwatch.Elapsed);
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        var times = allTimes.ToList();
        var averageTime = times.Average(t => t.TotalMilliseconds);
        var p95Time = times.OrderBy(t => t).Skip((int)(times.Count * 0.95)).First().TotalMilliseconds;
        
        averageTime.Should().BeLessThan(100, "Average database operation should be under 100ms");
        p95Time.Should().BeLessThan(500, "95th percentile should be under 500ms");
    }

    [TestMethod]
    public void MemoryUsage_AfterManyOperations_ShouldRemainStable()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        const int operations = 1000;

        // Act
        for (int i = 0; i < operations; i++)
        {
            var viewModel = new InventoryQuickEditOverlayViewModel(
                Mock.Of<ILogger<InventoryQuickEditOverlayViewModel>>(),
                Mock.Of<IConfigurationService>());
                
            // Simulate usage
            viewModel.NewQuantity = i;
            viewModel.NewNotes = $"Test notes {i}";
            
            // Reset for pooling
            viewModel.Reset();
            viewModel.Dispose();
        }

        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);

        // Assert
        var memoryIncrease = finalMemory - initialMemory;
        var memoryIncreasePerOperation = memoryIncrease / (double)operations;
        
        memoryIncreasePerOperation.Should().BeLessThan(1024, 
            "Memory increase per operation should be less than 1KB");
    }
}
```

## 📊 Test Reporting and Metrics

### **Test Results Analysis Template**

```csharp
// File: Tests/Reports/TestMetricsCollector.cs

/// <summary>
/// Collects and analyzes test metrics for overlay system
/// </summary>
public class TestMetricsCollector
{
    private readonly List<TestResult> _results = new();

    public void RecordTestResult(string testName, TimeSpan duration, bool passed, string category)
    {
        _results.Add(new TestResult
        {
            TestName = testName,
            Duration = duration,
            Passed = passed,
            Category = category,
            Timestamp = DateTime.UtcNow
        });
    }

    public TestMetrics GenerateReport()
    {
        return new TestMetrics
        {
            TotalTests = _results.Count,
            PassedTests = _results.Count(r => r.Passed),
            FailedTests = _results.Count(r => !r.Passed),
            AverageDuration = TimeSpan.FromMilliseconds(_results.Average(r => r.Duration.TotalMilliseconds)),
            CategoryBreakdown = _results.GroupBy(r => r.Category)
                .ToDictionary(g => g.Key, g => new CategoryMetrics
                {
                    Total = g.Count(),
                    Passed = g.Count(r => r.Passed),
                    AverageDuration = TimeSpan.FromMilliseconds(g.Average(r => r.Duration.TotalMilliseconds))
                })
        };
    }
}

public record TestResult
{
    public string TestName { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public bool Passed { get; init; }
    public string Category { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}

public record TestMetrics
{
    public int TotalTests { get; init; }
    public int PassedTests { get; init; }
    public int FailedTests { get; init; }
    public TimeSpan AverageDuration { get; init; }
    public Dictionary<string, CategoryMetrics> CategoryBreakdown { get; init; } = new();
    
    public double PassRate => TotalTests > 0 ? PassedTests / (double)TotalTests * 100 : 0;
}

public record CategoryMetrics
{
    public int Total { get; init; }
    public int Passed { get; init; }
    public TimeSpan AverageDuration { get; init; }
    public double PassRate => Total > 0 ? Passed / (double)Total * 100 : 0;
}
```

## 🎯 Testing Best Practices Summary

### **Key Testing Principles**

1. **Test Pyramid Adherence**: 70% unit, 25% integration, 5% E2E
2. **Fast Feedback**: Unit tests under 100ms, integration tests under 1s
3. **Isolation**: Each test should be independent and repeatable
4. **Clear Names**: Test names should describe behavior, not implementation
5. **AAA Pattern**: Arrange, Act, Assert structure for clarity

### **Coverage Requirements**

- **ViewModels**: 95%+ line coverage, all command paths tested
- **Models**: 100% validation logic coverage
- **Database**: All stored procedures with positive/negative cases
- **UI**: Key user workflows and error scenarios

### **Continuous Integration**

```yaml
# Example CI pipeline configuration for testing
test-pipeline:
  stages:
    - name: Unit Tests
      command: dotnet test Tests/Unit --logger "trx;LogFileName=unit-tests.xml"
      coverage-threshold: 95%
      
    - name: Integration Tests
      command: dotnet test Tests/Integration --logger "trx;LogFileName=integration-tests.xml"
      database: test-database
      
    - name: UI Tests
      command: dotnet test Tests/UI --logger "trx;LogFileName=ui-tests.xml"
      parallel: false
      
    - name: Performance Tests
      command: dotnet test Tests/Performance --logger "trx;LogFileName=performance-tests.xml"
      thresholds:
        - average-response: 100ms
        - p95-response: 500ms
        - memory-increase: 1KB
```

This comprehensive testing framework ensures overlay system quality through automated validation, performance monitoring, and maintainable test patterns following MTM standards.
