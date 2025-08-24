using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

public class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly MTM.Services.IInventoryService _inventoryService;

    public MainViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        MTM.Services.IInventoryService inventoryService,
        ILogger<MainViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));

        Logger.LogInformation("MainViewModel initialized with dependency injection");

        // Initialize commands and setup
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}