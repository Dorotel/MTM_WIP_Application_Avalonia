using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class AddItemViewModel : BaseViewModel
{
    private readonly MTM.Services.IInventoryService _inventoryService;
    private readonly IApplicationStateService _applicationState;

    public AddItemViewModel(
        MTM.Services.IInventoryService inventoryService,
        IApplicationStateService applicationState,
        ILogger<AddItemViewModel> logger) : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("AddItemViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}