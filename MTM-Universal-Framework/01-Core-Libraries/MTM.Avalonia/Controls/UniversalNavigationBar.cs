using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Navigation Bar with customizable menu items and responsive design.
    /// Provides consistent navigation experience across platforms with collapsible mobile support.
    /// </summary>
    public class UniversalNavigationBar : UserControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, string>(nameof(Title), string.Empty);

        public static readonly StyledProperty<IEnumerable> MenuItemsProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, IEnumerable>(nameof(MenuItems));

        public static readonly StyledProperty<bool> IsMobileProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, bool>(nameof(IsMobile), false);

        public static readonly StyledProperty<bool> IsExpandedProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, bool>(nameof(IsExpanded), true);

        public static readonly StyledProperty<object?> LogoContentProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, object?>(nameof(LogoContent));

        public static readonly StyledProperty<object?> ActionContentProperty =
            AvaloniaProperty.Register<UniversalNavigationBar, object?>(nameof(ActionContent));

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public IEnumerable MenuItems
        {
            get => GetValue(MenuItemsProperty);
            set => SetValue(MenuItemsProperty, value);
        }

        public bool IsMobile
        {
            get => GetValue(IsMobileProperty);
            set => SetValue(IsMobileProperty, value);
        }

        public bool IsExpanded
        {
            get => GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public object? LogoContent
        {
            get => GetValue(LogoContentProperty);
            set => SetValue(LogoContentProperty, value);
        }

        public object? ActionContent
        {
            get => GetValue(ActionContentProperty);
            set => SetValue(ActionContentProperty, value);
        }

        public UniversalNavigationBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new Border
            {
                Background = Avalonia.Media.Brushes.White,
                BorderBrush = Avalonia.Media.Brushes.LightGray,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Child = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
                    Margin = new Thickness(16, 12),
                    Children =
                    {
                        new ContentPresenter
                        {
                            [Grid.ColumnProperty] = 0,
                            [!ContentPresenter.ContentProperty] = this.GetObservable(LogoContentProperty).ToBinding()
                        },
                        new TextBlock
                        {
                            [Grid.ColumnProperty] = 1,
                            [!TextBlock.TextProperty] = this.GetObservable(TitleProperty).ToBinding(),
                            FontSize = 18,
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(16, 0)
                        },
                        new ContentPresenter
                        {
                            [Grid.ColumnProperty] = 2,
                            [!ContentPresenter.ContentProperty] = this.GetObservable(ActionContentProperty).ToBinding()
                        }
                    }
                }
            };
        }
    }
}