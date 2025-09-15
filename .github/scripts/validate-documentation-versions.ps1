# MTM Documentation Version Consistency Validation Script
# Purpose: Validate all documentation files for technology version consistency
# Target: All .github/ markdown files must match MTM_WIP_Application_Avalonia.csproj versions

param(
    [string]$RootPath = ".github",
    [switch]$Fix,
    [switch]$Detailed,
    [string]$ReportFile = "version-validation-report.txt"
)

# Define expected versions from project file analysis
$ExpectedVersions = @{
    ".NET" = @("net8.0", ".NET 8", "8.0")
    "Avalonia" = @("11.3.4")
    "MVVM Community Toolkit" = @("8.3.2")
    "MySQL" = @("9.4.0")
    "MySql.Data" = @("9.4.0")
    "Microsoft.Extensions" = @("9.0.8")
    "CSharp" = @("C# 12", "C#12")
}

# Common incorrect version patterns to detect
$IncorrectPatterns = @{
    ".NET" = @("net6.0", "net7.0", ".NET 6", ".NET 7", "6.0", "7.0")
    "Avalonia" = @("11.0.0", "11.1.0", "11.2.0", "10.x", "11.x")
    "MVVM Community Toolkit" = @("8.0.0", "8.1.0", "8.2.0", "7.x", "8.x")
    "MySQL" = @("8.0.0", "9.0.0", "9.1.0", "9.2.0", "9.3.0", "8.x", "9.x")
    "ReactiveUI" = @("ReactiveUI", "ReactiveObject", "ReactiveCommand")
}

Write-Host "MTM Documentation Version Consistency Validation" -ForegroundColor Green
Write-Host "=" * 50 -ForegroundColor Green

# Get all markdown files in .github directory
$MarkdownFiles = Get-ChildItem -Path $RootPath -Recurse -Filter "*.md" | Where-Object { 
    $_.FullName -notlike "*quarantine*" 
}

Write-Host "Found $($MarkdownFiles.Count) markdown files to validate" -ForegroundColor Yellow

$ValidationResults = @()
$IssuesFound = $false

foreach ($File in $MarkdownFiles) {
    Write-Host "Validating: $($File.Name)" -ForegroundColor Cyan
    
    $Content = Get-Content $File.FullName -Raw -ErrorAction SilentlyContinue
    if (-not $Content) {
        Write-Warning "Could not read file: $($File.FullName)"
        continue
    }
    
    $FileIssues = @()
    
    # Check for incorrect versions
    foreach ($Technology in $IncorrectPatterns.Keys) {
        foreach ($Pattern in $IncorrectPatterns[$Technology]) {
            if ($Content -match [regex]::Escape($Pattern)) {
                $FileIssues += "Incorrect $Technology version found: '$Pattern'"
                $IssuesFound = $true
            }
        }
    }
    
    # Check for ReactiveUI references (should be completely removed)
    if ($Content -match "ReactiveUI|ReactiveObject|ReactiveCommand|this\.RaiseAndSetIfChanged|WhenAnyValue") {
        $FileIssues += "ReactiveUI patterns detected (should use MVVM Community Toolkit)"
        $IssuesFound = $true
    }
    
    # Check for correct version usage
    $CorrectVersionsFound = @{}
    foreach ($Technology in $ExpectedVersions.Keys) {
        $Found = $false
        foreach ($Version in $ExpectedVersions[$Technology]) {
            if ($Content -match [regex]::Escape($Version)) {
                $Found = $true
                break
            }
        }
        $CorrectVersionsFound[$Technology] = $Found
    }
    
    # Create validation result
    $Result = [PSCustomObject]@{
        FilePath = $File.FullName.Replace((Get-Location).Path, "").TrimStart("\", "/")
        Issues = $FileIssues
        HasIssues = $FileIssues.Count -gt 0
        CorrectVersions = $CorrectVersionsFound
        LastModified = $File.LastWriteTime
    }
    
    $ValidationResults += $Result
    
    if ($FileIssues.Count -gt 0) {
        Write-Host "  Issues found: $($FileIssues.Count)" -ForegroundColor Red
        if ($Detailed) {
            foreach ($Issue in $FileIssues) {
                Write-Host "    - $Issue" -ForegroundColor Red
            }
        }
    } else {
        Write-Host "  No issues found" -ForegroundColor Green
    }
}

# Generate summary report
Write-Host "`n" + "=" * 50 -ForegroundColor Green
Write-Host "VALIDATION SUMMARY" -ForegroundColor Green
Write-Host "=" * 50 -ForegroundColor Green

$TotalFiles = $ValidationResults.Count
$FilesWithIssues = ($ValidationResults | Where-Object { $_.HasIssues }).Count
$FilesWithoutIssues = $TotalFiles - $FilesWithIssues

Write-Host "Total files validated: $TotalFiles" -ForegroundColor White
Write-Host "Files with issues: $FilesWithIssues" -ForegroundColor Red
Write-Host "Files without issues: $FilesWithoutIssues" -ForegroundColor Green

if ($IssuesFound) {
    Write-Host "`nDETAILED ISSUES:" -ForegroundColor Red
    Write-Host "-" * 30 -ForegroundColor Red
    
    $FilesWithIssues = $ValidationResults | Where-Object { $_.HasIssues }
    foreach ($FileResult in $FilesWithIssues) {
        Write-Host "`n$($FileResult.FilePath):" -ForegroundColor Yellow
        foreach ($Issue in $FileResult.Issues) {
            Write-Host "  - $Issue" -ForegroundColor Red
        }
    }
}

# Technology coverage analysis
Write-Host "`nTECHNOLOGY VERSION COVERAGE:" -ForegroundColor Blue
Write-Host "-" * 35 -ForegroundColor Blue

foreach ($Technology in $ExpectedVersions.Keys) {
    $FilesWithTechnology = ($ValidationResults | Where-Object { 
        $_.CorrectVersions[$Technology] 
    }).Count
    
    Write-Host "$Technology`: $FilesWithTechnology files" -ForegroundColor Cyan
}

# Generate report file
$ReportContent = @"
MTM Documentation Version Validation Report
Generated: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Root Path: $RootPath

SUMMARY:
- Total files validated: $TotalFiles
- Files with issues: $FilesWithIssues
- Files without issues: $FilesWithoutIssues

EXPECTED VERSIONS:
$(($ExpectedVersions.GetEnumerator() | ForEach-Object { "- $($_.Key): $($_.Value -join ', ')" }) -join "`n")

FILES WITH ISSUES:
$(if ($FilesWithIssues.Count -gt 0) {
    ($FilesWithIssues | ForEach-Object { 
        "`n$($_.FilePath):`n$(($_.Issues | ForEach-Object { "  - $_" }) -join "`n")" 
    }) -join "`n"
} else { "None" })

RECOMMENDATIONS:
1. Update all files with version issues to use exact versions from MTM_WIP_Application_Avalonia.csproj
2. Remove all ReactiveUI references and replace with MVVM Community Toolkit patterns
3. Use specific version numbers rather than approximate versions (11.x, latest, etc.)
4. Validate changes with 'dotnet build' to ensure compatibility

TECHNOLOGY COVERAGE:
$(($ExpectedVersions.Keys | ForEach-Object { 
    $tech = $_
    $count = ($ValidationResults | Where-Object { $_.CorrectVersions[$tech] }).Count
    "- $tech`: $count files"
}) -join "`n")
"@

$ReportContent | Out-File -FilePath $ReportFile -Encoding UTF8
Write-Host "`nReport saved to: $ReportFile" -ForegroundColor Green

# Exit with appropriate code
if ($IssuesFound) {
    Write-Host "`nValidation completed with issues found." -ForegroundColor Red
    exit 1
} else {
    Write-Host "`nValidation completed successfully - no issues found." -ForegroundColor Green
    exit 0
}