using System;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;

    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainWindowViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        ILogger<MainWindowViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("MainWindowViewModel initialized with dependency injection");

        // Set MainView as the current content - TODO: Get this from DI container
        CurrentView = new Views.MainView
        {
            // TODO: DataContext should be injected MainViewViewModel
            DataContext = Program.GetService<MainViewViewModel>()
        };
    }
}