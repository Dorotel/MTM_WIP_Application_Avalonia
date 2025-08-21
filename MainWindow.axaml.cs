using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Set DataContext to ensure MainView is the default view
            DataContext = new ViewModels.MainWindowViewModel();
        }
    }
}