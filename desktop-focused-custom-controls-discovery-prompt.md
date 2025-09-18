# Desktop-Focused MTM Custom Controls Discovery Prompt

**GitHub Copilot Agent Prompt for Refined Custom Controls Analysis**

---

## 🎯 Paste this exact prompt to GitHub Copilot

```text
I need you to refine and expand the MTM Custom Controls Discovery analysis with a focused approach for desktop/laptop Windows environments. Our manufacturing users exclusively use desktop workstations with keyboard/mouse or laptops with trackpads - NO tablet/touch optimization is needed.

REQUIREMENTS:

## Phase 1: Review and Refine Existing Discoveries

1. **Audit Current Custom Controls List**: 
   - Review the existing Top 10 Custom Controls from docs/recommendations/top-10-custom-controls.md
   - REMOVE any controls designed for touch/tablet (TouchOptimizedButton, touch-specific features)
   - REMOVE any Android-specific optimizations or references
   - Focus on desktop productivity and manufacturing efficiency

2. **Redesign Remaining Controls for Desktop Excellence**:
   - Optimize for keyboard shortcuts and mouse interactions
   - Implement proper focus management and tab navigation
   - Add desktop-specific features like context menus, drag-drop, keyboard accelerators
   - Ensure all controls work perfectly with Windows desktop themes and DPI scaling
   - Focus on manufacturing workstation efficiency (fast data entry, minimal mouse movement)

## Phase 2: Enhanced Desktop Custom Controls Discovery

Analyze the entire MTM WIP Application codebase (40+ Views, existing custom controls, ViewModels) and identify NEW desktop-optimized custom controls that would provide:

**Performance Benefits**:
- Faster rendering on desktop hardware
- Efficient memory usage for Windows workstations
- Optimized for desktop GPU acceleration
- Support for 10,000+ manufacturing records with smooth scrolling

**User Experience for Desktop Manufacturing**:
- Keyboard-first workflows with comprehensive shortcuts
- Mouse-optimized interactions (right-click menus, hover states)
- Desktop-native feel with Windows 11 design principles
- Multi-monitor support considerations
- Fast data entry patterns for manufacturing operators

**Quality Improvements**:
- Desktop-specific validation patterns
- Windows-native error handling and notifications
- Proper desktop accessibility (screen readers, high contrast)
- Integration with Windows clipboard and system features

## Phase 3: Identify Desktop-Specific Patterns

Look for these desktop manufacturing patterns across all 40+ Views:

1. **Keyboard-Heavy Data Entry**: Manufacturing operators prefer keyboard-centric workflows
2. **Right-Click Context Menus**: Desktop users expect context-sensitive options
3. **Drag and Drop Operations**: Natural for desktop inventory management
4. **Multi-Selection with Ctrl/Shift**: Standard desktop interaction patterns
5. **Window Management**: Proper support for resizing, minimizing, multi-monitor
6. **Copy/Paste Integration**: Seamless Windows clipboard integration
7. **Print Integration**: Manufacturing often requires desktop printing workflows

## Technical Requirements:

**Architecture Integration**:
- Avalonia UI 11.3.4 desktop-optimized patterns
- .NET 8 with Windows-specific optimizations where beneficial
- MVVM Community Toolkit with desktop event handling
- MTM theme system with Windows 11 design language
- Stored procedure integration for manufacturing workflows

**Desktop Performance Patterns**:
- Virtualization for large datasets on desktop hardware
- Efficient window rendering and updates
- Desktop-appropriate caching strategies
- Windows-native animations and transitions

**Manufacturing Workflow Optimization**:
- Fast part ID entry with keyboard shortcuts
- Efficient operation number selection (90, 100, 110, 120)
- Quick location navigation and selection
- Rapid quantity entry and validation
- Desktop-optimized error feedback and correction

## Expected Deliverables:

1. **Refined Top 10 Desktop Custom Controls**: Updated priority list removing touch/tablet controls and adding desktop-optimized alternatives

2. **New Desktop-Specific Custom Controls**: Discover 20 additional controls specifically designed for desktop manufacturing workstations

3. **Desktop UX Pattern Analysis**: Identify repetitive desktop UI patterns that could benefit from custom controls

4. **Keyboard Shortcut Integration Plan**: Comprehensive keyboard navigation strategy across all custom controls

5. **Desktop Performance Optimization Report**: Specific optimizations for Windows desktop hardware

6. **Windows Integration Opportunities**: Native Windows features that could enhance manufacturing workflows

## Analysis Scope:
- All 40+ AXAML Views with desktop interaction patterns
- Existing 3 custom controls optimized for desktop
- Manufacturing workflows optimized for keyboard/mouse
- Windows desktop integration opportunities
- Multi-monitor manufacturing workstation support
- Desktop printing and export workflows

Focus on creating the most efficient, keyboard-friendly, mouse-optimized manufacturing interface possible for Windows desktop environments while maintaining the performance, user experience, and quality goals from the original implementation plan.

Generate updated documentation that reflects this desktop-first approach and provides concrete implementation guidance for Windows desktop manufacturing workstations.
```

---

## 📋 How to Use This Prompt

1. **Copy the entire prompt** from the code block above
2. **Paste it into GitHub Copilot Chat** on this PR
3. **Wait for the analysis** - Copilot will review all existing documentation and generate:
   - Refined Top 10 list focused on desktop
   - New desktop-specific custom controls
   - Updated implementation priorities
   - Desktop UX optimization recommendations

## 🎯 Expected Outcomes

This prompt will deliver:

- **Removal of touch/tablet optimizations** from all custom control recommendations
- **Enhanced desktop keyboard shortcuts** and mouse interactions
- **New desktop-specific custom controls** not previously identified
- **Manufacturing workstation optimizations** for keyboard-heavy workflows
- **Windows 11 design language** integration improvements
- **Multi-monitor support** considerations for manufacturing environments

The refined analysis will maintain all the performance, user experience, and quality benefits from your original implementation plan while focusing exclusively on the desktop manufacturing environment your users actually work in.
