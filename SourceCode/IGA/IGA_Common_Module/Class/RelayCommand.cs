using System.Windows.Input;

namespace IGA_Common_Module.Class;

public class RelayCommand : ICommand
{
    private readonly Func<object?, bool> _canExecute;
    private readonly Action<object?> _execute;

    public bool CanExecute(object? __parameter)
    {
        return _canExecute.Invoke(__parameter);
    }

    public void Execute(object? __parameter)
    {
        if (CanExecute(__parameter))
        {
            _execute.Invoke(__parameter);
        }
        else
        {
            throw new InvalidOperationException($"Can not execute the command.");
        }
    }

    public static bool AlwaysExecutable(object? __parameter)
    {
        return true;
    }

    public static bool AlwaysNotExecutable(object? __parameter)
    {
        return false;
    }

    public RelayCommand(Action<object?> __execute, Func<object?, bool> __canExecute)
    {
        _execute = __execute;
        _canExecute = __canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
