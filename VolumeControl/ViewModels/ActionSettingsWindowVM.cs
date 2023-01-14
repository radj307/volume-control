using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolumeControl.Core.Input;
using VolumeControl.Core.Input.Actions;
using VolumeControl.Core.Interfaces;
using VolumeControl.WPF.Collections;

namespace VolumeControl.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ActionSettingsWindowVM
    {
        #region Properties
        /// <summary>
        /// Controls the window title.
        /// </summary>
        public string WindowTitle { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the hotkey that this window is currently editing
        /// </summary>
        public IBindableHotkey? Hotkey { get; set; }
        public ObservableImmutableList<IHotkeyActionSetting>? ActionSettings => Hotkey?.ActionSettings;
        #endregion Properties
    }
}
