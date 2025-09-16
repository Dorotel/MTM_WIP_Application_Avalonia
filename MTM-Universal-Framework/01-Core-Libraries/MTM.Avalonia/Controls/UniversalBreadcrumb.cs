using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;
using System.Windows.Input;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Breadcrumb navigation with customizable separators and click handling.
    /// Provides consistent breadcrumb experience with responsive design for mobile.
    /// </summary>
    public class UniversalBreadcrumb : UserControl
    {
        public static readonly StyledProperty<IEnumerable> ItemsProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, IEnumerable>(nameof(Items));

        public static readonly StyledProperty<string> SeparatorProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, string>(nameof(Separator), " / ");

        public static readonly StyledProperty<ICommand> ItemClickCommandProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, ICommand>(nameof(ItemClickCommand));

        public static readonly StyledProperty<bool> ShowHomeIconProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, bool>(nameof(ShowHomeIcon), true);

        public static readonly StyledProperty<int> MaxVisibleItemsProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, int>(nameof(MaxVisibleItems), 5);

        public static readonly StyledProperty<bool> IsCompactProperty =
            AvaloniaProperty.Register<UniversalBreadcrumb, bool>(nameof(IsCompact), false);

        public IEnumerable Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public string Separator
        {
            get => GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        public ICommand ItemClickCommand
        {
            get => GetValue(ItemClickCommandProperty);
            set => SetValue(ItemClickCommandProperty, value);
        }

        public bool ShowHomeIcon
        {
            get => GetValue(ShowHomeIconProperty);
            set => SetValue(ShowHomeIconProperty, value);
        }

        public int MaxVisibleItems
        {
            get => GetValue(MaxVisibleItemsProperty);
            set => SetValue(MaxVisibleItemsProperty, value);
        }

        public bool IsCompact
        {
            get => GetValue(IsCompactProperty);
            set => SetValue(IsCompactProperty, value);
        }

        public UniversalBreadcrumb()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new Border
            {
                Background = Avalonia.Media.Brushes.Transparent,
                Padding = new Thickness(8, 4),
                Child = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 4,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "🏠",
                            FontSize = 16,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 8, 0),
                            [!TextBlock.IsVisibleProperty] = this.GetObservable(ShowHomeIconProperty).ToBinding()
                        },
                        new ItemsControl
                        {
                            [!ItemsControl.ItemsSourceProperty] = this.GetObservable(ItemsProperty).ToBinding(),
                            ItemTemplate = new FuncDataTemplate<object>((item, nameScope) =>
                            {
                                return new StackPanel
                                {
                                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                                    Children =
                                    {
                                        new Button
                                        {
                                            Content = item?.ToString(),
                                            Background = Avalonia.Media.Brushes.Transparent,
                                            BorderThickness = new Thickness(0),
                                            Foreground = Avalonia.Media.Brushes.Blue,
                                            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                                            [!Button.CommandProperty] = this.GetObservable(ItemClickCommandProperty).ToBinding(),
                                            CommandParameter = item
                                        },
                                        new TextBlock
                                        {
                                            [!TextBlock.TextProperty] = this.GetObservable(SeparatorProperty).ToBinding(),
                                            VerticalAlignment = VerticalAlignment.Center,
                                            Foreground = Avalonia.Media.Brushes.Gray
                                        }
                                    }
                                };
                            })
                        }
                    }
                }
            };
        }
    }
}