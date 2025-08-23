# Views Structure Standards - MTM WIP Application Avalonia

## Overview
This document establishes mandatory standards for all Views in the MTM WIP Application Avalonia project to ensure consistency, maintainability, and proper scaling behavior across all screen sizes and resolutions.

## Mandatory View Structure

### 1. **Proportional Scaling Requirement**
**ALL Views MUST implement proportional scaling to maintain consistent appearance across different window sizes.**

#### **Implementation Pattern**
Every view that could be displayed as the main content MUST wrap its content in a Viewbox:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             mc:Ignorable="d"
             d:DesignWidth="800" d:DesignHeight="600"
             x:Class="MTM_WIP_Application_Avalonia.Views.ExampleView"
             x:CompileBindings="True"
             x:DataType="vm:ExampleViewModel">

    <!-- MANDATORY: Viewbox wrapper for proportional scaling -->
    <Viewbox Stretch="Uniform" StretchDirection="Both">
        <!-- Fixed dimensions for scaling reference -->
        <Grid Width="800" Height="600">
            <!-- Your content here -->
        </Grid>
    </Viewbox>
</UserControl>
```

#### **Scaling Rules**
- **Base Dimensions**: Define fixed Width/Height on the inner Grid as reference size
- **Viewbox Properties**: Always use `Stretch="Uniform"` and `StretchDirection="Both"`
- **Content Scaling**: All elements (text, buttons, spacing) scale proportionally
- **Ratio Maintenance**: Original aspect ratio is preserved at all window sizes

### 2. **Required AXAML Structure**

#### **Header Declaration (Mandatory)**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
             xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             mc:Ignorable="d"
             d:DesignWidth="800" d:DesignHeight="600"
             x:Class="MTM_WIP_Application_Avalonia.Views.{ViewName}View"
             x:CompileBindings="True"
             x:DataType="vm:{ViewName}ViewModel">
```

#### **Required Attributes**
- `x:CompileBindings="True"` - Enables compile-time binding validation
- `x:DataType="vm:{ViewName}ViewModel"` - Provides IntelliSense and type safety
- `d:DesignWidth` and `d:DesignHeight` - Design-time preview dimensions
- Proper namespace declarations including `vm` for ViewModels

### 3. **Content Organization Standards**

#### **Main Layout Pattern**
```xml
<Viewbox Stretch="Uniform" StretchDirection="Both">
    <Grid Width="800" Height="600" RowDefinitions="Auto,*,Auto">
        <!-- Header -->
        <Border Grid.Row="0" Background="{DynamicResource HeaderBackgroundBrush}">
            <!-- Header content -->
        </Border>
        
        <!-- Main Content -->
        <ScrollViewer Grid.Row="1" Padding="24">
            <!-- Main content area -->
        </ScrollViewer>
        
        <!-- Footer/Actions -->
        <Border Grid.Row="2" Background="{DynamicResource FooterBackgroundBrush}">
            <!-- Action buttons, status, etc. -->
        </Border>
    </Grid>
</Viewbox>
```

### 4. **Form Layout Standards**

#### **Input Form Pattern**
```xml
<Grid RowDefinitions="Auto,Auto,Auto,Auto" ColumnDefinitions="Auto,*" RowSpacing="8" ColumnSpacing="16">
    <!-- Part Number -->
    <TextBlock Grid.Row="0" Grid.Column="0" 
               Text="Part Number:" 
               Classes="field-label"
               VerticalAlignment="Center"/>
    <ComboBox Grid.Row="0" Grid.Column="1" 
              Classes="input-field"
              ItemsSource="{Binding PartOptions}"
              SelectedItem="{Binding SelectedPart, Mode=TwoWay}"/>
    
    <!-- Operation -->
    <TextBlock Grid.Row="1" Grid.Column="0" 
               Text="Operation:" 
               Classes="field-label"
               VerticalAlignment="Center"/>
    <ComboBox Grid.Row="1" Grid.Column="1" 
              Classes="input-field"
              ItemsSource="{Binding OperationOptions}"
              SelectedItem="{Binding SelectedOperation, Mode=TwoWay}"/>
</Grid>
```

#### **Required Form Elements**
- **Labels**: Use `TextBlock` with consistent styling
- **Spacing**: 16px between label and input, 8px between rows
- **Alignment**: `VerticalAlignment="Center"` for labels
- **Binding**: Two-way binding for input controls

### 5. **MTM Styling Standards**

#### **Color Usage (Required)**
```xml
<!-- MTM Brand Colors - Use Dynamic Resources -->
<Border Background="{DynamicResource PrimaryBrush}"/>        <!-- #4B45ED -->
<Button Classes="primary" Background="{DynamicResource PrimaryBrush}"/>
<Button Classes="accent" Background="{DynamicResource AccentBrush}"/>   <!-- #BA45ED -->
<TextBlock Foreground="{DynamicResource ForegroundBrush}"/>
```

#### **Standard MTM Button Classes**
- `primary` - Main actions (MTM Purple #4B45ED)
- `secondary` - Secondary actions  
- `accent` - Special highlights (Magenta #BA45ED)
- `danger` - Destructive actions
- `success` - Confirmation actions

#### **Spacing Standards**
- **Container margins**: `Margin="8"` for panels, `Margin="24"` for content areas
- **Element spacing**: `Spacing="8"` for related items, `Spacing="16"` for sections
- **Padding**: `Padding="8"` for compact areas, `Padding="16"` for comfortable areas, `Padding="24"` for spacious cards

### 6. **Card and Container Patterns**

#### **Standard Card**
```xml
<Border Classes="card" Padding="24" Margin="0,0,0,16">
    <Grid RowDefinitions="Auto,16,Auto,24,*">
        <!-- Card Header -->
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
            <!-- Content here -->
        </Grid>
    </Grid>
</Border>
```

#### **Required Card Styles**
```xml
<Style Selector="Border.card">
    <Setter Property="Background" Value="{DynamicResource CardBackgroundBrush}"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="BoxShadow" Value="0 2 8 0 #11000000"/>
</Style>
```

### 7. **Data Display Standards**

#### **DataGrid Pattern**
```xml
<DataGrid ItemsSource="{Binding Items}"
          SelectedItem="{Binding SelectedItem, Mode=TwoWay}"
          Classes="modern-grid"
          AutoGenerateColumns="False"
          CanUserReorderColumns="True"
          CanUserResizeColumns="True"
          GridLinesVisibility="Horizontal"
          HeadersVisibility="Column">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID" Binding="{Binding PartId}" Width="120"/>
        <DataGridTextColumn Header="Operation" Binding="{Binding Operation}" Width="80"/>
        <DataGridTextColumn Header="Quantity" Binding="{Binding Quantity}" Width="80"/>
    </DataGrid.Columns>
</DataGrid>
```

#### **List Display Pattern**
```xml
<ItemsControl ItemsSource="{Binding Items}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <Border Classes="card" Padding="16" Margin="0,0,0,8">
                <!-- Item content -->
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### 8. **Navigation and Menu Standards**

#### **Tab Control Pattern**
```xml
<TabControl SelectedIndex="{Binding SelectedTabIndex, Mode=TwoWay}"
            Background="Transparent">
    <TabItem Header="Inventory">
        <Border Padding="16">
            <ContentControl Content="{Binding InventoryContent}"/>
        </Border>
    </TabItem>
    <TabItem Header="Remove">
        <Border Padding="16">
            <ContentControl Content="{Binding RemoveContent}"/>
        </Border>
    </TabItem>
</TabControl>
```

#### **Context Menu Pattern**
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

### 9. **Responsive Design Requirements**

#### **Grid Responsive Patterns**
```xml
<!-- Responsive grid that adapts to content -->
<Grid ColumnDefinitions="Auto,*,Auto" MinWidth="400">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*" MinHeight="200"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
</Grid>
```

#### **Adaptive Content**
- Use `MinWidth` and `MaxWidth` on containers when appropriate
- Implement text trimming: `TextTrimming="CharacterEllipsis"`
- Use `TextWrapping="Wrap"` for dynamic content

### 10. **Error and Loading States**

#### **Loading State Pattern**
```xml
<Grid>
    <!-- Main content -->
    <ContentControl Content="{Binding MainContent}"/>
    
    <!-- Loading overlay -->
    <Border Background="#80000000" 
            IsVisible="{Binding IsLoading}">
        <StackPanel VerticalAlignment="Center" 
                    HorizontalAlignment="Center" 
                    Spacing="16">
            <ProgressBar IsIndeterminate="True" Width="200"/>
            <TextBlock Text="{Binding LoadingMessage}" 
                       Foreground="White" 
                       HorizontalAlignment="Center"/>
        </StackPanel>
    </Border>
</Grid>
```

#### **Error State Pattern**
```xml
<Border Background="#FFEEEE" 
        BorderBrush="Red" 
        BorderThickness="1" 
        CornerRadius="4" 
        Padding="12"
        IsVisible="{Binding HasError}">
    <StackPanel Spacing="8">
        <TextBlock Text="Error" 
                   Foreground="Red" 
                   FontWeight="Bold"/>
        <TextBlock Text="{Binding ErrorMessage}" 
                   Foreground="Red" 
                   TextWrapping="Wrap"/>
        <Button Content="Retry" 
                Command="{Binding RetryCommand}" 
                Classes="secondary"/>
    </StackPanel>
</Border>
```

## View-Specific Requirements

### 11. **Dialog Views**
```xml
<Viewbox Stretch="Uniform" StretchDirection="Both">
    <Grid Width="400" Height="300" RowDefinitions="Auto,*,Auto">
        <!-- Dialog header -->
        <Border Grid.Row="0" Background="{DynamicResource PrimaryBrush}" Padding="16">
            <TextBlock Text="{Binding Title}" 
                       Foreground="White" 
                       FontSize="16" 
                       FontWeight="SemiBold"/>
        </Border>
        
        <!-- Dialog content -->
        <ScrollViewer Grid.Row="1" Padding="24">
            <!-- Content here -->
        </ScrollViewer>
        
        <!-- Dialog buttons -->
        <Border Grid.Row="2" Background="{DynamicResource CardBackgroundBrush}" Padding="16">
            <StackPanel Orientation="Horizontal" 
                        HorizontalAlignment="Right" 
                        Spacing="8">
                <Button Content="Cancel" Classes="secondary" Command="{Binding CancelCommand}"/>
                <Button Content="OK" Classes="primary" Command="{Binding OkCommand}"/>
            </StackPanel>
        </Border>
    </Grid>
</Viewbox>
```

### 12. **Settings Views**
```xml
<Viewbox Stretch="Uniform" StretchDirection="Both">
    <Grid Width="800" Height="600" RowDefinitions="Auto,*">
        <!-- Settings header -->
        <Border Grid.Row="0" Background="{DynamicResource HeaderBackgroundBrush}" Padding="24,16">
            <TextBlock Text="Settings" FontSize="24" FontWeight="Bold"/>
        </Border>
        
        <!-- Settings content -->
        <ScrollViewer Grid.Row="1" Padding="24">
            <StackPanel Spacing="24">
                <!-- Settings sections -->
            </StackPanel>
        </ScrollViewer>
    </Grid>
</Viewbox>
```

### 13. **Dashboard Views**
```xml
<Viewbox Stretch="Uniform" StretchDirection="Both">
    <Grid Width="1200" Height="800" RowDefinitions="Auto,*">
        <!-- Dashboard header -->
        <Border Grid.Row="0" Padding="24">
            <Grid ColumnDefinitions="*,Auto">
                <TextBlock Grid.Column="0" 
                           Text="Dashboard" 
                           FontSize="28" 
                           FontWeight="Bold"/>
                <StackPanel Grid.Column="1" 
                            Orientation="Horizontal" 
                            Spacing="8">
                    <!-- Action buttons -->
                </StackPanel>
            </Grid>
        </Border>
        
        <!-- Dashboard content -->
        <Grid Grid.Row="1" Margin="24" RowDefinitions="Auto,*" ColumnDefinitions="*,*,*">
            <!-- Dashboard widgets -->
        </Grid>
    </Grid>
</Viewbox>
```

## Compliance Checklist

### ✅ **Every View Must Have:**
- [ ] Viewbox wrapper with Uniform stretch
- [ ] Fixed base dimensions on inner Grid
- [ ] Proper AXAML header with all required attributes
- [ ] x:CompileBindings="True" and x:DataType specified
- [ ] Dynamic resource usage for all colors and brushes
- [ ] Consistent spacing using MTM standards
- [ ] Proper ViewModel binding patterns
- [ ] Error handling UI elements where appropriate
- [ ] Loading state support where appropriate
- [ ] Responsive design considerations

### ✅ **Prohibited Patterns:**
- [ ] Hard-coded colors (use dynamic resources only)
- [ ] Missing Viewbox for main content views
- [ ] Inconsistent spacing and margins
- [ ] Missing error handling UI
- [ ] Non-responsive layouts
- [ ] Direct code-behind business logic
- [ ] Missing accessibility considerations

## Implementation Notes

### **For Existing Views**
1. **Audit current views** against this standard
2. **Add Viewbox wrapper** to any view missing proportional scaling
3. **Update color usage** to use dynamic resources
4. **Standardize spacing** according to MTM guidelines
5. **Add error and loading states** where missing

### **For New Views**
1. **Use this document as template** for all new view creation
2. **Copy standard patterns** from this guide
3. **Test scaling behavior** at different window sizes
4. **Validate with design system** before implementation completion

### **Testing Requirements**
1. **Test at multiple window sizes** (800x600, 1200x800, 1920x1080, 4K)
2. **Verify proportional scaling** of all elements
3. **Check color theme compatibility** with light/dark modes
4. **Validate responsive behavior** on different screen ratios
5. **Test keyboard navigation** and accessibility

## Related Documentation
- `Development/UI_Documentation/MTM_Design_System.md` - Color and styling guidelines
- `.github/copilot-instructions.md` - Overall project guidelines  
- `Development/UI_Documentation/Controls/` - Specific control documentation
- `Development/Examples/` - Implementation examples and patterns

---

**Last Updated**: December 2024  
**Applies To**: All Views in MTM WIP Application Avalonia  
**Status**: Mandatory for all new and updated views