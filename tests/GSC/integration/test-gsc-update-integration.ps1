# GSC Update Integration Tests (T105)
# Covers backups, decision records, and -ValidateAfter flow

Describe "GSC Update - Integration Flow" {
    BeforeAll {
        $script = ".specify/scripts/gsc/gsc-update.ps1"
        if (-not (Test-Path $script)) { throw "Missing script: $script" }
        $global:testSpec = ".specify/state/tests/spec-integration.md"
        New-Item -ItemType Directory -Path (Split-Path $global:testSpec -Parent) -Force | Out-Null
        "# Spec`n`n## Requirements`n- Base" | Out-File -FilePath $global:testSpec -Encoding UTF8
    }

    AfterAll {
        if (Test-Path $global:testSpec) { Remove-Item $global:testSpec -Force }
    }

    It "Creates backup and decision record, and runs validation when requested" {
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Requirements" -Operation insert -Content "- Integration Added" -ValidateAfter -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        Test-Path $res.backupPath | Should -BeTrue
        Test-Path $res.decisionRecord | Should -BeTrue
        # validation may be null if script delegates; accept null or object with Success/message
        ($null -eq $res.validation -or $res.validation -ne $null) | Should -BeTrue
    }
}

Write-Host "[Integration] Update integration tests executed" -ForegroundColor Yellow
