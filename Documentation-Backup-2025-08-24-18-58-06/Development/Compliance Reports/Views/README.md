# Views Compliance Reports

This directory contains compliance reports for Avalonia AXAML view files.

## View Standards

AXAML view files must comply with:
- **Avalonia Controls**: Use proper Avalonia UI controls, not WinForms
- **Compiled Bindings**: x:CompileBindings="True" with x:DataType
- **Modern Layout**: Grid over StackPanel, proper spacing and margins
- **MTM Color Scheme**: DynamicResource for colors, MTM purple palette
- **Theme Support**: No hard-coded colors, use resource references
- **Card-Based Design**: Modern layouts with cards, shadows, gradients
- **Responsive Design**: Proper column definitions and responsive patterns

## Current View Reports

*Reports will be generated here as Views are audited*

## Expected Views

Based on MTM WIP Application UI structure:
- `MainWindow.axaml` - Main application window with sidebar
- `MainView.axaml` - Primary content area
- `InventoryTabView.axaml` - Inventory management interface
- `RemoveTabView.axaml` - Item removal interface
- `QuickButtonsView.axaml` - Quick action buttons panel
- `TransactionHistoryView.axaml` - Transaction history display

---

*Generated compliance reports will appear in this directory following the naming convention: {ViewName}-compliance-report-{YYYY-MM-DD}.md*