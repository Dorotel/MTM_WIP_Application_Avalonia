using Avalonia.Controls;
using MTM_WIP_Application_Avalonia.ViewModels.Overlay;

namespace MTM_WIP_Application_Avalonia.Views.Overlay;

/// <summary>
/// Comprehensive inventory item edit dialog view.
/// Provides UI for editing all inventory fields with validation and status indicators.
/// </summary>
public partial class EditInventoryView : UserControl
{
    public EditInventoryView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Constructor with ViewModel injection for testing and explicit initialization.
    /// </summary>
    /// <param name="viewModel">The EditInventoryViewModel to bind to this view</param>
    public EditInventoryView(EditInventoryViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }
}
