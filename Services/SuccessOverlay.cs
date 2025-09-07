using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;
using MTM_WIP_Application_Avalonia.Views.Overlay;

namespace MTM_WIP_Application_Avalonia.Services;

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
    Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null);

    /// <summary>
    /// Shows success overlay using MainView integration (preferred method)
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 3000);
}

/// <summary>
/// Service that provides success overlay functionality for displaying temporary success messages
/// that cover the entire calling view without interfering with focus management.
/// </summary>
public class SuccessOverlayService : ISuccessOverlayService
{
    private readonly ILogger<SuccessOverlayService> _logger;
    private readonly IFocusManagementService _focusManagementService;

    public SuccessOverlayService(
        ILogger<SuccessOverlayService> logger,
        IFocusManagementService focusManagementService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _focusManagementService = focusManagementService ?? throw new ArgumentNullException(nameof(focusManagementService));
        _logger.LogDebug("SuccessOverlayService initialized");
    }

    /// <summary>
    /// Shows a success overlay on the specified target control
    /// </summary>
    /// <param name="targetControl">The control to overlay (should be a Panel or container)</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    /// <param name="onDismissed">Callback when overlay is dismissed</param>
    public async Task ShowSuccessOverlayAsync(
        Control targetControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 2000,
        Action? onDismissed = null)
    {
        try
        {
            if (targetControl == null)
            {
                _logger.LogWarning("Target control is null, cannot show success overlay");
                return;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Message is empty, cannot show success overlay");
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowOverlayInternalAsync(targetControl, message, details, iconKind, duration, onDismissed);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing success overlay");
            // Don't throw - success overlays should never break the main application flow
        }
    }

    /// <summary>
    /// Shows success overlay using MainView integration (preferred method)
    /// </summary>
    /// <param name="sourceControl">Source control to find MainView from (can be null)</param>
    /// <param name="message">Success message to display</param>
    /// <param name="details">Optional details about the success</param>
    /// <param name="iconKind">Material icon kind to display</param>
    /// <param name="duration">Display duration in milliseconds</param>
    public async Task ShowSuccessOverlayInMainViewAsync(
        Control? sourceControl,
        string message,
        string? details = null,
        string iconKind = "CheckCircle",
        int duration = 3000)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                _logger.LogWarning("Message is empty, cannot show success overlay");
                return;
            }

            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                // Find MainView - try from sourceControl first, then fallback to static search
                Views.MainView? mainView = null;
                
                if (sourceControl != null)
                {
                    mainView = Views.MainView.FindMainView(sourceControl);
                }
                
                // If we couldn't find MainView from sourceControl, try to find it globally
                if (mainView == null)
                {
                    // Get the main window and search from there
                    var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                        ? desktop.MainWindow
                        : null;
                        
                    if (mainWindow != null)
                    {
                        mainView = FindControlInVisualTree<Views.MainView>(mainWindow);
                    }
                }

                if (mainView == null)
                {
                    _logger.LogWarning("Could not find MainView for success overlay - falling back to basic overlay");
                    return;
                }

                // Create ViewModel and View
                var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuccessOverlayViewModel>();
                var viewModel = new SuccessOverlayViewModel(vmLogger);
                var overlayView = new Views.Overlay.SuccessOverlayView
                {
                    DataContext = viewModel,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                };

                // Set up auto-dismissal with focus management
                viewModel.DismissRequested += async () =>
                {
                    mainView.HideSuccessOverlay();
                    _logger.LogDebug("Success overlay dismissed via ViewModel");
                    
                    // Set focus to first TabIndex=1 control after dismissal
                    try
                    {
                        await _focusManagementService.SetInitialFocusAsync(mainView, 150);
                        _logger.LogDebug("Focus restored to first TabIndex=1 control after success overlay dismissal");
                    }
                    catch (Exception focusEx)
                    {
                        _logger.LogWarning(focusEx, "Failed to restore focus after success overlay dismissal");
                    }
                };

                // Show overlay in MainView
                mainView.ShowSuccessOverlay(overlayView);
                _logger.LogInformation("Success overlay displayed in MainView: {Message}", message);

                // Start success animation - overlay will auto-dismiss and handle focus independently
                await viewModel.ShowSuccessAsync(message, details, iconKind, duration);
                _logger.LogDebug("Success overlay animation started - continuing independently");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing success overlay in MainView");
        }
    }

    /// <summary>
    /// Internal method to show the overlay on the UI thread
    /// </summary>
    private async Task ShowOverlayInternalAsync(
        Control targetControl,
        string message,
        string? details,
        string iconKind,
        int duration,
        Action? onDismissed)
    {
        try
        {
            _logger.LogInformation("Showing success overlay: {Message} on {ControlType}", message, targetControl.GetType().Name);

            // Find a suitable container for the overlay
            var overlayContainer = FindOverlayContainer(targetControl);
            if (overlayContainer == null)
            {
                _logger.LogWarning("Could not find suitable overlay container");
                return;
            }

            // Create ViewModel and View
            var vmLogger = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SuccessOverlayViewModel>();
            var viewModel = new SuccessOverlayViewModel(vmLogger);
            var overlayView = new SuccessOverlayView
            {
                DataContext = viewModel,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                ZIndex = 1000 // Ensure overlay is on top
            };

            // Set up dismissal handling
            var isDismissed = false;
            viewModel.DismissRequested += () =>
            {
                if (!isDismissed)
                {
                    isDismissed = true;
                    RemoveOverlayFromContainer(overlayContainer, overlayView);
                    onDismissed?.Invoke();
                    _logger.LogDebug("Success overlay dismissed");
                }
            };

            // Add overlay to container
            AddOverlayToContainer(overlayContainer, overlayView);

            // Start the success animation
            await viewModel.ShowSuccessAsync(message, details, iconKind, duration);

            _logger.LogDebug("Success overlay setup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ShowOverlayInternalAsync");
        }
    }

    /// <summary>
    /// Finds a suitable container for the overlay
    /// </summary>
    private Panel? FindOverlayContainer(Control targetControl)
    {
        try
        {
            // Look for the nearest Grid, StackPanel, or other Panel
            var current = targetControl;
            while (current != null)
            {
                if (current is Panel panel)
                {
                    _logger.LogDebug("Found overlay container: {ContainerType}", panel.GetType().Name);
                    return panel;
                }
                current = current.Parent as Control;
            }

            _logger.LogWarning("No suitable overlay container found");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding overlay container");
            return null;
        }
    }

    /// <summary>
    /// Adds the overlay to the container
    /// </summary>
    private void AddOverlayToContainer(Panel container, SuccessOverlayView overlayView)
    {
        try
        {
            // For Grid containers, the overlay should span all rows and columns
            if (container is Grid grid)
            {
                Grid.SetRowSpan(overlayView, Math.Max(1, grid.RowDefinitions.Count));
                Grid.SetColumnSpan(overlayView, Math.Max(1, grid.ColumnDefinitions.Count));
            }

            container.Children.Add(overlayView);
            _logger.LogDebug("Overlay added to container");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding overlay to container");
        }
    }

    /// <summary>
    /// Removes the overlay from the container
    /// </summary>
    private void RemoveOverlayFromContainer(Panel container, SuccessOverlayView overlayView)
    {
        try
        {
            container.Children.Remove(overlayView);
            overlayView.DataContext = null; // Clean up binding
            _logger.LogDebug("Overlay removed from container");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing overlay from container");
        }
    }

    /// <summary>
    /// Helper method to find a control in the visual tree
    /// </summary>
    private T? FindControlInVisualTree<T>(Control parent) where T : Control
    {
        try
        {
            if (parent is T target)
                return target;

            var children = parent.GetVisualChildren().OfType<Control>();
            foreach (var child in children)
            {
                var result = FindControlInVisualTree<T>(child);
                if (result != null)
                    return result;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
