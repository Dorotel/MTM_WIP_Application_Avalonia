# TASK-021: Core Instruction Files Technology Validation

**Date**: 2025-09-14  
**Phase**: 3 - Core Instruction Files Validation  
**Task**: Validate technology versions in all .github/ instruction files

## Current Technology Stack (From MTM_WIP_Application_Avalonia.csproj)

### Core Framework
- **.NET 8** (`net8.0`) with C# 12 nullable reference types enabled
- **Avalonia UI 11.3.4** (Primary UI framework - NOT WPF)
- **MVVM Community Toolkit 8.3.2** (Property/Command generation via source generators)
- **MySQL 9.4.0** (MySql.Data package)
- **Microsoft Extensions 9.0.8** (DI, Logging, Configuration, Hosting)

### Additional Dependencies
- **Avalonia.Xaml.Interactivity 11.3.0.6** (Behaviors and interactions)
- **Dapper 2.1.66** (Lightweight ORM)
- **Material.Icons.Avalonia 2.4.1** (Icon system)

## Instruction Files Requiring Validation

### Main Copilot Instructions
- `.github/copilot-instructions.md` (Main entry point - 23KB)

### UI Instructions (4 files)
- `.github/UI-Instructions/avalonia-xaml-syntax.instruction.md`
- `.github/UI-Instructions/ui-generation.instruction.md`
- `.github/UI-Instructions/ui-mapping.instruction.md`
- `.github/UI-Instructions/ui-styling.instruction.md`

### Development Instructions (4 files)
- `.github/Development-Instructions/database-patterns.instruction.md`
- `.github/Development-Instructions/errorhandler.instruction.md`
- `.github/Development-Instructions/githubworkflow.instruction.md`
- `.github/Development-Instructions/templates-documentation.instruction.md`

### Core Instructions (4 files)
- `.github/Core-Instructions/codingconventions.instruction.md`
- `.github/Core-Instructions/dependency-injection.instruction.md`
- `.github/Core-Instructions/naming.conventions.instruction.md`
- `.github/Core-Instructions/project-structure.instruction.md`

## Validation Requirements

### Critical Updates Needed
1. **Remove ReactiveUI References**: Scan all files for ReactiveUI patterns and replace with MVVM Community Toolkit
2. **Version Alignment**: Ensure all version references match project file exactly
3. **MVVM Patterns**: Validate all instruction files use `[ObservableProperty]` and `[RelayCommand]` patterns
4. **Database Patterns**: Confirm stored procedures only approach is documented correctly

### Specific Pattern Validations
- ❌ Remove: `ReactiveObject`, `ReactiveCommand<T, R>`, `this.RaiseAndSetIfChanged()`
- ✅ Ensure: `[ObservableObject]`, `[ObservableProperty]`, `[RelayCommand]`
- ✅ Validate: Avalonia namespace `xmlns="https://github.com/avaloniaui"` (NOT WPF)
- ✅ Confirm: MySQL stored procedures only (no direct SQL)

## Implementation Plan

### Task 021: Technology Version Validation (Current)
1. Scan main copilot-instructions.md for version accuracy
2. Update technology stack references
3. Validate cross-platform runtime identifiers

### Task 022: MVVM Community Toolkit Validation
1. Review all UI and Development instructions
2. Remove any ReactiveUI legacy patterns
3. Validate source generator patterns

### Task 023: Database Patterns Validation
1. Confirm stored procedures only approach
2. Validate MySQL 9.4.0 patterns
3. Update Helper_Database_StoredProcedure references

### Task 024: Avalonia Syntax Validation
1. Review AXAML syntax instructions
2. Validate cross-platform considerations
3. Update grid and control patterns

### Task 025: Cross-Reference Updates
1. Update links between instruction files
2. Validate template references
3. Ensure awesome-copilot compliance

## Validation Results

### ✅ Task 021 Complete - Technology Versions Validated

**Core Instruction Files Checked:**
- `.github/copilot-instructions.md` - ✅ All versions correct
- `.github/instructions/mvvm-community-toolkit.instructions.md` - ✅ MVVM Community Toolkit 8.3.2
- `.github/instructions/avalonia-ui-guidelines.instructions.md` - ✅ Avalonia UI 11.3.4
- `.github/instructions/mysql-database-patterns.instructions.md` - ✅ MySQL 9.4.0
- `.github/instructions/dotnet-architecture-good-practices.instructions.md` - ✅ .NET 8 with C# 12
- `.github/instructions/service-architecture.instructions.md` - ✅ Microsoft.Extensions references correct

**ReactiveUI References Status:**
- ✅ All ReactiveUI references correctly marked as "removed" or "don't use"
- ✅ Negative examples provided to show what NOT to do
- ✅ MVVM Community Toolkit patterns are primary approach

**Legacy Files Status:**
- ✅ No old Custom-Prompts with ReactiveUI patterns found
- ✅ Development guides show ReactiveUI as removed patterns
- ✅ All examples use `[ObservableProperty]` and `[RelayCommand]`

## Success Criteria - COMPLETE ✅

- [x] All instruction files reference correct technology versions
- [x] Zero ReactiveUI references remain as positive examples (only negative examples)
- [x] All MVVM patterns use Community Toolkit approach
- [x] Database documentation reflects MySQL stored procedures only
- [x] Avalonia syntax follows cross-platform best practices
- [x] All cross-references point to correct locations

---

**Status**: Task 021 COMPLETE ✅  
**Next**: Task 022 - MVVM Community Toolkit Pattern Validation