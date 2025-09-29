using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IGSCOrchestrationService
{
    Task<bool> ValidateWorkflowAsync();
}

public class GSCOrchestrationService : IGSCOrchestrationService
{
    private readonly ILogger<GSCOrchestrationService> _logger;

    public GSCOrchestrationService(ILogger<GSCOrchestrationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> ValidateWorkflowAsync()
    {
        _logger.LogInformation("GSC workflow validation stub");
        return Task.FromResult(true);
    }
}
