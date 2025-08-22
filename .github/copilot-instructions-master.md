# Copilot Instructions (Master)

This repository uses modular instruction files for clarity and maintainability.  
Refer to the sections below for detailed guidance.

---

## Table of Contents

- [Coding Conventions](coding-conventions.instruction.md)
- [UI Generation Guidelines](ui-generation.instruction.md)
- [UI Mapping Reference](ui-mapping.instruction.md)
- [Error Handling & Logging](error_handler-instruction.md)
- [GitHub Workflow](github-workflow.instruction.md)
- [Naming Conventions](naming-conventions.instruction.md)
- [Custom Prompts](custom-prompts.instruction.md)
- [Copilot Personas Reference](personas-instruction.md)

---

If you are unsure which document to use, start here, then follow the links above.

---

## Synchronization with Website Help Files

> **Important:**  
> Whenever you make any change to any of the instruction files in this repository, you must also update the corresponding HTML help file (located in the docs folder, e.g., docs/coding-conventions.html for coding-conventions.instruction.md) to reflect the change.  
> If the change affects navigation, structure, or style, you must also update the docs/index.html and/or docs/styles.css files as required to keep the web help site in sync and consistent.
>
> **Before starting any change to an instruction file, you must acknowledge that you are responsible for keeping the HTML help site synchronized. Announce to the team (or in your commit/pull request) that you acknowledge this requirement and that you are about to start the update.**
>
> This ensures that all users always have access to the most up-to-date and accurate documentation—both in markdown and on the website.

---

## Instruction File to HTML File Mapping

All HTML files are located in the docs folder in the root directory of the repository.  
Below is the mapping of instruction files to their corresponding HTML files:

| Instruction File                      | HTML File in docs/                   |
|----------------------------------------|--------------------------------------|
| copilot-instructions.md                | docs/index.html                      |
| coding-conventions.instruction.md      | docs/coding-conventions.html         |
| ui-generation.instruction.md           | docs/ui-generation.html              |
| ui-mapping.instruction.md              | docs/ui-mapping.html                 |
| error_handler-instruction.md           | docs/error-handler.html              |
| github-workflow.instruction.md         | docs/github-workflow.html            |
| naming-conventions.instruction.md      | docs/naming-conventions.html         |
| custom-prompts.instruction.md          | docs/custom-prompts.html             |
| personas-instruction.md                | docs/personas.html                   |
| custom-prompts-examples.md             | docs/custom-prompts-examples.html    |
| (global styles for all HTML)           | docs/styles.css                      |

---

## Configuration Files

Configuration placeholders live under `Config/`. Primary file:
- `Config/appsettings.json` with sections:
  - `Application` (Name, Environment, Theme, Language)
  - `ErrorHandling` (EnableFileServerLogging, EnableMySqlLogging, EnableConsoleLogging, FileServerBasePath, MySqlConnectionString)
  - `Logging` (Level, File.Enable, File.Path)
  - `Database` (Provider, ConnectionString)

Guidance:
- Do not commit secrets in Git. Prefer environment variables, user secrets, or CI secret stores.
- Wire-ups should be implemented in services (e.g., `ErrorHandlingConfiguration.LoadFromConfiguration()` reads `ErrorHandling` section).
- Keep this file environment-specific via transforms or per-machine overrides when needed.

---

## Recent Workspace Updates (2025-08-20)

Acknowledgement: Updating this instruction file and synchronizing docs/index.html per the synchronization policy above.

### Error Handling System (Scaffolded)
- Added centralized error handling and logging scaffolding under Services/:
  - Service_ErrorHandler.cs (session duplicate detection, category routing)
  - LoggingUtility.cs (CSV file server logging, MySQL logging stubs + table ensure)
  - ErrorSeverity.cs, ErrorCategory.cs, ErrorEntry.cs (models/enums)
  - ErrorHandlingConfiguration.cs (config flags/paths, validation placeholders)
  - ErrorMessageProvider.cs (user-facing messages, recommendations, titles)
  - ErrorHandlingInitializer.cs (startup helpers, development mode, self-test)
- Added examples:
  - Examples/ErrorHandlingUsageExample.cs
- NuGet: Added MySql.Data (9.1.0) for future DB logging integration.

### UI Elements for Error Messages (Avalonia)
- Created inline error control for non-blocking display:
  - Controls/Control_ErrorMessage.axaml (+ .axaml.cs). Uses FindControl, severity-based visuals, retry/report/copy events, technical details expander.
- Created modal enhanced error dialog:
  - Views/ErrorDialog_Enhanced.axaml (+ .axaml.cs minimal/stub implementation to compile). Will be iterated as needed.
- Added examples:
  - Examples/ErrorMessageUIUsageExample.cs
- Developer docs:
  - Controls/README_ErrorMessageUI.md
  - Services/README.md (error system)

### Configuration (Placeholders)
- Added `Config/appsettings.json` and `Config/README.md` documenting fields for Application, ErrorHandling, Logging, Database.
- Next step: Implement reading `ErrorHandling` section in `ErrorHandlingConfiguration.LoadFromConfiguration()`.

### Build/Compatibility Fixes (Avalonia 11 / .NET 8)
- Replaced DataGrid compiled column bindings in Views/ErrorDialog_Enhanced.axaml with AutoGenerateColumns to avoid AVLN2000/AVLN2100 when x:DataType not specified.
- Removed unsupported properties in XAML where necessary and wrapped button area with Border for padding.
- In control code-behind, used FindControl lookups and null-checks to avoid missing InitializeComponent-generated fields.

### Usage Guidance
- Initialize error handling during app startup:
  - ErrorHandlingInitializer.Initialize();
  - Or ErrorHandlingInitializer.InitializeForDevelopment();
- Log exceptions via Service_ErrorHandler.HandleException(ex, severity, source, contextData);
- Display user message UI when appropriate:
  - Inline: new Control_ErrorMessage().Initialize(ex, severity, source, contextData, retryAction);
  - Modal: await ErrorDialog_Enhanced.ShowErrorAsync(owner, ex, severity, source, contextData, retryAction);

---