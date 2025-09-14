# Technology Versions Extracted from MTM_WIP_Application_Avalonia.csproj

**Analysis Date**: 2025-09-14  
**Source**: MTM_WIP_Application_Avalonia.csproj  
**Purpose**: Authoritative reference for all documentation version updates

## Extracted Versions

### Core Framework
- **.NET Framework**: `net8.0` (Target Framework)
- **C# Language Version**: C# 12 (implicit with .NET 8)
- **Nullable Reference Types**: Enabled

### Avalonia UI Framework
- **Avalonia Core**: `11.3.4`
- **Avalonia Controls DataGrid**: `11.3.4`
- **Avalonia Desktop**: `11.3.4`
- **Avalonia Themes Fluent**: `11.3.4`
- **Avalonia Fonts Inter**: `11.3.4`
- **Avalonia Diagnostics**: `11.3.4`
- **Avalonia Xaml Interactivity**: `11.3.0.6`

### MVVM Framework
- **MVVM Community Toolkit**: `8.3.2`

### Database
- **MySql.Data**: `9.4.0`
- **Dapper**: `2.1.66`

### Microsoft Extensions
- **Microsoft.Extensions.Hosting**: `9.0.8`
- **Microsoft.Extensions.Logging.Debug**: `9.0.8`
- **Microsoft.Extensions.Caching.Memory**: `9.0.8`
- **Microsoft.Extensions.Configuration**: `9.0.8`
- **Microsoft.Extensions.Configuration.Binder**: `9.0.8`
- **Microsoft.Extensions.Configuration.Json**: `9.0.8`
- **Microsoft.Extensions.Configuration.EnvironmentVariables**: `9.0.8`
- **Microsoft.Extensions.DependencyInjection**: `9.0.8`
- **Microsoft.Extensions.Hosting.Abstractions**: `9.0.8`
- **Microsoft.Extensions.Logging**: `9.0.8`
- **Microsoft.Extensions.Logging.Console**: `9.0.8`
- **Microsoft.Extensions.Options.ConfigurationExtensions**: `9.0.8`

### Additional Packages
- **Material.Icons.Avalonia**: `2.4.1`
- **System.Drawing.Common**: `8.0.0`

## Platform Support
- **Windows**: `win-x64`, `win-x86`
- **macOS**: `osx-x64`, `osx-arm64`
- **Linux**: `linux-x64`, `linux-arm64`

## Key Configuration Settings
- **Compiled Bindings**: Disabled (`<AvaloniaUseCompiledBindingsByDefault>false</AvaloniaUseCompiledBindingsByDefault>`)
- **XAML Compilation**: Enabled (`<EnableAvaloniaXamlCompilation>true</EnableAvaloniaXamlCompilation>`)
- **Assembly Info Generation**: Enabled
- **Cross-Platform Support**: Full cross-platform runtime identifiers

## Documentation Update Standards
- All version references in documentation MUST match these exact versions
- No approximate versions (e.g., "11.x" or "latest") allowed
- Version consistency validation required across all .github/ documentation files

## Validation Notes
- Microsoft Extensions packages are consistently at `9.0.8`
- Avalonia packages are consistently at `11.3.4` (except Xaml.Interactivity at `11.3.0.6`)
- Single target framework approach for cross-platform compatibility
- Configuration supports both Windows-specific and cross-platform deployments