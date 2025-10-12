using System.Windows.Input;

namespace SekiroTool.Core;

internal class DelegateCommand(Action<object> execute, Predicate<object>? canExecute) : ICommand
{
    public DelegateCommand(Action<object> execute) : this(execute, null) { }

    public event EventHandler? CanExecuteChanged;

    public void RaiseCanExecuteChange() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    public bool CanExecute(object? paramter) => canExecute?.Invoke(paramter ?? new()) ?? true;

    public void Execute(object? parameter) => execute?.Invoke(parameter ?? new());
}
