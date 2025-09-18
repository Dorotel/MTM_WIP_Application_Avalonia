using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;

namespace MTM_WIP_Application_Avalonia
{
    public class SortingTestApp : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new SortingTestWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

    public class SortingTestWindow : Window
    {
        public SortingTestWindow()
        {
            Title = "MTM CustomDataGrid - Phase 2 Sorting Test";
            Width = 1200;
            Height = 800;
            
            // Create test data
            var testData = new ObservableCollection<InventoryItem>
            {
                new() { PartId = "PART001", Operation = "100", Quantity = 25, Location = "STATION_A" },
                new() { PartId = "PART002", Operation = "90", Quantity = 10, Location = "STATION_B" },
                new() { PartId = "PART003", Operation = "110", Quantity = 15, Location = "STATION_A" },
                new() { PartId = "PART004", Operation = "100", Quantity = 5, Location = "STATION_C" },
                new() { PartId = "PART005", Operation = "120", Quantity = 30, Location = "STATION_B" },
                new() { PartId = "ABC-001", Operation = "90", Quantity = 8, Location = "STATION_A" },
                new() { PartId = "XYZ-999", Operation = "110", Quantity = 12, Location = "STATION_C" },
            };

            // Create CustomDataGrid
            var dataGrid = new CustomDataGrid
            {
                ItemsSource = testData,
                IsSortingEnabled = true,
                IsMultiColumnSortEnabled = true
            };

            // Create a simple layout
            var stackPanel = new StackPanel();
            
            var titleBlock = new TextBlock
            {
                Text = "CustomDataGrid Phase 2 Sorting Test",
                FontSize = 18,
                FontWeight = Avalonia.Media.FontWeight.Bold,
                Margin = new Avalonia.Thickness(10)
            };

            var instructionBlock = new TextBlock
            {
                Text = "• Click column headers to sort (toggles: None → Ascending → Descending)\n" +
                       "• Hold Shift + Click to add secondary sorts (up to 3 columns)\n" +
                       "• Sort indicators (↑↓) show current sort direction and precedence",
                FontSize = 12,
                Margin = new Avalonia.Thickness(10, 0, 10, 10)
            };

            stackPanel.Children.Add(titleBlock);
            stackPanel.Children.Add(instructionBlock);
            stackPanel.Children.Add(dataGrid);

            Content = stackPanel;
        }
    }

    public class SortingTestProgram
    {
        public static void Main(string[] args)
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<SortingTestApp>()
                .UsePlatformDetect()
                .LogToTrace();
        }
    }
}