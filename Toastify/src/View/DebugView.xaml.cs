using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using Toastify.Model;

namespace Toastify.View
{
    public partial class DebugView : Window
    {
        private Settings CurrentSettings { get { return Settings.Current; } }
        private Settings PreviewSettings { get; set; }

        public DebugView()
        {
            this.InitializeComponent();

            SettingsView.SettingsLaunched += this.SettingsView_SettingsLaunched;
            SettingsView.SettingsClosed += this.SettingsView_SettingsClosed;
        }

        private void ButtonPrintCurrentHotkeys_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("\n======= DebugView =======");
            Debug.WriteLine("CURRENT SETTINGS HOTKEYS:");
            if (this.CurrentSettings?.HotKeys != null)
            {
                foreach (var h in this.CurrentSettings.HotKeys)
                    Debug.WriteLine(h.ToString());
            }

            if (this.PreviewSettings != null)
            {
                Debug.WriteLine("\nPREVIEW SETTINGS HOTKEYS:");
                if (this.PreviewSettings.HotKeys != null)
                {
                    foreach (var h in this.PreviewSettings.HotKeys)
                        Debug.WriteLine(h.ToString());
                }
            }
            Debug.WriteLine("=========================\n");
        }

        private void DebugView_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            SettingsView.SettingsLaunched -= this.SettingsView_SettingsLaunched;
        }

        private void SettingsView_SettingsLaunched(object sender, Events.SettingsViewLaunchedEventArgs e)
        {
            this.PreviewSettings = e.Settings;
        }

        private void SettingsView_SettingsClosed(object sender, System.EventArgs e)
        {
            this.PreviewSettings = null;
        }
    }
}