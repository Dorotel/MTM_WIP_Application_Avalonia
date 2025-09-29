#!/usr/bin/env pwsh
#
# Integration Test: Error Recovery and Rollback (Scenario 4)
# Tests GSC workflow error recovery using full reset capabilities
# Based on quickstart.md Scenario 4
#
# Expected: Full workflow reset to beginning of current phase with memory preservation
# Tests error detection, rollback mechanism, and workflow resumption

Param(
    [switch]$Verbose,
    [string]$FeatureName = "Complex manufacturing workflow integration",
    [string]$ErrorType = "memory-corruption"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Test configuration
$TestName = "Error Recovery Workflow"
$RollbackTimeout = 10  # seconds
$RecoveryValidationTime = 5  # seconds

function Test-ErrorRecoveryWorkflow {
    param(
        [string]$FeatureName,
        [string]$ErrorType
    )

    Write-TestHeader $TestName

    # Setup test workspace
    $testWorkspace = New-TestWorkspace -Directory ".specify/test-error-recovery"

    try {
        # Phase 1: Establish successful workflow progression
        Write-TestStep "1. Establish successful workflow progression"

        # Run initial phases successfully
        $constitutionResult = & ".specify/scripts/gsc/gsc-constitution.ps1" $FeatureName
        Assert-Contains -Content $constitutionResult -ExpectedText "Constitution compliance validated" -TestName "Constitution phase success"

        $specifyResult = & ".specify/scripts/gsc/gsc-specify.ps1" "Multi-system integration with real-time sync"
        Assert-Contains -Content $specifyResult -ExpectedText "Specification created" -TestName "Specify phase success"

        $clarifyResult = & ".specify/scripts/gsc/gsc-clarify.ps1"
        Assert-Contains -Content $clarifyResult -ExpectedText "Requirements clarified" -TestName "Clarify phase success"

        # Verify workflow state before error
        $preErrorStatus = & ".specify/scripts/gsc/gsc-status.ps1"
        $preErrorJson = $preErrorStatus | ConvertFrom-Json
        Assert-Equal -Expected "clarify" -Actual $preErrorJson.currentPhase -TestName "Pre-error phase tracking"

        # Phase 2: Simulate error during planning phase
        Write-TestStep "2. Simulate error during plan phase"

        # Set environment to simulate various error types
        switch ($ErrorType) {
            "memory-corruption" {
                $env:GSC_SIMULATE_MEMORY_CORRUPTION = "true"
                $env:GSC_ERROR_MESSAGE = "Memory file integrity check failed"
            }
            "invalid-state" {
                $env:GSC_SIMULATE_INVALID_STATE = "true"
                $env:GSC_ERROR_MESSAGE = "Workflow state file corrupted"
            }
            "resource-exhaustion" {
                $env:GSC_SIMULATE_RESOURCE_ERROR = "true"
                $env:GSC_ERROR_MESSAGE = "Insufficient system resources"
            }
            default {
                $env:GSC_SIMULATE_GENERIC_ERROR = "true"
                $env:GSC_ERROR_MESSAGE = "Generic workflow error"
            }
        }

        # Attempt plan phase - should fail
        try {
            $planResult = & ".specify/scripts/gsc/gsc-plan.ps1"
            # If this succeeds when it should fail, that's an error
            Write-TestError "Plan phase should have failed but succeeded"
            return @{ Success = $false; Error = "Simulated error not triggered" }
        }
        catch {
            # Expected failure - verify error handling
            Assert-Contains -Content $_.Exception.Message -ExpectedText $env:GSC_ERROR_MESSAGE -TestName "Error simulation triggered"
        }

        # Phase 3: Check workflow status shows error
        Write-TestStep "3. Verify error state detection"

        $errorStatus = & ".specify/scripts/gsc/gsc-status.ps1"
        $errorJson = $errorStatus | ConvertFrom-Json

        Assert-Equal -Expected "plan" -Actual $errorJson.currentPhase -TestName "Error phase tracking"
        Assert-Contains -Content $errorStatus -ExpectedText "error state" -TestName "Error state detection"

        # Phase 4: Execute rollback to beginning of current phase
        Write-TestStep "4. Execute rollback to beginning of plan phase"

        $rollbackStart = Get-Date
        $rollbackResult = & ".specify/scripts/gsc/gsc-rollback.ps1"
        $rollbackTime = ((Get-Date) - $rollbackStart).TotalSeconds

        # Validate rollback performance
        Assert-Performance -ActualTime $rollbackTime -MaxTime $RollbackTimeout -Operation "Rollback execution"

        # Verify rollback behavior
        Assert-Contains -Content $rollbackResult -ExpectedText "Reset to beginning of 'plan' phase" -TestName "Phase reset confirmation"
        Assert-Contains -Content $rollbackResult -ExpectedText "Preserve accumulated memory patterns" -TestName "Memory pattern preservation"
        Assert-Contains -Content $rollbackResult -ExpectedText "Clear corrupted state" -TestName "State cleanup confirmation"
        Assert-Contains -Content $rollbackResult -ExpectedText "Restore from last known good checkpoint" -TestName "Checkpoint restoration"

        # Phase 5: Verify clean state after rollback
        Write-TestStep "5. Verify clean state after rollback"

        # Clear error simulation
        Remove-Item Env:GSC_SIMULATE_MEMORY_CORRUPTION -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_INVALID_STATE -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_RESOURCE_ERROR -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_GENERIC_ERROR -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_ERROR_MESSAGE -ErrorAction SilentlyContinue

        $postRollbackStatus = & ".specify/scripts/gsc/gsc-status.ps1"
        $postRollbackJson = $postRollbackStatus | ConvertFrom-Json

        Assert-Equal -Expected "plan" -Actual $postRollbackJson.currentPhase -TestName "Phase remains at plan after rollback"
        Assert-NotContains -Content $postRollbackStatus -ExpectedText "error state" -TestName "Error state cleared"

        # Phase 6: Resume workflow from clean state
        Write-TestStep "6. Resume workflow from clean plan phase"

        $recoveryStart = Get-Date
        $recoveryPlanResult = & ".specify/scripts/gsc/gsc-plan.ps1"
        $recoveryTime = ((Get-Date) - $recoveryStart).TotalSeconds

        # Validate recovery performance
        Assert-Performance -ActualTime $recoveryTime -MaxTime $RecoveryValidationTime -Operation "Recovery plan execution"

        # Verify successful recovery
        Assert-Contains -Content $recoveryPlanResult -ExpectedText "technical plan" -TestName "Recovery plan creation"
        Assert-Contains -Content $recoveryPlanResult -ExpectedText "fresh planning phase" -TestName "Clean state confirmation"
        Assert-NotContains -Content $recoveryPlanResult -ExpectedText "error" -TestName "No error in recovery"

        # Phase 7: Validate memory patterns preserved
        Write-TestStep "7. Validate memory patterns preserved through rollback"

        # Memory patterns should still be available after rollback
        Assert-Contains -Content $recoveryPlanResult -ExpectedText "memory patterns" -TestName "Memory patterns preserved"
        Assert-Contains -Content $recoveryPlanResult -ExpectedText "universal development patterns" -TestName "Specific memory integration"

        # Phase 8: Test continued workflow progression
        Write-TestStep "8. Test continued workflow progression after recovery"

        $taskResult = & ".specify/scripts/gsc/gsc-task.ps1"
        Assert-Contains -Content $taskResult -ExpectedText "task list" -TestName "Post-recovery task generation"

        $finalStatus = & ".specify/scripts/gsc/gsc-status.ps1"
        $finalJson = $finalStatus | ConvertFrom-Json
        Assert-Equal -Expected "task" -Actual $finalJson.currentPhase -TestName "Workflow progression after recovery"

        Write-TestSuccess "Error recovery workflow completed successfully"

        return @{
            Success          = $true
            ErrorType        = $ErrorType
            RecoveryMetrics  = @{
                RollbackTime      = $rollbackTime
                RecoveryTime      = $recoveryTime
                TotalRecoveryTime = $rollbackTime + $recoveryTime
            }
            RecoveryFeatures = @(
                "Error state detection",
                "Phase-specific rollback",
                "Memory pattern preservation",
                "State cleanup",
                "Checkpoint restoration",
                "Workflow resumption"
            )
        }
    }
    catch {
        Write-TestError "Error recovery workflow failed: $($_.Exception.Message)"
        return @{
            Success   = $false
            Error     = $_.Exception.Message
            ErrorType = $ErrorType
        }
    }
    finally {
        # Cleanup environment variables
        Remove-Item Env:GSC_SIMULATE_MEMORY_CORRUPTION -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_INVALID_STATE -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_RESOURCE_ERROR -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SIMULATE_GENERIC_ERROR -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_ERROR_MESSAGE -ErrorAction SilentlyContinue

        # Cleanup test workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly
if ($MyInvocation.InvocationName -ne '.') {
    $result = Test-ErrorRecoveryWorkflow -FeatureName $FeatureName -ErrorType $ErrorType

    if ($result.Success) {
        Write-Host "✅ Integration test passed: Error Recovery Workflow" -ForegroundColor Green
        Write-Host "Recovery metrics: $($result.RecoveryMetrics | ConvertTo-Json -Compress)" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ Integration test failed: Error Recovery Workflow" -ForegroundColor Red
        Write-Host "Error: $($result.Error)" -ForegroundColor Red
        exit 1
    }
}
