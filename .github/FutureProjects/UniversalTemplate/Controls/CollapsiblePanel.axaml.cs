using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Material.Icons;

namespace YourProject.Controls
{
    /// <summary>
    /// Universal collapsible panel control for Avalonia applications.
    /// Provides expandable/collapsible content areas with customizable header positioning.
    /// Extracted from MTM WIP Application for reuse in new projects.
    /// </summary>
    public partial class CollapsiblePanel : UserControl
    {
        /// <summary>
        /// Defines the Header property
        /// </summary>
        public static readonly StyledProperty<object?> HeaderProperty =
            AvaloniaProperty.Register<CollapsiblePanel, object?>(nameof(Header));

        /// <summary>
        /// Defines the IsExpanded property
        /// </summary>
        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<CollapsiblePanel, bool>(nameof(IsExpanded), defaultValue: true);

        /// <summary>
        /// Defines the HeaderPosition property
        /// </summary>
        public static readonly StyledProperty<HeaderPosition> HeaderPositionProperty =
            AvaloniaProperty.Register<CollapsiblePanel, HeaderPosition>(nameof(HeaderPosition), defaultValue: HeaderPosition.Top);

        /// <summary>
        /// Gets or sets the header content
        /// </summary>
        public object? Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the panel is expanded
        /// </summary>
        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Gets or sets the position of the header (Top, Bottom, Left, Right)
        /// </summary>
        public HeaderPosition HeaderPosition
        {
            get => GetValue(HeaderPositionProperty);
            set => SetValue(HeaderPositionProperty, value);
        }

        /// <summary>
        /// Occurs when the expansion state changes
        /// </summary>
        public event EventHandler<bool>? ExpandedChanged;

        public CollapsiblePanel()
        {
            InitializeComponent();
            
            // Subscribe to property changes
            IsExpandedProperty.Changed.Subscribe(OnIsExpandedChanged);
            HeaderPositionProperty.Changed.Subscribe(OnHeaderPositionChanged);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            
            // Update initial state
            UpdateExpandedState();
            UpdateHeaderPosition();
        }

        /// <summary>
        /// Handles the toggle button click
        /// </summary>
        private void OnToggleClick(object? sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        /// <summary>
        /// Handles changes to the IsExpanded property
        /// </summary>
        private void OnIsExpandedChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isExpanded)
            {
                UpdateExpandedState();
                ExpandedChanged?.Invoke(this, isExpanded);
            }
        }

        /// <summary>
        /// Handles changes to the HeaderPosition property
        /// </summary>
        private void OnHeaderPositionChanged(AvaloniaPropertyChangedEventArgs e)
        {
            UpdateHeaderPosition();
        }

        /// <summary>
        /// Updates the visual state based on the IsExpanded property
        /// </summary>
        private void UpdateExpandedState()
        {
            if (Template?.FindNameScope(this)?.Find("ContentArea") is Border contentArea &&
                Template?.FindNameScope(this)?.Find("ToggleIcon") is Material.Icons.Avalonia.MaterialIcon toggleIcon)
            {
                contentArea.IsVisible = IsExpanded;
                
                // Update toggle icon based on header position and expanded state
                toggleIcon.Kind = HeaderPosition switch
                {
                    HeaderPosition.Top => IsExpanded ? MaterialIconKind.ChevronDown : MaterialIconKind.ChevronRight,
                    HeaderPosition.Bottom => IsExpanded ? MaterialIconKind.ChevronUp : MaterialIconKind.ChevronRight,
                    HeaderPosition.Left => IsExpanded ? MaterialIconKind.ChevronRight : MaterialIconKind.ChevronDown,
                    HeaderPosition.Right => IsExpanded ? MaterialIconKind.ChevronLeft : MaterialIconKind.ChevronDown,
                    _ => MaterialIconKind.ChevronRight
                };
            }
        }

        /// <summary>
        /// Updates the layout based on the HeaderPosition property
        /// </summary>
        private void UpdateHeaderPosition()
        {
            if (Template?.FindNameScope(this)?.Find("RootGrid") is Grid rootGrid &&
                Template?.FindNameScope(this)?.Find("HeaderArea") is Border headerArea &&
                Template?.FindNameScope(this)?.Find("ContentArea") is Border contentArea)
            {
                // Clear existing layout
                rootGrid.RowDefinitions.Clear();
                rootGrid.ColumnDefinitions.Clear();
                Grid.SetRow(headerArea, 0);
                Grid.SetColumn(headerArea, 0);
                Grid.SetRow(contentArea, 0);
                Grid.SetColumn(contentArea, 0);

                // Set up layout based on header position
                switch (HeaderPosition)
                {
                    case HeaderPosition.Top:
                        rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                        rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                        Grid.SetRow(headerArea, 0);
                        Grid.SetRow(contentArea, 1);
                        break;

                    case HeaderPosition.Bottom:
                        rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
                        rootGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                        Grid.SetRow(contentArea, 0);
                        Grid.SetRow(headerArea, 1);
                        break;

                    case HeaderPosition.Left:
                        rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                        rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                        Grid.SetColumn(headerArea, 0);
                        Grid.SetColumn(contentArea, 1);
                        break;

                    case HeaderPosition.Right:
                        rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
                        rootGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                        Grid.SetColumn(contentArea, 0);
                        Grid.SetColumn(headerArea, 1);
                        break;
                }

                UpdateExpandedState();
            }
        }
    }

    /// <summary>
    /// Enumeration for header position options
    /// </summary>
    public enum HeaderPosition
    {
        Top,
        Bottom,
        Left,
        Right
    }
}