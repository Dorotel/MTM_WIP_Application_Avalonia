using System;
using System.Collections.Generic;
using System.IO;
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

    // Event to signal when the theme editor should open
    public event EventHandler? ThemeEditorRequested;

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
    private async void InitializeThemeDropdown()
    {
        try
        {
            if (_themeService != null && ThemeComboBox != null)
            {
                // Clear existing items
                ThemeComboBox.Items.Clear();

                // Add built-in themes
                AddBuiltInThemes();

                // Add custom themes
                await AddCustomThemesAsync();

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
    /// Add built-in MTM themes to the ComboBox.
    /// </summary>
    private void AddBuiltInThemes()
    {
        if (ThemeComboBox == null) return;

        // Core MTM Themes
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Default (Base)", Tag = "MTMTheme" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Light", Tag = "MTM_Light" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Dark", Tag = "MTM_Dark" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM High Contrast", Tag = "MTM_HighContrast" });

        // Professional Business Themes
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Professional Blue", Tag = "MTM_Blue" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Professional Blue Dark", Tag = "MTM_Blue_Dark" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Success Green", Tag = "MTM_Green" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Success Green Dark", Tag = "MTM_Green_Dark" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Focus Teal", Tag = "MTM_Teal" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Focus Teal Dark", Tag = "MTM_Teal_Dark" });

        // Manufacturing-Specialized Themes
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Alert Red", Tag = "MTM_Red" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Industrial Amber", Tag = "MTM_Amber" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Deep Indigo", Tag = "MTM_Indigo" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Deep Indigo Dark", Tag = "MTM_Indigo_Dark" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Soft Rose", Tag = "MTM_Rose" });
        ThemeComboBox.Items.Add(new ComboBoxItem { Content = "MTM Modern Emerald", Tag = "MTM_Emerald" });
    }

    /// <summary>
    /// Add custom themes from Resources/Themes/ directory to the ComboBox.
    /// </summary>
    private async Task AddCustomThemesAsync()
    {
        try
        {
            if (ThemeComboBox == null) return;

            var themesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Themes");

            if (!Directory.Exists(themesDirectory))
            {
                _logger?.LogDebug("Custom themes directory does not exist: {Directory}", themesDirectory);
                return;
            }

            var customThemeFiles = Directory.GetFiles(themesDirectory, "Custom_*.json");

            if (customThemeFiles.Length > 0)
            {
                // Add separator for custom themes
                var separator = new ComboBoxItem
                {
                    Content = "─── Custom Themes ───",
                    Tag = "separator",
                    IsEnabled = false
                };
                ThemeComboBox.Items.Add(separator);

                // Add custom theme items
                foreach (var filePath in customThemeFiles.OrderByDescending(f => new FileInfo(f).CreationTime))
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(filePath);
                        var themeInfo = System.Text.Json.JsonSerializer.Deserialize<ThemeExportModel>(json, new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        });

                        if (themeInfo != null && !string.IsNullOrWhiteSpace(themeInfo.Name))
                        {
                            var customThemeItem = new ComboBoxItem
                            {
                                Content = $"🎨 {themeInfo.Name} (Custom)",
                                Tag = Path.GetFileNameWithoutExtension(filePath)
                            };
                            ToolTip.SetTip(customThemeItem, $"Custom theme by {themeInfo.CreatedBy ?? "Unknown"}\nCreated: {new FileInfo(filePath).CreationTime:yyyy-MM-dd}");
                            ThemeComboBox.Items.Add(customThemeItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogWarning(ex, "Failed to read custom theme file: {FilePath}", filePath);
                    }
                }

                _logger?.LogInformation("Added {Count} custom themes to quick switcher", customThemeFiles.Length);
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding custom themes to quick switcher");
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

        // Wire up the edit theme button
        var editButton = this.FindControl<Button>("EditThemeButton");
        if (editButton != null)
        {
            editButton.Click += OnEditThemeButtonClicked;
        }
    }

    /// <summary>
    /// Handle edit theme button click.
    /// </summary>
    private void OnEditThemeButtonClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            _logger?.LogDebug("Edit theme button clicked, raising ThemeEditorRequested event");
            ThemeEditorRequested?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling edit theme button click");
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

/// <summary>
/// Simple theme export model for reading custom theme JSON files
/// </summary>
public class ThemeExportModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, string> Colors { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
}
