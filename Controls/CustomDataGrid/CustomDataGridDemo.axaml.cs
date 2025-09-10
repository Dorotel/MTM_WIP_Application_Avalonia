using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

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

    private ObservableCollection<object> _selectedItems = new();
    /// <summary>
    /// Selected items for multi-selection demonstration.
    /// </summary>
    public ObservableCollection<object> SelectedItems
    {
        get => _selectedItems;
        set 
        { 
            _selectedItems = value;
            OnPropertyChanged(nameof(SelectedItems));
            OnPropertyChanged(nameof(SelectedItemCount));
        }
    }

    private bool _isMultiSelectEnabled = false;
    /// <summary>
    /// Gets or sets whether multi-selection is enabled for demonstration.
    /// </summary>
    public bool IsMultiSelectEnabled
    {
        get => _isMultiSelectEnabled;
        set 
        { 
            _isMultiSelectEnabled = value;
            OnPropertyChanged(nameof(IsMultiSelectEnabled));
        }
    }

    /// <summary>
    /// Gets the count of selected items.
    /// </summary>
    public int SelectedItemCount => SelectedItems.Count;

    /// <summary>
    /// Gets the count of items in the sample data.
    /// </summary>
    public int ItemCount => SampleInventoryData.Count;

    #endregion

    #region Commands

    /// <summary>
    /// Command to delete an item from the grid.
    /// </summary>
    public ICommand DeleteItemCommand => new RelayCommand<object>(DeleteItem);

    /// <summary>
    /// Command to edit an item in the grid.
    /// </summary>
    public ICommand EditItemCommand => new RelayCommand<object>(EditItem);

    /// <summary>
    /// Command to duplicate an item in the grid.
    /// </summary>
    public ICommand DuplicateItemCommand => new RelayCommand<object>(DuplicateItem);

    /// <summary>
    /// Command to view details of an item.
    /// </summary>
    public ICommand ViewDetailsCommand => new RelayCommand<object>(ViewDetails);

    /// <summary>
    /// Command to toggle multi-selection mode.
    /// </summary>
    public ICommand ToggleMultiSelectCommand => new RelayCommand(ToggleMultiSelect);

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
        SelectedItems.Clear();
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(SelectedItemCount));
    }

    #endregion

    #region Command Implementations

    /// <summary>
    /// Deletes the specified item from the grid.
    /// </summary>
    private void DeleteItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item && SampleInventoryData.Contains(item))
        {
            SampleInventoryData.Remove(item);
            SelectedItems.Remove(item);
            OnPropertyChanged(nameof(ItemCount));
            OnPropertyChanged(nameof(SelectedItemCount));
        }
    }

    /// <summary>
    /// Edits the specified item (demo implementation shows details).
    /// </summary>
    private void EditItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            // For demo purposes, simulate edit action by updating notes
            item.Notes = $"{item.Notes} [Edited at {DateTime.Now:HH:mm:ss}]";
            OnPropertyChanged(nameof(SampleInventoryData));
        }
    }

    /// <summary>
    /// Duplicates the specified item.
    /// </summary>
    private void DuplicateItem(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            var duplicate = DemoInventoryItem.CreateDemo(
                $"{item.PartId}-COPY",
                item.Operation,
                item.Location,
                item.Quantity,
                $"Copy of {item.PartId} - {item.Notes}"
            );
            
            SampleInventoryData.Add(duplicate);
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    /// <summary>
    /// Views details of the specified item.
    /// </summary>
    private void ViewDetails(object? parameter)
    {
        if (parameter is DemoInventoryItem item)
        {
            // For demo purposes, select the item to show it's been viewed
            SelectedInventoryItem = item;
            
            // In a real implementation, this would open a details view or dialog
            System.Diagnostics.Debug.WriteLine($"Viewing details for: {item.PartId} - {item.DisplayText}");
        }
    }

    /// <summary>
    /// Toggles multi-selection mode on and off.
    /// </summary>
    private void ToggleMultiSelect()
    {
        IsMultiSelectEnabled = !IsMultiSelectEnabled;
        if (!IsMultiSelectEnabled)
        {
            SelectedItems.Clear();
            OnPropertyChanged(nameof(SelectedItemCount));
        }
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
            DemoInventoryItem.CreateDemo("MTM-001", "90", "RECV-A1", 150, "Recently received - ready for processing"),
            DemoInventoryItem.CreateDemo("ABC-123", "100", "WIP-B2", 45, "First operation in progress"),
            DemoInventoryItem.CreateDemo("XYZ-789", "110", "WIP-C3", 28, "Second operation - quality check needed"),
            DemoInventoryItem.CreateDemo("DEF-456", "120", "FINAL-D1", 75, "Final operation - ready for shipping"),
            DemoInventoryItem.CreateDemo("GHI-101", "90", "RECV-A2", 200, "Large batch received - high priority"),
            DemoInventoryItem.CreateDemo("JKL-202", "100", "WIP-B3", 60, "Standard processing"),
            DemoInventoryItem.CreateDemo("MNO-303", "110", "WIP-C1", 35, "Quality testing in progress"),
            DemoInventoryItem.CreateDemo("PQR-404", "120", "FINAL-D2", 90, "Final inspection complete"),
            DemoInventoryItem.CreateDemo("STU-505", "90", "RECV-A3", 180, "Urgent order - expedite processing"),
            DemoInventoryItem.CreateDemo("VWX-606", "100", "WIP-B1", 25, "Small batch - special handling required")
        };

        foreach (var item in sampleData)
        {
            SampleInventoryData.Add(item);
        }

        // Clear selection when loading new data
        SelectedItems.Clear();
        
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(SelectedItemCount));
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