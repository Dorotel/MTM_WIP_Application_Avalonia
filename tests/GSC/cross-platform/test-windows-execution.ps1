# GSC Cross-Platform Execution Test: Windows
# Purpose: Validate Windows execution via PowerShell Core and Git Bash wrapper
# CRITICAL: This test MUST FAIL initially (no implementation yet per TDD)

Describe "GSC Cross-Platform Execution - Windows" {
    BeforeAll {
        $script:modulesAvailable = $false
        try {
            if (Test-Path ".specify/scripts/powershell/cross-platform-utils.ps1") { . ".specify/scripts/powershell/cross-platform-utils.ps1" }
            $script:modulesAvailable = $true
        }
        catch {
            $script:modulesAvailable = $false
            Write-Warning "Cross-platform utils not available - expected for TDD"
        }
    }

    It "Should execute constitution via PowerShell Core and shell wrapper on Windows" {
        if ($modulesAvailable) {
            $ps = pwsh -File .specify/scripts/gsc/gsc-constitution.ps1 "test feature" 2>$null
            $ps | Should -Not -BeNullOrEmpty

            $sh = & .specify/scripts/gsc/gsc-constitution.sh "test feature" 2>$null
            $sh | Should -Not -BeNullOrEmpty
        }
        else {
            $false | Should -Be $true -Because "Cross-platform execution not implemented yet (TDD)"
        }
    }
}

Write-Host "[TDD] Windows Cross-Platform Execution Test - Expected to FAIL initially" -ForegroundColor Yellow
