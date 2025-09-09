using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Models;
using MTM_WIP_Application_Avalonia.Services;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using System.Drawing.Printing;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for print service operations
/// </summary>
public interface IPrintService
{
    /// <summary>
    /// Get all available printers
    /// </summary>
    Task<List<string>> GetAvailablePrintersAsync();

    /// <summary>
    /// Get print configuration for a data source
    /// </summary>
    Task<PrintConfiguration> GetPrintConfigurationAsync(PrintDataSourceType dataSourceType);

    /// <summary>
    /// Save print configuration
    /// </summary>
    Task<bool> SavePrintConfigurationAsync(PrintConfiguration configuration, PrintDataSourceType dataSourceType);

    /// <summary>
    /// Generate print preview for DataGrid data
    /// </summary>
    Task<Canvas> GeneratePrintPreviewAsync(DataTable data, PrintConfiguration configuration);

    /// <summary>
    /// Print DataGrid data
    /// </summary>
    Task<PrintStatus> PrintDataAsync(DataTable data, PrintConfiguration configuration);

    /// <summary>
    /// Get available print templates
    /// </summary>
    Task<List<PrintLayoutTemplate>> GetPrintTemplatesAsync(PrintDataSourceType? dataSourceType = null);

    /// <summary>
    /// Save print template
    /// </summary>
    Task<bool> SavePrintTemplateAsync(PrintLayoutTemplate template);

    /// <summary>
    /// Delete print template
    /// </summary>
    Task<bool> DeletePrintTemplateAsync(Guid templateId);

    /// <summary>
    /// Load print template by ID
    /// </summary>
    Task<PrintLayoutTemplate?> LoadPrintTemplateAsync(Guid templateId);
}

/// <summary>
/// Print service implementation for MTM WIP Application
/// Handles print operations, templates, and configuration management
/// </summary>
public class PrintService : IPrintService
{
    private readonly ILogger<PrintService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly string _templatesPath;
    private readonly string _configPath;

    public PrintService(
        ILogger<PrintService> logger,
        IConfigurationService configurationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));

        // Create paths for storing templates and configuration
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var mtmPath = Path.Combine(appDataPath, "MTM_WIP_Application");
        _templatesPath = Path.Combine(mtmPath, "PrintTemplates");
        _configPath = Path.Combine(mtmPath, "PrintConfigurations");

        // Ensure directories exist
        Directory.CreateDirectory(_templatesPath);
        Directory.CreateDirectory(_configPath);

        _logger.LogDebug("PrintService initialized with templates path: {TemplatesPath}, config path: {ConfigPath}", 
            _templatesPath, _configPath);
    }

    /// <summary>
    /// Get all available printers in the system
    /// </summary>
    public async Task<List<string>> GetAvailablePrintersAsync()
    {
        try
        {
            _logger.LogDebug("Getting available printers");
            
            var printers = new List<string>();
            
            await Task.Run(() =>
            {
                try
                {
                    // Get installed printers using System.Drawing.Printing (Windows only)
                    if (OperatingSystem.IsWindows())
                    {
                        foreach (string printerName in PrinterSettings.InstalledPrinters)
                        {
                            printers.Add(printerName);
                        }
                    }
                    else
                    {
                        // For non-Windows platforms, add placeholder printers
                        printers.Add("Default Printer");
                        printers.Add("PDF Printer");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accessing installed printers");
                }
            });

            // Ensure we always have at least one printer option
            if (printers.Count == 0)
            {
                printers.Add("Default Printer");
                _logger.LogWarning("No printers found, added default printer option");
            }

            _logger.LogInformation("Found {PrinterCount} available printers", printers.Count);
            return printers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available printers");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to get available printers", Environment.UserName);
            
            // Return default printer as fallback
            return new List<string> { "Default Printer" };
        }
    }

    /// <summary>
    /// Get print configuration for a specific data source type
    /// </summary>
    public async Task<PrintConfiguration> GetPrintConfigurationAsync(PrintDataSourceType dataSourceType)
    {
        try
        {
            _logger.LogDebug("Loading print configuration for data source: {DataSourceType}", dataSourceType);

            var configFile = Path.Combine(_configPath, $"{dataSourceType}.json");
            
            if (File.Exists(configFile))
            {
                var json = await File.ReadAllTextAsync(configFile);
                var config = JsonSerializer.Deserialize<PrintConfiguration>(json);
                
                if (config != null)
                {
                    _logger.LogDebug("Loaded saved print configuration for {DataSourceType}", dataSourceType);
                    return config;
                }
            }

            // Return default configuration if no saved config exists
            var defaultConfig = GetDefaultPrintConfiguration(dataSourceType);
            _logger.LogDebug("Using default print configuration for {DataSourceType}", dataSourceType);
            return defaultConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading print configuration for {DataSourceType}", dataSourceType);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load print configuration for {dataSourceType}", Environment.UserName);
            
            // Return default configuration as fallback
            return GetDefaultPrintConfiguration(dataSourceType);
        }
    }

    /// <summary>
    /// Save print configuration for a data source type
    /// </summary>
    public async Task<bool> SavePrintConfigurationAsync(PrintConfiguration configuration, PrintDataSourceType dataSourceType)
    {
        try
        {
            _logger.LogDebug("Saving print configuration for data source: {DataSourceType}", dataSourceType);

            var configFile = Path.Combine(_configPath, $"{dataSourceType}.json");
            var json = JsonSerializer.Serialize(configuration, new JsonSerializerOptions { WriteIndented = true });
            
            await File.WriteAllTextAsync(configFile, json);
            
            _logger.LogInformation("Print configuration saved successfully for {DataSourceType}", dataSourceType);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving print configuration for {DataSourceType}", dataSourceType);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to save print configuration for {dataSourceType}", Environment.UserName);
            return false;
        }
    }

    /// <summary>
    /// Generate print preview canvas for data
    /// </summary>
    public async Task<Canvas> GeneratePrintPreviewAsync(DataTable data, PrintConfiguration configuration)
    {
        try
        {
            _logger.LogDebug("Generating print preview for {RowCount} rows", data.Rows.Count);

            var canvas = new Canvas
            {
                Width = GetPageWidth(configuration.PaperSize),
                Height = GetPageHeight(configuration.PaperSize),
                Background = Avalonia.Media.Brushes.White
            };

            await Task.Run(() =>
            {
                // Generate print preview content
                Avalonia.Threading.Dispatcher.UIThread.Invoke(() =>
                {
                    GeneratePreviewContent(canvas, data, configuration);
                });
            });

            _logger.LogDebug("Print preview generated successfully");
            return canvas;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating print preview");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to generate print preview", Environment.UserName);
            
            // Return empty canvas as fallback
            return new Canvas { Background = Avalonia.Media.Brushes.White };
        }
    }

    /// <summary>
    /// Print data using the specified configuration
    /// </summary>
    public async Task<PrintStatus> PrintDataAsync(DataTable data, PrintConfiguration configuration)
    {
        try
        {
            _logger.LogInformation("Starting print operation for {RowCount} rows using printer: {PrinterName}", 
                data.Rows.Count, configuration.PrinterName);

            var printStatus = new PrintStatus
            {
                PrinterUsed = configuration.PrinterName,
                PrintTime = DateTime.Now
            };

            await Task.Run(() =>
            {
                try
                {
                    // TODO: Implement actual printing using System.Drawing.Printing
                    // For now, simulate successful printing
                    System.Threading.Thread.Sleep(2000); // Simulate print time
                    
                    printStatus.IsSuccess = true;
                    printStatus.Message = "Print completed successfully";
                    printStatus.PagesPrinted = CalculatePageCount(data, configuration);
                }
                catch (Exception printEx)
                {
                    printStatus.IsSuccess = false;
                    printStatus.Message = printEx.Message;
                    printStatus.Exception = printEx;
                }
            });

            if (printStatus.IsSuccess)
            {
                _logger.LogInformation("Print operation completed successfully. Pages printed: {PagesPrinted}", 
                    printStatus.PagesPrinted);
            }
            else
            {
                _logger.LogError("Print operation failed: {Message}", printStatus.Message);
                await Services.ErrorHandling.HandleErrorAsync(
                    printStatus.Exception ?? new Exception(printStatus.Message), 
                    "Print operation failed", 
                    Environment.UserName);
            }

            return printStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during print operation");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Print operation error", Environment.UserName);
            
            return new PrintStatus
            {
                IsSuccess = false,
                Message = ex.Message,
                Exception = ex,
                PrintTime = DateTime.Now,
                PrinterUsed = configuration.PrinterName
            };
        }
    }

    /// <summary>
    /// Get available print templates
    /// </summary>
    public async Task<List<PrintLayoutTemplate>> GetPrintTemplatesAsync(PrintDataSourceType? dataSourceType = null)
    {
        try
        {
            _logger.LogDebug("Loading print templates for data source type: {DataSourceType}", dataSourceType?.ToString() ?? "All");

            var templates = new List<PrintLayoutTemplate>();

            // Add default system templates
            templates.AddRange(DefaultPrintTemplates.GetAllDefaultTemplates());

            // Load user-created templates
            var templateFiles = Directory.GetFiles(_templatesPath, "*.json");
            
            foreach (var file in templateFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var template = JsonSerializer.Deserialize<PrintLayoutTemplate>(json);
                    
                    if (template != null)
                    {
                        if (dataSourceType == null || template.DataSourceType == dataSourceType)
                        {
                            templates.Add(template);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error loading template file: {FileName}", Path.GetFileName(file));
                }
            }

            // Filter by data source type if specified
            if (dataSourceType.HasValue)
            {
                templates = templates.FindAll(t => t.DataSourceType == dataSourceType.Value);
            }

            _logger.LogInformation("Loaded {TemplateCount} print templates", templates.Count);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading print templates");
            await Services.ErrorHandling.HandleErrorAsync(ex, "Failed to load print templates", Environment.UserName);
            
            // Return default templates as fallback
            return DefaultPrintTemplates.GetAllDefaultTemplates();
        }
    }

    /// <summary>
    /// Save a print template
    /// </summary>
    public async Task<bool> SavePrintTemplateAsync(PrintLayoutTemplate template)
    {
        try
        {
            _logger.LogDebug("Saving print template: {TemplateName}", template.Name);

            template.LastModified = DateTime.Now;
            
            var fileName = $"{template.Id}.json";
            var filePath = Path.Combine(_templatesPath, fileName);
            
            var json = JsonSerializer.Serialize(template, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, json);
            
            _logger.LogInformation("Print template saved successfully: {TemplateName}", template.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving print template: {TemplateName}", template.Name);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to save print template: {template.Name}", Environment.UserName);
            return false;
        }
    }

    /// <summary>
    /// Delete a print template
    /// </summary>
    public async Task<bool> DeletePrintTemplateAsync(Guid templateId)
    {
        try
        {
            _logger.LogDebug("Deleting print template: {TemplateId}", templateId);

            var fileName = $"{templateId}.json";
            var filePath = Path.Combine(_templatesPath, fileName);
            
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
                _logger.LogInformation("Print template deleted successfully: {TemplateId}", templateId);
                return true;
            }
            else
            {
                _logger.LogWarning("Print template file not found: {TemplateId}", templateId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting print template: {TemplateId}", templateId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to delete print template: {templateId}", Environment.UserName);
            return false;
        }
    }

    /// <summary>
    /// Load a print template by ID
    /// </summary>
    public async Task<PrintLayoutTemplate?> LoadPrintTemplateAsync(Guid templateId)
    {
        try
        {
            _logger.LogDebug("Loading print template: {TemplateId}", templateId);

            // Check system templates first
            var systemTemplates = DefaultPrintTemplates.GetAllDefaultTemplates();
            var systemTemplate = systemTemplates.Find(t => t.Id == templateId);
            if (systemTemplate != null)
            {
                return systemTemplate;
            }

            // Load from file
            var fileName = $"{templateId}.json";
            var filePath = Path.Combine(_templatesPath, fileName);
            
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var template = JsonSerializer.Deserialize<PrintLayoutTemplate>(json);
                
                _logger.LogDebug("Print template loaded successfully: {TemplateId}", templateId);
                return template;
            }
            else
            {
                _logger.LogWarning("Print template file not found: {TemplateId}", templateId);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading print template: {TemplateId}", templateId);
            await Services.ErrorHandling.HandleErrorAsync(ex, $"Failed to load print template: {templateId}", Environment.UserName);
            return null;
        }
    }

    #region Private Helper Methods

    private PrintConfiguration GetDefaultPrintConfiguration(PrintDataSourceType dataSourceType)
    {
        return new PrintConfiguration
        {
            PrinterName = "Default Printer",
            Orientation = PrintOrientation.Portrait,
            Copies = 1,
            Collate = true,
            Quality = PrintQuality.Normal,
            PaperSize = Models.PaperSize.Letter,
            Style = PrintStyle.Simple,
            IncludeHeaders = true,
            IncludeFooters = true,
            IncludeGridLines = true,
            FontSize = 10,
            FontFamily = "Arial",
            DocumentTitle = $"{dataSourceType} Report",
            IncludeTimestamp = true,
            IncludeUserInfo = true
        };
    }

    private void GeneratePreviewContent(Canvas canvas, DataTable data, PrintConfiguration configuration)
    {
        // TODO: Implement sophisticated print preview generation
        // This would create visual representation of the printed document
        
        // For now, add a simple placeholder
        var textBlock = new Avalonia.Controls.TextBlock
        {
            Text = $"Print Preview\n{configuration.DocumentTitle}\n{data.Rows.Count} rows",
            FontSize = 14,
            Margin = new Avalonia.Thickness(20)
        };
        
        canvas.Children.Add(textBlock);
    }

    private double GetPageWidth(Models.PaperSize paperSize)
    {
        return paperSize switch
        {
            Models.PaperSize.Letter => 8.5 * 96, // 8.5 inches * 96 DPI
            Models.PaperSize.Legal => 8.5 * 96,
            Models.PaperSize.A4 => 8.27 * 96,
            Models.PaperSize.A3 => 11.7 * 96,
            Models.PaperSize.Tabloid => 11 * 96,
            _ => 8.5 * 96
        };
    }

    private double GetPageHeight(Models.PaperSize paperSize)
    {
        return paperSize switch
        {
            Models.PaperSize.Letter => 11 * 96, // 11 inches * 96 DPI
            Models.PaperSize.Legal => 14 * 96,
            Models.PaperSize.A4 => 11.7 * 96,
            Models.PaperSize.A3 => 16.5 * 96,
            Models.PaperSize.Tabloid => 17 * 96,
            _ => 11 * 96
        };
    }

    private int CalculatePageCount(DataTable data, PrintConfiguration configuration)
    {
        // Simple page calculation - would be more sophisticated in real implementation
        var rowsPerPage = configuration.FontSize switch
        {
            <= 8 => 60,
            <= 10 => 50,
            <= 12 => 40,
            _ => 30
        };

        return Math.Max(1, (int)Math.Ceiling((double)data.Rows.Count / rowsPerPage));
    }

    #endregion
}