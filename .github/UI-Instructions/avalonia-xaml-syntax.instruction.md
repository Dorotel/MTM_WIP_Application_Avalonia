# GitHub Copilot Instructions: Avalonia AXAML Syntax (Preventing AVLN2000 Errors)

**Generate Avalonia AXAML code strictly following the established patterns in this .NET 8 Avalonia MVVM application. Prevent AVLN2000 compilation errors by using only Avalonia-compatible syntax.**

<details>
<summary><strong>🎯 Technology Version Detection for AXAML (CRITICAL)</strong></summary>

**BEFORE generating ANY AXAML code, confirm these exact versions:**

### **Core UI Technologies**
- **Avalonia UI**: 11.3.4 (Primary UI framework - NOT WPF)
- **.NET Version**: 8.0 with C# 12 features
- **MVVM Community Toolkit**: 8.3.2 for property binding patterns
- **Nullable Reference Types**: Enabled - affects binding expressions

### **MTM Application AXAML Patterns**
Based on analysis of Views folder structure:
- **UserControl Pattern**: All views inherit from Avalonia UserControl
- **Minimal Code-Behind**: Logic in ViewModels using MVVM Community Toolkit
- **Standard Bindings**: `{Binding PropertyName}` with INotifyPropertyChanged
- **Design System**: Purple theme (#6a0dad), card-based layouts, consistent spacing

</details>

<details>
<summary><strong>🚨 Critical AVLN2000 Error Prevention Rules</strong></summary>

**AVLN2000 errors occur when WPF XAML syntax is mistakenly used in Avalonia AXAML.**

### **Primary Error Causes (CRITICAL TO AVOID)**
1. **WPF property/control names** - Properties or controls that don't exist in Avalonia
2. **Grid definition naming** - Using `Name` property on RowDefinition/ColumnDefinition
3. **Wrong namespaces** - WPF presentation namespace instead of Avalonia
4. **Unsupported markup** - WPF-only triggers, behaviors, or markup extensions
5. **Incorrect bindings** - Missing x:DataType for compiled bindings, wrong ElementName syntax
6. **Visibility enum usage** - WPF Visibility enum instead of Avalonia IsVisible boolean

### **Error Detection Strategy**
- **Namespace Check**: Always verify `xmlns="https://github.com/avaloniaui"`
- **Control Validation**: Confirm all controls exist in Avalonia 11.3.4
- **Property Verification**: Ensure all properties are Avalonia-compatible
- **Binding Syntax**: Use standard bindings compatible with MVVM Community Toolkit

</details>Instructions: Avalonia AXAML Syntax (Preventing AVLN2000 Errors)

This document is a practical, copy-paste friendly guide for writing correct Avalonia AXAML and avoiding AVLN2000 errors that commonly happen when WPF XAML is used by mistake.

<details>
<summary><strong>🚨 Critical AVLN2000 Error Prevention Rules</strong></summary>

AVLN2000 errors often occur when WPF XAML syntax is used in Avalonia AXAML.

Top causes:
1) Using WPF property/control names or enums that don’t exist in Avalonia
2) Using WPF-only element structures (e.g., Grid.ColumnDefinitions with Name attributes)
3) Wrong namespaces and resource include URIs
4) Using unsupported WPF markup extensions, triggers, or behaviors
5) Incorrect bindings (missing x:DataType for compiled bindings, wrong ElementName syntax)

</details>

<details>
<summary><strong>📋 WPF vs Avalonia AXAML: What’s Different</strong></summary>

## Namespaces (Root Element)

- WPF (WRONG in Avalonia)
```xml
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

- Avalonia (CORRECT)
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

- Add CLR namespaces with using:
```xml
xmlns:vm="using:YourApp.ViewModels"
xmlns:views="using:YourApp.Views"
```

- Design-time (optional)
```xml
xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
mc:Ignorable="d"
```

## Grid Definitions (CRITICAL)

- WPF (WRONG in Avalonia if you use Name on definitions)
```xml
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Name="Left" Width="Auto"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
</Grid>
```

- Avalonia (Preferred attribute syntax)
```xml
<Grid ColumnDefinitions="Auto,*"
      RowDefinitions="Auto,*"/>
```

- Avalonia (Explicit, but DO NOT use Name on Row/ColumnDefinition)
```xml
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
  <Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="*"/>
  </Grid.RowDefinitions>
</Grid>
```

- Extra Avalonia features:
  - Grid.RowSpacing / Grid.ColumnSpacing are available in Avalonia (not in WPF pre-.NET 8)
  - No SharedSizeGroup (WPF feature) in core Avalonia

## Element Naming

- WPF (WRONG): Name on elements
```xml
<Button Name="MyButton"/>
```

- Avalonia (CORRECT)
```xml
<Button x:Name="MyButton"/>
```

## Show/Hide Elements

- WPF (WRONG in Avalonia): Visibility enum (Visible/Collapsed/Hidden)
```xml
<TextBlock Visibility="Collapsed"/>
```

- Avalonia (CORRECT): IsVisible boolean
```xml
<TextBlock IsVisible="False"/>
```

If you’re porting WPF bindings, replace BooleanToVisibilityConverter with a bool binding (or a custom converter if needed).

## Controls: Availability and Differences

- Exists in Avalonia (core): Button, TextBlock, TextBox, CheckBox, RadioButton, ToggleSwitch, ComboBox, ListBox, TreeView, TabControl, Expander, Slider, ProgressBar, Menu, MenuItem, ContextMenu (legacy), Flyout, ContextFlyout, DataGrid, GridSplitter, UniformGrid, WrapPanel, DockPanel, Canvas, ScrollViewer, Label
- Not in core Avalonia: Ribbon, DocumentViewer, FlowDocument controls, WebBrowser
  - Web content: use Avalonia.WebView (WebView2) package
- Label exists in Avalonia. However, WPF’s Label.Target isn’t available; use access keys (_) and focus management instead.

## Popup / Flyout / ToolTip

- WPF “Popup” exists in Avalonia as Avalonia.Controls.Primitives.Popup, but common scenarios should prefer:
  - Flyout / MenuFlyout / ContextFlyout
  - ToolTip via attached property

Examples:
```xml
<!-- ToolTip -->
<Button ToolTip.Tip="Save"/>

<!-- Flyout -->
<Button Content="More">
  <FlyoutBase.AttachedFlyout>
    <Flyout Placement="Bottom">
      <StackPanel>
        <Button Content="Action 1"/>
        <Button Content="Action 2"/>
      </StackPanel>
    </Flyout>
  </FlyoutBase.AttachedFlyout>
</Button>

<!-- ContextFlyout -->
<Button Content="Open">
  <Button.ContextFlyout>
    <MenuFlyout>
      <MenuItem Header="Open Recent"/>
      <MenuItem Header="Open Folder"/>
    </MenuFlyout>
  </Button.ContextFlyout>
</Button>
```

## Button Defaults (IsDefault / IsCancel)

- WPF: IsDefault / IsCancel
- Avalonia: IsDefault / IsCancel are supported (use these, not a HotKey property)
```xml
<Button Content="OK" IsDefault="True" Command="{Binding OkCommand}"/>
<Button Content="Cancel" IsCancel="True" Command="{Binding CancelCommand}"/>
```

For keyboard shortcuts, use KeyBindings or add accelerators in MenuItem.HotKey.

## Keyboard Shortcuts (KeyBindings)

- WPF InputBindings vs Avalonia KeyBindings
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Window.KeyBindings>
    <KeyBinding Gesture="Ctrl+S" Command="{Binding SaveCommand}"/>
    <KeyBinding Gesture="Escape" Command="{Binding CancelCommand}"/>
  </Window.KeyBindings>
</Window>
```

## Access Keys (Accelerators in Text)

- WPF and Avalonia both use underscore (_) for access keys in headers/Content
```xml
<Button Content="_Save"/>
<MenuItem Header="_File"/>
```

## StackPanel/Items Spacing

- WPF traditionally uses Margin on children
- Avalonia also supports spacing:
```xml
<StackPanel Orientation="Horizontal" Spacing="8">
  <Button Content="A"/>
  <Button Content="B"/>
</StackPanel>
```

## ScrollViewer ScrollBar Visibility

- WPF: ScrollViewer.HorizontalScrollBarVisibility
- Avalonia: Same attached properties exist, same enum values (Disabled, Auto, Hidden, Visible)
```xml
<TextBox ScrollViewer.HorizontalScrollBarVisibility="Auto"
         ScrollViewer.VerticalScrollBarVisibility="Auto"/>
```

## Image Source URIs

- WPF pack:// URIs (WRONG in Avalonia)
- Avalonia uses avares:// for resources embedded in assemblies
```xml
<Image Source="avares://YourAssembly/Assets/logo.png"/>
```

</details>

<details>
<summary><strong>🎯 Avalonia-Specific AXAML Patterns</strong></summary>

## Compiled Bindings (RECOMMENDED)

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:YourApp.ViewModels"
             x:Class="YourApp.Views.MainView"
             x:CompileBindings="True"
             x:DataType="vm:MainViewModel">
  <TextBox Text="{Binding PartId}"/>
  <Button Command="{Binding SearchCommand}"/>
</UserControl>
```

- Always set both x:CompileBindings and x:DataType to enable typed, compiled bindings.

## ElementName and Ancestor Bindings

- Standard ElementName works:
```xml
<TextBox x:Name="Input"/>
<TextBlock Text="{Binding ElementName=Input, Path=Text}"/>
```

- Avalonia also supports shorthand for ElementName with #:
```xml
<TextBlock Text="{Binding #Input.Text}"/>
```

- Relative/Ancestor bindings are supported; prefer compiled bindings where possible.

## Short Grid

```xml
<Grid ColumnDefinitions="Auto,*,Auto"
      RowDefinitions="Auto,*"
      ColumnSpacing="12"
      RowSpacing="8">
  <TextBlock Grid.Column="0" Grid.Row="0" Text="Label:"/>
  <TextBox Grid.Column="1" Grid.Row="0" Text="{Binding Value}"/>
  <Button Grid.Column="2" Grid.Row="0" Content="Search"/>
</Grid>
```

## Styles and Selectors (Not WPF Triggers)

- Avalonia styles use CSS-like selectors, not WPF’s Trigger syntax.
- Implicit style example:
```xml
<UserControl.Styles>
  <Style Selector="Button.primary">
    <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
    <Setter Property="Foreground" Value="White"/>
  </Style>
  <Style Selector="TextBlock.caption">
    <Setter Property="FontSize" Value="12"/>
    <Setter Property="Opacity" Value="0.8"/>
  </Style>
</UserControl.Styles>
```

- Pseudoclasses like :pointerover, :pressed, :checked, :disabled can be used in Selector
```xml
<Style Selector="Button:pointerover">
  <Setter Property="Opacity" Value="0.9"/>
</Style>
```

- DataTrigger is available in Avalonia 11+, but the syntax differs from WPF. Prefer selectors/pseudoclasses or use DataTriggers where needed.

## Resource Include vs Style Include

- Styles go under a Styles element and are included via StyleInclude
- Non-style resources (brushes, thicknesses, etc.) use ResourceInclude

App.axaml example:
```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Application.Styles>
    <FluentTheme Mode="Light"/>
    <StyleInclude Source="avares://YourAssembly/Styles/Controls.axaml"/>
  </Application.Styles>

  <Application.Resources>
    <ResourceInclude Source="avares://YourAssembly/Styles/Colors.axaml"/>
  </Application.Resources>
</Application>
```

## Templates

- ControlTemplate and TemplateBinding exist in Avalonia:
```xml
<Style Selector="Button.custom">
  <Setter Property="Template">
    <ControlTemplate>
      <Border CornerRadius="4" Background="{TemplateBinding Background}">
        <ContentPresenter HorizontalAlignment="Center"
                          VerticalAlignment="Center"/>
      </Border>
    </ControlTemplate>
  </Setter>
</Style>
```

## DataTemplates

- DataTemplate selection by DataType works similarly:
```xml
<UserControl.Resources>
  <DataTemplate DataType="vm:ItemViewModel">
    <TextBlock Text="{Binding Name}"/>
  </DataTemplate>
</UserControl.Resources>
```

</details>

<details>
<summary><strong>🔧 Properties and Markup: What to Use vs Avoid</strong></summary>

## Name vs x:Name
- Always use x:Name, not Name.

## Visibility vs IsVisible
- Use IsVisible (bool) in Avalonia, not Visibility (enum).

## Fonts
- FontFamily exists in Avalonia; supports fallbacks:
```xml
<TextBlock FontFamily="Segoe UI, Arial, Helvetica"/>
```

## TextBox
- Commonly supported: Text, AcceptsReturn, AcceptsTab, Watermark, TextWrapping, PlaceholderText (theme specific)
- If you used CharacterCasing in WPF, verify availability in your Avalonia version; prefer explicit logic/converters if needed.

## ToolTip
- Use ToolTip.Tip attached property or nested property:
```xml
<Button>
  <ToolTip.Tip>
    <StackPanel>
      <TextBlock Text="Line 1"/>
      <TextBlock Text="Line 2"/>
    </StackPanel>
  </ToolTip.Tip>
</Button>
```

## MenuItem shortcuts
- Use HotKey on MenuItem (not InputGestureText):
```xml
<MenuItem Header="_Save" HotKey="Ctrl+S" Command="{Binding SaveCommand}"/>
```

## Markup extensions you can use
- StaticResource, DynamicResource, Binding, TemplateBinding, x:Null
- x:Static is supported in Avalonia XAML; x:Type is generally not used the same way as WPF

## MultiBinding
- Avalonia supports MultiBinding with IMultiValueConverter (verify your Avalonia version and package references):
```xml
<TextBlock>
  <TextBlock.Text>
    <MultiBinding Converter="{StaticResource FullNameConverter}">
      <Binding Path="FirstName"/>
      <Binding Path="LastName"/>
    </MultiBinding>
  </TextBlock.Text>
</TextBlock>
```

## Event setters in styles
- WPF’s EventSetter is not supported. Use behaviors, attached properties, or commands/KeyBindings instead.

</details>

<details>
<summary><strong>⚠️ Common AVLN2000 Error Scenarios (and Fixes)</strong></summary>

## 1) Grid Column/Row Names
- WRONG
```xml
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Name="Left" Width="Auto"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
</Grid>
```
- FIX:
```xml
<Grid ColumnDefinitions="Auto,*"/>
<!-- OR -->
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
</Grid>
```

## 2) WPF Name Instead of x:Name
- WRONG
```xml
<TextBox Name="SearchBox"/>
```
- FIX
```xml
<TextBox x:Name="SearchBox"/>
```

## 3) Using Visibility
- WRONG
```xml
<Border Visibility="Collapsed"/>
```
- FIX
```xml
<Border IsVisible="False"/>
```

## 4) WPF-only namespaces
- WRONG
```xml
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"/>
```
- FIX
```xml
<UserControl xmlns="https://github.com/avaloniaui"/>
```

## 5) Missing DataType for compiled bindings
- WRONG
```xml
<UserControl x:CompileBindings="True">
  <TextBox Text="{Binding Name}"/>
</UserControl>
```
- FIX
```xml
<UserControl x:CompileBindings="True"
             xmlns:vm="using:YourApp.ViewModels"
             x:DataType="vm:CustomerViewModel">
  <TextBox Text="{Binding Name}"/>
</UserControl>
```

## 6) WPF triggers in styles
- WRONG (WPF-style Trigger)
```xml
<Style TargetType="Button">
  <Style.Triggers>
    <Trigger Property="IsMouseOver" Value="True">
      <Setter Property="Background" Value="Red"/>
    </Trigger>
  </Style.Triggers>
</Style>
```
- FIX (Avalonia selector)
```xml
<Style Selector="Button:pointerover">
  <Setter Property="Background" Value="Red"/>
</Style>
```

</details>

<details>
<summary><strong>🔁 Control and Property Mapping Reference</strong></summary>

## Controls
- WPF: Label → Avalonia: Label or TextBlock (Label exists; Target is not available. Use access keys and focus logic.)
- WPF: Popup → Avalonia: Popup exists, but prefer Flyout/ContextFlyout for menus and transient UI
- WPF: ToolTip → Avalonia: ToolTip.Tip attached property
- WPF: StatusBar → Avalonia: build with DockPanel/Grid
- WPF: Ribbon → Avalonia: no core equivalent
- WPF: DocumentViewer / FlowDocument → Avalonia: no core equivalent
- WPF: WebBrowser → Avalonia: use Avalonia.WebView
- WPF: ListView → Avalonia: ListBox or DataGrid for tabular, or ItemsControl with templates

## Properties
- Name → x:Name
- Visibility → IsVisible (bool)
- IsDefault → IsDefault (exists)
- IsCancel → IsCancel (exists)
- FontFamily → FontFamily (supports fallbacks)
- Margin / Padding → same names/syntax
- HorizontalContentAlignment / VerticalContentAlignment → supported on content controls
- ScrollViewer.*ScrollBarVisibility → exists with same enum names
- Grid.Row / Grid.Column / Grid.RowSpan / Grid.ColumnSpan → same
- Grid.RowSpacing / Grid.ColumnSpacing → Avalonia-only convenience
- DockPanel.Dock → same
- Canvas.Left/Top → same

</details>

<details>
<summary><strong>📦 Resources, URIs, and Assets</strong></summary>

## Static vs Dynamic Resources
```xml
<Border Background="{StaticResource PrimaryBrush}"/>
<Border Background="{DynamicResource PrimaryBrush}"/>
```

## Resource Includes
- Styles:
```xml
<StyleInclude Source="avares://YourAssembly/Styles/Buttons.axaml"/>
```
- Resources:
```xml
<ResourceInclude Source="avares://YourAssembly/Styles/Colors.axaml"/>
```

## Images and Fonts
- Use avares://
```xml
<Image Source="avares://YourAssembly/Assets/logo.png"/>
```
- Embed fonts and reference with FontFamily
```xml
<TextBlock FontFamily="avares://YourAssembly/Assets/Fonts#YourFontName"/>
```

</details>

<details>
<summary><strong>✅ AVLN2000 Prevention Checklist</strong></summary>

Grid
- [ ] No Name on RowDefinition/ColumnDefinition
- [ ] Prefer ColumnDefinitions/RowDefinitions attribute syntax when simple
- [ ] If explicit, only Width/Height and size constraints are used (no WPF-only props)

Naming and Visibility
- [ ] x:Name used, never Name
- [ ] IsVisible used, never Visibility

Namespaces and Includes
- [ ] Root xmlns is https://github.com/avaloniaui
- [ ] No WPF presentation namespace
- [ ] Resource/Style includes use avares://

Bindings
- [ ] x:CompileBindings and x:DataType set on views using compiled bindings
- [ ] ElementName or #element syntax correct
- [ ] No WPF-only binding features without Avalonia equivalents

Controls/Properties
- [ ] Only Avalonia-supported controls used (Popup/Flyout/ToolTip as per Avalonia)
- [ ] No WPF-only features like SharedSizeGroup, FlowDocument, Ribbon
- [ ] KeyBindings for shortcuts; MenuItem.HotKey for menu accelerators
- [ ] Triggers done via selectors or Avalonia DataTriggers (not WPF Trigger syntax)

Resources/URIs
- [ ] avares:// used for assets and includes
- [ ] StaticResource/DynamicResource used appropriately

</details>

<details>
<summary><strong>🎯 MTM-Specific Avalonia Template (Sample)</strong></summary>

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
    <SolidColorBrush x:Key="PrimaryBrush" Color="#6a0dad"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#ba45ed"/>
    <SolidColorBrush x:Key="CardBackgroundBrush" Color="#FFFFFF"/>
    <SolidColorBrush x:Key="BorderBrush" Color="#E0E0E0"/>
  </UserControl.Resources>

  <UserControl.Styles>
    <Style Selector="Button.primary">
      <Setter Property="Background" Value="{StaticResource PrimaryBrush}"/>
      <Setter Property="Foreground" Value="White"/>
    </Style>
  </UserControl.Styles>

  <Grid RowDefinitions="*,Auto">
    <Border Grid.Row="0" Classes="card" Background="{StaticResource CardBackgroundBrush}">
      <!-- Content -->
    </Border>

    <StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Right" Spacing="8" Margin="16">
      <Button Classes="primary" Content="Save" Command="{Binding SaveCommand}"/>
      <Button Content="Cancel" Command="{Binding CancelCommand}"/>
    </StackPanel>
  </Grid>
</UserControl>
```

</details>

<details>
<summary><strong>🚀 Quick Reference: Build and Search</strong></summary>

Validation
```bash
dotnet build
```

Find/Replace (Porting from WPF)
```
# Fix WPF root namespace
Find:    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
Replace: xmlns="https://github.com/avaloniaui"

# Name -> x:Name
Find:    Name="
Replace: x:Name="

# Remove Name on Grid definitions
Find:    <ColumnDefinition Name="[^"]*"
Replace: <ColumnDefinition
Find:    <RowDefinition Name="[^"]*"
Replace: <RowDefinition

# Visibility -> IsVisible (manual review still recommended)
Find:    Visibility="
Replace: IsVisible="
```

</details>

---

Remember: Prefer Avalonia’s concise attribute syntax, selectors-based styling, avares:// URIs, and compiled bindings with x:DataType. When unsure, check if a property/control exists in Avalonia before using WPF habits.
