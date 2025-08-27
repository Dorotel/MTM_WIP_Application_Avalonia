using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// Enhanced ComboBox with MTM styling, validation, and features
/// </summary>
public partial class MTMComboBox : UserControl
{
    // Styled Properties
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<MTMComboBox, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<MaterialIconKind> IconProperty =
        AvaloniaProperty.Register<MTMComboBox, MaterialIconKind>(nameof(Icon), MaterialIconKind.FilterVariant);

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<MTMComboBox, string>(nameof(PlaceholderText), "Select an option...");

    public static readonly StyledProperty<IEnumerable> ItemsProperty =
        AvaloniaProperty.Register<MTMComboBox, IEnumerable>(nameof(Items));

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<MTMComboBox, object?>(nameof(SelectedItem));

    public static readonly StyledProperty<bool> ShowClearButtonProperty =
        AvaloniaProperty.Register<MTMComboBox, bool>(nameof(ShowClearButton), true);

    public static readonly StyledProperty<bool> ShowStatusIconProperty =
        AvaloniaProperty.Register<MTMComboBox, bool>(nameof(ShowStatusIcon), false);

    public static readonly StyledProperty<MaterialIconKind> StatusIconKindProperty =
        AvaloniaProperty.Register<MTMComboBox, MaterialIconKind>(nameof(StatusIconKind), MaterialIconKind.CheckCircle);

    public static readonly StyledProperty<IBrush> StatusIconColorProperty =
        AvaloniaProperty.Register<MTMComboBox, IBrush>(nameof(StatusIconColor), Brushes.Green);

    public static readonly StyledProperty<bool> HasValidationErrorProperty =
        AvaloniaProperty.Register<MTMComboBox, bool>(nameof(HasValidationError), false);

    public static readonly StyledProperty<string> ValidationMessageProperty =
        AvaloniaProperty.Register<MTMComboBox, string>(nameof(ValidationMessage), string.Empty);

    public static readonly StyledProperty<bool> IsRequiredProperty =
        AvaloniaProperty.Register<MTMComboBox, bool>(nameof(IsRequired), false);

    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<MTMComboBox, bool>(nameof(ShowLabel), true);

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

    public IEnumerable Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool ShowClearButton
    {
        get => GetValue(ShowClearButtonProperty);
        set => SetValue(ShowClearButtonProperty, value);
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

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public bool ShowLabel
    {
        get => GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    // Commands
    public ICommand ClearCommand { get; }

    // Events
    public event EventHandler<object?>? SelectionChanged;
    public event EventHandler? Cleared;

    public MTMComboBox()
    {
        InitializeComponent();
        
        // Initialize commands
        ClearCommand = ReactiveCommand.Create(ClearSelection);
        
        // Subscribe to property changes
        this.GetObservable(SelectedItemProperty)
            .Subscribe(OnSelectedItemChanged);
            
        this.GetObservable(HasValidationErrorProperty)
            .Subscribe(OnValidationStateChanged);
    }

    protected override void OnApplyTemplate(Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Get reference to the ComboBox and subscribe to its SelectionChanged event
        var comboBox = e.NameScope.Find<ComboBox>("PART_ComboBox");
        if (comboBox != null)
        {
            comboBox.SelectionChanged += OnComboBoxSelectionChanged;
        }
    }

    private void OnComboBoxSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Update validation when selection changes
        ValidateSelection();
        
        // Raise our custom event
        SelectionChanged?.Invoke(this, SelectedItem);
    }

    private void OnSelectedItemChanged(object? selectedItem)
    {
        ValidateSelection();
        UpdateStatusIcon();
    }

    private void OnValidationStateChanged(bool hasError)
    {
        UpdateContainerState();
    }

    private void ClearSelection()
    {
        SelectedItem = null;
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    private void ValidateSelection()
    {
        if (IsRequired && SelectedItem == null)
        {
            HasValidationError = true;
            ValidationMessage = $"{Label} is required.";
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.AlertCircle;
            StatusIconColor = Brushes.Red;
        }
        else
        {
            HasValidationError = false;
            ValidationMessage = string.Empty;
            
            if (SelectedItem != null)
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
    }

    private void UpdateStatusIcon()
    {
        if (SelectedItem != null && !HasValidationError)
        {
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.CheckCircle;
            StatusIconColor = Brushes.Green;
        }
        else if (HasValidationError)
        {
            ShowStatusIcon = true;
            StatusIconKind = MaterialIconKind.AlertCircle;
            StatusIconColor = Brushes.Red;
        }
        else
        {
            ShowStatusIcon = false;
        }
    }

    private void UpdateContainerState()
    {
        var container = this.FindControl<Border>("ComboContainer");
        if (container != null)
        {
            if (HasValidationError)
            {
                container.Classes.Add("error");
                container.Classes.Remove("success");
            }
            else if (SelectedItem != null)
            {
                container.Classes.Add("success");
                container.Classes.Remove("error");
            }
            else
            {
                container.Classes.Remove("error");
                container.Classes.Remove("success");
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
        UpdateStatusIcon();
    }

    /// <summary>
    /// Set items from a collection of strings
    /// </summary>
    public void SetItems(params string[] items)
    {
        Items = new ObservableCollection<string>(items);
    }
}
