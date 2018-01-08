using System;
using System.Windows.Input;

namespace Toastify.Common
{
    public class DelegateCommand : ICommand
    {
        private readonly Action action;

        private readonly Func<bool> canExecuteMethod;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action action) : this(action, null)
        {
        }

        public DelegateCommand(Action action, Func<bool> canExecuteMethod)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns> true if this command can be executed; otherwise, false. </returns>
        public bool CanExecute()
        {
            return this.CanExecute(null);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        /// <returns> true if this command can be executed; otherwise, false. </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecuteMethod == null || this.canExecuteMethod();
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        public void Execute()
        {
            this.Execute(null);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            this.action();
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> action;

        private readonly Func<T, bool> canExecuteMethod;

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> action) : this(action, null)
        {
        }

        public DelegateCommand(Action<T> action, Func<T, bool> canExecuteMethod)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        /// <typeparam name="T"></typeparam>
        /// <returns> true if this command can be executed; otherwise, false. </returns>
        public bool CanExecute(T parameter)
        {
            return this.canExecuteMethod == null || this.canExecuteMethod(parameter);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns> true if this command can be executed; otherwise, false. </returns>
        public bool CanExecute()
        {
            return this.CanExecute(null);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        /// <returns> true if this command can be executed; otherwise, false. </returns>
        public bool CanExecute(object parameter)
        {
            T p = (parameter?.GetType().IsAssignableFrom(typeof(T)) ?? false) ? (T)parameter : default(T);
            return this.canExecuteMethod == null || this.canExecuteMethod(p);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        /// <typeparam name="T"></typeparam>
        public void Execute(T parameter)
        {
            this.action(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        public void Execute()
        {
            this.Execute(null);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter"> Data used by the command. If the command does not require data to be passed, this object can be set to null. </param>
        public void Execute(object parameter)
        {
            T p = (parameter?.GetType().IsAssignableFrom(typeof(T)) ?? false) ? (T)parameter : default(T);
            this.action(p);
        }
    }
}