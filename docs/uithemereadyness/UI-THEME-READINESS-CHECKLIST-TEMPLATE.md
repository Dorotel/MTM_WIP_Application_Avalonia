# UI Theme Readiness Checklist Template

**View File**: `[ViewName].axaml`  
**Analysis Date**: [DATE]  
**Analyst**: Automated Analysis  

## 🎯 Overview
This checklist ensures that UI elements use proper theme brushes for optimal visibility across light/dark themes and comply with MTM UI/UX guidelines.

---

## 📋 Theme Compliance Checklist

### ✅ Text Elements Compliance
- [ ] **Heading Text** - Uses `MTM_Shared_Logic.HeadingText` (#0056B3) or appropriate semantic color
- [ ] **Body Text** - Uses `MTM_Shared_Logic.BodyText` (#666666) for primary content
- [ ] **Secondary/Tertiary Text** - Uses `MTM_Shared_Logic.TertiaryTextBrush` (#666666) for less important text
- [ ] **Interactive Text** - Uses `MTM_Shared_Logic.InteractiveText` (#0056B3) for clickable text
- [ ] **Link Text** - Uses `MTM_Shared_Logic.LinkTextBrush` (#0056B3) for hyperlinks
- [ ] **Placeholder Text** - Uses `MTM_Shared_Logic.PlaceholderTextBrush` (#6C757D) for input placeholders
- [ ] **Disabled Text** - Uses `MTM_Shared_Logic.DisabledTextBrush` (#ADB5BD) for inactive elements
- [ ] **Overlay Text** - Uses `MTM_Shared_Logic.OverlayTextBrush` (#FFFFFF) for text over dark backgrounds

### 🔘 Button Elements Compliance
- [ ] **Primary Buttons** - Uses `MTM_Shared_Logic.PrimaryAction` (#0056B3) background
- [ ] **Secondary Buttons** - Uses `MTM_Shared_Logic.SecondaryAction` (#004085) background
- [ ] **Button Hover States** - Uses `MTM_Shared_Logic.PrimaryHoverBrush` / `SecondaryHoverBrush`
- [ ] **Button Pressed States** - Uses appropriate pressed state brushes
- [ ] **Disabled Button States** - Uses `MTM_Shared_Logic.PrimaryDisabledBrush` / `SecondaryDisabledBrush`
- [ ] **Button Text Color** - Proper contrast with button background (min 4.5:1 ratio)

### 📦 Container Elements Compliance
- [ ] **Main Background** - Uses `MTM_Shared_Logic.MainBackground` (#FFFFFF) or appropriate semantic color
- [ ] **Card Backgrounds** - Uses `MTM_Shared_Logic.CardBackground` (#F8F9FA) or `CardBackgroundBrush` (#FFFFFF)
- [ ] **Panel Backgrounds** - Uses `MTM_Shared_Logic.PanelBackgroundBrush` (#F8F9FA)
- [ ] **Border Elements** - Uses `MTM_Shared_Logic.BorderBrush` (#DEE2E6) or semantic variants
- [ ] **Sidebar Areas** - Uses `MTM_Shared_Logic.SidebarDark` (#212529) for dark sidebars
- [ ] **Content Areas** - Uses `MTM_Shared_Logic.ContentAreas` (#FFFFFF) for main content

### 📝 Form Controls Compliance  
- [ ] **Input Backgrounds** - Uses `MTM_Shared_Logic.InputBackground` (#FFFFFF)
- [ ] **Input Borders** - Uses `MTM_Shared_Logic.InputBorder` (#CED4DA)
- [ ] **Input Focus States** - Uses `MTM_Shared_Logic.InputFocusBorder` (#0056B3)
- [ ] **Input Hover States** - Uses `MTM_Shared_Logic.InputHoverBorder` (#004085)
- [ ] **Input Disabled States** - Uses `MTM_Shared_Logic.InputDisabledBackground` (#F8F9FA)
- [ ] **Input Error States** - Uses `MTM_Shared_Logic.InputErrorBorder` (#B71C1C)
- [ ] **Form Labels** - Proper contrast and semantic coloring

### 🔔 Status/Feedback Elements Compliance
- [ ] **Success Messages** - Uses `MTM_Shared_Logic.SuccessBrush` (#2E7D32)
- [ ] **Warning Messages** - Uses `MTM_Shared_Logic.WarningBrush` (#B85500)
- [ ] **Error Messages** - Uses `MTM_Shared_Logic.ErrorBrush` (#B71C1C) 
- [ ] **Info Messages** - Uses `MTM_Shared_Logic.InfoBrush` (#0056B3)
- [ ] **Critical Alerts** - Uses `MTM_Shared_Logic.Critical` (#B71C1C)
- [ ] **Transaction IN** - Uses `MTM_Shared_Logic.TransactionInBrush` (#2E7D32)
- [ ] **Transaction OUT** - Uses `MTM_Shared_Logic.TransactionOutBrush` (#DC3545)
- [ ] **Transaction Transfer** - Uses `MTM_Shared_Logic.TransactionTransferBrush` (#B85500)

### 🎨 Interactive States Compliance
- [ ] **Hover Effects** - Uses `MTM_Shared_Logic.HoverBackground` (#E9ECEF) or semantic variants
- [ ] **Selection States** - Uses `MTM_Shared_Logic.SelectionBrush` (#E3F2FD)
- [ ] **Focus Indicators** - Uses `MTM_Shared_Logic.FocusBrush` (#0056B3)
- [ ] **Active States** - Uses `MTM_Shared_Logic.ActiveBrush` (#0056B3)
- [ ] **Inactive States** - Uses `MTM_Shared_Logic.InactiveBrush` (#ADB5BD)

### 🚫 Hardcoded Color Validation
- [ ] **No Hex Colors** - No hardcoded hex values (#XXXXXX) found in AXAML
- [ ] **No Named Colors** - No hardcoded named colors (Red, Blue, etc.) found
- [ ] **No Inline Colors** - No Color="..." attributes with literal values
- [ ] **Dynamic Resource Usage** - All colors use {DynamicResource} or {StaticResource}

---

## 🎨 MTM UI/UX Guidelines Compliance

### ✅ Layout Standards
- [ ] **Grid Usage** - Uses `x:Name` (NOT `Name`) on Grid elements
- [ ] **Grid Definitions** - Uses attribute form: `ColumnDefinitions="Auto,*"` when possible
- [ ] **Avalonia Namespace** - Uses correct xmlns="https://github.com/avaloniaui"
- [ ] **Control Types** - Uses `TextBlock` instead of `Label`, appropriate Avalonia controls

### ✅ Spacing and Sizing
- [ ] **8px Spacing** - Small margins/padding use 8px
- [ ] **16px Spacing** - Medium spacing (card padding, form spacing) uses 16px
- [ ] **24px Spacing** - Large spacing (section separation) uses 24px
- [ ] **Consistent Margins** - Proper margin hierarchy maintained

### ✅ Card-Based Layout System
- [ ] **Border Usage** - Cards use Border controls with rounded corners
- [ ] **Card Padding** - Cards use 16px internal padding
- [ ] **Card Backgrounds** - Proper card background brush usage
- [ ] **Card Shadows** - Subtle shadow effects using theme brushes

### ✅ Typography Hierarchy
- [ ] **Font Sizing** - Consistent FontSize patterns used
- [ ] **Font Weights** - Proper FontWeight hierarchy (Bold for headings)
- [ ] **Line Height** - Appropriate line spacing for readability
- [ ] **Text Alignment** - Proper text alignment for layout structure

### ✅ Tab View Layout Pattern (Critical for MainView tabs)
- [ ] **ScrollViewer Root** - Uses ScrollViewer as root container to prevent overflow
- [ ] **Grid Structure** - Uses Grid with RowDefinitions="*,Auto" (content/actions separation)
- [ ] **Input Field Containment** - All input fields contained within grid boundaries
- [ ] **Theme Resource Binding** - Uses DynamicResource bindings for ALL colors

---

## 🔍 WCAG 2.1 AA Compliance Validation

### ✅ Contrast Ratios (Minimum 4.5:1)
- [ ] **Text on Background** - All text/background combinations meet 4.5:1 contrast
- [ ] **Interactive Elements** - Buttons, links meet contrast requirements
- [ ] **Form Controls** - Input fields, labels meet contrast requirements
- [ ] **Status Messages** - Success, warning, error messages meet contrast requirements

### ✅ Accessibility Features
- [ ] **Focus Indicators** - Visible focus states on all interactive elements
- [ ] **Color Independence** - Information not conveyed by color alone
- [ ] **High Contrast Support** - Elements work in high contrast themes
- [ ] **Screen Reader Support** - Proper semantic markup for assistive technology

---

## 📊 Analysis Results

### ✅ Compliant Elements
[List elements that are properly using theme resources]

### ⚠️ Issues Found
[List any hardcoded colors, improper brush usage, or guideline violations]

### 🔧 Recommended Fixes
[List specific fixes needed to achieve full compliance]

---

## 🎯 Compliance Score
- **Theme Resource Usage**: [X]% compliant
- **MTM UI/UX Guidelines**: [X]% compliant  
- **WCAG 2.1 AA Compliance**: [X]% compliant
- **Overall Score**: [X]% compliant

---

## ✅ Final Approval
- [ ] **Light Theme Tested** - All elements visible and properly styled
- [ ] **Dark Theme Tested** - All elements visible and properly styled  
- [ ] **High Contrast Tested** - All elements accessible in high contrast mode
- [ ] **Cross-Theme Compatibility** - No visual artifacts during theme switching
- [ ] **Build Validation** - View compiles without errors or warnings

**Status**: [PENDING/COMPLIANT/NEEDS-WORK]  
**Next Review Date**: [DATE]