#!/usr/bin/env pwsh
# GSC Help Command - Comprehensive documentation and examples

[CmdletBinding()]
param(
    [string]$Command
)

$commands = @(
    @{ name = "constitution"; description = "Project constitution management"; memoryIntegration = $false; parameters = @(); examples = @("gsc-constitution.ps1 display") },
    @{ name = "specify"; description = "Specification management"; memoryIntegration = $true; parameters = @(); examples = @("gsc-specify.ps1 -Action create -SpecType ui-component -Name InventoryControl -MemoryIntegration") },
    @{ name = "clarify"; description = "Clarify requirements"; memoryIntegration = $true; parameters = @(); examples = @("gsc-clarify.ps1") },
    @{ name = "plan"; description = "Planning operations"; memoryIntegration = $true; parameters = @(); examples = @("gsc-plan.ps1") },
    @{ name = "task"; description = "Task breakdown"; memoryIntegration = $true; parameters = @(); examples = @("gsc-task.ps1") },
    @{ name = "analyze"; description = "Analysis operations"; memoryIntegration = $true; parameters = @(); examples = @("gsc-analyze.ps1") },
    @{ name = "implement"; description = "Implementation operations"; memoryIntegration = $true; parameters = @(); examples = @("gsc-implement.ps1") },
    @{ name = "memory"; description = "Memory file operations"; memoryIntegration = $true; parameters = @(@{name = "operation"; description = "get|post|status|update" }); examples = @("gsc-memory.ps1 get") },
    @{ name = "validate"; description = "Validation with quality gates"; memoryIntegration = $true; parameters = @(@{name = "--memory-only"; description = "Validate only memory-related checks" }); examples = @("gsc-validate.ps1 --memory-only") },
    @{ name = "status"; description = "Workflow status report"; memoryIntegration = $false; parameters = @(@{name = "--json"; description = "Return JSON" }); examples = @("gsc-status.ps1 --json") },
    @{ name = "rollback"; description = "Workflow reset operations"; memoryIntegration = $false; parameters = @(@{name = "--confirm"; description = "Confirm reset" }, @{name = "--phase"; description = "Partial rollback to phase" }); examples = @("gsc-rollback.ps1 --confirm --phase plan") },
    @{ name = "help"; description = "This help documentation"; memoryIntegration = $false; parameters = @(@{name = "--command"; description = "Specific command help" }); examples = @("gsc-help.ps1 --command specify") }
)

if ($PSBoundParameters.ContainsKey('Command') -and $Command) {
    $cmd = $commands | Where-Object { $_.name -eq $Command }
    if ($null -ne $cmd) {
        return [ordered]@{
            command             = $cmd.name
            detailedDescription = $cmd.description
            parameters          = $cmd.parameters
            examples            = $cmd.examples
        }
    }
}

$quickStart = @(
    "Start with constitution to understand project guardrails",
    "Use specify/clarify to shape the work",
    "Plan and task to create executable steps",
    "Analyze and implement to deliver",
    "Use memory integration to leverage patterns"
)

$manufacturingGuide = @(
    "Design for 24/7 operations and shift handoffs",
    "Use collaboration locks for team workflows",
    "Ensure graceful degradation under load",
    "Validate with memory and manufacturing quality gates"
)

[ordered]@{
    gscVersion                 = "1.0.0"
    availableCommands          = $commands
    quickStartGuide            = $quickStart
    memoryIntegrationHelp      = "Use memory files to guide patterns and validation across commands"
    manufacturingWorkflowGuide = $manufacturingGuide
    systemCompatibility        = "Windows, macOS, Linux with PowerShell 7+"
}
