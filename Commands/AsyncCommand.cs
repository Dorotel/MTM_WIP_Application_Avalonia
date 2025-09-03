// Note: This file is maintained for legacy compatibility.
// New ViewModels should use CommunityToolkit.Mvvm.Input.AsyncRelayCommand directly.
// 
// Usage in new ViewModels:
// [RelayCommand]
// private async Task ExecuteSomeActionAsync() { ... }
// 
// or manually:
// public IAsyncRelayCommand SomeCommand => new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(ExecuteSomeActionAsync);

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTM_WIP_Application_Avalonia.Commands;

/// <summary>
/// Legacy async command implementation for backward compatibility.
/// New code should use CommunityToolkit.Mvvm.Input.AsyncRelayCommand with [RelayCommand] attributes.
/// </summary>
[Obsolete("Use CommunityToolkit.Mvvm.Input.AsyncRelayCommand with [RelayCommand] attribute instead")]
public class AsyncCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await _execute();
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Legacy generic async command implementation for backward compatibility.
/// New code should use CommunityToolkit.Mvvm.Input.AsyncRelayCommand<T> with [RelayCommand] attribute instead.
/// </summary>
[Obsolete("Use CommunityToolkit.Mvvm.Input.AsyncRelayCommand<T> with [RelayCommand] attribute instead")]
public class AsyncCommand<T> : ICommand
{
    private readonly Func<T?, Task> _execute;
    private readonly Func<T?, bool>? _canExecute;
    private bool _isExecuting;

    public AsyncCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        if (_isExecuting) return false;
        
        if (parameter is T typedParameter)
            return _canExecute?.Invoke(typedParameter) ?? true;
        
        if (parameter == null && typeof(T).IsValueType)
            return _canExecute?.Invoke(default(T)) ?? true;
            
        return _canExecute?.Invoke((T?)parameter) ?? true;
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        _isExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            T? typedParameter = parameter is T ? (T)parameter : default(T);
            await _execute(typedParameter);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}