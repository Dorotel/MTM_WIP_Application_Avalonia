using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using System.Collections;

namespace MTM.UniversalFramework.Avalonia.Controls;

/// <summary>
/// Universal chart container control for displaying data visualizations.
/// Supports multiple chart types with responsive design.
/// </summary>
public class UniversalChartContainer : UserControl
{
    public static readonly StyledProperty<string> ChartTypeProperty =
        AvaloniaProperty.Register<UniversalChartContainer, string>(nameof(ChartType), "Bar");

    public static readonly StyledProperty<IEnumerable?> DataSourceProperty =
        AvaloniaProperty.Register<UniversalChartContainer, IEnumerable?>(nameof(DataSource));

    public static readonly StyledProperty<string> XAxisLabelProperty =
        AvaloniaProperty.Register<UniversalChartContainer, string>(nameof(XAxisLabel), "X Axis");

    public static readonly StyledProperty<string> YAxisLabelProperty =
        AvaloniaProperty.Register<UniversalChartContainer, string>(nameof(YAxisLabel), "Y Axis");

    public static readonly StyledProperty<string> ChartTitleProperty =
        AvaloniaProperty.Register<UniversalChartContainer, string>(nameof(ChartTitle), "Chart");

    public static readonly StyledProperty<bool> ShowLegendProperty =
        AvaloniaProperty.Register<UniversalChartContainer, bool>(nameof(ShowLegend), true);

    public static readonly StyledProperty<bool> EnableAnimationsProperty =
        AvaloniaProperty.Register<UniversalChartContainer, bool>(nameof(EnableAnimations), true);

    /// <summary>
    /// Gets or sets the chart type (Bar, Line, Pie, Area, etc.).
    /// </summary>
    public string ChartType
    {
        get => GetValue(ChartTypeProperty);
        set => SetValue(ChartTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the data source for the chart.
    /// </summary>
    public IEnumerable? DataSource
    {
        get => GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the X-axis label.
    /// </summary>
    public string XAxisLabel
    {
        get => GetValue(XAxisLabelProperty);
        set => SetValue(XAxisLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the Y-axis label.
    /// </summary>
    public string YAxisLabel
    {
        get => GetValue(YAxisLabelProperty);
        set => SetValue(YAxisLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string ChartTitle
    {
        get => GetValue(ChartTitleProperty);
        set => SetValue(ChartTitleProperty, value);
    }

    /// <summary>
    /// Gets or sets whether to show the legend.
    /// </summary>
    public bool ShowLegend
    {
        get => GetValue(ShowLegendProperty);
        set => SetValue(ShowLegendProperty, value);
    }

    /// <summary>
    /// Gets or sets whether animations are enabled.
    /// </summary>
    public bool EnableAnimations
    {
        get => GetValue(EnableAnimationsProperty);
        set => SetValue(EnableAnimationsProperty, value);
    }

    static UniversalChartContainer()
    {
        BackgroundProperty.OverrideDefaultValue<UniversalChartContainer>(Avalonia.Media.Brushes.White);
        BorderThicknessProperty.OverrideDefaultValue<UniversalChartContainer>(new Thickness(1));
        BorderBrushProperty.OverrideDefaultValue<UniversalChartContainer>(Avalonia.Media.Brushes.LightGray);
        CornerRadiusProperty.OverrideDefaultValue<UniversalChartContainer>(new CornerRadius(4));
        PaddingProperty.OverrideDefaultValue<UniversalChartContainer>(new Thickness(12));
    }

    public UniversalChartContainer()
    {
        Classes.Add("universal-chart");
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        // Create chart placeholder content
        Content = new StackPanel
        {
            Children =
            {
                new TextBlock
                {
                    Text = "Chart Placeholder",
                    FontSize = 16,
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                },
                new TextBlock
                {
                    Text = "Connect to your preferred charting library",
                    FontSize = 12,
                    Foreground = Avalonia.Media.Brushes.Gray,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    Margin = new Thickness(0, 8, 0, 0)
                }
            }
        };
    }
}