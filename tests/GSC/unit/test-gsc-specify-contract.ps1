#!/usr/bin/env pwsh
# GSC Specify Command Contract Tests
# Date: September 28, 2025
# Updated Post-Implementation Validation
# Purpose: Validate GSC Specify command contract compliance and functionality
# Phase 3.4: Enhanced GSC Command Implementation

#Requires -Version 7.0

# Simple validation tests for GSC Specify command
Write-Host "[GSC-Specify-Test] Starting GSC Specify validation" -ForegroundColor Cyan

$scriptPath = (Resolve-Path "$PSScriptRoot/../../../.specify/scripts/gsc/gsc-specify.ps1").Path
Write-Host "[GSC-Specify-Test] Script path: $scriptPath" -ForegroundColor Gray

$testResults = @()

# Test 1: Script exists
if (Test-Path $scriptPath) {
    $testResults += "✅ PASS: GSC Specify script exists"
} else {
    $testResults += "❌ FAIL: GSC Specify script not found"
}

# Test 2: Help command works
try {
    $null = & $scriptPath -Action help -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Help command executes successfully"
    } else {
        $testResults += "❌ FAIL: Help command returned non-zero exit code"
    }
} catch {
    $testResults += "❌ FAIL: Help command threw exception: $($_.Exception.Message)"
}

# Test 3: Create command works
try {
    $null = & $scriptPath -Action create -SpecType ui-component -Name "TestComponent" -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Create command executes successfully"
    } else {
        $testResults += "❌ FAIL: Create command returned non-zero exit code"
    }
} catch {
    $testResults += "❌ FAIL: Create command threw exception: $($_.Exception.Message)"
}

# Test 4: Patterns command works
try {
    $null = & $scriptPath -Action patterns -SpecType ui-component -ErrorAction Stop
    if ($LASTEXITCODE -eq 0) {
        $testResults += "✅ PASS: Patterns command executes successfully"
    } else {
        $testResults += "❌ FAIL: Patterns command returned non-zero exit code"
    }
} catch {
    $testResults += "❌ FAIL: Patterns command threw exception: $($_.Exception.Message)"
}

Write-Host "`n[GSC-Specify-Test] Test Results:" -ForegroundColor Yellow
foreach ($result in $testResults) {
    Write-Host $result
}

$passCount = ($testResults | Where-Object { $_ -match "✅ PASS" }).Count
$failCount = ($testResults | Where-Object { $_ -match "❌ FAIL" }).Count

Write-Host "`n[GSC-Specify-Test] Summary: $passCount passed, $failCount failed" -ForegroundColor $(if ($failCount -eq 0) { 'Green' } else { 'Red' })

if ($failCount -eq 0) {
    Write-Host "[GSC-Specify-Test] ✅ All tests passed - GSC Specify command is functional" -ForegroundColor Green
    exit 0
} else {
    Write-Host "[GSC-Specify-Test] ❌ Some tests failed - GSC Specify command has issues" -ForegroundColor Red
    exit 1
}

Describe "GSC Specify Command Contract Tests" {
    Context "T044: Enhanced gsc-specify Command Implementation" {

        It "Should exist and be executable" {
            Test-Path $global:TestConfig.ScriptPath | Should -Be $true

            # Test basic help execution
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action help 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $global:TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout $global:TestConfig.MaxExecutionTime
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job -Force

            $result.Success | Should -Be $true
        }

        It "Should generate valid specification for ui-component" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -SpecType ui-component -Name "InventoryControl" -Template avalonia-usercontrol 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $global:TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 20
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match "InventoryControl"
            $result.Output | Should -Match "Avalonia UserControl"
            $result.Output | Should -Match "AXAML Structure"
        }

        It "Should perform validation with scoring" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action validate -SpecType viewmodel -Name "TestViewModel" 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $global:TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 15
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match "Score:"
            $result.Output | Should -Match "Validation Results:"
        }

        It "Should support memory pattern integration" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -Name "TestControl" -MemoryIntegration 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $global:TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 20
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job -Force

            $result.Success | Should -Be $true
            $result.Output | Should -Match "Loading memory instruction patterns"
            $result.Output | Should -Match "MemoryPatternsApplied:"
        }

        It "Should generate MVVM ViewModel specifications" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -SpecType viewmodel -Name "InventoryViewModel" -Template mvvm-viewmodel 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 15
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match "InventoryViewModel"
            $result.Output | Should -Match "MVVM Community Toolkit"
            $result.Output | Should -Match "ObservableObject"
        }

        It "Should support multiple output formats" {
            foreach ($format in @('markdown', 'json', 'yaml')) {
                $job = Start-Job -ScriptBlock {
                    param($ScriptPath, $Format)
                    try {
                        $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -Name "FormatTest" -OutputFormat $Format 2>&1
                        $success = $LASTEXITCODE -eq 0
                        return @{ Success = $success; Output = ($output -join "`n") }
                    } catch {
                        return @{ Success = $false; Error = $_.Exception.Message }
                    }
                } -ArgumentList $global:TestConfig.ScriptPath, $format

                $completed = Wait-Job $job -Timeout 15
                $completed | Should -Not -BeNullOrEmpty

                $result = Receive-Job $job
                Remove-Job $job -Force

                $result.Success | Should -Be $true
                $result.Output | Should -Match "FormatTest"
            }
        }

        It "Should provide pattern display functionality" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action patterns 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 10
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match "Memory Patterns"
        }
    }

    Context "Avalonia UI Integration Validation" {
        It "Should generate Avalonia-specific AXAML structure" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -SpecType ui-component -Template avalonia-usercontrol -Name "TestControl" 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 15
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match 'xmlns="https://github.com/avaloniaui"'
            $result.Output | Should -Match "x:Class"
            $result.Output | Should -Match "UserControl"
        }

        It "Should include Theme V2 semantic tokens" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -SpecType ui-component -Name "ThemedControl" -MemoryIntegration 2>&1
                    $success = $LASTEXITCODE -eq 0
                    return @{ Success = $success; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 15
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            $result.Success | Should -Be $true
            $result.Output | Should -Match "DynamicResource"
            $result.Output | Should -Match "Theme"
        }
    }

    Context "Performance and Error Handling" {
        It "Should complete within timeout limits" {
            $startTime = Get-Date

            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action create -Name "PerfTest" 2>&1
                    return @{ Success = $true; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout $TestConfig.MaxExecutionTime
            $completed | Should -Not -BeNullOrEmpty

            $executionTime = ((Get-Date) - $startTime).TotalSeconds
            $executionTime | Should -BeLessOrEqual $TestConfig.MaxExecutionTime

            Remove-Job $job
        }

        It "Should handle invalid actions gracefully" {
            $job = Start-Job -ScriptBlock {
                param($ScriptPath)
                try {
                    $output = & pwsh -ExecutionPolicy Bypass -File $ScriptPath -Action "invalid-action" 2>&1
                    return @{ Success = $true; Output = ($output -join "`n") }
                } catch {
                    return @{ Success = $false; Error = $_.Exception.Message }
                }
            } -ArgumentList $TestConfig.ScriptPath

            $completed = Wait-Job $job -Timeout 10
            $completed | Should -Not -BeNullOrEmpty

            $result = Receive-Job $job
            Remove-Job $job

            # Should handle gracefully, not crash
            $result.Output | Should -Match "help|usage|invalid"
        }
    }
}

AfterAll {
    Write-Host "[GSC-Specify-Test] Contract validation tests completed" -ForegroundColor Green
}
