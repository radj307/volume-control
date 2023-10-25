using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    /// <summary>
    /// Viewmodel for displaying lists of enums with the <see cref="FlagsAttribute"/>.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    public class FlagsEnumVM<T> : INotifyPropertyChanged where T : struct, Enum
    {
        #region FlagsEnumValueVM
        public class FlagsEnumValueVM : INotifyPropertyChanged
        {
            #region Constructor
            public FlagsEnumValueVM(T value, bool isSet)
            {
                IsSet = isSet;
                Value = value;
                Name = Enum.GetName<T>(value)!;
            }
            #endregion Constructor

            #region Properties
            public T Value { get; }
            public string Name { get; }
            public bool IsSet
            {
                get => _isSet;
                set
                {
                    _isSet = value;
                    NotifyPropertyChanged();
                }
            }
            private bool _isSet;
            #endregion Properties

            #region Events
            public event PropertyChangedEventHandler? PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
            #endregion Events
        }
        #endregion FlagsEnumValueVM

        #region Constructor
        public FlagsEnumVM(T selected, T? excluded, params string[] excludedNames)
        {
            _items = new();
            foreach (var value in Enum.GetValues<T>())
            {
                if ((excluded.HasValue && excluded.Value.HasFlag(value)) || excludedNames.Contains(Enum.GetName(value)))
                    continue;
                var item = new FlagsEnumValueVM(value, selected.HasFlag(value));
                item.PropertyChanged += this.Item_PropertyChanged;
                _items.Add(item);
            }
            _state = _items.Select(item => item.Value).GetSingleValue();
        }
        #endregion Constructor

        #region Properties
        /// <summary>
        /// Gets the list of <see cref="FlagsEnumValueVM"/> instances representing the flag values.
        /// </summary>
        public IReadOnlyList<FlagsEnumValueVM> Items => _items;
        private readonly List<FlagsEnumValueVM> _items;
        /// <summary>
        /// Gets or sets the current state of all of the flags.
        /// </summary>
        public T State
        {
            get => _state;
            set
            {
                _state = value;
                Items.ForEach(item => item.IsSet = value.HasFlag(item.Value));
                NotifyStateChanged(State, State.Xor(value));
                NotifyPropertyChanged();
            }
        }
        private T _state;
        #endregion Properties

        #region Events
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        /// <summary>
        /// Occurs when the value of the State property has changed.
        /// </summary>
        /// <remarks>
        /// Occurs before the PropertyChanged event.
        /// </remarks>
        public event EventHandler<(T NewState, T ChangedFlags)>? StateChanged;
        private void NotifyStateChanged(T newState, T changedFlags) => StateChanged?.Invoke(this, (newState, changedFlags));
        #endregion Events

        #region EventHandlers

        #region Item
        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(FlagsEnumValueVM.IsSet), StringComparison.Ordinal))
            {
                var item = (FlagsEnumValueVM)sender!;
                _state = _state.SetFlagState(item.Value, item.IsSet);
                NotifyStateChanged(State, item.Value);
            }
        }
        #endregion Item

        #endregion EventHandlers
    }
}
