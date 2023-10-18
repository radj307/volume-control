using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VolumeControl.Core;
using VolumeControl.Core.Input;
using VolumeControl.Log;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    public class HotkeyManagerVM : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        public HotkeyManagerVM()
        {
            HotkeyManager = new();

            Hotkeys = new();

            HotkeyManager.AddedHotkey += this.HotkeyManager_AddedHotkey;
            HotkeyManager.RemovedHotkey += this.HotkeyManager_RemovedHotkey;
        }
        #endregion Constructor

        #region Properties
        private static Config Settings => (Config.Default as Config)!;
        public HotkeyManager HotkeyManager { get; }
        public ObservableImmutableList<HotkeyVM> Hotkeys { get; }
        public bool? AllSelected
        {
            get
            {
                var total = Hotkeys.Count;
                var selected = Hotkeys.Count(hkvm => hkvm.Hotkey.IsRegistered);

                if (selected == total) return true;
                else if (selected == 0) return false;
                else return null;
            }
            set
            {
                var newState = value == true;
                Hotkeys.ForEach(hkvm => hkvm.Hotkey.IsRegistered = newState);
                NotifyPropertyChanged();
            }
        }
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region Methods

        #region GetHotkeyVM
        /// <summary>
        /// Gets the viewmodel associated with the specified <paramref name="hotkey"/>.
        /// </summary>
        /// <param name="hotkey">A <see cref="Hotkey"/> instance to get the viewmodel for.</param>
        /// <returns>The <see cref="HotkeyVM"/> instance associated with the specified <paramref name="hotkey"/>.</returns>
        public HotkeyVM GetHotkeyVM(Hotkey hotkey)
            => Hotkeys.First(hk => hk.Equals(hotkey));
        #endregion GetHotkeyVM

        #region Save/Load/Reset Hotkeys
        public void LoadHotkeys()
        {
            if (Settings.Hotkeys == null)
            {
                FLog.Log.Critical("There was a problem with the hotkeys provided by the previous configuration. Attempting to load default hotkeys instead.");
                Settings.Hotkeys = Config.Hotkeys_Default;
            }
            HotkeyManager.SetHotkeysFromJsonHotkeys<HotkeyWithError>(Settings.Hotkeys);
        }
        public void SaveHotkeys()
        {
            Settings.Hotkeys = HotkeyManager.GetJsonHotkeys();
        }
        public void ResetHotkeys()
        {
            HotkeyManager.SetHotkeysFromJsonHotkeys<HotkeyWithError>(Config.Hotkeys_Default);
        }
        #endregion Save/Load/Reset Hotkeys

        #endregion Methods

        #region EventHandlers

        #region HotkeyManager
        private void HotkeyManager_AddedHotkey(object? sender, Hotkey e)
        {
            var vm = new HotkeyVM((HotkeyWithError)e);
            vm.Hotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            Hotkeys.Add(vm);
        }
        private void HotkeyManager_RemovedHotkey(object? sender, Hotkey e)
        {
            //< GetHotkeyVM is not reliable here
            if (Hotkeys.FirstOrDefault(hk => hk.Hotkey.Equals(e)) is HotkeyVM vm)
            {
                vm.Hotkey.PropertyChanged -= this.Hotkey_PropertyChanged;
                Hotkeys.Remove(vm);
            }
        }
        #endregion HotkeyManager

        #region Hotkey
        private void Hotkey_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SaveHotkeys();
            if (e.PropertyName?.Equals(nameof(Hotkey.IsRegistered), StringComparison.Ordinal) ?? false)
            {
                NotifyPropertyChanged(nameof(AllSelected));
            }
        }
        #endregion Hotkey

        #endregion EventHandlers

        #region IDisposable Implementation
        public void Dispose()
        {
            foreach (var hk in Hotkeys)
            {
                hk.Dispose();
            }
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
