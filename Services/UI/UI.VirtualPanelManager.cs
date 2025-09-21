using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.ViewModels;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Views;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

namespace MTM_WIP_Application_Avalonia.Services.UI;


#region Virtual Panel Manager Service

/// <summary>
/// Interface for virtual panel management with adaptive performance-based panel creation.
/// </summary>
public interface IVirtualPanelManager
{
    /// <summary>
    /// Creates a virtual panel for the specified category with adaptive performance.
    /// </summary>
    Task<SettingsPanelViewModel?> CreateVirtualPanelAsync(SettingsCategoryViewModel category);

    /// <summary>
    /// Determines if a panel should be disposed based on performance and usage.
    /// </summary>
    bool ShouldDisposePanel(SettingsPanelViewModel panel);

    /// <summary>
    /// Optimizes panel collection based on current performance level.
    /// </summary>
    Task OptimizePanelCollectionAsync(IList<SettingsPanelViewModel> panels);
}

/// <summary>
/// Virtual panel manager for adaptive performance-based panel creation.
/// Manages dynamic loading and disposal of settings panels based on system performance.
/// Enhanced with comprehensive error handling and performance monitoring.
/// </summary>
public class VirtualPanelManager : IVirtualPanelManager
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IThemeService _themeService;
    private readonly ILogger<VirtualPanelManager> _logger;
    private readonly Dictionary<string, Type> _panelViewTypes;

    public VirtualPanelManager(
        IServiceProvider serviceProvider,
        IThemeService themeService,
        ILogger<VirtualPanelManager> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _panelViewTypes = new Dictionary<string, Type>();
        RegisterPanelViewTypes();

        _logger.LogDebug("VirtualPanelManager initialized with {PanelTypeCount} registered panel types", _panelViewTypes.Count);
    }

    #region Public Methods

    /// <summary>
    /// Creates a virtual panel for the specified category with adaptive performance.
    /// </summary>
    public async Task<SettingsPanelViewModel?> CreateVirtualPanelAsync(SettingsCategoryViewModel category)
    {
        if (category?.PanelType == null)
        {
            _logger.LogWarning("Cannot create panel for category {CategoryId} - no panel type specified", category?.Id);
            return null;
        }

        try
        {
            _logger.LogDebug("Creating virtual panel for category {CategoryId}", category.Id);

            // Determine performance level and create appropriate panel
            var performanceLevel = GetCurrentPerformanceLevel();

            var viewModel = await CreateViewModelAsync(category.PanelType, performanceLevel);
            var view = await CreateViewAsync(category.Id, viewModel, performanceLevel);

            var panel = new SettingsPanelViewModel(
                category.Id,
                category.DisplayName,
                view,
                viewModel);

            _logger.LogInformation("Virtual panel created for category {CategoryId} with {PerformanceLevel} performance level",
                category.Id, performanceLevel);

            return panel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating virtual panel for category {CategoryId}", category.Id);
            return null;
        }
    }

    /// <summary>
    /// Determines if a panel should be disposed based on performance and usage.
    /// </summary>
    public bool ShouldDisposePanel(SettingsPanelViewModel panel)
    {
        var performanceLevel = GetCurrentPerformanceLevel();

        var threshold = performanceLevel switch
        {
            PerformanceLevel.High => TimeSpan.FromMinutes(30),
            PerformanceLevel.Medium => TimeSpan.FromMinutes(15),
            PerformanceLevel.Low => TimeSpan.FromMinutes(5),
            _ => TimeSpan.FromMinutes(15)
        };

        return panel.IsEligibleForDisposal(threshold);
    }

    /// <summary>
    /// Optimizes panel collection based on current performance level.
    /// </summary>
    public Task OptimizePanelCollectionAsync(IList<SettingsPanelViewModel> panels)
    {
        var performanceLevel = GetCurrentPerformanceLevel();
        var maxPanels = GetMaxPanelsForPerformanceLevel(performanceLevel);

        if (panels.Count <= maxPanels) return Task.CompletedTask;

        // Find panels eligible for disposal
        var eligiblePanels = new List<SettingsPanelViewModel>();

        foreach (var panel in panels)
        {
            if (ShouldDisposePanel(panel))
            {
                eligiblePanels.Add(panel);
            }
        }

        // Sort by last accessed (oldest first)
        eligiblePanels.Sort((a, b) => a.LastAccessed.CompareTo(b.LastAccessed));

        // Dispose excess panels
        var panelsToDispose = Math.Min(eligiblePanels.Count, panels.Count - maxPanels);

        for (int i = 0; i < panelsToDispose; i++)
        {
            var panel = eligiblePanels[i];
            panels.Remove(panel);
            panel.Dispose();

            _logger.LogDebug("Disposed virtual panel {CategoryId} for performance optimization", panel.CategoryId);
        }

        if (panelsToDispose > 0)
        {
            _logger.LogInformation("Optimized panel collection: disposed {Count} panels for {PerformanceLevel} performance",
                panelsToDispose, performanceLevel);
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Registers view types for panel categories.
    /// </summary>
    private void RegisterPanelViewTypes()
    {
        // Database Settings
        _panelViewTypes["database"] = typeof(Views.SettingsForm.DatabaseSettingsView);

        // User Management
        _panelViewTypes["add-user"] = typeof(Views.SettingsForm.AddUserView);
        _panelViewTypes["edit-user"] = typeof(Views.SettingsForm.EditUserView);
        _panelViewTypes["delete-user"] = typeof(Views.SettingsForm.RemoveUserView);

        // Part Numbers
        _panelViewTypes["add-part"] = typeof(Views.SettingsForm.AddPartView);
        _panelViewTypes["edit-part"] = typeof(Views.SettingsForm.EditPartView);
        _panelViewTypes["remove-part"] = typeof(Views.SettingsForm.RemovePartView);

        // Operations
        _panelViewTypes["add-operation"] = typeof(Views.SettingsForm.AddOperationView);
        _panelViewTypes["edit-operation"] = typeof(Views.SettingsForm.EditOperationView);
        _panelViewTypes["remove-operation"] = typeof(Views.SettingsForm.RemoveOperationView);

        // Locations
        _panelViewTypes["add-location"] = typeof(Views.SettingsForm.AddLocationView);
        _panelViewTypes["edit-location"] = typeof(Views.SettingsForm.EditLocationView);
        _panelViewTypes["remove-location"] = typeof(Views.SettingsForm.RemoveLocationView);

        // Item Types
        _panelViewTypes["add-itemtype"] = typeof(Views.SettingsForm.AddItemTypeView);
        _panelViewTypes["edit-itemtype"] = typeof(Views.SettingsForm.EditItemTypeView);
        _panelViewTypes["remove-itemtype"] = typeof(Views.SettingsForm.RemoveItemTypeView);

        // Advanced Features
        _panelViewTypes["theme-builder"] = typeof(Views.ThemeEditorView);
        _panelViewTypes["shortcuts"] = typeof(Views.SettingsForm.ShortcutsView);
        _panelViewTypes["about"] = typeof(Views.SettingsForm.AboutView);

        // Additional Administrative Features
        _panelViewTypes["system-health"] = typeof(Views.SettingsForm.SystemHealthView);
        _panelViewTypes["backup-recovery"] = typeof(Views.SettingsForm.BackupRecoveryView);
        _panelViewTypes["security-permissions"] = typeof(Views.SettingsForm.SecurityPermissionsView);
    }

    /// <summary>
    /// Creates ViewModel instance with dependency injection.
    /// </summary>
    private async Task<BaseViewModel> CreateViewModelAsync(Type viewModelType, PerformanceLevel performanceLevel)
    {
        try
        {
            var viewModel = _serviceProvider.GetRequiredService(viewModelType) as BaseViewModel;

            if (viewModel == null)
            {
                throw new InvalidOperationException($"Could not create ViewModel of type {viewModelType.Name}");
            }

            // Initialize ViewModel based on performance level
            if (viewModel is IPerformanceAware performanceAware)
            {
                await performanceAware.InitializeAsync(performanceLevel);
            }

            return viewModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ViewModel of type {ViewModelType}", viewModelType.Name);
            throw;
        }
    }

    /// <summary>
    /// Creates View instance with ViewModel binding.
    /// </summary>
    private async Task<UserControl> CreateViewAsync(string categoryId, BaseViewModel viewModel, PerformanceLevel performanceLevel)
    {
        if (!_panelViewTypes.TryGetValue(categoryId, out var viewType))
        {
            throw new InvalidOperationException($"No view type registered for category {categoryId}");
        }

        try
        {
            var view = Activator.CreateInstance(viewType) as UserControl;

            if (view == null)
            {
                throw new InvalidOperationException($"Could not create View of type {viewType.Name}");
            }

            // Set DataContext
            view.DataContext = viewModel;

            // Initialize View based on performance level
            if (view is IPerformanceAware performanceAware)
            {
                await performanceAware.InitializeAsync(performanceLevel);
            }

            return view;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating View of type {ViewType}", viewType.Name);
            throw;
        }
    }

    /// <summary>
    /// Determines current system performance level.
    /// </summary>
    private PerformanceLevel GetCurrentPerformanceLevel()
    {
        // Simple performance determination based on available memory and CPU
        // In a real implementation, this could check system metrics
        var availableMemory = GC.GetTotalMemory(false);

        if (availableMemory > 500_000_000) // > 500MB
        {
            return PerformanceLevel.High;
        }
        else if (availableMemory > 200_000_000) // > 200MB
        {
            return PerformanceLevel.Medium;
        }
        else
        {
            return PerformanceLevel.Low;
        }
    }

    /// <summary>
    /// Gets maximum panels allowed for performance level.
    /// </summary>
    private int GetMaxPanelsForPerformanceLevel(PerformanceLevel level)
    {
        return level switch
        {
            PerformanceLevel.High => 10,
            PerformanceLevel.Medium => 6,
            PerformanceLevel.Low => 3,
            _ => 6
        };
    }

    #endregion
}

/// <summary>
/// Performance levels for adaptive panel management.
/// </summary>
public enum PerformanceLevel
{
    Low,
    Medium,
    High
}

/// <summary>
/// Interface for performance-aware components.
/// </summary>
public interface IPerformanceAware
{
    Task InitializeAsync(PerformanceLevel performanceLevel);
}

#endregion
