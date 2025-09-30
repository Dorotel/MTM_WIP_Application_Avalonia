# GSC Update Command Lock Behavior Tests (T104)
# Simulate team collaboration lock and verify -Force handling

Describe "GSC Update Command - Lock Handling" {
    BeforeAll {
        $script = ".specify/scripts/gsc/gsc-update.ps1"
        if (-not (Test-Path $script)) { throw "Missing script: $script" }

        $global:stateFile = ".specify/state/gsc-workflow.json"
        $stateDir = Split-Path $global:stateFile -Parent
        if (-not (Test-Path $stateDir)) { New-Item -ItemType Directory -Path $stateDir | Out-Null }
        $global:stateBackup = $null
        if (Test-Path $global:stateFile) { $global:stateBackup = Get-Content -Path $global:stateFile -Raw }

        $lockState = @{
            workflowId = [guid]::NewGuid().ToString()
            currentPhase = 'plan'
            teamCollaborationLock = @{ isLocked = $true; lockOwner = 'tester'; lockExpiration = (Get-Date).AddHours(1).ToString('o') }
        } | ConvertTo-Json
        $lockState | Out-File -FilePath $global:stateFile -Encoding UTF8

        $global:testSpec = ".specify/state/tests/spec-lock.md"
        New-Item -ItemType Directory -Path (Split-Path $global:testSpec -Parent) -Force | Out-Null
        "# Spec`n`n## Req`n- A" | Out-File -FilePath $global:testSpec -Encoding UTF8
    }

    AfterAll {
        if ($null -ne $global:stateBackup) { $global:stateBackup | Out-File -FilePath $global:stateFile -Encoding UTF8 }
        else { if (Test-Path $global:stateFile) { Remove-Item $global:stateFile -Force } }
        if (Test-Path $global:testSpec) { Remove-Item $global:testSpec -Force }
    }

    It "Fails when locked and -Force not provided" {
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Req" -Operation insert -Content "- X" -Json 2>$null
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeFalse
        $res.Message | Should -Match "locked"
    }

    It "Succeeds when locked but -Force provided" {
        $json = & .specify/scripts/gsc/gsc-update.ps1 -File $global:testSpec -Section "Req" -Operation insert -Content "- Y" -Force -Json
        $res = $json | ConvertFrom-Json
        $res.success | Should -BeTrue
        $res.wroteChanges | Should -BeTrue
    }
}

Write-Host "[Unit] Update Lock tests executed" -ForegroundColor Yellow
