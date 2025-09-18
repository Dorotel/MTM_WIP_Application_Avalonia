using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Controls.ManufacturingFormField
{
    /// <summary>
    /// ManufacturingFormField - Desktop-optimized form field control for manufacturing data entry
    /// Features keyboard shortcuts, validation, and Windows clipboard integration
    /// Follows MTM design system and MVVM Community Toolkit patterns
    /// </summary>
    public partial class ManufacturingFormField : UserControl
    {
        #region Static Properties

        /// <summary>
        /// Label property for the field
        /// </summary>
        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<ManufacturingFormField, string>(nameof(Label), string.Empty);

        /// <summary>
        /// Field icon property
        /// </summary>
        public static readonly StyledProperty<string> FieldIconProperty =
            AvaloniaProperty.Register<ManufacturingFormField, string>(nameof(FieldIcon), "📝");

        /// <summary>
        /// Show icon property
        /// </summary>
        public static readonly StyledProperty<bool> ShowIconProperty =
            AvaloniaProperty.Register<ManufacturingFormField, bool>(nameof(ShowIcon), true);

        /// <summary>
        /// Is required property
        /// </summary>
        public static readonly StyledProperty<bool> IsRequiredProperty =
            AvaloniaProperty.Register<ManufacturingFormField, bool>(nameof(IsRequired), false);

        /// <summary>
        /// Input control property
        /// </summary>
        public static readonly StyledProperty<Control?> InputControlProperty =
            AvaloniaProperty.Register<ManufacturingFormField, Control?>(nameof(InputControl));

        /// <summary>
        /// Validation message property
        /// </summary>
        public static readonly StyledProperty<string> ValidationMessageProperty =
            AvaloniaProperty.Register<ManufacturingFormField, string>(nameof(ValidationMessage), string.Empty);

        /// <summary>
        /// Validation message type property (error, success, info)
        /// </summary>
        public static readonly StyledProperty<string> ValidationTypeProperty =
            AvaloniaProperty.Register<ManufacturingFormField, string>(nameof(ValidationType), "info");

        /// <summary>
        /// Shortcut hint property
        /// </summary>
        public static readonly StyledProperty<string> ShortcutHintProperty =
            AvaloniaProperty.Register<ManufacturingFormField, string>(nameof(ShortcutHint), string.Empty);

        /// <summary>
        /// Show shortcut hint property
        /// </summary>
        public static readonly StyledProperty<bool> ShowShortcutHintProperty =
            AvaloniaProperty.Register<ManufacturingFormField, bool>(nameof(ShowShortcutHint), false);

        /// <summary>
        /// Field type property for manufacturing intelligence
        /// </summary>
        public static readonly StyledProperty<ManufacturingFieldType> FieldTypeProperty =
            AvaloniaProperty.Register<ManufacturingFormField, ManufacturingFieldType>(nameof(FieldType), ManufacturingFieldType.Text);

        /// <summary>
        /// Enable keyboard shortcuts property
        /// </summary>
        public static readonly StyledProperty<bool> EnableKeyboardShortcutsProperty =
            AvaloniaProperty.Register<ManufacturingFormField, bool>(nameof(EnableKeyboardShortcuts), true);

        #endregion

        #region Properties

        /// <summary>
        /// Label for the field
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Field icon
        /// </summary>
        public string FieldIcon
        {
            get => GetValue(FieldIconProperty);
            set => SetValue(FieldIconProperty, value);
        }

        /// <summary>
        /// Show icon
        /// </summary>
        public bool ShowIcon
        {
            get => GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// Is required field
        /// </summary>
        public bool IsRequired
        {
            get => GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        /// <summary>
        /// Input control
        /// </summary>
        public Control? InputControl
        {
            get => GetValue(InputControlProperty);
            set => SetValue(InputControlProperty, value);
        }

        /// <summary>
        /// Validation message
        /// </summary>
        public string ValidationMessage
        {
            get => GetValue(ValidationMessageProperty);
            set => SetValue(ValidationMessageProperty, value);
        }

        /// <summary>
        /// Validation message type
        /// </summary>
        public string ValidationType
        {
            get => GetValue(ValidationTypeProperty);
            set => SetValue(ValidationTypeProperty, value);
        }

        /// <summary>
        /// Shortcut hint text
        /// </summary>
        public string ShortcutHint
        {
            get => GetValue(ShortcutHintProperty);
            set => SetValue(ShortcutHintProperty, value);
        }

        /// <summary>
        /// Show shortcut hint
        /// </summary>
        public bool ShowShortcutHint
        {
            get => GetValue(ShowShortcutHintProperty);
            set => SetValue(ShowShortcutHintProperty, value);
        }

        /// <summary>
        /// Field type for manufacturing intelligence
        /// </summary>
        public ManufacturingFieldType FieldType
        {
            get => GetValue(FieldTypeProperty);
            set => SetValue(FieldTypeProperty, value);
        }

        /// <summary>
        /// Enable keyboard shortcuts
        /// </summary>
        public bool EnableKeyboardShortcuts
        {
            get => GetValue(EnableKeyboardShortcutsProperty);
            set => SetValue(EnableKeyboardShortcutsProperty, value);
        }

        /// <summary>
        /// Has validation message
        /// </summary>
        public bool HasValidationMessage => !string.IsNullOrWhiteSpace(ValidationMessage);

        #endregion

        #region Events

        /// <summary>
        /// Event fired when field value changes
        /// </summary>
        public event EventHandler<FieldValueChangedEventArgs>? FieldValueChanged;

        /// <summary>
        /// Event fired when keyboard shortcut is pressed
        /// </summary>
        public event EventHandler<KeyboardShortcutEventArgs>? KeyboardShortcutPressed;

        /// <summary>
        /// Event fired when field validation changes
        /// </summary>
        public event EventHandler<ValidationChangedEventArgs>? ValidationChanged;

        #endregion

        #region Private Fields

        private readonly ILogger<ManufacturingFormField> _logger;
        private bool _isInitialized = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of ManufacturingFormField
        /// </summary>
        public ManufacturingFormField()
        {
            // Use null logger for now - can be injected later if needed
            _logger = Microsoft.Extensions.Logging.Abstractions.NullLogger<ManufacturingFormField>.Instance;

            InitializeComponent();
            
            _isInitialized = true;
            _logger.LogDebug("ManufacturingFormField initialized");
        }

        #endregion

        #region Property Change Handlers

        /// <summary>
        /// Handles input control changes
        /// </summary>
        private void HandleInputControlChanged()
        {
            if (!_isInitialized) return;
            OnInputControlChanged();
        }

        /// <summary>
        /// Handles validation changes
        /// </summary>
        private void HandleValidationChanged()
        {
            if (!_isInitialized) return;
            OnValidationChanged();
        }

        /// <summary>
        /// Handles field type changes
        /// </summary>
        private void HandleFieldTypeChanged()
        {
            if (!_isInitialized) return;
            OnFieldTypeChanged();
        }

        /// <summary>
        /// Handles input control change
        /// </summary>
        private void OnInputControlChanged()
        {
            if (InputControl == null) return;

            // Apply manufacturing styling
            if (!InputControl.Classes.Contains("manufacturing-input"))
            {
                InputControl.Classes.Add("manufacturing-input");
            }

            // Subscribe to events based on control type
            SetupInputControlEvents();

            // Setup keyboard shortcuts if enabled
            if (EnableKeyboardShortcuts)
            {
                SetupKeyboardShortcuts();
            }

            _logger.LogDebug("Input control changed to {ControlType}", InputControl.GetType().Name);
        }

        /// <summary>
        /// Handles validation change
        /// </summary>
        private void OnValidationChanged()
        {
            if (InputControl == null) return;

            // Remove existing validation classes
            InputControl.Classes.Remove("error");
            InputControl.Classes.Remove("success");
            InputControl.Classes.Remove("info");

            // Add validation class based on type
            if (!string.IsNullOrWhiteSpace(ValidationMessage))
            {
                InputControl.Classes.Add(ValidationType.ToLowerInvariant());
            }

            // Update validation message styling
            var validationElements = this.GetLogicalDescendants().OfType<TextBlock>()
                .Where(t => t.Classes.Contains("validation-message"));
            foreach (var validationMessageElement in validationElements)
            {
                validationMessageElement.Classes.Clear();
                validationMessageElement.Classes.Add("validation-message");
                if (!string.IsNullOrWhiteSpace(ValidationType))
                {
                    validationMessageElement.Classes.Add(ValidationType.ToLowerInvariant());
                }
            }

            // Fire validation changed event
            ValidationChanged?.Invoke(this, new ValidationChangedEventArgs(ValidationMessage, ValidationType));
        }

        /// <summary>
        /// Handles field type change
        /// </summary>
        private void OnFieldTypeChanged()
        {
            // Update icon based on field type
            FieldIcon = FieldType switch
            {
                ManufacturingFieldType.PartId => "🔗",
                ManufacturingFieldType.Operation => "⚙️",
                ManufacturingFieldType.Quantity => "#️⃣",
                ManufacturingFieldType.Location => "📍",
                ManufacturingFieldType.User => "👤",
                ManufacturingFieldType.Date => "📅",
                ManufacturingFieldType.Notes => "📝",
                _ => "📝"
            };

            // Update shortcut hint based on field type
            if (ShowShortcutHint && string.IsNullOrWhiteSpace(ShortcutHint))
            {
                ShortcutHint = FieldType switch
                {
                    ManufacturingFieldType.PartId => "F2: Barcode",
                    ManufacturingFieldType.Operation => "F1-F4: Quick Ops",
                    ManufacturingFieldType.Quantity => "Ctrl+Up/Down",
                    ManufacturingFieldType.Location => "F5: Last Used",
                    _ => string.Empty
                };
            }
        }

        #endregion

        #region Input Control Setup

        /// <summary>
        /// Sets up events for the input control
        /// </summary>
        private void SetupInputControlEvents()
        {
            if (InputControl == null) return;

            // Common events for all input controls
            InputControl.LostFocus += OnInputLostFocus;
            InputControl.GotFocus += OnInputGotFocus;

            // Control-specific events
            switch (InputControl)
            {
                case TextBox textBox:
                    textBox.TextChanged += OnTextChanged;
                    break;
                case AutoCompleteBox autoComplete:
                    autoComplete.TextChanged += OnAutoCompleteTextChanged;
                    break;
                case ComboBox comboBox:
                    comboBox.SelectionChanged += OnComboBoxSelectionChanged;
                    break;
                case NumericUpDown numericUpDown:
                    numericUpDown.ValueChanged += OnNumericValueChanged;
                    break;
            }
        }

        /// <summary>
        /// Sets up keyboard shortcuts
        /// </summary>
        private void SetupKeyboardShortcuts()
        {
            if (InputControl == null) return;

            InputControl.KeyDown += OnKeyDown;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles input focus gained
        /// </summary>
        private void OnInputGotFocus(object? sender, GotFocusEventArgs e)
        {
            _logger.LogDebug("Input control gained focus for field: {Label}", Label);
        }

        /// <summary>
        /// Handles input focus lost
        /// </summary>
        private void OnInputLostFocus(object? sender, RoutedEventArgs e)
        {
            _logger.LogDebug("Input control lost focus for field: {Label}", Label);
        }

        /// <summary>
        /// Handles text change
        /// </summary>
        private void OnTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                FieldValueChanged?.Invoke(this, new FieldValueChangedEventArgs(textBox.Text, FieldType));
            }
        }

        /// <summary>
        /// Handles auto complete text change
        /// </summary>
        private void OnAutoCompleteTextChanged(object? sender, EventArgs e)
        {
            if (sender is AutoCompleteBox autoComplete)
            {
                FieldValueChanged?.Invoke(this, new FieldValueChangedEventArgs(autoComplete.Text, FieldType));
            }
        }

        /// <summary>
        /// Handles combo box selection change
        /// </summary>
        private void OnComboBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                FieldValueChanged?.Invoke(this, new FieldValueChangedEventArgs(comboBox.SelectedItem?.ToString(), FieldType));
            }
        }

        /// <summary>
        /// Handles numeric value change
        /// </summary>
        private void OnNumericValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
        {
            if (sender is NumericUpDown numericUpDown)
            {
                FieldValueChanged?.Invoke(this, new FieldValueChangedEventArgs(numericUpDown.Value?.ToString(), FieldType));
            }
        }

        /// <summary>
        /// Handles key down for shortcuts
        /// </summary>
        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (!EnableKeyboardShortcuts) return;

            var handled = false;

            // Manufacturing-specific shortcuts
            switch (FieldType)
            {
                case ManufacturingFieldType.Operation:
                    handled = HandleOperationShortcuts(e);
                    break;
                case ManufacturingFieldType.PartId:
                    handled = HandlePartIdShortcuts(e);
                    break;
                case ManufacturingFieldType.Quantity:
                    handled = HandleQuantityShortcuts(e);
                    break;
                case ManufacturingFieldType.Location:
                    handled = HandleLocationShortcuts(e);
                    break;
            }

            // Common shortcuts
            if (!handled)
            {
                handled = HandleCommonShortcuts(e);
            }

            if (handled)
            {
                e.Handled = true;
                KeyboardShortcutPressed?.Invoke(this, new KeyboardShortcutEventArgs(e.Key, e.KeyModifiers, FieldType));
            }
        }

        #endregion

        #region Keyboard Shortcut Handlers

        /// <summary>
        /// Handles operation field shortcuts (F1-F4 for 90-120)
        /// </summary>
        private bool HandleOperationShortcuts(KeyEventArgs e)
        {
            if (InputControl is not TextBox textBox) return false;

            switch (e.Key)
            {
                case Key.F1:
                    textBox.Text = "90";
                    return true;
                case Key.F2:
                    textBox.Text = "100";
                    return true;
                case Key.F3:
                    textBox.Text = "110";
                    return true;
                case Key.F4:
                    textBox.Text = "120";
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Handles part ID field shortcuts
        /// </summary>
        private bool HandlePartIdShortcuts(KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                // F2: Simulate barcode scan (could integrate with actual barcode scanner)
                _logger.LogDebug("Barcode shortcut pressed for Part ID field");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles quantity field shortcuts
        /// </summary>
        private bool HandleQuantityShortcuts(KeyEventArgs e)
        {
            if (InputControl is NumericUpDown numeric && e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.Up:
                        numeric.Value = (numeric.Value ?? 0) + 10;
                        return true;
                    case Key.Down:
                        numeric.Value = Math.Max(0, (numeric.Value ?? 0) - 10);
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles location field shortcuts
        /// </summary>
        private bool HandleLocationShortcuts(KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                // F5: Use last used location (placeholder for future implementation)
                _logger.LogDebug("Last location shortcut pressed");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles common shortcuts for all fields
        /// </summary>
        private bool HandleCommonShortcuts(KeyEventArgs e)
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                switch (e.Key)
                {
                    case Key.V:
                        // Enhanced paste handling (future: smart format detection)
                        _logger.LogDebug("Enhanced paste shortcut for field: {Label}", Label);
                        return false; // Let default paste handle it for now
                    case Key.Z:
                        // Undo (future implementation)
                        _logger.LogDebug("Undo shortcut for field: {Label}", Label);
                        return false;
                }
            }

            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Focuses the input control
        /// </summary>
        public void FocusInput()
        {
            InputControl?.Focus();
        }

        /// <summary>
        /// Clears the validation message
        /// </summary>
        public void ClearValidation()
        {
            ValidationMessage = string.Empty;
            ValidationType = "info";
        }

        /// <summary>
        /// Sets validation message
        /// </summary>
        public void SetValidation(string message, string type = "error")
        {
            ValidationMessage = message;
            ValidationType = type;
        }

        /// <summary>
        /// Gets the current field value
        /// </summary>
        public object? GetFieldValue()
        {
            return InputControl switch
            {
                TextBox textBox => textBox.Text,
                AutoCompleteBox autoComplete => autoComplete.Text,
                ComboBox comboBox => comboBox.SelectedItem,
                NumericUpDown numeric => numeric.Value,
                _ => null
            };
        }

        /// <summary>
        /// Sets the field value
        /// </summary>
        public void SetFieldValue(object? value)
        {
            switch (InputControl)
            {
                case TextBox textBox:
                    textBox.Text = value?.ToString() ?? string.Empty;
                    break;
                case AutoCompleteBox autoComplete:
                    autoComplete.Text = value?.ToString() ?? string.Empty;
                    break;
                case ComboBox comboBox:
                    comboBox.SelectedItem = value;
                    break;
                case NumericUpDown numeric:
                    if (value is decimal decimalValue)
                        numeric.Value = decimalValue;
                    else if (decimal.TryParse(value?.ToString(), out var parsedValue))
                        numeric.Value = parsedValue;
                    break;
            }
        }

        #endregion
    }

    #region Enums and Event Args

    /// <summary>
    /// Manufacturing field types
    /// </summary>
    public enum ManufacturingFieldType
    {
        Text,
        PartId,
        Operation,
        Quantity,
        Location,
        User,
        Date,
        Notes
    }

    /// <summary>
    /// Field value changed event arguments
    /// </summary>
    public class FieldValueChangedEventArgs : EventArgs
    {
        public object? Value { get; }
        public ManufacturingFieldType FieldType { get; }

        public FieldValueChangedEventArgs(object? value, ManufacturingFieldType fieldType)
        {
            Value = value;
            FieldType = fieldType;
        }
    }

    /// <summary>
    /// Keyboard shortcut pressed event arguments
    /// </summary>
    public class KeyboardShortcutEventArgs : EventArgs
    {
        public Key Key { get; }
        public KeyModifiers Modifiers { get; }
        public ManufacturingFieldType FieldType { get; }

        public KeyboardShortcutEventArgs(Key key, KeyModifiers modifiers, ManufacturingFieldType fieldType)
        {
            Key = key;
            Modifiers = modifiers;
            FieldType = fieldType;
        }
    }

    /// <summary>
    /// Validation changed event arguments
    /// </summary>
    public class ValidationChangedEventArgs : EventArgs
    {
        public string Message { get; }
        public string Type { get; }

        public ValidationChangedEventArgs(string message, string type)
        {
            Message = message;
            Type = type;
        }
    }

    #endregion
}