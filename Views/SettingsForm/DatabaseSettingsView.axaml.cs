using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.Views.SettingsForm;

/// <summary>
/// Code-behind for DatabaseSettingsView.
/// Implements the interface for configuring database connection settings within the MTM WIP Application.
/// Used in the settings form for system administration to manage MySQL connection parameters.
/// Follows minimal code-behind pattern with business logic handled by corresponding ViewModel.
/// Provides secure configuration management for database connectivity and performance tuning.
/// </summary>
public partial class DatabaseSettingsView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the DatabaseSettingsView.
    /// Component initialization is handled automatically by Avalonia framework.
    /// </summary>
    public DatabaseSettingsView()
    {
        InitializeComponent();
    }
}