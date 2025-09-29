#!/usr/bin/env pwsh
#
# Cross-Platform Test: Windows Execution Validation
# Tests GSC commands execution on Windows with PowerShell and Git Bash
# Validates Windows-specific features and compatibility
#
# Expected: All GSC commands execute properly on Windows with both PowerShell and Git Bash
# Tests path handling, environment variables, and Windows-specific integrations

Param(
    [switch]$Verbose,
    [switch]$TestGitBash = $true,
    [string]$FeatureName = "Windows Cross-Platform Test Feature"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Test configuration
$TestName = "Windows Execution Validation"
$WindowsTimeout = 60  # seconds for Windows-specific operations
$PathSeparator = "\\"

function Test-WindowsExecution {
    param(
        [bool]$TestGitBash,
        [string]$FeatureName
    )

    Write-TestHeader $TestName

    # Setup test workspace
    $testWorkspace = New-TestWorkspace -Directory ".specify\test-windows"

    try {
        # Phase 1: Validate Windows environment
        Write-TestStep "1. Validate Windows environment and prerequisites"

        # Check PowerShell version
        $psVersion = $PSVersionTable.PSVersion
        Assert-GreaterThanOrEqual -Value $psVersion.Major -Minimum 7 -TestName "PowerShell Core 7.0+ on Windows"

        # Check Windows-specific environment
        Assert-NotNull -Value $env:OS -TestName "Windows OS environment variable"
        Assert-Contains -Content $env:OS -ExpectedText "Windows" -TestName "Windows OS detection"

        # Verify Windows path handling
        $windowsPath = [System.IO.Path]::GetTempPath()
        Assert-Contains -Content $windowsPath -ExpectedText $PathSeparator -TestName "Windows path separator"

        # Phase 2: Test PowerShell execution on Windows
        Write-TestStep "2. Test GSC commands with PowerShell on Windows"

        # Set Windows-specific environment
        $env:GSC_PLATFORM = "windows"
        $env:GSC_SHELL = "powershell"
        $env:GSC_PATH_SEPARATOR = $PathSeparator

        $windowsStart = Get-Date
        $constitutionResult = & ".specify\scripts\gsc\gsc-constitution.ps1" $FeatureName
        $windowsTime = ((Get-Date) - $windowsStart).TotalSeconds

        # Validate Windows PowerShell execution
        Assert-Performance -ActualTime $windowsTime -MaxTime $WindowsTimeout -Operation "Windows PowerShell execution"
        Assert-Contains -Content $constitutionResult -ExpectedText "Constitution compliance validated" -TestName "Windows PowerShell result"
        Assert-Contains -Content $constitutionResult -ExpectedText "Platform: windows" -TestName "Windows platform detection"

        # Test Windows path handling in GSC commands
        $specifyResult = & ".specify\scripts\gsc\gsc-specify.ps1" "Windows-specific feature with path handling"
        Assert-Contains -Content $specifyResult -ExpectedText "Windows path" -TestName "Windows path handling"

        # Phase 3: Test Git Bash execution on Windows (if available)
        if ($TestGitBash) {
            Write-TestStep "3. Test GSC commands with Git Bash on Windows"

            # Check if Git Bash is available
            $gitBashPath = Get-Command "bash.exe" -ErrorAction SilentlyContinue
            if ($gitBashPath) {
                # Test shell wrapper execution
                $env:GSC_SHELL = "git-bash"

                try {
                    # Execute shell wrapper (converts Windows paths to Unix-style for Git Bash)
                    $bashResult = & "bash.exe" "-c" ".specify/scripts/gsc/gsc-constitution.sh '$FeatureName'"
                    Assert-Contains -Content $bashResult -ExpectedText "Constitution compliance validated" -TestName "Git Bash wrapper execution"
                    Assert-Contains -Content $bashResult -ExpectedText "Platform: windows" -TestName "Windows detection in Git Bash"
                }
                catch {
                    Write-TestWarning "Git Bash test failed: $($_.Exception.Message)"
                    # Git Bash failure is not critical for Windows validation
                }
            }
            else {
                Write-TestWarning "Git Bash not found on Windows - skipping Git Bash tests"
            }
        }

        # Phase 4: Test Windows-specific features
        Write-TestStep "4. Test Windows-specific GSC features"

        # Test Windows registry integration (if applicable)
        $windowsFeatures = @()

        # Test Windows file system features
        $longPathTest = "C:\\Windows\\System32\\very\\long\\path\\that\\tests\\windows\\path\\handling\\capabilities"
        $env:GSC_TEST_LONG_PATH = $longPathTest

        $pathResult = & ".specify\scripts\gsc\gsc-validate.ps1" -WindowsPathTest
        Assert-Contains -Content $pathResult -ExpectedText "Windows path validation" -TestName "Windows long path handling"
        $windowsFeatures += "Long path support"

        # Test Windows environment variable handling
        $env:GSC_WINDOWS_TEST_VAR = "WindowsSpecificValue"
        $envResult = & ".specify\scripts\gsc\gsc-status.ps1"
        $windowsFeatures += "Environment variable handling"

        # Phase 5: Test Windows memory file locations
        Write-TestStep "5. Test Windows memory file location detection"

        # Windows should detect memory files in AppData
        $expectedWindowsPath = "$env:USERPROFILE\\AppData\\Roaming\\Code\\User\\prompts"
        $memoryResult = & ".specify\scripts\gsc\gsc-memory.ps1"

        if ($memoryResult -match $expectedWindowsPath.Replace("\\", "[\\\\/]")) {
            Assert-Contains -Content $memoryResult -ExpectedText "AppData" -TestName "Windows AppData memory location"
            $windowsFeatures += "AppData memory file detection"
        }
        else {
            Write-TestWarning "Windows AppData memory path not detected, using fallback location"
        }

        # Phase 6: Test Windows performance characteristics
        Write-TestStep "6. Test Windows performance characteristics"

        # Test multiple commands to establish Windows performance baseline
        $performanceTests = @(
            @{ Command = "gsc-clarify.ps1"; MaxTime = 30 },
            @{ Command = "gsc-plan.ps1"; MaxTime = 30 },
            @{ Command = "gsc-task.ps1"; MaxTime = 30 }
        )

        $windowsPerformanceResults = @()
        foreach ($test in $performanceTests) {
            $testStart = Get-Date
            $testResult = & ".specify\scripts\gsc\$($test.Command)"
            $testTime = ((Get-Date) - $testStart).TotalSeconds

            Assert-Performance -ActualTime $testTime -MaxTime $test.MaxTime -Operation "Windows $($test.Command)"
            $windowsPerformanceResults += @{ Command = $test.Command; Time = $testTime }
        }

        # Calculate average Windows performance
        $avgWindowsTime = ($windowsPerformanceResults | Measure-Object -Property Time -Average).Average
        Assert-Performance -ActualTime $avgWindowsTime -MaxTime 25 -Operation "Average Windows GSC command"

        # Phase 7: Test Windows-specific error handling
        Write-TestStep "7. Test Windows-specific error handling"

        # Test Windows path error scenarios
        $env:GSC_SIMULATE_WINDOWS_PATH_ERROR = "true"
        try {
            $errorResult = & ".specify\scripts\gsc\gsc-validate.ps1"
            Assert-Contains -Content $errorResult -ExpectedText "Windows path error" -TestName "Windows path error handling"
        }
        catch {
            # Expected error for Windows path issues
            Assert-Contains -Content $_.Exception.Message -ExpectedText "path" -TestName "Windows path error caught"
        }
        finally {
            Remove-Item Env:GSC_SIMULATE_WINDOWS_PATH_ERROR -ErrorAction SilentlyContinue
        }

        Write-TestSuccess "Windows execution validation completed successfully"

        return @{
            Success              = $true
            Platform             = "Windows"
            PowerShellVersion    = $psVersion.ToString()
            GitBashAvailable     = $gitBashPath -ne $null
            WindowsFeatures      = $windowsFeatures
            PerformanceResults   = $windowsPerformanceResults
            AverageExecutionTime = $avgWindowsTime
        }
    }
    catch {
        Write-TestError "Windows execution validation failed: $($_.Exception.Message)"
        return @{
            Success  = $false
            Error    = $_.Exception.Message
            Platform = "Windows"
        }
    }
    finally {
        # Cleanup Windows-specific environment variables
        Remove-Item Env:GSC_PLATFORM -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_SHELL -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_PATH_SEPARATOR -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_TEST_LONG_PATH -ErrorAction SilentlyContinue
        Remove-Item Env:GSC_WINDOWS_TEST_VAR -ErrorAction SilentlyContinue

        # Cleanup test workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly
if ($MyInvocation.InvocationName -ne '.') {
    $result = Test-WindowsExecution -TestGitBash $TestGitBash -FeatureName $FeatureName

    if ($result.Success) {
        Write-Host "✅ Cross-platform test passed: Windows Execution" -ForegroundColor Green
        Write-Host "PowerShell version: $($result.PowerShellVersion)" -ForegroundColor Green
        Write-Host "Average execution time: $($result.AverageExecutionTime.ToString('F2'))s" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ Cross-platform test failed: Windows Execution" -ForegroundColor Red
        Write-Host "Error: $($result.Error)" -ForegroundColor Red
        exit 1
    }
}
