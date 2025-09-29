#!/usr/bin/env pwsh
#
# Integration Test: Memory File Management (Scenario 5)
# Tests GSC memory file management and pattern update capabilities
# Based on quickstart.md Scenario 5
#
# Expected: Memory file display, validation, and pattern updates with integrity checking
# Tests memory file operations, pattern management, and checksum validation

Param(
    [switch]$Verbose,
    [string]$TestPattern = "DataGrid column binding context switching resolution",
    [string]$MemoryFileType = "avalonia-ui-memory"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Test configuration
$TestName = "Memory Management Workflow"
$MemoryDisplayTimeout = 10  # seconds
$PatternUpdateTimeout = 5   # seconds
$IntegrityCheckTimeout = 3  # seconds

function Test-MemoryManagementWorkflow {
    param(
        [string]$TestPattern,
        [string]$MemoryFileType
    )

    Write-TestHeader $TestName

    # Setup test workspace
    $testWorkspace = New-TestWorkspace -Directory ".specify/test-memory"

    try {
        # Phase 1: Display current memory file status
        Write-TestStep "1. Display current memory file status"

        $memoryStart = Get-Date
        $memoryResult = & ".specify/scripts/gsc/gsc-memory.ps1"
        $memoryTime = ((Get-Date) - $memoryStart).TotalSeconds

        # Validate memory display performance
        Assert-Performance -ActualTime $memoryTime -MaxTime $MemoryDisplayTimeout -Operation "Memory file display"

        # Parse memory status JSON
        $memoryJson = $memoryResult | ConvertFrom-Json

        # Verify memory file structure
        Assert-NotNull -Value $memoryJson.memoryFiles -TestName "Memory files array exists"
        Assert-GreaterThan -Value $memoryJson.totalPatterns -Minimum 1 -TestName "Total patterns count"
        Assert-Contains -Content $memoryJson.integrityStatus -ExpectedText "valid" -TestName "Memory integrity status"

        # Verify specific memory file types exist
        $memoryFileTypes = @("avalonia-ui-memory", "debugging-memory", "memory", "avalonia-custom-controls-memory")
        foreach ($fileType in $memoryFileTypes) {
            $fileExists = $memoryJson.memoryFiles | Where-Object { $_.fileType -eq $fileType }
            Assert-NotNull -Value $fileExists -TestName "$fileType file exists"
        }

        # Phase 2: Validate individual memory file details
        Write-TestStep "2. Validate individual memory file details"

        $targetMemoryFile = $memoryJson.memoryFiles | Where-Object { $_.fileType -eq $MemoryFileType }
        Assert-NotNull -Value $targetMemoryFile -TestName "Target memory file found"

        # Verify memory file properties
        Assert-NotNull -Value $targetMemoryFile.filePath -TestName "Memory file path exists"
        Assert-Equal -Expected $true -Actual $targetMemoryFile.isValid -TestName "Memory file is valid"
        Assert-GreaterThan -Value $targetMemoryFile.patternCount -Minimum 1 -TestName "Pattern count"
        Assert-NotNull -Value $targetMemoryFile.checksumHash -TestName "Checksum hash exists"
        Assert-NotNull -Value $targetMemoryFile.applicableToCommands -TestName "Applicable commands list"

        # Store original pattern count for comparison
        $originalPatternCount = $targetMemoryFile.patternCount
        $originalChecksum = $targetMemoryFile.checksumHash

        # Phase 3: Test memory pattern update
        Write-TestStep "3. Update memory file with new pattern"

        $updateStart = Get-Date
        $updateResult = & ".specify/scripts/gsc/gsc-memory.ps1" -Update $MemoryFileType -Pattern $TestPattern
        $updateTime = ((Get-Date) - $updateStart).TotalSeconds

        # Validate update performance
        Assert-Performance -ActualTime $updateTime -MaxTime $PatternUpdateTimeout -Operation "Pattern update"

        # Verify update result
        Assert-Contains -Content $updateResult -ExpectedText "Pattern added to $MemoryFileType" -TestName "Pattern addition confirmation"
        Assert-Contains -Content $updateResult -ExpectedText "single source of truth" -TestName "Conflict resolution confirmation"

        # Phase 4: Verify pattern was added successfully
        Write-TestStep "4. Verify pattern addition and checksum update"

        $postUpdateResult = & ".specify/scripts/gsc/gsc-memory.ps1"
        $postUpdateJson = $postUpdateResult | ConvertFrom-Json

        $updatedMemoryFile = $postUpdateJson.memoryFiles | Where-Object { $_.fileType -eq $MemoryFileType }

        # Verify pattern count increased (or stayed same if pattern was replaced)
        Assert-GreaterThanOrEqual -Value $updatedMemoryFile.patternCount -Minimum $originalPatternCount -TestName "Pattern count maintained or increased"

        # Verify checksum changed (indicating file was modified)
        Assert-NotEqual -Expected $originalChecksum -Actual $updatedMemoryFile.checksumHash -TestName "Checksum updated after modification"

        # Phase 5: Test memory file integrity validation
        Write-TestStep "5. Validate memory file integrity"

        $integrityStart = Get-Date
        $integrityResult = & ".specify/scripts/gsc/gsc-validate.ps1" -MemoryOnly
        $integrityTime = ((Get-Date) - $integrityStart).TotalSeconds

        # Validate integrity check performance
        Assert-Performance -ActualTime $integrityTime -MaxTime $IntegrityCheckTimeout -Operation "Integrity validation"

        # Verify integrity validation results
        Assert-Contains -Content $integrityResult -ExpectedText "memory files validated" -TestName "Memory validation completion"
        Assert-Contains -Content $integrityResult -ExpectedText "checksum and content integrity" -TestName "Checksum validation"
        Assert-NotContains -Content $integrityResult -ExpectedText "integrity failed" -TestName "No integrity failures"

        # Phase 6: Test pattern conflict resolution
        Write-TestStep "6. Test pattern conflict resolution"

        # Add a conflicting pattern to test replacement
        $conflictingPattern = "DataGrid column binding - OLD APPROACH (replaced by context switching resolution)"
        $conflictResult = & ".specify/scripts/gsc/gsc-memory.ps1" -Update $MemoryFileType -Pattern $conflictingPattern

        Assert-Contains -Content $conflictResult -ExpectedText "Conflicting patterns replaced" -TestName "Conflict resolution"

        # Phase 7: Test memory integration with GSC commands
        Write-TestStep "7. Test memory integration with GSC commands"

        # Test that updated memory patterns are used in GSC commands
        $constitutionWithMemory = & ".specify/scripts/gsc/gsc-constitution.ps1" "Test pattern integration"

        # Should show evidence of memory pattern integration
        Assert-Contains -Content $constitutionWithMemory -ExpectedText "memory patterns loaded" -TestName "Memory integration in constitution"

        # Test specific memory file integration
        $specifyWithMemory = & ".specify/scripts/gsc/gsc-specify.ps1" "Test UI feature with updated patterns"
        Assert-Contains -Content $specifyWithMemory -ExpectedText "Avalonia UI patterns applied" -TestName "Specific memory file integration"

        # Phase 8: Test cross-platform memory file access
        Write-TestStep "8. Test cross-platform memory file access"

        # Verify memory files are accessible across different platforms
        $crossPlatformResult = & ".specify/scripts/gsc/gsc-memory.ps1" -CrossPlatformTest

        Assert-Contains -Content $crossPlatformResult -ExpectedText "cross-platform access validated" -TestName "Cross-platform access"

        Write-TestSuccess "Memory management workflow completed successfully"

        return @{
            Success            = $true
            MemoryFileType     = $MemoryFileType
            PatternMetrics     = @{
                OriginalPatternCount = $originalPatternCount
                FinalPatternCount    = $updatedMemoryFile.patternCount
                PatternAdded         = $TestPattern
                ChecksumChanged      = $originalChecksum -ne $updatedMemoryFile.checksumHash
            }
            PerformanceMetrics = @{
                DisplayTime   = $memoryTime
                UpdateTime    = $updateTime
                IntegrityTime = $integrityTime
            }
            MemoryFeatures     = @(
                "Memory file display",
                "Pattern count tracking",
                "Checksum validation",
                "Pattern addition",
                "Conflict resolution",
                "Integrity validation",
                "GSC command integration",
                "Cross-platform access"
            )
        }
    }
    catch {
        Write-TestError "Memory management workflow failed: $($_.Exception.Message)"
        return @{
            Success        = $false
            Error          = $_.Exception.Message
            MemoryFileType = $MemoryFileType
        }
    }
    finally {
        # Cleanup test workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly
if ($MyInvocation.InvocationName -ne '.') {
    $result = Test-MemoryManagementWorkflow -TestPattern $TestPattern -MemoryFileType $MemoryFileType

    if ($result.Success) {
        Write-Host "✅ Integration test passed: Memory Management Workflow" -ForegroundColor Green
        Write-Host "Pattern metrics: $($result.PatternMetrics | ConvertTo-Json -Compress)" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "❌ Integration test failed: Memory Management Workflow" -ForegroundColor Red
        Write-Host "Error: $($result.Error)" -ForegroundColor Red
        exit 1
    }
}
