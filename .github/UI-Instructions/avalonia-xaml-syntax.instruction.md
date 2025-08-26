# GitHub Copilot Instructions: Avalonia AXAML Syntax (Preventing AVLN2000 Errors)

<details>
<summary><strong>🚨 Critical AVLN2000 Error Prevention Rules</strong></summary>

**AVLN2000 errors occur when using WPF XAML syntax instead of Avalonia AXAML syntax. This instruction file provides the definitive guide to prevent these errors.**

### Primary Causes of AVLN2000 Errors:
1. **Using WPF property names instead of Avalonia property names**
2. **Incorrect Grid column/row definition syntax**
3. **Wrong namespace declarations**
4. **Using WPF-specific markup extensions that don't exist in Avalonia**
5. **Incorrect control property bindings**

</details>

<details>
<summary><strong>📋 WPF vs Avalonia AXAML Syntax Differences</strong></summary>

## Grid Definition Syntax (CRITICAL)

### ❌ WPF XAML (CAUSES AVLN2000)
```xml
<!-- WRONG: WPF syntax that causes AVLN2000 errors -->
<Grid.ColumnDefinitions>
    <ColumnDefinition Name="Column1" Width="Auto"/>
    <ColumnDefinition Name="Column2" Width="*"/>
</Grid.ColumnDefinitions>

<Grid.RowDefinitions>
    <RowDefinition Name="Row1" Height="Auto"/>
    <RowDefinition Name="Row2" Height="*"/>
</Grid.RowDefinitions>
```

### ✅ Avalonia AXAML (CORRECT)
```xml
<!-- CORRECT: Avalonia syntax using ColumnDefinitions/RowDefinitions attributes -->
<Grid ColumnDefinitions="Auto,*"
      RowDefinitions="Auto,*">
    <!-- Content -->
</Grid>

<!-- OR: Explicit definitions WITHOUT Name property -->
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <!-- Content -->
</Grid>
```

## Namespace Declarations

### ❌ WPF XAML
```xml
<!-- WRONG: WPF namespaces -->
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

### ✅ Avalonia AXAML
```xml
<!-- CORRECT: Avalonia namespaces -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

## Property Syntax Differences

### ❌ WPF Properties (CAUSES AVLN2000)
```xml
<!-- WRONG: WPF-specific properties -->
<Border Name="MyBorder"/>                    <!-- Use x:Name instead -->
<TextBlock FontFamily="Segoe UI"/>           <!-- Different font handling -->
<Button IsCancel="True"/>                    <!-- No IsCancel in Avalonia -->
<TextBox AcceptsTab="True"/>                 <!-- Different property name -->
```

### ✅ Avalonia Properties
```xml
<!-- CORRECT: Avalonia properties -->
<Border x:Name="MyBorder"/>
<TextBlock FontFamily="Segoe UI,Arial"/>     <!-- Font fallbacks -->
<Button HotKey="Escape"/>                    <!-- Use HotKey instead -->
<TextBox AcceptsTab="True"/>                 <!-- Same name, but verify support -->
```

## Control Differences

### ❌ WPF Controls Not Available in Avalonia
```xml
<!-- WRONG: These controls don't exist in Avalonia -->
<Label Target="{Binding ElementName=textBox}"/>
<Popup/>
<ToolTip/>
<StatusBar/>
<Ribbon/>
<DocumentViewer/>
```

### ✅ Avalonia Alternatives
```xml
<!-- CORRECT: Avalonia alternatives -->
<TextBlock/>                                 <!-- Instead of Label -->
<Flyout/>                                   <!-- Instead of Popup -->
<ToolTip.Tip="Text"/>                       <!-- Attached property for tooltips -->
<DockPanel LastChildFill="True"/>           <!-- For status bar layout -->
<!-- No direct Ribbon equivalent - use custom UI -->
<ScrollViewer/>                             <!-- For document viewing -->
```

</details>

<details>
<summary><strong>🎯 Avalonia-Specific AXAML Patterns</strong></summary>

## Compiled Bindings (CRITICAL FOR AVALONIA)

### ✅ Always Use Compiled Bindings
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainView"
             x:CompileBindings="True"
             x:DataType="vm:MainViewModel">
    
    <!-- Strongly-typed bindings -->
    <TextBox Text="{Binding PartId}"/>
    <Button Command="{Binding SearchCommand}"/>
</UserControl>
```

## Avalonia Grid Syntax Patterns

### ✅ Short Grid Definition Syntax (Preferred)
```xml
<!-- CORRECT: Concise grid definition -->
<Grid ColumnDefinitions="Auto,*,Auto"
      RowDefinitions="Auto,*,Auto"
      ColumnSpacing="12"
      RowSpacing="8">
    
    <TextBlock Grid.Column="0" Grid.Row="0" Text="Label:"/>
    <TextBox Grid.Column="1" Grid.Row="0" Text="{Binding Value}"/>
    <Button Grid.Column="2" Grid.Row="0" Content="Search"/>
</Grid>
```

### ✅ Explicit Grid Definition (When Complex)
```xml
<!-- CORRECT: Explicit when you need complex definitions -->
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200" MinWidth="150" MaxWidth="300"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*" MinHeight="200"/>
    </Grid.RowDefinitions>
    
    <!-- Content -->
</Grid>
```

## Avalonia Layout Controls

### ✅ Use Avalonia-Specific Layout Features
```xml
<!-- Avalonia's enhanced Grid features -->
<Grid ColumnDefinitions="*,Auto,*"
      RowDefinitions="Auto,*"
      ShowGridLines="False">
    
    <!-- Grid.ColumnSpan and Grid.RowSpan work the same -->
    <TextBlock Grid.ColumnSpan="3" 
               Text="Header" 
               HorizontalAlignment="Center"/>
</Grid>

<!-- UniformGrid for equal spacing -->
<UniformGrid Rows="2" Columns="3" 
             HorizontalAlignment="Stretch"
             VerticalAlignment="Stretch">
    <Button Content="1"/>
    <Button Content="2"/>
    <Button Content="3"/>
    <Button Content="4"/>
    <Button Content="5"/>
    <Button Content="6"/>
</UniformGrid>
```

## Avalonia Resource Syntax

### ✅ Avalonia Resource Patterns
```xml
<UserControl.Resources>
    <!-- Static Resources -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#6a0dad"/>
    
    <!-- Styles -->
    <Style Selector="Button.primary">
        <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
        <Setter Property="Foreground" Value="White"/>
    </Style>
    
    <!-- Data Templates -->
    <DataTemplate x:Key="ItemTemplate" DataType="vm:ItemViewModel">
        <Border Padding="8">
            <TextBlock Text="{Binding Name}"/>
        </Border>
    </DataTemplate>
</UserControl.Resources>
```

</details>

<details>
<summary><strong>⚠️ Common AVLN2000 Error Scenarios</strong></summary>

## Scenario 1: Grid Column/Row Names
### ❌ Error-Causing Code
```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Name="LeftColumn" Width="Auto"/>
        <ColumnDefinition Name="RightColumn" Width="*"/>
    </Grid.ColumnDefinitions>
</Grid>
```
**Error**: `AVLN2000: Unable to resolve suitable regular or attached property Name on type ColumnDefinition`

### ✅ Correct Code
```xml
<Grid ColumnDefinitions="Auto,*">
    <!-- OR -->
</Grid>

<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
</Grid>
```

## Scenario 2: WPF-Style Element Names
### ❌ Error-Causing Code
```xml
<Button Name="MyButton" Content="Click Me"/>
```
**Error**: Property `Name` doesn't exist on Button

### ✅ Correct Code
```xml
<Button x:Name="MyButton" Content="Click Me"/>
```

## Scenario 3: WPF Control Properties
### ❌ Error-Causing Code
```xml
<TextBox CharacterCasing="Upper"/>
<Button IsCancel="True"/>
<Label Target="{Binding ElementName=textBox}"/>
```

### ✅ Correct Code
```xml
<TextBox Classes="uppercase"/>  <!-- Use CSS classes for formatting -->
<Button HotKey="Escape"/>       <!-- Use HotKey instead of IsCancel -->
<TextBlock/>                    <!-- Use TextBlock instead of Label -->
```

## Scenario 4: Incorrect Binding Syntax
### ❌ Error-Causing Code
```xml
<!-- Missing DataType for compiled bindings -->
<UserControl x:CompileBindings="True">
    <TextBox Text="{Binding PropertyName}"/>
</UserControl>
```

### ✅ Correct Code
```xml
<UserControl x:CompileBindings="True"
             x:DataType="vm:MyViewModel">
    <TextBox Text="{Binding PropertyName}"/>
</UserControl>
```

</details>

<details>
<summary><strong>🔧 Avalonia Control Equivalents</strong></summary>

## Control Mapping Reference

| WPF Control | Avalonia Control | Notes |
|-------------|------------------|-------|
| `Label` | `TextBlock` or `Label` | TextBlock for display, Label for form labels |
| `Popup` | `Flyout` | Different API and usage |
| `ToolTip` | `ToolTip.Tip` | Attached property syntax |
| `StatusBar` | `DockPanel` | Custom implementation required |
| `GroupBox` | `HeaderedContentControl` | Or use Border with custom styling |
| `Expander` | `Expander` | Same name, different properties |
| `TabControl` | `TabControl` | Similar but check property names |
| `ListView` | `ListBox` | Or use ItemsControl with custom template |
| `TreeView` | `TreeView` | Similar API |
| `DataGrid` | `DataGrid` | Same name, different column syntax |
| `RichTextBox` | `TextBox` | Limited rich text support |
| `WebBrowser` | No direct equivalent | Use third-party controls |

## Property Mapping

| WPF Property | Avalonia Property | Notes |
|--------------|-------------------|-------|
| `Name` | `x:Name` | Always use x:Name |
| `IsCancel` | `HotKey="Escape"` | Different approach |
| `IsDefault` | `IsDefault` | Same property |
| `FontFamily` | `FontFamily` | Same but with fallbacks |
| `Visibility` | `IsVisible` | Boolean instead of enum |
| `Margin` | `Margin` | Same syntax |
| `Padding` | `Padding` | Same syntax |

</details>

<details>
<summary><strong>✅ AVLN2000 Prevention Checklist</strong></summary>

Before committing any AXAML file, verify:

### Grid Definitions
- [ ] ✅ No `Name` properties on `ColumnDefinition` or `RowDefinition`
- [ ] ✅ Use `ColumnDefinitions="Auto,*"` attribute syntax when possible
- [ ] ✅ Only use explicit `<Grid.ColumnDefinitions>` for complex scenarios

### Control Properties
- [ ] ✅ Use `x:Name` instead of `Name`
- [ ] ✅ No WPF-specific properties like `IsCancel`, `CharacterCasing`
- [ ] ✅ Use Avalonia equivalents for controls (TextBlock instead of Label)

### Namespace Declarations
- [ ] ✅ `xmlns="https://github.com/avaloniaui"` (not WPF namespace)
- [ ] ✅ Proper ViewModel namespace: `xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"`

### Compiled Bindings
- [ ] ✅ Include `x:CompileBindings="True"`
- [ ] ✅ Include `x:DataType="vm:ViewModelName"`
- [ ] ✅ All bindings target properties that exist on the ViewModel

### Resource Syntax
- [ ] ✅ Use `StaticResource` and `DynamicResource` correctly
- [ ] ✅ Proper Style Selector syntax: `Selector="Button.primary"`

### Layout Properties
- [ ] ✅ Use `IsVisible` instead of `Visibility`
- [ ] ✅ Check that all attached properties exist on target controls
- [ ] ✅ Verify Grid.Column and Grid.Row values are valid

</details>

<details>
<summary><strong>🎯 MTM-Specific Avalonia Patterns</strong></summary>

## MTM Standard AXAML Template
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             xmlns:materialIcons="using:Material.Icons.Avalonia"
             mc:Ignorable="d"
             d:DesignWidth="800"
             d:DesignHeight="600"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainForm.YourView"
             x:CompileBindings="True"
             x:DataType="vm:YourViewModel">

  <UserControl.Resources>
    <!-- MTM Color Scheme -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#6a0dad"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#ba45ed"/>
    <SolidColorBrush x:Key="CardBackgroundBrush" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="BorderBrush" Color="#E0E0E0"/>
  </UserControl.Resources>

  <UserControl.Styles>
    <!-- MTM Styles -->
    <Style Selector="Button.primary">
      <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
      <Setter Property="Foreground" Value="White"/>
    </Style>
  </UserControl.Styles>

  <!-- Content using correct Avalonia syntax -->
  <Grid RowDefinitions="*,Auto">
    <!-- Main content -->
    <Border Grid.Row="0" 
            Classes="card"
            Background="{StaticResource CardBackgroundBrush}">
      <!-- Your content here -->
    </Border>
    
    <!-- Action bar -->
    <StackPanel Grid.Row="1" 
                Orientation="Horizontal" 
                HorizontalAlignment="Right"
                Spacing="8"
                Margin="16">
      <Button Classes="primary" 
              Content="Save" 
              Command="{Binding SaveCommand}"/>
      <Button Content="Cancel" 
              Command="{Binding CancelCommand}"/>
    </StackPanel>
  </Grid>
</UserControl>
```

</details>

<details>
<summary><strong>🚀 Quick Reference Commands</strong></summary>

## Validation Commands
```bash
# Check for AVLN2000 errors before commit
dotnet build

# Validate AXAML syntax
# (Use Visual Studio IntelliSense or Avalonia VS extension)
```

## Common Find/Replace Patterns
```
# Fix Grid Names (Find & Replace in VS)
Find:    Name="[^"]*"
Replace: (empty)

# Fix WPF Namespaces
Find:    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
Replace: xmlns="https://github.com/avaloniaui"

# Fix Name to x:Name
Find:    Name="
Replace: x:Name="
```

</details>

---

**🎯 Remember**: When in doubt, prefer the simple Avalonia attribute syntax over complex element syntax. Always test your AXAML in the Avalonia designer to catch AVLN2000 errors early.
