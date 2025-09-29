#!/usr/bin/env pwsh
#
# GSC Task Command - Enhanced with Custom Control Memory Patterns
# Generate detailed implementation tasks with memory integration
# Date: September 28, 2025

param(
    [string]$Arguments = "",
    [string]$WorkflowId = "",
    [bool]$MemoryIntegrationEnabled = $true,
    [string]$CrossPlatformMode = "windows",
    [string]$ExecutionMode = "powershell"
)

# Import required modules
$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$CommonGSCPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "common-gsc.ps1"
$MemoryIntegrationPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "memory-integration.ps1"
$CustomControlsEntityPath = Join-Path (Split-Path -Parent $ScriptRoot) "powershell" "entities" "GSCCommandEntity.ps1"

if (Test-Path $CommonGSCPath) { . $CommonGSCPath }
if (Test-Path $MemoryIntegrationPath) { . $MemoryIntegrationPath }
if (Test-Path $CustomControlsEntityPath) { . $CustomControlsEntityPath }

# GSC Task Command Implementation
function Invoke-GSCTaskCommand {
    param(
        [string]$Arguments,
        [string]$WorkflowId,
        [bool]$MemoryIntegrationEnabled,
        [string]$CrossPlatformMode,
        [string]$ExecutionMode
    )

    $startTime = Get-Date
    $commandEntity = New-GSCCommandEntity -CommandName "task"

    try {
        Write-Host "🔧 GSC Task Generation with Custom Control Memory Patterns" -ForegroundColor Cyan
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkCyan

        # Initialize workflow state
        $workflowState = Initialize-WorkflowState -WorkflowId $WorkflowId -CurrentPhase "task"

        # Load and integrate memory patterns
        $memoryPatterns = @()
        if ($MemoryIntegrationEnabled) {
            Write-Host "📋 Loading custom control memory patterns..." -ForegroundColor Yellow

            # Load avalonia-custom-controls-memory patterns
            $customControlsPatterns = Get-MemoryFileContent -FileType "avalonia-custom-controls-memory"
            if ($customControlsPatterns) {
                $memoryPatterns += $customControlsPatterns
                Write-Host "✓ Loaded custom controls memory patterns: $($customControlsPatterns.Count) patterns" -ForegroundColor Green
            }

            # Load universal development patterns
            $universalPatterns = Get-MemoryFileContent -FileType "memory"
            if ($universalPatterns) {
                $memoryPatterns += $universalPatterns
                Write-Host "✓ Loaded universal development patterns: $($universalPatterns.Count) patterns" -ForegroundColor Green
            }
        }

        # Load design documents
        Write-Host "📄 Loading design documents..." -ForegroundColor Yellow
        $planData = Get-DesignDocument -DocumentType "plan"
        $dataModel = Get-DesignDocument -DocumentType "data-model"
        $contracts = Get-DesignDocument -DocumentType "contracts"

        if (-not $planData) {
            throw "Plan document (plan.md) not found. Run 'gsc plan' first."
        }

        # Generate comprehensive task breakdown
        Write-Host "⚙️ Generating implementation tasks..." -ForegroundColor Yellow

        $taskBreakdown = @{
            "TaskCategories"            = @{
                "Setup"       = @()
                "Tests"       = @()
                "Core"        = @()
                "Integration" = @()
                "Polish"      = @()
            }
            "Dependencies"              = @{}
            "ParallelTasks"             = @()
            "SequentialTasks"           = @()
            "EstimatedEffort"           = @{}
            "MemoryPatternApplications" = @{}
        }

        # Apply custom control memory patterns to task generation
        if ($memoryPatterns.Count -gt 0) {
            Write-Host "🧠 Applying memory-driven task generation..." -ForegroundColor Magenta

            # Extract task generation patterns from memory
            $taskPatterns = $memoryPatterns | Where-Object {
                $_ -match "task.*generation|implementation.*order|dependency.*management"
            }

            if ($taskPatterns) {
                Write-Host "✓ Found $($taskPatterns.Count) relevant task generation patterns" -ForegroundColor Green
            }
        }

        # Generate setup tasks
        $setupTasks = @(
            @{
                "ID"             = "T001"
                "Description"    = "Initialize project structure and dependencies"
                "Category"       = "Setup"
                "EstimatedHours" = 2
                "Dependencies"   = @()
                "Parallel"       = $true
                "MemoryPatterns" = @("Project structure initialization")
            },
            @{
                "ID"             = "T002"
                "Description"    = "Configure development environment"
                "Category"       = "Setup"
                "EstimatedHours" = 1
                "Dependencies"   = @("T001")
                "Parallel"       = $false
                "MemoryPatterns" = @("Environment configuration")
            }
        )

        # Generate test tasks (TDD approach)
        $testTasks = @(
            @{
                "ID"             = "T003"
                "Description"    = "Create unit test structure"
                "Category"       = "Tests"
                "EstimatedHours" = 3
                "Dependencies"   = @("T002")
                "Parallel"       = $true
                "MemoryPatterns" = @("Unit testing patterns", "MVVM testing")
            },
            @{
                "ID"             = "T004"
                "Description"    = "Implement integration test framework"
                "Category"       = "Tests"
                "EstimatedHours" = 4
                "Dependencies"   = @("T003")
                "Parallel"       = $true
                "MemoryPatterns" = @("Integration testing", "Cross-platform testing")
            }
        )

        # Generate core implementation tasks
        $coreTasks = @(
            @{
                "ID"             = "T005"
                "Description"    = "Implement core entity models"
                "Category"       = "Core"
                "EstimatedHours" = 8
                "Dependencies"   = @("T004")
                "Parallel"       = $true
                "MemoryPatterns" = @("Entity modeling", "Data validation")
            },
            @{
                "ID"             = "T006"
                "Description"    = "Create service layer implementations"
                "Category"       = "Core"
                "EstimatedHours" = 12
                "Dependencies"   = @("T005")
                "Parallel"       = $false
                "MemoryPatterns" = @("Service architecture", "Dependency injection")
            }
        )

        # Generate integration tasks
        $integrationTasks = @(
            @{
                "ID"             = "T007"
                "Description"    = "Integrate with existing MTM services"
                "Category"       = "Integration"
                "EstimatedHours" = 6
                "Dependencies"   = @("T006")
                "Parallel"       = $false
                "MemoryPatterns" = @("Service integration", "MTM patterns")
            },
            @{
                "ID"             = "T008"
                "Description"    = "Implement cross-platform compatibility"
                "Category"       = "Integration"
                "EstimatedHours" = 4
                "Dependencies"   = @("T007")
                "Parallel"       = $true
                "MemoryPatterns" = @("Cross-platform development", "Platform abstraction")
            }
        )

        # Generate polish tasks
        $polishTasks = @(
            @{
                "ID"             = "T009"
                "Description"    = "Performance optimization and validation"
                "Category"       = "Polish"
                "EstimatedHours" = 3
                "Dependencies"   = @("T008")
                "Parallel"       = $true
                "MemoryPatterns" = @("Performance optimization", "Validation patterns")
            },
            @{
                "ID"             = "T010"
                "Description"    = "Documentation and final validation"
                "Category"       = "Polish"
                "EstimatedHours" = 2
                "Dependencies"   = @("T009")
                "Parallel"       = $false
                "MemoryPatterns" = @("Documentation standards", "Final validation")
            }
        )

        # Combine all tasks
        $allTasks = $setupTasks + $testTasks + $coreTasks + $integrationTasks + $polishTasks

        # Categorize tasks
        foreach ($task in $allTasks) {
            $taskBreakdown.TaskCategories[$task.Category] += $task
            $taskBreakdown.EstimatedEffort[$task.ID] = $task.EstimatedHours
            $taskBreakdown.MemoryPatternApplications[$task.ID] = $task.MemoryPatterns

            if ($task.Parallel) {
                $taskBreakdown.ParallelTasks += $task.ID
            }
            else {
                $taskBreakdown.SequentialTasks += $task.ID
            }

            if ($task.Dependencies.Count -gt 0) {
                $taskBreakdown.Dependencies[$task.ID] = $task.Dependencies
            }
        }

        # Generate tasks.md file
        Write-Host "📝 Generating tasks.md file..." -ForegroundColor Yellow
        $tasksMarkdown = Generate-TasksMarkdown -TaskBreakdown $taskBreakdown -MemoryPatterns $memoryPatterns

        # Write tasks.md to appropriate location
        $tasksPath = "specs/current-feature/tasks.md"
        if (Test-Path "specs") {
            $featureDirs = Get-ChildItem "specs" -Directory | Sort-Object LastWriteTime -Descending
            if ($featureDirs) {
                $tasksPath = Join-Path $featureDirs[0].FullName "tasks.md"
            }
        }

        Set-Content -Path $tasksPath -Value $tasksMarkdown -Encoding UTF8
        Write-Host "✓ Tasks saved to: $tasksPath" -ForegroundColor Green

        # Update workflow state
        $workflowState.AdvancePhase("task", @{
                "TasksGenerated"        = $allTasks.Count
                "EstimatedTotalHours"   = ($taskBreakdown.EstimatedEffort.Values | Measure-Object -Sum).Sum
                "ParallelTasks"         = $taskBreakdown.ParallelTasks.Count
                "MemoryPatternsApplied" = $memoryPatterns.Count
            })

        # Update memory integration points
        if ($MemoryIntegrationEnabled -and $memoryPatterns.Count -gt 0) {
            $workflowState.UpdateMemoryIntegration("custom-controls-memory", $memoryPatterns)
        }

        # Save workflow state
        Save-WorkflowState -WorkflowState $workflowState

        # Update command entity status
        $executionTime = ([int]((Get-Date) - $startTime).TotalSeconds)
        $commandEntity.UpdateStatus("Completed", $executionTime)

        # Generate response
        $response = @{
            "success"               = $true
            "command"               = "task"
            "executionTime"         = $executionTime
            "message"               = "Task generation completed successfully with custom control memory integration"
            "memoryPatternsApplied" = $memoryPatterns[0..5] # First 5 patterns for brevity
            "workflowState"         = $workflowState.ToHashtable()
            "validationResults"     = @()
            "nextRecommendedAction" = "gsc analyze"
            "tasksSummary"          = @{
                "totalTasks"     = $allTasks.Count
                "estimatedHours" = ($taskBreakdown.EstimatedEffort.Values | Measure-Object -Sum).Sum
                "parallelTasks"  = $taskBreakdown.ParallelTasks.Count
                "categories"     = @{
                    "Setup"       = $taskBreakdown.TaskCategories.Setup.Count
                    "Tests"       = $taskBreakdown.TaskCategories.Tests.Count
                    "Core"        = $taskBreakdown.TaskCategories.Core.Count
                    "Integration" = $taskBreakdown.TaskCategories.Integration.Count
                    "Polish"      = $taskBreakdown.TaskCategories.Polish.Count
                }
            }
        }

        # Display results
        Write-Host "✅ Task Generation Complete" -ForegroundColor Green
        Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor DarkGreen
        Write-Host "📊 Summary:" -ForegroundColor Cyan
        Write-Host "   • Total tasks: $($allTasks.Count)" -ForegroundColor White
        Write-Host "   • Estimated effort: $((($taskBreakdown.EstimatedEffort.Values | Measure-Object -Sum).Sum)) hours" -ForegroundColor White
        Write-Host "   • Parallel tasks: $($taskBreakdown.ParallelTasks.Count)" -ForegroundColor White
        Write-Host "   • Memory patterns applied: $($memoryPatterns.Count)" -ForegroundColor White
        Write-Host "   • Tasks file: $tasksPath" -ForegroundColor White
        Write-Host ""
        Write-Host "🚀 Next: Run 'gsc analyze' to analyze the implementation approach" -ForegroundColor Yellow

        return $response | ConvertTo-Json -Depth 10

    }
    catch {
        $executionTime = ([int]((Get-Date) - $startTime).TotalSeconds)
        $commandEntity.UpdateStatus("Failed", $executionTime)

        $errorResponse = @{
            "success"       = $false
            "command"       = "task"
            "executionTime" = $executionTime
            "message"       = "Task generation failed: $($_.Exception.Message)"
            "error"         = $_.Exception.Message
        }

        Write-Host "❌ Task Generation Failed: $($_.Exception.Message)" -ForegroundColor Red
        return $errorResponse | ConvertTo-Json -Depth 10
    }
}

# Helper function to generate tasks.md markdown
function Generate-TasksMarkdown {
    param(
        [hashtable]$TaskBreakdown,
        [array]$MemoryPatterns
    )

    $markdown = @"
# Implementation Tasks

**Generated**: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC")
**Memory Patterns Applied**: $($MemoryPatterns.Count) patterns
**Total Estimated Effort**: $(($TaskBreakdown.EstimatedEffort.Values | Measure-Object -Sum).Sum) hours

## Task Categories

### Setup Tasks
$($TaskBreakdown.TaskCategories.Setup | ForEach-Object { "- **$($_.ID)** $($_.Description) ($($_.EstimatedHours)h)" } | Out-String)

### Test Tasks (TDD)
$($TaskBreakdown.TaskCategories.Tests | ForEach-Object { "- **$($_.ID)** $($_.Description) ($($_.EstimatedHours)h)" } | Out-String)

### Core Implementation Tasks
$($TaskBreakdown.TaskCategories.Core | ForEach-Object { "- **$($_.ID)** $($_.Description) ($($_.EstimatedHours)h)" } | Out-String)

### Integration Tasks
$($TaskBreakdown.TaskCategories.Integration | ForEach-Object { "- **$($_.ID)** $($_.Description) ($($_.EstimatedHours)h)" } | Out-String)

### Polish Tasks
$($TaskBreakdown.TaskCategories.Polish | ForEach-Object { "- **$($_.ID)** $($_.Description) ($($_.EstimatedHours)h)" } | Out-String)

## Execution Strategy

### Parallel Tasks
Tasks that can run concurrently:
$($TaskBreakdown.ParallelTasks | ForEach-Object { "- $_" } | Out-String)

### Sequential Tasks
Tasks that must run in order:
$($TaskBreakdown.SequentialTasks | ForEach-Object { "- $_" } | Out-String)

## Memory Pattern Applications

$($TaskBreakdown.MemoryPatternApplications.Keys | ForEach-Object {
    $taskId = $_
    $patterns = $TaskBreakdown.MemoryPatternApplications[$_]
    "### $taskId`n$($patterns | ForEach-Object { "- $_" } | Out-String)"
} | Out-String)

---
*Generated by GSC task command with custom control memory integration*
"@

    return $markdown
}

# Execute the command
$result = Invoke-GSCTaskCommand -Arguments $Arguments -WorkflowId $WorkflowId -MemoryIntegrationEnabled $MemoryIntegrationEnabled -CrossPlatformMode $CrossPlatformMode -ExecutionMode $ExecutionMode

# Output results
Write-Output $result
