# Create ReactiveUI ViewModel - Custom Prompt

## Instructions
Use this prompt when you need to generate ViewModels with ReactiveUI patterns, commands, and observable properties.

## Persona
**ReactiveUI Specialist Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Generate a ReactiveUI ViewModel for [Purpose] following MTM patterns.  
Include observable properties with RaiseAndSetIfChanged, ReactiveCommands with proper error handling,  
WhenAnyValue patterns for derived properties, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

## Purpose
For generating ViewModels with ReactiveUI patterns, commands, and observable properties.

## Usage Examples

### Example 1: Inventory Management ViewModel
```
Generate a ReactiveUI ViewModel for inventory management following MTM patterns.  
Include observable properties with RaiseAndSetIfChanged, ReactiveCommands with proper error handling,  
WhenAnyValue patterns for derived properties, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

### Example 2: Settings Configuration ViewModel
```
Generate a ReactiveUI ViewModel for application settings configuration following MTM patterns.  
Include observable properties with RaiseAndSetIfChanged, ReactiveCommands with proper error handling,  
WhenAnyValue patterns for derived properties, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

## Guidelines

### Basic ViewModel Template
```csharp
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class [Name]ViewModel : ReactiveObject
{
    // TODO: Inject services via constructor
    // private readonly IInventoryService _inventoryService;
    // private readonly IUserAndTransactionServices _userService;
    // private readonly IApplicationStateService _applicationState;

    #region Observable Properties

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private string _selectedPartId = string.Empty;
    public string SelectedPartId
    {
        get => _selectedPartId;
        set => this.RaiseAndSetIfChanged(ref _selectedPartId, value);
    }

    private string _selectedOperation = string.Empty;
    public string SelectedOperation
    {
        get => _selectedOperation;
        set => this.RaiseAndSetIfChanged(ref _selectedOperation, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => this.RaiseAndSetIfChanged(ref _quantity, value);
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => this.RaiseAndSetIfChanged(ref _hasError, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    #endregion

    #region Collections

    public ObservableCollection<InventoryItemViewModel> InventoryItems { get; } = new();
    public ObservableCollection<string> PartIds { get; } = new();
    public ObservableCollection<string> Operations { get; } = new();
    public ObservableCollection<string> Locations { get; } = new();

    #endregion

    #region Derived Properties (OAPH)

    private readonly ObservableAsPropertyHelper<bool> _isFormValid;
    public bool IsFormValid => _isFormValid.Value;

    private readonly ObservableAsPropertyHelper<string> _displayText;
    public string DisplayText => _displayText.Value;

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<InventoryItemViewModel, Unit> RemoveItemCommand { get; }

    #endregion

    #region Constructor

    public [Name]ViewModel()
    {
        // TODO: Replace with actual DI constructor
        // public [Name]ViewModel(
        //     IInventoryService inventoryService,
        //     IUserAndTransactionServices userService,
        //     IApplicationStateService applicationState)
        // {
        //     _inventoryService = inventoryService;
        //     _userService = userService;
        //     _applicationState = applicationState;

        InitializeCommands();
        InitializeDerivedProperties();
    }

    #endregion

    #region Initialization

    private void InitializeCommands()
    {
        // Async command with loading state
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsLoading = true;
            HasError = false;
            
            try
            {
                await LoadInventoryDataAsync();
                StatusMessage = "Data loaded successfully";
            }
            finally
            {
                IsLoading = false;
            }
        });

        // Command with CanExecute validation
        var canSave = this.WhenAnyValue(
            vm => vm.SelectedPartId,
            vm => vm.SelectedOperation,
            vm => vm.Quantity,
            (partId, operation, qty) =>
                !string.IsNullOrWhiteSpace(partId) &&
                !string.IsNullOrWhiteSpace(operation) &&
                qty > 0);

        SaveCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsLoading = true;
            HasError = false;
            
            try
            {
                await SaveInventoryItemAsync();
                StatusMessage = "Saved successfully";
            }
            finally
            {
                IsLoading = false;
            }
        }, canSave);

        // Simple sync command
        RefreshCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await LoadDataCommand.Execute();
        });

        ClearCommand = ReactiveCommand.Create(() =>
        {
            SelectedPartId = string.Empty;
            SelectedOperation = string.Empty;
            Quantity = 0;
            StatusMessage = "Form cleared";
        });

        // Parameterized command
        RemoveItemCommand = ReactiveCommand.Create<InventoryItemViewModel>(item =>
        {
            InventoryItems.Remove(item);
            StatusMessage = $"Removed {item.PartId}";
        });

        // Centralized error handling
        LoadDataCommand.ThrownExceptions
            .Merge(SaveCommand.ThrownExceptions)
            .Subscribe(HandleCommandException);
    }

    private void InitializeDerivedProperties()
    {
        // Form validation combining multiple properties
        _isFormValid = this.WhenAnyValue(
                vm => vm.SelectedPartId,
                vm => vm.SelectedOperation,
                vm => vm.Quantity,
                (partId, operation, qty) =>
                    !string.IsNullOrWhiteSpace(partId) &&
                    !string.IsNullOrWhiteSpace(operation) &&
                    qty > 0)
            .ToProperty(this, vm => vm.IsFormValid, initialValue: false);

        // Computed display text
        _displayText = this.WhenAnyValue(
                vm => vm.SelectedPartId,
                vm => vm.SelectedOperation,
                vm => vm.Quantity,
                (partId, operation, qty) =>
                    $"{partId} - Operation: {operation} - Qty: {qty}")
            .ToProperty(this, vm => vm.DisplayText, initialValue: string.Empty);
    }

    #endregion

    #region Command Implementations

    private async Task LoadInventoryDataAsync()
    {
        // TODO: Implement via service
        // var result = await _inventoryService.GetInventoryAsync(searchCriteria);
        // if (result.IsSuccess)
        // {
        //     InventoryItems.Clear();
        //     foreach (var item in result.Value)
        //     {
        //         InventoryItems.Add(new InventoryItemViewModel(item));
        //     }
        // }
        // else
        // {
        //     throw new InvalidOperationException(result.ErrorMessage);
        // }

        await Task.Delay(100); // Placeholder
    }

    private async Task SaveInventoryItemAsync()
    {
        // TODO: Implement via service with MTM patterns
        // var inventoryItem = new InventoryItem
        // {
        //     PartId = SelectedPartId,
        //     Operation = SelectedOperation, // String number (e.g., "90", "100")
        //     Quantity = Quantity,
        //     Location = SelectedLocation,
        //     User = _applicationState.CurrentUser,
        //     TransactionDate = DateTime.Now
        // };
        //
        // var result = await _inventoryService.CreateInventoryItemAsync(inventoryItem);
        // if (!result.IsSuccess)
        // {
        //     throw new InvalidOperationException(result.ErrorMessage);
        // }

        await Task.Delay(100); // Placeholder
    }

    #endregion

    #region Error Handling

    private void HandleCommandException(Exception ex)
    {
        HasError = true;
        ErrorMessage = GetUserFriendlyErrorMessage(ex);
        StatusMessage = "An error occurred";

        // TODO: Log error via Service_ErrorHandler
        // Service_ErrorHandler.HandleException(
        //     ex,
        //     ErrorSeverity.Medium,
        //     source: $"{GetType().Name}_CommandError",
        //     additionalData: new Dictionary<string, object>
        //     {
        //         ["PartId"] = SelectedPartId,
        //         ["Operation"] = SelectedOperation,
        //         ["Quantity"] = Quantity
        //     });
    }

    private static string GetUserFriendlyErrorMessage(Exception ex)
    {
        return ex switch
        {
            InvalidOperationException => ex.Message,
            TimeoutException => "The operation timed out. Please try again.",
            UnauthorizedAccessException => "You don't have permission to perform this action.",
            _ => "An unexpected error occurred. Please try again or contact support."
        };
    }

    #endregion

    #region Events

    // MTM-specific event patterns
    public event EventHandler<InventoryItemSavedEventArgs>? InventoryItemSaved;
    public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

    protected virtual void OnInventoryItemSaved(InventoryItem item)
    {
        InventoryItemSaved?.Invoke(this, new InventoryItemSavedEventArgs
        {
            PartId = item.PartId,
            Operation = item.Operation,
            Quantity = item.Quantity
        });
    }

    protected virtual void OnQuickActionExecuted(string partId, string operation, int quantity)
    {
        QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
        {
            PartId = partId,
            Operation = operation, // Just a workflow step number
            Quantity = quantity
        });
    }

    #endregion

    #region Validation

    private bool ValidatePartId(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
        {
            ErrorMessage = "Part ID is required";
            return false;
        }

        if (partId.Length > 50)
        {
            ErrorMessage = "Part ID cannot exceed 50 characters";
            return false;
        }

        // TODO: Add MTM-specific Part ID validation
        return true;
    }

    private bool ValidateOperation(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
        {
            ErrorMessage = "Operation is required";
            return false;
        }

        if (!operation.All(char.IsDigit))
        {
            ErrorMessage = "Operation must be numeric";
            return false;
        }

        // TODO: Add MTM-specific operation validation
        return true;
    }

    #endregion

    #region Cleanup

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isFormValid?.Dispose();
            _displayText?.Dispose();
        }
        base.Dispose(disposing);
    }

    #endregion
}
```

### MTM Event Args Classes
```csharp
public class InventoryItemSavedEventArgs : EventArgs
{
    public required string PartId { get; init; }
    public required string Operation { get; init; }
    public required int Quantity { get; init; }
}

public class QuickActionExecutedEventArgs : EventArgs
{
    public required string PartId { get; init; }
    public required string Operation { get; init; } // Workflow step number
    public required int Quantity { get; init; }
}
```

### Dependency Injection Pattern
```csharp
// Constructor for DI container
public [Name]ViewModel(
    IInventoryService inventoryService,
    IUserAndTransactionServices userService,
    IApplicationStateService applicationState)
{
    _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
    _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

    InitializeCommands();
    InitializeDerivedProperties();
}
```

## Technical Requirements

### ReactiveUI Patterns
- **Observable Properties**: Use `RaiseAndSetIfChanged` for all settable properties
- **Commands**: Use `ReactiveCommand` for all user interactions
- **Derived Properties**: Use `ObservableAsPropertyHelper` (OAPH) for computed values
- **Validation**: Use `WhenAnyValue` for real-time validation
- **Error Handling**: Centralized exception handling via `ThrownExceptions`

### MTM-Specific Requirements
- **Data Types**: Part ID (string), Operation (string numbers), Quantity (integer)
- **Events**: Use MTM event argument classes for inter-component communication
- **Validation**: Include MTM business rule validation
- **Error Handling**: Integration with `Service_ErrorHandler`

### Performance Considerations
- Use `ObservableAsPropertyHelper` for expensive computations
- Implement proper disposal for reactive subscriptions
- Use `CanExecute` observables to prevent unnecessary command execution
- Implement loading states for async operations

## Related Files
- `.github/codingconventions.instruction.md` - ReactiveUI patterns
- `Models/` - Data models for ViewModel properties
- `Services/` - Service interfaces for dependency injection
- `Views/` - Corresponding AXAML views

## Quality Checklist
- [ ] All properties use `RaiseAndSetIfChanged`
- [ ] Commands use `ReactiveCommand` patterns
- [ ] Derived properties use `ObservableAsPropertyHelper`
- [ ] Validation uses `WhenAnyValue` patterns
- [ ] Error handling is centralized
- [ ] MTM data patterns implemented
- [ ] Events use MTM event args classes
- [ ] Dependency injection prepared
- [ ] Loading states implemented
- [ ] Proper disposal implemented
- [ ] Comprehensive TODO comments for business logic