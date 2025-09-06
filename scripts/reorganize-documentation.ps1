#!/usr/bin/env pwsh
# =============================================================================
# MTM Documentation Reorganization Script
# Reorganizes all documentation files into proper categorical structure
# =============================================================================

param(
    [switch]$WhatIf = $false,
    [switch]$VerboseOutput = $false
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Write-Status($Message, $Color = "White") {
    Write-Host "📂 $Message" -ForegroundColor $Color
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

function Write-Move($From, $To) {
    Write-Host "   $From → $To" -ForegroundColor Yellow
}

# Define the reorganization plan
$REORGANIZATION_PLAN = @{
    # Root docs files to move
    RootFiles = @{
        "ci-cd/integration-guide.md" = "ci-cd/integration-guide.md"
        "theme-development/guidelines.md" = "theme-development/guidelines.md" 
        "accessibility/wcag-validation-guide.md" = "accessibility/accessibility/wcag-validation-guide.md"
        "project-management/todo-analysis-2025-09-06.md" = "project-management/todo-analysis-2025-09-06.md"
    }
    
    # Ways of work files to integrate
    WaysOfWorkFiles = @{
        # Main project files
        "project-management/epic.md" = "project-management/epic.md"
        "project-management/github-issue-template.md" = "project-management/github-issue-template.md"
        "project-management/issue-creation-guide.md" = "project-management/issue-creation-guide.md"
        "architecture/project-blueprint.md" = "architecture/project-blueprint.md"
        "architecture/folder-structure.md" = "architecture/folder-structure.md"
        
        # Architecture files
        "architecture/epic-specification.md" = "architecture/epic-specification.md"
        "architecture/viewmodels-specification.md" = "architecture/viewmodels-specification.md"
        
        # Feature specifications
        "features/inventory-transaction-management.md" = "features/inventory-transaction-management.md"
        "features/master-data-management.md" = "features/master-data-management.md"
        "features/quick-actions-panel.md" = "features/quick-actions-panel.md"
        "features/settings-system-administration.md" = "features/settings-system-administration.md"
        "theme-development/design-system.md" = "theme-development/design-system.md"
    }
    
    # Directories to create
    DirectoriesToCreate = @(
        "ci-cd",
        "theme-development", 
        "accessibility",
        "project-management",
        "architecture",
        "features"
    )
}

function New-DocumentationDirectories {
    Write-Status "Creating documentation directory structure..."
    
    foreach ($dir in $REORGANIZATION_PLAN.DirectoriesToCreate) {
        $fullPath = Join-Path "docs" $dir
        
        if (-not (Test-Path $fullPath)) {
            if (-not $WhatIf) {
                New-Item -Path $fullPath -ItemType Directory -Force | Out-Null
                Write-Success "Created directory: $dir/"
            } else {
                Write-Host "WHAT-IF: Would create directory: $dir/" -ForegroundColor Magenta
            }
        } else {
            if ($VerboseOutput) {
                Write-Status "Directory already exists: $dir/"
            }
        }
    }
}

function Move-DocumentationFiles {
    Write-Status "Moving documentation files..."
    
    $totalMoves = 0
    $successfulMoves = 0
    
    # Move root files
    Write-Host ""
    Write-Host "📁 ROOT DOCUMENTATION FILES:" -ForegroundColor Cyan
    foreach ($move in $REORGANIZATION_PLAN.RootFiles.GetEnumerator()) {
        $sourcePath = Join-Path "docs" $move.Key
        $destPath = Join-Path "docs" $move.Value
        $totalMoves++
        
        if (Test-Path $sourcePath) {
            Write-Move $move.Key $move.Value
            
            if (-not $WhatIf) {
                try {
                    # Ensure destination directory exists
                    $destDir = Split-Path $destPath -Parent
                    if (-not (Test-Path $destDir)) {
                        New-Item -Path $destDir -ItemType Directory -Force | Out-Null
                    }
                    
                    Move-Item $sourcePath $destPath -Force
                    $successfulMoves++
                } catch {
                    Write-Error "Failed to move $($move.Key): $($_.Exception.Message)"
                }
            } else {
                Write-Host "   WHAT-IF: Would move file" -ForegroundColor Magenta
                $successfulMoves++
            }
        } else {
            Write-Warning "Source file not found: $($move.Key)"
        }
    }
    
    # Move ways-of-work files
    Write-Host ""
    Write-Host "📁 WAYS-OF-WORK FILES:" -ForegroundColor Cyan
    foreach ($move in $REORGANIZATION_PLAN.WaysOfWorkFiles.GetEnumerator()) {
        $sourcePath = Join-Path "docs" $move.Key
        $destPath = Join-Path "docs" $move.Value
        $totalMoves++
        
        if (Test-Path $sourcePath) {
            Write-Move $move.Key $move.Value
            
            if (-not $WhatIf) {
                try {
                    # Ensure destination directory exists
                    $destDir = Split-Path $destPath -Parent
                    if (-not (Test-Path $destDir)) {
                        New-Item -Path $destDir -ItemType Directory -Force | Out-Null
                    }
                    
                    Move-Item $sourcePath $destPath -Force
                    $successfulMoves++
                } catch {
                    Write-Error "Failed to move $($move.Key): $($_.Exception.Message)"
                }
            } else {
                Write-Host "   WHAT-IF: Would move file" -ForegroundColor Magenta
                $successfulMoves++
            }
        } else {
            Write-Warning "Source file not found: $($move.Key)"
        }
    }
    
    return @{
        TotalMoves = $totalMoves
        SuccessfulMoves = $successfulMoves
    }
}

function Remove-EmptyDirectories {
    Write-Status "Cleaning up empty directories..."
    
    $emptyDirs = @("ways-of-work")
    
    foreach ($dir in $emptyDirs) {
        $fullPath = Join-Path "docs" $dir
        
        if (Test-Path $fullPath) {
            try {
                # Check if directory is empty (recursively)
                $items = Get-ChildItem $fullPath -Recurse
                
                if ($items.Count -eq 0) {
                    if (-not $WhatIf) {
                        Remove-Item $fullPath -Recurse -Force
                        Write-Success "Removed empty directory: $dir/"
                    } else {
                        Write-Host "WHAT-IF: Would remove empty directory: $dir/" -ForegroundColor Magenta
                    }
                } else {
                    Write-Warning "Directory not empty, keeping: $dir/ ($($items.Count) items remaining)"
                }
            } catch {
                Write-Warning "Failed to remove directory $dir/: $($_.Exception.Message)"
            }
        }
    }
}

function Rename-UIThemeFolder {
    Write-Status "Renaming UI theme readiness folder..."
    
    $oldPath = "docs/uithemereadyness"
    $newPath = "docs/ui-theme-readiness"
    
    if (Test-Path $oldPath) {
        if (-not $WhatIf) {
            try {
                Rename-Item $oldPath $newPath
                Write-Success "Renamed: uithemereadyness → ui-theme-readiness"
            } catch {
                Write-Error "Failed to rename folder: $($_.Exception.Message)"
            }
        } else {
            Write-Host "WHAT-IF: Would rename: uithemereadyness → ui-theme-readiness" -ForegroundColor Magenta
        }
    } else {
        Write-Warning "UI theme readiness folder not found"
    }
}

function Show-FinalStructure {
    if ($WhatIf) {
        Write-Host ""
        Write-Host "📋 PROPOSED FINAL STRUCTURE:" -ForegroundColor Cyan
        Write-Host "=============================" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "📋 FINAL DOCUMENTATION STRUCTURE:" -ForegroundColor Cyan
        Write-Host "==================================" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Host "docs/" -ForegroundColor Yellow
    Write-Host "├── README.md" -ForegroundColor White
    Write-Host "├── accessibility/" -ForegroundColor Yellow
    Write-Host "│   ├── accessibility/wcag-validation-guide.md" -ForegroundColor White
    Write-Host "├── architecture/" -ForegroundColor Yellow
    Write-Host "│   ├── project-blueprint.md" -ForegroundColor White
    Write-Host "│   ├── folder-structure.md" -ForegroundColor White
    Write-Host "│   ├── epic-specification.md" -ForegroundColor White
    Write-Host "│   └── viewmodels-specification.md" -ForegroundColor White
    Write-Host "├── ci-cd/" -ForegroundColor Yellow
    Write-Host "│   └── integration-guide.md" -ForegroundColor White
    Write-Host "├── development/" -ForegroundColor Yellow
    Write-Host "│   ├── color-contrast-testing-procedure.md" -ForegroundColor White
    Write-Host "│   ├── Documentation-Standards.md" -ForegroundColor White
    Write-Host "│   ├── wcag-validation-checklist.md" -ForegroundColor White
    Write-Host "│   └── accessibility/wcag-validation-guide.md" -ForegroundColor White
    Write-Host "├── features/" -ForegroundColor Yellow
    Write-Host "│   ├── inventory-transaction-management.md" -ForegroundColor White
    Write-Host "│   ├── master-data-management.md" -ForegroundColor White
    Write-Host "│   ├── quick-actions-panel.md" -ForegroundColor White
    Write-Host "│   └── settings-system-administration.md" -ForegroundColor White
    Write-Host "├── project-management/" -ForegroundColor Yellow
    Write-Host "│   ├── epic.md" -ForegroundColor White
    Write-Host "│   ├── github-issue-template.md" -ForegroundColor White
    Write-Host "│   ├── issue-creation-guide.md" -ForegroundColor White
    Write-Host "│   └── todo-analysis-2025-09-06.md" -ForegroundColor White
    Write-Host "├── theme-development/" -ForegroundColor Yellow
    Write-Host "│   ├── guidelines.md" -ForegroundColor White
    Write-Host "│   └── design-system.md" -ForegroundColor White
    Write-Host "└── ui-theme-readiness/" -ForegroundColor Yellow
    Write-Host "    ├── README.md" -ForegroundColor White
    Write-Host "    ├── ANALYSIS-SUMMARY.md" -ForegroundColor White
    Write-Host "    └── [31 view checklist files...]" -ForegroundColor Gray
}

function Main {
    Write-Host ""
    Write-Host "📚 MTM DOCUMENTATION REORGANIZATION" -ForegroundColor Cyan
    Write-Host "====================================" -ForegroundColor Cyan
    Write-Host ""
    
    if ($WhatIf) {
        Write-Warning "RUNNING IN WHAT-IF MODE - No changes will be made"
        Write-Host ""
    }
    
    # Step 1: Create directory structure
    New-DocumentationDirectories
    
    # Step 2: Rename UI theme folder
    Rename-UIThemeFolder
    
    # Step 3: Move all files
    $moveResults = Move-DocumentationFiles
    
    # Step 4: Clean up empty directories
    Remove-EmptyDirectories
    
    # Step 5: Show final structure
    Show-FinalStructure
    
    # Summary
    Write-Host ""
    Write-Host "📊 REORGANIZATION SUMMARY" -ForegroundColor Cyan
    Write-Host "=========================" -ForegroundColor Cyan
    Write-Host "Directories created: $($REORGANIZATION_PLAN.DirectoriesToCreate.Count)" -ForegroundColor White
    Write-Host "Files moved: $($moveResults.SuccessfulMoves) / $($moveResults.TotalMoves)" -ForegroundColor Green
    Write-Host "Folder renamed: uithemereadyness → ui-theme-readiness" -ForegroundColor Yellow
    
    if ($WhatIf) {
        Write-Host ""
        Write-Warning "This was a What-If run - no changes were made"
        Write-Host "Remove -WhatIf parameter to apply the reorganization" -ForegroundColor White
    } else {
        Write-Host ""
        Write-Success "Documentation reorganization completed!"
        Write-Host "Next step: Run update-documentation-paths.ps1 to fix all references" -ForegroundColor White
    }
}

# Execute main function
try {
    Main
} catch {
    Write-Error "Documentation reorganization failed: $($_.Exception.Message)"
    exit 1
}
