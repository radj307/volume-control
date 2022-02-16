using AudioAPI;
using HotkeyLib;
using System.ComponentModel;
using UIComposites;
using TargetListForm;
using System;
using System.Reflection;

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

        //        private NotificationForm notification = new();

        #endregion Members

        #region Properties

        private int TargetListSize
        {
            get => ComboBox_ProcessSelector.Items.Count;
        }
        private int CurrentTargetIndex
        {
            get => ComboBox_ProcessSelector.SelectedIndex;
            set => ComboBox_ProcessSelector.SelectedIndex = value;
        }
        private string CurrentTargetName
        {
            get => ComboBox_ProcessSelector.Text;
            set => ComboBox_ProcessSelector.Text = value;
        }
        private decimal CurrentTargetVolume
        {
            get
            {
                if (VolumeHelper.TryGetVolume(CurrentTargetName, out decimal vol))
                    return vol;
                return -1m;
            }
        }
        private bool CurrentTargetIsMuted
        {
            get
            {
                if (VolumeHelper.TryIsMuted(CurrentTargetName, out bool muted))
                    return muted;
                return true;
            }
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
        private bool VolumeUpHotkeyIsEnabled { get => HKEdit_VolumeUp.HotkeyIsEnabled; }
        /// <summary>
        /// Check if the Volume Down hotkey is enabled.
        /// </summary>
        private bool VolumeDownHotkeyIsEnabled { get => HKEdit_VolumeDown.HotkeyIsEnabled; }
        /// <summary>
        /// Check if the Volume Mute hotkey is enabled.
        /// </summary>
        private bool VolumeMuteHotkeyIsEnabled { get => HKEdit_VolumeMute.HotkeyIsEnabled; }

        private bool NextHotkeyIsEnabled
        {
            get => HKEdit_Next.HotkeyIsEnabled;
        }

        private bool PrevHotkeyIsEnabled { get => HKEdit_Prev.HotkeyIsEnabled; }
        private bool PlaybackHotkeyIsEnabled { get => HKEdit_TogglePlayback.HotkeyIsEnabled; }
        private bool NextTargetHotkeyIsEnabled { get => HKEdit_NextTarget.HotkeyIsEnabled; }
        private bool PrevTargetHotkeyIsEnabled { get => HKEdit_PrevTarget.HotkeyIsEnabled; }
        private bool ShowTargetHotkeyIsEnabled { get => HKEdit_ShowTarget.HotkeyIsEnabled; }

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
            {
                Text = $"{currentTarget} Volume Controller";
                //                targetListForm.SetTitle(currentTarget);
            }
            else
            {
                Text = "Volume Control";
            }
        }

        /// <summary>
        /// Updates the options available in the Process selector box & the target list form.
        /// </summary>
        internal void UpdateProcessList()
        {
            // get a list of all active audio sessions (applications that are outputting audio)
            List<string> active = AudioSessionList.GetProcessNames();

            string currentName = CurrentTargetName;
            bool isBlank = currentName.Length <= 0;

            if (!active.Contains(currentName) && !isBlank)
            {
                active.Add(currentName);
            }
            // sort the list (unsure of how useful this is)
            active.Sort();

            // update the target list window:
            targetListForm.FlushItems();
            targetListForm.LoadItems(active);
            // set the current list
            ComboBox_ProcessSelector.DataSource = sessions = ToBindingList(active);

            if (!isBlank)
            {
                CurrentTargetName = currentName;
            }

            targetListForm.Selected = CurrentTargetName; //< don't use the previously set current name in case it changed
        }

        private static BindingList<T> ToBindingList<T>(List<T> list)
        {
            BindingList<T> bindingList = new();
            foreach (T item in list)
                bindingList.Add(item);
            return bindingList;
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

        private Image SelectTargetImage(bool useWhiteImg)
        {
            decimal volume = CurrentTargetVolume;
            if (volume == -1m)
                return useWhiteImg ? Properties.Resources.target_null_white : Properties.Resources.target_null;
            else if (volume == 0m || CurrentTargetIsMuted)
                return useWhiteImg ? Properties.Resources.target_0_white : Properties.Resources.target_0;
            else if (volume <= 33m)
                return useWhiteImg ? Properties.Resources.target_1_white : Properties.Resources.target_1;
            else if (volume <= 66m)
                return useWhiteImg ? Properties.Resources.target_2_white : Properties.Resources.target_2;
            else
                return useWhiteImg ? Properties.Resources.target_3_white : Properties.Resources.target_3;
        }

        /// <summary>
        /// Set the current target selection to a specific entry.
        /// </summary>
        /// <param name="name">The target's name.</param>
        /// <param name="addIfMissing">When true, adds the target name if it doesn't already exist.</param>
        private void SetTarget(string name, bool addIfMissing = true)
        {
            int index = ComboBox_ProcessSelector.Items.IndexOf(name);
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
        private void NextTarget()
        {
            if (CurrentTargetIndex + 1 < TargetListSize)
                ++CurrentTargetIndex;
            else
                CurrentTargetIndex = 0;

            // if the main window is minimized, and the target list form is enabled.
            if (TargetListEnabled && WindowState == FormWindowState.Minimized)
            {
                targetListForm.Show(true, CurrentTargetName);
                //                notification.ShowNotification(CurrentTargetName, Properties.Settings.Default.tgtlist_timeout, SelectTargetImage(true), Color.DarkGray, Notify.GetAltColor(Color.DarkGray));
            }
            else if (targetListForm.WindowState != FormWindowState.Minimized)
            {
                targetListForm.Selected = CurrentTargetName;
            }

            Properties.Settings.Default.LastTarget = CurrentTargetName;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Decrement the current target indexer.
        /// </summary>
        private void PrevTarget()
        {
            if (CurrentTargetIndex - 1 >= 0)
                --CurrentTargetIndex;
            else
                CurrentTargetIndex = TargetListSize - 1;

            targetListForm.Selected = CurrentTargetName;

            if (TargetListEnabled && WindowState == FormWindowState.Minimized)
            {
                targetListForm.Show(true, CurrentTargetName);
                //                notification.ShowNotification(CurrentTargetName, Properties.Settings.Default.tgtlist_timeout, SelectTargetImage(true), Color.DarkGray, Notify.GetAltColor(Color.DarkGray));
            }
            else if (targetListForm.WindowState != FormWindowState.Minimized)
            {
                targetListForm.Selected = CurrentTargetName;
            }

            Properties.Settings.Default.LastTarget = ComboBox_ProcessSelector.SelectedValue?.ToString();
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }

        /// <summary>
        /// Toggle the target list window.
        /// </summary>
        private void ShowTarget()
        {
            //notification.ShowNotification(CurrentTargetName, Properties.Settings.Default.tgtlist_timeout, SelectTargetImage(true), Color.DarkGray, Notify.GetAltColor(Color.DarkGray));
            if (targetListForm.Visible)
                targetListForm.Hide();
            else
            {
                UpdateProcessList();
                targetListForm.Show();
            }
        }

        /// <summary>
        /// Send a virtual key press event using the Win32 API.
        /// </summary>
        private static void SendKeyboardEvent(VirtualKeyCode vk, byte scanCode = 0xAA, byte flags = 1)
        {
            AudioAPI.WindowsAPI.User32.KeyboardEvent(vk, scanCode, flags, IntPtr.Zero);
        }

        #endregion HelperMethods

        #region ClassFunctions

        public VolumeControlForm()
        {
            string lastTarget = Properties.Settings.Default.LastTarget;

            InitializeComponent();

            #region InitializeHotkeys
            // INITIALIZE VOLUME HOTKEYS
            hk_up = new(Properties.Settings.Default.hk_volumeup, delegate { VolumeHelper.IncrementVolume(CurrentTargetName, Properties.Settings.Default.VolumeStep); });
            hk_down = new(Properties.Settings.Default.hk_volumedown, delegate { VolumeHelper.DecrementVolume(CurrentTargetName, Properties.Settings.Default.VolumeStep); });
            hk_mute = new(Properties.Settings.Default.hk_volumemute, delegate { VolumeHelper.ToggleMute(CurrentTargetName); });
            // INITIALIZE PLAYBACK HOTKEYS
            hk_next = new(Properties.Settings.Default.hk_next, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_NEXT_TRACK); });
            hk_prev = new(Properties.Settings.Default.hk_prev, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PREV_TRACK); });
            hk_playback = new(Properties.Settings.Default.hk_playback, delegate { SendKeyboardEvent(VirtualKeyCode.VK_MEDIA_PLAY_PAUSE); });
            // INITIALIZE TARGET HOTKEYS
            hk_nextTarget = new(Properties.Settings.Default.hk_nextTarget, delegate { NextTarget(); });
            hk_prevTarget = new(Properties.Settings.Default.hk_prevTarget, delegate { PrevTarget(); });
            hk_showTarget = new(Properties.Settings.Default.hk_showTarget, delegate
            {
                ShowTarget();
            });

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
            // VOLUME STEP
            Numeric_VolumeStep.Value = Properties.Settings.Default.VolumeStep;

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
            Version currentVersion = typeof(VolumeControlForm).Assembly.GetName().Version!;
            if (Convert.ToBoolean(typeof(VolumeControlForm).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease))
            {
                Label_VersionNumber.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}";
            }
            else
            {
                Label_VersionNumber.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}";
            }

            // CANCEL BUTTON HANDLER (ESC)
            cancelHandler.Action += delegate { WindowState = FormWindowState.Minimized; };
            CancelButton = cancelHandler;
            // ALWAYS ON TOP
            bool alwaysOnTop = Properties.Settings.Default.AlwaysOnTop;
            TopMost = alwaysOnTop;
            Checkbox_AlwaysOnTop.Checked = alwaysOnTop;
            // PROCESS SELECTOR
            ComboBox_ProcessSelector.TextChanged -= ComboBox_ProcessName_TextChanged;
            UpdateProcessList(); // update the process list
            ComboBox_ProcessSelector.TextChanged += ComboBox_ProcessName_TextChanged;
            ComboBox_ProcessSelector.Text = lastTarget;

            // TARGET LIST FORM
            targetListForm.Resize += delegate { if (targetListForm.WindowState != FormWindowState.Minimized) UpdateProcessList(); }; // triggers when window is shown
            // TARGET LIST FORM ENABLED
            TargetListEnabled = Properties.Settings.Default.tgtlist_enabled;
            // TARGET LIST FORM TIMEOUT
            targetListForm.Timeout = Properties.Settings.Default.tgtlist_timeout;
            targetListForm.SelectionChanged += delegate //< Triggered when the user selects a process in the target list
            {
                SetTarget(targetListForm.Selected);
            };

            // Set a save event to trigger when properties change
            Properties.Settings.Default.PropertyChanged += SaveSettings;

            UpdateHotkeys();
            UpdateTitle();
        }
        ~VolumeControlForm()
        {
            Properties.Settings.Default.Save();
            UnregisterHotkeys();
        }

        #endregion ClassFunctions

        private static void SaveSettings(object? sender, EventArgs e)
        {
            Properties.Settings.Default.PropertyChanged -= SaveSettings;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            Properties.Settings.Default.PropertyChanged += SaveSettings;
        }

        #region FormComponents

        /// <summary>
        /// Automatically called when the value of "ComboBox_ProcessSelector.Text" is changed.
        /// Sets the settings value "ProcessName" to the new value, and updates the window title.
        /// </summary>
        private void ComboBox_ProcessName_TextChanged(object? sender, EventArgs e)
        {
            Properties.Settings.Default.LastTarget = ComboBox_ProcessSelector.Text;
            UpdateTitle();
        }
        /// <summary>
        /// Automatically called when the value of volume_step is changed.
        /// Sets the settings value "VolumeStep" to the new value.
        /// </summary>
        private void Numeric_VolumeStep_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.VolumeStep = Numeric_VolumeStep.Value;
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
            // Always bring the window to the foreground
            this.Activate();
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
        }
        /// <summary>
        /// Called when the window is focused by the user.
        /// </summary>
        private void Window_GotFocus(object sender, EventArgs e) => UpdateProcessList();
        /// <summary>
        /// Called when the Reload button is pressed.
        /// </summary>
        private void Button_ReloadProcessList_Click(object sender, EventArgs e) => UpdateProcessList();
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
        }
        private void CheckBox_VisibleInTaskbar_CheckedChanged(object sender, EventArgs e)
        {
            UnregisterHotkeys();
            bool isChecked = CheckBox_VisibleInTaskbar.Checked;
            if (isChecked != Properties.Settings.Default.VisibleInTaskbar)
            {
                // Set the value of "ShowInTaskbar", automatically re-registers hotkeys using property override
                this.ShowInTaskbar = Properties.Settings.Default.VisibleInTaskbar = isChecked;
            }
            RegisterHotkeys();
        }
        private bool TargetListEnabled
        {
            get => Checkbox_TargetListEnabled.Checked;
            set => Checkbox_TargetListEnabled.Checked = value;
        }
        private void Checkbox_ToastEnabled_CheckedChanged(object sender, EventArgs e) => Properties.Settings.Default.tgtlist_enabled = targetListForm.TimeoutEnabled = TargetListEnabled;

        private void ToastTimeout_ValueChanged(object sender, EventArgs e) => Properties.Settings.Default.tgtlist_timeout = targetListForm.Timeout = Convert.ToInt32(NumberUpDown_TargetListTimeout.Value);

        private void Checkbox_AlwaysOnTop_CheckedChanged(object sender, EventArgs e) => Properties.Settings.Default.AlwaysOnTop = TopMost = Checkbox_AlwaysOnTop.Checked;

        #endregion FormComponents

    }
}