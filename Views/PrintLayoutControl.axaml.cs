using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// PrintLayoutControl - Column customization interface for print operations
/// Provides column visibility toggles, ordering, and layout options
/// Uses minimal code-behind following established MTM pattern
/// </summary>
public partial class PrintLayoutControl : UserControl
{
    public PrintLayoutControl()
    {
        InitializeComponent();
        // Minimal initialization only - ViewModel injected via DI
    }
}