using VolumeControl.Core.Controls;
using System.Reflection;
using VolumeControl.Core;

namespace VolumeControl
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            SuspendLayout();

            // initialize hkedit subform (cannot bind data source yet -- VC_Static.InitializeHotkeys() hasn't been called yet. See 'Program.cs')
            hkedit.Hide();
            // initialize toast subform
            toast.Hide();

            // initialize designer components
            InitializeComponent();
            _panel1Height = splitContainer.Panel1.Height;

            Mixer.RowsAdded -= Mixer_RowsAdded;
            Mixer.RowsRemoved -= Mixer_RowsRemoved;

            bsAudioProcessAPI.DataSource = VC_Static.API;

            // Set the current version number
            Version currentVersion = typeof(Form).Assembly.GetName().Version!;
            if (Convert.ToBoolean(typeof(Form).Assembly.GetCustomAttribute<IsPreReleaseAttribute>()?.IsPreRelease))
                Label_Version.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}-pre{currentVersion.Revision}";
            else
                Label_Version.Text = $"v{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}{(currentVersion.Revision >= 1 ? $"-{currentVersion.Revision}" : "")}";

            // Initialize local form settings
            tbTargetSelector.Text = Properties.Settings.Default.LastSelectedTarget;
            nAutoReloadInterval.Value = Properties.Settings.Default.LastAutoReloadInterval;
            cbAutoReload.Checked = Properties.Settings.Default.LastAutoReloadEnabled;
            cbLockTarget.Checked = Properties.Settings.Default.LastLockTargetState;
            splitContainer.Panel2Collapsed = Properties.Settings.Default.LastMixerVisibleState;
            cbRunAtStartup.Checked = Properties.Settings.Default.RunAtStartup;
            cbStartMinimized.Checked = Properties.Settings.Default.StartMinimized;
            cbShowInTaskbar.Checked = Properties.Settings.Default.ShowInTaskbar;
            cbAlwaysOnTop.Checked = Properties.Settings.Default.AlwaysOnTop;
            cbToastEnabled.Checked = Properties.Settings.Default.ToastEnabled;
            nToastTimeoutInterval.Value = Properties.Settings.Default.ToastTimeoutInterval;

            // handle API events
            VC_Static.API.SelectedProcessChanged += delegate
            {
                tbTargetSelector.TextChanged -= tbTargetName_TextChanged!; //< prevent recursion
                tbTargetSelector.Text = VC_Static.API.GetSelectedProcess().ProcessName;
                tbTargetSelector.TextChanged += tbTargetName_TextChanged!;
            };
            VC_Static.API.LockSelectionChanged += delegate
            {
                cbLockTarget.CheckedChanged -= cbLockTarget_CheckedChanged!; //< prevent recursion
                tbTargetSelector.Enabled = !(cbLockTarget.Checked = VC_Static.API.LockSelection);
                cbLockTarget.CheckedChanged += cbLockTarget_CheckedChanged!;
            };

            Mixer.RowsAdded += Mixer_RowsAdded;
            Mixer.RowsRemoved += Mixer_RowsRemoved;

            ResumeLayout();
        }
        private void SaveAll()
        {
            // save hotkeys
            VC_Static.SaveSettings();
            // Update local properties
            Properties.Settings.Default.LastSelectedTarget = tbTargetSelector.Text;
            Properties.Settings.Default.LastAutoReloadInterval = nAutoReloadInterval.Value;
            Properties.Settings.Default.LastAutoReloadEnabled = cbAutoReload.Checked;
            Properties.Settings.Default.LastLockTargetState = cbLockTarget.Checked;
            Properties.Settings.Default.LastMixerVisibleState = splitContainer.Panel2Collapsed;
            Properties.Settings.Default.RunAtStartup = cbRunAtStartup.Checked;
            Properties.Settings.Default.StartMinimized = cbStartMinimized.Checked;
            Properties.Settings.Default.ShowInTaskbar = cbShowInTaskbar.Checked;
            Properties.Settings.Default.AlwaysOnTop = cbAlwaysOnTop.Checked;
            Properties.Settings.Default.ToastEnabled = cbToastEnabled.Checked;
            Properties.Settings.Default.ToastTimeoutInterval = nToastTimeoutInterval.Value;
            // Save properties
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
        }
        /// <summary>
        /// Called before the form closes.
        /// This is basically a cancellable finalizer.
        /// </summary>
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAll();
            e.Cancel = false; // don't cancel the close event (allow the form to close)
        }
        ~Form()
        {
            SaveAll();
        }

        #region Members
        /// <summary>
        /// This maintains the height of splitContainer.Panel1 to allow the 'Toggle Mixer' button to work correctly.
        /// </summary>
        private readonly int _panel1Height;
        /// <summary>
        /// The height of a single row in the mixer.
        /// </summary>
        private int _mixerListItemHeight = 0;
        /// <summary>
        /// Hotkey editor subform.
        /// </summary>
        public readonly HotkeyEditorForm hkedit = new();
        private readonly ToastForm toast = new();
        #endregion Members

        #region Properties
        /// <summary>
        /// Lock or unlock the current target selection.
        /// This binds directly to (VC_Static.API.LockSelection).
        /// </summary>
        private static bool LockTarget
        {
            get => VC_Static.API.LockSelection;
            set => VC_Static.API.LockSelection = value;
        }
        #endregion Properties

        #region Methods
        private void SizeToFit()
        {
            if (_mixerListItemHeight == 0) // set the height of a list item
                _mixerListItemHeight = Mixer.Font.Height + 9;

            int height = _panel1Height;

            if (FormBorderStyle != FormBorderStyle.None)
                height += 40;

            if (!splitContainer.Panel2Collapsed)
            {
                height += Mixer.ColumnHeadersHeight + splitContainer.SplitterWidth;
                int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);
                int totalFittingElements = threeQuartersHeight / _mixerListItemHeight;

                int totalCellHeight = _mixerListItemHeight * (Mixer.Rows.Count % totalFittingElements);
                height += totalCellHeight;
            }

            // set the size of the form
            Size = new(Width, height);
            UpdateBounds();
        }

        private void ReloadProcessList()
        {
            Mixer.SuspendLayout();
            Mixer.ResetDataSource(bsAudioProcessAPI, VC_Static.API.ReloadProcessList, true);
            Mixer.ResumeLayout();
        }

        public new void BringToFront()
        {
            if (WindowState != FormWindowState.Normal)
                WindowState = FormWindowState.Normal;
            Show();
            base.BringToFront();
        }
        #endregion Methods

        #region MixerEventHandlers
        /// <summary>
        /// Handles checkbox checked/unchecked events in the mixer datagrid.
        /// This allows the checkboxes to actually function.
        /// </summary>
        private void Mixer_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (Mixer.CurrentCell is DataGridViewCheckBoxCell)
                Mixer.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
        /// <summary>
        /// Handles Mixer 'Select' button click events.
        /// </summary>
        private void Mixer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == MixerColSelectButton.Index && !LockTarget)
            {
                tbTargetSelector.Text = VC_Static.API.ProcessList[e.RowIndex].ProcessName;
            }
        }
        /// <summary>
        /// This is triggered when rows are added from the list, including from the data binding source.
        /// </summary>
        private void Mixer_RowsAdded(object? sender, DataGridViewRowsAddedEventArgs e)
            => SizeToFit();
        /// <summary>
        /// This is triggered when rows are removed from the list, including from the data binding source.
        /// </summary>
        private void Mixer_RowsRemoved(object? sender, DataGridViewRowsRemovedEventArgs e)
            => SizeToFit();
        #endregion MixerEventHandlers

        #region ControlEventHandlers
        /// <summary>
        /// Handles click events for the 'Reload' button.
        /// </summary>
        private void bReload_Click(object sender, EventArgs e)
            => ReloadProcessList();
        /// <summary>
        /// Handles check/uncheck events for the 'Auto' reload checkbox
        /// </summary>
        private void cbAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);
            tAutoReload.Enabled = nAutoReloadInterval.Enabled = cbAutoReload.Checked;
        }
        /// <summary>
        /// Handles value change events for the 'Interval' number selector
        /// </summary>
        private void nAutoReloadInterval_ValueChanged(object sender, EventArgs e)
            => tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);
        /// <summary>
        /// Handles tick events for the auto-reload timer.
        /// </summary>
        private void tAutoReload_Tick(object sender, EventArgs e)
            => ReloadProcessList();
        /// <summary>
        /// Handles click events for the 'Edit Hotkeys...' button.
        /// </summary>
        private void bHotkeyEditor_Click(object sender, EventArgs e)
            => hkedit.Toggle();
        /// <summary>
        /// Handles text change events for the 'Target Process Name' textbox.
        /// </summary>
        private void tbTargetName_TextChanged(object sender, EventArgs e)
            => VC_Static.API.SetSelectedProcess(tbTargetSelector.Text);
        /// <summary>
        /// Handles check/uncheck events for the 'Lock' current target checkbox.
        /// </summary>
        private void cbLockTarget_CheckedChanged(object sender, EventArgs e)
            => tbTargetSelector.Enabled = !(LockTarget = cbLockTarget.Checked);
        /// <summary>
        /// Handles check/uncheck events for the 'Run at Startup' checkbox.
        /// </summary>
        private void cbRunAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRunAtStartup.Checked)
                RegAPI.EnableRunOnStartup();
            else
                RegAPI.DisableRunOnStartup();
        }
        /// <summary>
        /// Handles check/uncheck events for the 'Show in Taskbar' checkbox.
        /// </summary>
        private void cbShowInTaskbar_CheckedChanged(object sender, EventArgs e)
            => ShowInTaskbar = cbShowInTaskbar.Checked;
        /// <summary>
        /// Handles check/uncheck events for the 'Always on Top' checkbox.
        /// </summary>
        private void cbAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
            => TopMost = cbAlwaysOnTop.Checked;
        /// <summary>
        /// Handles click events for the 'Toggle Mixer' button.
        /// </summary>
        private void bToggleMixer_Click(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
            SizeToFit();
        }
        /// <summary>
        /// Handles double-click events for the tray icon.
        /// </summary>
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
            => BringToFront();
        /// <summary>
        /// Handles click events for the tray icon context menu's 'Bring to Front' button.
        /// </summary>
        private void TrayContextMenuBringToFront_Click(object sender, EventArgs e)
            => BringToFront();
        /// <summary>
        /// Handles click events for the tray icon context menu's 'Close' button.
        /// </summary>
        private void TrayContextMenuClose_Click(object sender, EventArgs e)
            => Close();
        /// <summary>
        /// Handles check/uncheck events for the toast 'Enable' checkbox
        /// </summary>
        private void cbToastEnabled_CheckedChanged(object sender, EventArgs e)
            => nToastTimeoutInterval.Enabled = toast.Visible = toast.Enabled = cbToastEnabled.Checked;
        /// <summary>
        /// Handles value change events for the toast 'Timeout' number selector.
        /// </summary>
        private void nToastTimeoutInterval_ValueChanged(object sender, EventArgs e)
            => toast.TimeoutInterval = Convert.ToInt32(nToastTimeoutInterval.Value);
        #endregion ControlEventHandlers


    }
}