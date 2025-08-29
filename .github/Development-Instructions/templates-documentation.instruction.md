# HTML Templates and Documentation Migration

## **FileDefinitions HTML Migration Templates**

### **NEW: Specialized Templates for FileDefinitions Migration**
To speed up the FileDefinitions migration process, use these specialized templates:

#### **PlainEnglish Template**
Location: `Documentation/HTML/Templates/FileDefinitions-PlainEnglish-Template.html`

**Key Features:**
- Black/Gold MTM branding for business users
- Manufacturing analogies and real-world comparisons
- Process-oriented explanations without technical jargon
- Cross-reference navigation with keyboard shortcuts (Alt+T for Technical view)
- Business impact and operational benefits focus
- Troubleshooting from user perspective
- All 4 CSS files integrated (modern-styles.css, mtm-theme.css, plain-english.css, responsive.css)

**Usage Pattern:**
```html
<!-- Replace {{PLACEHOLDERS}} with actual content -->
<title>FileDefinitions-{{FILENAME}} - {{COMPONENT_TYPE}} Explained</title>
<!-- Component badge, analogy sections, business impact, etc. -->
```

#### **Technical Template**
Location: `Documentation/HTML/Templates/FileDefinitions-Technical-Template.html`

**Key Features:**
- Purple MTM branding for developers
- Complete API documentation with code examples
- MTM business rule implementation details
- Database integration patterns (stored procedures only)
- Dependency and architecture documentation
- Performance and testing considerations
- Cross-reference navigation with keyboard shortcuts (Alt+P for Plain English view)
- Code syntax highlighting and copy-to-clipboard functionality

**Usage Pattern:**
```html
<!-- Replace {{PLACEHOLDERS}} with technical details -->
<title>FileDefinitions-{{FILENAME}} - MTM WIP Application Technical Documentation</title>
<!-- API docs, code examples, architecture, testing, etc. -->
```

#### **Template Integration with FileDefinitions Migration**
Both templates are designed for the comprehensive FileDefinitions migration with:

1. **Dual Version Support**: Easy switching between PlainEnglish and Technical views
2. **MTM Business Logic**: Built-in sections for MTM-specific conventions
3. **Cross-Reference Navigation**: TOP_OF_PAGE navigation buttons as specified
4. **Mobile Responsiveness**: All 4 CSS files integrated for complete responsiveness
5. **FileDefinitions Naming**: Uses FileDefinitions- prefix as required
6. **Accessibility**: WCAG AA compliant with keyboard navigation support

#### **Template Replacement Strategy**
1. **Copy Template**: Use appropriate template based on audience
2. **Replace Placeholders**: All {{PLACEHOLDER}} values with actual content
3. **Customize Sections**: Add/remove sections based on component type
4. **Update Navigation**: Ensure cross-reference links point to correct locations
5. **Validate Content**: Check that all links work and content is appropriate for audience

## **UI Generation from Markdown Instructions**

### When asked to "Create UI Element from {filename}.md":

1. **Parse the markdown file** from `Development/UI_Documentation/` to extract:
   - UI Element Name
   - Component Structure
   - Props/Inputs
   - Visual Representation
   - Related controls

2. **Generate the following files**:
   - `Views/{Name}View.axaml` - The Avalonia UI markup
   - `ViewModels/{Name}ViewModel.cs` - The ViewModel using standard .NET MVVM
   - Only UI structure and bindings, NO business logic implementation

3. **Follow established patterns** from UI Instructions

### **Example Conversion Process**
If MD file shows:
```
??? MainForm_TabControl (TabControl)
?   ??? Inventory Tab ? Control_InventoryTab
```

Generate appropriate Avalonia AXAML and ViewModel files following the patterns in UI-Instructions.

## **Documentation Standards**

### **Markdown Documentation**
- **Clear structure**: Use consistent heading hierarchy
- **Code examples**: Include relevant code snippets with syntax highlighting
- **Cross-references**: Link to related instruction files and documentation
- **Update frequency**: Keep documentation current with code changes

### **HTML Documentation**
- **Template consistency**: Use established templates for all HTML documentation
- **Responsive design**: Ensure all documentation works on mobile devices
- **Accessibility**: Follow WCAG AA guidelines for all content
- **Navigation**: Provide clear navigation between related documents

### **API Documentation**
- **Complete coverage**: Document all public interfaces and methods
- **Usage examples**: Include practical examples for each API
- **Parameter documentation**: Clear descriptions of all parameters and return values
- **Error handling**: Document expected exceptions and error conditions

## **Migration Workflows**

### **Legacy Documentation Migration**
1. **Assessment**: Identify all existing documentation requiring migration
2. **Prioritization**: Focus on frequently accessed and critical documentation first
3. **Template application**: Use appropriate template based on target audience
4. **Content adaptation**: Adapt content to new format while preserving essential information
5. **Cross-reference updates**: Update all links and references to new locations
6. **Validation**: Test all links and ensure content accuracy

### **Ongoing Documentation Maintenance**
- **Regular reviews**: Schedule periodic reviews of all documentation
- **Version tracking**: Maintain version history for significant documentation changes
- **Feedback integration**: Incorporate user feedback to improve documentation quality
- **Consistency checks**: Ensure all documentation follows established standards