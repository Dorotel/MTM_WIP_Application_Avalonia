# MTM UI Behavior Patterns

## 📋 Overview

This document establishes consistent behavior patterns for the MTM WIP Application interface, ensuring predictable user interactions, smooth transitions, and optimized workflows for manufacturing environments. These patterns create a cohesive user experience that reduces cognitive load and improves operational efficiency.

## 🔄 **Core Interaction Patterns**

### **1. Loading and State Management**

#### **Loading States Pattern**

```xml
<!-- Standard Loading State Implementation -->
<Grid>
    <!-- Content Layer -->
    <Border x:Name="ContentLayer"
            IsVisible="{Binding !IsLoading}">
        <!-- Main content goes here -->
    </Border>
    
    <!-- Loading Overlay -->
    <Border x:Name="LoadingOverlay"
            Background="#80FFFFFF"
            IsVisible="{Binding IsLoading}">
        <StackPanel HorizontalAlignment="Center"
                   VerticalAlignment="Center"
                   Spacing="16">
            
            <!-- Loading Indicator -->
            <Border Width="40" Height="40"
                   Background="{DynamicResource MTM_Primary}"
                   CornerRadius="20">
                <Border.RenderTransform>
                    <RotateTransform x:Name="SpinTransform" />
                </Border.RenderTransform>
                <Border.Styles>
                    <Style Selector="Border">
                        <Style.Animations>
                            <Animation Duration="0:0:1" IterationCount="Infinite">
                                <KeyFrame Cue="0%">
                                    <Setter Property="RenderTransform.Angle" Value="0" />
                                </KeyFrame>
                                <KeyFrame Cue="100%">
                                    <Setter Property="RenderTransform.Angle" Value="360" />
                                </KeyFrame>
                            </Animation>
                        </Style.Animations>
                    </Style>
                </Border.Styles>
            </Border>
            
            <!-- Loading Text -->
            <TextBlock Text="{Binding LoadingMessage, FallbackValue='Loading...'}"
                       FontSize="{DynamicResource MTM_FontSize_Body}"
                       HorizontalAlignment="Center"
                       Foreground="{DynamicResource MTM_TextPrimary}" />
        </StackPanel>
    </Border>
</Grid>
```

```csharp
// Loading State Management in ViewModels
public partial class InventoryViewModel : BaseViewModel
{
    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string loadingMessage = "Loading inventory data...";

    [RelayCommand]
    private async Task LoadInventoryAsync()
    {
        try
        {
            IsLoading = true;
            LoadingMessage = "Fetching inventory data...";
            
            // Simulate minimum loading time for UX consistency
            var dataTask = _inventoryService.GetInventoryAsync();
            var minimumDelay = Task.Delay(500); // Minimum 500ms for perceived reliability
            
            await Task.WhenAll(dataTask, minimumDelay);
            
            var inventoryData = await dataTask;
            ProcessInventoryData(inventoryData);
        }
        catch (Exception ex)
        {
            await ErrorHandling.HandleErrorAsync(ex, "Failed to load inventory data");
        }
        finally
        {
            IsLoading = false;
            LoadingMessage = string.Empty;
        }
    }
}
```

#### **Progressive Loading Pattern**

```csharp
// Progressive data loading for large datasets
public class ProgressiveLoadingService
{
    private const int BATCH_SIZE = 50;
    
    public async Task<ObservableCollection<InventoryItem>> LoadInventoryProgressively(
        Action<int, int> progressCallback = null)
    {
        var allItems = new ObservableCollection<InventoryItem>();
        var totalCount = await GetInventoryCountAsync();
        var loadedCount = 0;

        for (int offset = 0; offset < totalCount; offset += BATCH_SIZE)
        {
            var batch = await GetInventoryBatchAsync(offset, BATCH_SIZE);
            
            foreach (var item in batch)
            {
                allItems.Add(item);
                loadedCount++;
                
                // Update progress
                progressCallback?.Invoke(loadedCount, totalCount);
                
                // Brief pause to allow UI updates
                if (loadedCount % 10 == 0)
                {
                    await Task.Delay(1);
                }
            }
        }

        return allItems;
    }
}
```

### **2. Form Validation Patterns**

#### **Real-time Validation Implementation**

```xml
<!-- Validation-aware Input Control -->
<StackPanel Spacing="4">
    <TextBox x:Name="PartIdInput"
             Text="{Binding PartId, Mode=TwoWay}"
             Classes="mtm-textbox"
             Classes.has-error="{Binding HasPartIdError}"
             Classes.is-valid="{Binding IsPartIdValid}" />
    
    <!-- Validation Message -->
    <TextBlock Text="{Binding PartIdValidationMessage}"
               IsVisible="{Binding HasPartIdValidationMessage}"
               Classes="validation-message"
               Classes.error="{Binding HasPartIdError}"
               Classes.success="{Binding IsPartIdValid}" />
</StackPanel>

<!-- Validation Message Styles -->
<Style Selector="TextBlock.validation-message">
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Small}" />
    <Setter Property="Margin" Value="0,2,0,0" />
    <Setter Property="TextWrapping" Value="Wrap" />
</Style>

<Style Selector="TextBlock.validation-message.error">
    <Setter Property="Foreground" Value="{DynamicResource MTM_Danger}" />
</Style>

<Style Selector="TextBox.has-error">
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Danger}" />
    <Setter Property="BorderThickness" Value="2" />
</Style>

<Style Selector="TextBox.is-valid">
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Success}" />
</Style>
```

```csharp
// Real-time Validation ViewModel Pattern
public partial class TransactionFormViewModel : BaseViewModel
{
    [ObservableProperty]
    private string partId = string.Empty;

    [ObservableProperty]
    private string partIdValidationMessage = string.Empty;

    [ObservableProperty]
    private bool hasPartIdError;

    [ObservableProperty]
    private bool isPartIdValid;

    private readonly Timer _validationTimer;

    public TransactionFormViewModel()
    {
        // Debounced validation - validate after user stops typing
        _validationTimer = new Timer(ValidatePartId, null, Timeout.Infinite, Timeout.Infinite);
    }

    partial void OnPartIdChanged(string value)
    {
        // Reset validation state
        HasPartIdError = false;
        IsPartIdValid = false;
        PartIdValidationMessage = string.Empty;

        // Debounce validation by 500ms
        _validationTimer.Change(500, Timeout.Infinite);
    }

    private void ValidatePartId(object state)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var result = ValidatePartIdSync(PartId);
            
            HasPartIdError = !result.IsValid;
            IsPartIdValid = result.IsValid && !string.IsNullOrEmpty(PartId);
            PartIdValidationMessage = result.Message;
        });
    }

    private ValidationResult ValidatePartIdSync(string partId)
    {
        if (string.IsNullOrWhiteSpace(partId))
        {
            return new ValidationResult { IsValid = false, Message = "Part ID is required" };
        }

        if (!Regex.IsMatch(partId, @"^[A-Z]{2,4}\d{3,6}$"))
        {
            return new ValidationResult 
            { 
                IsValid = false, 
                Message = "Part ID must be 2-4 letters followed by 3-6 digits (e.g., PART001)" 
            };
        }

        return new ValidationResult { IsValid = true, Message = "Valid part ID format" };
    }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
```

### **3. Navigation and Workflow Patterns**

#### **Breadcrumb Navigation**

```xml
<!-- Breadcrumb Navigation Component -->
<Border Classes="mtm-breadcrumb-container">
    <ItemsControl ItemsSource="{Binding BreadcrumbItems}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel Orientation="Horizontal" Spacing="8" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <StackPanel Orientation="Horizontal" Spacing="8">
                    <!-- Breadcrumb Item -->
                    <Button Content="{Binding Title}"
                            Command="{Binding NavigateCommand}"
                            Classes="mtm-breadcrumb-item"
                            IsEnabled="{Binding IsNavigable}" />
                    
                    <!-- Separator -->
                    <TextBlock Text=">"
                               IsVisible="{Binding !IsLast}"
                               VerticalAlignment="Center"
                               Foreground="{DynamicResource MTM_TextSecondary}" />
                </StackPanel>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</Border>
```

```csharp
// Breadcrumb Navigation Service
public class BreadcrumbNavigationService : IBreadcrumbNavigationService
{
    private readonly ObservableCollection<BreadcrumbItem> _breadcrumbs = new();
    
    public ReadOnlyObservableCollection<BreadcrumbItem> Breadcrumbs { get; }

    public BreadcrumbNavigationService()
    {
        Breadcrumbs = new ReadOnlyObservableCollection<BreadcrumbItem>(_breadcrumbs);
    }

    public void NavigateTo(string title, string route, object parameters = null)
    {
        // Remove any breadcrumbs after the current position
        var existingIndex = _breadcrumbs.ToList().FindIndex(b => b.Route == route);
        
        if (existingIndex >= 0)
        {
            // Remove all items after this one
            for (int i = _breadcrumbs.Count - 1; i > existingIndex; i--)
            {
                _breadcrumbs.RemoveAt(i);
            }
        }
        else
        {
            // Add new breadcrumb
            var breadcrumb = new BreadcrumbItem
            {
                Title = title,
                Route = route,
                Parameters = parameters,
                NavigateCommand = new RelayCommand(() => NavigateToRoute(route, parameters))
            };
            
            _breadcrumbs.Add(breadcrumb);
        }

        UpdateLastItemFlags();
    }

    private void UpdateLastItemFlags()
    {
        for (int i = 0; i < _breadcrumbs.Count; i++)
        {
            _breadcrumbs[i].IsLast = (i == _breadcrumbs.Count - 1);
            _breadcrumbs[i].IsNavigable = !_breadcrumbs[i].IsLast;
        }
    }
}

public class BreadcrumbItem : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string route = string.Empty;

    [ObservableProperty]
    private bool isLast;

    [ObservableProperty]
    private bool isNavigable = true;

    public object Parameters { get; set; }
    public ICommand NavigateCommand { get; set; }
}
```

#### **Multi-step Workflow Pattern**

```xml
<!-- Step Indicator Component -->
<Border Classes="mtm-step-indicator">
    <ItemsControl ItemsSource="{Binding WorkflowSteps}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel Orientation="Horizontal" Spacing="16" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <StackPanel Spacing="8" HorizontalAlignment="Center">
                    <!-- Step Circle -->
                    <Border Width="32" Height="32"
                            CornerRadius="16"
                            Classes="step-indicator"
                            Classes.completed="{Binding IsCompleted}"
                            Classes.current="{Binding IsCurrent}"
                            Classes.pending="{Binding IsPending}">
                        
                        <!-- Step Number or Check -->
                        <TextBlock HorizontalAlignment="Center"
                                  VerticalAlignment="Center"
                                  FontWeight="Bold"
                                  FontSize="14">
                            <TextBlock.Text>
                                <MultiBinding Converter="{StaticResource StepDisplayConverter}">
                                    <Binding Path="StepNumber" />
                                    <Binding Path="IsCompleted" />
                                </MultiBinding>
                            </TextBlock.Text>
                        </TextBlock>
                    </Border>
                    
                    <!-- Step Title -->
                    <TextBlock Text="{Binding Title}"
                               HorizontalAlignment="Center"
                               FontSize="{DynamicResource MTM_FontSize_Small}"
                               Classes="step-title"
                               Classes.completed="{Binding IsCompleted}"
                               Classes.current="{Binding IsCurrent}" />
                </StackPanel>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</Border>

<!-- Step Content Area -->
<ContentPresenter Content="{Binding CurrentStepContent}"
                  ContentTemplate="{Binding CurrentStepTemplate}" />

<!-- Navigation Controls -->
<StackPanel Orientation="Horizontal" 
           HorizontalAlignment="Right"
           Spacing="12">
    <Button Content="Previous"
           Command="{Binding PreviousStepCommand}"
           IsVisible="{Binding CanGoPrevious}"
           Classes="mtm-button-secondary" />
    
    <Button Content="{Binding NextButtonText}"
           Command="{Binding NextStepCommand}"
           IsEnabled="{Binding CanGoNext}"
           Classes="mtm-button-primary" />
</StackPanel>
```

```csharp
// Multi-step Workflow ViewModel
public partial class WorkflowViewModel : BaseViewModel
{
    private readonly List<WorkflowStep> _steps;
    private int _currentStepIndex;

    [ObservableProperty]
    private ObservableCollection<WorkflowStepViewModel> workflowSteps = new();

    [ObservableProperty]
    private object currentStepContent;

    [ObservableProperty]
    private DataTemplate currentStepTemplate;

    [ObservableProperty]
    private bool canGoPrevious;

    [ObservableProperty]
    private bool canGoNext;

    [ObservableProperty]
    private string nextButtonText = "Next";

    public WorkflowViewModel(IEnumerable<WorkflowStep> steps)
    {
        _steps = steps.ToList();
        InitializeSteps();
        NavigateToStep(0);
    }

    private void InitializeSteps()
    {
        WorkflowSteps.Clear();
        for (int i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            WorkflowSteps.Add(new WorkflowStepViewModel
            {
                StepNumber = i + 1,
                Title = step.Title,
                IsPending = true
            });
        }
    }

    [RelayCommand]
    private async Task NextStepAsync()
    {
        if (_currentStepIndex < _steps.Count - 1)
        {
            // Validate current step
            var currentStep = _steps[_currentStepIndex];
            var validationResult = await currentStep.ValidateAsync();
            
            if (!validationResult.IsValid)
            {
                await ShowValidationErrorsAsync(validationResult.Errors);
                return;
            }

            // Mark current step as completed
            WorkflowSteps[_currentStepIndex].IsCompleted = true;
            WorkflowSteps[_currentStepIndex].IsCurrent = false;

            // Move to next step
            _currentStepIndex++;
            NavigateToStep(_currentStepIndex);
        }
        else
        {
            // Final step - complete workflow
            await CompleteWorkflowAsync();
        }
    }

    [RelayCommand]
    private void PreviousStep()
    {
        if (_currentStepIndex > 0)
        {
            // Mark current step as pending
            WorkflowSteps[_currentStepIndex].IsPending = true;
            WorkflowSteps[_currentStepIndex].IsCurrent = false;

            // Move to previous step
            _currentStepIndex--;
            NavigateToStep(_currentStepIndex);
        }
    }

    private void NavigateToStep(int stepIndex)
    {
        var step = _steps[stepIndex];
        
        // Update step states
        for (int i = 0; i < WorkflowSteps.Count; i++)
        {
            var stepViewModel = WorkflowSteps[i];
            stepViewModel.IsCurrent = (i == stepIndex);
            
            if (i < stepIndex)
            {
                stepViewModel.IsCompleted = true;
                stepViewModel.IsPending = false;
            }
            else if (i > stepIndex)
            {
                stepViewModel.IsCompleted = false;
                stepViewModel.IsPending = true;
            }
        }

        // Update content
        CurrentStepContent = step.ContentViewModel;
        CurrentStepTemplate = step.ContentTemplate;

        // Update navigation state
        CanGoPrevious = stepIndex > 0;
        CanGoNext = true; // Validation happens in NextStepAsync
        NextButtonText = (stepIndex == _steps.Count - 1) ? "Complete" : "Next";
    }
}
```

## 🎯 **Data Entry Patterns**

### **Auto-complete and Suggestion Patterns**

```xml
<!-- Auto-complete ComboBox with Suggestions -->
<ComboBox x:Name="PartIdComboBox"
          Text="{Binding PartIdInput, Mode=TwoWay}"
          ItemsSource="{Binding PartIdSuggestions}"
          IsTextSearchEnabled="True"
          IsEditable="True"
          Classes="mtm-combobox-autocomplete">
    
    <ComboBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <TextBlock Text="{Binding PartId}"
                          FontFamily="{DynamicResource MTM_FontFamilyMono}"
                          FontWeight="Bold" />
                <TextBlock Text="-" 
                          Foreground="{DynamicResource MTM_TextSecondary}" />
                <TextBlock Text="{Binding Description}"
                          Foreground="{DynamicResource MTM_TextSecondary}" />
            </StackPanel>
        </DataTemplate>
    </ComboBox.ItemTemplate>
</ComboBox>
```

```csharp
// Auto-complete Service Implementation
public class AutoCompleteService : IAutoCompleteService
{
    private readonly IPartRepository _partRepository;
    private readonly Timer _suggestionTimer;

    public AutoCompleteService(IPartRepository partRepository)
    {
        _partRepository = partRepository;
        _suggestionTimer = new Timer(UpdateSuggestions, null, Timeout.Infinite, Timeout.Infinite);
    }

    public async Task<ObservableCollection<PartSuggestion>> GetSuggestionsAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || input.Length < 2)
        {
            return new ObservableCollection<PartSuggestion>();
        }

        // Fuzzy matching with scoring
        var allParts = await _partRepository.GetAllPartsAsync();
        var scored = allParts
            .Select(part => new
            {
                Part = part,
                Score = CalculateMatchScore(input, part.PartId, part.Description)
            })
            .Where(item => item.Score > 0.3) // Threshold for relevance
            .OrderByDescending(item => item.Score)
            .Take(10) // Limit suggestions
            .Select(item => new PartSuggestion
            {
                PartId = item.Part.PartId,
                Description = item.Part.Description,
                Score = item.Score
            });

        return new ObservableCollection<PartSuggestion>(scored);
    }

    private double CalculateMatchScore(string input, string partId, string description)
    {
        var inputLower = input.ToLowerInvariant();
        var partIdLower = partId.ToLowerInvariant();
        var descLower = description.ToLowerInvariant();

        // Exact prefix match gets highest score
        if (partIdLower.StartsWith(inputLower))
            return 1.0;

        // Contains match in part ID
        if (partIdLower.Contains(inputLower))
            return 0.8;

        // Contains match in description
        if (descLower.Contains(inputLower))
            return 0.6;

        // Fuzzy matching using Levenshtein distance
        var partIdDistance = LevenshteinDistance(inputLower, partIdLower);
        var descDistance = LevenshteinDistance(inputLower, descLower);
        
        var bestDistance = Math.Min(partIdDistance, descDistance);
        var maxLength = Math.Max(inputLower.Length, Math.Max(partIdLower.Length, descLower.Length));
        
        return Math.Max(0, 1.0 - (double)bestDistance / maxLength);
    }
}
```

### **Bulk Data Entry Pattern**

```xml
<!-- Bulk Entry Data Grid -->
<DataGrid x:Name="BulkEntryGrid"
          ItemsSource="{Binding BulkEntryItems}"
          CanUserAddRows="True"
          CanUserDeleteRows="True"
          Classes="mtm-datagrid-bulk-entry">
    
    <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID"
                           Binding="{Binding PartId}"
                           Width="150"
                           IsReadOnly="False" />
        
        <DataGridTextColumn Header="Operation"
                           Binding="{Binding Operation}"
                           Width="100"
                           IsReadOnly="False" />
        
        <DataGridTextColumn Header="Quantity"
                           Binding="{Binding Quantity}"
                           Width="100"
                           IsReadOnly="False" />
        
        <DataGridComboBoxColumn Header="Location"
                               ItemsSource="{Binding DataContext.AvailableLocations, 
                                           RelativeSource={RelativeSource AncestorType=DataGrid}}"
                               SelectedItemBinding="{Binding Location}"
                               Width="150" />
        
        <DataGridTemplateColumn Header="Status" Width="100">
            <DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <Border Classes="status-indicator"
                           Classes.valid="{Binding IsValid}"
                           Classes.invalid="{Binding HasErrors}">
                        <TextBlock Text="{Binding ValidationStatus}"
                                  HorizontalAlignment="Center" />
                    </Border>
                </DataTemplate>
            </DataGridTemplateColumn.CellTemplate>
        </DataGridTemplateColumn>
    </DataGrid.Columns>
</DataGrid>

<!-- Bulk Entry Actions -->
<StackPanel Orientation="Horizontal" Spacing="12" Margin="0,16,0,0">
    <Button Content="Import from CSV"
           Command="{Binding ImportCsvCommand}"
           Classes="mtm-button-secondary" />
    
    <Button Content="Validate All"
           Command="{Binding ValidateAllCommand}"
           Classes="mtm-button-secondary" />
    
    <Button Content="Process All"
           Command="{Binding ProcessAllCommand}"
           IsEnabled="{Binding AllItemsValid}"
           Classes="mtm-button-primary" />
</StackPanel>
```

## 📱 **Responsive Behavior Patterns**

### **Adaptive Layout System**

```csharp
// Responsive Layout Service
public class ResponsiveLayoutService : IResponsiveLayoutService
{
    public BreakpointSize CurrentBreakpoint { get; private set; }
    
    public event EventHandler<BreakpointChangedEventArgs> BreakpointChanged;

    public void UpdateBreakpoint(Size windowSize)
    {
        var newBreakpoint = CalculateBreakpoint(windowSize.Width);
        
        if (newBreakpoint != CurrentBreakpoint)
        {
            var previousBreakpoint = CurrentBreakpoint;
            CurrentBreakpoint = newBreakpoint;
            
            BreakpointChanged?.Invoke(this, new BreakpointChangedEventArgs
            {
                Previous = previousBreakpoint,
                Current = newBreakpoint,
                WindowSize = windowSize
            });
        }
    }

    private BreakpointSize CalculateBreakpoint(double width)
    {
        return width switch
        {
            < 600 => BreakpointSize.Mobile,
            < 900 => BreakpointSize.Tablet,
            < 1200 => BreakpointSize.Desktop,
            _ => BreakpointSize.Large
        };
    }

    public GridLength GetColumnWidth(string columnName, BreakpointSize breakpoint)
    {
        return (columnName, breakpoint) switch
        {
            ("Sidebar", BreakpointSize.Mobile) => new GridLength(0),
            ("Sidebar", BreakpointSize.Tablet) => new GridLength(250),
            ("Sidebar", _) => new GridLength(300),
            ("Content", BreakpointSize.Mobile) => new GridLength(1, GridUnitType.Star),
            ("Content", _) => new GridLength(1, GridUnitType.Star),
            _ => new GridLength(1, GridUnitType.Star)
        };
    }
}

public enum BreakpointSize
{
    Mobile,
    Tablet,
    Desktop,
    Large
}
```

### **Touch-Friendly Adaptations**

```xml
<!-- Touch-adaptive Button Sizing -->
<Button Content="Save Transaction"
        Classes="mtm-button-primary touch-friendly">
    
    <Button.Styles>
        <!-- Desktop sizing -->
        <Style Selector="Button.mtm-button-primary">
            <Setter Property="MinHeight" Value="32" />
            <Setter Property="Padding" Value="16,8" />
        </Style>
        
        <!-- Touch-friendly sizing -->
        <Style Selector="Button.touch-friendly">
            <Setter Property="MinHeight" Value="44" />
            <Setter Property="Padding" Value="20,12" />
            <Setter Property="Margin" Value="4" />
        </Style>
    </Button.Styles>
</Button>
```

## 🎨 **Animation and Transition Patterns**

### **Smooth State Transitions**

```xml
<!-- Animated State Changes -->
<Border x:Name="StatusCard"
        Classes="status-card"
        Classes.success="{Binding IsSuccessState}"
        Classes.error="{Binding IsErrorState}"
        Classes.loading="{Binding IsLoadingState}">
    
    <Border.Styles>
        <!-- Base state -->
        <Style Selector="Border.status-card">
            <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
            <Setter Property="Opacity" Value="1" />
            <Setter Property="RenderTransform" Value="scaleX(1) scaleY(1)" />
            <Setter Property="Transitions">
                <Transitions>
                    <DoubleTransition Property="Opacity" Duration="0:0:0.3" />
                    <TransformOperationsTransition Property="RenderTransform" Duration="0:0:0.3" />
                    <BrushTransition Property="Background" Duration="0:0:0.3" />
                </Transitions>
            </Setter>
        </Style>
        
        <!-- Success state -->
        <Style Selector="Border.status-card.success">
            <Setter Property="Background" Value="{DynamicResource MTM_Success}" />
            <Style.Animations>
                <Animation Duration="0:0:0.6" FillMode="Forward">
                    <KeyFrame Cue="0%">
                        <Setter Property="RenderTransform" Value="scaleX(1) scaleY(1)" />
                    </KeyFrame>
                    <KeyFrame Cue="50%">
                        <Setter Property="RenderTransform" Value="scaleX(1.05) scaleY(1.05)" />
                    </KeyFrame>
                    <KeyFrame Cue="100%">
                        <Setter Property="RenderTransform" Value="scaleX(1) scaleY(1)" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
        
        <!-- Error state -->
        <Style Selector="Border.status-card.error">
            <Setter Property="Background" Value="{DynamicResource MTM_Danger}" />
            <Style.Animations>
                <Animation Duration="0:0:0.6" FillMode="Forward">
                    <KeyFrame Cue="0%">
                        <Setter Property="RenderTransform" Value="translateX(0)" />
                    </KeyFrame>
                    <KeyFrame Cue="25%">
                        <Setter Property="RenderTransform" Value="translateX(-5)" />
                    </KeyFrame>
                    <KeyFrame Cue="75%">
                        <Setter Property="RenderTransform" Value="translateX(5)" />
                    </KeyFrame>
                    <KeyFrame Cue="100%">
                        <Setter Property="RenderTransform" Value="translateX(0)" />
                    </KeyFrame>
                </Animation>
            </Style.Animations>
        </Style>
    </Border.Styles>
    
    <StackPanel Spacing="8" Margin="16">
        <TextBlock Text="{Binding StatusTitle}"
                   FontWeight="Bold"
                   Foreground="White" />
        <TextBlock Text="{Binding StatusMessage}"
                   Foreground="White" />
    </StackPanel>
</Border>
```

### **Page Transition Animations**

```csharp
// Page Transition Manager
public class PageTransitionManager
{
    public async Task TransitionToPageAsync(UserControl fromPage, UserControl toPage, 
                                          TransitionType transitionType = TransitionType.SlideLeft)
    {
        var container = fromPage.Parent as Panel;
        if (container == null) return;

        // Add new page to container
        container.Children.Add(toPage);
        
        // Set initial positions based on transition type
        switch (transitionType)
        {
            case TransitionType.SlideLeft:
                toPage.RenderTransform = new TranslateTransform(container.Bounds.Width, 0);
                break;
            case TransitionType.SlideRight:
                toPage.RenderTransform = new TranslateTransform(-container.Bounds.Width, 0);
                break;
            case TransitionType.FadeIn:
                toPage.Opacity = 0;
                break;
        }

        // Create animations
        var animationTasks = new List<Task>();

        switch (transitionType)
        {
            case TransitionType.SlideLeft:
                animationTasks.Add(AnimateTransform(fromPage, -container.Bounds.Width, 0, TimeSpan.FromMilliseconds(300)));
                animationTasks.Add(AnimateTransform(toPage, 0, 0, TimeSpan.FromMilliseconds(300)));
                break;
                
            case TransitionType.FadeIn:
                animationTasks.Add(AnimateOpacity(fromPage, 0, TimeSpan.FromMilliseconds(150)));
                animationTasks.Add(AnimateOpacity(toPage, 1, TimeSpan.FromMilliseconds(300)));
                break;
        }

        // Wait for animations to complete
        await Task.WhenAll(animationTasks);

        // Remove old page
        container.Children.Remove(fromPage);
    }

    private async Task AnimateTransform(Control control, double toX, double toY, TimeSpan duration)
    {
        var animation = new Animation
        {
            Duration = duration,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Control.RenderTransformProperty, new TranslateTransform(toX, toY)) }
                }
            }
        };

        await animation.RunAsync(control);
    }

    private async Task AnimateOpacity(Control control, double toOpacity, TimeSpan duration)
    {
        var animation = new Animation
        {
            Duration = duration,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(Control.OpacityProperty, toOpacity) }
                }
            }
        };

        await animation.RunAsync(control);
    }
}

public enum TransitionType
{
    SlideLeft,
    SlideRight,
    FadeIn,
    FadeOut
}
```

## 🔍 **Search and Filter Patterns**

### **Advanced Search Interface**

```xml
<!-- Expandable Search Panel -->
<Expander x:Name="SearchExpander"
          Header="Advanced Search"
          IsExpanded="{Binding IsAdvancedSearchExpanded}"
          Classes="mtm-search-expander">
    
    <Border Classes="mtm-card" Margin="0,8,0,0">
        <Grid ColumnDefinitions="1*,1*,1*" RowDefinitions="Auto,Auto,Auto" Spacing="16">
            
            <!-- Quick Search -->
            <TextBox Grid.Column="0" Grid.Row="0"
                     Watermark="Search parts..."
                     Text="{Binding SearchQuery, Mode=TwoWay}"
                     Classes="mtm-textbox-search">
                <TextBox.InnerLeftContent>
                    <Path Data="{StaticResource SearchIcon}"
                          Fill="{DynamicResource MTM_TextSecondary}"
                          Width="16" Height="16"
                          Margin="8,0,0,0" />
                </TextBox.InnerLeftContent>
                <TextBox.InnerRightContent>
                    <Button Content="×"
                           Command="{Binding ClearSearchCommand}"
                           IsVisible="{Binding HasSearchQuery}"
                           Classes="mtm-button-clear" />
                </TextBox.InnerRightContent>
            </TextBox>
            
            <!-- Category Filter -->
            <ComboBox Grid.Column="1" Grid.Row="0"
                     ItemsSource="{Binding AvailableCategories}"
                     SelectedItem="{Binding SelectedCategory}"
                     PlaceholderText="All Categories"
                     Classes="mtm-combobox" />
            
            <!-- Status Filter -->
            <ComboBox Grid.Column="2" Grid.Row="0"
                     ItemsSource="{Binding AvailableStatuses}"
                     SelectedItem="{Binding SelectedStatus}"
                     PlaceholderText="All Statuses"
                     Classes="mtm-combobox" />
            
            <!-- Date Range -->
            <StackPanel Grid.Column="0" Grid.Row="1" Orientation="Horizontal" Spacing="8">
                <TextBlock Text="From:" VerticalAlignment="Center" />
                <DatePicker SelectedDate="{Binding DateFrom}" Classes="mtm-datepicker" />
            </StackPanel>
            
            <StackPanel Grid.Column="1" Grid.Row="1" Orientation="Horizontal" Spacing="8">
                <TextBlock Text="To:" VerticalAlignment="Center" />
                <DatePicker SelectedDate="{Binding DateTo}" Classes="mtm-datepicker" />
            </StackPanel>
            
            <!-- Search Actions -->
            <StackPanel Grid.Column="2" Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8">
                <Button Content="Clear Filters"
                       Command="{Binding ClearFiltersCommand}"
                       Classes="mtm-button-secondary" />
                <Button Content="Search"
                       Command="{Binding SearchCommand}"
                       Classes="mtm-button-primary" />
            </StackPanel>
        </Grid>
    </Border>
</Expander>

<!-- Search Results with Sorting -->
<DataGrid ItemsSource="{Binding SearchResults}"
          CanUserSortColumns="True"
          Classes="mtm-datagrid-results">
    
    <!-- Results header with count -->
    <DataGrid.Resources>
        <Style Selector="DataGridColumnHeader">
            <Setter Property="Template">
                <ControlTemplate>
                    <Border Background="{TemplateBinding Background}"
                           BorderBrush="{TemplateBinding BorderBrush}"
                           BorderThickness="{TemplateBinding BorderThickness}">
                        <StackPanel Orientation="Horizontal" Spacing="4" Margin="8,4">
                            <ContentPresenter Content="{TemplateBinding Content}" />
                            <Path x:Name="SortIcon" IsVisible="False" />
                        </StackPanel>
                    </Border>
                </ControlTemplate>
            </Setter>
        </Style>
    </DataGrid.Resources>
</DataGrid>
```

### **Real-time Filter Implementation**

```csharp
// Advanced Search ViewModel
public partial class AdvancedSearchViewModel : BaseViewModel
{
    private readonly ISearchService _searchService;
    private readonly Timer _searchTimer;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<SearchResult> searchResults = new();

    [ObservableProperty]
    private bool isSearching;

    [ObservableProperty]
    private int resultCount;

    public AdvancedSearchViewModel(ISearchService searchService)
    {
        _searchService = searchService;
        _searchTimer = new Timer(PerformSearch, null, Timeout.Infinite, Timeout.Infinite);
    }

    partial void OnSearchQueryChanged(string value)
    {
        // Debounce search by 300ms
        _searchTimer.Change(300, Timeout.Infinite);
    }

    private async void PerformSearch(object state)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                IsSearching = true;
                
                var searchParameters = new SearchParameters
                {
                    Query = SearchQuery,
                    Category = SelectedCategory,
                    Status = SelectedStatus,
                    DateFrom = DateFrom,
                    DateTo = DateTo,
                    PageSize = 100
                };

                var results = await _searchService.SearchAsync(searchParameters);
                
                SearchResults.Clear();
                foreach (var result in results.Items)
                {
                    SearchResults.Add(result);
                }
                
                ResultCount = results.TotalCount;
            }
            catch (Exception ex)
            {
                await ErrorHandling.HandleErrorAsync(ex, "Search operation failed");
            }
            finally
            {
                IsSearching = false;
            }
        });
    }

    [RelayCommand]
    private void ClearFilters()
    {
        SearchQuery = string.Empty;
        SelectedCategory = null;
        SelectedStatus = null;
        DateFrom = null;
        DateTo = null;
        
        SearchResults.Clear();
        ResultCount = 0;
    }
}
```

This comprehensive behavior pattern guide ensures consistent, intuitive user interactions throughout the MTM WIP Application, optimized for manufacturing environments and accessible to all users.
