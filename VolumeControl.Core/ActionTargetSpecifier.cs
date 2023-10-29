using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.WPF.Collections;

namespace VolumeControl.Core
{
    /// <summary>
    /// Specifies the target(s) of a hotkey action.
    /// </summary>
    [JsonArray]
    public class ActionTargetSpecifier : INotifyPropertyChanged, ICollection<string>
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ActionTargetSpecifier"/> instance.
        /// </summary>
        public ActionTargetSpecifier()
        {
            Targets.CollectionChanged += this.Targets_CollectionChanged;
        }
        #endregion Constructor

        #region Fields
        private bool _lastHasItemsState = false;
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the list of targets.
        /// </summary>
        public ObservableImmutableList<string> Targets { get; } = new();
        /// <summary>
        /// Gets whether there are any targets or not.
        /// </summary>
        public bool HasTargets => Targets.Count > 0;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region ICollection Implementation

        #region Properties
        /// <inheritdoc/>
        public int Count => ((ICollection<string>)this.Targets).Count;
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<string>)this.Targets).IsReadOnly;
        #endregion Properties

        #region Methods
        /// <inheritdoc/>
        public void Add(string item) => ((ICollection<string>)this.Targets).Add(item);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<string>)this.Targets).Clear();
        /// <inheritdoc/>
        public bool Contains(string item) => ((ICollection<string>)this.Targets).Contains(item);
        /// <inheritdoc/>
        public void CopyTo(string[] array, int arrayIndex) => ((ICollection<string>)this.Targets).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)this.Targets).GetEnumerator();
        /// <inheritdoc/>
        public bool Remove(string item) => ((ICollection<string>)this.Targets).Remove(item);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.Targets).GetEnumerator();
        #endregion Methods

        #endregion ICollection Implementation

        #region EventHandlers

        #region Targets
        private void Targets_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (!_lastHasItemsState)
                {
                    NotifyPropertyChanged(nameof(HasTargets));
                    _lastHasItemsState = true;
                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                if (Targets.Count == 0)
                {
                    _lastHasItemsState = false;
                    NotifyPropertyChanged(nameof(HasTargets));
                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                _lastHasItemsState = HasTargets;
                NotifyPropertyChanged(nameof(HasTargets));
                break;
            }
        }
        #endregion Targets

        #endregion EventHandlers
    }
}
