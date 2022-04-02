using VolumeControl.Core;

namespace VolumeControl
{
    partial class Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TrayContextMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.TrayContextMenuBringToFront = new System.Windows.Forms.ToolStripMenuItem();
            this.bsAudioProcessAPI = new System.Windows.Forms.BindingSource(this.components);
            this.Mixer = new VolumeControl.Core.Controls.DoubleBufferedDataGridView();
            this.MixerColPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColMuted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MixerColSelectButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bHotkeyEditor = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.Label_ProgramName = new System.Windows.Forms.Label();
            this.gpSettings = new System.Windows.Forms.GroupBox();
            this.Label_VolumeStep = new System.Windows.Forms.Label();
            this.nVolumeStep = new System.Windows.Forms.NumericUpDown();
            this.cbRunAtStartup = new System.Windows.Forms.CheckBox();
            this.cbStartMinimized = new System.Windows.Forms.CheckBox();
            this.cbShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cbAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.cbLockTarget = new System.Windows.Forms.CheckBox();
            this.tbTargetSelector = new System.Windows.Forms.TextBox();
            this.gbToastNotifications = new System.Windows.Forms.GroupBox();
            this.LabelToastTimeout = new System.Windows.Forms.Label();
            this.cbToastEnabled = new System.Windows.Forms.CheckBox();
            this.nToastTimeoutInterval = new System.Windows.Forms.NumericUpDown();
            this.bToggleMixer = new System.Windows.Forms.Button();
            this.Label_Version = new System.Windows.Forms.Label();
            this.panel2SplitContainer = new System.Windows.Forms.SplitContainer();
            this.cbReloadOnHotkey = new System.Windows.Forms.CheckBox();
            this.nAutoReloadInterval = new System.Windows.Forms.NumericUpDown();
            this.bReload = new System.Windows.Forms.Button();
            this.Label_AutoReloadInterval = new System.Windows.Forms.Label();
            this.cbAutoReload = new System.Windows.Forms.CheckBox();
            this.tAutoReload = new System.Windows.Forms.Timer(this.components);
            this.TrayContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.gpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeStep)).BeginInit();
            this.gbToastNotifications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nToastTimeoutInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel2SplitContainer)).BeginInit();
            this.panel2SplitContainer.Panel1.SuspendLayout();
            this.panel2SplitContainer.Panel2.SuspendLayout();
            this.panel2SplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nAutoReloadInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            resources.ApplyResources(this.TrayIcon, "TrayIcon");
            this.TrayIcon.ContextMenuStrip = this.TrayContextMenu;
            this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayIcon_DoubleClick);
            // 
            // TrayContextMenu
            // 
            this.TrayContextMenu.BackColor = System.Drawing.SystemColors.Control;
            this.TrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TrayContextMenuClose,
            this.TrayContextMenuSeparator,
            this.TrayContextMenuBringToFront});
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            resources.ApplyResources(this.TrayContextMenu, "TrayContextMenu");
            this.TrayContextMenu.TabStop = true;
            // 
            // TrayContextMenuClose
            // 
            this.TrayContextMenuClose.Image = global::VolumeControl.Properties.Resources.X;
            this.TrayContextMenuClose.Name = "TrayContextMenuClose";
            resources.ApplyResources(this.TrayContextMenuClose, "TrayContextMenuClose");
            this.TrayContextMenuClose.Click += new System.EventHandler(this.TrayContextMenuClose_Click);
            // 
            // TrayContextMenuSeparator
            // 
            this.TrayContextMenuSeparator.Name = "TrayContextMenuSeparator";
            resources.ApplyResources(this.TrayContextMenuSeparator, "TrayContextMenuSeparator");
            // 
            // TrayContextMenuBringToFront
            // 
            this.TrayContextMenuBringToFront.Image = global::VolumeControl.Properties.Resources.foreground;
            this.TrayContextMenuBringToFront.Name = "TrayContextMenuBringToFront";
            resources.ApplyResources(this.TrayContextMenuBringToFront, "TrayContextMenuBringToFront");
            this.TrayContextMenuBringToFront.Click += new System.EventHandler(this.TrayContextMenuBringToFront_Click);
            // 
            // bsAudioProcessAPI
            // 
            this.bsAudioProcessAPI.DataMember = "ProcessList";
            this.bsAudioProcessAPI.DataSource = typeof(VolumeControl.Core.AudioProcessAPI);
            this.bsAudioProcessAPI.Sort = "";
            // 
            // Mixer
            // 
            this.Mixer.AllowUserToAddRows = false;
            this.Mixer.AllowUserToDeleteRows = false;
            this.Mixer.AllowUserToOrderColumns = true;
            this.Mixer.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.Mixer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.Mixer.AutoGenerateColumns = false;
            this.Mixer.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Mixer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Mixer.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.Mixer.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Mixer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Mixer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MixerColPID,
            this.MixerColProcessName,
            this.MixerColVolume,
            this.MixerColMuted,
            this.MixerColSelectButton});
            this.Mixer.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Mixer.DataSource = this.bsAudioProcessAPI;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.Mixer, "Mixer");
            this.Mixer.DoubleBuffered = true;
            this.Mixer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Mixer.MultiSelect = false;
            this.Mixer.Name = "Mixer";
            this.Mixer.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Mixer.RowHeadersVisible = false;
            this.Mixer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.Mixer.RowTemplate.Height = 25;
            this.Mixer.ShowCellErrors = false;
            this.Mixer.ShowEditingIcon = false;
            this.Mixer.ShowRowErrors = false;
            this.Mixer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Mixer_CellContentClick);
            this.Mixer.CurrentCellDirtyStateChanged += new System.EventHandler(this.Mixer_CurrentCellDirtyStateChanged);
            this.Mixer.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.Mixer_RowsAdded);
            this.Mixer.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.Mixer_RowsRemoved);
            // 
            // MixerColPID
            // 
            this.MixerColPID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MixerColPID.DataPropertyName = "PID";
            resources.ApplyResources(this.MixerColPID, "MixerColPID");
            this.MixerColPID.Name = "MixerColPID";
            this.MixerColPID.ReadOnly = true;
            this.MixerColPID.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // MixerColProcessName
            // 
            this.MixerColProcessName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MixerColProcessName.DataPropertyName = "ProcessName";
            resources.ApplyResources(this.MixerColProcessName, "MixerColProcessName");
            this.MixerColProcessName.Name = "MixerColProcessName";
            this.MixerColProcessName.ReadOnly = true;
            this.MixerColProcessName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // MixerColVolume
            // 
            this.MixerColVolume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MixerColVolume.DataPropertyName = "VolumePercent";
            resources.ApplyResources(this.MixerColVolume, "MixerColVolume");
            this.MixerColVolume.MaxInputLength = 4;
            this.MixerColVolume.Name = "MixerColVolume";
            this.MixerColVolume.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // MixerColMuted
            // 
            this.MixerColMuted.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MixerColMuted.DataPropertyName = "Muted";
            resources.ApplyResources(this.MixerColMuted, "MixerColMuted");
            this.MixerColMuted.Name = "MixerColMuted";
            // 
            // MixerColSelectButton
            // 
            this.MixerColSelectButton.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.MixerColSelectButton, "MixerColSelectButton");
            this.MixerColSelectButton.Name = "MixerColSelectButton";
            this.MixerColSelectButton.Text = "Select";
            this.MixerColSelectButton.UseColumnTextForButtonValue = true;
            // 
            // bHotkeyEditor
            // 
            resources.ApplyResources(this.bHotkeyEditor, "bHotkeyEditor");
            this.bHotkeyEditor.Name = "bHotkeyEditor";
            this.bHotkeyEditor.UseVisualStyleBackColor = true;
            this.bHotkeyEditor.Click += new System.EventHandler(this.bHotkeyEditor_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer.Cursor = System.Windows.Forms.Cursors.HSplit;
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.Label_ProgramName);
            this.splitContainer.Panel1.Controls.Add(this.gpSettings);
            this.splitContainer.Panel1.Controls.Add(this.cbLockTarget);
            this.splitContainer.Panel1.Controls.Add(this.tbTargetSelector);
            this.splitContainer.Panel1.Controls.Add(this.gbToastNotifications);
            this.splitContainer.Panel1.Controls.Add(this.bToggleMixer);
            this.splitContainer.Panel1.Controls.Add(this.Label_Version);
            this.splitContainer.Panel1.Controls.Add(this.bHotkeyEditor);
            this.splitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel2SplitContainer);
            this.splitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // Label_ProgramName
            // 
            resources.ApplyResources(this.Label_ProgramName, "Label_ProgramName");
            this.Label_ProgramName.Name = "Label_ProgramName";
            // 
            // gpSettings
            // 
            this.gpSettings.Controls.Add(this.Label_VolumeStep);
            this.gpSettings.Controls.Add(this.nVolumeStep);
            this.gpSettings.Controls.Add(this.cbRunAtStartup);
            this.gpSettings.Controls.Add(this.cbStartMinimized);
            this.gpSettings.Controls.Add(this.cbShowInTaskbar);
            this.gpSettings.Controls.Add(this.cbAlwaysOnTop);
            resources.ApplyResources(this.gpSettings, "gpSettings");
            this.gpSettings.Name = "gpSettings";
            this.gpSettings.TabStop = false;
            // 
            // Label_VolumeStep
            // 
            resources.ApplyResources(this.Label_VolumeStep, "Label_VolumeStep");
            this.Label_VolumeStep.Name = "Label_VolumeStep";
            // 
            // nVolumeStep
            // 
            resources.ApplyResources(this.nVolumeStep, "nVolumeStep");
            this.nVolumeStep.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nVolumeStep.Name = "nVolumeStep";
            this.nVolumeStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nVolumeStep.ValueChanged += new System.EventHandler(this.nVolumeStep_ValueChanged);
            // 
            // cbRunAtStartup
            // 
            resources.ApplyResources(this.cbRunAtStartup, "cbRunAtStartup");
            this.cbRunAtStartup.Name = "cbRunAtStartup";
            this.cbRunAtStartup.UseVisualStyleBackColor = true;
            this.cbRunAtStartup.CheckedChanged += new System.EventHandler(this.cbRunAtStartup_CheckedChanged);
            // 
            // cbStartMinimized
            // 
            resources.ApplyResources(this.cbStartMinimized, "cbStartMinimized");
            this.cbStartMinimized.Name = "cbStartMinimized";
            this.cbStartMinimized.UseVisualStyleBackColor = true;
            // 
            // cbShowInTaskbar
            // 
            resources.ApplyResources(this.cbShowInTaskbar, "cbShowInTaskbar");
            this.cbShowInTaskbar.Name = "cbShowInTaskbar";
            this.cbShowInTaskbar.UseVisualStyleBackColor = true;
            this.cbShowInTaskbar.CheckedChanged += new System.EventHandler(this.cbShowInTaskbar_CheckedChanged);
            // 
            // cbAlwaysOnTop
            // 
            resources.ApplyResources(this.cbAlwaysOnTop, "cbAlwaysOnTop");
            this.cbAlwaysOnTop.Name = "cbAlwaysOnTop";
            this.cbAlwaysOnTop.UseVisualStyleBackColor = true;
            this.cbAlwaysOnTop.CheckedChanged += new System.EventHandler(this.cbAlwaysOnTop_CheckedChanged);
            // 
            // cbLockTarget
            // 
            resources.ApplyResources(this.cbLockTarget, "cbLockTarget");
            this.cbLockTarget.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbLockTarget.Name = "cbLockTarget";
            this.cbLockTarget.UseVisualStyleBackColor = true;
            this.cbLockTarget.CheckedChanged += new System.EventHandler(this.cbLockTarget_CheckedChanged);
            // 
            // tbTargetSelector
            // 
            this.tbTargetSelector.AllowDrop = true;
            this.tbTargetSelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            resources.ApplyResources(this.tbTargetSelector, "tbTargetSelector");
            this.tbTargetSelector.Name = "tbTargetSelector";
            this.tbTargetSelector.TextChanged += new System.EventHandler(this.tbTargetName_TextChanged);
            // 
            // gbToastNotifications
            // 
            this.gbToastNotifications.Controls.Add(this.LabelToastTimeout);
            this.gbToastNotifications.Controls.Add(this.cbToastEnabled);
            this.gbToastNotifications.Controls.Add(this.nToastTimeoutInterval);
            this.gbToastNotifications.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.gbToastNotifications, "gbToastNotifications");
            this.gbToastNotifications.Name = "gbToastNotifications";
            this.gbToastNotifications.TabStop = false;
            // 
            // LabelToastTimeout
            // 
            resources.ApplyResources(this.LabelToastTimeout, "LabelToastTimeout");
            this.LabelToastTimeout.Name = "LabelToastTimeout";
            // 
            // cbToastEnabled
            // 
            resources.ApplyResources(this.cbToastEnabled, "cbToastEnabled");
            this.cbToastEnabled.Checked = true;
            this.cbToastEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbToastEnabled.Name = "cbToastEnabled";
            this.cbToastEnabled.UseVisualStyleBackColor = true;
            this.cbToastEnabled.CheckedChanged += new System.EventHandler(this.cbToastEnabled_CheckedChanged);
            // 
            // nToastTimeoutInterval
            // 
            this.nToastTimeoutInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nToastTimeoutInterval, "nToastTimeoutInterval");
            this.nToastTimeoutInterval.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nToastTimeoutInterval.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.nToastTimeoutInterval.Name = "nToastTimeoutInterval";
            this.nToastTimeoutInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nToastTimeoutInterval.ValueChanged += new System.EventHandler(this.nToastTimeoutInterval_ValueChanged);
            // 
            // bToggleMixer
            // 
            resources.ApplyResources(this.bToggleMixer, "bToggleMixer");
            this.bToggleMixer.Name = "bToggleMixer";
            this.bToggleMixer.UseVisualStyleBackColor = true;
            this.bToggleMixer.Click += new System.EventHandler(this.bToggleMixer_Click);
            // 
            // Label_Version
            // 
            resources.ApplyResources(this.Label_Version, "Label_Version");
            this.Label_Version.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Label_Version.Name = "Label_Version";
            // 
            // panel2SplitContainer
            // 
            this.panel2SplitContainer.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.panel2SplitContainer, "panel2SplitContainer");
            this.panel2SplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.panel2SplitContainer.Name = "panel2SplitContainer";
            // 
            // panel2SplitContainer.Panel1
            // 
            this.panel2SplitContainer.Panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2SplitContainer.Panel1.Controls.Add(this.cbReloadOnHotkey);
            this.panel2SplitContainer.Panel1.Controls.Add(this.nAutoReloadInterval);
            this.panel2SplitContainer.Panel1.Controls.Add(this.bReload);
            this.panel2SplitContainer.Panel1.Controls.Add(this.Label_AutoReloadInterval);
            this.panel2SplitContainer.Panel1.Controls.Add(this.cbAutoReload);
            this.panel2SplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // panel2SplitContainer.Panel2
            // 
            this.panel2SplitContainer.Panel2.Controls.Add(this.Mixer);
            this.panel2SplitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // cbReloadOnHotkey
            // 
            resources.ApplyResources(this.cbReloadOnHotkey, "cbReloadOnHotkey");
            this.cbReloadOnHotkey.Checked = true;
            this.cbReloadOnHotkey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReloadOnHotkey.Name = "cbReloadOnHotkey";
            this.cbReloadOnHotkey.UseVisualStyleBackColor = true;
            this.cbReloadOnHotkey.CheckedChanged += new System.EventHandler(this.cbReloadOnHotkey_CheckedChanged);
            // 
            // nAutoReloadInterval
            // 
            this.nAutoReloadInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.nAutoReloadInterval, "nAutoReloadInterval");
            this.nAutoReloadInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nAutoReloadInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.nAutoReloadInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nAutoReloadInterval.Name = "nAutoReloadInterval";
            this.nAutoReloadInterval.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nAutoReloadInterval.ValueChanged += new System.EventHandler(this.nAutoReloadInterval_ValueChanged);
            // 
            // bReload
            // 
            resources.ApplyResources(this.bReload, "bReload");
            this.bReload.Name = "bReload";
            this.bReload.UseVisualStyleBackColor = true;
            this.bReload.Click += new System.EventHandler(this.bReload_Click);
            // 
            // Label_AutoReloadInterval
            // 
            resources.ApplyResources(this.Label_AutoReloadInterval, "Label_AutoReloadInterval");
            this.Label_AutoReloadInterval.Name = "Label_AutoReloadInterval";
            // 
            // cbAutoReload
            // 
            resources.ApplyResources(this.cbAutoReload, "cbAutoReload");
            this.cbAutoReload.Name = "cbAutoReload";
            this.cbAutoReload.UseVisualStyleBackColor = true;
            this.cbAutoReload.CheckedChanged += new System.EventHandler(this.cbAutoReload_CheckedChanged);
            // 
            // tAutoReload
            // 
            this.tAutoReload.Interval = 1000;
            this.tAutoReload.Tick += new System.EventHandler(this.tAutoReload_Tick);
            // 
            // Form
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.TrayContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.gpSettings.ResumeLayout(false);
            this.gpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeStep)).EndInit();
            this.gbToastNotifications.ResumeLayout(false);
            this.gbToastNotifications.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nToastTimeoutInterval)).EndInit();
            this.panel2SplitContainer.Panel1.ResumeLayout(false);
            this.panel2SplitContainer.Panel1.PerformLayout();
            this.panel2SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panel2SplitContainer)).EndInit();
            this.panel2SplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nAutoReloadInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayContextMenu;
        private ToolStripMenuItem TrayContextMenuClose;
        private ToolStripSeparator TrayContextMenuSeparator;
        private ToolStripMenuItem TrayContextMenuBringToFront;
        private BindingSource bsAudioProcessAPI;
        private VolumeControl.Core.Controls.DoubleBufferedDataGridView Mixer;
        private Button bHotkeyEditor;
        private SplitContainer splitContainer;
        private Label Label_Version;
        private Button bReload;
        private NumericUpDown nAutoReloadInterval;
        private CheckBox cbAutoReload;
        private System.Windows.Forms.Timer tAutoReload;
        private Label Label_AutoReloadInterval;
        private TextBox tbTargetSelector;
        private CheckBox cbLockTarget;
        private CheckBox cbRunAtStartup;
        private CheckBox cbShowInTaskbar;
        private CheckBox cbStartMinimized;
        private CheckBox cbAlwaysOnTop;
        private Button bToggleMixer;
        private CheckBox cbToastEnabled;
        private NumericUpDown nToastTimeoutInterval;
        private GroupBox gbToastNotifications;
        private Label LabelToastTimeout;
        private SplitContainer panel2SplitContainer;
        private GroupBox gpSettings;
        private CheckBox cbReloadOnHotkey;
        private Label Label_ProgramName;
        private NumericUpDown nVolumeStep;
        private Label Label_VolumeStep;
        private DataGridViewTextBoxColumn MixerColPID;
        private DataGridViewTextBoxColumn MixerColProcessName;
        private DataGridViewTextBoxColumn MixerColVolume;
        private DataGridViewCheckBoxColumn MixerColMuted;
        private DataGridViewButtonColumn MixerColSelectButton;
    }
}