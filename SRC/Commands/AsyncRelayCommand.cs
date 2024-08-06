using System.Threading.Tasks;
using System.Windows.Input;
using System;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;

    public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // Event to handle requerying of CanExecute state to allow dynamic command enabling/disabling
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    // Determines whether the command can execute based on the _canExecute delegate
    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    // Executes the asynchronous command to keep the UI responsive
    public async void Execute(object parameter)
    {
        await _execute?.Invoke();
    }

    // Provides a direct way to execute the command asynchronously
    public Task ExecuteAsync()
    {
        return _execute();
    }
}
