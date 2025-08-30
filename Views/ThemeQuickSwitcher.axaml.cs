using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
            _logger?.LogDebug("ThemeQuickSwitcher OnLoaded event started");
            
            InitializeEventHandlers();
            _logger?.LogInformation("ThemeQuickSwitcher initialized successfully");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error initializing ThemeQuickSwitcher");
            System.Diagnostics.Debug.WriteLine($"Error initializing ThemeQuickSwitcher: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine($"Error applying theme: {ex.Message}");
        }
    }

    /// <summary>
    /// Handle theme selection change.
    /// </summary>
    private void OnThemeSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // You could implement immediate theme preview here
        // For now, we require explicit Apply button click
    }

    /// <summary>
    /// Apply the selected theme.
    /// </summary>
    private async System.Threading.Tasks.Task ApplyThemeAsync(string themeId)
    {
        try
        {
            // Try to get theme service from application services
            _themeService ??= GetThemeService();
            
            if (_themeService != null)
            {
                var result = await _themeService.SetThemeAsync(themeId);
                
                if (result.IsSuccess)
                {
                    System.Diagnostics.Debug.WriteLine($"Theme applied successfully: {themeId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to apply theme: {result.Message}");
                }
            }
            else
            {
                // Fallback: Apply basic theme switching via Avalonia's built-in mechanism
                ApplyBasicTheme(themeId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ApplyThemeAsync: {ex.Message}");
            
            // Fallback to basic theme switching
            ApplyBasicTheme(themeId);
        }
    }

    /// <summary>
    /// Get theme service from application services.
    /// </summary>
    private IThemeService? GetThemeService()
    {
        try
        {
            // Get the service from the application's service provider via Program class
            return Program.GetOptionalService<IThemeService>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting ThemeService: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Apply basic theme switching using Avalonia's built-in theme variants.
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
                
                System.Diagnostics.Debug.WriteLine($"Applied basic theme variant: {themeVariant}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error applying basic theme: {ex.Message}");
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
            }
        }
    }
}