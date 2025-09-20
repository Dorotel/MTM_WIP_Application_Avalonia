using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// Code-behind for EmergencyShutdownOverlayView.
/// Provides emergency shutdown functionality with countdown timer, graceful shutdown options,
/// and critical data saving capabilities. Follows MTM design system and Avalonia best practices.
/// </summary>
public partial class EmergencyShutdownOverlayView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the EmergencyShutdownOverlayView class.
    /// </summary>
    public EmergencyShutdownOverlayView()
    {
        InitializeComponent();
    }
}
