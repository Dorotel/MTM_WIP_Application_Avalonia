# AGENTS.md

## Project Overview

MTM_WIP_Application_Avalonia is a desktop application built with [Avalonia UI](https://avaloniaui.net/) targeting .NET 8.0. It provides inventory and transaction management features, with a modular architecture using MVVM (Model-View-ViewModel) patterns. The project is organized into logical folders for Commands, Config, Controls, Extensions, Models, Services, ViewModels, and Views. Key technologies include C#, Avalonia, and ReactiveUI.

## Setup Commands

- **Install .NET 8.0 SDK**: Download and install from https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- **Restore dependencies**:
  ```powershell
  dotnet restore
  ```
- **Build the solution**:
  ```powershell
  dotnet build
  ```
- **Run the application**:
  ```powershell
  dotnet run --project MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia.csproj
  ```
- **Configuration**: Edit `appsettings.json` or `appsettings.Development.json` in the project root or `Config/` as needed.

## Development Workflow

- **Start development**: Use `dotnet run` as above. Hot reload is supported in Avalonia for XAML and C# changes.
- **Watch for changes**:
  ```powershell
  dotnet watch run --project MTM_WIP_Application_Avalonia/MTM_WIP_Application_Avalonia.csproj
  ```
- **Environment variables**: Set via `appsettings.json` or user secrets if needed.
- **XAML UI changes**: Edit `.axaml` files in the project root or `Controls/`, `Resources/Themes/`, and `Views/`.

## Testing Instructions

- **Test framework**: (Add details if tests exist; otherwise, see below)
- **Run all tests**:
  ```powershell
  dotnet test
  ```
- **Test file locations**: Place test projects in a `Tests/` folder or alongside code with `.Tests` suffix. (No test projects detected; add as needed.)
- **Coverage**: Use [coverlet](https://github.com/coverlet-coverage/coverlet) or similar for code coverage.
- **Test patterns**: Use xUnit, NUnit, or MSTest as preferred.

## Code Style Guidelines

- **Language**: C# 10+, .NET 8.0
- **Frameworks**: Avalonia, ReactiveUI
- **Linting**: Use built-in .NET analyzers and [EditorConfig](https://learn.microsoft.com/en-us/visualstudio/ide/editorconfig-code-style-settings-reference) for code style.
- **Formatting**: Run `dotnet format` to auto-format code.
- **File organization**: Use folders for Commands, Config, Controls, Extensions, Models, Services, ViewModels, and Views. Shared code goes in `Shared/` subfolders.
- **Naming conventions**: PascalCase for types and methods, camelCase for variables, I-prefixed interfaces, ViewModel suffix for view models.
- **Imports**: Use explicit `using` statements at the top of each file.

## Build and Deployment

- **Build**:
  ```powershell
  dotnet build
  ```
- **Output**: Binaries are in `bin/Debug/net8.0/` or `bin/Release/net8.0/`.
- **Release build**:
  ```powershell
  dotnet publish -c Release -r win-x64 --self-contained true
  ```
- **Deployment**: Distribute published files from `bin/Release/net8.0/win-x64/publish/`.
- **Configuration**: Use `appsettings.json` for environment-specific settings.
- **CI/CD**: (Add details if using GitHub Actions or other CI/CD tools.)

## Security Considerations

- **Secrets**: Do not commit secrets to source control. Use [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or environment variables for sensitive data.
- **Authentication**: (Add details if authentication is implemented.)
- **Permissions**: Ensure file/database access is restricted as needed.

## Pull Request Guidelines

- **Title format**: [Component] Brief description
- **Required checks**: Run `dotnet build` and `dotnet test` before submitting.
- **Review**: Ensure code follows style guidelines and includes relevant documentation/comments.
- **Commit messages**: Use imperative mood, e.g., "Add feature X".

## Debugging and Troubleshooting

- **Common issues**: Check .NET SDK version, restore dependencies, and verify Avalonia compatibility.
- **Logging**: Add logging via built-in .NET logging or third-party libraries as needed.
- **Debug configuration**: Use Visual Studio or VS Code launch profiles for debugging.
- **Performance**: Profile with Visual Studio Diagnostic Tools or JetBrains dotTrace.

## Additional Notes

- **Avalonia documentation**: https://docs.avaloniaui.net/
- **.NET documentation**: https://learn.microsoft.com/en-us/dotnet/
- **Update this file** as the project evolves to keep agent instructions current.
