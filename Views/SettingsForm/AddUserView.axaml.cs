using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for AddUserView.
/// Implements the interface for adding new users to the MTM WIP Application.
/// Used in the settings form for user management and access control.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Users are tracked for audit purposes and access permissions within the manufacturing system.
/// </summary>
public partial class AddUserView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the AddUserView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public AddUserView() { InitializeComponent(); }
}