using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.MainForm;

public partial class TestControlsView : UserControl
{
    public TestControlsView()
    {
        try
        {
            InitializeComponent();
        }
        catch (System.Exception ex)
        {
            // Log the exception for debugging
            System.Diagnostics.Debug.WriteLine($"TestControlsView initialization error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
