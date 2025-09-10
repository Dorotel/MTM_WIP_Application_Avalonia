using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Demo control for showcasing the CustomDataGrid functionality.
/// Provides sample inventory data to demonstrate the grid's capabilities
/// with proper MTM theme integration and data binding.
/// </summary>
public partial class CustomDataGridDemo : UserControl
{
    #region Properties

    private ObservableCollection<DemoInventoryItem> _sampleInventoryData = new();
    /// <summary>
    /// Sample inventory data for demonstration purposes.
    /// </summary>
    public ObservableCollection<DemoInventoryItem> SampleInventoryData
    {
        get => _sampleInventoryData;
        set 
        { 
            _sampleInventoryData = value;
            OnPropertyChanged(nameof(SampleInventoryData));
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    private DemoInventoryItem? _selectedInventoryItem;
    /// <summary>
    /// Currently selected inventory item.
    /// </summary>
    public DemoInventoryItem? SelectedInventoryItem
    {
        get => _selectedInventoryItem;
        set 
        { 
            _selectedInventoryItem = value;
            OnPropertyChanged(nameof(SelectedInventoryItem));
        }
    }

    /// <summary>
    /// Gets the count of items in the sample data.
    /// </summary>
    public int ItemCount => SampleInventoryData.Count;

    #endregion

    #region Constructor

    public CustomDataGridDemo()
    {
        InitializeComponent();
        DataContext = this;
        
        // Load initial sample data
        LoadSampleData();
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the Load Sample Data button click.
    /// </summary>
    private void OnLoadSampleDataClick(object? sender, RoutedEventArgs e)
    {
        LoadSampleData();
    }

    /// <summary>
    /// Handles the Clear Data button click.
    /// </summary>
    private void OnClearDataClick(object? sender, RoutedEventArgs e)
    {
        SampleInventoryData.Clear();
        OnPropertyChanged(nameof(ItemCount));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads sample inventory data for demonstration.
    /// Creates realistic MTM manufacturing data with various scenarios.
    /// </summary>
    private void LoadSampleData()
    {
        SampleInventoryData.Clear();

        var sampleData = new[]
        {
            new DemoInventoryItem
            {
                PartId = "MTM-001",
                Operation = "90",
                Location = "RECV-A1",
                Quantity = 150,
                LastUpdated = DateTime.Now.AddHours(-2),
                Notes = "Recently received - ready for processing"
            },
            new DemoInventoryItem
            {
                PartId = "ABC-123",
                Operation = "100",
                Location = "WIP-B2",
                Quantity = 45,
                LastUpdated = DateTime.Now.AddMinutes(-30),
                Notes = "First operation in progress"
            },
            new DemoInventoryItem
            {
                PartId = "XYZ-789",
                Operation = "110",
                Location = "WIP-C3",
                Quantity = 28,
                LastUpdated = DateTime.Now.AddHours(-1),
                Notes = "Second operation - quality check needed"
            },
            new DemoInventoryItem
            {
                PartId = "DEF-456",
                Operation = "120",
                Location = "FINAL-D1",
                Quantity = 75,
                LastUpdated = DateTime.Now.AddMinutes(-15),
                Notes = "Final operation - ready for shipping"
            },
            new DemoInventoryItem
            {
                PartId = "GHI-101",
                Operation = "90",
                Location = "RECV-A2",
                Quantity = 200,
                LastUpdated = DateTime.Now.AddHours(-4),
                Notes = "Large batch received - high priority"
            }
        };

        foreach (var item in sampleData)
        {
            SampleInventoryData.Add(item);
        }

        OnPropertyChanged(nameof(ItemCount));
    }

    #endregion

    #region INotifyPropertyChanged Implementation

    public new event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Initialization

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    #endregion
}