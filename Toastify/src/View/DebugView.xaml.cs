using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Toastify.Core;
using Toastify.Events;
using Toastify.Model;
using Toastify.Threading;

namespace Toastify.View
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class DebugView : Window
    {
#if DEBUG

        internal static DebugView Current { get; private set; }

        private Settings PreviewSettings { get; set; }
        private IReadOnlyList<GenericHotkeyProxy> PreviewHotkeys { get; set; }

        public DebugView()
        {
            this.InitializeComponent();

            this.DataContext = this;
            Current = this;

            SettingsView.SettingsLaunched += this.SettingsView_SettingsLaunched;
            SettingsView.SettingsClosed += this.SettingsView_SettingsClosed;
            SettingsView.SettingsSaved += this.SettingsView_SettingsSaved;
        }

        internal static void Launch()
        {
            if (Current != null)
                return;

            WindowThread<DebugView> thread = ThreadManager.Instance.CreateWindowThread<DebugView>(ApartmentState.STA);
            thread.IsBackground = true;
            thread.ThreadName = $"{nameof(DebugView)}_Thread";
            thread.Start();
        }

        private void ButtonPrintSettings_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n======= DebugView =======");
            Debug.WriteLine("SETTINGS [Current | (Preview) | Default]");
            Debug.WriteLine("");

            PropertyInfo[] properties = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (this.PreviewSettings != null)
            {
                foreach (PropertyInfo property in properties)
                {
                    dynamic current = property.GetValue(Settings.Current);
                    dynamic preview = property.GetValue(this.PreviewSettings);
                    dynamic @default = property.GetValue(Settings.Default);

                    if (property.PropertyType.GetInterfaces().Contains(typeof(ISettingValue)))
                        Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-25} | {preview?.ToString(),-25} | {@default?.ToString(),-25}");
                    else
                    {
                        if (property.PropertyType.GetInterfaces().Contains(typeof(ICollection)))
                            continue;

                        if (property.PropertyType == typeof(ProxyConfigAdapter))
                        {
                            var cp = (ProxyConfigAdapter)current;
                            var pp = (ProxyConfigAdapter)preview;
                            var dp = (ProxyConfigAdapter)@default;

                            Debug.WriteLine($"{property.Name,-36}:  {cp?.ToString(true),-30} | {pp?.ToString(true),-30} | {dp?.ToString(true),-30}");
                        }
                        else if (property.PropertyType.IsPrimitive)
                            Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-30} | {preview?.ToString(),-30} | {@default?.ToString(),-30}");
                    }
                }
            }
            else
            {
                foreach (PropertyInfo property in properties)
                {
                    dynamic current = property.GetValue(Settings.Current);
                    dynamic @default = property.GetValue(Settings.Default);

                    if (property.PropertyType.GetInterfaces().Contains(typeof(ISettingValue)))
                        Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-25} | {@default?.ToString(),-25}");
                    else
                    {
                        if (property.PropertyType.GetInterfaces().Contains(typeof(ICollection)))
                            continue;

                        if (property.PropertyType == typeof(ProxyConfigAdapter))
                        {
                            var cp = (ProxyConfigAdapter)current;
                            var dp = (ProxyConfigAdapter)@default;

                            Debug.WriteLine($"{property.Name,-36}:  {cp?.ToString(true),-30} | {dp?.ToString(true),-30}");
                        }
                        else if (property.PropertyType.IsPrimitive)
                            Debug.WriteLine($"{property.Name,-36}:  {current?.ToString(),-30} | {@default?.ToString(),-30}");
                    }
                }
            }

            Debug.WriteLine("=========================\n");
        }

        private void ButtonPrintCurrentHotkeys_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n======= DebugView =======");
            Debug.WriteLine("HOTKEYS [Current | (Preview) | Default]");
            Debug.WriteLine("");

            int GetObjectHash(object obj) => RuntimeHelpers.GetHashCode(obj);
            Debug.Write($"{string.Empty,-15}  ");
            Debug.Write($"@ {GetObjectHash(Settings.Current),-23} | ");
            Debug.Write(this.PreviewSettings != null ? $"@ {GetObjectHash(this.PreviewSettings),-23} | " : "");
            Debug.Write($"@ {GetObjectHash(Settings.Default),-23}\n");

            foreach (Hotkey hotkey in Settings.Default.HotKeys)
            {
                Hotkey current = Settings.Current.HotKeys?.SingleOrDefault(h => h?.Action.Equals(hotkey.Action) ?? false);
                Hotkey preview = this.PreviewHotkeys?.SingleOrDefault(h => h?.Hotkey?.Action.Equals(hotkey.Action) ?? false)?.Hotkey;
                Hotkey @default = hotkey;

                Debug.Write($"{hotkey.HumanReadableAction,-15}: ");
                Debug.Write($"[{(current?.Enabled == true ? 'E' : ' ')}{(current?.Active == true ? 'A' : ' ')}] {current?.HumanReadableKey ?? "—",-20} | ");
                Debug.Write(this.PreviewSettings != null ? $"[{(preview?.Enabled == true ? 'E' : ' ')}{(preview?.Active == true ? 'A' : ' ')}] {preview?.HumanReadableKey ?? "—",-20} | " : "");
                Debug.Write($"[{(@default?.Enabled == true ? 'E' : ' ')}{(@default?.Active == true ? 'A' : ' ')}] {@default?.HumanReadableKey ?? "—",-20}\n");
            }

            Debug.WriteLine("=========================\n");
        }

        private void DebugView_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            SettingsView.SettingsLaunched -= this.SettingsView_SettingsLaunched;

            if (Settings.Current?.HotKeys != null)
            {
                string showDebugViewHotkey = (from Hotkey h in Settings.Current.HotKeys
                                              let toastifyAction = h.Action as ToastifyAction
                                              where toastifyAction != null && toastifyAction.ToastifyActionEnum == ToastifyActionEnum.ShowDebugView && h.Enabled && h.Active
                                              select h.HumanReadableKey).SingleOrDefault();

                if (!string.IsNullOrEmpty(showDebugViewHotkey))
                {
                    Debug.WriteLine("\n======= DebugView =======");
                    Debug.WriteLine($"{typeof(DebugView).Name} is closing. You can re-open it with \"{showDebugViewHotkey}\".");
                    Debug.WriteLine("=========================\n");
                }
            }

            Current = null;
        }

        private void SettingsView_SettingsLaunched(object sender, SettingsViewLaunchedEventArgs e)
        {
            this.PreviewSettings = e.Settings;
            this.PreviewHotkeys = e.SettingsViewModel.Hotkeys;
        }

        private void SettingsView_SettingsClosed(object sender, EventArgs e)
        {
            this.PreviewSettings = null;
            this.PreviewHotkeys = null;
        }

        private void SettingsView_SettingsSaved(object sender, SettingsSavedEventArgs e)
        {
            this.PreviewSettings = e.Settings;
            this.PreviewHotkeys = e.PreviewHotkeys;
        }

        private void LogShowToastAction_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ToastView.Current != null)
            {
                ToastView.Current.LogShowToastAction = true;
                this.cbLogShowToastAction.IsChecked = true;
            }
        }

        private void LogShowToastAction_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (ToastView.Current != null)
            {
                ToastView.Current.LogShowToastAction = false;
                this.cbLogShowToastAction.IsChecked = false;
            }
        }

#endif
    }
}