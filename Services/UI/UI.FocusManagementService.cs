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

#region Focus Management Service

/// <summary>
/// Service for managing focus across views in the MTM WIP Application.
/// Provides automatic focus management during application startup and view switching
/// while preserving SuggestionOverlay functionality.
/// </summary>
public interface IFocusManagementService
{
    /// <summary>
    /// Sets focus to the first focusable control with TabIndex=1 in the specified view.
    /// </summary>
    /// <param name="view">The view to set focus in</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 100ms)</param>
    Task SetInitialFocusAsync(Control view, int delayMs = 100);

    /// <summary>
    /// Sets focus to the first focusable control with TabIndex=1 after application startup completes.
    /// </summary>
    /// <param name="mainView">The main view containing the tab system</param>
    Task SetStartupFocusAsync(Control mainView);

    /// <summary>
    /// Sets focus when switching between tabs in the main view.
    /// </summary>
    /// <param name="mainView">The main view containing the tab system</param>
    /// <param name="tabIndex">The tab index being switched to</param>
    Task SetTabSwitchFocusAsync(Control mainView, int tabIndex);

    /// <summary>
    /// Sets focus to the next TabIndex control after SuggestionOverlay closes with a selection.
    /// </summary>
    /// <param name="currentControl">The control that had focus before the overlay</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 100ms)</param>
    Task SetNextTabIndexFocusAsync(Control currentControl, int delayMs = 100);

    /// <summary>
    /// Sets focus back to the current TabIndex control after SuggestionOverlay closes without selection.
    /// </summary>
    /// <param name="currentControl">The control to refocus</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 50ms)</param>
    Task RestoreFocusAsync(Control currentControl, int delayMs = 50);
}

/// <summary>
/// Focus management service implementation for MTM WIP Application.
/// </summary>
public class FocusManagementService : IFocusManagementService
{
    private readonly ILogger<FocusManagementService> _logger;

    public FocusManagementService(ILogger<FocusManagementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("FocusManagementService constructed successfully");
    }

    public async Task SetInitialFocusAsync(Control view, int delayMs = 100)
    {
        try
        {
            await Task.Delay(delayMs);

            var focusableControl = FindFirstFocusableControlWithTabIndex(view, 1);
            if (focusableControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    focusableControl.Focus();
                    _logger.LogDebug("Set initial focus to {ControlType}", focusableControl.GetType().Name);
                });
            }
            else
            {
                _logger.LogWarning("No focusable control with TabIndex=1 found in view {ViewType}", view.GetType().Name);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set initial focus in view {ViewType}", view.GetType().Name);
        }
    }

    public async Task SetStartupFocusAsync(Control mainView)
    {
        try
        {
            // Allow application to fully load before setting focus
            await Task.Delay(500);

            await SetInitialFocusAsync(mainView, 0);
            _logger.LogDebug("Set startup focus in main view");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set startup focus");
        }
    }

    public async Task SetTabSwitchFocusAsync(Control mainView, int tabIndex)
    {
        try
        {
            await Task.Delay(100); // Allow tab switching to complete

            var focusableControl = FindFirstFocusableControlWithTabIndex(mainView, 1);
            if (focusableControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    focusableControl.Focus();
                    _logger.LogDebug("Set tab switch focus for tab {TabIndex}", tabIndex);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set tab switch focus for tab {TabIndex}", tabIndex);
        }
    }

    public async Task SetNextTabIndexFocusAsync(Control currentControl, int delayMs = 100)
    {
        try
        {
            await Task.Delay(delayMs);

            var nextControl = FindNextTabIndexControl(currentControl);
            if (nextControl != null)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    nextControl.Focus();
                    _logger.LogDebug("Set focus to next TabIndex control");
                });
            }
            else
            {
                // Fall back to restoring focus to current control
                await RestoreFocusAsync(currentControl, 0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set next TabIndex focus");
            await RestoreFocusAsync(currentControl, 0);
        }
    }

    public async Task RestoreFocusAsync(Control currentControl, int delayMs = 50)
    {
        try
        {
            await Task.Delay(delayMs);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                currentControl.Focus();
                _logger.LogDebug("Restored focus to {ControlType}", currentControl.GetType().Name);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore focus to {ControlType}", currentControl.GetType().Name);
        }
    }

    private Control? FindFirstFocusableControlWithTabIndex(Control container, int targetTabIndex)
    {
        if (container is InputElement input && input.Focusable && input.TabIndex == targetTabIndex)
            return container;

        foreach (var child in container.GetVisualChildren().OfType<Control>())
        {
            var result = FindFirstFocusableControlWithTabIndex(child, targetTabIndex);
            if (result != null)
                return result;
        }

        return null;
    }

    private Control? FindNextTabIndexControl(Control currentControl)
    {
        var parent = currentControl.GetVisualParent<Control>();
        if (parent == null) return null;

        var currentTabIndex = (currentControl as InputElement)?.TabIndex ?? 0;
        var nextTabIndex = currentTabIndex + 1;

        return FindFirstFocusableControlWithTabIndex(parent, nextTabIndex);
    }
}

#endregion
