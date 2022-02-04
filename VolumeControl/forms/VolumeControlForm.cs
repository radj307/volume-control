using Common;

namespace VolumeControl
{
    public partial class VolumeControlForm : Form
    {
        #region Members

        /// <summary>
        /// Hotkey definition linked to volume up actions.
        /// </summary>
        private Hotkey hk_up;
        private bool enable_up;
        /// <summary>
        /// Hotkey definition linked to volume down actions.
        /// </summary>
        private Hotkey hk_down;
        private bool enable_down;
        /// <summary>
        /// Hotkey definition linked to toggle mute actions.
        /// </summary>
        private Hotkey hk_mute;
        private bool enable_mute;

        #endregion Members

        #region HelperMethods

        /// <summary>
        /// Register Hotkeys
        /// </summary>
        internal void RegisterHotkeys()
        {
            if (hk_up != null && enable_up && !hk_up.Registered)
                hk_up.Register(this);
            if (hk_down != null && enable_down && !hk_down.Registered)
                hk_down.Register(this);
            if (hk_mute != null && enable_mute && !hk_mute.Registered)
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

        /// <summary>
        /// Initializes the hotkeys and assign delegate functions that use settings from the WinForms settings structure.
        /// </summary>
        private void InitializeHotkeys()
        {
            // set hotkeys
            hk_up = TryInitHotkey(Properties.Settings.Default.hk_volumeup, new(Keys.VolumeUp, false, true, false, false));
            hk_up.Pressed += delegate { VolumeHelper.IncrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); };

            hk_down = TryInitHotkey(Properties.Settings.Default.hk_volumedown, new(Keys.VolumeDown, false, true, false, false));
            hk_down.Pressed += delegate { VolumeHelper.DecrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); };

            hk_mute = TryInitHotkey(Properties.Settings.Default.hk_volumemute, new(Keys.VolumeMute, false, true, false, false));
            hk_mute.Pressed += delegate { VolumeHelper.ToggleMute(Properties.Settings.Default.ProcessName); };

            // register hotkeys
            hk_up.Register(this);
            hk_down.Register(this);
            hk_mute.Register(this);
        }

        private static Hotkey TryInitHotkey(string hkString, Hotkey def)
        {
            Hotkey hk;
            try
            {
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
            string title = "Volume Control";
            if (process_name.Text.Length > 0)
                title += $"  ({process_name.Text})";
            Text = title;
        }

        #endregion HelperMethods

        #region ClassFunctions

        /// <summary>
        /// Constructor
        /// </summary>
        public VolumeControlForm()
        {
            InitializeComponent();

            // This needs to be done first, otherwise the window is destroyed & recreated which unbinds all of the hotkeys.
            ShowInTaskbar = false;

            // set all hotkeys to null to quiet compiler
            hk_up = null!;
            hk_down = null!;
            hk_mute = null!;
            // initialize hotkeys
            InitializeHotkeys();
            enable_up = Properties.Settings.Default.enable_volumeup;
            enable_down = Properties.Settings.Default.enable_volumedown;
            enable_mute = Properties.Settings.Default.enable_volumemute;

            hkedit_volumeup.Hotkey = hk_up;
            hkedit_volumeup.Text = "Volume Up";
            hkedit_volumeup.Enabled = true;

            hkedit_volumedown.Hotkey = hk_down;
            hkedit_volumedown.Text = "Volume Down";
            hkedit_volumedown.Enabled = true;

            hkedit_volumemute.Hotkey = hk_mute;
            hkedit_volumemute.Text = "Volume Mute";
            hkedit_volumemute.Enabled = true;

            Resize += delegate
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    Visible = false;
                }
            };

            // populate settings boxes
            checkbox_enabled.Checked = Properties.Settings.Default.Enabled;
            process_name.Text = Properties.Settings.Default.ProcessName;
            volume_step.Value = Properties.Settings.Default.VolumeStep;

            UpdateTitle();
        }

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
        /// Handler for the "Enabled" checkbox. When checked, it enables the hotkeys, when unchecked, it disables them.
        /// </summary>
        private void checkbox_enabled_event(object sender, EventArgs e)
        {
            Properties.Settings.Default.Enabled = checkbox_enabled.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            if (Properties.Settings.Default.Enabled)
                RegisterHotkeys();
            else
                UnregisterHotkeys();
        }
        /// <summary>
        /// Automatically called when the value of process_name is changed.
        /// Sets the settings value "ProcessName" to the new value, and updates the window title.
        /// </summary>
        private void process_name_event(object sender, EventArgs e)
        {
            Properties.Settings.Default.ProcessName = process_name.Text;
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

        private void notifyIcon1_event(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                Visible = true;
            }
        }

        private void hkedit_volumeup_update(object sender, EventArgs e)
        {
            hk_up.Unregister();
            hk_up = this.hkedit_volumeup.Hotkey;
            hk_up.Register(this);
            enable_up = this.hkedit_volumeup.Enabled;

            Properties.Settings.Default.hk_volumeup = hk_up.ToString();
            Properties.Settings.Default.enable_volumeup = enable_up;

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        private void hkedit_volumedown_update(object sender, EventArgs e)
        {
            hk_down.Unregister();
            hk_down = this.hkedit_volumedown.Hotkey;
            hk_down.Register(this);
            enable_down = this.hkedit_volumedown.Enabled;

            Properties.Settings.Default.hk_volumedown = hk_down.ToString();
            Properties.Settings.Default.enable_volumedown = enable_down;
            
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        private void hkedit_volumemute_update(object sender, EventArgs e)
        {
            hk_mute.Unregister();
            hk_mute = this.hkedit_volumemute.Hotkey;
            hk_mute.Register(this);
            enable_mute = this.hkedit_volumemute.Enabled;

            Properties.Settings.Default.hk_volumemute = hk_mute.ToString();
            Properties.Settings.Default.enable_volumemute = enable_mute;

            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        private void button_refresh_Click(object sender, EventArgs e)
        {
            hk_up = new(Properties.Settings.Default.hk_volumeup);
            enable_up = Properties.Settings.Default.enable_volumeup;
            hk_down = new(Properties.Settings.Default.hk_volumedown);
            enable_down = Properties.Settings.Default.enable_volumedown;
            hk_mute = new(Properties.Settings.Default.hk_volumemute);
            enable_mute = Properties.Settings.Default.enable_volumemute;
            checkbox_enabled.Checked = Properties.Settings.Default.Enabled;
            process_name.Text = Properties.Settings.Default.ProcessName;
            volume_step.Value = Properties.Settings.Default.VolumeStep;
        }

        #endregion FormComponents
    }
}