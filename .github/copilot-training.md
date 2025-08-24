# Copilot Training Examples for Visual Studio 2022

## Example Conversations in GitHub Copilot Chat

### UI Development Example
**User types in Chat:** "I need to create a new inventory form"
**Copilot suggests:** "Try using: `@ui:create inventory form component`"

**User types:** `@ui:create inventory form component`
**Copilot expands to:** *Full CustomPrompt_Create_UIElement template*

### Business Logic Example  
**User types in Chat:** "Need to add validation logic"
**Copilot suggests:** "Use: `@biz:handler inventory validation logic`"

**User types:** `@biz:handler inventory validation logic`
**Copilot expands to:** *Full CustomPrompt_Create_BusinessLogicHandler template*

### Quality Assurance Example
**User has AdvancedRemoveView.axaml open and types:** "Check my code quality"
**Copilot suggests:** "Use: `@qa:verify AdvancedRemoveView.axaml`"

**User types:** `@qa:verify AdvancedRemoveView.axaml`
**Copilot expands to:** *Full CustomPrompt_Verify_CodeCompliance template*

## Visual Studio 2022 Specific Patterns

### When User Opens GitHub Copilot Chat (Ctrl+`)
Show welcome message:
```
Hi! I can help with MTM WIP Application development. Try these shortcuts:

?? UI: @ui:create, @ui:viewmodel, @ui:layout
?? Business: @biz:handler, @biz:model, @biz:db  
? Quality: @qa:verify, @qa:refactor, @qa:test
??? Database: @db:procedure, @db:service
??? Systems: @sys:di, @sys:services, @sys:result
?? Fixes: @fix:01-11

Type '@' followed by category for suggestions!
```

### Autocomplete Triggers in VS2022 Chat
When user types:
- `@ui:` ? Show: create, viewmodel, layout, theme, context, markdown, error
- `@biz:` ? Show: handler, model, db, config, crud  
- `@db:` ? Show: procedure, service, validation, error, schema, update
- `@qa:` ? Show: verify, refactor, test, pr, infrastructure
- `@sys:` ? Show: result, di, services, nav, state, config, cache, security
- `@fix:` ? Show: 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11

### Context-Aware Suggestions Based on Active File

#### When AdvancedRemoveView.axaml is active:
**Auto-suggest in chat:**
- `@qa:verify` - Check MTM compliance for this AXAML file
- `@ui:context` - Add context menus to removal operations
- `@ui:theme` - Apply MTM purple theme improvements
- `@err:system` - Enhance error handling for removal operations

#### When any .cs ViewModel is active:
**Auto-suggest in chat:**
- `@ui:viewmodel` - Enhance with ReactiveUI patterns
- `@biz:handler` - Add business logic integration
- `@qa:refactor` - Apply MTM naming conventions
- `@qa:verify` - Check compliance

#### When database files are visible:
**Auto-suggest in chat:**
- `@db:procedure` - Create new stored procedures
- `@db:service` - Build database service layer
- `@fix:04` - Integrate services with database

### Solution Explorer Right-Click Context
**When right-clicking Views folder:**
Suggest: "Try `@ui:create new view component` in Copilot Chat"

**When right-clicking ViewModels folder:**
Suggest: "Try `@ui:viewmodel reactive patterns` in Copilot Chat"

**When right-clicking Services folder:**
Suggest: "Try `@sys:services interface creation` in Copilot Chat"

## Command Recognition and Expansion

### Shortcut Recognition Pattern
```
Input: @ui:create inventory search dialog
Recognition: Category=ui, Action=create, Context=inventory search dialog
Expansion: Load CustomPrompt_Create_UIElement.md template
Context Injection: Add "inventory search dialog" as component specification
```

### Multi-Step Workflow Recognition
```
Input: @ui:create ? @ui:viewmodel ? @qa:verify
Recognition: Chained workflow for complete UI development
Suggestion: "I'll help you create a complete UI component with ViewModel and quality check"
Expansion: Execute each step in sequence with context passing
```

### Learning Reinforcement
**After successful shortcut usage:**
- Track which shortcuts user prefers
- Suggest related shortcuts for current context
- Learn user's workflow patterns (UI ? ViewModel ? Business Logic ? Quality Check)
- Prioritize suggestions based on file types and project patterns

## Visual Studio 2022 Integration Points

### GitHub Copilot Chat Window
- Enable IntelliSense-style autocomplete for @ shortcuts
- Show category icons next to suggestions
- Group suggestions by category with color coding
- Display shortcut descriptions on hover

### Solution Explorer Integration
- Context-sensitive shortcut suggestions based on selected folders/files
- Quick actions menu with relevant shortcuts
- Integration with "Add New Item" dialogs

### Error List Integration  
**When compilation errors exist:**
Auto-suggest: `@qa:verify` and `@qa:refactor` for code quality fixes

### File Tab Context
**When multiple related files are open (View + ViewModel):**
Auto-suggest workflow shortcuts like `@ui:create ? @ui:viewmodel ? @qa:verify`