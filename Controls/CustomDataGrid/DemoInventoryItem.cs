using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Simple inventory item model for demonstration purposes.
/// Contains the basic properties expected by the CustomDataGrid demo.
/// Implements INotifyPropertyChanged for selection support using MVVM Community Toolkit.
/// </summary>
public partial class DemoInventoryItem : ObservableObject
{
    [ObservableProperty]
    private string partId = string.Empty;
    
    [ObservableProperty]
    private string operation = string.Empty;
    
    [ObservableProperty]
    private string location = string.Empty;
    
    [ObservableProperty]
    private int quantity;
    
    [ObservableProperty]
    private DateTime lastUpdated;
    
    [ObservableProperty]
    private string notes = string.Empty;
    
    [ObservableProperty]
    private bool isSelected;

    /// <summary>
    /// Gets the display text for the inventory item in lists.
    /// Format: (Operation) - [PartId x Quantity]
    /// </summary>
    public string DisplayText => $"({Operation}) - [{PartId} x {Quantity}]";

    /// <summary>
    /// Creates a demo inventory item with default values for testing.
    /// </summary>
    public static DemoInventoryItem CreateDemo(string partId, string operation, string location, int quantity, string notes = "")
    {
        return new DemoInventoryItem
        {
            PartId = partId,
            Operation = operation,
            Location = location,
            Quantity = quantity,
            LastUpdated = DateTime.Now.AddHours(-Random.Shared.Next(1, 72)),
            Notes = notes
        };
    }
}