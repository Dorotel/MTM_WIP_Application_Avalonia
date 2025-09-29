# Entity: Workflow State (JSON State Schema)
# Purpose: Tracks overall GSC workflow state with validation status and locks

class WorkflowStateEntity {
    [string] $WorkflowId
    [string] $CurrentPhase
    [object[]] $PhaseHistory
    [hashtable] $ValidationStatus
    [string[]] $MemoryIntegrationPoints
    [hashtable] $ConstitutionalComplianceStatus
    [hashtable] $CheckpointData
    [hashtable] $TeamCollaborationLock
    [bool] $PerformanceDegradationMode

    WorkflowStateEntity() {
        $this.WorkflowId = [guid]::NewGuid().ToString()
        $this.CurrentPhase = "not_started"
        $this.PhaseHistory = @()
        $this.ValidationStatus = @{}
        $this.MemoryIntegrationPoints = @()
        $this.ConstitutionalComplianceStatus = @{}
        $this.CheckpointData = @{}
        $this.TeamCollaborationLock = @{ isLocked = $false; lockOwner = $null; lockExpiration = $null }
        $this.PerformanceDegradationMode = $false
    }
}
