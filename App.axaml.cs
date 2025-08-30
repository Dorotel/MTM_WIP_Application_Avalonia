using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
                            _logger?.LogDebug("Showing startup dialog on UI thread");
                            await StartupDialog.ShowStartupInfoAsync(configuration);
                            _logger?.LogDebug("Startup dialog completed");
                        });
                    });
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
