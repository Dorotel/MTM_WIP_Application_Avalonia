using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Models.Core;




namespace MTM_WIP_Application_Avalonia;

/// <summary>
/// Main application class for MTM WIP Application.
/// Implements proper .NET dependency injection patterns and Avalonia XAML loading.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Logger instance for application lifecycle events
    /// </summary>
    private ILogger<App>? _logger;

    /// <summary>
    /// Initializes the application and loads XAML resources.
    /// Implements proper error handling and logging following .NET best practices.
    /// </summary>
    /// <exception cref="XamlLoadException">Thrown when XAML loading fails</exception>
    public override void Initialize()
    {
        try
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] App.Initialize() started - XAML loading begins");

            // Set up global exception handlers for emergency shutdown
            SetupGlobalExceptionHandlers();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Global exception handlers configured");

            // Load XAML with proper error handling
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to load XAML with AvaloniaXamlLoader.Load(this)");
            AvaloniaXamlLoader.Load(this);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] XAML loading completed successfully");

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() completed - XAML loaded successfully");
        }
        catch (Exception ex)
        {
            // Log the error and re-throw with additional context
            var errorMessage = $"Failed to initialize application XAML resources: {ex.Message}";
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() failed: {errorMessage}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Stack trace: {ex.StackTrace}");

            // Ensure the error is properly propagated
            throw new InvalidOperationException(errorMessage, ex);
        }
    }

    /// <summary>
    /// Called when the application framework initialization is completed.
    /// Implements dependency injection and service configuration following .NET best practices.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when critical services cannot be resolved</exception>
    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] App.OnFrameworkInitializationCompleted() started");

        // Check if we're in design mode and exit early
        if (IsDesignMode())
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Design mode detected - skipping service initialization");
            base.OnFrameworkInitializationCompleted();
            return;
        }

        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Not in design mode - proceeding with full initialization");

        try
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Initializing application services...");

            // Initialize configuration with proper null checking
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to get Configuration service...");
            var configuration = Program.GetService<IConfiguration>()
                ?? throw new InvalidOperationException("Configuration service could not be resolved");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration service obtained successfully");

            // Initialize application variables with configuration
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to initialize Model_AppVariables...");
            Model_AppVariables.Initialize(configuration);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Model_AppVariables initialized successfully");

            // Initialize logging infrastructure with proper error handling
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to get ILogger<App> service...");
            _logger = Program.GetService<ILogger<App>>();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ILogger<App> service obtained successfully");

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to get ILoggerFactory service...");
            var loggerFactory = Program.GetService<ILoggerFactory>()
                ?? throw new InvalidOperationException("LoggerFactory service could not be resolved");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ILoggerFactory service obtained successfully");

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to create general logger and set Helper_Database_StoredProcedure logger...");
            var generalLogger = loggerFactory.CreateLogger("Services.Core.Helper_Database_StoredProcedure");
            Services.Core.Helper_Database_StoredProcedure.SetLogger(generalLogger);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] General logger and Helper_Database_StoredProcedure logger set successfully");

            _logger?.LogInformation("MTM WIP Application framework initialization started");
            _logger?.LogInformation("Model_AppVariables and database helper initialized successfully");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Logging infrastructure initialized successfully");

            // Apply default theme immediately for UI rendering - user preferences will load asynchronously
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to apply default theme for immediate UI rendering...");
            ApplyDefaultThemeForUI();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Default theme applied for UI - user preferences will load after startup");

            // Configure desktop application lifetime with dependency injection
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Creating MainWindow with dependency injection...");

                // Resolve MainWindow ViewModel with proper error handling
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to resolve MainWindowViewModel service...");
                var mainWindowViewModel = Program.GetService<MainWindowViewModel>()
                    ?? throw new InvalidOperationException("MainWindowViewModel service could not be resolved");
                _logger?.LogInformation("MainWindowViewModel service resolved successfully");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainWindowViewModel resolved successfully");

                // Create and configure main window
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to create new MainWindow and set DataContext...");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainWindow created and DataContext set successfully");

                _logger?.LogInformation("Main window created with dependency injection - DataContext set to MainWindowViewModel");

                // Initialize MainView after Avalonia platform is ready
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to initialize MainView after platform initialization...");
                try
                {
                    mainWindowViewModel.InitializeMainView();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] MainView initialized successfully after platform startup");
                    _logger?.LogInformation("MainView initialized successfully after Avalonia platform startup");
                }
                catch (Exception mainViewEx)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Warning: MainView initialization failed after platform startup: {mainViewEx.Message}");
                    _logger?.LogWarning(mainViewEx, "MainView initialization failed after platform startup - application will continue with empty content");
                }

                // Schedule startup dialog with proper async handling
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Scheduling startup dialog task...");
                _ = Task.Run(async () =>
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Startup dialog task started");
                    _logger?.LogDebug("Startup dialog task started");
                    // Wait for main window initialization
                    await Task.Delay(1000);
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Startup dialog delay completed");

                    // Load database-driven user theme preferences AFTER UI is created and can render
                    try
                    {
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Loading user's database-driven theme preferences...");
                        var themeService = Program.GetService<IThemeService>();
                        if (themeService != null)
                        {
                            var themeInitResult = await themeService.InitializeThemeSystemAsync();
                            if (themeInitResult.IsSuccess)
                            {
                                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Database-driven theme loaded successfully: {themeInitResult.Message}");
                                _logger?.LogInformation("Database-driven theme loaded successfully after UI creation: {Message}", themeInitResult.Message);
                            }
                            else
                            {
                                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Database-driven theme loading failed: {themeInitResult.Message}");
                                _logger?.LogWarning("Database-driven theme loading failed: {Message}", themeInitResult.Message);
                            }
                        }
                    }
                    catch (Exception themeEx)
                    {
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Database-driven theme loading error: {themeEx.Message}");
                        _logger?.LogWarning(themeEx, "Database-driven theme loading error");
                    }

                    // Initialize master data service early for all ViewModels
                    try
                    {
                        var masterDataService = Program.GetService<Services.Business.IMasterDataService>();
                        if (masterDataService != null)
                        {
                            await masterDataService.LoadAllMasterDataAsync();
                            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Master data loaded successfully at startup");
                            _logger?.LogInformation("Master data loaded successfully at application startup");

                        }
                    }
                    catch (Exception masterDataEx)
                    {
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Warning: Master data loading failed: {masterDataEx.Message}");
                        _logger?.LogWarning(masterDataEx, "Master data loading failed at startup - will use fallback data");
                    }

                    // Execute startup dialog on UI thread
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        // Startup dialog implementation can be added here
                        _logger?.LogDebug("Startup dialog execution point reached");
                        return Task.CompletedTask;
                    });
                });

#if DEBUG
                this.AttachDevTools();
#endif
            }

            _logger?.LogInformation("Framework initialization completed successfully");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Framework initialization completed successfully");

            // Call base implementation
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to call base.OnFrameworkInitializationCompleted()...");
            base.OnFrameworkInitializationCompleted();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] base.OnFrameworkInitializationCompleted() completed successfully");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error during application framework initialization: {ex.Message}";
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL ERROR: {errorMessage}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Exception Type: {ex.GetType().Name}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Stack trace: {ex.StackTrace}");

            _logger?.LogError(ex, "Critical error during application framework initialization");

            // Create fallback window for debugging if main window creation failed
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Creating fallback MainWindow due to initialization error");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = null // Use design-time data
                };
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] About to rethrow exception after cleanup...");
            throw;
        }
        finally
        {
            // Always call base implementation
            base.OnFrameworkInitializationCompleted();
        }
    }

    /// <summary>
    /// Initialize the default theme to ensure proper startup appearance.
    /// Now checks database for user's preferred theme instead of forcing MTM_Light.
    /// Follows .NET best practices for resource management and error handling.
    /// </summary>
    /// <summary>
    /// Apply a default theme immediately for UI creation without database operations.
    /// This ensures UI elements can render properly while user preferences load asynchronously.
    /// </summary>
    private void ApplyDefaultThemeForUI()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Set basic theme variant for immediate UI rendering
            RequestedThemeVariant = ThemeVariant.Light;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Default light theme applied for immediate UI rendering");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Error applying default theme for UI: {ex.Message}");
            _logger?.LogError(ex, "Error applying default theme for UI");
        }
    }

    /// <summary>
    /// Sets a basic theme fallback when database-driven theme loading fails.
    /// Ensures UI has minimal theme resources to function properly.
    /// </summary>
    private void SetBasicThemeFallback()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Set basic theme variant as emergency fallback
            RequestedThemeVariant = ThemeVariant.Light;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Basic theme fallback applied");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] CRITICAL: Even basic theme fallback failed: {ex.Message}");
            _logger?.LogError(ex, "CRITICAL: Even basic theme fallback failed");
        }
    }

    /// <summary>
    /// Forces loading of MTM Light theme resources with proper error handling.
    /// Implements fallback strategies following .NET best practices.
    /// </summary>
    private void ForceLoadMTMLightTheme()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Create new resource dictionary for MTM Light theme
            var mtmLightResources = new Avalonia.Controls.ResourceDictionary();
            var mtmLightUri = new Uri("avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTM_Light.axaml");

            try
            {
                // Load MTM Light theme resources
                mtmLightResources = (Avalonia.Controls.ResourceDictionary)AvaloniaXamlLoader.Load(mtmLightUri);

                // Remove existing MTM Light themes to prevent conflicts
                var existingMTMLight = Application.Current.Resources.MergedDictionaries
                    .Where(dict => dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var value) &&
                           value?.ToString() == "#B8860B") // MTM Light's primary color
                    .ToList();

                foreach (var dict in existingMTMLight)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(dict);
                }

                // Add MTM Light theme as the last dictionary for proper precedence
                Application.Current.Resources.MergedDictionaries.Add(mtmLightResources);

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM Light theme resources loaded successfully");

                // Force tab theme colors for proper UI rendering
                ForceTabThemeColors();
            }
            catch (Exception loadEx)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Could not load MTM Light theme resources: {loadEx.Message}");

                // Fallback: Manually set MTM Light colors
                ForceMTMLightColors();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error in ForceLoadMTMLightTheme: {ex.Message}");
            _logger?.LogWarning(ex, "Error force loading MTM Light theme");
        }
    }

    /// <summary>
    /// Forces specific tab theme colors to ensure proper TabControl styling.
    /// Implements color constants following .NET best practices.
    /// </summary>
    private void ForceTabThemeColors()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Define tab theme colors using immutable dictionary for better performance
            var tabThemeColors = new Dictionary<string, string>
            {
                ["MTM_Shared_Logic.PrimaryAction"] = "#B8860B",      // Dark goldenrod for selected tabs
                ["MTM_Shared_Logic.OverlayTextBrush"] = "#FFFFFF",   // White text for selected tabs
                ["MTM_Shared_Logic.CardBackgroundBrush"] = "#FFFFFF", // White background for unselected tabs
                ["MTM_Shared_Logic.BorderAccentBrush"] = "#FFEB9C",  // Light border accent
                ["MTM_Shared_Logic.HoverBackground"] = "#FFF9D1",    // Light hover background
                ["MTM_Shared_Logic.BodyText"] = "#666666",           // Dark gray for unselected tab text
                ["MTM_Shared_Logic.BorderDarkBrush"] = "#F5F5DC"     // Light border for unselected tabs
            };

            foreach (var (key, colorValue) in tabThemeColors)
            {
                var brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(colorValue));
                Application.Current.Resources[key] = brush;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Tab theme colors applied successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error applying tab theme colors: {ex.Message}");
            _logger?.LogWarning(ex, "Error applying tab theme colors");
        }
    }

    /// <summary>
    /// Fallback method to manually set MTM Light theme colors when resource loading fails.
    /// Provides comprehensive color definitions following design system standards.
    /// </summary>
    private void ForceMTMLightColors()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Define complete MTM Light color palette using immutable dictionary
            var mtmLightColors = new Dictionary<string, string>
            {
                // Primary and secondary actions
                ["MTM_Shared_Logic.PrimaryAction"] = "#B8860B",      // Dark goldenrod
                ["MTM_Shared_Logic.SecondaryAction"] = "#DAA520",    // Goldenrod
                ["MTM_Shared_Logic.OverlayTextBrush"] = "#FFFFFF",   // White text

                // Background and layout colors
                ["MTM_Shared_Logic.SidebarGradientBrush"] = "#B8860B", // Sidebar gradient
                ["MTM_Shared_Logic.FooterBackgroundBrush"] = "#8C7300", // Footer background
                ["MTM_Shared_Logic.PageHeaders"] = "#B8860B",        // Page headers
                ["MTM_Shared_Logic.CardBackgroundBrush"] = "#FFFFFF", // Card backgrounds
                ["MTM_Shared_Logic.MainBackground"] = "#FFF9D1",     // Main background
                ["MTM_Shared_Logic.ContentAreas"] = "#FFFFFF",       // Content areas

                // Border and accent colors
                ["MTM_Shared_Logic.BorderAccentBrush"] = "#FFEB9C",  // Light border accent
                ["MTM_Shared_Logic.BorderDarkBrush"] = "#F5F5DC",    // Light borders
                ["MTM_Shared_Logic.HoverBackground"] = "#FFF9D1",    // Hover background

                // Text colors
                ["MTM_Shared_Logic.BodyText"] = "#666666",           // Body text
                ["MTM_Shared_Logic.HeadingText"] = "#8C7300",        // Heading text
                ["MTM_Shared_Logic.DarkNavigation"] = "#8C7300"      // Navigation text
            };

            foreach (var (key, colorValue) in mtmLightColors)
            {
                var brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(colorValue));
                Application.Current.Resources[key] = brush;
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM Light colors applied as fallback");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error applying fallback MTM Light colors: {ex.Message}");
            _logger?.LogWarning(ex, "Error applying fallback MTM Light colors");
        }
    }

    /// <summary>
    /// Clears conflicting theme resources that might interfere with proper theme application.
    /// Implements safe resource cleanup following .NET disposal patterns.
    /// </summary>
    private void ClearConflictingThemeResources()
    {
        try
        {
            if (Application.Current?.Resources?.MergedDictionaries == null) return;

            // Identify and remove conflicting theme dictionaries
            var conflictingThemes = Application.Current.Resources.MergedDictionaries
                .Where(dict => dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var value) &&
                       value?.ToString() != "#B8860B") // Remove non-MTM Light themes
                .ToList();

            foreach (var themeDict in conflictingThemes)
            {
                Application.Current.Resources.MergedDictionaries.Remove(themeDict);
            }

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Removed {conflictingThemes.Count} conflicting theme dictionaries");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error clearing theme conflicts: {ex.Message}");
            _logger?.LogWarning(ex, "Error clearing theme resource conflicts");
        }
    }

    /// <summary>
    /// Determines if the application is running in design mode.
    /// Provides safe fallback for design-time scenarios.
    /// </summary>
    /// <returns>True if running in design mode, false otherwise</returns>
    private static bool IsDesignMode()
    {
        try
        {
            return Design.IsDesignMode;
        }
        catch
        {
            // Safe fallback if Design class isn't available
            return false;
        }
    }

    /// <summary>
    /// Sets up global exception handlers for emergency shutdown capability
    /// when UI becomes unresponsive or critical errors occur
    /// </summary>
    private void SetupGlobalExceptionHandlers()
    {
        try
        {
            // Handle unhandled exceptions in the current AppDomain
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // Handle unobserved task exceptions
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Global exception handlers configured for emergency shutdown");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error setting up global exception handlers: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles unhandled exceptions with emergency shutdown capability
    /// </summary>
    private async void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        try
        {
            var exception = e.ExceptionObject as Exception;
            var message = exception?.Message ?? "Unknown critical error";

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] CRITICAL: Unhandled exception occurred - {message}");
            _logger?.LogCritical(exception, "Unhandled exception occurred - attempting emergency shutdown");

            // Try to show emergency error overlay if possible
            await TryShowEmergencyErrorOverlay(message, exception?.ToString() ?? "No details available");
        }
        catch (Exception emergencyEx)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Emergency error handling failed: {emergencyEx.Message}");
        }
        finally
        {
            // If it's terminating, force shutdown
            if (e.IsTerminating)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AppDomain is terminating - forcing immediate shutdown");
                Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Handles unobserved task exceptions
    /// </summary>
    private async void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        try
        {
            var message = e.Exception.InnerException?.Message ?? e.Exception.Message;
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: Unobserved task exception - {message}");
            _logger?.LogError(e.Exception, "Unobserved task exception occurred");

            // Try to show emergency error overlay for severe task exceptions
            if (e.Exception.InnerExceptions.Count > 3) // Multiple exceptions might indicate system instability
            {
                await TryShowEmergencyErrorOverlay("Multiple Task Failures",
                    $"Multiple background tasks have failed. The application may be unstable.\n\n{message}");
            }

            // Mark as observed to prevent app termination
            e.SetObserved();
        }
        catch (Exception emergencyEx)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Emergency task exception handling failed: {emergencyEx.Message}");
        }
    }

    /// <summary>
    /// Attempts to show an emergency error overlay with shutdown options
    /// </summary>
    private async Task TryShowEmergencyErrorOverlay(string message, string details)
    {
        try
        {
            // Try to get the success overlay service for emergency display
            var overlayService = Program.GetService<ISuccessOverlayService>();
            if (overlayService != null)
            {
                await overlayService.ShowSuccessOverlayInMainViewAsync(
                    null, // sourceControl
                    $"Critical Error: {message}",
                    $"Emergency shutdown available.\n\n{details}",
                    "AlertCircle", // Error icon
                    0, // No auto-dismiss for critical errors
                    true // isError = true (shows Exit/Continue buttons)
                );

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Emergency error overlay displayed");
            }
            else
            {
                // Fallback: Try direct UI thread emergency dialog
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    // Create simple emergency dialog
                    var dialog = new Window
                    {
                        Title = "Critical Error - Emergency Shutdown",
                        Width = 400,
                        Height = 200,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    };

                    var exitButton = new Button
                    {
                        Content = "Exit Application",
                        Margin = new Avalonia.Thickness(4)
                    };
                    exitButton.Click += (_, _) => Environment.Exit(1);

                    var continueButton = new Button
                    {
                        Content = "Continue",
                        Margin = new Avalonia.Thickness(4)
                    };
                    continueButton.Click += (_, _) => dialog.Close();

                    dialog.Content = new StackPanel
                    {
                        Spacing = 16,
                        Margin = new Avalonia.Thickness(16),
                        Children =
                        {
                            new TextBlock { Text = $"Critical Error: {message}", FontWeight = Avalonia.Media.FontWeight.Bold },
                            new TextBlock { Text = details, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                            new StackPanel
                            {
                                Orientation = Avalonia.Layout.Orientation.Horizontal,
                                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                                Spacing = 8,
                                Children = { exitButton, continueButton }
                            }
                        }
                    };

                    // Try to show as dialog with main window, fallback to Show() if dialog fails
                    try
                    {
                        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow != null)
                        {
                            await dialog.ShowDialog(desktop.MainWindow);
                        }
                        else
                        {
                            dialog.Show();
                        }
                    }
                    catch
                    {
                        dialog.Show(); // Fallback to non-modal
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Failed to show emergency error overlay: {ex.Message}");

            // Last resort: Console prompt for emergency action
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] EMERGENCY: Press Ctrl+C to terminate or wait 10 seconds for automatic shutdown");
            await Task.Delay(10000);
            Environment.Exit(1);
        }
    }
}

