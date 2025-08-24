#!/usr/bin/env pwsh
# Validate-Structure.ps1
# Verify directory structure and validate completeness

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false
)

Write-Host "=== MTM WIP Application - Structure Validation ===" -ForegroundColor Cyan
Write-Host "Validating directory structure and completeness..." -ForegroundColor Yellow

# Define expected directory structure
$ExpectedStructure = @{
    "Documentation/Development/Development Documentation" = @(
        "UI_Screenshots", "UI_Documentation", "Examples", "Verification", "Database", "GitHub"
    )
    "Documentation/Development/Development Scripts" = @(
        "Verification", "Automation", "Build", "Deployment", "Utilities"
    )
    "Documentation/Production/Production Documentation" = @(
        "Database"
    )
    "Documentation/Production" = @(
        "Production Scripts"
    )
    "docs" = @(
        "assets", "PlainEnglish", "Technical", "CoreFiles", "NeedsFixing", "Templates", "Components"
    )
    "docs/assets" = @(
        "css", "js"
    )
    "docs/PlainEnglish" = @(
        "Updates", "FileDefinitions"
    )
    "docs/Technical" = @(
        "Updates", "FileDefinitions"
    )
    "docs/CoreFiles" = @(
        "Services", "ViewModels", "Models", "Extensions"
    )
    "docs/NeedsFixing" = @(
        "items"
    )
    "docs/PlainEnglish/FileDefinitions" = @(
        "Services", "ViewModels", "Models", "Extensions"
    )
    "docs/Technical/FileDefinitions" = @(
        "Services", "ViewModels", "Models", "Extensions"
    )
}

$ValidationResults = @()
$TotalDirectories = 0
$MissingDirectories = 0
$ExtraDirectories = @()

# Validate each expected directory
foreach ($ParentPath in $ExpectedStructure.Keys) {
    $FullParentPath = Join-Path $ProjectRoot $ParentPath
    $ExpectedSubdirs = $ExpectedStructure[$ParentPath]
    
    if (-not (Test-Path $FullParentPath)) {
        Write-Host "❌ Missing parent directory: $ParentPath" -ForegroundColor Red
        $ValidationResults += [PSCustomObject]@{
            Path = $ParentPath
            Type = "Parent Directory"
            Status = "Missing"
            Expected = $true
            Message = "Parent directory does not exist"
        }
        $MissingDirectories++
        continue
    }
    
    Write-Host "`n📁 Validating: $ParentPath" -ForegroundColor Green
    
    foreach ($ExpectedSubdir in $ExpectedSubdirs) {
        $TotalDirectories++
        $FullSubdirPath = Join-Path $FullParentPath $ExpectedSubdir
        $RelativeSubdirPath = "$ParentPath/$ExpectedSubdir"
        
        if (Test-Path $FullSubdirPath) {
            Write-Host "  ✅ $ExpectedSubdir" -ForegroundColor Green
            $ValidationResults += [PSCustomObject]@{
                Path = $RelativeSubdirPath
                Type = "Subdirectory"
                Status = "Exists"
                Expected = $true
                Message = "Directory exists as expected"
            }
        } else {
            Write-Host "  ❌ $ExpectedSubdir" -ForegroundColor Red
            $ValidationResults += [PSCustomObject]@{
                Path = $RelativeSubdirPath
                Type = "Subdirectory"
                Status = "Missing"
                Expected = $true
                Message = "Expected directory is missing"
            }
            $MissingDirectories++
        }
    }
    
    # Check for unexpected directories
    if (Test-Path $FullParentPath) {
        $ActualSubdirs = Get-ChildItem -Path $FullParentPath -Directory | ForEach-Object { $_.Name }
        foreach ($ActualSubdir in $ActualSubdirs) {
            if ($ActualSubdir -notin $ExpectedSubdirs) {
                Write-Host "  ⚠️  Unexpected: $ActualSubdir" -ForegroundColor Yellow
                $ExtraDirectories += "$ParentPath/$ActualSubdir"
                $ValidationResults += [PSCustomObject]@{
                    Path = "$ParentPath/$ActualSubdir"
                    Type = "Subdirectory"
                    Status = "Unexpected"
                    Expected = $false
                    Message = "Directory not in expected structure"
                }
            }
        }
    }
}

# Validate required files
Write-Host "`n📄 Validating Required Files..." -ForegroundColor Cyan

$RequiredFiles = @{
    "Documentation/README.md" = "Main documentation navigation hub"
    ".github/copilot-instructions.md" = "Copilot instructions file"
    "README.md" = "Project root README"
    "docs/index.html" = "Main documentation entry point"
}

$MissingFiles = 0
foreach ($FilePath in $RequiredFiles.Keys) {
    $FullFilePath = Join-Path $ProjectRoot $FilePath
    $Description = $RequiredFiles[$FilePath]
    
    if (Test-Path $FullFilePath) {
        Write-Host "  ✅ $FilePath - $Description" -ForegroundColor Green
        $ValidationResults += [PSCustomObject]@{
            Path = $FilePath
            Type = "Required File"
            Status = "Exists"
            Expected = $true
            Message = $Description
        }
    } else {
        Write-Host "  ❌ $FilePath - $Description" -ForegroundColor Red
        $ValidationResults += [PSCustomObject]@{
            Path = $FilePath
            Type = "Required File"
            Status = "Missing"
            Expected = $true
            Message = "Required file: $Description"
        }
        $MissingFiles++
    }
}

# Validate discovery scripts
Write-Host "`n🔍 Validating Discovery Scripts..." -ForegroundColor Cyan

$RequiredScripts = @(
    "Discover-CoreFiles.ps1",
    "Discover-Documentation.ps1", 
    "Discover-Dependencies.ps1",
    "Discover-NeedsFixing.ps1",
    "Validate-Structure.ps1",
    "Generate-FileInventory.ps1"
)

$ScriptsPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification"
$MissingScripts = 0

foreach ($Script in $RequiredScripts) {
    $ScriptPath = Join-Path $ScriptsPath $Script
    if (Test-Path $ScriptPath) {
        Write-Host "  ✅ $Script" -ForegroundColor Green
        $ValidationResults += [PSCustomObject]@{
            Path = "Documentation/Development/Development Scripts/Verification/$Script"
            Type = "Discovery Script"
            Status = "Exists"
            Expected = $true
            Message = "Discovery script is present"
        }
    } else {
        Write-Host "  ❌ $Script" -ForegroundColor Red
        $ValidationResults += [PSCustomObject]@{
            Path = "Documentation/Development/Development Scripts/Verification/$Script"
            Type = "Discovery Script"
            Status = "Missing"
            Expected = $true
            Message = "Required discovery script"
        }
        $MissingScripts++
    }
}

# Check permissions and accessibility
Write-Host "`n🔐 Checking Permissions..." -ForegroundColor Cyan

$CriticalPaths = @(
    "Documentation/Development/Development Scripts",
    "docs",
    "Documentation/Production"
)

foreach ($Path in $CriticalPaths) {
    $FullPath = Join-Path $ProjectRoot $Path
    if (Test-Path $FullPath) {
        try {
            # Test write permissions by creating a temp file
            $TestFile = Join-Path $FullPath ".test-write-permission"
            "test" | Out-File -FilePath $TestFile -ErrorAction Stop
            Remove-Item $TestFile -ErrorAction SilentlyContinue
            Write-Host "  ✅ $Path - Read/Write access" -ForegroundColor Green
        } catch {
            Write-Host "  ⚠️  $Path - Read-only or permission issues" -ForegroundColor Yellow
        }
    }
}

# Validate HTML structure templates
Write-Host "`n🌐 Validating HTML Structure..." -ForegroundColor Cyan

$HtmlDirectories = @(
    "docs/PlainEnglish",
    "docs/Technical", 
    "docs/CoreFiles",
    "docs/NeedsFixing",
    "docs/Components"
)

$HtmlStructureScore = 0
foreach ($HtmlDir in $HtmlDirectories) {
    $FullHtmlPath = Join-Path $ProjectRoot $HtmlDir
    if (Test-Path $FullHtmlPath) {
        $HtmlStructureScore++
        Write-Host "  ✅ $HtmlDir structure exists" -ForegroundColor Green
        
        # Check for index.html in each category
        $IndexPath = Join-Path $FullHtmlPath "index.html"
        if (Test-Path $IndexPath) {
            Write-Host "    ✅ index.html present" -ForegroundColor Green
        } else {
            Write-Host "    ⚠️  index.html missing" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ❌ $HtmlDir structure missing" -ForegroundColor Red
    }
}

# Generate validation summary
Write-Host "`n=== Validation Summary ===" -ForegroundColor Cyan

$ExistingDirectories = $TotalDirectories - $MissingDirectories
$DirectoryCompleteness = [math]::Round(($ExistingDirectories / $TotalDirectories) * 100, 1)

Write-Host "📊 Directory Structure:" -ForegroundColor White
Write-Host "  ✅ Existing: $ExistingDirectories / $TotalDirectories ($DirectoryCompleteness%)" -ForegroundColor $(if ($DirectoryCompleteness -ge 90) { "Green" } elseif ($DirectoryCompleteness -ge 70) { "Yellow" } else { "Red" })
Write-Host "  ❌ Missing: $MissingDirectories directories" -ForegroundColor Red
Write-Host "  ⚠️  Extra: $($ExtraDirectories.Count) unexpected directories" -ForegroundColor Yellow

Write-Host "`n📄 Required Files:" -ForegroundColor White
Write-Host "  ❌ Missing: $MissingFiles critical files" -ForegroundColor Red

Write-Host "`n🔍 Discovery Scripts:" -ForegroundColor White
Write-Host "  ❌ Missing: $MissingScripts required scripts" -ForegroundColor Red

Write-Host "`n🌐 HTML Structure:" -ForegroundColor White
Write-Host "  ✅ Completeness: $HtmlStructureScore / $($HtmlDirectories.Count) categories" -ForegroundColor $(if ($HtmlStructureScore -eq $HtmlDirectories.Count) { "Green" } else { "Yellow" })

# Overall health score
$TotalChecks = $TotalDirectories + $RequiredFiles.Count + $RequiredScripts.Count + $HtmlDirectories.Count
$PassedChecks = $ExistingDirectories + ($RequiredFiles.Count - $MissingFiles) + ($RequiredScripts.Count - $MissingScripts) + $HtmlStructureScore
$OverallHealth = [math]::Round(($PassedChecks / $TotalChecks) * 100, 1)

Write-Host "`n🎯 Overall Structure Health: $OverallHealth%" -ForegroundColor $(if ($OverallHealth -ge 90) { "Green" } elseif ($OverallHealth -ge 70) { "Yellow" } else { "Red" })

# Recommendations
if ($MissingDirectories -gt 0 -or $MissingFiles -gt 0 -or $MissingScripts -gt 0) {
    Write-Host "`n=== Recommendations ===" -ForegroundColor Yellow
    
    if ($MissingDirectories -gt 0) {
        Write-Host "  📁 Create missing directories to complete structure" -ForegroundColor Cyan
    }
    
    if ($MissingFiles -gt 0) {
        Write-Host "  📄 Create missing required files" -ForegroundColor Cyan
    }
    
    if ($MissingScripts -gt 0) {
        Write-Host "  🔍 Complete missing discovery scripts" -ForegroundColor Cyan
    }
    
    if ($ExtraDirectories.Count -gt 0) {
        Write-Host "  🗂️  Review unexpected directories for cleanup" -ForegroundColor Cyan
    }
}

# Export validation report
$ReportPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/structure-validation-report.json"
$Report = @{
    Summary = @{
        TotalDirectories = $TotalDirectories
        ExistingDirectories = $ExistingDirectories
        MissingDirectories = $MissingDirectories
        ExtraDirectories = $ExtraDirectories.Count
        MissingFiles = $MissingFiles
        MissingScripts = $MissingScripts
        OverallHealth = $OverallHealth
        ValidationDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    }
    ValidationResults = $ValidationResults
    ExtraDirectories = $ExtraDirectories
}

$Report | ConvertTo-Json -Depth 4 | Out-File -FilePath $ReportPath -Encoding UTF8
Write-Host "`n📋 Detailed validation report saved to: $ReportPath" -ForegroundColor Cyan

if ($OverallHealth -ge 90) {
    Write-Host "`n✅ Structure Validation Complete - Excellent Health!" -ForegroundColor Green
} elseif ($OverallHealth -ge 70) {
    Write-Host "`n⚠️  Structure Validation Complete - Good Health with Improvements Needed" -ForegroundColor Yellow
} else {
    Write-Host "`n❌ Structure Validation Complete - Critical Issues Need Attention" -ForegroundColor Red
}