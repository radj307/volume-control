using AudioAPI;
using HotkeyLib;
using System.ComponentModel;

namespace VolumeControl
{
    public partial class VolumeControlForm : Form
    {
        #region Members

        /// <summary>
        /// Hotkey definition linked to the volume up action.
        /// Cannot be redefined, use the Reset() method to modify in-place.
        /// </summary>
        private readonly Hotkey hk_up;
        /// <summary>
        /// Hotkey definition linked to the volume down action.
        /// Cannot be redefined, use the Reset() method to modify in-place.
        /// </summary>
        private readonly Hotkey hk_down;
        /// <summary>
        /// Hotkey definition linked to the toggle mute action.
        /// Cannot be redefined, use the Reset() method to modify in-place.
        /// </summary>
        private readonly Hotkey hk_mute;
        /// <summary>
        /// Utility used to enumerate audio sessions to populate the Process selector box.
        /// </summary>
        private BindingList<string> sessions = new();
        /// <summary>
        /// Binding source for the audio session list / process list.
        /// </summary>
        private readonly BindingSource binding;

        #endregion Members

        #region Properties

        /// <summary>
        /// Overrides the base object's Visible member with a property that respects hotkeys.
        /// This is required because changing the value of "Visible" causes the window to be reconstructed, removing any hotkey registrations.
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
        /// This is required because changing the value of "ShowInTaskbar" causes the window to be reconstructed, removing any hotkey registrations.
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
        /// <summary>
        /// Check if the Volume Up hotkey is enabled.
        /// </summary>
        private bool VolumeUpHotkeyIsEnabled { get => HKEdit_VolumeUp.HotkeyIsEnabled; }
        /// <summary>
        /// Check if the Volume Down hotkey is enabled.
        /// </summary>
        private bool VolumeDownHotkeyIsEnabled { get => HKEdit_VolumeDown.HotkeyIsEnabled; }
        /// <summary>
        /// Check if the Volume Mute hotkey is enabled.
        /// </summary>
        private bool VolumeMuteHotkeyIsEnabled { get => HKEdit_VolumeMute.HotkeyIsEnabled; }

        #endregion Properties

        #region HelperMethods

        /// <summary>
        /// Register all enabled Hotkeys
        /// </summary>
        internal void RegisterHotkeys()
        {
            if (VolumeUpHotkeyIsEnabled && !hk_up.Registered)
            {
                hk_up.Register(this);
            }

            if (VolumeDownHotkeyIsEnabled && !hk_down.Registered)
            {
                hk_down.Register(this);
            }

            if (VolumeMuteHotkeyIsEnabled && !hk_mute.Registered)
            {
                hk_mute.Register(this);
            }
        }
        /// <summary>
        /// Unregister all Hotkeys
        /// </summary>
        internal void UnregisterHotkeys()
        {
            if (hk_up.Registered)
                hk_up.Unregister();

            if (hk_down.Registered)
                hk_down.Unregister();

            if (hk_mute.Registered)
                hk_mute.Unregister();
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
            sessions = AudioSessionList.GetProcessNames();
            ComboBox_ProcessSelector.DataSource = sessions;
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

        public VolumeControlForm()
        {
            InitializeComponent();

            // INITIALIZE HOTKEYS
            hk_up = new(Properties.Settings.Default.hk_volumeup, delegate { VolumeHelper.IncrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); });
            hk_down = new(Properties.Settings.Default.hk_volumedown, delegate { VolumeHelper.DecrementVolume(Properties.Settings.Default.ProcessName, Properties.Settings.Default.VolumeStep); });
            hk_mute = new(Properties.Settings.Default.hk_volumemute, delegate { VolumeHelper.ToggleMute(Properties.Settings.Default.ProcessName); });

            // INITIALIZE HOTKEY EDITORS
            // VOLUME UP
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
            // VOLUME DOWN
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
            // VOLUME MUTE
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

            // INITIALIZE UI COMPONENTS
            // VOLUME STEP
            Numeric_VolumeStep.Value = Properties.Settings.Default.VolumeStep;
            // PROCESS SELECTOR
            string proc = Properties.Settings.Default.ProcessName;
            binding = new();
            binding.DataSource = sessions;
            ComboBox_ProcessSelector.AutoCompleteSource = AutoCompleteSource.CustomSource;
            ComboBox_ProcessSelector.AutoCompleteCustomSource = new()
            {
                proc //< Always include setting value by default so it isn't overwritten if the process isn't active.
            };
            UpdateProcessList(); // update sessions list
            ComboBox_ProcessSelector.AutoCompleteCustomSource.AddRange(sessions.ToArray());
            ComboBox_ProcessSelector.DataSource = binding;
            ComboBox_ProcessSelector.Text = proc;
            // RUN ON STARTUP
            CheckBox_RunOnStartup.Checked = Properties.Settings.Default.RunOnStartup;
            // VISIBLE IN TASKBAR
            ShowInTaskbar = CheckBox_VisibleInTaskbar.Checked = Properties.Settings.Default.VisibleInTaskbar;
            // MINIMIZE ON STARTUP
            bool minimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            checkbox_minimizeOnStartup.Checked = minimizeOnStartup;
            if (minimizeOnStartup)
                WindowState = FormWindowState.Minimized;
            // VERSION NUMBER
            Label_VersionNumber.Text = "v3.0.0";

            UpdateHotkeys();
            UpdateTitle();
        }
        ~VolumeControlForm()
        {
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Upgrade();
            UnregisterHotkeys();
        }

        #endregion ClassFunctions

        #region FormComponents

        /// <summary>
        /// Automatically called when the value of "ComboBox_ProcessSelector.Text" is changed.
        /// Sets the settings value "ProcessName" to the new value, and updates the window title.
        /// </summary>
        private void ComboBox_ProcessName_TextChanged(object sender, EventArgs e)
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
        private void Numeric_VolumeStep_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.VolumeStep = Convert.ToDecimal(Numeric_VolumeStep.Value);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the system tray icon is double-clicked
        /// </summary>
        private void SystemTray_DoubleClick(object sender, MouseEventArgs e)
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
        private void SystemTray_ContextMenu_Close(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// Called when the "Minimize on Startup" checkbox is changed.
        /// </summary>
        private void Checkbox_MinimizeOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeOnStartup = checkbox_minimizeOnStartup.Checked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called when the window is focused by the user.
        /// </summary>
        private void Window_GotFocus(object sender, EventArgs e)
        {
            UpdateProcessList();
        }
        /// <summary>
        /// Called when the Reload button is pressed.
        /// </summary>
        private void Button_ReloadProcessList_Click(object sender, EventArgs e)
        {
            UpdateProcessList();
        }
        /// <summary>
        /// Called when the window is maximized, minimized, or resized.
        /// </summary>
        private void Form_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Visible = false;
            }
        }
        private void CheckBox_RunOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = CheckBox_RunOnStartup.Checked;
            if (isChecked)
                RegAPI.EnableRunOnStartup();
            else
                RegAPI.DisableRunOnStartup();
            Properties.Settings.Default.RunOnStartup = isChecked;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        private void CheckBox_VisibleInTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            UnregisterHotkeys();
            bool isChecked = CheckBox_VisibleInTaskbar.Checked;
            if (isChecked != Properties.Settings.Default.VisibleInTaskbar)
            {
                // Set the value of "ShowInTaskbar", automatically re-registers hotkeys using property override
                this.ShowInTaskbar = Properties.Settings.Default.VisibleInTaskbar = isChecked;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            RegisterHotkeys();
        }

        #endregion FormComponents

    }
}