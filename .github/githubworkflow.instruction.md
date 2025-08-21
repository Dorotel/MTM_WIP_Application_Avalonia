<!-- Copilot: Reading github-workflow.instruction.md — GitHub Workflow -->
# GitHub Workflow & CI/CD Instructions

## Overview
This document provides comprehensive GitHub workflow and CI/CD instructions for the MTM WIP Application Avalonia project. The project uses .NET 8, Avalonia UI, and ReactiveUI.

## General GitHub Workflow Guidelines

- Make small, focused commits with descriptive commit messages.
- Open pull requests early for feedback.
- Reference related issues and documentation where applicable.
- If your change involves error-handling or logging, cross-check with [error_handler-instruction.md](error_handler-instruction.md).
- Pull Request Guidelines:
  - Provide a clear summary of the problem and solution.
  - Include screenshots or logs if the change affects error handling.
  - Request review from at least one team member.

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
<PackageReference Include="Avalonia.ReactiveUI" Version="11.3.4" />
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
    
    - name: Publish artifacts
      run: dotnet publish --configuration Release --no-build --output ./publish
    
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: published-app
        path: ./publish
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

## ReactiveUI Testing Patterns

### Unit Testing ReactiveUI ViewModels
```yaml
- name: Install Test Dependencies
  run: |
    dotnet add package Microsoft.Reactive.Testing
    dotnet add package ReactiveUI.Testing
    dotnet add package xunit
    dotnet add package xunit.runner.visualstudio
    dotnet add package Microsoft.NET.Test.Sdk
```

### Test Configuration
```csharp
// Example test setup for ReactiveUI ViewModels
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
- name: Test ReactiveUI Commands
  run: |
    # Tests should validate:
    # - Command CanExecute behavior
    # - Async command completion
    # - Error handling in command execution
    # - Observable property changes triggered by commands
    dotnet test --filter "Category=ReactiveUI" --logger trx
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
      - ReactiveUI MVVM implementation
```

## Build Validation Requirements

### Pre-merge Checks
- All unit tests must pass
- Code must compile without warnings in Release mode
- AXAML files must compile successfully with compiled bindings
- ReactiveUI property change notifications must work correctly
- No nullable reference warnings

### Performance Testing
- Include basic performance benchmarks for UI responsiveness
- Validate memory usage patterns with ReactiveUI subscriptions
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

### Artifact Structure
```
MTM-WIP-Application-v{version}/
├── MTM_WIP_Application_Avalonia.exe
├── appsettings.json
├── appsettings.Production.json
├── Config/
│   ├── error-logging.json
│   └── database-config.json
└── Documentation/
    ├── README.md
    └── CHANGELOG.md
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