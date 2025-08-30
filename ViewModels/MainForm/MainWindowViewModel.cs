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

        // Set MainView as the current content - TODO: Get this from DI container
        Logger.LogDebug("Creating MainView with injected MainViewViewModel");
        try
        {
            var mainViewViewModel = Program.GetService<MainViewViewModel>();
            Logger.LogDebug("MainViewViewModel resolved successfully, type: {MainViewViewModelType}", mainViewViewModel.GetType().FullName);
            
            CurrentView = new MainView
            {
                DataContext = mainViewViewModel
            };
            
            Logger.LogInformation("MainView created and set as CurrentView with injected MainViewViewModel");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create MainView with injected MainViewViewModel");
            throw;
        }
        
        Logger.LogDebug("MainWindowViewModel constructor completed successfully");
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
