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

#region Success Overlay Service

/// <summary>
/// Interface for the success overlay service
/// </summary>
public interface ISuccessOverlayService
{
    /// <summary>
    /// Shows a success overlay on the specified target control
    /// </summary>
    /// <param name="targetControl">The control to overlay</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details</param>
    /// <param name="iconKind">Material icon to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="onDismissed">Callback when overlay is dismissed</param>
    /// <param name="isError">True if this is an error overlay, false for success</param>
    Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null,
        bool isError = false);

    /// <summary>
    /// Shows success overlay using MainView integration (preferred method)
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="isError">True if this is an error overlay, false for success</param>
    Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        bool isError = false);

    /// <summary>
    /// Shows error overlay using MainView integration
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Error message to display</param>
    /// <param name="details">Optional details about the error</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    Task ShowErrorOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "AlertCircle",
        int duration = 3000);
}

/// <summary>
/// Success overlay service implementation for MTM WIP Application.
/// </summary>
public class SuccessOverlayService : ISuccessOverlayService
{
    private readonly ILogger<SuccessOverlayService> _logger;

    public SuccessOverlayService(ILogger<SuccessOverlayService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogDebug("SuccessOverlayService constructed successfully");
    }

    public async Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null,
        bool isError = false)
    {
        try
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var overlayViewModel = new SuccessOverlayViewModel
                {
                    Message = message,
                    Details = details,
                    IconKind = iconKind,
                    IsError = isError
                };

                var overlayView = new Views.Overlay.SuccessOverlayView
                {
                    DataContext = overlayViewModel
                };

                // This would show the overlay over the target control
                // Implementation would depend on specific overlay system

                _logger.LogDebug("Showing {OverlayType} overlay: {Message}", isError ? "error" : "success", message);

                // Auto-dismiss after duration
                await Task.Delay(duration);
                onDismissed?.Invoke();
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show success overlay");
        }
    }

    public async Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        bool isError = false)
    {
        try
        {
            // Find MainView from source control or application
            var mainView = FindMainView(sourceControl);
            if (mainView != null)
            {
                await ShowSuccessOverlayAsync(mainView, message, details, iconKind, duration, null, isError);
            }
            else
            {
                _logger.LogWarning("Could not find MainView for overlay display");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to show success overlay in MainView");
        }
    }

    public async Task ShowErrorOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "AlertCircle",
        int duration = 3000)
    {
        await ShowSuccessOverlayInMainViewAsync(sourceControl, message, details, iconKind, duration, true);
    }

    private Control? FindMainView(Control? sourceControl)
    {
        // This would traverse the visual tree to find the MainView
        // Implementation would depend on the application structure

        if (sourceControl == null)
        {
            // Try to get from current application
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }
            return null;
        }

        // Traverse up the visual tree to find MainView
        var current = sourceControl;
        while (current != null)
        {
            if (current.Name == "MainView" || current.GetType().Name.Contains("MainView"))
            {
                return current;
            }
            current = current.GetVisualParent<Control>();
        }

        return null;
    }
}

#endregion
