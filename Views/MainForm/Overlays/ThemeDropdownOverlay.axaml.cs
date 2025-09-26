using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services.Interfaces;

namespace MTM_WIP_Application_Avalonia.Views.MainForm.Overlays;

/// <summary>
/// Theme Dropdown Overlay - Full-screen overlay for theme settings
/// Provides a dropdown panel that can overlay the entire application content.
/// Uses Theme V2 integration for light/dark mode switching.
/// </summary>
public partial class ThemeDropdownOverlay : UserControl
{
    // Services
    private ILogger<ThemeDropdownOverlay>? _logger;
    private IThemeServiceV2? _themeServiceV2;

    // Events
    public event EventHandler<EventArgs>? CloseRequested;
    public event EventHandler<EventArgs>? ThemeEditorRequested;

    /// <summary>
    /// Initializes a new instance of the ThemeDropdownOverlay.
    /// </summary>
    public ThemeDropdownOverlay()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        // Subscribe to IsVisible changes to refresh theme state when shown
        this.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Handles property changes, specifically IsVisible to refresh theme state when shown
    /// </summary>
    private async void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        try
        {
            if (e.Property.Name == nameof(IsVisible) && IsVisible)
            {
                // Refresh the toggle switch when overlay becomes visible
                await UpdateToggleSwitchFromCurrentTheme();
                _logger?.LogDebug("Theme toggle refreshed when overlay became visible");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling property change in ThemeDropdownOverlay");
        }
    }

    /// <summary>
    /// Initialize the component when loaded - resolve services and set up Theme V2.
    /// </summary>
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Resolve services from DI container
            _themeServiceV2 = Program.GetOptionalService<IThemeServiceV2>();
            _logger = Program.GetOptionalService<ILogger<ThemeDropdownOverlay>>();

            _logger?.LogDebug("Services resolved for ThemeDropdownOverlay");

            InitializeEventHandlers();

            // Small delay to ensure theme service initialization is complete
            await Task.Delay(100);
            await InitializeThemeComponentsAsync();

            _logger?.LogDebug("ThemeDropdownOverlay initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing ThemeDropdownOverlay");
            await Services.ErrorHandling.HandleErrorAsync(ex, "ThemeDropdownOverlay initialization failed", "SYSTEM");
        }
    }

    /// <summary>
    /// Clean up resources when the control is unloaded.
    /// </summary>
    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Unsubscribe from Theme V2 service events
            if (_themeServiceV2 != null)
            {
                _themeServiceV2.ThemeChanged -= OnThemeServiceChanged;
            }

            // Unsubscribe from property change events
            this.PropertyChanged -= OnPropertyChanged;

            _logger?.LogDebug("ThemeDropdownOverlay cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during ThemeDropdownOverlay cleanup");
        }
    }

    /// <summary>
    /// Initialize Theme V2 components and load current theme preferences.
    /// </summary>
    private async Task InitializeThemeComponentsAsync()
    {
        try
        {
            // Initialize toggle switch with current theme state
            await UpdateToggleSwitchFromCurrentTheme();

            if (_themeServiceV2 != null)
            {
                // Subscribe to theme changes from service V2
                _themeServiceV2.ThemeChanged += OnThemeServiceChanged;
                _logger?.LogDebug("Theme components initialized with ThemeServiceV2");
            }
            else
            {
                _logger?.LogWarning("ThemeServiceV2 not available - using Application.Current theme");
            }

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing Theme components");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Theme component initialization failed", "SYSTEM");
        }
    }

    /// <summary>
    /// Updates the toggle switch to reflect the current active theme.
    /// </summary>
    private async Task UpdateToggleSwitchFromCurrentTheme()
    {
        try
        {
            if (ThemeToggleSwitch == null)
            {
                _logger?.LogWarning("ThemeToggleSwitch control not found");
                return;
            }

            // Temporarily unsubscribe to prevent triggering change events during initialization
            ThemeToggleSwitch.IsCheckedChanged -= OnThemeToggleSwitchChanged;

            ThemeVariant currentTheme;

            if (_themeServiceV2 != null)
            {
                // Get resolved theme variant from service
                currentTheme = _themeServiceV2.GetResolvedThemeVariant();
                _logger?.LogDebug("Current theme from ThemeServiceV2: {Theme}", currentTheme);
            }
            else
            {
                // Fallback to Application.Current theme
                currentTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
                _logger?.LogDebug("Current theme from Application.Current: {Theme}", currentTheme);
            }

            // Set toggle switch: Dark mode = true, Light mode = false
            bool isDarkMode = currentTheme == ThemeVariant.Dark;
            ThemeToggleSwitch.IsChecked = isDarkMode;

            _logger?.LogInformation("Toggle switch initialized: Dark mode = {IsDarkMode}", isDarkMode);

            // Re-subscribe to change events
            ThemeToggleSwitch.IsCheckedChanged += OnThemeToggleSwitchChanged;

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating toggle switch from current theme");
        }
    }

    /// <summary>
    /// Handle Theme V2 service theme changes - update UI to reflect external theme changes.
    /// </summary>
    private void OnThemeServiceChanged(object? sender, Services.Interfaces.ThemeChangedEventArgs e)
    {
        try
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (ThemeToggleSwitch != null)
                {
                    // Update toggle switch to match new theme (avoid triggering change event)
                    ThemeToggleSwitch.IsCheckedChanged -= OnThemeToggleSwitchChanged;
                    ThemeToggleSwitch.IsChecked = e.NewTheme == ThemeVariant.Dark;
                    ThemeToggleSwitch.IsCheckedChanged += OnThemeToggleSwitchChanged;
                }

                _logger?.LogDebug("Theme UI updated to reflect external change: {ThemeName}", e.ThemeName);
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Theme change event");
        }
    }

    /// <summary>
    /// Initialize event handlers for theme controls.
    /// </summary>
    private void InitializeEventHandlers()
    {
        try
        {
            // Theme toggle switch event - for Light/Dark mode switching
            if (ThemeToggleSwitch != null)
            {
                ThemeToggleSwitch.IsCheckedChanged += OnThemeToggleSwitchChanged;
            }

            // Edit theme button event - opens advanced theme editor
            if (EditThemeButton != null)
            {
                EditThemeButton.Click += OnEditThemeButtonClick;
            }

            // Accent color button event - opens color picker
            if (AccentColorButton != null)
            {
                AccentColorButton.Click += OnAccentColorButtonClick;
            }

            // Apply button event - applies current settings
            if (ApplyButton != null)
            {
                ApplyButton.Click += OnApplyButtonClick;
            }

            // Close button event - closes overlay
            if (CloseButton != null)
            {
                CloseButton.Click += OnCloseButtonClick;
            }

            _logger?.LogDebug("Theme event handlers initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing theme event handlers");
        }
    }

    /// <summary>
    /// Handle theme toggle switch changes - switch between Light and Dark modes.
    /// </summary>
    private async void OnThemeToggleSwitchChanged(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_themeServiceV2 != null && ThemeToggleSwitch != null)
            {
                var isDarkMode = ThemeToggleSwitch.IsChecked ?? false;
                var targetTheme = isDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;

                _logger?.LogInformation("Switching theme via toggle: {Theme} (Dark={IsDark})", targetTheme, isDarkMode);

                await _themeServiceV2.ApplyThemeAsync(targetTheme);

                _logger?.LogInformation("Theme successfully switched to {Theme}", targetTheme);
            }
            else
            {
                _logger?.LogWarning("Cannot switch theme - ThemeServiceV2 or ToggleSwitch not available");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error switching theme via toggle switch");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Theme toggle switch failed", "SYSTEM");
        }
    }

    /// <summary>
    /// Handle edit theme button click - currently disabled (not implemented).
    /// </summary>
    private void OnEditThemeButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Edit theme button clicked - feature not yet implemented");

            // Feature not implemented yet - log for future development
            _logger?.LogInformation("Advanced theme editor requested but not yet implemented");

            // Don't close dropdown since no action was taken
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling edit theme button click");
        }
    }

    /// <summary>
    /// Handle accent color button click - currently disabled (not implemented).
    /// </summary>
    private void OnAccentColorButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Accent color button clicked - feature not yet implemented");

            // Feature not implemented yet - log for future development
            _logger?.LogInformation("Accent color picker requested but not yet implemented");

            // Don't close dropdown since no action was taken
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling accent color button click");
        }
    }

    /// <summary>
    /// Handle apply button click - applies current theme settings.
    /// </summary>
    private async void OnApplyButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_themeServiceV2 != null && ThemeToggleSwitch != null)
            {
                // Apply current toggle switch state
                var isDarkMode = ThemeToggleSwitch.IsChecked ?? false;
                var targetTheme = isDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;

                _logger?.LogInformation("Applying theme settings via Apply button: {Theme}", targetTheme);

                await _themeServiceV2.ApplyThemeAsync(targetTheme);

                _logger?.LogInformation("Theme settings applied successfully: {Theme}", targetTheme);

                // Close the dropdown after applying
                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _logger?.LogWarning("Cannot apply theme - ThemeServiceV2 or controls not available");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying theme settings via Apply button");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Apply theme settings failed", "SYSTEM");
        }
    }

    /// <summary>
    /// Handle close button click - closes the overlay.
    /// </summary>
    private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Close button clicked - closing theme dropdown");
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error closing theme dropdown");
        }
    }
}
