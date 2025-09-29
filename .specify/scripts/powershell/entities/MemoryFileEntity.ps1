# Entity: Memory File (JSON State Schema)
# Purpose: Implements memory file tracking with integrity and security metadata

class MemoryFileEntity {
    [string] $FilePath
    [string] $FileType
    [string] $ChecksumHash
    [bool] $IsEncrypted
    [int] $PatternCount
    [datetime] $LastModified
    [string[]] $ApplicableCommands

    MemoryFileEntity() {}

    [bool] IsValid() {
        return -not [string]::IsNullOrWhiteSpace($this.FilePath) -and
               -not [string]::IsNullOrWhiteSpace($this.FileType)
    }
}
