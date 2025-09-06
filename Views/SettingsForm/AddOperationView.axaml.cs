using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for AddOperationView.
/// Implements the interface for adding new manufacturing operations to the MTM WIP Application.
/// Used in the settings form for master data management of manufacturing operations.
/// Operations represent workflow steps in the manufacturing process (e.g., "90", "100", "110").
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// </summary>
public partial class AddOperationView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the AddOperationView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public AddOperationView() { InitializeComponent(); }
}