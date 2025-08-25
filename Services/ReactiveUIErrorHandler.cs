using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Microsoft.Extensions.Logging;
using MTM_WIP_Application_Avalonia.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Concurrency;

namespace MTM_WIP_Application_Avalonia.Services;

/// <summary>
/// Global ReactiveUI error handler that integrates with MTM error handling system
/// to prevent unhandled observable pipeline errors from crashing the application
/// </summary>
public static class ReactiveUIErrorHandler
{
    private static ILogger? _logger;
    private static bool _isInitialized = false;

    /// <summary>
    /// Initializes the global ReactiveUI error handler to prevent pipeline breaks
    /// </summary>
    /// <param name="logger">Logger instance for error logging</param>
    public static void Initialize(ILogger? logger = null)
    {
        if (_isInitialized) return;

        _logger = logger;

        try
        {
            // Override the default ReactiveUI exception handler
            RxApp.DefaultExceptionHandler = Observer.Create<Exception>(async ex =>
            {
                try
                {
                    // Log the error using MTM error handling system
                    if (_logger != null)
                    {
                        _logger.LogError(ex, "ReactiveUI unhandled observable error: {Message}", ex.Message);
                    }

                    // Use MTM centralized error handling
                    await Service_ErrorHandler.HandleErrorAsync(
                        ex, 
                        "ReactiveUIObservablePipeline", 
                        Environment.UserName,
                        new Dictionary<string, object>
                        {
                            ["Component"] = "ReactiveUI",
                            ["ErrorType"] = "UnhandledObservableError",
                            ["Source"] = ex.Source ?? "Unknown"
                        });

                    // Don't throw or break the application - just log and continue
                    Console.WriteLine($"ReactiveUI error handled gracefully: {ex.Message}");
                }
                catch (Exception handlerEx)
                {
                    // Fallback error handling if our error handler fails
                    Console.WriteLine($"Critical: ReactiveUI error handler failed: {handlerEx.Message}");
                    Console.WriteLine($"Original error: {ex.Message}");
                }
            });

            _isInitialized = true;
            
            if (_logger != null)
            {
                _logger.LogInformation("ReactiveUI global error handler initialized successfully");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize ReactiveUI error handler: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates an error-safe observable that catches exceptions and handles them gracefully
    /// </summary>
    /// <typeparam name="T">The type of the observable</typeparam>
    /// <param name="source">The source observable</param>
    /// <param name="defaultValue">Default value to return on error</param>
    /// <param name="context">Additional context for error handling</param>
    /// <returns>Error-safe observable</returns>
    public static IObservable<T> MakeSafe<T>(
        this IObservable<T> source, 
        T defaultValue = default(T)!, 
        string? context = null)
    {
        return source.Catch<T, Exception>(ex =>
        {
            // Log the error without breaking the pipeline
            Task.Run(async () =>
            {
                await Service_ErrorHandler.HandleErrorAsync(
                    ex,
                    context ?? "SafeObservable",
                    Environment.UserName,
                    new Dictionary<string, object>
                    {
                        ["Component"] = "SafeObservable",
                        ["Context"] = context ?? "Unknown"
                    });
            });

            // Return default value and continue the pipeline
            return Observable.Return(defaultValue);
        });
    }

    /// <summary>
    /// Creates an error-safe ReactiveCommand that handles exceptions gracefully
    /// </summary>
    /// <param name="execute">The command execution function</param>
    /// <param name="canExecute">Optional can execute observable</param>
    /// <param name="context">Context for error handling</param>
    /// <returns>Error-safe ReactiveCommand</returns>
    public static ReactiveCommand<Unit, Unit> CreateSafeCommand(
        Func<Task> execute,
        IObservable<bool>? canExecute = null,
        string? context = null)
    {
        var command = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                await execute();
            }
            catch (Exception ex)
            {
                await Service_ErrorHandler.HandleErrorAsync(
                    ex,
                    context ?? "SafeCommand",
                    Environment.UserName,
                    new Dictionary<string, object>
                    {
                        ["Component"] = "SafeCommand",
                        ["Context"] = context ?? "Unknown"
                    });
                
                // Don't re-throw - just log and continue
            }
        }, canExecute);

        // Subscribe to ThrownExceptions to prevent unhandled errors
        command.ThrownExceptions.Subscribe(ex =>
        {
            Task.Run(async () =>
            {
                await Service_ErrorHandler.HandleErrorAsync(
                    ex,
                    $"{context ?? "SafeCommand"}_ThrownException",
                    Environment.UserName);
            });
        });

        return command;
    }

    /// <summary>
    /// Extension method to make any ReactiveCommand error-safe
    /// </summary>
    /// <typeparam name="TParam">Command parameter type</typeparam>
    /// <typeparam name="TResult">Command result type</typeparam>
    /// <param name="command">The ReactiveCommand to make safe</param>
    /// <param name="context">Context for error handling</param>
    /// <returns>The same command with error handling attached</returns>
    public static ReactiveCommand<TParam, TResult> MakeSafe<TParam, TResult>(
        this ReactiveCommand<TParam, TResult> command,
        string? context = null)
    {
        command.ThrownExceptions.Subscribe(ex =>
        {
            Task.Run(async () =>
            {
                await Service_ErrorHandler.HandleErrorAsync(
                    ex,
                    context ?? "ReactiveCommand",
                    Environment.UserName,
                    new Dictionary<string, object>
                    {
                        ["Component"] = "ReactiveCommand",
                        ["Context"] = context ?? "Unknown"
                    });
            });
        });

        return command;
    }

    /// <summary>
    /// Extension method to make ObservableAsPropertyHelper error-safe
    /// </summary>
    /// <typeparam name="T">Property type</typeparam>
    /// <param name="source">Source observable</param>
    /// <param name="target">Target object</param>
    /// <param name="property">Property expression</param>
    /// <param name="initialValue">Initial value</param>
    /// <param name="scheduler">Scheduler</param>
    /// <param name="context">Context for error handling</param>
    /// <returns>Error-safe ObservableAsPropertyHelper</returns>
    public static ObservableAsPropertyHelper<T> ToSafeProperty<TObj, T>(
        this IObservable<T> source,
        TObj target,
        System.Linq.Expressions.Expression<Func<TObj, T>> property,
        T initialValue = default(T)!,
        IScheduler? scheduler = null,
        string? context = null) where TObj : ReactiveObject
    {
        var safeSource = source.MakeSafe(initialValue, context);
        return safeSource.ToProperty(target, property, initialValue, scheduler: scheduler);
    }
}

/// <summary>
/// Static class to ensure ReactiveUI error handler is initialized early
/// </summary>
public static class ReactiveUIInitializer
{
    private static bool _initialized = false;

    /// <summary>
    /// Ensures ReactiveUI error handling is properly initialized
    /// Call this early in application startup
    /// </summary>
    public static void EnsureInitialized(ILogger? logger = null)
    {
        if (_initialized) return;

        try
        {
            ReactiveUIErrorHandler.Initialize(logger);
            _initialized = true;
            
            logger?.LogInformation("ReactiveUI error handling initialized successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize ReactiveUI error handling: {ex.Message}");
        }
    }
}