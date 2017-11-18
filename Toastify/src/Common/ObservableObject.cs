using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Toastify.Common
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the new value of a field if it has changed and notifies the change.
        /// </summary>
        /// <typeparam name="T"> The type of the field. </typeparam>
        /// <param name="field"> A reference to the field. </param>
        /// <param name="newValue"> The new value. </param>
        /// <param name="propertyName"> The name of the property; automatically provided through the CallerMemberName attribute. </param>
        protected void RaiseAndSetIfChanged<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                this.NotifyPropertyChanged(propertyName);
            }
        }

        protected void RaiseAndSetIfChangedWithConstraint<T>(ref T field, T newValue, Expression<Func<bool>> constraint, [CallerMemberName] string propertyName = null)
        {
            if (!constraint.Compile()())
                throw new ConstraintFailedException(constraint);

            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                this.NotifyPropertyChanged(propertyName);
            }
        }

        protected void NotifyPropertyChanged(string propertyName = null, [CallerMemberName] string callerMemberName = null)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? callerMemberName));
        }
    }
}