using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// Theme quick switcher component for easy theme switching.
/// Self-contained component that can be embedded in any view.
/// </summary>
public partial class ThemeQuickSwitcher : UserControl
{
    private IThemeService? _themeService;
    private ILogger<ThemeQuickSwitcher>? _logger;

    public ThemeQuickSwitcher()
    {
        InitializeComponent();
        
        // Initialize components
        if (ThemeComboBox != null)
        {
            ThemeComboBox.SelectedIndex = 0; // Default to MTM Light
        }
        
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <summary>
    /// Initialize services when control is loaded.
    /// </summary>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Get services from DI container
            _logger = Program.GetOptionalService<ILogger<ThemeQuickSwitcher>>();
            _themeService = Program.GetOptionalService<IThemeService>();
            
            _logger?.LogDebug("ThemeQuickSwitcher OnLoaded event started");
            
            InitializeThemeDropdown();
            InitializeEventHandlers();
            
            _logger?.LogInformation("ThemeQuickSwitcher initialized successfully with {ThemeCount} themes", 
                ThemeComboBox?.Items.Count ?? 0);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing ThemeQuickSwitcher");
            System.Diagnostics.Debug.WriteLine($"Error initializing ThemeQuickSwitcher: {ex.Message}");
        }
    }

    /// <summary>
    /// Initialize the theme dropdown with available themes and set current selection.
    /// </summary>
    private void InitializeThemeDropdown()
    {
        try
        {
            if (_themeService != null && ThemeComboBox != null)
            {
                // Get current theme and set selection
                var currentTheme = _themeService.CurrentTheme;
                
                // Find the matching ComboBoxItem by Tag
                var matchingItem = ThemeComboBox.Items.OfType<ComboBoxItem>()
                    .FirstOrDefault(item => item.Tag?.ToString() == currentTheme);
                
                if (matchingItem != null)
                {
                    ThemeComboBox.SelectedItem = matchingItem;
                    _logger?.LogDebug("Set current theme selection to: {ThemeId}", currentTheme);
                }
                else
                {
                    // Fallback to first item if current theme not found
                    ThemeComboBox.SelectedIndex = 0;
                    _logger?.LogWarning("Current theme {ThemeId} not found in dropdown, defaulting to first item", currentTheme);
                }

                // Subscribe to theme changes from service
                _themeService.ThemeChanged += OnThemeServiceChanged;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing theme dropdown");
        }
    }

    /// <summary>
    /// Handle theme changes from the theme service.
    /// </summary>
    private void OnThemeServiceChanged(object? sender, ThemeChangedEventArgs e)
    {
        try
        {
            // THREADING FIX: Ensure UI updates happen on the UI thread
            if (Dispatcher.UIThread.CheckAccess())
            {
                // Already on UI thread, update directly
                SetSelectedTheme(e.NewTheme.Id);
                _logger?.LogDebug("Updated dropdown selection due to external theme change: {ThemeId}", e.NewTheme.Id);
            }
            else
            {
                // Not on UI thread, dispatch to UI thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    SetSelectedTheme(e.NewTheme.Id);
                    _logger?.LogDebug("Updated dropdown selection due to external theme change (dispatched): {ThemeId}", e.NewTheme.Id);
                });
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling theme service change event");
        }
    }

    /// <summary>
    /// Initialize event handlers for the controls.
    /// </summary>
    private void InitializeEventHandlers()
    {
        if (ApplyButton != null)
        {
            ApplyButton.Click += OnApplyButtonClick;
        }
        
        if (ThemeComboBox != null)
        {
            ThemeComboBox.SelectionChanged += OnThemeSelectionChanged;
        }
    }

    /// <summary>
    /// Handle apply button click.
    /// </summary>
    private async void OnApplyButtonClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (ThemeComboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                var themeId = selectedItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(themeId))
                {
                    await ApplyThemeAsync(themeId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in apply button click handler");
            System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle theme selection change - could implement instant preview here.
    /// </summary>
    private void OnThemeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // For now, we require explicit Apply button click
        // Could implement instant theme preview here in the future
        _logger?.LogDebug("Theme selection changed in dropdown");
    }

    /// <summary>
    /// Apply the selected theme using the ThemeService.
    /// </summary>
    private async System.Threading.Tasks.Task ApplyThemeAsync(string themeId)
    {
        try
        {
            _logger?.LogInformation("Applying theme: {ThemeId}", themeId);

            if (_themeService != null)
            {
                // Use the ThemeService to apply the theme
                var result = await _themeService.SetThemeAsync(themeId);
                
                if (result.IsSuccess)
                {
                    _logger?.LogInformation("Theme applied successfully: {ThemeId} - {Message}", themeId, result.Message);
                    
                    // Save user preference if successful
                    var saveResult = await _themeService.SaveUserPreferredThemeAsync(themeId);
                    if (!saveResult.IsSuccess)
                    {
                        _logger?.LogWarning("Failed to save theme preference: {Message}", saveResult.Message);
                    }

                    // CRITICAL FIX: Force application-wide theme update
                    await ForceApplicationThemeUpdateAsync();
                }
                else
                {
                    _logger?.LogWarning("Failed to apply theme {ThemeId}: {Message}", themeId, result.Message);
                    System.Diagnostics.Debug.WriteLine($"Failed to apply theme: {result.Message}");
                }
            }
            else
            {
                _logger?.LogWarning("ThemeService not available, falling back to basic theme switching");
                // Fallback: Apply basic theme switching via Avalonia's built-in mechanism
                ApplyBasicTheme(themeId);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in ApplyThemeAsync for theme {ThemeId}", themeId);
            System.Diagnostics.Debug.WriteLine($"Error in ApplyThemeAsync: {ex.Message}");
            
            // Fallback to basic theme switching
            ApplyBasicTheme(themeId);
        }
    }

    /// <summary>
    /// Forces application-wide theme update by invalidating visual trees.
    /// This ensures all views refresh with the new theme resources.
    /// </summary>
    private async System.Threading.Tasks.Task ForceApplicationThemeUpdateAsync()
    {
        try
        {
            await Task.Delay(50); // Small delay to allow resource loading

            // Get all open windows and force their visual trees to re-evaluate
            if (Avalonia.Application.Current != null)
            {
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Force invalidation of all top-level windows
                    foreach (var window in Avalonia.Application.Current.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                        ? desktop.Windows
                        : new Avalonia.Controls.Window[0])
                    {
                        try
                        {
                            // Force re-evaluation of the visual tree
                            window.InvalidateVisual();
                            window.InvalidateMeasure();
                            window.InvalidateArrange();
                            
                            // Recursively invalidate all child controls
                            InvalidateControlTree(window);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogWarning(ex, "Error invalidating window during theme update");
                        }
                    }
                });

                _logger?.LogDebug("Forced application-wide theme update completed");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during forced theme update");
        }
    }

    /// <summary>
    /// Recursively invalidates all controls in a visual tree to pick up new theme resources.
    /// </summary>
    private void InvalidateControlTree(Avalonia.Controls.Control control)
    {
        try
        {
            if (control == null) return;

            // Invalidate this control
            control.InvalidateVisual();
            control.InvalidateMeasure();
            control.InvalidateArrange();

            // Recursively invalidate children
            if (control is Avalonia.Controls.Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Avalonia.Controls.Control childControl)
                    {
                        InvalidateControlTree(childControl);
                    }
                }
            }
            else if (control is Avalonia.Controls.ContentControl contentControl && contentControl.Content is Avalonia.Controls.Control content)
            {
                InvalidateControlTree(content);
            }
            else if (control is Avalonia.Controls.Decorator decorator && decorator.Child is Avalonia.Controls.Control decoratorChild)
            {
                InvalidateControlTree(decoratorChild);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error invalidating control during theme update");
        }
    }

    /// <summary>
    /// Update the selected theme in the combo box.
    /// </summary>
    public void SetSelectedTheme(string themeId)
    {
        if (ThemeComboBox != null)
        {
            var matchingItem = ThemeComboBox.Items.OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == themeId);
            
            if (matchingItem != null)
            {
                ThemeComboBox.SelectedItem = matchingItem;
                _logger?.LogDebug("Updated ComboBox selection to theme: {ThemeId}", themeId);
            }
            else
            {
                _logger?.LogWarning("Could not find ComboBoxItem for theme: {ThemeId}", themeId);
            }
        }
    }

    /// <summary>
    /// Cleanup resources when the control is unloaded.
    /// </summary>
    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Unsubscribe from theme service events
            if (_themeService != null)
            {
                _themeService.ThemeChanged -= OnThemeServiceChanged;
            }

            _logger?.LogDebug("ThemeQuickSwitcher unloaded and cleaned up");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error during ThemeQuickSwitcher cleanup");
        }
    }

    /// <summary>
    /// Apply basic theme switching using Avalonia's built-in theme variants.
    /// This is a fallback when ThemeService is not available.
    /// </summary>
    private void ApplyBasicTheme(string themeId)
    {
        try
        {
            if (Avalonia.Application.Current != null)
            {
                var themeVariant = themeId.Contains("Dark") 
                    ? Avalonia.Styling.ThemeVariant.Dark 
                    : Avalonia.Styling.ThemeVariant.Light;
                
                Avalonia.Application.Current.RequestedThemeVariant = themeVariant;
                
                // CRITICAL FIX: Force immediate refresh for basic themes too
                _ = Task.Run(async () =>
                {
                    await Task.Delay(50);
                    await ForceApplicationThemeUpdateAsync();
                });
                
                _logger?.LogInformation("Applied basic theme variant: {ThemeVariant} for theme {ThemeId}", themeVariant, themeId);
                System.Diagnostics.Debug.WriteLine($"Applied basic theme variant: {themeVariant}");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error applying basic theme for {ThemeId}", themeId);
            System.Diagnostics.Debug.WriteLine($"Error applying basic theme: {ex.Message}");
        }
    }
}
