#!/usr/bin/env pwsh
<#
.SYNOPSIS
Quick setup script for the MTM Advanced Custom Prompt Evolution System

.DESCRIPTION
This script initializes the complete MTM Prompt Evolution System including:
- Analysis and automation scripts
- PowerShell module integration
- Directory structure
- Sample configuration
- Documentation and examples

.PARAMETER FullSetup
Perform complete setup including scheduled tasks and GitHub Actions

.PARAMETER ModuleOnly
Install only the PowerShell module

.PARAMETER SkipExamples
Skip creation of example prompts and usage scenarios

.EXAMPLE
.\Initialize-PromptEvolution.ps1

.EXAMPLE
.\Initialize-PromptEvolution.ps1 -FullSetup

.EXAMPLE
.\Initialize-PromptEvolution.ps1 -ModuleOnly
#>

param(
    [switch]$FullSetup,
    [switch]$ModuleOnly,
    [switch]$SkipExamples
)

# Script metadata
$ScriptVersion = "1.0.0"
$ScriptName = "MTM Prompt Evolution Initializer"

# Color functions
function Write-Success { param($Message) Write-Host "✅ $Message" -ForegroundColor Green }
function Write-Info { param($Message) Write-Host "ℹ️  $Message" -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host "⚠️  $Message" -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host "❌ $Message" -ForegroundColor Red }

# Header
Clear-Host
Write-Host "🧠 $ScriptName v$ScriptVersion" -ForegroundColor Magenta
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkMagenta
Write-Host ""

# Check PowerShell version
Write-Info "Checking system requirements..."
if ($PSVersionTable.PSVersion.Major -lt 7) {
    Write-Warning "PowerShell 7+ recommended for optimal performance"
    Write-Host "  Current version: $($PSVersionTable.PSVersion)" -ForegroundColor Gray
    Write-Host "  Consider upgrading: winget install Microsoft.PowerShell" -ForegroundColor Gray
}

# Check if running from correct directory
if (-not (Test-Path "MTM_WIP_Application_Avalonia.csproj")) {
    Write-Error "Please run this script from the MTM project root directory"
    Write-Host "Expected to find: MTM_WIP_Application_Avalonia.csproj" -ForegroundColor Gray
    exit 1
}

Write-Success "System requirements validated"

# Create directory structure
Write-Info "Creating directory structure..."

$Directories = @(
    "Documentation/Development/Scripts",
    "Documentation/Development/Reports", 
    "Documentation/Development/Logs",
    "Documentation/Development/Guides",
    ".github/Custom-Prompts",
    ".github/workflows"
)

foreach ($Dir in $Directories) {
    if (-not (Test-Path $Dir)) {
        New-Item -ItemType Directory -Path $Dir -Force | Out-Null
        Write-Host "  📁 Created: $Dir" -ForegroundColor Gray
    } else {
        Write-Host "  📁 Exists: $Dir" -ForegroundColor DarkGray
    }
}

Write-Success "Directory structure created"

# Install PowerShell module
Write-Info "Installing PowerShell module..."

try {
    # Get the current PowerShell module path
    $ModulePath = $env:PSModulePath.Split([IO.Path]::PathSeparator)[0]
    $MTMModulePath = Join-Path $ModulePath "MTM.PromptEvolution"
    
    if (-not (Test-Path $MTMModulePath)) {
        New-Item -ItemType Directory -Path $MTMModulePath -Force | Out-Null
    }
    
    # Copy module file
    $SourceModule = "Documentation/Development/Scripts/MTM.PromptEvolution.psm1"
    $TargetModule = Join-Path $MTMModulePath "MTM.PromptEvolution.psm1"
    
    if (Test-Path $SourceModule) {
        Copy-Item $SourceModule $TargetModule -Force
        Write-Success "PowerShell module installed to $MTMModulePath"
        
        # Create module manifest
        $ManifestPath = Join-Path $MTMModulePath "MTM.PromptEvolution.psd1"
        
        $ManifestContent = @"
@{
    RootModule = 'MTM.PromptEvolution.psm1'
    ModuleVersion = '1.0.0'
    GUID = '$(New-Guid)'
    Author = 'MTM Development Team'
    CompanyName = 'Manitowoc Tool and Manufacturing'
    Copyright = '(c) MTM Development Team. All rights reserved.'
    Description = 'Advanced Custom Prompt Evolution System for MTM GitHub Copilot workflows'
    PowerShellVersion = '5.1'
    
    FunctionsToExport = @(
        'Track-PromptUsage',
        'Get-PromptSuggestion', 
        'Get-AvailablePrompts',
        'Invoke-PromptEvolutionAnalysis',
        'Set-PromptEvolutionAutomation',
        'Set-PromptEvolutionConfig',
        'Get-PromptEvolutionConfig'
    )
    
    AliasesToExport = @('track', 'suggest', 'prompts', 'evolve')
    
    PrivateData = @{
        PSData = @{
            Tags = @('MTM', 'GitHub-Copilot', 'Prompt-Evolution', 'Development')
            ProjectUri = 'https://github.com/Dorotel/MTM_WIP_Application_Avalonia'
            ReleaseNotes = 'Initial release of MTM Prompt Evolution System'
        }
    }
}
"@
        
        $ManifestContent | Out-File -FilePath $ManifestPath -Encoding UTF8
        Write-Success "Module manifest created"
        
        # Test module import
        try {
            Import-Module $TargetModule -Force
            Write-Success "Module imported successfully"
            
            # Show available commands
            Write-Host "  📚 Available commands: " -ForegroundColor Gray -NoNewline
            Write-Host "track, suggest, prompts, evolve" -ForegroundColor Green
        }
        catch {
            Write-Warning "Module import test failed: $_"
        }
    } else {
        Write-Warning "Source module file not found: $SourceModule"
    }
}
catch {
    Write-Error "Failed to install PowerShell module: $_"
}

# Create example prompts if not skipping
if (-not $SkipExamples) {
    Write-Info "Creating example custom prompts..."
    
    $ExamplePrompts = @(
        @{
            Name = "CustomPrompt_Quick_Analysis"
            Content = @'
# Custom Prompt: Quick Analysis

## 🎯 Instructions
Perform a quick analysis of the current file or context and provide actionable insights

## 👤 Persona
**MTM Code Analyst** - Expert in code quality, patterns, and optimization for MTM projects

## 📝 Prompt Template
```
Analyze [file/context] and provide:
1. Code quality assessment
2. Pattern compliance check
3. Optimization opportunities
4. MTM standards adherence
5. Quick improvement suggestions
```

## 🎯 Purpose
Provide rapid feedback on code quality and compliance with MTM standards

## 💡 Usage Examples
```
@prompt:quick-analysis --file="InventoryService.cs"
@prompt:quick-analysis --context="service creation"
```

## 📋 Guidelines
• Focus on MTM-specific patterns and standards
• Provide actionable, specific feedback
• Check for ReactiveUI and Avalonia best practices
• Verify service organization rules
• Validate database access patterns

## 🔗 Related Files
- .github/Core-Instructions/codingconventions.instruction.md
- .github/Quality-Instructions/needsrepair.instruction.md

## ✅ Quality Checklist
- [ ] Follows MTM coding conventions
- [ ] Uses appropriate design patterns
- [ ] Implements proper error handling
- [ ] Follows service organization rules
- [ ] Complies with MTM standards
'@
        },
        @{
            Name = "CustomPrompt_Context_Helper"
            Content = @'
# Custom Prompt: Context Helper

## 🎯 Instructions
Provide contextual help and suggestions based on current development task

## 👤 Persona
**MTM Development Assistant** - Expert guide for MTM development workflows and patterns

## 📝 Prompt Template
```
Based on [current context], provide:
1. Relevant MTM patterns to use
2. Required dependencies and imports
3. Code structure recommendations
4. Testing considerations
5. Documentation requirements
```

## 🎯 Purpose
Guide developers through MTM-specific implementation patterns and best practices

## 💡 Usage Examples
```
@prompt:context-helper --task="creating inventory service"
@prompt:context-helper --file="AdvancedRemoveView.axaml"
```

## 📋 Guidelines
• Provide MTM-specific guidance
• Include code examples when helpful
• Reference relevant instruction files
• Suggest related custom prompts
• Focus on practical implementation steps

## 🔗 Related Files
- .github/copilot-instructions.md
- .github/Custom-Prompts/README.md

## ✅ Quality Checklist
- [ ] Provides actionable guidance
- [ ] References MTM patterns
- [ ] Includes practical examples
- [ ] Suggests best practices
- [ ] Links to relevant resources
'@
        }
    )
    
    foreach ($Prompt in $ExamplePrompts) {
        $PromptPath = ".github/Custom-Prompts/$($Prompt.Name).md"
        if (-not (Test-Path $PromptPath)) {
            $Prompt.Content | Out-File -FilePath $PromptPath -Encoding UTF8
            Write-Host "  📝 Created: $($Prompt.Name)" -ForegroundColor Gray
        }
    }
    
    Write-Success "Example prompts created"
}

# Create usage configuration
Write-Info "Creating configuration files..."

$ConfigContent = @"
# MTM Prompt Evolution Configuration
# This file contains settings for the prompt evolution system

# Tracking Settings
LogPath = "copilot_usage.log"
PromptDirectory = ".github/Custom-Prompts"
ReportsDirectory = "Documentation/Development/Reports"
Enabled = true

# Analysis Settings
AutoGenerate = true
AnalysisSchedule = "Weekly"
HighPriorityThreshold = 0.8

# Integration Settings
GitHubActionsEnabled = true
SlackNotifications = false
TeamsNotifications = false

# Advanced Settings
PatternRecognitionEnabled = true
CrossProjectAnalysis = false
MLOptimizationEnabled = false
"@

$ConfigPath = "Documentation/Development/prompt-evolution.config"
if (-not (Test-Path $ConfigPath)) {
    $ConfigContent | Out-File -FilePath $ConfigPath -Encoding UTF8
    Write-Success "Configuration file created: $ConfigPath"
}

# Full setup including automation
if ($FullSetup) {
    Write-Info "Performing full setup with automation..."
    
    try {
        # Setup weekly automation
        if (Test-Path "Documentation/Development/Scripts/Setup-PromptEvolution.ps1") {
            & "Documentation/Development/Scripts/Setup-PromptEvolution.ps1" -SetupTask -Schedule Weekly
            Write-Success "Automation configured for weekly analysis"
        } else {
            Write-Warning "Setup script not found - automation not configured"
        }
        
        # Run initial analysis
        if (Test-Path "Documentation/Development/Scripts/Analyze-PromptUsage.ps1") {
            Write-Info "Running initial analysis..."
            & "Documentation/Development/Scripts/Analyze-PromptUsage.ps1" -AutoGenerate
            Write-Success "Initial analysis completed"
        } else {
            Write-Warning "Analysis script not found - initial analysis skipped"
        }
    }
    catch {
        Write-Warning "Full setup encountered issues: $_"
    }
}

# Create quick start guide
Write-Info "Creating quick start guide..."

$QuickStartContent = @'
# MTM Prompt Evolution System - Quick Start Guide

## 🚀 Getting Started

### 1. Basic Usage
```powershell
# Import the module (if not auto-loaded)
Import-Module MTM.PromptEvolution

# Track prompt usage
track "CustomPrompt_Create_Service" "UserService.cs" -Success $true

# Get suggestions for current file
suggest -FilePath "Views\InventoryView.axaml"

# List all available prompts
prompts

# Run evolution analysis
evolve
```

### 2. Integration Examples

#### In your development workflow:
```powershell
# When creating a new ViewModel
suggest -ProjectArea "ViewModel"
track "CustomPrompt_Create_ReactiveUIViewModel" "NewViewModel.cs"

# When working on UI
suggest -FilePath "MyView.axaml"
track "CustomPrompt_Create_UIElement" "MyView.axaml" -Success $true
```

#### Automated analysis:
```powershell
# Setup weekly automation
Set-PromptEvolutionAutomation -Schedule Weekly

# Run immediate analysis with report
evolve -AutoGenerate -OpenReport
```

### 3. Configuration

```powershell
# View current configuration
Get-PromptEvolutionConfig

# Update configuration
Set-PromptEvolutionConfig -LogPath "custom_usage.log" -Enabled $true
```

### 4. Available Commands

| Command | Alias | Purpose |
|---------|-------|---------|
| `Track-PromptUsage` | `track` | Record prompt usage |
| `Get-PromptSuggestion` | `suggest` | Get contextual suggestions |
| `Get-AvailablePrompts` | `prompts` | List all prompts |
| `Invoke-PromptEvolutionAnalysis` | `evolve` | Run analysis |

### 5. Next Steps

1. **Start tracking**: Use `track` command when using custom prompts
2. **Regular analysis**: Run `evolve` weekly to see patterns
3. **Review suggestions**: Check generated reports for optimization opportunities
4. **Customize prompts**: Create new prompts based on suggestions
5. **Share insights**: Use reports to improve team prompt usage

## 📚 Additional Resources

- [Advanced Prompt Evolution System Documentation](Advanced-Prompt-Evolution-System.md)
- [Custom Prompts Library](.github/Custom-Prompts/README.md)
- [Integration Guide](Guides/Prompt-Evolution-Integration.md)
- [MTM Development Guidelines](.github/copilot-instructions.md)

## 🔧 Troubleshooting

### Module not loading
```powershell
# Manually import module
Import-Module ".\Documentation\Development\Scripts\MTM.PromptEvolution.psm1" -Force
```

### Tracking not working
```powershell
# Check configuration
Get-PromptEvolutionConfig

# Enable tracking
Set-PromptEvolutionConfig -Enabled $true
```

### Analysis fails
```powershell
# Check PowerShell version
$PSVersionTable.PSVersion

# Run with verbose output
evolve -Verbose
```

---
*Generated by MTM Prompt Evolution System Initializer v1.0.0*
'@

$QuickStartPath = "Documentation/Development/QUICKSTART-PromptEvolution.md"
$QuickStartContent | Out-File -FilePath $QuickStartPath -Encoding UTF8
Write-Success "Quick start guide created: $QuickStartPath"

# Final summary
Write-Host ""
Write-Host "🎉 MTM Prompt Evolution System Setup Complete!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGreen

Write-Host ""
Write-Host "📋 Setup Summary:" -ForegroundColor Cyan
Write-Host "  ✅ Directory structure created" -ForegroundColor White
Write-Host "  ✅ PowerShell module installed" -ForegroundColor White
Write-Host "  ✅ Configuration files created" -ForegroundColor White
if (-not $SkipExamples) {
    Write-Host "  ✅ Example prompts created" -ForegroundColor White
}
if ($FullSetup) {
    Write-Host "  ✅ Automation configured" -ForegroundColor White
    Write-Host "  ✅ Initial analysis completed" -ForegroundColor White
}
Write-Host "  ✅ Quick start guide created" -ForegroundColor White

Write-Host ""
Write-Host "🚀 Next Steps:" -ForegroundColor Yellow
Write-Host "  1. Review the quick start guide: Documentation/Development/QUICKSTART-PromptEvolution.md" -ForegroundColor White
Write-Host "  2. Import the module: Import-Module MTM.PromptEvolution" -ForegroundColor White
Write-Host "  3. Start tracking: track 'PromptName' 'Context.cs'" -ForegroundColor White
Write-Host "  4. Get suggestions: suggest -FilePath 'YourFile.cs'" -ForegroundColor White
Write-Host "  5. Run analysis: evolve -AutoGenerate" -ForegroundColor White

if (-not $FullSetup) {
    Write-Host ""
    Write-Host "💡 Tip: Run with -FullSetup for complete automation configuration" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "📖 Documentation:" -ForegroundColor Magenta
Write-Host "  • Quick Start: Documentation/Development/QUICKSTART-PromptEvolution.md" -ForegroundColor Gray
Write-Host "  • Full Documentation: Documentation/Development/Scripts/Advanced-Prompt-Evolution-System.md" -ForegroundColor Gray
Write-Host "  • Integration Guide: Documentation/Development/Guides/Prompt-Evolution-Integration.md" -ForegroundColor Gray

Write-Host ""
Write-Host "🎯 Happy prompting! The system is ready to learn from your usage patterns." -ForegroundColor Green
