# Create Modern Layout Pattern - Custom Prompt

## Instructions
Use this prompt when you need to generate modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns.

## ⚠️ CRITICAL: AVLN2000 Error Prevention
**BEFORE using this prompt, ALWAYS consult [../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md) to prevent AVLN2000 compilation errors.**

## Persona
**UI Architect Copilot + Design System Specialist**  
*(See [personas-instruction.md](../../.github/personas.instruction.md) for role details)*

## Prompt Template

```
Generate a modern Avalonia layout using MTM design patterns and AVLN2000 prevention rules.  
Include sidebar navigation (240px width), card-based content with rounded corners and shadows,  
hero sections with MTM purple gradients, and proper spacing using Grid layouts.  
Apply the MTM color scheme and ensure responsive design principles.
```

## Purpose
For generating modern Avalonia layouts with sidebars, cards, and hero sections using MTM design patterns while preventing AVLN2000 errors.

## Usage Examples

### Example 1: Main Application Window
```
Generate a modern Avalonia layout for the main application window using MTM design patterns and AVLN2000 prevention rules.  
Include sidebar navigation (240px width), card-based content with rounded corners and shadows,  
hero sections with MTM purple gradients, and proper spacing using Grid layouts.  
Apply the MTM color scheme and ensure responsive design principles.
```

### Example 2: Dashboard Layout
```
Generate a modern Avalonia dashboard layout using MTM design patterns and AVLN2000 prevention rules.  
Include sidebar navigation, card-based widgets with rounded corners and shadows,  
hero banner with MTM purple gradients, and responsive Grid layouts.  
Apply the MTM color scheme and ensure proper spacing throughout.
```

## Guidelines

### AVLN2000 Prevention Requirements
1. **Use Avalonia AXAML syntax** - Never WPF XAML
2. **Grid Definitions**: Use `ColumnDefinitions="240,*"` attribute form when possible
3. **No Grid Names**: Never use `Name` property on ColumnDefinition/RowDefinition
4. **Correct Namespaces**: Use `xmlns="https://github.com/avaloniaui"`
5. **Control Equivalents**: Use Avalonia controls (TextBlock instead of Label)

### Modern Application Layout Template (AVLN2000-Safe)
```xml
<!-- CORRECT: Avalonia AXAML with proper namespace -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="MTM_WIP_Application_Avalonia.MainWindow"
        Title="MTM WIP Application"
        Width="1200" Height="700"
        Background="{DynamicResource BackgroundBrush}">
    
    <!-- CORRECT: Use attribute syntax for Grid definitions -->
    <Grid ColumnDefinitions="240,*">
        <!-- Sidebar Navigation -->
        <Border Grid.Column="0" 
                Background="{DynamicResource SidebarBackgroundBrush}"
                BoxShadow="1 0 3 0 #22000000">
            <DockPanel>
                <!-- App Header/Logo -->
                <Border DockPanel.Dock="Top" 
                        Padding="16" 
                        Height="60"
                        Background="{DynamicResource PrimaryBrush}">
                    <TextBlock Text="MTM WIP System" 
                               FontSize="18" 
                               FontWeight="SemiBold"
                               Foreground="White"
                               VerticalAlignment="Center"/>
                </Border>
                
                <!-- Navigation Items -->
                <ScrollViewer>
                    <StackPanel Spacing="2" Margin="8">
                        <!-- Inventory Section -->
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
                                <RadioButton GroupName="Navigation"
                                             Classes="nav-item" 
                                             Content="Transfer Items"
                                             Command="{Binding NavigateCommand}"
                                             CommandParameter="Transfer"/>
                            </StackPanel>
                        </Expander>
                        
                        <!-- Reports Section -->
                        <Expander Header="Reports">
                            <StackPanel Spacing="2" Margin="24,4,0,4">
                                <RadioButton GroupName="Navigation" 
                                             Classes="nav-item"
                                             Content="Transaction History"
                                             Command="{Binding NavigateCommand}"
                                             CommandParameter="History"/>
                                <RadioButton GroupName="Navigation"
                                             Classes="nav-item" 
                                             Content="Inventory Reports"
                                             Command="{Binding NavigateCommand}"
                                             CommandParameter="Reports"/>
                            </StackPanel>
                        </Expander>
                        
                        <!-- Settings Section -->
                        <Expander Header="Settings">
                            <StackPanel Spacing="2" Margin="24,4,0,4">
                                <RadioButton GroupName="Navigation" 
                                             Classes="nav-item"
                                             Content="Database Settings"
                                             Command="{Binding NavigateCommand}"
                                             CommandParameter="DatabaseSettings"/>
                                <RadioButton GroupName="Navigation"
                                             Classes="nav-item" 
                                             Content="User Preferences"
                                             Command="{Binding NavigateCommand}"
                                             CommandParameter="UserSettings"/>
                            </StackPanel>
                        </Expander>
                    </StackPanel>
                </ScrollViewer>
            </DockPanel>
        </Border>
        
        <!-- Main Content Area -->
        <!-- CORRECT: Use attribute syntax for Grid definitions -->
        <Grid Grid.Column="1" 
              Background="{DynamicResource ContentBackgroundBrush}"
              RowDefinitions="Auto,*,Auto">
            
            <!-- Content Header -->
            <Border Grid.Row="0" 
                    Background="{DynamicResource CardBackgroundBrush}"
                    Padding="24,16"
                    BoxShadow="0 1 3 0 #11000000">
                <!-- CORRECT: Use attribute syntax for simple grids -->
                <Grid ColumnDefinitions="*,Auto">
                    <!-- Breadcrumb -->
                    <StackPanel Grid.Column="0" Orientation="Horizontal" Spacing="8">
                        <TextBlock Text="Home" 
                                   FontSize="14" 
                                   Foreground="{DynamicResource AccentBrush}"/>
                        <TextBlock Text=">" FontSize="14"/>
                        <TextBlock Text="{Binding CurrentPageTitle}" 
                                   FontSize="14" 
                                   FontWeight="SemiBold"/>
                    </StackPanel>
                    
                    <!-- User Info -->
                    <StackPanel Grid.Column="1" Orientation="Horizontal" Spacing="12">
                        <TextBlock Text="{Binding CurrentUser}" 
                                   FontSize="14" 
                                   VerticalAlignment="Center"/>
                        <Button Content="Logout" 
                                Classes="secondary"
                                Command="{Binding LogoutCommand}"/>
                    </StackPanel>
                </Grid>
            </Border>
            
            <!-- Main Content -->
            <ScrollViewer Grid.Row="1" Padding="24">
                <ContentControl Content="{Binding CurrentView}"/>
            </ScrollViewer>
            
            <!-- Status Bar -->
            <Border Grid.Row="2" 
                    Background="{DynamicResource StatusBarBackgroundBrush}"
                    Padding="16,8"
                    BorderThickness="0,1,0,0"
                    BorderBrush="{DynamicResource BorderBrush}">
                <!-- CORRECT: Use attribute syntax for simple grids -->
                <Grid ColumnDefinitions="*,Auto,Auto">
                    <!-- Status Message -->
                    <TextBlock Grid.Column="0" 
                               Text="{Binding StatusMessage}"
                               FontSize="12"
                               VerticalAlignment="Center"/>
                    
                    <!-- Connection Status -->
                    <StackPanel Grid.Column="1" 
                                Orientation="Horizontal" 
                                Spacing="8"
                                Margin="0,0,16,0">
                        <Ellipse Width="8" Height="8" 
                                 Fill="{Binding IsConnected, Converter={x:Static converters:BoolToBrushConverter.Instance}}"/>
                        <TextBlock Text="{Binding ConnectionStatus}" 
                                   FontSize="12"
                                   VerticalAlignment="Center"/>
                    </StackPanel>
                    
                    <!-- Progress Indicator -->
                    <ProgressBar Grid.Column="2" 
                                 Width="100" 
                                 Height="4"
                                 IsVisible="{Binding IsLoading}"
                                 IsIndeterminate="True"/>
                </Grid>
            </Border>
        </Grid>
    </Grid>
</Window>
```

### Card-Based Content Layout (AVLN2000-Safe)
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
            <TextBlock Text="Manage your inventory efficiently with modern tools"
                       FontSize="16"
                       Foreground="White"
                       Opacity="0.9"/>
            <Button Content="Get Started" 
                    Classes="hero-button"
                    Margin="0,16,0,0"
                    HorizontalAlignment="Left"/>
        </StackPanel>
    </Grid>
</Border>

<!-- Feature Cards Grid - CORRECT: Attribute syntax -->
<Grid RowDefinitions="Auto,Auto" 
      ColumnDefinitions="*,*,*" 
      ColumnSpacing="24" 
      RowSpacing="24">
    
    <!-- Inventory Card -->
    <Border Grid.Row="0" Grid.Column="0" Classes="feature-card">
        <!-- CORRECT: Attribute syntax for card grid -->
        <Grid RowDefinitions="Auto,16,Auto,24,*">
            <!-- Card Header with Icon -->
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
                       Text="Add, view, and manage inventory items with real-time tracking"
                       Opacity="0.8"
                       TextWrapping="Wrap"/>
            
            <!-- Card Actions -->
            <StackPanel Grid.Row="4" 
                        Orientation="Horizontal" 
                        Spacing="8"
                        VerticalAlignment="Bottom">
                <Button Content="View Inventory" 
                        Classes="primary"
                        Command="{Binding NavigateToInventoryCommand}"/>
                <Button Content="Add Items" 
                        Classes="secondary"
                        Command="{Binding NavigateToAddItemsCommand}"/>
            </StackPanel>
        </Grid>
    </Border>
    
    <!-- Transfer Card -->
    <Border Grid.Row="0" Grid.Column="1" Classes="feature-card">
        <Grid RowDefinitions="Auto,16,Auto,24,*">
            <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
                <PathIcon Grid.Column="0" 
                          Data="{StaticResource TransferIcon}"
                          Width="24" Height="24"
                          Foreground="{DynamicResource BlueAccentBrush}"/>
                <TextBlock Grid.Column="2" 
                           Text="Transfer Operations"
                           FontSize="20"
                           FontWeight="SemiBold"/>
            </Grid>
            
            <TextBlock Grid.Row="2" 
                       Text="Move inventory between locations and operations"
                       Opacity="0.8"
                       TextWrapping="Wrap"/>
            
            <StackPanel Grid.Row="4" 
                        Orientation="Horizontal" 
                        Spacing="8"
                        VerticalAlignment="Bottom">
                <Button Content="Transfer Items" 
                        Classes="primary"
                        Command="{Binding NavigateToTransferCommand}"/>
            </StackPanel>
        </Grid>
    </Border>
    
    <!-- Reports Card -->
    <Border Grid.Row="0" Grid.Column="2" Classes="feature-card">
        <Grid RowDefinitions="Auto,16,Auto,24,*">
            <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
                <PathIcon Grid.Column="0" 
                          Data="{StaticResource ReportsIcon}"
                          Width="24" Height="24"
                          Foreground="{DynamicResource MagentaAccentBrush}"/>
                <TextBlock Grid.Column="2" 
                           Text="Reports & Analytics"
                           FontSize="20"
                           FontWeight="SemiBold"/>
            </Grid>
            
            <TextBlock Grid.Row="2" 
                       Text="View transaction history and generate detailed reports"
                       Opacity="0.8"
                       TextWrapping="Wrap"/>
            
            <StackPanel Grid.Row="4" 
                        Orientation="Horizontal" 
                        Spacing="8"
                        VerticalAlignment="Bottom">
                <Button Content="View Reports" 
                        Classes="primary"
                        Command="{Binding NavigateToReportsCommand}"/>
            </StackPanel>
        </Grid>
    </Border>
</Grid>
```

### Navigation Sidebar Styling
```xml
<!-- Style for navigation items -->
<Style Selector="RadioButton.nav-item">
    <Setter Property="Background" Value="Transparent"/>
    <Setter Property="Padding" Value="12,8"/>
    <Setter Property="CornerRadius" Value="6"/>
    <Setter Property="Margin" Value="0,2"/>
    <Setter Property="FontSize" Value="14"/>
    <Setter Property="Foreground" Value="{DynamicResource ForegroundBrush}"/>
    <Setter Property="HorizontalAlignment" Value="Stretch"/>
    <Setter Property="HorizontalContentAlignment" Value="Left"/>
</Style>

<Style Selector="RadioButton.nav-item:checked">
    <Setter Property="Background" Value="{DynamicResource AccentBrush}"/>
    <Setter Property="Foreground" Value="White"/>
</Style>

<Style Selector="RadioButton.nav-item:pointerover /template/ ContentPresenter">
    <Setter Property="Background" Value="{DynamicResource AccentHoverBrush}"/>
    <Setter Property="TextElement.Foreground" Value="White"/>
</Style>

<!-- Feature card styling -->
<Style Selector="Border.feature-card">
    <Setter Property="Background" Value="{DynamicResource CardBackgroundBrush}"/>
    <Setter Property="CornerRadius" Value="12"/>
    <Setter Property="BoxShadow" Value="0 4 12 0 #11000000"/>
    <Setter Property="Padding" Value="24"/>
    <Setter Property="MinHeight" Value="200"/>
</Style>

<Style Selector="Border.feature-card:pointerover">
    <Setter Property="BoxShadow" Value="0 6 16 0 #16000000"/>
</Style>

<!-- Hero button styling -->
<Style Selector="Button.hero-button">
    <Setter Property="Background" Value="White"/>
    <Setter Property="Foreground" Value="{DynamicResource PrimaryBrush}"/>
    <Setter Property="Padding" Value="20,12"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="FontSize" Value="16"/>
</Style>

<Style Selector="Button.hero-button:pointerover">
    <Setter Property="Background" Value="#F0F0F0"/>
</Style>
```

### Responsive Design Principles
```xml
<!-- Responsive grid that stacks on smaller screens -->
<Grid Name="ResponsiveGrid">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="300"/>
        <ColumnDefinition Width="2*" MinWidth="400"/>
    </Grid.ColumnDefinitions>
    
    <!-- Content adapts based on available space -->
    <StackPanel Grid.Column="0" Spacing="16">
        <!-- Form controls -->
    </StackPanel>
    
    <Grid Grid.Column="1" Margin="16,0,0,0">
        <!-- Data display area -->
    </Grid>
</Grid>

<!-- Adaptive card layout -->
<ItemsControl ItemsSource="{Binding FeatureCards}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Columns="3" MinColumnWidth="300"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border Classes="feature-card" Margin="8">
                <!-- Card content -->
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### Layout Principles
- **Sidebar Width**: Fixed 240px for consistent navigation
- **Card Padding**: 24px for spacious content areas
- **Grid Spacing**: 24px between major sections, 16px between related items
- **Corner Radius**: 8-12px for modern, friendly appearance
- **Shadows**: Subtle depth with `BoxShadow="0 4 12 0 #11000000"`
- **Typography**: 28px for hero headers, 20px for section headers, 14px for body text
- **Color Usage**: MTM purple for primary actions, gradients for hero sections

## Related Files
- **[../UI-Instructions/avalonia-xaml-syntax.instruction.md](../UI-Instructions/avalonia-xaml-syntax.instruction.md)** - **CRITICAL**: AVLN2000 error prevention
- `.github/ui-generation.instruction.md` - UI generation guidelines
- `.github/copilot-instructions.md` - MTM color scheme and design patterns
- `Resources/Themes/` - Theme resource definitions
- `Views/` - Implementation examples

## Quality Checklist
- [ ] **AVLN2000 Prevention**: Avalonia AXAML syntax used (not WPF)
- [ ] **Grid Syntax**: Attribute syntax used for Grid definitions
- [ ] **No Grid Names**: No `Name` properties on Grid definitions
- [ ] **Namespaces**: Correct Avalonia namespace used
- [ ] Sidebar navigation implemented with 240px width
- [ ] Card-based content with rounded corners and shadows
- [ ] Hero sections with MTM purple gradients
- [ ] Proper Grid layouts with appropriate spacing
- [ ] MTM color scheme applied throughout
- [ ] Responsive design principles followed
- [ ] Navigation styles implemented
- [ ] Feature cards with proper hover effects
- [ ] Status bar with connection and progress indicators
- [ ] Accessibility considerations included
