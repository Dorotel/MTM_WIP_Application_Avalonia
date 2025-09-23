# MTM ViewModels - Technical Breakdown Documentation

**Document Version**: 1.0  
**Created**: September 4, 2025  
**Last Updated**: September 4, 2025  

---

## 📋 Executive Summary

This document provides a comprehensive technical breakdown of all ViewModels in the MTM WIP Application, detailing their responsibilities, dependencies, patterns, and relationships within the MVVM Community Toolkit architecture. The application contains 42+ ViewModels organized across MainForm, SettingsForm, TransactionsForm, Overlay, and Shared categories.

### Architecture Overview
- **Framework**: .NET 8 with MVVM Community Toolkit 8.3.2
- **Pattern**: `[ObservableObject]` with `[ObservableProperty]` and `[RelayCommand]` source generators
- **Base Class**: `BaseViewModel` with logging and common functionality
- **Dependency Injection**: Constructor injection with `ArgumentNullException.ThrowIfNull()` validation

---

## 🧠 ViewModel Architecture Categories

### 1. MainForm ViewModels (11 Files)
Core application functionality for inventory management operations.

### 2. SettingsForm ViewModels (23 Files) 
Administrative and configuration ViewModels for system management.

### 3. TransactionsForm ViewModels (2 Files)
Transaction history and audit trail ViewModels.

### 4. Overlay ViewModels (2 Files)
UI overlay functionality for suggestions and quick interactions.

### 5. Shared ViewModels (4 Files)
Base classes and shared functionality across all ViewModels.

---

## 📱 MainForm ViewModels Technical Specifications

### MainWindowViewModel.cs
**Purpose**: Primary window coordination and navigation management  
**Pattern**: Main coordinator ViewModel managing child ViewModels and application state

```csharp
[ObservableObject]
public partial class MainWindowViewModel : BaseViewModel
{
    // Navigation and State Management
    [ObservableProperty]
    private ViewModelBase _currentViewModel;
    
    [ObservableProperty] 
    private string _windowTitle = "MTM WIP Application";
    
    [ObservableProperty]
    private bool _isApplicationBusy;

    // Child ViewModel Management
    public InventoryTabViewModel InventoryTab { get; }
    public QuickButtonsViewModel QuickButtons { get; }
    public SettingsViewModel Settings { get; }
    
    // Navigation Commands
    [RelayCommand]
    private void NavigateToInventory() => CurrentViewModel = InventoryTab;
    
    [RelayCommand]
    private void NavigateToSettings() => CurrentViewModel = Settings;
}
```

**Dependencies**: 
- `ILogger<MainWindowViewModel>`
- Child ViewModels via constructor injection
- `INavigationService` (if implemented)

**Key Responsibilities**:
- Window-level state management and coordination
- Navigation between major application sections
- Application-wide busy state management
- Child ViewModel lifecycle management

---

### InventoryTabViewModel.cs
**Purpose**: Core inventory transaction entry and validation  
**Pattern**: Form-driven ViewModel with comprehensive validation and master data integration

```csharp
[ObservableObject]  
public partial class InventoryTabViewModel : BaseViewModel
{
    // Form Fields with Validation
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    private string _partId = string.Empty;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Operation is required")]
    private string _operation = string.Empty;
    
    [ObservableProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    private int _quantity = 1;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Location is required")]
    private string _location = string.Empty;
    
    // Master Data Collections
    [ObservableProperty]
    private ObservableCollection<string> _partIds = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _operations = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _locations = new();
    
    // State Management
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private string _statusMessage = string.Empty;
    
    // Commands
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        IsLoading = true;
        try
        {
            var inventoryItem = new InventoryItem
            {
                PartId = PartId,
                Operation = Operation,
                Quantity = Quantity,
                Location = Location
            };
            
            var result = await _inventoryService.AddInventoryAsync(inventoryItem);
            
            if (result.IsSuccess)
            {
                StatusMessage = "Inventory saved successfully";
                ResetFormCommand.Execute(null);
            }
            else
            {
                StatusMessage = $"Error: {result.Message}";
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Save inventory operation");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private bool CanSave() => !IsLoading && ValidateForm();
    
    [RelayCommand]
    private void ResetForm()
    {
        PartId = string.Empty;
        Operation = string.Empty;
        Quantity = 1;
        Location = string.Empty;
        StatusMessage = string.Empty;
    }
}
```

**Dependencies**:
- `ILogger<InventoryTabViewModel>`
- `IInventoryService` for database operations
- `IMasterDataService` for auto-complete data
- `IValidationService` (if implemented)

**Key Responsibilities**:
- Inventory transaction form management and validation
- Master data loading for auto-complete functionality
- Database persistence via service layer
- Real-time validation and user feedback

---

### QuickButtonsViewModel.cs  
**Purpose**: Recent transactions management with quick action shortcuts  
**Pattern**: Service-driven ViewModel with real-time data updates and command execution

```csharp
[ObservableObject]
public partial class QuickButtonsViewModel : BaseViewModel
{
    // Quick Actions Collection
    [ObservableProperty]
    private ObservableCollection<QuickButtonItem> _quickButtons = new();
    
    [ObservableProperty]
    private QuickButtonItem? _selectedQuickButton;
    
    // State Management  
    [ObservableProperty]
    private bool _isRefreshing;
    
    [ObservableProperty]
    private string _searchFilter = string.Empty;
    
    // Commands
    [RelayCommand]
    private async Task LoadQuickButtonsAsync()
    {
        IsRefreshing = true;
        try
        {
            var result = await _quickButtonsService.GetLast10TransactionsAsync();
            
            if (result.IsSuccess)
            {
                QuickButtons.Clear();
                foreach (var item in result.Data)
                {
                    QuickButtons.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Load quick buttons");
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    
    [RelayCommand]
    private async Task ExecuteQuickActionAsync(QuickButtonItem quickButton)
    {
        if (quickButton == null) return;
        
        try
        {
            var result = await _quickButtonsService.ExecuteQuickActionAsync(quickButton);
            
            if (result.IsSuccess)
            {
                // Trigger event for parent ViewModel coordination
                QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs 
                { 
                    QuickButton = quickButton 
                });
                
                // Refresh the list to show updated state
                await LoadQuickButtonsAsync();
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Execute quick action");
        }
    }
    
    [RelayCommand]
    private async Task RefreshAsync() => await LoadQuickButtonsAsync();
    
    // Events
    public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;
}
```

**Dependencies**:
- `ILogger<QuickButtonsViewModel>`
- `IQuickButtonsService` for recent transactions management
- `IProgressService` for progress tracking (if implemented)

**Key Responsibilities**:
- Loading and displaying last 10 inventory transactions
- Quick action execution with one-click repeat transactions
- Real-time refresh and data synchronization
- Event notification for parent ViewModel coordination

---

### AddItemViewModel.cs, RemoveItemViewModel.cs, TransferItemViewModel.cs
**Purpose**: Specialized inventory operation ViewModels  
**Pattern**: Form-specific ViewModels inheriting common validation and master data patterns

```csharp
[ObservableObject]
public partial class AddItemViewModel : BaseViewModel
{
    // Inherits similar pattern to InventoryTabViewModel but specialized for ADD operations
    
    [RelayCommand(CanExecute = nameof(CanAddItem))]
    private async Task AddItemAsync()
    {
        // ADD-specific business logic
        var result = await _inventoryService.AddInventoryAsync(CreateInventoryItem());
        // Handle result and update UI state
    }
    
    private bool CanAddItem() => ValidateForAdd();
    
    private InventoryItem CreateInventoryItem()
    {
        return new InventoryItem
        {
            PartId = PartId,
            Operation = Operation,
            Quantity = Quantity,
            Location = Location,
            TransactionType = "IN", // ADD operations are always IN
            UserId = Environment.UserName,
            Timestamp = DateTime.Now
        };
    }
}
```

---

## ⚙️ SettingsForm ViewModels Technical Specifications

### SettingsViewModel.cs
**Purpose**: Master settings coordination with virtual panel management  
**Pattern**: Hierarchical navigation ViewModel with state management and virtual panels

```csharp
[ObservableObject]
public partial class SettingsViewModel : BaseViewModel
{
    // Hierarchical Navigation
    [ObservableProperty]
    private ObservableCollection<SettingsCategoryViewModel> _categories = new();
    
    [ObservableProperty]
    private SettingsCategoryViewModel? _selectedCategory;
    
    // Virtual Panel Management
    [ObservableProperty]
    private ObservableCollection<SettingsPanelViewModel> _loadedPanels = new();
    
    [ObservableProperty]
    private SettingsPanelViewModel? _selectedPanel;
    
    // State Management
    [ObservableProperty]
    private bool _hasUnsavedChanges;
    
    [ObservableProperty]
    private string _currentPanelTitle = string.Empty;
    
    // Commands
    [RelayCommand]
    private async Task OnSelectedCategoryChangedAsync()
    {
        if (SelectedCategory == null) return;
        
        // Check for existing panel
        var existingPanel = LoadedPanels.FirstOrDefault(p => p.CategoryId == SelectedCategory.Id);
        
        if (existingPanel != null)
        {
            SelectedPanel = existingPanel;
        }
        else
        {
            // Create virtual panel
            var newPanel = await _panelManager.CreateVirtualPanelAsync(SelectedCategory);
            LoadedPanels.Add(newPanel);
            SelectedPanel = newPanel;
            
            // Create state snapshot for change tracking
            _stateManager.CreateSnapshot(SelectedCategory.Id, newPanel.ViewModel);
        }
        
        CurrentPanelTitle = SelectedCategory.DisplayName;
    }
    
    [RelayCommand]
    private async Task SaveAllChangesAsync()
    {
        var results = new List<ServiceResult>();
        
        foreach (var panel in LoadedPanels.Where(p => _stateManager.HasUnsavedChanges(p.CategoryId, p.ViewModel)))
        {
            var result = await _stateManager.SavePanelChangesAsync(panel.CategoryId);
            results.Add(result);
        }
        
        HasUnsavedChanges = false;
        StatusMessage = results.All(r => r.IsSuccess) ? "All changes saved" : "Some changes failed";
    }
    
    [RelayCommand]
    private async Task RevertAllChangesAsync()
    {
        foreach (var panel in LoadedPanels)
        {
            await _stateManager.RevertPanelChangesAsync(panel.CategoryId);
        }
        
        HasUnsavedChanges = false;
        StatusMessage = "All changes reverted";
    }
    
    // Initialization
    private void InitializeCategories()
    {
        Categories.Clear();
        
        var userManagement = new SettingsCategoryViewModel
        {
            Id = "user-management",
            DisplayName = "User Management",
            Icon = "👥",
            HasSubCategories = true
        };
        
        userManagement.SubCategories.Add(new SettingsCategoryViewModel
        {
            Id = "add-user",
            DisplayName = "Add User", 
            Icon = "➕",
            PanelType = typeof(AddUserViewModel),
            Parent = userManagement
        });
        
        // Add all other categories...
        Categories.Add(userManagement);
    }
}
```

**Dependencies**:
- `ILogger<SettingsViewModel>`
- `IVirtualPanelManager` for dynamic panel creation
- `ISettingsPanelStateManager` for change tracking
- `IServiceProvider` for ViewModel instantiation

**Key Responsibilities**:
- Hierarchical category navigation with TreeView support
- Virtual panel lifecycle management for memory efficiency
- State management with change tracking and rollback capabilities
- Unified save/revert operations across multiple panels

---

### AddPartViewModel.cs, EditPartViewModel.cs, RemovePartViewModel.cs
**Purpose**: Master data CRUD operations for Part IDs  
**Pattern**: CRUD-specific ViewModels with validation and database integration

```csharp
[ObservableObject]
public partial class AddPartViewModel : BaseViewModel
{
    // Form Fields
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    private string _partId = string.Empty;
    
    [ObservableProperty]
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    private string _description = string.Empty;
    
    [ObservableProperty]
    private string _category = string.Empty;
    
    [ObservableProperty]
    private bool _isActive = true;
    
    // State Management
    [ObservableProperty]
    private bool _isSaving;
    
    [ObservableProperty]
    private string _validationMessage = string.Empty;
    
    // Commands
    [RelayCommand(CanExecute = nameof(CanAddPart))]
    private async Task AddPartAsync()
    {
        IsSaving = true;
        ValidationMessage = string.Empty;
        
        try
        {
            // Validate input
            if (!ValidateInput())
            {
                ValidationMessage = "Please correct validation errors";
                return;
            }
            
            // Check for duplicates
            var existsResult = await _masterDataService.PartExistsAsync(PartId);
            if (existsResult.IsSuccess && existsResult.Data)
            {
                ValidationMessage = "Part ID already exists";
                return;
            }
            
            // Add to database
            var result = await _masterDataService.AddPartAsync(new PartInfo
            {
                PartId = PartId,
                Description = Description,
                Category = Category,
                IsActive = IsActive
            });
            
            if (result.IsSuccess)
            {
                StatusMessage = "Part added successfully";
                ResetFormCommand.Execute(null);
                
                // Notify parent of successful add
                PartAdded?.Invoke(this, new PartAddedEventArgs { PartId = PartId });
            }
            else
            {
                ValidationMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Add part operation");
            ValidationMessage = "An error occurred while adding the part";
        }
        finally
        {
            IsSaving = false;
        }
    }
    
    private bool CanAddPart() => !IsSaving && !string.IsNullOrWhiteSpace(PartId);
    
    [RelayCommand]
    private void ResetForm()
    {
        PartId = string.Empty;
        Description = string.Empty;
        Category = string.Empty;
        IsActive = true;
        ValidationMessage = string.Empty;
    }
    
    private bool ValidateInput()
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this);
        
        return Validator.TryValidateObject(this, context, validationResults, true);
    }
    
    // Events
    public event EventHandler<PartAddedEventArgs>? PartAdded;
}
```

**Dependencies**:
- `ILogger<AddPartViewModel>`
- `IMasterDataService` for part data operations
- `IValidationService` for custom validation rules

**Key Responsibilities**:
- Part master data creation with comprehensive validation
- Duplicate checking and business rule enforcement
- Event notification for data change coordination
- Form state management and reset capabilities

---

## 🔄 TransactionsForm ViewModels Technical Specifications

### TransactionHistoryViewModel.cs
**Purpose**: Transaction history display and filtering  
**Pattern**: Data-driven ViewModel with filtering, searching, and pagination

```csharp
[ObservableObject]
public partial class TransactionHistoryViewModel : BaseViewModel
{
    // Data Collections
    [ObservableProperty]
    private ObservableCollection<TransactionRecord> _transactions = new();
    
    [ObservableProperty]
    private ObservableCollection<TransactionRecord> _filteredTransactions = new();
    
    // Filtering and Search
    [ObservableProperty]
    private string _searchText = string.Empty;
    
    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(-30);
    
    [ObservableProperty]
    private DateTime _endDate = DateTime.Today;
    
    [ObservableProperty]
    private string _selectedTransactionType = "All";
    
    [ObservableProperty]
    private string _selectedUser = "All";
    
    // Pagination
    [ObservableProperty]
    private int _currentPage = 1;
    
    [ObservableProperty]
    private int _pageSize = 50;
    
    [ObservableProperty]
    private int _totalRecords;
    
    [ObservableProperty]
    private int _totalPages;
    
    // State Management
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private TransactionRecord? _selectedTransaction;
    
    // Commands
    [RelayCommand]
    private async Task LoadTransactionsAsync()
    {
        IsLoading = true;
        
        try
        {
            var filter = new TransactionFilter
            {
                StartDate = StartDate,
                EndDate = EndDate,
                TransactionType = SelectedTransactionType == "All" ? null : SelectedTransactionType,
                UserId = SelectedUser == "All" ? null : SelectedUser,
                SearchText = string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                PageNumber = CurrentPage,
                PageSize = PageSize
            };
            
            var result = await _transactionService.GetTransactionHistoryAsync(filter);
            
            if (result.IsSuccess)
            {
                Transactions.Clear();
                foreach (var transaction in result.Data.Transactions)
                {
                    Transactions.Add(transaction);
                }
                
                TotalRecords = result.Data.TotalCount;
                TotalPages = (int)Math.Ceiling((double)TotalRecords / PageSize);
                
                ApplyClientSideFiltering();
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Load transaction history");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private void ApplyFilter()
    {
        CurrentPage = 1;
        LoadTransactionsCommand.Execute(null);
    }
    
    [RelayCommand]
    private void ClearFilter()
    {
        SearchText = string.Empty;
        StartDate = DateTime.Today.AddDays(-30);
        EndDate = DateTime.Today;
        SelectedTransactionType = "All";
        SelectedUser = "All";
        CurrentPage = 1;
        
        LoadTransactionsCommand.Execute(null);
    }
    
    [RelayCommand]
    private async Task ExportTransactionsAsync()
    {
        try
        {
            var exportData = FilteredTransactions.ToList();
            var result = await _exportService.ExportTransactionsAsync(exportData);
            
            if (result.IsSuccess)
            {
                StatusMessage = $"Exported {exportData.Count} transactions successfully";
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Export transactions");
        }
    }
    
    private void ApplyClientSideFiltering()
    {
        var filtered = Transactions.AsEnumerable();
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(t => 
                t.PartId.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                t.Operation.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                t.Location.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        
        FilteredTransactions.Clear();
        foreach (var item in filtered)
        {
            FilteredTransactions.Add(item);
        }
    }
    
    // Property change handlers for real-time filtering
    partial void OnSearchTextChanged(string value) => ApplyClientSideFiltering();
    partial void OnSelectedTransactionTypeChanged(string value) => ApplyClientSideFiltering();
    partial void OnSelectedUserChanged(string value) => ApplyClientSideFiltering();
}
```

**Dependencies**:
- `ILogger<TransactionHistoryViewModel>`
- `ITransactionService` for transaction data operations
- `IExportService` for data export functionality
- `IDateTimeProvider` for testable date operations

**Key Responsibilities**:
- Transaction history loading with server-side pagination
- Multi-criteria filtering and real-time client-side search
- Export functionality for transaction data
- Detailed transaction record viewing and analysis

---

## 🎨 Overlay ViewModels Technical Specifications

### SuggestionOverlayViewModel.cs
**Purpose**: Auto-complete suggestions and intelligent input assistance  
**Pattern**: Real-time data ViewModel with fuzzy matching and keyboard navigation

```csharp
[ObservableObject]
public partial class SuggestionOverlayViewModel : BaseViewModel
{
    // Suggestions Collections
    [ObservableProperty]
    private ObservableCollection<SuggestionItem> _suggestions = new();
    
    [ObservableProperty]
    private SuggestionItem? _selectedSuggestion;
    
    [ObservableProperty]
    private int _selectedIndex = -1;
    
    // Input Tracking
    [ObservableProperty]
    private string _currentInput = string.Empty;
    
    [ObservableProperty]
    private string _targetField = string.Empty; // PartId, Operation, Location
    
    [ObservableProperty]
    private Point _overlayPosition;
    
    // State Management
    [ObservableProperty]
    private bool _isVisible;
    
    [ObservableProperty]
    private bool _isLoading;
    
    [ObservableProperty]
    private int _maxSuggestions = 10;
    
    // Commands
    [RelayCommand]
    private async Task LoadSuggestionsAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
        {
            Suggestions.Clear();
            IsVisible = false;
            return;
        }
        
        IsLoading = true;
        CurrentInput = input;
        
        try
        {
            var result = await _suggestionService.GetSuggestionsAsync(new SuggestionRequest
            {
                Input = input,
                FieldType = TargetField,
                MaxResults = MaxSuggestions,
                UseFuzzyMatching = true
            });
            
            if (result.IsSuccess)
            {
                Suggestions.Clear();
                foreach (var suggestion in result.Data)
                {
                    Suggestions.Add(suggestion);
                }
                
                IsVisible = Suggestions.Count > 0;
                SelectedIndex = Suggestions.Count > 0 ? 0 : -1;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading suggestions for {Input}", input);
            IsVisible = false;
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    [RelayCommand]
    private void SelectSuggestion(SuggestionItem? suggestion = null)
    {
        var itemToSelect = suggestion ?? SelectedSuggestion;
        
        if (itemToSelect != null)
        {
            SuggestionSelected?.Invoke(this, new SuggestionSelectedEventArgs
            {
                SelectedValue = itemToSelect.Value,
                TargetField = TargetField,
                AdditionalData = itemToSelect.AdditionalData
            });
            
            IsVisible = false;
            CurrentInput = string.Empty;
        }
    }
    
    [RelayCommand]
    private void NavigateUp()
    {
        if (SelectedIndex > 0)
        {
            SelectedIndex--;
            SelectedSuggestion = Suggestions[SelectedIndex];
        }
    }
    
    [RelayCommand]
    private void NavigateDown()
    {
        if (SelectedIndex < Suggestions.Count - 1)
        {
            SelectedIndex++;
            SelectedSuggestion = Suggestions[SelectedIndex];
        }
    }
    
    [RelayCommand]
    private void HideOverlay()
    {
        IsVisible = false;
        CurrentInput = string.Empty;
        SelectedIndex = -1;
        SelectedSuggestion = null;
    }
    
    // Public methods for external control
    public void ShowOverlayAt(Point position, string fieldType)
    {
        OverlayPosition = position;
        TargetField = fieldType;
        IsVisible = true;
    }
    
    // Events
    public event EventHandler<SuggestionSelectedEventArgs>? SuggestionSelected;
}
```

**Dependencies**:
- `ILogger<SuggestionOverlayViewModel>`
- `ISuggestionService` for intelligent suggestion algorithms
- `IMasterDataService` for base data lookup

**Key Responsibilities**:
- Real-time suggestion loading with fuzzy matching algorithms
- Keyboard navigation support for accessibility
- Position management for overlay display
- Event communication with parent input controls

---

## 🏗️ Shared ViewModels Technical Specifications

### BaseViewModel.cs
**Purpose**: Foundation base class with common functionality for all ViewModels  
**Pattern**: Abstract base with logging, property change notification, and common operations

```csharp
[ObservableObject]
public abstract partial class BaseViewModel
{
    protected readonly ILogger Logger;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _hasErrors;

    [ObservableProperty]
    private ObservableCollection<string> _validationErrors = new();

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Common validation method
    protected virtual bool ValidateViewModel()
    {
        ValidationErrors.Clear();
        HasErrors = false;

        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(this);

        if (!Validator.TryValidateObject(this, context, validationResults, true))
        {
            foreach (var validationResult in validationResults)
            {
                if (!string.IsNullOrEmpty(validationResult.ErrorMessage))
                {
                    ValidationErrors.Add(validationResult.ErrorMessage);
                }
            }
            
            HasErrors = ValidationErrors.Count > 0;
        }

        return !HasErrors;
    }

    // Common busy operation wrapper
    protected async Task<T> ExecuteWithBusyIndicatorAsync<T>(Func<Task<T>> operation, string operationName = "")
    {
        IsBusy = true;
        StatusMessage = $"Processing {operationName}...";

        try
        {
            Logger.LogInformation("Starting operation: {OperationName}", operationName);
            var result = await operation();
            Logger.LogInformation("Completed operation: {OperationName}", operationName);
            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in operation: {OperationName}", operationName);
            await Services.ErrorHandling.HandleErrorAsync(ex, operationName);
            throw;
        }
        finally
        {
            IsBusy = false;
            StatusMessage = string.Empty;
        }
    }

    // Property change notification with logging
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        Logger.LogTrace("Property changed: {PropertyName} in {ViewModelType}", 
            propertyName, GetType().Name);
    }

    // Cleanup method for derived classes
    public virtual void Dispose()
    {
        Logger.LogDebug("Disposing ViewModel: {ViewModelType}", GetType().Name);
    }
}
```

---

## 📊 ViewModel Dependency Matrix

| ViewModel Category | Primary Dependencies | Service Dependencies | Event Dependencies |
|-------------------|---------------------|--------------------|--------------------|
| **MainForm** | `BaseViewModel`, Logging | Inventory, MasterData, QuickButtons | PropertyChanged, Commands |
| **SettingsForm** | `BaseViewModel`, Logging | MasterData, Settings, Configuration | PropertyChanged, StateChanged |
| **TransactionsForm** | `BaseViewModel`, Logging | Transaction, Export | PropertyChanged, FilterChanged |
| **Overlay** | `BaseViewModel`, Logging | Suggestion, MasterData | SuggestionSelected, KeyboardNav |
| **Shared** | Logging Infrastructure | Configuration, Validation | PropertyChanged, ErrorOccurred |

---

## 🧪 Testing Strategy for ViewModels

### Unit Testing Standards
```csharp
public class InventoryTabViewModelTests
{
    private readonly Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<IMasterDataService> _mockMasterDataService;
    private readonly InventoryTabViewModel _viewModel;

    public InventoryTabViewModelTests()
    {
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();

        _viewModel = new InventoryTabViewModel(
            _mockLogger.Object, 
            _mockInventoryService.Object, 
            _mockMasterDataService.Object);
    }

    [Fact]
    public async Task SaveAsync_ValidInput_CallsInventoryService()
    {
        // Arrange
        _viewModel.PartId = "TEST001";
        _viewModel.Operation = "100";
        _viewModel.Quantity = 5;
        _viewModel.Location = "A01";
        
        _mockInventoryService
            .Setup(s => s.AddInventoryAsync(It.IsAny<InventoryItem>()))
            .ReturnsAsync(ServiceResult.Success("Added successfully"));

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _mockInventoryService.Verify(s => s.AddInventoryAsync(
            It.Is<InventoryItem>(i => i.PartId == "TEST001" && i.Quantity == 5)), 
            Times.Once);
    }
}
```

---

## 📚 Related Documentation

- **Epic Architecture**: [MTM Architecture Specification](../epic-architecture/specification.md)
- **Service Layer**: [MTM Service Documentation](../../../../Services/README.md)
- **MVVM Patterns**: [Community Toolkit Guide](../../../../.github/copilot/patterns/mtm-mvvm-community-toolkit.md)
- **Database Integration**: [Stored Procedures Guide](../../../../.github/copilot/patterns/mtm-stored-procedures-only.md)

---

**Document Status**: ✅ Complete Technical Reference  
**Next Review Date**: October 4, 2025  
**Technical Owner**: MTM Development Team