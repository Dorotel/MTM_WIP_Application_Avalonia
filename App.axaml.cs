using System;
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
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            // Check if we're in design mode and exit early
            if (IsDesignMode())
            {
                base.OnFrameworkInitializationCompleted();
                return;
            }

            try
            {
                // Initialize configuration and application variables
                var configuration = Program.GetService<IConfiguration>();
                Model_AppVariables.Initialize(configuration);

                // Initialize logging
                _logger = Program.GetService<ILogger<App>>();
                var loggerFactory = Program.GetService<ILoggerFactory>();
                var generalLogger = loggerFactory.CreateLogger("Helper_Database_StoredProcedure");
                Helper_Database_StoredProcedure.SetLogger(generalLogger);
                
                _logger?.LogInformation("MTM WIP Application starting...");
                _logger?.LogInformation("Model_AppVariables and database helper initialized");

                // Application initialization - error handling configured via services

                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // Create MainWindow with injected ViewModel
                    var mainWindowViewModel = Program.GetService<MainWindowViewModel>();
                    desktop.MainWindow = new MainWindow
                    {
                        DataContext = mainWindowViewModel
                    };

                    _logger?.LogInformation("Main window created with dependency injection");
                }

                base.OnFrameworkInitializationCompleted();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during application initialization");
                
                // In case of service provider issues, create a minimal window for debugging
                if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && desktop.MainWindow == null)
                {
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
