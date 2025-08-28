using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class UserManagementViewModel : BaseViewModel
{
    private readonly MTM_Shared_Logic.Services.IUserService _userService;
    private readonly IApplicationStateService _applicationState;

    public UserManagementViewModel(
        MTM_Shared_Logic.Services.IUserService userService,
        IApplicationStateService applicationState,
        ILogger<UserManagementViewModel> logger) : base(logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("UserManagementViewModel initialized with dependency injection");

        // TODO: Initialize commands and properties
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        // TODO: Initialize ReactiveCommands with injected services
    }
}