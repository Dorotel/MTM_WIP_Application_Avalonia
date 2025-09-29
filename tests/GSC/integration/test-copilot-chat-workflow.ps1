#!/usr/bin/env pwsh
#
# Integration Test: GitHub Copilot Chat Workflow Execution
# Tests GSC commands execution within GitHub Copilot Chat context
# Tests chat formatting, interactive display, and VS Code/VS2022 integration
#
# Expected: GSC commands work seamlessly in Copilot Chat with proper formatting
# Tests markdown formatting, collapsible sections, and quick actions

Param(
    [switch]$Verbose,
    [string]$ChatContext = "vscode",
    [string]$FeatureName = "Copilot Chat GSC Integration Test"
)

# Import GSC test utilities
. "$PSScriptRoot/../test-utilities.ps1"

# Resolve repository root (tests/GSC/integration -> repo root)
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path

function Invoke-GSCScript {
    param(
        [Parameter(Mandatory)] [string]$Name,
        [string[]]$ScriptArgs
    )
    $scriptPath = Join-Path $RepoRoot ".specify\scripts\gsc\gsc-$Name.ps1"
    Assert-FileExists -FilePath $scriptPath -TestName "GSC script exists: $Name"
    return & $scriptPath @ScriptArgs
}

# Test configuration
$TestName = "Copilot Chat Workflow"
$ChatResponseTimeout = 15
# Note: FormattingValidationTimeout removed (unused)

function Test-CopilotChatWorkflow {
    param(
        [string]$ChatContext,
        [string]$FeatureName
    )

    Write-TestHeader $TestName

    # Setup test workspace for chat integration
    $testWorkspace = New-TestWorkspace -Directory ".specify/test-copilot-chat"

    try {
        # Phase 1: GSC constitution command
        Write-TestStep "1. Test GSC constitution command in $ChatContext context"

        # Set environment variables for chat context
        $env:GSC_EXECUTION_CONTEXT = "copilot-chat-$ChatContext"
        $env:GSC_CHAT_FORMATTING_ENABLED = "true"
        $env:GSC_MARKDOWN_OUTPUT = "true"
        $env:GSC_COLLAPSIBLE_SECTIONS = "true"
        $env:GSC_INCLUDE_EMOJIS = "true"

        $constitutionStart = Get-Date
        $constitutionResult = Invoke-GSCScript -Name "constitution" -Args @($FeatureName)
        $constitutionTime = ((Get-Date) - $constitutionStart).TotalSeconds

        # Validate performance
        Assert-Performance -ActualTime $constitutionTime -MaxTime $ChatResponseTimeout -Operation "Chat constitution command"

        # Verify formatting
        Assert-Contains -Content $constitutionResult -ExpectedText "# Constitution Validation" -TestName "Markdown header"
        Assert-Contains -Content $constitutionResult -ExpectedText "<details>" -TestName "Collapsible sections"
        Assert-Contains -Content $constitutionResult -ExpectedText "<summary>" -TestName "Section summaries"
        Assert-Contains -Content $constitutionResult -ExpectedText '```json' -TestName "Code block formatting"

        # Try parsing JSON response if available
        if ($constitutionResult -match '"chatDisplay"') {
            try {
                $resultJson = $constitutionResult | ConvertFrom-Json -ErrorAction Stop
            }
            catch {
                $resultJson = $null
            }
            if ($resultJson) {
                Assert-NotNull -Value $resultJson.chatDisplay -TestName "Chat display object exists"
                Assert-NotNull -Value $resultJson.chatDisplay.formattedContent -TestName "Formatted content exists"
                Assert-NotNull -Value $resultJson.chatDisplay.quickActions -TestName "Quick actions exist"
                Assert-NotNull -Value $resultJson.chatDisplay.progressIndicator -TestName "Progress indicator exists"
            }
        }

        # Phase 2: Quick actions
        Write-TestStep "2. Test interactive quick actions in chat"
        Assert-Contains -Content $constitutionResult -ExpectedText "Next: gsc specify" -TestName "Next action suggestion"
        Assert-Contains -Content $constitutionResult -ExpectedText "Quick Actions" -TestName "Quick actions section"

        # Phase 3: Specify command
        Write-TestStep "3. Test specify command with enhanced chat display"

        $specifyStart = Get-Date
        $specifyResult = Invoke-GSCScript -Name "specify" -Args @("Add inventory management feature with Copilot Chat integration")
        $specifyTime = ((Get-Date) - $specifyStart).TotalSeconds

        Assert-Contains -Content $specifyResult -ExpectedText "## Feature Specification" -TestName "Specification header"
        Assert-Contains -Content $specifyResult -ExpectedText "### Memory Patterns Applied" -TestName "Memory patterns section"
        Assert-Contains -Content $specifyResult -ExpectedText "### Next Steps" -TestName "Next steps section"

        # Phase 4: Help integration
        Write-TestStep "4. Test contextual help in chat context"
        $helpResult = Invoke-GSCScript -Name "help" -Args @("-Format", "browser", "-Topic", "workflow")

        Assert-Contains -Content $helpResult -ExpectedText "Interactive help" -TestName "Interactive help confirmation"
        Assert-Contains -Content $helpResult -ExpectedText "Quick Reference" -TestName "Quick reference section"
        Assert-Contains -Content $helpResult -ExpectedText "workflow guides" -TestName "Workflow guides available"

        # Phase 5: Status command
        Write-TestStep "5. Test status command with visual progress indicators"

        $statusStart = Get-Date
        $statusResult = Invoke-GSCScript -Name "status"
        $statusTime = ((Get-Date) - $statusStart).TotalSeconds

        try {
            $statusJson = $statusResult | ConvertFrom-Json -ErrorAction Stop
        }
        catch {
            $statusJson = $null
        }

        if ($statusJson) {
            Assert-NotNull -Value $statusJson.progress -TestName "Progress object exists"
            Assert-GreaterThanOrEqual -Value $statusJson.progress.overallProgress -Minimum 0 -TestName "Progress percentage valid"

            if ($statusJson.chatDisplay) {
                Assert-NotNull -Value $statusJson.chatDisplay.progressIndicator -TestName "Progress indicator for chat"
                Assert-Contains -Content $statusJson.chatDisplay.progressIndicator.emoji -ExpectedText "Progress" -TestName "Progress indicator"
            }
        }

        # Phase 6: Memory command
        Write-TestStep "6. Test memory command with interactive display"
        $memoryResult = Invoke-GSCScript -Name "memory"
        Assert-Contains -Content $memoryResult -ExpectedText "## Memory Files Status" -TestName "Memory status header"
        Assert-Contains -Content $memoryResult -ExpectedText "Memory File" -TestName "Table formatting"
        Assert-Contains -Content $memoryResult -ExpectedText "OK" -TestName "Status indicators"

        # Phase 7: Error formatting
        Write-TestStep "7. Test error handling and formatting in chat context"
        $env:GSC_SIMULATE_VALIDATION_ERROR = "true"
        $env:GSC_ERROR_MESSAGE = "Test validation error for chat formatting"

        try {
            $errorResult = Invoke-GSCScript -Name "validate"
            Assert-Contains -Content $errorResult -ExpectedText "ERROR" -TestName "Error formatting"
            Assert-Contains -Content $errorResult -ExpectedText '```' -TestName "Error code block"
        }
        catch {
            Assert-Contains -Content $_.Exception.Message -ExpectedText "Test validation error" -TestName "Expected error occurred"
        }
        finally {
            if (Test-Path Env:GSC_SIMULATE_VALIDATION_ERROR) { Remove-Item Env:GSC_SIMULATE_VALIDATION_ERROR -Force }
            if (Test-Path Env:GSC_ERROR_MESSAGE) { Remove-Item Env:GSC_ERROR_MESSAGE -Force }
        }

        # Phase 8: Context compatibility
        Write-TestStep "8. Test cross-context compatibility for $ChatContext"
        if ($ChatContext -eq "vscode") {
            $vscodeResult = Invoke-GSCScript -Name "help" -Args @("-Format", "html")
            Assert-Contains -Content $vscodeResult -ExpectedText "VS Code integration" -TestName "VS Code specific features"
        }
        elseif ($ChatContext -eq "vs2022") {
            $vs2022Result = Invoke-GSCScript -Name "help" -Args @("-Format", "html")
            Assert-Contains -Content $vs2022Result -ExpectedText "Visual Studio integration" -TestName "VS2022 specific features"
        }

        # Phase 9: Workflow completion
        Write-TestStep "9. Test complete workflow display in chat"
        $clarifyResult = Invoke-GSCScript -Name "clarify"
        Assert-Contains -Content $clarifyResult -ExpectedText "Workflow Progress" -TestName "Workflow progress tracking"
        Assert-Contains -Content $clarifyResult -ExpectedText "Phase: clarify" -TestName "Current phase display"

        Write-TestSuccess "Copilot Chat workflow completed successfully"

        return @{
            Success            = $true
            ChatContext        = $ChatContext
            ChatFeatures       = @(
                "Markdown formatting",
                "Interactive display",
                "Collapsible sections",
                "Quick actions",
                "Progress indicators",
                "Interactive help",
                "Error formatting",
                "Context-specific integration"
            )
            PerformanceMetrics = @{
                ConstitutionChatTime    = $constitutionTime
                SpecifyTime             = $specifyTime
                StatusTime              = $statusTime
                AverageChatResponseTime = ($constitutionTime + $specifyTime + $statusTime) / 3
            }
        }
    }
    catch {
        Write-TestError "Copilot Chat workflow failed: $($_.Exception.Message)"
        return @{
            Success     = $false
            Error       = $_.Exception.Message
            ChatContext = $ChatContext
        }
    }
    finally {
        # Clean up environment variables
        foreach ($var in "GSC_EXECUTION_CONTEXT", "GSC_CHAT_FORMATTING_ENABLED", "GSC_MARKDOWN_OUTPUT", "GSC_COLLAPSIBLE_SECTIONS", "GSC_INCLUDE_EMOJIS") {
            if (Test-Path Env:$var) { Remove-Item Env:$var -Force }
        }
        # Clean up test workspace
        Remove-TestWorkspace -Directory $testWorkspace
    }
}

# Execute test if running directly (not dot-sourced)
if ($MyInvocation.InvocationName -ne '.' -and $MyInvocation.PSScriptRoot) {
    $result = Test-CopilotChatWorkflow -ChatContext $ChatContext -FeatureName $FeatureName

    if ($result.Success) {
        Write-Host "SUCCESS: Integration test passed - Copilot Chat Workflow ($($result.ChatContext))" -ForegroundColor Green
        Write-Host "Chat features: $($result.ChatFeatures -join ', ')" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "ERROR: Integration test failed - Copilot Chat Workflow" -ForegroundColor Red
        if ($result.Error) {
            Write-Host "Error Details: $($result.Error)" -ForegroundColor Red
        }
        exit 1
    }
}
