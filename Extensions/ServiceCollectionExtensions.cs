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
        // CONSOLIDATED CORE SERVICES (Services/Core/CoreServices.cs)
        services.TryAddSingleton<IConfigurationService, Core.ConfigurationService>();
        services.TryAddSingleton<IApplicationStateService, Core.ApplicationStateService>();
        services.TryAddSingleton<IDatabaseService, Core.DatabaseService>();

        // CONSOLIDATED BUSINESS SERVICES (Services/Business/BusinessServices.cs)
        services.TryAddSingleton<Business.IMasterDataService, Business.MasterDataService>();
        services.TryAddSingleton<Business.IRemoveService, Business.RemoveService>();
        services.TryAddSingleton<Business.IInventoryEditingService, Business.InventoryEditingService>();

        // CONSOLIDATED UI SERVICES (Services/UI/UIServices.cs)
        services.TryAddSingleton<UI.INavigationService, UI.NavigationService>();
        services.TryAddSingleton<UI.IThemeService, UI.ThemeService>();
        services.TryAddSingleton<UI.IFocusManagementService, UI.FocusManagementService>();
        services.TryAddSingleton<UI.ISuccessOverlayService, UI.SuccessOverlayService>();

        // CONSOLIDATED INFRASTRUCTURE SERVICES (Services/Infrastructure/InfrastructureServices.cs)
        services.TryAddSingleton<Infrastructure.IFileSelectionService, Infrastructure.FileSelectionService>();
        services.TryAddSingleton<Infrastructure.IFilePathService, Infrastructure.FilePathService>();
        services.TryAddSingleton<Infrastructure.IPrintService, Infrastructure.PrintService>();
        services.TryAddSingleton<Infrastructure.IFileLoggingService, Infrastructure.FileLoggingService>();
        services.TryAddSingleton<Infrastructure.IEmergencyKeyboardHookService, Infrastructure.EmergencyKeyboardHookService>();

        // Logging services using consolidated Infrastructure
        services.AddLogging(builder =>
        {
            // Add custom file logging provider
            builder.Services.AddSingleton<ILoggerProvider>(serviceProvider =>
            {
                var fileLoggingService = serviceProvider.GetRequiredService<Infrastructure.IFileLoggingService>();
                return new MTMFileLoggerProvider(fileLoggingService);
            });
        });

        // FEATURE SERVICES (kept separate as per plan)
        services.TryAddSingleton<ISettingsService, SettingsService>();
        services.TryAddSingleton<IQuickButtonsService, QuickButtonsService>();
        services.TryAddSingleton<IProgressService, ProgressService>();

        // UI PANEL AND OVERLAY SERVICES (Services root - to be consolidated later)
        services.TryAddSingleton<ISuggestionOverlayService, SuggestionOverlayService>();
        services.TryAddSingleton<VirtualPanelManager>();
        services.TryAddSingleton<SettingsPanelStateManager>();
        services.TryAddSingleton<ICustomDataGridService, CustomDataGridService>();
        services.TryAddSingleton<IColumnConfigurationService, ColumnConfigurationService>();

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
            typeof(IDatabaseService),
            typeof(UI.INavigationService),
            typeof(UI.IThemeService),
            typeof(UI.IFocusManagementService),
            typeof(UI.ISuccessOverlayService),
            typeof(Infrastructure.IFilePathService),
            typeof(Infrastructure.IFileLoggingService),
            typeof(Infrastructure.IFileSelectionService),
            typeof(Infrastructure.IPrintService),
            typeof(Business.IMasterDataService),
            typeof(Business.IRemoveService),
            typeof(Business.IInventoryEditingService),
            typeof(ISettingsService),
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
            typeof(IDatabaseService),
            typeof(UI.INavigationService),
            typeof(UI.IThemeService),
            typeof(UI.IFocusManagementService),
            typeof(UI.ISuccessOverlayService),
            typeof(Infrastructure.IFilePathService),
            typeof(Infrastructure.IFileLoggingService),
            typeof(Infrastructure.IFileSelectionService),
            typeof(Infrastructure.IPrintService),
            typeof(Business.IMasterDataService),
            typeof(Business.IRemoveService),
            typeof(Business.IInventoryEditingService),
            typeof(ISettingsService),
            typeof(IQuickButtonsService),
            typeof(IProgressService),
            typeof(ISuggestionOverlayService)
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

