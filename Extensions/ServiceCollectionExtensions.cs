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
using MTM_WIP_Application_Avalonia.Services.UI;
using MTM_WIP_Application_Avalonia.Services.Infrastructure;
using MTM_WIP_Application_Avalonia.Services.Feature;

namespace MTM_WIP_Application_Avalonia.Extensions;

/// <summary>
/// Extension methods for registering MTM services with dependency injection.
/// Simplified and clean service registration for the MTM WIP Application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all MTM services to the service collection.
    /// Services are organized in folder-based structure with individual files.
    /// </summary>
    public static IServiceCollection AddMTMServices(this IServiceCollection services, IConfiguration configuration)
    {
        // CORE SERVICES (Services/Core/ folder - using folder-specific namespace)
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService, MTM_WIP_Application_Avalonia.Services.Core.ConfigurationService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService, MTM_WIP_Application_Avalonia.Services.Core.ApplicationStateService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService, MTM_WIP_Application_Avalonia.Services.Core.DatabaseService>();

        // BUSINESS SERVICES (Services/Business/ folder - using base namespace)
        services.TryAddSingleton<IMasterDataService, MasterDataService>();
        services.TryAddSingleton<IInventoryEditingService, InventoryEditingService>();
        services.TryAddSingleton<IQuickButtonsService, QuickButtonsService>();
        services.TryAddSingleton<IProgressService, ProgressService>();
        services.TryAddSingleton<IRemoveService, RemoveService>();

        // UI SERVICES (Services/UI/ folder - using folder-specific namespace)
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IThemeService, MTM_WIP_Application_Avalonia.Services.UI.ThemeService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IFocusManagementService, MTM_WIP_Application_Avalonia.Services.UI.FocusManagementService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ISuccessOverlayService, MTM_WIP_Application_Avalonia.Services.UI.SuccessOverlayService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ISuggestionOverlayService, MTM_WIP_Application_Avalonia.Services.UI.SuggestionOverlayService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ICustomDataGridService, MTM_WIP_Application_Avalonia.Services.UI.CustomDataGridService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IColumnConfigurationService, MTM_WIP_Application_Avalonia.Services.UI.ColumnConfigurationService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.VirtualPanelManager>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.SettingsPanelStateManager>();

        // NEW INDIVIDUAL UI SERVICES (from {Folder}.{Service}.cs files)
        services.TryAddScoped<Services.UI.IColumnConfigurationService, Services.UI.ColumnConfigurationService>();
        services.TryAddScoped<Services.UI.IFocusManagementService, Services.UI.FocusManagementService>();
        services.TryAddScoped<Services.UI.IThemeService, Services.UI.ThemeService>();

        // INFRASTRUCTURE SERVICES (Services/Infrastructure/ folder - using folder-specific namespace)
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileSelectionService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FileSelectionService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFilePathService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FilePathService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IPrintService, MTM_WIP_Application_Avalonia.Services.Infrastructure.PrintService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FileLoggingService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IEmergencyKeyboardHookService, MTM_WIP_Application_Avalonia.Services.Infrastructure.EmergencyKeyboardHookService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.INavigationService, MTM_WIP_Application_Avalonia.Services.Infrastructure.NavigationService>();

        // NEW INDIVIDUAL INFRASTRUCTURE SERVICES (from {Folder}.{Service}.cs files)
        services.TryAddSingleton<Services.Infrastructure.IEmergencyKeyboardHookService, Services.Infrastructure.EmergencyKeyboardHookService>();
        services.TryAddSingleton<Services.Infrastructure.IFileLoggingService, Services.Infrastructure.FileLoggingService>();
        services.TryAddSingleton<Services.Infrastructure.IFilePathService, Services.Infrastructure.FilePathService>();
        services.TryAddSingleton<Services.Infrastructure.IFileSelectionService, Services.Infrastructure.FileSelectionService>();
        services.TryAddSingleton<Services.Infrastructure.IMTMFileLoggerProvider, Services.Infrastructure.MTMFileLoggerProvider>();
        services.TryAddSingleton<Services.Infrastructure.INavigationService, Services.Infrastructure.NavigationService>();
        services.TryAddSingleton<Services.Infrastructure.IPrintService, Services.Infrastructure.PrintService>();

        // FEATURE SERVICES (Services/Feature/ folder - using folder-specific namespace)
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Feature.ISettingsService, MTM_WIP_Application_Avalonia.Services.Feature.SettingsService>();
        services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Feature.IUniversalOverlayService, MTM_WIP_Application_Avalonia.Services.Feature.UniversalOverlayService>();
        // Note: StartupDialog is a static class, not a service

        // Logging services using Infrastructure services
        services.AddLogging(builder =>
        {
            // Add custom file logging provider
            builder.Services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                var fileLoggingService = serviceProvider.GetRequiredService<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService>();
                return new MTM_WIP_Application_Avalonia.Services.Infrastructure.MTMFileLoggerProvider(fileLoggingService);
            });
        });

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

        // Overlay ViewModels - using Lazy<T> for MainWindow integration
        services.TryAddTransient<NewQuickButtonOverlayViewModel>();
        services.TryAddTransient<ConfirmationOverlayViewModel>();
        services.TryAddTransient<EditInventoryViewModel>();
        services.TryAddTransient<ConnectionStatusOverlayViewModel>();
        services.TryAddTransient<EmergencyShutdownOverlayViewModel>();
        services.TryAddTransient<ThemeQuickSwitcherOverlayViewModel>();
        services.TryAddTransient<FieldValidationOverlayViewModel>();
        services.TryAddTransient<ViewModels.Overlay.ProgressOverlayViewModel>();

        // Lazy overlay registrations for MainWindow integration
        services.TryAddTransient<Lazy<ConnectionStatusOverlayViewModel>>(provider =>
            new Lazy<ConnectionStatusOverlayViewModel>(() => provider.GetRequiredService<ConnectionStatusOverlayViewModel>()));
        services.TryAddTransient<Lazy<EmergencyShutdownOverlayViewModel>>(provider =>
            new Lazy<EmergencyShutdownOverlayViewModel>(() => provider.GetRequiredService<EmergencyShutdownOverlayViewModel>()));
        services.TryAddTransient<Lazy<ThemeQuickSwitcherOverlayViewModel>>(provider =>
            new Lazy<ThemeQuickSwitcherOverlayViewModel>(() => provider.GetRequiredService<ThemeQuickSwitcherOverlayViewModel>()));
        services.TryAddTransient<Lazy<FieldValidationOverlayViewModel>>(provider =>
            new Lazy<FieldValidationOverlayViewModel>(() => provider.GetRequiredService<FieldValidationOverlayViewModel>()));
        services.TryAddTransient<Lazy<ViewModels.Overlay.ProgressOverlayViewModel>>(provider =>
            new Lazy<ViewModels.Overlay.ProgressOverlayViewModel>(() => provider.GetRequiredService<ViewModels.Overlay.ProgressOverlayViewModel>()));

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

            // Core Services (use Core namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService),
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService),
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService),

            // UI Services (use UI namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.UI.IThemeService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.IFocusManagementService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.ISuccessOverlayService),

            // Infrastructure Services (use Infrastructure namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFilePathService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileSelectionService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IPrintService),

            // Business Services (use base Services namespace)
            typeof(IMasterDataService),
            typeof(IRemoveService),
            typeof(IInventoryEditingService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),

            // Feature Services
            typeof(MTM_WIP_Application_Avalonia.Services.Feature.ISettingsService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.ISuggestionOverlayService)
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
            // Core Services (use Core namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService),
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService),
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService),

            // UI Services (use UI namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.UI.IThemeService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.IFocusManagementService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.ISuccessOverlayService),

            // Infrastructure Services (use Infrastructure namespace)
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFilePathService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileSelectionService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IPrintService),

            // Business Services (use base Services namespace)
            typeof(IMasterDataService),
            typeof(IRemoveService),
            typeof(IInventoryEditingService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),

            // Feature Services
            typeof(MTM_WIP_Application_Avalonia.Services.Feature.ISettingsService),
            typeof(MTM_WIP_Application_Avalonia.Services.UI.ISuggestionOverlayService)
        };

        var failedServices = new List<string>();

        foreach (var serviceType in criticalServices)
        {
            try
            {
                // Just test resolution - don't need to store the result
                serviceProvider.GetRequiredService(serviceType);
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

