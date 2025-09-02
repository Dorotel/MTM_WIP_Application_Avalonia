using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Controls;




namespace MTM_WIP_Application_Avalonia
{

    public partial class App : Application
    {
        private ILogger<App>? _logger;

        public override void Initialize()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() started");
                AvaloniaXamlLoader.Load(this);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() completed - XAML loaded successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] App.Initialize() failed: {ex.Message}");
                throw;
            }
        }

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

                // Initialize configuration and application variables
                var configuration = Program.GetService<IConfiguration>();
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Configuration service obtained");

                Model_AppVariables.Initialize(configuration);
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Model_AppVariables initialized");

                // Initialize comprehensive logging
                _logger = Program.GetService<ILogger<App>>();
                var loggerFactory = Program.GetService<ILoggerFactory>();
                var generalLogger = loggerFactory.CreateLogger("Helper_Database_StoredProcedure");
                Helper_Database_StoredProcedure.SetLogger(generalLogger);

                _logger?.LogInformation("MTM WIP Application framework initialization started");
                _logger?.LogInformation("Model_AppVariables and database helper initialized successfully");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logging infrastructure initialized");

                // CRITICAL: Initialize theme system BEFORE creating UI
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Initializing theme system...");
                InitializeDefaultTheme();

                // Application initialization - error handling configured via services

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Creating MainWindow with dependency injection...");

                    // Create MainWindow with injected ViewModel
                    var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
                    _logger?.LogInformation("MainWindowViewModel service resolved successfully");
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainWindowViewModel resolved");

                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = mainWindowViewModel
                    };

                    _logger?.LogInformation("Main window created with dependency injection - DataContext set to MainWindowViewModel");
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MainWindow created and DataContext set");

                    // Show startup information dialog after main window is created
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Scheduling startup dialog...");
                    Task.Run(async () =>
                    {
                        _logger?.LogDebug("Startup dialog task started");
                        // Wait a moment for the main window to fully initialize
                        await Task.Delay(1000);

                        // Show startup dialog on UI thread
                        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            // _logger?.LogDebug("Showing startup dialog on UI thread");
                            // await StartupDialog.ShowStartupInfoAsync(configuration);
                            // _logger?.LogDebug("Startup dialog completed");
                        });
                    });
                    #if DEBUG
    this.AttachDevTools();
#endif
    base.OnFrameworkInitializationCompleted();
                }

                _logger?.LogInformation("Framework initialization completed successfully");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Framework initialization completed");
                base.OnFrameworkInitializationCompleted();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error during application framework initialization: {ex.Message}";
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {errorMessage}");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Stack trace: {ex.StackTrace}");

                _logger?.LogError(ex, "Critical error during application framework initialization");

                // In case of service provider issues, create a minimal window for debugging
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow == null)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Creating fallback MainWindow due to initialization error");
                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = null // Will use design-time data
                    };
                }

                throw;
            }
        }

        /// <summary>
        /// Initialize the default theme to ensure proper startup appearance
        /// </summary>
        private void InitializeDefaultTheme()
        {
            try
            {
                if (Application.Current?.Resources == null) return;

                // Force clear any existing theme resource conflicts
                ClearConflictingThemeResources();

                // Apply the default MTM Light theme by setting proper theme variant
                RequestedThemeVariant = ThemeVariant.Light;

                // CRITICAL: Force reload MTM Light theme resources
                ForceLoadMTMLightTheme();

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Default theme initialized - MTM Light theme forced");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error initializing default theme: {ex.Message}");
                _logger?.LogError(ex, "Error initializing default theme");
            }
        }

        /// <summary>
        /// Force load MTM Light theme resources to override any conflicting themes
        /// </summary>
        private void ForceLoadMTMLightTheme()
        {
            try
            {
                if (Application.Current?.Resources == null) return;

                // Create a new resource dictionary for MTM Light theme
                var mtmLightResources = new Avalonia.Controls.ResourceDictionary();

                // Load MTM Light theme resources
                var mtmLightUri = new Uri("avares://MTM_WIP_Application_Avalonia/Resources/Themes/MTM_Light.axaml");

                try
                {
                    mtmLightResources = (Avalonia.Controls.ResourceDictionary)AvaloniaXamlLoader.Load(mtmLightUri);

                    // Remove any existing MTM Light theme from merged dictionaries
                    var existingMTMLight = Application.Current.Resources.MergedDictionaries
                        .Where(dict => dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var value) &&
                               value?.ToString() == "#B8860B") // MTM Light's primary color
                        .ToList();

                    foreach (var dict in existingMTMLight)
                    {
                        Application.Current.Resources.MergedDictionaries.Remove(dict);
                    }

                    // Add the MTM Light theme as the LAST dictionary to ensure it takes precedence
                    Application.Current.Resources.MergedDictionaries.Add(mtmLightResources);

                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM Light theme resources force-loaded successfully");

                    // CRITICAL: Also manually force the key tab colors to ensure proper override
                    ForceTabThemeColors();
                }
                catch (Exception loadEx)
                {
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Could not load MTM Light theme resources: {loadEx.Message}");

                    // Fallback: Manually set key MTM Light colors
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
        /// Force specific tab theme colors to ensure TabControl styling works properly
        /// </summary>
        private void ForceTabThemeColors()
        {
            try
            {
                if (Application.Current?.Resources == null) return;

                // Ensure critical tab theme colors are properly set
                var tabThemeColors = new Dictionary<string, string>
                {
                    ["MTM_Shared_Logic.PrimaryAction"] = "#B8860B",           // Dark goldenrod for selected tab background
                    ["MTM_Shared_Logic.OverlayTextBrush"] = "#FFFFFF",             // White text for selected tabs
                    ["MTM_Shared_Logic.CardBackgroundBrush"] = "#FFFFFF",    // White background for unselected tabs
                    ["MTM_Shared_Logic.BorderAccentBrush"] = "#FFEB9C",      // Light border accent
                    ["MTM_Shared_Logic.HoverBackground"] = "#FFF9D1",        // Light hover background
                    ["MTM_Shared_Logic.BodyText"] = "#666666",               // Dark gray for unselected tab text
                    ["MTM_Shared_Logic.BorderDarkBrush"] = "#F5F5DC"         // Light border for unselected tabs
                };

                foreach (var colorPair in tabThemeColors)
                {
                    var brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(colorPair.Value));
                    Application.Current.Resources[colorPair.Key] = brush;
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Tab theme colors manually enforced");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error enforcing tab theme colors: {ex.Message}");
                _logger?.LogWarning(ex, "Error enforcing tab theme colors");
            }
        }

        /// <summary>
        /// Fallback method to manually set MTM Light theme colors
        /// </summary>
        private void ForceMTMLightColors()
        {
            try
            {
                if (Application.Current?.Resources == null) return;

                // Manually override key theme colors to MTM Light values
                var mtmLightColors = new Dictionary<string, string>
                {
                    ["MTM_Shared_Logic.PrimaryAction"] = "#B8860B",          // Dark goldenrod - CRITICAL for selected tabs
                    ["MTM_Shared_Logic.SecondaryAction"] = "#DAA520",        // Goldenrod 
                    ["MTM_Shared_Logic.OverlayTextBrush"] = "#FFFFFF",             // White - CRITICAL for selected tab text
                    ["MTM_Shared_Logic.SidebarGradientBrush"] = "#B8860B",   // Sidebar gradient
                    ["MTM_Shared_Logic.FooterBackgroundBrush"] = "#8C7300",  // Footer background
                    ["MTM_Shared_Logic.PageHeaders"] = "#B8860B",            // Page headers

                    // CRITICAL TAB-SPECIFIC COLORS
                    ["MTM_Shared_Logic.CardBackgroundBrush"] = "#FFFFFF",    // White background for unselected tabs
                    ["MTM_Shared_Logic.BorderAccentBrush"] = "#FFEB9C",      // Light border accent
                    ["MTM_Shared_Logic.HoverBackground"] = "#FFF9D1",        // Light hover background
                    ["MTM_Shared_Logic.BodyText"] = "#666666",               // Dark gray for unselected tab text
                    ["MTM_Shared_Logic.BorderDarkBrush"] = "#F5F5DC",        // Light border for unselected tabs
                    ["MTM_Shared_Logic.HeadingText"] = "#8C7300",            // Heading text color

                    // Additional supporting colors
                    ["MTM_Shared_Logic.MainBackground"] = "#FFF9D1",         // Main background
                    ["MTM_Shared_Logic.ContentAreas"] = "#FFFFFF",           // Content areas
                    ["MTM_Shared_Logic.DarkNavigation"] = "#8C7300"          // Dark navigation
                };

                foreach (var colorPair in mtmLightColors)
                {
                    var brush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(colorPair.Value));
                    Application.Current.Resources[colorPair.Key] = brush;
                }

                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] MTM Light colors (including tab-specific) manually applied as fallback");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Error applying fallback MTM Light colors: {ex.Message}");
                _logger?.LogWarning(ex, "Error applying fallback MTM Light colors");
            }
        }

        /// <summary>
        /// Clear conflicting theme resources that might interfere with proper theme application
        /// </summary>
        private void ClearConflictingThemeResources()
        {
            try
            {
                if (Application.Current?.Resources?.MergedDictionaries == null) return;

                // Remove any existing theme resource dictionaries that might conflict
                var conflictingThemes = Application.Current.Resources.MergedDictionaries
                    .Where(dict => dict.TryGetResource("MTM_Shared_Logic.PrimaryAction", null, out var value) &&
                           value?.ToString() != "#B8860B") // Remove anything that's not MTM Light gold
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

        private static bool IsDesignMode()
        {
            try
            {
                return Design.IsDesignMode;
            }
            catch
            {
                // Fallback if Design class isn't available
                return false;
            }
        }
    }
}

