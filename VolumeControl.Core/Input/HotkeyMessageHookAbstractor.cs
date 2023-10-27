using System.Windows.Interop;
using VolumeControl.Core.Input.Enums;
using VolumeControl.TypeExtensions;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Maintains a single message hook for multiple hotkeys with the same key combination to allow them all to be "registered" at the same time.
    /// </summary>
    /// <remarks>
    /// Hotkeys are automatically removed when they are unregistered (which occurs during re-registration).
    /// </remarks>
    public class HotkeyMessageHookAbstractor : IHotkeyMessageHook
    {
        #region Constructor
        /// <summary>
        /// Creates a new <see cref="HotkeyMessageHookAbstractor"/> instance.
        /// </summary>
        public HotkeyMessageHookAbstractor()
        {
            WindowsHotkeyAPI.AddHook(this);
        }
        #endregion Constructor

        #region Fields
        private readonly List<ushort> _ids = new();
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets the list of hotkeys currently using this message hook abstractor.
        /// </summary>
        public IReadOnlyList<IHotkey> Hotkeys => _hotkeys;
        private readonly List<IHotkey> _hotkeys = new();
        /// <inheritdoc/>
        public HwndSourceHook MessageHook => MessageHookImpl;
        /// <summary>
        /// Gets whether this instance has started managing a specific key combination yet.
        /// </summary>
        public bool HasStarted { get; private set; } = false;
        /// <summary>
        /// Gets the key in the key combination that this instance manages.
        /// </summary>
        public EFriendlyKey ManagedKey { get; private set; } = EFriendlyKey.None;
        /// <summary>
        /// Gets the modifier keys in the key combination that this instance manages.
        /// </summary>
        public EModifierKey ManagedModifiers { get; private set; } = EModifierKey.None;
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when this <see cref="HotkeyMessageHookAbstractor"/> instance is no longer managing any hotkey message hooks.
        /// </summary>
        public event EventHandler? AllHotkeysRemoved;
        private void NotifyAllHotkeysRemoved() => AllHotkeysRemoved?.Invoke(this, EventArgs.Empty);
        /// <summary>
        /// Occurs when there is only one hotkey remaining in this hook abstractor instance.
        /// </summary>
        public event EventHandler<IHotkey>? OneHotkeyRemaining;
        private void NotifyOneHotkeyRemaining(IHotkey lastHotkeyInst) => OneHotkeyRemaining?.Invoke(this, lastHotkeyInst);
        #endregion Events

        #region Methods

        #region (Private)
        private static ushort GetIDFromWParam(IntPtr intPtr) => Convert.ToUInt16(intPtr.ToInt32());
        #endregion (Private)

        #region MessageHookImpl
        IntPtr MessageHookImpl(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
            case WindowsHotkeyAPI.WM_HOTKEY:
                var id = GetIDFromWParam(wParam);
                if (_ids.Contains(id))
                {
                    for (int i = 0, i_max = Hotkeys.Count; i < i_max; ++i)
                    {
                        var hotkey = Hotkeys[i];
                        if (hotkey.Action == null) continue;

                        var eventArgs = new HotkeyPressedEventArgs(hotkey.Action.ActionSettings);
                        hotkey.Action.Invoke(hotkey, eventArgs);
                        hotkey.RaiseEvent(nameof(IHotkey.Pressed), eventArgs);
                    }

                    handled = true;
                }
                break;
            default:
                break;
            }
            return IntPtr.Zero;
        }
        #endregion MessageHookImpl

        #region HasManagedKeyCombination
        /// <summary>
        /// Checks if the specified <paramref name="hotkey"/> instance has the same key combination as the one managed by this instance.
        /// </summary>
        /// <param name="hotkey">A hotkey to check the key combination of.</param>
        /// <returns><see langword="true"/> when the <paramref name="hotkey"/> does have the same key combination; otherwise <see langword="false"/>.</returns>
        public bool HotkeyHasManagedKeyCombination(IHotkey hotkey) => hotkey.Key == ManagedKey && hotkey.Modifiers == ManagedModifiers;
        #endregion HasManagedKeyCombination

        #region AddHotkey
        /// <summary>
        /// Adds the specified <paramref name="hotkey"/> to this message hook abstractor instance.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> that is not being managed by this instance.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="hotkey"/> is already being managed by this instance.</exception>
        /// <exception cref="InvalidOperationException">The specified <paramref name="hotkey"/> does not have this instance's managed key combination.</exception>
        public void AddHotkey(IHotkey hotkey)
        {
            // validate the incoming hotkey
            if (!HasStarted)
            {
                HasStarted = true;
                ManagedKey = hotkey.Key;
                ManagedModifiers = hotkey.Modifiers;
            }
            else if (!HotkeyHasManagedKeyCombination(hotkey))
                throw new InvalidOperationException($"Hotkey {hotkey.ID} \"{hotkey.Name}\" cannot be added to this {nameof(HotkeyMessageHookAbstractor)} instance because it has a different key combination! ({hotkey.Key:G} {hotkey.Modifiers:G}) != ({ManagedKey:G} {ManagedModifiers:G})");
            else if (Hotkeys.Contains(hotkey))
                throw new ArgumentException($"Hotkey {hotkey.ID} \"{hotkey.Name}\" is already being managed by this {nameof(HotkeyMessageHookAbstractor)} instance!", nameof(hotkey));

            //< hotkey IS NOT using this message hook, and is valid

            if (hotkey.IsRegistered) // remove the hotkey's message hook
                WindowsHotkeyAPI.RemoveHook(hotkey);

            // attach event handlers:
            hotkey.Unregistering += this.Hotkey_Unregistering;
            hotkey.Unregistered += this.Hotkey_Unregistered;

            // add to the list:
            _ids.Add(hotkey.ID);
            _hotkeys.Add(hotkey);

            //< hotkey IS using this message hook
        }
        #endregion AddHotkey

        #region AddHotkeys
        /// <summary>
        /// Adds the specified <paramref name="hotkeys"/> to this message hook abstractor instance.
        /// </summary>
        /// <param name="hotkeys">Any number of <see cref="IHotkey"/> instances that are not being managed by this instance.</param>
        /// <exception cref="ArgumentException">At least one of the <paramref name="hotkeys"/> is already being managed by this instance.</exception>
        /// <exception cref="InvalidOperationException">At least one of the <paramref name="hotkeys"/> does not have the same key combination as the one managed by this instance.</exception>
        public void AddHotkeys(params IHotkey[] hotkeys)
        {
            for (int i = 0, i_max = hotkeys.Length; i < i_max; ++i)
            {
                AddHotkey(hotkeys[i]);
            }
        }
        #endregion AddHotkeys

        #region RemoveHotkey
        /// <summary>
        /// Removes the specified <paramref name="hotkey"/> from this message hook abstractor instance.
        /// </summary>
        /// <param name="hotkey">An <see cref="IHotkey"/> that is being managed by this instance.</param>
        /// <exception cref="ArgumentException">The specified <paramref name="hotkey"/> is not being managed by this instance.</exception>
        public void RemoveHotkey(IHotkey hotkey)
        {
            if (!Hotkeys.Contains(hotkey))
                throw new ArgumentException($"Cannot remove hotkey {hotkey.ID} \"{hotkey.Name}\" because it is not being managed by this {nameof(HotkeyMessageHookAbstractor)} instance!", nameof(hotkey));

            //< hotkey IS using this message hook

            // detatch event handlers:
            hotkey.Unregistering -= this.Hotkey_Unregistered;
            hotkey.Unregistered -= this.Hotkey_Unregistered;

            // remove from the list:
            _ids.Remove(hotkey.ID);
            _hotkeys.Remove(hotkey);

            if (hotkey.IsRegistered) // add the hotkey's message hook if it's still registered
                WindowsHotkeyAPI.AddHook(hotkey);

            //< hotkey IS NOT using this message hook

            switch (_ids.Count)
            {
            case 1:
                NotifyOneHotkeyRemaining(Hotkeys[0]);
                break;
            case 0:
                NotifyAllHotkeysRemoved();
                WindowsHotkeyAPI.RemoveHook(this);
                break;
            }
        }
        #endregion RemoveHotkey

        #endregion Methods

        #region EventHandlers

        #region Hotkey
        private void Hotkey_Unregistered(object? sender, EventArgs e)
        {
            RemoveHotkey((IHotkey)sender!);
        }
        private void Hotkey_Unregistering(object sender, HotkeyRegisteringEventArgs e)
        {
            e.Handled = true;
            e.RegistrationSuccessStateWhenHandled = true;
        }
        #endregion Hotkey

        #endregion EventHandlers
    }
}
