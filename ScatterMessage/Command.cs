﻿using System;
using System.Windows.Input;

namespace ScatterMessage
{
    public class Command : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            return (CanExecuteDelegate != null) ? CanExecuteDelegate(parameter) : true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null) ExecuteDelegate(parameter);
        }
    }
}
