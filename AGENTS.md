# AGENTS.md

## Project Overview

The **MTM WIP Application** is a cross-platform manufacturing Work-in-Process management system built with .NET 8 and Avalonia UI. This enterprise-grade application manages inventory operations, manufacturing workflows, and operator transactions for Material Handler Manufacturing (MTM) systems.

### Key Technologies

- **.NET 8.0** - Cross-platform runtime with single target framework
- **Avalonia UI 11.3.4** - Cross-platform XAML UI framework
- **MVVM Community Toolkit 8.3.2** - Modern MVVM implementation
- **MySQL 9.4.0** - Production database with Dapper ORM
- **Microsoft.Extensions 9.0.8** - Dependency injection, logging, configuration
- **Material Icons Avalonia** - Material Design iconography

### Architecture

- **MVVM Pattern** - ViewModels, Views, Models separation
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **Service Layer** - Centralized business logic
- **Cross-Platform Support** - Windows, macOS, Linux, Android
- **Modular Design** - Custom controls, behaviors, converters

## Available AI Agent Tools

This repository includes **32 comprehensive AI agent tools** that provide complete development automation capabilities:

### Core Development Tools

- **codebase** - Full codebase analysis and manipulation
- **search** - Advanced code and content search
- **read** - File and directory reading
- **analysis** - Deep code analysis and pattern recognition
- **file_search** - Pattern-based file discovery
- **grep_search** - Content search with regex support
- **get_search_view_results** - Search results aggregation
- **list_dir** - Directory structure exploration
- **read_file** - Detailed file content reading
- **semantic_search** - AI-powered contextual search

### Joyride Automation Tools

- **joyride_evaluate_code** - Automated code evaluation
- **joyride_request_human_input** - Interactive development workflows
- **joyride_basics_for_agents** - Agent automation fundamentals
- **joyride_assisting_users_guide** - User assistance automation

### Advanced Integration Tools

- **web_search** - External documentation and reference retrieval
- **run_terminal** - Command execution and build automation
- **edit_file** - Direct file modification
- **create_file** - New file creation
- **move_file** - File organization
- **delete_file** - File cleanup
- **git_operations** - Version control automation
- **database_query** - Direct MySQL integration
- **test_runner** - Automated testing execution
- **documentation_generator** - Auto-documentation creation
- **dependency_analyzer** - Package and dependency management
- **performance_profiler** - Application performance analysis
- **security_scanner** - Code security validation
- **cross_platform_tester** - Multi-platform validation
- **ui_automation** - Avalonia UI testing
- **manufacturing_domain_validator** - MTM business rule validation
- **copilot_optimizer** - GitHub Copilot integration enhancement

### Comprehensive Instruction Library

The repository contains **34 specialized instruction files** covering:

#### Architecture & Patterns

- **dotnet-architecture-good-practices** - .NET architecture guidelines
- **avalonia-ui-guidelines** - Avalonia UI best practices
- **mvvm-community-toolkit** - MVVM implementation patterns
- **custom-controls** - Avalonia custom control development
- **value-converters** - Data binding converters
- **avalonia-behaviors** - Behavioral pattern implementation

#### Manufacturing Domain

- **mtm-manufacturing-context** - Manufacturing business domain
- **mtm-technology-context** - Technology stack integration
- **manufacturing-kpi-dashboard** - KPI system integration
- **quality-management-system** - QMS integration patterns
- **industry-40-integration** - Industry 4.0 connectivity

#### Data & Integration

- **mysql-database-patterns** - MySQL optimization and patterns
- **database-integration** - Database connection management
- **service-integration** - Cross-service communication
- **external-system-integration** - Third-party system integration
- **enterprise-integration-patterns** - Enterprise architecture

#### Testing & Quality

- **testing-standards** - Comprehensive testing framework
- **unit-testing-patterns** - MVVM unit testing
- **integration-testing-patterns** - Service integration testing
- **cross-platform-testing-standards** - Multi-platform validation
- **database-testing-patterns** - Database validation testing
- **ui-automation-standards** - UI testing automation

#### Performance & Management

- **advanced-performance-testing-framework** - Performance validation
- **advanced-quality-assurance-framework** - QA automation
- **resource-management** - Memory and resource optimization
- **application-configuration** - Configuration management

#### Development Workflow

- **advanced-github-copilot-integration** - AI development optimization
- **advanced-manufacturing-workflows** - Manufacturing-specific workflows
- **advanced-manufacturing-documentation** - Documentation automation
- **pitfalls** - Common issues and solutions



## 🤖 Enhanced with Joyride Automation

This repository supports **Joyride-enhanced development workflows** for maximum productivity:

**Key Joyride Capabilities:**
- **`joyride_evaluate_code`**: Direct VS Code Extension API automation
- **`joyride_request_human_input`**: Interactive human-in-the-loop workflows
- **`joyride_basics_for_agents`**: Agent automation patterns and capabilities
- **`joyride_assisting_users_guide`**: User-focused assistance automation

**MTM Development Automation:**
- MVVM Community Toolkit pattern enforcement and validation
- Automated Avalonia UI component generation following MTM standards
- Real-time validation of manufacturing domain rules (operations 90/100/110)
- Dynamic theme system testing and validation (17+ theme files)
- MySQL database integration testing and stored procedure validation
- Cross-platform build and deployment automation workflows

**Usage Priority**: Use Joyride automation whenever safe and possible to enhance consistency, speed, and quality in MTM development workflows.

## Setup Commands

### Prerequisites

- .NET 8.0 SDK
- MySQL Server 9.4.0 (local or network)
- Visual Studio 2022 or VS Code with C# extension

### Installation

```powershell
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

1. Install MySQL Server 9.4.0
2. Update connection string in `Config/appsettings.json`
3. Run database initialization scripts (45+ stored procedures)

### Configuration

- **Development**: `Config/appsettings.Development.json`
- **Production**: `Config/appsettings.json`
- **Test**: `Config/test-config.json`

## Development Workflow

### Starting Development

```powershell
# Start with hot reload (if available)
dotnet watch run

# Build and run specific configuration
dotnet run --configuration Debug

# Run with specific environment
dotnet run --environment Development
```

### Platform-Specific Building

```powershell
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
- Configure MySQL 9.4.0 connection for local development
- Enable detailed logging in development

## Testing Instructions

The MTM application follows comprehensive cross-platform testing standards:

### Unit Testing

```powershell
# Run all unit tests
dotnet test --filter Category=Unit

# Run specific test class
dotnet test --filter "ClassName~InventoryTabViewModelTests"

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Testing  

```powershell
# Run integration tests
dotnet test --filter Category=Integration

# Run database integration tests
dotnet test --filter Category=Database

# Run service integration tests
dotnet test --filter Category=Service
```

### Cross-Platform Testing

```powershell
# Test on specific runtime
dotnet test --runtime win-x64
dotnet test --runtime osx-x64
dotnet test --runtime linux-x64
```

### UI Automation Testing

```powershell
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
/Views - Avalonia AXAML views (32+ files)
/ViewModels - MVVM ViewModels with CommunityToolkit (42+ files)
/Models - Data models and DTOs (12+ files)
/Services - Business logic services (20+ files)
/Controls - Custom Avalonia controls (CollapsiblePanel, CustomDataGrid, SessionHistoryPanel)
/Converters - Value converters (4 files)
/Behaviors - Avalonia behaviors (3 files)
/Config - Configuration files (3 files)
/Resources/Themes - AXAML theme files (17 files)
/.github/instructions - Comprehensive instruction library (34 files)
/.github/prompts - Development automation prompts (54+ files)
```

### MVVM Patterns

- Use `CommunityToolkit.Mvvm` attributes: `[ObservableProperty]`, `[RelayCommand]`
- Implement `ObservableObject` base class
- Use `IAsyncRelayCommand` for async operations
- Follow dependency injection patterns for services

### Database Patterns

- Use Dapper for data access with MySQL 9.4.0
- Implement repository pattern for data operations
- Use 45+ stored procedures for complex operations
- Follow async/await patterns for database calls

### Avalonia UI Patterns

- Use AXAML for views with minimal code-behind logic
- Implement custom controls in Controls folder
- Use value converters for data transformation
- Follow Avalonia 11.3.4 binding conventions
- Utilize 17 theme files for comprehensive theming

## Build and Deployment

### Build Commands

```powershell
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

```powershell
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

- **Debug**: net8.0
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

```powershell
# Before submitting PR
dotnet build --configuration Release
dotnet test
dotnet format --verify-no-changes
```

### Code Review Requirements

- All unit tests pass
- Cross-platform compatibility verified
- Database operations tested with MySQL 9.4.0
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
- **Quick Buttons**: Rapid transaction shortcuts (max 10 per user)
- **Master Data**: Parts, locations, operations setup

### Business Rules

- All transactions require operation validation
- Location codes must be valid (`FLOOR`, `RECEIVING`, `SHIPPING`)
- Valid operations: `90` (Move), `100` (Receive), `110` (Ship)
- Session timeouts after 60 minutes of inactivity
- Maximum 10 quick buttons per user

### Database Schema

- 45+ stored procedures for operations
- MySQL 9.4.0 with connection pooling (5-100 connections)
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

## AI Development Automation

### GitHub Copilot Integration

- **32 comprehensive tools** for complete development automation
- **54+ prompt files** for specialized development scenarios
- **34 instruction files** covering all aspects of MTM development
- Advanced pattern recognition for manufacturing domain
- Cross-platform testing automation
- Database integration patterns
- UI automation capabilities

### Joyride Automation

- **joyride_evaluate_code** - Automated code quality assessment
- **joyride_request_human_input** - Interactive development workflows
- **joyride_basics_for_agents** - Agent automation fundamentals
- **joyride_assisting_users_guide** - User assistance patterns

### Manufacturing Domain AI

- **manufacturing_domain_validator** - Business rule validation
- **quality_management_integration** - QMS automation
- **industry_40_integration** - IoT and Industry 4.0 patterns
- **kpi_dashboard_automation** - Manufacturing metrics automation

## Troubleshooting

### Common Issues

- **Database Connection**: Check `ConnectionStrings:DefaultConnection` for MySQL 9.4.0
- **Theme Loading**: Verify 17 theme files in Themes folder
- **Cross-Platform**: Test on target platform before deployment
- **Memory Usage**: Monitor working set, especially with large datasets
- **File Logging**: Check network path accessibility for logging

### Debug Configuration

```powershell
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
- **MySQL Workbench** - Database management for MySQL 9.4.0
- **Avalonia DevTools** - Runtime debugging (Debug builds only)

### Development Best Practices

- Use dependency injection for all services
- Implement proper error handling with logging
- Follow MVVM patterns strictly with CommunityToolkit.Mvvm 8.3.2
- Test on multiple platforms during development
- Use configuration files for environment-specific settings
- Implement proper disposal patterns for resources
- Leverage 32 AI tools for comprehensive automation
- Follow manufacturing domain patterns from instruction library

### Performance Monitoring

- Application health service available
- File logging with configurable levels
- Network logging optional (for shared drives)
- Memory and thread monitoring
- Database connection pooling metrics (MySQL 9.4.0)

### Repository Structure Analysis

- **Views**: 32+ Avalonia AXAML view files
- **ViewModels**: 42+ MVVM ViewModels using CommunityToolkit
- **Services**: 20+ business logic services
- **Models**: 12+ data models and DTOs
- **Custom Controls**: 3 specialized Avalonia controls
- **Themes**: 17 comprehensive theme files
- **Instructions**: 34 specialized development instruction files
- **Prompts**: 54+ AI automation prompt files
- **Database**: 45+ MySQL stored procedures
