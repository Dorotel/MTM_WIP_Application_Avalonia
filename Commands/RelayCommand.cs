using System;
using System.Windows.Input;

namespace MTM_WIP_Application_Avalonia.Commands;

/// <summary>
/// Simple relay command implementation for synchronous operations.
/// Standard .NET ICommand implementation without ReactiveUI dependencies.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Generic relay command implementation for synchronous operations with parameters.
/// Standard .NET ICommand implementation without ReactiveUI dependencies.
/// </summary>
public class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter is T typedParameter)
            return _canExecute?.Invoke(typedParameter) ?? true;
        if (parameter == null && typeof(T).IsClass)
            return _canExecute?.Invoke(default(T)) ?? true;
        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParameter)
            _execute(typedParameter);
        else if (parameter == null && typeof(T).IsClass)
            _execute(default(T));
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
