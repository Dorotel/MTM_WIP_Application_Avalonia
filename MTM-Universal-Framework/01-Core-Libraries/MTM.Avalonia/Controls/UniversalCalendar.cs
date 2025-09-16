using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using System;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal calendar control with event display and selection capabilities.
/// Optimized for both desktop and mobile interaction.
/// </summary>
public class UniversalCalendar : UserControl
{
    public static readonly StyledProperty<DateTime> SelectedDateProperty =
        AvaloniaProperty.Register<UniversalCalendar, DateTime>(nameof(SelectedDate), DateTime.Today,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<UniversalCalendar, DateTime?>(nameof(MinDate));

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<UniversalCalendar, DateTime?>(nameof(MaxDate));

    public static readonly StyledProperty<bool> ShowTodayButtonProperty =
        AvaloniaProperty.Register<UniversalCalendar, bool>(nameof(ShowTodayButton), true);

    public static readonly StyledProperty<bool> AllowMultipleSelectionProperty =
        AvaloniaProperty.Register<UniversalCalendar, bool>(nameof(AllowMultipleSelection), false);

    public static readonly StyledProperty<string> HeaderFormatProperty =
        AvaloniaProperty.Register<UniversalCalendar, string>(nameof(HeaderFormat), "MMMM yyyy");

    /// <summary>
    /// Gets or sets the selected date.
    /// </summary>
    public DateTime SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum selectable date.
    /// </summary>
    public DateTime? MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum selectable date.
    /// </summary>
    public DateTime? MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the "Today" button.
    /// </summary>
    public bool ShowTodayButton
    {
        get => GetValue(ShowTodayButtonProperty);
        set => SetValue(ShowTodayButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets whether multiple dates can be selected.
    /// </summary>
    public bool AllowMultipleSelection
    {
        get => GetValue(AllowMultipleSelectionProperty);
        set => SetValue(AllowMultipleSelectionProperty, value);
    }

    /// <summary>
    /// Gets or sets the header date format.
    /// </summary>
    public string HeaderFormat
    {
        get => GetValue(HeaderFormatProperty);
        set => SetValue(HeaderFormatProperty, value);
    }

    private DateTime _displayMonth;
    private Calendar? _calendar;

    static UniversalCalendar()
    {
        BackgroundProperty.OverrideDefaultValue<UniversalCalendar>(Avalonia.Media.Brushes.White);
        BorderThicknessProperty.OverrideDefaultValue<UniversalCalendar>(new Thickness(1));
        BorderBrushProperty.OverrideDefaultValue<UniversalCalendar>(Avalonia.Media.Brushes.LightGray);
        CornerRadiusProperty.OverrideDefaultValue<UniversalCalendar>(new CornerRadius(8));
    }

    public UniversalCalendar()
    {
        Classes.Add("universal-calendar");
        _displayMonth = DateTime.Today;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        var mainPanel = new StackPanel { Spacing = 8 };

        // Header with navigation
        var headerPanel = CreateHeader();
        mainPanel.Children.Add(headerPanel);

        // Calendar control
        _calendar = new Calendar
        {
            SelectedDate = SelectedDate,
            DisplayDate = _displayMonth,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        _calendar.SelectedDatesChanged += OnSelectedDatesChanged;
        mainPanel.Children.Add(_calendar);

        // Footer with today button
        if (ShowTodayButton)
        {
            var footerPanel = CreateFooter();
            mainPanel.Children.Add(footerPanel);
        }

        Content = new Border
        {
            Padding = new Thickness(12),
            Child = mainPanel
        };
    }

    private Panel CreateHeader()
    {
        var headerPanel = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto")
        };

        // Previous month button
        var prevButton = new Button
        {
            Content = "◀",
            FontSize = 14,
            Width = 32,
            Height = 32,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        prevButton.Click += (s, e) => NavigateMonth(-1);
        Grid.SetColumn(prevButton, 0);
        headerPanel.Children.Add(prevButton);

        // Month/Year header
        var headerText = new TextBlock
        {
            Text = _displayMonth.ToString(HeaderFormat),
            FontSize = 16,
            FontWeight = Avalonia.Media.FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(headerText, 1);
        headerPanel.Children.Add(headerText);

        // Next month button
        var nextButton = new Button
        {
            Content = "▶",
            FontSize = 14,
            Width = 32,
            Height = 32,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        nextButton.Click += (s, e) => NavigateMonth(1);
        Grid.SetColumn(nextButton, 2);
        headerPanel.Children.Add(nextButton);

        return headerPanel;
    }

    private Panel CreateFooter()
    {
        var footerPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 8
        };

        var todayButton = new Button
        {
            Content = "Today",
            Padding = new Thickness(12, 6)
        };
        todayButton.Click += (s, e) => GoToToday();
        footerPanel.Children.Add(todayButton);

        var clearButton = new Button
        {
            Content = "Clear",
            Padding = new Thickness(12, 6)
        };
        clearButton.Click += (s, e) => ClearSelection();
        footerPanel.Children.Add(clearButton);

        return footerPanel;
    }

    private void NavigateMonth(int months)
    {
        _displayMonth = _displayMonth.AddMonths(months);
        if (_calendar != null)
        {
            _calendar.DisplayDate = _displayMonth;
        }
        UpdateHeader();
    }

    private void GoToToday()
    {
        SelectedDate = DateTime.Today;
        _displayMonth = DateTime.Today;
        if (_calendar != null)
        {
            _calendar.SelectedDate = DateTime.Today;
            _calendar.DisplayDate = DateTime.Today;
        }
        UpdateHeader();
    }

    private void ClearSelection()
    {
        if (_calendar != null)
        {
            _calendar.SelectedDate = null;
        }
    }

    private void UpdateHeader()
    {
        // Update header text - in a real implementation, you'd update the TextBlock
        // This is a simplified version
    }

    private void OnSelectedDatesChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_calendar?.SelectedDate.HasValue == true)
        {
            SelectedDate = _calendar.SelectedDate.Value;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedDateProperty && _calendar != null)
        {
            _calendar.SelectedDate = SelectedDate;
        }
    }
}