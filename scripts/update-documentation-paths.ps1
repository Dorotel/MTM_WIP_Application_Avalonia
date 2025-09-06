#!/usr/bin/env pwsh
# =============================================================================
# MTM Documentation Path Update Script
# Updates all references to documentation files after reorganization
# =============================================================================

param(
    [switch]$WhatIf = $false,
    [switch]$VerboseOutput = $false
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Define all path mappings for documentation reorganization
$PATH_MAPPINGS = @{
    # Root documentation files moved
    "docs/ci-cd/integration-guide.md" = "docs/ci-cd/integration-guide.md"
    "docs/theme-development/guidelines.md" = "docs/theme-development/guidelines.md"
    "docs/accessibility/accessibility/wcag-validation-guide.md" = "docs/accessibility/accessibility/wcag-validation-guide.md"
    "docs/project-management/todo-analysis-2025-09-06.md" = "docs/project-management/todo-analysis-2025-09-06.md"
    
    # Ways-of-work files integrated
    "docs/project-management/epic.md" = "docs/project-management/epic.md"
    "docs/project-management/github-issue-template.md" = "docs/project-management/github-issue-template.md"
    "docs/project-management/issue-creation-guide.md" = "docs/project-management/issue-creation-guide.md"
    "docs/architecture/project-blueprint.md" = "docs/architecture/project-blueprint.md"
    "docs/architecture/folder-structure.md" = "docs/architecture/folder-structure.md"
    "docs/architecture/epic-specification.md" = "docs/architecture/epic-specification.md"
    "docs/architecture/viewmodels-specification.md" = "docs/architecture/viewmodels-specification.md"
    "docs/features/inventory-transaction-management.md" = "docs/features/inventory-transaction-management.md"
    "docs/features/master-data-management.md" = "docs/features/master-data-management.md"
    "docs/features/quick-actions-panel.md" = "docs/features/quick-actions-panel.md"
    "docs/features/settings-system-administration.md" = "docs/features/settings-system-administration.md"
    "docs/theme-development/design-system.md" = "docs/theme-development/design-system.md"
    
    # Folder rename
    "docs/ui-theme-readiness/" = "docs/ui-theme-readiness/"
    "ui-theme-readiness/" = "ui-theme-readiness/"
}

function Write-Status($Message, $Color = "White") {
    Write-Host "🔄 $Message" -ForegroundColor $Color
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

function Find-FilesToUpdate {
    Write-Status "Finding files that may contain documentation references..."
    
    $filesToCheck = @()
    
    # Check all markdown files
    $filesToCheck += Get-ChildItem -Path "docs" -Recurse -Filter "*.md" -ErrorAction SilentlyContinue
    $filesToCheck += Get-ChildItem -Path "README.md" -ErrorAction SilentlyContinue
    
    # Check all script files
    $filesToCheck += Get-ChildItem -Path "scripts" -Recurse -Filter "*.ps1" -ErrorAction SilentlyContinue
    $filesToCheck += Get-ChildItem -Path "scripts" -Recurse -Filter "*.sh" -ErrorAction SilentlyContinue
    
    # Check project files
    $filesToCheck += Get-ChildItem -Path "*.md" -ErrorAction SilentlyContinue
    $filesToCheck += Get-ChildItem -Path ".github" -Recurse -Filter "*.md" -ErrorAction SilentlyContinue
    $filesToCheck += Get-ChildItem -Path ".github" -Recurse -Filter "*.yml" -ErrorAction SilentlyContinue
    $filesToCheck += Get-ChildItem -Path ".github" -Recurse -Filter "*.yaml" -ErrorAction SilentlyContinue
    
    Write-Status "Found $($filesToCheck.Count) files to check for references"
    return $filesToCheck
}

function Update-FileReferences($File) {
    if ($VerboseOutput) {
        Write-Status "Checking: $($File.Name)"
    }
    
    try {
        $content = Get-Content $File.FullName -Raw -ErrorAction Stop
        $replacementCount = 0
        
        foreach ($mapping in $PATH_MAPPINGS.GetEnumerator()) {
            $oldPath = $mapping.Key
            $newPath = $mapping.Value
            
            # Try multiple pattern variations for robust matching
            $patterns = @(
                [regex]::Escape($oldPath),
                [regex]::Escape($oldPath.Replace("/", "\")),
                [regex]::Escape($oldPath.Replace("\", "/")),
                # Handle relative paths
                [regex]::Escape($oldPath.Replace("docs/", "")),
                [regex]::Escape($oldPath.Replace("docs\", ""))
            )
            
            foreach ($pattern in $patterns) {
                if ($content -match $pattern) {
                    $replacementPattern = $oldPath
                    $replacementText = $newPath
                    
                    # Handle relative path replacements
                    if ($pattern -eq [regex]::Escape($oldPath.Replace("docs/", ""))) {
                        $replacementPattern = $oldPath.Replace("docs/", "")
                        $replacementText = $newPath.Replace("docs/", "")
                    } elseif ($pattern -eq [regex]::Escape($oldPath.Replace("docs\", ""))) {
                        $replacementPattern = $oldPath.Replace("docs\", "")
                        $replacementText = $newPath.Replace("docs\", "")
                    }
                    
                    $matchCount = ([regex]::Matches($content, [regex]::Escape($replacementPattern), [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)).Count
                    if ($matchCount -gt 0) {
                        $content = $content -replace [regex]::Escape($replacementPattern), $replacementText
                        $replacementCount += $matchCount
                        
                        if ($VerboseOutput) {
                            Write-Host "   → $replacementPattern ⇒ $replacementText ($matchCount matches)" -ForegroundColor Yellow
                        }
                    }
                }
            }
        }
        
        # Write updated content if changes were made
        if ($replacementCount -gt 0) {
            if (-not $WhatIf) {
                Set-Content -Path $File.FullName -Value $content -NoNewline -Encoding UTF8
                Write-Success "Updated $($File.Name) ($replacementCount replacements)"
            } else {
                Write-Host "WHAT-IF: Would update $($File.Name) ($replacementCount replacements)" -ForegroundColor Magenta
            }
            return $replacementCount
        }
        
        return 0
        
    } catch {
        Write-Error "Failed to process $($File.Name): $($_.Exception.Message)"
        return 0
    }
}

function Main {
    Write-Host ""
    Write-Host "🔄 MTM DOCUMENTATION PATH UPDATE" -ForegroundColor Cyan
    Write-Host "=================================" -ForegroundColor Cyan
    Write-Host ""
    
    if ($WhatIf) {
        Write-Warning "RUNNING IN WHAT-IF MODE - No changes will be made"
        Write-Host ""
    }
    
    # Find all files that might contain documentation references
    $filesToCheck = Find-FilesToUpdate
    
    if ($filesToCheck.Count -eq 0) {
        Write-Warning "No files found to check"
        return
    }
    
    Write-Host ""
    Write-Host "📝 UPDATING FILE REFERENCES:" -ForegroundColor Cyan
    Write-Host "=============================" -ForegroundColor Cyan
    Write-Host ""
    
    $totalReplacements = 0
    $filesModified = 0
    
    foreach ($file in $filesToCheck) {
        $replacements = Update-FileReferences $file
        if ($replacements -gt 0) {
            $totalReplacements += $replacements
            $filesModified++
        }
    }
    
    # Summary
    Write-Host ""
    Write-Host "📊 PATH UPDATE SUMMARY" -ForegroundColor Cyan
    Write-Host "======================" -ForegroundColor Cyan
    Write-Host "Files checked: $($filesToCheck.Count)" -ForegroundColor White
    Write-Host "Files modified: $filesModified" -ForegroundColor Green
    Write-Host "Total replacements: $totalReplacements" -ForegroundColor Yellow
    Write-Host "Path mappings applied: $($PATH_MAPPINGS.Count)" -ForegroundColor White
    
    if ($WhatIf) {
        Write-Host ""
        Write-Warning "This was a What-If run - no changes were made"
        Write-Host "Remove -WhatIf parameter to apply the path updates" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Success "Documentation path updates completed!"
        Write-Host ""
        Write-Status "All references have been updated to use the new documentation structure"
        Write-Host "Verify the changes and commit when satisfied" -ForegroundColor White
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Documentation path update failed: $($_.Exception.Message)"
    exit 1
}
