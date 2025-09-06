using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for AboutView - displays application information and version details.
/// Provides system information, version history, and support contact details for the MTM WIP Application.
/// This view presents essential information for manufacturing system administrators and support staff
/// including application version, build information, and contact details for technical support.
/// 
/// Key information displayed:
/// - Application name and current version
/// - Build date and system information  
/// - Manufacturing domain context and usage guidelines
/// - Technical support and maintenance contact information
/// - License and copyright information
/// 
/// Follows minimal code-behind pattern with all presentation logic handled by corresponding ViewModel.
/// </summary>
public partial class AboutView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the AboutView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// The DataContext (AboutViewModel) provides version information, system details,
    /// and support contact information for the manufacturing application.
    /// </summary>
    public AboutView() { InitializeComponent(); }
}