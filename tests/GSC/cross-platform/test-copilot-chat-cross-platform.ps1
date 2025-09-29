# GSC Cross-Platform Copilot Chat Test
# Purpose: Validate chat formatting and execution across Windows/macOS/Linux
# CRITICAL: This test MUST FAIL initially (no implementation yet per TDD)

Describe "GSC Copilot Chat Cross-Platform" {
    BeforeAll {
        try {
            Import-Module ".specify/scripts/powershell/common-gsc.ps1" -Force
            Import-Module ".specify/scripts/powershell/cross-platform-utils.ps1" -Force
            $modulesAvailable = $true
        }
        catch {
            $script:modulesAvailable = $false
            Write-Warning "GSC modules not available yet - expected for TDD"
        }
    }

    It "Should include chatDisplay metadata on all platforms" {
        if ($modulesAvailable) {
            $constitution = & .specify/scripts/gsc/gsc-constitution.ps1 "chat test" 2>$null
            $constitution.chatDisplay.formattedContent | Should -Not -BeNullOrEmpty
            $constitution.chatDisplay.progressIndicator | Should -Not -BeNullOrEmpty
        }
        else {
            $false | Should -Be $true -Because "Chat metadata not implemented yet (TDD)"
        }
    }
}

Write-Host "[TDD] Copilot Chat Cross-Platform Test - Expected to FAIL initially" -ForegroundColor Yellow
