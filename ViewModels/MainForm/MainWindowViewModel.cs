using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Feature;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.Services.Infrastructure;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Views;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly IUniversalOverlayService? _overlayService;
    private readonly Lazy<ConnectionStatusOverlayViewModel>? _connectionStatusOverlay;
    private readonly Lazy<EmergencyShutdownOverlayViewModel>? _emergencyShutdownOverlay;
    private readonly Lazy<ThemeQuickSwitcherOverlayViewModel>? _themeQuickSwitcherOverlay;

    private object? _currentView;
    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public MainWindowViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        ILogger<MainWindowViewModel> logger,
        IUniversalOverlayService? overlayService = null,
        Lazy<ConnectionStatusOverlayViewModel>? connectionStatusOverlay = null,
        Lazy<EmergencyShutdownOverlayViewModel>? emergencyShutdownOverlay = null,
        Lazy<ThemeQuickSwitcherOverlayViewModel>? themeQuickSwitcherOverlay = null) : base(logger)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainWindowViewModel constructor started");
        Logger.LogDebug("MainWindowViewModel constructor started");

        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainWindowViewModel - navigation and state services injected");

        // Optional overlay service injection - allows for graceful degradation if not available
        _overlayService = overlayService;
        _connectionStatusOverlay = connectionStatusOverlay;
        _emergencyShutdownOverlay = emergencyShutdownOverlay;
        _themeQuickSwitcherOverlay = themeQuickSwitcherOverlay;

        Logger.LogInformation("MainWindowViewModel initialized with dependency injection - Services injected successfully");
        Logger.LogDebug("NavigationService type: {NavigationServiceType}", _navigationService.GetType().FullName);
        Logger.LogDebug("ApplicationStateService type: {ApplicationStateType}", _applicationState.GetType().FullName);
        Logger.LogDebug("OverlayService available: {OverlayServiceAvailable}", _overlayService != null);

        // Initialize overlay commands
        InitializeOverlayCommands();

        // Wire up navigation service to update CurrentView when navigation occurs
        Logger.LogDebug("Subscribing to NavigationService.Navigated event");
        _navigationService.Navigated += OnNavigated;
        Logger.LogDebug("Successfully subscribed to NavigationService.Navigated event");

        // Set MainView as the current content - defer creation to avoid service locator pattern during startup validation
        Logger.LogDebug("Deferring MainView creation to avoid service provider dependency during startup validation");
        // CurrentView will be set after full application startup via navigation or manual assignment

        Logger.LogDebug("MainWindowViewModel constructor completed successfully");
    }

    #region Overlay Commands

    /// <summary>
    /// Command to show the connection status overlay (F9).
    /// </summary>
    public ICommand ShowConnectionStatusOverlayCommand { get; private set; } = null!;

    /// <summary>
    /// Command to show the emergency shutdown overlay (F10).
    /// </summary>
    public ICommand ShowEmergencyShutdownOverlayCommand { get; private set; } = null!;

    /// <summary>
    /// Command to show the theme quick switcher overlay (F11).
    /// </summary>
    public ICommand ShowThemeQuickSwitcherOverlayCommand { get; private set; } = null!;

    /// <summary>
    /// Initializes overlay-related commands.
    /// </summary>
    private void InitializeOverlayCommands()
    {
        Logger.LogDebug("Initializing MainWindow overlay commands");

        ShowConnectionStatusOverlayCommand = new RelayCommand(async () => await ShowConnectionStatusOverlayAsync());
        ShowEmergencyShutdownOverlayCommand = new RelayCommand(async () => await ShowEmergencyShutdownOverlayAsync());
        ShowThemeQuickSwitcherOverlayCommand = new RelayCommand(async () => await ShowThemeQuickSwitcherOverlayAsync());

        Logger.LogDebug("MainWindow overlay commands initialized successfully");
    }

    /// <summary>
    /// Shows the connection status overlay for database connectivity testing.
    /// </summary>
    public async Task ShowConnectionStatusOverlayAsync()
    {
        try
        {
            Logger.LogInformation("Showing Connection Status overlay via MainWindow");

            if (_connectionStatusOverlay?.Value != null)
            {
                await _connectionStatusOverlay.Value.ShowAsync();
                Logger.LogInformation("Connection Status overlay displayed successfully");
            }
            else if (_overlayService != null)
            {
                Logger.LogDebug("Using UniversalOverlayService for Connection Status overlay");
                var result = await _overlayService.ShowOverlayAsync(new { OverlayType = "ConnectionStatus" });
                Logger.LogInformation("Connection Status overlay via UniversalOverlayService completed with result: {Success}", result.IsSuccess);
            }
            else
            {
                Logger.LogWarning("No overlay service available to show Connection Status overlay");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing Connection Status overlay");
        }
    }

    /// <summary>
    /// Shows the emergency shutdown overlay for critical application termination.
    /// </summary>
    public async Task ShowEmergencyShutdownOverlayAsync()
    {
        try
        {
            Logger.LogCritical("Showing Emergency Shutdown overlay via MainWindow");

            if (_emergencyShutdownOverlay?.Value != null)
            {
                await _emergencyShutdownOverlay.Value.ShowAsync();
                Logger.LogCritical("Emergency Shutdown overlay displayed successfully");
            }
            else if (_overlayService != null)
            {
                Logger.LogDebug("Using UniversalOverlayService for Emergency Shutdown overlay");
                var result = await _overlayService.ShowOverlayAsync(new { OverlayType = "EmergencyShutdown" });
                Logger.LogCritical("Emergency Shutdown overlay via UniversalOverlayService completed with result: {Success}", result.IsSuccess);
            }
            else
            {
                Logger.LogError("No overlay service available to show Emergency Shutdown overlay - this is a critical system failure");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Critical error showing Emergency Shutdown overlay");
        }
    }

    /// <summary>
    /// Shows the theme quick switcher overlay for rapid theme changes.
    /// </summary>
    public async Task ShowThemeQuickSwitcherOverlayAsync()
    {
        try
        {
            Logger.LogInformation("Showing Theme Quick Switcher overlay via MainWindow");

            if (_themeQuickSwitcherOverlay?.Value != null)
            {
                await _themeQuickSwitcherOverlay.Value.ShowAsync();
                Logger.LogInformation("Theme Quick Switcher overlay displayed successfully");
            }
            else if (_overlayService != null)
            {
                Logger.LogDebug("Using UniversalOverlayService for Theme Quick Switcher overlay");
                var result = await _overlayService.ShowOverlayAsync(new { OverlayType = "ThemeQuickSwitcher" });
                Logger.LogInformation("Theme Quick Switcher overlay via UniversalOverlayService completed with result: {Success}", result.IsSuccess);
            }
            else
            {
                Logger.LogWarning("No overlay service available to show Theme Quick Switcher overlay");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error showing Theme Quick Switcher overlay");
        }
    }

    #endregion

    /// <summary>
    /// Manually initialize the MainView after application startup is complete.
    /// This avoids the service locator pattern during startup validation.
    /// </summary>
    public void InitializeMainView()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainWindowViewModel.InitializeMainView() started");

        if (CurrentView != null)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainView already initialized, skipping");
            Logger.LogDebug("MainView already initialized, skipping");
            return;
        }

        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Manually initializing MainView after startup");
        Logger.LogDebug("Manually initializing MainView after startup");
        try
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Resolving MainViewViewModel...");
            var mainViewViewModel = Program.GetService<MainViewViewModel>();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainViewViewModel resolved successfully");
            Logger.LogDebug("MainViewViewModel resolved successfully for manual initialization, type: {MainViewViewModelType}", mainViewViewModel.GetType().FullName);

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Creating MainView...");
            CurrentView = new MainView
            {
                DataContext = mainViewViewModel
            };
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainView created and set as CurrentView");

            Logger.LogInformation("MainView manually initialized and set as CurrentView");

            // Request startup focus after MainView is fully initialized
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Requesting startup focus...");
            Logger.LogDebug("Requesting startup focus after MainView initialization");
            mainViewViewModel.RequestStartupFocus();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainView initialization completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Failed to manually initialize MainView: {ex.Message}");
            Logger.LogError(ex, "Failed to manually initialize MainView");
            throw;
        }
    }

    private void OnNavigated(object? sender, Services.Infrastructure.NavigationEventArgs e)
    {
        Logger.LogDebug("OnNavigated event handler triggered - Sender: {SenderType}, Target: {TargetType}",
            sender?.GetType().Name ?? "null", e.Target?.GetType().Name ?? "null");

        try
        {
            var previousView = CurrentView?.GetType().Name ?? "null";
            CurrentView = e.Target;
            var newView = e.Target?.GetType().Name ?? "null";

            Logger.LogInformation("MainWindow navigation completed - Previous: {PreviousView}, New: {NewView}", previousView, newView);
            Logger.LogDebug("Navigation event args - Target: {Target}", e.Target);
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

