# MTM Accessibility Guidelines

## 📋 Overview

This document establishes comprehensive accessibility guidelines for the MTM WIP Application, ensuring compliance with WCAG 2.1 AA standards while optimizing the interface for manufacturing environments where users may have different abilities and work under various conditions.

## 🎯 **Accessibility Principles**

### **1. Perceivable - Information Must Be Presentable**

#### **Visual Accessibility**
```yaml
Color and Contrast:
  - Minimum contrast ratio 4.5:1 for normal text
  - Minimum contrast ratio 3:1 for large text (18pt+ or 14pt+ bold)
  - Minimum contrast ratio 3:1 for UI components and graphical objects
  - Never rely solely on color to convey information
  - Support Windows High Contrast mode

Text Readability:
  - Scalable fonts that work at 200% zoom
  - Clear typography with adequate spacing
  - Avoid italics in body text (harder to read)
  - Use sentence case instead of ALL CAPS
  - Maximum line length of 80 characters for readability
```

#### **Implementation Examples**
```xml
<!-- High Contrast Compatible Button -->
<Button x:Name="AccessibleButton"
        Content="Save Transaction"
        Classes="mtm-button-primary">
    
    <Button.Styles>
        <!-- Standard styling -->
        <Style Selector="Button.mtm-button-primary">
            <Setter Property="Background" Value="{DynamicResource MTM_Primary}" />
            <Setter Property="Foreground" Value="{DynamicResource MTM_TextOnPrimary}" />
        </Style>
        
        <!-- High contrast mode override -->
        <Style Selector="Button.mtm-button-primary">
            <Style.Triggers>
                <DataTrigger Binding="{x:Static SystemParameters.HighContrast}" Value="True">
                    <Setter Property="Background" Value="{x:Static SystemColors.ButtonFaceBrush}" />
                    <Setter Property="Foreground" Value="{x:Static SystemColors.ButtonTextBrush}" />
                    <Setter Property="BorderBrush" Value="{x:Static SystemColors.ButtonTextBrush}" />
                    <Setter Property="BorderThickness" Value="2" />
                </DataTrigger>
            </Style.Triggers>
        </Style>
    </Button.Styles>
</Button>

<!-- Accessible Status Indicators -->
<StackPanel Orientation="Horizontal" Spacing="8">
    <!-- Color + Icon + Text for status -->
    <Path Data="{StaticResource SuccessIconData}"
          Fill="{DynamicResource MTM_Success}"
          Width="16" Height="16"
          ToolTip.Tip="Success indicator" />
    <TextBlock Text="Operation Completed Successfully"
               Foreground="{DynamicResource MTM_Success}"
               FontWeight="Medium" />
</StackPanel>
```

### **2. Operable - Interface Must Be Usable**

#### **Keyboard Accessibility**
```yaml
Keyboard Navigation:
  - All interactive elements reachable via keyboard
  - Logical tab order following visual layout
  - Visible focus indicators on all focusable elements
  - No keyboard traps (user can navigate away from any element)
  - Keyboard shortcuts for common actions

Focus Management:
  - Focus indicators are clearly visible (2px outline minimum)
  - Focus moves predictably through the interface
  - Modal dialogs trap focus within the modal
  - Focus returns to trigger element when modals close
  - Skip links for repetitive navigation
```

#### **Keyboard Implementation**
```csharp
// Comprehensive keyboard handling for MTM components
public partial class MTMTextBox : MTMBaseUserControl
{
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        switch (e.Key)
        {
            case Key.Enter:
                // Submit form or move to next field
                if (AcceptsReturn)
                {
                    // Allow new line in multi-line text
                    return;
                }
                MoveToNextElement();
                e.Handled = true;
                break;
                
            case Key.Escape:
                // Clear field or cancel operation
                if (AllowEscapeClear && !string.IsNullOrEmpty(Text))
                {
                    Text = string.Empty;
                    e.Handled = true;
                }
                break;
                
            case Key.F1:
                // Show contextual help
                ShowFieldHelp();
                e.Handled = true;
                break;
        }
    }

    private void MoveToNextElement()
    {
        // Move focus to next tabbable element
        var request = new TraversalRequest(FocusNavigationDirection.Next);
        var currentElement = Keyboard.FocusedElement as UIElement;
        currentElement?.MoveFocus(request);
    }

    protected override void OnGotFocus(GotFocusEventArgs e)
    {
        base.OnGotFocus(e);
        
        // Announce field information to screen readers
        if (AutomationPeer.ListenerExists(AutomationEvents.AutomationFocusChanged))
        {
            var peer = UIElementAutomationPeer.FromElement(this);
            peer?.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
        }
    }
}

// Application-wide keyboard shortcuts
public class MTMKeyboardManager
{
    public static readonly Dictionary<KeyGesture, string> GlobalShortcuts = new()
    {
        { new KeyGesture(Key.S, KeyModifiers.Ctrl), "Save" },
        { new KeyGesture(Key.N, KeyModifiers.Ctrl), "New" },
        { new KeyGesture(Key.F, KeyModifiers.Ctrl), "Find" },
        { new KeyGesture(Key.H, KeyModifiers.Ctrl), "Help" },
        { new KeyGesture(Key.Escape), "Cancel" },
        { new KeyGesture(Key.F1), "Help" },
        { new KeyGesture(Key.F5), "Refresh" }
    };

    public static void RegisterGlobalShortcuts(Window window)
    {
        foreach (var shortcut in GlobalShortcuts)
        {
            var keyBinding = new KeyBinding
            {
                Gesture = shortcut.Key,
                Command = new RelayCommand(() => ExecuteShortcut(shortcut.Value))
            };
            window.KeyBindings.Add(keyBinding);
        }
    }

    private static void ExecuteShortcut(string action)
    {
        // Execute the appropriate action
        switch (action)
        {
            case "Save":
                // Trigger save command on current view
                break;
            case "New":
                // Trigger new item command
                break;
            // ... other shortcuts
        }
    }
}
```

#### **Touch and Mouse Accessibility**
```yaml
Touch Targets:
  - Minimum 44px x 44px for all interactive elements
  - Adequate spacing between adjacent targets (8px minimum)
  - Touch targets work with gloved hands in manufacturing environments
  - Support for both mouse and touch input simultaneously

Pointer Independence:
  - All functionality available via multiple input methods
  - No hover-only interactions (important for touch)
  - Click/tap targets clearly defined
  - Drag and drop operations have keyboard alternatives
```

### **3. Understandable - Information and UI Operation Must Be Clear**

#### **Content Clarity**
```yaml
Language and Reading Level:
  - Clear, simple language appropriate for manufacturing context
  - Technical terms explained or linked to glossary
  - Consistent terminology throughout the application
  - Error messages provide specific, actionable guidance

Predictable Interface:
  - Consistent navigation and layout patterns
  - Predictable component behavior
  - Changes in context are user-initiated or clearly announced
  - Form submission behavior is predictable and reversible
```

#### **Form Accessibility**
```xml
<!-- Accessible Form Implementation -->
<StackPanel Spacing="16">
    
    <!-- Form Instructions -->
    <TextBlock x:Name="FormInstructions"
               Text="Complete all required fields to process the inventory transaction. Required fields are marked with an asterisk (*)."
               FontSize="{DynamicResource MTM_FontSize_Body}"
               TextWrapping="Wrap"
               Margin="0,0,0,8" />
    
    <!-- Grouped Form Fields -->
    <Border Classes="mtm-card">
        <!-- Accessible fieldset equivalent -->
        <StackPanel Spacing="12">
            
            <!-- Section Heading -->
            <TextBlock Text="Part Information"
                       FontSize="{DynamicResource MTM_FontSize_H4}"
                       FontWeight="SemiBold"
                       AutomationProperties.HeadingLevel="2" />
            
            <!-- Required Field with Proper Labeling -->
            <StackPanel Spacing="4">
                <StackPanel Orientation="Horizontal" Spacing="4">
                    <TextBlock Text="Part ID"
                               FontWeight="Medium"
                               VerticalAlignment="Center" />
                    <TextBlock Text="*"
                               Foreground="{DynamicResource MTM_Danger}"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               AutomationProperties.Name="required" />
                </StackPanel>
                
                <TextBox x:Name="PartIdTextBox"
                         Text="{Binding PartId}"
                         AutomationProperties.Name="Part ID, required field"
                         AutomationProperties.HelpText="Enter the unique part identifier, for example PART001"
                         AutomationProperties.DescribedBy="{Binding #PartIdHelp}"
                         Classes="mtm-textbox" />
                
                <TextBlock x:Name="PartIdHelp"
                           Text="Format: PART followed by 3-6 digits (e.g., PART001)"
                           FontSize="{DynamicResource MTM_FontSize_Small}"
                           Foreground="{DynamicResource MTM_TextSecondary}" />
                
                <!-- Error Message -->
                <TextBlock x:Name="PartIdError"
                           Text="{Binding PartIdErrorMessage}"
                           FontSize="{DynamicResource MTM_FontSize_Small}"
                           Foreground="{DynamicResource MTM_Danger}"
                           IsVisible="{Binding HasPartIdError}"
                           AutomationProperties.LiveSetting="Assertive" />
            </StackPanel>
            
            <!-- Quantity Field with Input Constraints -->
            <StackPanel Spacing="4">
                <StackPanel Orientation="Horizontal" Spacing="4">
                    <TextBlock Text="Quantity"
                               FontWeight="Medium"
                               VerticalAlignment="Center" />
                    <TextBlock Text="*"
                               Foreground="{DynamicResource MTM_Danger}"
                               FontWeight="Bold"
                               VerticalAlignment="Center"
                               AutomationProperties.Name="required" />
                </StackPanel>
                
                <NumericUpDown x:Name="QuantityInput"
                               Value="{Binding Quantity}"
                               Minimum="1"
                               Maximum="9999"
                               Increment="1"
                               AutomationProperties.Name="Quantity, required field, numeric input"
                               AutomationProperties.HelpText="Enter quantity between 1 and 9999"
                               Classes="mtm-numeric" />
                
                <TextBlock Text="Enter a number between 1 and 9999"
                           FontSize="{DynamicResource MTM_FontSize_Small}"
                           Foreground="{DynamicResource MTM_TextSecondary}" />
            </StackPanel>
        </StackPanel>
    </Border>
    
    <!-- Form Actions with Clear Expectations -->
    <StackPanel Orientation="Horizontal" 
               HorizontalAlignment="Right"
               Spacing="12">
        <Button Content="Cancel"
               Classes="mtm-button-secondary"
               AutomationProperties.Name="Cancel transaction"
               AutomationProperties.HelpText="Discard changes and return to previous screen"
               Click="OnCancelClick" />
        
        <Button Content="Process Transaction"
               Classes="mtm-button-primary"
               AutomationProperties.Name="Process inventory transaction"
               AutomationProperties.HelpText="Submit the transaction for processing"
               IsEnabled="{Binding IsFormValid}"
               Click="OnSubmitClick" />
    </StackPanel>
</StackPanel>
```

### **4. Robust - Content Must Work with Assistive Technologies**

#### **Screen Reader Support**
```csharp
// Implementing proper AutomationPeer support
public class MTMTextBoxAutomationPeer : UserControlAutomationPeer
{
    private readonly MTMTextBox _owner;

    public MTMTextBoxAutomationPeer(MTMTextBox owner) : base(owner)
    {
        _owner = owner;
    }

    protected override string GetClassNameCore()
    {
        return "MTMTextBox";
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Edit;
    }

    protected override string GetNameCore()
    {
        // Combine label and required indicator
        var name = _owner.Label;
        if (_owner.IsRequired)
        {
            name += ", required field";
        }
        return name;
    }

    protected override string GetHelpTextCore()
    {
        return _owner.HelperText;
    }

    protected override bool IsKeyboardFocusableCore()
    {
        return _owner.IsEnabled && _owner.IsVisible;
    }

    protected override bool IsEnabledCore()
    {
        return _owner.IsEnabled;
    }

    protected override void SetFocusCore()
    {
        _owner.Focus();
    }

    // Announce state changes to screen readers
    public void AnnounceValidationState()
    {
        if (_owner.HasError)
        {
            RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}

// Usage in the component
protected override AutomationPeer OnCreateAutomationPeer()
{
    return new MTMTextBoxAutomationPeer(this);
}
```

## 🏭 **Manufacturing Environment Considerations**

### **Industrial Accessibility Challenges**

#### **Environmental Factors**
```yaml
Lighting Conditions:
  - Bright industrial lighting creates glare
  - Use high contrast colors and anti-glare design
  - Avoid pure white backgrounds in favor of off-white
  - Test visibility under various lighting conditions

Noise Levels:
  - Audio cues may not be effective in noisy environments
  - Rely primarily on visual feedback
  - Use vibration for mobile devices when available
  - Provide visual alternatives to all audio cues

Physical Constraints:
  - Users may wear safety gloves affecting touch precision
  - Safety equipment may limit vision or movement
  - Design for one-handed operation when possible
  - Larger touch targets and simplified interactions
```

#### **Manufacturing-Specific Accessibility Features**
```xml
<!-- Large Touch Targets for Gloved Hands -->
<Button Content="Emergency Stop"
        Width="80" Height="80"
        Background="{DynamicResource MTM_Danger}"
        Foreground="White"
        FontSize="14"
        FontWeight="Bold"
        CornerRadius="8"
        AutomationProperties.Name="Emergency stop button"
        AutomationProperties.HelpText="Press to immediately halt all operations" />

<!-- High Contrast Part Status Display -->
<Border Background="{DynamicResource MTM_Success}"
        BorderBrush="Black"
        BorderThickness="2"
        CornerRadius="4"
        Padding="12,6">
    <StackPanel Orientation="Horizontal" Spacing="8">
        <!-- Multiple indicators: color, icon, text -->
        <Path Data="{StaticResource CheckmarkIcon}"
              Fill="White"
              Width="16" Height="16" />
        <TextBlock Text="PART AVAILABLE"
                   Foreground="White"
                   FontWeight="Bold"
                   FontSize="14" />
        <TextBlock Text="Qty: 150"
                   Foreground="White"
                   FontFamily="{DynamicResource MTM_FontFamilyMono}"
                   FontSize="14" />
    </StackPanel>
</Border>

<!-- Voice-Friendly Part ID Display -->
<TextBlock Text="{Binding PartId}"
           FontFamily="{DynamicResource MTM_FontFamilyMono}"
           FontSize="18"
           FontWeight="Bold"
           AutomationProperties.Name="{Binding PartIdSpoken}"
           Background="{DynamicResource MTM_BackgroundAlt}"
           Padding="8,4" />
```

```csharp
// Converting part IDs to screen reader friendly format
public class PartIdConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string partId)
        {
            // Convert "PART001" to "Part 0 0 1" for better screen reader pronunciation
            return Regex.Replace(partId, @"(\w)(\d+)", match =>
            {
                var prefix = match.Groups[1].Value;
                var numbers = match.Groups[2].Value;
                var spokenNumbers = string.Join(" ", numbers.ToCharArray());
                return $"{prefix} {spokenNumbers}";
            });
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

## 🧪 **Accessibility Testing Guidelines**

### **Automated Testing**
```csharp
// Accessibility unit tests
[TestClass]
public class AccessibilityTests
{
    [TestMethod]
    public void MTMTextBox_ShouldHaveProperAutomationProperties()
    {
        // Arrange
        var textBox = new MTMTextBox
        {
            Label = "Part ID",
            IsRequired = true,
            HelperText = "Enter the part identifier"
        };

        // Act
        var peer = new MTMTextBoxAutomationPeer(textBox);

        // Assert
        Assert.AreEqual("Part ID, required field", peer.GetName());
        Assert.AreEqual("Enter the part identifier", peer.GetHelpText());
        Assert.AreEqual(AutomationControlType.Edit, peer.GetAutomationControlType());
    }

    [TestMethod]
    public void Button_ShouldHaveMinimumTouchTarget()
    {
        // Arrange
        var button = new Button { Content = "Save" };

        // Act
        button.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        // Assert - Minimum 44px touch target
        Assert.IsTrue(button.DesiredSize.Width >= 44 || button.MinWidth >= 44);
        Assert.IsTrue(button.DesiredSize.Height >= 44 || button.MinHeight >= 44);
    }

    [TestMethod]
    public void ColorContrast_ShouldMeetWCAGStandards()
    {
        // Test color contrast ratios
        var primaryColor = Color.Parse("#0078D4");
        var backgroundColor = Color.Parse("#FFFFFF");
        
        var contrastRatio = CalculateContrastRatio(primaryColor, backgroundColor);
        
        Assert.IsTrue(contrastRatio >= 4.5, $"Contrast ratio {contrastRatio} does not meet WCAG AA standard");
    }

    private double CalculateContrastRatio(Color foreground, Color background)
    {
        var l1 = GetRelativeLuminance(foreground);
        var l2 = GetRelativeLuminance(background);
        
        return (Math.Max(l1, l2) + 0.05) / (Math.Min(l1, l2) + 0.05);
    }
}
```

### **Manual Testing Checklist**
```yaml
Keyboard Navigation:
  - [ ] Tab order is logical and predictable
  - [ ] All interactive elements are keyboard accessible
  - [ ] Focus indicators are clearly visible
  - [ ] No keyboard traps exist
  - [ ] Skip links work properly

Screen Reader Testing:
  - [ ] All content is announced appropriately
  - [ ] Form labels are properly associated
  - [ ] Error messages are announced
  - [ ] Status changes are communicated
  - [ ] Heading structure is logical

Color and Contrast:
  - [ ] Interface works in high contrast mode
  - [ ] No information conveyed by color alone
  - [ ] Contrast ratios meet WCAG standards
  - [ ] Interface is usable at 200% zoom

Manufacturing Environment:
  - [ ] Touch targets are 44px minimum
  - [ ] Interface works with safety gloves
  - [ ] Visual feedback is clear under bright lights
  - [ ] One-handed operation is possible where appropriate
```

### **User Testing with Manufacturing Personnel**
```yaml
Test Scenarios:
  1. Part lookup with safety gloves on
  2. Transaction processing under time pressure
  3. Error recovery in noisy environment
  4. Interface use with protective eyewear
  5. One-handed operation scenarios

Feedback Collection:
  - Task completion times
  - Error rates and types
  - User satisfaction scores
  - Specific accessibility challenges
  - Suggestions for improvement

Documentation:
  - Record all accessibility issues found
  - Document solutions implemented
  - Create accessibility testing schedule
  - Maintain compliance checklist
```

## 📚 **Accessibility Resources and Training**

### **Development Team Resources**
```yaml
Guidelines and Standards:
  - WCAG 2.1 AA Compliance Checklist
  - Microsoft Accessibility Guidelines
  - Avalonia Accessibility Documentation
  - Manufacturing UX Best Practices

Testing Tools:
  - NVDA Screen Reader (free)
  - Windows Narrator
  - Accessibility Insights for Windows
  - Color Contrast Analyzers

Training Materials:
  - Accessibility fundamentals course
  - Screen reader usage training
  - Manufacturing environment considerations
  - Accessible design patterns workshop
```

### **User Documentation**
```yaml
Accessibility Features Guide:
  - Keyboard shortcuts reference
  - Screen reader compatibility notes
  - High contrast mode instructions
  - Font scaling recommendations

User Support:
  - Accessibility help desk contact
  - Video tutorials for assistive technology users
  - Alternative format documentation (large print, audio)
  - Regular accessibility feedback collection
```

This comprehensive accessibility framework ensures that the MTM WIP Application is usable by all manufacturing personnel, regardless of their abilities, while maintaining compliance with international accessibility standards and optimizing for the unique challenges of industrial environments.
