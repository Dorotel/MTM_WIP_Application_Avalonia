param(
    [ValidateSet("Daily", "Weekly", "Monthly")]
    [string]$Schedule = "Weekly",
    [switch]$SetupTask,
    [switch]$RunOnce,
    [string]$LogPath = "Documentation/Development/Logs"
)

# Setup logging
if (-not (Test-Path $LogPath)) {
    New-Item -ItemType Directory -Path $LogPath -Force | Out-Null
}

$LogFile = Join-Path $LogPath "prompt-evolution-$(Get-Date -Format 'yyyy-MM-dd').log"

function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $Timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $LogEntry = "[$Timestamp] [$Level] $Message"
    Add-Content -Path $LogFile -Value $LogEntry
    Write-Host $LogEntry -ForegroundColor $(switch($Level) { "ERROR" { "Red" } "WARN" { "Yellow" } "SUCCESS" { "Green" } default { "White" } })
}

Write-Log "Starting MTM Prompt Evolution Automation System" "INFO"

# Check for required PowerShell version
if ($PSVersionTable.PSVersion.Major -lt 7) {
    Write-Log "PowerShell 7+ recommended for optimal performance. Current version: $($PSVersionTable.PSVersion)" "WARN"
}

# Function to run the analysis
function Invoke-PromptAnalysis {
    param([bool]$AutoGenerate = $true)
    
    Write-Log "Starting prompt usage analysis..." "INFO"
    
    try {
        $AnalysisScript = "Documentation\Development\Scripts\Analyze-PromptUsage.ps1"
        
        if (-not (Test-Path $AnalysisScript)) {
            Write-Log "Analysis script not found: $AnalysisScript" "ERROR"
            return $false
        }
        
        $Parameters = @{
            AutoGenerate = $AutoGenerate
            LogPath = "copilot_usage_$(Get-Date -Format 'yyyy-MM-dd').log"
            PromptDirectory = ".github\Custom-Prompts"
            OutputReport = "Documentation\Development\Reports\prompt_evolution_report_$(Get-Date -Format 'yyyy-MM-dd_HH-mm').html"
        }
        
        Write-Log "Running analysis with parameters: $($Parameters | ConvertTo-Json -Compress)" "INFO"
        
        & $AnalysisScript @Parameters
        
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Prompt analysis completed successfully" "SUCCESS"
            return $true
        } else {
            Write-Log "Prompt analysis failed with exit code: $LASTEXITCODE" "ERROR"
            return $false
        }
    }
    catch {
        Write-Log "Error during prompt analysis: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Function to setup Windows Task Scheduler
function Set-ScheduledTask {
    param([string]$Schedule)
    
    Write-Log "Setting up scheduled task for $Schedule analysis..." "INFO"
    
    try {
        $TaskName = "MTM-PromptEvolution-$Schedule"
        $ScriptPath = (Resolve-Path $PSCommandPath).Path
        $WorkingDirectory = (Get-Location).Path
        
        # Remove existing task if it exists
        if (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue) {
            Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
            Write-Log "Removed existing scheduled task: $TaskName" "INFO"
        }
        
        # Create trigger based on schedule
        $Trigger = switch ($Schedule) {
            "Daily" { New-ScheduledTaskTrigger -Daily -At "02:00" }
            "Weekly" { New-ScheduledTaskTrigger -Weekly -DaysOfWeek Monday -At "02:00" }
            "Monthly" { New-ScheduledTaskTrigger -Weekly -WeeksInterval 4 -DaysOfWeek Monday -At "02:00" }
        }
        
        # Create action
        $Action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File `"$ScriptPath`" -RunOnce" -WorkingDirectory $WorkingDirectory
        
        # Create settings
        $Settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries -StartWhenAvailable
        
        # Register task
        Register-ScheduledTask -TaskName $TaskName -Trigger $Trigger -Action $Action -Settings $Settings -Description "MTM Custom Prompt Evolution Analysis - $Schedule"
        
        Write-Log "Scheduled task created successfully: $TaskName" "SUCCESS"
        Write-Log "Next run time: $((Get-ScheduledTask -TaskName $TaskName).Triggers[0].StartBoundary)" "INFO"
        
        return $true
    }
    catch {
        Write-Log "Failed to create scheduled task: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Function to create GitHub Actions workflow
function New-GitHubWorkflow {
    Write-Log "Creating GitHub Actions workflow for prompt evolution..." "INFO"
    
    $WorkflowPath = ".github\workflows\prompt-evolution.yml"
    $WorkflowDir = Split-Path $WorkflowPath -Parent
    
    if (-not (Test-Path $WorkflowDir)) {
        New-Item -ItemType Directory -Path $WorkflowDir -Force | Out-Null
    }
    
    $WorkflowContent = @"
name: MTM Prompt Evolution Analysis

on:
  schedule:
    # Run every Monday at 2 AM UTC
    - cron: '0 2 * * 1'
  workflow_dispatch:
    inputs:
      auto_generate:
        description: 'Auto-generate high priority prompts'
        required: false
        default: 'true'
        type: boolean

jobs:
  analyze-prompts:
    runs-on: windows-latest
    
    steps:
    - name: Checkout repository
      uses: actions/checkout@v4
      with:
        token: `${{ secrets.GITHUB_TOKEN }}
        fetch-depth: 0
    
    - name: Setup PowerShell
      uses: azure/powershell@v1
      with:
        azPSVersion: 'latest'
        inlineScript: |
          Write-Host "PowerShell version: `$(`$PSVersionTable.PSVersion)"
    
    - name: Run Prompt Evolution Analysis
      shell: pwsh
      run: |
        `$AutoGenerate = `${{ github.event.inputs.auto_generate || 'true' }}
        .\Documentation\Development\Scripts\Analyze-PromptUsage.ps1 -AutoGenerate:`$([bool]::Parse(`$AutoGenerate))
        
    - name: Upload Analysis Report
      uses: actions/upload-artifact@v3
      with:
        name: prompt-evolution-report
        path: Documentation/Development/Reports/prompt_evolution_report_*.html
        retention-days: 30
    
    - name: Commit Generated Prompts
      if: `${{ github.event.inputs.auto_generate == 'true' || github.event_name == 'schedule' }}
      run: |
        git config --local user.email "action@github.com"
        git config --local user.name "GitHub Action"
        git add .github/Custom-Prompts/
        git diff --staged --quiet || git commit -m "🤖 Auto-generated custom prompts from evolution analysis"
        git push
    
    - name: Create Issue for High Priority Suggestions
      if: `${{ always() }}
      uses: actions/github-script@v6
      with:
        script: |
          const fs = require('fs');
          const path = require('path');
          
          // Check if there are high priority suggestions
          const reportFiles = fs.readdirSync('Documentation/Development/Reports/')
            .filter(file => file.includes('prompt_evolution_report_'))
            .sort()
            .reverse();
          
          if (reportFiles.length > 0) {
            const reportContent = fs.readFileSync(
              path.join('Documentation/Development/Reports/', reportFiles[0]), 
              'utf8'
            );
            
            // Extract high priority count (simplified - in real implementation, parse HTML or use JSON)
            const highPriorityMatch = reportContent.match(/High Priority<\/div>\s*<\/div>\s*<div class="stat-card">\s*<div class="stat-number">(\d+)/);
            
            if (highPriorityMatch && parseInt(highPriorityMatch[1]) > 0) {
              const issueBody = \`
              ## 🧠 Custom Prompt Evolution Analysis Results
              
              The automated prompt evolution system has identified **`${highPriorityMatch[1]}`** high-priority prompt suggestions for improvement.
              
              ### 📊 Analysis Summary
              - **Analysis Date:** `${new Date().toISOString().split('T')[0]}`
              - **High Priority Suggestions:** `${highPriorityMatch[1]}`
              - **Report:** [View Full Report](../blob/main/Documentation/Development/Reports/`${reportFiles[0]}`)
              
              ### 🎯 Next Steps
              1. Review the generated analysis report
              2. Implement high-priority prompt suggestions
              3. Test new prompts with your development workflow
              4. Monitor success rates and usage patterns
              
              ### 🔗 Related Resources
              - [Custom Prompts Directory](../tree/main/.github/Custom-Prompts)
              - [Prompt Evolution System Documentation](../blob/main/Documentation/Development/Scripts/Advanced-Prompt-Evolution-System.md)
              
              ---
              *This issue was automatically created by the MTM Prompt Evolution System*
              \`;
              
              github.rest.issues.create({
                owner: context.repo.owner,
                repo: context.repo.repo,
                title: \`🤖 Prompt Evolution Analysis - `${new Date().toISOString().split('T')[0]}` (`${highPriorityMatch[1]}` High Priority Suggestions)\`,
                body: issueBody,
                labels: ['automation', 'custom-prompts', 'enhancement']
              });
            }
          }

permissions:
  contents: write
  issues: write
  pull-requests: write
"@

    try {
        $WorkflowContent | Out-File -FilePath $WorkflowPath -Encoding UTF8 -Force
        Write-Log "GitHub Actions workflow created: $WorkflowPath" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Failed to create GitHub Actions workflow: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Function to create integration documentation
function New-IntegrationDocumentation {
    Write-Log "Creating integration documentation..." "INFO"
    
    $DocPath = "Documentation\Development\Guides\Prompt-Evolution-Integration.md"
    $DocDir = Split-Path $DocPath -Parent
    
    if (-not (Test-Path $DocDir)) {
        New-Item -ItemType Directory -Path $DocDir -Force | Out-Null
    }
    
    $DocContent = @"
# MTM Custom Prompt Evolution System Integration Guide

## Overview
The Advanced Custom Prompt Evolution System intelligently analyzes your GitHub Copilot usage patterns and automatically evolves your custom prompts for maximum effectiveness.

## Features
- **📊 Usage Pattern Analysis** - Track which prompts work best
- **🤖 Automatic Prompt Generation** - Create new prompts for recurring tasks
- **⚡ Prompt Optimization** - Improve existing prompts based on data
- **📈 Success Rate Monitoring** - Measure prompt effectiveness
- **🔄 Workflow Integration** - Seamless development workflow integration

## Quick Start

### 1. Manual Analysis
``````powershell
# Run immediate analysis
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1

# Auto-generate high-priority prompts
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1 -AutoGenerate
``````

### 2. Setup Automation
``````powershell
# Setup weekly scheduled analysis (Windows)
.\Documentation\Development\Scripts\Setup-PromptEvolution.ps1 -SetupTask -Schedule Weekly

# Setup daily analysis
.\Documentation\Development\Scripts\Setup-PromptEvolution.ps1 -SetupTask -Schedule Daily
``````

### 3. GitHub Actions Integration
The system automatically creates a GitHub Actions workflow for:
- Weekly analysis runs
- Automatic prompt generation
- Report artifact storage
- Issue creation for high-priority suggestions

## System Components

### Analysis Engine (\`Analyze-PromptUsage.ps1\`)
- Scans workspace for usage patterns
- Analyzes file type distributions
- Identifies missing prompt categories
- Generates optimization suggestions

### Automation Framework (\`Setup-PromptEvolution.ps1\`)
- Windows Task Scheduler integration
- GitHub Actions workflow creation
- Automated report generation
- Issue tracking integration

### Intelligence Classes
- **PromptUsage**: Tracks individual prompt performance
- **PromptSuggestion**: Generates new prompt recommendations
- **Pattern Analysis**: Identifies workflow gaps and opportunities

## Configuration Options

### Analysis Parameters
``````powershell
param(
    [string]$LogPath = "copilot_usage.log",           # Usage log location
    [string]$PromptDirectory = ".github/Custom-Prompts", # Prompt storage
    [string]$OutputReport = "prompt_evolution_report.html", # Report output
    [switch]$AutoGenerate = $true                     # Auto-create prompts
)
``````

### Schedule Options
- **Daily**: High-frequency development teams
- **Weekly**: Standard development cycles (recommended)
- **Monthly**: Low-frequency or maintenance projects

## Integration with MTM Workflow

### 1. Custom Prompt Categories
The system recognizes MTM-specific patterns:
- **UI Components** (.axaml files) → UI generation prompts
- **ViewModels** (ReactiveUI patterns) → ViewModel creation prompts
- **Services** (dependency injection) → Service development prompts
- **Database** (stored procedures) → Database operation prompts

### 2. MTM-Specific Optimizations
- Service organization rules (multiple services per file)
- ReactiveUI and Avalonia patterns
- Purple theme and design system integration
- Database access pattern enforcement

### 3. Workflow Integration Points
``````powershell
# Integrate with daily development
@prompt:analyze-workspace    # Quick workspace analysis
@prompt:suggest-improvements # Get optimization suggestions
@prompt:generate-missing     # Create missing prompts
``````

## Usage Scenarios

### Scenario 1: New Team Member Onboarding
``````powershell
# Generate comprehensive prompt set
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1 -AutoGenerate

# Review onboarding prompts
Get-ChildItem .github\Custom-Prompts\ | Where-Object { $_.Name -like "*Onboarding*" }
``````

### Scenario 2: Project Phase Transition
``````powershell
# Analyze current phase patterns
.\Documentation\Development\Scripts\Analyze-PromptUsage.ps1

# Generate phase-specific prompts
# (System automatically detects phase based on file patterns)
``````

### Scenario 3: Performance Optimization
``````powershell
# Weekly optimization run
.\Documentation\Development\Scripts\Setup-PromptEvolution.ps1 -RunOnce

# Review optimization suggestions in generated report
``````

## Monitoring and Metrics

### Key Performance Indicators
- **Prompt Usage Frequency** - How often prompts are used
- **Success Rates** - Effectiveness of generated code
- **Modification Patterns** - How users adapt prompts
- **Context Effectiveness** - Which contexts work best

### Reports and Analytics
- **HTML Dashboard** - Interactive usage analytics
- **Trend Analysis** - Usage patterns over time
- **Suggestion Pipeline** - New prompt recommendations
- **Optimization Opportunities** - Improvement suggestions

## Troubleshooting

### Common Issues

#### 1. PowerShell Version
``````powershell
# Check PowerShell version
$PSVersionTable.PSVersion

# Install PowerShell 7+ for optimal performance
winget install Microsoft.PowerShell
``````

#### 2. Permissions
``````powershell
# Set execution policy for scripts
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
``````

#### 3. Missing Dependencies
``````powershell
# Verify required modules
Get-Module -ListAvailable | Where-Object { $_.Name -like "*ScheduledTasks*" }
``````

### Log Analysis
``````powershell
# Check system logs
Get-Content "Documentation\Development\Logs\prompt-evolution-$(Get-Date -Format 'yyyy-MM-dd').log"

# Monitor real-time logs
Get-Content "Documentation\Development\Logs\prompt-evolution-$(Get-Date -Format 'yyyy-MM-dd').log" -Wait
``````

## Advanced Configuration

### Custom Pattern Recognition
``````powershell
# Add custom file patterns
$CustomPatterns = @{
    "Configuration" = @{ Extensions = @(".json", ".xml"); MinCount = 2 }
    "Testing" = @{ Extensions = @(".test.cs", ".spec.cs"); MinCount = 3 }
}
``````

### Integration with External Tools
- **GitHub Copilot Chat** - Automatic suggestion integration
- **Visual Studio Code** - Extension development
- **Azure DevOps** - Pipeline integration
- **Slack/Teams** - Notification workflows

## Best Practices

### 1. Regular Analysis
- Run weekly analysis during low-activity periods
- Review reports within 24 hours of generation
- Implement high-priority suggestions immediately

### 2. Team Collaboration
- Share evolution reports with development team
- Collaborate on prompt optimization
- Create team-specific prompt libraries

### 3. Continuous Improvement
- Monitor success rates and adjust thresholds
- Evolve pattern recognition based on project changes
- Regular system updates and improvements

## Future Enhancements

### Planned Features
- **AI-Powered Optimization** - Machine learning for prompt improvement
- **Cross-Project Analysis** - Multi-repository pattern recognition
- **Real-Time Suggestions** - Live prompt recommendations
- **Team Analytics** - Collaborative usage insights

### Integration Roadmap
- **GitHub Copilot Extension** - Native integration
- **VS Code Extension** - IDE-level automation
- **Azure AI Integration** - Enhanced pattern recognition
- **Enterprise Dashboard** - Organization-wide analytics

## Support and Resources

### Documentation
- [Advanced Prompt Evolution System](Advanced-Prompt-Evolution-System.md)
- [Custom Prompts Library](.github/Custom-Prompts/README.md)
- [MTM Development Guidelines](.github/copilot-instructions.md)

### Community
- [GitHub Discussions](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/discussions)
- [Issue Tracking](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/issues)
- [Development Blog](Documentation/Development/Blog/)

---

*This integration guide is automatically maintained by the MTM Prompt Evolution System*
"@

    try {
        $DocContent | Out-File -FilePath $DocPath -Encoding UTF8 -Force
        Write-Log "Integration documentation created: $DocPath" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Failed to create integration documentation: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Main execution logic
try {
    if ($SetupTask) {
        Write-Log "Setting up scheduled task automation..." "INFO"
        
        # Create scheduled task
        $TaskSuccess = Set-ScheduledTask -Schedule $Schedule
        
        # Create GitHub Actions workflow
        $WorkflowSuccess = New-GitHubWorkflow
        
        # Create integration documentation
        $DocSuccess = New-IntegrationDocumentation
        
        if ($TaskSuccess -and $WorkflowSuccess -and $DocSuccess) {
            Write-Log "MTM Prompt Evolution System setup completed successfully!" "SUCCESS"
            Write-Log "Scheduled for: $Schedule analysis" "INFO"
            Write-Log "GitHub Actions workflow created for automated analysis" "INFO"
            Write-Log "Integration documentation available at: Documentation\Development\Guides\Prompt-Evolution-Integration.md" "INFO"
        } else {
            Write-Log "Setup completed with some issues. Check logs for details." "WARN"
        }
    }
    
    if ($RunOnce) {
        Write-Log "Running one-time prompt evolution analysis..." "INFO"
        $AnalysisSuccess = Invoke-PromptAnalysis -AutoGenerate $true
        
        if ($AnalysisSuccess) {
            Write-Log "One-time analysis completed successfully!" "SUCCESS"
        } else {
            Write-Log "One-time analysis failed. Check logs for details." "ERROR"
        }
    }
    
    if (-not $SetupTask -and -not $RunOnce) {
        Write-Log "No action specified. Use -SetupTask to setup automation or -RunOnce for immediate analysis." "WARN"
        Write-Host "`nUsage Examples:" -ForegroundColor Yellow
        Write-Host "  Setup weekly automation:  .\Setup-PromptEvolution.ps1 -SetupTask -Schedule Weekly" -ForegroundColor White
        Write-Host "  Run immediate analysis:   .\Setup-PromptEvolution.ps1 -RunOnce" -ForegroundColor White
        Write-Host "  Setup daily automation:   .\Setup-PromptEvolution.ps1 -SetupTask -Schedule Daily" -ForegroundColor White
    }
}
catch {
    Write-Log "Critical error in setup process: $($_.Exception.Message)" "ERROR"
    Write-Log "Stack trace: $($_.ScriptStackTrace)" "ERROR"
    exit 1
}

Write-Log "MTM Prompt Evolution automation script completed" "INFO"
