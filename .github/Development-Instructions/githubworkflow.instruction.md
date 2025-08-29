<!-- Copilot: Reading github-workflow.instruction.md — GitHub Workflow -->
# GitHub Workflow & CI/CD Instructions

## Overview
This document provides comprehensive GitHub workflow and CI/CD instructions for the MTM WIP Application Avalonia project. The project uses .NET 8, Avalonia UI, and standard .NET MVVM patterns.

## General GitHub Workflow Guidelines

- Make small, focused commits with descriptive commit messages.
- Open pull requests early for feedback.
- Reference related issues and documentation where applicable.
- If your change involves error-handling or logging, cross-check with [error_handler-instruction.md](error_handler-instruction.md).
- **CRITICAL**: When updating core systems, ensure corresponding updates to the `Exportable/` folder (see [Exportable Folder Maintenance](#exportable-folder-maintenance))
- Pull Request Guidelines:
  - Provide a clear summary of the problem and solution.
  - Include screenshots or logs if the change affects error handling.
  - Request review from at least one team member.
  - Include checklist item for Exportable folder updates if core systems are modified.

## Exportable Folder Maintenance

### When to Update Exportable Folder

**MANDATORY UPDATES** - Update `Exportable/` folder whenever changes are made to:
- Any file in `Services/` folder
- Any file in `Models/` folder  
- Any file in `Configuration/` folder
- Core infrastructure components
- Error handling systems
- Logging utilities

### Update Process

1. **Copy Updated Systems**: Copy modified files from main project to corresponding `Exportable/` locations
2. **Remove Framework Dependencies**: Strip out Avalonia specific code to make framework-agnostic
3. **Update Documentation**: Update `Exportable/README.md` with any new systems or changes
4. **Update Custom Prompts**: Add/modify prompts in `Exportable/exportable-customprompt.instruction.md` if new systems are added
5. **Test Compilation**: Ensure all exportable systems compile independently
6. **Update Dependencies**: Modify NuGet package requirements if needed

### Exportable Folder Structure
```
Exportable/
├── README.md                           # Main documentation (ALWAYS UPDATE)
├── exportable-customprompt.instruction.md  # Custom prompts (UPDATE WHEN ADDING SYSTEMS)
├── Models/                             # Framework-agnostic data models
├── Services/                           # Core business services
│   ├── ErrorHandler/                   # Error handling system
│   ├── Logging/                        # Logging utilities
│   └── Interfaces/                     # Service contracts
├── Configuration/                      # Configuration management
├── Extensions/                         # Dependency injection setup
└── Infrastructure/                     # Cross-cutting concerns
```

### PR Checklist for Core System Changes

- [ ] Main project changes implemented and tested
- [ ] Corresponding files copied to `Exportable/` folder
- [ ] Framework-specific code removed from exportable versions
- [ ] `Exportable/README.md` updated with new systems/changes
- [ ] Custom prompts added/updated if new systems introduced
- [ ] Exportable systems compile independently
- [ ] Version history updated in README.md

## .NET 8 Build Configuration

### Project Requirements
- **Target Framework**: .NET 8 (`<TargetFramework>net8.0</TargetFramework>`)
- **Output Type**: WinExe for desktop application
- **Nullable**: Enabled (`<Nullable>enable</Nullable>`)
- **Compiled Bindings**: Enabled by default (`<AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>`)

### Required Package References
```xml
<PackageReference Include="Avalonia" Version="11.3.4" />
<PackageReference Include="Avalonia.Desktop" Version="11.3.4" />
<PackageReference Include="Avalonia.Themes.Fluent" Version="11.3.4" />
<PackageReference Include="Avalonia.Fonts.Inter" Version="11.3.4" />
<PackageReference Include="Avalonia.Diagnostics" Version="11.3.4" Condition="'$(Configuration)' == 'Debug'" />
```

### Build Configuration Guidelines
- Debug builds include Avalonia.Diagnostics for development
- Release builds exclude Avalonia.Diagnostics package
- Use `BuiltInComInteropSupport` for Windows compatibility
- Include application manifest (`ApplicationManifest>app.manifest</ApplicationManifest>`)

## CI/CD Pipeline Configuration

### Basic .NET 8 CI Workflow (.github/workflows/ci.yml)
```yaml
name: CI Build and Test

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

env:
  DOTNET_VERSION: '8.0.x'
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET 8
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --configuration Release --no-build --verbosity normal
    
    - name: Validate Exportable Systems
      run: |
        # Ensure exportable systems compile independently
        cd Exportable
        find . -name "*.cs" -exec echo "Checking {}" \;
        # TODO: Add compilation check for exportable systems
    
    - name: Publish artifacts
      run: dotnet publish --configuration Release --no-build --output ./publish
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: published-app
        path: ./publish
        
    - name: Upload exportable systems
      uses: actions/upload-artifact@v4
      with:
        name: exportable-systems
        path: ./Exportable
```

### Cross-Platform Build Matrix
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
    dotnet-version: ['8.0.x']

runs-on: ${{ matrix.os }}
```

## Avalonia-Specific CI/CD Requirements

### Avalonia Build Dependencies
- Ensure all required Avalonia packages are restored
- Include platform-specific build steps for desktop targets
- Handle Avalonia.Diagnostics conditional compilation

### Desktop Application Packaging
```yaml
- name: Package Windows Application
  if: matrix.os == 'windows-latest'
  run: |
    dotnet publish -c Release -r win-x64 --self-contained true
    
- name: Package Linux Application  
  if: matrix.os == 'ubuntu-latest'
  run: |
    dotnet publish -c Release -r linux-x64 --self-contained true
    
- name: Package macOS Application
  if: matrix.os == 'macos-latest'
  run: |
    dotnet publish -c Release -r osx-x64 --self-contained true
```

### AXAML Compilation Validation
```yaml
- name: Validate AXAML Compilation
  run: |
    dotnet build --configuration Debug --verbosity normal
    # Ensure no AXAML compilation errors with AvaloniaUseCompiledBindingsByDefault
```

## Standard .NET MVVM Testing Patterns

### Unit Testing Standard .NET ViewModels
```yaml
- name: Install Test Dependencies
  run: |
    dotnet add package xunit
    dotnet add package xunit.runner.visualstudio
    dotnet add package Microsoft.NET.Test.Sdk
    dotnet add package Moq
```

### Test Configuration
```csharp
// Example test setup for Standard .NET ViewModels
[Test]
public void ViewModel_PropertyChanges_NotifyCorrectly()
{
    new TestScheduler().With(scheduler =>
    {
        var viewModel = new SampleViewModel();
        var results = new List<string>();
        
        viewModel.WhenAnyValue(vm => vm.PropertyName)
               .Subscribe(results.Add);
               
        viewModel.PropertyName = "Test";
        scheduler.AdvanceByMs(1);
        
        Assert.Contains("Test", results);
    });
}
```

### Command Testing Patterns
```yaml
- name: Test Standard .NET Commands
  run: |
    # Tests should validate:
    # - Command CanExecute behavior
    # - Async command completion
    # - Error handling in command execution
    # - Observable property changes triggered by commands
    dotnet test --filter "Category=StandardMVVM" --logger trx
```

## MTM Project Deployment Guidelines

### Deployment Environment Configuration
- **Development**: Auto-deploy on develop branch pushes
- **Staging**: Deploy on release branch creation
- **Production**: Manual deployment trigger for main branch

### MTM-Specific Deployment Steps
```yaml
deploy:
  needs: build
  runs-on: windows-latest
  if: github.ref == 'refs/heads/main'
  
  steps:
  - name: Download artifacts
    uses: actions/download-artifact@v4
    with:
      name: published-app
      
  - name: Download exportable systems
    uses: actions/download-artifact@v4
    with:
      name: exportable-systems
      path: ./exportable-release
      
  - name: Configure MTM Application Settings
    run: |
      # Update appsettings.json for production
      # Configure database connection strings
      # Set error logging paths for MTM environment
      
  - name: Deploy to MTM Infrastructure
    run: |
      # MTM-specific deployment commands
      # Update Windows services if applicable
      # Configure file server access for error logging
      
  - name: Package Exportable Systems
    run: |
      # Create versioned release of exportable systems
      # Include documentation and custom prompts
      # Archive for distribution to other projects
```

### Configuration Management
- Store sensitive configuration in GitHub Secrets
- Use environment-specific appsettings files
- Configure database connection strings per environment
- Set up error logging paths for MTM file server access

### Release Management
```yaml
- name: Create Release
  uses: actions/create-release@v1
  with:
    tag_name: v${{ github.run_number }}
    release_name: MTM WIP Application v${{ github.run_number }}
    body: |
      ## Changes
      - Automated release from CI/CD pipeline
      - .NET 8 Avalonia application
      - Standard .NET MVVM implementation
      - Updated exportable core systems
      
      ## Exportable Systems
      - Error handling and logging utilities
      - Configuration management
      - Framework-agnostic service interfaces
      - Custom implementation prompts available
```

## Build Validation Requirements

### Pre-merge Checks
- All unit tests must pass
- Code must compile without warnings in Release mode
- AXAML files must compile successfully with compiled bindings
- Standard .NET property change notifications must work correctly
- No nullable reference warnings
- **Exportable systems must compile independently**

### Exportable Systems Validation
```yaml
- name: Validate Exportable Systems Independence
  run: |
    # Create temporary project to test exportable systems
    mkdir temp-test-project
    cd temp-test-project
    dotnet new console
    
    # Copy exportable systems
    cp -r ../Exportable/* ./
    
    # Try to compile (should succeed without Avalonia dependencies)
    dotnet add package Microsoft.Extensions.DependencyInjection
    dotnet add package Microsoft.Extensions.Configuration
    dotnet build --configuration Release
```

### Performance Testing
- Include basic performance benchmarks for UI responsiveness
- Validate memory usage patterns with standard .NET event handling
- Test application startup time

### Code Quality Gates
```yaml
- name: Code Analysis
  run: |
    dotnet format --verify-no-changes
    dotnet build --configuration Release --verbosity normal /warnaserror
```

## Deployment Artifacts

### Required Artifacts
- Self-contained executable for Windows x64
- Application configuration files
- Database schema updates (if applicable)
- Error logging configuration templates
- User documentation updates
- **Exportable core systems package**

### Artifact Structure
```
MTM-WIP-Application-v{version}/
├── Application/
│   ├── MTM_WIP_Application_Avalonia.exe
│   ├── appsettings.json
│   ├── appsettings.Production.json
│   └── Config/
├── Exportable-Systems/
│   ├── README.md
│   ├── exportable-customprompt.instruction.md
│   ├── Models/
│   ├── Services/
│   ├── Configuration/
│   └── Extensions/
└── Documentation/
    ├── README.md
    ├── CHANGELOG.md
    └── exportable-integration-guide.md
```

## Error Handling in CI/CD

### Build Failure Handling
- Notify team via GitHub issues for build failures
- Automatically retry builds for transient failures
- Collect and archive build logs for analysis

### Deployment Rollback
- Maintain previous version artifacts for quick rollback
- Automated health checks post-deployment
- Database migration rollback procedures

## Security Considerations

### Secret Management
- Store database connection strings in GitHub Secrets
- Use environment variables for sensitive configuration
- Rotate secrets regularly

### Code Scanning
```yaml
- name: Security Scan
  uses: github/codeql-action/analyze@v3
  with:
    languages: csharp
```

## Monitoring and Alerts

### Post-Deployment Monitoring
- Application health checks
- Error rate monitoring
- Performance metric collection
- User access pattern analysis

### Alert Configuration
- Build failure notifications
- Deployment status updates
- Security vulnerability alerts
- Performance degradation warnings

---

## Cross-References
- [Error Handler Instructions](error_handler-instruction.md) - For error logging in CI/CD
- [Coding Conventions](codingconventions.instruction.md) - For build validation standards
- [UI Generation](ui-generation.instruction.md) - For AXAML compilation requirements
- [Naming Conventions](naming.conventions.instruction.md) - For consistent file naming in artifacts
- [Exportable Systems](../Exportable/README.md) - For core systems integration guidance