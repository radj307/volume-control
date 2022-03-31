namespace volume_control_2
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TrayContextMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.TrayContextMenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.TrayContextMenuBringToFront = new System.Windows.Forms.ToolStripMenuItem();
            this.bsAudioProcessAPI = new System.Windows.Forms.BindingSource(this.components);
            this.Mixer = new CoreControls.DoubleBufferedDataGridView();
            this.MixerColPID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MixerColMuted = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.MixerColSelectButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bHotkeyEditor = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cbAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.cbShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cbStartMinimized = new System.Windows.Forms.CheckBox();
            this.cbRunAtStartup = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbLockTarget = new System.Windows.Forms.CheckBox();
            this.Label_Target = new System.Windows.Forms.Label();
            this.tbTargetSelector = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nAutoReloadInterval = new System.Windows.Forms.NumericUpDown();
            this.bReload = new System.Windows.Forms.Button();
            this.Label_AutoReloadInterval = new System.Windows.Forms.Label();
            this.cbAutoReload = new System.Windows.Forms.CheckBox();
            this.Label_Version = new System.Windows.Forms.Label();
            this.tAutoReload = new System.Windows.Forms.Timer(this.components);
            this.TrayContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nAutoReloadInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = this.TrayContextMenu;
            resources.ApplyResources(this.TrayIcon, "TrayIcon");
            // 
            // TrayContextMenu
            // 
            this.TrayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TrayContextMenuClose,
            this.TrayContextMenuSeparator,
            this.TrayContextMenuBringToFront});
            this.TrayContextMenu.Name = "TrayContextMenu";
            this.TrayContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            resources.ApplyResources(this.TrayContextMenu, "TrayContextMenu");
            this.TrayContextMenu.TabStop = true;
            // 
            // TrayContextMenuClose
            // 
            this.TrayContextMenuClose.Name = "TrayContextMenuClose";
            resources.ApplyResources(this.TrayContextMenuClose, "TrayContextMenuClose");
            // 
            // TrayContextMenuSeparator
            // 
            this.TrayContextMenuSeparator.Name = "TrayContextMenuSeparator";
            resources.ApplyResources(this.TrayContextMenuSeparator, "TrayContextMenuSeparator");
            // 
            // TrayContextMenuBringToFront
            // 
            this.TrayContextMenuBringToFront.Name = "TrayContextMenuBringToFront";
            resources.ApplyResources(this.TrayContextMenuBringToFront, "TrayContextMenuBringToFront");
            // 
            // bsAudioProcessAPI
            // 
            this.bsAudioProcessAPI.DataMember = "ProcessList";
            this.bsAudioProcessAPI.DataSource = typeof(Core.AudioProcessAPI);
            this.bsAudioProcessAPI.Sort = "";
            // 
            // Mixer
            // 
            this.Mixer.AllowUserToAddRows = false;
            this.Mixer.AllowUserToDeleteRows = false;
            this.Mixer.AllowUserToOrderColumns = true;
            this.Mixer.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.Mixer.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.Mixer.AutoGenerateColumns = false;
            this.Mixer.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Mixer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Mixer.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.Mixer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Mixer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MixerColPID,
            this.MixerColProcessName,
            this.MixerColDisplayName,
            this.MixerColVolume,
            this.MixerColMuted,
            this.MixerColSelectButton});
            this.Mixer.DataSource = this.bsAudioProcessAPI;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Mixer.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.Mixer, "Mixer");
            this.Mixer.DoubleBuffered = true;
            this.Mixer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Mixer.MultiSelect = false;
            this.Mixer.Name = "Mixer";
            this.Mixer.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.Mixer.RowHeadersVisible = false;
            this.Mixer.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.Mixer.RowTemplate.Height = 25;
            this.Mixer.ShowEditingIcon = false;
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
            // MixerColDisplayName
            // 
            this.MixerColDisplayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MixerColDisplayName.DataPropertyName = "DisplayName";
            resources.ApplyResources(this.MixerColDisplayName, "MixerColDisplayName");
            this.MixerColDisplayName.Name = "MixerColDisplayName";
            this.MixerColDisplayName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.Arrow;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cbAlwaysOnTop);
            this.splitContainer1.Panel1.Controls.Add(this.cbShowInTaskbar);
            this.splitContainer1.Panel1.Controls.Add(this.cbStartMinimized);
            this.splitContainer1.Panel1.Controls.Add(this.cbRunAtStartup);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.Label_Version);
            this.splitContainer1.Panel1.Controls.Add(this.bHotkeyEditor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Mixer);
            // 
            // cbAlwaysOnTop
            // 
            resources.ApplyResources(this.cbAlwaysOnTop, "cbAlwaysOnTop");
            this.cbAlwaysOnTop.Name = "cbAlwaysOnTop";
            this.cbAlwaysOnTop.UseVisualStyleBackColor = true;
            this.cbAlwaysOnTop.CheckedChanged += new System.EventHandler(this.cbAlwaysOnTop_CheckedChanged);
            // 
            // cbShowInTaskbar
            // 
            resources.ApplyResources(this.cbShowInTaskbar, "cbShowInTaskbar");
            this.cbShowInTaskbar.Name = "cbShowInTaskbar";
            this.cbShowInTaskbar.UseVisualStyleBackColor = true;
            this.cbShowInTaskbar.CheckedChanged += new System.EventHandler(this.cbShowInTaskbar_CheckedChanged);
            // 
            // cbStartMinimized
            // 
            resources.ApplyResources(this.cbStartMinimized, "cbStartMinimized");
            this.cbStartMinimized.Name = "cbStartMinimized";
            this.cbStartMinimized.UseVisualStyleBackColor = true;
            // 
            // cbRunAtStartup
            // 
            resources.ApplyResources(this.cbRunAtStartup, "cbRunAtStartup");
            this.cbRunAtStartup.Name = "cbRunAtStartup";
            this.cbRunAtStartup.UseVisualStyleBackColor = true;
            this.cbRunAtStartup.CheckedChanged += new System.EventHandler(this.cbRunAtStartup_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cbLockTarget);
            this.panel2.Controls.Add(this.Label_Target);
            this.panel2.Controls.Add(this.tbTargetSelector);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // cbLockTarget
            // 
            resources.ApplyResources(this.cbLockTarget, "cbLockTarget");
            this.cbLockTarget.Name = "cbLockTarget";
            this.cbLockTarget.UseVisualStyleBackColor = true;
            this.cbLockTarget.CheckedChanged += new System.EventHandler(this.cbLockTarget_CheckedChanged);
            // 
            // Label_Target
            // 
            resources.ApplyResources(this.Label_Target, "Label_Target");
            this.Label_Target.Name = "Label_Target";
            // 
            // tbTargetSelector
            // 
            this.tbTargetSelector.AllowDrop = true;
            this.tbTargetSelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            resources.ApplyResources(this.tbTargetSelector, "tbTargetSelector");
            this.tbTargetSelector.Name = "tbTargetSelector";
            this.tbTargetSelector.TextChanged += new System.EventHandler(this.tbTargetName_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nAutoReloadInterval);
            this.panel1.Controls.Add(this.bReload);
            this.panel1.Controls.Add(this.Label_AutoReloadInterval);
            this.panel1.Controls.Add(this.cbAutoReload);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // nAutoReloadInterval
            // 
            this.nAutoReloadInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nAutoReloadInterval.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.nAutoReloadInterval, "nAutoReloadInterval");
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
            // Label_Version
            // 
            resources.ApplyResources(this.Label_Version, "Label_Version");
            this.Label_Version.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Label_Version.Name = "Label_Version";
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
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Form";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.TrayContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bsAudioProcessAPI)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Mixer)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private CoreControls.DoubleBufferedDataGridView Mixer;
        private Button bHotkeyEditor;
        private SplitContainer splitContainer1;
        private Label Label_Version;
        private Button bReload;
        private NumericUpDown nAutoReloadInterval;
        private CheckBox cbAutoReload;
        private System.Windows.Forms.Timer tAutoReload;
        private Label Label_AutoReloadInterval;
        private Panel panel1;
        private TextBox tbTargetSelector;
        private Panel panel2;
        private Label Label_Target;
        private DataGridViewTextBoxColumn MixerColPID;
        private DataGridViewTextBoxColumn MixerColProcessName;
        private DataGridViewTextBoxColumn MixerColDisplayName;
        private DataGridViewTextBoxColumn MixerColVolume;
        private DataGridViewCheckBoxColumn MixerColMuted;
        private DataGridViewButtonColumn MixerColSelectButton;
        private CheckBox cbLockTarget;
        private CheckBox cbRunAtStartup;
        private CheckBox cbShowInTaskbar;
        private CheckBox cbStartMinimized;
        private CheckBox cbAlwaysOnTop;
    }
}