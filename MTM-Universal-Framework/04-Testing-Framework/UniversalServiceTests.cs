using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MTM.UniversalFramework.Testing
{
    /// <summary>
    /// Universal Service test patterns for testing service layer components.
    /// Provides common testing utilities for any business domain services.
    /// </summary>
    public abstract class UniversalServiceTestBase<TService> : UniversalTestBase
        where TService : class
    {
        protected TService Service { get; private set; }
        protected Mock<TService> MockService { get; private set; }

        protected override async Task SetupAsync()
        {
            await base.SetupAsync();
            Service = CreateService();
            MockService = new Mock<TService>();
        }

        /// <summary>
        /// Override this method to create the Service instance with required dependencies
        /// </summary>
        protected abstract TService CreateService();

        /// <summary>
        /// Tests that service methods handle null parameters correctly
        /// </summary>
        protected async Task TestNullParameterHandlingAsync<TParam, TResult>(
            Func<TService, TParam, Task<TResult>> serviceMethod,
            string parameterName)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => serviceMethod(Service, default(TParam)));
            
            Assert.Contains(parameterName, exception.ParamName);
        }

        /// <summary>
        /// Tests that service methods return proper error results for invalid input
        /// </summary>
        protected async Task TestInvalidInputHandlingAsync<TParam, TResult>(
            Func<TService, TParam, Task<ServiceResult<TResult>>> serviceMethod,
            TParam invalidParam,
            string expectedErrorMessage = null)
        {
            // Act
            var result = await serviceMethod(Service, invalidParam);

            // Assert
            AssertServiceResult(result, shouldSucceed: false, expectedErrorMessage);
        }

        /// <summary>
        /// Tests that service methods return proper success results for valid input
        /// </summary>
        protected async Task TestValidInputHandlingAsync<TParam, TResult>(
            Func<TService, TParam, Task<ServiceResult<TResult>>> serviceMethod,
            TParam validParam,
            string expectedMessage = null)
        {
            // Act
            var result = await serviceMethod(Service, validParam);

            // Assert
            AssertServiceResult(result, shouldSucceed: true, expectedMessage);
        }

        /// <summary>
        /// Tests service error handling and logging
        /// </summary>
        protected async Task TestErrorHandlingAsync<TParam, TResult>(
            Func<TService, TParam, Task<ServiceResult<TResult>>> serviceMethod,
            TParam param,
            Exception simulatedException)
        {
            // This would require setting up the service to throw the exception
            // Implementation depends on specific service mocking strategy
            await Task.CompletedTask;
        }

        protected override async Task TearDownAsync()
        {
            await base.TearDownAsync();
            Service = null;
            MockService = null;
        }
    }

    /// <summary>
    /// Sample Service test implementation showing how to use the base class
    /// </summary>
    public class SampleServiceTests : UniversalServiceTestBase<ISampleService>
    {
        private Mock<ILogger<SampleService>> _mockLogger;

        protected override void ConfigureTestServices()
        {
            base.ConfigureTestServices();
            _mockLogger = CreateMockService<ILogger<SampleService>>();
        }

        protected override ISampleService CreateService()
        {
            return new SampleService(_mockLogger.Object, Configuration);
        }

        [Fact]
        public async Task GetDataAsync_ValidId_ShouldReturnSuccess()
        {
            await TestValidInputHandlingAsync(
                (service, id) => service.GetDataAsync(id),
                validParam: 1,
                expectedMessage: "Data retrieved successfully"
            );
        }

        [Fact]
        public async Task GetDataAsync_InvalidId_ShouldReturnError()
        {
            await TestInvalidInputHandlingAsync(
                (service, id) => service.GetDataAsync(id),
                invalidParam: -1,
                expectedErrorMessage: "Invalid ID"
            );
        }

        [Fact]
        public async Task SaveDataAsync_NullData_ShouldThrowArgumentNullException()
        {
            await TestNullParameterHandlingAsync<object, bool>(
                async (service, data) => (await service.SaveDataAsync(data)).IsSuccess,
                parameterName: "data"
            );
        }
    }

    /// <summary>
    /// Sample Service interface for demonstration purposes
    /// </summary>
    public interface ISampleService
    {
        Task<ServiceResult<string>> GetDataAsync(int id);
        Task<ServiceResult<bool>> SaveDataAsync(object data);
    }

    /// <summary>
    /// Sample Service implementation for demonstration purposes
    /// </summary>
    public class SampleService : ISampleService
    {
        private readonly ILogger<SampleService> _logger;
        private readonly IConfiguration _configuration;

        public SampleService(ILogger<SampleService> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<ServiceResult<string>> GetDataAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID provided: {Id}", id);
                return ServiceResult<string>.Failure("Invalid ID");
            }

            await Task.Delay(10); // Simulate async work

            _logger.LogInformation("Data retrieved for ID: {Id}", id);
            return ServiceResult<string>.Success($"Data for ID: {id}", "Data retrieved successfully");
        }

        public async Task<ServiceResult<bool>> SaveDataAsync(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await Task.Delay(10); // Simulate async work

            _logger.LogInformation("Data saved successfully");
            return ServiceResult<bool>.Success(true, "Data saved successfully");
        }
    }
}