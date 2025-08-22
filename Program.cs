using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Avalonia;
using Avalonia.ReactiveUI;
using MTM.Extensions;

namespace MTM_WIP_Application_Avalonia
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Build service container
            var services = new ServiceCollection();
            
            // Add configuration
            services.AddSingleton<IConfiguration>(configuration);
            
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(configuration.GetSection("Logging"));
                builder.AddConsole();
            });

            // Add MTM services based on environment
            var environment = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";
            if (environment == "Development")
            {
                services.AddMTMServicesForDevelopment(configuration);
            }
            else
            {
                services.AddMTMServicesForProduction(configuration);
            }

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            // Store service provider for use in App
            App.ServiceProvider = serviceProvider;

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()   
                .UseReactiveUI(); // Enable ReactiveUI integration
        }
    }
}
