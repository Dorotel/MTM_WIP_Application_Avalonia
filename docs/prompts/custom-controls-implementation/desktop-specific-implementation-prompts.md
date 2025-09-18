# Desktop-Specific Custom Controls Implementation Prompts

**Ready-to-Use GitHub Copilot Prompts for Desktop Manufacturing Controls**  
**Platform**: Windows Desktop Workstations Only  
**Framework**: Avalonia UI 11.3.4 with .NET 8  
**Focus**: Keyboard + Mouse + Native Windows Integration  

---

## 🖥️ Windows Integration Controls

### **WindowsExcelIntegrationControl Implementation Prompt**

```text
Create a WindowsExcelIntegrationControl for native Excel integration in Windows manufacturing workstations with:

WINDOWS DESKTOP REQUIREMENTS:
- COM Interop integration with Microsoft Excel installed on workstation
- Real-time data synchronization between MTM and Excel spreadsheets
- Template management for manufacturing reports (inventory, production, quality)
- Excel formula integration for manufacturing calculations
- Native Excel dialog integration for advanced operations

MANUFACTURING FEATURES:
- Import inventory data from Excel with validation against MTM master data
- Export manufacturing reports to Excel with formatting and formulas
- Template library for common manufacturing spreadsheets (BOM, production schedules)
- Real-time data binding: changes in Excel reflect in MTM and vice versa
- Manufacturing-specific Excel functions (yield calculations, material requirements)

DESKTOP INTEGRATION:
- Windows file association handling for .xlsx files
- Excel process management (start/stop Excel application)
- Memory management for COM objects and Excel instances
- Multi-monitor support when Excel and MTM span different monitors
- Windows clipboard integration for seamless data exchange

TECHNICAL REQUIREMENTS:
- Microsoft.Office.Interop.Excel integration
- Avalonia UI 11.3.4 UserControl base with MVVM Community Toolkit
- Async/await patterns for Excel operations to avoid UI blocking
- Proper COM object disposal and memory management
- MTM theme integration with Excel color scheme mapping

PERFORMANCE TARGETS:
- Handle 10,000+ row Excel imports within 30 seconds
- Real-time sync with max 2-second latency
- Memory efficient COM interop with automatic cleanup
- Support multiple concurrent Excel instances

Generate complete implementation with Excel template management and manufacturing-specific integrations.
```

### **BarcodeKeyboardWedgeControl Implementation Prompt**

```text
Create a BarcodeKeyboardWedgeControl optimized for Windows desktop manufacturing with USB barcode scanners:

HARDWARE INTEGRATION FOCUS:
- USB barcode scanner integration via keyboard wedge mode (scanners act as keyboard input)
- Automatic barcode format detection (Code 128, Code 39, UPC, QR codes)
- Scanner configuration management for different manufacturing environments
- Support for multiple scanners on single workstation (inventory + shipping)
- Windows USB device notifications for scanner connect/disconnect events

MANUFACTURING INTELLIGENCE:
- Part ID barcode validation against MTM master data
- Location barcode scanning with visual confirmation
- Operation barcode scanning for workflow tracking
- Quantity scanning from printed labels with validation
- Manufacturing context awareness (different behavior for inventory vs shipping)

DESKTOP OPTIMIZATION:
- Keyboard focus management (auto-focus target fields after scan)
- Visual scan confirmation with desktop notifications
- Audio feedback integration with Windows sound system
- Error handling with Windows error dialog integration
- Scan history logging with Windows Event Log integration

TECHNICAL REQUIREMENTS:
- Windows HID (Human Interface Device) API integration
- Avalonia UI KeyDown event handling for barcode data capture
- Real-time validation via Helper_Database_StoredProcedure patterns
- MVVM Community Toolkit [ObservableProperty] for scan results
- MTM theme integration with scan confirmation visuals

PERFORMANCE TARGETS:
- Sub-100ms scan processing and validation
- Support for 1000+ scans per shift without performance degradation
- Concurrent scanner support (2-3 scanners per workstation)
- Real-time database validation with stored procedure integration

Generate complete Windows-optimized implementation with manufacturing workflow integration.
```

## 🎯 Performance-Optimized Desktop Controls

### **DesktopMemoryOptimizedCacheControl Implementation Prompt**

```text
Create a DesktopMemoryOptimizedCacheControl leveraging desktop system memory for manufacturing data caching:

DESKTOP MEMORY UTILIZATION:
- Dynamic memory allocation based on available system RAM (8GB+ desktop systems)
- Intelligent cache eviction strategies (LRU, manufacturing context-based)
- Background cache warming for frequently accessed manufacturing data
- Memory pressure monitoring with Windows Performance Counters
- Cache persistence to disk for application restart scenarios

MANUFACTURING DATA OPTIMIZATION:
- Master data caching (10,000+ parts, 1000+ locations, operation sequences)
- Recent transaction caching for manufacturing history operations
- User preference caching for personalized manufacturing workflows
- Validation rule caching for real-time manufacturing data validation
- Manufacturing context caching (current operation, shift information)

DESKTOP PERFORMANCE FEATURES:
- Multi-threading for cache operations using desktop multi-core processors
- Background refresh of cached data during idle periods
- Cache hit/miss metrics with Windows Performance Counter integration
- Memory usage monitoring and automatic optimization
- Cache preloading based on manufacturing schedule and patterns

TECHNICAL REQUIREMENTS:
- .NET MemoryCache with custom eviction policies
- Background Task support for cache maintenance
- Windows Performance Counter integration for metrics
- MVVM Community Toolkit integration for cache status properties
- MTM stored procedure integration for data refresh operations

CACHE STRATEGIES:
- Part ID cache: 50,000 entries with 24-hour expiration
- Operation cache: All manufacturing operations with real-time updates
- Location cache: All locations with dependency tracking
- User preference cache: Personalized settings with instant access
- Validation cache: Manufacturing rules with 1-hour refresh

Generate complete implementation with manufacturing-optimized caching strategies and desktop memory management.
```

### **HardwareAcceleratedChartControl Implementation Prompt**

```text
Create a HardwareAcceleratedChartControl leveraging desktop GPU for manufacturing analytics:

DESKTOP GRAPHICS ACCELERATION:
- DirectX/OpenGL acceleration for smooth 60fps chart rendering
- GPU-based data point rendering for 100,000+ data points
- Hardware-accelerated animations and transitions
- Multi-monitor support with per-monitor GPU optimization
- Desktop GPU memory utilization for chart data buffering

MANUFACTURING ANALYTICS FEATURES:
- Real-time production metrics visualization (throughput, quality, efficiency)
- Inventory level trending with predictive analytics
- Quality control charts with statistical process control
- Manufacturing cycle time analysis with bottleneck identification
- Cost analysis charts with profitability trending

DESKTOP INTERACTION PATTERNS:
- Mouse zoom and pan with smooth hardware acceleration
- Keyboard shortcuts for chart navigation (arrow keys, page up/down)
- Right-click context menus for chart operations (export, print, analyze)
- Multi-selection of data points with Ctrl+Click desktop patterns
- Copy chart data to Windows clipboard in multiple formats

TECHNICAL REQUIREMENTS:
- Avalonia UI with Skia graphics backend for hardware acceleration
- Real-time data binding with manufacturing database stored procedures
- MVVM Community Toolkit integration for chart configuration properties
- Windows-optimized rendering pipeline leveraging desktop graphics cards
- MTM theme integration with professional chart styling

PERFORMANCE TARGETS:
- 60fps smooth scrolling and zooming with 100,000+ data points
- Sub-100ms response to data updates and user interactions
- Support for 10+ concurrent charts on multi-monitor setups
- Memory efficient GPU buffer management with automatic optimization

Generate complete hardware-accelerated implementation optimized for Windows desktop manufacturing analytics.
```

---

## 📋 Professional Desktop UI Controls

### **ManufacturingRibbonControl Implementation Prompt**

```text
Create a ManufacturingRibbonControl with Office-style interface optimized for manufacturing operations:

DESKTOP RIBBON INTERFACE:
- Office 2019/365 style ribbon with manufacturing-specific tabs
- Contextual tabs that appear based on current manufacturing operation
- Quick Access Toolbar for frequently used manufacturing functions
- Keyboard navigation with Alt+Key mnemonics for all ribbon items
- Ribbon customization with user preference persistence

MANUFACTURING WORKFLOW ORGANIZATION:
- Home Tab: Common operations (Add Inventory, Remove Items, Search)
- Inventory Tab: Inventory-specific operations (Transfer, Adjust, Report)
- Quality Tab: Quality control operations (Inspect, Approve, Reject)
- Reports Tab: Manufacturing reports (Production, Inventory, Analysis)
- Tools Tab: Utilities (Export, Import, Settings, Maintenance)

DESKTOP OPTIMIZATION FEATURES:
- Windows 11 design language integration with Fluent Design elements
- High-DPI scaling support for manufacturing monitors
- Keyboard-first navigation with full accessibility support
- Context-sensitive help integration with F1 key support
- Integration with Windows accessibility features

TECHNICAL REQUIREMENTS:
- Avalonia UI UserControl with complex layout management
- MVVM Community Toolkit for ribbon state management
- Command binding for all ribbon operations with manufacturing context
- MTM theme system integration with ribbon styling
- Stored procedure integration for manufacturing operations

RIBBON CUSTOMIZATION:
- User-configurable Quick Access Toolbar
- Hide/show ribbon tabs based on user permissions
- Manufacturing role-based ribbon customization
- Keyboard shortcut customization and conflict resolution
- Ribbon state persistence across application sessions

Generate complete Office-style ribbon implementation optimized for Windows desktop manufacturing workflows.
```

### **WindowsNativeDialogControl Implementation Prompt**

```text
Create WindowsNativeDialogControl for consistent Windows dialog experience in manufacturing:

NATIVE WINDOWS INTEGRATION:
- Windows 11 native dialog appearance with proper theming
- System modal dialogs with correct Z-order and focus management
- Windows accessibility integration (narrator, high contrast)
- Native Windows dialog keyboard navigation patterns
- Integration with Windows sound scheme for dialog events

MANUFACTURING DIALOG TYPES:
- Confirmation dialogs for manufacturing operations (delete inventory, approve transactions)
- Input dialogs for manufacturing data entry (quantities, notes, reasons)
- Selection dialogs for manufacturing choices (operations, locations, users)
- Progress dialogs for long-running manufacturing operations
- Error dialogs with manufacturing context and resolution guidance

DESKTOP USER EXPERIENCE:
- Proper modal dialog behavior with desktop window management
- Keyboard shortcuts (Enter for OK, Escape for Cancel, Alt+Key for buttons)
- Tab navigation within dialog controls
- Context-sensitive help with F1 key integration
- Multi-monitor awareness for dialog positioning

TECHNICAL REQUIREMENTS:
- Avalonia UI Window-based dialogs with native Windows styling
- MVVM Community Toolkit for dialog data binding
- Async/await dialog patterns for non-blocking operations
- MTM theme integration with Windows system theme awareness
- Manufacturing context integration with dialog content

DIALOG PATTERNS:
- Manufacturing confirmation: "Delete 150 units of PART001 from STATION_A?"
- Manufacturing input: "Enter reason for inventory adjustment:"
- Manufacturing selection: "Select destination operation for transfer:"
- Manufacturing progress: "Processing 500 inventory transactions..."
- Manufacturing error: "Cannot remove inventory - insufficient quantity available"

Generate complete native Windows dialog implementation with manufacturing workflow integration.
```

---

This comprehensive set of desktop-focused implementation prompts provides immediate actionable guidance for developing Windows manufacturing workstation optimized custom controls.