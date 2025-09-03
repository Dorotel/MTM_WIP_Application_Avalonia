using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Data;

namespace MTM_WIP_Application_Avalonia.Resources.Themes
{
    public partial class MTMComponentsPreview : UserControl
    {
        public MTMComponentsPreview()
        {
            AvaloniaXamlLoader.Load(this);
            // Ensure DataGrid is populated after the control is fully initialized
            this.AttachedToVisualTree += OnAttachedToVisualTree;
        }

        private void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            PopulateDataGrid();
        }

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
