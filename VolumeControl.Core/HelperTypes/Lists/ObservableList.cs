using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VolumeControl.Core.HelperTypes.Lists
{
    public class ObservableList<T> : ObservableCollection<T>
    {
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);
            base.OnPropertyChanged(new("Count"));
            base.OnPropertyChanged(new("Items[]"));
        }
        public void AddRange(IEnumerable<T> range, NotifyCollectionChangedAction notify = NotifyCollectionChangedAction.Add)
        {
            if (range is null)
                return;
            CheckReentrancy();
            if (notify == NotifyCollectionChangedAction.Reset)
            {
                foreach (T item in range)
                    Items.Add(item);
                OnPropertyChanged(new("Count"));
                OnPropertyChanged(new("Item[]"));
                OnCollectionChanged(new(notify));
            }
            else
            {
                int index = Count;
                var changedItems = range is List<T> list ? list : new List<T>(range);

                for (int i = 0; i < changedItems.Count; ++i)
                {
                    Items.Add(changedItems[i]);
                }

                OnPropertyChanged(new("Count"));
                OnPropertyChanged(new("Item[]"));
                OnCollectionChanged(new(notify, changedItems, index));
            }
        }
    }
}
