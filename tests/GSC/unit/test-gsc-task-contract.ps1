# GSC Task Command Contract Test
# Date: September 28, 2025
# Purpose: Contract validation for /gsc/task endpoint with custom control memory patterns
# CRITICAL: This test MUST FAIL initially - no implementation exists yet (TDD requirement)

<#
.SYNOPSIS
Contract test for GSC Task command with custom control memory patterns integration

.DESCRIPTION
Validates that the gsc-task command conforms to the OpenAPI 3.0 contract
specification for task generation with Avalonia custom control memory patterns.

This test must FAIL initially as required by TDD approach.
#>

Describe "GSC Task Command Contract Tests" {
    BeforeAll {
        # Test setup - these paths don't exist yet (intentional failure)
        $taskScriptPath = ".specify/scripts/gsc/gsc-task.ps1"
        $shellWrapperPath = ".specify/scripts/gsc/gsc-task.sh"
        $testWorkflowId = [System.Guid]::NewGuid().ToString()

        # Import GSC modules for testing (will fail initially)
        try {
            Import-Module ".specify/scripts/powershell/common-gsc.ps1" -Force
            Import-Module ".specify/scripts/powershell/memory-integration.ps1" -Force
            $modulesAvailable = $true
        }
        catch {
            $modulesAvailable = $false
            Write-Warning "GSC modules not available yet - expected for initial TDD test run"
        }
    }

    Context "OpenAPI 3.0 Contract Compliance" {
        It "Should generate detailed task list with custom control patterns" {
            # This test WILL FAIL initially - no implementation exists
            $validRequest = @{
                command = "task"
                arguments = ""
                workflowId = $testWorkflowId
                memoryIntegrationEnabled = $true
            }

            # Expected custom control memory patterns
            $expectedCustomControlPatterns = @(
                "Multi-Variant Styling System",
                "Grid Layout Container Patterns",
                "TextBox Scroll Integration",
                "Manufacturing Field Control Pattern",
                "Theme Integration Patterns"
            )

            if (Test-Path $taskScriptPath) {
                $response = & $taskScriptPath

                $response.success | Should -Be $true
                $response.command | Should -Be "task"

                # Should apply custom control patterns
                $appliedPatterns = $response.memoryPatternsApplied
                foreach ($pattern in $expectedCustomControlPatterns) {
                    $appliedPatterns | Should -Contain $pattern
                }

                # Should generate tasks.md file
                Test-Path "specs/*/tasks.md" | Should -Be $true
            }
            else {
                $false | Should -Be $true -Because "Task script should exist (will fail initially)"
            }
        }
    }

    Context "Custom Control Memory Integration" {
        It "Should load avalonia-custom-controls-memory.instructions.md patterns" {
            # This test WILL FAIL initially - no memory integration exists
            if ($modulesAvailable) {
                $result = Get-RelevantMemoryPatterns -CommandName "task"

                # Should specifically load custom controls memory file
                $customControlsMemoryLoaded = $result.Patterns | Where-Object { $_.FileType -eq "avalonia-custom-controls-memory" }
                $customControlsMemoryLoaded | Should -Not -BeNullOrEmpty

                # Should contain custom control patterns
                $customControlPatterns = $customControlsMemoryLoaded | Where-Object {
                    $_.Title -match "Custom|Control|Layout|Styling|Manufacturing"
                }
                $customControlPatterns.Count | Should -BeGreaterThan 0
            }
            else {
                $false | Should -Be $true -Because "Custom controls memory integration should be available (will fail initially)"
            }
        }
    }

    Context "Task Generation Contract" {
        It "Should generate 99 total tasks with proper phase organization" {
            # This test WILL FAIL initially - no task generation exists
            $expectedPhases = @(
                "Phase 3.1: Project Setup",
                "Phase 3.2: Tests First (TDD)",
                "Phase 3.3: Core Entity Models",
                "Phase 3.4: Enhanced GSC Command Implementation",
                "Phase 3.5: Validation and State Management"
            )

            if (Test-Path $taskScriptPath) {
                $result = & $taskScriptPath

                # Should create tasks.md with proper structure
                Test-Path "specs/*/tasks.md" | Should -Be $true
                $tasksContent = Get-Content "specs/*/tasks.md" -Raw

                # Should have expected number of tasks
                $taskCount = ([regex]::Matches($tasksContent, "\*\*T\d+\*\*")).Count
                $taskCount | Should -Be 99

                # Should have proper phase organization
                foreach ($phase in $expectedPhases) {
                    $tasksContent | Should -Match ([regex]::Escape($phase))
                }

                # Should have parallel execution markers
                $tasksContent | Should -Match "\[P\]"
            }
            else {
                $false | Should -Be $true -Because "Task generation should exist (will fail initially)"
            }
        }
    }
}

# Mark test as TDD requirement
Write-Host "[TDD] Task Contract Test - Expected to FAIL initially" -ForegroundColor Yellow
