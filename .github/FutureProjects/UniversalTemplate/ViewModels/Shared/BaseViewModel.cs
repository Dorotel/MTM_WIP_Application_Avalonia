using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YourProject.ViewModels.Shared;

/// <summary>
/// Universal Base ViewModel using MVVM Community Toolkit's ObservableValidator
/// Provides design-time safe logging, validation support, and proper disposal patterns
/// Extracted from MTM WIP Application for reuse in new .NET 8 Avalonia projects
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
    /// Centralized error handling for ViewModels
    /// </summary>
    protected async Task HandleErrorAsync(Exception exception, string operation)
    {
        Logger.LogError(exception, "Error in {Operation}: {Message}", operation, exception.Message);
        
        // TODO: Integrate with your application's error handling service
        // await YourErrorHandlingService.HandleErrorAsync(exception, operation);
    }

    /// <summary>
    /// Proper disposal pattern implementation
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                OnDisposing();
            }
            
            _isDisposed = true;
        }
    }

    /// <summary>
    /// Override this method to dispose of resources in derived ViewModels
    /// </summary>
    protected virtual void OnDisposing()
    {
        // Override in derived classes to clean up resources
    }

    /// <summary>
    /// Async initialization pattern for ViewModels that need to load data
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        try
        {
            await OnInitializeAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex, "ViewModel initialization");
        }
    }

    /// <summary>
    /// Override this method to perform async initialization in derived ViewModels
    /// </summary>
    protected virtual Task OnInitializeAsync()
    {
        return Task.CompletedTask;
    }
}