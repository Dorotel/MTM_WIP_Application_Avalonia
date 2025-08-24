# Views Directory

This directory contains all Avalonia AXAML view files that define the user interface for the MTM WIP Application Avalonia.

## ?? View Architecture

### Avalonia UI with MVVM
All views follow Avalonia UI patterns with MVVM data binding:
- **AXAML Markup**: Declarative UI definition using XAML-like syntax
- **Compiled Bindings**: Type-safe bindings with compile-time validation
- **ReactiveUI Integration**: Seamless integration with ReactiveUI ViewModels
- **Modern Design**: Clean, responsive design following MTM brand guidelines

## ?? View Files

### Core Views

#### `MainView.axaml` / `MainView.axaml.cs`
Primary content area view containing the main application interface.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainView"
             x:DataType="vm:MainViewViewModel"
             x:CompileBindings="True">
    <!-- Main content layout -->
</UserControl>
```

**Key Features:**
- **Tab-Based Navigation**: Primary interface for inventory, transfer, and remove operations
- **Sidebar Integration**: Coordinates with quick buttons panel
- **Responsive Layout**: Adapts to different screen sizes and resolutions
- **MTM Branding**: Implements MTM color scheme and design patterns

### Tab Views

#### `InventoryTabView.axaml` / `InventoryTabView.axaml.cs`
Primary inventory management interface view.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryTabView"
             x:DataType="vm:InventoryTabViewModel"
             x:CompileBindings="True">
    <!-- Inventory form layout -->
</UserControl>
```

**Layout Structure:**
```
InventoryTabView
??? Header Section
?   ??? Title and Description
?   ??? Action Buttons (Save, Reset, Advanced Entry)
??? Form Section
?   ??? Part ID ComboBox
?   ??? Operation ComboBox
?   ??? Location ComboBox
?   ??? Quantity TextBox
?   ??? Notes RichTextBox
??? Status Section
    ??? Validation Messages
    ??? Progress Indicators
```

**Key Bindings:**
```xml
<!-- Form Controls -->
<ComboBox ItemsSource="{Binding AvailableParts}"
          SelectedItem="{Binding SelectedPart}"
          Classes="form-control"/>

<TextBox Text="{Binding Quantity}"
         Classes="numeric-input"/>

<!-- Commands -->
<Button Content="Save"
        Command="{Binding SaveCommand}"
        Classes="primary"/>
```

### Component Views

#### `QuickButtonsView.axaml` / `QuickButtonsView.axaml.cs`
Quick action buttons panel view for rapid inventory operations.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.QuickButtonsView"
             x:DataType="vm:QuickButtonsViewModel"
             x:CompileBindings="True">
    <!-- Quick buttons layout -->
</UserControl>
```

**Dynamic Button Layout:**
```xml
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate DataType="vm:QuickButtonItemViewModel">
            <Button Classes="quick-button"
                    Command="{Binding ExecuteQuickActionCommand}"
                    CommandParameter="{Binding}">
                <Button.ContextMenu>
                    <ContextMenu>
                        <MenuItem Header="Edit Button" 
                                  Command="{Binding EditCommand}"/>
                        <MenuItem Header="Remove Button" 
                                  Command="{Binding RemoveCommand}"/>
                    </ContextMenu>
                </Button.ContextMenu>
                <StackPanel Orientation="Vertical" Spacing="4">
                    <TextBlock Text="{Binding PartId}" 
                               FontWeight="SemiBold"/>
                    <TextBlock Text="{Binding OperationDisplay}" 
                               FontSize="12" 
                               Opacity="0.8"/>
                    <TextBlock Text="{Binding QuantityDisplay}" 
                               FontSize="12" 
                               Opacity="0.8"/>
                </StackPanel>
            </Button>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

## ?? Design System Implementation

### MTM Brand Colors
Views implement the MTM purple-based color scheme:

```xml
<!-- Resource Definitions -->
<ResourceDictionary>
    <!-- Primary Colors -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#4B45ED"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>
    <SolidColorBrush x:Key="MagentaAccentBrush" Color="#BA45ED"/>
    <SolidColorBrush x:Key="BlueAccentBrush" Color="#4574ED"/>
    <SolidColorBrush x:Key="PinkAccentBrush" Color="#ED45E7"/>
    <SolidColorBrush x:Key="LightPurpleBrush" Color="#B594ED"/>

    <!-- Gradient Brushes -->
    <LinearGradientBrush x:Key="HeroGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#4574ED" Offset="0"/>
        <GradientStop Color="#4B45ED" Offset="0.3"/>
        <GradientStop Color="#8345ED" Offset="0.7"/>
        <GradientStop Color="#BA45ED" Offset="1"/>
    </LinearGradientBrush>
</ResourceDictionary>
```

### Modern UI Patterns

#### Card-Based Layout
```xml
<Border Classes="card" Padding="24" Margin="0,0,0,16">
    <Grid RowDefinitions="Auto,16,Auto,24,*">
        <!-- Card Header -->
        <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
            <PathIcon Grid.Column="0" 
                      Data="{StaticResource InventoryIcon}"
                      Width="24" Height="24"
                      Foreground="{DynamicResource AccentBrush}"/>
            <TextBlock Grid.Column="2" 
                       Text="Inventory Management"
                       FontSize="20"
                       FontWeight="SemiBold"/>
        </Grid>
        
        <!-- Card Description -->
        <TextBlock Grid.Row="2" 
                   Text="Add new inventory items to the system"
                   Opacity="0.8"
                   TextWrapping="Wrap"/>
        
        <!-- Card Content -->
        <Grid Grid.Row="4">
            <!-- Form content -->
        </Grid>
    </Grid>
</Border>
```

#### Responsive Form Layout
```xml
<Grid RowDefinitions="Auto,Auto,Auto,Auto,Auto" 
      ColumnDefinitions="Auto,*" 
      Margin="8">
    <!-- Part ID -->
    <TextBlock Grid.Row="0" Grid.Column="0" 
               Text="Part ID:" 
               VerticalAlignment="Center" 
               Margin="4"/>
    <ComboBox Grid.Row="0" Grid.Column="1" 
              ItemsSource="{Binding AvailableParts}"
              SelectedItem="{Binding SelectedPart}"
              Margin="4"/>
    
    <!-- Operation -->
    <TextBlock Grid.Row="1" Grid.Column="0" 
               Text="Operation:" 
               VerticalAlignment="Center" 
               Margin="4"/>
    <ComboBox Grid.Row="1" Grid.Column="1" 
              ItemsSource="{Binding AvailableOperations}"
              SelectedItem="{Binding SelectedOperation}"
              Margin="4"/>
    
    <!-- Additional form fields... -->
</Grid>
```

### Hero Section Pattern
```xml
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
            <TextBlock Text="MTM WIP Inventory System"
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

## ?? Binding Patterns

### Compiled Bindings
All views use compiled bindings for type safety and performance:

```xml
<UserControl x:DataType="vm:InventoryTabViewModel"
             x:CompileBindings="True">
    <!-- Type-safe bindings -->
    <TextBox Text="{Binding SelectedPart}"/>
    <Button Command="{Binding SaveCommand}"/>
</UserControl>
```

### Two-Way Data Binding
Form controls use two-way binding for immediate updates:

```xml
<!-- Text input with two-way binding -->
<TextBox Text="{Binding Quantity, Mode=TwoWay}"/>

<!-- ComboBox selection -->
<ComboBox ItemsSource="{Binding AvailableParts}"
          SelectedItem="{Binding SelectedPart, Mode=TwoWay}"/>
```

### Command Binding with Parameters
Commands can receive parameters from the view:

```xml
<Button Content="Execute Quick Action"
        Command="{Binding ExecuteQuickActionCommand}"
        CommandParameter="{Binding SelectedQuickButton}"/>
```

### Conditional Visibility
UI elements show/hide based on ViewModel state:

```xml
<!-- Error message display -->
<Border IsVisible="{Binding HasError}"
        Background="LightCoral"
        Padding="8"
        CornerRadius="4">
    <TextBlock Text="{Binding ErrorMessage}"
               Foreground="DarkRed"/>
</Border>

<!-- Loading indicator -->
<ProgressBar IsVisible="{Binding IsLoading}"
             IsIndeterminate="True"/>
```

## ?? Styling and Themes

### Control Classes
Views use CSS-like classes for consistent styling:

```xml
<!-- Primary action button -->
<Button Classes="primary" Content="Save"/>

<!-- Secondary action button -->
<Button Classes="secondary" Content="Cancel"/>

<!-- Form input control -->
<TextBox Classes="form-control"/>

<!-- Quick action button -->
<Button Classes="quick-button"/>

<!-- Card container -->
<Border Classes="card"/>
```

### Style Definitions
```xml
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

<Style Selector="Border.card">
    <Setter Property="Background" Value="{DynamicResource CardBackgroundBrush}"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="BoxShadow" Value="0 2 8 0 #11000000"/>
</Style>
```

### Responsive Design
Views adapt to different screen sizes:

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <!-- Desktop layout -->
        <ColumnDefinition Width="*" x:Name="MainColumn"/>
        <ColumnDefinition Width="Auto" x:Name="SidebarColumn"/>
    </Grid.ColumnDefinitions>
    
    <!-- Content adapts based on available space -->
</Grid>
```

## ?? Validation Display

### Visual Validation Feedback
Form controls show validation state through styling:

```xml
<TextBox Text="{Binding SelectedPart}"
         Classes.error="{Binding HasPartError}"/>

<Style Selector="TextBox.error">
    <Setter Property="BorderBrush" Value="Red"/>
    <Setter Property="BorderThickness" Value="2"/>
</Style>
```

### Error Message Display
```xml
<StackPanel>
    <TextBox Text="{Binding Quantity}"/>
    <TextBlock Text="{Binding QuantityError}"
               Foreground="Red"
               FontSize="12"
               IsVisible="{Binding HasQuantityError}"/>
</StackPanel>
```

### Form Validation Summary
```xml
<Border IsVisible="{Binding HasValidationErrors}"
        Background="#FFE6E6"
        BorderBrush="#FF0000"
        BorderThickness="1"
        Padding="8"
        Margin="0,0,0,16">
    <StackPanel>
        <TextBlock Text="Please correct the following errors:"
                   FontWeight="SemiBold"
                   Foreground="#CC0000"/>
        <ItemsControl ItemsSource="{Binding ValidationErrors}">
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding}"
                               Foreground="#CC0000"
                               Margin="16,2,0,2"/>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </StackPanel>
</Border>
```

## ?? Accessibility Features

### Keyboard Navigation
Views support full keyboard navigation:

```xml
<Button Content="Save"
        TabIndex="1"
        IsDefault="True"/>

<Button Content="Cancel"
        TabIndex="2"
        IsCancel="True"/>
```

### Screen Reader Support
Proper labeling for screen readers:

```xml
<TextBox AutomationProperties.Name="Part ID"
         AutomationProperties.HelpText="Select the part ID for this inventory item"/>
```

### High Contrast Support
Themes respect user accessibility preferences:

```xml
<Style Selector="Button:disabled">
    <Setter Property="Opacity" Value="0.5"/>
    <Setter Property="Background" Value="{DynamicResource DisabledBackgroundBrush}"/>
</Style>
```

## ?? Design-Time Support

### Design-Time Data
Views can display sample data during design:

```xml
<Design.DataContext>
    <vm:InventoryTabViewModel/>
</Design.DataContext>
```

### Preview in Designer
ViewModels provide design-time data:

```csharp
// In ViewModel constructor
if (Design.IsDesignMode)
{
    LoadDesignTimeData();
}
```

## ?? Layout Guidelines

### Spacing and Padding
Consistent spacing throughout the application:

- **Container Margins**: `Margin="8"` for containers
- **Control Margins**: `Margin="4"` for individual controls
- **Card Padding**: `Padding="24"` for card content
- **Form Spacing**: `Spacing="8"` for form elements

### Grid Layout Patterns
```xml
<!-- Form layout -->
<Grid RowDefinitions="Auto,Auto,Auto,*,Auto" 
      ColumnDefinitions="Auto,*">
    <!-- Form content -->
</Grid>

<!-- Card grid -->
<Grid RowDefinitions="Auto,16,*">
    <!-- Header, spacing, content -->
</Grid>

<!-- Action buttons -->
<StackPanel Orientation="Horizontal" 
            Spacing="8" 
            HorizontalAlignment="Right">
    <!-- Buttons -->
</StackPanel>
```

## ?? Development Guidelines

### Adding New Views
1. **Create AXAML File**: Define UI layout with proper bindings
2. **Set DataType**: Specify ViewModel type for compiled bindings
3. **Use Classes**: Apply consistent styling with CSS-like classes
4. **Implement Accessibility**: Add proper keyboard navigation and screen reader support
5. **Test Responsiveness**: Ensure layout works on different screen sizes

### View Conventions
- **File Naming**: Views end with `View` (e.g., `InventoryTabView.axaml`)
- **Compiled Bindings**: Always use `x:CompileBindings="True"`
- **DataType**: Always specify `x:DataType` for type safety
- **Resource Usage**: Use dynamic resources for theme-able properties
- **Margin/Padding**: Follow consistent spacing guidelines

## ?? Related Documentation

- **ViewModels**: See `ViewModels/README.md` for ViewModel patterns
- **UI Documentation**: `Documentation/Development/UI_Documentation/` for detailed component specs
- **Styling Guide**: Avalonia styling documentation for advanced styling
- **Design System**: MTM brand guidelines and color specifications

---

*This directory contains the user interface layer of the MTM WIP Application, implementing modern Avalonia UI patterns with responsive design and comprehensive accessibility support.*