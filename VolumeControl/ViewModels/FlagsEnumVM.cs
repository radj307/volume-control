using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VolumeControl.TypeExtensions;

namespace VolumeControl.ViewModels
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public abstract class FlagsEnumValueBase<T> : INotifyPropertyChanged where T : struct, Enum
    {
        #region Constructor
        protected FlagsEnumValueBase(T value, bool isSet)
        {
            IsSet = isSet;
            Value = value;
        }
        #endregion Constructor

        #region Properties
        public T Value { get; internal set; }
        public abstract string Name { get; }
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
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events
    }
    /// <summary>
    /// Viewmodel for displaying lists of enums with the <see cref="FlagsAttribute"/>.
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <typeparam name="TValueVM">The flag value viewmodel type to use. It must have a constructor with the same signature as that of <see cref="FlagsEnumValueBase{T}"/>.</typeparam>
    public class FlagsEnumVM<T, TValueVM> : INotifyPropertyChanged where T : struct, Enum where TValueVM : FlagsEnumValueBase<T>
    {
        #region Constructor
        public FlagsEnumVM(T selected, T? excluded, params string[] excludedNames)
        {
            _items = new();
            foreach (var value in Enum.GetValues<T>())
            {
                if (excluded.HasValue && excluded.Value.HasFlag(value) || excludedNames.Contains(Enum.GetName(value)))
                    continue;
                var item = (TValueVM)Activator.CreateInstance(typeof(TValueVM), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new object[] { value, selected.HasFlag(value) }, System.Globalization.CultureInfo.CurrentCulture)!;
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
        public IReadOnlyList<FlagsEnumValueBase<T>> Items => _items;
        private readonly List<FlagsEnumValueBase<T>> _items;
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

            if (e.PropertyName.Equals(nameof(FlagsEnumValueBase<T>.IsSet), StringComparison.Ordinal))
            {
                var item = (FlagsEnumValueBase<T>)sender!;
                _state = _state.SetFlagState(item.Value, item.IsSet);
                NotifyStateChanged(State, item.Value);
            }
        }
        #endregion Item

        #endregion EventHandlers
    }
    /// <inheritdoc cref="FlagsEnumVM{T, TValueVM}"/>
    public class FlagsEnumVM<T> : FlagsEnumVM<T, FlagsEnumVM<T>.FlagsEnumValueVM> where T : struct, Enum
    {
        #region Constructor
        public FlagsEnumVM(T selected, T? excluded, params string[] excludedNames) : base(selected, excluded, excludedNames) { }
        #endregion Constructor

        #region (class) FlagsEnumValueVM
        public class FlagsEnumValueVM : FlagsEnumValueBase<T>
        {
            #region Constructor
            internal FlagsEnumValueVM(T value, bool isSet) : base(value, isSet)
            {
                Name = Enum.GetName(value)!;
            }
            #endregion Constructor

            #region Properties
            public override string Name { get; }
            #endregion Properties
        }
        #endregion (class) FlagsEnumValueVM
    }
}
