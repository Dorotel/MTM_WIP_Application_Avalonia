# GSC Update Cross-Platform Tests (T106)
# Validates existence of shell wrapper and basic invocation contract

Describe "GSC Update - Cross Platform Wrapper" {
    It "Has a shell wrapper present" {
        $wrapper = ".specify/scripts/gsc/gsc-update.sh"
        Test-Path $wrapper | Should -BeTrue
    }

    It "PowerShell script exists and is invokable with -help" {
        $ps1 = ".specify/scripts/gsc/gsc-update.ps1"
        Test-Path $ps1 | Should -BeTrue
        $json = & $ps1 -Operation help -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.command | Should -Be "update"
        $res.Help | Should -Match "GSC Update Command Help"
    }
}

Write-Host "[CrossPlatform] Update wrapper tests executed" -ForegroundColor Yellow
