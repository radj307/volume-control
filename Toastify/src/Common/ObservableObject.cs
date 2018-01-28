using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            this.RaiseAndSetIfChangedInternal(ref field, newValue, propertyName);
        }

        protected bool RaiseAndSetIfChanged<T>(ref T field, T newValue, T minValue, T maxValue, [CallerMemberName] string propertyName = null) where T : IComparable<T>
        {
            Comparer<T> comparer = Comparer<T>.Default;
            bool coerced = false;
            if (comparer.Compare(newValue, minValue) < 0)
            {
                newValue = minValue;
                coerced = true;
            }
            else if (comparer.Compare(newValue, maxValue) > 0)
            {
                newValue = maxValue;
                coerced = true;
            }

            this.RaiseAndSetIfChangedInternal(ref field, newValue, propertyName);
            return coerced;
        }

        protected void NotifyPropertyChanged(string propertyName = null, [CallerMemberName] string callerMemberName = null)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? callerMemberName));
        }

        private void RaiseAndSetIfChangedInternal<T>(ref T field, T newValue, string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                this.NotifyPropertyChanged(propertyName);
            }
        }
    }
}