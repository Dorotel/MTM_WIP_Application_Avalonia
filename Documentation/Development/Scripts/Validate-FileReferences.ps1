# MTM WIP Application - File Reference Validation Script
# Generated based on user configuration
# =============================================================================

<#
.SYNOPSIS
    Validates all file references in MTM WIP Application documentation files.

.DESCRIPTION
    Comprehensive script that scans all documentation files for references to 
    application files and validates that all paths are correct. Generates a 
    detailed report of broken links and invalid references.

.PARAMETER ProjectRoot
    The root directory of the MTM WIP Application project. Defaults to current directory.

.PARAMETER OutputPath
    Path where the validation report will be saved. Defaults to Documentation/Development/Reports/

.PARAMETER Verbose
    Enables detailed progress output during scanning.

.EXAMPLE
    .\Validate-FileReferences.ps1
    Runs validation with default settings

.EXAMPLE
    .\Validate-FileReferences.ps1 -ProjectRoot "C:\MTM_Project" -Verbose
    Runs validation with custom project root and verbose output
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$ProjectRoot = ".",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "Documentation/Development/Reports",
    
    [Parameter(Mandatory=$false)]
    [switch]$Verbose
)

# Configuration based on user selections
$Config = @{
    # File Types Configuration
    ApplicationFileExtensions = @('.cs', '.axaml', '.csproj', '.sln', '.json', '.config', '.xml')
    DocumentationFileExtensions = @('.md', '.html', '.txt', '.instruction.md')
    
    # Path Resolution: FROM_PROJECT_ROOT
    PathResolutionMethod = "FROM_PROJECT_ROOT"
    
    # Case Sensitivity: WINDOWS_STANDARD (case insensitive)
    CaseSensitive = $false
    
    # Directory Exclusions: EXTENDED_EXCLUSIONS
    ExcludedDirectories = @('.git', 'bin', 'obj', 'node_modules', 'packages', '.vs', 'Debug', 'Release')
    
    # URL Handling: VALIDATE_URLS
    ValidateUrls = $true
    
    # Anchor Validation: BASIC_ANCHOR_VALIDATION
    ValidateAnchors = $true
    
    # Report Format: MARKDOWN
    ReportFormat = "MARKDOWN"
    
    # Error Grouping: BY_SOURCE_FILE
    GroupBy = "BY_SOURCE_FILE"
    
    # Progress Updates: PERCENTAGE_PROGRESS
    ShowProgress = $true
    
    # Performance: FULL_OPTIMIZATION
    UseParallelProcessing = $true
    UseCaching = $true
    
    # File Size Limits: NO_LIMIT
    MaxFileSize = $null
    
    # Special Handling: MTM_OPTIMIZED
    MTMOptimized = $true
}

# Global variables for caching and progress
$script:FileExistenceCache = @{}
$script:ProcessedFiles = 0
$script:TotalFiles = 0
$script:StartTime = Get-Date

# Reference pattern definitions for ALL_PATTERNS
$ReferencePatterns = @{
    MarkdownLinks = '\[([^\]]+)\]\(([^)]+)\)'
    MarkdownImages = '!\[([^\]]*)\]\(([^)]+)\)'
    HtmlHref = "href\s*=\s*[`"']([^`"']+)[`"']"
    HtmlSrc = "src\s*=\s*[`"']([^`"']+)[`"']"
    HtmlAction = "action\s*=\s*[`"']([^`"']+)[`"']"
    HtmlData = "data-[^=]+\s*=\s*[`"']([^`"']+)[`"']"
    PlainTextPaths = '(?:^|\s)([a-zA-Z]:[\\\/](?:[^\\\/\s]+[\\\/])*[^\\\/\s]*\.[a-zA-Z0-9]+)(?:\s|$)'
    RelativePaths = '(?:^|\s)((?:\.\.?[\\\/])+[^\\\/\s]+(?:[\\\/][^\\\/\s]+)*\.[a-zA-Z0-9]+)(?:\s|$)'
}

# MTM-specific patterns and rules
$MTMRules = @{
    InstructionFilePattern = '\.instruction\.md$'
    GitHubFolderPattern = '^\.github[\\\/]'
    DocumentationFolderPattern = '^Documentation[\\\/]'
    DevelopmentFolderPattern = '^Documentation[\\\/]Development[\\\/]'
}

function Write-ProgressUpdate {
    param(
        [string]$Status,
        [string]$CurrentFile = "",
        [int]$PercentComplete = 0
    )
    
    if ($Config.ShowProgress) {
        $elapsed = (Get-Date) - $script:StartTime
        $rate = if ($script:ProcessedFiles -gt 0) { $script:ProcessedFiles / $elapsed.TotalSeconds } else { 0 }
        $eta = if ($rate -gt 0 -and $script:TotalFiles -gt $script:ProcessedFiles) { 
            [TimeSpan]::FromSeconds(($script:TotalFiles - $script:ProcessedFiles) / $rate) 
        } else { 
            [TimeSpan]::Zero 
        }
        
        Write-Progress -Activity "MTM File Reference Validation" `
                      -Status $Status `
                      -CurrentOperation $CurrentFile `
                      -PercentComplete $PercentComplete `
                      -SecondsRemaining $eta.TotalSeconds
        
        if ($Verbose) {
            Write-Host "[$($script:ProcessedFiles)/$($script:TotalFiles)] $Status - $CurrentFile" -ForegroundColor Cyan
        }
    }
}

function Test-PathExists {
    param(
        [string]$Path,
        [string]$BasePath
    )
    
    # Use caching for performance
    $cacheKey = "{0}|{1}" -f $BasePath, $Path
    if ($Config.UseCaching -and $script:FileExistenceCache.ContainsKey($cacheKey)) {
        return $script:FileExistenceCache[$cacheKey]
    }
    
    $resolvedPath = $null
    $exists = $false
    
    try {
        # Path resolution: FROM_PROJECT_ROOT
        if ([System.IO.Path]::IsPathRooted($Path)) {
            $resolvedPath = $Path
        } else {
            $resolvedPath = Join-Path $ProjectRoot $Path.Replace('/', '\')
        }
        
        # Normalize path
        $resolvedPath = [System.IO.Path]::GetFullPath($resolvedPath)
        
        # Case insensitive check for Windows
        if (-not $Config.CaseSensitive) {
            if (Test-Path $resolvedPath) {
                $exists = $true
            } else {
                # Try to find file with different casing
                $parentDir = Split-Path $resolvedPath -Parent
                $fileName = Split-Path $resolvedPath -Leaf
                
                if (Test-Path $parentDir) {
                    $actualFiles = Get-ChildItem $parentDir -File | Where-Object { $_.Name -ieq $fileName }
                    $exists = $actualFiles.Count -gt 0
                }
            }
        } else {
            $exists = Test-Path $resolvedPath
        }
    }
    catch {
        $exists = $false
        $resolvedPath = $Path
    }
    
    # Cache the result
    if ($Config.UseCaching) {
        $script:FileExistenceCache[$cacheKey] = @{
            Exists = $exists
            ResolvedPath = $resolvedPath
        }
    }
    
    return @{
        Exists = $exists
        ResolvedPath = $resolvedPath
    }
}

function Test-UrlAccessible {
    param([string]$Url)
    
    if (-not $Config.ValidateUrls) {
        return @{ Accessible = $true; StatusCode = "SKIPPED" }
    }
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Head -TimeoutSec 10 -UseBasicParsing
        return @{
            Accessible = $true
            StatusCode = $response.StatusCode
        }
    }
    catch {
        return @{
            Accessible = $false
            StatusCode = $_.Exception.Message
        }
    }
}

function Test-AnchorExists {
    param(
        [string]$FilePath,
        [string]$Anchor
    )
    
    if (-not $Config.ValidateAnchors) {
        return $true
    }
    
    try {
        $content = Get-Content $FilePath -Raw -Encoding UTF8
        
        # Check for markdown headings that would generate this anchor
        $anchorPattern = $Anchor.TrimStart('#').Replace('-', '[-\s]*').Replace('_', '[_\s]*')
        $headingPattern = "^#{1,6}\s+.*$anchorPattern.*$"
        
        return $content -match $headingPattern
    }
    catch {
        return $false
    }
}

function Get-FileReferences {
    param(
        [string]$FilePath,
        [string]$Content
    )
    
    $references = @()
    
    foreach ($patternName in $ReferencePatterns.Keys) {
        $pattern = $ReferencePatterns[$patternName]
        $matches = [regex]::Matches($Content, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
        
        foreach ($match in $matches) {
            $linkText = if ($match.Groups.Count -gt 2) { $match.Groups[1].Value } else { "" }
            $linkPath = if ($match.Groups.Count -gt 2) { $match.Groups[2].Value } else { $match.Groups[1].Value }
            
            # Skip empty paths
            if ([string]::IsNullOrWhiteSpace($linkPath)) { continue }
            
            # Parse anchor links
            $anchor = ""
            if ($linkPath.Contains('#')) {
                $parts = $linkPath.Split('#', 2)
                $linkPath = $parts[0]
                $anchor = $parts[1]
            }
            
            $references += @{
                SourceFile = $FilePath
                LinkText = $linkText
                LinkPath = $linkPath
                Anchor = $anchor
                PatternType = $patternName
                FullMatch = $match.Value
                Position = $match.Index
            }
        }
    }
    
    return $references
}

function Test-Reference {
    param($Reference)
    
    $result = @{
        IsValid = $true
        ErrorType = ""
        ErrorMessage = ""
        ResolvedPath = ""
        StatusInfo = ""
    }
    
    $linkPath = $Reference.LinkPath
    
    # Skip empty paths
    if ([string]::IsNullOrWhiteSpace($linkPath)) {
        $result.IsValid = $false
        $result.ErrorType = "EMPTY_PATH"
        $result.ErrorMessage = "Empty or whitespace-only path"
        return $result
    }
    
    # Handle URLs
    if ($linkPath -match '^https?://') {
        $urlTest = Test-UrlAccessible $linkPath
        $result.IsValid = $urlTest.Accessible
        $result.ErrorType = if (-not $urlTest.Accessible) { "URL_INACCESSIBLE" } else { "" }
        $result.ErrorMessage = if (-not $urlTest.Accessible) { "URL not accessible: $($urlTest.StatusCode)" } else { "" }
        $result.StatusInfo = "HTTP Status: $($urlTest.StatusCode)"
        return $result
    }
    
    # Handle mailto and other protocols
    if ($linkPath -match '^[a-zA-Z][a-zA-Z0-9+.-]*:') {
        $result.StatusInfo = "Protocol link (not validated)"
        return $result
    }
    
    # Test file existence
    $pathTest = Test-PathExists $linkPath (Split-Path $Reference.SourceFile -Parent)
    $result.IsValid = $pathTest.Exists
    $result.ResolvedPath = $pathTest.ResolvedPath
    
    if (-not $pathTest.Exists) {
        $result.ErrorType = "FILE_NOT_FOUND"
        $result.ErrorMessage = "File not found: $($pathTest.ResolvedPath)"
        return $result
    }
    
    # Test anchor if present
    if (-not [string]::IsNullOrEmpty($Reference.Anchor)) {
        $anchorExists = Test-AnchorExists $pathTest.ResolvedPath $Reference.Anchor
        if (-not $anchorExists) {
            $result.IsValid = $false
            $result.ErrorType = "ANCHOR_NOT_FOUND"
            $result.ErrorMessage = "Anchor '#$($Reference.Anchor)' not found in file"
        }
    }
    
    return $result
}

function Get-DocumentationFiles {
    param([string]$RootPath)
    
    $files = @()
    
    # Build include patterns
    $includePatterns = $Config.DocumentationFileExtensions | ForEach-Object { "*$_" }
    
    # Get all files recursively
    $allFiles = Get-ChildItem -Path $RootPath -Recurse -File -Include $includePatterns
    
    foreach ($file in $allFiles) {
        # Check if file is in excluded directory
        $relativePath = $file.FullName.Substring($RootPath.Length).TrimStart('\', '/')
        $isExcluded = $false
        
        foreach ($excludedDir in $Config.ExcludedDirectories) {
            if ($relativePath.StartsWith($excludedDir, [StringComparison]::OrdinalIgnoreCase)) {
                $isExcluded = $true
                break
            }
        }
        
        # Apply file size limits if configured
        if ($Config.MaxFileSize -and $file.Length -gt $Config.MaxFileSize) {
            $isExcluded = $true
        }
        
        if (-not $isExcluded) {
            $files += $file
        }
    }
    
    return $files
}

function Get-ApplicationFiles {
    param([string]$RootPath)
    
    $files = @()
    
    # Build include patterns
    $includePatterns = $Config.ApplicationFileExtensions | ForEach-Object { "*$_" }
    
    # Get all files recursively
    $allFiles = Get-ChildItem -Path $RootPath -Recurse -File -Include $includePatterns
    
    foreach ($file in $allFiles) {
        # Check if file is in excluded directory
        $relativePath = $file.FullName.Substring($RootPath.Length).TrimStart('\', '/')
        $isExcluded = $false
        
        foreach ($excludedDir in $Config.ExcludedDirectories) {
            if ($relativePath.StartsWith($excludedDir, [StringComparison]::OrdinalIgnoreCase)) {
                $isExcluded = $true
                break
            }
        }
        
        if (-not $isExcluded) {
            $files += $file
        }
    }
    
    return $files
}

function Invoke-ValidationProcess {
    param([array]$DocumentationFiles)
    
    $allValidationResults = @()
    $script:ProcessedFiles = 0
    $script:TotalFiles = $DocumentationFiles.Count
    
    Write-ProgressUpdate "Starting validation process..." "" 0
    
    if ($Config.UseParallelProcessing -and $DocumentationFiles.Count -gt 10) {
        # Parallel processing for better performance
        $allValidationResults = $DocumentationFiles | ForEach-Object -Parallel {
            $file = $_
            $Config = $using:Config
            $ProjectRoot = $using:ProjectRoot
            $ReferencePatterns = $using:ReferencePatterns
            $MTMRules = $using:MTMRules
            
            # Import functions for parallel execution
            ${function:Get-FileReferences} = $using:function:Get-FileReferences
            ${function:Test-Reference} = $using:function:Test-Reference
            ${function:Test-PathExists} = $using:function:Test-PathExists
            ${function:Test-UrlAccessible} = $using:function:Test-UrlAccessible
            ${function:Test-AnchorExists} = $using:function:Test-AnchorExists
            
            try {
                $content = Get-Content $file.FullName -Raw -Encoding UTF8 -ErrorAction Stop
                $references = Get-FileReferences $file.FullName $content
                
                $fileResults = @()
                foreach ($reference in $references) {
                    $validation = Test-Reference $reference
                    $fileResults += [PSCustomObject]@{
                        SourceFile = $reference.SourceFile
                        LinkText = $reference.LinkText
                        LinkPath = $reference.LinkPath
                        Anchor = $reference.Anchor
                        PatternType = $reference.PatternType
                        IsValid = $validation.IsValid
                        ErrorType = $validation.ErrorType
                        ErrorMessage = $validation.ErrorMessage
                        ResolvedPath = $validation.ResolvedPath
                        StatusInfo = $validation.StatusInfo
                    }
                }
                
                return $fileResults
            }
            catch {
                return @([PSCustomObject]@{
                    SourceFile = $file.FullName
                    LinkText = ""
                    LinkPath = ""
                    Anchor = ""
                    PatternType = "FILE_ERROR"
                    IsValid = $false
                    ErrorType = "FILE_READ_ERROR"
                    ErrorMessage = "Could not read file: $($_.Exception.Message)"
                    ResolvedPath = ""
                    StatusInfo = ""
                })
            }
        } -ThrottleLimit 10
    }
    else {
        # Sequential processing
        foreach ($file in $DocumentationFiles) {
            $script:ProcessedFiles++
            $percentComplete = [math]::Round(($script:ProcessedFiles / $script:TotalFiles) * 100, 1)
            $relativePath = $file.FullName.Substring($ProjectRoot.Length).TrimStart('\', '/')
            
            Write-ProgressUpdate "Processing documentation files..." $relativePath $percentComplete
            
            try {
                $content = Get-Content $file.FullName -Raw -Encoding UTF8 -ErrorAction Stop
                $references = Get-FileReferences $file.FullName $content
                
                foreach ($reference in $references) {
                    $validation = Test-Reference $reference
                    $allValidationResults += [PSCustomObject]@{
                        SourceFile = $reference.SourceFile
                        LinkText = $reference.LinkText
                        LinkPath = $reference.LinkPath
                        Anchor = $reference.Anchor
                        PatternType = $reference.PatternType
                        IsValid = $validation.IsValid
                        ErrorType = $validation.ErrorType
                        ErrorMessage = $validation.ErrorMessage
                        ResolvedPath = $validation.ResolvedPath
                        StatusInfo = $validation.StatusInfo
                    }
                }
            }
            catch {
                $allValidationResults += [PSCustomObject]@{
                    SourceFile = $file.FullName
                    LinkText = ""
                    LinkPath = ""
                    Anchor = ""
                    PatternType = "FILE_ERROR"
                    IsValid = $false
                    ErrorType = "FILE_READ_ERROR"
                    ErrorMessage = "Could not read file: $($_.Exception.Message)"
                    ResolvedPath = ""
                    StatusInfo = ""
                }
            }
        }
    }
    
    Write-ProgressUpdate "Validation complete!" "" 100
    Write-Progress -Activity "MTM File Reference Validation" -Completed
    
    return $allValidationResults
}

function New-ValidationReport {
    param(
        [array]$ValidationResults,
        [array]$DocumentationFiles,
        [array]$ApplicationFiles
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $reportFileName = "File_Reference_Validation_Report_$timestamp.md"
    $reportPath = Join-Path $OutputPath $reportFileName
    
    # Ensure output directory exists
    if (-not (Test-Path $OutputPath)) {
        New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null
    }
    
    # Group results by source file (BY_SOURCE_FILE)
    $groupedResults = $ValidationResults | Group-Object SourceFile
    $brokenReferences = $ValidationResults | Where-Object { -not $_.IsValid }
    $validReferences = $ValidationResults | Where-Object { $_.IsValid }
    
    # Calculate statistics
    $totalReferences = $ValidationResults.Count
    $brokenCount = $brokenReferences.Count
    $validCount = $validReferences.Count
    $filesWithErrors = ($brokenReferences | Group-Object SourceFile).Count
    $filesScanned = $DocumentationFiles.Count
    $applicationFilesFound = $ApplicationFiles.Count
    
    # Generate report content
    $report = @"
# MTM WIP Application - File Reference Validation Report

**Generated:** $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")  
**Project Root:** $ProjectRoot  
**Configuration:** MTM Optimized with Full Performance Settings

---

## ?? Executive Summary

| Metric | Count | Percentage |
|--------|-------|------------|
| **Files Scanned** | $filesScanned | 100% |
| **Total References Found** | $totalReferences | - |
| **? Valid References** | $validCount | $([math]::Round(($validCount / $totalReferences) * 100, 1))% |
| **? Broken References** | $brokenCount | $([math]::Round(($brokenCount / $totalReferences) * 100, 1))% |
| **Files with Errors** | $filesWithErrors | $([math]::Round(($filesWithErrors / $filesScanned) * 100, 1))% |
| **Application Files Found** | $applicationFilesFound | - |

---

## ?? Validation Configuration

- **Application File Types:** .cs, .axaml, .csproj, .sln, .json, .config, .xml
- **Documentation File Types:** .md, .html, .txt, .instruction.md
- **Reference Patterns:** All patterns (Markdown, HTML, Plain Text)
- **Path Resolution:** From Project Root
- **Case Sensitivity:** Windows Standard (case insensitive)
- **URL Validation:** Enabled
- **Anchor Validation:** Basic validation enabled
- **MTM Optimized Rules:** Enabled

---

"@

    if ($brokenCount -eq 0) {
        $report += @"
## ? Validation Results: ALL REFERENCES VALID

?? **Congratulations!** All file references in your documentation are valid and point to existing files.

### ? What This Means:
- All markdown links `[text](path)` are working correctly
- All HTML references (`href`, `src`, etc.) point to valid files
- All anchor links `#section` exist in their target documents
- All URLs are accessible (if URL validation was enabled)
- Your documentation integrity is maintained

### ?? Quality Metrics:
- **Documentation Quality:** Excellent (100% valid references)
- **Maintenance Required:** None
- **User Experience:** Optimal (no broken links)

---

"@
    }
    else {
        $report += @"
## ? Validation Results: ISSUES FOUND

The following broken references were discovered and require attention:

"@

        # Group errors by type for better organization
        $errorsByType = $brokenReferences | Group-Object ErrorType
        
        foreach ($errorGroup in $errorsByType) {
            $errorType = $errorGroup.Name
            $errorCount = $errorGroup.Count
            
            $report += @"

### ?? $errorType ($errorCount issues)

"@
            
            $errorsByFile = $errorGroup.Group | Group-Object SourceFile
            
            foreach ($fileGroup in $errorsByFile) {
                $sourceFile = [System.IO.Path]::GetRelativePath($ProjectRoot, $fileGroup.Name)
                $report += @"

#### ?? $sourceFile

"@
                
                foreach ($error in $fileGroup.Group) {
                    $linkDisplay = if ($error.LinkText) { "$($error.LinkText) ? $($error.LinkPath)" } else { $error.LinkPath }
                    $anchorDisplay = if ($error.Anchor) { "#$($error.Anchor)" } else { "" }
                    
                    $report += @"
- **Link:** $linkDisplay$anchorDisplay
  - **Pattern:** $($error.PatternType)
  - **Error:** $($error.ErrorMessage)
  - **Resolved Path:** $($error.ResolvedPath)

"@
                }
            }
        }
    }

    # Add file-by-file breakdown
    $report += @"

---

## ?? Detailed Breakdown by Source File

"@

    foreach ($fileGroup in $groupedResults) {
        $sourceFile = [System.IO.Path]::GetRelativePath($ProjectRoot, $fileGroup.Name)
        $fileErrors = $fileGroup.Group | Where-Object { -not $_.IsValid }
        $fileValid = $fileGroup.Group | Where-Object { $_.IsValid }
        $fileTotal = $fileGroup.Group.Count
        
        $statusIcon = if ($fileErrors.Count -eq 0) { "?" } else { "?" }
        $statusText = if ($fileErrors.Count -eq 0) { "ALL VALID" } else { "$($fileErrors.Count) ERRORS" }
        
        $report += @"

### $statusIcon $sourceFile
**References:** $fileTotal total ($($fileValid.Count) valid, $($fileErrors.Count) errors)  
**Status:** $statusText

"@
        
        if ($fileErrors.Count -gt 0) {
            $report += @"
**Issues Found:**
"@
            foreach ($error in $fileErrors) {
                $linkDisplay = if ($error.LinkText) { "$($error.LinkText) ? $($error.LinkPath)" } else { $error.LinkPath }
                $anchorDisplay = if ($error.Anchor) { "#$($error.Anchor)" } else { "" }
                
                $report += @"
- ? $linkDisplay$anchorDisplay
  - Error: $($error.ErrorMessage)
  - Type: $($error.ErrorType)

"@
            }
        }
    }

    # Add summary and recommendations
    $report += @"

---

## ?? Recommendations

"@

    if ($brokenCount -eq 0) {
        $report += @"
### ? Maintenance Recommendations
- **Regular Validation:** Run this script monthly to catch new issues early
- **Pre-Commit Checks:** Consider adding reference validation to your CI/CD pipeline
- **Documentation Standards:** Continue following current documentation practices

"@
    }
    else {
        $report += @"
### ?? Priority Actions

#### High Priority (Broken Links)
$(
    $highPriorityErrors = $brokenReferences | Where-Object { $_.ErrorType -in @('FILE_NOT_FOUND', 'URL_INACCESSIBLE') }
    if ($highPriorityErrors.Count -gt 0) {
        "- Fix $($highPriorityErrors.Count) broken file/URL references immediately"
    } else {
        "- No high priority issues found"
    }
)

#### Medium Priority (Anchor Links)
$(
    $anchorErrors = $brokenReferences | Where-Object { $_.ErrorType -eq 'ANCHOR_NOT_FOUND' }
    if ($anchorErrors.Count -gt 0) {
        "- Fix $($anchorErrors.Count) broken anchor links"
    } else {
        "- No anchor link issues found"
    }
)

#### Maintenance
- **Re-run Validation:** After fixing issues, re-run this script to verify fixes
- **Documentation Review:** Review files with multiple errors for systematic issues
- **Link Standardization:** Consider standardizing link formats across documentation

"@
    }

    $executionTime = (Get-Date) - $script:StartTime
    $report += @"

---

## ?? Performance Statistics

- **Execution Time:** $($executionTime.TotalSeconds.ToString("F2")) seconds
- **Files per Second:** $([math]::Round($filesScanned / $executionTime.TotalSeconds, 2))
- **References per Second:** $([math]::Round($totalReferences / $executionTime.TotalSeconds, 2))
- **Cache Hits:** $(if ($Config.UseCaching) { $script:FileExistenceCache.Count } else { "Disabled" })
- **Parallel Processing:** $(if ($Config.UseParallelProcessing) { "Enabled" } else { "Disabled" })

---

## ?? Technical Details

### Scan Configuration
- **Project Root:** $ProjectRoot
- **Output Path:** $OutputPath
- **MTM Optimized:** Yes
- **Case Sensitive:** No (Windows Standard)
- **URL Validation:** $($Config.ValidateUrls)
- **Anchor Validation:** $($Config.ValidateAnchors)

### File Types Scanned
**Documentation Files:**
$($Config.DocumentationFileExtensions | ForEach-Object { "- $_" } | Out-String)

**Application Files Tracked:**
$($Config.ApplicationFileExtensions | ForEach-Object { "- $_" } | Out-String)

### Excluded Directories
$($Config.ExcludedDirectories | ForEach-Object { "- $_" } | Out-String)

---

*Report generated by MTM File Reference Validation Script v1.0*  
*For questions or issues, refer to the MTM WIP Application documentation.*
"@

    # Write report to file
    $report | Out-File -FilePath $reportPath -Encoding UTF8
    
    return @{
        ReportPath = $reportPath
        TotalReferences = $totalReferences
        BrokenReferences = $brokenCount
        ValidReferences = $validCount
        FilesScanned = $filesScanned
        FilesWithErrors = $filesWithErrors
    }
}

# Main execution
try {
    Write-Host "?? MTM WIP Application - File Reference Validation Script" -ForegroundColor Green
    Write-Host "=" * 60 -ForegroundColor Green
    
    # Validate project root
    $ProjectRoot = Resolve-Path $ProjectRoot
    if (-not (Test-Path $ProjectRoot)) {
        throw "Project root directory not found: $ProjectRoot"
    }
    
    Write-Host "?? Project Root: $ProjectRoot" -ForegroundColor Yellow
    Write-Host "?? Configuration: MTM Optimized with Full Performance" -ForegroundColor Yellow
    Write-Host ""
    
    # Discover files
    Write-Host "?? Discovering files..." -ForegroundColor Cyan
    $documentationFiles = Get-DocumentationFiles $ProjectRoot
    $applicationFiles = Get-ApplicationFiles $ProjectRoot
    
    Write-Host "   ?? Documentation files found: $($documentationFiles.Count)" -ForegroundColor White
    Write-Host "   ?? Application files found: $($applicationFiles.Count)" -ForegroundColor White
    Write-Host ""
    
    # Run validation
    Write-Host "?? Starting validation process..." -ForegroundColor Cyan
    $validationResults = Invoke-ValidationProcess $documentationFiles
    
    # Generate report
    Write-Host "?? Generating report..." -ForegroundColor Cyan
    $reportInfo = New-ValidationReport $validationResults $documentationFiles $applicationFiles
    
    Write-Host ""
    Write-Host "? Validation Complete!" -ForegroundColor Green
    Write-Host "=" * 60 -ForegroundColor Green
    Write-Host "?? Results Summary:" -ForegroundColor Yellow
    Write-Host "   Total References: $($reportInfo.TotalReferences)" -ForegroundColor White
    Write-Host "   Valid References: $($reportInfo.ValidReferences)" -ForegroundColor Green
    Write-Host "   Broken References: $($reportInfo.BrokenReferences)" -ForegroundColor $(if ($reportInfo.BrokenReferences -eq 0) { "Green" } else { "Red" })
    Write-Host "   Files Scanned: $($reportInfo.FilesScanned)" -ForegroundColor White
    Write-Host "   Files with Errors: $($reportInfo.FilesWithErrors)" -ForegroundColor $(if ($reportInfo.FilesWithErrors -eq 0) { "Green" } else { "Red" })
    Write-Host ""
    Write-Host "?? Report saved to: $($reportInfo.ReportPath)" -ForegroundColor Cyan
    
    if ($reportInfo.BrokenReferences -eq 0) {
        Write-Host "?? All references are valid! No issues found." -ForegroundColor Green
    } else {
        Write-Host "??  Issues found. Please review the report for details." -ForegroundColor Yellow
    }
    
    # Return exit code
    exit $(if ($reportInfo.BrokenReferences -eq 0) { 0 } else { 1 })
}
catch {
    Write-Host ""
    Write-Host "? Script execution failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($Verbose) {
        Write-Host "Stack Trace:" -ForegroundColor Red
        Write-Host $_.ScriptStackTrace -ForegroundColor Red
    }
    exit 2
}
finally {
    if ($Config.ShowProgress) {
        Write-Progress -Activity "MTM File Reference Validation" -Completed
    }
}