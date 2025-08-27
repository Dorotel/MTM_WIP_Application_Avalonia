using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text.RegularExpressions;
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
/// Enhanced RichTextBox with MTM styling, formatting toolbar, and advanced features
/// </summary>
public partial class MTMRichTextBox : UserControl, INotifyPropertyChanged
{
    // Styled Properties
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<MTMRichTextBox, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<MTMRichTextBox, string>(nameof(PlaceholderText), "Enter text...");

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MTMRichTextBox, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<bool> ShowToolbarProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowToolbar), true);

    public static readonly StyledProperty<bool> ShowStatusBarProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowStatusBar), true);

    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowLineNumbers), false);

    public static readonly StyledProperty<bool> ShowWordCountProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowWordCount), true);

    public static readonly StyledProperty<bool> ShowSaveButtonProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowSaveButton), false);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(IsReadOnly), false);

    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        AvaloniaProperty.Register<MTMRichTextBox, FontFamily>(nameof(FontFamily), FontFamily.Default);

    public static readonly StyledProperty<double> FontSizeProperty =
        AvaloniaProperty.Register<MTMRichTextBox, double>(nameof(FontSize), 14.0);

    public static readonly StyledProperty<bool> HasValidationErrorProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(HasValidationError), false);

    public static readonly StyledProperty<string> ValidationMessageProperty =
        AvaloniaProperty.Register<MTMRichTextBox, string>(nameof(ValidationMessage), string.Empty);

    public static readonly StyledProperty<bool> ShowStatusIconProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(ShowStatusIcon), false);

    public static readonly StyledProperty<MaterialIconKind> StatusIconKindProperty =
        AvaloniaProperty.Register<MTMRichTextBox, MaterialIconKind>(nameof(StatusIconKind), MaterialIconKind.CheckCircle);

    public static readonly StyledProperty<IBrush> StatusIconColorProperty =
        AvaloniaProperty.Register<MTMRichTextBox, IBrush>(nameof(StatusIconColor), Brushes.Green);

    // Formatting Properties
    public static readonly StyledProperty<bool> IsBoldProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(IsBold), false);

    public static readonly StyledProperty<bool> IsItalicProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(IsItalic), false);

    public static readonly StyledProperty<bool> IsUnderlineProperty =
        AvaloniaProperty.Register<MTMRichTextBox, bool>(nameof(IsUnderline), false);

    // Properties
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
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

    public bool ShowToolbar
    {
        get => GetValue(ShowToolbarProperty);
        set => SetValue(ShowToolbarProperty, value);
    }

    public bool ShowStatusBar
    {
        get => GetValue(ShowStatusBarProperty);
        set => SetValue(ShowStatusBarProperty, value);
    }

    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    public bool ShowWordCount
    {
        get => GetValue(ShowWordCountProperty);
        set => SetValue(ShowWordCountProperty, value);
    }

    public bool ShowSaveButton
    {
        get => GetValue(ShowSaveButtonProperty);
        set => SetValue(ShowSaveButtonProperty, value);
    }

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public new FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
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

    public bool IsBold
    {
        get => GetValue(IsBoldProperty);
        set => SetValue(IsBoldProperty, value);
    }

    public bool IsItalic
    {
        get => GetValue(IsItalicProperty);
        set => SetValue(IsItalicProperty, value);
    }

    public bool IsUnderline
    {
        get => GetValue(IsUnderlineProperty);
        set => SetValue(IsUnderlineProperty, value);
    }

    // Computed Properties
    public string CharacterCountText => $"Characters: {Text?.Length ?? 0}";
    public string WordCountText => $"Words: {GetWordCount()}";
    public string CursorPositionText => $"Line: {GetCurrentLine()}, Col: {GetCurrentColumn()}";
    public ObservableCollection<int> LineNumbers { get; } = new();

    // Commands
    public ICommand ToggleBoldCommand { get; }
    public ICommand ToggleItalicCommand { get; }
    public ICommand ToggleUnderlineCommand { get; }
    public ICommand ToggleBulletListCommand { get; }
    public ICommand ToggleNumberListCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CutCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ExportHtmlCommand { get; }

    // Events
    public event EventHandler<string>? TextChanged;
    public event EventHandler? Saved;
    public new event PropertyChangedEventHandler? PropertyChanged;

    private TextBox? _textBox;
    private ScrollViewer? _lineNumberScroller;
    private ScrollViewer? _mainScroller;

    public MTMRichTextBox()
    {
        InitializeComponent();
        
        // Initialize commands
        ToggleBoldCommand = ReactiveCommand.Create(() => IsBold = !IsBold);
        ToggleItalicCommand = ReactiveCommand.Create(() => IsItalic = !IsItalic);
        ToggleUnderlineCommand = ReactiveCommand.Create(() => IsUnderline = !IsUnderline);
        ToggleBulletListCommand = ReactiveCommand.Create(ToggleBulletList);
        ToggleNumberListCommand = ReactiveCommand.Create(ToggleNumberList);
        SaveCommand = ReactiveCommand.Create(Save);
        CutCommand = ReactiveCommand.Create(Cut);
        CopyCommand = ReactiveCommand.Create(Copy);
        PasteCommand = ReactiveCommand.CreateFromTask(Paste);
        SelectAllCommand = ReactiveCommand.Create(SelectAll);
        ClearCommand = ReactiveCommand.Create(Clear);
        ExportHtmlCommand = ReactiveCommand.Create(ExportToHtml);
        
        // Subscribe to property changes
        this.GetObservable(TextProperty)
            .Subscribe(OnTextChanged);
            
        this.GetObservable(ShowLineNumbersProperty)
            .Subscribe(OnShowLineNumbersChanged);
    }

    protected override void OnApplyTemplate(Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Get references to template parts
        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _lineNumberScroller = e.NameScope.Find<ScrollViewer>("LineNumberScroller");
        _mainScroller = e.NameScope.Find<ScrollViewer>("MainScroller");
        
        if (_textBox != null)
        {
            _textBox.TextChanged += OnTextBoxTextChanged;
            _textBox.KeyDown += OnTextBoxKeyDown;
            
            // Set up key bindings
            _textBox.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.B, KeyModifiers.Control), Command = ToggleBoldCommand });
            _textBox.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.I, KeyModifiers.Control), Command = ToggleItalicCommand });
            _textBox.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.U, KeyModifiers.Control), Command = ToggleUnderlineCommand });
            _textBox.KeyBindings.Add(new KeyBinding { Gesture = new KeyGesture(Key.S, KeyModifiers.Control), Command = SaveCommand });
        }
        
        // Sync scroll viewers for line numbers
        if (_mainScroller != null && _lineNumberScroller != null)
        {
            _mainScroller.ScrollChanged += (s, e) =>
            {
                _lineNumberScroller.Offset = new Vector(_lineNumberScroller.Offset.X, e.OffsetDelta.Y);
            };
        }
        
        UpdateLineNumbers();
    }

    private void OnTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_textBox != null)
        {
            Text = _textBox.Text ?? string.Empty;
        }
        
        UpdateLineNumbers();
        UpdateStats();
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        // Handle special key combinations for formatting
        if (e.KeyModifiers == KeyModifiers.Control)
        {
            switch (e.Key)
            {
                case Key.B:
                    ToggleBoldCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.I:
                    ToggleItalicCommand.Execute(null);
                    e.Handled = true;
                    break;
                case Key.U:
                    ToggleUnderlineCommand.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
    }

    private void OnTextChanged(string newText)
    {
        UpdateLineNumbers();
        UpdateStats();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterCountText)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WordCountText)));
        TextChanged?.Invoke(this, newText);
    }

    private void OnShowLineNumbersChanged(bool showLineNumbers)
    {
        if (showLineNumbers)
        {
            UpdateLineNumbers();
        }
        else
        {
            LineNumbers.Clear();
        }
    }

    private void UpdateLineNumbers()
    {
        if (!ShowLineNumbers) return;
        
        var lines = Text?.Split('\n') ?? new[] { "" };
        
        // Update line numbers collection
        LineNumbers.Clear();
        for (int i = 1; i <= lines.Length; i++)
        {
            LineNumbers.Add(i);
        }
    }

    private void UpdateStats()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CharacterCountText)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WordCountText)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CursorPositionText)));
    }

    private int GetWordCount()
    {
        if (string.IsNullOrWhiteSpace(Text))
            return 0;
            
        return Regex.Matches(Text, @"\b\w+\b").Count;
    }

    private int GetCurrentLine()
    {
        if (_textBox == null || string.IsNullOrEmpty(Text))
            return 1;
            
        var cursorPosition = _textBox.CaretIndex;
        var textUpToCursor = Text.Substring(0, Math.Min(cursorPosition, Text.Length));
        return textUpToCursor.Count(c => c == '\n') + 1;
    }

    private int GetCurrentColumn()
    {
        if (_textBox == null || string.IsNullOrEmpty(Text))
            return 1;
            
        var cursorPosition = _textBox.CaretIndex;
        var textUpToCursor = Text.Substring(0, Math.Min(cursorPosition, Text.Length));
        var lastNewLine = textUpToCursor.LastIndexOf('\n');
        return lastNewLine == -1 ? cursorPosition + 1 : cursorPosition - lastNewLine;
    }

    private void ToggleBulletList()
    {
        // Simple bullet list implementation
        var selectedText = GetSelectedTextOrCurrentLine();
        var lines = selectedText.Split('\n');
        var result = string.Join('\n', lines.Select(line => 
            line.StartsWith("• ") ? line.Substring(2) : "• " + line));
        ReplaceSelectedText(result);
    }

    private void ToggleNumberList()
    {
        // Simple numbered list implementation
        var selectedText = GetSelectedTextOrCurrentLine();
        var lines = selectedText.Split('\n');
        var result = string.Join('\n', lines.Select((line, index) => 
            Regex.IsMatch(line, @"^\d+\. ") ? Regex.Replace(line, @"^\d+\. ", "") : $"{index + 1}. {line}"));
        ReplaceSelectedText(result);
    }

    private string GetSelectedTextOrCurrentLine()
    {
        if (_textBox?.SelectionEnd > _textBox?.SelectionStart)
        {
            return Text.Substring(_textBox.SelectionStart, _textBox.SelectionEnd - _textBox.SelectionStart);
        }
        
        // Get current line
        var currentLine = GetCurrentLine() - 1;
        var lines = Text.Split('\n');
        return currentLine < lines.Length ? lines[currentLine] : "";
    }

    private void ReplaceSelectedText(string replacement)
    {
        if (_textBox == null) return;
        
        var start = _textBox.SelectionStart;
        var length = _textBox.SelectionEnd - _textBox.SelectionStart;
        
        if (length == 0)
        {
            // Replace current line
            var currentLine = GetCurrentLine() - 1;
            var lines = Text.Split('\n').ToList();
            if (currentLine < lines.Count)
            {
                lines[currentLine] = replacement;
                Text = string.Join('\n', lines);
            }
        }
        else
        {
            // Replace selected text
            Text = Text.Remove(start, length).Insert(start, replacement);
        }
    }

    private void Save()
    {
        Saved?.Invoke(this, EventArgs.Empty);
        
        ShowStatusIcon = true;
        StatusIconKind = MaterialIconKind.CheckCircle;
        StatusIconColor = Brushes.Green;
    }

    private void Cut()
    {
        _textBox?.Cut();
    }

    private void Copy()
    {
        _textBox?.Copy();
    }

    private async System.Threading.Tasks.Task Paste()
    {
        _textBox?.Paste();
    }

    private void SelectAll()
    {
        _textBox?.SelectAll();
    }

    private void Clear()
    {
        Text = string.Empty;
    }

    private void ExportToHtml()
    {
        // Basic HTML export - enhanced with WebUtility instead of HttpUtility
        var encodedText = WebUtility.HtmlEncode(Text);
        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>{WebUtility.HtmlEncode(Label)}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .bold {{ font-weight: bold; }}
        .italic {{ font-style: italic; }}
        .underline {{ text-decoration: underline; }}
    </style>
</head>
<body>
    <pre>{encodedText}</pre>
</body>
</html>";
        
        // In a real implementation, you'd save this to a file
        // For now, just copy to clipboard
        TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(html);
    }

    /// <summary>
    /// Focus the text input
    /// </summary>
    public new void Focus()
    {
        _textBox?.Focus();
    }

    /// <summary>
    /// Set validation error
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
}
