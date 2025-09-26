using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Views.MainForm.Overlays;

/// <summary>
/// Theme Quick Switcher - Simple icon button for opening theme dropdown overlay
/// Provides quick access to theme settings via a dropdown overlay.
/// </summary>
public partial class ThemeQuickSwitcher : UserControl
{
    // Services
    private readonly ILogger<ThemeQuickSwitcher>? _logger;

    // Events
    public event EventHandler<EventArgs>? DropdownRequested;

    /// <summary>
    /// Initialize ThemeQuickSwitcher with minimal setup.
    /// Simple icon button for opening theme dropdown overlay.
    /// </summary>
    public ThemeQuickSwitcher()
    {
        try
        {
            InitializeComponent();

            // Get logger service
            _logger = Program.GetOptionalService<ILogger<ThemeQuickSwitcher>>();

            // Initialize event handlers
            InitializeEventHandlers();

            _logger?.LogInformation("ThemeQuickSwitcher initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "ThemeQuickSwitcher initialization failed");
        }
    }

    /// <summary>
    /// Initialize event handlers for the theme icon button.
    /// </summary>
    private void InitializeEventHandlers()
    {
        try
        {
            // Find theme icon button
            var themeIconButton = this.FindControl<Button>("ThemeIconButton");

            // Theme icon button click handler
            if (themeIconButton != null)
            {
                themeIconButton.Click += OnThemeIconButtonClick;
                _logger?.LogDebug("Theme icon button event handler initialized");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing event handlers");
        }
    }

    /// <summary>
    /// Handle theme icon button click - requests dropdown overlay to be shown.
    /// </summary>
    private void OnThemeIconButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            DropdownRequested?.Invoke(this, EventArgs.Empty);
            _logger?.LogDebug("Theme dropdown requested via icon button click");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling theme icon button click");
        }
    }
}
