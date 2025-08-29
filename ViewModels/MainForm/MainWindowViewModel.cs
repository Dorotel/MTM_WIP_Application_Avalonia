using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Commands;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Views;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;

    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public MainWindowViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        ILogger<MainWindowViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("MainWindowViewModel initialized with dependency injection");

        // Wire up navigation service to update CurrentView when navigation occurs
        _navigationService.Navigated += OnNavigated;

        // Set MainView as the current content - TODO: Get this from DI container
        CurrentView = new MainView
        {
            // TODO: DataContext should be injected MainViewViewModel
            DataContext = Program.GetService<MainViewViewModel>()
        };
    }

    private void OnNavigated(object? sender, Services.NavigationEventArgs e)
    {
        CurrentView = e.Target;
        Logger.LogInformation("MainWindow navigated to: {ViewType}", e.Target?.GetType().Name ?? "null");
    }
}
