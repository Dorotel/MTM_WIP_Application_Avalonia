using System;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Interfaces;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_Shared_Logic.Extensions;

namespace MTM_WIP_Application_Avalonia;

public static class Program
{
    private static IServiceProvider? _serviceProvider;

    static Program()
    {
        // Enhanced assembly resolver to prevent ReactiveUI platform-specific loading issues
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var assemblyName = new AssemblyName(args.Name);
            
            // Redirect all platform-specific ReactiveUI assemblies to the main ReactiveUI assembly
            if (assemblyName.Name is "ReactiveUI.XamForms" or "ReactiveUI.Winforms" or "ReactiveUI.WinForms" or "ReactiveUI.WinUI" or "ReactiveUI.Maui")
            {
                Console.WriteLine($"Redirecting {assemblyName.Name} to ReactiveUI assembly");
                return typeof(ReactiveUI.ReactiveObject).Assembly;
            }
            
            // Handle version mismatches for ReactiveUI
            if (assemblyName.Name == "ReactiveUI" && assemblyName.Version != null)
            {
                var reactiveUIAssembly = typeof(ReactiveUI.ReactiveObject).Assembly;
                Console.WriteLine($"Resolving ReactiveUI version {assemblyName.Version} to {reactiveUIAssembly.GetName().Version}");
                return reactiveUIAssembly;
            }
            
            // Handle System.Reactive version conflicts
            if (assemblyName.Name == "System.Reactive" && assemblyName.Version != null)
            {
                var systemReactiveAssembly = typeof(System.Reactive.Linq.Observable).Assembly;
                Console.WriteLine($"Resolving System.Reactive version {assemblyName.Version} to {systemReactiveAssembly.GetName().Version}");
                return systemReactiveAssembly;
            }
            
            return null;
        };
    }

    public static void Main(string[] args) 
    {
        try
        {
            // Setup dependency injection before starting the application
            ConfigureServices();
            
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application startup failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Check for assembly loading issues
            if (ex is FileNotFoundException || ex is ReflectionTypeLoadException)
            {
                Console.WriteLine("This appears to be an assembly loading issue.");
                Console.WriteLine("Check the ReactiveUI package versions and assembly bindings.");
                
                // Log loaded assemblies for debugging
                Console.WriteLine("\nCurrently loaded assemblies:");
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName?.Contains("ReactiveUI") == true || 
                        assembly.FullName?.Contains("System.Reactive") == true)
                    {
                        Console.WriteLine($"  - {assembly.FullName}");
                    }
                }
            }
            
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void ConfigureServices()
    {
        try
        {
            var services = new ServiceCollection();

            // Configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Config/appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            // ✅ CRITICAL: Use comprehensive MTM service registration
            // This registers ALL required services including ViewModels
            services.AddMTMServices(configuration);

            // Note: No need to register services again - AddMTMServices handles everything
            // The following registrations would cause conflicts and are now handled by AddMTMServices:
            // - IConfigurationService, INavigationService, IApplicationStateService (already registered with TryAdd)
            // - All ViewModels (already registered with TryAdd)

            // Build service provider with validation
            _serviceProvider = services.BuildServiceProvider();

            // Validate that all critical services can be resolved
            _serviceProvider.ValidateRuntimeServices();

            // Get logger using the App class instead of static Program class
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("MTM_WIP_Application_Avalonia.Program");
            logger?.LogInformation("Service provider configured and validated successfully");
            logger?.LogInformation($"Total services registered: {services.Count}");
            
            // Log ReactiveUI assembly information for debugging
            var reactiveUIAssembly = typeof(ReactiveUI.ReactiveObject).Assembly;
            logger?.LogInformation($"ReactiveUI Assembly: {reactiveUIAssembly.GetName().FullName}");
            logger?.LogInformation($"ReactiveUI Location: {reactiveUIAssembly.Location}");
            
            var systemReactiveAssembly = typeof(System.Reactive.Linq.Observable).Assembly;
            logger?.LogInformation($"System.Reactive Assembly: {systemReactiveAssembly.GetName().FullName}");
            logger?.LogInformation($"System.Reactive Location: {systemReactiveAssembly.Location}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Service configuration failed: {ex.Message}");
            Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Enhanced error reporting for assembly issues
            if (ex is FileNotFoundException fileNotFound)
            {
                Console.WriteLine($"Missing file: {fileNotFound.FileName}");
                Console.WriteLine("This is likely a package reference or assembly binding issue.");
                
                // Check if it's a ReactiveUI platform-specific assembly
                if (fileNotFound.FileName?.Contains("ReactiveUI.") == true)
                {
                    Console.WriteLine("This appears to be a ReactiveUI platform-specific assembly conflict.");
                    Console.WriteLine("The assembly resolver should handle this, but there may be a deeper dependency issue.");
                }
            }
            
            throw;
        }
    }

    /// <summary>
    /// Get service instance from DI container
    /// </summary>
    public static T GetService<T>() where T : notnull
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured. Call ConfigureServices first.");

        try
        {
            return _serviceProvider.GetRequiredService<T>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to resolve service {typeof(T).Name}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Get service instance with optional fallback
    /// </summary>
    public static T? GetOptionalService<T>() where T : class
    {
        try
        {
            return _serviceProvider?.GetService<T>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to resolve optional service {typeof(T).Name}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Create scope for scoped services (useful for operations that need isolated service instances)
    /// </summary>
    public static IServiceScope CreateScope()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException("Service provider not configured.");

        return _serviceProvider.CreateScope();
    }
}
