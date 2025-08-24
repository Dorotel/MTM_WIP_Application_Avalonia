# MTM Documentation Cross-Reference Validation Script
# PowerShell script for automated cross-reference validation

param(
    [Parameter(Mandatory=$true)]
    [string]$RepositoryPath,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputFile = "cross-reference-report.json"
)

Write-Host "MTM Documentation Cross-Reference Validation" -ForegroundColor Cyan
Write-Host "=" * 50

# Initialize results
$ValidationResults = @{
    ValidationDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ssZ")
    TotalFiles = 0
    ValidReferences = 0
    BrokenReferences = 0
    MovedReferences = 0
    FileResults = @()
}

function Test-READMECrossReferences {
    param(
        [string]$RepositoryPath,
        [string[]]$READMEFiles
    )
    
    $ValidationResults = @()
    
    foreach ($ReadmeFile in $READMEFiles) {
        Write-Host "Validating cross-references in: $($ReadmeFile.Replace($RepositoryPath, ''))" -ForegroundColor Yellow
        
        # Extract all file references
        $Content = Get-Content $ReadmeFile -Raw -ErrorAction SilentlyContinue
        if (-not $Content) {
            continue
        }
        
        # Find markdown links [text](link)
        $FileReferences = [regex]::Matches($Content, '\[.*?\]\((.*?)\)')
        
        $FileResult = @{
            SourceFile = $ReadmeFile.Replace($RepositoryPath, '')
            References = @()
            BrokenCount = 0
            MovedCount = 0
            ValidCount = 0
        }
        
        foreach ($Reference in $FileReferences) {
            $ReferencedFile = $Reference.Groups[1].Value
            
            # Skip external links
            if ($ReferencedFile.StartsWith("http") -or $ReferencedFile.StartsWith("mailto:")) {
                continue
            }
            
            # Resolve relative path
            $BasePath = Split-Path $ReadmeFile -Parent
            $FullPath = Join-Path $BasePath $ReferencedFile
            
            $ValidationResult = @{
                SourceFile = $ReadmeFile.Replace($RepositoryPath, '')
                ReferencedFile = $ReferencedFile
                ResolvedPath = $FullPath
                Exists = Test-Path $FullPath
                LastModified = if (Test-Path $FullPath) { (Get-Item $FullPath).LastWriteTime } else { $null }
                Status = ""
                Recommendation = ""
            }
            
            # Determine status and recommendation
            if (Test-Path $FullPath) {
                $ValidationResult.Status = "Valid"
                $FileResult.ValidCount++
            } elseif ($ReferencedFile.EndsWith('.instruction.md')) {
                # Check if it's a moved instruction file
                $FileName = Split-Path $ReferencedFile -Leaf
                $NewPaths = @(
                    ".github/Core-Instructions/$FileName",
                    ".github/UI-Instructions/$FileName", 
                    ".github/Development-Instructions/$FileName",
                    ".github/Quality-Instructions/$FileName",
                    ".github/Automation-Instructions/$FileName"
                )
                
                $Found = $false
                foreach ($NewPath in $NewPaths) {
                    $TestPath = Join-Path $RepositoryPath $NewPath
                    if (Test-Path $TestPath) {
                        $ValidationResult.Status = "Moved"
                        $ValidationResult.Recommendation = "Update reference to: $NewPath"
                        $FileResult.MovedCount++
                        $Found = $true
                        break
                    }
                }
                
                if (-not $Found) {
                    $ValidationResult.Status = "Missing"
                    $ValidationResult.Recommendation = "File not found in new organized structure"
                    $FileResult.BrokenCount++
                }
            } else {
                $ValidationResult.Status = "Broken"
                $ValidationResult.Recommendation = "File does not exist at specified path"
                $FileResult.BrokenCount++
            }
            
            $FileResult.References += $ValidationResult
        }
        
        $ValidationResults += $FileResult
    }
    
    return $ValidationResults
}

function Test-HTMLCrossReferences {
    param(
        [string]$RepositoryPath,
        [string[]]$HTMLFiles
    )
    
    $ValidationResults = @()
    
    foreach ($HTMLFile in $HTMLFiles) {
        Write-Host "Validating HTML cross-references in: $($HTMLFile.Replace($RepositoryPath, ''))" -ForegroundColor Yellow
        
        $Content = Get-Content $HTMLFile -Raw -ErrorAction SilentlyContinue
        if (-not $Content) {
            continue
        }
        
        # Find HTML links href="link"
        $Links = [regex]::Matches($Content, 'href="([^"]*)"')
        
        $FileResult = @{
            SourceFile = $HTMLFile.Replace($RepositoryPath, '')
            References = @()
            BrokenCount = 0
            ValidCount = 0
        }
        
        foreach ($Link in $Links) {
            $LinkPath = $Link.Groups[1].Value
            
            # Skip external links and anchors
            if ($LinkPath.StartsWith("http") -or $LinkPath.StartsWith("#")) {
                continue
            }
            
            # Resolve relative paths
            $BasePath = Split-Path $HTMLFile -Parent
            $FullLinkPath = Join-Path $BasePath $LinkPath
            
            $ValidationResult = @{
                SourceFile = $HTMLFile.Replace($RepositoryPath, '')
                Link = $LinkPath
                ResolvedPath = $FullLinkPath
                Exists = Test-Path $FullLinkPath
                Status = if (Test-Path $FullLinkPath) { "Valid" } else { "Broken" }
            }
            
            if (Test-Path $FullLinkPath) {
                $FileResult.ValidCount++
            } else {
                $FileResult.BrokenCount++
            }
            
            $FileResult.References += $ValidationResult
        }
        
        $ValidationResults += $FileResult
    }
    
    return $ValidationResults
}

function Generate-FixRecommendations {
    param($ValidationResults)
    
    $Recommendations = @()
    
    foreach ($FileResult in $ValidationResults) {
        foreach ($Reference in $FileResult.References) {
            if ($Reference.Status -eq "Moved" -and $Reference.Recommendation) {
                $Recommendations += @{
                    Type = "Reference_Update"
                    File = $Reference.SourceFile
                    OldReference = $Reference.ReferencedFile
                    NewReference = $Reference.Recommendation.Replace("Update reference to: ", "")
                    Action = "Update markdown link"
                }
            } elseif ($Reference.Status -eq "Broken") {
                $Recommendations += @{
                    Type = "Broken_Reference"
                    File = $Reference.SourceFile
                    BrokenReference = $Reference.ReferencedFile
                    Action = "Fix or remove broken link"
                }
            }
        }
    }
    
    return $Recommendations
}

# Main execution
try {
    if (-not (Test-Path $RepositoryPath)) {
        throw "Repository path does not exist: $RepositoryPath"
    }
    
    # Find all README files
    $READMEFiles = Get-ChildItem -Path $RepositoryPath -Name "README*.md" -Recurse | ForEach-Object { Join-Path $RepositoryPath $_ }
    Write-Host "Found $($READMEFiles.Count) README files" -ForegroundColor Green
    
    # Find all HTML files
    $HTMLFiles = Get-ChildItem -Path $RepositoryPath -Name "*.html" -Recurse | ForEach-Object { Join-Path $RepositoryPath $_ }
    Write-Host "Found $($HTMLFiles.Count) HTML files" -ForegroundColor Green
    
    # Validate cross-references
    Write-Host "`nValidating README cross-references..." -ForegroundColor Cyan
    $READMEResults = Test-READMECrossReferences -RepositoryPath $RepositoryPath -READMEFiles $READMEFiles
    
    Write-Host "`nValidating HTML cross-references..." -ForegroundColor Cyan
    $HTMLResults = Test-HTMLCrossReferences -RepositoryPath $RepositoryPath -HTMLFiles $HTMLFiles
    
    # Calculate totals
    $TotalValid = ($READMEResults | ForEach-Object { $_.ValidCount } | Measure-Object -Sum).Sum + ($HTMLResults | ForEach-Object { $_.ValidCount } | Measure-Object -Sum).Sum
    $TotalBroken = ($READMEResults | ForEach-Object { $_.BrokenCount } | Measure-Object -Sum).Sum + ($HTMLResults | ForEach-Object { $_.BrokenCount } | Measure-Object -Sum).Sum
    $TotalMoved = ($READMEResults | ForEach-Object { $_.MovedCount } | Measure-Object -Sum).Sum
    
    # Update results
    $ValidationResults.TotalFiles = $READMEFiles.Count + $HTMLFiles.Count
    $ValidationResults.ValidReferences = $TotalValid
    $ValidationResults.BrokenReferences = $TotalBroken
    $ValidationResults.MovedReferences = $TotalMoved
    $ValidationResults.FileResults = @{
        README = $READMEResults
        HTML = $HTMLResults
    }
    
    # Generate fix recommendations
    $AllResults = $READMEResults + $HTMLResults
    $ValidationResults.FixRecommendations = Generate-FixRecommendations -ValidationResults $AllResults
    
    # Save results to JSON
    $ValidationResults | ConvertTo-Json -Depth 10 | Out-File $OutputFile -Encoding UTF8
    
    # Print summary
    Write-Host "`nValidation completed!" -ForegroundColor Green
    Write-Host "Total Files: $($ValidationResults.TotalFiles)" -ForegroundColor White
    Write-Host "Valid References: $($ValidationResults.ValidReferences)" -ForegroundColor Green
    Write-Host "Broken References: $($ValidationResults.BrokenReferences)" -ForegroundColor Red
    Write-Host "Moved References: $($ValidationResults.MovedReferences)" -ForegroundColor Yellow
    
    if ($ValidationResults.BrokenReferences -gt 0 -or $ValidationResults.MovedReferences -gt 0) {
        Write-Host "`nFix recommendations generated: $($ValidationResults.FixRecommendations.Count)" -ForegroundColor Yellow
    }
    
    Write-Host "`nDetailed report saved to: $OutputFile" -ForegroundColor Cyan
    
} catch {
    Write-Error "Validation failed: $($_.Exception.Message)"
    exit 1
}