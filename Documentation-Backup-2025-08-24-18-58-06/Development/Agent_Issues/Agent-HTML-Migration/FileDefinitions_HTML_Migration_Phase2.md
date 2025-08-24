# FileDefinitions_HTML_Migration_Phase2 - Agent Instructions

## ?? **AGENT EXECUTION CONTEXT**
**Issue Type:** Phase
**Complexity:** Complex
**Estimated Time:** 4hr
**Dependencies:** Fix1 (Backup), Phase1 (CSS Modernization) must be completed

## ?? **PRECISE OBJECTIVES**
### Primary Goal
Migrate all 27 FileDefinitions from Docs/ folder to Documentation/HTML/ with dual PlainEnglish and Technical versions, plus transform 3 root files with process-oriented PlainEnglish content.

### Acceptance Criteria
- [ ] 27 FileDefinitions migrated: 13 ViewModels, 11 Services, 2 Models, 1 Extension
- [ ] 54 total files created (PlainEnglish + Technical versions of each)
- [ ] 6 additional files from root-level transformations (3 × 2 versions)
- [ ] All files use FileDefinitions- prefix naming convention
- [ ] All files reference all three CSS files + responsive.css
- [ ] PlainEnglish versions focus on process-oriented, non-developer content
- [ ] Technical versions maintain full developer-focused documentation
- [ ] MTM styling and modern HTML structure applied throughout

## ?? **IMPLEMENTATION DETAILS**

### Files to Modify/Create
```
Documentation/HTML/PlainEnglish/FileDefinitions/ViewModels/ (13 files)
Documentation/HTML/Technical/FileDefinitions/ViewModels/ (13 files)
Documentation/HTML/PlainEnglish/FileDefinitions/Services/ (11 files)
Documentation/HTML/Technical/FileDefinitions/Services/ (11 files)
Documentation/HTML/PlainEnglish/FileDefinitions/Models/ (2 files)
Documentation/HTML/Technical/FileDefinitions/Models/ (2 files)
Documentation/HTML/PlainEnglish/FileDefinitions/Extensions/ (1 file)
Documentation/HTML/Technical/FileDefinitions/Extensions/ (1 file)
Documentation/HTML/PlainEnglish/FileDefinitions-database-development.html
Documentation/HTML/Technical/FileDefinitions-database-development.html
Documentation/HTML/PlainEnglish/FileDefinitions-database-operations-prompts.html
Documentation/HTML/Technical/FileDefinitions-database-operations-prompts.html
Documentation/HTML/PlainEnglish/FileDefinitions-missing-systems.html
Documentation/HTML/Technical/FileDefinitions-missing-systems.html
```

### Code Patterns Required
```html
<!-- Standard HTML template for all migrated files -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>FileDefinitions-{FileName} - MTM WIP Application</title>
    
    <!-- All CSS files as specified -->
    <link rel="stylesheet" href="../../assets/css/modern-styles.css">
    <link rel="stylesheet" href="../../assets/css/mtm-theme.css">
    <link rel="stylesheet" href="../../assets/css/plain-english.css">
    <link rel="stylesheet" href="../../assets/css/responsive.css">
</head>
<body>
    <!-- Cross-reference navigation placeholder (Phase3) -->
    <div class="cross-reference-nav">
        <!-- Will be populated in Phase3 -->
    </div>
    
    <!-- Main content with MTM styling -->
    <div class="container">
        <!-- Migrated content here -->
    </div>
</body>
</html>
```

### Database Operations (If Applicable)
Not applicable for content migration.

## ? **EXECUTION SEQUENCE**
1. **Step 1:** Scan Docs/ folder and verify 27 FileDefinitions files (13+11+2+1)
2. **Step 2:** Process ViewModels first (BY_CATEGORY order per user config)
   - Create PlainEnglish versions with process-oriented content
   - Create Technical versions with full developer documentation
   - Apply FileDefinitions- prefix naming
3. **Step 3:** Process Services (11 files) with same dual-version approach
4. **Step 4:** Process Models (2 files) with same dual-version approach  
5. **Step 5:** Process Extensions (1 file) with same dual-version approach
6. **Step 6:** Transform root files (database-development, database-operations-prompts, missing-systems)
   - PlainEnglish: Business impact, workflow descriptions, user benefits
   - Technical: Full implementation details and developer guidance
7. **Step 7:** Apply consistent HTML structure and MTM styling to all files
8. **Step 8:** Validate all files created with correct naming and structure

## ?? **VALIDATION REQUIREMENTS**
### Automated Tests
- [ ] Verify exactly 60 files created (54 FileDefinitions + 6 root transformations)
- [ ] Check all files use FileDefinitions- prefix
- [ ] Validate HTML syntax in all created files
- [ ] Confirm all CSS references are correct and accessible

### Manual Verification
- [ ] Spot-check PlainEnglish files for non-technical, process-oriented content
- [ ] Verify Technical files maintain full developer context
- [ ] Check MTM styling applied consistently
- [ ] Validate responsive design works on sample files

## ?? **CONTEXT REFERENCES**
### Related Files
- Docs/ folder (source content for migration)
- Documentation/HTML/assets/css/ (CSS files for referencing)
- User configuration answers for content treatment and structure

### MTM-Specific Requirements
- **Content Treatment:** MODERNIZE_FULL with MTM styling per user config
- **PlainEnglish Strategy:** PROCESS_ORIENTED for non-developers
- **File Naming:** FileDefinitions- prefix for ALL files
- **CSS Integration:** ALL_THREE CSS files + responsive.css
- **HTML Structure:** Match existing template structure in Documentation/HTML

## ?? **ERROR HANDLING**
### Expected Issues
- **Missing source files:** Some FileDefinitions may not exist in Docs/
- **Content formatting:** Original HTML may need significant cleanup
- **File naming conflicts:** Handle existing files with similar names

### Rollback Plan
- Delete all created FileDefinitions files
- Restore from backup if any original files were modified
- Document any files that couldn't be migrated for manual review

## ?? **CHECKPOINT MARKERS**
- ? ViewModels migrated (26 files: 13 PlainEnglish + 13 Technical)
- ? Services migrated (22 files: 11 PlainEnglish + 11 Technical)
- ? Models migrated (4 files: 2 PlainEnglish + 2 Technical)
- ? Extensions migrated (2 files: 1 PlainEnglish + 1 Technical)
- ? Root files transformed (6 files: 3 PlainEnglish + 3 Technical)
- ? All 60 files validated and ready for naming standardization (Fix2)

## ?? **OPTIMIZATION NOTES**
### Efficiency Tips
- Process files in batches by category for consistency
- Use template approach for HTML structure to maintain consistency
- Create PlainEnglish and Technical versions simultaneously for each file

### Success Indicators
- Exact file count achieved (60 total files)
- All files have proper MTM styling and responsive design
- PlainEnglish content accessible to non-developers
- Technical content maintains full developer value
- No broken links or missing CSS references

## ?? **Content Transformation Guidelines**
### PlainEnglish Content (Process-Oriented)
- Focus on **what** the component does and **why** it matters
- Explain business workflows and user benefits
- Use analogies and simple explanations
- Avoid technical implementation details
- Emphasize process steps and outcomes

### Technical Content (Developer-Focused)
- Maintain full technical implementation details
- Include code examples and patterns
- Reference MTM-specific requirements (TransactionType logic, stored procedures)
- Provide development context and dependencies
- Include troubleshooting and integration notes

---
*Agent-Optimized Instructions for GitHub Copilot*