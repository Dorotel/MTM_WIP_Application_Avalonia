# FileDefinitions_HTML_Migration_Fix1 - Agent Instructions

## ?? **AGENT EXECUTION CONTEXT**
**Issue Type:** Fix
**Complexity:** Medium
**Estimated Time:** 30min
**Dependencies:** None (First step in migration process)

## ?? **PRECISE OBJECTIVES**
### Primary Goal
Create comprehensive backup of entire Documentation folder and prepare directory structure for FileDefinitions migration.

### Acceptance Criteria
- [ ] Complete backup of Documentation folder created with timestamp
- [ ] Documentation/HTML/PlainEnglish/FileDefinitions/ directory created
- [ ] Documentation/HTML/Technical/FileDefinitions/ directory created
- [ ] Backup integrity verified (all files accessible)
- [ ] Directory structure validated and ready for migration

## ?? **IMPLEMENTATION DETAILS**

### Files to Modify/Create
```
Documentation-Backup-{timestamp}/ - Complete backup folder
Documentation/HTML/PlainEnglish/FileDefinitions/ - New directory
Documentation/HTML/Technical/FileDefinitions/ - New directory
Documentation/HTML/PlainEnglish/FileDefinitions/ViewModels/ - Subdirectory
Documentation/HTML/PlainEnglish/FileDefinitions/Services/ - Subdirectory
Documentation/HTML/PlainEnglish/FileDefinitions/Models/ - Subdirectory
Documentation/HTML/PlainEnglish/FileDefinitions/Extensions/ - Subdirectory
Documentation/HTML/Technical/FileDefinitions/ViewModels/ - Subdirectory
Documentation/HTML/Technical/FileDefinitions/Services/ - Subdirectory
Documentation/HTML/Technical/FileDefinitions/Models/ - Subdirectory
Documentation/HTML/Technical/FileDefinitions/Extensions/ - Subdirectory
```

### Code Patterns Required
```powershell
# PowerShell backup command pattern
$timestamp = Get-Date -Format "yyyy-MM-dd-HH-mm-ss"
Copy-Item -Path "Documentation" -Destination "Documentation-Backup-$timestamp" -Recurse -Force

# Directory creation pattern
New-Item -ItemType Directory -Path "Documentation/HTML/PlainEnglish/FileDefinitions" -Force
New-Item -ItemType Directory -Path "Documentation/HTML/Technical/FileDefinitions" -Force
```

### Database Operations (If Applicable)
Not applicable for this fix.

## ? **EXECUTION SEQUENCE**
1. **Step 1:** Generate timestamp for backup naming
2. **Step 2:** Create complete backup of Documentation folder to Documentation-Backup-{timestamp}
3. **Step 3:** Verify backup integrity by spot-checking key files
4. **Step 4:** Create PlainEnglish/FileDefinitions directory structure with subdirectories
5. **Step 5:** Create Technical/FileDefinitions directory structure with subdirectories
6. **Step 6:** Validate all directories created successfully
7. **Step 7:** Document backup location and directory structure

## ?? **VALIDATION REQUIREMENTS**
### Automated Tests
- [ ] Verify backup folder exists and contains all Documentation files
- [ ] Verify new directory structure exists with correct permissions
- [ ] Compare file counts between original and backup

### Manual Verification
- [ ] Open random files from backup to ensure accessibility
- [ ] Navigate to new directories to confirm structure
- [ ] Check that backup is complete and not corrupted

## ?? **CONTEXT REFERENCES**
### Related Files
- Documentation/ (entire folder to backup)
- Documentation/HTML/ (target location for new structure)

### MTM-Specific Requirements
- **Backup Strategy:** FULL_DOCUMENTATION_BACKUP per user configuration
- **Directory Structure:** Confirmed target structure per user answers
- **File Organization:** Prepare for BY_CATEGORY processing order

## ?? **ERROR HANDLING**
### Expected Issues
- **Insufficient disk space:** Check available space before backup
- **Permission errors:** Ensure write permissions to backup location
- **Existing backup folders:** Handle naming conflicts with timestamp

### Rollback Plan
- Delete any partially created directories
- Remove incomplete backup if backup fails
- No rollback needed as this is first step with no destructive changes

## ?? **CHECKPOINT MARKERS**
- ? Backup created successfully
- ? Directory structure prepared
- ? Validation completed
- ? Ready for CSS modernization (Phase1)

## ?? **OPTIMIZATION NOTES**
### Efficiency Tips
- Use robocopy or xcopy for faster large directory copying
- Create directories in parallel if possible
- Verify backup incrementally during copy process

### Success Indicators
- Backup folder size matches Documentation folder size
- All target directories exist and are writable
- No error messages during directory creation

---
*Agent-Optimized Instructions for GitHub Copilot*