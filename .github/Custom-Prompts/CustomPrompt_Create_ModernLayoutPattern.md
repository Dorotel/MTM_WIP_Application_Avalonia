# Custom Prompt: Create Modern Layout Pattern

## ?? **Instructions**
Use this prompt when you need to generate modern Avalonia layouts with sidebars, cards, hero sections, and responsive design using MTM design patterns. This prompt creates complete layout structures with proper spacing, shadows, and purple brand theming.

## ?? **Persona**
**Layout Designer Copilot** - Specializes in creating modern, responsive Avalonia layouts with MTM design system integration, proper spacing, and contemporary UI patterns.

## ?? **Prompt Template**

```
Act as Layout Designer Copilot. Create a modern Avalonia layout pattern for [LAYOUT_NAME] with the following requirements:

**Layout Type:** [Sidebar + Content | Card Grid | Hero + Content | Dashboard | Form Layout]
**Purpose:** [Describe the layout's intended use and content]
**Key Sections:** [List main areas - header, sidebar, content, footer, etc.]
**Responsive Behavior:** [How layout adapts to different screen sizes]
**Content Areas:** [Describe what goes in each section]

**Design Requirements:**
- Apply MTM purple brand theme (#4B45ED primary, #BA45ED accents, #8345ED secondary)
- Use modern card-based design with subtle shadows
- Implement proper spacing (8px containers, 4px controls, 24px card padding)
- Create responsive design that works on mobile and desktop
- Include hero/banner sections with gradient backgrounds
- Apply modern UI elements (rounded corners, shadows, gradients)

**Layout Specifications:**
- Use Grid layouts for performance and proper alignment
- Implement sidebar with fixed width (240-280px) and clear hierarchy
- Create card containers with BoxShadow="0 2 8 0 #11000000"
- Apply proper typography scaling (20-28px headers, 14-16px content)
- Use DynamicResource for all colors to support theming
- Include navigation patterns and content organization

**Technical Requirements:**
- Generate clean AXAML with proper indentation and structure
- Use compiled bindings preparation (x:CompileBindings ready)
- Apply Avalonia-specific controls and patterns
- Ensure proper performance with Grid over StackPanel preference
- Include responsive breakpoints and mobile considerations

**Additional Context:** [Any specific layout requirements or constraints]
```

## ?? **Purpose**
This prompt generates complete modern layout patterns that serve as foundations for Avalonia applications, incorporating MTM design system, responsive design principles, and contemporary UI patterns.

## ?? **Usage Examples**

### **Example 1: Main Application Layout with Sidebar**
```
Act as Layout Designer Copilot. Create a modern Avalonia layout pattern for MainApplicationLayout with the following requirements:

**Layout Type:** Sidebar + Content
**Purpose:** Primary application window layout with navigation sidebar and dynamic content area
**Key Sections:** App header, navigation sidebar, main content area, status bar
**Responsive Behavior:** Sidebar collapses to icons on small screens, content area adjusts
**Content Areas:** Logo/branding in header, navigation menu in sidebar, dynamic views in content

**Design Requirements:**
- Apply MTM purple brand theme (#4B45ED primary, #BA45ED accents, #8345ED secondary)
- Use modern card-based design with subtle shadows
- Implement proper spacing (8px containers, 4px controls, 24px card padding)
- Create responsive design that works on mobile and desktop
- Include hero/banner sections with gradient backgrounds
- Apply modern UI elements (rounded corners, shadows, gradients)

**Layout Specifications:**
- Use Grid layouts for performance and proper alignment
- Implement sidebar with fixed width (240-280px) and clear hierarchy
- Create card containers with BoxShadow="0 2 8 0 #11000000"
- Apply proper typography scaling (20-28px headers, 14-16px content)
- Use DynamicResource for all colors to support theming
- Include navigation patterns and content organization

**Technical Requirements:**
- Generate clean AXAML with proper indentation and structure
- Use compiled bindings preparation (x:CompileBindings ready)
- Apply Avalonia-specific controls and patterns
- Ensure proper performance with Grid over StackPanel preference
- Include responsive breakpoints and mobile considerations

**Additional Context:** Layout should support view switching through navigation and maintain state during transitions
```

### **Example 2: Dashboard Card Grid Layout**
```
Act as Layout Designer Copilot. Create a modern Avalonia layout pattern for DashboardLayout with the following requirements:

**Layout Type:** Card Grid
**Purpose:** Dashboard view with multiple information cards and quick actions
**Key Sections:** Hero banner, card grid area, action buttons section
**Responsive Behavior:** Cards reflow from 3 columns to 2 to 1 based on screen size
**Content Areas:** Summary banner, metric cards, chart widgets, action shortcuts

**Design Requirements:**
- Apply MTM purple brand theme (#4B45ED primary, #BA45ED accents, #8345ED secondary)
- Use modern card-based design with subtle shadows
- Implement proper spacing (8px containers, 4px controls, 24px card padding)
- Create responsive design that works on mobile and desktop
- Include hero/banner sections with gradient backgrounds
- Apply modern UI elements (rounded corners, shadows, gradients)

**Layout Specifications:**
- Use Grid layouts for performance and proper alignment
- Implement sidebar with fixed width (240-280px) and clear hierarchy
- Create card containers with BoxShadow="0 2 8 0 #11000000"
- Apply proper typography scaling (20-28px headers, 14-16px content)
- Use DynamicResource for all colors to support theming
- Include navigation patterns and content organization

**Technical Requirements:**
- Generate clean AXAML with proper indentation and structure
- Use compiled bindings preparation (x:CompileBindings ready)
- Apply Avalonia-specific controls and patterns
- Ensure proper performance with Grid over StackPanel preference
- Include responsive breakpoints and mobile considerations

**Additional Context:** Cards should support hover effects and click actions for detailed views
```

## ?? **Guidelines**

### **MTM Design System Integration**
```xml
<!-- MTM Color Variables -->
<Window.Resources>
    <SolidColorBrush x:Key="PrimaryBrush" Color="#4B45ED"/>
    <SolidColorBrush x:Key="AccentBrush" Color="#BA45ED"/>
    <SolidColorBrush x:Key="SecondaryBrush" Color="#8345ED"/>
    <SolidColorBrush x:Key="BlueAccentBrush" Color="#4574ED"/>
    <SolidColorBrush x:Key="PinkAccentBrush" Color="#ED45E7"/>
    <SolidColorBrush x:Key="LightPurpleBrush" Color="#B594ED"/>
</Window.Resources>

<!-- Hero Gradient Pattern -->
<LinearGradientBrush x:Key="HeroGradientBrush" StartPoint="0,0" EndPoint="1,1">
    <GradientStop Color="#4574ED" Offset="0"/>
    <GradientStop Color="#4B45ED" Offset="0.3"/>
    <GradientStop Color="#8345ED" Offset="0.7"/>
    <GradientStop Color="#BA45ED" Offset="1"/>
</LinearGradientBrush>
```

### **Layout Patterns**
- **Main Window**: Grid with sidebar and content areas
- **Cards**: Border with CornerRadius="8" and BoxShadow
- **Hero Sections**: Gradient background with centered content
- **Navigation**: Expandable groups with clear hierarchy
- **Content Areas**: Proper padding and margin application

### **Responsive Design Principles**
- Use Grid for flexible layouts that adapt to size changes
- Implement proper margin and padding that scale appropriately
- Design for both mouse and touch interaction
- Consider keyboard navigation and accessibility
- Plan for different screen densities and scaling factors

### **Performance Considerations**
- Prefer Grid over StackPanel for better virtualization
- Use proper control hierarchy to minimize layout passes
- Apply efficient data binding patterns
- Minimize complex visual effects that impact performance

## ?? **Related Files**
- [../UI-Instructions/ui-generation.instruction.md](../UI-Instructions/ui-generation.instruction.md) - Complete UI generation guidelines
- [../Core-Instructions/naming.conventions.instruction.md](../Core-Instructions/naming.conventions.instruction.md) - Layout and control naming standards
- [../Automation-Instructions/personas.instruction.md](../Automation-Instructions/personas.instruction.md) - Layout Designer Copilot persona details

## ? **Quality Checklist**

### **Design System Compliance**
- [ ] MTM purple color scheme applied consistently
- [ ] Proper spacing and margins (8px containers, 24px card padding)
- [ ] Modern UI elements (cards, shadows, gradients) implemented
- [ ] Typography hierarchy appropriate (20-28px headers)

### **Layout Structure**
- [ ] Clean Grid-based layout with proper rows/columns
- [ ] Responsive design principles applied
- [ ] Proper control hierarchy and nesting
- [ ] Performance-optimized structure (Grid over StackPanel)

### **Technical Implementation**
- [ ] Clean AXAML with proper indentation
- [ ] DynamicResource used for all colors
- [ ] Compiled bindings preparation included
- [ ] Avalonia-specific patterns followed

### **Accessibility and Usability**
- [ ] Keyboard navigation support considered
- [ ] Touch-friendly sizing for interactive elements
- [ ] Clear visual hierarchy and information organization
- [ ] Consistent interaction patterns throughout layout