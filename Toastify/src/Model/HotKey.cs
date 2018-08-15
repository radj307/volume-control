using JetBrains.Annotations;
using log4net;
using Newtonsoft.Json;
using StructureMap.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ToastifyAPI.Interop.Interfaces;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut), JsonConverter(typeof(HotkeyJsonConverter))]
    public abstract class Hotkey : Actionable, IHotkey, INotifyPropertyChanged, IEquatable<IHotkey>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Hotkey));

        protected bool _active;
        private ModifierKeys _modifiers = ModifierKeys.None;

        #region Public properties

        [JsonIgnore]
        [SetterProperty]
        public IInputDevices InputDevices { get; set; }

        /// <summary>
        /// Specifies whether or not the hotkey is enabled from a user's perspective.
        /// Does not actually register the hotkey, use Activate() and Deactivate().
        /// </summary>
        /// <remarks>
        /// Why do we have these two properties? We need a way to be able to deactivate a
        /// Hotkey (for example when unloading settings) without changing the Enabled
        /// property (which only indicates the user's preference)
        /// </remarks>
        public bool Enabled { get; set; }

        [JsonIgnore]
        public virtual bool Active
        {
            get
            {
                return this._active;
            }
            protected set
            {
                if (this._active != value)
                {
                    this._active = value;

                    if (this._active)
                        this.Init();
                    else
                        this.Dispose();
                }
            }
        }

        public virtual ModifierKeys Modifiers
        {
            get { return this._modifiers; }
            set { this._modifiers = value; }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public override bool CanPerformAction
        {
            get
            {
                if (!this.Enabled || !this.Active || !this.IsValid())
                    return false;
                return base.CanPerformAction;
            }
        }

        [JsonIgnore]
        public abstract string HumanReadableKey { get; }

        [JsonIgnore]
        public virtual string HumanReadableAction
        {
            get { return this.Action?.Name; }
        }

        [JsonIgnore]
        public virtual string InvalidReason { get; protected set; }

        #endregion Public properties

        /// <inheritdoc />
        protected Hotkey()
        {
        }

        protected Hotkey(IAction action) : base(action)
        {
        }

        protected Hotkey([NotNull] IHotkey hotkey)
        {
            if (hotkey == null)
                throw new ArgumentNullException(nameof(hotkey));

            this.InputDevices = hotkey.InputDevices;
            this.Action = hotkey.Action;
            this.Enabled = hotkey.Enabled;
            this._active = false;
            this._modifiers = hotkey.Modifiers;
        }

        protected void Init()
        {
            if (this.Active && this.Enabled && this.IsValid())
                this.InitInternal();
        }

        protected abstract void InitInternal();

        public abstract bool IsValid();

        public bool AreModifiersPressed()
        {
            return this.InputDevices?.ArePressed(this.Modifiers) ?? throw new InvalidOperationException("InputDevices is null");
        }

        public abstract IHotkeyVisitor GetVisitor();

        /// <summary>
        /// Turn this hotkey on. Does nothing if this Hotkey is not enabled.
        /// </summary>
        public void Activate()
        {
            this.Active = true;
        }

        /// <summary>
        /// Turn this HotKey off.
        /// </summary>
        public void Deactivate()
        {
            this.Active = false;
        }

        public void Dispatch(IHotkeyVisitor hotkeyVisitor)
        {
            // TODO: TEMPORARY FIX: Never show the toast on demand!
            if (this.Action is ToastifyShowToast)
                return;

            this.DispatchInternal(hotkeyVisitor);
        }

        protected abstract void DispatchInternal(IHotkeyVisitor hotkeyVisitor);

        public object Clone()
        {
            Hotkey clone = (Hotkey)this.MemberwiseClone();

            // Regardless of whether or not the original hotkey was active,
            // the cloned one should not start in an active state.
            clone._active = false;

            return clone;
        }

        #region Equals / GetHashCode

        /// <inheritdoc />
        public bool Equals(IHotkey other)
        {
            if (other is null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return this.Modifiers == other.Modifiers;
        }

        protected bool Equals(Hotkey other)
        {
            return base.Equals(other) &&
                   this.Equals((IHotkey)other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == this.GetType() &&
                   this.Equals((Hotkey)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (int)this.Modifiers;
            }
        }

        #endregion Equals / GetHashCode

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            this.DisposeInternal();

            if (this._active)
                this.Active = false;
        }

        protected abstract void DisposeInternal();

        #endregion Dispose

        public override string ToString()
        {
            return $"{this.HumanReadableKey} -> {this.HumanReadableAction}";
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}