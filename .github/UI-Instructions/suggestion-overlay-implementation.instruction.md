# GitHub Copilot Instructions: Implementing SuggestionOverlayView.axaml

This comprehensive guide explains every step required to implement the MTM-styled SuggestionOverlayView.axaml component, including UI structure, styling patterns, ViewModel integration, and event handling.

<details>
<summary><strong>🎯 Component Overview</strong></summary>

The `SuggestionOverlayView` is a sophisticated overlay component that displays filtered suggestions when no exact match is found during user input. It features:

- **Modern MTM Design**: Purple-themed gradient header with rounded corners
- **Interactive List**: Selectable suggestions with Material Design icons
- **Keyboard Navigation**: Full keyboard support with Enter/Escape handling
- **Action Buttons**: Primary/secondary button styling with proper states
- **Event-Driven Architecture**: Clean separation between UI and business logic

**Key Features:**
- Three-panel layout (header, content, actions)
- Material Icons integration
- MTM theme brush integration
- Compiled bindings for performance
- Responsive design patterns

</details>

<details>
<summary><strong>🏗️ Step 1: AXAML Root Structure Setup</strong></summary>

### Namespace Declaration (CRITICAL for AVLN2000 Prevention)

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Views.SuggestionOverlayView"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.Overlay"
             xmlns:conv="using:MTM_WIP_Application_Avalonia.Converters"
             xmlns:materialIcons="using:Material.Icons.Avalonia"
             Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
             x:DataType="vm:SuggestionOverlayViewModel">
```

**Critical Points:**
- ✅ **Avalonia namespace**: `https://github.com/avaloniaui` (NOT WPF)
- ✅ **Compiled bindings**: `x:DataType` specified for performance
- ✅ **Material Icons**: Integrated for consistent iconography
- ✅ **Dynamic theming**: Background uses MTM theme resources

### Resource Declarations

```xml
<UserControl.Resources>
    <conv:NullToBoolConverter x:Key="NullToBoolConverter" />
</UserControl.Resources>
```

**Purpose**: Enables the Select button only when a suggestion is selected.

</details>

<details>
<summary><strong>🎨 Step 2: MTM Theme Styling Implementation</strong></summary>

### Action Button Base Style

```xml
<Style Selector="Button.action-button">
    <Setter Property="Padding" Value="8,6"/>
    <Setter Property="Margin" Value="3"/>
    <Setter Property="FontSize" Value="11"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="CornerRadius" Value="4"/>
    <Setter Property="MinHeight" Value="28"/>
    <Setter Property="Cursor" Value="Hand"/>
    <Setter Property="HorizontalContentAlignment" Value="Center"/>
    <Setter Property="VerticalContentAlignment" Value="Center"/>
    <Setter Property="BorderThickness" Value="1"/>
</Style>
```

**Design Philosophy**: Consistent sizing, typography, and interaction patterns across all buttons.

### Primary Button States

```xml
<Style Selector="Button.primary">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>
</Style>

<Style Selector="Button.primary:pointerover">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}"/>
</Style>

<Style Selector="Button.primary:pressed">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryPressedBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.PrimaryPressedBrush}"/>
</Style>
```

**Key Features:**
- **State Management**: Hover and pressed states for visual feedback
- **Theme Integration**: All colors from MTM theme resources
- **Accessibility**: Clear visual hierarchy and interaction cues

### Secondary Button States

```xml
<Style Selector="Button.secondary">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.InteractiveText}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderDarkBrush}"/>
</Style>

<Style Selector="Button.secondary:pointerover">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryHoverBrush}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.SecondaryHoverBrush}"/>
</Style>

<Style Selector="Button.secondary:pressed">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryPressedBrush}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.SecondaryPressedBrush}"/>
</Style>
```

**Visual Hierarchy**: Secondary buttons have subtle styling to support primary actions.

### Panel Styling System

```xml
<!-- Header Panel with Gradient -->
<Style Selector="Border.header-panel">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SidebarGradientBrush}"/>
    <Setter Property="CornerRadius" Value="12,12,0,0"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"/>
    <Setter Property="BorderThickness" Value="1,1,1,0"/>
</Style>

<!-- Main Content Panel -->
<Style Selector="Border.suggestion-panel">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"/>
    <Setter Property="BorderThickness" Value="1,0,1,0"/>
    <Setter Property="CornerRadius" Value="0"/>
    <Setter Property="Margin" Value="0,1,0,1"/>
</Style>

<!-- Action Panel -->
<Style Selector="Border.action-panel">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"/>
    <Setter Property="BorderThickness" Value="1,0,1,1"/>
    <Setter Property="CornerRadius" Value="0,0,12,12"/>
</Style>
```

**Design System**: Three distinct panel styles create visual hierarchy and modern card appearance.

</details>

<details>
<summary><strong>📋 Step 3: ListBox Styling for Suggestions</strong></summary>

### Container Styling

```xml
<Style Selector="ListBox">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="CornerRadius" Value="4"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}"/>
    <Setter Property="Padding" Value="4"/>
</Style>
```

### Item State Management

```xml
<Style Selector="ListBoxItem">
    <Setter Property="Padding" Value="12,8"/>
    <Setter Property="Margin" Value="2"/>
    <Setter Property="CornerRadius" Value="3"/>
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}"/>
</Style>

<Style Selector="ListBoxItem:selected">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SelectionBrush}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
</Style>

<Style Selector="ListBoxItem:pointerover">
    <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}"/>
</Style>
```

### Text Visibility Enforcement

```xml
<!-- Force TextBlocks in ListBoxItems to be visible -->
<Style Selector="ListBoxItem TextBlock">
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}"/>
    <Setter Property="Background" Value="Transparent"/>
</Style>

<Style Selector="ListBoxItem:selected TextBlock">
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
</Style>

<Style Selector="ListBoxItem:pointerover TextBlock">
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.BodyText}"/>
</Style>
```

**Critical Fix**: Ensures text remains visible across all selection states and themes.

### Material Icons Integration

```xml
<Style Selector="materialIcons|MaterialIcon">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.InteractiveText}"/>
    <Setter Property="HorizontalAlignment" Value="Center"/>
    <Setter Property="VerticalAlignment" Value="Center"/>
</Style>

<Style Selector="Button materialIcons|MaterialIcon">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="BorderThickness" Value="0"/>
</Style>
```

**Consistency**: Ensures icons maintain proper styling within buttons and throughout the component.

</details>

<details>
<summary><strong>🏛️ Step 4: Main Grid Layout Structure</strong></summary>

### Three-Row Layout Design

```xml
<Grid RowDefinitions="Auto,*,Auto" 
      HorizontalAlignment="Stretch" 
      VerticalAlignment="Stretch"
      Margin="4">
```

**Layout Philosophy:**
- **Row 0** (Auto): Header panel with title and close button
- **Row 1** (*): Expandable content area for suggestions list
- **Row 2** (Auto): Fixed action panel with buttons

**Key Features:**
- **Responsive**: Middle row expands to available space
- **Fixed Elements**: Header and footer maintain consistent heights
- **Margin**: 4px margin provides visual breathing room

</details>

<details>
<summary><strong>📑 Step 5: Header Panel Implementation</strong></summary>

### Header Structure

```xml
<Border Grid.Row="0" 
        Classes="header-panel"
        Padding="16,12">
    <Grid ColumnDefinitions="Auto,*,Auto">
        <materialIcons:MaterialIcon Grid.Column="0"
                                    Kind="Lightbulb"
                                    Width="20"
                                    Height="20"
                                    Margin="0,0,12,0"
                                    Foreground="{DynamicResource MTM_Shared_Logic.InteractiveText}"/>
        <TextBlock Grid.Column="1"
                   Text="Did you mean?"
                   FontWeight="Bold"
                   FontSize="16"
                   VerticalAlignment="Center"
                   Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"/>
        <materialIcons:MaterialIcon Grid.Column="2"
                                    Kind="Close"
                                    Width="16"
                                    Height="16"
                                    Foreground="{DynamicResource MTM_Shared_Logic.OverlayTextBrush}"
                                    Cursor="Hand"
                                    Tapped="OnCloseClicked"/>
    </Grid>
</Border>
```

**Design Elements:**
- **Icon Semantics**: Lightbulb icon suggests helpful suggestions
- **Typography**: Bold, 16px title for clear hierarchy
- **Interaction**: Hand cursor on close icon indicates clickability
- **Spacing**: 12px margin between icon and text for breathing room

**Event Handling**: Close icon uses `Tapped` event for cross-platform compatibility.

</details>

<details>
<summary><strong>📝 Step 6: Content Panel and Suggestions List</strong></summary>

### Content Panel Structure

```xml
<Border Grid.Row="1"
        Classes="suggestion-panel"
        Padding="16">
    <Grid RowDefinitions="Auto,*">
        <!-- Info text -->
        <TextBlock Grid.Row="0"
                   Text="No exact match found. Please select from similar items:"
                   FontSize="12"
                   Margin="0,0,0,12"
                   Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"/>

        <!-- Suggestions ListBox with modern styling -->
        <ListBox Grid.Row="1"
                 ItemsSource="{Binding Suggestions}"
                 SelectedItem="{Binding SelectedSuggestion, Mode=TwoWay}"
                 MinHeight="120"
                 FontSize="13"
                 x:Name="SuggestionListBox"
                 KeyDown="OnSuggestionListBoxKeyDown"
                 DoubleTapped="OnSuggestionListBoxDoubleTapped">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="8" Background="Transparent">
                        <materialIcons:MaterialIcon Kind="Package"
                                                    Width="14"
                                                    Height="14"
                                                    VerticalAlignment="Center"
                                                    Foreground="{DynamicResource MTM_Shared_Logic.InteractiveText}"/>
                        <TextBlock Text="{Binding}"
                                   VerticalAlignment="Center"
                                   Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"
                                   Background="Transparent"/>
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Grid>
</Border>
```

**Key Implementation Details:**

#### Binding Configuration
- **ItemsSource**: `{Binding Suggestions}` - Observable collection from ViewModel
- **SelectedItem**: `{Binding SelectedSuggestion, Mode=TwoWay}` - Enables button state management
- **x:Name**: Required for code-behind event handling

#### Event Handling
- **KeyDown**: Handles Enter/Escape keyboard navigation
- **DoubleTapped**: Provides quick selection alternative

#### DataTemplate Design
- **Package Icon**: Visual indicator for each suggestion item
- **Horizontal Layout**: Icon + text with 8px spacing
- **Transparent Backgrounds**: Prevents visual conflicts with selection states

#### Accessibility Features
- **MinHeight**: 120px ensures adequate touch targets
- **FontSize**: 13px for optimal readability
- **Margin**: 12px bottom spacing separates info text from list

</details>

<details>
<summary><strong>⚡ Step 7: Action Panel and Button Implementation</strong></summary>

### Action Panel Structure

```xml
<Border Grid.Row="2"
        Classes="action-panel"
        Padding="12">
    <Grid ColumnDefinitions="*,Auto">
        <!-- Left side - help text -->
        <StackPanel Grid.Column="0" 
                    Orientation="Horizontal" 
                    VerticalAlignment="Center" 
                    Spacing="6">
            <materialIcons:MaterialIcon Kind="Information"
                                        Width="14"
                                        Height="14"
                                        Foreground="{DynamicResource MTM_Shared_Logic.InteractiveText}"/>
            <TextBlock Text="Double-click to select, or use buttons"
                       FontSize="11"
                       Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"/>
        </StackPanel>

        <!-- Right side buttons -->
        <StackPanel Grid.Column="1" 
                    Orientation="Horizontal" 
                    HorizontalAlignment="Right" 
                    Spacing="8">
            <Button Classes="action-button secondary"
                    Content="Cancel" 
                    Command="{Binding CancelCommand}" 
                    Width="70"
                    Height="28">
            </Button>
            <Button x:Name="SelectButton"
                    Classes="action-button primary"
                    Command="{Binding SelectCommand}" 
                    Width="70"
                    Height="28"
                    IsEnabled="{Binding SelectedSuggestion, Converter={StaticResource NullToBoolConverter}}">
                <StackPanel Orientation="Horizontal" Spacing="6"
                            HorizontalAlignment="Center" VerticalAlignment="Center">
                    <materialIcons:MaterialIcon Kind="Check" Width="12" Height="12"/>
                    <TextBlock Text="Select" FontSize="11" FontWeight="SemiBold"/>
                </StackPanel>
            </Button>
        </StackPanel>
    </Grid>
</Border>
```

**Layout Strategy:**
- **Two-Column Grid**: Help text on left, buttons on right
- **Column Definitions**: `*,Auto` allows text to expand while keeping buttons fixed width
- **Spacing**: 8px between buttons for touch-friendly interaction

**Button Features:**

#### Cancel Button
- **Secondary Style**: Subtle appearance to de-emphasize destructive action
- **Fixed Dimensions**: 70x28px for consistent button sizing
- **Command Binding**: Direct binding to ViewModel command

#### Select Button  
- **Primary Style**: Prominent styling emphasizes primary action
- **State Management**: Enabled only when suggestion is selected
- **Composite Content**: Icon + text for clear action indication
- **Converter Usage**: `NullToBoolConverter` enables button based on selection state

#### Help Text Section
- **Information Icon**: Provides visual cue for instructional content
- **Small Typography**: 11px font size for secondary information
- **Spacing**: 6px between icon and text maintains visual rhythm

</details>

<details>
<summary><strong>🔗 Step 8: ViewModel Integration Patterns</strong></summary>

### Required ViewModel Structure

```csharp
public class SuggestionOverlayViewModel : BaseViewModel
{
    private ObservableCollection<string> _suggestions = new();
    private string? _selectedSuggestion;
    private bool _isVisible;
    private readonly RelayCommand _selectCommand;
    
    public ObservableCollection<string> Suggestions
    {
        get => _suggestions;
        set { _suggestions = value; OnPropertyChanged(); }
    }
    
    public string? SelectedSuggestion
    {
        get => _selectedSuggestion;
        set
        {
            if (_selectedSuggestion != value)
            {
                _selectedSuggestion = value;
                OnPropertyChanged();
                _selectCommand?.RaiseCanExecuteChanged();
            }
        }
    }
    
    public bool IsVisible
    {
        get => _isVisible;
        set { _isVisible = value; OnPropertyChanged(); }
    }
    
    public ICommand SelectCommand => _selectCommand;
    public ICommand CancelCommand { get; }
    
    public event Action<string>? SuggestionSelected;
    public event Action? Cancelled;
}
```

**Key Integration Points:**

#### Property Binding
- **Suggestions**: Observable collection automatically updates UI when modified
- **SelectedSuggestion**: Two-way binding enables selection tracking
- **IsVisible**: Controls overlay display state

#### Command Implementation
- **SelectCommand**: RelayCommand with CanExecute logic based on selection
- **CancelCommand**: Simple RelayCommand for dismissal
- **CanExecute Updates**: SelectedSuggestion setter triggers command state refresh

#### Event-Driven Communication
- **SuggestionSelected**: Fired when user confirms selection
- **Cancelled**: Fired when user dismisses overlay
- **Decoupled Design**: Parent components subscribe to events rather than direct coupling

</details>

<details>
<summary><strong>⌨️ Step 9: Code-Behind Event Handling</strong></summary>

### Required Event Handlers

```csharp
public partial class SuggestionOverlayView : UserControl
{
    public SuggestionOverlayView()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object? sender, TappedEventArgs e)
    {
        if (DataContext is SuggestionOverlayViewModel vm)
        {
            vm.CancelCommand.Execute(null);
        }
    }

    private void OnSuggestionListBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not SuggestionOverlayViewModel vm) return;

        switch (e.Key)
        {
            case Key.Enter:
                if (vm.SelectedSuggestion != null)
                {
                    vm.SelectCommand.Execute(null);
                }
                e.Handled = true;
                break;
                
            case Key.Escape:
                vm.CancelCommand.Execute(null);
                e.Handled = true;
                break;
        }
    }

    private void OnSuggestionListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is SuggestionOverlayViewModel vm && vm.SelectedSuggestion != null)
        {
            vm.SelectCommand.Execute(null);
        }
    }
}
```

**Event Handling Strategy:**

#### Type Safety
- **DataContext Casting**: Safely cast to ViewModel with null checks
- **Return Early**: Exit methods early if casting fails

#### Keyboard Navigation
- **Enter Key**: Executes selection if item is selected
- **Escape Key**: Cancels overlay regardless of selection state
- **Event Handling**: `e.Handled = true` prevents event bubbling

#### Mouse Interaction
- **Double-Click**: Provides alternative to button-based selection
- **Selection Validation**: Ensures suggestion is selected before executing

#### Command Execution
- **Consistent Pattern**: All interactions execute ViewModel commands
- **Null Safety**: Check command existence and parameters before execution

</details>

<details>
<summary><strong>🎨 Step 10: MTM Theme Integration</strong></summary>

### Required Theme Resources

The component relies on these MTM theme resources being available:

```xml
<!-- Background Resources -->
MTM_Shared_Logic.CardBackgroundBrush
MTM_Shared_Logic.PanelBackgroundBrush
MTM_Shared_Logic.SidebarGradientBrush

<!-- Text Resources -->
MTM_Shared_Logic.BodyText
MTM_Shared_Logic.InteractiveText
MTM_Shared_Logic.OverlayTextBrush

<!-- Action Resources -->
MTM_Shared_Logic.PrimaryAction
MTM_Shared_Logic.PrimaryHoverBrush
MTM_Shared_Logic.PrimaryPressedBrush
MTM_Shared_Logic.SecondaryHoverBrush
MTM_Shared_Logic.SecondaryPressedBrush

<!-- Border Resources -->
MTM_Shared_Logic.BorderAccentBrush
MTM_Shared_Logic.BorderDarkBrush

<!-- State Resources -->
MTM_Shared_Logic.SelectionBrush
MTM_Shared_Logic.HoverBackground
```

### Theme Integration Checklist

- [ ] **Dynamic Resources**: All colors use `{DynamicResource}` for theme switching
- [ ] **Consistent Naming**: Follow MTM_Shared_Logic.* naming convention
- [ ] **State Support**: Hover, pressed, and selection states defined
- [ ] **Accessibility**: Sufficient contrast ratios in all themes
- [ ] **Gradient Usage**: Header panel uses gradient for visual hierarchy

### Fallback Strategy

If theme resources are unavailable, provide fallback colors:

```xml
<UserControl.Resources>
    <!-- Fallback colors if theme resources are missing -->
    <SolidColorBrush x:Key="FallbackPrimary" Color="#6a0dad"/>
    <SolidColorBrush x:Key="FallbackBackground" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="FallbackText" Color="#333333"/>
</UserControl.Resources>
```

</details>

<details>
<summary><strong>🧪 Step 11: Testing and Validation</strong></summary>

### UI Testing Checklist

#### Visual Validation
- [ ] **Header gradient** displays correctly across themes
- [ ] **Button states** show appropriate hover/pressed feedback
- [ ] **List selection** highlights correctly with theme colors
- [ ] **Text visibility** maintained across all selection states
- [ ] **Icons** display consistently throughout component

#### Interaction Testing
- [ ] **Keyboard navigation** works with Enter/Escape keys
- [ ] **Double-click selection** executes successfully
- [ ] **Button enabling** responds to selection state changes
- [ ] **Close button** properly dismisses overlay
- [ ] **Mouse hover** states transition smoothly

#### Data Binding Validation
- [ ] **Suggestions collection** updates UI when modified
- [ ] **Selected item** synchronizes between list and ViewModel
- [ ] **Command states** update when selection changes
- [ ] **Event firing** occurs for all user actions

#### Responsive Design Testing
- [ ] **Minimum heights** maintained for touch targets
- [ ] **Content overflow** handled gracefully
- [ ] **Button sizing** consistent across different content lengths
- [ ] **Panel spacing** maintains visual hierarchy

### Performance Validation

#### Memory Management
- [ ] **Event subscriptions** properly disposed when component is removed
- [ ] **Observable collections** cleared when no longer needed
- [ ] **Command references** released appropriately

#### Rendering Performance
- [ ] **Compiled bindings** enabled with x:DataType
- [ ] **Style selectors** optimized for fast lookup
- [ ] **Resource references** use appropriate StaticResource vs DynamicResource

</details>

<details>
<summary><strong>📚 Step 12: Integration Patterns</strong></summary>

### Parent Component Integration

```csharp
// In parent ViewModel
public SuggestionOverlayViewModel SuggestionOverlay { get; private set; }

private void ShowSuggestions(IEnumerable<string> suggestions)
{
    SuggestionOverlay = new SuggestionOverlayViewModel(suggestions);
    SuggestionOverlay.SuggestionSelected += OnSuggestionSelected;
    SuggestionOverlay.Cancelled += OnSuggestionCancelled;
    SuggestionOverlay.IsVisible = true;
}

private void OnSuggestionSelected(string selectedSuggestion)
{
    // Handle selected suggestion
    ProcessSelectedItem(selectedSuggestion);
    
    // Clean up
    DisposeOverlay();
}

private void OnSuggestionCancelled()
{
    // Handle cancellation
    ResetInputState();
    
    // Clean up
    DisposeOverlay();
}

private void DisposeOverlay()
{
    if (SuggestionOverlay != null)
    {
        SuggestionOverlay.SuggestionSelected -= OnSuggestionSelected;
        SuggestionOverlay.Cancelled -= OnSuggestionCancelled;
        SuggestionOverlay = null;
    }
}
```

### XAML Integration

```xml
<!-- In parent view -->
<Grid>
    <!-- Main content -->
    <ContentControl Content="{Binding MainContent}"/>
    
    <!-- Overlay -->
    <ContentControl Content="{Binding SuggestionOverlay}"
                    IsVisible="{Binding SuggestionOverlay.IsVisible}"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center"
                    ZIndex="100"/>
</Grid>
```

### Service Integration

```csharp
// In search service
public async Task<SuggestionResult> SearchWithSuggestionsAsync(string query)
{
    var exactMatch = await FindExactMatchAsync(query);
    if (exactMatch != null)
    {
        return new SuggestionResult { ExactMatch = exactMatch };
    }
    
    var suggestions = await FindSimilarItemsAsync(query);
    return new SuggestionResult { Suggestions = suggestions };
}

public class SuggestionResult
{
    public string? ExactMatch { get; set; }
    public IEnumerable<string> Suggestions { get; set; } = Array.Empty<string>();
    public bool HasSuggestions => Suggestions.Any();
}
```

</details>

<details>
<summary><strong>🔧 Step 13: Common Implementation Issues and Solutions</strong></summary>

### AVLN2000 Error Prevention

**Issue**: Grid ColumnDefinition with Name attribute
```xml
<!-- WRONG -->
<Grid.ColumnDefinitions>
    <ColumnDefinition Name="Icon" Width="Auto"/>
</Grid.ColumnDefinitions>

<!-- CORRECT -->
<Grid ColumnDefinitions="Auto,*,Auto">
```

**Issue**: WPF namespace usage
```xml
<!-- WRONG -->
xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"

<!-- CORRECT -->
xmlns="https://github.com/avaloniaui"
```

### Binding Issues

**Issue**: Missing x:DataType for compiled bindings
```xml
<!-- Incomplete -->
<UserControl x:CompileBindings="True">

<!-- Complete -->
<UserControl x:CompileBindings="True" 
             x:DataType="vm:SuggestionOverlayViewModel">
```

**Issue**: Incorrect converter usage
```xml
<!-- WRONG -->
IsEnabled="{Binding SelectedSuggestion, Converter={StaticResource NotNullConverter}}"

<!-- CORRECT -->
IsEnabled="{Binding SelectedSuggestion, Converter={StaticResource NullToBoolConverter}}"
```

### Event Handling Issues

**Issue**: Missing null checks in event handlers
```csharp
// Unsafe
private void OnKeyDown(object sender, KeyEventArgs e)
{
    var vm = (SuggestionOverlayViewModel)DataContext;
    vm.SelectCommand.Execute(null);
}

// Safe
private void OnKeyDown(object sender, KeyEventArgs e)
{
    if (DataContext is SuggestionOverlayViewModel vm && vm.SelectedSuggestion != null)
    {
        vm.SelectCommand.Execute(null);
    }
}
```

### Styling Issues

**Issue**: Missing Material Icons namespace
```xml
<!-- Missing namespace declaration -->
<materialIcons:MaterialIcon Kind="Close"/>

<!-- Add to UserControl -->
xmlns:materialIcons="using:Material.Icons.Avalonia"
```

**Issue**: Theme resource not found
```xml
<!-- Handle missing resources -->
<Setter Property="Background" 
        Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush, 
                FallbackValue=White}"/>
```

</details>

<details>
<summary><strong>📋 Step 14: Implementation Checklist</strong></summary>

### Pre-Implementation
- [ ] **Review AVLN2000 prevention guide** - [avalonia-xaml-syntax.instruction.md](avalonia-xaml-syntax.instruction.md)
- [ ] **Verify MTM theme resources** are available in application
- [ ] **Confirm Material Icons package** is installed and configured
- [ ] **Check ViewModel dependencies** (BaseViewModel, ICommand implementations)

### AXAML Implementation
- [ ] **Root UserControl** with correct Avalonia namespace
- [ ] **Compiled bindings** enabled with x:DataType
- [ ] **Resource declarations** for converters and local resources
- [ ] **Style definitions** for all button states and list styling
- [ ] **Grid layout** with three-row structure (Auto,*,Auto)
- [ ] **Header panel** with gradient styling and Material Icons
- [ ] **Content panel** with suggestions list and proper DataTemplate
- [ ] **Action panel** with help text and styled buttons

### Code-Behind Implementation
- [ ] **Event handler methods** for keyboard navigation and mouse interaction
- [ ] **Null safety checks** in all event handlers
- [ ] **Command execution** through ViewModel commands
- [ ] **Event marking** with e.Handled = true where appropriate

### ViewModel Integration
- [ ] **Property bindings** for Suggestions, SelectedSuggestion, and IsVisible
- [ ] **Command bindings** for SelectCommand and CancelCommand
- [ ] **Event declarations** for SuggestionSelected and Cancelled
- [ ] **Command state management** with CanExecute logic

### Testing and Validation
- [ ] **Build successfully** without AVLN2000 errors
- [ ] **Visual appearance** matches MTM design standards
- [ ] **Keyboard interaction** works correctly (Enter/Escape)
- [ ] **Mouse interaction** supports double-click selection
- [ ] **Button states** enable/disable based on selection
- [ ] **Theme integration** works across different themes
- [ ] **Event firing** occurs for all user actions

### Integration Testing
- [ ] **Parent component** can create and display overlay
- [ ] **Event subscription** and cleanup works properly
- [ ] **Data flow** from parent to overlay functions correctly
- [ ] **Overlay dismissal** cleans up resources appropriately

</details>

<details>
<summary><strong>🎯 Step 15: Advanced Customization Options</strong></summary>

### Dynamic Suggestion Sources

```csharp
// Support for different suggestion types
public interface ISuggestionProvider
{
    Task<IEnumerable<string>> GetSuggestionsAsync(string query);
    string FormatSuggestion(string suggestion);
}

// In ViewModel
public async Task LoadSuggestionsAsync(string query, ISuggestionProvider provider)
{
    var suggestions = await provider.GetSuggestionsAsync(query);
    Suggestions.Clear();
    foreach (var suggestion in suggestions)
    {
        Suggestions.Add(provider.FormatSuggestion(suggestion));
    }
}
```

### Custom DataTemplate Support

```xml
<!-- Extended DataTemplate with additional metadata -->
<ListBox.ItemTemplate>
    <DataTemplate DataType="{x:Type local:SuggestionItem}">
        <StackPanel Orientation="Horizontal" Spacing="8">
            <materialIcons:MaterialIcon Kind="{Binding IconKind}"
                                        Width="14" Height="14"/>
            <StackPanel Orientation="Vertical">
                <TextBlock Text="{Binding PrimaryText}" FontWeight="SemiBold"/>
                <TextBlock Text="{Binding SecondaryText}" FontSize="10" 
                          Opacity="0.7"/>
            </StackPanel>
        </StackPanel>
    </DataTemplate>
</ListBox.ItemTemplate>
```

### Performance Optimization

```csharp
// Virtualized scrolling for large suggestion lists
<ListBox VirtualizationMode="Standard"
         ScrollViewer.HorizontalScrollBarVisibility="Disabled"
         ScrollViewer.VerticalScrollBarVisibility="Auto">
```

### Accessibility Enhancements

```xml
<!-- Screen reader support -->
<ListBox AutomationProperties.Name="Suggestion List"
         AutomationProperties.HelpText="Select a suggestion using arrow keys and Enter">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding}"
                       AutomationProperties.Name="{Binding}"/>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

</details>

---

This comprehensive guide provides complete implementation details for the SuggestionOverlayView.axaml component, following MTM design standards and Avalonia best practices. Each step includes code examples, design rationale, and integration patterns necessary for successful implementation.
