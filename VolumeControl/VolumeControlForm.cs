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
            if (hk_up != null && !hk_up.Registered && HKEdit_VolumeUp.HotkeyIsEnabled)
                hk_up.Register(this);
            if (hk_down != null && !hk_down.Registered && HKEdit_VolumeDown.HotkeyIsEnabled)
                hk_down.Register(this);
            if (hk_mute != null && !hk_mute.Registered && HKEdit_VolumeMute.HotkeyIsEnabled)
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

            hk_up.Reset(Properties.Settings.Default.hk_volumeup);
            hk_down.Reset(Properties.Settings.Default.hk_volumedown);
            hk_mute.Reset(Properties.Settings.Default.hk_volumemute);

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
            hk_up = new(Properties.Settings.Default.hk_volumeup, delegate { VolumeHelper.IncrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); });

            hk_down = new(Properties.Settings.Default.hk_volumedown, delegate { VolumeHelper.DecrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); });

            hk_mute = new(Properties.Settings.Default.hk_volumemute, delegate { VolumeHelper.ToggleMute(Properties.Settings.Default.ProcessName); });

            // set the window visibility to false when the window is minimized
            Resize += delegate
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Visible = false;
                }
            };

            // populate settings boxes
            HKEdit_VolumeUp.Hotkey = hk_up;
            HKEdit_VolumeUp.SetLabel("Volume Up");
            HKEdit_VolumeUp.SetHotkeyIsEnabled(Properties.Settings.Default.hk_volumeup_enabled);
            HKEdit_VolumeUp.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_volumeup_enabled = HKEdit_VolumeUp.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            };
            HKEdit_VolumeUp.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_volumeup_enabled = HKEdit_VolumeUp.HotkeyIsEnabled;
                Properties.Settings.Default.hk_volumeup = HKEdit_VolumeUp.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };

            HKEdit_VolumeDown.Hotkey = hk_down;
            HKEdit_VolumeDown.SetLabel("Volume Down");
            HKEdit_VolumeDown.SetHotkeyIsEnabled(Properties.Settings.Default.hk_volumedown_enabled);
            HKEdit_VolumeDown.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_volumedown_enabled = HKEdit_VolumeDown.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            };
            HKEdit_VolumeDown.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_volumedown_enabled = HKEdit_VolumeDown.HotkeyIsEnabled;
                Properties.Settings.Default.hk_volumedown = HKEdit_VolumeDown.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };


            HKEdit_VolumeMute.Hotkey = hk_mute;
            HKEdit_VolumeMute.SetLabel("Toggle Mute");
            HKEdit_VolumeMute.SetHotkeyIsEnabled(Properties.Settings.Default.hk_volumemute_enabled);
            HKEdit_VolumeMute.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_volumemute_enabled = HKEdit_VolumeMute.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            };
            HKEdit_VolumeMute.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_volumemute_enabled = HKEdit_VolumeMute.HotkeyIsEnabled;
                Properties.Settings.Default.hk_volumemute = HKEdit_VolumeMute.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };


            ComboBox_ProcessSelector.Text = Properties.Settings.Default.ProcessName;
            volume_step.Value = Properties.Settings.Default.VolumeStep;
            bool minimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            checkbox_minimizeOnStartup.Checked = minimizeOnStartup;

            UpdateHotkeys();

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

        #endregion FormComponents
    }
}