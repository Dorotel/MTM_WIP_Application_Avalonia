# ViewModels Directory

This directory contains all ViewModel classes that implement the MVVM pattern using ReactiveUI for the MTM WIP Application Avalonia.

## ??? ViewModel Architecture

### MVVM with ReactiveUI
All ViewModels follow ReactiveUI patterns for reactive programming:
- **Reactive Properties**: Observable properties that notify on changes
- **Reactive Commands**: Commands that support async operations and CanExecute logic
- **Property Dependencies**: Automatic property updates based on other property changes
- **Error Handling**: Centralized exception handling for all commands

## ?? ViewModel Files

### Core ViewModels

#### `MainWindowViewModel.cs`
Primary application window ViewModel managing overall application state and navigation.

```csharp
public class MainWindowViewModel : ReactiveObject
{
    // Navigation management
    // Global application state
    // Menu and toolbar commands
    // Status bar information
}
```

**Key Features:**
- **Navigation Management**: Controls which view is currently displayed
- **Global Commands**: Application-wide commands (Exit, About, Settings)
- **Status Management**: Application status and progress reporting
- **Theme Management**: User theme selection and application

#### `MainViewViewModel.cs`
Main content area ViewModel coordinating tab-based navigation and content display.

```csharp
public class MainViewViewModel : ReactiveObject
{
    // Tab management
    // Content coordination
    // Inter-tab communication
    // Quick actions integration
}
```

**Key Features:**
- **Tab Coordination**: Manages inventory, transfer, and remove tabs
- **Quick Actions**: Integrates with quick buttons panel
- **Data Sharing**: Coordinates data sharing between tabs
- **Progress Tracking**: Manages progress feedback across operations

### Tab ViewModels

#### `InventoryTabViewModel.cs`
Primary inventory management interface ViewModel.

```csharp
public class InventoryTabViewModel : ReactiveObject
{
    // Inventory item creation
    // Form validation
    // Save operations
    // Reset functionality
}
```

**Key Responsibilities:**
- **Form Management**: Part, Operation, Location, Quantity, Notes inputs
- **Validation Logic**: Real-time form validation with visual feedback
- **Save Operations**: Inventory item creation with progress tracking
- **Quick Button Integration**: Updates quick buttons on successful saves

**Key Properties:**
```csharp
// Form inputs
public string SelectedPart { get; set; }
public string SelectedOperation { get; set; }
public string SelectedLocation { get; set; }
public int Quantity { get; set; }
public string Notes { get; set; }

// Validation states
public bool IsPartValid { get; }
public bool IsOperationValid { get; }
public bool IsLocationValid { get; }
public bool IsQuantityValid { get; }
public bool IsFormValid { get; }

// Commands
public ReactiveCommand<Unit, Unit> SaveCommand { get; }
public ReactiveCommand<Unit, Unit> ResetCommand { get; }
public ReactiveCommand<Unit, Unit> AdvancedEntryCommand { get; }
```

### Component ViewModels

#### `QuickButtonsViewModel.cs`
Quick action buttons panel ViewModel for rapid inventory operations.

```csharp
public class QuickButtonsViewModel : ReactiveObject
{
    // Quick button collection
    // Button management
    // Action execution
    // User customization
}
```

**Key Features:**
- **Dynamic Button Management**: Loads user's last 10 transactions as buttons
- **Button Customization**: Edit, remove, and clear button operations
- **Action Execution**: One-click inventory operations
- **Context Menus**: Right-click management options

**Key Properties:**
```csharp
// Button collection
public ObservableCollection<QuickButtonItemViewModel> QuickButtons { get; }

// Management commands
public ReactiveCommand<Unit, Unit> LoadButtonsCommand { get; }
public ReactiveCommand<QuickButtonItemViewModel, Unit> EditButtonCommand { get; }
public ReactiveCommand<QuickButtonItemViewModel, Unit> RemoveButtonCommand { get; }
public ReactiveCommand<Unit, Unit> ClearAllButtonsCommand { get; }
public ReactiveCommand<Unit, Unit> RefreshButtonsCommand { get; }

// Action execution
public ReactiveCommand<QuickButtonItemViewModel, Unit> ExecuteQuickActionCommand { get; }
```

## ?? ReactiveUI Patterns

### Observable Properties
Standard pattern for reactive properties:

```csharp
private string _partId = string.Empty;
public string PartId
{
    get => _partId;
    set => this.RaiseAndSetIfChanged(ref _partId, value);
}
```

### Derived Properties (OAPH)
Properties that derive from other properties:

```csharp
private readonly ObservableAsPropertyHelper<bool> _isFormValid;
public bool IsFormValid => _isFormValid.Value;

public InventoryTabViewModel()
{
    _isFormValid = this.WhenAnyValue(
            vm => vm.SelectedPart,
            vm => vm.SelectedOperation,
            vm => vm.SelectedLocation,
            vm => vm.Quantity,
            (part, op, loc, qty) => 
                !string.IsNullOrWhiteSpace(part) &&
                !string.IsNullOrWhiteSpace(op) &&
                !string.IsNullOrWhiteSpace(loc) &&
                qty > 0)
        .ToProperty(this, vm => vm.IsFormValid, initialValue: false);
}
```

### Reactive Commands
Commands with async support and CanExecute logic:

```csharp
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public InventoryTabViewModel()
{
    // Command with CanExecute
    var canSave = this.WhenAnyValue(vm => vm.IsFormValid);
    SaveCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        await SaveInventoryItemAsync();
    }, canSave);

    // Error handling
    SaveCommand.ThrownExceptions
        .Subscribe(ex =>
        {
            // Handle errors gracefully
            ErrorMessage = $"Save failed: {ex.Message}";
        });
}
```

### Property Change Notifications
Responding to property changes:

```csharp
public InventoryTabViewModel()
{
    // React to part selection changes
    this.WhenAnyValue(vm => vm.SelectedPart)
        .Where(part => !string.IsNullOrWhiteSpace(part))
        .Subscribe(async part =>
        {
            await LoadPartDetailsAsync(part);
        });
}
```

## ?? Service Integration

### Dependency Injection Pattern
ViewModels receive services through constructor injection:

```csharp
public class InventoryTabViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IUserAndTransactionServices _userService;
    private readonly IApplicationStateService _applicationState;

    public InventoryTabViewModel(
        IInventoryService inventoryService,
        IUserAndTransactionServices userService,
        IApplicationStateService applicationState)
    {
        _inventoryService = inventoryService;
        _userService = userService;
        _applicationState = applicationState;
        
        InitializeCommands();
        InitializeProperties();
    }
}
```

### Service Usage Patterns
Async service calls with error handling:

```csharp
private async Task SaveInventoryItemAsync()
{
    try
    {
        var result = await _inventoryService.AddInventoryItemAsync(new InventoryItem
        {
            PartId = SelectedPart,
            Operation = SelectedOperation,
            Location = SelectedLocation,
            Quantity = Quantity,
            Notes = Notes,
            TransactionType = TransactionType.IN, // User is adding stock
            User = _applicationState.CurrentUser
        });

        if (result.IsSuccess)
        {
            StatusMessage = "Inventory item saved successfully";
            await ResetFormAsync();
        }
        else
        {
            ErrorMessage = result.ErrorMessage;
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = $"Failed to save inventory item: {ex.Message}";
    }
}
```

## ?? Business Logic Integration

### Transaction Type Logic
ViewModels implement correct transaction type determination:

```csharp
// ? CORRECT: TransactionType based on user intent
private TransactionType GetTransactionTypeForUserAction(UserAction action)
{
    return action switch
    {
        UserAction.AddStock => TransactionType.IN,      // User adding stock
        UserAction.RemoveStock => TransactionType.OUT,  // User removing stock
        UserAction.TransferStock => TransactionType.TRANSFER, // User moving stock
        _ => throw new ArgumentException($"Unknown user action: {action}")
    };
}

// Operation is just a workflow step number
private async Task ProcessInventoryActionAsync(UserAction action, string operation)
{
    var transactionType = GetTransactionTypeForUserAction(action);
    
    var transaction = new InventoryTransaction
    {
        TransactionType = transactionType, // Based on user intent
        Operation = operation, // Just a workflow step identifier
        // ... other properties
    };
}
```

### Data Validation
Comprehensive validation with immediate feedback:

```csharp
// Property validation
private bool ValidatePartId(string partId)
{
    if (string.IsNullOrWhiteSpace(partId))
    {
        PartIdError = "Part ID is required";
        return false;
    }
    
    if (!_inventoryService.IsValidPartId(partId))
    {
        PartIdError = "Invalid Part ID";
        return false;
    }
    
    PartIdError = null;
    return true;
}

// Quantity validation
private bool ValidateQuantity(int quantity)
{
    if (quantity <= 0)
    {
        QuantityError = "Quantity must be greater than zero";
        return false;
    }
    
    QuantityError = null;
    return true;
}
```

## ?? UI Binding Support

### Design-Time Data
ViewModels support design-time data for XAML previews:

```csharp
public class InventoryTabViewModel : ReactiveObject
{
    public InventoryTabViewModel() : this(null, null, null)
    {
        // Design-time constructor
    }

    public InventoryTabViewModel(
        IInventoryService inventoryService,
        IUserAndTransactionServices userService,
        IApplicationStateService applicationState)
    {
        // Runtime constructor
        if (inventoryService == null) // Design-time
        {
            LoadDesignTimeData();
        }
        else // Runtime
        {
            _inventoryService = inventoryService;
            // ... other service assignments
        }
    }

    private void LoadDesignTimeData()
    {
        SelectedPart = "SAMPLE-PART-001";
        SelectedOperation = "100";
        SelectedLocation = "Main Warehouse";
        Quantity = 25;
        Notes = "Sample inventory item for design-time preview";
    }
}
```

### Avalonia Binding Support
Properties designed for Avalonia compiled bindings:

```csharp
// Properties with proper change notification
public string SelectedPart
{
    get => _selectedPart;
    set
    {
        this.RaiseAndSetIfChanged(ref _selectedPart, value);
        ValidatePartId(value); // Immediate validation
    }
}

// Collections for ItemsControl binding
public ObservableCollection<string> AvailableParts { get; } = new();
public ObservableCollection<string> AvailableOperations { get; } = new();
public ObservableCollection<string> AvailableLocations { get; } = new();
```

## ?? Error Handling

### Centralized Error Management
All ViewModels implement consistent error handling:

```csharp
// Error properties
private string _errorMessage;
public string ErrorMessage
{
    get => _errorMessage;
    set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
}

private bool _hasError;
public bool HasError
{
    get => _hasError;
    set => this.RaiseAndSetIfChanged(ref _hasError, value);
}

// Command error handling
public InventoryTabViewModel()
{
    SaveCommand = ReactiveCommand.CreateFromTask(SaveInventoryItemAsync);
    
    // Global error handling for all commands
    Observable.Merge(
            SaveCommand.ThrownExceptions,
            ResetCommand.ThrownExceptions,
            LoadDataCommand.ThrownExceptions)
        .Subscribe(HandleError);
}

private void HandleError(Exception ex)
{
    ErrorMessage = TranslateErrorToUserMessage(ex);
    HasError = true;
    
    // Log error for debugging
    Debug.WriteLine($"ViewModel Error: {ex}");
}
```

### User-Friendly Error Messages
Error translation for better user experience:

```csharp
private string TranslateErrorToUserMessage(Exception ex)
{
    return ex switch
    {
        ValidationException validationEx => validationEx.Message,
        DatabaseException => "Database connection issue. Please check your connection and try again.",
        TimeoutException => "Operation timed out. Please try again.",
        UnauthorizedAccessException => "You don't have permission to perform this action.",
        _ => "An unexpected error occurred. Please contact support if the problem persists."
    };
}
```

## ?? Performance Optimization

### Efficient Property Updates
Minimizing unnecessary updates:

```csharp
// Batch property updates
public void UpdateFormData(InventoryFormData data)
{
    using (DelayChangeNotifications())
    {
        SelectedPart = data.PartId;
        SelectedOperation = data.Operation;
        SelectedLocation = data.Location;
        Quantity = data.Quantity;
        Notes = data.Notes;
    } // Change notifications sent here
}
```

### Lazy Loading
Load data only when needed:

```csharp
private readonly Lazy<Task<List<string>>> _availableParts;

public InventoryTabViewModel()
{
    _availableParts = new Lazy<Task<List<string>>>(LoadAvailablePartsAsync);
}

public async Task<List<string>> GetAvailablePartsAsync()
{
    return await _availableParts.Value;
}
```

## ?? Testing Support

### Testable Design
ViewModels designed for easy unit testing:

```csharp
public class InventoryTabViewModelTests
{
    [Test]
    public async Task SaveCommand_ValidData_SavesSuccessfully()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockUserService = new Mock<IUserAndTransactionServices>();
        var mockApplicationState = new Mock<IApplicationStateService>();
        
        var viewModel = new InventoryTabViewModel(
            mockInventoryService.Object,
            mockUserService.Object,
            mockApplicationState.Object);
        
        viewModel.SelectedPart = "TEST-PART";
        viewModel.SelectedOperation = "100";
        viewModel.SelectedLocation = "Test Location";
        viewModel.Quantity = 10;
        
        // Act
        await viewModel.SaveCommand.Execute();
        
        // Assert
        mockInventoryService.Verify(s => s.AddInventoryItemAsync(It.IsAny<InventoryItem>()), Times.Once);
        Assert.That(viewModel.ErrorMessage, Is.Null);
    }
}
```

## ?? Development Guidelines

### Adding New ViewModels
1. **Inherit from ReactiveObject**: All ViewModels must inherit from `ReactiveObject`
2. **Use Reactive Patterns**: Implement properties and commands using ReactiveUI patterns
3. **Inject Services**: Use constructor injection for all service dependencies
4. **Handle Errors**: Implement comprehensive error handling for all operations
5. **Support Design-Time**: Provide design-time data for XAML previews
6. **Add Unit Tests**: Create comprehensive unit tests for all public methods

### ViewModel Conventions
- **Naming**: ViewModels end with `ViewModel` (e.g., `InventoryTabViewModel`)
- **Properties**: Use `RaiseAndSetIfChanged` for all settable properties
- **Commands**: Use `ReactiveCommand` for all user actions
- **Async Operations**: Use `CreateFromTask` for async command operations
- **Error Handling**: Subscribe to `ThrownExceptions` for all commands

## ?? Related Documentation

- **View Bindings**: See `Views/` directory for AXAML view files
- **Service Contracts**: `Services/Interfaces/` for service interfaces
- **UI Documentation**: `Development/UI_Documentation/` for component specifications
- **ReactiveUI Guide**: Official ReactiveUI documentation for advanced patterns

---

*This directory implements the ViewModel layer of the MVVM pattern, providing reactive data binding and command handling for the MTM WIP Application UI components.*