<!-- Copilot: Reading ui-generation.instruction.md — UI Generation Guidelines -->
# UI Generation Guidelines (Avalonia)

## Basic UI Generation Rules

- When creating UI elements, always use both:
  - The referenced `.instructions.md` file for control names, types, and event definitions.
  - The mapped screenshot in `UI_Winform_Screenshots` for layout, sizing, and style (see [UI Mapping Reference](./ui-mapping.instruction.md)).

- If Markdown and screenshot disagree, prioritize the screenshot for layout, but preserve control names/events from the Markdown.

- Code generation:
  - `.axaml` and `.axaml.cs` files are used for Avalonia layouts and logic.
  - Keep all layout, control initialization, and visual design in `.axaml`.
  - `.axaml.cs` file contains only:
    - The constructor
    - Navigation event handlers
    - Empty stubs for all other events (with TODO)
    - No business logic

- Adhere to the [Naming Conventions](../Core-Instructions/naming.conventions.instruction.md) and [Coding Conventions](../Core-Instructions/codingconventions.instruction.md).

- Checklist:
  - [ ] Use control names, events, and types from `.instructions.md`.
  - [ ] Match layout and style to screenshot.
  - [ ] Navigation logic only (no business logic).
  - [ ] Empty event stubs for all events.
  - [ ] Add TODO comments for business logic.

---

## UI Generation from Markdown Instructions

### When asked to "Create UI Element from {filename}.md":

1. **Parse the markdown file** to extract:
   - UI Element Name
   - Component Structure
   - Props/Inputs
   - Visual Representation
   - Related controls

2. **Generate the following files**:
   - `Views/{Name}View.axaml` - The Avalonia UI markup
   - `ViewModels/{Name}ViewModel.cs` - The ViewModel using ReactiveUI
   - Only UI structure and bindings, NO business logic implementation

3. **Follow these patterns**:

---

## AXAML View Template

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:YourApp.ViewModels"
             mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
             x:Class="YourApp.Views.{Name}View"
             x:DataType="vm:{Name}ViewModel"
             x:CompileBindings="True">
    <!-- UI Elements here -->
</UserControl>
```

---

## ViewModel Template (ReactiveUI)

```csharp
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace YourApp.ViewModels;

public class {Name}ViewModel : ReactiveObject
{
    // Observable properties
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    // Commands
    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public {Name}ViewModel()
    {
        // Async command
        LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await Task.CompletedTask; // TODO: Implement
        });

        // Sync command with CanExecute
        var canSave = this.WhenAnyValue(vm => vm.Title, t => !string.IsNullOrWhiteSpace(t));
        SaveCommand = ReactiveCommand.Create(() =>
        {
            // TODO: Implement
        }, canSave);

        // Error handling pattern
        LoadDataCommand.ThrownExceptions
            .Merge(SaveCommand.ThrownExceptions)
            .Subscribe(ex =>
            {
                // TODO: Log and present user-friendly error
            });
    }
}
```

---

## MTM-Specific UI Generation Guidelines

### Component Hierarchy Mapping
When parsing MD files with component structures like:
```
Control_QuickButtons
├── quickButtons List<Button> (10 buttons maximum)
│   ├── Button[0] - Position 1: (Operation) - [PartID x Quantity]
│   └── Button[9] - Position 10: (Operation) - [PartID x Quantity]
└── Context Menu (Right-click)
    ├── Edit Button
    └── Remove Button
```

Generate Avalonia structure:
```xml
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate DataType="vm:QuickButtonItemViewModel">
            <Button Classes="quick-button">
                <Button.ContextMenu>
                    <ContextMenu>
                        <MenuItem Header="Edit Button"/>
                        <MenuItem Header="Remove Button"/>
                    </ContextMenu>
                </Button.ContextMenu>
            </Button>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Business Logic Integration Points
When MD files mention database operations or business logic:
```csharp
// In ViewModel - Leave as TODO comments
// TODO: Implement database loading
// var dataResult = await Helper_Database_StoredProcedure.ExecuteDataTableWithStatus(
//     Model_AppVariables.ConnectionString,
//     "sys_last_10_transactions_Get_ByUser",
//     new Dictionary<string, object> { ["User"] = currentUser }
// );
```

### Event-Driven Communication Patterns
For inter-component communication described in MD files:
```csharp
// Events for parent-child communication
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct method calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation,
    Quantity = button.Quantity
});
```

### MTM Data Patterns
Operations in MTM are typically numbers, not actions:
- **Part ID**: String (e.g., "PART001")
- **Operation**: String number (e.g., "90", "100", "110")
- **Quantity**: Integer
- **Position**: 1-based indexing for UI display

### Context Menu Integration
For components with management features, prefer context menus over separate buttons:
```xml
<Button.ContextMenu>
    <ContextMenu>
        <MenuItem Header="Edit Button" Command="{Binding EditCommand}"/>
        <MenuItem Header="Remove Button" Command="{Binding RemoveCommand}"/>
        <Separator/>
        <MenuItem Header="Clear All" Command="{Binding ClearAllCommand}"/>
        <MenuItem Header="Refresh" Command="{Binding RefreshCommand}"/>
    </ContextMenu>
</Button.ContextMenu>
```

### Space Optimization Patterns
When removing UI elements, optimize space usage:
- Use `UniformGrid` for equal distribution
- Implement `VerticalAlignment="Stretch"` for full height usage
- Remove `ScrollViewer` when all items fit in available space
- Increase font sizes and padding when more space is available

### Quick Button Specific Patterns
For button collections that populate other controls:
```csharp
// Simple field population, no tab switching
private async Task ExecuteQuickActionAsync(QuickButtonItemViewModel button)
{
    QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
    {
        PartId = button.PartId,
        Operation = button.Operation, // Just a number
        Quantity = button.Quantity
    });
}
```

### Progress Integration Patterns
When MD mentions progress tracking:
```csharp
// TODO: Inject services
// private readonly IProgressService _progressService;

// Commands with progress support
LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
{
    // TODO: Show progress during operation
    await LoadLast10TransactionsAsync();
});
```

---

## Default Color Scheme
The MTM WIP Application uses a modern purple-based color scheme with vibrant accent colors:

### Primary Color Palette
```
/* Color Theme Swatches in Hex */
Primary Purple:     #4B45ED (rgba(75, 69, 237, 1))    - Deep purple blue
Magenta Accent:     #BA45ED (rgba(186, 69, 237, 1))   - Bright magenta
Secondary Purple:   #8345ED (rgba(131, 69, 237, 1))   - Mid-tone purple  
Blue Accent:        #4574ED (rgba(69, 116, 237, 1))   - Purple blue
Pink Accent:        #ED45E7 (rgba(237, 69, 231, 1))   - Bright pink
Light Purple:       #B594ED (rgba(181, 148, 237, 1))  - Light purple

/* Color Theme Swatches in HSLA for precise control */
Primary Purple:     hsla(242, 82%, 60%, 1)
Magenta Accent:     hsla(281, 82%, 60%, 1)
Secondary Purple:   hsla(262, 82%, 60%, 1)
Blue Accent:        hsla(223, 82%, 60%, 1)
Pink Accent:        hsla(302, 82%, 60%, 1)
Light Purple:       hsla(262, 71%, 75%, 1)
```

### Color Usage Guidelines
- **Primary Purple** (#4B45ED): Main accent color for buttons, highlights, and active states
- **Magenta Accent** (#BA45ED): Secondary accent for hover states and notifications
- **Secondary Purple** (#8345ED): Background accents and subtle highlights
- **Blue Accent** (#4574ED): Navigation elements and information states
- **Pink Accent** (#ED45E7): Warning states and special actions
- **Light Purple** (#B594ED): Disabled states and subtle backgrounds

### Avalonia Resource Mapping
```xml
<!-- In App.axaml or theme resources -->
<SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="AccentBrush" Color="#4B45ED"/>
<SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>
<SolidColorBrush x:Key="MagentaAccentBrush" Color="#BA45ED"/>
<SolidColorBrush x:Key="BlueAccentBrush" Color="#4574ED"/>
<SolidColorBrush x:Key="PinkAccentBrush" Color="#ED45E7"/>
<SolidColorBrush x:Key="LightPurpleBrush" Color="#B594ED"/>

<!-- Hover and interaction states -->
<SolidColorBrush x:Key="AccentHoverBrush" Color="#BA45ED"/>
<SolidColorBrush x:Key="AccentPressedBrush" Color="#8345ED"/>
<SolidColorBrush x:Key="AccentDisabledBrush" Color="#B594ED"/>

<!-- Gradient brushes for backgrounds -->
<LinearGradientBrush x:Key="PrimaryGradientBrush" StartPoint="0,0" EndPoint="1,0">
    <GradientStop Color="#4B45ED" Offset="0"/>
    <GradientStop Color="#BA45ED" Offset="0.5"/>
    <GradientStop Color="#8345ED" Offset="1"/>
</LinearGradientBrush>

<!-- Card and background gradients -->
<LinearGradientBrush x:Key="HeroGradientBrush" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#4574ED" Offset="0"/>
    <GradientStop Color="#4B45ED" Offset="0.3"/>
    <GradientStop Color="#8345ED" Offset="0.7"/>
    <GradientStop Color="#BA45ED" Offset="1"/>
</LinearGradientBrush>
```

---

## Avalonia UI Guidelines

### Modern Application Layout (MainWindow.axaml)
The main application window should follow a modern sidebar + content pattern:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="YourApp.MainWindow"
        Title="MTM WIP Application"
        Width="1200" Height="700">
    
    <Grid ColumnDefinitions="240,*">
        <!-- Sidebar Navigation -->
        <Border Grid.Column="0" 
                Background="{DynamicResource SidebarBackgroundBrush}"
                BoxShadow="1 0 3 0 #22000000">
            <DockPanel>
                <!-- App Header/Logo -->
                <Border DockPanel.Dock="Top" 
                        Padding="16" 
                        Height="60">
                    <TextBlock Text="MTM WIP System" 
                               FontSize="18" 
                               FontWeight="SemiBold"
                               VerticalAlignment="Center"/>
                </Border>
                
                <!-- Navigation Items -->
                <ScrollViewer>
                    <StackPanel Spacing="2" Margin="8">
                        <!-- Navigation sections with icons and nested items -->
                    </StackPanel>
                </ScrollViewer>
            </DockPanel>
        </Border>
        
        <!-- Main Content Area -->
        <Grid Grid.Column="1" Background="{DynamicResource ContentBackgroundBrush}">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>
            
            <!-- Content Header -->
            <Border Grid.Row="0" 
                    Background="{DynamicResource CardBackgroundBrush}"
                    Padding="24,16"
                    BoxShadow="0 1 3 0 #11000000">
                <Grid>
                    <!-- Breadcrumb or page title -->
                </Grid>
            </Border>
            
            <!-- Main Content -->
            <ScrollViewer Grid.Row="1" Padding="24">
                <ContentControl Content="{Binding CurrentView}"/>
            </ScrollViewer>
            
            <!-- Status Bar -->
            <Border Grid.Row="2" 
                    Background="{DynamicResource StatusBarBackgroundBrush}"
                    Padding="16,8">
                <Grid ColumnDefinitions="*,Auto,Auto">
                    <!-- Status information -->
                </Grid>
            </Border>
        </Grid>
    </Grid>
</Window>
```

### Navigation Sidebar Pattern
```xml
<!-- Expandable Navigation Group -->
<Expander Header="Inventory" IsExpanded="True">
    <StackPanel Spacing="2" Margin="24,4,0,4">
        <RadioButton GroupName="Navigation" 
                     Classes="nav-item"
                     Content="View Inventory"
                     Command="{Binding NavigateCommand}"
                     CommandParameter="Inventory"/>
        <RadioButton GroupName="Navigation"
                     Classes="nav-item" 
                     Content="Add Items"
                     Command="{Binding NavigateCommand}"
                     CommandParameter="AddItems"/>
    </StackPanel>
</Expander>

<!-- Style for navigation items -->
<Style Selector="RadioButton.nav-item">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Padding" Value="12,8"/>
    <Setter Property="CornerRadius" Value="6"/>
</Style>
<Style Selector="RadioButton.nav-item:checked">
    <Setter Property="Background" Value="{DynamicResource AccentBrush}"/>
    <Setter Property="Foreground" Value="White"/>
</Style>
```

### Card-Based Content Layout
```xml
<!-- Feature Card Pattern -->
<Border Classes="card" Padding="24" Margin="0,0,0,16">
    <Grid RowDefinitions="Auto,16,Auto,24,*">
        <!-- Card Header with Icon -->
        <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
            <PathIcon Grid.Column="0" 
                      Data="{StaticResource IconData}"
                      Width="24" Height="24"
                      Foreground="{DynamicResource AccentBrush}"/>
            <TextBlock Grid.Column="2" 
                       Text="Card Title"
                       FontSize="20"
                       FontWeight="SemiBold"/>
        </Grid>
        
        <!-- Card Description -->
        <TextBlock Grid.Row="2" 
                   Text="Card description text"
                   Opacity="0.8"
                   TextWrapping="Wrap"/>
        
        <!-- Card Content -->
        <Grid Grid.Row="4">
            <!-- Actual content -->
        </Grid>
    </Grid>
</Border>

<!-- Card Styles -->
<Style Selector="Border.card">
    <Setter Property="Background" Value="{DynamicResource CardBackgroundBrush}"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="BoxShadow" Value="0 2 8 0 #11000000"/>
</Style>
```

### Hero/Banner Section Pattern
```xml
<!-- Hero Banner with MTM Purple Gradient -->
<Border CornerRadius="12" 
        ClipToBounds="True"
        Height="200"
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
            <TextBlock Text="Welcome to MTM WIP System"
                       FontSize="28"
                       FontWeight="Bold"
                       Foreground="White"/>
            <TextBlock Text="Manage your inventory efficiently"
                       FontSize="16"
                       Foreground="White"
                       Opacity="0.9"/>
        </StackPanel>
    </Grid>
</Border>
```

### Layout Principles
- Use **Grid** for complex layouts with rows/columns
- Use **DockPanel** for toolbar/statusbar layouts
- Use **StackPanel** sparingly, prefer Grid for performance
- Default margins: `Margin="8"` for containers, `Margin="4"` for controls
- Default padding: `Padding="8"` for content areas
- Card padding: `Padding="24"` for spacious card content
- Ensure spacing isn't squished - use adequate margins between elements
- Use `Spacing` property on StackPanel for consistent gaps

### Modern UI Elements
- **Cards**: Rounded corners (8-12px), subtle shadows, white/light background
- **Sidebar**: Fixed width (240-280px), slightly darker background, clear hierarchy
- **Navigation Items**: Use RadioButtons or ToggleButtons for single selection
- **Headers**: Larger font sizes (20-28px), semi-bold or bold weight
- **Shadows**: Subtle box shadows for depth `BoxShadow="0 2 8 0 #11000000"`
- **Icons**: Use PathIcon or Avalonia.Icons packages, 24x24 for standard size
- **Gradients**: Use MTM brand gradient for hero sections and call-to-action areas

---

## Control Bindings

### Always use compiled bindings
```xml
<!-- Always include these attributes in your UserControl/Window -->
x:DataType="vm:{Name}ViewModel"
x:CompileBindings="True"
```

### Binding Patterns
- Use `{Binding PropertyName}` for one-way bindings
- Use `{Binding PropertyName, Mode=TwoWay}` for input controls
- Commands: bind to ReactiveCommand (implements ICommand)

### Common Controls Mapping (WinForms → Avalonia)
- `Form` → `Window` or `UserControl`
- `TableLayoutPanel` → `Grid` with RowDefinitions/ColumnDefinitions
- `SplitContainer` → `Grid` with `GridSplitter`
- `TabControl` → `TabControl` with `TabItem`
- `MenuStrip` → `Menu` with `MenuItem`
- `StatusStrip` → `DockPanel` with `TextBlock` at bottom
- `ProgressBar` → `ProgressBar`
- `Label` → `TextBlock` or `Label`
- `TextBox` → `TextBox`
- `Button` → `Button`
- `ComboBox` → `ComboBox`
- `DataGridView` → `DataGrid`

---

## ReactiveUI Patterns

### Properties
```csharp
public class SampleViewModel : ReactiveObject
{
    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set => this.RaiseAndSetIfChanged(ref _firstName, value);
    }

    // Derived property using OAPH (ObservableAsPropertyHelper)
    private readonly ObservableAsPropertyHelper<string> _fullName;
    public string FullName => _fullName.Value;

    public SampleViewModel()
    {
        _fullName = this.WhenAnyValue(vm => vm.FirstName)
                        .Select(fn => $"Name: {fn}")
                        .ToProperty(this, vm => vm.FullName, initialValue: string.Empty);
    }
}
```

### Commands
```csharp
public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public SampleViewModel()
{
    LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        await Task.CompletedTask;
    });

    var canSave = this.WhenAnyValue(vm => vm.FirstName, s => !string.IsNullOrWhiteSpace(s));
    SaveCommand = ReactiveCommand.Create(() => { /* ... */ }, canSave);

    // Centralized error handling
    LoadDataCommand.ThrownExceptions
        .Merge(SaveCommand.ThrownExceptions)
        .Subscribe(ex =>
        {
            // TODO: Log and present user-friendly message
        });
}
```

### Collections
```csharp
public ObservableCollection<ItemViewModel> Items { get; } = new();
```

---

## Styling Guidelines

### Default spacing for containers
```xml
<Grid Margin="8" RowDefinitions="Auto,*,Auto" ColumnDefinitions="*,2*">
    <!-- Content with proper spacing -->
</Grid>
```

### Modern Card
```xml
<Border Padding="24" 
        Margin="0,0,0,16" 
        CornerRadius="8"
        Background="{DynamicResource CardBackgroundBrush}"
        BoxShadow="0 2 8 0 #11000000">
    <!-- Content -->
</Border>
```

### Button Styles with MTM Colors
```xml
<Button Classes="primary" Content="Primary Action"/>
<Button Classes="secondary" Content="Secondary Action"/>

<!-- Primary Button Style using MTM Purple -->
<Style Selector="Button.primary">
    <Setter Property="Background" Value="#4B45ED"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Padding" Value="16,8"/>
    <Setter Property="CornerRadius" Value="6"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
</Style>
<Style Selector="Button.primary:pointerover">
    <Setter Property="Background" Value="#BA45ED"/>
</Style>
<Style Selector="Button.primary:pressed">
    <Setter Property="Background" Value="#8345ED"/>
</Style>
```

---

## Common UI Patterns

### Main Window with Sidebar Navigation
```xml
<Grid ColumnDefinitions="240,*">
    <!-- Sidebar -->
    <Border Grid.Column="0" Background="{DynamicResource SidebarBackgroundBrush}">
        <!-- Navigation content -->
    </Border>
    
    <!-- Main Content -->
    <Grid Grid.Column="1">
        <!-- Page content -->
    </Grid>
</Grid>
```

### Tab Control
```xml
<TabControl>
    <TabItem Header="Inventory">
        <views:InventoryView />
    </TabItem>
    <TabItem Header="Remove">
        <views:RemoveView />
    </TabItem>
</TabControl>
```

### Form Layout
```xml
<Grid RowDefinitions="Auto,Auto,Auto,*,Auto" ColumnDefinitions="Auto,*" Margin="8">
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Label:" Margin="4" VerticalAlignment="Center"/>
    <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding Property}" Margin="4"/>
    <!-- More form fields -->
</Grid>
```

### Grid of Cards
```xml
<ScrollViewer>
    <ItemsControl Items="{Binding Cards}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <WrapPanel Orientation="Horizontal"/>
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <Border Classes="card" Width="300" Margin="12">
                    <!-- Card content -->
                </Border>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</ScrollViewer>
```

---

## Code Generation Rules

1. **Keep code-behind minimal** - only use for view-specific logic that can't be in ViewModel
2. **Use async/await** for any operation that isn't directly UI-related
3. **Use dependency injection** - prepare constructors for DI even if not implementing services
4. **No business logic** in generated UI code - only structure and bindings
5. **Include placeholders** for service injection:
   ```csharp
   // TODO: Inject services
   // private readonly IDataService _dataService;
   ```

---

## Error Handling Pattern
```csharp
public ReactiveCommand<Unit, Unit> PerformOperationCommand { get; }

public SampleViewModel()
{
    PerformOperationCommand = ReactiveCommand.CreateFromTask(async () =>
    {
        // Operation
        await Task.CompletedTask;
    });

    PerformOperationCommand.ThrownExceptions
        .Subscribe(ex =>
        {
            // TODO: Log to MySQL and file
            // await _errorService.LogErrorAsync(ex);
            // Show user-friendly message
        });
}
```

---

## Special Instructions

### When Creating from MD Files
1. **Focus on structure** - Create the visual hierarchy exactly as described
2. **Map controls** - Convert WinForms controls to Avalonia equivalents
3. **Create bindings** - Set up all properties as observable with proper bindings
4. **Add commands** - Create command stubs for all interactions mentioned
5. **Skip implementation** - Leave business logic as TODO comments
6. **Preserve relationships** - Maintain parent-child control relationships

### Example Conversion
If MD file shows:
```
├── MainForm_TabControl (TabControl)
│   ├── Inventory Tab → Control_InventoryTab
```

Generate:
```xml
<TabControl x:Name="MainTabControl">
    <TabItem Header="Inventory">
        <views:InventoryTabView />
    </TabItem>
</TabControl>
```

And in ViewModel:
```csharp
private int _selectedTabIndex;
public int SelectedTabIndex
{
    get => _selectedTabIndex;
    set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
}

public ReactiveCommand<Unit, Unit> OnTabSelectionChangedCommand { get; }

public SampleViewModel()
{
    OnTabSelectionChangedCommand = ReactiveCommand.Create(() =>
    {
        // TODO: Handle tab change logic
    });
}
```

---

## Theme System Preparation
- Use `{DynamicResource ResourceKey}` for colors and brushes
- Prepare for theme switching by avoiding hard-coded colors
- Common theme resources to expect:
  - `PrimaryBrush` (#4B45ED), `SecondaryBrush` (#8345ED), `AccentBrush` (#4B45ED)
  - `MagentaAccentBrush` (#BA45ED), `BlueAccentBrush` (#4574ED), `PinkAccentBrush` (#ED45E7)
  - `LightPurpleBrush` (#B594ED) for disabled states
  - `BackgroundBrush`, `ForegroundBrush`
  - `CardBackgroundBrush`, `BorderBrush`
  - `SidebarBackgroundBrush`, `ContentBackgroundBrush`
  - `StatusBarBackgroundBrush`
  - `PrimaryGradientBrush`, `HeroGradientBrush` (MTM purple gradients)

---

## Testing Helpers
- Generate ViewModels with parameterless constructors for design-time support
- Include `Design.DataContext` in AXAML for design-time data
- Make properties virtual for mocking when needed

---

## Error UI Elements

To present errors consistently, prefer using the prebuilt components:
- Inline: `Controls/Control_ErrorMessage` — non-blocking control with severity styling, details expander, copy, retry, report buttons.
- Modal: `Views/ErrorDialog_Enhanced` — dialog with tabs for message, technical details, and context data.

Integration examples are available in:
- `Examples/ErrorMessageUIUsageExample.cs`

> Important: UI components must not implement business logic. Use `Service_ErrorHandler` and related services from the Services layer for logging and categorization.

---

## Remember
- This is an Avalonia app, not WPF or WinForms
- Use Avalonia-specific syntax and controls
- Follow MVVM pattern strictly with ReactiveUI
- Keep views and ViewModels paired and consistently named
- Generate clean, readable code with proper spacing
- Add XML comments only where helpful for understanding UI purpose
- Follow modern UI patterns with cards, sidebars, and clean layouts
- Use ReactiveUI's reactive programming paradigms (WhenAnyValue, OAPH, etc.)
- Apply the MTM brand gradient and color scheme consistently throughout the application