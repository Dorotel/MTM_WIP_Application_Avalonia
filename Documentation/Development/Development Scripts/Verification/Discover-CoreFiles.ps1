#!/usr/bin/env pwsh
# Discover-CoreFiles.ps1
# Discover all C# core files for documentation

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false
)

Write-Host "=== MTM WIP Application - Core Files Discovery ===" -ForegroundColor Cyan
Write-Host "Scanning for C# core files requiring documentation..." -ForegroundColor Yellow

# Define core file categories
$CoreFileCategories = @{
    "Services" = @{
        Path = "Services"
        Filter = "*.cs"
        Exclude = @("*Test*", "*Temp*")
    }
    "ViewModels" = @{
        Path = "ViewModels" 
        Filter = "*.cs"
        Exclude = @("*Test*", "*Temp*")
    }
    "Models" = @{
        Path = "Models"
        Filter = "*.cs"
        Exclude = @("*Test*", "*Temp*")
    }
    "Extensions" = @{
        Path = "Extensions"
        Filter = "*.cs"
        Exclude = @("*Test*", "*Temp*")
    }
}

$TotalFiles = 0
$MissingPlainDocs = 0
$MissingTechDocs = 0
$DiscoveryReport = @()

foreach ($Category in $CoreFileCategories.Keys) {
    $Config = $CoreFileCategories[$Category]
    $CategoryPath = Join-Path $ProjectRoot $Config.Path
    
    if (-not (Test-Path $CategoryPath)) {
        Write-Host "⚠️  Category path not found: $CategoryPath" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "`n=== $Category Files ===" -ForegroundColor Green
    
    # Get all files in category
    $Files = Get-ChildItem -Path $CategoryPath -Filter $Config.Filter -Recurse | Where-Object {
        $file = $_
        $excluded = $false
        foreach ($exclude in $Config.Exclude) {
            if ($file.Name -like $exclude) {
                $excluded = $true
                break
            }
        }
        -not $excluded
    }
    
    if ($Files.Count -eq 0) {
        Write-Host "  No files found in $Category" -ForegroundColor Gray
        continue
    }
    
    foreach ($File in $Files) {
        $TotalFiles++
        $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
        
        Write-Host "  📄 $($File.Name)" -ForegroundColor White
        if ($Verbose) {
            Write-Host "      Path: $RelativePath" -ForegroundColor Gray
        }
        
        # Check PlainEnglish documentation
        $PlainDocPath = Join-Path $ProjectRoot "docs/PlainEnglish/FileDefinitions/$Category/$($File.BaseName).html"
        $PlainDocExists = Test-Path $PlainDocPath
        
        # Check Technical documentation  
        $TechDocPath = Join-Path $ProjectRoot "docs/Technical/FileDefinitions/$Category/$($File.BaseName).html"
        $TechDocExists = Test-Path $TechDocPath
        
        # Check CoreFiles documentation (legacy)
        $CoreDocPath = Join-Path $ProjectRoot "docs/CoreFiles/$Category/$($File.BaseName).html"
        $CoreDocExists = Test-Path $CoreDocPath
        
        $Status = @()
        if ($PlainDocExists) {
            $Status += "✅ Plain"
        } else {
            $Status += "❌ Plain"
            $MissingPlainDocs++
        }
        
        if ($TechDocExists) {
            $Status += "✅ Tech"
        } else {
            $Status += "❌ Tech"
            $MissingTechDocs++
        }
        
        if ($CoreDocExists) {
            $Status += "✅ Core"
        } else {
            $Status += "❌ Core"
        }
        
        Write-Host "      Documentation: $($Status -join ' | ')" -ForegroundColor Cyan
        
        # Add to report
        $DiscoveryReport += [PSCustomObject]@{
            Category = $Category
            FileName = $File.Name
            BaseName = $File.BaseName
            RelativePath = $RelativePath
            PlainEnglishExists = $PlainDocExists
            TechnicalExists = $TechDocExists
            CoreFilesExists = $CoreDocExists
            PlainEnglishPath = $PlainDocPath
            TechnicalPath = $TechDocPath
            CoreFilesPath = $CoreDocPath
        }
    }
    
    Write-Host "  📊 Found $($Files.Count) files in $Category" -ForegroundColor Magenta
}

# Summary report
Write-Host "`n=== Discovery Summary ===" -ForegroundColor Cyan
Write-Host "📄 Total Core Files: $TotalFiles" -ForegroundColor White
Write-Host "❌ Missing Plain English Docs: $MissingPlainDocs" -ForegroundColor Red
Write-Host "❌ Missing Technical Docs: $MissingTechDocs" -ForegroundColor Red
Write-Host "✅ Documentation Coverage: $([math]::Round((($TotalFiles * 2 - $MissingPlainDocs - $MissingTechDocs) / ($TotalFiles * 2)) * 100, 1))%" -ForegroundColor Green

# Export detailed report
$ReportPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/core-files-discovery-report.json"
$DiscoveryReport | ConvertTo-Json -Depth 3 | Out-File -FilePath $ReportPath -Encoding UTF8
Write-Host "`n📋 Detailed report saved to: $ReportPath" -ForegroundColor Cyan

# Generate missing documentation list
$MissingDocs = @()
foreach ($Item in $DiscoveryReport) {
    if (-not $Item.PlainEnglishExists) {
        $MissingDocs += @{
            Type = "PlainEnglish"
            Category = $Item.Category
            File = $Item.FileName
            RequiredPath = $Item.PlainEnglishPath
        }
    }
    if (-not $Item.TechnicalExists) {
        $MissingDocs += @{
            Type = "Technical"
            Category = $Item.Category
            File = $Item.FileName
            RequiredPath = $Item.TechnicalPath
        }
    }
}

if ($MissingDocs.Count -gt 0) {
    Write-Host "`n=== Next Steps for Documentation Creation ===" -ForegroundColor Yellow
    Write-Host "Run Step 2 (Core C# Documentation) to generate:" -ForegroundColor White
    
    $PlainEnglishMissing = ($MissingDocs | Where-Object { $_.Type -eq "PlainEnglish" }).Count
    $TechnicalMissing = ($MissingDocs | Where-Object { $_.Type -eq "Technical" }).Count
    
    Write-Host "  📝 $PlainEnglishMissing Plain English documentation files" -ForegroundColor Cyan
    Write-Host "  🔧 $TechnicalMissing Technical documentation files" -ForegroundColor Cyan
}

Write-Host "`n✅ Core Files Discovery Complete!" -ForegroundColor Green