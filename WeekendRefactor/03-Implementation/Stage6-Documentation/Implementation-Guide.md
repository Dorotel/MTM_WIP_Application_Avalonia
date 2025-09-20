# Stage 6: Documentation & Testing - Implementation Guide

**Priority**: 🟢 Low  
**Timeline**: Weekend Day 3  
**Estimated Time**: 2-3 hours  

## 🎯 Overview

Stage 6 completes the overlay system refactor by creating comprehensive documentation, finalizing testing, and preparing the system for production use. This stage ensures maintainability and knowledge transfer.

## 📋 Task Breakdown

### **Task 6.1: Universal Service Documentation**

**Estimated Time**: 45 minutes  
**Risk Level**: Very Low  
**Dependencies**: Stage 2 Universal Service complete  

#### **Developer Guide Creation**

Create comprehensive documentation in `docs/development/overlay-universal-service-guide.md`:

```markdown
# Universal Overlay Service - Developer Guide

## Overview

The Universal Overlay Service provides a centralized, consistent way to display overlays across the MTM WIP Application. It handles overlay lifecycle, pooling, theming, and user interactions.

## Architecture

### Interface Design

```csharp
public interface IUniversalOverlayService : IDisposable
{
    // Confirmation overlays
    Task<ConfirmationOverlayResponse> ShowConfirmationOverlayAsync(ConfirmationOverlayRequest request);
    
    // Success/Error overlays  
    Task<SuccessOverlayResponse> ShowSuccessOverlayAsync(SuccessOverlayRequest request);
    
    // Progress overlays
    Task<IProgressTracker> ShowProgressAsync(ProgressOverlayRequest request);
    
    // Validation overlays
    Task<ValidationOverlayResponse> ShowValidationOverlayAsync(ValidationOverlayRequest request);
    
    // Generic overlay display
    Task<TResponse> ShowOverlayAsync<TRequest, TResponse, TViewModel>(TRequest request)
        where TRequest : BaseOverlayRequest
        where TResponse : BaseOverlayResponse  
        where TViewModel : BasePoolableOverlayViewModel;
}
```

### Request/Response Pattern

All overlay interactions follow a consistent request/response pattern:

```csharp
// Base request with common properties
public abstract record BaseOverlayRequest
{
    public string Title { get; init; } = string.Empty;
    public OverlayPriority Priority { get; init; } = OverlayPriority.Normal;
    public bool CanDismiss { get; init; } = true;
    public TimeSpan? AutoDismissAfter { get; init; }
}

// Base response with result information
public abstract record BaseOverlayResponse  
{
    public OverlayResult Result { get; init; } = OverlayResult.None;
    public DateTime ShownAt { get; init; } = DateTime.Now;
    public DateTime DismissedAt { get; init; }
    public TimeSpan Duration => DismissedAt - ShownAt;
}
```

## Usage Examples

### Confirmation Dialog

```csharp
var request = new ConfirmationOverlayRequest(
    Title: "Delete Item",
    Message: "Are you sure you want to delete this inventory item? This action cannot be undone.",
    ConfirmationType: ConfirmationType.Delete,
    PrimaryActionText: "Delete Item",
    SecondaryActionText: "Cancel"
);

var response = await _overlayService.ShowConfirmationOverlayAsync(request);

if (response.Result == OverlayResult.Confirmed)
{
    await DeleteItemAsync();
}
```

### Progress Dialog

```csharp
using var progress = await _overlayService.ShowProgressAsync(
    new ProgressOverlayRequest(
        Title: "Importing Data",
        InitialStep: "Preparing import...",
        CanCancel: true
    )
);

try
{
    await progress.UpdateProgressAsync(25, "Reading file...");
    // ... processing
    await progress.UpdateProgressAsync(75, "Saving to database...");
    // ... completion
    await progress.CompleteAsync("Import completed successfully");
}
catch (Exception ex)
{
    await progress.CancelAsync("Import failed");
    throw;
}
```

### Validation Feedback

```csharp
var validationErrors = ValidateForm();
if (validationErrors.Any())
{
    var request = new ValidationOverlayRequest(
        Title: "Input Validation",
        ValidationErrors: validationErrors,
        Severity: ValidationSeverity.Error
    );
    
    await _overlayService.ShowValidationOverlayAsync(request);
    return;
}
```

## Integration Patterns

### ViewModel Integration

```csharp
[ObservableObject]
public partial class MyViewModel : BaseViewModel
{
    private readonly IUniversalOverlayService _overlayService;
    
    public MyViewModel(
        ILogger<MyViewModel> logger,
        IUniversalOverlayService overlayService) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(overlayService);
        _overlayService = overlayService;
    }
    
    [RelayCommand]
    private async Task PerformActionWithConfirmationAsync()
    {
        var confirmed = await _overlayService.ShowConfirmationOverlayAsync(
            new ConfirmationOverlayRequest("Confirm Action", "Proceed with action?"));
            
        if (confirmed.Result == OverlayResult.Confirmed)
        {
            await PerformActionAsync();
        }
    }
}
```

### Service Registration

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOverlayServices(this IServiceCollection services)
    {
        // Register Universal Service
        services.TryAddSingleton<IUniversalOverlayService, UniversalOverlayService>();
        
        // Register supporting services
        services.TryAddSingleton<IOverlayPoolManager, OverlayPoolManager>();
        services.TryAddScoped<IOverlayAnimationService, OverlayAnimationService>();
        
        // Register ViewModels
        services.TryAddTransient<ConfirmationOverlayViewModel>();
        services.TryAddTransient<ProgressOverlayViewModel>();
        services.TryAddTransient<ValidationOverlayViewModel>();
        
        return services;
    }
}
```

```

#### **API Reference Documentation**

Create `docs/api/universal-overlay-service-api.md` with complete API documentation.

#### **Code Examples Collection**

Create `docs/examples/overlay-usage-examples.md` with comprehensive usage examples.

---

### **Task 6.2: Overlay Development Tutorial**

**Estimated Time**: 60 minutes  
**Risk Level**: Very Low  
**Dependencies**: All previous stages  

#### **Step-by-Step Tutorial Creation**

Create `docs/tutorials/creating-custom-overlays.md`:

```markdown
# Creating Custom Overlays - Tutorial

## Introduction

This tutorial walks through creating a custom overlay from scratch, following MTM patterns and integrating with the Universal Overlay Service.

## Step 1: Define Overlay Requirements

Before creating an overlay, define:

1. **Purpose**: What problem does this overlay solve?
2. **User Interaction**: How will users interact with it?
3. **Data Requirements**: What information does it need to display/collect?
4. **Integration Points**: Which ViewModels will use it?

### Example: Inventory Notes Overlay

We'll create a custom overlay for editing inventory item notes.

**Requirements**:
- Display current notes for an inventory item
- Allow editing with rich text support
- Validate note length (max 500 characters)
- Save/cancel actions
- Integration with InventoryTabView and RemoveTabView

## Step 2: Create Request/Response Models

```csharp
// Request model with all necessary data
public record InventoryNotesOverlayRequest(
    string PartId,
    string Operation, 
    string CurrentNotes = "",
    int MaxLength = 500
) : BaseOverlayRequest;

// Response model with result data
public record InventoryNotesOverlayResponse(
    OverlayResult Result,
    string UpdatedNotes = "",
    bool NotesChanged = false
) : BaseOverlayResponse;
```

## Step 3: Create ViewModel

```csharp
[ObservableObject]
public partial class InventoryNotesOverlayViewModel : BasePoolableOverlayViewModel
{
    [ObservableProperty] private string partId = string.Empty;
    [ObservableProperty] private string operation = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private string originalNotes = string.Empty;
    [ObservableProperty] private int maxLength = 500;
    [ObservableProperty] private int remainingCharacters = 500;
    [ObservableProperty] private bool hasUnsavedChanges = false;

    // Constructor with dependency injection
    public InventoryNotesOverlayViewModel(ILogger<InventoryNotesOverlayViewModel> logger) 
        : base(logger) { }

    // Initialize from request
    public void Initialize(InventoryNotesOverlayRequest request)
    {
        PartId = request.PartId;
        Operation = request.Operation;
        Notes = request.CurrentNotes;
        OriginalNotes = request.CurrentNotes;
        MaxLength = request.MaxLength;
        UpdateRemainingCharacters();
        HasUnsavedChanges = false;
    }

    // Commands
    [RelayCommand]
    private async Task SaveNotesAsync()
    {
        if (ValidateNotes())
        {
            var response = new InventoryNotesOverlayResponse(
                OverlayResult.Confirmed,
                UpdatedNotes: Notes,
                NotesChanged: Notes != OriginalNotes
            );
            
            await CloseAsync(response);
        }
    }

    [RelayCommand]
    private async Task CancelEditingAsync()
    {
        if (HasUnsavedChanges)
        {
            // Could show confirmation here
            var confirmDiscard = await ConfirmDiscardChanges();
            if (!confirmDiscard) return;
        }

        var response = new InventoryNotesOverlayResponse(OverlayResult.Cancelled);
        await CloseAsync(response);
    }

    // Property change handling
    partial void OnNotesChanged(string value)
    {
        UpdateRemainingCharacters();
        HasUnsavedChanges = value != OriginalNotes;
    }

    private void UpdateRemainingCharacters()
    {
        RemainingCharacters = MaxLength - Notes.Length;
    }

    private bool ValidateNotes()
    {
        return Notes.Length <= MaxLength && !string.IsNullOrWhiteSpace(Notes.Trim());
    }

    // Reset for pooling
    protected override void OnReset()
    {
        PartId = string.Empty;
        Operation = string.Empty; 
        Notes = string.Empty;
        OriginalNotes = string.Empty;
        MaxLength = 500;
        RemainingCharacters = 500;
        HasUnsavedChanges = false;
    }
}
```

## Step 4: Create AXAML View

```xml
<!-- Views/Overlay/InventoryNotesOverlayView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             x:Class="MTM_WIP_Application_Avalonia.Views.Overlay.InventoryNotesOverlayView"
             x:DataType="vm:InventoryNotesOverlayViewModel">

  <!-- Overlay background -->
  <Border IsVisible="{Binding IsVisible}"
          Background="#80000000"
          x:Name="OverlayBackground">
    
    <!-- Main overlay card -->
    <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
            BorderThickness="2"
            CornerRadius="12"
            Padding="24"
            MinWidth="500"
            MaxWidth="700"
            MinHeight="400"
            HorizontalAlignment="Center"
            VerticalAlignment="Center">
      
      <Grid x:Name="NotesEditContent" RowDefinitions="Auto,Auto,*,Auto,Auto">
        
        <!-- Header -->
        <Border Grid.Row="0"
                Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
                CornerRadius="8"
                Padding="16,12"
                Margin="0,0,0,16">
          
          <Grid x:Name="HeaderContent" ColumnDefinitions="*,Auto">
            <StackPanel Grid.Column="0">
              <TextBlock Text="Edit Inventory Notes"
                         FontSize="18"
                         FontWeight="Bold"
                         Foreground="White" />
              
              <TextBlock Text="{Binding PartId, StringFormat='Part: {0}'}"
                         FontSize="12"
                         Foreground="White"
                         Opacity="0.8" />
            </StackPanel>
            
            <TextBlock Grid.Column="1"
                       Text="{Binding Operation, StringFormat='Op: {0}'}"
                       FontSize="12"
                       Foreground="White"
                       VerticalAlignment="Center" />
          </Grid>
        </Border>
        
        <!-- Character count -->
        <Border Grid.Row="1"
                Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
                CornerRadius="4"
                Padding="12,8"
                Margin="0,0,0,12">
          
          <Grid x:Name="CharacterCount" ColumnDefinitions="*,Auto">
            <TextBlock Grid.Column="0"
                       Text="Notes (supports rich formatting)"
                       FontSize="14"
                       FontWeight="Bold"
                       VerticalAlignment="Center" />
            
            <TextBlock Grid.Column="1"
                       Text="{Binding RemainingCharacters, StringFormat='{}{0} characters remaining'}"
                       FontSize="12"
                       Foreground="{Binding RemainingCharacters, Converter={StaticResource CharacterCountToColorConverter}}"
                       VerticalAlignment="Center" />
          </Grid>
        </Border>
        
        <!-- Notes editor -->
        <Border Grid.Row="2"
                Background="{DynamicResource MTM_Shared_Logic.InputBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
                BorderThickness="1"
                CornerRadius="8"
                Padding="12"
                Margin="0,0,0,16">
          
          <TextBox Text="{Binding Notes}"
                   AcceptsReturn="True"
                   TextWrapping="Wrap"
                   VerticalContentAlignment="Top"
                   MinHeight="200"
                   MaxLength="{Binding MaxLength}"
                   Watermark="Enter notes for this inventory item..."
                   FontSize="14" />
        </Border>
        
        <!-- Unsaved changes indicator -->
        <Border Grid.Row="3"
                IsVisible="{Binding HasUnsavedChanges}"
                Background="#FFF3CD"
                BorderBrush="#856404"
                BorderThickness="1"
                CornerRadius="4"
                Padding="12,8"
                Margin="0,0,0,16">
          
          <TextBlock Text="⚠️ You have unsaved changes"
                     FontSize="12"
                     Foreground="#856404"
                     HorizontalAlignment="Center" />
        </Border>
        
        <!-- Action buttons -->
        <Grid Grid.Row="4" x:Name="ActionButtons" ColumnDefinitions="*,Auto,Auto">
          
          <Button Grid.Column="1"
                  Content="Save Notes"
                  Command="{Binding SaveNotesCommand}"
                  IsEnabled="{Binding HasUnsavedChanges}"
                  Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
                  Foreground="White"
                  Padding="20,10"
                  CornerRadius="6"
                  FontWeight="Bold"
                  Margin="0,0,12,0" />
          
          <Button Grid.Column="2"
                  Content="Cancel"
                  Command="{Binding CancelEditingCommand}"
                  Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
                  Foreground="White"
                  Padding="20,10"
                  CornerRadius="6" />
        </Grid>
      </Grid>
    </Border>
  </Border>
</UserControl>
```

## Step 5: Integrate with Universal Service

```csharp
// Add to Universal Service interface
public interface IUniversalOverlayService 
{
    // ... existing methods
    
    Task<InventoryNotesOverlayResponse> ShowInventoryNotesOverlayAsync(
        InventoryNotesOverlayRequest request);
}

// Implementation in Universal Service
public async Task<InventoryNotesOverlayResponse> ShowInventoryNotesOverlayAsync(
    InventoryNotesOverlayRequest request)
{
    return await ShowOverlayAsync<
        InventoryNotesOverlayRequest, 
        InventoryNotesOverlayResponse, 
        InventoryNotesOverlayViewModel>(request);
}
```

## Step 6: Register Services

```csharp
// Add to ServiceCollectionExtensions
public static IServiceCollection AddInventoryNotesOverlay(this IServiceCollection services)
{
    services.TryAddTransient<InventoryNotesOverlayViewModel>();
    return services;
}
```

## Step 7: Use in ViewModels

```csharp
[RelayCommand]
private async Task EditNotesAsync()
{
    var request = new InventoryNotesOverlayRequest(
        PartId: CurrentPartId,
        Operation: CurrentOperation,
        CurrentNotes: CurrentNotes
    );
    
    var response = await _overlayService.ShowInventoryNotesOverlayAsync(request);
    
    if (response.Result == OverlayResult.Confirmed && response.NotesChanged)
    {
        CurrentNotes = response.UpdatedNotes;
        await SaveNotesToDatabase();
    }
}
```

## Best Practices

1. **Follow MTM Patterns**: Use MVVM Community Toolkit, proper AXAML syntax
2. **Implement Pooling Support**: Extend BasePoolableOverlayViewModel and implement Reset()
3. **Handle Validation**: Validate input before allowing submission
4. **Provide Feedback**: Show loading states, character counts, unsaved changes
5. **Theme Integration**: Use DynamicResource for all colors and styles
6. **Error Handling**: Use centralized error handling service
7. **Testing**: Write unit tests for ViewModel logic

```

---

### **Task 6.3: Integration Testing Documentation**

**Estimated Time**: 30 minutes  
**Risk Level**: Very Low  
**Dependencies**: Testing infrastructure  

#### **Testing Strategy Documentation**

Create `docs/testing/overlay-testing-strategy.md`:

```markdown
# Overlay System Testing Strategy

## Testing Pyramid

### Unit Tests (70%)
- ViewModel logic testing
- Request/response model validation
- Pool manager functionality
- Animation service behavior

### Integration Tests (25%)
- Service interaction testing
- Database integration with overlays
- Theme switching with overlays active
- Memory management validation

### UI/End-to-End Tests (5%)
- Complete user workflows
- Cross-platform compatibility
- Performance benchmarks
- Accessibility compliance

## Test Templates

### ViewModel Unit Test Template
```csharp
[TestClass]
public class CustomOverlayViewModelTests
{
    private CustomOverlayViewModel _viewModel;
    private Mock<ILogger<CustomOverlayViewModel>> _mockLogger;
    
    [TestInitialize]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<CustomOverlayViewModel>>();
        _viewModel = new CustomOverlayViewModel(_mockLogger.Object);
    }
    
    [TestMethod]
    public async Task ShowOverlay_WithValidRequest_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var request = new CustomOverlayRequest("Test Title", "Test Message");
        
        // Act
        await _viewModel.InitializeAsync(request);
        
        // Assert
        _viewModel.Title.Should().Be("Test Title");
        _viewModel.Message.Should().Be("Test Message");
        _viewModel.IsVisible.Should().BeTrue();
    }
}
```

### Service Integration Test Template

```csharp
[TestClass]
public class UniversalOverlayServiceIntegrationTests
{
    private IServiceProvider _serviceProvider;
    private IUniversalOverlayService _overlayService;
    
    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOverlayServices();
        
        _serviceProvider = services.BuildServiceProvider();
        _overlayService = _serviceProvider.GetRequiredService<IUniversalOverlayService>();
    }
    
    [TestMethod]
    public async Task ShowConfirmationOverlay_ShouldReturnCorrectResponse()
    {
        // Arrange
        var request = new ConfirmationOverlayRequest("Test", "Are you sure?");
        
        // Act & Assert - This would need UI testing framework
        // or mock implementations for automated testing
    }
}
```

```

---

### **Task 6.4: Migration Guide Creation**

**Estimated Time**: 45 minutes  
**Risk Level**: Very Low  
**Dependencies**: Legacy system knowledge  

#### **Legacy to Universal Service Migration Guide**

Create `docs/migration/overlay-system-migration-guide.md`:

```markdown
# Overlay System Migration Guide

## Overview

This guide helps migrate from legacy overlay patterns to the new Universal Overlay Service system.

## Common Migration Scenarios

### 1. Window-Based Dialogs to Overlays

**Before (Window-based)**:
```csharp
private async Task ShowStartupDialog()
{
    var dialog = new StartupDialogWindow();
    dialog.DataContext = new StartupDialogViewModel();
    
    var result = await dialog.ShowDialog<bool?>(this);
    if (result == true)
    {
        // Handle result
    }
}
```

**After (Overlay-based)**:

```csharp
private async Task ShowStartupOverlay()
{
    var request = new StartupOverlayRequest("Application Startup", initialData);
    var response = await _overlayService.ShowStartupOverlayAsync(request);
    
    if (response.Result == OverlayResult.Confirmed)
    {
        // Handle result
    }
}
```

### 2. Direct ViewModel Creation to Service Injection

**Before**:

```csharp
private async Task ShowConfirmation()
{
    var viewModel = new ConfirmationViewModel();
    viewModel.Title = "Confirm Delete";
    viewModel.Message = "Are you sure?";
    
    // Manual overlay management
    ConfirmationOverlay.DataContext = viewModel;
    ConfirmationOverlay.IsVisible = true;
    
    // Wait for result manually
}
```

**After**:

```csharp
private async Task ShowConfirmation()
{
    var request = new ConfirmationOverlayRequest(
        "Confirm Delete", 
        "Are you sure?", 
        ConfirmationType.Delete);
        
    var response = await _overlayService.ShowConfirmationOverlayAsync(request);
    
    if (response.Result == OverlayResult.Confirmed)
    {
        // Handle confirmation
    }
}
```

## Step-by-Step Migration Process

### Phase 1: Identify Legacy Overlays

1. Search for Window-based dialogs
2. Find direct overlay ViewModel instantiation
3. Locate manual overlay visibility management

### Phase 2: Create Universal Service Integration

1. Define Request/Response models
2. Add methods to Universal Service interface
3. Implement service methods

### Phase 3: Update ViewModels

1. Replace manual overlay code with service calls
2. Update dependency injection
3. Remove direct overlay dependencies

### Phase 4: Testing and Validation

1. Test all migration scenarios
2. Verify memory usage improvements
3. Confirm performance enhancements

## Benefits After Migration

- **Consistency**: All overlays follow same patterns
- **Performance**: Overlay pooling reduces memory allocation
- **Maintainability**: Centralized overlay management
- **Testability**: Service-based architecture easier to test
- **Theming**: Automatic theme support for all overlays

```

## 📊 Stage 6 Success Criteria

### **Documentation Completeness**

- [ ] Universal Service API fully documented with examples
- [ ] Step-by-step overlay creation tutorial complete
- [ ] Testing strategies and templates provided
- [ ] Migration guide covers all legacy scenarios

### **Testing Infrastructure**

- [ ] Unit test templates for all overlay types
- [ ] Integration test examples for service interactions
- [ ] Performance testing guidelines established
- [ ] Accessibility testing checklist created

### **Knowledge Transfer**

- [ ] Documentation is clear and actionable
- [ ] Code examples are complete and working
- [ ] Migration path is well-defined
- [ ] Future maintenance guidance provided

## 📝 Completion Checklist

- [ ] Universal Service documentation complete
- [ ] Overlay development tutorial written
- [ ] Testing framework documented
- [ ] Migration guide created
- [ ] API reference documentation complete
- [ ] Code examples validated
- [ ] Performance guidelines documented
- [ ] Accessibility requirements documented
- [ ] All documentation reviewed and proofread
- [ ] Documentation integrated into main docs structure

---

**Stage 6 ensures the overlay system refactor is not only complete but also maintainable, well-documented, and ready for future development.**
