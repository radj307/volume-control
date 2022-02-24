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
            this.HKEdit_VolumeUp = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeDown = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeMute = new UIComposites.HotkeyEditor();
            this.TabController = new Manina.Windows.Forms.TabControl();
            this.tab_general = new Manina.Windows.Forms.Tab();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Label_VolumeControl = new System.Windows.Forms.Label();
            this.TargetSelector = new UIComposites.TargetSelector();
            this.Label_VersionNumber = new System.Windows.Forms.Label();
            this.Settings = new UIComposites.SettingsPane();
            this.TgtSettings = new UIComposites.ToastSettings();
            this.tab_volume = new Manina.Windows.Forms.Tab();
            this.panel_volume = new System.Windows.Forms.Panel();
            this.tab_media = new Manina.Windows.Forms.Tab();
            this.panel_media = new System.Windows.Forms.Panel();
            this.HKEdit_TogglePlayback = new UIComposites.HotkeyEditor();
            this.HKEdit_Prev = new UIComposites.HotkeyEditor();
            this.HKEdit_Next = new UIComposites.HotkeyEditor();
            this.tab_target = new Manina.Windows.Forms.Tab();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HKEdit_NextTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_ShowTarget = new UIComposites.HotkeyEditor();
            this.HKEdit_PrevTarget = new UIComposites.HotkeyEditor();
            this.TargetRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SystemTray_ContextMenu.SuspendLayout();
            this.TabController.SuspendLayout();
            this.tab_general.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tab_volume.SuspendLayout();
            this.panel_volume.SuspendLayout();
            this.tab_media.SuspendLayout();
            this.panel_media.SuspendLayout();
            this.tab_target.SuspendLayout();
            this.panel1.SuspendLayout();
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
            // HKEdit_VolumeUp
            // 
            this.HKEdit_VolumeUp.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeUp.Label = "Volume Up";
            this.HKEdit_VolumeUp.Location = new System.Drawing.Point(13, 10);
            this.HKEdit_VolumeUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeUp.Name = "HKEdit_VolumeUp";
            this.HKEdit_VolumeUp.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_VolumeUp.TabIndex = 1;
            this.HKEdit_VolumeUp.Tag = "";
            // 
            // HKEdit_VolumeDown
            // 
            this.HKEdit_VolumeDown.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeDown.Label = "Volume Down";
            this.HKEdit_VolumeDown.Location = new System.Drawing.Point(13, 76);
            this.HKEdit_VolumeDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeDown.Name = "HKEdit_VolumeDown";
            this.HKEdit_VolumeDown.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_VolumeDown.TabIndex = 2;
            this.HKEdit_VolumeDown.Tag = "";
            // 
            // HKEdit_VolumeMute
            // 
            this.HKEdit_VolumeMute.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_VolumeMute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeMute.Label = "Toggle Mute";
            this.HKEdit_VolumeMute.Location = new System.Drawing.Point(13, 142);
            this.HKEdit_VolumeMute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeMute.Name = "HKEdit_VolumeMute";
            this.HKEdit_VolumeMute.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_VolumeMute.TabIndex = 3;
            this.HKEdit_VolumeMute.Tag = "";
            // 
            // TabController
            // 
            this.TabController.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TabController.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TabController.Controls.Add(this.tab_general);
            this.TabController.Controls.Add(this.tab_volume);
            this.TabController.Controls.Add(this.tab_media);
            this.TabController.Controls.Add(this.tab_target);
            this.TabController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabController.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TabController.ForeColor = System.Drawing.Color.Black;
            this.TabController.Location = new System.Drawing.Point(0, 0);
            this.TabController.Name = "TabController";
            this.TabController.SelectedIndex = 0;
            this.TabController.Size = new System.Drawing.Size(368, 230);
            this.TabController.TabIndex = 0;
            this.TabController.TabLocation = ((Manina.Windows.Forms.TabLocation)((Manina.Windows.Forms.TabLocation.Center | Manina.Windows.Forms.TabLocation.Top)));
            this.TabController.Tabs.Add(this.tab_general);
            this.TabController.Tabs.Add(this.tab_volume);
            this.TabController.Tabs.Add(this.tab_media);
            this.TabController.Tabs.Add(this.tab_target);
            // 
            // tab_general
            // 
            this.tab_general.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tab_general.Controls.Add(this.panel2);
            this.tab_general.Font = new System.Drawing.Font("Calibri", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tab_general.Location = new System.Drawing.Point(0, 23);
            this.tab_general.Name = "tab_general";
            this.tab_general.Padding = new System.Windows.Forms.Padding(3);
            this.tab_general.Size = new System.Drawing.Size(368, 208);
            this.tab_general.Text = "General";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Label_VolumeControl);
            this.panel2.Controls.Add(this.TargetSelector);
            this.panel2.Controls.Add(this.Label_VersionNumber);
            this.panel2.Controls.Add(this.Settings);
            this.panel2.Controls.Add(this.TgtSettings);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(368, 208);
            this.panel2.TabIndex = 12;
            // 
            // Label_VolumeControl
            // 
            this.Label_VolumeControl.AutoSize = true;
            this.Label_VolumeControl.Font = new System.Drawing.Font("Lucida Sans Unicode", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label_VolumeControl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_VolumeControl.Location = new System.Drawing.Point(38, 20);
            this.Label_VolumeControl.Name = "Label_VolumeControl";
            this.Label_VolumeControl.Size = new System.Drawing.Size(130, 17);
            this.Label_VolumeControl.TabIndex = 10;
            this.Label_VolumeControl.Text = "Volume Control";
            this.Label_VolumeControl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TargetSelector
            // 
            this.TargetSelector.BackColor = System.Drawing.Color.Transparent;
            this.TargetSelector.ComboBoxBorder = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TargetSelector.ForeColor = System.Drawing.Color.Transparent;
            this.TargetSelector.LabelText = "Target";
            this.TargetSelector.Location = new System.Drawing.Point(9, 170);
            this.TargetSelector.Name = "TargetSelector";
            this.TargetSelector.SelectedIndex = -1;
            this.TargetSelector.SelectedItem = null;
            this.TargetSelector.Size = new System.Drawing.Size(350, 30);
            this.TargetSelector.TabIndex = 3;
            this.TargetSelector.ReloadButtonPressed += new System.EventHandler(this.Reload_Clicked);
            // 
            // Label_VersionNumber
            // 
            this.Label_VersionNumber.AutoSize = true;
            this.Label_VersionNumber.BackColor = System.Drawing.Color.Transparent;
            this.Label_VersionNumber.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Label_VersionNumber.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label_VersionNumber.Location = new System.Drawing.Point(72, 35);
            this.Label_VersionNumber.Name = "Label_VersionNumber";
            this.Label_VersionNumber.Size = new System.Drawing.Size(62, 15);
            this.Label_VersionNumber.TabIndex = 11;
            this.Label_VersionNumber.Text = "[ version ]";
            this.Label_VersionNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Settings
            // 
            this.Settings.AlwaysOnTop = false;
            this.Settings.BackColor = System.Drawing.Color.Transparent;
            this.Settings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Settings.EnableDarkMode = false;
            this.Settings.Location = new System.Drawing.Point(207, 10);
            this.Settings.MinimizeOnStartup = false;
            this.Settings.Name = "Settings";
            this.Settings.RunAtStartup = false;
            this.Settings.ShowInTaskbar = false;
            this.Settings.Size = new System.Drawing.Size(146, 154);
            this.Settings.TabIndex = 2;
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
            // TgtSettings
            // 
            this.TgtSettings.BackColor = System.Drawing.Color.Transparent;
            this.TgtSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TgtSettings.EnableDarkMode = false;
            this.TgtSettings.ForeColor = System.Drawing.Color.Transparent;
            this.TgtSettings.Location = new System.Drawing.Point(15, 72);
            this.TgtSettings.Name = "TgtSettings";
            this.TgtSettings.Size = new System.Drawing.Size(180, 92);
            this.TgtSettings.TabIndex = 1;
            this.TgtSettings.TargetListEnabled = false;
            this.TgtSettings.TargetListTimeout = 500;
            this.TgtSettings.TargetListEnabledChanged += new System.EventHandler(this.ToastEnabled_Changed);
            this.TgtSettings.TargetListTimeoutChanged += new System.EventHandler(this.ToastTimeout_Changed);
            this.TgtSettings.DarkModeChanged += new System.EventHandler(this.TgtSettings_DarkModeChanged);
            // 
            // tab_volume
            // 
            this.tab_volume.Controls.Add(this.panel_volume);
            this.tab_volume.Location = new System.Drawing.Point(0, 0);
            this.tab_volume.Name = "tab_volume";
            this.tab_volume.Padding = new System.Windows.Forms.Padding(3);
            this.tab_volume.Size = new System.Drawing.Size(0, 0);
            this.tab_volume.Text = "Volume Hotkeys";
            // 
            // panel_volume
            // 
            this.panel_volume.Controls.Add(this.HKEdit_VolumeUp);
            this.panel_volume.Controls.Add(this.HKEdit_VolumeMute);
            this.panel_volume.Controls.Add(this.HKEdit_VolumeDown);
            this.panel_volume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_volume.Location = new System.Drawing.Point(0, 0);
            this.panel_volume.Name = "panel_volume";
            this.panel_volume.Size = new System.Drawing.Size(0, 0);
            this.panel_volume.TabIndex = 10;
            // 
            // tab_media
            // 
            this.tab_media.Controls.Add(this.panel_media);
            this.tab_media.Location = new System.Drawing.Point(0, 0);
            this.tab_media.Name = "tab_media";
            this.tab_media.Size = new System.Drawing.Size(0, 0);
            this.tab_media.Text = "Media Hotkeys";
            // 
            // panel_media
            // 
            this.panel_media.Controls.Add(this.HKEdit_TogglePlayback);
            this.panel_media.Controls.Add(this.HKEdit_Prev);
            this.panel_media.Controls.Add(this.HKEdit_Next);
            this.panel_media.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_media.Location = new System.Drawing.Point(0, 0);
            this.panel_media.Name = "panel_media";
            this.panel_media.Size = new System.Drawing.Size(0, 0);
            this.panel_media.TabIndex = 3;
            // 
            // HKEdit_TogglePlayback
            // 
            this.HKEdit_TogglePlayback.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_TogglePlayback.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_TogglePlayback.Label = "Toggle Playback";
            this.HKEdit_TogglePlayback.Location = new System.Drawing.Point(13, 142);
            this.HKEdit_TogglePlayback.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_TogglePlayback.Name = "HKEdit_TogglePlayback";
            this.HKEdit_TogglePlayback.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_TogglePlayback.TabIndex = 3;
            // 
            // HKEdit_Prev
            // 
            this.HKEdit_Prev.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_Prev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Prev.Label = "Previous Track";
            this.HKEdit_Prev.Location = new System.Drawing.Point(13, 76);
            this.HKEdit_Prev.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Prev.Name = "HKEdit_Prev";
            this.HKEdit_Prev.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_Prev.TabIndex = 2;
            // 
            // HKEdit_Next
            // 
            this.HKEdit_Next.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_Next.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_Next.Label = "Next Track";
            this.HKEdit_Next.Location = new System.Drawing.Point(13, 10);
            this.HKEdit_Next.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_Next.Name = "HKEdit_Next";
            this.HKEdit_Next.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_Next.TabIndex = 1;
            // 
            // tab_target
            // 
            this.tab_target.Controls.Add(this.panel1);
            this.tab_target.Location = new System.Drawing.Point(0, 0);
            this.tab_target.Name = "tab_target";
            this.tab_target.Size = new System.Drawing.Size(0, 0);
            this.tab_target.Text = "Target Hotkeys";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.HKEdit_NextTarget);
            this.panel1.Controls.Add(this.HKEdit_ShowTarget);
            this.panel1.Controls.Add(this.HKEdit_PrevTarget);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(0, 0);
            this.panel1.TabIndex = 3;
            // 
            // HKEdit_NextTarget
            // 
            this.HKEdit_NextTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_NextTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_NextTarget.Label = "Next Target";
            this.HKEdit_NextTarget.Location = new System.Drawing.Point(13, 10);
            this.HKEdit_NextTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_NextTarget.Name = "HKEdit_NextTarget";
            this.HKEdit_NextTarget.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_NextTarget.TabIndex = 1;
            // 
            // HKEdit_ShowTarget
            // 
            this.HKEdit_ShowTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_ShowTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_ShowTarget.Label = "Show Current Target";
            this.HKEdit_ShowTarget.Location = new System.Drawing.Point(13, 142);
            this.HKEdit_ShowTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_ShowTarget.Name = "HKEdit_ShowTarget";
            this.HKEdit_ShowTarget.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_ShowTarget.TabIndex = 3;
            // 
            // HKEdit_PrevTarget
            // 
            this.HKEdit_PrevTarget.BackColor = System.Drawing.Color.Transparent;
            this.HKEdit_PrevTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_PrevTarget.Label = "Previous Target";
            this.HKEdit_PrevTarget.Location = new System.Drawing.Point(13, 76);
            this.HKEdit_PrevTarget.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_PrevTarget.Name = "HKEdit_PrevTarget";
            this.HKEdit_PrevTarget.Size = new System.Drawing.Size(342, 56);
            this.HKEdit_PrevTarget.TabIndex = 2;
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "VolumeControlForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Volume Control";
            this.GotFocus += new System.EventHandler(this.Window_GotFocus);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.SystemTray_ContextMenu.ResumeLayout(false);
            this.TabController.ResumeLayout(false);
            this.tab_general.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tab_volume.ResumeLayout(false);
            this.panel_volume.ResumeLayout(false);
            this.tab_media.ResumeLayout(false);
            this.panel_media.ResumeLayout(false);
            this.tab_target.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon SystemTray;
        private ContextMenuStrip SystemTray_ContextMenu;
        private ToolStripMenuItem closeToolStripMenuItem;
        private UIComposites.HotkeyEditor HKEdit_VolumeUp;
        private UIComposites.HotkeyEditor HKEdit_VolumeDown;
        private UIComposites.HotkeyEditor HKEdit_VolumeMute;
        private Manina.Windows.Forms.TabControl TabController;
        private Manina.Windows.Forms.Tab tab_general;
        private Manina.Windows.Forms.Tab tab_volume;
        private Label Label_VersionNumber;
        private Label Label_VolumeControl;
        private Manina.Windows.Forms.Tab tab_media;
        private UIComposites.HotkeyEditor HKEdit_Prev;
        private UIComposites.HotkeyEditor HKEdit_Next;
        private UIComposites.HotkeyEditor HKEdit_TogglePlayback;
        private Manina.Windows.Forms.Tab tab_target;
        private UIComposites.HotkeyEditor HKEdit_ShowTarget;
        private UIComposites.HotkeyEditor HKEdit_PrevTarget;
        private UIComposites.HotkeyEditor HKEdit_NextTarget;
        private System.Windows.Forms.Timer TargetRefreshTimer;
        private UIComposites.ToastSettings TgtSettings;
        private UIComposites.SettingsPane Settings;
        private UIComposites.TargetSelector TargetSelector;
        private Panel panel_volume;
        private Panel panel_media;
        private Panel panel1;
        private Panel panel2;
    }
}