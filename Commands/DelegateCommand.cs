using System;
using System.Windows.Input;

namespace MTM_WIP_Application_Avalonia.Commands;

/// <summary>
/// Delegate command implementation for parameterized operations.
/// Standard .NET ICommand implementation without ReactiveUI dependencies.
/// </summary>
public class DelegateCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public DelegateCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (parameter is T typedParameter)
            return _canExecute?.Invoke(typedParameter) ?? true;
        if (parameter == null && !typeof(T).IsValueType)
            return _canExecute?.Invoke(default(T)) ?? true;
        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParameter)
            _execute(typedParameter);
        else if (parameter == null && !typeof(T).IsValueType)
            _execute(default(T));
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Non-generic delegate command for object parameters.
/// </summary>
public class DelegateCommand : DelegateCommand<object>
{
    public DelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        : base(execute, canExecute)
    {
    }
}