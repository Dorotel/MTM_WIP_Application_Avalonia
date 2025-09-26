# MTM MVVM Community Toolkit - Patterns & Implementation Guide

## 📋 Overview

The MTM WIP Application uses **MVVM Community Toolkit 8.3.2** exclusively for all ViewModel implementations. This document provides comprehensive guidance on established patterns, ensuring consistency across all 33 Views and their ViewModels.

## 🏗️ **Core Architecture Patterns**

### **ViewModel Base Class Structure**
```csharp
[ObservableObject]
public partial class BaseViewModel : IDisposable
{
    protected readonly ILogger _logger;
    private bool _disposed = false;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty] 
    private string title = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    protected BaseViewModel(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

## 🎯 **Established ViewModel Patterns**

### **1. Property Declaration Pattern**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    // ✅ CORRECT: Source generator properties
    [ObservableProperty]
    private string partId = string.Empty;

    [ObservableProperty]
    private string operation = string.Empty;

    [ObservableProperty]
    private int quantity;

    [ObservableProperty]
    private string location = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PartInfo> parts = new();

    [ObservableProperty]
    private PartInfo? selectedPart;

    // ✅ CORRECT: Property change handling
    partial void OnSelectedPartChanged(PartInfo? value)
    {
        if (value != null)
        {
            PartId = value.PartId;
            Operation = value.Operation;
            _logger.LogInformation("Selected part changed to {PartId}", value.PartId);
        }
    }
}
```

### **❌ Deprecated ReactiveUI Patterns (DO NOT USE)**
```csharp
// ❌ WRONG: ReactiveUI patterns removed from codebase
public class OldViewModel : ReactiveObject
{
    private string _partId = string.Empty;
    public string PartId
    {
        get => _partId;
        set => this.RaiseAndSetIfChanged(ref _partId, value);
    }
    
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }
}
```

### **2. Command Implementation Pattern**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;

    // ✅ CORRECT: Async RelayCommand pattern
    [RelayCommand]
    private async Task SearchAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            _logger.LogInformation("Starting inventory search for {PartId}", PartId);

            var result = await _inventoryService.GetInventoryByPartIdAsync(PartId);
            if (result.Success)
            {
                Parts.Clear();
                foreach (var part in result.Data)
                {
                    Parts.Add(part);
                }
                _logger.LogInformation("Found {Count} inventory items", result.Data.Count);
            }
            else
            {
                ErrorMessage = result.ErrorMessage;
                _logger.LogWarning("Inventory search failed: {Error}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "Inventory search operation");
            ErrorMessage = "Search operation failed. Please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ✅ CORRECT: Command with parameter
    [RelayCommand]
    private async Task RemoveItemAsync(PartInfo partInfo)
    {
        if (partInfo == null) return;

        var confirmation = await ShowConfirmationDialogAsync(
            $"Remove {partInfo.Quantity} units of {partInfo.PartId}?");
            
        if (confirmation)
        {
            await ProcessRemovalAsync(partInfo);
        }
    }

    // ✅ CORRECT: Command with validation
    [RelayCommand(CanExecute = nameof(CanExecuteSearch))]
    private async Task ValidatedSearchAsync()
    {
        await SearchAsync();
    }

    private bool CanExecuteSearch => !string.IsNullOrEmpty(PartId) && !IsLoading;
}
```

### **3. Service Integration Pattern**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    private readonly IInventoryService _inventoryService;
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;

    // ✅ CORRECT: Constructor injection with validation
    public InventoryViewModel(
        ILogger<InventoryViewModel> logger,
        IInventoryService inventoryService,
        IConfigurationService configurationService,
        INavigationService navigationService)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(inventoryService);
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(navigationService);

        _inventoryService = inventoryService;
        _configurationService = configurationService;
        _navigationService = navigationService;

        InitializeAsync().ConfigureAwait(false);
    }

    private async Task InitializeAsync()
    {
        try
        {
            await LoadInitialDataAsync();
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, "ViewModel initialization");
        }
    }
}
```

## 📱 **View-ViewModel Integration Patterns**

### **UserControl Pattern (All 33 Views)**
```csharp
// ✅ CORRECT: Minimal code-behind pattern
public partial class InventoryView : UserControl
{
    public InventoryView()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup subscriptions and resources
        if (DataContext is IDisposable disposableViewModel)
        {
            disposableViewModel.Dispose();
        }
        base.OnDetachedFromVisualTree(e);
    }
}
```

### **AXAML Binding Pattern**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainForm.InventoryView">

    <Grid RowDefinitions="Auto,*,Auto">
        <!-- Header -->
        <Border Grid.Row="0" Background="#0078D4" Padding="16,8">
            <TextBlock Text="{Binding Title}" 
                       Foreground="White" 
                       FontWeight="Bold" 
                       FontSize="16" />
        </Border>

        <!-- Content -->
        <ScrollViewer Grid.Row="1">
            <StackPanel Margin="16" Spacing="16">
                
                <!-- Search Section -->
                <Border Background="White" 
                        BorderBrush="#E0E0E0" 
                        BorderThickness="1" 
                        CornerRadius="8" 
                        Padding="16">
                    
                    <Grid x:Name="SearchGrid" ColumnDefinitions="*,Auto">
                        <TextBox Grid.Column="0" 
                                 Text="{Binding PartId}" 
                                 Watermark="Enter Part ID..." 
                                 Margin="0,0,8,0" />
                        
                        <Button Grid.Column="1" 
                                Content="Search" 
                                Command="{Binding SearchCommand}" 
                                Background="#0078D4" 
                                Foreground="White" 
                                Padding="16,8" 
                                CornerRadius="4" />
                    </Grid>
                </Border>

                <!-- Results Section -->
                <Border Background="White" 
                        BorderBrush="#E0E0E0" 
                        BorderThickness="1" 
                        CornerRadius="8" 
                        Padding="16">
                    
                    <DataGrid ItemsSource="{Binding Parts}" 
                              SelectedItem="{Binding SelectedPart}" 
                              AutoGenerateColumns="False" 
                              CanUserReorderColumns="True" 
                              CanUserResizeColumns="True">
                        
                        <DataGrid.Columns>
                            <DataGridTextColumn Header="Part ID" 
                                                Binding="{Binding PartId}" 
                                                Width="120" />
                            <DataGridTextColumn Header="Operation" 
                                                Binding="{Binding Operation}" 
                                                Width="100" />
                            <DataGridTextColumn Header="Quantity" 
                                                Binding="{Binding Quantity}" 
                                                Width="100" />
                            <DataGridTextColumn Header="Location" 
                                                Binding="{Binding Location}" 
                                                Width="120" />
                        </DataGrid.Columns>
                    </DataGrid>
                </Border>
            </StackPanel>
        </ScrollViewer>

        <!-- Loading Indicator -->
        <Grid Grid.Row="0" Grid.RowSpan="3" 
              Background="#80000000" 
              IsVisible="{Binding IsLoading}">
            <Border Background="White" 
                    CornerRadius="8" 
                    Padding="24" 
                    HorizontalAlignment="Center" 
                    VerticalAlignment="Center">
                <StackPanel Orientation="Horizontal" Spacing="12">
                    <ProgressRing IsIndeterminate="True" Width="24" Height="24" />
                    <TextBlock Text="Loading..." VerticalAlignment="Center" />
                </StackPanel>
            </Border>
        </Grid>
    </Grid>
</UserControl>
```

## 🔄 **Data Binding Patterns**

### **Observable Collections**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    // ✅ CORRECT: ObservableCollection for UI binding
    [ObservableProperty]
    private ObservableCollection<PartInfo> parts = new();

    [ObservableProperty]
    private ObservableCollection<string> locations = new();

    private async Task LoadPartsAsync()
    {
        var result = await _inventoryService.GetAllPartsAsync();
        if (result.Success)
        {
            // Clear and repopulate to maintain binding
            Parts.Clear();
            foreach (var part in result.Data)
            {
                Parts.Add(part);
            }
        }
    }

    // ✅ CORRECT: Collection modification with change notification
    private void AddPart(PartInfo newPart)
    {
        Parts.Add(newPart);
        _logger.LogInformation("Added part {PartId} to collection", newPart.PartId);
    }

    private void RemovePart(PartInfo part)
    {
        if (Parts.Remove(part))
        {
            _logger.LogInformation("Removed part {PartId} from collection", part.PartId);
        }
    }
}
```

### **Value Conversion Patterns**
```csharp
// Custom converters for specific UI needs
public class QuantityToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int quantity)
        {
            return quantity > 0 ? Brushes.Green : Brushes.Red;
        }
        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

## 🚨 **Error Handling Patterns**

### **ViewModel Error Management**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    private async Task HandleOperationAsync(Func<Task> operation, string context)
    {
        ClearError();
        IsLoading = true;

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            await Services.ErrorHandling.HandleErrorAsync(ex, context);
            ShowError("Operation failed. Please try again.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
        _logger.LogWarning("Error displayed to user: {Message}", message);
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }
}
```

## 📊 **Performance Optimization Patterns**

### **Lazy Loading Implementation**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel
{
    private readonly Lazy<Task<List<PartInfo>>> _partsCache;

    public InventoryViewModel(IInventoryService inventoryService, ILogger<InventoryViewModel> logger)
        : base(logger)
    {
        _inventoryService = inventoryService;
        _partsCache = new Lazy<Task<List<PartInfo>>>(LoadPartsFromServiceAsync);
    }

    [RelayCommand]
    private async Task LoadPartsAsync()
    {
        var parts = await _partsCache.Value;
        Parts.Clear();
        foreach (var part in parts)
        {
            Parts.Add(part);
        }
    }

    private async Task<List<PartInfo>> LoadPartsFromServiceAsync()
    {
        var result = await _inventoryService.GetAllPartsAsync();
        return result.Success ? result.Data : new List<PartInfo>();
    }
}
```

### **Memory Management**
```csharp
[ObservableObject]
public partial class InventoryViewModel : BaseViewModel, IDisposable
{
    private readonly Timer? _refreshTimer;
    private bool _disposed = false;

    public InventoryViewModel(ILogger<InventoryViewModel> logger) : base(logger)
    {
        // Setup periodic refresh
        _refreshTimer = new Timer(OnTimerTick, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _refreshTimer?.Dispose();
                Parts.Clear();
            }
            _disposed = true;
        }
        base.Dispose(disposing);
    }

    private async void OnTimerTick(object? state)
    {
        if (!_disposed && !IsLoading)
        {
            await RefreshDataAsync();
        }
    }
}
```

## 🔧 **Dependency Injection Integration**

### **Service Registration Pattern**
```csharp
// In ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMTMViewModels(this IServiceCollection services)
    {
        // ✅ CORRECT: Transient ViewModels for proper lifecycle
        services.TryAddTransient<InventoryViewModel>();
        services.TryAddTransient<QuickButtonsViewModel>();
        services.TryAddTransient<RemoveTabViewModel>();
        services.TryAddTransient<TransferTabViewModel>();
        services.TryAddTransient<AdvancedInventoryViewModel>();
        
        // All 33 ViewModels registered as transient
        services.TryAddTransient<SettingsViewModel>();
        services.TryAddTransient<ThemeSelectionViewModel>();
        services.TryAddTransient<DatabaseConnectionViewModel>();
        
        return services;
    }
}
```

### **ViewModel Factory Pattern**
```csharp
public interface IViewModelFactory<T> where T : BaseViewModel
{
    T Create();
}

public class ViewModelFactory<T> : IViewModelFactory<T> where T : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create()
    {
        return _serviceProvider.GetRequiredService<T>();
    }
}
```

## 📋 **Testing Patterns**

### **ViewModel Unit Testing**
```csharp
[TestClass]
public class InventoryViewModelTests
{
    private Mock<IInventoryService> _mockInventoryService = null!;
    private Mock<ILogger<InventoryViewModel>> _mockLogger = null!;
    private InventoryViewModel _viewModel = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockInventoryService = new Mock<IInventoryService>();
        _mockLogger = new Mock<ILogger<InventoryViewModel>>();
        _viewModel = new InventoryViewModel(_mockLogger.Object, _mockInventoryService.Object);
    }

    [TestMethod]
    public async Task SearchCommand_WithValidPartId_UpdatesPartsCollection()
    {
        // Arrange
        var expectedParts = new List<PartInfo> 
        { 
            new() { PartId = "TEST001", Operation = "90", Quantity = 10 } 
        };
        _mockInventoryService
            .Setup(s => s.GetInventoryByPartIdAsync("TEST001"))
            .ReturnsAsync(Result<List<PartInfo>>.Success(expectedParts));

        _viewModel.PartId = "TEST001";

        // Act
        await _viewModel.SearchCommand.ExecuteAsync(null);

        // Assert
        Assert.AreEqual(1, _viewModel.Parts.Count);
        Assert.AreEqual("TEST001", _viewModel.Parts[0].PartId);
        _mockInventoryService.Verify(s => s.GetInventoryByPartIdAsync("TEST001"), Times.Once);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _viewModel.Dispose();
    }
}
```

This comprehensive MVVM patterns documentation ensures consistent implementation across all ViewModels in the MTM WIP Application, leveraging the full power of MVVM Community Toolkit while maintaining manufacturing domain focus.


## 🚀 Joyride Development Enhancement

**Priority Directive**: Leverage Joyride automation for enhanced MTM development workflows.

### Core Joyride Tools
- **`joyride_evaluate_code`**: Execute ClojureScript in VS Code Extension Host
- **`joyride_request_human_input`**: Interactive decision-making workflows
- **`joyride_basics_for_agents`**: Access automation patterns and capabilities
- **`joyride_assisting_users_guide`**: User assistance automation

### MTM-Optimized Automation
- **Pattern Enforcement**: Automated MVVM Community Toolkit validation
- **Domain Validation**: Manufacturing operation codes and business rules
- **UI Consistency**: Avalonia theme system and component standards
- **Database Integration**: MySQL connection testing and stored procedure validation
- **Cross-Platform**: Automated testing across Windows/macOS/Linux

**Implementation**: Use Joyride first when safe, fall back to traditional tools as needed.