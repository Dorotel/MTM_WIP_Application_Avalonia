using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Material.Icons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;

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
/// Manufacturing-grade collapsible panel with professional MTM styling and ResolutionIndependentSizing integration.
/// Features responsive design, touch-friendly controls, and comprehensive accessibility support.
/// Now inherits from ContentControl to eliminate TabItem styling inheritance while maintaining content properties.
/// </summary>
public partial class CollapsiblePanel : ContentControl
{
    // Styled properties
    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsiblePanel, bool>(nameof(IsExpanded), true);

    public static readonly StyledProperty<HeaderPosition> HeaderPositionProperty =
        AvaloniaProperty.Register<CollapsiblePanel, HeaderPosition>(nameof(HeaderPosition), HeaderPosition.Top);

    // Header property (ContentControl provides Content property)
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<CollapsiblePanel, object?>(nameof(Header));

    // Additional properties for styling compatibility
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty =
        AvaloniaProperty.Register<CollapsiblePanel, IBrush?>(nameof(HeaderBackground));

    public static readonly StyledProperty<IBrush?> ExpanderBackgroundProperty =
        AvaloniaProperty.Register<CollapsiblePanel, IBrush?>(nameof(ExpanderBackground));

    // ResolutionIndependentSizing integration
    private IResolutionIndependentSizingService? _sizingService;
    private ILogger<CollapsiblePanel>? _logger;

    // Properties
    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public HeaderPosition HeaderPosition
    {
        get => GetValue(HeaderPositionProperty);
        set => SetValue(HeaderPositionProperty, value);
    }

    // Header property (ContentControl provides Content property)
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    public IBrush? ExpanderBackground
    {
        get => GetValue(ExpanderBackgroundProperty);
        set => SetValue(ExpanderBackgroundProperty, value);
    }

    // Legacy property for backward compatibility
    public bool HeaderOnRight
    {
        get => HeaderPosition == HeaderPosition.Right;
        set => HeaderPosition = value ? HeaderPosition.Right : HeaderPosition.Left;
    }

    // Events
    public event EventHandler<bool>? ExpandedChanged;

    // Internal references for HeaderedContentControl template
    private Border? _contentArea;
    private Border? _headerArea;
    private Material.Icons.Avalonia.MaterialIcon? _toggleIcon;
    private Button? _toggleButton;
    private Grid? _rootGrid;
    private bool _isTemplateApplied = false;

    public CollapsiblePanel()
    {
        InitializeServices();
    }

    /// <summary>
    /// Initialize ResolutionIndependentSizingService and other services from DI container
    /// Uses proper MTM service access pattern via Program.GetOptionalService
    /// </summary>
    private void InitializeServices()
    {
        try
        {
            // MTM Pattern: Get services from Program static methods (proper dependency injection access)
            _sizingService = Program.GetOptionalService<IResolutionIndependentSizingService>();
            _logger = Program.GetOptionalService<ILogger<CollapsiblePanel>>();

            if (_sizingService != null)
            {
                _logger?.LogDebug("CollapsiblePanel initialized with ResolutionIndependentSizingService following MTM patterns");
                System.Diagnostics.Debug.WriteLine("CollapsiblePanel: Services initialized successfully");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CollapsiblePanel: ResolutionIndependentSizingService not available, using fallback sizing values");
            }
        }
        catch (Exception ex)
        {
            // If service resolution fails, continue without services (graceful degradation)
            System.Diagnostics.Debug.WriteLine($"CollapsiblePanel service initialization failed: {ex.Message}");
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from old button events if they exist
        if (_toggleButton != null)
        {
            _toggleButton.Click -= OnToggleClick;
        }

        // Get references to controls from the HeaderedContentControl template
        _contentArea = e.NameScope.Find<Border>("PART_ContentArea");
        _headerArea = e.NameScope.Find<Border>("PART_HeaderArea");
        _toggleIcon = e.NameScope.Find<Material.Icons.Avalonia.MaterialIcon>("PART_ToggleIcon");
        _toggleButton = e.NameScope.Find<Button>("PART_ToggleButton");
        _rootGrid = e.NameScope.Find<Grid>("PART_RootGrid");

        // Subscribe to new button events
        if (_toggleButton != null)
        {
            _toggleButton.Click += OnToggleClick;
        }

        _isTemplateApplied = true;

        // Set up initial layout and state
        UpdateLayout();      // Configure layout based on HeaderPosition
        UpdateDisplay();     // Set initial expanded/collapsed state
    }

    protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // If template wasn't applied yet, try to get references now
        if (!_isTemplateApplied)
        {
            _contentArea = this.FindControl<Border>("PART_ContentArea");
            _headerArea = this.FindControl<Border>("PART_HeaderArea");
            _toggleIcon = this.FindControl<Material.Icons.Avalonia.MaterialIcon>("PART_ToggleIcon");
            _toggleButton = this.FindControl<Button>("PART_ToggleButton");
            _rootGrid = this.FindControl<Grid>("PART_RootGrid");

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
        else if (change.Property == HeaderPositionProperty)
        {
            UpdateLayout();
        }
    }

    private new void UpdateLayout()
    {
        if (_rootGrid == null || _headerArea == null || _contentArea == null)
            return;

        // Clear existing row/column definitions and reset grid assignments
        _rootGrid.RowDefinitions.Clear();
        _rootGrid.ColumnDefinitions.Clear();

        Grid.SetRow(_headerArea, 0);
        Grid.SetColumn(_headerArea, 0);
        Grid.SetRow(_contentArea, 0);
        Grid.SetColumn(_contentArea, 0);

        switch (HeaderPosition)
        {
            case HeaderPosition.Left:
                SetupLeftHeader();
                // Content area gets right side rounded corners
                _contentArea.CornerRadius = new Avalonia.CornerRadius(0, 8, 8, 0);
                break;
            case HeaderPosition.Right:
                SetupRightHeader();
                // Content area gets left side rounded corners
                _contentArea.CornerRadius = new Avalonia.CornerRadius(8, 0, 0, 8);
                break;
            case HeaderPosition.Top:
                SetupTopHeader();
                // Content area gets bottom rounded corners
                _contentArea.CornerRadius = new Avalonia.CornerRadius(0, 0, 8, 8);
                break;
            case HeaderPosition.Bottom:
                SetupBottomHeader();
                // Content area gets top rounded corners
                _contentArea.CornerRadius = new Avalonia.CornerRadius(8, 8, 0, 0);
                break;
        }

        // Update button positioning
        UpdateButtonPositioning();

        // Ensure header maintains consistent size
        UpdateHeaderSize();

        // Update header corner radius based on current expanded state
        UpdateHeaderCornerRadius(!IsExpanded);
    }

    private void SetupLeftHeader()
    {
        // Root grid: Header | Content
        // Account for root border thickness (2px on each side = 4px total)
        _rootGrid!.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(30))); // 34 - 4 for border
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));

        if (_headerArea != null)
        {
            Grid.SetColumn(_headerArea, 0);
        }
        if (_contentArea != null)
        {
            Grid.SetColumn(_contentArea, 1);
        }

        if (_headerArea != null)
        {
            _headerArea.BorderThickness = new Avalonia.Thickness(0, 0, 1, 0);
            // Corner radius will be set by UpdateHeaderCornerRadius based on expanded state
            _headerArea.Width = 34;
            _headerArea.Height = double.NaN;
        }
    }

    private void SetupRightHeader()
    {
        // Root grid: Content | Header
        // Account for root border thickness (2px on each side = 4px total)
        _rootGrid!.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
        _rootGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(30))); // 34 - 4 for border

        if (_contentArea != null)
        {
            Grid.SetColumn(_contentArea, 0);
        }
        if (_headerArea != null)
        {
            Grid.SetColumn(_headerArea, 1);

            _headerArea.BorderThickness = new Avalonia.Thickness(1, 0, 0, 0);
            // Corner radius will be set by UpdateHeaderCornerRadius based on expanded state
            _headerArea.Width = 34;
            _headerArea.Height = double.NaN;
        }
    }

    private void SetupTopHeader()
    {
        // Root grid: Header / Content
        // Account for root border thickness (2px on each side = 4px total)
        _rootGrid!.RowDefinitions.Add(new RowDefinition(new GridLength(30))); // 34 - 4 for border
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));

        if (_headerArea != null)
        {
            Grid.SetRow(_headerArea, 0);
            _headerArea.BorderThickness = new Avalonia.Thickness(0, 0, 0, 1);
            // Corner radius will be set by UpdateHeaderCornerRadius based on expanded state
            _headerArea.Width = double.NaN;
            _headerArea.Height = 34;
        }
        if (_contentArea != null)
        {
            Grid.SetRow(_contentArea, 1);
        }
    }

    private void SetupBottomHeader()
    {
        // Root grid: Content / Header
        // Account for root border thickness (2px on each side = 4px total)
        _rootGrid!.RowDefinitions.Add(new RowDefinition(new GridLength(1, GridUnitType.Star)));
        _rootGrid.RowDefinitions.Add(new RowDefinition(new GridLength(30))); // 34 - 4 for border

        if (_contentArea != null)
        {
            Grid.SetRow(_contentArea, 0);
        }
        if (_headerArea != null)
        {
            Grid.SetRow(_headerArea, 1);

            _headerArea.BorderThickness = new Avalonia.Thickness(0, 1, 0, 0);
            // Corner radius will be set by UpdateHeaderCornerRadius based on expanded state
            _headerArea.Width = double.NaN;
            _headerArea.Height = 34;
        }
    }

    private void UpdateButtonPositioning()
    {
        if (_toggleButton == null)
            return;

        // Enhanced button centering for perfect alignment both vertically and horizontally
        _toggleButton.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        _toggleButton.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;

        switch (HeaderPosition)
        {
            case HeaderPosition.Left:
            case HeaderPosition.Right:
                // Button at bottom for vertical headers with perfect centering
                _toggleButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                _toggleButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
                _toggleButton.Margin = new Avalonia.Thickness(0, 0, 0, 8);
                break;

            case HeaderPosition.Top:
            case HeaderPosition.Bottom:
                // Button on left for horizontal headers with perfect centering
                _toggleButton.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                _toggleButton.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                _toggleButton.Margin = new Avalonia.Thickness(8, 0, 0, 0);
                break;
        }
    }

    private void UpdateDisplay()
    {
        if (_contentArea == null || _toggleIcon == null || _headerArea == null)
            return;

        // Update toggle icon and content visibility based on expanded state and header position
        if (IsExpanded)
        {
            _contentArea.IsVisible = true;
            _toggleIcon.Kind = GetCollapseIcon();

            // When expanded, use position-specific corner radius
            UpdateHeaderCornerRadius(false);
        }
        else
        {
            _contentArea.IsVisible = false;
            _toggleIcon.Kind = GetExpandIcon();

            // When collapsed, header should have all 4 corners rounded
            UpdateHeaderCornerRadius(true);
        }

        // Update the UserControl's dimensions based on expanded state and header position
        if (IsExpanded)
        {
            this.Width = double.NaN; // Auto width when expanded
            this.Height = double.NaN; // Auto height when expanded

            // Ensure header maintains consistent size
            UpdateHeaderSize();

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
            // Ensure header maintains consistent size even when collapsed
            UpdateHeaderSize();

            switch (HeaderPosition)
            {
                case HeaderPosition.Left:
                case HeaderPosition.Right:
                    this.Width = 34;
                    this.Height = double.NaN;
                    this.MinWidth = 34;
                    this.MinHeight = 100;
                    break;
                case HeaderPosition.Top:
                case HeaderPosition.Bottom:
                    this.Width = double.NaN;
                    this.Height = 34;
                    this.MinWidth = 200;
                    this.MinHeight = 34;
                    break;
            }
        }
    }

    private void UpdateHeaderCornerRadius(bool isCollapsed)
    {
        if (_headerArea == null)
            return;

        if (isCollapsed)
        {
            // When collapsed, header has all 4 corners rounded
            _headerArea.CornerRadius = new Avalonia.CornerRadius(8, 8, 8, 8);
        }
        else
        {
            // When expanded, use position-specific corner radius
            switch (HeaderPosition)
            {
                case HeaderPosition.Left:
                    _headerArea.CornerRadius = new Avalonia.CornerRadius(8, 0, 0, 8); // Left side rounded corners
                    break;
                case HeaderPosition.Right:
                    _headerArea.CornerRadius = new Avalonia.CornerRadius(0, 8, 8, 0); // Right side rounded corners
                    break;
                case HeaderPosition.Top:
                    _headerArea.CornerRadius = new Avalonia.CornerRadius(8, 8, 0, 0); // Top rounded corners
                    break;
                case HeaderPosition.Bottom:
                    _headerArea.CornerRadius = new Avalonia.CornerRadius(0, 0, 8, 8); // Bottom rounded corners
                    break;
            }
        }
    }

    private void UpdateHeaderSize()
    {
        if (_headerArea == null)
            return;

        // Header should maintain consistent size regardless of expanded/collapsed state
        switch (HeaderPosition)
        {
            case HeaderPosition.Left:
            case HeaderPosition.Right:
                // Always use consistent header width for vertical headers
                _headerArea.Width = 34; // Consistent 34px width
                _headerArea.Height = double.NaN; // Fill available height
                _headerArea.ClearValue(MaxWidthProperty);
                _headerArea.ClearValue(MaxHeightProperty);
                break;
            case HeaderPosition.Top:
            case HeaderPosition.Bottom:
                // Always use consistent header height for horizontal headers
                _headerArea.Width = double.NaN; // Fill available width
                _headerArea.Height = 34; // Consistent 34px height
                _headerArea.ClearValue(MaxWidthProperty);
                _headerArea.ClearValue(MaxHeightProperty);
                break;
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
