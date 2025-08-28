using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class TransactionHistoryViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;

    public TransactionHistoryViewModel(
        IApplicationStateService applicationState,
        ILogger<TransactionHistoryViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("TransactionHistoryViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}
