using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Material.Icons;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// Custom expandable button control for transaction history display.
/// Shows transaction type, Part ID, Operation, and Quantity in collapsed state.
/// Shows full transaction details when expanded.
/// Uses MTM theming with transaction type color coding (IN=Green, OUT=Red, Transfer=Yellow).
/// </summary>
public partial class TransactionExpandableButton : UserControl
{
    #region Styled Properties

    public static readonly StyledProperty<string> PartIdProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(PartId), string.Empty);

    public static readonly StyledProperty<string> OperationProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(Operation), string.Empty);

    public static readonly StyledProperty<int> QuantityProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, int>(nameof(Quantity), 0);

    public static readonly StyledProperty<string> LocationProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(Location), string.Empty);

    public static readonly StyledProperty<string> UserProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(User), string.Empty);

    public static readonly StyledProperty<DateTime> TransactionTimeProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, DateTime>(nameof(TransactionTime), DateTime.Now);

    public static readonly StyledProperty<string> StatusProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(Status), string.Empty);

    public static readonly StyledProperty<string> ItemTypeProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(ItemType), string.Empty);

    public static readonly StyledProperty<string> TransactionIdProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(TransactionId), string.Empty);

    public static readonly StyledProperty<string> NotesProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(Notes), string.Empty);

    public static readonly StyledProperty<string> TransactionTypeProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, string>(nameof(TransactionType), "IN");

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<TransactionExpandableButton, bool>(nameof(IsExpanded), false);

    #endregion

    #region Properties

    public string PartId
    {
        get => GetValue(PartIdProperty);
        set => SetValue(PartIdProperty, value);
    }

    public string Operation
    {
        get => GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }

    public int Quantity
    {
        get => GetValue(QuantityProperty);
        set => SetValue(QuantityProperty, value);
    }

    public string Location
    {
        get => GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public string User
    {
        get => GetValue(UserProperty);
        set => SetValue(UserProperty, value);
    }

    public DateTime TransactionTime
    {
        get => GetValue(TransactionTimeProperty);
        set => SetValue(TransactionTimeProperty, value);
    }

    public string Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string ItemType
    {
        get => GetValue(ItemTypeProperty);
        set => SetValue(ItemTypeProperty, value);
    }

    public string TransactionId
    {
        get => GetValue(TransactionIdProperty);
        set => SetValue(TransactionIdProperty, value);
    }

    public string Notes
    {
        get => GetValue(NotesProperty);
        set => SetValue(NotesProperty, value);
    }

    public string TransactionType
    {
        get => GetValue(TransactionTypeProperty);
        set => SetValue(TransactionTypeProperty, value);
    }

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    #endregion

    #region UI Elements

    private Border? _headerBorder;
    private Border? _contentBorder;
    private Material.Icons.Avalonia.MaterialIcon? _transactionIcon;
    private Material.Icons.Avalonia.MaterialIcon? _expandIcon;
    private TextBlock? _partIdText;
    private TextBlock? _operationText;
    private TextBlock? _quantityText;
    private TextBlock? _locationText;
    private TextBlock? _userText;
    private TextBlock? _transactionTimeText;
    private TextBlock? _statusText;
    private TextBlock? _itemTypeText;
    private TextBlock? _transactionIdText;
    private TextBlock? _notesText;
    private Grid? _notesPanel;

    #endregion

    #region Constructor

    public TransactionExpandableButton()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    #endregion

    #region Property Changed Handling

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        // Get references to named elements
        _headerBorder = this.FindControl<Border>("HeaderBorder");
        _contentBorder = this.FindControl<Border>("ContentBorder");
        _transactionIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("TransactionIcon");
        _expandIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("ExpandIcon");
        _partIdText = this.FindControl<TextBlock>("PartIdText");
        _operationText = this.FindControl<TextBlock>("OperationText");
        _quantityText = this.FindControl<TextBlock>("QuantityText");
        _locationText = this.FindControl<TextBlock>("LocationText");
        _userText = this.FindControl<TextBlock>("UserText");
        _transactionTimeText = this.FindControl<TextBlock>("TransactionTimeText");
        _statusText = this.FindControl<TextBlock>("StatusText");
        _itemTypeText = this.FindControl<TextBlock>("ItemTypeText");
        _transactionIdText = this.FindControl<TextBlock>("TransactionIdText");
        _notesText = this.FindControl<TextBlock>("NotesText");
        _notesPanel = this.FindControl<Grid>("NotesPanel");
        
        // Update the display with current values
        UpdateDisplay();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == PartIdProperty ||
            change.Property == OperationProperty ||
            change.Property == QuantityProperty ||
            change.Property == LocationProperty ||
            change.Property == UserProperty ||
            change.Property == TransactionTimeProperty ||
            change.Property == StatusProperty ||
            change.Property == ItemTypeProperty ||
            change.Property == TransactionIdProperty ||
            change.Property == NotesProperty ||
            change.Property == TransactionTypeProperty ||
            change.Property == IsExpandedProperty)
        {
            UpdateDisplay();
        }
    }

    #endregion

    #region Display Updates

    private void UpdateDisplay()
    {
        if (_partIdText == null) return; // Not initialized yet
        
        // Update header content
        _partIdText!.Text = PartId;
        _operationText!.Text = string.IsNullOrEmpty(Operation) ? "Unknown" : $"Operation {Operation}";
        _quantityText!.Text = Quantity.ToString();
        
        // Update content details
        if (_locationText != null) _locationText.Text = Location;
        if (_userText != null) _userText.Text = User;
        if (_transactionTimeText != null) _transactionTimeText.Text = TransactionTime.ToString("HH:mm:ss");
        if (_statusText != null) _statusText.Text = Status;
        if (_itemTypeText != null) _itemTypeText.Text = ItemType;
        if (_transactionIdText != null) _transactionIdText.Text = TransactionId;
        if (_notesText != null) _notesText.Text = Notes;
        
        // Show/hide notes panel based on content
        if (_notesPanel != null)
        {
            _notesPanel.IsVisible = !string.IsNullOrEmpty(Notes);
        }
        
        // Update transaction type styling and icon
        UpdateTransactionTypeDisplay();
        
        // Update expanded/collapsed state
        UpdateExpandedState();
    }

    private void UpdateTransactionTypeDisplay()
    {
        if (_headerBorder == null || _contentBorder == null || _transactionIcon == null) return;
        
        // Clear existing transaction type classes
        _headerBorder.Classes.Remove("transaction-in");
        _headerBorder.Classes.Remove("transaction-out");
        _headerBorder.Classes.Remove("transaction-transfer");
        
        _contentBorder.Classes.Remove("transaction-in-light");
        _contentBorder.Classes.Remove("transaction-out-light");
        _contentBorder.Classes.Remove("transaction-transfer-light");
        
        // Set appropriate styling based on transaction type
        switch (TransactionType.ToUpperInvariant())
        {
            case "IN":
                _headerBorder.Classes.Add("transaction-in");
                _contentBorder.Classes.Add("transaction-in-light");
                _transactionIcon.Kind = MaterialIconKind.ArrowUp;
                break;
            case "OUT":
                _headerBorder.Classes.Add("transaction-out");
                _contentBorder.Classes.Add("transaction-out-light");
                _transactionIcon.Kind = MaterialIconKind.ArrowDown;
                break;
            case "TRANSFER":
                _headerBorder.Classes.Add("transaction-transfer");
                _contentBorder.Classes.Add("transaction-transfer-light");
                _transactionIcon.Kind = MaterialIconKind.SwapHorizontal;
                break;
            default:
                _headerBorder.Classes.Add("transaction-in"); // Default to IN styling
                _contentBorder.Classes.Add("transaction-in-light");
                _transactionIcon.Kind = MaterialIconKind.Help;
                break;
        }
    }

    private void UpdateExpandedState()
    {
        if (_contentBorder == null || _expandIcon == null) return;
        
        _contentBorder.IsVisible = IsExpanded;
        _expandIcon.Kind = IsExpanded ? MaterialIconKind.ChevronDown : MaterialIconKind.ChevronRight;
        
        // Update header corner radius based on expanded state
        if (_headerBorder != null)
        {
            _headerBorder.CornerRadius = IsExpanded ? new CornerRadius(8, 8, 0, 0) : new CornerRadius(8);
        }
    }

    #endregion

    #region Event Handlers

    private void OnHeaderPressed(object? sender, PointerPressedEventArgs e)
    {
        // Toggle expanded state
        IsExpanded = !IsExpanded;
    }

    #endregion
}