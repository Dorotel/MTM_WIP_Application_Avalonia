# CustomDataGrid - Data Binding Requirements

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 🔗 Data Binding Requirements

The CustomDataGrid follows MVVM Community Toolkit patterns with parent ViewModel data binding and command routing through RelativeSource bindings.

## Control Properties

### Primary Data Source
```csharp
// CustomDataGrid.axaml.cs - Dependency Properties
public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
    AvaloniaProperty.Register<CustomDataGrid, IEnumerable>(nameof(ItemsSource));

public static readonly StyledProperty<bool> IsMultiSelectEnabledProperty =
    AvaloniaProperty.Register<CustomDataGrid, bool>(nameof(IsMultiSelectEnabled), true);

public IEnumerable ItemsSource
{
    get => GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
}

public bool IsMultiSelectEnabled
{
    get => GetValue(IsMultiSelectEnabledProperty);
    set => SetValue(IsMultiSelectEnabledProperty, value);
}
```

### XAML Data Binding
```xml
<!-- Parent View Usage -->
<customControls:CustomDataGrid 
    ItemsSource="{Binding InventoryItems}"
    IsMultiSelectEnabled="True"
    ReadNoteCommand="{Binding ReadNoteCommand}"
    DeleteItemCommand="{Binding DeleteItemCommand}"
    EditItemCommand="{Binding EditItemCommand}"
    DuplicateItemCommand="{Binding DuplicateItemCommand}"
    ViewDetailsCommand="{Binding ViewDetailsCommand}" />
```

## Command Bindings

### Required Commands (Parent ViewModel)
```csharp
// Parent ViewModel must implement these commands
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task ReadNoteAsync(object parameter)
    {
        if (parameter is InventoryItem item)
        {
            // Handle reading note for specific item
        }
    }

    [RelayCommand]
    private async Task DeleteItemAsync(object parameter)
    {
        if (parameter is InventoryItem item)
        {
            // Show confirmation dialog and delete item
        }
    }

    [RelayCommand]
    private async Task EditItemAsync(object parameter)
    {
        if (parameter is InventoryItem item)
        {
            // Handle item editing
        }
    }

    [RelayCommand]
    private async Task DuplicateItemAsync(object parameter)
    {
        if (parameter is InventoryItem item)
        {
            // Handle item duplication
        }
    }

    [RelayCommand]
    private async Task ViewDetailsAsync(object parameter)
    {
        if (parameter is InventoryItem item)
        {
            // Show detailed item information
        }
    }
}
```

### Command Property Definitions
```csharp
// CustomDataGrid.axaml.cs - Command Properties
public static readonly StyledProperty<ICommand> ReadNoteCommandProperty =
    AvaloniaProperty.Register<CustomDataGrid, ICommand>(nameof(ReadNoteCommand));

public static readonly StyledProperty<ICommand> DeleteItemCommandProperty =
    AvaloniaProperty.Register<CustomDataGrid, ICommand>(nameof(DeleteItemCommand));

public static readonly StyledProperty<ICommand> EditItemCommandProperty =
    AvaloniaProperty.Register<CustomDataGrid, ICommand>(nameof(EditItemCommand));

public static readonly StyledProperty<ICommand> DuplicateItemCommandProperty =
    AvaloniaProperty.Register<CustomDataGrid, ICommand>(nameof(DuplicateItemCommand));

public static readonly StyledProperty<ICommand> ViewDetailsCommandProperty =
    AvaloniaProperty.Register<CustomDataGrid, ICommand>(nameof(ViewDetailsCommand));

// Command Properties
public ICommand ReadNoteCommand
{
    get => GetValue(ReadNoteCommandProperty);
    set => SetValue(ReadNoteCommandProperty, value);
}

public ICommand DeleteItemCommand
{
    get => GetValue(DeleteItemCommandProperty);
    set => SetValue(DeleteItemCommandProperty, value);
}

// ... other command properties
```

## Data Item Requirements

### Expected Data Model
```csharp
// Data items should implement this interface or have these properties
public interface IInventoryItem
{
    string PartId { get; set; }
    string Operation { get; set; }
    string Location { get; set; }
    int Quantity { get; set; }
    DateTime LastUpdated { get; set; }
    bool HasNotes { get; set; }
    bool IsSelected { get; set; }
}

// Example implementation
public class InventoryItem : IInventoryItem, INotifyPropertyChanged
{
    private bool _isSelected;
    
    public string PartId { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool HasNotes { get; set; }
    
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## ItemTemplate Data Binding

### Complete ItemTemplate Structure
```xml
<DataTemplate x:DataType="models:InventoryItem">
  <Grid>
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="40" />      <!-- Selection -->
      <ColumnDefinition Width="1.5*" />    <!-- Part ID -->
      <ColumnDefinition Width="1*" />      <!-- Operation -->
      <ColumnDefinition Width="1.2*" />    <!-- Location -->
      <ColumnDefinition Width="1*" />      <!-- Quantity -->
      <ColumnDefinition Width="1.8*" />    <!-- Last Updated -->
      <ColumnDefinition Width="80" />      <!-- Notes -->
      <ColumnDefinition Width="100" />     <!-- Actions -->
      <ColumnDefinition Width="40" />      <!-- Management -->
    </Grid.ColumnDefinitions>

    <!-- Selection Checkbox -->
    <Border Grid.Column="0" Classes="checkbox-cell">
      <CheckBox IsChecked="{Binding IsSelected}"
                HorizontalAlignment="Center"
                VerticalAlignment="Center" />
    </Border>

    <!-- Part ID -->
    <Border Grid.Column="1" Classes="data-cell">
      <TextBlock Text="{Binding PartId}"
                 ToolTip.Tip="{Binding PartId}"
                 TextTrimming="CharacterEllipsis" />
    </Border>

    <!-- Operation -->
    <Border Grid.Column="2" Classes="data-cell">
      <TextBlock Text="{Binding Operation}"
                 ToolTip.Tip="{Binding Operation}" />
    </Border>

    <!-- Location -->
    <Border Grid.Column="3" Classes="data-cell">
      <TextBlock Text="{Binding Location}"
                 ToolTip.Tip="{Binding Location}" />
    </Border>

    <!-- Quantity -->
    <Border Grid.Column="4" Classes="data-cell">
      <TextBlock Text="{Binding Quantity, StringFormat=N0}"
                 HorizontalAlignment="Right"
                 ToolTip.Tip="{Binding Quantity, StringFormat='Quantity: {0:N0}'}" />
    </Border>

    <!-- Last Updated -->
    <Border Grid.Column="5" Classes="data-cell">
      <TextBlock Text="{Binding LastUpdated, StringFormat='MM/dd/yy HH:mm'}"
                 ToolTip.Tip="{Binding LastUpdated, StringFormat='Last Updated: MM/dd/yyyy HH:mm:ss'}" />
    </Border>

    <!-- Notes Indicator -->
    <Border Grid.Column="6" Classes="data-cell">
      <TextBlock Text="✓"
                 Foreground="{DynamicResource MTM_Shared_Logic.SuccessBrush}"
                 FontSize="14"
                 FontWeight="Bold"
                 HorizontalAlignment="Center"
                 VerticalAlignment="Center"
                 IsVisible="{Binding HasNotes}"
                 ToolTip.Tip="This item has notes" />
    </Border>

    <!-- Action Buttons -->
    <Border Grid.Column="7" Classes="action-cell">
      <StackPanel Orientation="Horizontal"
                  Spacing="2"
                  HorizontalAlignment="Center"
                  VerticalAlignment="Center">
        
        <!-- Read Note Button -->
        <Button Content="📝"
                Classes="primary-action"
                Width="24"
                Height="24"
                Command="{Binding ReadNoteCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                CommandParameter="{Binding}"
                ToolTip.Tip="Read Note"
                IsVisible="{Binding HasNotes}" />
        
        <!-- Edit Button -->
        <Button Content="✏️"
                Classes="primary-action"
                Width="24"
                Height="24"
                Command="{Binding EditItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                CommandParameter="{Binding}"
                ToolTip.Tip="Edit Item" />
        
        <!-- Delete Button -->
        <Button Content="🗑️"
                Classes="warning-action"
                Width="24"
                Height="24"
                Command="{Binding DeleteItemCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                CommandParameter="{Binding}"
                ToolTip.Tip="Delete Item" />
      </StackPanel>
    </Border>

    <!-- Management (Future) -->
    <Border Grid.Column="8" Classes="data-cell">
      <!-- Reserved for future features -->
    </Border>

  </Grid>
</DataTemplate>
```

## Selection Management

### Select All Implementation
```xml
<!-- Header Select All Checkbox -->
<Border Grid.Column="0" Classes="checkbox-header-cell">
  <CheckBox x:Name="SelectAllCheckBox"
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Click="OnSelectAllClick"
            ToolTip.Tip="Select/Deselect All Items" />
</Border>
```

### Code-Behind Selection Logic
```csharp
// CustomDataGrid.axaml.cs - Selection Management
private void OnSelectAllClick(object sender, RoutedEventArgs e)
{
    if (sender is CheckBox selectAllCheckBox && ItemsSource != null)
    {
        bool selectAll = selectAllCheckBox.IsChecked == true;
        
        foreach (var item in ItemsSource.OfType<IInventoryItem>())
        {
            item.IsSelected = selectAll;
        }
        
        // Notify parent of selection change
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs
        {
            SelectedItems = GetSelectedItems(),
            SelectionMode = selectAll ? "SelectAll" : "DeselectAll"
        });
    }
}

private List<object> GetSelectedItems()
{
    return ItemsSource?.OfType<IInventoryItem>()
        .Where(item => item.IsSelected)
        .Cast<object>()
        .ToList() ?? new List<object>();
}
```

## Event Communication

### Selection Changed Event
```csharp
// CustomDataGrid.axaml.cs - Events
public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

public class SelectionChangedEventArgs : EventArgs
{
    public List<object> SelectedItems { get; set; } = new();
    public string SelectionMode { get; set; } = string.Empty;
    public int SelectedCount => SelectedItems.Count;
}
```

### Parent ViewModel Event Handling
```csharp
// Parent ViewModel event subscription
private void OnCustomDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
{
    SelectedItemCount = e.SelectedCount;
    SelectedItems.Clear();
    
    foreach (var item in e.SelectedItems.OfType<InventoryItem>())
    {
        SelectedItems.Add(item);
    }
    
    // Update UI state based on selection
    HasSelectedItems = e.SelectedCount > 0;
    CanDeleteSelected = e.SelectedCount > 0;
    CanExportSelected = e.SelectedCount > 0;
}
```

## Data Validation

### Required Property Validation
```csharp
// Ensure data items have required properties
private static bool ValidateDataItem(object item)
{
    if (item is IInventoryItem inventoryItem)
    {
        return !string.IsNullOrEmpty(inventoryItem.PartId) &&
               !string.IsNullOrEmpty(inventoryItem.Operation) &&
               !string.IsNullOrEmpty(inventoryItem.Location);
    }
    
    return false;
}
```

### Collection Change Handling
```csharp
// Handle changes to ObservableCollection
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    
    if (change.Property == ItemsSourceProperty)
    {
        HandleItemsSourceChanged(change.OldValue, change.NewValue);
    }
}

private void HandleItemsSourceChanged(object? oldValue, object? newValue)
{
    // Unsubscribe from old collection
    if (oldValue is INotifyCollectionChanged oldCollection)
    {
        oldCollection.CollectionChanged -= OnItemsCollectionChanged;
    }
    
    // Subscribe to new collection
    if (newValue is INotifyCollectionChanged newCollection)
    {
        newCollection.CollectionChanged += OnItemsCollectionChanged;
    }
    
    // Update UI
    UpdateSelectAllState();
}
```

---

**Next Implementation Phase**: [06-Command-Implementation.md](./06-Command-Implementation.md)