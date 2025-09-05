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

    /// <summary>
    /// Parameterless constructor for design-time support and XAML binding.
    /// Creates a design-time safe logger that doesn't require dependency injection.
    /// </summary>
    protected BaseViewModel() : this(CreateDesignTimeLogger())
    {
    }

    /// <summary>
    /// Main constructor that accepts an ILogger instance from dependency injection.
    /// This is the primary constructor used at runtime with proper service injection.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic and debugging information</param>
    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? CreateDesignTimeLogger();
    }

    /// <summary>
    /// Creates a design-time safe logger that works without dependency injection.
    /// Used during XAML design-time and when the main constructor parameter is null.
    /// </summary>
    /// <returns>A console logger for design-time use, or NullLogger if console logging fails</returns>
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
    /// Enhanced SetProperty with logging - uses MVVM Community Toolkit's SetProperty internally.
    /// Provides detailed logging of property changes for debugging and diagnostics.
    /// </summary>
    /// <typeparam name="T">Type of the property being set</typeparam>
    /// <param name="field">Reference to the backing field</param>
    /// <param name="newValue">The new value to set</param>
    /// <param name="propertyName">Name of the property (automatically provided by CallerMemberName)</param>
    /// <returns>True if the property value changed, false if the value was the same</returns>
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
    /// Override of OnPropertyChanged to add comprehensive logging for property change events.
    /// Provides trace-level logging for all property changes and error handling.
    /// </summary>
    /// <param name="e">PropertyChangedEventArgs containing the name of the changed property</param>
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

    /// <summary>
    /// Releases managed and unmanaged resources used by this ViewModel.
    /// Override this method in derived classes to dispose of specific resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer</param>
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

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
    /// Implements the IDisposable pattern for proper resource management.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

