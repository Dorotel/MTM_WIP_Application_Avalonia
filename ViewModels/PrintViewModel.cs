using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTM_WIP_Application_Avalonia.ViewModels.Shared;
using MTM_WIP_Application_Avalonia.ViewModels.MainForm;
using MTM_WIP_Application_Avalonia.Services;
using MTM_WIP_Application_Avalonia.Models;
using Avalonia.Controls;

namespace MTM_WIP_Application_Avalonia.ViewModels;

/// <summary>
/// PrintViewModel manages comprehensive print functionality for MTM WIP Application.
/// Features print options, real-time preview, layout customization, and template management.
/// Uses MVVM Community Toolkit for property and command management.
/// Follows ThemeEditorViewModel pattern for full-window navigation integration.
/// </summary>
public partial class PrintViewModel : BaseViewModel
{
    private readonly IPrintService _printService;
    private readonly INavigationService? _navigationService;
    private readonly IThemeService? _themeService;
    private readonly IConfigurationService? _configurationService;

    /// <summary>
    /// ViewModel for the layout customization control
    /// </summary>
    public PrintLayoutControlViewModel PrintLayoutControlViewModel { get; }

    #region Data Context

    /// <summary>
    /// Data to be printed (passed from DataGrid)
    /// </summary>
    public DataTable? PrintData { get; set; }

    /// <summary>
    /// Type of data being printed
    /// </summary>
    public PrintDataSourceType DataSourceType { get; set; } = PrintDataSourceType.Inventory;

    /// <summary>
    /// Original view context for navigation back
    /// </summary>
    public object? OriginalViewContext { get; set; }

    #endregion

    #region Print Status and Loading

    [ObservableProperty]
    private string statusMessage = "Ready to configure print settings";

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private bool isPreviewLoading = false;

    [ObservableProperty]
    private bool hasUnsavedChanges = false;

    [ObservableProperty]
    private bool canPrint = true;

    #endregion

    #region Printer and Configuration Settings

    [ObservableProperty]
    private ObservableCollection<string> availablePrinters = new();

    [ObservableProperty]
    private string selectedPrinter = string.Empty;

    [ObservableProperty]
    private PrintOrientation selectedOrientation = PrintOrientation.Portrait;

    [ObservableProperty]
    private int copies = 1;

    [ObservableProperty]
    private bool collate = true;

    [ObservableProperty]
    private PrintQuality selectedQuality = PrintQuality.Normal;

    [ObservableProperty]
    private Models.PaperSize selectedPaperSize = Models.PaperSize.Letter;

    [ObservableProperty]
    private PrintStyle selectedStyle = PrintStyle.Simple;

    [ObservableProperty]
    private bool includeHeaders = true;

    [ObservableProperty]
    private bool includeFooters = true;

    [ObservableProperty]
    private bool includeGridLines = true;

    [ObservableProperty]
    private bool includeTimestamp = true;

    [ObservableProperty]
    private bool includeUserInfo = true;

    [ObservableProperty]
    private int fontSize = 10;

    [ObservableProperty]
    private string fontFamily = "Arial";

    [ObservableProperty]
    private string documentTitle = string.Empty;

    [ObservableProperty]
    private string customHeaderText = string.Empty;

    [ObservableProperty]
    private string customFooterText = string.Empty;

    #endregion

    #region Print Preview

    [ObservableProperty]
    private Canvas? printPreview;

    [ObservableProperty]
    private double previewZoom = 1.0;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int totalPages = 1;

    #endregion

    #region Layout and Templates

    [ObservableProperty]
    private ObservableCollection<PrintLayoutTemplate> availableTemplates = new();

    [ObservableProperty]
    private PrintLayoutTemplate? selectedTemplate;

    [ObservableProperty]
    private ObservableCollection<PrintColumnInfo> printColumns = new();

    [ObservableProperty]
    private bool showLayoutPanel = false;

    #endregion

    #region Collections for ComboBoxes

    public ObservableCollection<PrintOrientation> OrientationOptions { get; } = new()
    {
        PrintOrientation.Portrait,
        PrintOrientation.Landscape
    };

    public ObservableCollection<PrintQuality> QualityOptions { get; } = new()
    {
        PrintQuality.Draft,
        PrintQuality.Normal,
        PrintQuality.High
    };

    public ObservableCollection<Models.PaperSize> PaperSizeOptions { get; } = new()
    {
        Models.PaperSize.Letter,
        Models.PaperSize.Legal,
        Models.PaperSize.A4,
        Models.PaperSize.A3,
        Models.PaperSize.Tabloid
    };

    public ObservableCollection<PrintStyle> StyleOptions { get; } = new()
    {
        PrintStyle.Simple,
        PrintStyle.Stylized
    };

    public ObservableCollection<string> FontFamilyOptions { get; } = new()
    {
        "Arial",
        "Helvetica",
        "Times New Roman",
        "Calibri",
        "Verdana"
    };

    #endregion

    public PrintViewModel(
        ILogger<PrintViewModel> logger,
        IPrintService printService,
        INavigationService? navigationService = null,
        IThemeService? themeService = null,
        IConfigurationService? configurationService = null) : base(logger)
    {
        _printService = printService ?? throw new ArgumentNullException(nameof(printService));
        _navigationService = navigationService;
        _themeService = themeService;
        _configurationService = configurationService;

        // Initialize the layout control ViewModel
        var layoutLogger = Program.GetOptionalService<ILogger<PrintLayoutControlViewModel>>() ??
                          Microsoft.Extensions.Logging.Abstractions.NullLogger<PrintLayoutControlViewModel>.Instance;
        PrintLayoutControlViewModel = new PrintLayoutControlViewModel(layoutLogger);

        Logger.LogDebug("PrintViewModel initialized");
        
        // Initialize with default title
        DocumentTitle = "MTM Report";
        
        // Start initialization
        _ = InitializeAsync();
    }

    #region Initialization

    /// <summary>
    /// Initialize the print view with data and configuration
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Initializing print service...";

            // Load available printers
            await LoadAvailablePrintersAsync();

            // Load print configuration
            await LoadPrintConfigurationAsync();

            // Load available templates
            await LoadAvailableTemplatesAsync();

            // Initialize print columns based on data
            InitializePrintColumns();

            // Generate initial preview
            await GeneratePreviewAsync();

            StatusMessage = "Print service ready";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing PrintViewModel");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to initialize print service", Environment.UserName);
            StatusMessage = "Error initializing print service";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAvailablePrintersAsync()
    {
        try
        {
            var printers = await _printService.GetAvailablePrintersAsync();
            
            AvailablePrinters.Clear();
            foreach (var printer in printers)
            {
                AvailablePrinters.Add(printer);
            }

            // Select first printer as default
            if (AvailablePrinters.Any())
            {
                SelectedPrinter = AvailablePrinters.First();
            }

            Logger.LogDebug("Loaded {PrinterCount} available printers", AvailablePrinters.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading available printers");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load available printers", Environment.UserName);
        }
    }

    private async Task LoadPrintConfigurationAsync()
    {
        try
        {
            var config = await _printService.GetPrintConfigurationAsync(DataSourceType);
            
            // Apply configuration to properties
            SelectedPrinter = config.PrinterName;
            SelectedOrientation = config.Orientation;
            Copies = config.Copies;
            Collate = config.Collate;
            SelectedQuality = config.Quality;
            SelectedPaperSize = config.PaperSize;
            SelectedStyle = config.Style;
            IncludeHeaders = config.IncludeHeaders;
            IncludeFooters = config.IncludeFooters;
            IncludeGridLines = config.IncludeGridLines;
            IncludeTimestamp = config.IncludeTimestamp;
            IncludeUserInfo = config.IncludeUserInfo;
            FontSize = config.FontSize;
            FontFamily = config.FontFamily;
            DocumentTitle = config.DocumentTitle;
            CustomHeaderText = config.CustomHeaderText;
            CustomFooterText = config.CustomFooterText;

            Logger.LogDebug("Loaded print configuration for {DataSourceType}", DataSourceType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading print configuration");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load print configuration", Environment.UserName);
        }
    }

    private async Task LoadAvailableTemplatesAsync()
    {
        try
        {
            var templates = await _printService.GetPrintTemplatesAsync(DataSourceType);
            
            AvailableTemplates.Clear();
            foreach (var template in templates)
            {
                AvailableTemplates.Add(template);
            }

            Logger.LogDebug("Loaded {TemplateCount} available templates", AvailableTemplates.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading available templates");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load available templates", Environment.UserName);
        }
    }

    private void InitializePrintColumns()
    {
        try
        {
            PrintColumns.Clear();

            if (PrintData != null && PrintData.Columns.Count > 0)
            {
                int displayOrder = 0;
                foreach (DataColumn column in PrintData.Columns)
                {
                    var printColumn = new PrintColumnInfo
                    {
                        Header = column.ColumnName,
                        PropertyName = column.ColumnName,
                        Width = 100,
                        IsVisible = true,
                        Alignment = DetermineAlignment(column.DataType),
                        DisplayOrder = displayOrder++
                    };

                    PrintColumns.Add(printColumn);
                }

                // Initialize the layout control with print columns
                PrintLayoutControlViewModel.InitializeColumns(PrintColumns);

                Logger.LogDebug("Initialized {ColumnCount} print columns", PrintColumns.Count);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing print columns");
        }
    }

    private PrintAlignment DetermineAlignment(Type dataType)
    {
        // Determine alignment based on data type
        if (dataType == typeof(int) || dataType == typeof(double) || dataType == typeof(decimal) || 
            dataType == typeof(float) || dataType == typeof(long))
        {
            return PrintAlignment.Right;
        }
        else if (dataType == typeof(DateTime) || dataType == typeof(TimeSpan))
        {
            return PrintAlignment.Center;
        }
        else
        {
            return PrintAlignment.Left;
        }
    }

    #endregion

    #region Print Commands

    /// <summary>
    /// Print the data with current configuration
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanPrint))]
    private async Task PrintAsync()
    {
        try
        {
            if (PrintData == null)
            {
                StatusMessage = "No data to print";
                return;
            }

            IsLoading = true;
            StatusMessage = "Preparing to print...";

            var config = CreatePrintConfiguration();
            var printStatus = await _printService.PrintDataAsync(PrintData, config);

            if (printStatus.IsSuccess)
            {
                StatusMessage = $"Print completed successfully. {printStatus.PagesPrinted} pages printed.";
                Logger.LogInformation("Print operation completed successfully. Pages: {Pages}", printStatus.PagesPrinted);
                
                // Save configuration for next time
                await SaveCurrentConfigurationAsync();
                
                HasUnsavedChanges = false;
            }
            else
            {
                StatusMessage = $"Print failed: {printStatus.Message}";
                Logger.LogError("Print operation failed: {Message}", printStatus.Message);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during print operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Print operation failed", Environment.UserName);
            StatusMessage = "Print operation failed";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Generate print preview
    /// </summary>
    [RelayCommand]
    private async Task GeneratePreviewAsync()
    {
        try
        {
            if (PrintData == null) return;

            IsPreviewLoading = true;
            StatusMessage = "Generating print preview...";

            var config = CreatePrintConfiguration();
            var preview = await _printService.GeneratePrintPreviewAsync(PrintData, config);
            
            PrintPreview = preview;
            
            // Calculate total pages (simplified)
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)PrintData.Rows.Count / 50));
            CurrentPage = 1;

            StatusMessage = "Print preview generated";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating print preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate print preview", Environment.UserName);
            StatusMessage = "Error generating preview";
        }
        finally
        {
            IsPreviewLoading = false;
        }
    }

    /// <summary>
    /// Show/hide layout customization panel
    /// </summary>
    [RelayCommand]
    private void ToggleLayoutPanel()
    {
        ShowLayoutPanel = !ShowLayoutPanel;
        StatusMessage = ShowLayoutPanel ? "Layout customization panel opened" : "Layout customization panel closed";
        Logger.LogDebug("Layout panel toggled to: {ShowLayoutPanel}", ShowLayoutPanel);
    }

    /// <summary>
    /// Zoom in print preview
    /// </summary>
    [RelayCommand]
    private void ZoomIn()
    {
        if (PreviewZoom < 3.0)
        {
            PreviewZoom += 0.25;
            StatusMessage = $"Zoom: {PreviewZoom:P0}";
        }
    }

    /// <summary>
    /// Zoom out print preview
    /// </summary>
    [RelayCommand]
    private void ZoomOut()
    {
        if (PreviewZoom > 0.25)
        {
            PreviewZoom -= 0.25;
            StatusMessage = $"Zoom: {PreviewZoom:P0}";
        }
    }

    /// <summary>
    /// Reset zoom to 100%
    /// </summary>
    [RelayCommand]
    private void ResetZoom()
    {
        PreviewZoom = 1.0;
        StatusMessage = "Zoom reset to 100%";
    }

    #endregion

    #region Template Commands

    /// <summary>
    /// Load selected template
    /// </summary>
    [RelayCommand]
    private async Task LoadTemplateAsync()
    {
        try
        {
            if (SelectedTemplate == null) return;

            IsLoading = true;
            StatusMessage = $"Loading template: {SelectedTemplate.Name}";

            // Apply template configuration
            var config = SelectedTemplate.Configuration;
            SelectedOrientation = config.Orientation;
            Copies = config.Copies;
            Collate = config.Collate;
            SelectedQuality = config.Quality;
            SelectedPaperSize = config.PaperSize;
            SelectedStyle = config.Style;
            IncludeHeaders = config.IncludeHeaders;
            IncludeFooters = config.IncludeFooters;
            IncludeGridLines = config.IncludeGridLines;
            IncludeTimestamp = config.IncludeTimestamp;
            IncludeUserInfo = config.IncludeUserInfo;
            FontSize = config.FontSize;
            FontFamily = config.FontFamily;
            DocumentTitle = config.DocumentTitle;
            CustomHeaderText = config.CustomHeaderText;
            CustomFooterText = config.CustomFooterText;

            // Apply column configuration
            ApplyTemplateColumns(SelectedTemplate.Columns);

            // Regenerate preview
            await GeneratePreviewAsync();

            HasUnsavedChanges = true;
            StatusMessage = $"Template '{SelectedTemplate.Name}' loaded successfully";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading template: {TemplateName}", SelectedTemplate?.Name);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load template: {SelectedTemplate?.Name}", Environment.UserName);
            StatusMessage = "Error loading template";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyTemplateColumns(List<TemplateColumnInfo> templateColumns)
    {
        try
        {
            foreach (var printColumn in PrintColumns)
            {
                var templateColumn = templateColumns.Find(tc => tc.ColumnId == printColumn.PropertyName);
                if (templateColumn != null)
                {
                    printColumn.Header = templateColumn.Header;
                    printColumn.IsVisible = templateColumn.IsVisible;
                    printColumn.DisplayOrder = templateColumn.DisplayOrder;
                    printColumn.Width = templateColumn.Width;
                    printColumn.Alignment = templateColumn.Alignment;
                }
            }

            Logger.LogDebug("Applied template column configuration");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error applying template columns");
        }
    }

    #endregion

    #region Navigation Commands

    /// <summary>
    /// Close print view and return to original context
    /// </summary>
    [RelayCommand]
    private async Task CloseAsync()
    {
        try
        {
            Logger.LogDebug("Closing print view");

            if (HasUnsavedChanges)
            {
                // TODO: Show confirmation dialog
                Logger.LogWarning("Closing with unsaved changes");
            }

            // Save current configuration before closing
            await SaveCurrentConfigurationAsync();

            // Navigate back to original view using NavigationService
            if (_navigationService != null)
            {
                if (OriginalViewContext != null)
                {
                    _navigationService.NavigateTo(OriginalViewContext);
                    Logger.LogDebug("Navigated back to original view context");
                }
                else
                {
                    // Navigate to MainView as fallback
                    var mainViewModel = Program.GetOptionalService<MainViewViewModel>();
                    if (mainViewModel != null)
                    {
                        var mainView = new Views.MainView
                        {
                            DataContext = mainViewModel
                        };
                        _navigationService.NavigateTo(mainView);
                        Logger.LogDebug("Navigated back to MainView as fallback");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error closing print view");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to close print view", Environment.UserName);
        }
    }

    #endregion

    #region Private Helper Methods

    private PrintConfiguration CreatePrintConfiguration()
    {
        return new PrintConfiguration
        {
            PrinterName = SelectedPrinter,
            Orientation = SelectedOrientation,
            Copies = Copies,
            Collate = Collate,
            Quality = SelectedQuality,
            PaperSize = SelectedPaperSize,
            Style = SelectedStyle,
            IncludeHeaders = IncludeHeaders,
            IncludeFooters = IncludeFooters,
            IncludeGridLines = IncludeGridLines,
            IncludeTimestamp = IncludeTimestamp,
            IncludeUserInfo = IncludeUserInfo,
            FontSize = FontSize,
            FontFamily = FontFamily,
            DocumentTitle = DocumentTitle,
            CustomHeaderText = CustomHeaderText,
            CustomFooterText = CustomFooterText,
            VisibleColumns = PrintColumns.ToList()
        };
    }

    private async Task SaveCurrentConfigurationAsync()
    {
        try
        {
            var config = CreatePrintConfiguration();
            await _printService.SavePrintConfigurationAsync(config, DataSourceType);
            HasUnsavedChanges = false;
            Logger.LogDebug("Current print configuration saved for {DataSourceType}", DataSourceType);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error saving current configuration");
        }
    }

    #endregion

    #region Property Change Handlers

    /// <summary>
    /// Handle changes to settings that affect preview
    /// </summary>
    partial void OnSelectedOrientationChanged(PrintOrientation value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnSelectedStyleChanged(PrintStyle value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnIncludeHeadersChanged(bool value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnIncludeFootersChanged(bool value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnIncludeGridLinesChanged(bool value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnFontSizeChanged(int value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    partial void OnSelectedPaperSizeChanged(Models.PaperSize value)
    {
        HasUnsavedChanges = true;
        _ = GeneratePreviewAsync();
    }

    #endregion
}