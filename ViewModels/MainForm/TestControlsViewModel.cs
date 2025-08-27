using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using Material.Icons;
using Avalonia.Media;
using Avalonia;
using System.Linq;

namespace MTM_WIP_Application_Avalonia.ViewModels.MainForm;

/// <summary>
/// ViewModel for testing and showcasing custom MTM controls
/// </summary>
public class TestControlsViewModel : BaseViewModel
{
    #region MTMComboBox Test Properties
    private ObservableCollection<string> _testItems = new();
    public ObservableCollection<string> TestItems
    {
        get => _testItems;
        set => this.RaiseAndSetIfChanged(ref _testItems, value);
    }

    private ObservableCollection<string> _operationCodes = new();
    public ObservableCollection<string> OperationCodes
    {
        get => _operationCodes;
        set => this.RaiseAndSetIfChanged(ref _operationCodes, value);
    }

    private ObservableCollection<string> _systemStatusOptions = new();
    public ObservableCollection<string> SystemStatusOptions
    {
        get => _systemStatusOptions;
        set => this.RaiseAndSetIfChanged(ref _systemStatusOptions, value);
    }

    private ObservableCollection<string> _qualityStatusOptions = new();
    public ObservableCollection<string> QualityStatusOptions
    {
        get => _qualityStatusOptions;
        set => this.RaiseAndSetIfChanged(ref _qualityStatusOptions, value);
    }

    // Generic Items property for MTMComboBox binding
    private ObservableCollection<string> _items = new();
    public ObservableCollection<string> Items
    {
        get => _items;
        set => this.RaiseAndSetIfChanged(ref _items, value);
    }

    private object? _selectedItem;
    public object? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private string _placeholderText = "Select an option...";
    public string PlaceholderText
    {
        get => _placeholderText;
        set => this.RaiseAndSetIfChanged(ref _placeholderText, value);
    }

    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    // Icon and Label properties
    private MaterialIconKind _icon = MaterialIconKind.TestTube;
    public MaterialIconKind Icon
    {
        get => _icon;
        set => this.RaiseAndSetIfChanged(ref _icon, value);
    }

    private string _label = "Test Label";
    public string Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }

    // Status properties
    private bool _showStatusIcon = false;
    public bool ShowStatusIcon
    {
        get => _showStatusIcon;
        set => this.RaiseAndSetIfChanged(ref _showStatusIcon, value);
    }

    private MaterialIconKind _statusIconKind = MaterialIconKind.CheckCircle;
    public MaterialIconKind StatusIconKind
    {
        get => _statusIconKind;
        set => this.RaiseAndSetIfChanged(ref _statusIconKind, value);
    }

    private IBrush _statusIconColor = Brushes.Green;
    public IBrush StatusIconColor
    {
        get => _statusIconColor;
        set => this.RaiseAndSetIfChanged(ref _statusIconColor, value);
    }

    // Clear button properties
    private bool _showClearButton = true;
    public bool ShowClearButton
    {
        get => _showClearButton;
        set => this.RaiseAndSetIfChanged(ref _showClearButton, value);
    }

    // Validation properties
    private string? _validationMessage;
    public string? ValidationMessage
    {
        get => _validationMessage;
        set => this.RaiseAndSetIfChanged(ref _validationMessage, value);
    }

    private bool _hasValidationError = false;
    public bool HasValidationError
    {
        get => _hasValidationError;
        set => this.RaiseAndSetIfChanged(ref _hasValidationError, value);
    }
    #endregion

    #region MTMTextBox Test Properties
    private string _textBoxValue = string.Empty;
    public string TextBoxValue
    {
        get => _textBoxValue;
        set => this.RaiseAndSetIfChanged(ref _textBoxValue, value);
    }

    // Generic Text property for MTMTextBox binding
    private string _text = string.Empty;
    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, value);
    }

    private string _floatingLabelText = "Floating Label";
    public string FloatingLabelText
    {
        get => _floatingLabelText;
        set => this.RaiseAndSetIfChanged(ref _floatingLabelText, value);
    }

    private bool _useFloatingLabel = true;
    public bool UseFloatingLabel
    {
        get => _useFloatingLabel;
        set => this.RaiseAndSetIfChanged(ref _useFloatingLabel, value);
    }

    private int _maxLength = 100;
    public int MaxLength
    {
        get => _maxLength;
        set => this.RaiseAndSetIfChanged(ref _maxLength, value);
    }

    private bool _isReadOnly = false;
    public bool IsReadOnly
    {
        get => _isReadOnly;
        set => this.RaiseAndSetIfChanged(ref _isReadOnly, value);
    }

    private bool _acceptsReturn = false;
    public bool AcceptsReturn
    {
        get => _acceptsReturn;
        set => this.RaiseAndSetIfChanged(ref _acceptsReturn, value);
    }

    private Avalonia.Media.TextWrapping _textWrapping = Avalonia.Media.TextWrapping.NoWrap;
    public Avalonia.Media.TextWrapping TextWrapping
    {
        get => _textWrapping;
        set => this.RaiseAndSetIfChanged(ref _textWrapping, value);
    }

    // Character count properties
    private string _characterCountText = "0/100";
    public string CharacterCountText
    {
        get => _characterCountText;
        set => this.RaiseAndSetIfChanged(ref _characterCountText, value);
    }

    private bool _showCharacterCount = true;
    public bool ShowCharacterCount
    {
        get => _showCharacterCount;
        set => this.RaiseAndSetIfChanged(ref _showCharacterCount, value);
    }

    // Copy/Paste button properties
    private bool _showCopyButton = true;
    public bool ShowCopyButton
    {
        get => _showCopyButton;
        set => this.RaiseAndSetIfChanged(ref _showCopyButton, value);
    }

    private bool _showPasteButton = true;
    public bool ShowPasteButton
    {
        get => _showPasteButton;
        set => this.RaiseAndSetIfChanged(ref _showPasteButton, value);
    }

    // Helper text
    private string? _helperText = "This is helper text";
    public string? HelperText
    {
        get => _helperText;
        set => this.RaiseAndSetIfChanged(ref _helperText, value);
    }
    #endregion

    #region MTMRichTextBox Test Properties
    // Toolbar properties
    private bool _showToolbar = true;
    public bool ShowToolbar
    {
        get => _showToolbar;
        set => this.RaiseAndSetIfChanged(ref _showToolbar, value);
    }

    // Formatting properties
    private bool _isBold = false;
    public bool IsBold
    {
        get => _isBold;
        set => this.RaiseAndSetIfChanged(ref _isBold, value);
    }

    private bool _isItalic = false;
    public bool IsItalic
    {
        get => _isItalic;
        set => this.RaiseAndSetIfChanged(ref _isItalic, value);
    }

    private bool _isUnderline = false;
    public bool IsUnderline
    {
        get => _isUnderline;
        set => this.RaiseAndSetIfChanged(ref _isUnderline, value);
    }

    // Word count properties
    private string _wordCountText = "0 words";
    public string WordCountText
    {
        get => _wordCountText;
        set => this.RaiseAndSetIfChanged(ref _wordCountText, value);
    }

    private bool _showWordCount = true;
    public bool ShowWordCount
    {
        get => _showWordCount;
        set => this.RaiseAndSetIfChanged(ref _showWordCount, value);
    }

    // Save button properties
    private bool _showSaveButton = true;
    public bool ShowSaveButton
    {
        get => _showSaveButton;
        set => this.RaiseAndSetIfChanged(ref _showSaveButton, value);
    }

    // Line numbers properties
    private bool _showLineNumbers = false;
    public bool ShowLineNumbers
    {
        get => _showLineNumbers;
        set => this.RaiseAndSetIfChanged(ref _showLineNumbers, value);
    }

    private ObservableCollection<int> _lineNumbers = new();
    public ObservableCollection<int> LineNumbers
    {
        get => _lineNumbers;
        set => this.RaiseAndSetIfChanged(ref _lineNumbers, value);
    }

    // Font properties
    private FontFamily _fontFamily = FontFamily.Default;
    public FontFamily FontFamily
    {
        get => _fontFamily;
        set => this.RaiseAndSetIfChanged(ref _fontFamily, value);
    }

    private double _fontSize = 12;
    public double FontSize
    {
        get => _fontSize;
        set => this.RaiseAndSetIfChanged(ref _fontSize, value);
    }

    // Status bar properties
    private bool _showStatusBar = true;
    public bool ShowStatusBar
    {
        get => _showStatusBar;
        set => this.RaiseAndSetIfChanged(ref _showStatusBar, value);
    }

    private string _cursorPositionText = "Line 1, Column 1";
    public string CursorPositionText
    {
        get => _cursorPositionText;
        set => this.RaiseAndSetIfChanged(ref _cursorPositionText, value);
    }
    #endregion

    #region General Test Properties
    private string _multilineText = string.Empty;
    public string MultilineText
    {
        get => _multilineText;
        set => this.RaiseAndSetIfChanged(ref _multilineText, value);
    }

    private bool _showValidationErrors = false;
    public bool ShowValidationErrors
    {
        get => _showValidationErrors;
        set => this.RaiseAndSetIfChanged(ref _showValidationErrors, value);
    }

    private string _statusMessage = "Ready - Test the controls below";
    public string StatusMessage
    {
        get => _statusMessage;
        set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
    }
    #endregion

    #region Additional Missing Properties for TestControlsView
    // Control enabled state
    private bool _controlsEnabled = true;
    public bool ControlsEnabled
    {
        get => _controlsEnabled;
        set => this.RaiseAndSetIfChanged(ref _controlsEnabled, value);
    }

    // Selected combo item
    private object? _selectedComboItem;
    public object? SelectedComboItem
    {
        get => _selectedComboItem;
        set => this.RaiseAndSetIfChanged(ref _selectedComboItem, value);
    }

    // Rich text content
    private string _richTextContent = "Rich text content for testing formatting features...";
    public string RichTextContent
    {
        get => _richTextContent;
        set => this.RaiseAndSetIfChanged(ref _richTextContent, value);
    }

    // Code editor content
    private string _codeEditorContent = "// Code editor content\nusing System;\nusing Avalonia;\n\npublic class TestClass\n{\n    public void TestMethod()\n    {\n        Console.WriteLine(\"Hello MTM!\");\n    }\n}";
    public string CodeEditorContent
    {
        get => _codeEditorContent;
        set => this.RaiseAndSetIfChanged(ref _codeEditorContent, value);
    }
    #endregion

    #region Commands
    public ReactiveCommand<Unit, Unit> LoadSampleDataCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearAllFieldsCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleControlsEnabledCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleValidationCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveAllDataCommand { get; }
    public ReactiveCommand<Unit, Unit> TestFormattingCommand { get; }
    public ReactiveCommand<Unit, Unit> PopulateComboBoxCommand { get; }

    // Additional commands required by TestControlsView
    public ReactiveCommand<Unit, Unit> ToggleEnabledCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearAllCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveAllCommand { get; }
    public ReactiveCommand<Unit, Unit> PopulateComboCommand { get; }

    // Commands for MTM controls
    public ReactiveCommand<Unit, Unit> ClearCommand { get; }
    public ReactiveCommand<Unit, Unit> CopyCommand { get; }
    public ReactiveCommand<Unit, Unit> PasteCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    // Formatting commands for RichTextBox
    public ReactiveCommand<Unit, Unit> ToggleBoldCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleItalicCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleUnderlineCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleBulletListCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleNumberListCommand { get; }
    #endregion

    public TestControlsViewModel() : base()
    {
        Logger.LogDebug("TestControlsViewModel constructor started");

        // Initialize commands
        LoadSampleDataCommand = ReactiveCommand.Create(LoadSampleData);
        ClearAllFieldsCommand = ReactiveCommand.Create(ClearAllFields);
        ToggleControlsEnabledCommand = ReactiveCommand.Create(ToggleControlsEnabled);
        ToggleValidationCommand = ReactiveCommand.Create(ToggleValidationErrors);
        SaveAllDataCommand = ReactiveCommand.CreateFromTask(SaveAllData);
        TestFormattingCommand = ReactiveCommand.Create(TestFormatting);
        PopulateComboBoxCommand = ReactiveCommand.Create(PopulateComboBox);

        // Additional commands for TestControlsView
        ToggleEnabledCommand = ReactiveCommand.Create(() => {
            ControlsEnabled = !ControlsEnabled;
            StatusMessage = $"Controls {(ControlsEnabled ? "enabled" : "disabled")}";
        });

        ClearAllCommand = ReactiveCommand.Create(() => {
            ClearAllFields();
        });

        SaveAllCommand = ReactiveCommand.CreateFromTask(async () => {
            await SaveAllData();
        });

        PopulateComboCommand = ReactiveCommand.Create(() => {
            PopulateComboBox();
        });

        // MTM Control commands
        ClearCommand = ReactiveCommand.Create(() => {
            Text = string.Empty;
            TextBoxValue = string.Empty;
            SelectedItem = null;
            StatusMessage = "Fields cleared";
        });

        CopyCommand = ReactiveCommand.Create(() => {
            // TODO: Implement clipboard copy
            StatusMessage = "Text copied to clipboard";
        });

        PasteCommand = ReactiveCommand.Create(() => {
            // TODO: Implement clipboard paste
            StatusMessage = "Text pasted from clipboard";
        });

        SaveCommand = ReactiveCommand.Create(() => {
            StatusMessage = "Content saved successfully";
        });

        // Formatting commands
        ToggleBoldCommand = ReactiveCommand.Create(() => {
            IsBold = !IsBold;
            StatusMessage = $"Bold formatting {(IsBold ? "enabled" : "disabled")}";
        });

        ToggleItalicCommand = ReactiveCommand.Create(() => {
            IsItalic = !IsItalic;
            StatusMessage = $"Italic formatting {(IsItalic ? "enabled" : "disabled")}";
        });

        ToggleUnderlineCommand = ReactiveCommand.Create(() => {
            IsUnderline = !IsUnderline;
            StatusMessage = $"Underline formatting {(IsUnderline ? "enabled" : "disabled")}";
        });

        ToggleBulletListCommand = ReactiveCommand.Create(() => {
            StatusMessage = "Bullet list toggled";
        });

        ToggleNumberListCommand = ReactiveCommand.Create(() => {
            StatusMessage = "Numbered list toggled";
        });

        // Initialize data
        InitializeData();

        // Set up property change handlers for character count
        this.WhenAnyValue(x => x.Text)
            .Subscribe(text => {
                var count = text?.Length ?? 0;
                CharacterCountText = $"{count}/{MaxLength}";
                
                // Update word count for rich text
                var words = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
                WordCountText = $"{words} word{(words != 1 ? "s" : "")}";
            });

        Logger.LogInformation("TestControlsViewModel initialized");
        Logger.LogInformation($"TestItems count: {TestItems.Count}");
        Logger.LogInformation($"OperationCodes count: {OperationCodes.Count}");
    }

    private void InitializeData()
    {
        try
        {
            Logger.LogDebug("Initializing test data");

            // Initialize TestItems
            TestItems.Clear();
            TestItems.Add("Test Item 1");
            TestItems.Add("Test Item 2");
            TestItems.Add("Test Item 3");
            TestItems.Add("Test Item 4");
            TestItems.Add("Test Item 5");
            TestItems.Add("Advanced Test Item A");
            TestItems.Add("Advanced Test Item B");
            TestItems.Add("Complex Test Scenario");

            // Initialize OperationCodes  
            OperationCodes.Clear();
            OperationCodes.Add("90 - Raw Material");
            OperationCodes.Add("100 - Work in Process");
            OperationCodes.Add("110 - Quality Check");
            OperationCodes.Add("120 - Final Assembly");
            OperationCodes.Add("200 - Finished Goods");

            // Initialize SystemStatusOptions
            SystemStatusOptions.Clear();
            SystemStatusOptions.Add("Active");
            SystemStatusOptions.Add("Inactive");
            SystemStatusOptions.Add("Pending");
            SystemStatusOptions.Add("Maintenance");
            SystemStatusOptions.Add("Error");

            // Initialize QualityStatusOptions
            QualityStatusOptions.Clear();
            QualityStatusOptions.Add("Pass");
            QualityStatusOptions.Add("Fail");
            QualityStatusOptions.Add("Review Required");
            QualityStatusOptions.Add("On Hold");
            QualityStatusOptions.Add("Approved");

            // Set Items to TestItems by default
            Items.Clear();
            foreach (var item in TestItems)
            {
                Items.Add(item);
            }

            // Initialize line numbers
            LineNumbers.Clear();
            for (int i = 1; i <= 10; i++)
            {
                LineNumbers.Add(i);
            }

            // Set default text content
            Text = "Sample text for testing";
            TextBoxValue = "Test value";
            MultilineText = "Line 1\nLine 2\nLine 3\nThis is a multi-line text area for testing the MTM Rich Text Box control.";

            Logger.LogDebug("Test data initialization completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing test data");
            StatusMessage = $"Error initializing data: {ex.Message}";
        }
    }

    private void PopulateComboBox()
    {
        try
        {
            Logger.LogDebug("Populating ComboBox with operation codes");
            
            Items.Clear();
            foreach (var code in OperationCodes)
            {
                Items.Add(code);
            }
            
            StatusMessage = "ComboBox populated with operation codes";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error populating ComboBox");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void ClearAllFields()
    {
        try
        {
            Logger.LogDebug("Clearing all test fields");
            
            Text = string.Empty;
            TextBoxValue = string.Empty;
            MultilineText = string.Empty;
            SelectedItem = null;
            HasValidationError = false;
            ValidationMessage = null;
            
            StatusMessage = "All fields cleared";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing fields");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void ToggleControlsEnabled()
    {
        try
        {
            ControlsEnabled = !ControlsEnabled;
            IsEnabled = ControlsEnabled; // Keep both properties in sync
            StatusMessage = $"Controls {(ControlsEnabled ? "enabled" : "disabled")}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling controls");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void ToggleValidationErrors()
    {
        try
        {
            ShowValidationErrors = !ShowValidationErrors;
            
            if (ShowValidationErrors)
            {
                TriggerValidationErrors();
            }
            else
            {
                ClearValidationErrors();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling validation");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void LoadSampleData()
    {
        try
        {
            Logger.LogDebug("Loading sample data");
            
            Text = "MTM Test Application - Sample Data";
            TextBoxValue = "Sample Value for Testing";
            MultilineText = "This is sample data for testing the MTM custom controls.\n\nLine 1: Basic text input\nLine 2: Advanced formatting\nLine 3: Validation testing\n\nThe controls should handle this data properly.";
            SelectedItem = Items.FirstOrDefault();
            
            StatusMessage = "Sample data loaded successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading sample data");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void TestFormatting()
    {
        try
        {
            Logger.LogDebug("Testing formatting options");
            
            IsBold = !IsBold;
            IsItalic = !IsItalic;
            ShowToolbar = !ShowToolbar;
            ShowLineNumbers = !ShowLineNumbers;
            
            StatusMessage = "Formatting options toggled";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error testing formatting");
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private async Task SaveAllData()
    {
        try
        {
            Logger.LogDebug("Saving all test data");
            StatusMessage = "Saving data...";
            
            // Simulate async save operation
            await Task.Delay(1000);
            
            StatusMessage = "All data saved successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving data");
            StatusMessage = $"Save error: {ex.Message}";
        }
    }

    /// <summary>
    /// Simulate validation errors for testing
    /// </summary>
    public void TriggerValidationErrors()
    {
        try
        {
            HasValidationError = true;
            ValidationMessage = "Sample validation error - This is a test error message";
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.AlertCircle;
            StatusIconColor = Brushes.Red;
            
            StatusMessage = "Validation errors triggered for testing";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error triggering validation errors");
        }
    }

    /// <summary>
    /// Reset all validation states
    /// </summary>
    public void ClearValidationErrors()
    {
        try
        {
            HasValidationError = false;
            ValidationMessage = null;
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.CheckCircle;
            StatusIconColor = Brushes.Green;
            
            StatusMessage = "Validation errors cleared";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error clearing validation errors");
        }
    }

    #region Additional Missing Properties
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
}
