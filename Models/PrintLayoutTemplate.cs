using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Models;

/// <summary>
/// Print layout template for saving and reusing column configurations and print settings
/// </summary>
public class PrintLayoutTemplate
{
    /// <summary>
    /// Unique identifier for the template
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// User-friendly name for the template
    /// </summary>
    [Required(ErrorMessage = "Template name is required")]
    [StringLength(100, ErrorMessage = "Template name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the template
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data source type this template is designed for
    /// </summary>
    public PrintDataSourceType DataSourceType { get; set; } = PrintDataSourceType.Inventory;

    /// <summary>
    /// User who created this template
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// When this template was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    /// <summary>
    /// When this template was last modified
    /// </summary>
    public DateTime LastModified { get; set; } = DateTime.Now;

    /// <summary>
    /// Whether this is a system template (read-only)
    /// </summary>
    public bool IsSystemTemplate { get; set; } = false;

    /// <summary>
    /// Whether this template is shared with all users
    /// </summary>
    public bool IsShared { get; set; } = false;

    /// <summary>
    /// Print configuration settings
    /// </summary>
    public PrintConfiguration Configuration { get; set; } = new();

    /// <summary>
    /// Column layout configuration
    /// </summary>
    public List<TemplateColumnInfo> Columns { get; set; } = new();

    /// <summary>
    /// Template usage statistics
    /// </summary>
    public TemplateUsageStats UsageStats { get; set; } = new();
}

/// <summary>
/// Data source types that can be printed
/// </summary>
public enum PrintDataSourceType
{
    Inventory,
    Transactions,
    Remove,
    Transfer,
    QuickButtons,
    Custom
}

/// <summary>
/// Column information for template storage
/// </summary>
public class TemplateColumnInfo
{
    /// <summary>
    /// Column identifier (property name or key)
    /// </summary>
    public string ColumnId { get; set; } = string.Empty;

    /// <summary>
    /// Display header text
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Whether column is visible in print
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Display order (0-based)
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Column width in print units
    /// </summary>
    public double Width { get; set; } = 100;

    /// <summary>
    /// Text alignment
    /// </summary>
    public PrintAlignment Alignment { get; set; } = PrintAlignment.Left;

    /// <summary>
    /// Whether to wrap text in this column
    /// </summary>
    public bool WrapText { get; set; } = false;

    /// <summary>
    /// Custom format string for data display
    /// </summary>
    public string FormatString { get; set; } = string.Empty;
}

/// <summary>
/// Template usage statistics
/// </summary>
public class TemplateUsageStats
{
    /// <summary>
    /// Number of times this template has been used
    /// </summary>
    public int UseCount { get; set; } = 0;

    /// <summary>
    /// Last time this template was used
    /// </summary>
    public DateTime? LastUsed { get; set; }

    /// <summary>
    /// Average pages printed per use
    /// </summary>
    public double AveragePagesPerUse { get; set; } = 0;

    /// <summary>
    /// Total pages printed using this template
    /// </summary>
    public int TotalPagesPrinted { get; set; } = 0;
}

/// <summary>
/// Built-in template definitions
/// </summary>
public static class DefaultPrintTemplates
{
    /// <summary>
    /// Get default inventory template
    /// </summary>
    public static PrintLayoutTemplate GetInventoryTemplate()
    {
        return new PrintLayoutTemplate
        {
            Name = "Default Inventory Report",
            Description = "Standard inventory listing with all essential columns",
            DataSourceType = PrintDataSourceType.Inventory,
            IsSystemTemplate = true,
            CreatedBy = "System",
            Columns = new List<TemplateColumnInfo>
            {
                new() { ColumnId = "PartId", Header = "Part ID", IsVisible = true, DisplayOrder = 0, Width = 120, Alignment = PrintAlignment.Left },
                new() { ColumnId = "Operation", Header = "Operation", IsVisible = true, DisplayOrder = 1, Width = 80, Alignment = PrintAlignment.Center },
                new() { ColumnId = "Quantity", Header = "Quantity", IsVisible = true, DisplayOrder = 2, Width = 80, Alignment = PrintAlignment.Right },
                new() { ColumnId = "Location", Header = "Location", IsVisible = true, DisplayOrder = 3, Width = 100, Alignment = PrintAlignment.Left },
                new() { ColumnId = "LastUpdated", Header = "Last Updated", IsVisible = true, DisplayOrder = 4, Width = 140, Alignment = PrintAlignment.Center, FormatString = "MM/dd/yyyy HH:mm" },
                new() { ColumnId = "LastUpdatedBy", Header = "Updated By", IsVisible = true, DisplayOrder = 5, Width = 100, Alignment = PrintAlignment.Left }
            },
            Configuration = new PrintConfiguration
            {
                DocumentTitle = "Inventory Report",
                Style = PrintStyle.Simple,
                IncludeHeaders = true,
                IncludeFooters = true,
                IncludeGridLines = true,
                FontSize = 10,
                FontFamily = "Arial"
            }
        };
    }

    /// <summary>
    /// Get default transactions template
    /// </summary>
    public static PrintLayoutTemplate GetTransactionsTemplate()
    {
        return new PrintLayoutTemplate
        {
            Name = "Default Transaction History",
            Description = "Standard transaction history with timestamps and user information",
            DataSourceType = PrintDataSourceType.Transactions,
            IsSystemTemplate = true,
            CreatedBy = "System",
            Columns = new List<TemplateColumnInfo>
            {
                new() { ColumnId = "Timestamp", Header = "Date/Time", IsVisible = true, DisplayOrder = 0, Width = 140, Alignment = PrintAlignment.Center, FormatString = "MM/dd/yyyy HH:mm" },
                new() { ColumnId = "PartId", Header = "Part ID", IsVisible = true, DisplayOrder = 1, Width = 120, Alignment = PrintAlignment.Left },
                new() { ColumnId = "Operation", Header = "Operation", IsVisible = true, DisplayOrder = 2, Width = 80, Alignment = PrintAlignment.Center },
                new() { ColumnId = "TransactionType", Header = "Type", IsVisible = true, DisplayOrder = 3, Width = 60, Alignment = PrintAlignment.Center },
                new() { ColumnId = "Quantity", Header = "Quantity", IsVisible = true, DisplayOrder = 4, Width = 80, Alignment = PrintAlignment.Right },
                new() { ColumnId = "Location", Header = "Location", IsVisible = true, DisplayOrder = 5, Width = 100, Alignment = PrintAlignment.Left },
                new() { ColumnId = "UserId", Header = "User", IsVisible = true, DisplayOrder = 6, Width = 100, Alignment = PrintAlignment.Left }
            },
            Configuration = new PrintConfiguration
            {
                DocumentTitle = "Transaction History Report",
                Style = PrintStyle.Simple,
                IncludeHeaders = true,
                IncludeFooters = true,
                IncludeGridLines = true,
                FontSize = 10,
                FontFamily = "Arial"
            }
        };
    }

    /// <summary>
    /// Get default remove operations template
    /// </summary>
    public static PrintLayoutTemplate GetRemoveTemplate()
    {
        return new PrintLayoutTemplate
        {
            Name = "Default Remove Operations",
            Description = "Remove operations report with quantities and locations",
            DataSourceType = PrintDataSourceType.Remove,
            IsSystemTemplate = true,
            CreatedBy = "System",
            Columns = new List<TemplateColumnInfo>
            {
                new() { ColumnId = "PartId", Header = "Part ID", IsVisible = true, DisplayOrder = 0, Width = 120, Alignment = PrintAlignment.Left },
                new() { ColumnId = "Operation", Header = "Operation", IsVisible = true, DisplayOrder = 1, Width = 80, Alignment = PrintAlignment.Center },
                new() { ColumnId = "CurrentQuantity", Header = "Current Qty", IsVisible = true, DisplayOrder = 2, Width = 90, Alignment = PrintAlignment.Right },
                new() { ColumnId = "RemoveQuantity", Header = "Remove Qty", IsVisible = true, DisplayOrder = 3, Width = 90, Alignment = PrintAlignment.Right },
                new() { ColumnId = "Location", Header = "Location", IsVisible = true, DisplayOrder = 4, Width = 100, Alignment = PrintAlignment.Left },
                new() { ColumnId = "Status", Header = "Status", IsVisible = true, DisplayOrder = 5, Width = 80, Alignment = PrintAlignment.Center }
            },
            Configuration = new PrintConfiguration
            {
                DocumentTitle = "Remove Operations Report",
                Style = PrintStyle.Simple,
                IncludeHeaders = true,
                IncludeFooters = true,
                IncludeGridLines = true,
                FontSize = 10,
                FontFamily = "Arial"
            }
        };
    }

    /// <summary>
    /// Get all default templates
    /// </summary>
    public static List<PrintLayoutTemplate> GetAllDefaultTemplates()
    {
        return new List<PrintLayoutTemplate>
        {
            GetInventoryTemplate(),
            GetTransactionsTemplate(),
            GetRemoveTemplate()
        };
    }
}