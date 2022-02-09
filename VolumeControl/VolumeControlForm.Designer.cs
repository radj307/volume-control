using HotkeyLib;

namespace VolumeControl
{
    partial class VolumeControlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolumeControlForm));
            this.SystemTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.SystemTray_ContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkbox_minimizeOnStartup = new System.Windows.Forms.CheckBox();
            this.ComboBox_ProcessSelector = new System.Windows.Forms.ComboBox();
            this.BindingSource_KeyList = new System.Windows.Forms.BindingSource(this.components);
            this.HKEdit_VolumeUp = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeDown = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeMute = new UIComposites.HotkeyEditor();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Tab_General = new System.Windows.Forms.TabPage();
            this.CheckBox_RunOnStartup = new System.Windows.Forms.CheckBox();
            this.CheckBox_VisibleInTaskbar = new System.Windows.Forms.CheckBox();
            this.Icon_VolumeControl = new System.Windows.Forms.PictureBox();
            this.Label_VersionNumber = new System.Windows.Forms.Label();
            this.Label_VolumeControl = new System.Windows.Forms.Label();
            this.GroupBox_Volume = new System.Windows.Forms.GroupBox();
            this.Label_VolumeStep = new System.Windows.Forms.Label();
            this.Numeric_VolumeStep = new System.Windows.Forms.NumericUpDown();
            this.GroupBox_TargetProcess = new System.Windows.Forms.GroupBox();
            this.Button_ReloadProcessList = new System.Windows.Forms.Button();
            this.Tab_Hotkeys_Volume = new System.Windows.Forms.TabPage();
            this.Tab_Hotkeys_Playback = new System.Windows.Forms.TabPage();
            this.HKEdit_TogglePlayback = new UIComposites.HotkeyEditor();
            this.HKEdit_Next = new UIComposites.HotkeyEditor();
            this.HKEdit_Prev = new UIComposites.HotkeyEditor();
            this.SystemTray_ContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource_KeyList)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Tab_General.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon_VolumeControl)).BeginInit();
            this.GroupBox_Volume.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_VolumeStep)).BeginInit();
            this.GroupBox_TargetProcess.SuspendLayout();
            this.Tab_Hotkeys_Volume.SuspendLayout();
            this.Tab_Hotkeys_Playback.SuspendLayout();
            this.SuspendLayout();
            // 
            // SystemTray
            // 
            this.SystemTray.BalloonTipText = "Application-Specific Volume Control Hotkeys";
            this.SystemTray.BalloonTipTitle = "Volume Control";
            this.SystemTray.ContextMenuStrip = this.SystemTray_ContextMenu;
            this.SystemTray.Icon = ((System.Drawing.Icon)(resources.GetObject("SystemTray.Icon")));
            this.SystemTray.Text = "Volume Control";
            this.SystemTray.Visible = true;
            this.SystemTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.SystemTray_DoubleClick);
            // 
            // SystemTray_ContextMenu
            // 
            this.SystemTray_ContextMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SystemTray_ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.SystemTray_ContextMenu.Name = "system_tray_menu";
            this.SystemTray_ContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.SystemTray_ContextMenu.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.SystemTray_ContextMenu_Close);
            // 
            // checkbox_minimizeOnStartup
            // 
            this.checkbox_minimizeOnStartup.AutoSize = true;
            this.checkbox_minimizeOnStartup.Checked = true;
            this.checkbox_minimizeOnStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkbox_minimizeOnStartup.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkbox_minimizeOnStartup.Location = new System.Drawing.Point(214, 40);
            this.checkbox_minimizeOnStartup.Name = "checkbox_minimizeOnStartup";
            this.checkbox_minimizeOnStartup.Size = new System.Drawing.Size(140, 20);
            this.checkbox_minimizeOnStartup.TabIndex = 5;
            this.checkbox_minimizeOnStartup.Text = "Minimize on Startup";
            this.checkbox_minimizeOnStartup.UseVisualStyleBackColor = true;
            this.checkbox_minimizeOnStartup.CheckedChanged += new System.EventHandler(this.Checkbox_MinimizeOnStartup_CheckedChanged);
            // 
            // ComboBox_ProcessSelector
            // 
            this.ComboBox_ProcessSelector.DataSource = this.BindingSource_KeyList;
            this.ComboBox_ProcessSelector.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ComboBox_ProcessSelector.Location = new System.Drawing.Point(6, 25);
            this.ComboBox_ProcessSelector.MaxDropDownItems = 16;
            this.ComboBox_ProcessSelector.Name = "ComboBox_ProcessSelector";
            this.ComboBox_ProcessSelector.Size = new System.Drawing.Size(260, 24);
            this.ComboBox_ProcessSelector.TabIndex = 6;
            this.ComboBox_ProcessSelector.TextChanged += new System.EventHandler(this.ComboBox_ProcessName_TextChanged);
            // 
            // BindingSource_KeyList
            // 
            this.BindingSource_KeyList.DataMember = "List";
            this.BindingSource_KeyList.DataSource = typeof(HotkeyLib.KeyList);
            // 
            // HKEdit_VolumeUp
            // 
            this.HKEdit_VolumeUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeUp.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_VolumeUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeUp.Name = "HKEdit_VolumeUp";
            this.HKEdit_VolumeUp.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeUp.TabIndex = 7;
            this.HKEdit_VolumeUp.Tag = "";
            // 
            // HKEdit_VolumeDown
            // 
            this.HKEdit_VolumeDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeDown.Location = new System.Drawing.Point(9, 79);
            this.HKEdit_VolumeDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeDown.Name = "HKEdit_VolumeDown";
            this.HKEdit_VolumeDown.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeDown.TabIndex = 8;
            this.HKEdit_VolumeDown.Tag = "";
            // 
            // HKEdit_VolumeMute
            // 
            this.HKEdit_VolumeMute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeMute.Location = new System.Drawing.Point(9, 151);
            this.HKEdit_VolumeMute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeMute.Name = "HKEdit_VolumeMute";
            this.HKEdit_VolumeMute.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeMute.TabIndex = 9;
            this.HKEdit_VolumeMute.Tag = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Tab_General);
            this.tabControl1.Controls.Add(this.Tab_Hotkeys_Volume);
            this.tabControl1.Controls.Add(this.Tab_Hotkeys_Playback);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(368, 253);
            this.tabControl1.TabIndex = 10;
            // 
            // Tab_General
            // 
            this.Tab_General.Controls.Add(this.CheckBox_RunOnStartup);
            this.Tab_General.Controls.Add(this.CheckBox_VisibleInTaskbar);
            this.Tab_General.Controls.Add(this.checkbox_minimizeOnStartup);
            this.Tab_General.Controls.Add(this.Icon_VolumeControl);
            this.Tab_General.Controls.Add(this.Label_VersionNumber);
            this.Tab_General.Controls.Add(this.Label_VolumeControl);
            this.Tab_General.Controls.Add(this.GroupBox_Volume);
            this.Tab_General.Controls.Add(this.GroupBox_TargetProcess);
            this.Tab_General.Location = new System.Drawing.Point(4, 25);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(360, 224);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            this.Tab_General.UseVisualStyleBackColor = true;
            // 
            // CheckBox_RunOnStartup
            // 
            this.CheckBox_RunOnStartup.AutoSize = true;
            this.CheckBox_RunOnStartup.Location = new System.Drawing.Point(214, 14);
            this.CheckBox_RunOnStartup.Name = "CheckBox_RunOnStartup";
            this.CheckBox_RunOnStartup.Size = new System.Drawing.Size(111, 20);
            this.CheckBox_RunOnStartup.TabIndex = 14;
            this.CheckBox_RunOnStartup.Text = "Run on Startup";
            this.CheckBox_RunOnStartup.UseVisualStyleBackColor = true;
            this.CheckBox_RunOnStartup.CheckedChanged += new System.EventHandler(this.CheckBox_RunOnStartup_CheckedChanged);
            // 
            // CheckBox_VisibleInTaskbar
            // 
            this.CheckBox_VisibleInTaskbar.AutoSize = true;
            this.CheckBox_VisibleInTaskbar.Cursor = System.Windows.Forms.Cursors.Default;
            this.CheckBox_VisibleInTaskbar.Location = new System.Drawing.Point(214, 66);
            this.CheckBox_VisibleInTaskbar.Name = "CheckBox_VisibleInTaskbar";
            this.CheckBox_VisibleInTaskbar.Size = new System.Drawing.Size(130, 20);
            this.CheckBox_VisibleInTaskbar.TabIndex = 13;
            this.CheckBox_VisibleInTaskbar.Text = "Visible in Taskbar";
            this.CheckBox_VisibleInTaskbar.UseVisualStyleBackColor = true;
            this.CheckBox_VisibleInTaskbar.CheckedChanged += new System.EventHandler(this.CheckBox_VisibleInTaskbar_CheckedChanged);
            // 
            // Icon_VolumeControl
            // 
            this.Icon_VolumeControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Icon_VolumeControl.ErrorImage = null;
            this.Icon_VolumeControl.Image = global::VolumeControl.Properties.Resources.Icon_Refresh_PNG;
            this.Icon_VolumeControl.InitialImage = null;
            this.Icon_VolumeControl.Location = new System.Drawing.Point(8, 21);
            this.Icon_VolumeControl.Name = "Icon_VolumeControl";
            this.Icon_VolumeControl.Size = new System.Drawing.Size(50, 50);
            this.Icon_VolumeControl.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Icon_VolumeControl.TabIndex = 12;
            this.Icon_VolumeControl.TabStop = false;
            // 
            // Label_VersionNumber
            // 
            this.Label_VersionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Label_VersionNumber.AutoSize = true;
            this.Label_VersionNumber.Location = new System.Drawing.Point(71, 46);
            this.Label_VersionNumber.Name = "Label_VersionNumber";
            this.Label_VersionNumber.Size = new System.Drawing.Size(64, 16);
            this.Label_VersionNumber.TabIndex = 11;
            this.Label_VersionNumber.Text = "[ version ]";
            this.Label_VersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label_VolumeControl
            // 
            this.Label_VolumeControl.AutoSize = true;
            this.Label_VolumeControl.Font = new System.Drawing.Font("Lucida Sans Unicode", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeControl.Location = new System.Drawing.Point(71, 29);
            this.Label_VolumeControl.Name = "Label_VolumeControl";
            this.Label_VolumeControl.Size = new System.Drawing.Size(130, 17);
            this.Label_VolumeControl.TabIndex = 10;
            this.Label_VolumeControl.Text = "Volume Control";
            // 
            // GroupBox_Volume
            // 
            this.GroupBox_Volume.Controls.Add(this.Label_VolumeStep);
            this.GroupBox_Volume.Controls.Add(this.Numeric_VolumeStep);
            this.GroupBox_Volume.Location = new System.Drawing.Point(8, 157);
            this.GroupBox_Volume.Name = "GroupBox_Volume";
            this.GroupBox_Volume.Size = new System.Drawing.Size(344, 59);
            this.GroupBox_Volume.TabIndex = 9;
            this.GroupBox_Volume.TabStop = false;
            this.GroupBox_Volume.Text = "Volume Adjustment";
            // 
            // Label_VolumeStep
            // 
            this.Label_VolumeStep.AutoSize = true;
            this.Label_VolumeStep.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeStep.Location = new System.Drawing.Point(6, 27);
            this.Label_VolumeStep.Name = "Label_VolumeStep";
            this.Label_VolumeStep.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.Label_VolumeStep.Size = new System.Drawing.Size(39, 16);
            this.Label_VolumeStep.TabIndex = 2;
            this.Label_VolumeStep.Text = "Step";
            // 
            // Numeric_VolumeStep
            // 
            this.Numeric_VolumeStep.AccessibleDescription = "Defines how much the volume increases or decreases when a hotkey is pressed.";
            this.Numeric_VolumeStep.AccessibleName = "Volume Step Control";
            this.Numeric_VolumeStep.AutoSize = true;
            this.Numeric_VolumeStep.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Numeric_VolumeStep.Location = new System.Drawing.Point(55, 25);
            this.Numeric_VolumeStep.Name = "Numeric_VolumeStep";
            this.Numeric_VolumeStep.Size = new System.Drawing.Size(47, 26);
            this.Numeric_VolumeStep.TabIndex = 1;
            this.Numeric_VolumeStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.Numeric_VolumeStep.ValueChanged += new System.EventHandler(this.Numeric_VolumeStep_ValueChanged);
            // 
            // GroupBox_TargetProcess
            // 
            this.GroupBox_TargetProcess.BackColor = System.Drawing.Color.Transparent;
            this.GroupBox_TargetProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GroupBox_TargetProcess.Controls.Add(this.ComboBox_ProcessSelector);
            this.GroupBox_TargetProcess.Controls.Add(this.Button_ReloadProcessList);
            this.GroupBox_TargetProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GroupBox_TargetProcess.Location = new System.Drawing.Point(8, 89);
            this.GroupBox_TargetProcess.Margin = new System.Windows.Forms.Padding(0);
            this.GroupBox_TargetProcess.Name = "GroupBox_TargetProcess";
            this.GroupBox_TargetProcess.Size = new System.Drawing.Size(344, 65);
            this.GroupBox_TargetProcess.TabIndex = 8;
            this.GroupBox_TargetProcess.TabStop = false;
            this.GroupBox_TargetProcess.Text = "Target Process";
            // 
            // Button_ReloadProcessList
            // 
            this.Button_ReloadProcessList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_ReloadProcessList.Location = new System.Drawing.Point(272, 25);
            this.Button_ReloadProcessList.Name = "Button_ReloadProcessList";
            this.Button_ReloadProcessList.Size = new System.Drawing.Size(66, 24);
            this.Button_ReloadProcessList.TabIndex = 7;
            this.Button_ReloadProcessList.Text = "Reload";
            this.Button_ReloadProcessList.UseVisualStyleBackColor = true;
            this.Button_ReloadProcessList.Click += new System.EventHandler(this.Button_ReloadProcessList_Click);
            // 
            // Tab_Hotkeys_Volume
            // 
            this.Tab_Hotkeys_Volume.Controls.Add(this.HKEdit_VolumeUp);
            this.Tab_Hotkeys_Volume.Controls.Add(this.HKEdit_VolumeMute);
            this.Tab_Hotkeys_Volume.Controls.Add(this.HKEdit_VolumeDown);
            this.Tab_Hotkeys_Volume.Location = new System.Drawing.Point(4, 25);
            this.Tab_Hotkeys_Volume.Name = "Tab_Hotkeys_Volume";
            this.Tab_Hotkeys_Volume.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Hotkeys_Volume.Size = new System.Drawing.Size(360, 224);
            this.Tab_Hotkeys_Volume.TabIndex = 1;
            this.Tab_Hotkeys_Volume.Text = "Volume Hotkeys";
            this.Tab_Hotkeys_Volume.UseVisualStyleBackColor = true;
            // 
            // Tab_Hotkeys_Playback
            // 
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_Prev);
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_Next);
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_TogglePlayback);
            this.Tab_Hotkeys_Playback.Location = new System.Drawing.Point(4, 25);
            this.Tab_Hotkeys_Playback.Name = "Tab_Hotkeys_Playback";
            this.Tab_Hotkeys_Playback.Size = new System.Drawing.Size(360, 224);
            this.Tab_Hotkeys_Playback.TabIndex = 2;
            this.Tab_Hotkeys_Playback.Text = "Playback Hotkeys";
            this.Tab_Hotkeys_Playback.UseVisualStyleBackColor = true;
            // 
            // HKEdit_TogglePlayback
            // 
            this.HKEdit_TogglePlayback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_TogglePlayback.Location = new System.Drawing.Point(9, 3);
            this.HKEdit_TogglePlayback.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_TogglePlayback.Name = "HKEdit_TogglePlayback";
            this.HKEdit_TogglePlayback.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_TogglePlayback.TabIndex = 0;
            // 
            // HKEdit_Next
            // 
            this.HKEdit_Next.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Next.Location = new System.Drawing.Point(9, 75);
            this.HKEdit_Next.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Next.Name = "HKEdit_Next";
            this.HKEdit_Next.Size = new System.Drawing.Size(343, 70);
            this.HKEdit_Next.TabIndex = 1;
            // 
            // HKEdit_Prev
            // 
            this.HKEdit_Prev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Prev.Location = new System.Drawing.Point(9, 151);
            this.HKEdit_Prev.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Prev.Name = "HKEdit_Prev";
            this.HKEdit_Prev.Size = new System.Drawing.Size(343, 70);
            this.HKEdit_Prev.TabIndex = 2;
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 253);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "VolumeControlForm";
            this.Text = "Form1";
            this.GotFocus += new System.EventHandler(this.Window_GotFocus);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.SystemTray_ContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource_KeyList)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Icon_VolumeControl)).EndInit();
            this.GroupBox_Volume.ResumeLayout(false);
            this.GroupBox_Volume.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_VolumeStep)).EndInit();
            this.GroupBox_TargetProcess.ResumeLayout(false);
            this.Tab_Hotkeys_Volume.ResumeLayout(false);
            this.Tab_Hotkeys_Playback.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon SystemTray;
        private ContextMenuStrip SystemTray_ContextMenu;
        private ToolStripMenuItem closeToolStripMenuItem;
        private CheckBox checkbox_minimizeOnStartup;
        private ComboBox ComboBox_ProcessSelector;
        private UIComposites.HotkeyEditor HKEdit_VolumeUp;
        private UIComposites.HotkeyEditor HKEdit_VolumeDown;
        private UIComposites.HotkeyEditor HKEdit_VolumeMute;
        private TabControl tabControl1;
        private TabPage Tab_General;
        private TabPage Tab_Hotkeys_Volume;
        private Button Button_ReloadProcessList;
        private BindingSource BindingSource_KeyList;
        private PictureBox Icon_VolumeControl;
        private Label Label_VersionNumber;
        private Label Label_VolumeControl;
        private GroupBox GroupBox_Volume;
        private GroupBox GroupBox_TargetProcess;
        private Label Label_VolumeStep;
        private NumericUpDown Numeric_VolumeStep;
        private CheckBox CheckBox_RunOnStartup;
        private CheckBox CheckBox_VisibleInTaskbar;
        private TabPage Tab_Hotkeys_Playback;
        private UIComposites.HotkeyEditor HKEdit_Prev;
        private UIComposites.HotkeyEditor HKEdit_Next;
        private UIComposites.HotkeyEditor HKEdit_TogglePlayback;
    }
}