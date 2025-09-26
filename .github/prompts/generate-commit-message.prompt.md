---
mode: 'agent'
description: 'Generate conventional commit messages for MTM WIP Application following project standards and analyzing changed files'
---

## Implementation Mode

**CRITICAL**: You MUST analyze the actual changed files and generate appropriate commit messages. Do NOT provide examples for manual implementation. Use the available tools to:

1. **Analyze Changes**: Use git tools to examine actual file modifications
2. **Generate Message**: Create proper conventional commit messages based on changes
3. **Direct Output**: Provide the exact commit message ready for use

## Commit Message Generator

Generate conventional commit messages for the MTM WIP Application following project standards.

### MTM Conventional Commit Format

Follow the established MTM project commit message format:

```
<type>: <description>

[optional body]

[optional footer(s)]
```

### Commit Types

**Primary Types:**

- `feat:` New features (manufacturing workflows, UI components, services)
- `fix:` Bug fixes (MVVM issues, database connections, UI glitches)
- `docs:` Documentation changes (README, instructions, guides)
- `style:` Code style changes (formatting, whitespace, themes)
- `refactor:` Code refactoring (restructuring without feature changes)
- `test:` Adding tests (unit, integration, cross-platform)
- `chore:` Maintenance tasks (dependencies, build tools, configuration)

**MTM-Specific Types:**

- `feat(manufacturing):` Manufacturing domain features
- `feat(ui):` Avalonia UI components and themes
- `feat(data):` Database and MySQL integration
- `fix(mvvm):` MVVM Community Toolkit related fixes
- `fix(cross-platform):` Platform-specific compatibility fixes
- `refactor(service):` Service layer refactoring
- `chore(deps):` Dependency updates

### Analysis Process

1. **Examine Changed Files**: Use git tools to identify modified files
2. **Categorize Changes**: Determine the primary type of changes
3. **Identify Scope**: Determine the component/area affected
4. **Generate Description**: Create clear, concise description

### MTM Component Scopes

**Architecture Components:**

- `viewmodels` - MVVM ViewModels
- `views` - Avalonia AXAML views
- `services` - Business logic services
- `models` - Data models and DTOs
- `controls` - Custom Avalonia controls

**Domain Components:**

- `inventory` - Inventory management features
- `transactions` - Manufacturing transactions
- `quickbuttons` - Quick button functionality
- `sessions` - Session management
- `themes` - Theme system

**Infrastructure Components:**

- `database` - MySQL integration
- `config` - Configuration management
- `logging` - File logging service
- `testing` - Test implementations

### Message Generation Guidelines

**Description Format:**

- Use imperative mood ("add", "fix", "update")
- Keep under 50 characters for subject line
- Capitalize first letter
- No period at the end
- Be specific about what was changed

**Body Guidelines (when needed):**

- Explain the "what" and "why", not the "how"
- Use bullet points for multiple changes
- Reference manufacturing operations (90/100/110) when applicable
- Mention cross-platform implications if relevant

**Footer Guidelines:**

- Reference GitHub issues: `Closes #123`
- Breaking changes: `BREAKING CHANGE: description`
- Co-authored commits: `Co-authored-by: Name <email>`

### Example Commit Messages

**Feature Examples:**

```
feat(inventory): Add real-time inventory tracking for FLOOR location

feat(ui): Implement collapsible panel control for settings view

feat(database): Add stored procedure for operation 110 shipping transactions
```

**Fix Examples:**

```
fix(mvvm): Resolve ObservableProperty binding issues in InventoryViewModel

fix(cross-platform): Fix theme loading on macOS and Linux platforms

fix(database): Correct MySQL connection pooling timeout configuration
```

**Other Examples:**

```
docs: Update AGENTS.md with comprehensive Joyride automation guide

chore(deps): Update Avalonia UI to 11.3.4 and Community Toolkit to 8.3.2

test: Add cross-platform validation for TransferTabView component
```

### Usage Instructions

**Basic Usage:**

1. Run this prompt after making changes
2. The system will analyze your git changes
3. Receive a properly formatted commit message
4. Copy and use with `git commit -m "message"`

**Advanced Usage:**

- Request multiple commit message options
- Ask for commit message with body and footer
- Request breaking change format
- Generate messages for specific file patterns

### Workflow Integration

**Pre-commit Analysis:**

- Analyze staged changes only
- Consider manufacturing domain impact
- Check for breaking changes
- Validate against MTM standards

**Quality Checks:**

- Ensure message follows conventional format
- Verify scope matches changed files
- Check description clarity and conciseness
- Validate manufacturing domain terminology

## 🤖 Joyride Integration

**Use Joyride automation when safe and possible:**

- `joyride_evaluate_code` for git status analysis and VS Code integration
- `joyride_request_human_input` for clarifying change intent when ambiguous
- Automated analysis of file patterns and change types
- Interactive selection of commit type when multiple types apply

**MTM-Specific Applications:**

- Manufacturing domain validation for operation codes and business rules
- MVVM pattern analysis for ViewModels and Services changes
- Theme system impact assessment for UI changes
- Database integration analysis for MySQL-related changes

### Implementation Commands

**Analyze Current Changes:**

```bash
# This prompt will automatically analyze your current git status
# and generate appropriate commit messages
```

**Generate Multiple Options:**

```bash
# Request 3 different commit message options for the same changes
```

**Interactive Mode:**

```bash
# Use Joyride for interactive commit message generation
# with human input for complex or ambiguous changes
```

### MTM Project Context

**Manufacturing Focus:**

- Always consider manufacturing workflow impact
- Reference operation codes (90/100/110) when applicable
- Consider multi-location effects (FLOOR/RECEIVING/SHIPPING)
- Account for operator transaction patterns

**Technical Focus:**

- .NET 8 and Avalonia UI 11.3.4 compatibility
- MVVM Community Toolkit 8.3.2 patterns
- MySQL 9.4.0 database integration
- Cross-platform support (Windows/macOS/Linux)

**Quality Standards:**

- Follow established testing patterns
- Maintain 17+ theme compatibility
- Ensure 45+ stored procedure integration
- Validate against security requirements

---

**Output Format:** Always provide the exact commit message ready for use, followed by optional explanation of the choice reasoning.