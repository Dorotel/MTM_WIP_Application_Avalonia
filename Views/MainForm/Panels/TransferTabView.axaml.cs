using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views
{
    /// <summary>
    /// Code-behind for TransferTabView.
    /// Implements the inventory transfer interface for moving inventory between operations within the MTM WIP Application.
    /// Follows the minimal code-behind pattern with business logic delegated to the ViewModel.
    /// Used for transferring parts between manufacturing operations (e.g., from operation "90" to "100").
    /// </summary>
    public partial class TransferTabView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the TransferTabView with minimal setup.
        /// Business logic and data binding are handled by the TransferItemViewModel.
        /// </summary>
        public TransferTabView()
        {
            InitializeComponent();
        }
    }
}