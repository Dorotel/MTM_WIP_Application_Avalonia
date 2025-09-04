# MTM MVVM Community Toolkit Patterns

## 🏗️ MVVM Community Toolkit Implementation Patterns

### **Observable Property Patterns**

#### **Basic Observable Property**
```csharp
[ObservableObject]
public partial class ExampleViewModel : BaseViewModel
{
    [ObservableProperty]
    private string textValue = string.Empty;
    
    [ObservableProperty]
    private int numericValue;
    
    [ObservableProperty]
    private bool isEnabled = true;
    
    [ObservableProperty]
    private ObservableCollection<ItemModel> items = new();
}
```

#### **Observable Property with Validation**
```csharp
[ObservableProperty]
[NotifyDataErrorInfo]
[Required(ErrorMessage = "This field is required")]
[StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
private string validatedProperty = string.Empty;

[ObservableProperty]
[NotifyDataErrorInfo]
[Range(1, 1000, ErrorMessage = "Value must be between 1 and 1000")]
private int quantityValue = 1;
```

#### **Observable Property with Change Notification**
```csharp
[ObservableProperty]
private string inputValue = string.Empty;

// Automatically generated: partial void OnInputValueChanged(string value)
partial void OnInputValueChanged(string value)
{
    // Custom logic when property changes
    ErrorMessage = null; // Clear errors when user types
    
    // Notify commands that depend on this property
    SaveCommand.NotifyCanExecuteChanged();
    ValidateCommand.NotifyCanExecuteChanged();
}

// Also available: OnInputValueChanging(string value) - called before change
```

### **RelayCommand Patterns**

#### **Basic Async Command**
```csharp
[RelayCommand]
private async Task ExecuteOperationAsync()
{
    IsLoading = true;
    
    try
    {
        // Async operation
        await SomeService.PerformOperationAsync();
        
        // Update UI
        StatusMessage = "Operation completed successfully";
    }
    catch (Exception ex)
    {
        await Services.ErrorHandling.HandleErrorAsync(ex, "Execute operation");
        ErrorMessage = "Operation failed. Please try again.";
    }
    finally
    {
        IsLoading = false;
    }
}
```

#### **Command with CanExecute**
```csharp
[RelayCommand(CanExecute = nameof(CanExecuteSave))]
private async Task SaveAsync()
{
    // Save implementation
}

private bool CanExecuteSave => !IsLoading && 
                               !HasErrors && 
                               !string.IsNullOrWhiteSpace(RequiredField);

// Notify when CanExecute state changes
partial void OnIsLoadingChanged(bool value)
{
    SaveCommand.NotifyCanExecuteChanged();
}

partial void OnRequiredFieldChanged(string value)
{
    SaveCommand.NotifyCanExecuteChanged();
}
```

#### **Command with Parameter**
```csharp
[RelayCommand]
private async Task DeleteItemAsync(ItemModel item)
{
    if (item == null) return;
    
    var confirmed = await ShowConfirmationDialog($"Delete {item.Name}?");
    if (!confirmed) return;
    
    Items.Remove(item);
    await SaveChangesAsync();
}

[RelayCommand]
private void SelectItem(object parameter)
{
    if (parameter is ItemModel item)
    {
        SelectedItem = item;
        // Handle selection
    }
}
```

#### **Cancellable Async Command**
```csharp
[RelayCommand]
private async Task LongRunningOperationAsync(CancellationToken cancellationToken)
{
    IsLoading = true;
    
    try
    {
        for (int i = 0; i < 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            // Simulate work
            await Task.Delay(100, cancellationToken);
            
            // Update progress
            ProgressValue = i + 1;
        }
    }
    catch (OperationCanceledException)
    {
        StatusMessage = "Operation cancelled";
    }
    finally
    {
        IsLoading = false;
    }
}

[RelayCommand]
private void CancelOperation()
{
    LongRunningOperationCommand.Cancel();
}
```

### **Property Change Notification Patterns**

#### **Dependent Property Notifications**
```csharp
[ObservableProperty]
private string firstName = string.Empty;

[ObservableProperty]
private string lastName = string.Empty;

// Computed property that depends on other properties
public string FullName => $"{FirstName} {LastName}".Trim();

// Notify dependent properties when source properties change
partial void OnFirstNameChanged(string value)
{
    OnPropertyChanged(nameof(FullName));
}

partial void OnLastNameChanged(string value)
{
    OnPropertyChanged(nameof(FullName));
}
```

#### **Collection Change Notifications**
```csharp
[ObservableProperty]
private ObservableCollection<ItemModel> items = new();

public bool HasItems => Items.Count > 0;

public int ItemCount => Items.Count;

partial void OnItemsChanged(ObservableCollection<ItemModel> value)
{
    // Unsubscribe from old collection
    if (value != null)
    {
        value.CollectionChanged -= OnItemsCollectionChanged;
    }
    
    // Subscribe to new collection
    if (value != null)
    {
        value.CollectionChanged += OnItemsCollectionChanged;
    }
    
    OnPropertyChanged(nameof(HasItems));
    OnPropertyChanged(nameof(ItemCount));
}

private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
{
    OnPropertyChanged(nameof(HasItems));
    OnPropertyChanged(nameof(ItemCount));
    
    // Update commands that depend on collection
    DeleteSelectedCommand.NotifyCanExecuteChanged();
}
```

### **Complex ViewModel Patterns**

#### **Master-Detail ViewModel**
```csharp
[ObservableObject]
public partial class MasterDetailViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<MasterItem> masterItems = new();
    
    [ObservableProperty]
    private MasterItem? selectedMasterItem;
    
    [ObservableProperty]
    private ObservableCollection<DetailItem> detailItems = new();
    
    [ObservableProperty]
    private bool isLoadingDetails;

    // When master selection changes, load details
    partial void OnSelectedMasterItemChanged(MasterItem? value)
    {
        if (value != null)
        {
            LoadDetailsAsync(value.Id);
        }
        else
        {
            DetailItems.Clear();
        }
    }

    [RelayCommand]
    private async Task LoadDetailsAsync(int masterId)
    {
        IsLoadingDetails = true;
        
        try
        {
            var details = await _service.GetDetailsAsync(masterId);
            DetailItems.Clear();
            
            foreach (var detail in details)
            {
                DetailItems.Add(detail);
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Load details");
        }
        finally
        {
            IsLoadingDetails = false;
        }
    }
}
```

#### **Form ViewModel with Validation**
```csharp
[ObservableObject]
public partial class FormViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [StringLength(50)]
    private string partId = string.Empty;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    private string location = string.Empty;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(1, int.MaxValue)]
    private int quantity = 1;
    
    [ObservableProperty]
    private ObservableCollection<string> availableLocations = new();
    
    [ObservableProperty]
    private bool isDirty;

    // Form is valid when there are no validation errors
    public bool IsValid => !HasErrors;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        if (!IsValid) return;
        
        var item = new InventoryItem
        {
            PartId = PartId,
            Location = Location,
            Quantity = Quantity
        };
        
        var result = await _inventoryService.SaveAsync(item);
        
        if (result.IsSuccess)
        {
            IsDirty = false;
            StatusMessage = "Saved successfully";
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
    }
    
    private bool CanSave => IsValid && IsDirty && !IsLoading;
    
    // Mark form as dirty when any field changes
    partial void OnPartIdChanged(string value) => IsDirty = true;
    partial void OnLocationChanged(string value) => IsDirty = true;
    partial void OnQuantityChanged(int value) => IsDirty = true;
    
    // Notify save command when validation or dirty state changes
    protected override void OnErrorsChanged(DataErrorsChangedEventArgs e)
    {
        base.OnErrorsChanged(e);
        SaveCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(IsValid));
    }
}
```

### **Advanced Patterns**

#### **Messenger Pattern Integration**
```csharp
[ObservableObject]
public partial class MessengerViewModel : BaseViewModel, IRecipient<ItemUpdatedMessage>
{
    public MessengerViewModel(IMessenger messenger) : base()
    {
        messenger.Register<ItemUpdatedMessage>(this);
    }
    
    public void Receive(ItemUpdatedMessage message)
    {
        // Update UI when message received
        var existingItem = Items.FirstOrDefault(x => x.Id == message.Item.Id);
        if (existingItem != null)
        {
            var index = Items.IndexOf(existingItem);
            Items[index] = message.Item;
        }
    }
    
    [RelayCommand]
    private async Task UpdateItemAsync(ItemModel item)
    {
        // Update item
        var result = await _service.UpdateAsync(item);
        
        if (result.IsSuccess)
        {
            // Notify other ViewModels
            Messenger.Send(new ItemUpdatedMessage(result.Data));
        }
    }
}
```

#### **Reactive Property Dependencies**
```csharp
[ObservableObject]
public partial class CalculatorViewModel : BaseViewModel
{
    [ObservableProperty]
    private decimal value1;
    
    [ObservableProperty]
    private decimal value2;
    
    [ObservableProperty]
    private string operation = "+";
    
    // Computed property that updates automatically
    public decimal Result
    {
        get
        {
            return Operation switch
            {
                "+" => Value1 + Value2,
                "-" => Value1 - Value2,
                "*" => Value1 * Value2,
                "/" => Value2 != 0 ? Value1 / Value2 : 0,
                _ => 0
            };
        }
    }
    
    // Notify Result when dependencies change
    partial void OnValue1Changed(decimal value) => OnPropertyChanged(nameof(Result));
    partial void OnValue2Changed(decimal value) => OnPropertyChanged(nameof(Result));
    partial void OnOperationChanged(string value) => OnPropertyChanged(nameof(Result));
}
```

### **Migration from ReactiveUI Patterns**

#### **Before (ReactiveUI)**
```csharp
// OLD - ReactiveUI pattern
public class OldViewModel : ReactiveObject
{
    private string _property;
    public string Property
    {
        get => _property;
        set => this.RaiseAndSetIfChanged(ref _property, value);
    }
    
    public ReactiveCommand<Unit, Unit> Command { get; }
    
    public OldViewModel()
    {
        Command = ReactiveCommand.CreateFromTask(ExecuteAsync);
    }
}
```

#### **After (MVVM Community Toolkit)**
```csharp
// NEW - MVVM Community Toolkit pattern
[ObservableObject]
public partial class NewViewModel : BaseViewModel
{
    [ObservableProperty]
    private string property = string.Empty;
    
    [RelayCommand]
    private async Task ExecuteAsync()
    {
        // Implementation
    }
}
```

### **Performance Optimizations**

#### **Bulk Property Updates**
```csharp
// For multiple property updates, use OnPropertyChanging/Changed
private void UpdateMultipleProperties(DataModel data)
{
    // Suppress notifications during bulk update
    OnPropertyChanging(string.Empty); // Empty string means "all properties"
    
    _partId = data.PartId;
    _location = data.Location;  
    _quantity = data.Quantity;
    
    OnPropertyChanged(string.Empty); // Notify all properties changed
}
```

#### **Lazy Loading Properties**
```csharp
private readonly Lazy<ExpensiveData> _expensiveData;

public ExpensiveData ExpensiveData => _expensiveData.Value;

public ViewModel()
{
    _expensiveData = new Lazy<ExpensiveData>(() => LoadExpensiveData());
}
```