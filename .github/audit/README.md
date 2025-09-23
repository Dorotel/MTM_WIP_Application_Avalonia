# MTM Pull Request Audit & Copilot Continuation System

## Overview

This system analyzes the current state of any pull request branch against its implementation plan, generates comprehensive gap reports, and creates targeted @copilot prompts for seamless continuation work across multiple sessions.

## How It Works

1. **Branch Analysis**: Scans all files in the current branch and compares against implementation plan requirements
2. **Gap Detection**: Identifies missing files, incomplete implementations, and architectural deviations
3. **Progress Tracking**: Tracks completed vs remaining work items
4. **Copilot Prompt Generation**: Creates context-aware @copilot prompts with specific instructions
5. **Session Continuity**: Supports multiple development sessions by updating progress tracking

## Usage Instructions

### Step 1: Navigate to Your Pull Request Branch
```bash
git checkout your-feature-branch
```

### Step 2: Run the Audit System
Use the audit prompt in VS Code with GitHub Copilot Chat:

**Copy and paste this into Copilot Chat:**
```
## Quick Start

### Method 1: Direct Prompt Execution (Recommended)

**Use the dedicated slash command in VS Code GitHub Copilot Chat:**

```markdown
/mtm-audit-system
```

This will execute the complete audit system using the comprehensive prompt file located at `.github/prompts/mtm-audit-system.prompt.md`.

### Method 2: Manual Template Usage

For custom gap analysis or when you prefer manual control:

1. **Create Gap Report**:
   ```powershell
   Copy-Item ".github\scripts\templates\gap-report-template.md" "my-feature-gap-report.md"
   # Edit with your feature specifics
   ```

2. **Generate Copilot Prompt**:
   ```powershell  
   Copy-Item ".github\scripts\templates\copilot-prompt-template.md" "my-feature-copilot.md"
   # Customize based on gap analysis findings
   ```

### Step 3: Review Generated Files
The system will create/update:
- `docs/audit/BRANCH-NAME-gap-report.md` - Comprehensive gap analysis
- `docs/audit/BRANCH-NAME-copilot-prompt.md` - Ready-to-use @copilot continuation prompt

### Step 4: Use Continuation Prompt
Copy the generated @copilot prompt from the `-copilot-prompt.md` file and paste it in your pull request comments or Copilot Chat to continue development.

## File Structure
```
.github/
├── prompts/
│   └── mtm-audit-system.prompt.md (main audit execution - use with /mtm-audit-system)
└── scripts/
    ├── README.md (this file)
    ├── USAGE-GUIDE.md (comprehensive usage documentation)
    └── templates/
        ├── gap-report-template.md
        ├── copilot-prompt-template.md
        └── examples/
            ├── README.md
            ├── print-service-gap-report-example.md
            └── print-service-copilot-prompt-example.md
```

## Features

- ✅ **Universal Compatibility**: Works with any MTM feature implementation
- ✅ **Smart File Detection**: Automatically finds implementation plans and requirements
- ✅ **Progress Persistence**: Tracks completed work across multiple sessions  
- ✅ **Architecture Validation**: Ensures MTM patterns and conventions compliance
- ✅ **Context Preservation**: Maintains development context for Copilot
- ✅ **Gap Prioritization**: Identifies critical missing components first
- ✅ **Session Recovery**: Handles interrupted development workflows

## Supported Features

- Print Service implementations
- UI/UX feature development
- Service layer implementations
- Database integration features
- Navigation and routing features
- Theme and styling updates
- Any MTM architectural pattern implementation

## Next Steps

1. Use the system on your current pull request
2. Review the gap report for missing components
3. Use the generated @copilot prompt to continue implementation
4. Repeat as needed for multi-session development