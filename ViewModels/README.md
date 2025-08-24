# ViewModels Directory

This directory contains all ViewModel classes that implement the MVVM pattern using ReactiveUI for the MTM WIP Application Avalonia (.NET 8).

## ??? ViewModel Architecture

### MVVM with ReactiveUI (.NET 8)
All ViewModels follow ReactiveUI patterns optimized for .NET 8:
- **Reactive Properties**: Observable properties with modern C# nullable reference types
- **Reactive Commands**: Async commands with CancellationToken support and proper error handling
- **Property Dependencies**: Automatic updates using ObservableAsPropertyHelper (OAPH)
- **Error Handling**: Centralized exception handling with structured logging
- **Dependency Injection**: Constructor injection with .NET 8 DI container

## ?? ViewModel Files

### Core ViewModels

#### `MainWindowViewModel.cs`
Primary application window ViewModel managing overall application state and navigation.

```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject
{
    private readonly INavigationService _navigationService;
    private readonly IApplicationStateService _applicationState;
    private readonly ILogger<MainWindowViewModel> _logger;

    public MainWindowViewModel(
        INavigationService navigationService,
        IApplicationStateService applicationState,
        ILogger<MainWindowViewModel> logger)
    {
        _navigationService = navigationService;
        _applicationState = applicationState;
        _logger = logger;
        
        InitializeCommands();
        InitializeProperties();
    }

    // Modern C# properties with nullable reference types
    private string? _currentUser;
    public string? CurrentUser
    {
        get => _currentUser;
        set => this.RaiseAndSetIfChanged(ref _currentUser, value);
    }

    private ViewModelBase? _currentContent;
    public ViewModelBase? CurrentContent
    {
        get => _currentContent;
        set => this.RaiseAndSetIfChanged(ref _currentContent, value);
    }

    public ReactiveCommand<string, Unit> NavigateCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ExitCommand { get; private set; } = null!;
}
```

**Key Features:**
- **Navigation Management**: Type-safe navigation with dependency injection
- **Global State**: Application-wide state management with ReactiveUI
- **Error Handling**: Structured logging and user-friendly error presentation
- **Theme Support**: Dynamic theme switching with .NET 8 configuration patterns

#### `MainViewViewModel.cs`
Main content area ViewModel coordinating tab-based navigation.

```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels;

public sealed class MainViewViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<bool> _hasActiveTab;
    public bool HasActiveTab => _hasActiveTab.Value;

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    // Modern collection initialization
    public ObservableCollection<TabItemViewModel> Tabs { get; } = [];

    public ReactiveCommand<TabItemViewModel, Unit> SwitchTabCommand { get; }
    public ReactiveCommand<TabItemViewModel, Unit> CloseTabCommand { get; }

    public MainViewViewModel(ILogger<MainViewViewModel> logger)
    {
        // OAPH with .NET 8 patterns
        _hasActiveTab = this.WhenAnyValue(vm => vm.SelectedTabIndex)
            .Select(index => index >= 0 && index < Tabs.Count)
            .ToProperty(this, vm => vm.HasActiveTab);

        SwitchTabCommand = ReactiveCommand.Create<TabItemViewModel>(SwitchTab);
        CloseTabCommand = ReactiveCommand.Create<TabItemViewModel>(CloseTab);
    }
}
```

### Business ViewModels

#### `InventoryTabViewModel.cs`
Primary inventory management interface with enhanced .NET 8 patterns.

```csharp
namespace MTM_WIP_Application_Avalonia.ViewModels;

public sealed class InventoryTabViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IUserAndTransactionServices _userService;
    private readonly IApplicationStateService _applicationState;
    private readonly ILogger<InventoryTabViewModel> _logger;

    public InventoryTabViewModel(
        IInventoryService inventoryService,
        IUserAndTransactionServices userService,
        IApplicationStateService applicationState,
        ILogger<InventoryTabViewModel> logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        InitializeProperties();
        InitializeCommands();
        InitializeValidation();
    }

    // Properties with nullable reference types
    private string _selectedPart = string.Empty;
    public string SelectedPart
    {
        get => _selectedPart;
        set => this.RaiseAndSetIfChanged(ref _selectedPart, value);
    }

    private string _selectedOperation = string.Empty;
    public string SelectedOperation
    {
        get => _selectedOperation;
        set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
    }

    private string _selectedLocation = string.Empty;
    public string SelectedLocation
    {
        get => _selectedLocation;
        set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _notes = string.Empty;
    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    // Validation with OAPH
    private readonly ObservableAsPropertyHelper<bool> _isFormValid;
    public bool IsFormValid => _isFormValid.Value;

    private readonly ObservableAsPropertyHelper<string?> _validationMessage;
    public string? ValidationMessage => _validationMessage.Value;

    // Commands with CancellationToken support
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> ResetCommand { get; private set; } = null!;
    public ReactiveCommand<Unit, Unit> LoadPartsCommand { get; private set; } = null!;

    private void InitializeProperties()
    {
        // Advanced validation with multiple criteria
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

        _validationMessage = this.WhenAnyValue(
                vm => vm.SelectedPart,
                vm => vm.SelectedOperation,
                vm => vm.SelectedLocation,
                vm => vm.Quantity,
                GetValidationMessage)
            .ToProperty(this, vm => vm.ValidationMessage, initialValue: null);
    }

    private void InitializeCommands()
    {
        // Commands with proper CanExecute
        SaveCommand = ReactiveCommand.CreateFromTask(
            SaveInventoryItemAsync,
            this.WhenAnyValue(vm => vm.IsFormValid));

        ResetCommand = ReactiveCommand.Create(ResetForm);

        LoadPartsCommand = ReactiveCommand.CreateFromTask(LoadAvailablePartsAsync);

        // Centralized error handling
        Observable.Merge(
                SaveCommand.ThrownExceptions,
                LoadPartsCommand.ThrownExceptions)
            .Subscribe(HandleError);
    }

    private async Task SaveInventoryItemAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Saving inventory item: Part={PartId}, Operation={Operation}, Quantity={Quantity}", 
                SelectedPart, SelectedOperation, Quantity);

            // Use stored procedure for database operations
            var parameters = new Dictionary<string, object>
            {
                ["p_PartID"] = SelectedPart,
                ["p_OperationID"] = SelectedOperation,
                ["p_LocationID"] = SelectedLocation,
                ["p_Quantity"] = Quantity,
                ["p_Notes"] = Notes,
                ["p_UserID"] = _applicationState.CurrentUser ?? "Unknown",
                ["p_TransactionType"] = "IN" // Always IN when adding inventory
            };

            var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
                Model_AppVariables.ConnectionString,
                "inv_inventory_Add_Item_Enhanced",
                parameters
            );

            if (result.Status == 0)
            {
                _logger.LogInformation("Inventory item saved successfully");
                ResetForm();
                
                // Raise event for parent components
                ItemSaved?.Invoke(this, new InventoryItemSavedEventArgs
                {
                    PartId = SelectedPart,
                    Operation = SelectedOperation,
                    Quantity = Quantity
                });
            }
            else
            {
                throw new InvalidOperationException(result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save inventory item");
            throw; // Re-throw for error handling subscription
        }
    }

    private void HandleError(Exception ex)
    {
        _logger.LogError(ex, "ViewModel error occurred");
        
        // Show user-friendly error message
        ErrorMessage = ex switch
        {
            InvalidOperationException => ex.Message,
            TimeoutException => "Operation timed out. Please try again.",
            _ => "An unexpected error occurred. Please contact support."
        };
    }

    // Events for component communication
    public event EventHandler<InventoryItemSavedEventArgs>? ItemSaved;
}

public record InventoryItemSavedEventArgs
{
    public required string PartId { get; init; }
    public required string Operation { get; init; }
    public required int Quantity { get; init; }
}
```

## ?? ReactiveUI Patterns (.NET 8)

### Observable Properties with Nullable Reference Types
```csharp
// Nullable string property
private string? _description;
public string? Description
{
    get => _description;
    set => this.RaiseAndSetIfChanged(ref _description, value);
}

// Non-nullable string with default value
private string _partId = string.Empty;
public string PartId
{
    get => _partId;
    set => this.RaiseAndSetIfChanged(ref _partId, value);
}
```

### Derived Properties with Modern C#
```csharp
private readonly ObservableAsPropertyHelper<string> _displayName;
public string DisplayName => _displayName.Value;

public InventoryViewModel()
{
    _displayName = this.WhenAnyValue(
            vm => vm.PartId,
            vm => vm.Description,
            (partId, desc) => $"{partId}: {desc ?? "No Description"}")
        .ToProperty(this, vm => vm.DisplayName, initialValue: string.Empty);
}
```

### Async Commands with CancellationToken
```csharp
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

public InventoryViewModel()
{
    LoadDataCommand = ReactiveCommand.CreateFromTask(async (cancellationToken) =>
    {
        await LoadInventoryDataAsync(cancellationToken);
    });

    // Error handling with structured logging
    LoadDataCommand.ThrownExceptions
        .Subscribe(ex =>
        {
            _logger.LogError(ex, "Failed to load inventory data");
            ErrorMessage = "Failed to load data. Please try again.";
        });
}

private async Task LoadInventoryDataAsync(CancellationToken cancellationToken = default)
{
    var result = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
        Model_AppVariables.ConnectionString,
        "inv_inventory_Get_All",
        new Dictionary<string, object>()
    );

    if (result.Status == 0)
    {
        // Process successful result
        Items.Clear();
        foreach (DataRow row in result.Data.Rows)
        {
            Items.Add(MapToInventoryItem(row));
        }
    }
    else
    {
        throw new InvalidOperationException(result.Message);
    }
}
```

### Collection Binding with Modern Patterns
```csharp
// Modern collection initialization
public ObservableCollection<InventoryItemViewModel> Items { get; } = [];

// Reactive collection operations
public ReactiveCommand<InventoryItemViewModel, Unit> RemoveItemCommand { get; }

public InventoryViewModel()
{
    RemoveItemCommand = ReactiveCommand.Create<InventoryItemViewModel>(item =>
    {
        Items.Remove(item);
    });

    // React to collection changes
    Items.ObserveCollectionChanges()
        .Subscribe(change =>
        {
            _logger.LogInformation("Inventory collection changed: {ChangeType}", change.EventArgs.Action);
        });
}
```

## ?? Service Integration (.NET 8)

### Constructor Injection with Nullable Guards
```csharp
public sealed class InventoryTabViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IUserAndTransactionServices _userService;
    private readonly IApplicationStateService _applicationState;
    private readonly ILogger<InventoryTabViewModel> _logger;

    public InventoryTabViewModel(
        IInventoryService inventoryService,
        IUserAndTransactionServices userService,
        IApplicationStateService applicationState,
        ILogger<InventoryTabViewModel> logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _logger.LogInformation("InventoryTabViewModel initialized with dependency injection");
        
        InitializeViewModel();
    }
}
```

### Service Usage with Result Patterns
```csharp
private async Task<Result> SaveInventoryItemAsync(CancellationToken cancellationToken = default)
{
    try
    {
        var inventoryItem = new InventoryItem
        {
            PartId = SelectedPart,
            Operation = SelectedOperation, // Workflow step identifier
            Location = SelectedLocation,
            Quantity = Quantity,
            Notes = Notes,
            TransactionType = TransactionType.IN, // User is adding stock
            User = _applicationState.CurrentUser ?? "System",
            CreatedDate = DateTime.UtcNow
        };

        var result = await _inventoryService.AddInventoryItemAsync(inventoryItem, cancellationToken);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Inventory item saved successfully: {PartId}", inventoryItem.PartId);
            await ResetFormAsync();
            return Result.Success();
        }
        else
        {
            _logger.LogWarning("Failed to save inventory item: {Error}", result.ErrorMessage);
            return Result.Failure(result.ErrorMessage ?? "Unknown error occurred");
        }
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
        _logger.LogInformation("Save operation was cancelled");
        return Result.Failure("Operation was cancelled");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error saving inventory item");
        return Result.Failure($"Failed to save inventory item: {ex.Message}");
    }
}
```

## ?? Business Logic Integration (.NET 8)

### Transaction Type Logic with Pattern Matching
```csharp
public enum UserAction { AddInventory, RemoveInventory, TransferInventory }

// Modern switch expression
private TransactionType GetTransactionTypeForUserAction(UserAction action) => action switch
{
    UserAction.AddInventory => TransactionType.IN,      // User adding stock
    UserAction.RemoveInventory => TransactionType.OUT,  // User removing stock
    UserAction.TransferInventory => TransactionType.TRANSFER, // User moving stock
    _ => throw new ArgumentException($"Unknown user action: {action}")
};

// Operation validation with modern patterns
private static bool IsValidOperation(string? operation) =>
    !string.IsNullOrWhiteSpace(operation) && 
    operation.All(char.IsDigit) && 
    operation.Length is > 0 and <= 10;
```

### Data Validation with Record Types
```csharp
public record ValidationResult(bool IsValid, string? ErrorMessage = null)
{
    public static ValidationResult Valid() => new(true);
    public static ValidationResult Invalid(string message) => new(false, message);
}

private ValidationResult ValidateInventoryData()
{
    if (string.IsNullOrWhiteSpace(SelectedPart))
        return ValidationResult.Invalid("Part ID is required");
        
    if (string.IsNullOrWhiteSpace(SelectedOperation))
        return ValidationResult.Invalid("Operation is required");
        
    if (!IsValidOperation(SelectedOperation))
        return ValidationResult.Invalid("Operation must be numeric");
        
    if (Quantity <= 0)
        return ValidationResult.Invalid("Quantity must be greater than zero");
        
    return ValidationResult.Valid();
}
```

## ?? Error Handling (.NET 8)

### Centralized Error Management
```csharp
public sealed class ViewModelErrorHandler
{
    private readonly ILogger _logger;
    
    public ViewModelErrorHandler(ILogger logger)
    {
        _logger = logger;
    }
    
    public string HandleError(Exception ex) => ex switch
    {
        ValidationException validationEx => validationEx.Message,
        TimeoutException => "The operation timed out. Please try again.",
        UnauthorizedAccessException => "You don't have permission to perform this action.",
        InvalidOperationException invalidOp => invalidOp.Message,
        _ => "An unexpected error occurred. Please contact support if the problem persists."
    };
}

// Usage in ViewModel
private readonly ViewModelErrorHandler _errorHandler;

private void HandleCommandError(Exception ex)
{
    var userMessage = _errorHandler.HandleError(ex);
    ErrorMessage = userMessage;
    HasError = true;
    
    _logger.LogError(ex, "ViewModel command error: {UserMessage}", userMessage);
}
```

## ?? Testing Support (.NET 8)

### Testable ViewModel Design
```csharp
public sealed class InventoryTabViewModelTests
{
    [Test]
    public async Task SaveCommand_ValidData_SavesSuccessfully()
    {
        // Arrange
        var mockInventoryService = new Mock<IInventoryService>();
        var mockUserService = new Mock<IUserAndTransactionServices>();
        var mockApplicationState = new Mock<IApplicationStateService>();
        var mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        
        mockApplicationState.Setup(x => x.CurrentUser).Returns("TestUser");
        mockInventoryService.Setup(x => x.AddInventoryItemAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(Result.Success());
        
        var viewModel = new InventoryTabViewModel(
            mockInventoryService.Object,
            mockUserService.Object,
            mockApplicationState.Object,
            mockLogger.Object);
        
        // Set up valid form data
        viewModel.SelectedPart = "TEST-PART";
        viewModel.SelectedOperation = "100";
        viewModel.SelectedLocation = "Test Location";
        viewModel.Quantity = 10;
        
        // Act
        await viewModel.SaveCommand.Execute();
        
        // Assert
        mockInventoryService.Verify(s => s.AddInventoryItemAsync(
            It.Is<InventoryItem>(item => 
                item.PartId == "TEST-PART" && 
                item.TransactionType == TransactionType.IN), 
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## ?? Development Guidelines (.NET 8)

### Adding New ViewModels
1. **Use file-scoped namespaces**: `namespace MTM_WIP_Application_Avalonia.ViewModels;`
2. **Sealed classes**: Mark ViewModels as `sealed` unless inheritance is specifically needed
3. **Nullable reference types**: Enable and properly handle nullable reference types
4. **Constructor injection**: Use primary constructors where appropriate
5. **Structured logging**: Use `ILogger<T>` with structured logging patterns
6. **CancellationToken support**: Include CancellationToken in async operations
7. **Result patterns**: Use Result<T> for service operations
8. **Modern C# features**: Utilize pattern matching, switch expressions, and record types

### ViewModel Conventions (.NET 8)
- **Naming**: ViewModels end with `ViewModel` (e.g., `InventoryTabViewModel`)
- **Namespace**: `MTM_WIP_Application_Avalonia.ViewModels`
- **Properties**: Use `RaiseAndSetIfChanged` with nullable reference type annotations
- **Commands**: Use `ReactiveCommand` with proper CanExecute and error handling
- **Collections**: Initialize with collection expressions `[]`
- **Async Operations**: Always include CancellationToken support
- **Error Handling**: Subscribe to `ThrownExceptions` for all commands

## ?? Related Documentation

- **View Bindings**: See `Views/README.md` for Avalonia 11+ view patterns
- **Service Contracts**: `Services/Interfaces/` for service interfaces with .NET 8 patterns
- **UI Documentation**: `Documentation/Development/UI_Documentation/` for component specifications
- **ReactiveUI Guide**: Official ReactiveUI documentation for .NET 8 compatibility
- **GitHub Instructions**: `.github/copilot-instructions.md` for comprehensive development guidelines

---

*This directory implements the ViewModel layer using ReactiveUI with .NET 8 modern patterns, providing type-safe reactive data binding and command handling for the MTM WIP Application UI components.*