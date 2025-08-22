using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MTM.Services;
using MTM.Models;
using MTM.Core.Services;

namespace MTM_WIP_Application_Avalonia
{
    public partial class App : Application
    {
        /// <summary>
        /// Service provider for dependency injection throughout the application.
        /// </summary>
        public static IServiceProvider? ServiceProvider { get; set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Initialize services
                InitializeServices();

                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Initializes application services and performs startup tasks.
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                if (ServiceProvider == null)
                {
                    throw new InvalidOperationException("Service provider not initialized");
                }

                // Get logger
                var logger = ServiceProvider.GetService<ILogger<App>>();
                logger?.LogInformation("Initializing MTM WIP Application services");

                // Initialize application state service
                var appStateService = ServiceProvider.GetService<IApplicationStateService>();
                if (appStateService != null)
                {
                    // Set initial connection status
                    appStateService.SetConnectionStatus(ConnectionStatus.Disconnected);
                    
                    // Load user preferences if available
                    // TODO: Load persisted state from storage
                    
                    logger?.LogInformation("Application state service initialized");
                }

                // Test database connection
                var databaseService = ServiceProvider.GetService<IDatabaseService>();
                if (databaseService != null)
                {
                    // Test connection in background (don't block startup)
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var connectionResult = await databaseService.TestConnectionAsync();
                            if (connectionResult.IsSuccess && connectionResult.Value)
                            {
                                appStateService?.SetConnectionStatus(ConnectionStatus.Connected);
                                logger?.LogInformation("Database connection established");
                            }
                            else
                            {
                                appStateService?.SetConnectionStatus(ConnectionStatus.Failed);
                                logger?.LogWarning("Database connection failed: {Error}", connectionResult.ErrorMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            appStateService?.SetConnectionStatus(ConnectionStatus.Failed);
                            logger?.LogError(ex, "Error testing database connection");
                        }
                    });
                }

                logger?.LogInformation("MTM WIP Application services initialized successfully");
            }
            catch (Exception ex)
            {
                // Log to console if logger service fails
                Console.WriteLine($"Failed to initialize services: {ex.Message}");
                
                // Try to get a logger and log the error
                try
                {
                    var logger = ServiceProvider?.GetService<ILogger<App>>();
                    logger?.LogCritical(ex, "Failed to initialize application services");
                }
                catch
                {
                    // If logger fails too, just continue
                    Console.WriteLine($"Failed to log initialization error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets a service from the dependency injection container.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The service instance, or null if not found</returns>
        public static T? GetService<T>() where T : class
        {
            return ServiceProvider?.GetService<T>();
        }

        /// <summary>
        /// Gets a required service from the dependency injection container.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The service instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if service is not found</exception>
        public static T GetRequiredService<T>() where T : class
        {
            if (ServiceProvider == null)
            {
                throw new InvalidOperationException("Service provider not initialized");
            }

            return ServiceProvider.GetRequiredService<T>();
        }
    }
}