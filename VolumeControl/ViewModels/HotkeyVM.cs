using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Actions;

namespace VolumeControl.ViewModels
{
    public class HotkeyVM : INotifyPropertyChanged, IDisposable
    {
        #region Constructor
        public HotkeyVM(HotkeyWithError hotkey)
        {
            Hotkey = hotkey;
        }
        #endregion Constructor

        #region Properties
        public HotkeyWithError Hotkey { get; }
        public HotkeyActionDefinition? ActionDefinition
        {
            get => Hotkey.Action?.HotkeyActionDefinition;
            set
            {
                Hotkey.Action = value?.CreateInstance();
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(UsesActionSettings));
            }
        }
        public bool UsesActionSettings
            => Hotkey.Action != null && Hotkey.Action.ActionSettings.Length > 0;
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region IDisposable Implementation
        public void Dispose()
        {
            ((IDisposable)this.Hotkey).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
