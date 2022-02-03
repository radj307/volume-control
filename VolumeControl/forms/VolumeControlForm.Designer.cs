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
            this.checkbox_enabled = new System.Windows.Forms.CheckBox();
            this.volume_step = new System.Windows.Forms.NumericUpDown();
            this.volume_step_label = new System.Windows.Forms.Label();
            this.process_name_label = new System.Windows.Forms.Label();
            this.process_name = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).BeginInit();
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
            this.checkbox_enabled.Size = new System.Drawing.Size(255, 20);
            this.checkbox_enabled.TabIndex = 0;
            this.checkbox_enabled.Text = "Enabled";
            this.checkbox_enabled.UseVisualStyleBackColor = true;
            this.checkbox_enabled.CheckedChanged += new System.EventHandler(this.checkbox_enabled_CheckedChanged);
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
            // 
            // VolumeControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 90);
            this.Controls.Add(this.process_name);
            this.Controls.Add(this.process_name_label);
            this.Controls.Add(this.volume_step_label);
            this.Controls.Add(this.volume_step);
            this.Controls.Add(this.checkbox_enabled);
            this.Name = "VolumeControlForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.volume_step)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox checkbox_enabled;
        private NumericUpDown volume_step;
        private Label volume_step_label;
        private Label process_name_label;
        private TextBox process_name;
    }
}