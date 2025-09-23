# Avalonia UI Guidelines - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Target Framework**: .NET 8  
**UI Pattern**: MVVM with Community Toolkit  
**Created**: September 4, 2025  
**Updated**: September 21, 2025 (Phase 1 Material.Avalonia Integration)

---

## 📚 Comprehensive Avalonia Documentation Reference

**IMPORTANT**: This repository contains the complete Avalonia documentation straight from the official website in the `.github/Avalonia-Documentation/` folder. This includes:

- **Complete API Reference**: `.github/Avalonia-Documentation/reference/`
- **Comprehensive Guides**: `.github/Avalonia-Documentation/guides/`
- **Cross-Platform Deployment**: `.github/Avalonia-Documentation/deployment/`
- **MVVM Patterns**: `.github/Avalonia-Documentation/concepts/the-mvvm-pattern/`
- **Control Documentation**: `.github/Avalonia-Documentation/reference/controls/`
- **Styling and Themes**: `.github/Avalonia-Documentation/guides/styles-and-resources/`
- **Data Binding**: `.github/Avalonia-Documentation/guides/data-binding/`

**Always reference the local Avalonia-Documentation folder for the most current and comprehensive guidance.**

---

## 🎯 Critical Avalonia AXAML Syntax Rules

### MANDATORY Header Structure (Prevents AVLN2000 Errors)

```xml
<!-- CORRECT: Required header for all UserControl files -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.SomeView">

<!-- WRONG: Never use WPF namespace -->
<UserControl xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
```

### Grid Definition Syntax (CRITICAL - Prevents AVLN2000)

```xml
<!-- CORRECT: Use x:Name on Grid definitions -->
<Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Header" />
    <Border Grid.Row="1" Grid.Column="1" Background="White">
        <!-- Content here -->
    </Border>
</Grid>

<!-- WRONG: Never use Name property on Grid - causes AVLN2000 compilation errors -->
<Grid Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
    <!-- This will fail to compile -->
</Grid>

<!-- CORRECT: Attribute-based definitions preferred -->
<Grid RowDefinitions="Auto,*,Auto" ColumnDefinitions="200,*">
    <!-- Cleaner than Grid.RowDefinitions elements -->
</Grid>

<!-- Also CORRECT: Element-based definitions when complex -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="Auto" />
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="200" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
</Grid>
```

**Reference**: Complete Grid layout patterns and advanced usage examples available in `.github/Avalonia-Documentation/reference/controls/grid.md`

### Control Equivalents and Naming

```xml
<!-- Avalonia Control Names (NOT WPF equivalents) -->

<!-- CORRECT: Avalonia controls -->
<TextBlock Text="Display text" />           <!-- NOT Label -->
<Button Content="Click me" />               <!-- Same as WPF -->
<ComboBox ItemsSource="{Binding Items}" />  <!-- Same as WPF -->
<ListBox ItemsSource="{Binding Items}" />   <!-- Same as WPF -->
<Border Background="White" />               <!-- Same as WPF -->
<StackPanel Orientation="Vertical" />       <!-- Same as WPF -->
<Panel />                                   <!-- Base panel type -->

<!-- CORRECT: Avalonia-specific controls -->
<Flyout>                                    <!-- NOT Popup -->
    <Border Background="White" Padding="10">
        <TextBlock Text="Flyout content" />
    </Border>
</Flyout>

<MenuFlyout>                               <!-- NOT ContextMenu -->
    <MenuItem Header="Copy" />
    <MenuItem Header="Paste" />
</MenuFlyout>
```

---

## 🎨 MTM Design System Implementation

### Theme Resource Usage (MANDATORY)

```xml
<!-- All UI elements MUST use MTM theme resources -->

<!-- Primary Action Buttons -->
<Button Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}"
        Foreground="White"
        Padding="16,8"
        CornerRadius="4"
        Content="Save Changes" />

<!-- Secondary Action Buttons -->
<Button Background="{DynamicResource MTM_Shared_Logic.SecondaryAction}"
        Foreground="White"
        Padding="12,6"
        CornerRadius="4"
        Content="Cancel" />

<!-- Card-Based Layout System -->
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="8">
    
    <!-- Card Header -->
    <Grid RowDefinitions="Auto,*">
        <Border Grid.Row="0" 
                Background="{DynamicResource MTM_Shared_Logic.PrimaryAction}" 
                CornerRadius="8,8,0,0" 
                Padding="16,8">
            <TextBlock Text="Card Title" 
                      Foreground="White" 
                      FontWeight="Bold" />
        </Border>
        
        <!-- Card Content -->
        <StackPanel Grid.Row="1" Margin="16" Spacing="8">
            <TextBlock Text="Card content goes here"
                      Foreground="{DynamicResource MTM_Shared_Logic.BodyText}" />
        </StackPanel>
    </Grid>
</Border>

<!-- Form Input Fields -->
<TextBox Text="{Binding PartId}"
         Watermark="Enter Part ID"
         Background="{DynamicResource MTM_Shared_Logic.ContentAreas}"
         BorderBrush="{DynamicResource MTM_Shared_Logic.BorderBrush}"
         Margin="8,4" />

<!-- Status and Feedback Colors -->
<Border Background="{DynamicResource MTM_Shared_Logic.SuccessBrush}" 
        CornerRadius="4" 
        Padding="8">
    <TextBlock Text="Success message" Foreground="White" />
</Border>

<Border Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}" 
        CornerRadius="4" 
        Padding="8">
    <TextBlock Text="Error message" Foreground="White" />
</Border>
```

### MTM Color Palette Reference

```xml
<!-- Core MTM Brand Colors -->
MTM_Shared_Logic.PrimaryAction: #0078D4 (Windows 11 Blue)
MTM_Shared_Logic.SecondaryAction: #106EBE (Darker Blue)
MTM_Shared_Logic.Warning: #FFB900 (Amber Warning)
MTM_Shared_Logic.Critical: #D13438 (Red Alert)
MTM_Shared_Logic.Highlight: #005A9E (Selected State)

<!-- Background and Layout Colors -->
MTM_Shared_Logic.MainBackground: #FAFAFA (Light Gray)
MTM_Shared_Logic.ContentAreas: #FFFFFF (Pure White)
MTM_Shared_Logic.CardBackgroundBrush: #F3F2F1 (Card Background)
MTM_Shared_Logic.BorderBrush: #E1DFDD (Light Border)

<!-- Text Color Hierarchy -->
MTM_Shared_Logic.HeadingText: #323130 (Dark Gray)
MTM_Shared_Logic.BodyText: #605E5C (Medium Gray)
MTM_Shared_Logic.TertiaryTextBrush: #8A8886 (Light Gray)
MTM_Shared_Logic.InteractiveText: #0078D4 (Primary Blue)

<!-- Semantic Colors -->
MTM_Shared_Logic.SuccessBrush: #4CAF50 (Green)
MTM_Shared_Logic.WarningBrush: #FF9800 (Orange)
MTM_Shared_Logic.ErrorBrush: #F44336 (Red)
```

---

## 📏 Layout and Spacing Standards

### 🚨 MANDATORY: InventoryTabView Grid Pattern for ALL Tab Views

**CRITICAL IMPLEMENTATION REQUIREMENT**: All tab views in MainView must implement this exact pattern to ensure proper input field containment and theme consistency. This pattern prevents UI overflow issues and maintains professional appearance.

```xml
<!-- ROOT: ScrollViewer for overflow handling - REQUIRED -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" 
              VerticalScrollBarVisibility="Auto"
              Background="{DynamicResource MTM_Shared_Logic.MainBackground}">
    
  <!-- MAIN: Container Grid with proper row definitions - REQUIRED -->
  <Grid x:Name="MainContainer" 
        RowDefinitions="*,Auto"
        MinWidth="600"
        MinHeight="400"
        Margin="8">

    <!-- CONTENT: Entry Panel with card styling - REQUIRED -->
    <Border Grid.Row="0"
            Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="1"
            CornerRadius="8"
            Padding="16"
            Margin="0,0,0,8">
      
      <!-- FORM: Structured grid for input fields - REQUIRED -->
      <Grid x:Name="EntryFormGrid" RowDefinitions="Auto,Auto,*">
        
        <!-- Header Section with theme consistency -->
        <Border Grid.Row="0"
                Background="{DynamicResource MTM_Shared_Logic.SidebarGradientBrush}"
                CornerRadius="6"
                Padding="12,8"
                Margin="0,0,0,16">
          <!-- Header content -->
        </Border>

        <!-- Error Display Section (when needed) -->
        <Border Grid.Row="1"
                Background="{DynamicResource MTM_Shared_Logic.ErrorLightBrush}"
                BorderBrush="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                BorderThickness="2"
                CornerRadius="4"
                Padding="12,8"
                Margin="0,0,0,16"
                IsVisible="{Binding HasError}">
          <!-- Error content -->
        </Border>

        <!-- Form Fields Grid with proper containment -->
        <Grid x:Name="FormFieldsGrid" 
              Grid.Row="2"
              RowDefinitions="Auto,Auto,Auto,Auto,*"
              RowSpacing="12">
          
          <!-- Individual field grids with ColumnDefinitions="90,*" pattern -->
          <Grid x:Name="FieldGrid" 
                Grid.Row="0" 
                ColumnDefinitions="90,*" 
                ColumnSpacing="12">
            <StackPanel Grid.Column="0"><!-- Label --></StackPanel>
            <Control Grid.Column="1" Classes="input-field"><!-- Input --></Control>
          </Grid>
          
        </Grid>
      </Grid>
    </Border>
    
    <!-- ACTIONS: Button panel with theme consistency - REQUIRED -->
    <Border Grid.Row="1"
            Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
            BorderThickness="1"
            CornerRadius="6"
            Padding="12"
            Margin="0,0,0,8">
      <!-- Action buttons with proper spacing -->
    </Border>
  </Grid>
</ScrollViewer>
```

**NON-NEGOTIABLE REQUIREMENTS:**

1. **ScrollViewer** as root container - prevents content overflow
2. **Grid with RowDefinitions="*,Auto"** - separates content from actions
3. **All input fields contained within grid boundaries** - prevents UI overflow
4. **DynamicResource theme bindings for ALL colors** - ensures theme consistency
5. **Consistent spacing: 8px, 16px, 24px** - maintains professional appearance

**AFFECTED VIEWS (MUST IMPLEMENT THIS PATTERN):**

- ✅ InventoryTabView (reference implementation)
- ❌ RemoveTabView (requires update to this pattern)  
- ❌ TransferTabView (requires update to this pattern)

---

### MTM Spacing System

```xml
<!-- Consistent spacing using 8px base unit -->

<!-- Small spacing: 8px -->
<StackPanel Spacing="8">
    <TextBlock Text="Related items" />
    <TextBlock Text="Close together" />
</StackPanel>

<!-- Medium spacing: 16px -->
<Grid RowDefinitions="Auto,*" Margin="16">
    <TextBlock Grid.Row="0" Text="Section Header" />
    <Border Grid.Row="1" Margin="0,16,0,0">
        <!-- Section content with 16px top margin -->
    </Border>
</Grid>

<!-- Large spacing: 24px -->
<StackPanel Spacing="24">
    <Border><!-- Major section 1 --></Border>
    <Border><!-- Major section 2 --></Border>
</StackPanel>

<!-- Card padding: 16px -->
<Border Padding="16" Background="White">
    <!-- Standard card content padding -->
</Border>

<!-- Button padding: Primary (16,8), Secondary (12,6) -->
<Button Padding="16,8" Content="Primary Action" />
<Button Padding="12,6" Content="Secondary Action" />
```

### Responsive Layout Patterns

```xml
<!-- Adaptive grid columns for different window sizes -->
<Grid x:Name="ResponsiveGrid">
    <Grid.ColumnDefinitions>
        <!-- Sidebar: Fixed 250px on large screens, collapsible on small -->
        <ColumnDefinition Width="250" x:Name="SidebarColumn" />
        <!-- Content: Takes remaining space -->
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    
    <!-- Sidebar Panel -->
    <Border Grid.Column="0" 
            Background="{DynamicResource MTM_Shared_Logic.SidebarDark}"
            x:Name="SidebarPanel">
        <ScrollViewer>
            <!-- Navigation content -->
        </ScrollViewer>
    </Border>
    
    <!-- Main Content Area -->
    <ScrollViewer Grid.Column="1" Padding="16">
        <!-- Main application content -->
    </ScrollViewer>
</Grid>

<!-- Card-based responsive layout -->
<WrapPanel Orientation="Horizontal" ItemWidth="300" ItemHeight="200">
    <!-- Cards automatically wrap based on available space -->
    <Border Classes="mtm-card" Margin="8">
        <StackPanel Padding="16">
            <TextBlock Text="Card 1" FontWeight="Bold" />
            <TextBlock Text="Content here" />
        </StackPanel>
    </Border>
    
    <Border Classes="mtm-card" Margin="8">
        <StackPanel Padding="16">
            <TextBlock Text="Card 2" FontWeight="Bold" />
            <TextBlock Text="Content here" />
        </StackPanel>
    </Border>
</WrapPanel>
```

---

## 🔧 Data Binding and MVVM Integration

**Reference**: Complete data binding patterns and advanced MVVM scenarios available in `.github/Avalonia-Documentation/guides/data-binding/` and `.github/Avalonia-Documentation/concepts/the-mvvm-pattern/`

### MVVM Community Toolkit Binding Patterns

```xml
<!-- Property binding with MVVM Community Toolkit -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:DataType="vm:InventoryTabViewModel">

    <!-- Two-way binding for form inputs -->
    <StackPanel Spacing="12">
        <TextBox Text="{Binding PartId}" 
                 Watermark="Enter Part ID" />
        
        <ComboBox ItemsSource="{Binding Operations}"
                  SelectedItem="{Binding SelectedOperation}" />
        
        <NumericUpDown Value="{Binding Quantity}"
                       Minimum="1"
                       Maximum="999999" />
        
        <!-- Command binding -->
        <Button Content="Save Inventory"
                Command="{Binding SaveCommand}"
                IsEnabled="{Binding !IsLoading}" />
        
        <!-- Status display -->
        <TextBlock Text="{Binding StatusMessage}"
                   IsVisible="{Binding StatusMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
        
        <!-- Loading indicator -->
        <ProgressBar IsIndeterminate="True"
                     IsVisible="{Binding IsLoading}" />
    </StackPanel>
</UserControl>
```

### Collection Binding and Templates

```xml
<!-- ItemsSource binding with DataTemplate -->
<ListBox ItemsSource="{Binding QuickButtons}"
         SelectedItem="{Binding SelectedQuickButton}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <Border Classes="mtm-card" Margin="4" Padding="12">
                <Grid ColumnDefinitions="*,Auto">
                    <StackPanel Grid.Column="0">
                        <TextBlock Text="{Binding PartId}" FontWeight="Bold" />
                        <TextBlock Text="{Binding Operation}" 
                                  FontSize="12" 
                                  Foreground="{DynamicResource MTM_Shared_Logic.TertiaryTextBrush}" />
                        <TextBlock Text="{Binding Quantity, StringFormat='Qty: {0}'}" />
                    </StackPanel>
                    
                    <Button Grid.Column="1" 
                            Content="Execute"
                            Command="{Binding $parent[ListBox].((vm:QuickButtonsViewModel)DataContext).ExecuteQuickActionCommand}"
                            CommandParameter="{Binding}"
                            Classes="secondary" />
                </Grid>
            </Border>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>

<!-- DataGrid for tabular data -->
<DataGrid ItemsSource="{Binding TransactionHistory}"
          SelectedItem="{Binding SelectedTransaction}"
          AutoGenerateColumns="False"
          GridLinesVisibility="Horizontal">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID" Binding="{Binding PartId}" Width="120" />
        <DataGridTextColumn Header="Operation" Binding="{Binding Operation}" Width="100" />
        <DataGridTextColumn Header="Quantity" Binding="{Binding Quantity}" Width="80" />
        <DataGridTextColumn Header="Location" Binding="{Binding Location}" Width="100" />
        <DataGridTextColumn Header="User" Binding="{Binding UserId}" Width="120" />
        <DataGridTextColumn Header="Date" 
                           Binding="{Binding Timestamp, StringFormat='{0:yyyy-MM-dd HH:mm}'}" 
                           Width="140" />
    </DataGrid.Columns>
</DataGrid>
```

### Value Converters and Validation

```xml
<!-- Built-in converters -->
<TextBlock Text="{Binding Count, StringFormat='Items: {0}'}" />
<Border IsVisible="{Binding HasItems}" />
<TextBlock Text="{Binding LastUpdated, StringFormat='Last Updated: September 20, 2025

<!-- Custom converters (defined in Converters/ folder) -->
<TextBlock Foreground="{Binding Status, Converter={StaticResource StatusToBrushConverter}}" />
<CheckBox IsChecked="{Binding Value, Converter={StaticResource NullToBoolConverter}}" />

<!-- Validation display -->
<TextBox Text="{Binding PartId}">
    <TextBox.Styles>
        <Style Selector="TextBox:error">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
    </TextBox.Styles>
</TextBox>

<TextBlock Text="{Binding (Validation.Errors)[0].ErrorContent}"
           IsVisible="{Binding (Validation.HasErrors)}"
           Foreground="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
           FontSize="12" />
```

---

## 🎭 Styles and Themes Integration

**Reference**: Complete styling patterns, theme systems, and resource management available in `.github/Avalonia-Documentation/guides/styles-and-resources/`

### Style Classes and Themes

```xml
<!-- Define style classes for reusable components -->
<UserControl.Styles>
    <!-- MTM Card Style Class -->
    <Style Selector="Border.mtm-card">
        <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}" />
        <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.BorderBrush}" />
        <Setter Property="BorderThickness" Value="1" />
        <Setter Property="CornerRadius" Value="8" />
        <Setter Property="Padding" Value="16" />
        <Setter Property="Margin" Value="8" />
        
        <!-- Hover effect -->
        <Style Selector="^:pointerover">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.HoverBackground}" />
        </Style>
    </Style>
    
    <!-- MTM Primary Button Class -->
    <Style Selector="Button.primary">
        <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="Padding" Value="16,8" />
        <Setter Property="CornerRadius" Value="4" />
        <Setter Property="FontWeight" Value="SemiBold" />
        
        <!-- Hover state -->
        <Style Selector="^:pointerover /template/ ContentPresenter">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryHoverBrush}" />
        </Style>
        
        <!-- Disabled state -->
        <Style Selector="^:disabled">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.PrimaryDisabledBrush}" />
            <Setter Property="Opacity" Value="0.5" />
        </Style>
    </Style>
    
    <!-- MTM Secondary Button Class -->
    <Style Selector="Button.secondary">
        <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryAction}" />
        <Setter Property="Foreground" Value="White" />
        <Setter Property="Padding" Value="12,6" />
        <Setter Property="CornerRadius" Value="4" />
        
        <Style Selector="^:pointerover /template/ ContentPresenter">
            <Setter Property="Background" Value="{DynamicResource MTM_Shared_Logic.SecondaryHoverBrush}" />
        </Style>
    </Style>
    
    <!-- Form Section Header Style -->
    <Style Selector="TextBlock.section-header">
        <Setter Property="FontSize" Value="18" />
        <Setter Property="FontWeight" Value="SemiBold" />
        <Setter Property="Foreground" Value="{DynamicResource MTM_Shared_Logic.HeadingText}" />
        <Setter Property="Margin" Value="0,16,0,8" />
    </Style>
</UserControl.Styles>

<!-- Usage of style classes -->
<StackPanel>
    <TextBlock Classes="section-header" Text="Inventory Details" />
    
    <Border Classes="mtm-card">
        <Grid RowDefinitions="Auto,Auto,Auto" ColumnDefinitions="100,*" Spacing="12">
            <TextBlock Grid.Row="0" Grid.Column="0" Text="Part ID:" />
            <TextBox Grid.Row="0" Grid.Column="1" Text="{Binding PartId}" />
            
            <TextBlock Grid.Row="1" Grid.Column="0" Text="Quantity:" />
            <NumericUpDown Grid.Row="1" Grid.Column="1" Value="{Binding Quantity}" />
            
            <StackPanel Grid.Row="2" Grid.Column="1" Orientation="Horizontal" Spacing="8">
                <Button Classes="primary" Content="Save" Command="{Binding SaveCommand}" />
                <Button Classes="secondary" Content="Reset" Command="{Binding ResetCommand}" />
            </StackPanel>
        </Grid>
    </Border>
</StackPanel>
```

### Animation and Transitions

```xml
<!-- Smooth transitions for theme changes -->
<UserControl.Styles>
    <Style Selector="Border">
        <Setter Property="Transitions">
            <Transitions>
                <BrushTransition Property="Background" Duration="0:0:0.3" />
                <BrushTransition Property="BorderBrush" Duration="0:0:0.3" />
            </Transitions>
        </Setter>
    </Style>
    
    <Style Selector="Button">
        <Setter Property="Transitions">
            <Transitions>
                <BrushTransition Property="Background" Duration="0:0:0.2" />
                <TransformOperationsTransition Property="RenderTransform" Duration="0:0:0.1" />
            </Transitions>
        </Setter>
        
        <!-- Subtle press animation -->
        <Style Selector="^:pressed">
            <Setter Property="RenderTransform" Value="scale(0.98)" />
        </Style>
    </Style>
</UserControl.Styles>

<!-- Loading animations -->
<Grid IsVisible="{Binding IsLoading}">
    <Border Background="#80000000" /> <!-- Semi-transparent overlay -->
    
    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <ProgressBar IsIndeterminate="True" 
                     Width="200" 
                     Height="4" 
                     Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
        <TextBlock Text="Loading..." 
                   HorizontalAlignment="Center" 
                   Margin="0,12,0,0"
                   Foreground="White" />
    </StackPanel>
</Grid>
```

---

## 🔧 Custom Controls and Behaviors

### Behavior Implementation

```xml
<!-- Auto-complete behavior integration -->
<TextBox Text="{Binding PartId}" 
         Watermark="Enter Part ID">
    <Interaction.Behaviors>
        <behaviors:AutoCompleteBoxNavigationBehavior 
            SuggestionSource="{Binding PartIds}"
            SelectedSuggestion="{Binding SelectedPartId}" />
    </Interaction.Behaviors>
</TextBox>

<!-- Fuzzy validation behavior -->
<TextBox Text="{Binding PartId}">
    <Interaction.Behaviors>
        <behaviors:TextBoxFuzzyValidationBehavior 
            ValidationRules="{Binding PartIdValidationRules}"
            IsValid="{Binding IsPartIdValid, Mode=TwoWay}" />
    </Interaction.Behaviors>
</TextBox>

<!-- ComboBox behavior for enhanced functionality -->
<ComboBox ItemsSource="{Binding Operations}" 
          SelectedItem="{Binding SelectedOperation}">
    <Interaction.Behaviors>
        <behaviors:ComboBoxBehavior 
            AllowCustomInput="True"
            FilterOnInput="True" />
    </Interaction.Behaviors>
</ComboBox>
```

### Custom Control Integration

```xml
<!-- CollapsiblePanel custom control -->
<controls:CollapsiblePanel Header="Advanced Options" 
                          IsExpanded="{Binding ShowAdvancedOptions}">
    <StackPanel Spacing="12">
        <TextBox Text="{Binding AdditionalNotes}" Watermark="Additional notes..." />
        <CheckBox Content="Require approval" IsChecked="{Binding RequiresApproval}" />
        <Button Content="Advanced Settings" Command="{Binding ShowAdvancedSettingsCommand}" />
    </StackPanel>
</controls:CollapsiblePanel>

<!-- Usage in larger layout -->
<StackPanel Spacing="16">
    <!-- Basic options always visible -->
    <Border Classes="mtm-card">
        <StackPanel Spacing="12">
            <TextBlock Classes="section-header" Text="Basic Inventory Information" />
            <TextBox Text="{Binding PartId}" Watermark="Part ID" />
            <ComboBox ItemsSource="{Binding Operations}" SelectedItem="{Binding SelectedOperation}" />
        </StackPanel>
    </Border>
    
    <!-- Advanced options in collapsible panel -->
    <controls:CollapsiblePanel Header="Advanced Options">
        <Border Classes="mtm-card">
            <!-- Advanced form fields -->
        </Border>
    </controls:CollapsiblePanel>
</StackPanel>
```

---

## 🎨 View Code-Behind Patterns

### Minimal Code-Behind Standard (MTM Pattern)

```csharp
// InventoryTabView.axaml.cs - Standard MTM pattern
public partial class InventoryTabView : UserControl
{
    public InventoryTabView()
    {
        InitializeComponent();
        // Minimal initialization only - ViewModel injected via DI
    }
    
    // Resource cleanup on detach
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Clean up event subscriptions, timers, etc.
        if (DataContext is IDisposable disposableContext)
        {
            disposableContext.Dispose();
        }
        
        base.OnDetachedFromVisualTree(e);
    }
    
    // Event handlers only for UI-specific logic that can't be in ViewModel
    private void OnTextBox_GotFocus(object? sender, GotFocusEventArgs e)
    {
        // UI-specific focus handling that doesn't belong in ViewModel
        if (sender is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}

// AVOID: Heavy business logic in code-behind
public partial class InventoryTabView : UserControl
{
    // WRONG: Business logic belongs in ViewModel
    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        // Don't do database operations here
        // Don't do business validation here
        // Don't do service calls here
    }
}
```

### View-ViewModel Connection Pattern

```csharp
// MainWindow.axaml.cs - Window-level coordination
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // ViewModel is injected via DI container, not created here
        // DataContext is set by the DI container or parent coordinator
    }
    
    // Window-specific event handling
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        // Check for unsaved changes through ViewModel
        if (DataContext is MainWindowViewModel viewModel && viewModel.HasUnsavedChanges)
        {
            // Show confirmation dialog
            e.Cancel = !ShowUnsavedChangesDialog();
        }
        
        base.OnClosing(e);
    }
    
    private bool ShowUnsavedChangesDialog()
    {
        // Simple confirmation logic - complex logic stays in ViewModel
        var result = MessageBox.Show(
            "You have unsaved changes. Do you want to exit?",
            "Unsaved Changes",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
            
        return result == MessageBoxResult.Yes;
    }
}
```

---

## 📱 Accessibility and User Experience

### Accessibility Standards Implementation

```xml
<!-- Keyboard navigation support -->
<StackPanel KeyboardNavigation.TabNavigation="Continue">
    <TextBox Text="{Binding PartId}" 
             TabIndex="1"
             AutomationProperties.Name="Part ID"
             AutomationProperties.HelpText="Enter the part identification number" />
    
    <ComboBox ItemsSource="{Binding Operations}"
              TabIndex="2"
              AutomationProperties.Name="Operation"
              AutomationProperties.HelpText="Select the manufacturing operation" />
    
    <Button Content="Save"
            TabIndex="3"
            AutomationProperties.Name="Save Inventory"
            AutomationProperties.HelpText="Save the inventory transaction" />
</StackPanel>

<!-- High contrast theme support -->
<TextBox Text="{Binding PartId}">
    <TextBox.Styles>
        <!-- High contrast mode styling -->
        <Style Selector="^[SystemParameters.HighContrast=True]">
            <Setter Property="Background" Value="White" />
            <Setter Property="Foreground" Value="Black" />
            <Setter Property="BorderBrush" Value="Black" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
    </TextBox.Styles>
</TextBox>

<!-- Screen reader support -->
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
    </Grid.RowDefinitions>
    
    <!-- Screen reader announces this as a landmark -->
    <TextBlock Grid.Row="0" 
               AutomationProperties.AutomationId="PageTitle"
               AutomationProperties.LandmarkType="Main"
               Classes="section-header"
               Text="Inventory Management" />
    
    <!-- Main content area -->
    <ScrollViewer Grid.Row="1" 
                  AutomationProperties.LandmarkType="Main"
                  AutomationProperties.Name="Main content area">
        <!-- Form content -->
    </ScrollViewer>
</Grid>
```

### Touch and Mobile-Friendly Design

```xml
<!-- Touch-friendly button sizes (minimum 44px) -->
<Button Content="Add Inventory"
        MinHeight="44"
        MinWidth="120"
        Padding="16,12"
        FontSize="14" />

<!-- Touch-friendly list items -->
<ListBox ItemsSource="{Binding QuickButtons}">
    <ListBox.ItemContainerStyle>
        <Style TargetType="ListBoxItem">
            <Setter Property="MinHeight" Value="48" />
            <Setter Property="Padding" Value="16,8" />
        </Style>
    </ListBox.ItemContainerStyle>
</ListBox>

<!-- Responsive grid that adapts to screen size -->
<Grid x:Name="ResponsiveLayout">
    <!-- Desktop layout: Side-by-side -->
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="*" />
    </Grid.ColumnDefinitions>
    
    <StackPanel Grid.Column="0" Spacing="12">
        <!-- Left column content -->
    </StackPanel>
    
    <StackPanel Grid.Column="1" Spacing="12">
        <!-- Right column content -->
    </StackPanel>
</Grid>
```

---

## 🧪 Testing UI Components

### AXAML Testing Approaches

```csharp
// UI testing with Avalonia.Headless
[Fact]
public async Task InventoryTabView_LoadsWithCorrectInitialState()
{
    // Arrange
    using var app = AvaloniaApp.BuildAvaloniaApp().StartWithClassicDesktopLifetime(Array.Empty<string>());
    
    var viewModel = new InventoryTabViewModel(mockLogger, mockInventoryService, mockMasterDataService);
    var view = new InventoryTabView { DataContext = viewModel };
    
    // Act - Simulate the view loading
    var window = new Window { Content = view };
    window.Show();
    
    // Assert - Check initial UI state
    var partIdTextBox = view.FindControl<TextBox>("PartIdTextBox");
    Assert.NotNull(partIdTextBox);
    Assert.Equal(string.Empty, partIdTextBox.Text);
    
    var saveButton = view.FindControl<Button>("SaveButton");
    Assert.NotNull(saveButton);
    Assert.False(saveButton.IsEnabled); // Should be disabled initially
}

// ViewModel testing with UI interaction simulation
[Fact]
public async Task InventoryTabViewModel_SaveCommand_UpdatesUIState()
{
    // Arrange
    var viewModel = new InventoryTabViewModel(mockLogger, mockInventoryService, mockMasterDataService);
    viewModel.PartId = "TEST001";
    viewModel.Operation = "100";
    viewModel.Quantity = 5;
    viewModel.Location = "A01";
    
    // Act
    await viewModel.SaveCommand.ExecuteAsync(null);
    
    // Assert
    Assert.False(viewModel.IsLoading);
    Assert.Contains("success", viewModel.StatusMessage.ToLowerInvariant());
}
```

---

## 🚀 Advanced Avalonia UI Patterns

### Complex Data Binding with Manufacturing Context

#### Advanced Converter Patterns for Manufacturing Data

```csharp
// Manufacturing-specific value converters for inventory operations
public class QuantityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int quantity)
        {
            // Manufacturing color coding for inventory levels
            return quantity switch
            {
                <= 0 => Brushes.Red,        // Out of stock - critical
                <= 10 => Brushes.Orange,    // Low stock - warning  
                <= 50 => Brushes.Yellow,    // Medium stock - caution
                _ => Brushes.LimeGreen      // Good stock - normal
            };
        }
        return Brushes.Gray; // Default for invalid data
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("QuantityToColorConverter is one-way only");
    }
}

// Multi-binding converter for manufacturing workflow validation
public class ManufacturingOperationValidationConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values?.Count >= 4 && 
            values[0] is string partId && 
            values[1] is string operation && 
            values[2] is int quantity && 
            values[3] is string location)
        {
            // Manufacturing business rules validation
            var validationResults = new List<string>();
            
            if (string.IsNullOrWhiteSpace(partId))
                validationResults.Add("Part ID is required");
                
            if (string.IsNullOrWhiteSpace(operation))
                validationResults.Add("Operation is required");
                
            if (quantity <= 0)
                validationResults.Add("Quantity must be greater than 0");
                
            if (string.IsNullOrWhiteSpace(location))
                validationResults.Add("Location is required");
                
            // Manufacturing-specific operation validation
            if (operation == "90" && quantity > 1000)
                validationResults.Add("Receiving operation limited to 1000 units per transaction");
                
            if (operation == "130" && string.IsNullOrWhiteSpace(location))
                validationResults.Add("Shipping operation requires specific location");

            return validationResults.Count == 0 
                ? "✅ Ready for manufacturing operation" 
                : $"❌ Validation errors: {string.Join(", ", validationResults)}";
        }
        
        return "⚠️ Incomplete manufacturing operation data";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ManufacturingOperationValidationConverter is one-way only");
    }
}
```

#### Complex DataTemplate Patterns for Manufacturing Data

```axaml
<!-- Advanced DataTemplate for manufacturing transaction history -->
<DataTemplate x:Key="ManufacturingTransactionTemplate" x:DataType="models:TransactionRecord">
    <Border Classes="mtm-card transaction-card" 
            Margin="4"
            Padding="12"
            Background="{DynamicResource MTM_Shared_Logic.ContentAreas}">
        <Grid RowDefinitions="Auto,Auto,Auto,Auto" 
              ColumnDefinitions="60,*,100,80">
            
            <!-- Transaction Type Icon -->
            <Border Grid.Row="0" Grid.Column="0" Grid.RowSpan="2"
                    Width="50" Height="50"
                    CornerRadius="25"
                    HorizontalAlignment="Center"
                    VerticalAlignment="Center">
                <Border.Background>
                    <MultiBinding Converter="{StaticResource TransactionTypeToColorConverter}">
                        <Binding Path="TransactionType"/>
                        <Binding Path="IsSuccessful"/>
                    </MultiBinding>
                </Border.Background>
                
                <TextBlock Text="{Binding TransactionType, Converter={StaticResource TransactionTypeToIconConverter}}"
                           FontFamily="Segoe MDL2 Assets"
                           FontSize="24"
                           Foreground="White"
                           HorizontalAlignment="Center"
                           VerticalAlignment="Center"/>
            </Border>
            
            <!-- Manufacturing Details -->
            <StackPanel Grid.Row="0" Grid.Column="1" Orientation="Horizontal" Spacing="16">
                <TextBlock Text="{Binding PartId}" 
                           FontWeight="Bold"
                           FontSize="14"
                           Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}"/>
                           
                <TextBlock Text="@" 
                           Foreground="{DynamicResource MTM_Shared_Logic.TertiaryTextBrush}"/>
                           
                <TextBlock Text="{Binding Operation}" 
                           FontWeight="Medium"
                           Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}"/>
                           
                <Border Background="{DynamicResource MTM_Shared_Logic.Highlight}"
                        CornerRadius="3"
                        Padding="6,2">
                    <TextBlock Text="{Binding Location}" 
                               FontSize="10"
                               Foreground="White"/>
                </Border>
            </StackPanel>
            
            <!-- Quantity and Status -->
            <StackPanel Grid.Row="0" Grid.Column="2" 
                        Orientation="Horizontal" 
                        HorizontalAlignment="Right"
                        Spacing="8">
                <TextBlock Text="{Binding Quantity, StringFormat='N0'}" 
                           FontSize="16"
                           FontWeight="Bold">
                    <TextBlock.Foreground>
                        <MultiBinding Converter="{StaticResource QuantityDirectionToColorConverter}">
                            <Binding Path="TransactionType"/>
                            <Binding Path="Quantity"/>
                        </MultiBinding>
                    </TextBlock.Foreground>
                </TextBlock>
                
                <TextBlock Text="units" 
                           FontSize="10"
                           VerticalAlignment="Bottom"
                           Foreground="{DynamicResource MTM_Shared_Logic.TertiaryTextBrush}"/>
            </StackPanel>
            
            <!-- Timestamp -->
            <TextBlock Grid.Row="0" Grid.Column="3"
                       Text="{Binding Timestamp, StringFormat='HH:mm'}"
                       FontSize="12"
                       HorizontalAlignment="Right"
                       VerticalAlignment="Top"
                       Foreground="{DynamicResource MTM_Shared_Logic.BodyText}"/>
            
            <!-- Manufacturing Workflow Context -->
            <TextBlock Grid.Row="1" Grid.Column="1" Grid.ColumnSpan="3"
                       Margin="0,4,0,0"
                       FontSize="11"
                       Foreground="{DynamicResource MTM_Shared_Logic.TertiaryTextBrush}">
                <Run Text="User:"/>
                <Run Text="{Binding UserId}" FontWeight="Medium"/>
                <Run Text=" • "/>
                <Run Text="{Binding WorkOrder, TargetNullValue='No Work Order'}"/>
                <Run Text=" • "/>
                <Run Text="{Binding BatchNumber, TargetNullValue='No Batch'}"/>
            </TextBlock>
            
            <!-- Error Details (if any) -->
            <Border Grid.Row="2" Grid.Column="1" Grid.ColumnSpan="3"
                    IsVisible="{Binding HasErrors}"
                    Background="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
                    CornerRadius="4"
                    Padding="8,4"
                    Margin="0,4,0,0">
                <TextBlock Text="{Binding ErrorMessage}"
                           FontSize="10"
                           Foreground="White"
                           TextWrapping="Wrap"/>
            </Border>
            
            <!-- Quick Actions for Manufacturing Operations -->
            <StackPanel Grid.Row="3" Grid.Column="0" Grid.ColumnSpan="4"
                        Orientation="Horizontal"
                        Spacing="8"
                        Margin="0,8,0,0"
                        HorizontalAlignment="Right">
                        
                <Button Classes="mtm-quick-action"
                        Content="📋 Details"
                        Command="{Binding $parent[UserControl].DataContext.ShowTransactionDetailsCommand}"
                        CommandParameter="{Binding}"/>
                        
                <Button Classes="mtm-quick-action"
                        Content="🔄 Reverse"
                        Command="{Binding $parent[UserControl].DataContext.ReverseTransactionCommand}"
                        CommandParameter="{Binding}"
                        IsVisible="{Binding CanReverse}"/>
                        
                <Button Classes="mtm-quick-action"
                        Content="⚡ QuickButton"
                        Command="{Binding $parent[UserControl].DataContext.CreateQuickButtonCommand}"
                        CommandParameter="{Binding}"/>
            </StackPanel>
        </Grid>
    </Border>
</DataTemplate>
```

### Advanced Custom Control Patterns

#### Manufacturing Data Grid with Virtual Scrolling

```csharp
// Custom virtualized DataGrid optimized for manufacturing datasets
public class ManufacturingDataGrid : DataGrid
{
    public static readonly StyledProperty<bool> EnableManufacturingFeaturesProperty =
        AvaloniaProperty.Register<ManufacturingDataGrid, bool>(
            nameof(EnableManufacturingFeatures), 
            defaultValue: true);

    public bool EnableManufacturingFeatures
    {
        get => GetValue(EnableManufacturingFeaturesProperty);
        set => SetValue(EnableManufacturingFeaturesProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(DataGrid);

    public ManufacturingDataGrid()
    {
        // Manufacturing-optimized defaults
        CanUserReorderColumns = true;
        CanUserResizeColumns = true;
        CanUserSortColumns = true;
        SelectionMode = DataGridSelectionMode.Extended;
        GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
        
        // Performance optimizations for large manufacturing datasets
        EnableRowVirtualization = true;
        EnableColumnVirtualization = true;
        
        // Manufacturing-specific styling
        RowBackground = new SolidColorBrush(Colors.Transparent);
        AlternatingRowBackground = new SolidColorBrush(Color.FromArgb(25, 0, 120, 212));
        
        this.GetObservable(EnableManufacturingFeaturesProperty)
            .Subscribe(OnManufacturingFeaturesChanged);
    }

    private void OnManufacturingFeaturesChanged(bool enableFeatures)
    {
        if (enableFeatures)
        {
            EnableManufacturingContextMenus();
            EnableManufacturingKeyboardShortcuts();
            EnableManufacturingColumnFormatting();
        }
    }

    private void EnableManufacturingContextMenus()
    {
        var contextMenu = new ContextMenu
        {
            Items =
            {
                new MenuItem
                {
                    Header = "📋 Copy Part ID",
                    Command = ReactiveCommand.Create(CopyPartId)
                },
                new MenuItem
                {
                    Header = "⚡ Create QuickButton",
                    Command = ReactiveCommand.Create(CreateQuickButtonFromSelection)
                },
                new Separator(),
                new MenuItem
                {
                    Header = "📊 Export Selection",
                    Command = ReactiveCommand.Create(ExportSelectedRows)
                },
                new MenuItem
                {
                    Header = "🖨️ Print Selection", 
                    Command = ReactiveCommand.Create(PrintSelectedRows)
                }
            }
        };

        ContextMenu = contextMenu;
    }

    private void EnableManufacturingKeyboardShortcuts()
    {
        KeyBindings.Add(new KeyBinding
        {
            Command = ReactiveCommand.Create(CopyPartId),
            Gesture = new KeyGesture(Key.C, KeyModifiers.Control | KeyModifiers.Shift)
        });

        KeyBindings.Add(new KeyBinding
        {
            Command = ReactiveCommand.Create(CreateQuickButtonFromSelection),
            Gesture = new KeyGesture(Key.Q, KeyModifiers.Control)
        });
    }

    private void EnableManufacturingColumnFormatting()
    {
        LoadingRow += (sender, e) =>
        {
            if (e.Row.DataContext is InventoryItem item)
            {
                // Manufacturing-specific row styling based on inventory levels
                if (item.Quantity <= 0)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(50, 244, 67, 54)); // Light red
                }
                else if (item.Quantity <= 10)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(50, 255, 152, 0)); // Light orange
                }
            }
        };
    }

    private void CopyPartId()
    {
        if (SelectedItem is InventoryItem item)
        {
            Application.Current?.Clipboard?.SetTextAsync(item.PartId);
        }
    }

    private void CreateQuickButtonFromSelection()
    {
        if (DataContext is IManufacturingViewModel viewModel && SelectedItem is InventoryItem item)
        {
            viewModel.CreateQuickButtonCommand?.Execute(item);
        }
    }
}
```

#### Advanced Manufacturing Input Control

```csharp
// Specialized input control for manufacturing part IDs and operations
public class ManufacturingPartInput : UserControl
{
    public static readonly StyledProperty<string> PartIdProperty =
        AvaloniaProperty.Register<ManufacturingPartInput, string>(
            nameof(PartId), 
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> OperationProperty =
        AvaloniaProperty.Register<ManufacturingPartInput, string>(
            nameof(Operation), 
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> EnableAutoCompleteProperty =
        AvaloniaProperty.Register<ManufacturingPartInput, bool>(
            nameof(EnableAutoComplete), 
            defaultValue: true);

    private TextBox _partIdTextBox;
    private ComboBox _operationComboBox;
    private Border _validationBorder;
    private IDisposable _validationSubscription;

    public string PartId
    {
        get => GetValue(PartIdProperty);
        set => SetValue(PartIdProperty, value);
    }

    public string Operation
    {
        get => GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    public bool EnableAutoComplete
    {
        get => GetValue(EnableAutoCompleteProperty);
        set => SetValue(EnableAutoCompleteProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _partIdTextBox = e.NameScope.Find<TextBox>("PART_PartIdTextBox");
        _operationComboBox = e.NameScope.Find<ComboBox>("PART_OperationComboBox");
        _validationBorder = e.NameScope.Find<Border>("PART_ValidationBorder");

        if (_partIdTextBox != null)
        {
            _partIdTextBox.TextChanged += OnPartIdTextChanged;
            _partIdTextBox.LostFocus += OnPartIdLostFocus;
        }

        if (_operationComboBox != null)
        {
            _operationComboBox.SelectionChanged += OnOperationSelectionChanged;
        }

        SetupValidation();
    }

    private void SetupValidation()
    {
        // Real-time validation for manufacturing part IDs
        _validationSubscription = Observable.CombineLatest(
            this.GetObservable(PartIdProperty),
            this.GetObservable(OperationProperty),
            (partId, operation) => new { PartId = partId, Operation = operation })
            .Throttle(TimeSpan.FromMilliseconds(300))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(async data => await ValidateInputAsync(data.PartId, data.Operation));
    }

    private async Task ValidateInputAsync(string partId, string operation)
    {
        if (_validationBorder == null) return;

        var validationResult = await ValidateManufacturingInputAsync(partId, operation);
        
        Dispatcher.UIThread.Post(() =>
        {
            if (validationResult.IsValid)
            {
                _validationBorder.BorderBrush = Brushes.LimeGreen;
                _validationBorder.BorderThickness = new Thickness(2);
                ToolTip.SetTip(_validationBorder, "✅ Valid manufacturing part and operation");
            }
            else
            {
                _validationBorder.BorderBrush = Brushes.Red;
                _validationBorder.BorderThickness = new Thickness(2);
                ToolTip.SetTip(_validationBorder, $"❌ {validationResult.ErrorMessage}");
            }
        });
    }

    private async Task<ValidationResult> ValidateManufacturingInputAsync(string partId, string operation)
    {
        // Simulate manufacturing validation (would be injected service in real implementation)
        if (string.IsNullOrWhiteSpace(partId))
            return ValidationResult.Invalid("Part ID is required");

        if (!Regex.IsMatch(partId, @"^[A-Z0-9\-]{3,50}$"))
            return ValidationResult.Invalid("Part ID format invalid (3-50 chars, A-Z, 0-9, dash)");

        if (string.IsNullOrWhiteSpace(operation))
            return ValidationResult.Invalid("Operation is required");

        var validOperations = new[] { "90", "100", "110", "120", "130" };
        if (!validOperations.Contains(operation))
            return ValidationResult.Invalid($"Operation must be one of: {string.Join(", ", validOperations)}");

        return ValidationResult.Valid();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _validationSubscription?.Dispose();
        base.OnDetachedFromLogicalTree(e);
    }
}
```

### ❌ Avalonia UI Anti-Patterns (Avoid These)

#### Performance Anti-Patterns

```axaml
<!-- ❌ WRONG: Binding to complex properties in ItemTemplate causes performance issues -->
<ListBox ItemsSource="{Binding LargeManufacturingDataset}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <!-- BAD: Complex calculation in binding expression -->
            <TextBlock Text="{Binding ., Converter={StaticResource ComplexCalculationConverter}}" />
            
            <!-- BAD: Multiple converter chains -->
            <TextBlock Text="{Binding SomeProperty, 
                Converter={StaticResource FirstConverter}, 
                ConverterParameter={Binding SecondProperty, 
                    Converter={StaticResource SecondConverter}}}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>

<!-- ✅ CORRECT: Pre-calculated properties in ViewModel -->
<ListBox ItemsSource="{Binding LargeManufacturingDataset}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <!-- GOOD: Simple property binding -->
            <TextBlock Text="{Binding PreCalculatedDisplayValue}" />
            
            <!-- GOOD: Simple conversion -->
            <TextBlock Text="{Binding Quantity, StringFormat='N0'}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### Memory Leak Anti-Patterns

```csharp
// ❌ WRONG: Creating controls without proper disposal
public partial class ManufacturingDashboard : UserControl
{
    private Timer _refreshTimer;
    
    public ManufacturingDashboard()
    {
        InitializeComponent();
        
        // BAD: Timer never disposed - memory leak
        _refreshTimer = new Timer(RefreshData, null, 0, 5000);
        
        // BAD: Event subscription without cleanup
        SomeStaticEventPublisher.DataUpdated += OnDataUpdated;
    }
    
    private void OnDataUpdated(object sender, EventArgs e)
    {
        // Handler will keep this control alive forever
    }
}

// ✅ CORRECT: Proper resource management
public partial class ManufacturingDashboard : UserControl, IDisposable
{
    private Timer? _refreshTimer;
    private bool _disposed = false;
    
    public ManufacturingDashboard()
    {
        InitializeComponent();
        
        _refreshTimer = new Timer(RefreshData, null, 0, 5000);
        SomeStaticEventPublisher.DataUpdated += OnDataUpdated;
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _refreshTimer?.Dispose();
            SomeStaticEventPublisher.DataUpdated -= OnDataUpdated;
            _disposed = true;
        }
    }
}
```

#### UI Thread Blocking Anti-Patterns

```csharp
// ❌ WRONG: Blocking UI thread during manufacturing data operations
private void LoadManufacturingDataButton_Click(object sender, RoutedEventArgs e)
{
    // BAD: Synchronous database call blocks UI
    var data = _manufacturingService.GetAllInventoryData().Result;
    
    // BAD: Large collection update on UI thread
    InventoryItems.Clear();
    foreach (var item in data) // Could be thousands of items
    {
        InventoryItems.Add(item); // UI freezes
    }
}

// ✅ CORRECT: Async operations with progress feedback
private async Task LoadManufacturingDataAsync()
{
    try
    {
        IsLoading = true;
        StatusMessage = "Loading manufacturing data...";
        
        // GOOD: Async database call
        var data = await _manufacturingService.GetAllInventoryDataAsync();
        
        // GOOD: Batch UI updates to maintain responsiveness
        InventoryItems.Clear();
        
        const int batchSize = 100;
        for (int i = 0; i < data.Count; i += batchSize)
        {
            var batch = data.Skip(i).Take(batchSize);
            
            // Update UI in batches
            foreach (var item in batch)
            {
                InventoryItems.Add(item);
            }
            
            // Allow UI to update between batches
            await Task.Delay(10);
            
            // Update progress
            ProgressValue = (i + batchSize) * 100 / data.Count;
        }
        
        StatusMessage = $"Loaded {data.Count} inventory items";
    }
    catch (Exception ex)
    {
        await HandleErrorAsync(ex, "Load manufacturing data");
    }
    finally
    {
        IsLoading = false;
    }
}
```

## 🔧 Manufacturing UI Troubleshooting Guide

### Common Avalonia Issues in Manufacturing Context

#### Issue: DataGrid Performance with Large Manufacturing Datasets

**Symptoms**: UI freezes or becomes unresponsive when loading inventory data

**Solution**: Enable virtualization and implement paging

```axaml
<DataGrid ItemsSource="{Binding PagedInventoryData}"
          EnableRowVirtualization="True"
          EnableColumnVirtualization="True"
          VirtualizationMode="Recycling"
          MaxHeight="600">
    <!-- Column definitions -->
</DataGrid>
```

#### Issue: Memory Usage Increases During Manufacturing Operations  

**Symptoms**: Application memory grows during shift operations

**Solution**: Implement proper collection management

```csharp
// Limit collection size and clean up old data
private void CleanupOldTransactions()
{
    const int maxTransactions = 1000;
    
    if (TransactionHistory.Count > maxTransactions)
    {
        var itemsToRemove = TransactionHistory
            .OrderBy(t => t.Timestamp)
            .Take(TransactionHistory.Count - maxTransactions)
            .ToList();
            
        foreach (var item in itemsToRemove)
        {
            TransactionHistory.Remove(item);
        }
    }
}
```

#### Issue: UI Not Updating During Manufacturing Batch Operations

**Symptoms**: UI appears frozen during large batch processing

**Solution**: Use progress reporting and batch UI updates

```csharp
public async Task ProcessManufacturingBatchAsync(List<InventoryOperation> operations)
{
    var progress = new Progress<BatchProgress>(p =>
    {
        ProgressValue = p.CompletedItems;
        StatusMessage = $"Processing {p.CompletedItems}/{p.TotalItems} operations...";
    });
    
    await _batchProcessor.ProcessOperationsAsync(operations, progress);
}
```

---

## 📚 Related UI Documentation

- **Theme System**: [MTM Design System Documentation](../../Resources/Themes/README.md)
- **MVVM Patterns**: [Community Toolkit Implementation](./mvvm-community-toolkit.instructions.md)
- **Custom Controls**: [MTM Controls Documentation](../../Controls/README.md)
- **Behaviors**: [Avalonia Behaviors Guide](../../Behaviors/README.md)

---

**Document Status**: ✅ Complete UI Reference  
**Framework Version**: Avalonia UI 11.3.4  
**Last Updated**: September 4, 2025  
**UI Standards Owner**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**

- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**

- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**

- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**

- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**

- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**

- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**

- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.