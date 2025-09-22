# Custom Controls - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Pattern**: Custom Control Development  
**Created**: 2025-09-14  
**Updated**: 2025-09-21 (Phase 1 Material.Avalonia Integration)

---

## 📚 Comprehensive Avalonia Documentation Reference

**IMPORTANT**: This repository contains the complete Avalonia documentation straight from the official website in the `.github/Avalonia-Documentation/` folder. For custom control development:

- **Custom Controls Guide**: `.github/Avalonia-Documentation/guides/custom-controls/`
- **Control Development**: `.github/Avalonia-Documentation/guides/custom-controls/create-a-custom-control.md`
- **Styled Properties**: `.github/Avalonia-Documentation/guides/custom-controls/defining-properties.md`
- **Control Templates**: `.github/Avalonia-Documentation/guides/styles-and-resources/control-themes.md`
- **Advanced Controls**: `.github/Avalonia-Documentation/reference/controls/`

**Always reference the local Avalonia-Documentation folder for the most current and comprehensive control development guidance.**

---

## 🎯 Core Custom Control Patterns

### UserControl vs Control Base Classes

```csharp
// UserControl pattern - Composite controls with XAML UI
public partial class CollapsiblePanel : UserControl
{
    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<CollapsiblePanel, string>(nameof(Header), "");
    
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsiblePanel, bool>(nameof(IsExpanded), true);
    
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<CollapsiblePanel, object?>(nameof(Content));
    
    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }
    
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    
    public CollapsiblePanel()
    {
        InitializeComponent();
        SetupEventHandlers();
    }
    
    private void SetupEventHandlers()
    {
        // Property change notifications
        this.GetObservable(IsExpandedProperty)
            .Subscribe(isExpanded => UpdateExpandedState(isExpanded));
    }
    
    private void UpdateExpandedState(bool isExpanded)
    {
        // Update visual state based on expansion
        Classes.Set("expanded", isExpanded);
        Classes.Set("collapsed", !isExpanded);
        
        // Animate expansion/collapse
        if (this.FindControl<Border>("ContentBorder") is Border contentBorder)
        {
            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(250),
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0.0),
                        Setters = { new Setter(OpacityProperty, isExpanded ? 0.0 : 1.0) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters = { new Setter(OpacityProperty, isExpanded ? 1.0 : 0.0) }
                    }
                }
            };
            
            animation.RunAsync(contentBorder);
        }
    }
}
```

### Control Base Class Pattern - Lookless Controls

```csharp
// Control pattern - Lookless control with template support
public class TransactionExpandableButton : Button
{
    public static readonly StyledProperty<string> TransactionTypeProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(
            nameof(TransactionType), "");
    
    public static readonly StyledProperty<int> QuantityProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, int>(
            nameof(Quantity), 0);
    
    public static readonly StyledProperty<string> PartIdProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(
            nameof(PartId), "");
    
    public static readonly StyledProperty<bool> IsTransactionValidProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, bool>(
            nameof(IsTransactionValid), false);
    
    public string TransactionType
    {
        get => GetValue(TransactionTypeProperty);
        set => SetValue(TransactionTypeProperty, value);
    }
    
    public int Quantity
    {
        get => GetValue(QuantityProperty);
        set => SetValue(QuantityProperty, value);
    }
    
    public string PartId
    {
        get => GetValue(PartIdProperty);
        set => SetValue(PartIdProperty, value);
    }
    
    public bool IsTransactionValid
    {
        get => GetValue(IsTransactionValidProperty);
        set => SetValue(IsTransactionValidProperty, value);
    }
    
    static TransactionExpandableButton()
    {
        // Set default style key for template lookup
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TransactionExpandableButton), 
            new StyledPropertyMetadata<Type>(typeof(TransactionExpandableButton)));
    }
    
    public TransactionExpandableButton()
    {
        SetupPropertyNotifications();
    }
    
    private void SetupPropertyNotifications()
    {
        // Multi-property validation
        this.GetObservable(TransactionTypeProperty)
            .CombineLatest(
                this.GetObservable(QuantityProperty),
                this.GetObservable(PartIdProperty),
                (type, qty, partId) => ValidateTransaction(type, qty, partId))
            .Subscribe(isValid => IsTransactionValid = isValid);
    }
    
    private bool ValidateTransaction(string transactionType, int quantity, string partId)
    {
        // Manufacturing transaction validation
        return !string.IsNullOrWhiteSpace(transactionType) &&
               !string.IsNullOrWhiteSpace(partId) &&
               quantity > 0 &&
               IsValidTransactionType(transactionType) &&
               IsValidPartId(partId);
    }
    
    private bool IsValidTransactionType(string type)
    {
        return type is "IN" or "OUT" or "TRANSFER" or "ADJUSTMENT";
    }
    
    private bool IsValidPartId(string partId)
    {
        return !string.IsNullOrWhiteSpace(partId) &&
               partId.Length <= 50 &&
               System.Text.RegularExpressions.Regex.IsMatch(partId, @"^[A-Z0-9\-]+$");
    }
    
    protected override void OnClick()
    {
        if (IsTransactionValid)
        {
            // Execute manufacturing transaction
            ExecuteTransaction();
        }
        
        base.OnClick();
    }
    
    private void ExecuteTransaction()
    {
        // Manufacturing-specific transaction execution
        var transactionData = new
        {
            Type = TransactionType,
            PartId = PartId,
            Quantity = Quantity,
            Timestamp = DateTime.Now
        };
        
        // Raise custom routed event
        var args = new TransactionExecutedEventArgs(transactionData);
        RaiseEvent(args);
    }
}

// Custom event args for manufacturing transactions
public class TransactionExecutedEventArgs : RoutedEventArgs
{
    public object TransactionData { get; }
    
    public TransactionExecutedEventArgs(object transactionData)
    {
        TransactionData = transactionData;
        RoutedEvent = TransactionExpandableButton.TransactionExecutedEvent;
    }
}

// Add to TransactionExpandableButton class
public static readonly RoutedEvent<TransactionExecutedEventArgs> TransactionExecutedEvent =
    RoutedEvent.Register<TransactionExpandableButton, TransactionExecutedEventArgs>(
        nameof(TransactionExecuted), RoutingStrategies.Bubble);

public event EventHandler<TransactionExecutedEventArgs> TransactionExecuted
{
    add => AddHandler(TransactionExecutedEvent, value);
    remove => RemoveHandler(TransactionExecutedEvent, value);
}
```

---

## 🏭 Manufacturing-Specific Control Patterns

### Manufacturing DataGrid with Custom Columns

```csharp
// Custom DataGrid for manufacturing inventory display
public class ManufacturingInventoryGrid : DataGrid
{
    public static readonly StyledProperty<bool> ShowOperationColorsProperty =
        AvaloniaProperty.Register<ManufacturingInventoryGrid, bool>(
            nameof(ShowOperationColors), true);
    
    public static readonly StyledProperty<Dictionary<string, IBrush>> OperationColorMapProperty =
        AvaloniaProperty.Register<ManufacturingInventoryGrid, Dictionary<string, IBrush>>(
            nameof(OperationColorMap), CreateDefaultOperationColors());
    
    public bool ShowOperationColors
    {
        get => GetValue(ShowOperationColorsProperty);
        set => SetValue(ShowOperationColorsProperty, value);
    }
    
    public Dictionary<string, IBrush> OperationColorMap
    {
        get => GetValue(OperationColorMapProperty);
        set => SetValue(OperationColorMapProperty, value);
    }
    
    static ManufacturingInventoryGrid()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ManufacturingInventoryGrid), 
            new StyledPropertyMetadata<Type>(typeof(ManufacturingInventoryGrid)));
    }
    
    public ManufacturingInventoryGrid()
    {
        SetupManufacturingColumns();
        SetupEventHandlers();
    }
    
    private void SetupManufacturingColumns()
    {
        AutoGenerateColumns = false;
        
        Columns.Add(new DataGridTextColumn
        {
            Header = "Part ID",
            Binding = new Binding("PartId"),
            Width = new DataGridLength(120),
            IsReadOnly = true
        });
        
        Columns.Add(new ManufacturingOperationColumn
        {
            Header = "Operation",
            Binding = new Binding("Operation"),
            Width = new DataGridLength(100),
            OperationColors = OperationColorMap
        });
        
        Columns.Add(new DataGridTextColumn
        {
            Header = "Quantity",
            Binding = new Binding("Quantity", StringFormat = "N0"),
            Width = new DataGridLength(80)
        });
        
        Columns.Add(new DataGridTextColumn
        {
            Header = "Location", 
            Binding = new Binding("Location"),
            Width = new DataGridLength(100)
        });
        
        Columns.Add(new ManufacturingDateColumn
        {
            Header = "Last Updated",
            Binding = new Binding("LastUpdated"),
            Width = new DataGridLength(140)
        });
    }
    
    private void SetupEventHandlers()
    {
        SelectionChanged += OnSelectionChanged;
        LoadingRow += OnLoadingRow;
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Manufacturing-specific selection handling
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is InventoryItem selectedItem)
        {
            // Raise custom manufacturing selection event
            var args = new ManufacturingItemSelectedEventArgs(selectedItem);
            RaiseEvent(args);
        }
    }
    
    private void OnLoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (!ShowOperationColors) return;
        
        // Apply manufacturing operation-based row styling
        if (e.Row.DataContext is InventoryItem item)
        {
            ApplyOperationStyling(e.Row, item.Operation);
        }
    }
    
    private void ApplyOperationStyling(DataGridRow row, string operation)
    {
        if (OperationColorMap.TryGetValue(operation, out var brush))
        {
            // Apply subtle background color based on operation
            row.Background = new SolidColorBrush(((SolidColorBrush)brush).Color, 0.1);
        }
        
        // Apply operation-specific CSS classes
        row.Classes.Clear();
        row.Classes.Add($"operation-{operation}");
        
        switch (operation)
        {
            case "90": // Receiving
                row.Classes.Add("receiving-operation");
                break;
            case "100": // First operation
                row.Classes.Add("production-operation");
                break;
            case "110": // Second operation  
                row.Classes.Add("production-operation");
                break;
            case "120": // Final operation
                row.Classes.Add("final-operation");
                break;
            default:
                row.Classes.Add("custom-operation");
                break;
        }
    }
    
    private static Dictionary<string, IBrush> CreateDefaultOperationColors()
    {
        return new Dictionary<string, IBrush>
        {
            ["90"] = new SolidColorBrush(Colors.LightBlue),   // Receiving
            ["100"] = new SolidColorBrush(Colors.LightGreen), // First operation
            ["110"] = new SolidColorBrush(Colors.LightYellow), // Second operation  
            ["120"] = new SolidColorBrush(Colors.LightCoral)   // Final operation
        };
    }
}

// Custom column for manufacturing operations
public class ManufacturingOperationColumn : DataGridTextColumn
{
    public Dictionary<string, IBrush>? OperationColors { get; set; }
    
    protected override IControl GenerateElement(DataGridCell cell, object dataItem)
    {
        var textBlock = (TextBlock)base.GenerateElement(cell, dataItem);
        
        if (textBlock.Text is string operation && OperationColors?.TryGetValue(operation, out var brush) == true)
        {
            textBlock.Foreground = brush;
            textBlock.FontWeight = FontWeight.SemiBold;
        }
        
        return textBlock;
    }
}

// Custom column for manufacturing dates with relative time
public class ManufacturingDateColumn : DataGridTextColumn
{
    protected override IControl GenerateElement(DataGridCell cell, object dataItem)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Vertical };
        
        if (Binding?.Eval(dataItem) is DateTime dateTime)
        {
            // Absolute date/time
            var absoluteText = new TextBlock
            {
                Text = dateTime.ToString("yyyy-MM-dd HH:mm"),
                FontSize = 11
            };
            
            // Relative time
            var relativeText = new TextBlock
            {
                Text = FormatRelativeTime(dateTime),
                FontSize = 10,
                Opacity = 0.7
            };
            
            stackPanel.Children.Add(absoluteText);
            stackPanel.Children.Add(relativeText);
        }
        
        return stackPanel;
    }
    
    private string FormatRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        return timeSpan.TotalDays >= 1 
            ? $"{(int)timeSpan.TotalDays} days ago"
            : timeSpan.TotalHours >= 1
            ? $"{(int)timeSpan.TotalHours} hours ago"
            : $"{(int)timeSpan.TotalMinutes} minutes ago";
    }
}
```

### Manufacturing Quick Action Panel

```csharp
// Composite control for manufacturing quick actions
public partial class ManufacturingQuickActionPanel : UserControl
{
    public static readonly StyledProperty<IEnumerable> QuickActionsProperty =
        AvaloniaProperty.Register<ManufacturingQuickActionPanel, IEnumerable>(
            nameof(QuickActions));
    
    public static readonly StyledProperty<ICommand> ExecuteActionCommandProperty =
        AvaloniaProperty.Register<ManufacturingQuickActionPanel, ICommand>(
            nameof(ExecuteActionCommand));
    
    public static readonly StyledProperty<int> MaxVisibleActionsProperty =
        AvaloniaProperty.Register<ManufacturingQuickActionPanel, int>(
            nameof(MaxVisibleActions), 12);
    
    public IEnumerable QuickActions
    {
        get => GetValue(QuickActionsProperty);
        set => SetValue(QuickActionsProperty, value);
    }
    
    public ICommand ExecuteActionCommand
    {
        get => GetValue(ExecuteActionCommandProperty);
        set => SetValue(ExecuteActionCommandProperty, value);
    }
    
    public int MaxVisibleActions
    {
        get => GetValue(MaxVisibleActionsProperty);
        set => SetValue(MaxVisibleActionsProperty, value);
    }
    
    public ManufacturingQuickActionPanel()
    {
        InitializeComponent();
        SetupPropertyObservers();
    }
    
    private void SetupPropertyObservers()
    {
        this.GetObservable(QuickActionsProperty)
            .Subscribe(actions => UpdateQuickActionButtons());
        
        this.GetObservable(MaxVisibleActionsProperty)
            .Subscribe(_ => UpdateQuickActionButtons());
    }
    
    private void UpdateQuickActionButtons()
    {
        var buttonsPanel = this.FindControl<WrapPanel>("QuickActionsPanel");
        if (buttonsPanel == null) return;
        
        buttonsPanel.Children.Clear();
        
        if (QuickActions == null) return;
        
        var actions = QuickActions.Cast<object>().Take(MaxVisibleActions);
        
        foreach (var action in actions)
        {
            var button = CreateQuickActionButton(action);
            buttonsPanel.Children.Add(button);
        }
        
        // Add "More" button if there are additional actions
        var totalCount = QuickActions.Cast<object>().Count();
        if (totalCount > MaxVisibleActions)
        {
            var moreButton = CreateMoreActionsButton(totalCount - MaxVisibleActions);
            buttonsPanel.Children.Add(moreButton);
        }
    }
    
    private Button CreateQuickActionButton(object actionData)
    {
        var button = new Button
        {
            Classes = { "quick-action-button" },
            Margin = new Thickness(4),
            Padding = new Thickness(12, 8),
            Command = ExecuteActionCommand,
            CommandParameter = actionData
        };
        
        // Create button content based on action data
        if (actionData is QuickActionModel action)
        {
            var content = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            // Part ID display
            content.Children.Add(new TextBlock
            {
                Text = action.PartId,
                FontWeight = FontWeight.Bold,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            // Operation display with color coding
            var operationText = new TextBlock
            {
                Text = $"Op: {action.Operation}",
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            
            // Apply operation-specific styling
            switch (action.Operation)
            {
                case "90":
                    operationText.Foreground = Brushes.Blue;
                    break;
                case "100":
                    operationText.Foreground = Brushes.Green;
                    break;
                case "110":
                    operationText.Foreground = Brushes.Orange;
                    break;
                case "120":
                    operationText.Foreground = Brushes.Red;
                    break;
            }
            
            content.Children.Add(operationText);
            
            // Quantity display
            content.Children.Add(new TextBlock
            {
                Text = $"Qty: {action.Quantity:N0}",
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            button.Content = content;
        }
        
        return button;
    }
    
    private Button CreateMoreActionsButton(int additionalCount)
    {
        return new Button
        {
            Classes = { "more-actions-button" },
            Content = $"+{additionalCount} more",
            Margin = new Thickness(4),
            Padding = new Thickness(12, 8),
            FontStyle = FontStyle.Italic
        };
    }
}
```

---

## ❌ Anti-Patterns (Avoid These)

### Heavy Operations in Property Setters

```csharp
// ❌ WRONG: Expensive operations in property setters block UI
public class ExpensiveControl : UserControl
{
    public static readonly StyledProperty<string> DataSourceProperty =
        AvaloniaProperty.Register<ExpensiveControl, string>(nameof(DataSource));
    
    public string DataSource
    {
        get => GetValue(DataSourceProperty);
        set 
        {
            // BAD: Heavy operation on UI thread
            var data = DatabaseService.LoadLargeDataset(value).Result;
            ProcessData(data);
            
            // BAD: File I/O on property setter
            File.WriteAllText("cache.json", JsonSerializer.Serialize(data));
            
            SetValue(DataSourceProperty, value);
        }
    }
}

// ✅ CORRECT: Async operations with proper loading states
public class EfficientControl : UserControl
{
    public static readonly StyledProperty<string> DataSourceProperty =
        AvaloniaProperty.Register<EfficientControl, string>(nameof(DataSource));
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<EfficientControl, bool>(nameof(IsLoading));
    
    public string DataSource
    {
        get => GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public EfficientControl()
    {
        // GOOD: Observe property changes and handle async
        this.GetObservable(DataSourceProperty)
            .Where(source => !string.IsNullOrEmpty(source))
            .Subscribe(async source => await LoadDataAsync(source));
    }
    
    private async Task LoadDataAsync(string dataSource)
    {
        try
        {
            IsLoading = true;
            
            // GOOD: Async operation doesn't block UI
            var data = await DatabaseService.LoadLargeDatasetAsync(dataSource);
            
            // GOOD: Update UI on UI thread
            await Dispatcher.UIThread.InvokeAsync(() => ProcessData(data));
            
            // GOOD: Async file operation
            await File.WriteAllTextAsync("cache.json", JsonSerializer.Serialize(data));
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### Improper Dependency Property Implementation

```csharp
// ❌ WRONG: Improper dependency property patterns
public class BadPropertyControl : UserControl
{
    // BAD: Using regular property instead of styled property
    public string Title { get; set; } = "";
    
    // BAD: Not using proper property registration
    public static readonly AvaloniaProperty BadProperty = 
        AvaloniaProperty.Register(typeof(BadPropertyControl), "BadValue", typeof(object));
    
    // BAD: Inconsistent naming
    public static readonly StyledProperty<int> NumberProperty =
        AvaloniaProperty.Register<BadPropertyControl, int>("DifferentName");
    
    // BAD: No property wrapper
    // Missing: public int Number { get => ...; set => ...; }
}

// ✅ CORRECT: Proper styled property implementation
public class GoodPropertyControl : UserControl
{
    // GOOD: Proper styled property registration
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<GoodPropertyControl, string>(
            nameof(Title), 
            defaultValue: "",
            validate: ValidateTitle);
    
    public static readonly StyledProperty<int> NumberProperty =
        AvaloniaProperty.Register<GoodPropertyControl, int>(
            nameof(Number),
            defaultValue: 0,
            validate: ValidateNumber,
            coerce: CoerceNumber);
    
    // GOOD: Proper property wrappers
    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public int Number
    {
        get => GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }
    
    // GOOD: Property validation
    private static bool ValidateTitle(string title)
    {
        return title?.Length <= 100;
    }
    
    private static bool ValidateNumber(int number)
    {
        return number >= 0;
    }
    
    private static int CoerceNumber(IAvaloniaObject obj, int value)
    {
        return Math.Max(0, Math.Min(9999, value));
    }
}
```

### Memory Leaks from Event Subscriptions

```csharp
// ❌ WRONG: Event subscriptions without cleanup
public class LeakyControl : UserControl
{
    public LeakyControl()
    {
        InitializeComponent();
        
        // BAD: Static event subscription without cleanup
        SomeStaticService.DataChanged += OnDataChanged;
        
        // BAD: Timer without disposal
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timer.Tick += OnTimerTick;
        timer.Start();
    }
    
    // BAD: No cleanup - control will never be garbage collected
    private void OnDataChanged(object sender, EventArgs e) { }
    private void OnTimerTick(object sender, EventArgs e) { }
}

// ✅ CORRECT: Proper resource cleanup
public class ProperControl : UserControl, IDisposable
{
    private DispatcherTimer? _timer;
    private bool _disposed = false;
    
    public ProperControl()
    {
        InitializeComponent();
        
        SomeStaticService.DataChanged += OnDataChanged;
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        Dispose();
        base.OnDetachedFromVisualTree(e);
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            // GOOD: Clean up event subscriptions
            SomeStaticService.DataChanged -= OnDataChanged;
            
            // GOOD: Dispose timer
            _timer?.Stop();
            _timer = null;
            
            _disposed = true;
        }
    }
    
    private void OnDataChanged(object sender, EventArgs e) { }
    private void OnTimerTick(object sender, EventArgs e) { }
}
```

---

## 🔧 Manufacturing Control Troubleshooting

### Issue: Custom Control Not Applying Styles

**Symptoms**: Control renders with default appearance, custom styles ignored

**Solution**: Ensure proper DefaultStyleKey and resource registration

```csharp
// ✅ CORRECT: Proper style key registration
static TransactionExpandableButton()
{
    DefaultStyleKeyProperty.OverrideMetadata(typeof(TransactionExpandableButton), 
        new StyledPropertyMetadata<Type>(typeof(TransactionExpandableButton)));
}
```

```xml
<!-- Resources/Styles/CustomControls.axaml -->
<Style Selector="local|TransactionExpandableButton">
    <Setter Property="Template">
        <ControlTemplate>
            <Border Background="{TemplateBinding Background}"
                    BorderBrush="{TemplateBinding BorderBrush}"
                    BorderThickness="{TemplateBinding BorderThickness}">
                <ContentPresenter Content="{TemplateBinding Content}" />
            </Border>
        </ControlTemplate>
    </Setter>
</Style>
```

### Issue: Data Binding Not Working in Custom Controls

**Symptoms**: Bound values not updating in custom control properties

**Solution**: Implement proper property change notifications

```csharp
// ✅ CORRECT: Property change observation
public ManufacturingControl()
{
    // Observe property changes for updates
    this.GetObservable(DataProperty)
        .Subscribe(data => UpdateDisplay(data));
    
    // Multi-property observation
    this.GetObservable(PartIdProperty)
        .CombineLatest(this.GetObservable(OperationProperty), 
                      (partId, operation) => new { PartId = partId, Operation = operation })
        .Subscribe(combined => ValidateCombination(combined.PartId, combined.Operation));
}
```

### Issue: Performance Problems with Large Manufacturing Datasets

**Symptoms**: UI becomes sluggish when displaying many items

**Solution**: Implement virtualization and efficient rendering

```csharp
// ✅ CORRECT: Virtualized rendering for manufacturing data
public class VirtualizedManufacturingList : ListBox
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Enable virtualization for performance
        VirtualizationMode = ItemVirtualizationMode.Recycling;
        
        // Set reasonable item container recycling
        if (ItemsPanel is VirtualizingStackPanel panel)
        {
            panel.VirtualizationMode = ItemVirtualizationMode.Recycling;
            panel.IsVirtualizing = true;
        }
    }
    
    protected override IControl CreateContainerForItemOverride(object item, int index, object recycleKey)
    {
        // Efficient container creation/recycling
        var container = base.CreateContainerForItemOverride(item, index, recycleKey);
        
        // Apply manufacturing-specific styling efficiently
        if (container is ListBoxItem listBoxItem && item is InventoryItem inventoryItem)
        {
            listBoxItem.Classes.Set($"operation-{inventoryItem.Operation}", true);
        }
        
        return container;
    }
}
```

---

## 🧪 Custom Control Testing Patterns

### Unit Testing Custom Controls

```csharp
[TestFixture]
public class CollapsiblePanelTests
{
    private CollapsiblePanel _panel;
    
    [SetUp]
    public void SetUp()
    {
        _panel = new CollapsiblePanel
        {
            Header = "Test Panel",
            Content = new TextBlock { Text = "Test Content" }
        };
    }
    
    [Test]
    public void CollapsiblePanel_IsExpanded_True_ShowsContent()
    {
        // Arrange
        _panel.IsExpanded = true;
        
        // Act & Assert
        Assert.That(_panel.Classes.Contains("expanded"), Is.True);
        Assert.That(_panel.Classes.Contains("collapsed"), Is.False);
    }
    
    [Test]
    public void CollapsiblePanel_HeaderProperty_UpdatesDisplay()
    {
        // Arrange
        var expectedHeader = "Manufacturing Operations";
        
        // Act
        _panel.Header = expectedHeader;
        
        // Assert
        Assert.That(_panel.Header, Is.EqualTo(expectedHeader));
        
        // Verify UI update (if header TextBlock is accessible)
        if (_panel.FindControl<TextBlock>("HeaderTextBlock") is TextBlock headerText)
        {
            Assert.That(headerText.Text, Is.EqualTo(expectedHeader));
        }
    }
    
    [Test]
    public void TransactionExpandableButton_ValidTransaction_EnablesButton()
    {
        // Arrange
        var button = new TransactionExpandableButton
        {
            PartId = "PART001",
            Operation = "100", 
            Quantity = 10,
            TransactionType = "IN"
        };
        
        // Act & Assert
        Assert.That(button.IsTransactionValid, Is.True);
        Assert.That(button.IsEnabled, Is.True);
    }
    
    [TestCase("", "100", 10, "IN", false)]      // Empty PartId
    [TestCase("PART001", "", 10, "IN", false)]  // Empty Operation
    [TestCase("PART001", "100", 0, "IN", false)] // Zero Quantity
    [TestCase("PART001", "100", 10, "", false)]  // Empty TransactionType
    [TestCase("PART001", "100", 10, "INVALID", false)] // Invalid TransactionType
    [TestCase("PART001", "100", 10, "IN", true)] // Valid Transaction
    public void TransactionExpandableButton_ValidationScenarios_ReturnsExpectedResult(
        string partId, string operation, int quantity, string transactionType, bool expectedValid)
    {
        // Arrange
        var button = new TransactionExpandableButton
        {
            PartId = partId,
            Operation = operation,
            Quantity = quantity,
            TransactionType = transactionType
        };
        
        // Act & Assert
        Assert.That(button.IsTransactionValid, Is.EqualTo(expectedValid));
    }
}
```

### UI Integration Testing

```csharp
[TestFixture]
public class CustomControlIntegrationTests
{
    private Window _testWindow;
    
    [SetUp]
    public void SetUp()
    {
        _testWindow = new Window
        {
            Width = 800,
            Height = 600
        };
        
        _testWindow.Show();
    }
    
    [TearDown]
    public void TearDown()
    {
        _testWindow?.Close();
    }
    
    [Test]
    public async Task ManufacturingQuickActionPanel_ClickAction_ExecutesCommand()
    {
        // Arrange
        var commandExecuted = false;
        var command = ReactiveCommand.Create<object>(action =>
        {
            commandExecuted = true;
        });
        
        var quickActions = new List<QuickActionModel>
        {
            new() { PartId = "PART001", Operation = "100", Quantity = 5 }
        };
        
        var panel = new ManufacturingQuickActionPanel
        {
            QuickActions = quickActions,
            ExecuteActionCommand = command
        };
        
        _testWindow.Content = panel;
        
        // Allow UI to render
        await Task.Delay(100);
        
        // Act - Find and click the quick action button
        var button = panel.FindControl<Button>("QuickActionButton");
        button?.Command?.Execute(button.CommandParameter);
        
        // Assert
        Assert.That(commandExecuted, Is.True);
    }
}
```

---

## 🔗 AXAML Usage Examples

### CollapsiblePanel Usage

```xml
<controls:CollapsiblePanel Header="Manufacturing Operations" IsExpanded="True">
    <StackPanel Spacing="8">
        <TextBox Text="{Binding PartId}" Watermark="Part ID" />
        <ComboBox ItemsSource="{Binding Operations}" SelectedItem="{Binding SelectedOperation}" />
        <NumericUpDown Value="{Binding Quantity}" Minimum="1" />
    </StackPanel>
</controls:CollapsiblePanel>
```

### TransactionExpandableButton Usage

```xml
<controls:TransactionExpandableButton 
    PartId="{Binding PartId}"
    Operation="{Binding Operation}"
    Quantity="{Binding Quantity}" 
    TransactionType="{Binding TransactionType}"
    Command="{Binding ExecuteTransactionCommand}"
    TransactionExecuted="OnTransactionExecuted" />
```

### ManufacturingInventoryGrid Usage

```xml
<controls:ManufacturingInventoryGrid 
    ItemsSource="{Binding InventoryItems}"
    SelectedItem="{Binding SelectedInventoryItem}"
    ShowOperationColors="True"
    ManufacturingItemSelected="OnInventoryItemSelected" />
```

### ManufacturingQuickActionPanel Usage

```xml
<controls:ManufacturingQuickActionPanel 
    QuickActions="{Binding RecentTransactions}"
    ExecuteActionCommand="{Binding ExecuteQuickActionCommand}"
    MaxVisibleActions="12" />
```

---

## 📚 Related Documentation

- **Avalonia Behaviors**: [Behavior Implementation Patterns](./avalonia-behaviors.instructions.md)
- **Value Converters**: [Converter Creation Patterns](./value-converters.instructions.md)
- **UI Guidelines**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **MVVM Patterns**: [MVVM Community Toolkit](./mvvm-community-toolkit.instructions.md)

---

**Document Status**: ✅ Complete Custom Controls Reference  
**Framework Version**: Avalonia UI 11.3.4  
**Last Updated**: 2025-09-14  
**Custom Controls Owner**: MTM Development Team

## 🤖 Joyride Automation Integration

**PRIORITY**: Use Joyride automation whenever safe and possible for enhanced productivity and consistency.

### When to Use Joyride

**Always Use When Available:**

- File template generation and boilerplate creation
- Repetitive code pattern enforcement (MVVM, dependency injection)
- Automated testing workflows and validation
- VS Code workspace manipulation and configuration
- Interactive user input collection for complex decisions
- Real-time code validation and pattern checking

### Joyride Implementation Priorities

**High Priority (Use First):**

- `joyride_evaluate_code` - Execute ClojureScript for workspace automation
- `joyride_request_human_input` - Interactive decision-making workflows
- Dynamic file creation and template generation
- Automated pattern validation and enforcement

**Medium Priority (Use When Applicable):**

- VS Code API manipulation for UI automation
- Custom workflow creation for MTM-specific patterns
- Real-time validation of manufacturing domain rules
- Cross-platform testing automation

**Integration Benefits:**

- **Consistency**: Automated pattern enforcement reduces human error
- **Speed**: Bulk operations and template generation
- **Quality**: Real-time validation and compliance checking
- **Interactivity**: Human-in-the-loop for complex domain decisions

### MTM-Specific Joyride Applications

**Manufacturing Domain:**

- Automated validation of operation codes (90/100/110)
- Location code verification (FLOOR/RECEIVING/SHIPPING)
- Quick button configuration validation (max 10 per user)
- Session timeout and transaction logging automation

**Development Workflows:**

- MVVM Community Toolkit pattern enforcement
- Avalonia UI component generation following MTM standards
- MySQL stored procedure validation and testing
- Cross-platform build and deployment automation

**Quality Assurance:**

- Automated code review against MTM standards
- Theme system validation (17+ theme files)
- Database connection pooling configuration checks
- Security pattern enforcement (connection string encryption)

### Implementation Guidelines

1. **Safety First**: Always verify Joyride operations in development environment
2. **Fallback Ready**: Have traditional tool alternatives for critical operations
3. **User Feedback**: Use `joyride_request_human_input` for domain-critical decisions
4. **Incremental Adoption**: Start with low-risk automation and expand gradually
5. **Documentation**: Document custom Joyride workflows for team consistency

**Note**: Joyride enhances traditional development tools - use both together for maximum effectiveness.
