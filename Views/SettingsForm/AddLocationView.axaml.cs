using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for AddLocationView.
/// Implements the interface for adding new manufacturing locations to the MTM WIP Application.
/// Used in the settings form for master data management of manufacturing locations.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Locations are used to track physical areas where inventory operations occur.
/// </summary>
public partial class AddLocationView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the AddLocationView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public AddLocationView() { InitializeComponent(); }
}