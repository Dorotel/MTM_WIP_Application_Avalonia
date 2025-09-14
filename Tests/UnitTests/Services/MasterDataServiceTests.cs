using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM.Tests.UnitTests.Services;

/// <summary>
/// Unit tests for MasterDataService following MTM testing patterns
/// Tests MTM "NO FALLBACK DATA PATTERN" and ObservableCollection management
/// Validates manufacturing domain data handling
/// </summary>
[TestFixture]
[Category("Unit")]
[Category("Service")]
public class MasterDataServiceTests
{
    #region Test Setup and Mocks

    private Mock<ILogger<MasterDataService>> _mockLogger = null!;
    private Mock<IConfigurationService> _mockConfigurationService = null!;
    private Mock<IDatabaseService> _mockDatabaseService = null!;
    
    private MasterDataService _masterDataService = null!;

    [SetUp]
    public void SetUp()
    {
        // Initialize mocks
        _mockLogger = new Mock<ILogger<MasterDataService>>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockDatabaseService = new Mock<IDatabaseService>();

        // Setup default mock behavior
        SetupDefaultMockBehavior();

        // Create service instance
        _masterDataService = new MasterDataService(
            _mockLogger.Object,
            _mockConfigurationService.Object,
            _mockDatabaseService.Object
        );
    }

    private void SetupDefaultMockBehavior()
    {
        // Setup configuration service defaults
        _mockConfigurationService.Setup(s => s.GetConnectionString())
            .Returns("Server=localhost;Database=mtm_test;Uid=test;Pwd=test;");
    }

    #endregion

    #region ObservableCollection Tests

    [Test]
    public void MasterDataService_Initialization_ShouldCreateEmptyCollections()
    {
        // Assert - All collections should be initialized but empty
        _masterDataService.PartIds.Should().NotBeNull();
        _masterDataService.Operations.Should().NotBeNull();
        _masterDataService.Locations.Should().NotBeNull();
        _masterDataService.Users.Should().NotBeNull();

        _masterDataService.PartIds.Should().BeEmpty();
        _masterDataService.Operations.Should().BeEmpty();
        _masterDataService.Locations.Should().BeEmpty();
        _masterDataService.Users.Should().BeEmpty();
    }

    [Test]
    public void MasterDataService_Collections_ShouldBeObservableCollections()
    {
        // Assert - All collections should be ObservableCollection type for MVVM binding
        _masterDataService.PartIds.Should().BeOfType<ObservableCollection<string>>();
        _masterDataService.Operations.Should().BeOfType<ObservableCollection<string>>();
        _masterDataService.Locations.Should().BeOfType<ObservableCollection<string>>();
        _masterDataService.Users.Should().BeOfType<ObservableCollection<string>>();
    }

    [Test]
    public void MasterDataService_IsLoading_ShouldInitializeToFalse()
    {
        // Assert
        _masterDataService.IsLoading.Should().BeFalse("IsLoading should be false initially");
    }

    #endregion

    #region Manufacturing Domain Logic Tests

    [Test]
    public void MasterDataService_FollowsNoFallbackPattern()
    {
        // Act - Without loading data, collections should remain empty
        var partIds = _masterDataService.PartIds;
        var operations = _masterDataService.Operations;
        var locations = _masterDataService.Locations;

        // Assert - Following MTM NO FALLBACK DATA PATTERN
        partIds.Should().BeEmpty("Should not provide fallback data when database unavailable");
        operations.Should().BeEmpty("Should not provide fallback data when database unavailable");
        locations.Should().BeEmpty("Should not provide fallback data when database unavailable");
    }

    [Test]
    public async Task MasterDataService_LoadAllMasterDataAsync_ShouldHandleGracefully()
    {
        // Act & Assert - Should not throw exception even with mock services
        Func<Task> action = async () => await _masterDataService.LoadAllMasterDataAsync();
        await action.Should().NotThrowAsync("Service should handle database issues gracefully");
    }

    #endregion

    #region Event Handling Tests

    [Test]
    public void MasterDataService_MasterDataLoaded_ShouldHaveEvent()
    {
        // Arrange
        var eventFired = false;
        _masterDataService.MasterDataLoaded += (s, e) => eventFired = true;

        // Act - Trigger event (would normally happen after successful data load)
        // Note: This tests that the event exists and can be subscribed to

        // Assert
        eventFired.Should().BeFalse("Event should not fire without actual data loading");
        
        // Verify event can be subscribed to without errors
        Action subscribeAction = () => _masterDataService.MasterDataLoaded += (s, e) => { };
        subscribeAction.Should().NotThrow("Should be able to subscribe to MasterDataLoaded event");
    }

    #endregion

    #region Refresh Methods Tests

    [Test]
    public async Task MasterDataService_RefreshMethods_ShouldExistAndNotThrow()
    {
        // Act & Assert - All refresh methods should exist and not throw
        Func<Task> refreshPartIds = async () => await _masterDataService.RefreshPartIdsAsync();
        Func<Task> refreshOperations = async () => await _masterDataService.RefreshOperationsAsync();
        Func<Task> refreshLocations = async () => await _masterDataService.RefreshLocationsAsync();
        Func<Task> refreshUsers = async () => await _masterDataService.RefreshUsersAsync();

        await refreshPartIds.Should().NotThrowAsync("RefreshPartIdsAsync should not throw");
        await refreshOperations.Should().NotThrowAsync("RefreshOperationsAsync should not throw");
        await refreshLocations.Should().NotThrowAsync("RefreshLocationsAsync should not throw");
        await refreshUsers.Should().NotThrowAsync("RefreshUsersAsync should not throw");
    }

    #endregion
}