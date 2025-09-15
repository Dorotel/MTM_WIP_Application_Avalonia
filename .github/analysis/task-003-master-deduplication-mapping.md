# TASK-003: Master Deduplication Mapping

**Date**: 2025-09-14  
**Phase**: 1 - Analysis, Preparation, and docs/ Folder Migration  
**Task**: Create master deduplication mapping showing files to merge/quarantine

## Executive Summary

**Migration Plan**: 86 files from `docs/` → `.github/`  
**Quarantine Plan**: 76 duplicate/legacy files → `quarantine/`  
**Consolidation Plan**: 33 theme checklists → 3 consolidated guides  
**Final Target**: 150 active files in `.github/` (40% reduction from 252 files)

## Master File Mapping

### 1. CRITICAL MIGRATIONS (docs/ → .github/)

#### A. Implementation Plans (9 files) → .github/project-management/
```
SOURCE → DESTINATION

docs/ways-of-work/plan/documentation-restructure/implementation-plan.md
→ .github/project-management/documentation-restructure-implementation-plan.md

docs/ways-of-work/plan/database-consistency-epic/stored-procedure-validation-and-standardization/implementation-plan.md
→ .github/project-management/database-stored-procedure-standardization-plan.md

docs/ways-of-work/plan/advanced-inventory/advanced-inventory-view/implementation-plan.md
→ .github/project-management/advanced-inventory-view-implementation-plan.md

docs/ways-of-work/plan/inventory-management-ui-overhaul/custom-data-grid-control/implementation-plan.md
→ .github/project-management/custom-data-grid-control-implementation-plan.md

docs/ways-of-work/plan/advanced-theme-editor-system/comprehensive-theme-editor-feature/implementation-plan.md
→ .github/project-management/theme-editor-system-implementation-plan.md

docs/ways-of-work/plan/remove-service/implementation-plan/implementation-plan.md
→ .github/project-management/remove-service-implementation-plan.md

docs/ways-of-work/plan/transfer-service/implementation-plan/implementation-plan.md
→ .github/project-management/transfer-service-implementation-plan.md

docs/ways-of-work/plan/print-service/implementation-plan/implementation-plan.md
→ .github/project-management/print-service-implementation-plan.md

docs/ways-of-work/plan/advanced-inventory/advanced-remove-view/implementation-plan.md
→ .github/project-management/advanced-remove-view-implementation-plan.md
```

#### B. Development Guides (12 files) → .github/development-guides/
```
SOURCE → DESTINATION

docs/development/view-management-md-files/MTM-View-Implementation-Guide.md
→ .github/development-guides/MTM-View-Implementation-Guide.md

docs/development/view-management-md-files/MTM-New-View-Implementation-Guide.md
→ .github/development-guides/MTM-New-View-Implementation-Guide.md

docs/development/view-management-md-files/MTM-Update-View-Implementation-Guide.md
→ .github/development-guides/MTM-Update-View-Implementation-Guide.md

docs/development/view-management-md-files/MTM-Bugfix-View-Implementation-Guide.md
→ .github/development-guides/MTM-Bugfix-View-Implementation-Guide.md

docs/development/view-management-md-files/MTM-Refactor-View-Implementation-Guide.md
→ .github/development-guides/MTM-Refactor-View-Implementation-Guide.md

docs/development/Documentation-Standards.md
→ .github/development-guides/MTM-Documentation-Standards.md

docs/development/Service-Implementation/FocusManagementImplementation.md
→ .github/development-guides/Focus-Management-Implementation.md

docs/development/Overlay-Implementation/SuccessOverlayImplementation.md
→ .github/development-guides/Success-Overlay-Implementation.md

docs/development/color-contrast-testing-procedure.md
→ .github/development-guides/Color-Contrast-Testing-Procedure.md

docs/development/wcag-validation-checklist.md
→ .github/development-guides/WCAG-Validation-Checklist.md

docs/development/wcag-validation-guide.md
→ .github/development-guides/WCAG-Validation-Guide.md
```

#### C. Architecture Documentation (2 files) → .github/architecture/
```
SOURCE → DESTINATION

docs/architecture/ [contents]
→ .github/architecture/ [merged with existing]
```

#### D. Theme Development (2 files) → .github/ui-ux/
```
SOURCE → DESTINATION

docs/theme-development/guidelines.md
→ .github/ui-ux/MTM-Theme-Development-Guidelines.md

docs/theme-development/design-system.md
→ .github/ui-ux/MTM-Design-System-Specifications.md
```

#### E. Other Essential Content
```
SOURCE → DESTINATION

docs/README.md
→ .github/docs-overview.md

docs/features/ [2 files]
→ .github/features/ [merge with existing]

docs/project-management/ [1 file]  
→ .github/project-management/ [merge]

docs/audit/ [1 file]
→ .github/audit/ [merge]

docs/ci-cd/ [contents]
→ .github/deployment/ [merge]

docs/accessibility/ [contents]
→ .github/development-guides/accessibility/ [new folder]
```

### 2. CONSOLIDATION MAPPINGS (Reduce File Count)

#### A. Theme Readiness Checklists (33 files → 3 files)
```
CONSOLIDATE FROM:
docs/ui-theme-readiness/*_theme_readiness_checklist.md (33 files)

CONSOLIDATE TO:
.github/ui-ux/MTM-Theme-Compliance-Standards.md
.github/ui-ux/MTM-Theme-Validation-Process.md  
.github/ui-ux/MTM-Theme-Implementation-Guide.md

QUARANTINE ORIGINALS:
quarantine/individual-theme-checklists/ (all 33 files preserved)
```

### 3. QUARANTINE MAPPINGS (Preserve but Remove from Active)

#### A. Redundant UI Documentation (33 files)
```
SOURCE → QUARANTINE

Documentation/Development/UI_Documentation/Controls/SettingsForm/*.instructions.md (20 files)
→ quarantine/redundant-ui-documentation/settings-form-controls/

Documentation/Development/UI_Documentation/Controls/Addons/*.instructions.md (3 files)
→ quarantine/redundant-ui-documentation/addon-controls/

Documentation/Development/UI_Documentation/Forms/*.instructions.md (5 files)
→ quarantine/redundant-ui-documentation/forms/

Documentation/Development/UI_Documentation/Views/*.instructions.md (5 files)
→ quarantine/redundant-ui-documentation/views/
```

#### B. Completed Implementation Reports (8 files)
```
SOURCE → QUARANTINE

Documentation/RemoveTabView-Implementation-Complete.md
Documentation/QuickButtonsView-Footer-Implementation-Complete.md
Documentation/QuickButtonsView-Database-Validation-Complete.md
Documentation/success-overlay-system-implementation.md
[Additional completion reports]
→ quarantine/completed-implementations/
```

#### C. Legacy Database Analysis (5 files)
```
SOURCE → QUARANTINE

Documentation/QuickButtonsView-Database-Analysis.md
Documentation/NewQuickButtonSpec.md
Documentation/RemoveTabView_Integration_Tests.md
[Additional database analysis files]
→ quarantine/legacy-database-analysis/
```

#### D. Individual Theme Checklists (35+ files)
```
SOURCE → QUARANTINE

docs/ui-theme-readiness/*_theme_readiness_checklist.md
docs/ui-theme-readiness/UI-THEME-READINESS-CHECKLIST-TEMPLATE.md
docs/ui-theme-readiness/ANALYSIS-SUMMARY.md
→ quarantine/individual-theme-checklists/
```

### 4. ROOT-LEVEL RELOCATIONS

#### A. Miscellaneous Files
```
SOURCE → DESTINATION

scripts/README.md
→ .github/scripts/README.md

prompt.md  
→ quarantine/legacy-prompts/root-prompt.md

Documentation/FileLogging-README.md
→ .github/development-guides/File-Logging-Implementation.md
```

## Cross-Reference Update Requirements

### Files Requiring Link Updates After Migration:

1. **.github/copilot-instructions.md** - Update all #file: references
2. **.github/Documentation-Management/master_documentation-index.md** - Update entire navigation structure
3. **All implementation plans** - Update relative path references
4. **Development guides** - Update cross-references between guides
5. **README files** - Update navigation links

### Specific Link Patterns to Update:
```
OLD PATTERN → NEW PATTERN

docs/development/view-management-md-files/MTM-View-Implementation-Guide.md
→ .github/development-guides/MTM-View-Implementation-Guide.md

docs/ways-of-work/plan/*/implementation-plan.md
→ .github/project-management/*-implementation-plan.md

Documentation/Development/UI_Documentation/*
→ [QUARANTINED] - Remove links or update to consolidated versions
```

## Validation Checkpoints

### Before Migration:
- [ ] Create complete backup of docs/ folder
- [ ] Create quarantine/ folder structure
- [ ] Verify .github/ destination folders exist

### During Migration:
- [ ] Validate each file transfer preserves content
- [ ] Update cross-references immediately after each file move
- [ ] Test critical links after each batch

### After Migration:
- [ ] Verify all essential content migrated to .github/
- [ ] Confirm quarantine folder contains all preserved files
- [ ] Validate link integrity throughout .github/ structure
- [ ] Test GitHub Copilot functionality with updated paths

## File Count Verification

### Before Migration:
- Total files: 252
- docs/: 86 files
- Documentation/: 43 files
- .github/: 123 files

### After Migration Target:
- .github/: ~150 files (core documentation)
- quarantine/: ~102 files (preserved legacy content)
- Root reduction: 252 → 150 active files (40% reduction)

**TASK-003 COMPLETE** ✅

## Next Steps - TASK-004
Validate current .github/copilot-instructions.md as authoritative reference before beginning migrations.