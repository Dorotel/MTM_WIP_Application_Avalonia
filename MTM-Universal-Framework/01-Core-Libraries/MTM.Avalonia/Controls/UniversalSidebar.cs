using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Sidebar with collapsible navigation and adaptive width.
    /// Provides consistent side navigation experience with touch-friendly mobile support.
    /// </summary>
    public class UniversalSidebar : UserControl
    {
        public static readonly StyledProperty<IEnumerable> NavigationItemsProperty =
            AvaloniaProperty.Register<UniversalSidebar, IEnumerable>(nameof(NavigationItems));

        public static readonly StyledProperty<bool> IsCollapsedProperty =
            AvaloniaProperty.Register<UniversalSidebar, bool>(nameof(IsCollapsed), false);

        public static readonly StyledProperty<double> ExpandedWidthProperty =
            AvaloniaProperty.Register<UniversalSidebar, double>(nameof(ExpandedWidth), 250);

        public static readonly StyledProperty<double> CollapsedWidthProperty =
            AvaloniaProperty.Register<UniversalSidebar, double>(nameof(CollapsedWidth), 60);

        public static readonly StyledProperty<object?> HeaderContentProperty =
            AvaloniaProperty.Register<UniversalSidebar, object?>(nameof(HeaderContent));

        public static readonly StyledProperty<object?> FooterContentProperty =
            AvaloniaProperty.Register<UniversalSidebar, object?>(nameof(FooterContent));

        public IEnumerable NavigationItems
        {
            get => GetValue(NavigationItemsProperty);
            set => SetValue(NavigationItemsProperty, value);
        }

        public bool IsCollapsed
        {
            get => GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }

        public double ExpandedWidth
        {
            get => GetValue(ExpandedWidthProperty);
            set => SetValue(ExpandedWidthProperty, value);
        }

        public double CollapsedWidth
        {
            get => GetValue(CollapsedWidthProperty);
            set => SetValue(CollapsedWidthProperty, value);
        }

        public object? HeaderContent
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        public object? FooterContent
        {
            get => GetValue(FooterContentProperty);
            set => SetValue(FooterContentProperty, value);
        }

        public UniversalSidebar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new Border
            {
                Background = Avalonia.Media.Brushes.WhiteSmoke,
                BorderBrush = Avalonia.Media.Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 1, 0),
                [!Border.WidthProperty] = this.GetObservable(IsCollapsedProperty)
                    .Select(collapsed => collapsed ? CollapsedWidth : ExpandedWidth)
                    .ToBinding(),
                Child = new Grid
                {
                    RowDefinitions = new RowDefinitions("Auto,*,Auto"),
                    Children =
                    {
                        new ContentPresenter
                        {
                            [Grid.RowProperty] = 0,
                            [!ContentPresenter.ContentProperty] = this.GetObservable(HeaderContentProperty).ToBinding(),
                            Margin = new Thickness(16)
                        },
                        new ScrollViewer
                        {
                            [Grid.RowProperty] = 1,
                            Content = new ItemsControl
                            {
                                [!ItemsControl.ItemsSourceProperty] = this.GetObservable(NavigationItemsProperty).ToBinding(),
                                Margin = new Thickness(8)
                            }
                        },
                        new ContentPresenter
                        {
                            [Grid.RowProperty] = 2,
                            [!ContentPresenter.ContentProperty] = this.GetObservable(FooterContentProperty).ToBinding(),
                            Margin = new Thickness(16)
                        }
                    }
                }
            };
        }
    }
}