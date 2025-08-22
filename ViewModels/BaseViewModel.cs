using System;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace MTM_WIP_Application_Avalonia.ViewModels;

public abstract class BaseViewModel : ReactiveObject, IDisposable
{
    protected readonly ILogger Logger;
    private bool _isDisposed = false;

    protected BaseViewModel(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
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