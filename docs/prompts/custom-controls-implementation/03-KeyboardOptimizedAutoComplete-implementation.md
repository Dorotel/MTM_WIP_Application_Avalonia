# KeyboardOptimizedAutoComplete Control - Desktop Implementation Prompt

**GitHub Copilot Implementation Prompt**  
**Priority**: #3 (Very High ROI)  
**Complexity**: Medium (2-3 weeks)  
**Focus**: Windows Desktop Manufacturing Workstations

---

## 🎯 Paste this exact prompt to GitHub Copilot:

```text
Create a high-performance KeyboardOptimizedAutoComplete custom control for MTM WIP Application desktop manufacturing workstations. This control must be optimized for keyboard-first workflows with 60+ WPM data entry speeds.

DESKTOP MANUFACTURING REQUIREMENTS:

## Technical Specifications
- **Framework**: Avalonia UI 11.3.4 with .NET 8
- **Base Class**: UserControl with INotifyPropertyChanged
- **Pattern**: MVVM Community Toolkit integration with [ObservableProperty] and [RelayCommand]
- **Platform**: Windows Desktop (Keyboard + Mouse optimized)
- **Performance**: Handle 10,000+ manufacturing parts with sub-100ms filtering

## Desktop Keyboard Behavior (CRITICAL)
- **Arrow Keys**: Up/Down navigate suggestions, Left/Right move cursor
- **Enter Key**: Select highlighted suggestion and move to next field
- **Tab Key**: Auto-complete best match and move to next field
- **Escape Key**: Clear suggestions and revert to previous value
- **Ctrl+Space**: Manual trigger for suggestion dropdown
- **Home/End**: Navigate to first/last suggestion
- **Page Up/Down**: Navigate suggestions in pages of 10

## Manufacturing Intelligence Features
- **Part ID Recognition**: Smart recognition of manufacturing part patterns (ABC-123, PART001)
- **Operation Context**: Different behavior for Part IDs vs Operations (90,100,110,120) vs Locations
- **Recent Items Cache**: Recently used items appear first (desktop RAM optimization)
- **Manufacturing Validation**: Real-time validation against manufacturing master data
- **Barcode Integration**: Support for barcode scanner input (keyboard wedge mode)

## Desktop Performance Optimization
- **Virtualized Suggestions**: Only render visible suggestions for large datasets
- **Background Filtering**: Non-blocking suggestion generation using desktop threading
- **Smart Caching**: Desktop RAM-optimized caching for 10,000+ items
- **Debounced Input**: Optimize for fast typing without overwhelming the UI thread
- **Hardware Acceleration**: Leverage desktop GPU for smooth scrolling

## AXAML Structure Template
```xml
<UserControl x:Class="MTM_WIP_Application_Avalonia.Controls.KeyboardOptimizedAutoComplete"
             xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Grid RowDefinitions="Auto,Auto">
    <!-- Input TextBox with keyboard handling -->
    <TextBox Grid.Row="0" Name="InputTextBox" />
    
    <!-- Suggestion Popup with virtualized list -->
    <Popup Grid.Row="1" Name="SuggestionPopup">
      <Border Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}">
        <VirtualizingStackPanel Name="SuggestionsList" />
      </Border>
    </Popup>
  </Grid>
</UserControl>
```

## Code-Behind C# Implementation
```csharp
public partial class KeyboardOptimizedAutoComplete : UserControl
{
    // Desktop-optimized properties
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty = ...;
    public static readonly StyledProperty<object> SelectedItemProperty = ...;
    public static readonly StyledProperty<string> FilterPropertyProperty = ...;
    public static readonly StyledProperty<int> MaxSuggestionsProperty = ...;
    public static readonly StyledProperty<ManufacturingContextType> ContextProperty = ...;
    
    // Keyboard handling
    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Down: NavigateDown(); break;
            case Key.Up: NavigateUp(); break;
            case Key.Enter: SelectCurrent(); break;
            case Key.Escape: ClearSuggestions(); break;
            case Key.Tab: AutoCompleteAndMoveFocus(); break;
            // ... more keyboard handling
        }
        base.OnKeyDown(e);
    }
}
```

## Manufacturing Context Integration
- **PartId Context**: Validate against md_part_ids_Get_All stored procedure
- **Operation Context**: Limit to valid operations (90, 100, 110, 120)
- **Location Context**: Filter based on active manufacturing locations
- **Recent Items**: Cache last 50 used items per manufacturing context

## MTM Architecture Integration
- **Service Integration**: Use IMasterDataService for manufacturing data
- **Theme Compliance**: Full DynamicResource integration for all MTM themes
- **Error Handling**: Use Services.ErrorHandling.HandleErrorAsync() pattern
- **Database Pattern**: Integrate with existing stored procedure patterns

## Desktop UX Requirements
- **Visual Focus**: Clear focus indicators for keyboard navigation
- **Mouse Support**: Click to select suggestions, hover highlighting
- **Context Menu**: Right-click support for copy/paste/clear operations
- **DPI Scaling**: Perfect rendering on high-DPI manufacturing monitors
- **Multi-Monitor**: Proper popup positioning on multi-monitor setups

## Performance Targets
- **Filtering Speed**: <50ms for 10,000 items
- **Memory Usage**: <50MB for full manufacturing parts dataset
- **Keyboard Response**: <16ms response time (60fps)
- **Startup Time**: <100ms initialization time

## Testing Requirements
- Unit tests for keyboard navigation and filtering logic
- Performance tests with 10,000+ manufacturing items
- Cross-platform tests (Windows primary, macOS/Linux secondary)
- Manufacturing context validation tests
- Keyboard accessibility tests

Create complete implementation with all keyboard shortcuts, manufacturing intelligence, and desktop optimization features. Focus on manufacturing operator efficiency and Windows desktop integration.

Include comprehensive XML documentation and usage examples for manufacturing scenarios.

#github-pull-request_copilot-coding-agent
```

---

## 🖥️ Desktop Manufacturing Integration Notes

### **Usage in Manufacturing Views**
This control replaces standard ComboBox/TextBox combinations in:
- InventoryTabView (Part ID selection)
- RemoveTabView (Part ID and Location selection)  
- TransferTabView (Source/Destination location selection)
- All SettingsForm views for master data entry

### **Keyboard Efficiency Impact**
- **Current**: 3-4 clicks + typing for part selection
- **With Control**: Type + Enter for instant selection
- **Speed Improvement**: 70% faster part selection workflows

### **Manufacturing Operator Benefits** 
- Muscle memory keyboard patterns
- No mouse required for common operations
- Intelligent suggestions based on manufacturing context
- Immediate validation feedback for data quality

### **Technical Architecture Benefits**
- Eliminates 50+ manual ComboBox implementations
- Consistent keyboard behavior across all manufacturing forms
- Centralized manufacturing data validation
- Desktop performance optimization throughout application