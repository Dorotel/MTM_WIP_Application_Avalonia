# Views Directory

This directory contains all Avalonia AXAML view files that define the user interface for the MTM WIP Application Avalonia (.NET 8).

## ??? View Architecture

### Avalonia 11+ with MVVM (.NET 8)
All views follow Avalonia 11+ patterns optimized for .NET 8:
- **AXAML Markup**: Declarative UI using Avalonia 11+ syntax and features
- **Compiled Bindings**: Type-safe bindings with compile-time validation and null safety
- **ReactiveUI Integration**: Seamless binding to ReactiveUI ViewModels with modern C# patterns
- **Modern Design**: Responsive design following MTM brand guidelines with Avalonia 11+ controls
- **Performance Optimized**: Leveraging Avalonia 11+ performance improvements

## ?? View Files

### Core Views

#### `MainView.axaml` / `MainView.axaml.cs`
Primary content area view using Avalonia 11+ layout patterns.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainView"
             x:DataType="vm:MainViewViewModel"
             x:CompileBindings="True">
    
    <Design.DataContext>
        <vm:MainViewViewModel/>
    </Design.DataContext>
    
    <!-- Modern Grid with Avalonia 11+ patterns -->
    <Grid RowDefinitions="Auto,*,Auto" ColumnDefinitions="*,Auto">
        <!-- Header Section with Avalonia 11+ styling -->
        <Border Grid.Row="0" Grid.ColumnSpan="2" 
                Classes="header-section"
                Padding="24,16">
            <Grid ColumnDefinitions="*,Auto">
                <StackPanel Grid.Column="0" Orientation="Horizontal" Spacing="16">
                    <PathIcon Data="{StaticResource InventoryIcon}" 
                              Width="32" Height="32"
                              Foreground="{DynamicResource AccentBrush}"/>
                    <StackPanel Spacing="4">
                        <TextBlock Text="MTM WIP Inventory System"
                                   FontSize="24"
                                   FontWeight="SemiBold"
                                   Foreground="{DynamicResource TextBrush}"/>
                        <TextBlock Text="Manage your inventory efficiently"
                                   FontSize="14"
                                   Foreground="{DynamicResource TextSecondaryBrush}"/>
                    </StackPanel>
                </StackPanel>
                
                <!-- User info with modern styling -->
                <Border Grid.Column="1" 
                        Classes="user-info"
                        Padding="12,8"
                        CornerRadius="8">
                    <StackPanel Orientation="Horizontal" Spacing="8">
                        <PathIcon Data="{StaticResource UserIcon}"
                                  Width="16" Height="16"/>
                        <TextBlock Text="{Binding CurrentUser, FallbackValue='Not Logged In'}"
                                   FontWeight="Medium"/>
                    </StackPanel>
                </Border>
            </Grid>
        </Border>
        
        <!-- Main Content with TabView (Avalonia 11+ control) -->
        <TabView Grid.Row="1" Grid.Column="0"
                 ItemsSource="{Binding Tabs}"
                 SelectedIndex="{Binding SelectedTabIndex}"
                 Classes="modern-tabs"
                 Margin="24,0,24,24">
            <TabView.ItemTemplate>
                <DataTemplate DataType="vm:TabItemViewModel">
                    <TabViewItem Header="{Binding Title}"
                                 IconSource="{Binding IconSource}"
                                 Content="{Binding Content}"/>
                </DataTemplate>
            </TabView.ItemTemplate>
        </TabView>
        
        <!-- Sidebar with modern collapsible panel -->
        <Expander Grid.Row="1" Grid.Column="1"
                  Header="Quick Actions"
                  ExpandDirection="Left"
                  IsExpanded="True"
                  Classes="sidebar-expander"
                  Width="300"
                  Margin="0,0,24,24">
            <Border Classes="sidebar-content">
                <ContentControl Content="{Binding QuickActionsViewModel}"/>
            </Border>
        </Expander>
        
        <!-- Status Bar with modern styling -->
        <Border Grid.Row="2" Grid.ColumnSpan="2"
                Classes="status-bar"
                Padding="24,8">
            <Grid ColumnDefinitions="*,Auto,Auto,Auto">
                <TextBlock Grid.Column="0"
                           Text="{Binding StatusMessage, FallbackValue='Ready'}"
                           VerticalAlignment="Center"/>
                
                <ProgressBar Grid.Column="1"
                             IsVisible="{Binding IsLoading}"
                             IsIndeterminate="True"
                             Width="120"
                             Margin="8,0"/>
                
                <TextBlock Grid.Column="2"
                           Text="{Binding ItemCount, StringFormat='Items: {0}'}"
                           VerticalAlignment="Center"
                           Margin="16,0,0,0"/>
                
                <TextBlock Grid.Column="3"
                           Text="{Binding LastUpdated, StringFormat='Updated: {0:HH:mm:ss}'}"
                           VerticalAlignment="Center"
                           Margin="16,0,0,0"
                           FontSize="12"
                           Foreground="{DynamicResource TextSecondaryBrush}"/>
            </Grid>
        </Border>
    </Grid>
</UserControl>
```

**Avalonia 11+ Features Used:**
- **TabView Control**: Modern tab interface with icons and improved styling
- **PathIcon**: Vector icons with proper scaling and theming
- **Enhanced Grid**: Improved layout performance and syntax
- **Modern Binding**: Compiled bindings with FallbackValue support
- **Improved Styling**: Better theme integration and responsive design

### Tab Views

#### `InventoryTabView.axaml` / `InventoryTabView.axaml.cs`
Primary inventory management interface using Avalonia 11+ form patterns.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.InventoryTabView"
             x:DataType="vm:InventoryTabViewModel"
             x:CompileBindings="True">
    
    <ScrollViewer Classes="modern-scroll">
        <StackPanel Spacing="24" Margin="24">
            <!-- Hero Section with Gradient Background -->
            <Border Classes="hero-section" 
                    Height="160"
                    CornerRadius="12">
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
                        <TextBlock Text="Add Inventory Item"
                                   FontSize="28"
                                   FontWeight="Bold"
                                   Foreground="White"/>
                        <TextBlock Text="Create new inventory entries with part tracking and location management"
                                   FontSize="16"
                                   Foreground="White"
                                   Opacity="0.9"/>
                    </StackPanel>
                </Grid>
            </Border>
            
            <!-- Form Section with Modern Card Layout -->
            <Border Classes="form-card">
                <Grid RowDefinitions="Auto,24,*,24,Auto" Margin="32">
                    <!-- Form Header -->
                    <Grid Grid.Row="0" ColumnDefinitions="Auto,16,*,Auto">
                        <PathIcon Grid.Column="0"
                                  Data="{StaticResource FormIcon}"
                                  Width="28" Height="28"
                                  Foreground="{DynamicResource AccentBrush}"/>
                        <StackPanel Grid.Column="2" Spacing="4">
                            <TextBlock Text="Inventory Details"
                                       FontSize="20"
                                       FontWeight="SemiBold"/>
                            <TextBlock Text="Enter the inventory item information"
                                       FontSize="14"
                                       Foreground="{DynamicResource TextSecondaryBrush}"/>
                        </StackPanel>
                        
                        <!-- Validation Indicator -->
                        <Border Grid.Column="3"
                                Classes.valid="{Binding IsFormValid}"
                                Classes.invalid="{Binding !IsFormValid}"
                                CornerRadius="12"
                                Padding="8,4">
                            <TextBlock Text="{Binding IsFormValid, Converter={StaticResource BoolToValidationTextConverter}}"
                                       FontSize="12"
                                       FontWeight="Medium"/>
                        </Border>
                    </Grid>
                    
                    <!-- Form Controls with Modern Styling -->
                    <Grid Grid.Row="2" 
                          RowDefinitions="Auto,16,Auto,16,Auto,16,Auto,16,Auto,24,Auto"
                          ColumnDefinitions="Auto,24,*,24,Auto,24,*">
                        
                        <!-- Part ID -->
                        <TextBlock Grid.Row="0" Grid.Column="0"
                                   Text="Part ID:"
                                   Classes="form-label"/>
                        <ComboBox Grid.Row="0" Grid.Column="2"
                                  ItemsSource="{Binding AvailableParts}"
                                  SelectedItem="{Binding SelectedPart}"
                                  IsEditable="True"
                                  Classes="modern-combobox"
                                  Watermark="Select or enter part ID..."/>
                        
                        <!-- Operation -->
                        <TextBlock Grid.Row="0" Grid.Column="4"
                                   Text="Operation:"
                                   Classes="form-label"/>
                        <ComboBox Grid.Row="0" Grid.Column="6"
                                  ItemsSource="{Binding AvailableOperations}"
                                  SelectedItem="{Binding SelectedOperation}"
                                  Classes="modern-combobox"
                                  Watermark="Select operation..."/>
                        
                        <!-- Location -->
                        <TextBlock Grid.Row="2" Grid.Column="0"
                                   Text="Location:"
                                   Classes="form-label"/>
                        <ComboBox Grid.Row="2" Grid.Column="2"
                                  ItemsSource="{Binding AvailableLocations}"
                                  SelectedItem="{Binding SelectedLocation}"
                                  Classes="modern-combobox"
                                  Watermark="Select location..."/>
                        
                        <!-- Quantity -->
                        <TextBlock Grid.Row="2" Grid.Column="4"
                                   Text="Quantity:"
                                   Classes="form-label"/>
                        <NumericUpDown Grid.Row="2" Grid.Column="6"
                                       Value="{Binding Quantity}"
                                       Minimum="1"
                                       Maximum="999999"
                                       Classes="modern-numeric"
                                       Watermark="Enter quantity..."/>
                        
                        <!-- Notes -->
                        <TextBlock Grid.Row="4" Grid.Column="0"
                                   Text="Notes:"
                                   Classes="form-label"
                                   VerticalAlignment="Top"
                                   Margin="0,8,0,0"/>
                        <TextBox Grid.Row="4" Grid.Column="2" Grid.ColumnSpan="5"
                                 Text="{Binding Notes}"
                                 AcceptsReturn="True"
                                 TextWrapping="Wrap"
                                 Height="80"
                                 Classes="modern-textbox multiline"
                                 Watermark="Optional notes or comments..."/>
                        
                        <!-- Validation Messages -->
                        <Border Grid.Row="6" Grid.Column="0" Grid.ColumnSpan="7"
                                IsVisible="{Binding HasValidationErrors}"
                                Classes="validation-panel">
                            <StackPanel Spacing="8">
                                <Grid ColumnDefinitions="Auto,12,*">
                                    <PathIcon Grid.Column="0"
                                              Data="{StaticResource WarningIcon}"
                                              Width="16" Height="16"
                                              Foreground="#FF6B35"/>
                                    <TextBlock Grid.Column="2"
                                               Text="Please correct the following:"
                                               FontWeight="SemiBold"
                                               Foreground="#FF6B35"/>
                                </Grid>
                                <ItemsControl ItemsSource="{Binding ValidationErrors}">
                                    <ItemsControl.ItemTemplate>
                                        <DataTemplate>
                                            <TextBlock Text="{Binding}"
                                                       Margin="28,0,0,4"
                                                       FontSize="13"
                                                       Foreground="#FF6B35"/>
                                        </DataTemplate>
                                    </ItemsControl.ItemTemplate>
                                </ItemsControl>
                            </StackPanel>
                        </Border>
                        
                        <!-- Action Buttons -->
                        <StackPanel Grid.Row="8" Grid.Column="0" Grid.ColumnSpan="7"
                                    Orientation="Horizontal" 
                                    Spacing="12"
                                    HorizontalAlignment="Right">
                            <Button Content="Reset Form"
                                    Command="{Binding ResetCommand}"
                                    Classes="secondary-button"/>
                            <Button Content="Advanced Entry"
                                    Command="{Binding AdvancedEntryCommand}"
                                    Classes="tertiary-button"/>
                            <Button Content="Save Item"
                                    Command="{Binding SaveCommand}"
                                    Classes="primary-button"
                                    IsDefault="True"/>
                        </StackPanel>
                        
                        <!-- Progress Indicator -->
                        <ProgressBar Grid.Row="10" Grid.Column="0" Grid.ColumnSpan="7"
                                     IsVisible="{Binding IsSaving}"
                                     IsIndeterminate="True"
                                     Height="4"
                                     Classes="modern-progress"/>
                    </Grid>
                    
                    <!-- Success/Error Messages -->
                    <Border Grid.Row="4"
                            IsVisible="{Binding HasMessage}"
                            Classes.success="{Binding IsSuccessMessage}"
                            Classes.error="{Binding IsErrorMessage}"
                            Classes="message-panel">
                        <Grid ColumnDefinitions="Auto,12,*,Auto">
                            <PathIcon Grid.Column="0"
                                      Data="{Binding MessageIcon}"
                                      Width="20" Height="20"/>
                            <TextBlock Grid.Column="2"
                                       Text="{Binding Message}"
                                       TextWrapping="Wrap"
                                       VerticalAlignment="Center"/>
                            <Button Grid.Column="3"
                                    Content="?"
                                    Command="{Binding ClearMessageCommand}"
                                    Classes="close-button"/>
                        </Grid>
                    </Border>
                </Grid>
            </Border>
            
            <!-- Recent Items Preview -->
            <Border Classes="preview-card"
                    IsVisible="{Binding HasRecentItems}">
                <Grid RowDefinitions="Auto,16,*" Margin="24">
                    <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*,Auto">
                        <PathIcon Grid.Column="0"
                                  Data="{StaticResource HistoryIcon}"
                                  Width="20" Height="20"
                                  Foreground="{DynamicResource AccentBrush}"/>
                        <TextBlock Grid.Column="2"
                                   Text="Recent Items"
                                   FontSize="16"
                                   FontWeight="SemiBold"/>
                        <Button Grid.Column="3"
                                Content="View All"
                                Command="{Binding ViewAllItemsCommand}"
                                Classes="text-button"/>
                    </Grid>
                    
                    <ListBox Grid.Row="2"
                             ItemsSource="{Binding RecentItems}"
                             Classes="recent-items-list"
                             MaxHeight="200">
                        <ListBox.ItemTemplate>
                            <DataTemplate DataType="vm:InventoryItemViewModel">
                                <Grid ColumnDefinitions="Auto,12,*,Auto,Auto" Margin="8">
                                    <Border Grid.Column="0"
                                            Classes="item-indicator"
                                            Background="{Binding StatusColor}"
                                            Width="8" Height="8"
                                            CornerRadius="4"/>
                                    <StackPanel Grid.Column="2" Spacing="2">
                                        <TextBlock Text="{Binding PartId}"
                                                   FontWeight="SemiBold"/>
                                        <TextBlock Text="{Binding LocationOperation}"
                                                   FontSize="12"
                                                   Foreground="{DynamicResource TextSecondaryBrush}"/>
                                    </StackPanel>
                                    <TextBlock Grid.Column="3"
                                               Text="{Binding Quantity, StringFormat='Qty: {0}'}"
                                               FontWeight="Medium"
                                               VerticalAlignment="Center"/>
                                    <TextBlock Grid.Column="4"
                                               Text="{Binding CreatedDate, StringFormat='{}{0:MMM dd}'}"
                                               FontSize="11"
                                               Foreground="{DynamicResource TextSecondaryBrush}"
                                               VerticalAlignment="Center"
                                               Margin="12,0,0,0"/>
                                </Grid>
                            </DataTemplate>
                        </ListBox.ItemTemplate>
                    </ListBox>
                </Grid>
            </Border>
        </StackPanel>
    </ScrollViewer>
</UserControl>
```

### Component Views

#### `QuickButtonsView.axaml` / `QuickButtonsView.axaml.cs`
Quick action buttons panel using Avalonia 11+ ItemsRepeater for performance.

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:Class="MTM_WIP_Application_Avalonia.Views.QuickButtonsView"
             x:DataType="vm:QuickButtonsViewModel"
             x:CompileBindings="True">
    
    <Border Classes="quick-buttons-container">
        <Grid RowDefinitions="Auto,16,*,16,Auto">
            <!-- Header -->
            <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*,Auto">
                <PathIcon Grid.Column="0"
                          Data="{StaticResource LightningIcon}"
                          Width="20" Height="20"
                          Foreground="{DynamicResource AccentBrush}"/>
                <TextBlock Grid.Column="2"
                           Text="Quick Actions"
                           FontSize="16"
                           FontWeight="SemiBold"/>
                <Button Grid.Column="3"
                        Content="??"
                        Command="{Binding ManageButtonsCommand}"
                        Classes="icon-button"
                        ToolTip.Tip="Manage quick buttons"/>
            </Grid>
            
            <!-- Quick Buttons using ItemsRepeater for performance -->
            <ScrollViewer Grid.Row="2" 
                          Classes="quick-scroll"
                          VerticalScrollBarVisibility="Auto">
                <ItemsRepeater ItemsSource="{Binding QuickButtons}"
                               Layout="{StaticResource UniformGridLayout}">
                    <ItemsRepeater.ItemTemplate>
                        <DataTemplate DataType="vm:QuickButtonItemViewModel">
                            <Button Classes="quick-action-button"
                                    Command="{Binding $parent[UserControl].((vm:QuickButtonsViewModel)DataContext).ExecuteQuickActionCommand}"
                                    CommandParameter="{Binding}"
                                    Margin="0,0,0,8">
                                <Button.ContextFlyout>
                                    <MenuFlyout>
                                        <MenuItem Header="Edit Button" 
                                                  Command="{Binding EditCommand}"
                                                  Icon="{StaticResource EditIcon}"/>
                                        <MenuItem Header="Remove Button" 
                                                  Command="{Binding RemoveCommand}"
                                                  Icon="{StaticResource DeleteIcon}"/>
                                        <Separator/>
                                        <MenuItem Header="Move Up" 
                                                  Command="{Binding MoveUpCommand}"
                                                  Icon="{StaticResource UpArrowIcon}"/>
                                        <MenuItem Header="Move Down" 
                                                  Command="{Binding MoveDownCommand}"
                                                  Icon="{StaticResource DownArrowIcon}"/>
                                    </MenuFlyout>
                                </Button.ContextFlyout>
                                
                                <Grid RowDefinitions="Auto,4,Auto,4,Auto">
                                    <TextBlock Grid.Row="0"
                                               Text="{Binding PartId}" 
                                               FontWeight="SemiBold"
                                               FontSize="13"
                                               HorizontalAlignment="Center"
                                               TextTrimming="CharacterEllipsis"/>
                                    <TextBlock Grid.Row="2"
                                               Text="{Binding OperationDisplay}" 
                                               FontSize="11" 
                                               Opacity="0.8"
                                               HorizontalAlignment="Center"/>
                                    <Border Grid.Row="4"
                                            Classes="quantity-badge"
                                            HorizontalAlignment="Center">
                                        <TextBlock Text="{Binding QuantityDisplay}" 
                                                   FontSize="10"
                                                   FontWeight="Medium"/>
                                    </Border>
                                </Grid>
                            </Button>
                        </DataTemplate>
                    </ItemsRepeater.ItemTemplate>
                </ItemsRepeater>
            </ScrollViewer>
            
            <!-- Management Actions -->
            <StackPanel Grid.Row="4" Spacing="8">
                <Button Content="Refresh Buttons"
                        Command="{Binding RefreshButtonsCommand}"
                        Classes="secondary-button full-width"/>
                <Button Content="Clear All"
                        Command="{Binding ClearAllButtonsCommand}"
                        Classes="danger-button full-width"/>
            </StackPanel>
        </Grid>
    </Border>
</UserControl>
```

## ?? Avalonia 11+ Design System Implementation

### Modern Control Usage
```xml
<!-- TabView (Avalonia 11+ control) -->
<TabView ItemsSource="{Binding Tabs}"
         SelectedIndex="{Binding SelectedTabIndex}"
         TabStripPlacement="Top"
         IsCloseButtonVisible="True">
    <TabView.ItemTemplate>
        <DataTemplate>
            <TabViewItem Header="{Binding Title}"
                         IconSource="{Binding IconSource}"/>
        </DataTemplate>
    </TabView.ItemTemplate>
</TabView>

<!-- NumberBox for numeric input -->
<NumberBox Value="{Binding Quantity}"
           Minimum="0"
           Maximum="999999"
           SpinButtonPlacementMode="Inline"
           Classes="modern-number"/>

<!-- InfoBadge for notifications -->
<InfoBadge Value="{Binding NotificationCount}"
           IsVisible="{Binding HasNotifications}"
           Classes="accent-badge"/>

<!-- MenuFlyout with modern styling -->
<Button Content="Options">
    <Button.Flyout>
        <MenuFlyout>
            <MenuItem Header="Edit" Icon="{StaticResource EditIcon}"/>
            <MenuItem Header="Delete" Icon="{StaticResource DeleteIcon}"/>
        </MenuFlyout>
    </Button.Flyout>
</Button>
```

### MTM Brand Colors with Avalonia 11+ Theming
```xml
<!-- Enhanced Resource System -->
<ResourceDictionary>
    <!-- Primary MTM Colors -->
    <SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#4B45ED"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>
    <SolidColorBrush x:Key="MagentaAccentBrush" Color="#BA45ED"/>
    <SolidColorBrush x:Key="BlueAccentBrush" Color="#4574ED"/>
    <SolidColorBrush x:Key="PinkAccentBrush" Color="#ED45E7"/>
    <SolidColorBrush x:Key="LightPurpleBrush" Color="#B594ED"/>

    <!-- Semantic Colors -->
    <SolidColorBrush x:Key="SuccessBrush" Color="#22C55E"/>
    <SolidColorBrush x:Key="WarningBrush" Color="#F59E0B"/>
    <SolidColorBrush x:Key="ErrorBrush" Color="#EF4444"/>
    <SolidColorBrush x:Key="InfoBrush" Color="#3B82F6"/>

    <!-- Gradient Brushes -->
    <LinearGradientBrush x:Key="HeroGradientBrush" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="#4574ED" Offset="0"/>
        <GradientStop Color="#4B45ED" Offset="0.3"/>
        <GradientStop Color="#8345ED" Offset="0.7"/>
        <GradientStop Color="#BA45ED" Offset="1"/>
    </LinearGradientBrush>

    <!-- Modern Shadow Effects -->
    <BoxShadow x:Key="CardShadow">0 4 12 0 #1A000000</BoxShadow>
    <BoxShadow x:Key="ElevatedShadow">0 8 24 0 #20000000</BoxShadow>
    <BoxShadow x:Key="FocusShadow">0 0 0 3 #4B45ED40</BoxShadow>
</ResourceDictionary>
```

### Modern Layout Patterns
```xml
<!-- Card-Based Layout with Shadows -->
<Border Classes="modern-card"
        BoxShadow="{StaticResource CardShadow}"
        CornerRadius="12"
        Padding="24">
    <!-- Card content -->
</Border>

<!-- Responsive Grid -->
<Grid RowDefinitions="Auto,*,Auto" 
      ColumnDefinitions="*,300"
      Classes="responsive-layout">
    <!-- Adapts to screen size -->
</Grid>

<!-- Hero Section with Gradient -->
<Border Height="200"
        CornerRadius="16"
        Background="{StaticResource HeroGradientBrush}"
        ClipToBounds="True">
    <Grid Margin="32">
        <StackPanel VerticalAlignment="Center" Spacing="12">
            <TextBlock Text="Welcome to MTM WIP"
                       FontSize="32"
                       FontWeight="Bold"
                       Foreground="White"/>
            <TextBlock Text="Modern inventory management"
                       FontSize="18"
                       Foreground="White"
                       Opacity="0.9"/>
        </StackPanel>
    </Grid>
</Border>
```

## ?? Binding Patterns (Avalonia 11+)

### Compiled Bindings with Null Safety
```xml
<!-- Type-safe compiled bindings -->
<UserControl x:DataType="vm:InventoryTabViewModel"
             x:CompileBindings="True">
    
    <!-- Null-safe binding with fallback -->
    <TextBlock Text="{Binding CurrentUser, FallbackValue='Not Logged In'}"
               ToolTip.Tip="{Binding CurrentUser, FallbackValue='No user logged in'}"/>
    
    <!-- Collection binding with performance optimization -->
    <ListBox ItemsSource="{Binding Items}"
             SelectedItem="{Binding SelectedItem}"
             VirtualizationMode="Recycling"/>
</UserControl>
```

### Command Binding with Parameters
```xml
<!-- Command with parameter -->
<Button Content="Execute Action"
        Command="{Binding ExecuteActionCommand}"
        CommandParameter="{Binding SelectedItem}"/>

<!-- Event binding (Avalonia 11+) -->
<TextBox Text="{Binding SearchText}"
         TextChanged="{Binding OnSearchTextChanged}"/>
```

### Conditional Styling and Templating
```xml
<!-- Conditional classes -->
<Border Classes.success="{Binding IsSuccess}"
        Classes.error="{Binding HasError}"
        Classes.loading="{Binding IsLoading}">
    
    <!-- Data template selector -->
    <ContentControl Content="{Binding CurrentItem}"
                    ContentTemplateSelector="{StaticResource ItemTemplateSelector}"/>
</Border>
```

## ?? Styling and Themes (Avalonia 11+)

### Modern Control Styles
```xml
<!-- Primary Button Style -->
<Style Selector="Button.primary-button">
    <Setter Property="Background" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="Foreground" Value="White"/>
    <Setter Property="Padding" Value="20,12"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="BoxShadow" Value="0 2 8 0 #4B45ED40"/>
    <Setter Property="Transitions">
        <Transitions>
            <TransformTransition Property="Scale" Duration="0:0:0.1"/>
            <BrushTransition Property="Background" Duration="0:0:0.2"/>
        </Transitions>
    </Setter>
</Style>

<Style Selector="Button.primary-button:pointerover">
    <Setter Property="Background" Value="{DynamicResource MagentaAccentBrush}"/>
    <Setter Property="RenderTransform" Value="scale(1.02)"/>
</Style>

<Style Selector="Button.primary-button:pressed">
    <Setter Property="Background" Value="{DynamicResource SecondaryBrush}"/>
    <Setter Property="RenderTransform" Value="scale(0.98)"/>
</Style>

<!-- Modern Form Controls -->
<Style Selector="ComboBox.modern-combobox">
    <Setter Property="Height" Value="44"/>
    <Setter Property="Padding" Value="16,12"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="BorderThickness" Value="2"/>
    <Setter Property="BorderBrush" Value="{DynamicResource BorderBrush}"/>
    <Setter Property="Background" Value="{DynamicResource ControlBackgroundBrush}"/>
</Style>

<Style Selector="ComboBox.modern-combobox:focus">
    <Setter Property="BorderBrush" Value="{DynamicResource AccentBrush}"/>
    <Setter Property="BoxShadow" Value="{StaticResource FocusShadow}"/>
</Style>
```

### Responsive Design Support
```xml
<!-- Responsive layout with Avalonia 11+ -->
<Grid Classes="responsive-grid">
    <Grid.ColumnDefinitions>
        <!-- Desktop: Sidebar + Content -->
        <ColumnDefinition Width="300" MinWidth="280"/>
        <ColumnDefinition Width="*" MinWidth="400"/>
    </Grid.ColumnDefinitions>
    
    <!-- Mobile layout handled by style selectors -->
</Grid>

<!-- Mobile-first responsive styling -->
<Style Selector="Grid.responsive-grid[IsVisible=true]">
    <!-- Mobile layout -->
    <Setter Property="ColumnDefinitions" Value="*"/>
    <Setter Property="RowDefinitions" Value="Auto,*"/>
</Style>

<Style Selector="Grid.responsive-grid[MinWidth=768]">
    <!-- Tablet and desktop layout -->
    <Setter Property="ColumnDefinitions" Value="300,*"/>
    <Setter Property="RowDefinitions" Value="*"/>
</Style>
```

## ?? Accessibility (Avalonia 11+)

### Enhanced Accessibility Support
```xml
<!-- Proper ARIA labeling -->
<TextBox AutomationProperties.Name="Part ID"
         AutomationProperties.HelpText="Enter the unique part identifier"
         AutomationProperties.LabeledBy="{Binding ElementName=PartIdLabel}"/>

<!-- Keyboard navigation -->
<Button TabIndex="1"
        IsDefault="True"
        AutomationProperties.Name="Save inventory item"/>

<!-- Screen reader support -->
<Image AutomationProperties.Name="Inventory status indicator"
       AutomationProperties.Description="{Binding StatusDescription}"/>

<!-- High contrast support -->
<Border Classes="status-indicator"
        Classes.high-contrast="{Binding IsHighContrastMode}"/>
```

## ?? Development Guidelines (Avalonia 11+)

### Adding New Views (.NET 8 + Avalonia 11+)
1. **File-scoped namespaces**: Use `namespace MTM_WIP_Application_Avalonia.Views;`
2. **Compiled bindings**: Always use `x:CompileBindings="True"` with `x:DataType`
3. **Modern controls**: Prefer Avalonia 11+ controls (TabView, NumberBox, InfoBadge)
4. **Responsive design**: Design mobile-first with responsive breakpoints
5. **Accessibility**: Include proper AutomationProperties and keyboard navigation
6. **Performance**: Use virtualization for large collections

### View Conventions (Avalonia 11+)
- **File naming**: Views end with `View` (e.g., `InventoryTabView.axaml`)
- **Namespace**: `MTM_WIP_Application_Avalonia.Views`
- **Bindings**: Use compiled bindings with fallback values
- **Styling**: Use class-based styling with semantic names
- **Resources**: Use DynamicResource for theme-able properties
- **Layout**: Prefer Grid over StackPanel for complex layouts

## ?? Related Documentation

- **ViewModels**: See `ViewModels/README.md` for ReactiveUI patterns with .NET 8
- **GitHub Instructions**: `.github/copilot-instructions.md` for UI generation patterns
- **Avalonia Documentation**: Official Avalonia 11+ documentation for advanced features
- **MTM Design System**: `Documentation/Development/UI_Documentation/` for component specifications

---

*This directory contains the user interface layer using Avalonia 11+ with .NET 8, implementing modern UI patterns with responsive design, enhanced accessibility, and performance optimizations for the MTM WIP Application.*