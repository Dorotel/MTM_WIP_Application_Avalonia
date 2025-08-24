#!/usr/bin/env pwsh
# Discover-Documentation.ps1
# Find all documentation files throughout the repository

param(
    [string]$ProjectRoot = ".",
    [switch]$Verbose = $false
)

Write-Host "=== MTM WIP Application - Documentation Discovery ===" -ForegroundColor Cyan
Write-Host "Scanning for all documentation files..." -ForegroundColor Yellow

# Define documentation patterns to discover
$DocumentationPatterns = @{
    "README Files" = @{
        Filter = "README*.md"
        Paths = @(".", "Documentation", "Development", "Database_Files", "Services", "ViewModels", "Models", "Extensions")
    }
    "Instruction Files" = @{
        Filter = "*.instruction.md"
        Paths = @(".", ".github", "Development", "Documentation")
    }
    "HTML Documentation" = @{
        Filter = "*.html"
        Paths = @("Documentation", "docs", "HTML")
    }
    "Markdown Documentation" = @{
        Filter = "*.md"
        Paths = @("Documentation", "Development", ".github")
        Exclude = @("README*.md", "*.instruction.md")
    }
    "Configuration Files" = @{
        Filter = @("*.json", "*.yml", "*.yaml")
        Paths = @(".", ".github", "Config")
        Include = @("*config*", "*settings*", "*copilot*", "*prompt*")
    }
}

$TotalFiles = 0
$DocumentationInventory = @()

foreach ($Category in $DocumentationPatterns.Keys) {
    $Config = $DocumentationPatterns[$Category]
    Write-Host "`n=== $Category ===" -ForegroundColor Green
    
    $CategoryFiles = @()
    
    foreach ($SearchPath in $Config.Paths) {
        $FullPath = Join-Path $ProjectRoot $SearchPath
        
        if (-not (Test-Path $FullPath)) {
            if ($Verbose) {
                Write-Host "  ⚠️  Path not found: $FullPath" -ForegroundColor Yellow
            }
            continue
        }
        
        # Handle multiple filters
        $Filters = if ($Config.Filter -is [array]) { $Config.Filter } else { @($Config.Filter) }
        
        foreach ($Filter in $Filters) {
            $Files = Get-ChildItem -Path $FullPath -Filter $Filter -Recurse -ErrorAction SilentlyContinue | Where-Object {
                $file = $_
                $included = $true
                
                # Apply include filter if specified
                if ($Config.Include) {
                    $included = $false
                    foreach ($include in $Config.Include) {
                        if ($file.Name -like $include) {
                            $included = $true
                            break
                        }
                    }
                }
                
                # Apply exclude filter if specified
                if ($included -and $Config.Exclude) {
                    foreach ($exclude in $Config.Exclude) {
                        if ($file.Name -like $exclude) {
                            $included = $false
                            break
                        }
                    }
                }
                
                $included
            }
            
            $CategoryFiles += $Files
        }
    }
    
    # Remove duplicates
    $CategoryFiles = $CategoryFiles | Sort-Object FullName | Get-Unique -AsString
    
    if ($CategoryFiles.Count -eq 0) {
        Write-Host "  No files found in $Category" -ForegroundColor Gray
        continue
    }
    
    foreach ($File in $CategoryFiles) {
        $TotalFiles++
        $RelativePath = $File.FullName.Replace((Resolve-Path $ProjectRoot).Path + [System.IO.Path]::DirectorySeparatorChar, "")
        
        Write-Host "  📄 $($File.Name)" -ForegroundColor White
        if ($Verbose) {
            Write-Host "      Path: $RelativePath" -ForegroundColor Gray
            Write-Host "      Size: $([math]::Round($File.Length / 1KB, 1)) KB" -ForegroundColor Gray
            Write-Host "      Modified: $($File.LastWriteTime.ToString('yyyy-MM-dd HH:mm'))" -ForegroundColor Gray
        }
        
        # Add to inventory
        $DocumentationInventory += [PSCustomObject]@{
            Category = $Category
            FileName = $File.Name
            BaseName = $File.BaseName
            Extension = $File.Extension
            RelativePath = $RelativePath
            FullPath = $File.FullName
            SizeKB = [math]::Round($File.Length / 1KB, 1)
            LastModified = $File.LastWriteTime
            Directory = $File.Directory.Name
        }
    }
    
    Write-Host "  📊 Found $($CategoryFiles.Count) files in $Category" -ForegroundColor Magenta
}

# Analyze documentation organization
Write-Host "`n=== Documentation Organization Analysis ===" -ForegroundColor Cyan

# Check for documentation in expected locations
$ExpectedLocations = @{
    "docs/index.html" = "Main documentation entry point"
    "Documentation/README.md" = "Documentation navigation hub"
    ".github/copilot-instructions.md" = "Copilot instructions"
    "README.md" = "Project root README"
}

Write-Host "Checking expected documentation files:" -ForegroundColor Yellow
foreach ($Location in $ExpectedLocations.Keys) {
    $FullPath = Join-Path $ProjectRoot $Location
    $Exists = Test-Path $FullPath
    $Status = if ($Exists) { "✅" } else { "❌" }
    $Description = $ExpectedLocations[$Location]
    
    Write-Host "  $Status $Location - $Description" -ForegroundColor $(if ($Exists) { "Green" } else { "Red" })
}

# Documentation gaps analysis
Write-Host "`nDocumentation Gaps Analysis:" -ForegroundColor Yellow

$CoreDirectories = @("Services", "ViewModels", "Models", "Extensions")
foreach ($Dir in $CoreDirectories) {
    $DirPath = Join-Path $ProjectRoot $Dir
    if (Test-Path $DirPath) {
        $CsFiles = Get-ChildItem -Path $DirPath -Filter "*.cs" -Recurse
        $ReadmeExists = Test-Path (Join-Path $DirPath "README.md")
        
        Write-Host "  📁 $Dir: $($CsFiles.Count) C# files, README: $(if ($ReadmeExists) { '✅' } else { '❌' })" -ForegroundColor Cyan
    }
}

# Summary report
Write-Host "`n=== Documentation Discovery Summary ===" -ForegroundColor Cyan
Write-Host "📄 Total Documentation Files: $TotalFiles" -ForegroundColor White

$CategoryCounts = $DocumentationInventory | Group-Object Category | ForEach-Object {
    Write-Host "  📋 $($_.Name): $($_.Count) files" -ForegroundColor Cyan
}

# Export detailed inventory
$InventoryPath = Join-Path $ProjectRoot "Documentation/Development/Development Scripts/Verification/documentation-inventory.json"
$DocumentationInventory | ConvertTo-Json -Depth 3 | Out-File -FilePath $InventoryPath -Encoding UTF8
Write-Host "`n📋 Detailed inventory saved to: $InventoryPath" -ForegroundColor Cyan

# Generate recommendations
Write-Host "`n=== Recommendations ===" -ForegroundColor Yellow

$ReadmeFiles = ($DocumentationInventory | Where-Object { $_.Category -eq "README Files" }).Count
$InstructionFiles = ($DocumentationInventory | Where-Object { $_.Category -eq "Instruction Files" }).Count
$HtmlFiles = ($DocumentationInventory | Where-Object { $_.Category -eq "HTML Documentation" }).Count

if ($ReadmeFiles -gt 10) {
    Write-Host "  📝 Consider consolidating $ReadmeFiles README files" -ForegroundColor Cyan
}

if ($InstructionFiles -gt 5) {
    Write-Host "  📋 Consider organizing $InstructionFiles instruction files" -ForegroundColor Cyan
}

if ($HtmlFiles -eq 0) {
    Write-Host "  🌐 Consider creating HTML documentation for better accessibility" -ForegroundColor Cyan
}

Write-Host "`n✅ Documentation Discovery Complete!" -ForegroundColor Green