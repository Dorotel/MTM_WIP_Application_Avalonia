using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views;

public partial class RemoveTabView : UserControl
{
    public RemoveTabView()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
    }
}
