#!/usr/bin/env pwsh
#
# Integration Test: New Feature Development Workflow (Scenario 1)
# Tests complete GSC workflow for new Avalonia UI feature with memory integration
# Based on quickstart.md Scenario 1
#
# Expected: All GSC commands execute within performance targets with memory integration
# Performance targets: <85 seconds total, <5s memory file reading, <30s per command

Param(
    [switch]$Verbose,
    [string]$FeatureName = "Test Inventory Management Panel",
    [string]$WorkingDirectory = ".specify/test-workspace"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Test configuration
$TestName = "Feature Development Workflow"
$MaxTotalTime = 85  # seconds
$MaxCommandTime = 30  # seconds per command
$MaxMemoryTime = 5   # seconds for memory operations

function Test-FeatureDevelopmentWorkflow {
    param(
        [string]$FeatureName,
        [string]$WorkingDirectory
    )

    Write-TestHeader $TestName

    # Setup test workspace
    $testWorkspace = New-TestWorkspace -Directory $WorkingDirectory
    $totalStartTime = Get-Date

    try {
        # Test Phase 1: Constitutional validation with memory integration
        Write-TestStep "1. Constitution validation with memory integration"
        $constitutionStart = Get-Date

        $constitutionResult = & ".specify/scripts/gsc/gsc-constitution.ps1" -Action display -Format console
        $constitutionTime = ((Get-Date) - $constitutionStart).TotalSeconds

        # Validate constitution command performance
        Assert-Performance -ActualTime $constitutionTime -MaxTime $MaxCommandTime -Operation "Constitution validation"
        Assert-Contains -Content $constitutionResult -ExpectedText "PROJECT CONSTITUTION" -TestName "Constitution validation result"

        # Test Phase 2: Feature specification with Avalonia UI memory
        Write-TestStep "2. Feature specification with Avalonia UI memory patterns"
        $specifyStart = Get-Date

        $specifyResult = & ".specify/scripts/gsc/gsc-specify.ps1" -Action create -SpecType ui-component -Name "Add real-time inventory status panel with drag-drop functionality" -Template avalonia-usercontrol -OutputFormat markdown -MemoryIntegration
        $specifyTime = ((Get-Date) - $specifyStart).TotalSeconds

        # Validate specify command performance
        Assert-Performance -ActualTime $specifyTime -MaxTime $MaxCommandTime -Operation "Feature specification"
        Assert-Contains -Content $specifyResult -ExpectedText "Specification created" -TestName "Specification creation result"

        # Test Phase 3: Requirements clarification with debugging memory
        Write-TestStep "3. Requirements clarification with debugging workflows"
        $clarifyStart = Get-Date

        $clarifyResult = & ".specify/scripts/gsc/gsc-clarify.ps1"
        $clarifyTime = ((Get-Date) - $clarifyStart).TotalSeconds

        # Validate clarify command performance
        Assert-Performance -ActualTime $clarifyTime -MaxTime $MaxCommandTime -Operation "Requirements clarification"
        Assert-Contains -Content $clarifyResult -ExpectedText "Requirements clarified" -TestName "Clarification completion"
        Assert-Contains -Content $clarifyResult -ExpectedText "systematic problem-solving" -TestName "Debugging memory integration"

        # Test Phase 4: Implementation planning with universal patterns
        Write-TestStep "4. Implementation planning with universal patterns"
        $planStart = Get-Date

        $planResult = & ".specify/scripts/gsc/gsc-plan.ps1"
        $planTime = ((Get-Date) - $planStart).TotalSeconds

        # Validate plan command performance
        Assert-Performance -ActualTime $planTime -MaxTime $MaxCommandTime -Operation "Implementation planning"
        Assert-Contains -Content $planResult -ExpectedText "technical plan" -TestName "Plan creation result"
        Assert-Contains -Content $planResult -ExpectedText "memory-driven architecture" -TestName "Universal patterns integration"

        # Test Phase 5: Task generation with custom control memory
        Write-TestStep "5. Task generation with custom control patterns"
        $taskStart = Get-Date

        $taskResult = & ".specify/scripts/gsc/gsc-task.ps1"
        $taskTime = ((Get-Date) - $taskStart).TotalSeconds

        # Validate task command performance
        Assert-Performance -ActualTime $taskTime -MaxTime $MaxCommandTime -Operation "Task generation"
        Assert-Contains -Content $taskResult -ExpectedText "task list" -TestName "Task generation result"
        Assert-Contains -Content $taskResult -ExpectedText "custom control patterns" -TestName "Custom control memory integration"

        # Test Phase 6: Implementation analysis
        Write-TestStep "6. Implementation analysis with systematic debugging"
        $analyzeStart = Get-Date

        $analyzeResult = & ".specify/scripts/gsc/gsc-analyze.ps1"
        $analyzeTime = ((Get-Date) - $analyzeStart).TotalSeconds

        # Validate analyze command performance
        Assert-Performance -ActualTime $analyzeTime -MaxTime $MaxCommandTime -Operation "Implementation analysis"
        Assert-Contains -Content $analyzeResult -ExpectedText "code quality analysis" -TestName "Analysis completion"
        Assert-Contains -Content $analyzeResult -ExpectedText "memory-driven recommendations" -TestName "Memory-driven analysis"

        # Test Phase 7: Feature implementation
        Write-TestStep "7. Feature implementation with all memory patterns"
        $implementStart = Get-Date

        $implementResult = & ".specify/scripts/gsc/gsc-implement.ps1"
        $implementTime = ((Get-Date) - $implementStart).TotalSeconds

        # Validate implement command performance
        Assert-Performance -ActualTime $implementTime -MaxTime $MaxCommandTime -Operation "Feature implementation"
        Assert-Contains -Content $implementResult -ExpectedText "code generation" -TestName "Implementation completion"
        Assert-Contains -Content $implementResult -ExpectedText "memory patterns" -TestName "All memory patterns integration"

        # Validate total workflow time
        $totalTime = ((Get-Date) - $totalStartTime).TotalSeconds
        Assert-Performance -ActualTime $totalTime -MaxTime $MaxTotalTime -Operation "Complete workflow"

        # Verify workflow state persistence
        $workflowState = Get-Content ".specify/state/gsc-workflow.json" | ConvertFrom-Json
        Assert-NotNull -Value $workflowState.workflowId -TestName "Workflow ID persistence"
        Assert-Equal -Expected "implement" -Actual $workflowState.currentPhase -TestName "Current phase tracking"

        Write-TestSuccess "Feature development workflow completed successfully"
        Write-TestResult "Total execution time: $($totalTime.ToString('F2')) seconds (target: <$MaxTotalTime seconds)"

        return @{
            Success      = $true
            TotalTime    = $totalTime
            PhaseResults = @{
                Constitution = @{ Time = $constitutionTime; Result = $constitutionResult }
                Specify      = @{ Time = $specifyTime; Result = $specifyResult }
                Clarify      = @{ Time = $clarifyTime; Result = $clarifyResult }
                Plan         = @{ Time = $planTime; Result = $planResult }
                Task         = @{ Time = $taskTime; Result = $taskResult }
                Analyze      = @{ Time = $analyzeTime; Result = $analyzeResult }
                Implement    = @{ Time = $implementTime; Result = $implementResult }
            }
        }
    }
    catch {
        Write-TestError "Feature development workflow failed: $($_.Exception.Message)"
        return @{
            Success     = $false
            Error       = $_.Exception.Message
            FailedPhase = $currentPhase
        }
    }
    finally {
        # Cleanup test workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly
if ($MyInvocation.InvocationName -ne '.') {
    $result = Test-FeatureDevelopmentWorkflow -FeatureName $FeatureName -WorkingDirectory $WorkingDirectory

    if ($result.Success) {
        Write-Host "✅ Integration test passed: Feature Development Workflow" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ Integration test failed: Feature Development Workflow" -ForegroundColor Red
        Write-Host "Error: $($result.Error)" -ForegroundColor Red
        exit 1
    }
}
