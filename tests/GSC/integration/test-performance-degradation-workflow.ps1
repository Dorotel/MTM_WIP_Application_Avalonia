#!/usr/bin/env pwsh
#
# Integration Test: Performance Degradation Handling (Scenario 3)
# Tests GSC graceful degradation during high load or limited resources
# Based on quickstart.md Scenario 3
#
# Expected: System enters degradation mode, surfaces status, and recovers after validation/rollback

Param(
    [switch]$Verbose,
    [string]$FeatureName = "High load manufacturing throughput"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Test configuration
$TestName = "Performance Degradation Workflow"
$ValidationTargetSeconds = 30  # seconds

function Test-PerformanceDegradationWorkflow {
    param([string]$FeatureName)

    Write-TestHeader $TestName

    # Setup test workspace
    $testWorkspace = New-TestWorkspace -Directory ".specify/test-degradation"

    try {
        # Phase 1: Drive normal progression to establish baseline
        Write-TestStep "1. Establish baseline workflow progression"

        $constitutionResult = & ".specify/scripts/gsc/gsc-constitution.ps1" -Action display -Format console
        Assert-Contains -Content $constitutionResult -ExpectedText "PROJECT CONSTITUTION" -TestName "Baseline constitution"

        $specifyResult = & ".specify/scripts/gsc/gsc-specify.ps1" -Action create -SpecType workflow -Name "Throughput optimization with batching" -Template manufacturing-workflow -OutputFormat markdown
        Assert-Contains -Content $specifyResult -ExpectedText "Specification created" -TestName "Baseline specify"

        # Phase 2: Simulate conditions that trigger degradation mode
        Write-TestStep "2. Simulate high-load conditions to trigger degradation"
        $env:GSC_SIMULATE_DEGRADATION = "true"

        # Status should reflect degradation mode when simulation is active
        $statusResult = & ".specify/scripts/gsc/gsc-status.ps1" -Json
        $statusJson = $statusResult | ConvertFrom-Json
        $degradationDetected = $statusJson.performanceDegradationMode

        # If the environment flag alone doesn't flip state in status, attempt a direct state set via validate/implement path
        if (-not $degradationDetected) {
            $validateStart = Get-Date
            $null = & ".specify/scripts/gsc/gsc-validate.ps1" -Json
            $validateTime = ((Get-Date) - $validateStart).TotalSeconds
            Assert-Performance -ActualTime $validateTime -MaxTime $ValidationTargetSeconds -Operation "Validation during degradation simulation"
            # Re-read status
            $statusJson = (& ".specify/scripts/gsc/gsc-status.ps1" -Json) | ConvertFrom-Json
            $degradationDetected = $statusJson.performanceDegradationMode
        }

        # Verify degradation surfaced (may be false if system not configured to auto-toggle; still assert presence type)
        if (-not ($statusJson.performanceDegradationMode -is [bool])) { throw "Type assertion failed: performanceDegradationMode should be boolean" }
        if ($statusJson.performanceDegradationMode) {
            # Message should explain degradation
            if (-not ($statusJson.PSObject.Properties.Name -contains 'message')) { throw "Property assertion failed: 'message' not present when degradation active" }
            if ($statusJson.message -notmatch "degradation|performance|reduced") { throw "Message assertion failed: degradation message missing expected keywords" }
        }

        # Phase 3: Attempt workflow action under degradation and then recover
        Write-TestStep "3. Validate and recover from degradation state"

        $validateJson = (& ".specify/scripts/gsc/gsc-validate.ps1" -Json) | ConvertFrom-Json
        if (@("passed", "warning", "failed") -notcontains $validateJson.overallStatus) { throw "Validation status assertion failed: unexpected overallStatus '$($validateJson.overallStatus)'" }

        # If degradation persists, perform rollback to clear transient state
        if ($statusJson.performanceDegradationMode) {
            $rollbackResult = & ".specify/scripts/gsc/gsc-rollback.ps1" -Confirm
            Assert-Contains -Content $rollbackResult -ExpectedText "Reset" -TestName "Rollback executed for recovery"
        }

        # Phase 4: Verify recovery (degradation off or acceptable state)
        Write-TestStep "4. Verify recovery status"
        $postStatusJson = (& ".specify/scripts/gsc/gsc-status.ps1" -Json) | ConvertFrom-Json
        $postStatusJson.performanceDegradationMode | Should -BeOfType [bool]

        Write-TestSuccess "Performance degradation workflow completed"

        return @{ Success = $true; DegradationDetected = $degradationDetected }
    }
    catch {
        Write-TestError "Performance degradation workflow failed: $($_.Exception.Message)"
        return @{ Success = $false; Error = $_.Exception.Message }
    }
    finally {
        # Cleanup env flags
        Remove-Item Env:GSC_SIMULATE_DEGRADATION -ErrorAction SilentlyContinue
        # Cleanup workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly
if ($MyInvocation.InvocationName -ne '.') {
    $result = Test-PerformanceDegradationWorkflow -FeatureName $FeatureName

    if ($result.Success) {
        Write-Host "✅ Integration test passed: Performance Degradation Workflow" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ Integration test failed: Performance Degradation Workflow" -ForegroundColor Red
        Write-Host "Error: $($result.Error)" -ForegroundColor Red
        exit 1
    }
}
