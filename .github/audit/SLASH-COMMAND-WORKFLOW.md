# MTM Feature Development Workflow - Slash Command Guide

This guide shows you how to develop new MTM features using only slash commands and prompts. Follow this step-by-step process for consistent, high-quality feature implementation.

## Prerequisites

- Have VS Code with GitHub Copilot extension installed
- Be working in the MTM_WIP_Application_Avalonia repository
- Have the feature idea/requirements ready

---

## Step 1: Create Implementation Plan

**Where**: VS Code Copilot Chat  
**Command**:
```
/create-implementation-plan
```

**What happens**: Copilot will guide you through creating a comprehensive implementation plan document similar to the Print Service plan.

**You'll be prompted for**:
- Feature name and description
- Technical requirements
- UI/UX specifications
- Architecture considerations
- File structure and integration points

**Result**: Creates `docs/ways-of-work/plan/{your-feature}/implementation-plan/implementation-plan.md`

---

## Step 2: Generate GitHub Issue Template

**Where**: VS Code Copilot Chat  
**Command**:
```
/create-github-action-workflow-specification
```

**Follow up with**:
```
Based on the implementation plan at docs/ways-of-work/plan/{your-feature}/implementation-plan/implementation-plan.md, 
create a comprehensive GitHub issue template identical in format to 
docs/ways-of-work/plan/print-service/COPILOT-ISSUE-TEMPLATE.md but for {your-feature}.

Include:
- Complete feature overview and business need
- Detailed technical requirements from implementation plan  
- Expected files to create/modify
- Integration points and dependencies
- Acceptance criteria
- Special instructions for Copilot
- #github-pull-request_copilot-coding-agent hashtag at end
```

**Result**: Creates `docs/ways-of-work/plan/{your-feature}/COPILOT-ISSUE-TEMPLATE.md`

---

## Step 3: Create GitHub Issue and Start Implementation

### 3a: Create the GitHub Issue

**Where**: GitHub repository → Issues → New Issue  
**Action**: Copy the entire content from your `COPILOT-ISSUE-TEMPLATE.md` file and paste it as a new GitHub issue.

**Important**: Make sure the issue ends with `#github-pull-request_copilot-coding-agent` hashtag.

### 3b: Wait for First Implementation Session

**What happens**: The GitHub Copilot coding agent will:
- Create a new branch for your feature
- Implement the initial components following MTM patterns
- Open a Pull Request with the first session's work
- Post progress updates in the issue comments

**Timeline**: Usually 10-30 minutes depending on feature complexity

---

## Step 4: Run Audit and Generate Continuation Prompt

### 4a: Switch to the Feature Branch

**Where**: VS Code Terminal  
**Command**:
```bash
git checkout {feature-branch-name}
```

### 4b: Run MTM Audit System

**Where**: VS Code Copilot Chat  
**Command**:
```
/mtm-audit-system
```

**What happens**: 
- Analyzes current implementation vs your implementation plan
- Identifies gaps and missing components
- Generates comprehensive gap report
- Creates ready-to-use Copilot continuation prompt

**Results**: 
- `docs/audit/{branch-name}-gap-report.md` (detailed analysis)
- `docs/audit/{branch-name}-copilot-prompt.md` (continuation prompt)

### 4c: Use Continuation Prompt

**Where**: GitHub Pull Request Comments  
**Action**: 
1. Open the Pull Request created by Copilot
2. Go to the conversation/comments section
3. Copy the entire content from `docs/audit/{branch-name}-copilot-prompt.md`
4. Paste it as a new PR comment

**Result**: Copilot continues implementation based on gap analysis and priorities

---

## Step 5: Repeat Audit Cycle (If Needed)

For complex features, repeat Step 4 until implementation is complete:

1. **Check Progress**: Review the PR updates from Copilot
2. **Run Audit**: Use `/mtm-audit-system` again to see remaining gaps
3. **Continue**: Paste the new continuation prompt in PR comments
4. **Validate**: Test the implementation and verify acceptance criteria

---

## Command Quick Reference

| Phase | Location | Command | Purpose |
|-------|----------|---------|---------|
| Plan | VS Code Chat | `/create-implementation-plan` | Create feature plan |
| Template | VS Code Chat | `/create-github-action-workflow-specification` + custom prompt | Generate issue template |
| Implement | GitHub Issue | Paste COPILOT-ISSUE-TEMPLATE.md content | Start automated implementation |
| Audit | VS Code Chat | `/mtm-audit-system` | Analyze gaps and generate continuation |
| Continue | GitHub PR Comments | Paste audit continuation prompt | Resume implementation |

---

## Example Workflow

**Feature**: "User Settings Export/Import"

```bash
# Step 1: Create plan
/create-implementation-plan

# Step 2: Generate template (in VS Code Chat)
Based on docs/ways-of-work/plan/user-settings-export/implementation-plan/implementation-plan.md, 
create GitHub issue template like print-service COPILOT-ISSUE-TEMPLATE.md for user settings export/import feature.

# Step 3: Create GitHub issue (paste template content with #github-pull-request_copilot-coding-agent)

# Step 4: Switch to feature branch and audit
git checkout feature/user-settings-export
/mtm-audit-system

# Step 5: Continue in PR (paste docs/audit/{branch}-copilot-prompt.md content)
```

---

## Tips for Success

### Planning Phase
- Be specific about business requirements and user workflows
- Include all integration points and dependencies
- Reference similar existing features for pattern consistency

### Implementation Phase
- Let the first session complete before running audit
- Check PR progress regularly but don't interrupt mid-session
- Review generated code for MTM pattern compliance

### Continuation Phase
- Always run audit before requesting continuation
- Use specific, actionable prompts from the audit system
- Focus on one priority area at a time for complex features

### Quality Assurance
- Test all acceptance criteria before marking complete
- Verify theme compatibility across all MTM variants
- Ensure error handling follows established patterns

---

**This workflow ensures consistent, high-quality feature development while leveraging GitHub Copilot's automated implementation capabilities and MTM's established architectural patterns.**
