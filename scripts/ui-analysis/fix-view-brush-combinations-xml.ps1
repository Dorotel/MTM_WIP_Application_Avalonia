#!/usr/bin/env pwsh
# =============================================================================
# MTM Context-Aware Brush Combination Fixer (XML Parser)
# Uses XML parsing to understand element hierarchy and context
# Fixes brush combinations based on parent-child relationships
# =============================================================================

param(
    [string]$RootPath = ".",
    [string]$OutputPath = "context-aware-brush-fixes-report.json",
    [switch]$WhatIf,
    [switch]$VerboseOutput,
    [bool]$BackupFiles = $true,
    [string]$TargetFile = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Define context-aware brush rules (for future expansion)
# Currently using algorithmic detection of dark/light backgrounds

# Brushes that indicate dark backgrounds
$DARK_BACKGROUND_BRUSHES = @(
    "MTM_Shared_Logic.PrimaryAction",
    "MTM_Shared_Logic.SidebarGradientBrush",
    "MTM_Shared_Logic.DarkNavigationBrush",
    "MTM_Shared_Logic.AccentBrush"
)

# Brushes that indicate light backgrounds
$LIGHT_BACKGROUND_BRUSHES = @(
    "MTM_Shared_Logic.MainBackground",
    "MTM_Shared_Logic.CardBackgroundBrush",
    "MTM_Shared_Logic.PanelBackgroundBrush",
    "MTM_Shared_Logic.ContentBackground"
)

function Write-Status($Message, $Color = "White") {
    Write-Host "🔧 $Message" -ForegroundColor $Color
}

function Write-Success($Message) {
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Warning($Message) {
    Write-Host "⚠️  $Message" -ForegroundColor Yellow
}

function Write-Error($Message) {
    Write-Host "❌ $Message" -ForegroundColor Red
}

function Write-Fix($Message) {
    Write-Host "🔨 $Message" -ForegroundColor Cyan
}

function Find-AxamlFiles($Path) {
    Write-Status "Scanning for AXAML files in: $Path"
    
    $axamlFiles = @(Get-ChildItem -Path $Path -Recurse -Filter "*.axaml" -File | 
                  Where-Object { $_.Name -notlike "*.g.axaml" })  # Exclude generated files
    
    Write-Status "Found $($axamlFiles.Count) AXAML files"
    return $axamlFiles
}

function Get-ElementContext($Element) {
    $context = @{
        ElementName = $Element.Name
        Classes = $null
        Background = $null
        IsPrimaryButton = $false
        IsSecondaryButton = $false
        HasDarkBackground = $false
        HasLightBackground = $false
        ParentContext = $null
    }
    
    # Check for classes attribute
    if ($Element.HasAttribute("Classes")) {
        $context.Classes = $Element.GetAttribute("Classes")
        if ($context.Classes -like "*primary*") {
            $context.IsPrimaryButton = $true
        }
        if ($context.Classes -like "*secondary*") {
            $context.IsSecondaryButton = $true
        }
    }
    
    # Check for background attribute
    if ($Element.HasAttribute("Background")) {
        $backgroundValue = $Element.GetAttribute("Background")
        $context.Background = $backgroundValue
        
        # Extract brush name from DynamicResource
        if ($backgroundValue -match 'DynamicResource\s+([^}]+)') {
            $brushName = $matches[1].Trim()
            if ($DARK_BACKGROUND_BRUSHES -contains $brushName) {
                $context.HasDarkBackground = $true
            } elseif ($LIGHT_BACKGROUND_BRUSHES -contains $brushName) {
                $context.HasLightBackground = $true
            }
        }
    }
    
    return $context
}

function Get-AncestorWithDarkBackground($Element) {
    $current = $Element.ParentNode
    
    while ($current -and $current.NodeType -eq [System.Xml.XmlNodeType]::Element) {
        $context = Get-ElementContext $current
        
        # Check if this ancestor is a primary button
        if ($context.IsPrimaryButton) {
            return @{
                Type = "PrimaryButton"
                Element = $current
                Reason = "Inside primary button with dark background"
            }
        }
        
        # Check if this ancestor has a dark background
        if ($context.HasDarkBackground) {
            return @{
                Type = "DarkBackground"
                Element = $current
                Background = $context.Background
                Reason = "Inside element with dark background: $($context.Background)"
            }
        }
        
        $current = $current.ParentNode
    }
    
    return $null
}

function Test-ShouldUseOverlayText($Element) {
    # Check if element is inside a context that requires overlay text
    $darkAncestor = Get-AncestorWithDarkBackground $Element
    
    if ($darkAncestor) {
        return @{
            ShouldFix = $true
            Reason = $darkAncestor.Reason
            TargetBrush = "MTM_Shared_Logic.OverlayTextBrush"
        }
    }
    
    return @{
        ShouldFix = $false
        Reason = "No dark background ancestor found"
        TargetBrush = $null
    }
}

function Repair-ElementBrush($Element, $FileName) {
    $fixes = @()
    
    # Only process elements that can have foreground
    if ($Element.Name -notin @("MaterialIcon", "TextBlock", "Button")) {
        return $fixes
    }
    
    # Skip if element doesn't have foreground attribute
    if (-not $Element.HasAttribute("Foreground")) {
        return $fixes
    }
    
    $currentForeground = $Element.GetAttribute("Foreground")
    
    # Skip if not using DynamicResource
    if ($currentForeground -notlike "*DynamicResource*") {
        return $fixes
    }
    
    # Extract current brush name
    if ($currentForeground -match 'DynamicResource\s+([^}]+)') {
        $currentBrush = $matches[1].Trim()
    } else {
        return $fixes
    }
    
    # Check if this element should use overlay text
    $overlayCheck = Test-ShouldUseOverlayText $Element
    
    if ($overlayCheck.ShouldFix) {
        # Only fix if current brush is not already OverlayTextBrush
        if ($currentBrush -ne "MTM_Shared_Logic.OverlayTextBrush") {
            $fix = @{
                FileName = $FileName
                ElementName = $Element.Name
                XPath = Get-ElementXPath $Element
                CurrentBrush = $currentBrush
                NewBrush = $overlayCheck.TargetBrush
                Reason = $overlayCheck.Reason
                Element = $Element
            }
            $fixes += $fix
            
            if ($VerboseOutput) {
                Write-Fix "$FileName - $($Element.Name): $currentBrush → $($overlayCheck.TargetBrush)"
                Write-Host "   Reason: $($overlayCheck.Reason)" -ForegroundColor Gray
            }
        }
    }
    
    return $fixes
}

function Get-ElementXPath($Element) {
    $path = ""
    $current = $Element
    
    while ($current -and $current.NodeType -eq [System.Xml.XmlNodeType]::Element) {
        $index = 1
        $sibling = $current.PreviousSibling
        
        while ($sibling) {
            if ($sibling.NodeType -eq [System.Xml.XmlNodeType]::Element -and $sibling.Name -eq $current.Name) {
                $index++
            }
            $sibling = $sibling.PreviousSibling
        }
        
        $nodeName = $current.Name
        if ($current.HasAttribute("x:Name")) {
            $nodeName += "[@x:Name='" + $current.GetAttribute("x:Name") + "']"
        } elseif ($index -gt 1) {
            $nodeName += "[$index]"
        }
        
        $path = "/" + $nodeName + $path
        $current = $current.ParentNode
    }
    
    return $path
}

function Invoke-AxamlFileProcessing($FilePath) {
    Write-Status "Processing: $(Split-Path $FilePath -Leaf)"
    
    $fixes = @()
    
    try {
        # Load XML with namespace handling
        $xmlDoc = New-Object System.Xml.XmlDocument
        $xmlDoc.PreserveWhitespace = $true
        
        # Read content and handle namespaces
        $content = Get-Content $FilePath -Raw
        $xmlDoc.LoadXml($content)
        
        # Create namespace manager for xpath queries
        $nsManager = New-Object System.Xml.XmlNamespaceManager($xmlDoc.NameTable)
        $nsManager.AddNamespace("avalonia", "https://github.com/avaloniaui")
        $nsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml")
        $nsManager.AddNamespace("materialIcons", "using:Material.Icons.Avalonia")
        
        # Find all elements that might need brush fixes
        $elementsToCheck = @()
        
        # Get all MaterialIcons and TextBlocks
        $elementsToCheck += $xmlDoc.SelectNodes("//materialIcons:MaterialIcon[@Foreground]", $nsManager)
        $elementsToCheck += $xmlDoc.SelectNodes("//TextBlock[@Foreground]", $nsManager)
        
        # Handle elements without namespace prefix
        $elementsToCheck += $xmlDoc.SelectNodes("//*[local-name()='MaterialIcon' and @Foreground]")
        $elementsToCheck += $xmlDoc.SelectNodes("//*[local-name()='TextBlock' and @Foreground]")
        
        Write-Status "Found $($elementsToCheck.Count) elements with Foreground attributes"
        
        foreach ($element in $elementsToCheck) {
            if ($element) {
                $elementFixes = Repair-ElementBrush $element (Split-Path $FilePath -Leaf)
                $fixes += $elementFixes
            }
        }
        
        # Apply fixes if any were found
        if ($fixes.Count -gt 0 -and -not $WhatIf) {
            # Create backup
            if ($BackupFiles) {
                $backupPath = $FilePath + ".backup." + (Get-Date -Format "yyyyMMdd-HHmmss")
                Copy-Item $FilePath $backupPath
            }
            
            # Apply each fix
            foreach ($fix in $fixes) {
                $newForegroundValue = '{DynamicResource ' + $fix.NewBrush + '}'
                $fix.Element.SetAttribute("Foreground", $newForegroundValue)
            }
            
            # Save the modified XML
            $xmlDoc.Save($FilePath)
            Write-Success "Applied $($fixes.Count) fixes to $(Split-Path $FilePath -Leaf)"
        } elseif ($fixes.Count -gt 0 -and $WhatIf) {
            Write-Host "WHAT-IF: Would apply $($fixes.Count) fixes to $(Split-Path $FilePath -Leaf)" -ForegroundColor Yellow
        }
        
        return $fixes
        
    } catch {
        Write-Error "Failed to process $FilePath`: $($_.Exception.Message)"
        return @()
    }
}

function Main {
    Write-Host ""
    Write-Host "🔨 MTM CONTEXT-AWARE BRUSH FIXER" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    Write-Host ""

    if ($WhatIf) {
        Write-Warning "RUNNING IN WHAT-IF MODE - No changes will be made"
        Write-Host ""
    }

    # Find all AXAML files
    $axamlFiles = Find-AxamlFiles $RootPath
    
    if ($axamlFiles.Count -eq 0) {
        Write-Warning "No AXAML files found in $RootPath"
        return
    }
    
    # Apply target file filter if specified
    if ($TargetFile) {
        $axamlFiles = @($axamlFiles | Where-Object { $_.Name -like "*$TargetFile*" })
        if ($axamlFiles.Count -eq 0) {
            Write-Warning "No AXAML files found matching '$TargetFile'"
            return
        }
        Write-Status "Filtering to target file: $TargetFile ($($axamlFiles.Count) files)"
    }

    Write-Host ""
    Write-Host "🎯 FILES TO PROCESS:" -ForegroundColor Yellow
    Write-Host "====================" -ForegroundColor Yellow
    foreach ($file in $axamlFiles) {
        $relativePath = $file.FullName.Replace((Get-Location).Path, "").TrimStart('\', '/')
        Write-Host "  • $relativePath" -ForegroundColor Yellow
    }
    Write-Host ""

    # Process each AXAML file
    $allFixes = @()
    $processedFiles = 0
    $filesWithFixes = 0

    Write-Host "🔧 PROCESSING FILES:" -ForegroundColor Cyan
    Write-Host "===================" -ForegroundColor Cyan
    Write-Host ""

    foreach ($file in $axamlFiles) {
        $processedFiles++
        
        try {
            $fixes = Invoke-AxamlFileProcessing $file.FullName
            
            if ($fixes -and $fixes.Count -gt 0) {
                $allFixes += $fixes
                $filesWithFixes++
            }
        } catch {
            Write-Error "Failed to process $($file.Name): $($_.Exception.Message)"
        }
        
        if ($VerboseOutput) {
            Write-Host ""
        }
    }

    # Generate summary report
    $summary = @{
        FixDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        Mode = if ($WhatIf) { "What-If" } else { "Applied" }
        ProcessedFiles = $processedFiles
        FilesWithFixes = $filesWithFixes
        TotalFixes = if ($allFixes) { $allFixes.Count } else { 0 }
        BackupsCreated = if ($BackupFiles -and -not $WhatIf) { $filesWithFixes } else { 0 }
        FixDetails = $allFixes | ForEach-Object { 
            @{
                FileName = $_.FileName
                ElementName = $_.ElementName
                XPath = $_.XPath
                CurrentBrush = $_.CurrentBrush
                NewBrush = $_.NewBrush
                Reason = $_.Reason
            }
        }
    }

    # Export results
    $summary | ConvertTo-Json -Depth 10 | Out-File $OutputPath -Encoding UTF8
    
    # Display final summary
    Write-Host ""
    Write-Host "📊 CONTEXT-AWARE BRUSH FIXES SUMMARY" -ForegroundColor Cyan
    Write-Host "====================================" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Files processed: $processedFiles" -ForegroundColor White
    Write-Host "Files with fixes: $filesWithFixes" -ForegroundColor Green
    Write-Host "Total brush fixes: $(if ($allFixes) { $allFixes.Count } else { 0 })" -ForegroundColor Green
    
    if ($BackupFiles -and -not $WhatIf) {
        Write-Host "Backup files created: $($summary.BackupsCreated)" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Status "Detailed fix report saved to: $OutputPath"
    
    if ($allFixes -and $allFixes.Count -gt 0) {
        Write-Host ""
        Write-Host "🔍 TOP FIXES APPLIED:" -ForegroundColor Yellow
        $topFixes = $allFixes | Group-Object { "$($_.CurrentBrush) → $($_.NewBrush)" } | 
                   Sort-Object Count -Descending | Select-Object -First 5
        
        foreach ($fixGroup in $topFixes) {
            Write-Host "  • $($fixGroup.Name): $($fixGroup.Count) fixes" -ForegroundColor Yellow
        }
        
        Write-Host ""
        Write-Host "📋 FILES MODIFIED:" -ForegroundColor Yellow
        $fileGroups = $allFixes | Group-Object FileName | Sort-Object Name
        foreach ($fileGroup in $fileGroups) {
            Write-Host "  • $($fileGroup.Name): $($fileGroup.Count) fixes" -ForegroundColor Yellow
        }
    }
    
    if (-not $WhatIf) {
        Write-Success "🎉 CONTEXT-AWARE BRUSH FIXES COMPLETE!"
        Write-Host ""
        Write-Status "Next steps:"
        Write-Host "  1. Test the application with different themes"
        Write-Host "  2. Verify button text is now visible on primary buttons"
        Write-Host "  3. Run validate-wcag-compliance.ps1 to check contrast ratios"
        
        if ($BackupFiles) {
            Write-Host "  4. Remove .backup files once you're satisfied with the changes"
        }
    } else {
        Write-Warning "This was a What-If run - no changes were made"
        Write-Host "  • Remove -WhatIf parameter to apply the fixes"
        Write-Host "  • Use -TargetFile parameter to test on specific files first"
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Context-aware brush fixer failed: $($_.Exception.Message)"
    Write-Host $_.ScriptStackTrace -ForegroundColor Red
    exit 1
}
