# Entity: State File (JSON State Schema)
# Purpose: Describes JSON state persistence files and metadata

class StateFileEntity {
    [string] $FileName
    [string] $FilePath
    [string] $Version
    [datetime] $LastModified
    [hashtable] $LockStatus
    [string] $BackupPath
    [hashtable] $CrossPlatformCompatibility

    StateFileEntity() {}
}
