using Avalonia.Controls;
using Avalonia.Interactivity;

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
