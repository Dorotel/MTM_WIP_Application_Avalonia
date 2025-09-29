# Entity: Validation Script (JSON State Schema)
# Purpose: Tracks validation scripts and results per phase

class ValidationScriptEntity {
    [string] $ScriptName
    [string] $ValidationScope
    [string[]] $MemoryPatternValidation
    [string[]] $ConstitutionalChecks
    [string[]] $CrossPlatformTests
    [hashtable] $PerformanceThresholds
    [string[]] $ErrorHandlingProcedures

    ValidationScriptEntity() {}
}
