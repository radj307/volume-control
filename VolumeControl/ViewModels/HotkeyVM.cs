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
            Hotkey.PropertyChanged += this.Hotkey_PropertyChanged;

            UsesAutoName = Hotkey.Name.Equals(Hotkey.Action?.Name, StringComparison.Ordinal);
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

                if (Hotkey.Action != null && (UsesAutoName || Hotkey.Name.Length == 0))
                { // give the hotkey a name if it doesn't have one
                    Hotkey.PropertyChanged -= this.Hotkey_PropertyChanged;
                    Hotkey.Name = Hotkey.Action.Identifier;
                    Hotkey.PropertyChanged += this.Hotkey_PropertyChanged;
                    UsesAutoName = true;
                }
            }
        }
        public bool UsesActionSettings
            => Hotkey.Action != null && Hotkey.Action.ActionSettings.Length > 0;
        public bool UsesAutoName { get; set; }
        #endregion Properties

        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
        #endregion Events

        #region EventHandlers

        #region Hotkey
        private void Hotkey_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null) return;

            if (e.PropertyName.Equals(nameof(Hotkey.Name), StringComparison.Ordinal))
            {
                UsesAutoName = false;
            }
        }
        #endregion Hotkey

        #endregion EventHandlers

        #region IDisposable Implementation
        public void Dispose()
        {
            ((IDisposable)this.Hotkey).Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Implementation
    }
}
