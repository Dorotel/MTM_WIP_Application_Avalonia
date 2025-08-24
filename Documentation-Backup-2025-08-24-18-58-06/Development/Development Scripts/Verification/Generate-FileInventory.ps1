#!/usr/bin/env pwsh
# Generate-FileInventory.ps1
# Create comprehensive inventory of all project files

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false,
    [switch]$IncludeBinary = $false
)

Write-Host "=== MTM WIP Application - File Inventory Generation ===" -ForegroundColor Cyan
Write-Host "Creating comprehensive file inventory..." -ForegroundColor Yellow

# Define file categories for organization
$FileCategories = @{
    "Source Code" = @{
        Extensions = @(".cs", ".axaml")
        Directories = @("Services", "ViewModels", "Models", "Extensions", "Views")
        Description = "C# source files and AXAML UI files"
    }
    "Documentation" = @{
        Extensions = @(".md", ".txt")
        Directories = @("Documentation", "docs", ".github")
        Description = "Markdown and text documentation files"
    }
    "Web Content" = @{
        Extensions = @(".html", ".css", ".js")
        Directories = @("docs", "Documentation", "HTML")
        Description = "HTML, CSS, and JavaScript files"
    }
    "Configuration" = @{
        Extensions = @(".json", ".yml", ".yaml", ".xml", ".config")
        Directories = @(".", "Config", ".github")
        Description = "Configuration and settings files"
    }
    "Database" = @{
        Extensions = @(".sql")
        Directories = @("Database_Files", "Documentation/Development/Database_Files")
        Description = "SQL scripts and database files"
    }
    "Scripts" = @{
        Extensions = @(".ps1", ".sh", ".bat", ".cmd")
        Directories = @("Documentation/Development/Development Scripts", "scripts")
        Description = "PowerShell and shell scripts"
    }
    "Project Files" = @{
        Extensions = @(".csproj", ".sln", ".user", ".pubxml")
        Directories = @(".")
        Description = "Visual Studio and MSBuild project files"
    }
    "Binary/Assets" = @{
        Extensions = @(".dll", ".exe", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".zip")
        Directories = @("bin", "obj", "assets", "images")
        Description = "Binary files and image assets"
    }
}

# Exclusion patterns
$ExcludeDirectories = @("bin", "obj", ".vs", ".git", "node_modules", "packages")
$ExcludeFiles = @("*.user", "*.suo", "*.cache", "*~", "Thumbs.db", ".DS_Store")

$FileInventory = @()
$CategoryStats = @{}
$TotalFiles = 0
$TotalSizeBytes = 0

# Initialize category stats
foreach ($Category in $FileCategories.Keys) {
    $CategoryStats[$Category] = @{
        Count = 0
        SizeBytes = 0
        Files = @()
    }
}

Write-Host "`n🔍 Scanning project files..." -ForegroundColor Yellow

# Get all files in project
$AllFiles = Get-ChildItem -Path $ProjectRoot -Recurse -File | Where-Object {
    $file = $_
    $excluded = $false
    
    # Check if file is in excluded directory
    foreach ($excludeDir in $ExcludeDirectories) {
        if ($file.FullName -like "*$([System.IO.Path]::DirectorySeparatorChar)$excludeDir$([System.IO.Path]::DirectorySeparatorChar)*") {
            $excluded = $true
            break
        }
    }
    
    # Check if file matches exclude patterns
    if (-not $excluded) {
        foreach ($excludePattern in $ExcludeFiles) {
            if ($file.Name -like $excludePattern) {
                $excluded = $true
                break
            }
        }
    }
    
    # Exclude binary files unless specifically requested
    if (-not $excluded -and -not $IncludeBinary) {
        $binaryExtensions = @(".dll", ".exe", ".pdb", ".cache", ".tmp")
        if ($file.Extension -in $binaryExtensions) {
            $excluded = $true
        }
    }
    
    -not $excluded
}

Write-Host "Found $($AllFiles.Count) files to categorize..." -ForegroundColor Cyan

foreach ($File in $AllFiles) {
    $TotalFiles++
    $TotalSizeBytes += $File.Length
    
    $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
    $FileExtension = $File.Extension.ToLower()
    $FileDirectory = $File.Directory.Name
    
    if ($Verbose) {
        Write-Host "  📄 Processing: $($File.Name)" -ForegroundColor Gray
    }
    
    # Categorize file
    $Category = "Other"
    $CategoryFound = $false
    
    foreach ($CategoryName in $FileCategories.Keys) {
        $CategoryConfig = $FileCategories[$CategoryName]
        
        # Check by extension
        if ($FileExtension -in $CategoryConfig.Extensions) {
            $Category = $CategoryName
            $CategoryFound = $true
            break
        }
        
        # Check by directory if no extension match
        if (-not $CategoryFound) {
            foreach ($TargetDir in $CategoryConfig.Directories) {
                if ($RelativePath -like "$TargetDir*" -or $RelativePath -like "*$([System.IO.Path]::DirectorySeparatorChar)$TargetDir*") {
                    $Category = $CategoryName
                    $CategoryFound = $true
                    break
                }
            }
        }
        
        if ($CategoryFound) { break }
    }
    
    # Handle "Other" category
    if ($Category -eq "Other") {
        if (-not $CategoryStats.ContainsKey("Other")) {
            $CategoryStats["Other"] = @{
                Count = 0
                SizeBytes = 0
                Files = @()
            }
        }
    }
    
    # Create file record
    $FileRecord = [PSCustomObject]@{
        FileName = $File.Name
        BaseName = $File.BaseName
        Extension = $FileExtension
        RelativePath = $RelativePath
        Directory = $FileDirectory
        Category = $Category
        SizeBytes = $File.Length
        SizeKB = [math]::Round($File.Length / 1KB, 2)
        SizeMB = [math]::Round($File.Length / 1MB, 2)
        LastModified = $File.LastWriteTime
        CreationTime = $File.CreationTime
        IsReadOnly = $File.IsReadOnly
        Attributes = $File.Attributes.ToString()
    }
    
    $FileInventory += $FileRecord
    
    # Update category statistics
    $CategoryStats[$Category].Count++
    $CategoryStats[$Category].SizeBytes += $File.Length
    $CategoryStats[$Category].Files += $FileRecord
}

# Generate statistics and analysis
Write-Host "`n=== File Inventory Results ===" -ForegroundColor Green

Write-Host "`n📊 Category Statistics:" -ForegroundColor Cyan
$SortedCategories = $CategoryStats.Keys | Sort-Object { $CategoryStats[$_].Count } -Descending

foreach ($Category in $SortedCategories) {
    $Stats = $CategoryStats[$Category]
    $SizeMB = [math]::Round($Stats.SizeBytes / 1MB, 2)
    $Percentage = [math]::Round(($Stats.Count / $TotalFiles) * 100, 1)
    
    Write-Host "  📁 $Category`: $($Stats.Count) files ($Percentage%) - $SizeMB MB" -ForegroundColor White
    
    if ($Verbose -and $Stats.Count -gt 0) {
        # Show file type breakdown for this category
        $ExtensionGroups = $Stats.Files | Group-Object Extension | Sort-Object Count -Descending
        foreach ($ExtGroup in $ExtensionGroups) {
            Write-Host "    $($ExtGroup.Name): $($ExtGroup.Count) files" -ForegroundColor Gray
        }
    }
}

# Top directories by file count
Write-Host "`n📂 Top Directories by File Count:" -ForegroundColor Cyan
$DirectoryGroups = $FileInventory | Group-Object Directory | Sort-Object Count -Descending | Select-Object -First 10
foreach ($DirGroup in $DirectoryGroups) {
    Write-Host "  📁 $($DirGroup.Name): $($DirGroup.Count) files" -ForegroundColor White
}

# Largest files
Write-Host "`n📄 Largest Files:" -ForegroundColor Cyan
$LargestFiles = $FileInventory | Sort-Object SizeBytes -Descending | Select-Object -First 10
foreach ($File in $LargestFiles) {
    Write-Host "  📄 $($File.FileName) - $($File.SizeMB) MB ($($File.Category))" -ForegroundColor White
}

# Recently modified files
Write-Host "`n🕒 Recently Modified Files (Last 7 days):" -ForegroundColor Cyan
$RecentFiles = $FileInventory | Where-Object { $_.LastModified -gt (Get-Date).AddDays(-7) } | Sort-Object LastModified -Descending | Select-Object -First 10
foreach ($File in $RecentFiles) {
    Write-Host "  📄 $($File.FileName) - $($File.LastModified.ToString('yyyy-MM-dd HH:mm')) ($($File.Category))" -ForegroundColor White
}

# File extension analysis
Write-Host "`n🔍 File Extension Analysis:" -ForegroundColor Cyan
$ExtensionGroups = $FileInventory | Group-Object Extension | Sort-Object Count -Descending | Select-Object -First 15
foreach ($ExtGroup in $ExtensionGroups) {
    $ExtensionName = if ([string]::IsNullOrEmpty($ExtGroup.Name)) { "(no extension)" } else { $ExtGroup.Name }
    Write-Host "  $ExtensionName`: $($ExtGroup.Count) files" -ForegroundColor White
}

# Summary statistics
$TotalSizeMB = [math]::Round($TotalSizeBytes / 1MB, 2)
$AverageFileSizeKB = [math]::Round(($TotalSizeBytes / $TotalFiles) / 1KB, 2)

Write-Host "`n=== Summary Statistics ===" -ForegroundColor Cyan
Write-Host "📄 Total Files: $TotalFiles" -ForegroundColor White
Write-Host "💾 Total Size: $TotalSizeMB MB" -ForegroundColor White
Write-Host "📏 Average File Size: $AverageFileSizeKB KB" -ForegroundColor White
Write-Host "📁 Categories: $($CategoryStats.Keys.Count)" -ForegroundColor White
Write-Host "📅 Inventory Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White

# Generate change detection baseline
Write-Host "`n🔍 Generating Change Detection Baseline..." -ForegroundColor Yellow

$ChangeDetectionBaseline = @{
    GeneratedDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    TotalFiles = $TotalFiles
    TotalSizeBytes = $TotalSizeBytes
    FileHashes = @{}
    CategoryCounts = @{}
}

# Generate file hashes for change detection
foreach ($Category in $CategoryStats.Keys) {
    $ChangeDetectionBaseline.CategoryCounts[$Category] = $CategoryStats[$Category].Count
}

# Create hash for each file (for important categories only)
$ImportantCategories = @("Source Code", "Documentation", "Configuration", "Database", "Scripts")
$FilesToHash = $FileInventory | Where-Object { $_.Category -in $ImportantCategories }

Write-Host "Generating hashes for $($FilesToHash.Count) important files..." -ForegroundColor Cyan
foreach ($File in $FilesToHash) {
    try {
        $FullPath = Join-Path $ProjectRoot $File.RelativePath
        $Hash = Get-FileHash -Path $FullPath -Algorithm SHA256 -ErrorAction SilentlyContinue
        if ($Hash) {
            $ChangeDetectionBaseline.FileHashes[$File.RelativePath] = @{
                Hash = $Hash.Hash
                LastModified = $File.LastModified
                SizeBytes = $File.SizeBytes
            }
        }
    } catch {
        if ($Verbose) {
            Write-Host "    ⚠️  Could not hash: $($File.RelativePath)" -ForegroundColor Yellow
        }
    }
}

# Export reports
$InventoryPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/file-inventory-report.json"
$BaselinePath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/change-detection-baseline.json"

$InventoryReport = @{
    Summary = @{
        TotalFiles = $TotalFiles
        TotalSizeBytes = $TotalSizeBytes
        TotalSizeMB = $TotalSizeMB
        AverageFileSizeKB = $AverageFileSizeKB
        CategoryCount = $CategoryStats.Keys.Count
        GeneratedDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    }
    CategoryStatistics = $CategoryStats
    FileInventory = $FileInventory
    LargestFiles = $LargestFiles
    RecentFiles = $RecentFiles
    TopDirectories = $DirectoryGroups
    ExtensionAnalysis = $ExtensionGroups
}

$InventoryReport | ConvertTo-Json -Depth 4 | Out-File -FilePath $InventoryPath -Encoding UTF8
$ChangeDetectionBaseline | ConvertTo-Json -Depth 3 | Out-File -FilePath $BaselinePath -Encoding UTF8

Write-Host "`n📋 File inventory report saved to: $InventoryPath" -ForegroundColor Cyan
Write-Host "🔍 Change detection baseline saved to: $BaselinePath" -ForegroundColor Cyan

Write-Host "`n✅ File Inventory Generation Complete!" -ForegroundColor Green