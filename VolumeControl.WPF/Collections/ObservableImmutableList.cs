using PropertyChanged;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace VolumeControl.WPF.Collections
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [SuppressPropertyChangedWarnings]
    public abstract class ObservableCollectionObject : INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private

        private bool _lockObjWasTaken;
        private readonly object _lockObj;
        private int _lock; // 0=unlocked		1=locked

        #endregion Private

        #region Public Properties

        public LockTypeEnum LockType { get; }

        #endregion Public Properties

        #region Constructor

        protected ObservableCollectionObject(LockTypeEnum lockType)
        {
            this.LockType = lockType;
            _lockObj = new object();
        }

        #endregion Constructor

        #region SpinWait/PumpWait Methods

        // note : find time to put all these methods into a helper class instead of in a base class

        // returns a valid dispatcher if this is a UI thread (can be more than one UI thread so different dispatchers are possible); null if not a UI thread
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Dispatcher GetDispatcher() => Dispatcher.FromThread(Thread.CurrentThread);

        protected void WaitForCondition(Func<bool> condition)
        {
            Dispatcher? dispatcher = this.GetDispatcher();

            if (dispatcher == null)
            {
                switch (this.LockType)
                {
                case LockTypeEnum.SpinWait:
                    SpinWait.SpinUntil(condition); // spin baby... 
                    break;
                case LockTypeEnum.Lock:
                    bool isLockTaken = false;
                    Monitor.Enter(_lockObj, ref isLockTaken);
                    _lockObjWasTaken = isLockTaken;
                    break;
                }
                return;
            }

            _lockObjWasTaken = true;
            this.PumpWait_PumpUntil(dispatcher, condition);
        }

        protected void PumpWait_PumpUntil(Dispatcher dispatcher, Func<bool> condition)
        {
            var frame = new DispatcherFrame();
            BeginInvokePump(dispatcher, frame, condition);
            Dispatcher.PushFrame(frame);
        }

        private static void BeginInvokePump(Dispatcher dispatcher, DispatcherFrame frame, Func<bool> condition) => dispatcher.BeginInvoke
                (
                DispatcherPriority.DataBind,


                    () =>
                    {
                        frame.Continue = !condition();

                        if (frame.Continue)
                            BeginInvokePump(dispatcher, frame, condition);
                    }

                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DoEvents()
        {
            Dispatcher? dispatcher = this.GetDispatcher();
            if (dispatcher == null)
                return;

            var frame = new DispatcherFrame();
            _ = dispatcher.BeginInvoke(DispatcherPriority.DataBind, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool TryLock() => this.LockType switch
        {
            LockTypeEnum.SpinWait => Interlocked.CompareExchange(ref _lock, 1, 0) == 0,
            LockTypeEnum.Lock => Monitor.TryEnter(_lockObj),
            _ => false,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Lock()
        {
            switch (this.LockType)
            {
            case LockTypeEnum.SpinWait:
                this.WaitForCondition(() => Interlocked.CompareExchange(ref _lock, 1, 0) == 0);
                break;
            case LockTypeEnum.Lock:
                this.WaitForCondition(() => Monitor.TryEnter(_lockObj));
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Unlock()
        {
            switch (this.LockType)
            {
            case LockTypeEnum.SpinWait:
                _lock = 0;
                break;
            case LockTypeEnum.Lock:
                if (_lockObjWasTaken)
                    Monitor.Exit(_lockObj);
                _lockObjWasTaken = false;
                break;
            }
        }

        #endregion SpinWait/PumpWait Methods

        #region INotifyCollectionChanged

        public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler? notifyCollectionChangedEventHandler = CollectionChanged;

            if (notifyCollectionChangedEventHandler == null)
                return;

            foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
            {
                if (handler.Target is DispatcherObject dispatcherObject && !dispatcherObject.CheckAccess())
                    _ = dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
                else
                {
                    handler(this, args);
                }
            }
        }

        protected virtual void RaiseNotifyCollectionChanged() => this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        protected virtual void RaiseNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.RaisePropertyChanged("Count");
            this.RaisePropertyChanged("Item[]");
            this.OnCollectionChanged(args);
        }

        #endregion INotifyCollectionChanged

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion INotifyPropertyChanged

        #region Nested Types

        public enum LockTypeEnum
        {
            SpinWait,
            Lock
        }

        #endregion Nested Types
    }
    /// <inheritdoc cref="ObservableCollection{T}"/>
    public class ObservableList<T> : ObservableCollection<T>
    {
        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            this.CheckReentrancy();
            base.InsertItem(index, item);
            base.OnPropertyChanged(new("Count"));
            base.OnPropertyChanged(new("Items[]"));
        }
        /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
        public void AddRange(IEnumerable<T> range)
        {
            foreach (T item in range) this.Add(item);
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
    [SuppressPropertyChangedWarnings]
    public class ObservableImmutableList<T> : ObservableCollectionObject, IList, ICollection, IEnumerable, IList<T>, IImmutableList<T>, ICollection<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Private
        private ImmutableList<T> _items;
        #endregion Private

        #region Constructors
        public ObservableImmutableList() : this(Array.Empty<T>(), LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableList(IEnumerable<T> items) : this(items, LockTypeEnum.SpinWait)
        {
        }

        public ObservableImmutableList(LockTypeEnum lockType) : this(Array.Empty<T>(), lockType)
        {
        }

        public ObservableImmutableList(IEnumerable<T> items, LockTypeEnum lockType) : base(lockType)
        {
            this.SyncRoot = new object();
            _items = ImmutableList<T>.Empty.AddRange(items);
        }

        #endregion Constructors

        #region Thread-Safe Methods

        #region General

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation) => this.TryOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation) => this.DoOperation(operation, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

        #region Helpers

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            try
            {
                if (this.TryLock())
                {
                    ImmutableList<T>? oldList = _items;
                    ImmutableList<T>? newItems = operation(oldList);

                    if (newItems == null)
                        // user returned null which means he cancelled operation
                        return false;

                    _items = newItems;

                    if (args != null)
                        this.RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                this.Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            try
            {
                if (this.TryLock())
                {
                    ImmutableList<T>? oldList = _items;
                    KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs> kvp = operation(oldList);
                    ImmutableList<T>? newItems = kvp.Key;
                    NotifyCollectionChangedEventArgs? args = kvp.Value;

                    if (newItems == null)
                        // user returned null which means he cancelled operation
                        return false;

                    _items = newItems;

                    if (args != null)
                        this.RaiseNotifyCollectionChanged(args);
                    return true;
                }
            }
            finally
            {
                this.Unlock();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, ImmutableList<T>> operation, NotifyCollectionChangedEventArgs args)
        {
            bool result;

            try
            {
                this.Lock();
                ImmutableList<T>? oldItems = _items;
                ImmutableList<T>? newItems = operation(_items);

                if (newItems == null)
                    // user returned null which means he cancelled operation
                    return false;

                result = (_items = newItems) != oldItems;

                if (args != null)
                    this.RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                this.Unlock();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool DoOperation(Func<ImmutableList<T>, KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>> operation)
        {
            bool result;

            try
            {
                this.Lock();
                ImmutableList<T>? oldItems = _items;
                KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs> kvp = operation(_items);
                ImmutableList<T>? newItems = kvp.Key;
                NotifyCollectionChangedEventArgs? args = kvp.Value;

                if (newItems == null)
                    // user returned null which means he cancelled operation
                    return false;

                result = (_items = newItems) != oldItems;

                if (args != null)
                    this.RaiseNotifyCollectionChanged(args);
            }
            finally
            {
                this.Unlock();
            }

            return result;
        }

        #endregion Helpers

        #endregion General

        #region Specific

        public bool DoInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.DoOperation
                (
                currentItems =>
                    {
                        KeyValuePair<int, T> kvp = valueProvider(currentItems);
                        ImmutableList<T>? newItems = currentItems.Insert(kvp.Key, kvp.Value);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
                    }
                );

        public bool DoAdd(Func<ImmutableList<T>, T> valueProvider) => this.DoOperation
                (
                currentItems =>
                    {
                        T value;
                        ImmutableList<T>? newItems = _items.Add(value = valueProvider(currentItems));
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
                    }
                );

        public bool DoAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider) => this.DoOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );

        public bool DoRemove(Func<ImmutableList<T>, T> valueProvider) => this.DoRemoveAt
                (
                currentItems =>
                    currentItems.IndexOf(valueProvider(currentItems))
                );

        public bool DoRemoveAt(Func<ImmutableList<T>, int> valueProvider) => this.DoOperation
                (
                currentItems =>
                    {
                        int index = valueProvider(currentItems);
                        T? value = currentItems[index];
                        ImmutableList<T>? newItems = currentItems.RemoveAt(index);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                    }
                );

        public bool DoSetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.DoOperation
                (
                currentItems =>
                    {
                        KeyValuePair<int, T> kvp = valueProvider(currentItems);
                        T? newValue = kvp.Value;
                        int index = kvp.Key;
                        T? oldValue = currentItems[index];
                        ImmutableList<T>? newItems = currentItems.SetItem(kvp.Key, newValue);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
                    }
                );

        public bool TryInsert(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.TryOperation
                (
                currentItems =>
                    {
                        KeyValuePair<int, T> kvp = valueProvider(currentItems);
                        ImmutableList<T>? newItems = currentItems.Insert(kvp.Key, kvp.Value);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp.Value, kvp.Key));
                    }
                );

        public bool TryAdd(Func<ImmutableList<T>, T> valueProvider) => this.TryOperation
                (
                currentItems =>
                    {
                        T value;
                        ImmutableList<T>? newItems = _items.Add(value = valueProvider(currentItems));
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, currentItems.Count));
                    }
                );

        public bool TryAddRange(Func<ImmutableList<T>, IEnumerable<T>> valueProvider) => this.TryOperation
                (
                currentItems =>
                    currentItems.AddRange(valueProvider(currentItems))
                );

        public bool TryRemove(Func<ImmutableList<T>, T> valueProvider) => this.TryRemoveAt
                (
                currentItems =>
                    currentItems.IndexOf(valueProvider(currentItems))
                );

        public bool TryRemoveAt(Func<ImmutableList<T>, int> valueProvider) => this.TryOperation
                (
                currentItems =>
                    {
                        int index = valueProvider(currentItems);
                        T? value = currentItems[index];
                        ImmutableList<T>? newItems = currentItems.RemoveAt(index);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
                    }
                );

        public bool TrySetItem(Func<ImmutableList<T>, KeyValuePair<int, T>> valueProvider) => this.TryOperation
                (
                currentItems =>
                    {
                        KeyValuePair<int, T> kvp = valueProvider(currentItems);
                        T? newValue = kvp.Value;
                        int index = kvp.Key;
                        T? oldValue = currentItems[index];
                        ImmutableList<T>? newItems = currentItems.SetItem(kvp.Key, newValue);
                        return new KeyValuePair<ImmutableList<T>, NotifyCollectionChangedEventArgs>(newItems, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
                    }
                );

        #endregion Specific

        public ImmutableList<T> ToImmutableList() => _items;

        #region IEnumerable<T>

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion IEnumerable<T>

        #endregion Thread-Safe Methods

        #region Non Thead-Safe Methods

        #region IList

        /// <inheritdoc/>
        public int Add(object? value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            var val = (T)value;
            _ = this.Add(val);
            return this.IndexOf(val);
        }

        /// <inheritdoc/>
        public bool Contains(object? value) => value is null ? false : value is T val && this.Contains(val);

        /// <inheritdoc/>
        public int IndexOf(object? value) => value is null ? throw new ArgumentNullException(nameof(value)) : value is not T val ? -1 : this.IndexOf(val);

        /// <inheritdoc/>
        public void Insert(int index, object? value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value is not T val) throw new ArgumentException($"Unexpected value type {value.GetType().FullName}; expected {typeof(T).FullName}!", nameof(value));
            _ = this.Insert(index, val);
        }

        /// <inheritdoc/>
        public bool IsFixedSize => false;

        /// <inheritdoc/>
        public void Remove(object? value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (value is not T val) throw new ArgumentException($"Unexpected value type {value.GetType().FullName}; expected {typeof(T).FullName}!", nameof(value));
            _ = this.Remove(val);
        }

        /// <inheritdoc/>
        void IList.RemoveAt(int index) => this.RemoveAt(index);

        /// <inheritdoc/>
        object IList.this[int index]
        {
#           pragma warning disable CS8603 // Possible null reference return.
            get => this[index];
#           pragma warning restore CS8603 // Possible null reference return.
#           pragma warning disable CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
            set => this.SetItem(index, (T)value);
#           pragma warning restore CS8769 // Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
        }

        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => _items.ToArray().CopyTo(array, index);

        /// <inheritdoc/>
        public bool IsSynchronized => false;

        /// <inheritdoc/>
        public object SyncRoot { get; }

        #endregion IList

        #region IList<T>

        /// <inheritdoc/>
        public int IndexOf(T item) => _items.IndexOf(item);

        /// <inheritdoc/>
        void IList<T>.Insert(int index, T item) => this.Insert(index, item);

        /// <inheritdoc/>
        void IList<T>.RemoveAt(int index) => this.RemoveAt(index);

        /// <inheritdoc/>
        public T this[int index]
        {
            get => _items[index];
            set => this.SetItem(index, value);
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <inheritdoc/>
        void IList.Clear() => this.Clear();

        /// <inheritdoc/>
        void ICollection<T>.Clear() => this.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => _items.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public int Count => _items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            int index = _items.IndexOf(item);
            if (index == -1)
                return false;

            _items = _items.Remove(item);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            this.RaisePropertyChanged(nameof(Count));
            return true;
        }

        #endregion IList<T>

        #region IImmutableList<T>

        /// <inheritdoc/>
        public IImmutableList<T> Add(T value)
        {
            int index = _items.Count;
            _items = _items.Add(value);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> AddRange(IEnumerable<T> items)
        {
            _items = _items.AddRange(items);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> Clear()
        {
            _items = _items.Clear();
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => _items.IndexOf(item, index, count, equalityComparer);

        /// <inheritdoc/>
        public IImmutableList<T> Insert(int index, T element)
        {
            _items = _items.Insert(index, element);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> InsertRange(int index, IEnumerable<T> items)
        {
            _items = _items.InsertRange(index, items);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => _items.LastIndexOf(item, index, count, equalityComparer);

        /// <inheritdoc/>
        public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer)
        {
            int index = _items.IndexOf(value, equalityComparer);
            _ = this.RemoveAt(index);
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveAll(Predicate<T> match)
        {
            _items = _items.RemoveAll(match);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveAt(int index)
        {
            T? value = _items[index];
            _items = _items.RemoveAt(index);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveRange(int index, int count)
        {
            _items = _items.RemoveRange(index, count);
            this.RaiseNotifyCollectionChanged();
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer)
        {
            foreach (var item in items)
            {
                Remove(item);
            }
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer)
        {
            int index = _items.IndexOf(oldValue, equalityComparer);
            _ = this.SetItem(index, newValue);
            return this;
        }

        /// <inheritdoc/>
        public IImmutableList<T> SetItem(int index, T value)
        {
            T? oldItem = _items[index];
            _items = _items.SetItem(index, value);
            this.RaiseNotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItem, value, index));
            return this;
        }

        public void ForEach(Action<T> action)
        {
            for (int i = 0; i < _items.Count; ++i)
                action(_items[i]);
        }

        #endregion IImmutableList<T>

        /// <summary>
        /// Moves the item at the specified <paramref name="oldIndex"/> to a new location specified by <paramref name="newIndex"/>.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        /// <returns>This <see cref="ObservableImmutableList{T}"/> instance as a <see cref="IImmutableList{T}"/>.</returns>
        public IImmutableList<T> Move(int oldIndex, int newIndex)
        {
            T removedItem = this[oldIndex];

            // remove & insert the item without triggering CollectionChanged event:
            _items = _items
                .RemoveAt(oldIndex)
                .Insert(newIndex, removedItem);

            RaiseNotifyCollectionChanged(new(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex));
            return this;
        }

        #endregion Non Thead-Safe Methods
    }
    /// <summary>
    /// Extends the <see cref="ObservableImmutableList{T}"/> class with sorting methods.
    /// </summary>
    public static class ObservableImmutableListExtension_Sort
    {
        /// <summary>
        /// Sorts the list using the <see cref="IComparable.CompareTo(object?)"/> method.
        /// </summary>
        /// <typeparam name="T">Type of object contained by the list. It must implement <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="observableImmutableList">(implicit) The <see cref="ObservableImmutableList{T}"/> instance to sort.</param>
        public static void Sort<T>(this ObservableImmutableList<T> observableImmutableList) where T : IComparable
        {
            var sorted = observableImmutableList.OrderBy(x => x).ToList();
            for (int i = 0, max = sorted.Count; i < max; ++i)
            {
                observableImmutableList.Move(observableImmutableList.IndexOf(sorted[i]), i);
            }
        }
        /// <summary>
        /// Sorts the list using the specified <paramref name="comparer"/>.
        /// </summary>
        /// <typeparam name="T">Type of object contained by the list.</typeparam>
        /// <param name="observableImmutableList">(implicit) The <see cref="ObservableImmutableList{T}"/> instance to sort.</param>
        /// <param name="comparer">A comparer object to use when sorting the list.</param>
        public static void Sort<T>(this ObservableImmutableList<T> observableImmutableList, IComparer<T> comparer)
        {
            var sorted = observableImmutableList.OrderBy(x => x, comparer).ToList();
            for (int i = 0, max = sorted.Count; i < max; ++i)
            {
                observableImmutableList.Move(observableImmutableList.IndexOf(sorted[i]), i);
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
