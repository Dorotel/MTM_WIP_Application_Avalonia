# CustomDataGrid - HTML Integration Guide

**Version**: 1.0  
**Created**: September 14, 2025  

---

## 📄 HTML Integration Guide

This document provides comprehensive guidance for integrating the CustomDataGrid control into parent views within the MTM WIP Application Avalonia architecture.

## Basic Integration Example

### Parent View Setup
```xml
<!-- InventoryManagementView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             xmlns:customControls="using:MTM_WIP_Application_Avalonia.Controls.CustomDataGrid"
             xmlns:overlay="using:MTM_WIP_Application_Avalonia.Views.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainForm.InventoryManagementView"
             x:DataType="vm:InventoryManagementViewModel">

  <Grid RowDefinitions="Auto,*,Auto">
    
    <!-- Toolbar Section -->
    <Border Grid.Row="0" 
            Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="0,0,0,1"
            Padding="16,8">
      
      <Grid ColumnDefinitions="*,Auto">
        <!-- Search and Filter Controls -->
        <StackPanel Grid.Column="0" Orientation="Horizontal" Spacing="8">
          <TextBox x:Name="SearchTextBox"
                   Text="{Binding SearchText}"
                   Watermark="Search inventory items..."
                   Width="250" />
          
          <Button Content="🔍 Search"
                  Command="{Binding SearchCommand}"
                  Classes="primary-action" />
          
          <Button Content="🔄 Refresh"
                  Command="{Binding RefreshDataCommand}"
                  Classes="secondary-action" />
        </StackPanel>
        
        <!-- Action Buttons -->
        <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="8">
          <Button Content="➕ Add Item"
                  Command="{Binding AddNewItemCommand}"
                  Classes="primary-action" />
          
          <Button Content="🗑️ Delete Selected"
                  Command="{Binding DeleteSelectedItemsCommand}"
                  IsEnabled="{Binding HasSelectedItems}"
                  Classes="warning-action" />
          
          <Button Content="📤 Export"
                  Command="{Binding ExportDataCommand}"
                  Classes="secondary-action" />
        </StackPanel>
      </Grid>
    </Border>

    <!-- CustomDataGrid Section -->
    <Border Grid.Row="1" 
            Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="1"
            CornerRadius="8"
            Margin="16">
      
      <customControls:CustomDataGrid
          x:Name="InventoryDataGrid"
          ItemsSource="{Binding InventoryItems}"
          IsMultiSelectEnabled="True"
          ReadNoteCommand="{Binding ReadNoteCommand}"
          DeleteItemCommand="{Binding DeleteItemCommand}"
          EditItemCommand="{Binding EditItemCommand}"
          DuplicateItemCommand="{Binding DuplicateItemCommand}"
          ViewDetailsCommand="{Binding ViewDetailsCommand}"
          SelectionChanged="OnDataGridSelectionChanged" />
    </Border>

    <!-- Status Bar Section -->
    <Border Grid.Row="2"
            Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="0,1,0,0"
            Padding="16,8">
      
      <Grid ColumnDefinitions="*,Auto,Auto">
        <!-- Status Message -->
        <TextBlock Grid.Column="0"
                   Text="{Binding StatusMessage}"
                   Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"
                   VerticalAlignment="Center" />
        
        <!-- Item Count -->
        <TextBlock Grid.Column="1"
                   Text="{Binding TotalItemCount, StringFormat='Total: {0:N0} items'}"
                   Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"
                   VerticalAlignment="Center"
                   Margin="16,0" />
        
        <!-- Selected Count -->
        <TextBlock Grid.Column="2"
                   Text="{Binding SelectedItemCount, StringFormat='Selected: {0:N0}'}"
                   Foreground="{DynamicResource MTM_Shared_Logic.InteractiveText}"
                   FontWeight="SemiBold"
                   VerticalAlignment="Center"
                   IsVisible="{Binding HasSelectedItems}" />
      </Grid>
    </Border>

    <!-- Overlay Layer -->
    <Grid Grid.RowSpan="3"
          IsVisible="{Binding ShowOverlays}"
          Background="rgba(0,0,0,0.3)">
      
      <!-- Confirmation Overlay -->
      <overlay:ConfirmationOverlayView
          x:Name="ConfirmationOverlay"
          IsVisible="{Binding ShowConfirmationOverlay}"
          DataContext="{Binding ConfirmationViewModel}" />
      
      <!-- Success Overlay -->
      <overlay:SuccessOverlayView
          x:Name="SuccessOverlay"
          IsVisible="{Binding ShowSuccessOverlay}"
          DataContext="{Binding SuccessViewModel}" />
    </Grid>

    <!-- Loading Overlay -->
    <Border Grid.RowSpan="3"
            Background="#80000000"
            IsVisible="{Binding IsLoading}"
            CornerRadius="8">
      
      <StackPanel HorizontalAlignment="Center"
                  VerticalAlignment="Center"
                  Spacing="16">
        
        <ProgressBar IsIndeterminate="True"
                     Width="200"
                     Height="4"
                     Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
        
        <TextBlock Text="{Binding LoadingMessage}"
                   Foreground="White"
                   FontSize="14"
                   HorizontalAlignment="Center" />
      </StackPanel>
    </Border>

  </Grid>
</UserControl>
```

### Parent Code-Behind
```csharp
// InventoryManagementView.axaml.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MTM_WIP_Application_Avalonia.Controls.CustomDataGrid;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Views.MainForm
{
    public partial class InventoryManagementView : UserControl
    {
        private readonly ILogger<InventoryManagementView> _logger;

        public InventoryManagementView()
        {
            InitializeComponent();
            _logger = App.ServiceProvider?.GetService<ILogger<InventoryManagementView>>() ?? 
                     NullLogger<InventoryManagementView>.Instance;
        }

        public InventoryManagementView(ILogger<InventoryManagementView> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeComponent();
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is InventoryManagementViewModel viewModel)
            {
                viewModel.HandleSelectionChanged(e.SelectedItems, e.SelectedCount);
                
                _logger.LogDebug("DataGrid selection changed: {SelectedCount} items selected", e.SelectedCount);
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            // Clean up any subscriptions or resources
            if (DataContext is InventoryManagementViewModel viewModel)
            {
                viewModel.Cleanup();
            }

            base.OnDetachedFromVisualTree(e);
        }
    }
}
```

## Advanced Integration Scenarios

### Tab-Based Integration
```xml
<!-- MainFormView.axaml with TabView integration -->
<TabView x:Name="MainTabView">
  
  <!-- Inventory Management Tab -->
  <TabViewItem Header="Inventory Management" Icon="Package">
    <views:InventoryManagementView DataContext="{Binding InventoryManagementViewModel}" />
  </TabViewItem>
  
  <!-- Transaction History Tab -->
  <TabViewItem Header="Transaction History" Icon="History">
    <Grid RowDefinitions="Auto,*">
      
      <!-- Filter Controls -->
      <Border Grid.Row="0" Classes="filter-panel">
        <StackPanel Orientation="Horizontal" Spacing="8">
          <DatePicker SelectedDate="{Binding StartDate}" />
          <DatePicker SelectedDate="{Binding EndDate}" />
          <ComboBox ItemsSource="{Binding TransactionTypes}"
                    SelectedItem="{Binding SelectedTransactionType}" />
          <Button Content="Apply Filters" Command="{Binding ApplyFiltersCommand}" />
        </StackPanel>
      </Border>
      
      <!-- Transaction Data Grid -->
      <customControls:CustomDataGrid
          Grid.Row="1"
          ItemsSource="{Binding TransactionHistory}"
          ReadNoteCommand="{Binding ViewTransactionDetailsCommand}"
          EditItemCommand="{Binding EditTransactionCommand}"
          DeleteItemCommand="{Binding DeleteTransactionCommand}"
          IsMultiSelectEnabled="False" />
    </Grid>
  </TabViewItem>
  
</TabView>
```

### Modal Dialog Integration
```xml
<!-- ItemDetailsDialog.axaml -->
<Window xmlns="https://github.com/avaloniaui"
        Title="Item Details"
        Width="800"
        Height="600"
        WindowStartupLocation="CenterOwner"
        ShowInTaskbar="False"
        CanResize="True">

  <Grid RowDefinitions="Auto,*,Auto">
    
    <!-- Header -->
    <Border Grid.Row="0" Classes="dialog-header">
      <TextBlock Text="{Binding DialogTitle}" Classes="dialog-title" />
    </Border>
    
    <!-- Content with Data Grid -->
    <ScrollViewer Grid.Row="1" Padding="16">
      <StackPanel Spacing="16">
        
        <!-- Item Information -->
        <Border Classes="info-panel">
          <Grid ColumnDefinitions="*,*" RowDefinitions="Auto,Auto,Auto,Auto" RowSpacing="8">
            <TextBlock Grid.Row="0" Grid.Column="0" Text="Part ID:" Classes="label" />
            <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding SelectedItem.PartId}" Classes="value" />
            
            <TextBlock Grid.Row="1" Grid.Column="0" Text="Operation:" Classes="label" />
            <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding SelectedItem.Operation}" Classes="value" />
            
            <TextBlock Grid.Row="2" Grid.Column="0" Text="Location:" Classes="label" />
            <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding SelectedItem.Location}" Classes="value" />
            
            <TextBlock Grid.Row="3" Grid.Column="0" Text="Quantity:" Classes="label" />
            <TextBlock Grid.Row="3" Grid.Column="1" Text="{Binding SelectedItem.Quantity, StringFormat=N0}" Classes="value" />
          </Grid>
        </Border>
        
        <!-- Related Transactions -->
        <Border Classes="data-panel">
          <StackPanel Spacing="8">
            <TextBlock Text="Related Transactions" Classes="section-header" />
            
            <customControls:CustomDataGrid
                ItemsSource="{Binding RelatedTransactions}"
                IsMultiSelectEnabled="False"
                ViewDetailsCommand="{Binding ViewTransactionCommand}"
                MaxHeight="300" />
          </StackPanel>
        </Border>
        
        <!-- Notes Section -->
        <Border Classes="notes-panel" IsVisible="{Binding HasNotes}">
          <StackPanel Spacing="8">
            <TextBlock Text="Notes" Classes="section-header" />
            <TextBox Text="{Binding Notes}"
                     TextWrapping="Wrap"
                     AcceptsReturn="True"
                     MinHeight="100"
                     IsReadOnly="{Binding IsNotesReadOnly}" />
          </StackPanel>
        </Border>
        
      </StackPanel>
    </ScrollViewer>
    
    <!-- Action Buttons -->
    <Border Grid.Row="2" Classes="dialog-footer">
      <StackPanel Orientation="Horizontal" 
                  HorizontalAlignment="Right" 
                  Spacing="8">
        
        <Button Content="Edit" 
                Command="{Binding EditItemCommand}"
                Classes="primary-action"
                IsVisible="{Binding CanEdit}" />
        
        <Button Content="Save Changes" 
                Command="{Binding SaveChangesCommand}"
                Classes="primary-action"
                IsVisible="{Binding HasUnsavedChanges}" />
        
        <Button Content="Close" 
                Command="{Binding CloseDialogCommand}"
                Classes="secondary-action" />
      </StackPanel>
    </Border>
    
  </Grid>
</Window>
```

## Integration with MTM Services

### Service Registration for Parent ViewModels
```csharp
// ServiceCollectionExtensions.cs - Add CustomDataGrid related services
public static IServiceCollection AddCustomDataGridServices(this IServiceCollection services)
{
    // Core services
    services.TryAddScoped<IInventoryService, InventoryService>();
    services.TryAddScoped<IConfirmationService, ConfirmationService>();
    services.TryAddScoped<ISuccessOverlayService, SuccessOverlayService>();
    
    // ViewModels that use CustomDataGrid
    services.TryAddTransient<InventoryManagementViewModel>();
    services.TryAddTransient<TransactionHistoryViewModel>();
    services.TryAddTransient<ItemDetailsViewModel>();
    
    // Overlay ViewModels
    services.TryAddTransient<ConfirmationOverlayViewModel>();
    services.TryAddTransient<SuccessOverlayViewModel>();
    
    return services;
}
```

### Parent ViewModel Base Pattern
```csharp
// Base class for ViewModels using CustomDataGrid
[ObservableObject]
public abstract partial class DataGridViewModelBase<T> : BaseViewModel where T : class, INotifyPropertyChanged
{
    protected readonly IConfirmationService _confirmationService;
    protected readonly ISuccessOverlayService _successOverlayService;

    [ObservableProperty]
    private ObservableCollection<T> _items = new();

    [ObservableProperty]
    private List<T> _selectedItems = new();

    [ObservableProperty]
    private int _selectedItemCount;

    [ObservableProperty]
    private int _totalItemCount;

    [ObservableProperty]
    private bool _hasSelectedItems;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _showConfirmationOverlay;

    [ObservableProperty]
    private bool _showSuccessOverlay;

    protected DataGridViewModelBase(
        ILogger logger,
        IConfirmationService confirmationService,
        ISuccessOverlayService successOverlayService)
        : base(logger)
    {
        _confirmationService = confirmationService ?? throw new ArgumentNullException(nameof(confirmationService));
        _successOverlayService = successOverlayService ?? throw new ArgumentNullException(nameof(successOverlayService));

        Items.CollectionChanged += OnItemsCollectionChanged;
    }

    public virtual void HandleSelectionChanged(IList<object> selectedItems, int selectedCount)
    {
        SelectedItems = selectedItems.OfType<T>().ToList();
        SelectedItemCount = selectedCount;
        HasSelectedItems = selectedCount > 0;
        
        OnSelectionChanged();
    }

    protected virtual void OnSelectionChanged()
    {
        // Override in derived classes for selection-specific logic
    }

    protected virtual void OnItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        TotalItemCount = Items.Count;
        StatusMessage = $"Loaded {TotalItemCount:N0} items";
    }

    protected virtual async Task<bool> ConfirmDeleteAsync(T item, string itemDescription)
    {
        var result = await _confirmationService.ShowDeleteConfirmationAsync(item, itemDescription);
        return result.IsConfirmed;
    }

    protected virtual async Task ShowSuccessAsync(string operation, T item)
    {
        await _successOverlayService.ShowOperationSuccessAsync(operation, item, item.ToString() ?? "Item");
    }

    public virtual void Cleanup()
    {
        Items.CollectionChanged -= OnItemsCollectionChanged;
    }
}
```

## Performance Optimization Integration

### Virtualization and Large Datasets
```csharp
// InventoryManagementViewModel with performance optimization
[ObservableObject]
public partial class InventoryManagementViewModel : DataGridViewModelBase<InventoryItem>
{
    private const int PageSize = 100;
    private int _currentPage = 1;
    private bool _isLoadingMore;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private bool _hasMoreData = true;

    public InventoryManagementViewModel(
        ILogger<InventoryManagementViewModel> logger,
        IInventoryService inventoryService,
        IConfirmationService confirmationService,
        ISuccessOverlayService successOverlayService)
        : base(logger, confirmationService, successOverlayService)
    {
        _inventoryService = inventoryService;
    }

    [RelayCommand]
    private async Task LoadInitialDataAsync()
    {
        try
        {
            IsLoading = true;
            _currentPage = 1;
            HasMoreData = true;

            var result = await _inventoryService.GetInventoryPagedAsync(_currentPage, PageSize, SearchText);

            if (result.IsSuccess)
            {
                Items.Clear();
                foreach (var item in result.Data.Items)
                {
                    Items.Add(item);
                }

                HasMoreData = result.Data.HasMorePages;
                StatusMessage = $"Loaded {Items.Count:N0} of {result.Data.TotalCount:N0} items";
            }
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Loading initial inventory data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadMoreDataAsync()
    {
        if (_isLoadingMore || !HasMoreData || IsLoading)
            return;

        try
        {
            _isLoadingMore = true;
            _currentPage++;

            var result = await _inventoryService.GetInventoryPagedAsync(_currentPage, PageSize, SearchText);

            if (result.IsSuccess)
            {
                foreach (var item in result.Data.Items)
                {
                    Items.Add(item);
                }

                HasMoreData = result.Data.HasMorePages;
                StatusMessage = $"Loaded {Items.Count:N0} of {result.Data.TotalCount:N0} items";
            }
        }
        catch (Exception ex)
        {
            _currentPage--; // Rollback on error
            await ErrorHandling.HandleErrorAsync(ex, "Loading more inventory data");
        }
        finally
        {
            _isLoadingMore = false;
        }
    }
}
```

### Infinite Scrolling Integration
```xml
<!-- CustomDataGrid with infinite scroll support -->
<customControls:CustomDataGrid
    ItemsSource="{Binding Items}"
    ScrolledToBottom="OnScrolledToBottom"
    LoadMoreCommand="{Binding LoadMoreDataCommand}"
    HasMoreData="{Binding HasMoreData}"
    IsLoadingMore="{Binding IsLoadingMore}" />
```

## Testing Integration Patterns

### Unit Testing Parent ViewModel
```csharp
// InventoryManagementViewModelTests.cs
public class InventoryManagementViewModelTests
{
    private readonly Mock<ILogger<InventoryManagementViewModel>> _mockLogger;
    private readonly Mock<IInventoryService> _mockInventoryService;
    private readonly Mock<IConfirmationService> _mockConfirmationService;
    private readonly Mock<ISuccessOverlayService> _mockSuccessOverlayService;
    private readonly InventoryManagementViewModel _viewModel;

    public InventoryManagementViewModelTests()
    {
        _mockLogger = new Mock<ILogger<InventoryManagementViewModel>>();
        _mockInventoryService = new Mock<IInventoryService>();
        _mockConfirmationService = new Mock<IConfirmationService>();
        _mockSuccessOverlayService = new Mock<ISuccessOverlayService>();

        _viewModel = new InventoryManagementViewModel(
            _mockLogger.Object,
            _mockInventoryService.Object,
            _mockConfirmationService.Object,
            _mockSuccessOverlayService.Object);
    }

    [Fact]
    public void HandleSelectionChanged_UpdatesSelectedItemCount()
    {
        // Arrange
        var items = new List<object> { new InventoryItem(), new InventoryItem() };

        // Act
        _viewModel.HandleSelectionChanged(items, 2);

        // Assert
        Assert.Equal(2, _viewModel.SelectedItemCount);
        Assert.True(_viewModel.HasSelectedItems);
    }

    [Fact]
    public async Task DeleteItemCommand_ShowsConfirmation_BeforeDeleting()
    {
        // Arrange
        var item = new InventoryItem { PartId = "TEST001" };
        
        _mockConfirmationService
            .Setup(x => x.ShowDeleteConfirmationAsync(It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(new ConfirmationResult { IsConfirmed = true });

        _mockInventoryService
            .Setup(x => x.DeleteInventoryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(ServiceResult.Success("Deleted"));

        // Act
        await _viewModel.DeleteItemCommand.ExecuteAsync(item);

        // Assert
        _mockConfirmationService.Verify(x => x.ShowDeleteConfirmationAsync(
            It.Is<object>(o => o == item),
            It.IsAny<string>()), Times.Once);
    }
}
```

---

**Implementation Complete**: All documentation files created successfully for the CustomDataGrid implementation.