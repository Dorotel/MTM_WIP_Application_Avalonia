using System;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTM_WIP_Application_Avalonia.ViewModels.Shared;

public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
{
    protected readonly ILogger Logger;
    private bool _isDisposed = false;

    public event PropertyChangedEventHandler? PropertyChanged;

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
    /// Sets a property value and raises PropertyChanged if the value has changed
    /// Replaces ReactiveUI's RaiseAndSetIfChanged with standard INotifyPropertyChanged pattern
    /// </summary>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        
        var oldValue = field;
        field = value;
        
        Logger.LogDebug("Property changed: {PropertyName} from '{OldValue}' to '{NewValue}' in {ViewModelType}", 
            propertyName, oldValue, value, GetType().Name);
            
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        try
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Logger.LogTrace("PropertyChanged event raised for: {PropertyName} in {ViewModelType}", propertyName, GetType().Name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error raising PropertyChanged event for property: {PropertyName} in {ViewModelType}", propertyName, GetType().Name);
            throw;
        }
    }

    /// <summary>
    /// Raises PropertyChanged for a specific property name
    /// Replaces ReactiveUI's RaisePropertyChanged
    /// </summary>
    protected void RaisePropertyChanged(string propertyName)
    {
        OnPropertyChanged(propertyName);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources
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

