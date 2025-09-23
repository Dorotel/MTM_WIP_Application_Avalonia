# MTM Component Library Documentation

## 📋 Overview

This document provides comprehensive documentation for all reusable UI components in the MTM WIP Application, including implementation details, usage guidelines, and architectural requirements for maintaining consistency across the manufacturing workflow interface.

**Currently Implemented Components:**

- **CollapsiblePanel** - Expandable/collapsible container for organizing content
- **CustomDataGrid** - Advanced data grid with filtering, column management, and manufacturing-specific features
- **TransactionExpandableButton** - Interactive button for transaction history display

## 🧱 **Core Component Architecture**

### **Service Integration Pattern**

The MTM application follows a consistent pattern for component integration with the application's service layer through dependency injection. All components should access services through Application.Current.Services when needed.

**Required Integration Approach:**

- Components should access `ILogger<ComponentType>` for consistent logging
- Service dependencies should be resolved in component constructors
- Error handling should use ServiceResult pattern from service layer
- All database operations must go through appropriate service interfaces (never direct database access)

### **MVVM Community Toolkit Integration**

All components must integrate seamlessly with MVVM Community Toolkit 8.3.2 patterns:

**Property Binding Requirements:**

- Use ObservableProperty source generators in ViewModels
- Support two-way binding for input components
- Implement PropertyChanged notifications appropriately
- Follow RelayCommand patterns for user actions

**Data Binding Standards:**

- All bindable properties must be StyledProperty definitions
- Support both ItemsSource and individual property binding patterns
- Implement proper default values and validation

## 📊 **Data Components**

### **CustomDataGrid - Advanced Manufacturing Data Display**

#### **Purpose and Context**

The CustomDataGrid serves as the primary data visualization component for manufacturing operations, specifically designed to handle large datasets with WinForms-style multi-selection capabilities optimized for inventory management workflows.

#### **Core Architecture Requirements**

**Multi-Selection Implementation:**

- **Ctrl+Click Behavior**: Must toggle individual item selection without affecting other selections
- **Shift+Click Behavior**: Must select range from last selected index to clicked index
- **Drag Selection**: Should support mouse drag for selecting ranges of items
- **Keyboard Navigation**: Must support arrow key navigation with Ctrl/Shift modifiers
- **Selection State Persistence**: Maintain selection across data refreshes when possible

**Column Management System:**

- **Dynamic Configuration**: Must support runtime column show/hide functionality
- **Column Reordering**: Should allow drag-and-drop column reordering
- **Persistent Settings**: Column configurations must persist between sessions
- **Service Integration**: Must integrate with ICustomDataGridService for configuration management

**Filtering Infrastructure:**

- **Real-time Filtering**: Must support live filtering as user types
- **Multiple Filter Types**: Text, numeric range, date range, dropdown selection filters
- **Filter Persistence**: Applied filters should persist during data refreshes
- **Clear Filters**: Must provide easy way to clear all or individual filters

**Performance Optimization:**

- **Virtual Scrolling**: Must implement virtualization for large datasets (1000+ items)
- **Lazy Loading**: Should support progressive loading of data batches
- **Memory Management**: Proper cleanup of resources and event handlers
- **Update Batching**: Batch UI updates during bulk operations

#### **Integration Points**

**Service Layer Integration:**

- Must integrate with ICustomDataGridService for configuration and data operations
- Should use `ILogger<CustomDataGrid>` for consistent logging patterns
- Error handling must follow MTM ServiceResult patterns
- All data operations should be async with proper cancellation token support

**MVVM Integration:**

- ItemsSource binding for data collections
- SelectedItems binding for multi-selection support
- Command bindings for user actions (refresh, export, etc.)
- Property bindings for configuration settings

**Theme System Integration:**

- Must use DynamicResource bindings for all colors and styling
- Support all 19 MTM themes with proper contrast ratios
- Implement theme switching without data loss or selection reset

#### **Manufacturing-Specific Features**

**Inventory Context:**

- Support for part-specific data types (Part IDs, quantities, locations)
- Specialized formatting for manufacturing data (dates, measurements)
- Integration with inventory transaction workflows
- Support for manufacturing-specific sorting and grouping

**Operational Requirements:**

- **Glove-Friendly Touch Targets**: Minimum 44px touch targets for manufacturing floor use
- **High Contrast Support**: Must work with MTM_HighContrast theme for industrial lighting
- **Keyboard-Only Operation**: Complete functionality available without mouse
- **Screen Reader Support**: Proper ARIA labels and accessible navigation

### **CollapsiblePanel - Content Organization Container**

#### **CollapsiblePanel - Purpose and Context**

The CollapsiblePanel addresses the manufacturing interface challenge of presenting comprehensive information while maintaining clean, organized layouts. It provides hierarchical content organization essential for complex manufacturing workflows.

#### **CollapsiblePanel - Core Architecture Requirements**

**Flexibility Requirements:**

- **Header Position Options**: Must support Left, Right, Top, Bottom header positioning
- **Content Type Agnostic**: Must support any Avalonia UI content (controls, data grids, forms)
- **Animation Support**: Smooth expand/collapse animations with configurable duration
- **State Persistence**: Remember expansion state across application sessions

**Visual Design Requirements:**

- **Gold Header Bar**: Must implement MTM gold branding color for headers
- **Theme Integration**: Full integration with all 19 MTM themes
- **Professional Appearance**: Consistent with MTM design system standards
- **Icon Integration**: Material.Icons for expand/collapse indicators with smooth rotation

**Behavioral Requirements:**

- **Click Expansion**: Header click should toggle expansion state
- **Keyboard Support**: Enter/Space key support for accessibility
- **Touch Support**: Touch-friendly header sizing for manufacturing floor use
- **Content Scrolling**: Proper scroll handling for content that exceeds panel size

#### **Implementation Architecture**

**StyledProperty Definitions Needed:**

- `IsExpanded` (bool) - Expansion state with PropertyChanged notification
- `HeaderText` (string) - Display text for panel header
- `HeaderPosition` (enum) - Position of header relative to content
- `Content` (object) - Content to display in expandable area
- `HeaderBackground` (Brush) - Customizable header background color

**Event Handling Requirements:**

- Header click event handling for expansion toggle
- Keyboard event handling for accessibility
- Property changed callbacks for state management
- Content loading events for lazy content scenarios

**Animation System:**

- Smooth height/width transitions during expand/collapse
- Icon rotation animations for expand/collapse indicators
- Configurable animation duration and easing functions
- Performance optimization for multiple panels on single view

#### **Manufacturing Applications**

**Common Use Cases:**

- **Equipment Details**: Collapsible machine status and diagnostic information
- **Inventory Sections**: Organized part information with expandable details
- **Process Instructions**: Step-by-step manufacturing procedures
- **Quality Control**: Inspection checklists and criteria organization

**Integration Scenarios:**

- **Forms Organization**: Group related form sections for better UX
- **Dashboard Widgets**: Collapsible dashboard sections for customizable views
- **Report Sections**: Organized reporting with expandable detail sections
- **Settings Panels**: Hierarchical settings organization

### **TransactionExpandableButton - Manufacturing Transaction Display**

#### **TransactionExpandableButton - Purpose and Context**

The TransactionExpandableButton specializes in manufacturing transaction history display, providing compact summary views with expandable detailed information. It addresses the need for quick transaction overview while maintaining access to comprehensive transaction details.

#### **TransactionExpandableButton - Core Architecture Requirements**

**Transaction Data Properties:**

- **PartId** (string) - Manufacturing part identification
- **Operation** (string) - Type of operation performed (Assembly, Inspection, Transfer)
- **Quantity** (int) - Quantity involved in transaction
- **TransactionType** (string) - Transaction classification (IN, OUT, TRANSFER)
- **Timestamp** (DateTime) - When transaction occurred
- **UserId** (string) - User who performed transaction
- **Notes** (string) - Additional transaction notes
- **RelatedTransactions** (collection) - Linked transactions for traceability

**Visual Design System:**

- **Color Coding Logic**: IN transactions (Green), OUT transactions (Red), TRANSFER transactions (Yellow)
- **Compact Summary Layout**: Part ID prominent, operation and quantity secondary
- **Expanded Detail Layout**: Full transaction information with proper hierarchy
- **Professional Styling**: Consistent with MTM theme system and manufacturing context

**Behavioral Requirements:**

- **Single-Click Expansion**: Toggle expanded state with smooth animation
- **Keyboard Accessibility**: Enter/Space key activation support
- **Touch-Friendly**: Minimum 44px target size for manufacturing floor use
- **Screen Reader Support**: Descriptive accessible names for transaction context

#### **Manufacturing Integration**

**Session History Integration:**

- Must integrate with SessionHistoryPanel for transaction list display
- Support binding to collections of transaction objects
- Provide proper ItemTemplate support for data templates
- Handle large transaction lists with performance optimization

**Service Layer Integration:**

- Integration with transaction services for detailed data loading
- Lazy loading of detailed transaction information on expansion
- Support for transaction filtering and searching
- Integration with audit trail and compliance systems

**MVVM Pattern Support:**

- Command binding for expansion actions
- Property binding for all transaction properties
- Collection binding for related transactions
- Event binding for transaction selection and interaction

#### **Accessibility and Usability**

**Manufacturing Floor Requirements:**

- **High Contrast Support**: Must work with MTM_HighContrast theme
- **Glove Operation**: Touch targets sized for gloved hands
- **Quick Recognition**: Visual indicators for rapid transaction type identification
- **Information Hierarchy**: Critical information prominently displayed

**Compliance Integration:**

- **Audit Trail Support**: Integration with manufacturing audit systems
- **Regulatory Compliance**: Support for manufacturing compliance reporting
- **Data Integrity**: Proper handling of sensitive manufacturing data
- **User Activity Tracking**: Integration with user activity logging systems

## 🔧 **Implementation Guidelines**

### **Service Integration Standards**

All components must follow these service integration patterns:

**Dependency Resolution:**

- Use `Application.Current.Services.GetService<T>()` pattern
- Implement proper null checking for service availability
- Use constructor-based service resolution when possible
- Handle service unavailability gracefully

**Logging Integration:**

- Use `ILogger<ComponentType>` for all logging
- Follow MTM logging patterns and message formats
- Log user interactions for audit purposes
- Implement proper error logging with context

**Configuration Management:**

- Use Microsoft.Extensions.Configuration for settings
- Support appsettings.json configuration patterns
- Implement configuration change handling
- Provide sensible default values

### **Theme System Integration**

**DynamicResource Requirements:**

- All colors must use DynamicResource bindings
- No hardcoded color values in component definitions
- Support theme switching without component restart
- Maintain visual consistency across all themes

**Contrast Ratio Compliance:**

- All components must meet WCAG 2.1 AA standards
- Support MTM_HighContrast theme requirements
- Provide sufficient contrast for manufacturing environments
- Test with all 19 MTM theme variants

### **Performance Standards**

**Memory Management:**

- Proper disposal of event handlers and resources
- Avoid memory leaks during component lifecycle
- Implement proper garbage collection patterns
- Monitor memory usage during extended operations

**Responsiveness Requirements:**

- All UI operations must complete within 100ms
- Long operations must show progress indicators
- Implement proper async/await patterns
- Use cancellation tokens for cancellable operations

This documentation provides the architectural foundation for implementing, maintaining, and extending the MTM component library while ensuring consistency with manufacturing requirements and modern development practices.