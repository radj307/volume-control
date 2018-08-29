using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;
using JetBrains.Annotations;
using log4net;
using ManagedWinapi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Toastify.DI;
using ToastifyAPI.Helpers;
using ToastifyAPI.Logic.Interfaces;
using ToastifyAPI.Model;
using ToastifyAPI.Model.Interfaces;

namespace Toastify.Model
{
    /// <summary>
    ///     A hotkey based on a keyboard global hotkey.
    /// </summary>
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class KeyboardHotkey : Hotkey, IKeyboardHotkey
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(KeyboardHotkey));

        private IGlobalHotkey globalHotkey;

        private Key? _key;
        private bool? isValid;

        #region Public Properties

        [PropertyDependency]
        public IKeyboardHotkeyVisitor HotkeyVisitor { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Key? Key
        {
            get { return this._key; }
            set
            {
                if (this._key != value)
                {
                    this._key = value;
                    this.CheckIfValid();
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public override ModifierKeys Modifiers
        {
            get { return base.Modifiers; }
            set
            {
                if (base.Modifiers != value)
                {
                    base.Modifiers = value;
                    this.CheckIfValid();
                    this.OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        public override string HumanReadableKey
        {
            get
            {
                string alt = this.Modifiers.HasFlag(ModifierKeys.Alt) ? "Alt+" : string.Empty;
                string ctlr = this.Modifiers.HasFlag(ModifierKeys.Control) ? "Ctrl+" : string.Empty;
                string shift = this.Modifiers.HasFlag(ModifierKeys.Shift) ? "Shift+" : string.Empty;
                string win = this.Modifiers.HasFlag(ModifierKeys.Windows) ? "Win+" : string.Empty;
                return $"{alt}{ctlr}{shift}{win}{this.Key.ToString()}";
            }
        }

        public override string InvalidReason
        {
            get { return base.InvalidReason; }
            protected set
            {
                if (base.InvalidReason != value)
                {
                    base.InvalidReason = value;
                    base.OnPropertyChanged();
                }
            }
        }

        public ILockedGlobalHotkey GlobalHotkey
        {
            get { return this.globalHotkey != null ? new LockedGlobalHotkey(this.globalHotkey) : null; }
        }

        #endregion

        /// <inheritdoc />
        public KeyboardHotkey()
        {
        }

        /// <inheritdoc />
        public KeyboardHotkey(IAction action) : base(action)
        {
        }

        /// <inheritdoc />
        public KeyboardHotkey([NotNull] IHotkey hotkey) : base(hotkey)
        {
        }

        public KeyboardHotkey([NotNull] KeyboardHotkey hotkey) : base(hotkey)
        {
            this._key = hotkey._key;
            this.isValid = hotkey.isValid;

            this.HotkeyVisitor = hotkey.HotkeyVisitor;
        }

        public KeyboardHotkey([NotNull] IGlobalHotkey globalHotkey)
        {
            this.globalHotkey = globalHotkey ?? throw new ArgumentNullException(nameof(globalHotkey));
        }

        /// <inheritdoc />
        protected override void InitInternal()
        {
            // InitInternal is executed only if (this.Enabled && this.Active && this.IsValid())

            if (this.globalHotkey != null)
                this.globalHotkey.HotkeyPressed -= this.GlobalHotkey_HotkeyPressed;

            if (this.globalHotkey == null)
                this.globalHotkey = new GlobalHotkey();

            // Make sure that we don't try to re-register the key midway updating the combination.
            if (this.globalHotkey.Enabled)
                this.globalHotkey.Enabled = false;

            // ReSharper disable once PossibleInvalidOperationException
            this.globalHotkey.KeyCode = this.Key.Value.ConvertToWindowsFormsKeys();
            this.globalHotkey.Alt = this.Modifiers.HasFlag(ModifierKeys.Alt);
            this.globalHotkey.Ctrl = this.Modifiers.HasFlag(ModifierKeys.Control);
            this.globalHotkey.Shift = this.Modifiers.HasFlag(ModifierKeys.Shift);
            this.globalHotkey.WindowsKey = this.Modifiers.HasFlag(ModifierKeys.Windows);

            this.globalHotkey.HotkeyPressed += this.GlobalHotkey_HotkeyPressed;

            try
            {
                this.globalHotkey.Enabled = true;
            }
            catch (HotkeyAlreadyInUseException)
            {
                this.isValid = false;
                this.InvalidReason = "Hotkey is already in use by a different program";
            }
        }

        /// <inheritdoc />
        public override bool IsValid()
        {
            this.CheckIfValid();
            return this.isValid ?? true;
        }

        internal override void SetIsValid(bool isValid, string invalidReason)
        {
            this.isValid = isValid;
            this.InvalidReason = isValid ? string.Empty : invalidReason;
        }

        protected virtual void CheckIfValid()
        {
            // If the hotkey has been invalidated elsewhere, ignore the following basic checks
            if (this.isValid == false)
                return;

            if (!this.Key.HasValue || this.Key == System.Windows.Input.Key.None || this.Key.Value.ConvertToWindowsFormsKeys() == Keys.None)
            {
                this.isValid = false;
                this.InvalidReason = "You must select a valid key for your hotkey combination";
            }
            else
            {
                this.isValid = true;
                this.InvalidReason = string.Empty;
            }
        }

        /// <inheritdoc />
        public override IHotkeyVisitor GetVisitor()
        {
            return this.HotkeyVisitor;
        }

        /// <inheritdoc />
        protected override void DispatchInternal(IHotkeyVisitor hotkeyVisitor)
        {
            if (hotkeyVisitor is IKeyboardHotkeyVisitor keyboardHotkeyVisitor)
                keyboardHotkeyVisitor.Visit(this);
        }

        /// <inheritdoc />
        protected override void DisposeInternal()
        {
            if (this.globalHotkey != null)
            {
                this.globalHotkey.HotkeyPressed -= this.GlobalHotkey_HotkeyPressed;

                this.globalHotkey.Enabled = false;
                this.globalHotkey = null;

                this._active = false;
            }
        }

        private void GlobalHotkey_HotkeyPressed(object sender, EventArgs e)
        {
            if (this.HotkeyVisitor != null)
                this.Dispatch(this.HotkeyVisitor);
            else
                logger.Warn($"{nameof(this.HotkeyVisitor)} is null!");
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            this.isValid = null;
        }
    }
}