namespace UIComposites
{
    partial class SettingsPane
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.cb_DarkMode = new System.Windows.Forms.CheckBox();
            this.num_VolumeStep = new UIComposites.NumberUpDownWithLabel();
            this.cb_RunAtStartup = new System.Windows.Forms.CheckBox();
            this.cb_MinimizeOnStartup = new System.Windows.Forms.CheckBox();
            this.cb_ShowInTaskbar = new System.Windows.Forms.CheckBox();
            this.cb_AlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.panel = new System.Windows.Forms.Panel();
            this.SettingsGroupBox.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsGroupBox
            // 
            this.SettingsGroupBox.BackColor = System.Drawing.SystemColors.Window;
            this.SettingsGroupBox.Controls.Add(this.cb_DarkMode);
            this.SettingsGroupBox.Controls.Add(this.num_VolumeStep);
            this.SettingsGroupBox.Controls.Add(this.cb_RunAtStartup);
            this.SettingsGroupBox.Controls.Add(this.cb_MinimizeOnStartup);
            this.SettingsGroupBox.Controls.Add(this.cb_ShowInTaskbar);
            this.SettingsGroupBox.Controls.Add(this.cb_AlwaysOnTop);
            this.SettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsGroupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SettingsGroupBox.Location = new System.Drawing.Point(2, 2);
            this.SettingsGroupBox.Name = "SettingsGroupBox";
            this.SettingsGroupBox.Size = new System.Drawing.Size(142, 151);
            this.SettingsGroupBox.TabIndex = 0;
            this.SettingsGroupBox.TabStop = false;
            this.SettingsGroupBox.Text = "Settings";
            // 
            // cb_DarkMode
            // 
            this.cb_DarkMode.AutoSize = true;
            this.cb_DarkMode.BackColor = System.Drawing.Color.Transparent;
            this.cb_DarkMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_DarkMode.Location = new System.Drawing.Point(6, 100);
            this.cb_DarkMode.Name = "cb_DarkMode";
            this.cb_DarkMode.Size = new System.Drawing.Size(119, 19);
            this.cb_DarkMode.TabIndex = 5;
            this.cb_DarkMode.Text = "Enable Dark Mode";
            this.cb_DarkMode.UseVisualStyleBackColor = false;
            // 
            // num_VolumeStep
            // 
            this.num_VolumeStep.AutoSize = true;
            this.num_VolumeStep.BackColor = System.Drawing.Color.Transparent;
            this.num_VolumeStep.ForeColor = System.Drawing.Color.Transparent;
            this.num_VolumeStep.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_VolumeStep.LabelText = "Volume Step";
            this.num_VolumeStep.Location = new System.Drawing.Point(6, 120);
            this.num_VolumeStep.MaxValue = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.num_VolumeStep.MinValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.num_VolumeStep.Name = "num_VolumeStep";
            this.num_VolumeStep.NumberUpDownWidth = 50;
            this.num_VolumeStep.Size = new System.Drawing.Size(133, 27);
            this.num_VolumeStep.TabIndex = 4;
            this.num_VolumeStep.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // cb_RunAtStartup
            // 
            this.cb_RunAtStartup.AutoSize = true;
            this.cb_RunAtStartup.BackColor = System.Drawing.Color.Transparent;
            this.cb_RunAtStartup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_RunAtStartup.Location = new System.Drawing.Point(6, 20);
            this.cb_RunAtStartup.Name = "cb_RunAtStartup";
            this.cb_RunAtStartup.Size = new System.Drawing.Size(98, 19);
            this.cb_RunAtStartup.TabIndex = 3;
            this.cb_RunAtStartup.Text = "Run at Startup";
            this.cb_RunAtStartup.UseVisualStyleBackColor = false;
            // 
            // cb_MinimizeOnStartup
            // 
            this.cb_MinimizeOnStartup.AutoSize = true;
            this.cb_MinimizeOnStartup.BackColor = System.Drawing.Color.Transparent;
            this.cb_MinimizeOnStartup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_MinimizeOnStartup.Location = new System.Drawing.Point(6, 40);
            this.cb_MinimizeOnStartup.Name = "cb_MinimizeOnStartup";
            this.cb_MinimizeOnStartup.Size = new System.Drawing.Size(130, 19);
            this.cb_MinimizeOnStartup.TabIndex = 2;
            this.cb_MinimizeOnStartup.Text = "Minimize on Startup";
            this.cb_MinimizeOnStartup.UseVisualStyleBackColor = false;
            // 
            // cb_ShowInTaskbar
            // 
            this.cb_ShowInTaskbar.AutoSize = true;
            this.cb_ShowInTaskbar.BackColor = System.Drawing.Color.Transparent;
            this.cb_ShowInTaskbar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_ShowInTaskbar.Location = new System.Drawing.Point(6, 60);
            this.cb_ShowInTaskbar.Name = "cb_ShowInTaskbar";
            this.cb_ShowInTaskbar.Size = new System.Drawing.Size(107, 19);
            this.cb_ShowInTaskbar.TabIndex = 1;
            this.cb_ShowInTaskbar.Text = "Show In Taskbar";
            this.cb_ShowInTaskbar.UseVisualStyleBackColor = false;
            // 
            // cb_AlwaysOnTop
            // 
            this.cb_AlwaysOnTop.AutoSize = true;
            this.cb_AlwaysOnTop.BackColor = System.Drawing.Color.Transparent;
            this.cb_AlwaysOnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_AlwaysOnTop.Location = new System.Drawing.Point(6, 80);
            this.cb_AlwaysOnTop.Name = "cb_AlwaysOnTop";
            this.cb_AlwaysOnTop.Size = new System.Drawing.Size(99, 19);
            this.cb_AlwaysOnTop.TabIndex = 0;
            this.cb_AlwaysOnTop.Text = "Always on Top";
            this.cb_AlwaysOnTop.UseMnemonic = false;
            this.cb_AlwaysOnTop.UseVisualStyleBackColor = false;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.SettingsGroupBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.ForeColor = System.Drawing.Color.Transparent;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Padding = new System.Windows.Forms.Padding(2);
            this.panel.Size = new System.Drawing.Size(146, 155);
            this.panel.TabIndex = 1;
            // 
            // SettingsPane
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Controls.Add(this.panel);
            this.Name = "SettingsPane";
            this.Size = new System.Drawing.Size(146, 155);
            this.SettingsGroupBox.ResumeLayout(false);
            this.SettingsGroupBox.PerformLayout();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox SettingsGroupBox;
        private CheckBox cb_AlwaysOnTop;
        private Panel panel;
        private CheckBox cb_RunAtStartup;
        private CheckBox cb_MinimizeOnStartup;
        private CheckBox cb_ShowInTaskbar;
        private NumberUpDownWithLabel num_VolumeStep;
        private CheckBox cb_DarkMode;
    }
}
