using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class AddItemViewModel : BaseViewModel
{
    private readonly IApplicationStateService _applicationState;

    public AddItemViewModel(
        IApplicationStateService applicationState,
        ILogger<AddItemViewModel> logger) : base(logger)
    {
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("AddItemViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize commands with injected services
    }
}
