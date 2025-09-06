using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for SystemHealthView.
/// Implements the interface for monitoring system health and connectivity status within the MTM WIP Application.
/// Used in the settings form to display database connectivity, service status, and application performance metrics.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Provides administrators with real-time system monitoring capabilities.
/// </summary>
public partial class SystemHealthView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the SystemHealthView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public SystemHealthView()
    {
        InitializeComponent();
    }
}