﻿using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Input.Enums;
using VolumeControl.Core.Input.Json;

namespace VolumeControl.Core.Input
{
    /// <summary>
    /// Manages a list of <see cref="Hotkey"/> instances.
    /// </summary>
    public class HotkeyManager
    {
        #region Constructors
        /// <summary>
        /// Creates a new <see cref="HotkeyManager"/> instance from the specified <paramref name="actionManager"/>.
        /// </summary>
        /// <param name="actionManager">A <see cref="HotkeyActionManager"/> instance to use for managing hotkey actions.</param>
        public HotkeyManager(HotkeyActionManager actionManager)
        {
            HotkeyActionManager = actionManager;
        }
        /// <summary>
        /// Creates a new <see cref="HotkeyManager"/> instance with a new <see cref="Actions.HotkeyActionManager"/> instance.
        /// </summary>
        public HotkeyManager() : this(new HotkeyActionManager()) { }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the action manager instance for this HotkeyManager.
        /// </summary>
        public HotkeyActionManager HotkeyActionManager { get; }
        /// <summary>
        /// Gets the hotkey instances that are being managed by this HotkeyManager.
        /// </summary>
        public IReadOnlyList<Hotkey> Hotkeys => _hotkeys;
        private readonly List<Hotkey> _hotkeys = new();
        /// <summary>
        /// Gets the list of <see cref="HotkeyMessageHookAbstractor"/> instances that are currently active.
        /// </summary>
        public IReadOnlyList<HotkeyMessageHookAbstractor> HookAbstractors => _hookAbstractors;
        private readonly List<HotkeyMessageHookAbstractor> _hookAbstractors = new();
        #endregion Properties

        #region Events
        /// <summary>
        /// Occurs when a hotkey is added to the list for any reason.
        /// </summary>
        public event EventHandler<Hotkey>? AddedHotkey;
        private void NotifyAddedHotkey(Hotkey hotkey) => AddedHotkey?.Invoke(this, hotkey);
        /// <summary>
        /// Occurs when a hotkey is removed from the list for any reason.
        /// </summary>
        public event EventHandler<Hotkey>? RemovedHotkey;
        private void NotifyRemovedHotkey(Hotkey hotkey) => RemovedHotkey?.Invoke(this, hotkey);
        #endregion Events

        #region Methods

        #region GetRegisteredHotkeyWithKeyCombination
        private Hotkey? GetRegisteredHotkeyWithKeyCombination(EFriendlyKey key, EModifierKey modifiers)
        {
            foreach (var hotkey in Hotkeys)
            {
                if (!hotkey.IsRegistered) continue;

                if (hotkey.Key == key && hotkey.Modifiers == modifiers)
                    return hotkey;
            }

            return null;
        }
        #endregion GetRegisteredHotkeyWithKeyCombination

        #region GetHotkey
        /// <summary>
        /// Gets the <see cref="Hotkey"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The unique ID number of the hotkey to get.</param>
        /// <returns>The <see cref="Hotkey"/> instance with the specified <paramref name="id"/> number if found; otherwise <see langword="null"/>.</returns>
        public Hotkey? GetHotkey(ushort id)
            => Hotkeys.FirstOrDefault(hk => hk.ID.Equals(id));
        #endregion GetHotkey

        #region Add/Remove Hotkey
        /// <summary>
        /// Adds the specified <paramref name="hotkey"/> to the manager.
        /// </summary>
        /// <param name="hotkey">A <see cref="Hotkey"/> instance to add to the list of managed hotkeys.</param>
        /// <exception cref="ArgumentException">The <paramref name="hotkey"/> is already being managed by this <see cref="HotkeyManager"/>.</exception>
        public void AddHotkey(Hotkey hotkey)
        {
            if (Hotkeys.Contains(hotkey))
                throw new ArgumentException($"Hotkey instance '{hotkey.Name}' ({hotkey.GetStringRepresentation()}) is already being managed by this {nameof(HotkeyManager)} instance!");

            hotkey.Registering += this.Hotkey_Registering;

            _hotkeys.Add(hotkey);
            NotifyAddedHotkey(hotkey);
        }
        /// <summary>
        /// Removes the specified <paramref name="hotkey"/> from the manager.
        /// </summary>
        /// <param name="hotkey">A <see cref="Hotkey"/> instance to remove from the list of managed hotkeys.</param>
        /// <returns><see langword="true"/> when the <paramref name="hotkey"/> was successfully removed; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="hotkey"/> is not being managed by this <see cref="HotkeyManager"/>.</exception>
        public bool RemoveHotkey(Hotkey hotkey)
        {
            if (!Hotkeys.Contains(hotkey))
                throw new ArgumentException($"Hotkey instance '{hotkey.Name}' ({hotkey.GetStringRepresentation()}) is not being managed by this {nameof(HotkeyManager)} instance!");

            hotkey.Registering -= this.Hotkey_Registering;

            _hotkeys.Remove(hotkey);
            NotifyRemovedHotkey(hotkey);
            hotkey.Dispose(); //< dispose of the hotkey
            return true;
        }
        /// <summary>
        /// Removes the hotkey with the specified <paramref name="id"/> from the manager.
        /// </summary>
        /// <param name="id">The unique ID number of a hotkey to remove.</param>
        /// <returns><see langword="true"/> when the hotkey was successfully removed; otherwise <see langword="false"/>.</returns>
        /// <exception cref="ArgumentException">The hotkey with <paramref name="id"/> is not being managed by this <see cref="HotkeyManager"/>.</exception>
        public bool RemoveHotkey(ushort id)
            => GetHotkey(id) is Hotkey hk && RemoveHotkey(hk);
        #endregion Add/Remove Hotkey

        #region ClearHotkeys
        /// <summary>
        /// Removes all hotkeys from the manager.
        /// </summary>
        public void ClearHotkeys()
        {
            for (int i = Hotkeys.Count - 1; i >= 0; --i)
            {
                RemoveHotkey(Hotkeys[i]);
            }
        }
        #endregion ClearHotkeys

        #region GetJsonHotkeys
        /// <summary>
        /// Gets a list of <see cref="JsonHotkey"/> instances for the list of managed hotkeys.
        /// </summary>
        /// <returns>An array of <see cref="JsonHotkey"/> instances.</returns>
        public virtual JsonHotkey[] GetJsonHotkeys()
        {
            List<JsonHotkey> l = new();

            for (int i = 0, max = Hotkeys.Count; i < max; ++i)
            {
                l.Add(new(Hotkeys[i]));
            }

            return l.ToArray();
        }
        #endregion GetJsonHotkeys

        #region AddJsonHotkeys
        /// <summary>
        /// Creates and adds a Hotkey for each element of the specified <paramref name="hotkeyJsonObjects"/>.
        /// </summary>
        /// <typeparam name="THotkey">A type derived from <see cref="Hotkey"/> to create. Specifying this allows you to substitute derived types in place of the default <see cref="Hotkey"/> type.</typeparam>
        /// <param name="hotkeyJsonObjects">The JSON object representation of any number of Hotkeys.</param>
        public void AddJsonHotkeys<THotkey>(params JsonHotkey[] hotkeyJsonObjects) where THotkey : Hotkey
        {
            for (int i = 0, max = hotkeyJsonObjects.Length; i < max; ++i)
            {
                var hotkeyJsonObject = hotkeyJsonObjects[i];
                var hotkey = hotkeyJsonObject.CreateInstance<THotkey>(HotkeyActionManager, deferRegistration: true);
                AddHotkey(hotkey);
                hotkey.IsRegistered = hotkeyJsonObject.IsRegistered;
            }
        }
        #endregion AddJsonHotkeys

        #region SetHotkeysFromJsonHotkeys
        /// <summary>
        /// Sets the list of managed hotkeys from the specified <paramref name="hotkeyJsonObjects"/>.
        /// </summary>
        /// <typeparam name="THotkey">A type derived from <see cref="Hotkey"/> to create. Specifying this allows you to substitute derived types in place of the default <see cref="Hotkey"/> type.</typeparam>
        /// <param name="hotkeyJsonObjects">The JSON object representation of any number of Hotkeys.</param>
        public void SetHotkeysFromJsonHotkeys<THotkey>(JsonHotkey[] hotkeyJsonObjects) where THotkey : Hotkey
        {
            ClearHotkeys(); //< clear the list
            AddJsonHotkeys<THotkey>(hotkeyJsonObjects);
        }
        /// <inheritdoc cref="SetHotkeysFromJsonHotkeys{THotkey}(JsonHotkey[])"/>
        public virtual void SetHotkeysFromJsonHotkeys(JsonHotkey[] hotkeyJsonObjects)
            => SetHotkeysFromJsonHotkeys<Hotkey>(hotkeyJsonObjects);
        #endregion SetHotkeysFromJsonHotkeys

        #endregion Methods

        #region EventHandlers

        #region Hotkey
        private void Hotkey_Registering(object? sender, HotkeyRegisteringEventArgs e)
        {
            var hotkey = (Hotkey)sender!;

            foreach (var existingHookManager in HookAbstractors)
            {
                if (existingHookManager.HotkeyHasManagedKeyCombination(hotkey))
                { // a hook manager already exists for this duplicate key combination:
                    existingHookManager.AddHotkey(hotkey);
                    e.Handled = true;
                    e.RegistrationSuccessStateWhenHandled = true;
                    return;
                }
            }

            var existingHotkey = GetRegisteredHotkeyWithKeyCombination(hotkey.Key, hotkey.Modifiers);

            if (existingHotkey == null) return; //< this hotkey is not a duplicate, register it normally.

            e.Handled = true; //< don't allow this hotkey to register itself
            e.RegistrationSuccessStateWhenHandled = true;

            var hookManager = new HotkeyMessageHookAbstractor();

            hookManager.AddHotkey(existingHotkey);
            hookManager.AddHotkey(hotkey);

            hookManager.OneHotkeyRemaining += this.HookManager_OneHotkeyRemaining;

            _hookAbstractors.Add(hookManager);
        }
        #endregion Hotkey

        #region HookManager
        private void HookManager_OneHotkeyRemaining(object? sender, IHotkey e)
        {
            var hookManager = (HotkeyMessageHookAbstractor)sender!;

            hookManager.RemoveHotkey(e);

            _hookAbstractors.Remove(hookManager);
        }
        #endregion HookManager

        #endregion EventHandlers
    }
}
