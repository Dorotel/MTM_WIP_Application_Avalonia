# Avalonia Behaviors - MTM WIP Application Instructions

**Framework**: Avalonia UI 11.3.4  
**Pattern**: Behavior-Driven UI Enhancement  
**Created**: 2025-09-14  

---

## 🎯 Core Avalonia Behavior Patterns

### Behavior Base Implementation
```csharp
// Standard behavior pattern for MTM application
public class MTMBehaviorBase<T> : Behavior<T> where T : Control
{
    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject != null)
        {
            AttachHandlers();
        }
    }
    
    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            DetachHandlers();
        }
        
        base.OnDetaching();
    }
    
    protected virtual void AttachHandlers()
    {
        // Override in derived classes
    }
    
    protected virtual void DetachHandlers()
    {
        // Override in derived classes for cleanup
    }
}
```

### AttachedProperty Implementation
```csharp
// Manufacturing-specific validation behavior with AttachedProperty
public class TextBoxFuzzyValidationBehavior : MTMBehaviorBase<TextBox>
{
    public static readonly StyledProperty<ObservableCollection<string>> ValidationRulesProperty =
        AvaloniaProperty.Register<TextBoxFuzzyValidationBehavior, ObservableCollection<string>>(
            nameof(ValidationRules), new ObservableCollection<string>());
    
    public static readonly StyledProperty<bool> IsValidProperty =
        AvaloniaProperty.Register<TextBoxFuzzyValidationBehavior, bool>(
            nameof(IsValid), true, defaultBindingMode: BindingMode.TwoWay);
    
    public ObservableCollection<string> ValidationRules
    {
        get => GetValue(ValidationRulesProperty);
        set => SetValue(ValidationRulesProperty, value);
    }
    
    public bool IsValid
    {
        get => GetValue(IsValidProperty);
        set => SetValue(IsValidProperty, value);
    }
    
    protected override void AttachHandlers()
    {
        AssociatedObject.TextChanged += OnTextChanged;
        AssociatedObject.LostFocus += OnLostFocus;
    }
    
    protected override void DetachHandlers()
    {
        AssociatedObject.TextChanged -= OnTextChanged;
        AssociatedObject.LostFocus -= OnLostFocus;
    }
    
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        ValidateInput(AssociatedObject.Text ?? string.Empty);
    }
    
    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        // Perform complete validation on focus loss
        ValidateInput(AssociatedObject.Text ?? string.Empty, isComplete: true);
    }
    
    private void ValidateInput(string input, bool isComplete = false)
    {
        if (ValidationRules?.Count == 0)
        {
            IsValid = true;
            return;
        }
        
        // Manufacturing part ID fuzzy matching
        var isValid = ValidationRules?.Any(rule => 
        {
            if (isComplete)
            {
                return string.Equals(input, rule, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return rule.StartsWith(input, StringComparison.OrdinalIgnoreCase);
            }
        }) ?? true;
        
        IsValid = isValid;
        
        // Update visual state
        UpdateVisualState();
    }
    
    private void UpdateVisualState()
    {
        if (AssociatedObject == null) return;
        
        // Manufacturing validation visual feedback
        AssociatedObject.Classes.Set("mtm-validation-error", !IsValid);
        AssociatedObject.Classes.Set("mtm-validation-valid", IsValid);
    }
}
```

---

## 🏭 Manufacturing-Specific Behavior Patterns

### ComboBox Navigation Enhancement for Manufacturing Operations
```csharp
// Based on ComboBoxBehavior.cs - Manufacturing operation selection enhancement
public class ManufacturingComboBoxBehavior : MTMBehaviorBase<ComboBox>
{
    public static readonly StyledProperty<bool> AllowCustomInputProperty =
        AvaloniaProperty.Register<ManufacturingComboBoxBehavior, bool>(
            nameof(AllowCustomInput), false);
    
    public static readonly StyledProperty<bool> FilterOnInputProperty =
        AvaloniaProperty.Register<ManufacturingComboBoxBehavior, bool>(
            nameof(FilterOnInput), true);
    
    public bool AllowCustomInput
    {
        get => GetValue(AllowCustomInputProperty);
        set => SetValue(AllowCustomInputProperty, value);
    }
    
    public bool FilterOnInput
    {
        get => GetValue(FilterOnInputProperty);
        set => SetValue(FilterOnInputProperty, value);
    }
    
    protected override void AttachHandlers()
    {
        if (AssociatedObject.IsKeyboardFocusWithin)
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }
        
        AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
        AssociatedObject.LostKeyboardFocus += OnLostKeyboardFocus;
        
        if (FilterOnInput)
        {
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }
    }
    
    protected override void DetachHandlers()
    {
        AssociatedObject.KeyDown -= OnKeyDown;
        AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
        AssociatedObject.LostKeyboardFocus -= OnLostKeyboardFocus;
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Manufacturing-specific navigation shortcuts
        switch (e.Key)
        {
            case Key.F1:
                // Quick select operation 90 (Receiving)
                SelectOperation("90");
                e.Handled = true;
                break;
            
            case Key.F2:
                // Quick select operation 100 (First Operation)
                SelectOperation("100");
                e.Handled = true;
                break;
                
            case Key.F3:
                // Quick select operation 110 (Second Operation)
                SelectOperation("110");
                e.Handled = true;
                break;
                
            case Key.F4:
                // Quick select operation 120 (Final Operation)
                SelectOperation("120");
                e.Handled = true;
                break;
                
            case Key.Enter:
                // Validate and accept current selection
                if (AllowCustomInput && !string.IsNullOrWhiteSpace(AssociatedObject.Text))
                {
                    ValidateCustomInput();
                }
                e.Handled = true;
                break;
        }
    }
    
    private void SelectOperation(string operationNumber)
    {
        if (AssociatedObject.Items?.Cast<object>().FirstOrDefault(item => 
            item.ToString() == operationNumber) is object operation)
        {
            AssociatedObject.SelectedItem = operation;
        }
    }
    
    private void ValidateCustomInput()
    {
        var customInput = AssociatedObject.Text ?? string.Empty;
        
        // Validate against manufacturing operation pattern
        if (IsValidOperationNumber(customInput))
        {
            // Add to items collection if valid and not already present
            if (AssociatedObject.Items?.Cast<object>().All(item => 
                item.ToString() != customInput) == true)
            {
                ((IList)AssociatedObject.Items).Add(customInput);
                AssociatedObject.SelectedItem = customInput;
            }
        }
        else
        {
            // Invalid input - revert to last valid selection
            AssociatedObject.Text = AssociatedObject.SelectedItem?.ToString() ?? string.Empty;
        }
    }
    
    private bool IsValidOperationNumber(string input)
    {
        // Manufacturing operation validation
        return !string.IsNullOrWhiteSpace(input) && 
               int.TryParse(input, out var number) && 
               number >= 90 && number <= 999 && 
               number % 10 == 0; // Operations are typically multiples of 10
    }
    
    private void OnGotKeyboardFocus(object? sender, GotFocusEventArgs e)
    {
        AssociatedObject.KeyDown += OnKeyDown;
    }
    
    private void OnLostKeyboardFocus(object? sender, RoutedEventArgs e)
    {
        AssociatedObject.KeyDown -= OnKeyDown;
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Manufacturing operation change logging
        if (e.AddedItems?.Count > 0 && e.AddedItems[0]?.ToString() is string selectedOperation)
        {
            LogOperationSelection(selectedOperation);
        }
    }
    
    private void LogOperationSelection(string operation)
    {
        // Integration with MTM logging system
        Console.WriteLine($"Manufacturing Operation Selected: {operation} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }
}
```

### AutoComplete Navigation for Manufacturing Part IDs
```csharp
// Based on AutoCompleteBoxNavigationBehavior.cs - Enhanced for manufacturing part lookup
public class ManufacturingAutoCompleteNavigationBehavior : MTMBehaviorBase<AutoCompleteBox>
{
    public static readonly StyledProperty<ICommand> PartSelectedCommandProperty =
        AvaloniaProperty.Register<ManufacturingAutoCompleteNavigationBehavior, ICommand>(
            nameof(PartSelectedCommand));
    
    public ICommand PartSelectedCommand
    {
        get => GetValue(PartSelectedCommandProperty);
        set => SetValue(PartSelectedCommandProperty, value);
    }
    
    protected override void AttachHandlers()
    {
        AssociatedObject.KeyDown += OnKeyDown;
        AssociatedObject.SelectionChanged += OnSelectionChanged;
        AssociatedObject.TextChanged += OnTextChanged;
    }
    
    protected override void DetachHandlers()
    {
        AssociatedObject.KeyDown -= OnKeyDown;
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
        AssociatedObject.TextChanged -= OnTextChanged;
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Tab:
            case Key.Enter:
                // Manufacturing part selection confirmation
                if (AssociatedObject.SelectedItem != null)
                {
                    ConfirmPartSelection();
                    e.Handled = true;
                }
                break;
                
            case Key.Escape:
                // Clear selection and close dropdown
                AssociatedObject.SelectedItem = null;
                AssociatedObject.Text = string.Empty;
                AssociatedObject.IsDropDownOpen = false;
                e.Handled = true;
                break;
                
            case Key.F5:
                // Refresh manufacturing part list
                RefreshPartsList();
                e.Handled = true;
                break;
        }
    }
    
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is string selectedPart)
        {
            // Manufacturing part validation
            ValidatePartId(selectedPart);
        }
    }
    
    private void OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var input = AssociatedObject.Text ?? string.Empty;
        
        // Manufacturing part ID formatting
        if (input.Length > 0)
        {
            // Auto-uppercase for manufacturing part IDs
            var formattedInput = input.ToUpperInvariant();
            if (formattedInput != input)
            {
                AssociatedObject.Text = formattedInput;
            }
        }
    }
    
    private void ConfirmPartSelection()
    {
        if (AssociatedObject.SelectedItem?.ToString() is string partId)
        {
            // Execute manufacturing part selection command
            if (PartSelectedCommand?.CanExecute(partId) == true)
            {
                PartSelectedCommand.Execute(partId);
            }
            
            // Close dropdown after selection
            AssociatedObject.IsDropDownOpen = false;
        }
    }
    
    private void ValidatePartId(string partId)
    {
        // Manufacturing part ID validation pattern
        var isValid = !string.IsNullOrWhiteSpace(partId) &&
                     partId.Length <= 50 &&
                     System.Text.RegularExpressions.Regex.IsMatch(partId, @"^[A-Z0-9\-]{1,50}$");
        
        // Update visual feedback
        AssociatedObject.Classes.Set("mtm-invalid-part", !isValid);
        AssociatedObject.Classes.Set("mtm-valid-part", isValid);
    }
    
    private void RefreshPartsList()
    {
        // Trigger refresh of manufacturing parts list
        // This would typically call a service to reload the ItemsSource
        Console.WriteLine("Refreshing manufacturing parts list...");
    }
}
```

---

## ❌ Anti-Patterns (Avoid These)

### Memory Leaks in Behaviors
```csharp
// ❌ WRONG: Not cleaning up event handlers leads to memory leaks
public class LeakyBehavior : Behavior<TextBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        
        // BAD: Event subscription without cleanup
        if (AssociatedObject != null)
        {
            AssociatedObject.TextChanged += OnTextChanged;
            
            // BAD: Static event subscription - will never be cleaned up
            SomeStaticEventPublisher.GlobalEvent += OnGlobalEvent;
        }
    }
    
    // BAD: No OnDetaching override - events never cleaned up
    
    private void OnTextChanged(object sender, TextChangedEventArgs e) { }
    private void OnGlobalEvent(object sender, EventArgs e) { }
}

// ✅ CORRECT: Proper event cleanup
public class ProperBehavior : MTMBehaviorBase<TextBox>
{
    protected override void AttachHandlers()
    {
        AssociatedObject.TextChanged += OnTextChanged;
        SomeStaticEventPublisher.GlobalEvent += OnGlobalEvent;
    }
    
    protected override void DetachHandlers()
    {
        AssociatedObject.TextChanged -= OnTextChanged;
        SomeStaticEventPublisher.GlobalEvent -= OnGlobalEvent;
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e) { }
    private void OnGlobalEvent(object sender, EventArgs e) { }
}
```

### Expensive Operations in Event Handlers
```csharp
// ❌ WRONG: Heavy operations on every keystroke
public class ExpensiveBehavior : MTMBehaviorBase<TextBox>
{
    protected override void AttachHandlers()
    {
        AssociatedObject.TextChanged += OnTextChanged;
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // BAD: Database call on every keystroke
        var parts = DatabaseService.SearchParts(AssociatedObject.Text).Result;
        
        // BAD: Complex regex validation on every character
        var isValid = ComplexValidationPattern.IsMatch(AssociatedObject.Text ?? string.Empty);
        
        // BAD: File I/O on UI thread
        File.WriteAllText("temp.txt", AssociatedObject.Text);
    }
}

// ✅ CORRECT: Debounced operations with async patterns
public class EfficientBehavior : MTMBehaviorBase<TextBox>
{
    private readonly Timer _debounceTimer;
    private readonly SemaphoreSlim _validationSemaphore = new(1, 1);
    
    public EfficientBehavior()
    {
        _debounceTimer = new Timer(PerformValidation, null, Timeout.Infinite, Timeout.Infinite);
    }
    
    protected override void AttachHandlers()
    {
        AssociatedObject.TextChanged += OnTextChanged;
    }
    
    protected override void DetachHandlers()
    {
        AssociatedObject.TextChanged -= OnTextChanged;
        _debounceTimer?.Dispose();
        _validationSemaphore?.Dispose();
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // GOOD: Debounce expensive operations
        _debounceTimer.Change(500, Timeout.Infinite); // Wait 500ms after last keystroke
    }
    
    private async void PerformValidation(object state)
    {
        if (!await _validationSemaphore.WaitAsync(100)) return;
        
        try
        {
            var text = await Dispatcher.UIThread.InvokeAsync(() => AssociatedObject?.Text ?? string.Empty);
            
            // GOOD: Async database operation
            var parts = await DatabaseService.SearchPartsAsync(text);
            
            // GOOD: Update UI on UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                UpdateSuggestions(parts);
            });
        }
        finally
        {
            _validationSemaphore.Release();
        }
    }
}
```

### Thread Safety Violations
```csharp
// ❌ WRONG: Modifying UI properties from background thread
public class ThreadUnsafeBehavior : MTMBehaviorBase<TextBox>
{
    protected override void AttachHandlers()
    {
        // BAD: Background operation that modifies UI
        Task.Run(() =>
        {
            Thread.Sleep(1000);
            // BAD: Cross-thread operation exception
            AssociatedObject.Text = "Updated from background thread";
            AssociatedObject.Background = Brushes.Red;
        });
    }
}

// ✅ CORRECT: Proper thread marshaling
public class ThreadSafeBehavior : MTMBehaviorBase<TextBox>
{
    protected override void AttachHandlers()
    {
        // GOOD: Background work with UI marshaling
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            
            // GOOD: Marshal UI updates to UI thread
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (AssociatedObject != null)
                {
                    AssociatedObject.Text = "Updated safely from background";
                    AssociatedObject.Background = Brushes.Green;
                }
            });
        });
    }
}
```

---

## 🔧 Manufacturing Behavior Troubleshooting

### Common Issues and Solutions

#### Issue: Behavior Not Attaching to Control
**Symptoms**: Event handlers not firing, behavior seems inactive

**Solution**: Check XAML namespace and behavior registration
```xml
<!-- ✅ CORRECT: Proper namespace and behavior attachment -->
<TextBox Text="{Binding PartId}">
    <i:Interaction.Behaviors>
        <behaviors:TextBoxFuzzyValidationBehavior 
            ValidationRules="{Binding PartIdValidationRules}"
            IsValid="{Binding IsPartIdValid, Mode=TwoWay}" />
    </i:Interaction.Behaviors>
</TextBox>
```

#### Issue: Memory Leaks from Behaviors
**Symptoms**: Application memory usage increases over time, especially with frequently created/destroyed controls

**Solution**: Implement proper cleanup in base behavior class
```csharp
// ✅ CORRECT: Base behavior with guaranteed cleanup
public abstract class MTMBehaviorBase<T> : Behavior<T>, IDisposable where T : Control
{
    private bool _disposed = false;
    
    protected override void OnDetaching()
    {
        Dispose();
        base.OnDetaching();
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            DetachHandlers();
            DisposeManagedResources();
            _disposed = true;
        }
    }
    
    protected virtual void DisposeManagedResources()
    {
        // Override in derived classes for additional cleanup
    }
}
```

#### Issue: Performance Degradation with Large Lists
**Symptoms**: UI becomes sluggish when working with manufacturing datasets

**Solution**: Implement virtualization and debouncing
```csharp
// ✅ CORRECT: Virtualized behavior for large manufacturing datasets
public class VirtualizedListBehavior : MTMBehaviorBase<ListBox>
{
    protected override void AttachHandlers()
    {
        // Enable virtualization for large manufacturing datasets
        AssociatedObject.VirtualizationMode = ItemVirtualizationMode.Recycling;
        
        // Implement selection debouncing for performance
        AssociatedObject.SelectionChanged += OnSelectionChangedDebounced;
    }
    
    private readonly Dictionary<object, Timer> _selectionTimers = new();
    
    private void OnSelectionChangedDebounced(object sender, SelectionChangedEventArgs e)
    {
        foreach (var item in e.AddedItems ?? Array.Empty<object>())
        {
            if (_selectionTimers.TryGetValue(item, out var existingTimer))
            {
                existingTimer.Dispose();
            }
            
            _selectionTimers[item] = new Timer(_ =>
            {
                Dispatcher.UIThread.Post(() => ProcessSelection(item));
                _selectionTimers.Remove(item);
            }, null, 300, Timeout.Infinite);
        }
    }
    
    private void ProcessSelection(object selectedItem)
    {
        // Process manufacturing item selection
        Console.WriteLine($"Manufacturing item selected: {selectedItem}");
    }
}
```

---

## 🧪 Behavior Testing Patterns

### Unit Testing Behaviors
```csharp
[TestFixture]
public class TextBoxFuzzyValidationBehaviorTests
{
    private TextBox _textBox;
    private TextBoxFuzzyValidationBehavior _behavior;
    
    [SetUp]
    public void SetUp()
    {
        _textBox = new TextBox();
        _behavior = new TextBoxFuzzyValidationBehavior
        {
            ValidationRules = new ObservableCollection<string> { "PART001", "PART002", "TEST001" }
        };
        
        _behavior.Attach(_textBox);
    }
    
    [TearDown]
    public void TearDown()
    {
        _behavior.Detach();
    }
    
    [Test]
    public void ValidationBehavior_ValidPartId_SetsIsValidTrue()
    {
        // Act
        _textBox.Text = "PART001";
        
        // Assert
        Assert.That(_behavior.IsValid, Is.True);
        Assert.That(_textBox.Classes.Contains("mtm-validation-valid"), Is.True);
        Assert.That(_textBox.Classes.Contains("mtm-validation-error"), Is.False);
    }
    
    [Test]
    public void ValidationBehavior_InvalidPartId_SetsIsValidFalse()
    {
        // Act
        _textBox.Text = "INVALID_PART";
        
        // Assert
        Assert.That(_behavior.IsValid, Is.False);
        Assert.That(_textBox.Classes.Contains("mtm-validation-error"), Is.True);
        Assert.That(_textBox.Classes.Contains("mtm-validation-valid"), Is.False);
    }
    
    [Test]
    public void ValidationBehavior_PartialMatch_AllowsTyping()
    {
        // Act - partial match should be valid during typing
        _textBox.Text = "PART";
        
        // Assert - should allow partial matches while typing
        Assert.That(_behavior.IsValid, Is.True);
    }
}
```

### Integration Testing with UI
```csharp
[TestFixture]
public class BehaviorIntegrationTests
{
    private Window _testWindow;
    
    [SetUp]
    public async Task SetUp()
    {
        _testWindow = new Window
        {
            Width = 400,
            Height = 300,
            Content = new StackPanel()
        };
        
        // Show window for UI testing
        _testWindow.Show();
        await Task.Delay(100); // Allow UI to render
    }
    
    [TearDown]
    public void TearDown()
    {
        _testWindow?.Close();
    }
    
    [Test]
    public async Task ManufacturingComboBox_F1Key_SelectsOperation90()
    {
        // Arrange
        var comboBox = new ComboBox
        {
            Items = new[] { "90", "100", "110", "120" }
        };
        
        var behavior = new ManufacturingComboBoxBehavior();
        behavior.Attach(comboBox);
        
        ((StackPanel)_testWindow.Content).Children.Add(comboBox);
        comboBox.Focus();
        
        // Act
        var keyEventArgs = new KeyEventArgs
        {
            Key = Key.F1,
            Route = RoutingStrategies.Bubble
        };
        
        comboBox.RaiseEvent(keyEventArgs);
        
        // Assert
        Assert.That(comboBox.SelectedItem?.ToString(), Is.EqualTo("90"));
    }
}
```

---

## 🔗 AXAML Usage Examples

### Basic Behavior Usage
```xml
<TextBox Text="{Binding PartId}" Watermark="Enter Part ID">
    <i:Interaction.Behaviors>
        <behaviors:TextBoxFuzzyValidationBehavior 
            ValidationRules="{Binding PartIdValidationRules}"
            IsValid="{Binding IsPartIdValid, Mode=TwoWay}" />
    </i:Interaction.Behaviors>
</TextBox>
```

### Advanced Manufacturing ComboBox
```xml
<ComboBox ItemsSource="{Binding Operations}" 
          SelectedItem="{Binding SelectedOperation}"
          Watermark="Select Operation">
    <i:Interaction.Behaviors>
        <behaviors:ManufacturingComboBoxBehavior 
            AllowCustomInput="True"
            FilterOnInput="True" />
    </i:Interaction.Behaviors>
</ComboBox>
```

### AutoComplete for Manufacturing Parts
```xml
<AutoCompleteBox ItemsSource="{Binding PartIds}"
                 SelectedItem="{Binding SelectedPartId}"
                 Text="{Binding PartIdText}"
                 Watermark="Search Parts...">
    <i:Interaction.Behaviors>
        <behaviors:ManufacturingAutoCompleteNavigationBehavior 
            PartSelectedCommand="{Binding SelectPartCommand}" />
    </i:Interaction.Behaviors>
</AutoCompleteBox>
```

---

## 📚 Related Documentation

- **Custom Controls**: [Custom Controls Implementation](./custom-controls.instructions.md)
- **MVVM Patterns**: [MVVM Community Toolkit](./mvvm-community-toolkit.instructions.md)
- **UI Guidelines**: [Avalonia UI Guidelines](./avalonia-ui-guidelines.instructions.md)
- **Value Converters**: [Value Converter Patterns](./value-converters.instructions.md)

---

**Document Status**: ✅ Complete Behavior Reference  
**Framework Version**: Avalonia UI 11.3.4  
**Last Updated**: 2025-09-14  
**Behavior Patterns Owner**: MTM Development Team