# GSC Command Entity Model
# Enhanced GitHub Spec Commands with Memory Integration
# Date: September 28, 2025

class GSCCommandEntity {
    [string]$CommandName
    [string]$ExecutionLogicPowerShell
    [string]$ExecutionLogicShell
    [string[]]$ValidationRules
    [string[]]$MemoryIntegrationPoints
    [hashtable]$StateManagementRequirements
    [hashtable]$CrossPlatformCompatibility
    [hashtable]$PerformanceTargets
    [string]$Status
    [datetime]$LastExecuted
    [int]$ExecutionTimeSeconds

    # Constructor
    GSCCommandEntity([string]$commandName) {
        $this.CommandName = $commandName
        $this.Status = "NotStarted"
        $this.ValidationRules = @()
        $this.MemoryIntegrationPoints = @()
        $this.StateManagementRequirements = @{}
        $this.CrossPlatformCompatibility = @{
            "Windows" = $true
            "macOS"   = $true
            "Linux"   = $true
        }
        $this.PerformanceTargets = @{
            "MaxExecutionTimeSeconds"  = 30
            "MaxMemoryReadTimeSeconds" = 5
            "MaxStateWriteTimeSeconds" = 2
        }
        $this.SetExecutionPaths($commandName)
    }

    # Set execution paths based on command name
    [void]SetExecutionPaths([string]$commandName) {
        $this.ExecutionLogicPowerShell = ".specify/scripts/gsc/gsc-$commandName.ps1"
        $this.ExecutionLogicShell = ".specify/scripts/gsc/gsc-$commandName.sh"
    }

    # Validate command configuration
    [bool]IsValid() {
        if ([string]::IsNullOrEmpty($this.CommandName)) { return $false }
        if ([string]::IsNullOrEmpty($this.ExecutionLogicPowerShell)) { return $false }
        if ([string]::IsNullOrEmpty($this.ExecutionLogicShell)) { return $false }
        if ($this.PerformanceTargets.MaxExecutionTimeSeconds -gt 30) { return $false }
        if ($this.PerformanceTargets.MaxMemoryReadTimeSeconds -gt 5) { return $false }
        return $true
    }

    # Update execution status
    [void]UpdateStatus([string]$newStatus, [int]$executionTime = 0) {
        $this.Status = $newStatus
        $this.LastExecuted = Get-Date
        if ($executionTime -gt 0) {
            $this.ExecutionTimeSeconds = $executionTime
        }
    }

    # Add memory integration point
    [void]AddMemoryIntegration([string]$memoryFileType) {
        if ($this.MemoryIntegrationPoints -notcontains $memoryFileType) {
            $this.MemoryIntegrationPoints += $memoryFileType
        }
    }

    # Convert to JSON for state persistence
    [hashtable]ToHashtable() {
        return @{
            "CommandName"                 = $this.CommandName
            "ExecutionLogicPowerShell"    = $this.ExecutionLogicPowerShell
            "ExecutionLogicShell"         = $this.ExecutionLogicShell
            "ValidationRules"             = $this.ValidationRules
            "MemoryIntegrationPoints"     = $this.MemoryIntegrationPoints
            "StateManagementRequirements" = $this.StateManagementRequirements
            "CrossPlatformCompatibility"  = $this.CrossPlatformCompatibility
            "PerformanceTargets"          = $this.PerformanceTargets
            "Status"                      = $this.Status
            "LastExecuted"                = $this.LastExecuted.ToString("yyyy-MM-ddTHH:mm:ssZ")
            "ExecutionTimeSeconds"        = $this.ExecutionTimeSeconds
        }
    }

    # Create from JSON/hashtable
    static [GSCCommandEntity]FromHashtable([hashtable]$data) {
        $entity = [GSCCommandEntity]::new($data.CommandName)
        $entity.ExecutionLogicPowerShell = $data.ExecutionLogicPowerShell
        $entity.ExecutionLogicShell = $data.ExecutionLogicShell
        $entity.ValidationRules = $data.ValidationRules
        $entity.MemoryIntegrationPoints = $data.MemoryIntegrationPoints
        $entity.StateManagementRequirements = $data.StateManagementRequirements
        $entity.CrossPlatformCompatibility = $data.CrossPlatformCompatibility
        $entity.PerformanceTargets = $data.PerformanceTargets
        $entity.Status = $data.Status
        if ($data.LastExecuted) {
            $entity.LastExecuted = [datetime]::Parse($data.LastExecuted)
        }
        $entity.ExecutionTimeSeconds = $data.ExecutionTimeSeconds
        return $entity
    }
}

# Factory function for creating GSC commands
function New-GSCCommandEntity {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet("constitution", "specify", "clarify", "plan", "task", "analyze", "implement", "memory", "validate", "status", "rollback", "help")]
        [string]$CommandName
    )

    $entity = [GSCCommandEntity]::new($CommandName)

    # Set command-specific memory integration points
    switch ($CommandName) {
        "constitution" { $entity.AddMemoryIntegration("memory") }
        "specify" {
            $entity.AddMemoryIntegration("avalonia-ui-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "clarify" {
            $entity.AddMemoryIntegration("debugging-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "plan" { $entity.AddMemoryIntegration("memory") }
        "task" {
            $entity.AddMemoryIntegration("avalonia-custom-controls-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "analyze" {
            $entity.AddMemoryIntegration("debugging-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "implement" {
            $entity.AddMemoryIntegration("avalonia-ui-memory")
            $entity.AddMemoryIntegration("debugging-memory")
            $entity.AddMemoryIntegration("avalonia-custom-controls-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "memory" {
            $entity.AddMemoryIntegration("avalonia-ui-memory")
            $entity.AddMemoryIntegration("debugging-memory")
            $entity.AddMemoryIntegration("avalonia-custom-controls-memory")
            $entity.AddMemoryIntegration("memory")
        }
        "help" {
            $entity.AddMemoryIntegration("avalonia-ui-memory")
            $entity.AddMemoryIntegration("debugging-memory")
            $entity.AddMemoryIntegration("avalonia-custom-controls-memory")
            $entity.AddMemoryIntegration("memory")
        }
    }

    return $entity
}

# Export functions when run as module
if ($MyInvocation.InvocationName -ne '.') {
    Export-ModuleMember -Function New-GSCCommandEntity
}
