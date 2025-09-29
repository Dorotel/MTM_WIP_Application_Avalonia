using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

public interface IWorkflowStateService
{
    Task<string> GetCurrentPhaseAsync();
}

public class WorkflowStateService : IWorkflowStateService
{
    private readonly ILogger<WorkflowStateService> _logger;
    private const string StatePath = ".specify/state/gsc-workflow.json";

    public WorkflowStateService(ILogger<WorkflowStateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<string> GetCurrentPhaseAsync()
    {
        try
        {
            if (File.Exists(StatePath))
            {
                var json = File.ReadAllText(StatePath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("currentPhase", out var phase))
                {
                    return Task.FromResult(phase.GetString() ?? "not_started");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read workflow state");
        }
        return Task.FromResult("not_started");
    }
}
