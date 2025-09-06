# Theme Completion Script
# PowerShell script to add missing brushes to incomplete MTM theme files

param(
    [string]$ThemesPath = "Resources\Themes",
    [switch]$DryRun = $false
)

Write-Host "🔧 MTM Theme Completion Script" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan

# Standard transaction brushes that need to be added to all themes
# These colors are semantic and should be consistent across all themes
$transactionBrushes = @"

	<!-- Transaction Type Colors for History Panel (WCAG 2.1 AA Compliant) -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionInBrush" Color="#198754"/>      <!-- Green for IN - 4.5:1 contrast -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionOutBrush" Color="#DC3545"/>     <!-- Red for OUT - 4.5:1 contrast -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionTransferBrush" Color="#E67E00"/> <!-- Orange for Transfer - 4.5:1 contrast -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionInLightBrush" Color="#D1E7DD"/>  <!-- Light Green -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionOutLightBrush" Color="#F8D7DA"/> <!-- Light Red -->
	<SolidColorBrush x:Key="MTM_Shared_Logic.TransactionTransferLightBrush" Color="#FFF3CD"/> <!-- Light Orange -->
"@

$themesToFix = @(
    "MTM_Amber",
    "MTM_Blue_Dark", 
    "MTM_Emerald",
    "MTM_Green_Dark",
    "MTM_HighContrast",
    "MTM_Indigo_Dark",
    "MTM_Indigo",
    "MTM_Light_Dark", 
    "MTM_Light",
    "MTM_Orange",
    "MTM_Red_Dark",
    "MTM_Rose_Dark",
    "MTM_Rose",
    "MTM_Teal_Dark",
    "MTM_Teal"
)

function Add-TransactionBrushes {
    param([string]$ThemeFilePath)
    
    $content = Get-Content $ThemeFilePath -Raw
    
    # Check if transaction brushes are already present
    if ($content -match "MTM_Shared_Logic\.TransactionInBrush") {
        return @{ Success = $false; Message = "Transaction brushes already present" }
    }
    
    # Find the insertion point - before the Specialized Theme Colors section
    if ($content -match '(\s*<!-- Specialized Theme Colors -->)') {
        $insertionPoint = $matches[1]
        $updatedContent = $content -replace '(\s*<!-- Specialized Theme Colors -->)', "$transactionBrushes`r`n`r`n`$1"
        
        if (-not $DryRun) {
            Set-Content -Path $ThemeFilePath -Value $updatedContent -Encoding UTF8
        }
        
        return @{ Success = $true; Message = "Transaction brushes added successfully" }
    } else {
        # Fallback: add before the closing ResourceDictionary tag
        if ($content -match '(\s*</ResourceDictionary>)') {
            $updatedContent = $content -replace '(\s*</ResourceDictionary>)', "$transactionBrushes`r`n`$1"
            
            if (-not $DryRun) {
                Set-Content -Path $ThemeFilePath -Value $updatedContent -Encoding UTF8
            }
            
            return @{ Success = $true; Message = "Transaction brushes added at end of file" }
        } else {
            return @{ Success = $false; Message = "Could not find suitable insertion point" }
        }
    }
}

$results = @{
    ProcessedThemes = 0
    SuccessfulThemes = 0
    FailedThemes = 0
    SkippedThemes = 0
    ThemeResults = @()
}

foreach ($themeName in $themesToFix) {
    $themeFilePath = Join-Path $ThemesPath "$themeName.axaml"
    
    if (-not (Test-Path $themeFilePath)) {
        Write-Host "⚠️  Theme file not found: $themeName" -ForegroundColor Yellow
        $results.SkippedThemes++
        continue
    }
    
    $results.ProcessedThemes++
    
    Write-Host "Processing: $themeName" -ForegroundColor Gray
    
    $result = Add-TransactionBrushes $themeFilePath
    
    $themeResult = @{
        ThemeName = $themeName
        FilePath = $themeFilePath
        Success = $result.Success
        Message = $result.Message
        DryRun = $DryRun
    }
    
    $results.ThemeResults += $themeResult
    
    if ($result.Success) {
        $results.SuccessfulThemes++
        $dryRunText = if ($DryRun) { " (DRY RUN)" } else { "" }
        Write-Host "✅ $themeName - $($result.Message)$dryRunText" -ForegroundColor Green
    } else {
        $results.FailedThemes++
        Write-Host "❌ $themeName - $($result.Message)" -ForegroundColor Red
    }
}

# Display summary
Write-Host "`n📊 THEME COMPLETION SUMMARY" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan
Write-Host "Themes processed: $($results.ProcessedThemes)"
Write-Host "Successful: $($results.SuccessfulThemes)" -ForegroundColor Green
Write-Host "Failed: $($results.FailedThemes)" -ForegroundColor Red  
Write-Host "Skipped: $($results.SkippedThemes)" -ForegroundColor Yellow

if ($DryRun) {
    Write-Host "`n🔍 DRY RUN MODE - No files were modified" -ForegroundColor Yellow
    Write-Host "Run without -DryRun to apply changes" -ForegroundColor Yellow
} else {
    Write-Host "`n✅ File modifications completed" -ForegroundColor Green
}

if ($results.SuccessfulThemes -gt 0) {
    Write-Host "`n📋 NEXT STEPS:" -ForegroundColor Cyan
    Write-Host "1. Run theme validation script to confirm completion"
    Write-Host "2. Test build to ensure no syntax errors"  
    Write-Host "3. Remove Design.PreviewWith sections to reduce file size"
    Write-Host "4. Run WCAG contrast validation"
}

# Exit with appropriate code
if ($results.FailedThemes -gt 0) {
    exit 1
} else {
    exit 0
}