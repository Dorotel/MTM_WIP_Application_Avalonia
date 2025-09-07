using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

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
    /// <param name="currentControl">The control that should regain focus</param>
    /// <param name="delayMs">Optional delay before setting focus (default: 100ms)</param>
    Task SetCurrentTabIndexFocusAsync(Control currentControl, int delayMs = 100);
}

public class FocusManagementService : IFocusManagementService
{
    private readonly ILogger<FocusManagementService> _logger;
    
    public FocusManagementService(ILogger<FocusManagementService> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SetInitialFocusAsync(Control view, int delayMs = 100)
    {
        try
        {
            if (view == null)
            {
                _logger.LogWarning("Cannot set initial focus: view is null");
                return;
            }

            // Add delay to allow view to fully render and initialize
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var focusTarget = FindFirstTabIndexControl(view, 1);
                if (focusTarget != null)
                {
                    focusTarget.Focus();
                    _logger.LogDebug("Initial focus set to control with TabIndex=1: {ControlName}", 
                        focusTarget.Name ?? focusTarget.GetType().Name);
                }
                else
                {
                    _logger.LogDebug("No control with TabIndex=1 found in view");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting initial focus in view");
        }
    }

    /// <inheritdoc />
    public async Task SetStartupFocusAsync(Control mainView)
    {
        try
        {
            if (mainView == null)
            {
                _logger.LogWarning("Cannot set startup focus: mainView is null");
                return;
            }

            _logger.LogInformation("Setting startup focus after application initialization completes");

            // Wait for application startup to fully complete
            // This ensures theme loading, data loading, and UI initialization are done
            await Task.Delay(2000);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Find the tab control and ensure we're on the first tab (Inventory)
                var tabControl = FindControlInVisualTree<TabControl>(mainView, "MainForm_TabControl");
                if (tabControl != null)
                {
                    // Ensure we're on the first tab
                    if (tabControl.SelectedIndex != 0)
                    {
                        tabControl.SelectedIndex = 0;
                    }

                    // Find the first focusable control with TabIndex=1
                    var focusTarget = FindFirstTabIndexControlInCurrentTab(tabControl, 1);
                    if (focusTarget != null)
                    {
                        focusTarget.Focus();
                        _logger.LogInformation("Startup focus set to first TabIndex=1 control: {ControlName}", 
                            focusTarget.Name ?? focusTarget.GetType().Name);
                    }
                    else
                    {
                        _logger.LogWarning("No control with TabIndex=1 found for startup focus");
                    }
                }
                else
                {
                    _logger.LogWarning("MainForm_TabControl not found for startup focus");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting startup focus");
        }
    }

    /// <inheritdoc />
    public async Task SetTabSwitchFocusAsync(Control mainView, int tabIndex)
    {
        try
        {
            if (mainView == null)
            {
                _logger.LogWarning("Cannot set tab switch focus: mainView is null");
                return;
            }

            _logger.LogDebug("Setting focus for tab switch to tab index: {TabIndex}", tabIndex);

            // Small delay to allow tab switch animation to complete
            await Task.Delay(150);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var tabControl = FindControlInVisualTree<TabControl>(mainView, "MainForm_TabControl");
                if (tabControl != null && tabControl.SelectedIndex == tabIndex)
                {
                    var focusTarget = FindFirstTabIndexControlInCurrentTab(tabControl, 1);
                    if (focusTarget != null)
                    {
                        focusTarget.Focus();
                        _logger.LogDebug("Tab switch focus set to TabIndex=1 control: {ControlName}", 
                            focusTarget.Name ?? focusTarget.GetType().Name);
                    }
                    else
                    {
                        _logger.LogDebug("No TabIndex=1 control found in current tab for focus");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting tab switch focus for tab {TabIndex}", tabIndex);
        }
    }

    /// <summary>
    /// Finds the first control with the specified TabIndex in the visual tree.
    /// </summary>
    /// <param name="parent">Parent control to search from</param>
    /// <param name="tabIndex">TabIndex to search for</param>
    /// <returns>The first control with the specified TabIndex, or null if not found</returns>
    private Control? FindFirstTabIndexControl(Control parent, int tabIndex)
    {
        try
        {
            // Check current control - in Avalonia, TabIndex is accessed via GetValue
            if (parent.GetValue(KeyboardNavigation.TabIndexProperty) == tabIndex && parent.Focusable && parent.IsVisible)
            {
                return parent;
            }

            // Recursively search children
            var children = parent.GetVisualChildren().OfType<Control>();
            foreach (var child in children)
            {
                var result = FindFirstTabIndexControl(child, tabIndex);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error searching for TabIndex {TabIndex} control", tabIndex);
            return null;
        }
    }

    /// <summary>
    /// Finds the first control with the specified TabIndex in the currently selected tab.
    /// </summary>
    /// <param name="tabControl">The TabControl containing the tabs</param>
    /// <param name="tabIndex">TabIndex to search for</param>
    /// <returns>The first control with the specified TabIndex in the current tab, or null if not found</returns>
    private Control? FindFirstTabIndexControlInCurrentTab(TabControl tabControl, int tabIndex)
    {
        try
        {
            if (tabControl.SelectedItem is TabItem selectedTab)
            {
                var tabContent = selectedTab.Content as Control;
                if (tabContent != null)
                {
                    return FindFirstTabIndexControl(tabContent, tabIndex);
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error searching for TabIndex {TabIndex} control in current tab", tabIndex);
            return null;
        }
    }

    /// <summary>
    /// Helper method to find a control in the visual tree by name and type.
    /// </summary>
    /// <typeparam name="T">The type of control to find</typeparam>
    /// <param name="parent">Parent control to search from</param>
    /// <param name="name">Name of the control to find</param>
    /// <returns>The control if found, or null</returns>
    private T? FindControlInVisualTree<T>(Control parent, string name) where T : Control
    {
        try
        {
            if (parent.Name == name && parent is T typedControl)
                return typedControl;

            var children = parent.GetVisualChildren().OfType<Control>();
            foreach (var child in children)
            {
                var result = FindControlInVisualTree<T>(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error finding control {Name} of type {Type}", name, typeof(T).Name);
            return null;
        }
    }

    /// <inheritdoc />
    public async Task SetNextTabIndexFocusAsync(Control currentControl, int delayMs = 100)
    {
        try
        {
            if (currentControl == null)
            {
                _logger.LogWarning("Cannot set next TabIndex focus: currentControl is null");
                return;
            }

            // Add delay to allow SuggestionOverlay to fully close and UI to stabilize
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Get the current control's TabIndex
                var currentTabIndex = currentControl.GetValue(KeyboardNavigation.TabIndexProperty);
                var nextTabIndex = currentTabIndex + 1;

                _logger.LogDebug("Looking for next TabIndex control: current={CurrentTabIndex}, next={NextTabIndex}", 
                    currentTabIndex, nextTabIndex);

                // Find the parent container (typically the tab content)
                var parentContainer = GetTabContentContainer(currentControl);
                if (parentContainer != null)
                {
                    // Look for the next TabIndex control
                    var nextControl = FindFirstTabIndexControl(parentContainer, nextTabIndex);
                    if (nextControl != null)
                    {
                        nextControl.Focus();
                        _logger.LogDebug("Focus set to next TabIndex control: {ControlName} (TabIndex={TabIndex})", 
                            nextControl.Name ?? nextControl.GetType().Name, nextTabIndex);
                    }
                    else
                    {
                        // No next control found, stay on current control
                        currentControl.Focus();
                        _logger.LogDebug("No next TabIndex control found, staying on current control: {ControlName}", 
                            currentControl.Name ?? currentControl.GetType().Name);
                    }
                }
                else
                {
                    // Fallback: just refocus the current control
                    currentControl.Focus();
                    _logger.LogDebug("Could not find parent container, refocusing current control");
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting next TabIndex focus");
        }
    }

    /// <inheritdoc />
    public async Task SetCurrentTabIndexFocusAsync(Control currentControl, int delayMs = 100)
    {
        try
        {
            if (currentControl == null)
            {
                _logger.LogWarning("Cannot set current TabIndex focus: currentControl is null");
                return;
            }

            // Add delay to allow SuggestionOverlay to fully close and UI to stabilize
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Simply refocus the current control
                currentControl.Focus();
                _logger.LogDebug("Focus restored to current control: {ControlName}", 
                    currentControl.Name ?? currentControl.GetType().Name);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting current TabIndex focus");
        }
    }

    /// <summary>
    /// Gets the parent container that holds the tab content (usually a TabItem content or similar).
    /// </summary>
    /// <param name="control">The control to start searching from</param>
    /// <returns>The parent container, or null if not found</returns>
    private Control? GetTabContentContainer(Control control)
    {
        try
        {
            var current = control.Parent;
            while (current != null)
            {
                // Look for common tab content containers
                if (current is Grid || current is StackPanel || current is Border)
                {
                    // Check if this container has multiple focusable children (likely a form container)
                    var focusableChildren = (current as Control)?
                        .GetVisualDescendants()
                        .OfType<Control>()
                        .Where(c => c.Focusable && c.GetValue(KeyboardNavigation.TabIndexProperty) > 0)
                        .Count() ?? 0;

                    if (focusableChildren > 1)
                    {
                        return current as Control;
                    }
                }

                current = current.Parent;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Error finding tab content container");
            return null;
        }
    }
}
