---
description: 'Universal component implementation workflow for .NET 8 Avalonia applications using MVVM Community Toolkit patterns'
mode: 'prompt'
tools: ['codebase', 'editFiles', 'search', 'problems']
---

# Universal Component Implementation Workflow

You are an expert .NET 8 Avalonia application developer specializing in MVVM Community Toolkit patterns, service-oriented architecture, and modern UI development. Use this prompt template for implementing complete components (View + ViewModel + Service integration) in any .NET 8 Avalonia project.

## 📋 Component Analysis Phase

### 1. Requirements Gathering
```markdown
**Component Purpose**: [Brief description of what this component does]
**User Stories**: 
- As a [user type], I want to [action] so that [benefit]
- As a [user type], I need to [requirement] because [reason]

**Functional Requirements**:
- [ ] Primary functionality requirement
- [ ] Secondary functionality requirement  
- [ ] Integration requirements
- [ ] Performance requirements

**Non-Functional Requirements**:
- [ ] Responsive design (mobile/desktop)
- [ ] Theme integration
- [ ] Accessibility compliance
- [ ] Performance targets
```

### 2. Architecture Planning
```markdown
**Component Structure**:
- **View**: [ComponentName]View.axaml / .axaml.cs
- **ViewModel**: [ComponentName]ViewModel.cs
- **Model**: [ComponentName]Model.cs (if needed)
- **Service**: I[ComponentName]Service.cs / [ComponentName]Service.cs (if needed)
- **Tests**: [ComponentName]Tests.cs

**Dependencies**:
- Base classes: BaseViewModel, BaseService
- Services: INavigationService, IConfigurationService, etc.
- External packages: Material.Icons.Avalonia, etc.

**Integration Points**:
- Navigation: How component is accessed
- State Management: Shared state requirements
- Communication: Events, messaging, callbacks
```

## 🏗️ Implementation Standards

### MVVM Community Toolkit Patterns (MANDATORY)
```csharp
// ViewModel Implementation Template
[ObservableObject]
public partial class [ComponentName]ViewModel : BaseViewModel
{
    private readonly I[ComponentName]Service _service;
    private readonly INavigationService _navigationService;

    public [ComponentName]ViewModel(
        I[ComponentName]Service service,
        INavigationService navigationService,
        ILogger<[ComponentName]ViewModel> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(navigationService);
        
        _service = service;
        _navigationService = navigationService;
    }

    #region Observable Properties

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteAction))]
    private string inputText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExecuteAction))]
    private bool isLoading;

    [ObservableProperty]
    private ObservableCollection<DataItem> items = new();

    #endregion

    #region Computed Properties

    public bool CanExecuteAction => 
        !IsLoading && 
        !string.IsNullOrWhiteSpace(InputText);

    #endregion

    #region Commands

    [RelayCommand(CanExecute = nameof(CanExecuteAction))]
    private async Task ExecuteActionAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _service.PerformActionAsync(InputText);
            if (result.IsSuccess)
            {
                // Handle success
                Items.Clear();
                foreach (var item in result.Data ?? [])
                {
                    Items.Add(item);
                }
            }
            else
            {
                // Handle failure
                await HandleErrorAsync(
                    new InvalidOperationException(result.Message), 
                    "Execute action");
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Execute action");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateBackAsync()
    {
        await _navigationService.GoBackAsync();
    }

    #endregion

    #region Lifecycle

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _service.GetInitialDataAsync();
            if (result.IsSuccess && result.Data != null)
            {
                Items.Clear();
                foreach (var item in result.Data)
                {
                    Items.Add(item);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Load initial data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    #endregion
}
```

### Avalonia AXAML Implementation Template
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:YourProject.ViewModels"
             xmlns:converters="using:YourProject.Converters"
             mc:Ignorable="d"
             d:DesignWidth="800"
             d:DesignHeight="600"
             x:Class="YourProject.Views.[ComponentName]View"
             x:DataType="vm:[ComponentName]ViewModel">

    <UserControl.Resources>
        <converters:NullToBoolConverter x:Key="NullToBoolConverter"/>
    </UserControl.Resources>

    <!-- Universal layout pattern: ScrollViewer -> Grid with RowDefinitions -->
    <ScrollViewer>
        <Grid x:Name="MainGrid" RowDefinitions="*,Auto" Margin="16">
            
            <!-- Content Area -->
            <StackPanel Grid.Row="0" Spacing="16">
                
                <!-- Header Section -->
                <Border Background="{DynamicResource HeaderBackgroundBrush}"
                        Padding="16"
                        CornerRadius="4">
                    <TextBlock Text="[Component Title]"
                               FontSize="24"
                               FontWeight="Bold"
                               Foreground="{DynamicResource HeaderForegroundBrush}"/>
                </Border>

                <!-- Input Section -->
                <Border Background="{DynamicResource CardBackgroundBrush}"
                        Padding="16"
                        CornerRadius="4"
                        BorderBrush="{DynamicResource BorderBrush}"
                        BorderThickness="1">
                    
                    <Grid RowDefinitions="Auto,Auto,Auto" ColumnDefinitions="Auto,*" 
                          Spacing="12">
                        
                        <!-- Input Field -->
                        <TextBlock Grid.Row="0" Grid.Column="0" 
                                   Text="Input:" 
                                   VerticalAlignment="Center"/>
                        <TextBox Grid.Row="0" Grid.Column="1"
                                 Text="{Binding InputText}"
                                 Watermark="Enter text here..."
                                 IsEnabled="{Binding !IsLoading}"/>

                        <!-- Action Button -->
                        <Button Grid.Row="1" Grid.Column="1"
                                Content="Execute Action"
                                Command="{Binding ExecuteActionCommand}"
                                HorizontalAlignment="Left"
                                Margin="0,8,0,0"/>

                        <!-- Loading Indicator -->
                        <ProgressBar Grid.Row="2" Grid.Column="1"
                                     IsIndeterminate="True"
                                     IsVisible="{Binding IsLoading}"
                                     Margin="0,8,0,0"/>
                        
                    </Grid>
                </Border>

                <!-- Results Section -->
                <Border Background="{DynamicResource CardBackgroundBrush}"
                        Padding="16"
                        CornerRadius="4"
                        BorderBrush="{DynamicResource BorderBrush}"
                        BorderThickness="1"
                        IsVisible="{Binding Items.Count, Converter={StaticResource NullToBoolConverter}}">
                    
                    <StackPanel Spacing="8">
                        <TextBlock Text="Results:" 
                                   FontWeight="Bold" 
                                   FontSize="16"/>
                        
                        <ListBox ItemsSource="{Binding Items}"
                                 MaxHeight="200">
                            <ListBox.ItemTemplate>
                                <DataTemplate>
                                    <TextBlock Text="{Binding DisplayText}"/>
                                </DataTemplate>
                            </ListBox.ItemTemplate>
                        </ListBox>
                    </StackPanel>
                </Border>

            </StackPanel>

            <!-- Action Buttons -->
            <StackPanel Grid.Row="1" 
                        Orientation="Horizontal" 
                        HorizontalAlignment="Right" 
                        Spacing="8"
                        Margin="0,16,0,0">
                
                <Button Content="Back" 
                        Command="{Binding NavigateBackCommand}"
                        Classes="secondary"/>
                
                <!-- Add more action buttons as needed -->
                
            </StackPanel>

        </Grid>
    </ScrollViewer>

</UserControl>
```

### Service Implementation Template
```csharp
// Service Interface
public interface I[ComponentName]Service
{
    Task<ServiceResult<List<DataItem>>> GetInitialDataAsync();
    Task<ServiceResult<List<DataItem>>> PerformActionAsync(string input);
    Task<ServiceResult> SaveDataAsync(DataItem item);
}

// Service Implementation
public class [ComponentName]Service : I[ComponentName]Service
{
    private readonly IDataService _dataService;
    private readonly ILogger<[ComponentName]Service> _logger;

    public [ComponentName]Service(
        IDataService dataService,
        ILogger<[ComponentName]Service> logger)
    {
        ArgumentNullException.ThrowIfNull(dataService);
        ArgumentNullException.ThrowIfNull(logger);
        
        _dataService = dataService;
        _logger = logger;
    }

    public async Task<ServiceResult<List<DataItem>>> GetInitialDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading initial data for [ComponentName]");
            
            var result = await _dataService.GetAllAsync<DataItem>();
            if (result.IsSuccess)
            {
                _logger.LogInformation("Loaded {Count} items successfully", result.Data?.Count ?? 0);
                return ServiceResult<List<DataItem>>.Success(result.Data ?? new List<DataItem>());
            }
            
            return ServiceResult<List<DataItem>>.Failure("Failed to load initial data");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading initial data");
            return ServiceResult<List<DataItem>>.Failure("Error loading data", ex);
        }
    }

    public async Task<ServiceResult<List<DataItem>>> PerformActionAsync(string input)
    {
        try
        {
            _logger.LogInformation("Performing action with input: {Input}", input);
            
            // TODO: Implement your business logic here
            var processedData = ProcessInput(input);
            
            return ServiceResult<List<DataItem>>.Success(processedData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing action with input: {Input}", input);
            return ServiceResult<List<DataItem>>.Failure("Action failed", ex);
        }
    }

    public async Task<ServiceResult> SaveDataAsync(DataItem item)
    {
        try
        {
            _logger.LogInformation("Saving data item: {ItemId}", item.Id);
            
            return await _dataService.SaveAsync(item.Id, item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving data item: {ItemId}", item.Id);
            return ServiceResult.Failure("Save failed", ex);
        }
    }

    private List<DataItem> ProcessInput(string input)
    {
        // TODO: Implement your processing logic
        return new List<DataItem>
        {
            new DataItem { Id = Guid.NewGuid().ToString(), DisplayText = $"Processed: {input}" }
        };
    }
}
```

## 🧪 Testing Implementation

### Unit Test Template
```csharp
public class [ComponentName]ViewModelTests
{
    private readonly Mock<I[ComponentName]Service> _mockService;
    private readonly Mock<INavigationService> _mockNavigationService;
    private readonly Mock<ILogger<[ComponentName]ViewModel>> _mockLogger;
    private readonly [ComponentName]ViewModel _viewModel;

    public [ComponentName]ViewModelTests()
    {
        _mockService = new Mock<I[ComponentName]Service>();
        _mockNavigationService = new Mock<INavigationService>();
        _mockLogger = new Mock<ILogger<[ComponentName]ViewModel>>();
        
        _viewModel = new [ComponentName]ViewModel(
            _mockService.Object,
            _mockNavigationService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public void CanExecuteAction_WithValidInput_ReturnsTrue()
    {
        // Arrange
        _viewModel.InputText = "Valid input";
        _viewModel.IsLoading = false;

        // Act & Assert
        Assert.True(_viewModel.CanExecuteAction);
    }

    [Fact]
    public void CanExecuteAction_WithEmptyInput_ReturnsFalse()
    {
        // Arrange
        _viewModel.InputText = "";
        _viewModel.IsLoading = false;

        // Act & Assert
        Assert.False(_viewModel.CanExecuteAction);
    }

    [Fact]
    public async Task ExecuteActionAsync_WithValidInput_CallsService()
    {
        // Arrange
        var expectedData = new List<DataItem> { new DataItem { DisplayText = "Test" } };
        _mockService.Setup(s => s.PerformActionAsync(It.IsAny<string>()))
                   .ReturnsAsync(ServiceResult<List<DataItem>>.Success(expectedData));
        
        _viewModel.InputText = "Test input";

        // Act
        await _viewModel.ExecuteActionCommand.ExecuteAsync(null);

        // Assert
        _mockService.Verify(s => s.PerformActionAsync("Test input"), Times.Once);
        Assert.Single(_viewModel.Items);
        Assert.Equal("Test", _viewModel.Items[0].DisplayText);
    }
}
```

## 🔧 Integration Checklist

### Service Registration
```csharp
// Add to ServiceCollectionExtensions.cs
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    // Register new service
    services.TryAddScoped<I[ComponentName]Service, [ComponentName]Service>();
    
    // Register ViewModel
    services.TryAddTransient<[ComponentName]ViewModel>();
    
    return services;
}
```

### Navigation Integration
```csharp
// Add navigation method to NavigationService
public async Task NavigateTo[ComponentName]Async()
{
    await NavigateToAsync<[ComponentName]View, [ComponentName]ViewModel>();
}
```

### Theme Integration
```markdown
**Required Theme Resources**:
- `HeaderBackgroundBrush`
- `HeaderForegroundBrush`
- `CardBackgroundBrush`
- `BorderBrush`
- `ForegroundBrush`

**Responsive Design**:
- Minimum width: 320px (mobile)
- Optimal width: 800px (desktop)
- Scalable font sizes and spacing
```

## 📋 Implementation Checklist

- [ ] **Requirements Analysis**
  - [ ] Functional requirements documented
  - [ ] User stories defined
  - [ ] Architecture planned

- [ ] **ViewModel Implementation**
  - [ ] BaseViewModel inheritance
  - [ ] [ObservableProperty] for all bindable properties
  - [ ] [RelayCommand] for all commands
  - [ ] Proper async/await patterns
  - [ ] Error handling integration

- [ ] **View Implementation**
  - [ ] Avalonia namespace and syntax compliance
  - [ ] Universal layout pattern (ScrollViewer -> Grid)
  - [ ] Theme-aware styling with DynamicResource
  - [ ] Proper data binding with x:DataType

- [ ] **Service Implementation**
  - [ ] Interface definition
  - [ ] ServiceResult pattern usage
  - [ ] Structured logging
  - [ ] Error handling

- [ ] **Testing**
  - [ ] Unit tests for ViewModel
  - [ ] Service tests with mocking
  - [ ] Integration tests if needed

- [ ] **Integration**
  - [ ] Service registration
  - [ ] Navigation integration
  - [ ] Theme resource verification

- [ ] **Quality Assurance**
  - [ ] Code review compliance
  - [ ] Performance validation
  - [ ] Accessibility verification
  - [ ] Documentation updates

## 🚀 Next Steps

After implementing the component:

1. **Test thoroughly** across different screen sizes and themes
2. **Update documentation** with usage examples
3. **Add to component library** for reuse
4. **Monitor performance** and optimize if needed
5. **Gather user feedback** and iterate

---

*This template provides a complete workflow for implementing universal components in .NET 8 Avalonia applications following MVVM Community Toolkit patterns and modern architectural practices.*