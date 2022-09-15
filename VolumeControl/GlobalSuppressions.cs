// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>", Scope = "member", Target = "~F:VolumeControl.Program.appMutex")]
[assembly: SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.Program.Main(System.String[])")]
[assembly: SuppressMessage("Style", "IDE0018:Inline variable declaration", Justification = "Mutex's constructor does not initialize the isNewInstance boolean, which causes an access violation exception.", Scope = "member", Target = "~M:VolumeControl.Program.Main(System.String[])")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.AdvancedHotkeyMode")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.CheckForUpdates")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.CustomAddonDirectories")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.CustomLocalizationDirectories")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.EnableDefaultDevice")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationEnabled")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationMode")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationShowsVolumeChange")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationTimeout")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.ShowIcons")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.ShowUpdateMessageBox")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.StartMinimized")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.PeakMeterUpdateIntervalMs")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.ShowPeakMeters")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.EnableDeviceControl")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.ShowInTaskbar")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.AlwaysOnTop")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationShowsCustomControls")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.AllowMultipleDistinctInstances")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.ConfigLocation")]
[assembly: SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>", Scope = "member", Target = "~P:VolumeControl.Helpers.VCSettings.NotificationDragRequiresAlt")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.displayableControlsTemplate_Loaded(System.Object,System.Windows.RoutedEventArgs)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.displayableControlsTemplate_Unloaded(System.Object,System.Windows.RoutedEventArgs)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.lnotifWindow_Closing(System.Object,System.ComponentModel.CancelEventArgs)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.lnotifWindow_LocationChanged(System.Object,System.EventArgs)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.lnotifWindow_MouseDown(System.Object,System.Windows.Input.MouseButtonEventArgs)")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.ListNotification.lnotifWindow_SizeChanged(System.Object,System.Windows.SizeChangedEventArgs)")]
[assembly: SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "<Pending>", Scope = "member", Target = "~M:VolumeControl.Helpers.Addon.AttributeWrapper.GetFromType(System.Type,VolumeControl.Helpers.Addon.AttributeWrapper)~System.ValueTuple{System.Reflection.MemberInfo,System.Attribute}")]
