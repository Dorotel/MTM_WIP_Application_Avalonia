# GSC Cross-Platform Execution Test: macOS
# Purpose: Validate macOS execution via PowerShell Core and shell wrapper
# CRITICAL: This test MUST FAIL initially (no implementation yet per TDD)

Describe "GSC Cross-Platform Execution - macOS" {
    BeforeAll {
        try {
            Import-Module ".specify/scripts/powershell/cross-platform-utils.ps1" -Force
            $modulesAvailable = $true
        }
        catch {
            $script:modulesAvailable = $false
            Write-Warning "Cross-platform utils not available - expected for TDD"
        }
    }

    It "Should execute core commands via pwsh and shell wrapper on macOS" {
        if ($modulesAvailable) {
            $ps = pwsh -File .specify/scripts/gsc/gsc-constitution.ps1 "test feature" 2>$null
            $ps | Should -Not -BeNullOrEmpty

            $sh = & .specify/scripts/gsc/gsc-constitution.sh "test feature" 2>$null
            $sh | Should -Not -BeNullOrEmpty
        }
        else {
            $false | Should -Be $true -Because "macOS execution not implemented yet (TDD)"
        }
    }
}

Write-Host "[TDD] macOS Cross-Platform Execution Test - Expected to FAIL initially" -ForegroundColor Yellow
