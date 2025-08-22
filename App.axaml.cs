using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.Views;

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
            try
            {
                // Initialize logging
                _logger = Program.GetService<ILogger<App>>();
                _logger?.LogInformation("MTM WIP Application starting...");

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
                throw;
            }
        }
    }
}