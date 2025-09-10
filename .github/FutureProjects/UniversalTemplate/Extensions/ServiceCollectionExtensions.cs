using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YourProject.Extensions
{
    /// <summary>
    /// Universal service collection extensions for .NET 8 Avalonia applications.
    /// Provides standardized patterns for dependency injection configuration.
    /// Extracted from MTM WIP Application for reuse in new projects.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core application services with standard lifetime management.
        /// Customize service registrations based on your application's needs.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Core Services (Singleton - Application lifetime)
            services.TryAddSingleton<IConfigurationService, ConfigurationService>();
            services.TryAddSingleton<IThemeService, ThemeService>();
            services.TryAddSingleton<INavigationService, NavigationService>();
            
            // Application State Services (Singleton - Shared state)
            services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
            
            // Data Services (Scoped - Per operation lifetime)
            // TODO: Add your data services here
            // services.TryAddScoped<IDataService, DataService>();
            
            // Business Services (Scoped - Per operation lifetime)
            // TODO: Add your business services here
            // services.TryAddScoped<IBusinessService, BusinessService>();
            
            // Transient Services (New instance each time)
            // TODO: Add services that should be created fresh each time
            // services.TryAddTransient<ITransientService, TransientService>();
            
            return services;
        }

        /// <summary>
        /// Adds ViewModels with proper lifetime management.
        /// All ViewModels are registered as Transient for proper disposal.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            // TODO: Register your ViewModels here
            // services.TryAddTransient<MainWindowViewModel>();
            // services.TryAddTransient<ExampleViewModel>();
            
            return services;
        }

        /// <summary>
        /// Adds Views with proper lifetime management.
        /// Views are typically registered as Transient.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            // TODO: Register your Views here if using DI for view creation
            // services.TryAddTransient<MainWindow>();
            // services.TryAddTransient<ExampleView>();
            
            return services;
        }

        /// <summary>
        /// Configures logging with standard settings for Avalonia applications.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">Application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddLogging(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration(configuration.GetSection("Logging"));
                
                // Add console logging for development
                builder.AddConsole();
                
                // Add debug output for development
                builder.AddDebug();
                
                // TODO: Add additional logging providers as needed
                // builder.AddFile("logs/app.log"); // Requires additional package
            });
            
            return services;
        }

        /// <summary>
        /// Extension method to safely try adding services that may already be registered.
        /// Prevents duplicate registration exceptions.
        /// </summary>
        private static IServiceCollection TryAddSingleton<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (services.Any(s => s.ServiceType == typeof(TInterface)))
                return services;
                
            return services.AddSingleton<TInterface, TImplementation>();
        }

        /// <summary>
        /// Extension method to safely try adding scoped services that may already be registered.
        /// </summary>
        private static IServiceCollection TryAddScoped<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (services.Any(s => s.ServiceType == typeof(TInterface)))
                return services;
                
            return services.AddScoped<TInterface, TImplementation>();
        }

        /// <summary>
        /// Extension method to safely try adding transient services that may already be registered.
        /// </summary>
        private static IServiceCollection TryAddTransient<TService>(
            this IServiceCollection services)
            where TService : class
        {
            if (services.Any(s => s.ServiceType == typeof(TService)))
                return services;
                
            return services.AddTransient<TService>();
        }

        /// <summary>
        /// Extension method to safely try adding transient services with interface.
        /// </summary>
        private static IServiceCollection TryAddTransient<TInterface, TImplementation>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            if (services.Any(s => s.ServiceType == typeof(TInterface)))
                return services;
                
            return services.AddTransient<TInterface, TImplementation>();
        }
    }

    // TODO: Define your service interfaces here
    
    /// <summary>
    /// Interface for configuration service
    /// </summary>
    public interface IConfigurationService
    {
        string GetConnectionString(string name);
        T GetSection<T>(string sectionName) where T : new();
        string GetValue(string key, string defaultValue = "");
    }

    /// <summary>
    /// Interface for theme service
    /// </summary>
    public interface IThemeService
    {
        string CurrentTheme { get; }
        void SetTheme(string themeName);
        event EventHandler<string>? ThemeChanged;
    }

    /// <summary>
    /// Interface for navigation service
    /// </summary>
    public interface INavigationService
    {
        Task NavigateToAsync<TViewModel>() where TViewModel : class;
        Task NavigateToAsync<TView, TViewModel>() where TView : class where TViewModel : class;
        bool CanGoBack { get; }
        Task GoBackAsync();
    }

    /// <summary>
    /// Interface for application state service
    /// </summary>
    public interface IApplicationStateService
    {
        bool IsInitialized { get; }
        string CurrentUser { get; }
        event EventHandler<string>? StateChanged;
    }

    // TODO: Implement the actual service classes
    // These are placeholder implementations - replace with your actual implementations
    
    internal class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        
        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string GetConnectionString(string name) => 
            _configuration.GetConnectionString(name) ?? string.Empty;
            
        public T GetSection<T>(string sectionName) where T : new() =>
            _configuration.GetSection(sectionName).Get<T>() ?? new T();
            
        public string GetValue(string key, string defaultValue = "") =>
            _configuration.GetValue(key, defaultValue);
    }
    
    internal class ThemeService : IThemeService
    {
        public string CurrentTheme { get; private set; } = "Default";
        public event EventHandler<string>? ThemeChanged;
        
        public void SetTheme(string themeName)
        {
            CurrentTheme = themeName;
            ThemeChanged?.Invoke(this, themeName);
        }
    }
    
    internal class NavigationService : INavigationService
    {
        public bool CanGoBack => false; // TODO: Implement navigation history
        
        public Task NavigateToAsync<TViewModel>() where TViewModel : class
        {
            // TODO: Implement navigation logic
            return Task.CompletedTask;
        }
        
        public Task NavigateToAsync<TView, TViewModel>() where TView : class where TViewModel : class
        {
            // TODO: Implement navigation logic
            return Task.CompletedTask;
        }
        
        public Task GoBackAsync()
        {
            // TODO: Implement back navigation
            return Task.CompletedTask;
        }
    }
    
    internal class ApplicationStateService : IApplicationStateService
    {
        public bool IsInitialized { get; private set; } = false;
        public string CurrentUser { get; private set; } = Environment.UserName;
        public event EventHandler<string>? StateChanged;
    }
}