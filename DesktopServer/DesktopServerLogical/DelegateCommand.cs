using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopServerLogical
{
    public class DelegateCommand : ICommand
    {
        private Action _action;
        private Action<object> _actionWithParameter;
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action)
        {
            _action = action;
        }
        public DelegateCommand(Action<object> action)
        {
            _actionWithParameter= action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (_actionWithParameter != null)
                _actionWithParameter(parameter);
            else
                _action();
        }
    }
}
