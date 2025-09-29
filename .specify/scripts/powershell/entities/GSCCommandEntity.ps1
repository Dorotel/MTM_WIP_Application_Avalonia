# Entity: GSC Command (JSON State Schema)
# Purpose: Implements the GSC Command entity per data-model.md for state serialization

class GSCCommandEntity {
    [string] $CommandName
    [string] $ExecutionScript
    [string] $ShellWrapper
    [string[]] $ValidationRules
    [string[]] $MemoryIntegrationPoints
    [hashtable] $StateManagementRequirements
    [hashtable] $CrossPlatformCompatibility
    [hashtable] $PerformanceTargets

    GSCCommandEntity() {}

    static [GSCCommandEntity] FromSpec([hashtable] $spec) {
        $e = [GSCCommandEntity]::new()
        $e.CommandName = $spec.CommandName
        $e.ExecutionScript = $spec.ExecutionScript
        $e.ShellWrapper = $spec.ShellWrapper
        $e.ValidationRules = @($spec.ValidationRules)
        $e.MemoryIntegrationPoints = @($spec.MemoryIntegrationPoints)
        $e.StateManagementRequirements = $spec.StateManagementRequirements
        $e.CrossPlatformCompatibility = $spec.CrossPlatformCompatibility
        $e.PerformanceTargets = $spec.PerformanceTargets
        return $e
    }

    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.CommandName) -and
               -not [string]::IsNullOrWhiteSpace($this.ExecutionScript)
    }
}
