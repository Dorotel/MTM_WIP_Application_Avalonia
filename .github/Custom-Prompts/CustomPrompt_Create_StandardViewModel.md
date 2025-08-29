# Create Standard .NET ViewModel - Custom Prompt

## Instructions
Use this prompt when you need to generate ViewModels with standard .NET patterns, ICommand implementations, and INotifyPropertyChanged.

## Persona
**Standard .NET MVVM Specialist Copilot**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Generate a standard .NET ViewModel for [Purpose] following MTM patterns.  
Include properties with INotifyPropertyChanged, ICommand implementations with proper error handling,  
standard property validation, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

## Purpose
For generating ViewModels with standard .NET patterns, commands, and observable properties.

## Usage Examples

### Example 1: Inventory Management ViewModel
```
Generate a standard .NET ViewModel for inventory management following MTM patterns.  
Include properties with INotifyPropertyChanged, ICommand implementations with proper error handling,  
standard property validation, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

### Example 2: Settings Configuration ViewModel
```
Generate a standard .NET ViewModel for application settings configuration following MTM patterns.  
Include properties with INotifyPropertyChanged, ICommand implementations with proper error handling,  
standard property validation, and centralized exception handling.  
Use ObservableCollection for data collections and prepare for dependency injection.
```

## Guidelines

### Basic ViewModel Template
```csharp
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.Commands;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public class [Name]ViewModel : BaseViewModel, INotifyPropertyChanged
{
    // TODO: Inject services via constructor
    // private readonly IInventoryService _inventoryService;
    // private readonly IUserService _userService;
    // private readonly IApplicationStateService _applicationState;

    #region Properties

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string _selectedPartId = string.Empty;
    public string SelectedPartId
    {
        get => _selectedPartId;
        set => SetProperty(ref _selectedPartId, value);
    }

    private string _selectedOperation = string.Empty;
    public string SelectedOperation
    {
        get => _selectedOperation;
        set => SetProperty(ref _selectedOperation, value);
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set => SetProperty(ref _quantity, value);
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _hasError;
    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    #endregion

    #region Collections

    public ObservableCollection<InventoryItemViewModel> InventoryItems { get; } = new();
    public ObservableCollection<string> PartIds { get; } = new();
    public ObservableCollection<string> Operations { get; } = new();
    public ObservableCollection<string> Locations { get; } = new();

    #endregion

    #region Computed Properties

    public bool IsFormValid => !string.IsNullOrWhiteSpace(SelectedPartId) &&
                               !string.IsNullOrWhiteSpace(SelectedOperation) &&
                               Quantity > 0;

    public string DisplayText => $"{SelectedPartId} - Operation: {SelectedOperation} - Qty: {Quantity}";

    #endregion

    #region Commands

    public ICommand LoadDataCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand RefreshCommand { get; private set; }
    public ICommand ClearCommand { get; private set; }
    public ICommand RemoveItemCommand { get; private set; }

    #endregion

    #region Constructor

    public [Name]ViewModel()
    {
        // TODO: Replace with actual DI constructor
        // public [Name]ViewModel(
        //     IInventoryService inventoryService,
        //     IUserService userService,
        //     IApplicationStateService applicationState)
        // {
        //     _inventoryService = inventoryService;
        //     _userService = userService;
        //     _applicationState = applicationState;

        InitializeCommands();
    }

    #endregion

    #region Command Initialization

    private void InitializeCommands()
    {
        LoadDataCommand = new AsyncCommand(ExecuteLoadDataAsync);
        SaveCommand = new AsyncCommand(ExecuteSaveAsync, CanExecuteSave);
        RefreshCommand = new AsyncCommand(ExecuteRefreshAsync);
        ClearCommand = new RelayCommand(ExecuteClear);
        RemoveItemCommand = new RelayCommand<InventoryItemViewModel>(ExecuteRemoveItem);
    }

    #endregion

    #region Command Implementations

    private async Task ExecuteLoadDataAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            
            await LoadInventoryDataAsync();
            StatusMessage = "Data loaded successfully";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "LoadData");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ExecuteSaveAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            
            await SaveInventoryItemAsync();
            StatusMessage = "Saved successfully";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Save");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanExecuteSave()
    {
        return !string.IsNullOrWhiteSpace(SelectedPartId) && 
               !string.IsNullOrWhiteSpace(SelectedOperation) && 
               Quantity > 0 && 
               !IsLoading;
    }

    private async Task ExecuteRefreshAsync()
    {
        await ExecuteLoadDataAsync();
    }

    private void ExecuteClear()
    {
        SelectedPartId = string.Empty;
        SelectedOperation = string.Empty;
        Quantity = 0;
        StatusMessage = "Form cleared";
        HasError = false;
        ErrorMessage = string.Empty;
    }

    private void ExecuteRemoveItem(InventoryItemViewModel? item)
    {
        if (item != null)
        {
            InventoryItems.Remove(item);
            StatusMessage = $"Removed {item.PartId}";
        }
    }

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

    private async Task HandleErrorAsync(Exception ex, string operation)
    {
        HasError = true;
        ErrorMessage = GetUserFriendlyErrorMessage(ex);
        StatusMessage = "An error occurred";

        // TODO: Log error via ErrorHandling service
        await ErrorHandling.HandleErrorAsync(ex, operation, Environment.UserName, 
            new Dictionary<string, object>
            {
                ["PartId"] = SelectedPartId,
                ["Operation"] = SelectedOperation,
                ["Quantity"] = Quantity
            });
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

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(SelectedPartId))
        {
            ErrorMessage = "Part ID is required";
            return false;
        }

        if (Quantity <= 0)
        {
            ErrorMessage = "Quantity must be greater than zero";
            return false;
        }

        return true;
    }

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
    IUserService userService,
    IApplicationStateService applicationState)
{
    _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
    _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    _applicationState = applicationState ?? throw new ArgumentNullException(nameof(applicationState));

    InitializeCommands();
}
```

## Technical Requirements

### Standard .NET Patterns
- **Properties**: Use `SetProperty(ref _field, value)` from BaseViewModel for all settable properties
- **Commands**: Use `ICommand` implementations (AsyncCommand for async, RelayCommand for sync)
- **Computed Properties**: Use simple property getters that access other properties
- **Validation**: Use standard validation methods called from command implementations
- **Error Handling**: Centralized exception handling via ErrorHandling service

### MTM-Specific Requirements
- **Data Types**: Part ID (string), Operation (string numbers), Quantity (integer)
- **Events**: Use MTM event argument classes for inter-component communication
- **Validation**: Include MTM business rule validation
- **Error Handling**: Integration with `ErrorHandling` service

### Performance Considerations
- Use computed properties for expensive calculations
- Implement proper command CanExecute methods
- Use loading states for async operations
- Follow standard .NET disposal patterns

## Related Files
- `.github/Core-Instructions/codingconventions.instruction.md` - Standard .NET patterns
- `ViewModels/Shared/BaseViewModel.cs` - Base class with SetProperty implementation
- `Commands/` - ICommand implementations (AsyncCommand, RelayCommand)
- `Models/` - Data models for ViewModel properties
- `Services/` - Service interfaces for dependency injection
- `Views/` - Corresponding AXAML views

## Quality Checklist
- [ ] All properties use `SetProperty(ref _field, value)` from BaseViewModel
- [ ] Commands use `ICommand` implementations (AsyncCommand/RelayCommand)
- [ ] Computed properties use simple getters accessing other properties
- [ ] Validation uses standard validation methods
- [ ] Error handling uses ErrorHandling service
- [ ] MTM data patterns implemented
- [ ] Events use MTM event args classes
- [ ] Dependency injection prepared
- [ ] Loading states implemented
- [ ] CanExecute methods implemented for commands
- [ ] Comprehensive TODO comments for business logic