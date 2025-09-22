using System;
using Avalonia;
using Microsoft.Extensions.Logging;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Interface for resolution-independent UI sizing service
/// </summary>
public interface IResolutionIndependentSizingService
{
    /// <summary>
    /// Gets a scaled size based on the current display scaling factor
    /// </summary>
    double GetScaledSize(double baseSize);

    /// <summary>
    /// Gets the recommended touch target size for manufacturing applications
    /// </summary>
    double GetManufacturingTouchTargetSize();

    /// <summary>
    /// Gets standard padding for consistent spacing
    /// </summary>
    Thickness GetStandardPadding();

    /// <summary>
    /// Gets standard font size scaled for current display
    /// </summary>
    double GetStandardFontSize();

    /// <summary>
    /// Gets standard control height scaled for current display
    /// </summary>
    double GetStandardControlHeight();

    /// <summary>
    /// Gets current display scaling factor
    /// </summary>
    double GetDisplayScalingFactor();
}

/// <summary>
/// Service for providing consistent, resolution-independent UI sizing across platforms
/// Phase 1: Foundation implementation for Material Design integration
/// </summary>
public class ResolutionIndependentSizingService : IResolutionIndependentSizingService
{
    private readonly ILogger<ResolutionIndependentSizingService> _logger;
    private double _cachedScalingFactor = 1.0;
    private DateTime _lastScalingUpdate = DateTime.MinValue;
    private static readonly TimeSpan ScalingCacheTimeout = TimeSpan.FromSeconds(5);

    // Base sizes for Material Design compliance
    private const double BaseFontSize = 14.0;
    private const double BaseControlHeight = 40.0;
    private const double BaseManufacturingTouchTarget = 48.0; // Material Design minimum touch target
    private const double BasePadding = 16.0;

    public ResolutionIndependentSizingService(ILogger<ResolutionIndependentSizingService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        UpdateScalingFactor();

        _logger.LogInformation("ResolutionIndependentSizingService initialized with scaling factor: {ScalingFactor}", _cachedScalingFactor);
    }

    /// <summary>
    /// Gets a scaled size based on the current display scaling factor
    /// </summary>
    public double GetScaledSize(double baseSize)
    {
        if (baseSize <= 0)
        {
            _logger.LogWarning("Invalid base size provided: {BaseSize}", baseSize);
            return 0;
        }

        UpdateScalingFactorIfNeeded();
        var scaledSize = Math.Round(baseSize * _cachedScalingFactor, 1);

        _logger.LogTrace("Scaled size {BaseSize} to {ScaledSize} with factor {ScalingFactor}",
            baseSize, scaledSize, _cachedScalingFactor);

        return scaledSize;
    }

    /// <summary>
    /// Gets the recommended touch target size for manufacturing applications
    /// Ensures minimum 48dp touch targets for accessibility and manufacturing use
    /// </summary>
    public double GetManufacturingTouchTargetSize()
    {
        var touchTargetSize = GetScaledSize(BaseManufacturingTouchTarget);

        // Ensure minimum size for manufacturing environments (accessibility compliance)
        var minimumSize = Math.Max(touchTargetSize, 44.0);

        _logger.LogTrace("Manufacturing touch target size: {Size}", minimumSize);
        return minimumSize;
    }

    /// <summary>
    /// Gets standard padding for consistent spacing following Material Design principles
    /// </summary>
    public Thickness GetStandardPadding()
    {
        var paddingValue = GetScaledSize(BasePadding);
        var padding = new Thickness(paddingValue);

        _logger.LogTrace("Standard padding: {Padding}", padding);
        return padding;
    }

    /// <summary>
    /// Gets standard font size scaled for current display
    /// </summary>
    public double GetStandardFontSize()
    {
        var fontSize = GetScaledSize(BaseFontSize);

        // Ensure minimum readable size
        var minimumFontSize = Math.Max(fontSize, 12.0);

        _logger.LogTrace("Standard font size: {FontSize}", minimumFontSize);
        return minimumFontSize;
    }

    /// <summary>
    /// Gets standard control height scaled for current display
    /// </summary>
    public double GetStandardControlHeight()
    {
        var controlHeight = GetScaledSize(BaseControlHeight);

        // Ensure minimum usable height
        var minimumHeight = Math.Max(controlHeight, 36.0);

        _logger.LogTrace("Standard control height: {Height}", minimumHeight);
        return minimumHeight;
    }

    /// <summary>
    /// Gets current display scaling factor
    /// </summary>
    public double GetDisplayScalingFactor()
    {
        UpdateScalingFactorIfNeeded();
        return _cachedScalingFactor;
    }

    /// <summary>
    /// Updates the scaling factor if cache has expired
    /// </summary>
    private void UpdateScalingFactorIfNeeded()
    {
        if (DateTime.UtcNow - _lastScalingUpdate > ScalingCacheTimeout)
        {
            UpdateScalingFactor();
        }
    }

    /// <summary>
    /// Updates the cached scaling factor from the display
    /// </summary>
    private void UpdateScalingFactor()
    {
        try
        {
            // For Phase 1, use a safe default scaling factor
            // In future phases, we can integrate with Avalonia's screen detection
            _cachedScalingFactor = 1.0;
            _lastScalingUpdate = DateTime.UtcNow;

            _logger.LogDebug("Updated scaling factor to default: {ScalingFactor}", _cachedScalingFactor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating scaling factor, using cached value: {ScalingFactor}", _cachedScalingFactor);
        }
    }
}
