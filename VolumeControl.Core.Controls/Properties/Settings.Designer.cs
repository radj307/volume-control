﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VolumeControl.Core.Controls.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.0.3.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public byte DisplayCorner {
            get {
                return ((byte)(this["DisplayCorner"]));
            }
            set {
                this["DisplayCorner"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10, 10")]
        public global::System.Drawing.Size DisplayPadding {
            get {
                return ((global::System.Drawing.Size)(this["DisplayPadding"]));
            }
            set {
                this["DisplayPadding"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DisplayScreen {
            get {
                return ((string)(this["DisplayScreen"]));
            }
            set {
                this["DisplayScreen"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200, 17, 31")]
        public global::System.Drawing.Color LockedColor {
            get {
                return ((global::System.Drawing.Color)(this["LockedColor"]));
            }
            set {
                this["LockedColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("39, 204, 30")]
        public global::System.Drawing.Color UnlockedColor {
            get {
                return ((global::System.Drawing.Color)(this["UnlockedColor"]));
            }
            set {
                this["UnlockedColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Segoe UI, 9.75pt")]
        public global::System.Drawing.Font ToastFont {
            get {
                return ((global::System.Drawing.Font)(this["ToastFont"]));
            }
            set {
                this["ToastFont"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0, 0")]
        public global::System.Drawing.Size DisplayOffset {
            get {
                return ((global::System.Drawing.Size)(this["DisplayOffset"]));
            }
            set {
                this["DisplayOffset"] = value;
            }
        }
    }
}
