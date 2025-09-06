# MTM WIP Application Scripts Directory

This directory contains all automation scripts for the MTM WIP Application organized by functional category.

## 📁 Directory Structure

### 🎨 **ui-analysis/**
Scripts for analyzing and fixing UI elements, themes, and visual consistency.

- `analyze-ui-theme-readiness.ps1` - Comprehensive UI theme compliance analysis
- `analyze-ui-theme-readiness.sh` - Shell version of theme readiness analysis  
- `detect-hardcoded-colors.ps1` - Detects hardcoded color values in AXAML files
- `fix-view-brush-combinations.ps1` - Automatically fixes problematic brush combinations
- `validate-view-brush-combinations.ps1` - Validates foreground/background brush pairings

### 🎭 **theme-management/**
Scripts for theme creation, optimization, and maintenance.

- `complete-theme-brushes.ps1` - Ensures all themes have complete brush definitions
- `optimize-theme-file-sizes.ps1` - Reduces theme file sizes and improves performance
- `phase6-resource-mapping-resolution.ps1` - Advanced theme resource mapping
- `validate-theme-structure.ps1` - Validates theme file structure and consistency

### ♿ **accessibility/**
Scripts focused on WCAG compliance and accessibility improvements.

- `manual-wcag-optimization.ps1` - Manual WCAG compliance optimization
- `phase6-advanced-wcag-optimization.ps1` - Advanced accessibility enhancements
- `remediate-wcag-failures.ps1` - Automatically fixes WCAG compliance issues
- `validate-wcag-compliance.ps1` - Comprehensive WCAG 2.1 AA compliance testing

### ⚡ **performance-optimization/**
Scripts for performance testing, monitoring, and optimization.

- `manual-performance-optimization.ps1` - Manual performance improvement tasks
- `performance-test-themes.ps1` - Comprehensive theme performance testing
- `phase6-performance-optimization.ps1` - Advanced performance optimization
- `quick-performance-check.ps1` - Quick performance validation checks

### 🧪 **validation-testing/**
Scripts for comprehensive testing and validation processes.

- `phase6-visual-regression-testing.ps1` - Visual regression testing framework

### 📊 **reporting/**
Scripts and files for generating reports and documentation.

- `theme-epic-status-report.ps1` - Comprehensive EPIC status reporting
- `wcag-validation-report.json` - WCAG validation results data

## 🚀 Usage Examples

### Run UI Theme Analysis
```powershell
.\ui-analysis\analyze-ui-theme-readiness.ps1 -AllViews
```

### Fix Brush Combinations
```powershell
.\ui-analysis\fix-view-brush-combinations.ps1 -BackupFiles:$true
```

### Validate WCAG Compliance
```powershell
.\accessibility\validate-wcag-compliance.ps1
```

### Performance Testing
```powershell
.\performance-optimization\performance-test-themes.ps1
```

### Generate Status Report
```powershell
.\reporting\theme-epic-status-report.ps1
```

## 🔧 Script Dependencies

Most scripts require:
- PowerShell 5.1+ or PowerShell Core 7+
- .NET 8 SDK (for theme processing)
- Access to Views/ and Resources/Themes/ directories

## 📝 Notes

- All PowerShell scripts support `-WhatIf` parameter for safe testing
- Backup files are created automatically where appropriate
- Scripts follow MTM naming conventions and coding standards
- JSON report files are generated in UTF-8 encoding

## 🆘 Troubleshooting

If scripts fail to execute:
1. Check PowerShell execution policy: `Get-ExecutionPolicy`
2. Set execution policy if needed: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
3. Ensure you're running from the application root directory
4. Check file paths match your project structure

## 🔄 Version History

- **v1.0** - Initial script organization into categorical structure
- Scripts migrated from flat structure to organized categories
- Improved maintainability and discoverability
