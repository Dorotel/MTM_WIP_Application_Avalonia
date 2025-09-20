using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.Models.Print;

/// <summary>
/// Configuration model for print operations
/// Contains printer settings, layout options, and print parameters
/// </summary>
public class PrintConfiguration
{
    /// <summary>
    /// Selected printer name
    /// </summary>
    public string PrinterName { get; set; } = string.Empty;

    /// <summary>
    /// Paper orientation: Portrait or Landscape
    /// </summary>
    public PrintOrientation Orientation { get; set; } = PrintOrientation.Portrait;

    /// <summary>
    /// Number of copies to print
    /// </summary>
    [Range(1, 999, ErrorMessage = "Copies must be between 1 and 999")]
    public int Copies { get; set; } = 1;

    /// <summary>
    /// Whether to collate multiple copies
    /// </summary>
    public bool Collate { get; set; } = true;

    /// <summary>
    /// Print quality setting
    /// </summary>
    public PrintQuality Quality { get; set; } = PrintQuality.Normal;

    /// <summary>
    /// Paper size selection
    /// </summary>
    public PaperSize PaperSize { get; set; } = PaperSize.Letter;

    /// <summary>
    /// Print margins in inches
    /// </summary>
    public PrintMargins Margins { get; set; } = new();

    /// <summary>
    /// Print layout style
    /// </summary>
    public PrintStyle Style { get; set; } = PrintStyle.Simple;

    /// <summary>
    /// Whether to include headers
    /// </summary>
    public bool IncludeHeaders { get; set; } = true;

    /// <summary>
    /// Whether to include footers
    /// </summary>
    public bool IncludeFooters { get; set; } = true;

    /// <summary>
    /// Whether to include grid lines
    /// </summary>
    public bool IncludeGridLines { get; set; } = true;

    /// <summary>
    /// Font size for print output
    /// </summary>
    [Range(8, 72, ErrorMessage = "Font size must be between 8 and 72")]
    public int FontSize { get; set; } = 10;

    /// <summary>
    /// Font family for print output
    /// </summary>
    public string FontFamily { get; set; } = "Arial";

    /// <summary>
    /// Title to display on printed document
    /// </summary>
    public string DocumentTitle { get; set; } = string.Empty;

    /// <summary>
    /// Whether to include timestamp on document
    /// </summary>
    public bool IncludeTimestamp { get; set; } = true;

    /// <summary>
    /// Whether to include user information
    /// </summary>
    public bool IncludeUserInfo { get; set; } = true;

    /// <summary>
    /// Visible columns for print output
    /// </summary>
    public List<PrintColumnInfo> VisibleColumns { get; set; } = new();

    /// <summary>
    /// Custom header text
    /// </summary>
    public string CustomHeaderText { get; set; } = string.Empty;

    /// <summary>
    /// Custom footer text
    /// </summary>
    public string CustomFooterText { get; set; } = string.Empty;
}

/// <summary>
/// Print orientation enumeration
/// </summary>
public enum PrintOrientation
{
    Portrait,
    Landscape
}

/// <summary>
/// Print quality enumeration
/// </summary>
public enum PrintQuality
{
    Draft,
    Normal,
    High
}

/// <summary>
/// Paper size enumeration
/// </summary>
public enum PaperSize
{
    Letter,
    Legal,
    A4,
    A3,
    Tabloid
}

/// <summary>
/// Print style enumeration
/// </summary>
public enum PrintStyle
{
    Simple,
    Stylized
}

/// <summary>
/// Print margins configuration
/// </summary>
public class PrintMargins
{
    [Range(0, 5, ErrorMessage = "Left margin must be between 0 and 5 inches")]
    public double Left { get; set; } = 0.75;

    [Range(0, 5, ErrorMessage = "Right margin must be between 0 and 5 inches")]
    public double Right { get; set; } = 0.75;

    [Range(0, 5, ErrorMessage = "Top margin must be between 0 and 5 inches")]
    public double Top { get; set; } = 1.0;

    [Range(0, 5, ErrorMessage = "Bottom margin must be between 0 and 5 inches")]
    public double Bottom { get; set; } = 1.0;
}

/// <summary>
/// Information about a printable column
/// </summary>
public class PrintColumnInfo
{
    /// <summary>
    /// Column header text
    /// </summary>
    public string Header { get; set; } = string.Empty;

    /// <summary>
    /// Column property name for data binding
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Column width in print units
    /// </summary>
    public double Width { get; set; } = 100;

    /// <summary>
    /// Whether this column is visible in print output
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Column alignment
    /// </summary>
    public PrintAlignment Alignment { get; set; } = PrintAlignment.Left;

    /// <summary>
    /// Display order in print output
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Print alignment enumeration
/// </summary>
public enum PrintAlignment
{
    Left,
    Center,
    Right
}

/// <summary>
/// Print status information
/// </summary>
public class PrintStatus
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public Exception? Exception { get; set; }
    public DateTime PrintTime { get; set; } = DateTime.Now;
    public int PagesPrinted { get; set; }
    public string PrinterUsed { get; set; } = string.Empty;
}
