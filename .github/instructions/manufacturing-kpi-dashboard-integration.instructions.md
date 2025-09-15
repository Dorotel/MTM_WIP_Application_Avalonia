# Manufacturing KPI Dashboard Integration - MTM WIP Application Instructions

**Framework**: .NET 8 with Real-Time Data Integration  
**Pattern**: Manufacturing KPI Monitoring and Performance Analytics  
**Created**: 2025-09-15  

---

## 🏭 Manufacturing KPI Dashboard Architecture

### Real-Time Manufacturing Metrics Dashboard

```csharp
// Comprehensive KPI dashboard ViewModel with real-time manufacturing metrics
public partial class ManufacturingKPIDashboardViewModel : BaseViewModel
{
    private readonly IManufacturingKPIService _kpiService;
    private readonly IRealTimeDataService _realTimeDataService;
    private readonly IProductionMetricsService _productionMetricsService;
    private readonly IQualityMetricsService _qualityMetricsService;
    private readonly ITimer _refreshTimer;
    private readonly IMessenger _messenger;
    private readonly ILogger<ManufacturingKPIDashboardViewModel> _logger;

    // Real-time KPI properties
    [ObservableProperty]
    private double overallEquipmentEffectiveness;

    [ObservableProperty]
    private double availability;

    [ObservableProperty]
    private double performance;

    [ObservableProperty]
    private double quality;

    [ObservableProperty]
    private int partsProduced;

    [ObservableProperty]
    private int partsDefective;

    [ObservableProperty]
    private double throughputRate;

    [ObservableProperty]
    private TimeSpan cycleTime;

    [ObservableProperty]
    private double efficiencyPercentage;

    [ObservableProperty]
    private ObservableCollection<ProductionMetric> productionTrends = new();

    [ObservableProperty]
    private ObservableCollection<QualityMetric> qualityTrends = new();

    [ObservableProperty]
    private ObservableCollection<EquipmentStatus> equipmentStatuses = new();

    // Dashboard state management
    [ObservableProperty]
    private bool isDashboardActive;

    [ObservableProperty]
    private DateTime lastUpdateTime;

    [ObservableProperty]
    private string selectedTimeRange = "LastHour";

    [ObservableProperty]
    private bool isRealTimeMode = true;

    public ManufacturingKPIDashboardViewModel(
        IManufacturingKPIService kpiService,
        IRealTimeDataService realTimeDataService,
        IProductionMetricsService productionMetricsService,
        IQualityMetricsService qualityMetricsService,
        IMessenger messenger,
        ILogger<ManufacturingKPIDashboardViewModel> logger)
    {
        _kpiService = kpiService ?? throw new ArgumentNullException(nameof(kpiService));
        _realTimeDataService = realTimeDataService ?? throw new ArgumentNullException(nameof(realTimeDataService));
        _productionMetricsService = productionMetricsService ?? throw new ArgumentNullException(nameof(productionMetricsService));
        _qualityMetricsService = qualityMetricsService ?? throw new ArgumentNullException(nameof(qualityMetricsService));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Initialize real-time data refresh
        _refreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        
        // Subscribe to real-time manufacturing events
        _messenger.Register<ProductionDataUpdatedMessage>(this, OnProductionDataUpdated);
        _messenger.Register<QualityDataUpdatedMessage>(this, OnQualityDataUpdated);
        _messenger.Register<EquipmentStatusChangedMessage>(this, OnEquipmentStatusChanged);
    }

    [RelayCommand]
    private async Task StartDashboardAsync()
    {
        try
        {
            IsLoading = true;
            IsDashboardActive = true;

            // Initialize KPI calculations
            await InitializeKPIDataAsync();

            // Start real-time updates
            if (IsRealTimeMode)
            {
                _ = StartRealTimeUpdatesAsync();
            }

            _logger.LogInformation("Manufacturing KPI dashboard started successfully");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Start KPI dashboard");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task StopDashboardAsync()
    {
        try
        {
            IsDashboardActive = false;
            IsRealTimeMode = false;

            _logger.LogInformation("Manufacturing KPI dashboard stopped");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Stop KPI dashboard");
        }
    }

    [RelayCommand]
    private async Task RefreshKPIDataAsync()
    {
        try
        {
            IsLoading = true;
            await UpdateKPIMetricsAsync();
            LastUpdateTime = DateTime.Now;
            
            _logger.LogDebug("KPI data refreshed at {UpdateTime}", LastUpdateTime);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Refresh KPI data");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ChangeTimeRangeAsync(string timeRange)
    {
        try
        {
            SelectedTimeRange = timeRange;
            await UpdateKPIMetricsAsync();
            
            _logger.LogDebug("Time range changed to {TimeRange}", timeRange);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "Change time range");
        }
    }

    private async Task InitializeKPIDataAsync()
    {
        // Load initial KPI data
        await UpdateKPIMetricsAsync();
        await LoadProductionTrendsAsync();
        await LoadQualityTrendsAsync();
        await LoadEquipmentStatusesAsync();
    }

    private async Task UpdateKPIMetricsAsync()
    {
        var kpiData = await _kpiService.GetKPIDataAsync(GetTimeRangeFilter());

        // Update OEE components
        Availability = kpiData.Availability;
        Performance = kpiData.Performance;
        Quality = kpiData.Quality;
        OverallEquipmentEffectiveness = Availability * Performance * Quality;

        // Update production metrics
        PartsProduced = kpiData.PartsProduced;
        PartsDefective = kpiData.PartsDefective;
        ThroughputRate = kpiData.ThroughputRate;
        CycleTime = kpiData.AverageCycleTime;
        EfficiencyPercentage = kpiData.EfficiencyPercentage;
    }

    private async Task LoadProductionTrendsAsync()
    {
        var trends = await _productionMetricsService.GetProductionTrendsAsync(GetTimeRangeFilter());
        
        ProductionTrends.Clear();
        foreach (var trend in trends)
        {
            ProductionTrends.Add(trend);
        }
    }

    private async Task LoadQualityTrendsAsync()
    {
        var trends = await _qualityMetricsService.GetQualityTrendsAsync(GetTimeRangeFilter());
        
        QualityTrends.Clear();
        foreach (var trend in trends)
        {
            QualityTrends.Add(trend);
        }
    }

    private async Task LoadEquipmentStatusesAsync()
    {
        var statuses = await _kpiService.GetEquipmentStatusesAsync();
        
        EquipmentStatuses.Clear();
        foreach (var status in statuses)
        {
            EquipmentStatuses.Add(status);
        }
    }

    private async Task StartRealTimeUpdatesAsync()
    {
        try
        {
            while (IsDashboardActive && IsRealTimeMode)
            {
                await _refreshTimer.WaitForNextTickAsync();
                
                if (IsDashboardActive && IsRealTimeMode)
                {
                    await UpdateKPIMetricsAsync();
                    LastUpdateTime = DateTime.Now;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in real-time KPI updates");
        }
    }

    private TimeRangeFilter GetTimeRangeFilter()
    {
        return SelectedTimeRange switch
        {
            "LastHour" => new TimeRangeFilter(DateTime.Now.AddHours(-1), DateTime.Now),
            "LastShift" => new TimeRangeFilter(DateTime.Now.AddHours(-8), DateTime.Now),
            "Last24Hours" => new TimeRangeFilter(DateTime.Now.AddDays(-1), DateTime.Now),
            "LastWeek" => new TimeRangeFilter(DateTime.Now.AddDays(-7), DateTime.Now),
            _ => new TimeRangeFilter(DateTime.Now.AddHours(-1), DateTime.Now)
        };
    }

    // Real-time event handlers
    private void OnProductionDataUpdated(ProductionDataUpdatedMessage message)
    {
        // Update production metrics in real-time
        PartsProduced = message.TotalPartsProduced;
        ThroughputRate = message.CurrentThroughputRate;
        CycleTime = message.AverageCycleTime;
    }

    private void OnQualityDataUpdated(QualityDataUpdatedMessage message)
    {
        // Update quality metrics in real-time
        PartsDefective = message.TotalDefectiveParts;
        Quality = message.QualityPercentage;
        OverallEquipmentEffectiveness = Availability * Performance * Quality;
    }

    private void OnEquipmentStatusChanged(EquipmentStatusChangedMessage message)
    {
        // Update equipment status in real-time
        var existingStatus = EquipmentStatuses.FirstOrDefault(s => s.EquipmentId == message.EquipmentId);
        if (existingStatus != null)
        {
            existingStatus.Status = message.NewStatus;
            existingStatus.LastUpdated = message.Timestamp;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _refreshTimer?.Dispose();
            _messenger.Unregister<ProductionDataUpdatedMessage>(this);
            _messenger.Unregister<QualityDataUpdatedMessage>(this);
            _messenger.Unregister<EquipmentStatusChangedMessage>(this);
        }
        base.Dispose(disposing);
    }
}
```

### Manufacturing KPI Service Implementation

```csharp
// Manufacturing KPI calculation service with advanced analytics
public class ManufacturingKPIService : IManufacturingKPIService
{
    private readonly IDatabaseService _databaseService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ManufacturingKPIService> _logger;
    
    // KPI calculation constants
    private const double TARGET_CYCLE_TIME = 60.0; // seconds
    private const double PLANNED_PRODUCTION_TIME = 28800.0; // 8 hours in seconds
    private const double TARGET_QUALITY_RATE = 0.98; // 98%

    public ManufacturingKPIService(
        IDatabaseService databaseService,
        IMemoryCache cache,
        ILogger<ManufacturingKPIService> logger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<KPIData> GetKPIDataAsync(TimeRangeFilter timeRange)
    {
        var cacheKey = $"kpi_data_{timeRange.StartTime:yyyyMMddHHmm}_{timeRange.EndTime:yyyyMMddHHmm}";
        
        if (_cache.TryGetValue(cacheKey, out KPIData? cachedData))
        {
            return cachedData!;
        }

        try
        {
            var kpiData = await CalculateKPIDataAsync(timeRange);
            
            // Cache for 1 minute
            _cache.Set(cacheKey, kpiData, TimeSpan.FromMinutes(1));
            
            return kpiData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating KPI data for time range {StartTime} - {EndTime}", 
                timeRange.StartTime, timeRange.EndTime);
            throw;
        }
    }

    private async Task<KPIData> CalculateKPIDataAsync(TimeRangeFilter timeRange)
    {
        // Get production data from database
        var productionData = await GetProductionDataAsync(timeRange);
        var qualityData = await GetQualityDataAsync(timeRange);
        var downTimeData = await GetDownTimeDataAsync(timeRange);

        // Calculate OEE components
        var availability = CalculateAvailability(productionData, downTimeData, timeRange);
        var performance = CalculatePerformance(productionData);
        var quality = CalculateQuality(productionData, qualityData);

        // Calculate additional metrics
        var throughputRate = CalculateThroughputRate(productionData, timeRange);
        var cycleTime = CalculateAverageCycleTime(productionData);
        var efficiencyPercentage = CalculateEfficiency(productionData, timeRange);

        return new KPIData
        {
            Availability = availability,
            Performance = performance,
            Quality = quality,
            PartsProduced = productionData.TotalPartsProduced,
            PartsDefective = qualityData.DefectiveParts,
            ThroughputRate = throughputRate,
            AverageCycleTime = cycleTime,
            EfficiencyPercentage = efficiencyPercentage,
            TimeRange = timeRange
        };
    }

    private double CalculateAvailability(ProductionData productionData, DownTimeData downTimeData, TimeRangeFilter timeRange)
    {
        var plannedProductionTime = (timeRange.EndTime - timeRange.StartTime).TotalSeconds;
        var actualRunTime = plannedProductionTime - downTimeData.TotalDownTimeSeconds;
        
        return Math.Max(0.0, actualRunTime / plannedProductionTime);
    }

    private double CalculatePerformance(ProductionData productionData)
    {
        if (productionData.TotalPartsProduced == 0 || productionData.ActualCycleTimeSeconds == 0)
            return 0.0;

        var idealCycleTime = TARGET_CYCLE_TIME;
        var actualCycleTime = productionData.ActualCycleTimeSeconds / productionData.TotalPartsProduced;

        return Math.Min(1.0, idealCycleTime / actualCycleTime);
    }

    private double CalculateQuality(ProductionData productionData, QualityData qualityData)
    {
        if (productionData.TotalPartsProduced == 0)
            return 1.0;

        var goodParts = productionData.TotalPartsProduced - qualityData.DefectiveParts;
        return (double)goodParts / productionData.TotalPartsProduced;
    }

    private double CalculateThroughputRate(ProductionData productionData, TimeRangeFilter timeRange)
    {
        var timeSpanHours = (timeRange.EndTime - timeRange.StartTime).TotalHours;
        return timeSpanHours > 0 ? productionData.TotalPartsProduced / timeSpanHours : 0.0;
    }

    private TimeSpan CalculateAverageCycleTime(ProductionData productionData)
    {
        if (productionData.TotalPartsProduced == 0)
            return TimeSpan.Zero;

        var averageSeconds = productionData.ActualCycleTimeSeconds / productionData.TotalPartsProduced;
        return TimeSpan.FromSeconds(averageSeconds);
    }

    private double CalculateEfficiency(ProductionData productionData, TimeRangeFilter timeRange)
    {
        var plannedProduction = CalculatePlannedProduction(timeRange);
        return plannedProduction > 0 ? (double)productionData.TotalPartsProduced / plannedProduction : 0.0;
    }

    private int CalculatePlannedProduction(TimeRangeFilter timeRange)
    {
        var timeSpanSeconds = (timeRange.EndTime - timeRange.StartTime).TotalSeconds;
        return (int)(timeSpanSeconds / TARGET_CYCLE_TIME);
    }

    // Database query methods
    private async Task<ProductionData> GetProductionDataAsync(TimeRangeFilter timeRange)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_StartTime", timeRange.StartTime),
            new("p_EndTime", timeRange.EndTime)
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "kpi_production_Get_Data", parameters);

        return new ProductionData
        {
            TotalPartsProduced = Convert.ToInt32(result.Data.Rows[0]["TotalPartsProduced"]),
            ActualCycleTimeSeconds = Convert.ToDouble(result.Data.Rows[0]["ActualCycleTimeSeconds"])
        };
    }

    private async Task<QualityData> GetQualityDataAsync(TimeRangeFilter timeRange)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_StartTime", timeRange.StartTime),
            new("p_EndTime", timeRange.EndTime)
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "kpi_quality_Get_Data", parameters);

        return new QualityData
        {
            DefectiveParts = Convert.ToInt32(result.Data.Rows[0]["DefectiveParts"])
        };
    }

    private async Task<DownTimeData> GetDownTimeDataAsync(TimeRangeFilter timeRange)
    {
        var parameters = new MySqlParameter[]
        {
            new("p_StartTime", timeRange.StartTime),
            new("p_EndTime", timeRange.EndTime)
        };

        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "kpi_downtime_Get_Data", parameters);

        return new DownTimeData
        {
            TotalDownTimeSeconds = Convert.ToDouble(result.Data.Rows[0]["TotalDownTimeSeconds"])
        };
    }

    public async Task<IList<EquipmentStatus>> GetEquipmentStatusesAsync()
    {
        var result = await _databaseService.ExecuteStoredProcedureAsync(
            "kpi_equipment_Get_Status", Array.Empty<MySqlParameter>());

        var statuses = new List<EquipmentStatus>();
        foreach (DataRow row in result.Data.Rows)
        {
            statuses.Add(new EquipmentStatus
            {
                EquipmentId = row["EquipmentId"].ToString()!,
                EquipmentName = row["EquipmentName"].ToString()!,
                Status = row["Status"].ToString()!,
                LastUpdated = Convert.ToDateTime(row["LastUpdated"])
            });
        }

        return statuses;
    }
}
```

### Manufacturing KPI UI Components

```xml
<!-- Manufacturing KPI Dashboard UserControl -->
<UserControl x:Class="MTM_WIP_Application_Avalonia.Views.ManufacturingKPIDashboardView"
             xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:MTM_WIP_Application_Avalonia.ViewModels"
             x:DataType="vm:ManufacturingKPIDashboardViewModel">
    
    <ScrollViewer>
        <Grid RowDefinitions="Auto,*,Auto" Margin="16">
            
            <!-- Dashboard Header -->
            <Border Grid.Row="0" 
                    Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                    CornerRadius="8" 
                    Padding="16" 
                    Margin="0,0,0,16">
                <Grid ColumnDefinitions="*,Auto,Auto">
                    <StackPanel Grid.Column="0">
                        <TextBlock Text="Manufacturing KPI Dashboard" 
                                   FontSize="24" 
                                   FontWeight="Bold" 
                                   Foreground="{DynamicResource MTM_Shared_Logic.HeadingText}" />
                        <TextBlock Text="{Binding LastUpdateTime, StringFormat='Last Updated: September 15, 2025
                                   FontSize="14" 
                                   Foreground="{DynamicResource MTM_Shared_Logic.TertiaryTextBrush}" />
                    </StackPanel>
                    
                    <ComboBox Grid.Column="1" 
                              SelectedItem="{Binding SelectedTimeRange}" 
                              Margin="8,0">
                        <ComboBoxItem Content="Last Hour" />
                        <ComboBoxItem Content="Last Shift" />
                        <ComboBoxItem Content="Last 24 Hours" />
                        <ComboBoxItem Content="Last Week" />
                    </ComboBox>
                    
                    <StackPanel Grid.Column="2" Orientation="Horizontal" Spacing="8">
                        <Button Content="Start Dashboard" 
                                Command="{Binding StartDashboardCommand}"
                                IsVisible="{Binding !IsDashboardActive}"
                                Classes="primary" />
                        <Button Content="Stop Dashboard" 
                                Command="{Binding StopDashboardCommand}"
                                IsVisible="{Binding IsDashboardActive}"
                                Classes="secondary" />
                        <Button Content="Refresh" 
                                Command="{Binding RefreshKPIDataCommand}"
                                Classes="secondary" />
                    </StackPanel>
                </Grid>
            </Border>
            
            <!-- KPI Metrics Grid -->
            <Grid Grid.Row="1" RowDefinitions="Auto,Auto,*" ColumnDefinitions="*,*">
                
                <!-- OEE Section -->
                <Border Grid.Row="0" Grid.ColumnSpan="2"
                        Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                        CornerRadius="8" 
                        Padding="16" 
                        Margin="0,0,0,16">
                    <Grid ColumnDefinitions="*,*,*,*">
                        <!-- Overall Equipment Effectiveness -->
                        <StackPanel Grid.Column="0" HorizontalAlignment="Center">
                            <TextBlock Text="OEE" 
                                       FontSize="16" 
                                       FontWeight="Bold" 
                                       HorizontalAlignment="Center" />
                            <TextBlock Text="{Binding OverallEquipmentEffectiveness, StringFormat='{0:P1}'}" 
                                       FontSize="36" 
                                       FontWeight="Bold" 
                                       HorizontalAlignment="Center"
                                       Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
                        </StackPanel>
                        
                        <!-- Availability -->
                        <StackPanel Grid.Column="1" HorizontalAlignment="Center">
                            <TextBlock Text="Availability" 
                                       FontSize="14" 
                                       HorizontalAlignment="Center" />
                            <TextBlock Text="{Binding Availability, StringFormat='{0:P1}'}" 
                                       FontSize="24" 
                                       FontWeight="Bold" 
                                       HorizontalAlignment="Center" />
                        </StackPanel>
                        
                        <!-- Performance -->
                        <StackPanel Grid.Column="2" HorizontalAlignment="Center">
                            <TextBlock Text="Performance" 
                                       FontSize="14" 
                                       HorizontalAlignment="Center" />
                            <TextBlock Text="{Binding Performance, StringFormat='{0:P1}'}" 
                                       FontSize="24" 
                                       FontWeight="Bold" 
                                       HorizontalAlignment="Center" />
                        </StackPanel>
                        
                        <!-- Quality -->
                        <StackPanel Grid.Column="3" HorizontalAlignment="Center">
                            <TextBlock Text="Quality" 
                                       FontSize="14" 
                                       HorizontalAlignment="Center" />
                            <TextBlock Text="{Binding Quality, StringFormat='{0:P1}'}" 
                                       FontSize="24" 
                                       FontWeight="Bold" 
                                       HorizontalAlignment="Center" />
                        </StackPanel>
                    </Grid>
                </Border>
                
                <!-- Production Metrics -->
                <Border Grid.Row="1" Grid.Column="0"
                        Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                        CornerRadius="8" 
                        Padding="16" 
                        Margin="0,0,8,16">
                    <StackPanel Spacing="12">
                        <TextBlock Text="Production Metrics" 
                                   FontSize="18" 
                                   FontWeight="Bold" />
                        
                        <Grid ColumnDefinitions="*,Auto" RowDefinitions="Auto,Auto,Auto,Auto">
                            <TextBlock Grid.Row="0" Grid.Column="0" Text="Parts Produced:" />
                            <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding PartsProduced, StringFormat='{0:N0}'}" FontWeight="Bold" />
                            
                            <TextBlock Grid.Row="1" Grid.Column="0" Text="Parts Defective:" />
                            <TextBlock Grid.Row="1" Grid.Column="1" Text="{Binding PartsDefective, StringFormat='{0:N0}'}" FontWeight="Bold" />
                            
                            <TextBlock Grid.Row="2" Grid.Column="0" Text="Throughput Rate:" />
                            <TextBlock Grid.Row="2" Grid.Column="1" Text="{Binding ThroughputRate, StringFormat='{0:F1} parts/hr'}" FontWeight="Bold" />
                            
                            <TextBlock Grid.Row="3" Grid.Column="0" Text="Cycle Time:" />
                            <TextBlock Grid.Row="3" Grid.Column="1" Text="{Binding CycleTime, StringFormat='{0:mm\\:ss}'}" FontWeight="Bold" />
                        </Grid>
                    </StackPanel>
                </Border>
                
                <!-- Equipment Status -->
                <Border Grid.Row="1" Grid.Column="1"
                        Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                        CornerRadius="8" 
                        Padding="16" 
                        Margin="8,0,0,16">
                    <StackPanel Spacing="12">
                        <TextBlock Text="Equipment Status" 
                                   FontSize="18" 
                                   FontWeight="Bold" />
                        
                        <ItemsControl ItemsSource="{Binding EquipmentStatuses}">
                            <ItemsControl.ItemTemplate>
                                <DataTemplate>
                                    <Grid ColumnDefinitions="*,Auto" Margin="0,2">
                                        <TextBlock Grid.Column="0" Text="{Binding EquipmentName}" />
                                        <Border Grid.Column="1" 
                                                Background="{Binding Status, Converter={StaticResource StatusToColorConverter}}"
                                                CornerRadius="4" 
                                                Padding="8,2">
                                            <TextBlock Text="{Binding Status}" 
                                                       Foreground="White" 
                                                       FontSize="12" />
                                        </Border>
                                    </Grid>
                                </DataTemplate>
                            </ItemsControl.ItemTemplate>
                        </ItemsControl>
                    </StackPanel>
                </Border>
                
                <!-- Production Trends Chart -->
                <Border Grid.Row="2" Grid.ColumnSpan="2"
                        Background="{DynamicResource MTM_Shared_Logic.CardBackgroundBrush}"
                        CornerRadius="8" 
                        Padding="16">
                    <StackPanel Spacing="12">
                        <TextBlock Text="Production Trends" 
                                   FontSize="18" 
                                   FontWeight="Bold" />
                        
                        <!-- Chart would be implemented with a charting library -->
                        <Border Background="{DynamicResource MTM_Shared_Logic.MainBackground}"
                                Height="200" 
                                CornerRadius="4">
                            <TextBlock Text="Production Trend Chart" 
                                       HorizontalAlignment="Center" 
                                       VerticalAlignment="Center" 
                                       FontSize="16" 
                                       Opacity="0.7" />
                        </Border>
                    </StackPanel>
                </Border>
            </Grid>
            
            <!-- Loading Overlay -->
            <Border Grid.Row="0" Grid.RowSpan="3"
                    Background="#80000000" 
                    IsVisible="{Binding IsLoading}">
                <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
                    <ProgressBar IsIndeterminate="True" 
                                 Width="200" 
                                 Height="4" 
                                 Foreground="{DynamicResource MTM_Shared_Logic.PrimaryAction}" />
                    <TextBlock Text="Loading KPI Data..." 
                               HorizontalAlignment="Center" 
                               Margin="0,12,0,0" 
                               Foreground="White" />
                </StackPanel>
            </Border>
        </Grid>
    </ScrollViewer>
</UserControl>
```

### Data Models for KPI Dashboard

```csharp
// KPI data models for manufacturing metrics
namespace MTM_WIP_Application_Avalonia.Models
{
    public class KPIData
    {
        public double Availability { get; set; }
        public double Performance { get; set; }
        public double Quality { get; set; }
        public int PartsProduced { get; set; }
        public int PartsDefective { get; set; }
        public double ThroughputRate { get; set; }
        public TimeSpan AverageCycleTime { get; set; }
        public double EfficiencyPercentage { get; set; }
        public TimeRangeFilter TimeRange { get; set; } = new();
    }

    public class ProductionMetric
    {
        public DateTime Timestamp { get; set; }
        public int PartsProduced { get; set; }
        public double ThroughputRate { get; set; }
        public TimeSpan CycleTime { get; set; }
        public double EfficiencyPercentage { get; set; }
    }

    public class QualityMetric
    {
        public DateTime Timestamp { get; set; }
        public int TotalParts { get; set; }
        public int DefectiveParts { get; set; }
        public double QualityPercentage { get; set; }
        public string DefectType { get; set; } = string.Empty;
    }

    public class EquipmentStatus
    {
        public string EquipmentId { get; set; } = string.Empty;
        public string EquipmentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // "Running", "Stopped", "Maintenance", "Error"
        public DateTime LastUpdated { get; set; }
        public double UtilizationPercentage { get; set; }
    }

    public class TimeRangeFilter
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public TimeRangeFilter() { }

        public TimeRangeFilter(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }

    public class ProductionData
    {
        public int TotalPartsProduced { get; set; }
        public double ActualCycleTimeSeconds { get; set; }
    }

    public class QualityData
    {
        public int DefectiveParts { get; set; }
    }

    public class DownTimeData
    {
        public double TotalDownTimeSeconds { get; set; }
    }
}
```

### Manufacturing KPI Messages for Real-Time Updates

```csharp
// MVVM Community Toolkit messages for real-time KPI updates
namespace MTM_WIP_Application_Avalonia.Messages
{
    public record ProductionDataUpdatedMessage(
        int TotalPartsProduced,
        double CurrentThroughputRate,
        TimeSpan AverageCycleTime,
        DateTime Timestamp);

    public record QualityDataUpdatedMessage(
        int TotalDefectiveParts,
        double QualityPercentage,
        DateTime Timestamp);

    public record EquipmentStatusChangedMessage(
        string EquipmentId,
        string NewStatus,
        DateTime Timestamp);
}
```

---

## 📚 Related Documentation

- **Service Integration**: [Cross-Service Communication](./service-integration.instructions.md)
- **Database Integration**: [Advanced Database Patterns](./database-integration.instructions.md)
- **MVVM Patterns**: [Complex ViewModel Implementation](./mvvm-community-toolkit.instructions.md)
- **Real-Time Data**: [SignalR Integration Patterns](./external-system-integration.instructions.md)

---

**Document Status**: ✅ Complete Manufacturing KPI Dashboard Integration Reference  
**Framework Version**: .NET 8 with Real-Time Data Integration  
**Last Updated**: 2025-09-15  
**Manufacturing KPI Owner**: MTM Development Team