using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MTM_WIP_Application_Avalonia.ViewModels.Shared;

/// <summary>
/// Base ViewModel using MVVM Community Toolkit's ObservableValidator for property change notifications and validation
/// Provides design-time safe logging, validation support, and proper disposal patterns
/// </summary>
public abstract partial class BaseViewModel : ObservableValidator, IDisposable
{
    protected readonly ILogger Logger;
    private bool _isDisposed = false;

    // Design-time safe constructor
    protected BaseViewModel() : this(CreateDesignTimeLogger())
    {
    }

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? CreateDesignTimeLogger();
    }

    private static ILogger CreateDesignTimeLogger()
    {
        // Create a design-time safe logger that doesn't require DI
        try
        {
            using var loggerFactory = LoggerFactory.Create(builder => 
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning); // Reduce noise in design mode
            });
            return loggerFactory.CreateLogger("DesignTime");
        }
        catch
        {
            // Fallback to null logger if console logger fails
            return Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;
        }
    }

    /// <summary>
    /// Enhanced SetProperty with logging - uses MVVM Community Toolkit's SetProperty internally
    /// </summary>
    protected bool SetPropertyWithLogging<T>(ref T field, T newValue, string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, newValue)) return false;
        
        var oldValue = field;
        var result = SetProperty(ref field, newValue, propertyName);
        
        if (result)
        {
            Logger.LogDebug("Property changed: {PropertyName} from '{OldValue}' to '{NewValue}' in {ViewModelType}", 
                propertyName, oldValue, newValue, GetType().Name);
        }
            
        return result;
    }

    /// <summary>
    /// Override OnPropertyChanged to add logging
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        try
        {
            base.OnPropertyChanged(e);
            Logger.LogTrace("PropertyChanged event raised for: {PropertyName} in {ViewModelType}", e.PropertyName, GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error raising PropertyChanged event for property: {PropertyName} in {ViewModelType}", e.PropertyName, GetType().Name);
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                Logger.LogDebug("Disposing {ViewModelType}", GetType().Name);
            }
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

