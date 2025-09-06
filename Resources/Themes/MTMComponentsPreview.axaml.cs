using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Data;

namespace MTM_WIP_Application_Avalonia.Resources.Themes
{
    /// <summary>
    /// Preview control that demonstrates MTM application themes and component styling in manufacturing contexts.
    /// This control provides a comprehensive showcase of how different UI components appear with various
    /// MTM theme configurations (MTM_Blue, MTM_Green, MTM_Dark, MTM_Red), allowing system administrators
    /// and developers to evaluate theme effectiveness in manufacturing environments.
    /// 
    /// Component demonstrations include:
    /// - DataGrid styling with sample manufacturing inventory data
    /// - Button and control styling across different manufacturing contexts
    /// - Typography and color scheme effectiveness for manufacturing readability
    /// - Manufacturing-specific data presentation and formatting
    /// - Accessibility and visibility considerations for industrial environments
    /// 
    /// Sample data represents typical MTM manufacturing scenarios:
    /// - Part IDs with various naming conventions (ABC-001, MTM-502, etc.)
    /// - Operation workflow steps (90=Receiving, 100=First Op, 110=Second Op)
    /// - Manufacturing locations (MAIN-A1, WIP-B2, SHIP-C1, etc.)
    /// - Inventory quantities and item type classifications
    /// - Manufacturing transaction timestamps and operational notes
    /// 
    /// Used for theme evaluation and design validation before production deployment.
    /// Essential for ensuring UI effectiveness in various manufacturing lighting conditions.
    /// </summary>
    public partial class MTMComponentsPreview : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MTMComponentsPreview control.
        /// Sets up the visual tree and ensures proper initialization of sample data
        /// for manufacturing theme demonstration and evaluation purposes.
        /// </summary>
        public MTMComponentsPreview()
        {
            AvaloniaXamlLoader.Load(this);
            // Ensure DataGrid is populated after the control is fully initialized
            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        /// <summary>
        /// Event handler called when the control is attached to the visual tree.
        /// Triggers population of sample manufacturing data for theme demonstration,
        /// ensuring all components display properly with realistic manufacturing content.
        /// </summary>
        /// <param name="sender">The control being attached to visual tree</param>
        /// <param name="e">Visual tree attachment event arguments</param>
        private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            PopulateDataGrid();
        }

        /// <summary>
        /// Populates the sample DataGrid with representative manufacturing inventory data.
        /// Creates realistic manufacturing scenarios to demonstrate theme effectiveness
        /// and component styling in typical MTM operational contexts including various
        /// part types, operation stages, and manufacturing locations.
        /// 
        /// Sample data includes:
        /// - Diverse part ID formats reflecting real manufacturing naming conventions
        /// - Operation progression through manufacturing workflow (90→100→110)
        /// - Various manufacturing locations and facility areas
        /// - Realistic quantity ranges from low stock alerts to bulk inventory
        /// - Item type classifications for different manufacturing materials
        /// - Recent timestamps reflecting active manufacturing operations
        /// </summary>
        private void PopulateDataGrid()
        {
            var dataGrid = this.FindControl<DataGrid>("SampleDataGrid");
            if (dataGrid == null) return;

            // Create a simple list of objects for the DataGrid
            var inventoryData = new List<object>
            {
                new { PartId = "ABC-001", Operation = "90", Location = "MAIN-A1", Quantity = 45, ItemType = "STEEL", LastUpdated = "12/19/2024", Notes = "Ready for processing" },
                new { PartId = "MTM-502", Operation = "100", Location = "WIP-B2", Quantity = 12, ItemType = "ALUMINUM", LastUpdated = "12/19/2024", Notes = "In progress - Operation 100" },
                new { PartId = "PART-789", Operation = "110", Location = "SHIP-C1", Quantity = 8, ItemType = "FINISHED", LastUpdated = "12/18/2024", Notes = "Quality check complete" },
                new { PartId = "XYZ-999", Operation = "90", Location = "RAW-A3", Quantity = 150, ItemType = "RAW", LastUpdated = "12/17/2024", Notes = "Bulk inventory stock" },
                new { PartId = "DEF-456", Operation = "100", Location = "MACH-B1", Quantity = 3, ItemType = "MACHINED", LastUpdated = "12/19/2024", Notes = "Low stock - reorder needed" },
                new { PartId = "GHI-123", Operation = "110", Location = "QC-D1", Quantity = 25, ItemType = "ASSEMBLY", LastUpdated = "12/19/2024", Notes = "Awaiting final inspection" }
            };

            // Set the ItemsSource directly
            dataGrid.ItemsSource = inventoryData;
        }
    }
}
