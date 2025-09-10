using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;

/// <summary>
/// Wrapper class that adds selection functionality to any data item.
/// Used by CustomDataGrid to support multi-selection with checkboxes.
/// Follows MVVM Community Toolkit patterns for consistency with MTM architecture.
/// </summary>
public partial class SelectableItem<T> : ObservableObject where T : class
{
    /// <summary>
    /// The underlying data item.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// Gets or sets whether this item is selected.
    /// </summary>
    [ObservableProperty]
    private bool isSelected;

    /// <summary>
    /// Initializes a new instance of the SelectableItem class.
    /// </summary>
    /// <param name="item">The data item to wrap</param>
    public SelectableItem(T item)
    {
        Item = item ?? throw new System.ArgumentNullException(nameof(item));
    }

    /// <summary>
    /// Returns the string representation of the underlying item.
    /// </summary>
    public override string ToString()
    {
        return Item.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Implicit conversion from SelectableItem to the underlying item type.
    /// </summary>
    public static implicit operator T(SelectableItem<T> selectableItem)
    {
        return selectableItem.Item;
    }

    /// <summary>
    /// Implicit conversion from item type to SelectableItem.
    /// </summary>
    public static implicit operator SelectableItem<T>(T item)
    {
        return new SelectableItem<T>(item);
    }
}

/// <summary>
/// Non-generic version for object-based binding scenarios.
/// </summary>
public partial class SelectableItem : ObservableObject
{
    /// <summary>
    /// The underlying data item.
    /// </summary>
    public object Item { get; }

    /// <summary>
    /// Gets or sets whether this item is selected.
    /// </summary>
    [ObservableProperty]
    private bool isSelected;

    /// <summary>
    /// Initializes a new instance of the SelectableItem class.
    /// </summary>
    /// <param name="item">The data item to wrap</param>
    public SelectableItem(object item)
    {
        Item = item ?? throw new System.ArgumentNullException(nameof(item));
    }

    /// <summary>
    /// Returns the string representation of the underlying item.
    /// </summary>
    public override string ToString()
    {
        return Item.ToString() ?? string.Empty;
    }
}