using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MTM.Avalonia.Controls;

/// <summary>
/// Universal analytics dashboard control with KPI cards, charts, and performance metrics.
/// Extracted from handler-dashboard-analytics-dashboard.html mockup.
/// </summary>
public class UniversalAnalyticsDashboard : TemplatedControl
{
    public static readonly StyledProperty<string> DashboardTitleProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, string>(
            nameof(DashboardTitle), 
            "Analytics Dashboard");

    public static readonly StyledProperty<string> UserNameProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, string>(
            nameof(UserName), 
            "User");

    public static readonly StyledProperty<string> UserStatusProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, string>(
            nameof(UserStatus), 
            "Active");

    public static readonly StyledProperty<ObservableCollection<KpiMetric>> KpiMetricsProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, ObservableCollection<KpiMetric>>(
            nameof(KpiMetrics), 
            new ObservableCollection<KpiMetric>());

    public static readonly StyledProperty<ObservableCollection<ChartData>> ChartsDataProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, ObservableCollection<ChartData>>(
            nameof(ChartsData), 
            new ObservableCollection<ChartData>());

    public static readonly StyledProperty<string> SelectedTimePeriodProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, string>(
            nameof(SelectedTimePeriod), 
            "Today",
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand> TimePeriodChangedCommandProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, ICommand>(
            nameof(TimePeriodChangedCommand));

    public static readonly StyledProperty<ICommand> RefreshDashboardCommandProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, ICommand>(
            nameof(RefreshDashboardCommand));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, bool>(
            nameof(IsLoading), 
            false);

    public static readonly StyledProperty<DateTime> LastUpdateProperty =
        AvaloniaProperty.Register<UniversalAnalyticsDashboard, DateTime>(
            nameof(LastUpdate), 
            DateTime.Now);

    /// <summary>
    /// Gets or sets the dashboard title.
    /// </summary>
    public string DashboardTitle
    {
        get => GetValue(DashboardTitleProperty);
        set => SetValue(DashboardTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets the user name displayed in the dashboard header.
    /// </summary>
    public string UserName
    {
        get => GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the user status (Active, Busy, Away, etc.).
    /// </summary>
    public string UserStatus
    {
        get => GetValue(UserStatusProperty);
        set => SetValue(UserStatusProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of KPI metrics to display.
    /// </summary>
    public ObservableCollection<KpiMetric> KpiMetrics
    {
        get => GetValue(KpiMetricsProperty);
        set => SetValue(KpiMetricsProperty, value);
    }

    /// <summary>
    /// Gets or sets the collection of chart data.
    /// </summary>
    public ObservableCollection<ChartData> ChartsData
    {
        get => GetValue(ChartsDataProperty);
        set => SetValue(ChartsDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the selected time period for analytics.
    /// </summary>
    public string SelectedTimePeriod
    {
        get => GetValue(SelectedTimePeriodProperty);
        set => SetValue(SelectedTimePeriodProperty, value);
    }

    /// <summary>
    /// Gets or sets the command executed when time period changes.
    /// </summary>
    public ICommand TimePeriodChangedCommand
    {
        get => GetValue(TimePeriodChangedCommandProperty);
        set => SetValue(TimePeriodChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets the command to refresh dashboard data.
    /// </summary>
    public ICommand RefreshDashboardCommand
    {
        get => GetValue(RefreshDashboardCommandProperty);
        set => SetValue(RefreshDashboardCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets whether the dashboard is currently loading data.
    /// </summary>
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    /// <summary>
    /// Gets or sets the last update timestamp.
    /// </summary>
    public DateTime LastUpdate
    {
        get => GetValue(LastUpdateProperty);
        set => SetValue(LastUpdateProperty, value);
    }

    static UniversalAnalyticsDashboard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(UniversalAnalyticsDashboard), 
            new StyledPropertyMetadata(typeof(UniversalAnalyticsDashboard)));
    }

    public UniversalAnalyticsDashboard()
    {
        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        // Initialize with sample KPI data
        KpiMetrics = new ObservableCollection<KpiMetric>
        {
            new() { Title = "Tasks Completed", Value = "142", Trend = "+12%", TrendType = "Positive", Icon = "CheckCircle" },
            new() { Title = "Efficiency Rate", Value = "94.2%", Trend = "+3.1%", TrendType = "Positive", Icon = "TrendingUp" },
            new() { Title = "Active Time", Value = "7.2h", Trend = "-0.3h", TrendType = "Neutral", Icon = "Clock" },
            new() { Title = "Queue Position", Value = "#3", Trend = "+2", TrendType = "Negative", Icon = "Users" }
        };

        // Initialize with sample chart data
        ChartsData = new ObservableCollection<ChartData>
        {
            new() { Title = "Performance Trends", Type = "LineChart", DataPoints = new[] { 85, 92, 88, 94, 89, 96, 91 } },
            new() { Title = "Task Distribution", Type = "PieChart", DataPoints = new[] { 45, 25, 20, 10 } }
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedTimePeriodProperty)
        {
            TimePeriodChangedCommand?.Execute(change.NewValue);
        }
    }
}

/// <summary>
/// Represents a KPI metric for the analytics dashboard.
/// </summary>
public class KpiMetric
{
    public string Title { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Trend { get; set; } = string.Empty;
    public string TrendType { get; set; } = "Neutral"; // Positive, Negative, Neutral
    public string Icon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents chart data for visualization.
/// </summary>
public class ChartData
{
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = "LineChart"; // LineChart, PieChart, BarChart
    public double[] DataPoints { get; set; } = Array.Empty<double>();
    public string[] Labels { get; set; } = Array.Empty<string>();
    public string ChartColor { get; set; } = "#667eea";
}