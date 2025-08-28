using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

public class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;

    public MainViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        ILogger<MainViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("MainViewModel initialized with dependency injection");

        // Initialize commands and setup
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize commands with injected services
    }
}