# MTM Avalonia Style Guide

## 📋 Overview

This comprehensive style guide defines the visual design system and UI patterns for the MTM WIP Application built with Avalonia UI 11.3.4, ensuring consistency, accessibility, and optimal user experience in manufacturing environments.

## 🎨 **Design System Foundation**

### **Color Palette**

#### **Primary Colors**

```xml
<!-- MTM Brand Colors -->
<Color x:Key="MTM_Primary">#0078D4</Color>           <!-- Windows 11 Blue -->
<Color x:Key="MTM_PrimaryDark">#106EBE</Color>       <!-- Darker blue for hover states -->
<Color x:Key="MTM_PrimaryLight">#40A2E8</Color>      <!-- Lighter blue for backgrounds -->

<!-- Secondary Colors -->
<Color x:Key="MTM_Secondary">#6C757D</Color>         <!-- Neutral gray -->
<Color x:Key="MTM_Accent">#17A2B8</Color>           <!-- Teal accent -->
<Color x:Key="MTM_Success">#28A745</Color>           <!-- Green for success states -->
<Color x:Key="MTM_Warning">#FFC107</Color>           <!-- Yellow for warnings -->
<Color x:Key="MTM_Danger">#DC3545</Color>            <!-- Red for errors/critical -->
```

#### **Neutral Colors**

```xml
<!-- Background Colors -->
<Color x:Key="MTM_Background">#FFFFFF</Color>        <!-- Pure white -->
<Color x:Key="MTM_BackgroundAlt">#F8F9FA</Color>    <!-- Light gray background -->
<Color x:Key="MTM_Surface">#FFFFFF</Color>           <!-- Card/surface background -->
<Color x:Key="MTM_SurfaceAlt">#F1F3F4</Color>       <!-- Alternate surface -->

<!-- Text Colors -->
<Color x:Key="MTM_TextPrimary">#212529</Color>       <!-- Primary text (dark) -->
<Color x:Key="MTM_TextSecondary">#6C757D</Color>     <!-- Secondary text (gray) -->
<Color x:Key="MTM_TextMuted">#ADB5BD</Color>         <!-- Muted text (light gray) -->
<Color x:Key="MTM_TextOnPrimary">#FFFFFF</Color>     <!-- Text on primary backgrounds -->

<!-- Border Colors -->
<Color x:Key="MTM_Border">#DEE2E6</Color>            <!-- Standard borders -->
<Color x:Key="MTM_BorderLight">#E9ECEF</Color>       <!-- Light borders -->
<Color x:Key="MTM_BorderDark">#CED4DA</Color>        <!-- Dark borders -->
```

#### **Theme Colors**

```xml
<!-- MTM Theme Variations -->
<Color x:Key="MTM_Blue">#0078D4</Color>
<Color x:Key="MTM_Green">#107C10</Color>
<Color x:Key="MTM_Red">#D13438</Color>
<Color x:Key="MTM_Amber">#FF8C00</Color>
<Color x:Key="MTM_Purple">#8764B8</Color>
<Color x:Key="MTM_Dark">#2D2D30</Color>
```

### **Typography System**

#### **Font Specifications**

```xml
<!-- Primary Font Family -->
<FontFamily x:Key="MTM_FontFamily">Segoe UI, Arial, sans-serif</FontFamily>
<FontFamily x:Key="MTM_FontFamilyMono">Consolas, Monaco, monospace</FontFamily>

<!-- Font Weights -->
<FontWeight x:Key="MTM_FontWeightLight">Light</FontWeight>      <!-- 300 -->
<FontWeight x:Key="MTM_FontWeightNormal">Normal</FontWeight>    <!-- 400 -->
<FontWeight x:Key="MTM_FontWeightMedium">Medium</FontWeight>    <!-- 500 -->
<FontWeight x:Key="MTM_FontWeightSemiBold">SemiBold</FontWeight><!-- 600 -->
<FontWeight x:Key="MTM_FontWeightBold">Bold</FontWeight>        <!-- 700 -->
```

#### **Font Sizes and Line Heights**

```xml
<!-- Heading Styles -->
<x:Double x:Key="MTM_FontSize_H1">32</x:Double>      <!-- Page titles -->
<x:Double x:Key="MTM_FontSize_H2">28</x:Double>      <!-- Section headers -->
<x:Double x:Key="MTM_FontSize_H3">24</x:Double>      <!-- Subsection headers -->
<x:Double x:Key="MTM_FontSize_H4">20</x:Double>      <!-- Card titles -->
<x:Double x:Key="MTM_FontSize_H5">16</x:Double>      <!-- Form section titles -->
<x:Double x:Key="MTM_FontSize_H6">14</x:Double>      <!-- Minor headings -->

<!-- Body Text Sizes -->
<x:Double x:Key="MTM_FontSize_Large">18</x:Double>   <!-- Large body text -->
<x:Double x:Key="MTM_FontSize_Body">14</x:Double>    <!-- Standard body text -->
<x:Double x:Key="MTM_FontSize_Small">12</x:Double>   <!-- Small text, captions -->
<x:Double x:Key="MTM_FontSize_XSmall">10</x:Double>  <!-- Very small text -->

<!-- Line Heights -->
<x:Double x:Key="MTM_LineHeight_Tight">1.2</x:Double>   <!-- Headings -->
<x:Double x:Key="MTM_LineHeight_Normal">1.5</x:Double>  <!-- Body text -->
<x:Double x:Key="MTM_LineHeight_Loose">1.8</x:Double>   <!-- Long form content -->
```

### **Spacing System**

#### **Standard Spacing Scale**

```xml
<!-- Base spacing unit: 4px -->
<Thickness x:Key="MTM_Spacing_XS">2</Thickness>     <!-- 2px - Very tight -->
<Thickness x:Key="MTM_Spacing_SM">4</Thickness>     <!-- 4px - Tight -->
<Thickness x:Key="MTM_Spacing_MD">8</Thickness>     <!-- 8px - Standard -->
<Thickness x:Key="MTM_Spacing_LG">16</Thickness>    <!-- 16px - Comfortable -->
<Thickness x:Key="MTM_Spacing_XL">24</Thickness>    <!-- 24px - Spacious -->
<Thickness x:Key="MTM_Spacing_XXL">32</Thickness>   <!-- 32px - Very spacious -->

<!-- Component-specific spacing -->
<Thickness x:Key="MTM_Padding_Button">12,8</Thickness>
<Thickness x:Key="MTM_Padding_Card">16</Thickness>
<Thickness x:Key="MTM_Padding_Form">20</Thickness>
<Thickness x:Key="MTM_Padding_Page">24</Thickness>

<!-- Margins -->
<Thickness x:Key="MTM_Margin_Element">8</Thickness>
<Thickness x:Key="MTM_Margin_Section">16</Thickness>
<Thickness x:Key="MTM_Margin_Page">24</Thickness>
```

## 🧱 **Component Styles**

### **Button Styles**

#### **Primary Button**

```xml
<Style x:Key="MTM_Button_Primary" TargetType="Button">
    <Setter Property="Background" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_TextOnPrimary}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="Padding" Value="{DynamicResource MTM_Padding_Button}" />
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
    <Setter Property="FontWeight" Value="{DynamicResource MTM_FontWeightMedium}" />
    <Setter Property="MinHeight" Value="36" />
    <Setter Property="MinWidth" Value="80" />
    <Setter Property="HorizontalContentAlignment" Value="Center" />
    <Setter Property="VerticalContentAlignment" Value="Center" />
    
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="{DynamicResource MTM_PrimaryDark}" />
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_PrimaryDark}" />
        </Trigger>
        <Trigger Property="IsPressed" Value="True">
            <Setter Property="Background" Value="{DynamicResource MTM_PrimaryDark}" />
            <Setter Property="Transform" Value="scale(0.98)" />
        </Trigger>
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Background" Value="{DynamicResource MTM_TextMuted}" />
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_TextMuted}" />
            <Setter Property="Opacity" Value="0.6" />
        </Trigger>
    </Style.Triggers>
</Style>
```

#### **Secondary Button**

```xml
<Style x:Key="MTM_Button_Secondary" TargetType="Button">
    <Setter Property="Background" Value="Transparent" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="Padding" Value="{DynamicResource MTM_Padding_Button}" />
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
    <Setter Property="FontWeight" Value="{DynamicResource MTM_FontWeightMedium}" />
    <Setter Property="MinHeight" Value="36" />
    <Setter Property="MinWidth" Value="80" />
    
    <Style.Triggers>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="{DynamicResource MTM_PrimaryLight}" />
            <Setter Property="Foreground" Value="{DynamicResource MTM_TextOnPrimary}" />
        </Trigger>
    </Style.Triggers>
</Style>
```

### **Input Control Styles**

#### **TextBox Style**

```xml
<Style x:Key="MTM_TextBox_Standard" TargetType="TextBox">
    <Setter Property="Background" Value="{DynamicResource MTM_Surface}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_TextPrimary}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="Padding" Value="8,6" />
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
    <Setter Property="MinHeight" Value="32" />
    <Setter Property="VerticalContentAlignment" Value="Center" />
    
    <Style.Triggers>
        <Trigger Property="IsFocused" Value="True">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_Primary}" />
            <Setter Property="BorderThickness" Value="2" />
        </Trigger>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="BorderBrush" Value="{DynamicResource MTM_PrimaryLight}" />
        </Trigger>
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
            <Setter Property="Foreground" Value="{DynamicResource MTM_TextMuted}" />
        </Trigger>
    </Style.Triggers>
</Style>
```

#### **ComboBox Style**

```xml
<Style x:Key="MTM_ComboBox_Standard" TargetType="ComboBox">
    <Setter Property="Background" Value="{DynamicResource MTM_Surface}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_TextPrimary}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="Padding" Value="8,6" />
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
    <Setter Property="MinHeight" Value="32" />
    <Setter Property="VerticalContentAlignment" Value="Center" />
</Style>
```

### **Card and Panel Styles**

#### **Standard Card**

```xml
<Style x:Key="MTM_Card_Standard" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Surface}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="CornerRadius" Value="8" />
    <Setter Property="Padding" Value="{DynamicResource MTM_Padding_Card}" />
    <Setter Property="Effect">
        <Setter.Value>
            <DropShadowEffect Color="#000000" 
                            Opacity="0.1" 
                            ShadowDepth="2" 
                            BlurRadius="8" />
        </Setter.Value>
    </Setter>
</Style>
```

#### **Header Card**

```xml
<Style x:Key="MTM_Card_Header" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="BorderThickness" Value="0" />
    <Setter Property="CornerRadius" Value="8,8,0,0" />
    <Setter Property="Padding" Value="16,8" />
</Style>
```

### **Data Display Styles**

#### **DataGrid Style**

```xml
<Style x:Key="MTM_DataGrid_Standard" TargetType="DataGrid">
    <Setter Property="Background" Value="{DynamicResource MTM_Surface}" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
    <Setter Property="BorderThickness" Value="1" />
    <Setter Property="RowBackground" Value="{DynamicResource MTM_Surface}" />
    <Setter Property="AlternatingRowBackground" Value="{DynamicResource MTM_BackgroundAlt}" />
    <Setter Property="HeadersVisibility" Value="Column" />
    <Setter Property="GridLinesVisibility" Value="Horizontal" />
    <Setter Property="HorizontalGridLinesBrush" Value="{DynamicResource MTM_BorderLight}" />
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamily}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
</Style>

<!-- DataGrid Header Style -->
<Style x:Key="MTM_DataGridColumnHeader" TargetType="DataGridColumnHeader">
    <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_TextPrimary}" />
    <Setter Property="FontWeight" Value="{DynamicResource MTM_FontWeightSemiBold}" />
    <Setter Property="Padding" Value="8,6" />
    <Setter Property="BorderBrush" Value="{DynamicResource MTM_Border}" />
    <Setter Property="BorderThickness" Value="0,0,1,1" />
</Style>

<!-- DataGrid Row Style -->
<Style x:Key="MTM_DataGridRow" TargetType="DataGridRow">
    <Setter Property="MinHeight" Value="32" />
    <Style.Triggers>
        <Trigger Property="IsSelected" Value="True">
            <Setter Property="Background" Value="{DynamicResource MTM_PrimaryLight}" />
            <Setter Property="Foreground" Value="{DynamicResource MTM_TextOnPrimary}" />
        </Trigger>
        <Trigger Property="IsMouseOver" Value="True">
            <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
        </Trigger>
    </Style.Triggers>
</Style>
```

## 🏭 **Manufacturing-Specific UI Patterns**

### **Inventory Status Indicators**

```xml
<!-- Status Badge Styles -->
<Style x:Key="MTM_Status_Available" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Success}" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Padding" Value="8,4" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Text="AVAILABLE" 
                      Foreground="White" 
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightSemiBold}"
                      TextAlignment="Center" />
        </Setter.Value>
    </Setter>
</Style>

<Style x:Key="MTM_Status_LowStock" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Warning}" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Padding" Value="8,4" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Text="LOW STOCK" 
                      Foreground="White" 
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightSemiBold}"
                      TextAlignment="Center" />
        </Setter.Value>
    </Setter>
</Style>

<Style x:Key="MTM_Status_OutOfStock" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Danger}" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Padding" Value="8,4" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Text="OUT OF STOCK" 
                      Foreground="White" 
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightSemiBold}"
                      TextAlignment="Center" />
        </Setter.Value>
    </Setter>
</Style>
```

### **Operation Progress Indicators**

```xml
<Style x:Key="MTM_ProgressBar_Operation" TargetType="ProgressBar">
    <Setter Property="Height" Value="8" />
    <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="BorderThickness" Value="0" />
    <Setter Property="CornerRadius" Value="4" />
</Style>

<!-- Operation Step Indicator -->
<Style x:Key="MTM_OperationStep_Active" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="Width" Value="24" />
    <Setter Property="Height" Value="24" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Foreground="White" 
                      TextAlignment="Center"
                      VerticalAlignment="Center"
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightBold}" />
        </Setter.Value>
    </Setter>
</Style>

<Style x:Key="MTM_OperationStep_Completed" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Success}" />
    <Setter Property="Width" Value="24" />
    <Setter Property="Height" Value="24" />
    <Setter Property="CornerRadius" Value="12" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Text="✓" 
                      Foreground="White" 
                      TextAlignment="Center"
                      VerticalAlignment="Center"
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightBold}" />
        </Setter.Value>
    </Setter>
</Style>
```

### **Part Information Display**

```xml
<!-- Part ID Display -->
<Style x:Key="MTM_PartId_Display" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{DynamicResource MTM_FontFamilyMono}" />
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Body}" />
    <Setter Property="FontWeight" Value="{DynamicResource MTM_FontWeightBold}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="Background" Value="{DynamicResource MTM_BackgroundAlt}" />
    <Setter Property="Padding" Value="6,3" />
</Style>

<!-- Quantity Display -->
<Style x:Key="MTM_Quantity_Display" TargetType="TextBlock">
    <Setter Property="FontSize" Value="{DynamicResource MTM_FontSize_Large}" />
    <Setter Property="FontWeight" Value="{DynamicResource MTM_FontWeightBold}" />
    <Setter Property="Foreground" Value="{DynamicResource MTM_TextPrimary}" />
    <Setter Property="TextAlignment" Value="Center" />
</Style>

<!-- Location Badge -->
<Style x:Key="MTM_Location_Badge" TargetType="Border">
    <Setter Property="Background" Value="{DynamicResource MTM_Secondary}" />
    <Setter Property="CornerRadius" Value="4" />
    <Setter Property="Padding" Value="6,3" />
    <Setter Property="Child">
        <Setter.Value>
            <TextBlock Foreground="White"
                      FontSize="{DynamicResource MTM_FontSize_Small}"
                      FontWeight="{DynamicResource MTM_FontWeightMedium}" />
        </Setter.Value>
    </Setter>
</Style>
```

## 🎯 **Layout Patterns**

### **Main Application Layout**

```xml
<!-- Standard Application Layout -->
<Grid x:Name="MainApplicationGrid">
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" /> <!-- Header/Navigation -->
        <RowDefinition Height="*" />    <!-- Content Area -->
        <RowDefinition Height="Auto" /> <!-- Status Bar -->
    </Grid.RowDefinitions>
    
    <!-- Header Section -->
    <Border Grid.Row="0" 
            Background="{DynamicResource MTM_Primary}"
            Padding="{DynamicResource MTM_Padding_Page}">
        <!-- Header content -->
    </Border>
    
    <!-- Content Section -->
    <Grid Grid.Row="1" Margin="{DynamicResource MTM_Margin_Page}">
        <!-- Main content area with proper spacing -->
    </Grid>
    
    <!-- Status Bar -->
    <Border Grid.Row="2"
            Background="{DynamicResource MTM_BackgroundAlt}"
            BorderBrush="{DynamicResource MTM_Border}"
            BorderThickness="0,1,0,0"
            Padding="16,8">
        <!-- Status information -->
    </Border>
</Grid>
```

### **Card-Based Layout**

```xml
<!-- Standard Card Layout for Manufacturing Workflows -->
<Border Style="{DynamicResource MTM_Card_Standard}" Margin="8">
    <Grid x:Name="CardContentGrid">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" /> <!-- Header -->
            <RowDefinition Height="*" />    <!-- Content -->
            <RowDefinition Height="Auto" /> <!-- Actions -->
        </Grid.RowDefinitions>
        
        <!-- Card Header -->
        <Border Grid.Row="0" Style="{DynamicResource MTM_Card_Header}">
            <TextBlock Text="Card Title"
                      Foreground="{DynamicResource MTM_TextOnPrimary}"
                      FontSize="{DynamicResource MTM_FontSize_H5}"
                      FontWeight="{DynamicResource MTM_FontWeightSemiBold}" />
        </Border>
        
        <!-- Card Content -->
        <StackPanel Grid.Row="1" 
                   Margin="0,16,0,16"
                   Spacing="12">
            <!-- Card content goes here -->
        </StackPanel>
        
        <!-- Card Actions -->
        <StackPanel Grid.Row="2"
                   Orientation="Horizontal"
                   HorizontalAlignment="Right"
                   Spacing="8">
            <!-- Action buttons -->
        </StackPanel>
    </Grid>
</Border>
```

### **Form Layout**

```xml
<!-- Standard Form Layout -->
<ScrollViewer VerticalScrollBarVisibility="Auto">
    <StackPanel Margin="{DynamicResource MTM_Padding_Form}" Spacing="16">
        
        <!-- Form Section -->
        <Border Style="{DynamicResource MTM_Card_Standard}">
            <StackPanel Spacing="12">
                
                <!-- Section Header -->
                <TextBlock Text="Section Title"
                          FontSize="{DynamicResource MTM_FontSize_H5}"
                          FontWeight="{DynamicResource MTM_FontWeightSemiBold}"
                          Foreground="{DynamicResource MTM_TextPrimary}"
                          Margin="0,0,0,8" />
                
                <!-- Form Fields -->
                <Grid x:Name="FormFieldsGrid">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="120" />  <!-- Label column -->
                        <ColumnDefinition Width="*" />    <!-- Input column -->
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition Height="Auto" />
                        <RowDefinition Height="Auto" />
                    </Grid.RowDefinitions>
                    
                    <!-- Field 1 -->
                    <TextBlock Grid.Row="0" Grid.Column="0" 
                              Text="Field Label:"
                              VerticalAlignment="Center"
                              FontWeight="{DynamicResource MTM_FontWeightMedium}" />
                    <TextBox Grid.Row="0" Grid.Column="1"
                            Style="{DynamicResource MTM_TextBox_Standard}"
                            Margin="8,0,0,0" />
                    
                    <!-- Field 2 -->
                    <TextBlock Grid.Row="1" Grid.Column="0"
                              Text="Another Field:"
                              VerticalAlignment="Center"
                              FontWeight="{DynamicResource MTM_FontWeightMedium}"
                              Margin="0,8,0,0" />
                    <ComboBox Grid.Row="1" Grid.Column="1"
                             Style="{DynamicResource MTM_ComboBox_Standard}"
                             Margin="8,8,0,0" />
                </Grid>
            </StackPanel>
        </Border>
        
        <!-- Form Actions -->
        <StackPanel Orientation="Horizontal" 
                   HorizontalAlignment="Right"
                   Spacing="12">
            <Button Content="Cancel"
                   Style="{DynamicResource MTM_Button_Secondary}"
                   MinWidth="100" />
            <Button Content="Save"
                   Style="{DynamicResource MTM_Button_Primary}"
                   MinWidth="100" />
        </StackPanel>
    </StackPanel>
</ScrollViewer>
```

## 🔧 **Animation and Transitions**

### **Standard Animations**

```xml
<!-- Fade In Animation -->
<Storyboard x:Key="MTM_FadeIn">
    <DoubleAnimation Storyboard.TargetProperty="Opacity"
                    From="0" To="1" 
                    Duration="0:0:0.3" />
</Storyboard>

<!-- Slide In Animation -->
<Storyboard x:Key="MTM_SlideIn">
    <DoubleAnimation Storyboard.TargetProperty="(TranslateTransform.X)"
                    From="50" To="0"
                    Duration="0:0:0.3" />
    <DoubleAnimation Storyboard.TargetProperty="Opacity"
                    From="0" To="1"
                    Duration="0:0:0.3" />
</Storyboard>

<!-- Button Press Animation -->
<Storyboard x:Key="MTM_ButtonPress">
    <DoubleAnimation Storyboard.TargetProperty="(ScaleTransform.ScaleX)"
                    From="1" To="0.98" Duration="0:0:0.1"
                    AutoReverse="True" />
    <DoubleAnimation Storyboard.TargetProperty="(ScaleTransform.ScaleY)"
                    From="1" To="0.98" Duration="0:0:0.1"
                    AutoReverse="True" />
</Storyboard>
```

## 📱 **Responsive Design Guidelines**

### **Screen Size Breakpoints**

```xml
<!-- Responsive breakpoints for manufacturing workstations -->
<x:Double x:Key="MTM_Breakpoint_Small">1024</x:Double>   <!-- Small workstations -->
<x:Double x:Key="MTM_Breakpoint_Medium">1366</x:Double>  <!-- Standard workstations -->
<x:Double x:Key="MTM_Breakpoint_Large">1920</x:Double>   <!-- Large displays -->
<x:Double x:Key="MTM_Breakpoint_XLarge">2560</x:Double>  <!-- Ultra-wide displays -->
```

### **Adaptive Layouts**

```xml
<!-- Responsive Grid Columns -->
<Style x:Key="MTM_ResponsiveGrid" TargetType="Grid">
    <Style.Triggers>
        <!-- Small screens: Single column -->
        <DataTrigger Binding="{Binding ActualWidth, RelativeSource={RelativeSource Self}}"
                    Value="{StaticResource MTM_Breakpoint_Small}">
            <Setter Property="ColumnDefinitions">
                <Setter.Value>
                    <ColumnDefinitionCollection>
                        <ColumnDefinition Width="*" />
                    </ColumnDefinitionCollection>
                </Setter.Value>
            </Setter>
        </DataTrigger>
        
        <!-- Medium screens: Two columns -->
        <DataTrigger Binding="{Binding ActualWidth, RelativeSource={RelativeSource Self}}"
                    Value="{StaticResource MTM_Breakpoint_Medium}">
            <Setter Property="ColumnDefinitions">
                <Setter.Value>
                    <ColumnDefinitionCollection>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </ColumnDefinitionCollection>
                </Setter.Value>
            </Setter>
        </DataTrigger>
        
        <!-- Large screens: Three columns -->
        <DataTrigger Binding="{Binding ActualWidth, RelativeSource={RelativeSource Self}}"
                    Value="{StaticResource MTM_Breakpoint_Large}">
            <Setter Property="ColumnDefinitions">
                <Setter.Value>
                    <ColumnDefinitionCollection>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="*" />
                    </ColumnDefinitionCollection>
                </Setter.Value>
            </Setter>
        </DataTrigger>
    </Style.Triggers>
</Style>
```

## ♿ **Accessibility Standards**

### **Keyboard Navigation**

```xml
<!-- Focus Visual Styles -->
<Style x:Key="MTM_FocusVisual" TargetType="Control">
    <Setter Property="Template">
        <Setter.Value>
            <ControlTemplate>
                <Rectangle Stroke="{DynamicResource MTM_Primary}"
                          StrokeThickness="2"
                          StrokeDashArray="1 2"
                          SnapsToDevicePixels="True" />
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>
```

### **High Contrast Support**

```xml
<!-- High Contrast Theme Overrides -->
<Style x:Key="MTM_HighContrast_Button" TargetType="Button">
    <Style.Triggers>
        <Trigger Property="SystemParameters.HighContrast" Value="True">
            <Setter Property="Background" Value="{x:Static SystemColors.ControlBrush}" />
            <Setter Property="Foreground" Value="{x:Static SystemColors.ControlTextBrush}" />
            <Setter Property="BorderBrush" Value="{x:Static SystemColors.ControlTextBrush}" />
            <Setter Property="BorderThickness" Value="2" />
        </Trigger>
    </Style.Triggers>
</Style>
```

## 🎨 **Icon System**

### **Standard Icons**

```xml
<!-- Material Design Icons for Manufacturing -->
<Style x:Key="MTM_Icon_Inventory" TargetType="Path">
    <Setter Property="Data" Value="M12,2A10,10 0 0,1 22,12A10,10 0 0,1 12,22A10,10 0 0,1 2,12A10,10 0 0,1 12,2M12,4A8,8 0 0,0 4,12A8,8 0 0,0 12,20A8,8 0 0,0 20,12A8,8 0 0,0 12,4M12,6A6,6 0 0,1 18,12A6,6 0 0,1 12,18A6,6 0 0,1 6,12A6,6 0 0,1 12,6M12,8A4,4 0 0,0 8,12A4,4 0 0,0 12,16A4,4 0 0,0 16,12A4,4 0 0,0 12,8Z" />
    <Setter Property="Fill" Value="{DynamicResource MTM_Primary}" />
    <Setter Property="Width" Value="16" />
    <Setter Property="Height" Value="16" />
</Style>

<Style x:Key="MTM_Icon_Add" TargetType="Path">
    <Setter Property="Data" Value="M19,13H13V19H11V13H5V11H11V5H13V11H19V13Z" />
    <Setter Property="Fill" Value="{DynamicResource MTM_Success}" />
    <Setter Property="Width" Value="16" />
    <Setter Property="Height" Value="16" />
</Style>

<Style x:Key="MTM_Icon_Remove" TargetType="Path">
    <Setter Property="Data" Value="M19,13H5V11H19V13Z" />
    <Setter Property="Fill" Value="{DynamicResource MTM_Danger}" />
    <Setter Property="Width" Value="16" />
    <Setter Property="Height" Value="16" />
</Style>
```

This comprehensive Avalonia style guide ensures consistent, accessible, and user-friendly interfaces throughout the MTM WIP Application while maintaining focus on manufacturing workflow efficiency.
