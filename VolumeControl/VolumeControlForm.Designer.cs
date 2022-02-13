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
            this.HKEdit_VolumeUp = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeDown = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeMute = new UIComposites.HotkeyEditor();
            this.Tab_TargetSelection = new System.Windows.Forms.TabControl();
            this.Tab_General = new System.Windows.Forms.TabPage();
            this.GroupBox_Toast = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ToastTimeout = new System.Windows.Forms.NumericUpDown();
            this.Checkbox_ToastEnabled = new System.Windows.Forms.CheckBox();
            this.Label_VersionNumber = new System.Windows.Forms.Label();
            this.Label_VolumeControl = new System.Windows.Forms.Label();
            this.CheckBox_VisibleInTaskbar = new System.Windows.Forms.CheckBox();
            this.CheckBox_RunOnStartup = new System.Windows.Forms.CheckBox();
            this.GroupBox_TargetProcess = new System.Windows.Forms.GroupBox();
            this.Label_VolumeStep = new System.Windows.Forms.Label();
            this.Numeric_VolumeStep = new System.Windows.Forms.NumericUpDown();
            this.Button_ReloadProcessList = new System.Windows.Forms.Button();
            this.Tab_Hotkeys_Volume = new System.Windows.Forms.TabPage();
            this.Tab_Hotkeys_Playback = new System.Windows.Forms.TabPage();
            this.HKEdit_Prev = new UIComposites.HotkeyEditor();
            this.HKEdit_Next = new UIComposites.HotkeyEditor();
            this.HKEdit_TogglePlayback = new UIComposites.HotkeyEditor();
            this.TabPage_Target = new System.Windows.Forms.TabPage();
            this.HKEdit_ShowTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_PrevTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_NextTarget = new UIComposites.HotkeyEditor();
            this.SystemTray_ContextMenu.SuspendLayout();
            this.Tab_TargetSelection.SuspendLayout();
            this.Tab_General.SuspendLayout();
            this.GroupBox_Toast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ToastTimeout)).BeginInit();
            this.GroupBox_TargetProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_VolumeStep)).BeginInit();
            this.Tab_Hotkeys_Volume.SuspendLayout();
            this.Tab_Hotkeys_Playback.SuspendLayout();
            this.TabPage_Target.SuspendLayout();
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
            this.SystemTray_ContextMenu.BackColor = System.Drawing.SystemColors.Menu;
            this.SystemTray_ContextMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SystemTray_ContextMenu.DropShadowEnabled = false;
            this.SystemTray_ContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.SystemTray_ContextMenu.Name = "system_tray_menu";
            this.SystemTray_ContextMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.SystemTray_ContextMenu.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = global::VolumeControl.Properties.Resources.png_x;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.SystemTray_ContextMenu_Close);
            // 
            // checkbox_minimizeOnStartup
            // 
            this.checkbox_minimizeOnStartup.AutoSize = true;
            this.checkbox_minimizeOnStartup.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkbox_minimizeOnStartup.Location = new System.Drawing.Point(11, 90);
            this.checkbox_minimizeOnStartup.Name = "checkbox_minimizeOnStartup";
            this.checkbox_minimizeOnStartup.Size = new System.Drawing.Size(115, 20);
            this.checkbox_minimizeOnStartup.TabIndex = 5;
            this.checkbox_minimizeOnStartup.Text = "Start Minimized";
            this.checkbox_minimizeOnStartup.UseVisualStyleBackColor = true;
            this.checkbox_minimizeOnStartup.CheckedChanged += new System.EventHandler(this.Checkbox_MinimizeOnStartup_CheckedChanged);
            // 
            // ComboBox_ProcessSelector
            // 
            this.ComboBox_ProcessSelector.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ComboBox_ProcessSelector.Location = new System.Drawing.Point(6, 17);
            this.ComboBox_ProcessSelector.MaxDropDownItems = 16;
            this.ComboBox_ProcessSelector.Name = "ComboBox_ProcessSelector";
            this.ComboBox_ProcessSelector.Size = new System.Drawing.Size(128, 24);
            this.ComboBox_ProcessSelector.TabIndex = 6;
            this.ComboBox_ProcessSelector.TextChanged += new System.EventHandler(this.ComboBox_ProcessName_TextChanged);
            // 
            // HKEdit_VolumeUp
            // 
            this.HKEdit_VolumeUp.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeUp.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_VolumeUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeUp.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeUp.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeUp.Name = "HKEdit_VolumeUp";
            this.HKEdit_VolumeUp.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeUp.TabIndex = 7;
            this.HKEdit_VolumeUp.Tag = "";
            // 
            // HKEdit_VolumeDown
            // 
            this.HKEdit_VolumeDown.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeDown.Location = new System.Drawing.Point(9, 69);
            this.HKEdit_VolumeDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeDown.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeDown.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeDown.Name = "HKEdit_VolumeDown";
            this.HKEdit_VolumeDown.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeDown.TabIndex = 8;
            this.HKEdit_VolumeDown.Tag = "";
            // 
            // HKEdit_VolumeMute
            // 
            this.HKEdit_VolumeMute.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeMute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeMute.Location = new System.Drawing.Point(9, 132);
            this.HKEdit_VolumeMute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeMute.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.Name = "HKEdit_VolumeMute";
            this.HKEdit_VolumeMute.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.TabIndex = 9;
            this.HKEdit_VolumeMute.Tag = "";
            // 
            // Tab_TargetSelection
            // 
            this.Tab_TargetSelection.Controls.Add(this.Tab_General);
            this.Tab_TargetSelection.Controls.Add(this.Tab_Hotkeys_Volume);
            this.Tab_TargetSelection.Controls.Add(this.Tab_Hotkeys_Playback);
            this.Tab_TargetSelection.Controls.Add(this.TabPage_Target);
            this.Tab_TargetSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tab_TargetSelection.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Tab_TargetSelection.Location = new System.Drawing.Point(0, 0);
            this.Tab_TargetSelection.Name = "Tab_TargetSelection";
            this.Tab_TargetSelection.SelectedIndex = 0;
            this.Tab_TargetSelection.Size = new System.Drawing.Size(368, 230);
            this.Tab_TargetSelection.TabIndex = 10;
            // 
            // Tab_General
            // 
            this.Tab_General.Controls.Add(this.GroupBox_Toast);
            this.Tab_General.Controls.Add(this.Label_VersionNumber);
            this.Tab_General.Controls.Add(this.Label_VolumeControl);
            this.Tab_General.Controls.Add(this.CheckBox_VisibleInTaskbar);
            this.Tab_General.Controls.Add(this.CheckBox_RunOnStartup);
            this.Tab_General.Controls.Add(this.checkbox_minimizeOnStartup);
            this.Tab_General.Controls.Add(this.GroupBox_TargetProcess);
            this.Tab_General.Location = new System.Drawing.Point(4, 24);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(360, 202);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            this.Tab_General.UseVisualStyleBackColor = true;
            // 
            // GroupBox_Toast
            // 
            this.GroupBox_Toast.Controls.Add(this.label1);
            this.GroupBox_Toast.Controls.Add(this.ToastTimeout);
            this.GroupBox_Toast.Controls.Add(this.Checkbox_ToastEnabled);
            this.GroupBox_Toast.Location = new System.Drawing.Point(176, 56);
            this.GroupBox_Toast.Name = "GroupBox_Toast";
            this.GroupBox_Toast.Size = new System.Drawing.Size(176, 88);
            this.GroupBox_Toast.TabIndex = 18;
            this.GroupBox_Toast.TabStop = false;
            this.GroupBox_Toast.Text = "Target Switching";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 17;
            this.label1.Text = "Duration (ms)";
            // 
            // ToastTimeout
            // 
            this.ToastTimeout.AccessibleDescription = "";
            this.ToastTimeout.AccessibleName = "";
            this.ToastTimeout.AutoSize = true;
            this.ToastTimeout.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ToastTimeout.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.ToastTimeout.Location = new System.Drawing.Point(107, 47);
            this.ToastTimeout.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.ToastTimeout.Name = "ToastTimeout";
            this.ToastTimeout.Size = new System.Drawing.Size(63, 26);
            this.ToastTimeout.TabIndex = 16;
            this.ToastTimeout.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.ToastTimeout.ValueChanged += new System.EventHandler(this.ToastTimeout_ValueChanged);
            // 
            // Checkbox_ToastEnabled
            // 
            this.Checkbox_ToastEnabled.AutoSize = true;
            this.Checkbox_ToastEnabled.Location = new System.Drawing.Point(14, 22);
            this.Checkbox_ToastEnabled.Name = "Checkbox_ToastEnabled";
            this.Checkbox_ToastEnabled.Size = new System.Drawing.Size(148, 19);
            this.Checkbox_ToastEnabled.TabIndex = 15;
            this.Checkbox_ToastEnabled.Text = "Enable Target Window";
            this.Checkbox_ToastEnabled.UseVisualStyleBackColor = true;
            this.Checkbox_ToastEnabled.CheckedChanged += new System.EventHandler(this.Checkbox_ToastEnabled_CheckedChanged);
            // 
            // Label_VersionNumber
            // 
            this.Label_VersionNumber.AutoSize = true;
            this.Label_VersionNumber.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label_VersionNumber.Location = new System.Drawing.Point(3, 20);
            this.Label_VersionNumber.Name = "Label_VersionNumber";
            this.Label_VersionNumber.Padding = new System.Windows.Forms.Padding(151, 0, 0, 0);
            this.Label_VersionNumber.Size = new System.Drawing.Size(213, 15);
            this.Label_VersionNumber.TabIndex = 11;
            this.Label_VersionNumber.Text = "[ version ]";
            this.Label_VersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_VolumeControl
            // 
            this.Label_VolumeControl.AutoSize = true;
            this.Label_VolumeControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.Label_VolumeControl.Font = new System.Drawing.Font("Lucida Sans Unicode", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeControl.Location = new System.Drawing.Point(3, 3);
            this.Label_VolumeControl.Name = "Label_VolumeControl";
            this.Label_VolumeControl.Padding = new System.Windows.Forms.Padding(117, 0, 0, 0);
            this.Label_VolumeControl.Size = new System.Drawing.Size(247, 17);
            this.Label_VolumeControl.TabIndex = 10;
            this.Label_VolumeControl.Text = "Volume Control";
            this.Label_VolumeControl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckBox_VisibleInTaskbar
            // 
            this.CheckBox_VisibleInTaskbar.AutoSize = true;
            this.CheckBox_VisibleInTaskbar.Cursor = System.Windows.Forms.Cursors.Default;
            this.CheckBox_VisibleInTaskbar.Location = new System.Drawing.Point(11, 116);
            this.CheckBox_VisibleInTaskbar.Name = "CheckBox_VisibleInTaskbar";
            this.CheckBox_VisibleInTaskbar.Size = new System.Drawing.Size(128, 19);
            this.CheckBox_VisibleInTaskbar.TabIndex = 13;
            this.CheckBox_VisibleInTaskbar.Text = "Show Taskbar Icon";
            this.CheckBox_VisibleInTaskbar.UseVisualStyleBackColor = true;
            this.CheckBox_VisibleInTaskbar.CheckedChanged += new System.EventHandler(this.CheckBox_VisibleInTaskbar_CheckedChanged);
            // 
            // CheckBox_RunOnStartup
            // 
            this.CheckBox_RunOnStartup.AutoSize = true;
            this.CheckBox_RunOnStartup.Location = new System.Drawing.Point(11, 65);
            this.CheckBox_RunOnStartup.Name = "CheckBox_RunOnStartup";
            this.CheckBox_RunOnStartup.Size = new System.Drawing.Size(107, 19);
            this.CheckBox_RunOnStartup.TabIndex = 14;
            this.CheckBox_RunOnStartup.Text = "Run on Startup";
            this.CheckBox_RunOnStartup.UseVisualStyleBackColor = true;
            this.CheckBox_RunOnStartup.CheckedChanged += new System.EventHandler(this.CheckBox_RunOnStartup_CheckedChanged);
            // 
            // GroupBox_TargetProcess
            // 
            this.GroupBox_TargetProcess.BackColor = System.Drawing.Color.Transparent;
            this.GroupBox_TargetProcess.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.GroupBox_TargetProcess.Controls.Add(this.Label_VolumeStep);
            this.GroupBox_TargetProcess.Controls.Add(this.Numeric_VolumeStep);
            this.GroupBox_TargetProcess.Controls.Add(this.ComboBox_ProcessSelector);
            this.GroupBox_TargetProcess.Controls.Add(this.Button_ReloadProcessList);
            this.GroupBox_TargetProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GroupBox_TargetProcess.Location = new System.Drawing.Point(5, 147);
            this.GroupBox_TargetProcess.Margin = new System.Windows.Forms.Padding(0);
            this.GroupBox_TargetProcess.Name = "GroupBox_TargetProcess";
            this.GroupBox_TargetProcess.Size = new System.Drawing.Size(347, 50);
            this.GroupBox_TargetProcess.TabIndex = 8;
            this.GroupBox_TargetProcess.TabStop = false;
            this.GroupBox_TargetProcess.Text = "Target Process";
            // 
            // Label_VolumeStep
            // 
            this.Label_VolumeStep.AutoSize = true;
            this.Label_VolumeStep.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeStep.Location = new System.Drawing.Point(212, 19);
            this.Label_VolumeStep.Name = "Label_VolumeStep";
            this.Label_VolumeStep.Size = new System.Drawing.Size(79, 16);
            this.Label_VolumeStep.TabIndex = 2;
            this.Label_VolumeStep.Text = "Volume Step";
            // 
            // Numeric_VolumeStep
            // 
            this.Numeric_VolumeStep.AccessibleDescription = "Defines how much the volume increases or decreases when a hotkey is pressed.";
            this.Numeric_VolumeStep.AccessibleName = "Volume Step Control";
            this.Numeric_VolumeStep.AutoSize = true;
            this.Numeric_VolumeStep.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Numeric_VolumeStep.Location = new System.Drawing.Point(294, 15);
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
            // Button_ReloadProcessList
            // 
            this.Button_ReloadProcessList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_ReloadProcessList.Location = new System.Drawing.Point(140, 17);
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
            this.Tab_Hotkeys_Volume.Location = new System.Drawing.Point(4, 24);
            this.Tab_Hotkeys_Volume.Name = "Tab_Hotkeys_Volume";
            this.Tab_Hotkeys_Volume.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Hotkeys_Volume.Size = new System.Drawing.Size(360, 202);
            this.Tab_Hotkeys_Volume.TabIndex = 1;
            this.Tab_Hotkeys_Volume.Text = "Volume Hotkeys";
            this.Tab_Hotkeys_Volume.UseVisualStyleBackColor = true;
            // 
            // Tab_Hotkeys_Playback
            // 
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_Prev);
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_Next);
            this.Tab_Hotkeys_Playback.Controls.Add(this.HKEdit_TogglePlayback);
            this.Tab_Hotkeys_Playback.Location = new System.Drawing.Point(4, 24);
            this.Tab_Hotkeys_Playback.Name = "Tab_Hotkeys_Playback";
            this.Tab_Hotkeys_Playback.Size = new System.Drawing.Size(360, 202);
            this.Tab_Hotkeys_Playback.TabIndex = 2;
            this.Tab_Hotkeys_Playback.Text = "Playback Hotkeys";
            this.Tab_Hotkeys_Playback.UseVisualStyleBackColor = true;
            // 
            // HKEdit_Prev
            // 
            this.HKEdit_Prev.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_Prev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Prev.Location = new System.Drawing.Point(9, 132);
            this.HKEdit_Prev.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Prev.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_Prev.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_Prev.Name = "HKEdit_Prev";
            this.HKEdit_Prev.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_Prev.TabIndex = 2;
            // 
            // HKEdit_Next
            // 
            this.HKEdit_Next.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_Next.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Next.Location = new System.Drawing.Point(9, 69);
            this.HKEdit_Next.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Next.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_Next.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_Next.Name = "HKEdit_Next";
            this.HKEdit_Next.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_Next.TabIndex = 1;
            // 
            // HKEdit_TogglePlayback
            // 
            this.HKEdit_TogglePlayback.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_TogglePlayback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_TogglePlayback.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_TogglePlayback.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_TogglePlayback.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_TogglePlayback.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_TogglePlayback.Name = "HKEdit_TogglePlayback";
            this.HKEdit_TogglePlayback.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_TogglePlayback.TabIndex = 0;
            // 
            // TabPage_Target
            // 
            this.TabPage_Target.Controls.Add(this.HKEdit_ShowTarget);
            this.TabPage_Target.Controls.Add(this.HKEdit_PrevTarget);
            this.TabPage_Target.Controls.Add(this.HKEdit_NextTarget);
            this.TabPage_Target.Location = new System.Drawing.Point(4, 24);
            this.TabPage_Target.Name = "TabPage_Target";
            this.TabPage_Target.Size = new System.Drawing.Size(360, 202);
            this.TabPage_Target.TabIndex = 4;
            this.TabPage_Target.Text = "Target Hotkeys";
            this.TabPage_Target.UseVisualStyleBackColor = true;
            // 
            // HKEdit_ShowTarget
            // 
            this.HKEdit_ShowTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_ShowTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_ShowTarget.Location = new System.Drawing.Point(9, 132);
            this.HKEdit_ShowTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_ShowTarget.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_ShowTarget.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_ShowTarget.Name = "HKEdit_ShowTarget";
            this.HKEdit_ShowTarget.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_ShowTarget.TabIndex = 2;
            // 
            // HKEdit_PrevTarget
            // 
            this.HKEdit_PrevTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_PrevTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_PrevTarget.Location = new System.Drawing.Point(9, 69);
            this.HKEdit_PrevTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_PrevTarget.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_PrevTarget.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_PrevTarget.Name = "HKEdit_PrevTarget";
            this.HKEdit_PrevTarget.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_PrevTarget.TabIndex = 1;
            // 
            // HKEdit_NextTarget
            // 
            this.HKEdit_NextTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_NextTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_NextTarget.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_NextTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_NextTarget.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.Name = "HKEdit_NextTarget";
            this.HKEdit_NextTarget.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.TabIndex = 0;
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 230);
            this.Controls.Add(this.Tab_TargetSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "VolumeControlForm";
            this.Text = "Form1";
            this.GotFocus += new System.EventHandler(this.Window_GotFocus);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.SystemTray_ContextMenu.ResumeLayout(false);
            this.Tab_TargetSelection.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            this.GroupBox_Toast.ResumeLayout(false);
            this.GroupBox_Toast.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ToastTimeout)).EndInit();
            this.GroupBox_TargetProcess.ResumeLayout(false);
            this.GroupBox_TargetProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Numeric_VolumeStep)).EndInit();
            this.Tab_Hotkeys_Volume.ResumeLayout(false);
            this.Tab_Hotkeys_Playback.ResumeLayout(false);
            this.TabPage_Target.ResumeLayout(false);
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
        private TabControl Tab_TargetSelection;
        private TabPage Tab_General;
        private TabPage Tab_Hotkeys_Volume;
        private Button Button_ReloadProcessList;
        private PictureBox Icon_VolumeControl;
        private Label Label_VersionNumber;
        private Label Label_VolumeControl;
        private GroupBox GroupBox_TargetProcess;
        private Label Label_VolumeStep;
        private NumericUpDown Numeric_VolumeStep;
        private CheckBox CheckBox_RunOnStartup;
        private CheckBox CheckBox_VisibleInTaskbar;
        private TabPage Tab_Hotkeys_Playback;
        private UIComposites.HotkeyEditor HKEdit_Prev;
        private UIComposites.HotkeyEditor HKEdit_Next;
        private UIComposites.HotkeyEditor HKEdit_TogglePlayback;
        private TabPage TabPage_Target;
        private UIComposites.HotkeyEditor HKEdit_ShowTarget;
        private UIComposites.HotkeyEditor HKEdit_PrevTarget;
        private UIComposites.HotkeyEditor HKEdit_NextTarget;
        private CheckBox Checkbox_ToastEnabled;
        private Label label1;
        private NumericUpDown ToastTimeout;
        private GroupBox GroupBox_Toast;
    }
}