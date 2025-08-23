<!-- Copilot: Reading ui-mapping.instruction.md — UI Mapping Reference (Avalonia) -->
# UI Mapping Reference (Avalonia)

This file lists the mappings between `.instructions.md` files and corresponding screenshots in `UI_Winform_Screenshots/`.

> For control and event naming, see [naming-conventions.instruction.md](naming-conventions.instruction.md).

## Common Controls Mapping (WinForms → Avalonia)

| WinForms Control | Avalonia Control | Notes |
|------------------|-----------------|-------|
| `Form` | `Window` or `UserControl` | Use Window for main app, UserControl for components |
| `TableLayoutPanel` | `Grid` with RowDefinitions/ColumnDefinitions | More flexible layout system |
| `SplitContainer` | `Grid` with `GridSplitter` | Add GridSplitter between rows/columns |
| `TabControl` | `TabControl` with `TabItem` | Similar API, use Header property |
| `MenuStrip` | `Menu` with `MenuItem` | Simplified menu structure |
| `StatusStrip` | `DockPanel` with `TextBlock` at bottom | Custom implementation needed |
| `ProgressBar` | `ProgressBar` | Direct equivalent |
| `Label` | `TextBlock` or `Label` | TextBlock for display, Label for form labels |
| `TextBox` | `TextBox` | Direct equivalent with additional properties |
| `Button` | `Button` | Direct equivalent |
| `ComboBox` | `ComboBox` | Direct equivalent |
| `DataGridView` | `DataGrid` | Direct equivalent with different column setup |
| `Panel` | `Panel`, `StackPanel`, or `Grid` | Choose based on layout needs |
| `GroupBox` | `Border` with `HeaderedContentControl` | Custom styling for grouping |
| `CheckBox` | `CheckBox` | Direct equivalent |
| `RadioButton` | `RadioButton` | Direct equivalent |
| `ListBox` | `ListBox` | Direct equivalent |
| `TreeView` | `TreeView` | Direct equivalent |

## MTM-Specific Control Hierarchies

### Component Hierarchy Mapping
When parsing MD files with component structures like:
```
Control_QuickButtons
├── quickButtons List<Button> (10 buttons maximum)
│   ├── Button[0] - Position 1: (Operation) - [PartID x Quantity]
│   └── Button[9] - Position 10: (Operation) - [PartID x Quantity]
└── Context Menu (Right-click)
    ├── Edit Button
    └── Remove Button
```

Generate Avalonia structure:
```xml
<ItemsControl ItemsSource="{Binding QuickButtons}">
    <ItemsControl.ItemsPanel>
        <ItemsPanelTemplate>
            <UniformGrid Rows="10" Columns="1"/>
        </ItemsPanelTemplate>
    </ItemsControl.ItemsPanel>
    <ItemsControl.ItemTemplate>
        <DataTemplate DataType="vm:QuickButtonItemViewModel">
            <Button Classes="quick-button">
                <Button.ContextMenu>
                    <ContextMenu>
                        <MenuItem Header="Edit Button"/>
                        <MenuItem Header="Remove Button"/>
                    </ContextMenu>
                </Button.ContextMenu>
            </Button>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

### MTM Data Patterns
Operations in MTM are typically numbers, not actions:
- **Part ID**: String (e.g., "PART001")
- **Operation**: String number (e.g., "90", "100", "110")
- **Quantity**: Integer
- **Position**: 1-based indexing for UI display

### Context Menu Integration
For components with management features, prefer context menus over separate buttons:
```xml
<Button.ContextMenu>
    <ContextMenu>
        <MenuItem Header="Edit Button" Command="{Binding EditCommand}"/>
        <MenuItem Header="Remove Button" Command="{Binding RemoveCommand}"/>
        <Separator/>
        <MenuItem Header="Clear All" Command="{Binding ClearAllCommand}"/>
        <MenuItem Header="Refresh" Command="{Binding RefreshCommand}"/>
    </ContextMenu>
</Button.ContextMenu>
```

## Modern UI Pattern Mappings

### Main Window Layout Pattern
Replace traditional WinForms with modern sidebar + content pattern:
```xml
<Grid ColumnDefinitions="240,*">
    <!-- Sidebar Navigation -->
    <Border Grid.Column="0" 
            Background="{DynamicResource SidebarBackgroundBrush}"
            BoxShadow="1 0 3 0 #22000000">
        <!-- Navigation content -->
    </Border>
    
    <!-- Main Content Area -->
    <Grid Grid.Column="1" Background="{DynamicResource ContentBackgroundBrush}">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <!-- Content Header -->
        <Border Grid.Row="0" Background="{DynamicResource CardBackgroundBrush}"/>
        
        <!-- Main Content -->
        <ScrollViewer Grid.Row="1" Padding="24">
            <ContentControl Content="{Binding CurrentView}"/>
        </ScrollViewer>
        
        <!-- Status Bar -->
        <Border Grid.Row="2" Background="{DynamicResource StatusBarBackgroundBrush}"/>
    </Grid>
</Grid>
```

### Card-Based Content Layout
Replace traditional panels with modern card design:
```xml
<!-- Feature Card Pattern -->
<Border Classes="card" Padding="24" Margin="0,0,0,16">
    <Grid RowDefinitions="Auto,16,Auto,24,*">
        <!-- Card Header with Icon -->
        <Grid Grid.Row="0" ColumnDefinitions="Auto,12,*">
            <PathIcon Grid.Column="0" 
                      Data="{StaticResource IconData}"
                      Width="24" Height="24"
                      Foreground="{DynamicResource AccentBrush}"/>
            <TextBlock Grid.Column="2" 
                       Text="Card Title"
                       FontSize="20"
                       FontWeight="SemiBold"/>
        </Grid>
        
        <!-- Card Description -->
        <TextBlock Grid.Row="2" 
                   Text="Card description text"
                   Opacity="0.8"
                   TextWrapping="Wrap"/>
        
        <!-- Card Content -->
        <Grid Grid.Row="4">
            <!-- Actual content -->
        </Grid>
    </Grid>
</Border>
```

### Navigation Sidebar Pattern
Replace traditional menus with modern navigation:
```xml
<!-- Expandable Navigation Group -->
<Expander Header="Inventory" IsExpanded="True">
    <StackPanel Spacing="2" Margin="24,4,0,4">
        <RadioButton GroupName="Navigation" 
                     Classes="nav-item"
                     Content="View Inventory"
                     Command="{Binding NavigateCommand}"
                     CommandParameter="Inventory"/>
        <RadioButton GroupName="Navigation"
                     Classes="nav-item" 
                     Content="Add Items"
                     Command="{Binding NavigateCommand}"
                     CommandParameter="AddItems"/>
    </StackPanel>
</Expander>
```

### Hero/Banner Section Pattern
Replace traditional headers with gradient banners:
```xml
<!-- Hero Banner with MTM Purple Gradient -->
<Border CornerRadius="12" 
        ClipToBounds="True"
        Height="200"
        Margin="0,0,0,24">
    <Border.Background>
        <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
            <GradientStop Color="#4574ED" Offset="0"/>
            <GradientStop Color="#4B45ED" Offset="0.3"/>
            <GradientStop Color="#8345ED" Offset="0.7"/>
            <GradientStop Color="#BA45ED" Offset="1"/>
        </LinearGradientBrush>
    </Border.Background>
    
    <Grid Margin="32">
        <StackPanel VerticalAlignment="Center" Spacing="8">
            <TextBlock Text="Welcome to MTM WIP System"
                       FontSize="28"
                       FontWeight="Bold"
                       Foreground="White"/>
            <TextBlock Text="Manage your inventory efficiently"
                       FontSize="16"
                       Foreground="White"
                       Opacity="0.9"/>
        </StackPanel>
    </Grid>
</Border>
```

## Screenshot-to-Instruction Relationships

### Settings View Control Screenshots

| Screenshot Filename                 | Instructions File                                                    |
|-------------------------------------|---------------------------------------------------------------------|
| Control_About.png                   | UI_Documentation/Controls/SettingsView/Control_About.instructions.md      |
| Control_AddItemType.png             | UI_Documentation/Controls/SettingsView/Control_ItemTypes.instructions.md  |
| Control_AddLocation.png             | UI_Documentation/Controls/SettingsView/Control_Locations.instructions.md  |
| Control_AddOperation.png            | UI_Documentation/Controls/SettingsView/Control_Operations.instructions.md |
| Control_AddPart.png                 | UI_Documentation/Controls/SettingsView/Control_PartNumbers.instructions.md|
| Control_AddUser.png                 | UI_Documentation/Controls/SettingsView/Control_Users.instructions.md      |
| Control_Database.png                | UI_Documentation/Controls/SettingsView/Control_Database.instructions.md   |
| Control_EditItemType.png            | UI_Documentation/Controls/SettingsView/Control_ItemTypes.instructions.md  |
| Control_EditLocation.png            | UI_Documentation/Controls/SettingsView/Control_Locations.instructions.md  |
| Control_EditOperation.png           | UI_Documentation/Controls/SettingsView/Control_Operations.instructions.md |
| Control_EditPart.png                | UI_Documentation/Controls/SettingsView/Control_PartNumbers.instructions.md|
| Control_EditUser.png                | UI_Documentation/Controls/SettingsView/Control_Users.instructions.md      |
| Control_RemoveLocation.png          | UI_Documentation/Controls/SettingsView/Control_Locations.instructions.md  |
| Control_RemoveOperation.png         | UI_Documentation/Controls/SettingsView/Control_Operations.instructions.md |
| Control_RemovePart.png              | UI_Documentation/Controls/SettingsView/Control_PartNumbers.instructions.md|
| Control_RemoveType.png              | UI_Documentation/Controls/SettingsView/Control_ItemTypes.instructions.md  |
| Control_RemoveUser.png              | UI_Documentation/Controls/SettingsView/Control_Users.instructions.md      |
| Control_Shortcuts.png               | UI_Documentation/Controls/SettingsView/Control_Shortcuts.instructions.md  |
| Control_Theme.png                   | UI_Documentation/Controls/SettingsView/Control_Theme.instructions.md      |

### MainView Controls Screenshots

| Screenshot Filename                                           | Instructions File                                                    |
|--------------------------------------------------------------|---------------------------------------------------------------------|
| Control_AdvancedInventory_Import.png                         | UI_Documentation/Controls/MainView/Control_AdvancedInventory.instructions.md |
| Control_AdvancedInventory_SingleItemMultipleLocations.png     | UI_Documentation/Controls/MainView/Control_AdvancedInventory.instructions.md |
| Control_AdvancedInventory_SingleItemMultipleTimes.png         | UI_Documentation/Controls/MainView/Control_AdvancedInventory.instructions.md |
| Control_AdvancedRemove.png                                   | UI_Documentation/Controls/MainView/Control_AdvancedRemove.instructions.md    |
| Control_InventoryTab.png                                     | UI_Documentation/Controls/MainView/Control_InventoryTab.instructions.md      |
| Control_RemoveTab.png                                        | UI_Documentation/Controls/MainView/Control_RemoveTab.instructions.md         |
| Control_TransferTab.png                                      | UI_Documentation/Controls/MainView/Control_TransferTab.instructions.md       |

### View Screenshots

| Screenshot Filename       | Instructions File                                                 |
|--------------------------|-------------------------------------------------------------------|
| View_ChangeButtonOrder.png| UI_Documentation/Views/ChangeButtonOrderView.instructions.md      |
| View_ChangeColumnOrder.png| UI_Documentation/Views/ChangeColumnOrderView.instructions.md      |
| View_Transactions.png     | UI_Documentation/Views/TransactionsView.instructions.md           |
| View_Settings.png         | UI_Documentation/Views/SettingsView.instructions.md               |

## Space Optimization Patterns
When removing UI elements, optimize space usage:
- Use `UniformGrid` for equal distribution
- Implement `VerticalAlignment="Stretch"` for full height usage
- Remove `ScrollViewer` when all items fit in available space
- Increase font sizes and padding when more space is available

## Event-Driven Communication Patterns
For inter-component communication described in MD files:
```csharp
// Events for parent-child communication
public event EventHandler<QuickActionExecutedEventArgs>? QuickActionExecuted;

// Fire events instead of direct method calls
QuickActionExecuted?.Invoke(this, new QuickActionExecutedEventArgs
{
    PartId = button.PartId,
    Operation = button.Operation,
    Quantity = button.Quantity
});
```

## Usage Guidelines

**How to use:**  
When creating a UI element, always refer to the mapped screenshot for layout and style, and the `.instructions.md` file for control/event details.

**Priority Rules:**
- If Markdown and screenshot disagree, prioritize the screenshot for layout
- Preserve control names/events from the Markdown
- Apply modern Avalonia patterns where appropriate
- Use MTM-specific data patterns and color scheme
- Implement context menus for management features
- Follow the WinForms → Avalonia control mapping table

> For complete UI generation guidelines, see [ui-generation.instruction.md](ui-generation.instruction.md).
> For control and event naming conventions, see [naming-conventions.instruction.md](naming-conventions.instruction.md)