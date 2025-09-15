# Cross-Reference Links Update Analysis

**Analysis Date**: 2025-09-14  
**Purpose**: Identify all cross-reference links requiring updates after docs/ → .github/ migration  
**Scope**: All .github/ files with docs/ references

## Files Requiring Cross-Reference Updates

### High Priority - Documentation Navigation Files
1. **.github/docs-overview.md** - Main documentation index
2. **.github/Documentation-Management/master_documentation-index.md** - Complete documentation inventory
3. **.github/instructions/README.md** - Instructions directory index

### Medium Priority - Development Guide Files  
4. **.github/development-guides/MTM-Update-View-Implementation-Guide.md**
5. **.github/development-guides/MTM-New-View-Implementation-Guide.md**
6. **.github/development-guides/MTM-Bugfix-View-Implementation-Guide.md**
7. **.github/development-guides/MTM-Refactor-View-Implementation-Guide.md**

### Architecture and Structure Files
8. **.github/architecture/folder-structure.md** - Project structure documentation

### Project Management Files
9. **.github/project-management/issue-creation-guide.md**
10. **.github/project-management/github-issue-template.md**

### Script and Automation Files
11. **.github/scripts/README.md**
12. **.github/scripts/audit-system-prompt.md**
13. **.github/scripts/SLASH-COMMAND-WORKFLOW.md**
14. **.github/audit/README.md** (duplicate)
15. **.github/audit/audit-system-prompt.md** (duplicate)

### Prompt Files
16. **.github/prompts/update-implementation-plan.prompt.md** - This file
17. **.github/prompts/mtm-ui-documentation-audit.prompt.md**
18. **.github/prompts/breakdown-feature-implementation.prompt.md**
19. **.github/prompts/mtm-audit-system.prompt.md**
20. **.github/prompts/breakdown-feature-prd.prompt.md**

### Analysis Files (Already Created)
21. **.github/analysis/task-004-copilot-instructions-validation.md**
22. **.github/analysis/task-001-file-categorization-report.md**
23. **.github/analysis/task-002-duplicate-content-analysis.md**
24. **.github/analysis/task-003-master-deduplication-mapping.md**

### Example and Template Files
25. **.github/scripts/templates/examples/print-service-gap-report-example.md**
26. **.github/scripts/templates/examples/print-service-copilot-prompt-example.md**
27. **.github/audit/templates/examples/** (duplicates)

### Other Process Files
28. **.github/processes/plan/documentation-restructure/implementation-plan.md**
29. **.github/processes/plan/print-service/COPILOT-ISSUE-TEMPLATE.md**
30. **.github/processes/plan/inventory-management-ui-overhaul/custom-data-grid-control/prd.md**

### Issue Templates
31. **.github/ISSUE_TEMPLATE/epic-comprehensive-testing-implementation.md**

### Documentation Files
32. **.github/docs/Platform-Specific-File-System-Differences.md**
33. **.github/docs/README-CrossPlatformTesting.md**

## Common docs/ Reference Patterns to Update

### Pattern Replacements Required:
```
# From docs/ structure references:
docs/README.md → .github/docs-overview.md
docs/architecture/ → .github/architecture/
docs/development/ → .github/development-guides/
docs/features/ → .github/features/
docs/project-management/ → .github/project-management/
docs/theme-development/ → .github/ui-ux/
docs/ways-of-work/ → .github/processes/
docs/accessibility/ → .github/development-guides/ (consolidated)
docs/audit/ → .github/audit/ (already exists)
docs/ci-cd/ → .github/deployment/ (to be created)
docs/ui-theme-readiness/ → quarantine/ui-theme-checklists/ (redundant)
```

### Link Pattern Updates:
- `[text](../docs/...)` → `[text](../...)` (relative path adjustment)
- `[text](docs/...)` → `[text](.github/...)` (absolute from root)
- `#file: docs/...` → `#file: .github/...` (copilot instructions)

## Validation Steps Required

1. **Update each identified file** with correct .github/ paths
2. **Test all internal links** to ensure they resolve correctly  
3. **Update relative paths** based on new .github/ structure
4. **Validate cross-references** in copilot-instructions.md
5. **Check documentation indexes** for accuracy after migration

## Script Requirements

Create automated link update script:
- Find and replace docs/ references with appropriate .github/ paths
- Preserve external links unchanged
- Update relative path calculations
- Generate validation report for manual review

## Dependencies

This task depends on:
- **TASK-011 to TASK-014**: Complete migration of docs/ content
- **TASK-015**: Checkpoint validation of migration completion
- **Quarantine setup**: For files being relocated rather than migrated

## Risk Mitigation

- **Backup original files** before link updates
- **Test links individually** rather than bulk replacement
- **Maintain cross-reference mapping** for rollback capability
- **Validate with actual navigation** in GitHub interface

## Success Criteria

- All internal docs/ links resolve correctly to .github/ structure
- No broken links in .github/ documentation system
- Navigation flows work seamlessly from entry points
- Cross-references in copilot-instructions.md are accurate