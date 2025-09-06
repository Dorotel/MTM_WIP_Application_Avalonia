using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for RemovePartView.
/// Implements the interface for removing manufacturing parts from the MTM WIP Application.
/// Used in the settings form for master data management when parts are no longer needed.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Includes safety measures to prevent accidental deletion of parts with active inventory.
/// </summary>
public partial class RemovePartView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the RemovePartView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public RemovePartView() { InitializeComponent(); }
}