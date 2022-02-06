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
            this.system_tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.system_tray_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkbox_minimizeOnStartup = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).BeginInit();
            this.system_tray_menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkbox_enabled
            // 
            this.checkbox_enabled.AccessibleDescription = "When checked, the volume control application\'s hotkeys are enabled.";
            this.checkbox_enabled.AccessibleName = "Enable Volume Control Checkbox";
            this.checkbox_enabled.AutoSize = true;
            this.checkbox_enabled.Checked = true;
            this.checkbox_enabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkbox_enabled.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.checkbox_enabled.Location = new System.Drawing.Point(12, 0);
            this.checkbox_enabled.Name = "checkbox_enabled";
            this.checkbox_enabled.Size = new System.Drawing.Size(73, 20);
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
            this.volume_step.Location = new System.Drawing.Point(122, 56);
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
            this.volume_step_label.Location = new System.Drawing.Point(0, 57);
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
            this.process_name_label.Location = new System.Drawing.Point(-1, 27);
            this.process_name_label.Name = "process_name_label";
            this.process_name_label.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.process_name_label.Size = new System.Drawing.Size(113, 16);
            this.process_name_label.TabIndex = 3;
            this.process_name_label.Text = "Target Process";
            // 
            // process_name
            // 
            this.process_name.Font = new System.Drawing.Font("Lucida Sans Unicode", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.process_name.Location = new System.Drawing.Point(122, 24);
            this.process_name.Name = "process_name";
            this.process_name.Size = new System.Drawing.Size(120, 26);
            this.process_name.TabIndex = 4;
            this.process_name.WordWrap = false;
            this.process_name.KeyDown += new System.Windows.Forms.KeyEventHandler(this.process_name_event);
            this.process_name.LostFocus += new System.EventHandler(this.process_name_event);
            // 
            // system_tray
            // 
            this.system_tray.BalloonTipText = "Application-Specific Volume Control Hotkeys";
            this.system_tray.BalloonTipTitle = "Volume Control";
            this.system_tray.ContextMenuStrip = this.system_tray_menu;
            this.system_tray.Icon = ((System.Drawing.Icon)(resources.GetObject("system_tray.Icon")));
            this.system_tray.Text = "Volume Control";
            this.system_tray.Visible = true;
            this.system_tray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.system_tray_event);
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
            this.checkbox_minimizeOnStartup.Location = new System.Drawing.Point(109, 1);
            this.checkbox_minimizeOnStartup.Name = "checkbox_minimizeOnStartup";
            this.checkbox_minimizeOnStartup.Size = new System.Drawing.Size(133, 19);
            this.checkbox_minimizeOnStartup.TabIndex = 5;
            this.checkbox_minimizeOnStartup.Text = "Minimize on Startup";
            this.checkbox_minimizeOnStartup.UseVisualStyleBackColor = true;
            this.checkbox_minimizeOnStartup.CheckedChanged += new System.EventHandler(this.checkbox_minimizeOnStartup_CheckedChanged);
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 90);
            this.Controls.Add(this.checkbox_minimizeOnStartup);
            this.Controls.Add(this.process_name);
            this.Controls.Add(this.process_name_label);
            this.Controls.Add(this.volume_step_label);
            this.Controls.Add(this.volume_step);
            this.Controls.Add(this.checkbox_enabled);
            this.Name = "VolumeControlForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).EndInit();
            this.system_tray_menu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox checkbox_enabled;
        private NumericUpDown volume_step;
        private Label volume_step_label;
        private Label process_name_label;
        private TextBox process_name;
        private NotifyIcon system_tray;
        private ContextMenuStrip system_tray_menu;
        private ToolStripMenuItem closeToolStripMenuItem;
        private CheckBox checkbox_minimizeOnStartup;
    }
}