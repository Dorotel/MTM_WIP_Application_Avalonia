# Workflow State Entity Model
# Enhanced GitHub Spec Commands with Memory Integration
# Date: September 28, 2025

class WorkflowStateEntity {
    [string]$WorkflowId
    [string]$CurrentPhase
    [System.Collections.ArrayList]$PhaseHistory
    [hashtable]$ValidationStatus
    [hashtable]$MemoryIntegrationPoints
    [hashtable]$ConstitutionalComplianceStatus
    [hashtable]$CheckpointData
    [hashtable]$TeamCollaborationLock
    [bool]$PerformanceDegradationMode
    [string]$Status
    [datetime]$CreatedAt
    [datetime]$LastUpdated

    # Constructor
    WorkflowStateEntity([string]$workflowId) {
        $this.WorkflowId = $workflowId
        $this.CurrentPhase = "constitution"
        $this.PhaseHistory = [System.Collections.ArrayList]::new()
        $this.ValidationStatus = @{
            "ConstitutionalCompliance"   = "pending"
            "MemoryIntegration"          = "pending"
            "CrossPlatformCompatibility" = "pending"
            "PerformanceTargets"         = "pending"
        }
        $this.MemoryIntegrationPoints = @{
            "ProcessedFiles"      = @()
            "AppliedPatterns"     = @()
            "LastIntegrationTime" = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
        $this.ConstitutionalComplianceStatus = @{
            "CodeQuality"      = "pending"
            "TestingStandards" = "pending"
            "UserExperience"   = "pending"
            "Performance"      = "pending"
            "GSCWorkflow"      = "pending"
        }
        $this.CheckpointData = @{}
        $this.TeamCollaborationLock = @{
            "IsLocked"       = $false
            "LockOwner"      = ""
            "LockExpiration" = ""
        }
        $this.PerformanceDegradationMode = $false
        $this.Status = "Initializing"
        $this.CreatedAt = Get-Date
        $this.LastUpdated = Get-Date
    }

    # Advance to next phase
    [bool]AdvancePhase([string]$newPhase, [hashtable]$phaseResult) {
        $validPhases = @("constitution", "specify", "clarify", "plan", "task", "analyze", "implement")

        if ($validPhases -notcontains $newPhase) {
            return $false
        }

        # Record current phase in history
        $historyEntry = @{
            "Phase"                 = $this.CurrentPhase
            "CompletedAt"           = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
            "Result"                = $phaseResult
            "MemoryPatternsApplied" = $this.MemoryIntegrationPoints.AppliedPatterns
        }
        $this.PhaseHistory.Add($historyEntry)

        # Create checkpoint before advancing
        $this.CreateCheckpoint()

        # Advance to new phase
        $this.CurrentPhase = $newPhase
        $this.LastUpdated = Get-Date
        $this.Status = "Active"

        return $true
    }

    # Create checkpoint for rollback capability
    [void]CreateCheckpoint() {
        $this.CheckpointData = @{
            "Phase"                          = $this.CurrentPhase
            "Timestamp"                      = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
            "ValidationStatus"               = $this.ValidationStatus.Clone()
            "MemoryIntegrationPoints"        = $this.MemoryIntegrationPoints.Clone()
            "ConstitutionalComplianceStatus" = $this.ConstitutionalComplianceStatus.Clone()
        }
    }

    # Rollback to checkpoint or specific phase
    [bool]RollbackToPhase([string]$targetPhase, [bool]$preserveMemoryPatterns = $true) {
        $validPhases = @("constitution", "specify", "clarify", "plan", "task", "analyze", "implement")

        if ($validPhases -notcontains $targetPhase) {
            return $false
        }

        # Record rollback in history
        $rollbackEntry = @{
            "Phase"                 = "rollback"
            "CompletedAt"           = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
            "Result"                = @{
                "Action"                 = "rollback_to_$targetPhase"
                "Reason"                 = "workflow_reset"
                "PreserveMemoryPatterns" = $preserveMemoryPatterns
            }
            "MemoryPatternsApplied" = if ($preserveMemoryPatterns) { $this.MemoryIntegrationPoints.AppliedPatterns } else { @() }
        }
        $this.PhaseHistory.Add($rollbackEntry)

        # Reset to target phase
        $this.CurrentPhase = $targetPhase
        $this.Status = "Active"

        # Reset validation status
        $this.ValidationStatus = @{
            "ConstitutionalCompliance"   = "pending"
            "MemoryIntegration"          = "pending"
            "CrossPlatformCompatibility" = "pending"
            "PerformanceTargets"         = "pending"
        }

        # Optionally preserve memory patterns
        if (-not $preserveMemoryPatterns) {
            $this.MemoryIntegrationPoints.AppliedPatterns = @()
            $this.MemoryIntegrationPoints.ProcessedFiles = @()
        }

        $this.LastUpdated = Get-Date
        return $true
    }

    # Acquire team collaboration lock
    [bool]AcquireLock([string]$userId, [int]$expirationMinutes = 240) {
        if ($this.TeamCollaborationLock.IsLocked) {
            # Check if lock is expired
            $expiration = [datetime]::Parse($this.TeamCollaborationLock.LockExpiration)
            if ((Get-Date) -lt $expiration) {
                return $false # Lock still active
            }
        }

        # Acquire lock
        $this.TeamCollaborationLock.IsLocked = $true
        $this.TeamCollaborationLock.LockOwner = $userId
        $this.TeamCollaborationLock.LockExpiration = (Get-Date).AddMinutes($expirationMinutes).ToString("yyyy-MM-ddTHH:mm:ssZ")
        $this.LastUpdated = Get-Date

        return $true
    }

    # Release team collaboration lock
    [void]ReleaseLock([string]$userId) {
        if ($this.TeamCollaborationLock.LockOwner -eq $userId -or -not $this.TeamCollaborationLock.IsLocked) {
            $this.TeamCollaborationLock.IsLocked = $false
            $this.TeamCollaborationLock.LockOwner = ""
            $this.TeamCollaborationLock.LockExpiration = ""
            $this.LastUpdated = Get-Date
        }
    }

    # Enable performance degradation mode
    [void]EnablePerformanceDegradation() {
        $this.PerformanceDegradationMode = $true
        $this.LastUpdated = Get-Date
    }

    # Disable performance degradation mode
    [void]DisablePerformanceDegradation() {
        $this.PerformanceDegradationMode = $false
        $this.LastUpdated = Get-Date
    }

    # Update memory integration points
    [void]UpdateMemoryIntegration([string]$filePath, [string[]]$appliedPatterns) {
        if ($this.MemoryIntegrationPoints.ProcessedFiles -notcontains $filePath) {
            $this.MemoryIntegrationPoints.ProcessedFiles += $filePath
        }

        foreach ($pattern in $appliedPatterns) {
            if ($this.MemoryIntegrationPoints.AppliedPatterns -notcontains $pattern) {
                $this.MemoryIntegrationPoints.AppliedPatterns += $pattern
            }
        }

        $this.MemoryIntegrationPoints.LastIntegrationTime = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
        $this.LastUpdated = Get-Date
    }

    # Update validation status
    [void]UpdateValidationStatus([string]$validationType, [string]$status) {
        if ($this.ValidationStatus.ContainsKey($validationType)) {
            $this.ValidationStatus[$validationType] = $status
            $this.LastUpdated = Get-Date
        }
    }

    # Get workflow progress percentage
    [int]GetProgressPercentage() {
        $phases = @("constitution", "specify", "clarify", "plan", "task", "analyze", "implement")
        $currentIndex = $phases.IndexOf($this.CurrentPhase)
        if ($currentIndex -eq -1) { return 0 }

        return [math]::Round(($currentIndex / ($phases.Count - 1)) * 100)
    }

    # Convert to JSON for state persistence
    [hashtable]ToHashtable() {
        return @{
            "WorkflowId"                     = $this.WorkflowId
            "CurrentPhase"                   = $this.CurrentPhase
            "PhaseHistory"                   = $this.PhaseHistory.ToArray()
            "ValidationStatus"               = $this.ValidationStatus
            "MemoryIntegrationPoints"        = $this.MemoryIntegrationPoints
            "ConstitutionalComplianceStatus" = $this.ConstitutionalComplianceStatus
            "CheckpointData"                 = $this.CheckpointData
            "TeamCollaborationLock"          = $this.TeamCollaborationLock
            "PerformanceDegradationMode"     = $this.PerformanceDegradationMode
            "Status"                         = $this.Status
            "CreatedAt"                      = $this.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
            "LastUpdated"                    = $this.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ssZ")
        }
    }

    # Create from JSON/hashtable
    static [WorkflowStateEntity]FromHashtable([hashtable]$data) {
        $entity = [WorkflowStateEntity]::new($data.WorkflowId)
        $entity.CurrentPhase = $data.CurrentPhase
        $entity.PhaseHistory = [System.Collections.ArrayList]::new($data.PhaseHistory)
        $entity.ValidationStatus = $data.ValidationStatus
        $entity.MemoryIntegrationPoints = $data.MemoryIntegrationPoints
        $entity.ConstitutionalComplianceStatus = $data.ConstitutionalComplianceStatus
        $entity.CheckpointData = $data.CheckpointData
        $entity.TeamCollaborationLock = $data.TeamCollaborationLock
        $entity.PerformanceDegradationMode = $data.PerformanceDegradationMode
        $entity.Status = $data.Status
        if ($data.CreatedAt) {
            $entity.CreatedAt = [datetime]::Parse($data.CreatedAt)
        }
        if ($data.LastUpdated) {
            $entity.LastUpdated = [datetime]::Parse($data.LastUpdated)
        }
        return $entity
    }
}

# Factory function for creating workflow state entities
function New-WorkflowStateEntity {
    param(
        [Parameter(Mandatory = $false)]
        [string]$WorkflowId = [guid]::NewGuid().ToString()
    )

    return [WorkflowStateEntity]::new($WorkflowId)
}

# Export functions when run as module
if ($MyInvocation.InvocationName -ne '.') {
    Export-ModuleMember -Function New-WorkflowStateEntity
}
