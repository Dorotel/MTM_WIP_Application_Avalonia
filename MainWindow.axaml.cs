using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;

namespace MTM_WIP_Application_Avalonia
{
    /// <summary>
    /// Main application window for the MTM Work-in-Process (WIP) manufacturing inventory management system.
    /// Serves as the primary entry point and container for all manufacturing operations including inventory tracking,
    /// part management, transaction processing, and system administration.
    /// 
    /// This window hosts the MainView which provides access to manufacturing workflow operations:
    /// - Inventory management (add, remove, transfer parts between operations)
    /// - Transaction history and reporting
    /// - Quick action buttons for common manufacturing tasks
    /// - Settings and system administration
    /// 
    /// The window follows MVVM architecture with minimal code-behind, delegating business logic
    /// to the MainWindowViewModel and coordinating with various manufacturing services.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow.
        /// Component initialization is handled automatically by Avalonia framework.
        /// The DataContext (MainWindowViewModel) is injected via dependency injection
        /// and provides all manufacturing business logic and data management.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }  
}
