# MTM WIP Application - File Reference Validation Script
# Generated based on user configuration
# =============================================================================

param(
    [Parameter(Mandatory=$false)]
    [string]$ProjectRoot = ".",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "Documentation/Development/Reports",
    
    [Parameter(Mandatory=$false)]
    [switch]$ShowVerbose
)

# Configuration based on user selections
$Config = @{
    ApplicationFileExtensions = @('.cs', '.axaml', '.csproj', '.sln', '.json', '.config', '.xml')
    DocumentationFileExtensions = @('.md', '.html', '.txt', '.instruction.md')
    PathResolutionMethod = "FROM_PROJECT_ROOT"
    CaseSensitive = $false
    ExcludedDirectories = @('.git', 'bin', 'obj', 'node_modules', 'packages', '.vs', 'Debug', 'Release')
    ValidateUrls = $true
    ValidateAnchors = $true
    ReportFormat = "MARKDOWN"
    GroupBy = "BY_SOURCE_FILE"
    ShowProgress = $true
    UseParallelProcessing = $false  # Disable parallel for now to avoid complexity
    UseCaching = $true
    MaxFileSize = $null
    MTMOptimized = $true
}

# Global variables
$script:FileExistenceCache = @{}
$script:ProcessedFiles = 0
$script:TotalFiles = 0
$script:StartTime = Get-Date

# Reference patterns
$ReferencePatterns = @{
    MarkdownLinks = '\[([^\]]+)\]\(([^)]+)\)'
    MarkdownImages = '!\[([^\]]*)\]\(([^)]+)\)'
    HtmlHref = "href\s*=\s*[`"']([^`"']+)[`"']"
    HtmlSrc = "src\s*=\s*[`"']([^`"']+)[`"']"
}

function Write-ProgressUpdate {
    param([string]$Status, [string]$CurrentFile = "", [int]$PercentComplete = 0)
    
    if ($Config.ShowProgress) {
        Write-Progress -Activity "MTM File Reference Validation" -Status $Status -CurrentOperation $CurrentFile -PercentComplete $PercentComplete
        if ($ShowVerbose) {
            Write-Host "[$($script:ProcessedFiles)/$($script:TotalFiles)] $Status - $CurrentFile" -ForegroundColor Cyan
        }
    }
}

function Test-PathExists {
    param([string]$Path, [string]$BasePath)
    
    $cacheKey = "$BasePath|$Path"
    if ($Config.UseCaching -and $script:FileExistenceCache.ContainsKey($cacheKey)) {
        return $script:FileExistenceCache[$cacheKey]
    }
    
    $resolvedPath = $null
    $exists = $false
    
    try {
        if ([System.IO.Path]::IsPathRooted($Path)) {
            # Absolute path - use as is
            $resolvedPath = $Path
        } else {
            # Relative path - resolve from the source file's directory
            $resolvedPath = Join-Path $BasePath $Path.Replace('/', '\')
            $resolvedPath = [System.IO.Path]::GetFullPath($resolvedPath)
        }
        
        # First, try the resolved path as-is
        $exists = Test-Path $resolvedPath
        
        if (-not $exists) {
            # If the file is in Docs/ folder, also try Documentation/HTML/ structure
            $relativePath = $resolvedPath.Substring($ProjectRoot.Length).TrimStart('\', '/')
            
            if ($relativePath.StartsWith('Docs\', [StringComparison]::OrdinalIgnoreCase)) {
                # Convert Docs path to Documentation/HTML path
                $htmlPath = $relativePath -replace '^Docs\\', 'Documentation\HTML\'
                $alternativeResolvedPath = Join-Path $ProjectRoot $htmlPath
                if (Test-Path $alternativeResolvedPath) {
                    $exists = $true
                    $resolvedPath = $alternativeResolvedPath
                }
            }
            elseif ($relativePath.StartsWith('Documentation\HTML\', [StringComparison]::OrdinalIgnoreCase)) {
                # Convert Documentation/HTML path to Docs path
                $docsPath = $relativePath -replace '^Documentation\\HTML\\', 'Docs\'
                $alternativeResolvedPath = Join-Path $ProjectRoot $docsPath
                if (Test-Path $alternativeResolvedPath) {
                    $exists = $true
                    $resolvedPath = $alternativeResolvedPath
                }
            }
        }
        
        # Case insensitive check for Windows if still not found
        if (-not $exists -and -not $Config.CaseSensitive) {
            $parentDir = Split-Path $resolvedPath -Parent
            $fileName = Split-Path $resolvedPath -Leaf
            
            if (Test-Path $parentDir) {
                $actualFiles = Get-ChildItem $parentDir -File | Where-Object { $_.Name -ieq $fileName }
                if ($actualFiles.Count -gt 0) {
                    $exists = $true
                    $resolvedPath = $actualFiles[0].FullName
                }
            }
        }
    }
    catch {
        $exists = $false
        $resolvedPath = $Path
    }
    
    $result = @{ Exists = $exists; ResolvedPath = $resolvedPath }
    if ($Config.UseCaching) {
        $script:FileExistenceCache[$cacheKey] = $result
    }
    
    return $result
}

function Test-UrlAccessible {
    param([string]$Url)
    
    if (-not $Config.ValidateUrls) {
        return @{ Accessible = $true; StatusCode = "SKIPPED" }
    }
    
    try {
        $response = Invoke-WebRequest -Uri $Url -Method Head -TimeoutSec 10 -UseBasicParsing
        return @{ Accessible = $true; StatusCode = $response.StatusCode }
    }
    catch {
        return @{ Accessible = $false; StatusCode = $_.Exception.Message }
    }
}

function Get-FileReferences {
    param([string]$FilePath, [string]$Content)
    
    $references = @()
    $lines = $Content -split "`r?`n"
    
    foreach ($patternName in $ReferencePatterns.Keys) {
        $pattern = $ReferencePatterns[$patternName]
        
        for ($lineIndex = 0; $lineIndex -lt $lines.Count; $lineIndex++) {
            $line = $lines[$lineIndex]
            $lineNumber = $lineIndex + 1  # Line numbers are 1-based
            
            $matches = [regex]::Matches($line, $pattern, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            
            foreach ($match in $matches) {
                $linkText = if ($match.Groups.Count -gt 2) { $match.Groups[1].Value } else { "" }
                $linkPath = if ($match.Groups.Count -gt 2) { $match.Groups[2].Value } else { $match.Groups[1].Value }
                
                # Skip empty, whitespace-only, or just # paths
                if (-not [string]::IsNullOrWhiteSpace($linkPath) -and $linkPath -ne "#") {
                    $anchor = ""
                    if ($linkPath.Contains('#')) {
                        $parts = $linkPath.Split('#', 2)
                        $linkPath = $parts[0]
                        $anchor = $parts[1]
                        
                        # If the link path becomes empty after removing anchor, keep it as an anchor-only link
                        if ([string]::IsNullOrWhiteSpace($linkPath)) {
                            $linkPath = "#$anchor"
                            $anchor = ""
                        }
                    }
                    
                    $references += @{
                        SourceFile = $FilePath
                        LinkText = $linkText
                        LinkPath = $linkPath
                        Anchor = $anchor
                        PatternType = $patternName
                        LineNumber = $lineNumber
                    }
                }
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
    
    # Skip empty or whitespace-only paths
    if ([string]::IsNullOrWhiteSpace($linkPath)) {
        $result.IsValid = $false
        $result.ErrorType = "EMPTY_PATH"
        $result.ErrorMessage = "Empty or whitespace-only path"
        return $result
    }
    
    # Handle anchor-only links (e.g., #section)
    if ($linkPath.StartsWith('#')) {
        $result.StatusInfo = "Anchor-only link (not validated)"
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
    }
    
    return $result
}

function Get-DocumentationFiles {
    param([string]$RootPath)
    
    $files = @()
    $includePatterns = $Config.DocumentationFileExtensions | ForEach-Object { "*$_" }
    $allFiles = Get-ChildItem -Path $RootPath -Recurse -File -Include $includePatterns
    
    foreach ($file in $allFiles) {
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

function Get-ApplicationFiles {
    param([string]$RootPath)
    
    $files = @()
    $includePatterns = $Config.ApplicationFileExtensions | ForEach-Object { "*$_" }
    $allFiles = Get-ChildItem -Path $RootPath -Recurse -File -Include $includePatterns
    
    foreach ($file in $allFiles) {
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
                    LineNumber = $reference.LineNumber
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
                LineNumber = 0
                IsValid = $false
                ErrorType = "FILE_READ_ERROR"
                ErrorMessage = "Could not read file: $($_.Exception.Message)"
                ResolvedPath = ""
                StatusInfo = ""
            }
        }
    }
    
    Write-ProgressUpdate "Validation complete!" "" 100
    Write-Progress -Activity "MTM File Reference Validation" -Completed
    
    return $allValidationResults
}

function New-ValidationReport {
    param([array]$ValidationResults, [array]$DocumentationFiles, [array]$ApplicationFiles)
    
    $timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $reportFileName = "File_Reference_Validation_Report_$timestamp.md"
    $reportPath = Join-Path $OutputPath $reportFileName
    
    # Ensure output directory exists
    if (-not (Test-Path $OutputPath)) {
        New-Item -Path $OutputPath -ItemType Directory -Force | Out-Null
    }
    
    $groupedResults = $ValidationResults | Group-Object SourceFile
    $brokenReferences = $ValidationResults | Where-Object { -not $_.IsValid }
    $validReferences = $ValidationResults | Where-Object { $_.IsValid }
    
    $totalReferences = $ValidationResults.Count
    $brokenCount = $brokenReferences.Count
    $validCount = $validReferences.Count
    $filesWithErrors = ($brokenReferences | Group-Object SourceFile).Count
    $filesScanned = $DocumentationFiles.Count
    $applicationFilesFound = $ApplicationFiles.Count
    
    # Build report content
    $reportContent = @()
    $reportContent += "# MTM WIP Application - File Reference Validation Report"
    $reportContent += ""
    $reportContent += "**Generated:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    $reportContent += "**Project Root:** $ProjectRoot"
    $reportContent += "**Configuration:** MTM Optimized with Full Performance Settings"
    $reportContent += ""
    $reportContent += "---"
    $reportContent += ""
    $reportContent += "## ?? Executive Summary"
    $reportContent += ""
    $reportContent += "| Metric | Count | Percentage |"
    $reportContent += "|--------|-------|------------|"
    $reportContent += "| **Files Scanned** | $filesScanned | 100% |"
    $reportContent += "| **Total References Found** | $totalReferences | - |"
    $reportContent += "| **? Valid References** | $validCount | $([math]::Round(($validCount / [math]::Max($totalReferences, 1)) * 100, 1))% |"
    $reportContent += "| **? Broken References** | $brokenCount | $([math]::Round(($brokenCount / [math]::Max($totalReferences, 1)) * 100, 1))% |"
    $reportContent += "| **Files with Errors** | $filesWithErrors | $([math]::Round(($filesWithErrors / [math]::Max($filesScanned, 1)) * 100, 1))% |"
    $reportContent += "| **Application Files Found** | $applicationFilesFound | - |"
    $reportContent += ""
    
    if ($brokenCount -eq 0) {
        $reportContent += "## ? Validation Results: ALL REFERENCES VALID"
        $reportContent += ""
        $reportContent += "?? **Congratulations!** All file references in your documentation are valid and point to existing files."
        $reportContent += ""
    } else {
        $reportContent += "## ? Validation Results: ISSUES FOUND"
        $reportContent += ""
        $reportContent += "The following broken references were discovered and require attention:"
        $reportContent += ""
        
        $errorsByType = $brokenReferences | Group-Object ErrorType
        foreach ($errorGroup in $errorsByType) {
            $errorType = $errorGroup.Name
            $errorCount = $errorGroup.Count
            
            $reportContent += "### ?? $errorType ($errorCount issues)"
            $reportContent += ""
            
            $errorsByFile = $errorGroup.Group | Group-Object SourceFile
            foreach ($fileGroup in $errorsByFile) {
                $sourceFile = Get-RelativePath $fileGroup.Name $ProjectRoot
                $reportContent += "#### ?? $sourceFile"
                $reportContent += ""
                
                foreach ($error in $fileGroup.Group) {
                    # Show the exact link as it appears in the source file
                    $linkDisplay = $error.LinkPath
                    if ($error.Anchor) {
                        $linkDisplay += "#$($error.Anchor)"
                    }
                    
                    $reportContent += "- **Link:** $linkDisplay"
                    $reportContent += "  - **Line:** $($error.LineNumber)"
                    $reportContent += "  - **Pattern:** $($error.PatternType)"
                    $reportContent += "  - **Error:** $($error.ErrorMessage)"
                    $reportContent += "  - **Resolved Path:** $($error.ResolvedPath)"
                    $reportContent += ""
                }
            }
        }
    }
    
    $executionTime = (Get-Date) - $script:StartTime
    $reportContent += "---"
    $reportContent += ""
    $reportContent += "## ?? Performance Statistics"
    $reportContent += ""
    $reportContent += "- **Execution Time:** $($executionTime.TotalSeconds.ToString('F2')) seconds"
    $reportContent += "- **Files per Second:** $([math]::Round($filesScanned / [math]::Max($executionTime.TotalSeconds, 1), 2))"
    $reportContent += "- **References per Second:** $([math]::Round($totalReferences / [math]::Max($executionTime.TotalSeconds, 1), 2))"
    $reportContent += "- **Cache Hits:** $($script:FileExistenceCache.Count)"
    $reportContent += ""
    $reportContent += "---"
    $reportContent += ""
    $reportContent += "*Report generated by MTM File Reference Validation Script v1.0*"
    
    # Write report
    $reportContent | Out-File -FilePath $reportPath -Encoding UTF8
    
    return @{
        ReportPath = $reportPath
        TotalReferences = $totalReferences
        BrokenReferences = $brokenCount
        ValidReferences = $validCount
        FilesScanned = $filesScanned
        FilesWithErrors = $filesWithErrors
    }
}

function Get-RelativePath {
    param([string]$Path, [string]$BasePath)
    
    try {
        $fullPath = [System.IO.Path]::GetFullPath($Path)
        $fullBasePath = [System.IO.Path]::GetFullPath($BasePath)
        
        if ($fullPath.StartsWith($fullBasePath, [StringComparison]::OrdinalIgnoreCase)) {
            return $fullPath.Substring($fullBasePath.Length).TrimStart('\', '/')
        } else {
            return $Path
        }
    }
    catch {
        return $Path
    }
}

# Main execution
try {
    Write-Host "?? MTM WIP Application - File Reference Validation Script" -ForegroundColor Green
    Write-Host "=" * 60 -ForegroundColor Green
    
    $ProjectRoot = Resolve-Path $ProjectRoot
    if (-not (Test-Path $ProjectRoot)) {
        throw "Project root directory not found: $ProjectRoot"
    }
    
    Write-Host "?? Project Root: $ProjectRoot" -ForegroundColor Yellow
    Write-Host "?? Configuration: MTM Optimized" -ForegroundColor Yellow
    Write-Host ""
    
    Write-Host "?? Discovering files..." -ForegroundColor Cyan
    $documentationFiles = Get-DocumentationFiles $ProjectRoot
    $applicationFiles = Get-ApplicationFiles $ProjectRoot
    
    Write-Host "   ?? Documentation files found: $($documentationFiles.Count)" -ForegroundColor White
    Write-Host "   ?? Application files found: $($applicationFiles.Count)" -ForegroundColor White
    Write-Host ""
    
    Write-Host "?? Starting validation process..." -ForegroundColor Cyan
    $validationResults = Invoke-ValidationProcess $documentationFiles
    
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
    
    exit $(if ($reportInfo.BrokenReferences -eq 0) { 0 } else { 1 })
}
catch {
    Write-Host ""
    Write-Host "? Script execution failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($ShowVerbose) {
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