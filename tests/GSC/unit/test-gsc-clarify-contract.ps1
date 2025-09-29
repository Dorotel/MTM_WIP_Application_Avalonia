#!/usr/bin/env pwsh
# GSC Clarify Command Contract Tests
# Date: September 28, 2025
# Updated Post-Implementation Validation
# Purpose: Validate GSC Clarify command contract compliance and functionality
# Phase 3.4: Enhanced GSC Command Implementation
# Tasks: T046 - Enhanced gsc-clarify command, T047 - Shell wrapper

#Requires -Version 7.0

# Simple validation tests for GSC Clarify command
Write-Host "[GSC-Clarify-Test] Starting GSC Clarify validation" -ForegroundColor Cyan

$scriptPath = (Resolve-Path "$PSScriptRoot/../../../.specify/scripts/gsc/gsc-clarify.ps1").Path
Write-Host "[GSC-Clarify-Test] Script path: $scriptPath" -ForegroundColor Gray

$testResults = @()

# Test 1: Script exists
if (Test-Path $scriptPath) {
    $testResults += "✅ PASS: GSC Clarify script exists"
}
else {
    $testResults += "❌ FAIL: GSC Clarify script not found"
}

# Test 2: Help command works
try {
    $null = & $scriptPath -Action help -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Help command executes successfully"
    }
    else {
        $testResults += "❌ FAIL: Help command returned non-zero exit code"
    }
}
catch {
    $testResults += "❌ FAIL: Help command threw exception: $($_.Exception.Message)"
}

# Test 3: Patterns command works
try {
    $null = & $scriptPath -Action patterns -OutputFormat console -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Patterns command executes successfully"
    }
    else {
        $testResults += "❌ FAIL: Patterns command returned non-zero exit code"
    }
}
catch {
    $testResults += "❌ FAIL: Patterns command threw exception: $($_.Exception.Message)"
}

# Test 4: Resolve command works
try {
    $null = & $scriptPath -Action resolve -AmbiguityType layout -OutputFormat markdown -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Resolve command executes successfully"
    }
    else {
        $testResults += "❌ FAIL: Resolve command returned non-zero exit code"
    }
}
catch {
    $testResults += "❌ FAIL: Resolve command threw exception: $($_.Exception.Message)"
}

# Test 5: Shell wrapper exists
$shellWrapperPath = (Resolve-Path "$PSScriptRoot/../../../.specify/scripts/gsc/gsc-clarify.sh").Path
if (Test-Path $shellWrapperPath) {
    $testResults += "✅ PASS: Shell wrapper exists"
}
else {
    $testResults += "❌ FAIL: Shell wrapper not found"
}

# Test 6: Memory integration works
try {
    # Capture both stdout and stderr from the command
    $output = & $scriptPath -Action patterns -OutputFormat console -ErrorAction Stop *>&1 | Out-String
    if ($output -match "Memory integration initialized successfully" -or $output -match "Loaded patterns:.*debugging-memory") {
        $testResults += "✅ PASS: Memory integration working"
    }
    else {
        $testResults += "❌ FAIL: Memory integration not working properly"
    }
}
catch {
    $testResults += "❌ FAIL: Memory integration test threw exception: $($_.Exception.Message)"
}

# Display results
Write-Host "`n[GSC-Clarify-Test] Test Results:" -ForegroundColor Yellow
foreach ($result in $testResults) {
    Write-Host "  $result" -ForegroundColor $(if ($result.StartsWith('✅')) { 'Green' } else { 'Red' })
}

# Summary
$passCount = ($testResults | Where-Object { $_.StartsWith('✅') }).Count
$failCount = ($testResults | Where-Object { $_.StartsWith('❌') }).Count

Write-Host "`n[GSC-Clarify-Test] Summary: $passCount passed, $failCount failed" -ForegroundColor $(if ($failCount -eq 0) { 'Green' } else { 'Red' })

if ($failCount -eq 0) {
    Write-Host "[GSC-Clarify-Test] ✅ All tests passed - GSC Clarify command is functional" -ForegroundColor Green
    exit 0
}
else {
    Write-Host "[GSC-Clarify-Test] ❌ Some tests failed - GSC Clarify command has issues" -ForegroundColor Red
    exit 1
}
