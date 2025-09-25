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
/// Theme Quick Switcher V2 - Modern theme switching component for Avalonia 11.3.4
/// Provides quick access to light/dark theme switching with Theme V2 integration.
/// Uses ToggleSwitch for Light/Dark mode and accent color selection button.
/// </summary>
public partial class ThemeQuickSwitcher : UserControl
{
    // Services
    private ILogger<ThemeQuickSwitcher>? _logger;
    private IThemeServiceV2? _themeServiceV2;

    // Events
    public event EventHandler<EventArgs>? ThemeEditorRequested;

    /// <summary>
    /// Initializes a new instance of the ThemeQuickSwitcher.
    /// </summary>
    public ThemeQuickSwitcher()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Initialize the component when loaded - resolve services and set up Theme V2.
    /// </summary>
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Resolve services from DI container using Program.GetService pattern
            _themeServiceV2 = Program.GetOptionalService<IThemeServiceV2>();
            _logger = Program.GetOptionalService<ILogger<ThemeQuickSwitcher>>();

            _logger?.LogDebug("Services resolved for ThemeQuickSwitcher V2");

            InitializeEventHandlers();

            // Small delay to ensure theme service initialization is complete
            await Task.Delay(100);
            await InitializeThemeV2ComponentsAsync();

            _logger?.LogDebug("ThemeQuickSwitcher V2 initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing ThemeQuickSwitcher V2");
            await Services.ErrorHandling.HandleErrorAsync(ex, "ThemeQuickSwitcher initialization failed", "SYSTEM");
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
                _themeServiceV2.ThemeChanged -= OnThemeServiceV2Changed;
            }

            _logger?.LogDebug("ThemeQuickSwitcher V2 cleanup completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during ThemeQuickSwitcher V2 cleanup");
        }
    }

    /// <summary>
    /// Initialize Theme V2 components and load current theme preferences.
    /// </summary>
    private async Task InitializeThemeV2ComponentsAsync()
    {
        try
        {
            if (_themeServiceV2 != null)
            {
                // Get resolved theme variant (never Default) and update toggle switch
                var resolvedVariant = _themeServiceV2.GetResolvedThemeVariant();
                if (ThemeToggleSwitch != null)
                {
                    // Set toggle switch: Dark mode = true, Light mode = false
                    ThemeToggleSwitch.IsChecked = resolvedVariant == ThemeVariant.Dark;
                }

                // Subscribe to theme changes from service V2
                _themeServiceV2.ThemeChanged += OnThemeServiceV2Changed;

                _logger?.LogDebug("Theme V2 components initialized with resolved theme: {ResolvedTheme}", resolvedVariant);
            }
            else
            {
                _logger?.LogWarning("ThemeServiceV2 not available - using Application.Current theme");

                // Fallback to Application.Current theme if service not available
                if (ThemeToggleSwitch != null)
                {
                    var appTheme = Application.Current?.ActualThemeVariant ?? ThemeVariant.Light;
                    ThemeToggleSwitch.IsChecked = appTheme == ThemeVariant.Dark;
                    _logger?.LogDebug("Toggle initialized from Application theme: {AppTheme}", appTheme);
                }
            }

            await Task.CompletedTask; // Satisfy async requirement
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing Theme V2 components");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Theme V2 component initialization failed", "SYSTEM");
        }
    }

    /// <summary>
    /// Handle Theme V2 service theme changes - update UI to reflect external theme changes.
    /// </summary>
    private void OnThemeServiceV2Changed(object? sender, Services.Interfaces.ThemeChangedEventArgs e)
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

                _logger?.LogDebug("Theme V2 UI updated to reflect external change: {ThemeName}", e.ThemeName);
            });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling Theme V2 change event");
        }
    }

    /// <summary>
    /// Initialize event handlers for Theme V2 controls.
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

            // Accent color button event - opens color picker (Theme V2 feature)
            if (AccentColorButton != null)
            {
                AccentColorButton.Click += OnAccentColorButtonClick;
            }

            // Apply button event - applies current settings
            if (ApplyButton != null)
            {
                ApplyButton.Click += OnApplyButtonClick;
            }

            _logger?.LogDebug("Theme V2 event handlers initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing Theme V2 event handlers");
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
    /// Handle edit theme button click - opens advanced theme editor.
    /// </summary>
    private void OnEditThemeButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Edit theme button clicked - requesting advanced theme editor");
            ThemeEditorRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error opening advanced theme editor");
        }
    }

    /// <summary>
    /// Handle accent color button click - opens color picker for Theme V2 accent customization.
    /// </summary>
    private void OnAccentColorButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Accent color button clicked - opening color picker");

            // TODO: Implement color picker dialog for Theme V2 accent colors
            // This would integrate with ThemeServiceV2 accent color methods when implemented:
            // - await _themeServiceV2.SetAccentColorAsync(selectedColor);
            // - var currentAccent = await _themeServiceV2.GetAccentColorAsync();

            // For now, log the action
            _logger?.LogInformation("Accent color picker requested (not yet implemented)");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error opening accent color picker");
        }
    }

    /// <summary>
    /// Handle apply button click - applies current theme settings and saves preferences.
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

                // TODO: Save theme preference to database when user preference storage is implemented
                // This would use stored procedure: thm_user_preferences_Save

                _logger?.LogInformation("Theme settings applied successfully: {Theme}", targetTheme);
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
}
