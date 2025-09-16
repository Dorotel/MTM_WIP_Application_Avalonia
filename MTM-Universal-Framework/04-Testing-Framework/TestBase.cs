using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace MTM.Universal.Testing
{
    /// <summary>
    /// Base class for all unit tests providing common testing infrastructure.
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        protected IServiceProvider ServiceProvider { get; private set; } = null!;
        protected ILogger Logger { get; private set; } = null!;
        private bool _disposed;

        [OneTimeSetUp]
        public virtual async Task OneTimeSetUpAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
            Logger = ServiceProvider.GetRequiredService<ILogger<TestBase>>();
            
            await InitializeAsync();
        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDownAsync()
        {
            await CleanupAsync();
            ServiceProvider?.Dispose();
        }

        [SetUp]
        public virtual async Task SetUpAsync()
        {
            await BeforeEachTestAsync();
        }

        [TearDown]
        public virtual async Task TearDownAsync()
        {
            await AfterEachTestAsync();
        }

        /// <summary>
        /// Configure services for testing. Override in derived classes.
        /// </summary>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
            // Configure logging
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        }

        /// <summary>
        /// Initialize test environment. Override in derived classes.
        /// </summary>
        protected virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Cleanup test environment. Override in derived classes.
        /// </summary>
        protected virtual Task CleanupAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Setup before each test. Override in derived classes.
        /// </summary>
        protected virtual Task BeforeEachTestAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Cleanup after each test. Override in derived classes.
        /// </summary>
        protected virtual Task AfterEachTestAsync()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                ServiceProvider?.Dispose();
                _disposed = true;
            }
        }
    }
}