<!-- markdownlint-disable-file -->
<!-- markdownlint-disable-file -->
# Task Research Notes: Better CollapsiblePanel Implementation Without TabControl

## Research Executed

### File Analysis
- c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\.github\Avalonia-Documentation\reference\controls\splitview.md
  - SplitView control with built-in expand/collapse functionality
- c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\.github\Avalonia-Documentation\reference\controls\expander.md  
  - Native Avalonia Expander control for collapsible content
- c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\.github\Avalonia-Documentation\reference\controls\contentcontrol.md
  - ContentControl base class for custom controls
- c:\Users\johnk\source\repos\MTM_WIP_Application_Avalonia\.github\Avalonia-Documentation\reference\controls\border.md
  - Border control for visual presentation

### Code Search Results
- #githubRepo:"AvaloniaUI/Avalonia" "Expander control implementation HeaderedContentControl"
  - Avalonia Expander inherits from HeaderedContentControl
  - Uses IsExpanded property with proper state management
  - Implements expand/collapse animations and events
- HeaderedContentControl patterns
  - Separates header and content presentation
  - Built-in template support with PART_HeaderPresenter
  - Clean MVVM integration

### External Research
- #githubRepo:"AvaloniaUI/Avalonia" Expander.cs source code analysis
  - Complete implementation of expand/collapse behavior
  - Proper pseudo-class management (:expanded)
  - Event handling (Expanding, Expanded, Collapsing, Collapsed)
  - Animation support via ContentTransition property
- Avalonia documentation on custom control types
  - UserControl vs TemplatedControl vs Custom-drawn approaches
  - HeaderedContentControl as optimal base for collapsible panels

### Project Conventions
- Standards referenced: Avalonia UI patterns, HeaderedContentControl inheritance
- Instructions followed: MTM styling requirements, Theme V2 integration

## Key Discoveries

### Optimal Base Class: HeaderedContentControl
The best approach for CollapsiblePanel is to inherit from `HeaderedContentControl` rather than using TabControl patterns:

```csharp
public class CollapsiblePanel : HeaderedContentControl
{
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsiblePanel, bool>(nameof(IsExpanded), true);
        
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
}
```

### Native Avalonia Expander Pattern
The Avalonia source code shows the ideal implementation pattern:

```csharp
[PseudoClasses(":expanded")]
public class Expander : HeaderedContentControl
{
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<Expander, bool>(
            nameof(IsExpanded),
            defaultBindingMode: BindingMode.TwoWay);
            
    // Event handling for expand/collapse
    public static readonly RoutedEvent<RoutedEventArgs> ExpandedEvent = ...;
    public static readonly RoutedEvent<RoutedEventArgs> CollapsedEvent = ...;
}
```

### Complete Template Structure
HeaderedContentControl provides clean template architecture:

```xml
<ControlTemplate TargetType="controls:CollapsiblePanel">
    <Border Classes="collapsible-root">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            
            <!-- Header with toggle button -->
            <Border Grid.Row="0" Classes="collapsible-header">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="Auto"/>
                        <ColumnDefinition Width="*"/>
                    </Grid.ColumnDefinitions>
                    
                    <Button Grid.Column="0" Classes="toggle-button" 
                            Command="{TemplateBinding ToggleCommand}">
                        <PathIcon Classes="chevron-icon"/>
                    </Button>
                    
                    <ContentPresenter Grid.Column="1" 
                                    Name="PART_HeaderPresenter"
                                    Content="{TemplateBinding Header}"/>
                </Grid>
            </Border>
            
            <!-- Content area -->
            <ContentPresenter Grid.Row="1" 
                            Name="PART_ContentPresenter"
                            Content="{TemplateBinding Content}"
                            IsVisible="{TemplateBinding IsExpanded}"/>
        </Grid>
    </Border>
</ControlTemplate>
```

### API and Schema Documentation
Key properties from HeaderedContentControl:
- `Header`: Object content for the header section
- `HeaderTemplate`: DataTemplate for header presentation  
- `Content`: Object content for the collapsible section
- `ContentTemplate`: DataTemplate for content presentation

Custom CollapsiblePanel properties:
- `IsExpanded`: Boolean for expand/collapse state
- `HeaderPosition`: Enum for header placement (Top, Bottom, Left, Right)

### Configuration Examples
Clean AXAML usage without TabControl dependencies:

```xml
<controls:CollapsiblePanel IsExpanded="True" HeaderPosition="Top">
    <controls:CollapsiblePanel.Header>
        <TextBlock Text="Quick Actions" FontWeight="Bold"/>
    </controls:CollapsiblePanel.Header>
    
    <!-- Content goes here -->
    <StackPanel>
        <Button Content="Action 1"/>
        <Button Content="Action 2"/>
    </StackPanel>
</controls:CollapsiblePanel>
```

### Technical Requirements
1. **Inherit from HeaderedContentControl**: Provides clean separation of header/content
2. **Implement IsExpanded property**: Controls visibility and state
3. **Add proper pseudo-classes**: :expanded for styling
4. **Use ControlTemplate**: Templated control approach for theming
5. **Remove TabControl dependencies**: Eliminates blue line issues

## Recommended Approach

### Complete Redesign Using HeaderedContentControl
Replace the current UserControl implementation with a proper TemplatedControl inheriting from HeaderedContentControl. This eliminates all TabControl-related styling conflicts.

### Implementation Strategy
1. **Create new CollapsiblePanel.cs**: Inherit from HeaderedContentControl
2. **Implement IsExpanded logic**: With proper change notifications
3. **Add HeaderPosition support**: For flexible layout options
4. **Create ControlTemplate**: In Generic.xaml or theme files
5. **Add pseudo-class styling**: :expanded state management

### Benefits of HeaderedContentControl Approach
- **No TabControl conflicts**: Eliminates blue line issue completely
- **Native Avalonia patterns**: Follows established framework conventions
- **Better theming support**: ControlTemplate approach allows complete restyling
- **Cleaner MVVM integration**: Proper dependency properties and binding
- **Animation support**: Can easily add expand/collapse transitions
- **Event handling**: Built-in support for state change events

## Implementation Guidance
- **Objectives**: Replace UserControl with HeaderedContentControl-based TemplatedControl
- **Key Tasks**: Create control class, implement template, add styling support
- **Dependencies**: HeaderedContentControl base class, ControlTemplate system
- **Success Criteria**: No TabControl inheritance, clean expand/collapse behavior, full theme integration
