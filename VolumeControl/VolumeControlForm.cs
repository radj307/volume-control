using AudioAPI;
using HotkeyLib;

namespace VolumeControl
{
    public partial class VolumeControlForm : Form
    {
        #region Members

        /// <summary>
        /// Hotkey definition linked to volume up actions.
        /// </summary>
        private Hotkey hk_up;
        /// <summary>
        /// Hotkey definition linked to volume down actions.
        /// </summary>
        private Hotkey hk_down;
        /// <summary>
        /// Hotkey definition linked to toggle mute actions.
        /// </summary>
        private Hotkey hk_mute;
        /// <summary>
        /// Utility used to enumerate audio sessions to populate the Process selector box.
        /// </summary>
        private readonly AudioSessionList sessions = new();

        #endregion Members

        #region MemberOverrides

        /// <summary>
        /// Destructor
        /// Saves the current settings.
        /// </summary>
        ~VolumeControlForm()
        {
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            UnregisterHotkeys();
        }

        /// <summary>
        /// Overrides the base object's Visible member with a property that respects hotkeys.
        /// This is required because changing the value of Visible causes all hotkey registrations to expire.
        /// </summary>
        private new bool Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                RegisterHotkeys();
            }
        }

        #endregion MemberOverrides

        #region HelperMethods

        /// <summary>
        /// Register Hotkeys
        /// </summary>
        internal void RegisterHotkeys()
        {
            if (hk_up != null && !hk_up.Registered && HKEdit_VolumeUp.IsEnabled)
                hk_up.Register(this);
            if (hk_down != null && !hk_down.Registered && HKEdit_VolumeDown.IsEnabled)
                hk_down.Register(this);
            if (hk_mute != null && !hk_mute.Registered && HKEdit_VolumeMute.IsEnabled)
                hk_mute.Register(this);
        }
        /// <summary>
        /// Unregister Hotkeys
        /// </summary>
        internal void UnregisterHotkeys()
        {
            if (hk_up != null && hk_up.Registered)
                hk_up.Unregister();
            if (hk_down != null && hk_down.Registered)
                hk_down.Unregister();
            if (hk_mute != null && hk_mute.Registered)
                hk_mute.Unregister();
        }

        private static Hotkey TryInitHotkey(string hkString, Hotkey def)
        {
            Hotkey hk;
            try
            {
                // initializing hotkeys using a user-specified string might throw
                hk = new Hotkey(hkString);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                hk = def;
            }
            return hk;
        }

        private void UpdateTitle()
        {
            if (ComboBox_ProcessSelector.Text.Length > 0)
            {
                Text = $"{ComboBox_ProcessSelector.Text} Volume Controller";
            }
            else
            {
                Text = "Volume Control";
            }
        }

        /// <summary>
        /// Updates the options available in the Process selector box.
        /// </summary>
        private void UpdateProcessList()
        {
            sessions.UpdateProcessNames();
            ComboBox_ProcessSelector.DataSource = sessions.ProcessNames;
        }

        private void UpdateHotkeys()
        {
            UnregisterHotkeys();

            hk_up = HKEdit_VolumeUp.Hotkey;
            hk_up.Pressed += delegate { VolumeHelper.IncrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); };
            
            hk_down = HKEdit_VolumeDown.Hotkey;
            hk_down.Pressed += delegate { VolumeHelper.DecrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); };
            
            hk_mute = HKEdit_VolumeMute.Hotkey;
            hk_mute.Pressed += delegate { VolumeHelper.ToggleMute(Properties.Settings.Default.ProcessName); };

            RegisterHotkeys();
        }

        #endregion HelperMethods

        #region ClassFunctions

        /// <summary>
        /// Constructor
        /// </summary>
        public VolumeControlForm()
        {
            InitializeComponent();
            UpdateProcessList();

            // This needs to be done first, otherwise the window is destroyed & recreated which unbinds all of the hotkeys.
            ShowInTaskbar = true;

            // set all hotkeys to null to quiet compiler
            hk_up = null!;
            hk_down = null!;
            hk_mute = null!;

            // set the window visibility to false when the window is minimized
            Resize += delegate
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Visible = false;
                }
            };

            // Initialize hotkey editor names
            HKEdit_VolumeUp.Label = "Volume Up";
            HKEdit_VolumeDown.Label = "Volume Down";
            HKEdit_VolumeMute.Label = "Toggle Mute";

            HKEdit_VolumeUp.IsEnabled = Properties.Settings.Default.hk_volumeup_enabled;
            HKEdit_VolumeDown.IsEnabled = Properties.Settings.Default.hk_volumedown_enabled;
            HKEdit_VolumeMute.IsEnabled = Properties.Settings.Default.hk_volumemute_enabled;

            UpdateHotkeys();

            // populate settings boxes
            ComboBox_ProcessSelector.Text = Properties.Settings.Default.ProcessName;
            volume_step.Value = Properties.Settings.Default.VolumeStep;
            bool minimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            checkbox_minimizeOnStartup.Checked = minimizeOnStartup;

            if (minimizeOnStartup)
            {
                WindowState = FormWindowState.Minimized;
            }

            UpdateTitle();
        }


        /// <summary>
        /// Overrides the base object's ShowInTaskbar member with a property that respects hotkeys.
        /// This is required because changing the value of ShowInTaskbar causes all hotkey registrations to expire.
        /// </summary>
        private new bool ShowInTaskbar
        {
            get => base.ShowInTaskbar;
            set
            {
                base.ShowInTaskbar = value;
                RegisterHotkeys();
            }
        }

        #endregion ClassFunctions

        #region FormComponents

        /// <summary>
        /// Automatically called when the value of "ComboBox_ProcessSelector.Text" is changed.
        /// Sets the settings value "ProcessName" to the new value, and updates the window title.
        /// </summary>
        private void process_name_text_changed(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProcessName = ComboBox_ProcessSelector.Text;
            UpdateTitle();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Automatically called when the value of volume_step is changed.
        /// Sets the settings value "VolumeStep" to the new value.
        /// </summary>
        private void volume_step_event(object sender, EventArgs e)
        {
            Properties.Settings.Default.VolumeStep = Convert.ToDecimal(volume_step.Value);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the system tray icon is double-clicked
        /// </summary>
        private void system_tray_double_click(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                // make menu visible
                Visible = true;
                WindowState = FormWindowState.Normal;
            }
        }
        /// <summary>
        /// Called when the user clicks the close button in the system tray context menu
        /// </summary>
        private void system_tray_menu_close(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// Called when the "Minimize on Startup" checkbox is changed.
        /// </summary>
        private void checkbox_minimizeOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeOnStartup = checkbox_minimizeOnStartup.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the window is focused by the user.
        /// </summary>
        private void window_got_focus_event(object sender, EventArgs e)
        {
            UpdateProcessList();
        }
        /// <summary>
        /// Called when the hotkey for VolumeUp was modified
        /// </summary>
        private void HKEdit_VolumeUp_HotkeyChanged(object sender, EventArgs e)
        {
            hk_up = HKEdit_VolumeUp.Hotkey;
            Properties.Settings.Default.hk_volumeup = hk_up.ToString();
            Properties.Settings.Default.hk_volumeup_enabled = HKEdit_VolumeUp.IsEnabled;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the hotkey for VolumeDown was modified
        /// </summary>
        private void HKEdit_VolumeDown_HotkeyChanged(object sender, EventArgs e)
        {
            hk_down = HKEdit_VolumeDown.Hotkey;
            Properties.Settings.Default.hk_volumedown = hk_down.ToString();
            Properties.Settings.Default.hk_volumedown_enabled = HKEdit_VolumeDown.IsEnabled;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the hotkey for VolumeMute was modified
        /// </summary>
        private void HKEdit_VolumeMute_HotkeyChanged(object sender, EventArgs e)
        {
            hk_mute = HKEdit_VolumeMute.Hotkey;
            Properties.Settings.Default.hk_volumemute = hk_mute.ToString();
            Properties.Settings.Default.hk_volumemute_enabled = HKEdit_VolumeMute.IsEnabled;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        #endregion FormComponents
    }
}