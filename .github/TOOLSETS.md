# TOOLSETS.md

## Overview

This file documents the primary toolsets used in the MTM_WIP_Application_Avalonia repository. It is intended to help coding agents and developers understand which tools are available, how to use them, and where to find relevant commands for building, testing, and maintaining the project.

---

## Core Toolsets

### .NET SDK
- **Version:** 8.0+
- **Purpose:** Build, run, test, and publish the application
- **Key Commands:**
  - Restore dependencies: `dotnet restore`
  - Build: `dotnet build`
  - Run: `dotnet run --project MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia.csproj`
  - Watch (hot reload): `dotnet watch run --project MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia.csproj`
  - Test: `dotnet test`
  - Format: `dotnet format`
  - Publish: `dotnet publish -c Release -r win-x64 --self-contained true`

### Avalonia UI
- **Purpose:** Cross-platform UI framework for .NET
- **Usage:**
  - XAML files for UI layout (`.axaml`)
  - C# for code-behind and view models
  - Hot reload supported via `dotnet watch`

### ReactiveUI
- **Purpose:** MVVM framework for reactive programming in .NET
- **Usage:**
  - Used in ViewModels for data binding and reactive state management

### Configuration
- **Files:** `appsettings.json`, `appsettings.Development.json`, `Config/appsettings.json`
- **Purpose:** Store environment and application settings

### Logging & Diagnostics
- **Tools:** .NET built-in logging, Visual Studio Diagnostic Tools, JetBrains dotTrace (optional)
- **Purpose:** Debugging, performance profiling, and error tracking

---

## Optional/Recommended Toolsets

### Code Quality
- **Analyzers:** .NET built-in analyzers, EditorConfig
- **Formatting:** `dotnet format`
- **Linting:** (Add custom analyzers or third-party tools as needed)

### Testing
- **Frameworks:** xUnit, NUnit, or MSTest (add as needed)
- **Coverage:** [coverlet](https://github.com/coverlet-coverage/coverlet) (optional)

### CI/CD
- **Tools:** (Add details if using GitHub Actions, Azure Pipelines, etc.)

---

## How to Extend Toolsets
- Add new tools by updating this file and AGENTS.md
- Document any new scripts or utilities in the repository
- Ensure all agents and developers are aware of changes

---

## References
- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Avalonia Documentation](https://docs.avaloniaui.net/)
- [ReactiveUI Documentation](https://www.reactiveui.net/)
