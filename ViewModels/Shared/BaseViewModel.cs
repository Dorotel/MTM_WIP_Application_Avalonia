using System;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels.Shared;

public abstract class BaseViewModel : ReactiveObject, IDisposable
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
