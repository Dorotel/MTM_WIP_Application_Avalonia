# MVVM Community Toolkit - MTM WIP Application Instructions

**Framework**: MVVM Community Toolkit 8.3.2  
**Target**: .NET 8 with C# 12  
**Pattern**: Source Generator-Based MVVM  
**Created**: September 4, 2025  

---

## 🎯 MANDATORY: MVVM Community Toolkit Only

### CRITICAL: ReactiveUI Completely Removed
```csharp
// ✅ CORRECT: MVVM Community Toolkit pattern (ONLY pattern used in MTM)
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    [ObservableProperty]
    private string partId = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        try
        {
            // Business logic here
        }
        finally
        {
            IsLoading = false;
        }
    }
}

// ❌ WRONG: Never use ReactiveUI patterns (completely removed)
public class InventoryTabViewModel : ReactiveObject // DON'T USE
{
    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => this.RaiseAndSetIfChanged(ref _partId, value); // DON'T USE
    }

    public ReactiveCommand<Unit, Unit> SearchCommand { get; } // DON'T USE
}
```

### Required Using Statements
```csharp
// Standard MVVM Community Toolkit imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
```

---

## 🏗️ BaseViewModel Architecture

### Standard BaseViewModel Pattern
```csharp
// BaseViewModel.cs - Used by all ViewModels in MTM application
[ObservableObject]
public abstract partial class BaseViewModel : INotifyPropertyChanged
{
    protected readonly ILogger Logger;

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasErrors))]
    private bool isLoading;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> errors = new();

    public bool HasErrors => Errors.Count > 0;

    protected void ClearErrors()
    {
        Errors.Clear();
    }

    protected void AddError(string error)
    {
        if (!Errors.Contains(error))
        {
            Errors.Add(error);
        }
    }

    protected virtual async Task HandleErrorAsync(Exception ex, string context)
    {
        Logger.LogError(ex, "Error in {Context}", context);
        await Services.ErrorHandling.HandleErrorAsync(ex, context);
        
        AddError($"Error in {context}: {ex.Message}");
        StatusMessage = $"Operation failed: {ex.Message}";
    }

    // Virtual method for cleanup
    public virtual void Dispose()
    {
        // Cleanup logic in derived classes
    }
}
```

### ViewModel Dependency Injection Pattern
```csharp
// Standard constructor pattern for all ViewModels
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IMasterDataService _masterDataService;
    private readonly IMessenger _messenger;

    public InventoryTabViewModel(
        ILogger<InventoryTabViewModel> logger,
        IInventoryService inventoryService,
        IMasterDataService masterDataService,
        IMessenger messenger)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(inventoryService);
        ArgumentNullException.ThrowIfNull(masterDataService);
        ArgumentNullException.ThrowIfNull(messenger);

        _inventoryService = inventoryService;
        _masterDataService = masterDataService;
        _messenger = messenger;

        // Initialize collections and state
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            IsLoading = true;
            await LoadInitialDataAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "ViewModel initialization");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

---

## 🔧 ObservableProperty Patterns

### Basic Property Declarations
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Simple properties with automatic change notification
    [ObservableProperty]
    private string partId = string.Empty;

    [ObservableProperty]
    private string selectedOperation = string.Empty;

    [ObservableProperty]
    private int quantity = 1;

    [ObservableProperty]
    private string location = string.Empty;

    [ObservableProperty]
    private bool canSave;

    [ObservableProperty]
    private DateTime lastUpdated = DateTime.Now;

    [ObservableProperty]
    private ObservableCollection<string> operations = new();

    [ObservableProperty]
    private ObservableCollection<InventoryItem> inventoryItems = new();
}
```

### Advanced Property Patterns with Validation
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Property with validation
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required")]
    [StringLength(50, ErrorMessage = "Part ID cannot exceed 50 characters")]
    [NotifyDataErrorInfo]
    private string partId = string.Empty;

    // Property with dependent property notifications
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteTransaction))]
    [NotifyPropertyChangedFor(nameof(IsFormValid))]
    private int quantity = 1;

    // Computed properties that depend on other properties
    public bool CanExecuteTransaction => 
        !IsLoading && 
        !string.IsNullOrWhiteSpace(PartId) && 
        !string.IsNullOrWhiteSpace(SelectedOperation) && 
        Quantity > 0;

    public bool IsFormValid => 
        !string.IsNullOrWhiteSpace(PartId) && 
        !string.IsNullOrWhiteSpace(SelectedOperation) && 
        Quantity > 0 && 
        !string.IsNullOrWhiteSpace(Location);

    // Property with custom partial method for additional logic
    [ObservableProperty]
    private string selectedOperation = string.Empty;

    partial void OnSelectedOperationChanged(string value)
    {
        // Custom logic when operation changes
        Logger.LogInformation("Operation changed to: {Operation}", value);
        
        // Clear dependent fields
        if (value != selectedOperation)
        {
            PartId = string.Empty;
            Quantity = 1;
        }

        // Trigger validation
        ValidateForm();
    }

    partial void OnPartIdChanged(string value)
    {
        // Auto-format part ID
        if (!string.IsNullOrWhiteSpace(value))
        {
            PartId = value.ToUpperInvariant().Trim();
        }

        // Clear validation errors for this field
        ClearErrors();
    }
}
```

### Collection Properties and Management
```csharp
[ObservableObject]
public partial class QuickButtonsTabViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<QuickActionModel> quickButtons = new();

    [ObservableProperty]
    private QuickActionModel? selectedQuickButton;

    [ObservableProperty]
    private ObservableCollection<string> partIds = new();

    [ObservableProperty]
    private ObservableCollection<string> operations = new();

    // Collection management methods
    public void AddQuickButton(QuickActionModel quickButton)
    {
        if (!QuickButtons.Contains(quickButton))
        {
            QuickButtons.Add(quickButton);
            Logger.LogInformation("Added quick button: {PartId}-{Operation}", quickButton.PartId, quickButton.Operation);
        }
    }

    public void RemoveQuickButton(QuickActionModel quickButton)
    {
        if (QuickButtons.Remove(quickButton))
        {
            Logger.LogInformation("Removed quick button: {PartId}-{Operation}", quickButton.PartId, quickButton.Operation);
        }
    }

    public void UpdateQuickButtons(IEnumerable<QuickActionModel> newButtons)
    {
        QuickButtons.Clear();
        foreach (var button in newButtons)
        {
            QuickButtons.Add(button);
        }
    }
}
```

---

## ⚡ RelayCommand Patterns

### Basic Command Patterns
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Simple async command
    [RelayCommand]
    private async Task SaveAsync()
    {
        IsLoading = true;
        ClearErrors();
        
        try
        {
            var result = await _inventoryService.AddInventoryAsync(
                PartId, 
                SelectedOperation, 
                Quantity, 
                Location
            );

            if (result)
            {
                StatusMessage = "Inventory saved successfully";
                await ResetFormAsync();
            }
            else
            {
                AddError("Failed to save inventory");
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Save inventory");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Command with parameter
    [RelayCommand]
    private async Task DeleteInventoryAsync(InventoryItem item)
    {
        if (item == null) return;

        try
        {
            IsLoading = true;
            
            var result = await _inventoryService.RemoveInventoryAsync(
                item.PartId, 
                item.Operation, 
                item.Quantity, 
                item.Location
            );

            if (result)
            {
                InventoryItems.Remove(item);
                StatusMessage = $"Removed inventory for {item.PartId}";
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Delete inventory");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Simple synchronous command
    [RelayCommand]
    private void ResetForm()
    {
        PartId = string.Empty;
        SelectedOperation = string.Empty;
        Quantity = 1;
        Location = string.Empty;
        ClearErrors();
        StatusMessage = string.Empty;
    }

    // Command with CanExecute logic
    [RelayCommand(CanExecute = nameof(CanExecuteTransaction))]
    private async Task ExecuteTransactionAsync()
    {
        // Command implementation
        await SaveAsync();
    }

    // The CanExecute property (must match command name)
    private bool CanExecuteTransaction => 
        !IsLoading && 
        IsFormValid && 
        !string.IsNullOrWhiteSpace(PartId);
}
```

### Advanced Command Patterns
```csharp
[ObservableObject]
public partial class TransactionHistoryTabViewModel : BaseViewModel
{
    // Command with complex parameter handling
    [RelayCommand]
    private async Task SearchTransactionsAsync(SearchCriteria? criteria)
    {
        criteria ??= new SearchCriteria();
        
        try
        {
            IsLoading = true;
            ClearErrors();

            var results = await _transactionService.SearchTransactionsAsync(
                criteria.PartId,
                criteria.StartDate,
                criteria.EndDate,
                criteria.Operation
            );

            TransactionHistory.Clear();
            foreach (var transaction in results)
            {
                TransactionHistory.Add(transaction);
            }

            StatusMessage = $"Found {results.Count} transactions";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Search transactions");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Command with cancellation token support
    [RelayCommand]
    private async Task ExportDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            IsLoading = true;
            var progress = new Progress<int>(percent => 
            {
                StatusMessage = $"Exporting... {percent}%";
            });

            await _exportService.ExportTransactionsAsync(
                TransactionHistory.ToList(),
                progress,
                cancellationToken
            );

            StatusMessage = "Export completed successfully";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Export was cancelled";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Export data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Batch operation command
    [RelayCommand]
    private async Task ProcessBatchAsync(IEnumerable<InventoryItem> items)
    {
        if (!items?.Any() == true) return;

        var itemList = items.ToList();
        var progress = 0;
        var total = itemList.Count;

        try
        {
            IsLoading = true;

            foreach (var item in itemList)
            {
                await _inventoryService.ProcessItemAsync(item);
                progress++;
                
                StatusMessage = $"Processing... {progress}/{total}";
                
                // Allow UI to update
                await Task.Delay(10);
            }

            StatusMessage = $"Processed {total} items successfully";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Batch processing");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### Command State Management
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Properties that affect command state
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteTransactionCommand))]
    private string partId = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(ExecuteTransactionCommand))]
    private bool isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private InventoryItem? selectedItem;

    // Commands with dynamic CanExecute
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync() { /* Implementation */ }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync() { /* Implementation */ }

    // CanExecute methods
    private bool CanSave => !IsLoading && !string.IsNullOrWhiteSpace(PartId) && IsFormValid;
    private bool CanDelete => !IsLoading && SelectedItem != null;

    // Manual command state refresh when complex conditions change
    private void RefreshCommandStates()
    {
        SaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        ExecuteTransactionCommand.NotifyCanExecuteChanged();
    }

    // Method called when validation state changes
    private void OnValidationChanged()
    {
        RefreshCommandStates();
    }
}
```

---

## 📬 Messaging Patterns

### WeakReferenceMessenger Integration
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel, 
    IRecipient<InventoryUpdatedMessage>, 
    IRecipient<MasterDataUpdatedMessage>
{
    private readonly IMessenger _messenger;

    public InventoryTabViewModel(
        ILogger<InventoryTabViewModel> logger,
        IInventoryService inventoryService,
        IMessenger messenger)
        : base(logger)
    {
        _messenger = messenger;
        
        // Register for messages
        _messenger.RegisterAll(this);
    }

    // Receive inventory update messages
    public void Receive(InventoryUpdatedMessage message)
    {
        Logger.LogInformation("Received inventory update for {PartId}", message.PartId);
        
        // Update local data
        var existingItem = InventoryItems.FirstOrDefault(i => 
            i.PartId == message.PartId && 
            i.Operation == message.Operation);

        if (existingItem != null)
        {
            existingItem.Quantity = message.NewQuantity;
            existingItem.LastUpdated = message.Timestamp;
        }
        else
        {
            InventoryItems.Add(new InventoryItem
            {
                PartId = message.PartId,
                Operation = message.Operation,
                Quantity = message.NewQuantity,
                Location = message.Location,
                LastUpdated = message.Timestamp
            });
        }
    }

    // Receive master data updates
    public void Receive(MasterDataUpdatedMessage message)
    {
        Logger.LogInformation("Received master data update: {DataType}", message.DataType);
        
        switch (message.DataType)
        {
            case MasterDataType.PartIds:
                _ = RefreshPartIdsAsync();
                break;
            case MasterDataType.Operations:
                _ = RefreshOperationsAsync();
                break;
            case MasterDataType.Locations:
                _ = RefreshLocationsAsync();
                break;
        }
    }

    // Send messages when inventory changes
    [RelayCommand]
    private async Task SaveAsync()
    {
        // ... save logic ...

        // Send update message to other ViewModels
        _messenger.Send(new InventoryUpdatedMessage
        {
            PartId = PartId,
            Operation = SelectedOperation,
            NewQuantity = Quantity,
            Location = Location,
            Timestamp = DateTime.Now,
            UserId = CurrentUser.UserId
        });
    }

    // Cleanup messaging on disposal
    public override void Dispose()
    {
        _messenger.UnregisterAll(this);
        base.Dispose();
    }
}

// Message definitions
public record InventoryUpdatedMessage
{
    public string PartId { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public int NewQuantity { get; init; }
    public string Location { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string UserId { get; init; } = string.Empty;
}

public record MasterDataUpdatedMessage
{
    public MasterDataType DataType { get; init; }
    public string[] UpdatedItems { get; init; } = Array.Empty<string>();
    public DateTime Timestamp { get; init; }
}

public enum MasterDataType
{
    PartIds,
    Operations,
    Locations,
    Users
}
```

---

## 🔄 Async Patterns and Data Loading

### Asynchronous Initialization
```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    public InventoryTabViewModel(
        ILogger<InventoryTabViewModel> logger,
        IInventoryService inventoryService,
        IMasterDataService masterDataService)
        : base(logger)
    {
        _inventoryService = inventoryService;
        _masterDataService = masterDataService;

        // Non-blocking initialization
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            
            // Load data in parallel
            var partIdsTask = _masterDataService.GetPartIdsAsync();
            var operationsTask = _masterDataService.GetOperationsAsync();
            var locationsTask = _masterDataService.GetLocationsAsync();
            var recentInventoryTask = _inventoryService.GetRecentInventoryAsync(50);

            await Task.WhenAll(partIdsTask, operationsTask, locationsTask, recentInventoryTask);

            // Update collections on UI thread
            PartIds.Clear();
            foreach (var partId in await partIdsTask)
            {
                PartIds.Add(partId);
            }

            Operations.Clear();
            foreach (var operation in await operationsTask)
            {
                Operations.Add(operation);
            }

            Locations.Clear();
            foreach (var location in await locationsTask)
            {
                Locations.Add(location);
            }

            InventoryItems.Clear();
            foreach (var item in await recentInventoryTask)
            {
                InventoryItems.Add(item);
            }

            StatusMessage = "Data loaded successfully";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Data initialization");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Refresh data command
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        await InitializeAsync();
    }
}
```

### Progressive Data Loading
```csharp
[ObservableObject]
public partial class TransactionHistoryTabViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<TransactionGroup> transactionGroups = new();

    [ObservableProperty]
    private bool isLoadingMore;

    [ObservableProperty]
    private bool hasMoreData = true;

    private int _currentPage = 1;
    private const int PageSize = 100;

    // Load initial data
    private async Task LoadInitialDataAsync()
    {
        try
        {
            IsLoading = true;
            _currentPage = 1;
            
            var transactions = await _transactionService.GetTransactionsPagedAsync(
                _currentPage, 
                PageSize
            );

            TransactionGroups.Clear();
            
            // Group transactions by date
            var groups = transactions
                .GroupBy(t => t.Timestamp.Date)
                .OrderByDescending(g => g.Key);

            foreach (var group in groups)
            {
                TransactionGroups.Add(new TransactionGroup
                {
                    Date = group.Key,
                    Transactions = new ObservableCollection<TransactionModel>(group)
                });
            }

            HasMoreData = transactions.Count == PageSize;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Load initial transaction data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Load more data (infinite scroll)
    [RelayCommand(CanExecute = nameof(CanLoadMore))]
    private async Task LoadMoreAsync()
    {
        try
        {
            IsLoadingMore = true;
            _currentPage++;

            var transactions = await _transactionService.GetTransactionsPagedAsync(
                _currentPage, 
                PageSize
            );

            if (transactions.Any())
            {
                // Add to existing groups or create new ones
                var groups = transactions
                    .GroupBy(t => t.Timestamp.Date)
                    .OrderByDescending(g => g.Key);

                foreach (var group in groups)
                {
                    var existingGroup = TransactionGroups.FirstOrDefault(g => g.Date == group.Key);
                    if (existingGroup != null)
                    {
                        foreach (var transaction in group)
                        {
                            existingGroup.Transactions.Add(transaction);
                        }
                    }
                    else
                    {
                        TransactionGroups.Add(new TransactionGroup
                        {
                            Date = group.Key,
                            Transactions = new ObservableCollection<TransactionModel>(group)
                        });
                    }
                }

                HasMoreData = transactions.Count == PageSize;
            }
            else
            {
                HasMoreData = false;
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Load more transaction data");
        }
        finally
        {
            IsLoadingMore = false;
        }
    }

    private bool CanLoadMore => !IsLoading && !IsLoadingMore && HasMoreData;
}
```

---

## 🧪 Testing ViewModel Patterns

### Unit Testing ViewModels
```csharp
public class InventoryTabViewModelTests
{
    private readonly Mock<ILogger<InventoryTabViewModel>> _mockLogger;
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<IMasterDataService> _mockMasterDataService;
    private readonly Mock<IMessenger> _mockMessenger;

    public InventoryTabViewModelTests()
    {
        _mockLogger = new Mock<ILogger<InventoryTabViewModel>>();
        _mockInventoryService = new Mock<IInventoryService>();
        _mockMasterDataService = new Mock<IMasterDataService>();
        _mockMessenger = new Mock<IMessenger>();
    }

    [Fact]
    public async Task SaveCommand_ValidData_CallsServiceAndSendsMessage()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.PartId = "TEST001";
        viewModel.SelectedOperation = "100";
        viewModel.Quantity = 5;
        viewModel.Location = "A01";

        _mockInventoryService
            .Setup(s => s.AddInventoryAsync("TEST001", "100", 5, "A01"))
            .ReturnsAsync(true);

        // Act
        await viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        _mockInventoryService.Verify(s => s.AddInventoryAsync("TEST001", "100", 5, "A01"), Times.Once);
        _mockMessenger.Verify(m => m.Send(It.IsAny<InventoryUpdatedMessage>()), Times.Once);
        Assert.Contains("success", viewModel.StatusMessage.ToLowerInvariant());
    }

    [Fact]
    public void CanExecuteTransaction_ValidForm_ReturnsTrue()
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.PartId = "TEST001";
        viewModel.SelectedOperation = "100";
        viewModel.Quantity = 5;
        viewModel.Location = "A01";
        viewModel.IsLoading = false;

        // Act & Assert
        Assert.True(viewModel.CanExecuteTransaction);
    }

    [Theory]
    [InlineData("", "100", 5, "A01", false)]    // Empty part ID
    [InlineData("TEST001", "", 5, "A01", false)] // Empty operation
    [InlineData("TEST001", "100", 0, "A01", false)] // Zero quantity
    [InlineData("TEST001", "100", 5, "", false)]    // Empty location
    [InlineData("TEST001", "100", 5, "A01", true)]  // Valid data
    public void CanExecuteTransaction_VariousInputs_ReturnsExpectedResult(
        string partId, string operation, int quantity, string location, bool expected)
    {
        // Arrange
        var viewModel = CreateViewModel();
        viewModel.PartId = partId;
        viewModel.SelectedOperation = operation;
        viewModel.Quantity = quantity;
        viewModel.Location = location;
        viewModel.IsLoading = false;

        // Act & Assert
        Assert.Equal(expected, viewModel.CanExecuteTransaction);
    }

    [Fact]
    public void Receive_InventoryUpdatedMessage_UpdatesLocalData()
    {
        // Arrange
        var viewModel = CreateViewModel();
        var existingItem = new InventoryItem
        {
            PartId = "TEST001",
            Operation = "100",
            Quantity = 5,
            Location = "A01"
        };
        viewModel.InventoryItems.Add(existingItem);

        var message = new InventoryUpdatedMessage
        {
            PartId = "TEST001",
            Operation = "100",
            NewQuantity = 10,
            Location = "A01",
            Timestamp = DateTime.Now
        };

        // Act
        viewModel.Receive(message);

        // Assert
        Assert.Equal(10, existingItem.Quantity);
        Assert.Equal(message.Timestamp, existingItem.LastUpdated);
    }

    private InventoryTabViewModel CreateViewModel()
    {
        return new InventoryTabViewModel(
            _mockLogger.Object,
            _mockInventoryService.Object,
            _mockMasterDataService.Object,
            _mockMessenger.Object
        );
    }
}
```

### Integration Testing with UI
```csharp
public class InventoryTabViewIntegrationTests
{
    [Fact]
    public async Task ViewModel_PropertyChanges_UpdateUI()
    {
        // Arrange
        using var app = AvaloniaApp.BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(Array.Empty<string>());

        var serviceProvider = CreateServiceProvider();
        var viewModel = serviceProvider.GetRequiredService<InventoryTabViewModel>();
        var view = new InventoryTabView { DataContext = viewModel };

        var window = new Window { Content = view };
        window.Show();

        // Act
        viewModel.PartId = "TEST001";
        viewModel.IsLoading = true;

        // Assert
        var partIdTextBox = view.FindControl<TextBox>("PartIdTextBox");
        var loadingIndicator = view.FindControl<ProgressBar>("LoadingIndicator");

        Assert.Equal("TEST001", partIdTextBox?.Text);
        Assert.True(loadingIndicator?.IsVisible);
    }

    [Fact]
    public async Task SaveCommand_Execution_UpdatesUIState()
    {
        // Arrange
        var serviceProvider = CreateServiceProvider();
        var viewModel = serviceProvider.GetRequiredService<InventoryTabViewModel>();
        
        viewModel.PartId = "TEST001";
        viewModel.SelectedOperation = "100";
        viewModel.Quantity = 5;
        viewModel.Location = "A01";

        // Act
        await viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        Assert.False(viewModel.IsLoading);
        Assert.Contains("success", viewModel.StatusMessage.ToLowerInvariant());
    }

    private IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Add mocked services
        services.AddSingleton(Mock.Of<IInventoryService>());
        services.AddSingleton(Mock.Of<IMasterDataService>());
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();
        services.AddLogging();
        
        services.AddTransient<InventoryTabViewModel>();
        
        return services.BuildServiceProvider();
    }
}
```

---

## 📚 Related MVVM Documentation

- **.NET Architecture**: [Good Practices](./dotnet-architecture-good-practices.instructions.md)
- **Database Integration**: [MySQL Database Patterns](./mysql-database-patterns.instructions.md)
- **UI Integration**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **Service Layer**: [Service Implementation Patterns](./service-layer-patterns.instructions.md)

---

**Document Status**: ✅ Complete MVVM Reference  
**Framework Version**: MVVM Community Toolkit 8.3.2  
**Last Updated**: September 4, 2025  
**MVVM Owner**: MTM Development Team
