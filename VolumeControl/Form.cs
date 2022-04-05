using System.Reflection;
using VolumeControl.Core;
using VolumeControl.Core.Attributes;
using VolumeControl.Core.Controls;
using VolumeControl.Core.Events;

namespace VolumeControl
{

    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            SuspendLayout();
            SuspendSizeToFit();

            // initialize hkedit subform (cannot bind data source yet -- VC_Static.InitializeHotkeys() hasn't been called yet. See 'Program.cs')
            hkedit.Visible = false;
            // initialize toast subform
            toast.Visible = false;

            // initialize designer components
            InitializeComponent();
            ForceCorrectLayout();

            if (Properties.Settings.Default.EnableEscapeMinimize)
                CancelButton = new VirtualButton(delegate { WindowState = FormWindowState.Minimized; });

            _panel1Height = MainSplitContainer.Panel1.Height;

            _width = Size.Width;

            bsAudioProcessAPI.DataSource = VC_Static.API;

            // get the current version number
            Assembly assembly = Assembly.GetExecutingAssembly();
            var extver = assembly.GetCustomAttribute<ExtendedVersion>()?.Version;
            if (extver != null)
                SetVersion($"v{extver}");
            else SetVersion("[????]");

            // Initialize local form settings
            tbTargetSelector.Text = Properties.Settings.Default.LastSelectedTarget;
            nAutoReloadInterval.Value = Properties.Settings.Default.LastAutoReloadInterval;
            cbAutoReload.Checked = Properties.Settings.Default.LastAutoReloadEnabled;
            cbLockTarget.Checked = Properties.Settings.Default.LastLockTargetState;
            MainSplitContainer.Panel2Collapsed = Properties.Settings.Default.LastMixerVisibleState;
            cbRunAtStartup.Checked = Properties.Settings.Default.RunAtStartup;
            cbStartMinimized.Checked = Properties.Settings.Default.StartMinimized;
            cbShowInTaskbar.Checked = Properties.Settings.Default.ShowInTaskbar;
            cbAlwaysOnTop.Checked = Properties.Settings.Default.AlwaysOnTop;
            cbToastEnabled.Checked = Properties.Settings.Default.ToastEnabled;
            nToastTimeoutInterval.Value = Properties.Settings.Default.ToastTimeoutInterval;
            cbReloadOnHotkey.Checked = Properties.Settings.Default.ReloadOnHotkey;
            VC_Static.VolumeStep = nVolumeStep.Value = Properties.Settings.Default.VolumeStep;

            // Attempt to restore window position
            var lastOrigin = Properties.Settings.Default.LastLocation;
            var lastMax = new Point(lastOrigin.X + Size.Width, lastOrigin.Y + Size.Height);
            var lastScrBounds = Screen.FromPoint(lastOrigin).Bounds;
            if (lastScrBounds.Contains(lastOrigin) && lastScrBounds.Contains(lastMax))
                Location = lastOrigin;
            else Location = lastScrBounds.Location + lastScrBounds.Size / 2 - Size / 2; // fallback to middle of the last screen

            // handle API events
            VC_Static.API.SelectedProcessChanged += delegate (object sender, TargetEventArgs e)
            {
                if (!e.UserOrigin)
                {
                    tbTargetSelector.TextChanged -= tbTargetName_TextChanged!; //< prevent recursion
                    tbTargetSelector.Text = VC_Static.API.GetSelectedProcess()?.ProcessName ?? tbTargetSelector.Text;
                    tbTargetSelector.TextChanged += tbTargetName_TextChanged!;
                }
            };
            VC_Static.API.LockSelectionChanged += delegate
            {
                cbLockTarget.CheckedChanged -= cbLockTarget_CheckedChanged!; //< prevent recursion
                bool value = VC_Static.API.LockSelection;
                tbTargetSelector.Enabled = !(cbLockTarget.Checked = value);
                cbLockTarget.CheckedChanged += cbLockTarget_CheckedChanged!;
            };
            VC_Static.API.ProcessListUpdated += RefreshProcessList!;

            // Initialize hotkey API now that we have a form
            VC_Static.InitializeHotkeys(this);
            hkedit.DataSource = VC_Static.Hotkeys;

#           if !DEBUG
            if (Properties.Settings.Default.StartMinimized)
                WindowState = FormWindowState.Minimized;
#           endif

            ResumeLayout();

            VC_Static.Log.WriteInfo("Form initialization completed.");
        }
        /// <summary>
        /// Called before the form closes.
        /// This is basically a cancellable finalizer.
        /// </summary>
        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            Mixer.RowsRemoved -= Mixer_RowsRemoved;
            SuspendLayout();
            SuspendSizeToFit();
            SaveAll();
            e.Cancel = false; // don't cancel the close event (allow the form to close)
            VC_Static.Log.WriteDebug("Form closing event triggered.");
            ResumeLayout();
        }

#region Members
        /// <summary>
        /// This maintains the height of splitContainer.Panel1 to allow the 'Toggle Mixer' button to work correctly.
        /// </summary>
        private readonly int _panel1Height;
        private readonly int _width;
        /// <summary>
        /// When true, the SizeToFit() function is enabled.
        /// When false, the SizeToFit() function will return before doing anything.
        /// </summary>
        private bool _allowAutoSize = false;
        /// <summary>
        /// The height of a single row in the mixer.
        /// </summary>
        private int _mixerListItemHeight = 0;
        /// <summary>
        /// Hotkey editor subform.
        /// </summary>
        private readonly HotkeyEditorForm hkedit = new();
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
        private void SuspendSizeToFit()
            => _allowAutoSize = false;
        private void ResumeSizeToFit(bool trigger = false)
        {
            _allowAutoSize = true;
            if (trigger) SizeToFit();
        }
        private void SizeToFit()
        {
            if (!_allowAutoSize)
                return;
            if (_mixerListItemHeight == 0) // set the height of a list item
                _mixerListItemHeight = Mixer.Font.Height + 9;

            int height = _panel1Height;

            if (FormBorderStyle != FormBorderStyle.None)
                height += 40;

            if (!MainSplitContainer.Panel2Collapsed)
            {
                if (Mixer.ColumnHeadersVisible)
                    height += Mixer.ColumnHeadersHeight;
                height += MainSplitContainer.SplitterWidth + MixerSplitContainer.Panel1.Height;
                int threeQuartersHeight = Screen.PrimaryScreen.WorkingArea.Height - (Screen.PrimaryScreen.WorkingArea.Height / 4) - _panel1Height;
                int totalFittingElements = threeQuartersHeight / _mixerListItemHeight;

                int totalCellHeight = _mixerListItemHeight * (Mixer.Rows.Count % totalFittingElements);
                height += totalCellHeight;
            }

            // set the size of the form
            Size = new(_width, height);
            UpdateBounds();
            VC_Static.Log.WriteDebug($"Form size updated to ({_width}, {height})");
        }
        /// <summary>
        /// Request a full reload of the process list from <see cref="VC_Static.API"/>.
        /// </summary>
        private static void ReloadProcessList()
            => VC_Static.API.ReloadProcessList();
        /// <summary>
        /// Event handler overload of the <see cref="ReloadProcessList()"/> function.
        /// </summary>
        private void ReloadProcessList(object sender, EventArgs e) => ReloadProcessList();
        /// <summary>
        /// Refresh the list of processes displayed in the Mixer. <br></br>
        /// This doesn't actually request a reload event from <see cref="VC_Static.API"/>, instead it simply resets the data source in order to refresh the displayed list. <br></br>
        /// To perform a full reload of the list, see the <see cref="ReloadProcessList()"/> function.
        /// </summary>
        private void RefreshProcessList()
        {
            SuspendSizeToFit();
            Mixer.SuspendLayout();
            Mixer.SetDataSource(Mixer.UnsetDataSource());
            Mixer.Update();
            ResumeSizeToFit();
            Mixer.ResumeLayout();
        }
        /// <summary>
        /// Event handler overload of the <see cref="RefreshProcessList()"/> function.
        /// </summary>
        private void RefreshProcessList(object sender, EventArgs e) => RefreshProcessList();
        public new void BringToFront()
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
            base.BringToFront();
        }
        private void SetVersion(string ver)
            => Label_Version.Text = ver;
        /// <summary>
        /// Set properties to their current UI values and save them to the config file.
        /// </summary>
        private void SaveAll()
        {
            // save hotkeys
            VC_Static.SaveSettings();
            // Update local properties
            Properties.Settings.Default.SetProperty("LastSelectedTarget", tbTargetSelector.Text);
            Properties.Settings.Default.SetProperty("LastAutoReloadInterval", nAutoReloadInterval.Value);
            Properties.Settings.Default.SetProperty("LastAutoReloadEnabled", cbAutoReload.Checked);
            Properties.Settings.Default.SetProperty("LastLockTargetState", cbLockTarget.Checked);
            Properties.Settings.Default.SetProperty("LastMixerVisibleState", MainSplitContainer.Panel2Collapsed);
            Properties.Settings.Default.SetProperty("RunAtStartup", cbRunAtStartup.Checked);
            Properties.Settings.Default.SetProperty("StartMinimized", cbStartMinimized.Checked);
            Properties.Settings.Default.SetProperty("ShowInTaskbar", cbShowInTaskbar.Checked);
            Properties.Settings.Default.SetProperty("AlwaysOnTop", cbAlwaysOnTop.Checked);
            Properties.Settings.Default.SetProperty("ToastEnabled", cbToastEnabled.Checked);
            Properties.Settings.Default.SetProperty("ToastTimeoutInterval", nToastTimeoutInterval.Value);
            Properties.Settings.Default.SetProperty("ReloadOnHotkey", cbReloadOnHotkey.Checked);
            Properties.Settings.Default.SetProperty("VolumeStep", nVolumeStep.Value);
            Properties.Settings.Default.SetProperty("LastLocation", Location);
            // Save properties
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();

            VC_Static.Log.WriteInfo("Saved 'VolumeControl' project properties.");
        }
        /// <summary>
        /// This function fixes the designer's failure to actually apply certain settings.
        /// </summary>
        private void ForceCorrectLayout()
        {
            // Force default cursors on both split containers, in case the designer decides to change them again
            MainSplitContainer.Cursor = Cursors.Default;
            MixerSplitContainer.Cursor = Cursors.Default;

            // force correct layout on panel2SplitContainer
            MixerSplitContainer.Panel1MinSize = 0; // zero sizes first
            MixerSplitContainer.Panel2MinSize = 0;
            MixerSplitContainer.SplitterDistance = 29; // set splitter dist
            MixerSplitContainer.SplitterWidth = 1; // correct splitter width
            MixerSplitContainer.Panel1MinSize = 29; // apply minimum panel sizes
            MixerSplitContainer.Panel2MinSize = 23;
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
            if (e.ColumnIndex == MixerColSelectButton.Index)
            {
                if (!LockTarget)
                    tbTargetSelector.Text = VC_Static.API.ProcessList[e.RowIndex].ProcessName;
            }
            else if (e.ColumnIndex == MixerColVolumeUp.Index)
            { // volume up
                var proc = VC_Static.API.ProcessList[e.RowIndex];
                AudioAPI.VolumeHelper.IncrementVolume(proc.PID, VC_Static.VolumeStep);
                RefreshProcessList();
                VC_Static.Log.WriteInfo($"Increased volume of '{proc.ProcessName}' to '{proc.VolumePercent}'");
            }
            else if (e.ColumnIndex == MixerColVolumeDown.Index)
            { // volume down
                var proc = VC_Static.API.ProcessList[e.RowIndex];
                AudioAPI.VolumeHelper.DecrementVolume(proc.PID, VC_Static.VolumeStep);
                RefreshProcessList();
                VC_Static.Log.WriteInfo($"Decreased volume of '{proc.ProcessName}' to '{proc.VolumePercent}'");
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
        /// <summary>
        /// Handles drawing checkboxes for the Mixer.
        /// </summary>
        private void Mixer_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == MixerColMuted.Index)
                {
                    e.Handled = true;
                    Graphics g = e.Graphics;
                    var rect = e.CellBounds;
                    int bottom = rect.Y + rect.Height - 1;
                    rect.Size = new(rect.Width, rect.Height - 1);
                    // draw background
                    g.FillRectangle(new SolidBrush(e.CellStyle.BackColor), rect);
                    // draw divider
                    g.DrawLine(new Pen(new SolidBrush(Mixer.GridColor), 1f), new(rect.X, bottom), new Point(rect.X + rect.Width, bottom));

                    int boxSize = 13;
                    var box = new Rectangle(rect.Location.X + (rect.Width / 2 - boxSize / 2), rect.Location.Y + (rect.Height / 2 - boxSize / 2), boxSize, boxSize);
                    var b = new SolidBrush(Color.FromArgb(200, 200, 200));
                    g.DrawRectangle(new Pen(b, 1f), box);
                    if (Convert.ToBoolean(e.Value))
                    {
                        g.FillRectangle(b, new Rectangle(box.X + 3, box.Y + 3, box.Width - 5, box.Height - 5));
                    }
                }
            }
        }
#endregion MixerEventHandlers

#region ControlEventHandlers
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
        /// Handles click events for the 'Edit Hotkeys...' button.
        /// </summary>
        private void bHotkeyEditor_Click(object sender, EventArgs e)
        {
            hkedit.Toggle();
            if (hkedit.Visible)
            {
                var workingArea = Screen.FromPoint(Location).WorkingArea;
                var size = hkedit.Size;
                if (Location.X > workingArea.X + size.Width + 2)
                {
                    hkedit.Location = new(Location.X - size.Width - 2, Location.Y);
                }
                else
                {
                    hkedit.Location = new(Location.X + Size.Width + 1, Location.Y);
                }
            }
        }
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
        {
            SuspendSizeToFit(); // disable the SizeToFit() function to prevent flicker during the transition.
            ShowInTaskbar = cbShowInTaskbar.Checked;
            ResumeSizeToFit();
        }
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
            MainSplitContainer.Panel2Collapsed = !MainSplitContainer.Panel2Collapsed;
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
        /// <summary>
        /// Handles check/uncheck events for the reload 'On Hotkey' checkbox.
        /// </summary>
        private void cbReloadOnHotkey_CheckedChanged(object sender, EventArgs e)
            => VC_Static.API.ReloadOnHotkey = cbReloadOnHotkey.Checked;
        /// <summary>
        /// Handles value change events for the 'Volume Step' number selector.
        /// </summary>
        private void nVolumeStep_ValueChanged(object sender, EventArgs e)
            => VC_Static.VolumeStep = nVolumeStep.Value;
        /// <summary>
        /// Handles form 'Resize' events by setting the 'Visible' property to allow minimizing the form without showing a small box in the bottom-left of the screen.
        /// </summary>
        private void Form_Resize(object sender, EventArgs e)
            => Visible = !(WindowState == FormWindowState.Minimized);
        /// <summary>
        /// Handles the form 'Load' event, and enables the SizeToFit() function.
        /// This function is disabled during initialization, to prevent constant form resizing when the mixer is being populated.
        /// </summary>
        private void Form_Load(object sender, EventArgs e)
            => ResumeSizeToFit(true);
        /// <summary>
        /// Handles drawing checkboxes.
        /// </summary>
        private void cbPaint(object sender, PaintEventArgs e)
        {
            if (sender is not CheckBox cb) return;

            Graphics g = e.Graphics;
            var rect = e.ClipRectangle;

            CheckBoxRenderer.DrawParentBackground(g, rect, cb);

            int boxSize = 13;

            var box = new Rectangle(rect.Location.X, rect.Location.Y + (rect.Height / 2 - boxSize / 2), boxSize - 1, boxSize - 1);
            int textStart = box.X + box.Width + 4;

            var b = new SolidBrush(Color.FromArgb(200, 200, 200));

            g.DrawRectangle(new Pen(b, 1f), box);
            if (cb.Checked)
            {
                g.FillRectangle(b, new Rectangle(box.X + 3, box.Y + 3, box.Width - 5, box.Height - 5));
            }

            g.DrawString(cb.Text, cb.Font, new SolidBrush(cb.ForeColor), new Point(textStart, rect.Location.Y + 1));
        }
#endregion ControlEventHandlers
    }
}