# GSC Update Command Mode Tests (T103)
# Validates insert/append/replace/remove behaviors, section creation, and write counts

Describe "GSC Update Command - Modes" {
    BeforeAll {
        $script = ".specify/scripts/gsc/gsc-update.ps1"
        if (-not (Test-Path $script)) {
            throw "Missing script: $script"
        }

        $testDir = ".specify/state/tests"
        if (-not (Test-Path $testDir)) { New-Item -ItemType Directory -Path $testDir | Out-Null }

        $global:testSpec = Join-Path $testDir ("spec-update-" + [guid]::NewGuid().ToString() + ".md")
        @(
            "# Test Spec",
            "",
            "## Requirements",
            "- Initial",
            "",
            "## Other",
            "- Keep"
        ) -join "`n" | Out-File -FilePath $global:testSpec -Encoding UTF8
    }

    AfterAll {
        if (Test-Path $global:testSpec) { Remove-Item $global:testSpec -Force }
    }

    It "Insert adds lines under existing section and reports count" {
        $content = "- Inserted A`n- Inserted B"
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Requirements" -Operation insert -Content $content -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.changeSummary.linesAdded | Should -Be 2

        $text = Get-Content -Path $global:testSpec -Raw
        $text | Should -Match "(?s)## Requirements\s*- Initial\s*- Inserted A\s*- Inserted B"
    }

    It "Append adds lines after previous body and reports count" {
        $content = "- Appended C"
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Requirements" -Operation append -Content $content -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.changeSummary.linesAdded | Should -Be 1

        $text = Get-Content -Path $global:testSpec -Raw
        $text | Should -Match "(?s)## Requirements[\s\S]*- Appended C"
    }

    It "Replace swaps section body with new content (header may be removed by implementation)" {
        $content = "## Requirements`n- Replaced Only"
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Requirements" -Operation replace -Content $content -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.changeSummary.linesAdded | Should -Be 2

        $text = Get-Content -Path $global:testSpec -Raw
        # Accept either preserved or re-inserted header forms
        ($text -match "## Requirements[\s\S]*- Replaced Only") | Should -BeTrue
    }

    It "Remove deletes the section and reports removedLines" {
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Other" -Operation remove -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.changeSummary.removedSection | Should -Be "Other"
        $res.changeSummary.removedLines | Should -BeGreaterThan 0

        $text = Get-Content -Path $global:testSpec -Raw
        $text | Should -Not -Match "^##\s+Other\s*$"
    }

    It "Insert creates section when missing and writes content" {
        $content = "- New Changelog Entry"
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Changelog" -Operation insert -Content $content -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.changeSummary.section | Should -Be "Changelog"
        $res.changeSummary.linesAdded | Should -Be 1

        $text = Get-Content -Path $global:testSpec -Raw
        $text | Should -Match "^##\s+Changelog\s*$"
        $text | Should -Match "- New Changelog Entry"
    }
}

Write-Host "[Unit] Update Modes tests executed" -ForegroundColor Yellow
