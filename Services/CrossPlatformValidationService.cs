using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public interface ICrossPlatformValidationService
{
    Task<bool> IsWindowsAsync();
}

public class CrossPlatformValidationService : ICrossPlatformValidationService
{
    private readonly ILogger<CrossPlatformValidationService> _logger;

    public CrossPlatformValidationService(ILogger<CrossPlatformValidationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsWindowsAsync()
    {
        var isWindows = OperatingSystem.IsWindows();
        _logger.LogDebug("IsWindows: {IsWindows}", isWindows);
        return Task.FromResult(isWindows);
    }
}
