using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTM.Tests.UnitTests.ViewModels
{
    [TestFixture]
    [Category("Unit")]
    [Category("ViewModels")]
    [Category("AdvancedRemove")]
    public class AdvancedRemoveViewModelTests
    {
        private AdvancedRemoveViewModel _viewModel = null!;
        private Mock<ILogger<AdvancedRemoveViewModel>> _mockLogger = null!;
        private Mock<IApplicationStateService> _mockApplicationStateService = null!;
        private Mock<IConfigurationService> _mockConfigurationService = null!;
        private Mock<IDatabaseService> _mockDatabaseService = null!;
        private Mock<IMasterDataService> _mockMasterDataService = null!;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger<AdvancedRemoveViewModel>>();
            _mockApplicationStateService = new Mock<IApplicationStateService>();
            _mockConfigurationService = new Mock<IConfigurationService>();
            _mockDatabaseService = new Mock<IDatabaseService>();
            _mockMasterDataService = new Mock<IMasterDataService>();

            // Setup default behavior
            _mockApplicationStateService.Setup(x => x.CurrentUser).Returns("TestUser");
            _mockConfigurationService.Setup(x => x.GetConnectionString()).Returns("TestConnectionString");
            _mockMasterDataService.Setup(x => x.PartIds).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Operations).Returns(new ObservableCollection<string>());
            _mockMasterDataService.Setup(x => x.Locations).Returns(new ObservableCollection<string>());

            _viewModel = new AdvancedRemoveViewModel(
                _mockLogger.Object,
                _mockConfigurationService.Object,
                _mockApplicationStateService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _viewModel?.Dispose();
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act & Assert
            _viewModel.Should().NotBeNull();
            _viewModel.InventoryItems.Should().NotBeNull();
            _viewModel.InventoryItems.Should().BeEmpty();
            _viewModel.SelectedItems.Should().NotBeNull();
            _viewModel.SelectedItems.Should().BeEmpty();
        }

        [Test]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new AdvancedRemoveViewModel(
                null!,
                _mockApplicationStateService.Object,
                _mockConfigurationService.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*logger*");
        }

        [Test]
        public void Constructor_WithNullApplicationStateService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var act = () => new AdvancedRemoveViewModel(
                _mockLogger.Object,
                null!,
                _mockConfigurationService.Object);

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*applicationState*");
        }

        #endregion

        #region Property Tests

        [Test]
        public void IsLoading_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.IsLoading))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.IsLoading = true;

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.IsLoading.Should().BeTrue();
        }

        [Test]
        public void SearchPartId_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.SearchPartId))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.SearchPartId = "PART001";

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.SearchPartId.Should().Be("PART001");
        }

        [Test]
        public void SearchOperation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.SearchOperation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.SearchOperation = "100";

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.SearchOperation.Should().Be("100");
        }

        [Test]
        public void SearchLocation_WhenSet_ShouldRaisePropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.SearchLocation))
                    propertyChangedRaised = true;
            };

            // Act
            _viewModel.SearchLocation = "A01";

            // Assert
            propertyChangedRaised.Should().BeTrue();
            _viewModel.SearchLocation.Should().Be("A01");
        }

        [Test]
        public void HasSelectedItems_WithNoSelection_ShouldReturnFalse()
        {
            // Arrange & Act
            _viewModel.SelectedItems.Clear();

            // Assert
            _viewModel.HasSelectedItems.Should().BeFalse();
        }

        [Test]
        public void HasSelectedItems_WithSelection_ShouldReturnTrue()
        {
            // Arrange
            var testItem = new InventoryItem
            {
                PartId = "PART001",
                Operation = "100",
                Quantity = 5,
                Location = "A01"
            };

            // Act
            _viewModel.SelectedItems.Add(testItem);

            // Assert
            _viewModel.HasSelectedItems.Should().BeTrue();
        }

        [Test]
        public void TotalSelectedQuantity_WithMultipleItems_ShouldSumCorrectly()
        {
            // Arrange
            var item1 = new InventoryItem { Quantity = 10 };
            var item2 = new InventoryItem { Quantity = 25 };
            var item3 = new InventoryItem { Quantity = 15 };

            // Act
            _viewModel.SelectedItems.Add(item1);
            _viewModel.SelectedItems.Add(item2);
            _viewModel.SelectedItems.Add(item3);

            // Assert
            _viewModel.TotalSelectedQuantity.Should().Be(50);
        }

        #endregion

        #region Command Tests

        [Test]
        public async Task SearchCommand_WithValidCriteria_ShouldLoadResults()
        {
            // Arrange
            var testItems = new List<InventoryItem>
            {
                new() { PartId = "PART001", Operation = "100", Quantity = 10, Location = "A01" },
                new() { PartId = "PART001", Operation = "110", Quantity = 15, Location = "A02" }
            };

            _mockDatabaseService.Setup(x => x.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(testItems);

            _viewModel.SearchPartId = "PART001";

            // Act
            await _viewModel.SearchCommand.ExecuteAsync(null);

            // Assert
            _mockDatabaseService.Verify(x => x.SearchInventoryAsync("PART001", "", ""), Times.Once);
            _viewModel.InventoryItems.Should().HaveCount(2);
            _viewModel.InventoryItems[0].PartId.Should().Be("PART001");
        }

        [Test]
        public async Task SearchCommand_WhenDatabaseFails_ShouldHandleError()
        {
            // Arrange
            _mockDatabaseService.Setup(x => x.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Database connection failed"));

            _viewModel.SearchPartId = "PART001";

            // Act
            await _viewModel.SearchCommand.ExecuteAsync(null);

            // Assert
            _viewModel.InventoryItems.Should().BeEmpty();
            _viewModel.StatusMessage.Should().Contain("Failed to search");
        }

        [Test]
        public async Task BulkRemoveCommand_WithSelectedItems_ShouldRemoveSuccessfully()
        {
            // Arrange
            var item1 = new InventoryItem
            {
                PartId = "PART001",
                Operation = "100",
                Quantity = 10,
                Location = "A01"
            };
            var item2 = new InventoryItem
            {
                PartId = "PART002",
                Operation = "110",
                Quantity = 15,
                Location = "A02"
            };

            _viewModel.SelectedItems.Add(item1);
            _viewModel.SelectedItems.Add(item2);

            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.BulkRemoveCommand.ExecuteAsync(null);

            // Assert
            _mockDatabaseService.Verify(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()), Times.Exactly(2));
            _viewModel.StatusMessage.Should().Contain("Successfully removed 2 items");
        }

        [Test]
        public void BulkRemoveCommand_CanExecute_WithNoSelectedItems_ShouldReturnFalse()
        {
            // Arrange
            _viewModel.SelectedItems.Clear();
            _viewModel.IsLoading = false;

            // Act & Assert
            _viewModel.BulkRemoveCommand.CanExecute(null).Should().BeFalse();
        }

        [Test]
        public void BulkRemoveCommand_CanExecute_WithSelectedItemsAndNotLoading_ShouldReturnTrue()
        {
            // Arrange
            _viewModel.SelectedItems.Add(new InventoryItem());
            _viewModel.IsLoading = false;

            // Act & Assert
            _viewModel.BulkRemoveCommand.CanExecute(null).Should().BeTrue();
        }

        [Test]
        public async Task SelectAllCommand_WithItemsInGrid_ShouldSelectAll()
        {
            // Arrange
            var items = new List<InventoryItem>
            {
                new() { PartId = "PART001" },
                new() { PartId = "PART002" },
                new() { PartId = "PART003" }
            };

            foreach (var item in items)
            {
                _viewModel.InventoryItems.Add(item);
            }

            // Act
            await _viewModel.SelectAllCommand.ExecuteAsync(null);

            // Assert
            _viewModel.SelectedItems.Should().HaveCount(3);
            _viewModel.SelectedItems.Should().Contain(items);
        }

        [Test]
        public async Task ClearSelectionCommand_WithSelectedItems_ShouldClearSelection()
        {
            // Arrange
            _viewModel.SelectedItems.Add(new InventoryItem());
            _viewModel.SelectedItems.Add(new InventoryItem());

            // Act
            await _viewModel.ClearSelectionCommand.ExecuteAsync(null);

            // Assert
            _viewModel.SelectedItems.Should().BeEmpty();
        }

        [Test]
        public async Task ResetSearchCommand_WithSearchCriteria_ShouldClearCriteria()
        {
            // Arrange
            _viewModel.SearchPartId = "PART001";
            _viewModel.SearchOperation = "100";
            _viewModel.SearchLocation = "A01";

            // Act
            await _viewModel.ResetSearchCommand.ExecuteAsync(null);

            // Assert
            _viewModel.SearchPartId.Should().BeEmpty();
            _viewModel.SearchOperation.Should().BeEmpty();
            _viewModel.SearchLocation.Should().BeEmpty();
            _viewModel.InventoryItems.Should().BeEmpty();
            _viewModel.SelectedItems.Should().BeEmpty();
        }

        #endregion

        #region MTM Manufacturing Domain Tests

        [Test]
        [TestCase("PART001", "100", "A01", true)]
        [TestCase("ABC-123", "110", "STATION_B", true)]
        [TestCase("", "100", "A01", false)]
        [TestCase("PART001", "", "A01", false)]
        [TestCase("PART001", "100", "", false)]
        public async Task ValidateSearchCriteria_VariousInputs_ShouldFollowMTMStandards(
            string partId, string operation, string location, bool shouldBeValid)
        {
            // Arrange
            _viewModel.SearchPartId = partId;
            _viewModel.SearchOperation = operation;
            _viewModel.SearchLocation = location;

            if (shouldBeValid)
            {
                _mockDatabaseService.Setup(x => x.SearchInventoryAsync(partId, operation, location))
                    .ReturnsAsync(new List<InventoryItem>());
            }

            // Act
            await _viewModel.SearchCommand.ExecuteAsync(null);

            // Assert
            if (shouldBeValid)
            {
                _mockDatabaseService.Verify(x => x.SearchInventoryAsync(partId, operation, location), Times.Once);
            }
            else
            {
                _mockDatabaseService.Verify(x => x.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
                _viewModel.StatusMessage.Should().Contain("criteria");
            }
        }

        [Test]
        public async Task BulkRemove_WithMixedOperations_ShouldProcessAllCorrectly()
        {
            // Arrange - Mix of different manufacturing operations
            var items = new List<InventoryItem>
            {
                new() { PartId = "PART001", Operation = "90", Quantity = 10, Location = "RECEIVING" },   // Receiving
                new() { PartId = "PART002", Operation = "100", Quantity = 15, Location = "STATION_A" }, // First op
                new() { PartId = "PART003", Operation = "110", Quantity = 20, Location = "STATION_B" }, // Second op
                new() { PartId = "PART004", Operation = "120", Quantity = 8, Location = "STATION_C" }   // Final op
            };

            foreach (var item in items)
            {
                _viewModel.SelectedItems.Add(item);
            }

            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.BulkRemoveCommand.ExecuteAsync(null);

            // Assert
            _mockDatabaseService.Verify(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()), Times.Exactly(4));
            _viewModel.StatusMessage.Should().Contain("Successfully removed 4 items");
        }

        [Test]
        public void FilterByOperation_WithStandardOperations_ShouldReturnCorrectItems()
        {
            // Arrange
            var allItems = new List<InventoryItem>
            {
                new() { PartId = "PART001", Operation = "90" },
                new() { PartId = "PART002", Operation = "100" },
                new() { PartId = "PART003", Operation = "110" },
                new() { PartId = "PART004", Operation = "120" },
                new() { PartId = "PART005", Operation = "100" }
            };

            foreach (var item in allItems)
            {
                _viewModel.InventoryItems.Add(item);
            }

            // Act
            var operation100Items = _viewModel.InventoryItems.Where(i => i.Operation == "100").ToList();

            // Assert
            operation100Items.Should().HaveCount(2);
            operation100Items.All(i => i.Operation == "100").Should().BeTrue();
        }

        #endregion

        #region Error Handling Tests

        [Test]
        public async Task BulkRemoveCommand_WhenSomeItemsFail_ShouldContinueWithOthers()
        {
            // Arrange
            var item1 = new InventoryItem { PartId = "PART001", Quantity = 10 };
            var item2 = new InventoryItem { PartId = "PART002", Quantity = 15 };
            var item3 = new InventoryItem { PartId = "PART003", Quantity = 20 };

            _viewModel.SelectedItems.Add(item1);
            _viewModel.SelectedItems.Add(item2);
            _viewModel.SelectedItems.Add(item3);

            // Setup: first and third succeed, second fails
            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(item1)).ReturnsAsync(true);
            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(item2)).ThrowsAsync(new InvalidOperationException("Remove failed"));
            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(item3)).ReturnsAsync(true);

            // Act
            await _viewModel.BulkRemoveCommand.ExecuteAsync(null);

            // Assert
            _mockDatabaseService.Verify(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()), Times.Exactly(3));
            _viewModel.StatusMessage.Should().Contain("2 items removed successfully, 1 failed");
        }

        [Test]
        public async Task SearchCommand_WhenNoItemsFound_ShouldShowAppropriateMessage()
        {
            // Arrange
            _mockDatabaseService.Setup(x => x.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<InventoryItem>());

            _viewModel.SearchPartId = "NONEXISTENT";

            // Act
            await _viewModel.SearchCommand.ExecuteAsync(null);

            // Assert
            _viewModel.InventoryItems.Should().BeEmpty();
            _viewModel.StatusMessage.Should().Contain("No items found");
        }

        #endregion

        #region Integration Tests

        [Test]
        public async Task CompleteWorkflow_SearchSelectRemove_ShouldWorkCorrectly()
        {
            // Arrange
            var searchResults = new List<InventoryItem>
            {
                new() { PartId = "WORKFLOW_001", Operation = "100", Quantity = 10, Location = "A01" },
                new() { PartId = "WORKFLOW_002", Operation = "100", Quantity = 15, Location = "A02" },
                new() { PartId = "WORKFLOW_003", Operation = "100", Quantity = 20, Location = "A03" }
            };

            _mockDatabaseService.Setup(x => x.SearchInventoryAsync("WORKFLOW", "", ""))
                .ReturnsAsync(searchResults);
            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()))
                .ReturnsAsync(true);

            _viewModel.SearchPartId = "WORKFLOW";

            // Act 1: Search
            await _viewModel.SearchCommand.ExecuteAsync(null);
            _viewModel.InventoryItems.Should().HaveCount(3);

            // Act 2: Select first two items
            _viewModel.SelectedItems.Add(searchResults[0]);
            _viewModel.SelectedItems.Add(searchResults[1]);
            _viewModel.HasSelectedItems.Should().BeTrue();
            _viewModel.TotalSelectedQuantity.Should().Be(25);

            // Act 3: Remove selected items
            await _viewModel.BulkRemoveCommand.ExecuteAsync(null);

            // Assert
            _mockDatabaseService.Verify(x => x.SearchInventoryAsync("WORKFLOW", "", ""), Times.Once);
            _mockDatabaseService.Verify(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()), Times.Exactly(2));
            _viewModel.StatusMessage.Should().Contain("Successfully removed 2 items");
        }

        #endregion

        #region Performance Tests

        [Test]
        public async Task BulkRemove_WithLargeSelection_ShouldCompleteReasonably()
        {
            // Arrange
            var largeSelection = new List<InventoryItem>();
            for (int i = 1; i <= 50; i++)
            {
                largeSelection.Add(new InventoryItem
                {
                    PartId = $"BULK{i:000}",
                    Operation = "100",
                    Quantity = i,
                    Location = $"STATION_{(char)('A' + i % 5)}"
                });
            }

            foreach (var item in largeSelection)
            {
                _viewModel.SelectedItems.Add(item);
            }

            _mockDatabaseService.Setup(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()))
                .ReturnsAsync(true);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await _viewModel.BulkRemoveCommand.ExecuteAsync(null);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Bulk removal of 50 items should complete within 5 seconds");
            _mockDatabaseService.Verify(x => x.RemoveInventoryAsync(It.IsAny<InventoryItem>()), Times.Exactly(50));
            _viewModel.StatusMessage.Should().Contain("Successfully removed 50 items");
        }

        [Test]
        public async Task Search_WithLargeResultSet_ShouldLoadEfficiently()
        {
            // Arrange
            var largeResultSet = new List<InventoryItem>();
            for (int i = 1; i <= 200; i++)
            {
                largeResultSet.Add(new InventoryItem
                {
                    PartId = $"SEARCH{i:000}",
                    Operation = (90 + (i % 4) * 10).ToString(),
                    Quantity = i % 50 + 1,
                    Location = $"LOCATION_{(char)('A' + i % 10)}"
                });
            }

            _mockDatabaseService.Setup(x => x.SearchInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(largeResultSet);

            _viewModel.SearchPartId = "SEARCH";

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await _viewModel.SearchCommand.ExecuteAsync(null);
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, "Loading 200 search results should complete within 2 seconds");
            _viewModel.InventoryItems.Should().HaveCount(200);
        }

        #endregion

        #region Selection Management Tests

        [Test]
        public void SelectedItems_CollectionChanged_ShouldUpdateProperties()
        {
            // Arrange
            var item1 = new InventoryItem { Quantity = 10 };
            var item2 = new InventoryItem { Quantity = 15 };

            bool hasSelectedItemsChanged = false;
            bool totalQuantityChanged = false;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_viewModel.HasSelectedItems))
                    hasSelectedItemsChanged = true;
                if (e.PropertyName == nameof(_viewModel.TotalSelectedQuantity))
                    totalQuantityChanged = true;
            };

            // Act
            _viewModel.SelectedItems.Add(item1);
            _viewModel.SelectedItems.Add(item2);

            // Assert
            hasSelectedItemsChanged.Should().BeTrue();
            totalQuantityChanged.Should().BeTrue();
            _viewModel.HasSelectedItems.Should().BeTrue();
            _viewModel.TotalSelectedQuantity.Should().Be(25);
        }

        #endregion

        #region Disposal Tests

        [Test]
        public void Dispose_WhenCalled_ShouldNotThrow()
        {
            // Act & Assert
            var act = () => _viewModel.Dispose();
            act.Should().NotThrow();
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_ShouldNotThrow()
        {
            // Act & Assert
            var act = () =>
            {
                _viewModel.Dispose();
                _viewModel.Dispose();
                _viewModel.Dispose();
            };
            act.Should().NotThrow();
        }

        #endregion
    }
}