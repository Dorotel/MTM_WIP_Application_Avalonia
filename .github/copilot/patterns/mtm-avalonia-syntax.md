# MTM Avalonia AXAML Syntax Patterns

## 🎨 Avalonia AXAML Syntax Rules (AVLN2000 Prevention)

### **Critical AXAML Rules**
**These rules are MANDATORY to prevent AVLN2000 compilation errors:**

1. **NEVER use `Name` property on Grid definitions** - Always use `x:Name`
2. **Use correct Avalonia namespace**: `xmlns="https://github.com/avaloniaui"` 
3. **Use Avalonia controls**: `TextBlock` instead of `Label`, `Flyout` instead of `Popup`
4. **Grid syntax**: Use attribute form `ColumnDefinitions="Auto,*"` when possible
5. **Standard bindings**: `{Binding PropertyName}` with INotifyPropertyChanged

### **Required AXAML Header Structure**

#### **UserControl Header Pattern**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.MainForm.ExampleView">
```

#### **Window Header Pattern**
```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
        x:Class="MTM_WIP_Application_Avalonia.Views.ExampleWindow"
        Title="MTM WIP Application"
        WindowStartupLocation="CenterScreen">
```

### **Grid Layout Patterns**

#### **✅ Correct Grid Usage**
```xml
<!-- Use x:Name, NOT Name -->
<Grid x:Name="MainGrid" 
      ColumnDefinitions="Auto,*,Auto" 
      RowDefinitions="Auto,*,Auto">
    
    <!-- Content here -->
</Grid>

<!-- Alternative explicit syntax -->
<Grid x:Name="MainGrid">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="100" />
    </Grid.ColumnDefinitions>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="50" />
    </Grid.RowDefinitions>
</Grid>
```

#### **❌ WRONG Grid Usage (Causes AVLN2000)**
```xml
<!-- NEVER DO THIS - Will cause compilation errors -->
<Grid Name="MainGrid" ColumnDefinitions="Auto,*">
    <!-- This will fail -->
</Grid>
```

### **Control Equivalents**

#### **Text Display**
```xml
<!-- ✅ CORRECT - Use TextBlock -->
<TextBlock Text="Display Text" 
           FontWeight="Bold"
           Foreground="#6a0dad" />

<!-- ❌ WRONG - Don't use Label in Avalonia -->
<Label Content="Display Text" />
```

#### **Popup/Flyout**
```xml
<!-- ✅ CORRECT - Use Flyout -->
<Button Content="Options">
    <Button.Flyout>
        <Flyout>
            <StackPanel>
                <Button Content="Option 1" />
                <Button Content="Option 2" />
            </StackPanel>
        </Flyout>
    </Button.Flyout>
</Button>

<!-- ❌ WRONG - Popup doesn't exist in Avalonia -->
<Popup>
    <!-- This control doesn't exist -->
</Popup>
```

### **MTM Design System Implementation**

#### **Purple Theme Colors**
```xml
<!-- Primary MTM purple -->
<Button Background="#6a0dad" 
        Foreground="White"
        Content="MTM Button" />

<!-- Lighter purple variants -->
<Border Background="#8A2BE2" /> <!-- BlueViolet -->
<Border Background="#9370DB" /> <!-- MediumPurple -->
<Border Background="#DDA0DD" /> <!-- Plum -->
```

#### **Card Layout Pattern**
```xml
<Border Background="White"
        BorderBrush="#E0E0E0"
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="8">
    
    <Grid x:Name="CardContent" RowDefinitions="Auto,*">
        <!-- Card Header -->
        <Border Grid.Row="0" 
                Background="#6a0dad"
                CornerRadius="8,8,0,0"
                Padding="16,8">
            <TextBlock Text="Card Title"
                       Foreground="White"
                       FontWeight="Bold"
                       FontSize="16" />
        </Border>
        
        <!-- Card Content -->
        <StackPanel Grid.Row="1" 
                    Margin="16"
                    Spacing="8">
            <!-- Card content here -->
        </StackPanel>
    </Grid>
</Border>
```

#### **Form Layout Pattern**
```xml
<Grid x:Name="FormGrid" ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto,Auto">
    
    <!-- Form Field Pattern -->
    <TextBlock Grid.Column="0" Grid.Row="0"
               Text="Part ID:"
               VerticalAlignment="Center"
               Margin="0,0,8,8" />
    <TextBox Grid.Column="1" Grid.Row="0"
             Text="{Binding PartId}"
             Margin="0,0,0,8" />
    
    <TextBlock Grid.Column="0" Grid.Row="1"
               Text="Location:"
               VerticalAlignment="Center"
               Margin="0,0,8,8" />
    <ComboBox Grid.Column="1" Grid.Row="1"
              ItemsSource="{Binding Locations}"
              SelectedItem="{Binding SelectedLocation}"
              Margin="0,0,0,8" />
    
    <!-- Button Row -->
    <StackPanel Grid.Column="0" Grid.Row="2" Grid.ColumnSpan="2"
                Orientation="Horizontal"
                HorizontalAlignment="Right"
                Spacing="8">
        <Button Content="Save"
                Command="{Binding SaveCommand}"
                Background="#6a0dad"
                Foreground="White"
                Padding="16,8" />
        <Button Content="Cancel"
                Command="{Binding CancelCommand}"
                Background="Gray"
                Foreground="White"
                Padding="16,8" />
    </StackPanel>
</Grid>
```

### **Data Binding Patterns**

#### **Property Binding**
```xml
<!-- Simple property binding -->
<TextBox Text="{Binding InputValue}" />

<!-- Two-way binding with update trigger -->
<TextBox Text="{Binding InputValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />

<!-- Binding with converter -->
<TextBox Text="{Binding Quantity, Converter={StaticResource IntToStringConverter}}" />
```

#### **Command Binding**
```xml
<!-- Simple command -->
<Button Content="Execute" Command="{Binding ExecuteCommand}" />

<!-- Command with parameter -->
<Button Content="Delete" 
        Command="{Binding DeleteCommand}" 
        CommandParameter="{Binding SelectedItem}" />

<!-- Command binding in list items -->
<ListBox ItemsSource="{Binding Items}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <TextBlock Text="{Binding Name}" />
                <Button Content="Edit" 
                        Command="{Binding DataContext.EditCommand, RelativeSource={RelativeSource AncestorType=ListBox}}"
                        CommandParameter="{Binding}" />
            </StackPanel>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### **Collection Binding**
```xml
<!-- Simple list -->
<ListBox ItemsSource="{Binding Items}"
         SelectedItem="{Binding SelectedItem}" />

<!-- ComboBox binding -->
<ComboBox ItemsSource="{Binding Options}"
          SelectedValue="{Binding SelectedOptionId}"
          DisplayMemberPath="Name"
          SelectedValuePath="Id" />

<!-- DataGrid equivalent (DataGrid) -->
<DataGrid ItemsSource="{Binding InventoryItems}"
          SelectedItem="{Binding SelectedInventoryItem}"
          IsReadOnly="True">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Part ID" Binding="{Binding PartId}" />
        <DataGridTextColumn Header="Location" Binding="{Binding Location}" />
        <DataGridTextColumn Header="Quantity" Binding="{Binding Quantity}" />
    </DataGrid.Columns>
</DataGrid>
```

### **Styling and Theming**

#### **Inline Styles**
```xml
<Button Content="Styled Button">
    <Button.Styles>
        <Style Selector="Button:pointerover">
            <Setter Property="Background" Value="#8A2BE2" />
        </Style>
        <Style Selector="Button:pressed">
            <Setter Property="Background" Value="#4B0082" />
        </Style>
    </Button.Styles>
</Button>
```

#### **Resource Usage**
```xml
<UserControl.Resources>
    <SolidColorBrush x:Key="MTMPurple" Color="#6a0dad" />
    <Thickness x:Key="StandardMargin">8</Thickness>
</UserControl.Resources>

<Border Background="{StaticResource MTMPurple}"
        Margin="{StaticResource StandardMargin}" />
```

### **Layout Container Patterns**

#### **StackPanel Usage**
```xml
<!-- Vertical stack (default) -->
<StackPanel Spacing="8">
    <TextBlock Text="Item 1" />
    <TextBlock Text="Item 2" />
    <TextBlock Text="Item 3" />
</StackPanel>

<!-- Horizontal stack -->
<StackPanel Orientation="Horizontal" Spacing="16">
    <Button Content="OK" />
    <Button Content="Cancel" />
</StackPanel>
```

#### **DockPanel Usage**
```xml
<DockPanel LastChildFill="True">
    <Border DockPanel.Dock="Top" Background="#6a0dad" Height="60">
        <TextBlock Text="Header" Foreground="White" />
    </Border>
    <Border DockPanel.Dock="Bottom" Background="Gray" Height="30">
        <TextBlock Text="Status Bar" />
    </Border>
    <Grid x:Name="MainContent" Background="White">
        <!-- Main content fills remaining space -->
    </Grid>
</DockPanel>
```

#### **WrapPanel Usage**
```xml
<WrapPanel Orientation="Horizontal">
    <Button Content="Button 1" Width="100" Margin="4" />
    <Button Content="Button 2" Width="100" Margin="4" />
    <Button Content="Button 3" Width="100" Margin="4" />
    <!-- Buttons wrap to next line as needed -->
</WrapPanel>
```

### **Advanced Layout Patterns**

#### **Master-Detail Layout**
```xml
<Grid x:Name="MasterDetailGrid" ColumnDefinitions="300,*">
    
    <!-- Master List -->
    <Border Grid.Column="0" 
            BorderBrush="#E0E0E0" 
            BorderThickness="0,0,1,0">
        <ListBox ItemsSource="{Binding MasterItems}"
                 SelectedItem="{Binding SelectedMasterItem}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Name}" Margin="8" />
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Border>
    
    <!-- Detail View -->
    <ContentControl Grid.Column="1" 
                    Content="{Binding SelectedMasterItem}"
                    Margin="16">
        <ContentControl.ContentTemplate>
            <DataTemplate>
                <StackPanel Spacing="8">
                    <TextBlock Text="{Binding Name}" FontWeight="Bold" FontSize="18" />
                    <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
                    <!-- Detail content -->
                </StackPanel>
            </DataTemplate>
        </ContentControl.ContentTemplate>
    </ContentControl>
</Grid>
```

#### **Responsive Layout**
```xml
<Grid x:Name="ResponsiveGrid">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*" MinWidth="300" />
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" MinWidth="200" />
    </Grid.ColumnDefinitions>
    
    <!-- Left Panel -->
    <Border Grid.Column="0" Background="LightGray" Margin="4" />
    
    <!-- Splitter -->
    <GridSplitter Grid.Column="1" 
                  Width="4" 
                  Background="DarkGray"
                  HorizontalAlignment="Center" />
    
    <!-- Right Panel -->
    <Border Grid.Column="2" Background="LightBlue" Margin="4" />
</Grid>
```

### **Validation and Error Display**

#### **Validation Error Template**
```xml
<TextBox Text="{Binding ValidatedProperty}">
    <TextBox.Styles>
        <Style Selector="TextBox:error">
            <Setter Property="BorderBrush" Value="Red" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
    </TextBox.Styles>
</TextBox>

<!-- Error display -->
<TextBlock Text="{Binding ValidationErrors}" 
           Foreground="Red"
           IsVisible="{Binding HasValidationErrors}"
           TextWrapping="Wrap" />
```

#### **Loading State**
```xml
<Grid x:Name="ContentWithLoading">
    <!-- Main content -->
    <ContentControl Content="{Binding MainContent}"
                    IsVisible="{Binding !IsLoading}" />
    
    <!-- Loading overlay -->
    <Border Background="Black" 
            Opacity="0.5"
            IsVisible="{Binding IsLoading}">
        <StackPanel HorizontalAlignment="Center" 
                    VerticalAlignment="Center">
            <TextBlock Text="Loading..." 
                       Foreground="White"
                       FontSize="16"
                       HorizontalAlignment="Center" />
        </StackPanel>
    </Border>
</Grid>
```

### **Common Avalonia-Specific Features**

#### **Attached Properties**
```xml
<!-- ToolTip -->
<Button Content="Hover Me" ToolTip.Tip="This is a tooltip" />

<!-- Grid positioning -->
<TextBlock Grid.Row="1" Grid.Column="2" Text="Positioned" />

<!-- DockPanel docking -->
<Button DockPanel.Dock="Left" Content="Docked Left" />
```

#### **Template Binding**
```xml
<Button>
    <Button.Template>
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}"
                    CornerRadius="4">
                <ContentPresenter Content="{TemplateBinding Content}"
                                HorizontalAlignment="Center"
                                VerticalAlignment="Center" />
            </Border>
        </ControlTemplate>
    </Button.Template>
</Button>
```

### **Performance Optimization Patterns**

#### **Virtualization**
```xml
<!-- For large lists, use virtualized containers -->
<ListBox ItemsSource="{Binding LargeDataSet}"
         VirtualizationMode="Simple">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding DisplayText}" />
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### **Resource Optimization**
```xml
<!-- Use StaticResource for static resources -->
<Button Background="{StaticResource MTMPurpleBrush}" />

<!-- Use DynamicResource for theme-changing resources -->
<Button Background="{DynamicResource ThemePrimaryBrush}" />
```