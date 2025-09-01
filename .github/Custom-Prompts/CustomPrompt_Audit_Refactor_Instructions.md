# Instruction File Audit, Refactor, and Enhancement Prompt

## Objective
Systematically audit, refactor, reorganize, improve, enhance, optimize, and streamline all markdown instruction files in the `.github` folder (and its subfolders) that contain the word `instruction` in their filename. The goal is to maximize clarity, conciseness, and effectiveness, and to ensure the content is easily navigable and understandable by GitHub Copilot.

## Scope
- **Files:** All `.md` files in `.github` and its subfolders with `instruction` in the filename.
- **Allowed Actions:**
  - Edit, reorganize, and refactor content for clarity and conciseness
  - Merge, split, or rename files if it improves organization or usability
  - Delete files if their content has been fully migrated elsewhere (with changelog entry)
  - Add navigation aids (e.g., tables of contents, cross-links) as needed
  - Apply any formatting or conventions that help Copilot understand and follow the instructions
  - **Audit all sample code in instruction files:**
    - For every code sample, explicitly review and either:
      - **Keep** (if already correct and current)
      - **Update** (if improvements or corrections are needed)
      - **Remove** (if completely irrelevant or obsolete, and only if the sample cannot be recreated elsewhere—e.g., if a service was removed but recreated under a different name, ensure the new sample is present)
- **Documentation:**
  - For every change, append an entry to a central `InstructionChangelog.md` file in `.github/` with:
    - A brief description of the change
    - The date of the change
    - The reason for the change
- **Backup:**
  - Before making any changes, create a backup of all instruction files in `.github` as `InstructionBackup-{Date}.zip`

## Copilot Personality and Personas
- Copilot should act as a helpful, detail-oriented, and proactive documentation specialist.
- Copilot may adopt multiple personas as needed to ensure all aspects of the instructions are covered and optimized for both human and AI understanding. Example personas:
  - **Quality Assurance Auditor**: Reviews for compliance, clarity, and correctness.
  - **UI Architect**: Ensures navigation aids and structure are optimal.
  - **Code Style Advisor**: Checks for formatting, naming, and consistency.
  - **Accessibility Specialist**: Ensures accessibility and inclusive formatting.
  - **Documentation Specialist**: Focuses on clarity, conciseness, and human readability.
  - Use each persona as appropriate for the file or section being improved.

## Deliverables
- All relevant instruction files refactored and reorganized for maximum clarity and Copilot usability
- A single, cumulative `InstructionChangelog.md` in `.github/` documenting all changes as specified
- A backup zip file of all instruction files before changes, named `InstructionBackup-{Date}.zip` in `.github/`

## Success Criteria
- All instruction files are clear, concise, and well-organized (e.g., Flesch Reading Ease > 60, no redundant sections, clear headings)
- Navigation and cross-referencing between files is easy (e.g., file-level TOCs, cross-file links, collapsible sections)
- Copilot can easily parse and follow all instructions (test with Copilot or peer review)
- All changes are fully documented in the changelog (see example below)
- A backup zip exists for all original instruction files
- Accessibility and formatting standards are met (e.g., proper heading structure, alt text for images, consistent markdown)
## Navigation Aids and Formatting
- Use collapsible sections (`<details>/<summary>`) for major topics
- Add file-level tables of contents (TOC) where appropriate
- Cross-link related files and sections
- Use consistent heading levels and markdown formatting
- Add alt text for any images
## Changelog Entry Example
```
### [2025-09-01] Refactored dependency-injection.instruction.md
- Description: Merged duplicate sections, clarified DI patterns, added TOC
- Reason: Improve clarity and reduce redundancy
```
## Review and Approval
- All major changes should be peer-reviewed or approved before finalizing
## Rollback Instructions
- To restore original files, extract `InstructionBackup-{Date}.zip` in `.github/` and overwrite the modified files.

## Constraints
- Do not remove essential technical content
- Do not introduce ambiguity or conflicting guidance
- All documentation must be human-readable

## Steps
1. Identify all relevant instruction files in `.github` and subfolders
2. Create a backup zip of all instruction files as `InstructionBackup-{Date}.zip` in `.github/`
3. Audit each file for clarity, conciseness, and Copilot usability
4. Refactor, reorganize, merge, split, rename, or delete as needed (deletion only if content is migrated)
5. Add navigation aids and cross-references where helpful
6. Document every change in `InstructionChangelog.md` with date, description, and reason
7. **Actively use and encourage the Joyride Extension** to improve, automate, or streamline the process. Examples of where and how to use Joyride:
  - **Navigation:** Use Joyride to quickly jump between related instruction files or sections.
  - **Automation:** Use Joyride scripts to batch rename, reorganize, or refactor files and headings.
  - **Advanced Editing:** Use Joyride to apply consistent formatting, insert navigation aids, or validate cross-links across multiple files.
  - **Quality Checks:** Use Joyride to run accessibility or formatting audits on markdown files.
  - **Example Command:** `(joyride-evaluate-code "(require '[joyride.fs :as fs]) (fs/list-files \".github\")")` to list all instruction files for batch processing.
  - **Encouragement:** Whenever a repetitive or complex documentation task arises, consider if Joyride can automate or simplify it, and prefer using it when possible.
8. Ensure all files, the changelog, and the backup meet the success criteria above
