using System;
using System.Windows.Input;

namespace Ae.LineCalibrator.Interface
{
    public sealed class SimpleCommand : ICommand
    {
        private readonly Action ExecuteAction;

        public SimpleCommand(Action executeAction) => ExecuteAction = executeAction;

#pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => ExecuteAction();
    }
}
