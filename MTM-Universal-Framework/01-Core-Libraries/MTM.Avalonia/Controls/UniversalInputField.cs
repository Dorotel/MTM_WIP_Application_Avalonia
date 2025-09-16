using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Input Field combining TextBox with label, validation, and help text.
    /// Provides consistent input experience across platforms with built-in validation styling.
    /// </summary>
    public class UniversalInputField : UserControl
    {
        public static readonly StyledProperty<string> LabelProperty =
            AvaloniaProperty.Register<UniversalInputField, string>(nameof(Label), string.Empty);

        public static readonly StyledProperty<string> ValueProperty =
            AvaloniaProperty.Register<UniversalInputField, string>(nameof(Value), string.Empty);

        public static readonly StyledProperty<string> PlaceholderProperty =
            AvaloniaProperty.Register<UniversalInputField, string>(nameof(Placeholder), string.Empty);

        public static readonly StyledProperty<string> HelpTextProperty =
            AvaloniaProperty.Register<UniversalInputField, string>(nameof(HelpText), string.Empty);

        public static readonly StyledProperty<string> ErrorTextProperty =
            AvaloniaProperty.Register<UniversalInputField, string>(nameof(ErrorText), string.Empty);

        public static readonly StyledProperty<bool> IsRequiredProperty =
            AvaloniaProperty.Register<UniversalInputField, bool>(nameof(IsRequired), false);

        public static readonly StyledProperty<bool> IsReadOnlyProperty =
            AvaloniaProperty.Register<UniversalInputField, bool>(nameof(IsReadOnly), false);

        public static readonly StyledProperty<bool> HasErrorProperty =
            AvaloniaProperty.Register<UniversalInputField, bool>(nameof(HasError), false);

        public static readonly StyledProperty<InputFieldType> FieldTypeProperty =
            AvaloniaProperty.Register<UniversalInputField, InputFieldType>(nameof(FieldType), InputFieldType.Text);

        private TextBox _textBox;
        private TextBlock _labelBlock;
        private TextBlock _helpTextBlock;
        private TextBlock _errorTextBlock;

        /// <summary>
        /// Label text displayed above the input
        /// </summary>
        public string Label
        {
            get => GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Current value of the input field
        /// </summary>
        public string Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Placeholder text shown when field is empty
        /// </summary>
        public string Placeholder
        {
            get => GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        /// <summary>
        /// Help text displayed below the input
        /// </summary>
        public string HelpText
        {
            get => GetValue(HelpTextProperty);
            set => SetValue(HelpTextProperty, value);
        }

        /// <summary>
        /// Error text displayed when validation fails
        /// </summary>
        public string ErrorText
        {
            get => GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        /// <summary>
        /// Whether this field is required
        /// </summary>
        public bool IsRequired
        {
            get => GetValue(IsRequiredProperty);
            set => SetValue(IsRequiredProperty, value);
        }

        /// <summary>
        /// Whether the field is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get => GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// Whether the field has a validation error
        /// </summary>
        public bool HasError
        {
            get => GetValue(HasErrorProperty);
            set => SetValue(HasErrorProperty, value);
        }

        /// <summary>
        /// Type of input field for specialized behavior
        /// </summary>
        public InputFieldType FieldType
        {
            get => GetValue(FieldTypeProperty);
            set => SetValue(FieldTypeProperty, value);
        }

        public UniversalInputField()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var stackPanel = new StackPanel
            {
                Spacing = 4
            };

            // Label
            _labelBlock = new TextBlock
            {
                FontWeight = FontWeight.SemiBold,
                IsVisible = false
            };
            stackPanel.Children.Add(_labelBlock);

            // Input field
            _textBox = new TextBox
            {
                Padding = new Thickness(12, 8),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6)
            };
            stackPanel.Children.Add(_textBox);

            // Help text
            _helpTextBlock = new TextBlock
            {
                FontSize = 12,
                Opacity = 0.7,
                IsVisible = false
            };
            stackPanel.Children.Add(_helpTextBlock);

            // Error text
            _errorTextBlock = new TextBlock
            {
                FontSize = 12,
                Foreground = Brushes.Red,
                IsVisible = false
            };
            stackPanel.Children.Add(_errorTextBlock);

            Content = stackPanel;

            // Bind properties
            UpdateDisplay();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == LabelProperty ||
                change.Property == ValueProperty ||
                change.Property == PlaceholderProperty ||
                change.Property == HelpTextProperty ||
                change.Property == ErrorTextProperty ||
                change.Property == IsRequiredProperty ||
                change.Property == IsReadOnlyProperty ||
                change.Property == HasErrorProperty ||
                change.Property == FieldTypeProperty)
            {
                UpdateDisplay();
            }
        }

        private void UpdateDisplay()
        {
            if (_labelBlock != null)
            {
                _labelBlock.Text = IsRequired ? $"{Label} *" : Label;
                _labelBlock.IsVisible = !string.IsNullOrEmpty(Label);
            }

            if (_textBox != null)
            {
                _textBox.Text = Value;
                _textBox.Watermark = Placeholder;
                _textBox.IsReadOnly = IsReadOnly;
                
                // Apply error styling
                if (HasError)
                {
                    _textBox.BorderBrush = Brushes.Red;
                    _textBox.Classes.Add("error");
                }
                else
                {
                    _textBox.BorderBrush = Brushes.Gray;
                    _textBox.Classes.Remove("error");
                }

                // Configure for field type
                switch (FieldType)
                {
                    case InputFieldType.Password:
                        _textBox.PasswordChar = '•';
                        break;
                    case InputFieldType.Email:
                        // Could add email validation here
                        break;
                    case InputFieldType.Number:
                        // Could restrict to numbers only
                        break;
                }
            }

            if (_helpTextBlock != null)
            {
                _helpTextBlock.Text = HelpText;
                _helpTextBlock.IsVisible = !string.IsNullOrEmpty(HelpText) && !HasError;
            }

            if (_errorTextBlock != null)
            {
                _errorTextBlock.Text = ErrorText;
                _errorTextBlock.IsVisible = HasError && !string.IsNullOrEmpty(ErrorText);
            }

            // Update field classes
            Classes.Set("required", IsRequired);
            Classes.Set("readonly", IsReadOnly);
            Classes.Set("error", HasError);
        }
    }

    /// <summary>
    /// Input field types for specialized behavior
    /// </summary>
    public enum InputFieldType
    {
        Text,
        Password,
        Email,
        Number,
        Phone,
        Url
    }
}