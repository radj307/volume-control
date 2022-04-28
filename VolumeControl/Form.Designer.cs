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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.gbNotifications = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cbVolumeIndicatorEnabled = new System.Windows.Forms.CheckBox();
            this.nVolumeIndicatorTimeoutInterval = new System.Windows.Forms.NumericUpDown();
            this.cbToastEnabled = new System.Windows.Forms.CheckBox();
            this.nToastTimeoutInterval = new System.Windows.Forms.NumericUpDown();
            this.tbTargetSelector = new VolumeControl.Core.Controls.CenteredTextBox();
            this.gpSettings = new System.Windows.Forms.GroupBox();
            this.flpSettings = new System.Windows.Forms.FlowLayoutPanel();
            this.cbRunAtStartup = new System.Windows.Forms.CheckBox();
            this.cbStartMinimized = new System.Windows.Forms.CheckBox();
            this.cbShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cbAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.nlVolumeStep = new VolumeControl.Core.Controls.NumericUpDownWithLabel();
            this.bHotkeyEditor = new System.Windows.Forms.Button();
            this.cbLockTarget = new System.Windows.Forms.CheckBox();
            this.bToggleMixer = new System.Windows.Forms.Button();
            this.Label_Version = new System.Windows.Forms.Label();
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
            this.TooltipController = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            this.gbNotifications.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeIndicatorTimeoutInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nToastTimeoutInterval)).BeginInit();
            this.gpSettings.SuspendLayout();
            this.flpSettings.SuspendLayout();
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
            this.MainSplitContainer.Panel1.Controls.Add(this.gbNotifications);
            this.MainSplitContainer.Panel1.Controls.Add(this.tbTargetSelector);
            this.MainSplitContainer.Panel1.Controls.Add(this.gpSettings);
            this.MainSplitContainer.Panel1.Controls.Add(this.cbLockTarget);
            this.MainSplitContainer.Panel1.Controls.Add(this.bToggleMixer);
            this.MainSplitContainer.Panel1.Controls.Add(this.Label_Version);
            this.MainSplitContainer.Panel1.Cursor = System.Windows.Forms.Cursors.Default;
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.MixerSplitContainer);
            this.MainSplitContainer.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.MainSplitContainer.Panel2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.MainSplitContainer.TabStop = false;
            // 
            // gbNotifications
            // 
            this.gbNotifications.Controls.Add(this.flowLayoutPanel1);
            resources.ApplyResources(this.gbNotifications, "gbNotifications");
            this.gbNotifications.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.gbNotifications.Name = "gbNotifications";
            this.gbNotifications.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.cbVolumeIndicatorEnabled);
            this.flowLayoutPanel1.Controls.Add(this.nVolumeIndicatorTimeoutInterval);
            this.flowLayoutPanel1.Controls.Add(this.cbToastEnabled);
            this.flowLayoutPanel1.Controls.Add(this.nToastTimeoutInterval);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // cbVolumeIndicatorEnabled
            // 
            resources.ApplyResources(this.cbVolumeIndicatorEnabled, "cbVolumeIndicatorEnabled");
            this.cbVolumeIndicatorEnabled.Checked = true;
            this.cbVolumeIndicatorEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVolumeIndicatorEnabled.Name = "cbVolumeIndicatorEnabled";
            this.cbVolumeIndicatorEnabled.UseVisualStyleBackColor = true;
            this.cbVolumeIndicatorEnabled.CheckedChanged += new System.EventHandler(this.cbVolumeIndicatorEnabled_CheckedChanged);
            this.cbVolumeIndicatorEnabled.Paint += new System.Windows.Forms.PaintEventHandler(this.cbPaint);
            // 
            // nVolumeIndicatorTimeoutInterval
            // 
            this.nVolumeIndicatorTimeoutInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.nVolumeIndicatorTimeoutInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.nVolumeIndicatorTimeoutInterval, "nVolumeIndicatorTimeoutInterval");
            this.flowLayoutPanel1.SetFlowBreak(this.nVolumeIndicatorTimeoutInterval, true);
            this.nVolumeIndicatorTimeoutInterval.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.nVolumeIndicatorTimeoutInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nVolumeIndicatorTimeoutInterval.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.nVolumeIndicatorTimeoutInterval.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nVolumeIndicatorTimeoutInterval.Name = "nVolumeIndicatorTimeoutInterval";
            this.TooltipController.SetToolTip(this.nVolumeIndicatorTimeoutInterval, resources.GetString("nVolumeIndicatorTimeoutInterval.ToolTip"));
            this.nVolumeIndicatorTimeoutInterval.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nVolumeIndicatorTimeoutInterval.ValueChanged += new System.EventHandler(this.nVolumeIndicatorTimeoutInterval_ValueChanged);
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
            // nToastTimeoutInterval
            // 
            this.nToastTimeoutInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.nToastTimeoutInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.nToastTimeoutInterval, "nToastTimeoutInterval");
            this.nToastTimeoutInterval.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.nToastTimeoutInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            this.TooltipController.SetToolTip(this.nToastTimeoutInterval, resources.GetString("nToastTimeoutInterval.ToolTip"));
            this.nToastTimeoutInterval.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.nToastTimeoutInterval.ValueChanged += new System.EventHandler(this.nToastTimeoutInterval_ValueChanged);
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
            this.tbTargetSelector.SelectionLength = 0;
            this.tbTargetSelector.SelectionStart = 0;
            this.tbTargetSelector.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tbTargetSelector.TextChanged += new System.EventHandler(this.tbTargetName_TextChanged);
            // 
            // gpSettings
            // 
            this.gpSettings.Controls.Add(this.flpSettings);
            resources.ApplyResources(this.gpSettings, "gpSettings");
            this.gpSettings.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.gpSettings.Name = "gpSettings";
            this.gpSettings.TabStop = false;
            // 
            // flpSettings
            // 
            this.flpSettings.Controls.Add(this.cbRunAtStartup);
            this.flpSettings.Controls.Add(this.cbStartMinimized);
            this.flpSettings.Controls.Add(this.cbShowInTaskbar);
            this.flpSettings.Controls.Add(this.cbAlwaysOnTop);
            this.flpSettings.Controls.Add(this.nlVolumeStep);
            this.flpSettings.Controls.Add(this.bHotkeyEditor);
            resources.ApplyResources(this.flpSettings, "flpSettings");
            this.flpSettings.Name = "flpSettings";
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
            // nlVolumeStep
            // 
            this.nlVolumeStep.BackColor = System.Drawing.Color.Transparent;
            this.nlVolumeStep.BoxBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.nlVolumeStep.BoxWidth = 45;
            resources.ApplyResources(this.nlVolumeStep, "nlVolumeStep");
            this.nlVolumeStep.LabelText = "Volume Step";
            this.nlVolumeStep.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nlVolumeStep.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nlVolumeStep.Name = "nlVolumeStep";
            this.TooltipController.SetToolTip(this.nlVolumeStep, resources.GetString("nlVolumeStep.ToolTip"));
            this.nlVolumeStep.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nlVolumeStep.ValueChanged += new System.EventHandler(this.nlVolumeStep_ValueChanged);
            // 
            // bHotkeyEditor
            // 
            this.bHotkeyEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            resources.ApplyResources(this.bHotkeyEditor, "bHotkeyEditor");
            this.bHotkeyEditor.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.bHotkeyEditor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.bHotkeyEditor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.bHotkeyEditor.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.bHotkeyEditor.Name = "bHotkeyEditor";
            this.bHotkeyEditor.UseVisualStyleBackColor = false;
            this.bHotkeyEditor.Click += new System.EventHandler(this.bHotkeyEditor_Click);
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
            this.nAutoReloadInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.nAutoReloadInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.nAutoReloadInterval, "nAutoReloadInterval");
            this.nAutoReloadInterval.ForeColor = System.Drawing.Color.WhiteSmoke;
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
            this.bReload.Click += new System.EventHandler(this.ReloadProcessList);
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
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(65)))), ((int)(((byte)(65)))));
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            this.Mixer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.Mixer.AutoGenerateColumns = false;
            this.Mixer.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.Mixer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Mixer.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.Mixer.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.Mixer.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.Mixer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Mixer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MixerColPID,
            this.MixerColProcessName,
            this.MixerColVolumeDown,
            this.MixerColVolume,
            this.MixerColVolumeUp,
            this.MixerColMuted,
            this.MixerColSelectButton});
            this.Mixer.Cursor = System.Windows.Forms.Cursors.Default;
            this.Mixer.DataSource = this.bsAudioProcessAPI;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle14.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.DefaultCellStyle = dataGridViewCellStyle14;
            resources.ApplyResources(this.Mixer, "Mixer");
            this.Mixer.DoubleBuffered = true;
            this.Mixer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Mixer.EnableHeadersVisualStyles = false;
            this.Mixer.GridColor = System.Drawing.Color.Gainsboro;
            this.Mixer.MultiSelect = false;
            this.Mixer.Name = "Mixer";
            this.Mixer.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle15.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
            this.Mixer.RowHeadersVisible = false;
            this.Mixer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle16.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.RowsDefaultCellStyle = dataGridViewCellStyle16;
            this.Mixer.RowTemplate.Height = 25;
            this.Mixer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Mixer.ShowCellErrors = false;
            this.Mixer.ShowEditingIcon = false;
            this.Mixer.ShowRowErrors = false;
            this.Mixer.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Mixer_CellContentClick);
            this.Mixer.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.Mixer_CellPainting);
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
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.MixerColVolumeDown.DefaultCellStyle = dataGridViewCellStyle11;
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
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle12.Padding = new System.Windows.Forms.Padding(2);
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.MixerColVolumeUp.DefaultCellStyle = dataGridViewCellStyle12;
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
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle13.Padding = new System.Windows.Forms.Padding(2);
            this.MixerColSelectButton.DefaultCellStyle = dataGridViewCellStyle13;
            this.MixerColSelectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
            this.tAutoReload.Tick += new System.EventHandler(this.ReloadProcessList);
            // 
            // TooltipController
            // 
            this.TooltipController.AutomaticDelay = 750;
            this.TooltipController.AutoPopDelay = 3000;
            this.TooltipController.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(55)))));
            this.TooltipController.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.TooltipController.InitialDelay = 750;
            this.TooltipController.OwnerDraw = true;
            this.TooltipController.ReshowDelay = 150;
            this.TooltipController.UseAnimation = false;
            this.TooltipController.Draw += new System.Windows.Forms.DrawToolTipEventHandler(this.TooltipController_Draw);
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
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.Name = "Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Shown += new System.EventHandler(this.Form_Load);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel1.PerformLayout();
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.gbNotifications.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nVolumeIndicatorTimeoutInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nToastTimeoutInterval)).EndInit();
            this.gpSettings.ResumeLayout(false);
            this.flpSettings.ResumeLayout(false);
            this.flpSettings.PerformLayout();
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
        private SplitContainer MixerSplitContainer;
        private GroupBox gpSettings;
        private CheckBox cbReloadOnHotkey;
        private Splitter splitter1;
        private Core.Controls.CenteredTextBox tbTargetSelector;
        private Splitter splitter2;
        private NumericUpDown nToastTimeoutInterval;
        private DataGridViewTextBoxColumn MixerColPID;
        private DataGridViewTextBoxColumn MixerColProcessName;
        private DataGridViewButtonColumn MixerColVolumeDown;
        private DataGridViewTextBoxColumn MixerColVolume;
        private DataGridViewButtonColumn MixerColVolumeUp;
        private DataGridViewCheckBoxColumn MixerColMuted;
        private DataGridViewButtonColumn MixerColSelectButton;
        private CheckBox cbVolumeIndicatorEnabled;
        private NumericUpDown nVolumeIndicatorTimeoutInterval;
        private FlowLayoutPanel flpSettings;
        private NumericUpDownWithLabel nlVolumeStep;
        private GroupBox gbNotifications;
        private FlowLayoutPanel flowLayoutPanel1;
        private ToolTip TooltipController;
    }
}