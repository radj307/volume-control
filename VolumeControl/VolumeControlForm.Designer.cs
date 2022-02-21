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
            this.ComboBox_ProcessSelector = new System.Windows.Forms.ComboBox();
            this.HKEdit_VolumeUp = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeDown = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeMute = new UIComposites.HotkeyEditor();
            this.TabController = new Manina.Windows.Forms.TabControl();
            this.Tab_General = new Manina.Windows.Forms.Tab();
            this.TgtSettings = new UIComposites.ToastSettings();
            this.Settings = new UIComposites.SettingsPane();
            this.label_targetswitch = new System.Windows.Forms.Label();
            this.Button_ReloadProcessList = new System.Windows.Forms.Button();
            this.Label_VersionNumber = new System.Windows.Forms.Label();
            this.Label_VolumeControl = new System.Windows.Forms.Label();
            this.Tab_Hotkeys_Volume = new Manina.Windows.Forms.Tab();
            this.Tab_Hotkeys_Playback = new Manina.Windows.Forms.Tab();
            this.HKEdit_Prev = new UIComposites.HotkeyEditor();
            this.HKEdit_Next = new UIComposites.HotkeyEditor();
            this.HKEdit_TogglePlayback = new UIComposites.HotkeyEditor();
            this.TabPage_Target = new Manina.Windows.Forms.Tab();
            this.HKEdit_ShowTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_PrevTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_NextTarget = new UIComposites.HotkeyEditor();
            this.TargetRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SystemTray_ContextMenu.SuspendLayout();
            this.Tab_General.SuspendLayout();
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
            // ComboBox_ProcessSelector
            // 
            this.ComboBox_ProcessSelector.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ComboBox_ProcessSelector.Location = new System.Drawing.Point(54, 170);
            this.ComboBox_ProcessSelector.MaxDropDownItems = 16;
            this.ComboBox_ProcessSelector.Name = "ComboBox_ProcessSelector";
            this.ComboBox_ProcessSelector.Size = new System.Drawing.Size(226, 24);
            this.ComboBox_ProcessSelector.TabIndex = 6;
            this.ComboBox_ProcessSelector.TextChanged += new System.EventHandler(this.ComboBox_ProcessName_TextChanged);
            // 
            // HKEdit_VolumeUp
            // 
            this.HKEdit_VolumeUp.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeUp.Label = "Volume Up";
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
            this.HKEdit_VolumeDown.Label = "Volume Down";
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
            this.HKEdit_VolumeMute.Label = "Toggle Mute";
            this.HKEdit_VolumeMute.Location = new System.Drawing.Point(9, 132);
            this.HKEdit_VolumeMute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeMute.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.Name = "HKEdit_VolumeMute";
            this.HKEdit_VolumeMute.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_VolumeMute.TabIndex = 9;
            this.HKEdit_VolumeMute.Tag = "";
            // 
            // TabController
            // 
            this.TabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabController.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabController.Location = new System.Drawing.Point(0, 0);
            this.TabController.Name = "TabController";
            this.TabController.SelectedIndex = -1;
            this.TabController.Size = new System.Drawing.Size(368, 230);
            this.TabController.TabIndex = 10;
            this.TabController.Tabs.Add(this.Tab_General);
            this.TabController.Tabs.Add(this.Tab_Hotkeys_Volume);
            this.TabController.Tabs.Add(this.Tab_Hotkeys_Playback);
            this.TabController.Tabs.Add(this.TabPage_Target);
            // 
            // Tab_General
            // 
            this.Tab_General.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Tab_General.Controls.Add(this.TgtSettings);
            this.Tab_General.Controls.Add(this.Settings);
            this.Tab_General.Controls.Add(this.label_targetswitch);
            this.Tab_General.Controls.Add(this.ComboBox_ProcessSelector);
            this.Tab_General.Controls.Add(this.Button_ReloadProcessList);
            this.Tab_General.Controls.Add(this.Label_VersionNumber);
            this.Tab_General.Controls.Add(this.Label_VolumeControl);
            this.Tab_General.Font = new System.Drawing.Font("Calibri", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Tab_General.Location = new System.Drawing.Point(4, 24);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(360, 202);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            // 
            // TgtSettings
            // 
            this.TgtSettings.BackColor = System.Drawing.Color.Transparent;
            this.TgtSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TgtSettings.EnableDarkMode = false;
            this.TgtSettings.ForeColor = System.Drawing.Color.Transparent;
            this.TgtSettings.Location = new System.Drawing.Point(8, 65);
            this.TgtSettings.Name = "TgtSettings";
            this.TgtSettings.Size = new System.Drawing.Size(180, 99);
            this.TgtSettings.TabIndex = 23;
            this.TgtSettings.TargetListEnabled = false;
            this.TgtSettings.TargetListTimeout = 500;
            this.TgtSettings.TargetListEnabledChanged += new System.EventHandler(this.ToastEnabled_Changed);
            this.TgtSettings.TargetListTimeoutChanged += new System.EventHandler(this.ToastTimeout_Changed);
            this.TgtSettings.DarkModeChanged += new System.EventHandler(this.TgtSettings_DarkModeChanged);
            // 
            // Settings
            // 
            this.Settings.AlwaysOnTop = false;
            this.Settings.BackColor = System.Drawing.Color.Transparent;
            this.Settings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Settings.EnableDarkMode = false;
            this.Settings.Location = new System.Drawing.Point(206, 10);
            this.Settings.MinimizeOnStartup = false;
            this.Settings.Name = "Settings";
            this.Settings.RunAtStartup = false;
            this.Settings.ShowInTaskbar = false;
            this.Settings.Size = new System.Drawing.Size(146, 154);
            this.Settings.TabIndex = 22;
            this.Settings.VolumeStep = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.Settings.AlwaysOnTopChanged += new System.EventHandler(this.AlwaysOnTop_Changed);
            this.Settings.RunAtStartupChanged += new System.EventHandler(this.RunOnStartup_Changed);
            this.Settings.MinimizeOnStartupChanged += new System.EventHandler(this.MinimizeOnStartup_Changed);
            this.Settings.ShowInTaskbarChanged += new System.EventHandler(this.VisibleInTaskbar_Changed);
            this.Settings.DarkModeChanged += new System.EventHandler(this.Settings_DarkModeChanged);
            this.Settings.VolumeStepChanged += new System.EventHandler(this.VolumeStep_Changed);
            // 
            // label_targetswitch
            // 
            this.label_targetswitch.AutoSize = true;
            this.label_targetswitch.Location = new System.Drawing.Point(8, 170);
            this.label_targetswitch.Name = "label_targetswitch";
            this.label_targetswitch.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.label_targetswitch.Size = new System.Drawing.Size(40, 19);
            this.label_targetswitch.TabIndex = 21;
            this.label_targetswitch.Text = "Target";
            // 
            // Button_ReloadProcessList
            // 
            this.Button_ReloadProcessList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button_ReloadProcessList.Location = new System.Drawing.Point(286, 170);
            this.Button_ReloadProcessList.Name = "Button_ReloadProcessList";
            this.Button_ReloadProcessList.Size = new System.Drawing.Size(66, 24);
            this.Button_ReloadProcessList.TabIndex = 7;
            this.Button_ReloadProcessList.Text = "Reload";
            this.Button_ReloadProcessList.UseVisualStyleBackColor = true;
            this.Button_ReloadProcessList.Click += new System.EventHandler(this.Button_ReloadProcessList_Click);
            // 
            // Label_VersionNumber
            // 
            this.Label_VersionNumber.AutoSize = true;
            this.Label_VersionNumber.BackColor = System.Drawing.Color.Transparent;
            this.Label_VersionNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Label_VersionNumber.Location = new System.Drawing.Point(67, 35);
            this.Label_VersionNumber.Name = "Label_VersionNumber";
            this.Label_VersionNumber.Size = new System.Drawing.Size(62, 15);
            this.Label_VersionNumber.TabIndex = 11;
            this.Label_VersionNumber.Text = "[ version ]";
            this.Label_VersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label_VolumeControl
            // 
            this.Label_VolumeControl.AutoSize = true;
            this.Label_VolumeControl.Font = new System.Drawing.Font("Lucida Sans Unicode", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeControl.Location = new System.Drawing.Point(33, 18);
            this.Label_VolumeControl.Name = "Label_VolumeControl";
            this.Label_VolumeControl.Size = new System.Drawing.Size(130, 17);
            this.Label_VolumeControl.TabIndex = 10;
            this.Label_VolumeControl.Text = "Volume Control";
            this.Label_VolumeControl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // 
            // HKEdit_Prev
            // 
            this.HKEdit_Prev.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_Prev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Prev.Label = "Previous Track";
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
            this.HKEdit_Next.Label = "Next Track";
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
            this.HKEdit_TogglePlayback.Label = "Toggle Playback";
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
            // 
            // HKEdit_ShowTarget
            // 
            this.HKEdit_ShowTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_ShowTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_ShowTarget.Label = "Show Current Target";
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
            this.HKEdit_PrevTarget.Label = "Previous Target";
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
            this.HKEdit_NextTarget.Label = "Next Target";
            this.HKEdit_NextTarget.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_NextTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_NextTarget.MaximumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.MinimumSize = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.Name = "HKEdit_NextTarget";
            this.HKEdit_NextTarget.Size = new System.Drawing.Size(343, 57);
            this.HKEdit_NextTarget.TabIndex = 0;
            // 
            // TargetRefreshTimer
            // 
            this.TargetRefreshTimer.Enabled = true;
            this.TargetRefreshTimer.Interval = 4000;
            this.TargetRefreshTimer.Tick += new System.EventHandler(this.TargetRefreshTimer_Tick);
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(368, 230);
            this.Controls.Add(this.TabController);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "VolumeControlForm";
            this.Text = "Volume Control";
            this.GotFocus += new System.EventHandler(this.Window_GotFocus);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.SystemTray_ContextMenu.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            this.Tab_Hotkeys_Volume.ResumeLayout(false);
            this.Tab_Hotkeys_Playback.ResumeLayout(false);
            this.TabPage_Target.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon SystemTray;
        private ContextMenuStrip SystemTray_ContextMenu;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ComboBox ComboBox_ProcessSelector;
        private UIComposites.HotkeyEditor HKEdit_VolumeUp;
        private UIComposites.HotkeyEditor HKEdit_VolumeDown;
        private UIComposites.HotkeyEditor HKEdit_VolumeMute;
        private Manina.Windows.Forms.TabControl TabController;
        private Manina.Windows.Forms.Tab Tab_General;
        private Manina.Windows.Forms.Tab Tab_Hotkeys_Volume;
        private Button Button_ReloadProcessList;
        private Label Label_VersionNumber;
        private Label Label_VolumeControl;
        private Manina.Windows.Forms.Tab Tab_Hotkeys_Playback;
        private UIComposites.HotkeyEditor HKEdit_Prev;
        private UIComposites.HotkeyEditor HKEdit_Next;
        private UIComposites.HotkeyEditor HKEdit_TogglePlayback;
        private Manina.Windows.Forms.Tab TabPage_Target;
        private UIComposites.HotkeyEditor HKEdit_ShowTarget;
        private UIComposites.HotkeyEditor HKEdit_PrevTarget;
        private UIComposites.HotkeyEditor HKEdit_NextTarget;
        private System.Windows.Forms.Timer TargetRefreshTimer;
        private Label label_targetswitch;
        private UIComposites.ToastSettings TgtSettings;
        private UIComposites.SettingsPane Settings;
    }
}