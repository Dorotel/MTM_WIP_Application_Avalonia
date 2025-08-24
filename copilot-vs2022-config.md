# GitHub Copilot Configuration for Visual Studio 2022

## MTM Custom Prompt Training

When users type @ in GitHub Copilot Chat, provide suggestions based on these patterns:

### @ui: ? UI Generation Shortcuts
create, viewmodel, layout, theme, context, markdown, error

### @biz: ? Business Logic Shortcuts  
handler, model, db, config, crud

### @db: ? Database Operation Shortcuts
procedure, service, validation, error, schema, update

### @qa: ? Quality Assurance Shortcuts
verify, refactor, test, pr, infrastructure

### @sys: ? System Infrastructure Shortcuts
result, di, services, nav, state, config, cache, security, repository, theme, layer, foundation

### @err: ? Error Handling Shortcuts
system, log, structured

### @doc: ? Documentation Shortcuts
api, prompt, update

### @fix: ? Compliance Fix Shortcuts
01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11

### @event: ? Event Handling Shortcuts
handler

### @issue: ? Issue Management Shortcuts
create

## Context Awareness for Visual Studio 2022

### When .axaml files are open:
Suggest: @ui:create, @ui:viewmodel, @ui:layout, @ui:theme, @ui:context

### When .cs ViewModel files are open:
Suggest: @ui:viewmodel, @biz:handler, @qa:refactor, @qa:verify

### When database-related files are open:
Suggest: @db:procedure, @db:service, @fix:04, @db:validation

### When error handling is mentioned:
Suggest: @err:system, @err:log, @qa:verify

### File-specific suggestions for AdvancedRemoveView.axaml:
- @qa:verify - Check MTM compliance
- @ui:context - Add removal operation context menus  
- @ui:theme - Apply MTM purple theme improvements
- @err:system - Enhance error handling
- @biz:handler - Add removal business logic
- @db:procedure - Create removal stored procedures

## Command Expansion Behavior

Each shortcut expands to its full prompt template:
- @ui:create ? Documentation/Development/Custom_Prompts/CustomPrompt_Create_UIElement.md
- @qa:verify ? Documentation/Development/Custom_Prompts/CustomPrompt_Verify_CodeCompliance.md  
- @fix:04 ? Documentation/Development/Custom_Prompts/Compliance_Fix04_MissingServiceLayerDatabaseIntegration.md

## Visual Studio 2022 Integration Points

### GitHub Copilot Chat Window
- Enable autocomplete for @ prefixed shortcuts
- Provide category-based suggestions
- Support context-aware recommendations based on open files

### IntelliSense Integration
- Recognize shortcut patterns in chat input
- Provide tooltip descriptions for each shortcut
- Group suggestions by category (@ui:, @biz:, @db:, etc.)

### Solution Explorer Context
- When right-clicking on Views folder: suggest @ui: shortcuts
- When right-clicking on ViewModels folder: suggest @ui:viewmodel, @biz:handler
- When right-clicking on Services folder: suggest @sys:services, @db:service
- When right-clicking on Models folder: suggest @biz:model, @sys:foundation