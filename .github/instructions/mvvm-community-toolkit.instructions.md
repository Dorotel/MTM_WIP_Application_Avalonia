# MVVM Community Toolkit - MTM WIP Application Instructions

**Framework**: MVVM Community Toolkit 8.3.2  
**Target**: .NET 8 with C# 12  
**Pattern**: Source Generator-Based MVVM  
**Created**: September 4, 2025  
**Updated**: 2025-09-21 (Phase 1 Material.Avalonia Integration)

---

## 📚 Comprehensive Avalonia Documentation Reference

**IMPORTANT**: This repository contains the complete Avalonia documentation straight from the official website in the `.github/Avalonia-Documentation/` folder. For MVVM patterns:

- **MVVM Patterns**: `.github/Avalonia-Documentation/concepts/the-mvvm-pattern/`
- **Data Binding**: `.github/Avalonia-Documentation/guides/data-binding/`
- **ViewModels**: `.github/Avalonia-Documentation/concepts/the-mvvm-pattern/viewmodel.md`
- **Binding from Code**: `.github/Avalonia-Documentation/guides/data-binding/binding-from-code.md`
- **Community Toolkit Integration**: `.github/Avalonia-Documentation/guides/data-binding/data-binding-syntax.md`

**Always reference the local Avalonia-Documentation folder for the most current MVVM implementation guidance.**

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

## 🚀 Advanced MVVM Community Toolkit Patterns

### Advanced [ObservableProperty] Implementation Patterns

#### Complex Property Validation with Manufacturing Context

```csharp
[ObservableObject]
public partial class InventoryTabViewModel : BaseViewModel
{
    // Advanced validation with manufacturing business rules
    [ObservableProperty]
    [Required(ErrorMessage = "Part ID is required for inventory operations")]
    [RegularExpression(@"^[A-Z0-9\-]{3,50}$", ErrorMessage = "Part ID must be 3-50 characters, uppercase alphanumeric with dashes")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecuteTransaction))]
    private string partId = string.Empty;

    // Property with complex validation and manufacturing workflow logic
    [ObservableProperty]
    [Range(1, 999999, ErrorMessage = "Quantity must be between 1 and 999,999")]
    [NotifyDataErrorInfo]
    [NotifyPropertyChangedFor(nameof(CanExecuteTransaction))]
    [NotifyPropertyChangedFor(nameof(TotalValue))]
    private int quantity = 1;

    // Dependent property with manufacturing calculations
    public decimal TotalValue => Quantity * (UnitCost ?? 0);
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalValue))]
    private decimal? unitCost;

    // Advanced partial method for business logic
    partial void OnPartIdChanged(string value)
    {
        // Auto-format part ID according to MTM standards
        if (!string.IsNullOrWhiteSpace(value))
        {
            var formatted = value.ToUpperInvariant().Trim();
            if (formatted != value)
            {
                PartId = formatted; // This will trigger validation
                return;
            }
        }

        // Clear related fields when part changes
        UnitCost = null;
        ClearErrors(nameof(PartId));
        
        // Trigger async validation for manufacturing part ID
        _ = ValidatePartIdAsync(value);
    }

    private async Task ValidatePartIdAsync(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId)) return;

        try
        {
            // Check if part exists in manufacturing master data
            var partExists = await _masterDataService.ValidatePartIdAsync(partId);
            if (!partExists)
            {
                AddError(nameof(PartId), $"Part ID '{partId}' not found in master data");
            }
            
            // Load part details for cost calculation
            var partDetails = await _masterDataService.GetPartDetailsAsync(partId);
            UnitCost = partDetails?.StandardCost;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating part ID: {PartId}", partId);
            AddError(nameof(PartId), "Unable to validate part ID. Check network connection.");
        }
    }

    // Complex computed property with manufacturing logic
    public bool CanExecuteTransaction => 
        !HasErrors &&
        !IsLoading &&
        !string.IsNullOrWhiteSpace(PartId) &&
        !string.IsNullOrWhiteSpace(SelectedOperation) &&
        Quantity > 0 &&
        !string.IsNullOrWhiteSpace(Location) &&
        IsValidManufacturingWorkflow();

    private bool IsValidManufacturingWorkflow()
    {
        // Manufacturing-specific validation
        if (TransactionType == "OUT" && Quantity > AvailableQuantity)
        {
            return false; // Cannot remove more than available
        }

        if (SelectedOperation == "90" && TransactionType == "OUT")
        {
            return false; // Cannot remove from receiving operation
        }

        return true;
    }
}
```

#### Memory-Efficient Collection Properties for Large Datasets

```csharp
[ObservableObject]
public partial class TransactionHistoryViewModel : BaseViewModel
{
    // Virtualized collection for large manufacturing datasets
    [ObservableProperty]
    private VirtualizingCollection<TransactionRecord> transactionHistory = new();

    // Paginated loading pattern for manufacturing transaction history
    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int pageSize = 100;

    [ObservableProperty]
    private int totalRecords;

    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

    // Advanced collection management with memory optimization
    private readonly ConcurrentDictionary<int, List<TransactionRecord>> _pageCache = new();
    private readonly SemaphoreSlim _loadingSemaphore = new(1, 1);

    [RelayCommand]
    private async Task LoadPageAsync(int page)
    {
        if (page < 1 || page > TotalPages) return;

        await _loadingSemaphore.WaitAsync();
        try
        {
            IsLoading = true;

            // Check cache first
            if (_pageCache.TryGetValue(page, out var cachedData))
            {
                UpdateTransactionHistory(cachedData);
                CurrentPage = page;
                return;
            }

            // Load from database
            var searchCriteria = new TransactionSearchCriteria
            {
                PageNumber = page,
                PageSize = PageSize,
                PartId = FilterPartId,
                StartDate = FilterStartDate,
                EndDate = FilterEndDate
            };

            var result = await _transactionService.GetPagedTransactionsAsync(searchCriteria);
            
            if (result.IsSuccess)
            {
                // Cache the result
                _pageCache.TryAdd(page, result.Data.ToList());
                
                UpdateTransactionHistory(result.Data);
                TotalRecords = result.TotalCount;
                CurrentPage = page;
                
                // Cleanup old cache entries to prevent memory leaks
                CleanupCache();
            }
            else
            {
                AddError("Failed to load transaction history");
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Load transaction page");
        }
        finally
        {
            IsLoading = false;
            _loadingSemaphore.Release();
        }
    }

    private void UpdateTransactionHistory(IEnumerable<TransactionRecord> transactions)
    {
        TransactionHistory.Clear();
        foreach (var transaction in transactions)
        {
            TransactionHistory.Add(transaction);
        }
    }

    private void CleanupCache()
    {
        // Keep only current page ±2 pages to manage memory
        var pagesToKeep = Enumerable.Range(Math.Max(1, CurrentPage - 2), 5)
                                  .Where(p => p <= TotalPages)
                                  .ToHashSet();

        var keysToRemove = _pageCache.Keys.Where(k => !pagesToKeep.Contains(k)).ToList();
        foreach (var key in keysToRemove)
        {
            _pageCache.TryRemove(key, out _);
        }
    }

    // Dispose pattern for proper cleanup
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _loadingSemaphore?.Dispose();
            _pageCache.Clear();
        }
        base.Dispose(disposing);
    }
}
```

### Advanced [RelayCommand] Patterns with Manufacturing Context

#### Complex Async Command with Error Recovery

```csharp
[ObservableObject]
public partial class InventoryOperationsViewModel : BaseViewModel
{
    private readonly SemaphoreSlim _operationSemaphore = new(1, 1);
    private CancellationTokenSource? _currentOperationCancellation;

    // Advanced command with cancellation, retry, and manufacturing workflow validation
    [RelayCommand(CanExecute = nameof(CanExecuteInventoryOperation))]
    private async Task ExecuteInventoryOperationAsync(InventoryOperation operation)
    {
        // Cancel any existing operation
        _currentOperationCancellation?.Cancel();
        _currentOperationCancellation = new CancellationTokenSource();
        var cancellationToken = _currentOperationCancellation.Token;

        await _operationSemaphore.WaitAsync(cancellationToken);
        try
        {
            IsLoading = true;
            ClearErrors();

            // Pre-execution validation
            var validationResult = await ValidateManufacturingOperationAsync(operation);
            if (!validationResult.IsValid)
            {
                AddError($"Operation validation failed: {validationResult.ErrorMessage}");
                return;
            }

            // Execute with retry logic for manufacturing operations
            var success = await ExecuteWithRetryAsync(
                () => PerformInventoryOperationAsync(operation, cancellationToken),
                maxRetries: 3,
                cancellationToken: cancellationToken
            );

            if (success)
            {
                StatusMessage = $"✅ {operation.Type} operation completed successfully";
                
                // Trigger UI refresh
                await RefreshInventoryDataAsync();
                
                // Create QuickButton for successful operations
                await CreateQuickButtonFromOperationAsync(operation);
                
                // Send notification to other ViewModels
                _messenger.Send(new InventoryOperationCompletedMessage(operation));
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "⏹️ Operation cancelled";
        }
        catch (ManufacturingBusinessRuleException ex)
        {
            AddError($"Manufacturing rule violation: {ex.Message}");
            Logger.LogWarning(ex, "Manufacturing business rule violated for operation: {@Operation}", operation);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Execute inventory operation");
        }
        finally
        {
            IsLoading = false;
            _operationSemaphore.Release();
        }
    }

    private async Task<bool> ExecuteWithRetryAsync(
        Func<Task> operation, 
        int maxRetries, 
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await operation();
                return true;
            }
            catch (DatabaseConnectionException) when (attempt < maxRetries)
            {
                Logger.LogWarning("Database connection failed on attempt {Attempt}/{MaxRetries}", attempt, maxRetries);
                
                // Exponential backoff for manufacturing operations
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt - 1));
                await Task.Delay(delay, cancellationToken);
            }
            catch (ManufacturingBusinessRuleException)
            {
                // Don't retry business rule violations
                throw;
            }
        }
        
        return false;
    }

    private async Task<ManufacturingValidationResult> ValidateManufacturingOperationAsync(InventoryOperation operation)
    {
        // Manufacturing-specific validation
        switch (operation.Type)
        {
            case "IN":
                return await ValidateReceivingOperationAsync(operation);
            case "OUT":
                return await ValidateRemovalOperationAsync(operation);
            case "TRANSFER":
                return await ValidateTransferOperationAsync(operation);
            default:
                return ManufacturingValidationResult.Invalid($"Unknown operation type: {operation.Type}");
        }
    }

    private bool CanExecuteInventoryOperation => 
        !IsLoading && 
        SelectedOperation != null && 
        !HasErrors &&
        !string.IsNullOrWhiteSpace(CurrentUser);

    // Batch operation command for manufacturing efficiency
    [RelayCommand]
    private async Task ExecuteBatchOperationsAsync(IEnumerable<InventoryOperation> operations)
    {
        if (!operations?.Any() == true) return;

        var operationsList = operations.ToList();
        var totalOperations = operationsList.Count;
        var completedOperations = 0;

        IsLoading = true;
        ProgressValue = 0;
        ProgressMaximum = totalOperations;
        
        try
        {
            // Group operations by type for manufacturing efficiency
            var groupedOperations = operationsList.GroupBy(op => op.Type);
            
            foreach (var group in groupedOperations)
            {
                StatusMessage = $"Processing {group.Count()} {group.Key} operations...";
                
                // Process in batches for better performance
                await foreach (var batch in group.Chunk(10).ToAsyncEnumerable())
                {
                    var tasks = batch.Select(async op =>
                    {
                        try
                        {
                            await PerformInventoryOperationAsync(op, CancellationToken.None);
                            Interlocked.Increment(ref completedOperations);
                            ProgressValue = completedOperations;
                            return (Operation: op, Success: true, Error: (Exception?)null);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, "Batch operation failed: {@Operation}", op);
                            return (Operation: op, Success: false, Error: ex);
                        }
                    });

                    var results = await Task.WhenAll(tasks);
                    
                    // Report any failures
                    foreach (var result in results.Where(r => !r.Success))
                    {
                        AddError($"Failed to process {result.Operation.PartId}: {result.Error?.Message}");
                    }
                }
            }

            var successCount = operationsList.Count - Errors.Count;
            StatusMessage = $"✅ Batch operation completed: {successCount}/{totalOperations} successful";
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Execute batch operations");
        }
        finally
        {
            IsLoading = false;
            ProgressValue = 0;
        }
    }
}
```

### ❌ Advanced Anti-Patterns (Avoid These)

#### Memory Leaks in ViewModels

```csharp
// ❌ WRONG: Event subscription without cleanup leads to memory leaks
[ObservableObject]
public partial class LeakyViewModel : BaseViewModel
{
    public LeakyViewModel()
    {
        // This creates a memory leak - ViewModel will never be garbage collected
        SomeStaticEventPublisher.DataChanged += OnDataChanged;
        
        // Timer without disposal
        _timer = new Timer(UpdateData, null, 0, 1000);
    }
    
    private void OnDataChanged(object sender, EventArgs e)
    {
        // Handler is never unsubscribed
    }
}

// ✅ CORRECT: Proper event subscription management
[ObservableObject]
public partial class ProperViewModel : BaseViewModel, IDisposable
{
    private readonly Timer _timer;
    private bool _disposed = false;

    public ProperViewModel()
    {
        SomeStaticEventPublisher.DataChanged += OnDataChanged;
        _timer = new Timer(UpdateData, null, 0, 1000);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            // Unsubscribe from events
            SomeStaticEventPublisher.DataChanged -= OnDataChanged;
            
            // Dispose resources
            _timer?.Dispose();
            
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private void OnDataChanged(object sender, EventArgs e)
    {
        if (_disposed) return;
        // Handle event
    }
}
```

#### Blocking UI Thread with Synchronous Operations

```csharp
// ❌ WRONG: Blocking the UI thread in manufacturing operations
[RelayCommand]
private void LoadInventoryData() // Synchronous method
{
    IsLoading = true;
    
    // This blocks the UI thread - NEVER do this
    var data = _inventoryService.GetInventoryDataAsync().Result;
    
    InventoryItems.Clear();
    foreach (var item in data)
    {
        InventoryItems.Add(item); // UI freezes during large data loads
    }
    
    IsLoading = false;
}

// ✅ CORRECT: Async operations with proper UI responsiveness
[RelayCommand]
private async Task LoadInventoryDataAsync()
{
    IsLoading = true;
    try
    {
        // Non-blocking async operation
        var data = await _inventoryService.GetInventoryDataAsync().ConfigureAwait(false);
        
        // Update UI on UI thread in batches to maintain responsiveness
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            InventoryItems.Clear();
        });
        
        // Add items in batches to prevent UI freezing
        const int batchSize = 50;
        for (int i = 0; i < data.Count; i += batchSize)
        {
            var batch = data.Skip(i).Take(batchSize);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                foreach (var item in batch)
                {
                    InventoryItems.Add(item);
                }
            });
            
            // Allow UI to update between batches
            await Task.Delay(10);
        }
    }
    catch (Exception ex)
    {
        await HandleErrorAsync(ex, "Load inventory data");
    }
    finally
    {
        IsLoading = false;
    }
}
```

#### Improper Property Dependency Chains

```csharp
// ❌ WRONG: Circular property dependencies cause infinite loops
[ObservableObject]
public partial class CircularDependencyViewModel : BaseViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PropertyB))]
    private string propertyA = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PropertyA))] // Circular dependency!
    private string propertyB = string.Empty;
    
    // This creates infinite recursion
    partial void OnPropertyAChanged(string value)
    {
        PropertyB = value.ToUpperInvariant(); // Triggers PropertyB change
    }
    
    partial void OnPropertyBChanged(string value)
    {
        PropertyA = value.ToLowerInvariant(); // Triggers PropertyA change - INFINITE LOOP!
    }
}

// ✅ CORRECT: Break dependency cycles with flags or different approaches
[ObservableObject]
public partial class ProperDependencyViewModel : BaseViewModel
{
    private bool _isUpdatingFromCode = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FormattedValue))]
    private string rawValue = string.Empty;

    // Computed property doesn't create circular dependency
    public string FormattedValue => !string.IsNullOrWhiteSpace(RawValue) 
        ? RawValue.ToUpperInvariant() 
        : string.Empty;

    partial void OnRawValueChanged(string value)
    {
        if (_isUpdatingFromCode) return;
        
        _isUpdatingFromCode = true;
        try
        {
            // Perform side effects without circular updates
            ValidateValue(value);
        }
        finally
        {
            _isUpdatingFromCode = false;
        }
    }
}
```

## 🔧 Manufacturing-Specific Troubleshooting Guide

### Common MVVM Issues in Manufacturing Context

#### Issue: ViewModel Not Updating UI During Long Operations

**Symptoms**: UI freezes during manufacturing batch operations, user cannot see progress

**Solution**:

```csharp
// Use progress reporting and async operations
[RelayCommand]
private async Task ProcessLargeBatchAsync()
{
    var progress = new Progress<BatchProgress>(p =>
    {
        ProgressValue = p.CompletedItems;
        StatusMessage = $"Processing {p.CompletedItems}/{p.TotalItems} items...";
    });
    
    await _batchService.ProcessItemsAsync(Items, progress, CancellationToken.None);
}
```

#### Issue: Memory Usage Grows During Manufacturing Operations

**Symptoms**: Application memory increases over time, especially during shift changes

**Solution**: Implement proper collection management and disposal patterns:

```csharp
// Clear collections periodically
private async Task CleanupOldDataAsync()
{
    // Keep only recent transactions (e.g., last 1000)
    if (TransactionHistory.Count > 1000)
    {
        var itemsToRemove = TransactionHistory
            .OrderBy(t => t.Timestamp)
            .Take(TransactionHistory.Count - 1000)
            .ToList();
            
        foreach (var item in itemsToRemove)
        {
            TransactionHistory.Remove(item);
        }
    }
}
```

#### Issue: Commands Not Updating CanExecute State

**Symptoms**: Buttons remain disabled/enabled inappropriately during manufacturing operations

**Solution**: Ensure proper property dependency notifications:

```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveInventoryCommand))]
[NotifyCanExecuteChangedFor(nameof(RemoveInventoryCommand))]
private string selectedPartId = string.Empty;

// Manually notify when complex conditions change
private void OnValidationStateChanged()
{
    SaveInventoryCommand.NotifyCanExecuteChanged();
    RemoveInventoryCommand.NotifyCanExecuteChanged();
}
```

### Performance Optimization for Manufacturing Workflows

#### Optimize Large Dataset Binding

```csharp
// Use virtualization for large manufacturing datasets
[ObservableProperty]
private CollectionView filteredTransactions = new();

public void ApplyManufacturingFilters()
{
    // Use CollectionView for better performance than ObservableCollection filtering
    FilteredTransactions.Filter = transaction =>
    {
        var t = (TransactionRecord)transaction;
        return (!FilterPartId.HasValue || t.PartId.Contains(FilterPartId)) &&
               (!FilterStartDate.HasValue || t.Timestamp >= FilterStartDate) &&
               (!FilterEndDate.HasValue || t.Timestamp <= FilterEndDate);
    };
}
```

#### Batch UI Updates for Manufacturing Efficiency

```csharp
// Batch multiple property changes to reduce UI updates
public void UpdateInventoryBatch(IEnumerable<InventoryUpdate> updates)
{
    // Suspend collection change notifications
    using var suspension = InventoryItems.SuspendNotifications();
    
    foreach (var update in updates)
    {
        var item = InventoryItems.FirstOrDefault(i => i.PartId == update.PartId);
        if (item != null)
        {
            item.Quantity = update.NewQuantity;
            item.LastUpdated = update.Timestamp;
        }
    }
    
    // All changes fire notifications at once when suspension is disposed
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

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**

- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**

- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**

- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**

- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**

- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**

- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**

- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
