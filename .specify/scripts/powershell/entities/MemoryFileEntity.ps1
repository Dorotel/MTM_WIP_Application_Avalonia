# Memory File Entity Model
# Enhanced GitHub Spec Commands with Memory Integration
# Date: September 28, 2025

class MemoryFileEntity {
    [string]$FilePath
    [string]$FileType
    [string]$Content
    [hashtable]$Metadata
    [string[]]$IntegrationTriggers
    [string]$ChecksumHash
    [bool]$EncryptionStatus
    [string[]]$ApplicabilityRules
    [string]$Status
    [datetime]$LastProcessed
    [int]$PatternCount

    # Constructor
    MemoryFileEntity([string]$filePath, [string]$fileType) {
        $this.FilePath = $filePath
        $this.FileType = $fileType
        $this.Status = "Unread"
        $this.EncryptionStatus = $false
        $this.IntegrationTriggers = @()
        $this.ApplicabilityRules = @()
        $this.Metadata = @{
            "CreatedDate"        = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
            "LastModified"       = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
            "PatternCount"       = 0
            "ApplicabilityRules" = @()
        }
        $this.SetIntegrationTriggers($fileType)
    }

    # Set integration triggers based on file type
    [void]SetIntegrationTriggers([string]$fileType) {
        switch ($fileType) {
            "avalonia-ui-memory" {
                $this.IntegrationTriggers = @("specify", "implement", "memory", "help")
            }
            "debugging-memory" {
                $this.IntegrationTriggers = @("clarify", "analyze", "implement", "memory", "help")
            }
            "memory" {
                $this.IntegrationTriggers = @("constitution", "specify", "clarify", "plan", "task", "analyze", "implement", "memory", "validate", "status", "rollback", "help")
            }
            "avalonia-custom-controls-memory" {
                $this.IntegrationTriggers = @("task", "implement", "memory", "help")
            }
        }
    }

    # Load and validate memory file content
    [bool]LoadContent() {
        try {
            if (-not (Test-Path $this.FilePath)) {
                $this.Status = "Failed"
                return $false
            }

            $this.Content = Get-Content $this.FilePath -Raw -Encoding UTF8
            $this.Status = "Loaded"
            $this.LastProcessed = Get-Date
            $this.UpdateChecksum()
            $this.CountPatterns()
            return $true
        }
        catch {
            $this.Status = "Failed"
            return $false
        }
    }

    # Update checksum for integrity validation
    [void]UpdateChecksum() {
        if (-not [string]::IsNullOrEmpty($this.Content)) {
            $hasher = [System.Security.Cryptography.SHA256]::Create()
            $hash = $hasher.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($this.Content))
            $this.ChecksumHash = [System.BitConverter]::ToString($hash) -replace '-', ''
        }
    }

    # Count patterns in memory file
    [void]CountPatterns() {
        if (-not [string]::IsNullOrEmpty($this.Content)) {
            # Count sections starting with ##
            $patterns = ($this.Content -split "`n" | Where-Object { $_ -match "^##\s+" }).Count
            $this.PatternCount = $patterns
            $this.Metadata.PatternCount = $patterns
        }
    }

    # Validate file integrity
    [bool]ValidateIntegrity() {
        if (-not (Test-Path $this.FilePath)) { return $false }

        $currentContent = Get-Content $this.FilePath -Raw -Encoding UTF8
        $hasher = [System.Security.Cryptography.SHA256]::Create()
        $hash = $hasher.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($currentContent))
        $currentHash = [System.BitConverter]::ToString($hash) -replace '-', ''

        return $currentHash -eq $this.ChecksumHash
    }

    # Extract patterns relevant to specific GSC command
    [string[]]ExtractPatternsForCommand([string]$commandName) {
        if ($this.IntegrationTriggers -notcontains $commandName) {
            return @()
        }

        if ([string]::IsNullOrEmpty($this.Content)) {
            $this.LoadContent()
        }

        # Extract relevant sections based on command context
        $patterns = @()
        $lines = $this.Content -split "`n"
        $currentSection = ""
        $inRelevantSection = $false

        foreach ($line in $lines) {
            if ($line -match "^##\s+(.+)") {
                $currentSection = $matches[1]
                $inRelevantSection = $this.IsSectionRelevant($currentSection, $commandName)
            }
            elseif ($inRelevantSection -and $line.Trim() -ne "") {
                $patterns += $line
            }
        }

        return $patterns
    }

    # Check if section is relevant to command
    [bool]IsSectionRelevant([string]$sectionTitle, [string]$commandName) {
        # Basic relevance matching - can be enhanced with more sophisticated rules
        $relevantKeywords = @{
            "specify"   = @("UI", "XAML", "Avalonia", "Specification", "Feature")
            "clarify"   = @("Debug", "Problem", "Issue", "Troubleshoot", "Error")
            "plan"      = @("Architecture", "Design", "Pattern", "Strategy")
            "task"      = @("Control", "Component", "Custom", "Implementation")
            "analyze"   = @("Analysis", "Debug", "Performance", "Review")
            "implement" = @("Implementation", "Code", "Pattern", "Best Practice")
        }

        if ($relevantKeywords.ContainsKey($commandName)) {
            foreach ($keyword in $relevantKeywords[$commandName]) {
                if ($sectionTitle -match $keyword) {
                    return $true
                }
            }
        }

        return $true # Default to include all sections for now
    }

    # Convert to JSON for state persistence
    [hashtable]ToHashtable() {
        return @{
            "FilePath"            = $this.FilePath
            "FileType"            = $this.FileType
            "Content"             = if ($this.Content.Length -gt 1000) { $this.Content.Substring(0, 1000) + "..." } else { $this.Content }
            "Metadata"            = $this.Metadata
            "IntegrationTriggers" = $this.IntegrationTriggers
            "ChecksumHash"        = $this.ChecksumHash
            "EncryptionStatus"    = $this.EncryptionStatus
            "ApplicabilityRules"  = $this.ApplicabilityRules
            "Status"              = $this.Status
            "LastProcessed"       = $this.LastProcessed.ToString("yyyy-MM-ddTHH:mm:ssZ")
            "PatternCount"        = $this.PatternCount
        }
    }

    # Create from JSON/hashtable
    static [MemoryFileEntity]FromHashtable([hashtable]$data) {
        $entity = [MemoryFileEntity]::new($data.FilePath, $data.FileType)
        $entity.Content = $data.Content
        $entity.Metadata = $data.Metadata
        $entity.IntegrationTriggers = $data.IntegrationTriggers
        $entity.ChecksumHash = $data.ChecksumHash
        $entity.EncryptionStatus = $data.EncryptionStatus
        $entity.ApplicabilityRules = $data.ApplicabilityRules
        $entity.Status = $data.Status
        if ($data.LastProcessed) {
            $entity.LastProcessed = [datetime]::Parse($data.LastProcessed)
        }
        $entity.PatternCount = $data.PatternCount
        return $entity
    }
}

# Factory function for creating memory file entities
function New-MemoryFileEntity {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [ValidateSet("avalonia-ui-memory", "debugging-memory", "memory", "avalonia-custom-controls-memory")]
        [string]$FileType
    )

    return [MemoryFileEntity]::new($FilePath, $FileType)
}

# Export functions when run as module
if ($MyInvocation.InvocationName -ne '.') {
    Export-ModuleMember -Function New-MemoryFileEntity
}
