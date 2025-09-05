# MTM Component Library Documentation

## 📋 Overview

This document provides comprehensive documentation for all reusable UI components in the MTM WIP Application, including implementation details, usage guidelines, and code examples for maintaining consistency across the manufacturing workflow interface.

## 🧱 **Core Component Architecture**

### **Base Component Structure**
```csharp
// Base class for all MTM UI components
public abstract class MTMBaseUserControl : UserControl, INotifyPropertyChanged
{
    protected readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    protected MTMBaseUserControl()
    {
        InitializeComponent();
        
        // Get logger through DI if available
        if (Application.Current?.Services != null)
        {
            _logger = Application.Current.Services.GetService<ILogger<MTMBaseUserControl>>();
            _serviceProvider = Application.Current.Services;
        }
        
        // Apply theme
        ApplyMTMTheme();
        
        // Setup accessibility
        SetupAccessibility();
    }

    protected virtual void ApplyMTMTheme()
    {
        // Apply consistent MTM theming
        if (Application.Current.Resources.TryGetResource("MTM_FontFamily", out var fontFamily))
        {
            FontFamily = (FontFamily)fontFamily;
        }
    }

    protected virtual void SetupAccessibility()
    {
        // Ensure keyboard navigation
        Focusable = true;
        IsTabStop = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }
}
```

## 📝 **Input Components**

### **MTMTextBox - Enhanced Text Input**

#### **Implementation**
```xml
<!-- MTMTextBox.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMTextBox">
    
    <UserControl.Styles>
        <Style Selector="TextBox.mtm-textbox">
            <Setter Property="Background" Value="{DynamicResource MTM_Surface}" />
            <Setter Property="Foreground" Value="{DynamicResource MTM_TextPrimary}" />
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
            <Setter Property="BorderThickness" Value="1" />
            <Setter Property="CornerRadius" Value="4" />
            <Setter Property="Padding" Value="8,6" />
            <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
            <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
            <Setter Property="MinHeight" Value="32" />
        </Style>
        
        <Style Selector="TextBox.mtm-textbox:focus">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Primary}" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
        
        <Style Selector="TextBox.mtm-textbox.error">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Danger}" />
            <Setter Property="BorderThickness" Value="2" />
        </Style>
        
        <Style Selector="TextBox.mtm-textbox.success">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Success}" />
        </Style>
    </UserControl.Styles>
    
    <Grid x:Name="RootGrid">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <!-- Label -->
        <TextBlock x:Name="LabelTextBlock"
                   Grid.Row="0"
                   Text="{Binding Label, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   FontWeight="Medium"
                   Margin="0,0,0,4"
                   IsVisible="{Binding ShowLabel, RelativeSource={RelativeSource AncestorType=UserControl}}" />
        
        <!-- Input Field with Icon -->
        <Border Grid.Row="1" 
                BorderBrush="{Binding #MainTextBox.BorderBrush}"
                BorderThickness="{Binding #MainTextBox.BorderThickness}"
                CornerRadius="4"
                Background="{Binding #MainTextBox.Background}">
            <Grid x:Name="InputGrid">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                
                <!-- Leading Icon -->
                <ContentPresenter x:Name="LeadingIconPresenter"
                                Grid.Column="0"
                                Content="{Binding LeadingIcon, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                Margin="8,0,4,0"
                                VerticalAlignment="Center"
                                IsVisible="{Binding LeadingIcon, RelativeSource={RelativeSource AncestorType=UserControl}, Converter={x:Static ObjectConverters.IsNotNull}}" />
                
                <!-- Text Input -->
                <TextBox x:Name="MainTextBox"
                        Grid.Column="1"
                        Classes="mtm-textbox"
                        Text="{Binding Text, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        Watermark="{Binding Placeholder, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        IsEnabled="{Binding IsEnabled, RelativeSource={RelativeSource AncestorType=UserControl}}"
                        BorderThickness="0"
                        Background="Transparent" />
                
                <!-- Trailing Icon/Button -->
                <ContentPresenter x:Name="TrailingIconPresenter"
                                Grid.Column="2"
                                Content="{Binding TrailingIcon, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                Margin="4,0,8,0"
                                VerticalAlignment="Center"
                                IsVisible="{Binding TrailingIcon, RelativeSource={RelativeSource AncestorType=UserControl}, Converter={x:Static ObjectConverters.IsNotNull}}" />
            </Grid>
        </Border>
        
        <!-- Helper/Error Text -->
        <TextBlock x:Name="HelperTextBlock"
                   Grid.Row="2"
                   Text="{Binding HelperText, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   FontSize="{DynamicResource MTM_FontSize_Small}"
                   Foreground="{DynamicResource MTM_TextSecondary}"
                   Margin="0,4,0,0"
                   IsVisible="{Binding ShowHelperText, RelativeSource={RelativeSource AncestorType=UserControl}}" />
        
        <TextBlock x:Name="ErrorTextBlock"
                   Grid.Row="2"
                   Text="{Binding ErrorText, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   FontSize="{DynamicResource MTM_FontSize_Small}"
                   Foreground="{DynamicResource MTM_Danger}"
                   Margin="0,4,0,0"
                   IsVisible="{Binding HasError, RelativeSource={RelativeSource AncestorType=UserControl}}" />
    </Grid>
</UserControl>
```

```csharp
// MTMTextBox.axaml.cs
public partial class MTMTextBox : MTMBaseUserControl
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(Placeholder), string.Empty);

    public static readonly StyledProperty<object?> LeadingIconProperty =
        AvaloniaProperty.Register<MTMTextBox, object?>(nameof(LeadingIcon));

    public static readonly StyledProperty<object?> TrailingIconProperty =
        AvaloniaProperty.Register<MTMTextBox, object?>(nameof(TrailingIcon));

    public static readonly StyledProperty<string> HelperTextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(HelperText), string.Empty);

    public static readonly StyledProperty<string> ErrorTextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(ErrorText), string.Empty);

    public static readonly StyledProperty<bool> HasErrorProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(HasError));

    public static readonly StyledProperty<MTMTextBoxType> TextBoxTypeProperty =
        AvaloniaProperty.Register<MTMTextBox, MTMTextBoxType>(nameof(TextBoxType), MTMTextBoxType.Standard);

    public MTMTextBox()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public object? LeadingIcon
    {
        get => GetValue(LeadingIconProperty);
        set => SetValue(LeadingIconProperty, value);
    }

    public object? TrailingIcon
    {
        get => GetValue(TrailingIconProperty);
        set => SetValue(TrailingIconProperty, value);
    }

    public string HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public string ErrorText
    {
        get => GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }

    public bool HasError
    {
        get => GetValue(HasErrorProperty);
        set => SetValue(HasErrorProperty, value);
    }

    public MTMTextBoxType TextBoxType
    {
        get => GetValue(TextBoxTypeProperty);
        set => SetValue(TextBoxTypeProperty, value);
    }

    public bool ShowLabel => !string.IsNullOrEmpty(Label);
    public bool ShowHelperText => !string.IsNullOrEmpty(HelperText) && !HasError;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Apply specialized styling based on type
        ApplyTextBoxTypeStyles();
    }

    private void ApplyTextBoxTypeStyles()
    {
        var textBox = this.FindControl<TextBox>("MainTextBox");
        if (textBox == null) return;

        switch (TextBoxType)
        {
            case MTMTextBoxType.PartId:
                textBox.Classes.Add("part-id");
                break;
            case MTMTextBoxType.Quantity:
                textBox.Classes.Add("quantity");
                break;
            case MTMTextBoxType.Location:
                textBox.Classes.Add("location");
                break;
        }
    }
}

public enum MTMTextBoxType
{
    Standard,
    PartId,
    Quantity,
    Location,
    Operation
}
```

#### **Usage Examples**
```xml
<!-- Basic Text Input -->
<controls:MTMTextBox Label="Part Description"
                    Text="{Binding PartDescription}"
                    Placeholder="Enter part description..." />

<!-- Part ID Input with Icon -->
<controls:MTMTextBox Label="Part ID"
                    Text="{Binding PartId}"
                    TextBoxType="PartId"
                    Placeholder="PART001">
    <controls:MTMTextBox.LeadingIcon>
        <Path Data="{StaticResource PartIconData}" 
              Fill="{DynamicResource MTM_Primary}"
              Width="16" Height="16" />
    </controls:MTMTextBox.LeadingIcon>
</controls:MTMTextBox>

<!-- Quantity Input with Validation -->
<controls:MTMTextBox Label="Quantity"
                    Text="{Binding Quantity}"
                    TextBoxType="Quantity"
                    HasError="{Binding HasQuantityError}"
                    ErrorText="{Binding QuantityErrorMessage}"
                    HelperText="Enter quantity between 1 and 9999" />
```

### **MTMComboBox - Enhanced Selection Control**

#### **Implementation**
```xml
<!-- MTMComboBox.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMComboBox">
    
    <Grid x:Name="RootGrid">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <!-- Label -->
        <TextBlock x:Name="LabelTextBlock"
                   Grid.Row="0"
                   Text="{Binding Label, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   FontWeight="Medium"
                   Margin="0,0,0,4"
                   IsVisible="{Binding ShowLabel, RelativeSource={RelativeSource AncestorType=UserControl}}" />
        
        <!-- ComboBox -->
        <ComboBox x:Name="MainComboBox"
                  Grid.Row="1"
                  ItemsSource="{Binding Items, RelativeSource={RelativeSource AncestorType=UserControl}}"
                  SelectedItem="{Binding SelectedItem, RelativeSource={RelativeSource AncestorType=UserControl}}"
                  PlaceholderText="{Binding Placeholder, RelativeSource={RelativeSource AncestorType=UserControl}}"
                  IsEnabled="{Binding IsEnabled, RelativeSource={RelativeSource AncestorType=UserControl}}"
                  Classes="mtm-combobox">
            
            <ComboBox.ItemTemplate>
                <DataTemplate>
                    <Grid x:Name="ItemGrid">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width="Auto" />
                            <ColumnDefinition Width="*" />
                            <ColumnDefinition Width="Auto" />
                        </Grid.ColumnDefinitions>
                        
                        <!-- Item Icon -->
                        <ContentPresenter x:Name="ItemIcon"
                                        Grid.Column="0"
                                        Content="{Binding Icon}"
                                        Margin="0,0,8,0"
                                        VerticalAlignment="Center"
                                        IsVisible="{Binding Icon, Converter={x:Static ObjectConverters.IsNotNull}}" />
                        
                        <!-- Item Content -->
                        <StackPanel Grid.Column="1" VerticalAlignment="Center">
                            <TextBlock Text="{Binding DisplayText}"
                                      FontWeight="Medium" />
                            <TextBlock Text="{Binding SubText}"
                                      FontSize="{DynamicResource MTM_FontSize_Small}"
                                      Foreground="{DynamicResource MTM_TextSecondary}"
                                      IsVisible="{Binding SubText, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
                        </StackPanel>
                        
                        <!-- Status Indicator -->
                        <Border x:Name="StatusIndicator"
                               Grid.Column="2"
                               Background="{Binding StatusColor}"
                               Width="8" Height="8"
                               CornerRadius="4"
                               Margin="8,0,0,0"
                               IsVisible="{Binding HasStatus}" />
                    </Grid>
                </DataTemplate>
            </ComboBox.ItemTemplate>
        </ComboBox>
        
        <!-- Helper Text -->
        <TextBlock x:Name="HelperTextBlock"
                   Grid.Row="2"
                   Text="{Binding HelperText, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   FontSize="{DynamicResource MTM_FontSize_Small}"
                   Foreground="{DynamicResource MTM_TextSecondary}"
                   Margin="0,4,0,0"
                   IsVisible="{Binding ShowHelperText, RelativeSource={RelativeSource AncestorType=UserControl}}" />
    </Grid>
</UserControl>
```

```csharp
// MTMComboBox.axaml.cs
public partial class MTMComboBox : MTMBaseUserControl
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<MTMComboBox, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<IEnumerable> ItemsProperty =
        AvaloniaProperty.Register<MTMComboBox, IEnumerable>(nameof(Items));

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<MTMComboBox, object?>(nameof(SelectedItem));

    public static readonly StyledProperty<string> PlaceholderProperty =
        AvaloniaProperty.Register<MTMComboBox, string>(nameof(Placeholder), "Select an option...");

    public MTMComboBox()
    {
        InitializeComponent();
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public IEnumerable Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public bool ShowLabel => !string.IsNullOrEmpty(Label);
}

// ComboBox item model for complex items
public class MTMComboBoxItem
{
    public string DisplayText { get; set; } = string.Empty;
    public string SubText { get; set; } = string.Empty;
    public object? Icon { get; set; }
    public object Value { get; set; } = new();
    public bool HasStatus { get; set; }
    public IBrush? StatusColor { get; set; }
}
```

## 🎛️ **Display Components**

### **MTMStatusBadge - Status Indicator**

#### **Implementation**
```xml
<!-- MTMStatusBadge.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMStatusBadge">
    
    <Border x:Name="BadgeContainer"
            Background="{Binding StatusBackground, RelativeSource={RelativeSource AncestorType=UserControl}}"
            BorderBrush="{Binding StatusBorder, RelativeSource={RelativeSource AncestorType=UserControl}}"
            BorderThickness="1"
            CornerRadius="12"
            Padding="8,4"
            MinWidth="60">
        
        <StackPanel Orientation="Horizontal" 
                   HorizontalAlignment="Center"
                   Spacing="4">
            
            <!-- Status Icon -->
            <ContentPresenter x:Name="IconPresenter"
                            Content="{Binding StatusIcon, RelativeSource={RelativeSource AncestorType=UserControl}}"
                            VerticalAlignment="Center"
                            IsVisible="{Binding StatusIcon, RelativeSource={RelativeSource AncestorType=UserControl}, Converter={x:Static ObjectConverters.IsNotNull}}" />
            
            <!-- Status Text -->
            <TextBlock x:Name="StatusText"
                      Text="{Binding Status, RelativeSource={RelativeSource AncestorType=UserControl}}"
                      Foreground="{Binding StatusForeground, RelativeSource={RelativeSource AncestorType=UserControl}}"
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="SemiBold"
                      VerticalAlignment="Center"
                      TextAlignment="Center" />
        </StackPanel>
    </Border>
</UserControl>
```

```csharp
// MTMStatusBadge.axaml.cs
public partial class MTMStatusBadge : MTMBaseUserControl
{
    public static readonly StyledProperty<string> StatusProperty =
        AvaloniaProperty.Register<MTMStatusBadge, string>(nameof(Status), string.Empty);

    public static readonly StyledProperty<MTMStatusType> StatusTypeProperty =
        AvaloniaProperty.Register<MTMStatusBadge, MTMStatusType>(nameof(StatusType), MTMStatusType.Info);

    public static readonly StyledProperty<object?> StatusIconProperty =
        AvaloniaProperty.Register<MTMStatusBadge, object?>(nameof(StatusIcon));

    public MTMStatusBadge()
    {
        InitializeComponent();
    }

    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public MTMStatusType StatusType
    {
        get => GetValue(StatusTypeProperty);
        set => SetValue(StatusTypeProperty, value);
    }

    public object? StatusIcon
    {
        get => GetValue(StatusIconProperty);
        set => SetValue(StatusIconProperty, value);
    }

    // Computed properties for styling
    public IBrush StatusBackground => StatusType switch
    {
        MTMStatusType.Success => Application.Current.TryFindResource("MTM_Success", out var successBg) ? (IBrush)successBg : Brushes.Green,
        MTMStatusType.Warning => Application.Current.TryFindResource("MTM_Warning", out var warningBg) ? (IBrush)warningBg : Brushes.Orange,
        MTMStatusType.Error => Application.Current.TryFindResource("MTM_Danger", out var errorBg) ? (IBrush)errorBg : Brushes.Red,
        MTMStatusType.Info => Application.Current.TryFindResource("MTM_Primary", out var infoBg) ? (IBrush)infoBg : Brushes.Blue,
        _ => Application.Current.TryFindResource("MTM_Secondary", out var defaultBg) ? (IBrush)defaultBg : Brushes.Gray
    };

    public IBrush StatusForeground => Brushes.White;

    public IBrush StatusBorder => StatusBackground;
}

public enum MTMStatusType
{
    Success,
    Warning,
    Error,
    Info,
    Default
}
```

#### **Usage Examples**
```xml
<!-- Inventory Status -->
<controls:MTMStatusBadge Status="AVAILABLE"
                        StatusType="Success" />

<controls:MTMStatusBadge Status="LOW STOCK"
                        StatusType="Warning" />

<controls:MTMStatusBadge Status="OUT OF STOCK"
                        StatusType="Error" />

<!-- Operation Status with Icon -->
<controls:MTMStatusBadge Status="IN PROGRESS"
                        StatusType="Info">
    <controls:MTMStatusBadge.StatusIcon>
        <Path Data="{StaticResource ClockIconData}"
              Fill="White"
              Width="12" Height="12" />
    </controls:MTMStatusBadge.StatusIcon>
</controls:MTMStatusBadge>
```

### **MTMProgressIndicator - Operation Progress**

#### **Implementation**
```xml
<!-- MTMProgressIndicator.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMProgressIndicator">
    
    <Grid x:Name="RootGrid">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>
        
        <!-- Progress Header -->
        <Grid Grid.Row="0" Margin="0,0,0,8">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>
            
            <TextBlock Grid.Column="0"
                      Text="{Binding Title, RelativeSource={RelativeSource AncestorType=UserControl}}"
                      FontWeight="Medium" />
            
            <TextBlock Grid.Column="1"
                      Text="{Binding PercentageText, RelativeSource={RelativeSource AncestorType=UserControl}}"
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      Foreground="{DynamicResource MTM_TextSecondary}" />
        </Grid>
        
        <!-- Progress Bar -->
        <Border Grid.Row="1"
                Background="{DynamicResource MTM_BackgroundAlt}"
                CornerRadius="4"
                Height="8">
            <Border x:Name="ProgressFill"
                   Background="{Binding ProgressColor, RelativeSource={RelativeSource AncestorType=UserControl}}"
                   CornerRadius="4"
                   HorizontalAlignment="Left"
                   Width="{Binding ProgressWidth, RelativeSource={RelativeSource AncestorType=UserControl}}" />
        </Border>
        
        <!-- Progress Steps (Optional) -->
        <ItemsControl Grid.Row="2"
                     ItemsSource="{Binding Steps, RelativeSource={RelativeSource AncestorType=UserControl}}"
                     Margin="0,8,0,0"
                     IsVisible="{Binding ShowSteps, RelativeSource={RelativeSource AncestorType=UserControl}}">
            <ItemsControl.ItemsPanel>
                <ItemsPanelTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="8" />
                </ItemsPanelTemplate>
            </ItemsControl.ItemsPanel>
            
            <ItemsControl.ItemTemplate>
                <DataTemplate>
                    <Border Background="{Binding StepColor}"
                           Width="24" Height="24"
                           CornerRadius="12">
                        <TextBlock Text="{Binding StepNumber}"
                                  Foreground="White"
                                  FontSize="{DynamicResource MTM_FontSize_Small}"
                                  FontWeight="Bold"
                                  TextAlignment="Center"
                                  VerticalAlignment="Center" />
                    </Border>
                </DataTemplate>
            </ItemsControl.ItemTemplate>
        </ItemsControl>
    </Grid>
</UserControl>
```

## 🏭 **Manufacturing-Specific Components**

### **MTMPartCard - Part Information Display**

#### **Implementation**
```xml
<!-- MTMPartCard.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMPartCard">
    
    <Border Background="{DynamicResource MTM_Surface}"
            BorderBrush="{DynamicResource MTM_Border}"
            BorderThickness="1"
            CornerRadius="8"
            Padding="16"
            MinWidth="280"
            MinHeight="160">
        
        <Border.Effect>
            <DropShadowEffect Color="#000000" Opacity="0.1" ShadowDepth="2" BlurRadius="8" />
        </Border.Effect>
        
        <Grid x:Name="CardContent">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto" />  <!-- Header -->
                <RowDefinition Height="*" />     <!-- Content -->
                <RowDefinition Height="Auto" />  <!-- Footer -->
            </Grid.RowDefinitions>
            
            <!-- Card Header -->
            <Grid Grid.Row="0" Margin="0,0,0,12">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                
                <TextBlock Grid.Column="0"
                          Text="{Binding PartId, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          FontFamily="{DynamicResource MTM_FontFamilyMono}"
                          FontSize="{DynamicResource MTM_FontSize_H4}"
                          FontWeight="Bold"
                          Foreground="{DynamicResource MTM_Primary}" />
                
                <controls:MTMStatusBadge Grid.Column="1"
                                       Status="{Binding Status, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                       StatusType="{Binding StatusType, RelativeSource={RelativeSource AncestorType=UserControl}}" />
            </Grid>
            
            <!-- Card Content -->
            <StackPanel Grid.Row="1" Spacing="8">
                
                <!-- Part Description -->
                <TextBlock Text="{Binding Description, RelativeSource={RelativeSource AncestorType=UserControl}}"
                          FontSize="{DynamicResource MTM_FontSize_Body}"
                          TextWrapping="Wrap"
                          MaxLines="2" />
                
                <!-- Quantity Information -->
                <Grid x:Name="QuantityGrid">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="Auto" />
                    </Grid.ColumnDefinitions>
                    
                    <StackPanel Grid.Column="0">
                        <TextBlock Text="Current Quantity"
                                  FontSize="{DynamicResource MTM_FontSize_Small}"
                                  Foreground="{DynamicResource MTM_TextSecondary}" />
                        <TextBlock Text="{Binding CurrentQuantity, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                  FontSize="{DynamicResource MTM_FontSize_Large}"
                                  FontWeight="Bold" />
                    </StackPanel>
                    
                    <StackPanel Grid.Column="1" HorizontalAlignment="Right">
                        <TextBlock Text="Location"
                                  FontSize="{DynamicResource MTM_FontSize_Small}"
                                  Foreground="{DynamicResource MTM_TextSecondary}"
                                  TextAlignment="Right" />
                        <Border Background="{DynamicResource MTM_Secondary}"
                               CornerRadius="4"
                               Padding="6,3">
                            <TextBlock Text="{Binding Location, RelativeSource={RelativeSource AncestorType=UserControl}}"
                                      Foreground="White"
                                      FontSize="{DynamicResource MTM_FontSize_Small}"
                                      FontWeight="Medium"
                                      TextAlignment="Center" />
                        </Border>
                    </StackPanel>
                </Grid>
            </StackPanel>
            
            <!-- Card Footer -->
            <Grid Grid.Row="2" Margin="0,12,0,0">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*" />
                    <ColumnDefinition Width="Auto" />
                </Grid.ColumnDefinitions>
                
                <TextBlock Grid.Column="0"
                          Text="{Binding LastUpdated, RelativeSource={RelativeSource AncestorType=UserControl}, StringFormat='Last updated: {0:MM/dd/yyyy HH:mm}'}"
                          FontSize="{DynamicResource MTM_FontSize_XSmall}"
                          Foreground="{DynamicResource MTM_TextMuted}"
                          VerticalAlignment="Center" />
                
                <StackPanel Grid.Column="1"
                           Orientation="Horizontal"
                           Spacing="8">
                    <Button Content="Edit"
                           Classes="mtm-button-secondary"
                           Padding="8,4"
                           FontSize="{DynamicResource MTM_FontSize_Small}"
                           Command="{Binding EditCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                           CommandParameter="{Binding PartId, RelativeSource={RelativeSource AncestorType=UserControl}}" />
                    
                    <Button Content="View"
                           Classes="mtm-button-primary" 
                           Padding="8,4"
                           FontSize="{DynamicResource MTM_FontSize_Small}"
                           Command="{Binding ViewCommand, RelativeSource={RelativeSource AncestorType=UserControl}}"
                           CommandParameter="{Binding PartId, RelativeSource={RelativeSource AncestorType=UserControl}}" />
                </StackPanel>
            </Grid>
        </Grid>
    </Border>
</UserControl>
```

```csharp
// MTMPartCard.axaml.cs
public partial class MTMPartCard : MTMBaseUserControl
{
    public static readonly StyledProperty<string> PartIdProperty =
        AvaloniaProperty.Register<MTMPartCard, string>(nameof(PartId), string.Empty);

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<MTMPartCard, string>(nameof(Description), string.Empty);

    public static readonly StyledProperty<int> CurrentQuantityProperty =
        AvaloniaProperty.Register<MTMPartCard, int>(nameof(CurrentQuantity));

    public static readonly StyledProperty<string> LocationProperty =
        AvaloniaProperty.Register<MTMPartCard, string>(nameof(Location), string.Empty);

    public static readonly StyledProperty<string> StatusProperty =
        AvaloniaProperty.Register<MTMPartCard, string>(nameof(Status), string.Empty);

    public static readonly StyledProperty<MTMStatusType> StatusTypeProperty =
        AvaloniaProperty.Register<MTMPartCard, MTMStatusType>(nameof(StatusType), MTMStatusType.Info);

    public static readonly StyledProperty<DateTime> LastUpdatedProperty =
        AvaloniaProperty.Register<MTMPartCard, DateTime>(nameof(LastUpdated), DateTime.Now);

    public static readonly StyledProperty<ICommand?> EditCommandProperty =
        AvaloniaProperty.Register<MTMPartCard, ICommand?>(nameof(EditCommand));

    public static readonly StyledProperty<ICommand?> ViewCommandProperty =
        AvaloniaProperty.Register<MTMPartCard, ICommand?>(nameof(ViewCommand));

    public MTMPartCard()
    {
        InitializeComponent();
    }

    // Properties with getters and setters...
    public string PartId
    {
        get => GetValue(PartIdProperty);
        set => SetValue(PartIdProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    // ... other properties

    public ICommand? EditCommand
    {
        get => GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }

    public ICommand? ViewCommand
    {
        get => GetValue(ViewCommandProperty);
        set => SetValue(ViewCommandProperty, value);
    }
}
```

### **MTMOperationStepper - Process Flow Indicator**

#### **Implementation**
```xml
<!-- MTMOperationStepper.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="MTM_WIP_Application_Avalonia.Controls.MTMOperationStepper">
    
    <ItemsControl ItemsSource="{Binding Steps, RelativeSource={RelativeSource AncestorType=UserControl}}">
        <ItemsControl.ItemsPanel>
            <ItemsPanelTemplate>
                <StackPanel Orientation="Horizontal" />
            </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        
        <ItemsControl.ItemTemplate>
            <DataTemplate>
                <StackPanel Orientation="Horizontal">
                    
                    <!-- Step Circle -->
                    <Grid Width="40" Height="40">
                        <Ellipse Fill="{Binding StepColor}"
                                Stroke="{Binding StepBorderColor}"
                                StrokeThickness="2" />
                        
                        <!-- Step Content (Number or Icon) -->
                        <ContentPresenter Content="{Binding StepContent}"
                                        HorizontalAlignment="Center"
                                        VerticalAlignment="Center" />
                        
                        <!-- Completed Checkmark -->
                        <Path Data="M8,13L2.5,7.5L3.91,6.09L8,10.17L20.09,1.09L21.5,2.5L8,13Z"
                             Fill="White"
                             Width="16" Height="16"
                             IsVisible="{Binding IsCompleted}" />
                    </Grid>
                    
                    <!-- Step Label -->
                    <StackPanel Margin="0,44,0,0" 
                               HorizontalAlignment="Center"
                               Width="80">
                        <TextBlock Text="{Binding StepTitle}"
                                  FontSize="{DynamicResource MTM_FontSize_Small}"
                                  FontWeight="Medium"
                                  TextAlignment="Center"
                                  TextWrapping="Wrap" />
                        <TextBlock Text="{Binding StepSubtitle}"
                                  FontSize="{DynamicResource MTM_FontSize_XSmall}"
                                  Foreground="{DynamicResource MTM_TextSecondary}"
                                  TextAlignment="Center"
                                  TextWrapping="Wrap"
                                  IsVisible="{Binding StepSubtitle, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
                    </StackPanel>
                    
                    <!-- Connection Line -->
                    <Rectangle Width="40" Height="2"
                              Fill="{Binding ConnectorColor}"
                              VerticalAlignment="Center"
                              Margin="-20,0,-20,0"
                              IsVisible="{Binding ShowConnector}" />
                </StackPanel>
            </DataTemplate>
        </ItemsControl.ItemTemplate>
    </ItemsControl>
</UserControl>
```

## 📋 **Usage Guidelines**

### **Component Integration**
```xml
<!-- Example: Complete Manufacturing Form -->
<ScrollViewer>
    <StackPanel Spacing="24" Margin="24">
        
        <!-- Form Header -->
        <TextBlock Text="Inventory Transaction"
                   FontSize="{DynamicResource MTM_FontSize_H2}"
                   FontWeight="SemiBold" />
        
        <!-- Operation Stepper -->
        <controls:MTMOperationStepper Steps="{Binding TransactionSteps}" />
        
        <!-- Form Section -->
        <Border Classes="mtm-card">
            <StackPanel Spacing="16">
                <TextBlock Text="Part Information"
                          FontSize="{DynamicResource MTM_FontSize_H4}"
                          FontWeight="Medium" />
                
                <!-- Part ID Input -->
                <controls:MTMTextBox Label="Part ID"
                                   Text="{Binding PartId}"
                                   TextBoxType="PartId"
                                   HasError="{Binding HasPartIdError}"
                                   ErrorText="{Binding PartIdError}" />
                
                <!-- Location Selection -->
                <controls:MTMComboBox Label="Location"
                                    Items="{Binding AvailableLocations}"
                                    SelectedItem="{Binding SelectedLocation}" />
                
                <!-- Quantity Input -->
                <controls:MTMTextBox Label="Quantity"
                                   Text="{Binding Quantity}"
                                   TextBoxType="Quantity"
                                   HasError="{Binding HasQuantityError}"
                                   ErrorText="{Binding QuantityError}" />
            </StackPanel>
        </Border>
        
        <!-- Current Part Display -->
        <controls:MTMPartCard PartId="{Binding CurrentPart.Id}"
                            Description="{Binding CurrentPart.Description}"
                            CurrentQuantity="{Binding CurrentPart.Quantity}"
                            Location="{Binding CurrentPart.Location}"
                            Status="{Binding CurrentPart.Status}"
                            StatusType="{Binding CurrentPart.StatusType}"
                            LastUpdated="{Binding CurrentPart.LastUpdated}" />
        
        <!-- Action Buttons -->
        <StackPanel Orientation="Horizontal" 
                   HorizontalAlignment="Right"
                   Spacing="12">
            <Button Content="Cancel" Classes="mtm-button-secondary" />
            <Button Content="Save Transaction" Classes="mtm-button-primary" />
        </StackPanel>
    </StackPanel>
</ScrollViewer>
```

### **Accessibility Features**
```csharp
// Accessibility implementation in components
protected override void SetupAccessibility()
{
    base.SetupAccessibility();
    
    // Keyboard navigation
    KeyDown += OnKeyDown;
    
    // Screen reader support
    AutomationProperties.SetName(this, Label);
    AutomationProperties.SetHelpText(this, HelperText);
    
    // Focus management
    GotFocus += OnGotFocus;
    LostFocus += OnLostFocus;
}

private void OnKeyDown(object? sender, KeyEventArgs e)
{
    switch (e.Key)
    {
        case Key.Enter:
            // Handle enter key
            break;
        case Key.Escape:
            // Handle escape key
            break;
        case Key.Tab:
            // Handle tab navigation
            break;
    }
}
```

This comprehensive component library ensures consistent, accessible, and manufacturing-optimized UI components throughout the MTM WIP Application while providing flexibility for different use cases and scenarios.
