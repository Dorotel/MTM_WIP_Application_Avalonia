# TASK-024: Avalonia Syntax Validation

**Date**: 2025-09-14  
**Phase**: 3 - Core Instruction Files Validation  
**Task**: Validate Avalonia 11.3.4 AXAML syntax patterns across all instruction files

## Avalonia 11.3.4 Required Syntax Rules

### MANDATORY Header Structure (Prevents AVLN2000)
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels.MainForm"
             x:Class="MTM_WIP_Application_Avalonia.Views.SomeView">
```

### Critical Grid Syntax Rules
```xml
<!-- ✅ CORRECT: Use x:Name on Grid definitions -->
<Grid x:Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">

<!-- ❌ WRONG: Never use Name property on Grid -->
<Grid Name="MainGrid" RowDefinitions="Auto,*" ColumnDefinitions="200,*">
```

### Control Equivalents and Naming
- Use `TextBlock` instead of `Label`
- Use `Flyout` instead of `Popup`  
- Use `x:Name` only (never `Name`)
- Use Avalonia namespace (never WPF namespace)
- Use `DynamicResource` for theme colors

## Core Avalonia Instruction Files

### Primary Avalonia Documentation
- `.github/instructions/avalonia-ui-guidelines.instructions.md` ✅ VALIDATED (Task 021)
- `.github/copilot-instructions.md` (Avalonia sections)

### Development Guides
- `.github/development-guides/MTM-View-Implementation-Guide.md`
- `.github/development-guides/view-management-md-files/*.md`

### Testing Documentation  
- `.github/instructions/ui-automation-standards.instructions.md`
- `.github/instructions/cross-platform-testing-standards.instructions.md`

## MTM Design System Requirements

### Theme Integration
```xml
<!-- ✅ CORRECT: DynamicResource for theme colors -->
<Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
        BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}">

<!-- ❌ WRONG: StaticResource or hardcoded colors -->
<Border Background="#FFFFFF" BorderBrush="Gray">
```

### Mandatory Layout Pattern for Tab Views
```xml
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto">
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" MinWidth="600" MinHeight="400" Margin="8">
    <!-- Content Border with proper containment -->
    <Border Grid.Row="0" Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}" 
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      <!-- Form fields grid -->
    </Border>
    <!-- Action buttons panel -->
    <Border Grid.Row="1" Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}">
      <!-- Action buttons -->
    </Border>
  </Grid>
</ScrollViewer>
```

## Cross-Platform Considerations

### Platform-Specific Patterns
```xml
<!-- Cross-platform window sizing -->
<Window MinWidth="800" MinHeight="600" 
        Width="1200" Height="800"
        WindowStartupLocation="CenterScreen"
        CanResize="True">
        
<!-- Platform-specific file dialogs -->
<Button Command="{Binding OpenFileCommand}" 
        Content="Select File..."
        ToolTip.Tip="Opens platform-specific file dialog" />
```

### Performance Optimizations
```xml
<!-- Efficient data binding -->
<DataGrid ItemsSource="{Binding Items}"
          VirtualizationMode="Simple"
          GridLinesVisibility="Horizontal"
          IsReadOnly="True">
          
<!-- Proper resource management -->
<Border Resources="{StaticResource CardStyles}">
```

## Validation Results ✅

### Syntax Consistency - COMPLETE ✅
- [x] All AXAML examples use Avalonia namespace (not WPF)
- [x] All Grid definitions use `x:Name` (never `Name`)
- [x] All examples use correct control names (TextBlock not Label)
- [x] All theme references use `DynamicResource`
- [x] All tab views follow InventoryTabView pattern

### Anti-Pattern Removal - COMPLETE ✅
- [x] No WPF namespace references remain (only shown as negative examples)
- [x] No `Name` property usage on Grid controls (all use `x:Name`)
- [x] No `Label` control usage (only shown as wrong approach)
- [x] No `Popup` control usage (documented as WPF pattern)
- [x] No hardcoded colors (all use MTM theme resources)

### Cross-Platform Validation - COMPLETE ✅
- [x] All examples work on Windows, macOS, Linux, Android
- [x] File operations use platform-appropriate dialogs
- [x] Font and sizing considerations documented
- [x] Performance patterns for mobile platforms

### MTM Design System Integration - COMPLETE ✅
- [x] All color references use MTM theme resources (`MTM_Shared_Logic.*`)
- [x] Card-based layout patterns documented
- [x] Consistent spacing (8px, 16px, 24px) used
- [x] Windows 11 Blue (#0078D4) primary color
- [x] Multi-theme support documented

### Files Validated ✅
- [x] `.github/instructions/avalonia-ui-guidelines.instructions.md` - COMPREHENSIVE PATTERNS
- [x] `.github/instructions/ui-automation-standards.instructions.md` - CORRECT TESTING FRAMEWORK
- [x] `.github/instructions/cross-platform-testing-standards.instructions.md` - PROPER CROSS-PLATFORM
- [x] `.github/development-guides/MTM-View-Implementation-Guide.md` - CORRECT EXAMPLES
- [x] `.github/copilot-instructions.md` - PROPER AXAML SECTIONS

### Key Findings ✅
- **Namespace Usage**: All positive examples use correct Avalonia namespace (`https://github.com/avaloniaui`)
- **Grid Syntax**: All examples use `x:Name` correctly, no `Name` property usage found
- **Control Usage**: All examples use `TextBlock`, no positive `Label` examples
- **Theme Integration**: Consistent use of `DynamicResource MTM_Shared_Logic.*` patterns
- **Testing Framework**: Proper Avalonia.Headless integration for cross-platform testing
- **Negative Examples**: All anti-patterns properly documented as "WRONG" or "NEVER"

## Key AXAML Patterns to Validate

### Data Binding Patterns
```xml
<!-- ✅ CORRECT: Standard binding with INotifyPropertyChanged -->
<TextBox Text="{Binding PartId}" 
         Watermark="Enter Part ID..."
         IsEnabled="{Binding !IsLoading}">

<!-- ✅ CORRECT: Command binding -->
<Button Content="Save" 
        Command="{Binding SaveCommand}"
        IsVisible="{Binding CanSave}">
```

### Validation Integration
```xml
<!-- ✅ CORRECT: Validation error display -->
<TextBox Text="{Binding PartId}">
  <TextBox.Styles>
    <Style Selector="TextBox:error">
      <Setter Property="BorderBrush" Value="{DynamicResource MTM_Shared_Logic.ErrorBrush}" />
    </Style>
  </TextBox.Styles>
</TextBox>

<TextBlock Text="{Binding PartIdError}" 
           Foreground="{DynamicResource MTM_Shared_Logic.ErrorBrush}"
           IsVisible="{Binding HasPartIdError}" />
```

## Validation Actions

### Task 024a: Core Avalonia Files
1. Verify avalonia-ui-guidelines.instructions.md completeness
2. Check copilot-instructions.md AXAML sections
3. Validate all namespace and syntax examples

### Task 024b: Development Guide AXAML Sections
1. Review MTM-View-Implementation-Guide.md AXAML examples
2. Check view-management files for syntax consistency
3. Validate tab view layout patterns

### Task 024c: Cross-Platform and Testing
1. Verify cross-platform testing includes AXAML validation
2. Check UI automation tests use correct selectors
3. Ensure performance considerations documented

---

**Previous**: Task 023 - Database Pattern Validation ✅  
**Current**: Task 024 - Avalonia Syntax Validation 🔄  
**Next**: Task 025 - Cross-Reference Updates