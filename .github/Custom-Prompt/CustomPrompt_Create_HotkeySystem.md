# Custom Prompt: Create Hotkey System

## ??? **Instructions**
Use this prompt when you need to create a hotkey or shortcut system for quickly accessing custom prompts in GitHub Copilot chatbox. This prompt creates trigger patterns and reference systems for rapid prompt access.

## ?? **Persona**
**Automation Enhancement Copilot** - Specializes in creating automation shortcuts, hotkey systems, and workflow optimization tools for development productivity.

## ?? **Prompt Template**

```
Act as Automation Enhancement Copilot. Create a hotkey/shortcut system for MTM custom prompts with the following requirements:

**System Type:** [Trigger patterns | Reference shortcuts | Quick access system]
**Access Method:** [Chatbox shortcuts | File references | Command patterns]
**Prompt Categories:** [UI Generation | Business Logic | Quality Assurance | Error Handling | Documentation]
**User Experience:** [Memorizable shortcuts | Consistent patterns | Quick reference guide]

**Hotkey System Requirements:**
- Create memorable trigger patterns for all 25+ custom prompts
- Establish consistent naming conventions for shortcuts
- Generate quick reference documentation
- Support category-based grouping (@ui:, @biz:, @qa:, etc.)
- Include fallback methods for accessing full prompts
- Create integration with existing HTML documentation

**Technical Implementation:**
- Generate shortcut mapping documentation
- Create reference cards for common prompts
- Implement bookmark-friendly HTML interface
- Include search functionality for prompt discovery
- Support keyboard navigation through prompt categories
- Prepare auto-completion suggestions

**MTM-Specific Features:**
- Align with MTM workflow patterns
- Support ReactiveUI and Avalonia-specific shortcuts
- Include business logic and database pattern shortcuts
- Apply MTM branding to reference materials
- Support quality assurance workflow shortcuts

**Additional Context:** [Specific workflow needs or shortcut preferences]
```

## ?? **Purpose**
This prompt generates a comprehensive hotkey/shortcut system for accessing MTM custom prompts quickly, improving development productivity and workflow efficiency.

## ?? **Usage Examples**

### **Example 1: Complete Hotkey System**
```
Act as Automation Enhancement Copilot. Create a hotkey/shortcut system for MTM custom prompts with the following requirements:

**System Type:** Trigger patterns with quick reference
**Access Method:** Chatbox shortcuts with @-prefix system
**Prompt Categories:** UI Generation, Business Logic, Quality Assurance, Error Handling, Documentation
**User Experience:** Memorizable shortcuts with category grouping

**Hotkey System Requirements:**
- Create memorable trigger patterns for all 25+ custom prompts
- Establish consistent naming conventions for shortcuts
- Generate quick reference documentation
- Support category-based grouping (@ui:, @biz:, @qa:, etc.)
- Include fallback methods for accessing full prompts
- Create integration with existing HTML documentation

**Additional Context:** Focus on workflow efficiency for Avalonia development and MTM business patterns
```

## ?? **Guidelines**
- Follow MTM automation instruction patterns
- Maintain consistency with existing custom prompt system
- Create intuitive and memorable shortcut patterns
- Ensure shortcuts align with development workflow
- Support both experienced and new developers
- Include comprehensive documentation

## ?? **Related Files**
- `customprompts.instruction.md` - Master index of all custom prompts
- `personas.instruction.md` - Persona definitions and behaviors
- `Documentation/HTML/Technical/custom-prompts.html` - HTML prompt interface
- `Development/Custom_Prompts/` - All custom prompt implementation files

## ? **Quality Checklist**
- [ ] Shortcuts follow consistent naming patterns
- [ ] All 25+ custom prompts have assigned shortcuts
- [ ] Quick reference documentation created
- [ ] Integration with HTML documentation maintained
- [ ] Shortcuts are intuitive and memorable
- [ ] Fallback access methods provided
- [ ] MTM workflow alignment verified