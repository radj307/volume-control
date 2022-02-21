using AudioAPI;
using HotkeyLib;
using System.ComponentModel;
using System.Reflection;
using UIComposites;

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
        /// Hotkey definition linked to the Next Song action.
        /// </summary>
        private readonly Hotkey hk_next;
        /// <summary>
        /// Hotkey definition linked to the Previous Song action
        /// </summary>
        private readonly Hotkey hk_prev;
        /// <summary>
        /// Hotkey definition linked to the Play/Pause action.
        /// </summary>
        private readonly Hotkey hk_playback;
        /// <summary>
        /// Hotkey definition linked to the Next Target action.
        /// </summary>
        private readonly Hotkey hk_nextTarget;
        /// <summary>
        /// Hotkey definition linked to the Previous Target action.
        /// </summary>
        private readonly Hotkey hk_prevTarget;
        /// <summary>
        /// Hotkey definition linked to the Show Targets List action
        /// </summary>
        private readonly Hotkey hk_showTarget;
        /// <summary>
        /// Utility used to enumerate audio sessions to populate the Process selector box.
        /// </summary>
        private BindingList<string> sessions = new();

        private readonly ToastForm targetListForm = new();

        private readonly CancelButtonHandler cancelHandler = new();

        private bool triggerTargetRefresh = false;

        #endregion Members

        #region Properties

        private int TargetListSize => TargetSelector.ListSize;
        private int CurrentTargetIndex
        {
            get => TargetSelector.SelectedIndex;
            set => TargetSelector.SelectedIndex = value;
        }
        private string CurrentTargetName
        {
            get => TargetSelector.Text;
            set => TargetSelector.Text = value;
        }
        private bool TargetListEnabled
        {
            get => TgtSettings.TargetListEnabled;
            set => TgtSettings.TargetListEnabled = value;
        }

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
        private bool VolumeUpHotkeyIsEnabled => HKEdit_VolumeUp.HotkeyIsEnabled;
        /// <summary>
        /// Check if the Volume Down hotkey is enabled.
        /// </summary>
        private bool VolumeDownHotkeyIsEnabled => HKEdit_VolumeDown.HotkeyIsEnabled;
        /// <summary>
        /// Check if the Volume Mute hotkey is enabled.
        /// </summary>
        private bool VolumeMuteHotkeyIsEnabled => HKEdit_VolumeMute.HotkeyIsEnabled;

        private bool NextHotkeyIsEnabled => HKEdit_Next.HotkeyIsEnabled;

        private bool PrevHotkeyIsEnabled => HKEdit_Prev.HotkeyIsEnabled;
        private bool PlaybackHotkeyIsEnabled => HKEdit_TogglePlayback.HotkeyIsEnabled;
        private bool NextTargetHotkeyIsEnabled => HKEdit_NextTarget.HotkeyIsEnabled;
        private bool PrevTargetHotkeyIsEnabled => HKEdit_PrevTarget.HotkeyIsEnabled;
        private bool ShowTargetHotkeyIsEnabled => HKEdit_ShowTarget.HotkeyIsEnabled;

        private bool TargetListVisible => targetListForm.Visible;

        #endregion Properties

        #region HelperMethods

        /// <summary>
        /// Register all enabled Hotkeys
        /// </summary>
        internal void RegisterHotkeys()
        {
            if (VolumeUpHotkeyIsEnabled && !hk_up.Registered)
                hk_up.Register(this);

            if (VolumeDownHotkeyIsEnabled && !hk_down.Registered)
                hk_down.Register(this);

            if (VolumeMuteHotkeyIsEnabled && !hk_mute.Registered)
                hk_mute.Register(this);

            if (NextHotkeyIsEnabled && !hk_next.Registered)
                hk_next.Register(this);

            if (PrevHotkeyIsEnabled && !hk_prev.Registered)
                hk_prev.Register(this);

            if (PlaybackHotkeyIsEnabled && !hk_playback.Registered)
                hk_playback.Register(this);

            if (NextTargetHotkeyIsEnabled && !hk_nextTarget.Registered)
                hk_nextTarget.Register(this);

            if (PrevTargetHotkeyIsEnabled && !hk_prevTarget.Registered)
                hk_prevTarget.Register(this);

            if (ShowTargetHotkeyIsEnabled && !hk_showTarget.Registered)
                hk_showTarget.Register(this);
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

            if (hk_next.Registered)
                hk_next.Unregister();

            if (hk_prev.Registered)
                hk_prev.Unregister();

            if (hk_playback.Registered)
                hk_playback.Unregister();

            if (hk_nextTarget.Registered)
                hk_nextTarget.Unregister();

            if (hk_prevTarget.Registered)
                hk_prevTarget.Unregister();

            if (hk_showTarget.Registered)
                hk_showTarget.Unregister();
        }
        /// <summary>
        /// Update the window title of the main window & the target list form using the current target.
        /// </summary>
        private void UpdateTitle()
        {
            string currentTarget = CurrentTargetName;
            if (currentTarget.Length > 0)
                Text = $"{currentTarget} Volume Controller";
            else
                Text = "Volume Control";
        }
        /// <summary>
        /// Updates the options available in the Process selector box & the target list form.
        /// </summary>
        internal void UpdateProcessList()
        {
            // get a list of all active audio sessions (applications that are outputting audio)
            List<string> active = ProcessNameList.GetProcessNames();

            string currentName = CurrentTargetName;
            bool isBlank = currentName.Length <= 0;

            if (!active.Contains(currentName) && !isBlank)
            {
                active.Add(currentName);
            }
            // sort the list (unsure of how useful this is)
            active.Sort();

            bool tgtListEnabled = TargetListEnabled;
            // update the target list window:
            if (tgtListEnabled)
            {
                targetListForm.FlushItems();
                targetListForm.LoadItems(active);
            }
            // set the current list
            TargetSelector.DataSource = sessions = ToBindingList(active);

            if (!isBlank)
                CurrentTargetName = currentName;

            currentName = CurrentTargetName;

            if (tgtListEnabled)
                targetListForm.Selected = currentName; //< don't use the previously set current name in case it changed
        }

        private static BindingList<T> ToBindingList<T>(List<T> list)
        {
            BindingList<T> bindingList = new();
            foreach (T item in list)
                bindingList.Add(item);
            return bindingList;
        }

        private (float, bool)? GetTargetVolume(string? target = null)
        {
            AudioAPI.WindowsAPI.Audio.ISimpleAudioVolume? vol = VolumeHelper.GetVolumeObject(target ?? CurrentTargetName);
            if (vol == null)
                return null;
            vol.GetMute(out bool isMuted);
            vol.GetMasterVolume(out float volume);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(vol);
            return (volume, isMuted);
        }

        private void UpdateStatusImage(string? target = null)
        {
            var volume = GetTargetVolume(target);
            if (volume == null)
                return;
            targetListForm.UpdateActiveStateImage(volume.Value.Item1, volume.Value.Item2);
        }

        private void UpdateHotkeys()
        {
            UnregisterHotkeys();

            hk_up.Reset(Properties.Settings.Default.hk_volumeup);
            hk_down.Reset(Properties.Settings.Default.hk_volumedown);
            hk_mute.Reset(Properties.Settings.Default.hk_volumemute);
            hk_next.Reset(Properties.Settings.Default.hk_next);
            hk_prev.Reset(Properties.Settings.Default.hk_prev);
            hk_playback.Reset(Properties.Settings.Default.hk_playback);
            hk_nextTarget.Reset(Properties.Settings.Default.hk_nextTarget);
            hk_prevTarget.Reset(Properties.Settings.Default.hk_prevTarget);
            hk_showTarget.Reset(Properties.Settings.Default.hk_showTarget);

            RegisterHotkeys();
        }

        /// <summary>
        /// Set the current target selection to a specific entry.
        /// </summary>
        /// <param name="name">The target's name.</param>
        /// <param name="addIfMissing">When true, adds the target name if it doesn't already exist.</param>
        private void SetTarget(string name, bool addIfMissing = true)
        {
            int index = TargetSelector.IndexOf(name);
            if (index != -1)
                CurrentTargetIndex = index;
            else if (addIfMissing && name.Length > 0)
            {
                sessions.Add(name);
                CurrentTargetIndex = TargetListSize - 1;
            }
        }

        /// <summary>
        /// Increment the current target indexer.
        /// </summary>
        private void NextTarget(object? sender, EventArgs e)
        {
            if (triggerTargetRefresh)
            {
                triggerTargetRefresh = false;
                UpdateProcessList();
                TargetRefreshTimer.Enabled = true;
            }

            if (CurrentTargetIndex + 1 < TargetListSize)
                ++CurrentTargetIndex;
            else
                CurrentTargetIndex = 0;

            Properties.Settings.Default.LastTarget = targetListForm.Selected = CurrentTargetName;

            if (TargetListEnabled)
            {
                UpdateStatusImage();
                targetListForm.Show(true);
            }
        }

        /// <summary>
        /// Decrement the current target indexer.
        /// </summary>
        private void PrevTarget(object? sender, EventArgs e)
        {
            if (triggerTargetRefresh)
            {
                triggerTargetRefresh = false;
                UpdateProcessList();
                TargetRefreshTimer.Enabled = true;
            }

            if (CurrentTargetIndex - 1 >= 0)
                --CurrentTargetIndex;
            else
                CurrentTargetIndex = TargetListSize - 1;

            Properties.Settings.Default.LastTarget = targetListForm.Selected = CurrentTargetName;

            if (TargetListEnabled)
            {
                UpdateStatusImage();
                targetListForm.Show(true);
            }
        }

        /// <summary>
        /// Toggle the target list window.
        /// </summary>
        private void ShowTarget(object? sender, EventArgs e)
        {
            if (targetListForm.Visible)
            {
                targetListForm.Hide();
                targetListForm.ForceShow = false;
            }
            else
            {
                UpdateProcessList();
                targetListForm.ForceShow = true;
                targetListForm.Show();
            }
        }

        private void IncreaseVolume(object? sender, EventArgs e)
        {
            VolumeHelper.IncrementVolume(CurrentTargetName, Properties.Settings.Default.VolumeStep);
            if (TargetListVisible)
            {
                UpdateStatusImage(CurrentTargetName);
                targetListForm.Refresh();
            }
        }
        private void DecreaseVolume(object? sender, EventArgs e)
        {
            VolumeHelper.DecrementVolume(CurrentTargetName, Properties.Settings.Default.VolumeStep);
            if (TargetListVisible)
            {
                UpdateStatusImage(CurrentTargetName);
                targetListForm.Refresh();
            }
        }
        private void ToggleMute(object? sender, EventArgs e)
        {
            VolumeHelper.ToggleMute(CurrentTargetName);
            if (TargetListVisible)
            {
                UpdateStatusImage(CurrentTargetName);
                targetListForm.Refresh();
            }
        }

        /// <summary>
        /// Send a virtual key press event using the Win32 API.
        /// </summary>
        private static void SendKeyboardEvent(VirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1) => AudioAPI.WindowsAPI.User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);

        /// <summary>
        /// Saves and Reloads the Properties.Settings.Default object.
        /// </summary>
        private static void SaveSettings(object? sender, EventArgs e)
        {
            Properties.Settings.Default.PropertyChanged -= SaveSettings; // don't recurse
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            Properties.Settings.Default.PropertyChanged += SaveSettings;
        }

        #endregion HelperMethods

        #region ClassFunctions

        public VolumeControlForm()
        {
            string lastTarget = Properties.Settings.Default.LastTarget;

            InitializeComponent();

            #region InitializeHotkeys

            // INITIALIZE VOLUME HOTKEYS
            hk_up = new(Properties.Settings.Default.hk_volumeup, IncreaseVolume);
            hk_down = new(Properties.Settings.Default.hk_volumedown, DecreaseVolume);
            hk_mute = new(Properties.Settings.Default.hk_volumemute, ToggleMute);
            // INITIALIZE PLAYBACK HOTKEYS
            hk_next = new(Properties.Settings.Default.hk_next, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_NEXT_TRACK); });
            hk_prev = new(Properties.Settings.Default.hk_prev, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PREV_TRACK); });
            hk_playback = new(Properties.Settings.Default.hk_playback, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PLAY_PAUSE); });
            // INITIALIZE TARGET HOTKEYS
            hk_nextTarget = new(Properties.Settings.Default.hk_nextTarget, NextTarget);
            hk_prevTarget = new(Properties.Settings.Default.hk_prevTarget, PrevTarget);
            hk_showTarget = new(Properties.Settings.Default.hk_showTarget, ShowTarget);

            // INITIALIZE VOLUME HOTKEY EDITORS
            // VOLUME UP
            HKEdit_VolumeUp.Hotkey = hk_up;
            HKEdit_VolumeUp.SetLabel("Volume Up");
            HKEdit_VolumeUp.SetHotkeyIsEnabled(Properties.Settings.Default.hk_volumeup_enabled);
            HKEdit_VolumeUp.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_volumeup_enabled = HKEdit_VolumeUp.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
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
                UpdateHotkeys();
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
                UpdateHotkeys();
            };
            HKEdit_VolumeMute.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_volumemute_enabled = HKEdit_VolumeMute.HotkeyIsEnabled;
                Properties.Settings.Default.hk_volumemute = HKEdit_VolumeMute.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // INITIALIZE PLAYBACK HOTKEY EDITORS
            // NEXT SONG
            HKEdit_Next.Hotkey = hk_next;
            HKEdit_Next.SetLabel("Next Song");
            HKEdit_Next.SetHotkeyIsEnabled(Properties.Settings.Default.hk_next_enabled);
            HKEdit_Next.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_next_enabled = HKEdit_Next.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_Next.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_next_enabled = HKEdit_Next.HotkeyIsEnabled;
                Properties.Settings.Default.hk_next = HKEdit_Next.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // PREVIOUS SONG
            HKEdit_Prev.Hotkey = hk_prev;
            HKEdit_Prev.SetLabel("Previous Song");
            HKEdit_Prev.SetHotkeyIsEnabled(Properties.Settings.Default.hk_prev_enabled);
            HKEdit_Prev.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_prev_enabled = HKEdit_Prev.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_Prev.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_prev_enabled = HKEdit_Prev.HotkeyIsEnabled;
                Properties.Settings.Default.hk_prev = HKEdit_Prev.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // TOGGLE PLAYBACK
            HKEdit_TogglePlayback.Hotkey = hk_playback;
            HKEdit_TogglePlayback.SetLabel("Play / Pause");
            HKEdit_TogglePlayback.SetHotkeyIsEnabled(Properties.Settings.Default.hk_next_enabled);
            HKEdit_TogglePlayback.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_playback_enabled = HKEdit_TogglePlayback.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_TogglePlayback.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_playback_enabled = HKEdit_TogglePlayback.HotkeyIsEnabled;
                Properties.Settings.Default.hk_playback = HKEdit_TogglePlayback.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // INITIALIZE TARGET SELECTION HOTKEYS
            // NEXT TARGET
            HKEdit_NextTarget.Hotkey = hk_nextTarget;
            HKEdit_NextTarget.SetLabel("Next Target");
            HKEdit_NextTarget.SetHotkeyIsEnabled(Properties.Settings.Default.hk_nextTarget_enabled);
            HKEdit_NextTarget.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_nextTarget_enabled = HKEdit_NextTarget.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_NextTarget.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_nextTarget_enabled = HKEdit_NextTarget.HotkeyIsEnabled;
                Properties.Settings.Default.hk_nextTarget = HKEdit_NextTarget.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // PREV TARGET
            HKEdit_PrevTarget.Hotkey = hk_prevTarget;
            HKEdit_PrevTarget.SetLabel("Prev Target");
            HKEdit_PrevTarget.SetHotkeyIsEnabled(Properties.Settings.Default.hk_prevTarget_enabled);
            HKEdit_PrevTarget.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_prevTarget_enabled = HKEdit_PrevTarget.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_PrevTarget.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_prevTarget_enabled = HKEdit_PrevTarget.HotkeyIsEnabled;
                Properties.Settings.Default.hk_prevTarget = HKEdit_PrevTarget.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            // SHOW TARGET
            HKEdit_ShowTarget.Hotkey = hk_showTarget;
            HKEdit_ShowTarget.SetLabel("Show Target");
            HKEdit_ShowTarget.SetHotkeyIsEnabled(Properties.Settings.Default.hk_showTarget_enabled);
            HKEdit_ShowTarget.Checkbox_Enabled.CheckedChanged += delegate
            {
                Properties.Settings.Default.hk_showTarget_enabled = HKEdit_ShowTarget.HotkeyIsEnabled;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };
            HKEdit_ShowTarget.ModifierChanged += delegate
            {
                Properties.Settings.Default.hk_showTarget_enabled = HKEdit_ShowTarget.HotkeyIsEnabled;
                Properties.Settings.Default.hk_showTarget = HKEdit_ShowTarget.Hotkey.ToString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                UpdateHotkeys();
            };

            #endregion InitializeHotkeys

            // INITIALIZE UI COMPONENTS
            // VERSION NUMBER
            Version currentVersion = typeof(VolumeControlForm).Assembly.GetName().Version!;
            if (Convert.ToBoolean(typeof(VolumeControlForm).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease))
                Label_VersionNumber.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}";
            else
                Label_VersionNumber.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}";
            // CANCEL BUTTON HANDLER (ESC)
            cancelHandler.Action += delegate { WindowState = FormWindowState.Minimized; };
            CancelButton = cancelHandler;
            // PROCESS SELECTOR
            TargetSelector.TextChanged -= TargetComboBox_TextChanged;
            UpdateProcessList(); // update the process list
            TargetSelector.Text = lastTarget;
            TargetSelector.TextChanged += TargetComboBox_TextChanged;
            // TARGET LIST FORM
            targetListForm.Resize += delegate { if (targetListForm.WindowState != FormWindowState.Minimized) UpdateProcessList(); }; // triggers when window is shown
            targetListForm.SelectionChanged += delegate //< Triggered when the user selects a process in the target list
            {
                SetTarget(targetListForm.Selected);
            };
            // SETTINGS
            Settings.VolumeStep = Properties.Settings.Default.VolumeStep;
            Settings.RunAtStartup = Properties.Settings.Default.RunOnStartup;
            Settings.MinimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            Settings.ShowInTaskbar = ShowInTaskbar = Properties.Settings.Default.VisibleInTaskbar;
            Settings.AlwaysOnTop = TopMost = Properties.Settings.Default.AlwaysOnTop;
            Settings.DarkModeChanged -= Settings_DarkModeChanged;
            Settings.EnableDarkMode = Properties.Settings.Default.EnableDarkMode;
            Settings.DarkModeChanged += Settings_DarkModeChanged;
            // TARGET LIST SETTINGS
            TgtSettings.TargetListEnabled = TargetListEnabled = Properties.Settings.Default.tgtlist_enabled;
            TgtSettings.TargetListTimeout = targetListForm.Timeout = Properties.Settings.Default.tgtlist_timeout;
            TgtSettings.DarkModeChanged -= TgtSettings_DarkModeChanged;
            TgtSettings.EnableDarkMode = Properties.Settings.Default.EnableToastDarkMode;
            TgtSettings.DarkModeChanged += TgtSettings_DarkModeChanged;

            // Apply theme settings
            Settings_DarkModeChanged(this, EventArgs.Empty);
            TgtSettings_DarkModeChanged(this, EventArgs.Empty);

            // Set a save event to trigger when properties change
            Properties.Settings.Default.PropertyChanged += SaveSettings;

            UpdateHotkeys();
            UpdateTitle();

            if (Settings.MinimizeOnStartup)
            {
                WindowState = FormWindowState.Minimized;
                Visible = false;
            }
        }
        ~VolumeControlForm()
        {
            Properties.Settings.Default.Save();
            UnregisterHotkeys();
        }

        #endregion ClassFunctions

        #region FormComponents

        /// <summary>
        /// Automatically called when the value of "ComboBox_ProcessSelector.Text" is changed.
        /// Sets the settings value "ProcessName" to the new value, and updates the window title.
        /// </summary>
        private void TargetComboBox_TextChanged(object? sender, EventArgs e)
        {
            Properties.Settings.Default.LastTarget = TargetSelector.Text;
            UpdateTitle();
        }
        /// <summary>
        /// Automatically called when the value of volume_step is changed.
        /// Sets the settings value "VolumeStep" to the new value.
        /// </summary>
        private void VolumeStep_Changed(object sender, EventArgs e) => Properties.Settings.Default.VolumeStep = Settings.VolumeStep;
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
            // Always bring the window to the foreground
            Activate();
        }
        /// <summary>
        /// Called when the user clicks the close button in the system tray context menu
        /// </summary>
        private void SystemTray_ContextMenu_Close(object sender, EventArgs e) => Application.Exit();
        /// <summary>
        /// Called when the "Minimize on Startup" checkbox is changed.
        /// </summary>
        private void MinimizeOnStartup_Changed(object sender, EventArgs e) => Properties.Settings.Default.MinimizeOnStartup = Settings.MinimizeOnStartup;
        /// <summary>
        /// Called when the window is focused by the user.
        /// </summary>
        private void Window_GotFocus(object sender, EventArgs e) => UpdateProcessList();

        /// <summary>
        /// Called when the Reload button is pressed.
        /// </summary>
        private void Reload_Clicked(object sender, EventArgs e) => UpdateProcessList();

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
        private void RunOnStartup_Changed(object sender, EventArgs e)
        {
            bool isChecked = Settings.RunAtStartup;
            if (isChecked)
                RegAPI.EnableRunOnStartup();
            else
                RegAPI.DisableRunOnStartup();
            Properties.Settings.Default.RunOnStartup = isChecked;
        }
        private void VisibleInTaskbar_Changed(object sender, EventArgs e)
        {
            UnregisterHotkeys();
            bool isChecked = Settings.ShowInTaskbar;
            if (isChecked != Properties.Settings.Default.VisibleInTaskbar)
            {
                // Set the value of "ShowInTaskbar", automatically re-registers hotkeys using property override
                ShowInTaskbar = Properties.Settings.Default.VisibleInTaskbar = isChecked;
            }
            RegisterHotkeys();
        }
        private void ToastEnabled_Changed(object sender, EventArgs e) => Properties.Settings.Default.tgtlist_enabled = targetListForm.TimeoutEnabled = TargetListEnabled;

        private void ToastTimeout_Changed(object sender, EventArgs e) => Properties.Settings.Default.tgtlist_timeout = targetListForm.Timeout = TgtSettings.TargetListTimeout;

        private void AlwaysOnTop_Changed(object sender, EventArgs e) => Properties.Settings.Default.AlwaysOnTop = Settings.AlwaysOnTop;

        private void TargetRefreshTimer_Tick(object sender, EventArgs e)
        {
            TargetRefreshTimer.Enabled = false;
            triggerTargetRefresh = true;
        }
        private void TgtSettings_DarkModeChanged(object? sender, EventArgs e)
        {
            bool enabled = TgtSettings.EnableDarkMode;
            if (enabled)
            {
                ColorScheme.DarkMode.ApplyTo(targetListForm.Controls);
                targetListForm.ForeColor = ColorScheme.DarkMode.Default.GetForeColor();
                targetListForm.BackColor = ColorScheme.DarkMode.Default.GetBackColor();
            }
            else
            {
                ColorScheme.LightMode.ApplyTo(targetListForm.Controls);
                targetListForm.ForeColor = ColorScheme.LightMode.Default.GetForeColor();
                targetListForm.BackColor = ColorScheme.LightMode.Default.GetBackColor();
            }
            Properties.Settings.Default.EnableToastDarkMode = TgtSettings.EnableDarkMode;
        }

        private void Settings_DarkModeChanged(object? sender, EventArgs e)
        {
            bool enabled = Settings.EnableDarkMode;
            if (enabled)
            {
                ColorScheme.DarkMode.ApplyTo(Controls);
                ForeColor = ColorScheme.DarkMode.Default.GetForeColor();
                BackColor = ColorScheme.DarkMode.Default.GetBackColor();
            }
            else
            {
                ColorScheme.LightMode.ApplyTo(Controls);
                ForeColor = ColorScheme.LightMode.Default.GetForeColor();
                BackColor = ColorScheme.LightMode.Default.GetBackColor();
            }
            Properties.Settings.Default.EnableDarkMode = enabled;
        }

        #endregion FormComponents
    }
}