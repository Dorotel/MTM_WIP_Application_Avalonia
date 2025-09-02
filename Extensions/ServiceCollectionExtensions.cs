using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_Shared_Logic.Extensions;

/// <summary>
/// Extension methods for registering MTM services with dependency injection.
/// Simplified and clean service registration for the MTM WIP Application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all MTM services to the service collection.
    /// Clean, simple registration of only the services that exist and work.
    /// </summary>
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core infrastructure services
        services.TryAddSingleton<IConfigurationService, ConfigurationService>();
        services.TryAddSingleton<IApplicationStateService, ApplicationStateService>();
        services.TryAddSingleton<INavigationService, NavigationService>();
        
        // Theme and Settings services
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<ISettingsService, SettingsService>();
        
        // SettingsForm services
        services.TryAddSingleton<VirtualPanelManager>();
        services.TryAddSingleton<SettingsPanelStateManager>();
        
        // Database services
        services.TryAddScoped<IDatabaseService, DatabaseService>();
        
        // UI and Application services
        services.TryAddScoped<IQuickButtonsService, QuickButtonsService>();
        services.TryAddSingleton<IProgressService, ProgressService>();
        
        // Register SuggestionOverlay service
        services.TryAddScoped<ISuggestionOverlayService, SuggestionOverlayService>();
        
        // ViewModels - register only those that exist and compile
        services.TryAddTransient<MainWindowViewModel>();
        services.TryAddTransient<MainViewViewModel>();
        services.TryAddTransient<InventoryTabViewModel>();
        services.TryAddTransient<AdvancedRemoveViewModel>();
        services.TryAddTransient<AddItemViewModel>();
        services.TryAddTransient<RemoveItemViewModel>();
        services.TryAddTransient<TransferItemViewModel>();
        services.TryAddTransient<AdvancedInventoryViewModel>();
        services.TryAddTransient<QuickButtonsViewModel>();
        services.TryAddTransient<SettingsViewModel>();
        
        // SettingsForm ViewModels
        services.TryAddTransient<SettingsFormViewModel>();
        services.TryAddTransient<DatabaseSettingsViewModel>();
        services.TryAddTransient<AddUserViewModel>();
        services.TryAddTransient<EditUserViewModel>();
        services.TryAddTransient<DeleteUserViewModel>();
        services.TryAddTransient<AddPartViewModel>();
        services.TryAddTransient<EditPartViewModel>();
        services.TryAddTransient<RemovePartViewModel>();
        services.TryAddTransient<AddOperationViewModel>();
        services.TryAddTransient<EditOperationViewModel>();
        services.TryAddTransient<RemoveOperationViewModel>();
        services.TryAddTransient<AddLocationViewModel>();
        services.TryAddTransient<EditLocationViewModel>();
        services.TryAddTransient<RemoveLocationViewModel>();
        services.TryAddTransient<AddItemTypeViewModel>();
        services.TryAddTransient<EditItemTypeViewModel>();
        services.TryAddTransient<RemoveItemTypeViewModel>();
        services.TryAddTransient<ThemeBuilderViewModel>();
        services.TryAddTransient<ShortcutsViewModel>();
        services.TryAddTransient<AboutViewModel>();
        
        // Additional SettingsForm ViewModels
        services.TryAddTransient<SystemHealthViewModel>();
        services.TryAddTransient<BackupRecoveryViewModel>();
        services.TryAddTransient<SecurityPermissionsViewModel>();

        return services;
    }

    /// <summary>
    /// Extension methods for TryAdd functionality to prevent duplicate registrations
    /// </summary>
    public static IServiceCollection TryAddTransient<TService>(this IServiceCollection services)
        where TService : class
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddTransient<TService>();
        }
        return services;
    }

    public static IServiceCollection TryAddSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddSingleton<TService, TImplementation>();
        }
        return services;
    }

    public static IServiceCollection TryAddScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        if (!services.Any(x => x.ServiceType == typeof(TService)))
        {
            services.AddScoped<TService, TImplementation>();
        }
        return services;
    }

    /// <summary>
    /// Validates that all critical MTM services are properly registered.
    /// Only checks for services that actually exist.
    /// </summary>
    public static IServiceCollection ValidateMTMServices(this IServiceCollection services)
    {
        var requiredServices = new[]
        {
            typeof(IConfiguration),
            typeof(ILoggerFactory),
            typeof(IConfigurationService),
            typeof(IApplicationStateService),
            typeof(INavigationService),
            typeof(IThemeService),
            typeof(ISettingsService),
            typeof(IDatabaseService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),
            typeof(ISuggestionOverlayService)
        };

        var missingServices = requiredServices
            .Where(serviceType => !services.Any(x => x.ServiceType == serviceType))
            .Select(serviceType => serviceType.Name)
            .ToList();

        if (missingServices.Count > 0)
        {
            var errorMessage = $"Missing required services: {string.Join(", ", missingServices)}";
            throw new InvalidOperationException(errorMessage);
        }

        return services;
    }

    /// <summary>
    /// Tests service resolution at runtime to identify dependency issues.
    /// Only tests services that should actually exist.
    /// </summary>
    public static void ValidateRuntimeServices(this IServiceProvider serviceProvider)
    {
        var criticalServices = new[]
        {
            typeof(IConfigurationService),
            typeof(IApplicationStateService),
            typeof(INavigationService),
            typeof(IThemeService),
            typeof(ISettingsService),
            typeof(IDatabaseService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),
            typeof(ISuggestionOverlayService)
        };

        var failedServices = criticalServices
            .Where(serviceType => 
            {
                try
                {
                    serviceProvider.GetRequiredService(serviceType);
                    return false;
                }
                catch
                {
                    return true;
                }
            })
            .Select(serviceType => serviceType.Name)
            .ToList();

        if (failedServices.Count > 0)
        {
            var errorMessage = $"Failed to resolve critical services: {string.Join(", ", failedServices)}";
            throw new InvalidOperationException(errorMessage);
        }
    }
}

