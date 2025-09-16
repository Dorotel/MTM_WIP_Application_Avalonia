using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls
{
    /// <summary>
    /// Universal Search Box with auto-complete suggestions and clear functionality.
    /// Provides consistent search experience with real-time filtering and mobile optimization.
    /// </summary>
    public class UniversalSearchBox : UserControl
    {
        public static readonly StyledProperty<string> QueryProperty =
            AvaloniaProperty.Register<UniversalSearchBox, string>(nameof(Query), string.Empty);

        public static readonly StyledProperty<string> PlaceholderProperty =
            AvaloniaProperty.Register<UniversalSearchBox, string>(nameof(Placeholder), "Search...");

        public static readonly StyledProperty<IEnumerable> SuggestionsProperty =
            AvaloniaProperty.Register<UniversalSearchBox, IEnumerable>(nameof(Suggestions));

        public static readonly StyledProperty<bool> ShowSuggestionsProperty =
            AvaloniaProperty.Register<UniversalSearchBox, bool>(nameof(ShowSuggestions), false);

        public static readonly StyledProperty<bool> ShowClearButtonProperty =
            AvaloniaProperty.Register<UniversalSearchBox, bool>(nameof(ShowClearButton), true);

        public static readonly StyledProperty<bool> ShowSearchIconProperty =
            AvaloniaProperty.Register<UniversalSearchBox, bool>(nameof(ShowSearchIcon), true);

        public static readonly StyledProperty<int> MaxSuggestionsProperty =
            AvaloniaProperty.Register<UniversalSearchBox, int>(nameof(MaxSuggestions), 10);

        public string Query
        {
            get => GetValue(QueryProperty);
            set => SetValue(QueryProperty, value);
        }

        public string Placeholder
        {
            get => GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public IEnumerable Suggestions
        {
            get => GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

        public bool ShowSuggestions
        {
            get => GetValue(ShowSuggestionsProperty);
            set => SetValue(ShowSuggestionsProperty, value);
        }

        public bool ShowClearButton
        {
            get => GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }

        public bool ShowSearchIcon
        {
            get => GetValue(ShowSearchIconProperty);
            set => SetValue(ShowSearchIconProperty, value);
        }

        public int MaxSuggestions
        {
            get => GetValue(MaxSuggestionsProperty);
            set => SetValue(MaxSuggestionsProperty, value);
        }

        public UniversalSearchBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Content = new Grid
            {
                Children =
                {
                    new Border
                    {
                        Background = Brushes.White,
                        BorderBrush = Brushes.Gray,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(20),
                        Child = new Grid
                        {
                            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
                            Children =
                            {
                                new TextBlock
                                {
                                    [Grid.ColumnProperty] = 0,
                                    Text = "🔍",
                                    FontSize = 16,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(12, 0, 8, 0),
                                    [!TextBlock.IsVisibleProperty] = this.GetObservable(ShowSearchIconProperty).ToBinding()
                                },
                                new TextBox
                                {
                                    [Grid.ColumnProperty] = 1,
                                    [!TextBox.TextProperty] = this.GetObservable(QueryProperty).ToBinding(),
                                    [!TextBox.WatermarkProperty] = this.GetObservable(PlaceholderProperty).ToBinding(),
                                    BorderThickness = new Thickness(0),
                                    Background = Brushes.Transparent,
                                    VerticalAlignment = VerticalAlignment.Center,
                                    Margin = new Thickness(4, 8)
                                },
                                new Button
                                {
                                    [Grid.ColumnProperty] = 2,
                                    Content = "✕",
                                    Background = Brushes.Transparent,
                                    BorderThickness = new Thickness(0),
                                    FontSize = 12,
                                    Margin = new Thickness(8, 0, 12, 0),
                                    [!Button.IsVisibleProperty] = this.GetObservable(ShowClearButtonProperty).ToBinding()
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}