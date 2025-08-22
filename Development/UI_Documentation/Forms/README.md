# Forms Directory

This directory contains comprehensive documentation for all complete forms and dialogs in the MTM WIP Application Avalonia.

## ?? Form Documentation

### Application Forms

#### `MainForm.instructions.md`
**Primary application window and main interface container**
- **Purpose**: Central hub for all application functionality
- **Architecture**: Modern sidebar navigation with tabbed content area
- **Key Features**: Menu system, status bar, progress tracking, theme management
- **Integration**: Contains all main control tabs, quick buttons panel, settings access

#### `SettingsForm.instructions.md`
**Comprehensive application configuration interface**
- **Purpose**: User preferences, system settings, and administrative functions
- **Architecture**: Tabbed interface organizing settings by category
- **Key Features**: Database settings, theme selection, user management, master data CRUD
- **Integration**: Theme application, configuration persistence, validation system

### Dialog Forms

#### `EnhancedErrorDialog.instructions.md`
**Advanced error display and reporting dialog**
- **Purpose**: User-friendly error presentation with technical details available
- **Architecture**: Expandable dialog with summary and detailed views
- **Key Features**: Error categorization, technical details, user actions, error reporting
- **Integration**: Centralized error handling, logging system, user feedback collection

#### `SplashScreenForm.instructions.md`
**Application startup screen with branding and initialization progress**
- **Purpose**: Professional application loading experience
- **Architecture**: Full-screen overlay with MTM branding and progress indication
- **Key Features**: Logo display, initialization progress, loading messages, error handling
- **Integration**: Application startup sequence, resource loading, error recovery

#### `Transactions.instructions.md`
**Transaction history viewing and analysis interface**
- **Purpose**: Comprehensive audit trail and transaction analysis
- **Architecture**: Advanced DataGrid with filtering, sorting, and export capabilities
- **Key Features**: Transaction search, detailed views, export functionality, audit reports
- **Integration**: Database transaction logs, reporting system, data analysis tools

## ??? Form Architecture Standards

### Window and Dialog Patterns
All forms follow consistent Avalonia UI patterns:

#### Main Window Pattern
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
        x:Class="MTM_WIP_Application_Avalonia.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        x:CompileBindings="True"
        Title="MTM WIP Application"
        Width="1200" Height="700"
        Icon="/Assets/mtm-icon.ico">
    
    <!-- Modern application layout -->
    <Grid ColumnDefinitions="240,*">
        <!-- Sidebar navigation -->
        <Border Grid.Column="0" Classes="sidebar"/>
        
        <!-- Main content area -->
        <Grid Grid.Column="1" Classes="content-area">
            <!-- Content with header, body, status bar -->
        </Grid>
    </Grid>
</Window>
```

#### Dialog Pattern
```xml
<Window xmlns="https://github.com/avaloniaui"
        x:Class="MTM_WIP_Application_Avalonia.Views.DialogWindow"
        Title="Dialog Title"
        Width="500" Height="350"
        WindowStartupLocation="CenterOwner"
        CanResize="False"
        ShowInTaskbar="False">
    
    <Grid RowDefinitions="*,Auto">
        <!-- Dialog content -->
        <ScrollViewer Grid.Row="0" Padding="24">
            <!-- Main dialog content -->
        </ScrollViewer>
        
        <!-- Dialog buttons -->
        <Border Grid.Row="1" Classes="dialog-buttons">
            <StackPanel Orientation="Horizontal" 
                        HorizontalAlignment="Right" 
                        Spacing="8" 
                        Margin="16">
                <Button Content="OK" Classes="primary"/>
                <Button Content="Cancel" Classes="secondary"/>
            </StackPanel>
        </Border>
    </Grid>
</Window>
```

### MTM Design System Implementation
Forms implement consistent MTM branding and visual design:

#### Hero Section Pattern
```xml
<!-- MTM branded hero section for forms -->
<Border Classes="hero-section" 
        Height="120" 
        Margin="0,0,0,24">
    <Border.Background>
        <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
            <GradientStop Color="#4574ED" Offset="0"/>
            <GradientStop Color="#4B45ED" Offset="0.3"/>
            <GradientStop Color="#8345ED" Offset="0.7"/>
            <GradientStop Color="#BA45ED" Offset="1"/>
        </LinearGradientBrush>
    </Border.Background>
    
    <Grid Margin="32">
        <StackPanel VerticalAlignment="Center" Spacing="8">
            <TextBlock Text="Form Title"
                       FontSize="24"
                       FontWeight="Bold"
                       Foreground="White"/>
            <TextBlock Text="Form description and purpose"
                       FontSize="14"
                       Foreground="White"
                       Opacity="0.9"/>
        </StackPanel>
    </Grid>
</Border>
```

#### Navigation Sidebar
```xml
<!-- Modern sidebar navigation for main form -->
<Border Classes="sidebar" Grid.Column="0">
    <DockPanel>
        <!-- Application header -->
        <Border DockPanel.Dock="Top" Classes="app-header">
            <Grid ColumnDefinitions="Auto,*">
                <Image Grid.Column="0" Source="/Assets/mtm-logo.png" Width="32"/>
                <TextBlock Grid.Column="1" Text="MTM WIP System" 
                           FontSize="16" FontWeight="SemiBold" 
                           VerticalAlignment="Center" Margin="12,0,0,0"/>
            </Grid>
        </Border>
        
        <!-- Navigation sections -->
        <ScrollViewer>
            <StackPanel Spacing="8" Margin="8">
                <!-- Expandable navigation groups -->
                <Expander Header="Inventory" IsExpanded="True">
                    <StackPanel Spacing="4" Margin="16,8,0,8">
                        <RadioButton GroupName="Navigation" 
                                     Content="Add Items" 
                                     Classes="nav-item"/>
                        <RadioButton GroupName="Navigation" 
                                     Content="Transfer Items" 
                                     Classes="nav-item"/>
                        <RadioButton GroupName="Navigation" 
                                     Content="Remove Items" 
                                     Classes="nav-item"/>
                    </StackPanel>
                </Expander>
                
                <Expander Header="Reports">
                    <StackPanel Spacing="4" Margin="16,8,0,8">
                        <RadioButton GroupName="Navigation" 
                                     Content="Transaction History" 
                                     Classes="nav-item"/>
                        <RadioButton GroupName="Navigation" 
                                     Content="Inventory Reports" 
                                     Classes="nav-item"/>
                    </StackPanel>
                </Expander>
            </StackPanel>
        </ScrollViewer>
    </DockPanel>
</Border>
```

## ?? Form Integration Patterns

### ViewModel Integration
Forms use ReactiveUI ViewModels with dependency injection:

```csharp
public class MainFormViewModel : ReactiveObject
{
    private readonly IInventoryService _inventoryService;
    private readonly IApplicationStateService _applicationState;
    private readonly INavigationService _navigationService;

    // Current view management
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }

    // Navigation commands
    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainFormViewModel(
        IInventoryService inventoryService,
        IApplicationStateService applicationState,
        INavigationService navigationService)
    {
        _inventoryService = inventoryService;
        _applicationState = applicationState;
        _navigationService = navigationService;

        InitializeCommands();
        InitializeNavigation();
    }

    private void InitializeCommands()
    {
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);
        
        // Global application commands
        ExitCommand = ReactiveCommand.Create(Application.Current.Shutdown);
        SettingsCommand = ReactiveCommand.Create(ShowSettings);
        AboutCommand = ReactiveCommand.Create(ShowAbout);
    }
}
```

### Inter-Form Communication
Forms coordinate through event-driven patterns and shared services:

```csharp
// Form-to-form communication via application state
public class FormCommunicationService : IFormCommunicationService
{
    public event EventHandler<InventoryChangedEventArgs>? InventoryChanged;
    public event EventHandler<SettingsChangedEventArgs>? SettingsChanged;
    public event EventHandler<UserChangedEventArgs>? UserChanged;

    public void NotifyInventoryChanged(InventoryItem item, ChangeType changeType)
    {
        InventoryChanged?.Invoke(this, new InventoryChangedEventArgs
        {
            Item = item,
            ChangeType = changeType,
            Timestamp = DateTime.Now
        });
    }

    public void NotifySettingsChanged(string settingName, object? oldValue, object? newValue)
    {
        SettingsChanged?.Invoke(this, new SettingsChangedEventArgs
        {
            SettingName = settingName,
            OldValue = oldValue,
            NewValue = newValue
        });
    }
}
```

### Modal Dialog Management
Standardized dialog display and result handling:

```csharp
// Dialog service for consistent modal management
public class DialogService : IDialogService
{
    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        var dialog = new ConfirmationDialog
        {
            Title = title,
            Message = message
        };

        var result = await dialog.ShowDialog<bool>(GetMainWindow());
        return result;
    }

    public async Task ShowErrorAsync(string title, string message, Exception? exception = null)
    {
        var dialog = new EnhancedErrorDialog
        {
            Title = title,
            Message = message,
            Exception = exception
        };

        await dialog.ShowDialog(GetMainWindow());
    }

    public async Task<T?> ShowDialogAsync<T>(Window dialog)
    {
        return await dialog.ShowDialog<T>(GetMainWindow());
    }
}
```

## ?? Business Logic Integration

### Service Layer Integration
Forms integrate with business services following clean architecture:

```csharp
// Form ViewModels coordinate multiple services
public class SettingsFormViewModel : ReactiveObject
{
    private readonly IConfigurationService _configurationService;
    private readonly IUserManagementService _userService;
    private readonly IThemeService _themeService;
    private readonly IDatabaseService _databaseService;

    public SettingsFormViewModel(
        IConfigurationService configurationService,
        IUserManagementService userService,
        IThemeService themeService,
        IDatabaseService databaseService)
    {
        _configurationService = configurationService;
        _userService = userService;
        _themeService = themeService;
        _databaseService = databaseService;

        InitializeSettings();
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            // Save all settings through appropriate services
            await _configurationService.SaveSettingsAsync(AppSettings);
            await _themeService.ApplyThemeAsync(SelectedTheme);
            await _databaseService.UpdateConnectionSettingsAsync(DatabaseSettings);

            StatusMessage = "Settings saved successfully";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save settings: {ex.Message}";
        }
    }
}
```

### Database Operations
Forms use stored procedures through service layer:

```csharp
// All database operations use stored procedures
private async Task LoadTransactionHistoryAsync()
{
    var searchCriteria = new TransactionSearchCriteria
    {
        StartDate = StartDate,
        EndDate = EndDate,
        PartId = SelectedPart,
        TransactionType = SelectedTransactionType,
        User = SelectedUser
    };

    var result = await _transactionService.SearchTransactionsAsync(searchCriteria);
    
    if (result.IsSuccess)
    {
        Transactions.Clear();
        Transactions.AddRange(result.Data);
        
        // Update summary information
        TotalTransactions = result.Data.Count;
        TotalValue = result.Data.Sum(t => t.Quantity * t.UnitCost);
    }
    else
    {
        ShowError($"Failed to load transaction history: {result.ErrorMessage}");
    }
}
```

### Transaction Type Implementation
Forms correctly implement MTM business rules:

```csharp
// Transaction type correctly determined by user intent
private void InitializeTransactionFilters()
{
    TransactionTypes = new ObservableCollection<TransactionTypeOption>
    {
        new() { Value = TransactionType.IN, Display = "IN - Adding Stock" },
        new() { Value = TransactionType.OUT, Display = "OUT - Removing Stock" },
        new() { Value = TransactionType.TRANSFER, Display = "TRANSFER - Moving Stock" }
    };

    // Note: Operation numbers are workflow identifiers, not transaction types
    OperationCodes = new ObservableCollection<string>
    {
        "90", "100", "110", "120", "130" // Workflow step identifiers
    };
}

// Display logic shows correct transaction type context
private string GetTransactionDescription(InventoryTransaction transaction)
{
    var actionDescription = transaction.TransactionType switch
    {
        TransactionType.IN => "Added to inventory",
        TransactionType.OUT => "Removed from inventory", 
        TransactionType.TRANSFER => $"Transferred from {transaction.FromLocation} to {transaction.ToLocation}",
        _ => "Unknown transaction type"
    };

    return $"{actionDescription} (Operation: {transaction.Operation})";
}
```

## ?? User Experience Features

### Responsive Design
Forms adapt to different screen sizes and user preferences:

```xml
<!-- Responsive layout that adapts to screen size -->
<Grid>
    <Grid.ColumnDefinitions>
        <!-- Sidebar collapses on smaller screens -->
        <ColumnDefinition Width="{Binding SidebarWidth, Mode=OneWay}"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <!-- Collapsible sidebar -->
    <Border Grid.Column="0" 
            IsVisible="{Binding IsSidebarVisible}"
            Classes="sidebar">
        <!-- Navigation content -->
    </Border>
    
    <!-- Main content adapts to available space -->
    <Grid Grid.Column="1" 
          Margin="{Binding ContentMargin}">
        <!-- Responsive content -->
    </Grid>
</Grid>
```

### Accessibility Implementation
Full accessibility support across all forms:

```xml
<!-- Comprehensive accessibility attributes -->
<Window AutomationProperties.Name="MTM WIP Application Main Window"
        AutomationProperties.HelpText="Main application window for inventory management">
    
    <!-- Proper heading structure -->
    <TextBlock Text="Inventory Management" 
               AutomationProperties.HeadingLevel="1"/>
    
    <!-- Keyboard navigation -->
    <Menu x:Name="MainMenu" 
          AutomationProperties.Name="Main application menu">
        <MenuItem Header="_File" 
                  AutomationProperties.AccessKey="F">
            <MenuItem Header="_Exit" 
                      HotKey="Alt+F4" 
                      Command="{Binding ExitCommand}"/>
        </MenuItem>
    </Menu>
    
    <!-- Form controls with proper labeling -->
    <TextBox AutomationProperties.Name="Part ID"
             AutomationProperties.HelpText="Enter or select the part identifier"
             AutomationProperties.IsRequiredForForm="True"/>
</Window>
```

### Progressive Enhancement
Forms provide enhanced functionality while maintaining core usability:

```csharp
// Feature detection and graceful degradation
public class FormCapabilityService
{
    public bool SupportsAdvancedFeatures { get; private set; }
    public bool SupportsRealTimeUpdates { get; private set; }
    public bool SupportsOfflineMode { get; private set; }

    public async Task InitializeCapabilitiesAsync()
    {
        try
        {
            // Test advanced features
            SupportsAdvancedFeatures = await TestAdvancedFeaturesAsync();
            SupportsRealTimeUpdates = await TestRealTimeConnectionAsync();
            SupportsOfflineMode = await TestOfflineCapabilitiesAsync();
        }
        catch (Exception ex)
        {
            // Log but continue with basic functionality
            _logger.LogWarning(ex, "Some advanced features may not be available");
        }
    }
}
```

## ?? Form Development Guidelines

### Creating New Forms
1. **Requirements Analysis**: Understand the business process and user needs
2. **UI/UX Design**: Create mockups following MTM design system
3. **Architecture Planning**: Define ViewModel structure and service dependencies
4. **Documentation**: Write complete `.instructions.md` specification
5. **Implementation**: Follow established patterns and standards
6. **Testing**: Implement comprehensive testing at all levels
7. **Accessibility**: Ensure full keyboard navigation and screen reader support
8. **Integration**: Test interaction with other forms and services

### Form Maintenance Standards
- **Documentation Currency**: Keep instruction files synchronized with implementation
- **Service Integration**: Maintain proper dependency injection and service usage
- **Error Handling**: Implement comprehensive error management
- **Performance**: Monitor and optimize form loading and operation times
- **User Feedback**: Incorporate user experience feedback into improvements

---

*This directory documents the complete form-level user interface of the MTM WIP Application, ensuring consistent implementation of modern, accessible, and maintainable form designs that integrate seamlessly with the overall application architecture.*