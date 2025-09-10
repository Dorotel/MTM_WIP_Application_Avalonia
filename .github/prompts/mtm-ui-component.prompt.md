---
description: 'Generate complete UI components for MTM application following Avalonia AXAML patterns and MTM design system'
tools: ['codebase', 'search', 'usages', 'editFiles']
---

# MTM UI Component Generator

Create complete UI components for the MTM WIP Application that follow established Avalonia AXAML patterns, MTM design system standards, and MVVM Community Toolkit integration.

## Component Generation Framework

### 1. UI Component Analysis
- **Component Type**: Determine component category (Form, Dialog, Panel, Control, View)
- **Data Requirements**: Identify data binding needs and ViewModel integration
- **User Interactions**: Map user actions to commands and events
- **Design System Integration**: Ensure MTM theme compliance and responsive design

### 2. Technical Requirements Assessment
- **Avalonia Compatibility**: Verify AXAML syntax and control usage
- **MVVM Integration**: Plan ViewModel properties and command bindings
- **Accessibility**: Include TabIndex, ToolTip.Tip, and screen reader support
- **Performance**: Optimize for smooth 60fps interactions and efficient memory usage

## UI Component Template Structure

### AXAML View Template
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.[Area]"
             x:Class="MTM_WIP_Application_Avalonia.Views.[Area].[ComponentName]">

<!-- MANDATORY: Use ScrollViewer for tab views to prevent overflow -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    
    <!-- Content Border with MTM theme integration -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" 
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      
      <!-- Component-specific content -->
      <Grid x:Name="ContentGrid" RowDefinitions="Auto,Auto,*">
        <!-- Header section -->
        <TextBlock Grid.Row="0" Text="{Binding Title}" 
                   FontSize="18" FontWeight="SemiBold"
                   Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}"
                   Margin="0,0,0,16" />
        
        <!-- Form fields section -->
        <Grid Grid.Row="1" x:Name="FormFieldsGrid" 
              ColumnDefinitions="120,*" RowDefinitions="Auto,Auto,Auto" 
              RowSpacing="12" ColumnSpacing="12">
          
          <!-- Example field pattern -->
          <TextBlock Grid.Column="0" Grid.Row="0" Text="Field Label:" 
                     VerticalAlignment="Center"
                     Foreground="{DynamicResource MTM_Shared_Logic.BodyText}" />
          <TextBox Grid.Column="1" Grid.Row="0" 
                   Text="{Binding FieldValue}"
                   TabIndex="1"
                   ToolTip.Tip="Enter field value"
                   Background="{DynamicResource MTM_Shared_Logic.ContentAreas}"
                   BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}" />
        </Grid>
        
        <!-- Data display section -->
        <Border Grid.Row="2" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
                BorderThickness="1" CornerRadius="6" Padding="12" Margin="0,16,0,0">
          
          <!-- Data grid or content display -->
          <ContentPresenter Content="{Binding DataContent}" />
        </Border>
      </Grid>
    </Border>
    
    <!-- Action buttons panel -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
            BorderThickness="1" CornerRadius="6" Padding="12">
      
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8">
        <!-- Primary action button -->
        <Button Content="Save Changes"
                Command="{Binding SaveCommand}"
                Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
                Foreground="White"
                Padding="16,8"
                CornerRadius="4"
                FontWeight="SemiBold"
                TabIndex="10"
                ToolTip.Tip="Save all changes" />
        
        <!-- Secondary action button -->
        <Button Content="Cancel"
                Command="{Binding CancelCommand}"
                Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
                Foreground="White"
                Padding="12,6"
                CornerRadius="4"
                TabIndex="11"
                ToolTip.Tip="Cancel without saving" />
      </StackPanel>
    </Border>
  </Grid>
</ScrollViewer>
</UserControl>
```

### ViewModel Template
```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MTM_WIP_Application_Avalonia.ViewModels.[Area];

/// <summary>
/// ViewModel for [ComponentName] - [Brief description of functionality]
/// </summary>
[ObservableObject]
public partial class [ComponentName]ViewModel : BaseViewModel
{
    #region Private Fields
    
    private readonly I[Service] _service;
    
    #endregion

    #region Observable Properties
    
    [ObservableProperty]
    private string title = "[Component Title]";
    
    [ObservableProperty]
    [Required(ErrorMessage = "Field is required")]
    [StringLength(50, ErrorMessage = "Field cannot exceed 50 characters")]
    private string fieldValue = string.Empty;
    
    [ObservableProperty]
    private bool isLoading;
    
    [ObservableProperty]
    private string? statusMessage;
    
    [ObservableProperty]
    private ObservableCollection<DataItemModel> dataItems = new();
    
    [ObservableProperty]
    private DataItemModel? selectedItem;
    
    #endregion

    #region Computed Properties
    
    public bool CanSave => !IsLoading && !string.IsNullOrWhiteSpace(FieldValue) && !HasErrors;
    
    public bool HasData => DataItems.Count > 0;
    
    #endregion

    #region Commands
    
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                // Validate input
                ValidateAllProperties();
                if (HasErrors)
                {
                    StatusMessage = "Please correct validation errors";
                    return;
                }

                // Perform save operation
                var result = await _service.SaveAsync(new SaveRequest
                {
                    FieldValue = FieldValue,
                    // Map other properties
                });

                if (result.IsSuccess)
                {
                    StatusMessage = "Save completed successfully";
                    await RefreshDataAsync();
                }
                else
                {
                    StatusMessage = $"Save failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                await Services.ErrorHandling.HandleErrorAsync(ex, "Save operation failed");
                StatusMessage = "An error occurred during save";
            }
        });
    }
    
    [RelayCommand]
    private async Task CancelAsync()
    {
        // Reset form or navigate away
        FieldValue = string.Empty;
        StatusMessage = null;
        ClearErrors();
    }
    
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            try
            {
                var data = await _service.GetDataAsync();
                
                DataItems.Clear();
                foreach (var item in data)
                {
                    DataItems.Add(item);
                }
                
                StatusMessage = $"Loaded {data.Count} items";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to refresh data");
                await Services.ErrorHandling.HandleErrorAsync(ex, "Refreshing data");
                StatusMessage = "Failed to load data";
            }
        });
    }
    
    [RelayCommand]
    private void SelectItem(DataItemModel item)
    {
        SelectedItem = item;
        // Handle item selection logic
    }
    
    #endregion

    #region Constructor
    
    public [ComponentName]ViewModel(
        ILogger<[ComponentName]ViewModel> logger,
        I[Service] service)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(service);
        
        _service = service;
        
        // Initialize data
        _ = InitializeAsync();
    }
    
    #endregion

    #region Private Methods
    
    private async Task InitializeAsync()
    {
        await ExecuteWithLoadingAsync(async () =>
        {
            await RefreshDataAsync();
        });
    }
    
    #endregion

    #region Property Change Handlers
    
    partial void OnFieldValueChanged(string value)
    {
        // Clear status when user starts typing
        StatusMessage = null;
        
        // Update command states
        SaveCommand.NotifyCanExecuteChanged();
    }
    
    partial void OnSelectedItemChanged(DataItemModel? value)
    {
        if (value != null)
        {
            // Update form with selected item data
            FieldValue = value.SomeProperty;
        }
    }
    
    #endregion
}
```

### Code-Behind Template
```csharp
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace MTM_WIP_Application_Avalonia.Views.[Area];

public partial class [ComponentName] : UserControl
{
    public [ComponentName]()
    {
        InitializeComponent();
        // Minimal initialization only - ViewModel injected via DI
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        // Set initial focus if needed
        var firstInput = this.FindControl<TextBox>("FirstInputField");
        firstInput?.Focus();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Clean up resources
        if (DataContext is IDisposable disposableContext)
        {
            disposableContext.Dispose();
        }
        
        base.OnDetachedFromVisualTree(e);
    }
    
    // UI-specific event handlers only (business logic stays in ViewModel)
    private void OnTextBox_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}
```

## MTM Component Categories

### Form Components
- **User Input Forms**: Data entry with validation and submission
- **Search Forms**: Filtering and search interfaces
- **Configuration Forms**: Settings and preferences management

### Display Components
- **Data Grids**: Tabular data display with sorting and filtering
- **Detail Views**: Single item detailed display
- **Dashboard Panels**: Summary and metrics display

### Interactive Components
- **Dialog Boxes**: Modal interactions and confirmations
- **Tab Panels**: Multi-section content organization
- **Navigation Menus**: Application navigation interfaces

### Specialized Components
- **Inventory Controls**: Manufacturing-specific data entry
- **Transaction Displays**: Operation history and tracking
- **Report Viewers**: Data analysis and reporting interfaces

## Design System Requirements

### Color Usage
- **Primary Actions**: `MTM_Shared_Logic.PrimaryAction` (#0078D4)
- **Secondary Actions**: `MTM_Shared_Logic.SecondaryAction` (#106EBE)
- **Success States**: `MTM_Shared_Logic.SuccessBrush` (#4CAF50)
- **Warning States**: `MTM_Shared_Logic.WarningBrush` (#FFB900)
- **Error States**: `MTM_Shared_Logic.ErrorBrush` (#D13438)

### Typography
- **Headings**: FontSize="18" FontWeight="SemiBold"
- **Body Text**: FontSize="14" FontWeight="Normal"
- **Labels**: FontSize="13" FontWeight="Medium"
- **Captions**: FontSize="12" FontWeight="Normal"

### Spacing
- **Card Padding**: 16px
- **Form Spacing**: 12px between fields
- **Button Spacing**: 8px between buttons
- **Section Margins**: 24px between major sections

## Accessibility Requirements

### Keyboard Navigation
- **TabIndex**: Sequential tab order for all interactive elements
- **Focus Management**: Visible focus indicators and logical flow
- **Keyboard Shortcuts**: Standard shortcuts (Enter, Escape, Arrow keys)

### Screen Reader Support
- **ToolTip.Tip**: Descriptive tooltips for all controls
- **AutomationProperties**: Proper automation IDs and names
- **Labels**: Associated labels for all input fields

### High Contrast Support
- **Dynamic Resources**: Use theme-aware colors throughout
- **Border Visibility**: Ensure borders are visible in high contrast
- **Text Contrast**: Maintain readable contrast ratios

Use this template when creating new UI components that need to integrate seamlessly with the MTM application architecture and design system.