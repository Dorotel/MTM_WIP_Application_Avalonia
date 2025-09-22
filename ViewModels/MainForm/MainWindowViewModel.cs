using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
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

        Logger.LogInformation("MainWindowViewModel initialized with dependency injection - Services injected successfully");

        // Wire up navigation service to update CurrentView when navigation occurs
        _navigationService.Navigated += OnNavigated;

        // Set MainView as the current content - defer creation to avoid service locator pattern during startup validation
        // CurrentView will be set after full application startup via navigation or manual assignment

    }

    /// <summary>
    /// Manually initialize the MainView after application startup is complete.
    /// This avoids the service locator pattern during startup validation.
    /// </summary>
    public void InitializeMainView()
    {
        if (CurrentView != null)
        {
            return;
        }

        try
        {
            var mainViewViewModel = Program.GetService<MainViewViewModel>();

            CurrentView = new MainView
            {
                DataContext = mainViewViewModel
            };

            Logger.LogInformation("MainView manually initialized and set as CurrentView");

            // Request startup focus after MainView is fully initialized
            mainViewViewModel.RequestStartupFocus();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to manually initialize MainView");
            throw;
        }
    }

    private void OnNavigated(object? sender, Services.NavigationEventArgs e)
    {

        try
        {
            var previousView = CurrentView?.GetType().Name ?? "null";
            CurrentView = e.Target;
            var newView = e.Target?.GetType().Name ?? "null";

            Logger.LogInformation("MainWindow navigation completed - Previous: {PreviousView}, New: {NewView}", previousView, newView);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in OnNavigated event handler");
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _navigationService.Navigated -= OnNavigated;
        }
        base.Dispose(disposing);
    }
}
