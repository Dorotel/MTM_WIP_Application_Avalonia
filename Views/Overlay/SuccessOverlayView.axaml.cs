using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// Code-behind for SuccessOverlayView - provides temporary success message display
/// </summary>
public partial class SuccessOverlayView : UserControl
{
    public SuccessOverlayView()
    {
        InitializeComponent();
        
        // Handle loaded event for focus management
        Loaded += OnSuccessOverlayLoaded;
        
        // Handle key presses for emergency shutdown
        KeyDown += OnKeyDown;
    }

    /// <summary>
    /// Handles key presses for emergency functions when UI might be locked
    /// </summary>
    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        try
        {
            // Emergency shutdown: Ctrl+Alt+Q (when UI is locked up)
            if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Alt) && e.Key == Key.Q)
            {
                if (DataContext is SuccessOverlayViewModel viewModel && viewModel.IsError)
                {
                    System.Diagnostics.Debug.WriteLine("Emergency shutdown triggered via Ctrl+Alt+Q");
                    await viewModel.ExitApplicationCommand.ExecuteAsync(null);
                    e.Handled = true;
                }
            }
            
            // Emergency continue: Ctrl+Alt+C (when UI is locked up)
            if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Alt) && e.Key == Key.C)
            {
                if (DataContext is SuccessOverlayViewModel viewModel && viewModel.IsError)
                {
                    System.Diagnostics.Debug.WriteLine("Emergency continue triggered via Ctrl+Alt+C");
                    await viewModel.ContinueCommand.ExecuteAsync(null);
                    e.Handled = true;
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in emergency key handler: {ex.Message}");
            // If even the emergency handler fails, force exit
            if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Alt) && e.Key == Key.Q)
            {
                System.Environment.Exit(1);
            }
        }
    }

    /// <summary>
    /// Handles the loaded event - ensures overlay doesn't interfere with focus
    /// </summary>
    private void OnSuccessOverlayLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Ensure the overlay doesn't steal focus from the parent view
            // The overlay should be purely visual and not interfere with tab navigation
            this.Focusable = false;
            
            // Make sure child controls also don't interfere with focus
            var contentGrid = this.FindControl<Grid>("ContentGrid");
            if (contentGrid != null)
            {
                contentGrid.Focusable = false;
            }
            
            var successCard = this.FindControl<Border>("SuccessCard");
            if (successCard != null)
            {
                successCard.Focusable = false;
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in SuccessOverlayView OnLoaded: {ex.Message}");
        }
    }
}
