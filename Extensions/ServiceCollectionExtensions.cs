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
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ServiceCollectionExtensions.AddMTMServices() started");

        try
        {
            // CORE SERVICES (Services/Core/ folder - using folder-specific namespace)
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Registering Core services...");
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService, MTM_WIP_Application_Avalonia.Services.Core.ConfigurationService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService, MTM_WIP_Application_Avalonia.Services.Core.ApplicationStateService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService, MTM_WIP_Application_Avalonia.Services.Core.DatabaseService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Core.IApplicationHealthService, MTM_WIP_Application_Avalonia.Services.Core.ApplicationHealthService>();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Core services registered successfully");

            // BUSINESS SERVICES (Services/Business/ folder - using base namespace)
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Registering Business services...");
            services.TryAddSingleton<IMasterDataService, MasterDataService>();
            services.TryAddSingleton<IInventoryEditingService, InventoryEditingService>();
            services.TryAddSingleton<IQuickButtonsService, QuickButtonsService>();
            services.TryAddSingleton<IProgressService, ProgressService>();
            services.TryAddSingleton<IRemoveService, RemoveService>();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Business services registered successfully");
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IThemeService, MTM_WIP_Application_Avalonia.Services.UI.ThemeService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IFocusManagementService, MTM_WIP_Application_Avalonia.Services.UI.FocusManagementService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ISuccessOverlayService, MTM_WIP_Application_Avalonia.Services.UI.SuccessOverlayService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ISuggestionOverlayService, MTM_WIP_Application_Avalonia.Services.UI.SuggestionOverlayService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ICustomDataGridService, MTM_WIP_Application_Avalonia.Services.UI.CustomDataGridService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IColumnConfigurationService, MTM_WIP_Application_Avalonia.Services.UI.ColumnConfigurationService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.ISettingsPanelStateManager, MTM_WIP_Application_Avalonia.Services.UI.SettingsPanelStateManager>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.UI.IVirtualPanelManager, MTM_WIP_Application_Avalonia.Services.UI.VirtualPanelManager>();

            // INFRASTRUCTURE SERVICES (Services/Infrastructure/ folder - using folder-specific namespace)
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileSelectionService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FileSelectionService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFilePathService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FilePathService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IPrintService, MTM_WIP_Application_Avalonia.Services.Infrastructure.PrintService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService, MTM_WIP_Application_Avalonia.Services.Infrastructure.FileLoggingService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IEmergencyKeyboardHookService, MTM_WIP_Application_Avalonia.Services.Infrastructure.EmergencyKeyboardHookService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.INavigationService, MTM_WIP_Application_Avalonia.Services.Infrastructure.NavigationService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Infrastructure.IMTMFileLoggerProvider, MTM_WIP_Application_Avalonia.Services.Infrastructure.MTMFileLoggerProvider>();

            // FEATURE SERVICES (Services/Feature/ folder - using folder-specific namespace)
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Feature.ISettingsService, MTM_WIP_Application_Avalonia.Services.Feature.SettingsService>();
            services.TryAddSingleton<MTM_WIP_Application_Avalonia.Services.Feature.IUniversalOverlayService, MTM_WIP_Application_Avalonia.Services.Feature.UniversalOverlayService>();
            // Note: StartupDialog is a static class, not a service

            // Logging services using Infrastructure services
            // TODO: Fix circular dependency - FileLoggingService needs ILogger but we're registering ILoggerProvider
            // Temporarily disable custom file logging to prevent startup deadlock
            /*
            services.AddLogging(builder =>
            {
                // Add custom file logging provider
                builder.Services.AddSingleton<ILoggerProvider>(serviceProvider =>
                {
                    var fileLoggingService = serviceProvider.GetRequiredService<MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService>();
                    return new MTM_WIP_Application_Avalonia.Services.Infrastructure.MTMFileLoggerProvider(fileLoggingService);
                });
            });
            */

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
            services.TryAddTransient<ShortcutsViewModel>();
            services.TryAddTransient<AboutViewModel>();

            // Additional SettingsForm ViewModels
            services.TryAddTransient<SystemHealthViewModel>();
            services.TryAddTransient<BackupRecoveryViewModel>();
            services.TryAddTransient<SecurityPermissionsViewModel>();

            stopwatch.Stop();
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] All services registered successfully in {stopwatch.ElapsedMilliseconds}ms");

            return services;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Service registration failed: {ex.Message}");
            throw new InvalidOperationException($"Service registration failed: {ex.Message}", ex);
        }
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

    /// <summary>
    /// Validates critical services one by one to identify problematic services.
    /// Performs selective testing to avoid circular dependency issues.
    /// </summary>
    public static void ValidateSelectiveRuntimeServices(this IServiceProvider serviceProvider)
    {
        var criticalServices = new[]
        {
            // Test Core Services first (most fundamental)
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IConfigurationService),
            typeof(MTM_WIP_Application_Avalonia.Services.Core.IApplicationStateService),

            // Test Infrastructure Services (no complex dependencies)
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFilePathService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileLoggingService),
            typeof(MTM_WIP_Application_Avalonia.Services.Infrastructure.IFileSelectionService),

            // Test simpler UI Services
            typeof(MTM_WIP_Application_Avalonia.Services.UI.IThemeService),

            // Test Business Services (may have more complex dependencies)
            typeof(IMasterDataService),
            typeof(IRemoveService),
            typeof(IInventoryEditingService),

            // Skip complex services that might cause issues
            // typeof(MTM_WIP_Application_Avalonia.Services.Core.IDatabaseService),
            // typeof(MTM_WIP_Application_Avalonia.Services.UI.IFocusManagementService),
            // typeof(MTM_WIP_Application_Avalonia.Services.UI.ISuccessOverlayService),
        };

        foreach (var serviceType in criticalServices)
        {
            try
            {
                Console.WriteLine($"[VALIDATION-TEST] Testing {serviceType.Name}...");
                var service = serviceProvider.GetRequiredService(serviceType);
                Console.WriteLine($"[VALIDATION-SUCCESS] {serviceType.Name} resolved successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VALIDATION-ERROR] {serviceType.Name}: {ex.Message}");
                // Don't throw - just log and continue to identify all problematic services
            }
        }
    }
}

