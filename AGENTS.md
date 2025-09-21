# AGENTS.md

## Project Overview

The **MTM WIP Application** is a cross-platform manufacturing Work-in-Process management system built with .NET 8 and Avalonia UI. This enterprise-grade application manages inventory operations, manufacturing workflows, and operator transactions for Material Handler Manufacturing (MTM) systems.

### Key Technologies

- **.NET 8.0** - Cross-platform runtime with single target framework
- **Avalonia UI 11.3.4** - Cross-platform XAML UI framework
- **MVVM Community Toolkit 8.3.2** - Modern MVVM implementation
- **MySQL** - Production database with Dapper ORM
- **Microsoft.Extensions** - Dependency injection, logging, configuration
- **Material Icons Avalonia** - Material Design iconography

### Architecture

- **MVVM Pattern** - ViewModels, Views, Models separation
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **Service Layer** - Centralized business logic
- **Cross-Platform Support** - Windows, macOS, Linux, Android
- **Modular Design** - Custom controls, behaviors, converters

## Setup Commands

### Prerequisites

- .NET 8.0 SDK
- MySQL Server (local or network)
- Visual Studio 2022 or VS Code with C# extension

### Installation

```bash
# Clone repository
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia

# Restore dependencies
dotnet restore

# Build application
dotnet build

# Run in development mode
dotnet run
```

### Database Setup

1. Install MySQL Server
2. Update connection string in `Config/appsettings.json`
3. Run database initialization scripts (if available)

### Configuration

- **Development**: `Config/appsettings.Development.json`
- **Production**: `Config/appsettings.json`
- **Test**: `Config/test-config.json`

## Development Workflow

### Starting Development

```bash
# Start with hot reload (if available)
dotnet watch run

# Build and run specific configuration
dotnet run --configuration Debug

# Run with specific environment
dotnet run --environment Development
```

### Platform-Specific Building

```bash
# Windows (x64)
dotnet publish -r win-x64 --self-contained

# macOS (Intel)
dotnet publish -r osx-x64 --self-contained

# macOS (Apple Silicon)
dotnet publish -r osx-arm64 --self-contained

# Linux (x64)
dotnet publish -r linux-x64 --self-contained

# Android (when mobile support added)
dotnet publish -r android-arm64 --self-contained
```

### Development Environment

- Use Visual Studio 2022 or VS Code with C# Dev Kit
- Install Avalonia extension for XAML editing
- Configure MySQL connection for local development
- Enable detailed logging in development

## Testing Instructions

The MTM application follows comprehensive cross-platform testing standards:

### Unit Testing

```bash
# Run all unit tests
dotnet test --filter Category=Unit

# Run specific test class
dotnet test --filter "ClassName~InventoryTabViewModelTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Testing  

```bash
# Run integration tests
dotnet test --filter Category=Integration

# Run database integration tests
dotnet test --filter Category=Database

# Run service integration tests
dotnet test --filter Category=Service
```

### Cross-Platform Testing

```bash
# Test on specific runtime
dotnet test --runtime win-x64
dotnet test --runtime osx-x64
dotnet test --runtime linux-x64
```

### UI Automation Testing

```bash
# Run UI tests (when implemented)
dotnet test --filter Category=UI

# Run cross-platform UI tests
dotnet test --filter Category=CrossPlatform
```

### Test Patterns

- **Unit Tests**: Individual component validation using MVVM Community Toolkit patterns
- **Integration Tests**: Service interaction and database validation
- **UI Tests**: Avalonia UI workflow testing
- **Cross-Platform Tests**: Feature validation on all supported platforms
- **End-to-End Tests**: Complete manufacturing operator workflows

## Code Style Guidelines

### Naming Conventions

- **Classes**: PascalCase (`InventoryService`, `MainWindowViewModel`)
- **Methods**: PascalCase (`GetInventoryAsync`, `SaveChanges`)
- **Properties**: PascalCase (`CurrentUser`, `IsLoading`)
- **Fields**: camelCase with underscore prefix (`_logger`, `_serviceProvider`)
- **Parameters**: camelCase (`userId`, `inventoryItem`)
- **Constants**: PascalCase (`MaxRetryAttempts`, `DefaultTimeout`)

### File Organization

```
/Views - Avalonia AXAML views
/ViewModels - MVVM ViewModels with CommunityToolkit
/Models - Data models and DTOs
/Services - Business logic services
/Controls - Custom Avalonia controls
/Converters - Value converters
/Behaviors - Avalonia behaviors
/Config - Configuration files
/Resources/Themes - AXAML theme files
```

### MVVM Patterns

- Use `CommunityToolkit.Mvvm` attributes: `[ObservableProperty]`, `[RelayCommand]`
- Implement `ObservableObject` base class
- Use `IAsyncRelayCommand` for async operations
- Follow dependency injection patterns for services

### Database Patterns

- Use Dapper for data access
- Implement repository pattern for data operations
- Use stored procedures for complex operations
- Follow async/await patterns for database calls

### Avalonia UI Patterns

- Use AXAML for views with code-behind minimal logic
- Implement custom controls in `/Controls` folder
- Use value converters for data transformation
- Follow Avalonia binding conventions

## Build and Deployment

### Build Commands

```bash
# Debug build
dotnet build --configuration Debug

# Release build
dotnet build --configuration Release

# Clean build
dotnet clean && dotnet build

# Restore and build
dotnet restore && dotnet build
```

### Publishing

```bash
# Self-contained deployment (Windows)
dotnet publish -c Release -r win-x64 --self-contained true

# Framework-dependent deployment
dotnet publish -c Release --self-contained false

# Single file deployment
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Platform-Specific Deployment

- **Windows**: Creates `.exe` with Windows-specific features
- **macOS**: Creates app bundle with native integration
- **Linux**: Creates executable with Linux compatibility
- **Android**: Mobile deployment (future implementation)

### Build Outputs

- **Debug**: `bin/Debug/net8.0/`
- **Release**: `bin/Release/net8.0/`
- **Published**: `bin/Release/net8.0/publish/`

## Pull Request Guidelines

### Title Format

`[component] Brief description of changes`

Examples:

- `[ViewModels] Add inventory validation logic`
- `[Services] Implement async database operations`
- `[UI] Update theme switching functionality`

### Required Checks

```bash
# Before submitting PR
dotnet build --configuration Release
dotnet test
dotnet format --verify-no-changes
```

### Code Review Requirements

- All unit tests pass
- Cross-platform compatibility verified
- Database operations tested
- UI changes tested on multiple platforms
- Performance impact considered
- Security implications reviewed

### Commit Message Format

Follow conventional commits:

- `feat:` New features
- `fix:` Bug fixes
- `docs:` Documentation changes
- `style:` Code style changes
- `refactor:` Code refactoring
- `test:` Adding tests
- `chore:` Maintenance tasks

## Manufacturing Domain Context

### Core Operations

- **Inventory Management**: Track parts, locations, quantities
- **Work Orders**: Manufacturing job processing
- **Transactions**: Operator actions and movements
- **Quick Buttons**: Rapid transaction shortcuts
- **Master Data**: Parts, locations, operations setup

### Business Rules

- All transactions require operation validation
- Location codes must be valid (`FLOOR`, `RECEIVING`, `SHIPPING`)
- Valid operations: `90` (Move), `100` (Receive), `110` (Ship)
- Session timeouts after 60 minutes of inactivity
- Maximum 10 quick buttons per user

### Database Schema

- 45+ stored procedures for operations
- MySQL with connection pooling
- Transaction logging and audit trails
- Master data synchronization

## Security and Performance

### Security Considerations

- Connection string encryption
- SQL injection prevention via Dapper parameterization
- Input validation at service layer
- Secure logging practices (no sensitive data)

### Performance Guidelines

- Async/await for all database operations
- Connection pooling (5-100 connections)
- Query timeout: 30 seconds
- Memory caching for master data
- File logging with rotation (50MB max, 30 day retention)

## Troubleshooting

### Common Issues

- **Database Connection**: Check `ConnectionStrings:DefaultConnection` in config
- **Theme Loading**: Verify theme files in `Resources/Themes/` folder
- **Cross-Platform**: Test on target platform before deployment
- **Memory Usage**: Monitor working set, especially with large datasets
- **File Logging**: Check network path accessibility for logging

### Debug Configuration

```bash
# Enable debug logging
dotnet run --configuration Debug

# Detailed logging
# Set MTM log level to "Debug" in appsettings.json

# Check application health
# Service provides health status monitoring
```

### Platform-Specific Issues

- **Windows**: Verify app.manifest for Windows-specific features
- **macOS**: Check code signing for distribution
- **Linux**: Ensure required libraries are available
- **Cross-Platform**: Test file path separators and case sensitivity

## Additional Notes

### Extensions and Tools

- **Avalonia for Visual Studio** - XAML editing support
- **C# Dev Kit** - Enhanced C# development
- **MySQL Workbench** - Database management
- **Avalonia DevTools** - Runtime debugging (Debug builds only)

### Development Best Practices

- Use dependency injection for all services
- Implement proper error handling with logging
- Follow MVVM patterns strictly
- Test on multiple platforms during development
- Use configuration files for environment-specific settings
- Implement proper disposal patterns for resources

### Performance Monitoring

- Application health service available
- File logging with configurable levels
- Network logging optional (for shared drives)
- Memory and thread monitoring
- Database connection pooling metrics

This AGENTS.md provides comprehensive context for AI coding agents to effectively work with the MTM WIP Application, covering all essential development workflows, testing patterns, and manufacturing domain knowledge required for successful contributions.
