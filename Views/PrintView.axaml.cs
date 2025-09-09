using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views;

/// <summary>
/// PrintView - Full-window print interface for MTM WIP Application
/// Provides print options, real-time preview, and layout customization
/// Uses minimal code-behind following established MTM pattern
/// </summary>
public partial class PrintView : UserControl
{
    public PrintView()
    {
        InitializeComponent();
        // Minimal initialization only - ViewModel injected via DI
    }
}