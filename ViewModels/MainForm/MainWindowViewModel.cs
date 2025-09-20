using System;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
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
        Logger.LogDebug("MainWindowViewModel constructor started");
        
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

        Logger.LogInformation("MainWindowViewModel initialized with dependency injection - Services injected successfully");
        Logger.LogDebug("NavigationService type: {NavigationServiceType}", _navigationService.GetType().FullName);
        Logger.LogDebug("ApplicationStateService type: {ApplicationStateType}", _applicationState.GetType().FullName);

        // Wire up navigation service to update CurrentView when navigation occurs
        Logger.LogDebug("Subscribing to NavigationService.Navigated event");
        _navigationService.Navigated += OnNavigated;
        Logger.LogDebug("Successfully subscribed to NavigationService.Navigated event");

        // Set MainView as the current content - defer creation to avoid service locator pattern during startup validation
        Logger.LogDebug("Deferring MainView creation to avoid service provider dependency during startup validation");
        // CurrentView will be set after full application startup via navigation or manual assignment
        
        Logger.LogDebug("MainWindowViewModel constructor completed successfully");
    }

    /// <summary>
    /// Manually initialize the MainView after application startup is complete.
    /// This avoids the service locator pattern during startup validation.
    /// </summary>
    public void InitializeMainView()
    {
        if (CurrentView != null)
        {
            Logger.LogDebug("MainView already initialized, skipping");
            return;
        }

        Logger.LogDebug("Manually initializing MainView after startup");
        try
        {
            var mainViewViewModel = Program.GetService<MainViewViewModel>();
            Logger.LogDebug("MainViewViewModel resolved successfully for manual initialization, type: {MainViewViewModelType}", mainViewViewModel.GetType().FullName);
            
            CurrentView = new MainView
            {
                DataContext = mainViewViewModel
            };
            
            Logger.LogInformation("MainView manually initialized and set as CurrentView");
            
            // Request startup focus after MainView is fully initialized
            Logger.LogDebug("Requesting startup focus after MainView initialization");
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
        Logger.LogDebug("OnNavigated event handler triggered - Sender: {SenderType}, Target: {TargetType}", 
            sender?.GetType().Name ?? "null", e.Target?.GetType().Name ?? "null");
        
        try
        {
            var previousView = CurrentView?.GetType().Name ?? "null";
            CurrentView = e.Target;
            var newView = e.Target?.GetType().Name ?? "null";
            
            Logger.LogInformation("MainWindow navigation completed - Previous: {PreviousView}, New: {NewView}", previousView, newView);
            Logger.LogDebug("Navigation event args - Target: {Target}, Source: {Source}", e.Target, e.Source);
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
            Logger.LogDebug("MainWindowViewModel disposing - Unsubscribing from events");
            _navigationService.Navigated -= OnNavigated;
            Logger.LogDebug("Successfully unsubscribed from NavigationService.Navigated event");
        }
        base.Dispose(disposing);
    }
}
