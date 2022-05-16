using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VolumeControl.Core.HelperTypes.Lists
{
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
        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range) Add(item);
        }
    }
}
