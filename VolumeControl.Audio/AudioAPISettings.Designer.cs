﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VolumeControl.Audio {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")]
    public sealed partial class AudioAPISettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static AudioAPISettings defaultInstance = ((AudioAPISettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AudioAPISettings())));
        
        public static AudioAPISettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int AutoReloadInterval {
            get {
                return ((int)(this["AutoReloadInterval"]));
            }
            set {
                this["AutoReloadInterval"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("500")]
        public int AutoReloadIntervalMin {
            get {
                return ((int)(this["AutoReloadIntervalMin"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120000")]
        public int AutoReloadIntervalMax {
            get {
                return ((int)(this["AutoReloadIntervalMax"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LockSelectedSession {
            get {
                return ((bool)(this["LockSelectedSession"]));
            }
            set {
                this["LockSelectedSession"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool LockSelectedDevice {
            get {
                return ((bool)(this["LockSelectedDevice"]));
            }
            set {
                this["LockSelectedDevice"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SelectedSession {
            get {
                return ((string)(this["SelectedSession"]));
            }
            set {
                this["SelectedSession"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SelectedDevice {
            get {
                return ((string)(this["SelectedDevice"]));
            }
            set {
                this["SelectedDevice"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2")]
        public int VolumeStepSize {
            get {
                return ((int)(this["VolumeStepSize"]));
            }
            set {
                this["VolumeStepSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CheckAllDevices {
            get {
                return ((bool)(this["CheckAllDevices"]));
            }
            set {
                this["CheckAllDevices"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ReloadOnHotkey {
            get {
                return ((bool)(this["ReloadOnHotkey"]));
            }
            set {
                this["ReloadOnHotkey"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int ReloadOnHotkeyMinInterval {
            get {
                return ((int)(this["ReloadOnHotkeyMinInterval"]));
            }
            set {
                this["ReloadOnHotkeyMinInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ReloadOnInterval {
            get {
                return ((bool)(this["ReloadOnInterval"]));
            }
            set {
                this["ReloadOnInterval"] = value;
            }
        }
    }
}