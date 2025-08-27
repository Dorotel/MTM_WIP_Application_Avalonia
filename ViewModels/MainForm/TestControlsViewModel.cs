using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Windows.Input;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for testing MTM custom controls with comprehensive null safety
/// </summary>
public sealed class TestControlsViewModel : ReactiveObject, INotifyPropertyChanged
{
    #region Private Fields with Null Safety

    private string _label = "Test Label";
    private string _placeholderText = "Enter test text...";
    private string _text = string.Empty;
    private string _helperText = "This is helper text";
    private string _validationMessage = string.Empty;
    private MaterialIconKind _icon = MaterialIconKind.FilterVariant;
    private bool _isEnabled = true;
    private bool _isReadOnly = false;
    private bool _hasValidationError = false;
    private bool _showClearButton = true;
    private bool _showCopyButton = false;
    private bool _showPasteButton = false;
    private bool _showStatusIcon = false;
    private bool _showCharacterCount = false;
    private bool _useFloatingLabel = false;
    private int _maxLength = 0;
    private MaterialIconKind _statusIconKind = MaterialIconKind.CheckCircle;
    private IBrush _statusIconColor = Brushes.Green;

    #endregion

    #region Properties with Null Safety and Fallbacks

    /// <summary>
    /// Label text for the control with null safety
    /// </summary>
    public string Label
    {
        get => _label ?? "Test Label";
        set => this.RaiseAndSetIfChanged(ref _label, value ?? "Test Label");
    }

    /// <summary>
    /// Placeholder text with null safety
    /// </summary>
    public string PlaceholderText
    {
        get => _placeholderText ?? "Enter test text...";
        set => this.RaiseAndSetIfChanged(ref _placeholderText, value ?? "Enter test text...");
    }

    /// <summary>
    /// Main text content with null safety
    /// </summary>
    public string Text
    {
        get => _text ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _text, value ?? string.Empty);
    }

    /// <summary>
    /// Helper text with null safety
    /// </summary>
    public string HelperText
    {
        get => _helperText ?? "This is helper text";
        set => this.RaiseAndSetIfChanged(ref _helperText, value ?? string.Empty);
    }

    /// <summary>
    /// Validation message with null safety
    /// </summary>
    public string ValidationMessage
    {
        get => _validationMessage ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _validationMessage, value ?? string.Empty);
    }

    /// <summary>
    /// Icon for the control
    /// </summary>
    public MaterialIconKind Icon
    {
        get => _icon;
        set => this.RaiseAndSetIfChanged(ref _icon, value);
    }

    /// <summary>
    /// Whether the control is enabled
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    /// <summary>
    /// Whether the control is read-only
    /// </summary>
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
    }

    /// <summary>
    /// Whether the control has validation errors
    /// </summary>
    public bool HasValidationError
    {
        get => _hasValidationError;
        set => this.RaiseAndSetIfChanged(ref _hasValidationError, value);
    }

    /// <summary>
    /// Whether to show the clear button
    /// </summary>
    public bool ShowClearButton
    {
        get => _showClearButton;
        set => this.RaiseAndSetIfChanged(ref _showClearButton, value);
    }

    /// <summary>
    /// Whether to show the copy button
    /// </summary>
    public bool ShowCopyButton
    {
        get => _showCopyButton;
        set => this.RaiseAndSetIfChanged(ref _showCopyButton, value);
    }

    /// <summary>
    /// Whether to show the paste button
    /// </summary>
    public bool ShowPasteButton
    {
        get => _showPasteButton;
        set => this.RaiseAndSetIfChanged(ref _showPasteButton, value);
    }

    /// <summary>
    /// Whether to show the status icon
    /// </summary>
    public bool ShowStatusIcon
    {
        get => _showStatusIcon;
        set => this.RaiseAndSetIfChanged(ref _showStatusIcon, value);
    }

    /// <summary>
    /// Whether to show character count
    /// </summary>
    public bool ShowCharacterCount
    {
        get => _showCharacterCount;
        set => this.RaiseAndSetIfChanged(ref _showCharacterCount, value);
    }

    /// <summary>
    /// Whether to use floating label
    /// </summary>
    public bool UseFloatingLabel
    {
        get => _useFloatingLabel;
        set => this.RaiseAndSetIfChanged(ref _useFloatingLabel, value);
    }

    /// <summary>
    /// Maximum text length
    /// </summary>
    public int MaxLength
    {
        get => _maxLength;
        set => this.RaiseAndSetIfChanged(ref _maxLength, Math.Max(0, value));
    }

    /// <summary>
    /// Status icon kind
    /// </summary>
    public MaterialIconKind StatusIconKind
    {
        get => _statusIconKind;
        set => this.RaiseAndSetIfChanged(ref _statusIconKind, value);
    }

    /// <summary>
    /// Status icon color with null safety
    /// </summary>
    public IBrush StatusIconColor
    {
        get => _statusIconColor ?? Brushes.Green;
        set => this.RaiseAndSetIfChanged(ref _statusIconColor, value ?? Brushes.Green);
    }

    // Show/Hide properties for labels and helpers
    private bool _showLabel = true;
    public bool ShowLabel
    {
        get => _showLabel;
        set => this.RaiseAndSetIfChanged(ref _showLabel, value);
    }

    private bool _showStaticLabel = true;
    public bool ShowStaticLabel
    {
        get => _showStaticLabel;
        set => this.RaiseAndSetIfChanged(ref _showStaticLabel, value);
    }

    private bool _showHelperText = true;
    public bool ShowHelperText
    {
        get => _showHelperText;
        set => this.RaiseAndSetIfChanged(ref _showHelperText, value);
    }

    #endregion

    #region ComboBox Properties with Null Safety

    private ObservableCollection<string> _availableOptions = new(new[] { "Option 1", "Option 2", "Option 3" });
    public ObservableCollection<string> AvailableOptions
    {
        get => _availableOptions ?? new ObservableCollection<string>();
        set => this.RaiseAndSetIfChanged(ref _availableOptions, value ?? new ObservableCollection<string>());
    }

    private string? _selectedOption;
    public string? SelectedOption
    {
        get => _selectedOption;
        set => this.RaiseAndSetIfChanged(ref _selectedOption, value);
    }

    #endregion

    #region Commands with Null Safety

    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleValidationCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleEnabledCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleReadOnlyCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadSampleDataCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveAllCommand { get; }
    public ReactiveCommand<Unit, Unit> PopulateComboCommand { get; }
    public ReactiveCommand<Unit, Unit> TestFormattingCommand { get; }

    #endregion

    #region Additional Properties for Integration

    private string _textBoxValue = string.Empty;
    public string TextBoxValue
    {
        get => _textBoxValue ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _textBoxValue, value ?? string.Empty);
    }

    private string _floatingLabelText = string.Empty;
    public string FloatingLabelText
    {
        get => _floatingLabelText ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _floatingLabelText, value ?? string.Empty);
    }

    private string _multilineText = string.Empty;
    public string MultilineText
    {
        get => _multilineText ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _multilineText, value ?? string.Empty);
    }

    private string _statusMessage = "Ready";
    public string StatusMessage
    {
        get => _statusMessage ?? "Ready";
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value ?? "Ready");
    }

    private bool _controlsEnabled = true;
    public bool ControlsEnabled
    {
        get => _controlsEnabled;
        set => this.RaiseAndSetIfChanged(ref _controlsEnabled, value);
    }

    private bool _showValidationErrors = false;
    public bool ShowValidationErrors
    {
        get => _showValidationErrors;
        set => this.RaiseAndSetIfChanged(ref _showValidationErrors, value);
    }

    private string _richTextContent = "Sample rich text content";
    public string RichTextContent
    {
        get => _richTextContent ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _richTextContent, value ?? string.Empty);
    }

    private string _codeEditorContent = "// Sample code content\nvar example = 'Hello World';";
    public string CodeEditorContent
    {
        get => _codeEditorContent ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _codeEditorContent, value ?? string.Empty);
    }

    #endregion

    #region Collections with Null Safety

    private ObservableCollection<string> _testItems = new(new[] { "Test Item 1", "Test Item 2", "Test Item 3" });
    public ObservableCollection<string> TestItems
    {
        get => _testItems ?? new ObservableCollection<string>();
        set => this.RaiseAndSetIfChanged(ref _testItems, value ?? new ObservableCollection<string>());
    }

    private string? _selectedComboItem;
    public string? SelectedComboItem
    {
        get => _selectedComboItem;
        set => this.RaiseAndSetIfChanged(ref _selectedComboItem, value);
    }

    private ObservableCollection<string> _operationCodes = new(new[] { "90", "100", "110", "120" });
    public ObservableCollection<string> OperationCodes
    {
        get => _operationCodes ?? new ObservableCollection<string>();
        set => this.RaiseAndSetIfChanged(ref _operationCodes, value ?? new ObservableCollection<string>());
    }

    private ObservableCollection<string> _systemStatusOptions = new(new[] { "Active", "Inactive", "Pending", "Error" });
    public ObservableCollection<string> SystemStatusOptions
    {
        get => _systemStatusOptions ?? new ObservableCollection<string>();
        set => this.RaiseAndSetIfChanged(ref _systemStatusOptions, value ?? new ObservableCollection<string>());
    }

    private ObservableCollection<string> _qualityStatusOptions = new(new[] { "Pass", "Fail", "Review", "Hold" });
    public ObservableCollection<string> QualityStatusOptions
    {
        get => _qualityStatusOptions ?? new ObservableCollection<string>();
        set => this.RaiseAndSetIfChanged(ref _qualityStatusOptions, value ?? new ObservableCollection<string>());
    }

    #endregion

    #region Constructor

    public TestControlsViewModel()
    {
        // Initialize commands with null safety
        ClearCommand = ReactiveCommand.Create(ExecuteClear);
        CopyCommand = ReactiveCommand.Create(ExecuteCopy);
        PasteCommand = ReactiveCommand.Create(ExecutePaste);
        ToggleValidationCommand = ReactiveCommand.Create(ExecuteToggleValidation);
        ToggleEnabledCommand = ReactiveCommand.Create(ExecuteToggleEnabled);
        ToggleReadOnlyCommand = ReactiveCommand.Create(ExecuteToggleReadOnly);
        LoadSampleDataCommand = ReactiveCommand.Create(ExecuteLoadSampleData);
        ClearAllCommand = ReactiveCommand.Create(ExecuteClearAll);
        SaveAllCommand = ReactiveCommand.Create(ExecuteSaveAll);
        PopulateComboCommand = ReactiveCommand.Create(ExecutePopulateCombo);
        TestFormattingCommand = ReactiveCommand.Create(ExecuteTestFormatting);

        // Set up property change notifications with null safety
        this.WhenAnyValue(x => x.Text)
            .Subscribe(text => UpdateCharacterCount(text ?? string.Empty));

        this.WhenAnyValue(x => x.HasValidationError)
            .Subscribe(hasError => UpdateStatusIcon(hasError));
    }

    #endregion

    #region Command Implementations

    private void ExecuteClear()
    {
        Text = string.Empty;
        HasValidationError = false;
        ValidationMessage = string.Empty;
    }

    private void ExecuteCopy()
    {
        // Copy current text to clipboard (implementation would go here)
        if (!string.IsNullOrEmpty(Text))
        {
            // Clipboard implementation
        }
    }

    private void ExecutePaste()
    {
        // Paste from clipboard (implementation would go here)
        Text = "Pasted content example";
    }

    private void ExecuteToggleValidation()
    {
        HasValidationError = !HasValidationError;
        ValidationMessage = HasValidationError ? "This is a validation error message" : string.Empty;
    }

    private void ExecuteToggleEnabled()
    {
        IsEnabled = !IsEnabled;
    }

    private void ExecuteToggleReadOnly()
    {
        IsReadOnly = !IsReadOnly;
    }

    private void ExecuteLoadSampleData()
    {
        Text = "Sample data loaded";
        TextBoxValue = "Sample TextBox Value";
        FloatingLabelText = "Sample Floating Label";
        MultilineText = "Sample multiline text\nLine 2\nLine 3";
        Label = "Sample Label";
        HelperText = "Sample helper text";
        ShowStatusIcon = true;
        StatusIconKind = MaterialIconKind.CheckCircle;
        StatusIconColor = Brushes.Green;
    }

    private void ExecuteClearAll()
    {
        Text = string.Empty;
        TextBoxValue = string.Empty;
        FloatingLabelText = string.Empty;
        MultilineText = string.Empty;
        RichTextContent = string.Empty;
        CodeEditorContent = string.Empty;
        SelectedComboItem = null;
        HasValidationError = false;
        ValidationMessage = string.Empty;
        StatusMessage = "All cleared";
    }

    private void ExecuteSaveAll()
    {
        StatusMessage = "All data saved successfully";
        ShowStatusIcon = true;
        StatusIconKind = MaterialIconKind.CheckCircle;
        StatusIconColor = Brushes.Green;
    }

    private void ExecutePopulateCombo()
    {
        TestItems.Clear();
        for (int i = 1; i <= 10; i++)
        {
            TestItems.Add($"Generated Item {i}");
        }
        StatusMessage = "ComboBox populated with sample data";
    }

    private void ExecuteTestFormatting()
    {
        RichTextContent = "This is **bold** text and *italic* text with formatting examples.";
        StatusMessage = "Rich text formatting applied";
    }

    #endregion

    #region Helper Methods

    private void UpdateCharacterCount(string text)
    {
        if (ShowCharacterCount && MaxLength > 0)
        {
            var count = text?.Length ?? 0;
            // Update character count display logic here
        }
    }

    private void UpdateStatusIcon(bool hasError)
    {
        if (hasError)
        {
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.AlertCircle;
            StatusIconColor = Brushes.Red;
        }
        else if (!string.IsNullOrEmpty(Text))
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

    #endregion

    #region INotifyPropertyChanged Implementation

    // ReactiveObject already implements INotifyPropertyChanged
    // We just need to expose the PropertyChanged event if needed for compatibility
    public new event PropertyChangedEventHandler? PropertyChanged
    {
        add => ((INotifyPropertyChanged)this).PropertyChanged += value;
        remove => ((INotifyPropertyChanged)this).PropertyChanged -= value;
    }

    #endregion
}
