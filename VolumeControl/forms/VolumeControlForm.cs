using MovablePython;

namespace VolumeControl
{
    public partial class VolumeControlForm : Form
    {
        #region Members

        /// <summary>
        /// Hotkey definition linked to volume up actions.
        /// </summary>
        private readonly Hotkey hk_up;
        /// <summary>
        /// Hotkey definition linked to volume down actions.
        /// </summary>
        private readonly Hotkey hk_down;
        /// <summary>
        /// Hotkey definition linked to toggle mute actions.
        /// </summary>
        private readonly Hotkey hk_mute;

        #endregion Members

        #region HelperMethods

        /// <summary>
        /// Register Hotkeys
        /// </summary>
        internal void EnableHotkeys()
        {
            if (!hk_up.Registered)
                hk_up.Register(this);
            if (!hk_down.Registered)
                hk_down.Register(this);
            if (!hk_mute.Registered)
                hk_mute.Register(this);
        }
        /// <summary>
        /// Unregister Hotkeys
        /// </summary>
        internal void DisableHotkeys()
        {
            if (hk_up.Registered)
                hk_up.Unregister();
            if (hk_down.Registered)
                hk_down.Unregister();
            if (hk_mute.Registered)
                hk_mute.Unregister();
        }

        /// <summary>
        /// Increments the target application's volume by the current step value.
        /// </summary>
        private void IncrementVolumeHandler()
        {
            VolumeHelper.IncrementVolume(process_name.Text, (float)volume_step.Value);
        }
        /// <summary>
        /// Decrements the target application's volume by the current step value.
        /// </summary>
        private void DecrementVolumeHandler()
        {
            VolumeHelper.DecrementVolume(process_name.Text, (float)volume_step.Value);
        }
        /// <summary>
        /// Toggles the target application's current mute state.
        /// </summary>
        private void MuteVolumeHandler()
        {
            VolumeHelper.ToggleMute(process_name.Text);
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

        #endregion HelperMethods

        #region ClassFunctions
        public VolumeControlForm()
        {
            InitializeComponent();

            // populate settings boxes
            checkbox_enabled.Checked = Properties.Settings.Default.Enabled;
            process_name.Text = Properties.Settings.Default.ProcessName;
            
            // set window title
            Text = $"Volume Control{(process_name.Text.Length > 0 ? $"  ({process_name.Text})" : "")}";

            // set hotkeys
            hk_up = TryInitHotkey(Properties.Settings.Default.hk_volumeup, new(Keys.VolumeUp, false, true, false, false));
            hk_up.Pressed += delegate { IncrementVolumeHandler(); };

            hk_down = TryInitHotkey(Properties.Settings.Default.hk_volumedown, new(Keys.VolumeDown, false, true, false, false));
            hk_down.Pressed += delegate { DecrementVolumeHandler(); };

            hk_mute = TryInitHotkey(Properties.Settings.Default.hk_volumemute, new(Keys.VolumeMute, false, true, false, false));
            hk_mute.Pressed += delegate { MuteVolumeHandler(); };

            hk_up.Register(this);
            hk_down.Register(this);
            hk_mute.Register(this);
        }
        ~VolumeControlForm()
        {
            Properties.Settings.Default.Save();
            DisableHotkeys();
        }
        #endregion ClassFunctions

        #region FormComponents
        /// <summary>
        /// Handler for the "Enabled" checkbox. When checked, it enables the hotkeys, when unchecked, it disables them.
        /// </summary>
        private void checkbox_enabled_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Enabled = checkbox_enabled.Checked;
            if (checkbox_enabled.Checked)
                EnableHotkeys();
            else
                DisableHotkeys();
        }
        #endregion FormComponents
    }
}