// Note: This file is maintained for legacy compatibility.
// New ViewModels should use CommunityToolkit.Mvvm.Input.RelayCommand directly.
// 
// Usage in new ViewModels:
// [RelayCommand]
// private async Task ExecuteSomeActionAsync() { ... }
// 
// or manually:
// public ICommand SomeCommand => new CommunityToolkit.Mvvm.Input.RelayCommand(ExecuteSomeAction);

using System;
using System.Windows.Input;

namespace MTM_WIP_Application_Avalonia.Commands;

/// <summary>
/// Legacy relay command implementation for backward compatibility.
/// New code should use CommunityToolkit.Mvvm.Input.RelayCommand with [RelayCommand] attributes.
/// </summary>
[Obsolete("Use CommunityToolkit.Mvvm.Input.RelayCommand with [RelayCommand] attribute instead")]
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

    public bool CanExecute(object? parameter)
    {
        var result = _canExecute?.Invoke() ?? true;
        System.Diagnostics.Debug.WriteLine($"[RelayCommand] CanExecute called. Result: {result}");
        return result;
    }

    public void Execute(object? parameter) => _execute();

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Legacy generic relay command implementation for backward compatibility.
/// New code should use CommunityToolkit.Mvvm.Input.RelayCommand<T> with [RelayCommand] attribute instead.
/// </summary>
[Obsolete("Use CommunityToolkit.Mvvm.Input.RelayCommand<T> with [RelayCommand] attribute instead")]
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
