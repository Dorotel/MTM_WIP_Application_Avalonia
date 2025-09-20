using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.SettingsForm;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Services.Core;
using MTM_WIP_Application_Avalonia.Services.Business;

namespace MTM_WIP_Application_Avalonia.Extensions;

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
        services.TryAddSingleton<IFilePathService, FilePathService>();
        
        // Logging services
        services.TryAddSingleton<IFileLoggingService, FileLoggingService>();
        services.AddLogging(builder =>
        {
            // Add custom file logging provider
            builder.Services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                var fileLoggingService = serviceProvider.GetRequiredService<IFileLoggingService>();
                return new MTMFileLoggerProvider(fileLoggingService);
            });
        });
        
        // Theme and Settings services
        services.TryAddSingleton<IThemeService, ThemeService>();
        services.TryAddSingleton<ISettingsService, SettingsService>();
        
        // SettingsForm services
        services.TryAddSingleton<VirtualPanelManager>();
        services.TryAddSingleton<SettingsPanelStateManager>();
        
        // Database services
        services.TryAddSingleton<IDatabaseService, DatabaseService>();
        
        // UI and Application services
        services.TryAddSingleton<IQuickButtonsService, QuickButtonsService>();
        services.TryAddSingleton<IProgressService, ProgressService>();
        
        // Register SuggestionOverlay service - change to singleton for validation
        services.TryAddSingleton<ISuggestionOverlayService, SuggestionOverlayService>();
        
        // Register SuccessOverlay service - singleton for shared access across ViewModels
        services.TryAddSingleton<ISuccessOverlayService, SuccessOverlayService>();
        
        // Register Master Data service - singleton for shared access across ViewModels
        services.TryAddSingleton<IMasterDataService, Business.MasterDataService>();
        
        // Register Focus Management service - singleton for application-wide focus management
        services.TryAddSingleton<IFocusManagementService, FocusManagementService>();
        

        // Register Print service - singleton for shared access across ViewModels
        services.TryAddSingleton<IPrintService, PrintService>();

        // Register Remove service - singleton for centralized inventory removal business logic
        services.TryAddSingleton<IRemoveService, RemoveService>();

        // Register CustomDataGrid service - singleton for shared data grid functionality across views
        services.TryAddSingleton<ICustomDataGridService, CustomDataGridService>();
        
        // Register File Selection service - singleton for unified file operations across application
        services.TryAddSingleton<IFileSelectionService, FileSelectionService>();
        
        // Register Inventory Editing service - singleton for comprehensive inventory editing operations
        services.TryAddSingleton<IInventoryEditingService, InventoryEditingService>();
        
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
        services.TryAddTransient<ThemeEditorViewModel>();
        
        // Print ViewModels
        services.TryAddTransient<PrintViewModel>();
        services.TryAddTransient<PrintLayoutControlViewModel>();
        
        // Overlay ViewModels  
        services.TryAddTransient<NewQuickButtonOverlayViewModel>();
        services.TryAddTransient<NoteEditorViewModel>();
        services.TryAddTransient<ConfirmationOverlayViewModel>();
        services.TryAddTransient<EditInventoryViewModel>();
        
        // SettingsForm ViewModels
        services.TryAddTransient<SettingsViewModel>();
        services.TryAddTransient<DatabaseSettingsViewModel>();
        services.TryAddTransient<AddUserViewModel>();
        services.TryAddTransient<EditUserViewModel>();
        services.TryAddTransient<RemoveUserViewModel>();
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
            typeof(IFilePathService),
            typeof(IThemeService),
            typeof(ISettingsService),
            typeof(IDatabaseService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),
            typeof(ISuggestionOverlayService),
            typeof(IMasterDataService),
            typeof(IFileLoggingService),
            typeof(IFocusManagementService),
            typeof(IRemoveService),
            typeof(IFileSelectionService)
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
            typeof(IFilePathService),
            typeof(IThemeService),
            typeof(ISettingsService),
            typeof(IDatabaseService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),
            typeof(ISuggestionOverlayService),
            typeof(IMasterDataService),
            typeof(IFileLoggingService),
            typeof(IFocusManagementService),
            typeof(IRemoveService),
            typeof(IFileSelectionService)
        };

        var failedServices = new List<string>();

        foreach (var serviceType in criticalServices)
        {
            try
            {
                var service = serviceProvider.GetRequiredService(serviceType);
                Console.WriteLine($"[VALIDATION-SUCCESS] {serviceType.Name} resolved successfully");
            }
            catch (Exception ex)
            {
                var errorDetail = $"{serviceType.Name}: {ex.Message}";
                failedServices.Add(errorDetail);
                Console.WriteLine($"[VALIDATION-ERROR] {errorDetail}");
            }
        }

        if (failedServices.Count > 0)
        {
            var errorMessage = $"Failed to resolve critical services:{Environment.NewLine}{string.Join(Environment.NewLine, failedServices)}";
            throw new InvalidOperationException(errorMessage);
        }
    }
}

