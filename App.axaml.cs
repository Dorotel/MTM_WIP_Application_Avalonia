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
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Views;




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
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() started");
            
            // Load XAML with proper error handling
            AvaloniaXamlLoader.Load(this);
            
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
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.OnFrameworkInitializationCompleted() started");

        // Check if we're in design mode and exit early
        if (IsDesignMode())
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Design mode detected - skipping service initialization");
            base.OnFrameworkInitializationCompleted();
            return;
        }

        try
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Initializing application services...");

            // Initialize configuration with proper null checking
            var configuration = Program.GetService<IConfiguration>() 
                ?? throw new InvalidOperationException("Configuration service could not be resolved");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Configuration service obtained");

            // Initialize application variables with configuration
            Model_AppVariables.Initialize(configuration);
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Model_AppVariables initialized");

            // Initialize logging infrastructure with proper error handling
            _logger = Program.GetService<ILogger<App>>();
            var loggerFactory = Program.GetService<ILoggerFactory>() 
                ?? throw new InvalidOperationException("LoggerFactory service could not be resolved");
            var generalLogger = loggerFactory.CreateLogger("Helper_Database_StoredProcedure");
            Helper_Database_StoredProcedure.SetLogger(generalLogger);

            _logger?.LogInformation("MTM WIP Application framework initialization started");
            _logger?.LogInformation("Model_AppVariables and database helper initialized successfully");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logging infrastructure initialized");

            // Initialize theme system before creating UI components
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Initializing theme system...");
            // Use synchronous initialization to avoid theme loading conflicts
            InitializeDefaultTheme();

            // Configure desktop application lifetime with dependency injection
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Creating MainWindow with dependency injection...");

                // Resolve MainWindow ViewModel with proper error handling
                var mainWindowViewModel = Program.GetService<MainWindowViewModel>() 
                    ?? throw new InvalidOperationException("MainWindowViewModel service could not be resolved");
                _logger?.LogInformation("MainWindowViewModel service resolved successfully");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainWindowViewModel resolved");

                // Create and configure main window
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel
                };

                _logger?.LogInformation("Main window created with dependency injection - DataContext set to MainWindowViewModel");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainWindow created and DataContext set");

                // Initialize MainView after Avalonia platform is ready
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Initializing MainView after platform initialization...");
                try
                {
                    mainWindowViewModel.InitializeMainView();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainView initialized successfully after platform startup");
                    _logger?.LogInformation("MainView initialized successfully after Avalonia platform startup");
                }
                catch (Exception mainViewEx)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Warning: MainView initialization failed after platform startup: {mainViewEx.Message}");
                    _logger?.LogWarning(mainViewEx, "MainView initialization failed after platform startup - application will continue with empty content");
                }

                // Schedule startup dialog with proper async handling
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Scheduling startup dialog...");
                _ = Task.Run(async () =>
                {
                    _logger?.LogDebug("Startup dialog task started");
                    // Wait for main window initialization
                    await Task.Delay(1000);

                    // Initialize theme service after UI is ready
                    try
                    {
                        var themeService = Program.GetService<IThemeService>();
                        var themeInitResult = await themeService.InitializeThemeSystemAsync();
                        if (themeInitResult.IsSuccess)
                        {
                            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Deferred theme initialization successful: {themeInitResult.Message}");
                            _logger?.LogInformation("Deferred theme initialization successful: {Message}", themeInitResult.Message);
                        }
                        else
                        {
                            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Deferred theme initialization failed: {themeInitResult.Message}");
                            _logger?.LogWarning("Deferred theme initialization failed: {Message}", themeInitResult.Message);
                        }
                    }
                    catch (Exception themeEx)
                    {
                        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Deferred theme initialization error: {themeEx.Message}");
                        _logger?.LogWarning(themeEx, "Deferred theme initialization error");
                    }

                    // Initialize master data service early for all ViewModels
                    try
                    {
                        var masterDataService = Program.GetService<IMasterDataService>();
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
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Framework initialization completed");
        }
        catch (Exception ex)
        {
            var errorMessage = $"Error during application framework initialization: {ex.Message}";
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {errorMessage}");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Stack trace: {ex.StackTrace}");

            _logger?.LogError(ex, "Critical error during application framework initialization");

            // Create fallback window for debugging if main window creation failed
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow == null)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Creating fallback MainWindow due to initialization error");
                desktop.MainWindow = new MainWindow
                {
                    DataContext = null // Use design-time data
                };
            }

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
    private void InitializeDefaultTheme()
    {
        try
        {
            if (Application.Current?.Resources == null) return;

            // Clear any existing theme resource conflicts
            ClearConflictingThemeResources();

            // Set a basic light theme variant - actual theme will be loaded later by ThemeService
            RequestedThemeVariant = ThemeVariant.Light;

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Basic theme initialization complete - ThemeService will load user's preferred theme");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error initializing basic theme: {ex.Message}");
            _logger?.LogError(ex, "Error initializing basic theme");
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
}

