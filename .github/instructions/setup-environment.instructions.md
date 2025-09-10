---
description: 'Complete development environment setup and configuration procedures for MTM WIP Application development'
mode: 'instruction'
tools: ['codebase', 'editFiles', 'search']
---

# Development Environment Setup Instructions

Complete guide for setting up a development environment for the MTM WIP Application using .NET 8, Avalonia UI 11.3.4, and MySQL 9.4.0.

## When to Use This Guide

Use this guide when:
- Setting up a new development machine
- Onboarding new team members to the project
- Troubleshooting development environment issues
- Updating development tools and dependencies

## Prerequisites

### System Requirements
- **Operating System**: Windows 10/11, macOS 10.15+, or Ubuntu 20.04+
- **RAM**: Minimum 8GB, recommended 16GB+
- **Storage**: At least 10GB free space for development tools
- **Network**: Reliable internet connection for package downloads

## Step-by-Step Setup Process

### 1. Install .NET 8 SDK

**Windows:**
```powershell
# Download and install .NET 8 SDK from Microsoft
winget install Microsoft.DotNet.SDK.8

# Verify installation
dotnet --version
# Expected output: 8.0.x
```

**macOS:**
```bash
# Install via Homebrew
brew install dotnet

# Verify installation
dotnet --version
```

**Linux (Ubuntu):**
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Install .NET 8 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Verify installation
dotnet --version
```

### 2. Install Visual Studio Code with Extensions

**Essential Extensions:**
```bash
# Install VS Code extensions via command line
code --install-extension ms-dotnettools.csharp
code --install-extension ms-dotnettools.vscode-dotnet-runtime
code --install-extension GitHub.copilot
code --install-extension GitHub.copilot-chat
code --install-extension ms-vscode.vscode-json
code --install-extension redhat.vscode-xml
```

**MTM-Specific VS Code Settings:**
```json
// .vscode/settings.json
{
    "dotnet.defaultSolution": "MTM_WIP_Application_Avalonia.sln",
    "files.exclude": {
        "**/bin": true,
        "**/obj": true,
        "**/.vs": true
    },
    "omnisharp.enableRoslynAnalyzers": true,
    "omnisharp.enableEditorConfigSupport": true,
    "csharp.semanticHighlighting.enabled": true
}
```

### 3. Install MySQL Development Environment

**MySQL Server Installation:**
```bash
# Windows (via Chocolatey)
choco install mysql

# macOS (via Homebrew)
brew install mysql
brew services start mysql

# Linux (Ubuntu)
sudo apt-get update
sudo apt-get install mysql-server
sudo systemctl start mysql
sudo systemctl enable mysql
```

**MySQL Configuration for MTM:**
```sql
-- Create development database
CREATE DATABASE MTM_WIP_Development;

-- Create development user
CREATE USER 'mtm_dev'@'localhost' IDENTIFIED BY 'DevPassword123!';
GRANT ALL PRIVILEGES ON MTM_WIP_Development.* TO 'mtm_dev'@'localhost';
FLUSH PRIVILEGES;
```

### 4. Clone and Configure MTM Repository

```bash
# Clone the repository
git clone https://github.com/Dorotel/MTM_WIP_Application_Avalonia.git
cd MTM_WIP_Application_Avalonia

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build
```

### 5. Configure Application Settings

**Create Development Configuration:**
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MTM_WIP_Development;Uid=mtm_dev;Pwd=DevPassword123!;SslMode=none;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "MTMSettings": {
    "Theme": "MTM_Blue",
    "EnableDeveloperMode": true,
    "ShowDeveloperConsole": true
  }
}
```

**Environment Variables Setup:**
```bash
# Windows (PowerShell)
$env:DOTNET_ENVIRONMENT = "Development"
$env:MTM_LOG_LEVEL = "Debug"

# macOS/Linux (Bash)
export DOTNET_ENVIRONMENT=Development
export MTM_LOG_LEVEL=Debug
```

### 6. Install Avalonia Development Tools

```bash
# Install Avalonia templates
dotnet new install Avalonia.ProjectTemplates

# Install Avalonia UI designer (if using Visual Studio)
# Download from: https://avaloniaui.net/download
```

### 7. Verify Complete Setup

**Run Verification Script:**
```bash
# Verify .NET version
dotnet --version
echo "Expected: 8.0.x"

# Verify project builds
dotnet build --configuration Debug
echo "Expected: Build succeeded"

# Verify tests run
dotnet test
echo "Expected: All tests pass"

# Verify application starts
dotnet run --project MTM_WIP_Application_Avalonia
echo "Expected: Application window opens"
```

## MTM-Specific Development Configuration

### IDE Configuration for MTM Patterns

**EditorConfig Settings (.editorconfig):**
```ini
# MTM Coding Standards
root = true

[*.cs]
# Indentation preferences
indent_style = space
indent_size = 4

# C# code style rules
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true

# MVVM Community Toolkit preferences
dotnet_diagnostic.MVVMTK0001.severity = error  # Use ObservableProperty
dotnet_diagnostic.MVVMTK0002.severity = error  # Use RelayCommand
```

### Database Development Setup

**Install Database Tools:**
```bash
# Install MySQL Workbench for database management
# Windows: winget install Oracle.MySQLWorkbench
# macOS: brew install --cask mysqlworkbench
# Linux: sudo apt install mysql-workbench
```

**Connection Configuration:**
```csharp
// Connection string validation
private static void ValidateConnectionString()
{
    var connectionString = Configuration.GetConnectionString("DefaultConnection");
    using var connection = new MySqlConnection(connectionString);
    connection.Open();
    Console.WriteLine("Database connection successful");
}
```

### Git Configuration for MTM Development

```bash
# Configure Git for MTM standards
git config user.name "Your Name"
git config user.email "your.email@company.com"

# Set up commit message template
git config commit.template .github/.gitmessage

# Configure line endings
git config core.autocrlf true  # Windows
git config core.autocrlf input # macOS/Linux
```

## Troubleshooting Common Setup Issues

### .NET SDK Issues
```bash
# Clear NuGet cache if packages fail to restore
dotnet nuget locals all --clear

# Reinstall specific packages
dotnet remove package [PackageName]
dotnet add package [PackageName]
```

### MySQL Connection Issues
```bash
# Reset MySQL root password
sudo mysql
ALTER USER 'root'@'localhost' IDENTIFIED WITH mysql_native_password BY 'NewPassword123!';
FLUSH PRIVILEGES;
EXIT;
```

### Avalonia Designer Issues
```bash
# Clear Avalonia designer cache
dotnet clean
dotnet build

# Restart Avalonia designer
# VS Code: Ctrl+Shift+P > "Avalonia: Restart Avalonia Designer"
```

## Development Workflow Configuration

### Pre-commit Hooks Setup
```bash
# Install pre-commit hooks for code quality
npm install -g @commitlint/cli @commitlint/config-conventional
echo "module.exports = {extends: ['@commitlint/config-conventional']}" > commitlint.config.js
```

### Debugging Configuration
```json
// .vscode/launch.json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch MTM Application",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/bin/Debug/net8.0/MTM_WIP_Application_Avalonia.dll",
            "args": [],
            "cwd": "${workspaceFolder}",
            "console": "integratedTerminal",
            "stopAtEntry": false
        }
    ]
}
```

## Success Validation Checklist

- [ ] .NET 8 SDK installed and version verified
- [ ] Visual Studio Code configured with essential extensions
- [ ] MySQL server running and development database created
- [ ] MTM repository cloned and builds successfully
- [ ] Application configuration files created
- [ ] Environment variables set correctly
- [ ] Avalonia development tools installed
- [ ] Application runs without errors
- [ ] Git configured for MTM development standards
- [ ] Database connection successful
- [ ] Pre-commit hooks configured
- [ ] Debugging configuration functional

## Next Steps

After completing environment setup:
1. Review [Testing Procedures](testing-procedures.instructions.md) for testing setup
2. Consult [Component Development](component-development.instructions.md) for UI development
3. Reference [MVVM Community Toolkit](mvvm-community-toolkit.instructions.md) patterns
4. Check [Service Architecture](service-architecture.instructions.md) for dependency injection

## Getting Help

If you encounter issues during setup:
- Check the [Troubleshooting documentation](../Troubleshooting/)
- Review the [MTM Development Guides](../Development-Guides/)
- Contact the development team via the project channels
- Create an issue in the repository with setup details