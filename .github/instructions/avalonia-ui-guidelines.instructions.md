# Avalonia UI Guidelines - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Target Framework**: .NET 8  
**UI Pattern**: MVVM with Community Toolkit  
**Created**: September 4, 2025  

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
<TextBlock Text="{Binding LastUpdated, StringFormat='Last Updated: September 07, 2025

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
