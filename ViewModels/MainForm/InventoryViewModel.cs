using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class InventoryViewModel : BaseViewModel
{
    private readonly MTM_Shared_Logic.Services.IInventoryService _inventoryService;
    private readonly IApplicationStateService _applicationState;

    public InventoryViewModel(
        MTM_Shared_Logic.Services.IInventoryService inventoryService,
        IApplicationStateService applicationState,
        ILogger<InventoryViewModel> logger) : base(logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("InventoryViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}