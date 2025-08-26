using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Material.Icons;

namespace MTM_WIP_Application_Avalonia.Controls;

/// <summary>
/// Defines the position of the header in a CollapsiblePanel
/// </summary>
public enum HeaderPosition
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// Collapsible panel with gold header bar and content area
/// </summary>
public partial class CollapsiblePanel : UserControl
{
    // Styled properties
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsiblePanel, bool>(nameof(IsExpanded), true);

    public static readonly StyledProperty<MaterialIconKind> IconProperty =
        AvaloniaProperty.Register<CollapsiblePanel, MaterialIconKind>(nameof(Icon), MaterialIconKind.FilterVariant);

    public static readonly StyledProperty<HeaderPosition> HeaderPositionProperty =
        AvaloniaProperty.Register<CollapsiblePanel, HeaderPosition>(nameof(HeaderPosition), HeaderPosition.Left);

    public static readonly StyledProperty<string> HeaderTextProperty =
        AvaloniaProperty.Register<CollapsiblePanel, string>(nameof(HeaderText), "Panel");

    // Properties
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public MaterialIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public HeaderPosition HeaderPosition
    {
        get => GetValue(HeaderPositionProperty);
        set => SetValue(HeaderPositionProperty, value);
    }

    public string HeaderText
    {
        get => GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    // Legacy property for backward compatibility
    public bool HeaderOnRight
    {
        get => HeaderPosition == HeaderPosition.Right;
        set => HeaderPosition = value ? HeaderPosition.Right : HeaderPosition.Left;
    }

    // Events
    public event EventHandler<bool>? ExpandedChanged;

    // Internal references
    private ContentPresenter? _contentPresenter;
    private Border? _contentArea;
    private Border? _headerArea;
    private Grid? _headerContentGrid;
    private TextBlock? _headerText;
    private Material.Icons.Avalonia.MaterialIcon? _panelIcon;
    private Material.Icons.Avalonia.MaterialIcon? _toggleIcon;
    private Grid? _rootGrid;
    private bool _isTemplateApplied = false;

    public CollapsiblePanel()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        
        // Get references to controls from the template
        _contentPresenter = e.NameScope.Find<ContentPresenter>("ContentPresenter");
        _contentArea = e.NameScope.Find<Border>("ContentArea");
        _headerArea = e.NameScope.Find<Border>("HeaderArea");
        _headerContentGrid = e.NameScope.Find<Grid>("HeaderContentGrid");
        _headerText = e.NameScope.Find<TextBlock>("HeaderText");
        _panelIcon = e.NameScope.Find<Material.Icons.Avalonia.MaterialIcon>("PanelIcon");
        _toggleIcon = e.NameScope.Find<Material.Icons.Avalonia.MaterialIcon>("ToggleIcon");
        _rootGrid = e.NameScope.Find<Grid>("RootGrid");
        
        _isTemplateApplied = true;
        
        // Set up initial layout and state
        UpdateIcon();        // Set the initial icon
        UpdateHeaderText();  // Set the initial header text
        UpdateLayout();      // Configure layout based on HeaderPosition
        UpdateDisplay();     // Set initial expanded/collapsed state
    }

    protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        
        // If template wasn't applied yet, try to get references now
        if (!_isTemplateApplied)
        {
            _contentPresenter = this.FindControl<ContentPresenter>("ContentPresenter");
            _contentArea = this.FindControl<Border>("ContentArea");
            _headerArea = this.FindControl<Border>("HeaderArea");
            _headerContentGrid = this.FindControl<Grid>("HeaderContentGrid");
            _panelIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("PanelIcon");
            _toggleIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("ToggleIcon");
            _rootGrid = this.FindControl<Grid>("RootGrid");
            
            UpdateLayout();
            UpdateDisplay();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsExpandedProperty)
        {
            UpdateDisplay();
            ExpandedChanged?.Invoke(this, IsExpanded);
        }
        else if (change.Property == IconProperty)
        {
            UpdateIcon();
        }
        else if (change.Property == HeaderPositionProperty)
        {
            UpdateLayout();
        }
        else if (change.Property == HeaderTextProperty)
        {
            UpdateHeaderText();
        }
    }

    private void UpdateLayout()
    {
        if (_rootGrid == null || _headerArea == null || _contentArea == null || _headerContentGrid == null)
            return;

        // Clear existing row/column definitions and reset grid assignments
        _rootGrid.RowDefinitions.Clear();
        _rootGrid.ColumnDefinitions.Clear();
        _headerContentGrid.RowDefinitions.Clear();
        _headerContentGrid.ColumnDefinitions.Clear();
        
        Grid.SetRow(_headerArea, 0);
        Grid.SetColumn(_headerArea, 0);
        Grid.SetRow(_contentArea, 0);
        Grid.SetColumn(_contentArea, 0);

        switch (HeaderPosition)
        {
            case HeaderPosition.Left:
                SetupLeftHeader();
                break;
            case HeaderPosition.Right:
                SetupRightHeader();
                break;
            case HeaderPosition.Top:
                SetupTopHeader();
                break;
            case HeaderPosition.Bottom:
                SetupBottomHeader();
                break;
        }

        // Update header element positioning
        UpdateHeaderElementPositioning();
    }

    private void SetupLeftHeader()
    {
        // Root grid: Header | Content
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(40)));
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        
        Grid.SetColumn(_headerArea, 0);
        Grid.SetColumn(_contentArea, 1);
        
        _headerArea.BorderThickness = new Avalonia.Thickness(0, 0, 1, 0);
        _headerArea.Width = 40;
        _headerArea.Height = double.NaN;

        // Header content grid is now fixed in AXAML - no need to modify definitions
    }

    private void SetupRightHeader()
    {
        // Root grid: Content | Header
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(40)));
        
        Grid.SetColumn(_contentArea, 0);
        Grid.SetColumn(_headerArea, 1);
        
        _headerArea.BorderThickness = new Avalonia.Thickness(1, 0, 0, 0);
        _headerArea.Width = 40;
        _headerArea.Height = double.NaN;

        // Header content grid is now fixed in AXAML - no need to modify definitions
    }

    private void SetupTopHeader()
    {
        // Root grid: Header / Content
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(40)));
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        
        Grid.SetRow(_headerArea, 0);
        Grid.SetRow(_contentArea, 1);
        
        _headerArea.BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);
        _headerArea.Width = double.NaN;
        _headerArea.Height = 40;

        // For horizontal headers, update the header grid to horizontal layout
        if (_headerContentGrid != null)
        {
            _headerContentGrid.RowDefinitions.Clear();
            _headerContentGrid.ColumnDefinitions.Clear();
            
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star))); // Text
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(24)));                   // Icon
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(24)));                   // Button
            
            // Update element positions for horizontal layout
            if (_headerText != null) Grid.SetColumn(_headerText, 0);
            if (_panelIcon != null) Grid.SetColumn(_panelIcon, 1);
            
            var button = _toggleIcon?.Parent as Button;
            if (button != null) Grid.SetColumn(button, 2);
            
            // Reset row positions for horizontal layout
            if (_headerText != null) Grid.SetRow(_headerText, 0);
            if (_panelIcon != null) Grid.SetRow(_panelIcon, 0);
            if (button != null) Grid.SetRow(button, 0);
        }
    }

    private void SetupBottomHeader()
    {
        // Root grid: Content / Header
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(40)));
        
        Grid.SetRow(_contentArea, 0);
        Grid.SetRow(_headerArea, 1);
        
        _headerArea.BorderThickness = new Avalonia.Thickness(0, 1, 0, 0);
        _headerArea.Width = double.NaN;
        _headerArea.Height = 40;

        // For horizontal headers, update the header grid to horizontal layout
        if (_headerContentGrid != null)
        {
            _headerContentGrid.RowDefinitions.Clear();
            _headerContentGrid.ColumnDefinitions.Clear();
            
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star))); // Text
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(24)));                   // Icon
            _headerContentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(24)));                   // Button
            
            // Update element positions for horizontal layout
            if (_headerText != null) Grid.SetColumn(_headerText, 0);
            if (_panelIcon != null) Grid.SetColumn(_panelIcon, 1);
            
            var button = _toggleIcon?.Parent as Button;
            if (button != null) Grid.SetColumn(button, 2);
            
            // Reset row positions for horizontal layout
            if (_headerText != null) Grid.SetRow(_headerText, 0);
            if (_panelIcon != null) Grid.SetRow(_panelIcon, 0);
            if (button != null) Grid.SetRow(button, 0);
        }
    }

    private void UpdateDisplay()
    {
        if (_contentArea == null || _toggleIcon == null)
            return;

        // Update toggle icon and content visibility based on expanded state and header position
        if (IsExpanded)
        {
            _contentArea.IsVisible = true;
            _toggleIcon.Kind = GetCollapseIcon();
        }
        else
        {
            _contentArea.IsVisible = false;
            _toggleIcon.Kind = GetExpandIcon();
        }
        
        // Update the UserControl's dimensions based on expanded state and header position
        if (IsExpanded)
        {
            this.Width = double.NaN; // Auto width when expanded
            this.Height = double.NaN; // Auto height when expanded
            
            switch (HeaderPosition)
            {
                case HeaderPosition.Left:
                case HeaderPosition.Right:
                    this.MinWidth = 300;
                    this.MinHeight = 100;
                    break;
                case HeaderPosition.Top:
                case HeaderPosition.Bottom:
                    this.MinWidth = 200;
                    this.MinHeight = 150;
                    break;
            }
        }
        else
        {
            switch (HeaderPosition)
            {
                case HeaderPosition.Left:
                case HeaderPosition.Right:
                    this.Width = 40;
                    this.Height = double.NaN;
                    this.MinWidth = 40;
                    this.MinHeight = 100;
                    break;
                case HeaderPosition.Top:
                case HeaderPosition.Bottom:
                    this.Width = double.NaN;
                    this.Height = 40;
                    this.MinWidth = 200;
                    this.MinHeight = 40;
                    break;
            }
        }
    }

    private MaterialIconKind GetExpandIcon()
    {
        return HeaderPosition switch
        {
            HeaderPosition.Left => MaterialIconKind.ChevronRight,
            HeaderPosition.Right => MaterialIconKind.ChevronLeft,
            HeaderPosition.Top => MaterialIconKind.ChevronDown,
            HeaderPosition.Bottom => MaterialIconKind.ChevronUp,
            _ => MaterialIconKind.ChevronRight
        };
    }

    private MaterialIconKind GetCollapseIcon()
    {
        return HeaderPosition switch
        {
            HeaderPosition.Left => MaterialIconKind.ChevronLeft,
            HeaderPosition.Right => MaterialIconKind.ChevronRight,
            HeaderPosition.Top => MaterialIconKind.ChevronUp,
            HeaderPosition.Bottom => MaterialIconKind.ChevronDown,
            _ => MaterialIconKind.ChevronLeft
        };
    }

    private void UpdateIcon()
    {
        if (_panelIcon != null)
        {
            _panelIcon.Kind = Icon;
        }
    }

    private void UpdateHeaderText()
    {
        if (_headerText != null)
        {
            _headerText.Text = HeaderText;
        }
    }

    private void UpdateHeaderElementPositioning()
    {
        if (_headerText == null || _panelIcon == null || _toggleIcon?.Parent is not Button toggleButton)
            return;

        // Header layout is now fixed in AXAML - just update text rotation based on header position
        switch (HeaderPosition)
        {
            case HeaderPosition.Left:
            case HeaderPosition.Right:
                // Vertical headers: Rotate text -90 degrees (sideways, top to bottom reading)
                _headerText.RenderTransform = new Avalonia.Media.RotateTransform(-90);
                _headerText.Margin = new Avalonia.Thickness(0, 8, 0, 4);
                break;

            case HeaderPosition.Top:
            case HeaderPosition.Bottom:
                // Horizontal headers: No rotation, normal text
                _headerText.RenderTransform = null;
                _headerText.Margin = new Avalonia.Thickness(4, 0, 4, 0);
                break;
        }
    }

    private void OnToggleClick(object? sender, RoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
        // UpdateDisplay and event will be called via PropertyChanged
    }

    /// <summary>
    /// Programmatically expand or collapse the panel
    /// </summary>
    public void SetExpanded(bool expanded, bool raiseEvent = true)
    {
        if (IsExpanded != expanded)
        {
            IsExpanded = expanded;
            // UpdateDisplay and event will be called via PropertyChanged if raiseEvent is true
            
            if (!raiseEvent)
            {
                // Temporarily disable events
                var oldHandler = ExpandedChanged;
                ExpandedChanged = null;
                IsExpanded = expanded;
                ExpandedChanged = oldHandler;
            }
        }
    }
}
