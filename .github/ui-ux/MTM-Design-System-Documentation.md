# MTM Design System Documentation

## 📋 Overview

This document defines the comprehensive design system for the MTM WIP Application, establishing visual consistency, user experience patterns, and design principles that support efficient manufacturing workflows while maintaining modern, accessible, and professional interfaces.

## 🎯 **Design Philosophy**

### **Core Principles**

#### **1. Manufacturing-First Design**

```yaml
Principle: Design for the factory floor environment
Application:
  - High contrast colors for industrial lighting conditions
  - Large touch targets for gloved hands operation
  - Clear visual hierarchy for quick decision making
  - Minimal cognitive load during repetitive tasks

Examples:
  - Part IDs displayed in monospace fonts with high contrast
  - Operation buttons sized 44px minimum for touch accessibility
  - Status indicators using universally understood color conventions
  - Critical information positioned in primary visual areas
```

#### **2. Consistency and Predictability**

```yaml
Principle: Users should never have to learn the interface twice
Application:
  - Standardized interaction patterns across all views
  - Consistent component behavior and appearance
  - Predictable navigation and information architecture
  - Uniform response to user actions

Examples:
  - Save buttons always positioned bottom-right in forms
  - Status badges follow consistent color coding throughout
  - Navigation breadcrumbs maintain same position and style
  - Error messages appear in standardized modal patterns
```

#### **3. Efficiency and Speed**

```yaml
Principle: Minimize time between intent and completion
Application:
  - Keyboard shortcuts for power users
  - Quick action buttons for common operations
  - Auto-complete and smart defaults
  - Bulk operations support

Examples:
  - Tab navigation through all form controls
  - Enter key submits forms, Escape cancels operations
  - Recent part IDs suggested during data entry
  - Multi-select for batch inventory operations
```

#### **4. Error Prevention and Recovery**

```yaml
Principle: Prevent errors when possible, make recovery easy when not
Application:
  - Validation during data entry, not just on submit
  - Confirmation dialogs for destructive actions
  - Undo capabilities for critical operations
  - Clear error messages with suggested solutions

Examples:
  - Part ID format validation as user types
  - "Are you sure?" dialogs for inventory deletions
  - Transaction rollback capabilities
  - Specific error messages: "Part ID not found" vs "Error occurred"
```

## 🎨 **Visual Language**

### **Color Psychology in Manufacturing Context**

#### **Semantic Color Mapping**

```yaml
Status Colors:
  Green (#28A745): 
    - Available inventory
    - Completed operations
    - System health OK
    - Successful transactions
    
  Yellow/Amber (#FFC107):
    - Low stock warnings
    - Pending operations
    - Attention required
    - Caution states
    
  Red (#DC3545):
    - Out of stock
    - Failed operations
    - Critical errors
    - Stop/danger states
    
  Blue (#0078D4):
    - Active operations
    - Selected items
    - Primary actions
    - Information states
    
  Gray (#6C757D):
    - Inactive/disabled states
    - Secondary information
    - Background elements
    - Neutral states
```

#### **Color Accessibility Matrix**

```yaml
Contrast Ratios (WCAG 2.1 AA Compliance):
  Primary Text on Background: 4.5:1 minimum
  Large Text on Background: 3:1 minimum  
  UI Components: 3:1 minimum
  Graphical Objects: 3:1 minimum

High Contrast Support:
  - All colors work in Windows High Contrast mode
  - Icons maintain visibility with outline variants
  - Text remains readable with system color overrides
  - Interactive elements clearly distinguishable

Color Blindness Considerations:
  - Red/Green combinations avoided for critical information
  - Patterns and icons supplement color-only information
  - Status indicated through multiple visual cues
  - Alternative text provided for color-dependent content
```

### **Typography Hierarchy**

#### **Information Hierarchy Mapping**

```yaml
Page Titles (H1 - 32px Bold):
  Usage: Primary page identification
  Example: "Inventory Management", "Transaction History"
  Context: Top of main content areas
  
Section Headers (H2 - 28px SemiBold):
  Usage: Major content sections within pages
  Example: "Current Inventory", "Recent Transactions"
  Context: Content area divisions
  
Subsection Headers (H3 - 24px SemiBold):
  Usage: Content groupings within sections
  Example: "Parts by Location", "Today's Activity"
  Context: Card headers, form sections
  
Form Labels (H4 - 20px Medium):
  Usage: Important form sections and groups
  Example: "Part Information", "Transaction Details"
  Context: Form organization
  
Field Labels (H5 - 16px Medium):
  Usage: Individual form field labels
  Example: "Part ID", "Quantity", "Location"
  Context: Form fields, data labels
  
Body Text (14px Regular):
  Usage: General content, descriptions, help text
  Example: Instructional text, descriptions
  Context: Content areas, form help text
  
Small Text (12px Regular):
  Usage: Secondary information, timestamps, captions
  Example: "Last updated: 2025-01-15 14:30", metadata
  Context: Status bars, data tables, footnotes
```

#### **Readability Optimization**

```yaml
Line Height Guidelines:
  Headings: 1.2x font size for tight spacing
  Body Text: 1.5x font size for comfortable reading
  Form Fields: 1.3x font size for compact layouts
  
Character Width:
  Optimal: 45-75 characters per line
  Maximum: 90 characters per line
  Forms: 25-35 characters for input fields
  
Font Weight Usage:
  Light (300): Never used (insufficient contrast)
  Regular (400): Body text, secondary information
  Medium (500): Form labels, navigation items
  SemiBold (600): Section headers, important labels
  Bold (700): Page titles, emphasis, alerts
```

### **Spacing and Layout System**

#### **8-Point Grid System**

```yaml
Base Unit: 8px (optimized for both desktop and touch interfaces)

Micro Spacing (2-4px):
  - Letter spacing adjustments
  - Fine border adjustments
  - Icon alignment tweaks
  
Small Spacing (8px):
  - Element margins within components
  - Padding in compact UI elements
  - Grid gaps in dense layouts
  
Medium Spacing (16px):
  - Standard component padding
  - Form field spacing
  - Card content margins
  
Large Spacing (24px):
  - Section separations
  - Page content margins
  - Modal dialog spacing
  
Extra Large Spacing (32px+):
  - Major page sections
  - Full-page layout margins
  - Significant content separations
```

#### **Component Spacing Rules**

```yaml
Buttons:
  Internal Padding: 12px horizontal, 8px vertical
  Minimum Touch Target: 44px x 44px
  Button Groups: 8px spacing between buttons
  
Form Controls:
  Label to Input: 4px vertical spacing
  Between Fields: 16px vertical spacing
  Form Sections: 24px separation
  
Cards:
  Internal Padding: 16px all sides
  Card Spacing: 16px between cards
  Header Padding: 16px horizontal, 8px vertical
  
Data Tables:
  Cell Padding: 8px horizontal, 6px vertical
  Row Height: 32px minimum
  Header Height: 36px minimum
```

## 🧱 **Component Design Specifications**

### **Input Components**

#### **Text Input Fields**

```yaml
Standard TextBox:
  Height: 32px minimum
  Padding: 8px horizontal, 6px vertical
  Border: 1px solid #DEE2E6
  Border Radius: 4px
  Focus State: 2px solid #0078D4 border
  Error State: 2px solid #DC3545 border
  Disabled State: #F8F9FA background, #6C757D text
  
Large TextBox (for manufacturing floor):
  Height: 44px minimum
  Padding: 12px horizontal, 10px vertical
  Font Size: 16px
  Touch-optimized for gloved operation
  
Validation States:
  Success: Green left border accent
  Warning: Yellow left border accent
  Error: Red border with error icon
  Info: Blue left border accent
```

#### **Selection Controls**

```yaml
ComboBox/Dropdown:
  Closed Height: 32px
  Padding: 8px
  Dropdown Max Height: 200px (scrollable)
  Item Height: 28px
  Selected Item: Primary color background
  
Radio Buttons:
  Button Size: 16px
  Touch Target: 44px x 44px
  Label Spacing: 8px from button
  Group Spacing: 12px between options
  
Checkboxes:
  Box Size: 16px
  Touch Target: 44px x 44px
  Check Mark: White on primary background
  Mixed State: Dash indicator for partial selection
```

### **Navigation Components**

#### **Primary Navigation**

```yaml
Main Navigation Bar:
  Height: 56px
  Background: Primary color (#0078D4)
  Text Color: White
  Hover State: 10% darker background
  Active State: 20% darker background
  
Breadcrumb Navigation:
  Height: 32px
  Separator: "/" or ">" character
  Link Color: Primary color
  Current Page: Regular text color (non-linked)
  
Tab Navigation:
  Tab Height: 44px
  Active Tab: Bottom border accent (3px)
  Inactive Tabs: Transparent background
  Hover State: Light background tint
```

#### **Secondary Navigation**

```yaml
Sidebar Navigation:
  Width: 240px (collapsed: 60px)
  Item Height: 44px
  Icon Size: 20px
  Text Size: 14px
  Hover State: Light background
  Active State: Primary color accent
  
Context Menus:
  Item Height: 32px
  Padding: 8px 12px
  Separator: 1px border line
  Icons: 16px with 8px spacing
  Keyboard Focus: Primary background
```

### **Feedback Components**

#### **Status and Progress Indicators**

```yaml
Status Badges:
  Height: 20px
  Padding: 4px 8px
  Border Radius: 10px (pill shape)
  Font Size: 10px
  Font Weight: SemiBold
  Text: All caps
  
Progress Bars:
  Height: 8px (small), 16px (standard)
  Border Radius: 4px
  Background: Light gray
  Fill: Primary color
  Animation: Smooth progress updates
  
Loading Spinners:
  Small: 16px diameter
  Standard: 24px diameter
  Large: 32px diameter
  Color: Primary or current text color
  Animation: 1s rotation duration
```

#### **Alert and Notification Systems**

```yaml
Toast Notifications:
  Width: 320px
  Height: Auto (min 60px)
  Position: Top-right corner
  Duration: 4s (info), 6s (warning), persistent (error)
  Animation: Slide in from right
  
Modal Dialogs:
  Max Width: 480px
  Padding: 24px
  Border Radius: 8px
  Backdrop: Semi-transparent black (50%)
  Animation: Fade in with scale
  
Inline Alerts:
  Padding: 12px 16px
  Border Radius: 4px
  Icon: 16px on left side
  Close Button: 16px on right side
  Colors: Semantic (info, success, warning, error)
```

## 🏭 **Manufacturing-Specific Design Patterns**

### **Inventory Display Patterns**

#### **Part Information Cards**

```yaml
Part Card Layout:
  Width: 280px minimum
  Height: 160px
  Border Radius: 8px
  Shadow: Subtle drop shadow
  
Content Structure:
  - Header: Part ID (large, monospace font)
  - Body: Description, current quantity
  - Footer: Location, last updated
  - Status: Color-coded availability badge
  
Visual Hierarchy:
  1. Part ID: Most prominent (H3, bold)
  2. Quantity: Second most prominent (large number)
  3. Status: Color-coded badge
  4. Secondary info: Smaller text, muted color
```

#### **Stock Level Indicators**

```yaml
Stock Gauge Design:
  Shape: Half-circle or linear progress bar
  Colors: Green (good) → Yellow (low) → Red (critical)
  Thresholds: Configurable per part
  Labels: Current/Target quantities
  
Quick Indicators:
  Dot Notation: ● ● ● ○ ○ (filled/empty circles)
  Color Bars: Horizontal bars with semantic colors
  Icons: Triangle (warning), Circle (OK), X (critical)
```

### **Operation Workflow Patterns**

#### **Process Step Indicators**

```yaml
Step Progression:
  Layout: Horizontal flow with connecting lines
  Active Step: Primary color circle with number
  Completed Step: Green circle with checkmark
  Future Step: Gray circle with number
  Connection Lines: Gray (incomplete), Primary (active path)
  
Step Details:
  Number/Icon: Centered in circle
  Label: Below circle
  Status: Above circle (optional)
  Time Estimate: Small text below label
```

#### **Operation Status Displays**

```yaml
Status Card Design:
  Background: Status-appropriate color (light tint)
  Border: Status-appropriate color (solid)
  Icon: Status-specific symbol
  Text: Clear status label
  Timestamp: When status changed
  
Status Hierarchy:
  1. Critical/Error: Red, exclamation icon
  2. Warning: Yellow, warning triangle
  3. In Progress: Blue, clock icon
  4. Complete: Green, checkmark icon
  5. Pending: Gray, circle icon
```

### **Data Entry Optimization**

#### **Manufacturing Form Patterns**

```yaml
Quick Entry Forms:
  Layout: Single column for mobile/tablet
  Field Order: Most common to least common
  Auto-complete: Based on recent entries
  Validation: Real-time with clear messaging
  Submit: Large button, keyboard accessible
  
Batch Entry:
  Layout: Table-like with repeated rows
  Add Row: Prominent "+" button
  Remove Row: Discrete "×" button on hover
  Validation: Per-row and overall
  Keyboard: Tab navigation, Enter to add row
```

#### **Part ID Entry Patterns**

```yaml
Part ID Field Design:
  Font: Monospace for character alignment
  Validation: Format checking as user types
  Auto-complete: Dropdown with recent/suggested parts
  Scanner Integration: Barcode scanner support
  Clear Button: Easy field reset
  
Visual Feedback:
  Valid ID: Green border/checkmark
  Invalid ID: Red border/error message
  Checking: Loading indicator
  Not Found: Warning with search suggestions
```

## 📐 **Layout and Grid Systems**

### **🚨 MANDATORY Tab View Layout Pattern**

**CRITICAL IMPLEMENTATION**: All tab views connected to MainView.axaml must implement the InventoryTabView standard layout pattern to ensure proper input field containment and professional UI consistency.

#### **Required Structure Pattern**

```xml
<!-- REQUIRED: ScrollViewer root container -->
<ScrollViewer HorizontalScrollBarVisibility="Auto" VerticalScrollBarVisibility="Auto"
              Background="{DynamicResource MTM_Shared_Logic.MainBackground}">
  
  <!-- REQUIRED: Main container with proper row separation -->
  <Grid x:Name="MainContainer" RowDefinitions="*,Auto" 
        MinWidth="600" MinHeight="400" Margin="8">

    <!-- REQUIRED: Content panel with theme integration -->
    <Border Grid.Row="0"
            Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderLightBrush}"
            BorderThickness="1" CornerRadius="8" Padding="16" Margin="0,0,0,8">
      
      <!-- REQUIRED: Form structure with proper containment -->
      <Grid x:Name="FormFieldsGrid" RowDefinitions="Auto,Auto,Auto,Auto,*" RowSpacing="12">
        <!-- Individual field grids with ColumnDefinitions="90,*" -->
      </Grid>
    </Border>
    
    <!-- REQUIRED: Action buttons panel -->
    <Border Grid.Row="1"
            Background="{DynamicResource MTM_Shared_Logic.PanelBackgroundBrush}"
            BorderBrush="{DynamicResource MTM_Shared_Logic.BorderAccentBrush}"
            BorderThickness="1" CornerRadius="6" Padding="12">
      <!-- Action buttons -->
    </Border>
  </Grid>
</ScrollViewer>
```

#### **Implementation Status**

- ✅ **InventoryTabView**: Reference implementation (complete)
- ❌ **RemoveTabView**: Requires update to this pattern  
- ❌ **TransferTabView**: Requires update to this pattern

#### **Non-Negotiable Requirements**

1. **ScrollViewer as root container** - Prevents content overflow
2. **Grid with RowDefinitions="*,Auto"** - Separates content from actions
3. **All input fields contained within grid boundaries** - Professional appearance
4. **DynamicResource bindings for ALL colors** - Theme consistency
5. **Consistent spacing: 8px, 16px, 24px** - Visual hierarchy

---

### **Responsive Grid Framework**

#### **Breakpoint System**

```yaml
Manufacturing Workstation Sizes:
  Small: 1024px - 1365px (older workstations)
  Medium: 1366px - 1919px (standard workstations)  
  Large: 1920px - 2559px (modern workstations)
  XLarge: 2560px+ (ultra-wide displays)
  
Grid Columns:
  Small: 8 columns
  Medium: 12 columns
  Large: 16 columns
  XLarge: 20 columns
  
Gutter Width: 16px (consistent across breakpoints)
```

#### **Content Area Guidelines**

```yaml
Maximum Content Width:
  Reading Content: 720px
  Form Content: 480px
  Data Tables: Full width with horizontal scroll
  Dashboard Cards: 320px per card
  
Minimum Content Width:
  Forms: 280px
  Cards: 240px
  Buttons: 80px
  Touch Targets: 44px
```

### **Dashboard Layout Patterns**

#### **Widget Grid System**

```yaml
Widget Sizes:
  Small: 1x1 grid units (280px × 200px)
  Medium: 2x1 grid units (576px × 200px)
  Large: 2x2 grid units (576px × 416px)
  Wide: 3x1 grid units (872px × 200px)
  
Widget Types:
  KPI: Current numbers, trends (Small/Medium)
  Charts: Line/bar charts (Medium/Large)
  Lists: Recent items, alerts (Medium/Wide)
  Actions: Quick buttons (Small)
  
Grid Behavior:
  Auto-fit: Widgets adjust to available space
  Drag & Drop: User customizable layouts
  Responsive: Stack on smaller screens
```

## 🎭 **Animation and Interaction Design**

### **Motion Principles**

#### **Animation Guidelines**

```yaml
Duration Standards:
  Micro-interactions: 100-200ms
  Component transitions: 200-300ms
  Page transitions: 300-500ms
  Loading states: 1s+ (with progress indication)
  
Easing Functions:
  Enter animations: ease-out (starts fast, slows down)
  Exit animations: ease-in (starts slow, speeds up)
  Continuous: ease-in-out (smooth start and end)
  
Animation Purposes:
  Feedback: Confirm user actions
  Attention: Draw focus to important changes
  Continuity: Show relationships between elements
  Loading: Indicate system processing
```

#### **Interaction Feedback**

```yaml
Button Interactions:
  Hover: Background color change (100ms)
  Press: Scale down to 98% (100ms)
  Success: Green flash animation (200ms)
  Error: Red shake animation (300ms)
  
Form Field Interactions:
  Focus: Border color change (150ms)
  Error: Shake animation + color change (300ms)
  Success: Checkmark fade-in (200ms)
  
Loading States:
  Button: Spinner replaces text
  Form: Overlay with progress indicator
  Page: Skeleton screens during load
  Data: Progressive enhancement
```

### **Micro-Interactions**

#### **Manufacturing-Specific Interactions**

```yaml
Part Selection:
  Visual: Highlight with primary color border
  Audio: Optional confirmation beep
  Haptic: Not applicable (desktop application)
  Duration: Immediate (no delay)
  
Quantity Changes:
  Visual: Number animation (count up/down)
  Validation: Real-time bounds checking
  Feedback: Color change for valid/invalid values
  Undo: Clear visual indication of changes
  
Status Updates:
  Visual: Status badge color transition
  Animation: Pulse effect for new statuses
  Grouping: Related status changes animate together
  Persistence: Status changes logged visibly
```

This comprehensive design system ensures that the MTM WIP Application maintains visual consistency, optimal user experience, and manufacturing-specific functionality while adhering to modern design principles and accessibility standards.
