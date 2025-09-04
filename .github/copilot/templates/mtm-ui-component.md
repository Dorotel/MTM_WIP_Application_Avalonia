# MTM UI Component Template

## 🎨 UI Component Development Instructions

For creating new UI components in the MTM WIP Application:

### **UserControl Pattern**
```csharp
// Code-behind pattern - minimal code only
public partial class [Component]Control : UserControl
{
    public [Component]Control()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        // Cleanup resources, subscriptions
        base.OnDetachedFromVisualTree(e);
    }
}
```

### **AXAML Structure (AVLN2000 Prevention)**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.[Area]"
             x:Class="MTM_WIP_Application_Avalonia.Controls.[Area].[Component]Control">

    <!-- CRITICAL: Use x:Name, NOT Name on Grid -->
    <Grid x:Name="MainGrid" 
          ColumnDefinitions="Auto,*,Auto" 
          RowDefinitions="Auto,*">
        
        <!-- Header with MTM Purple Theme -->
        <Border Grid.Column="0" Grid.Row="0" Grid.ColumnSpan="3"
                Background="#6a0dad"
                CornerRadius="8,8,0,0"
                Padding="16,8">
            <TextBlock Text="[Component] Title"
                       Foreground="White"
                       FontWeight="Bold"
                       FontSize="16" />
        </Border>

        <!-- Content Area -->
        <Border Grid.Column="0" Grid.Row="1" Grid.ColumnSpan="3"
                Background="White"
                BorderBrush="#E0E0E0"
                BorderThickness="1,0,1,1"
                CornerRadius="0,0,8,8"
                Padding="16">
            
            <!-- Use TextBlock instead of Label -->
            <StackPanel Spacing="8">
                <TextBlock Text="Component Content" />
                
                <!-- Form elements with consistent spacing -->
                <Grid ColumnDefinitions="Auto,*" RowDefinitions="Auto,Auto">
                    <TextBlock Grid.Column="0" Grid.Row="0" 
                               Text="Property:" 
                               VerticalAlignment="Center"
                               Margin="0,0,8,0" />
                    <TextBox Grid.Column="1" Grid.Row="0"
                             Text="{Binding PropertyValue}"
                             MinWidth="200" />
                </Grid>
            </StackPanel>
        </Border>
    </Grid>
</UserControl>
```

### **Data Binding Patterns**
```xml
<!-- Standard binding to ViewModel properties -->
<TextBox Text="{Binding InputValue}" />

<!-- Command binding -->
<Button Content="Execute" 
        Command="{Binding ExecuteCommand}"
        Background="#6a0dad"
        Foreground="White" />

<!-- List binding -->
<ListBox ItemsSource="{Binding Items}"
         SelectedItem="{Binding SelectedItem}" />
```

### **MTM Design System Elements**
```xml
<!-- Primary Button Style -->
<Button Background="#6a0dad"
        Foreground="White"
        Padding="12,8"
        CornerRadius="4"
        FontWeight="SemiBold" />

<!-- Card Layout -->
<Border Background="White"
        BorderBrush="#E0E0E0" 
        BorderThickness="1"
        CornerRadius="8"
        Padding="16"
        Margin="8">
    <!-- Card content -->
</Border>

<!-- Input Group -->
<StackPanel Spacing="8" Margin="0,0,0,16">
    <TextBlock Text="Label" FontWeight="Medium" />
    <TextBox />
</StackPanel>
```

### **Loading States**
```xml
<!-- Loading indicator -->
<Grid IsVisible="{Binding IsLoading}">
    <Border Background="Black" Opacity="0.5" />
    <StackPanel HorizontalAlignment="Center" 
                VerticalAlignment="Center">
        <TextBlock Text="Loading..." 
                   Foreground="White"
                   FontSize="16" />
    </StackPanel>
</Grid>
```

### **Validation Display**
```xml
<!-- Validation error display -->
<Border Background="#FFE5E5" 
        BorderBrush="#FF6B6B"
        BorderThickness="1"
        CornerRadius="4"
        Padding="8"
        IsVisible="{Binding HasError}">
    <TextBlock Text="{Binding ErrorMessage}"
               Foreground="#CC0000" />
</Border>
```