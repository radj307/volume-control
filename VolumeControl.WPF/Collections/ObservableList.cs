using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VolumeControl.WPF.Collections
{
    /// <inheritdoc cref="ObservableCollection{T}"/>
    public class ObservableList<T> : ObservableCollection<T>
    {
        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);
            base.OnPropertyChanged(new("Count"));
            base.OnPropertyChanged(new("Items[]"));
        }
        /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range) Add(item);
        }
        /// <inheritdoc cref="List{T}.RemoveAll(Predicate{T})"/>
        public void RemoveAll(Predicate<T> predicate)
        {
            for (int i = this.Count - 1; i >= 0; --i)
            {
                if (predicate(this[i]))
                    this.RemoveAt(i);
            }
        }
    }
}
