using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Extensions;

namespace MTM_WIP_Application_Avalonia.Core.Startup;

/// <summary>
/// Simple test harness for the startup infrastructure
/// </summary>
public static class StartupTest
{
    public static async Task<bool> TestStartupInfrastructureAsync()
    {
        await Task.Yield(); // Ensure async execution
        
        try
        {
            Console.WriteLine("=== MTM WIP Application Startup Test ===");
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Starting startup infrastructure test...");

            // Test configuration loading
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Configuration loaded successfully");

            // Test service collection setup
            var services = new ServiceCollection();
            
            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Add configuration
            services.AddSingleton<IConfiguration>(configuration);

            // Test our MTM services registration
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Registering MTM services...");
            services.AddMTMServices(configuration);

            // Build service provider
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Building service provider...");
            using var serviceProvider = services.BuildServiceProvider();

            // Test service validation
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Testing service validation...");
            serviceProvider.ValidateRuntimeServices();

            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ✅ Startup infrastructure test completed successfully!");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ❌ Startup infrastructure test failed:");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }
}
