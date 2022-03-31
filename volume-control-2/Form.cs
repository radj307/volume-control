using Core;
using CoreControls;
using System.Reflection;

namespace volume_control_2
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            hkedit = new(VC_Static.API);
            hkedit.Hide();
            InitializeComponent();
            bsAudioProcessAPI.DataSource = VC_Static.API;

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

            VC_Static.API.SelectedProcessChanged += delegate
            {
                tbTargetSelector.TextChanged -= tbTargetName_TextChanged!;
                tbTargetSelector.Text = VC_Static.API.GetSelectedProcess().ProcessName;
                tbTargetSelector.TextChanged += tbTargetName_TextChanged!;
            };
            VC_Static.API.LockSelectionChanged += delegate
            {
                cbLockTarget.CheckedChanged -= cbLockTarget_CheckedChanged!;
                tbTargetSelector.Enabled = !(cbLockTarget.Checked = VC_Static.API.LockSelection);
                cbLockTarget.CheckedChanged += cbLockTarget_CheckedChanged!;
            };
        }
        /// <summary>
        /// Called before the form closes.
        /// This is basically a cancellable finalizer.
        /// </summary>
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            VC_Static.SaveSettings();
            // Update local properties
            Properties.Settings.Default.LastSelectedTarget = tbTargetSelector.Text;
            Properties.Settings.Default.LastAutoReloadInterval = nAutoReloadInterval.Value;
            Properties.Settings.Default.LastAutoReloadEnabled = cbAutoReload.Checked;
            Properties.Settings.Default.LastLockTargetState = cbLockTarget.Checked;
            // Save properties
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            e.Cancel = false; // don't cancel the close event (allow the form to close)
        }

        /// <summary>
        /// The height of a single row in the mixer.
        /// </summary>
        private int _mixerListItemHeight = 0;
        /// <summary>
        /// Hotkey editor subform.
        /// </summary>
        public readonly HotkeyEditorForm hkedit;

        private void SizeToFit()
        {
            if (_mixerListItemHeight == 0) // set the height of a list item
                _mixerListItemHeight = Mixer.Font.Height + 9;

            int padding = splitContainer1.Panel1.Height + splitContainer1.SplitterWidth + Mixer.ColumnHeadersHeight;
            if (FormBorderStyle != FormBorderStyle.None)
                padding += 40;

            int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4);
            int totalFittingElements = threeQuartersHeight / _mixerListItemHeight;

            int totalCellHeight = _mixerListItemHeight * (Mixer.Rows.Count % totalFittingElements);

            // set the size of the form
            Size = new(Width, totalCellHeight + padding);
            UpdateBounds();
        }

        private void ReloadProcessList()
        {
            Mixer.SuspendLayout();
            Mixer.ResetDataSource(bsAudioProcessAPI, VC_Static.API.ReloadProcessList, true);
            Mixer.ResumeLayout();
        }

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

        /// <summary>
        /// Handles click events for the 'Reload' button.
        /// </summary>
        private void bReload_Click(object sender, EventArgs e)
            => ReloadProcessList();

        private void cbAutoReload_CheckedChanged(object sender, EventArgs e)
        {
            tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);
            tAutoReload.Enabled = nAutoReloadInterval.Enabled = cbAutoReload.Checked;
        }

        private void nAutoReloadInterval_ValueChanged(object sender, EventArgs e)
            => tAutoReload.Interval = Convert.ToInt32(nAutoReloadInterval.Value);

        private void tAutoReload_Tick(object sender, EventArgs e)
            => ReloadProcessList();

        private void bHotkeyEditor_Click(object sender, EventArgs e)
            => hkedit.Toggle();

        private void tbTargetName_TextChanged(object sender, EventArgs e)
            => VC_Static.API.SetSelectedProcess(tbTargetSelector.Text);
        private void cbLockTarget_CheckedChanged(object sender, EventArgs e)
            => tbTargetSelector.Enabled = !(LockTarget = cbLockTarget.Checked);

        private bool LockTarget
        {
            get => VC_Static.API.LockSelection;
            set => VC_Static.API.LockSelection = value;
        }
    }
}