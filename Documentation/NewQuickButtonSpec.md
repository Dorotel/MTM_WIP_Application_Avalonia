# NewQuickButtonView.axaml - User Story + Specification

## **🎯 User Story**

**As a** manufacturing operator in the MTM WIP Application  
**I want** to create new QuickButton shortcuts for my frequently used inventory operations  
**So that** I can rapidly perform repetitive part inventory transactions without repeatedly entering the same Part ID, Operation, and Quantity combinations.

---

## **📋 Feature Specifications**

### **🎪 Primary Use Case**
**Quick Manufacturing Button Creation Interface**
- **Purpose**: Enable users to define custom shortcuts for frequent inventory operations
- **Context**: Manufacturing floor environment requiring rapid inventory entry
- **Integration**: Seamlessly connects with QuickButtons panel and InventoryTab workflow

### **🎨 UI/UX Specifications**

#### **Visual Layout**
- **Container**: ScrollViewer with contained Grid layout (600x400 minimum)
- **Structure**: Two-section design - Content area + Action buttons
- **Theme**: MTM Windows 11 Blue design system with card-based aesthetics
- **Responsive**: Handles overflow with auto-scrolling

#### **Input Field Requirements**
| Field | Type | Validation | Behavior |
|-------|------|------------|----------|
| **Part ID** | TextBox | Master data validation | Fuzzy search with suggestion overlay |
| **Operation** | TextBox | Master data validation | Fuzzy search with suggestion overlay |  
| **Quantity** | TextBox | Integer > 0 required | Standard numeric input |

#### **Visual States**
- ✅ **Success State**: Green validation, enabled Create button
- ❌ **Error State**: Red borders, error watermarks, disabled Create button
- ⚠️ **Duplicate Warning**: Orange alert panel with icon
- 🔄 **Loading State**: Disabled inputs during processing

### **🔧 Technical Specifications**

#### **TextBox Fuzzy Validation Integration**
```csharp
// Suggestion Overlay Pattern (CRITICAL)
behaviors:TextBoxFuzzyValidationBehavior.EnableFuzzyValidation="True"
behaviors:TextBoxFuzzyValidationBehavior.ValidationSource="{Binding [MasterDataCollection]}"

// Event Subscription in Code-Behind
TextBoxFuzzyValidationBehavior.SuggestionOverlayRequested += OnSuggestionOverlayRequested;
```

#### **Validation Logic Requirements**
- **Master Data Integration**: Validate against `_masterDataService.PartIDs` and `_masterDataService.Operations`
- **Duplicate Detection**: Check existing QuickButtons for PartID + Operation combinations
- **Real-time Validation**: Property change handlers trigger immediate validation feedback
- **Input Sanitization**: Clear invalid entries automatically on validation failure

#### **MVVM Community Toolkit Implementation**
```csharp
[ObservableProperty] private string _selectedOperation = string.Empty;
[ObservableProperty] private string _operationWatermark = "Enter operation...";
[ObservableProperty] private bool _isOperationValid = true;

// Computed Properties
public bool IsOperationInvalid => !IsOperationValid;
public IEnumerable<string> Operations => _masterDataService.Operations;
```

### **📊 Business Logic Specifications**

#### **Manufacturing Domain Rules**
- **Part ID**: Must exist in master data (`md_part_ids_Get_All` stored procedure)
- **Operation**: Must be valid workflow step ("90", "100", "110", "120", etc.)
- **Quantity**: Manufacturing count (integer > 0)
- **Transaction Context**: User intent determines IN/OUT/TRANSFER classification

#### **QuickButton Creation Workflow**
1. **Input Validation**: All fields validated against master data
2. **Duplicate Check**: Prevent identical PartID + Operation combinations
3. **Database Integration**: Call `_quickButtonsService.CreateQuickButtonAsync()`
4. **UI Feedback**: Success overlay + refresh QuickButtons panel
5. **Navigation**: Return focus to calling interface

#### **Integration Points**
```csharp
// Service Dependencies
IQuickButtonsService _quickButtonsService;
IMasterDataService _masterDataService;
ISuggestionOverlayService _suggestionOverlayService;

// Event System
public event EventHandler? QuickButtonCreated;
public event EventHandler? Cancelled;
```

### **🎮 Interaction Specifications**

#### **Keyboard Navigation**
- **Tab Order**: Part ID → Operation → Quantity → Cancel → Create
- **Enter Key**: Advance to next field or trigger Create button
- **Escape Key**: Cancel overlay and return to previous interface

#### **Suggestion Overlay Behavior**
- **Trigger**: Typing in Part ID or Operation fields
- **Display**: Modal overlay with filtered suggestions
- **Selection**: Click or Enter key applies suggestion
- **Integration**: Uses `SuggestionOverlayService` with MainView panel system

#### **Error Handling**
- **Visual Feedback**: Red borders, error watermarks, status messages
- **Status Updates**: Real-time validation messaging
- **Duplicate Alert**: Prominent warning with specific messaging

### **🔄 Event-Driven Architecture**

#### **Lifecycle Events**
```csharp
// Initialization
OnViewLoaded() → Subscribe to suggestion overlay events
OnPropertyChanged() → Validate inputs and check duplicates

// Completion
OnCreateCommand() → Validate → Create → Fire QuickButtonCreated event
OnCancelCommand() → Fire Cancelled event
OnDetachedFromVisualTree() → Cleanup subscriptions
```

#### **Service Integration Events**
- **SuggestionOverlayRequested**: Triggered by TextBoxFuzzyValidationBehavior
- **QuickButtonCreated**: Notifies MainView to refresh QuickButtons
- **PropertyChanged**: MVVM Community Toolkit observable property updates

### **📈 Success Criteria**

#### **Functional Requirements**
- ✅ Create QuickButtons with valid Part ID + Operation + Quantity
- ✅ Prevent duplicate QuickButton combinations
- ✅ Provide instant suggestion overlays for Part ID and Operation fields
- ✅ Integrate seamlessly with MTM manufacturing workflow
- ✅ Handle all error states gracefully with clear user feedback

#### **Performance Requirements**
- ⚡ Suggestion overlays appear within 200ms of typing
- ⚡ Master data validation occurs in real-time without UI blocking
- ⚡ QuickButton creation completes within 2 seconds
- ⚡ No memory leaks from event subscriptions

#### **Integration Requirements**
- 🔗 Fully compatible with InventoryTabView suggestion overlay pattern
- 🔗 Uses identical TextBoxFuzzyValidationBehavior implementation
- 🔗 Follows MTM design system and theme consistency
- 🔗 Proper cleanup and resource management

---

## **🎪 User Experience Flow**

1. **Entry**: User clicks "Create Quick Button" from QuickButtons panel
2. **Interface**: NewQuickButtonView overlay appears with focus on Part ID field
3. **Input**: User types Part ID → suggestion overlay appears → selection applied
4. **Validation**: Real-time feedback shows validation status
5. **Operation**: User enters Operation → suggestion overlay → validation
6. **Quantity**: User enters quantity → validation
7. **Creation**: All fields valid → Create button enabled → QuickButton created
8. **Feedback**: Success message + overlay closes + QuickButtons panel refreshes
9. **Integration**: New QuickButton available for one-click inventory operations

**This interface bridges the gap between user convenience and manufacturing precision, enabling rapid creation of customized workflow shortcuts while maintaining data integrity and system integration.**