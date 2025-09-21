using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for AddItemTypeView.
/// Implements the interface for adding new item types to the MTM WIP Application.
/// Used in the settings form for master data management to classify parts by type (e.g., "WIP", "Raw Material", "Finished Good").
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Item types are used for categorization and reporting within the manufacturing system.
/// </summary>
public partial class AddItemTypeView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the AddItemTypeView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public AddItemTypeView() { InitializeComponent(); }
}
