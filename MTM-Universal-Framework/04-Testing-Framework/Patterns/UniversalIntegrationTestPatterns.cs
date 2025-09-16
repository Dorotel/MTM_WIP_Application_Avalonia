using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using System.Data;

namespace MTM.UniversalFramework.Testing.Patterns;

/// <summary>
/// Comprehensive integration testing patterns for cross-service communication,
/// database operations, and external system integration.
/// </summary>
public static class UniversalIntegrationTestPatterns
{
    /// <summary>
    /// Base class for integration tests with full service provider setup.
    /// </summary>
    public abstract class IntegrationTestBase
    {
        protected IServiceProvider ServiceProvider { get; private set; }
        protected IServiceScope TestScope { get; private set; }

        [OneTimeSetUp]
        public virtual async Task OneTimeSetUp()
        {
            var services = new ServiceCollection();
            await ConfigureServicesAsync(services);
            ServiceProvider = services.BuildServiceProvider();
            
            await InitializeTestEnvironmentAsync();
        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDown()
        {
            await CleanupTestEnvironmentAsync();
            
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }

        [SetUp]
        public virtual async Task SetUp()
        {
            TestScope = ServiceProvider.CreateScope();
            await SetupTestDataAsync();
        }

        [TearDown]
        public virtual async Task TearDown()
        {
            await CleanupTestDataAsync();
            TestScope?.Dispose();
        }

        protected abstract Task ConfigureServicesAsync(IServiceCollection services);
        protected virtual async Task InitializeTestEnvironmentAsync() => await Task.CompletedTask;
        protected virtual async Task CleanupTestEnvironmentAsync() => await Task.CompletedTask;
        protected virtual async Task SetupTestDataAsync() => await Task.CompletedTask;
        protected virtual async Task CleanupTestDataAsync() => await Task.CompletedTask;

        /// <summary>
        /// Gets a service from the test scope.
        /// </summary>
        protected T GetService<T>() where T : notnull
        {
            return TestScope.ServiceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Validates end-to-end workflow execution.
        /// </summary>
        protected async Task AssertWorkflowExecution<TInput, TOutput>(
            Func<TInput, Task<TOutput>> workflow,
            TInput input,
            Func<TOutput, bool> resultValidator,
            string workflowName = "Workflow")
        {
            // Execute workflow
            var result = await workflow(input);

            // Validate result
            Assert.That(result, Is.Not.Null, $"{workflowName} should return a result");
            Assert.That(resultValidator(result), Is.True, 
                $"{workflowName} result should pass validation");
        }
    }

    /// <summary>
    /// Database integration testing patterns for multiple providers.
    /// </summary>
    public abstract class DatabaseIntegrationTestBase : IntegrationTestBase
    {
        protected string ConnectionString { get; private set; }
        protected string TestDatabaseName { get; private set; }

        [OneTimeSetUp]
        public override async Task OneTimeSetUp()
        {
            TestDatabaseName = $"TestDB_{Guid.NewGuid():N}";
            ConnectionString = GetTestConnectionString();
            
            await CreateTestDatabaseAsync();
            await base.OneTimeSetUp();
        }

        [OneTimeTearDown]
        public override async Task OneTimeTearDown()
        {
            await base.OneTimeTearDown();
            await DropTestDatabaseAsync();
        }

        protected abstract string GetTestConnectionString();
        protected abstract Task CreateTestDatabaseAsync();
        protected abstract Task DropTestDatabaseAsync();

        /// <summary>
        /// Validates database transaction integrity.
        /// </summary>
        protected async Task AssertTransactionIntegrity(
            Func<Task> transactionOperation,
            Func<Task<bool>> dataVerification,
            bool shouldCommit = true)
        {
            // Execute transaction
            if (shouldCommit)
            {
                await transactionOperation();
                
                // Verify data was committed
                var dataExists = await dataVerification();
                Assert.That(dataExists, Is.True, "Transaction data should be committed");
            }
            else
            {
                // Transaction should rollback on exception
                Assert.ThrowsAsync<Exception>(async () => await transactionOperation(),
                    "Transaction should throw exception and rollback");
                
                // Verify data was not committed
                var dataExists = await dataVerification();
                Assert.That(dataExists, Is.False, "Transaction data should not be committed after rollback");
            }
        }

        /// <summary>
        /// Validates database performance under load.
        /// </summary>
        protected async Task AssertDatabasePerformance(
            Func<Task> databaseOperation,
            int concurrentOperations = 10,
            TimeSpan maxExecutionTime = default)
        {
            if (maxExecutionTime == default)
                maxExecutionTime = TimeSpan.FromSeconds(30);

            var tasks = Enumerable.Range(0, concurrentOperations)
                .Select(_ => Task.Run(databaseOperation))
                .ToArray();

            var startTime = DateTime.UtcNow;
            await Task.WhenAll(tasks);
            var totalTime = DateTime.UtcNow - startTime;

            Assert.That(totalTime, Is.LessThan(maxExecutionTime),
                $"Database operations took {totalTime.TotalSeconds:F2}s, " +
                $"which exceeds maximum allowed time of {maxExecutionTime.TotalSeconds:F2}s");
        }
    }

    /// <summary>
    /// Service communication integration testing patterns.
    /// </summary>
    public abstract class ServiceCommunicationTestBase : IntegrationTestBase
    {
        /// <summary>
        /// Validates service-to-service communication patterns.
        /// </summary>
        protected async Task AssertServiceCommunication<TService1, TService2, TMessage>(
            Func<TService1, Task> serviceAction,
            Func<TService2, Task<TMessage>> messageReceiver,
            Func<TMessage, bool> messageValidator,
            TimeSpan timeout = default) where TService1 : notnull where TService2 : notnull
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(10);

            var service1 = GetService<TService1>();
            var service2 = GetService<TService2>();

            // Set up message reception
            var messageReceived = false;
            var receivedMessage = default(TMessage);

            var receiveTask = Task.Run(async () =>
            {
                var endTime = DateTime.UtcNow.Add(timeout);
                while (DateTime.UtcNow < endTime && !messageReceived)
                {
                    try
                    {
                        receivedMessage = await messageReceiver(service2);
                        if (receivedMessage != null)
                        {
                            messageReceived = true;
                        }
                    }
                    catch
                    {
                        // Continue polling
                    }

                    if (!messageReceived)
                        await Task.Delay(100);
                }
            });

            // Trigger service action
            await serviceAction(service1);

            // Wait for message reception
            await receiveTask;

            // Validate message
            Assert.That(messageReceived, Is.True, "Message should be received within timeout period");
            Assert.That(messageValidator(receivedMessage), Is.True, "Received message should pass validation");
        }

        /// <summary>
        /// Tests service circuit breaker patterns.
        /// </summary>
        protected async Task AssertCircuitBreakerBehavior<TService>(
            Func<TService, Task> serviceOperation,
            int failureThreshold = 3,
            TimeSpan circuitOpenTime = default) where TService : notnull
        {
            if (circuitOpenTime == default)
                circuitOpenTime = TimeSpan.FromSeconds(30);

            var service = GetService<TService>();

            // Trigger failures to open circuit
            for (int i = 0; i < failureThreshold; i++)
            {
                Assert.ThrowsAsync<Exception>(async () => await serviceOperation(service),
                    $"Service operation should fail on attempt {i + 1}");
            }

            // Circuit should now be open - operations should fail fast
            var startTime = DateTime.UtcNow;
            Assert.ThrowsAsync<Exception>(async () => await serviceOperation(service),
                "Service operation should fail fast when circuit is open");
            var fastFailTime = DateTime.UtcNow - startTime;

            Assert.That(fastFailTime, Is.LessThan(TimeSpan.FromMilliseconds(100)),
                "Circuit breaker should fail fast when open");

            // Wait for circuit to close (in real implementation, would need to wait for circuit open time)
            // This would need to be implemented based on the specific circuit breaker implementation
        }
    }

    /// <summary>
    /// External system integration testing patterns.
    /// </summary>
    public abstract class ExternalSystemTestBase : IntegrationTestBase
    {
        protected Mock<IExternalSystemClient> MockExternalClient { get; private set; }

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            MockExternalClient = new Mock<IExternalSystemClient>();
            ConfigureMockExternalClient();
        }

        protected abstract void ConfigureMockExternalClient();

        /// <summary>
        /// Tests external system integration with retry logic.
        /// </summary>
        protected async Task AssertExternalSystemIntegration<TRequest, TResponse>(
            Func<TRequest, Task<TResponse>> integrationOperation,
            TRequest request,
            TResponse expectedResponse,
            int maxRetries = 3)
        {
            var attemptCount = 0;

            MockExternalClient.Setup(client => client.SendAsync<TRequest, TResponse>(
                It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns<TRequest, CancellationToken>((req, ct) =>
                {
                    attemptCount++;
                    
                    // Simulate failures for first few attempts
                    if (attemptCount <= maxRetries - 1)
                    {
                        throw new Exception($"External system failure attempt {attemptCount}");
                    }
                    
                    return Task.FromResult(expectedResponse);
                });

            // Execute integration operation
            var result = await integrationOperation(request);

            // Validate retry behavior
            Assert.That(attemptCount, Is.EqualTo(maxRetries), 
                $"Should have retried {maxRetries} times");
            Assert.That(result, Is.EqualTo(expectedResponse), 
                "Should return expected response after retries");
        }

        /// <summary>
        /// Tests timeout and fallback behavior.
        /// </summary>
        protected async Task AssertTimeoutAndFallback<TRequest, TResponse>(
            Func<TRequest, Task<TResponse>> integrationOperation,
            TRequest request,
            TResponse fallbackResponse,
            TimeSpan timeout = default)
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(5);

            MockExternalClient.Setup(client => client.SendAsync<TRequest, TResponse>(
                It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
                .Returns<TRequest, CancellationToken>(async (req, ct) =>
                {
                    // Simulate timeout
                    await Task.Delay(timeout.Add(TimeSpan.FromSeconds(1)), ct);
                    return default(TResponse);
                });

            // Execute integration operation
            var result = await integrationOperation(request);

            // Should return fallback response on timeout
            Assert.That(result, Is.EqualTo(fallbackResponse), 
                "Should return fallback response on timeout");
        }
    }

    /// <summary>
    /// Event sourcing and messaging integration patterns.
    /// </summary>
    public abstract class EventSourcingTestBase : IntegrationTestBase
    {
        protected List<object> PublishedEvents { get; private set; } = new();

        [SetUp]
        public override async Task SetUp()
        {
            await base.SetUp();
            PublishedEvents.Clear();
        }

        /// <summary>
        /// Validates event publication and consumption.
        /// </summary>
        protected async Task AssertEventFlow<TEvent>(
            Func<Task> triggerAction,
            Func<TEvent, bool> eventValidator,
            TimeSpan timeout = default) where TEvent : class
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(10);

            var eventReceived = false;
            TEvent receivedEvent = null;

            // Set up event monitoring
            var endTime = DateTime.UtcNow.Add(timeout);
            var monitorTask = Task.Run(async () =>
            {
                while (DateTime.UtcNow < endTime && !eventReceived)
                {
                    var events = PublishedEvents.OfType<TEvent>().ToList();
                    if (events.Any())
                    {
                        receivedEvent = events.First();
                        eventReceived = true;
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }
            });

            // Trigger action that should publish event
            await triggerAction();

            // Wait for event
            await monitorTask;

            // Validate event
            Assert.That(eventReceived, Is.True, "Event should be published within timeout");
            Assert.That(eventValidator(receivedEvent), Is.True, "Published event should pass validation");
        }

        /// <summary>
        /// Tests event replay and projection building.
        /// </summary>
        protected async Task AssertEventReplay<TAggregate>(
            IEnumerable<object> events,
            Func<IEnumerable<object>, Task<TAggregate>> replayFunction,
            Func<TAggregate, bool> aggregateValidator)
        {
            // Replay events to build aggregate
            var aggregate = await replayFunction(events);

            // Validate resulting aggregate
            Assert.That(aggregate, Is.Not.Null, "Event replay should produce an aggregate");
            Assert.That(aggregateValidator(aggregate), Is.True, "Replayed aggregate should pass validation");
        }
    }

    /// <summary>
    /// Interface for external system client (mock target).
    /// </summary>
    public interface IExternalSystemClient
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);
    }
}