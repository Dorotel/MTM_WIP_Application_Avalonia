using System;
using System.ComponentModel;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// Enhanced TextBox with MTM styling, floating labels, validation, and advanced features
/// </summary>
public partial class MTMTextBox : UserControl, INotifyPropertyChanged
{
    // Styled Properties
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<MaterialIconKind> IconProperty =
        AvaloniaProperty.Register<MTMTextBox, MaterialIconKind>(nameof(Icon), MaterialIconKind.FilterVariant);

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(PlaceholderText), string.Empty);

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<bool> UseFloatingLabelProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(UseFloatingLabel), true);

    public static readonly StyledProperty<bool> ShowClearButtonProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(ShowClearButton), true);

    public static readonly StyledProperty<bool> ShowCopyButtonProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(ShowCopyButton), false);

    public static readonly StyledProperty<bool> ShowPasteButtonProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(ShowPasteButton), false);

    public static readonly StyledProperty<bool> ShowStatusIconProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(ShowStatusIcon), false);

    public static readonly StyledProperty<MaterialIconKind> StatusIconKindProperty =
        AvaloniaProperty.Register<MTMTextBox, MaterialIconKind>(nameof(StatusIconKind), MaterialIconKind.CheckCircle);

    public static readonly StyledProperty<IBrush> StatusIconColorProperty =
        AvaloniaProperty.Register<MTMTextBox, IBrush>(nameof(StatusIconColor), Brushes.Green);

    public static readonly StyledProperty<bool> HasValidationErrorProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(HasValidationError), false);

    public static readonly StyledProperty<string> ValidationMessageProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(ValidationMessage), string.Empty);

    public static readonly StyledProperty<string> HelperTextProperty =
        AvaloniaProperty.Register<MTMTextBox, string>(nameof(HelperText), string.Empty);

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(IsRequired), false);

    public static readonly StyledProperty<int> MaxLengthProperty =
        AvaloniaProperty.Register<MTMTextBox, int>(nameof(MaxLength), 0);

    public static readonly StyledProperty<bool> ShowCharacterCountProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(ShowCharacterCount), false);

    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(AcceptsReturn), false);

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<MTMTextBox, TextWrapping>(nameof(TextWrapping), TextWrapping.NoWrap);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<MTMTextBox, bool>(nameof(IsReadOnly), false);

    // Properties
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public MaterialIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public string PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool UseFloatingLabel
    {
        get => GetValue(UseFloatingLabelProperty);
        set => SetValue(UseFloatingLabelProperty, value);
    }

    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
    }

    public bool ShowCopyButton
    {
        get => GetValue(ShowCopyButtonProperty);
        set => SetValue(ShowCopyButtonProperty, value);
    }

    public bool ShowPasteButton
    {
        get => GetValue(ShowPasteButtonProperty);
        set => SetValue(ShowPasteButtonProperty, value);
    }

    public bool ShowStatusIcon
    {
        get => GetValue(ShowStatusIconProperty);
        set => SetValue(ShowStatusIconProperty, value);
    }

    public MaterialIconKind StatusIconKind
    {
        get => GetValue(StatusIconKindProperty);
        set => SetValue(StatusIconKindProperty, value);
    }

    public IBrush StatusIconColor
    {
        get => GetValue(StatusIconColorProperty);
        set => SetValue(StatusIconColorProperty, value);
    }

    public bool HasValidationError
    {
        get => GetValue(HasValidationErrorProperty);
        set => SetValue(HasValidationErrorProperty, value);
    }

    public string ValidationMessage
    {
        get => GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }

    public string HelperText
    {
        get => GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public int MaxLength
    {
        get => GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public bool ShowCharacterCount
    {
        get => GetValue(ShowCharacterCountProperty);
        set => SetValue(ShowCharacterCountProperty, value);
    }

    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    // Computed properties
    public string CharacterCountText => MaxLength > 0 ? $"{Text?.Length ?? 0}/{MaxLength}" : string.Empty;

    // Commands
    public ICommand ClearCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }

    // Events
    public event EventHandler<string>? TextChanged;
    public event EventHandler? Cleared;
    public event EventHandler? Copied;
    public event EventHandler? Pasted;
    public new event PropertyChangedEventHandler? PropertyChanged;

    private TextBox? _textBox;
    private TextBlock? _floatingLabel;
    private bool _isFocused;

    public MTMTextBox()
    {
        InitializeComponent();
        
        // Initialize commands
        ClearCommand = ReactiveCommand.Create(ClearText);
        CopyCommand = ReactiveCommand.Create(CopyText);
        PasteCommand = ReactiveCommand.CreateFromTask(PasteText);
        
        // Subscribe to property changes
        this.GetObservable(TextProperty)
            .Subscribe(OnTextChanged);
            
        this.GetObservable(HasValidationErrorProperty)
            .Subscribe(OnValidationStateChanged);
            
        this.GetObservable(MaxLengthProperty)
            .Subscribe(_ => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterCountText))));
    }

    protected override void OnApplyTemplate(Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Get references to template parts
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _floatingLabel = e.NameScope.Find<TextBlock>("FloatingLabel");
        
        if (_textBox != null)
        {
            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.GotFocus += OnTextBoxGotFocus;
            _textBox.LostFocus += OnTextBoxLostFocus;
        }
        
        UpdateFloatingLabel();
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_textBox != null)
        {
            Text = _textBox.Text ?? string.Empty;
        }
        
        UpdateFloatingLabel();
        ValidateInput();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterCountText)));
    }

    private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
    {
        _isFocused = true;
        UpdateFloatingLabel();
        
        if (_floatingLabel != null)
        {
            _floatingLabel.Classes.Add("focused");
        }
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        _isFocused = false;
        UpdateFloatingLabel();
        ValidateInput();
        
        if (_floatingLabel != null)
        {
            _floatingLabel.Classes.Remove("focused");
        }
    }

    private void OnTextChanged(string newText)
    {
        ValidateInput();
        UpdateFloatingLabel();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterCountText)));
        TextChanged?.Invoke(this, newText);
    }

    private void OnValidationStateChanged(bool hasError)
    {
        UpdateContainerState();
    }

    private void UpdateFloatingLabel()
    {
        if (!UseFloatingLabel || _floatingLabel == null)
            return;

        bool shouldFloat = _isFocused || !string.IsNullOrEmpty(Text);
        
        // Here you would implement the floating animation
        // For now, we'll just show/hide based on content
        _floatingLabel.IsVisible = UseFloatingLabel && (shouldFloat || string.IsNullOrEmpty(Text));
    }

    private void ValidateInput()
    {
        // Required field validation
        if (IsRequired && string.IsNullOrWhiteSpace(Text))
        {
            SetValidationError($"{Label} is required.");
            return;
        }

        // Max length validation
        if (MaxLength > 0 && Text?.Length > MaxLength)
        {
            SetValidationError($"{Label} cannot exceed {MaxLength} characters.");
            return;
        }

        // Clear validation if all checks pass
        ClearValidationError();
        
        // Set success state if text is present
        if (!string.IsNullOrEmpty(Text))
        {
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.CheckCircle;
            StatusIconColor = Brushes.Green;
        }
        else
        {
            ShowStatusIcon = false;
        }
    }

    private void UpdateContainerState()
    {
        var container = this.FindControl<Border>("TextBoxContainer");
        if (container != null)
        {
            container.Classes.Remove("error");
            container.Classes.Remove("success");
            container.Classes.Remove("warning");
            
            if (HasValidationError)
            {
                container.Classes.Add("error");
            }
            else if (!string.IsNullOrEmpty(Text))
            {
                container.Classes.Add("success");
            }
        }
    }

    private void ClearText()
    {
        Text = string.Empty;
        Cleared?.Invoke(this, EventArgs.Empty);
        _textBox?.Focus();
    }

    private void CopyText()
    {
        if (!string.IsNullOrEmpty(Text))
        {
            TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(Text);
            Copied?.Invoke(this, EventArgs.Empty);
        }
    }

    private async System.Threading.Tasks.Task PasteText()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard != null)
        {
            var text = await clipboard.GetTextAsync();
            if (!string.IsNullOrEmpty(text))
            {
                Text = text;
                Pasted?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Programmatically set validation error
    /// </summary>
    public void SetValidationError(string message)
    {
        HasValidationError = true;
        ValidationMessage = message;
        ShowStatusIcon = true;
        StatusIconKind = MaterialIconKind.AlertCircle;
        StatusIconColor = Brushes.Red;
    }

    /// <summary>
    /// Clear validation error
    /// </summary>
    public void ClearValidationError()
    {
        HasValidationError = false;
        ValidationMessage = string.Empty;
    }

    /// <summary>
    /// Set warning state
    /// </summary>
    public void SetWarning(string message)
    {
        HasValidationError = false;
        HelperText = message;
        ShowStatusIcon = true;
        StatusIconKind = MaterialIconKind.AlertOutline;
        StatusIconColor = Brushes.Orange;
        
        var container = this.FindControl<Border>("TextBoxContainer");
        container?.Classes.Add("warning");
    }

    /// <summary>
    /// Focus the text input
    /// </summary>
    public new void Focus()
    {
        _textBox?.Focus();
    }
}
