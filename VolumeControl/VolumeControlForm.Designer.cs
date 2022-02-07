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
            HotkeyLib.Hotkey hotkey4 = new HotkeyLib.Hotkey();
            HotkeyLib.Hotkey hotkey5 = new HotkeyLib.Hotkey();
            HotkeyLib.Hotkey hotkey6 = new HotkeyLib.Hotkey();
            this.volume_step = new System.Windows.Forms.NumericUpDown();
            this.volume_step_label = new System.Windows.Forms.Label();
            this.process_name_label = new System.Windows.Forms.Label();
            this.system_tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.system_tray_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkbox_minimizeOnStartup = new System.Windows.Forms.CheckBox();
            this.ComboBox_ProcessSelector = new System.Windows.Forms.ComboBox();
            this.HKEdit_VolumeUp = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeDown = new UIComposites.HotkeyEditor();
            this.HKEdit_VolumeMute = new UIComposites.HotkeyEditor();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Tab_General = new System.Windows.Forms.TabPage();
            this.Tab_Hotkeys = new System.Windows.Forms.TabPage();
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).BeginInit();
            this.system_tray_menu.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Tab_General.SuspendLayout();
            this.Tab_Hotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // volume_step
            // 
            this.volume_step.AccessibleDescription = "Defines how much the volume increases or decreases when a hotkey is pressed.";
            this.volume_step.AccessibleName = "Volume Step Control";
            this.volume_step.AutoSize = true;
            this.volume_step.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.volume_step.Location = new System.Drawing.Point(112, 56);
            this.volume_step.Name = "volume_step";
            this.volume_step.Size = new System.Drawing.Size(60, 26);
            this.volume_step.TabIndex = 1;
            this.volume_step.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.volume_step.ValueChanged += new System.EventHandler(this.volume_step_event);
            this.volume_step.LostFocus += new System.EventHandler(this.volume_step_event);
            // 
            // volume_step_label
            // 
            this.volume_step_label.AutoSize = true;
            this.volume_step_label.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.volume_step_label.Location = new System.Drawing.Point(6, 58);
            this.volume_step_label.Name = "volume_step_label";
            this.volume_step_label.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.volume_step_label.Size = new System.Drawing.Size(86, 16);
            this.volume_step_label.TabIndex = 2;
            this.volume_step_label.Text = "Volume Step";
            // 
            // process_name_label
            // 
            this.process_name_label.AutoSize = true;
            this.process_name_label.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.process_name_label.Location = new System.Drawing.Point(6, 29);
            this.process_name_label.Name = "process_name_label";
            this.process_name_label.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.process_name_label.Size = new System.Drawing.Size(100, 16);
            this.process_name_label.TabIndex = 3;
            this.process_name_label.Text = "Target Process";
            // 
            // system_tray
            // 
            this.system_tray.BalloonTipText = "Application-Specific Volume Control Hotkeys";
            this.system_tray.BalloonTipTitle = "Volume Control";
            this.system_tray.ContextMenuStrip = this.system_tray_menu;
            this.system_tray.Icon = ((System.Drawing.Icon)(resources.GetObject("system_tray.Icon")));
            this.system_tray.Text = "Volume Control";
            this.system_tray.Visible = true;
            this.system_tray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.system_tray_double_click);
            // 
            // system_tray_menu
            // 
            this.system_tray_menu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.system_tray_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.system_tray_menu.Name = "system_tray_menu";
            this.system_tray_menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.system_tray_menu.Size = new System.Drawing.Size(104, 26);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.system_tray_menu_close);
            // 
            // checkbox_minimizeOnStartup
            // 
            this.checkbox_minimizeOnStartup.AutoSize = true;
            this.checkbox_minimizeOnStartup.Checked = true;
            this.checkbox_minimizeOnStartup.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkbox_minimizeOnStartup.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkbox_minimizeOnStartup.Location = new System.Drawing.Point(189, 58);
            this.checkbox_minimizeOnStartup.Name = "checkbox_minimizeOnStartup";
            this.checkbox_minimizeOnStartup.Size = new System.Drawing.Size(140, 20);
            this.checkbox_minimizeOnStartup.TabIndex = 5;
            this.checkbox_minimizeOnStartup.Text = "Minimize on Startup";
            this.checkbox_minimizeOnStartup.UseVisualStyleBackColor = true;
            this.checkbox_minimizeOnStartup.CheckedChanged += new System.EventHandler(this.checkbox_minimizeOnStartup_CheckedChanged);
            // 
            // ComboBox_ProcessSelector
            // 
            this.ComboBox_ProcessSelector.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ComboBox_ProcessSelector.Location = new System.Drawing.Point(112, 26);
            this.ComboBox_ProcessSelector.MaxDropDownItems = 16;
            this.ComboBox_ProcessSelector.Name = "ComboBox_ProcessSelector";
            this.ComboBox_ProcessSelector.Size = new System.Drawing.Size(232, 24);
            this.ComboBox_ProcessSelector.Sorted = true;
            this.ComboBox_ProcessSelector.TabIndex = 6;
            this.ComboBox_ProcessSelector.TextChanged += new System.EventHandler(this.process_name_text_changed);
            // 
            // HKEdit_VolumeUp
            // 
            this.HKEdit_VolumeUp.Alt = false;
            this.HKEdit_VolumeUp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeUp.Ctrl = false;
            hotkey4.Alt = false;
            hotkey4.Control = false;
            hotkey4.KeyCode = System.Windows.Forms.Keys.None;
            hotkey4.Shift = false;
            hotkey4.Windows = false;
            this.HKEdit_VolumeUp.Hotkey = hotkey4;
            this.HKEdit_VolumeUp.IsEnabled = false;
            this.HKEdit_VolumeUp.Key = System.Windows.Forms.Keys.None;
            this.HKEdit_VolumeUp.Label = "NULL";
            this.HKEdit_VolumeUp.Location = new System.Drawing.Point(9, 6);
            this.HKEdit_VolumeUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeUp.Name = "HKEdit_VolumeUp";
            this.HKEdit_VolumeUp.Shift = false;
            this.HKEdit_VolumeUp.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeUp.TabIndex = 7;
            this.HKEdit_VolumeUp.Win = false;
            this.HKEdit_VolumeUp.HotkeyChanged += new System.EventHandler(this.HKEdit_VolumeUp_HotkeyChanged);
            // 
            // HKEdit_VolumeDown
            // 
            this.HKEdit_VolumeDown.Alt = false;
            this.HKEdit_VolumeDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeDown.Ctrl = false;
            hotkey5.Alt = false;
            hotkey5.Control = false;
            hotkey5.KeyCode = System.Windows.Forms.Keys.None;
            hotkey5.Shift = false;
            hotkey5.Windows = false;
            this.HKEdit_VolumeDown.Hotkey = hotkey5;
            this.HKEdit_VolumeDown.IsEnabled = false;
            this.HKEdit_VolumeDown.Key = System.Windows.Forms.Keys.None;
            this.HKEdit_VolumeDown.Label = "NULL";
            this.HKEdit_VolumeDown.Location = new System.Drawing.Point(9, 79);
            this.HKEdit_VolumeDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeDown.Name = "HKEdit_VolumeDown";
            this.HKEdit_VolumeDown.Shift = false;
            this.HKEdit_VolumeDown.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeDown.TabIndex = 8;
            this.HKEdit_VolumeDown.Win = false;
            this.HKEdit_VolumeDown.HotkeyChanged += new System.EventHandler(this.HKEdit_VolumeDown_HotkeyChanged);
            // 
            // HKEdit_VolumeMute
            // 
            this.HKEdit_VolumeMute.Alt = false;
            this.HKEdit_VolumeMute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.HKEdit_VolumeMute.Ctrl = false;
            hotkey6.Alt = false;
            hotkey6.Control = false;
            hotkey6.KeyCode = System.Windows.Forms.Keys.None;
            hotkey6.Shift = false;
            hotkey6.Windows = false;
            this.HKEdit_VolumeMute.Hotkey = hotkey6;
            this.HKEdit_VolumeMute.IsEnabled = false;
            this.HKEdit_VolumeMute.Key = System.Windows.Forms.Keys.None;
            this.HKEdit_VolumeMute.Label = "NULL";
            this.HKEdit_VolumeMute.Location = new System.Drawing.Point(9, 151);
            this.HKEdit_VolumeMute.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.HKEdit_VolumeMute.Name = "HKEdit_VolumeMute";
            this.HKEdit_VolumeMute.Shift = false;
            this.HKEdit_VolumeMute.Size = new System.Drawing.Size(343, 66);
            this.HKEdit_VolumeMute.TabIndex = 9;
            this.HKEdit_VolumeMute.Win = false;
            this.HKEdit_VolumeMute.HotkeyChanged += new System.EventHandler(this.HKEdit_VolumeMute_HotkeyChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Tab_General);
            this.tabControl1.Controls.Add(this.Tab_Hotkeys);
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
            this.Tab_General.Controls.Add(this.checkbox_minimizeOnStartup);
            this.Tab_General.Controls.Add(this.volume_step);
            this.Tab_General.Controls.Add(this.ComboBox_ProcessSelector);
            this.Tab_General.Controls.Add(this.process_name_label);
            this.Tab_General.Controls.Add(this.volume_step_label);
            this.Tab_General.Location = new System.Drawing.Point(4, 25);
            this.Tab_General.Name = "Tab_General";
            this.Tab_General.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_General.Size = new System.Drawing.Size(360, 224);
            this.Tab_General.TabIndex = 0;
            this.Tab_General.Text = "General";
            this.Tab_General.UseVisualStyleBackColor = true;
            // 
            // Tab_Hotkeys
            // 
            this.Tab_Hotkeys.Controls.Add(this.HKEdit_VolumeUp);
            this.Tab_Hotkeys.Controls.Add(this.HKEdit_VolumeMute);
            this.Tab_Hotkeys.Controls.Add(this.HKEdit_VolumeDown);
            this.Tab_Hotkeys.Location = new System.Drawing.Point(4, 25);
            this.Tab_Hotkeys.Name = "Tab_Hotkeys";
            this.Tab_Hotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.Tab_Hotkeys.Size = new System.Drawing.Size(360, 224);
            this.Tab_Hotkeys.TabIndex = 1;
            this.Tab_Hotkeys.Text = "Hotkeys";
            this.Tab_Hotkeys.UseVisualStyleBackColor = true;
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
            this.GotFocus += new System.EventHandler(this.window_got_focus_event);
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).EndInit();
            this.system_tray_menu.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.Tab_General.ResumeLayout(false);
            this.Tab_General.PerformLayout();
            this.Tab_Hotkeys.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private NumericUpDown volume_step;
        private Label volume_step_label;
        private Label process_name_label;
        private NotifyIcon system_tray;
        private ContextMenuStrip system_tray_menu;
        private ToolStripMenuItem closeToolStripMenuItem;
        private CheckBox checkbox_minimizeOnStartup;
        private ComboBox ComboBox_ProcessSelector;
        private UIComposites.HotkeyEditor HKEdit_VolumeUp;
        private UIComposites.HotkeyEditor HKEdit_VolumeDown;
        private UIComposites.HotkeyEditor HKEdit_VolumeMute;
        private TabControl tabControl1;
        private TabPage Tab_General;
        private TabPage Tab_Hotkeys;
    }
}