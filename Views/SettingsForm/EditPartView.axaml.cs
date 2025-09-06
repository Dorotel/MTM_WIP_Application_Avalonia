using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for EditPartView.
/// Implements the interface for editing existing manufacturing parts in the MTM WIP Application.
/// Used in the settings form for master data management to update part descriptions, customers, and classifications.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Supports modification of part details while maintaining referential integrity with inventory records.
/// </summary>
public partial class EditPartView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the EditPartView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public EditPartView() { InitializeComponent(); }
}