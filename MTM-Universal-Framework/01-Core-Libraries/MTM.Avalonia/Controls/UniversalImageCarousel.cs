using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.ObjectModel;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal image carousel control with touch and mouse navigation support.
/// Optimized for cross-platform image display and navigation.
/// </summary>
public class UniversalImageCarousel : UserControl
{
    public static readonly StyledProperty<ObservableCollection<CarouselItem>> ItemsProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, ObservableCollection<CarouselItem>>(nameof(Items));

    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, int>(nameof(SelectedIndex), 0,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> ShowIndicatorsProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, bool>(nameof(ShowIndicators), true);

    public static readonly StyledProperty<bool> ShowNavigationButtonsProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, bool>(nameof(ShowNavigationButtons), true);

    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, bool>(nameof(AutoPlay), false);

    public static readonly StyledProperty<TimeSpan> AutoPlayIntervalProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, TimeSpan>(nameof(AutoPlayInterval), TimeSpan.FromSeconds(5));

    public static readonly StyledProperty<Stretch> ImageStretchProperty =
        AvaloniaProperty.Register<UniversalImageCarousel, Stretch>(nameof(ImageStretch), Stretch.UniformToFill);

    /// <summary>
    /// Gets or sets the collection of carousel items.
    /// </summary>
    public ObservableCollection<CarouselItem> Items
    {
        get => GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected item index.
    /// </summary>
    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show page indicators.
    /// </summary>
    public bool ShowIndicators
    {
        get => GetValue(ShowIndicatorsProperty);
        set => SetValue(ShowIndicatorsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show navigation buttons.
    /// </summary>
    public bool ShowNavigationButtons
    {
        get => GetValue(ShowNavigationButtonsProperty);
        set => SetValue(ShowNavigationButtonsProperty, value);
    }

    /// <summary>
    /// Gets or sets whether auto-play is enabled.
    /// </summary>
    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Gets or sets the auto-play interval.
    /// </summary>
    public TimeSpan AutoPlayInterval
    {
        get => GetValue(AutoPlayIntervalProperty);
        set => SetValue(AutoPlayIntervalProperty, value);
    }

    /// <summary>
    /// Gets or sets the image stretch mode.
    /// </summary>
    public Stretch ImageStretch
    {
        get => GetValue(ImageStretchProperty);
        set => SetValue(ImageStretchProperty, value);
    }

    private Carousel? _carousel;
    private StackPanel? _indicatorsPanel;
    private System.Threading.Timer? _autoPlayTimer;

    static UniversalImageCarousel()
    {
        BackgroundProperty.OverrideDefaultValue<UniversalImageCarousel>(Brushes.Black);
        BorderThicknessProperty.OverrideDefaultValue<UniversalImageCarousel>(new Thickness(1));
        BorderBrushProperty.OverrideDefaultValue<UniversalImageCarousel>(Brushes.Gray);
        CornerRadiusProperty.OverrideDefaultValue<UniversalImageCarousel>(new CornerRadius(4));

        ItemsProperty.Changed.AddClassHandler<UniversalImageCarousel>((carousel, args) =>
        {
            carousel.UpdateCarousel();
        });

        SelectedIndexProperty.Changed.AddClassHandler<UniversalImageCarousel>((carousel, args) =>
        {
            carousel.UpdateSelectedIndex();
        });

        AutoPlayProperty.Changed.AddClassHandler<UniversalImageCarousel>((carousel, args) =>
        {
            carousel.UpdateAutoPlay();
        });
    }

    public UniversalImageCarousel()
    {
        Classes.Add("universal-image-carousel");
        Items = new ObservableCollection<CarouselItem>();
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var mainGrid = new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto")
        };

        // Carousel container with navigation
        var carouselContainer = CreateCarouselContainer();
        Grid.SetRow(carouselContainer, 0);
        mainGrid.Children.Add(carouselContainer);

        // Indicators
        if (ShowIndicators)
        {
            _indicatorsPanel = CreateIndicatorsPanel();
            Grid.SetRow(_indicatorsPanel, 1);
            mainGrid.Children.Add(_indicatorsPanel);
        }

        Content = mainGrid;
    }

    private Panel CreateCarouselContainer()
    {
        var container = new Grid();

        // Main carousel
        _carousel = new Carousel
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        _carousel.SelectionChanged += OnCarouselSelectionChanged;
        container.Children.Add(_carousel);

        // Navigation buttons
        if (ShowNavigationButtons)
        {
            var navGrid = CreateNavigationButtons();
            container.Children.Add(navGrid);
        }

        return container;
    }

    private Grid CreateNavigationButtons()
    {
        var navGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto")
        };

        // Previous button
        var prevButton = new Button
        {
            Content = "◀",
            FontSize = 20,
            Width = 44,
            Height = 44,
            CornerRadius = new CornerRadius(22),
            Background = new SolidColorBrush(Colors.Black, 0.5),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0)
        };
        prevButton.Click += (s, e) => NavigatePrevious();
        Grid.SetColumn(prevButton, 0);
        navGrid.Children.Add(prevButton);

        // Next button
        var nextButton = new Button
        {
            Content = "▶",
            FontSize = 20,
            Width = 44,
            Height = 44,
            CornerRadius = new CornerRadius(22),
            Background = new SolidColorBrush(Colors.Black, 0.5),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };
        nextButton.Click += (s, e) => NavigateNext();
        Grid.SetColumn(nextButton, 2);
        navGrid.Children.Add(nextButton);

        return navGrid;
    }

    private StackPanel CreateIndicatorsPanel()
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 8,
            Margin = new Thickness(0, 8),
            Background = new SolidColorBrush(Colors.Black, 0.3)
        };
    }

    private void UpdateCarousel()
    {
        if (_carousel == null || Items == null) return;

        _carousel.Items.Clear();

        foreach (var item in Items)
        {
            var imageControl = CreateImageControl(item);
            _carousel.Items.Add(imageControl);
        }

        UpdateIndicators();
    }

    private Control CreateImageControl(CarouselItem item)
    {
        var container = new Grid();

        // Main image
        var image = new Image
        {
            Source = item.ImageSource,
            Stretch = ImageStretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        container.Children.Add(image);

        // Caption overlay
        if (!string.IsNullOrEmpty(item.Caption) || !string.IsNullOrEmpty(item.Description))
        {
            var captionOverlay = CreateCaptionOverlay(item);
            container.Children.Add(captionOverlay);
        }

        return container;
    }

    private Border CreateCaptionOverlay(CarouselItem item)
    {
        var captionPanel = new StackPanel
        {
            Spacing = 4,
            Margin = new Thickness(12)
        };

        if (!string.IsNullOrEmpty(item.Caption))
        {
            captionPanel.Children.Add(new TextBlock
            {
                Text = item.Caption,
                FontSize = 16,
                FontWeight = FontWeight.Bold,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            });
        }

        if (!string.IsNullOrEmpty(item.Description))
        {
            captionPanel.Children.Add(new TextBlock
            {
                Text = item.Description,
                FontSize = 12,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            });
        }

        return new Border
        {
            Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop { Offset = 0, Color = Colors.Transparent },
                    new GradientStop { Offset = 1, Color = Color.FromArgb(180, 0, 0, 0) }
                }
            },
            Child = captionPanel,
            VerticalAlignment = VerticalAlignment.Bottom
        };
    }

    private void UpdateIndicators()
    {
        if (_indicatorsPanel == null || Items == null) return;

        _indicatorsPanel.Children.Clear();

        for (int i = 0; i < Items.Count; i++)
        {
            var indicator = CreateIndicator(i);
            _indicatorsPanel.Children.Add(indicator);
        }

        UpdateIndicatorStates();
    }

    private Button CreateIndicator(int index)
    {
        var indicator = new Button
        {
            Width = 12,
            Height = 12,
            CornerRadius = new CornerRadius(6),
            BorderThickness = new Thickness(0),
            Background = Brushes.LightGray,
            Margin = new Thickness(2)
        };

        indicator.Click += (s, e) => SelectedIndex = index;
        return indicator;
    }

    private void UpdateSelectedIndex()
    {
        if (_carousel != null && SelectedIndex >= 0 && SelectedIndex < (_carousel.Items?.Count ?? 0))
        {
            _carousel.SelectedIndex = SelectedIndex;
        }

        UpdateIndicatorStates();
    }

    private void UpdateIndicatorStates()
    {
        if (_indicatorsPanel == null) return;

        for (int i = 0; i < _indicatorsPanel.Children.Count; i++)
        {
            if (_indicatorsPanel.Children[i] is Button indicator)
            {
                indicator.Background = i == SelectedIndex ? Brushes.White : Brushes.LightGray;
            }
        }
    }

    private void UpdateAutoPlay()
    {
        _autoPlayTimer?.Dispose();

        if (AutoPlay && Items?.Count > 1)
        {
            _autoPlayTimer = new System.Threading.Timer(
                _ => Avalonia.Threading.Dispatcher.UIThread.Post(NavigateNext),
                null, AutoPlayInterval, AutoPlayInterval);
        }
    }

    public void NavigateNext()
    {
        if (Items?.Count > 0)
        {
            SelectedIndex = (SelectedIndex + 1) % Items.Count;
        }
    }

    public void NavigatePrevious()
    {
        if (Items?.Count > 0)
        {
            SelectedIndex = SelectedIndex > 0 ? SelectedIndex - 1 : Items.Count - 1;
        }
    }

    private void OnCarouselSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_carousel != null)
        {
            SelectedIndex = _carousel.SelectedIndex;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _autoPlayTimer?.Dispose();
        base.OnDetachedFromVisualTree(e);
    }
}

/// <summary>
/// Represents an item in the image carousel.
/// </summary>
public class CarouselItem
{
    public IImage? ImageSource { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public object? Tag { get; set; }
}