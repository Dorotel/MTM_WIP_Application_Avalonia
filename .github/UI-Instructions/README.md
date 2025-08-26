# UI Instructions

This folder contains user interface generation and design guidelines for the MTM WIP Application Avalonia.

## Files in this Category

### avalonia-xaml-syntax.instruction.md
- **Purpose**: **CRITICAL** - AVLN2000 error prevention guide for Avalonia AXAML syntax
- **Key Topics**: WPF vs Avalonia differences, Grid syntax, namespace corrections
- **Usage**: **MUST READ FIRST** before any UI generation to prevent compilation errors
- **Size**: Comprehensive reference with checklist and examples

### ui-generation.instruction.md
- **Purpose**: Avalonia AXAML generation patterns and guidelines
- **Key Topics**: ReactiveUI ViewModels, AXAML templates, control mapping
- **Usage**: Primary reference for creating new UI components
- **Prerequisites**: Read avalonia-xaml-syntax.instruction.md first

### ui-mapping.instruction.md  
- **Purpose**: WinForms to Avalonia control mapping reference
- **Key Topics**: Control equivalencies, pattern translations
- **Usage**: Reference when converting WinForms patterns to Avalonia
- **Prerequisites**: Follow AVLN2000 prevention guidelines

### ui-styling.instruction.md
- **Purpose**: MTM design system and styling patterns
- **Key Topics**: MTM purple theme, component styling, responsive design
- **Usage**: Reference for consistent visual design application

### Related Development Files
- **Development/UI_Documentation/**: Detailed component-specific instruction files
- **UI_Screenshots/**: Visual references for component layout and styling
- **Views/**: Actual Avalonia view implementations

## ⚠️ CRITICAL: AVLN2000 Error Prevention

**BEFORE creating any AXAML file, ALWAYS consult [avalonia-xaml-syntax.instruction.md](avalonia-xaml-syntax.instruction.md)** to prevent AVLN2000 compilation errors.

Common AVLN2000 causes:
- Using WPF XAML syntax instead of Avalonia AXAML
- Adding `Name` properties to Grid definitions (use `x:Name` only)
- Wrong namespaces (use `xmlns="https://github.com/avaloniaui"`)
- Incorrect Grid syntax patterns

## MTM Design System Integration

These UI instructions integrate with MTM-specific design elements:
- **MTM Color Palette**: #4B45ED, #BA45ED, #8345ED, #4574ED, #ED45E7, #B594ED
- **Purple Branding**: Consistent brand application across components
- **Avalonia 11+ Patterns**: Modern UI patterns with compiled bindings

## Integration Points

- **Core Instructions**: Builds on coding conventions for MVVM patterns
- **Development Instructions**: Database patterns and error handling integration
- **Quality Instructions**: UI compliance and accessibility standards
