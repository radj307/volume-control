using VolumeControl.Core;
using VolumeControl.Core.Controls;

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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tbTargetSelector = new VolumeControl.Core.Controls.CenteredTextBox();
            this.gpSettings = new System.Windows.Forms.GroupBox();
            this.Label_VolumeStep = new System.Windows.Forms.Label();
            this.nVolumeStep = new System.Windows.Forms.NumericUpDown();
            this.cbRunAtStartup = new System.Windows.Forms.CheckBox();
            this.cbStartMinimized = new System.Windows.Forms.CheckBox();
            this.cbShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cbAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.cbLockTarget = new System.Windows.Forms.CheckBox();
            this.gbToastNotifications = new System.Windows.Forms.GroupBox();
            this.nToastTimeoutInterval = new VolumeControl.Core.Controls.CenteredNumericUpDown();
            this.LabelToastTimeout = new System.Windows.Forms.Label();
            this.cbToastEnabled = new System.Windows.Forms.CheckBox();
            this.bToggleMixer = new System.Windows.Forms.Button();
            this.Label_Version = new System.Windows.Forms.Label();
            this.bHotkeyEditor = new System.Windows.Forms.Button();
            this.MixerSplitContainer = new System.Windows.Forms.SplitContainer();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.cbReloadOnHotkey = new System.Windows.Forms.CheckBox();
            this.nAutoReloadInterval = new System.Windows.Forms.NumericUpDown();
            this.bReload = new System.Windows.Forms.Button();
            this.Label_AutoReloadInterval = new System.Windows.Forms.Label();
            this.cbAutoReload = new System.Windows.Forms.CheckBox();
            this.Mixer = new VolumeControl.Core.Controls.DoubleBufferedDataGridView();
            this.MixerColPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColVolumeDown = new System.Windows.Forms.DataGridViewButtonColumn();
            this.MixerColVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColVolumeUp = new System.Windows.Forms.DataGridViewButtonColumn();
            this.MixerColMuted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MixerColSelectButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bsAudioProcessAPI = new System.Windows.Forms.BindingSource(this.components);
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TrayContextMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.TrayContextMenuBringToFront = new System.Windows.Forms.ToolStripMenuItem();
            this.tAutoReload = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.gpSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeStep)).BeginInit();
            this.gbToastNotifications.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MixerSplitContainer)).BeginInit();
            this.MixerSplitContainer.Panel1.SuspendLayout();
            this.MixerSplitContainer.Panel2.SuspendLayout();
            this.MixerSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nAutoReloadInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).BeginInit();
            this.TrayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.BackColor = System.Drawing.Color.Transparent;
            this.MainSplitContainer.Cursor = System.Windows.Forms.Cursors.HSplit;
            resources.ApplyResources(this.MainSplitContainer, "MainSplitContainer");
            this.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.BackgroundImage = global::VolumeControl.Properties.Resources.background_img1;
            resources.ApplyResources(this.MainSplitContainer.Panel1, "MainSplitContainer.Panel1");
            this.MainSplitContainer.Panel1.Controls.Add(this.tbTargetSelector);
            this.MainSplitContainer.Panel1.Controls.Add(this.gpSettings);
            this.MainSplitContainer.Panel1.Controls.Add(this.cbLockTarget);
            this.MainSplitContainer.Panel1.Controls.Add(this.gbToastNotifications);
            this.MainSplitContainer.Panel1.Controls.Add(this.bToggleMixer);
            this.MainSplitContainer.Panel1.Controls.Add(this.Label_Version);
            this.MainSplitContainer.Panel1.Controls.Add(this.bHotkeyEditor);
            this.MainSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.MixerSplitContainer);
            this.MainSplitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.MainSplitContainer.Panel2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.MainSplitContainer.TabStop = false;
            // 
            // tbTargetSelector
            // 
            this.tbTargetSelector.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.tbTargetSelector.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbTargetSelector.EdgePadding = 3;
            this.tbTargetSelector.ForeColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.tbTargetSelector, "tbTargetSelector");
            this.tbTargetSelector.Name = "tbTargetSelector";
            this.tbTargetSelector.PlaceholderText = "Target Process Name";
            this.tbTargetSelector.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tbTargetSelector.TextChanged += new System.EventHandler(this.tbTargetName_TextChanged);
            // 
            // gpSettings
            // 
            this.gpSettings.Controls.Add(this.Label_VolumeStep);
            this.gpSettings.Controls.Add(this.nVolumeStep);
            this.gpSettings.Controls.Add(this.cbRunAtStartup);
            this.gpSettings.Controls.Add(this.cbStartMinimized);
            this.gpSettings.Controls.Add(this.cbShowInTaskbar);
            this.gpSettings.Controls.Add(this.cbAlwaysOnTop);
            this.gpSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
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
            this.nVolumeStep.BackColor = System.Drawing.Color.Gainsboro;
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
            this.cbRunAtStartup.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // cbStartMinimized
            // 
            resources.ApplyResources(this.cbStartMinimized, "cbStartMinimized");
            this.cbStartMinimized.Name = "cbStartMinimized";
            this.cbStartMinimized.UseVisualStyleBackColor = true;
            this.cbStartMinimized.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // cbShowInTaskbar
            // 
            resources.ApplyResources(this.cbShowInTaskbar, "cbShowInTaskbar");
            this.cbShowInTaskbar.Checked = true;
            this.cbShowInTaskbar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowInTaskbar.Name = "cbShowInTaskbar";
            this.cbShowInTaskbar.UseVisualStyleBackColor = true;
            this.cbShowInTaskbar.CheckedChanged += new System.EventHandler(this.cbShowInTaskbar_CheckedChanged);
            this.cbShowInTaskbar.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // cbAlwaysOnTop
            // 
            resources.ApplyResources(this.cbAlwaysOnTop, "cbAlwaysOnTop");
            this.cbAlwaysOnTop.FlatAppearance.BorderSize = 0;
            this.cbAlwaysOnTop.Name = "cbAlwaysOnTop";
            this.cbAlwaysOnTop.UseVisualStyleBackColor = false;
            this.cbAlwaysOnTop.CheckedChanged += new System.EventHandler(this.cbAlwaysOnTop_CheckedChanged);
            this.cbAlwaysOnTop.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // cbLockTarget
            // 
            resources.ApplyResources(this.cbLockTarget, "cbLockTarget");
            this.cbLockTarget.FlatAppearance.BorderSize = 0;
            this.cbLockTarget.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cbLockTarget.Name = "cbLockTarget";
            this.cbLockTarget.UseVisualStyleBackColor = false;
            this.cbLockTarget.CheckedChanged += new System.EventHandler(this.cbLockTarget_CheckedChanged);
            this.cbLockTarget.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // gbToastNotifications
            // 
            this.gbToastNotifications.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.gbToastNotifications, "gbToastNotifications");
            this.gbToastNotifications.Controls.Add(this.nToastTimeoutInterval);
            this.gbToastNotifications.Controls.Add(this.LabelToastTimeout);
            this.gbToastNotifications.Controls.Add(this.cbToastEnabled);
            this.gbToastNotifications.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbToastNotifications.ForeColor = System.Drawing.Color.Gainsboro;
            this.gbToastNotifications.Name = "gbToastNotifications";
            this.gbToastNotifications.TabStop = false;
            // 
            // nToastTimeoutInterval
            // 
            this.nToastTimeoutInterval.AllowDecimalPlaces = false;
            this.nToastTimeoutInterval.AllowSeparators = false;
            this.nToastTimeoutInterval.BackColorSplitter = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.nToastTimeoutInterval.BackColorTextBox = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.nToastTimeoutInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nToastTimeoutInterval.FlatBorderColorDown = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.nToastTimeoutInterval.FlatBorderColorUp = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.nToastTimeoutInterval.FlatBorderSizeDown = 1;
            this.nToastTimeoutInterval.FlatBorderSizeUp = 1;
            this.nToastTimeoutInterval.FlatMouseDownBackColorDown = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.nToastTimeoutInterval.FlatMouseDownBackColorUp = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.nToastTimeoutInterval.FlatMouseOverBackColorDown = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nToastTimeoutInterval.FlatMouseOverBackColorUp = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.nToastTimeoutInterval.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nToastTimeoutInterval.ForeColor = System.Drawing.Color.Transparent;
            this.nToastTimeoutInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nToastTimeoutInterval, "nToastTimeoutInterval");
            this.nToastTimeoutInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.nToastTimeoutInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nToastTimeoutInterval.Name = "nToastTimeoutInterval";
            this.nToastTimeoutInterval.PlaceholderText = "";
            this.nToastTimeoutInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nToastTimeoutInterval.ValueChanged += new System.EventHandler(this.nToastTimeoutInterval_ValueChanged);
            // 
            // LabelToastTimeout
            // 
            resources.ApplyResources(this.LabelToastTimeout, "LabelToastTimeout");
            this.LabelToastTimeout.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.LabelToastTimeout.Name = "LabelToastTimeout";
            // 
            // cbToastEnabled
            // 
            resources.ApplyResources(this.cbToastEnabled, "cbToastEnabled");
            this.cbToastEnabled.Checked = true;
            this.cbToastEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbToastEnabled.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cbToastEnabled.Name = "cbToastEnabled";
            this.cbToastEnabled.UseVisualStyleBackColor = true;
            this.cbToastEnabled.CheckedChanged += new System.EventHandler(this.cbToastEnabled_CheckedChanged);
            this.cbToastEnabled.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // bToggleMixer
            // 
            this.bToggleMixer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.bToggleMixer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.bToggleMixer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.bToggleMixer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.bToggleMixer, "bToggleMixer");
            this.bToggleMixer.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.bToggleMixer.Name = "bToggleMixer";
            this.bToggleMixer.UseVisualStyleBackColor = false;
            this.bToggleMixer.Click += new System.EventHandler(this.bToggleMixer_Click);
            // 
            // Label_Version
            // 
            resources.ApplyResources(this.Label_Version, "Label_Version");
            this.Label_Version.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Label_Version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.Label_Version.Name = "Label_Version";
            // 
            // bHotkeyEditor
            // 
            this.bHotkeyEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.bHotkeyEditor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.bHotkeyEditor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.bHotkeyEditor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.bHotkeyEditor, "bHotkeyEditor");
            this.bHotkeyEditor.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.bHotkeyEditor.Name = "bHotkeyEditor";
            this.bHotkeyEditor.UseVisualStyleBackColor = false;
            this.bHotkeyEditor.Click += new System.EventHandler(this.bHotkeyEditor_Click);
            // 
            // MixerSplitContainer
            // 
            this.MixerSplitContainer.Cursor = System.Windows.Forms.Cursors.HSplit;
            resources.ApplyResources(this.MixerSplitContainer, "MixerSplitContainer");
            this.MixerSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MixerSplitContainer.Name = "MixerSplitContainer";
            // 
            // MixerSplitContainer.Panel1
            // 
            this.MixerSplitContainer.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.MixerSplitContainer.Panel1.Controls.Add(this.splitter2);
            this.MixerSplitContainer.Panel1.Controls.Add(this.splitter1);
            this.MixerSplitContainer.Panel1.Controls.Add(this.cbReloadOnHotkey);
            this.MixerSplitContainer.Panel1.Controls.Add(this.nAutoReloadInterval);
            this.MixerSplitContainer.Panel1.Controls.Add(this.bReload);
            this.MixerSplitContainer.Panel1.Controls.Add(this.Label_AutoReloadInterval);
            this.MixerSplitContainer.Panel1.Controls.Add(this.cbAutoReload);
            this.MixerSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // MixerSplitContainer.Panel2
            // 
            this.MixerSplitContainer.Panel2.Controls.Add(this.Mixer);
            this.MixerSplitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.MixerSplitContainer.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.Color.DarkGray;
            this.splitter2.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.splitter2, "splitter2");
            this.splitter2.Name = "splitter2";
            this.splitter2.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.DarkGray;
            this.splitter1.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // cbReloadOnHotkey
            // 
            resources.ApplyResources(this.cbReloadOnHotkey, "cbReloadOnHotkey");
            this.cbReloadOnHotkey.Checked = true;
            this.cbReloadOnHotkey.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbReloadOnHotkey.Name = "cbReloadOnHotkey";
            this.cbReloadOnHotkey.UseVisualStyleBackColor = true;
            this.cbReloadOnHotkey.CheckedChanged += new System.EventHandler(this.cbReloadOnHotkey_CheckedChanged);
            this.cbReloadOnHotkey.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // nAutoReloadInterval
            // 
            this.nAutoReloadInterval.BackColor = System.Drawing.Color.Gainsboro;
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
            this.bReload.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.bReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.bReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.bReload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.bReload, "bReload");
            this.bReload.Name = "bReload";
            this.bReload.UseVisualStyleBackColor = false;
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
            this.cbAutoReload.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // Mixer
            // 
            this.Mixer.AllowUserToAddRows = false;
            this.Mixer.AllowUserToDeleteRows = false;
            this.Mixer.AllowUserToOrderColumns = true;
            this.Mixer.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(99)))), ((int)(((byte)(99)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.Mixer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.Mixer.AutoGenerateColumns = false;
            this.Mixer.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.Mixer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Mixer.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.Mixer.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.Mixer.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.DimGray;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Mixer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Mixer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MixerColPID,
            this.MixerColProcessName,
            this.MixerColVolumeDown,
            this.MixerColVolume,
            this.MixerColVolumeUp,
            this.MixerColMuted,
            this.MixerColSelectButton});
            this.Mixer.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.Mixer.DataSource = this.bsAudioProcessAPI;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(99)))), ((int)(((byte)(99)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.Mixer, "Mixer");
            this.Mixer.DoubleBuffered = true;
            this.Mixer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Mixer.EnableHeadersVisualStyles = false;
            this.Mixer.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.Mixer.MultiSelect = false;
            this.Mixer.Name = "Mixer";
            this.Mixer.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(99)))), ((int)(((byte)(99)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.Mixer.RowHeadersVisible = false;
            this.Mixer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(99)))), ((int)(((byte)(99)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.Mixer.RowTemplate.Height = 25;
            this.Mixer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
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
            // 
            // MixerColProcessName
            // 
            this.MixerColProcessName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.MixerColProcessName.DataPropertyName = "ProcessName";
            resources.ApplyResources(this.MixerColProcessName, "MixerColProcessName");
            this.MixerColProcessName.Name = "MixerColProcessName";
            this.MixerColProcessName.ReadOnly = true;
            // 
            // MixerColVolumeDown
            // 
            this.MixerColVolumeDown.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MixerColVolumeDown.DefaultCellStyle = dataGridViewCellStyle3;
            this.MixerColVolumeDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.MixerColVolumeDown, "MixerColVolumeDown");
            this.MixerColVolumeDown.Name = "MixerColVolumeDown";
            this.MixerColVolumeDown.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MixerColVolumeDown.Text = "-";
            this.MixerColVolumeDown.UseColumnTextForButtonValue = true;
            // 
            // MixerColVolume
            // 
            this.MixerColVolume.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.MixerColVolume.DataPropertyName = "VolumePercent";
            resources.ApplyResources(this.MixerColVolume, "MixerColVolume");
            this.MixerColVolume.MaxInputLength = 4;
            this.MixerColVolume.Name = "MixerColVolume";
            // 
            // MixerColVolumeUp
            // 
            this.MixerColVolumeUp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.MixerColVolumeUp.DefaultCellStyle = dataGridViewCellStyle4;
            this.MixerColVolumeUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.MixerColVolumeUp, "MixerColVolumeUp");
            this.MixerColVolumeUp.Name = "MixerColVolumeUp";
            this.MixerColVolumeUp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MixerColVolumeUp.Text = "+";
            this.MixerColVolumeUp.UseColumnTextForButtonValue = true;
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
            // bsAudioProcessAPI
            // 
            this.bsAudioProcessAPI.DataMember = "ProcessList";
            this.bsAudioProcessAPI.DataSource = typeof(VolumeControl.Core.AudioProcessAPI);
            this.bsAudioProcessAPI.Sort = "";
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
            // tAutoReload
            // 
            this.tAutoReload.Interval = 1000;
            this.tAutoReload.Tick += new System.EventHandler(this.tAutoReload_Tick);
            // 
            // Form
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.Controls.Add(this.MainSplitContainer);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel1.PerformLayout();
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.gpSettings.ResumeLayout(false);
            this.gpSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeStep)).EndInit();
            this.gbToastNotifications.ResumeLayout(false);
            this.gbToastNotifications.PerformLayout();
            this.MixerSplitContainer.Panel1.ResumeLayout(false);
            this.MixerSplitContainer.Panel1.PerformLayout();
            this.MixerSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MixerSplitContainer)).EndInit();
            this.MixerSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nAutoReloadInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).EndInit();
            this.TrayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayContextMenu;
        private ToolStripMenuItem TrayContextMenuClose;
        private ToolStripSeparator TrayContextMenuSeparator;
        private ToolStripMenuItem TrayContextMenuBringToFront;
        private BindingSource bsAudioProcessAPI;
        private DoubleBufferedDataGridView Mixer;
        private Button bHotkeyEditor;
        private SplitContainer MainSplitContainer;
        private Label Label_Version;
        private Button bReload;
        private NumericUpDown nAutoReloadInterval;
        private CheckBox cbAutoReload;
        private System.Windows.Forms.Timer tAutoReload;
        private Label Label_AutoReloadInterval;
        private CheckBox cbLockTarget;
        private CheckBox cbRunAtStartup;
        private CheckBox cbShowInTaskbar;
        private CheckBox cbStartMinimized;
        private CheckBox cbAlwaysOnTop;
        private Button bToggleMixer;
        private CheckBox cbToastEnabled;
        private GroupBox gbToastNotifications;
        private Label LabelToastTimeout;
        private SplitContainer MixerSplitContainer;
        private GroupBox gpSettings;
        private CheckBox cbReloadOnHotkey;
        private NumericUpDown nVolumeStep;
        private Label Label_VolumeStep;
        private Splitter splitter1;
        private Core.Controls.CenteredTextBox tbTargetSelector;
        private DataGridViewTextBoxColumn MixerColPID;
        private DataGridViewTextBoxColumn MixerColProcessName;
        private DataGridViewButtonColumn MixerColVolumeDown;
        private DataGridViewTextBoxColumn MixerColVolume;
        private DataGridViewButtonColumn MixerColVolumeUp;
        private DataGridViewCheckBoxColumn MixerColMuted;
        private DataGridViewButtonColumn MixerColSelectButton;
        private Splitter splitter2;
        private CenteredNumericUpDown nToastTimeoutInterval;
    }
}