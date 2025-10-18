using System.Windows.Input;

namespace SekiroTool.Core;

internal class DelegateCommand : ICommand
{
    private readonly Action<object>? _executeWithParam;
    private readonly Action? _executeWithoutParam;
    private readonly Predicate<object>? _canExecute;

    public DelegateCommand(Action<object> execute, Predicate<object>? canExecute = null)
    {
        _executeWithParam = execute;
        _canExecute = canExecute;
    }

    public DelegateCommand(Action execute, Predicate<object>? canExecute = null)
    {
        _executeWithoutParam = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChange() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter ?? new()) ?? true;

    public void Execute(object? parameter)
    {
        _executeWithParam?.Invoke(parameter ?? new());
        _executeWithoutParam?.Invoke();
    }
}



