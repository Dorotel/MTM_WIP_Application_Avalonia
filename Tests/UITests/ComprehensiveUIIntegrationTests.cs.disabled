using NUnit.Framework;
using FluentAssertions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Threading;
using MTM_WIP_Application_Avalonia;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System;

namespace MTM.Tests.UITests
{
    [TestFixture]
    [Category("UI")]
    [Category("Integration")]
    [Category("Comprehensive")]
    public class ComprehensiveUIIntegrationTests
    {
        private Application _app = null!;
        private IServiceProvider _serviceProvider = null!;
        private Mock<ILogger<InventoryTabViewModel>> _mockLogger = null!;
        private Mock<IDatabaseService> _mockDatabaseService = null!;
        private Mock<IMasterDataService> _mockMasterDataService = null!;
        private Mock<IApplicationStateService> _mockApplicationStateService = null!;
        private Mock<INavigationService> _mockNavigationService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Initialize mocks
            _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockMasterDataService = new Mock<IMasterDataService>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockNavigationService = new Mock<INavigationService>();

            // Setup default mock behavior
            SetupMockDefaults();

            // Configure service provider for UI tests
            var services = new ServiceCollection();
            ConfigureUITestServices(services);
            _serviceProvider = services.BuildServiceProvider();

            // Initialize Avalonia for headless testing
            try
            {
                AppBuilder.Configure<App>()
                    .UseHeadless(new AvaloniaHeadlessPlatformOptions
                    {
                        UseHeadlessDrawing = true
                    })
                    .SetupWithoutStarting();

                _app = Application.Current ?? throw new InvalidOperationException("Application not initialized");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Warning: Avalonia initialization failed: {ex.Message}");
                // Continue with tests that don't require full Avalonia setup
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            (_serviceProvider as IDisposable)?.Dispose();
        }

        private void SetupMockDefaults()
        {
            _mockApplicationStateService.Setup(x => x.CurrentUser).Returns("UITestUser");
            _mockMasterDataService.Setup(x => x.PartIds).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Operations).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Locations).Returns(new ObservableCollection<string>());
            _mockDatabaseService.Setup(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            _mockDatabaseService.Setup(x => x.GetInventoryAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<InventoryItem>());
        }

        private void ConfigureUITestServices(IServiceCollection services)
        {
            services.AddSingleton(_mockLogger.Object);
            services.AddSingleton(_mockDatabaseService.Object);
            services.AddSingleton(_mockMasterDataService.Object);
            services.AddSingleton(_mockApplicationStateService.Object);
            services.AddSingleton(_mockNavigationService.Object);
        }

        #region ViewModel UI Integration Tests

        [Test]
        public void InventoryTabViewModel_DataBinding_ShouldSynchronizeWithUI()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            var propertyChangedEvents = new List<string>();
            viewModel.PropertyChanged += (s, e) => propertyChangedEvents.Add(e.PropertyName ?? "");

            // Act
            viewModel.PartId = "UI_BINDING_001";
            viewModel.Operation = "100";
            viewModel.Quantity = 25;
            viewModel.Location = "UI_STATION";
            viewModel.IsLoading = true;

            // Assert
            propertyChangedEvents.Should().Contain("PartId");
            propertyChangedEvents.Should().Contain("Operation");
            propertyChangedEvents.Should().Contain("Quantity");
            propertyChangedEvents.Should().Contain("Location");
            propertyChangedEvents.Should().Contain("IsLoading");

            viewModel.PartId.Should().Be("UI_BINDING_001");
            viewModel.Operation.Should().Be("100");
            viewModel.Quantity.Should().Be(25);
            viewModel.Location.Should().Be("UI_STATION");
            viewModel.IsLoading.Should().BeTrue();
        }

        [Test]
        public async Task InventoryTabViewModel_CommandExecution_ShouldUpdateUIState()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            viewModel.PartId = "UI_COMMAND_001";
            viewModel.Operation = "100";
            viewModel.Quantity = 15;
            viewModel.Location = "COMMAND_STATION";

            // Act
            var initialIsLoading = viewModel.IsLoading;
            var saveTask = viewModel.SaveCommand.ExecuteAsync(null);
            var duringExecutionIsLoading = viewModel.IsLoading;
            await saveTask;
            var afterExecutionIsLoading = viewModel.IsLoading;

            // Assert
            initialIsLoading.Should().BeFalse("Initially should not be loading");
            afterExecutionIsLoading.Should().BeFalse("Should not be loading after completion");

            _mockDatabaseService.Verify(x => x.AddInventoryAsync("UI_COMMAND_001", "100", 15, "COMMAND_STATION"), Times.Once);
        }

        [Test]
        public void InventoryTabViewModel_ValidationRules_ShouldPreventInvalidSubmission()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            // Test cases for validation
            var testCases = new[]
            {
                new { PartId = "", Operation = "100", Quantity = 5, Location = "A01", ShouldBeValid = false },
                new { PartId = "PART001", Operation = "", Quantity = 5, Location = "A01", ShouldBeValid = false },
                new { PartId = "PART001", Operation = "100", Quantity = 0, Location = "A01", ShouldBeValid = false },
                new { PartId = "PART001", Operation = "100", Quantity = 5, Location = "", ShouldBeValid = false },
                new { PartId = "PART001", Operation = "100", Quantity = 5, Location = "A01", ShouldBeValid = true }
            };

            foreach (var testCase in testCases)
            {
                // Act
                viewModel.PartId = testCase.PartId;
                viewModel.Operation = testCase.Operation;
                viewModel.Quantity = testCase.Quantity;
                viewModel.Location = testCase.Location;

                // Assert
                var canExecute = viewModel.SaveCommand.CanExecute(null);
                canExecute.Should().Be(testCase.ShouldBeValid, 
                    $"PartId='{testCase.PartId}', Operation='{testCase.Operation}', Quantity={testCase.Quantity}, Location='{testCase.Location}' should be {(testCase.ShouldBeValid ? "valid" : "invalid")}");
            }
        }

        #endregion

        #region Collection UI Integration Tests

        [Test]
        public async Task MasterDataCollections_UIBinding_ShouldUpdateReactively()
        {
            // Arrange
            var partIds = new ObservableCollection<string>();
            var operations = new ObservableCollection<string>();
            var locations = new ObservableCollection<string>();

            _mockMasterDataService.Setup(x => x.PartIds).Returns(partIds);
            _mockMasterDataService.Setup(x => x.Operations).Returns(operations);
            _mockMasterDataService.Setup(x => x.Locations).Returns(locations);

            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            // Act - Add items to collections
            partIds.Add("PART001");
            partIds.Add("PART002");
            operations.Add("90");
            operations.Add("100");
            locations.Add("STATION_A");
            locations.Add("STATION_B");

            // Assert - ViewModel should have access to updated collections
            viewModel.MasterData_PartIds.Should().HaveCount(2);
            viewModel.MasterData_Operations.Should().HaveCount(2);
            viewModel.MasterData_Locations.Should().HaveCount(2);

            viewModel.MasterData_PartIds.Should().Contain("PART001");
            viewModel.MasterData_PartIds.Should().Contain("PART002");
            viewModel.MasterData_Operations.Should().Contain("90");
            viewModel.MasterData_Operations.Should().Contain("100");
            viewModel.MasterData_Locations.Should().Contain("STATION_A");
            viewModel.MasterData_Locations.Should().Contain("STATION_B");
        }

        [Test]
        public void InventoryCollection_DynamicUpdates_ShouldReflectInUI()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            var collectionChangedEvents = new List<string>();
            viewModel.RecentTransactions.CollectionChanged += (s, e) =>
            {
                collectionChangedEvents.Add($"{e.Action}");
            };

            // Act - Simulate adding recent transactions
            var transaction1 = new SessionTransaction
            {
                CurrentPartId = "RECENT_001",
                CurrentOperation = "100",
                CurrentQuantity = 10,
                CurrentLocation = "STATION_A"
            };

            var transaction2 = new SessionTransaction
            {
                CurrentPartId = "RECENT_002",
                CurrentOperation = "110",
                CurrentQuantity = 15,
                CurrentLocation = "STATION_B"
            };

            viewModel.RecentTransactions.Add(transaction1);
            viewModel.RecentTransactions.Add(transaction2);

            // Assert
            collectionChangedEvents.Should().HaveCount(2);
            collectionChangedEvents.Should().AllSatisfy(e => e.Should().Contain("Add"));
            viewModel.RecentTransactions.Should().HaveCount(2);
            viewModel.RecentTransactions[0].CurrentPartId.Should().Be("RECENT_001");
            viewModel.RecentTransactions[1].CurrentPartId.Should().Be("RECENT_002");
        }

        #endregion

        #region Error Handling UI Integration Tests

        [Test]
        public async Task InventoryTabViewModel_ServiceError_ShouldDisplayUserFriendlyMessage()
        {
            // Arrange
            _mockDatabaseService.Setup(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            viewModel.PartId = "ERROR_TEST_001";
            viewModel.Operation = "100";
            viewModel.Quantity = 10;
            viewModel.Location = "ERROR_STATION";

            // Act
            await viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            viewModel.StatusMessage.Should().NotBeEmpty();
            viewModel.StatusMessage.Should().Contain("Failed");
            viewModel.IsLoading.Should().BeFalse();
        }

        [Test]
        public async Task InventoryTabViewModel_ValidationError_ShouldPreventExecution()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            // Set invalid state
            viewModel.PartId = ""; // Invalid - empty part ID
            viewModel.Operation = "100";
            viewModel.Quantity = 10;
            viewModel.Location = "VALIDATION_STATION";

            // Act
            var canExecuteBefore = viewModel.SaveCommand.CanExecute(null);
            await viewModel.SaveCommand.ExecuteAsync(null);

            // Assert
            canExecuteBefore.Should().BeFalse("Command should not be executable with invalid data");
            _mockDatabaseService.Verify(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Multi-ViewModel UI Integration Tests

        [Test]
        public void MultipleViewModels_IndependentState_ShouldNotInterfere()
        {
            // Arrange
            using var viewModel1 = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            using var viewModel2 = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            // Act
            viewModel1.PartId = "VM1_PART";
            viewModel1.Operation = "100";
            viewModel1.Quantity = 10;

            viewModel2.PartId = "VM2_PART";
            viewModel2.Operation = "110";
            viewModel2.Quantity = 20;

            // Assert
            viewModel1.PartId.Should().Be("VM1_PART");
            viewModel1.Operation.Should().Be("100");
            viewModel1.Quantity.Should().Be(10);

            viewModel2.PartId.Should().Be("VM2_PART");
            viewModel2.Operation.Should().Be("110");
            viewModel2.Quantity.Should().Be(20);

            // ViewModels should have independent state
            viewModel1.PartId.Should().NotBe(viewModel2.PartId);
            viewModel1.Operation.Should().NotBe(viewModel2.Operation);
            viewModel1.Quantity.Should().NotBe(viewModel2.Quantity);
        }

        [Test]
        public async Task MultipleViewModels_ConcurrentOperations_ShouldExecuteIndependently()
        {
            // Arrange
            const int viewModelCount = 5;
            var viewModels = new List<InventoryTabViewModel>();

            for (int i = 0; i < viewModelCount; i++)
            {
                var viewModel = new InventoryTabViewModel(
                    _mockLogger.Object,
                    _mockDatabaseService.Object,
                    _mockMasterDataService.Object,
                    _mockApplicationStateService.Object,
                    _mockNavigationService.Object);

                viewModel.PartId = $"CONCURRENT_VM_{i:00}";
                viewModel.Operation = "100";
                viewModel.Quantity = i + 1;
                viewModel.Location = $"STATION_{i}";

                viewModels.Add(viewModel);
            }

            // Act
            var tasks = viewModels.Select(vm => vm.SaveCommand.ExecuteAsync(null));
            await Task.WhenAll(tasks);

            // Assert
            _mockDatabaseService.Verify(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), 
                Times.Exactly(viewModelCount));

            // Cleanup
            foreach (var vm in viewModels)
            {
                vm.Dispose();
            }
        }

        #endregion

        #region UI State Management Tests

        [Test]
        public void InventoryTabViewModel_StateTransitions_ShouldFollowExpectedFlow()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            var stateChanges = new List<(string Property, object Value)>();
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != null)
                {
                    var property = viewModel.GetType().GetProperty(e.PropertyName);
                    if (property != null)
                    {
                        var value = property.GetValue(viewModel);
                        stateChanges.Add((e.PropertyName, value ?? "null"));
                    }
                }
            };

            // Act - Simulate user interaction flow
            viewModel.PartId = "STATE_TEST_001";
            viewModel.Operation = "100";
            viewModel.Quantity = 5;
            viewModel.Location = "STATE_STATION";
            viewModel.IsLoading = true;
            viewModel.StatusMessage = "Processing...";
            viewModel.IsLoading = false;
            viewModel.StatusMessage = "Completed successfully";

            // Assert
            stateChanges.Should().NotBeEmpty();
            stateChanges.Should().Contain(change => change.Property == "PartId" && change.Value.Equals("STATE_TEST_001"));
            stateChanges.Should().Contain(change => change.Property == "Operation" && change.Value.Equals("100"));
            stateChanges.Should().Contain(change => change.Property == "Quantity" && change.Value.Equals(5));
            stateChanges.Should().Contain(change => change.Property == "Location" && change.Value.Equals("STATE_STATION"));
            stateChanges.Should().Contain(change => change.Property == "IsLoading" && change.Value.Equals(true));
            stateChanges.Should().Contain(change => change.Property == "IsLoading" && change.Value.Equals(false));
        }

        [Test]
        public async Task InventoryTabViewModel_ResetCommand_ShouldRestoreInitialState()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            // Set some values
            viewModel.PartId = "RESET_TEST_001";
            viewModel.Operation = "100";
            viewModel.Quantity = 25;
            viewModel.Location = "RESET_STATION";
            viewModel.StatusMessage = "Some status message";

            // Act
            await viewModel.ResetCommand.ExecuteAsync(null);

            // Assert
            viewModel.PartId.Should().BeEmpty();
            viewModel.Operation.Should().BeEmpty();
            viewModel.Quantity.Should().Be(1); // Default quantity
            viewModel.Location.Should().BeEmpty();
            viewModel.StatusMessage.Should().BeEmpty();
            viewModel.IsLoading.Should().BeFalse();
        }

        #endregion

        #region Manufacturing Domain UI Integration Tests

        [Test]
        public async Task InventoryTabViewModel_ManufacturingWorkflow_ShouldSupportCompleteOperationSequence()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            var workflowOperations = new[] { "90", "100", "110", "120" };
            var workflowLocations = new[] { "RECEIVING", "STATION_A", "STATION_B", "FINAL_STATION" };
            var workflowQuantities = new[] { 100, 95, 90, 85 };

            var executedOperations = new List<(string PartId, string Operation, int Quantity, string Location)>();
            _mockDatabaseService.Setup(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback<string, string, int, string>((partId, operation, quantity, location) =>
                {
                    executedOperations.Add((partId, operation, quantity, location));
                })
                .ReturnsAsync(true);

            // Act - Execute complete manufacturing workflow
            for (int i = 0; i < workflowOperations.Length; i++)
            {
                viewModel.PartId = "WORKFLOW_PART_001";
                viewModel.Operation = workflowOperations[i];
                viewModel.Quantity = workflowQuantities[i];
                viewModel.Location = workflowLocations[i];

                await viewModel.SaveCommand.ExecuteAsync(null);
            }

            // Assert
            executedOperations.Should().HaveCount(workflowOperations.Length);
            
            for (int i = 0; i < workflowOperations.Length; i++)
            {
                var execution = executedOperations[i];
                execution.PartId.Should().Be("WORKFLOW_PART_001");
                execution.Operation.Should().Be(workflowOperations[i]);
                execution.Quantity.Should().Be(workflowQuantities[i]);
                execution.Location.Should().Be(workflowLocations[i]);
            }
        }

        [Test]
        public void InventoryTabViewModel_MTMTransactionTypes_ShouldSupportAllStandardTypes()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            var transactionTypes = new[] { "IN", "OUT", "TRANSFER" };

            // Act & Assert
            foreach (var transactionType in transactionTypes)
            {
                viewModel.TransactionType = transactionType;
                viewModel.TransactionType.Should().Be(transactionType, 
                    $"Transaction type {transactionType} should be supported");

                // Verify transaction type affects UI state appropriately
                var isValidTransactionType = !string.IsNullOrWhiteSpace(viewModel.TransactionType);
                isValidTransactionType.Should().BeTrue($"Transaction type {transactionType} should be considered valid");
            }
        }

        #endregion

        #region Performance UI Integration Tests

        [Test]
        public void InventoryTabViewModel_HighFrequencyPropertyChanges_ShouldMaintainResponsiveness()
        {
            // Arrange
            using var viewModel = new InventoryTabViewModel(
                _mockLogger.Object,
                _mockDatabaseService.Object,
                _mockMasterDataService.Object,
                _mockApplicationStateService.Object,
                _mockNavigationService.Object);

            const int changeCount = 1000;
            var changeEvents = 0;

            viewModel.PropertyChanged += (s, e) => changeEvents++;

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < changeCount; i++)
            {
                viewModel.PartId = $"PERF_PART_{i:0000}";
                viewModel.Quantity = i % 100 + 1;
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, 
                $"{changeCount} property changes should complete within 1 second");
            changeEvents.Should().Be(changeCount * 2); // PartId and Quantity changes
            viewModel.PartId.Should().Be($"PERF_PART_{changeCount - 1:0000}");
            viewModel.Quantity.Should().Be((changeCount - 1) % 100 + 1);
        }

        [Test]
        public async Task InventoryTabViewModel_ConcurrentUIOperations_ShouldHandleCorrectly()
        {
            // Arrange
            const int concurrentOperations = 10;
            var viewModels = new List<InventoryTabViewModel>();
            var tasks = new List<Task>();

            for (int i = 0; i < concurrentOperations; i++)
            {
                var viewModel = new InventoryTabViewModel(
                    _mockLogger.Object,
                    _mockDatabaseService.Object,
                    _mockMasterDataService.Object,
                    _mockApplicationStateService.Object,
                    _mockNavigationService.Object);

                viewModel.PartId = $"CONCURRENT_UI_{i:00}";
                viewModel.Operation = "100";
                viewModel.Quantity = i + 1;
                viewModel.Location = "CONCURRENT_STATION";

                viewModels.Add(viewModel);
            }

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var vm in viewModels)
            {
                tasks.Add(vm.SaveCommand.ExecuteAsync(null));
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(3000, 
                $"{concurrentOperations} concurrent UI operations should complete within 3 seconds");

            _mockDatabaseService.Verify(x => x.AddInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), 
                Times.Exactly(concurrentOperations));

            // Cleanup
            foreach (var vm in viewModels)
            {
                vm.Dispose();
            }
        }

        #endregion
    }
}