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
            this.checkbox_enabled = new System.Windows.Forms.CheckBox();
            this.volume_step = new System.Windows.Forms.NumericUpDown();
            this.volume_step_label = new System.Windows.Forms.Label();
            this.process_name_label = new System.Windows.Forms.Label();
            this.process_name = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.General = new System.Windows.Forms.TabPage();
            this.Hotkeys = new System.Windows.Forms.TabPage();
            this.hkedit_volumemute = new UIComponents.HotkeyEditor();
            this.hkedit_volumedown = new UIComponents.HotkeyEditor();
            this.hkedit_volumeup = new UIComponents.HotkeyEditor();
            this.button_refresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.General.SuspendLayout();
            this.Hotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkbox_enabled
            // 
            this.checkbox_enabled.AccessibleDescription = "When checked, the volume control application\'s hotkeys are enabled.";
            this.checkbox_enabled.AccessibleName = "Enable Volume Control Checkbox";
            this.checkbox_enabled.AutoSize = true;
            this.checkbox_enabled.Checked = true;
            this.checkbox_enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkbox_enabled.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkbox_enabled.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkbox_enabled.Location = new System.Drawing.Point(0, 0);
            this.checkbox_enabled.Name = "checkbox_enabled";
            this.checkbox_enabled.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.checkbox_enabled.Size = new System.Drawing.Size(526, 20);
            this.checkbox_enabled.TabIndex = 0;
            this.checkbox_enabled.Text = "Enabled";
            this.checkbox_enabled.UseVisualStyleBackColor = true;
            this.checkbox_enabled.CheckedChanged += new System.EventHandler(this.checkbox_enabled_event);
            // 
            // volume_step
            // 
            this.volume_step.AccessibleDescription = "Defines how much the volume increases or decreases when a hotkey is pressed.";
            this.volume_step.AccessibleName = "Volume Step Control";
            this.volume_step.AutoSize = true;
            this.volume_step.Location = new System.Drawing.Point(129, 54);
            this.volume_step.Name = "volume_step";
            this.volume_step.Size = new System.Drawing.Size(120, 23);
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
            this.volume_step_label.Location = new System.Drawing.Point(7, 55);
            this.volume_step_label.Name = "volume_step_label";
            this.volume_step_label.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.volume_step_label.Size = new System.Drawing.Size(99, 16);
            this.volume_step_label.TabIndex = 2;
            this.volume_step_label.Text = "Volume Step";
            // 
            // process_name_label
            // 
            this.process_name_label.AutoSize = true;
            this.process_name_label.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.process_name_label.Location = new System.Drawing.Point(6, 25);
            this.process_name_label.Name = "process_name_label";
            this.process_name_label.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.process_name_label.Size = new System.Drawing.Size(113, 16);
            this.process_name_label.TabIndex = 3;
            this.process_name_label.Text = "Target Process";
            // 
            // process_name
            // 
            this.process_name.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.process_name.Location = new System.Drawing.Point(129, 22);
            this.process_name.Name = "process_name";
            this.process_name.Size = new System.Drawing.Size(120, 26);
            this.process_name.TabIndex = 4;
            this.process_name.WordWrap = false;
            this.process_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.process_name_event);
            this.process_name.LostFocus += new System.EventHandler(this.process_name_event);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Application-Specific Volume Control Hotkeys";
            this.notifyIcon1.BalloonTipTitle = "Volume Control";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Volume Control";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_event);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.General);
            this.tabControl1.Controls.Add(this.Hotkeys);
            this.tabControl1.Location = new System.Drawing.Point(12, 26);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(513, 130);
            this.tabControl1.TabIndex = 8;
            // 
            // General
            // 
            this.General.Controls.Add(this.process_name);
            this.General.Controls.Add(this.volume_step);
            this.General.Controls.Add(this.process_name_label);
            this.General.Controls.Add(this.volume_step_label);
            this.General.Location = new System.Drawing.Point(4, 24);
            this.General.Name = "General";
            this.General.Padding = new System.Windows.Forms.Padding(3);
            this.General.Size = new System.Drawing.Size(505, 102);
            this.General.TabIndex = 1;
            this.General.Text = "General";
            this.General.UseVisualStyleBackColor = true;
            // 
            // Hotkeys
            // 
            this.Hotkeys.Controls.Add(this.hkedit_volumemute);
            this.Hotkeys.Controls.Add(this.hkedit_volumedown);
            this.Hotkeys.Controls.Add(this.hkedit_volumeup);
            this.Hotkeys.Location = new System.Drawing.Point(4, 24);
            this.Hotkeys.Name = "Hotkeys";
            this.Hotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.Hotkeys.Size = new System.Drawing.Size(192, 72);
            this.Hotkeys.TabIndex = 0;
            this.Hotkeys.Text = "Hotkeys";
            this.Hotkeys.UseVisualStyleBackColor = true;
            // 
            // hkedit_volumemute
            // 
            this.hkedit_volumemute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hkedit_volumemute.Enabled = false;
            this.hkedit_volumemute.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hkedit_volumemute.Location = new System.Drawing.Point(6, 66);
            this.hkedit_volumemute.Name = "hkedit_volumemute";
            this.hkedit_volumemute.Size = new System.Drawing.Size(500, 24);
            this.hkedit_volumemute.TabIndex = 2;
            this.hkedit_volumemute.LostFocus += new System.EventHandler(this.hkedit_volumemute_update);
            // 
            // hkedit_volumedown
            // 
            this.hkedit_volumedown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hkedit_volumedown.Enabled = false;
            this.hkedit_volumedown.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hkedit_volumedown.Location = new System.Drawing.Point(6, 36);
            this.hkedit_volumedown.Name = "hkedit_volumedown";
            this.hkedit_volumedown.Size = new System.Drawing.Size(500, 24);
            this.hkedit_volumedown.TabIndex = 1;
            this.hkedit_volumedown.LostFocus += new System.EventHandler(this.hkedit_volumedown_update);
            // 
            // hkedit_volumeup
            // 
            this.hkedit_volumeup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.hkedit_volumeup.Enabled = false;
            this.hkedit_volumeup.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.hkedit_volumeup.Location = new System.Drawing.Point(6, 6);
            this.hkedit_volumeup.Name = "hkedit_volumeup";
            this.hkedit_volumeup.Size = new System.Drawing.Size(500, 24);
            this.hkedit_volumeup.TabIndex = 0;
            this.hkedit_volumeup.LostFocus += new System.EventHandler(this.hkedit_volumeup_update);
            // 
            // button_refresh
            // 
            this.button_refresh.Location = new System.Drawing.Point(464, 2);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(59, 24);
            this.button_refresh.TabIndex = 9;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 161);
            this.Controls.Add(this.button_refresh);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.checkbox_enabled);
            this.Name = "VolumeControlForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.General.ResumeLayout(false);
            this.General.PerformLayout();
            this.Hotkeys.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox checkbox_enabled;
        private NumericUpDown volume_step;
        private Label volume_step_label;
        private Label process_name_label;
        private TextBox process_name;
        private NotifyIcon notifyIcon1;
        private TabControl tabControl1;
        private TabPage Hotkeys;
        private UIComponents.HotkeyEditor hkedit_volumeup;
        private UIComponents.HotkeyEditor hkedit_volumemute;
        private UIComponents.HotkeyEditor hkedit_volumedown;
        private TabPage General;
        private Button button_refresh;
    }
}