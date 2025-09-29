# State File Entity Model
# Enhanced GitHub Spec Commands with Memory Integration
# Date: September 28, 2025

class StateFileEntity {
    [string]$FileName
    [string]$FilePath
    [hashtable]$Content
    [string]$Version
    [datetime]$LastModified
    [hashtable]$LockStatus
    [string]$BackupPath
    [string]$Status
    [string]$ChecksumHash

    # Constructor
    StateFileEntity([string]$fileName, [string]$filePath) {
        $this.FileName = $fileName
        $this.FilePath = $filePath
        $this.Content = @{}
        $this.Version = "1.0.0"
        $this.LastModified = Get-Date
        $this.LockStatus = @{
            "IsLocked" = $false
            "LockOwner" = ""
            "LockAcquiredAt" = ""
            "LockExpiresAt" = ""
        }
        $this.BackupPath = $filePath -replace "\.json$", ".backup.json"
        $this.Status = "NonExistent"
        $this.ChecksumHash = ""
    }

    # Create new state file
    [bool]Create([hashtable]$initialContent = @{}) {
        try {
            $this.Status = "Creating"
            $this.Content = $initialContent
            $this.UpdateChecksum()

            # Ensure directory exists
            $directory = Split-Path -Path $this.FilePath -Parent
            if (-not (Test-Path $directory)) {
                New-Item -ItemType Directory -Path $directory -Force | Out-Null
            }

            # Write content to file
            $this.WriteToFile()
            $this.Status = "Created"
            return $true
        }
        catch {
            $this.Status = "Failed"
            Write-Error "Failed to create state file: $($_.Exception.Message)"
            return $false
        }
    }

    # Load existing state file
    [bool]Load() {
        try {
            if (-not (Test-Path $this.FilePath)) {
                $this.Status = "NonExistent"
                return $false
            }

            $fileContent = Get-Content $this.FilePath -Raw -Encoding UTF8
            $this.Content = $fileContent | ConvertFrom-Json -AsHashtable
            $this.LastModified = (Get-Item $this.FilePath).LastWriteTime
            $this.UpdateChecksum()
            $this.Status = "Loaded"
            return $true
        }
        catch {
            $this.Status = "Corrupted"
            Write-Error "Failed to load state file: $($_.Exception.Message)"
            return $false
        }
    }

    # Update state file content
    [bool]Update([hashtable]$newContent, [string]$owner) {
        try {
            $this.Status = "Updating"

            # Create backup before updating
            $this.CreateBackup()

            # Update content
            $this.Content = $newContent
            $this.LastModified = Get-Date
            $this.UpdateChecksum()

            # Write to file
            $this.WriteToFile()
            $this.Status = "Updated"
            return $true
        }
        catch {
            $this.Status = "Failed"
            Write-Error "Failed to update state file: $($_.Exception.Message)"
            return $false
        }
    }

    # Create backup of current state
    [bool]CreateBackup() {
        try {
            if (Test-Path $this.FilePath) {
                Copy-Item -Path $this.FilePath -Destination $this.BackupPath -Force
                return $true
            }
            return $false
        }
        catch {
            Write-Error "Failed to create backup: $($_.Exception.Message)"
            return $false
        }
    }

    # Write content to file with cross-platform compatibility
    [void]WriteToFile() {
        $jsonContent = $this.Content | ConvertTo-Json -Depth 10 -Compress:$false

        # Ensure consistent line endings (LF)
        $jsonContent = $jsonContent -replace "`r`n", "`n" -replace "`r", "`n"

        # Write with UTF8 encoding
        [System.IO.File]::WriteAllText($this.FilePath, $jsonContent, [System.Text.Encoding]::UTF8)
    }

    # Update checksum for integrity validation
    [void]UpdateChecksum() {
        if ($this.Content.Count -gt 0) {
            $contentJson = $this.Content | ConvertTo-Json -Depth 10 -Compress
            $hasher = [System.Security.Cryptography.SHA256]::Create()
            $hash = $hasher.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($contentJson))
            $this.ChecksumHash = [System.BitConverter]::ToString($hash) -replace '-', ''
        }
    }

    # Convert to JSON for serialization
    [hashtable]ToHashtable() {
        return @{
            "FileName" = $this.FileName
            "FilePath" = $this.FilePath
            "Content" = $this.Content
            "Version" = $this.Version
            "LastModified" = $this.LastModified.ToString("yyyy-MM-ddTHH:mm:ssZ")
            "Status" = $this.Status
            "ChecksumHash" = $this.ChecksumHash
        }
    }
}

# Factory function for creating state file entities
function New-StateFileEntity {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet("gsc-workflow.json", "validation-status.json", "constitutional-compliance.json", "memory-integration.json")]
        [string]$FileName,

        [Parameter(Mandatory = $false)]
        [string]$BasePath = ".specify/state"
    )

    $filePath = Join-Path $BasePath $FileName
    return [StateFileEntity]::new($FileName, $filePath)
}

# Export functions when run as module
if ($MyInvocation.InvocationName -ne '.') {
    Export-ModuleMember -Function New-StateFileEntity
}
