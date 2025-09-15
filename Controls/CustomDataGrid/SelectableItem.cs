using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Controls.CustomDataGrid
{
    /// <summary>
    /// Base class for items that can be selected in the CustomDataGrid
    /// Provides selection state management and change notification
    /// Follows MTM MVVM Community Toolkit patterns for consistency
    /// </summary>
    public partial class SelectableItem : ObservableObject
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets whether this item is selected
        /// </summary>
        [ObservableProperty]
        private bool _isSelected;

        /// <summary>
        /// Gets or sets whether this item is enabled for selection
        /// </summary>
        [ObservableProperty]
        private bool _isEnabled = true;

        /// <summary>
        /// Gets or sets whether this item is highlighted (hover state)
        /// </summary>
        [ObservableProperty]
        private bool _isHighlighted;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when the selection state changes
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

        #endregion

        #region Property Change Handlers

        /// <summary>
        /// Handles changes to the IsSelected property
        /// </summary>
        /// <param name="value">The new value</param>
        partial void OnIsSelectedChanged(bool value)
        {
            // Raise the SelectionChanged event
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
            {
                SelectedItems = value ? new System.Collections.Generic.List<object> { this } : new(),
                SelectionMode = value ? "Selected" : "Deselected",
                Timestamp = DateTime.Now
            });

            // Additional selection logic can be added here
            OnSelectionStateChanged(value);
        }

        /// <summary>
        /// Virtual method that derived classes can override to handle selection changes
        /// </summary>
        /// <param name="isSelected">The new selection state</param>
        protected virtual void OnSelectionStateChanged(bool isSelected)
        {
            // Default implementation - can be overridden by derived classes
        }

        #endregion

        #region Methods

        /// <summary>
        /// Toggles the selection state of this item
        /// </summary>
        public virtual void ToggleSelection()
        {
            if (IsEnabled)
            {
                IsSelected = !IsSelected;
            }
        }

        /// <summary>
        /// Selects this item if it's enabled
        /// </summary>
        public virtual void Select()
        {
            if (IsEnabled)
            {
                IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects this item
        /// </summary>
        public virtual void Deselect()
        {
            IsSelected = false;
        }

        /// <summary>
        /// Sets the selection state programmatically
        /// </summary>
        /// <param name="selected">The desired selection state</param>
        /// <param name="force">Whether to force the change even if disabled</param>
        public virtual void SetSelected(bool selected, bool force = false)
        {
            if (force || IsEnabled)
            {
                IsSelected = selected;
            }
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this selectable item
        /// </summary>
        public override string ToString()
        {
            return $"SelectableItem - Selected: {IsSelected}, Enabled: {IsEnabled}";
        }

        #endregion
    }

    /// <summary>
    /// Generic selectable item wrapper that can hold any data object
    /// Useful for making existing data models selectable without modification
    /// </summary>
    /// <typeparam name="T">The type of data this item contains</typeparam>
    public partial class SelectableItem<T> : SelectableItem where T : class
    {
        #region Observable Properties

        /// <summary>
        /// Gets or sets the wrapped data item
        /// </summary>
        [ObservableProperty]
        private T? _data;

        /// <summary>
        /// Gets or sets a display text for this item
        /// </summary>
        [ObservableProperty]
        private string _displayText = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SelectableItem class
        /// </summary>
        public SelectableItem()
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified data
        /// </summary>
        /// <param name="data">The data to wrap</param>
        public SelectableItem(T data)
        {
            Data = data;
            DisplayText = data?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance with data and display text
        /// </summary>
        /// <param name="data">The data to wrap</param>
        /// <param name="displayText">The display text</param>
        public SelectableItem(T data, string displayText)
        {
            Data = data;
            DisplayText = displayText;
        }

        #endregion

        #region Property Change Handlers

        /// <summary>
        /// Handles changes to the Data property
        /// </summary>
        /// <param name="value">The new data value</param>
        partial void OnDataChanged(T? value)
        {
            // Update display text when data changes
            if (string.IsNullOrEmpty(DisplayText) || DisplayText == Data?.ToString())
            {
                DisplayText = value?.ToString() ?? string.Empty;
            }

            // Subscribe/unsubscribe from data change notifications if the data implements INotifyPropertyChanged
            OnDataObjectChanged(value);
        }

        /// <summary>
        /// Handles data object change notifications
        /// </summary>
        /// <param name="newData">The new data object</param>
        protected virtual void OnDataObjectChanged(T? newData)
        {
            // If data implements INotifyPropertyChanged, we could subscribe to its changes
            // This allows the selectable wrapper to react to changes in the wrapped data
            if (newData is INotifyPropertyChanged notifyPropertyChanged)
            {
                // Could add subscription logic here if needed
                // notifyPropertyChanged.PropertyChanged += OnWrappedDataPropertyChanged;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the wrapped data, or creates a default instance if null
        /// </summary>
        /// <returns>The data object</returns>
        public T GetDataOrDefault()
        {
            if (Data == null && typeof(T).GetConstructor(Type.EmptyTypes) != null)
            {
                Data = Activator.CreateInstance<T>();
            }
            return Data!;
        }

        /// <summary>
        /// Determines if this item has valid data
        /// </summary>
        /// <returns>True if data is not null, false otherwise</returns>
        public bool HasData()
        {
            return Data != null;
        }

        /// <summary>
        /// Updates the display text based on the current data
        /// </summary>
        public void RefreshDisplayText()
        {
            DisplayText = Data?.ToString() ?? string.Empty;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this selectable item with data info
        /// </summary>
        public override string ToString()
        {
            var dataInfo = Data?.GetType().Name ?? "null";
            return $"SelectableItem<{typeof(T).Name}> - Data: {dataInfo}, Selected: {IsSelected}, Display: {DisplayText}";
        }

        /// <summary>
        /// Determines equality based on the wrapped data
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is SelectableItem<T> other)
            {
                return Equals(Data, other.Data);
            }
            
            // Also allow direct comparison with the data type
            if (obj is T directData)
            {
                return Equals(Data, directData);
            }

            return false;
        }

        /// <summary>
        /// Gets hash code based on the wrapped data
        /// </summary>
        public override int GetHashCode()
        {
            return Data?.GetHashCode() ?? 0;
        }

        #endregion
    }

    /// <summary>
    /// Collection of utility methods for working with selectable items
    /// </summary>
    public static class SelectableItemExtensions
    {
        /// <summary>
        /// Converts a collection of items to selectable items
        /// </summary>
        /// <typeparam name="T">The type of items to convert</typeparam>
        /// <param name="items">The items to convert</param>
        /// <returns>Collection of selectable items</returns>
        public static System.Collections.Generic.IEnumerable<SelectableItem<T>> ToSelectableItems<T>(
            this System.Collections.Generic.IEnumerable<T> items) where T : class
        {
            return items.Select(item => new SelectableItem<T>(item));
        }

        /// <summary>
        /// Gets all selected items from a collection of selectable items
        /// </summary>
        /// <typeparam name="T">The type of wrapped data</typeparam>
        /// <param name="items">The collection of selectable items</param>
        /// <returns>Collection of selected data items</returns>
        public static System.Collections.Generic.IEnumerable<T> GetSelectedData<T>(
            this System.Collections.Generic.IEnumerable<SelectableItem<T>> items) where T : class
        {
            return items.Where(item => item.IsSelected && item.HasData()).Select(item => item.Data!);
        }

        /// <summary>
        /// Selects all items in the collection
        /// </summary>
        /// <param name="items">The items to select</param>
        public static void SelectAll(this System.Collections.Generic.IEnumerable<SelectableItem> items)
        {
            foreach (var item in items.Where(i => i.IsEnabled))
            {
                item.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselects all items in the collection
        /// </summary>
        /// <param name="items">The items to deselect</param>
        public static void DeselectAll(this System.Collections.Generic.IEnumerable<SelectableItem> items)
        {
            foreach (var item in items)
            {
                item.IsSelected = false;
            }
        }

        /// <summary>
        /// Gets the count of selected items
        /// </summary>
        /// <param name="items">The items to count</param>
        /// <returns>Number of selected items</returns>
        public static int GetSelectedCount(this System.Collections.Generic.IEnumerable<SelectableItem> items)
        {
            return items.Count(item => item.IsSelected);
        }

        /// <summary>
        /// Toggles the selection state of all items
        /// </summary>
        /// <param name="items">The items to toggle</param>
        public static void ToggleAll(this System.Collections.Generic.IEnumerable<SelectableItem> items)
        {
            var itemsList = items.ToList();
            var selectedCount = itemsList.GetSelectedCount();
            var totalCount = itemsList.Count(i => i.IsEnabled);

            // If none or some are selected, select all; if all are selected, deselect all
            if (selectedCount < totalCount)
            {
                itemsList.SelectAll();
            }
            else
            {
                itemsList.DeselectAll();
            }
        }
    }

    /// <summary>
    /// MTM-specific selectable item that includes common MTM properties
    /// Used for inventory items, transactions, and other MTM business objects
    /// </summary>
    public partial class MTMSelectableItem : SelectableItem
    {
        #region MTM-Specific Properties

        /// <summary>
        /// Gets or sets the Part ID for MTM inventory items
        /// </summary>
        [ObservableProperty]
        private string _partId = string.Empty;

        /// <summary>
        /// Gets or sets the Operation number for MTM items
        /// </summary>
        [ObservableProperty]
        private string _operation = string.Empty;

        /// <summary>
        /// Gets or sets the Location for MTM items
        /// </summary>
        [ObservableProperty]
        private string _location = string.Empty;

        /// <summary>
        /// Gets or sets the Quantity for MTM items
        /// </summary>
        [ObservableProperty]
        private int _quantity;

        /// <summary>
        /// Gets or sets the Last Updated timestamp
        /// </summary>
        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.Now;

        /// <summary>
        /// Gets or sets whether this item has notes
        /// </summary>
        [ObservableProperty]
        private bool _hasNotes;

        /// <summary>
        /// Gets or sets the notes content (loaded on demand)
        /// </summary>
        [ObservableProperty]
        private string _notes = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of MTMSelectableItem
        /// </summary>
        public MTMSelectableItem()
        {
        }

        /// <summary>
        /// Initializes a new instance with basic MTM data
        /// </summary>
        /// <param name="partId">The Part ID</param>
        /// <param name="operation">The Operation number</param>
        /// <param name="location">The Location</param>
        /// <param name="quantity">The Quantity</param>
        public MTMSelectableItem(string partId, string operation, string location, int quantity)
        {
            PartId = partId;
            Operation = operation;
            Location = location;
            Quantity = quantity;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a display key for this MTM item (Part ID + Operation)
        /// </summary>
        /// <returns>String key identifying this item</returns>
        public string GetDisplayKey()
        {
            return $"{PartId}-{Operation}";
        }

        /// <summary>
        /// Gets a summary description of this MTM item
        /// </summary>
        /// <returns>Summary string</returns>
        public string GetSummary()
        {
            return $"Part: {PartId}, Op: {Operation}, Loc: {Location}, Qty: {Quantity:N0}";
        }

        /// <summary>
        /// Validates that this MTM item has required data
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValidMTMItem()
        {
            return !string.IsNullOrWhiteSpace(PartId) && 
                   !string.IsNullOrWhiteSpace(Operation) && 
                   !string.IsNullOrWhiteSpace(Location) && 
                   Quantity >= 0;
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Returns a string representation of this MTM item
        /// </summary>
        public override string ToString()
        {
            return $"MTM Item - {GetSummary()}, Selected: {IsSelected}";
        }

        /// <summary>
        /// Determines equality based on Part ID and Operation
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is MTMSelectableItem other)
            {
                return string.Equals(PartId, other.PartId, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(Operation, other.Operation, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// Gets hash code based on Part ID and Operation
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                PartId.GetHashCode(StringComparison.OrdinalIgnoreCase), 
                Operation.GetHashCode(StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}