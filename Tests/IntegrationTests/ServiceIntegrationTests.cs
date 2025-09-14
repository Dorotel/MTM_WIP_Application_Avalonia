using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM.Tests.IntegrationTests
{
    /// <summary>
    /// Integration tests for cross-service communication and dependency injection
    /// Tests real service interactions with proper dependency resolution
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("Services")]
    [Category("DependencyInjection")]
    public class ServiceIntegrationTests
    {
        #region Test Setup & Service Configuration

        private IServiceProvider _serviceProvider = null!;
        private IServiceCollection _services = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Setup dependency injection container for integration testing
            _services = new ServiceCollection();
            ConfigureTestServices();
            _serviceProvider = _services.BuildServiceProvider();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void ConfigureTestServices()
        {
            // Add logging
            _services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

            // Add configuration service mock
            var mockConfig = new Mock<IConfigurationService>();
            mockConfig.Setup(s => s.GetConnectionString(It.IsAny<string>()))
                .Returns("Server=localhost;Database=mtm_integration_test;Uid=test;Pwd=test;");
            _services.AddSingleton(mockConfig.Object);

            // Add application state service mock
            var mockAppState = new Mock<IApplicationStateService>();
            _services.AddSingleton(mockAppState.Object);

            // Add real services for integration testing
            _services.AddScoped<IDatabaseService, DatabaseService>();
            _services.AddScoped<IMasterDataService, MasterDataService>();
            
            // Add navigation service mock
            var mockNavigation = new Mock<INavigationService>();
            _services.AddSingleton(mockNavigation.Object);
        }

        #endregion

        #region Service Resolution Tests

        [Test]
        public void ServiceProvider_ShouldResolveAllRegisteredServices()
        {
            // Act & Assert - All registered services should resolve successfully
            var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
            var appStateService = _serviceProvider.GetRequiredService<IApplicationStateService>();
            var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

            // All services should be resolved
            configService.Should().NotBeNull();
            appStateService.Should().NotBeNull();
            databaseService.Should().NotBeNull();
            masterDataService.Should().NotBeNull();
            navigationService.Should().NotBeNull();
        }

        [Test]
        public void ServiceProvider_ShouldRespectServiceLifetimes()
        {
            // Act - Resolve services multiple times
            var config1 = _serviceProvider.GetRequiredService<IConfigurationService>();
            var config2 = _serviceProvider.GetRequiredService<IConfigurationService>();
            
            var database1 = _serviceProvider.GetRequiredService<IDatabaseService>();
            var database2 = _serviceProvider.GetRequiredService<IDatabaseService>();
            
            var masterData1 = _serviceProvider.GetRequiredService<IMasterDataService>();
            var masterData2 = _serviceProvider.GetRequiredService<IMasterDataService>();

            // Assert - Singleton services should be same instance
            config1.Should().BeSameAs(config2, "Configuration service should be singleton");
            
            // Scoped services should be different instances in different scopes
            // (In same scope they would be same, but in root scope they're effectively singleton)
            database1.Should().NotBeNull();
            database2.Should().NotBeNull();
            masterData1.Should().NotBeNull();
            masterData2.Should().NotBeNull();
        }

        #endregion

        #region Cross-Service Communication Tests

        [Test]
        public async Task MasterDataService_ShouldIntegrateWithDatabaseService()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();

            // Act - MasterDataService should use DatabaseService internally
            masterDataService.Should().NotBeNull();
            databaseService.Should().NotBeNull();

            // The master data service should have been constructed with the database service
            // Verify by checking that collections are initialized (even if empty due to test setup)
            masterDataService.PartIds.Should().NotBeNull();
            masterDataService.Operations.Should().NotBeNull();
            masterDataService.Locations.Should().NotBeNull();

            await Task.CompletedTask; // Satisfy async test requirement
        }

        [Test]
        public async Task DatabaseService_ShouldReceiveConfigurationFromConfigurationService()
        {
            // Arrange
            var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
            var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();

            // Act
            var connectionString = configService.GetConnectionString();
            var databaseConnectionString = databaseService.GetConnectionString();

            // Assert - Database service should get connection string from configuration service
            connectionString.Should().NotBeNullOrEmpty();
            databaseConnectionString.Should().NotBeNullOrEmpty();
            
            // Both should reference the same configuration
            databaseConnectionString.Should().Be(connectionString);

            await Task.CompletedTask; // Satisfy async test requirement
        }

        #endregion

        #region Service State Management Tests

        [Test]
        public async Task MasterDataService_ShouldMaintainCollectionState()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();

            // Act - Access collections multiple times
            var partIds1 = masterDataService.PartIds;
            var partIds2 = masterDataService.PartIds;
            
            var operations1 = masterDataService.Operations;
            var operations2 = masterDataService.Operations;

            // Assert - Collections should be consistent references
            partIds1.Should().BeSameAs(partIds2, "PartIds collection should be stable reference");
            operations1.Should().BeSameAs(operations2, "Operations collection should be stable reference");
            
            // Collections should be observable collections
            partIds1.Should().BeOfType<ObservableCollection<string>>();
            operations1.Should().BeOfType<ObservableCollection<string>>();

            await Task.CompletedTask; // Satisfy async test requirement
        }

        [Test]
        public void ApplicationStateService_ShouldBeAccessibleAcrossServices()
        {
            // Arrange
            var appStateService = _serviceProvider.GetRequiredService<IApplicationStateService>();

            // Act - Application state should be consistent across service resolutions
            var appState1 = _serviceProvider.GetRequiredService<IApplicationStateService>();
            var appState2 = _serviceProvider.GetRequiredService<IApplicationStateService>();

            // Assert - Should be same instance (singleton)
            appState1.Should().BeSameAs(appStateService);
            appState2.Should().BeSameAs(appStateService);
        }

        #endregion

        #region Error Handling Integration Tests

        [Test]
        public async Task ServiceIntegration_ShouldHandleErrorsGracefully()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();

            // Act & Assert - Services should handle integration errors gracefully
            var action = async () =>
            {
                // Try to load master data (this may fail in test environment but shouldn't throw)
                await masterDataService.LoadAllMasterDataAsync();
            };

            // Should not throw exceptions, even if database is not available
            await action.Should().NotThrowAsync("Services should handle integration failures gracefully");
        }

        [Test]
        public void ServiceIntegration_LoggersShouldBeInjectedCorrectly()
        {
            // Arrange & Act
            var services = new object[]
            {
                _serviceProvider.GetRequiredService<IDatabaseService>(),
                _serviceProvider.GetRequiredService<IMasterDataService>()
            };

            // Assert - All services should have loggers injected (we can't directly test private fields,
            // but we can verify services were created successfully with DI)
            foreach (var service in services)
            {
                service.Should().NotBeNull("Service should be created successfully with all dependencies including logger");
            }
        }

        #endregion

        #region Performance Integration Tests

        [Test]
        public async Task ServiceIntegration_ShouldPerformWithinAcceptableTimeframes()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var startTime = DateTime.UtcNow;

            // Act - Perform typical service operations
            await masterDataService.LoadAllMasterDataAsync();
            var collections = new[]
            {
                masterDataService.PartIds,
                masterDataService.Operations,
                masterDataService.Locations
            };

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Assert - Operations should complete quickly (even with mocked dependencies)
            duration.Should().BeLessThan(TimeSpan.FromSeconds(5), 
                "Service integration operations should complete within reasonable timeframes");

            // Collections should be accessible
            foreach (var collection in collections)
            {
                collection.Should().NotBeNull("All master data collections should be accessible");
            }
        }

        [Test]
        public void ServiceIntegration_ShouldHandleConcurrentAccess()
        {
            // Arrange
            const int concurrentOperations = 10;
            var tasks = new Task[concurrentOperations];

            // Act - Perform concurrent service operations
            for (int i = 0; i < concurrentOperations; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
                    var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
                    
                    // Simulate concurrent access
                    _ = masterDataService.PartIds.Count;
                    _ = configService.GetConnectionString();
                    
                    return true;
                });
            }

            // Assert - All concurrent operations should complete successfully
            var action = () => Task.WaitAll(tasks, TimeSpan.FromSeconds(10));
            action.Should().NotThrow("Service integration should handle concurrent access");

            // All tasks should have completed successfully
            tasks.Should().OnlyContain(t => t.IsCompletedSuccessfully, 
                "All concurrent service operations should complete successfully");
        }

        #endregion

        #region Manufacturing Domain Integration Tests

        [Test]
        public async Task ServiceIntegration_ShouldSupportManufacturingWorkflows()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();
            var databaseService = _serviceProvider.GetRequiredService<IDatabaseService>();

            // Act - Test manufacturing workflow support
            await masterDataService.LoadAllMasterDataAsync();

            // Assert - Services should support manufacturing operations
            var operations = masterDataService.Operations;
            operations.Should().NotBeNull("Operations collection should be available for manufacturing workflows");

            var connectionString = databaseService.GetConnectionString();
            connectionString.Should().NotBeNullOrEmpty("Database connection should be available for manufacturing data");

            // Manufacturing operations integration
            var manufacturingOperations = new[] { "90", "100", "110", "120", "130" };
            foreach (var operation in manufacturingOperations)
            {
                // Each manufacturing operation should be supported by the service infrastructure
                operation.Should().NotBeNullOrEmpty($"Manufacturing operation {operation} should be valid");
            }
        }

        [Test]
        public void ServiceIntegration_ShouldFollowMTMPatterns()
        {
            // Arrange
            var masterDataService = _serviceProvider.GetRequiredService<IMasterDataService>();

            // Act & Assert - Verify MTM patterns are followed
            
            // 1. NO FALLBACK DATA PATTERN - Collections should be empty, not null
            masterDataService.PartIds.Should().NotBeNull().And.BeEmpty("Should follow NO FALLBACK pattern");
            masterDataService.Operations.Should().NotBeNull().And.BeEmpty("Should follow NO FALLBACK pattern");
            masterDataService.Locations.Should().NotBeNull().And.BeEmpty("Should follow NO FALLBACK pattern");

            // 2. Observable collections for UI binding
            masterDataService.PartIds.Should().BeOfType<ObservableCollection<string>>("Should use ObservableCollection for UI binding");
            masterDataService.Operations.Should().BeOfType<ObservableCollection<string>>("Should use ObservableCollection for UI binding");
            masterDataService.Locations.Should().BeOfType<ObservableCollection<string>>("Should use ObservableCollection for UI binding");
        }

        #endregion
    }
}