# 📋 GitHub Copilot Guide: Creating Phase 2 GitHub Issue

## 🎯 **Overview**

This guide provides instructions for GitHub Copilot to create a comprehensive GitHub issue for Phase 2 implementation using our complete Phase 1 foundation. The issue will leverage all established documentation, architectural patterns, and GitHub Copilot context optimization.

---

## 🔧 **Pre-Creation Validation**

### **Step 1: Verify Phase 1 Foundation**
Copilot should confirm these assets are accessible before creating the issue:

✅ **Documentation Assets (Complete)**
- `docs/ways-of-work/plan/mtm-inventory-management/Project_Architecture_Blueprint.md`
- `docs/ways-of-work/plan/mtm-inventory-management/Project_Folders_Structure_Blueprint.md`
- Complete `.github/instructions/` folder with 7 instruction files
- Epic and Feature PRDs (8 documents total)

✅ **GitHub Repository Context**
- Repository: `Dorotel/MTM_WIP_Application_Avalonia`
- Branch: `master` (default)
- User has appropriate permissions for issue creation

---

## 🚀 **Issue Creation Process**

### **Step 2: Navigate to GitHub Issues**

1. **Go to Repository**
   ```
   https://github.com/Dorotel/MTM_WIP_Application_Avalonia
   ```

2. **Click "Issues" Tab**
   - Located in repository navigation bar
   - Should show existing issues (if any)

3. **Click "New Issue" Button**
   - Green button in top-right of issues page
   - May show template options if any exist

### **Step 3: Configure Issue Basic Information**

#### **Issue Title** (Copy Exactly)
```
Phase 2: Infrastructure - GitHub Project Management and Memory Bank System Implementation
```

#### **Issue Labels** (Add These)
- `enhancement` - Feature enhancement
- `documentation` - Documentation work
- `infrastructure` - Infrastructure setup
- `phase-2` - Phase identification
- `awesome-copilot` - Project initiative

*Note: Create labels if they don't exist*

#### **Assignees**
- Assign to yourself (issue creator and manager)
- GitHub Copilot will implement the tasks through AI assistance

#### **Projects** 
- Link to GitHub Project if one exists
- Will create GitHub Projects board as part of Phase 2

#### **Milestone**
- Create "Phase 2: Infrastructure" milestone
- Set due date 3-4 weeks from creation

### **Step 4: Copy Issue Body Content**

**Copy the entire content** from the file we just created:
`docs/ways-of-work/plan/mtm-inventory-management/Phase_2_GitHub_Issue_Template.md`

**Key sections to verify are included:**
- 🎯 Objective with Phase 1 completion reference
- 📋 Complete Phase 1 foundation summary  
- 🚀 Phase 2 scope with 25 detailed tasks
- 🎯 Detailed task breakdown with acceptance criteria
- 🏗️ Technical implementation approach
- 📂 Phase 1 foundation references (critical for context)
- 🚀 Implementation approach and timeline

### **Step 5: Enhance with Repository-Specific Context**

#### **Add Repository Links** (Update these in the issue body)
Replace placeholder references with actual links:

```markdown
### **Phase 1 Foundation References** (UPDATE THESE)
- **Architecture Blueprint**: [Project_Architecture_Blueprint.md](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/blob/master/docs/ways-of-work/plan/mtm-inventory-management/Project_Architecture_Blueprint.md)
- **Folders Structure**: [Project_Folders_Structure_Blueprint.md](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/blob/master/docs/ways-of-work/plan/mtm-inventory-management/Project_Folders_Structure_Blueprint.md)
- **GitHub Instructions**: [.github/instructions/](https://github.com/Dorotel/MTM_WIP_Application_Avalonia/tree/master/.github/instructions)
```

#### **Add Implementation Context**
```markdown
### **Implementation Assignment**
- **Issue Creator**: User (creating and managing the issue)
- **Implementation**: GitHub Copilot (AI assistant will complete all 25 Phase 2 tasks)
- **Oversight**: User (review and approval of Copilot's work)
- **Integration**: Copilot will reference all Phase 1 foundation documents for context
```

### **Step 6: Create Connected Issues (Optional)**

For large-scale implementation, consider breaking into connected issues:

#### **Create Child Issues** (Link to main Phase 2 issue)
```markdown
**Related Issues:**
- [ ] #XXX - GitHub Projects Board Setup (MTM_PROJ_001-008)
- [ ] #XXX - Task Planning System (MTM_TASK_001-006)  
- [ ] #XXX - Memory Bank Implementation (MTM_MEMORY_001-007)
- [ ] #XXX - Component Documentation (MTM_COMP_001-004)
```

---

## ⚡ **Advanced Configuration**

### **Step 7: GitHub Projects Integration**

If creating GitHub Projects board as part of Phase 2:

1. **Create New Project**
   - Go to repository "Projects" tab
   - Click "New project"
   - Choose "Board" layout
   - Name: "MTM Awesome-Copilot Implementation"

2. **Configure Custom Fields**
   ```
   - Priority: High, Medium, Low, Critical
   - Component: ViewModels, Services, Database, UI, Infrastructure
   - Effort: 1, 2, 3, 5, 8, 13 (Fibonacci)
   - Phase: 1, 2, 3, 4
   ```

3. **Add Views**
   - By Phase (filter by Phase field)
   - By Component (group by Component field)
   - By Priority (sort by Priority field)

### **Step 8: Automation Setup**

#### **GitHub Actions Workflow** (Create if needed)
```yaml
# .github/workflows/issue-management.yml
name: Issue Management
on:
  issues:
    types: [opened, labeled, assigned]
jobs:
  auto-assign-project:
    runs-on: ubuntu-latest
    steps:
      - name: Add to project
        uses: actions/add-to-project@v0.4.0
        with:
          project-url: https://github.com/users/Dorotel/projects/[PROJECT-NUMBER]
          github-token: ${{ secrets.GITHUB_TOKEN }}
```

---

## 📊 **Issue Creation Checklist**

### **Before Creating Issue**
- [ ] Phase 1 documentation complete and accessible
- [ ] Repository write permissions confirmed
- [ ] Ready to guide GitHub Copilot through implementation
- [ ] GitHub Projects board planned or existing

### **During Issue Creation**
- [ ] Title uses exact format specified
- [ ] All 5 labels applied (enhancement, documentation, infrastructure, phase-2, awesome-copilot)
- [ ] Complete issue body from template copied
- [ ] Repository-specific links updated
- [ ] Self-assigned for management and Copilot coordination
- [ ] Milestone created and assigned

### **After Issue Creation**
- [ ] Issue number recorded for reference
- [ ] GitHub Copilot notified and ready to begin implementation
- [ ] Connected to GitHub Projects board
- [ ] Child issues created if using breakdown approach
- [ ] Automation rules tested

---

## 🎯 **Success Validation**

### **Issue Quality Check**
- [ ] **Comprehensive Scope**: All 25 Phase 2 tasks clearly defined
- [ ] **Clear Acceptance Criteria**: Each task has measurable completion criteria
- [ ] **Technical Context**: Phase 1 foundation properly referenced
- [ ] **Implementation Ready**: No missing dependencies or prerequisites
- [ ] **Copilot Context**: GitHub Copilot has access to all Phase 1 foundation documents

### **GitHub Integration**
- [ ] **Proper Labeling**: All labels applied and visible
- [ ] **Project Association**: Issue appears in GitHub Projects board
- [ ] **Milestone Tracking**: Due date and progress trackable
- [ ] **Automation Active**: Workflow rules functioning correctly

---

## 📚 **Reference Materials**

### **Phase 1 Foundation Documents**
All these documents provide context for Phase 2 implementation:

1. **Project Architecture Blueprint** - Complete architectural overview
2. **Project Folders Structure Blueprint** - File organization analysis
3. **GitHub Instructions System** - 7 specialized Copilot instruction files
4. **Epic and Feature PRDs** - 8 comprehensive technical specifications
5. **Service Architecture Documentation** - Complete service layer analysis
6. **Data Models Documentation** - Domain-driven design patterns

### **GitHub Best Practices**
- **Issue Templates**: Consider creating reusable templates for future phases
- **Label Management**: Maintain consistent labeling across issues
- **Milestone Planning**: Use milestones for phase-based planning
- **Project Boards**: Leverage GitHub Projects for visual progress tracking

---

## 🚀 **Expected Timeline**

### **Issue Creation Process**
- **Setup and Preparation**: 15-30 minutes
- **Issue Creation**: 10-15 minutes  
- **Configuration and Links**: 15-30 minutes
- **Team Notification**: 5-10 minutes
- **Total Time**: 45-90 minutes for comprehensive setup

### **Phase 2 Implementation**
- **GitHub Project Management**: 1-2 weeks
- **Task Planning System**: 3-5 days
- **Memory Bank System**: 1-2 weeks  
- **Component Documentation**: 3-5 days
- **Total Phase 2 Duration**: 3-4 weeks

---

**This guide ensures your Phase 2 GitHub issue leverages all Phase 1 foundation work and sets up comprehensive infrastructure for continued awesome-copilot implementation success.**
