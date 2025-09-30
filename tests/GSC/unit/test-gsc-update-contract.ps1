# GSC Update Command Contract Test
# Date: September 30, 2025
# Purpose: Contract validation for /gsc/update endpoint for spec updates with backups and validation

<#
.SYNOPSIS
Contract test for GSC Update command for safe specification updates

.DESCRIPTION
Validates that the gsc-update command conforms to the OpenAPI 3.0 contract
specification by returning a GSCCommandResponse-compatible payload with
success, command, and executionTime fields, plus command-specific fields.
#>

Describe "GSC Update Command Contract Tests" {
    BeforeAll {
        $updateScriptPath = ".specify/scripts/gsc/gsc-update.ps1"
        $testSpec = "specs/master/spec.md"
        # Ensure the spec file exists for the test
        if (-not (Test-Path $testSpec)) {
            New-Item -ItemType Directory -Path (Split-Path $testSpec -Parent) -Force | Out-Null
            "# Test Spec`n`n## Requirements`n- Initial" | Out-File -FilePath $testSpec -Encoding UTF8
        }
    }

    Context "OpenAPI 3.0 Contract Compliance" {
        It "Should return contract-compliant fields and update payload" {
            if (Test-Path $updateScriptPath) {
                $responseJson = & $updateScriptPath -File $testSpec -Section "Requirements" -Operation suggest -Json
                $response = $responseJson | ConvertFrom-Json

                # Contract-required fields (robust property checks)
                $response.PSObject.Properties.Name | Should -Contain "success"
                $response.PSObject.Properties.Name | Should -Contain "command"
                $response.PSObject.Properties.Name | Should -Contain "executionTime"
                $response.command | Should -Be "update"

                # Extended fields
                $response.PSObject.Properties.Name | Should -Contain "operation"
                $response.PSObject.Properties.Name | Should -Contain "file"
                $response.PSObject.Properties.Name | Should -Contain "changeSummary"
                $response.changeSummary.PSObject.Properties.Name | Should -Contain "suggestions"
            }
            else {
                $false | Should -Be $true -Because "Update script should exist"
            }
        }
    }
}

Write-Host "[Contract] Update Command Test executed" -ForegroundColor Yellow